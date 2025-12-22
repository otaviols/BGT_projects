using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAdventureHeroPowerDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AdventureHeroPowerDbfRecord> GetRecords()
  {
    AdventureHeroPowerDbfAsset asset = this.assetBundleRequest.asset as AdventureHeroPowerDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AdventureHeroPowerDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAdventureHeroPowerDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AdventureHeroPowerDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
