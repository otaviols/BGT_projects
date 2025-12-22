using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadModifiedLettuceAbilityCardTagDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ModifiedLettuceAbilityCardTagDbfRecord> GetRecords()
  {
    ModifiedLettuceAbilityCardTagDbfAsset asset = this.assetBundleRequest.asset as ModifiedLettuceAbilityCardTagDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ModifiedLettuceAbilityCardTagDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadModifiedLettuceAbilityCardTagDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ModifiedLettuceAbilityCardTagDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
