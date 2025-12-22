using Assets;
using Blizzard.T5.AssetManager;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SoundManager : IService, IHasFixedUpdate, IHasUpdate
{
  private static SoundManager s_instance;
  private SoundConfig m_config;
  private List<AudioSource> m_generatedSources = new List<AudioSource>();
  private List<SoundManager.ExtensionMapping> m_extensionMappings = new List<SoundManager.ExtensionMapping>();
  private Map<Global.SoundCategory, List<AudioSource>> m_sourcesByCategory = new Map<Global.SoundCategory, List<AudioSource>>();
  private Map<string, List<AudioSource>> m_sourcesByClipName = new Map<string, List<AudioSource>>();
  private Map<string, SoundManager.BundleInfo> m_bundleInfos = new Map<string, SoundManager.BundleInfo>();
  private Map<Global.SoundCategory, List<SoundManager.DuckState>> m_duckStates = new Map<Global.SoundCategory, List<SoundManager.DuckState>>();
  private uint m_nextDuckStateTweenId;
  private List<AudioSource> m_inactiveSources = new List<AudioSource>();
  private List<string> m_bundleInfosToRemove = new List<string>();
  private GameObject m_sceneObject;
  private Map<string, int> activeLimitedSounds = new Map<string, int>();
  private List<MusicTrack> m_musicTracks = new List<MusicTrack>();
  private List<MusicTrack> m_ambienceTracks = new List<MusicTrack>();
  private bool m_musicIsAboutToPlay;
  private bool m_ambienceIsAboutToPlay;
  private AudioSource m_currentMusicTrack;
  private AudioSource m_currentAmbienceTrack;
  private List<AudioSource> m_fadingTracks = new List<AudioSource>();
  private float m_musicTrackStartTime;
  private int m_musicTrackIndex;
  private int m_nextMusicTrackIndex;
  private int m_ambienceTrackIndex;
  private bool m_isMasterEnabled;
  private bool m_isMusicEnabled;
  private bool m_mute;
  private int m_nextSourceId = 1;
  private uint m_frame;
  private List<Coroutine> m_fadingTracksIn = new List<Coroutine>();
  public static readonly AssetReference FallbackSound = new AssetReference("tavern_crowd_play_reaction_very_positive_2.wav:07343a9a2cec38942b8fdbbafa9165d7");
  public Action OnMusicStarted;

  private GameObject SceneObject
  {
    get
    {
      if ((UnityEngine.Object) this.m_sceneObject == (UnityEngine.Object) null)
        this.m_sceneObject = new GameObject("SoundManagerSceneObject", new System.Type[1]
        {
          typeof (HSDontDestroyOnLoad)
        });
      return this.m_sceneObject;
    }
  }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    SoundManager soundManager = this;
    InstantiatePrefab instantiateSoundConfigPrefab = new InstantiatePrefab((AssetReference) "SoundConfig.prefab:cd41c731c777d4f468b79ffa365a9f94");
    yield return (IAsyncJobResult) instantiateSoundConfigPrefab;
    soundManager.SetConfig(instantiateSoundConfigPrefab.InstantiatedPrefab.GetComponent<SoundConfig>());
    soundManager.SetMonoSoundOption(Options.Get().GetBool(Option.SOUND_MONO_ENABLED));
    Options.Get().RegisterChangedListener(Option.SOUND, new Options.ChangedCallback(soundManager.OnMasterEnabledOptionChanged));
    Options.Get().RegisterChangedListener(Option.SOUND_VOLUME, new Options.ChangedCallback(soundManager.OnMasterVolumeOptionChanged));
    Options.Get().RegisterChangedListener(Option.MUSIC, new Options.ChangedCallback(soundManager.OnEnabledOptionChanged));
    Options.Get().RegisterChangedListener(Option.MUSIC_VOLUME, new Options.ChangedCallback(soundManager.OnVolumeOptionChanged));
    Options.Get().RegisterChangedListener(Option.BACKGROUND_SOUND, new Options.ChangedCallback(soundManager.OnBackgroundSoundOptionChanged));
    Options.Get().RegisterChangedListener(Option.SOUND_MONO_ENABLED, new Options.ChangedCallback(soundManager.OnMonoSoundOptionChanged));
    soundManager.m_isMasterEnabled = Options.Get().GetBool(Option.SOUND);
    soundManager.m_isMusicEnabled = Options.Get().GetBool(Option.MUSIC);
    soundManager.SetMasterVolumeExponential();
    soundManager.UpdateAppMute();
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
      hearthstoneApplication.AddFocusChangedListener(new HearthstoneApplication.FocusChangedCallback(soundManager.OnAppFocusChanged));
    AudioSettings.OnAudioConfigurationChanged += new AudioSettings.AudioConfigurationChangeHandler(soundManager.OnAudioConfigurationChanged);
    yield return (IAsyncJobResult) new ServiceSoftDependency(typeof (SceneMgr), serviceLocator);
    SceneMgr service;
    if (serviceLocator.TryGetService<SceneMgr>(out service))
      service.RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(soundManager.OnSceneLoaded));
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (IAssetLoader)
  };

  public void Shutdown()
  {
    AudioSettings.OnAudioConfigurationChanged -= new AudioSettings.AudioConfigurationChangeHandler(this.OnAudioConfigurationChanged);
    SoundManager.s_instance = (SoundManager) null;
  }

  public void Update()
  {
    this.m_frame = (uint) ((int) this.m_frame + 1 & -1);
    this.UpdateMusicAndAmbience();
  }

  public void FixedUpdate() => this.UpdateSources();

  public float GetSecondsBetweenUpdates() => 1f;

  public static SoundManager Get()
  {
    if (SoundManager.s_instance == null)
      SoundManager.s_instance = ServiceManager.Get<SoundManager>();
    return SoundManager.s_instance;
  }

  public SoundConfig GetConfig() => this.m_config;

  public void SetConfig(SoundConfig config) => this.m_config = config;

  public bool IsInitialized() => (UnityEngine.Object) this.m_config != (UnityEngine.Object) null;

  public GameObject GetPlaceholderSound()
  {
    AudioSource placeholderSource = this.GetPlaceholderSource();
    return (UnityEngine.Object) placeholderSource == (UnityEngine.Object) null ? (GameObject) null : placeholderSource.gameObject;
  }

  public AudioSource GetPlaceholderSource()
  {
    if ((UnityEngine.Object) this.m_config == (UnityEngine.Object) null)
      return (AudioSource) null;
    return HearthstoneApplication.IsInternal() ? this.m_config.m_PlaceholderSound : (AudioSource) null;
  }

  public SoundDef GetSoundDef(AudioSource source) => source.gameObject.GetComponent<SoundDef>();

  private void SetMasterVolumeExponential() => AudioListener.volume = Mathf.Pow(Mathf.Clamp01(Options.Get().GetFloat(Option.SOUND_VOLUME)), 1.75f);

  private void SetMonoSoundOption(bool enabled) => AudioSettings.Reset(AudioSettings.GetConfiguration() with
  {
    speakerMode = enabled ? AudioSpeakerMode.Mono : AudioSpeakerMode.Stereo
  });

  public bool Play(
    AudioSource source,
    SoundDef oneShotDef = null,
    AudioClip oneShotClip = null,
    SoundManager.SoundOptions options = null)
  {
    return (bool) (UnityEngine.Object) this.PlayImpl(source, oneShotDef, oneShotClip, options);
  }

  public void RegisterVideoSoundSource(AudioSource source, SoundDef def = null) => this.RegisterExtensionForVideoSource(source, def);

  public bool PlayOneShot(
    AudioSource source,
    SoundDef oneShotDef,
    float volume = 1f,
    SoundManager.SoundOptions options = null)
  {
    if (!(bool) (UnityEngine.Object) this.PlayImpl(source, oneShotDef, additionalSettings: options))
      return false;
    if (this.IsActive(source))
      this.SetVolume(source, volume);
    return true;
  }

  public bool IsPlaying(AudioSource source) => !((UnityEngine.Object) source == (UnityEngine.Object) null) && source.isPlaying;

  public bool Pause(AudioSource source)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null || this.IsPaused(source))
      return false;
    SoundManager.SourceExtension ext = this.RegisterExtension(source);
    if (ext == null)
      return false;
    ext.m_paused = true;
    this.UpdateSource(source, ext);
    source.Pause();
    return true;
  }

  public bool IsPaused(AudioSource source)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
      return false;
    SoundManager.SourceExtension extension = this.GetExtension(source);
    return extension != null && extension.m_paused;
  }

  public bool Stop(AudioSource source)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null || !this.IsActive(source))
      return false;
    source.Stop();
    this.FinishSource(source);
    return true;
  }

  public void Destroy(AudioSource source)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
      return;
    this.FinishSource(source);
  }

  public void DestroyAll(Global.SoundCategory category)
  {
    List<AudioSource> audioSourceList = new List<AudioSource>();
    for (int index = 0; index < this.m_generatedSources.Count; ++index)
    {
      AudioSource generatedSource = this.m_generatedSources[index];
      SoundDef component = generatedSource.GetComponent<SoundDef>();
      if (component.m_Category == category && !component.m_persistPastGameEnd)
        audioSourceList.Add(generatedSource);
    }
    foreach (AudioSource source in audioSourceList)
      this.Destroy(source);
  }

  public bool IsActive(AudioSource source) => !((UnityEngine.Object) source == (UnityEngine.Object) null) && (this.IsPlaying(source) || this.IsPaused(source));

  public bool IsPlaybackFinished(AudioSource source) => !((UnityEngine.Object) source == (UnityEngine.Object) null) && !((UnityEngine.Object) source.clip == (UnityEngine.Object) null) && source.timeSamples >= source.clip.samples;

  public float GetVolume(AudioSource source)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
      return 1f;
    SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source);
    return sourceExtension == null ? 1f : sourceExtension.m_codeVolume;
  }

  public void SetVolume(AudioSource source, float volume)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
      return;
    SoundManager.SourceExtension ext = this.RegisterExtension(source);
    if (ext == null)
      return;
    ext.m_codeVolume = volume;
    this.UpdateVolume(source, ext);
  }

  public void SetPitch(AudioSource source, float pitch)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
      return;
    SoundManager.SourceExtension ext = this.RegisterExtension(source);
    if (ext == null)
      return;
    ext.m_codePitch = pitch;
    this.UpdatePitch(source, ext);
  }

  public Global.SoundCategory GetCategory(AudioSource source) => (UnityEngine.Object) source == (UnityEngine.Object) null ? Global.SoundCategory.NONE : this.GetDefFromSource(source).m_Category;

  public void Set3d(AudioSource source, bool enable)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
      return;
    source.spatialBlend = enable ? 1f : 0.0f;
  }

  public AudioSource GetCurrentMusicTrack() => this.m_currentMusicTrack;

  public bool Load(AssetReference assetRef) => AssetLoader.Get().IsAssetAvailable(assetRef) && SoundLoader.LoadSound(assetRef, new PrefabCallback<GameObject>(this.OnLoadSoundLoaded));

  public void LoadAndPlay(AssetReference assetRef) => this.LoadAndPlay(assetRef, (GameObject) null, 1f, (SoundManager.LoadedCallback) null, (object) null);

  public void LoadAndPlay(AssetReference assetRef, GameObject parent) => this.LoadAndPlay(assetRef, parent, 1f, (SoundManager.LoadedCallback) null, (object) null);

  public void LoadAndPlay(AssetReference assetRef, GameObject parent, float volume) => this.LoadAndPlay(assetRef, parent, volume, (SoundManager.LoadedCallback) null, (object) null);

  public void LoadAndPlay(
    AssetReference assetRef,
    GameObject parent,
    float volume,
    SoundManager.LoadedCallback callback)
  {
    this.LoadAndPlay(assetRef, parent, volume, callback, (object) null);
  }

  public void LoadAndPlay(
    AssetReference assetRef,
    GameObject parent,
    float volume,
    SoundManager.LoadedCallback callback,
    object callbackData)
  {
    if (string.IsNullOrEmpty((string) assetRef))
    {
      Log.Sound.PrintWarning("Missing assetref for LoadAndPlay().");
      if (callback == null)
        return;
      callback((AudioSource) null, callbackData);
    }
    else
    {
      SoundManager.SoundLoadContext callbackData1 = new SoundManager.SoundLoadContext();
      callbackData1.Init(parent, volume, callback, callbackData);
      SoundLoader.LoadSound(assetRef, new PrefabCallback<GameObject>(this.OnLoadAndPlaySoundLoaded), (object) callbackData1, this.GetPlaceholderSound());
    }
  }

  public void PlayPreloaded(AudioSource source) => this.PlayPreloaded(source, (GameObject) null);

  public void PlayPreloaded(AudioSource source, float volume) => this.PlayPreloaded(source, (GameObject) null, volume);

  public void PlayPreloaded(AudioSource source, GameObject parentObject) => this.PlayPreloaded(source, parentObject, 1f);

  public void PlayPreloaded(AudioSource source, GameObject parentObject, float volume)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Preloaded audio source is null! Cannot play!");
    }
    else
    {
      SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source);
      if (sourceExtension != null)
        sourceExtension.m_codeVolume = volume;
      this.InitSourceTransform(source, parentObject);
      this.m_generatedSources.Add(source);
      this.Play(source);
    }
  }

  public AudioSource PlayClip(
    SoundPlayClipArgs args,
    bool createNewSource = true,
    SoundManager.SoundOptions options = null)
  {
    if (args == null || (UnityEngine.Object) args.m_def == (UnityEngine.Object) null && (UnityEngine.Object) args.m_forcedAudioClip == (UnityEngine.Object) null)
    {
      Debug.LogWarningFormat("PlayClip: using placeholder sound for audio clip: {0}", args != null ? (object) args.ToString() : (object) "");
      return this.PlayImpl((AudioSource) null, (SoundDef) null);
    }
    AudioSource source;
    if (createNewSource)
    {
      source = this.GenerateAudioSource(args.m_templateSource, args.m_def);
    }
    else
    {
      source = args.m_def.GetComponent<AudioSource>();
      if ((UnityEngine.Object) source != (UnityEngine.Object) null)
      {
        this.m_generatedSources.Add(source);
      }
      else
      {
        Log.Asset.PrintWarning("PlayClip: Loaded sound asset missing AudioSource. Generating new one...");
        source = this.GenerateAudioSource(args.m_templateSource, args.m_def);
      }
    }
    if ((UnityEngine.Object) args.m_forcedAudioClip != (UnityEngine.Object) null)
      source.clip = args.m_forcedAudioClip;
    if (args.m_volume.HasValue)
      source.volume = args.m_volume.Value;
    if (args.m_pitch.HasValue)
      source.pitch = args.m_pitch.Value;
    if (args.m_spatialBlend.HasValue)
      source.spatialBlend = args.m_spatialBlend.Value;
    if (args.m_category.HasValue)
      source.GetComponent<SoundDef>().m_Category = args.m_category.Value;
    this.InitSourceTransform(source, args.m_parentObject);
    if ((UnityEngine.Object) args.m_forcedAudioClip != (UnityEngine.Object) null)
    {
      if (this.Play(source, oneShotClip: args.m_forcedAudioClip))
        return source;
    }
    else if (this.Play(source, args.m_def, options: options))
      return source;
    this.FinishGeneratedSource(source);
    return (AudioSource) null;
  }

  public bool LoadAndPlayClip(AssetReference assetRef, SoundPlayClipArgs args)
  {
    if (string.IsNullOrEmpty((string) assetRef))
    {
      Log.Sound.PrintError("LoadAndPlayClip: Missing asset AssetReference!");
      return false;
    }
    if (!AssetLoader.Get().IsAssetAvailable(assetRef))
      return false;
    if (args == null)
    {
      Log.Sound.PrintWarning("LoadAndPlayClip: Missing SoundPlayClipArgs. Using default...");
      args = new SoundPlayClipArgs()
      {
        m_category = new Global.SoundCategory?(Global.SoundCategory.FX)
      };
    }
    return SoundLoader.LoadSound(assetRef, new PrefabCallback<GameObject>(this.OnLoadAndPlayClipLoaded), (object) args);
  }

  private void OnLoadSoundLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if (assetRef == null)
      Debug.LogErrorFormat("SoundManager.OnLoadSoundLoaded() - ERROR Tried to load null assetRef!", (object) assetRef, (object) go);
    else if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogErrorFormat("SoundManager.OnLoadSoundLoaded() - ERROR assetRef=\"{0}\" go=\"{1}\" failed to load", (object) assetRef, (object) go);
    }
    else
    {
      AudioSource component = go.GetComponent<AudioSource>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) go);
        Debug.LogErrorFormat("SoundManager.OnLoadSoundLoaded() - ERROR assetRef=\"{0}\" has no AudioSource", (object) assetRef);
      }
      else
      {
        this.RegisterSourceBundle(assetRef, component);
        component.volume = 0.0f;
        component.Play();
        component.Stop();
        this.UnregisterSourceBundle(assetRef.ToString(), component);
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component.gameObject);
      }
    }
  }

  private void OnLoadAndPlaySoundLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("SoundManager.OnLoadAndPlaySoundLoaded() - ERROR \"{0}\" failed to load", (object) assetRef));
    }
    else
    {
      AudioSource component = go.GetComponent<AudioSource>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) go);
        Debug.LogError((object) string.Format("SoundManager.OnLoadAndPlaySoundLoaded() - ERROR \"{0}\" has no AudioSource", (object) assetRef));
      }
      else
      {
        SoundManager.SoundLoadContext soundLoadContext = (SoundManager.SoundLoadContext) callbackData;
        if (soundLoadContext.m_sceneMode != SceneMgr.Mode.FATAL_ERROR && SceneMgr.Get() != null && SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FATAL_ERROR))
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) go);
        else if (this.RegisterSourceBundle(assetRef, component) == null)
          Debug.LogWarningFormat("Failed to load and play sound name={0}, go={1} (this may be due to it not yet being downloaded)", (object) assetRef, (object) go.name);
        else if (soundLoadContext.m_haveCallback && !GeneralUtils.IsCallbackValid((Delegate) soundLoadContext.m_callback))
        {
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) go);
          this.UnregisterSourceBundle(this.SceneObject.name, component);
        }
        else
        {
          this.m_generatedSources.Add(component);
          this.RegisterExtension(component).m_codeVolume = soundLoadContext.m_volume;
          this.InitSourceTransform(component, soundLoadContext.m_parent);
          this.Play(component);
          if (soundLoadContext.m_callback == null)
            return;
          soundLoadContext.m_callback(component, soundLoadContext.m_userData);
        }
      }
    }
  }

  private void OnLoadAndPlayClipLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Log.Sound.PrintError("LoadAndPlayClip: Sound asset \"{0}\" failed to load", (object) assetRef);
    }
    else
    {
      SoundDef component = go.GetComponent<SoundDef>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Log.Sound.PrintError("LoadAndPlayClip: SoundDef missing from asset! Aborting playing \"{0}\"", (object) assetRef);
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) go);
      }
      else
      {
        SoundPlayClipArgs args = (SoundPlayClipArgs) callbackData;
        args.m_def = component;
        this.PlayClip(args, false);
      }
    }
  }

  public void AddMusicTracks(List<MusicTrack> tracks) => this.AddTracks(tracks, this.m_musicTracks);

  public void AddAmbienceTracks(List<MusicTrack> tracks) => this.AddTracks(tracks, this.m_ambienceTracks);

  public List<MusicTrack> GetCurrentMusicTracks() => this.m_musicTracks;

  public List<MusicTrack> GetCurrentAmbienceTracks() => this.m_ambienceTracks;

  public int GetCurrentMusicTrackIndex() => this.m_musicTrackIndex;

  public void SetCurrentMusicTrackIndex(int idx)
  {
    if (this.m_musicTrackIndex == idx)
      return;
    this.m_musicIsAboutToPlay = this.PlayMusicTrack(idx);
  }

  public void SetCurrentMusicTrackTime(float time)
  {
    if ((bool) (UnityEngine.Object) this.m_currentMusicTrack)
      this.m_currentMusicTrack.time = time;
    else
      this.m_musicTrackStartTime = time;
  }

  public void StopCurrentMusicTrack()
  {
    if (!((UnityEngine.Object) this.m_currentMusicTrack != (UnityEngine.Object) null))
      return;
    this.FadeTrackOut(this.m_currentMusicTrack);
    this.ChangeCurrentMusicTrack((AudioSource) null);
  }

  public void StopCurrentAmbienceTrack()
  {
    if (!((UnityEngine.Object) this.m_currentAmbienceTrack != (UnityEngine.Object) null))
      return;
    this.FadeTrackOut(this.m_currentAmbienceTrack);
    this.ChangeCurrentAmbienceTrack((AudioSource) null);
  }

  public void NukeMusicAndAmbiencePlaylists()
  {
    this.m_musicTracks.Clear();
    this.m_ambienceTracks.Clear();
    this.m_nextMusicTrackIndex = 0;
    this.m_musicTrackIndex = 0;
    this.m_ambienceTrackIndex = 0;
  }

  public void NukePlaylistsAndStopPlayingCurrentTracks()
  {
    this.NukeMusicAndAmbiencePlaylists();
    this.StopCurrentMusicTrack();
    this.StopCurrentAmbienceTrack();
  }

  public void NukeMusicAndStopPlayingCurrentTrack()
  {
    this.m_musicTracks.Clear();
    this.m_nextMusicTrackIndex = 0;
    this.m_musicTrackIndex = 0;
    this.StopCurrentMusicTrack();
  }

  public void NukeAmbienceAndStopPlayingCurrentTrack()
  {
    this.m_ambienceTracks.Clear();
    this.m_ambienceTrackIndex = 0;
    this.StopCurrentAmbienceTrack();
  }

  public void ImmediatelyKillMusicAndAmbience()
  {
    this.NukeMusicAndAmbiencePlaylists();
    foreach (AudioSource source in this.m_fadingTracks.ToArray())
      this.FinishSource(source);
    if ((UnityEngine.Object) this.m_currentMusicTrack != (UnityEngine.Object) null)
    {
      this.FinishSource(this.m_currentMusicTrack);
      this.ChangeCurrentMusicTrack((AudioSource) null);
    }
    if (!((UnityEngine.Object) this.m_currentAmbienceTrack != (UnityEngine.Object) null))
      return;
    this.FinishSource(this.m_currentAmbienceTrack);
    this.ChangeCurrentAmbienceTrack((AudioSource) null);
  }

  private void AddTracks(List<MusicTrack> sourceTracks, List<MusicTrack> destTracks)
  {
    foreach (MusicTrack sourceTrack in sourceTracks)
      destTracks.Add(sourceTrack);
  }

  private void OnMusicLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("SoundManager.OnMusicLoaded() - ERROR \"{0}\" failed to load", (object) assetRef));
    }
    else
    {
      AudioSource component = go.GetComponent<AudioSource>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("SoundManager.OnMusicLoaded() - ERROR \"{0}\" has no AudioSource", (object) this.SceneObject.name));
      }
      else
      {
        this.RegisterSourceBundle(assetRef, component);
        MusicTrack musicTrack = (MusicTrack) callbackData;
        if (this.m_musicTrackIndex >= this.m_musicTracks.Count || this.m_musicTracks[this.m_musicTrackIndex] != musicTrack)
        {
          this.UnregisterSourceBundle((string) assetRef, component);
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) go);
        }
        else
        {
          this.m_generatedSources.Add(component);
          component.transform.parent = this.SceneObject.transform;
          component.volume *= musicTrack.m_volume;
          component.time = this.m_musicTrackStartTime;
          this.m_musicTrackStartTime = 0.0f;
          this.ChangeCurrentMusicTrack(component);
          this.Play(component);
          if (this.OnMusicStarted != null)
            this.OnMusicStarted();
        }
        this.m_musicIsAboutToPlay = false;
      }
    }
  }

  private void OnAmbienceLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogError((object) string.Format("SoundManager.OnAmbienceLoaded() - ERROR \"{0}\" failed to load", (object) assetRef));
    }
    else
    {
      AudioSource component = go.GetComponent<AudioSource>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("SoundManager.OnAmbienceLoaded() - ERROR \"{0}\" has no AudioSource", (object) this.SceneObject.name));
      }
      else
      {
        this.RegisterSourceBundle(assetRef, component);
        MusicTrack musicTrack = (MusicTrack) callbackData;
        if (!this.m_ambienceTracks.Contains(musicTrack))
        {
          this.UnregisterSourceBundle((string) assetRef, component);
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) go);
        }
        else
        {
          this.m_generatedSources.Add(component);
          component.transform.parent = this.SceneObject.transform;
          component.volume *= musicTrack.m_volume;
          this.ChangeCurrentAmbienceTrack(component);
          this.m_fadingTracksIn.Add(Processor.RunCoroutine(this.FadeTrackIn(component)));
          this.Play(component);
        }
        this.m_ambienceIsAboutToPlay = false;
      }
    }
  }

  private void ChangeCurrentMusicTrack(AudioSource source) => this.m_currentMusicTrack = source;

  private void ChangeCurrentAmbienceTrack(AudioSource source) => this.m_currentAmbienceTrack = source;

  private void UpdateMusicAndAmbience()
  {
    if (!this.IsMusicEnabled())
      return;
    if (!this.m_musicIsAboutToPlay)
    {
      if ((UnityEngine.Object) this.m_currentMusicTrack != (UnityEngine.Object) null)
      {
        if (!this.IsPlaying(this.m_currentMusicTrack))
          Processor.RunCoroutine(this.PlayMusicInSeconds(this.m_config.m_SecondsBetweenMusicTracks));
      }
      else
        this.m_musicIsAboutToPlay = this.PlayNextMusic();
    }
    if (this.m_ambienceIsAboutToPlay)
      return;
    if ((UnityEngine.Object) this.m_currentAmbienceTrack != (UnityEngine.Object) null)
    {
      if (this.IsPlaying(this.m_currentAmbienceTrack))
        return;
      Processor.RunCoroutine(this.PlayAmbienceInSeconds(0.0f));
    }
    else
      this.m_ambienceIsAboutToPlay = this.PlayNextAmbience();
  }

  private IEnumerator PlayMusicInSeconds(float seconds)
  {
    this.m_musicIsAboutToPlay = true;
    yield return (object) new WaitForSeconds(seconds);
    this.m_musicIsAboutToPlay = this.PlayNextMusic();
  }

  private bool PlayNextMusic() => this.IsMusicEnabled() && this.m_musicTracks.Count > 0 && this.PlayMusicTrack(this.m_nextMusicTrackIndex);

  private bool PlayMusicTrack(int index)
  {
    if (index < 0 || index >= this.m_musicTracks.Count)
      return false;
    this.m_musicTrackIndex = index;
    MusicTrack musicTrack = this.m_musicTracks[this.m_musicTrackIndex];
    this.m_nextMusicTrackIndex = (index + 1) % this.m_musicTracks.Count;
    if (musicTrack == null)
      return false;
    if ((UnityEngine.Object) this.m_currentMusicTrack != (UnityEngine.Object) null)
    {
      this.FadeTrackOut(this.m_currentMusicTrack);
      this.ChangeCurrentMusicTrack((AudioSource) null);
    }
    return SoundLoader.LoadSound((AssetReference) (AssetLoader.Get().IsAssetAvailable((AssetReference) musicTrack.m_name) ? musicTrack.m_name : musicTrack.m_fallback), new PrefabCallback<GameObject>(this.OnMusicLoaded), (object) musicTrack, this.GetPlaceholderSound());
  }

  private bool IsMusicEnabled() => !SoundUtils.IsDeviceBackgroundMusicPlaying() && this.m_isMasterEnabled && this.m_isMusicEnabled;

  private IEnumerator PlayAmbienceInSeconds(float seconds)
  {
    this.m_ambienceIsAboutToPlay = true;
    yield return (object) new WaitForSeconds(seconds);
    this.m_ambienceIsAboutToPlay = this.PlayNextAmbience();
  }

  private bool PlayNextAmbience()
  {
    if (!this.IsMusicEnabled() || this.m_ambienceTracks.Count <= 0)
      return false;
    MusicTrack ambienceTrack = this.m_ambienceTracks[this.m_ambienceTrackIndex];
    this.m_ambienceTrackIndex = (this.m_ambienceTrackIndex + 1) % this.m_ambienceTracks.Count;
    if (ambienceTrack == null)
      return false;
    string assetRef = AssetLoader.Get().IsAssetAvailable((AssetReference) ambienceTrack.m_name) ? ambienceTrack.m_name : ambienceTrack.m_fallback;
    foreach (Coroutine routine in this.m_fadingTracksIn)
    {
      if (routine != null)
        Processor.CancelCoroutine(routine);
    }
    this.m_fadingTracksIn.Clear();
    return SoundLoader.LoadSound((AssetReference) assetRef, new PrefabCallback<GameObject>(this.OnAmbienceLoaded), (object) ambienceTrack, this.GetPlaceholderSound());
  }

  private void FadeTrackOut(AudioSource source)
  {
    if (!this.IsActive(source))
      this.FinishSource(source);
    else
      Processor.RunCoroutine(this.FadeTrack(source, 0.0f));
  }

  private IEnumerator FadeTrackIn(AudioSource source)
  {
    SoundManager.SourceExtension ext = this.GetExtension(source);
    if (ext == null)
    {
      Log.Sound.PrintWarning("Unable to find extension for sound {0}", (object) source.name);
    }
    else
    {
      float targetVolume = this.GetVolume(source);
      float currTime = 0.0f;
      float targetVolumeTime = 1f;
      ext.m_codeVolume = 0.0f;
      this.UpdateVolume(source, ext);
      while ((double) ext.m_codeVolume < (double) targetVolume)
      {
        currTime += Time.deltaTime;
        ext.m_codeVolume = Mathf.Lerp(0.0f, targetVolume, Mathf.Clamp01(currTime / targetVolumeTime));
        this.UpdateVolume(source, ext);
        yield return (object) null;
        if ((UnityEngine.Object) source == (UnityEngine.Object) null || !this.IsActive(source))
          break;
      }
    }
  }

  private IEnumerator FadeTrack(AudioSource source, float targetVolume)
  {
    this.m_fadingTracks.Add(source);
    SoundManager.SourceExtension ext = this.GetExtension(source);
    while ((double) ext.m_codeVolume > 9.99999974737875E-05)
    {
      ext.m_codeVolume = Mathf.Lerp(ext.m_codeVolume, targetVolume, Time.deltaTime);
      this.UpdateVolume(source, ext);
      yield return (object) null;
      if ((UnityEngine.Object) source == (UnityEngine.Object) null || !this.IsActive(source))
        yield break;
    }
    this.FinishSource(source);
  }

  private SoundManager.SourceExtension RegisterExtension(
    AudioSource source,
    SoundDef oneShotDef = null,
    AudioClip oneShotClip = null,
    bool aboutToPlay = false)
  {
    SoundDef soundDef = oneShotDef;
    if ((UnityEngine.Object) soundDef == (UnityEngine.Object) null)
      soundDef = this.GetDefFromSource(source);
    SoundManager.SourceExtension ext = this.GetExtension(source);
    if (ext == null)
    {
      AssetHandle<AudioClip> clipHandle = (AssetHandle<AudioClip>) null;
      AudioClip clip = (UnityEngine.Object) oneShotClip == (UnityEngine.Object) null ? this.LoadClipForPlayback(ref clipHandle, source, soundDef) : oneShotClip;
      ServiceManager.Get<DisposablesCleaner>()?.Attach((Component) source, (IDisposable) clipHandle);
      if ((UnityEngine.Object) clip == (UnityEngine.Object) null || aboutToPlay && this.ProcessClipLimits(clip))
        return (SoundManager.SourceExtension) null;
      ext = this.RegisterSourceExtensionCommon(source, soundDef);
      ext.m_sourceClip = source.clip;
      this.InitNewClipOnSource(source, soundDef, ext, clip);
    }
    else if (aboutToPlay)
    {
      AudioClip clip;
      if ((UnityEngine.Object) oneShotClip == (UnityEngine.Object) null)
      {
        AssetHandle<AudioClip> clipHandle = (AssetHandle<AudioClip>) null;
        clip = this.LoadClipForPlayback(ref clipHandle, source, soundDef);
        ServiceManager.Get<DisposablesCleaner>()?.Attach((Component) source, (IDisposable) clipHandle);
      }
      else
        clip = oneShotClip;
      if (!this.CanPlayClipOnExistingSource(source, clip))
      {
        if (this.IsActive(source))
          this.Stop(source);
        else
          this.FinishSource(source);
        return (SoundManager.SourceExtension) null;
      }
      if ((UnityEngine.Object) source.clip != (UnityEngine.Object) clip)
      {
        if ((UnityEngine.Object) source.clip != (UnityEngine.Object) null)
          this.UnregisterSourceByClip(source);
        this.InitNewClipOnSource(source, soundDef, ext, clip);
      }
    }
    return ext;
  }

  private SoundManager.SourceExtension RegisterExtensionForVideoSource(
    AudioSource source,
    SoundDef def = null)
  {
    if ((UnityEngine.Object) def == (UnityEngine.Object) null)
      def = this.GetDefFromSource(source);
    return this.RegisterSourceExtensionCommon(source, def);
  }

  private SoundManager.SourceExtension RegisterSourceExtensionCommon(
    AudioSource source,
    SoundDef def)
  {
    SoundManager.SourceExtension extension = new SoundManager.SourceExtension();
    extension.m_sourceVolume = source.volume;
    extension.m_sourcePitch = source.pitch;
    extension.m_id = this.GetNextSourceId();
    this.AddExtensionMapping(source, extension);
    Global.SoundCategory category = this.GetCategory(source);
    if (category == Global.SoundCategory.NONE)
      category = def.m_Category;
    this.RegisterSourceByCategory(source, category);
    extension.m_defVolume = SoundUtils.GetRandomVolumeFromDef(def);
    extension.m_defPitch = SoundUtils.GetRandomPitchFromDef(def);
    return extension;
  }

  public AudioClip LoadClipForPlayback(
    ref AssetHandle<AudioClip> clipHandle,
    AudioSource source,
    SoundDef oneShotDef)
  {
    string clipAsset = (string) null;
    SoundDef def = oneShotDef;
    if ((UnityEngine.Object) oneShotDef == (UnityEngine.Object) null)
      def = source.GetComponent<SoundDef>();
    if ((UnityEngine.Object) def != (UnityEngine.Object) null)
    {
      clipAsset = SoundUtils.GetRandomClipFromDef(def);
      if (clipAsset == null)
      {
        clipAsset = def.m_AudioClip;
        if (string.IsNullOrEmpty(clipAsset))
        {
          if ((UnityEngine.Object) source.clip != (UnityEngine.Object) null)
            return source.clip;
          string str = "";
          if (HearthstoneApplication.IsInternal())
            str = " " + DebugUtils.GetHierarchyPathAndType((UnityEngine.Object) source);
          Error.AddDevFatal("{0} has no AudioClip. Top-level parent is {1}{2}.", (object) source.gameObject.name, (object) GameObjectUtils.FindTopParent((Component) source), (object) str);
          return (AudioClip) null;
        }
      }
    }
    if (clipAsset == null || (UnityEngine.Object) def == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("DetermineClipForPlayback: failed to GET AudioClip clipAsset={0}, gameObject={2}, soundDef={3}", (object) clipAsset, (object) source.gameObject.name, (object) def);
      return (AudioClip) null;
    }
    SoundLoader.LoadAudioClipWithFallback(ref clipHandle, source, (AssetReference) clipAsset);
    return clipHandle?.Asset;
  }

  private bool CanPlayClipOnExistingSource(AudioSource source, AudioClip clip) => !((UnityEngine.Object) clip == (UnityEngine.Object) null) && (this.IsActive(source) && !((UnityEngine.Object) source.clip != (UnityEngine.Object) clip) || !this.ProcessClipLimits(clip));

  private void InitNewClipOnSource(
    AudioSource source,
    SoundDef def,
    SoundManager.SourceExtension ext,
    AudioClip clip)
  {
    ext.m_defVolume = SoundUtils.GetRandomVolumeFromDef(def);
    ext.m_defPitch = SoundUtils.GetRandomPitchFromDef(def);
    source.clip = clip;
    this.RegisterSourceByClip(source, clip);
  }

  private void UnregisterExtension(AudioSource source, SoundManager.SourceExtension ext)
  {
    source.volume = ext.m_sourceVolume;
    source.pitch = ext.m_sourcePitch;
    source.clip = ext.m_sourceClip;
    this.RemoveExtensionMapping(source);
  }

  private void UpdateSource(AudioSource source, SoundManager.SourceExtension ext)
  {
    this.UpdateMute(source);
    this.UpdateVolume(source, ext);
    this.UpdatePitch(source, ext);
  }

  private void UpdateMute(AudioSource source)
  {
    bool categoryEnabled = this.IsCategoryEnabled(source);
    this.UpdateMute(source, categoryEnabled);
  }

  private void UpdateMute(AudioSource source, bool categoryEnabled) => source.mute = this.m_mute || !categoryEnabled;

  private void UpdateCategoryMute(Global.SoundCategory cat)
  {
    List<AudioSource> audioSourceList;
    if (!this.m_sourcesByCategory.TryGetValue(cat, out audioSourceList))
      return;
    bool categoryEnabled = this.IsCategoryEnabled(cat);
    for (int index = 0; index < audioSourceList.Count; ++index)
      this.UpdateMute(audioSourceList[index], categoryEnabled);
  }

  private void UpdateAllMutes()
  {
    foreach (SoundManager.ExtensionMapping extensionMapping in this.m_extensionMappings)
      this.UpdateMute(extensionMapping.Source);
  }

  private void UpdateVolume(AudioSource source, SoundManager.SourceExtension ext)
  {
    float categoryVolume = this.GetCategoryVolume(source);
    float duckingVolume = this.GetDuckingVolume(source);
    this.UpdateVolume(source, ext, categoryVolume, duckingVolume);
  }

  private void UpdateVolume(
    AudioSource source,
    SoundManager.SourceExtension ext,
    float categoryVolume,
    float duckingVolume)
  {
    source.volume = ext.m_codeVolume * ext.m_sourceVolume * ext.m_defVolume * categoryVolume * duckingVolume;
  }

  public void UpdateCategoryVolume(Global.SoundCategory cat)
  {
    List<AudioSource> audioSourceList;
    if (!this.m_sourcesByCategory.TryGetValue(cat, out audioSourceList))
      return;
    float categoryVolume = SoundUtils.GetCategoryVolume(cat);
    for (int index = 0; index < audioSourceList.Count; ++index)
    {
      AudioSource source = audioSourceList[index];
      if (!((UnityEngine.Object) source == (UnityEngine.Object) null))
      {
        SoundManager.SourceExtension extension = this.GetExtension(source);
        float duckingVolume = this.GetDuckingVolume(source);
        this.UpdateVolume(source, extension, categoryVolume, duckingVolume);
      }
    }
  }

  private void UpdateAllCategoryVolumes()
  {
    foreach (Global.SoundCategory key in this.m_sourcesByCategory.Keys)
      this.UpdateCategoryVolume(key);
  }

  private void UpdatePitch(AudioSource source, SoundManager.SourceExtension ext) => source.pitch = ext.m_codePitch * ext.m_sourcePitch * ext.m_defPitch;

  private void OnMasterEnabledOptionChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    this.m_isMasterEnabled = Options.Get().GetBool(option);
    this.UpdateAllMutes();
  }

  private void OnMasterVolumeOptionChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    this.SetMasterVolumeExponential();
  }

  private void OnEnabledOptionChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    this.m_isMusicEnabled = Options.Get().GetBool(option);
    foreach (KeyValuePair<Global.SoundCategory, Option> categoryEnabledOption in SoundDataTables.s_categoryEnabledOptionMap)
    {
      Global.SoundCategory key = categoryEnabledOption.Key;
      if (categoryEnabledOption.Value == option)
        this.UpdateCategoryMute(key);
    }
  }

  private void OnVolumeOptionChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    foreach (KeyValuePair<Global.SoundCategory, Option> categoryVolumeOption in SoundDataTables.s_categoryVolumeOptionMap)
    {
      Global.SoundCategory key = categoryVolumeOption.Key;
      if (categoryVolumeOption.Value == option)
        this.UpdateCategoryVolume(key);
    }
  }

  private void OnBackgroundSoundOptionChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    this.UpdateAppMute();
  }

  private void OnMonoSoundOptionChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    this.SetMonoSoundOption(Options.Get().GetBool(option));
  }

  private void OnAudioConfigurationChanged(bool deviceWasChanged)
  {
    if (!deviceWasChanged)
      return;
    Log.Sound.Print("System audio output device was changed.");
    this.UpdateAllCategoryVolumes();
  }

  private void RegisterSourceByCategory(AudioSource source, Global.SoundCategory cat)
  {
    List<AudioSource> audioSourceList1;
    if (!this.m_sourcesByCategory.TryGetValue(cat, out audioSourceList1))
    {
      List<AudioSource> audioSourceList2 = new List<AudioSource>();
      this.m_sourcesByCategory.Add(cat, audioSourceList2);
      audioSourceList2.Add(source);
    }
    else
    {
      if (audioSourceList1.Contains(source))
        return;
      audioSourceList1.Add(source);
    }
  }

  private void UnregisterSourceByCategory(AudioSource source)
  {
    Global.SoundCategory category = this.GetCategory(source);
    List<AudioSource> audioSourceList;
    if (!this.m_sourcesByCategory.TryGetValue(category, out audioSourceList))
      Debug.LogWarning((object) string.Format("SoundManager.UnregisterSourceByCategory() - {0} is untracked. category={1}", (object) this.GetSourceId(source), (object) category));
    else
      audioSourceList.Remove(source);
  }

  private bool IsCategoryEnabled(AudioSource source) => this.IsCategoryEnabled(source.GetComponent<SoundDef>().m_Category);

  private bool IsCategoryEnabled(Global.SoundCategory cat)
  {
    if (SoundUtils.IsMusicCategory(cat) && SoundUtils.IsDeviceBackgroundMusicPlaying() || !this.m_isMasterEnabled)
      return false;
    Option categoryEnabledOption = SoundUtils.GetCategoryEnabledOption(cat);
    switch (categoryEnabledOption)
    {
      case Option.INVALID:
        return true;
      case Option.MUSIC:
        return this.m_isMusicEnabled;
      default:
        return Options.Get().GetBool(categoryEnabledOption);
    }
  }

  private float GetCategoryVolume(AudioSource source) => SoundUtils.GetCategoryVolume(source.GetComponent<SoundDef>().m_Category);

  private bool IsCategoryAudible(Global.SoundCategory cat) => (double) SoundUtils.GetCategoryVolume(cat) > (double) Mathf.Epsilon && this.IsCategoryEnabled(cat);

  private void RegisterSourceByClip(AudioSource source, AudioClip clip)
  {
    List<AudioSource> audioSourceList1;
    if (!this.m_sourcesByClipName.TryGetValue(clip.name, out audioSourceList1))
    {
      List<AudioSource> audioSourceList2 = new List<AudioSource>();
      this.m_sourcesByClipName.Add(clip.name, audioSourceList2);
      audioSourceList2.Add(source);
    }
    else
    {
      if (audioSourceList1.Contains(source))
        return;
      audioSourceList1.Add(source);
    }
  }

  private void UnregisterSourceByClip(AudioSource source)
  {
    AudioClip clip = source.clip;
    if ((UnityEngine.Object) clip == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("SoundManager.UnregisterSourceByClip() - id {0} (source {1}) is untracked", (object) this.GetSourceId(source), (object) source));
    }
    else
    {
      List<AudioSource> audioSourceList;
      if (!this.m_sourcesByClipName.TryGetValue(clip.name, out audioSourceList))
      {
        Debug.LogError((object) string.Format("SoundManager.UnregisterSourceByClip() - id {0} (source {1}) is untracked. clip={2}", (object) this.GetSourceId(source), (object) source, (object) clip));
      }
      else
      {
        audioSourceList.Remove(source);
        if (audioSourceList.Count != 0)
          return;
        this.m_sourcesByClipName.Remove(clip.name);
      }
    }
  }

  private bool ProcessClipLimits(AudioClip clip)
  {
    if ((UnityEngine.Object) this.m_config == (UnityEngine.Object) null || this.m_config.m_PlaybackLimitDefs == null)
      return false;
    string name = clip.name;
    bool flag = false;
    AudioSource source1 = (AudioSource) null;
    foreach (SoundPlaybackLimitDef playbackLimitDef in this.m_config.m_PlaybackLimitDefs)
    {
      SoundPlaybackLimitClipDef defInPlaybackDef = this.FindClipDefInPlaybackDef(name, playbackLimitDef);
      if (defInPlaybackDef != null)
      {
        int num1 = defInPlaybackDef.m_Priority;
        float num2 = 2f;
        int num3 = 0;
        foreach (SoundPlaybackLimitClipDef clipDef in playbackLimitDef.m_ClipDefs)
        {
          List<AudioSource> audioSourceList;
          if (this.m_sourcesByClipName.TryGetValue(clipDef.LegacyName, out audioSourceList))
          {
            int priority = clipDef.m_Priority;
            foreach (AudioSource source2 in audioSourceList)
            {
              if (this.IsPlaying(source2))
              {
                float num4 = source2.time / source2.clip.length;
                if ((double) num4 <= (double) clipDef.m_ExclusivePlaybackThreshold)
                {
                  ++num3;
                  if (priority < num1 && (double) num4 < (double) num2)
                  {
                    source1 = source2;
                    num1 = priority;
                    num2 = num4;
                  }
                }
              }
            }
          }
        }
        if (num3 >= playbackLimitDef.m_Limit)
        {
          flag = true;
          break;
        }
      }
    }
    if (!flag)
      return false;
    if ((UnityEngine.Object) source1 == (UnityEngine.Object) null)
      return true;
    this.Stop(source1);
    return false;
  }

  private SoundPlaybackLimitClipDef FindClipDefInPlaybackDef(
    string clipName,
    SoundPlaybackLimitDef def)
  {
    if (def.m_ClipDefs == null)
      return (SoundPlaybackLimitClipDef) null;
    foreach (SoundPlaybackLimitClipDef clipDef in def.m_ClipDefs)
    {
      string legacyName = clipDef.LegacyName;
      if (clipName == legacyName)
        return clipDef;
    }
    return (SoundPlaybackLimitClipDef) null;
  }

  public bool StartDucking(SoundDucker ducker)
  {
    if ((UnityEngine.Object) ducker == (UnityEngine.Object) null || ducker.m_DuckedCategoryDefs == null || ducker.m_DuckedCategoryDefs.Count == 0)
      return false;
    this.RegisterForDucking((object) ducker, ducker.GetDuckedCategoryDefs());
    return true;
  }

  public void StopDucking(SoundDucker ducker)
  {
    if ((UnityEngine.Object) ducker == (UnityEngine.Object) null || ducker.m_DuckedCategoryDefs == null || ducker.m_DuckedCategoryDefs.Count == 0)
      return;
    this.UnregisterForDucking((object) ducker, ducker.GetDuckedCategoryDefs());
  }

  public void SetIgnoreDucking(AudioSource source, bool enable)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
      return;
    SoundDef component = source.GetComponent<SoundDef>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    component.m_IgnoreDucking = enable;
  }

  private void RegisterSourceForDucking(AudioSource source, SoundManager.SourceExtension ext)
  {
    SoundDuckingDef duckingDefForSource = this.FindDuckingDefForSource(source);
    if (duckingDefForSource == null)
      return;
    this.RegisterForDucking((object) source, duckingDefForSource.m_DuckedCategoryDefs);
    ext.m_ducking = true;
  }

  private void RegisterForDucking(object trigger, List<SoundDuckedCategoryDef> defs)
  {
    foreach (SoundDuckedCategoryDef def in defs)
      this.ChangeDuckState(this.RegisterDuckState(trigger, def), SoundManager.DuckMode.BEGINNING);
  }

  private SoundManager.DuckState RegisterDuckState(
    object trigger,
    SoundDuckedCategoryDef duckedCatDef)
  {
    Global.SoundCategory category = duckedCatDef.m_Category;
    List<SoundManager.DuckState> duckStateList;
    if (this.m_duckStates.TryGetValue(category, out duckStateList))
    {
      SoundManager.DuckState duckState = duckStateList.Find((Predicate<SoundManager.DuckState>) (currState => currState.IsTrigger(trigger)));
      if (duckState != null)
        return duckState;
    }
    else
    {
      duckStateList = new List<SoundManager.DuckState>();
      this.m_duckStates.Add(category, duckStateList);
    }
    SoundManager.DuckState duckState1 = new SoundManager.DuckState();
    duckStateList.Add(duckState1);
    duckState1.SetTrigger(trigger);
    duckState1.SetDuckedDef(duckedCatDef);
    return duckState1;
  }

  private void UnregisterSourceForDucking(AudioSource source, SoundManager.SourceExtension ext)
  {
    if (!ext.m_ducking)
      return;
    SoundDuckingDef duckingDefForSource = this.FindDuckingDefForSource(source);
    if (duckingDefForSource == null)
      return;
    this.UnregisterForDucking((object) source, duckingDefForSource.m_DuckedCategoryDefs);
  }

  private void UnregisterForDucking(object trigger, List<SoundDuckedCategoryDef> defs)
  {
    foreach (SoundDuckedCategoryDef def in defs)
    {
      Global.SoundCategory category = def.m_Category;
      List<SoundManager.DuckState> duckStateList;
      if (!this.m_duckStates.TryGetValue(category, out duckStateList))
      {
        Debug.LogError((object) string.Format("SoundManager.UnregisterForDucking() - {0} ducks {1}, but no DuckStates were found for {1}", trigger, (object) category));
      }
      else
      {
        SoundManager.DuckState state = duckStateList.Find((Predicate<SoundManager.DuckState>) (currState => currState.IsTrigger(trigger)));
        if (state != null)
          this.ChangeDuckState(state, SoundManager.DuckMode.RESTORING);
      }
    }
  }

  private uint GetNextDuckStateTweenId()
  {
    this.m_nextDuckStateTweenId = (uint) ((int) this.m_nextDuckStateTweenId + 1 & -1);
    return this.m_nextDuckStateTweenId;
  }

  private void ChangeDuckState(SoundManager.DuckState state, SoundManager.DuckMode mode)
  {
    string tweenName = state.GetTweenName();
    if (tweenName != null)
      iTween.StopByName(this.SceneObject, tweenName);
    state.SetMode(mode);
    state.SetTweenName((string) null);
    if (mode != SoundManager.DuckMode.BEGINNING)
    {
      if (mode != SoundManager.DuckMode.RESTORING)
        return;
      this.AnimateRestoringDuckState(state);
    }
    else
      this.AnimateBeginningDuckState(state);
  }

  private void AnimateBeginningDuckState(SoundManager.DuckState state)
  {
    string name = string.Format("DuckState Begin id={0}", (object) this.GetNextDuckStateTweenId());
    state.SetTweenName(name);
    SoundDuckedCategoryDef duckedDef = state.GetDuckedDef();
    Action<object> action1 = (Action<object>) (amount =>
    {
      state.SetVolume((float) amount);
      this.UpdateCategoryVolume(duckedDef.m_Category);
    });
    Action<object> action2 = (Action<object>) (e => this.OnDuckStateBeginningComplete(e));
    Hashtable tweenHashTable = iTweenManager.Get().GetTweenHashTable();
    tweenHashTable.Add((object) "name", (object) name);
    tweenHashTable.Add((object) "time", (object) duckedDef.m_BeginSec);
    tweenHashTable.Add((object) "easeType", (object) duckedDef.m_BeginEaseType);
    tweenHashTable.Add((object) "from", (object) state.GetVolume());
    tweenHashTable.Add((object) "to", (object) duckedDef.m_Volume);
    tweenHashTable.Add((object) "onupdate", (object) action1);
    tweenHashTable.Add((object) "oncomplete", (object) action2);
    tweenHashTable.Add((object) "oncompleteparams", (object) state);
    iTween.ValueTo(this.SceneObject, tweenHashTable, false);
  }

  private void OnDuckStateBeginningComplete(object arg)
  {
    if (!(arg is SoundManager.DuckState duckState))
      return;
    duckState.SetMode(SoundManager.DuckMode.HOLD);
    duckState.SetTweenName((string) null);
  }

  private void AnimateRestoringDuckState(SoundManager.DuckState state)
  {
    string name = string.Format("DuckState Finish id={0}", (object) this.GetNextDuckStateTweenId());
    state.SetTweenName(name);
    SoundDuckedCategoryDef duckedDef = state.GetDuckedDef();
    Action<object> action1 = (Action<object>) (amount =>
    {
      state.SetVolume((float) amount);
      this.UpdateCategoryVolume(duckedDef.m_Category);
    });
    Action<object> action2 = (Action<object>) (e => this.OnDuckStateRestoringComplete(e));
    Hashtable tweenHashTable = iTweenManager.Get().GetTweenHashTable();
    tweenHashTable.Add((object) "name", (object) name);
    tweenHashTable.Add((object) "time", (object) duckedDef.m_RestoreSec);
    tweenHashTable.Add((object) "easeType", (object) duckedDef.m_RestoreEaseType);
    tweenHashTable.Add((object) "from", (object) state.GetVolume());
    tweenHashTable.Add((object) "to", (object) 1f);
    tweenHashTable.Add((object) "onupdate", (object) action1);
    tweenHashTable.Add((object) "oncomplete", (object) action2);
    tweenHashTable.Add((object) "oncompleteparams", (object) state);
    iTween.ValueTo(this.SceneObject, tweenHashTable, false);
  }

  private void OnDuckStateRestoringComplete(object arg)
  {
    if (!(arg is SoundManager.DuckState duckState1))
      return;
    Global.SoundCategory category = duckState1.GetDuckedDef().m_Category;
    List<SoundManager.DuckState> duckState2 = this.m_duckStates[category];
    for (int index = 0; index < duckState2.Count; ++index)
    {
      if (duckState2[index] == duckState1)
      {
        duckState2.RemoveAt(index);
        if (duckState2.Count != 0)
          break;
        this.m_duckStates.Remove(category);
        break;
      }
    }
  }

  private SoundDuckingDef FindDuckingDefForSource(AudioSource source) => this.FindDuckingDefForCategory(this.GetCategory(source));

  private SoundDuckingDef FindDuckingDefForCategory(Global.SoundCategory cat)
  {
    if ((UnityEngine.Object) this.m_config == (UnityEngine.Object) null || this.m_config.m_DuckingDefs == null)
      return (SoundDuckingDef) null;
    foreach (SoundDuckingDef duckingDef in this.m_config.m_DuckingDefs)
    {
      if (cat == duckingDef.m_TriggerCategory)
        return duckingDef;
    }
    return (SoundDuckingDef) null;
  }

  private float GetDuckingVolume(AudioSource source)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
      return 1f;
    SoundDef component = source.GetComponent<SoundDef>();
    return component.m_IgnoreDucking ? 1f : this.GetDuckingVolume(component.m_Category);
  }

  private float GetDuckingVolume(Global.SoundCategory cat)
  {
    List<SoundManager.DuckState> duckStateList;
    if (!this.m_duckStates.TryGetValue(cat, out duckStateList))
      return 1f;
    float duckingVolume = 1f;
    foreach (SoundManager.DuckState duckState in duckStateList)
    {
      Global.SoundCategory triggerCategory = duckState.GetTriggerCategory();
      if (triggerCategory == Global.SoundCategory.NONE || this.IsCategoryAudible(triggerCategory))
      {
        float volume = duckState.GetVolume();
        if ((double) duckingVolume > (double) volume)
          duckingVolume = volume;
      }
    }
    return duckingVolume;
  }

  private int GetNextSourceId()
  {
    int nextSourceId = this.m_nextSourceId;
    this.m_nextSourceId = this.m_nextSourceId == int.MaxValue ? 1 : this.m_nextSourceId + 1;
    return nextSourceId;
  }

  private int GetSourceId(AudioSource source)
  {
    SoundManager.SourceExtension extension = this.GetExtension(source);
    return extension == null ? 0 : extension.m_id;
  }

  private AudioSource PlayImpl(
    AudioSource source,
    SoundDef oneShotDef,
    AudioClip oneShotClip = null,
    SoundManager.SoundOptions additionalSettings = null)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null)
    {
      AudioSource placeholderSource = this.GetPlaceholderSource();
      if ((UnityEngine.Object) placeholderSource == (UnityEngine.Object) null)
      {
        Error.AddDevFatal("SoundManager.Play() - source is null and fallback is null");
        return (AudioSource) null;
      }
      Debug.LogWarningFormat("Using placeholder sound for source={0}, oneShotDef={1}, oneShotClip={2}", (object) source, (object) oneShotDef, (object) oneShotClip);
      source = UnityEngine.Object.Instantiate<AudioSource>(placeholderSource);
      this.m_generatedSources.Add(source);
    }
    bool flag = this.IsActive(source);
    SoundManager.SourceExtension ext = this.RegisterExtension(source, oneShotDef, oneShotClip, true);
    if (ext == null)
      return (AudioSource) null;
    if (!flag)
      this.RegisterSourceForDucking(source, ext);
    this.UpdateSource(source, ext);
    if (additionalSettings != null && additionalSettings.InstanceLimited)
    {
      int num1;
      if (this.activeLimitedSounds.TryGetValue(source.gameObject.name, out num1))
      {
        int num2 = additionalSettings.MaxInstancesOfThisSound;
        if (num2 < 1)
          num2 = 1;
        if (num1 >= num2)
        {
          switch (additionalSettings.LimitMaxingOutOption)
          {
            case SoundManager.LimitMaxOutOption.SKIP_NEWEST:
              return (AudioSource) null;
            case SoundManager.LimitMaxOutOption.STOP_OLDEST:
              this.FinishFirstGeneratedSourceByName(source.gameObject.name);
              break;
            default:
              Log.Presence.PrintWarning("Unknown Sound MaxOut Option: {0}", (object) additionalSettings.LimitMaxingOutOption);
              return (AudioSource) null;
          }
        }
        else
        {
          this.activeLimitedSounds.Remove(source.gameObject.name);
          this.activeLimitedSounds.Add(source.gameObject.name, num1 + 1);
        }
      }
      else
        this.activeLimitedSounds.Add(source.gameObject.name, 1);
      float time = additionalSettings.InstanceTimeLimit;
      if ((double) time <= 0.0)
        time = source.clip.length;
      HearthstoneApplication.Get().StartCoroutine(this.EnableInstanceLimitedSound(source.gameObject.name, time));
    }
    source.Play();
    return source;
  }

  private IEnumerator EnableInstanceLimitedSound(string sound, float time)
  {
    if (this.activeLimitedSounds.ContainsKey(sound))
    {
      while ((double) time > 0.0)
      {
        time -= Time.deltaTime;
        yield return (object) null;
      }
      int num1;
      if (this.activeLimitedSounds.TryGetValue(sound, out num1))
      {
        this.activeLimitedSounds.Remove(sound);
        int num2 = num1 - 1;
        if (num2 > 0)
          this.activeLimitedSounds.Add(sound, num2);
      }
    }
  }

  private SoundDef GetDefFromSource(AudioSource source)
  {
    SoundDef defFromSource = source.GetComponent<SoundDef>();
    if ((UnityEngine.Object) defFromSource == (UnityEngine.Object) null)
    {
      Log.Sound.Print("SoundUtils.GetDefFromSource() - source={0} has no def. adding new def.", (object) source);
      defFromSource = source.gameObject.AddComponent<SoundDef>();
    }
    return defFromSource;
  }

  private void OnAppFocusChanged(bool focus, object userData) => this.UpdateAppMute();

  private void UpdateAppMute()
  {
    this.UpdateMusicAndSources();
    if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
      this.m_mute = !HearthstoneApplication.Get().HasFocus() && !Options.Get().GetBool(Option.BACKGROUND_SOUND);
    this.UpdateAllMutes();
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData) => this.GarbageCollectBundles();

  private AudioSource GenerateAudioSource(AudioSource templateSource, SoundDef def)
  {
    string name = string.Format("Audio Object - {0}", (UnityEngine.Object) def != (UnityEngine.Object) null ? (object) Path.GetFileNameWithoutExtension(def.m_AudioClip) : (object) "CreatedSound");
    AudioSource component;
    if ((bool) (UnityEngine.Object) templateSource)
    {
      GameObject go = new GameObject(name);
      SoundUtils.AddAudioSourceComponents(go);
      component = go.GetComponent<AudioSource>();
      SoundUtils.CopyAudioSource(templateSource, component);
    }
    else if ((bool) (UnityEngine.Object) this.m_config.m_PlayClipTemplate)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_config.m_PlayClipTemplate.gameObject);
      gameObject.name = name;
      component = gameObject.GetComponent<AudioSource>();
    }
    else
    {
      GameObject go = new GameObject(name);
      SoundUtils.AddAudioSourceComponents(go);
      component = go.GetComponent<AudioSource>();
    }
    this.m_generatedSources.Add(component);
    return component;
  }

  private void InitSourceTransform(AudioSource source, GameObject parentObject)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null || (UnityEngine.Object) source.gameObject == (UnityEngine.Object) null || (UnityEngine.Object) source.transform == (UnityEngine.Object) null)
      return;
    source.transform.parent = this.SceneObject.transform;
    if ((UnityEngine.Object) parentObject == (UnityEngine.Object) null || (UnityEngine.Object) parentObject.transform == (UnityEngine.Object) null)
      source.transform.position = Vector3.zero;
    else
      source.transform.position = parentObject.transform.position;
  }

  private void FinishSource(AudioSource source)
  {
    if ((UnityEngine.Object) this.m_currentMusicTrack == (UnityEngine.Object) source)
      this.ChangeCurrentMusicTrack((AudioSource) null);
    else if ((UnityEngine.Object) this.m_currentAmbienceTrack == (UnityEngine.Object) source)
      this.ChangeCurrentAmbienceTrack((AudioSource) null);
    for (int index = 0; index < this.m_fadingTracks.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_fadingTracks[index] == (UnityEngine.Object) source)
      {
        this.m_fadingTracks.RemoveAt(index);
        break;
      }
    }
    this.UnregisterSourceByCategory(source);
    this.UnregisterSourceByClip(source);
    SoundManager.SourceExtension extension = this.GetExtension(source);
    if (extension != null)
    {
      this.UnregisterSourceForDucking(source, extension);
      this.UnregisterSourceBundle(source, extension);
      this.UnregisterExtension(source, extension);
    }
    this.FinishGeneratedSource(source);
  }

  private void FinishGeneratedSource(AudioSource source)
  {
    for (int index = 0; index < this.m_generatedSources.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_generatedSources[index] == (UnityEngine.Object) source)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) source.gameObject);
        this.m_generatedSources.RemoveAt(index);
        break;
      }
    }
  }

  private void FinishFirstGeneratedSourceByName(string sourceName)
  {
    for (int index = 0; index < this.m_generatedSources.Count; ++index)
    {
      AudioSource generatedSource = this.m_generatedSources[index];
      if (generatedSource.gameObject.name == sourceName)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) generatedSource.gameObject);
        this.m_generatedSources.RemoveAt(index);
        break;
      }
    }
  }

  private SoundManager.BundleInfo RegisterSourceBundle(
    AssetReference assetRef,
    AudioSource source)
  {
    SoundManager.BundleInfo bundleInfo;
    if (!this.m_bundleInfos.TryGetValue((string) assetRef, out bundleInfo))
    {
      bundleInfo = new SoundManager.BundleInfo();
      bundleInfo.SetAssetRef(assetRef);
      this.m_bundleInfos.Add((string) assetRef, bundleInfo);
    }
    if ((UnityEngine.Object) source != (UnityEngine.Object) null)
    {
      bundleInfo.AddRef(source);
      SoundManager.SourceExtension sourceExtension = this.RegisterExtension(source);
      if (sourceExtension == null)
        return (SoundManager.BundleInfo) null;
      sourceExtension.m_bundleName = (string) assetRef;
    }
    return bundleInfo;
  }

  private void UnregisterSourceBundle(AudioSource source, SoundManager.SourceExtension ext)
  {
    if (ext.m_bundleName == null)
      return;
    this.UnregisterSourceBundle(ext.m_bundleName, source);
  }

  private void UnregisterSourceBundle(string name, AudioSource source)
  {
    SoundManager.BundleInfo bundleInfo;
    if (!this.m_bundleInfos.TryGetValue(name, out bundleInfo) || !bundleInfo.RemoveRef(source) || !bundleInfo.CanGarbageCollect())
      return;
    this.m_bundleInfos.Remove(name);
    this.UnloadSoundBundle((AssetReference) name);
  }

  private void UnloadSoundBundle(AssetReference assetRef)
  {
  }

  private void GarbageCollectBundles()
  {
    Map<string, SoundManager.BundleInfo> map = new Map<string, SoundManager.BundleInfo>();
    foreach (KeyValuePair<string, SoundManager.BundleInfo> bundleInfo1 in this.m_bundleInfos)
    {
      string key = bundleInfo1.Key;
      SoundManager.BundleInfo bundleInfo2 = bundleInfo1.Value;
      bundleInfo2.EnableGarbageCollect(true);
      if (bundleInfo2.CanGarbageCollect())
        this.UnloadSoundBundle((AssetReference) key);
      else
        map.Add(key, bundleInfo2);
    }
    this.m_bundleInfos = map;
  }

  private void UpdateMusicAndSources()
  {
    this.UpdateMusicAndAmbience();
    this.UpdateSources();
  }

  private void UpdateSources()
  {
    this.UpdateSourceExtensionMappings();
    this.UpdateSourcesByCategory();
    this.UpdateSourcesByClipName();
    this.UpdateSourceBundles();
    this.UpdateGeneratedSources();
    this.UpdateDuckStates();
  }

  private void UpdateSourceExtensionMappings()
  {
    int index = 0;
    while (index < this.m_extensionMappings.Count)
    {
      AudioSource source = this.m_extensionMappings[index].Source;
      if ((UnityEngine.Object) source == (UnityEngine.Object) null)
      {
        this.m_extensionMappings.RemoveAt(index);
      }
      else
      {
        if (!this.IsActive(source))
          this.m_inactiveSources.Add(source);
        ++index;
      }
    }
    this.CleanInactiveSources();
  }

  private void CleanUpSourceList(List<AudioSource> sources)
  {
    if (sources == null)
      return;
    int index = 0;
    while (index < sources.Count)
    {
      if ((UnityEngine.Object) sources[index] == (UnityEngine.Object) null)
        sources.RemoveAt(index);
      else
        ++index;
    }
  }

  private void UpdateSourcesByCategory()
  {
    foreach (KeyValuePair<Global.SoundCategory, List<AudioSource>> keyValuePair in this.m_sourcesByCategory)
      this.CleanUpSourceList(keyValuePair.Value);
  }

  private void UpdateSourcesByClipName()
  {
    foreach (KeyValuePair<string, List<AudioSource>> keyValuePair in this.m_sourcesByClipName)
      this.CleanUpSourceList(keyValuePair.Value);
  }

  private void UpdateSourceBundles()
  {
    this.m_bundleInfosToRemove.Clear();
    foreach (KeyValuePair<string, SoundManager.BundleInfo> bundleInfo1 in this.m_bundleInfos)
    {
      SoundManager.BundleInfo bundleInfo2 = bundleInfo1.Value;
      List<AudioSource> refs = bundleInfo2.GetRefs();
      int index = 0;
      bool flag = false;
      while (index < refs.Count)
      {
        if ((UnityEngine.Object) refs[index] == (UnityEngine.Object) null)
        {
          flag = true;
          refs.RemoveAt(index);
        }
        else
          ++index;
      }
      if (flag)
      {
        string assetRef = bundleInfo2.GetAssetRef();
        if (bundleInfo2.CanGarbageCollect())
          this.m_bundleInfosToRemove.Add(assetRef);
      }
    }
    for (int index = 0; index < this.m_bundleInfosToRemove.Count; ++index)
    {
      string str = this.m_bundleInfosToRemove[index];
      this.m_bundleInfos.Remove(str);
      this.UnloadSoundBundle((AssetReference) str);
    }
  }

  private void UpdateGeneratedSources() => this.CleanUpSourceList(this.m_generatedSources);

  private void UpdateDuckStates()
  {
    foreach (KeyValuePair<Global.SoundCategory, List<SoundManager.DuckState>> duckState in this.m_duckStates)
    {
      foreach (SoundManager.DuckState state in duckState.Value)
      {
        if (!state.IsTriggerAlive() && state.GetMode() != SoundManager.DuckMode.RESTORING)
          this.ChangeDuckState(state, SoundManager.DuckMode.RESTORING);
      }
    }
  }

  private void CleanInactiveSources()
  {
    foreach (AudioSource inactiveSource in this.m_inactiveSources)
      this.FinishSource(inactiveSource);
    this.m_inactiveSources.Clear();
  }

  private void AddExtensionMapping(AudioSource source, SoundManager.SourceExtension extension)
  {
    if ((UnityEngine.Object) source == (UnityEngine.Object) null || extension == null)
      return;
    this.m_extensionMappings.Add(new SoundManager.ExtensionMapping()
    {
      Source = source,
      Extension = extension
    });
  }

  private void RemoveExtensionMapping(AudioSource source)
  {
    for (int index = 0; index < this.m_extensionMappings.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_extensionMappings[index].Source == (UnityEngine.Object) source)
      {
        this.m_extensionMappings.RemoveAt(index);
        break;
      }
    }
  }

  private SoundManager.SourceExtension GetExtension(AudioSource source)
  {
    for (int index = 0; index < this.m_extensionMappings.Count; ++index)
    {
      SoundManager.ExtensionMapping extensionMapping = this.m_extensionMappings[index];
      if ((UnityEngine.Object) extensionMapping.Source == (UnityEngine.Object) source)
        return extensionMapping.Extension;
    }
    return (SoundManager.SourceExtension) null;
  }

  public delegate void LoadedCallback(AudioSource source, object userData);

  public enum LimitMaxOutOption
  {
    SKIP_NEWEST,
    STOP_OLDEST,
  }

  public class SoundOptions
  {
    public bool InstanceLimited { get; set; }

    public float InstanceTimeLimit { get; set; }

    public int MaxInstancesOfThisSound { get; set; } = 1;

    public SoundManager.LimitMaxOutOption LimitMaxingOutOption { get; set; }
  }

  private class ExtensionMapping
  {
    public AudioSource Source;
    public SoundManager.SourceExtension Extension;
  }

  private class SoundLoadContext
  {
    public GameObject m_parent;
    public float m_volume;
    public SceneMgr.Mode m_sceneMode;
    public bool m_haveCallback;
    public SoundManager.LoadedCallback m_callback;
    public object m_userData;

    public void Init(
      GameObject parent,
      float volume,
      SoundManager.LoadedCallback callback,
      object userData)
    {
      this.m_parent = parent;
      this.m_volume = volume;
      this.Init(callback, userData);
    }

    public void Init(SoundManager.LoadedCallback callback, object userData)
    {
      SceneMgr service;
      this.m_sceneMode = ServiceManager.TryGet<SceneMgr>(out service) ? service.GetMode() : SceneMgr.Mode.INVALID;
      this.m_haveCallback = callback != null;
      this.m_callback = callback;
      this.m_userData = userData;
    }
  }

  private class SourceExtension
  {
    public int m_id;
    public float m_codeVolume = 1f;
    public float m_sourceVolume = 1f;
    public float m_defVolume = 1f;
    public float m_codePitch = 1f;
    public float m_sourcePitch = 1f;
    public float m_defPitch = 1f;
    public AudioClip m_sourceClip;
    public bool m_paused;
    public bool m_ducking;
    public string m_bundleName;
  }

  private class BundleInfo
  {
    private AssetReference m_assetRef;
    private List<AudioSource> m_refs = new List<AudioSource>();
    private bool m_garbageCollect;

    public string GetAssetRef() => (string) this.m_assetRef;

    public void SetAssetRef(AssetReference assetRef) => this.m_assetRef = assetRef;

    public List<AudioSource> GetRefs() => this.m_refs;

    public void AddRef(AudioSource instance)
    {
      this.m_garbageCollect = false;
      this.m_refs.Add(instance);
    }

    public bool RemoveRef(AudioSource instance) => this.m_refs.Remove(instance);

    public bool CanGarbageCollect() => this.m_garbageCollect && this.m_refs.Count <= 0;

    public void EnableGarbageCollect(bool enable) => this.m_garbageCollect = enable;
  }

  private enum DuckMode
  {
    IDLE,
    BEGINNING,
    HOLD,
    RESTORING,
  }

  private class DuckState
  {
    private object m_trigger;
    private Global.SoundCategory m_triggerCategory;
    private SoundDuckedCategoryDef m_duckedDef;
    private SoundManager.DuckMode m_mode;
    private string m_tweenName;
    private float m_volume = 1f;

    public void SetTrigger(object trigger)
    {
      this.m_trigger = trigger;
      AudioSource source = trigger as AudioSource;
      if (!((UnityEngine.Object) source != (UnityEngine.Object) null))
        return;
      this.m_triggerCategory = SoundManager.Get().GetCategory(source);
    }

    public bool IsTrigger(object trigger) => this.m_trigger == trigger;

    public bool IsTriggerAlive() => GeneralUtils.IsObjectAlive(this.m_trigger);

    public Global.SoundCategory GetTriggerCategory() => this.m_triggerCategory;

    public SoundDuckedCategoryDef GetDuckedDef() => this.m_duckedDef;

    public void SetDuckedDef(SoundDuckedCategoryDef def) => this.m_duckedDef = def;

    public SoundManager.DuckMode GetMode() => this.m_mode;

    public void SetMode(SoundManager.DuckMode mode) => this.m_mode = mode;

    public string GetTweenName() => this.m_tweenName;

    public void SetTweenName(string name) => this.m_tweenName = name;

    public float GetVolume() => this.m_volume;

    public void SetVolume(float volume) => this.m_volume = volume;
  }
}
