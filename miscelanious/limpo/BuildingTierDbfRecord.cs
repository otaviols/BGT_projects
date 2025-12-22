using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuildingTierDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_mercenaryBuildingId;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private int m_unlockAchievementId;
  [SerializeField]
  private int m_onUpgradedDialogId;
  [SerializeField]
  private int m_upgradeCost;
  [SerializeField]
  private BuildingTier.VillageTutorialServerEvent m_tutorialEventType;
  [SerializeField]
  private int m_tutorialEventValue;

  [DbfField("MERCENARY_BUILDING_ID")]
  public int MercenaryBuildingId => this.m_mercenaryBuildingId;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("UNLOCK_ACHIEVEMENT")]
  public int UnlockAchievement => this.m_unlockAchievementId;

  [DbfField("ON_UPGRADED_DIALOG")]
  public int OnUpgradedDialog => this.m_onUpgradedDialogId;

  [DbfField("UPGRADE_COST")]
  public int UpgradeCost => this.m_upgradeCost;

  [DbfField("TUTORIAL_EVENT_TYPE")]
  public BuildingTier.VillageTutorialServerEvent TutorialEventType => this.m_tutorialEventType;

  [DbfField("TUTORIAL_EVENT_VALUE")]
  public int TutorialEventValue => this.m_tutorialEventValue;

  public List<TierPropertiesDbfRecord> MercenaryBuildingTierProperties
  {
    get
    {
      int id = this.ID;
      List<TierPropertiesDbfRecord> buildingTierProperties = new List<TierPropertiesDbfRecord>();
      List<TierPropertiesDbfRecord> records = GameDbf.TierProperties.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        TierPropertiesDbfRecord propertiesDbfRecord = records[index];
        if (propertiesDbfRecord.BuildingTierId == id)
          buildingTierProperties.Add(propertiesDbfRecord);
      }
      return buildingTierProperties;
    }
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "DESCRIPTION":
        return (object) this.m_description;
      case "ID":
        return (object) this.ID;
      case "MERCENARY_BUILDING_ID":
        return (object) this.m_mercenaryBuildingId;
      case "NAME":
        return (object) this.m_name;
      case "ON_UPGRADED_DIALOG":
        return (object) this.m_onUpgradedDialogId;
      case "TUTORIAL_EVENT_TYPE":
        return (object) this.m_tutorialEventType;
      case "TUTORIAL_EVENT_VALUE":
        return (object) this.m_tutorialEventValue;
      case "UNLOCK_ACHIEVEMENT":
        return (object) this.m_unlockAchievementId;
      case "UPGRADE_COST":
        return (object) this.m_upgradeCost;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 644321962:
        if (!(name == "TUTORIAL_EVENT_VALUE"))
          break;
        this.m_tutorialEventValue = (int) val;
        break;
      case 717079821:
        if (!(name == "TUTORIAL_EVENT_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_tutorialEventType = BuildingTier.VillageTutorialServerEvent.NONE;
            return;
          case BuildingTier.VillageTutorialServerEvent _:
          case int _:
            this.m_tutorialEventType = (BuildingTier.VillageTutorialServerEvent) val;
            return;
          case string _:
            this.m_tutorialEventType = BuildingTier.ParseVillageTutorialServerEventValue((string) val);
            return;
          default:
            return;
        }
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
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
      case 2687342828:
        if (!(name == "MERCENARY_BUILDING_ID"))
          break;
        this.m_mercenaryBuildingId = (int) val;
        break;
      case 3034864917:
        if (!(name == "UNLOCK_ACHIEVEMENT"))
          break;
        this.m_unlockAchievementId = (int) val;
        break;
      case 3088507594:
        if (!(name == "ON_UPGRADED_DIALOG"))
          break;
        this.m_onUpgradedDialogId = (int) val;
        break;
      case 3111376797:
        if (!(name == "UPGRADE_COST"))
          break;
        this.m_upgradeCost = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "ID":
        return typeof (int);
      case "MERCENARY_BUILDING_ID":
        return typeof (int);
      case "NAME":
        return typeof (DbfLocValue);
      case "ON_UPGRADED_DIALOG":
        return typeof (int);
      case "TUTORIAL_EVENT_TYPE":
        return typeof (BuildingTier.VillageTutorialServerEvent);
      case "TUTORIAL_EVENT_VALUE":
        return typeof (int);
      case "UNLOCK_ACHIEVEMENT":
        return typeof (int);
      case "UPGRADE_COST":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadBuildingTierDbfRecords loadRecords = new LoadBuildingTierDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BuildingTierDbfAsset buildingTierDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BuildingTierDbfAsset)) as BuildingTierDbfAsset;
    if ((UnityEngine.Object) buildingTierDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BuildingTierDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < buildingTierDbfAsset.Records.Count; ++index)
      buildingTierDbfAsset.Records[index].StripUnusedLocales();
    records = buildingTierDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
  }
}
