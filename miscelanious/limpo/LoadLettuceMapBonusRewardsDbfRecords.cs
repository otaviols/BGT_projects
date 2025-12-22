using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceMapBonusRewardsDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceMapBonusRewardsDbfRecord> GetRecords()
  {
    LettuceMapBonusRewardsDbfAsset asset = this.assetBundleRequest.asset as LettuceMapBonusRewardsDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceMapBonusRewardsDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceMapBonusRewardsDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceMapBonusRewardsDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
