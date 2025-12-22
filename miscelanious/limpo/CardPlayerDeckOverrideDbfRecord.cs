using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardPlayerDeckOverrideDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_cardId;
  [SerializeField]
  private int m_heroCardId;
  [SerializeField]
  private DbfLocValue m_deckName;
  [SerializeField]
  private DbfLocValue m_addToDeckWarningHeader;
  [SerializeField]
  private DbfLocValue m_addToDeckWarningBody;

  [DbfField("CARD_ID")]
  public int CardId => this.m_cardId;

  [DbfField("HERO_CARD_ID")]
  public int HeroCardId => this.m_heroCardId;

  [DbfField("DECK_NAME")]
  public DbfLocValue DeckName => this.m_deckName;

  [DbfField("ADD_TO_DECK_WARNING_HEADER")]
  public DbfLocValue AddToDeckWarningHeader => this.m_addToDeckWarningHeader;

  [DbfField("ADD_TO_DECK_WARNING_BODY")]
  public DbfLocValue AddToDeckWarningBody => this.m_addToDeckWarningBody;

  public override object GetVar(string name)
  {
    if (name == "ID")
      return (object) this.ID;
    if (name == "CARD_ID")
      return (object) this.m_cardId;
    if (name == "HERO_CARD_ID")
      return (object) this.m_heroCardId;
    if (name == "DECK_NAME")
      return (object) this.m_deckName;
    if (name == "ADD_TO_DECK_WARNING_HEADER")
      return (object) this.m_addToDeckWarningHeader;
    return name == "ADD_TO_DECK_WARNING_BODY" ? (object) this.m_addToDeckWarningBody : (object) null;
  }

  public override void SetVar(string name, object val)
  {
    if (!(name == "ID"))
    {
      if (!(name == "CARD_ID"))
      {
        if (!(name == "HERO_CARD_ID"))
        {
          if (!(name == "DECK_NAME"))
          {
            if (!(name == "ADD_TO_DECK_WARNING_HEADER"))
            {
              if (!(name == "ADD_TO_DECK_WARNING_BODY"))
                return;
              this.m_addToDeckWarningBody = (DbfLocValue) val;
            }
            else
              this.m_addToDeckWarningHeader = (DbfLocValue) val;
          }
          else
            this.m_deckName = (DbfLocValue) val;
        }
        else
          this.m_heroCardId = (int) val;
      }
      else
        this.m_cardId = (int) val;
    }
    else
      this.SetID((int) val);
  }

  public override System.Type GetVarType(string name)
  {
    if (name == "ID")
      return typeof (int);
    if (name == "CARD_ID")
      return typeof (int);
    if (name == "HERO_CARD_ID")
      return typeof (int);
    if (name == "DECK_NAME")
      return typeof (DbfLocValue);
    if (name == "ADD_TO_DECK_WARNING_HEADER")
      return typeof (DbfLocValue);
    return name == "ADD_TO_DECK_WARNING_BODY" ? typeof (DbfLocValue) : (System.Type) null;
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardPlayerDeckOverrideDbfRecords loadRecords = new LoadCardPlayerDeckOverrideDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardPlayerDeckOverrideDbfAsset overrideDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardPlayerDeckOverrideDbfAsset)) as CardPlayerDeckOverrideDbfAsset;
    if ((UnityEngine.Object) overrideDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardPlayerDeckOverrideDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < overrideDbfAsset.Records.Count; ++index)
      overrideDbfAsset.Records[index].StripUnusedLocales();
    records = overrideDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_deckName.StripUnusedLocales();
    this.m_addToDeckWarningHeader.StripUnusedLocales();
    this.m_addToDeckWarningBody.StripUnusedLocales();
  }
}
