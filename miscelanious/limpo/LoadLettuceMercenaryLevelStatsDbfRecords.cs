using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceMercenaryLevelStatsDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceMercenaryLevelStatsDbfRecord> GetRecords()
  {
    LettuceMercenaryLevelStatsDbfAsset asset = this.assetBundleRequest.asset as LettuceMercenaryLevelStatsDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceMercenaryLevelStatsDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceMercenaryLevelStatsDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceMercenaryLevelStatsDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
