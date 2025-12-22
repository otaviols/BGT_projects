using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadPowerDefinitionDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<PowerDefinitionDbfRecord> GetRecords()
  {
    PowerDefinitionDbfAsset asset = this.assetBundleRequest.asset as PowerDefinitionDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<PowerDefinitionDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadPowerDefinitionDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (PowerDefinitionDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
