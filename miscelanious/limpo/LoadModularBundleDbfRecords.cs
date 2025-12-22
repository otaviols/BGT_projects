using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadModularBundleDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ModularBundleDbfRecord> GetRecords()
  {
    ModularBundleDbfAsset asset = this.assetBundleRequest.asset as ModularBundleDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ModularBundleDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadModularBundleDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ModularBundleDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
