using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadRewardBagDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<RewardBagDbfRecord> GetRecords()
  {
    RewardBagDbfAsset asset = this.assetBundleRequest.asset as RewardBagDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<RewardBagDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadRewardBagDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (RewardBagDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
