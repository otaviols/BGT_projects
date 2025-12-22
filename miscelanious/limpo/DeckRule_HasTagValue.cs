using System.Collections.Generic;

public class DeckRule_HasTagValue : DeckRule
{
  public DeckRule_HasTagValue(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.HAS_TAG_VALUE, record)
  {
  }

  public override bool Filter(EntityDef def, CollectionDeck deck) => !this.AppliesTo(def.GetCardId()) || this.GetResult(DeckRule_HasTagValue.CardHasTagValue(def.GetTag(this.m_tag), this.m_tagMaxValue, this.m_tagMinValue));

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    bool flag = true;
    List<CollectionDeckSlot> slots = deck.GetSlots();
    int countParam = 0;
    foreach (CollectionDeckSlot collectionDeckSlot in slots)
    {
      string cardId = collectionDeckSlot.CardID;
      if (this.AppliesTo(cardId) && !this.GetResult(DeckRule_HasTagValue.CardHasTagValue(DefLoader.Get().GetEntityDef(cardId).GetTag(this.m_tag), this.m_tagMaxValue, this.m_tagMinValue)))
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
    if (!this.AppliesTo(def.GetCardId()))
      return true;
    reason = new RuleInvalidReason(GameStrings.Get("GLUE_COLLECTION_LOCK_CARD_BANNED"));
    return this.GetResult(DeckRule_HasTagValue.CardHasTagValue(def.GetTag(this.m_tag), this.m_tagMaxValue, this.m_tagMinValue));
  }

  private static bool CardHasTagValue(int tagValue, int max, int min) => tagValue >= max && tagValue <= min;
}
