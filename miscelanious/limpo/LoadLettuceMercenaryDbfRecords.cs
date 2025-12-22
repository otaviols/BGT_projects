using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceMercenaryDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceMercenaryDbfRecord> GetRecords()
  {
    LettuceMercenaryDbfAsset asset = this.assetBundleRequest.asset as LettuceMercenaryDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceMercenaryDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceMercenaryDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceMercenaryDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
