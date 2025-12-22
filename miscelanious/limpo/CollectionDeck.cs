using PegasusShared;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectionDeck
{
  public static int DefaultMaxDeckNameCharacters = 24;
  public static List<DeckRule.RuleType> DefaultIgnoreRules = new List<DeckRule.RuleType>()
  {
    DeckRule.RuleType.PLAYER_OWNS_EACH_COPY,
    DeckRule.RuleType.IS_CARD_PLAYABLE,
    DeckRule.RuleType.HAS_TAG_VALUE
  };
  private int m_changeNumber;
  private string m_name;
  private List<CollectionDeckSlot> m_slots = new List<CollectionDeckSlot>();
  private bool m_netContentsLoaded;
  private bool m_isSavingContentChanges;
  private bool m_isSavingNameChanges;
  private bool m_isBeingDeleted;
  private string m_randomHeroCardId = "None";
  private string m_currentDisplayHeroCardId = "None";
  private ShareableDeck m_createdFromShareableDeck;
  public long ID;
  public DeckType Type = DeckType.NORMAL_DECK;
  public bool IsLoanerDeck;
  public string HeroCardID = string.Empty;
  public bool HeroOverridden;
  public bool RandomHeroUseFavorite = true;
  public int? CardBackID;
  public int SeasonId;
  public int BrawlLibraryItemId;
  public bool NeedsName;
  public long SortOrder;
  public ulong CreateDate;
  public bool Locked;
  public DeckSourceType SourceType;
  public string HeroPowerCardID = string.Empty;
  public string UIHeroOverrideCardID = string.Empty;
  public TAG_PREMIUM UIHeroOverridePremium;
  public int DeckTemplateId;
  private readonly RuneType[] m_runeOrder = new RuneType[DeckRule_DeathKnightRuneLimit.MaxRuneSlots];

  public override string ToString() => string.Format("Deck [id={0} name=\"{1}\" heroCardId={2} cardBackId={3} ", (object) this.ID, (object) this.Name, (object) this.HeroCardID, (object) this.CardBackID) + string.Format("heroOverridden={0} slotCount={1} needsName={2} sortOrder={3}]", (object) this.HeroOverridden, (object) this.GetSlotCount(), (object) this.NeedsName, (object) this.SortOrder);

  public string Name
  {
    get => this.m_name;
    set
    {
      if (value == null)
      {
        Debug.LogError((object) string.Format("CollectionDeck.SetName() - null name given for deck {0}", (object) this));
      }
      else
      {
        if (value.Equals(this.m_name, StringComparison.InvariantCultureIgnoreCase))
          return;
        this.m_name = value;
      }
    }
  }

  public RunePattern Runes { get; private set; }

  public void SetRuneAtIndex(int index, RuneType runeType)
  {
    if (index < 0 || index >= this.m_runeOrder.Length)
    {
      Debug.LogWarning((object) string.Format("CollectionDeck: SetRuneAtIndex: index {0} is out of range of {1}", (object) index, (object) this.m_runeOrder.Length));
    }
    else
    {
      this.m_runeOrder[index] = runeType;
      this.Runes = new RunePattern(this.m_runeOrder);
    }
  }

  public RuneType GetRuneAtIndex(int index)
  {
    if (index >= 0 && index < this.m_runeOrder.Length)
      return this.m_runeOrder[index];
    Debug.LogWarning((object) string.Format("CollectionDeck: GetRuneAtIndex: index {0} is out of range of {1}", (object) index, (object) this.m_runeOrder.Length));
    return RuneType.RT_NONE;
  }

  public RuneType[] GetRuneOrder()
  {
    RuneType[] runeOrder = new RuneType[this.m_runeOrder.Length];
    for (int index = 0; index < this.m_runeOrder.Length; ++index)
      runeOrder[index] = this.m_runeOrder[index];
    return runeOrder;
  }

  public bool IsRuneOrderEqual(RuneType[] otherRuneOrder)
  {
    if (otherRuneOrder == null)
    {
      Debug.LogError((object) "IsRuneOrderEqual() - other rune order is null.");
      return false;
    }
    int maxRuneSlots = DeckRule_DeathKnightRuneLimit.MaxRuneSlots;
    if (this.m_runeOrder.Length != otherRuneOrder.Length)
    {
      Debug.LogError((object) "IsRuneOrderEqual() - rune orders are not the same length.");
      return false;
    }
    if (otherRuneOrder.Length < maxRuneSlots || this.m_runeOrder.Length < maxRuneSlots)
    {
      Debug.LogError((object) "IsRuneOrderEqual() - rune order is less than MaxRuneSlots size");
      return false;
    }
    for (int index = 0; index < maxRuneSlots; ++index)
    {
      if (this.m_runeOrder[index] != otherRuneOrder[index])
        return false;
    }
    return true;
  }

  public void SetRuneOrder(params RuneType[] runeTypes)
  {
    if (runeTypes == null)
    {
      Debug.LogError((object) "SetRuneOrder() - rune types is null.");
    }
    else
    {
      int num = Math.Min(this.m_runeOrder.Length, runeTypes.Length);
      for (int index = 0; index < num; ++index)
        this.m_runeOrder[index] = runeTypes[index];
      this.Runes = new RunePattern(this.m_runeOrder);
    }
  }

  public void ClearRuneOrder() => this.SetRuneOrder(new RuneType[3]);

  public bool HasUIHeroOverride() => !string.IsNullOrEmpty(this.UIHeroOverrideCardID);

  public string GetDisplayHeroCardID(bool rerollFavoriteHero)
  {
    if (this.HasUIHeroOverride())
      this.m_currentDisplayHeroCardId = this.UIHeroOverrideCardID;
    else if (this.HeroOverridden || this.IsDuelsDeck)
      this.m_currentDisplayHeroCardId = this.HeroCardID;
    else if (rerollFavoriteHero || this.m_randomHeroCardId == "None")
    {
      int dbId = GameUtils.TranslateCardIdToDbId(this.m_currentDisplayHeroCardId);
      int heroIdOwnedByPlayer = CollectionManager.Get().GetRandomHeroIdOwnedByPlayer(this.GetClass(), this.RandomHeroUseFavorite, new int?(dbId));
      if (heroIdOwnedByPlayer > 0)
        this.m_randomHeroCardId = GameUtils.TranslateDbIdToCardId(heroIdOwnedByPlayer);
      this.m_currentDisplayHeroCardId = this.m_randomHeroCardId;
    }
    return this.m_currentDisplayHeroCardId;
  }

  public TAG_PREMIUM? GetDisplayHeroPremiumOverride() => this.HasUIHeroOverride() ? new TAG_PREMIUM?(this.UIHeroOverridePremium) : new TAG_PREMIUM?();

  public List<string> GetCardsWithCardID()
  {
    List<string> cardsWithCardId = new List<string>();
    for (int index1 = 0; index1 < this.m_slots.Count; ++index1)
    {
      for (int index2 = 0; index2 < this.m_slots[index1].Count; ++index2)
        cardsWithCardId.Add(this.m_slots[index1].CardID);
    }
    return cardsWithCardId;
  }

  public List<CardWithPremiumStatus> GetCardsWithPremiumStatus()
  {
    List<CardWithPremiumStatus> withPremiumStatus1 = new List<CardWithPremiumStatus>();
    for (int index1 = 0; index1 < this.m_slots.Count; ++index1)
    {
      long dbId = (long) GameUtils.TranslateCardIdToDbId(this.m_slots[index1].CardID);
      int count1 = this.m_slots[index1].GetCount(TAG_PREMIUM.DIAMOND);
      int count2 = this.m_slots[index1].GetCount(TAG_PREMIUM.SIGNATURE);
      int count3 = this.m_slots[index1].GetCount(TAG_PREMIUM.GOLDEN);
      int count4 = this.m_slots[index1].GetCount(TAG_PREMIUM.NORMAL);
      for (int index2 = 0; index2 < count1; ++index2)
      {
        CardWithPremiumStatus withPremiumStatus2 = new CardWithPremiumStatus(dbId, TAG_PREMIUM.DIAMOND);
        withPremiumStatus1.Add(withPremiumStatus2);
      }
      for (int index3 = 0; index3 < count2; ++index3)
      {
        CardWithPremiumStatus withPremiumStatus3 = new CardWithPremiumStatus(dbId, TAG_PREMIUM.SIGNATURE);
        withPremiumStatus1.Add(withPremiumStatus3);
      }
      for (int index4 = 0; index4 < count3; ++index4)
      {
        CardWithPremiumStatus withPremiumStatus4 = new CardWithPremiumStatus(dbId, TAG_PREMIUM.GOLDEN);
        withPremiumStatus1.Add(withPremiumStatus4);
      }
      for (int index5 = 0; index5 < count4; ++index5)
      {
        CardWithPremiumStatus withPremiumStatus5 = new CardWithPremiumStatus(dbId, TAG_PREMIUM.NORMAL);
        withPremiumStatus1.Add(withPremiumStatus5);
      }
    }
    return withPremiumStatus1;
  }

  public FormatType FormatType { get; set; }

  public bool IsShared { get; set; }

  public bool IsCreatedWithDeckComplete { get; set; }

  public bool IsBrawlDeck => TavernBrawlManager.IsBrawlDeckType(this.Type);

  public bool IsDuelsDeck => this.Type == DeckType.PVPDR_DECK || this.Type == DeckType.PVPDR_DISPLAY_DECK;

  public bool IsConstructedDeck => this.Type == DeckType.NORMAL_DECK;

  public bool IsValidForRuleset
  {
    get
    {
      if (this.IsShared)
        return true;
      if (!this.m_netContentsLoaded && this.Type != DeckType.CLIENT_ONLY_DECK && this.Type != DeckType.PVPDR_DISPLAY_DECK && !this.IsLoanerDeck)
        return false;
      DeckRuleset ruleset = this.GetRuleset();
      if (ruleset == null)
        return false;
      if (!this.IsLoanerDeck)
        return ruleset.IsDeckValid(this);
      return ruleset.IsDeckValid(this, DeckRule.RuleType.PLAYER_OWNS_EACH_COPY);
    }
  }

  public void MarkNetworkContentsLoaded() => this.m_netContentsLoaded = true;

  public bool NetworkContentsLoaded() => this.m_netContentsLoaded;

  public void MarkBeingDeleted() => this.m_isBeingDeleted = true;

  public bool IsBeingDeleted() => this.m_isBeingDeleted;

  public bool IsSavingChanges() => this.m_isSavingNameChanges || this.m_isSavingContentChanges;

  public bool IsBeingEdited() => this == CollectionManager.Get().GetEditedDeck();

  public ShareableDeck CreatedFromShareableDeck => this.m_createdFromShareableDeck;

  public int GetMaxCardCount()
  {
    if (this.GetRuleset() != null)
      return this.GetRuleset().GetDeckSize(this);
    Debug.LogError((object) "GetMaxCardCount() - unable to get correct count, ruleset was unavailable");
    return 0;
  }

  public int GetTotalCardCount()
  {
    int totalCardCount = 0;
    foreach (CollectionDeckSlot slot in this.m_slots)
      totalCardCount += slot.Count;
    return totalCardCount;
  }

  public CollectionDeck.CardCountByStatus CountCardsByStatus(
    FormatType? formatTypeToValidateAgainst = null)
  {
    CollectionDeck.CardCountByStatus cardCountByStatus = new CollectionDeck.CardCountByStatus();
    foreach (CollectionDeckSlot slot in this.m_slots)
    {
      cardCountByStatus.Total += slot.Count;
      if (this.IsValidSlot(slot, enforceRemainingDeckRuleset: true, formatTypeToValidateAgainst: formatTypeToValidateAgainst))
        cardCountByStatus.Valid += slot.Count;
      else
        cardCountByStatus.Invalid += slot.Count;
    }
    cardCountByStatus.Max = this.GetMaxCardCount();
    cardCountByStatus.Missing = Mathf.Max(0, cardCountByStatus.Max - cardCountByStatus.Total);
    cardCountByStatus.Extra = Mathf.Max(0, cardCountByStatus.Total - cardCountByStatus.Max);
    cardCountByStatus.MissingPlusInvalid = cardCountByStatus.Missing + cardCountByStatus.Invalid;
    return cardCountByStatus;
  }

  public int GetTotalValidCardCount(FormatType? formatTypeToValidateAgainst = null)
  {
    int totalValidCardCount = 0;
    foreach (CollectionDeckSlot slot in this.m_slots)
    {
      if (this.IsValidSlot(slot, enforceRemainingDeckRuleset: true, formatTypeToValidateAgainst: formatTypeToValidateAgainst))
        totalValidCardCount += slot.Count;
    }
    return totalValidCardCount;
  }

  public int GetTotalInvalidCardCount(FormatType? formatTypeToValidateAgainst = null)
  {
    int invalidCardCount = 0;
    foreach (CollectionDeckSlot slot in this.m_slots)
    {
      if (!this.IsValidSlot(slot, this.IsLoanerDeck, enforceRemainingDeckRuleset: true, formatTypeToValidateAgainst: formatTypeToValidateAgainst))
        invalidCardCount += slot.Count;
    }
    return invalidCardCount;
  }

  public List<CollectionDeckSlot> GetSlots() => this.m_slots;

  public int GetSlotCount() => this.m_slots.Count;

  public bool IsValidSlot(
    CollectionDeckSlot slot,
    bool ignoreOwnership = false,
    bool ignoreGameplayEvent = false,
    bool enforceRemainingDeckRuleset = false,
    FormatType? formatTypeToValidateAgainst = null)
  {
    if (this.Locked)
      return true;
    FormatType? formatTypeToValidateAgainst1 = formatTypeToValidateAgainst;
    FormatType formatType = (FormatType) ((int) formatTypeToValidateAgainst1 ?? (int) this.FormatType);
    if (formatType != FormatType.FT_WILD && GameUtils.IsWildCard(slot.CardID))
      return false;
    formatTypeToValidateAgainst1 = new FormatType?();
    if (this.GetRuleset(formatTypeToValidateAgainst1) == null)
    {
      Debug.LogError((object) "IsValidSlot() - Unable to find ruleset");
      return false;
    }
    formatTypeToValidateAgainst1 = new FormatType?();
    if (this.GetRuleset(formatTypeToValidateAgainst1).HasIsPlayableRule() && !ignoreGameplayEvent && !GameUtils.IsCardGameplayEventActive(slot.CardID))
      return false;
    if (DuelsConfig.IsCardLoadoutTreasure(slot.CardID))
      return true;
    if (!ignoreOwnership && !slot.Owned)
      return false;
    EntityDef entityDef = slot.GetEntityDef();
    if (formatType != FormatType.FT_UNKNOWN && GameUtils.IsBanned(this, entityDef))
      return false;
    if (enforceRemainingDeckRuleset && entityDef != null)
    {
      List<DeckRule.RuleType> ruleTypeList = new List<DeckRule.RuleType>();
      if (ignoreOwnership)
        ruleTypeList.Add(DeckRule.RuleType.PLAYER_OWNS_EACH_COPY);
      if (ignoreGameplayEvent)
        ruleTypeList.Add(DeckRule.RuleType.IS_CARD_PLAYABLE);
      if (SceneMgr.Get().IsInDuelsMode() && !PvPDungeonRunScene.IsEditingDeck())
        ruleTypeList.Add(DeckRule.RuleType.DEATHKNIGHT_RUNE_LIMIT);
      if (!this.GetRuleset(formatTypeToValidateAgainst).Filter(entityDef, this, ruleTypeList.Count == 0 ? (DeckRule.RuleType[]) null : ruleTypeList.ToArray()))
        return false;
    }
    return true;
  }

  public CollectionDeck.SlotStatus GetSlotStatus(CollectionDeckSlot slot)
  {
    if (slot == null)
      return CollectionDeck.SlotStatus.UNKNOWN;
    if (this.ShouldSplitSlotsByOwnershipOrFormatValidity() && !DuelsConfig.IsCardLoadoutTreasure(slot.CardID))
    {
      if (!GameUtils.IsCardCollectible(slot.CardID))
        return CollectionDeck.SlotStatus.NOT_VALID;
      if (!slot.Owned)
        return CollectionDeck.SlotStatus.MISSING;
      if (!this.IsValidSlot(slot, true, enforceRemainingDeckRuleset: true))
        return CollectionDeck.SlotStatus.NOT_VALID;
    }
    return CollectionDeck.SlotStatus.VALID;
  }

  public bool HasReplaceableSlot()
  {
    for (int index = 0; index < this.m_slots.Count; ++index)
    {
      if (!this.IsValidSlot(this.m_slots[index]))
        return true;
    }
    return false;
  }

  public CollectionDeckSlot GetSlotByIndex(int slotIndex) => slotIndex < 0 || slotIndex >= this.GetSlotCount() ? (CollectionDeckSlot) null : this.m_slots[slotIndex];

  public CollectionDeckSlot GetExistingSlot(CollectionDeckSlot searchSlot)
  {
    if (this.ShouldSplitSlotsByOwnershipOrFormatValidity())
    {
      foreach (CollectionDeckSlot slot in this.m_slots)
      {
        if (slot.CardID == searchSlot.CardID && slot.Owned == searchSlot.Owned)
          return slot;
      }
    }
    else
    {
      foreach (CollectionDeckSlot slot in this.m_slots)
      {
        if (slot.CardID == searchSlot.CardID)
          return slot;
      }
    }
    return (CollectionDeckSlot) null;
  }

  public DeckRuleset GetRuleset(FormatType? formatTypeToValidateAgainst = null)
  {
    DeckRuleset ruleset = (DeckRuleset) null;
    switch (this.Type)
    {
      case DeckType.NORMAL_DECK:
      case DeckType.PRECON_DECK:
        ruleset = DeckRuleset.GetRuleset(formatTypeToValidateAgainst.HasValue ? formatTypeToValidateAgainst.Value : this.FormatType);
        break;
      case DeckType.TAVERN_BRAWL_DECK:
      case DeckType.FSG_BRAWL_DECK:
        ruleset = TavernBrawlManager.Get().GetCurrentDeckRuleset();
        break;
      case DeckType.PVPDR_DECK:
        ruleset = DeckRuleset.GetPVPDRRuleset();
        break;
      case DeckType.PVPDR_DISPLAY_DECK:
        ruleset = DeckRuleset.GetPVPDRDisplayRuleset();
        break;
    }
    if (ruleset == null)
      ruleset = DeckRuleset.GetRuleset(FormatType.FT_WILD);
    return ruleset;
  }

  public bool IsValidForFormat(FormatType formatType) => formatType == FormatType.FT_WILD && this.FormatType == FormatType.FT_STANDARD || this.FormatType == formatType;

  public static bool DoesModeRequireSpecificFormat(SceneMgr.Mode mode, bool isRanked)
  {
    if (mode == SceneMgr.Mode.TOURNAMENT & isRanked)
      return true;
    switch (mode)
    {
      case SceneMgr.Mode.FRIENDLY:
        return true;
      case SceneMgr.Mode.FIRESIDE_GATHERING:
        if (FiresideGatheringManager.Get().CurrentFiresideGatheringMode == FiresideGatheringManager.FiresideGatheringMode.FRIENDLY_CHALLENGE)
          return true;
        break;
    }
    return false;
  }

  public bool IsValidForModeAndFormat(SceneMgr.Mode mode, bool isRanked, FormatType formatType) => (mode != SceneMgr.Mode.TOURNAMENT || GameUtils.HasUnlockedClass(this.GetClass())) && (this.FormatType != FormatType.FT_CLASSIC || mode != SceneMgr.Mode.ADVENTURE && ((IEnumerable<TAG_CLASS>) GameUtils.CLASSIC_ORDERED_HERO_CLASSES).Contains<TAG_CLASS>(this.GetClass())) && (!CollectionDeck.DoesModeRequireSpecificFormat(mode, isRanked) || this.IsValidForFormat(formatType));

  public void CopyFrom(CollectionDeck otherDeck)
  {
    this.ID = otherDeck.ID;
    this.Type = otherDeck.Type;
    this.m_name = otherDeck.m_name;
    this.HeroCardID = otherDeck.HeroCardID;
    this.HeroOverridden = otherDeck.HeroOverridden;
    this.CardBackID = otherDeck.CardBackID;
    this.NeedsName = otherDeck.NeedsName;
    this.SeasonId = otherDeck.SeasonId;
    this.BrawlLibraryItemId = otherDeck.BrawlLibraryItemId;
    this.FormatType = otherDeck.FormatType;
    this.SortOrder = otherDeck.SortOrder;
    this.SourceType = otherDeck.SourceType;
    this.UIHeroOverrideCardID = otherDeck.UIHeroOverrideCardID;
    this.UIHeroOverridePremium = otherDeck.UIHeroOverridePremium;
    this.SetRuneOrder(otherDeck.GetRuneOrder());
    this.m_slots.Clear();
    for (int slotIndex = 0; slotIndex < otherDeck.GetSlotCount(); ++slotIndex)
    {
      CollectionDeckSlot slotByIndex = otherDeck.GetSlotByIndex(slotIndex);
      CollectionDeckSlot collectionDeckSlot = new CollectionDeckSlot();
      collectionDeckSlot.CopyFrom(slotByIndex);
      this.m_slots.Add(collectionDeckSlot);
    }
  }

  public void CopyContents(CollectionDeck otherDeck)
  {
    this.HeroCardID = otherDeck.HeroCardID;
    this.UIHeroOverrideCardID = otherDeck.UIHeroOverrideCardID;
    this.UIHeroOverridePremium = otherDeck.UIHeroOverridePremium;
    this.SetRuneOrder(otherDeck.GetRuneOrder());
    this.m_slots.Clear();
    for (int slotIndex = 0; slotIndex < otherDeck.GetSlotCount(); ++slotIndex)
    {
      CollectionDeckSlot slotByIndex = otherDeck.GetSlotByIndex(slotIndex);
      foreach (TAG_PREMIUM premium in Enum.GetValues(typeof (TAG_PREMIUM)))
      {
        for (int index = 0; index < slotByIndex.GetCount(premium); ++index)
          this.AddCard(slotByIndex.CardID, premium, false);
      }
    }
  }

  public bool FillFromShareableDeck(ShareableDeck shareableDeck)
  {
    this.HeroCardID = GameUtils.TranslateDbIdToCardId(shareableDeck.HeroCardDbId);
    this.FormatType = shareableDeck.FormatType;
    bool flag = true;
    this.m_slots.Clear();
    for (int index1 = 0; index1 < shareableDeck.DeckContents.Cards.Count; ++index1)
    {
      string cardId = GameUtils.TranslateDbIdToCardId(shareableDeck.DeckContents.Cards[index1].Def.Asset);
      TAG_PREMIUM premium = (TAG_PREMIUM) shareableDeck.DeckContents.Cards[index1].Def.Premium;
      int qty = shareableDeck.DeckContents.Cards[index1].Qty;
      for (int index2 = 0; index2 < qty; ++index2)
      {
        if (!this.AddCard(cardId, premium, false))
          flag = false;
      }
    }
    return flag;
  }

  public void FillFromTemplateDeck(CollectionManager.TemplateDeck tplDeck)
  {
    this.ClearSlotContents();
    this.Name = tplDeck.m_title;
    this.SetRuneOrder(tplDeck.m_rune1, tplDeck.m_rune2, tplDeck.m_rune3);
    foreach (KeyValuePair<string, int> cardId in tplDeck.m_cardIds)
    {
      int golden;
      int signature;
      int diamond;
      CollectionManager.Get().GetOwnedCardCount(cardId.Key, out int _, out golden, out signature, out diamond);
      int num;
      for (num = cardId.Value; num > 0 && diamond > 0; --num)
      {
        this.AddCard(cardId.Key, TAG_PREMIUM.DIAMOND, false);
        --diamond;
      }
      for (; num > 0 && signature > 0; --num)
      {
        this.AddCard(cardId.Key, TAG_PREMIUM.SIGNATURE, false);
        --diamond;
      }
      for (; num > 0 && golden > 0; --num)
      {
        this.AddCard(cardId.Key, TAG_PREMIUM.GOLDEN, false);
        --golden;
      }
      for (; num > 0; --num)
        this.AddCard(cardId.Key, TAG_PREMIUM.NORMAL, false);
    }
    this.SetRuneOrder(tplDeck.m_rune1, tplDeck.m_rune2, tplDeck.m_rune3);
  }

  public void FillFromCardList(
    IEnumerable<DeckMaker.DeckFill> fillCards,
    CollectionDeck.ChangeSource changeSource)
  {
    if (fillCards == null)
      return;
    foreach (DeckMaker.DeckFill fillCard in fillCards)
    {
      if (this.GetTotalCardCount() < this.GetMaxCardCount())
      {
        if (fillCard.m_addCard != null)
        {
          TAG_PREMIUM? premiumThatCanBeAdded = this.GetPreferredPremiumThatCanBeAdded(fillCard.m_addCard.GetCardId());
          if (premiumThatCanBeAdded.HasValue)
            this.AddCard(fillCard.m_addCard.GetCardId(), premiumThatCanBeAdded.Value, false);
        }
      }
      else
        break;
    }
    this.SendChanges(changeSource);
  }

  public void ReconcileOwnershipOnCollectionCardRemoved(string cardID, TAG_PREMIUM premium)
  {
    CollectionDeckSlot ownedSlotByCardId = this.FindFirstOwnedSlotByCardId(cardID, true);
    if (ownedSlotByCardId == null)
      return;
    int ownedCount = CollectionManager.Get().GetOwnedCount(cardID, premium);
    int count1 = ownedSlotByCardId.GetCount(premium);
    if (ownedCount >= count1)
      return;
    int count2 = count1 - ownedCount;
    ownedSlotByCardId.RemoveCard(count2, premium);
    for (; count2 > 0; --count2)
    {
      TAG_PREMIUM? nullable = new TAG_PREMIUM?((TAG_PREMIUM) ((int) this.GetPreferredPremiumThatCanBeAdded(cardID) ?? 0));
      this.AddCard(cardID, nullable.Value, false);
    }
    if (this.IsBeingEdited())
      return;
    this.SendChanges(CollectionDeck.ChangeSource.ReconcileCardOwnership);
  }

  public void ReconcileOwnershipOnCollectionCardAdded(string cardID)
  {
    CollectionDeckSlot ownedSlotByCardId = this.FindFirstOwnedSlotByCardId(cardID, false);
    if (ownedSlotByCardId == null)
      return;
    bool flag = false;
    for (int count = ownedSlotByCardId.Count; count > 0; --count)
    {
      TAG_PREMIUM? premiumThatCanBeAdded = this.GetPreferredPremiumThatCanBeAdded(cardID);
      if (premiumThatCanBeAdded.HasValue)
      {
        if (this.AddCard(cardID, premiumThatCanBeAdded.Value, false))
          flag = true;
      }
      else
        break;
    }
    if (!flag)
      return;
    this.SendChanges(CollectionDeck.ChangeSource.ReconcileCardOwnership);
  }

  public CollectionDeckSlot FindInvalidSlot() => this.GetSlots().Find((Predicate<CollectionDeckSlot>) (slot => !this.IsValidSlot(slot, enforceRemainingDeckRuleset: true)));

  public List<CollectionDeckSlot> FindInvalidSlots(
    FormatType? formatTypeToValidateAgainst = null)
  {
    return this.GetSlots().FindAll((Predicate<CollectionDeckSlot>) (slot => !this.IsValidSlot(slot, enforceRemainingDeckRuleset: true, formatTypeToValidateAgainst: formatTypeToValidateAgainst)));
  }

  public void RemoveInvalidCards(FormatType? formatTypeToValidateAgainst = null)
  {
    foreach (CollectionDeckSlot invalidSlot in this.FindInvalidSlots(formatTypeToValidateAgainst))
      this.RemoveSlot(invalidSlot);
  }

  public void RemoveExtraCards(FormatType? formatTypeToValidateAgainst = null)
  {
    CollectionDeck.CardCountByStatus cardCountByStatus1 = this.CountCardsByStatus(formatTypeToValidateAgainst);
    if (cardCountByStatus1.Extra <= 0)
      return;
    using (List<CollectionDeckSlot>.Enumerator enumerator = this.FindInvalidSlots(formatTypeToValidateAgainst).GetEnumerator())
    {
label_7:
      while (enumerator.MoveNext())
      {
        CollectionDeckSlot current = enumerator.Current;
        while (true)
        {
          if (cardCountByStatus1.Extra > 0 && current.Count > 0)
          {
            current.RemoveCard(1, current.UnPreferredPremium);
            --cardCountByStatus1.Extra;
          }
          else
            goto label_7;
        }
      }
    }
    CollectionDeck.CardCountByStatus cardCountByStatus2 = this.CountCardsByStatus(formatTypeToValidateAgainst);
    if (cardCountByStatus2.Extra <= 0)
      return;
    for (List<CollectionDeckSlot> list = this.m_slots.Where<CollectionDeckSlot>((Func<CollectionDeckSlot, bool>) (slot => !slot.GetEntityDef().HasTag(GAME_TAG.DECK_RULE_MOD_DECK_SIZE))).ToList<CollectionDeckSlot>(); cardCountByStatus2.Extra > 0 && list.Count > 0; --cardCountByStatus2.Extra)
    {
      int index = UnityEngine.Random.Range(0, list.Count);
      CollectionDeckSlot collectionDeckSlot = list[index];
      collectionDeckSlot.RemoveCard(1, collectionDeckSlot.UnPreferredPremium);
      if (collectionDeckSlot.Count == 0)
        list.RemoveAt(index);
    }
  }

  public int GetCardIdCount(string cardID, bool includeUnowned = true)
  {
    int cardIdCount = 0;
    foreach (CollectionDeckSlot slot in this.m_slots)
    {
      if (slot.CardID.Equals(cardID) && (includeUnowned || slot.Owned))
        cardIdCount += slot.Count;
    }
    return cardIdCount;
  }

  public int GetCardCountMatchingTag(GAME_TAG tagName, int tagValue)
  {
    int countMatchingTag = 0;
    foreach (CollectionDeckSlot slot in this.m_slots)
    {
      if (GameUtils.GetCardTagValue(slot.CardID, tagName) == tagValue)
        countMatchingTag += slot.Count;
    }
    return countMatchingTag;
  }

  public int GetCardCountAllMatchingSlots(string cardID)
  {
    int allMatchingSlots = 0;
    foreach (CollectionDeckSlot slot in this.m_slots)
    {
      if (slot.CardID.Equals(cardID))
        allMatchingSlots += slot.Count;
    }
    return allMatchingSlots;
  }

  public int GetCardCountAllMatchingSlots(string cardID, TAG_PREMIUM premium)
  {
    int allMatchingSlots = 0;
    foreach (CollectionDeckSlot slot in this.m_slots)
    {
      if (slot.CardID.Equals(cardID))
        allMatchingSlots += slot.GetCount(premium);
    }
    return allMatchingSlots;
  }

  public int GetOwnedCardCountInDeck(string cardID, TAG_PREMIUM premium, bool owned = true)
  {
    if (!this.ShouldSplitSlotsByOwnershipOrFormatValidity())
      return this.GetCardCountAllMatchingSlots(cardID);
    int ownedCardCountInDeck = 0;
    foreach (CollectionDeckSlot slot in this.m_slots)
    {
      if (slot.CardID.Equals(cardID) && slot.Owned == owned)
        ownedCardCountInDeck += slot.GetCount(premium);
    }
    return ownedCardCountInDeck;
  }

  public int GetCardCountInSet(HashSet<string> set, bool isNot)
  {
    int cardCountInSet = 0;
    for (int index = 0; index < this.m_slots.Count; ++index)
    {
      CollectionDeckSlot slot = this.m_slots[index];
      if (set.Contains(slot.CardID) == !isNot)
        cardCountInSet += slot.Count;
    }
    return cardCountInSet;
  }

  public void ClearSlotContents() => this.m_slots.Clear();

  public TAG_PREMIUM? GetPreferredPremiumThatCanBeAdded(string cardId)
  {
    if (this.CanCardBeAddedAsOwned(cardId, TAG_PREMIUM.DIAMOND))
      return new TAG_PREMIUM?(TAG_PREMIUM.DIAMOND);
    if (this.CanCardBeAddedAsOwned(cardId, TAG_PREMIUM.SIGNATURE))
      return new TAG_PREMIUM?(TAG_PREMIUM.SIGNATURE);
    if (this.CanCardBeAddedAsOwned(cardId, TAG_PREMIUM.GOLDEN))
      return new TAG_PREMIUM?(TAG_PREMIUM.GOLDEN);
    return this.CanCardBeAddedAsOwned(cardId, TAG_PREMIUM.NORMAL) ? new TAG_PREMIUM?(TAG_PREMIUM.NORMAL) : new TAG_PREMIUM?();
  }

  public bool CanCardBeAddedAsOwned(string cardID, TAG_PREMIUM premium)
  {
    int ownedCardCountInDeck = this.GetOwnedCardCountInDeck(cardID, premium);
    return CollectionManager.Get().GetOwnedCount(cardID, premium) > ownedCardCountInDeck;
  }

  public bool AddCard(
    string cardID,
    TAG_PREMIUM premium,
    bool allowInvalid = false,
    params DeckRule.RuleType[] ignoreRules)
  {
    bool owned = false;
    if (this.ShouldSplitSlotsByOwnershipOrFormatValidity())
    {
      owned = this.CanCardBeAddedAsOwned(cardID, premium);
      if (!owned)
        premium = TAG_PREMIUM.NORMAL;
    }
    CollectionDeckSlot ownedSlotByCardId = this.FindFirstOwnedSlotByCardId(cardID, false);
    CollectionDeckSlot collectionDeckSlot;
    if (owned)
    {
      collectionDeckSlot = this.FindFirstOwnedSlotByCardId(cardID, true);
      ownedSlotByCardId?.RemoveCard(1, ownedSlotByCardId.UnPreferredPremium);
    }
    else
      collectionDeckSlot = ownedSlotByCardId;
    if (!allowInvalid && !this.CanAddCard(DefLoader.Get().GetEntityDef(cardID), premium, ignoreRules))
      return false;
    bool flag;
    if (collectionDeckSlot == null)
    {
      collectionDeckSlot = this.InsertSlotWithCard(cardID, premium, owned, 1);
      flag = collectionDeckSlot != null;
    }
    else
    {
      collectionDeckSlot.AddCard(1, premium);
      flag = true;
    }
    if (flag)
      this.UpdateDeckRunes((EntityBase) collectionDeckSlot.GetEntityDef());
    return flag;
  }

  private void UpdateDeckRunes(EntityBase entity)
  {
    if (entity == null)
      return;
    RuneType[] array = this.Runes.CombineRunes(entity.GetRuneCost(), DeckRule_DeathKnightRuneLimit.MaxRuneSlots).ToArray();
    int index1 = 0;
    for (int index2 = 0; index2 < this.m_runeOrder.Length; ++index2)
    {
      if (this.m_runeOrder[index2] == RuneType.RT_NONE && index1 < array.Length)
      {
        this.SetRuneAtIndex(index2, array[index1]);
        ++index1;
      }
    }
  }

  public CollectionDeckSlot InsertSlotWithCard(
    string cardID,
    TAG_PREMIUM premium,
    bool owned,
    int count)
  {
    CollectionDeckSlot slot = new CollectionDeckSlot()
    {
      CardID = cardID,
      Owned = owned
    };
    slot.SetCount(count, premium);
    return this.InsertSlot(this.GetInsertionIdxByDefaultSort(slot), slot) ? slot : (CollectionDeckSlot) null;
  }

  public bool AddCard_DungeonCrawlBuff(string cardId, TAG_PREMIUM premium, List<int> enchantments)
  {
    CollectionDeckSlot collectionDeckSlot = this.InsertSlotWithCard(cardId, premium, true, 1);
    if (collectionDeckSlot == null)
      return false;
    collectionDeckSlot.CreateEntityDefOverride();
    foreach (int enchantment in enchantments)
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(enchantment);
      int tag1 = entityDef.GetTag(GAME_TAG.UI_BUFF_ATK_UP);
      if (tag1 != 0)
      {
        int tag2 = collectionDeckSlot.m_entityDefOverride.GetTag(GAME_TAG.ATK);
        collectionDeckSlot.m_entityDefOverride.SetTag(GAME_TAG.ATK, tag2 + tag1);
      }
      int tag3 = entityDef.GetTag(GAME_TAG.UI_BUFF_HEALTH_UP);
      if (tag3 != 0)
      {
        int tag4 = collectionDeckSlot.m_entityDefOverride.GetTag(GAME_TAG.HEALTH);
        collectionDeckSlot.m_entityDefOverride.SetTag(GAME_TAG.HEALTH, tag4 + tag3);
      }
      int tag5 = entityDef.GetTag(GAME_TAG.UI_BUFF_DURABILITY_UP);
      if (tag5 != 0)
      {
        int tag6 = collectionDeckSlot.m_entityDefOverride.GetTag(GAME_TAG.DURABILITY);
        collectionDeckSlot.m_entityDefOverride.SetTag(GAME_TAG.DURABILITY, tag6 + tag5);
      }
      int tag7 = entityDef.GetTag(GAME_TAG.UI_BUFF_COST_UP);
      if (tag7 != 0)
      {
        int tag8 = collectionDeckSlot.m_entityDefOverride.GetTag(GAME_TAG.COST);
        collectionDeckSlot.m_entityDefOverride.SetTag(GAME_TAG.COST, tag8 + tag7);
      }
      int tag9 = entityDef.GetTag(GAME_TAG.UI_BUFF_COST_DOWN);
      if (tag9 != 0)
      {
        int tag10 = collectionDeckSlot.m_entityDefOverride.GetTag(GAME_TAG.COST);
        collectionDeckSlot.m_entityDefOverride.SetTag(GAME_TAG.COST, Math.Max(tag10 - tag9, 0));
      }
      if (entityDef.GetTag(GAME_TAG.UI_BUFF_SET_COST_ZERO) != 0)
        collectionDeckSlot.m_entityDefOverride.SetTag(GAME_TAG.COST, 0);
    }
    return true;
  }

  public bool RemoveCard(
    string cardID,
    TAG_PREMIUM premium,
    bool valid,
    bool enforceRemainingDeckRuleset)
  {
    CollectionDeckSlot cardIdAndValidity = this.FindFirstSlotByCardIdAndValidity(cardID, valid, false, enforceRemainingDeckRuleset);
    if (cardIdAndValidity == null)
      return false;
    cardIdAndValidity.RemoveCard(1, premium);
    this.UpdateUIHeroOverrideCardRemoval(cardID);
    return true;
  }

  public void RemoveAllCards() => this.m_slots = new List<CollectionDeckSlot>();

  private void UpdateUIHeroOverrideCardRemoval(string cardID)
  {
    if (!GameDbf.GetIndex().HasCardPlayerDeckOverride(cardID) || !((UnityEngine.Object) CollectionDeckTray.Get() == (UnityEngine.Object) null) && CollectionDeckTray.Get().IsShowingDeckContents())
      return;
    this.UIHeroOverrideCardID = string.Empty;
    this.UIHeroOverridePremium = TAG_PREMIUM.NORMAL;
    this.Name = GameStrings.Format("GLOBAL_BASIC_DECK_NAME", (object) GameStrings.GetClassName(this.GetClass()));
    CollectionManager.Get().OnUIHeroOverrideCardRemoved();
  }

  public void OnContentChangesComplete() => this.m_isSavingContentChanges = false;

  public void OnNameChangeComplete() => this.m_isSavingNameChanges = false;

  public void SendChanges(CollectionDeck.ChangeSource changeSource)
  {
    CollectionDeck baseDeck = CollectionManager.Get().GetBaseDeck(this.ID);
    if (this == baseDeck)
      Debug.LogError((object) string.Format("CollectionDeck.Send() - {0} is a base deck. You cannot send a base deck to the network.", (object) baseDeck));
    else if (baseDeck == null)
    {
      Log.CollectionManager.PrintError("CollectionDeck.SendChanges() - No base deck with id=" + (object) this.ID);
    }
    else
    {
      string deckName;
      this.GenerateNameDiff(baseDeck, out deckName);
      List<Network.CardUserData> contentChanges = this.GenerateContentChanges(baseDeck);
      int heroAssetID;
      bool? heroOverrideStatus;
      int overrideHeroAssetID;
      TAG_PREMIUM overrideHeroPremium;
      bool heroDiff = this.GenerateHeroDiff(baseDeck, out heroAssetID, out heroOverrideStatus, out overrideHeroAssetID, out overrideHeroPremium);
      int? cardBackID;
      bool cardBackDiff = this.GenerateCardBackDiff(baseDeck, out cardBackID);
      bool flag1 = baseDeck.FormatType != this.FormatType;
      bool flag2 = baseDeck.SortOrder != this.SortOrder;
      bool flag3 = !this.IsRuneOrderEqual(baseDeck.GetRuneOrder());
      bool? randomHeroUseFavorite = new bool?();
      bool flag4 = baseDeck.RandomHeroUseFavorite != this.RandomHeroUseFavorite;
      if (flag4)
        randomHeroUseFavorite = new bool?(this.RandomHeroUseFavorite);
      Network network = Network.Get();
      if (deckName != null)
      {
        this.m_isSavingNameChanges = true;
        network.RenameDeck(this.ID, deckName);
      }
      string pastedDeckHash = (string) null;
      if (this.m_createdFromShareableDeck != null)
        pastedDeckHash = this.m_createdFromShareableDeck.Serialize(false);
      if (contentChanges.Count > 0 | heroDiff | flag4 | cardBackDiff | flag1 | flag2 | flag3)
      {
        this.m_isSavingContentChanges = true;
        ++this.m_changeNumber;
        Network.Get().SendDeckData(changeSource, this.m_changeNumber, this.ID, contentChanges, heroAssetID, heroOverrideStatus, overrideHeroAssetID, overrideHeroPremium, cardBackID, this.FormatType, this.SortOrder, randomHeroUseFavorite, this.m_runeOrder, pastedDeckHash);
      }
      if (Network.IsLoggedIn())
        return;
      this.OnContentChangesComplete();
      this.OnNameChangeComplete();
    }
  }

  public static string GetUserFriendlyCopyErrorMessageFromDeckRuleViolation(
    DeckRuleViolation violation)
  {
    if (violation == null || violation.Rule == null)
      return string.Empty;
    switch (violation.Rule.Type)
    {
      case DeckRule.RuleType.IS_NOT_ROTATED:
        return GameStrings.Get("GLUE_COLLECTION_DECK_COPY_TOOLTIP_FORMAT");
      case DeckRule.RuleType.PLAYER_OWNS_EACH_COPY:
      case DeckRule.RuleType.DECK_SIZE:
        return GameStrings.Get("GLUE_COLLECTION_DECK_COPY_TOOLTIP_INCOMPLETE");
      case DeckRule.RuleType.IS_CARD_PLAYABLE:
        return GameStrings.Get("GLUE_COLLECTION_DECK_COPY_TOOLTIP_UNPLAYABLE");
      default:
        return violation.DisplayError;
    }
  }

  public void SetShareableDeckCreatedFrom(ShareableDeck shareableDeck) => this.m_createdFromShareableDeck = shareableDeck;

  public bool CanAddCard(
    EntityDef entityDef,
    TAG_PREMIUM premium,
    params DeckRule.RuleType[] ignoreRules)
  {
    if (entityDef == null)
      return false;
    if (DeckType.DRAFT_DECK == this.Type || DeckType.CLIENT_ONLY_DECK == this.Type)
      return true;
    DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset();
    if (deckRuleset == null || this.Type == DeckType.PVPDR_DISPLAY_DECK && DuelsConfig.IsCardLoadoutTreasure(entityDef.GetCardId()))
      return true;
    List<DeckRule.RuleType> ruleTypeList = new List<DeckRule.RuleType>((IEnumerable<DeckRule.RuleType>) ignoreRules);
    ruleTypeList.AddRange((IEnumerable<DeckRule.RuleType>) CollectionDeck.DefaultIgnoreRules);
    return deckRuleset.CanAddToDeck(entityDef, premium, this, out RuleInvalidReason _, out DeckRule _, ruleTypeList.ToArray());
  }

  private bool InsertSlot(int slotIndex, CollectionDeckSlot slot)
  {
    if (slotIndex < 0 || slotIndex > this.GetSlotCount())
      return false;
    slot.OnSlotEmptied += new CollectionDeckSlot.DelOnSlotEmptied(this.OnSlotEmptied);
    slot.Index = slotIndex;
    this.m_slots.Insert(slotIndex, slot);
    this.UpdateSlotIndices(slotIndex, this.GetSlotCount() - 1);
    return true;
  }

  private void RemoveSlot(CollectionDeckSlot slot)
  {
    slot.OnSlotEmptied -= new CollectionDeckSlot.DelOnSlotEmptied(this.OnSlotEmptied);
    int index = slot.Index;
    this.m_slots.RemoveAt(index);
    slot.m_entityDefOverride = (EntityDef) null;
    this.UpdateSlotIndices(index, this.GetSlotCount() - 1);
    this.UpdateUIHeroOverrideCardRemoval(slot.CardID);
  }

  private void OnSlotEmptied(CollectionDeckSlot slot)
  {
    if (this.GetExistingSlot(slot) == null)
      Log.Decks.Print(string.Format("CollectionDeck.OnSlotCountUpdated(): Trying to remove slot {0}, but it does not exist in deck {1}", (object) slot, (object) this));
    else
      this.RemoveSlot(slot);
  }

  private void UpdateSlotIndices(int indexA, int indexB)
  {
    if (this.GetSlotCount() == 0)
      return;
    int val2;
    int val1;
    if (indexA < indexB)
    {
      val2 = indexA;
      val1 = indexB;
    }
    else
    {
      val2 = indexB;
      val1 = indexA;
    }
    int num1 = Math.Max(0, val2);
    int num2 = Math.Min(val1, this.GetSlotCount() - 1);
    for (int slotIndex = num1; slotIndex <= num2; ++slotIndex)
      this.GetSlotByIndex(slotIndex).Index = slotIndex;
  }

  public CollectionDeckSlot FindFirstSlotByCardId(string cardID) => this.m_slots.Find((Predicate<CollectionDeckSlot>) (slot => slot.CardID.Equals(cardID)));

  public CollectionDeckSlot FindFirstOwnedSlotByCardId(string cardID, bool owned) => !this.ShouldSplitSlotsByOwnershipOrFormatValidity() ? this.FindFirstSlotByCardId(cardID) : this.m_slots.Find((Predicate<CollectionDeckSlot>) (slot => slot.CardID.Equals(cardID) && slot.Owned == owned));

  public CollectionDeckSlot FindFirstSlotByCardIdAndValidity(
    string cardID,
    bool valid,
    bool ignoreGameplayEvent,
    bool enforceRemainingDeckRuleset)
  {
    if (this.ShouldSplitSlotsByOwnershipOrFormatValidity())
      return this.m_slots.Find((Predicate<CollectionDeckSlot>) (slot => slot.CardID == cardID && valid == this.IsValidSlot(slot, ignoreGameplayEvent: ignoreGameplayEvent, enforceRemainingDeckRuleset: enforceRemainingDeckRuleset)));
    Log.Decks.PrintWarning("Your deck doesn't care about Validity.  Why are you using 'FindFirstValidSlot' as opposed to 'FindFirstOwnedSlot'? This may be a bug!");
    return this.FindFirstSlotByCardId(cardID);
  }

  private void GenerateNameDiff(CollectionDeck baseDeck, out string deckName)
  {
    deckName = (string) null;
    if (this.Name.Equals(baseDeck.Name))
      return;
    deckName = this.Name;
  }

  private bool GenerateHeroDiff(
    CollectionDeck baseDeck,
    out int heroAssetID,
    out bool? heroOverrideStatus,
    out int overrideHeroAssetID,
    out TAG_PREMIUM overrideHeroPremium)
  {
    heroAssetID = -1;
    overrideHeroAssetID = -1;
    overrideHeroPremium = TAG_PREMIUM.NORMAL;
    heroOverrideStatus = new bool?(this.HeroOverridden);
    bool heroDiff = false;
    if (this.HeroOverridden != baseDeck.HeroOverridden)
      heroDiff = true;
    bool flag = this.HeroCardID == baseDeck.HeroCardID;
    if (this.HeroOverridden && !flag)
    {
      heroAssetID = GameUtils.TranslateCardIdToDbId(this.HeroCardID);
      heroDiff = true;
    }
    if ((!(this.UIHeroOverrideCardID == baseDeck.UIHeroOverrideCardID) ? 0 : (this.UIHeroOverridePremium == baseDeck.UIHeroOverridePremium ? 1 : 0)) == 0)
    {
      overrideHeroAssetID = string.IsNullOrEmpty(this.UIHeroOverrideCardID) ? 0 : GameUtils.TranslateCardIdToDbId(this.UIHeroOverrideCardID);
      overrideHeroPremium = this.UIHeroOverridePremium;
      heroDiff = true;
    }
    return heroDiff;
  }

  private bool GenerateCardBackDiff(CollectionDeck baseDeck, out int? cardBackID)
  {
    cardBackID = new int?(-1);
    int? cardBackId1 = this.CardBackID;
    int? cardBackId2 = baseDeck.CardBackID;
    if (cardBackId1.GetValueOrDefault() == cardBackId2.GetValueOrDefault() & cardBackId1.HasValue == cardBackId2.HasValue)
      return false;
    cardBackID = this.CardBackID;
    return true;
  }

  private List<Network.CardUserData> CardUserDataFromSlot(
    CollectionDeckSlot deckSlot,
    bool deleted)
  {
    List<Network.CardUserData> cardUserDataList = new List<Network.CardUserData>();
    Network.CardUserData cardUserData1 = new Network.CardUserData();
    cardUserData1.DbId = GameUtils.TranslateCardIdToDbId(deckSlot.CardID);
    cardUserData1.Count = deleted ? 0 : deckSlot.GetCount(TAG_PREMIUM.NORMAL);
    cardUserData1.Premium = TAG_PREMIUM.NORMAL;
    Network.CardUserData cardUserData2 = new Network.CardUserData();
    cardUserData2.DbId = cardUserData1.DbId;
    cardUserData2.Count = deleted ? 0 : deckSlot.GetCount(TAG_PREMIUM.GOLDEN);
    cardUserData2.Premium = TAG_PREMIUM.GOLDEN;
    Network.CardUserData cardUserData3 = new Network.CardUserData();
    cardUserData3.DbId = cardUserData1.DbId;
    cardUserData3.Count = deleted ? 0 : deckSlot.GetCount(TAG_PREMIUM.SIGNATURE);
    cardUserData3.Premium = TAG_PREMIUM.SIGNATURE;
    Network.CardUserData cardUserData4 = new Network.CardUserData();
    cardUserData4.DbId = cardUserData1.DbId;
    cardUserData4.Count = deleted ? 0 : deckSlot.GetCount(TAG_PREMIUM.DIAMOND);
    cardUserData4.Premium = TAG_PREMIUM.DIAMOND;
    cardUserDataList.Add(cardUserData1);
    cardUserDataList.Add(cardUserData2);
    cardUserDataList.Add(cardUserData3);
    cardUserDataList.Add(cardUserData4);
    return cardUserDataList;
  }

  private List<Network.CardUserData> GenerateContentChanges(CollectionDeck baseDeck)
  {
    SortedDictionary<string, CollectionDeckSlot> sortedDictionary1 = new SortedDictionary<string, CollectionDeckSlot>();
    foreach (CollectionDeckSlot slot in baseDeck.GetSlots())
    {
      CollectionDeckSlot collectionDeckSlot1 = (CollectionDeckSlot) null;
      if (sortedDictionary1.TryGetValue(slot.CardID, out collectionDeckSlot1))
      {
        foreach (TAG_PREMIUM premium in Enum.GetValues(typeof (TAG_PREMIUM)))
          collectionDeckSlot1.AddCard(slot.GetCount(premium), premium);
      }
      else
      {
        CollectionDeckSlot collectionDeckSlot2 = new CollectionDeckSlot();
        collectionDeckSlot2.CopyFrom(slot);
        sortedDictionary1.Add(collectionDeckSlot2.CardID, collectionDeckSlot2);
      }
    }
    SortedDictionary<string, CollectionDeckSlot> sortedDictionary2 = new SortedDictionary<string, CollectionDeckSlot>();
    foreach (CollectionDeckSlot slot in this.GetSlots())
    {
      CollectionDeckSlot collectionDeckSlot3 = (CollectionDeckSlot) null;
      if (sortedDictionary2.TryGetValue(slot.CardID, out collectionDeckSlot3))
      {
        foreach (TAG_PREMIUM premium in Enum.GetValues(typeof (TAG_PREMIUM)))
          collectionDeckSlot3.AddCard(slot.GetCount(premium), premium);
      }
      else
      {
        CollectionDeckSlot collectionDeckSlot4 = new CollectionDeckSlot();
        collectionDeckSlot4.CopyFrom(slot);
        sortedDictionary2.Add(collectionDeckSlot4.CardID, collectionDeckSlot4);
      }
    }
    SortedDictionary<string, CollectionDeckSlot>.Enumerator enumerator1 = sortedDictionary1.GetEnumerator();
    SortedDictionary<string, CollectionDeckSlot>.Enumerator enumerator2 = sortedDictionary2.GetEnumerator();
    List<Network.CardUserData> contentChanges = new List<Network.CardUserData>();
    bool flag1 = enumerator1.MoveNext();
    bool flag2 = enumerator2.MoveNext();
    KeyValuePair<string, CollectionDeckSlot> current;
    while (flag1 & flag2)
    {
      current = enumerator1.Current;
      CollectionDeckSlot deckSlot1 = current.Value;
      current = enumerator2.Current;
      CollectionDeckSlot deckSlot2 = current.Value;
      if (deckSlot1.CardID == deckSlot2.CardID)
      {
        if (deckSlot1.GetCount(TAG_PREMIUM.NORMAL) != deckSlot2.GetCount(TAG_PREMIUM.NORMAL) || deckSlot1.GetCount(TAG_PREMIUM.GOLDEN) != deckSlot2.GetCount(TAG_PREMIUM.GOLDEN) || deckSlot1.GetCount(TAG_PREMIUM.SIGNATURE) != deckSlot2.GetCount(TAG_PREMIUM.SIGNATURE) || deckSlot1.GetCount(TAG_PREMIUM.DIAMOND) != deckSlot2.GetCount(TAG_PREMIUM.DIAMOND))
          contentChanges.AddRange((IEnumerable<Network.CardUserData>) this.CardUserDataFromSlot(deckSlot2, deckSlot2.Count == 0));
        flag1 = enumerator1.MoveNext();
        flag2 = enumerator2.MoveNext();
      }
      else if (deckSlot1.CardID.CompareTo(deckSlot2.CardID) < 0)
      {
        contentChanges.AddRange((IEnumerable<Network.CardUserData>) this.CardUserDataFromSlot(deckSlot1, true));
        flag1 = enumerator1.MoveNext();
      }
      else
      {
        contentChanges.AddRange((IEnumerable<Network.CardUserData>) this.CardUserDataFromSlot(deckSlot2, false));
        flag2 = enumerator2.MoveNext();
      }
    }
    for (; flag1; flag1 = enumerator1.MoveNext())
    {
      current = enumerator1.Current;
      CollectionDeckSlot deckSlot = current.Value;
      contentChanges.AddRange((IEnumerable<Network.CardUserData>) this.CardUserDataFromSlot(deckSlot, true));
    }
    for (; flag2; flag2 = enumerator2.MoveNext())
    {
      current = enumerator2.Current;
      CollectionDeckSlot deckSlot = current.Value;
      contentChanges.AddRange((IEnumerable<Network.CardUserData>) this.CardUserDataFromSlot(deckSlot, false));
    }
    return contentChanges;
  }

  private int GetInsertionIdxByDefaultSort(CollectionDeckSlot slot)
  {
    EntityDef entityDef1 = slot.GetEntityDef();
    if (entityDef1 == null)
    {
      Log.Decks.Print(string.Format("CollectionDeck.GetInsertionIdxByDefaultSort(): could not get entity def for {0}", (object) slot.CardID));
      return -1;
    }
    int slotIndex;
    for (slotIndex = 0; slotIndex < this.GetSlotCount(); ++slotIndex)
    {
      CollectionDeckSlot slotByIndex = this.GetSlotByIndex(slotIndex);
      EntityDef entityDef2 = slotByIndex.GetEntityDef();
      if (entityDef2 == null)
      {
        Log.Decks.Print(string.Format("CollectionDeck.GetInsertionIdxByDefaultSort(): entityDef is null at slot index {0}", (object) slotIndex));
        break;
      }
      int num = CollectionManager.EntityDefSortComparison(entityDef1, entityDef2);
      if (num < 0 || num <= 0 && (!this.ShouldSplitSlotsByOwnershipOrFormatValidity() || slot.Owned == slotByIndex.Owned))
        break;
    }
    return slotIndex;
  }

  public TAG_CLASS GetClass()
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(this.HeroCardID);
    return entityDef != null ? entityDef.GetClass() : TAG_CLASS.INVALID;
  }

  public List<TAG_CLASS> GetClasses()
  {
    EntityDef entityDef = DefLoader.Get().GetEntityDef(this.HeroCardID);
    List<TAG_CLASS> classes = new List<TAG_CLASS>();
    if (entityDef == null)
    {
      classes.Clear();
      classes.Add(TAG_CLASS.INVALID);
      return classes;
    }
    entityDef.GetClasses((IList<TAG_CLASS>) classes);
    return classes;
  }

  public bool HasClass(TAG_CLASS tagClass)
  {
    foreach (TAG_CLASS tagClass1 in this.GetClasses())
    {
      if (tagClass1 == tagClass)
        return true;
    }
    return false;
  }

  public ShareableDeck GetShareableDeck()
  {
    PegasusUtil.DeckContents deckContents = this.GetDeckContents();
    return new ShareableDeck(this.Name, GameUtils.TranslateCardIdToDbId(this.HeroCardID), deckContents, this.FormatType, this.Type == DeckType.DRAFT_DECK);
  }

  public bool CanCopyAsShareableDeck(out DeckRuleViolation topViolation)
  {
    topViolation = (DeckRuleViolation) null;
    if (this.GetRuleset() == null)
      return false;
    IList<DeckRuleViolation> violations;
    if (this.GetRuleset().IsDeckValid(this, out violations) || violations == null || violations.Count <= 0)
      return true;
    topViolation = violations[0];
    return false;
  }

  public void LogDeckStringInformation()
  {
    Log.Decks.PrintInfo(string.Format("{0} {1}", (object) "###", (object) this.Name));
    Log.Decks.PrintInfo(string.Format("{0}Deck ID: {1}", (object) "# ", (object) this.ID));
    Log.Decks.PrintInfo(this.GetShareableDeck().Serialize(false));
  }

  public PegasusUtil.DeckContents GetDeckContents()
  {
    PegasusUtil.DeckContents deckContents = new PegasusUtil.DeckContents()
    {
      DeckId = this.ID
    };
    foreach (CollectionDeckSlot slot in this.m_slots)
    {
      DeckCardData deckCardData = new DeckCardData()
      {
        Def = new PegasusShared.CardDef()
        {
          Asset = GameUtils.TranslateCardIdToDbId(slot.CardID),
          Premium = (int) slot.PreferredPremium
        },
        Qty = slot.Count
      };
      deckContents.Cards.Add(deckCardData);
    }
    return deckContents;
  }

  public bool ShouldSplitSlotsByOwnershipOrFormatValidity()
  {
    if (this.Locked)
      return false;
    switch (this.Type)
    {
      case DeckType.CLIENT_ONLY_DECK:
      case DeckType.DRAFT_DECK:
        return false;
      case DeckType.TAVERN_BRAWL_DECK:
      case DeckType.FSG_BRAWL_DECK:
        return TavernBrawlManager.Get().IsCurrentBrawlTypeActive && TavernBrawlManager.Get().GetCurrentDeckRuleset() != null && TavernBrawlManager.Get().GetCurrentDeckRuleset().HasOwnershipOrRotatedRule();
      default:
        return true;
    }
  }

  public bool CanAddRunes(RunePattern runesToAdd, int maxRuneSlots) => this.Runes.CanAddRunes(runesToAdd, maxRuneSlots);

  public bool ContainsDeathKnightRuneCards()
  {
    RunePattern runePattern = new RunePattern();
    foreach (CollectionDeckSlot slot in this.m_slots)
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(slot.CardID);
      runePattern.SetCostsFromEntity((EntityBase) entityDef);
      if (runePattern.HasRunes)
        return true;
    }
    return false;
  }

  public enum SlotStatus
  {
    UNKNOWN,
    VALID,
    NOT_VALID,
    MISSING,
  }

  public enum ChangeSource
  {
    Unknown,
    ClickToFixMissingAndInvalidCards,
    MarkDeckAsSeen,
    PocoSetDeckName,
    OnScenePreUnload,
    SaveCurrentDeck,
    NavigateToSceneForPartyChallenge,
    StartChallengeProcess,
    StopDragToReorder,
    ReconcileCardOwnership,
    ClickToFixExtraCards,
    Cheat,
  }

  public class CardCountByStatus
  {
    public int Max;
    public int Total;
    public int Valid;
    public int Invalid;
    public int Missing;
    public int MissingPlusInvalid;
    public int Extra;
  }
}
