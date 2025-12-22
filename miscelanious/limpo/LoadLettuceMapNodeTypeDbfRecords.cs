using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceMapNodeTypeDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceMapNodeTypeDbfRecord> GetRecords()
  {
    LettuceMapNodeTypeDbfAsset asset = this.assetBundleRequest.asset as LettuceMapNodeTypeDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceMapNodeTypeDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceMapNodeTypeDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceMapNodeTypeDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
