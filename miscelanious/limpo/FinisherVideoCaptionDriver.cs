using Hearthstone.UI.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FinisherVideoCaptionDriver : MonoBehaviour
{
  [Tooltip("The video player to attach to for deriving timing information.")]
  public VideoPlayer VideoPlayer;
  [Tooltip("Ubertext that will display the caption titles.")]
  public UberText CaptionTitleText;
  [Tooltip("Ubertext that will display the caption Subtitles.")]
  public UberText CaptionSubtitleText;
  [Tooltip("Animation curve (range 0-1 for height and time) that drives fading in text. Should start at 0 and end at 1.")]
  public AnimationCurve FadeInCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1f, 1f);
  [Tooltip("Time for fade in transition for captions. If 0, transition is instant rather than fade.")]
  public double FadeInSeconds = 0.2;
  [Tooltip("Animation curve (range 0-1 for height and time) that drives fading out text. Should start at 1 and end at 0.")]
  public AnimationCurve FadeOutCurve = AnimationCurve.EaseInOut(0.0f, 1f, 1f, 0.0f);
  [Tooltip("Time for fade out transition for captions. If 0, transition is instant rather than fade.")]
  public double FadeOutSeconds = 0.2;
  private int m_finisherId = -1;
  private IEnumerator m_showCaptionsCoroutine;
  private double m_aboutToLoopTime = double.MaxValue;

  [Overridable]
  public int FinisherID
  {
    get => this.m_finisherId;
    set
    {
      if (value == this.m_finisherId)
        return;
      if (value <= 0)
      {
        if (!Application.isPlaying)
          return;
        Error.AddDevWarning("Finisher Video Caption Drive", "Finisher Video Caption Driver instructed to play back video when no finisher was specified.");
      }
      else if (GameDbf.BattlegroundsFinisher.GetRecord(value) == null)
      {
        if (!Application.isPlaying)
          return;
        Error.AddDevWarning("Finisher Video Caption Drive", string.Format("Finisher Video Caption Driver instructed to play back video for non-existent finisher ID {0}.", (object) value));
      }
      else
      {
        this.m_finisherId = value;
        if (!((Object) this.VideoPlayer != (Object) null) || !this.VideoPlayer.isPlaying)
          return;
        this.ShowCaptions();
      }
    }
  }

  private void Start()
  {
    if ((Object) this.CaptionTitleText == (Object) null)
      Error.AddDevFatal("Finisher Video Caption Driver configured without target title ubertext.");
    else if ((Object) this.CaptionSubtitleText == (Object) null)
      Error.AddDevFatal("Finisher Video Caption Driver configured without target subtitle ubertext.");
    else if ((Object) this.VideoPlayer == (Object) null)
    {
      Error.AddDevFatal("Finisher Video Caption Driver configured without target video player.");
    }
    else
    {
      this.VideoPlayer.started += new VideoPlayer.EventHandler(this.OnVideoStarted);
      this.VideoPlayer.loopPointReached += new VideoPlayer.EventHandler(this.OnVideoLooped);
    }
  }

  public void OnClose()
  {
    this.m_finisherId = -1;
    this.StopAndHideCaptions();
  }

  private void OnVideoStarted(VideoPlayer source) => this.ShowCaptions();

  private void OnVideoLooped(VideoPlayer source)
  {
    this.m_aboutToLoopTime = source.time;
    this.StopAndHideCaptions();
    this.ShowCaptions();
  }

  private void ShowCaptions()
  {
    if ((Object) this.CaptionTitleText == (Object) null)
      Error.AddDevFatal("Finisher Video Caption Driver configured without target title ubertext.");
    else if ((Object) this.CaptionSubtitleText == (Object) null)
      Error.AddDevFatal("Finisher Video Caption Driver configured without target subtitle ubertext.");
    else if ((Object) this.VideoPlayer == (Object) null)
      Error.AddDevFatal("Finisher Video Caption Driver configured without target video player.");
    else if (this.m_finisherId <= 0)
    {
      Error.AddDevWarning("Finisher Video Caption Drive", "Finisher Video Caption Driver instructed to play back video when no finisher was specified.");
    }
    else
    {
      BattlegroundsFinisherDbfRecord record = GameDbf.BattlegroundsFinisher.GetRecord(this.m_finisherId);
      if (record == null)
      {
        Error.AddDevWarning("Finisher Video Caption Drive", string.Format("Finisher Video Caption Driver instructed to play back video for non-existent finisher ID {0}.", (object) this.m_finisherId));
      }
      else
      {
        List<DetailsVideoCueDbfRecord> videoCues = record.VideoCues;
        if (videoCues == null || videoCues.Count == 0)
        {
          Error.AddDevWarning("Finisher Video Caption Drive", string.Format("Finisher Video Caption Driver instructed to play back video for finisher ID {0} which does not have cues specified.", (object) this.m_finisherId));
        }
        else
        {
          if (this.m_showCaptionsCoroutine != null)
            this.StopCoroutine(this.m_showCaptionsCoroutine);
          this.m_showCaptionsCoroutine = this.ShowCaptionsCoroutine();
          this.StartCoroutine(this.m_showCaptionsCoroutine);
        }
      }
    }
  }

  private void StopAndHideCaptions()
  {
    if ((Object) this.CaptionTitleText == (Object) null)
      Error.AddDevFatal("Finisher Video Caption Driver configured without target title ubertext.");
    else if ((Object) this.CaptionSubtitleText == (Object) null)
      Error.AddDevFatal("Finisher Video Caption Driver configured without target subtitle ubertext.");
    else if ((Object) this.VideoPlayer == (Object) null)
    {
      Error.AddDevFatal("Finisher Video Caption Driver configured without target video player.");
    }
    else
    {
      if (this.m_showCaptionsCoroutine != null)
      {
        this.StopCoroutine(this.m_showCaptionsCoroutine);
        this.m_showCaptionsCoroutine = (IEnumerator) null;
      }
      this.CaptionTitleText.TextAlpha = 0.0f;
      this.CaptionSubtitleText.TextAlpha = 0.0f;
    }
  }

  private IEnumerator ShowCaptionsCoroutine()
  {
    List<DetailsVideoCueDbfRecord> cues = GameDbf.BattlegroundsFinisher.GetRecord(this.m_finisherId).VideoCues;
    double time;
    for (time = this.VideoPlayer.time; time >= this.m_aboutToLoopTime; time = this.VideoPlayer.time)
      yield return (object) null;
    this.m_aboutToLoopTime = double.MaxValue;
    for (int cueIdx = 0; cueIdx < cues.Count; ++cueIdx)
    {
      DetailsVideoCueDbfRecord currentCue = cues[cueIdx];
      if (time < currentCue.StartSeconds)
      {
        yield return (object) new WaitForSeconds((float) (currentCue.StartSeconds - time));
        time = this.VideoPlayer.time;
      }
      double cueFadeInEndSeconds = currentCue.StartSeconds + this.FadeInSeconds;
      double cueEndSeconds = this.VideoPlayer.length;
      if (cueIdx + 1 < cues.Count)
        cueEndSeconds = cues[cueIdx + 1].StartSeconds;
      double cueFadeOutStartSeconds = cueEndSeconds - this.FadeOutSeconds;
      if (cueFadeOutStartSeconds < cueFadeInEndSeconds)
        Error.AddDevWarning("Finisher Video Caption Drive", string.Format("Finisher Video Caption Driver for finisher ID {0}, caption #{1} has a fade out start before a fade-in ends, which will lead to clipped fade-outs.", (object) this.m_finisherId, (object) cueIdx));
      this.CaptionTitleText.Text = (string) currentCue.CaptionTitle;
      this.CaptionSubtitleText.Text = (string) currentCue.CaptionSubtitle;
      this.CaptionTitleText.TextAlpha = 0.0f;
      this.CaptionSubtitleText.TextAlpha = 0.0f;
      for (; time < cueFadeInEndSeconds && this.FadeInSeconds > 0.0; time = this.VideoPlayer.time)
      {
        float num = this.FadeInCurve.Evaluate(Mathf.Clamp01((float) ((time - currentCue.StartSeconds) / this.FadeInSeconds)));
        this.CaptionTitleText.TextAlpha = num;
        this.CaptionSubtitleText.TextAlpha = num;
        yield return (object) null;
      }
      if (time < cueFadeOutStartSeconds)
      {
        this.CaptionTitleText.TextAlpha = 1f;
        this.CaptionSubtitleText.TextAlpha = 1f;
        yield return (object) new WaitForSeconds((float) (cueFadeOutStartSeconds - time));
        time = this.VideoPlayer.time;
      }
      for (; time < cueEndSeconds && this.FadeOutSeconds > 0.0; time = this.VideoPlayer.time)
      {
        float num = this.FadeOutCurve.Evaluate(Mathf.Clamp01((float) ((time - cueFadeOutStartSeconds) / this.FadeOutSeconds)));
        this.CaptionTitleText.TextAlpha = num;
        this.CaptionSubtitleText.TextAlpha = num;
        yield return (object) null;
      }
      currentCue = (DetailsVideoCueDbfRecord) null;
    }
    this.m_showCaptionsCoroutine = (IEnumerator) null;
  }
}
