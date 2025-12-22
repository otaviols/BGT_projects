using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadGuestHeroDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<GuestHeroDbfRecord> GetRecords()
  {
    GuestHeroDbfAsset asset = this.assetBundleRequest.asset as GuestHeroDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<GuestHeroDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadGuestHeroDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (GuestHeroDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
