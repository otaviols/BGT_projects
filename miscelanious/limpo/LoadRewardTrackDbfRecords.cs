using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadRewardTrackDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<RewardTrackDbfRecord> GetRecords()
  {
    RewardTrackDbfAsset asset = this.assetBundleRequest.asset as RewardTrackDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<RewardTrackDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadRewardTrackDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (RewardTrackDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
