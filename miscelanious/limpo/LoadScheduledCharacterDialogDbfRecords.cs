using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadScheduledCharacterDialogDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ScheduledCharacterDialogDbfRecord> GetRecords()
  {
    ScheduledCharacterDialogDbfAsset asset = this.assetBundleRequest.asset as ScheduledCharacterDialogDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ScheduledCharacterDialogDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadScheduledCharacterDialogDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ScheduledCharacterDialogDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
