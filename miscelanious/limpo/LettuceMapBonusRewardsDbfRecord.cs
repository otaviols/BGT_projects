using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceMapBonusRewardsDbfRecord : DbfRecord
{
  [SerializeField]
  private LettuceMapBonusRewards.MercenariesBonusRewardType m_bonusRewardType;

  public override object GetVar(string name) => name == "BONUS_REWARD_TYPE" ? (object) this.m_bonusRewardType : (object) null;

  public override void SetVar(string name, object val)
  {
    if (!(name == "BONUS_REWARD_TYPE"))
      return;
    switch (val)
    {
      case null:
        this.m_bonusRewardType = LettuceMapBonusRewards.MercenariesBonusRewardType.NONE;
        break;
      case LettuceMapBonusRewards.MercenariesBonusRewardType _:
      case int _:
        this.m_bonusRewardType = (LettuceMapBonusRewards.MercenariesBonusRewardType) val;
        break;
      case string _:
        this.m_bonusRewardType = LettuceMapBonusRewards.ParseMercenariesBonusRewardTypeValue((string) val);
        break;
    }
  }

  public override System.Type GetVarType(string name) => name == "BONUS_REWARD_TYPE" ? typeof (LettuceMapBonusRewards.MercenariesBonusRewardType) : (System.Type) null;

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceMapBonusRewardsDbfRecords loadRecords = new LoadLettuceMapBonusRewardsDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceMapBonusRewardsDbfAsset bonusRewardsDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceMapBonusRewardsDbfAsset)) as LettuceMapBonusRewardsDbfAsset;
    if ((UnityEngine.Object) bonusRewardsDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceMapBonusRewardsDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < bonusRewardsDbfAsset.Records.Count; ++index)
      bonusRewardsDbfAsset.Records[index].StripUnusedLocales();
    records = bonusRewardsDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
