using Blizzard.T5.Core.Utils;
using Hearthstone;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using tracert;
using UnityEngine;

public class TracertReporter
{
  private static NetCache.NetCacheFeatures.CacheTraceroute s_settings;

  private static bool IsElevateNeeded => false;

  private static bool IsReady { get; set; } = false;

  private static string InitStatus { get; set; } = "Undefined";

  private static bool TracerouteEnabled
  {
    get
    {
      NetCache.NetCacheFeatures netObject = NetCache.Get()?.GetNetObject<NetCache.NetCacheFeatures>();
      if (netObject == null)
        return TracertReporter.IsReady;
      TracertReporter.s_settings = netObject.Traceroute;
      return netObject.TracerouteEnabled && TracertReporter.IsReady;
    }
  }

  private static int MaxHops => TracertReporter.s_settings != null ? TracertReporter.s_settings.MaxHops : 30;

  private static int MessageSize => TracertReporter.s_settings != null ? TracertReporter.s_settings.MessageSize : 32;

  private static int MaxRetries => TracertReporter.s_settings != null ? TracertReporter.s_settings.MaxRetries : 3;

  private static int TimeoutMs => TracertReporter.s_settings != null ? TracertReporter.s_settings.TimeoutMs : 3000;

  private static bool ResolveHost => TracertReporter.s_settings != null && TracertReporter.s_settings.ResolveHost;

  private static void ReportTracertInfoInternal(string host, string hopStr)
  {
    if (string.IsNullOrEmpty(hopStr))
      return;
    List<string> hops = new List<string>();
    string[] strArray = hopStr.Split(new string[1]{ "%%%" }, StringSplitOptions.None);
    Debug.LogFormat("Tracert: " + hopStr);
    if (strArray.Length > 1)
      Debug.LogFormat(strArray[1]);
    hops.AddRange((IEnumerable<string>) strArray[0].TrimEnd().Split(new string[3]
    {
      "\r\n",
      "\r",
      "\n"
    }, StringSplitOptions.None));
    int nFailedHops = 0;
    hops.ForEach((Action<string>) (h =>
    {
      if (!h.EndsWith("*\t"))
        return;
      ++nFailedHops;
    }));
    TelemetryManager.Client().SendTraceroute(host, hops, hops.Count, nFailedHops, hops.Count - nFailedHops);
  }

  private static async Task<string> RunTracertAsync(string host)
  {
    string resultLines = string.Empty;
    await Task.Run((Action) (() =>
    {
      try
      {
        resultLines = TracertAPI.GetTraceRouteStrWrapper(host, TracertReporter.MaxHops, TracertReporter.MessageSize, TracertReporter.MaxRetries, TracertReporter.TimeoutMs, TracertReporter.ResolveHost, true);
      }
      catch (Exception ex)
      {
        Log.All.PrintWarning("Failed to get tracert information with " + host + ": " + ex.Message);
      }
    }));
    return resultLines;
  }

  public static void SendTelemetry() => TelemetryManager.Client().SendInitTraceroute(TracertReporter.IsReady, TracertReporter.InitStatus);

  public static void ReportTracertInfo(string host)
  {
    if (!TracertReporter.TracerouteEnabled || string.IsNullOrEmpty(host))
      return;
    Task.Run((Func<Task>) (async () =>
    {
      string hopStr = await TracertReporter.RunTracertAsync(host);
      TracertReporter.ReportTracertInfoInternal(host, hopStr);
    }));
  }

  public static void Initialize()
  {
    try
    {
      if (TracertAPI.IsAvailableICMP())
      {
        Debug.Log((object) "Tracert: ICMP protocol is ready.");
        TracertReporter.IsReady = true;
        TracertReporter.InitStatus = "OK";
      }
      else if (TracertAPI.PrepareICMPRule(TracertReporter.IsElevateNeeded, string.Join(" ", HearthstoneApplication.CommandLineArgs)))
      {
        if (TracertReporter.IsElevateNeeded)
        {
          Debug.Log((object) "Tracert: It's elevated. Closing the current exe.");
          GeneralUtils.ExitApplication();
        }
        TracertReporter.IsReady = true;
        TracertReporter.InitStatus = "OK";
      }
      else if (TracertReporter.IsElevateNeeded)
      {
        if (TracertAPI.IsRunAsAdministrator())
        {
          TracertReporter.IsReady = true;
          TracertReporter.InitStatus = "OK";
        }
        else
        {
          Debug.Log((object) "Tracert: Need to admin permission!");
          TracertReporter.InitStatus = "NotAdmin";
        }
      }
      else
      {
        Debug.Log((object) "Tracert: failed to add ICMP rule.");
        TracertReporter.InitStatus = "ICMPRuleAddFailure";
      }
    }
    catch (Exception ex)
    {
      Debug.Log((object) ("Failed to initialize tracert: " + ex.Message));
      TracertReporter.InitStatus = "Exception-" + ex.Message;
    }
  }
}
