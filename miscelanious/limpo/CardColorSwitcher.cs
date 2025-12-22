using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class CardColorSwitcher : MonoBehaviour
{
  private static CardColorSwitcher s_instance;
  [CustomEditField(Sections = "Spells", T = EditType.TEXTURE)]
  public List<string> spellCardTextures;
  [CustomEditField(Sections = "Minions", T = EditType.TEXTURE)]
  public List<string> minionCardTextures;
  [CustomEditField(Sections = "Heroes", T = EditType.TEXTURE)]
  public List<string> heroCardTextures;
  [CustomEditField(Sections = "Weapons", T = EditType.TEXTURE)]
  public List<string> weaponCardTextures;
  [CustomEditField(Sections = "Mercenaries Abilities", T = EditType.TEXTURE)]
  public List<string> mercenariesAbilityCardTextures;
  [CustomEditField(Sections = "Locations", T = EditType.TEXTURE)]
  public List<string> locationCardTextures;

  private void Awake()
  {
    CardColorSwitcher.s_instance = this;
    this.gameObject.AddComponent<HSDontDestroyOnLoad>();
  }

  private void OnDestroy() => CardColorSwitcher.s_instance = (CardColorSwitcher) null;

  public static CardColorSwitcher Get() => CardColorSwitcher.s_instance;

  public AssetReference GetTexture(
    TAG_CARDTYPE cardType,
    CardColorSwitcher.CardColorType colorType)
  {
    List<string> stringList;
    switch (cardType)
    {
      case TAG_CARDTYPE.HERO:
        stringList = this.heroCardTextures;
        break;
      case TAG_CARDTYPE.MINION:
        stringList = this.minionCardTextures;
        break;
      case TAG_CARDTYPE.SPELL:
        stringList = this.spellCardTextures;
        break;
      case TAG_CARDTYPE.WEAPON:
        stringList = this.weaponCardTextures;
        break;
      case TAG_CARDTYPE.LETTUCE_ABILITY:
        if (colorType < CardColorSwitcher.CardColorType.TYPE_MERCENARIES_NEUTRAL_TIER_1)
        {
          Log.Lettuce.PrintError("CardColorSwitcher.GetTexture: Invalid mercenary ability type {0}", (object) colorType);
          return (AssetReference) null;
        }
        colorType -= CardColorSwitcher.CardColorType.TYPE_MERCENARIES_NEUTRAL_TIER_1;
        stringList = this.mercenariesAbilityCardTextures;
        break;
      case TAG_CARDTYPE.LOCATION:
        stringList = this.locationCardTextures;
        break;
      default:
        Debug.LogErrorFormat("Wrong cardType {0}", (object) cardType);
        stringList = this.minionCardTextures;
        break;
    }
    int index = (int) colorType;
    return stringList.Count <= index ? (AssetReference) null : (AssetReference) stringList[index];
  }

  public enum CardColorType
  {
    TYPE_GENERIC = 0,
    TYPE_WARLOCK = 1,
    TYPE_ROGUE = 2,
    TYPE_DRUID = 3,
    TYPE_SHAMAN = 4,
    TYPE_HUNTER = 5,
    TYPE_MAGE = 6,
    TYPE_PALADIN = 7,
    TYPE_PRIEST = 8,
    TYPE_WARRIOR = 9,
    TYPE_DEATHKNIGHT = 10, // 0x0000000A
    TYPE_DEMONHUNTER = 11, // 0x0000000B
    TYPE_PALADIN_PRIEST = 12, // 0x0000000C
    TYPE_WARLOCK_PRIEST = 13, // 0x0000000D
    TYPE_WARLOCK_DEMONHUNTER = 14, // 0x0000000E
    TYPE_HUNTER_DEMONHUNTER = 15, // 0x0000000F
    TYPE_DRUID_HUNTER = 16, // 0x00000010
    TYPE_DRUID_SHAMAN = 17, // 0x00000011
    TYPE_SHAMAN_MAGE = 18, // 0x00000012
    TYPE_MAGE_ROGUE = 19, // 0x00000013
    TYPE_WARRIOR_ROGUE = 20, // 0x00000014
    TYPE_WARRIOR_PALADIN = 21, // 0x00000015
    TYPE_MERCENARIES_CASTER_TIER_1 = 22, // 0x00000016
    TYPE_MERCENARIES_CASTER_TIER_2 = 23, // 0x00000017
    TYPE_MERCENARIES_CASTER_TIER_3 = 24, // 0x00000018
    TYPE_MERCENARIES_FIGHTER_TIER_1 = 25, // 0x00000019
    TYPE_MERCENARIES_FIGHTER_TIER_2 = 26, // 0x0000001A
    TYPE_MERCENARIES_FIGHTER_TIER_3 = 27, // 0x0000001B
    TYPE_MERCENARIES_TANK_TIER_1 = 28, // 0x0000001C
    TYPE_MERCENARIES_TANK_TIER_2 = 29, // 0x0000001D
    TYPE_MERCENARIES_TANK_TIER_3 = 30, // 0x0000001E
    TYPE_MERCENARIES_ABILITY_CASTER_SPELL = 31, // 0x0000001F
    TYPE_MERCENARIES_NEUTRAL_TIER_1 = 31, // 0x0000001F
    TYPE_MERCENARIES_ABILITY_CASTER_MINION = 32, // 0x00000020
    TYPE_MERCENARIES_NEUTRAL_TIER_2 = 32, // 0x00000020
    TYPE_MERCENARIES_ABILITY_FIGHTER_SPELL = 33, // 0x00000021
    TYPE_MERCENARIES_NEUTRAL_TIER_3 = 33, // 0x00000021
    TYPE_MERCENARIES_ABILITY_FIGHTER_MINION = 34, // 0x00000022
    TYPE_MERCENARIES_ABILITY_TANK_SPELL = 35, // 0x00000023
    TYPE_MERCENARIES_ABILITY_TANK_MINION = 36, // 0x00000024
    TYPE_MERCENARIES_ABILITY_NEUTRAL_SPELL = 37, // 0x00000025
    TYPE_MERCENARIES_ABILITY_NEUTRAL_MINION = 38, // 0x00000026
  }
}
