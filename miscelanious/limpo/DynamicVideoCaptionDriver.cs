using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class DynamicVideoCaptionDriver : MonoBehaviour
{
  [SerializeField]
  [Tooltip("The video player to attach to for deriving timing information.")]
  private VideoPlayer m_VideoPlayer;
  [SerializeField]
  [Tooltip("Ubertext that will display the caption titles.")]
  private UberText m_CaptionTitleText;
  [Tooltip("Ubertext that will display the caption Subtitles.")]
  [SerializeField]
  private UberText m_CaptionDescText;
  [SerializeField]
  [Tooltip("Animation curve (range 0-1 for height and time) that drives fading in text. Should start at 0 and end at 1.")]
  private AnimationCurve m_FadeInCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);
  [SerializeField]
  [Tooltip("Time for fade in transition for captions. If 0, transition is instant rather than fade.")]
  private double m_FadeInSeconds = 0.2;
  [SerializeField]
  [Tooltip("Animation curve (range 0-1 for height and time) that drives fading out text. Should start at 1 and end at 0.")]
  private AnimationCurve m_FadeOutCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
  [SerializeField]
  [Tooltip("Time for fade out transition for captions. If 0, transition is instant rather than fade.")]
  private double m_FadeOutSeconds = 0.2;
  private List<VideoCaptionKey> m_captionKeys;
  private Coroutine m_showCaptionsCoroutine;
  private double m_aboutToLoopTime = double.MaxValue;
  private bool m_isInitialized;

  public List<VideoCaptionKey> VideoCaptionKeys
  {
    get => this.m_captionKeys;
    set
    {
      if (value == this.m_captionKeys)
        return;
      if (value == null || value.Count == 0)
      {
        this.m_captionKeys = (List<VideoCaptionKey>) null;
        this.HideCaptions();
      }
      else
      {
        this.m_captionKeys = value;
        if (!((Object) this.m_VideoPlayer != (Object) null) || !this.m_VideoPlayer.isPlaying)
          return;
        this.ShowCaptions();
      }
    }
  }

  public void StopCaptions() => this.HideCaptions();

  private void Awake()
  {
    if ((Object) this.m_CaptionTitleText == (Object) null)
      Error.AddDevFatal("DynamicVideoCaptionDriver configured without target title ubertext.");
    else if ((Object) this.m_CaptionDescText == (Object) null)
      Error.AddDevFatal("DynamicVideoCaptionDriver configured without target subtitle ubertext.");
    else if ((Object) this.m_VideoPlayer == (Object) null)
    {
      Error.AddDevFatal("DynamicVideoCaptionDriver configured without target video player.");
    }
    else
    {
      this.m_isInitialized = true;
      this.HideCaptions();
      this.m_VideoPlayer.started += new VideoPlayer.EventHandler(this.OnVideoStarted);
      this.m_VideoPlayer.loopPointReached += new VideoPlayer.EventHandler(this.OnVideoLooped);
    }
  }

  private void OnDestroy()
  {
    if (!((Object) this.m_VideoPlayer != (Object) null))
      return;
    this.m_VideoPlayer.started -= new VideoPlayer.EventHandler(this.OnVideoStarted);
    this.m_VideoPlayer.loopPointReached -= new VideoPlayer.EventHandler(this.OnVideoLooped);
  }

  private void OnEnable()
  {
    if (!this.m_isInitialized || this.m_captionKeys == null)
      return;
    this.ShowCaptions();
  }

  private void OnDisable() => this.HideCaptions();

  private void OnVideoStarted(VideoPlayer source) => this.ShowCaptions();

  private void OnVideoLooped(VideoPlayer source)
  {
    this.HideCaptions();
    this.m_aboutToLoopTime = source.time;
    this.ShowCaptions();
  }

  private void ShowCaptions()
  {
    if (!this.m_isInitialized)
      Error.AddDevFatal("DynamicVideoCaptionDriver unable to show captions due to failure to initialize!");
    else if (this.m_captionKeys == null || this.m_captionKeys.Count == 0)
    {
      this.HideCaptions();
    }
    else
    {
      if (this.m_showCaptionsCoroutine != null)
        this.StopCoroutine(this.m_showCaptionsCoroutine);
      this.m_showCaptionsCoroutine = this.StartCoroutine(this.ShowCaptionsCoroutine());
    }
  }

  private void HideCaptions()
  {
    if (!this.m_isInitialized)
      return;
    if (this.m_showCaptionsCoroutine != null)
    {
      this.StopCoroutine(this.m_showCaptionsCoroutine);
      this.m_showCaptionsCoroutine = (Coroutine) null;
    }
    this.m_CaptionTitleText.Text = string.Empty;
    this.m_CaptionDescText.Text = string.Empty;
    this.m_CaptionTitleText.TextAlpha = 0.0f;
    this.m_CaptionDescText.TextAlpha = 0.0f;
  }

  private IEnumerator ShowCaptionsCoroutine()
  {
    List<VideoCaptionKey> keys = this.m_captionKeys;
    double time;
    for (time = this.m_VideoPlayer.time; time >= this.m_aboutToLoopTime; time = this.m_VideoPlayer.time)
      yield return (object) null;
    this.m_aboutToLoopTime = double.MaxValue;
    for (int cueIdx = 0; cueIdx < keys.Count; ++cueIdx)
    {
      VideoCaptionKey currentKey = keys[cueIdx];
      if (time < (double) currentKey.TimeStampSeconds)
      {
        yield return (object) new WaitForSeconds(currentKey.TimeStampSeconds - (float) time);
        time = this.m_VideoPlayer.time;
      }
      double cueFadeInEndSeconds = (double) currentKey.TimeStampSeconds + this.m_FadeInSeconds;
      double cueEndSeconds = this.m_VideoPlayer.length;
      if (cueIdx + 1 < keys.Count)
        cueEndSeconds = (double) keys[cueIdx + 1].TimeStampSeconds;
      double cueFadeOutStartSeconds = cueEndSeconds - this.m_FadeOutSeconds;
      if (cueFadeOutStartSeconds < cueFadeInEndSeconds)
        Error.AddDevWarning("Finisher Video Caption Drive", string.Format("{0} caption #{1} has a fade out start before a fade-in ends, which will lead to clipped fade-outs.", (object) nameof (DynamicVideoCaptionDriver), (object) cueIdx));
      this.m_CaptionTitleText.Text = currentKey.TitleLocalizedString;
      this.m_CaptionDescText.Text = currentKey.DescLocalizedString;
      this.m_CaptionTitleText.TextAlpha = 0.0f;
      this.m_CaptionDescText.TextAlpha = 0.0f;
      for (; time < cueFadeInEndSeconds && this.m_FadeInSeconds > 0.0; time = this.m_VideoPlayer.time)
      {
        float num = this.m_FadeInCurve.Evaluate(Mathf.Clamp01((float) ((time - (double) currentKey.TimeStampSeconds) / this.m_FadeInSeconds)));
        this.m_CaptionTitleText.TextAlpha = num;
        this.m_CaptionDescText.TextAlpha = num;
        yield return (object) null;
      }
      if (time < cueFadeOutStartSeconds)
      {
        this.m_CaptionTitleText.TextAlpha = 1f;
        this.m_CaptionDescText.TextAlpha = 1f;
        yield return (object) new WaitForSeconds((float) (cueFadeOutStartSeconds - time));
        time = this.m_VideoPlayer.time;
      }
      for (; time < cueEndSeconds && this.m_FadeOutSeconds > 0.0; time = this.m_VideoPlayer.time)
      {
        float num = this.m_FadeOutCurve.Evaluate(Mathf.Clamp01((float) ((time - cueFadeOutStartSeconds) / this.m_FadeOutSeconds)));
        this.m_CaptionTitleText.TextAlpha = num;
        this.m_CaptionDescText.TextAlpha = num;
        yield return (object) null;
      }
      currentKey = (VideoCaptionKey) null;
    }
    this.m_showCaptionsCoroutine = (Coroutine) null;
  }
}
