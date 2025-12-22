using System.Collections.Generic;

public class DeckRule_IsNotBannedInLeague : DeckRule
{
  public DeckRule_IsNotBannedInLeague(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.IS_NOT_BANNED_IN_LEAGUE, record)
  {
  }

  public override bool Filter(EntityDef def, CollectionDeck deck) => !RankMgr.Get().IsCardBannedInCurrentLeague(def);

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    List<CollectionDeckSlot> slots = deck.GetSlots();
    int countParam = 0;
    foreach (CollectionDeckSlot collectionDeckSlot in slots)
    {
      EntityDef entityDef = DefLoader.Get().GetEntityDef(collectionDeckSlot.CardID);
      if (this.AppliesTo(collectionDeckSlot.CardID) && GameUtils.IsBanned(deck, entityDef))
        ++countParam;
    }
    bool result = this.GetResult(countParam == 0);
    if (!result)
    {
      if (RankMgr.Get().IsNewPlayer())
        reason = new RuleInvalidReason(GameStrings.Format("GLUE_COLLECTION_DECK_RULE_INVALID_CARDS_NPR", (object) countParam), countParam);
      else
        reason = new RuleInvalidReason(GameStrings.Format("GLUE_COLLECTION_DECK_RULE_INVALID_CARDS", (object) countParam), countParam);
    }
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
    bool result = this.GetResult(!GameUtils.IsBannedByDuelsDenylist(deck, cardId) && !GameUtils.IsBannedByConstructedDenylist(deck, cardId));
    if (!result)
      reason = new RuleInvalidReason(GameStrings.Format("GLUE_COLLECTION_DECK_RULE_INVALID_CARDS", (object) 1), 1);
    return result;
  }
}
