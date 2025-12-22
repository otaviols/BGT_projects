using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuestDialogOnProgress2DbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<QuestDialogOnProgress2DbfRecord> GetRecords()
  {
    QuestDialogOnProgress2DbfAsset asset = this.assetBundleRequest.asset as QuestDialogOnProgress2DbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<QuestDialogOnProgress2DbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadQuestDialogOnProgress2DbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (QuestDialogOnProgress2DbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
