using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientLocationManager : IService
{
  private bool m_requestingGPSData;
  private bool m_requestingWifiData;
  private IEnumerator m_requestGPSData;
  private IEnumerator m_requestWifiData;
  private float m_scanTimeout = 15f;
  private bool m_GPSCheatOn;
  private bool m_GPSCheatGPSEnabled;
  private bool m_WifiCheatOn;
  private bool m_WifiCheatWifiEnabled;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    yield break;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (LocationServices),
    typeof (WifiInfo)
  };

  public void Shutdown()
  {
  }

  public static ClientLocationManager Get() => ServiceManager.Get<ClientLocationManager>();

  public void RequestGPSAndWifiData(
    Action<ClientLocationData> updateGPSCallback,
    Action<ClientLocationData> updateWIFICallback,
    Action completeCallback = null)
  {
    Processor.RunCoroutine(this.RequestGPSAndWifiDataCoroutine(updateGPSCallback, updateWIFICallback, completeCallback));
  }

  public void RequestGPSData(Action<ClientLocationData> updateCallback, Action completeCallback = null)
  {
    this.m_requestGPSData = this.RequestGPSDataCoroutine(updateCallback, completeCallback);
    Processor.RunCoroutine(this.m_requestGPSData);
  }

  public void RequestWifiData(Action<ClientLocationData> updateCallback, Action completeCallback = null)
  {
    this.m_requestWifiData = this.RequestWifiDataCoroutine(updateCallback, completeCallback);
    Processor.RunCoroutine(this.m_requestWifiData);
  }

  public ClientLocationData GetBestLocationData()
  {
    GpsCoordinate gpsCoordinate = ServiceManager.Get<LocationServices>().GetBestLocation();
    if (this.m_GPSCheatOn)
    {
      gpsCoordinate = (GpsCoordinate) null;
      if (this.m_GPSCheatGPSEnabled)
      {
        gpsCoordinate = new GpsCoordinate();
        gpsCoordinate.Accuracy = 30.0;
        gpsCoordinate.Timestamp = TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds;
      }
    }
    return new ClientLocationData()
    {
      location = gpsCoordinate,
      accessPointSamples = ServiceManager.Get<WifiInfo>().GetLastKnownAccessPoints()
    };
  }

  public bool GPSServicesReady => this.m_GPSCheatOn || ServiceManager.Get<LocationServices>().IsReady;

  public bool GPSServicesEnabled => this.m_GPSCheatOn ? this.m_GPSCheatGPSEnabled : ServiceManager.Get<LocationServices>().IsEnabled;

  public bool WifiEnabled
  {
    get
    {
      if (this.m_WifiCheatOn)
        return this.m_WifiCheatWifiEnabled;
      return !string.IsNullOrEmpty(this.GetWifiSSID) || MobilePermissionsManager.Get().CheckPermission(MobilePermission.WIFI);
    }
  }

  public string GetWifiSSID
  {
    get
    {
      if (!this.m_WifiCheatOn)
        return ServiceManager.Get<WifiInfo>().GetConnectedSSIDString();
      return !this.m_WifiCheatWifiEnabled ? (string) null : "FAKE NETWORK";
    }
  }

  public bool GPSAvailable => ServiceManager.Get<LocationServices>().IsAvailable;

  public bool GPSOrWifiServicesAvailable => ServiceManager.Get<WifiInfo>().IsAvailable || ServiceManager.Get<LocationServices>().IsAvailable;

  private IEnumerator RequestGPSAndWifiDataCoroutine(
    Action<ClientLocationData> updateGPSCallback,
    Action<ClientLocationData> updateWIFICallback,
    Action completeCallback)
  {
    Log.FiresideGatherings.Print("ClientLocationManager.RequestGPSAndWIFIDataCoroutine");
    if (this.GPSServicesEnabled && this.GPSAvailable)
      Processor.RunCoroutine(this.RequestGPSDataCoroutine(updateGPSCallback, (Action) null));
    if (this.WifiEnabled)
      Processor.RunCoroutine(this.RequestWifiDataCoroutine(updateWIFICallback, (Action) null));
    float timer = 0.0f;
    while ((double) timer < (double) this.m_scanTimeout && (this.m_requestingGPSData || this.m_requestingWifiData))
    {
      timer += Time.deltaTime;
      yield return (object) new WaitForSeconds(0.25f);
    }
    Log.FiresideGatherings.Print("ClientLocationManager.RequestGPSAndWIFIDataCoroutine Finished");
    if (completeCallback != null)
      completeCallback();
  }

  private IEnumerator RequestGPSDataCoroutine(
    Action<ClientLocationData> updateCallback,
    Action completeCallback)
  {
    Log.FiresideGatherings.Print("ClientLocationManager.RequestGPSDataCoroutine");
    if (!this.m_requestingGPSData && this.GPSServicesEnabled && this.GPSAvailable)
      Processor.RunCoroutine(ServiceManager.Get<LocationServices>().UpdateLocation());
    this.m_requestingGPSData = true;
    ClientLocationData bestData = this.GetBestLocationData();
    float timer = 0.0f;
    bool hasUpdated = false;
    while ((double) timer < (double) this.m_scanTimeout)
    {
      ClientLocationData bestLocationData = this.GetBestLocationData();
      if (bestLocationData.location != null && (!hasUpdated || !bestLocationData.location.Equals(bestData.location)))
      {
        hasUpdated = true;
        if (updateCallback != null)
          updateCallback(bestLocationData);
      }
      bestData = bestLocationData;
      if (ServiceManager.Get<LocationServices>().IsQueryingLocation())
      {
        timer += Time.deltaTime;
        yield return (object) new WaitForSeconds(0.25f);
      }
      else
        break;
    }
    this.m_requestingGPSData = false;
    Log.FiresideGatherings.Print("ClientLocationManager.RequestGPSDataCoroutine Finished");
    if (completeCallback != null)
      completeCallback();
  }

  private IEnumerator RequestWifiDataCoroutine(
    Action<ClientLocationData> updateCallback,
    Action completeCallback)
  {
    Log.FiresideGatherings.Print("ClientLocationManager.RequestWIFIDataCoroutine");
    while (!ServiceManager.IsAvailable<WifiInfo>())
      yield return (object) null;
    if (!this.m_requestingWifiData && this.WifiEnabled)
      Processor.RunCoroutine(ServiceManager.Get<WifiInfo>().RequestVisibleAccessPoints());
    this.m_requestingWifiData = true;
    ClientLocationData bestData = this.GetBestLocationData();
    bool hasUpdated = false;
    float timer = 0.0f;
    while ((double) timer < (double) this.m_scanTimeout)
    {
      ClientLocationData bestLocationData = this.GetBestLocationData();
      if (!bestLocationData.accessPointSamples.Equals((object) bestData.accessPointSamples) || !hasUpdated)
      {
        hasUpdated = true;
        if (updateCallback != null)
          updateCallback(bestLocationData);
      }
      bestData = bestLocationData;
      if (ServiceManager.Get<WifiInfo>().IsScanningWifi())
      {
        timer += Time.deltaTime;
        yield return (object) new WaitForSeconds(0.25f);
      }
      else
        break;
    }
    Log.FiresideGatherings.Print("ClientLocationManager.RequestWIFIDataCoroutine Finished");
    this.m_requestingWifiData = false;
    yield return (object) null;
    if (completeCallback != null)
      completeCallback();
  }

  public void Cheat_SetGPSEnabled(bool on)
  {
    this.m_GPSCheatOn = true;
    this.m_GPSCheatGPSEnabled = on;
  }

  public void Cheat_SetWifiEnabled(bool on)
  {
    this.m_WifiCheatOn = true;
    this.m_WifiCheatWifiEnabled = on;
  }
}
