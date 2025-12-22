using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PowerDefinitionDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_notes;

  public override object GetVar(string name) => name == "NOTES" ? (object) this.m_notes : (object) null;

  public override void SetVar(string name, object val)
  {
    if (!(name == "NOTES"))
      return;
    this.m_notes = (string) val;
  }

  public override System.Type GetVarType(string name) => name == "NOTES" ? typeof (string) : (System.Type) null;

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadPowerDefinitionDbfRecords loadRecords = new LoadPowerDefinitionDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    PowerDefinitionDbfAsset definitionDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (PowerDefinitionDbfAsset)) as PowerDefinitionDbfAsset;
    if ((UnityEngine.Object) definitionDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("PowerDefinitionDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < definitionDbfAsset.Records.Count; ++index)
      definitionDbfAsset.Records[index].StripUnusedLocales();
    records = definitionDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
