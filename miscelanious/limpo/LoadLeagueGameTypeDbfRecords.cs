using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLeagueGameTypeDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LeagueGameTypeDbfRecord> GetRecords()
  {
    LeagueGameTypeDbfAsset asset = this.assetBundleRequest.asset as LeagueGameTypeDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LeagueGameTypeDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLeagueGameTypeDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LeagueGameTypeDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
