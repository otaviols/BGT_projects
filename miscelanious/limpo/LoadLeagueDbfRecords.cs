using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLeagueDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LeagueDbfRecord> GetRecords()
  {
    LeagueDbfAsset asset = this.assetBundleRequest.asset as LeagueDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LeagueDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLeagueDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LeagueDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
