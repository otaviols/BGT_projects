using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadModularBundleLayoutNodeDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ModularBundleLayoutNodeDbfRecord> GetRecords()
  {
    ModularBundleLayoutNodeDbfAsset asset = this.assetBundleRequest.asset as ModularBundleLayoutNodeDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ModularBundleLayoutNodeDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadModularBundleLayoutNodeDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ModularBundleLayoutNodeDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
