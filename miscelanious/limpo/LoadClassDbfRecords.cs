using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadClassDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ClassDbfRecord> GetRecords()
  {
    ClassDbfAsset asset = this.assetBundleRequest.asset as ClassDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ClassDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadClassDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ClassDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
