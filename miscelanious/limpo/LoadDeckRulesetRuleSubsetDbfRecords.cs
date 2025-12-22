using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadDeckRulesetRuleSubsetDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<DeckRulesetRuleSubsetDbfRecord> GetRecords()
  {
    DeckRulesetRuleSubsetDbfAsset asset = this.assetBundleRequest.asset as DeckRulesetRuleSubsetDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<DeckRulesetRuleSubsetDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadDeckRulesetRuleSubsetDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (DeckRulesetRuleSubsetDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
