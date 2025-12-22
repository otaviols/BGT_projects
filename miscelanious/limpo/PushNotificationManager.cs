using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core.Deeplinking;
using Hearthstone.CRM;
using Hearthstone.Mobile.PushNotifications;
using HearthstoneTelemetry;
using MiniJSON;
using PegasusShared;
using System;
using UnityEngine;

public class PushNotificationManager : MonoBehaviour
{
  public const int UNASKED = 1;
  public const int DISALLOWED = 2;
  public const int ALLOWED = 3;
  private static PushNotificationManager s_instance;
  private static IPushNotificationPlugin s_pushSDK;
  private static IPushNotificationPlugin s_swrveSDK;
  private static ITelemetryClient s_telemetryClient;
  private static Action s_dismissCallback;
  private static bool s_isShowingContext;

  [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
  private static void InitializeNativePlugins()
  {
    if (HearthstoneApplication.IsCNMobileBinary)
      PushNotificationManager.SetPushPluginsUnsupported();
    else
      PushNotificationManager.SetPushPluginsUnsupported();
    PushNotificationManager.s_telemetryClient = TelemetryManager.Client();
    PushNotificationManager.s_pushSDK.Initialize(PushNotificationManager.s_telemetryClient);
    PushNotificationManager.s_swrveSDK.Initialize(PushNotificationManager.s_telemetryClient);
    PushNotificationManager.s_instance = new GameObject(nameof (PushNotificationManager), new System.Type[1]
    {
      typeof (HSDontDestroyOnLoad)
    }).AddComponent<PushNotificationManager>();
  }

  private static void SetPushPluginsUnsupported()
  {
    PushNotificationManager.s_pushSDK = (IPushNotificationPlugin) new PushNotificationPluginUnsupported();
    PushNotificationManager.s_swrveSDK = (IPushNotificationPlugin) new PushNotificationPluginUnsupported();
    Debug.Log((object) "PushNotificationManager - Setting Push Plugins to uninitialized state");
  }

  public static PushNotificationManager Get() => PushNotificationManager.s_instance;

  public void SetPushNotificationFeatureStatus(bool isEnabled)
  {
    Log.MobileCallback.Print(string.Format("PushNotificationManager - Setting push notification feature status to: {0}", (object) isEnabled));
    if (!isEnabled)
      this.UnregisterPushNotifications();
    else
      PushNotificationManager.s_instance.GetDevicePushNotificationStatus();
    Options.Get().SetInt(Option.PUSH_NOTIFICATION_STATUS, isEnabled ? 3 : 2);
  }

  public bool ShouldDisallowPushNotifications()
  {
    bool flag = Options.Get().GetInt(Option.PUSH_NOTIFICATION_STATUS) == 2;
    Log.MobileCallback.Print(string.Format("PushNotificationManager - Are push notifications disallowed? {0}", (object) flag));
    return flag;
  }

  public void DisallowPushNotifications()
  {
    Log.MobileCallback.Print("PushNotificationManager - Setting push notifications to disallowed.");
    Options.Get().SetInt(Option.PUSH_NOTIFICATION_STATUS, 2);
  }

  public bool CanRegisterPushAtLogin()
  {
    if (PlatformSettings.RuntimeOS != OSCategory.iOS)
      Options.Get().SetInt(Option.PUSH_NOTIFICATION_STATUS, 3);
    bool flag = Options.Get().GetInt(Option.PUSH_NOTIFICATION_STATUS) == 3;
    Log.MobileCallback.Print(string.Format("PushNotificationManager - Can register pushes at login? {0}", (object) flag));
    return flag;
  }

  public bool ShowPushNotificationContext(Action dismissCallback)
  {
    Log.MobileCallback.Print(string.Format("PushNotificationManager - Showing push notifications context. DismissCallback: {0}", (object) dismissCallback));
    if (PlatformSettings.RuntimeOS == OSCategory.PC || PlatformSettings.RuntimeOS == OSCategory.Mac || SpectatorManager.Get().IsSpectatingOrWatching || Options.Get().GetInt(Option.PUSH_NOTIFICATION_STATUS, 1) != 1 || this.GetGamesWon() < 3)
      return false;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_PUSH_NOTIFICATION_CONTEXT_HEADER"),
      m_text = GameStrings.Get("GLUE_PUSH_NOTIFICATION_CONTEXT_BODY"),
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_confirmText = GameStrings.Get("GLUE_PUSH_NOTIFICATION_CONTEXT_CONFIRM"),
      m_cancelText = GameStrings.Get("GLUE_PUSH_NOTIFICATION_CONTEXT_CANCEL"),
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnPushNotificationContextResponse)
    };
    PushNotificationManager.s_dismissCallback = dismissCallback;
    DialogManager.Get().ShowPopup(info);
    PushNotificationManager.s_isShowingContext = true;
    return true;
  }

  public void ShowPushNotificationsDisabledContext()
  {
    Log.MobileCallback.Print("PushNotificationManager - Showing push notifications disabled context.");
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_PUSH_NOTIFICATION_CONTEXT_HEADER"),
      m_text = GameStrings.Get("GLUE_PUSH_NOTIFICATION_EXTENDED_CONTEXT_BODY"),
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response2, userData2) =>
      {
        if (PushNotificationManager.s_dismissCallback != null)
        {
          PushNotificationManager.s_dismissCallback();
          PushNotificationManager.s_dismissCallback = (Action) null;
        }
        PushNotificationManager.s_isShowingContext = false;
      })
    };
    DialogManager.Get().ShowPopup(info);
  }

  public bool IsShowingContext() => PushNotificationManager.s_isShowingContext;

  public void SetTelemetryInfo(string appId, string appName, string appVersion, string sessionId)
  {
    Log.MobileCallback.Print("PushNotificationManager - SetTelemetryInfo(" + appId + ", " + appName + ", " + appVersion + ", " + sessionId + ")");
    PushNotificationManager.s_pushSDK.SetTelemetryInfo(appId, appName, appVersion, sessionId);
    PushNotificationManager.s_swrveSDK.SetTelemetryInfo(appId, appName, appVersion, sessionId);
  }

  public void SetPushRegistrationInfo(
    string tassadarAuthToken,
    ulong bnetAccountId,
    string bnetAccountRegion,
    string bnetAccountLocale)
  {
    Log.MobileCallback.Print(string.Format("PushNotificationManager - SetPushRegistrationInfo({0}, {1}, {2}, {3})", (object) tassadarAuthToken, (object) bnetAccountId, (object) bnetAccountRegion, (object) bnetAccountLocale));
    PushNotificationManager.s_pushSDK.SetPushRegistrationInfo(tassadarAuthToken, bnetAccountId, bnetAccountRegion, bnetAccountLocale);
    PushNotificationManager.s_swrveSDK.SetPushRegistrationInfo(tassadarAuthToken, bnetAccountId, bnetAccountRegion, bnetAccountLocale);
  }

  public void RegisterPushNotifications()
  {
    Log.MobileCallback.Print("PushNotificationManager - Registering push notifications.");
    PushNotificationManager.s_pushSDK.RegisterPushNotifications();
    PushNotificationManager.s_swrveSDK.RegisterPushNotifications();
  }

  public void UnregisterPushNotifications()
  {
    Log.MobileCallback.Print("PushNotificationManager - Unregistering push notifications.");
    PushNotificationManager.s_pushSDK.UnregisterPushNotifications();
    PushNotificationManager.s_swrveSDK.UnregisterPushNotifications();
  }

  public string[] ConsumeDeepLink(bool retain)
  {
    string url = PushNotificationManager.s_pushSDK.ConsumeDeepLink(retain);
    Log.DeepLink.Print("Deep Link recieved " + url);
    if (string.IsNullOrEmpty(url))
    {
      url = PushNotificationManager.s_pushSDK.GetStartupDeepLink();
    }
    else
    {
      DeeplinkService service;
      if (ServiceManager.TryGet<DeeplinkService>(out service) && service.ProcessDeeplink(url))
      {
        if (retain)
          PushNotificationManager.s_pushSDK.ConsumeDeepLink(false);
        return (string[]) null;
      }
    }
    string[] strArray = (string[]) null;
    if (url != null && url.StartsWith("hearthstone://"))
      strArray = url.Substring("hearthstone://".Length).Split('/');
    string str = strArray == null ? "null" : string.Join(" ", strArray);
    Log.MobileCallback.Print(string.Format("PushNotificationManager - Consuming deep link. Retain link afterwards? {0} Returned arguments: ${1}", (object) retain, (object) str));
    return strArray;
  }

  private void GetDevicePushNotificationStatus() => Log.MobileCallback.Print("PushNotificationManager - Getting push notification status.");

  private int GetGamesWon()
  {
    int gamesWon = 0;
    if (NetCache.Get() == null || NetCache.Get().GetNetObject<NetCache.NetCachePlayerRecords>() == null || NetCache.Get().GetNetObject<NetCache.NetCachePlayerRecords>().Records == null)
      return gamesWon;
    foreach (NetCache.PlayerRecord record in NetCache.Get().GetNetObject<NetCache.NetCachePlayerRecords>().Records)
    {
      if (record.Data == 0)
      {
        switch (record.RecordType)
        {
          case GameType.GT_VS_AI:
          case GameType.GT_ARENA:
          case GameType.GT_RANKED:
          case GameType.GT_CASUAL:
          case GameType.GT_TAVERNBRAWL:
          case GameType.GT_FSG_BRAWL:
          case GameType.GT_FSG_BRAWL_2P_COOP:
            gamesWon += record.Wins;
            continue;
          default:
            continue;
        }
      }
    }
    return gamesWon;
  }

  private void OnPushNotificationContextResponse(AlertPopup.Response response, object userData)
  {
    Log.MobileCallback.Print(string.Format("PushNotificationManager, OnPushNotificationContextResponse({0}, {1})", (object) response, userData));
    if (response == AlertPopup.Response.CANCEL)
    {
      Log.MobileCallback.Print("PushNotificationManager - In-app prompt: Push Notification permission denied.");
      this.DisallowPushNotifications();
    }
    else
    {
      Log.MobileCallback.Print("PushNotificationManager - In-app prompt: Push Notification permission authorized.");
      Options.Get().SetInt(Option.PUSH_NOTIFICATION_STATUS, 3);
      PushNotificationManager.s_instance.GetDevicePushNotificationStatus();
    }
    if (PushNotificationManager.s_dismissCallback != null)
    {
      PushNotificationManager.s_dismissCallback();
      PushNotificationManager.s_dismissCallback = (Action) null;
    }
    PushNotificationManager.s_isShowingContext = false;
  }

  private void OnPushRegistrationSucceeded(string token)
  {
    Log.MobileCallback.Print("OnPushRegistrationSucceeded(" + token + ")");
    BlizzardCRMManager.Get().SendEvent_PushRegistration(token);
  }

  private void OnPushNotificationReceived(string campaign)
  {
    Log.MobileCallback.Print("OnPushNotificationReceived(" + campaign + ")");
    BlizzardCRMManager.Get().SendEvent_PushEvent(campaign, (JsonNode) null);
  }

  private void OnDidRegisterForRemoteNotificationsWithDeviceToken(string deviceToken)
  {
    Log.MobileCallback.Print("OnDidRegisterForRemoteNotificationsWithDeviceToken(" + deviceToken + ")");
    PushNotificationManager.s_telemetryClient.SendPushRegistrationSucceeded(deviceToken);
  }

  private void OnDidFailToRegisterForRemoteNotificationsWithError(string error)
  {
    Log.MobileCallback.Print("OnDidFailToRegisterForRemoteNotificationsWithError(" + error + ")");
    PushNotificationManager.s_telemetryClient.SendPushRegistrationFailed(error);
  }

  private void OnHandleDevicePushNotificationStatus(string status)
  {
    Log.MobileCallback.Print("PushNotificationManager - OnHandleDevicePushNotificationStatus(" + status + ")");
    if (status == 2.ToString())
    {
      Log.MobileCallback.Print("PushNotificationManager - Native OS prompt: Push Notification permission denied.");
    }
    else
    {
      Log.MobileCallback.Print("PushNotificationManager - Native OS prompt: Push Notification permission authorized.");
      this.RegisterPushNotifications();
    }
  }
}
