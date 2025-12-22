using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateUtils
{
  private static readonly Map<AndroidStore, string> s_androidStoreUrls = new Map<AndroidStore, string>()
  {
    {
      AndroidStore.AMAZON,
      "http://www.amazon.com/gp/mas/dl/android?p=com.blizzard.wtcg.hearthstone"
    },
    {
      AndroidStore.GOOGLE,
      "https://play.google.com/store/apps/details?id=com.blizzard.wtcg.hearthstone"
    },
    {
      AndroidStore.ONE_STORE,
      "https://onesto.re/OA00752154"
    }
  };

  public static bool addSkipBackupAttributeToItemAtPath(string path) => true;

  public static void ShowWirelessSettings()
  {
  }

  public static bool AreUpdatesEnabledForCurrentPlatform => PlatformSettings.IsMobileRuntimeOS && !DemoMgr.Get().IsDemo();

  public static void ResizeListIfNeeded(List<string> list, int minSize)
  {
    if (list.Capacity >= minSize)
      return;
    list.Capacity = minSize;
  }

  public static string GetLocaleFromAssetBundle(string assetBundleName)
  {
    string[] strArray = assetBundleName.Split('-')[0].Split('_');
    if (strArray.Length != 0)
    {
      string str = strArray[strArray.Length - 1];
      string localeName = str.Substring(0, 2) + str.Substring(2, 2).ToUpper();
      if (Localization.IsValidLocaleName(localeName))
        return localeName;
    }
    return string.Empty;
  }

  public static void OpenAppStore()
  {
    AndroidStore androidStore = AndroidDeviceSettings.Get().GetAndroidStore();
    PlatformDependentValue<string> url = new PlatformDependentValue<string>(PlatformCategory.OS)
    {
      iOS = "https://itunes.apple.com/app/hearthstone-heroes-warcraft/id625257520?ls=1&mt=8",
      Android = UpdateUtils.GetAndroidStoreUrl(androidStore)
    };
    PlatformDependentValue<string> platformDependentValue = new PlatformDependentValue<string>(PlatformCategory.OS)
    {
      iOS = "https://itunes.apple.com/cn/app/lu-shi-chuan-shuo-mo-shou/id841140063?ls=1&mt=8",
      Android = androidStore == AndroidStore.HUAWEI ? "https://a.vmall.com/order/app?appId=C101669777&pkgName=com.blizzard.wtcg.hearthstone.huawei" : "https://www.battlenet.com.cn/account/download/hearthstone/android?style=hearthstone"
    };
    if (MobileDeviceLocale.GetCurrentRegionId() == BnetRegion.REGION_CN)
      url = platformDependentValue;
    Application.OpenURL((string) url);
  }

  private static string GetAndroidStoreUrl(AndroidStore store)
  {
    string str;
    return UpdateUtils.s_androidStoreUrls.TryGetValue(store, out str) ? str : string.Empty;
  }

  public static bool GetSplitVersion(string versionStr, out int[] versionInt)
  {
    Log.Downloader.PrintInfo("VersionStr=" + versionStr);
    try
    {
      List<string> stringList = new List<string>();
      string[] strArray = versionStr.Split('_');
      int num = 4;
      if (strArray.Length == 1)
      {
        stringList.AddRange((IEnumerable<string>) versionStr.Split('.'));
      }
      else
      {
        string str = Vars.Key("Mobile.BinaryVersion").GetStr("");
        string empty = string.Empty;
        if (!string.IsNullOrEmpty(str))
        {
          string oldValue = "." + str;
          strArray[1] = strArray[1].Replace(oldValue, "");
        }
        stringList.AddRange((IEnumerable<string>) strArray[1].Split('-')[0].Split('.'));
        stringList.Add(strArray[0]);
        if (!string.IsNullOrEmpty(str))
        {
          stringList.Add(str);
          ++num;
        }
      }
      versionInt = Array.ConvertAll<string, int>(stringList.ToArray(), new Converter<string, int>(int.Parse));
      if (versionInt.Length < num)
        throw new Exception("Version is too short");
    }
    catch (Exception ex)
    {
      Error.AddDevFatal("Failed to parse the version string-'{0}': {1}", (object) versionStr, (object) ex.Message);
      versionInt = new int[0];
      return false;
    }
    return true;
  }
}
