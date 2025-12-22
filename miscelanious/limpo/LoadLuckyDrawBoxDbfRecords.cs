using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLuckyDrawBoxDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LuckyDrawBoxDbfRecord> GetRecords()
  {
    LuckyDrawBoxDbfAsset asset = this.assetBundleRequest.asset as LuckyDrawBoxDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LuckyDrawBoxDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLuckyDrawBoxDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LuckyDrawBoxDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
