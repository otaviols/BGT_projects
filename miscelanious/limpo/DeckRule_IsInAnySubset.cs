using System.Collections.Generic;

public class DeckRule_IsInAnySubset : DeckRule
{
  public DeckRule_IsInAnySubset(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.IS_IN_ANY_SUBSET, record)
  {
  }

  public override bool Filter(EntityDef def, CollectionDeck deck)
  {
    string cardId = def.GetCardId();
    return !this.AppliesTo(cardId) || this.GetResult(DeckRule_IsInAnySubset.CardBelongsInAnySubset(cardId, (IList<HashSet<string>>) this.m_subsets));
  }

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    bool flag = true;
    List<CollectionDeckSlot> slots = deck.GetSlots();
    int countParam = 0;
    foreach (CollectionDeckSlot collectionDeckSlot in slots)
    {
      string cardId = collectionDeckSlot.CardID;
      if (this.AppliesTo(cardId) && !this.GetResult(DeckRule_IsInAnySubset.CardBelongsInAnySubset(cardId, (IList<HashSet<string>>) this.m_subsets)))
      {
        countParam += collectionDeckSlot.Count;
        flag = false;
      }
    }
    if (!flag)
      reason = new RuleInvalidReason(this.m_errorString, countParam);
    return flag;
  }

  public override bool CanAddToDeck(
    EntityDef def,
    TAG_PREMIUM premium,
    CollectionDeck deck,
    out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    string cardId = def.GetCardId();
    if (!this.AppliesTo(cardId))
      return true;
    reason = new RuleInvalidReason(GameStrings.Get("GLUE_COLLECTION_LOCK_CARD_BANNED"));
    return !DeckRule_IsInAnySubset.CardBelongsInAnySubset(cardId, (IList<HashSet<string>>) this.m_subsets) ? this.GetResult(false) : this.GetResult(true);
  }

  private static bool CardBelongsInAnySubset(string cardId, IList<HashSet<string>> subsets)
  {
    for (int index = 0; index < subsets.Count; ++index)
    {
      if (subsets[index].Contains(cardId))
        return true;
    }
    return false;
  }
}
