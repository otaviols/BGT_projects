using UnityEngine;

public class DeckRule_DeckSize : DeckRule
{
  public DeckRule_DeckSize(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.DECK_SIZE, record)
  {
    if (this.m_ruleIsNot)
      Debug.LogError((object) "DECK_SIZE rules do not support \"is not\".");
    if (this.m_appliesToSubset == null)
      return;
    Debug.LogError((object) "DECK_SIZE rules do not support \"applies to subset\".");
  }

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    int totalCardCount = deck.GetTotalCardCount();
    int countParam = 0;
    bool isMinimum = false;
    int num1 = this.m_minValue;
    int num2 = this.m_maxValue;
    foreach (CollectionDeckSlot slot in deck.GetSlots())
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(slot.CardID);
      if (entityDef.HasTag(GAME_TAG.DECK_RULE_MOD_DECK_SIZE))
      {
        num2 = entityDef.GetTag(GAME_TAG.DECK_RULE_MOD_DECK_SIZE);
        num1 = num2;
        break;
      }
    }
    bool val = true;
    if (totalCardCount < num1)
    {
      val = false;
      countParam = num1 - totalCardCount;
      isMinimum = true;
    }
    else if (totalCardCount > num2)
    {
      val = false;
      countParam = totalCardCount - num2;
    }
    bool result = this.GetResult(val);
    if (!result)
    {
      string error;
      if (totalCardCount < num1)
        error = GameStrings.Format("GLUE_COLLECTION_DECK_RULE_MISSING_CARDS", (object) countParam);
      else
        error = GameStrings.Format("GLUE_COLLECTION_DECK_RULE_TOO_MANY_CARDS", (object) countParam);
      reason = new RuleInvalidReason(error, countParam, isMinimum);
    }
    return result;
  }

  public int GetMaximumDeckSize(CollectionDeck deck = null)
  {
    if (deck == null)
      return this.GetDefaultDeckSize();
    int modifiedDeckSize;
    return this.CardInDeckModifiesDeckSize(deck, out modifiedDeckSize) ? modifiedDeckSize : this.m_maxValue;
  }

  public int GetMinimumDeckSize(CollectionDeck deck = null)
  {
    if (deck == null)
      return this.GetDefaultDeckSize();
    int modifiedDeckSize;
    return this.CardInDeckModifiesDeckSize(deck, out modifiedDeckSize) ? modifiedDeckSize : this.m_minValue;
  }

  private bool CardInDeckModifiesDeckSize(CollectionDeck deck, out int modifiedDeckSize)
  {
    foreach (CollectionDeckSlot slot in deck.GetSlots())
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(slot.CardID);
      if (entityDef.HasTag(GAME_TAG.DECK_RULE_MOD_DECK_SIZE))
      {
        modifiedDeckSize = entityDef.GetTag(GAME_TAG.DECK_RULE_MOD_DECK_SIZE);
        return true;
      }
      if (entityDef.HasTag(GAME_TAG.IGNORE_DECK_RULESET))
      {
        modifiedDeckSize = int.MaxValue;
        return true;
      }
    }
    modifiedDeckSize = 0;
    return false;
  }

  private int GetDefaultDeckSize() => this.m_maxValue;
}
