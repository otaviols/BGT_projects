using Blizzard.T5.Jobs;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.Core.Streaming;
using Hearthstone.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PreloadScreen : MonoBehaviour
{
  public GameObject m_updateFrame;
  public ProgressBar m_updateProgressBar;
  public TextMesh m_updateProgressText;
  private const int DOWNLOAD_MESSAGES_COUNT = 8;
  private const int CHECKING_MESSAGES_COUNT = 2;
  private const int DOWNLOAD_MESSAGES_INTERVAL = 10;
  private float m_updateFrameShownTime;
  private float m_progress;
  private double m_downloadSpeed;
  private bool m_isInitialDownloadComplete;
  private const float REPAIR_TOUCH_MAX_TIMEOUT = 1f;
  private const float REPAIR_TOUCH_VARIANCE_POSITION = 1f;

  private IGameDownloadManager DownloadManager => GameDownloadManagerProvider.Get();

  private void Start()
  {
    this.m_updateProgressBar.SetProgressBar(0.0f);
    Screen.sleepTimeout = -1;
    Processor.QueueJob(HearthstoneJobs.CreateJobFromAction("HearthstoneApplication.ShowPrivacyPolicyPopup", new Action(this.ShowPrivacyPolicyPopup), (object) typeof (UniversalInputManager)));
  }

  public int EnabledDoubleTapFingerCount { get; private set; }

  private void DoubleTapCheck()
  {
    if (Input.touchCount < 2 || Input.GetTouch(0).phase != TouchPhase.Began)
      return;
    bool flag = true;
    for (int index = 0; index < Input.touchCount & flag; ++index)
    {
      float deltaTime = Input.GetTouch(index).deltaTime;
      if ((double) deltaTime <= 0.0 || (double) deltaTime >= 1.0 || (double) Input.GetTouch(index).deltaPosition.magnitude >= 1.0)
        flag = false;
    }
    if (!flag)
      return;
    this.EnabledDoubleTapFingerCount = Input.touchCount;
  }

  private int GetCountInstalledLocales()
  {
    int installedLocales = 0;
    string[] names = Enum.GetNames(typeof (AssetVariantTags.Locale));
    List<string> values = new List<string>();
    foreach (string str in names)
    {
      if (!str.Equals("Global") && File.Exists(AssetBundleInfo.GetAssetBundlePath(string.Format("initial_base_{0}-0.unity3d", (object) str.ToLower()))))
      {
        ++installedLocales;
        values.Add(str);
      }
    }
    Options.Get().SetString(Option.INSTALLED_LOCALES, string.Join(",", (IEnumerable<string>) values));
    return installedLocales;
  }

  private void Update()
  {
    this.DoubleTapCheck();
    if (this.DownloadManager == null)
      return;
    if (this.DownloadManager.IsCompletedInitialBaseDownload())
    {
      if (!this.m_isInitialDownloadComplete)
      {
        HearthstoneApplication.SendStartupTimeTelemetry("GameDownloadManager.CompletedInitialBaseDownload");
        this.m_isInitialDownloadComplete = true;
      }
      this.m_updateProgressBar.SetProgressBar(1f);
      if (!((UnityEngine.Object) SplashScreen.Get() != (UnityEngine.Object) null) || !SplashScreen.Get().gameObject.activeInHierarchy)
        return;
      if (PlatformSettings.IsMobileRuntimeOS)
        TelemetryManager.Client().SendRepairPrestep(this.EnabledDoubleTapFingerCount, this.GetCountInstalledLocales());
      HearthstoneApplication.SendStartupTimeTelemetry("GameDownloadManager.EnteringSplashScreen");
      Log.Downloader.Print("killing preloadscreen");
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
    else
    {
      TagDownloadStatus currentDownloadStatus = this.DownloadManager.GetCurrentDownloadStatus();
      if (currentDownloadStatus == null)
        return;
      this.m_progress = currentDownloadStatus.Progress;
      this.m_downloadSpeed = this.DownloadManager.BytesPerSecond;
      if (this.DownloadManager.InterruptionReason == InterruptionReason.AgentImpeded)
      {
        this.m_updateProgressBar.SetLabel(GameStrings.Get("GLUE_LOADINGSCREEN_PROGRESS_IMPEDED"));
      }
      else
      {
        if (this.DownloadManager.InterruptionReason == InterruptionReason.Error && this.m_updateFrame.activeSelf)
          this.m_updateFrame.SetActive(false);
        if (!this.m_updateFrame.activeSelf)
        {
          Log.Downloader.Print("Preloadscreen setting bar active");
          this.m_updateFrame.SetActive(true);
          this.m_updateFrameShownTime = Time.realtimeSinceStartup;
        }
        float num1 = Time.realtimeSinceStartup - this.m_updateFrameShownTime;
        if (!currentDownloadStatus.Complete)
        {
          int num2 = 8;
          string str1 = "GLUE_LOADINGSCREEN_PROGRESS_UNITY_";
          if (PlatformSettings.RuntimeOS == OSCategory.Android)
          {
            num2 = 6;
            str1 = "GLUE_LOADINGSCREEN_PROGRESS_";
          }
          int num3 = (int) ((double) num1 / 10.0) % num2 + 1;
          this.m_updateProgressBar.SetLabel(GameStrings.Get(str1 + (object) num3));
          this.m_updateProgressBar.SetProgressBar(this.m_progress);
          string str2 = DownloadStatusView.FormatBytesAsHumanReadable(currentDownloadStatus.BytesRemaining);
          string str3 = DownloadStatusView.FormatBytesAsHumanReadable((long) this.m_downloadSpeed);
          if (!((UnityEngine.Object) this.m_updateProgressText != (UnityEngine.Object) null))
            return;
          this.m_updateProgressText.text = GameStrings.Format("GLUE_LOADINGSCREEN_PROGRESS_TEXT", (object) str2, (object) str3);
        }
        else
        {
          this.m_updateProgressBar.SetLabel(GameStrings.Get("GLUE_LOADINGSCREEN_CHECKING_UNITY_" + (object) ((int) ((double) num1 / 10.0) % 2 + 1)));
          this.m_updateProgressBar.SetProgressBar(1f);
        }
      }
    }
  }

  public void ShowPrivacyPolicyPopup()
  {
    if (PlatformSettings.LocaleVariant == LocaleVariant.China && !Options.Get().GetBool(Option.HAS_ACCEPTED_PRIVACY_POLICY_AND_EULA, false))
      Processor.QueueJob(HearthstoneJobs.CreateJobFromDependency("Load_PrivacyPolicyPopup", (IJobDependency) new LoadResource("Prefabs/PrivacyPolicyPopup", LoadResourceFlags.AutoInstantiateOnLoad | LoadResourceFlags.FailOnError)));
    else
      HearthstoneApplication.Get().DataTransferDependency.Callback();
  }
}
