using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAchievementSectionDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AchievementSectionDbfRecord> GetRecords()
  {
    AchievementSectionDbfAsset asset = this.assetBundleRequest.asset as AchievementSectionDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AchievementSectionDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAchievementSectionDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AchievementSectionDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
