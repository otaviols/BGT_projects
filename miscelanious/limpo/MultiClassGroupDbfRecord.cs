using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MultiClassGroupDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private string m_iconAssetPath;
  [SerializeField]
  private int m_cardColorType;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  [DbfField("ICON_ASSET_PATH")]
  public string IconAssetPath => this.m_iconAssetPath;

  [DbfField("CARD_COLOR_TYPE")]
  public int CardColorType => this.m_cardColorType;

  public void SetNoteDesc(string v) => this.m_noteDesc = v;

  public void SetIconAssetPath(string v) => this.m_iconAssetPath = v;

  public void SetCardColorType(int v) => this.m_cardColorType = v;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "NOTE_DESC")
      return (object) this.m_noteDesc;
    if (name == "ICON_ASSET_PATH")
      return (object) this.m_iconAssetPath;
    return name == "CARD_COLOR_TYPE" ? (object) this.m_cardColorType : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NOTE_DESC"))
      {
        if (!(name == "ICON_ASSET_PATH"))
        {
          if (!(name == "CARD_COLOR_TYPE"))
            return;
          this.m_cardColorType = (int) val;
        }
        else
          this.m_iconAssetPath = (string) val;
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
    if (name == "ICON_ASSET_PATH")
      return typeof (string);
    return name == "CARD_COLOR_TYPE" ? typeof (int) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadMultiClassGroupDbfRecords loadRecords = new LoadMultiClassGroupDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    MultiClassGroupDbfAsset classGroupDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (MultiClassGroupDbfAsset)) as MultiClassGroupDbfAsset;
    if ((UnityEngine.Object) classGroupDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("MultiClassGroupDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < classGroupDbfAsset.Records.Count; ++index)
      classGroupDbfAsset.Records[index].StripUnusedLocales();
    records = classGroupDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
