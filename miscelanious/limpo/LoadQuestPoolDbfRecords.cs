using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuestPoolDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<QuestPoolDbfRecord> GetRecords()
  {
    QuestPoolDbfAsset asset = this.assetBundleRequest.asset as QuestPoolDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<QuestPoolDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadQuestPoolDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (QuestPoolDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
