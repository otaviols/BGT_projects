using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadXpOnPlacementDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<XpOnPlacementDbfRecord> GetRecords()
  {
    XpOnPlacementDbfAsset asset = this.assetBundleRequest.asset as XpOnPlacementDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<XpOnPlacementDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadXpOnPlacementDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (XpOnPlacementDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
