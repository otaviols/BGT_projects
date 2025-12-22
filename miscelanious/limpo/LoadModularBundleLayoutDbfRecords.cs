using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadModularBundleLayoutDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ModularBundleLayoutDbfRecord> GetRecords()
  {
    ModularBundleLayoutDbfAsset asset = this.assetBundleRequest.asset as ModularBundleLayoutDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ModularBundleLayoutDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadModularBundleLayoutDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ModularBundleLayoutDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
