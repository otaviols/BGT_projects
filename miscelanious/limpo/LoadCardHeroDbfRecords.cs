using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardHeroDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardHeroDbfRecord> GetRecords()
  {
    CardHeroDbfAsset asset = this.assetBundleRequest.asset as CardHeroDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardHeroDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardHeroDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardHeroDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
