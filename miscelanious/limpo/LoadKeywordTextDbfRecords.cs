using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadKeywordTextDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<KeywordTextDbfRecord> GetRecords()
  {
    KeywordTextDbfAsset asset = this.assetBundleRequest.asset as KeywordTextDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<KeywordTextDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadKeywordTextDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (KeywordTextDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
