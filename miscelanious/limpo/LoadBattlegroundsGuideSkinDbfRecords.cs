using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBattlegroundsGuideSkinDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BattlegroundsGuideSkinDbfRecord> GetRecords()
  {
    BattlegroundsGuideSkinDbfAsset asset = this.assetBundleRequest.asset as BattlegroundsGuideSkinDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BattlegroundsGuideSkinDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBattlegroundsGuideSkinDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BattlegroundsGuideSkinDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
