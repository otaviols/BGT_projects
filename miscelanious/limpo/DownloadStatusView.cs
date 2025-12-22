using Hearthstone.Core.Streaming;
using Hearthstone.Streaming;
using System;
using System.Collections;
using UnityEngine;

public class DownloadStatusView : MonoBehaviour
{
  private static Color s_normalColor = Color.white;
  private static Color s_warningColor = Color.yellow;
  private static Color s_errorColor = Color.red;
  [SerializeField]
  private UberText m_contentDetailsText;
  [SerializeField]
  private UberText m_transferDetailsText;
  [SerializeField]
  private ProgressBar m_progressBar;
  [SerializeField]
  private UIBButton m_button;
  [SerializeField]
  private float m_crossfadeSeconds = 1f;
  [SerializeField]
  private float m_secondsUntilCrossfade = 2f;
  [SerializeField]
  private bool m_shortenText;
  private string m_remaningBytesStr = string.Empty;
  private bool m_isShowingProgressPercentage = true;
  private Coroutine m_crossfadeCoroutine;
  private static string[] s_suffixes = new string[4]
  {
    "GLOBAL_ASSET_DOWNLOAD_BYTE_SYMBOL",
    "GLOBAL_ASSET_DOWNLOAD_KILOBYTE_SYMBOL",
    "GLOBAL_ASSET_DOWNLOAD_MEGABYTE_SYMBOL",
    "GLOBAL_ASSET_DOWNLOAD_GIGABYTE_SYMBOL"
  };

  private IGameDownloadManager DownloadManager => GameDownloadManagerProvider.Get();

  private void Update()
  {
    if (this.DownloadManager == null)
    {
      this.SetButtonState(false);
    }
    else
    {
      if (this.DownloadManager.IsInterrupted)
      {
        this.StartCrossfade();
        if ((UnityEngine.Object) this.m_transferDetailsText != (UnityEngine.Object) null)
        {
          switch (this.DownloadManager.InterruptionReason)
          {
            case InterruptionReason.Disabled:
              this.m_transferDetailsText.TextColor = DownloadStatusView.s_errorColor;
              this.m_transferDetailsText.Text = GameStrings.Get("GLOBAL_ASSET_DOWNLOAD_ERROR_DOWNLOAD_DISABLED");
              break;
            case InterruptionReason.Paused:
              this.m_transferDetailsText.TextColor = DownloadStatusView.s_normalColor;
              this.m_transferDetailsText.Text = GameStrings.Get("GLOBAL_ASSET_DOWNLOAD_PAUSED");
              break;
            case InterruptionReason.AwaitingWifi:
              this.m_transferDetailsText.TextColor = DownloadStatusView.s_warningColor;
              this.m_transferDetailsText.Text = GameStrings.Format("GLOBAL_ASSET_DOWNLOAD_ERROR_CELLULAR_DISABLED");
              break;
            case InterruptionReason.DiskFull:
              this.m_transferDetailsText.TextColor = DownloadStatusView.s_warningColor;
              this.m_transferDetailsText.Text = GameStrings.Format("GLOBAL_ASSET_DOWNLOAD_ERROR_OUT_OF_STORAGE");
              break;
            case InterruptionReason.AgentImpeded:
              this.m_transferDetailsText.TextColor = DownloadStatusView.s_errorColor;
              this.m_transferDetailsText.Text = GameStrings.Format("GLOBAL_ASSET_DOWNLOAD_ERROR_AGENT_IMPEDED", (object) this.m_remaningBytesStr);
              break;
            case InterruptionReason.Fetching:
              this.m_transferDetailsText.TextColor = DownloadStatusView.s_warningColor;
              this.m_transferDetailsText.Text = GameStrings.Format("GLOBAL_ASSET_DOWNLOAD_AWAITING_FETCH");
              break;
          }
        }
      }
      ContentDownloadStatus contentDownloadStatus = this.DownloadManager.GetContentDownloadStatus(DownloadTags.Content.Base);
      if (!DownloadStatusView.HasDownloadStarted(contentDownloadStatus))
      {
        this.SetStartingProgressAndText();
      }
      else
      {
        float progress1 = contentDownloadStatus.Progress;
        this.m_remaningBytesStr = DownloadStatusView.FormatBytesAsHumanReadable(contentDownloadStatus.BytesTotal - contentDownloadStatus.BytesDownloaded);
        this.SetButtonState(this.DownloadManager.IsAnyDownloadRequestedAndIncomplete);
        if (!this.DownloadManager.IsInterrupted)
        {
          if (!this.DownloadManager.IsAnyDownloadRequestedAndIncomplete)
          {
            this.StopCrossfade();
            if ((UnityEngine.Object) this.m_contentDetailsText != (UnityEngine.Object) null)
            {
              this.m_contentDetailsText.Text = GameStrings.Get("GLOBAL_ASSET_DOWNLOAD_COMPLETE");
              this.m_contentDetailsText.TextAlpha = 1f;
            }
            if ((UnityEngine.Object) this.m_transferDetailsText != (UnityEngine.Object) null)
            {
              this.m_transferDetailsText.TextColor = DownloadStatusView.s_normalColor;
              this.m_transferDetailsText.Text = string.Empty;
            }
          }
          else
          {
            this.StartCrossfade();
            if ((UnityEngine.Object) this.m_transferDetailsText != (UnityEngine.Object) null)
            {
              string str = DownloadStatusView.FormatBytesAsHumanReadable((long) this.DownloadManager.BytesPerSecond);
              this.m_transferDetailsText.TextColor = DownloadStatusView.s_normalColor;
              this.m_transferDetailsText.Text = GameStrings.Format(this.m_shortenText ? "GLOBAL_ASSET_DOWNLOAD_STATUS_SHORT" : "GLOBAL_ASSET_DOWNLOAD_STATUS", (object) this.m_remaningBytesStr, (object) str);
            }
          }
        }
        double progress2 = (double) Mathf.Clamp01(progress1);
        if ((UnityEngine.Object) this.m_progressBar != (UnityEngine.Object) null)
          this.m_progressBar.SetProgressBar((float) progress2);
        if (!((UnityEngine.Object) this.m_contentDetailsText != (UnityEngine.Object) null) || !this.DownloadManager.IsAnyDownloadRequestedAndIncomplete)
          return;
        string format = GameStrings.Get("GLOBAL_ASSET_DOWNLOAD_INTENTION_PAUSED");
        string str1 = !this.m_isShowingProgressPercentage ? GameStrings.Get(this.LocalizedDescriptionForDownloadStatus(contentDownloadStatus)) : string.Format("{0:0.}%", (object) (progress2 * 100.0));
        this.m_contentDetailsText.Text = this.DownloadManager.IsInterrupted ? string.Format(format, (object) str1) : str1;
      }
    }
  }

