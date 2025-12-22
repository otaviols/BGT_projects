using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MercenaryVisitorDbfRecord : DbfRecord
{
  [SerializeField]
  private SpecialEventType m_event = SpecialEventType.UNKNOWN;
  [SerializeField]
  private int m_mercenaryId;
  [SerializeField]
  private MercenaryVisitor.VillageVisitorType m_visitorType;
  [SerializeField]
  private int m_maxEventTasksPerDay = 1;

  [DbfField("EVENT")]
  public SpecialEventType Event => this.m_event;

  [DbfField("MERCENARY_ID")]
  public int MercenaryId => this.m_mercenaryId;

  [DbfField("VISITOR_TYPE")]
  public MercenaryVisitor.VillageVisitorType VisitorType => this.m_visitorType;

  public List<VisitorTaskChainDbfRecord> VisitorTaskChains
  {
    get
    {
      int id = this.ID;
      List<VisitorTaskChainDbfRecord> visitorTaskChains = new List<VisitorTaskChainDbfRecord>();
      List<VisitorTaskChainDbfRecord> records = GameDbf.VisitorTaskChain.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        VisitorTaskChainDbfRecord taskChainDbfRecord = records[index];
        if (taskChainDbfRecord.MercenaryVisitorId == id)
          visitorTaskChains.Add(taskChainDbfRecord);
      }
      return visitorTaskChains;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "EVENT")
      return (object) this.m_event;
    if (name == "MERCENARY_ID")
      return (object) this.m_mercenaryId;
    if (name == "VISITOR_TYPE")
      return (object) this.m_visitorType;
    return name == "MAX_EVENT_TASKS_PER_DAY" ? (object) this.m_maxEventTasksPerDay : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "EVENT"))
      {
        if (!(name == "MERCENARY_ID"))
        {
          if (!(name == "VISITOR_TYPE"))
          {
            if (!(name == "MAX_EVENT_TASKS_PER_DAY"))
              return;
            this.m_maxEventTasksPerDay = (int) val;
          }
          else
          {
            switch (val)
            {
              case null:
                this.m_visitorType = MercenaryVisitor.VillageVisitorType.STANDARD;
                break;
              case MercenaryVisitor.VillageVisitorType _:
              case int _:
                this.m_visitorType = (MercenaryVisitor.VillageVisitorType) val;
                break;
              case string _:
                this.m_visitorType = MercenaryVisitor.ParseVillageVisitorTypeValue((string) val);
                break;
            }
          }
        }
        else
          this.m_mercenaryId = (int) val;
      }
      else
        this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "EVENT")
      return typeof (string);
    if (name == "MERCENARY_ID")
      return typeof (int);
    if (name == "VISITOR_TYPE")
      return typeof (MercenaryVisitor.VillageVisitorType);
    return name == "MAX_EVENT_TASKS_PER_DAY" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadMercenaryVisitorDbfRecords loadRecords = new LoadMercenaryVisitorDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    MercenaryVisitorDbfAsset mercenaryVisitorDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (MercenaryVisitorDbfAsset)) as MercenaryVisitorDbfAsset;
    if ((UnityEngine.Object) mercenaryVisitorDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("MercenaryVisitorDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < mercenaryVisitorDbfAsset.Records.Count; ++index)
      mercenaryVisitorDbfAsset.Records[index].StripUnusedLocales();
    records = mercenaryVisitorDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
