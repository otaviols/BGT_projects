using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MercenariesRankedSeasonRewardRankDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_minPublicRatingUnlock;
  [SerializeField]
  private int m_rewardListId;

  [DbfField("MIN_PUBLIC_RATING_UNLOCK")]
  public int MinPublicRatingUnlock => this.m_minPublicRatingUnlock;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "MIN_PUBLIC_RATING_UNLOCK")
      return (object) this.m_minPublicRatingUnlock;
    return name == "REWARD_LIST" ? (object) this.m_rewardListId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "MIN_PUBLIC_RATING_UNLOCK"))
      {
        if (!(name == "REWARD_LIST"))
          return;
        this.m_rewardListId = (int) val;
      }
      else
        this.m_minPublicRatingUnlock = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "MIN_PUBLIC_RATING_UNLOCK")
      return typeof (int);
    return name == "REWARD_LIST" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadMercenariesRankedSeasonRewardRankDbfRecords loadRecords = new LoadMercenariesRankedSeasonRewardRankDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    MercenariesRankedSeasonRewardRankDbfAsset rewardRankDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (MercenariesRankedSeasonRewardRankDbfAsset)) as MercenariesRankedSeasonRewardRankDbfAsset;
    if ((UnityEngine.Object) rewardRankDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("MercenariesRankedSeasonRewardRankDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < rewardRankDbfAsset.Records.Count; ++index)
      rewardRankDbfAsset.Records[index].StripUnusedLocales();
    records = rewardRankDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
