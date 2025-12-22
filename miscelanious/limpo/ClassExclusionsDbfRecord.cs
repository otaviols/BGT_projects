using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ClassExclusionsDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_scenarioId;
  [SerializeField]
  private int m_classId;

  [DbfField("SCENARIO_ID")]
  public int ScenarioId => this.m_scenarioId;

  [DbfField("CLASS_ID")]
  public int ClassId => this.m_classId;

  public void SetScenarioId(int v) => this.m_scenarioId = v;

  public void SetClassId(int v) => this.m_classId = v;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "SCENARIO_ID")
      return (object) this.m_scenarioId;
    return name == "CLASS_ID" ? (object) this.m_classId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "SCENARIO_ID"))
      {
        if (!(name == "CLASS_ID"))
          return;
        this.m_classId = (int) val;
      }
      else
        this.m_scenarioId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "SCENARIO_ID")
      return typeof (int);
    return name == "CLASS_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadClassExclusionsDbfRecords loadRecords = new LoadClassExclusionsDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    ClassExclusionsDbfAsset exclusionsDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (ClassExclusionsDbfAsset)) as ClassExclusionsDbfAsset;
    if ((UnityEngine.Object) exclusionsDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("ClassExclusionsDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < exclusionsDbfAsset.Records.Count; ++index)
      exclusionsDbfAsset.Records[index].StripUnusedLocales();
    records = exclusionsDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
