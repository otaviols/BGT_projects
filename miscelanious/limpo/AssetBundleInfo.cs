using Hearthstone.Util;
using System.Collections.Generic;

public class AssetBundleInfo
{
  private static Dictionary<string, string> s_bundleNameToPath = new Dictionary<string, string>();

  public static string BundlePathPlatformModifier() => "Win/";

  public static string GetAssetBundlePath(string bundleName)
  {
    string assetBundlePath = string.Empty;
    if (bundleName != null && !AssetBundleInfo.s_bundleNameToPath.TryGetValue(bundleName, out assetBundlePath))
    {
      assetBundlePath = PlatformFilePaths.CreateLocalFilePath(string.Format("Data/{0}{1}", (object) AssetBundleInfo.BundlePathPlatformModifier(), (object) bundleName));
      AssetBundleInfo.s_bundleNameToPath[bundleName] = assetBundlePath;
    }
    return assetBundlePath;
  }
}
