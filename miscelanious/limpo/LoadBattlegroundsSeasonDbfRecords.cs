using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBattlegroundsSeasonDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BattlegroundsSeasonDbfRecord> GetRecords()
  {
    BattlegroundsSeasonDbfAsset asset = this.assetBundleRequest.asset as BattlegroundsSeasonDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BattlegroundsSeasonDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBattlegroundsSeasonDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BattlegroundsSeasonDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
