using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MercenariesRandomRewardDbfRecord : DbfRecord
{
  [SerializeField]
  private MercenariesRandomReward.RewardType m_rewardType;
  [SerializeField]
  private bool m_restrictRarity;
  [SerializeField]
  private int m_rarityId;
  [SerializeField]
  private MercenariesRandomReward.MercenariesPremium m_premium;

  [DbfField("REWARD_TYPE")]
  public MercenariesRandomReward.RewardType RewardType => this.m_rewardType;

  [DbfField("RESTRICT_RARITY")]
  public bool RestrictRarity => this.m_restrictRarity;

  [DbfField("RARITY")]
  public int Rarity => this.m_rarityId;

  [DbfField("PREMIUM")]
  public MercenariesRandomReward.MercenariesPremium Premium => this.m_premium;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "REWARD_TYPE")
      return (object) this.m_rewardType;
    if (name == "RESTRICT_RARITY")
      return (object) this.m_restrictRarity;
    if (name == "RARITY")
      return (object) this.m_rarityId;
    return name == "PREMIUM" ? (object) this.m_premium : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "REWARD_TYPE"))
      {
        if (!(name == "RESTRICT_RARITY"))
        {
          if (!(name == "RARITY"))
          {
            if (!(name == "PREMIUM"))
              return;
            switch (val)
            {
              case null:
                this.m_premium = MercenariesRandomReward.MercenariesPremium.PREMIUM_NORMAL;
                break;
              case MercenariesRandomReward.MercenariesPremium _:
              case int _:
                this.m_premium = (MercenariesRandomReward.MercenariesPremium) val;
                break;
              case string _:
                this.m_premium = MercenariesRandomReward.ParseMercenariesPremiumValue((string) val);
                break;
            }
          }
          else
            this.m_rarityId = (int) val;
        }
        else
          this.m_restrictRarity = (bool) val;
      }
      else
      {
        switch (val)
        {
          case null:
            this.m_rewardType = MercenariesRandomReward.RewardType.REWARD_TYPE_MERCENARY;
            break;
          case MercenariesRandomReward.RewardType _:
          case int _:
            this.m_rewardType = (MercenariesRandomReward.RewardType) val;
            break;
          case string _:
            this.m_rewardType = MercenariesRandomReward.ParseRewardTypeValue((string) val);
            break;
        }
      }
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "REWARD_TYPE")
      return typeof (MercenariesRandomReward.RewardType);
    if (name == "RESTRICT_RARITY")
      return typeof (bool);
    if (name == "RARITY")
      return typeof (int);
    return name == "PREMIUM" ? typeof (MercenariesRandomReward.MercenariesPremium) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadMercenariesRandomRewardDbfRecords loadRecords = new LoadMercenariesRandomRewardDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    MercenariesRandomRewardDbfAsset randomRewardDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (MercenariesRandomRewardDbfAsset)) as MercenariesRandomRewardDbfAsset;
    if ((UnityEngine.Object) randomRewardDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("MercenariesRandomRewardDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < randomRewardDbfAsset.Records.Count; ++index)
      randomRewardDbfAsset.Records[index].StripUnusedLocales();
    records = randomRewardDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
