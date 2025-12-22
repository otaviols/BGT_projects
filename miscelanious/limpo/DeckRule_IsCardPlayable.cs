using System;
using System.Linq;

public class DeckRule_IsCardPlayable : DeckRule
{
  public DeckRule_IsCardPlayable(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.IS_CARD_PLAYABLE, record)
  {
  }

  public override bool Filter(EntityDef def, CollectionDeck deck) => GameUtils.IsCardGameplayEventActive(def);

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    int countParam = deck.GetSlots().Sum<CollectionDeckSlot>((Func<CollectionDeckSlot, int>) (s => !this.AppliesTo(s.CardID) || GameUtils.IsCardGameplayEventActive(s.CardID) ? 0 : s.Count));
    bool result = this.GetResult(countParam <= 0);
    if (!result)
      reason = new RuleInvalidReason(GameStrings.Format("GLUE_COLLECTION_DECK_RULE_UNPLAYABLE_CARDS", (object) countParam), countParam);
    return result;
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
    if (GameUtils.IsCardGameplayEventActive(cardId))
      return this.GetResult(true);
    reason = new RuleInvalidReason(GameStrings.Get("GLUE_COLLECTION_LOCK_CARD_NOT_PLAYABLE"));
    return this.GetResult(false);
  }
}
