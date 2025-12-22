using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBoosterDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BoosterDbfRecord> GetRecords()
  {
    BoosterDbfAsset asset = this.assetBundleRequest.asset as BoosterDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BoosterDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBoosterDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BoosterDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
