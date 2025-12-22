using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceEquipmentTierDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceEquipmentTierDbfRecord> GetRecords()
  {
    LettuceEquipmentTierDbfAsset asset = this.assetBundleRequest.asset as LettuceEquipmentTierDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceEquipmentTierDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceEquipmentTierDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceEquipmentTierDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
