using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadDkRuneListDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<DkRuneListDbfRecord> GetRecords()
  {
    DkRuneListDbfAsset asset = this.assetBundleRequest.asset as DkRuneListDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<DkRuneListDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadDkRuneListDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (DkRuneListDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
