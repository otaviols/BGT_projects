using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.DungeonCrawl;
using Hearthstone.Login;
using PegasusGame;
using PegasusLettuce;
using PegasusShared;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GameUtils
{
  public static GameUtils.StringEvent OnAnimationExitEvent = new GameUtils.StringEvent();
  public static readonly TAG_CLASS[] ORDERED_HERO_CLASSES = new TAG_CLASS[11]
  {
    TAG_CLASS.DEATHKNIGHT,
    TAG_CLASS.DEMONHUNTER,
    TAG_CLASS.DRUID,
    TAG_CLASS.HUNTER,
    TAG_CLASS.MAGE,
    TAG_CLASS.PALADIN,
    TAG_CLASS.PRIEST,
    TAG_CLASS.ROGUE,
    TAG_CLASS.SHAMAN,
    TAG_CLASS.WARLOCK,
    TAG_CLASS.WARRIOR
  };
  public static readonly TAG_CLASS[] DEFAULT_HERO_CLASSES = new TAG_CLASS[10]
  {
    TAG_CLASS.DEMONHUNTER,
    TAG_CLASS.DRUID,
    TAG_CLASS.HUNTER,
    TAG_CLASS.MAGE,
    TAG_CLASS.PALADIN,
    TAG_CLASS.PRIEST,
    TAG_CLASS.ROGUE,
    TAG_CLASS.SHAMAN,
    TAG_CLASS.WARLOCK,
    TAG_CLASS.WARRIOR
  };
  public static readonly TAG_CLASS[] CLASSIC_ORDERED_HERO_CLASSES = new TAG_CLASS[9]
  {
    TAG_CLASS.DRUID,
    TAG_CLASS.HUNTER,
    TAG_CLASS.MAGE,
    TAG_CLASS.PALADIN,
    TAG_CLASS.PRIEST,
    TAG_CLASS.ROGUE,
    TAG_CLASS.SHAMAN,
    TAG_CLASS.WARLOCK,
    TAG_CLASS.WARRIOR
  };
  public static readonly Dictionary<TAG_CLASS, GameUtils.HeroSkinAchievements> HERO_SKIN_ACHIEVEMENTS = new Dictionary<TAG_CLASS, GameUtils.HeroSkinAchievements>()
  {
    {
      TAG_CLASS.MAGE,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 179,
        Honored1kWin = 180
      }
    },
    {
      TAG_CLASS.PRIEST,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 196,
        Honored1kWin = 197
      }
    },
    {
      TAG_CLASS.WARLOCK,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 213,
        Honored1kWin = 214
      }
    },
    {
      TAG_CLASS.ROGUE,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 230,
        Honored1kWin = 231
      }
    },
    {
      TAG_CLASS.DRUID,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 247,
        Honored1kWin = 248
      }
    },
    {
      TAG_CLASS.DEMONHUNTER,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 264,
        Honored1kWin = 265
      }
    },
    {
      TAG_CLASS.DEATHKNIGHT,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 5520,
        Honored1kWin = 5521
      }
    },
    {
      TAG_CLASS.SHAMAN,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 281,
        Honored1kWin = 282
      }
    },
    {
      TAG_CLASS.HUNTER,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 298,
        Honored1kWin = 299
      }
    },
    {
      TAG_CLASS.PALADIN,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 315,
        Honored1kWin = 316
      }
    },
    {
      TAG_CLASS.WARRIOR,
      new GameUtils.HeroSkinAchievements()
      {
        Golden500Win = 332,
        Honored1kWin = 333
      }
    }
  };
  private static ReactiveNetCacheObject<NetCache.NetCacheProfileProgress> s_profileProgress = ReactiveNetCacheObject<NetCache.NetCacheProfileProgress>.CreateInstance();
  private static Comparison<BoosterDbfRecord> SortBoostersDescending = (Comparison<BoosterDbfRecord>) ((a, b) => b.LatestExpansionOrder.CompareTo(a.LatestExpansionOrder));
  private const int RANKED_SEASON_ID_START = 6;
  private const int RANKED_SEASON_MONTH_START = 4;
  private const int RANKED_SEASON_YEAR_START = 2014;
  private const int MERCENARIES_SEASON_ID_START = 1;
  private const int MERCENARIES_SEASON_MONTH_START = 11;
  private const int MERCENARIES_SEASON_YEAR_START = 2021;

  public static string TranslateDbIdToCardId(int dbId, bool showWarning = false)
  {
    CardDbfRecord record = GameDbf.Card.GetRecord(dbId);
    if (record == null)
    {
      if (showWarning)
        Log.All.PrintError("GameUtils.TranslateDbIdToCardId() - Failed to find card with database id {0} in the Card DBF.", (object) dbId);
      return (string) null;
    }
    string noteMiniGuid = record.NoteMiniGuid;
    if (noteMiniGuid != null)
      return noteMiniGuid;
    if (showWarning)
      Log.All.PrintError("GameUtils.TranslateDbIdToCardId() - Card with database id {0} has no NOTE_MINI_GUID field in the Card DBF.", (object) dbId);
    return (string) null;
  }

  public static int TranslateCardIdToDbId(string cardId, bool showWarning = false)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardId);
    if (cardRecord != null)
      return cardRecord.ID;
    if (showWarning)
      Log.All.PrintError("GameUtils.TranslateCardIdToDbId() - There is no card with NOTE_MINI_GUID {0} in the Card DBF.", (object) cardId);
    return 0;
  }

  public static bool IsCardCollectible(string cardId) => GameUtils.GetCardTagValue(cardId, GAME_TAG.COLLECTIBLE) == 1;

  public static bool IsCardInBattlegroundsPool(string cardId) => GameUtils.GetCardTagValue(cardId, GAME_TAG.IS_BACON_POOL_MINION) == 1 || GameUtils.GetCardTagValue(cardId, GAME_TAG.BACON_HERO_CAN_BE_DRAFTED) == 1;

  public static bool IsAdventureRotated(AdventureDbId adventureID) => GameUtils.IsAdventureRotated(adventureID, DateTime.UtcNow);

  public static bool IsAdventureRotated(AdventureDbId adventureID, DateTime utcTimestamp)
  {
    AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) adventureID);
    return record != null && !SpecialEventManager.Get().IsEventActive(record.StandardEvent, false, utcTimestamp);
  }

  public static bool IsBoosterRotated(BoosterDbId boosterID, DateTime utcTimestamp)
  {
    BoosterDbfRecord record = GameDbf.Booster.GetRecord((int) boosterID);
    return record != null && !SpecialEventManager.Get().IsEventActive(record.StandardEvent, false, utcTimestamp);
  }

  public static PegasusShared.FormatType GetCardSetFormat(TAG_CARD_SET cardSet)
  {
    if (cardSet == TAG_CARD_SET.VANILLA)
      return PegasusShared.FormatType.FT_CLASSIC;
    return GameUtils.IsSetRotated(cardSet) ? PegasusShared.FormatType.FT_WILD : PegasusShared.FormatType.FT_STANDARD;
  }

  public static TAG_CARD_SET[] GetCardSetsInFormat(PegasusShared.FormatType formatType)
  {
    TAG_CARD_SET[] cardSetsInFormat = (TAG_CARD_SET[]) null;
    switch (formatType)
    {
      case PegasusShared.FormatType.FT_WILD:
        cardSetsInFormat = GameUtils.GetAllWildPlayableSets();
        break;
      case PegasusShared.FormatType.FT_STANDARD:
        cardSetsInFormat = GameUtils.GetStandardSets();
        break;
      case PegasusShared.FormatType.FT_CLASSIC:
        cardSetsInFormat = GameUtils.GetClassicSets();
        break;
    }
    return cardSetsInFormat;
  }

  public static bool IsCardSetValidForFormat(PegasusShared.FormatType formatType, TAG_CARD_SET cardSet)
  {
    switch (formatType)
    {
      case PegasusShared.FormatType.FT_WILD:
        return GameUtils.IsWildCardSet(cardSet) || GameUtils.IsStandardCardSet(cardSet);
      case PegasusShared.FormatType.FT_STANDARD:
        return GameUtils.IsStandardCardSet(cardSet);
      case PegasusShared.FormatType.FT_CLASSIC:
        return GameUtils.IsClassicCardSet(cardSet);
      default:
        return false;
    }
  }

  public static bool IsCardValidForFormat(PegasusShared.FormatType formatType, int cardDbId)
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardDbId);
    return GameUtils.IsCardValidForFormat(formatType, entityDef);
  }

  public static bool IsCardValidForFormat(PegasusShared.FormatType formatType, string cardId)
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
    return GameUtils.IsCardValidForFormat(formatType, entityDef);
  }

  public static bool IsCardValidForFormat(PegasusShared.FormatType formatType, EntityDef def) => def != null && GameUtils.IsCardSetValidForFormat(formatType, def.GetCardSet());

  public static bool IsWildCardSet(TAG_CARD_SET cardSet) => GameUtils.GetCardSetFormat(cardSet) == PegasusShared.FormatType.FT_WILD;

  public static bool IsWildCard(int cardDbId) => GameUtils.IsWildCard(DefLoader.Get().GetEntityDef(cardDbId));

  public static bool IsWildCard(string cardId) => GameUtils.IsWildCard(DefLoader.Get().GetEntityDef(cardId));

  public static bool IsWildCard(EntityDef def) => def != null && GameUtils.IsWildCardSet(def.GetCardSet());

  public static bool IsClassicCardSet(TAG_CARD_SET cardSet) => GameUtils.GetCardSetFormat(cardSet) == PegasusShared.FormatType.FT_CLASSIC;

  public static bool IsClassicCard(int cardDbId) => GameUtils.IsClassicCard(DefLoader.Get().GetEntityDef(cardDbId));

  public static bool IsClassicCard(string cardId) => GameUtils.IsClassicCard(DefLoader.Get().GetEntityDef(cardId));

  public static bool IsClassicCard(EntityDef def) => def != null && GameUtils.IsClassicCardSet(def.GetCardSet());

  public static bool IsCoreCard(string cardId) => GameUtils.IsCoreCard(DefLoader.Get().GetEntityDef(cardId));

  public static bool IsCoreCard(EntityDef def) => def != null && def.IsCoreCard();

  public static bool IsStandardCardSet(TAG_CARD_SET cardSet) => GameUtils.GetCardSetFormat(cardSet) == PegasusShared.FormatType.FT_STANDARD;

  public static bool IsStandardCard(int cardDbId) => GameUtils.IsStandardCard(DefLoader.Get().GetEntityDef(cardDbId));

  public static bool IsStandardCard(string cardId) => GameUtils.IsStandardCard(DefLoader.Get().GetEntityDef(cardId));

  public static bool IsStandardCard(EntityDef def) => def != null && GameUtils.IsStandardCardSet(def.GetCardSet());

  public static string GetCardSetFormatAsString(TAG_CARD_SET cardSet) => GameUtils.GetCardSetFormat(cardSet).ToString().Replace("FT_", "");

  public static bool IsSetRotated(TAG_CARD_SET set) => GameUtils.IsSetRotated(set, DateTime.UtcNow);

  public static bool IsSetRotated(TAG_CARD_SET set, DateTime utcTimestamp)
  {
    CardSetDbfRecord cardSet = GameDbf.GetIndex().GetCardSet(set);
    if (cardSet == null)
      return false;
    SpecialEventManager specialEventManager = SpecialEventManager.Get();
    return !specialEventManager.IsEventActive(cardSet.StandardEvent, false, utcTimestamp) && specialEventManager.HasEventStarted(cardSet.StandardEvent);
  }

  public static bool IsCardRotated(int cardDbId) => GameUtils.IsCardRotated(DefLoader.Get().GetEntityDef(cardDbId));

  public static bool IsCardRotated(string cardId) => GameUtils.IsCardRotated(DefLoader.Get().GetEntityDef(cardId));

  public static bool IsCardRotated(EntityDef def) => GameUtils.IsCardRotated(def, DateTime.UtcNow);

  public static bool IsCardRotated(EntityDef def, DateTime utcTimestamp) => GameUtils.IsSetRotated(def.GetCardSet(), utcTimestamp);

  public static bool IsLegacySet(TAG_CARD_SET set) => GameUtils.IsLegacySet(set, DateTime.UtcNow);

  public static bool IsLegacySet(TAG_CARD_SET set, DateTime utcTimestamp)
  {
    CardSetDbfRecord cardSet = GameDbf.GetIndex().GetCardSet(set);
    return cardSet != null && SpecialEventManager.Get().IsEventActive(cardSet.LegacyCardSetEvent, false, utcTimestamp);
  }

  public static bool IsBannedByConstructedDenylist(CollectionDeck deck, string designerCardId) => deck.IsConstructedDeck && NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().ConstructedCardDenylist.Contains(GameUtils.TranslateCardIdToDbId(designerCardId));

  public static bool IsBannedByDuelsDenylist(CollectionDeck deck, string designerCardId) => deck.IsDuelsDeck && NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().DuelsCardDenylist.Contains(GameUtils.TranslateCardIdToDbId(designerCardId));

  public static bool IsBanned(CollectionDeck deck, EntityDef def)
  {
    string cardId = def.GetCardId();
    return RankMgr.Get().IsCardBannedInCurrentLeague(def) || GameUtils.IsBannedByDuelsDenylist(deck, cardId) || GameUtils.IsBannedByConstructedDenylist(deck, cardId);
  }

  public static bool IsCardGameplayEventActive(EntityDef def) => GameUtils.IsCardGameplayEventActive(def.GetCardId());

  public static bool IsCardGameplayEventActive(string cardId)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardId);
    if (cardRecord == null)
    {
      Debug.LogWarning((object) string.Format("GameUtils.IsCardGameplayEventActive could not find DBF record for card {0}", (object) cardId));
      return false;
    }
    SpecialEventType eventType = cardRecord.GameplayEvent;
    if (eventType == SpecialEventType.UNKNOWN)
    {
      CardSetDbfRecord cardSetRecord = GameUtils.GetCardSetRecord(cardId);
      if (cardSetRecord != null)
        eventType = cardSetRecord.ContentLaunchEvent;
    }
    return SpecialEventManager.Get().IsEventActive(eventType, true);
  }

  public static bool IsCardSetFilterEventActive(string cardId)
  {
    CardSetDbfRecord cardSetRecord = GameUtils.GetCardSetRecord(cardId);
    return cardSetRecord != null && SpecialEventManager.Get().IsEventActive(cardSetRecord.SetFilterEvent, false);
  }

  public static bool IsCardCraftableWhenWild(string cardId)
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
    if (entityDef == null)
      return false;
    CardSetDbfRecord cardSet = GameDbf.GetIndex().GetCardSet(entityDef.GetCardSet());
    return cardSet != null && cardSet.CraftableWhenWild;
  }

  public static bool DeckIncludesRotatedCards(int deckId)
  {
    DeckDbfRecord record = GameDbf.Deck.GetRecord(deckId);
    if (record == null)
    {
      Log.Decks.PrintWarning("DeckRuleset.IsDeckWild(): {0} is invalid deck id", (object) deckId);
      return false;
    }
    foreach (DeckCardDbfRecord card in record.Cards)
    {
      if (GameUtils.IsCardRotated(card.CardId))
        return true;
    }
    return false;
  }

  public static TAG_CARD_SET[] GetStandardSets()
  {
    List<TAG_CARD_SET> tagCardSetList = new List<TAG_CARD_SET>();
    foreach (TAG_CARD_SET displayableCardSet in CollectionManager.Get().GetDisplayableCardSets())
    {
      if (GameUtils.GetCardSetFormat(displayableCardSet) == PegasusShared.FormatType.FT_STANDARD)
        tagCardSetList.Add(displayableCardSet);
    }
    return tagCardSetList.ToArray();
  }

  public static TAG_CARD_SET[] GetWildSets()
  {
    List<TAG_CARD_SET> tagCardSetList = new List<TAG_CARD_SET>();
    foreach (TAG_CARD_SET displayableCardSet in CollectionManager.Get().GetDisplayableCardSets())
    {
      if (GameUtils.GetCardSetFormat(displayableCardSet) == PegasusShared.FormatType.FT_WILD)
        tagCardSetList.Add(displayableCardSet);
    }
    return tagCardSetList.ToArray();
  }

  public static TAG_CARD_SET[] GetAllWildPlayableSets()
  {
    List<TAG_CARD_SET> tagCardSetList = new List<TAG_CARD_SET>();
    tagCardSetList.AddRange((IEnumerable<TAG_CARD_SET>) GameUtils.GetStandardSets());
    tagCardSetList.AddRange((IEnumerable<TAG_CARD_SET>) GameUtils.GetWildSets());
    return tagCardSetList.ToArray();
  }

  public static TAG_CARD_SET[] GetLegacySets()
  {
    List<TAG_CARD_SET> tagCardSetList = new List<TAG_CARD_SET>();
    foreach (TAG_CARD_SET displayableCardSet in CollectionManager.Get().GetDisplayableCardSets())
    {
      if (GameUtils.IsLegacySet(displayableCardSet))
        tagCardSetList.Add(displayableCardSet);
    }
    return tagCardSetList.ToArray();
  }

  public static TAG_CARD_SET[] GetClassicSets() => CollectionManager.Get().GetDisplayableCardSets().Where<TAG_CARD_SET>((Func<TAG_CARD_SET, bool>) (cardSet => GameUtils.IsClassicCardSet(cardSet))).ToArray<TAG_CARD_SET>();

  public static TAG_CLASS GetTagClassFromCardId(string cardId)
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
    return entityDef == null ? TAG_CLASS.INVALID : entityDef.GetClass();
  }

  public static TAG_CLASS GetTagClassFromCardDbId(int cardDbId) => (TAG_CLASS) GameDbf.GetIndex().GetCardTagValue(cardDbId, GAME_TAG.CLASS);

  public static int CountAllCollectibleCards() => GameDbf.GetIndex().GetCollectibleCardCount();

  public static List<string> GetAllCardIds() => GameDbf.GetIndex().GetAllCardIds();

  public static List<string> GetAllCollectibleCardIds() => GameDbf.GetIndex().GetCollectibleCardIds();

  public static List<int> GetAllCollectibleCardDbIds() => GameDbf.GetIndex().GetCollectibleCardDbIds();

  public static List<string> GetNonHeroSkinCollectibleCardIds()
  {
    List<string> collectibleCardIds = new List<string>();
    foreach (string collectibleCardId in GameUtils.GetAllCollectibleCardIds())
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(collectibleCardId);
      if (entityDef != null && !entityDef.IsHeroSkin())
        collectibleCardIds.Add(collectibleCardId);
    }
    return collectibleCardIds;
  }

  public static List<string> GetNonHeroSkinAllCardIds()
  {
    List<string> heroSkinAllCardIds = new List<string>();
    foreach (string allCardId in GameUtils.GetAllCardIds())
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(allCardId);
      if (entityDef != null && !entityDef.IsHeroSkin() && entityDef.GetCardType() != TAG_CARDTYPE.ENCHANTMENT)
        heroSkinAllCardIds.Add(allCardId);
    }
    return heroSkinAllCardIds;
  }

  public static CardDbfRecord GetCardRecord(string cardId) => cardId == null ? (CardDbfRecord) null : GameDbf.GetIndex().GetCardRecord(cardId);

  public static CardSetDbfRecord GetCardSetRecord(string cardId) => GameUtils.GetCardSetRecord(GameUtils.GetCardSetFromCardID(cardId));

  public static CardSetDbfRecord GetCardSetRecord(TAG_CARD_SET cardSetId) => cardSetId == TAG_CARD_SET.INVALID ? (CardSetDbfRecord) null : GameDbf.GetIndex().GetCardSet(cardSetId);

  public static List<CardChangeDbfRecord> GetCardChangeRecords(string cardId)
  {
    if (cardId == null)
      return (List<CardChangeDbfRecord>) null;
    int dbId = GameUtils.TranslateCardIdToDbId(cardId);
    return GameDbf.GetIndex().GetCardChangeRecords(dbId);
  }

  public static int GetCardTagValue(string cardId, GAME_TAG tagId)
  {
    int dbId = GameUtils.TranslateCardIdToDbId(cardId);
    return GameDbf.GetIndex().GetCardTagValue(dbId, tagId);
  }

  public static int GetCardTagValue(int cardDbId, GAME_TAG tagId) => GameDbf.GetIndex().GetCardTagValue(cardDbId, tagId);

  public static bool TryGetCardTagRecords(string cardId, out List<CardTagDbfRecord> tagDbfRecords)
  {
    int dbId = GameUtils.TranslateCardIdToDbId(cardId);
    return GameDbf.GetIndex().TryGetCardTagRecords(dbId, out tagDbfRecords);
  }

  public static string GetHeroPowerCardIdFromHero(string heroCardId)
  {
    int cardTagValue = GameUtils.GetCardTagValue(heroCardId, GAME_TAG.HERO_POWER);
    return cardTagValue == 0 ? string.Empty : GameUtils.TranslateDbIdToCardId(cardTagValue);
  }

  public static string GetHeroPowerCardIdFromHero(int heroDbId)
  {
    if (GameDbf.Card.GetRecord(heroDbId) != null)
      return GameUtils.TranslateDbIdToCardId(GameUtils.GetCardTagValue(heroDbId, GAME_TAG.HERO_POWER));
    Debug.LogError((object) string.Format("GameUtils.GetHeroPowerCardIdFromHero() - failed to find record for heroDbId {0}", (object) heroDbId));
    return string.Empty;
  }

  public static string GetCardIdFromHeroDbId(int heroDbId)
  {
    CardHeroDbfRecord record = GameDbf.CardHero.GetRecord(heroDbId);
    if (record != null)
      return GameUtils.TranslateDbIdToCardId(record.CardId);
    Debug.LogError((object) string.Format("GameUtils.GetCardIdFromHeroDbId() - failed to find record for heroDbId {0}", (object) heroDbId));
    return string.Empty;
  }

  public static TAG_CARD_SET GetCardSetFromCardID(string cardID)
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardID);
    if (entityDef != null)
      return entityDef.GetCardSet();
    Debug.LogError((object) string.Format("Null EntityDef in GetCardSetFromCardID() for {0}", (object) cardID));
    return TAG_CARD_SET.INVALID;
  }

  public static int GetCardIdFromGuestHeroDbId(int guestHeroDbId)
  {
    GuestHeroDbfRecord record = GameDbf.GuestHero.GetRecord(guestHeroDbId);
    if (record != null)
      return record.CardId;
    Debug.LogError((object) string.Format("GameUtils.GetCardIdFromGuestHeroDbId() - failed to find record for guestHeroDbId {0}", (object) guestHeroDbId));
    return 0;
  }

  public static int GetFavoriteHeroCardDBIdFromClass(TAG_CLASS classTag)
  {
    string cardId = CollectionManager.Get().GetRandomFavoriteHero(classTag)?.Name;
    if (string.IsNullOrEmpty(cardId))
      cardId = CollectionManager.GetVanillaHero(classTag);
    return GameUtils.TranslateCardIdToDbId(cardId);
  }

  public static CardHero.HeroType? GetHeroType(CardRewardData cardRewardData) => GameUtils.GetHeroType(cardRewardData.CardID);

  public static CardHero.HeroType? GetHeroType(string cardId) => GameUtils.GetHeroType(GameUtils.TranslateCardIdToDbId(cardId));

  public static CardHero.HeroType? GetHeroType(int cardDbId) => GameUtils.GetHeroType(GameDbf.Card.GetRecord(cardDbId));

  public static CardHero.HeroType? GetHeroType(CardDbfRecord cardRecord) => GameUtils.GetHeroType(cardRecord?.CardHero);

  public static CardHero.HeroType? GetHeroType(CardHeroDbfRecord heroRecord) => heroRecord?.HeroType;

  public static bool IsVanillaHero(string cardId) => GameUtils.IsVanillaHero(GameUtils.TranslateCardIdToDbId(cardId));

  public static bool IsVanillaHero(int cardDbId) => GameUtils.IsVanillaHero(GameDbf.Card.GetRecord(cardDbId));

  public static bool IsVanillaHero(CardDbfRecord cardRecord) => GameUtils.IsVanillaHero(cardRecord?.CardHero);

  public static bool IsVanillaHero(CardHeroDbfRecord heroRecord) => GameUtils.IsVanillaHero(GameUtils.GetHeroType(heroRecord));

  public static bool IsVanillaHero(CardHero.HeroType? heroType)
  {
    CardHero.HeroType? nullable = heroType;
    CardHero.HeroType heroType1 = CardHero.HeroType.VANILLA;
    return nullable.GetValueOrDefault() == heroType1 & nullable.HasValue;
  }

  public static bool IsBattlegroundsHero(string cardId) => GameUtils.IsBattlegroundsHero(GameUtils.TranslateCardIdToDbId(cardId));

  public static bool IsBattlegroundsHero(int cardDbId) => GameUtils.IsBattlegroundsHero(GameDbf.Card.GetRecord(cardDbId));

  public static bool IsBattlegroundsHero(CardDbfRecord cardRecord) => GameUtils.IsBattlegroundsHero(cardRecord?.CardHero);

  public static bool IsBattlegroundsHero(CardHeroDbfRecord heroRecord) => GameUtils.IsBattlegroundsHero(GameUtils.GetHeroType(heroRecord));

  public static bool IsBattlegroundsHero(CardHero.HeroType? heroType)
  {
    CardHero.HeroType? nullable = heroType;
    CardHero.HeroType heroType1 = CardHero.HeroType.BATTLEGROUNDS_HERO;
    return nullable.GetValueOrDefault() == heroType1 & nullable.HasValue;
  }

  public static bool IsBattlegroundsGuide(string cardId) => GameUtils.IsBattlegroundsGuide(GameUtils.TranslateCardIdToDbId(cardId));

  public static bool IsBattlegroundsGuide(int cardDbId) => GameUtils.IsBattlegroundsGuide(GameDbf.Card.GetRecord(cardDbId));

  public static bool IsBattlegroundsGuide(CardDbfRecord cardRecord) => GameUtils.IsBattlegroundsGuide(cardRecord?.CardHero);

  public static bool IsBattlegroundsGuide(CardHeroDbfRecord heroRecord) => GameUtils.IsBattlegroundsGuide(GameUtils.GetHeroType(heroRecord));

  public static bool IsBattlegroundsGuide(CardHero.HeroType? heroType)
  {
    CardHero.HeroType? nullable = heroType;
    CardHero.HeroType heroType1 = CardHero.HeroType.BATTLEGROUNDS_GUIDE;
    return nullable.GetValueOrDefault() == heroType1 & nullable.HasValue;
  }

  public static string GetGalakrondCardIdByClass(TAG_CLASS classTag)
  {
    string galakrondCardIdByClass = "";
    switch (classTag)
    {
      case TAG_CLASS.PRIEST:
        galakrondCardIdByClass = "DRG_660";
        break;
      case TAG_CLASS.ROGUE:
        galakrondCardIdByClass = "DRG_610";
        break;
      case TAG_CLASS.SHAMAN:
        galakrondCardIdByClass = "DRG_620";
        break;
      case TAG_CLASS.WARLOCK:
        galakrondCardIdByClass = "DRG_600";
        break;
      case TAG_CLASS.WARRIOR:
        galakrondCardIdByClass = "DRG_650";
        break;
    }
    return galakrondCardIdByClass;
  }

  public static NetCache.HeroLevel GetHeroLevel(TAG_CLASS heroClass)
  {
    NetCache.NetCacheHeroLevels netObject = NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>();
    if (netObject != null)
      return netObject.Levels.Find((Predicate<NetCache.HeroLevel>) (obj => obj.Class == heroClass));
    Debug.LogWarning((object) "GameUtils.GetHeroLevel() - NetCache.NetCacheHeroLevels is null");
    return (NetCache.HeroLevel) null;
  }

  public static int? GetTotalHeroLevel()
  {
    int? totalHeroLevel = new int?();
    NetCache.NetCacheHeroLevels netObject = NetCache.Get().GetNetObject<NetCache.NetCacheHeroLevels>();
    if (netObject != null)
    {
      totalHeroLevel = new int?(0);
      foreach (NetCache.HeroLevel level1 in netObject.Levels)
      {
        int? nullable = totalHeroLevel;
        int level2 = level1.CurrentLevel.Level;
        totalHeroLevel = nullable.HasValue ? new int?(nullable.GetValueOrDefault() + level2) : new int?();
      }
    }
    else
      Debug.LogError((object) "GameUtils.GetHeroLevel() - NetCache.NetCacheHeroLevels is null");
    return totalHeroLevel;
  }

  public static bool HasUnlockedClass(TAG_CLASS heroClass) => GameUtils.GetHeroLevel(heroClass) != null;

  public static int CardPremiumSortComparisonAsc(TAG_PREMIUM premium1, TAG_PREMIUM premium2) => premium1 - premium2;

  public static int CardPremiumSortComparisonDesc(TAG_PREMIUM premium1, TAG_PREMIUM premium2) => premium2 - premium1;

  public static bool CanConcedeCurrentMission()
  {
    if (GameState.Get() == null)
      return false;
    return GameMgr.Get().IsTraditionalTutorial() ? NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().SkippableTutorialEnabled && GameState.Get().GetGameEntity() is TutorialEntity gameEntity && gameEntity.IsCustomIntroFinished() : !GameMgr.Get().IsSpectator() && !GameMgr.Get().IsLettuceTutorial();
  }

  public static bool CanRestartCurrentMission(bool checkTutorial = true) => GameState.Get() != null && !GameState.Get().GetBooleanGameOption(GameEntityOption.DISABLE_RESTART_BUTTON) && (!checkTutorial || !GameMgr.Get().IsTraditionalTutorial()) && !GameMgr.Get().IsSpectator() && GameMgr.Get().IsAI() && GameMgr.Get().HasLastPlayedDeckId() && BattleNet.IsConnected() && (!DemoMgr.Get().IsDemo() || DemoMgr.Get().CanRestartMissions()) && !GameMgr.Get().IsDungeonCrawlMission();

  public static bool IsWaitingForOpponentReconnect() => GameState.Get() != null && GameState.Get().GetGameEntity().HasTag(GAME_TAG.WAIT_FOR_PLAYER_RECONNECT_PERIOD);

  public static Card GetJoustWinner(Network.HistMetaData metaData)
  {
    if (metaData == null)
      return (Card) null;
    if (metaData.MetaType != HistoryMeta.Type.JOUST)
      return (Card) null;
    return GameState.Get().GetEntity(metaData.Data)?.GetCard();
  }

  public static bool IsHistoryDeathTagChange(Network.HistTagChange tagChange)
  {
    Entity entity = GameState.Get().GetEntity(tagChange.Entity);
    return entity != null && !entity.IsEnchantment() && entity.GetCardType() != TAG_CARDTYPE.INVALID && (tagChange.Tag == 360 && tagChange.Value == 1 || entity.IsMinion() && tagChange.Tag == 49 && tagChange.Value == 4 && entity.GetZone() == TAG_ZONE.PLAY);
  }

  public static bool IsHistoryDiscardTagChange(Network.HistTagChange tagChange) => tagChange.Tag == 49 && GameState.Get().GetEntity(tagChange.Entity).GetZone() == TAG_ZONE.HAND && tagChange.Value == 4;

  public static bool IsHistoryMovedToSetAsideTagChange(Network.HistTagChange tagChange) => tagChange.Tag == 49 && tagChange.Value == 6;

  public static bool IsEntityDeathTagChange(Network.HistTagChange tagChange) => tagChange.Tag == 49 && tagChange.Value == 4 && GameState.Get().GetEntity(tagChange.Entity) != null;

  public static bool IsCharacterDeathTagChange(Network.HistTagChange tagChange)
  {
    if (tagChange.Tag != 49 || tagChange.Value != 4)
      return false;
    Entity entity = GameState.Get().GetEntity(tagChange.Entity);
    return entity != null && entity.IsCharacter();
  }

  public static bool IsPreGameOverPlayState(TAG_PLAYSTATE playState)
  {
    switch (playState)
    {
      case TAG_PLAYSTATE.WINNING:
      case TAG_PLAYSTATE.LOSING:
      case TAG_PLAYSTATE.DISCONNECTED:
      case TAG_PLAYSTATE.CONCEDED:
        return true;
      default:
        return false;
    }
  }

  public static bool IsGameOverTag(int entityId, int tag, int val) => GameUtils.IsGameOverTag(GameState.Get().GetEntity(entityId) as Player, tag, val);

  public static bool IsGameOverTag(Player player, int tag, int val)
  {
    if (player == null || tag != 17 || !player.IsFriendlySide() || !player.IsTeamLeader())
      return false;
    switch (val)
    {
      case 4:
      case 5:
      case 6:
        return true;
      default:
        return false;
    }
  }

  public static bool IsFriendlyConcede(Network.HistTagChange tagChange) => tagChange.Tag == 17 && GameState.Get().GetEntity(tagChange.Entity) is Player entity && entity.IsFriendlySide() && tagChange.Value == 8;

  public static bool IsBeginPhase(TAG_STEP step)
  {
    switch (step)
    {
      case TAG_STEP.INVALID:
      case TAG_STEP.BEGIN_FIRST:
      case TAG_STEP.BEGIN_SHUFFLE:
      case TAG_STEP.BEGIN_DRAW:
      case TAG_STEP.BEGIN_MULLIGAN:
        return true;
      default:
        return false;
    }
  }

  public static bool IsPastBeginPhase(TAG_STEP step) => !GameUtils.IsBeginPhase(step);

  public static bool IsMainPhase(TAG_STEP step)
  {
    switch (step)
    {
      case TAG_STEP.MAIN_BEGIN:
      case TAG_STEP.MAIN_READY:
      case TAG_STEP.MAIN_RESOURCE:
      case TAG_STEP.MAIN_DRAW:
      case TAG_STEP.MAIN_START:
      case TAG_STEP.MAIN_ACTION:
      case TAG_STEP.MAIN_COMBAT:
      case TAG_STEP.MAIN_END:
      case TAG_STEP.MAIN_NEXT:
      case TAG_STEP.MAIN_CLEANUP:
      case TAG_STEP.MAIN_START_TRIGGERS:
      case TAG_STEP.MAIN_SET_ACTION_STEP_TYPE:
      case TAG_STEP.MAIN_PRE_ACTION:
      case TAG_STEP.MAIN_POST_ACTION:
        return true;
      default:
        return false;
    }
  }

  public static List<Entity> GetEntitiesKilledBySourceAmongstTargets(
    int damageSourceID,
    List<Entity> targetEntities)
  {
    List<Entity> entityList = new List<Entity>();
    foreach (Entity targetEntity in targetEntities)
    {
      if (targetEntity != null)
        entityList.Add(targetEntity.CloneForZoneMgr());
    }
    List<Entity> sourceAmongstTargets = new List<Entity>();
    PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
    List<PowerTaskList> powerTaskListList = new List<PowerTaskList>();
    if (powerProcessor.GetCurrentTaskList() != null)
      powerTaskListList.Add(powerProcessor.GetCurrentTaskList());
    powerTaskListList.AddRange((IEnumerable<PowerTaskList>) powerProcessor.GetPowerQueue().GetList());
    for (int index1 = 0; index1 < powerTaskListList.Count; ++index1)
    {
      List<PowerTask> taskList = powerTaskListList[index1].GetTaskList();
      for (int index2 = 0; index2 < taskList.Count; ++index2)
      {
        Network.HistTagChange tagChange = taskList[index2].GetPower() as Network.HistTagChange;
        if (tagChange != null)
        {
          if (tagChange.Tag == 18)
            entityList.Find((Predicate<Entity>) (targetEntity => targetEntity.GetEntityId() == tagChange.Entity))?.SetTag(18, tagChange.Value);
          else if (tagChange.Tag == 49 && tagChange.Value == 4)
          {
            Entity entity = entityList.Find((Predicate<Entity>) (targetEntity => targetEntity.GetEntityId() == tagChange.Entity));
            if (entity != null && entity.GetTag(GAME_TAG.LAST_AFFECTED_BY) == damageSourceID)
              sourceAmongstTargets.Add(entity);
          }
        }
      }
    }
    return sourceAmongstTargets;
  }

  public static void ApplyPower(Entity entity, Network.PowerHistory power)
  {
    switch (power.Type)
    {
      case Network.PowerType.SHOW_ENTITY:
        GameUtils.ApplyShowEntity(entity, (Network.HistShowEntity) power);
        break;
      case Network.PowerType.HIDE_ENTITY:
        GameUtils.ApplyHideEntity(entity, (Network.HistHideEntity) power);
        break;
      case Network.PowerType.TAG_CHANGE:
        GameUtils.ApplyTagChange(entity, (Network.HistTagChange) power);
        break;
    }
  }

  public static void ApplyShowEntity(Entity entity, Network.HistShowEntity showEntity)
  {
    foreach (Network.Entity.Tag tag in showEntity.Entity.Tags)
      entity.SetTag(tag.Name, tag.Value);
  }

  public static void ApplyHideEntity(Entity entity, Network.HistHideEntity hideEntity) => entity.SetTag(GAME_TAG.ZONE, hideEntity.Zone);

  public static void ApplyTagChange(Entity entity, Network.HistTagChange tagChange) => entity.SetTag(tagChange.Tag, tagChange.Value);

  public static TAG_ZONE GetFinalZoneForEntity(Entity entity)
  {
    PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
    List<PowerTaskList> powerTaskListList = new List<PowerTaskList>();
    if (powerProcessor.GetCurrentTaskList() != null)
      powerTaskListList.Add(powerProcessor.GetCurrentTaskList());
    powerTaskListList.AddRange((IEnumerable<PowerTaskList>) powerProcessor.GetPowerQueue().GetList());
    for (int index1 = powerTaskListList.Count - 1; index1 >= 0; --index1)
    {
      List<PowerTask> taskList = powerTaskListList[index1].GetTaskList();
      for (int index2 = taskList.Count - 1; index2 >= 0; --index2)
      {
        if (taskList[index2].GetPower() is Network.HistTagChange power && power.Entity == entity.GetEntityId() && (power.Tag == 49 || power.Tag == 1702))
          return (TAG_ZONE) power.Value;
      }
    }
    TAG_ZONE tag = entity.GetTag<TAG_ZONE>(GAME_TAG.FAKE_ZONE);
    return tag != TAG_ZONE.INVALID ? tag : entity.GetZone();
  }

  public static bool IsEntityHiddenAfterCurrentTasklist(Entity entity)
  {
    if (!entity.IsHidden())
      return false;
    PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
    if (powerProcessor.GetCurrentTaskList() != null)
    {
      foreach (PowerTask task in powerProcessor.GetCurrentTaskList().GetTaskList())
      {
        if (task.GetPower() is Network.HistShowEntity power && power.Entity.ID == entity.GetEntityId() && !string.IsNullOrEmpty(power.Entity.CardID))
          return false;
      }
    }
    return true;
  }

  public static bool IsGalakrond(string cardId) => cardId == "DRG_600" || cardId == "DRG_600t2" || cardId == "DRG_600t3" || cardId == "DRG_650" || cardId == "DRG_650t2" || cardId == "DRG_650t3" || cardId == "DRG_620" || cardId == "DRG_620t2" || cardId == "DRG_620t3" || cardId == "DRG_660" || cardId == "DRG_660t2" || cardId == "DRG_660t3" || cardId == "DRG_610" || cardId == "DRG_610t2" || cardId == "DRG_610t3";

  public static bool IsGalakrondInPlay(Player player)
  {
    if (player == null)
      return false;
    Entity hero = player.GetHero();
    return hero != null && GameUtils.IsGalakrond(hero.GetCardId());
  }

  public static void DoDamageTasks(PowerTaskList powerTaskList, Card sourceCard, Card targetCard)
  {
    List<PowerTask> taskList = powerTaskList.GetTaskList();
    if (taskList == null || taskList.Count == 0)
      return;
    int entityId1 = sourceCard.GetEntity().GetEntityId();
    int entityId2 = targetCard.GetEntity().GetEntityId();
    foreach (PowerTask powerTask in taskList)
    {
      Network.PowerHistory power = powerTask.GetPower();
      if (power.Type == Network.PowerType.META_DATA)
      {
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        if (histMetaData.MetaType == HistoryMeta.Type.DAMAGE || histMetaData.MetaType == HistoryMeta.Type.HEALING)
        {
          foreach (int num in histMetaData.Info)
          {
            if (num == entityId1 || num == entityId2)
              powerTask.DoTask();
          }
        }
      }
      else if (power.Type == Network.PowerType.TAG_CHANGE)
      {
        Network.HistTagChange histTagChange = (Network.HistTagChange) power;
        if (histTagChange.Entity == entityId1 || histTagChange.Entity == entityId2)
        {
          switch ((GAME_TAG) histTagChange.Tag)
          {
            case GAME_TAG.EXHAUSTED:
            case GAME_TAG.DAMAGE:
              powerTask.DoTask();
              continue;
            default:
              continue;
          }
        }
      }
    }
  }

  public static AdventureDbfRecord GetAdventureRecordFromMissionId(int missionId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    if (record == null)
      return (AdventureDbfRecord) null;
    int adventureId = record.AdventureId;
    return GameDbf.Adventure.GetRecord(adventureId);
  }

  public static WingDbfRecord GetWingRecordFromMissionId(int missionId)
  {
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId((ScenarioDbId) missionId);
    return wingIdFromMissionId == WingDbId.INVALID ? (WingDbfRecord) null : GameDbf.Wing.GetRecord((int) wingIdFromMissionId);
  }

  public static WingDbId GetWingIdFromMissionId(ScenarioDbId missionId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((int) missionId);
    return record != null ? (WingDbId) record.WingId : WingDbId.INVALID;
  }

  public static AdventureDataDbfRecord GetAdventureDataRecord(
    int adventureId,
    int modeId)
  {
    foreach (AdventureDataDbfRecord record in GameDbf.AdventureData.GetRecords())
    {
      if (record.AdventureId == adventureId && record.ModeId == modeId)
        return record;
    }
    return (AdventureDataDbfRecord) null;
  }

  public static List<ScenarioDbfRecord> GetClassChallengeRecords(
    int adventureId,
    int wingId)
  {
    List<ScenarioDbfRecord> challengeRecords = new List<ScenarioDbfRecord>();
    foreach (ScenarioDbfRecord record in GameDbf.Scenario.GetRecords())
    {
      if (record.ModeId == 4 && record.AdventureId == adventureId && record.WingId == wingId)
        challengeRecords.Add(record);
    }
    return challengeRecords;
  }

  public static TAG_CLASS GetClassChallengeHeroClass(ScenarioDbfRecord rec)
  {
    if (rec.ModeId != 4)
      return TAG_CLASS.INVALID;
    int player1HeroCardId = rec.Player1HeroCardId;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(player1HeroCardId);
    return entityDef == null ? TAG_CLASS.INVALID : entityDef.GetClass();
  }

  public static List<TAG_CLASS> GetClassChallengeHeroClasses(
    int adventureId,
    int wingId)
  {
    List<ScenarioDbfRecord> challengeRecords = GameUtils.GetClassChallengeRecords(adventureId, wingId);
    List<TAG_CLASS> challengeHeroClasses = new List<TAG_CLASS>();
    foreach (ScenarioDbfRecord rec in challengeRecords)
      challengeHeroClasses.Add(GameUtils.GetClassChallengeHeroClass(rec));
    return challengeHeroClasses;
  }

  public static bool IsAIMission(int missionId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    return record != null && record.Players == 1;
  }

  public static bool IsCoopMission(int missionId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    return record != null && record.IsCoop;
  }

  public static bool IsMercenariesMission(int missionid) => missionid == 3778 || missionid == 3900 || missionid == 3901 || missionid == 4067 || missionid == 3779 || missionid == 3744 || missionid == 3792 || missionid == 3790 || missionid == 3899 || missionid == 3862;

  public static string GetMissionHeroCardId(int missionId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    if (record == null)
      return (string) null;
    int player2HeroCardId = record.ClientPlayer2HeroCardId;
    if (player2HeroCardId == 0)
      player2HeroCardId = record.Player2HeroCardId;
    return GameUtils.TranslateDbIdToCardId(player2HeroCardId);
  }

  public static string GetMissionHeroName(int missionId)
  {
    string missionHeroCardId = GameUtils.GetMissionHeroCardId(missionId);
    if (missionHeroCardId == null)
      return (string) null;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(missionHeroCardId);
    if (entityDef != null)
      return entityDef.GetName();
    Debug.LogError((object) string.Format("GameUtils.GetMissionHeroName() - hero {0} for mission {1} has no EntityDef", (object) missionHeroCardId, (object) missionId));
    return (string) null;
  }

  public static string GetMissionHeroPowerCardId(int missionId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    if (record == null)
      return (string) null;
    int player2HeroPowerCardId = record.ClientPlayer2HeroPowerCardId;
    if (player2HeroPowerCardId != 0)
      return GameUtils.TranslateDbIdToCardId(player2HeroPowerCardId);
    int player2HeroCardId = record.ClientPlayer2HeroCardId;
    if (player2HeroCardId == 0)
      player2HeroCardId = record.Player2HeroCardId;
    return GameUtils.GetHeroPowerCardIdFromHero(player2HeroCardId);
  }

  public static bool IsMissionForAdventure(int missionId, int adventureId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    return record != null && adventureId == record.AdventureId;
  }

  public static bool IsTutorialMission(int missionId) => GameUtils.IsMissionForAdventure(missionId, 1);

  public static bool IsPracticeMission(int missionId) => GameUtils.IsMissionForAdventure(missionId, 2);

  public static bool IsDungeonCrawlMission(int missionId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    return record != null && GameUtils.DoesAdventureModeUseDungeonCrawlFormat((AdventureModeDbId) record.ModeId);
  }

  public static bool DoesAdventureModeUseDungeonCrawlFormat(AdventureModeDbId modeId) => modeId == AdventureModeDbId.DUNGEON_CRAWL || modeId == AdventureModeDbId.DUNGEON_CRAWL_HEROIC;

  public static bool IsBoosterLatestActiveExpansion(int boosterId) => (BoosterDbId) boosterId == GameUtils.GetLatestRewardableBooster();

  public static BoosterDbId GetLatestRewardableBooster() => GameUtils.GetRewardableBoosterOffsetFromLatest(0);

  public static BoosterDbId GetRewardableBoosterOffsetFromLatest(int offset)
  {
    List<BoosterDbfRecord> rewardableBoosters = GameUtils.GetRewardableBoosters();
    if (rewardableBoosters.Count <= 0)
    {
      Debug.LogError((object) "No active Booster sets found");
      return BoosterDbId.INVALID;
    }
    offset = Mathf.Clamp(offset, 0, rewardableBoosters.Count - 1);
    return (BoosterDbId) rewardableBoosters[offset].ID;
  }

  public static BoosterDbId GetRewardableBoosterFromSelector(
    RewardItem.BoosterSelector selector)
  {
    switch (selector)
    {
      case RewardItem.BoosterSelector.LATEST:
        return GameUtils.GetRewardableBoosterOffsetFromLatest(0);
      case RewardItem.BoosterSelector.LATEST_OFFSET_BY_1:
        return GameUtils.GetRewardableBoosterOffsetFromLatest(1);
      case RewardItem.BoosterSelector.LATEST_OFFSET_BY_2:
        return GameUtils.GetRewardableBoosterOffsetFromLatest(2);
      case RewardItem.BoosterSelector.LATEST_OFFSET_BY_3:
        return GameUtils.GetRewardableBoosterOffsetFromLatest(3);
      default:
        Debug.LogError((object) string.Format("Unknown BoosterSelector {0}", (object) selector));
        return BoosterDbId.INVALID;
    }
  }

  public static AdventureDbId GetLatestActiveAdventure()
  {
    AdventureDbId latestActiveAdventure = AdventureDbId.INVALID;
    foreach (DbfRecord record in GameDbf.Adventure.GetRecords())
    {
      AdventureDbId id = (AdventureDbId) record.ID;
      if (!AdventureConfig.IsAdventureComingSoon(id) && AdventureConfig.IsAdventureEventActive(id) && id > latestActiveAdventure)
        latestActiveAdventure = id;
    }
    return latestActiveAdventure;
  }

  public static bool IsExpansionMission(int missionId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    if (record == null)
      return false;
    int adventureId = record.AdventureId;
    return adventureId != 0 && GameUtils.IsExpansionAdventure((AdventureDbId) adventureId);
  }

  public static bool IsExpansionAdventure(AdventureDbId adventureId)
  {
    switch (adventureId)
    {
      case AdventureDbId.INVALID:
      case AdventureDbId.TUTORIAL:
      case AdventureDbId.PRACTICE:
      case AdventureDbId.TAVERN_BRAWL:
      case AdventureDbId.MERCENARY_PVE:
        return false;
      default:
        return true;
    }
  }

  public static string GetAdventureProductStringKey(int wingID)
  {
    AdventureDbId adventureIdByWingId = GameUtils.GetAdventureIdByWingId(wingID);
    return adventureIdByWingId != AdventureDbId.INVALID ? GameDbf.Adventure.GetRecord((int) adventureIdByWingId).ProductStringKey : string.Empty;
  }

  public static AdventureDbId GetAdventureId(int missionId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    return record == null ? AdventureDbId.INVALID : (AdventureDbId) record.AdventureId;
  }

  public static AdventureDbId GetAdventureIdByWingId(int wingID)
  {
    WingDbfRecord record = GameDbf.Wing.GetRecord(wingID);
    if (record == null)
      return AdventureDbId.INVALID;
    AdventureDbId adventureId = (AdventureDbId) record.AdventureId;
    return !GameUtils.IsExpansionAdventure(adventureId) ? AdventureDbId.INVALID : adventureId;
  }

  public static AdventureModeDbId GetAdventureModeId(int missionId)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    return record == null ? AdventureModeDbId.INVALID : (AdventureModeDbId) record.ModeId;
  }

  public static bool IsHeroicAdventureMission(int missionId) => GameUtils.IsModeHeroic(GameUtils.GetAdventureModeId(missionId));

  public static bool IsModeHeroic(AdventureModeDbId mode) => mode == AdventureModeDbId.LINEAR_HEROIC || mode == AdventureModeDbId.DUNGEON_CRAWL_HEROIC;

  public static AdventureModeDbId GetNormalModeFromHeroicMode(
    AdventureModeDbId mode)
  {
    if (mode == AdventureModeDbId.DUNGEON_CRAWL_HEROIC)
      return AdventureModeDbId.DUNGEON_CRAWL;
    return mode == AdventureModeDbId.LINEAR_HEROIC ? AdventureModeDbId.LINEAR : mode;
  }

  public static bool IsClassChallengeMission(int missionId) => GameUtils.GetAdventureModeId(missionId) == AdventureModeDbId.CLASS_CHALLENGE;

  public static int GetSortedWingUnlockIndex(WingDbfRecord wingRecord)
  {
    List<WingDbfRecord> records = GameDbf.Wing.GetRecords((Predicate<WingDbfRecord>) (r => r.AdventureId == wingRecord.AdventureId));
    bool wingsHaveSameUnlockOrder = false;
    records.Sort((Comparison<WingDbfRecord>) ((l, r) =>
    {
      int sortedWingUnlockIndex = l.UnlockOrder - r.UnlockOrder;
      if (sortedWingUnlockIndex != 0 || l.ID == r.ID)
        return sortedWingUnlockIndex;
      wingsHaveSameUnlockOrder = true;
      return sortedWingUnlockIndex;
    }));
    return wingsHaveSameUnlockOrder ? 0 : records.FindIndex((Predicate<WingDbfRecord>) (r => r.ID == wingRecord.ID));
  }

  public static int GetNumWingsInAdventure(AdventureDbId adventureId) => GameDbf.Wing.GetRecords((Predicate<WingDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureId)).Count;

  public static void CompleteTraditionalTutorial()
  {
    NetCache.NetCacheProfileProgress cacheProfileProgress = GameUtils.s_profileProgress.Value;
    if (cacheProfileProgress == null || GameUtils.IsTraditionalTutorialComplete(cacheProfileProgress.CampaignProgress))
      return;
    if (GameState.Get().GetGameEntity() is TutorialEntity gameEntity)
      gameEntity.ClearPreTutorialNotification();
    GameUtils.SetTutorialProgress(TutorialProgress.ILLIDAN_COMPLETE);
    if (Network.ShouldBeConnectedToAurora() && Network.IsLoggedIn())
      BnetPresenceMgr.Get().SetGameField(15U, 1);
    NotificationManager.Get().DestroyAllPopUps();
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
  }

  public static void SetTutorialProgress(TutorialProgress val)
  {
    if (GameMgr.Get().IsSpectator())
      return;
    AdTrackingManager.Get().TrackTutorialProgress(val);
    NetCache.NetCacheProfileProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileProgress>();
    if (netObject != null)
      netObject.CampaignProgress = val;
    NetCache.Get().NetCacheChanged<NetCache.NetCacheProfileProgress>();
  }

  public static bool IsTraditionalTutorialComplete()
  {
    NetCache.NetCacheProfileProgress cacheProfileProgress = GameUtils.s_profileProgress.Value;
    return cacheProfileProgress != null && GameUtils.IsTraditionalTutorialComplete(cacheProfileProgress.CampaignProgress);
  }

  public static bool AreAllTutorialsComplete(TutorialProgress playTutorialProgress) => GameUtils.IsTraditionalTutorialComplete(playTutorialProgress) && GameUtils.IsBattleGroundsTutorialComplete() && GameUtils.IsMercenariesVillageTutorialComplete();

  public static bool IsTraditionalTutorialComplete(TutorialProgress progress) => DemoMgr.Get().GetMode() != DemoMode.BLIZZ_MUSEUM && progress == TutorialProgress.ILLIDAN_COMPLETE;

  public static bool CanCheckTutorialCompletion() => GameSaveDataManager.Get().IsDataReady(GameSaveKeyId.BACON) && GameSaveDataManager.Get().IsDataReady(GameSaveKeyId.MERCENARIES) && GameUtils.s_profileProgress.Value != null;

  public static bool IsAnyTutorialComplete()
  {
    if (GameUtils.IsBattleGroundsTutorialComplete() || GameUtils.IsMercenariesVillageTutorialComplete())
      return true;
    NetCache.NetCacheProfileProgress cacheProfileProgress = GameUtils.s_profileProgress.Value;
    return cacheProfileProgress != null && GameUtils.IsTraditionalTutorialComplete(cacheProfileProgress.CampaignProgress);
  }

  public static bool IsBattleGroundsTutorialComplete()
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    bool flag = false;
    if (netObject != null)
      flag = !netObject.Games.BattlegroundsTutorial;
    if (flag)
      return false;
    long num = 0;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.BACON, GameSaveKeySubkeyId.BACON_HAS_SEEN_TUTORIAL, out num);
    return num > 0L;
  }

  public static bool IsMercenariesPrologueBountyComplete(
    NetCache.NetCacheMercenariesPlayerInfo playerInfo)
  {
    if (playerInfo == null)
    {
      Debug.LogError((object) "Player Info was null when check prologue bounty completion.  This should be checked before entering this function or undesirable results may occur");
      return false;
    }
    List<LettuceBountyDbfRecord> list = GameDbf.LettuceBounty.GetRecords((Predicate<LettuceBountyDbfRecord>) (r => r.BountySetRecord != null && r.BountySetRecord.IsTutorial && r.Enabled)).ToList<LettuceBountyDbfRecord>();
    return list.Count > 0 && MercenariesDataUtil.IsBountyComplete(list[0].ID, playerInfo);
  }

  public static bool IsMercenariesVillageTutorialComplete() => LettuceTutorialUtils.IsSpecificEventComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_END);

  public static bool AreAllTutorialsComplete()
  {
    NetCache.NetCacheProfileProgress cacheProfileProgress = GameUtils.s_profileProgress.Value;
    return cacheProfileProgress != null && GameUtils.AreAllTutorialsComplete(cacheProfileProgress.CampaignProgress);
  }

  public static bool TutorialPreviewVideosEnabled()
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject != null)
      return netObject.TutorialPreviewVideosEnabled;
    Log.All.Print(" Could not get NetCacheFeatures Object");
    return true;
  }

  public static float TutorialPreviewVideosTimeout()
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject != null)
      return netObject.TutorialPreviewVideosTimeout;
    Log.All.Print(" Could not get NetCacheFeatures Object");
    return NetCache.NetCacheFeatures.Defaults.TutorialPreviewVideosTimeout;
  }

  public static int GetNextTutorial(TutorialProgress progress)
  {
    switch (progress)
    {
      case TutorialProgress.NOTHING_COMPLETE:
        return 3;
      case TutorialProgress.HOGGER_COMPLETE:
        return 4;
      case TutorialProgress.MILLHOUSE_COMPLETE:
        return 249;
      case TutorialProgress.CHO_COMPLETE:
        return 181;
      case TutorialProgress.MUKLA_COMPLETE:
        return 201;
      case TutorialProgress.NESINGWARY_COMPLETE:
        return 248;
      default:
        return 0;
    }
  }

  public static int GetNextTutorial()
  {
    NetCache.NetCacheProfileProgress cacheProfileProgress = GameUtils.s_profileProgress.Value;
    return cacheProfileProgress == null ? GameUtils.GetNextTutorial(Options.Get().GetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS)) : GameUtils.GetNextTutorial(cacheProfileProgress.CampaignProgress);
  }

  public static string GetTutorialCardRewardDetails(int missionId)
  {
    switch ((ScenarioDbId) missionId)
    {
      case ScenarioDbId.TUTORIAL_HOGGER:
        return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL01");
      case ScenarioDbId.TUTORIAL_MILLHOUSE:
        return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL02");
      case ScenarioDbId.TUTORIAL_MUKLA:
        return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL03");
      case ScenarioDbId.TUTORIAL_NESINGWARY:
        return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL04");
      case ScenarioDbId.TUTORIAL_ILLIDAN:
        return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL05");
      case ScenarioDbId.TUTORIAL_CHO:
        return GameStrings.Get("GLOBAL_REWARD_CARD_DETAILS_TUTORIAL06");
      default:
        Debug.LogWarning((object) string.Format("GameUtils.GetTutorialCardRewardDetails(): no card reward details for mission {0}", (object) missionId));
        return "";
    }
  }

  public static string GetCurrentTutorialCardRewardDetails() => GameUtils.GetTutorialCardRewardDetails(GameMgr.Get().GetMissionId());

  public static int MissionSortComparison(ScenarioDbfRecord rec1, ScenarioDbfRecord rec2) => rec1.SortOrder - rec2.SortOrder;

  public static List<ScenarioGuestHeroesDbfRecord> GetScenarioGuestHeroes(
    int scenarioId)
  {
    return GameDbf.ScenarioGuestHeroes.GetRecords((Predicate<ScenarioGuestHeroesDbfRecord>) (r => r.ScenarioId == scenarioId));
  }

  public static int GetDefeatedBossCount()
  {
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord(Options.Get().GetInt(Option.SELECTED_ADVENTURE), Options.Get().GetInt(Option.SELECTED_ADVENTURE_MODE));
    if (adventureDataRecord == null)
      return 0;
    GameSaveKeyId saveDataServerKey = (GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey;
    if (!DungeonCrawlUtil.IsDungeonRunActive(saveDataServerKey))
      return 0;
    List<long> values = (List<long>) null;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSSES_DEFEATED, out values);
    return values == null ? 0 : values.Count;
  }

  public static List<FixedRewardActionDbfRecord> GetFixedActionRecords(
    FixedRewardAction.Type actionType)
  {
    return GameDbf.GetIndex().GetFixedActionRecordsForType(actionType);
  }

  public static FixedRewardDbfRecord GetFixedRewardForCard(
    string cardID,
    TAG_PREMIUM premium)
  {
    int dbId = GameUtils.TranslateCardIdToDbId(cardID);
    return GameDbf.GetIndex().GetFixedRewardRecordsForCardId(dbId, (int) premium);
  }

  public static List<FixedRewardMapDbfRecord> GetFixedRewardMapRecordsForAction(
    int actionID)
  {
    return GameDbf.GetIndex().GetFixedRewardMapRecordsForAction(actionID);
  }

  public static int GetFixedRewardCounterpartCardID(int cardID)
  {
    foreach (FixedRewardActionDbfRecord fixedActionRecord in GameUtils.GetFixedActionRecords(FixedRewardAction.Type.OWNS_COUNTERPART_CARD))
    {
      if (SpecialEventManager.Get().IsEventActive(fixedActionRecord.ActiveEvent, false))
      {
        foreach (FixedRewardMapDbfRecord rewardMapDbfRecord in GameUtils.GetFixedRewardMapRecordsForAction(fixedActionRecord.ID))
        {
          FixedRewardDbfRecord record = GameDbf.FixedReward.GetRecord(rewardMapDbfRecord.RewardId);
          if (GameUtils.GetCardTagValue(record.CardId, GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID) == cardID)
            return record.CardId;
        }
      }
    }
    return 0;
  }

  public static string GetOwnedCounterpartCardIDForFormat(
    EntityDef cardDef,
    PegasusShared.FormatType formatType,
    int minOwned)
  {
    string cardId = GameUtils.TranslateDbIdToCardId(cardDef.GetTag(GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID));
    if (cardId != null)
      return cardId;
    TAG_CARD_SET[] cardSetsInFormat = GameUtils.GetCardSetsInFormat(formatType);
    CollectionManager collectionManager = CollectionManager.Get();
    int? manaCost = new int?();
    int? nullable = new int?(minOwned);
    TAG_CARD_SET[] theseCardSets = cardSetsInFormat;
    TAG_RARITY? rarity = new TAG_RARITY?();
    TAG_RACE? race = new TAG_RACE?();
    bool? isHero = new bool?();
    int? minOwned1 = nullable;
    bool? notSeen = new bool?();
    bool? isCraftable = new bool?();
    bool? filterCoreCounterpartCards = new bool?();
    foreach (CollectibleCard card in collectionManager.FindCards(manaCost: manaCost, theseCardSets: theseCardSets, rarity: rarity, race: race, isHero: isHero, minOwned: minOwned1, notSeen: notSeen, isCraftable: isCraftable, filterCoreCounterpartCards: filterCoreCounterpartCards).m_cards)
    {
      if (GameUtils.TranslateDbIdToCardId(card.GetEntityDef().GetTag(GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID)) == cardDef.GetCardId())
      {
        cardId = card.CardId;
        break;
      }
    }
    return cardId;
  }

  public static bool IsMatchmadeGameType(GameType gameType, int? missionId = null)
  {
    switch (gameType)
    {
      case GameType.GT_VS_AI:
      case GameType.GT_VS_FRIEND:
      case GameType.GT_TUTORIAL:
      case GameType.GT_FSG_BRAWL_VS_FRIEND:
      case GameType.GT_FSG_BRAWL_1P_VS_AI:
      case GameType.GT_BATTLEGROUNDS_FRIENDLY:
      case GameType.GT_MERCENARIES_PVE:
      case GameType.GT_MERCENARIES_PVE_COOP:
        return false;
      case GameType.GT_ARENA:
      case GameType.GT_RANKED:
      case GameType.GT_CASUAL:
      case GameType.GT_BATTLEGROUNDS:
      case GameType.GT_MERCENARIES_PVP:
        return true;
      case GameType.GT_PVPDR_PAID:
      case GameType.GT_PVPDR:
        return !missionId.HasValue || !DungeonCrawlUtil.IsPVPDRFriendlyEncounter(missionId.Value);
      default:
        if (!GameUtils.IsTavernBrawlGameType(gameType))
          return false;
        int missionId1;
        if (missionId.HasValue)
        {
          missionId1 = missionId.Value;
        }
        else
        {
          TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
          if (tavernBrawlMission == null)
            return true;
          missionId1 = tavernBrawlMission.missionId;
        }
        return !GameUtils.IsAIMission(missionId1);
    }
  }

  public static bool IsTavernBrawlGameType(GameType gameType)
  {
    switch (gameType)
    {
      case GameType.GT_TAVERNBRAWL:
      case GameType.GT_TB_1P_VS_AI:
      case GameType.GT_TB_2P_COOP:
      case GameType.GT_FSG_BRAWL_VS_FRIEND:
      case GameType.GT_FSG_BRAWL:
      case GameType.GT_FSG_BRAWL_1P_VS_AI:
      case GameType.GT_FSG_BRAWL_2P_COOP:
        return true;
      default:
        return false;
    }
  }

  public static bool IsFiresideGatheringGameType(GameType gameType)
  {
    switch (gameType)
    {
      case GameType.GT_FSG_BRAWL_VS_FRIEND:
      case GameType.GT_FSG_BRAWL:
      case GameType.GT_FSG_BRAWL_1P_VS_AI:
      case GameType.GT_FSG_BRAWL_2P_COOP:
        return true;
      default:
        return false;
    }
  }

  public static bool IsPvpDrGameType(GameType gameType)
  {
    switch (gameType)
    {
      case GameType.GT_PVPDR_PAID:
      case GameType.GT_PVPDR:
        return true;
      default:
        return false;
    }
  }

  public static bool IsMercenariesGameType(GameType gameType)
  {
    switch (gameType)
    {
      case GameType.GT_MERCENARIES_PVP:
      case GameType.GT_MERCENARIES_PVE:
      case GameType.GT_MERCENARIES_PVE_COOP:
      case GameType.GT_MERCENARIES_AI_VS_AI:
      case GameType.GT_MERCENARIES_FRIENDLY:
        return true;
      default:
        return false;
    }
  }

  public static bool ShouldShowArenaModeIcon() => GameMgr.Get().GetGameType() == GameType.GT_ARENA;

  public static bool ShouldShowCasualModeIcon() => GameMgr.Get().GetGameType() == GameType.GT_CASUAL;

  public static bool ShouldShowFriendlyChallengeIcon() => GameMgr.Get().GetGameType() == GameType.GT_VS_FRIEND && !FriendChallengeMgr.Get().IsChallengeTavernBrawl();

  public static bool ShouldShowTavernBrawlModeIcon()
  {
    GameType gameType = GameMgr.Get().GetGameType();
    return gameType == GameType.GT_VS_FRIEND && FriendChallengeMgr.Get().IsChallengeTavernBrawl() || GameUtils.IsTavernBrawlGameType(gameType);
  }

  public static bool ShouldShowAdventureModeIcon()
  {
    int missionId = GameMgr.Get().GetMissionId();
    GameType gameType = GameMgr.Get().GetGameType();
    AdventureDbId adventureId = GameUtils.GetAdventureId(missionId);
    return GameUtils.IsExpansionMission(missionId) && adventureId != AdventureDbId.TAVERN_BRAWL && !AdventureUtils.IsDuelsAdventure(adventureId) && !GameUtils.IsTavernBrawlGameType(gameType) && !GameUtils.IsMercenariesGameType(gameType);
  }

  public static bool ShouldShowPvpDrModeIcon() => AdventureUtils.IsDuelsAdventure(GameUtils.GetAdventureId(GameMgr.Get().GetMissionId()));

  public static bool IsGameTypeRanked() => GameUtils.IsGameTypeRanked(GameMgr.Get().GetGameType());

  public static bool IsGameTypeRanked(GameType gameType) => !DemoMgr.Get().IsExpoDemo() && gameType == GameType.GT_RANKED;

  public static void RequestPlayerPresence(BnetGameAccountId gameAccountId)
  {
    List<PresenceFieldKey> presenceFieldKeyList = new List<PresenceFieldKey>();
    PresenceFieldKey presenceFieldKey = new PresenceFieldKey();
    presenceFieldKey.programId = BnetProgramId.BNET.GetValue();
    presenceFieldKey.groupId = 2U;
    presenceFieldKey.fieldId = 7U;
    presenceFieldKey.uniqueId = 0UL;
    presenceFieldKeyList.Add(presenceFieldKey);
    presenceFieldKey.programId = BnetProgramId.BNET.GetValue();
    presenceFieldKey.groupId = 2U;
    presenceFieldKey.fieldId = 3U;
    presenceFieldKey.uniqueId = 0UL;
    presenceFieldKeyList.Add(presenceFieldKey);
    presenceFieldKey.programId = BnetProgramId.BNET.GetValue();
    presenceFieldKey.groupId = 2U;
    presenceFieldKey.fieldId = 5U;
    presenceFieldKey.uniqueId = 0UL;
    presenceFieldKeyList.Add(presenceFieldKey);
    if (GameUtils.IsGameTypeRanked())
      presenceFieldKeyList.Add(new PresenceFieldKey()
      {
        programId = BnetProgramId.HEARTHSTONE.GetValue(),
        groupId = 2U,
        fieldId = 18U,
        uniqueId = 0UL
      });
    PresenceFieldKey[] array = presenceFieldKeyList.ToArray();
    BattleNet.RequestPresenceFields(true, (BnetEntityId) gameAccountId, array);
  }

  public static bool IsAIPlayer(BnetGameAccountId gameAccountId) => !((BnetEntityId) gameAccountId == (BnetEntityId) null) && !gameAccountId.IsValid();

  public static bool IsHumanPlayer(BnetGameAccountId gameAccountId) => !((BnetEntityId) gameAccountId == (BnetEntityId) null) && gameAccountId.IsValid();

  public static bool IsBnetPlayer(BnetGameAccountId gameAccountId) => GameUtils.IsHumanPlayer(gameAccountId) && Network.ShouldBeConnectedToAurora();

  public static bool IsGuestPlayer(BnetGameAccountId gameAccountId) => GameUtils.IsHumanPlayer(gameAccountId) && !Network.ShouldBeConnectedToAurora();

  public static bool IsAnyTransitionActive()
  {
    SceneMgr sceneMgr = SceneMgr.Get();
    if (sceneMgr != null)
    {
      if (sceneMgr.IsTransitionNowOrPending())
        return true;
      PegasusScene scene = sceneMgr.GetScene();
      if ((UnityEngine.Object) scene != (UnityEngine.Object) null && scene.IsTransitioning())
        return true;
    }
    Box box = Box.Get();
    if ((UnityEngine.Object) box != (UnityEngine.Object) null && box.IsTransitioningToSceneMode())
      return true;
    LoadingScreen loadingScreen = LoadingScreen.Get();
    return (UnityEngine.Object) loadingScreen != (UnityEngine.Object) null && loadingScreen.IsTransitioning();
  }

  public static void LogoutConfirmation()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get(Network.ShouldBeConnectedToAurora() ? "GLOBAL_SWITCH_ACCOUNT" : "GLOBAL_LOGIN_CONFIRM_TITLE"),
      m_text = GameStrings.Get(Network.ShouldBeConnectedToAurora() ? "GLOBAL_LOGOUT_CONFIRM_MESSAGE" : "GLOBAL_LOGIN_CONFIRM_MESSAGE"),
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = new AlertPopup.ResponseCallback(GameUtils.OnLogoutConfirmationResponse)
    };
    DialogManager.Get().ShowPopup(info);
  }

  private static void OnLogoutConfirmationResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
      return;
    TemporaryAccountManager.Get().UnselectTemporaryAccount();
    GameUtils.Logout();
  }

  public static void Logout()
  {
    GameMgr.Get().SetPendingAutoConcede(true);
    if (Network.ShouldBeConnectedToAurora())
      ServiceManager.Get<ILoginService>()?.ClearAuthentication();
    HearthstoneApplication.Get().ResetAndForceLogin();
  }

  public static PackOpeningRarity GetPackOpeningRarity(TAG_RARITY tag)
  {
    switch (tag)
    {
      case TAG_RARITY.COMMON:
        return PackOpeningRarity.COMMON;
      case TAG_RARITY.FREE:
        return PackOpeningRarity.COMMON;
      case TAG_RARITY.RARE:
        return PackOpeningRarity.RARE;
      case TAG_RARITY.EPIC:
        return PackOpeningRarity.EPIC;
      case TAG_RARITY.LEGENDARY:
        return PackOpeningRarity.LEGENDARY;
      default:
        return PackOpeningRarity.NONE;
    }
  }

  public static List<BoosterDbfRecord> GetPackRecordsWithStorePrefab() => GameDbf.Booster.GetRecords((Predicate<BoosterDbfRecord>) (r => !string.IsNullOrEmpty(r.StorePrefab)));

  public static List<AdventureDbfRecord> GetSortedAdventureRecordsWithStorePrefab()
  {
    List<AdventureDbfRecord> records = GameDbf.Adventure.GetRecords((Predicate<AdventureDbfRecord>) (r => !string.IsNullOrEmpty(r.StorePrefab)));
    records.Sort((Comparison<AdventureDbfRecord>) ((l, r) => r.SortOrder - l.SortOrder));
    return records;
  }

  public static List<AdventureDbfRecord> GetAdventureRecordsWithDefPrefab() => GameDbf.Adventure.GetRecords((Predicate<AdventureDbfRecord>) (r => !string.IsNullOrEmpty(r.AdventureDefPrefab)));

  public static List<AdventureDataDbfRecord> GetAdventureDataRecordsWithSubDefPrefab() => GameDbf.AdventureData.GetRecords((Predicate<AdventureDataDbfRecord>) (r => !string.IsNullOrEmpty(r.AdventureSubDefPrefab)));

  public static int PackSortingPredicate(BoosterDbfRecord left, BoosterDbfRecord right)
  {
    if (right.ListDisplayOrderCategory != left.ListDisplayOrderCategory)
      return Mathf.Clamp(right.ListDisplayOrderCategory - left.ListDisplayOrderCategory, -1, 1);
    return right.ListDisplayOrder != left.ListDisplayOrder ? Mathf.Clamp(right.ListDisplayOrder - left.ListDisplayOrder, -1, 1) : Mathf.Clamp(right.ID - left.ID, -1, 1);
  }

  public static IEnumerable<int> GetSortedPackIds(bool ascending = true)
  {
    List<BoosterDbfRecord> records = GameDbf.Booster.GetRecords();
    if (ascending)
      records.Sort((Comparison<BoosterDbfRecord>) ((l, r) => GameUtils.PackSortingPredicate(r, l)));
    else
      records.Sort((Comparison<BoosterDbfRecord>) ((l, r) => GameUtils.PackSortingPredicate(l, r)));
    return records.Select<BoosterDbfRecord, int>((Func<BoosterDbfRecord, int>) (b => b.ID));
  }

  public static bool IsFakePackOpeningEnabled() => HearthstoneApplication.IsInternal() && Options.Get().GetBool(Option.FAKE_PACK_OPENING);

  public static int GetFakePackCount() => !HearthstoneApplication.IsInternal() ? 0 : Options.Get().GetInt(Option.FAKE_PACK_COUNT);

  public static bool IsFirstPurchaseBundleBooster(StorePackId storePackId) => storePackId.Type == StorePackType.BOOSTER && 181 == storePackId.Id;

  public static bool IsMammothBundleBooster(StorePackId storePackId) => storePackId.Type == StorePackType.BOOSTER && 41 == storePackId.Id;

  public static bool IsLimitedTimeOffer(StorePackId storePackId)
  {
    if (storePackId.Type == StorePackType.MODULAR_BUNDLE)
    {
      int dataFromStorePackId = GameUtils.GetProductDataFromStorePackId(storePackId);
      if (dataFromStorePackId != 0)
      {
        Network.Bundle bundle = StoreManager.Get().EnumerateBundlesForProductType(ProductType.PRODUCT_TYPE_HIDDEN_LICENSE, false, dataFromStorePackId).FirstOrDefault<Network.Bundle>();
        if ((Record) bundle != (Record) null && !string.IsNullOrEmpty(bundle.ProductEvent))
        {
          SpecialEventManager specialEventManager = SpecialEventManager.Get();
          SpecialEventType eventType = specialEventManager.GetEventType(bundle.ProductEvent);
          if (eventType != SpecialEventType.UNKNOWN)
          {
            DateTime? eventEndTimeUtc = specialEventManager.GetEventEndTimeUtc(eventType);
            if (eventEndTimeUtc.HasValue && eventEndTimeUtc.Value.Subtract(DateTime.UtcNow).TotalDays < 365.0)
              return true;
          }
        }
      }
    }
    return false;
  }

  public static bool IsHiddenLicenseBundleBooster(StorePackId storePackId)
  {
    if (storePackId.Type == StorePackType.BOOSTER)
    {
      switch ((BoosterDbId) storePackId.Id)
      {
        case BoosterDbId.MAMMOTH_BUNDLE:
        case BoosterDbId.FIRST_PURCHASE:
          return true;
        default:
          return false;
      }
    }
    else
      return storePackId.Type == StorePackType.MODULAR_BUNDLE;
  }

  public static int GetProductDataFromStorePackId(StorePackId storePackId, int selectedIndex = 0)
  {
    if (storePackId.Type == StorePackType.BOOSTER)
    {
      if (storePackId.Id == 181)
        return 40;
      return storePackId.Id == 41 ? 27 : storePackId.Id;
    }
    if (storePackId.Type != StorePackType.MODULAR_BUNDLE)
      return 0;
    List<ModularBundleLayoutDbfRecord> layoutsForBundle = StoreManager.Get().GetRegionNodeLayoutsForBundle(storePackId.Id);
    if (selectedIndex >= layoutsForBundle.Count)
    {
      Log.Store.PrintWarning(string.Format("Selected invalid layout at index={0}. Defaulting to layout at index=0.", (object) selectedIndex));
      selectedIndex = 0;
    }
    return layoutsForBundle[selectedIndex].HiddenLicenseId;
  }

  public static int GetProductDataCountFromStorePackId(StorePackId storePackId)
  {
    if (storePackId.Type == StorePackType.BOOSTER)
      return 1;
    return storePackId.Type == StorePackType.MODULAR_BUNDLE ? StoreManager.Get().GetRegionNodeLayoutsForBundle(storePackId.Id).Count : 0;
  }

  public static List<BoosterDbfRecord> GetRewardableBoosters()
  {
    List<BoosterDbfRecord> rewardableBoosters = new List<BoosterDbfRecord>();
    DateTime utcNow = DateTime.UtcNow;
    foreach (BoosterDbfRecord record in GameDbf.Booster.GetRecords())
    {
      if (!GameUtils.IsBoosterRotated((BoosterDbId) record.ID, utcNow) && SpecialEventManager.Get().IsEventActive(record.RewardableEvent, false, utcNow))
        rewardableBoosters.Add(record);
    }
    rewardableBoosters.Sort(GameUtils.SortBoostersDescending);
    return rewardableBoosters;
  }

  public static int GetBoardIdFromAssetName(string name)
  {
    foreach (BoardDbfRecord record in GameDbf.Board.GetRecords())
    {
      string prefab = record.Prefab;
      if (!(name != prefab))
        return record.ID;
    }
    return 0;
  }

  public static UnityEngine.Object Instantiate(
    GameObject original,
    GameObject parent,
    bool withRotation = false)
  {
    if ((UnityEngine.Object) original == (UnityEngine.Object) null)
      return (UnityEngine.Object) null;
    GameObject child = UnityEngine.Object.Instantiate<GameObject>(original);
    GameUtils.SetParent(child, parent, withRotation);
    return (UnityEngine.Object) child;
  }

  public static UnityEngine.Object Instantiate(
    Component original,
    GameObject parent,
    bool withRotation = false)
  {
    if ((UnityEngine.Object) original == (UnityEngine.Object) null)
      return (UnityEngine.Object) null;
    Component child = UnityEngine.Object.Instantiate<Component>(original);
    GameUtils.SetParent(child, parent, withRotation);
    return (UnityEngine.Object) child;
  }

  public static UnityEngine.Object Instantiate(UnityEngine.Object original) => original == (UnityEngine.Object) null ? (UnityEngine.Object) null : UnityEngine.Object.Instantiate(original);

  public static UnityEngine.Object InstantiateGameObject(
    string path,
    GameObject parent = null,
    bool withRotation = false)
  {
    if (path == null)
      return (UnityEngine.Object) null;
    GameObject child = AssetLoader.Get().InstantiatePrefab((AssetReference) path);
    if ((UnityEngine.Object) parent != (UnityEngine.Object) null)
      GameUtils.SetParent(child, parent, withRotation);
    return (UnityEngine.Object) child;
  }

  public static void SetParent(Component child, Component parent, bool withRotation = false) => GameUtils.SetParent(child.transform, parent.transform, withRotation);

  public static void SetParent(GameObject child, Component parent, bool withRotation = false) => GameUtils.SetParent(child.transform, parent.transform, withRotation);

  public static void SetParent(Component child, GameObject parent, bool withRotation = false) => GameUtils.SetParent(child.transform, parent.transform, withRotation);

  public static void SetParent(GameObject child, GameObject parent, bool withRotation = false) => GameUtils.SetParent(child.transform, parent.transform, withRotation);

  private static void SetParent(Transform child, Transform parent, bool withRotation)
  {
    Vector3 localScale = child.localScale;
    Quaternion localRotation = child.localRotation;
    child.parent = parent;
    child.localPosition = Vector3.zero;
    child.localScale = localScale;
    if (!withRotation)
      return;
    child.localRotation = localRotation;
  }

  public static void ResetTransform(GameObject obj)
  {
    obj.transform.localPosition = Vector3.zero;
    obj.transform.localScale = Vector3.one;
    obj.transform.localRotation = Quaternion.identity;
  }

  public static void ResetTransform(Component comp) => GameUtils.ResetTransform(comp.gameObject);

  public static T LoadGameObjectWithComponent<T>(string assetPath) where T : Component
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) assetPath);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      return default (T);
    T component = gameObject.GetComponent<T>();
    if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
      return component;
    Debug.LogError((object) string.Format("{0} object does not contain {1} component.", (object) assetPath, (object) typeof (T)));
    UnityEngine.Object.Destroy((UnityEngine.Object) gameObject);
    return default (T);
  }

  public static T FindChildByName<T>(Transform transform, string name) where T : Component
  {
    foreach (Transform transform1 in transform)
    {
      if (transform1.name == name)
        return transform1.GetComponent<T>();
      T childByName = GameUtils.FindChildByName<T>(transform1, name);
      if ((UnityEngine.Object) childByName != (UnityEngine.Object) null)
        return childByName;
    }
    return default (T);
  }

  public static void PlayCardEffectDefSounds(CardEffectDef cardEffectDef)
  {
    if (cardEffectDef == null)
      return;
    foreach (string soundSpellPath in cardEffectDef.m_SoundSpellPaths)
      AssetLoader.Get().InstantiatePrefab((AssetReference) soundSpellPath, (PrefabCallback<GameObject>) ((name, go, data) =>
      {
        if ((UnityEngine.Object) go == (UnityEngine.Object) null)
        {
          Debug.LogError((object) string.Format("Unable to load spell object: {0}", (object) name));
        }
        else
        {
          GameObject destroyObj = go;
          CardSoundSpell component = go.GetComponent<CardSoundSpell>();
          if ((UnityEngine.Object) component == (UnityEngine.Object) null)
          {
            Debug.LogError((object) string.Format("Card sound spell component not found: {0}", (object) name));
            UnityEngine.Object.Destroy((UnityEngine.Object) destroyObj);
          }
          else
          {
            component.AddStateFinishedCallback((Spell.StateFinishedCallback) ((spell, prevStateType, userData) =>
            {
              if (spell.GetActiveState() != SpellStateType.NONE)
                return;
              UnityEngine.Object.Destroy((UnityEngine.Object) destroyObj);
            }));
            component.ForceDefaultAudioSource();
            component.Activate();
          }
        }
      }));
  }

  public static bool LoadCardDefEmoteSound(
    List<EmoteEntryDef> emoteDefs,
    EmoteType type,
    GameUtils.EmoteSoundLoaded callback)
  {
    if (callback == null)
    {
      Debug.LogError((object) "No callback provided for LoadEmote!");
      return false;
    }
    if (emoteDefs == null)
      return false;
    EmoteEntryDef emoteEntryDef = emoteDefs.Find((Predicate<EmoteEntryDef>) (e => e.m_emoteType == type));
    if (emoteEntryDef == null)
      return false;
    AssetLoader.Get().InstantiatePrefab((AssetReference) emoteEntryDef.m_emoteSoundSpellPath, (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
        callback((CardSoundSpell) null);
      else
        callback(go.GetComponent<CardSoundSpell>());
    }));
    return true;
  }

  public static string GetCardIdFromMercenaryId(int mercenaryId)
  {
    MercenaryArtVariationDbfRecord artVariationRecord = LettuceMercenary.GetDefaultArtVariationRecord(mercenaryId);
    if (artVariationRecord != null)
      return GameUtils.TranslateDbIdToCardId(artVariationRecord.CardId);
    Debug.LogErrorFormat("GetCardIdFromMercenaryId() - No record found for merc: {0}", (object) mercenaryId);
    return (string) null;
  }

  public static int GetMercenaryIdFromCardId(int cardId)
  {
    foreach (LettuceMercenaryDbfRecord record in GameDbf.LettuceMercenary.GetRecords())
    {
      foreach (MercenaryArtVariationDbfRecord mercenaryArtVariation in record.MercenaryArtVariations)
      {
        if (mercenaryArtVariation.CardId == cardId)
          return record.ID;
      }
    }
    return 0;
  }

  public static int GetMaxMercenaryLevel() => GameDbf.GetIndex().GetMercenaryMaxLevel();

  public static int GetMercenaryLevelFromExperience(int experience)
  {
    int maxMercenaryLevel = GameUtils.GetMaxMercenaryLevel();
    List<LettuceMercenaryLevelDbfRecord> records = GameDbf.LettuceMercenaryLevel.GetRecords();
    for (int index1 = 1; index1 <= maxMercenaryLevel; ++index1)
    {
      LettuceMercenaryLevelDbfRecord mercenaryLevelDbfRecord = (LettuceMercenaryLevelDbfRecord) null;
      int index2 = 0;
      for (int count = records.Count; index2 < count; ++index2)
      {
        if (records[index2].Level == index1)
        {
          mercenaryLevelDbfRecord = records[index2];
          break;
        }
      }
      if (mercenaryLevelDbfRecord == null)
      {
        Log.Lettuce.PrintError("GetMercenaryLevelFromExperience - Missing mercenary level data!");
        break;
      }
      if (experience < mercenaryLevelDbfRecord.TotalXpRequired)
        return index1 - 1;
    }
    return maxMercenaryLevel;
  }

  public static float GetExperiencePercentageFromExperienceValue(int experience)
  {
    int currentLevel = GameUtils.GetMercenaryLevelFromExperience(experience);
    return currentLevel < GameUtils.GetMaxMercenaryLevel() ? Mathf.InverseLerp((float) GameDbf.LettuceMercenaryLevel.GetRecord((Predicate<LettuceMercenaryLevelDbfRecord>) (r => r.Level == currentLevel)).TotalXpRequired, (float) GameDbf.LettuceMercenaryLevel.GetRecord((Predicate<LettuceMercenaryLevelDbfRecord>) (r => r.Level == currentLevel + 1)).TotalXpRequired, (float) experience) : 1f;
  }

  public static float GetExperiencePercentageDelta(int startingExperience, int experienceDelta)
  {
    int maxMercenaryLevel = GameUtils.GetMaxMercenaryLevel();
    if (experienceDelta == 0)
      return 0.0f;
    int num1 = startingExperience + experienceDelta;
    int startingLevel = maxMercenaryLevel;
    int currentLevel = maxMercenaryLevel;
    int nextLevel = maxMercenaryLevel;
    for (int level = 1; level <= maxMercenaryLevel; level++)
    {
      LettuceMercenaryLevelDbfRecord record = GameDbf.LettuceMercenaryLevel.GetRecord((Predicate<LettuceMercenaryLevelDbfRecord>) (r => r.Level == level));
      if (record == null)
      {
        Log.Lettuce.PrintError("GetMercenaryLevelFromExperience - Missing mercenary level data!");
        break;
      }
      if (startingExperience < record.TotalXpRequired)
      {
        if (level <= startingLevel)
          startingLevel = level - 1;
        if (num1 <= record.TotalXpRequired)
        {
          currentLevel = level - 1;
          nextLevel = level;
          break;
        }
      }
    }
    LettuceMercenaryLevelDbfRecord record1 = GameDbf.LettuceMercenaryLevel.GetRecord((Predicate<LettuceMercenaryLevelDbfRecord>) (r => r.Level == startingLevel));
    LettuceMercenaryLevelDbfRecord record2 = GameDbf.LettuceMercenaryLevel.GetRecord((Predicate<LettuceMercenaryLevelDbfRecord>) (r => r.Level == nextLevel));
    if (startingLevel == currentLevel)
    {
      float num2 = Mathf.InverseLerp((float) record1.TotalXpRequired, (float) record2.TotalXpRequired, (float) startingExperience);
      return Mathf.InverseLerp((float) record1.TotalXpRequired, (float) record2.TotalXpRequired, (float) num1) - num2;
    }
    LettuceMercenaryLevelDbfRecord record3 = GameDbf.LettuceMercenaryLevel.GetRecord((Predicate<LettuceMercenaryLevelDbfRecord>) (r => r.Level == startingLevel + 1));
    LettuceMercenaryLevelDbfRecord record4 = GameDbf.LettuceMercenaryLevel.GetRecord((Predicate<LettuceMercenaryLevelDbfRecord>) (r => r.Level == currentLevel));
    return (float) (currentLevel - startingLevel - 1) + (1f - Mathf.InverseLerp((float) record1.TotalXpRequired, (float) record3.TotalXpRequired, (float) startingExperience)) + Mathf.InverseLerp((float) record4.TotalXpRequired, (float) record2.TotalXpRequired, (float) num1);
  }

  public static LettuceMercenaryLevelStatsDbfRecord GetMercenaryStatsByLevel(
    int mercenaryId,
    int level,
    out bool isMaxLevel)
  {
    int maxMercenaryLevel = GameUtils.GetMaxMercenaryLevel();
    int num = Mathf.Clamp(level, 1, maxMercenaryLevel);
    isMaxLevel = num == maxMercenaryLevel;
    LettuceMercenaryLevelDbfRecord mercenaryLevelDbfRecord1 = (LettuceMercenaryLevelDbfRecord) null;
    List<LettuceMercenaryLevelDbfRecord> records1 = GameDbf.LettuceMercenaryLevel.GetRecords();
    int index1 = 0;
    for (int count = records1.Count; index1 < count; ++index1)
    {
      LettuceMercenaryLevelDbfRecord mercenaryLevelDbfRecord2 = records1[index1];
      if (mercenaryLevelDbfRecord2.Level == num)
      {
        mercenaryLevelDbfRecord1 = mercenaryLevelDbfRecord2;
        break;
      }
    }
    if (mercenaryLevelDbfRecord1 == null)
    {
      Log.Lettuce.PrintError("GetMercenaryStatsByLevel() - Unable to get level dbf record for level {0}", (object) level);
      return (LettuceMercenaryLevelStatsDbfRecord) null;
    }
    LettuceMercenaryLevelStatsDbfRecord mercenaryStatsByLevel = (LettuceMercenaryLevelStatsDbfRecord) null;
    List<LettuceMercenaryLevelStatsDbfRecord> records2 = GameDbf.LettuceMercenaryLevelStats.GetRecords();
    int index2 = 0;
    for (int count = records2.Count; index2 < count; ++index2)
    {
      LettuceMercenaryLevelStatsDbfRecord levelStatsDbfRecord = records2[index2];
      if (levelStatsDbfRecord.LettuceMercenaryId == mercenaryId && levelStatsDbfRecord.LettuceMercenaryLevelId == mercenaryLevelDbfRecord1.ID)
      {
        mercenaryStatsByLevel = levelStatsDbfRecord;
        break;
      }
    }
    if (mercenaryStatsByLevel == null)
      Log.Lettuce.PrintError("GetMercenaryStatsByLevel() - Unable to get level stats dbf record for level {0}", (object) num);
    return mercenaryStatsByLevel;
  }

  public static bool IsFinalBossNodeType(int nodeTypeId)
  {
    LettuceMapNodeTypeDbfRecord record = GameDbf.LettuceMapNodeType.GetRecord(nodeTypeId);
    return record != null && record.BossType == LettuceMapNodeType.LettuceMapBossType.FINAL_BOSS;
  }

  public static TAG_ROLE GetMercenaryTagRoleFromProtoRole(Mercenary.Role role)
  {
    switch (role)
    {
      case Mercenary.Role.ROLE_CASTER:
        return TAG_ROLE.CASTER;
      case Mercenary.Role.ROLE_STRIKER:
        return TAG_ROLE.FIGHTER;
      case Mercenary.Role.ROLE_PROTECTOR:
        return TAG_ROLE.TANK;
      case Mercenary.Role.ROLE_NEUTRAL:
        return TAG_ROLE.NEUTRAL;
      default:
        return TAG_ROLE.INVALID;
    }
  }

  public static bool LoadAndPositionCardActor(
    string actorName,
    string heroCardID,
    TAG_PREMIUM premium,
    GameUtils.LoadActorCallback callback)
  {
    if (string.IsNullOrEmpty(heroCardID))
      return false;
    DefLoader.Get().LoadFullDef(heroCardID, (DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>) ((cardID, def, userData) => GameUtils.LoadAndPositionCardActor_OnFullDefLoaded(actorName, cardID, def, userData, callback)), (object) premium);
    return true;
  }

  private static void LoadAndPositionCardActor_OnFullDefLoaded(
    string actorName,
    string cardID,
    DefLoader.DisposableFullDef def,
    object userData,
    GameUtils.LoadActorCallback callback)
  {
    TAG_PREMIUM tagPremium = (TAG_PREMIUM) userData;
    GameUtils.LoadActorCallbackInfo callbackData1 = new GameUtils.LoadActorCallbackInfo()
    {
      fullDef = def,
      premium = tagPremium
    };
    AssetLoader.Get().InstantiatePrefab((AssetReference) actorName, (PrefabCallback<GameObject>) ((assetRef, go, callbackData) => GameUtils.LoadAndPositionActorCard_OnActorLoaded(assetRef, go, callbackData, callback)), (object) callbackData1, AssetLoadingOptions.IgnorePrefabPosition);
  }

  private static void LoadAndPositionActorCard_OnActorLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData,
    GameUtils.LoadActorCallback callback)
  {
    GameUtils.LoadActorCallbackInfo actorCallbackInfo = callbackData as GameUtils.LoadActorCallbackInfo;
    using (actorCallbackInfo.fullDef)
    {
      if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("GameUtils.OnHeroActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
      }
      else
      {
        Actor component = go.GetComponent<Actor>();
        if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        {
          Debug.LogWarning((object) string.Format("GameUtils.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
        }
        else
        {
          component.SetPremium(actorCallbackInfo.premium);
          component.SetEntityDef(actorCallbackInfo.fullDef.EntityDef);
          component.SetCardDef(actorCallbackInfo.fullDef.DisposableCardDef);
          component.UpdateAllComponents();
          component.gameObject.name = actorCallbackInfo.fullDef.CardDef.name + "_actor";
          if ((bool) UniversalInputManager.UsePhoneUI)
            LayerUtils.SetLayer(component.gameObject, GameLayer.IgnoreFullScreenEffects);
          GemObject healthObject = component.GetHealthObject();
          if ((UnityEngine.Object) healthObject != (UnityEngine.Object) null)
            healthObject.Hide();
          if (callback == null)
            return;
          callback(component);
        }
      }
    }
  }

  public static bool AtPrereleaseEvent() => FiresideGatheringManager.Get().IsPrerelease;

  public static bool IsBoosterWild(BoosterDbId boosterId) => boosterId != BoosterDbId.INVALID && GameUtils.IsBoosterWild(GameDbf.Booster.GetRecord((int) boosterId));

  public static bool IsBoosterWild(BoosterDbfRecord boosterRecord)
  {
    if (boosterRecord != null)
    {
      SpecialEventType standardEvent = boosterRecord.StandardEvent;
      switch (standardEvent)
      {
        case SpecialEventType.UNKNOWN:
        case SpecialEventType.IGNORE:
          break;
        default:
          if (SpecialEventManager.Get().HasEventEnded(standardEvent))
            return true;
          break;
      }
    }
    return false;
  }

  public static bool IsAdventureWild(AdventureDbId adventureId)
  {
    if (adventureId == AdventureDbId.INVALID)
      return false;
    AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) adventureId);
    if (record != null)
    {
      SpecialEventType standardEvent = record.StandardEvent;
      switch (standardEvent)
      {
        case SpecialEventType.UNKNOWN:
        case SpecialEventType.IGNORE:
          break;
        default:
          if (SpecialEventManager.Get().HasEventEnded(standardEvent))
            return true;
          break;
      }
    }
    return false;
  }

  private static bool GetSeasonMonthAndYear(
    int seasonId,
    int startId,
    int startMonth,
    int startYear,
    out int month,
    out int year)
  {
    month = 0;
    year = 0;
    if (seasonId < startId)
    {
      Debug.LogFormat("GetSeasonMonthAndYear called with invalid seasonId {0}. Launch season is 6.", (object) seasonId);
      return false;
    }
    int num = seasonId - startId + startMonth - 1;
    month = num % 12 + 1;
    year = startYear + num / 12;
    return true;
  }

  private static string GetSeasonName(int seasonId, int startId, int startMonth, int startYear)
  {
    int month;
    int year;
    if (!GameUtils.GetSeasonMonthAndYear(seasonId, startId, startMonth, startYear, out month, out year))
      return (string) null;
    string key = string.Format("GLUE_RANKED_SEASON_NAME_{0}", (object) seasonId);
    string monthFromDigits = GameStrings.GetMonthFromDigits(month);
    return GameStrings.HasKey(key) ? GameStrings.Format(key, (object) monthFromDigits, (object) year, (object) seasonId) : GameStrings.Format("GLUE_RANKED_SEASON_NAME_GENERIC", (object) monthFromDigits, (object) year, (object) seasonId);
  }

  public static string GetRankedSeasonName(int seasonId) => GameUtils.GetSeasonName(seasonId, 6, 4, 2014);

  public static string GetMercenariesSeasonName(int seasonId) => GameUtils.GetSeasonName(seasonId, 1, 11, 2021);

  public static string GetMercenariesSeasonEndDescription(int seasonId, int highestRating)
  {
    int month;
    int year;
    if (!GameUtils.GetSeasonMonthAndYear(seasonId, 1, 11, 2021, out month, out year))
      return (string) null;
    return GameStrings.Format("GLUE_LETTUCE_SEASON_ROLL_DESC", (object) GameStrings.GetMonthFromDigits(month), (object) year, (object) highestRating);
  }

  public static bool IsGSDFlagSet(GameSaveKeyId saveKey, GameSaveKeySubkeyId subkey)
  {
    if (!GameSaveDataManager.Get().IsDataReady(saveKey))
      return false;
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(saveKey, subkey, out num);
    return num > 0L;
  }

  public static void SetGSDFlag(GameSaveKeyId saveKey, GameSaveKeySubkeyId subkey, bool enableFlag)
  {
    if (GameUtils.IsGSDFlagSet(saveKey, subkey) == enableFlag)
      return;
    int num = enableFlag ? 1 : 0;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(saveKey, subkey, new long[1]
    {
      (long) num
    }));
  }

  public static bool IsGolden500HeroSkinAchievement(int achievementId)
  {
    foreach (KeyValuePair<TAG_CLASS, GameUtils.HeroSkinAchievements> keyValuePair in GameUtils.HERO_SKIN_ACHIEVEMENTS)
    {
      if (keyValuePair.Value.Golden500Win == achievementId)
        return true;
    }
    return false;
  }

  public static bool IsHonored1KHeroSkinAchievement(int achievementId)
  {
    foreach (KeyValuePair<TAG_CLASS, GameUtils.HeroSkinAchievements> keyValuePair in GameUtils.HERO_SKIN_ACHIEVEMENTS)
    {
      if (keyValuePair.Value.Honored1kWin == achievementId)
        return true;
    }
    return false;
  }

  public static bool HasClassTag(TAG_CLASS classTag, List<TAG_CLASS> tagsToCheck)
  {
    if (tagsToCheck == null)
      return false;
    foreach (TAG_CLASS tagClass in tagsToCheck)
    {
      if (tagClass == classTag)
        return true;
    }
    return false;
  }

  [Serializable]
  public class StringEvent : UnityEvent<string>
  {
  }

  public delegate void EmoteSoundLoaded(CardSoundSpell emoteObj);

  public delegate void LoadActorCallback(Actor actor);

  public class HeroSkinAchievements
  {
    public int Golden500Win { get; set; }

    public int Honored1kWin { get; set; }
  }

  private class LoadActorCallbackInfo
  {
    public DefLoader.DisposableFullDef fullDef;
    public TAG_PREMIUM premium;
  }
}
