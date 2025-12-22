using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadFixedRewardDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<FixedRewardDbfRecord> GetRecords()
  {
    FixedRewardDbfAsset asset = this.assetBundleRequest.asset as FixedRewardDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<FixedRewardDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadFixedRewardDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (FixedRewardDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
