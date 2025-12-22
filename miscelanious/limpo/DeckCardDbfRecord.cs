using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckCardDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_nextCardId;
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_deckId;
  [SerializeField]
  private DbfLocValue m_description;

  [DbfField("NEXT_CARD")]
  public int NextCard => this.m_nextCardId;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  public CardDbfRecord CardRecord => GameDbf.Card.GetRecord(this.m_cardId);

  [DbfField("DECK_ID")]
  public int DeckId => this.m_deckId;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "NEXT_CARD")
      return (object) this.m_nextCardId;
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    if (name == "DECK_ID")
      return (object) this.m_deckId;
    return name == "DESCRIPTION" ? (object) this.m_description : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "NEXT_CARD"))
      {
        if (!(name == "CARD_ID"))
        {
          if (!(name == "DECK_ID"))
          {
            if (!(name == "DESCRIPTION"))
              return;
            this.m_description = (DbfLocValue) val;
          }
          else
            this.m_deckId = (int) val;
        }
        else
          this.m_cardId = (int) val;
      }
      else
        this.m_nextCardId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "NEXT_CARD")
      return typeof (int);
    if (name == "CARD_ID")
      return typeof (int);
    if (name == "DECK_ID")
      return typeof (int);
    return name == "DESCRIPTION" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadDeckCardDbfRecords loadRecords = new LoadDeckCardDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    DeckCardDbfAsset deckCardDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (DeckCardDbfAsset)) as DeckCardDbfAsset;
    if ((UnityEngine.Object) deckCardDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("DeckCardDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < deckCardDbfAsset.Records.Count; ++index)
      deckCardDbfAsset.Records[index].StripUnusedLocales();
    records = deckCardDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales() => this.m_description.StripUnusedLocales();
}
