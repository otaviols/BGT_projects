using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubsetDbfRecord : DbfRecord
{
  public List<SubsetRuleDbfRecord> Rules
  {
    get
    {
      int id = this.ID;
      List<SubsetRuleDbfRecord> rules = new List<SubsetRuleDbfRecord>();
      List<SubsetRuleDbfRecord> records = GameDbf.SubsetRule.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        SubsetRuleDbfRecord subsetRuleDbfRecord = records[index];
        if (subsetRuleDbfRecord.SubsetId == id)
          rules.Add(subsetRuleDbfRecord);
      }
      return rules;
    }
  }

  public override object GetVar(string name) => name == "ID" ? (object) this.ID : (object) null;

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
      return;
    this.SetID((int) val);
  }

  public override System.Type GetVarType(string name) => name == "ID" ? typeof (int) : (System.Type) null;

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadSubsetDbfRecords loadRecords = new LoadSubsetDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    SubsetDbfAsset subsetDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (SubsetDbfAsset)) as SubsetDbfAsset;
    if ((UnityEngine.Object) subsetDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("SubsetDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < subsetDbfAsset.Records.Count; ++index)
      subsetDbfAsset.Records[index].StripUnusedLocales();
    records = subsetDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
