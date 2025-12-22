using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadRewardChestDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<RewardChestDbfRecord> GetRecords()
  {
    RewardChestDbfAsset asset = this.assetBundleRequest.asset as RewardChestDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<RewardChestDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadRewardChestDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (RewardChestDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
