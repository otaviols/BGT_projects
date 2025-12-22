using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadLettuceMapNodeTypeAnomalyDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<LettuceMapNodeTypeAnomalyDbfRecord> GetRecords()
  {
    LettuceMapNodeTypeAnomalyDbfAsset asset = this.assetBundleRequest.asset as LettuceMapNodeTypeAnomalyDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<LettuceMapNodeTypeAnomalyDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadLettuceMapNodeTypeAnomalyDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (LettuceMapNodeTypeAnomalyDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
