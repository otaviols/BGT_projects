using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadMercenaryVisitorDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<MercenaryVisitorDbfRecord> GetRecords()
  {
    MercenaryVisitorDbfAsset asset = this.assetBundleRequest.asset as MercenaryVisitorDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<MercenaryVisitorDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadMercenaryVisitorDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (MercenaryVisitorDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
