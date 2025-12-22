using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadTriggerDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<TriggerDbfRecord> GetRecords()
  {
    TriggerDbfAsset asset = this.assetBundleRequest.asset as TriggerDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<TriggerDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadTriggerDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (TriggerDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
