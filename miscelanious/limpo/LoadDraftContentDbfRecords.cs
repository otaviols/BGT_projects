using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadDraftContentDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<DraftContentDbfRecord> GetRecords()
  {
    DraftContentDbfAsset asset = this.assetBundleRequest.asset as DraftContentDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<DraftContentDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadDraftContentDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (DraftContentDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
