using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadRewardTrackLevelDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<RewardTrackLevelDbfRecord> GetRecords()
  {
    RewardTrackLevelDbfAsset asset = this.assetBundleRequest.asset as RewardTrackLevelDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<RewardTrackLevelDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadRewardTrackLevelDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (RewardTrackLevelDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
