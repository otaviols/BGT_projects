using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAdventureDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AdventureDbfRecord> GetRecords()
  {
    AdventureDbfAsset asset = this.assetBundleRequest.asset as AdventureDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AdventureDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAdventureDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AdventureDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
