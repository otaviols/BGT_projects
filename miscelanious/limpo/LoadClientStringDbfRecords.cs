using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadClientStringDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<ClientStringDbfRecord> GetRecords()
  {
    ClientStringDbfAsset asset = this.assetBundleRequest.asset as ClientStringDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<ClientStringDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadClientStringDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (ClientStringDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
