using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLoginPopupSequenceDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LoginPopupSequenceDbfRecord> GetRecords()
  {
    LoginPopupSequenceDbfAsset asset = this.assetBundleRequest.asset as LoginPopupSequenceDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LoginPopupSequenceDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLoginPopupSequenceDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LoginPopupSequenceDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
