using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLoginPopupSequencePopupDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LoginPopupSequencePopupDbfRecord> GetRecords()
  {
    LoginPopupSequencePopupDbfAsset asset = this.assetBundleRequest.asset as LoginPopupSequencePopupDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LoginPopupSequencePopupDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLoginPopupSequencePopupDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LoginPopupSequencePopupDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
