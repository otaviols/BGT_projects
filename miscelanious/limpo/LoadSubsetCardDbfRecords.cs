using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadSubsetCardDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<SubsetCardDbfRecord> GetRecords()
  {
    SubsetCardDbfAsset asset = this.assetBundleRequest.asset as SubsetCardDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<SubsetCardDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadSubsetCardDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (SubsetCardDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
