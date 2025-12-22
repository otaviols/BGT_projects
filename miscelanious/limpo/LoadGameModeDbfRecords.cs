using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameModeDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<GameModeDbfRecord> GetRecords()
  {
    GameModeDbfAsset asset = this.assetBundleRequest.asset as GameModeDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<GameModeDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadGameModeDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (GameModeDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
