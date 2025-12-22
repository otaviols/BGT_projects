using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestDbfRecord : DbfRecord
{
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private string m_icon;
  [SerializeField]
  private int m_quota;
  [SerializeField]
  private SpecialEventType m_event = SpecialEventType.UNKNOWN;
  [SerializeField]
  private int m_nextInChainId;
  [SerializeField]
  private bool m_canAbandon;
  [SerializeField]
  private string m_deepLink;
  [SerializeField]
  private int m_questPoolId;
  [SerializeField]
  private bool m_poolGuaranteed;
  [SerializeField]
  private int m_poolInstantGrantDay;
  [SerializeField]
  private int m_rewardTrackXp;
  [SerializeField]
  private int m_rewardListId;
  [SerializeField]
  private Quest.RewardTrackType m_rewardTrackType;
  [SerializeField]
  private int m_proxyForLegacyId;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("ICON")]
  public string Icon => this.m_icon;

  [DbfField("QUOTA")]
  public int Quota => this.m_quota;

  [DbfField("EVENT")]
  public SpecialEventType Event => this.m_event;

  [DbfField("NEXT_IN_CHAIN")]
  public int NextInChain => this.m_nextInChainId;

  [DbfField("CAN_ABANDON")]
  public bool CanAbandon => this.m_canAbandon;

  [DbfField("DEEP_LINK")]
  public string DeepLink => this.m_deepLink;

  [DbfField("QUEST_POOL")]
  public int QuestPool => this.m_questPoolId;

  public QuestPoolDbfRecord QuestPoolRecord => GameDbf.QuestPool.GetRecord(this.m_questPoolId);

  [DbfField("REWARD_TRACK_XP")]
  public int RewardTrackXp => this.m_rewardTrackXp;

  [DbfField("REWARD_LIST")]
  public int RewardList => this.m_rewardListId;

  [DbfField("REWARD_TRACK_TYPE")]
  public Quest.RewardTrackType RewardTrackType => this.m_rewardTrackType;

  [DbfField("PROXY_FOR_LEGACY_ID")]
  public int ProxyForLegacyId => this.m_proxyForLegacyId;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "CAN_ABANDON":
        return (object) this.m_canAbandon;
      case "DEEP_LINK":
        return (object) this.m_deepLink;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "EVENT":
        return (object) this.m_event;
      case "ICON":
        return (object) this.m_icon;
      case "ID":
        return (object) this.ID;
      case "NAME":
        return (object) this.m_name;
      case "NEXT_IN_CHAIN":
        return (object) this.m_nextInChainId;
      case "POOL_GUARANTEED":
        return (object) this.m_poolGuaranteed;
      case "POOL_INSTANT_GRANT_DAY":
        return (object) this.m_poolInstantGrantDay;
      case "PROXY_FOR_LEGACY_ID":
        return (object) this.m_proxyForLegacyId;
      case "QUEST_POOL":
        return (object) this.m_questPoolId;
      case "QUOTA":
        return (object) this.m_quota;
      case "REWARD_LIST":
        return (object) this.m_rewardListId;
      case "REWARD_TRACK_TYPE":
        return (object) this.m_rewardTrackType;
      case "REWARD_TRACK_XP":
        return (object) this.m_rewardTrackXp;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 74569132:
        if (!(name == "QUEST_POOL"))
          break;
        this.m_questPoolId = (int) val;
        break;
      case 91158759:
        if (!(name == "REWARD_LIST"))
          break;
        this.m_rewardListId = (int) val;
        break;
      case 231185982:
        if (!(name == "POOL_GUARANTEED"))
          break;
        this.m_poolGuaranteed = (bool) val;
        break;
      case 236776447:
        if (!(name == "EVENT"))
          break;
        this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 416172651:
        if (!(name == "QUOTA"))
          break;
        this.m_quota = (int) val;
        break;
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1138013160:
        if (!(name == "NEXT_IN_CHAIN"))
          break;
        this.m_nextInChainId = (int) val;
        break;
      case 1223790211:
        if (!(name == "REWARD_TRACK_XP"))
          break;
        this.m_rewardTrackXp = (int) val;
        break;
      case 1261696513:
        if (!(name == "PROXY_FOR_LEGACY_ID"))
          break;
        this.m_proxyForLegacyId = (int) val;
        break;
      case 1351746555:
        if (!(name == "REWARD_TRACK_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_rewardTrackType = Quest.RewardTrackType.NONE;
            return;
          case Quest.RewardTrackType _:
          case int _:
            this.m_rewardTrackType = (Quest.RewardTrackType) val;
            return;
          case string _:
            this.m_rewardTrackType = Quest.ParseRewardTrackTypeValue((string) val);
            return;
          default:
            return;
        }
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1783488128:
        if (!(name == "DEEP_LINK"))
          break;
        this.m_deepLink = (string) val;
        break;
      case 3650776359:
        if (!(name == "CAN_ABANDON"))
          break;
        this.m_canAbandon = (bool) val;
        break;
      case 3828435440:
        if (!(name == "ICON"))
          break;
        this.m_icon = (string) val;
        break;
      case 3991086193:
        if (!(name == "POOL_INSTANT_GRANT_DAY"))
          break;
        this.m_poolInstantGrantDay = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "CAN_ABANDON":
        return typeof (bool);
      case "DEEP_LINK":
        return typeof (string);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "EVENT":
        return typeof (string);
      case "ICON":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "NAME":
        return typeof (DbfLocValue);
      case "NEXT_IN_CHAIN":
        return typeof (int);
      case "POOL_GUARANTEED":
        return typeof (bool);
      case "POOL_INSTANT_GRANT_DAY":
        return typeof (int);
      case "PROXY_FOR_LEGACY_ID":
        return typeof (int);
      case "QUEST_POOL":
        return typeof (int);
      case "QUOTA":
        return typeof (int);
      case "REWARD_LIST":
        return typeof (int);
      case "REWARD_TRACK_TYPE":
        return typeof (Quest.RewardTrackType);
      case "REWARD_TRACK_XP":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadQuestDbfRecords loadRecords = new LoadQuestDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    QuestDbfAsset questDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (QuestDbfAsset)) as QuestDbfAsset;
    if ((UnityEngine.Object) questDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("QuestDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < questDbfAsset.Records.Count; ++index)
      questDbfAsset.Records[index].StripUnusedLocales();
    records = questDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
  }
}
