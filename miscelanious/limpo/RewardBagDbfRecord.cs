using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardBagDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_bagId;
  [SerializeField]
  private RewardBag.Reward m_reward = RewardBag.ParseRewardValue("unknown");
  [SerializeField]
  private int m_base;
  [SerializeField]
  private int m_rewardData;

  [DbfField("BAG_ID")]
  public int BagId => this.m_bagId;

  [DbfField("REWARD")]
  public RewardBag.Reward Reward => this.m_reward;

  [DbfField("BASE")]
  public int Base => this.m_base;

  [DbfField("REWARD_DATA")]
  public int RewardData => this.m_rewardData;

  public override object GetVar(string name)
  {
    if (name == "BAG_ID")
      return (object) this.m_bagId;
    if (name == "REWARD")
      return (object) this.m_reward;
    if (name == "BASE")
      return (object) this.m_base;
    return name == "REWARD_DATA" ? (object) this.m_rewardData : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "BAG_ID"))
    {
      if (!(name == "REWARD"))
      {
        if (!(name == "BASE"))
        {
          if (!(name == "REWARD_DATA"))
            return;
          this.m_rewardData = (int) val;
        }
        else
          this.m_base = (int) val;
      }
      else
      {
        switch (val)
        {
          case null:
            this.m_reward = RewardBag.Reward.NONE;
            break;
          case RewardBag.Reward _:
          case int _:
            this.m_reward = (RewardBag.Reward) val;
            break;
          case string _:
            this.m_reward = RewardBag.ParseRewardValue((string) val);
            break;
        }
      }
    }
    else
      this.m_bagId = (int) val;
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "BAG_ID")
      return typeof (int);
    if (name == "REWARD")
      return typeof (RewardBag.Reward);
    if (name == "BASE")
      return typeof (int);
    return name == "REWARD_DATA" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadRewardBagDbfRecords loadRecords = new LoadRewardBagDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    RewardBagDbfAsset rewardBagDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (RewardBagDbfAsset)) as RewardBagDbfAsset;
    if ((UnityEngine.Object) rewardBagDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("RewardBagDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < rewardBagDbfAsset.Records.Count; ++index)
      rewardBagDbfAsset.Records[index].StripUnusedLocales();
    records = rewardBagDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
