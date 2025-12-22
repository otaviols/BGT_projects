using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBattlegroundsFinisherDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BattlegroundsFinisherDbfRecord> GetRecords()
  {
    BattlegroundsFinisherDbfAsset asset = this.assetBundleRequest.asset as BattlegroundsFinisherDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BattlegroundsFinisherDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBattlegroundsFinisherDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BattlegroundsFinisherDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
