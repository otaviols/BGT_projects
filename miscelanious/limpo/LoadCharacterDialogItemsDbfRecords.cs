using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacterDialogItemsDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CharacterDialogItemsDbfRecord> GetRecords()
  {
    CharacterDialogItemsDbfAsset asset = this.assetBundleRequest.asset as CharacterDialogItemsDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CharacterDialogItemsDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCharacterDialogItemsDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CharacterDialogItemsDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
