using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceEquipmentModifierDataDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceEquipmentModifierDataDbfRecord> GetRecords()
  {
    LettuceEquipmentModifierDataDbfAsset asset = this.assetBundleRequest.asset as LettuceEquipmentModifierDataDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceEquipmentModifierDataDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceEquipmentModifierDataDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceEquipmentModifierDataDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
