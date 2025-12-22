using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadVisitorTaskChainDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<VisitorTaskChainDbfRecord> GetRecords()
  {
    VisitorTaskChainDbfAsset asset = this.assetBundleRequest.asset as VisitorTaskChainDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<VisitorTaskChainDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadVisitorTaskChainDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (VisitorTaskChainDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
