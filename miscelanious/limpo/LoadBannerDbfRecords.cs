using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBannerDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BannerDbfRecord> GetRecords()
  {
    BannerDbfAsset asset = this.assetBundleRequest.asset as BannerDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BannerDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBannerDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BannerDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
