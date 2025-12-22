using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteName;
  [SerializeField]
  private int m_topCardId;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_description;
  [SerializeField]
  private DbfLocValue m_altDescription;
  [SerializeField]
  private int m_preconClass;

  [DbfField("TOP_CARD_ID")]
  public int TopCardId => this.m_topCardId;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("DESCRIPTION")]
  public DbfLocValue Description => this.m_description;

  [DbfField("ALT_DESCRIPTION")]
  public DbfLocValue AltDescription => this.m_altDescription;

  public List<DeckCardDbfRecord> Cards
  {
    get
    {
      int id = this.ID;
      List<DeckCardDbfRecord> cards = new List<DeckCardDbfRecord>();
      List<DeckCardDbfRecord> records = GameDbf.DeckCard.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        DeckCardDbfRecord deckCardDbfRecord = records[index];
        if (deckCardDbfRecord.DeckId == id)
          cards.Add(deckCardDbfRecord);
      }
      return cards;
    }
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ALT_DESCRIPTION":
        return (object) this.m_altDescription;
      case "DESCRIPTION":
        return (object) this.m_description;
      case "ID":
        return (object) this.ID;
      case "NAME":
        return (object) this.m_name;
      case "NOTE_NAME":
        return (object) this.m_noteName;
      case "PRECON_CLASS":
        return (object) this.m_preconClass;
      case "TOP_CARD_ID":
        return (object) this.m_topCardId;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 1103584457:
        if (!(name == "DESCRIPTION"))
          break;
        this.m_description = (DbfLocValue) val;
        break;
      case 1387956774:
        if (!(name == "NAME"))
          break;
        this.m_name = (DbfLocValue) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 1629023597:
        if (!(name == "ALT_DESCRIPTION"))
          break;
        this.m_altDescription = (DbfLocValue) val;
        break;
      case 1927439553:
        if (!(name == "TOP_CARD_ID"))
          break;
        this.m_topCardId = (int) val;
        break;
      case 2485258469:
        if (!(name == "NOTE_NAME"))
          break;
        this.m_noteName = (string) val;
        break;
      case 2986314849:
        if (!(name == "PRECON_CLASS"))
          break;
        this.m_preconClass = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ALT_DESCRIPTION":
        return typeof (DbfLocValue);
      case "DESCRIPTION":
        return typeof (DbfLocValue);
      case "ID":
        return typeof (int);
      case "NAME":
        return typeof (DbfLocValue);
      case "NOTE_NAME":
        return typeof (string);
      case "PRECON_CLASS":
        return typeof (int);
      case "TOP_CARD_ID":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadDeckDbfRecords loadRecords = new LoadDeckDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    DeckDbfAsset deckDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (DeckDbfAsset)) as DeckDbfAsset;
    if ((UnityEngine.Object) deckDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("DeckDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < deckDbfAsset.Records.Count; ++index)
      deckDbfAsset.Records[index].StripUnusedLocales();
    records = deckDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_name.StripUnusedLocales();
    this.m_description.StripUnusedLocales();
    this.m_altDescription.StripUnusedLocales();
  }
}
