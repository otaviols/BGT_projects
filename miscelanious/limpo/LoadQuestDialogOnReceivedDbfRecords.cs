using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuestDialogOnReceivedDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<QuestDialogOnReceivedDbfRecord> GetRecords()
  {
    QuestDialogOnReceivedDbfAsset asset = this.assetBundleRequest.asset as QuestDialogOnReceivedDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<QuestDialogOnReceivedDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadQuestDialogOnReceivedDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (QuestDialogOnReceivedDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
