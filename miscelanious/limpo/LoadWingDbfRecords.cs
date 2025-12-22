using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadWingDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<WingDbfRecord> GetRecords()
  {
    WingDbfAsset asset = this.assetBundleRequest.asset as WingDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<WingDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadWingDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (WingDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
