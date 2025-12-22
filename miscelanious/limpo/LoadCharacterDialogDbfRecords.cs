using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCharacterDialogDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CharacterDialogDbfRecord> GetRecords()
  {
    CharacterDialogDbfAsset asset = this.assetBundleRequest.asset as CharacterDialogDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CharacterDialogDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCharacterDialogDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CharacterDialogDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
