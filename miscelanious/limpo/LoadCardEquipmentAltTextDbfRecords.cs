using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardEquipmentAltTextDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardEquipmentAltTextDbfRecord> GetRecords()
  {
    CardEquipmentAltTextDbfAsset asset = this.assetBundleRequest.asset as CardEquipmentAltTextDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardEquipmentAltTextDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardEquipmentAltTextDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardEquipmentAltTextDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
