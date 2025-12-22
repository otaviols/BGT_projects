using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadVisitorTaskDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<VisitorTaskDbfRecord> GetRecords()
  {
    VisitorTaskDbfAsset asset = this.assetBundleRequest.asset as VisitorTaskDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<VisitorTaskDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadVisitorTaskDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (VisitorTaskDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
