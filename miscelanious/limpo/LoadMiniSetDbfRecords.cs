using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMiniSetDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MiniSetDbfRecord> GetRecords()
  {
    MiniSetDbfAsset asset = this.assetBundleRequest.asset as MiniSetDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MiniSetDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMiniSetDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MiniSetDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
