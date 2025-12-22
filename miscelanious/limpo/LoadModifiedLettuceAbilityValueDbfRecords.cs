using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadModifiedLettuceAbilityValueDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ModifiedLettuceAbilityValueDbfRecord> GetRecords()
  {
    ModifiedLettuceAbilityValueDbfAsset asset = this.assetBundleRequest.asset as ModifiedLettuceAbilityValueDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ModifiedLettuceAbilityValueDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadModifiedLettuceAbilityValueDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ModifiedLettuceAbilityValueDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
