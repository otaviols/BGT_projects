using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLuckyDrawRewardsDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LuckyDrawRewardsDbfRecord> GetRecords()
  {
    LuckyDrawRewardsDbfAsset asset = this.assetBundleRequest.asset as LuckyDrawRewardsDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LuckyDrawRewardsDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLuckyDrawRewardsDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LuckyDrawRewardsDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
