using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AchieveConditionDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_achieveId;
  [SerializeField]
  private int m_scenarioId;

  [DbfField("ACHIEVE_ID")]
  public int AchieveId => this.m_achieveId;

  [DbfField("SCENARIO_ID")]
  public int ScenarioId => this.m_scenarioId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "ACHIEVE_ID")
      return (object) this.m_achieveId;
    return name == "SCENARIO_ID" ? (object) this.m_scenarioId : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "ACHIEVE_ID"))
      {
        if (!(name == "SCENARIO_ID"))
          return;
        this.m_scenarioId = (int) val;
      }
      else
        this.m_achieveId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "ACHIEVE_ID")
      return typeof (int);
    return name == "SCENARIO_ID" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAchieveConditionDbfRecords loadRecords = new LoadAchieveConditionDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AchieveConditionDbfAsset conditionDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AchieveConditionDbfAsset)) as AchieveConditionDbfAsset;
    if ((UnityEngine.Object) conditionDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AchieveConditionDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < conditionDbfAsset.Records.Count; ++index)
      conditionDbfAsset.Records[index].StripUnusedLocales();
    records = conditionDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
