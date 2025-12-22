using UnityEngine;

public class GameplayStringTextBuilder : CardTextBuilder
{
  private const string SPELLSTONE_DIAMOND = "LOOT_507";
  private const string SPELLSTONE_PEARL = "LOOT_091";
  private const string SPELLSTONE_SAPPHIRE = "LOOT_064";
  private const string SPELLSTONE_AMETHYST = "LOOT_043";
  private const string SPELLSTONE_JASPER = "LOOT_051";
  private const string SPELLSTONE_RUBY = "LOOT_103";
  private const string SPELLSTONE_ONYX = "LOOT_503";
  private const string DARKNESS_TOKEN = "LOOT_526d";
  private const string STASIS_DRAGON_TOKEN = "TOT_109t";
  private const string TROLL_SHRINE_TOKEN = "TRLA_1";

  public GameplayStringTextBuilder() => this.m_useEntityForTextInPlay = true;

  public override string BuildCardTextInHand(Entity entity)
  {
    string str = base.BuildCardTextInHand(entity);
    int num1 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) + 1;
    int tag = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2);
    if (num1 > tag)
      num1 = tag;
    int num2 = str.IndexOf("@");
    if (num2 != -1)
    {
      string newValue = GameStrings.Get(this.GetGameStringsKey(entity.GetCardId()) + num1.ToString());
      str = str.Substring(0, num2 + 1).Replace("@", newValue);
    }
    return str;
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

  public override string BuildCardTextInHand(EntityDef entityDef)
  {
    string str = base.BuildCardTextInHand(entityDef);
    int num = str.IndexOf("@");
    if (num != -1)
    {
      string newValue = GameStrings.Get(this.GetGameStringsKey(entityDef.GetCardId()) + "1");
      str = str.Substring(0, num + 1).Replace("@", newValue);
    }
    return str;
  }

  public override string BuildCardTextInHistory(Entity entity)
  {
    CardTextHistoryData cardTextHistoryData = entity.GetCardTextHistoryData();
    if (cardTextHistoryData == null)
    {
      Log.All.Print("DiamondSpellstoneTextBuilder.BuildCardTextInHistory: entity {0} does not have a CardTextHistoryData object.", (object) entity.GetEntityId());
      return "";
    }
    string text = CardTextBuilder.GetRawCardTextInHand(entity.GetCardId());
    int num1 = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) + 1;
    int tag = entity.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2);
    if (num1 > tag)
      num1 = tag;
    int num2 = text.IndexOf("@");
    if (num2 != -1)
    {
      string newValue = GameStrings.Get(this.GetGameStringsKey(entity.GetCardId()) + num1.ToString());
      text = text.Substring(0, num2 + 1).Replace("@", newValue);
    }
    return TextUtils.TransformCardText(cardTextHistoryData, text);
  }

  private string GetGameStringsKey(string cardID)
  {
    if (cardID.Contains("LOOT_507"))
      return "GAMEPLAY_DIAMOND_SPELLSTONE_";
    if (cardID.Contains("LOOT_091"))
      return "GAMEPLAY_PEARL_SPELLSTONE_";
    if (cardID.Contains("LOOT_064"))
      return "GAMEPLAY_SAPPHIRE_SPELLSTONE_";
    if (cardID.Contains("LOOT_051"))
      return "GAMEPLAY_JASPER_SPELLSTONE_";
    if (cardID.Contains("LOOT_043"))
      return "GAMEPLAY_AMETHYST_SPELLSTONE_";
    if (cardID.Contains("LOOT_103"))
      return "GAMEPLAY_RUBY_SPELLSTONE_";
    if (cardID.Contains("LOOT_503"))
      return "GAMEPLAY_ONYX_SPELLSTONE_";
    if (cardID.Contains("LOOT_526d"))
      return "GAMEPLAY_LOOT_526d_DARKNESS_";
    if (cardID.Contains("TOT_109t"))
      return "GAMEPLAY_TOT_109t_STASIS_DRAGON_";
    return cardID.Contains("TRLA_1") ? "GAMEPLAY_TRLA_TROLL_SHRINE_" : "";
  }
}
