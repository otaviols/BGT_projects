using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class KeywordTextDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteDesc;
  [SerializeField]
  private int m_tag;
  [SerializeField]
  private string m_name;
  [SerializeField]
  private string m_text;
  [SerializeField]
  private string m_refText;
  [SerializeField]
  private string m_collectionText;

  [DbfField("TAG")]
  public int Tag => this.m_tag;

  [DbfField("NAME")]
  public string Name => this.m_name;

  [DbfField("TEXT")]
  public string Text => this.m_text;

  [DbfField("REF_TEXT")]
  public string RefText => this.m_refText;

  [DbfField("COLLECTION_TEXT")]
  public string CollectionText => this.m_collectionText;

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "COLLECTION_TEXT":
        return (object) this.m_collectionText;
      case "ID":
        return (object) this.ID;
      case "NAME":
        return (object) this.m_name;
      case "NOTE_DESC":
        return (object) this.m_noteDesc;
      case "REF_TEXT":
        return (object) this.m_refText;
      case "TAG":
        return (object) this.m_tag;
      case "TEXT":
        return (object) this.m_text;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 301355425:
        if (!(name == "COLLECTION_TEXT"))
          break;
        this.m_collectionText = (string) val;
        break;
      case 406049971:
        if (!(name == "TAG"))
          break;
        this.m_tag = (int) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (string) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2163098750:
        if (!(name == "TEXT"))
          break;
        this.m_text = (string) val;
        break;
      case 3022554311:
        if (!(name == "NOTE_DESC"))
          break;
        this.m_noteDesc = (string) val;
        break;
      case 3417487296:
        if (!(name == "REF_TEXT"))
          break;
        this.m_refText = (string) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "COLLECTION_TEXT":
        return typeof (string);
      case "ID":
        return typeof (int);
      case "NAME":
        return typeof (string);
      case "NOTE_DESC":
        return typeof (string);
      case "REF_TEXT":
        return typeof (string);
      case "TAG":
        return typeof (int);
      case "TEXT":
        return typeof (string);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadKeywordTextDbfRecords loadRecords = new LoadKeywordTextDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    KeywordTextDbfAsset keywordTextDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (KeywordTextDbfAsset)) as KeywordTextDbfAsset;
    if ((UnityEngine.Object) keywordTextDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("KeywordTextDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < keywordTextDbfAsset.Records.Count; ++index)
      keywordTextDbfAsset.Records[index].StripUnusedLocales();
    records = keywordTextDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
