using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MercTriggeringEventDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_mercenaryVillageTriggerId;
  [SerializeField]
  private MercTriggeringEvent.EventType m_eventType;
  [SerializeField]
  private int m_visitorId;
  [SerializeField]
  private int m_visitorTaskId;
  [SerializeField]
  private int m_buildingTierId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "MERCENARY_VILLAGE_TRIGGER_ID")
      return (object) this.m_mercenaryVillageTriggerId;
    if (name == "EVENT_TYPE")
      return (object) this.m_eventType;
    if (name == "VISITOR")
      return (object) this.m_visitorId;
    if (name == "VISITOR_TASK")
      return (object) this.m_visitorTaskId;
    return name == "BUILDING_TIER" ? (object) this.m_buildingTierId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "MERCENARY_VILLAGE_TRIGGER_ID"))
      {
        if (!(name == "EVENT_TYPE"))
        {
          if (!(name == "VISITOR"))
          {
            if (!(name == "VISITOR_TASK"))
            {
              if (!(name == "BUILDING_TIER"))
                return;
              this.m_buildingTierId = (int) val;
            }
            else
              this.m_visitorTaskId = (int) val;
          }
          else
            this.m_visitorId = (int) val;
        }
        else
        {
          switch (val)
          {
            case null:
              this.m_eventType = MercTriggeringEvent.EventType.NONE;
              break;
            case MercTriggeringEvent.EventType _:
            case int _:
              this.m_eventType = (MercTriggeringEvent.EventType) val;
              break;
            case string _:
              this.m_eventType = MercTriggeringEvent.ParseEventTypeValue((string) val);
              break;
          }
        }
      }
      else
        this.m_mercenaryVillageTriggerId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "MERCENARY_VILLAGE_TRIGGER_ID")
      return typeof (int);
    if (name == "EVENT_TYPE")
      return typeof (MercTriggeringEvent.EventType);
    if (name == "VISITOR")
      return typeof (int);
    if (name == "VISITOR_TASK")
      return typeof (int);
    return name == "BUILDING_TIER" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadMercTriggeringEventDbfRecords loadRecords = new LoadMercTriggeringEventDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    MercTriggeringEventDbfAsset triggeringEventDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (MercTriggeringEventDbfAsset)) as MercTriggeringEventDbfAsset;
    if ((UnityEngine.Object) triggeringEventDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("MercTriggeringEventDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < triggeringEventDbfAsset.Records.Count; ++index)
      triggeringEventDbfAsset.Records[index].StripUnusedLocales();
    records = triggeringEventDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
