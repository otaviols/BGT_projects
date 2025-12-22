using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadNextTiersDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<NextTiersDbfRecord> GetRecords()
  {
    NextTiersDbfAsset asset = this.assetBundleRequest.asset as NextTiersDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<NextTiersDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadNextTiersDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (NextTiersDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
