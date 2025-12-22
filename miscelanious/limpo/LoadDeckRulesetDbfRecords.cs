using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadDeckRulesetDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<DeckRulesetDbfRecord> GetRecords()
  {
    DeckRulesetDbfAsset asset = this.assetBundleRequest.asset as DeckRulesetDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<DeckRulesetDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadDeckRulesetDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (DeckRulesetDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
