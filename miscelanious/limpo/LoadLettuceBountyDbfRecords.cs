using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceBountyDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceBountyDbfRecord> GetRecords()
  {
    LettuceBountyDbfAsset asset = this.assetBundleRequest.asset as LettuceBountyDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceBountyDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceBountyDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceBountyDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
