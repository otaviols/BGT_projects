using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAchieveRegionDataDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AchieveRegionDataDbfRecord> GetRecords()
  {
    AchieveRegionDataDbfAsset asset = this.assetBundleRequest.asset as AchieveRegionDataDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AchieveRegionDataDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAchieveRegionDataDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AchieveRegionDataDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
