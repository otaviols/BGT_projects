using UnityEngine;

public class AlternateCardTextCardTextBuilder : CardTextBuilder
{
  public AlternateCardTextCardTextBuilder() => this.m_useEntityForTextInPlay = true;

  public override string BuildCardTextInHand(Entity entity) => this.GetAlternateCardText(base.BuildCardTextInHand(entity), entity.GetTag(GAME_TAG.USE_ALTERNATE_CARD_TEXT));

  private string GetAlternateCardText(string builtText, int alternateCardTextIndex)
  {
    int length = builtText.IndexOf('@');
    if (length < 0)
      return builtText;
    for (int index = 0; index < alternateCardTextIndex; ++index)
    {
      builtText = builtText.Substring(length + 1);
      length = builtText.IndexOf('@');
      if (length < 0)
        break;
    }
    if (length >= 0)
      builtText = builtText.Substring(0, length);
    return builtText;
  }

  public override string BuildCardTextInHand(EntityDef entityDef) => this.GetAlternateCardText(base.BuildCardTextInHand(entityDef), entityDef.GetTag(GAME_TAG.USE_ALTERNATE_CARD_TEXT));

  public override string BuildCardTextInHistory(Entity entity) => this.GetAlternateCardText(base.BuildCardTextInHand(entity), entity.GetTag(GAME_TAG.USE_ALTERNATE_CARD_TEXT));

  public override void OnTagChange(Card card, TagDelta tagChange)
  {
    if (tagChange.tag == 955)
    {
      if (!((Object) card != (Object) null) || !((Object) card.GetActor() != (Object) null))
        return;
      card.GetActor().UpdatePowersText();
    }
    else
      base.OnTagChange(card, tagChange);
  }
}
