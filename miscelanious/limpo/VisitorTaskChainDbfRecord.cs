using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class VisitorTaskChainDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_mercenaryVisitorId;

  [DbfField("MERCENARY_VISITOR_ID")]
  public int MercenaryVisitorId => this.m_mercenaryVisitorId;

  public List<TaskListDbfRecord> TaskList
  {
    get
    {
      int id = this.ID;
      List<TaskListDbfRecord> taskList = new List<TaskListDbfRecord>();
      List<TaskListDbfRecord> records = GameDbf.TaskList.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        TaskListDbfRecord taskListDbfRecord = records[index];
        if (taskListDbfRecord.VisitorTaskChainId == id)
          taskList.Add(taskListDbfRecord);
      }
      return taskList;
    }
  }

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "MERCENARY_VISITOR_ID" ? (object) this.m_mercenaryVisitorId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "MERCENARY_VISITOR_ID"))
        return;
      this.m_mercenaryVisitorId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "MERCENARY_VISITOR_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadVisitorTaskChainDbfRecords loadRecords = new LoadVisitorTaskChainDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    VisitorTaskChainDbfAsset taskChainDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (VisitorTaskChainDbfAsset)) as VisitorTaskChainDbfAsset;
    if ((UnityEngine.Object) taskChainDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("VisitorTaskChainDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < taskChainDbfAsset.Records.Count; ++index)
      taskChainDbfAsset.Records[index].StripUnusedLocales();
    records = taskChainDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
