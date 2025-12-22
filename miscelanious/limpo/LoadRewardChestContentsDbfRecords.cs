using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadRewardChestContentsDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<RewardChestContentsDbfRecord> GetRecords()
  {
    RewardChestContentsDbfAsset asset = this.assetBundleRequest.asset as RewardChestContentsDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<RewardChestContentsDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadRewardChestContentsDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (RewardChestContentsDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
