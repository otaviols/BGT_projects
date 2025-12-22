using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckMaker
{
  private static readonly DeckMaker.CardRequirements[] s_OrderedCardRequirements = new DeckMaker.CardRequirements[6]
  {
    new DeckMaker.CardRequirements(8, (DeckMaker.CardRequirementsCondition) (e => DeckMaker.IsMinion(e) && DeckMaker.HasMinCost(e, 1) && DeckMaker.HasMaxCost(e, 2)), "GLUE_RDM_LOW_COST"),
    new DeckMaker.CardRequirements(5, (DeckMaker.CardRequirementsCondition) (e => DeckMaker.IsMinion(e) && DeckMaker.HasMinCost(e, 3) && DeckMaker.HasMaxCost(e, 4)), "GLUE_RDM_MEDIUM_COST"),
    new DeckMaker.CardRequirements(4, (DeckMaker.CardRequirementsCondition) (e => DeckMaker.IsMinion(e) && DeckMaker.HasMinCost(e, 5)), "GLUE_RDM_HIGH_COST"),
    new DeckMaker.CardRequirements(7, (DeckMaker.CardRequirementsCondition) (e => DeckMaker.IsSpell(e)), "GLUE_RDM_MORE_SPELLS"),
    new DeckMaker.CardRequirements(2, (DeckMaker.CardRequirementsCondition) (e => DeckMaker.IsWeapon(e)), "GLUE_RDM_MORE_WEAPONS"),
    new DeckMaker.CardRequirements(int.MaxValue, (DeckMaker.CardRequirementsCondition) (e => DeckMaker.IsMinion(e)), "GLUE_RDM_NO_SPECIFICS")
  };

  private static bool IsMinion(EntityDef e) => e.GetCardType() == TAG_CARDTYPE.MINION;

  private static bool IsSpell(EntityDef e) => e.GetCardType() == TAG_CARDTYPE.SPELL;

  private static bool IsWeapon(EntityDef e) => e.GetCardType() == TAG_CARDTYPE.WEAPON;

  private static bool HasMinCost(EntityDef e, int minCost) => e.GetCost() >= minCost;

  private static bool HasMaxCost(EntityDef e, int maxCost) => e.GetCost() <= maxCost;

  public static IEnumerable<DeckMaker.DeckFill> GetFillCards(
    CollectionDeck deck,
    DeckRuleset deckRuleset)
  {
    bool flag = true;
    List<EntityDef> cardsICanAddToDeck;
    List<EntityDef> currentDeckCards;
    List<EntityDef> currentInvalidCards;
    DeckMaker.InitFromDeck(deck, deckRuleset, out currentDeckCards, out currentInvalidCards, out cardsICanAddToDeck);
    int remainingCardsToFill = (deckRuleset != null ? deckRuleset.GetDeckSize(deck) : CollectionManager.Get().GetDeckSize()) - currentDeckCards.Count;
    if (remainingCardsToFill > 0)
    {
      if (flag)
      {
        foreach (DeckMaker.DeckFill invalidFillCard in DeckMaker.GetInvalidFillCards(cardsICanAddToDeck, currentDeckCards, currentInvalidCards))
        {
          --remainingCardsToFill;
          yield return invalidFillCard;
        }
      }
      int i;
      for (i = 0; i < DeckMaker.s_OrderedCardRequirements.Length; ++i)
      {
        if (remainingCardsToFill > 0)
        {
          DeckMaker.CardRequirements cardReq = DeckMaker.s_OrderedCardRequirements[i];
          DeckMaker.CardRequirementsCondition condition = cardReq.m_condition;
          int cardsToAddFromSet = Mathf.Min(cardReq.m_requiredCount - currentDeckCards.FindAll((Predicate<EntityDef>) (e => condition(e))).Count, remainingCardsToFill);
          if (cardsToAddFromSet > 0)
          {
            foreach (EntityDef entityDef in cardsICanAddToDeck.FindAll((Predicate<EntityDef>) (e => condition(e))))
            {
              if (cardsToAddFromSet > 0)
              {
                cardsICanAddToDeck.Remove(entityDef);
                currentDeckCards.Add(entityDef);
                --cardsToAddFromSet;
                --remainingCardsToFill;
                yield return new DeckMaker.DeckFill()
                {
                  m_removeTemplate = (EntityDef) null,
                  m_addCard = entityDef,
                  m_reason = cardReq.GetRequirementReason()
                };
              }
              else
                break;
            }
          }
          cardReq = (DeckMaker.CardRequirements) null;
        }
        else
          break;
      }
      for (i = 0; i < cardsICanAddToDeck.Count; ++i)
      {
        EntityDef entityDef = cardsICanAddToDeck[i];
        if (entityDef != null)
        {
          currentDeckCards.Add(entityDef);
          cardsICanAddToDeck[i] = (EntityDef) null;
          yield return new DeckMaker.DeckFill()
          {
            m_removeTemplate = (EntityDef) null,
            m_addCard = entityDef,
            m_reason = (string) null
          };
        }
      }
    }
  }

  public static DeckMaker.DeckChoiceFill GetFillCardChoices(
    CollectionDeck deck,
    EntityDef referenceCard,
    int choices,
    DeckRuleset deckRuleset = null)
  {
    if (deckRuleset == null)
      deckRuleset = deck.GetRuleset();
    List<EntityDef> currentDeckCards;
    List<EntityDef> currentInvalidCards;
    List<EntityDef> distinctCardsICanAddToDeck;
    DeckMaker.InitFromDeck(deck, deckRuleset, out currentDeckCards, out currentInvalidCards, out distinctCardsICanAddToDeck);
    return DeckMaker.GetFillCard(referenceCard, distinctCardsICanAddToDeck, currentDeckCards, currentInvalidCards, choices);
  }

  private static void InitFromDeck(
    CollectionDeck deck,
    DeckRuleset deckRuleset,
    out List<EntityDef> currentDeckCards,
    out List<EntityDef> currentInvalidCards,
    out List<EntityDef> distinctCardsICanAddToDeck)
  {
    CollectionManager collectionManager = CollectionManager.Get();
    List<DeckMaker.SortableEntityDef> sortableEntityDefList = new List<DeckMaker.SortableEntityDef>();
    currentDeckCards = new List<EntityDef>();
    currentInvalidCards = new List<EntityDef>();
    bool flag1 = false;
    bool flag2 = false;
    foreach (CollectionDeckSlot slot in deck.GetSlots())
    {
      foreach (TAG_PREMIUM premium in Enum.GetValues(typeof (TAG_PREMIUM)))
      {
        int count = slot.GetCount(premium);
        if (count > 0)
        {
          CollectibleCard card = CollectionManager.Get().GetCard(slot.CardID, premium);
          if (card != null)
          {
            EntityDef entityDef = card.GetEntityDef();
            for (int index = 0; index < count; ++index)
            {
              if (deck.IsValidSlot(slot))
                currentDeckCards.Add(entityDef);
              else
                currentInvalidCards.Add(entityDef);
            }
            if (entityDef.IsCollectionManagerFilterManaCostByEven)
              flag1 = true;
            if (entityDef.IsCollectionManagerFilterManaCostByOdd)
              flag2 = true;
          }
        }
      }
    }
    if (flag1 & flag2)
    {
      flag1 = false;
      flag2 = false;
    }
    foreach (KeyValuePair<string, EntityDef> allEntityDef in DefLoader.Get().GetAllEntityDefs())
    {
      KeyValuePair<string, EntityDef> kvpair = allEntityDef;
      CollectibleCard card1 = collectionManager.GetCard(kvpair.Key, TAG_PREMIUM.NORMAL);
      if (card1 != null && !card1.IsHeroSkin && (card1.GetEntityDef().HasClass(deck.GetClass()) || card1.Class == TAG_CLASS.NEUTRAL) && (deckRuleset == null || deckRuleset.Filter(kvpair.Value, deck)) && (!kvpair.Value.HasRuneCost || deckRuleset == null || deckRuleset.CanAddToDeck(kvpair.Value, TAG_PREMIUM.NORMAL, deck)) && !CollectionManager.Get().HasCoreCounterpart(GameUtils.TranslateCardIdToDbId(card1.GetEntityDef().GetCardId())) && !RankMgr.Get().IsCardLockedInCurrentLeague(card1.GetEntityDef()) && (!flag1 || card1.ManaCost % 2 == 0) && (!flag2 || card1.ManaCost % 2 != 0) && GameUtils.IsCardSetFilterEventActive(card1.CardId))
      {
        int a = 2;
        if (deckRuleset != null)
          a = Mathf.Min(2, deckRuleset.GetMaxCopiesOfCardAllowed(kvpair.Value));
        int ownedCount = card1.OwnedCount;
        CollectibleCard card2 = collectionManager.GetCard(kvpair.Key, TAG_PREMIUM.GOLDEN);
        if (card2 != null)
          ownedCount += card2.OwnedCount;
        int count = currentDeckCards.FindAll((Predicate<EntityDef>) (e => e == kvpair.Value)).Count;
        int num = Mathf.Min(a, ownedCount) - count;
        for (int index = 0; index < num; ++index)
          sortableEntityDefList.Add(new DeckMaker.SortableEntityDef()
          {
            m_entityDef = kvpair.Value,
            m_suggestWeight = card1.SuggestWeight
          });
      }
    }
    int randomizer = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    sortableEntityDefList.Sort((Comparison<DeckMaker.SortableEntityDef>) ((lhs, rhs) =>
    {
      int num = rhs.m_suggestWeight - lhs.m_suggestWeight;
      return num != 0 ? num : (lhs.GetHashCode() ^ randomizer) - (rhs.GetHashCode() ^ randomizer);
    }));
    distinctCardsICanAddToDeck = new List<EntityDef>();
    foreach (DeckMaker.SortableEntityDef sortableEntityDef in sortableEntityDefList)
      distinctCardsICanAddToDeck.Add(sortableEntityDef.m_entityDef);
  }

  private static IEnumerable<DeckMaker.DeckFill> GetInvalidFillCards(
    List<EntityDef> cardsICanAddToDeck,
    List<EntityDef> currentDeckCards,
    List<EntityDef> currentInvalidCards)
  {
    foreach (EntityDef referenceCard in currentInvalidCards.ToArray())
    {
      DeckMaker.DeckFill deckFillChoice = DeckMaker.GetFillCard(referenceCard, cardsICanAddToDeck, (List<EntityDef>) null, currentInvalidCards, 1).GetDeckFillChoice(0);
      if (DeckMaker.ReplaceInvalidCard(deckFillChoice, cardsICanAddToDeck, currentDeckCards, currentInvalidCards))
        yield return deckFillChoice;
    }
  }

  private static bool ReplaceInvalidCard(
    DeckMaker.DeckFill choice,
    List<EntityDef> cardsICanAddToDeck,
    List<EntityDef> currentDeckCards,
    List<EntityDef> currentInvalidCards)
  {
    if (choice == null || !currentInvalidCards.Remove(choice.m_removeTemplate))
      return false;
    cardsICanAddToDeck.Remove(choice.m_addCard);
    currentDeckCards.Add(choice.m_addCard);
    return true;
  }

  private static DeckMaker.DeckChoiceFill GetFillCard(
    EntityDef referenceCard,
    List<EntityDef> cardsICanAddToDeck,
    List<EntityDef> currentDeckCards,
    List<EntityDef> currentInvalidCards,
    int totalNumChoices = 3)
  {
    if (referenceCard == null && currentInvalidCards != null && currentInvalidCards.Count > 0)
      referenceCard = currentInvalidCards.First<EntityDef>();
    int requirementsStartIndex = DeckMaker.GetCardRequirementsStartIndex(referenceCard, currentDeckCards);
    DeckMaker.DeckChoiceFill fillCard = new DeckMaker.DeckChoiceFill(referenceCard, Array.Empty<EntityDef>());
    for (int index = requirementsStartIndex; index < DeckMaker.s_OrderedCardRequirements.Length; ++index)
    {
      if (totalNumChoices > 0)
      {
        DeckMaker.CardRequirements orderedCardRequirement = DeckMaker.s_OrderedCardRequirements[index];
        DeckMaker.CardRequirementsCondition condition = orderedCardRequirement.m_condition;
        List<EntityDef> all = cardsICanAddToDeck.FindAll((Predicate<EntityDef>) (e => condition(e)));
        if (all.Count > 0)
        {
          int num = 8;
          List<EntityDef> arr1 = new List<EntityDef>();
          List<EntityDef> arr2 = new List<EntityDef>();
          int a = int.MinValue;
          foreach (EntityDef entityDef in all.Distinct<EntityDef>())
          {
            CollectibleCard card = CollectionManager.Get().GetCard(entityDef.GetCardId(), TAG_PREMIUM.NORMAL);
            a = Mathf.Max(a, card.SuggestWeight);
          }
          foreach (EntityDef entityDef in all.Distinct<EntityDef>())
          {
            if (num > 0)
            {
              CollectibleCard card = CollectionManager.Get().GetCard(entityDef.GetCardId(), TAG_PREMIUM.NORMAL);
              if (a - card.SuggestWeight > 100)
                arr2.Add(entityDef);
              else
                arr1.Add(entityDef);
              --num;
            }
            else
              break;
          }
          GeneralUtils.Shuffle<EntityDef>((IList<EntityDef>) arr1);
          GeneralUtils.Shuffle<EntityDef>((IList<EntityDef>) arr2);
          int count1 = Mathf.Min(arr1.Count, totalNumChoices);
          int count2 = Mathf.Min(arr2.Count, totalNumChoices - count1);
          if (count1 > 0)
            fillCard.m_addChoices.AddRange((IEnumerable<EntityDef>) arr1.GetRange(0, count1));
          if (count2 > 0)
            fillCard.m_addChoices.AddRange((IEnumerable<EntityDef>) arr2.GetRange(0, count2));
          totalNumChoices -= count1 + count2;
          DeckMaker.DeckChoiceFill deckChoiceFill = fillCard;
          string str;
          if (referenceCard != null)
            str = GameStrings.Format("GLUE_RDM_TEMPLATE_REPLACE", (object) referenceCard.GetName());
          else
            str = orderedCardRequirement.GetRequirementReason();
          deckChoiceFill.m_reason = str;
        }
      }
      else
        break;
    }
    return fillCard;
  }

  private static int GetCardRequirementsStartIndex(
    EntityDef referenceCard,
    List<EntityDef> currentDeckCards)
  {
    if (referenceCard != null)
    {
      for (int requirementsStartIndex = 0; requirementsStartIndex < DeckMaker.s_OrderedCardRequirements.Length; ++requirementsStartIndex)
      {
        if (DeckMaker.s_OrderedCardRequirements[requirementsStartIndex].m_condition(referenceCard))
          return requirementsStartIndex;
      }
    }
    else if (currentDeckCards != null)
    {
      for (int requirementsStartIndex = 0; requirementsStartIndex < DeckMaker.s_OrderedCardRequirements.Length; ++requirementsStartIndex)
      {
        DeckMaker.CardRequirements orderedCardRequirement = DeckMaker.s_OrderedCardRequirements[requirementsStartIndex];
        DeckMaker.CardRequirementsCondition condition = orderedCardRequirement.m_condition;
        if (currentDeckCards.FindAll((Predicate<EntityDef>) (e => condition(e))).Count < orderedCardRequirement.m_requiredCount)
          return requirementsStartIndex;
      }
    }
    return 0;
  }

  public class DeckChoiceFill
  {
    public EntityDef m_removeTemplate;
    public List<EntityDef> m_addChoices = new List<EntityDef>();
    public string m_reason;

    public DeckChoiceFill(EntityDef remove, params EntityDef[] addChoices)
    {
      this.m_removeTemplate = remove;
      if (addChoices == null || addChoices.Length == 0)
        return;
      this.m_addChoices = new List<EntityDef>((IEnumerable<EntityDef>) addChoices);
    }

    public DeckMaker.DeckFill GetDeckFillChoice(int idx)
    {
      if (idx >= this.m_addChoices.Count)
        return (DeckMaker.DeckFill) null;
      return new DeckMaker.DeckFill()
      {
        m_removeTemplate = this.m_removeTemplate,
        m_addCard = this.m_addChoices[idx],
        m_reason = this.m_reason
      };
    }
  }

  public class DeckFill
  {
    public EntityDef m_removeTemplate;
    public EntityDef m_addCard;
    public string m_reason;
  }

  public delegate bool CardRequirementsCondition(EntityDef entityDef);

  private class CardRequirements
  {
    public int m_requiredCount;
    public DeckMaker.CardRequirementsCondition m_condition;
    private string m_reason;

    public CardRequirements(
      int requiredCount,
      DeckMaker.CardRequirementsCondition condition,
      string reason = "")
    {
      this.m_requiredCount = requiredCount;
      this.m_condition = condition;
      this.m_reason = reason;
    }

    public string GetRequirementReason() => string.IsNullOrEmpty(this.m_reason) ? "No reason!" : GameStrings.Get(this.m_reason);
  }

  private class SortableEntityDef
  {
    public EntityDef m_entityDef;
    public int m_suggestWeight;
  }
}
