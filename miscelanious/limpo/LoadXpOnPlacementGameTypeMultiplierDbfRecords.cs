using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadXpOnPlacementGameTypeMultiplierDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<XpOnPlacementGameTypeMultiplierDbfRecord> GetRecords()
  {
    XpOnPlacementGameTypeMultiplierDbfAsset asset = this.assetBundleRequest.asset as XpOnPlacementGameTypeMultiplierDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<XpOnPlacementGameTypeMultiplierDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadXpOnPlacementGameTypeMultiplierDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (XpOnPlacementGameTypeMultiplierDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
