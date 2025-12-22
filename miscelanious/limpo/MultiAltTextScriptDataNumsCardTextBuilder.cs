using Hearthstone;
using System.Reflection;
using UnityEngine;

public class MultiAltTextScriptDataNumsCardTextBuilder : CardTextBuilder
{
  public MultiAltTextScriptDataNumsCardTextBuilder() => this.m_useEntityForTextInPlay = true;

  private string GetAlternateTextSubstring(string baseText, Entity entity)
  {
    if (string.IsNullOrEmpty(baseText) || entity == null)
      return string.Empty;
    string[] strArray = baseText.Split('@');
    int index = entity.GetTag(GAME_TAG.USE_ALTERNATE_CARD_TEXT);
    if (index < 0 || index >= strArray.Length)
    {
      Log.Gameplay.PrintWarning(MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name + "(): " + string.Format("index of alternate text ({0}) on Entity {1} ({2}) ", (object) index, (object) entity.GetEntityId(), (object) entity.GetName()) + string.Format("is outside of the valid range [0, {0}]. Value rounded to nearest valid one.", (object) (strArray.Length - 1)));
      index = Mathf.Clamp(index, 0, strArray.Length - 1);
    }
    return strArray[index];
  }

  private string GetAlternateTextSubstring(string baseText, EntityDef entityDef)
  {
    if (string.IsNullOrEmpty(baseText) || entityDef == null)
      return string.Empty;
    string[] strArray = baseText.Split('@');
    int index = entityDef.GetTag(GAME_TAG.USE_ALTERNATE_CARD_TEXT);
    if (index < 0 || index >= strArray.Length)
    {
      Log.Gameplay.PrintWarning(MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name + "(): " + string.Format("index of alternate text ({0}) on EntityDef ({1}) ", (object) index, (object) entityDef.GetName()) + string.Format("is outside of the valid range [0, {0}]. Value rounded to nearest valid one.", (object) (strArray.Length - 1)));
      index = Mathf.Clamp(index, 0, strArray.Length - 1);
    }
    return strArray[index];
  }

  private string SubstituteScriptDataNums(string rawText, Entity entity)
  {
    if (string.IsNullOrEmpty(rawText))
      return string.Empty;
    if (entity == null)
      return rawText;
    string format = rawText;
    if (rawText.Contains("{0}"))
    {
      int tag1 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
      if (rawText.Contains("{1}"))
      {
        int tag2 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2);
        format = string.Format(format, (object) tag1, (object) tag2);
      }
      else
        format = string.Format(format, (object) tag1);
    }
    return format;
  }

  private string SubstituteScriptDataNums(string rawText, EntityDef entityDef)
  {
    if (string.IsNullOrEmpty(rawText))
      return string.Empty;
    if (entityDef == null)
      return rawText;
    string format = rawText;
    if (rawText.Contains("{0}"))
    {
      int tag1 = entityDef.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1);
      if (rawText.Contains("{1}"))
      {
        int tag2 = entityDef.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2);
        format = string.Format(format, (object) tag1, (object) tag2);
      }
      else
        format = string.Format(format, (object) tag1);
    }
    return format;
  }

  private string BuildCardTextForEntity(Entity entity)
  {
    if (entity == null)
    {
      if (!HearthstoneApplication.IsPublic())
        return string.Empty;
      return "Error: parameter entity in " + MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name + "() is null.";
    }
    string text = this.SubstituteScriptDataNums(CardTextBuilder.GetRawCardTextInHand(entity.GetCardId()), entity);
    return this.GetAlternateTextSubstring(TextUtils.TransformCardText(entity, text), entity);
  }

  public override string BuildCardTextInHand(Entity entity) => this.BuildCardTextForEntity(entity);

  public override string BuildCardTextInHand(EntityDef entityDef) => this.GetAlternateTextSubstring(TextUtils.TransformCardText(this.SubstituteScriptDataNums(CardTextBuilder.GetRawCardTextInHand(entityDef.GetCardId()), entityDef)), entityDef);

  public override string BuildCardTextInHistory(Entity entity) => this.BuildCardTextForEntity(entity);
}
