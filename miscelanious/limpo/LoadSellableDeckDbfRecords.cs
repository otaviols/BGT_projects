using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadSellableDeckDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<SellableDeckDbfRecord> GetRecords()
  {
    SellableDeckDbfAsset asset = this.assetBundleRequest.asset as SellableDeckDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<SellableDeckDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadSellableDeckDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (SellableDeckDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
