using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCoinDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CoinDbfRecord> GetRecords()
  {
    CoinDbfAsset asset = this.assetBundleRequest.asset as CoinDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CoinDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCoinDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CoinDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
