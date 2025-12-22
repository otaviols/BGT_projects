using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardChangeDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardChangeDbfRecord> GetRecords()
  {
    CardChangeDbfAsset asset = this.assetBundleRequest.asset as CardChangeDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardChangeDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardChangeDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardChangeDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
