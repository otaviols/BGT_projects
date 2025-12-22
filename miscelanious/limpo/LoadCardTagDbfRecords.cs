using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardTagDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardTagDbfRecord> GetRecords()
  {
    CardTagDbfAsset asset = this.assetBundleRequest.asset as CardTagDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardTagDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardTagDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardTagDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
