using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardAdditonalSearchTermsDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardAdditonalSearchTermsDbfRecord> GetRecords()
  {
    CardAdditonalSearchTermsDbfAsset asset = this.assetBundleRequest.asset as CardAdditonalSearchTermsDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardAdditonalSearchTermsDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardAdditonalSearchTermsDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardAdditonalSearchTermsDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
