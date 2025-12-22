using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceAnalytics : IService
{
  private float m_initStartTime;
  private bool m_isReconnecting;
  private string m_reconnectType = "INVALID";
  private float m_reconnectStartTime;
  private float m_disconnectTime;
  private string m_location = string.Empty;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    this.BeginStartupTimer();
    if (BattleNet.IsInitialized() && BattleNet.IsConnected())
      this.m_location = BattleNet.GetAccountCountry();
    this.SendDisconnectAndTimeoutEvents();
    yield break;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (SceneMgr)
  };

  public void Shutdown()
  {
  }

  public static PerformanceAnalytics Get() => ServiceManager.Get<PerformanceAnalytics>();

  public void BeginStartupTimer() => this.m_initStartTime = Time.realtimeSinceStartup;

  public void ReconnectStart(string reconnectType)
  {
    if (this.m_isReconnecting)
      return;
    this.m_isReconnecting = true;
    this.m_reconnectType = reconnectType;
    this.m_reconnectStartTime = Time.realtimeSinceStartup;
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.ReconnectSceneLoaded));
    this.SendDisconnectAndTimeoutEvents();
  }

  public void ReconnectSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (!this.m_isReconnecting || mode != SceneMgr.Mode.GAMEPLAY)
      return;
    this.ReconnectEnd(true);
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.ReconnectSceneLoaded));
  }

  public void DisconnectEvent(string mode)
  {
    this.m_disconnectTime = Time.realtimeSinceStartup;
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.DisconnectTimeReset));
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetInt(nameof (DisconnectEvent), 1);
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetString("DisconnectEvent_Mode", mode);
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetString("DisconnectEvent_Location", this.GetCountry());
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetString("DisconnectEvent_Connection", this.GetConnectionType());
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetString("DisconnectEvent_OS", PlatformSettings.OS.ToString());
  }

  public void SendDisconnectAndTimeoutEvents()
  {
    if (Application.internetReachability == NetworkReachability.NotReachable)
      return;
    if (Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.GetInt("DisconnectEvent") == 1)
    {
      Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetInt("DisconnectEvent", 0);
      Log.Performance.Print("Sent Disconnect Event");
    }
    if (Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.GetInt("ReconnectTimeOut") != 1)
      return;
    Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetInt("ReconnectTimeOut", 0);
    TelemetryManager.Client().SendReconnectTimeout(Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.GetString("ReconnectTimeOut_Type"));
    Log.Performance.Print("Sent Reconnect Timout Event");
  }

  public void DisconnectTimeReset(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (this.m_isReconnecting || mode != SceneMgr.Mode.GAMEPLAY && mode != SceneMgr.Mode.HUB)
      return;
    this.m_disconnectTime = 0.0f;
  }

  public void ReconnectEnd(bool success)
  {
    if (!this.m_isReconnecting)
      return;
    this.SendDisconnectAndTimeoutEvents();
    this.m_isReconnecting = false;
    float reconnectDuration = Time.realtimeSinceStartup - this.m_reconnectStartTime;
    float disconnectDuration = Time.realtimeSinceStartup - this.m_disconnectTime;
    if (success)
    {
      TelemetryManager.Client().SendReconnectSuccess(disconnectDuration, reconnectDuration, this.m_reconnectType);
      this.m_disconnectTime = 0.0f;
      Log.Performance.Print("Sent Reconnect Success Event");
    }
    else
    {
      Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetInt("ReconnectTimeOut", 1);
      Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetString("ReconnectTimeOut_Type", this.m_reconnectType);
      Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetString("ReconnectTimeOut_Location", this.GetCountry());
      Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetString("ReconnectTimeOut_Connection", this.GetConnectionType());
      Blizzard.T5.Configuration.PreferencesManager.PreferencesManager.SetString("ReconnectTimeOut_OS", PlatformSettings.OS.ToString());
      this.m_disconnectTime = 0.0f;
      Log.Performance.Print("Recorded Reconnect Timout");
    }
  }

  private string GetCountry()
  {
    if (string.IsNullOrEmpty(this.m_location) && BattleNet.IsConnected())
      this.m_location = BattleNet.GetAccountCountry();
    if (string.IsNullOrEmpty(this.m_location))
      this.m_location = "Unknown";
    return this.m_location;
  }

  private string GetConnectionType()
  {
    if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
      return "Cellular";
    return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork ? "LAN" : "None";
  }
}
