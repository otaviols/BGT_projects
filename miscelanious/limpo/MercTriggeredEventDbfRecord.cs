using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MercTriggeredEventDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_mercenaryVillageTriggerId;
  [SerializeField]
  private MercTriggeredEvent.EventType m_eventType;
  [SerializeField]
  private bool m_successRequired = true;
  [SerializeField]
  private int m_visitorId;
  [SerializeField]
  private int m_quantity = 1;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "MERCENARY_VILLAGE_TRIGGER_ID")
      return (object) this.m_mercenaryVillageTriggerId;
    if (name == "EVENT_TYPE")
      return (object) this.m_eventType;
    if (name == "SUCCESS_REQUIRED")
      return (object) this.m_successRequired;
    if (name == "VISITOR")
      return (object) this.m_visitorId;
    return name == "QUANTITY" ? (object) this.m_quantity : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "MERCENARY_VILLAGE_TRIGGER_ID"))
      {
        if (!(name == "EVENT_TYPE"))
        {
          if (!(name == "SUCCESS_REQUIRED"))
          {
            if (!(name == "VISITOR"))
            {
              if (!(name == "QUANTITY"))
                return;
              this.m_quantity = (int) val;
            }
            else
              this.m_visitorId = (int) val;
          }
          else
            this.m_successRequired = (bool) val;
        }
        else
        {
          switch (val)
          {
            case null:
              this.m_eventType = MercTriggeredEvent.EventType.NONE;
              break;
            case MercTriggeredEvent.EventType _:
            case int _:
              this.m_eventType = (MercTriggeredEvent.EventType) val;
              break;
            case string _:
              this.m_eventType = MercTriggeredEvent.ParseEventTypeValue((string) val);
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
      return typeof (MercTriggeredEvent.EventType);
    if (name == "SUCCESS_REQUIRED")
      return typeof (bool);
    if (name == "VISITOR")
      return typeof (int);
    return name == "QUANTITY" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadMercTriggeredEventDbfRecords loadRecords = new LoadMercTriggeredEventDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    MercTriggeredEventDbfAsset triggeredEventDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (MercTriggeredEventDbfAsset)) as MercTriggeredEventDbfAsset;
    if ((UnityEngine.Object) triggeredEventDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("MercTriggeredEventDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < triggeredEventDbfAsset.Records.Count; ++index)
      triggeredEventDbfAsset.Records[index].StripUnusedLocales();
    records = triggeredEventDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
