using System;
using System.Linq;

public class DeckRule_IsNotRotated : DeckRule
{
  public DeckRule_IsNotRotated(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.IS_NOT_ROTATED, record)
  {
  }

  public override bool Filter(EntityDef def, CollectionDeck deck) => !GameUtils.IsCardRotated(def);

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    int countParam = deck.GetSlots().Sum<CollectionDeckSlot>((Func<CollectionDeckSlot, int>) (s => !this.AppliesTo(s.CardID) || !GameUtils.IsCardRotated(s.CardID) ? 0 : s.Count));
    bool result = this.GetResult(countParam <= 0);
    if (!result)
    {
      if (RankMgr.Get().IsNewPlayer())
        reason = new RuleInvalidReason(GameStrings.Format("GLUE_COLLECTION_DECK_RULE_INVALID_CARDS_NPR", (object) countParam), countParam);
      else
        reason = new RuleInvalidReason(GameStrings.Format("GLUE_COLLECTION_DECK_RULE_INVALID_CARDS", (object) countParam), countParam);
    }
    return result;
  }
}
