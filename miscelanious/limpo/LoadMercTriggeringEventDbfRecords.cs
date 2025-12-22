using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMercTriggeringEventDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MercTriggeringEventDbfRecord> GetRecords()
  {
    MercTriggeringEventDbfAsset asset = this.assetBundleRequest.asset as MercTriggeringEventDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MercTriggeringEventDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMercTriggeringEventDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MercTriggeringEventDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
