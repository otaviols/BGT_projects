using System.Collections.Generic;

public class DeckRule_CountCopiesOfEachCard : DeckRule
{
  public DeckRule_CountCopiesOfEachCard(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.COUNT_COPIES_OF_EACH_CARD, record)
  {
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
    int cardIdCount = deck.GetCardIdCount(def.GetCardId());
    int dbId = GameUtils.TranslateCardIdToDbId(def.GetCardId());
    int countMatchingTag = deck.GetCardCountMatchingTag(GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID, dbId);
    bool flag = cardIdCount + countMatchingTag + deck.GetCardIdCount(GameUtils.TranslateDbIdToCardId(GameUtils.GetCardTagValue(dbId, GAME_TAG.DECK_RULE_COUNT_AS_COPY_OF_CARD_ID))) >= this.m_maxValue;
    if (flag)
      reason = new RuleInvalidReason(GameStrings.Format("GLUE_COLLECTION_LOCK_MAX_DECK_COPIES", (object) this.m_maxValue), this.m_maxValue);
    return this.GetResult(!flag);
  }

  public bool GetMaxCopies(EntityDef def, out int maxCopies)
  {
    maxCopies = int.MaxValue;
    if (!this.AppliesTo(def.GetCardId()))
      return false;
    maxCopies = this.m_maxValue;
    return true;
  }

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    bool val = true;
    List<CollectionDeckSlot> slots = deck.GetSlots();
    int countParam = 0;
    bool isMinimum = false;
    foreach (CollectionDeckSlot collectionDeckSlot in slots)
    {
      string cardId = collectionDeckSlot.CardID;
      if (this.AppliesTo(cardId))
      {
        int cardIdCount = deck.GetCardIdCount(cardId);
        if (cardIdCount < this.m_minValue)
        {
          val = false;
          countParam = this.m_minValue - cardIdCount;
          isMinimum = true;
          break;
        }
        if (cardIdCount > this.m_maxValue)
        {
          val = false;
          int maxValue;
          countParam = maxValue = this.m_maxValue;
          break;
        }
      }
    }
    bool result = this.GetResult(val);
    if (!result)
      reason = new RuleInvalidReason(this.m_errorString, countParam, isMinimum);
    return result;
  }
}
