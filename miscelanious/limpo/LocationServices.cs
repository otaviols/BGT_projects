using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationServices : IService
{
  private float m_initializationTimeout = 20f;
  private float m_stoppingAccuracy = 80f;
  private GpsCoordinate m_lastKnownLocation;
  private GpsCoordinate m_bestLocation;
  private bool m_isQueryingLocation;

  public static void StartGeoSearch() => WindowsLocationAPI.StartGeoSearch();

  public static double GetLatitude() => WindowsLocationAPI.GetLatitude();

  public static double GetLongitude() => WindowsLocationAPI.GetLongitude();

  public static double GetHorizontalAccuracy() => WindowsLocationAPI.GetHorizontalAccuracy();

  public static bool GetEnabled() => WindowsLocationAPI.GetEnabled();

  public static bool GetReady() => WindowsLocationAPI.GetReady();

  public static void StartSearching()
  {
  }

  public static void StopSearching()
  {
  }

  public static int HasPermission() => 0;

  public static string GetLocationData() => (string) null;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    yield break;
  }

  public System.Type[] GetDependencies() => (System.Type[]) null;

  public void Shutdown()
  {
  }

  public bool IsAvailable => Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXPlayer && Application.platform != RuntimePlatform.OSXEditor;

  public bool IsReady
  {
    get
    {
      try
      {
        return LocationServices.GetReady();
      }
      catch (DllNotFoundException ex)
      {
        Log.FiresideGatherings.Print("Location API DLL not available");
        return true;
      }
      catch (Exception ex)
      {
        Log.FiresideGatherings.PrintWarning("Couldn't check for device location services readiness.\n" + ex.Message);
        return true;
      }
    }
  }

  public bool IsEnabled
  {
    get
    {
      try
      {
        switch (Application.platform)
        {
          case RuntimePlatform.OSXEditor:
          case RuntimePlatform.OSXPlayer:
            return LocationServices.HasPermission() == 1;
          case RuntimePlatform.WindowsPlayer:
          case RuntimePlatform.WindowsEditor:
            return LocationServices.GetEnabled();
          default:
            return MobilePermissionsManager.Get().CheckPermission(MobilePermission.FINE_LOCATION);
        }
      }
      catch (DllNotFoundException ex)
      {
        Log.FiresideGatherings.Print("Location API DLL not available");
        return false;
      }
      catch (Exception ex)
      {
        Log.FiresideGatherings.PrintWarning("Couldn't check for device location services availability.\n" + ex.Message);
        return false;
      }
    }
  }

  public GpsCoordinate GetBestLocation() => this.m_bestLocation;

  public bool IsQueryingLocation() => this.m_isQueryingLocation;

  public IEnumerator UpdateLocation(int maxTime = 15)
  {
    this.m_lastKnownLocation = new GpsCoordinate();
    this.m_bestLocation = new GpsCoordinate();
    this.m_isQueryingLocation = true;
    switch (Application.platform)
    {
      case RuntimePlatform.OSXEditor:
      case RuntimePlatform.OSXPlayer:
        yield return (object) Processor.RunCoroutine(this.UpdateLocationOSX(maxTime));
        break;
      case RuntimePlatform.WindowsPlayer:
      case RuntimePlatform.WindowsEditor:
        yield return (object) Processor.RunCoroutine(this.UpdateLocationWindows(maxTime));
        break;
      default:
        yield return (object) Processor.RunCoroutine(this.UpdateLocationMobile(maxTime));
        break;
    }
  }

  public IEnumerator UpdateLocationWindows(int maxTime)
  {
    for (int timeSpent = 0; timeSpent < maxTime; ++timeSpent)
    {
      LocationServices.StartGeoSearch();
      GpsCoordinate gpsCoordinate = new GpsCoordinate();
      gpsCoordinate.Latitude = LocationServices.GetLatitude();
      gpsCoordinate.Longitude = LocationServices.GetLongitude();
      gpsCoordinate.Accuracy = LocationServices.GetHorizontalAccuracy();
      gpsCoordinate.Timestamp = TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds;
      if (gpsCoordinate.Accuracy < this.m_bestLocation.Accuracy)
        this.m_bestLocation = gpsCoordinate;
      this.m_lastKnownLocation = gpsCoordinate;
      if (this.m_bestLocation.Accuracy > (double) this.m_stoppingAccuracy)
        yield return (object) new WaitForSeconds(1f);
      else
        break;
    }
    this.m_isQueryingLocation = false;
  }

  public IEnumerator UpdateLocationOSX(int maxTime)
  {
    int timeSpent = 0;
    LocationServices.StartSearching();
    while (timeSpent < maxTime)
    {
      GpsCoordinate gpsCoordinate = new GpsCoordinate();
      string locationData = LocationServices.GetLocationData();
      if (string.IsNullOrEmpty(locationData))
      {
        yield return (object) new WaitForSeconds(1f);
      }
      else
      {
        string[] strArray = locationData.Split(';');
        if (strArray.Length != 3)
        {
          Log.FiresideGatherings.PrintWarning("Invalid OSX location data string: \"{0}\"", (object) locationData);
          yield return (object) new WaitForSeconds(1f);
        }
        else
        {
          double result1 = 0.0;
          double result2 = 0.0;
          double result3 = double.MaxValue;
          if (!double.TryParse(strArray[0], out result1) || !double.TryParse(strArray[1], out result2) || !double.TryParse(strArray[2], out result3))
          {
            Log.FiresideGatherings.PrintWarning("Invalid OSX location data string: \"{0}\"", (object) locationData);
            yield return (object) new WaitForSeconds(1f);
          }
          else
          {
            gpsCoordinate.Latitude = result1;
            gpsCoordinate.Longitude = result2;
            gpsCoordinate.Accuracy = result3;
            gpsCoordinate.Timestamp = TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds;
            if (gpsCoordinate.Accuracy < this.m_bestLocation.Accuracy)
              this.m_bestLocation = gpsCoordinate;
            this.m_lastKnownLocation = gpsCoordinate;
            if (this.m_bestLocation.Accuracy > (double) this.m_stoppingAccuracy)
            {
              yield return (object) new WaitForSeconds(1f);
              ++timeSpent;
            }
            else
              break;
          }
        }
      }
    }
    LocationServices.StopSearching();
    this.m_isQueryingLocation = false;
  }

  public IEnumerator UpdateLocationMobile(int maxTime)
  {
    double locationStartTime = TimeUtils.GetElapsedTimeSinceEpoch().TotalSeconds;
    if (!ClientLocationManager.Get().GPSServicesEnabled)
    {
      Log.FiresideGatherings.PrintWarning("Location services not available to user!");
      this.m_isQueryingLocation = false;
    }
    else
    {
      Input.location.Stop();
      int timeSpent = 0;
      while (timeSpent < maxTime)
      {
        if (Input.location.status != LocationServiceStatus.Running)
        {
          Input.location.Start(0.1f, 0.1f);
          int timeoutLeft = (int) this.m_initializationTimeout;
          while (Input.location.status == LocationServiceStatus.Initializing && timeoutLeft > 0)
          {
            yield return (object) new WaitForSeconds(1f);
            --timeoutLeft;
            ++timeSpent;
          }
          if (timeoutLeft < 1)
          {
            Log.FiresideGatherings.PrintError("LocationServices Timed out");
            this.m_isQueryingLocation = false;
            yield break;
          }
          else if (Input.location.status == LocationServiceStatus.Failed)
          {
            Log.FiresideGatherings.PrintError("Unable to determine device location");
            this.m_isQueryingLocation = false;
            yield break;
          }
        }
        GpsCoordinate lastData = (GpsCoordinate) Input.location.lastData;
        Input.location.Stop();
        if (lastData.Timestamp < locationStartTime)
        {
          yield return (object) new WaitForSeconds(1f);
        }
        else
        {
          if (lastData.Accuracy < this.m_bestLocation.Accuracy)
            this.m_bestLocation = lastData;
          this.m_lastKnownLocation = lastData;
          if (this.m_bestLocation.Accuracy > (double) this.m_stoppingAccuracy)
          {
            Log.FiresideGatherings.Print("Best location updated with accuracy: " + (object) this.m_bestLocation.Accuracy);
            yield return (object) new WaitForSeconds(1f);
            ++timeSpent;
          }
          else
            break;
        }
      }
      this.m_isQueryingLocation = false;
    }
  }
}
