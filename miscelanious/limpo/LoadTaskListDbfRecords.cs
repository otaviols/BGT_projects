using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadTaskListDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<TaskListDbfRecord> GetRecords()
  {
    TaskListDbfAsset asset = this.assetBundleRequest.asset as TaskListDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<TaskListDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadTaskListDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (TaskListDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
