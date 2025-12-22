using Blizzard.T5.Configuration;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Util;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSettings
{
  public static bool s_isDeviceSupported = true;
  public static bool s_isDeviceInMinSpec = true;
  public static OSCategory s_os = OSCategory.PC;
  public static MemoryCategory s_memory = MemoryCategory.High;
  public static ScreenCategory s_screen = ScreenCategory.PC;
  public static InputCategory s_input = InputCategory.Mouse;
  public static ScreenDensityCategory s_screenDensity = ScreenDensityCategory.High;
  private static LocaleVariant s_CurrentLocaleVariant = LocaleVariant.Global;
  private static IGraphicsManager s_graphicsManager;
  private static string s_deviceModel = (string) null;
  private static bool s_isEmulating = false;

  public static OSCategory OS => PlatformSettings.s_os;

  public static OSCategory RuntimeOS => OSCategory.PC;

  public static LocaleVariant LocaleVariant
  {
    get
    {
      if (PlatformSettings.s_CurrentLocaleVariant != LocaleVariant.Invalid)
        return PlatformSettings.s_CurrentLocaleVariant;
      PlatformSettings.s_CurrentLocaleVariant = HearthstoneApplication.IsCNMobileBinary ? LocaleVariant.China : LocaleVariant.Global;
      return PlatformSettings.s_CurrentLocaleVariant;
    }
  }

  public static MemoryCategory Memory => PlatformSettings.s_memory;

  public static ScreenCategory Screen => PlatformSettings.s_screen;

  public static InputCategory Input => PlatformSettings.s_input;

  public static bool IsEmulating => PlatformSettings.s_isEmulating;

  public static string DeviceName => string.IsNullOrEmpty(SystemInfo.deviceModel) ? "unknown" : SystemInfo.deviceModel;

  public static string DeviceModel => PlatformSettings.s_deviceModel ?? PlatformSettings.DeviceName;

  public static bool ShouldFallbackToLowRes
  {
    get
    {
      if (Application.isEditor || PlatformSettings.IsEmulating)
        return false;
      if (PlatformSettings.Screen == ScreenCategory.Phone)
        return true;
      if (PlatformSettings.s_graphicsManager == null)
        PlatformSettings.s_graphicsManager = ServiceManager.Get<IGraphicsManager>();
      if (PlatformSettings.IsMobileRuntimeOS && PlatformSettings.s_graphicsManager != null && PlatformSettings.s_graphicsManager.RenderQualityLevel == GraphicsQuality.Low)
        return true;
      if (PlatformSettings.RuntimeOS == OSCategory.iOS)
      {
        NetCache netCache = NetCache.Get();
        if (netCache != null)
        {
          NetCache.NetCacheFeatures netObject = netCache.GetNetObject<NetCache.NetCacheFeatures>();
          if (netObject != null && netObject.ForceIosLowRes)
            return true;
        }
      }
      return false;
    }
  }

  public static int GetBestScreenMatch(List<ScreenCategory> categories)
  {
    ScreenCategory screen = PlatformSettings.Screen;
    int bestScreenMatch = 0;
    int num1 = int.MaxValue;
    for (int index = 0; index < categories.Count; ++index)
    {
      int num2 = categories[index] - screen;
      if (num2 >= 0 && num2 < num1)
      {
        bestScreenMatch = index;
        num1 = num2;
      }
    }
    return bestScreenMatch;
  }

  public static bool IsMobile() => PlatformSettings.OS == OSCategory.iOS || PlatformSettings.OS == OSCategory.Android;

  public static bool IsMobileRuntimeOS
  {
    get
    {
      OSCategory runtimeOs = PlatformSettings.RuntimeOS;
      return runtimeOs == OSCategory.iOS || runtimeOs == OSCategory.Android;
    }
  }

  public static bool IsTablet
  {
    get
    {
      if (!PlatformSettings.IsMobile())
        return false;
      return PlatformSettings.Screen == ScreenCategory.MiniTablet || PlatformSettings.Screen == ScreenCategory.Tablet;
    }
  }

  public static void RecomputeDeviceSettings()
  {
    if (PlatformSettings.EmulateMobileDevice())
      return;
    PlatformSettings.s_os = OSCategory.PC;
    PlatformSettings.s_input = InputCategory.Mouse;
    PlatformSettings.s_screen = ScreenCategory.PC;
    PlatformSettings.s_screenDensity = ScreenDensityCategory.High;
    PlatformSettings.s_os = OSCategory.PC;
    int systemMemorySize = SystemInfo.systemMemorySize;
    if (systemMemorySize < 500)
    {
      Debug.LogWarning((object) ("Low Memory Warning: Device has only " + (object) systemMemorySize + "MBs of system memory"));
      PlatformSettings.s_memory = MemoryCategory.Low;
    }
    else if (systemMemorySize < 1000)
      PlatformSettings.s_memory = MemoryCategory.Low;
    else if (systemMemorySize < 1500)
      PlatformSettings.s_memory = MemoryCategory.Medium;
    else
      PlatformSettings.s_memory = MemoryCategory.High;
  }

  private static bool EmulateMobileDevice()
  {
    if (HearthstoneApplication.IsPublic())
      return false;
    ConfigFile config = new ConfigFile();
    if (!config.FullLoad(PlatformFilePaths.GetClientConfigPath()))
    {
      Debug.LogWarningFormat("Failed to read DeviceEmulation from {0}", (object) PlatformFilePaths.GetClientConfigPath());
      return false;
    }
    DevicePreset devicePreset = new DevicePreset();
    devicePreset.ReadFromConfig(config);
    if (devicePreset.name == "No Emulation" || !config.Get("Emulation.emulateOnDevice", false))
      return false;
    PlatformSettings.s_isEmulating = true;
    PlatformSettings.s_os = devicePreset.os;
    PlatformSettings.s_input = devicePreset.input;
    PlatformSettings.s_screen = devicePreset.screen;
    PlatformSettings.s_screenDensity = devicePreset.screenDensity;
    Log.DeviceEmulation.Print("Emulating an " + devicePreset.name);
    return true;
  }
}
