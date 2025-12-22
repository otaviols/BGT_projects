using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RepeatableTaskListDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_mercenaryVisitorId;
  [SerializeField]
  private int m_taskId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "MERCENARY_VISITOR_ID")
      return (object) this.m_mercenaryVisitorId;
    return name == "TASK_ID" ? (object) this.m_taskId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "MERCENARY_VISITOR_ID"))
      {
        if (!(name == "TASK_ID"))
          return;
        this.m_taskId = (int) val;
      }
      else
        this.m_mercenaryVisitorId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "MERCENARY_VISITOR_ID")
      return typeof (int);
    return name == "TASK_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadRepeatableTaskListDbfRecords loadRecords = new LoadRepeatableTaskListDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    RepeatableTaskListDbfAsset taskListDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (RepeatableTaskListDbfAsset)) as RepeatableTaskListDbfAsset;
    if ((UnityEngine.Object) taskListDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("RepeatableTaskListDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
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
