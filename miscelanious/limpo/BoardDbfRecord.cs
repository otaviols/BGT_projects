using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoardDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private string m_prefab;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  [DbfField("PREFAB")]
  public string Prefab => this.m_prefab;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "NOTE_DESC")
      return (object) this.m_noteDesc;
    return name == "PREFAB" ? (object) this.m_prefab : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NOTE_DESC"))
      {
        if (!(name == "PREFAB"))
          return;
        this.m_prefab = (string) val;
      }
      else
        this.m_noteDesc = (string) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "NOTE_DESC")
      return typeof (string);
    return name == "PREFAB" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadBoardDbfRecords loadRecords = new LoadBoardDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BoardDbfAsset boardDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BoardDbfAsset)) as BoardDbfAsset;
    if ((UnityEngine.Object) boardDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BoardDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < boardDbfAsset.Records.Count; ++index)
      boardDbfAsset.Records[index].StripUnusedLocales();
    records = boardDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
