using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceTreasureTierDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceTreasureTierDbfRecord> GetRecords()
  {
    LettuceTreasureTierDbfAsset asset = this.assetBundleRequest.asset as LettuceTreasureTierDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceTreasureTierDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceTreasureTierDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceTreasureTierDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
