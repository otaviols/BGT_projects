using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceMercenaryLevelDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceMercenaryLevelDbfRecord> GetRecords()
  {
    LettuceMercenaryLevelDbfAsset asset = this.assetBundleRequest.asset as LettuceMercenaryLevelDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceMercenaryLevelDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceMercenaryLevelDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceMercenaryLevelDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
