using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadAchieveConditionDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<AchieveConditionDbfRecord> GetRecords()
  {
    AchieveConditionDbfAsset asset = this.assetBundleRequest.asset as AchieveConditionDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<AchieveConditionDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadAchieveConditionDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (AchieveConditionDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
