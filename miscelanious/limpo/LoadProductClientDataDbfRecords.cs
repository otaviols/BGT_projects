using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadProductClientDataDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ProductClientDataDbfRecord> GetRecords()
  {
    ProductClientDataDbfAsset asset = this.assetBundleRequest.asset as ProductClientDataDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ProductClientDataDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadProductClientDataDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ProductClientDataDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
