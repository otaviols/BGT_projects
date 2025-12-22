using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAdventureModeDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AdventureModeDbfRecord> GetRecords()
  {
    AdventureModeDbfAsset asset = this.assetBundleRequest.asset as AdventureModeDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AdventureModeDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAdventureModeDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AdventureModeDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
