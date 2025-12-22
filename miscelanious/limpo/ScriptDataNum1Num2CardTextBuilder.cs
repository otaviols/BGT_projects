using UnityEngine;

public class ScriptDataNum1Num2CardTextBuilder : CardTextBuilder
{
  public ScriptDataNum1Num2CardTextBuilder() => this.m_useEntityForTextInPlay = true;

  public override string BuildCardTextInHand(Entity entity)
  {
    string rawCardTextInHand = CardTextBuilder.GetRawCardTextInHand(entity.GetCardId());
    string str1 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1).ToString();
    string str2 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2).ToString();
    string str3 = str1;
    string str4 = str2;
    string text = string.Format(rawCardTextInHand, (object) str3, (object) str4);
    return TextUtils.TransformCardText(entity, text);
  }

  public override string BuildCardTextInHistory(Entity entity)
  {
    string rawCardTextInHand = CardTextBuilder.GetRawCardTextInHand(entity.GetCardId());
    string str1 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1).ToString();
    string str2 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2).ToString();
    string str3 = str1;
    string str4 = str2;
    string text = string.Format(rawCardTextInHand, (object) str3, (object) str4);
    return TextUtils.TransformCardText(entity, text);
  }

  public override string BuildCardTextInHand(EntityDef entityDef)
  {
    string rawCardTextInHand = CardTextBuilder.GetRawCardTextInHand(entityDef.GetCardId());
    string str1 = entityDef.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1).ToString();
    string str2 = entityDef.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2).ToString();
    string str3 = str1;
    string str4 = str2;
    return TextUtils.TransformCardText(string.Format(rawCardTextInHand, (object) str3, (object) str4));
  }

  public override void OnTagChange(Card card, TagDelta tagChange)
  {
    switch (tagChange.tag)
    {
      case 2:
      case 3:
        if (!((Object) card != (Object) null) || !((Object) card.GetActor() != (Object) null))
          break;
        card.GetActor().UpdateTextComponents();
        break;
      default:
        base.OnTagChange(card, tagChange);
        break;
    }
  }
}
