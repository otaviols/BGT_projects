using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardDbfRecord> GetRecords()
  {
    CardDbfAsset asset = this.assetBundleRequest.asset as CardDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
