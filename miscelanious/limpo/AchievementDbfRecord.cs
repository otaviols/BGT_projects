using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchievementDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_achievementSectionId;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private bool m_enabled = true;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private Assets.Achievement.AchievementVisibility m_achievementVisibility;
  [SerializeField]
  private int m_quota = 1;
  [SerializeField]
  private bool m_allowExceedQuota;
  [SerializeField]
  private int m_triggerId;
  [SerializeField]
  private int m_points;
  [SerializeField]
  private int m_rewardTrackXp;
  [SerializeField]
  private Assets.Achievement.RewardTrackType m_rewardTrackType;
  [SerializeField]
  private int m_rewardListId;
  [SerializeField]
  private int m_nextTierId;
  [SerializeField]
  private bool m_socialToast;

  [DbfField("ACHIEVEMENT_SECTION")]
  public int AchievementSection => this.m_achievementSectionId;

  public AchievementSectionDbfRecord AchievementSectionRecord => GameDbf.AchievementSection.GetRecord(this.m_achievementSectionId);

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("ENABLED")]
  public bool Enabled => this.m_enabled;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("ACHIEVEMENT_VISIBILITY")]
  public Assets.Achievement.AchievementVisibility AchievementVisibility => this.m_achievementVisibility;

  [DbfField("QUOTA")]
  public int Quota => this.m_quota;

  [DbfField("ALLOW_EXCEED_QUOTA")]
  public bool AllowExceedQuota => this.m_allowExceedQuota;

  [DbfField("POINTS")]
  public int Points => this.m_points;

  [DbfField("REWARD_TRACK_XP")]
  public int RewardTrackXp => this.m_rewardTrackXp;

  [DbfField("REWARD_TRACK_TYPE")]
  public Assets.Achievement.RewardTrackType RewardTrackType => this.m_rewardTrackType;

  [DbfField("REWARD_LIST")]
  public int RewardList => this.m_rewardListId;

  public RewardListDbfRecord RewardListRecord => GameDbf.RewardList.GetRecord(this.m_rewardListId);

  [DbfField("NEXT_TIER")]
  public int NextTier => this.m_nextTierId;

  [DbfField("SOCIAL_TOAST")]
  public bool SocialToast => this.m_socialToast;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ACHIEVEMENT_SECTION":
        return (object) this.m_achievementSectionId;
      case "ACHIEVEMENT_VISIBILITY":
        return (object) this.m_achievementVisibility;
      case "ALLOW_EXCEED_QUOTA":
        return (object) this.m_allowExceedQuota;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "ENABLED":
        return (object) this.m_enabled;
      case "ID":
        return (object) this.ID;
      case "NAME":
        return (object) this.m_name;
      case "NEXT_TIER":
        return (object) this.m_nextTierId;
      case "POINTS":
        return (object) this.m_points;
      case "QUOTA":
        return (object) this.m_quota;
      case "REWARD_LIST":
        return (object) this.m_rewardListId;
      case "REWARD_TRACK_TYPE":
        return (object) this.m_rewardTrackType;
      case "REWARD_TRACK_XP":
        return (object) this.m_rewardTrackXp;
      case "SOCIAL_TOAST":
        return (object) this.m_socialToast;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "TRIGGER":
        return (object) this.m_triggerId;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 91158759:
        if (!(name == "REWARD_LIST"))
          break;
        this.m_rewardListId = (int) val;
        break;
      case 257123909:
        if (!(name == "ACHIEVEMENT_VISIBILITY"))
          break;
        switch (val)
        {
          case null:
            this.m_achievementVisibility = Assets.Achievement.AchievementVisibility.VISIBLE;
            return;
          case Assets.Achievement.AchievementVisibility _:
          case int _:
            this.m_achievementVisibility = (Assets.Achievement.AchievementVisibility) val;
            return;
          case string _:
            this.m_achievementVisibility = Assets.Achievement.ParseAchievementVisibilityValue((string) val);
            return;
          default:
            return;
        }
      case 416172651:
        if (!(name == "QUOTA"))
          break;
        this.m_quota = (int) val;
        break;
      case 479864542:
        if (!(name == "SOCIAL_TOAST"))
          break;
        this.m_socialToast = (bool) val;
        break;
      case 937620916:
        if (!(name == "ALLOW_EXCEED_QUOTA"))
          break;
        this.m_allowExceedQuota = (bool) val;
        break;
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1223790211:
        if (!(name == "REWARD_TRACK_XP"))
          break;
        this.m_rewardTrackXp = (int) val;
        break;
      case 1351746555:
        if (!(name == "REWARD_TRACK_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_rewardTrackType = Assets.Achievement.RewardTrackType.NONE;
            return;
          case Assets.Achievement.RewardTrackType _:
          case int _:
            this.m_rewardTrackType = (Assets.Achievement.RewardTrackType) val;
            return;
          case string _:
            this.m_rewardTrackType = Assets.Achievement.ParseRewardTrackTypeValue((string) val);
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
      case 1752184140:
        if (!(name == "ACHIEVEMENT_SECTION"))
          break;
        this.m_achievementSectionId = (int) val;
        break;
      case 1777744857:
        if (!(name == "NEXT_TIER"))
          break;
        this.m_nextTierId = (int) val;
        break;
      case 1951464006:
        if (!(name == "POINTS"))
          break;
        this.m_points = (int) val;
        break;
      case 2294480894:
        if (!(name == "ENABLED"))
          break;
        this.m_enabled = (bool) val;
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
      case 4220586723:
        if (!(name == "TRIGGER"))
          break;
        this.m_triggerId = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ACHIEVEMENT_SECTION":
        return typeof (int);
      case "ACHIEVEMENT_VISIBILITY":
        return typeof (Assets.Achievement.AchievementVisibility);
      case "ALLOW_EXCEED_QUOTA":
        return typeof (bool);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "ENABLED":
        return typeof (bool);
      case "ID":
        return typeof (int);
      case "NAME":
        return typeof (DbfLocValue);
      case "NEXT_TIER":
        return typeof (int);
      case "POINTS":
        return typeof (int);
      case "QUOTA":
        return typeof (int);
      case "REWARD_LIST":
        return typeof (int);
      case "REWARD_TRACK_TYPE":
        return typeof (Assets.Achievement.RewardTrackType);
      case "REWARD_TRACK_XP":
        return typeof (int);
      case "SOCIAL_TOAST":
        return typeof (bool);
      case "SORT_ORDER":
        return typeof (int);
      case "TRIGGER":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAchievementDbfRecords loadRecords = new LoadAchievementDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AchievementDbfAsset achievementDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AchievementDbfAsset)) as AchievementDbfAsset;
    if ((UnityEngine.Object) achievementDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AchievementDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < achievementDbfAsset.Records.Count; ++index)
      achievementDbfAsset.Records[index].StripUnusedLocales();
    records = achievementDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
  }
}