  private static bool HasDownloadStarted(ContentDownloadStatus baseContentStatus) => baseContentStatus != null && baseContentStatus.BytesTotal > 0L;

  private void SetStartingProgressAndText()
  {
    this.SetProgressBarToZero();
    this.SetStartingContentDetailsText();
  }

  private void SetStartingContentDetailsText()
  {
    if ((UnityEngine.Object) this.m_contentDetailsText == (UnityEngine.Object) null)
      return;
    this.m_contentDetailsText.Text = this.GetStartingTextForContentDetails();
  }

  private string GetStartingTextForContentDetails() => this.DownloadManager.InterruptionReason == InterruptionReason.Disabled ? string.Empty : GameStrings.Format("GLOBAL_ASSET_INTENTION_UNINITIALIZED");

  private void SetProgressBarToZero()
  {
    if ((UnityEngine.Object) this.m_progressBar == (UnityEngine.Object) null)
      return;
    this.m_progressBar.SetProgressBar(0.0f);
  }

  private string LocalizedDescriptionForDownloadStatus(ContentDownloadStatus downloadStatus)
  {
    if (downloadStatus.ContentTag == DownloadTags.GetTagString(DownloadTags.Content.Base))
    {
      if (downloadStatus.InProgressQualityTag == DownloadTags.Quality.Fonts)
        return "GLOBAL_ASSET_INTENTION_DOWNLOADING_FONTS";
      if (downloadStatus.InProgressQualityTag == DownloadTags.Quality.PortHigh)
        return "GLOBAL_ASSET_INTENTION_DOWNLOADING_HIGH_RES_PORTRAITS";
      if (downloadStatus.InProgressQualityTag == DownloadTags.Quality.PortPremium)
        return "GLOBAL_ASSET_INTENTION_DOWNLOADING_PREMIUM_ANIMATIONS";
      if (downloadStatus.InProgressQualityTag == DownloadTags.Quality.SoundSpell)
        return "GLOBAL_ASSET_INTENTION_DOWNLOADING_SPELL_SOUNDS";
      if (downloadStatus.InProgressQualityTag == DownloadTags.Quality.SoundLegend)
        return "GLOBAL_ASSET_INTENTION_DOWNLOADING_LEGEND_STINGERS";
      if (downloadStatus.InProgressQualityTag == DownloadTags.Quality.MusicExpansion)
        return "GLOBAL_ASSET_INTENTION_DOWNLOADING_EXPANSION_MUSIC";
      if (downloadStatus.InProgressQualityTag == DownloadTags.Quality.SoundOtherMinion)
        return "GLOBAL_ASSET_INTENTION_DOWNLOADING_OTHER_MINION_SOUNDS";
      if (downloadStatus.InProgressQualityTag == DownloadTags.Quality.PlaySounds)
        return "GLOBAL_ASSET_INTENTION_DOWNLOADING_MINION_PLAY_SOUNDS";
      if (downloadStatus.InProgressQualityTag == DownloadTags.Quality.SoundMission)
        return "GLOBAL_ASSET_INTENTION_DOWNLOADING_MISSION_SOUNDS";
      if (downloadStatus.InProgressQualityTag == DownloadTags.Quality.HeroMusic)
        return "GLOBAL_ASSET_INTENTION_DOWNLOADING_HERO_MUSIC";
    }
    return "";
  }

