using Blizzard.T5.Jobs;
using System.Collections.Generic;
using UnityEngine;

public class LoadTavernBrawlTicketDbfRecords : IJobDependency, IAsyncJobResult
{
  private AssetBundleRequest assetBundleRequest;

  public List<TavernBrawlTicketDbfRecord> GetRecords()
  {
    TavernBrawlTicketDbfAsset asset = this.assetBundleRequest.asset as TavernBrawlTicketDbfAsset;
    if (!((Object) asset != (Object) null))
      return (List<TavernBrawlTicketDbfRecord>) null;
    for (int index = 0; index < asset.Records.Count; ++index)
      asset.Records[index].StripUnusedLocales();
    return asset.Records;
  }

  public LoadTavernBrawlTicketDbfRecords(string resourcePath) => this.assetBundleRequest = DbfShared.GetAssetBundle().LoadAssetAsync(resourcePath, typeof (TavernBrawlTicketDbfAsset));

  public bool IsReady() => this.assetBundleRequest.isDone;
}
