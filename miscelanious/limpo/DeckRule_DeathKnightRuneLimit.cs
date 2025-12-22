using System.Collections.Generic;

public class DeckRule_DeathKnightRuneLimit : DeckRule
{
  public static int MaxRuneSlots = 3;

  public DeckRule_DeathKnightRuneLimit(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.DEATHKNIGHT_RUNE_LIMIT, record)
  {
    DeckRule_DeathKnightRuneLimit.MaxRuneSlots = record.MaxValue;
  }

  public override bool CanAddToDeck(
    EntityDef def,
    TAG_PREMIUM premium,
    CollectionDeck deck,
    out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    List<TAG_CLASS> classes = deck.GetClasses();
    bool flag = false;
    foreach (TAG_CLASS tagClass in classes)
    {
      if (tagClass == TAG_CLASS.DEATHKNIGHT)
      {
        flag = true;
        break;
      }
    }
    return !flag || this.ValidateRunes(new RunePattern((EntityBase) def), deck.Runes, out reason);
  }

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    if (!deck.HasClass(TAG_CLASS.DEATHKNIGHT))
      return true;
    int countParam = 0;
    foreach (CollectionDeckSlot slot in deck.GetSlots())
    {
      RunePattern runeCost = slot.GetEntityDef().GetRuneCost();
      if (!deck.Runes.CanAddRunes(runeCost, deck.Runes.CombinedValue))
        ++countParam;
    }
    if (countParam <= 0)
      return true;
    reason = new RuleInvalidReason("Invalid rune cards.", countParam);
    return false;
  }

  public override bool Filter(EntityDef def, CollectionDeck deck)
  {
    CollectionManager collectionManager = CollectionManager.Get();
    return collectionManager != null && collectionManager.IsEditingDeathKnightDeck() || !deck.HasClass(TAG_CLASS.DEATHKNIGHT) || deck.CanAddRunes(def.GetRuneCost(), deck.Runes.CombinedValue);
  }

  private bool ValidateRunes(
    RunePattern runesToAdd,
    RunePattern validRunes,
    out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    if (!runesToAdd.HasRunes || validRunes.CanAddRunes(runesToAdd, this.m_maxValue))
      return true;
    reason = new RuleInvalidReason("GLUE_COLLECTION_INCOMPATIBLE_RUNES");
    return false;
  }
}
