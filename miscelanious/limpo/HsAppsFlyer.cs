using AFMiniJSON;
using AppsFlyerSDK;
using Blizzard.BlizzardErrorMobile;
using Hearthstone;
using System;
using System.Collections.Generic;

public static class HsAppsFlyer
{
  private static bool s_enableDebugLogs;
  private static HsAppsFlyer.IAppsFlyerSDK s_sdk;

  public static void Initialize(int attUserAuthorizationTimeoutSeconds)
  {
    if (HsAppsFlyer.s_sdk != null)
    {
      Log.AdTracking.PrintWarning("AFSDK already initialized");
    }
    else
    {
      HsAppsFlyer.IAppsFlyerSDK appsFlyerSdk = (HsAppsFlyer.IAppsFlyerSDK) new HsAppsFlyer.AppsFlyerSDKImpl();
      Log.AdTracking.PrintInfo("Initializing AFSDK");
      try
      {
        appsFlyerSdk.setIsDebug(HsAppsFlyer.s_enableDebugLogs);
        string empty = string.Empty;
        if (HearthstoneApplication.IsCNMobileBinary)
          appsFlyerSdk.setHost("", "appsflyer-cn.com");
        appsFlyerSdk.initSDK("biU9Lo4fZQJRMhPK4VVZjP", empty);
        if (attUserAuthorizationTimeoutSeconds > 0)
          appsFlyerSdk.waitForATTUserAuthorizationWithTimeoutInterval(attUserAuthorizationTimeoutSeconds);
        appsFlyerSdk.startSDK();
        HsAppsFlyer.s_sdk = appsFlyerSdk;
        Log.AdTracking.PrintInfo("AFSDK initialized");
      }
      catch (Exception ex)
      {
        Log.AdTracking.PrintError("Failed to initialize AFSDK: " + (object) ex);
        ExceptionReporter.Get().ReportCaughtException(ex.Message, ex.StackTrace);
      }
    }
  }

  public static void SetCustomerUserId(string id)
  {
    if (HsAppsFlyer.s_sdk == null)
    {
      Log.AdTracking.PrintWarning("AF not initialized. Can't set id.");
    }
    else
    {
      Log.AdTracking.PrintInfo("Applying AF customer user id");
      try
      {
        if (HsAppsFlyer.s_enableDebugLogs)
          Log.AdTracking.PrintInfo("AF customer user id set to " + id);
        HsAppsFlyer.s_sdk.setCustomerUserId(id);
      }
      catch (Exception ex)
      {
        Log.AdTracking.PrintError("Failed set AF customer user id: " + (object) ex);
        ExceptionReporter.Get().ReportCaughtException(ex.Message, ex.StackTrace);
      }
    }
  }

  public static void SendEvent(string eventName, Dictionary<string, string> eventValues)
  {
    if (HsAppsFlyer.s_sdk == null)
    {
      Log.AdTracking.PrintWarning("AF not initialized. Can't log event " + eventName + ".");
    }
    else
    {
      Log.AdTracking.PrintInfo("Logging AF event: " + eventName);
      try
      {
        if (HsAppsFlyer.s_enableDebugLogs)
          Log.AdTracking.PrintInfo("    eventValues=" + Json.Serialize((object) eventValues));
        HsAppsFlyer.s_sdk.sendEvent(eventName, eventValues);
      }
      catch (Exception ex)
      {
        Log.AdTracking.PrintError("Failed to log AF event: " + (object) ex);
        ExceptionReporter.Get().ReportCaughtException(ex.Message, ex.StackTrace);
      }
    }
  }

  private interface IAppsFlyerSDK
  {
    void initSDK(string devKey, string appID);

    void startSDK();

    void waitForATTUserAuthorizationWithTimeoutInterval(int timeoutInterval);

    void setCustomerUserId(string id);

    void setIsDebug(bool shouldEnable);

    void sendEvent(string eventName, Dictionary<string, string> eventValues);

    void setHost(string hostPrefixName, string hostName);
  }

  private class AppsFlyerSDKImpl : HsAppsFlyer.IAppsFlyerSDK
  {
    public void initSDK(string devKey, string appID) => AppsFlyer.initSDK(devKey, appID);

    public void startSDK() => AppsFlyer.startSDK();

    public void waitForATTUserAuthorizationWithTimeoutInterval(int timeoutInterval)
    {
    }

    public void sendEvent(string eventName, Dictionary<string, string> eventValues) => AppsFlyer.sendEvent(eventName, eventValues);

    public void setCustomerUserId(string id) => AppsFlyer.setCustomerUserId(id);

    public void setIsDebug(bool shouldEnable) => AppsFlyer.setIsDebug(shouldEnable);

    public void setHost(string hostPrefixName, string hostName) => AppsFlyer.setHost(hostPrefixName, hostName);
  }
}
