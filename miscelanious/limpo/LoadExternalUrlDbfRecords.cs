using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadExternalUrlDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ExternalUrlDbfRecord> GetRecords()
  {
    ExternalUrlDbfAsset asset = this.assetBundleRequest.asset as ExternalUrlDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ExternalUrlDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadExternalUrlDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ExternalUrlDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
