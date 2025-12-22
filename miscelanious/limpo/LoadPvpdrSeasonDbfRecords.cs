using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadPvpdrSeasonDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<PvpdrSeasonDbfRecord> GetRecords()
  {
    PvpdrSeasonDbfAsset asset = this.assetBundleRequest.asset as PvpdrSeasonDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<PvpdrSeasonDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadPvpdrSeasonDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (PvpdrSeasonDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
