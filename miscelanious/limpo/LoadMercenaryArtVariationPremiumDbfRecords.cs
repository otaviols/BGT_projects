using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMercenaryArtVariationPremiumDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MercenaryArtVariationPremiumDbfRecord> GetRecords()
  {
    MercenaryArtVariationPremiumDbfAsset asset = this.assetBundleRequest.asset as MercenaryArtVariationPremiumDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MercenaryArtVariationPremiumDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMercenaryArtVariationPremiumDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MercenaryArtVariationPremiumDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
