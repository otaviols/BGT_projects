using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardPlayerDeckOverrideDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardPlayerDeckOverrideDbfRecord> GetRecords()
  {
    CardPlayerDeckOverrideDbfAsset asset = this.assetBundleRequest.asset as CardPlayerDeckOverrideDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardPlayerDeckOverrideDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardPlayerDeckOverrideDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardPlayerDeckOverrideDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
