using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardValueDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardValueDbfRecord> GetRecords()
  {
    CardValueDbfAsset asset = this.assetBundleRequest.asset as CardValueDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardValueDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardValueDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardValueDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
