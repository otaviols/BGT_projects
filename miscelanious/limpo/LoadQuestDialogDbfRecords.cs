using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadQuestDialogDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<QuestDialogDbfRecord> GetRecords()
  {
    QuestDialogDbfAsset asset = this.assetBundleRequest.asset as QuestDialogDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<QuestDialogDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadQuestDialogDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (QuestDialogDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
