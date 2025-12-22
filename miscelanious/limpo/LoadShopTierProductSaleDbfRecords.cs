using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadShopTierProductSaleDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ShopTierProductSaleDbfRecord> GetRecords()
  {
    ShopTierProductSaleDbfAsset asset = this.assetBundleRequest.asset as ShopTierProductSaleDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ShopTierProductSaleDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadShopTierProductSaleDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ShopTierProductSaleDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
