using System.Collections.Generic;
using UnityEngine;

public class ScriptDataNum1CardTextBuilder : CardTextBuilder
{
  public ScriptDataNum1CardTextBuilder() => this.m_useEntityForTextInPlay = true;

  protected static List<int> GetDelimiterIndexList(string text)
  {
    List<int> delimiterIndexList = new List<int>();
    for (int index = text.IndexOf('@'); index >= 0; index = text.IndexOf('@', index + 1))
      delimiterIndexList.Add(index);
    return delimiterIndexList;
  }

  protected string BuildCardTextInternal(Entity entity)
  {
    string rawCardTextInHand = CardTextBuilder.GetRawCardTextInHand(entity.GetCardId());
    string newValue = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1).ToString();
    List<int> delimiterIndexList = ScriptDataNum1CardTextBuilder.GetDelimiterIndexList(rawCardTextInHand);
    string text = delimiterIndexList.Count != 2 || entity.GetEntityDef().GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) != 0 ? rawCardTextInHand.Replace("@", newValue) : rawCardTextInHand.Substring(0, delimiterIndexList[0]) + rawCardTextInHand.Substring(delimiterIndexList[0] + 1).Replace("@", newValue);
    return TextUtils.TransformCardText(entity, text);
  }

  public override string BuildCardTextInHand(Entity entity) => this.BuildCardTextInternal(entity);

  public override string BuildCardTextInHistory(Entity entity) => this.BuildCardTextInternal(entity);

  public override string BuildCardTextInHand(EntityDef entityDef)
  {
    string rawCardTextInHand = CardTextBuilder.GetRawCardTextInHand(entityDef.GetCardId());
    List<int> delimiterIndexList = ScriptDataNum1CardTextBuilder.GetDelimiterIndexList(rawCardTextInHand);
    if (delimiterIndexList.Count == 2 && entityDef.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) == 0)
      return TextUtils.TransformCardText(rawCardTextInHand.Substring(0, delimiterIndexList[0]));
    string newValue = entityDef.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1).ToString();
    return TextUtils.TransformCardText(rawCardTextInHand.Replace("@", newValue));
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

  public override string GetTargetingArrowText(Entity entity) => TextUtils.TransformCardText(base.GetTargetingArrowText(entity).Replace("@", entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1).ToString()));
}
