using UnityEngine;

public class PlayerTagThresholdCardTextBuilder : CardTextBuilder
{
  public override string BuildCardTextInHand(Entity entity)
  {
    Player controller = entity.GetController();
    GAME_TAG tag1 = (GAME_TAG) entity.GetTag(GAME_TAG.PLAYER_TAG_THRESHOLD_TAG_ID);
    int num1 = controller != null ? controller.GetTag(tag1) : 0;
    int tag2 = entity.GetTag(GAME_TAG.PLAYER_TAG_THRESHOLD_VALUE);
    string str1 = base.BuildCardTextInHand(entity);
    int length1 = str1.IndexOf('@');
    int num2 = str1.IndexOf('@', length1 + 1);
    if (length1 >= 0 && num2 >= 0)
    {
      string str2 = str1.Substring(0, length1);
      string str3;
      if (num1 >= tag2)
      {
        str3 = str2 + str1.Substring(num2 + 1);
      }
      else
      {
        int length2 = num2 - length1 - 1;
        str3 = string.Format(str2 + str1.Substring(length1 + 1, length2), (object) (tag2 - num1));
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
