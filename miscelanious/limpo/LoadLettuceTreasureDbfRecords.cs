using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceTreasureDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceTreasureDbfRecord> GetRecords()
  {
    LettuceTreasureDbfAsset asset = this.assetBundleRequest.asset as LettuceTreasureDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceTreasureDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceTreasureDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceTreasureDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
