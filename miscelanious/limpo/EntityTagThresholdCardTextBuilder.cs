using UnityEngine;

public class EntityTagThresholdCardTextBuilder : CardTextBuilder
{
  public override string BuildCardTextInHand(Entity entity)
  {
    GAME_TAG tag1 = (GAME_TAG) entity.GetTag(GAME_TAG.ENTITY_TAG_THRESHOLD_TAG_ID);
    int tag2 = entity.GetTag(tag1);
    int tag3 = entity.GetTag(GAME_TAG.ENTITY_TAG_THRESHOLD_VALUE);
    string str1 = base.BuildCardTextInHand(entity);
    int length1 = str1.IndexOf('@');
    int num = str1.IndexOf('@', length1 + 1);
    if (length1 >= 0 && num >= 0)
    {
      string str2 = str1.Substring(0, length1);
      string str3;
      if (tag2 >= tag3)
      {
        str3 = str2 + str1.Substring(num + 1);
      }
      else
      {
        int length2 = num - length1 - 1;
        str3 = string.Format(str2 + str1.Substring(length1 + 1, length2), (object) (tag3 - tag2));
      }
      str1 = str3;
    }
    return str1;
  }

  public override string BuildCardTextInHand(EntityDef entityDef)
  {
    string str = base.BuildCardTextInHand(entityDef);
    int length = str.IndexOf('@');
    if (length >= 0)
      str = str.Substring(0, length);
    return str;
  }

  public override string BuildCardTextInHistory(Entity entity)
  {
    string str = base.BuildCardTextInHistory(entity);
    int length = str.IndexOf('@');
    if (length >= 0)
      str = str.Substring(0, length);
    return str;
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
