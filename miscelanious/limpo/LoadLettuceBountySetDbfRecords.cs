using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceBountySetDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceBountySetDbfRecord> GetRecords()
  {
    LettuceBountySetDbfAsset asset = this.assetBundleRequest.asset as LettuceBountySetDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceBountySetDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceBountySetDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceBountySetDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
