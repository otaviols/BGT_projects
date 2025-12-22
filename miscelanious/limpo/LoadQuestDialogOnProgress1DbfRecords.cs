using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuestDialogOnProgress1DbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<QuestDialogOnProgress1DbfRecord> GetRecords()
  {
    QuestDialogOnProgress1DbfAsset asset = this.assetBundleRequest.asset as QuestDialogOnProgress1DbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<QuestDialogOnProgress1DbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadQuestDialogOnProgress1DbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (QuestDialogOnProgress1DbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
