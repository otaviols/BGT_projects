using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadInitCardValueDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<InitCardValueDbfRecord> GetRecords()
  {
    InitCardValueDbfAsset asset = this.assetBundleRequest.asset as InitCardValueDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<InitCardValueDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadInitCardValueDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (InitCardValueDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
