using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadRewardListDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<RewardListDbfRecord> GetRecords()
  {
    RewardListDbfAsset asset = this.assetBundleRequest.asset as RewardListDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<RewardListDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadRewardListDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (RewardListDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
