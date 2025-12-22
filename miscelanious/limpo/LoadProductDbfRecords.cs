using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadProductDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ProductDbfRecord> GetRecords()
  {
    ProductDbfAsset asset = this.assetBundleRequest.asset as ProductDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ProductDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadProductDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ProductDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
