using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAdventureGuestHeroesDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AdventureGuestHeroesDbfRecord> GetRecords()
  {
    AdventureGuestHeroesDbfAsset asset = this.assetBundleRequest.asset as AdventureGuestHeroesDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AdventureGuestHeroesDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAdventureGuestHeroesDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AdventureGuestHeroesDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
