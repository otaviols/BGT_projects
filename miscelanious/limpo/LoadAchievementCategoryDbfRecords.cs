using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAchievementCategoryDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AchievementCategoryDbfRecord> GetRecords()
  {
    AchievementCategoryDbfAsset asset = this.assetBundleRequest.asset as AchievementCategoryDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AchievementCategoryDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAchievementCategoryDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AchievementCategoryDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
