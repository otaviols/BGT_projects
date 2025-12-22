using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMultiClassGroupDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MultiClassGroupDbfRecord> GetRecords()
  {
    MultiClassGroupDbfAsset asset = this.assetBundleRequest.asset as MultiClassGroupDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MultiClassGroupDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMultiClassGroupDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MultiClassGroupDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
