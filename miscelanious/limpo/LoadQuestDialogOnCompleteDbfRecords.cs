using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuestDialogOnCompleteDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<QuestDialogOnCompleteDbfRecord> GetRecords()
  {
    QuestDialogOnCompleteDbfAsset asset = this.assetBundleRequest.asset as QuestDialogOnCompleteDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<QuestDialogOnCompleteDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadQuestDialogOnCompleteDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (QuestDialogOnCompleteDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
