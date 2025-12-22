using Blizzard.T5.AssetManager;
using Hearthstone.Core;

public class AssetLocator : IAssetLocator
{
  private readonly IAssetManifest m_assetManifest;

  public AssetLocator(IAssetManifest assetManifest) => this.m_assetManifest = assetManifest;

  public bool TryLocateAsset(string assetAddress, out string bundleName, out string assetPath)
  {
    assetPath = assetAddress;
    return this.m_assetManifest.TryGetDirectBundleFromGuid(assetAddress, out bundleName);
  }

  public string GetBundlePath(string bundleName) => AssetBundleInfo.GetAssetBundlePath(bundleName);
}
