using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMercTriggeredEventDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MercTriggeredEventDbfRecord> GetRecords()
  {
    MercTriggeredEventDbfAsset asset = this.assetBundleRequest.asset as MercTriggeredEventDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MercTriggeredEventDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMercTriggeredEventDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MercTriggeredEventDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
