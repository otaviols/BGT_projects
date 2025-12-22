using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadScenarioDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ScenarioDbfRecord> GetRecords()
  {
    ScenarioDbfAsset asset = this.assetBundleRequest.asset as ScenarioDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ScenarioDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadScenarioDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ScenarioDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
