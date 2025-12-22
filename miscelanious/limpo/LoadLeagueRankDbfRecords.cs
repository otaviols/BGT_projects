using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLeagueRankDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LeagueRankDbfRecord> GetRecords()
  {
    LeagueRankDbfAsset asset = this.assetBundleRequest.asset as LeagueRankDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LeagueRankDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLeagueRankDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LeagueRankDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
