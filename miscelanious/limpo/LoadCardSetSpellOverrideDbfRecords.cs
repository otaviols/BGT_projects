using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadCardSetSpellOverrideDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<CardSetSpellOverrideDbfRecord> GetRecords()
  {
    CardSetSpellOverrideDbfAsset asset = this.assetBundleRequest.asset as CardSetSpellOverrideDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<CardSetSpellOverrideDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadCardSetSpellOverrideDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (CardSetSpellOverrideDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
