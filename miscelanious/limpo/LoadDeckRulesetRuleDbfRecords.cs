using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadDeckRulesetRuleDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<DeckRulesetRuleDbfRecord> GetRecords()
  {
    DeckRulesetRuleDbfAsset asset = this.assetBundleRequest.asset as DeckRulesetRuleDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<DeckRulesetRuleDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadDeckRulesetRuleDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (DeckRulesetRuleDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
