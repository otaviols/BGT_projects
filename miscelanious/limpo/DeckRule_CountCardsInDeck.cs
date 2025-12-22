using UnityEngine;

public class DeckRule_CountCardsInDeck : DeckRule
{
  public DeckRule_CountCardsInDeck(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.COUNT_CARDS_IN_DECK, record)
  {
    if (this.m_appliesToSubset != null)
      return;
    Debug.LogError((object) "COUNT_CARDS_IN_DECK only supports rules with a defined \"applies to\" subset");
  }

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    int cardCountInSet = deck.GetCardCountInSet(this.m_appliesToSubset, this.m_appliesToIsNot);
    int countParam = 0;
    bool isMinimum = false;
    bool val = true;
    if (cardCountInSet < this.m_minValue)
    {
      val = false;
      countParam = this.m_minValue - cardCountInSet;
      isMinimum = true;
    }
    else if (cardCountInSet > this.m_maxValue)
    {
      val = false;
      countParam = cardCountInSet - this.m_maxValue;
    }
    int num = this.GetResult(val) ? 1 : 0;
    if (num != 0)
      return num != 0;
    reason = new RuleInvalidReason(this.m_errorString, countParam, isMinimum);
    return num != 0;
  }
}
