using Assets;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDbfIndex
{
  private Map<string, CardDbfRecord> m_cardsByCardId;
  private Map<int, List<CardTagDbfRecord>> m_cardTagsByCardDbId;
  private Map<string, CardDiscoverStringDbfRecord> m_cardDiscoverStringsByCardId;
  private List<string> m_allCardIds;
  private List<int> m_allCardDbIds;
  private List<string> m_collectibleCardIds;
  private List<int> m_collectibleCardDbIds;
  private int m_collectibleCardCount;
  private HashSet<CardDbfRecord> m_featuredCardEventCards;
  private Map<(int, int), FixedRewardDbfRecord> m_fixedRewardsByCardId;
  private Map<int, List<FixedRewardMapDbfRecord>> m_fixedRewardsByAction;
  private Map<FixedRewardAction.Type, List<FixedRewardActionDbfRecord>> m_fixedActionRecordsByType;
  private Map<int, List<int>> m_subsetsReferencedByRuleId;
  private Map<int, HashSet<string>> m_subsetCards;
  private Map<int, HashSet<int>> m_rulesByDeckRulesetId;
  private Map<int, List<CardChangeDbfRecord>> m_cardChangesByCardId;
  private Map<int, Map<SpellType, string>> m_spellOverridesByCardSetId;
  private Map<int, int> m_cardsWithPlayerDeckOverrides;
  private Map<int, LettuceEquipmentTierDbfRecord> m_equipmentTierByCardId;
  private Map<int, MercenaryUnlock> m_equipmentUnlockByEquipmentId;
  private Map<int, Map<TAG_PREMIUM, MercenaryUnlock>> m_UnlockByArtVariationIdAndPremium;
  private Map<int, int> m_MercenariesTaskIndex;
  private int m_maxMercenaryLevel;

  public GameDbfIndex() => this.Initialize();

  public void Initialize()
  {
    this.m_cardsByCardId = new Map<string, CardDbfRecord>();
    this.m_cardTagsByCardDbId = new Map<int, List<CardTagDbfRecord>>();
    this.m_cardDiscoverStringsByCardId = new Map<string, CardDiscoverStringDbfRecord>();
    this.m_allCardIds = new List<string>();
    this.m_allCardDbIds = new List<int>();
    this.m_collectibleCardIds = new List<string>();
    this.m_collectibleCardDbIds = new List<int>();
    this.m_collectibleCardCount = 0;
    this.m_featuredCardEventCards = new HashSet<CardDbfRecord>();
    this.m_fixedRewardsByCardId = new Map<(int, int), FixedRewardDbfRecord>();
    this.m_fixedRewardsByAction = new Map<int, List<FixedRewardMapDbfRecord>>();
    this.m_fixedActionRecordsByType = new Map<FixedRewardAction.Type, List<FixedRewardActionDbfRecord>>();
    this.m_subsetsReferencedByRuleId = new Map<int, List<int>>();
    this.m_subsetCards = new Map<int, HashSet<string>>();
    this.m_rulesByDeckRulesetId = new Map<int, HashSet<int>>();
    this.m_cardChangesByCardId = new Map<int, List<CardChangeDbfRecord>>();
    this.m_cardsWithPlayerDeckOverrides = new Map<int, int>();
    this.m_spellOverridesByCardSetId = new Map<int, Map<SpellType, string>>();
    this.m_equipmentTierByCardId = new Map<int, LettuceEquipmentTierDbfRecord>();
    this.m_equipmentUnlockByEquipmentId = new Map<int, MercenaryUnlock>();
    this.m_UnlockByArtVariationIdAndPremium = new Map<int, Map<TAG_PREMIUM, MercenaryUnlock>>();
    this.m_MercenariesTaskIndex = new Map<int, int>();
    this.m_maxMercenaryLevel = 0;
  }

  public void PostProcessDbfLoad_CardTag(Dbf<CardTagDbfRecord> dbf)
  {
    this.m_cardTagsByCardDbId.Clear();
    foreach (CardTagDbfRecord record in dbf.GetRecords())
      this.OnCardTagAdded(record);
  }

  public void OnCardTagAdded(CardTagDbfRecord cardTagRecord)
  {
    int cardId = cardTagRecord.CardId;
    List<CardTagDbfRecord> cardTagDbfRecordList = (List<CardTagDbfRecord>) null;
    if (!this.m_cardTagsByCardDbId.TryGetValue(cardId, out cardTagDbfRecordList))
    {
      cardTagDbfRecordList = new List<CardTagDbfRecord>();
      this.m_cardTagsByCardDbId[cardId] = cardTagDbfRecordList;
    }
    cardTagDbfRecordList.Add(cardTagRecord);
  }

  public void OnCardTagRemoved(List<CardTagDbfRecord> removedRecords)
  {
    foreach (CardTagDbfRecord removedRecord in removedRecords)
    {
      foreach (List<CardTagDbfRecord> cardTagDbfRecordList in this.m_cardTagsByCardDbId.Values)
      {
        if (cardTagDbfRecordList.Remove(removedRecord))
          break;
      }
    }
  }

  public void PostProcessDbfLoad_CardDiscoverString(Dbf<CardDiscoverStringDbfRecord> dbf)
  {
    this.m_cardDiscoverStringsByCardId.Clear();
    foreach (CardDiscoverStringDbfRecord record in dbf.GetRecords())
      this.OnCardDiscoverStringAdded(record);
  }

  public void OnCardDiscoverStringAdded(
    CardDiscoverStringDbfRecord cardDiscoverStringRecord)
  {
    this.m_cardDiscoverStringsByCardId[cardDiscoverStringRecord.NoteMiniGuid] = cardDiscoverStringRecord;
  }

  public void OnCardDiscoverStringRemoved(List<CardDiscoverStringDbfRecord> removedRecords)
  {
    foreach (CardDiscoverStringDbfRecord removedRecord in removedRecords)
      this.m_cardDiscoverStringsByCardId.Remove(removedRecord.NoteMiniGuid);
  }

  public int GetCardTagValue(int cardDbId, GAME_TAG tagId)
  {
    List<CardTagDbfRecord> cardTagDbfRecordList = (List<CardTagDbfRecord>) null;
    if (!this.m_cardTagsByCardDbId.TryGetValue(cardDbId, out cardTagDbfRecordList))
      return 0;
    int index = 0;
    for (int count = cardTagDbfRecordList.Count; index < count; ++index)
    {
      CardTagDbfRecord cardTagDbfRecord = cardTagDbfRecordList[index];
      if ((GAME_TAG) cardTagDbfRecord.TagId == tagId)
        return cardTagDbfRecord.TagValue;
    }
    return 0;
  }

  public bool TryGetCardTagRecords(int cardDbId, out List<CardTagDbfRecord> tagRecords) => this.m_cardTagsByCardDbId.TryGetValue(cardDbId, out tagRecords);

  public void PostProcessDbfLoad_Card(Dbf<CardDbfRecord> dbf)
  {
    this.m_cardsByCardId.Clear();
    this.m_allCardDbIds.Clear();
    this.m_allCardIds.Clear();
    this.m_collectibleCardCount = 0;
    this.m_collectibleCardIds.Clear();
    this.m_collectibleCardDbIds.Clear();
    this.m_featuredCardEventCards.Clear();
    foreach (CardDbfRecord record in dbf.GetRecords())
      this.OnCardAdded(record);
  }

  public void OnCardAdded(CardDbfRecord cardRecord)
  {
    int id = cardRecord.ID;
    int num = this.GetCardTagValue(id, GAME_TAG.COLLECTIBLE) == 1 ? 1 : 0;
    string noteMiniGuid = cardRecord.NoteMiniGuid;
    this.m_cardsByCardId[noteMiniGuid] = cardRecord;
    this.m_allCardDbIds.Add(id);
    this.m_allCardIds.Add(noteMiniGuid);
    if (num != 0)
    {
      ++this.m_collectibleCardCount;
      this.m_collectibleCardIds.Add(noteMiniGuid);
      this.m_collectibleCardDbIds.Add(id);
    }
    if (cardRecord.FeaturedCardsEvent == SpecialEventType.UNKNOWN)
      return;
    this.m_featuredCardEventCards.Add(cardRecord);
  }

  public void OnCardRemoved(List<CardDbfRecord> removedRecords)
  {
    HashSet<int> removedCardDbIds = new HashSet<int>();
    HashSet<string> removedCardIds = new HashSet<string>();
    foreach (CardDbfRecord removedRecord in removedRecords)
    {
      removedCardDbIds.Add(removedRecord.ID);
      if (removedRecord.NoteMiniGuid != null)
      {
        removedCardIds.Add(removedRecord.NoteMiniGuid);
        this.m_cardsByCardId.Remove(removedRecord.NoteMiniGuid);
      }
      this.m_featuredCardEventCards.Remove(removedRecord);
    }
    if (removedCardDbIds.Count > 0)
    {
      this.m_allCardDbIds.RemoveAll((Predicate<int>) (cardDbId => removedCardDbIds.Contains(cardDbId)));
      this.m_collectibleCardDbIds.RemoveAll((Predicate<int>) (cardDbId => this.m_collectibleCardDbIds.Contains(cardDbId)));
    }
    if (removedCardIds.Count <= 0)
      return;
    this.m_allCardIds.RemoveAll((Predicate<string>) (cardId => removedCardIds.Contains(cardId)));
    this.m_collectibleCardIds.RemoveAll((Predicate<string>) (cardId => removedCardIds.Contains(cardId)));
  }

  public void PostProcessDbfLoad_FixedReward(Dbf<FixedRewardDbfRecord> dbf)
  {
    this.m_fixedRewardsByCardId.Clear();
    foreach (FixedRewardDbfRecord record in dbf.GetRecords())
    {
      if (record.CardRecord != null)
        this.m_fixedRewardsByCardId.Add((record.CardId, record.CardPremium), record);
    }
  }

  public void PostProcessDbfLoad_FixedRewardMap(Dbf<FixedRewardMapDbfRecord> dbf)
  {
    this.m_fixedRewardsByAction.Clear();
    foreach (FixedRewardMapDbfRecord record in dbf.GetRecords())
      this.OnFixedRewardMapAdded(record);
  }

  public void OnFixedRewardMapAdded(FixedRewardMapDbfRecord record)
  {
    int actionId = record.ActionId;
    List<FixedRewardMapDbfRecord> rewardMapDbfRecordList;
    if (!this.m_fixedRewardsByAction.TryGetValue(actionId, out rewardMapDbfRecordList))
    {
      rewardMapDbfRecordList = new List<FixedRewardMapDbfRecord>();
      this.m_fixedRewardsByAction.Add(actionId, rewardMapDbfRecordList);
    }
    rewardMapDbfRecordList.Add(record);
  }

  public void OnFixedRewardMapRemoved(List<FixedRewardMapDbfRecord> removedRecords)
  {
    HashSet<int> removedIds = new HashSet<int>(removedRecords.Select<FixedRewardMapDbfRecord, int>((Func<FixedRewardMapDbfRecord, int>) (r => r.ID)));
    foreach (int key in new HashSet<int>(removedRecords.Select<FixedRewardMapDbfRecord, int>((Func<FixedRewardMapDbfRecord, int>) (r => r.ActionId))))
    {
      List<FixedRewardMapDbfRecord> rewardMapDbfRecordList;
      if (this.m_fixedRewardsByAction.TryGetValue(key, out rewardMapDbfRecordList))
        rewardMapDbfRecordList.RemoveAll((Predicate<FixedRewardMapDbfRecord>) (r => removedIds.Contains(r.ID)));
    }
  }

  public void PostProcessDbfLoad_FixedRewardAction(Dbf<FixedRewardActionDbfRecord> dbf)
  {
    this.m_fixedActionRecordsByType.Clear();
    foreach (FixedRewardActionDbfRecord record in dbf.GetRecords())
      this.OnFixedRewardActionAdded(record);
  }

  public void OnFixedRewardActionAdded(FixedRewardActionDbfRecord record)
  {
    FixedRewardAction.Type type = record.Type;
    List<FixedRewardActionDbfRecord> rewardActionDbfRecordList;
    if (!this.m_fixedActionRecordsByType.TryGetValue(type, out rewardActionDbfRecordList))
    {
      rewardActionDbfRecordList = new List<FixedRewardActionDbfRecord>();
      this.m_fixedActionRecordsByType.Add(type, rewardActionDbfRecordList);
    }
    rewardActionDbfRecordList.Add(record);
  }

  public void OnFixedRewardActionRemoved(List<FixedRewardActionDbfRecord> removedRecords)
  {
    HashSet<int> removedIds = new HashSet<int>(removedRecords.Select<FixedRewardActionDbfRecord, int>((Func<FixedRewardActionDbfRecord, int>) (r => r.ID)));
    HashSet<FixedRewardAction.Type> typeSet;
    try
    {
      typeSet = new HashSet<FixedRewardAction.Type>(removedRecords.Select<FixedRewardActionDbfRecord, FixedRewardAction.Type>((Func<FixedRewardActionDbfRecord, FixedRewardAction.Type>) (r => EnumUtils.GetEnum<FixedRewardAction.Type>(r.Type.ToString()))));
    }
    catch
    {
      Debug.LogErrorFormat("Error parsing FixedRewardAction.Type, type did not match a FixedRewardType: {0}", (object) string.Join(", ", removedRecords.Select<FixedRewardActionDbfRecord, string>((Func<FixedRewardActionDbfRecord, string>) (r => r.Type.ToString())).ToArray<string>()));
      typeSet = new HashSet<FixedRewardAction.Type>();
    }
    foreach (FixedRewardAction.Type key in typeSet)
    {
      List<FixedRewardActionDbfRecord> rewardActionDbfRecordList;
      if (this.m_fixedActionRecordsByType.TryGetValue(key, out rewardActionDbfRecordList))
        rewardActionDbfRecordList.RemoveAll((Predicate<FixedRewardActionDbfRecord>) (r => removedIds.Contains(r.ID)));
    }
  }

  public void PostProcessDbfLoad_DeckRulesetRuleSubset(Dbf<DeckRulesetRuleSubsetDbfRecord> dbf)
  {
    this.m_subsetsReferencedByRuleId.Clear();
    foreach (DeckRulesetRuleSubsetDbfRecord record in dbf.GetRecords())
      this.OnDeckRulesetRuleSubsetAdded(record);
  }

  public void OnDeckRulesetRuleSubsetAdded(DeckRulesetRuleSubsetDbfRecord record)
  {
    int deckRulesetRuleId = record.DeckRulesetRuleId;
    int subsetId = record.SubsetId;
    List<int> intList;
    if (!this.m_subsetsReferencedByRuleId.TryGetValue(deckRulesetRuleId, out intList))
    {
      intList = new List<int>();
      this.m_subsetsReferencedByRuleId[deckRulesetRuleId] = intList;
    }
    intList.Add(subsetId);
  }

  public void OnDeckRulesetRuleSubsetRemoved(
    List<DeckRulesetRuleSubsetDbfRecord> removedRecords)
  {
    foreach (DeckRulesetRuleSubsetDbfRecord removedRecord in removedRecords)
    {
      DeckRulesetRuleSubsetDbfRecord rec = removedRecord;
      List<int> intList;
      if (this.m_subsetsReferencedByRuleId.TryGetValue(rec.DeckRulesetRuleId, out intList))
        intList.RemoveAll((Predicate<int>) (subsetId => subsetId == rec.SubsetId));
    }
  }

  public void PostProcessDbfLoad_SubsetCard(Dbf<SubsetCardDbfRecord> dbf)
  {
    this.m_subsetCards.Clear();
    foreach (SubsetCardDbfRecord record in dbf.GetRecords())
      this.OnSubsetCardAdded(record);
  }

  public void OnSubsetCardAdded(SubsetCardDbfRecord record)
  {
    int subsetId = record.SubsetId;
    int cardId = record.CardId;
    CardDbfRecord record1 = GameDbf.Card.GetRecord(cardId);
    if (record1 == null)
      return;
    HashSet<string> stringSet;
    if (!this.m_subsetCards.TryGetValue(subsetId, out stringSet))
    {
      stringSet = new HashSet<string>();
      this.m_subsetCards[subsetId] = stringSet;
    }
    stringSet.Add(record1.NoteMiniGuid);
  }

  public void OnSubsetCardRemoved(List<SubsetCardDbfRecord> removedRecords)
  {
    foreach (SubsetCardDbfRecord removedRecord in removedRecords)
    {
      HashSet<string> stringSet;
      if (this.m_subsetCards.TryGetValue(removedRecord.SubsetId, out stringSet) && stringSet != null)
      {
        CardDbfRecord record = GameDbf.Card.GetRecord(removedRecord.CardId);
        if (record != null && record.NoteMiniGuid != null)
          stringSet.Remove(record.NoteMiniGuid);
      }
    }
  }

  public void PostProcessDbfLoad_DeckRulesetRule(Dbf<DeckRulesetRuleDbfRecord> dbf)
  {
    this.m_rulesByDeckRulesetId.Clear();
    foreach (DeckRulesetRuleDbfRecord record in dbf.GetRecords())
      this.OnDeckRulesetRuleAdded(record);
  }

  public void OnDeckRulesetRuleAdded(DeckRulesetRuleDbfRecord record)
  {
    HashSet<int> intSet;
    if (!this.m_rulesByDeckRulesetId.TryGetValue(record.DeckRulesetId, out intSet))
    {
      intSet = new HashSet<int>();
      this.m_rulesByDeckRulesetId[record.DeckRulesetId] = intSet;
    }
    intSet.Add(record.ID);
  }

  public void OnDeckRulesetRuleRemoved(List<DeckRulesetRuleDbfRecord> removedRecords)
  {
    foreach (DeckRulesetRuleDbfRecord removedRecord in removedRecords)
    {
      HashSet<int> intSet;
      if (this.m_rulesByDeckRulesetId.TryGetValue(removedRecord.DeckRulesetId, out intSet))
        intSet.Remove(removedRecord.ID);
    }
  }

  public void PostProcessDbfLoad_CardChange(Dbf<CardChangeDbfRecord> dbf)
  {
    this.m_cardChangesByCardId.Clear();
    foreach (CardChangeDbfRecord record in dbf.GetRecords())
      this.OnCardChangeAdded(record);
  }

  public void OnCardChangeAdded(CardChangeDbfRecord record)
  {
    List<CardChangeDbfRecord> cardChangeDbfRecordList;
    if (!this.m_cardChangesByCardId.TryGetValue(record.CardId, out cardChangeDbfRecordList))
    {
      cardChangeDbfRecordList = new List<CardChangeDbfRecord>();
      this.m_cardChangesByCardId[record.CardId] = cardChangeDbfRecordList;
    }
    cardChangeDbfRecordList.Add(record);
  }

  public void OnCardChangeRemoved(List<CardChangeDbfRecord> removedRecords)
  {
    foreach (CardChangeDbfRecord removedRecord in removedRecords)
    {
      List<CardChangeDbfRecord> cardChangeDbfRecordList;
      if (this.m_cardChangesByCardId.TryGetValue(removedRecord.CardId, out cardChangeDbfRecordList))
        cardChangeDbfRecordList.Remove(removedRecord);
    }
  }

  public void PostProcessDbfLoad_CardPlayerDeckOverride(Dbf<CardPlayerDeckOverrideDbfRecord> dbf)
  {
    this.m_cardsWithPlayerDeckOverrides.Clear();
    foreach (CardPlayerDeckOverrideDbfRecord record in dbf.GetRecords())
      this.OnCardPlayerDeckOverrideAdded(record);
  }

  public void OnCardPlayerDeckOverrideAdded(CardPlayerDeckOverrideDbfRecord record) => this.m_cardsWithPlayerDeckOverrides[record.CardId] = record.ID;

  public void OnCardPlayerDeckOverrideRemoved(
    List<CardPlayerDeckOverrideDbfRecord> removedRecords)
  {
    foreach (CardPlayerDeckOverrideDbfRecord removedRecord in removedRecords)
      this.m_cardsWithPlayerDeckOverrides.Remove(removedRecord.CardId);
  }

  public CardDbfRecord GetCardRecord(string cardId)
  {
    if (string.IsNullOrEmpty(cardId))
      return (CardDbfRecord) null;
    if (cardId == "PlaceholderCard")
      this.CachePlaceholderRecord();
    CardDbfRecord cardRecord = (CardDbfRecord) null;
    this.m_cardsByCardId.TryGetValue(cardId, out cardRecord);
    return cardRecord;
  }

  public List<CardChangeDbfRecord> GetCardChangeRecords(int cardId)
  {
    List<CardChangeDbfRecord> cardChangeRecords = (List<CardChangeDbfRecord>) null;
    this.m_cardChangesByCardId.TryGetValue(cardId, out cardChangeRecords);
    return cardChangeRecords;
  }

  public CardSetDbfRecord GetCardSet(TAG_CARD_SET cardSetId) => GameDbf.CardSet.GetRecord((int) cardSetId);

  public string GetCardSetSpellOverride(TAG_CARD_SET cardSetId, SpellType spellType)
  {
    Map<SpellType, string> map = (Map<SpellType, string>) null;
    if (this.m_spellOverridesByCardSetId.TryGetValue((int) cardSetId, out map))
    {
      string setSpellOverride = (string) null;
      if (map.TryGetValue(spellType, out setSpellOverride))
        return setSpellOverride;
    }
    return (string) null;
  }

  public void PostProcessDbfLoad_CardSetSpellOverride(Dbf<CardSetSpellOverrideDbfRecord> dbf)
  {
    this.m_spellOverridesByCardSetId.Clear();
    foreach (CardSetSpellOverrideDbfRecord record in dbf.GetRecords())
      this.OnCardSetSpellOverrideAdded(record);
  }

  public void OnCardSetSpellOverrideAdded(CardSetSpellOverrideDbfRecord record)
  {
    Map<SpellType, string> map;
    if (!this.m_spellOverridesByCardSetId.TryGetValue(record.CardSetId, out map))
    {
      map = new Map<SpellType, string>();
      this.m_spellOverridesByCardSetId[record.CardSetId] = map;
    }
    SpellType key = (SpellType) Enum.Parse(typeof (SpellType), record.SpellType);
    if (!Enum.IsDefined(typeof (SpellType), (object) key))
      return;
    map.Add(key, record.OverridePrefab);
  }

  public void OnCardSetSpellOverrideRemoved(List<CardSetSpellOverrideDbfRecord> removedRecords)
  {
    foreach (CardSetSpellOverrideDbfRecord removedRecord in removedRecords)
    {
      Map<SpellType, string> map;
      if (this.m_spellOverridesByCardSetId.TryGetValue(removedRecord.CardSetId, out map))
      {
        SpellType key = (SpellType) Enum.Parse(typeof (SpellType), removedRecord.SpellType);
        if (Enum.IsDefined(typeof (SpellType), (object) key))
        {
          map.Remove(key);
          if (map.Count == 0)
            this.m_spellOverridesByCardSetId.Remove(removedRecord.CardSetId);
        }
      }
    }
  }

  public string GetCardDiscoverString(string cardId)
  {
    if (string.IsNullOrEmpty(cardId))
      return (string) null;
    CardDiscoverStringDbfRecord discoverStringDbfRecord = (CardDiscoverStringDbfRecord) null;
    return this.m_cardDiscoverStringsByCardId.TryGetValue(cardId, out discoverStringDbfRecord) ? discoverStringDbfRecord.StringId : (string) null;
  }

  public string GetClientString(int recordId)
  {
    ClientStringDbfRecord record = GameDbf.ClientString.GetRecord(recordId);
    return (string) (record == null ? (DbfLocValue) null : record.Text);
  }

  private void CachePlaceholderRecord()
  {
    if (this.m_cardsByCardId.ContainsKey("PlaceholderCard"))
      return;
    CardDbfRecord cardDbfRecord = new CardDbfRecord();
    cardDbfRecord.SetID(-1);
    cardDbfRecord.SetNoteMiniGuid("PlaceholderCard");
    DbfLocValue v1 = new DbfLocValue();
    v1.SetString(Locale.enUS, "Placeholder Card");
    cardDbfRecord.SetName(v1);
    DbfLocValue v2 = new DbfLocValue();
    v2.SetString(Locale.enUS, "Battlecry: Someone remembers to publish this card.");
    cardDbfRecord.SetTextInHand(v2);
    Dictionary<GAME_TAG, int> dictionary = new Dictionary<GAME_TAG, int>();
    dictionary.Add(GAME_TAG.CARD_SET, 7);
    dictionary.Add(GAME_TAG.CARDTYPE, 4);
    dictionary.Add(GAME_TAG.CLASS, 4);
    dictionary.Add(GAME_TAG.RARITY, 4);
    dictionary.Add(GAME_TAG.FACTION, 3);
    dictionary.Add(GAME_TAG.COST, 9);
    dictionary.Add(GAME_TAG.HEALTH, 8);
    dictionary.Add(GAME_TAG.ATK, 6);
    List<CardTagDbfRecord> cardTagDbfRecordList = new List<CardTagDbfRecord>();
    foreach (KeyValuePair<GAME_TAG, int> keyValuePair in dictionary)
    {
      CardTagDbfRecord cardTagDbfRecord = new CardTagDbfRecord();
      cardTagDbfRecord.SetCardId(cardDbfRecord.ID);
      cardTagDbfRecord.SetTagId((int) keyValuePair.Key);
      cardTagDbfRecord.SetTagValue(keyValuePair.Value);
      cardTagDbfRecordList.Add(cardTagDbfRecord);
    }
    this.m_cardsByCardId.Add("PlaceholderCard", cardDbfRecord);
    this.m_cardTagsByCardDbId.Add(cardDbfRecord.ID, cardTagDbfRecordList);
  }

  public int GetCollectibleCardCount() => this.m_collectibleCardCount;

  public List<string> GetAllCardIds() => this.m_allCardIds;

  public List<int> GetAllCardDbIds() => this.m_allCardDbIds;

  public List<string> GetCollectibleCardIds() => this.m_collectibleCardIds;

  public List<int> GetCollectibleCardDbIds() => this.m_collectibleCardDbIds;

  public HashSet<CardDbfRecord> GetCardsWithFeaturedCardsEvent() => this.m_featuredCardEventCards;

  public FixedRewardDbfRecord GetFixedRewardRecordsForCardId(
    int cardId,
    int premiumType)
  {
    FixedRewardDbfRecord recordsForCardId = (FixedRewardDbfRecord) null;
    this.m_fixedRewardsByCardId.TryGetValue((cardId, premiumType), out recordsForCardId);
    return recordsForCardId;
  }

  public List<FixedRewardMapDbfRecord> GetFixedRewardMapRecordsForAction(
    int actionId)
  {
    List<FixedRewardMapDbfRecord> recordsForAction = (List<FixedRewardMapDbfRecord>) null;
    if (!this.m_fixedRewardsByAction.TryGetValue(actionId, out recordsForAction))
    {
      recordsForAction = new List<FixedRewardMapDbfRecord>();
      this.m_fixedRewardsByAction[actionId] = recordsForAction;
    }
    return recordsForAction;
  }

  public List<FixedRewardActionDbfRecord> GetFixedActionRecordsForType(
    FixedRewardAction.Type type)
  {
    List<FixedRewardActionDbfRecord> actionRecordsForType = (List<FixedRewardActionDbfRecord>) null;
    if (!this.m_fixedActionRecordsByType.TryGetValue(type, out actionRecordsForType))
    {
      actionRecordsForType = new List<FixedRewardActionDbfRecord>();
      this.m_fixedActionRecordsByType[type] = actionRecordsForType;
    }
    return actionRecordsForType;
  }

  public List<HashSet<string>> GetSubsetsForRule(int ruleId)
  {
    List<HashSet<string>> subsetsForRule = new List<HashSet<string>>();
    List<int> intList;
    if (this.m_subsetsReferencedByRuleId.TryGetValue(ruleId, out intList))
    {
      for (int index = 0; index < intList.Count; ++index)
        subsetsForRule.Add(this.GetSubsetById(intList[index]));
    }
    return subsetsForRule;
  }

  public List<int> GetCardSetIdsForSubsetRule(int ruleId)
  {
    List<int> intList = new List<int>();
    List<int> idsForSubsetRule = new List<int>();
    if (this.m_subsetsReferencedByRuleId.TryGetValue(ruleId, out intList))
    {
      foreach (int id in intList)
      {
        SubsetDbfRecord record = GameDbf.Subset.GetRecord(id);
        if (record != null)
        {
          foreach (SubsetRuleDbfRecord rule in record.Rules)
          {
            if (rule.Tag == 183 && !rule.RuleIsNot && rule.MaxValue == rule.MinValue)
              idsForSubsetRule.Add(rule.MaxValue);
          }
        }
      }
    }
    return idsForSubsetRule;
  }

  public DeckRulesetRuleDbfRecord[] GetRulesForDeckRuleset(int deckRulesetId)
  {
    HashSet<int> source;
    if (!this.m_rulesByDeckRulesetId.TryGetValue(deckRulesetId, out source))
      source = new HashSet<int>();
    // ISSUE: object of a compiler-generated type is created
    return source.Select(ruleId => new \u003C\u003Ef__AnonymousType3<int, DeckRulesetRuleDbfRecord>(ruleId, GameDbf.DeckRulesetRule.GetRecord(ruleId))).Where(_param1 => _param1.ruleDbf != null).Select(_param1 => _param1.ruleDbf).ToArray<DeckRulesetRuleDbfRecord>();
  }

  public HashSet<string> GetSubsetById(int id)
  {
    HashSet<string> subsetById = (HashSet<string>) null;
    if (!this.m_subsetCards.TryGetValue(id, out subsetById))
    {
      subsetById = new HashSet<string>();
      this.m_subsetCards[id] = subsetById;
    }
    return subsetById;
  }

  public IEnumerable<CardPlayerDeckOverrideDbfRecord> GetAllCardPlayerDeckOverrides() => this.m_cardsWithPlayerDeckOverrides.Select<KeyValuePair<int, int>, CardPlayerDeckOverrideDbfRecord>((Func<KeyValuePair<int, int>, CardPlayerDeckOverrideDbfRecord>) (kv => GameDbf.CardPlayerDeckOverride.GetRecord(kv.Value)));

  public bool HasCardPlayerDeckOverride(string cardId) => this.m_cardsWithPlayerDeckOverrides.TryGetValue(GameUtils.TranslateCardIdToDbId(cardId), out int _);

  public CardPlayerDeckOverrideDbfRecord GetCardPlayerDeckOverride(
    string cardId)
  {
    int id;
    return !this.m_cardsWithPlayerDeckOverrides.TryGetValue(GameUtils.TranslateCardIdToDbId(cardId), out id) ? (CardPlayerDeckOverrideDbfRecord) null : GameDbf.CardPlayerDeckOverride.GetRecord(id);
  }

  public void PostProcessDbfLoad_LettuceEquipmentTier()
  {
    this.m_equipmentTierByCardId.Clear();
    foreach (LettuceEquipmentTierDbfRecord record in GameDbf.LettuceEquipmentTier.GetRecords())
      this.OnLettuceEquipmentTierAdded(record);
  }

  public void OnLettuceEquipmentTierAdded(LettuceEquipmentTierDbfRecord tier)
  {
    this.m_equipmentTierByCardId[tier.CardId] = tier;
    List<BonusBountyDropChanceDbfRecord> bountyDropChances = tier.BonusBountyDropChances;
    if (bountyDropChances == null)
      return;
    int index = 0;
    for (int count = bountyDropChances.Count; index < count; ++index)
    {
      BonusBountyDropChanceDbfRecord dropChanceDbfRecord = bountyDropChances[index];
      if (dropChanceDbfRecord.LettuceBountyRecord != null)
      {
        int lettuceEquipmentId = tier.LettuceEquipmentId;
        MercenaryUnlock mercenaryUnlock;
        if (this.m_equipmentUnlockByEquipmentId.TryGetValue(lettuceEquipmentId, out mercenaryUnlock))
          Log.Lettuce.PrintError(string.Format("GameDbFIndex.OnLettuceEquipmentTierAdded(): EquipmentID [{0}] is already unlocked by {1}", (object) lettuceEquipmentId, (object) mercenaryUnlock));
        else
          this.m_equipmentUnlockByEquipmentId.Add(lettuceEquipmentId, MercenaryUnlock.Create(dropChanceDbfRecord.LettuceBountyRecord));
      }
    }
  }

  public void OnLettuceEquipmentTierRemoved(List<LettuceEquipmentTierDbfRecord> removedRecords)
  {
    foreach (LettuceEquipmentTierDbfRecord removedRecord in removedRecords)
      this.m_equipmentTierByCardId.Remove(removedRecord.CardId);
  }

  public LettuceEquipmentTierDbfRecord GetEquipmentTierFromCardID(
    int cardId)
  {
    if (this.m_equipmentTierByCardId.ContainsKey(cardId))
      return this.m_equipmentTierByCardId[cardId];
    Log.Lettuce.PrintError(string.Format("Missing LETTUCE_EQUIPMENT_TIER record for CARD database ID: {0}. Did you forget to create it in HearthEdit 2?", (object) cardId));
    return (LettuceEquipmentTierDbfRecord) null;
  }

  public void PostProcessDbfLoad_VisitorTask()
  {
    foreach (MercenaryVisitorDbfRecord record in GameDbf.MercenaryVisitor.GetRecords())
    {
      List<VisitorTaskChainDbfRecord> visitorTaskChains = record.VisitorTaskChains;
      if (visitorTaskChains != null && visitorTaskChains.Count > 0)
      {
        List<TaskListDbfRecord> taskList = visitorTaskChains.Last<VisitorTaskChainDbfRecord>()?.TaskList;
        if (taskList != null)
        {
          for (int index = 0; index < taskList.Count; ++index)
          {
            this.OnVisitorTaskAdded(taskList[index].TaskRecord, index);
            this.m_MercenariesTaskIndex[taskList[index].TaskRecord.ID] = index;
          }
        }
      }
    }
  }

  public void OnVisitorTaskAdded(VisitorTaskDbfRecord record, int taskIndex)
  {
    RewardListDbfRecord rewardListRecord = record.RewardListRecord;
    if (rewardListRecord == null)
      return;
    List<RewardItemDbfRecord> rewardItems = rewardListRecord.RewardItems;
    if (rewardItems == null)
      return;
    foreach (RewardItemDbfRecord rewardItemDbfRecord in rewardItems)
    {
      switch (rewardItemDbfRecord.RewardType)
      {
        case RewardItem.RewardType.MERCENARY_EQUIPMENT:
          int mercenaryEquipment = rewardItemDbfRecord.MercenaryEquipment;
          if (mercenaryEquipment != 0)
          {
            MercenaryUnlock mercenaryUnlock;
            if (this.m_equipmentUnlockByEquipmentId.TryGetValue(rewardItemDbfRecord.MercenaryEquipment, out mercenaryUnlock))
            {
              Log.Lettuce.PrintError(string.Format("GameDbFIndex.OnVisitorTaskAdded(): EquipmentID [{0}] is already unlocked by {1}", (object) mercenaryEquipment, (object) mercenaryUnlock));
              continue;
            }
            this.m_equipmentUnlockByEquipmentId.Add(mercenaryEquipment, MercenaryUnlock.Create(record, taskIndex));
            continue;
          }
          continue;
        case RewardItem.RewardType.MERCENARY:
          if (rewardItemDbfRecord.MercenaryArtVariationRecord != null)
          {
            MercenaryUnlock newUnlock = MercenaryUnlock.Create(record, taskIndex);
            foreach (MercenaryArtVariationPremiumDbfRecord variationPremium in rewardItemDbfRecord.MercenaryArtVariationRecord.MercenaryArtVariationPremiums)
            {
              if (variationPremium.Premium == (MercenaryArtVariationPremium.MercenariesPremium) rewardItemDbfRecord.MercenaryArtPremium && !string.IsNullOrEmpty((string) variationPremium.CustomAcquireText))
              {
                newUnlock.m_unlockType = MercenaryUnlock.UnlockType.Custom;
                newUnlock.m_customAcquireText = (string) variationPremium.CustomAcquireText;
              }
            }
            MercenaryUnlock mercenaryUnlock = this.AddArtVariationUnlock(rewardItemDbfRecord.MercenaryArtVariation, rewardItemDbfRecord.MercenaryArtPremium, newUnlock);
            if (mercenaryUnlock != null)
            {
              Log.Lettuce.PrintError(string.Format("GameDbFIndex.OnVisitorTaskAdded(): ArtVariationID [{0}][{1}] is already unlocked by {2}", (object) rewardItemDbfRecord.MercenaryArtVariation, (object) rewardItemDbfRecord.MercenaryArtPremium, (object) mercenaryUnlock));
              continue;
            }
            continue;
          }
          if (rewardItemDbfRecord.MercenaryRecord != null)
          {
            using (List<MercenaryArtVariationDbfRecord>.Enumerator enumerator = rewardItemDbfRecord.MercenaryRecord.MercenaryArtVariations.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                MercenaryArtVariationDbfRecord current = enumerator.Current;
                if (current.DefaultVariation)
                {
                  MercenaryUnlock newUnlock = MercenaryUnlock.Create(record, taskIndex);
                  foreach (MercenaryArtVariationPremiumDbfRecord variationPremium in current.MercenaryArtVariationPremiums)
                  {
                    if (variationPremium.Premium == MercenaryArtVariationPremium.MercenariesPremium.PREMIUM_NORMAL && !string.IsNullOrEmpty((string) variationPremium.CustomAcquireText))
                    {
                      newUnlock.m_unlockType = MercenaryUnlock.UnlockType.Custom;
                      newUnlock.m_customAcquireText = (string) variationPremium.CustomAcquireText;
                    }
                  }
                  this.AddArtVariationUnlock(current.ID, RewardItem.MercenariesPremium.PREMIUM_NORMAL, newUnlock);
                  break;
                }
              }
              continue;
            }
          }
          else
            continue;
        default:
          continue;
      }
    }
  }

  public int GetTaskChainIndexForTask(int taskID) => this.m_MercenariesTaskIndex.ContainsKey(taskID) ? this.m_MercenariesTaskIndex[taskID] : -1;

  public void PostProcessDbfLoad_Achievement()
  {
    foreach (AchievementDbfRecord record in GameDbf.Achievement.GetRecords())
      this.OnAchievementAdded(record);
  }

  public void OnAchievementAdded(AchievementDbfRecord record)
  {
    RewardListDbfRecord rewardListRecord = record.RewardListRecord;
    if (rewardListRecord == null)
      return;
    List<RewardItemDbfRecord> rewardItems = rewardListRecord.RewardItems;
    if (rewardItems == null)
      return;
    foreach (RewardItemDbfRecord rewardItemDbfRecord in rewardItems)
    {
      if (rewardItemDbfRecord.MercenaryEquipment != 0)
      {
        int mercenaryEquipment = rewardItemDbfRecord.MercenaryEquipment;
        MercenaryUnlock mercenaryUnlock;
        if (this.m_equipmentUnlockByEquipmentId.TryGetValue(mercenaryEquipment, out mercenaryUnlock))
          Log.Lettuce.PrintError(string.Format("GameDbFIndex.OnAchievementAdded(): EquipmentID [{0}] is already unlocked by {1}", (object) mercenaryEquipment, (object) mercenaryUnlock));
        else
          this.m_equipmentUnlockByEquipmentId.Add(mercenaryEquipment, MercenaryUnlock.Create(record));
      }
    }
  }

  public void PostProcessDbfLoad_MercenaryArtVariation()
  {
    foreach (MercenaryArtVariationDbfRecord record in GameDbf.MercenaryArtVariation.GetRecords())
    {
      foreach (MercenaryArtVariationPremiumDbfRecord variationPremium in record.MercenaryArtVariationPremiums)
      {
        if (!string.IsNullOrEmpty((string) variationPremium.CustomAcquireText))
        {
          MercenaryUnlock newUnlock = new MercenaryUnlock(MercenaryUnlock.UnlockType.Custom, (string) variationPremium.CustomAcquireText);
          MercenaryUnlock mercenaryUnlock = this.AddArtVariationUnlock(record.ID, variationPremium.Premium, newUnlock);
          if (mercenaryUnlock != null)
            Log.Lettuce.PrintError(string.Format("GameDbFIndex.PostProcessDbfLoad_MercenaryArtVariation(): ArtVariationID [{0}][{1}] is already unlocked by {2}", (object) record.ID, (object) variationPremium.Premium, (object) mercenaryUnlock));
        }
        else if (variationPremium.RewardTrack)
        {
          MercenaryUnlock newUnlock = new MercenaryUnlock(MercenaryUnlock.UnlockType.RewardTrack);
          MercenaryUnlock mercenaryUnlock = this.AddArtVariationUnlock(record.ID, variationPremium.Premium, newUnlock);
          if (mercenaryUnlock != null)
            Log.Lettuce.PrintError(string.Format("GameDbFIndex.PostProcessDbfLoad_MercenaryArtVariation(): ArtVariationID [{0}][{1}] is already unlocked by {2}", (object) record.ID, (object) variationPremium.Premium, (object) mercenaryUnlock));
        }
      }
    }
  }

  public void PostProcessDbfLoad_MercenaryEquipmentUnlock() => this.m_equipmentUnlockByEquipmentId.Clear();

  public MercenaryUnlock GetEquipmentUnlockFromEquipmentID(int equipmentId)
  {
    MercenaryUnlock unlockFromEquipmentId;
    if (this.m_equipmentUnlockByEquipmentId.TryGetValue(equipmentId, out unlockFromEquipmentId))
      return unlockFromEquipmentId;
    Log.Lettuce.PrintError(string.Format("Missing: MercenaryEquipmentUnlock not found for EQUIPMENT database ID: {0}", (object) equipmentId));
    return (MercenaryUnlock) null;
  }

  public void PostProcessDbfLoad_MercenaryArtVariationUnlock() => this.m_UnlockByArtVariationIdAndPremium.Clear();

  public MercenaryUnlock GetArtVariationUnlock(
    int artVariationId,
    TAG_PREMIUM tagPremium)
  {
    Map<TAG_PREMIUM, MercenaryUnlock> map;
    MercenaryUnlock mercenaryUnlock;
    return this.m_UnlockByArtVariationIdAndPremium.TryGetValue(artVariationId, out map) && map.TryGetValue(tagPremium, out mercenaryUnlock) ? mercenaryUnlock : MercenaryUnlock.FromPacks;
  }

  private MercenaryUnlock AddArtVariationUnlock(
    int artVariationId,
    TAG_PREMIUM tagPremium,
    MercenaryUnlock newUnlock)
  {
    Map<TAG_PREMIUM, MercenaryUnlock> map;
    if (!this.m_UnlockByArtVariationIdAndPremium.TryGetValue(artVariationId, out map))
    {
      map = new Map<TAG_PREMIUM, MercenaryUnlock>();
      this.m_UnlockByArtVariationIdAndPremium.Add(artVariationId, map);
    }
    MercenaryUnlock mercenaryUnlock;
    if (map.TryGetValue(tagPremium, out mercenaryUnlock))
      return mercenaryUnlock;
    map.Add(tagPremium, newUnlock);
    return (MercenaryUnlock) null;
  }

  private MercenaryUnlock AddArtVariationUnlock(
    int artVariationId,
    RewardItem.MercenariesPremium premium,
    MercenaryUnlock newUnlock)
  {
    TAG_PREMIUM tagPremium = TAG_PREMIUM.NORMAL;
    switch (premium)
    {
      case RewardItem.MercenariesPremium.PREMIUM_GOLDEN:
        tagPremium = TAG_PREMIUM.GOLDEN;
        break;
      case RewardItem.MercenariesPremium.PREMIUM_DIAMOND:
        tagPremium = TAG_PREMIUM.DIAMOND;
        break;
    }
    return this.AddArtVariationUnlock(artVariationId, tagPremium, newUnlock);
  }

  private MercenaryUnlock AddArtVariationUnlock(
    int artVariationId,
    MercenaryArtVariationPremium.MercenariesPremium premium,
    MercenaryUnlock newUnlock)
  {
    TAG_PREMIUM tagPremium = TAG_PREMIUM.NORMAL;
    switch (premium)
    {
      case MercenaryArtVariationPremium.MercenariesPremium.PREMIUM_GOLDEN:
        tagPremium = TAG_PREMIUM.GOLDEN;
        break;
      case MercenaryArtVariationPremium.MercenariesPremium.PREMIUM_DIAMOND:
        tagPremium = TAG_PREMIUM.DIAMOND;
        break;
    }
    return this.AddArtVariationUnlock(artVariationId, tagPremium, newUnlock);
  }

  public void PostProcessDbfLoad_MercenaryLevel()
  {
    this.m_maxMercenaryLevel = 0;
    foreach (LettuceMercenaryLevelDbfRecord record in GameDbf.LettuceMercenaryLevel.GetRecords())
      this.m_maxMercenaryLevel = Math.Max(this.m_maxMercenaryLevel, record.Level);
  }

  public void OnMercenaryLevelAdded(LettuceMercenaryLevelDbfRecord record) => this.m_maxMercenaryLevel = Math.Max(this.m_maxMercenaryLevel, record.Level);

  public void OnMercenaryLevelRemoved(List<LettuceMercenaryLevelDbfRecord> records)
  {
    foreach (LettuceMercenaryLevelDbfRecord record in records)
    {
      if (record.Level == this.m_maxMercenaryLevel)
      {
        this.PostProcessDbfLoad_MercenaryLevel();
        break;
      }
    }
  }

  public int GetMercenaryMaxLevel() => this.m_maxMercenaryLevel;
}
