using Assets;
using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckTemplateDbfRecord : DbfRecord
{
  [SerializeField]
  private int m_classId;
  [SerializeField]
  private SpecialEventType m_event = SpecialEventType.UNKNOWN;
  [SerializeField]
  private int m_sortOrder;
  [SerializeField]
  private int m_deckId;
  [SerializeField]
  private string m_displayTexture;
  [SerializeField]
  private bool m_isFreeReward;
  [SerializeField]
  private bool m_isStarterDeck;
  [SerializeField]
  private DeckTemplate.FormatType m_formatType = DeckTemplate.FormatType.FT_STANDARD;
  [SerializeField]
  private int m_displayCardId;

  [DbfField("CLASS_ID")]
  public int ClassId => this.m_classId;

  [DbfField("EVENT")]
  public SpecialEventType Event => this.m_event;

  [DbfField("SORT_ORDER")]
  public int SortOrder => this.m_sortOrder;

  [DbfField("DECK_ID")]
  public int DeckId => this.m_deckId;

  public DeckDbfRecord DeckRecord => GameDbf.Deck.GetRecord(this.m_deckId);

  [DbfField("DISPLAY_TEXTURE")]
  public string DisplayTexture => this.m_displayTexture;

  [DbfField("IS_FREE_REWARD")]
  public bool IsFreeReward => this.m_isFreeReward;

  [DbfField("IS_STARTER_DECK")]
  public bool IsStarterDeck => this.m_isStarterDeck;

  [DbfField("FORMAT_TYPE")]
  public DeckTemplate.FormatType FormatType => this.m_formatType;

  [DbfField("DISPLAY_CARD_ID")]
  public int DisplayCardId => this.m_displayCardId;

  public List<DkRuneListDbfRecord> DKRunes
  {
    get
    {
      int id = this.ID;
      List<DkRuneListDbfRecord> dkRunes = new List<DkRuneListDbfRecord>();
      List<DkRuneListDbfRecord> records = GameDbf.DkRuneList.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        DkRuneListDbfRecord runeListDbfRecord = records[index];
        if (runeListDbfRecord.DeckTemplateId == id)
          dkRunes.Add(runeListDbfRecord);
      }
      return dkRunes;
    }
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "CLASS_ID":
        return (object) this.m_classId;
      case "DECK_ID":
        return (object) this.m_deckId;
      case "DISPLAY_CARD_ID":
        return (object) this.m_displayCardId;
      case "DISPLAY_TEXTURE":
        return (object) this.m_displayTexture;
      case "EVENT":
        return (object) this.m_event;
      case "FORMAT_TYPE":
        return (object) this.m_formatType;
      case "ID":
        return (object) this.ID;
      case "IS_FREE_REWARD":
        return (object) this.m_isFreeReward;
      case "IS_STARTER_DECK":
        return (object) this.m_isStarterDeck;
      case "SORT_ORDER":
        return (object) this.m_sortOrder;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 96691247:
        if (!(name == "FORMAT_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_formatType = DeckTemplate.FormatType.FT_UNKNOWN;
            return;
          case DeckTemplate.FormatType _:
          case int _:
            this.m_formatType = (DeckTemplate.FormatType) val;
            return;
          case string _:
            this.m_formatType = DeckTemplate.ParseFormatTypeValue((string) val);
            return;
          default:
            return;
        }
      case 236776447:
        if (!(name == "EVENT"))
          break;
        this.m_event = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 281873083:
        if (!(name == "IS_STARTER_DECK"))
          break;
        this.m_isStarterDeck = (bool) val;
        break;
      case 771121008:
        if (!(name == "DECK_ID"))
          break;
        this.m_deckId = (int) val;
        break;
      case 1458105184:
        if (!(name == "ID"))
          break;
        this.SetID((int) val);
        break;
      case 2452245441:
        if (!(name == "DISPLAY_TEXTURE"))
          break;
        this.m_displayTexture = (string) val;
        break;
      case 3420453062:
        if (!(name == "IS_FREE_REWARD"))
          break;
        this.m_isFreeReward = (bool) val;
        break;
      case 4214602626:
        if (!(name == "SORT_ORDER"))
          break;
        this.m_sortOrder = (int) val;
        break;
      case 4257872637:
        if (!(name == "CLASS_ID"))
          break;
        this.m_classId = (int) val;
        break;
      case 4274360108:
        if (!(name == "DISPLAY_CARD_ID"))
          break;
        this.m_displayCardId = (int) val;
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "CLASS_ID":
        return typeof (int);
      case "DECK_ID":
        return typeof (int);
      case "DISPLAY_CARD_ID":
        return typeof (int);
      case "DISPLAY_TEXTURE":
        return typeof (string);
      case "EVENT":
        return typeof (string);
      case "FORMAT_TYPE":
        return typeof (DeckTemplate.FormatType);
      case "ID":
        return typeof (int);
      case "IS_FREE_REWARD":
        return typeof (bool);
      case "IS_STARTER_DECK":
        return typeof (bool);
      case "SORT_ORDER":
        return typeof (int);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadDeckTemplateDbfRecords loadRecords = new LoadDeckTemplateDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    DeckTemplateDbfAsset templateDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (DeckTemplateDbfAsset)) as DeckTemplateDbfAsset;
    if ((UnityEngine.Object) templateDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("DeckTemplateDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < templateDbfAsset.Records.Count; ++index)
      templateDbfAsset.Records[index].StripUnusedLocales();
    records = templateDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
  }
}
