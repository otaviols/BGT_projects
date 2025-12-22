using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuestDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<QuestDbfRecord> GetRecords()
  {
    QuestDbfAsset asset = this.assetBundleRequest.asset as QuestDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<QuestDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadQuestDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (QuestDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
