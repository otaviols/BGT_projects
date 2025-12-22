using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceEquipmentDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceEquipmentDbfRecord> GetRecords()
  {
    LettuceEquipmentDbfAsset asset = this.assetBundleRequest.asset as LettuceEquipmentDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceEquipmentDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceEquipmentDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceEquipmentDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
