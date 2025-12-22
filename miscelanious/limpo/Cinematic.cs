using Assets;
using Blizzard.T5.AssetManager;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Core;
using Hearthstone.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Cinematic : IService, IHasUpdate
{
  private static readonly string Hearthstone_Tavern_Abridged = nameof (Hearthstone_Tavern_Abridged);
  private static readonly string Hearthstone_Tavern_Abridged_Logo = nameof (Hearthstone_Tavern_Abridged_Logo);
  private static readonly AssetReference Hearthstone_Tavern_Abridged_Audio = new AssetReference("Hearthstone_Tavern_Abridged_Audio.wav:f89a884079f1645598bb19565f5915ef");
  private static readonly string Mobile_Assets_Path = "UnimportedAssets/MobileAssets/Android/";
  private AssetHandle<AudioClip> m_movieAudio;
  private AudioSource m_audioSource;
  private Camera m_camera;
  private SoundDucker m_soundDucker;
  private bool m_started;
  private bool m_canceled;
  private int m_previousTargetFrameRate;
  private Action m_callback;
  private VideoPlayer m_mainPlayer;
  private VideoPlayer m_logoPlayer;
  private GameObject m_sceneObject;
  private float m_playBeginTime;

  private GameObject SceneObject
  {
    get
    {
      if ((UnityEngine.Object) this.m_sceneObject == (UnityEngine.Object) null)
        this.m_sceneObject = new GameObject("CinematicSceneObject", new System.Type[1]
        {
          typeof (HSDontDestroyOnLoad)
        });
      return this.m_sceneObject;
    }
  }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    this.m_soundDucker = this.SceneObject.AddComponent<SoundDucker>();
    this.m_soundDucker.m_GlobalDuckDef = new SoundDuckedCategoryDef();
    this.m_soundDucker.m_GlobalDuckDef.m_Volume = 0.0f;
    this.m_soundDucker.m_GlobalDuckDef.m_RestoreSec = 1.5f;
    this.m_soundDucker.m_GlobalDuckDef.m_BeginSec = 1.5f;
    yield break;
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown() => AssetHandle.SafeDispose<AudioClip>(ref this.m_movieAudio);

  public void Play(Action callback)
  {
    this.m_callback = callback;
    Options.Get().SetBool(Option.HAS_SEEN_NEW_CINEMATIC, true);
    this.m_canceled = false;
    this.m_started = true;
    Processor.RunCoroutine(this.AwaitReadinessThenPlay());
  }

  private VideoPlayer CreatePlayer()
  {
    VideoPlayer player = this.SceneObject.AddComponent<VideoPlayer>();
    player.isLooping = false;
    player.playOnAwake = false;
    player.audioOutputMode = VideoAudioOutputMode.None;
    return player;
  }

  private void OnPlayBegin()
  {
    TelemetryManager.Client().SendCinematic(true, -1f);
    this.m_playBeginTime = Time.realtimeSinceStartup;
    this.m_previousTargetFrameRate = Application.targetFrameRate;
    Application.targetFrameRate = 0;
    this.m_mainPlayer = this.CreatePlayer();
    this.m_logoPlayer = this.CreatePlayer();
    this.m_mainPlayer.renderMode = VideoRenderMode.CameraNearPlane;
    this.m_logoPlayer.renderMode = VideoRenderMode.CameraFarPlane;
    this.m_mainPlayer.loopPointReached += new VideoPlayer.EventHandler(this.OnMainVideoComplete);
    this.m_logoPlayer.loopPointReached += new VideoPlayer.EventHandler(this.OnLogoVideoComplete);
    BnetBar.Get().gameObject.SetActive(false);
    PegCursor.Get().Hide();
    this.CreateCamera();
  }

  private void OnPlayEnd(bool canceled)
  {
    Application.targetFrameRate = this.m_previousTargetFrameRate;
    if ((UnityEngine.Object) BnetBar.Get() != (UnityEngine.Object) null)
    {
      BnetBar.Get().gameObject.SetActive(true);
      BnetBar.Get().UpdateLayout();
    }
    if ((UnityEngine.Object) PegCursor.Get() != (UnityEngine.Object) null)
      PegCursor.Get().Show();
    if ((UnityEngine.Object) SocialToastMgr.Get() != (UnityEngine.Object) null)
      SocialToastMgr.Get().Reset();
    if ((UnityEngine.Object) this.m_camera != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_camera.gameObject);
    if (SoundManager.Get() != null)
      SoundManager.Get().Stop(this.m_audioSource);
    if ((UnityEngine.Object) this.m_soundDucker != (UnityEngine.Object) null)
      this.m_soundDucker.StopDucking();
    AssetHandle.SafeDispose<AudioClip>(ref this.m_movieAudio);
    if ((UnityEngine.Object) this.m_audioSource != (UnityEngine.Object) null)
      this.m_audioSource.Stop();
    this.m_canceled = true;
    this.m_started = false;
    if (this.m_callback != null)
    {
      this.m_callback();
      this.m_callback = (Action) null;
    }
    float duration = -1f;
    if (canceled)
      duration = Time.realtimeSinceStartup - this.m_playBeginTime;
    TelemetryManager.Client().SendCinematic(false, duration);
    Processor.RunCoroutine(this.WaitOneFrameThenTeardownPlayer());
  }

  private IEnumerator WaitOneFrameThenTeardownPlayer()
  {
    yield return (object) null;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_mainPlayer);
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_logoPlayer);
  }

  private IEnumerator AwaitReadinessThenPlay()
  {
    Cinematic cinematic = this;
    cinematic.OnPlayBegin();
    AssetLoader.Get().LoadAsset<AudioClip>(Cinematic.Hearthstone_Tavern_Abridged_Audio, new AssetHandleCallback<AudioClip>(cinematic.AudioLoaded));
    cinematic.LoadMovie();
    cinematic.LoadLogo();
    if (PlatformSettings.IsMobile())
    {
      while (cinematic.m_movieAudio == null && !cinematic.m_canceled)
        yield return (object) null;
    }
    else
    {
      while ((!cinematic.m_mainPlayer.isPrepared || cinematic.m_movieAudio == null || !cinematic.m_logoPlayer.isPrepared) && !cinematic.m_canceled)
        yield return (object) null;
    }
    if (!cinematic.m_canceled)
    {
      cinematic.m_mainPlayer.Play();
      while (!cinematic.m_canceled && cinematic.m_mainPlayer.frame < 1L)
        yield return (object) null;
    }
    if (!cinematic.m_canceled)
    {
      cinematic.m_mainPlayer.targetCamera = cinematic.m_camera;
      cinematic.m_logoPlayer.targetCamera = cinematic.m_camera;
      cinematic.PlaySound();
    }
  }

  public void Update()
  {
    if (!InputCollection.GetAnyKey())
      return;
    if ((UnityEngine.Object) this.m_audioSource != (UnityEngine.Object) null && this.m_audioSource.isPlaying)
      this.m_audioSource.Stop();
    if ((UnityEngine.Object) this.m_mainPlayer != (UnityEngine.Object) null && this.m_mainPlayer.isPlaying)
      this.m_mainPlayer.Stop();
    if ((UnityEngine.Object) this.m_logoPlayer != (UnityEngine.Object) null && this.m_logoPlayer.isPlaying)
      this.m_logoPlayer.Stop();
    if (!this.m_started)
      return;
    this.OnPlayEnd(true);
  }

  private void PlaySound()
  {
    this.m_audioSource = SoundManager.Get().PlayClip(new SoundPlayClipArgs()
    {
      m_forcedAudioClip = (AudioClip) this.m_movieAudio,
      m_volume = new float?(1f),
      m_pitch = new float?(1f),
      m_category = new Global.SoundCategory?(Global.SoundCategory.FX),
      m_parentObject = this.SceneObject
    });
    SoundManager.Get().Set3d(this.m_audioSource, false);
    SoundManager.Get().SetIgnoreDucking(this.m_audioSource, true);
    this.m_soundDucker.StartDucking();
  }

  private void OnMainVideoComplete(VideoPlayer _)
  {
    this.m_mainPlayer.renderMode = VideoRenderMode.CameraFarPlane;
    this.m_logoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
    this.m_logoPlayer.Play();
  }

  private void OnLogoVideoComplete(VideoPlayer _) => this.OnPlayEnd(false);

  private void CreateCamera()
  {
    this.m_camera = new GameObject()
    {
      transform = {
        position = new Vector3(-9997.9f, -9998.9f, -9999.9f)
      }
    }.AddComponent<Camera>();
    this.m_camera.name = "Cinematic Background Camera";
    this.m_camera.clearFlags = CameraClearFlags.Color;
    this.m_camera.backgroundColor = Color.black;
    this.m_camera.depth = 1000f;
    this.m_camera.nearClipPlane = 0.01f;
    this.m_camera.farClipPlane = 0.02f;
    this.m_camera.allowHDR = false;
  }

  private void LoadMovie()
  {
    string path = "Movies/" + Cinematic.Hearthstone_Tavern_Abridged;
    if (PlatformSettings.IsMobile())
    {
      string str = ".mp4";
      if (Application.isEditor)
      {
        this.m_logoPlayer.url = Cinematic.Mobile_Assets_Path + path + str;
      }
      else
      {
        if (PlatformSettings.OS == OSCategory.Android)
          str = ".mkv";
        this.m_mainPlayer.url = PlatformFilePaths.GetAssetPath(path + str);
      }
    }
    else
      Processor.RunCoroutine(this.AwaitRequestThenCallback(Resources.LoadAsync<VideoClip>(path), new ObjectCallback(this.MovieLoaded)));
  }

  private void LoadLogo()
  {
    string str1 = Cinematic.Hearthstone_Tavern_Abridged_Logo;
    Locale locale = Localization.GetLocale();
    switch (locale)
    {
      case Locale.zhTW:
      case Locale.zhCN:
      case Locale.jaJP:
      case Locale.thTH:
        str1 = locale.ToString() + "/" + str1;
        break;
    }
    string path = "Movies/" + str1;
    if (PlatformSettings.IsMobile())
    {
      string str2 = ".mp4";
      if (Application.isEditor)
      {
        this.m_logoPlayer.url = Cinematic.Mobile_Assets_Path + path + str2;
      }
      else
      {
        if (PlatformSettings.OS == OSCategory.Android)
          str2 = ".mkv";
        this.m_logoPlayer.url = PlatformFilePaths.GetAssetPath(path + str2);
      }
    }
    else
      Processor.RunCoroutine(this.AwaitRequestThenCallback(Resources.LoadAsync<VideoClip>(path), new ObjectCallback(this.LogoLoaded)));
  }

  private IEnumerator AwaitRequestThenCallback(
    ResourceRequest request,
    ObjectCallback callback)
  {
    while (!request.isDone)
      yield return (object) null;
    callback((AssetReference) null, request.asset, (object) null);
  }

  private void AudioLoaded(
    AssetReference assetRef,
    AssetHandle<AudioClip> asset,
    object callbackData)
  {
    using (asset)
    {
      if (asset == null)
      {
        Error.AddDevFatal("Failed to load Cinematic Audio Track!");
      }
      else
      {
        if (this.m_canceled)
          return;
        AssetHandle.Set<AudioClip>(ref this.m_movieAudio, asset);
      }
    }
  }

  private void MovieLoaded(AssetReference assetRef, UnityEngine.Object asset, object callbackData)
  {
    if (asset == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("Failed to load Cinematic movie!");
      this.m_canceled = true;
    }
    else
    {
      if (this.m_canceled)
        return;
      this.m_mainPlayer.clip = asset as VideoClip;
      this.m_mainPlayer.Prepare();
    }
  }

  private void LogoLoaded(AssetReference assetRef, UnityEngine.Object asset, object callbackData)
  {
    if (asset == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("Failed to load Cinematic logo!");
      this.m_canceled = true;
    }
    else
    {
      if (this.m_canceled)
        return;
      this.m_logoPlayer.clip = asset as VideoClip;
      this.m_logoPlayer.Prepare();
    }
  }
}
