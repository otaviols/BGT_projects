using UnityEngine;

public class ReferenceScriptDataNum1EntityPower : CardTextBuilder
{
  private static bool m_building;

  public ReferenceScriptDataNum1EntityPower() => this.m_useEntityForTextInPlay = true;

  public override string BuildCardTextInHand(EntityDef entityDef)
  {
    string str = base.BuildCardTextInHand(entityDef);
    int length = str.IndexOf('@');
    if (length >= 0)
      str = str.Substring(0, length);
    return str;
  }

  public override string BuildCardTextInHand(Entity entity) => this.BuildTextWithReferenceEntityPower(entity);

  public override string BuildCardTextInHistory(Entity entity) => this.BuildTextWithReferenceEntityPower(entity);

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

  private string BuildTextWithReferenceEntityPower(Entity entity)
  {
    string rawCardTextInHand = CardTextBuilder.GetRawCardTextInHand(entity.GetCardId());
    if (ReferenceScriptDataNum1EntityPower.m_building)
    {
      string alternateCardText = this.GetAlternateCardText(rawCardTextInHand, 0);
      return TextUtils.TransformCardText(entity, alternateCardText);
    }
    Entity entity1 = GameState.Get().GetEntity(entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1));
    ReferenceScriptDataNum1EntityPower.m_building = true;
    string text = entity1 == null ? this.GetAlternateCardText(rawCardTextInHand, 0) : string.Format(this.GetAlternateCardText(rawCardTextInHand, 1), (object) entity1.GetCardTextBuilder().BuildCardTextInHand(entity1).Replace('\n', ' '));
    ReferenceScriptDataNum1EntityPower.m_building = false;
    return TextUtils.TransformCardText(entity, text);
  }

  public override void OnTagChange(Card card, TagDelta tagChange)
  {
    if (tagChange.tag == 2)
    {
      if (!((Object) card != (Object) null) || !((Object) card.GetActor() != (Object) null))
        return;
      card.GetActor().UpdateTextComponents();
    }
    else
      base.OnTagChange(card, tagChange);
  }
}
