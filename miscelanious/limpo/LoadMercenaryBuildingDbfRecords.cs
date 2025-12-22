using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMercenaryBuildingDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MercenaryBuildingDbfRecord> GetRecords()
  {
    MercenaryBuildingDbfAsset asset = this.assetBundleRequest.asset as MercenaryBuildingDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MercenaryBuildingDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMercenaryBuildingDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MercenaryBuildingDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
