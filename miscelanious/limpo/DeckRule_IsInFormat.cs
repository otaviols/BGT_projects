using PegasusShared;
using System.Collections.Generic;

public class DeckRule_IsInFormat : DeckRule
{
  public DeckRule_IsInFormat(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.IS_IN_FORMAT, record)
  {
  }

  public override bool Filter(EntityDef def, CollectionDeck deck)
  {
    string cardId = def.GetCardId();
    return !this.AppliesTo(cardId) || deck == null || this.GetResult(this.CardBelongsInFormat(cardId));
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
      if (this.AppliesTo(cardId) && !this.GetResult(this.CardBelongsInFormat(cardId)))
      {
        countParam += collectionDeckSlot.Count;
        flag = false;
      }
    }
    if (!flag)
      reason = new RuleInvalidReason(GameStrings.Format("GLUE_COLLECTION_DECK_RULE_NOT_IN_FORMAT", (object) countParam), countParam);
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
    bool result = this.GetResult(this.CardBelongsInFormat(cardId));
    if (!result)
      reason = new RuleInvalidReason(GameStrings.Get("GLUE_COLLECTION_LOCK_CARD_NOT_IN_FORMAT"));
    return result;
  }

  private bool CardBelongsInFormat(string cardId) => GameUtils.IsCardValidForFormat((FormatType) this.m_minValue, cardId);
}
