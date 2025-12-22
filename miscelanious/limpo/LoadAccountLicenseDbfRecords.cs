using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAccountLicenseDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AccountLicenseDbfRecord> GetRecords()
  {
    AccountLicenseDbfAsset asset = this.assetBundleRequest.asset as AccountLicenseDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AccountLicenseDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAccountLicenseDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AccountLicenseDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
