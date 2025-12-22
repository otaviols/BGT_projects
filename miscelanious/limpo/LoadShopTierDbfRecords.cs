using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadShopTierDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ShopTierDbfRecord> GetRecords()
  {
    ShopTierDbfAsset asset = this.assetBundleRequest.asset as ShopTierDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ShopTierDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadShopTierDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ShopTierDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
