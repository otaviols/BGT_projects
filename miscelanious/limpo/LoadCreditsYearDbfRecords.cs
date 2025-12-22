using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCreditsYearDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CreditsYearDbfRecord> GetRecords()
  {
    CreditsYearDbfAsset asset = this.assetBundleRequest.asset as CreditsYearDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CreditsYearDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCreditsYearDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CreditsYearDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
