using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadDeckCardDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<DeckCardDbfRecord> GetRecords()
  {
    DeckCardDbfAsset asset = this.assetBundleRequest.asset as DeckCardDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<DeckCardDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadDeckCardDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (DeckCardDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
