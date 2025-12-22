using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMercenaryArtVariationDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MercenaryArtVariationDbfRecord> GetRecords()
  {
    MercenaryArtVariationDbfAsset asset = this.assetBundleRequest.asset as MercenaryArtVariationDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MercenaryArtVariationDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMercenaryArtVariationDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MercenaryArtVariationDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
