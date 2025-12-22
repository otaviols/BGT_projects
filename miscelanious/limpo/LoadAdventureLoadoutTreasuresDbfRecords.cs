using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAdventureLoadoutTreasuresDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AdventureLoadoutTreasuresDbfRecord> GetRecords()
  {
    AdventureLoadoutTreasuresDbfAsset asset = this.assetBundleRequest.asset as AdventureLoadoutTreasuresDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AdventureLoadoutTreasuresDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAdventureLoadoutTreasuresDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AdventureLoadoutTreasuresDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
