using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestPoolDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_grantDayOfWeek = -1;
  [SerializeField]
  private int m_grantHourOfDay;
  [SerializeField]
  private int m_numQuestsGranted = 1;
  [SerializeField]
  private int m_maxQuestsActive = 1;
  [SerializeField]
  private int m_maxQuestsBanked;
  [SerializeField]
  private int m_rerollCountMax;
  [SerializeField]
  private QuestPool.QuestPoolType m_questPoolType = QuestPool.QuestPoolType.DAILY;
  [SerializeField]
  private QuestPool.RewardTrackType m_rewardTrackType;

  [DbfField("NUM_QUESTS_GRANTED")]
  public int NumQuestsGranted => this.m_numQuestsGranted;

  [DbfField("MAX_QUESTS_ACTIVE")]
  public int MaxQuestsActive => this.m_maxQuestsActive;

  [DbfField("QUEST_POOL_TYPE")]
  public QuestPool.QuestPoolType QuestPoolType => this.m_questPoolType;

  [DbfField("REWARD_TRACK_TYPE")]
  public QuestPool.RewardTrackType RewardTrackType => this.m_rewardTrackType;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "GRANT_DAY_OF_WEEK":
        return (object) this.m_grantDayOfWeek;
      case "GRANT_HOUR_OF_DAY":
        return (object) this.m_grantHourOfDay;
      case "ID":
        return (object) this.ID;
      case "MAX_QUESTS_ACTIVE":
        return (object) this.m_maxQuestsActive;
      case "MAX_QUESTS_BANKED":
        return (object) this.m_maxQuestsBanked;
      case "NUM_QUESTS_GRANTED":
        return (object) this.m_numQuestsGranted;
      case "QUEST_POOL_TYPE":
        return (object) this.m_questPoolType;
      case "REROLL_COUNT_MAX":
        return (object) this.m_rerollCountMax;
      case "REWARD_TRACK_TYPE":
        return (object) this.m_rewardTrackType;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 295297505:
        if (!(name == "QUEST_POOL_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_questPoolType = QuestPool.QuestPoolType.NONE;
            return;
          case QuestPool.QuestPoolType _:
          case int _:
            this.m_questPoolType = (QuestPool.QuestPoolType) val;
            return;
          case string _:
            this.m_questPoolType = QuestPool.ParseQuestPoolTypeValue((string) val);
            return;
          default:
            return;
        }
      case 810012553:
        if (!(name == "GRANT_HOUR_OF_DAY"))
          break;
        this.m_grantHourOfDay = (int) val;
        break;
      case 1188156935:
        if (!(name == "NUM_QUESTS_GRANTED"))
          break;
        this.m_numQuestsGranted = (int) val;
        break;
      case 1351746555:
        if (!(name == "REWARD_TRACK_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_rewardTrackType = QuestPool.RewardTrackType.NONE;
            return;
          case QuestPool.RewardTrackType _:
          case int _:
            this.m_rewardTrackType = (QuestPool.RewardTrackType) val;
            return;
          case string _:
            this.m_rewardTrackType = QuestPool.ParseRewardTrackTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2197881361:
        if (!(name == "MAX_QUESTS_BANKED"))
          break;
        this.m_maxQuestsBanked = (int) val;
        break;
      case 2510957890:
        if (!(name == "REROLL_COUNT_MAX"))
          break;
        this.m_rerollCountMax = (int) val;
        break;
      case 3136795921:
        if (!(name == "GRANT_DAY_OF_WEEK"))
          break;
        this.m_grantDayOfWeek = (int) val;
        break;
      case 4002087504:
        if (!(name == "MAX_QUESTS_ACTIVE"))
          break;
        this.m_maxQuestsActive = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "GRANT_DAY_OF_WEEK":
        return typeof (int);
      case "GRANT_HOUR_OF_DAY":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "MAX_QUESTS_ACTIVE":
        return typeof (int);
      case "MAX_QUESTS_BANKED":
        return typeof (int);
      case "NUM_QUESTS_GRANTED":
        return typeof (int);
      case "QUEST_POOL_TYPE":
        return typeof (QuestPool.QuestPoolType);
      case "REROLL_COUNT_MAX":
        return typeof (int);
      case "REWARD_TRACK_TYPE":
        return typeof (QuestPool.RewardTrackType);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadQuestPoolDbfRecords loadRecords = new LoadQuestPoolDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    QuestPoolDbfAsset questPoolDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (QuestPoolDbfAsset)) as QuestPoolDbfAsset;
    if ((UnityEngine.Object) questPoolDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("QuestPoolDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < questPoolDbfAsset.Records.Count; ++index)
      questPoolDbfAsset.Records[index].StripUnusedLocales();
    records = questPoolDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
