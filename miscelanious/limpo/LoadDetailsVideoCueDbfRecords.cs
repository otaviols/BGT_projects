using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadDetailsVideoCueDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<DetailsVideoCueDbfRecord> GetRecords()
  {
    DetailsVideoCueDbfAsset asset = this.assetBundleRequest.asset as DetailsVideoCueDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<DetailsVideoCueDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadDetailsVideoCueDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (DetailsVideoCueDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
