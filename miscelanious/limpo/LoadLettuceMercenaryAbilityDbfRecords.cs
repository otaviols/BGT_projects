using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceMercenaryAbilityDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceMercenaryAbilityDbfRecord> GetRecords()
  {
    LettuceMercenaryAbilityDbfAsset asset = this.assetBundleRequest.asset as LettuceMercenaryAbilityDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceMercenaryAbilityDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceMercenaryAbilityDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceMercenaryAbilityDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
