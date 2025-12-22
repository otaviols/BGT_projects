using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAdventureMissionDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AdventureMissionDbfRecord> GetRecords()
  {
    AdventureMissionDbfAsset asset = this.assetBundleRequest.asset as AdventureMissionDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AdventureMissionDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAdventureMissionDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AdventureMissionDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
