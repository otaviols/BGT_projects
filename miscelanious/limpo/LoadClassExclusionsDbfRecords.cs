using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadClassExclusionsDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ClassExclusionsDbfRecord> GetRecords()
  {
    ClassExclusionsDbfAsset asset = this.assetBundleRequest.asset as ClassExclusionsDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ClassExclusionsDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadClassExclusionsDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ClassExclusionsDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
