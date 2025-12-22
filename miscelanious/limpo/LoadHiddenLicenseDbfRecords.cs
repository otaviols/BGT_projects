using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadHiddenLicenseDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<HiddenLicenseDbfRecord> GetRecords()
  {
    HiddenLicenseDbfAsset asset = this.assetBundleRequest.asset as HiddenLicenseDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<HiddenLicenseDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadHiddenLicenseDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (HiddenLicenseDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
