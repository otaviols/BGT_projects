using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBattlegroundsBoardSkinDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BattlegroundsBoardSkinDbfRecord> GetRecords()
  {
    BattlegroundsBoardSkinDbfAsset asset = this.assetBundleRequest.asset as BattlegroundsBoardSkinDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BattlegroundsBoardSkinDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBattlegroundsBoardSkinDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BattlegroundsBoardSkinDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
