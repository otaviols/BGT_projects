using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CreditsYearDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private string m_contentsFilename;
  [SerializeField]
  private DbfLocValue m_buttonLabel;

  [DbfField("CONTENTS_FILENAME")]
  public string ContentsFilename => this.m_contentsFilename;

  [DbfField("BUTTON_LABEL")]
  public DbfLocValue ButtonLabel => this.m_buttonLabel;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "NOTE_DESC")
      return (object) this.m_noteDesc;
    if (name == "CONTENTS_FILENAME")
      return (object) this.m_contentsFilename;
    return name == "BUTTON_LABEL" ? (object) this.m_buttonLabel : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NOTE_DESC"))
      {
        if (!(name == "CONTENTS_FILENAME"))
        {
          if (!(name == "BUTTON_LABEL"))
            return;
          this.m_buttonLabel = (DbfLocValue) val;
        }
        else
          this.m_contentsFilename = (string) val;
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
    if (name == "CONTENTS_FILENAME")
      return typeof (string);
    return name == "BUTTON_LABEL" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCreditsYearDbfRecords loadRecords = new LoadCreditsYearDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CreditsYearDbfAsset creditsYearDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CreditsYearDbfAsset)) as CreditsYearDbfAsset;
    if ((UnityEngine.Object) creditsYearDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CreditsYearDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < creditsYearDbfAsset.Records.Count; ++index)
      creditsYearDbfAsset.Records[index].StripUnusedLocales();
    records = creditsYearDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_buttonLabel.StripUnusedLocales();
}
