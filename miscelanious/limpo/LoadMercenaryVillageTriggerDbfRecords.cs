using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMercenaryVillageTriggerDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MercenaryVillageTriggerDbfRecord> GetRecords()
  {
    MercenaryVillageTriggerDbfAsset asset = this.assetBundleRequest.asset as MercenaryVillageTriggerDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MercenaryVillageTriggerDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMercenaryVillageTriggerDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MercenaryVillageTriggerDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
