using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMercenariesRankedSeasonRewardRankDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MercenariesRankedSeasonRewardRankDbfRecord> GetRecords()
  {
    MercenariesRankedSeasonRewardRankDbfAsset asset = this.assetBundleRequest.asset as MercenariesRankedSeasonRewardRankDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MercenariesRankedSeasonRewardRankDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMercenariesRankedSeasonRewardRankDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MercenariesRankedSeasonRewardRankDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
