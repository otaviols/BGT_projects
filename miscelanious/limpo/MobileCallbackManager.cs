using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class MobileCallbackManager : MonoBehaviour, IService
{
  private const string CHINESE_CURRENCY_CODE = "CNY";
  private const string CHINESE_COUNTRY_CODE = "CN";
  private const char RECEIPT_DATA_DELIMITER = '|';
  private const int LARGE_RECEIPT_CHAR_THRESHOLD = 9788;
  private const int MAX_TRIM_DELAY_SECONDS = 10;
  private ulong m_nextTrimAvailableTime;
  private ulong m_trimDelay = 1;

  public static string VersionCodeInStore { get; protected set; } = string.Empty;

  public static bool IsReadyVersionCodeInStore => AndroidDeviceSettings.Get().GetAndroidStore() != AndroidStore.GOOGLE || !string.IsNullOrEmpty(MobileCallbackManager.VersionCodeInStore);

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    MobileCallbackManager mobileCallbackManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    mobileCallbackManager.gameObject.AddComponent<CloudStorageManager>();
    mobileCallbackManager.CheckVersionInStore();
    return false;
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown()
  {
  }

  public static MobileCallbackManager Get() => ServiceManager.Get<MobileCallbackManager>();

  public void ClearCaches(LowMemorySeverity severity)
  {
    SpellManager service;
    if (severity != LowMemorySeverity.CRITICAL || !ServiceManager.TryGet<SpellManager>(out service))
      return;
    Debug.LogWarning((object) "Clearing SpellCache");
    service.Clear();
  }

  public void LowMemoryWarning(string msg)
  {
    ulong unixTimeStamp = TimeUtils.DateTimeToUnixTimeStamp(DateTime.Now);
    if (unixTimeStamp < this.m_nextTrimAvailableTime)
    {
      Debug.Log((object) ("Ignored because it didn't pass max time(" + (object) this.m_nextTrimAvailableTime + ")"));
    }
    else
    {
      if (unixTimeStamp - this.m_nextTrimAvailableTime > 10UL)
        this.m_trimDelay = 1UL;
      this.m_nextTrimAvailableTime = unixTimeStamp + this.m_trimDelay;
      this.m_trimDelay *= 2UL;
      LowMemorySeverity outVal;
      if (!EnumUtils.TryGetEnum<LowMemorySeverity>(msg, out outVal))
        outVal = LowMemorySeverity.MODERATE;
      Debug.LogWarningFormat("Receiving LowMemoryWarning severity={0}", (object) outVal);
      this.ClearCaches(outVal);
      HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
      if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
        hearthstoneApplication.UnloadUnusedAssets();
      ++PreviousInstanceStatus.LowMemoryCount;
    }
  }

  public static bool IsAndroidDeviceTabletSized() => Application.isEditor;

  public static bool RequestAppReview(bool forcePopupToShow = false)
  {
    Log.MobileCallback.Print("RequestAppReview()");
    int num = 0;
    if (!forcePopupToShow)
    {
      switch (PlatformSettings.RuntimeOS)
      {
        case OSCategory.iOS:
          if (NetCache.Get() != null)
          {
            NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
            if (!netObject.AppRatingEnabled)
              return false;
            ulong? gameAccountId = BnetUtils.TryGetGameAccountId();
            if ((gameAccountId.HasValue ? new float?((float) gameAccountId.GetValueOrDefault()) : new float?()).HasValue && !BnetUtils.IsPlayerPartOfSamplingPercentage(netObject.AppRatingSamplingPercentage))
              return false;
          }
          num = Options.Get().GetInt(Option.APP_RATING_POPUP_COUNT, 0);
          if (num >= 1)
            return false;
          goto label_12;
        case OSCategory.Android:
          if (AndroidDeviceSettings.Get().GetAndroidStore() == AndroidStore.GOOGLE)
            goto case OSCategory.iOS;
          else
            break;
      }
      Log.MobileCallback.PrintInfo("No applicable storefront for rating app found.");
      return false;
    }
    Log.MobileCallback.Print("Forcing app rating popup to show, bypassing popup limitations.");
label_12:
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_APP_RATING_REQUEST_POPUP_TITLE"),
      m_text = GameStrings.Get("GLUE_APP_RATING_REQUEST_POPUP_TEXT"),
      m_confirmText = GameStrings.Get("GLUE_APP_RATING_REQUEST_CONFIRM"),
      m_cancelText = GameStrings.Get("GLUE_APP_RATING_REQUEST_CANCEL"),
      m_showAlertIcon = true,
      m_iconTexture = new AssetReference("HS.tif:f7eebe7fed3c76b4da1dd53875182b34"),
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if (response == AlertPopup.Response.CONFIRM)
        {
          TelemetryManager.Client().SendButtonPressed("AppEnjoymentAccept");
          MobileCallbackManager.ShowAppRatingPopup();
        }
        else
        {
          TelemetryManager.Client().SendButtonPressed("AppEnjoymentReject");
          MobileCallbackManager.ShowTroubleshootingPopup();
        }
      })
    };
    DialogManager.Get().ShowPopup(info);
    Options.Get().SetInt(Option.APP_RATING_POPUP_COUNT, num + 1);
    return true;
  }

  public static float GetSystemTotalMemoryMB() => (float) MobileCallbackManager.GetSystemTotalMemoryBytes() / 1048576f;

  public static float GetSystemOSSpec() => 0.0f;

  private static void ShowAppRatingPopup()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_APP_RATING_REQUEST_POPUP_TITLE"),
      m_text = GameStrings.Get("GLUE_APP_RATING_POPUP_TEXT"),
      m_confirmText = GameStrings.Get("GLUE_APP_RATING_REQUEST_CONFIRM"),
      m_cancelText = GameStrings.Get("GLUE_APP_RATING_REQUEST_CANCEL"),
      m_showAlertIcon = true,
      m_iconTexture = new AssetReference("HS.tif:f7eebe7fed3c76b4da1dd53875182b34"),
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if (response == AlertPopup.Response.CONFIRM)
        {
          TelemetryManager.Client().SendButtonPressed("AppReviewAccept");
          MobileCallbackManager.ShowNativeAppRatingPopup();
        }
        else
          TelemetryManager.Client().SendButtonPressed("AppReviewReject");
      })
    };
    DialogManager.Get().ShowPopup(info);
  }

  private static void ShowTroubleshootingPopup()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_APP_RATING_REQUEST_POPUP_TITLE"),
      m_text = GameStrings.Get("GLUE_TROUBLESHOOTING_POPUP_TEXT"),
      m_confirmText = GameStrings.Get("GLUE_APP_RATING_REQUEST_CONFIRM"),
      m_cancelText = GameStrings.Get("GLUE_APP_RATING_REQUEST_CANCEL"),
      m_showAlertIcon = true,
      m_iconTexture = new AssetReference("HS.tif:f7eebe7fed3c76b4da1dd53875182b34"),
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if (response == AlertPopup.Response.CONFIRM)
        {
          TelemetryManager.Client().SendButtonPressed("TroubleshootingAccept");
          Application.OpenURL(ExternalUrlService.Get().GetCustomerSupportLink());
        }
        else
          TelemetryManager.Client().SendButtonPressed("TroubleshootingReject");
      })
    };
    DialogManager.Get().ShowPopup(info);
  }

  private void CheckVersionInStore()
  {
  }

  private void CheckVersionInStoreListener(string versionCode)
  {
    Log.Downloader.PrintInfo("Version in Store: " + versionCode);
    string[] strArray = versionCode.Split(',');
    MobileCallbackManager.VersionCodeInStore = strArray[0];
    TelemetryManager.Client().SendVersionCodeInStore(MobileCallbackManager.VersionCodeInStore, strArray.Length > 1 ? strArray[1] : "");
  }

  public static int GetMemoryUsage() => (int) Profiler.GetTotalAllocatedMemoryLong();

  public static void CreateCrashPlugInLayer(string desc)
  {
  }

  public static void CreateCrashInNativeLayer(string desc)
  {
  }

  public static bool AreMotionEffectsEnabled() => true;

  private static bool IsDevice(string deviceModel) => false;

  public static string GetSharedKeychainIdentifier() => string.Empty;

  public static void ShowNativeAppRatingPopup()
  {
  }

  public static string GetAlpha2CountryCode() => "US";

  public static ulong GetSystemTotalMemoryBytes() => (ulong) SystemInfo.systemMemorySize;
}
