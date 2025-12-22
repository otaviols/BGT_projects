using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBuildingTierDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BuildingTierDbfRecord> GetRecords()
  {
    BuildingTierDbfAsset asset = this.assetBundleRequest.asset as BuildingTierDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BuildingTierDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBuildingTierDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BuildingTierDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
