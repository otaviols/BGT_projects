using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TierPropertiesDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_buildingTierId;
  [SerializeField]
  private TierProperties.Buildingtierproperty m_tierPropertyType = TierProperties.ParseBuildingtierpropertyValue("Invalid");
  [SerializeField]
  private int m_tierPropertyValue;

  [DbfField("BUILDING_TIER_ID")]
  public int BuildingTierId => this.m_buildingTierId;

  [DbfField("TIER_PROPERTY_TYPE")]
  public TierProperties.Buildingtierproperty TierPropertyType => this.m_tierPropertyType;

  [DbfField("TIER_PROPERTY_VALUE")]
  public int TierPropertyValue => this.m_tierPropertyValue;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "BUILDING_TIER_ID")
      return (object) this.m_buildingTierId;
    if (name == "TIER_PROPERTY_TYPE")
      return (object) this.m_tierPropertyType;
    return name == "TIER_PROPERTY_VALUE" ? (object) this.m_tierPropertyValue : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "BUILDING_TIER_ID"))
      {
        if (!(name == "TIER_PROPERTY_TYPE"))
        {
          if (!(name == "TIER_PROPERTY_VALUE"))
            return;
          this.m_tierPropertyValue = (int) val;
        }
        else
        {
          switch (val)
          {
            case null:
              this.m_tierPropertyType = ~TierProperties.Buildingtierproperty.INVALID;
              break;
            case TierProperties.Buildingtierproperty _:
            case int _:
              this.m_tierPropertyType = (TierProperties.Buildingtierproperty) val;
              break;
            case string _:
              this.m_tierPropertyType = TierProperties.ParseBuildingtierpropertyValue((string) val);
              break;
          }
        }
      }
      else
        this.m_buildingTierId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "BUILDING_TIER_ID")
      return typeof (int);
    if (name == "TIER_PROPERTY_TYPE")
      return typeof (TierProperties.Buildingtierproperty);
    return name == "TIER_PROPERTY_VALUE" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadTierPropertiesDbfRecords loadRecords = new LoadTierPropertiesDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    TierPropertiesDbfAsset propertiesDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (TierPropertiesDbfAsset)) as TierPropertiesDbfAsset;
    if ((UnityEngine.Object) propertiesDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("TierPropertiesDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < propertiesDbfAsset.Records.Count; ++index)
      propertiesDbfAsset.Records[index].StripUnusedLocales();
    records = propertiesDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
