using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceMapNodeTypeAnomalyDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_lettuceMapNodeTypeId;
  [SerializeField]
  private int m_anomalyCardId;
  [SerializeField]
  private LettuceMapNodeTypeAnomaly.MercenariesBonusRewardType m_bonusRewardType;

  [DbfField("ANOMALY_CARD")]
  public int AnomalyCard => this.m_anomalyCardId;

  [DbfField("BONUS_REWARD_TYPE")]
  public LettuceMapNodeTypeAnomaly.MercenariesBonusRewardType BonusRewardType => this.m_bonusRewardType;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "LETTUCE_MAP_NODE_TYPE_ID")
      return (object) this.m_lettuceMapNodeTypeId;
    if (name == "ANOMALY_CARD")
      return (object) this.m_anomalyCardId;
    return name == "BONUS_REWARD_TYPE" ? (object) this.m_bonusRewardType : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "LETTUCE_MAP_NODE_TYPE_ID"))
      {
        if (!(name == "ANOMALY_CARD"))
        {
          if (!(name == "BONUS_REWARD_TYPE"))
            return;
          switch (val)
          {
            case null:
              this.m_bonusRewardType = LettuceMapNodeTypeAnomaly.MercenariesBonusRewardType.NONE;
              break;
            case LettuceMapNodeTypeAnomaly.MercenariesBonusRewardType _:
            case int _:
              this.m_bonusRewardType = (LettuceMapNodeTypeAnomaly.MercenariesBonusRewardType) val;
              break;
            case string _:
              this.m_bonusRewardType = LettuceMapNodeTypeAnomaly.ParseMercenariesBonusRewardTypeValue((string) val);
              break;
          }
        }
        else
          this.m_anomalyCardId = (int) val;
      }
      else
        this.m_lettuceMapNodeTypeId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "LETTUCE_MAP_NODE_TYPE_ID")
      return typeof (int);
    if (name == "ANOMALY_CARD")
      return typeof (int);
    return name == "BONUS_REWARD_TYPE" ? typeof (LettuceMapNodeTypeAnomaly.MercenariesBonusRewardType) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceMapNodeTypeAnomalyDbfRecords loadRecords = new LoadLettuceMapNodeTypeAnomalyDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceMapNodeTypeAnomalyDbfAsset typeAnomalyDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceMapNodeTypeAnomalyDbfAsset)) as LettuceMapNodeTypeAnomalyDbfAsset;
    if ((UnityEngine.Object) typeAnomalyDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceMapNodeTypeAnomalyDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < typeAnomalyDbfAsset.Records.Count; ++index)
      typeAnomalyDbfAsset.Records[index].StripUnusedLocales();
    records = typeAnomalyDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