  private void OnDisable() => this.m_crossfadeCoroutine = (Coroutine) null;

  private void StartCrossfade()
  {
    if (this.m_crossfadeCoroutine != null || !((UnityEngine.Object) this.m_contentDetailsText != (UnityEngine.Object) null))
      return;
    this.m_crossfadeCoroutine = this.StartCoroutine(this.CrossfadeBetweenProgressAndContentDetailsText());
  }

  private void StopCrossfade()
  {
    if (this.m_crossfadeCoroutine == null)
      return;
    this.StopCoroutine(this.m_crossfadeCoroutine);
    this.m_crossfadeCoroutine = (Coroutine) null;
  }

  private IEnumerator CrossfadeBetweenProgressAndContentDetailsText()
  {
    DownloadStatusView downloadStatusView = this;
    downloadStatusView.m_contentDetailsText.TextAlpha = 0.0f;
    while (true)
    {
      downloadStatusView.m_isShowingProgressPercentage = !downloadStatusView.m_isShowingProgressPercentage;
      // ISSUE: reference to a compiler-generated method
      yield return (object) downloadStatusView.LerpBetweenValues(downloadStatusView.m_crossfadeSeconds, 0.0f, 1f, new Action<float>(downloadStatusView.\u003CCrossfadeBetweenProgressAndContentDetailsText\u003Eb__25_0));
      yield return (object) new WaitForSeconds(downloadStatusView.m_secondsUntilCrossfade);
      // ISSUE: reference to a compiler-generated method
      yield return (object) downloadStatusView.LerpBetweenValues(downloadStatusView.m_crossfadeSeconds, 1f, 0.0f, new Action<float>(downloadStatusView.\u003CCrossfadeBetweenProgressAndContentDetailsText\u003Eb__25_1));
    }
  }

  private IEnumerator LerpBetweenValues(
    float duration,
    float from,
    float to,
    Action<float> onUpdate)
  {
    float timeLeft = duration;
    while ((double) timeLeft >= 0.0)
    {
      onUpdate(Mathf.Lerp(to, from, timeLeft / duration));
      timeLeft -= Time.deltaTime;
      yield return (object) null;
    }
  }

  private void SetButtonState(bool state)
  {
    if (!((UnityEngine.Object) this.m_button != (UnityEngine.Object) null) || state == this.m_button.IsEnabled())
      return;
    this.m_button.SetEnabled(state);
    this.m_button.Flip(state);
  }

  public static string FormatBytesAsHumanReadable(long bytes)
  {
    int b = 0;
    long num1 = 0;
    long num2 = 0;
    for (; bytes > 0L && b < DownloadStatusView.s_suffixes.Length; bytes /= 1024L)
    {
      ++b;
      num2 = num1;
      num1 = bytes % 1024L;
    }
    int num3 = Mathf.Max(1, b);
    int num4 = Mathf.RoundToInt((float) ((double) num2 * 10.0 / 1024.0));
    if (num4 == 10)
    {
      ++num1;
      num4 = 0;
    }
    return string.Format(GameStrings.Get("GLOBAL_ASSET_DOWNLOAD_STATUS_DECIMAL_FORMAT"), (object) num1, (object) num4, (object) GameStrings.Get(DownloadStatusView.s_suffixes[num3 - 1]));
  }
}
