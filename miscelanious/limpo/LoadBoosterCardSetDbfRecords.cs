using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBoosterCardSetDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BoosterCardSetDbfRecord> GetRecords()
  {
    BoosterCardSetDbfAsset asset = this.assetBundleRequest.asset as BoosterCardSetDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BoosterCardSetDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBoosterCardSetDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BoosterCardSetDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
