using UnityEngine;

public class DeckRule_EditingDeckExtraCardCount : DeckRule
{
  public DeckRule_EditingDeckExtraCardCount(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.EDITING_DECK_EXTRA_CARD_COUNT, record)
  {
    if (this.m_ruleIsNot)
      Debug.LogError((object) "EDITING_DECK_EXTRA_CARD_COUNT rules do not support \"is not\".");
    if (this.m_appliesToSubset == null)
      return;
    Debug.LogError((object) "EDITING_DECK_EXTRA_CARD_COUNT rules do not support \"applies to subset\".");
  }

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    return true;
  }

  public int GetEditingDeckExtraCardCount() => this.m_maxValue;
}
