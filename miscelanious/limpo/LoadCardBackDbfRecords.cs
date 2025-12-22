using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardBackDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardBackDbfRecord> GetRecords()
  {
    CardBackDbfAsset asset = this.assetBundleRequest.asset as CardBackDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardBackDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardBackDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardBackDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
