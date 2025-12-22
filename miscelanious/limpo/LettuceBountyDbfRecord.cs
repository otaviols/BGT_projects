using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LettuceBountyDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_bountyLevel;
  [SerializeField]
  private bool m_enabled = true;
  [SerializeField]
  private SpecialEventType m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent("always");
  [SerializeField]
  private SpecialEventType m_availableAfterEvent = SpecialEventType.UNKNOWN;
  [SerializeField]
  private DbfLocValue m_bossNameOverride;
  [SerializeField]
  private DbfLocValue m_bountyNameOverride;
  [SerializeField]
  private DbfLocValue m_comingSoonText;
  [SerializeField]
  private int m_bountySetId;
  [SerializeField]
  private LettuceBounty.MercenariesBountyDifficulty m_difficultyMode = LettuceBounty.MercenariesBountyDifficulty.NORMAL;
  [SerializeField]
  private bool m_heroic;
  [SerializeField]
  private int m_finalBossCardId;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private int m_requiredCompletedBountyId;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  [DbfField("BOUNTY_LEVEL")]
  public int BountyLevel => this.m_bountyLevel;

  [DbfField("ENABLED")]
  public bool Enabled => this.m_enabled;

  [DbfField("EVENT")]
  public SpecialEventType Event => this.m_event;

  [DbfField("AVAILABLE_AFTER_EVENT")]
  public SpecialEventType AvailableAfterEvent => this.m_availableAfterEvent;

  [DbfField("BOSS_NAME_OVERRIDE")]
  public DbfLocValue BossNameOverride => this.m_bossNameOverride;

  [DbfField("BOUNTY_NAME_OVERRIDE")]
  public DbfLocValue BountyNameOverride => this.m_bountyNameOverride;

  [DbfField("COMING_SOON_TEXT")]
  public DbfLocValue ComingSoonText => this.m_comingSoonText;

  [DbfField("BOUNTY_SET_ID")]
  public int BountySetId => this.m_bountySetId;

  public LettuceBountySetDbfRecord BountySetRecord => GameDbf.LettuceBountySet.GetRecord(this.m_bountySetId);

  [DbfField("DIFFICULTY_MODE")]
  public LettuceBounty.MercenariesBountyDifficulty DifficultyMode => this.m_difficultyMode;

  [DbfField("HEROIC")]
  public bool Heroic => this.m_heroic;

  [DbfField("FINAL_BOSS_CARD_ID")]
  public int FinalBossCardId => this.m_finalBossCardId;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("REQUIRED_COMPLETED_BOUNTY")]
  public int RequiredCompletedBounty => this.m_requiredCompletedBountyId;

  public List<LettuceBountyFinalRewardsDbfRecord> FinalBossRewards
  {
    get
    {
      int id = this.ID;
      List<LettuceBountyFinalRewardsDbfRecord> finalBossRewards = new List<LettuceBountyFinalRewardsDbfRecord>();
      List<LettuceBountyFinalRewardsDbfRecord> records = GameDbf.LettuceBountyFinalRewards.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        LettuceBountyFinalRewardsDbfRecord rewardsDbfRecord = records[index];
        if (rewardsDbfRecord.LettuceBountyId == id)
          finalBossRewards.Add(rewardsDbfRecord);
      }
      return finalBossRewards;
    }
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "AVAILABLE_AFTER_EVENT":
        return (object) this.m_availableAfterEvent;
      case "BOSS_NAME_OVERRIDE":
        return (object) this.m_bossNameOverride;
      case "BOUNTY_LEVEL":
        return (object) this.m_bountyLevel;
      case "BOUNTY_NAME_OVERRIDE":
        return (object) this.m_bountyNameOverride;
      case "BOUNTY_SET_ID":
        return (object) this.m_bountySetId;
      case "COMING_SOON_TEXT":
        return (object) this.m_comingSoonText;
      case "DIFFICULTY_MODE":
        return (object) this.m_difficultyMode;
      case "ENABLED":
        return (object) this.m_enabled;
      case "EVENT":
        return (object) this.m_event;
      case "FINAL_BOSS_CARD_ID":
        return (object) this.m_finalBossCardId;
      case "HEROIC":
        return (object) this.m_heroic;
      case "ID":
        return (object) this.ID;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "REQUIRED_COMPLETED_BOUNTY":
        return (object) this.m_requiredCompletedBountyId;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
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
      case 640595383:
        if (!(name == "BOSS_NAME_OVERRIDE"))
          break;
        this.m_bossNameOverride = (DbfLocValue) val;
        break;
      case 808259902:
        if (!(name == "AVAILABLE_AFTER_EVENT"))
          break;
        this.m_availableAfterEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 864600850:
        if (!(name == "COMING_SOON_TEXT"))
          break;
        this.m_comingSoonText = (DbfLocValue) val;
        break;
      case 1302558956:
        if (!(name == "REQUIRED_COMPLETED_BOUNTY"))
          break;
        this.m_requiredCompletedBountyId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2294480894:
        if (!(name == "ENABLED"))
          break;
        this.m_enabled = (bool) val;
        break;
      case 2806321865:
        if (!(name == "HEROIC"))
          break;
        this.m_heroic = (bool) val;
        break;
      case 2823441600:
        if (!(name == "FINAL_BOSS_CARD_ID"))
          break;
        this.m_finalBossCardId = (int) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3633246378:
        if (!(name == "DIFFICULTY_MODE"))
          break;
        switch (val)
        {
          case null:
            this.m_difficultyMode = LettuceBounty.MercenariesBountyDifficulty.NONE;
            return;
          case LettuceBounty.MercenariesBountyDifficulty _:
          case int _:
            this.m_difficultyMode = (LettuceBounty.MercenariesBountyDifficulty) val;
            return;
          case string _:
            this.m_difficultyMode = LettuceBounty.ParseMercenariesBountyDifficultyValue((string) val);
            return;
          default:
            return;
        }
      case 3932547493:
        if (!(name == "BOUNTY_NAME_OVERRIDE"))
          break;
        this.m_bountyNameOverride = (DbfLocValue) val;
        break;
      case 4071717019:
        if (!(name == "BOUNTY_LEVEL"))
          break;
        this.m_bountyLevel = (int) val;
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
      case 4277087599:
        if (!(name == "BOUNTY_SET_ID"))
          break;
        this.m_bountySetId = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "AVAILABLE_AFTER_EVENT":
        return typeof (string);
      case "BOSS_NAME_OVERRIDE":
        return typeof (DbfLocValue);
      case "BOUNTY_LEVEL":
        return typeof (int);
      case "BOUNTY_NAME_OVERRIDE":
        return typeof (DbfLocValue);
      case "BOUNTY_SET_ID":
        return typeof (int);
      case "COMING_SOON_TEXT":
        return typeof (DbfLocValue);
      case "DIFFICULTY_MODE":
        return typeof (LettuceBounty.MercenariesBountyDifficulty);
      case "ENABLED":
        return typeof (bool);
      case "EVENT":
        return typeof (string);
      case "FINAL_BOSS_CARD_ID":
        return typeof (int);
      case "HEROIC":
        return typeof (bool);
      case "ID":
        return typeof (int);
      case "NOTE_DESC":
        return typeof (string);
      case "REQUIRED_COMPLETED_BOUNTY":
        return typeof (int);
      case "SORT_ORDER":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadLettuceBountyDbfRecords loadRecords = new LoadLettuceBountyDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    LettuceBountyDbfAsset lettuceBountyDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (LettuceBountyDbfAsset)) as LettuceBountyDbfAsset;
    if ((UnityEngine.Object) lettuceBountyDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("LettuceBountyDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < lettuceBountyDbfAsset.Records.Count; ++index)
      lettuceBountyDbfAsset.Records[index].StripUnusedLocales();
    records = lettuceBountyDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_bossNameOverride.StripUnusedLocales();
    this.m_bountyNameOverride.StripUnusedLocales();
    this.m_comingSoonText.StripUnusedLocales();
  }
}
