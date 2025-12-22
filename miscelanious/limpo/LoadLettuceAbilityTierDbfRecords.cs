using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceAbilityTierDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceAbilityTierDbfRecord> GetRecords()
  {
    LettuceAbilityTierDbfAsset asset = this.assetBundleRequest.asset as LettuceAbilityTierDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceAbilityTierDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceAbilityTierDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceAbilityTierDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
