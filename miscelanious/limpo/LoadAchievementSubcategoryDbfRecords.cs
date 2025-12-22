using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAchievementSubcategoryDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AchievementSubcategoryDbfRecord> GetRecords()
  {
    AchievementSubcategoryDbfAsset asset = this.assetBundleRequest.asset as AchievementSubcategoryDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AchievementSubcategoryDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAchievementSubcategoryDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AchievementSubcategoryDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
