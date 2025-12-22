using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FixedRewardActionDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private FixedRewardAction.Type m_type = FixedRewardAction.ParseTypeValue("wing_progress");
  [SerializeField]
  private int m_wingId;
  [SerializeField]
  private int m_wingProgress;
  [SerializeField]
  private ulong m_wingFlags;
  [SerializeField]
  private int m_classId;
  [SerializeField]
  private int m_totalHeroLevel;
  [SerializeField]
  private int m_heroLevel;
  [SerializeField]
  private ulong m_metaActionFlags;
  [SerializeField]
  private int m_achieveId;
  [SerializeField]
  private long m_accountLicenseId;
  [SerializeField]
  private ulong m_accountLicenseFlags;
  [SerializeField]
  private SpecialEventType m_activeEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("always");
  [SerializeField]
  private int m_cardId;

  [DbfField("TYPE")]
  public FixedRewardAction.Type Type => this.m_type;

  [DbfField("WING_ID")]
  public int WingId => this.m_wingId;

  [DbfField("WING_PROGRESS")]
  public int WingProgress => this.m_wingProgress;

  [DbfField("WING_FLAGS")]
  public ulong WingFlags => this.m_wingFlags;

  [DbfField("CLASS_ID")]
  public int ClassId => this.m_classId;

  [DbfField("TOTAL_HERO_LEVEL")]
  public int TotalHeroLevel => this.m_totalHeroLevel;

  [DbfField("HERO_LEVEL")]
  public int HeroLevel => this.m_heroLevel;

  [DbfField("META_ACTION_FLAGS")]
  public ulong MetaActionFlags => this.m_metaActionFlags;

  [DbfField("ACHIEVE_ID")]
  public int AchieveId => this.m_achieveId;

  [DbfField("ACCOUNT_LICENSE_ID")]
  public long AccountLicenseId => this.m_accountLicenseId;

  [DbfField("ACCOUNT_LICENSE_FLAGS")]
  public ulong AccountLicenseFlags => this.m_accountLicenseFlags;

  [DbfField("ACTIVE_EVENT")]
  public SpecialEventType ActiveEvent => this.m_activeEvent;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ACCOUNT_LICENSE_FLAGS":
        return (object) this.m_accountLicenseFlags;
      case "ACCOUNT_LICENSE_ID":
        return (object) this.m_accountLicenseId;
      case "ACHIEVE_ID":
        return (object) this.m_achieveId;
      case "ACTIVE_EVENT":
        return (object) this.m_activeEvent;
      case "CARD_ID":
        return (object) this.m_cardId;
      case "CLASS_ID":
        return (object) this.m_classId;
      case "HERO_LEVEL":
        return (object) this.m_heroLevel;
      case "ID":
        return (object) this.ID;
      case "META_ACTION_FLAGS":
        return (object) this.m_metaActionFlags;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "TOTAL_HERO_LEVEL":
        return (object) this.m_totalHeroLevel;
      case "TYPE":
        return (object) this.m_type;
      case "WING_FLAGS":
        return (object) this.m_wingFlags;
      case "WING_ID":
        return (object) this.m_wingId;
      case "WING_PROGRESS":
        return (object) this.m_wingProgress;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 338683789:
        if (!(name == "TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_type = FixedRewardAction.Type.WING_PROGRESS;
            return;
          case FixedRewardAction.Type _:
          case int _:
            this.m_type = (FixedRewardAction.Type) val;
            return;
          case string _:
            this.m_type = FixedRewardAction.ParseTypeValue((string) val);
            return;
          default:
            return;
        }
      case 451390141:
        if (!(name == "CARD_ID"))
          break;
        this.m_cardId = (int) val;
        break;
      case 655212868:
        if (!(name == "ACHIEVE_ID"))
          break;
        this.m_achieveId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1559555090:
        if (!(name == "WING_ID"))
          break;
        this.m_wingId = (int) val;
        break;
      case 2102410176:
        if (!(name == "HERO_LEVEL"))
          break;
        this.m_heroLevel = (int) val;
        break;
      case 2658990091:
        if (!(name == "TOTAL_HERO_LEVEL"))
          break;
        this.m_totalHeroLevel = (int) val;
        break;
      case 2695008864:
        if (!(name == "WING_PROGRESS"))
          break;
        this.m_wingProgress = (int) val;
        break;
      case 2832947347:
        if (!(name == "META_ACTION_FLAGS"))
          break;
        this.m_metaActionFlags = (ulong) val;
        break;
      case 2947115096:
        if (!(name == "ACTIVE_EVENT"))
          break;
        this.m_activeEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3365816664:
        if (!(name == "ACCOUNT_LICENSE_ID"))
          break;
        this.m_accountLicenseId = (long) val;
        break;
      case 3553888982:
        if (!(name == "WING_FLAGS"))
          break;
        this.m_wingFlags = (ulong) val;
        break;
      case 3756307540:
        if (!(name == "ACCOUNT_LICENSE_FLAGS"))
          break;
        this.m_accountLicenseFlags = (ulong) val;
        break;
      case 4257872637:
        if (!(name == "CLASS_ID"))
          break;
        this.m_classId = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ACCOUNT_LICENSE_FLAGS":
        return typeof (ulong);
      case "ACCOUNT_LICENSE_ID":
        return typeof (long);
      case "ACHIEVE_ID":
        return typeof (int);
      case "ACTIVE_EVENT":
        return typeof (string);
      case "CARD_ID":
        return typeof (int);
      case "CLASS_ID":
        return typeof (int);
      case "HERO_LEVEL":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "META_ACTION_FLAGS":
        return typeof (ulong);
      case "NOTE_DESC":
        return typeof (string);
      case "TOTAL_HERO_LEVEL":
        return typeof (int);
      case "TYPE":
        return typeof (FixedRewardAction.Type);
      case "WING_FLAGS":
        return typeof (ulong);
      case "WING_ID":
        return typeof (int);
      case "WING_PROGRESS":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadFixedRewardActionDbfRecords loadRecords = new LoadFixedRewardActionDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    FixedRewardActionDbfAsset rewardActionDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (FixedRewardActionDbfAsset)) as FixedRewardActionDbfAsset;
    if ((UnityEngine.Object) rewardActionDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("FixedRewardActionDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < rewardActionDbfAsset.Records.Count; ++index)
      rewardActionDbfAsset.Records[index].StripUnusedLocales();
    records = rewardActionDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
