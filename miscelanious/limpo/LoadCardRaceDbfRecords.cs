using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardRaceDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardRaceDbfRecord> GetRecords()
  {
    CardRaceDbfAsset asset = this.assetBundleRequest.asset as CardRaceDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardRaceDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardRaceDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardRaceDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
