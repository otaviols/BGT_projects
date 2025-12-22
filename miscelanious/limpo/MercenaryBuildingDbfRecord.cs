using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MercenaryBuildingDbfRecord : DbfRecord
{
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private MercenaryBuilding.Mercenarybuildingtype m_mercenaryBuildingType = MercenaryBuilding.ParseMercenarybuildingtypeValue("Invalid");
  [SerializeField]
  private int m_defaultTierId;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("MERCENARY_BUILDING_TYPE")]
  public MercenaryBuilding.Mercenarybuildingtype MercenaryBuildingType => this.m_mercenaryBuildingType;

  [DbfField("DEFAULT_TIER")]
  public int DefaultTier => this.m_defaultTierId;

  public List<BuildingTierDbfRecord> MercenaryBuildingTiers
  {
    get
    {
      int id = this.ID;
      List<BuildingTierDbfRecord> mercenaryBuildingTiers = new List<BuildingTierDbfRecord>();
      List<BuildingTierDbfRecord> records = GameDbf.BuildingTier.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        BuildingTierDbfRecord buildingTierDbfRecord = records[index];
        if (buildingTierDbfRecord.MercenaryBuildingId == id)
          mercenaryBuildingTiers.Add(buildingTierDbfRecord);
      }
      return mercenaryBuildingTiers;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "NAME")
      return (object) this.m_name;
    if (name == "DESCRIPTION")
      return (object) this.m_description;
    if (name == "MERCENARY_BUILDING_TYPE")
      return (object) this.m_mercenaryBuildingType;
    return name == "DEFAULT_TIER" ? (object) this.m_defaultTierId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NAME"))
      {
        if (!(name == "DESCRIPTION"))
        {
          if (!(name == "MERCENARY_BUILDING_TYPE"))
          {
            if (!(name == "DEFAULT_TIER"))
              return;
            this.m_defaultTierId = (int) val;
          }
          else
          {
            switch (val)
            {
              case null:
                this.m_mercenaryBuildingType = MercenaryBuilding.Mercenarybuildingtype.VILLAGE;
                break;
              case MercenaryBuilding.Mercenarybuildingtype _:
              case int _:
                this.m_mercenaryBuildingType = (MercenaryBuilding.Mercenarybuildingtype) val;
                break;
              case string _:
                this.m_mercenaryBuildingType = MercenaryBuilding.ParseMercenarybuildingtypeValue((string) val);
                break;
            }
          }
        }
        else
          this.m_description = (DbfLocValue) val;
      }
      else
        this.m_name = (DbfLocValue) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "NAME")
      return typeof (DbfLocValue);
    if (name == "DESCRIPTION")
      return typeof (DbfLocValue);
    if (name == "MERCENARY_BUILDING_TYPE")
      return typeof (MercenaryBuilding.Mercenarybuildingtype);
    return name == "DEFAULT_TIER" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadMercenaryBuildingDbfRecords loadRecords = new LoadMercenaryBuildingDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    MercenaryBuildingDbfAsset buildingDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (MercenaryBuildingDbfAsset)) as MercenaryBuildingDbfAsset;
    if ((UnityEngine.Object) buildingDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("MercenaryBuildingDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < buildingDbfAsset.Records.Count; ++index)
      buildingDbfAsset.Records[index].StripUnusedLocales();
    records = buildingDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
  }
}
