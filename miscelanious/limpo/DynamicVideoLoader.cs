using Blizzard.T5.AssetManager;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.UI.Core;
using HutongGames.PlayMaker;
using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class DynamicVideoLoader : MonoBehaviour
{
  public VideoPlayer VideoPlayer;
  [Tooltip("Optional texture to display while video is loading for non-mobile devices")]
  public Texture InitialTexture;
  [Tooltip("Optional texture to display while video is loading for mobile devices")]
  public Texture InitialTexturePhone;
  [Tooltip("Render texture that the video player draws to.")]
  public Texture VideoPlaybackTexture;
  [Tooltip("Renderer to display the video and optional textures on.")]
  public Renderer MainDisplayRenderer;
  public PlayMakerFSM OptionalFSM;
  [Tooltip("If defined, OptionalFSM will be set to this state when the video starts playing.")]
  public string OptionalFSMState = "Playing";
  [Tooltip("Will automatically trigger the VideoPlayer if it has already configured (e.g. configured -> Ui hidden -> Ui shown)")]
  public bool ReTriggerPlayOnEnable;
  public AudioSource AudioSource;
  public SoundDef SoundDef;
  private string m_currentVideoLocation;
  private string m_currentFallbackTextureLocation;
  private bool m_requiresFallbackTexture;
  private Texture m_fallbackTexture;
  private IEnumerator m_textureSwapCoroutine;
  private bool m_hasConfiguredVideoPlayer;

  [Overridable]
  public string VideoLocation
  {
    get => this.m_currentVideoLocation;
    set
    {
      if (!(value != this.m_currentVideoLocation))
        return;
      this.m_currentVideoLocation = value;
      this.SetInitialTexture();
      if (string.IsNullOrEmpty(this.m_currentVideoLocation))
        return;
      AssetLoader.Get().LoadAsset<VideoClip>((AssetReference) this.m_currentVideoLocation, new AssetHandleCallback<VideoClip>(this.OnVideoLoaded), (object) this.m_currentVideoLocation);
    }
  }

  [Overridable]
  public string FallbackTextureLocation
  {
    get => this.m_currentFallbackTextureLocation;
    set
    {
      if (!(value != this.m_currentFallbackTextureLocation))
        return;
      this.m_currentFallbackTextureLocation = value;
      if (string.IsNullOrEmpty(this.m_currentFallbackTextureLocation))
        return;
      AssetLoader.Get().LoadAsset<Texture>((AssetReference) this.m_currentFallbackTextureLocation, new AssetHandleCallback<Texture>(this.OnFallbackTextureLoaded));
    }
  }

  private void OnEnable()
  {
    if (!this.ReTriggerPlayOnEnable || !this.m_hasConfiguredVideoPlayer || (Object) this.VideoPlayer == (Object) null || string.IsNullOrEmpty(this.m_currentVideoLocation) || (Object) this.VideoPlayer.clip == (Object) null || this.VideoPlayer.isPlaying)
      return;
    this.VideoPlayer.Play();
  }

  public void OnClosed()
  {
    this.m_hasConfiguredVideoPlayer = false;
    this.m_currentVideoLocation = "";
    this.m_currentFallbackTextureLocation = "";
    this.m_fallbackTexture = (Texture) null;
    this.m_requiresFallbackTexture = false;
    if ((Object) this.VideoPlayer != (Object) null)
      this.VideoPlayer.clip = (VideoClip) null;
    if ((Object) this.MainDisplayRenderer != (Object) null && (Object) this.VideoPlaybackTexture != (Object) null)
    {
      RendererExtension.GetMaterial(this.MainDisplayRenderer).mainTexture = this.VideoPlaybackTexture;
      RenderTexture videoPlaybackTexture = this.VideoPlaybackTexture as RenderTexture;
      if ((Object) videoPlaybackTexture != (Object) null)
      {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = videoPlaybackTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = active;
      }
    }
    if (this.m_textureSwapCoroutine == null)
      return;
    this.StopCoroutine(this.m_textureSwapCoroutine);
  }

  private void SetInitialTexture()
  {
    if ((Object) this.VideoPlaybackTexture == (Object) null)
      Error.AddDevFatal("Dynamic video loader configured without video playback texture.");
    else if ((Object) this.MainDisplayRenderer == (Object) null)
    {
      Error.AddDevFatal("Dynamic video loader configured without main display renderer.");
    }
    else
    {
      if (PlatformSettings.Screen == ScreenCategory.Phone && (Object) this.InitialTexturePhone != (Object) null)
        RendererExtension.GetMaterial(this.MainDisplayRenderer).mainTexture = this.InitialTexturePhone;
      else if ((Object) this.InitialTexture != (Object) null)
        RendererExtension.GetMaterial(this.MainDisplayRenderer).mainTexture = this.InitialTexture;
      this.m_hasConfiguredVideoPlayer = false;
    }
  }

  private void OnFallbackTextureLoaded(
    AssetReference assetRef,
    AssetHandle<Texture> asset,
    object callbackData)
  {
    if (string.IsNullOrEmpty(this.m_currentVideoLocation) || string.IsNullOrEmpty(this.m_currentFallbackTextureLocation))
      return;
    if ((Object) this.VideoPlayer == (Object) null)
    {
      Error.AddDevFatal("Dynamic video loader configured without connection to a video playback component.");
    }
    else
    {
      if ((Object) this.VideoPlayer.clip != (Object) null)
        return;
      if ((Object) this.VideoPlaybackTexture == (Object) null)
        Error.AddDevFatal("Dynamic video loader configured without video playback texture.");
      else if ((Object) this.MainDisplayRenderer == (Object) null)
        Error.AddDevFatal("Dynamic video loader configured without main display renderer.");
      else if (this.m_requiresFallbackTexture)
        RendererExtension.GetMaterial(this.MainDisplayRenderer).mainTexture = asset.Asset;
      else
        this.m_fallbackTexture = asset.Asset;
    }
  }

  private void OnVideoLoaded(AssetReference assetRef, AssetHandle<VideoClip> asset, object assetId)
  {
    if (string.IsNullOrEmpty(this.m_currentVideoLocation))
      return;
    if ((Object) this.VideoPlayer == (Object) null)
      Error.AddDevFatal("Dynamic video loader configured without connection to a video playback component.");
    else if ((Object) this.VideoPlaybackTexture == (Object) null)
      Error.AddDevFatal("Dynamic video loader configured without video playback texture.");
    else if ((Object) this.MainDisplayRenderer == (Object) null)
      Error.AddDevFatal("Dynamic video loader configured without main display renderer.");
    else if (asset == null)
    {
      Error.AddDevWarning("Missing Asset", "Dynamic video loader failed to load video at " + assetId.ToString());
      if ((Object) this.m_fallbackTexture != (Object) null)
        RendererExtension.GetMaterial(this.MainDisplayRenderer).mainTexture = this.m_fallbackTexture;
      else
        this.m_requiresFallbackTexture = true;
    }
    else
    {
      this.VideoPlayer.clip = asset.Asset;
      this.VideoPlayer.prepareCompleted += new VideoPlayer.EventHandler(this.OnReadyToPlay);
      this.VideoPlayer.Prepare();
    }
  }

  private void OnReadyToPlay(VideoPlayer source)
  {
    if (string.IsNullOrEmpty(this.m_currentVideoLocation))
      return;
    this.VideoPlayer.started += new VideoPlayer.EventHandler(this.ShowVideo);
    SoundManager.Get().RegisterVideoSoundSource(this.AudioSource, this.SoundDef);
    this.VideoPlayer.SetTargetAudioSource((ushort) 0, this.AudioSource);
    this.VideoPlayer.Play();
    if ((Object) this.OptionalFSM != (Object) null && !string.IsNullOrEmpty(this.OptionalFSMState))
    {
      foreach (FsmState fsmState in this.OptionalFSM.FsmStates)
      {
        if (fsmState.Name == this.OptionalFSMState)
        {
          this.OptionalFSM.SetState(this.OptionalFSMState);
          break;
        }
      }
    }
    this.VideoPlayer.prepareCompleted -= new VideoPlayer.EventHandler(this.OnReadyToPlay);
  }

  private void ShowVideo(VideoPlayer player)
  {
    this.m_textureSwapCoroutine = this.SwapTexturesWhenReady();
    this.StartCoroutine(this.m_textureSwapCoroutine);
    this.VideoPlayer.started -= new VideoPlayer.EventHandler(this.ShowVideo);
  }

  private IEnumerator SwapTexturesWhenReady()
  {
    while (this.VideoPlayer.frame < 1L)
      yield return (object) null;
    yield return (object) null;
    RendererExtension.GetMaterial(this.MainDisplayRenderer).mainTexture = this.VideoPlaybackTexture;
    this.m_hasConfiguredVideoPlayer = true;
    this.m_textureSwapCoroutine = (IEnumerator) null;
  }
}
