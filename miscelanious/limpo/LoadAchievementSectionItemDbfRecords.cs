using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAchievementSectionItemDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AchievementSectionItemDbfRecord> GetRecords()
  {
    AchievementSectionItemDbfAsset asset = this.assetBundleRequest.asset as AchievementSectionItemDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AchievementSectionItemDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAchievementSectionItemDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AchievementSectionItemDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
