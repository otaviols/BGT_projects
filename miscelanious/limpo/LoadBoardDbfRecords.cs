using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBoardDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BoardDbfRecord> GetRecords()
  {
    BoardDbfAsset asset = this.assetBundleRequest.asset as BoardDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BoardDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBoardDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BoardDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
