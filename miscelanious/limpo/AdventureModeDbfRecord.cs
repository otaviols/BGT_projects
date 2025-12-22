using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AdventureModeDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    return name == "NOTE_DESC" ? (object) this.m_noteDesc : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NOTE_DESC"))
        return;
      this.m_noteDesc = (string) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    return name == "NOTE_DESC" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadAdventureModeDbfRecords loadRecords = new LoadAdventureModeDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    AdventureModeDbfAsset adventureModeDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (AdventureModeDbfAsset)) as AdventureModeDbfAsset;
    if ((UnityEngine.Object) adventureModeDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("AdventureModeDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < adventureModeDbfAsset.Records.Count; ++index)
      adventureModeDbfAsset.Records[index].StripUnusedLocales();
    records = adventureModeDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
