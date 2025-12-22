using System;

public class Tags
{
  public static string DebugTag(int tag, int val)
  {
    string str1 = tag.ToString();
    try
    {
      str1 = ((GAME_TAG) tag).ToString();
    }
    catch (Exception ex)
    {
    }
    string str2 = val.ToString();
    switch ((GAME_TAG) tag)
    {
      case GAME_TAG.PLAYSTATE:
        try
        {
          str2 = ((TAG_PLAYSTATE) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.STEP:
      case GAME_TAG.NEXT_STEP:
        try
        {
          str2 = ((TAG_STEP) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.ZONE:
        try
        {
          str2 = ((TAG_ZONE) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.CARD_SET:
        try
        {
          str2 = ((TAG_CARD_SET) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.CLASS:
        try
        {
          str2 = ((TAG_CLASS) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.CARDRACE:
        try
        {
          str2 = ((TAG_RACE) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.FACTION:
        try
        {
          str2 = ((TAG_FACTION) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.CARDTYPE:
        try
        {
          str2 = ((TAG_CARDTYPE) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.RARITY:
        try
        {
          str2 = ((TAG_RARITY) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.STATE:
        try
        {
          str2 = ((TAG_STATE) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.MULLIGAN_STATE:
        try
        {
          str2 = ((TAG_MULLIGAN) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
      case GAME_TAG.ENCHANTMENT_BIRTH_VISUAL:
      case GAME_TAG.ENCHANTMENT_IDLE_VISUAL:
        try
        {
          str2 = ((TAG_ENCHANTMENT_VISUAL) val).ToString();
          break;
        }
        catch (Exception ex)
        {
          break;
        }
    }
    return string.Format("tag={0} value={1}", (object) str1, (object) str2);
  }
}
