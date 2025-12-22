using Microsoft.Win32;
using System.Runtime.InteropServices;

public static class WindowsLocationAPI
{
  private static bool m_canUseLocationDll;
  private static bool m_isInitialized;

  public static void StartGeoSearch()
  {
    WindowsLocationAPI.CheckInit();
    if (!WindowsLocationAPI.m_canUseLocationDll)
      return;
    WindowsLocationAPI.ApiStartGeoSearch();
  }

  public static double GetLatitude()
  {
    WindowsLocationAPI.CheckInit();
    return WindowsLocationAPI.m_canUseLocationDll ? WindowsLocationAPI.ApiGetLatitude() : 0.0;
  }

  public static double GetLongitude()
  {
    WindowsLocationAPI.CheckInit();
    return WindowsLocationAPI.m_canUseLocationDll ? WindowsLocationAPI.ApiGetLongitude() : 0.0;
  }

  public static double GetHorizontalAccuracy()
  {
    WindowsLocationAPI.CheckInit();
    return WindowsLocationAPI.m_canUseLocationDll ? WindowsLocationAPI.ApiGetHorizontalAccuracy() : 0.0;
  }

  public static bool GetEnabled()
  {
    WindowsLocationAPI.CheckInit();
    return WindowsLocationAPI.m_canUseLocationDll && WindowsLocationAPI.ApiGetEnabled();
  }

  public static bool GetReady()
  {
    WindowsLocationAPI.CheckInit();
    return WindowsLocationAPI.m_canUseLocationDll && WindowsLocationAPI.ApiGetReady();
  }

  [DllImport("LocationAPI", EntryPoint = "StartGeoSearch")]
  private static extern void ApiStartGeoSearch();

  [DllImport("LocationAPI", EntryPoint = "GetLatitude")]
  private static extern double ApiGetLatitude();

  [DllImport("LocationAPI", EntryPoint = "GetLongitude")]
  private static extern double ApiGetLongitude();

  [DllImport("LocationAPI", EntryPoint = "GetHorizontalAccuracy")]
  private static extern double ApiGetHorizontalAccuracy();

  [DllImport("LocationAPI", EntryPoint = "GetEnabled")]
  private static extern bool ApiGetEnabled();

  [DllImport("LocationAPI", EntryPoint = "GetReady")]
  private static extern bool ApiGetReady();

  private static void Init()
  {
    WindowsLocationAPI.m_canUseLocationDll = WindowsLocationAPI.IsNetFramework4OrHigher();
    WindowsLocationAPI.m_isInitialized = true;
  }

  private static void CheckInit()
  {
    if (WindowsLocationAPI.m_isInitialized)
      return;
    WindowsLocationAPI.Init();
  }

  private static bool IsNetFramework4OrHigher()
  {
    RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full");
    if (registryKey1 != null)
      return (int) (registryKey1.GetValue("Release") ?? (object) 0) >= 378389;
    RegistryKey registryKey2 = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, "").OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\");
    if (registryKey2 != null)
    {
      foreach (string subKeyName in registryKey2.GetSubKeyNames())
      {
        if (WindowsLocationAPI.GetVersionNumberFromKey(registryKey2.OpenSubKey(subKeyName)) >= 4)
          return true;
      }
    }
    return false;
  }

  private static int GetVersionNumberFromKey(RegistryKey versionKey)
  {
    if (versionKey == null)
      return -1;
    int versionNumberFromKey = WindowsLocationAPI.ParseVersionNumber(versionKey);
    foreach (string subKeyName in versionKey.GetSubKeyNames())
    {
      RegistryKey versionKey1 = versionKey.OpenSubKey(subKeyName);
      if (versionKey1 != null)
      {
        int versionNumber = WindowsLocationAPI.ParseVersionNumber(versionKey1);
        if (versionNumber > versionNumberFromKey)
          versionNumberFromKey = versionNumber;
      }
    }
    return versionNumberFromKey;
  }

  private static int ParseVersionNumber(RegistryKey versionKey)
  {
    int result = -1;
    string str = (string) versionKey.GetValue("Version", (object) "");
    if (!(str != ""))
      return result;
    int.TryParse(str.Substring(0, str.IndexOf('.')), out result);
    return result;
  }
}
