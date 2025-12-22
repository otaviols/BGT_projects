using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadFixedRewardMapDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<FixedRewardMapDbfRecord> GetRecords()
  {
    FixedRewardMapDbfAsset asset = this.assetBundleRequest.asset as FixedRewardMapDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<FixedRewardMapDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadFixedRewardMapDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (FixedRewardMapDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
