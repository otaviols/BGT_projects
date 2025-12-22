using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class WifiInfo : IService
{
  private List<AccessPointInfo> m_lastKnownAccessPoints = new List<AccessPointInfo>();
  private bool m_waitingForResponse;
  private string m_connectedSSID;

  public bool IsAvailable { get; private set; }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    this.IsAvailable = false;
    yield return (IAsyncJobResult) new ServiceSoftDependency(typeof (LoginManager), serviceLocator);
    this.IsAvailable = this.DoWifiScan();
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (MobileCallbackManager)
  };

  public void Shutdown()
  {
  }

  public bool DoWifiScan()
  {
    Process process1 = new Process()
    {
      StartInfo = {
        FileName = "netsh.exe",
        Arguments = "wlan show networks mode=bssid",
        UseShellExecute = false,
        RedirectStandardOutput = true,
        CreateNoWindow = true
      }
    };
    try
    {
      process1.Start();
    }
    catch (Exception ex)
    {
      Log.FiresideGatherings.Print("Failed to execute netsh: " + ex.Message);
      return false;
    }
    string[] strArray1 = process1.StandardOutput.ReadToEnd().Split(new string[1]
    {
      Environment.NewLine
    }, StringSplitOptions.RemoveEmptyEntries);
    if (strArray1 == null || strArray1.Length < 2)
      return false;
    List<AccessPointInfo> points = new List<AccessPointInfo>();
    string str1 = (string) null;
    AccessPointInfo accessPointInfo = (AccessPointInfo) null;
    foreach (string str2 in strArray1)
    {
      if (str2.TrimStart().StartsWith("SSID"))
        str1 = str2.Substring(str2.IndexOf(':', 0) + 1).Trim();
      else if (str2.TrimStart().StartsWith("BSSID"))
      {
        if (str1 == null)
          Log.FiresideGatherings.Print("Warning currentSSID is Null");
        if (accessPointInfo != null)
        {
          Log.FiresideGatherings.Print("Failed to find BSSID");
          return true;
        }
        accessPointInfo = new AccessPointInfo();
        accessPointInfo.ssid = str1;
        accessPointInfo.bssid = str2.Substring(str2.IndexOf(':', 0) + 1).Trim();
      }
      else if (str2.TrimStart().StartsWith("Signal"))
      {
        if (accessPointInfo == null)
        {
          Log.FiresideGatherings.Print("Failed to find Signal");
          return true;
        }
        string str3 = str2.Substring(str2.IndexOf(':', 0) + 1).Trim();
        accessPointInfo.signalStrength = (float) Convert.ToInt32(str3.Substring(0, str3.Length - 1));
        points.Add(accessPointInfo);
        accessPointInfo = (AccessPointInfo) null;
      }
    }
    this.ReceiveVisibleAccessPointList(points);
    Process process2 = new Process();
    process2.StartInfo.FileName = "netsh.exe";
    process2.StartInfo.Arguments = "wlan show interfaces";
    process2.StartInfo.UseShellExecute = false;
    process2.StartInfo.RedirectStandardOutput = true;
    process2.StartInfo.CreateNoWindow = true;
    process2.Start();
    string[] strArray2 = process2.StandardOutput.ReadToEnd().Split(new string[1]
    {
      Environment.NewLine
    }, StringSplitOptions.RemoveEmptyEntries);
    if (strArray2 == null)
      return true;
    this.m_connectedSSID = (string) null;
    foreach (string str4 in strArray2)
    {
      if (str4.TrimStart().StartsWith("State"))
      {
        if (!str4.Substring(str4.IndexOf(':', 0) + 1).Trim().Equals("connected"))
          break;
      }
      else if (str4.TrimStart().StartsWith("SSID"))
      {
        this.m_connectedSSID = str4.Substring(str4.IndexOf(':', 0) + 1).Trim();
        break;
      }
    }
    return true;
  }

  public bool IsScanningWifi() => this.m_waitingForResponse;

  public string GetConnectedSSIDString()
  {
    this.IsAvailable = this.DoWifiScan();
    return this.IsAvailable ? this.m_connectedSSID : (string) null;
  }

  public IEnumerator RequestVisibleAccessPoints()
  {
    if (!this.m_waitingForResponse)
    {
      this.m_waitingForResponse = false;
      this.IsAvailable = this.DoWifiScan();
      while (this.m_waitingForResponse)
        yield return (object) null;
    }
  }

  public void ReceiveVisibleAccessPointList(List<AccessPointInfo> points)
  {
    this.m_waitingForResponse = false;
    this.m_lastKnownAccessPoints = points;
    if (points == null || points.Count < 1)
      return;
    points.Sort();
  }

  public List<AccessPointInfo> GetLastKnownAccessPoints() => this.m_lastKnownAccessPoints;
}
