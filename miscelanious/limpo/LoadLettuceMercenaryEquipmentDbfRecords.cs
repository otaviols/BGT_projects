using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceMercenaryEquipmentDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceMercenaryEquipmentDbfRecord> GetRecords()
  {
    LettuceMercenaryEquipmentDbfAsset asset = this.assetBundleRequest.asset as LettuceMercenaryEquipmentDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceMercenaryEquipmentDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceMercenaryEquipmentDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceMercenaryEquipmentDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
