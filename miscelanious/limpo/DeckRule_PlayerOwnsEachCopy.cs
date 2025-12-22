using Blizzard.T5.Core;
using System;
using System.Collections.Generic;

public class DeckRule_PlayerOwnsEachCopy : DeckRule
{
  public DeckRule_PlayerOwnsEachCopy(DeckRulesetRuleDbfRecord record)
    : base(DeckRule.RuleType.PLAYER_OWNS_EACH_COPY, record)
  {
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
    CollectibleCard card = CollectionManager.Get().GetCard(cardId, premium);
    if (card != null && deck.GetOwnedCardCountInDeck(cardId, premium) < card.OwnedCount)
      return this.GetResult(true);
    reason = new RuleInvalidReason(GameStrings.Get("GLUE_COLLECTION_LOCK_NO_MORE_INSTANCES"));
    return this.GetResult(false);
  }

  public override bool IsDeckValid(CollectionDeck deck, out RuleInvalidReason reason)
  {
    reason = (RuleInvalidReason) null;
    if (deck.Locked)
      return true;
    CollectionManager collectionManager = CollectionManager.Get();
    List<CollectionDeckSlot> slots = deck.GetSlots();
    Map<KeyValuePair<string, TAG_PREMIUM>, int> map = new Map<KeyValuePair<string, TAG_PREMIUM>, int>();
    for (int index = 0; index < slots.Count; ++index)
    {
      CollectionDeckSlot collectionDeckSlot = slots[index];
      if (collectionDeckSlot.Count > 0 && this.AppliesTo(collectionDeckSlot.CardID))
      {
        foreach (TAG_PREMIUM premium in Enum.GetValues(typeof (TAG_PREMIUM)))
        {
          KeyValuePair<string, TAG_PREMIUM> key = new KeyValuePair<string, TAG_PREMIUM>(collectionDeckSlot.CardID, premium);
          int num = 0;
          map.TryGetValue(key, out num);
          map[key] = num + collectionDeckSlot.GetCount(premium);
        }
      }
    }
    int countParam = 0;
    foreach (KeyValuePair<KeyValuePair<string, TAG_PREMIUM>, int> keyValuePair in map)
    {
      KeyValuePair<string, TAG_PREMIUM> key1 = keyValuePair.Key;
      string key2 = key1.Key;
      key1 = keyValuePair.Key;
      TAG_PREMIUM premium = key1.Value;
      int num1 = keyValuePair.Value;
      CollectibleCard card = collectionManager.GetCard(key2, premium);
      int num2 = card == null ? 0 : card.OwnedCount;
      if (num2 < num1)
        countParam += num1 - num2;
    }
    bool result = this.GetResult(countParam == 0);
    if (!result)
      reason = new RuleInvalidReason(GameStrings.Format("GLUE_COLLECTION_DECK_RULE_MISSING_CARDS", (object) countParam), countParam);
    return result;
  }
}
