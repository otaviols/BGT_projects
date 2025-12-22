using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadScoreLabelDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ScoreLabelDbfRecord> GetRecords()
  {
    ScoreLabelDbfAsset asset = this.assetBundleRequest.asset as ScoreLabelDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ScoreLabelDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadScoreLabelDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ScoreLabelDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
