using UnityEngine;

public class GalakrondCounterCardTextBuilder : CardTextBuilder
{
  public GalakrondCounterCardTextBuilder() => this.m_useEntityForTextInPlay = true;

  public override string BuildCardTextInHand(Entity entity)
  {
    string text = CardTextBuilder.GetRawCardTextInHand(entity.GetCardId()).Replace("@", entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2) - entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) == 1 ? GameStrings.Get("GALAKROND_ONCE") : GameStrings.Get("GALAKROND_TWICE"));
    return TextUtils.TransformCardText(entity, text);
  }

  public override string BuildCardTextInHistory(Entity entity)
  {
    string text = CardTextBuilder.GetRawCardTextInHand(entity.GetCardId()).Replace("@", entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2) - entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) == 1 ? GameStrings.Get("GALAKROND_ONCE") : GameStrings.Get("GALAKROND_TWICE"));
    return TextUtils.TransformCardText(entity, text);
  }

  public override string BuildCardTextInHand(EntityDef entityDef) => TextUtils.TransformCardText(CardTextBuilder.GetRawCardTextInHand(entityDef.GetCardId()).Replace("@", entityDef.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2) - entityDef.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) == 1 ? GameStrings.Get("GALAKROND_ONCE") : GameStrings.Get("GALAKROND_TWICE")));

  public override void OnTagChange(Card card, TagDelta tagChange)
  {
    if (tagChange.tag != 2 || !((Object) card != (Object) null) || !((Object) card.GetActor() != (Object) null))
      return;
    card.GetActor().UpdateTextComponents();
  }
}
