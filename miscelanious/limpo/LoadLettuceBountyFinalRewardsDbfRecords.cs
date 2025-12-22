using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceBountyFinalRewardsDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceBountyFinalRewardsDbfRecord> GetRecords()
  {
    LettuceBountyFinalRewardsDbfAsset asset = this.assetBundleRequest.asset as LettuceBountyFinalRewardsDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceBountyFinalRewardsDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceBountyFinalRewardsDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceBountyFinalRewardsDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
