using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TaskListDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_visitorTaskChainId;
  [SerializeField]
  private int m_taskId;

  [DbfField("VISITOR_TASK_CHAIN_ID")]
  public int VisitorTaskChainId => this.m_visitorTaskChainId;

  public VisitorTaskDbfRecord TaskRecord => GameDbf.VisitorTask.GetRecord(this.m_taskId);

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "VISITOR_TASK_CHAIN_ID")
      return (object) this.m_visitorTaskChainId;
    return name == "TASK_ID" ? (object) this.m_taskId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "VISITOR_TASK_CHAIN_ID"))
      {
        if (!(name == "TASK_ID"))
          return;
        this.m_taskId = (int) val;
      }
      else
        this.m_visitorTaskChainId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "VISITOR_TASK_CHAIN_ID")
      return typeof (int);
    return name == "TASK_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadTaskListDbfRecords loadRecords = new LoadTaskListDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    TaskListDbfAsset taskListDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (TaskListDbfAsset)) as TaskListDbfAsset;
    if ((UnityEngine.Object) taskListDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("TaskListDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < taskListDbfAsset.Records.Count; ++index)
      taskListDbfAsset.Records[index].StripUnusedLocales();
    records = taskListDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
