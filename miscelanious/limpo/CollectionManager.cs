using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusLettuce;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionManager
{
  private Map<BattlegroundsHeroSkinId, int> m_BattlegroundsHeroSkinIdToHeroBaseCardId;
  private Map<BattlegroundsHeroSkinId, int> m_BattlegroundsHeroSkinIdToHeroSkinCardId;
  private Map<int, BattlegroundsHeroSkinId> m_BattlegroundsHeroSkinCardIdToHeroSkinId;
  private Map<int, int> m_BattlegroundsHeroSkinCardIdToHeroBaseCardId;
  private List<string> m_BattlegroundsHeroCardIds;
  private Map<BattlegroundsGuideSkinId, int> m_BattlegroundsGuideSkinIdToSkinCardId;
  private HashSet<BattlegroundsGuideSkinId> m_BattlegroundsGuideSkinIds;
  private List<string> m_BattlegroundsGuideCardIds;
  private static readonly string m_DefaultGuideCardId = "TB_BaconShopBob";
  private Map<int, BattlegroundsGuideSkinId> m_BattlegroundsGuideSkinCardIdToGuideSkinId;
  private HashSet<BattlegroundsBoardSkinId> m_BattlegroundsBoardSkinIds;
  private HashSet<BattlegroundsFinisherId> m_BattlegroundsFinisherIds;
  private HashSet<BattlegroundsEmoteId> m_BattlegroundsEmoteIds;
  public static PegasusShared.FormatType s_PreHeroPickerFormat = PegasusShared.FormatType.FT_STANDARD;
  public static PegasusShared.FormatType s_HeroPickerFormat = PegasusShared.FormatType.FT_STANDARD;
  private static Comparison<CollectibleCard> OrderedCardsSort = (Comparison<CollectibleCard>) ((a, b) =>
  {
    int num = a.ManaCost.CompareTo(b.ManaCost);
    if (num == 0)
    {
      num = string.Compare(a.Name, b.Name, false, Localization.GetCultureInfo());
      if (num == 0)
        num = CollectionManager.GetPremiumSortOrder(a.PremiumType).CompareTo(CollectionManager.GetPremiumSortOrder(b.PremiumType));
    }
    return num;
  });
  private static CollectionManager s_instance;
  private bool m_collectionLoaded;
  private bool m_achievesLoaded;
  private bool m_netCacheLoaded;
  private bool m_duelsSessionInfoLoaded;
  private Map<long, CollectionDeck> m_decks = new Map<long, CollectionDeck>();
  private Map<long, CollectionDeck> m_baseDecks = new Map<long, CollectionDeck>();
  private Map<TAG_CLASS, CollectionManager.PreconDeck> m_preconDecks = new Map<TAG_CLASS, CollectionManager.PreconDeck>();
  private Map<TAG_CLASS, List<CollectionManager.TemplateDeck>> m_templateDecks = new Map<TAG_CLASS, List<CollectionManager.TemplateDeck>>();
  private Map<int, CollectionManager.TemplateDeck> m_templateDeckMap = new Map<int, CollectionManager.TemplateDeck>();
  private CollectionDeck m_EditedDeck;
  private List<TAG_CARD_SET> m_displayableCardSets = new List<TAG_CARD_SET>();
  private List<CollectionManager.DelOnCollectionLoaded> m_collectionLoadedListeners = new List<CollectionManager.DelOnCollectionLoaded>();
  private List<CollectionManager.DelOnCollectionChanged> m_collectionChangedListeners = new List<CollectionManager.DelOnCollectionChanged>();
  private List<CollectionManager.DelOnDeckCreated> m_deckCreatedListeners = new List<CollectionManager.DelOnDeckCreated>();
  private List<CollectionManager.DelOnDeckDeleted> m_deckDeletedListeners = new List<CollectionManager.DelOnDeckDeleted>();
  private List<CollectionManager.DelOnDeckContents> m_deckContentsListeners = new List<CollectionManager.DelOnDeckContents>();
  private List<CollectionManager.DelOnAllDeckContents> m_allDeckContentsListeners = new List<CollectionManager.DelOnAllDeckContents>();
  private List<CollectionManager.DelOnNewCardSeen> m_newCardSeenListeners = new List<CollectionManager.DelOnNewCardSeen>();
  private List<CollectionManager.DelOnCardRewardsInserted> m_cardRewardListeners = new List<CollectionManager.DelOnCardRewardsInserted>();
  private List<CollectionManager.OnMassDisenchant> m_massDisenchantListeners = new List<CollectionManager.OnMassDisenchant>();
  private List<CollectionManager.OnEditedDeckChanged> m_editedDeckChangedListeners = new List<CollectionManager.OnEditedDeckChanged>();
  private List<System.Action> m_initialCollectionReceivedListeners = new List<System.Action>();
  private Map<long, float> m_pendingRequestDeckContents;
  private List<CollectibleCard> m_collectibleCards = new List<CollectibleCard>();
  private Map<int, int> m_coreCounterpartCardMap = new Map<int, int>();
  private Map<CollectionManager.CollectibleCardIndex, CollectibleCard> m_collectibleCardIndex;
  private float m_collectionLastModifiedTime;
  private DateTime? m_timeOfLastPlayerDeckSave;
  private bool m_accountHasWildCards;
  private float m_lastSearchForWildCardsTime;
  private List<System.Action> m_onNetCacheDecksProcessed = new List<System.Action>();
  private Dictionary<long, CollectionManager.DeckAutoFillCallback> m_smartDeckCallbackByDeckId = new Dictionary<long, CollectionManager.DeckAutoFillCallback>();
  private HashSet<long> m_decksToRequestContentsAfterDeckSetDataResonse = new HashSet<long>();
  private HashSet<int> m_inTransitDeckCreateRequests = new HashSet<int>();
  private HashSet<TAG_CARD_SET> m_filterCardSet = new HashSet<TAG_CARD_SET>((IEqualityComparer<TAG_CARD_SET>) new CollectionManager.TagCardSetEnumComparer());
  private HashSet<TAG_CLASS> m_filterCardClass = new HashSet<TAG_CLASS>((IEqualityComparer<TAG_CLASS>) new CollectionManager.TagClassEnumComparer());
  private HashSet<TAG_CARDTYPE> m_filterCardType = new HashSet<TAG_CARDTYPE>((IEqualityComparer<TAG_CARDTYPE>) new CollectionManager.TagCardTypeEnumComparer());
  private Map<TAG_CARD_SET, bool> m_filterIsSetRotatedCache;
  private Map<int, int> m_startsWithMatchNames = new Map<int, int>();
  private Map<string, TAG_CARD_SET> m_cachedCardSetValues = new Map<string, TAG_CARD_SET>();
  private List<TAG_CLASS> m_cardClasses = new List<TAG_CLASS>();
  private HashSet<int> m_UniqueHero = new HashSet<int>();
  private List<CollectionManager.FavoriteHeroChangedListener> m_favoriteHeroChangedListeners = new List<CollectionManager.FavoriteHeroChangedListener>();
  private List<CollectionManager.OnUIHeroOverrideCardRemovedListener> m_onUIHeroOverrideCardRemovedListeners = new List<CollectionManager.OnUIHeroOverrideCardRemovedListener>();
  private bool m_waitingForBoxTransition;
  private bool m_hasVisitedCollection;
  private bool m_editMode;
  private TAG_PREMIUM m_premiumPreference = TAG_PREMIUM.DIAMOND;
  private CollectibleDisplay m_collectibleDisplay;
  private CollectionManager.PendingDeckCreateData m_pendingDeckCreate;
  private List<CollectionManager.PendingDeckDeleteData> m_pendingDeckDeleteList;
  private List<CollectionManager.PendingDeckRenameData> m_pendingDeckRenameList;
  private List<CollectionManager.PendingDeckEditData> m_pendingDeckEditList;
  private long m_currentPVPDRDeckId;
  private DeckRuleset m_deckRuleset;
  private Dictionary<string, ShareableDeck> m_decksToCheatIn = new Dictionary<string, ShareableDeck>();
  private static Comparison<LettuceMercenary> OrderMercernaries = (Comparison<LettuceMercenary>) ((a, b) => string.Compare(a.m_mercName, b.m_mercName, false, Localization.GetCultureInfo()));
  private HashSet<TAG_ROLE> m_filterCardRole = new HashSet<TAG_ROLE>((IEqualityComparer<TAG_ROLE>) new CollectionManager.TagRoleEnumComparer());
  private Map<long, LettuceTeam> m_teams = new Map<long, LettuceTeam>();
  private long m_editingTeamID;
  private List<LettuceMercenary> m_collectibleMercenaries = new List<LettuceMercenary>();
  private Map<long, LettuceMercenary> m_collectibleMercenaryDBIds = new Map<long, LettuceMercenary>();
  private List<LettuceMercenary> m_extraMercenaries = new List<LettuceMercenary>();
  private Map<long, LettuceMercenary> m_extraMercenaryDBIds = new Map<long, LettuceMercenary>();
  private List<CollectionManager.DelOnTeamCreated> m_teamCreatedListeners = new List<CollectionManager.DelOnTeamCreated>();
  private List<CollectionManager.DelOnTeamDeleted> m_teamDeletedListeners = new List<CollectionManager.DelOnTeamDeleted>();
  private List<CollectionManager.DelOnTeamContents> m_teamContentsListeners = new List<CollectionManager.DelOnTeamContents>();
  private List<CollectionManager.DelOnAllTeamContents> m_allTeamContentsListeners = new List<CollectionManager.DelOnAllTeamContents>();
  private List<CollectionManager.OnEditingTeamChanged> m_editingTeamChangedListeners = new List<CollectionManager.OnEditingTeamChanged>();
  private HashSet<long> m_teamsToRequestContentsAfterTeamSetDataResonse = new HashSet<long>();
  private HashSet<int> m_inTransitTeamCreateRequests = new HashSet<int>();
  private bool m_editTeamMode;
  private bool m_initialDataRequested;
  private bool m_mercsAndTeamsReceived;
  private bool m_playerInfoReceived;
  private bool m_hasVisitedDetailsDisplay;
  private MercenariesCollectionResponse m_mercenariesCollectionResponse;
  private LettuceTeamList m_mercTeamListResponse;
  private CollectionManager.PendingTeamCreateData m_pendingTeamCreate;
  private List<CollectionManager.PendingTeamDeleteData> m_pendingTeamDeleteList;
  private List<CollectionManager.PendingTeamEditData> m_pendingTeamEditList;
  private List<CollectionManager.PendingMercenaryEditData> m_pendingMercenaryEditList;

  private void BattlegroundsDataInit()
  {
    this.m_BattlegroundsHeroSkinIdToHeroBaseCardId = new Map<BattlegroundsHeroSkinId, int>();
    this.m_BattlegroundsHeroSkinIdToHeroSkinCardId = new Map<BattlegroundsHeroSkinId, int>();
    this.m_BattlegroundsHeroSkinCardIdToHeroSkinId = new Map<int, BattlegroundsHeroSkinId>();
    this.m_BattlegroundsHeroSkinCardIdToHeroBaseCardId = new Map<int, int>();
    this.m_BattlegroundsHeroCardIds = new List<string>();
    this.m_BattlegroundsGuideSkinIds = new HashSet<BattlegroundsGuideSkinId>();
    this.m_BattlegroundsGuideCardIds = new List<string>();
    this.m_BattlegroundsGuideSkinCardIdToGuideSkinId = new Map<int, BattlegroundsGuideSkinId>();
    this.m_BattlegroundsGuideSkinIdToSkinCardId = new Map<BattlegroundsGuideSkinId, int>();
    this.m_BattlegroundsBoardSkinIds = new HashSet<BattlegroundsBoardSkinId>();
    this.m_BattlegroundsFinisherIds = new HashSet<BattlegroundsFinisherId>();
    this.m_BattlegroundsEmoteIds = new HashSet<BattlegroundsEmoteId>();
    foreach (BattlegroundsHeroSkinDbfRecord record in GameDbf.BattlegroundsHeroSkin.GetRecords())
    {
      BattlegroundsHeroSkinId key = BattlegroundsHeroSkinId.FromTrustedValue(record.ID);
      this.m_BattlegroundsHeroSkinIdToHeroBaseCardId[key] = record.BaseCardId;
      this.m_BattlegroundsHeroSkinIdToHeroSkinCardId[key] = record.SkinCardId;
      this.m_BattlegroundsHeroSkinCardIdToHeroSkinId[record.SkinCardId] = key;
      this.m_BattlegroundsHeroSkinCardIdToHeroBaseCardId[record.SkinCardId] = record.BaseCardId;
    }
    foreach (CardHeroDbfRecord record in GameDbf.CardHero.GetRecords((Predicate<CardHeroDbfRecord>) (card_hero => card_hero.HeroType == CardHero.HeroType.BATTLEGROUNDS_HERO)))
      this.m_BattlegroundsHeroCardIds.Add(GameUtils.TranslateDbIdToCardId(record.CardId));
    foreach (BattlegroundsGuideSkinDbfRecord record in GameDbf.BattlegroundsGuideSkin.GetRecords())
    {
      BattlegroundsGuideSkinId key = BattlegroundsGuideSkinId.FromTrustedValue(record.ID);
      this.m_BattlegroundsGuideSkinIds.Add(key);
      this.m_BattlegroundsGuideSkinIdToSkinCardId[key] = record.SkinCardId;
      this.m_BattlegroundsGuideSkinCardIdToGuideSkinId[record.SkinCardId] = key;
    }
    foreach (CardHeroDbfRecord record in GameDbf.CardHero.GetRecords((Predicate<CardHeroDbfRecord>) (card_hero => card_hero.HeroType == CardHero.HeroType.BATTLEGROUNDS_GUIDE)))
      this.m_BattlegroundsGuideCardIds.Add(GameUtils.TranslateDbIdToCardId(record.CardId));
    foreach (DbfRecord record in GameDbf.BattlegroundsBoardSkin.GetRecords())
      this.m_BattlegroundsBoardSkinIds.Add(BattlegroundsBoardSkinId.FromTrustedValue(record.ID));
    foreach (DbfRecord record in GameDbf.BattlegroundsFinisher.GetRecords())
      this.m_BattlegroundsFinisherIds.Add(BattlegroundsFinisherId.FromTrustedValue(record.ID));
    foreach (DbfRecord record in GameDbf.BattlegroundsEmote.GetRecords())
      this.m_BattlegroundsEmoteIds.Add(BattlegroundsEmoteId.FromTrustedValue(record.ID));
  }

  public bool IsValidBattlegroundsHeroSkinId(BattlegroundsHeroSkinId skinId) => this.m_BattlegroundsHeroSkinIdToHeroBaseCardId.ContainsKey(skinId);

  public bool IsValidBattlegroundsGuideSkinId(BattlegroundsGuideSkinId skinId) => this.m_BattlegroundsGuideSkinIds.Contains(skinId);

  public bool IsValidBattlegroundsBoardSkinId(BattlegroundsBoardSkinId skinId) => this.m_BattlegroundsBoardSkinIds.Contains(skinId);

  public bool IsValidBattlegroundsFinisherId(BattlegroundsFinisherId finisherId) => this.m_BattlegroundsFinisherIds.Contains(finisherId);

  public bool IsValidBattlegroundsEmoteId(BattlegroundsEmoteId emoteId) => this.m_BattlegroundsEmoteIds.Contains(emoteId);

  public bool GetBattlegroundsBaseCardIdForHeroSkinId(
    BattlegroundsHeroSkinId skinId,
    out int baseHeroCardId)
  {
    return this.m_BattlegroundsHeroSkinIdToHeroBaseCardId.TryGetValue(skinId, out baseHeroCardId);
  }

  public bool GetBattlegroundsHeroSkinCardIdForSkinId(
    BattlegroundsHeroSkinId skinId,
    out int skinHeroCardId)
  {
    return this.m_BattlegroundsHeroSkinIdToHeroSkinCardId.TryGetValue(skinId, out skinHeroCardId);
  }

  public bool GetBattlegroundsHeroSkinIdForSkinCardId(
    int skinCardId,
    out BattlegroundsHeroSkinId skinId)
  {
    return this.m_BattlegroundsHeroSkinCardIdToHeroSkinId.TryGetValue(skinCardId, out skinId);
  }

  public string GetBattlegroundsBaseHeroCardId(string skinOrBaseCardId)
  {
    int dbId1 = GameUtils.TranslateCardIdToDbId(skinOrBaseCardId);
    if (dbId1 == 0)
    {
      Log.CollectionManager.PrintError("GetBattlegroundsBaseCardId: could not find card with ID: {0}", (object) skinOrBaseCardId);
      return skinOrBaseCardId;
    }
    if (!this.m_BattlegroundsHeroSkinCardIdToHeroBaseCardId.ContainsKey(dbId1))
      return skinOrBaseCardId;
    int dbId2 = this.m_BattlegroundsHeroSkinCardIdToHeroBaseCardId[dbId1];
    string cardId = GameUtils.TranslateDbIdToCardId(dbId2);
    if (cardId != null && cardId.Length != 0)
      return cardId;
    Log.CollectionManager.PrintError("GetBattlegroundsBaseCardId: could not find base card ID string for ID: {0}", (object) dbId2);
    return skinOrBaseCardId;
  }

  public List<string> GetAllBattlegroundsHeroCardIds() => this.m_BattlegroundsHeroCardIds;

  public List<string> GetAllBattlegroundsGuideCardIds() => this.m_BattlegroundsGuideCardIds;

  public bool GetBattlegroundsGuideSkinCardIdForSkinId(
    BattlegroundsGuideSkinId skinId,
    out int cardId)
  {
    return this.m_BattlegroundsGuideSkinIdToSkinCardId.TryGetValue(skinId, out cardId);
  }

  public string GetFavoriteBattlegroundsGuideSkinCardId()
  {
    NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    if (netObject == null)
    {
      Log.CollectionManager.PrintError("Trying to invoke GetFavoriteBattlegroundsGuideSkinCardId before protobuf response from server.");
      return CollectionManager.m_DefaultGuideCardId;
    }
    BattlegroundsGuideSkinId? favoriteGuideSkin = netObject.BattlegroundsFavoriteGuideSkin;
    if (!favoriteGuideSkin.HasValue)
      return CollectionManager.m_DefaultGuideCardId;
    int dbId;
    if (this.m_BattlegroundsGuideSkinIdToSkinCardId.TryGetValue(favoriteGuideSkin.Value, out dbId))
      return GameUtils.TranslateDbIdToCardId(dbId);
    Log.CollectionManager.PrintError("GetFavoriteBattlegroundsGuideSkinCardId: Could not find card for skin id: {1}", (object) favoriteGuideSkin);
    return CollectionManager.m_DefaultGuideCardId;
  }

  public bool IsBattlegroundsGuideCardId(string cardId) => this.m_BattlegroundsGuideCardIds.Contains(cardId);

  public bool OwnsBattlegroundsHeroSkin(string skinCardId)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(skinCardId);
    return cardRecord != null && this.OwnsBattlegroundsHeroSkin(cardRecord.ID);
  }

  public bool OwnsBattlegroundsHeroSkin(int skinCardId)
  {
    BattlegroundsHeroSkinId battlegroundsHeroSkinId;
    return this.m_BattlegroundsHeroSkinCardIdToHeroSkinId.TryGetValue(skinCardId, out battlegroundsHeroSkinId) && NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>().OwnedBattlegroundsSkins.Contains(battlegroundsHeroSkinId);
  }

  public bool IsBattlegroundsHeroCard(string cardId) => this.m_BattlegroundsHeroCardIds.Contains(cardId);

  public bool IsBattlegroundsBaseHeroCardWithSkin(int cardId) => this.m_BattlegroundsHeroSkinIdToHeroBaseCardId.ContainsValue(cardId);

  public bool IsBattlegroundsHeroSkinCard(int cardId) => this.m_BattlegroundsHeroSkinCardIdToHeroSkinId.ContainsKey(cardId);

  public bool IsBattlegroundsGuideSkinCard(int cardId) => this.m_BattlegroundsGuideSkinCardIdToGuideSkinId.ContainsKey(cardId);

  public bool GetFavoriteBattlegroundsHeroSkin(
    int cardId,
    out BattlegroundsHeroSkinId favoriteSkinId)
  {
    NetCache.NetCacheBattlegroundsHeroSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
    if (netObject != null && netObject.BattlegroundsFavoriteHeroSkins.TryGetValue(cardId, out favoriteSkinId))
      return true;
    favoriteSkinId = new BattlegroundsHeroSkinId();
    return false;
  }

  public string GetFavoriteBattleGroundsHeroSkinCardId(int cardDbId)
  {
    BattlegroundsHeroSkinId favoriteSkinId;
    if (!this.GetFavoriteBattlegroundsHeroSkin(cardDbId, out favoriteSkinId))
      return GameUtils.TranslateDbIdToCardId(cardDbId);
    BattlegroundsHeroSkinDbfRecord record = GameDbf.BattlegroundsHeroSkin.GetRecord(favoriteSkinId.ToValue());
    if (record != null && record.SkinCardRecord != null)
      return record.SkinCardRecord.NoteMiniGuid;
    Log.CollectionManager.PrintError("GetFavoriteBattleGroundsHeroSkinCardId: Unable to retrieve record for skinId [{1}]", (object) favoriteSkinId.ToValue());
    return GameUtils.TranslateDbIdToCardId(cardDbId);
  }

  public bool HasFavoriteBattlegroundsGuideSkin()
  {
    NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    return netObject != null && netObject.BattlegroundsFavoriteGuideSkin.HasValue;
  }

  public bool GetFavoriteBattlegroundsGuideSkin(out BattlegroundsGuideSkinId favoriteSkinId)
  {
    NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    if (netObject != null && netObject.BattlegroundsFavoriteGuideSkin.HasValue)
    {
      favoriteSkinId = netObject.BattlegroundsFavoriteGuideSkin.Value;
      return true;
    }
    favoriteSkinId = new BattlegroundsGuideSkinId();
    return false;
  }

  public bool GetBattlegroundsGuideSkinIdForCardId(
    int skinCardId,
    out BattlegroundsGuideSkinId skinId)
  {
    return this.m_BattlegroundsGuideSkinCardIdToGuideSkinId.TryGetValue(skinCardId, out skinId);
  }

  public bool OwnsBattlegroundsGuideSkin(string skinCardId)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(skinCardId);
    return cardRecord != null && this.OwnsBattlegroundsGuideSkin(cardRecord.ID);
  }

  public bool OwnsBattlegroundsGuideSkin(int skinCardId)
  {
    BattlegroundsGuideSkinId battlegroundsGuideSkinId;
    if (!this.m_BattlegroundsGuideSkinCardIdToGuideSkinId.TryGetValue(skinCardId, out battlegroundsGuideSkinId))
      return false;
    NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    return netObject != null && netObject.OwnedBattlegroundsGuideSkins.Contains(battlegroundsGuideSkinId);
  }

  public bool OwnsAssociatedBattlegroundsHeroSkin(int baseCardId)
  {
    NetCache.NetCacheBattlegroundsHeroSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
    if (netObject == null)
      return false;
    foreach (KeyValuePair<BattlegroundsHeroSkinId, int> keyValuePair in this.m_BattlegroundsHeroSkinIdToHeroBaseCardId)
    {
      if (keyValuePair.Value == baseCardId && netObject.OwnedBattlegroundsSkins.Contains(keyValuePair.Key))
        return true;
    }
    return false;
  }

  public bool OwnsAnyBattlegroundsGuideSkin()
  {
    NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    return netObject != null && netObject.OwnedBattlegroundsGuideSkins.Count > 0;
  }

  public bool OwnsBattlegroundsBoardSkin(BattlegroundsBoardSkinId skinId)
  {
    if (skinId.IsDefaultBoard())
      return true;
    NetCache.NetCacheBattlegroundsBoardSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
    return netObject != null && netObject.OwnedBattlegroundsBoardSkins.Contains(skinId);
  }

  public bool IsFavoriteBattlegroundsBoardSkin(BattlegroundsBoardSkinId skinId)
  {
    NetCache.NetCacheBattlegroundsBoardSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
    if (netObject == null)
      return false;
    if (!netObject.BattlegroundsFavoriteBoardSkin.HasValue)
      return skinId.IsDefaultBoard();
    BattlegroundsBoardSkinId? favoriteBoardSkin = netObject.BattlegroundsFavoriteBoardSkin;
    BattlegroundsBoardSkinId battlegroundsBoardSkinId = skinId;
    if (!favoriteBoardSkin.HasValue)
      return false;
    return !favoriteBoardSkin.HasValue || favoriteBoardSkin.GetValueOrDefault() == battlegroundsBoardSkinId;
  }

  public bool OwnsBattlegroundsFinisher(BattlegroundsFinisherId finisherId)
  {
    if (finisherId.IsDefaultFinisher())
      return true;
    NetCache.NetCacheBattlegroundsFinishers netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
    return netObject != null && netObject.OwnedBattlegroundsFinishers.Contains(finisherId);
  }

  public bool IsFavoriteBattlegroundsFinisher(BattlegroundsFinisherId finisherId)
  {
    NetCache.NetCacheBattlegroundsFinishers netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
    if (netObject == null)
      return false;
    if (!netObject.BattlegroundsFavoriteFinisher.HasValue)
      return finisherId.IsDefaultFinisher();
    BattlegroundsFinisherId? favoriteFinisher = netObject.BattlegroundsFavoriteFinisher;
    BattlegroundsFinisherId battlegroundsFinisherId = finisherId;
    if (!favoriteFinisher.HasValue)
      return false;
    return !favoriteFinisher.HasValue || favoriteFinisher.GetValueOrDefault() == battlegroundsFinisherId;
  }

  public bool OwnsBattlegroundsEmote(BattlegroundsEmoteId emoteId)
  {
    if (emoteId.IsDefaultEmote())
      return true;
    NetCache.NetCacheBattlegroundsEmotes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsEmotes>();
    return netObject != null && netObject.OwnedBattlegroundsEmotes.Contains(emoteId);
  }

  public bool HasAnyNewBattlegroundsSkins() => this.CountNewBattlegroundsHeroSkins() > 0 || this.CountNewBattlegroundsGuideSkins() > 0 || this.CountNewBattlegroundsBoardSkins() > 0 || this.CountNewBattlegroundsFinishers() > 0 || this.CountNewBattlegroundsEmotes() > 0;

  public int CountNewBattlegroundsHeroSkins() => NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>().UnseenSkinIds.Count;

  public int CountNewBattlegroundsGuideSkins() => NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>().UnseenSkinIds.Count;

  public int CountNewBattlegroundsBoardSkins() => NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>().UnseenSkinIds.Count;

  public int CountNewBattlegroundsFinishers()
  {
    NetCache.NetCacheBattlegroundsFinishers netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
    return netObject == null ? 0 : netObject.UnseenSkinIds.Count;
  }

  public int CountNewBattlegroundsEmotes()
  {
    NetCache.NetCacheBattlegroundsEmotes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsEmotes>();
    return netObject == null ? 0 : netObject.UnseenEmoteIds.Count;
  }

  public void MarkBattlegroundsHeroSkinSeen(string skinCardId, TAG_PREMIUM premium)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(skinCardId);
    BattlegroundsHeroSkinId skinId;
    if (cardRecord == null || !this.m_BattlegroundsHeroSkinCardIdToHeroSkinId.TryGetValue(cardRecord.ID, out skinId) || !Network.Get().TryAddSeenBattlegroundsHeroSkin(skinId))
      return;
    foreach (CollectionManager.DelOnNewCardSeen cardSeenListener in this.m_newCardSeenListeners)
      cardSeenListener(skinCardId, premium);
  }

  public void MarkBattlegroundsGuideSkinSeen(string skinCardId, TAG_PREMIUM premium)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(skinCardId);
    BattlegroundsGuideSkinId skinId;
    if (cardRecord == null || !this.m_BattlegroundsGuideSkinCardIdToGuideSkinId.TryGetValue(cardRecord.ID, out skinId) || !Network.Get().TryAddSeenBattlegroundsGuideSkin(skinId))
      return;
    foreach (CollectionManager.DelOnNewCardSeen cardSeenListener in this.m_newCardSeenListeners)
      cardSeenListener(skinCardId, premium);
  }

  public void MarkBattlegroundsBoardSkinSeen(BattlegroundsBoardSkinId skinId)
  {
    if (skinId.IsDefaultBoard() || !Network.Get().TryAddSeenBattlegroundsBoardSkin(skinId))
      return;
    this.OnCollectionChanged();
  }

  public void MarkBattlegroundsFinisherSeen(BattlegroundsFinisherId finisherId)
  {
    if (finisherId.IsDefaultFinisher() || !Network.Get().TryAddSeenBattlegroundsFinisher(finisherId))
      return;
    this.OnCollectionChanged();
  }

  public void MarkBattlegroundsEmoteSeen(BattlegroundsEmoteId emoteId)
  {
    if (emoteId.IsDefaultEmote() || !Network.Get().TryAddSeenBattlegroundsEmote(emoteId))
      return;
    this.OnCollectionChanged();
  }

  public bool ShouldShowNewBattlegroundsHeroSkinGlow(string skinCardId)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(skinCardId);
    BattlegroundsHeroSkinId battlegroundsHeroSkinId;
    if (cardRecord == null || !this.m_BattlegroundsHeroSkinCardIdToHeroSkinId.TryGetValue(cardRecord.ID, out battlegroundsHeroSkinId))
      return false;
    NetCache.NetCacheBattlegroundsHeroSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
    if (netObject != null)
      return netObject.UnseenSkinIds.Contains(battlegroundsHeroSkinId);
    Log.CollectionManager.PrintError("Trying to invoke ShouldShowNewBattlegroundsHeroSkinGlow before protobuf response from server.");
    return false;
  }

  public bool ShouldShowNewBattlegroundsGuideSkinGlow(string skinCardId)
  {
    CardDbfRecord cardRecord = GameUtils.GetCardRecord(skinCardId);
    BattlegroundsGuideSkinId battlegroundsGuideSkinId;
    if (cardRecord == null || !this.m_BattlegroundsGuideSkinCardIdToGuideSkinId.TryGetValue(cardRecord.ID, out battlegroundsGuideSkinId))
      return false;
    NetCache.NetCacheBattlegroundsGuideSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
    if (netObject != null)
      return netObject.UnseenSkinIds.Contains(battlegroundsGuideSkinId);
    Log.CollectionManager.PrintError("Trying to invoke ShouldShowNewBattlegroundsGuideSkinGlow before protobuf response from server.");
    return false;
  }

  public bool ShouldShowNewBattlegroundsBoardSkinGlow(BattlegroundsBoardSkinId skinId)
  {
    if (skinId.IsDefaultBoard())
      return false;
    NetCache.NetCacheBattlegroundsBoardSkins netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
    if (netObject != null)
      return netObject.UnseenSkinIds.Contains(skinId);
    Log.CollectionManager.PrintError("Trying to invoke ShouldShowNewBattlegroundsBoardSkinGlow before protobuf response from server.");
    return false;
  }

  public bool ShouldShowNewBattlegroundsFinisherGlow(BattlegroundsFinisherId finisherId)
  {
    if (finisherId.IsDefaultFinisher())
      return false;
    NetCache.NetCacheBattlegroundsFinishers netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsFinishers>();
    if (netObject != null)
      return netObject.UnseenSkinIds.Contains(finisherId);
    Log.CollectionManager.PrintError("Trying to invoke ShouldShowNewBattlegroundsFinisherGlow before protobuf response from server.");
    return false;
  }

  public bool ShouldShowNewBattlegroundsEmoteGlow(BattlegroundsEmoteId emoteId)
  {
    if (emoteId.IsDefaultEmote())
      return false;
    NetCache.NetCacheBattlegroundsEmotes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsEmotes>();
    if (netObject != null)
      return netObject.UnseenEmoteIds.Contains(emoteId);
    Log.CollectionManager.PrintError("Trying to invoke ShouldShowNewBattlegroundsEmoteGlow before protobuf response from server.");
    return false;
  }

  public BattlegroundsEmoteLoadoutDataModel CreateEmoteLoadoutDataModel()
  {
    NetCache.NetCacheBattlegroundsEmotes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsEmotes>();
    BattlegroundsEmoteLoadoutDataModel loadoutDataModel = new BattlegroundsEmoteLoadoutDataModel();
    loadoutDataModel.EmoteList = new DataModelList<BattlegroundsEmoteDataModel>();
    if (netObject == null)
    {
      Log.CollectionManager.PrintError("Trying to invoke CreateEmoteLoadoutDataModel before protobuf response from server.");
      return loadoutDataModel;
    }
    if (netObject.CurrentLoadout != (Hearthstone.BattlegroundsEmoteLoadout) null)
    {
      foreach (BattlegroundsEmoteId emote in netObject.CurrentLoadout.Emotes)
      {
        BattlegroundsEmoteDbfRecord record = GameDbf.BattlegroundsEmote.GetRecord(emote.ToValue());
        if (record != null)
        {
          CollectibleBattlegroundsEmote battlegroundsEmote = new CollectibleBattlegroundsEmote(record);
          loadoutDataModel.EmoteList.Add(battlegroundsEmote.CreateEmoteDataModel());
        }
        else
          loadoutDataModel.EmoteList.Add(new BattlegroundsEmoteDataModel());
      }
    }
    return loadoutDataModel;
  }

  public bool IsEquippedBattlegroundsEmote(BattlegroundsEmoteId emoteId)
  {
    BaconCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as BaconCollectionDisplay;
    bool inLoadout = false;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.TryCheckEmoteInLoadout(emoteId.ToValue(), out inLoadout))
      return inLoadout;
    NetCache.NetCacheBattlegroundsEmotes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBattlegroundsEmotes>();
    if (netObject == null)
    {
      Log.CollectionManager.PrintError("Trying to invoke ShouldShowNewBattlegroundsEmoteGlow before protobuf response from server.");
      return false;
    }
    for (int index = 0; index < netObject.CurrentLoadout.Emotes.Length; ++index)
    {
      if (emoteId.Equals(netObject.CurrentLoadout.Emotes[index]))
        return true;
    }
    return false;
  }

  private static int GetPremiumSortOrder(TAG_PREMIUM premiumType)
  {
    switch (premiumType)
    {
      case TAG_PREMIUM.NORMAL:
        return 0;
      case TAG_PREMIUM.GOLDEN:
        return 1;
      case TAG_PREMIUM.DIAMOND:
        return 3;
      case TAG_PREMIUM.SIGNATURE:
        return 2;
      default:
        Debug.LogWarning((object) "CollectionManager.GetPremiumSortOrder - Unknown premium type");
        return (int) premiumType;
    }
  }

  public bool HasSeenOvercappedDeckInfoPopup { get; set; }

  public bool HasSeenExtraRunesDeckInfoPopup { get; set; }

  public static event CollectionManager.DelCollectionManagerReady OnCollectionManagerReady;

  public NetCache.NetCacheCollection OnInitialCollectionReceived(Collection collection)
  {
    NetCache.NetCacheCollection netCacheCollection = new NetCache.NetCacheCollection();
    if (collection == null)
      return netCacheCollection;
    List<string> stringList = new List<string>();
    for (int index = 0; index < collection.Stacks.Count; ++index)
    {
      PegasusShared.CardStack stack = collection.Stacks[index];
      NetCache.CardStack netStack = new NetCache.CardStack();
      netStack.Def.Name = GameUtils.TranslateDbIdToCardId(stack.CardDef.Asset);
      if (string.IsNullOrEmpty(netStack.Def.Name))
      {
        Error.AddDevFatal("CollectionManager.OnInitialCollectionReceived: failed to find a card with databaseId: {0}", (object) stack.CardDef.Asset);
        stringList.Add(stack.CardDef.Asset.ToString());
      }
      else
      {
        netStack.Def.Premium = (TAG_PREMIUM) stack.CardDef.Premium;
        netStack.Date = TimeUtils.PegDateToFileTimeUtc(stack.LatestInsertDate);
        netStack.Count = stack.Count;
        netStack.NumSeen = stack.NumSeen;
        netCacheCollection.Stacks.Add(netStack);
        netCacheCollection.TotalCardsOwned += netStack.Count;
        if (GameUtils.IsCardCollectible(netStack.Def.Name))
        {
          EntityDef entityDef = DefLoader.Get().GetEntityDef(netStack.Def.Name);
          this.SetCounts(netStack, entityDef);
          if (entityDef.IsCoreCard() && netStack.Def.Premium == TAG_PREMIUM.NORMAL)
            netCacheCollection.CoreCardsUnlockedPerClass[entityDef.GetClass()].Add(entityDef.GetCardId());
        }
      }
    }
    foreach (System.Action action in this.m_initialCollectionReceivedListeners.ToArray())
      action();
    if (stringList.Count > 0)
      Error.AddDevWarning("Card Errors", "CollectionManager.OnInitialCollectionRecieved: Cards with the following dbIds could not be found:\n{0}", (object) string.Join(", ", stringList.ToArray()));
    this.BuildCoreCounterpartMap();
    return netCacheCollection;
  }

  private void OnCardSale()
  {
    Network.CardSaleResult cardSaleResult = Network.Get().GetCardSaleResult();
    bool flag;
    switch (cardSaleResult.Action)
    {
      case Network.CardSaleResult.SaleResult.GENERIC_FAILURE:
        CraftingManager.Get().OnCardGenericError(cardSaleResult);
        flag = false;
        break;
      case Network.CardSaleResult.SaleResult.CARD_WAS_SOLD:
        CraftingManager.Get().OnCardDisenchanted(cardSaleResult);
        flag = true;
        break;
      case Network.CardSaleResult.SaleResult.CARD_WAS_BOUGHT:
        CraftingManager.Get().OnCardCreated(cardSaleResult);
        flag = true;
        break;
      case Network.CardSaleResult.SaleResult.SOULBOUND:
        CraftingManager.Get().OnCardDisenchantSoulboundError(cardSaleResult);
        flag = false;
        break;
      case Network.CardSaleResult.SaleResult.FAILED_WRONG_SELL_PRICE:
        CraftingManager.Get().OnCardValueChangedError(cardSaleResult);
        flag = false;
        break;
      case Network.CardSaleResult.SaleResult.FAILED_WRONG_BUY_PRICE:
        CraftingManager.Get().OnCardValueChangedError(cardSaleResult);
        flag = false;
        break;
      case Network.CardSaleResult.SaleResult.FAILED_NO_PERMISSION:
        CraftingManager.Get().OnCardPermissionError(cardSaleResult);
        flag = false;
        break;
      case Network.CardSaleResult.SaleResult.FAILED_EVENT_NOT_ACTIVE:
        CraftingManager.Get().OnCardCraftingEventNotActiveError(cardSaleResult);
        flag = false;
        break;
      case Network.CardSaleResult.SaleResult.COUNT_MISMATCH:
        CraftingManager.Get().OnCardCountError(cardSaleResult);
        flag = false;
        break;
      case Network.CardSaleResult.SaleResult.CARD_WAS_UPGRADED:
        CraftingManager.Get().OnCardUpgraded(cardSaleResult);
        flag = true;
        break;
      default:
        CraftingManager.Get().OnCardUnknownError(cardSaleResult);
        flag = false;
        break;
    }
    string str = string.Format("CollectionManager.OnCardSale {0} for card {1} (asset {2}) premium {3}", (object) cardSaleResult.Action, (object) cardSaleResult.AssetName, (object) cardSaleResult.AssetID, (object) cardSaleResult.Premium);
    if (!flag)
    {
      Debug.LogWarning((object) str);
    }
    else
    {
      Log.Crafting.Print(str);
      this.OnCollectionChanged();
    }
  }

  private void OnMassDisenchantResponse()
  {
    Network.MassDisenchantResponse disenchantResponse = Network.Get().GetMassDisenchantResponse();
    if (disenchantResponse.Amount == 0)
    {
      Debug.LogError((object) "CollectionManager.OnMassDisenchantResponse(): Amount is 0. This means the backend failed to mass disenchant correctly.");
    }
    else
    {
      foreach (CollectionManager.OnMassDisenchant onMassDisenchant in this.m_massDisenchantListeners.ToArray())
        onMassDisenchant(disenchantResponse.Amount);
    }
  }

  public void UpdateFavoriteHero(
    TAG_CLASS heroClass,
    string heroCardId,
    TAG_PREMIUM premium,
    bool isFavorite)
  {
    if (this.m_favoriteHeroChangedListeners.Count <= 0)
      return;
    NetCache.CardDefinition favoriteHero = new NetCache.CardDefinition();
    favoriteHero.Name = heroCardId;
    favoriteHero.Premium = premium;
    foreach (CollectionManager.FavoriteHeroChangedListener heroChangedListener in this.m_favoriteHeroChangedListeners.ToArray())
      heroChangedListener.Fire(heroClass, favoriteHero, isFavorite);
  }

  private void OnPVPDRSessionInfoResponse()
  {
    this.m_currentPVPDRDeckId = 0L;
    PVPDRSessionInfoResponse sessionInfoResponse = Network.Get().GetPVPDRSessionInfoResponse();
    if (sessionInfoResponse.HasSession)
      this.m_currentPVPDRDeckId = sessionInfoResponse.Session.DeckId;
    this.m_duelsSessionInfoLoaded = true;
  }

  public bool IsDuelsSessionInfoLoaded() => this.m_duelsSessionInfoLoaded;

  public void NetCache_OnDecksReceived()
  {
    foreach (NetCache.DeckHeader deck in NetCache.Get().GetNetObject<NetCache.NetCacheDecks>().Decks)
    {
      if (deck.Type == DeckType.NORMAL_DECK && this.GetDeck(deck.ID) == null && DefLoader.Get().GetEntityDef(deck.Hero) != null)
        this.AddDeck(deck, false);
    }
    for (int index = this.m_onNetCacheDecksProcessed.Count - 1; index >= 0; --index)
      this.m_onNetCacheDecksProcessed[index]();
  }

  public void AddOnNetCacheDecksProcessedListener(System.Action a) => this.m_onNetCacheDecksProcessed.Add(a);

  public void RemoveOnNetCacheDecksProcessedListener(System.Action a) => this.m_onNetCacheDecksProcessed.Remove(a);

  public void OnFavoriteBattlegroundsGuideSkinChanged(
    BattlegroundsGuideSkinId? newFavoriteBattlegroundsGuideSkinID)
  {
  }

  public void OnInitialClientStateDeckContents(
    NetCache.NetCacheDecks netCacheDecks,
    List<PegasusUtil.DeckContents> deckContents)
  {
    if (deckContents == null)
      return;
    foreach (NetCache.DeckHeader deck in netCacheDecks.Decks)
    {
      if (deck.Type != DeckType.PRECON_DECK)
        this.AddDeck(deck, false);
    }
    this.UpdateFromDeckContents(deckContents);
  }

  private void OnGetDeckContentsResponse() => this.UpdateFromDeckContents(Network.Get().GetDeckContentsResponse().Decks);

  public void UpdateFromDeckContents(List<PegasusUtil.DeckContents> deckContents)
  {
    if (deckContents == null)
    {
      Log.CollectionManager.PrintError("Could not update CollectionManager from Deck Contents. Deck Contents was null");
    }
    else
    {
      foreach (PegasusUtil.DeckContents deckContent in deckContents)
      {
        if (deckContent == null)
        {
          Log.CollectionManager.PrintError("UpdateFromDeckContents: deckContents contained a null deckContent.");
        }
        else
        {
          Network.DeckContents deckContents1 = Network.DeckContents.FromPacket(deckContent);
          if (this.m_pendingRequestDeckContents != null)
            this.m_pendingRequestDeckContents.Remove(deckContents1.Deck);
          CollectionDeck collectionDeck1 = (CollectionDeck) null;
          if (this.m_decks != null)
            this.m_decks.TryGetValue(deckContents1.Deck, out collectionDeck1);
          else
            Log.CollectionManager.PrintError("UpdateFromDeckContents: m_decks is null!");
          CollectionDeck collectionDeck2 = (CollectionDeck) null;
          if (this.m_baseDecks != null)
            this.m_baseDecks.TryGetValue(deckContents1.Deck, out collectionDeck2);
          else
            Log.CollectionManager.PrintError("UpdateFromDeckContents: m_baseDecks is null!");
          if (collectionDeck1 != null && collectionDeck2 != null)
          {
            bool flag = collectionDeck1 != null && this.IsInEditMode() && this.GetEditedDeck().ID == collectionDeck1.ID;
            if (!flag)
              collectionDeck1.ClearSlotContents();
            collectionDeck2.ClearSlotContents();
            foreach (Network.CardUserData card in deckContents1.Cards)
            {
              string cardId = GameUtils.TranslateDbIdToCardId(card.DbId);
              if (cardId != null)
              {
                for (int count = card.Count; count > 0; --count)
                {
                  if (!flag)
                    collectionDeck1.AddCard(cardId, card.Premium, true);
                  collectionDeck2.AddCard(cardId, card.Premium, true);
                }
              }
            }
            collectionDeck1.MarkNetworkContentsLoaded();
          }
          this.FireDeckContentsEvent(deckContents1.Deck);
        }
      }
      foreach (CollectionDeck collectionDeck in this.GetDecks().Values)
      {
        if (!collectionDeck.NetworkContentsLoaded())
          return;
      }
      this.LogAllDeckStringsInCollection();
      if (this.m_pendingRequestDeckContents != null)
      {
        float now = Time.realtimeSinceStartup;
        foreach (long key in this.m_pendingRequestDeckContents.Where<KeyValuePair<long, float>>((Func<KeyValuePair<long, float>, bool>) (kv => (double) now - (double) kv.Value > 10.0)).Select<KeyValuePair<long, float>, long>((Func<KeyValuePair<long, float>, long>) (kv => kv.Key)).ToArray<long>())
          this.m_pendingRequestDeckContents.Remove(key);
      }
      if (this.m_pendingRequestDeckContents != null && this.m_pendingRequestDeckContents.Count != 0)
        return;
      this.FireAllDeckContentsEvent();
    }
  }

  private void OnDBAction()
  {
    Network.DBAction response = Network.Get().GetDeckResponse();
    Log.CollectionManager.Print(string.Format("MetaData:{0} DBAction:{1} Result:{2}", (object) response.MetaData, (object) response.Action, (object) response.Result));
    bool flag1 = false;
    bool flag2 = false;
    switch (response.Action)
    {
      case Network.DBAction.ActionType.CREATE_DECK:
        if (response.Result != Network.DBAction.ResultType.SUCCESS && (UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null)
        {
          CollectionDeckTray.Get().GetDecksContent().CreateNewDeckCancelled();
          break;
        }
        break;
      case Network.DBAction.ActionType.RENAME_DECK:
        flag1 = true;
        if (this.m_pendingDeckRenameList != null && this.m_pendingDeckRenameList.Any<CollectionManager.PendingDeckRenameData>())
        {
          this.m_pendingDeckRenameList.RemoveAll((Predicate<CollectionManager.PendingDeckRenameData>) (d => d.m_deckId == response.MetaData));
          break;
        }
        break;
      case Network.DBAction.ActionType.SET_DECK:
        flag2 = true;
        if (this.m_decksToRequestContentsAfterDeckSetDataResonse.Contains(response.MetaData))
        {
          Network.Get().RequestDeckContents(response.MetaData);
          this.m_decksToRequestContentsAfterDeckSetDataResonse.Remove(response.MetaData);
        }
        if (this.m_timeOfLastPlayerDeckSave.HasValue)
        {
          DateTime now = DateTime.Now;
          DateTime? time = this.m_timeOfLastPlayerDeckSave;
          double totalSeconds = (time.HasValue ? new TimeSpan?(now - time.GetValueOrDefault()) : new TimeSpan?()).Value.TotalSeconds;
          TelemetryManager.Client().SendDeckUpdateResponseInfo((float) totalSeconds);
          time = new DateTime?();
          this.SetTimeOfLastPlayerDeckSave(time);
        }
        if (this.m_pendingDeckEditList != null && this.m_pendingDeckEditList.Any<CollectionManager.PendingDeckEditData>())
        {
          this.m_pendingDeckEditList.RemoveAll((Predicate<CollectionManager.PendingDeckEditData>) (d => d.m_deckId == response.MetaData));
          break;
        }
        break;
    }
    if (!(flag1 | flag2))
      return;
    long deckID = response.MetaData;
    CollectionDeck deck = this.GetDeck(deckID);
    CollectionDeck baseDeck = this.GetBaseDeck(deckID);
    if (deck == null)
      return;
    if (response.Result == Network.DBAction.ResultType.SUCCESS)
    {
      Log.CollectionManager.Print(string.Format("CollectionManager.OnDBAction(): overwriting baseDeck with {0} updated deck ({1}:{2})", deck.IsValidForRuleset ? (object) "valid" : (object) "INVALID", (object) deck.ID, (object) deck.Name));
      baseDeck.CopyFrom(deck);
      NetCache.DeckHeader deckHeader1 = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>().Decks.Find((Predicate<NetCache.DeckHeader>) (deckHeader => deckHeader.ID == deckID));
      if (deckHeader1 != null)
      {
        RuneType[] runeOrder = deck.GetRuneOrder();
        deckHeader1.HeroOverridden = deck.HeroOverridden;
        deckHeader1.SeasonId = deck.SeasonId;
        deckHeader1.BrawlLibraryItemId = deck.BrawlLibraryItemId;
        deckHeader1.NeedsName = deck.NeedsName;
        deckHeader1.FormatType = deck.FormatType;
        deckHeader1.LastModified = new DateTime?(DateTime.Now);
        deckHeader1.Rune1 = runeOrder[0];
        deckHeader1.Rune2 = runeOrder[1];
        deckHeader1.Rune3 = runeOrder[2];
      }
    }
    else
    {
      Log.CollectionManager.Print(string.Format("CollectionManager.OnDBAction(): overwriting deck that failed to update with base deck ({0}:{1})", (object) baseDeck.ID, (object) baseDeck.Name));
      deck.CopyFrom(baseDeck);
    }
    if (flag1)
      deck.OnNameChangeComplete();
    if (!flag2)
      return;
    deck.OnContentChangesComplete();
  }

  private void OnDeckCreatedNetworkResponse()
  {
    int? requestId;
    this.OnDeckCreated(Network.Get().GetCreatedDeck(out requestId), requestId);
    List<DeckInfo> listFromNetCache = NetCache.Get().GetDeckListFromNetCache();
    OfflineDataCache.CacheLocalAndOriginalDeckList(listFromNetCache, listFromNetCache);
  }

  private void OnDeckCreated(NetCache.DeckHeader deck, int? requestId)
  {
    Log.CollectionManager.Print(string.Format("DeckCreated:{0} ID:{1} Hero:{2}", (object) deck.Name, (object) deck.ID, (object) deck.Hero));
    this.m_pendingDeckCreate = (CollectionManager.PendingDeckCreateData) null;
    this.AddDeck(deck).MarkNetworkContentsLoaded();
    if (requestId.HasValue)
    {
      if (!this.m_inTransitDeckCreateRequests.Contains(requestId.Value))
        return;
      this.m_inTransitDeckCreateRequests.Remove(requestId.Value);
    }
    foreach (CollectionManager.DelOnDeckCreated delOnDeckCreated in this.m_deckCreatedListeners.ToArray())
      delOnDeckCreated(deck.ID, deck.Name);
  }

  private void OnDeckDeleted() => this.OnDeckDeleted(Network.Get().GetDeletedDeckID());

  private void OnDeckDeleted(long deckId)
  {
    Log.CollectionManager.Print("CollectionManager.OnDeckDeleted");
    Log.CollectionManager.Print(string.Format("DeckDeleted:{0}", (object) deckId));
    CollectionDeck removedDeck = this.RemoveDeck(deckId);
    if (this.m_pendingDeckDeleteList != null && this.m_pendingDeckDeleteList.Any<CollectionManager.PendingDeckDeleteData>())
      this.m_pendingDeckDeleteList.RemoveAll((Predicate<CollectionManager.PendingDeckDeleteData>) (d => d.m_deckId == deckId));
    if ((UnityEngine.Object) CollectionDeckTray.Get() == (UnityEngine.Object) null)
      return;
    CollectionDeck editedDeck = this.GetEditedDeck();
    if (this.IsInEditMode() && editedDeck != null && editedDeck.ID == deckId)
    {
      Navigation.Pop();
      if (SceneMgr.Get().GetMode() != SceneMgr.Mode.HUB)
      {
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_OFFLINE_FEATURE_DISABLED_HEADER"),
          m_text = GameStrings.Get("GLUE_OFFLINE_DECK_DELETED_REMOTELY_ERROR_BODY"),
          m_responseDisplay = AlertPopup.ResponseDisplay.OK,
          m_showAlertIcon = true
        };
        DialogManager.Get().ShowPopup(info);
      }
    }
    if (removedDeck == null)
      return;
    foreach (CollectionManager.DelOnDeckDeleted delOnDeckDeleted in this.m_deckDeletedListeners.ToArray())
      delOnDeckDeleted(removedDeck);
  }

  public void OnDeckDeletedWhileOffline(long deckId) => this.OnDeckDeleted(deckId);

  public void AddPendingDeckDelete(long deckId)
  {
    if (this.m_pendingDeckDeleteList == null)
      this.m_pendingDeckDeleteList = new List<CollectionManager.PendingDeckDeleteData>();
    this.m_pendingDeckDeleteList.Add(new CollectionManager.PendingDeckDeleteData()
    {
      m_deckId = deckId
    });
  }

  public void AddPendingDeckEdit(long deckId)
  {
    if (this.m_pendingDeckEditList == null)
      this.m_pendingDeckEditList = new List<CollectionManager.PendingDeckEditData>();
    this.m_pendingDeckEditList.Add(new CollectionManager.PendingDeckEditData()
    {
      m_deckId = deckId
    });
  }

  public void AddPendingDeckRename(long deckId, string name)
  {
    if (this.m_pendingDeckRenameList == null)
      this.m_pendingDeckRenameList = new List<CollectionManager.PendingDeckRenameData>();
    this.m_pendingDeckRenameList.Add(new CollectionManager.PendingDeckRenameData()
    {
      m_deckId = deckId,
      m_name = name
    });
  }

  private void OnDeckRenamed()
  {
    Network.DeckName renamedDeck = Network.Get().GetRenamedDeck();
    this.OnDeckRenamed(renamedDeck.Deck, renamedDeck.Name);
  }

  private void OnDeckRenamed(long deckId, string newName)
  {
    Log.CollectionManager.Print(string.Format("OnDeckRenamed {0}", (object) deckId));
    CollectionDeck baseDeck = this.GetBaseDeck(deckId);
    CollectionDeck deck = this.GetDeck(deckId);
    if (baseDeck == null || deck == null)
    {
      Debug.LogWarning((object) string.Format("For deck with ID {0}, unable to handle OnDeckRenamed event to new name {1} due to null deck or null baseDeck", (object) deckId, (object) newName));
    }
    else
    {
      baseDeck.Name = newName;
      deck.Name = newName;
      NetCache.DeckHeader deckHeader1 = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>().Decks.Find((Predicate<NetCache.DeckHeader>) (deckHeader => deckHeader.ID == deckId));
      if (deckHeader1 != null)
      {
        deckHeader1.Name = newName;
        deckHeader1.LastModified = new DateTime?(DateTime.Now);
      }
      OfflineDataCache.RenameDeck(deckId, newName);
      deck.OnNameChangeComplete();
    }
  }

  public static void Init()
  {
    if (CollectionManager.s_instance != null)
      return;
    CollectionManager.s_instance = new CollectionManager();
    HearthstoneApplication.Get().WillReset += new System.Action(CollectionManager.s_instance.WillReset);
    NetCache.Get().FavoriteBattlegroundsGuideSkinChanged += new NetCache.DelFavoriteBattlegroundsGuideSkinChangedListener(CollectionManager.s_instance.OnFavoriteBattlegroundsGuideSkinChanged);
    CollectionManager.s_instance.InitImpl();
  }

  public static CollectionManager Get() => CollectionManager.s_instance;

  public CollectibleDisplay GetCollectibleDisplay() => this.m_collectibleDisplay;

  public bool IsFullyLoaded() => this.m_collectionLoaded;

  public void RegisterCollectionNetHandlers()
  {
    Network network = Network.Get();
    network.RegisterNetHandler((object) BoughtSoldCard.PacketID.ID, new Network.NetHandler(this.OnCardSale));
    network.RegisterNetHandler((object) PegasusUtil.MassDisenchantResponse.PacketID.ID, new Network.NetHandler(this.OnMassDisenchantResponse));
    network.RegisterNetHandler((object) PVPDRSessionInfoResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRSessionInfoResponse));
  }

  public void RemoveCollectionNetHandlers()
  {
    Network network = Network.Get();
    network.RemoveNetHandler((object) BoughtSoldCard.PacketID.ID, new Network.NetHandler(this.OnCardSale));
    network.RemoveNetHandler((object) PegasusUtil.MassDisenchantResponse.PacketID.ID, new Network.NetHandler(this.OnMassDisenchantResponse));
    network.RemoveNetHandler((object) PVPDRSessionInfoResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRSessionInfoResponse));
  }

  public bool HasVisitedCollection() => this.m_hasVisitedCollection;

  public void SetHasVisitedCollection(bool enable) => this.m_hasVisitedCollection = enable;

  public bool IsWaitingForBoxTransition() => this.m_waitingForBoxTransition;

  public void NotifyOfBoxTransitionStart()
  {
    Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    this.m_waitingForBoxTransition = true;
  }

  public void OnBoxTransitionFinished(object userData)
  {
    Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxTransitionFinished));
    this.m_waitingForBoxTransition = false;
  }

  public void SetCollectibleDisplay(CollectibleDisplay display) => this.m_collectibleDisplay = display;

  public void AddCardReward(CardRewardData cardReward, bool markAsNew) => this.AddCardRewards(new List<CardRewardData>()
  {
    cardReward
  }, markAsNew);

  public void AddCardRewards(List<CardRewardData> cardRewards, bool markAsNew)
  {
    List<string> cardIDs = new List<string>();
    List<TAG_PREMIUM> tagPremiumList = new List<TAG_PREMIUM>();
    List<DateTime> insertDates = new List<DateTime>();
    List<int> counts = new List<int>();
    DateTime now = DateTime.Now;
    foreach (CardRewardData cardReward in cardRewards)
    {
      cardIDs.Add(cardReward.CardID);
      tagPremiumList.Add(cardReward.Premium);
      insertDates.Add(now);
      counts.Add(cardReward.Count);
    }
    this.InsertNewCollectionCards(cardIDs, tagPremiumList, insertDates, counts, !markAsNew);
    AchieveManager.Get().ValidateAchievesNow();
    foreach (CollectionManager.DelOnCardRewardsInserted cardRewardsInserted in this.m_cardRewardListeners.ToArray())
      cardRewardsInserted(cardIDs, tagPremiumList);
  }

  public float CollectionLastModifiedTime() => this.m_collectionLastModifiedTime;

  public static int EntityDefSortComparison(EntityDef entityDef1, EntityDef entityDef2)
  {
    int num1 = (entityDef1.HasTag(GAME_TAG.DECK_LIST_SORT_ORDER) ? entityDef1.GetTag(GAME_TAG.DECK_LIST_SORT_ORDER) : int.MaxValue) - (entityDef2.HasTag(GAME_TAG.DECK_LIST_SORT_ORDER) ? entityDef2.GetTag(GAME_TAG.DECK_LIST_SORT_ORDER) : int.MaxValue);
    if (num1 != 0)
      return num1;
    int num2 = entityDef1.GetCost() - entityDef2.GetCost();
    if (num2 != 0)
      return num2;
    int num3 = string.Compare(entityDef1.GetName(), entityDef2.GetName(), true);
    return num3 != 0 ? num3 : CollectionManager.GetCardTypeSortOrder(entityDef1) - CollectionManager.GetCardTypeSortOrder(entityDef2);
  }

  public static int GetCardTypeSortOrder(EntityDef entityDef)
  {
    switch (entityDef.GetCardType())
    {
      case TAG_CARDTYPE.MINION:
        return 3;
      case TAG_CARDTYPE.SPELL:
        return 2;
      case TAG_CARDTYPE.WEAPON:
        return 1;
      default:
        return 0;
    }
  }

  private bool IsSetRotatedWithCache(TAG_CARD_SET set, Map<TAG_CARD_SET, bool> cache)
  {
    bool flag;
    if (!cache.TryGetValue(set, out flag))
    {
      flag = GameUtils.IsSetRotated(set);
      cache[set] = flag;
    }
    return flag;
  }

  private void BuildCoreCounterpartMap()
  {
    this.m_coreCounterpartCardMap.Clear();
    foreach (CollectibleCard collectibleCard in this.m_collectibleCards)
    {
      if (collectibleCard.Set == TAG_CARD_SET.CORE && !this.m_coreCounterpartCardMap.ContainsKey(collectibleCard.CardDbId))
      {
        int tag = collectibleCard.GetEntityDef().GetTag(GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID);
        if (tag != 0 && this.GetCard(GameUtils.TranslateDbIdToCardId(tag), collectibleCard.PremiumType) != null)
          this.m_coreCounterpartCardMap.Add(collectibleCard.CardDbId, tag);
      }
    }
  }

  public CollectionManager.FindCardsResult FindCards(
    string searchString = null,
    List<CollectibleCardFilter.FilterMask> filterMasks = null,
    int? manaCost = null,
    TAG_CARD_SET[] theseCardSets = null,
    TAG_CLASS[] theseClassTypes = null,
    TAG_CARDTYPE[] theseCardTypes = null,
    TAG_ROLE[] theseRoleTypes = null,
    TAG_RARITY? rarity = null,
    TAG_RACE? race = null,
    bool? isHero = null,
    int? minOwned = null,
    bool? notSeen = null,
    bool? isCraftable = null,
    CollectionManager.CollectibleCardFilterFunc[] priorityFilters = null,
    DeckRuleset deckRuleset = null,
    bool returnAfterFirstResult = false,
    HashSet<string> leagueBannedCardsSubset = null,
    List<int> specificCards = null,
    bool? filterCoreCounterpartCards = null)
  {
    CollectionManager.FindCardsResult results = new CollectionManager.FindCardsResult();
    CollectibleCardFilter.FilterMask searchFilterMask = CollectibleCardFilter.FilterMask.PREMIUM_ALL;
    this.m_filterCardSet.Clear();
    this.m_filterCardClass.Clear();
    this.m_filterCardType.Clear();
    this.m_filterCardRole.Clear();
    this.m_filterIsSetRotatedCache.Clear();
    this.m_cachedCardSetValues.Clear();
    List<CollectionManager.CollectibleCardFilterFunc> filterFuncs = new List<CollectionManager.CollectibleCardFilterFunc>();
    if (priorityFilters != null)
      filterFuncs.AddRange((IEnumerable<CollectionManager.CollectibleCardFilterFunc>) priorityFilters);
    CollectionManager.CollectibleCardFilterFunc collectibleCardFilterFunc1 = (CollectionManager.CollectibleCardFilterFunc) (card =>
    {
      if (card.IsHeroSkin)
        return card.OwnedCount < 1;
      return card.IsCraftable && this.GetOwnedCardCountByFilterMask(card.CardId, searchFilterMask) < card.DefaultMaxCopiesPerDeck;
    });
    CollectionManager.CollectibleCardFilterFunc collectibleCardFilterFunc2 = (CollectionManager.CollectibleCardFilterFunc) (card => !card.IsHeroSkin && card.IsCraftable && this.GetOwnedCardCountByFilterMask(card.CardId, searchFilterMask) > card.DefaultMaxCopiesPerDeck);
    CollectionManager.CollectibleCardFilterFunc collectibleCardFilterFunc3 = (CollectionManager.CollectibleCardFilterFunc) (card => card.IsHeroSkin && this.IsFavoriteHero(card.CardId));
    if (filterMasks != null)
      filterFuncs.Add(new CollectionManager.CollectibleCardFilterFunc(maskFilter));
    bool flag1 = !string.IsNullOrEmpty(searchString);
    if (flag1)
    {
      string[] source = searchString.ToLower().Split(' ');
      string str1 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MISSING");
      string str2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EXTRA");
      string str3 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_FAVORITE");
      if (((IEnumerable<string>) source).Contains<string>(str3) && CardBackManager.Get().MultipleFavoriteCardBacksEnabled())
        filterFuncs.Add(collectibleCardFilterFunc3);
      else if (((IEnumerable<string>) source).Contains<string>(str1))
      {
        searchFilterMask = CollectibleCardFilter.FilterMask.PREMIUM_ALL | CollectibleCardFilter.FilterMask.UNOWNED;
        filterFuncs.Add(collectibleCardFilterFunc1);
      }
      else if (((IEnumerable<string>) source).Contains<string>(str2))
      {
        searchFilterMask = CollectibleCardFilter.FilterMask.PREMIUM_ALL | CollectibleCardFilter.FilterMask.OWNED;
        filterFuncs.Add(collectibleCardFilterFunc2);
      }
      CollectibleCardFilter collectibleCardFilter = (CollectibleCardFilter) new CollectibleCardClassFilter();
      filterFuncs.AddRange((IEnumerable<CollectionManager.CollectibleCardFilterFunc>) collectibleCardFilter.FiltersFromSearchString(searchString));
    }
    if (theseClassTypes != null && theseClassTypes.Length != 0)
      filterFuncs.Add(new CollectionManager.CollectibleCardFilterFunc(classTypeFilter));
    if (theseCardTypes != null && theseCardTypes.Length != 0)
    {
      foreach (TAG_CARDTYPE theseCardType in theseCardTypes)
        this.m_filterCardType.Add(theseCardType);
      filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card => this.m_filterCardType.Contains(card.CardType)));
    }
    if (theseRoleTypes != null && theseRoleTypes.Length != 0)
      filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card => ((IEnumerable<TAG_ROLE>) theseRoleTypes).Contains<TAG_ROLE>(card.Role)));
    if (rarity.HasValue)
      filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card => card.Rarity == rarity.Value));
    if (race.HasValue)
      filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card => card.Races.Contains(race.Value)));
    if (isHero.HasValue)
    {
      bool isHeroValue = isHero.Value;
      filterFuncs.Add(new CollectionManager.CollectibleCardFilterFunc(heroFilter));

      bool heroFilter(CollectibleCard card) => card.IsHeroSkin == isHeroValue;
    }
    if (notSeen.HasValue)
    {
      if (notSeen.Value)
        filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card => card.SeenCount < card.OwnedCount));
      else
        filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card => card.SeenCount == card.OwnedCount));
    }
    if (isCraftable.HasValue)
      filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card => card.IsCraftable == isCraftable.Value));
    if (flag1)
    {
      this.m_startsWithMatchNames.Clear();
      string lowerSearchString = searchString.ToLower();
      filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card =>
      {
        if (card.Set == TAG_CARD_SET.LETTUCE)
          return false;
        string lowerCardName = card.Name.ToLower();
        if (((IEnumerable<string>) lowerCardName.Split(' ')).Any<string>((Func<string, bool>) (s => s.StartsWith(lowerSearchString) || SearchableString.SearchInternationalText(lowerCardName, lowerSearchString))))
        {
          if (!this.m_startsWithMatchNames.ContainsKey(card.CardDbId))
            this.m_startsWithMatchNames[card.CardDbId] = 0;
          this.m_startsWithMatchNames[card.CardDbId] += card.OwnedCount;
        }
        return true;
      }));
    }
    if (manaCost.HasValue)
    {
      int minManaCost = manaCost.Value;
      int maxManaCost = manaCost.Value;
      if (maxManaCost >= 7)
        maxManaCost = int.MaxValue;
      filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card =>
      {
        int num = card.ManaCost < minManaCost ? 0 : (card.ManaCost <= maxManaCost ? 1 : 0);
        if (num != 0)
          return num != 0;
        if (!this.m_startsWithMatchNames.ContainsKey(card.CardDbId))
          return num != 0;
        results.m_resultsWithoutManaFilterExist = true;
        return num != 0;
      }));
    }
    if (theseCardSets != null && theseCardSets.Length != 0)
    {
      foreach (TAG_CARD_SET theseCardSet in theseCardSets)
        this.m_filterCardSet.Add(theseCardSet);
      filterFuncs.Add(new CollectionManager.CollectibleCardFilterFunc(standardSetFilter));
    }
    if (minOwned.HasValue)
    {
      int minOwnedValue = minOwned.Value;
      filterFuncs.Add(new CollectionManager.CollectibleCardFilterFunc(minOwnedFilter));

      bool minOwnedFilter(CollectibleCard card)
      {
        int ownedCount = card.OwnedCount;
        int num = ownedCount >= minOwnedValue ? 1 : 0;
        if (num != 0)
          return num != 0;
        int cardDbId = card.CardDbId;
        if (!this.m_startsWithMatchNames.ContainsKey(cardDbId))
          return num != 0;
        this.m_startsWithMatchNames[cardDbId] -= ownedCount;
        if (this.m_startsWithMatchNames[cardDbId] >= 1)
          return num != 0;
        results.m_resultsUnownedExist = true;
        return num != 0;
      }
    }
    if (theseCardSets != null && theseCardSets.Length != 0)
      filterFuncs.Add(new CollectionManager.CollectibleCardFilterFunc(wildSetFilter));
    if (deckRuleset != null)
    {
      CollectionDeck deck = CollectionManager.Get().GetEditedDeck();
      filterFuncs.Add(new CollectionManager.CollectibleCardFilterFunc(deckRulesetFilter));

      bool deckRulesetFilter(CollectibleCard card)
      {
        bool flag = deckRuleset.Filter(card.GetEntityDef(), deck);
        if (!flag && card.OwnedCount > 0 && deckRuleset.FilterFailsOnShowInvalidRule(card.GetEntityDef(), deck))
          flag = true;
        return flag;
      }
    }
    if (leagueBannedCardsSubset != null)
      filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card => !leagueBannedCardsSubset.Contains(card.GetEntityDef().GetCardId())));
    if (specificCards != null)
      filterFuncs.Add((CollectionManager.CollectibleCardFilterFunc) (card => specificCards.Contains(card.CardDbId)));
    Predicate<CollectibleCard> match = (Predicate<CollectibleCard>) (card =>
    {
      int index = 0;
      for (int count = filterFuncs.Count; index < count; ++index)
      {
        if (!filterFuncs[index](card))
          return false;
      }
      return true;
    });
    if (returnAfterFirstResult)
    {
      CollectibleCard collectibleCard = this.m_collectibleCards.Find(match);
      if (collectibleCard != null)
        results.m_cards.Add(collectibleCard);
    }
    else
      results.m_cards = this.m_collectibleCards.FindAll(match);
    if (filterCoreCounterpartCards.HasValue)
    {
      bool? nullable = filterCoreCounterpartCards;
      bool flag2 = true;
      if (nullable.GetValueOrDefault() == flag2 & nullable.HasValue)
        this.FilterOutCardWithCoreCounterparts(results.m_cards);
    }
    return results;

    bool maskFilter(CollectibleCard card)
    {
      CollectibleCardFilter.FilterMask filterMask1 = CollectibleCardFilter.FilterMaskFromPremiumType(card.PremiumType);
      CollectibleCardFilter.FilterMask filterMask2 = card.OwnedCount <= 0 ? filterMask1 | CollectibleCardFilter.FilterMask.UNOWNED : filterMask1 | CollectibleCardFilter.FilterMask.OWNED;
      int index = 0;
      for (int count = filterMasks.Count; index < count; ++index)
      {
        if ((filterMasks[index] & filterMask2) == filterMask2)
          return true;
      }
      return false;
    }

    bool classTypeFilter(CollectibleCard card)
    {
      card.GetEntityDef().GetClasses((IList<TAG_CLASS>) this.m_cardClasses);
      int index1 = 0;
      for (int length = theseClassTypes.Length; index1 < length; ++index1)
      {
        TAG_CLASS theseClassType = theseClassTypes[index1];
        for (int index2 = 0; index2 < this.m_cardClasses.Count; ++index2)
        {
          if (theseClassType == this.m_cardClasses[index2])
            return true;
        }
      }
      return false;
    }

    bool standardSetFilter(CollectibleCard card)
    {
      string cardId = card.CardId;
      TAG_CARD_SET set = TAG_CARD_SET.INVALID;
      if (!this.m_cachedCardSetValues.TryGetValue(cardId, out set))
      {
        set = card.Set;
        this.m_cachedCardSetValues.Add(cardId, set);
      }
      if (this.IsSetRotatedWithCache(set, this.m_filterIsSetRotatedCache))
        return true;
      int num = this.m_filterCardSet.Contains(set) ? 1 : 0;
      if (num != 0)
        return num != 0;
      if (!this.m_startsWithMatchNames.ContainsKey(card.CardDbId))
        return num != 0;
      results.m_resultsWithoutSetFilterExist = true;
      return num != 0;
    }

    bool wildSetFilter(CollectibleCard card)
    {
      TAG_CARD_SET cachedCardSetValue = this.m_cachedCardSetValues[card.CardId];
      if (!this.IsSetRotatedWithCache(cachedCardSetValue, this.m_filterIsSetRotatedCache))
        return true;
      int num = this.m_filterCardSet.Contains(cachedCardSetValue) ? 1 : 0;
      if (num != 0)
        return num != 0;
      if (!this.m_startsWithMatchNames.ContainsKey(card.CardDbId))
        return num != 0;
      results.m_resultsInWildExist = true;
      return num != 0;
    }
  }

  public CollectionManager.FindCardsResult FindOrderedCards(
    string searchString = null,
    List<CollectibleCardFilter.FilterMask> filterMasks = null,
    int? manaCost = null,
    TAG_CARD_SET[] theseCardSets = null,
    TAG_CLASS[] theseClassTypes = null,
    TAG_CARDTYPE[] theseCardTypes = null,
    TAG_ROLE[] theseRoleTypes = null,
    TAG_RARITY? rarity = null,
    TAG_RACE? race = null,
    bool? isHero = null,
    int? minOwned = null,
    bool? notSeen = null,
    bool? isCraftable = null,
    CollectionManager.CollectibleCardFilterFunc[] priorityFilters = null,
    DeckRuleset deckRuleset = null,
    bool returnAfterFirstResult = false,
    HashSet<string> leagueBannedCardsSubset = null,
    List<int> specificCards = null,
    bool? filterCounterpartCards = null)
  {
    CollectionManager.FindCardsResult cards = this.FindCards(searchString, filterMasks, manaCost, theseCardSets, theseClassTypes, theseCardTypes, theseRoleTypes, rarity, race, isHero, minOwned, notSeen, isCraftable, priorityFilters, deckRuleset, returnAfterFirstResult, leagueBannedCardsSubset, specificCards, filterCounterpartCards);
    cards.m_cards.Sort(CollectionManager.OrderedCardsSort);
    return cards;
  }

  public bool HasCoreCounterpart(int originalCardId) => this.m_coreCounterpartCardMap.ContainsValue(originalCardId);

  public void FilterOutCardWithCoreCounterparts(List<CollectibleCard> collectibleCards)
  {
    HashSet<CollectionManager.CollectibleCardIndex> collectibleCardIndexSet = new HashSet<CollectionManager.CollectibleCardIndex>((IEqualityComparer<CollectionManager.CollectibleCardIndex>) new CollectionManager.CollectibleCardIndexComparer());
    foreach (CollectibleCard collectibleCard in collectibleCards)
    {
      if (collectibleCard.Set == TAG_CARD_SET.CORE)
      {
        int dbId = 0;
        if (this.m_coreCounterpartCardMap.TryGetValue(collectibleCard.CardDbId, out dbId))
        {
          CollectibleCard card = this.GetCard(GameUtils.TranslateDbIdToCardId(dbId), collectibleCard.PremiumType);
          if (card != null)
          {
            string cardId;
            if (collectibleCard.OwnedCount == card.DefaultMaxCopiesPerDeck)
              cardId = card.CardId;
            else if (collectibleCard.OwnedCount != 1 || card.OwnedCount != 1)
              cardId = collectibleCard.OwnedCount >= card.OwnedCount ? card.CardId : collectibleCard.CardId;
            else
              continue;
            if (cardId != null)
              collectibleCardIndexSet.Add(new CollectionManager.CollectibleCardIndex(cardId, collectibleCard.PremiumType));
          }
        }
      }
    }
    for (int index = collectibleCards.Count - 1; index > -1; --index)
    {
      CollectibleCard collectibleCard = collectibleCards[index];
      if (collectibleCardIndexSet.Contains(new CollectionManager.CollectibleCardIndex(collectibleCard.CardId, collectibleCard.PremiumType)))
        collectibleCards.RemoveAt(index);
    }
  }

  public List<CollectibleCard> GetAllCards() => this.m_collectibleCards;

  public bool IsCardOwned(string cardId) => this.GetTotalOwnedCount(cardId) > 0;

  public void RegisterCollectionLoadedListener(CollectionManager.DelOnCollectionLoaded listener)
  {
    if (this.m_collectionLoadedListeners.Contains(listener))
      return;
    this.m_collectionLoadedListeners.Add(listener);
  }

  public bool RemoveCollectionLoadedListener(CollectionManager.DelOnCollectionLoaded listener) => this.m_collectionLoadedListeners.Remove(listener);

  public void RegisterCollectionChangedListener(CollectionManager.DelOnCollectionChanged listener)
  {
    if (this.m_collectionChangedListeners.Contains(listener))
      return;
    this.m_collectionChangedListeners.Add(listener);
  }

  public bool RemoveCollectionChangedListener(CollectionManager.DelOnCollectionChanged listener) => this.m_collectionChangedListeners.Remove(listener);

  public void RegisterDeckCreatedListener(CollectionManager.DelOnDeckCreated listener)
  {
    if (this.m_deckCreatedListeners.Contains(listener))
      return;
    this.m_deckCreatedListeners.Add(listener);
  }

  public bool RemoveDeckCreatedListener(CollectionManager.DelOnDeckCreated listener) => this.m_deckCreatedListeners.Remove(listener);

  public void RegisterDeckDeletedListener(CollectionManager.DelOnDeckDeleted listener)
  {
    if (this.m_deckDeletedListeners.Contains(listener))
      return;
    this.m_deckDeletedListeners.Add(listener);
  }

  public bool RemoveDeckDeletedListener(CollectionManager.DelOnDeckDeleted listener) => this.m_deckDeletedListeners.Remove(listener);

  public void RegisterDeckContentsListener(CollectionManager.DelOnDeckContents listener)
  {
    if (this.m_deckContentsListeners.Contains(listener))
      return;
    this.m_deckContentsListeners.Add(listener);
  }

  public bool RemoveDeckContentsListener(CollectionManager.DelOnDeckContents listener) => this.m_deckContentsListeners.Remove(listener);

  public void RegisterNewCardSeenListener(CollectionManager.DelOnNewCardSeen listener)
  {
    if (this.m_newCardSeenListeners.Contains(listener))
      return;
    this.m_newCardSeenListeners.Add(listener);
  }

  public bool RemoveNewCardSeenListener(CollectionManager.DelOnNewCardSeen listener) => this.m_newCardSeenListeners.Remove(listener);

  public void RegisterCardRewardsInsertedListener(
    CollectionManager.DelOnCardRewardsInserted listener)
  {
    if (this.m_cardRewardListeners.Contains(listener))
      return;
    this.m_cardRewardListeners.Add(listener);
  }

  public bool RemoveCardRewardsInsertedListener(
    CollectionManager.DelOnCardRewardsInserted listener)
  {
    return this.m_cardRewardListeners.Remove(listener);
  }

  public void RegisterMassDisenchantListener(CollectionManager.OnMassDisenchant listener)
  {
    if (this.m_massDisenchantListeners.Contains(listener))
      return;
    this.m_massDisenchantListeners.Add(listener);
  }

  public void RemoveMassDisenchantListener(CollectionManager.OnMassDisenchant listener) => this.m_massDisenchantListeners.Remove(listener);

  public void RegisterEditedDeckChanged(CollectionManager.OnEditedDeckChanged listener) => this.m_editedDeckChangedListeners.Add(listener);

  public void RemoveEditedDeckChanged(CollectionManager.OnEditedDeckChanged listener) => this.m_editedDeckChangedListeners.Remove(listener);

  public bool RegisterFavoriteHeroChangedListener(
    CollectionManager.FavoriteHeroChangedCallback callback)
  {
    return this.RegisterFavoriteHeroChangedListener(callback, (object) null);
  }

  public bool RegisterFavoriteHeroChangedListener(
    CollectionManager.FavoriteHeroChangedCallback callback,
    object userData)
  {
    CollectionManager.FavoriteHeroChangedListener heroChangedListener = new CollectionManager.FavoriteHeroChangedListener();
    heroChangedListener.SetCallback(callback);
    heroChangedListener.SetUserData(userData);
    if (this.m_favoriteHeroChangedListeners.Contains(heroChangedListener))
      return false;
    this.m_favoriteHeroChangedListeners.Add(heroChangedListener);
    return true;
  }

  public bool RemoveFavoriteHeroChangedListener(
    CollectionManager.FavoriteHeroChangedCallback callback)
  {
    return this.RemoveFavoriteHeroChangedListener(callback, (object) null);
  }

  public bool RemoveFavoriteHeroChangedListener(
    CollectionManager.FavoriteHeroChangedCallback callback,
    object userData)
  {
    CollectionManager.FavoriteHeroChangedListener heroChangedListener = new CollectionManager.FavoriteHeroChangedListener();
    heroChangedListener.SetCallback(callback);
    heroChangedListener.SetUserData(userData);
    return this.m_favoriteHeroChangedListeners.Remove(heroChangedListener);
  }

  public bool RegisterOnUIHeroOverrideCardRemovedListener(
    CollectionManager.OnUIHeroOverrideCardRemovedCallback callback)
  {
    return this.RegisterOnUIHeroOverrideCardRemovedListener(callback, (object) null);
  }

  public bool RegisterOnUIHeroOverrideCardRemovedListener(
    CollectionManager.OnUIHeroOverrideCardRemovedCallback callback,
    object userData)
  {
    CollectionManager.OnUIHeroOverrideCardRemovedListener cardRemovedListener = new CollectionManager.OnUIHeroOverrideCardRemovedListener();
    cardRemovedListener.SetCallback(callback);
    cardRemovedListener.SetUserData(userData);
    if (this.m_onUIHeroOverrideCardRemovedListeners.Contains(cardRemovedListener))
      return false;
    this.m_onUIHeroOverrideCardRemovedListeners.Add(cardRemovedListener);
    return true;
  }

  public bool RemoveOnUIHeroOverrideCardRemovedListener(
    CollectionManager.OnUIHeroOverrideCardRemovedCallback callback)
  {
    return this.RemoveOnUIHeroOverrideCardRemovedListener(callback, (object) null);
  }

  public bool RemoveOnUIHeroOverrideCardRemovedListener(
    CollectionManager.OnUIHeroOverrideCardRemovedCallback callback,
    object userData)
  {
    CollectionManager.OnUIHeroOverrideCardRemovedListener cardRemovedListener = new CollectionManager.OnUIHeroOverrideCardRemovedListener();
    cardRemovedListener.SetCallback(callback);
    cardRemovedListener.SetUserData(userData);
    return this.m_onUIHeroOverrideCardRemovedListeners.Remove(cardRemovedListener);
  }

  public void RegisterOnInitialCollectionReceivedListener(System.Action callback)
  {
    if (this.m_initialCollectionReceivedListeners.Contains(callback))
      return;
    this.m_initialCollectionReceivedListeners.Add(callback);
  }

  public void RemoveOnInitialCollectionReceivedListener(System.Action callback)
  {
    if (!this.m_initialCollectionReceivedListeners.Contains(callback))
      return;
    this.m_initialCollectionReceivedListeners.Remove(callback);
  }

  public bool OwnsAnyCollectible() => CardBackManager.Get().GetNumCardBacksOwned() > 0 || CoinManager.Get().GetCoinsOwned().Count > 0 || CollectionManager.Get().GetOwnedCards().Count > 0;

  public TAG_PREMIUM GetBestCardPremium(string cardID)
  {
    CollectibleCard collectibleCard = (CollectibleCard) null;
    if (this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardID, TAG_PREMIUM.DIAMOND), out collectibleCard) && collectibleCard.OwnedCount > 0)
      return TAG_PREMIUM.DIAMOND;
    if (this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardID, TAG_PREMIUM.SIGNATURE), out collectibleCard) && collectibleCard.OwnedCount > 0)
      return TAG_PREMIUM.SIGNATURE;
    return this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardID, TAG_PREMIUM.GOLDEN), out collectibleCard) && collectibleCard.OwnedCount > 0 ? TAG_PREMIUM.GOLDEN : TAG_PREMIUM.NORMAL;
  }

  public CollectibleCard GetCard(string cardID, TAG_PREMIUM premium)
  {
    CollectibleCard card = (CollectibleCard) null;
    this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardID, premium), out card);
    return card;
  }

  public List<CollectibleCard> GetOwnedHeroesForClass(TAG_CLASS heroClass)
  {
    int? manaCost = new int?();
    int? nullable1 = new int?(1);
    bool? nullable2 = new bool?(true);
    TAG_CLASS[] theseClassTypes = new TAG_CLASS[1]
    {
      heroClass
    };
    TAG_RARITY? rarity = new TAG_RARITY?();
    TAG_RACE? race = new TAG_RACE?();
    bool? isHero = nullable2;
    int? minOwned = nullable1;
    bool? notSeen = new bool?();
    bool? isCraftable = new bool?();
    bool? filterCoreCounterpartCards = new bool?();
    return this.FindCards(manaCost: manaCost, theseClassTypes: theseClassTypes, rarity: rarity, race: race, isHero: isHero, minOwned: minOwned, notSeen: notSeen, isCraftable: isCraftable, filterCoreCounterpartCards: filterCoreCounterpartCards).m_cards;
  }

  public int GetCountOfOwnedHeroesForClass(TAG_CLASS heroClass) => this.GetOwnedHeroesForClass(heroClass).Count;

  public int GetRandomHeroIdOwnedByPlayer(
    TAG_CLASS heroClass,
    bool shouldLimitToFavorites,
    int? heroIdToExclude = null)
  {
    if (shouldLimitToFavorites)
    {
      string cardId = CollectionManager.GetVanillaHero(heroClass);
      NetCache.CardDefinition randomFavoriteHero = this.GetRandomFavoriteHero(heroClass, heroIdToExclude);
      if (randomFavoriteHero != null)
        cardId = randomFavoriteHero.Name;
      return GameUtils.TranslateCardIdToDbId(cardId);
    }
    List<CollectibleCard> ownedHeroesForClass = this.GetOwnedHeroesForClass(heroClass);
    if (this.GetHeroPremium(heroClass) == TAG_PREMIUM.GOLDEN && ownedHeroesForClass.Count > 1)
    {
      string vanillaHeroCardId = CollectionManager.GetVanillaHero(heroClass);
      int index = ownedHeroesForClass.FindIndex((Predicate<CollectibleCard>) (hero => hero.PremiumType == TAG_PREMIUM.NORMAL && hero.CardId == vanillaHeroCardId));
      if (index > -1 && ownedHeroesForClass.Exists((Predicate<CollectibleCard>) (hero => TAG_PREMIUM.GOLDEN == hero.PremiumType && hero.CardId == vanillaHeroCardId)))
        ownedHeroesForClass.RemoveAt(index);
    }
    if (heroIdToExclude.HasValue && ownedHeroesForClass.Count > 1)
      ownedHeroesForClass.RemoveAll((Predicate<CollectibleCard>) (hero =>
      {
        int cardDbId = hero.CardDbId;
        int? nullable = heroIdToExclude;
        int valueOrDefault = nullable.GetValueOrDefault();
        return cardDbId == valueOrDefault & nullable.HasValue;
      }));
    if (ownedHeroesForClass.Count == 0)
      return 0;
    int index1 = UnityEngine.Random.Range(0, ownedHeroesForClass.Count);
    return ownedHeroesForClass[index1].CardDbId;
  }

  public List<(TAG_CLASS, NetCache.CardDefinition)> GetFavoriteHeroes()
  {
    NetCache.NetCacheFavoriteHeroes netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFavoriteHeroes>();
    return netObject == null ? this.GetFavoriteHeroesFromOfflineData() : netObject.FavoriteHeroes;
  }

  public List<NetCache.CardDefinition> GetFavoriteHeroesForClass(TAG_CLASS heroClass)
  {
    List<(TAG_CLASS, NetCache.CardDefinition)> favoriteHeroes = this.GetFavoriteHeroes();
    List<NetCache.CardDefinition> favoriteHeroesForClass = new List<NetCache.CardDefinition>();
    foreach ((TAG_CLASS, NetCache.CardDefinition) tuple in favoriteHeroes)
    {
      if (tuple.Item1 == heroClass)
        favoriteHeroesForClass.Add(tuple.Item2);
    }
    return favoriteHeroesForClass;
  }

  public NetCache.CardDefinition GetRandomFavoriteHero(
    TAG_CLASS heroClass,
    int? heroIdToExclude = null)
  {
    List<NetCache.CardDefinition> favoriteHeroesForClass = this.GetFavoriteHeroesForClass(heroClass);
    if (favoriteHeroesForClass.Count<NetCache.CardDefinition>() == 0)
      return (NetCache.CardDefinition) null;
    if (heroIdToExclude.HasValue && favoriteHeroesForClass.Count > 1)
    {
      string heroCardIdToExclude = GameUtils.TranslateDbIdToCardId(heroIdToExclude.Value);
      favoriteHeroesForClass.RemoveAll((Predicate<NetCache.CardDefinition>) (hero => hero.Name == heroCardIdToExclude));
    }
    int index = UnityEngine.Random.Range(0, favoriteHeroesForClass.Count);
    return favoriteHeroesForClass[index];
  }

  public bool IsFavoriteHero(string heroId) => NetCache.Get().GetNetObject<NetCache.NetCacheFavoriteHeroes>().FavoriteHeroes.Any<(TAG_CLASS, NetCache.CardDefinition)>((Func<(TAG_CLASS, NetCache.CardDefinition), bool>) (obj => obj.Item2.Name == heroId));

  public NetCache.CardDefinition GetFavoriteHero(string heroId) => NetCache.Get().GetNetObject<NetCache.NetCacheFavoriteHeroes>().FavoriteHeroes.Find((Predicate<(TAG_CLASS, NetCache.CardDefinition)>) (obj => obj.Item2.Name == heroId)).Item2;

  private List<(TAG_CLASS, NetCache.CardDefinition)> GetFavoriteHeroesFromOfflineData()
  {
    List<FavoriteHero> favoriteHeroesFromCache = OfflineDataCache.GetFavoriteHeroesFromCache();
    List<(TAG_CLASS, NetCache.CardDefinition)> heroesFromOfflineData = new List<(TAG_CLASS, NetCache.CardDefinition)>();
    foreach (FavoriteHero favoriteHero in favoriteHeroesFromCache)
      heroesFromOfflineData.Add(((TAG_CLASS) favoriteHero.ClassId, new NetCache.CardDefinition()
      {
        Name = GameUtils.TranslateDbIdToCardId(favoriteHero.Hero.Asset),
        Premium = (TAG_PREMIUM) favoriteHero.Hero.Premium
      }));
    return heroesFromOfflineData;
  }

  public int GetCoreCardsIOwn(TAG_CLASS cardClass)
  {
    NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
    return netObject == null ? 0 : netObject.CoreCardsUnlockedPerClass[cardClass].Count;
  }

  public List<CollectibleCard> GetOwnedCards() => this.FindCards(minOwned: new int?(1)).m_cards;

  public void GetOwnedCardCount(
    string cardId,
    out int normal,
    out int golden,
    out int signature,
    out int diamond)
  {
    normal = 0;
    golden = 0;
    signature = 0;
    diamond = 0;
    CollectibleCard collectibleCard;
    if (this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardId, TAG_PREMIUM.NORMAL), out collectibleCard))
      normal += collectibleCard.OwnedCount;
    if (this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardId, TAG_PREMIUM.GOLDEN), out collectibleCard))
      golden += collectibleCard.OwnedCount;
    if (this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardId, TAG_PREMIUM.SIGNATURE), out collectibleCard))
      signature += collectibleCard.OwnedCount;
    if (!this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardId, TAG_PREMIUM.DIAMOND), out collectibleCard))
      return;
    diamond += collectibleCard.OwnedCount;
  }

  public int GetOwnedCardCountByFilterMask(
    string cardId,
    CollectibleCardFilter.FilterMask filterMask)
  {
    int countByFilterMask = 0;
    CollectibleCard collectibleCard = (CollectibleCard) null;
    if ((filterMask & CollectibleCardFilter.FilterMask.PREMIUM_NORMAL) != CollectibleCardFilter.FilterMask.NONE && this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardId, TAG_PREMIUM.NORMAL), out collectibleCard))
      countByFilterMask += collectibleCard.OwnedCount;
    if ((filterMask & CollectibleCardFilter.FilterMask.PREMIUM_GOLDEN) != CollectibleCardFilter.FilterMask.NONE && this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardId, TAG_PREMIUM.GOLDEN), out collectibleCard))
      countByFilterMask += collectibleCard.OwnedCount;
    if ((filterMask & CollectibleCardFilter.FilterMask.PREMIUM_SIGNATURE) != CollectibleCardFilter.FilterMask.NONE && this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardId, TAG_PREMIUM.SIGNATURE), out collectibleCard))
      countByFilterMask += collectibleCard.OwnedCount;
    if ((filterMask & CollectibleCardFilter.FilterMask.PREMIUM_DIAMOND) != CollectibleCardFilter.FilterMask.NONE && this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardId, TAG_PREMIUM.DIAMOND), out collectibleCard))
      countByFilterMask += collectibleCard.OwnedCount;
    return countByFilterMask;
  }

  public List<TAG_CARD_SET> GetDisplayableCardSets() => this.m_displayableCardSets;

  public bool IsCardInCollection(string cardID, TAG_PREMIUM premium)
  {
    CollectibleCard collectibleCard;
    return this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardID, premium), out collectibleCard) && collectibleCard.OwnedCount > 0;
  }

  public int GetNumCopiesInCollection(string cardID, TAG_PREMIUM premium)
  {
    CollectibleCard collectibleCard;
    return this.m_collectibleCardIndex.TryGetValue(new CollectionManager.CollectibleCardIndex(cardID, premium), out collectibleCard) ? collectibleCard.OwnedCount : 0;
  }

  public int GetTotalNumCopiesInCollection(string cardID)
  {
    int copiesInCollection = 0;
    CollectionManager.CollectibleCardIndex key;
    key.CardId = cardID;
    key.Premium = TAG_PREMIUM.NORMAL;
    CollectibleCard collectibleCard1;
    if (this.m_collectibleCardIndex.TryGetValue(key, out collectibleCard1))
      copiesInCollection += collectibleCard1.OwnedCount;
    key.Premium = TAG_PREMIUM.GOLDEN;
    CollectibleCard collectibleCard2;
    if (this.m_collectibleCardIndex.TryGetValue(key, out collectibleCard2))
      copiesInCollection += collectibleCard2.OwnedCount;
    key.Premium = TAG_PREMIUM.DIAMOND;
    CollectibleCard collectibleCard3;
    if (this.m_collectibleCardIndex.TryGetValue(key, out collectibleCard3))
      copiesInCollection += collectibleCard3.OwnedCount;
    return copiesInCollection;
  }

  public void GetMassDisenchantCards(List<CollectibleCard> collectibleCards)
  {
    collectibleCards.Clear();
    foreach (CollectibleCard ownedCard in this.GetOwnedCards())
    {
      if (ownedCard.DisenchantCount > 0)
        collectibleCards.Add(ownedCard);
    }
  }

  public void GetMassDisenchantCardsAndCount(
    List<CollectibleCard> collectibleCards,
    out int disenchantCount)
  {
    collectibleCards.Clear();
    disenchantCount = 0;
    foreach (CollectibleCard ownedCard in this.GetOwnedCards())
    {
      int disenchantCount1 = ownedCard.DisenchantCount;
      if (disenchantCount1 > 0)
      {
        collectibleCards.Add(ownedCard);
        disenchantCount += disenchantCount1;
      }
    }
  }

  public int GetCardsToDisenchantCount()
  {
    int toDisenchantCount = 0;
    foreach (CollectibleCard ownedCard in this.GetOwnedCards())
      toDisenchantCount += ownedCard.DisenchantCount;
    return toDisenchantCount;
  }

  public void MarkAllInstancesAsSeen(string cardID, TAG_PREMIUM premium)
  {
    NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
    int dbId = GameUtils.TranslateCardIdToDbId(cardID);
    if (dbId == 0)
      return;
    CollectibleCard card = this.GetCard(cardID, premium);
    if (card == null || card.SeenCount == card.OwnedCount)
      return;
    Network.Get().AckCardSeenBefore(dbId, premium);
    card.SeenCount = card.OwnedCount;
    NetCache.CardStack cardStack = netObject.Stacks.Find((Predicate<NetCache.CardStack>) (obj => obj.Def.Name == card.CardId && obj.Def.Premium == card.PremiumType));
    if (cardStack != null)
      cardStack.NumSeen = cardStack.Count;
    foreach (CollectionManager.DelOnNewCardSeen cardSeenListener in this.m_newCardSeenListeners)
      cardSeenListener(cardID, premium);
  }

  public void OnCardAdded(string cardID, TAG_PREMIUM premium, int count, bool seenBefore)
  {
    this.InsertNewCollectionCard(cardID, premium, DateTime.Now, count, seenBefore);
    this.OnCollectionChanged();
  }

  public void OnCardRemoved(string cardID, TAG_PREMIUM premium, int count)
  {
    this.RemoveCollectionCard(cardID, premium, count);
    this.OnCollectionChanged();
  }

  public void OnUIHeroOverrideCardRemoved()
  {
    if (this.m_onUIHeroOverrideCardRemovedListeners.Count <= 0)
      return;
    foreach (CollectionManager.OnUIHeroOverrideCardRemovedListener cardRemovedListener in this.m_onUIHeroOverrideCardRemovedListeners.ToArray())
      cardRemovedListener.Fire();
  }

  public CollectionManager.PreconDeck GetPreconDeck(TAG_CLASS heroClass)
  {
    if (this.m_preconDecks.ContainsKey(heroClass))
      return this.m_preconDecks[heroClass];
    Log.All.PrintWarning(string.Format("CollectionManager.GetPreconDeck(): Could not retrieve precon deck for class {0}", (object) heroClass));
    return (CollectionManager.PreconDeck) null;
  }

  public SortedDictionary<long, CollectionDeck> GetDecks()
  {
    SortedDictionary<long, CollectionDeck> decks = new SortedDictionary<long, CollectionDeck>();
    foreach (KeyValuePair<long, CollectionDeck> deck in this.m_decks)
    {
      CollectionDeck collectionDeck = deck.Value;
      if (collectionDeck != null && (!collectionDeck.IsBrawlDeck || TavernBrawlManager.Get().IsSeasonActive(collectionDeck.Type, collectionDeck.SeasonId, collectionDeck.BrawlLibraryItemId)))
        decks.Add(deck.Key, deck.Value);
    }
    return decks;
  }

  public List<CollectionDeck> GetDecks(DeckType deckType)
  {
    if (!NetCache.Get().IsNetObjectAvailable<NetCache.NetCacheDecks>())
      Debug.LogWarning((object) "Attempting to get decks from CollectionManager, even though NetCacheDecks is not ready (meaning it's waiting for the decks to be updated)!");
    List<CollectionDeck> decks = new List<CollectionDeck>();
    foreach (CollectionDeck collectionDeck in this.m_decks.Values)
    {
      if (collectionDeck.Type == deckType && (!collectionDeck.IsBrawlDeck || TavernBrawlManager.Get().IsSeasonActive(collectionDeck.Type, collectionDeck.SeasonId, collectionDeck.BrawlLibraryItemId)))
        decks.Add(collectionDeck);
    }
    decks.Sort((IComparer<CollectionDeck>) new CollectionManager.DeckSort());
    return decks;
  }

  public List<long> LoadDeckFromDBF(
    int deckID,
    out string deckName,
    out string deckDescription)
  {
    deckName = string.Empty;
    deckDescription = string.Empty;
    DeckDbfRecord record = GameDbf.Deck.GetRecord(deckID);
    if (record == null)
    {
      Debug.LogError((object) string.Format("Unable to find deck with ID {0}", (object) deckID));
      return (List<long>) null;
    }
    if (record.Name == null)
      Debug.LogErrorFormat("Deck with ID {0} has no name defined.", (object) deckID);
    else
      deckName = record.Name.GetString();
    if (record.Description != null)
      deckDescription = record.Description.GetString();
    List<long> longList = new List<long>();
    int nextCard;
    for (DeckCardDbfRecord deckCardDbfRecord = GameDbf.DeckCard.GetRecord(record.TopCardId); deckCardDbfRecord != null; deckCardDbfRecord = nextCard != 0 ? GameDbf.DeckCard.GetRecord(nextCard) : (DeckCardDbfRecord) null)
    {
      int cardId = deckCardDbfRecord.CardId;
      longList.Add((long) cardId);
      nextCard = deckCardDbfRecord.NextCard;
    }
    return longList;
  }

  public CollectionDeck GetDeck(long id)
  {
    CollectionDeck collectionDeck;
    if (!this.m_decks.TryGetValue(id, out collectionDeck))
      return (CollectionDeck) null;
    return collectionDeck != null && collectionDeck.IsBrawlDeck && !TavernBrawlManager.Get().IsSeasonActive(collectionDeck.Type, collectionDeck.SeasonId, collectionDeck.BrawlLibraryItemId) ? (CollectionDeck) null : collectionDeck;
  }

  public CollectionDeck GetDuelsDeck()
  {
    List<CollectionDeck> decks = this.GetDecks(DeckType.PVPDR_DECK);
    if (decks != null)
    {
      for (int index = 0; index < decks.Count; ++index)
      {
        if (decks[index].ID == this.m_currentPVPDRDeckId && !decks[index].IsBeingDeleted())
          return decks[index];
      }
    }
    return (CollectionDeck) null;
  }

  public bool AreAllDeckContentsReady() => FixedRewardsMgr.Get().IsStartupFinished() && this.m_decks.FirstOrDefault<KeyValuePair<long, CollectionDeck>>((Func<KeyValuePair<long, CollectionDeck>, bool>) (kv => !kv.Value.NetworkContentsLoaded() && !kv.Value.IsBrawlDeck && !kv.Value.IsDuelsDeck)).Value == null;

  public bool ShouldAccountSeeStandardWild() => RankMgr.Get().WildCardsAllowedInCurrentLeague() && this.AccountHasUnlockedWild();

  public bool AccountHasUnlockedWild()
  {
    long num = 0;
    return GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.PLAYER_FLAGS, GameSaveKeySubkeyId.PLAYER_FLAGS_UNLOCKED_WILD, out num) && num != 0L;
  }

  public bool AccountHasRotatedBoosters(DateTime utcTimestamp)
  {
    NetCache.NetCacheBoosters netObject = NetCache.Get().GetNetObject<NetCache.NetCacheBoosters>();
    if (netObject != null)
    {
      foreach (NetCache.BoosterStack boosterStack in netObject.BoosterStacks)
      {
        if (GameUtils.IsBoosterRotated((BoosterDbId) boosterStack.Id, utcTimestamp))
          return true;
      }
    }
    return false;
  }

  public bool AccountHasWildCards()
  {
    if (this.GetNumberOfWildDecks() > 0)
      return true;
    if ((double) this.m_lastSearchForWildCardsTime > (double) this.m_collectionLastModifiedTime)
      return this.m_accountHasWildCards;
    this.m_accountHasWildCards = this.m_collectibleCards.Any<CollectibleCard>((Func<CollectibleCard, bool>) (c => c.OwnedCount > 0 && GameUtils.IsCardRotated(c.GetEntityDef())));
    this.m_lastSearchForWildCardsTime = Time.realtimeSinceStartup;
    return this.m_accountHasWildCards;
  }

  public int GetNumberOfWildDecks() => this.m_decks.Values.Count<CollectionDeck>((Func<CollectionDeck, bool>) (deck => deck.FormatType == PegasusShared.FormatType.FT_WILD));

  public int GetNumberOfStandardDecks() => this.m_decks.Values.Count<CollectionDeck>((Func<CollectionDeck, bool>) (deck => deck.FormatType == PegasusShared.FormatType.FT_STANDARD));

  public int GetNumberOfClassicDecks() => this.m_decks.Values.Count<CollectionDeck>((Func<CollectionDeck, bool>) (deck => deck.FormatType == PegasusShared.FormatType.FT_CLASSIC));

  public bool AccountHasValidDeck(PegasusShared.FormatType formatType)
  {
    foreach (CollectionDeck deck in CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK))
    {
      if (deck.IsValidForRuleset && deck.IsValidForFormat(formatType))
        return true;
    }
    return false;
  }

  public CollectionDeck GetEditedDeck()
  {
    CollectionDeck editedDeck = this.m_EditedDeck;
    if (editedDeck != null && editedDeck.IsBrawlDeck)
    {
      TavernBrawlManager tavernBrawlManager = TavernBrawlManager.Get();
      if (tavernBrawlManager != null)
      {
        TavernBrawlMission tavernBrawlMission = tavernBrawlManager.IsCurrentBrawlTypeActive ? tavernBrawlManager.CurrentMission() : (TavernBrawlMission) null;
        if (tavernBrawlMission == null || editedDeck.SeasonId != tavernBrawlMission.seasonId)
          return (CollectionDeck) null;
      }
    }
    return editedDeck;
  }

  public int GetDeckSize() => this.m_deckRuleset == null ? 30 : this.m_deckRuleset.GetDeckSize(this.GetEditedDeck());

  public int GetDeckSizeWhileEditing(EntityDef cardBeingAdded = null) => this.m_deckRuleset == null ? 30 : this.m_deckRuleset.GetDeckSizeWhileEditing(this.GetEditedDeck(), cardBeingAdded);

  public List<CollectionManager.TemplateDeck> GetTemplateDecks(
    PegasusShared.FormatType formatType,
    TAG_CLASS classType)
  {
    if (this.m_templateDeckMap.Values.Count == 0)
      this.LoadTemplateDecks();
    List<CollectionManager.TemplateDeck> source = (List<CollectionManager.TemplateDeck>) null;
    this.m_templateDecks.TryGetValue(classType, out source);
    return formatType == PegasusShared.FormatType.FT_WILD ? source.Where<CollectionManager.TemplateDeck>((Func<CollectionManager.TemplateDeck, bool>) (x => x.m_formatType == PegasusShared.FormatType.FT_STANDARD || x.m_formatType == PegasusShared.FormatType.FT_WILD)).ToList<CollectionManager.TemplateDeck>() : source.Where<CollectionManager.TemplateDeck>((Func<CollectionManager.TemplateDeck, bool>) (x => x.m_formatType == formatType)).ToList<CollectionManager.TemplateDeck>();
  }

  public List<CollectionManager.TemplateDeck> GetNonStarterTemplateDecks(
    PegasusShared.FormatType formatType,
    TAG_CLASS classType)
  {
    List<CollectionManager.TemplateDeck> templateDecks = this.GetTemplateDecks(formatType, classType);
    return templateDecks == null ? (List<CollectionManager.TemplateDeck>) null : templateDecks.Where<CollectionManager.TemplateDeck>((Func<CollectionManager.TemplateDeck, bool>) (x => !x.m_isStarterDeck)).ToList<CollectionManager.TemplateDeck>();
  }

  public CollectionManager.TemplateDeck GetTemplateDeck(int id)
  {
    if (this.m_templateDeckMap.Values.Count == 0)
      this.LoadTemplateDecks();
    CollectionManager.TemplateDeck templateDeck;
    this.m_templateDeckMap.TryGetValue(id, out templateDeck);
    return templateDeck;
  }

  public bool IsInEditMode() => this.m_editMode;

  public bool IsEditingDeathKnightDeck()
  {
    CollectionDeck editedDeck = this.GetEditedDeck();
    return editedDeck != null && editedDeck.HasClass(TAG_CLASS.DEATHKNIGHT);
  }

  public void StartEditingDeck(CollectionDeck deck, object callbackData = null)
  {
    if (deck == null)
      return;
    this.m_editMode = true;
    FriendChallengeMgr.Get().UpdateMyAvailability();
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
    {
      collectibleDisplay.SetHeroSkinClass(new TAG_CLASS?());
      ActiveFilterButton filterButton = collectibleDisplay.GetFilterButton();
      if ((UnityEngine.Object) filterButton != (UnityEngine.Object) null)
        filterButton.UpdateFilterView();
      collectibleDisplay.HideAllCosmeticTips();
    }
    DeckRuleset deckRuleset;
    if (SceneMgr.Get().IsInTavernBrawlMode())
      deckRuleset = TavernBrawlManager.Get().GetCurrentDeckRuleset();
    else if (SceneMgr.Get().GetMode() == SceneMgr.Mode.PVP_DUNGEON_RUN)
    {
      deckRuleset = deck.Type != DeckType.PVPDR_DECK ? DeckRuleset.GetPVPDRDisplayRuleset() : DeckRuleset.GetPVPDRRuleset();
    }
    else
    {
      deckRuleset = DeckRuleset.GetRuleset(deck.FormatType);
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DECKEDITOR);
    }
    this.SetDeckRuleset(deckRuleset);
    this.SetEditedDeck(deck, callbackData);
  }

  public void DoneEditing()
  {
    int num = this.m_editMode ? 1 : 0;
    this.m_editMode = false;
    FriendChallengeMgr.Get().UpdateMyAvailability();
    if (num != 0 && SceneMgr.Get() != null && !SceneMgr.Get().IsInTavernBrawlMode())
      PresenceMgr.Get().SetPrevStatus();
    this.SetDeckRuleset((DeckRuleset) null);
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if (num != 0 && (UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
      collectibleDisplay.EnableTutorialsByViewMode(collectibleDisplay.GetViewMode());
    if (SceneMgr.Get().IsInLettuceMode())
      this.ClearEditingTeam();
    else
      this.ClearEditedDeck();
  }

  public DeckRuleset GetDeckRuleset() => this.m_deckRuleset;

  public PegasusShared.FormatType GetThemeShowing(CollectionDeck deck = null)
  {
    if (CollectionUtils.IsHeroSkinDisplayMode(this.GetCollectibleDisplay().GetViewMode()))
      return deck == null ? PegasusShared.FormatType.FT_STANDARD : deck.FormatType;
    if (CollectionManagerDisplay.IsSpecialOneDeckMode())
      return PegasusShared.FormatType.FT_STANDARD;
    if (deck == null)
      deck = this.GetEditedDeck();
    if (deck != null && deck.Type != DeckType.CLIENT_ONLY_DECK)
      return deck.FormatType;
    CollectionPageManager collectionPageManager = (UnityEngine.Object) this.m_collectibleDisplay != (UnityEngine.Object) null ? this.m_collectibleDisplay.GetPageManager() as CollectionPageManager : (CollectionPageManager) null;
    if ((UnityEngine.Object) this.m_collectibleDisplay != (UnityEngine.Object) null && (UnityEngine.Object) collectionPageManager != (UnityEngine.Object) null && this.m_collectibleDisplay.SetFilterTrayInitialized())
    {
      if (collectionPageManager.CardSetFilterIncludesWild())
        return PegasusShared.FormatType.FT_WILD;
      if (collectionPageManager.CardSetFilterIsClassic())
        return PegasusShared.FormatType.FT_CLASSIC;
    }
    return PegasusShared.FormatType.FT_STANDARD;
  }

  public void SetDeckRuleset(DeckRuleset deckRuleset)
  {
    this.m_deckRuleset = deckRuleset;
    CollectionPageManager collectionPageManager = (UnityEngine.Object) this.m_collectibleDisplay != (UnityEngine.Object) null ? this.m_collectibleDisplay.GetPageManager() as CollectionPageManager : (CollectionPageManager) null;
    if (!((UnityEngine.Object) collectionPageManager != (UnityEngine.Object) null))
      return;
    collectionPageManager.SetDeckRuleset(deckRuleset);
  }

  public void SetEditedDeck(CollectionDeck deck, object callbackData = null)
  {
    CollectionDeck editedDeck = this.GetEditedDeck();
    if (deck == editedDeck)
      return;
    this.m_EditedDeck = deck;
    foreach (CollectionManager.OnEditedDeckChanged editedDeckChanged in this.m_editedDeckChangedListeners.ToArray())
      editedDeckChanged(deck, editedDeck, callbackData);
  }

  public void ClearEditedDeck() => this.SetEditedDeck((CollectionDeck) null);

  public void SendCreateDeck(
    DeckType deckType,
    string name,
    string heroCardID,
    DeckSourceType deckSourceType = DeckSourceType.DECK_SOURCE_TYPE_NORMAL,
    string pastedDeckHashString = null)
  {
    int dbId = GameUtils.TranslateCardIdToDbId(heroCardID);
    if (dbId == 0)
    {
      Debug.LogWarning((object) string.Format("CollectionManager.SendCreateDeck(): Unknown hero cardID {0}", (object) heroCardID));
    }
    else
    {
      PegasusShared.FormatType formatType = Options.GetFormatType();
      int brawlLibraryItemId = 0;
      if (SceneMgr.Get().IsInTavernBrawlMode())
        formatType = TavernBrawlManager.Get().CurrentMission().formatType;
      if (deckType == DeckType.PVPDR_DECK)
        formatType = PegasusShared.FormatType.FT_WILD;
      if (formatType == PegasusShared.FormatType.FT_UNKNOWN)
      {
        Debug.LogWarning((object) string.Format("CollectionManager.SendCreateDeck(): Bad format type {0}", (object) formatType.ToString()));
      }
      else
      {
        switch (deckType)
        {
          case DeckType.TAVERN_BRAWL_DECK:
          case DeckType.FSG_BRAWL_DECK:
            brawlLibraryItemId = TavernBrawlManager.Get().CurrentMission().SelectedBrawlLibraryItemId;
            break;
        }
        if (this.m_pendingDeckCreate != null)
          Log.Offline.PrintWarning("SendCreateDeck - Attempting to create a deck while another is still pending.");
        this.m_pendingDeckCreate = new CollectionManager.PendingDeckCreateData()
        {
          m_deckType = deckType,
          m_name = name,
          m_heroDbId = dbId,
          m_formatType = formatType,
          m_sourceType = deckSourceType,
          m_pastedDeckHash = pastedDeckHashString
        };
        if (Network.IsLoggedIn())
        {
          int? requestId;
          Network.Get().CreateDeck(deckType, name, dbId, formatType, -100L, deckSourceType, out requestId, pastedDeckHashString, brawlLibraryItemId);
          if (!requestId.HasValue)
            return;
          this.m_inTransitDeckCreateRequests.Add(requestId.Value);
        }
        else
          this.CreateDeckOffline(this.m_pendingDeckCreate);
      }
    }
  }

  private void CreateDeckOffline(CollectionManager.PendingDeckCreateData data)
  {
    DeckInfo deck = OfflineDataCache.CreateDeck(data.m_deckType, data.m_name, data.m_heroDbId, data.m_formatType, -100L, data.m_sourceType, data.m_pastedDeckHash);
    if (deck == null)
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_OFFLINE_FEATURE_DISABLED_HEADER"),
        m_text = GameStrings.Get("GLUE_OFFLINE_DECK_ERROR_BODY"),
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_showAlertIcon = true
      };
      DialogManager.Get().ShowPopup(info);
      CollectionManagerDisplay collectibleDisplay = this.m_collectibleDisplay as CollectionManagerDisplay;
      if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
        collectibleDisplay.CancelSelectNewDeckHeroMode();
      if (!((UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null))
        return;
      CollectionDeckTray.Get().m_doneButton.SetEnabled(true);
    }
    else
    {
      NetCache.DeckHeader deckHeader = Network.GetDeckHeaderFromDeckInfo(deck);
      Processor.ScheduleCallback(0.5f, false, (Processor.ScheduledCallback) (_0 => this.OnDeckCreated(deckHeader, new int?())));
    }
  }

  public void HandleDisconnect()
  {
    if (this.m_pendingDeckCreate != null)
    {
      this.CreateDeckOffline(this.m_pendingDeckCreate);
      this.m_pendingDeckCreate = (CollectionManager.PendingDeckCreateData) null;
    }
    if (this.m_pendingDeckDeleteList != null)
    {
      foreach (CollectionManager.PendingDeckDeleteData pendingDeckDeleteData in this.m_pendingDeckDeleteList.ToArray())
        this.OnDeckDeletedWhileOffline(pendingDeckDeleteData.m_deckId);
      this.m_pendingDeckDeleteList = (List<CollectionManager.PendingDeckDeleteData>) null;
    }
    if (this.m_pendingDeckEditList != null)
    {
      foreach (CollectionManager.PendingDeckEditData pendingDeckEdit in this.m_pendingDeckEditList)
        this.GetDeck(pendingDeckEdit.m_deckId)?.OnContentChangesComplete();
      this.m_pendingDeckEditList = (List<CollectionManager.PendingDeckEditData>) null;
    }
    if (this.m_pendingDeckRenameList == null)
      return;
    foreach (CollectionManager.PendingDeckRenameData pendingDeckRename in this.m_pendingDeckRenameList)
    {
      CollectionDeck deck = this.GetDeck(pendingDeckRename.m_deckId);
      if (deck != null)
      {
        OfflineDataCache.RenameDeck(pendingDeckRename.m_deckId, pendingDeckRename.m_name);
        deck.OnNameChangeComplete();
      }
    }
    this.m_pendingDeckRenameList = (List<CollectionManager.PendingDeckRenameData>) null;
  }

  public bool RequestDeckContentsForDecksWithoutContentsLoaded(
    CollectionManager.DelOnAllDeckContents callback = null)
  {
    float now = Time.realtimeSinceStartup;
    IEnumerable<KeyValuePair<long, CollectionDeck>> source1 = this.m_decks.Where<KeyValuePair<long, CollectionDeck>>((Func<KeyValuePair<long, CollectionDeck>, bool>) (kv => !kv.Value.NetworkContentsLoaded())).Where<KeyValuePair<long, CollectionDeck>>((Func<KeyValuePair<long, CollectionDeck>, bool>) (kv => !kv.Value.IsBrawlDeck || TavernBrawlManager.Get().IsTavernBrawlActiveByDeckType(kv.Value.Type)));
    if (!source1.Any<KeyValuePair<long, CollectionDeck>>())
    {
      if (callback != null)
        callback();
      return false;
    }
    if (callback != null && !this.m_allDeckContentsListeners.Contains(callback))
      this.m_allDeckContentsListeners.Add(callback);
    if (this.m_pendingRequestDeckContents != null)
      source1 = source1.Where<KeyValuePair<long, CollectionDeck>>((Func<KeyValuePair<long, CollectionDeck>, bool>) (kv => !this.m_pendingRequestDeckContents.ContainsKey(kv.Value.ID) || (double) now - (double) this.m_pendingRequestDeckContents[kv.Value.ID] >= 10.0));
    IEnumerable<long> source2 = source1.Select<KeyValuePair<long, CollectionDeck>, long>((Func<KeyValuePair<long, CollectionDeck>, long>) (kv => kv.Value.ID));
    if (!source2.Any<long>())
      return true;
    long[] array = source2.ToArray<long>();
    if (this.m_pendingRequestDeckContents == null)
      this.m_pendingRequestDeckContents = new Map<long, float>();
    for (int index = 0; index < array.Length; ++index)
      this.m_pendingRequestDeckContents[array[index]] = now;
    Network.Get().RequestDeckContents(array);
    return true;
  }

  public void RequestDeckContents(long id)
  {
    CollectionDeck deck = this.GetDeck(id);
    if (deck != null && deck.NetworkContentsLoaded())
      this.FireDeckContentsEvent(id);
    else if (Network.IsLoggedIn())
    {
      float realtimeSinceStartup = Time.realtimeSinceStartup;
      float num;
      if (this.m_pendingRequestDeckContents != null && this.m_pendingRequestDeckContents.TryGetValue(id, out num))
      {
        if ((double) realtimeSinceStartup - (double) num < 10.0)
          return;
        this.m_pendingRequestDeckContents.Remove(id);
      }
      if (this.m_pendingRequestDeckContents == null)
        this.m_pendingRequestDeckContents = new Map<long, float>();
      this.m_pendingRequestDeckContents[id] = realtimeSinceStartup;
      Network.Get().RequestDeckContents(id);
    }
    else
      this.OnGetDeckContentsResponse();
  }

  public CollectionDeck GetBaseDeck(long id)
  {
    CollectionDeck collectionDeck;
    return this.m_baseDecks.TryGetValue(id, out collectionDeck) ? collectionDeck : (CollectionDeck) null;
  }

  public string AutoGenerateDeckName(TAG_CLASS classTag)
  {
    string className = GameStrings.GetClassName(classTag);
    int num = 1;
    string name;
    do
    {
      name = GameStrings.Format("GLUE_COLLECTION_CUSTOM_DECKNAME_TEMPLATE", (object) className, num == 1 ? (object) "" : (object) num.ToString());
      if (name.Length > CollectionDeck.DefaultMaxDeckNameCharacters)
        name = GameStrings.Format("GLUE_COLLECTION_CUSTOM_DECKNAME_SHORT", (object) className, num == 1 ? (object) "" : (object) num.ToString());
      ++num;
    }
    while (this.IsDeckNameTaken(name));
    return name;
  }

  public bool HasPendingSmartDeckRequest(long deckId) => this.m_smartDeckCallbackByDeckId.ContainsKey(deckId);

  public void AutoFillDeck(
    CollectionDeck deck,
    bool allowSmartDeckCompletion,
    CollectionManager.DeckAutoFillCallback resultCallback)
  {
    if (this.HasPendingSmartDeckRequest(deck.ID))
      return;
    deck.IsCreatedWithDeckComplete = true;
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().EnableSmartDeckCompletion)
      allowSmartDeckCompletion = false;
    if (!Network.IsLoggedIn())
      allowSmartDeckCompletion = false;
    if (deck.FormatType == PegasusShared.FormatType.FT_CLASSIC)
      allowSmartDeckCompletion = false;
    if (allowSmartDeckCompletion)
    {
      this.m_smartDeckCallbackByDeckId.Add(deck.ID, resultCallback);
      Network.Get().RequestSmartDeckCompletion(deck);
      Processor.ScheduleCallback(5f, true, new Processor.ScheduledCallback(this.OnSmartDeckTimeout), (object) deck.ID);
    }
    else
      resultCallback(deck, DeckMaker.GetFillCards(deck, deck.GetRuleset()));
  }

  private void OnSmartDeckTimeout(object userdata)
  {
    long num = (long) userdata;
    if (!this.HasPendingSmartDeckRequest(num))
      return;
    CollectionDeck deck = this.GetDeck(num);
    IEnumerable<DeckMaker.DeckFill> fillCards = DeckMaker.GetFillCards(deck, deck.GetRuleset());
    this.m_smartDeckCallbackByDeckId[num](deck, fillCards);
    this.m_smartDeckCallbackByDeckId.Remove(num);
  }

  private void OnSmartDeckResponse()
  {
    SmartDeckResponse smartDeckResponse1 = Network.Get().GetSmartDeckResponse();
    if (smartDeckResponse1.HasErrorCode && smartDeckResponse1.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.CollectionManager.PrintError("OnSmartDeckResponse: Response contained errors. ErrorCode=" + (object) smartDeckResponse1.ErrorCode);
      if (smartDeckResponse1.ResponseMessage != null)
        this.OnSmartDeckTimeout((object) smartDeckResponse1.ResponseMessage.DeckId);
    }
    if (smartDeckResponse1.ResponseMessage == null)
      return;
    long deckId = smartDeckResponse1.ResponseMessage.DeckId;
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.OnSmartDeckTimeout), (object) deckId);
    if (!this.HasPendingSmartDeckRequest(deckId))
      return;
    CollectionDeck deck = this.GetDeck(deckId);
    List<DeckMaker.DeckFill> smartDeckResponse2 = this.GetCardFillFromSmartDeckResponse(deck, smartDeckResponse1);
    this.m_smartDeckCallbackByDeckId[deckId](deck, (IEnumerable<DeckMaker.DeckFill>) smartDeckResponse2);
    this.m_smartDeckCallbackByDeckId.Remove(deckId);
  }

  private List<DeckMaker.DeckFill> GetCardFillFromSmartDeckResponse(
    CollectionDeck deck,
    SmartDeckResponse response)
  {
    Log.CollectionManager.PrintDebug("Smart Deck Response Received: " + response.ToHumanReadableString());
    List<DeckMaker.DeckFill> smartDeckResponse = new List<DeckMaker.DeckFill>();
    foreach (DeckCardData deckCardData in response.ResponseMessage.PlayerDeckCard)
    {
      string cardId = GameUtils.TranslateDbIdToCardId(deckCardData.Def.Asset);
      int num = deckCardData.Qty - deck.GetCardIdCount(cardId);
      for (int index = 0; index < num; ++index)
        smartDeckResponse.Add(new DeckMaker.DeckFill()
        {
          m_addCard = DefLoader.Get().GetEntityDef(deckCardData.Def.Asset)
        });
    }
    int num1 = deck.GetTotalValidCardCount() + smartDeckResponse.Count;
    int num2 = deck.GetMaxCardCount() - num1;
    if (num2 > 0)
    {
      smartDeckResponse.AddRange(DeckMaker.GetFillCards(deck, deck.GetRuleset()));
      Log.CollectionManager.PrintWarning("Smart Deck: Insufficient number of cards. Adding {0} more cards to deck {1}.", (object) num2, (object) deck.ID);
    }
    return smartDeckResponse;
  }

  private bool OnBnetError(BnetErrorInfo info, object _)
  {
    if (info.GetError() != BattleNetErrors.ERROR_ATTRIBUTE_MAX_SIZE_EXCEEDED || this.m_smartDeckCallbackByDeckId.Count <= 0)
      return false;
    Log.CollectionManager.PrintError(string.Format("BnetError {0}: timing out all pending Smart Deck requests.", (object) info));
    foreach (long num in this.m_smartDeckCallbackByDeckId.Keys.ToArray<long>())
    {
      SmartDeckRequest deckRequestMessage = Network.GenerateSmartDeckRequestMessage(this.GetDeck(num));
      TelemetryManager.Client().SendSmartDeckCompleteFailed((int) deckRequestMessage.GetSerializedSize());
      this.OnSmartDeckTimeout((object) num);
    }
    return true;
  }

  public static string GetHeroCardId(TAG_CLASS heroClass, CardHero.HeroType heroType)
  {
    if (heroClass == TAG_CLASS.WHIZBANG)
      return "BOT_914h";
    foreach (CardHeroDbfRecord record in GameDbf.CardHero.GetRecords())
    {
      if (record.HeroType == heroType && GameUtils.GetTagClassFromCardDbId(record.CardId) == heroClass)
        return GameUtils.TranslateDbIdToCardId(record.CardId);
    }
    return string.Empty;
  }

  public static string GetVanillaHero(TAG_CLASS classTag) => CollectionManager.GetHeroCardId(classTag, CardHero.HeroType.VANILLA);

  public TAG_PREMIUM GetHeroPremium(TAG_CLASS classTag) => this.GetBestCardPremium(CollectionManager.GetVanillaHero(classTag));

  public bool ShouldShowDeckTemplatePageForClass(TAG_CLASS classType) => (Options.Get().GetInt(Option.SKIP_DECK_TEMPLATE_PAGE_FOR_CLASS_FLAGS, 0) & 1 << (int) (classType & (TAG_CLASS) 31)) == 0;

  public void SetShowDeckTemplatePageForClass(TAG_CLASS classType, bool show)
  {
    int num1 = Options.Get().GetInt(Option.SKIP_DECK_TEMPLATE_PAGE_FOR_CLASS_FLAGS, 0);
    int num2 = 1 << (int) (classType & (TAG_CLASS) 31);
    int val = num1 | num2;
    if (show)
      val ^= num2;
    Options.Get().SetInt(Option.SKIP_DECK_TEMPLATE_PAGE_FOR_CLASS_FLAGS, val);
  }

  public bool ShouldShowWildToStandardTutorial(bool checkPrevSceneIsPlayMode = true) => this.ShouldAccountSeeStandardWild() && SceneMgr.Get().GetMode() == SceneMgr.Mode.COLLECTIONMANAGER && (!checkPrevSceneIsPlayMode || SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.TOURNAMENT) && Options.Get().GetBool(Option.NEEDS_TO_MAKE_STANDARD_DECK);

  public bool UpdateDeckWithNewId(long oldId, long newId)
  {
    if ((UnityEngine.Object) CollectionDeckTray.Get() != (UnityEngine.Object) null && !CollectionDeckTray.Get().GetDecksContent().UpdateDeckBoxWithNewId(oldId, newId))
      return false;
    CollectionDeck editedDeck = this.GetEditedDeck();
    if (this.IsInEditMode() && editedDeck.ID == oldId && this.m_decks.ContainsKey(newId))
    {
      this.m_decks[newId].CopyContents(editedDeck);
      this.SetEditedDeck(this.m_decks[newId]);
    }
    this.RemoveDeck(oldId);
    return true;
  }

  public int GetOwnedCount(string cardId, TAG_PREMIUM premium)
  {
    int normal;
    int golden;
    int signature;
    int diamond;
    this.GetOwnedCardCount(cardId, out normal, out golden, out signature, out diamond);
    int ownedCount = 0;
    switch (premium)
    {
      case TAG_PREMIUM.NORMAL:
        ownedCount = normal;
        break;
      case TAG_PREMIUM.GOLDEN:
        ownedCount = golden;
        break;
      case TAG_PREMIUM.DIAMOND:
        ownedCount = diamond;
        break;
      case TAG_PREMIUM.SIGNATURE:
        ownedCount = signature;
        break;
    }
    return ownedCount;
  }

  public int GetTotalOwnedCount(string cardId)
  {
    int normal;
    int golden;
    int signature;
    int diamond;
    this.GetOwnedCardCount(cardId, out normal, out golden, out signature, out diamond);
    return normal + golden + signature + diamond;
  }

  private void InitImpl()
  {
    this.m_filterIsSetRotatedCache = new Map<TAG_CARD_SET, bool>(Blizzard.T5.Core.Utils.EnumUtils.Length<TAG_CARD_SET>(), (IEqualityComparer<TAG_CARD_SET>) new CollectionManager.TagCardSetEnumComparer());
    List<CardTagDbfRecord> all = GameDbf.CardTag.GetRecords().FindAll((Predicate<CardTagDbfRecord>) (record =>
    {
      GAME_TAG tagId = (GAME_TAG) record.TagId;
      return tagId == GAME_TAG.HAS_DIAMOND_QUALITY || tagId == GAME_TAG.HAS_SIGNATURE_QUALITY;
    }));
    List<string> collectibleCardIds = GameUtils.GetAllCollectibleCardIds();
    this.m_collectibleCardIndex = new Map<CollectionManager.CollectibleCardIndex, CollectibleCard>(collectibleCardIds.Count * 2 + all.Count, (IEqualityComparer<CollectionManager.CollectibleCardIndex>) new CollectionManager.CollectibleCardIndexComparer());
    this.m_collectibleCards = new List<CollectibleCard>(collectibleCardIds.Count * 2 + all.Count);
    DefLoader defLoader = DefLoader.Get();
    Dictionary<int, List<CardSetTimingDbfRecord>> timings = new Dictionary<int, List<CardSetTimingDbfRecord>>(collectibleCardIds.Count);
    foreach (string cardId in collectibleCardIds)
    {
      CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(cardId);
      timings.Add(cardRecord.ID, new List<CardSetTimingDbfRecord>(1));
    }
    List<CardSetTimingDbfRecord> records = GameDbf.CardSetTiming.GetRecords();
    int index = 0;
    for (int count = records.Count; index < count; ++index)
    {
      CardSetTimingDbfRecord setTimingDbfRecord = records[index];
      int cardId = setTimingDbfRecord.CardId;
      if (timings.ContainsKey(cardId))
        timings[cardId].Add(setTimingDbfRecord);
    }
    foreach (string str in collectibleCardIds)
    {
      EntityDef entityDef = defLoader.GetEntityDef(str);
      if (entityDef == null)
      {
        Error.AddDevFatal("Failed to find an EntityDef for collectible card {0}", (object) str);
        return;
      }
      this.RegisterCard(entityDef, str, TAG_PREMIUM.NORMAL);
      entityDef.InitCardSetTimings(timings);
      if (entityDef.GetCardSet() != TAG_CARD_SET.HERO_SKINS || GameUtils.IsVanillaHero(str))
        this.RegisterCard(entityDef, str, TAG_PREMIUM.GOLDEN);
    }
    foreach (CardTagDbfRecord cardTagDbfRecord in all)
    {
      string cardId = GameUtils.TranslateDbIdToCardId(cardTagDbfRecord.CardId);
      if (GameUtils.IsCardCollectible(cardId))
      {
        EntityDef entityDef = defLoader.GetEntityDef(cardId);
        if (entityDef != null)
        {
          TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
          switch ((GAME_TAG) cardTagDbfRecord.TagId)
          {
            case GAME_TAG.HAS_DIAMOND_QUALITY:
              premium = TAG_PREMIUM.DIAMOND;
              break;
            case GAME_TAG.HAS_SIGNATURE_QUALITY:
              premium = TAG_PREMIUM.SIGNATURE;
              break;
            default:
              Debug.LogError((object) "CollectionManager::InitImpl - Unknown card quality level");
              break;
          }
          this.RegisterCard(entityDef, cardId, premium);
        }
      }
    }
    Network network = Network.Get();
    network.RegisterNetHandler((object) GetDeckContentsResponse.PacketID.ID, new Network.NetHandler(this.OnGetDeckContentsResponse));
    network.RegisterNetHandler((object) PegasusUtil.DBAction.PacketID.ID, new Network.NetHandler(this.OnDBAction));
    network.RegisterNetHandler((object) DeckCreated.PacketID.ID, new Network.NetHandler(this.OnDeckCreatedNetworkResponse));
    network.RegisterNetHandler((object) DeckDeleted.PacketID.ID, new Network.NetHandler(this.OnDeckDeleted));
    network.RegisterNetHandler((object) DeckRenamed.PacketID.ID, new Network.NetHandler(this.OnDeckRenamed));
    network.RegisterNetHandler((object) SmartDeckResponse.PacketID.ID, new Network.NetHandler(this.OnSmartDeckResponse));
    network.AddBnetErrorListener(BnetFeature.Games, new Network.BnetErrorCallback(this.OnBnetError));
    if (HearthstoneApplication.IsInternal())
    {
      CheatMgr.Get().RegisterCategory("collection");
      CheatMgr.Get().RegisterCheatHandler("deckadd", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_AddDecks));
      CheatMgr.Get().RegisterCheatHandler("deckreplace", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_ReplaceDecks));
      CheatMgr.Get().RegisterCheatHandler("deckremoveall", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_RemoveDecks));
      CollectionManager.Get().RegisterDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreatedFromDeckcodeCheat));
    }
    this.BattlegroundsDataInit();
    this.LettuceInitImpl();
    NetCache.Get().RegisterCollectionManager(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    LoginManager.Get().OnAchievesLoaded += new System.Action(this.OnAchievesLoaded);
  }

  private void WillReset()
  {
    this.m_achievesLoaded = false;
    this.m_netCacheLoaded = false;
    this.m_collectionLoaded = false;
    this.m_duelsSessionInfoLoaded = false;
    HearthstoneApplication.Get().WillReset -= new System.Action(CollectionManager.s_instance.WillReset);
    NetCache.Get().FavoriteBattlegroundsGuideSkinChanged -= new NetCache.DelFavoriteBattlegroundsGuideSkinChangedListener(CollectionManager.s_instance.OnFavoriteBattlegroundsGuideSkinChanged);
    NetCache.Get().RemoveUpdatedListener(typeof (NetCache.NetCacheDecks), new System.Action(CollectionManager.s_instance.NetCache_OnDecksReceived));
    if (HearthstoneApplication.IsInternal())
      CollectionManager.Get().RemoveDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreatedFromDeckcodeCheat));
    this.m_decks.Clear();
    this.m_baseDecks.Clear();
    this.m_preconDecks.Clear();
    this.m_favoriteHeroChangedListeners.Clear();
    this.m_templateDecks.Clear();
    this.m_templateDeckMap.Clear();
    this.m_displayableCardSets.Clear();
    this.m_onUIHeroOverrideCardRemovedListeners.Clear();
    this.m_collectibleCards = new List<CollectibleCard>();
    this.m_collectibleCardIndex = new Map<CollectionManager.CollectibleCardIndex, CollectibleCard>();
    this.m_collectionLastModifiedTime = 0.0f;
    this.m_lastSearchForWildCardsTime = 0.0f;
    this.m_EditedDeck = (CollectionDeck) null;
    this.LettuceReset();
    CollectionManager.s_instance = (CollectionManager) null;
  }

  private void OnCollectionChanged()
  {
    foreach (CollectionManager.DelOnCollectionChanged collectionChanged in this.m_collectionChangedListeners.ToArray())
      collectionChanged();
  }

  public int NumCardsOwnedInSet(TAG_CARD_SET cardSet)
  {
    int? manaCost = new int?();
    int? nullable = new int?(1);
    TAG_CARD_SET[] theseCardSets = new TAG_CARD_SET[1]
    {
      cardSet
    };
    TAG_RARITY? rarity = new TAG_RARITY?();
    TAG_RACE? race = new TAG_RACE?();
    bool? isHero = new bool?();
    int? minOwned = nullable;
    bool? notSeen = new bool?();
    bool? isCraftable = new bool?();
    bool? filterCoreCounterpartCards = new bool?();
    List<CollectibleCard> cards = this.FindCards(manaCost: manaCost, theseCardSets: theseCardSets, rarity: rarity, race: race, isHero: isHero, minOwned: minOwned, notSeen: notSeen, isCraftable: isCraftable, filterCoreCounterpartCards: filterCoreCounterpartCards).m_cards;
    int num = 0;
    foreach (CollectibleCard collectibleCard in cards)
      num += collectibleCard.OwnedCount;
    return num;
  }

  private CollectibleCard RegisterCard(
    EntityDef entityDef,
    string cardID,
    TAG_PREMIUM premium)
  {
    CollectionManager.CollectibleCardIndex key = new CollectionManager.CollectibleCardIndex(cardID, premium);
    CollectibleCard collectibleCard = (CollectibleCard) null;
    if (!this.m_collectibleCardIndex.TryGetValue(key, out collectibleCard))
    {
      collectibleCard = new CollectibleCard(GameUtils.GetCardRecord(cardID), entityDef, premium);
      this.m_collectibleCards.Add(collectibleCard);
      this.m_collectibleCardIndex.Add(key, collectibleCard);
    }
    return collectibleCard;
  }

  private void ClearCardCounts(EntityDef entityDef, string cardID, TAG_PREMIUM premium) => this.RegisterCard(entityDef, cardID, premium).ClearCounts();

  private CollectibleCard SetCounts(NetCache.CardStack netStack, EntityDef entityDef)
  {
    this.ClearCardCounts(entityDef, netStack.Def.Name, netStack.Def.Premium);
    return this.AddCounts(entityDef, netStack.Def.Name, netStack.Def.Premium, new DateTime(netStack.Date), netStack.Count, netStack.NumSeen);
  }

  private CollectibleCard AddCounts(
    EntityDef entityDef,
    string cardID,
    TAG_PREMIUM premium,
    DateTime insertDate,
    int count,
    int numSeen)
  {
    if (entityDef == null)
    {
      Debug.LogError((object) string.Format("CollectionManager.RegisterCardStack(): DefLoader failed to get entity def for {0}", (object) cardID));
      return (CollectibleCard) null;
    }
    this.m_collectionLastModifiedTime = Time.realtimeSinceStartup;
    CollectibleCard collectibleCard = this.RegisterCard(entityDef, cardID, premium);
    if (GameUtils.IsCoreCard(cardID))
    {
      count = Math.Min(collectibleCard.DefaultMaxCopiesPerDeck - collectibleCard.OwnedCount, count);
      numSeen = Math.Min(numSeen, count);
    }
    collectibleCard.AddCounts(count, numSeen, insertDate);
    return collectibleCard;
  }

  private void AddPreconDeckFromNotice(NetCache.ProfileNoticePreconDeck preconDeckNotice)
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(preconDeckNotice.HeroAsset);
    if (entityDef == null)
      return;
    this.AddPreconDeck(entityDef.GetClass(), preconDeckNotice.DeckID);
    NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
    if (netObject == null)
      return;
    NetCache.DeckHeader deckHeader = new NetCache.DeckHeader()
    {
      ID = preconDeckNotice.DeckID,
      Name = "precon",
      Hero = entityDef.GetCardId(),
      HeroPower = GameUtils.GetHeroPowerCardIdFromHero(preconDeckNotice.HeroAsset),
      Type = DeckType.PRECON_DECK,
      SortOrder = preconDeckNotice.DeckID,
      SourceType = DeckSourceType.DECK_SOURCE_TYPE_BASIC_DECK
    };
    netObject.Decks.Add(deckHeader);
    Network.Get().AckNotice(preconDeckNotice.NoticeID);
  }

  private void AddPreconDeck(TAG_CLASS heroClass, long deckID)
  {
    if (this.m_preconDecks.ContainsKey(heroClass))
    {
      Log.CollectionManager.PrintDebug(string.Format("CollectionManager.AddPreconDeck(): Already have a precon deck for class {0}, cannot add deckID {1}", (object) heroClass, (object) deckID));
    }
    else
    {
      Log.CollectionManager.Print(string.Format("CollectionManager.AddPreconDeck() heroClass={0} deckID={1}", (object) heroClass, (object) deckID));
      this.m_preconDecks[heroClass] = new CollectionManager.PreconDeck(deckID);
    }
  }

  private CollectionDeck AddDeck(NetCache.DeckHeader deckHeader) => this.AddDeck(deckHeader, true);

  private CollectionDeck AddDeck(NetCache.DeckHeader deckHeader, bool updateNetCache)
  {
    if (deckHeader.Type != DeckType.NORMAL_DECK && !TavernBrawlManager.IsBrawlDeckType(deckHeader.Type) && deckHeader.Type != DeckType.PVPDR_DECK)
    {
      Debug.LogWarning((object) string.Format("CollectionManager.AddDeck(): deckHeader {0} is not of type NORMAL_DECK, Brawl, or PVPDR deck", (object) deckHeader));
      return (CollectionDeck) null;
    }
    ulong num = (ulong) deckHeader.ID;
    if (deckHeader.CreateDate.HasValue)
      num = TimeUtils.DateTimeToUnixTimeStamp(deckHeader.CreateDate.Value);
    CollectionDeck collectionDeck1 = new CollectionDeck()
    {
      ID = deckHeader.ID,
      Type = deckHeader.Type,
      Name = deckHeader.Name,
      HeroCardID = deckHeader.Hero,
      HeroOverridden = deckHeader.HeroOverridden,
      CardBackID = deckHeader.CardBack,
      SeasonId = deckHeader.SeasonId,
      BrawlLibraryItemId = deckHeader.BrawlLibraryItemId,
      NeedsName = deckHeader.NeedsName,
      SortOrder = deckHeader.SortOrder,
      FormatType = deckHeader.FormatType,
      SourceType = deckHeader.SourceType,
      CreateDate = num,
      Locked = deckHeader.Locked,
      UIHeroOverrideCardID = deckHeader.UIHeroOverride,
      UIHeroOverridePremium = deckHeader.UIHeroOverridePremium,
      RandomHeroUseFavorite = deckHeader.RandomHeroUseFavorite
    };
    collectionDeck1.SetRuneOrder(deckHeader.Rune1, deckHeader.Rune2, deckHeader.Rune3);
    if (collectionDeck1.NeedsName && string.IsNullOrEmpty(collectionDeck1.Name))
    {
      collectionDeck1.Name = GameStrings.Format("GLOBAL_BASIC_DECK_NAME", (object) GameStrings.GetClassName(collectionDeck1.GetClass()));
      Log.CollectionManager.Print(string.Format("Set deck name to {0}", (object) collectionDeck1.Name));
    }
    if ((!this.IsInEditMode() || this.GetEditedDeck() == null ? 0 : (this.GetEditedDeck().ID == collectionDeck1.ID ? 1 : 0)) == 0)
    {
      if (this.m_decks.ContainsKey(deckHeader.ID))
        this.m_decks.Remove(deckHeader.ID);
      this.m_decks.Add(deckHeader.ID, collectionDeck1);
    }
    CollectionDeck collectionDeck2 = new CollectionDeck()
    {
      ID = deckHeader.ID,
      Type = deckHeader.Type,
      Name = deckHeader.Name,
      HeroCardID = deckHeader.Hero,
      HeroOverridden = deckHeader.HeroOverridden,
      CardBackID = deckHeader.CardBack,
      SeasonId = deckHeader.SeasonId,
      BrawlLibraryItemId = deckHeader.BrawlLibraryItemId,
      NeedsName = deckHeader.NeedsName,
      SortOrder = deckHeader.SortOrder,
      FormatType = deckHeader.FormatType,
      SourceType = deckHeader.SourceType,
      UIHeroOverrideCardID = deckHeader.UIHeroOverride,
      UIHeroOverridePremium = deckHeader.UIHeroOverridePremium,
      RandomHeroUseFavorite = deckHeader.RandomHeroUseFavorite
    };
    collectionDeck2.SetRuneOrder(deckHeader.Rune1, deckHeader.Rune2, deckHeader.Rune3);
    if (this.m_baseDecks.ContainsKey(deckHeader.ID))
      this.m_baseDecks.Remove(deckHeader.ID);
    this.m_baseDecks.Add(deckHeader.ID, collectionDeck2);
    if (updateNetCache)
      NetCache.Get().GetNetObject<NetCache.NetCacheDecks>().Decks.Add(deckHeader);
    return collectionDeck1;
  }

  private CollectionDeck RemoveDeck(long id)
  {
    CollectionDeck collectionDeck = (CollectionDeck) null;
    if (this.m_baseDecks.TryGetValue(id, out collectionDeck))
      this.m_baseDecks.Remove(id);
    if (this.m_decks.TryGetValue(id, out collectionDeck))
      this.m_decks.Remove(id);
    NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
    if (netObject == null)
      return collectionDeck;
    for (int index = 0; index < netObject.Decks.Count; ++index)
    {
      if (netObject.Decks[index].ID == id)
      {
        netObject.Decks.RemoveAt(index);
        break;
      }
    }
    return collectionDeck;
  }

  private void LogAllDeckStringsInCollection()
  {
    Log.Decks.PrintInfo("Deck Contents Received:");
    foreach (CollectionDeck collectionDeck in this.GetDecks().Values)
      collectionDeck.LogDeckStringInformation();
  }

  private bool IsDeckNameTaken(string name)
  {
    foreach (CollectionDeck collectionDeck in this.GetDecks().Values)
    {
      if (collectionDeck.Name.Trim().Equals(name, StringComparison.InvariantCultureIgnoreCase))
        return true;
    }
    return false;
  }

  private void FireDeckContentsEvent(long id)
  {
    foreach (CollectionManager.DelOnDeckContents delOnDeckContents in this.m_deckContentsListeners.ToArray())
      delOnDeckContents(id);
  }

  private void FireAllDeckContentsEvent()
  {
    CollectionManager.DelOnAllDeckContents[] array = this.m_allDeckContentsListeners.ToArray();
    this.m_allDeckContentsListeners.Clear();
    foreach (CollectionManager.DelOnAllDeckContents onAllDeckContents in array)
      onAllDeckContents();
  }

  private void OnNetCacheReady()
  {
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    this.m_netCacheLoaded = true;
    Log.CollectionManager.Print("CollectionManager.OnNetCacheReady");
    this.m_displayableCardSets.AddRange(GameDbf.CardSet.GetRecords().Where<CardSetDbfRecord>((Func<CardSetDbfRecord, bool>) (cardSetRecord => cardSetRecord != null && cardSetRecord.IsCollectible && cardSetRecord.ID != 17 && cardSetRecord.ID != 1586 && cardSetRecord.ID != 1705)).Where<CardSetDbfRecord>((Func<CardSetDbfRecord, bool>) (cardSetRecord => SpecialEventManager.Get().IsEventActive(cardSetRecord.SetFilterEvent, false))).Select<CardSetDbfRecord, TAG_CARD_SET>((Func<CardSetDbfRecord, TAG_CARD_SET>) (cardSetRecord => (TAG_CARD_SET) cardSetRecord.ID)));
    this.UpdateShowAdvancedCMOption();
    if (Options.GetFormatType() == PegasusShared.FormatType.FT_WILD && !this.ShouldAccountSeeStandardWild())
    {
      Log.CollectionManager.Print("Options are set to Wild mode, but account shouldn't see Standard/Wild, so setting format type to Standard!");
      Options.SetFormatType(PegasusShared.FormatType.FT_STANDARD);
    }
    NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
    if (netObject != null)
      this.OnNewNotices(netObject.Notices, true);
    NetCache.Get().RegisterNewNoticesListener(new NetCache.DelNewNoticesListener(this.OnNewNotices));
    this.CheckAchievesAndNetCacheLoaded();
  }

  private void OnAchievesLoaded()
  {
    LoginManager.Get().OnAchievesLoaded -= new System.Action(this.OnAchievesLoaded);
    this.m_achievesLoaded = true;
    this.CheckAchievesAndNetCacheLoaded();
  }

  private void CheckAchievesAndNetCacheLoaded()
  {
    if (!this.m_achievesLoaded || !this.m_netCacheLoaded)
      return;
    this.CreateCollectionDecksFromNetCache();
    foreach (CollectionManager.DelOnCollectionLoaded collectionLoaded in this.m_collectionLoadedListeners.ToArray())
      collectionLoaded();
    this.m_collectionLoaded = true;
    if (CollectionManager.OnCollectionManagerReady == null)
      return;
    CollectionManager.OnCollectionManagerReady();
  }

  private void CreateCollectionDecksFromNetCache()
  {
    List<NetCache.DeckHeader> deckHeaderList = new List<NetCache.DeckHeader>();
    NetCache.NetCacheDecks netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDecks>();
    if (netObject != null)
      deckHeaderList = netObject.Decks;
    foreach (NetCache.DeckHeader deckHeader in deckHeaderList)
    {
      switch (deckHeader.Type)
      {
        case DeckType.NORMAL_DECK:
        case DeckType.TAVERN_BRAWL_DECK:
        case DeckType.FSG_BRAWL_DECK:
        case DeckType.PVPDR_DECK:
          this.AddDeck(deckHeader, false);
          continue;
        case DeckType.PRECON_DECK:
          EntityDef entityDef = DefLoader.Get().GetEntityDef(deckHeader.Hero);
          if (entityDef == null)
          {
            Debug.LogErrorFormat("CollectionManager.OnAchievesLoaded: cannot add precon deck because cannot determine class for hero with string cardId={0} (deckId={1})", (object) deckHeader.Hero, (object) deckHeader.ID);
            continue;
          }
          this.AddPreconDeck(entityDef.GetClass(), deckHeader.ID);
          continue;
        default:
          Debug.LogWarning((object) string.Format("CollectionManager.OnAchievesLoaded(): don't know how to handle deck type {0}", (object) deckHeader.Type));
          continue;
      }
    }
    List<PegasusUtil.DeckContents> contentsFromCache = OfflineDataCache.GetLocalDeckContentsFromCache();
    if (contentsFromCache != null)
      this.UpdateFromDeckContents(contentsFromCache);
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheDecks), new System.Action(CollectionManager.s_instance.NetCache_OnDecksReceived));
  }

  private void OnNewNotices(List<NetCache.ProfileNotice> newNotices, bool isInitialNoticeList)
  {
    List<NetCache.ProfileNotice> all = newNotices.FindAll((Predicate<NetCache.ProfileNotice>) (obj => obj.Type == NetCache.ProfileNotice.NoticeType.PRECON_DECK));
    bool flag1 = false;
    foreach (NetCache.ProfileNotice preconDeckNotice in all)
    {
      this.AddPreconDeckFromNotice(preconDeckNotice as NetCache.ProfileNoticePreconDeck);
      flag1 = true;
    }
    bool flag2 = false;
    foreach (NetCache.ProfileNotice profileNotice in newNotices.FindAll((Predicate<NetCache.ProfileNotice>) (obj => obj.Type == NetCache.ProfileNotice.NoticeType.DECK_REMOVED)))
    {
      NetCache.ProfileNoticeDeckRemoved noticeDeckRemoved = profileNotice as NetCache.ProfileNoticeDeckRemoved;
      this.RemoveDeck(noticeDeckRemoved.DeckID);
      Network.Get().AckNotice(noticeDeckRemoved.NoticeID);
      flag2 = true;
    }
    if (!(flag1 | flag2))
      return;
    NetCache.Get().ReloadNetObject<NetCache.NetCacheDecks>();
  }

  private void UpdateShowAdvancedCMOption()
  {
    if (Options.Get().GetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, false))
      return;
    NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
    if (netObject == null)
      return;
    bool flag = netObject.TotalCardsOwned >= 116;
    if (RankMgr.Get().IsNewPlayer())
    {
      if (!this.AccountHasUnlockedWild() && !flag)
        return;
    }
    else if (!flag)
      return;
    Options.Get().SetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, true);
  }

  private void InsertNewCollectionCard(
    string cardID,
    TAG_PREMIUM premium,
    DateTime insertDate,
    int count,
    bool seenBefore)
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardID);
    if (entityDef == null)
    {
      Log.CollectionManager.PrintWarning("Couldn't find entity def for card with card ID {0}", (object) cardID);
    }
    else
    {
      int numSeen = seenBefore ? count : 0;
      this.AddCounts(entityDef, cardID, premium, insertDate, count, numSeen);
      if (entityDef.IsHeroSkin())
      {
        StoreManager.Get().Catalog.UpdateProductStatus();
      }
      else
      {
        foreach (CollectionDeck deck in this.GetDecks(DeckType.NORMAL_DECK))
        {
          if (!deck.IsBeingEdited())
            deck.ReconcileOwnershipOnCollectionCardAdded(cardID);
        }
        CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
        if ((UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null)
        {
          collectionDeckTray.HandleAddedCardDeckUpdate(entityDef, premium, count);
          if ((UnityEngine.Object) collectionDeckTray.m_decksContent != (UnityEngine.Object) null)
            collectionDeckTray.m_decksContent.RefreshMissingCardIndicators();
        }
        this.NotifyNetCacheOfNewCards(new NetCache.CardDefinition()
        {
          Name = cardID,
          Premium = premium
        }, insertDate.Ticks, count, seenBefore);
        this.UpdateShowAdvancedCMOption();
      }
    }
  }

  private void InsertNewCollectionCards(
    List<string> cardIDs,
    List<TAG_PREMIUM> cardPremiums,
    List<DateTime> insertDates,
    List<int> counts,
    bool seenBefore)
  {
    for (int index = 0; index < cardIDs.Count; ++index)
      this.InsertNewCollectionCard(cardIDs[index], cardPremiums[index], insertDates[index], counts[index], seenBefore);
  }

  private void RemoveCollectionCard(string cardID, TAG_PREMIUM premium, int count)
  {
    this.GetCard(cardID, premium).RemoveCounts(count);
    this.m_collectionLastModifiedTime = Time.realtimeSinceStartup;
    foreach (CollectionDeck deck in this.GetDecks(DeckType.NORMAL_DECK))
      deck.ReconcileOwnershipOnCollectionCardRemoved(cardID, premium);
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    if ((UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null)
    {
      collectionDeckTray.HandleDeletedCardDeckUpdate(cardID);
      if ((UnityEngine.Object) collectionDeckTray.m_decksContent != (UnityEngine.Object) null)
        collectionDeckTray.m_decksContent.RefreshMissingCardIndicators();
    }
    this.NotifyNetCacheOfRemovedCards(new NetCache.CardDefinition()
    {
      Name = cardID,
      Premium = premium
    }, count);
  }

  private void UpdateCardCounts(
    NetCache.NetCacheCollection netCacheCards,
    NetCache.CardDefinition cardDef,
    int count,
    int newCount)
  {
    netCacheCards.TotalCardsOwned += count;
    if (cardDef.Premium != TAG_PREMIUM.NORMAL)
      return;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardDef.Name);
    if (!entityDef.IsCoreCard())
      return;
    int num = entityDef.IsElite() ? 1 : 2;
    if (newCount < 0 || newCount > num)
    {
      Debug.LogError((object) ("CollectionManager.UpdateCardCounts: created an illegal stack size of " + (object) newCount + " for card " + (object) entityDef));
      count = 0;
    }
    netCacheCards.CoreCardsUnlockedPerClass[entityDef.GetClass()].Add(entityDef.GetCardId());
  }

  private void NotifyNetCacheOfRemovedCards(NetCache.CardDefinition cardDef, int count)
  {
    NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
    NetCache.CardStack cardStack = netObject.Stacks.Find((Predicate<NetCache.CardStack>) (obj => obj.Def.Name.Equals(cardDef.Name) && obj.Def.Premium == cardDef.Premium));
    if (cardStack == null)
    {
      Debug.LogError((object) "CollectionManager.NotifyNetCacheOfRemovedCards() - trying to remove a card from an empty stack!");
    }
    else
    {
      cardStack.Count -= count;
      if (cardStack.Count <= 0)
        netObject.Stacks.Remove(cardStack);
      this.UpdateCardCounts(netObject, cardDef, -count, cardStack.Count);
    }
  }

  private void NotifyNetCacheOfNewCards(
    NetCache.CardDefinition cardDef,
    long insertDate,
    int count,
    bool seenBefore)
  {
    NetCache.NetCacheCollection netObject = NetCache.Get().GetNetObject<NetCache.NetCacheCollection>();
    if (netObject == null)
      return;
    NetCache.CardStack cardStack = netObject.Stacks.Find((Predicate<NetCache.CardStack>) (obj => obj.Def.Name.Equals(cardDef.Name) && obj.Def.Premium == cardDef.Premium));
    if (cardStack == null)
    {
      cardStack = new NetCache.CardStack()
      {
        Def = cardDef,
        Date = insertDate,
        Count = count,
        NumSeen = seenBefore ? count : 0
      };
      netObject.Stacks.Add(cardStack);
    }
    else
    {
      if (insertDate > cardStack.Date)
        cardStack.Date = insertDate;
      cardStack.Count += count;
      if (seenBefore)
        cardStack.NumSeen += count;
    }
    this.UpdateCardCounts(netObject, cardDef, count, cardStack.Count);
  }

  private void LoadTemplateDecks()
  {
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    foreach (DeckTemplateDbfRecord record1 in GameDbf.DeckTemplate.GetRecords())
    {
      SpecialEventType eventType = record1.Event;
      if (eventType == SpecialEventType.UNKNOWN || SpecialEventManager.Get().IsEventActive(eventType, false))
      {
        int deckId = record1.DeckId;
        if (!this.m_templateDeckMap.ContainsKey(deckId))
        {
          DeckDbfRecord record2 = GameDbf.Deck.GetRecord(deckId);
          if (record2 == null)
          {
            Debug.LogError((object) string.Format("Unable to find deck with ID {0}", (object) deckId));
          }
          else
          {
            Map<string, int> map = new Map<string, int>();
            int nextCard;
            for (DeckCardDbfRecord deckCardDbfRecord = GameDbf.DeckCard.GetRecord(record2.TopCardId); deckCardDbfRecord != null; deckCardDbfRecord = nextCard != 0 ? GameDbf.DeckCard.GetRecord(nextCard) : (DeckCardDbfRecord) null)
            {
              int cardId = deckCardDbfRecord.CardId;
              CardDbfRecord record3 = GameDbf.Card.GetRecord(cardId);
              if (record3 != null)
              {
                string noteMiniGuid = record3.NoteMiniGuid;
                if (map.ContainsKey(noteMiniGuid))
                  ++map[noteMiniGuid];
                else
                  map[noteMiniGuid] = 1;
              }
              else
                Debug.LogError((object) string.Format("Card ID in deck not found in CARD.XML: {0}", (object) cardId));
              nextCard = deckCardDbfRecord.NextCard;
            }
            TAG_CLASS classId = (TAG_CLASS) record1.ClassId;
            List<CollectionManager.TemplateDeck> templateDeckList = (List<CollectionManager.TemplateDeck>) null;
            if (!this.m_templateDecks.TryGetValue(classId, out templateDeckList))
            {
              templateDeckList = new List<CollectionManager.TemplateDeck>();
              this.m_templateDecks.Add(classId, templateDeckList);
            }
            CollectionManager.TemplateDeck templateDeck = new CollectionManager.TemplateDeck()
            {
              m_id = deckId,
              m_deckTemplateId = record1.ID,
              m_class = classId,
              m_sortOrder = record1.SortOrder,
              m_cardIds = map,
              m_title = (string) record2.Name,
              m_description = (string) record2.Description,
              m_displayTexture = record1.DisplayTexture,
              m_event = record1.Event,
              m_isStarterDeck = record1.IsStarterDeck,
              m_formatType = (PegasusShared.FormatType) record1.FormatType
            };
            if (record1.DKRunes != null)
            {
              if (record1.DKRunes.Count >= 1)
                templateDeck.m_rune1 = (RuneType) record1.DKRunes[0].Rune;
              if (record1.DKRunes.Count >= 2)
                templateDeck.m_rune2 = (RuneType) record1.DKRunes[1].Rune;
              if (record1.DKRunes.Count >= 3)
                templateDeck.m_rune3 = (RuneType) record1.DKRunes[2].Rune;
            }
            templateDeckList.Add(templateDeck);
            this.m_templateDeckMap.Add(templateDeck.m_id, templateDeck);
          }
        }
      }
    }
    foreach (KeyValuePair<TAG_CLASS, List<CollectionManager.TemplateDeck>> templateDeck in this.m_templateDecks)
      templateDeck.Value.Sort((Comparison<CollectionManager.TemplateDeck>) ((a, b) =>
      {
        int num = a.m_sortOrder.CompareTo(b.m_sortOrder);
        if (num == 0)
          num = a.m_id.CompareTo(b.m_id);
        return num;
      }));
    Log.CollectionManager.Print("_decktemplate: Time spent loading template decks: " + (object) (float) ((double) Time.realtimeSinceStartup - (double) realtimeSinceStartup));
  }

  public TAG_PREMIUM GetPreferredPremium() => this.m_premiumPreference;

  public void SetPremiumPreference(TAG_PREMIUM premium)
  {
    this.m_premiumPreference = premium;
    this.RefreshCurrentPageContents();
  }

  public void RefreshCurrentPageContents()
  {
    if (!((UnityEngine.Object) this.m_collectibleDisplay != (UnityEngine.Object) null))
      return;
    this.m_collectibleDisplay.GetPageManager().RefreshCurrentPageContents();
  }

  public void RegisterDecksToRequestContentsAfterDeckSetDataResponse(List<long> decksToRequest)
  {
    foreach (long num in decksToRequest)
    {
      if (!this.m_decksToRequestContentsAfterDeckSetDataResonse.Contains(num))
        this.m_decksToRequestContentsAfterDeckSetDataResonse.Add(num);
    }
  }

  public static void ShowFeatureDisabledWhileOfflinePopup()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_OFFLINE_FEATURE_DISABLED_HEADER"),
      m_text = GameStrings.Get("GLUE_OFFLINE_FEATURE_DISABLED_BODY"),
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_showAlertIcon = false
    };
    DialogManager.Get().ShowPopup(info);
  }

  public void SetTimeOfLastPlayerDeckSave(DateTime? time) => this.m_timeOfLastPlayerDeckSave = time;

  public static List<int> GetFeaturedCards() => GameDbf.GetIndex().GetCardsWithFeaturedCardsEvent().Where<CardDbfRecord>((Func<CardDbfRecord, bool>) (c => SpecialEventManager.Get().IsEventActive(c.FeaturedCardsEvent, false))).Select<CardDbfRecord, int>((Func<CardDbfRecord, int>) (c => c.ID)).ToList<int>();

  private bool OnProcessCheat_AddDecks(string func, string[] args, string rawArgs)
  {
    if (args.Length != 0)
    {
      foreach (string pastedString in args)
      {
        ShareableDeck deck = ShareableDeck.Deserialize(pastedString);
        if (deck != null)
          this.AddDeckFromShareableDeck(deck);
      }
    }
    else
    {
      string message = "USAGE: deckadd <whitespace separated list of deckcodes>";
      UIStatus.Get().AddInfo(message, 5f);
    }
    return true;
  }

  private bool OnProcessCheat_ReplaceDecks(string func, string[] args, string rawArgs)
  {
    this.OnProcessCheat_RemoveDecks("deckremoveall", args, rawArgs);
    this.OnProcessCheat_AddDecks("deckadd", args, rawArgs);
    return true;
  }

  private bool OnProcessCheat_RemoveDecks(string func, string[] args, string rawArgs)
  {
    foreach (KeyValuePair<long, CollectionDeck> deck in this.m_decks)
      Network.Get().DeleteDeck(deck.Key, deck.Value.Type);
    return true;
  }

  private void AddDeckFromShareableDeck(ShareableDeck deck)
  {
    string str = deck.Serialize(false);
    if (!this.m_decksToCheatIn.ContainsKey(str))
      this.m_decksToCheatIn.Add(str, deck);
    int? requestId;
    Network.Get().CreateDeck(DeckType.NORMAL_DECK, str, deck.HeroCardDbId, deck.FormatType, 0L, DeckSourceType.DECK_SOURCE_TYPE_PASTED_DECK, out requestId, str);
    if (!requestId.HasValue)
      return;
    this.m_inTransitDeckCreateRequests.Add(requestId.Value);
  }

  private void OnDeckCreatedFromDeckcodeCheat(long deckId, string name)
  {
    ShareableDeck shareableDeck;
    if (!this.m_decksToCheatIn.TryGetValue(name, out shareableDeck))
      return;
    List<Network.CardUserData> cards = new List<Network.CardUserData>();
    foreach (DeckCardData card in shareableDeck.DeckContents.Cards)
      cards.Add(new Network.CardUserData()
      {
        DbId = card.Def.Asset,
        Count = card.Qty,
        Premium = (TAG_PREMIUM) card.Def.Premium
      });
    Network.Get().SendDeckData(CollectionDeck.ChangeSource.Cheat, 0, deckId, cards, -1, new bool?(false), -1, TAG_PREMIUM.NORMAL, new int?(-1), shareableDeck.FormatType, 0L, new bool?(true), (RuneType[]) null, name);
    string cardId = GameUtils.TranslateDbIdToCardId(shareableDeck.HeroCardDbId);
    string className = GameStrings.GetClassName(DefLoader.Get().GetEntityDef(cardId).GetClass());
    Network.Get().RenameDeck(deckId, "Custom " + className);
    CollectionDeck collectionDeck;
    if (this.m_decks.TryGetValue(deckId, out collectionDeck))
      collectionDeck.FillFromShareableDeck(shareableDeck);
    this.m_decksToCheatIn.Remove(name);
  }

  public event System.Action OnLettuceLoaded;

  public event System.Action OnMercenariesTrainingAddResponseReceived;

  public event System.Action OnMercenariesTrainingRemoveResponseReceived;

  public event System.Action OnMercenariesTrainingCollectResponseReceived;

  public event System.Action<int, int, TAG_PREMIUM> MercenaryArtVariationChangedEvent;

  public CollectionManager.FindMercenariesResult FindMercenaries(
    string searchString = null,
    bool? isOwned = null,
    bool? isUpgradeable = null,
    bool? isCraftable = null,
    bool? excludeCraftableFromOwned = null,
    bool ordered = true)
  {
    CollectionManager.FindMercenariesResult mercenaries = new CollectionManager.FindMercenariesResult();
    List<CollectionManager.MercenaryFilterFunc> filterFuncs = new List<CollectionManager.MercenaryFilterFunc>();
    CollectibleCardRoleFilter.SearchTerms setSearchTerms = new CollectibleCardRoleFilter.SearchTerms();
    if (!string.IsNullOrEmpty(searchString))
      filterFuncs.AddRange((IEnumerable<CollectionManager.MercenaryFilterFunc>) CollectibleCardRoleFilter.FilterMercsFromSearchString(searchString, ref setSearchTerms));
    bool flag = setSearchTerms.Owned || isOwned.HasValue && isOwned.Value;
    int num = setSearchTerms.Missing ? 1 : (!isOwned.HasValue ? 0 : (!isOwned.Value ? 1 : 0));
    bool dontIncludeCraftableWithOwned = flag && excludeCraftableFromOwned.HasValue && excludeCraftableFromOwned.Value;
    if (flag)
      filterFuncs.Add((CollectionManager.MercenaryFilterFunc) (merc =>
      {
        if (merc.m_owned)
          return true;
        return merc.IsReadyForCrafting() && !dontIncludeCraftableWithOwned;
      }));
    if (num != 0)
      filterFuncs.Add((CollectionManager.MercenaryFilterFunc) (merc => !merc.m_owned));
    if (isUpgradeable.HasValue)
      filterFuncs.Add((CollectionManager.MercenaryFilterFunc) (merc => merc.CanAnyCardBeUpgraded() == isUpgradeable.Value));
    if (isCraftable.HasValue)
      filterFuncs.Add((CollectionManager.MercenaryFilterFunc) (merc => !merc.m_owned && merc.IsReadyForCrafting() == isCraftable.Value));
    Predicate<LettuceMercenary> match = (Predicate<LettuceMercenary>) (merc =>
    {
      if (merc == null)
        return false;
      for (int index = 0; index < filterFuncs.Count; ++index)
      {
        if (!filterFuncs[index](merc))
          return false;
      }
      return true;
    });
    mercenaries.m_mercenaries = this.m_collectibleMercenaries.FindAll(match);
    if (ordered)
      mercenaries.m_mercenaries.Sort(CollectionManager.OrderMercernaries);
    return mercenaries;
  }

  public void StartInitialMercenaryLoadIfRequired()
  {
    if (this.m_initialDataRequested)
      return;
    foreach (LettuceMercenaryDbfRecord record in GameDbf.LettuceMercenary.GetRecords())
    {
      if (record.Collectible)
        this.RegisterMercenary(record.ID);
    }
    this.m_initialDataRequested = true;
    Network.Get().MercenariesPlayerInfoRequest();
    Network.Get().MercenariesCollectionRequest();
    Network.Get().MercenariesTeamListRequest();
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if (!((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null))
      return;
    hearthstoneApplication.OnShutdown += new System.Action(this.OnShutdown);
    hearthstoneApplication.Paused += new System.Action(this.OnPause);
  }

  private void OnPause()
  {
    if (!Application.isMobilePlatform)
      return;
    Network.Get().MercenariesCollectionRequest();
  }

  private void OnShutdown() => Network.Get().MercenariesCollectionRequest();

  public bool IsLettuceLoaded() => this.m_mercsAndTeamsReceived && this.m_playerInfoReceived;

  public bool GetHasOpenedDetailsDisplay() => this.m_hasVisitedDetailsDisplay;

  public void SetHasVisitedDetailsDisplayTrue() => this.m_hasVisitedDetailsDisplay = true;

  public void RegisterTeamCreatedListener(CollectionManager.DelOnTeamCreated listener)
  {
    if (this.m_teamCreatedListeners.Contains(listener))
      return;
    this.m_teamCreatedListeners.Add(listener);
  }

  public bool RemoveTeamCreatedListener(CollectionManager.DelOnTeamCreated listener) => this.m_teamCreatedListeners.Remove(listener);

  public void RegisterTeamDeletedListener(CollectionManager.DelOnTeamDeleted listener)
  {
    if (this.m_teamDeletedListeners.Contains(listener))
      return;
    this.m_teamDeletedListeners.Add(listener);
  }

  public bool RemoveTeamDeletedListener(CollectionManager.DelOnTeamDeleted listener) => this.m_teamDeletedListeners.Remove(listener);

  public void RegisterTeamContentsListener(CollectionManager.DelOnTeamContents listener)
  {
    if (this.m_teamContentsListeners.Contains(listener))
      return;
    this.m_teamContentsListeners.Add(listener);
  }

  public bool RemoveTeamContentsListener(CollectionManager.DelOnTeamContents listener) => this.m_teamContentsListeners.Remove(listener);

  public void RegisterEditingTeamChanged(CollectionManager.OnEditingTeamChanged listener)
  {
    if (this.m_editingTeamChangedListeners.Contains(listener))
      return;
    this.m_editingTeamChangedListeners.Add(listener);
  }

  public void RemoveEditingTeamChanged(CollectionManager.OnEditingTeamChanged listener) => this.m_editingTeamChangedListeners.Remove(listener);

  public void TriggerNewCardSeenListeners(string id = "", TAG_PREMIUM premium = TAG_PREMIUM.NORMAL)
  {
    foreach (CollectionManager.DelOnNewCardSeen cardSeenListener in this.m_newCardSeenListeners)
      cardSeenListener(id, premium);
  }

  private void ProcessMercInitDataAfterEverythingReceived()
  {
    if (this.m_mercTeamListResponse == null || this.m_mercenariesCollectionResponse == null || !this.m_playerInfoReceived)
      return;
    this.ProcessCollectibleMercenariesResponse(this.m_mercenariesCollectionResponse);
    this.ProcessMercenariesTeamListResponse(this.m_mercTeamListResponse);
    this.m_mercenariesCollectionResponse = (MercenariesCollectionResponse) null;
    this.m_mercTeamListResponse = (LettuceTeamList) null;
    this.m_mercsAndTeamsReceived = true;
    System.Action onLettuceLoaded = this.OnLettuceLoaded;
    if (onLettuceLoaded != null)
      onLettuceLoaded();
    this.OnLettuceLoaded = (System.Action) null;
  }

  private void OnMercenariesCollectionResponse()
  {
    if (this.IsLettuceLoaded())
    {
      this.ProcessCollectibleMercenariesResponse(Network.Get().MercenariesCollectionResponse());
    }
    else
    {
      this.m_mercenariesCollectionResponse = Network.Get().MercenariesCollectionResponse();
      this.ProcessMercInitDataAfterEverythingReceived();
    }
  }

  private void ProcessCollectibleMercenariesResponse(MercenariesCollectionResponse response)
  {
    if (response == null)
      Log.CollectionManager.PrintError("OnMercenariesCollectionResponse(): No response received.");
    else if (!response.HasMercenaryList || response.MercenaryList == null)
    {
      Log.CollectionManager.PrintError("OnMercenariesCollectionResponse(): No mercenary list received.");
    }
    else
    {
      foreach (MercenaryDetailed mercenary in response.MercenaryList.Mercenaries)
      {
        if (mercenary.Mercenary.HasAssetId)
        {
          LettuceMercenary collectibleMercenary = this.RegisterMercenary(mercenary.Mercenary.AssetId);
          if (collectibleMercenary == null)
            Log.CollectionManager.PrintError("OnMercenariesCollectionResponse(): Invalid mercenary with DB ID [{0}].", (object) mercenary.Mercenary.AssetId);
          else
            this.UpdateCollectibleMercenary(ref collectibleMercenary, mercenary);
        }
      }
    }
  }

  private void OnMercenariesCollectionUpdate()
  {
    MercenariesCollectionUpdate collectionUpdate = Network.Get().MercenariesCollectionUpdate();
    if (collectionUpdate == null)
    {
      Log.CollectionManager.PrintError("OnMercenariesCollectionUpdate(): No response received.");
    }
    else
    {
      if (!this.IsLettuceLoaded())
        return;
      if (!collectionUpdate.HasMercenaryList)
      {
        Log.CollectionManager.PrintError("OnMercenariesCollectionUpdate(): No mercenary list received.");
      }
      else
      {
        foreach (MercenaryDetailed mercenary1 in collectionUpdate.MercenaryList.Mercenaries)
        {
          if (mercenary1.Mercenary.HasAssetId)
          {
            LettuceMercenary mercenary2 = this.GetMercenary((long) mercenary1.Mercenary.AssetId);
            if (mercenary2 == null)
              Log.CollectionManager.PrintError("OnMercenariesCollectionResponse(): Invalid mercenary with DB ID [{0}].", (object) mercenary1.Mercenary.AssetId);
            else
              this.UpdateCollectibleMercenary(ref mercenary2, mercenary1);
          }
        }
        GameSaveDataManager.Get().ApplyGameSaveDataUpdate(collectionUpdate.GameSaveData);
        LettuceVillageDataUtil.RefreshData();
      }
    }
  }

  private void UpdateCollectibleMercenary(
    ref LettuceMercenary collectibleMercenary,
    MercenaryDetailed mercenary)
  {
    if (mercenary.Mercenary.HasAcquired)
      collectibleMercenary.m_owned = mercenary.Mercenary.Acquired;
    if (mercenary.HasIsFullyUpgraded)
      collectibleMercenary.m_isFullyUpgraded = mercenary.IsFullyUpgraded;
    if (mercenary.HasArtVariationList && mercenary.ArtVariationList.ArtVariations.Count > 0)
    {
      collectibleMercenary.m_artVariations.Clear();
      foreach (MercenaryArtVariation artVariation in mercenary.ArtVariationList.ArtVariations)
      {
        MercenaryArtVariationDbfRecord record = GameDbf.MercenaryArtVariation.GetRecord(artVariation.AssetId);
        if (record == null)
        {
          Log.CollectionManager.PrintError("UpdateCollectibleMercenary: no record for art variation!");
        }
        else
        {
          collectibleMercenary.m_artVariations.Add(new LettuceMercenary.ArtVariation(record, (TAG_PREMIUM) artVariation.Premium, record.DefaultVariation, artVariation.AcquireAcknowledged));
          if (artVariation.Equipped)
            collectibleMercenary.GetBaseLoadout().SetArtVariation(record, (TAG_PREMIUM) artVariation.Premium);
        }
      }
    }
    if (mercenary.Mercenary.HasExp)
      collectibleMercenary.SetExperience(mercenary.Mercenary.Exp);
    if (mercenary.Mercenary.HasCurrencyAmount)
      collectibleMercenary.m_currencyAmount = mercenary.Mercenary.CurrencyAmount;
    if (mercenary.HasAbilityList)
    {
      foreach (MercenaryAbility ability1 in mercenary.AbilityList.Abilities)
      {
        if (!ability1.HasAssetId)
        {
          Log.CollectionManager.PrintError("OnMercenariesCollectionResponse(): No ability ID.");
        }
        else
        {
          int abilityIndex = collectibleMercenary.GetAbilityIndex(ability1.AssetId);
          if (abilityIndex == -1)
          {
            Log.CollectionManager.PrintError(string.Format("OnMercenariesCollectionResponse(): Ability ID [{0}] not found in collectible mercenary [{1}:{2}].", (object) ability1.AssetId, (object) collectibleMercenary.m_mercName, (object) collectibleMercenary.ID));
          }
          else
          {
            LettuceAbility ability2 = collectibleMercenary.m_abilityList[abilityIndex];
            if (ability1.HasTier)
              ability2.m_tier = (int) ability1.Tier;
            ability2.m_acquireAcknowledged = ability1.AcquireAcknowledged;
            ability2.m_upgradeAcknowledged = ability1.UpgradeAcknowledged;
          }
        }
      }
    }
    if (mercenary.HasEquipmentList)
    {
      foreach (MercenaryEquipment mercenaryEquipment in mercenary.EquipmentList.Equipment)
      {
        if (!mercenaryEquipment.HasAssetId)
        {
          Log.CollectionManager.PrintError("UpdateCollectibleMercenary: No asset Id on Equipment!");
        }
        else
        {
          int equipmentIndex = collectibleMercenary.GetEquipmentIndex(mercenaryEquipment.AssetId);
          if (equipmentIndex == -1)
          {
            Log.CollectionManager.PrintError("OnMercenariesCollectionResponse(): Equipment ID [{0}] not found in collectible mercenary [{1}].", (object) mercenaryEquipment.AssetId, (object) collectibleMercenary.ID);
          }
          else
          {
            LettuceAbility equipment = collectibleMercenary.m_equipmentList[equipmentIndex];
            equipment.Owned = true;
            equipment.m_acquireAcknowledged = mercenaryEquipment.AcquireAcknowledged;
            equipment.m_upgradeAcknowledged = mercenaryEquipment.UpgradeAcknowledged;
            LettuceEquipmentDbfRecord record = GameDbf.LettuceEquipment.GetRecord(mercenaryEquipment.AssetId);
            if (mercenaryEquipment.HasTier)
              equipment.m_tier = (int) mercenaryEquipment.Tier;
            if (mercenaryEquipment.Equipped)
              collectibleMercenary.GetBaseLoadout().SetSlottedEquipment(record);
          }
        }
      }
    }
    if (mercenary.Mercenary.HasTrainingStartDate)
      collectibleMercenary.m_trainingStartDate = mercenary.Mercenary.TrainingStartDate;
    else
      collectibleMercenary.m_trainingStartDate = (Date) null;
  }

  private void OnMercenariesTeamUpdate()
  {
    MercenariesTeamUpdate mercenariesTeamUpdate = Network.Get().MercenariesTeamUpdate();
    if (mercenariesTeamUpdate == null)
      Log.CollectionManager.PrintError("OnMercenariesTeamUpdate(): No response received.");
    else if (!mercenariesTeamUpdate.HasTeam || mercenariesTeamUpdate.Team == null)
      Log.CollectionManager.PrintError("OnMercenariesTeamUpdate(): No mercenary team received.");
    else
      this.UpdateTeam(mercenariesTeamUpdate.Team);
  }

  private void OnMercenariesTrainingAddResponse()
  {
    MercenariesTrainingAddResponse trainingAddResponse = Network.Get().MercenariesTrainingAddResponse();
    if (trainingAddResponse == null)
    {
      Log.CollectionManager.PrintError("OnMercenariesTrainingAddResponse(): No response received.");
    }
    else
    {
      LettuceMercenary mercenary = this.GetMercenary((long) trainingAddResponse.MercenaryId);
      if (mercenary == null)
      {
        Log.CollectionManager.PrintError(string.Format("{0}(): Could not find mercenary instance {1}", (object) nameof (OnMercenariesTrainingAddResponse), (object) trainingAddResponse.MercenaryId));
      }
      else
      {
        mercenary.m_trainingStartDate = trainingAddResponse.TrainingStartDate;
        System.Action responseReceived = this.OnMercenariesTrainingAddResponseReceived;
        if (responseReceived == null)
          return;
        responseReceived();
      }
    }
  }

  private void OnMercenariesTrainingRemoveResponse()
  {
    MercenariesTrainingRemoveResponse trainingRemoveResponse = Network.Get().MercenariesTrainingRemoveResponse();
    if (trainingRemoveResponse == null)
    {
      Log.CollectionManager.PrintError("OnMercenariesTrainingRemoveResponse(): No response received.");
    }
    else
    {
      LettuceMercenary mercenary = this.GetMercenary((long) trainingRemoveResponse.MercenaryId);
      if (mercenary == null)
      {
        Log.CollectionManager.PrintError(string.Format("{0}(): Could not find mercenary instance {1}", (object) nameof (OnMercenariesTrainingRemoveResponse), (object) trainingRemoveResponse.MercenaryId));
      }
      else
      {
        mercenary.m_trainingStartDate = (Date) null;
        System.Action responseReceived = this.OnMercenariesTrainingRemoveResponseReceived;
        if (responseReceived == null)
          return;
        responseReceived();
      }
    }
  }

  private void OnMercenariesTrainingCollectResponse()
  {
    MercenariesTrainingCollectResponse trainingCollectResponse = Network.Get().MercenariesTrainingCollectResponse();
    if (trainingCollectResponse == null)
    {
      Log.CollectionManager.PrintError("OnMercenariesTrainingCollectResponse(): No response received.");
    }
    else
    {
      LettuceMercenary mercenary = this.GetMercenary((long) trainingCollectResponse.MercenaryId);
      if (mercenary == null)
      {
        Log.CollectionManager.PrintError(string.Format("{0}(): Could not find mercenary instance {1}", (object) nameof (OnMercenariesTrainingCollectResponse), (object) trainingCollectResponse.MercenaryId));
      }
      else
      {
        mercenary.m_trainingStartDate = trainingCollectResponse.NewTrainingStartDate;
        System.Action responseReceived = this.OnMercenariesTrainingCollectResponseReceived;
        if (responseReceived == null)
          return;
        responseReceived();
      }
    }
  }

  private void OnMercenariesCurrencyUpdate()
  {
    Network network = Network.Get();
    if (network == null)
    {
      Log.CollectionManager.PrintError("OnMercenariesCurrencyUpdate(): Network connection does not exist. Likely this handler has not been cleaned up on desruction.");
    }
    else
    {
      MercenariesCurrencyUpdate mercenariesCurrencyUpdate = network.MercenariesCurrencyUpdate();
      if (mercenariesCurrencyUpdate == null)
      {
        Log.CollectionManager.PrintError("OnMercenariesCurrencyUpdate(): No response received.");
      }
      else
      {
        if (!this.IsLettuceLoaded())
          return;
        if (!mercenariesCurrencyUpdate.HasMercenaryId)
          Log.CollectionManager.PrintError("OnMercenariesCurrencyUpdate(): No mercenary ID received.");
        else if (!mercenariesCurrencyUpdate.HasPostCurrency)
        {
          Log.CollectionManager.PrintError("OnMercenariesCurrencyUpdate(): No post currency received.");
        }
        else
        {
          LettuceMercenary mercenary = this.GetMercenary((long) mercenariesCurrencyUpdate.MercenaryId);
          if (mercenary == null)
          {
            Log.CollectionManager.PrintError("OnMercenariesCurrencyUpdate(): Invalid mercenary with DB ID [{0}].", (object) mercenariesCurrencyUpdate.MercenaryId);
          }
          else
          {
            if (mercenary.m_currencyAmount == mercenariesCurrencyUpdate.PostCurrency)
              return;
            mercenary.m_currencyAmount = mercenariesCurrencyUpdate.PostCurrency;
            bool flag = mercenary.CanAnyCardBeUpgraded();
            LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get()?.GetCollectibleDisplay() as LettuceCollectionDisplay;
            if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null)
            {
              LettuceMercenaryDataModel displayDataModel = collectibleDisplay.GetMercenaryDetailsDisplay().GetMercenaryDisplayDataModel();
              if (displayDataModel != null)
              {
                if (displayDataModel.MercenaryCoin != null)
                  displayDataModel.MercenaryCoin.Quantity = (int) mercenary.m_currencyAmount;
                CollectionUtils.UpdateReadyForUpgradeStatus(displayDataModel, mercenary);
              }
              LettuceCollectionPageManager pageManager = collectibleDisplay.GetPageManager() as LettuceCollectionPageManager;
              if ((UnityEngine.Object) pageManager == (UnityEngine.Object) null)
              {
                Log.Lettuce.PrintWarning("MercenaryDetailDisplay.UpdateDataModelsAfterTransaction - Unable to retrieve LettuceCollectionPageManager!");
              }
              else
              {
                LettuceMercenaryDataModel mercenaryOnPage = pageManager.GetMercenaryOnPage(mercenariesCurrencyUpdate.MercenaryId);
                if (mercenaryOnPage != null && mercenaryOnPage.MercenaryCoin != null)
                {
                  mercenaryOnPage.ChildUpgradeAvailable = flag;
                  mercenaryOnPage.MercenaryCoin.Quantity = (int) mercenary.m_currencyAmount;
                }
              }
            }
            LettuceMercenaryDataModel mercenaryDataModel = CollectionDeckTray.Get()?.GetMercsContent()?.GetMercenaryDataModel(mercenary.ID);
            if (mercenaryDataModel == null)
              return;
            mercenaryDataModel.ChildUpgradeAvailable = flag;
          }
        }
      }
    }
  }

  private void OnMercenariesExperienceUpdate()
  {
    MercenariesExperienceUpdate experienceUpdate = Network.Get().MercenariesExperienceUpdate();
    if (experienceUpdate == null)
      Log.CollectionManager.PrintError("OnMercenariesExperienceUpdate(): No response received.");
    else if (!experienceUpdate.HasMercenaryId)
      Log.CollectionManager.PrintError("OnMercenariesExperienceUpdate(): No mercenary ID received.");
    else if (!experienceUpdate.HasExpDelta)
    {
      Log.CollectionManager.PrintError("OnMercenariesExperienceUpdate(): No experience delta received.");
    }
    else
    {
      LettuceMercenary mercenary = this.GetMercenary((long) experienceUpdate.MercenaryId);
      if (mercenary == null)
        Log.CollectionManager.PrintError("OnMercenariesExperienceUpdate(): Invalid mercenary with DB ID [{0}].", (object) experienceUpdate.MercenaryId);
      else
        mercenary.SetExperience(mercenary.m_experience + experienceUpdate.ExpDelta);
    }
  }

  private void OnMercenariesRewardUpdate()
  {
    MercenariesRewardUpdate mercenariesRewardUpdate = Network.Get().MercenariesRewardUpdate();
    if (mercenariesRewardUpdate == null)
    {
      Log.CollectionManager.PrintError("OnMercenariesRewardUpdate(): No response received.");
    }
    else
    {
      foreach (MercenariesExperienceUpdate experienceUpdate in mercenariesRewardUpdate.ExperienceUpdates)
      {
        if (!experienceUpdate.HasMercenaryId)
          Log.CollectionManager.PrintError("OnMercenariesRewardUpdate(): No mercenary ID received.");
        else if (!experienceUpdate.HasExpDelta)
        {
          Log.CollectionManager.PrintError("OnMercenariesRewardUpdate(): No experience delta received.");
        }
        else
        {
          LettuceMercenary mercenary = this.GetMercenary((long) experienceUpdate.MercenaryId);
          if (mercenary == null)
          {
            Log.CollectionManager.PrintError("OnMercenariesRewardUpdate(): Invalid mercenary with DB ID [{0}].", (object) experienceUpdate.MercenaryId);
          }
          else
          {
            mercenary.SetExperience(mercenary.m_experience + experienceUpdate.ExpDelta);
            Log.All.Print("EXP UPDATE - mercenary[" + (object) mercenary.ID + "] amount=" + (object) experienceUpdate.ExpDelta + "]");
          }
        }
      }
      foreach (MercenariesEquipmentUpdate equipmentUpdate in mercenariesRewardUpdate.EquipmentUpdates)
      {
        if (!equipmentUpdate.HasMercenaryId)
          Log.CollectionManager.PrintError("OnMercenariesRewardUpdate(): No mercenary ID received.");
        else if (!equipmentUpdate.HasEquipmentId)
          Log.CollectionManager.PrintError("OnMercenariesRewardUpdate(): No equipment ID received.");
        else if (!equipmentUpdate.HasTier)
        {
          Log.CollectionManager.PrintError("OnMercenariesRewardUpdate(): No tier value received.");
        }
        else
        {
          LettuceMercenary mercenary = this.GetMercenary((long) equipmentUpdate.MercenaryId);
          if (mercenary == null)
          {
            Log.CollectionManager.PrintError("OnMercenariesRewardUpdate(): Invalid mercenary with DB ID [{0}].", (object) equipmentUpdate.MercenaryId);
          }
          else
          {
            LettuceAbility lettuceEquipment = mercenary.GetLettuceEquipment(equipmentUpdate.EquipmentId);
            if (lettuceEquipment == null)
              Log.CollectionManager.PrintError("OnMercenariesRewardUpdate(): Invalid equipment with DB ID [{0}] on mercenary with DB ID [{1}].", (object) equipmentUpdate.EquipmentId, (object) equipmentUpdate.MercenaryId);
            else if (equipmentUpdate.HasCurrencyDelta)
            {
              mercenary.m_currencyAmount += equipmentUpdate.CurrencyDelta;
              Log.All.Print("EQUIPMENT UPDATE - mercenary[" + (object) mercenary.ID + "] equipment[" + (object) lettuceEquipment.ID + "] amount=" + (object) equipmentUpdate.CurrencyDelta + "]");
            }
            else
            {
              lettuceEquipment.Owned = true;
              lettuceEquipment.m_tier = (int) equipmentUpdate.Tier;
              Log.All.Print("EQUIPMENT UPDATE - mercenary[" + (object) mercenary.ID + "] equipment[" + (object) lettuceEquipment.ID + "] tier=" + (object) equipmentUpdate.Tier + "]");
            }
          }
        }
      }
    }
  }

  private void OnUpdateMercenariesTeamResponse()
  {
    UpdateMercenariesTeamResponse mercenariesTeamResponse = Network.Get().UpdateMercenariesTeamResponse();
    if (mercenariesTeamResponse == null)
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.OnUpdateMercenariesTeamResponse(): No response received.");
    else if (!mercenariesTeamResponse.HasTeamId)
    {
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.OnUpdateMercenariesTeamResponse(): No team ID received.");
    }
    else
    {
      if (this.m_pendingTeamEditList == null)
        return;
      for (int index = this.m_pendingTeamEditList.Count - 1; index > -1; --index)
      {
        if (this.m_pendingTeamEditList[index].m_teamId == mercenariesTeamResponse.TeamId)
          this.m_pendingTeamEditList.RemoveAt(index);
      }
    }
  }

  private void NetCache_OnMercenariesTeamListResponse()
  {
    if (this.IsLettuceLoaded())
    {
      this.ProcessMercenariesTeamListResponse(NetCache.Get().GetNetObject<LettuceTeamList>());
    }
    else
    {
      this.m_mercTeamListResponse = NetCache.Get().GetNetObject<LettuceTeamList>();
      this.ProcessMercInitDataAfterEverythingReceived();
    }
  }

  private void ProcessMercenariesTeamListResponse(LettuceTeamList netCacheTeamList)
  {
    foreach (PegasusLettuce.LettuceTeam team in netCacheTeamList.Teams)
    {
      if (!team.HasTeamId)
        Log.CollectionManager.PrintError("CollectionManager_Lettuce.NetCache_OnMercenariesTeamListResponse(): Team has no team ID!");
      else if (this.GetTeam(team.TeamId) == null)
        this.AddReplaceTeam(team);
    }
  }

  private void NetCache_OnMercenariesPlayerInfoResponse()
  {
    this.m_playerInfoReceived = true;
    if (this.IsLettuceLoaded())
      return;
    this.ProcessMercInitDataAfterEverythingReceived();
  }

  private void OnTeamCreatedNetworkResponse()
  {
    CreateMercenariesTeamResponse mercenariesTeamResponse = Network.Get().CreateMercenariesTeamResponse();
    if (mercenariesTeamResponse == null)
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.OnTeamCreatedNetworkResponse(): No response received.");
    else if (!mercenariesTeamResponse.HasTeam)
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.OnTeamCreatedNetworkResponse(): No team received.");
    else if (!mercenariesTeamResponse.Team.HasName)
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.OnTeamCreatedNetworkResponse(): Received team has no name.");
    else if (!mercenariesTeamResponse.Team.HasTeamId)
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.OnTeamCreatedNetworkResponse(): Received teams has no team ID.");
    else if (!mercenariesTeamResponse.Team.HasType_)
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.OnTeamCreatedNetworkResponse(): Received teams has no team type.");
    else
      this.OnTeamCreated(new CollectionManager.PendingTeamCreateData()
      {
        m_name = mercenariesTeamResponse.Team.Name,
        m_teamId = mercenariesTeamResponse.Team.TeamId,
        m_type = mercenariesTeamResponse.Team.Type_,
        m_sortOrder = mercenariesTeamResponse.Team.SortOrder
      }, new int?(mercenariesTeamResponse.RequestId));
  }

  private void OnTeamCreated(
    CollectionManager.PendingTeamCreateData pendingTeamCreate,
    int? requestId)
  {
    this.m_pendingTeamCreate = (CollectionManager.PendingTeamCreateData) null;
    LettuceTeam lettuceTeam = this.AddTeam(pendingTeamCreate);
    lettuceTeam?.MarkNetworkContentsLoaded();
    if (requestId.HasValue)
    {
      if (!this.m_inTransitTeamCreateRequests.Contains(requestId.Value))
        return;
      this.m_inTransitTeamCreateRequests.Remove(requestId.Value);
    }
    foreach (CollectionManager.DelOnTeamCreated delOnTeamCreated in this.m_teamCreatedListeners.ToArray())
    {
      if (lettuceTeam != null)
        delOnTeamCreated(lettuceTeam.ID);
    }
  }

  private void OnTeamDeleted()
  {
    DeleteMercenariesTeamResponse mercenariesTeamResponse = Network.Get().DeleteMercenariesTeamResponse();
    if (mercenariesTeamResponse == null)
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.OnTeamDeleted(): No response received.");
    else if (!mercenariesTeamResponse.HasTeamId)
    {
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.OnTeamDeleted(): No team ID received.");
    }
    else
    {
      Log.CollectionManager.Print("CollectionManager_Lettuce.OnTeamDeleted");
      Log.CollectionManager.Print(string.Format("TeamDeleted:{0}", (object) mercenariesTeamResponse.TeamId));
      LettuceTeam removedTeam = this.RemoveTeam(mercenariesTeamResponse.TeamId);
      if (this.m_pendingTeamDeleteList != null)
      {
        for (int index = this.m_pendingTeamDeleteList.Count - 1; index > -1; --index)
        {
          if (this.m_pendingTeamDeleteList[index].m_teamId == mercenariesTeamResponse.TeamId)
            this.m_pendingTeamDeleteList.RemoveAt(index);
        }
      }
      if ((UnityEngine.Object) CollectionDeckTray.Get() == (UnityEngine.Object) null)
        return;
      LettuceTeam editingTeam = this.GetEditingTeam();
      if (this.IsInEditTeamMode() && editingTeam != null && editingTeam.ID == mercenariesTeamResponse.TeamId)
      {
        Navigation.Pop();
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_OFFLINE_FEATURE_DISABLED_HEADER"),
          m_text = GameStrings.Get("GLUE_OFFLINE_DECK_DELETED_REMOTELY_ERROR_BODY"),
          m_responseDisplay = AlertPopup.ResponseDisplay.OK,
          m_showAlertIcon = true
        };
        DialogManager.Get().ShowPopup(info);
      }
      if (removedTeam == null)
        return;
      foreach (CollectionManager.DelOnTeamDeleted delOnTeamDeleted in this.m_teamDeletedListeners.ToArray())
        delOnTeamDeleted(removedTeam);
    }
  }

  public void OnTeamDeletedWhileOffline(long teamId)
  {
  }

  public void AddPendingTeamDelete(long teamId)
  {
    if (this.m_pendingTeamDeleteList == null)
      this.m_pendingTeamDeleteList = new List<CollectionManager.PendingTeamDeleteData>();
    this.m_pendingTeamDeleteList.Add(new CollectionManager.PendingTeamDeleteData()
    {
      m_teamId = teamId
    });
  }

  public void SendCreateTeam(string name, PegasusLettuce.LettuceTeam.Type type, string pastedTeamHashString = null)
  {
    if (this.m_pendingTeamCreate != null)
      Log.Offline.PrintWarning("SendCreateTeam - Attempting to create a team while another is still pending.");
    this.m_pendingTeamCreate = new CollectionManager.PendingTeamCreateData()
    {
      m_name = name,
      m_pastedTeamHash = pastedTeamHashString,
      m_type = type
    };
    if (Network.IsLoggedIn())
    {
      int? requestId;
      Network.Get().CreateMercenariesTeamRequest(name, type, out requestId);
      if (!requestId.HasValue)
        return;
      this.m_inTransitTeamCreateRequests.Add(requestId.Value);
    }
    else
      this.CreateTeamOffline(this.m_pendingTeamCreate);
  }

  private void CreateTeamOffline(CollectionManager.PendingTeamCreateData data) => Processor.ScheduleCallback(0.5f, false, (Processor.ScheduledCallback) (_0 => this.OnTeamCreated(data, new int?())));

  private void FireTeamContentsEvent(long id)
  {
    foreach (CollectionManager.DelOnTeamContents delOnTeamContents in this.m_teamContentsListeners.ToArray())
      delOnTeamContents(id);
  }

  public string AutoGenerateTeamName()
  {
    int num = 1;
    string name;
    do
    {
      name = GameStrings.Format("GLUE_COLLECTION_CUSTOM_TEAMNAME_TEMPLATE", (object) num.ToString());
      ++num;
    }
    while (this.IsTeamNameTaken(name));
    return name;
  }

  private bool IsTeamNameTaken(string name)
  {
    foreach (LettuceTeam team in this.GetTeams())
    {
      if (team.Name.Trim().Equals(name, StringComparison.InvariantCultureIgnoreCase))
        return true;
    }
    return false;
  }

  public LettuceTeam GetEditingTeam()
  {
    LettuceTeam editingTeam = (LettuceTeam) null;
    this.m_teams.TryGetValue(this.m_editingTeamID, out editingTeam);
    return editingTeam;
  }

  public LettuceTeam SetEditingTeam(long teamId, object callbackData = null)
  {
    LettuceTeam team = (LettuceTeam) null;
    this.m_teams.TryGetValue(teamId, out team);
    this.SetEditingTeam(team, callbackData);
    return team;
  }

  public void SetEditingTeam(LettuceTeam team, object callbackData = null)
  {
    LettuceTeam editingTeam = this.GetEditingTeam();
    if (team == editingTeam)
      return;
    this.m_editingTeamID = team == null ? 0L : team.ID;
    foreach (CollectionManager.OnEditingTeamChanged editingTeamChanged in this.m_editingTeamChangedListeners.ToArray())
      editingTeamChanged(team, editingTeam, callbackData);
  }

  public void ClearEditingTeam() => this.SetEditingTeam((LettuceTeam) null);

  private LettuceTeam AddReplaceTeam(PegasusLettuce.LettuceTeam networkTeam)
  {
    LettuceTeam team = LettuceTeam.Convert(networkTeam);
    if (team != null)
    {
      this.AddTeam(team, false);
      team.MarkNetworkContentsLoaded();
      this.FireTeamContentsEvent(team.ID);
    }
    return team;
  }

  private LettuceTeam AddTeam(
    CollectionManager.PendingTeamCreateData pendingTeamCreate)
  {
    if (pendingTeamCreate == null)
      return (LettuceTeam) null;
    LettuceTeam team = new LettuceTeam(pendingTeamCreate.m_sortOrder)
    {
      ID = pendingTeamCreate.m_teamId,
      Name = pendingTeamCreate.m_name,
      NeedsName = false,
      TeamType = pendingTeamCreate.m_type
    };
    this.AddTeam(team, true);
    return team;
  }

  private void AddTeam(LettuceTeam team, bool updateNetCache)
  {
    if (this.m_teams.ContainsKey(team.ID))
      this.m_teams.Remove(team.ID);
    this.m_teams.Add(team.ID, team);
    if (!updateNetCache)
      return;
    LettuceTeamList netObject = NetCache.Get().GetNetObject<LettuceTeamList>();
    PegasusLettuce.LettuceTeam lettuceTeam1 = netObject.Teams.Find((Predicate<PegasusLettuce.LettuceTeam>) (t => t.TeamId == team.ID));
    if (lettuceTeam1 != null)
      netObject.Teams.Remove(lettuceTeam1);
    PegasusLettuce.LettuceTeam lettuceTeam2 = LettuceTeam.Convert(team);
    if (lettuceTeam2 == null)
      return;
    netObject.Teams.Add(lettuceTeam2);
  }

  private void UpdateTeam(PegasusLettuce.LettuceTeam team)
  {
    if (team == null)
      Log.CollectionManager.PrintError("UpdateFromTeamList: teamList contained a null team!");
    else if (!team.HasTeamId)
      Log.CollectionManager.PrintError("UpdateFromTeamList: Team has no team ID!");
    else if (!team.HasType_)
      Log.CollectionManager.PrintError("UpdateFromTeamList: Team has no team type!");
    else if (this.m_teams == null)
    {
      Log.CollectionManager.PrintError("UpdateFromTeamList: m_teams is null!");
    }
    else
    {
      if (this.AddReplaceTeam(team) != null)
        return;
      Log.CollectionManager.PrintError("UpdateFromTeamList: failed to update team!");
    }
  }

  public LettuceTeam RemoveTeam(long id)
  {
    LettuceTeam lettuceTeam = (LettuceTeam) null;
    if (this.m_teams.TryGetValue(id, out lettuceTeam))
      this.m_teams.Remove(id);
    LettuceTeamList netObject = NetCache.Get().GetNetObject<LettuceTeamList>();
    if (netObject == null)
      return lettuceTeam;
    for (int index = 0; index < netObject.Teams.Count; ++index)
    {
      PegasusLettuce.LettuceTeam team = netObject.Teams[index];
      if (team.HasTeamId && team.TeamId == id)
      {
        netObject.Teams.RemoveAt(index);
        break;
      }
    }
    return lettuceTeam;
  }

  public bool IsInEditTeamMode() => this.m_editTeamMode;

  public LettuceTeam StartEditingTeam(long teamId, object callbackData = null)
  {
    this.m_editTeamMode = true;
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.MERCENARIES_TEAM_EDITOR);
    return this.SetEditingTeam(teamId, callbackData);
  }

  public void DoneEditingTeam()
  {
    int num = this.m_editTeamMode ? 1 : 0;
    this.m_editTeamMode = false;
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.MERCENARIES_COLLECTION);
  }

  public void RequestTeamContents(long id)
  {
    LettuceTeam team = this.GetTeam(id);
    if (team == null || !team.NetworkContentsLoaded())
      return;
    this.FireTeamContentsEvent(id);
  }

  public LettuceTeam GetTeam(long id)
  {
    LettuceTeam lettuceTeam;
    return this.m_teams.TryGetValue(id, out lettuceTeam) ? lettuceTeam : (LettuceTeam) null;
  }

  public List<LettuceTeam> GetTeams()
  {
    List<LettuceTeam> teams = new List<LettuceTeam>();
    foreach (LettuceTeam lettuceTeam in this.m_teams.Values)
      teams.Add(lettuceTeam);
    return teams;
  }

  public static void SortTeams(List<LettuceTeam> teams) => teams?.Sort((Comparison<LettuceTeam>) ((a, b) => ((long) a.SortOrder + -100L).CompareTo((long) b.SortOrder + -100L)));

  public int GetTeamSize() => NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().MercenariesTeamMaxSize;

  public LettuceMercenary GetMercenary(
    long mercenaryDbId,
    bool AttemptToGenerate = false,
    bool ReportError = true)
  {
    LettuceMercenary mercenary = (LettuceMercenary) null;
    this.m_collectibleMercenaryDBIds.TryGetValue(mercenaryDbId, out mercenary);
    if (mercenary == null)
      this.m_extraMercenaryDBIds.TryGetValue(mercenaryDbId, out mercenary);
    if (mercenary == null & AttemptToGenerate)
    {
      mercenary = this.GenerateMercenary((int) mercenaryDbId);
      if (mercenary != null)
      {
        this.m_extraMercenaries.Add(mercenary);
        this.m_extraMercenaryDBIds.Add(mercenaryDbId, mercenary);
      }
    }
    if (ReportError && mercenary == null)
      Log.Lettuce.PrintError("Invalid mercenary for card ID [{0}]", (object) mercenaryDbId);
    return mercenary;
  }

  public LettuceMercenary GetMercenary(string cardId)
  {
    foreach (LettuceMercenaryDbfRecord record in GameDbf.LettuceMercenary.GetRecords())
    {
      foreach (MercenaryArtVariationDbfRecord mercenaryArtVariation in record.MercenaryArtVariations)
      {
        if (mercenaryArtVariation.CardRecord.NoteMiniGuid == cardId)
          return this.m_collectibleMercenaryDBIds[(long) mercenaryArtVariation.LettuceMercenaryId];
      }
    }
    return (LettuceMercenary) null;
  }

  public int GetTotalMercenaryCount() => this.m_collectibleMercenaries.Count;

  public int GetOwnedMercenaryCount()
  {
    int ownedMercenaryCount = 0;
    foreach (LettuceMercenary collectibleMercenary in this.m_collectibleMercenaries)
    {
      if (collectibleMercenary.m_owned)
        ++ownedMercenaryCount;
    }
    return ownedMercenaryCount;
  }

  public bool HasFullyUpgradedAnyCollectibleMercenary()
  {
    foreach (LettuceMercenary collectibleMercenary in this.m_collectibleMercenaries)
    {
      if (collectibleMercenary.m_isFullyUpgraded)
        return true;
    }
    return false;
  }

  public void SendEquippedMercenaryEquipment(int mercenaryDbId)
  {
    LettuceMercenary mercenary = this.GetMercenary((long) mercenaryDbId);
    if (mercenary == null)
    {
      Log.CollectionManager.PrintError("SendEquippedMercenaryEquipment(): Invalid mercenary [{0}]!", (object) mercenaryDbId);
    }
    else
    {
      Network.Get().UpdateEquippedMercenaryEquipment(mercenaryDbId, mercenary.GetCurrentLoadout().m_equipmentRecord?.ID);
      this.AddPendingMercenaryEdit((long) mercenaryDbId);
      mercenary.m_equipmentSelectionChanged = false;
    }
  }

  public void AddPendingMercenaryEdit(long mercenaryDbId)
  {
    if (this.m_pendingMercenaryEditList == null)
      this.m_pendingMercenaryEditList = new List<CollectionManager.PendingMercenaryEditData>();
    this.m_pendingMercenaryEditList.Add(new CollectionManager.PendingMercenaryEditData()
    {
      m_mercenaryId = mercenaryDbId
    });
  }

  private void OnUpdateEquippedMercenaryEquipmentResponse()
  {
    UpdateEquippedMercenaryEquipmentResponse equipmentResponse = Network.Get().UpdateEquippedMercenaryEquipmentResponse();
    if (equipmentResponse == null)
      Log.CollectionManager.PrintError("UpdateEquippedMercenaryEquipmentResponse(): No response received.");
    else if (!equipmentResponse.HasMercenaryId)
    {
      Log.CollectionManager.PrintError("UpdateEquippedMercenaryEquipmentResponse(): No mercenary ID received.");
    }
    else
    {
      if (this.m_pendingMercenaryEditList == null)
        return;
      for (int index = this.m_pendingMercenaryEditList.Count - 1; index > -1; --index)
      {
        if (this.m_pendingMercenaryEditList[index].m_mercenaryId == (long) equipmentResponse.MercenaryId)
          this.m_pendingMercenaryEditList.RemoveAt(index);
      }
    }
  }

  public void SendSelectedMercenaryArtVariation(
    int mercenaryDbId,
    int artVariationId,
    TAG_PREMIUM premium)
  {
    LettuceMercenary mercenary = this.GetMercenary((long) mercenaryDbId);
    if (mercenary == null)
    {
      Log.CollectionManager.PrintError("SendSelectedMercenaryArtVariation(): Invalid mercenary [{0}]!", (object) mercenaryDbId);
    }
    else
    {
      Network.Get().UpdateEquippedMercenaryArtVariation(mercenaryDbId, artVariationId, premium);
      mercenary.SetEquippedArtVariation(artVariationId, premium);
      System.Action<int, int, TAG_PREMIUM> variationChangedEvent = this.MercenaryArtVariationChangedEvent;
      if (variationChangedEvent == null)
        return;
      variationChangedEvent(mercenaryDbId, artVariationId, premium);
    }
  }

  private void OnUpdateEquippedMercenaryArtVariationResponse()
  {
    UpdateEquippedMercenaryArtVariationResponse variationResponse = Network.Get().UpdateEquippedMercenaryArtVariationResponse();
    if (variationResponse == null)
    {
      Log.CollectionManager.PrintError("OnUpdateEquippedMercenaryArtVariationResponse(): No response received.");
    }
    else
    {
      if (variationResponse.HasMercenaryId)
        return;
      Log.CollectionManager.PrintError("OnUpdateEquippedMercenaryArtVariationResponse(): No mercenary ID received.");
    }
  }

  private void LettuceReset()
  {
    this.m_teams.Clear();
    this.m_editingTeamID = 0L;
    this.m_mercsAndTeamsReceived = false;
  }

  private void LettuceInitImpl()
  {
    Network.Get().RegisterNetHandler((object) MercenariesCollectionResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesCollectionResponse));
    Network.Get().RegisterNetHandler((object) MercenariesCollectionUpdate.PacketID.ID, new Network.NetHandler(this.OnMercenariesCollectionUpdate));
    Network.Get().RegisterNetHandler((object) MercenariesCurrencyUpdate.PacketID.ID, new Network.NetHandler(this.OnMercenariesCurrencyUpdate));
    Network.Get().RegisterNetHandler((object) MercenariesExperienceUpdate.PacketID.ID, new Network.NetHandler(this.OnMercenariesExperienceUpdate));
    Network.Get().RegisterNetHandler((object) MercenariesRewardUpdate.PacketID.ID, new Network.NetHandler(this.OnMercenariesRewardUpdate));
    Network.Get().RegisterNetHandler((object) MercenariesTeamUpdate.PacketID.ID, new Network.NetHandler(this.OnMercenariesTeamUpdate));
    Network.Get().RegisterNetHandler((object) MercenariesTrainingAddResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesTrainingAddResponse));
    Network.Get().RegisterNetHandler((object) MercenariesTrainingRemoveResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesTrainingRemoveResponse));
    Network.Get().RegisterNetHandler((object) MercenariesTrainingCollectResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesTrainingCollectResponse));
    Network.Get().RegisterNetHandler((object) CreateMercenariesTeamResponse.PacketID.ID, new Network.NetHandler(this.OnTeamCreatedNetworkResponse));
    Network.Get().RegisterNetHandler((object) UpdateMercenariesTeamResponse.PacketID.ID, new Network.NetHandler(this.OnUpdateMercenariesTeamResponse));
    Network.Get().RegisterNetHandler((object) DeleteMercenariesTeamResponse.PacketID.ID, new Network.NetHandler(this.OnTeamDeleted));
    Network.Get().RegisterNetHandler((object) UpdateEquippedMercenaryEquipmentResponse.PacketID.ID, new Network.NetHandler(this.OnUpdateEquippedMercenaryEquipmentResponse));
    Network.Get().RegisterNetHandler((object) UpdateEquippedMercenaryArtVariationResponse.PacketID.ID, new Network.NetHandler(this.OnUpdateEquippedMercenaryArtVariationResponse));
    NetCache.Get().RegisterUpdatedListener(typeof (LettuceTeamList), new System.Action(this.NetCache_OnMercenariesTeamListResponse));
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheMercenariesPlayerInfo), new System.Action(this.NetCache_OnMercenariesPlayerInfoResponse));
  }

  private LettuceMercenary RegisterMercenary(int mercenaryDbId)
  {
    LettuceMercenary mercenary = this.GetMercenary((long) mercenaryDbId, ReportError: false);
    if (mercenary == null)
    {
      mercenary = this.GenerateMercenary(mercenaryDbId);
      if (mercenary != null)
      {
        this.m_collectibleMercenaries.Add(mercenary);
        this.m_collectibleMercenaryDBIds.Add((long) mercenaryDbId, mercenary);
      }
    }
    return mercenary;
  }

  private void RegisterMercenaryCard(int cardDBId)
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardDBId);
    if (entityDef == null)
      Error.AddDevFatal(string.Format("Failed to find an EntityDef for mercenary card {0}", (object) cardDBId));
    else
      this.RegisterCard(entityDef, entityDef.GetCardId(), TAG_PREMIUM.NORMAL);
  }

  public LettuceMercenary GenerateMercenary(int mercenaryDbId)
  {
    LettuceMercenary mercenary1 = (LettuceMercenary) null;
    LettuceMercenaryDbfRecord record = GameDbf.LettuceMercenary.GetRecord(mercenaryDbId);
    if (record == null)
    {
      Log.CollectionManager.PrintError("CollectionManager_Lettuce.RegisterMercenary(): Invalid mercenary ID [{0}]!", (object) mercenaryDbId);
      return mercenary1;
    }
    LettuceMercenary mercenary2 = new LettuceMercenary()
    {
      ID = mercenaryDbId,
      m_mercName = record.NoteDesc,
      m_mercShortName = record.NoteDesc,
      m_rarity = (TAG_RARITY) record.Rarity,
      m_acquireType = (TAG_ACQUIRE_TYPE) record.AcquireType,
      m_customAcquireText = (string) record.HowToAcquireText
    };
    LettuceMercenary.ArtVariation defaultArtVariation = LettuceMercenary.CreateDefaultArtVariation(mercenaryDbId);
    mercenary2.m_artVariations.Add(defaultArtVariation);
    mercenary2.GetBaseLoadout().SetArtVariation(defaultArtVariation.m_record, defaultArtVariation.m_premium);
    foreach (MercenaryArtVariationDbfRecord mercenaryArtVariation in record.MercenaryArtVariations)
    {
      if (mercenary2.m_role == TAG_ROLE.INVALID)
      {
        EntityDef entityDef = DefLoader.Get().GetEntityDef(mercenaryArtVariation.CardId);
        if (entityDef != null)
        {
          string shortName = entityDef.GetShortName();
          mercenary2.m_mercName = entityDef.GetName();
          mercenary2.m_mercShortName = string.IsNullOrEmpty(shortName) ? mercenary2.m_mercName : shortName;
          mercenary2.m_role = entityDef.GetTag<TAG_ROLE>(GAME_TAG.LETTUCE_ROLE);
        }
      }
      this.RegisterMercenaryCard(mercenaryArtVariation.CardId);
    }
    foreach (LettuceMercenarySpecializationDbfRecord mercenarySpecialization in record.LettuceMercenarySpecializations)
    {
      mercenary2.m_abilitySpecializations.Add((string) mercenarySpecialization.Name);
      foreach (LettuceMercenaryAbilityDbfRecord mercenaryAbility in mercenarySpecialization.LettuceMercenaryAbilities)
      {
        LettuceAbilityDbfRecord lettuceAbilityRecord = mercenaryAbility.LettuceAbilityRecord;
        LettuceAbility lettuceAbility = new LettuceAbility(CollectionUtils.MercenariesModeCardType.Ability)
        {
          ID = lettuceAbilityRecord.ID,
          m_abilityName = lettuceAbilityRecord.NoteDesc,
          m_unlockLevel = mercenaryAbility.LettuceMercenaryLevelIdRequired
        };
        foreach (LettuceAbilityTierDbfRecord lettuceAbilityTier in lettuceAbilityRecord.LettuceAbilityTiers)
        {
          if (lettuceAbilityTier.Tier < 1 || lettuceAbilityTier.Tier > lettuceAbility.m_tierList.Length)
          {
            Log.CollectionManager.PrintError("CollectionManager_Lettuce.RegisterMercenary(): Invalid ability tier [{0}] from ability record [{1}]!", (object) lettuceAbilityTier.Tier, (object) lettuceAbilityTier);
          }
          else
          {
            LettuceAbility.AbilityTier tier = lettuceAbility.m_tierList[lettuceAbilityTier.Tier - 1];
            tier.m_tier = lettuceAbilityTier.Tier;
            tier.m_coinCost = lettuceAbilityTier.CoinCraftCost;
            tier.m_cardId = GameUtils.TranslateDbIdToCardId(lettuceAbilityTier.CardId, true);
            tier.m_cardName = lettuceAbilityTier.CardRecord.Name.GetString();
            tier.m_validTier = true;
            this.RegisterMercenaryCard(lettuceAbilityTier.CardId);
          }
        }
        mercenary2.m_abilityList.Add(lettuceAbility);
      }
    }
    foreach (LettuceMercenaryEquipmentDbfRecord equipmentDbfRecord in record.LettuceMercenaryEquipment)
    {
      LettuceEquipmentDbfRecord lettuceEquipmentRecord = equipmentDbfRecord.LettuceEquipmentRecord;
      if (lettuceEquipmentRecord == null)
      {
        Log.CollectionManager.PrintError("CollectionManager_Lettuce.RegisterMercenary(): Mercenary " + record.NoteDesc + " equipment record is null!");
      }
      else
      {
        LettuceAbility lettuceAbility = new LettuceAbility(CollectionUtils.MercenariesModeCardType.Equipment)
        {
          ID = lettuceEquipmentRecord.ID,
          m_abilityName = lettuceEquipmentRecord.NoteDesc
        };
        foreach (LettuceEquipmentTierDbfRecord lettuceEquipmentTier in lettuceEquipmentRecord.LettuceEquipmentTiers)
        {
          if (lettuceEquipmentTier.Tier < 1 || lettuceEquipmentTier.Tier > lettuceAbility.m_tierList.Length)
          {
            Log.CollectionManager.PrintError("CollectionManager_Lettuce.RegisterMercenary(): Invalid equipment tier [{0}] from equipment record [{1}]!", (object) lettuceEquipmentTier.Tier, (object) lettuceEquipmentTier);
          }
          else
          {
            LettuceAbility.AbilityTier tier = lettuceAbility.m_tierList[lettuceEquipmentTier.Tier - 1];
            tier.m_tier = lettuceEquipmentTier.Tier;
            tier.m_coinCost = lettuceEquipmentTier.CoinCraftCost;
            tier.m_cardId = GameUtils.TranslateDbIdToCardId(lettuceEquipmentTier.CardId, true);
            tier.m_cardName = lettuceEquipmentTier.CardRecord.Name.GetString();
            tier.m_validTier = true;
            this.RegisterMercenaryCard(lettuceEquipmentTier.CardId);
          }
        }
        lettuceAbility.m_tier = lettuceAbility.GetBaseTier();
        mercenary2.m_equipmentList.Add(lettuceAbility);
      }
    }
    return mercenary2;
  }

  public (LettuceMercenary, LettuceMercenary) GetMercenariesInTraining()
  {
    (LettuceMercenary, LettuceMercenary) mercenariesInTraining = ((LettuceMercenary) null, (LettuceMercenary) null);
    foreach (KeyValuePair<long, LettuceMercenary> collectibleMercenaryDbId in this.m_collectibleMercenaryDBIds)
    {
      if (collectibleMercenaryDbId.Value.m_trainingStartDate != null)
      {
        if (mercenariesInTraining.Item1 == null)
        {
          mercenariesInTraining.Item1 = collectibleMercenaryDbId.Value;
        }
        else
        {
          mercenariesInTraining.Item2 = collectibleMercenaryDbId.Value;
          break;
        }
      }
    }
    return mercenariesInTraining;
  }

  public bool DoesMercenaryNeedToBeAcknowledged(LettuceMercenary merc)
  {
    if (merc != null)
    {
      foreach (LettuceAbility ability in merc.m_abilityList)
      {
        if (!ability.IsAcknowledged(merc))
          return true;
      }
      foreach (LettuceAbility equipment in merc.m_equipmentList)
      {
        if (!equipment.IsAcknowledged(merc))
          return true;
      }
      if (this.GetNumNewPortraitsToAcknowledgeForMercenary(merc) > 0)
        return true;
    }
    return false;
  }

  public int GetNumNewPortraitsToAcknowledgeForMercenary(LettuceMercenary merc)
  {
    int acknowledgeForMercenary = 0;
    foreach (LettuceMercenary.ArtVariation artVariation in merc.m_artVariations)
    {
      if (!artVariation.m_acknowledged)
        ++acknowledgeForMercenary;
    }
    return acknowledgeForMercenary;
  }

  public bool DoesAnyMercenaryNeedToBeAcknowledged()
  {
    foreach (KeyValuePair<long, LettuceMercenary> collectibleMercenaryDbId in this.m_collectibleMercenaryDBIds)
    {
      if (collectibleMercenaryDbId.Value.m_owned && this.DoesMercenaryNeedToBeAcknowledged(collectibleMercenaryDbId.Value))
        return true;
    }
    return false;
  }

  public int GetNumMercenariesToAcknowledgeForRole(TAG_ROLE roleTag)
  {
    int acknowledgeForRole = 0;
    foreach (KeyValuePair<long, LettuceMercenary> collectibleMercenaryDbId in this.m_collectibleMercenaryDBIds)
    {
      if (collectibleMercenaryDbId.Value.m_owned && collectibleMercenaryDbId.Value.m_role == roleTag && this.DoesMercenaryNeedToBeAcknowledged(collectibleMercenaryDbId.Value))
        ++acknowledgeForRole;
    }
    return acknowledgeForRole;
  }

  public void MarkMercenaryAsAcknowledgedinCollection(MercenaryAcknowledgeData ackData)
  {
    LettuceMercenary mercenary = this.GetMercenary((long) ackData.MercenaryId);
    if (mercenary == null)
      return;
    switch (ackData.Type)
    {
      case MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_ABILITY_ALL:
        using (List<LettuceAbility>.Enumerator enumerator = mercenary.m_abilityList.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            LettuceAbility current = enumerator.Current;
            if (ackData.AssetId == current.ID)
            {
              current.m_acquireAcknowledged = true;
              current.m_upgradeAcknowledged = true;
            }
          }
          break;
        }
      case MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_ABILITY_ACQUIRED:
        using (List<LettuceAbility>.Enumerator enumerator = mercenary.m_abilityList.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            LettuceAbility current = enumerator.Current;
            if (ackData.AssetId == current.ID)
              current.m_acquireAcknowledged = true;
          }
          break;
        }
      case MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_ABILITY_UPGRADE:
        using (List<LettuceAbility>.Enumerator enumerator = mercenary.m_abilityList.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            LettuceAbility current = enumerator.Current;
            if (ackData.AssetId == current.ID)
              current.m_upgradeAcknowledged = true;
          }
          break;
        }
      case MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_ART_VARIATION_ACQUIRED:
        using (List<LettuceMercenary.ArtVariation>.Enumerator enumerator = mercenary.m_artVariations.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            LettuceMercenary.ArtVariation current = enumerator.Current;
            if (current.m_premium == (TAG_PREMIUM) ackData.Premium && current.m_record.ID == ackData.AssetId)
              current.m_acknowledged = true;
          }
          break;
        }
      case MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_EQUIPMENT_ALL:
        using (List<LettuceAbility>.Enumerator enumerator = mercenary.m_equipmentList.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            LettuceAbility current = enumerator.Current;
            if (ackData.AssetId == current.ID)
            {
              current.m_acquireAcknowledged = true;
              current.m_upgradeAcknowledged = true;
            }
          }
          break;
        }
      case MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_EQUIPMENT_ACQUIRED:
        using (List<LettuceAbility>.Enumerator enumerator = mercenary.m_equipmentList.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            LettuceAbility current = enumerator.Current;
            if (ackData.AssetId == current.ID)
              current.m_acquireAcknowledged = true;
          }
          break;
        }
      case MercenaryAcknowledgeData.AcknowledgeType.ACKNOWLEDGE_MERC_EQUIPMENT_UPGRADE:
        using (List<LettuceAbility>.Enumerator enumerator = mercenary.m_equipmentList.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            LettuceAbility current = enumerator.Current;
            if (ackData.AssetId == current.ID)
              current.m_upgradeAcknowledged = true;
          }
          break;
        }
    }
  }

  public delegate bool CollectibleCardFilterFunc(CollectibleCard card);

  public class PreconDeck
  {
    private long m_id;

    public PreconDeck(long id) => this.m_id = id;

    public long ID => this.m_id;
  }

  public class TemplateDeck
  {
    public int m_id;
    public int m_deckTemplateId;
    public TAG_CLASS m_class;
    public int m_sortOrder;
    public Map<string, int> m_cardIds = new Map<string, int>();
    public string m_title;
    public string m_description;
    public string m_displayTexture;
    public SpecialEventType m_event;
    public bool m_isStarterDeck;
    public PegasusShared.FormatType m_formatType;
    public RuneType m_rune1;
    public RuneType m_rune2;
    public RuneType m_rune3;
  }

  public class FindCardsResult
  {
    public List<CollectibleCard> m_cards = new List<CollectibleCard>();
    public bool m_resultsWithoutManaFilterExist;
    public bool m_resultsWithoutSetFilterExist;
    public bool m_resultsUnownedExist;
    public bool m_resultsInWildExist;
  }

  public delegate void DelCollectionManagerReady();

  public delegate void DelOnCollectionLoaded();

  public delegate void DelOnCollectionChanged();

  public delegate void DelOnDeckCreated(long id, string name);

  public delegate void DelOnDeckDeleted(CollectionDeck removedDeck);

  public delegate void DelOnDeckContents(long id);

  public delegate void DelOnAllDeckContents();

  public delegate void DelOnNewCardSeen(string cardID, TAG_PREMIUM premium);

  public delegate void DelOnCardRewardsInserted(List<string> cardIDs, List<TAG_PREMIUM> premium);

  public delegate void OnMassDisenchant(int amount);

  public delegate void OnEditedDeckChanged(
    CollectionDeck newDeck,
    CollectionDeck oldDeck,
    object callbackData);

  public delegate void FavoriteHeroChangedCallback(
    TAG_CLASS heroClass,
    NetCache.CardDefinition favoriteHero,
    bool isFavorite,
    object userData);

  public delegate void OnUIHeroOverrideCardRemovedCallback();

  public delegate void DeckAutoFillCallback(
    CollectionDeck deck,
    IEnumerable<DeckMaker.DeckFill> deckFill);

  private class TagCardSetEnumComparer : IEqualityComparer<TAG_CARD_SET>
  {
    public bool Equals(TAG_CARD_SET x, TAG_CARD_SET y) => x == y;

    public int GetHashCode(TAG_CARD_SET obj) => (int) obj;
  }

  private class TagClassEnumComparer : IEqualityComparer<TAG_CLASS>
  {
    public bool Equals(TAG_CLASS x, TAG_CLASS y) => x == y;

    public int GetHashCode(TAG_CLASS obj) => (int) obj;
  }

  private class TagCardTypeEnumComparer : IEqualityComparer<TAG_CARDTYPE>
  {
    public bool Equals(TAG_CARDTYPE x, TAG_CARDTYPE y) => x == y;

    public int GetHashCode(TAG_CARDTYPE obj) => (int) obj;
  }

  private struct CollectibleCardIndex
  {
    public string CardId;
    public TAG_PREMIUM Premium;

    public CollectibleCardIndex(string cardId, TAG_PREMIUM premium)
    {
      this.CardId = cardId;
      this.Premium = premium;
    }
  }

  private class CollectibleCardIndexComparer : 
    IEqualityComparer<CollectionManager.CollectibleCardIndex>
  {
    public bool Equals(
      CollectionManager.CollectibleCardIndex x,
      CollectionManager.CollectibleCardIndex y)
    {
      return x.CardId == y.CardId && x.Premium == y.Premium;
    }

    public int GetHashCode(CollectionManager.CollectibleCardIndex obj) => (obj.CardId, obj.Premium).GetHashCode();
  }

  private class FavoriteHeroChangedListener : 
    EventListener<CollectionManager.FavoriteHeroChangedCallback>
  {
    public void Fire(TAG_CLASS heroClass, NetCache.CardDefinition favoriteHero, bool isFavorite) => this.m_callback(heroClass, favoriteHero, isFavorite, this.m_userData);
  }

  private class OnUIHeroOverrideCardRemovedListener : 
    EventListener<CollectionManager.OnUIHeroOverrideCardRemovedCallback>
  {
    public void Fire() => this.m_callback();
  }

  private class PendingDeckCreateData
  {
    public DeckType m_deckType;
    public string m_name;
    public int m_heroDbId;
    public PegasusShared.FormatType m_formatType;
    public DeckSourceType m_sourceType;
    public string m_pastedDeckHash;
  }

  private class PendingDeckDeleteData
  {
    public long m_deckId;
  }

  private class PendingDeckEditData
  {
    public long m_deckId;
  }

  private class PendingDeckRenameData
  {
    public long m_deckId;
    public string m_name;
  }

  public class DeckSort : IComparer<CollectionDeck>
  {
    public int Compare(CollectionDeck a, CollectionDeck b) => a.SortOrder == b.SortOrder ? b.CreateDate.CompareTo(a.CreateDate) : a.SortOrder.CompareTo(b.SortOrder);
  }

  public class FindMercenariesResult
  {
    public List<LettuceMercenary> m_mercenaries = new List<LettuceMercenary>();
  }

  public delegate bool MercenaryFilterFunc(LettuceMercenary merc);

  public delegate void DelOnTeamCreated(long id);

  public delegate void DelOnTeamDeleted(LettuceTeam removedTeam);

  public delegate void DelOnTeamContents(long id);

  public delegate void DelOnAllTeamContents();

  public delegate void OnEditingTeamChanged(
    LettuceTeam newTeam,
    LettuceTeam oldTeam,
    object callbackData);

  private class TagRoleEnumComparer : IEqualityComparer<TAG_ROLE>
  {
    public bool Equals(TAG_ROLE x, TAG_ROLE y) => x == y;

    public int GetHashCode(TAG_ROLE obj) => (int) obj;
  }

  private class PendingTeamCreateData
  {
    public string m_name;
    public string m_pastedTeamHash;
    public long m_teamId;
    public PegasusLettuce.LettuceTeam.Type m_type;
    public uint m_sortOrder;
  }

  private class PendingTeamDeleteData
  {
    public long m_teamId;
  }

  private class PendingTeamEditData
  {
    public long m_teamId;
  }

  private class PendingMercenaryEditData
  {
    public long m_mercenaryId;
  }
}
