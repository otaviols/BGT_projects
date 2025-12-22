using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceMercenarySpecializationDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceMercenarySpecializationDbfRecord> GetRecords()
  {
    LettuceMercenarySpecializationDbfAsset asset = this.assetBundleRequest.asset as LettuceMercenarySpecializationDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceMercenarySpecializationDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceMercenarySpecializationDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceMercenarySpecializationDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
