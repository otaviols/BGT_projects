using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAdventureDeckDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AdventureDeckDbfRecord> GetRecords()
  {
    AdventureDeckDbfAsset asset = this.assetBundleRequest.asset as AdventureDeckDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AdventureDeckDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAdventureDeckDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AdventureDeckDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
