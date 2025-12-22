using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadRegionOverridesDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<RegionOverridesDbfRecord> GetRecords()
  {
    RegionOverridesDbfAsset asset = this.assetBundleRequest.asset as RegionOverridesDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<RegionOverridesDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadRegionOverridesDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (RegionOverridesDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
