using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceTutorialVoDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceTutorialVoDbfRecord> GetRecords()
  {
    LettuceTutorialVoDbfAsset asset = this.assetBundleRequest.asset as LettuceTutorialVoDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceTutorialVoDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceTutorialVoDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceTutorialVoDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
