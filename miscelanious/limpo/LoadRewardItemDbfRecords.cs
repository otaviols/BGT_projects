using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadRewardItemDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<RewardItemDbfRecord> GetRecords()
  {
    RewardItemDbfAsset asset = this.assetBundleRequest.asset as RewardItemDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<RewardItemDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadRewardItemDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (RewardItemDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
