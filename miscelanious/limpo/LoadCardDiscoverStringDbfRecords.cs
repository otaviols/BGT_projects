using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardDiscoverStringDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardDiscoverStringDbfRecord> GetRecords()
  {
    CardDiscoverStringDbfAsset asset = this.assetBundleRequest.asset as CardDiscoverStringDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardDiscoverStringDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardDiscoverStringDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardDiscoverStringDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
