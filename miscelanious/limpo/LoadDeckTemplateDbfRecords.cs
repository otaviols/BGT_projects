using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadDeckTemplateDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<DeckTemplateDbfRecord> GetRecords()
  {
    DeckTemplateDbfAsset asset = this.assetBundleRequest.asset as DeckTemplateDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<DeckTemplateDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadDeckTemplateDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (DeckTemplateDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
