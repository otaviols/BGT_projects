using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadScenarioGuestHeroesDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ScenarioGuestHeroesDbfRecord> GetRecords()
  {
    ScenarioGuestHeroesDbfAsset asset = this.assetBundleRequest.asset as ScenarioGuestHeroesDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ScenarioGuestHeroesDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadScenarioGuestHeroesDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ScenarioGuestHeroesDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
