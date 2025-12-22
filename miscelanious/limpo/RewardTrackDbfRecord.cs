using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RewardTrackDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_season;
  [SerializeField]
  private SpecialEventType m_event = SpecialEventType.UNKNOWN;
  [SerializeField]
  private int m_accountLicenseId;
  [SerializeField]
  private RewardTrack.RewardTrackType m_rewardTrackType;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_shortDescription;
  [SerializeField]
  private DbfLocValue m_longDescription;
  [SerializeField]
  private DbfLocValue m_shortConclusion;
  [SerializeField]
  private DbfLocValue m_longConclusion;
  [SerializeField]
  private int m_levelCapSoft;

  [DbfField("SEASON")]
  public int Season => this.m_season;

  [DbfField("EVENT")]
  public SpecialEventType Event => this.m_event;

  public AccountLicenseDbfRecord AccountLicenseRecord => GameDbf.AccountLicense.GetRecord(this.m_accountLicenseId);

  [DbfField("REWARD_TRACK_TYPE")]
  public RewardTrack.RewardTrackType RewardTrackType => this.m_rewardTrackType;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("SHORT_DESCRIPTION")]
  public DbfLocValue ShortDescription => this.m_shortDescription;

  [DbfField("LONG_DESCRIPTION")]
  public DbfLocValue LongDescription => this.m_longDescription;

  [DbfField("SHORT_CONCLUSION")]
  public DbfLocValue ShortConclusion => this.m_shortConclusion;

  [DbfField("LONG_CONCLUSION")]
  public DbfLocValue LongConclusion => this.m_longConclusion;

  [DbfField("LEVEL_CAP_SOFT")]
  public int LevelCapSoft => this.m_levelCapSoft;

  public List<RewardTrackLevelDbfRecord> Levels
  {
    get
    {
      int id = this.ID;
      List<RewardTrackLevelDbfRecord> levels = new List<RewardTrackLevelDbfRecord>();
      List<RewardTrackLevelDbfRecord> records = GameDbf.RewardTrackLevel.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        RewardTrackLevelDbfRecord trackLevelDbfRecord = records[index];
        if (trackLevelDbfRecord.RewardTrackId == id)
          levels.Add(trackLevelDbfRecord);
      }
      return levels;
    }
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ACCOUNT_LICENSE_ID":
        return (object) this.m_accountLicenseId;
      case "EVENT":
        return (object) this.m_event;
      case "ID":
        return (object) this.ID;
      case "LEVEL_CAP_SOFT":
        return (object) this.m_levelCapSoft;
      case "LONG_CONCLUSION":
        return (object) this.m_longConclusion;
      case "LONG_DESCRIPTION":
        return (object) this.m_longDescription;
      case "NAME":
        return (object) this.m_name;
      case "REWARD_TRACK_TYPE":
        return (object) this.m_rewardTrackType;
      case "SEASON":
        return (object) this.m_season;
      case "SHORT_CONCLUSION":
        return (object) this.m_shortConclusion;
      case "SHORT_DESCRIPTION":
        return (object) this.m_shortDescription;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 236776447:
        if (!(name == "EVENT"))
          break;
        this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 1351746555:
        if (!(name == "REWARD_TRACK_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_rewardTrackType = RewardTrack.RewardTrackType.NONE;
            return;
          case RewardTrack.RewardTrackType _:
          case int _:
            this.m_rewardTrackType = (RewardTrack.RewardTrackType) val;
            return;
          case string _:
            this.m_rewardTrackType = RewardTrack.ParseRewardTrackTypeValue((string) val);
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
      case 1803124559:
        if (!(name == "LEVEL_CAP_SOFT"))
          break;
        this.m_levelCapSoft = (int) val;
        break;
      case 2208190360:
        if (!(name == "SEASON"))
          break;
        this.m_season = (int) val;
        break;
      case 2418820992:
        if (!(name == "SHORT_DESCRIPTION"))
          break;
        this.m_shortDescription = (DbfLocValue) val;
        break;
      case 2790822542:
        if (!(name == "LONG_DESCRIPTION"))
          break;
        this.m_longDescription = (DbfLocValue) val;
        break;
      case 3113897185:
        if (!(name == "SHORT_CONCLUSION"))
          break;
        this.m_shortConclusion = (DbfLocValue) val;
        break;
      case 3365816664:
        if (!(name == "ACCOUNT_LICENSE_ID"))
          break;
        this.m_accountLicenseId = (int) val;
        break;
      case 3861165555:
        if (!(name == "LONG_CONCLUSION"))
          break;
        this.m_longConclusion = (DbfLocValue) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ACCOUNT_LICENSE_ID":
        return typeof (int);
      case "EVENT":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "LEVEL_CAP_SOFT":
        return typeof (int);
      case "LONG_CONCLUSION":
        return typeof (DbfLocValue);
      case "LONG_DESCRIPTION":
        return typeof (DbfLocValue);
      case "NAME":
        return typeof (DbfLocValue);
      case "REWARD_TRACK_TYPE":
        return typeof (RewardTrack.RewardTrackType);
      case "SEASON":
        return typeof (int);
      case "SHORT_CONCLUSION":
        return typeof (DbfLocValue);
      case "SHORT_DESCRIPTION":
        return typeof (DbfLocValue);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadRewardTrackDbfRecords loadRecords = new LoadRewardTrackDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    RewardTrackDbfAsset rewardTrackDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (RewardTrackDbfAsset)) as RewardTrackDbfAsset;
    if ((UnityEngine.Object) rewardTrackDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("RewardTrackDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < rewardTrackDbfAsset.Records.Count; ++index)
      rewardTrackDbfAsset.Records[index].StripUnusedLocales();
    records = rewardTrackDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_shortDescription.StripUnusedLocales();
    this.m_longDescription.StripUnusedLocales();
    this.m_shortConclusion.StripUnusedLocales();
    this.m_longConclusion.StripUnusedLocales();
  }
}
