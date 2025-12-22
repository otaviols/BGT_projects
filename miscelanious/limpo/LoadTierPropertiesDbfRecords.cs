using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadTierPropertiesDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<TierPropertiesDbfRecord> GetRecords()
  {
    TierPropertiesDbfAsset asset = this.assetBundleRequest.asset as TierPropertiesDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<TierPropertiesDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadTierPropertiesDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (TierPropertiesDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
