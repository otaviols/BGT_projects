using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLeagueBgPublicRatingEquivDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LeagueBgPublicRatingEquivDbfRecord> GetRecords()
  {
    LeagueBgPublicRatingEquivDbfAsset asset = this.assetBundleRequest.asset as LeagueBgPublicRatingEquivDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LeagueBgPublicRatingEquivDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLeagueBgPublicRatingEquivDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LeagueBgPublicRatingEquivDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
