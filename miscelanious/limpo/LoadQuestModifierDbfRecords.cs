using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuestModifierDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<QuestModifierDbfRecord> GetRecords()
  {
    QuestModifierDbfAsset asset = this.assetBundleRequest.asset as QuestModifierDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<QuestModifierDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadQuestModifierDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (QuestModifierDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
