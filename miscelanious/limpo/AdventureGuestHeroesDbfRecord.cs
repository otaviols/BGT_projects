using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AdventureGuestHeroesDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_adventureId;
  [SerializeField]
  private int m_wingId;
  [SerializeField]
  private int m_guestHeroId;
  [SerializeField]
  private int m_baseGuestHeroId;
  [SerializeField]
  private DbfLocValue m_unlockCriteriaText;
  [SerializeField]
  private DbfLocValue m_comingSoonText;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private int m_customScenarioId;

  [DbfField("ADVENTURE_ID")]
  public int AdventureId => this.m_adventureId;

  [DbfField("WING_ID")]
  public int WingId => this.m_wingId;

  public WingDbfRecord WingRecord => GameDbf.Wing.GetRecord(this.m_wingId);

  [DbfField("GUEST_HERO_ID")]
  public int GuestHeroId => this.m_guestHeroId;

  public GuestHeroDbfRecord GuestHeroRecord => GameDbf.GuestHero.GetRecord(this.m_guestHeroId);

  [DbfField("BASE_GUEST_HERO_ID")]
  public int BaseGuestHeroId => this.m_baseGuestHeroId;

  public GuestHeroDbfRecord BaseGuestHeroRecord => GameDbf.GuestHero.GetRecord(this.m_baseGuestHeroId);

  [DbfField("UNLOCK_CRITERIA_TEXT")]
  public DbfLocValue UnlockCriteriaText => this.m_unlockCriteriaText;

  [DbfField("COMING_SOON_TEXT")]
  public DbfLocValue ComingSoonText => this.m_comingSoonText;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("CUSTOM_SCENARIO")]
  public int CustomScenario => this.m_customScenarioId;

  public ScenarioDbfRecord CustomScenarioRecord => GameDbf.Scenario.GetRecord(this.m_customScenarioId);

  public void SetAdventureId(int v) => this.m_adventureId = v;

  public void SetWingId(int v) => this.m_wingId = v;

  public void SetGuestHeroId(int v) => this.m_guestHeroId = v;

  public void SetBaseGuestHeroId(int v) => this.m_baseGuestHeroId = v;

  public void SetUnlockCriteriaText(DbfLocValue v)
  {
    this.m_unlockCriteriaText = v;
    v.SetDebugInfo(this.ID, "UNLOCK_CRITERIA_TEXT");
  }

  public void SetComingSoonText(DbfLocValue v)
  {
    this.m_comingSoonText = v;
    v.SetDebugInfo(this.ID, "COMING_SOON_TEXT");
  }

  public void SetSortOrder(int v) => this.m_sortOrder = v;

  public void SetCustomScenario(int v) => this.m_customScenarioId = v;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ADVENTURE_ID":
        return (object) this.m_adventureId;
      case "BASE_GUEST_HERO_ID":
        return (object) this.m_baseGuestHeroId;
      case "COMING_SOON_TEXT":
        return (object) this.m_comingSoonText;
      case "CUSTOM_SCENARIO":
        return (object) this.m_customScenarioId;
      case "GUEST_HERO_ID":
        return (object) this.m_guestHeroId;
      case "ID":
        return (object) this.ID;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      case "UNLOCK_CRITERIA_TEXT":
        return (object) this.m_unlockCriteriaText;
      case "WING_ID":
        return (object) this.m_wingId;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 190718801:
        if (!(name == "ADVENTURE_ID"))
          break;
        this.m_adventureId = (int) val;
        break;
      case 864600850:
        if (!(name == "COMING_SOON_TEXT"))
          break;
        this.m_comingSoonText = (DbfLocValue) val;
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
      case 1966695012:
        if (!(name == "GUEST_HERO_ID"))
          break;
        this.m_guestHeroId = (int) val;
        break;
      case 3115778841:
        if (!(name == "CUSTOM_SCENARIO"))
          break;
        this.m_customScenarioId = (int) val;
        break;
      case 3620978596:
        if (!(name == "BASE_GUEST_HERO_ID"))
          break;
        this.m_baseGuestHeroId = (int) val;
        break;
      case 3710150967:
        if (!(name == "UNLOCK_CRITERIA_TEXT"))
          break;
        this.m_unlockCriteriaText = (DbfLocValue) val;
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ADVENTURE_ID":
        return typeof (int);
      case "BASE_GUEST_HERO_ID":
        return typeof (int);
      case "COMING_SOON_TEXT":
        return typeof (DbfLocValue);
      case "CUSTOM_SCENARIO":
        return typeof (int);
      case "GUEST_HERO_ID":
        return typeof (int);
      case "ID":
        return typeof (int);
      case "SORT_ORDER":
        return typeof (int);
      case "UNLOCK_CRITERIA_TEXT":
        return typeof (DbfLocValue);
      case "WING_ID":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAdventureGuestHeroesDbfRecords loadRecords = new LoadAdventureGuestHeroesDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AdventureGuestHeroesDbfAsset guestHeroesDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AdventureGuestHeroesDbfAsset)) as AdventureGuestHeroesDbfAsset;
    if ((UnityEngine.Object) guestHeroesDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AdventureGuestHeroesDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < guestHeroesDbfAsset.Records.Count; ++index)
      guestHeroesDbfAsset.Records[index].StripUnusedLocales();
    records = guestHeroesDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_unlockCriteriaText.StripUnusedLocales();
    this.m_comingSoonText.StripUnusedLocales();
  }
}
