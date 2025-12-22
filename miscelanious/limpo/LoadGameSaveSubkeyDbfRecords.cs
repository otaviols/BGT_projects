using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameSaveSubkeyDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<GameSaveSubkeyDbfRecord> GetRecords()
  {
    GameSaveSubkeyDbfAsset asset = this.assetBundleRequest.asset as GameSaveSubkeyDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<GameSaveSubkeyDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadGameSaveSubkeyDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (GameSaveSubkeyDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
