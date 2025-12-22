using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadFixedRewardActionDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<FixedRewardActionDbfRecord> GetRecords()
  {
    FixedRewardActionDbfAsset asset = this.assetBundleRequest.asset as FixedRewardActionDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<FixedRewardActionDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadFixedRewardActionDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (FixedRewardActionDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
