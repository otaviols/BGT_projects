using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardSetTimingDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardSetTimingDbfRecord> GetRecords()
  {
    CardSetTimingDbfAsset asset = this.assetBundleRequest.asset as CardSetTimingDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardSetTimingDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardSetTimingDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardSetTimingDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
