using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMercenariesRandomRewardDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MercenariesRandomRewardDbfRecord> GetRecords()
  {
    MercenariesRandomRewardDbfAsset asset = this.assetBundleRequest.asset as MercenariesRandomRewardDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MercenariesRandomRewardDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMercenariesRandomRewardDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MercenariesRandomRewardDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
