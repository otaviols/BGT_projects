using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAchievementDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AchievementDbfRecord> GetRecords()
  {
    AchievementDbfAsset asset = this.assetBundleRequest.asset as AchievementDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AchievementDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAchievementDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AchievementDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
