using Blizzard.T5.Jobs;
using Hearthstone.Util;
using System.Collections.Generic;
using UnityEngine;

public static class DbfShared
{
  private static AssetBundle s_assetBundle = (AssetBundle) null;
  private static SpecialEventMap s_eventMap = ScriptableObject.CreateInstance<SpecialEventMap>();

  public static AssetBundle GetAssetBundle() => DbfShared.s_assetBundle;

  public static SpecialEventMap GetEventMap() => DbfShared.s_eventMap;

  public static void LoadSharedAssetBundle()
  {
    string dbfAssetBundlePath = DbfShared.GetSharedDBFAssetBundlePath();
    DbfShared.s_assetBundle = AssetBundle.LoadFromFile(dbfAssetBundlePath);
    if ((Object) DbfShared.s_assetBundle == (Object) null)
      Debug.LogErrorFormat("Failed to load DBF asset bundle from: \"{0}\"", (object) dbfAssetBundlePath);
    else
      DbfShared.LoadSpecialEventMap();
  }

  public static IEnumerator<IAsyncJobResult> Job_LoadSharedDBFAssetBundle()
  {
    LoadAssetBundleFromFile loadDBFSharedAssetBundle = new LoadAssetBundleFromFile(DbfShared.GetSharedDBFAssetBundlePath(), true);
    yield return (IAsyncJobResult) loadDBFSharedAssetBundle;
    DbfShared.s_assetBundle = loadDBFSharedAssetBundle.LoadedAssetBundle;
    DbfShared.LoadSpecialEventMap();
  }

  private static void LoadSpecialEventMap()
  {
    Locale actualLocale = Localization.GetActualLocale();
    DbfShared.s_eventMap = DbfShared.s_assetBundle.LoadAsset<SpecialEventMap>("Assets/Game/DBF-Asset/" + (object) actualLocale + "/EventMap.asset");
    DbfShared.s_eventMap.Initialize();
  }

  private static string GetSharedDBFAssetBundlePath()
  {
    Locale actualLocale = Localization.GetActualLocale();
    return AssetLoaderPrefs.AssetLoadingMethod == AssetLoaderPrefs.ASSET_LOADING_METHOD.ASSET_BUNDLES ? PlatformFilePaths.CreateLocalFilePath(string.Format("Data/{0}dbf_{1}.unity3d", (object) AssetBundleInfo.BundlePathPlatformModifier(), (object) actualLocale.ToString().ToLower())) : "Assets/Game/DBF-Asset/dbf_" + actualLocale.ToString().ToLower() + ".unity3d";
  }

  public static void Reset()
  {
    if ((Object) DbfShared.s_assetBundle != (Object) null)
      DbfShared.s_assetBundle.Unload(true);
    DbfShared.s_assetBundle = (AssetBundle) null;
  }
}
