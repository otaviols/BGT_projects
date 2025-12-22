using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceAbilityDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceAbilityDbfRecord> GetRecords()
  {
    LettuceAbilityDbfAsset asset = this.assetBundleRequest.asset as LettuceAbilityDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceAbilityDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceAbilityDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceAbilityDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
