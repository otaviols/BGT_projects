using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAdventureDataDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AdventureDataDbfRecord> GetRecords()
  {
    AdventureDataDbfAsset asset = this.assetBundleRequest.asset as AdventureDataDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AdventureDataDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAdventureDataDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AdventureDataDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
