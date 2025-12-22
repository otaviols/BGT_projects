using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadRepeatableTaskListDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<RepeatableTaskListDbfRecord> GetRecords()
  {
    RepeatableTaskListDbfAsset asset = this.assetBundleRequest.asset as RepeatableTaskListDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<RepeatableTaskListDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadRepeatableTaskListDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (RepeatableTaskListDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
