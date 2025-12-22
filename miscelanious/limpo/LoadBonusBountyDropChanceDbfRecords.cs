using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadBonusBountyDropChanceDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<BonusBountyDropChanceDbfRecord> GetRecords()
  {
    BonusBountyDropChanceDbfAsset asset = this.assetBundleRequest.asset as BonusBountyDropChanceDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<BonusBountyDropChanceDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadBonusBountyDropChanceDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (BonusBountyDropChanceDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
