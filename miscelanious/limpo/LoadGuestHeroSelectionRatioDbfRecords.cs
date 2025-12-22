using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadGuestHeroSelectionRatioDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<GuestHeroSelectionRatioDbfRecord> GetRecords()
  {
    GuestHeroSelectionRatioDbfAsset asset = this.assetBundleRequest.asset as GuestHeroSelectionRatioDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<GuestHeroSelectionRatioDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadGuestHeroSelectionRatioDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (GuestHeroSelectionRatioDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
