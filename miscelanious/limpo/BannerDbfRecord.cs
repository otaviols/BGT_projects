using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BannerDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private DbfLocValue m_headerText;
  [SerializeField]
  private DbfLocValue m_text;
  [SerializeField]
  private string m_prefab;

  [DbfField("NOTE_DESC")]
  public string NoteDesc => this.m_noteDesc;

  [DbfField("HEADER_TEXT")]
  public DbfLocValue HeaderText => this.m_headerText;

  [DbfField("TEXT")]
  public DbfLocValue Text => this.m_text;

  [DbfField("PREFAB")]
  public string Prefab => this.m_prefab;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "NOTE_DESC")
      return (object) this.m_noteDesc;
    if (name == "HEADER_TEXT")
      return (object) this.m_headerText;
    if (name == "TEXT")
      return (object) this.m_text;
    return name == "PREFAB" ? (object) this.m_prefab : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NOTE_DESC"))
      {
        if (!(name == "HEADER_TEXT"))
        {
          if (!(name == "TEXT"))
          {
            if (!(name == "PREFAB"))
              return;
            this.m_prefab = (string) val;
          }
          else
            this.m_text = (DbfLocValue) val;
        }
        else
          this.m_headerText = (DbfLocValue) val;
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
    if (name == "HEADER_TEXT")
      return typeof (DbfLocValue);
    if (name == "TEXT")
      return typeof (DbfLocValue);
    return name == "PREFAB" ? typeof (string) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadBannerDbfRecords loadRecords = new LoadBannerDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    BannerDbfAsset bannerDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (BannerDbfAsset)) as BannerDbfAsset;
    if ((UnityEngine.Object) bannerDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("BannerDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < bannerDbfAsset.Records.Count; ++index)
      bannerDbfAsset.Records[index].StripUnusedLocales();
    records = bannerDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_headerText.StripUnusedLocales();
    this.m_text.StripUnusedLocales();
  }
}
