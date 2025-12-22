using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadDeckDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<DeckDbfRecord> GetRecords()
  {
    DeckDbfAsset asset = this.assetBundleRequest.asset as DeckDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<DeckDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadDeckDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (DeckDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
