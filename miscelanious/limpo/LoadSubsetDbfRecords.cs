using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadSubsetDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<SubsetDbfRecord> GetRecords()
  {
    SubsetDbfAsset asset = this.assetBundleRequest.asset as SubsetDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<SubsetDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadSubsetDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (SubsetDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
