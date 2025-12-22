using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadSubsetRuleDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<SubsetRuleDbfRecord> GetRecords()
  {
    SubsetRuleDbfAsset asset = this.assetBundleRequest.asset as SubsetRuleDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<SubsetRuleDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadSubsetRuleDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (SubsetRuleDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
