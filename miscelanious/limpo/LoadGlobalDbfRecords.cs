using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadGlobalDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<GlobalDbfRecord> GetRecords()
  {
    GlobalDbfAsset asset = this.assetBundleRequest.asset as GlobalDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<GlobalDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadGlobalDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (GlobalDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
