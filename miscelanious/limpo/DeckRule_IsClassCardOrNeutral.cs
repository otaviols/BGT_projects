using System.Collections.Generic;
using UnityEngine;

public class DeckRule_IsClassCardOrNeutral : DeckRule
{
  public DeckRule_IsClassCardOrNeutral(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.IS_CLASS_CARD_OR_NEUTRAL, record)
  {
    if (!this.m_ruleIsNot)
      return;
    Debug.LogError((object) "IS_CLASS_CARD_OR_NEUTRAL rules do not support \"is not\".");
  }

  public override bool Filter(EntityDef def, CollectionDeck deck)
  {
    if (!this.AppliesTo(def.GetCardId()) || deck == null)
      return true;
    List<TAG_CLASS> classes = deck.GetClasses();
    return this.GetResult(DeckRule_IsClassCardOrNeutral.CardIsClassCardOrNeutral((EntityBase) def, classes));
  }

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    bool flag = true;
    List<CollectionDeckSlot> slots = deck.GetSlots();
    int countParam = 0;
    List<TAG_CLASS> classes = deck.GetClasses();
    foreach (CollectionDeckSlot collectionDeckSlot in slots)
    {
      string cardId = collectionDeckSlot.CardID;
      if (this.AppliesTo(cardId) && !this.GetResult(DeckRule_IsClassCardOrNeutral.CardIsClassCardOrNeutral((EntityBase) DefLoader.Get().GetEntityDef(cardId), classes)))
      {
        countParam += collectionDeckSlot.Count;
        flag = false;
      }
    }
    if (!flag)
      reason = new RuleInvalidReason(GameStrings.Format("GLUE_COLLECTION_DECK_RULE_INVALID_CLASS_CARD", (object) countParam), countParam);
    return flag;
  }

  public override bool CanAddToDeck(
    EntityDef def,
    TAG_PREMIUM premium,
    CollectionDeck deck,
    out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    if (!this.AppliesTo(def.GetCardId()))
      return true;
    List<TAG_CLASS> classes = deck.GetClasses();
    bool result = this.GetResult(DeckRule_IsClassCardOrNeutral.CardIsClassCardOrNeutral((EntityBase) def, classes));
    if (!result)
      reason = new RuleInvalidReason(GameStrings.Get("GLUE_COLLECTION_LOCK_CARD_INVALID_CLASS"));
    return result;
  }

  private static bool CardIsClassCardOrNeutral(EntityBase def, List<TAG_CLASS> deckClasses)
  {
    List<TAG_CLASS> classes = new List<TAG_CLASS>();
    def.GetClasses((IList<TAG_CLASS>) classes);
    foreach (TAG_CLASS tagClass in classes)
    {
      if (tagClass == TAG_CLASS.NEUTRAL)
        return true;
      foreach (TAG_CLASS deckClass in deckClasses)
      {
        if (deckClass == tagClass)
          return true;
      }
    }
    return false;
  }
}
