using Blizzard.T5.Jobs;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CardDbfRecord : DbfRecord
{
  [SerializeField]
  private string m_noteMiniGuid;
  [SerializeField]
  private DbfLocValue m_textInHand;
  [SerializeField]
  private SpecialEventType m_gameplayEvent = SpecialEventType.UNKNOWN;
  [SerializeField]
  private SpecialEventType m_craftingEvent = SpecialEventType.UNKNOWN;
  [SerializeField]
  private SpecialEventType m_goldenCraftingEvent = SpecialEventType.UNKNOWN;
  [SerializeField]
  private int m_suggestionWeight;
  [SerializeField]
  private int m_changeVersion;
  [SerializeField]
  private DbfLocValue m_name;
  [SerializeField]
  private DbfLocValue m_flavorText;
  [SerializeField]
  private DbfLocValue m_howToGetCard;
  [SerializeField]
  private DbfLocValue m_howToGetGoldCard;
  [SerializeField]
  private DbfLocValue m_howToGetSignatureCard;
  [SerializeField]
  private DbfLocValue m_howToGetDiamondCard;
  [SerializeField]
  private DbfLocValue m_targetArrowText;
  [SerializeField]
  private string m_artistName;
  [SerializeField]
  private string m_signatureArtistName;
  [SerializeField]
  private DbfLocValue m_shortName;
  [SerializeField]
  private string m_creditsCardName;
  [SerializeField]
  private SpecialEventType m_featuredCardsEvent = SpecialEventType.UNKNOWN;
  [SerializeField]
  private SpecialEventType m_battlegroundsActiveEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("always");
  [SerializeField]
  private SpecialEventType m_battlegroundsEarlyAccessEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("never");
  [SerializeField]
  private SpecialEventType m_battlegroundsEveryGameEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent("never");
  [SerializeField]
  private Assets.Card.CardTextBuilderType m_cardTextBuilderType;
  [SerializeField]
  private string m_watermarkTextureOverride;

  [DbfField("NOTE_MINI_GUID")]
  public string NoteMiniGuid => this.m_noteMiniGuid;

  [DbfField("TEXT_IN_HAND")]
  public DbfLocValue TextInHand => this.m_textInHand;

  [DbfField("GAMEPLAY_EVENT")]
  public SpecialEventType GameplayEvent => this.m_gameplayEvent;

  [DbfField("CRAFTING_EVENT")]
  public SpecialEventType CraftingEvent => this.m_craftingEvent;

  [DbfField("GOLDEN_CRAFTING_EVENT")]
  public SpecialEventType GoldenCraftingEvent => this.m_goldenCraftingEvent;

  [DbfField("SUGGESTION_WEIGHT")]
  public int SuggestionWeight => this.m_suggestionWeight;

  [DbfField("NAME")]
  public DbfLocValue Name => this.m_name;

  [DbfField("FLAVOR_TEXT")]
  public DbfLocValue FlavorText => this.m_flavorText;

  [DbfField("HOW_TO_GET_CARD")]
  public DbfLocValue HowToGetCard => this.m_howToGetCard;

  [DbfField("HOW_TO_GET_GOLD_CARD")]
  public DbfLocValue HowToGetGoldCard => this.m_howToGetGoldCard;

  [DbfField("HOW_TO_GET_SIGNATURE_CARD")]
  public DbfLocValue HowToGetSignatureCard => this.m_howToGetSignatureCard;

  [DbfField("HOW_TO_GET_DIAMOND_CARD")]
  public DbfLocValue HowToGetDiamondCard => this.m_howToGetDiamondCard;

  [DbfField("TARGET_ARROW_TEXT")]
  public DbfLocValue TargetArrowText => this.m_targetArrowText;

  [DbfField("ARTIST_NAME")]
  public string ArtistName => this.m_artistName;

  [DbfField("SIGNATURE_ARTIST_NAME")]
  public string SignatureArtistName => this.m_signatureArtistName;

  [DbfField("SHORT_NAME")]
  public DbfLocValue ShortName => this.m_shortName;

  [DbfField("CREDITS_CARD_NAME")]
  public string CreditsCardName => this.m_creditsCardName;

  [DbfField("FEATURED_CARDS_EVENT")]
  public SpecialEventType FeaturedCardsEvent => this.m_featuredCardsEvent;

  [DbfField("BATTLEGROUNDS_ACTIVE_EVENT")]
  public SpecialEventType BattlegroundsActiveEvent => this.m_battlegroundsActiveEvent;

  [DbfField("BATTLEGROUNDS_EARLY_ACCESS_EVENT")]
  public SpecialEventType BattlegroundsEarlyAccessEvent => this.m_battlegroundsEarlyAccessEvent;

  [DbfField("CARD_TEXT_BUILDER_TYPE")]
  public Assets.Card.CardTextBuilderType CardTextBuilderType => this.m_cardTextBuilderType;

  [DbfField("WATERMARK_TEXTURE_OVERRIDE")]
  public string WatermarkTextureOverride => this.m_watermarkTextureOverride;

  public CardHeroDbfRecord CardHero
  {
    get
    {
      int id = this.ID;
      List<CardHeroDbfRecord> records = GameDbf.CardHero.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        CardHeroDbfRecord cardHero = records[index];
        if (cardHero.CardId == id)
          return cardHero;
      }
      return (CardHeroDbfRecord) null;
    }
  }

  public List<CardAdditonalSearchTermsDbfRecord> SearchTerms
  {
    get
    {
      int id = this.ID;
      List<CardAdditonalSearchTermsDbfRecord> searchTerms = new List<CardAdditonalSearchTermsDbfRecord>();
      List<CardAdditonalSearchTermsDbfRecord> records = GameDbf.CardAdditonalSearchTerms.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        CardAdditonalSearchTermsDbfRecord searchTermsDbfRecord = records[index];
        if (searchTermsDbfRecord.CardId == id)
          searchTerms.Add(searchTermsDbfRecord);
      }
      return searchTerms;
    }
  }

  public List<CardEquipmentAltTextDbfRecord> EquipmentAltText
  {
    get
    {
      int id = this.ID;
      List<CardEquipmentAltTextDbfRecord> equipmentAltText = new List<CardEquipmentAltTextDbfRecord>();
      List<CardEquipmentAltTextDbfRecord> records = GameDbf.CardEquipmentAltText.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        CardEquipmentAltTextDbfRecord altTextDbfRecord = records[index];
        if (altTextDbfRecord.CardId == id)
          equipmentAltText.Add(altTextDbfRecord);
      }
      return equipmentAltText;
    }
  }

  public List<CardSetTimingDbfRecord> CardSetTimings
  {
    get
    {
      int id = this.ID;
      List<CardSetTimingDbfRecord> cardSetTimings = new List<CardSetTimingDbfRecord>();
      List<CardSetTimingDbfRecord> records = GameDbf.CardSetTiming.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        CardSetTimingDbfRecord setTimingDbfRecord = records[index];
        if (setTimingDbfRecord.CardId == id)
          cardSetTimings.Add(setTimingDbfRecord);
      }
      return cardSetTimings;
    }
  }

  public List<CardTagDbfRecord> Tags
  {
    get
    {
      int id = this.ID;
      List<CardTagDbfRecord> tags = new List<CardTagDbfRecord>();
      List<CardTagDbfRecord> records = GameDbf.CardTag.GetRecords();
      int index = 0;
      for (int count = records.Count; index < count; ++index)
      {
        CardTagDbfRecord cardTagDbfRecord = records[index];
        if (cardTagDbfRecord.CardId == id)
          tags.Add(cardTagDbfRecord);
      }
      return tags;
    }
  }

  public void SetNoteMiniGuid(string v) => this.m_noteMiniGuid = v;

  public void SetTextInHand(DbfLocValue v)
  {
    this.m_textInHand = v;
    v.SetDebugInfo(this.ID, "TEXT_IN_HAND");
  }

  public void SetName(DbfLocValue v)
  {
    this.m_name = v;
    v.SetDebugInfo(this.ID, "NAME");
  }

  public override object GetVar(string name)
  {
    switch (name)
    {
      case "ARTIST_NAME":
        return (object) this.m_artistName;
      case "BATTLEGROUNDS_ACTIVE_EVENT":
        return (object) this.m_battlegroundsActiveEvent;
      case "BATTLEGROUNDS_EARLY_ACCESS_EVENT":
        return (object) this.m_battlegroundsEarlyAccessEvent;
      case "BATTLEGROUNDS_EVERY_GAME_EVENT":
        return (object) this.m_battlegroundsEveryGameEvent;
      case "CARD_TEXT_BUILDER_TYPE":
        return (object) this.m_cardTextBuilderType;
      case "CHANGE_VERSION":
        return (object) this.m_changeVersion;
      case "CRAFTING_EVENT":
        return (object) this.m_craftingEvent;
      case "CREDITS_CARD_NAME":
        return (object) this.m_creditsCardName;
      case "FEATURED_CARDS_EVENT":
        return (object) this.m_featuredCardsEvent;
      case "FLAVOR_TEXT":
        return (object) this.m_flavorText;
      case "GAMEPLAY_EVENT":
        return (object) this.m_gameplayEvent;
      case "GOLDEN_CRAFTING_EVENT":
        return (object) this.m_goldenCraftingEvent;
      case "HOW_TO_GET_CARD":
        return (object) this.m_howToGetCard;
      case "HOW_TO_GET_DIAMOND_CARD":
        return (object) this.m_howToGetDiamondCard;
      case "HOW_TO_GET_GOLD_CARD":
        return (object) this.m_howToGetGoldCard;
      case "HOW_TO_GET_SIGNATURE_CARD":
        return (object) this.m_howToGetSignatureCard;
      case "ID":
        return (object) this.ID;
      case "NAME":
        return (object) this.m_name;
      case "NOTE_MINI_GUID":
        return (object) this.m_noteMiniGuid;
      case "SHORT_NAME":
        return (object) this.m_shortName;
      case "SIGNATURE_ARTIST_NAME":
        return (object) this.m_signatureArtistName;
      case "SUGGESTION_WEIGHT":
        return (object) this.m_suggestionWeight;
      case "TARGET_ARROW_TEXT":
        return (object) this.m_targetArrowText;
      case "TEXT_IN_HAND":
        return (object) this.m_textInHand;
      case "WATERMARK_TEXTURE_OVERRIDE":
        return (object) this.m_watermarkTextureOverride;
      default:
        return (object) null;
    }
  }

  public override void SetVar(string name, object val)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(name))
    {
      case 605406857:
        if (!(name == "FLAVOR_TEXT"))
          break;
        this.m_flavorText = (DbfLocValue) val;
        break;
      case 752250442:
        if (!(name == "HOW_TO_GET_GOLD_CARD"))
          break;
        this.m_howToGetGoldCard = (DbfLocValue) val;
        break;
      case 889535395:
        if (!(name == "BATTLEGROUNDS_ACTIVE_EVENT"))
          break;
        this.m_battlegroundsActiveEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 1015438284:
        if (!(name == "FEATURED_CARDS_EVENT"))
          break;
        this.m_featuredCardsEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 1278449607:
        if (!(name == "BATTLEGROUNDS_EVERY_GAME_EVENT"))
          break;
        this.m_battlegroundsEveryGameEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
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
      case 1667820126:
        if (!(name == "SUGGESTION_WEIGHT"))
          break;
        this.m_suggestionWeight = (int) val;
        break;
      case 1677273194:
        if (!(name == "GAMEPLAY_EVENT"))
          break;
        this.m_gameplayEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 2225759764:
        if (!(name == "HOW_TO_GET_SIGNATURE_CARD"))
          break;
        this.m_howToGetSignatureCard = (DbfLocValue) val;
        break;
      case 2635681263:
        if (!(name == "HOW_TO_GET_CARD"))
          break;
        this.m_howToGetCard = (DbfLocValue) val;
        break;
      case 2694405912:
        if (!(name == "ARTIST_NAME"))
          break;
        this.m_artistName = (string) val;
        break;
      case 2741045532:
        if (!(name == "TARGET_ARROW_TEXT"))
          break;
        this.m_targetArrowText = (DbfLocValue) val;
        break;
      case 2829424697:
        if (!(name == "SIGNATURE_ARTIST_NAME"))
          break;
        this.m_signatureArtistName = (string) val;
        break;
      case 2914581156:
        if (!(name == "GOLDEN_CRAFTING_EVENT"))
          break;
        this.m_goldenCraftingEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 3023772136:
        if (!(name == "CARD_TEXT_BUILDER_TYPE"))
          break;
        switch (val)
        {
          case null:
            this.m_cardTextBuilderType = Assets.Card.CardTextBuilderType.DEFAULT;
            return;
          case Assets.Card.CardTextBuilderType _:
          case int _:
            this.m_cardTextBuilderType = (Assets.Card.CardTextBuilderType) val;
            return;
          case string _:
            this.m_cardTextBuilderType = Assets.Card.ParseCardTextBuilderTypeValue((string) val);
            return;
          default:
            return;
        }
      case 3226467965:
        if (!(name == "SHORT_NAME"))
          break;
        this.m_shortName = (DbfLocValue) val;
        break;
      case 3336689320:
        if (!(name == "CRAFTING_EVENT"))
          break;
        this.m_craftingEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
      case 3593298050:
        if (!(name == "CREDITS_CARD_NAME"))
          break;
        this.m_creditsCardName = (string) val;
        break;
      case 3632971787:
        if (!(name == "NOTE_MINI_GUID"))
          break;
        this.m_noteMiniGuid = (string) val;
        break;
      case 3794169416:
        if (!(name == "TEXT_IN_HAND"))
          break;
        this.m_textInHand = (DbfLocValue) val;
        break;
      case 3802577366:
        if (!(name == "HOW_TO_GET_DIAMOND_CARD"))
          break;
        this.m_howToGetDiamondCard = (DbfLocValue) val;
        break;
      case 4085651046:
        if (!(name == "WATERMARK_TEXTURE_OVERRIDE"))
          break;
        this.m_watermarkTextureOverride = (string) val;
        break;
      case 4209797098:
        if (!(name == "CHANGE_VERSION"))
          break;
        this.m_changeVersion = (int) val;
        break;
      case 4244237467:
        if (!(name == "BATTLEGROUNDS_EARLY_ACCESS_EVENT"))
          break;
        this.m_battlegroundsEarlyAccessEvent = DbfShared.GetEventMap().ConvertStringToSpecialEvent((string) val);
        break;
    }
  }

  public override System.Type GetVarType(string name)
  {
    switch (name)
    {
      case "ARTIST_NAME":
        return typeof (string);
      case "BATTLEGROUNDS_ACTIVE_EVENT":
        return typeof (string);
      case "BATTLEGROUNDS_EARLY_ACCESS_EVENT":
        return typeof (string);
      case "BATTLEGROUNDS_EVERY_GAME_EVENT":
        return typeof (string);
      case "CARD_TEXT_BUILDER_TYPE":
        return typeof (Assets.Card.CardTextBuilderType);
      case "CHANGE_VERSION":
        return typeof (int);
      case "CRAFTING_EVENT":
        return typeof (string);
      case "CREDITS_CARD_NAME":
        return typeof (string);
      case "FEATURED_CARDS_EVENT":
        return typeof (string);
      case "FLAVOR_TEXT":
        return typeof (DbfLocValue);
      case "GAMEPLAY_EVENT":
        return typeof (string);
      case "GOLDEN_CRAFTING_EVENT":
        return typeof (string);
      case "HOW_TO_GET_CARD":
        return typeof (DbfLocValue);
      case "HOW_TO_GET_DIAMOND_CARD":
        return typeof (DbfLocValue);
      case "HOW_TO_GET_GOLD_CARD":
        return typeof (DbfLocValue);
      case "HOW_TO_GET_SIGNATURE_CARD":
        return typeof (DbfLocValue);
      case "ID":
        return typeof (int);
      case "NAME":
        return typeof (DbfLocValue);
      case "NOTE_MINI_GUID":
        return typeof (string);
      case "SHORT_NAME":
        return typeof (DbfLocValue);
      case "SIGNATURE_ARTIST_NAME":
        return typeof (string);
      case "SUGGESTION_WEIGHT":
        return typeof (int);
      case "TARGET_ARROW_TEXT":
        return typeof (DbfLocValue);
      case "TEXT_IN_HAND":
        return typeof (DbfLocValue);
      case "WATERMARK_TEXTURE_OVERRIDE":
        return typeof (string);
      default:
        return (System.Type) null;
    }
  }

  public override IEnumerator<IAsyncJobResult> Job_LoadRecordsFromAssetAsync<T>(
    string resourcePath,
    Action<List<T>> resultHandler)
  {
    LoadCardDbfRecords loadRecords = new LoadCardDbfRecords(resourcePath);
    yield return (IAsyncJobResult) loadRecords;
    if (resultHandler != null)
      resultHandler(loadRecords.GetRecords() as List<T>);
  }

  public override bool LoadRecordsFromAsset<T>(string resourcePath, out List<T> records)
  {
    CardDbfAsset cardDbfAsset = DbfShared.GetAssetBundle().LoadAsset(resourcePath, typeof (CardDbfAsset)) as CardDbfAsset;
    if ((UnityEngine.Object) cardDbfAsset == (UnityEngine.Object) null)
    {
      records = new List<T>();
      Debug.LogError((object) string.Format("CardDbfAsset.LoadRecordsFromAsset() - failed to load records from assetbundle: {0}", (object) resourcePath));
      return false;
    }
    for (int index = 0; index < cardDbfAsset.Records.Count; ++index)
      cardDbfAsset.Records[index].StripUnusedLocales();
    records = cardDbfAsset.Records as List<T>;
    return true;
  }

  public override bool SaveRecordsToAsset<T>(string assetPath, List<T> records, Locale locale) => false;

  public override void StripUnusedLocales()
  {
    this.m_textInHand.StripUnusedLocales();
    this.m_name.StripUnusedLocales();
    this.m_flavorText.StripUnusedLocales();
    this.m_howToGetCard.StripUnusedLocales();
    this.m_howToGetGoldCard.StripUnusedLocales();
    this.m_howToGetSignatureCard.StripUnusedLocales();
    this.m_howToGetDiamondCard.StripUnusedLocales();
    this.m_targetArrowText.StripUnusedLocales();
    this.m_shortName.StripUnusedLocales();
  }
}
