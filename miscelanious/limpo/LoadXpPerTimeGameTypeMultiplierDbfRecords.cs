using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadXpPerTimeGameTypeMultiplierDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<XpPerTimeGameTypeMultiplierDbfRecord> GetRecords()
  {
    XpPerTimeGameTypeMultiplierDbfAsset asset = this.assetBundleRequest.asset as XpPerTimeGameTypeMultiplierDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<XpPerTimeGameTypeMultiplierDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadXpPerTimeGameTypeMultiplierDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (XpPerTimeGameTypeMultiplierDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
