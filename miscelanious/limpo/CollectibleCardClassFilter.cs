using Blizzard.T5.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleCardClassFilter : CollectibleCardFilter
{
  private int m_cardsPerPage = 8;
  private Map<TAG_CLASS, List<CollectibleCard>> m_currentResultsByClass = new Map<TAG_CLASS, List<CollectibleCard>>();
  private List<CollectibleCard> m_unfilteredDeathKnightCards = new List<CollectibleCard>();
  private List<CollectibleCard> m_hiddenDeathKnightCards = new List<CollectibleCard>();
  private static CollectionTabInfo[] m_orderedCollectionTabInfos = new CollectionTabInfo[12]
  {
    new CollectionTabInfo() { tagClass = TAG_CLASS.DEATHKNIGHT },
    new CollectionTabInfo() { tagClass = TAG_CLASS.DEMONHUNTER },
    new CollectionTabInfo() { tagClass = TAG_CLASS.DRUID },
    new CollectionTabInfo() { tagClass = TAG_CLASS.HUNTER },
    new CollectionTabInfo() { tagClass = TAG_CLASS.MAGE },
    new CollectionTabInfo() { tagClass = TAG_CLASS.PALADIN },
    new CollectionTabInfo() { tagClass = TAG_CLASS.PRIEST },
    new CollectionTabInfo() { tagClass = TAG_CLASS.ROGUE },
    new CollectionTabInfo() { tagClass = TAG_CLASS.SHAMAN },
    new CollectionTabInfo() { tagClass = TAG_CLASS.WARLOCK },
    new CollectionTabInfo() { tagClass = TAG_CLASS.WARRIOR },
    new CollectionTabInfo() { tagClass = TAG_CLASS.NEUTRAL }
  };

  public void Init(int cardsPerPage)
  {
    this.m_cardsPerPage = cardsPerPage;
    foreach (CollectionTabInfo collectionTabInfo in CollectibleCardClassFilter.m_orderedCollectionTabInfos)
    {
      if (!this.m_currentResultsByClass.ContainsKey(collectionTabInfo.tagClass))
        this.m_currentResultsByClass[collectionTabInfo.tagClass] = new List<CollectibleCard>();
    }
  }

  public override void UpdateResults()
  {
    this.FindCardsResult = this.GenerateResults();
    List<CollectibleCard> cards = this.FindCardsResult.m_cards;
    foreach (KeyValuePair<TAG_CLASS, List<CollectibleCard>> keyValuePair in this.m_currentResultsByClass)
      keyValuePair.Value.Clear();
    this.m_unfilteredDeathKnightCards.Clear();
    this.m_hiddenDeathKnightCards.Clear();
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    List<TAG_CLASS> classes = new List<TAG_CLASS>();
    foreach (CollectibleCard collectibleCard in cards)
    {
      collectibleCard.GetEntityDef().GetClasses((IList<TAG_CLASS>) classes);
      foreach (TAG_CLASS key in classes)
      {
        if (this.m_filterClasses == null || ((IEnumerable<TAG_CLASS>) this.m_filterClasses).Contains<TAG_CLASS>(key))
        {
          if (!this.m_currentResultsByClass.ContainsKey(key))
          {
            Error.AddDevFatal("Card: {0} ({1}) has an invalid class: {2}. Cannot render page.", (object) collectibleCard.Name, (object) collectibleCard.CardId, (object) collectibleCard.Class);
            return;
          }
          if (key != TAG_CLASS.DEATHKNIGHT)
          {
            this.m_currentResultsByClass[key].Add(collectibleCard);
          }
          else
          {
            this.m_unfilteredDeathKnightCards.Add(collectibleCard);
            EntityDef entityDef = collectibleCard.GetEntityDef();
            if (entityDef.HasRuneCost)
            {
              RunePattern runesToAdd = new RunePattern();
              runesToAdd.SetCostsFromEntity((EntityBase) entityDef);
              if (!CollectionPageManager.IsShowingLockedRuneCards && editedDeck != null && !editedDeck.CanAddRunes(runesToAdd, DeckRule_DeathKnightRuneLimit.MaxRuneSlots))
              {
                this.m_hiddenDeathKnightCards.Add(collectibleCard);
                continue;
              }
            }
            this.m_currentResultsByClass[key].Add(collectibleCard);
          }
        }
      }
    }
  }

  public bool HasHiddenDeathKnightCards => this.m_hiddenDeathKnightCards.Count > 0;

  public CollectibleCard GetFirstRuneCard()
  {
    if (this.m_unfilteredDeathKnightCards.Count <= 0)
      return (CollectibleCard) null;
    CollectibleCard unfilteredDeathKnightCard = this.m_unfilteredDeathKnightCards[0];
    return unfilteredDeathKnightCard.Runes.HasRunes ? unfilteredDeathKnightCard : this.GetNextValidDeathKnightCardRight(unfilteredDeathKnightCard, true);
  }

  public List<CollectibleCard> GetCardsForTab(CollectionTabInfo tabInfo)
  {
    if (tabInfo.tagClass == TAG_CLASS.INVALID)
      return (List<CollectibleCard>) null;
    List<CollectibleCard> collectibleCardList;
    return !this.m_currentResultsByClass.TryGetValue(tabInfo.tagClass, out collectibleCardList) ? (List<CollectibleCard>) null : collectibleCardList;
  }

  public int GetNumPagesForTab(CollectionTabInfo tabInfo)
  {
    List<CollectibleCard> cardsForTab = this.GetCardsForTab(tabInfo);
    return cardsForTab == null ? 0 : cardsForTab.Count / this.m_cardsPerPage + (cardsForTab.Count % this.m_cardsPerPage > 0 ? 1 : 0);
  }

  public int GetNumNewCardsForTab(CollectionTabInfo tabInfo)
  {
    List<CollectibleCard> cardsForTab = this.GetCardsForTab(tabInfo);
    return cardsForTab == null ? 0 : cardsForTab.Where<CollectibleCard>((Func<CollectibleCard, bool>) (c => c.IsNewCard)).Count<CollectibleCard>();
  }

  public override int GetTotalNumPages()
  {
    int totalNumPages = 0;
    foreach (CollectionTabInfo collectionTabInfo in CollectibleCardClassFilter.m_orderedCollectionTabInfos)
      totalNumPages += this.GetNumPagesForTab(collectionTabInfo);
    return totalNumPages;
  }

  public override List<CollectibleCard> GetPageContents(int page)
  {
    if (page < 0 || page > this.GetTotalNumPages())
      return new List<CollectibleCard>();
    int num1 = 0;
    foreach (CollectionTabInfo collectionTabInfo in CollectibleCardClassFilter.m_orderedCollectionTabInfos)
    {
      int num2 = num1;
      num1 += this.GetNumPagesForTab(collectionTabInfo);
      if (page <= num1)
      {
        int pageWithinClass = page - num2;
        return this.GetPageContentsForTab(collectionTabInfo, pageWithinClass, false, out int _);
      }
    }
    return new List<CollectibleCard>();
  }

  public CollectionTabInfo GetCurrentTabInfoFromPage(int page)
  {
    if (page < 0 || page > this.GetTotalNumPages())
      return new CollectionTabInfo();
    int num = 0;
    foreach (CollectionTabInfo collectionTabInfo in CollectibleCardClassFilter.m_orderedCollectionTabInfos)
    {
      num += this.GetNumPagesForTab(collectionTabInfo);
      if (page <= num)
        return collectionTabInfo;
    }
    return new CollectionTabInfo();
  }

  public override List<CollectibleCard> GetFirstNonEmptyPage(
    out int collectionPage)
  {
    collectionPage = 0;
    CollectionTabInfo pageTabInfo = new CollectionTabInfo()
    {
      tagClass = TAG_CLASS.NEUTRAL
    };
    for (int index = 0; index < CollectibleCardClassFilter.m_orderedCollectionTabInfos.Length; ++index)
    {
      if (this.m_currentResultsByClass[CollectibleCardClassFilter.m_orderedCollectionTabInfos[index].tagClass].Count > 0)
      {
        pageTabInfo = CollectibleCardClassFilter.m_orderedCollectionTabInfos[index];
        break;
      }
    }
    return this.GetPageContentsForTab(pageTabInfo, 1, true, out collectionPage);
  }

  public List<CollectibleCard> GetPageContentsForTab(
    CollectionTabInfo pageTabInfo,
    int pageWithinClass,
    bool calculateCollectionPage,
    out int collectionPage)
  {
    collectionPage = 0;
    if (pageWithinClass <= 0 || pageWithinClass > this.GetNumPagesForTab(pageTabInfo))
      return new List<CollectibleCard>();
    if (calculateCollectionPage)
    {
      for (int index = 0; index < CollectibleCardClassFilter.m_orderedCollectionTabInfos.Length; ++index)
      {
        CollectionTabInfo collectionTabInfo = CollectibleCardClassFilter.m_orderedCollectionTabInfos[index];
        if (collectionTabInfo.tagClass != pageTabInfo.tagClass)
          collectionPage += this.GetNumPagesForTab(collectionTabInfo);
        else
          break;
      }
      collectionPage += pageWithinClass;
    }
    List<CollectibleCard> cardsForTab = this.GetCardsForTab(pageTabInfo);
    return cardsForTab == null ? new List<CollectibleCard>() : cardsForTab.Skip<CollectibleCard>(this.m_cardsPerPage * (pageWithinClass - 1)).Take<CollectibleCard>(this.m_cardsPerPage).ToList<CollectibleCard>();
  }

  public List<CollectibleCard> GetPageContentsForCard(
    string cardID,
    TAG_PREMIUM premiumType,
    out int collectionPage,
    CollectionTabInfo tabInfoContext)
  {
    collectionPage = 0;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardID);
    List<TAG_CLASS> source = new List<TAG_CLASS>();
    List<TAG_CLASS> classes = source;
    entityDef.GetClasses((IList<TAG_CLASS>) classes);
    CollectionTabInfo collectionTabInfo = new CollectionTabInfo()
    {
      tagClass = TAG_CLASS.NEUTRAL
    };
    if (source.Count<TAG_CLASS>() == 1)
      collectionTabInfo.tagClass = source.ElementAt<TAG_CLASS>(0);
    else if (tabInfoContext.tagClass != TAG_CLASS.INVALID && source.Contains(tabInfoContext.tagClass))
      collectionTabInfo = tabInfoContext;
    else
      Debug.LogWarning((object) "CollectibleCardClassFilter.GetPageContentsForCard() - The specified card class mismatches its class context.");
    int index = this.GetCardsForTab(collectionTabInfo).FindIndex((Predicate<CollectibleCard>) (obj => obj.CardId == cardID && obj.PremiumType == premiumType));
    if (index < 0)
      return new List<CollectibleCard>();
    int num = index + 1;
    int pageWithinClass = num / this.m_cardsPerPage + (num % this.m_cardsPerPage > 0 ? 1 : 0);
    return this.GetPageContentsForTab(collectionTabInfo, pageWithinClass, true, out collectionPage);
  }

  public CollectibleCard GetNextValidDeathKnightCardLeft(
    CollectibleCard startingCard,
    bool mustHaveRunes = false)
  {
    if (startingCard == null)
      return (CollectibleCard) null;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null)
      return (CollectibleCard) null;
    int index1 = this.m_unfilteredDeathKnightCards.FindIndex((Predicate<CollectibleCard>) (card => card.CardId == startingCard.CardId));
    if (index1 < 0)
      return (CollectibleCard) null;
    RunePattern runes = editedDeck.Runes;
    for (int index2 = index1 - 1; index2 >= 0; --index2)
    {
      CollectibleCard unfilteredDeathKnightCard = this.m_unfilteredDeathKnightCards[index2];
      RunePattern runeCost = unfilteredDeathKnightCard.GetEntityDef().GetRuneCost();
      if ((!mustHaveRunes || runeCost.HasRunes) && runes.CanAddRunes(unfilteredDeathKnightCard.GetEntityDef().GetRuneCost(), DeckRule_DeathKnightRuneLimit.MaxRuneSlots))
        return unfilteredDeathKnightCard;
    }
    return (CollectibleCard) null;
  }

  public CollectibleCard GetNextValidDeathKnightCardRight(
    CollectibleCard startingCard,
    bool mustHaveRunes = false)
  {
    if (startingCard == null)
      return (CollectibleCard) null;
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (editedDeck == null)
      return (CollectibleCard) null;
    int index1 = this.m_unfilteredDeathKnightCards.FindIndex((Predicate<CollectibleCard>) (card => card.CardId == startingCard.CardId));
    if (index1 < 0)
      return (CollectibleCard) null;
    RunePattern runes = editedDeck.Runes;
    for (int index2 = index1 + 1; index2 < this.m_unfilteredDeathKnightCards.Count; ++index2)
    {
      CollectibleCard unfilteredDeathKnightCard = this.m_unfilteredDeathKnightCards[index2];
      RunePattern runeCost = unfilteredDeathKnightCard.GetEntityDef().GetRuneCost();
      if ((!mustHaveRunes || runeCost.HasRunes) && runes.CanAddRunes(runeCost, DeckRule_DeathKnightRuneLimit.MaxRuneSlots))
        return unfilteredDeathKnightCard;
    }
    return (CollectibleCard) null;
  }

  public int GetPageNumberForCard(CollectibleCard card, CollectionTabInfo classContext)
  {
    int collectionPage;
    this.GetPageContentsForCard(card.CardId, card.PremiumType, out collectionPage, classContext);
    return collectionPage;
  }

  public int GetFirstPageForTab(CollectionTabInfo tabInfo)
  {
    List<CollectibleCard> cardsForTab = this.GetCardsForTab(tabInfo);
    if (cardsForTab == null)
      return 0;
    CollectibleCard collectibleCard = cardsForTab[0];
    int collectionPage;
    this.GetPageContentsForCard(collectibleCard.CardId, collectibleCard.PremiumType, out collectionPage, tabInfo);
    return collectionPage;
  }

  public int GetLastPageForTab(CollectionTabInfo tabInfo)
  {
    List<CollectibleCard> cardsForTab = this.GetCardsForTab(tabInfo);
    if (cardsForTab == null)
      return 0;
    CollectibleCard collectibleCard = cardsForTab[cardsForTab.Count - 1];
    int collectionPage;
    this.GetPageContentsForCard(collectibleCard.CardId, collectibleCard.PremiumType, out collectionPage, tabInfo);
    return collectionPage;
  }
}
