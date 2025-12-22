using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBattlegroundsEmoteDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BattlegroundsEmoteDbfRecord> GetRecords()
  {
    BattlegroundsEmoteDbfAsset asset = this.assetBundleRequest.asset as BattlegroundsEmoteDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BattlegroundsEmoteDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBattlegroundsEmoteDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BattlegroundsEmoteDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
