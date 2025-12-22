using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardSetDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardSetDbfRecord> GetRecords()
  {
    CardSetDbfAsset asset = this.assetBundleRequest.asset as CardSetDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardSetDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardSetDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardSetDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
