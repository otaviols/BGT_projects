using Assets;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Hearthstone.Util;
using PegasusShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class GameStrings
{
  public const string s_UnknownName = "UNKNOWN";
  private static Map<Global.GameStringCategory, GameStringTable> s_tables = new Map<Global.GameStringCategory, GameStringTable>();
  private static readonly char[] LANGUAGE_RULE_ARG_DELIMITERS = new char[1]
  {
    ','
  };
  private static List<Global.GameStringCategory> s_nativeGameStringCatetories = new List<Global.GameStringCategory>()
  {
    Global.GameStringCategory.GLOBAL,
    Global.GameStringCategory.GLUE
  };
  private const string NUMBER_PATTERN = "(?<!\\/)(?:[0-9]+,)*[0-9]+(?!\\/)";
  private const string NUMBER_PATTERN_ALT = "(?<!\\/)(?:[0-9]+,)*[0-9]+";
  private const string SKIPPABLE_KOREAN_CHARACTERS = ")}]:;?/*&^!~`/\\|_'\"";
  private const int KOREAN_NO_JONGSEONG = 51060;
  private const int KOREAN_JONGSEONG = 50689;
  private const int KOREAN_RIEUL_JONGSEONG = 51068;
  public static Map<TAG_CLASS, string> s_classNames = new Map<TAG_CLASS, string>()
  {
    {
      TAG_CLASS.DEATHKNIGHT,
      "GLOBAL_CLASS_DEATHKNIGHT"
    },
    {
      TAG_CLASS.DRUID,
      "GLOBAL_CLASS_DRUID"
    },
    {
      TAG_CLASS.HUNTER,
      "GLOBAL_CLASS_HUNTER"
    },
    {
      TAG_CLASS.MAGE,
      "GLOBAL_CLASS_MAGE"
    },
    {
      TAG_CLASS.PALADIN,
      "GLOBAL_CLASS_PALADIN"
    },
    {
      TAG_CLASS.PRIEST,
      "GLOBAL_CLASS_PRIEST"
    },
    {
      TAG_CLASS.ROGUE,
      "GLOBAL_CLASS_ROGUE"
    },
    {
      TAG_CLASS.SHAMAN,
      "GLOBAL_CLASS_SHAMAN"
    },
    {
      TAG_CLASS.WARLOCK,
      "GLOBAL_CLASS_WARLOCK"
    },
    {
      TAG_CLASS.WARRIOR,
      "GLOBAL_CLASS_WARRIOR"
    },
    {
      TAG_CLASS.DEMONHUNTER,
      "GLOBAL_CLASS_DEMONHUNTER"
    },
    {
      TAG_CLASS.NEUTRAL,
      "GLOBAL_CLASS_NEUTRAL"
    }
  };
  public static Map<TAG_RACE, string> s_raceNames = new Map<TAG_RACE, string>()
  {
    {
      TAG_RACE.BLOODELF,
      "GLOBAL_RACE_BLOODELF"
    },
    {
      TAG_RACE.DRAENEI,
      "GLOBAL_RACE_DRAENEI"
    },
    {
      TAG_RACE.DWARF,
      "GLOBAL_RACE_DWARF"
    },
    {
      TAG_RACE.GNOME,
      "GLOBAL_RACE_GNOME"
    },
    {
      TAG_RACE.GOBLIN,
      "GLOBAL_RACE_GOBLIN"
    },
    {
      TAG_RACE.HUMAN,
      "GLOBAL_RACE_HUMAN"
    },
    {
      TAG_RACE.NIGHTELF,
      "GLOBAL_RACE_NIGHTELF"
    },
    {
      TAG_RACE.ORC,
      "GLOBAL_RACE_ORC"
    },
    {
      TAG_RACE.TAUREN,
      "GLOBAL_RACE_TAUREN"
    },
    {
      TAG_RACE.TROLL,
      "GLOBAL_RACE_TROLL"
    },
    {
      TAG_RACE.UNDEAD,
      "GLOBAL_RACE_UNDEAD"
    },
    {
      TAG_RACE.WORGEN,
      "GLOBAL_RACE_WORGEN"
    },
    {
      TAG_RACE.MURLOC,
      "GLOBAL_RACE_MURLOC"
    },
    {
      TAG_RACE.DEMON,
      "GLOBAL_RACE_DEMON"
    },
    {
      TAG_RACE.SCOURGE,
      "GLOBAL_RACE_SCOURGE"
    },
    {
      TAG_RACE.MECHANICAL,
      "GLOBAL_RACE_MECHANICAL"
    },
    {
      TAG_RACE.ELEMENTAL,
      "GLOBAL_RACE_ELEMENTAL"
    },
    {
      TAG_RACE.OGRE,
      "GLOBAL_RACE_OGRE"
    },
    {
      TAG_RACE.PET,
      "GLOBAL_RACE_PET"
    },
    {
      TAG_RACE.TOTEM,
      "GLOBAL_RACE_TOTEM"
    },
    {
      TAG_RACE.NERUBIAN,
      "GLOBAL_RACE_NERUBIAN"
    },
    {
      TAG_RACE.PIRATE,
      "GLOBAL_RACE_PIRATE"
    },
    {
      TAG_RACE.DRAGON,
      "GLOBAL_RACE_DRAGON"
    },
    {
      TAG_RACE.ALL,
      "GLOBAL_RACE_ALL"
    },
    {
      TAG_RACE.EGG,
      "GLOBAL_RACE_EGG"
    },
    {
      TAG_RACE.QUILBOAR,
      "GLOBAL_RACE_QUILBOAR"
    },
    {
      TAG_RACE.CENTAUR,
      "GLOBAL_RACE_CENTAUR"
    },
    {
      TAG_RACE.FURBOLG,
      "GLOBAL_RACE_FURBOLG"
    },
    {
      TAG_RACE.HIGHELF,
      "GLOBAL_RACE_HIGHELF"
    },
    {
      TAG_RACE.TREANT,
      "GLOBAL_RACE_TREANT"
    },
    {
      TAG_RACE.OWLKIN,
      "GLOBAL_RACE_OWLKIN"
    },
    {
      TAG_RACE.HALFORC,
      "GLOBAL_RACE_HALFORC"
    },
    {
      TAG_RACE.LOCK,
      "GLOBAL_RACE_LOCK"
    },
    {
      TAG_RACE.NAGA,
      "GLOBAL_RACE_NAGA"
    },
    {
      TAG_RACE.OLDGOD,
      "GLOBAL_RACE_OLDGOD"
    },
    {
      TAG_RACE.PANDAREN,
      "GLOBAL_RACE_PANDAREN"
    },
    {
      TAG_RACE.GRONN,
      "GLOBAL_RACE_GRONN"
    },
    {
      TAG_RACE.CELESTIAL,
      "GLOBAL_RACE_CELESTIAL"
    },
    {
      TAG_RACE.GNOLL,
      "GLOBAL_RACE_GNOLL"
    },
    {
      TAG_RACE.GOLEM,
      "GLOBAL_RACE_GOLEM"
    },
    {
      TAG_RACE.HARPY,
      "GLOBAL_RACE_HARPY"
    },
    {
      TAG_RACE.VULPERA,
      "GLOBAL_RACE_VULPERA"
    }
  };
  public static Map<TAG_RACE, string> s_raceNamesBattlegrounds = new Map<TAG_RACE, string>()
  {
    {
      TAG_RACE.BLOODELF,
      "GLOBAL_RACE_BLOODELF_BATTLEGROUNDS"
    },
    {
      TAG_RACE.DRAENEI,
      "GLOBAL_RACE_DRAENEI_BATTLEGROUNDS"
    },
    {
      TAG_RACE.DWARF,
      "GLOBAL_RACE_DWARF_BATTLEGROUNDS"
    },
    {
      TAG_RACE.GNOME,
      "GLOBAL_RACE_GNOME_BATTLEGROUNDS"
    },
    {
      TAG_RACE.GOBLIN,
      "GLOBAL_RACE_GOBLIN_BATTLEGROUNDS"
    },
    {
      TAG_RACE.HUMAN,
      "GLOBAL_RACE_HUMAN_BATTLEGROUNDS"
    },
    {
      TAG_RACE.NIGHTELF,
      "GLOBAL_RACE_NIGHTELF_BATTLEGROUNDS"
    },
    {
      TAG_RACE.ORC,
      "GLOBAL_RACE_ORC_BATTLEGROUNDS"
    },
    {
      TAG_RACE.TAUREN,
      "GLOBAL_RACE_TAUREN_BATTLEGROUNDS"
    },
    {
      TAG_RACE.TROLL,
      "GLOBAL_RACE_TROLL_BATTLEGROUNDS"
    },
    {
      TAG_RACE.UNDEAD,
      "GLOBAL_RACE_UNDEAD_BATTLEGROUNDS"
    },
    {
      TAG_RACE.WORGEN,
      "GLOBAL_RACE_WORGEN_BATTLEGROUNDS"
    },
    {
      TAG_RACE.MURLOC,
      "GLOBAL_RACE_MURLOC_BATTLEGROUNDS"
    },
    {
      TAG_RACE.DEMON,
      "GLOBAL_RACE_DEMON_BATTLEGROUNDS"
    },
    {
      TAG_RACE.SCOURGE,
      "GLOBAL_RACE_SCOURGE_BATTLEGROUNDS"
    },
    {
      TAG_RACE.MECHANICAL,
      "GLOBAL_RACE_MECHANICAL_BATTLEGROUNDS"
    },
    {
      TAG_RACE.ELEMENTAL,
      "GLOBAL_RACE_ELEMENTAL_BATTLEGROUNDS"
    },
    {
      TAG_RACE.OGRE,
      "GLOBAL_RACE_OGRE_BATTLEGROUNDS"
    },
    {
      TAG_RACE.PET,
      "GLOBAL_RACE_PET_BATTLEGROUNDS"
    },
    {
      TAG_RACE.TOTEM,
      "GLOBAL_RACE_TOTEM_BATTLEGROUNDS"
    },
    {
      TAG_RACE.NERUBIAN,
      "GLOBAL_RACE_NERUBIAN_BATTLEGROUNDS"
    },
    {
      TAG_RACE.PIRATE,
      "GLOBAL_RACE_PIRATE_BATTLEGROUNDS"
    },
    {
      TAG_RACE.DRAGON,
      "GLOBAL_RACE_DRAGON_BATTLEGROUNDS"
    },
    {
      TAG_RACE.ALL,
      "GLOBAL_RACE_ALL_BATTLEGROUNDS"
    },
    {
      TAG_RACE.EGG,
      "GLOBAL_RACE_EGG_BATTLEGROUNDS"
    },
    {
      TAG_RACE.NAGA,
      "GLOBAL_RACE_NAGA_BATTLEGROUNDS"
    },
    {
      TAG_RACE.QUILBOAR,
      "GLOBAL_RACE_QUILBOARS_BATTLEGROUNDS"
    }
  };
  public static Map<TAG_RARITY, string> s_rarityNames = new Map<TAG_RARITY, string>()
  {
    {
      TAG_RARITY.COMMON,
      "GLOBAL_RARITY_COMMON"
    },
    {
      TAG_RARITY.EPIC,
      "GLOBAL_RARITY_EPIC"
    },
    {
      TAG_RARITY.LEGENDARY,
      "GLOBAL_RARITY_LEGENDARY"
    },
    {
      TAG_RARITY.RARE,
      "GLOBAL_RARITY_RARE"
    },
    {
      TAG_RARITY.FREE,
      "GLOBAL_RARITY_FREE"
    }
  };
  public static Map<TAG_PREMIUM, string> s_premiumNames = new Map<TAG_PREMIUM, string>()
  {
    {
      TAG_PREMIUM.NORMAL,
      "GLOBAL_COLLECTION_NORMAL"
    },
    {
      TAG_PREMIUM.GOLDEN,
      "GLOBAL_COLLECTION_GOLDEN"
    },
    {
      TAG_PREMIUM.DIAMOND,
      "GLOBAL_COLLECTION_DIAMOND"
    },
    {
      TAG_PREMIUM.SIGNATURE,
      "GLOBAL_COLLECTION_SIGNATURE"
    }
  };
  public static Map<TAG_CARD_SET, string> s_cardSetNames = new Map<TAG_CARD_SET, string>()
  {
    {
      TAG_CARD_SET.BASIC,
      "GLOBAL_CARD_SET_BASIC"
    },
    {
      TAG_CARD_SET.EXPERT1,
      "GLOBAL_CARD_SET_EXPERT1"
    },
    {
      TAG_CARD_SET.HOF,
      "GLOBAL_CARD_SET_HOF"
    },
    {
      TAG_CARD_SET.PROMO,
      "GLOBAL_CARD_SET_PROMO"
    },
    {
      TAG_CARD_SET.FP1,
      "GLOBAL_CARD_SET_NAXX"
    },
    {
      TAG_CARD_SET.PE1,
      "GLOBAL_CARD_SET_GVG"
    },
    {
      TAG_CARD_SET.BRM,
      "GLOBAL_CARD_SET_BRM"
    },
    {
      TAG_CARD_SET.TGT,
      "GLOBAL_CARD_SET_TGT"
    },
    {
      TAG_CARD_SET.LOE,
      "GLOBAL_CARD_SET_LOE"
    },
    {
      TAG_CARD_SET.OG,
      "GLOBAL_CARD_SET_OG"
    },
    {
      TAG_CARD_SET.OG_RESERVE,
      "GLOBAL_CARD_SET_OG_RESERVE"
    },
    {
      TAG_CARD_SET.SLUSH,
      "GLOBAL_CARD_SET_DEBUG"
    },
    {
      TAG_CARD_SET.KARA,
      "GLOBAL_CARD_SET_KARA"
    },
    {
      TAG_CARD_SET.KARA_RESERVE,
      "GLOBAL_CARD_SET_KARA_RESERVE"
    },
    {
      TAG_CARD_SET.GANGS,
      "GLOBAL_CARD_SET_GANGS"
    },
    {
      TAG_CARD_SET.GANGS_RESERVE,
      "GLOBAL_CARD_SET_GANGS_RESERVE"
    },
    {
      TAG_CARD_SET.UNGORO,
      "GLOBAL_CARD_SET_UNGORO"
    },
    {
      TAG_CARD_SET.ICECROWN,
      "GLOBAL_CARD_SET_ICECROWN"
    },
    {
      TAG_CARD_SET.LOOTAPALOOZA,
      "GLOBAL_CARD_SET_LOOTAPALOOZA"
    },
    {
      TAG_CARD_SET.GILNEAS,
      "GLOBAL_CARD_SET_GILNEAS"
    },
    {
      TAG_CARD_SET.BOOMSDAY,
      "GLOBAL_CARD_SET_BOOMSDAY"
    },
    {
      TAG_CARD_SET.TROLL,
      "GLOBAL_CARD_SET_TROLL"
    },
    {
      TAG_CARD_SET.DALARAN,
      "GLOBAL_CARD_SET_DALARAN"
    },
    {
      TAG_CARD_SET.ULDUM,
      "GLOBAL_CARD_SET_ULDUM"
    },
    {
      TAG_CARD_SET.WILD_EVENT,
      "GLOBAL_CARD_SET_WILD_EVENT"
    },
    {
      TAG_CARD_SET.DRAGONS,
      "GLOBAL_CARD_SET_DRG"
    },
    {
      TAG_CARD_SET.YEAR_OF_THE_DRAGON,
      "GLOBAL_CARD_SET_YOD"
    },
    {
      TAG_CARD_SET.BLACK_TEMPLE,
      "GLOBAL_CARD_SET_BT"
    },
    {
      TAG_CARD_SET.DEMON_HUNTER_INITIATE,
      "GLOBAL_CARD_SET_DHI"
    },
    {
      TAG_CARD_SET.SCHOLOMANCE,
      "GLOBAL_CARD_SET_SCH"
    },
    {
      TAG_CARD_SET.DARKMOON_FAIRE,
      "GLOBAL_CARD_SET_DMF"
    },
    {
      TAG_CARD_SET.THE_BARRENS,
      "GLOBAL_CARD_SET_BAR"
    },
    {
      TAG_CARD_SET.LEGACY,
      "GLOBAL_CARD_SET_LEGACY"
    },
    {
      TAG_CARD_SET.CORE,
      "GLOBAL_CARD_SET_CORE"
    },
    {
      TAG_CARD_SET.VANILLA,
      "GLOBAL_CARD_SET_VANILLA"
    },
    {
      TAG_CARD_SET.STORMWIND,
      "GLOBAL_CARD_SET_SW"
    },
    {
      TAG_CARD_SET.ALTERAC_VALLEY,
      "GLOBAL_CARD_SET_AV"
    },
    {
      TAG_CARD_SET.THE_SUNKEN_CITY,
      "GLOBAL_CARD_SET_TSC"
    },
    {
      TAG_CARD_SET.REVENDRETH,
      "GLOBAL_CARD_SET_REV"
    },
    {
      TAG_CARD_SET.RETURN_OF_THE_LICH_KING,
      "GLOBAL_CARD_SET_RLK"
    },
    {
      TAG_CARD_SET.PATH_OF_ARTHAS,
      "GLOBAL_CARD_SET_PA"
    }
  };
  public static Map<TAG_CARD_SET, string> s_cardSetNamesShortened = new Map<TAG_CARD_SET, string>()
  {
    {
      TAG_CARD_SET.BASIC,
      "GLOBAL_CARD_SET_BASIC"
    },
    {
      TAG_CARD_SET.EXPERT1,
      "GLOBAL_CARD_SET_EXPERT1"
    },
    {
      TAG_CARD_SET.HOF,
      "GLOBAL_CARD_SET_HOF"
    },
    {
      TAG_CARD_SET.PROMO,
      "GLOBAL_CARD_SET_PROMO"
    },
    {
      TAG_CARD_SET.FP1,
      "GLOBAL_CARD_SET_NAXX"
    },
    {
      TAG_CARD_SET.PE1,
      "GLOBAL_CARD_SET_GVG"
    },
    {
      TAG_CARD_SET.BRM,
      "GLOBAL_CARD_SET_BRM"
    },
    {
      TAG_CARD_SET.TGT,
      "GLOBAL_CARD_SET_TGT_SHORT"
    },
    {
      TAG_CARD_SET.LOE,
      "GLOBAL_CARD_SET_LOE_SHORT"
    },
    {
      TAG_CARD_SET.OG,
      "GLOBAL_CARD_SET_OG_SHORT"
    },
    {
      TAG_CARD_SET.OG_RESERVE,
      "GLOBAL_CARD_SET_OG_RESERVE"
    },
    {
      TAG_CARD_SET.SLUSH,
      "GLOBAL_CARD_SET_DEBUG"
    },
    {
      TAG_CARD_SET.KARA,
      "GLOBAL_CARD_SET_KARA_SHORT"
    },
    {
      TAG_CARD_SET.KARA_RESERVE,
      "GLOBAL_CARD_SET_KARA_RESERVE"
    },
    {
      TAG_CARD_SET.GANGS,
      "GLOBAL_CARD_SET_GANGS_SHORT"
    },
    {
      TAG_CARD_SET.GANGS_RESERVE,
      "GLOBAL_CARD_SET_GANGS_RESERVE"
    },
    {
      TAG_CARD_SET.UNGORO,
      "GLOBAL_CARD_SET_UNGORO_SHORT"
    },
    {
      TAG_CARD_SET.ICECROWN,
      "GLOBAL_CARD_SET_ICECROWN_SHORT"
    },
    {
      TAG_CARD_SET.LOOTAPALOOZA,
      "GLOBAL_CARD_SET_LOOTAPALOOZA_SHORT"
    },
    {
      TAG_CARD_SET.GILNEAS,
      "GLOBAL_CARD_SET_GILNEAS_SHORT"
    },
    {
      TAG_CARD_SET.BOOMSDAY,
      "GLOBAL_CARD_SET_BOOMSDAY_SHORT"
    },
    {
      TAG_CARD_SET.TROLL,
      "GLOBAL_CARD_SET_TROLL_SHORT"
    },
    {
      TAG_CARD_SET.DALARAN,
      "GLOBAL_CARD_SET_DALARAN_SHORT"
    },
    {
      TAG_CARD_SET.ULDUM,
      "GLOBAL_CARD_SET_ULDUM_SHORT"
    },
    {
      TAG_CARD_SET.WILD_EVENT,
      "GLOBAL_CARD_SET_WILD_EVENT_SHORT"
    },
    {
      TAG_CARD_SET.DRAGONS,
      "GLOBAL_CARD_SET_DRG_SHORT"
    },
    {
      TAG_CARD_SET.YEAR_OF_THE_DRAGON,
      "GLOBAL_CARD_SET_YOD_SHORT"
    },
    {
      TAG_CARD_SET.BLACK_TEMPLE,
      "GLOBAL_CARD_SET_BT_SHORT"
    },
    {
      TAG_CARD_SET.DEMON_HUNTER_INITIATE,
      "GLOBAL_CARD_SET_DHI_SHORT"
    },
    {
      TAG_CARD_SET.SCHOLOMANCE,
      "GLOBAL_CARD_SET_SCH_SHORT"
    },
    {
      TAG_CARD_SET.DARKMOON_FAIRE,
      "GLOBAL_CARD_SET_DMF_SHORT"
    },
    {
      TAG_CARD_SET.THE_BARRENS,
      "GLOBAL_CARD_SET_BAR_SHORT"
    },
    {
      TAG_CARD_SET.LEGACY,
      "GLOBAL_CARD_SET_LEGACY_SHORT"
    },
    {
      TAG_CARD_SET.CORE,
      "GLOBAL_CARD_SET_CORE_SHORT"
    },
    {
      TAG_CARD_SET.VANILLA,
      "GLOBAL_CARD_SET_VANILLA_SHORT"
    },
    {
      TAG_CARD_SET.STORMWIND,
      "GLOBAL_CARD_SET_SW_SHORT"
    },
    {
      TAG_CARD_SET.ALTERAC_VALLEY,
      "GLOBAL_CARD_SET_AV_SHORT"
    },
    {
      TAG_CARD_SET.THE_SUNKEN_CITY,
      "GLOBAL_CARD_SET_TSC_SHORT"
    },
    {
      TAG_CARD_SET.REVENDRETH,
      "GLOBAL_CARD_SET_REV_SHORT"
    },
    {
      TAG_CARD_SET.RETURN_OF_THE_LICH_KING,
      "GLOBAL_CARD_SET_RLK_SHORT"
    },
    {
      TAG_CARD_SET.PATH_OF_ARTHAS,
      "GLOBAL_CARD_SET_PA_SHORT"
    }
  };
  public static Map<TAG_CARD_SET, string> s_cardSetNamesInitials = new Map<TAG_CARD_SET, string>()
  {
    {
      TAG_CARD_SET.FP1,
      "GLOBAL_CARD_SET_NAXX_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.PE1,
      "GLOBAL_CARD_SET_GVG_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.BRM,
      "GLOBAL_CARD_SET_BRM_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.TGT,
      "GLOBAL_CARD_SET_TGT_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.LOE,
      "GLOBAL_CARD_SET_LOE_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.OG,
      "GLOBAL_CARD_SET_OG_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.GANGS,
      "GLOBAL_CARD_SET_GANGS_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.LOOTAPALOOZA,
      "GLOBAL_CARD_SET_LOOTAPALOOZA_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.BOOMSDAY,
      "GLOBAL_CARD_SET_BOOMSDAY_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.TROLL,
      "GLOBAL_CARD_SET_TROLL_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.DALARAN,
      "GLOBAL_CARD_SET_DALARAN_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.ULDUM,
      "GLOBAL_CARD_SET_ULDUM_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.DRAGONS,
      "GLOBAL_CARD_SET_DRG_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.BLACK_TEMPLE,
      "GLOBAL_CARD_SET_BT_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.DEMON_HUNTER_INITIATE,
      "GLOBAL_CARD_SET_DHI_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.SCHOLOMANCE,
      "GLOBAL_CARD_SET_SCH_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.DARKMOON_FAIRE,
      "GLOBAL_CARD_SET_DMF_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.THE_BARRENS,
      "GLOBAL_CARD_SET_BAR_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.LEGACY,
      "GLOBAL_CARD_SET_LEGACY_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.CORE,
      "GLOBAL_CARD_SET_CORE_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.VANILLA,
      "GLOBAL_CARD_SET_VANILLA_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.STORMWIND,
      "GLOBAL_CARD_SET_SW_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.ALTERAC_VALLEY,
      "GLOBAL_CARD_SET_AV_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.THE_SUNKEN_CITY,
      "GLOBAL_CARD_SET_TSC_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.REVENDRETH,
      "GLOBAL_CARD_SET_REV_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.RETURN_OF_THE_LICH_KING,
      "GLOBAL_CARD_SET_RLK_SEARCHABLE_SHORTHAND_NAMES"
    },
    {
      TAG_CARD_SET.PATH_OF_ARTHAS,
      "GLOBAL_CARD_SET_PA_SEARCHABLE_SHORTHAND_NAMES"
    }
  };
  public static Map<TAG_CARD_SET, string> s_miniSetNames = new Map<TAG_CARD_SET, string>()
  {
    {
      TAG_CARD_SET.DARKMOON_FAIRE,
      "GLOBAL_MINI_SET_DMF"
    },
    {
      TAG_CARD_SET.THE_BARRENS,
      "GLOBAL_MINI_SET_BAR"
    },
    {
      TAG_CARD_SET.ALTERAC_VALLEY,
      "GLOBAL_MINI_SET_ONY"
    }
  };
  public static Map<TAG_CARDTYPE, string> s_cardTypeNames = new Map<TAG_CARDTYPE, string>()
  {
    {
      TAG_CARDTYPE.HERO,
      "GLOBAL_CARDTYPE_HERO"
    },
    {
      TAG_CARDTYPE.MINION,
      "GLOBAL_CARDTYPE_MINION"
    },
    {
      TAG_CARDTYPE.SPELL,
      "GLOBAL_CARDTYPE_SPELL"
    },
    {
      TAG_CARDTYPE.ENCHANTMENT,
      "GLOBAL_CARDTYPE_ENCHANTMENT"
    },
    {
      TAG_CARDTYPE.WEAPON,
      "GLOBAL_CARDTYPE_WEAPON"
    },
    {
      TAG_CARDTYPE.ITEM,
      "GLOBAL_CARDTYPE_ITEM"
    },
    {
      TAG_CARDTYPE.TOKEN,
      "GLOBAL_CARDTYPE_TOKEN"
    },
    {
      TAG_CARDTYPE.HERO_POWER,
      "GLOBAL_CARDTYPE_HEROPOWER"
    },
    {
      TAG_CARDTYPE.LOCATION,
      "GLOBAL_CARDTYPE_LOCATION"
    },
    {
      TAG_CARDTYPE.BATTLEGROUND_HERO_BUDDY,
      "GLOBAL_CARDTYPE_BACONHEROBUDDY"
    },
    {
      TAG_CARDTYPE.BATTLEGROUND_QUEST_REWARD,
      "GLOBAL_CARDTYPE_BACONQUESTREWARD"
    }
  };
  public static Map<TAG_MULTI_CLASS_GROUP, string> s_multiClassGroupNames = new Map<TAG_MULTI_CLASS_GROUP, string>()
  {
    {
      TAG_MULTI_CLASS_GROUP.GRIMY_GOONS,
      "GLOBAL_KEYWORD_GRIMY_GOONS"
    },
    {
      TAG_MULTI_CLASS_GROUP.JADE_LOTUS,
      "GLOBAL_KEYWORD_JADE_LOTUS"
    },
    {
      TAG_MULTI_CLASS_GROUP.KABAL,
      "GLOBAL_KEYWORD_KABAL"
    }
  };
  public static Map<TAG_SPELL_SCHOOL, string> s_spellSchoolNames = new Map<TAG_SPELL_SCHOOL, string>()
  {
    {
      TAG_SPELL_SCHOOL.ARCANE,
      "GLOBAL_SPELL_SCHOOL_ARCANE"
    },
    {
      TAG_SPELL_SCHOOL.FIRE,
      "GLOBAL_SPELL_SCHOOL_FIRE"
    },
    {
      TAG_SPELL_SCHOOL.FROST,
      "GLOBAL_SPELL_SCHOOL_FROST"
    },
    {
      TAG_SPELL_SCHOOL.NATURE,
      "GLOBAL_SPELL_SCHOOL_NATURE"
    },
    {
      TAG_SPELL_SCHOOL.HOLY,
      "GLOBAL_SPELL_SCHOOL_HOLY"
    },
    {
      TAG_SPELL_SCHOOL.SHADOW,
      "GLOBAL_SPELL_SCHOOL_SHADOW"
    },
    {
      TAG_SPELL_SCHOOL.FEL,
      "GLOBAL_SPELL_SCHOOL_FEL"
    },
    {
      TAG_SPELL_SCHOOL.PHYSICAL_COMBAT,
      "GLOBAL_SPELL_SCHOOL_PHYSICAL_COMBAT"
    }
  };
  public static Map<PegasusShared.FormatType, string> s_formatNames = new Map<PegasusShared.FormatType, string>()
  {
    {
      PegasusShared.FormatType.FT_STANDARD,
      "GLOBAL_STANDARD"
    },
    {
      PegasusShared.FormatType.FT_WILD,
      "GLOBAL_WILD"
    },
    {
      PegasusShared.FormatType.FT_CLASSIC,
      "GLOBAL_CLASSIC"
    }
  };
  public static Map<TAG_ROLE, string> s_roleNames = new Map<TAG_ROLE, string>()
  {
    {
      TAG_ROLE.FIGHTER,
      "GLOBAL_ROLE_FIGHTER"
    },
    {
      TAG_ROLE.TANK,
      "GLOBAL_ROLE_TANK"
    },
    {
      TAG_ROLE.CASTER,
      "GLOBAL_ROLE_CASTER"
    }
  };
  public static Map<RuneType, string> m_deathKnightRuneTypeNames = new Map<RuneType, string>()
  {
    {
      RuneType.RT_BLOOD,
      "GLOBAL_DEATHKNIGHT_RUNE_BLOOD"
    },
    {
      RuneType.RT_FROST,
      "GLOBAL_DEATHKNIGHT_RUNE_FROST"
    },
    {
      RuneType.RT_UNHOLY,
      "GLOBAL_DEATHKNIGHT_RUNE_UNHOLY"
    }
  };

  public static void LoadAll()
  {
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    foreach (Global.GameStringCategory cat in Enum.GetValues(typeof (Global.GameStringCategory)))
    {
      if (cat != Global.GameStringCategory.INVALID)
        GameStrings.LoadCategory(cat, false);
    }
    Log.Performance.Print(string.Format("Loading All GameStrings took {0}s)", (object) (float) ((double) Time.realtimeSinceStartup - (double) realtimeSinceStartup)));
  }

  public static IEnumerator<IAsyncJobResult> Job_LoadAll()
  {
    JobResultCollection resultCollection = new JobResultCollection(Array.Empty<IAsyncJobResult>());
    foreach (Global.GameStringCategory category in Enum.GetValues(typeof (Global.GameStringCategory)))
    {
      if (category != Global.GameStringCategory.INVALID)
        resultCollection.Add(GameStrings.CreateLoadCategoryJob(category, false));
    }
    yield return (IAsyncJobResult) resultCollection;
  }

  private static IAsyncJobResult CreateLoadCategoryJob(
    Global.GameStringCategory category,
    bool native)
  {
    return (IAsyncJobResult) new JobDefinition(string.Format("GameStrings.LoadCategory[{0}]", (object) category), GameStrings.Job_LoadCategory(category, native), Array.Empty<IJobDependency>());
  }

  private static IEnumerator<IAsyncJobResult> Job_LoadCategory(
    Global.GameStringCategory category,
    bool native)
  {
    if (GameStrings.s_tables.ContainsKey(category))
      GameStrings.UnloadCategory(category);
    GameStrings.LoadCategory(category, native);
    yield break;
  }

  private static void ReloadAllInternal(bool native)
  {
    float realtimeSinceStartup1 = Time.realtimeSinceStartup;
    foreach (Global.GameStringCategory gameStringCategory in Enum.GetValues(typeof (Global.GameStringCategory)))
    {
      if (gameStringCategory != Global.GameStringCategory.INVALID && (!native || GameStrings.s_nativeGameStringCatetories.Contains(gameStringCategory)))
      {
        if (GameStrings.s_tables.ContainsKey(gameStringCategory))
          GameStrings.UnloadCategory(gameStringCategory);
        GameStrings.LoadCategory(gameStringCategory, native);
      }
    }
    float realtimeSinceStartup2 = Time.realtimeSinceStartup;
    Log.Performance.Print(string.Format("Reloading {0} GameStrings took {1}s)", native ? (object) "Native" : (object) "All", (object) (float) ((double) realtimeSinceStartup2 - (double) realtimeSinceStartup1)));
  }

  public static void ReloadAll() => GameStrings.ReloadAllInternal(false);

  public static void LoadNative() => GameStrings.ReloadAllInternal(true);

  public static string GetAssetPath(Locale locale, string fileName, bool native = false) => native ? PlatformFilePaths.GetAssetPath(string.Format("{0}/{1}/{2}", (object) "NativeStrings", (object) locale, (object) fileName), false) : PlatformFilePaths.GetAssetPath(string.Format("{0}/{1}/{2}", (object) "Strings", (object) locale, (object) fileName));

  public static bool HasKey(string key) => GameStrings.Find(key) != null;

  public static bool TryGet(string key, out string localized)
  {
    localized = (string) null;
    string str = GameStrings.Find(key);
    if (str == null)
      return false;
    localized = GameStrings.ParseLanguageRules(str);
    return true;
  }

  public static string Get(string key)
  {
    string localized;
    if (!GameStrings.TryGet(key, out localized))
      localized = key;
    return localized;
  }

  public static string Format(string key, params object[] args)
  {
    string text = GameStrings.Find(key);
    return text == null ? key : GameStrings.FormatLocalizedString(text, args);
  }

  public static string FormatLocalizedString(string text, params object[] args)
  {
    text = string.Format((IFormatProvider) Localization.GetCultureInfo(), text, args);
    text = GameStrings.ParseLanguageRules(text);
    return text;
  }

  public static string FormatLocalizedStringWithPlurals(
    string text,
    GameStrings.PluralNumber[] pluralNumbers,
    params object[] args)
  {
    text = string.Format((IFormatProvider) Localization.GetCultureInfo(), text, args);
    text = GameStrings.ParseLanguageRules(text, pluralNumbers);
    return text;
  }

  public static string FormatPlurals(
    string key,
    GameStrings.PluralNumber[] pluralNumbers,
    params object[] args)
  {
    string format = GameStrings.Find(key);
    return format == null ? key : GameStrings.ParseLanguageRules(string.Format((IFormatProvider) Localization.GetCultureInfo(), format, args), pluralNumbers);
  }

  public static string FormatStringWithPlurals(
    List<LocalizedString> protoLocalized,
    string stringKey,
    params object[] optionalFormatArgs)
  {
    Locale locale = Localization.GetActualLocale();
    LocalizedString localizedString = protoLocalized.FirstOrDefault<LocalizedString>((Func<LocalizedString, bool>) (s => s.Key == stringKey));
    if (localizedString == null)
    {
      Debug.LogWarning((object) string.Format("GameStrings.FormatStringWithPlurals() - localizedStr was null for string key {0}", (object) stringKey));
      return (string) null;
    }
    LocalizedStringValue localizedStringValue = localizedString.Values.FirstOrDefault<LocalizedStringValue>((Func<LocalizedStringValue, bool>) (v => (Locale) v.Locale == locale));
    if (localizedStringValue.Value != null)
      return GameStrings.ParseLanguageRules(string.Format(localizedStringValue.Value, optionalFormatArgs));
    Debug.LogWarning((object) string.Format("GameStrings.FormatStringWithPlurals() - localizedVal was null"));
    return (string) null;
  }

  public static GameStrings.PluralNumber[] MakePlurals(params int[] quantities)
  {
    List<GameStrings.PluralNumber> pluralNumberList = new List<GameStrings.PluralNumber>();
    for (int index = 0; index < quantities.Length; ++index)
    {
      GameStrings.PluralNumber pluralNumber = new GameStrings.PluralNumber()
      {
        m_index = index,
        m_number = quantities[index]
      };
      pluralNumberList.Add(pluralNumber);
    }
    return pluralNumberList.ToArray();
  }

  public static string ParseLanguageRules(string str)
  {
    str = GameStrings.ParseLanguageRule1(str);
    str = GameStrings.ParseLanguageRule4(str);
    return str;
  }

  public static string ParseLanguageRules(string str, GameStrings.PluralNumber[] pluralNumbers)
  {
    str = GameStrings.ParseLanguageRule1(str);
    str = GameStrings.ParseLanguageRule4(str, pluralNumbers);
    return str;
  }

  public static bool HasClassName(TAG_CLASS tag) => GameStrings.s_classNames.ContainsKey(tag);

  public static string GetClassName(TAG_CLASS tag)
  {
    string key = (string) null;
    return !GameStrings.s_classNames.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static string GetClassesName(IList<TAG_CLASS> tags)
  {
    string key = (string) null;
    for (int index = 0; index < tags.Count; ++index)
    {
      TAG_CLASS tag = tags[index];
      key += GameStrings.s_classNames.TryGetValue(tag, out key) ? GameStrings.Get(key) : "UNKNOWN";
      if (index != tags.Count - 1)
        key += "/";
    }
    return key;
  }

  public static string GetClassNameKey(TAG_CLASS tag)
  {
    string str = (string) null;
    return !GameStrings.s_classNames.TryGetValue(tag, out str) ? (string) null : str;
  }

  public static string GetRoleName(TAG_ROLE tag)
  {
    string key = (string) null;
    return !GameStrings.s_roleNames.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static string GetRoleNameKey(TAG_ROLE tag)
  {
    string str = (string) null;
    return !GameStrings.s_roleNames.TryGetValue(tag, out str) ? (string) null : str;
  }

  public static string GetRuneTypeName(RuneType runeType)
  {
    string key = (string) null;
    return !GameStrings.m_deathKnightRuneTypeNames.TryGetValue(runeType, out key) ? (string) null : GameStrings.Get(key);
  }

  private static KeywordTextDbfRecord GetKeywordTextRecord(GAME_TAG tag) => GameDbf.KeywordText.GetRecord((Predicate<KeywordTextDbfRecord>) (r => (GAME_TAG) r.Tag == tag));

  public static bool HasKeywordName(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord != null && !string.IsNullOrEmpty(keywordTextRecord.Name);
  }

  public static string GetKeywordName(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord == null || keywordTextRecord.Name == null ? "UNKNOWN" : GameStrings.Get(GameStrings.GetModeSpecificKey(keywordTextRecord.Name));
  }

  public static bool HasKeywordText(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord != null && !string.IsNullOrEmpty(keywordTextRecord.Text);
  }

  public static string GetKeywordText(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord == null || keywordTextRecord.Text == null ? "UNKNOWN" : GameStrings.Get(keywordTextRecord.Text);
  }

  public static string GetKeywordTextKey(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord == null || keywordTextRecord.Text == null ? "UNKNOWN" : GameStrings.GetModeSpecificKey(keywordTextRecord.Text);
  }

  public static bool HasRefKeywordText(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord != null && !string.IsNullOrEmpty(keywordTextRecord.RefText);
  }

  public static string GetRefKeywordText(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord == null || keywordTextRecord.RefText == null ? "UNKNOWN" : GameStrings.Get(keywordTextRecord.RefText);
  }

  public static string GetRefKeywordTextKey(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord == null || keywordTextRecord.RefText == null ? "UNKNOWN" : GameStrings.GetModeSpecificKey(keywordTextRecord.RefText);
  }

  public static bool HasCollectionKeywordText(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord != null && !string.IsNullOrEmpty(keywordTextRecord.CollectionText);
  }

  public static string GetCollectionKeywordText(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord == null || keywordTextRecord.CollectionText == null ? "UNKNOWN" : GameStrings.Get(keywordTextRecord.CollectionText);
  }

  public static string GetCollectionKeywordTextKey(GAME_TAG tag)
  {
    KeywordTextDbfRecord keywordTextRecord = GameStrings.GetKeywordTextRecord(tag);
    return keywordTextRecord == null || keywordTextRecord.CollectionText == null ? "UNKNOWN" : GameStrings.GetModeSpecificKey(keywordTextRecord.CollectionText);
  }

  private static string GetModeSpecificKey(string key)
  {
    int num;
    if (!(GameState.Get()?.GetGameEntity() is LettuceMissionEntity))
    {
      SceneMgr sceneMgr1 = SceneMgr.Get();
      if ((sceneMgr1 != null ? (sceneMgr1.GetMode() == SceneMgr.Mode.LETTUCE_COLLECTION ? 1 : 0) : 0) == 0)
      {
        SceneMgr sceneMgr2 = SceneMgr.Get();
        num = sceneMgr2 != null ? (sceneMgr2.GetMode() == SceneMgr.Mode.LETTUCE_MAP ? 1 : 0) : 0;
        goto label_4;
      }
    }
    num = 1;
label_4:
    if (num != 0)
    {
      string key1 = key + "_MERC";
      if (GameStrings.HasKey(key1))
        return key1;
    }
    return key;
  }

  public static bool HasRarityText(TAG_RARITY tag) => GameStrings.s_rarityNames.ContainsKey(tag);

  public static string GetRarityText(TAG_RARITY tag)
  {
    string key = (string) null;
    return !GameStrings.s_rarityNames.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static string GetRarityTextKey(TAG_RARITY tag)
  {
    string str = (string) null;
    return !GameStrings.s_rarityNames.TryGetValue(tag, out str) ? (string) null : str;
  }

  public static bool HasPremiumText(TAG_PREMIUM tag) => GameStrings.s_premiumNames.ContainsKey(tag);

  public static string GetPremiumText(TAG_PREMIUM tag)
  {
    string key = (string) null;
    return !GameStrings.s_premiumNames.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static bool HasRaceName(TAG_RACE tag) => GameStrings.s_raceNames.ContainsKey(tag);

  public static string GetRaceName(TAG_RACE tag)
  {
    string key = (string) null;
    return !GameStrings.s_raceNames.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static string GetRaceNameKey(TAG_RACE tag)
  {
    string str = (string) null;
    return !GameStrings.s_raceNames.TryGetValue(tag, out str) ? (string) null : str;
  }

  public static bool HasRaceNameBattlegrounds(TAG_RACE tag) => GameStrings.s_raceNamesBattlegrounds.ContainsKey(tag);

  public static string GetRaceNameBattlegrounds(TAG_RACE tag)
  {
    string key = (string) null;
    return !GameStrings.s_raceNamesBattlegrounds.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static string GetRaceNameKeyBattlegrounds(TAG_RACE tag)
  {
    string str = (string) null;
    return !GameStrings.s_raceNamesBattlegrounds.TryGetValue(tag, out str) ? (string) null : str;
  }

  public static bool HasCardTypeName(TAG_CARDTYPE tag) => GameStrings.s_cardTypeNames.ContainsKey(tag);

  public static string GetCardTypeName(TAG_CARDTYPE tag)
  {
    string key = (string) null;
    return !GameStrings.s_cardTypeNames.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static string GetCardTypeNameKey(TAG_CARDTYPE tag)
  {
    string str = (string) null;
    return !GameStrings.s_cardTypeNames.TryGetValue(tag, out str) ? (string) null : str;
  }

  public static bool HasCardSetName(TAG_CARD_SET tag) => GameStrings.s_cardSetNames.ContainsKey(tag);

  public static string GetCardSetName(TAG_CARD_SET tag)
  {
    string key = (string) null;
    return !GameStrings.s_cardSetNames.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static string GetCardSetNameKey(TAG_CARD_SET tag)
  {
    string str = (string) null;
    return !GameStrings.s_cardSetNames.TryGetValue(tag, out str) ? (string) null : str;
  }

  public static bool HasCardSetNameShortened(TAG_CARD_SET tag) => GameStrings.s_cardSetNamesShortened.ContainsKey(tag);

  public static string GetCardSetNameShortened(TAG_CARD_SET tag)
  {
    string key = (string) null;
    if (GameStrings.s_cardSetNamesShortened.TryGetValue(tag, out key))
      return GameStrings.Get(key);
    Log.All.PrintWarning("GetCardSetNameShortened - Could not find a Card Set name for tag {0}; returning {1}", (object) tag, (object) "UNKNOWN");
    return "UNKNOWN";
  }

  public static string GetCardSetNameKeyShortened(TAG_CARD_SET tag)
  {
    string str = (string) null;
    return !GameStrings.s_cardSetNamesShortened.TryGetValue(tag, out str) ? (string) null : str;
  }

  public static bool HasCardSetNameInitials(TAG_CARD_SET tag) => GameStrings.s_cardSetNamesInitials.ContainsKey(tag);

  public static string GetCardSetNameInitials(TAG_CARD_SET tag)
  {
    string key = (string) null;
    return !GameStrings.s_cardSetNamesInitials.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static bool HasMiniSetName(TAG_CARD_SET tag) => GameStrings.s_miniSetNames.ContainsKey(tag);

  public static string GetMiniSetName(TAG_CARD_SET tag)
  {
    string key;
    return !GameStrings.s_miniSetNames.TryGetValue(tag, out key) ? (string) null : GameStrings.Get(key);
  }

  public static bool HasMultiClassGroupName(TAG_MULTI_CLASS_GROUP tag) => GameStrings.s_multiClassGroupNames.ContainsKey(tag);

  public static string GetMultiClassGroupName(TAG_MULTI_CLASS_GROUP tag)
  {
    string key = (string) null;
    return !GameStrings.s_multiClassGroupNames.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static bool HasSpellSchoolName(TAG_SPELL_SCHOOL tag) => GameStrings.s_spellSchoolNames.ContainsKey(tag);

  public static string GetSpellSchoolName(TAG_SPELL_SCHOOL tag)
  {
    string key = (string) null;
    return !GameStrings.s_spellSchoolNames.TryGetValue(tag, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static bool HasFormatName(PegasusShared.FormatType format) => GameStrings.s_formatNames.ContainsKey(format);

  public static string GetFormatName(PegasusShared.FormatType format)
  {
    string key = (string) null;
    return !GameStrings.s_formatNames.TryGetValue(format, out key) ? "UNKNOWN" : GameStrings.Get(key);
  }

  public static string GetRandomTip(TipCategory tipCategory)
  {
    List<string> listOfTips = GameStrings.GetListOfTips(tipCategory);
    if (listOfTips.Count == 0)
    {
      Debug.LogError((object) string.Format("GameStrings.GetRandomTip() - no tips in category {0}", (object) tipCategory));
      return "UNKNOWN";
    }
    int index = UnityEngine.Random.Range(0, listOfTips.Count);
    return listOfTips[index];
  }

  public static string GetTip(TipCategory tipCategory, int? tipIndex)
  {
    List<string> listOfTips = GameStrings.GetListOfTips(tipCategory);
    return tipIndex.HasValue && tipIndex.Value < listOfTips.Count ? listOfTips[tipIndex.Value] : GameStrings.GetRandomTip(tipCategory);
  }

  private static List<string> GetListOfTips(TipCategory tipCategory)
  {
    int num = 0;
    List<string> listOfTips = new List<string>();
    while (true)
    {
      string key1 = string.Format("GLUE_TIP_{0}_{1}", (object) tipCategory, (object) num);
      string str1 = GameStrings.Get(key1);
      if (!str1.Equals(key1))
      {
        if (tipCategory == TipCategory.DEFAULT && num == 25)
          str1 = GameStrings.Format(key1, (object) SetRotationManager.Get().GetActiveSetRotationYearLocalizedString());
        if (UniversalInputManager.Get().IsTouchMode())
        {
          string key2 = key1 + "_TOUCH";
          string str2 = GameStrings.Get(key2);
          if (!str2.Equals(key2))
            str1 = str2;
          if ((bool) UniversalInputManager.UsePhoneUI)
          {
            string key3 = key1 + "_PHONE";
            string str3 = GameStrings.Get(key3);
            if (!str3.Equals(key3))
              str1 = str3;
          }
        }
        if (!string.IsNullOrEmpty(str1))
          listOfTips.Add(str1);
        ++num;
      }
      else
        break;
    }
    return listOfTips;
  }

  public static string GetMonthFromDigits(int monthDigits)
  {
    if (Localization.GetLocale() != Locale.thTH)
      return Localization.GetCultureInfo().DateTimeFormat.GetMonthName(monthDigits);
    switch (monthDigits)
    {
      case 1:
        return "มกราคม";
      case 2:
        return "กุมภาพันธ์";
      case 3:
        return "มีนาคม";
      case 4:
        return "เมษายน";
      case 5:
        return "พฤษภาคม";
      case 6:
        return "มิถุนายน";
      case 7:
        return "กรกฎาคม";
      case 8:
        return "สิงหาคม";
      case 9:
        return "กันยายน";
      case 10:
        return "ตุลาคม";
      case 11:
        return "พฤศจิกายน";
      case 12:
        return "ธันวาคม";
      default:
        return string.Empty;
    }
  }

  public static string GetOrdinalNumber(int number)
  {
    string key = "GLUE_ORDINAL_" + (object) number;
    string ordinalNumber = GameStrings.Get(key);
    if (!(ordinalNumber == key))
      return ordinalNumber;
    Debug.LogError((object) string.Format("GameStrings.GetOrdinalNumber() - Unable to find ordinal string for number={0}", (object) number));
    return number.ToString();
  }

  private static bool LoadCategory(Global.GameStringCategory cat, bool native)
  {
    if (GameStrings.s_tables.ContainsKey(cat))
    {
      Debug.LogWarning((object) string.Format("GameStrings.LoadCategory() - {0} is already loaded", (object) cat));
      return false;
    }
    GameStringTable gameStringTable = new GameStringTable();
    if (!gameStringTable.Load(cat, native))
    {
      Debug.LogError((object) string.Format("GameStrings.LoadCategory() - {0} failed to load", (object) cat));
      return false;
    }
    GameStrings.s_tables.Add(cat, gameStringTable);
    return true;
  }

  private static bool UnloadCategory(Global.GameStringCategory cat)
  {
    if (GameStrings.s_tables.Remove(cat))
      return true;
    Debug.LogWarning((object) string.Format("GameStrings.UnloadCategory() - {0} was never loaded", (object) cat));
    return false;
  }

  private static string Find(string key)
  {
    if (key == null)
      return (string) null;
    foreach (KeyValuePair<Global.GameStringCategory, GameStringTable> table in GameStrings.s_tables)
    {
      string str = table.Value.Get(key);
      if (str != null)
        return str;
    }
    if (key.StartsWith("Assets/"))
      Debug.LogErrorFormat("Asset path being used as GameString key={0}", (object) key);
    return (string) null;
  }

  private static string[] ParseLanguageRuleArgs(
    string str,
    int ruleIndex,
    out int argStartIndex,
    out int argEndIndex)
  {
    argStartIndex = -1;
    argEndIndex = -1;
    argStartIndex = str.IndexOf('(', ruleIndex + 2);
    if (argStartIndex < 0)
    {
      Debug.LogWarning((object) string.Format("GameStrings.ParseLanguageRuleArgs() - failed to parse '(' for rule at index {0} in string {1}", (object) ruleIndex, (object) str));
      return (string[]) null;
    }
    argEndIndex = str.IndexOf(')', argStartIndex + 1);
    if (argEndIndex < 0)
    {
      Debug.LogWarning((object) string.Format("GameStrings.ParseLanguageRuleArgs() - failed to parse ')' for rule at index {0} in string {1}", (object) ruleIndex, (object) str));
      return (string[]) null;
    }
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(str, argStartIndex + 1, argEndIndex - argStartIndex - 1);
    string input = stringBuilder.ToString();
    MatchCollection matchCollection = Regex.Matches(input, "(?<!\\/)(?:[0-9]+,)*[0-9]+(?!\\/)");
    if (matchCollection.Count == 0)
      matchCollection = Regex.Matches(input, "(?<!\\/)(?:[0-9]+,)*[0-9]+");
    if (matchCollection.Count > 0)
    {
      stringBuilder.Remove(0, stringBuilder.Length);
      int startIndex = 0;
      foreach (Match match in matchCollection)
      {
        stringBuilder.Append(input, startIndex, match.Index - startIndex);
        stringBuilder.Append('0', match.Length);
        startIndex = match.Index + match.Length;
      }
      stringBuilder.Append(input, startIndex, input.Length - startIndex);
      input = stringBuilder.ToString();
    }
    string[] languageRuleArgs = input.Split(GameStrings.LANGUAGE_RULE_ARG_DELIMITERS);
    int num1 = 0;
    for (int index = 0; index < languageRuleArgs.Length; ++index)
    {
      string str1 = languageRuleArgs[index];
      if (matchCollection.Count > 0)
      {
        stringBuilder.Remove(0, stringBuilder.Length);
        int startIndex = 0;
        foreach (Match match in matchCollection)
        {
          if (match.Index >= num1 && match.Index < num1 + str1.Length)
          {
            int num2 = match.Index - num1;
            stringBuilder.Append(str1, startIndex, num2 - startIndex);
            stringBuilder.Append(match.Value);
            startIndex = num2 + match.Length;
          }
        }
        stringBuilder.Append(str1, startIndex, str1.Length - startIndex);
        str1 = stringBuilder.ToString();
        num1 += str1.Length + 1;
      }
      string str2 = str1.Trim();
      languageRuleArgs[index] = str2;
    }
    return languageRuleArgs;
  }

  private static bool FindPrecedingChar(string preStr, out int precedingChar)
  {
    int index = preStr.Length - 1;
    precedingChar = (int) preStr[index];
    for (; index >= 0; --index)
    {
      precedingChar = (int) preStr[index];
      if (precedingChar < 44032 || precedingChar > 55203)
      {
        if (precedingChar >= 65 && precedingChar <= 90 || precedingChar >= 97 && precedingChar <= 122)
        {
          precedingChar = precedingChar == 76 || precedingChar == 108 || precedingChar == 82 || precedingChar == 114 ? 51068 : (precedingChar == 77 || precedingChar == 109 || precedingChar == 110 || precedingChar == 110 ? 50689 : 51060);
          break;
        }
        if (!")}]:;?/*&^!~`/\\|_'\"".Contains((char) precedingChar))
        {
          if (precedingChar == 62 && preStr[index - 3] == '<')
            index -= 3;
          if (precedingChar >= 48 && precedingChar <= 57)
          {
            precedingChar = precedingChar == 48 || precedingChar == 51 || precedingChar == 54 ? 50689 : (precedingChar == 49 || precedingChar == 55 || precedingChar == 56 ? 51068 : 51060);
            break;
          }
        }
      }
      else
        break;
    }
    if (index < 0)
      precedingChar = 51060;
    return true;
  }

  private static string ParseLanguageRule1(string str)
  {
    int num1 = str.IndexOf("|1");
    if (num1 < 0)
      return str;
    StringBuilder stringBuilder = new StringBuilder();
    for (; num1 >= 0; num1 = str.IndexOf("|1"))
    {
      string preStr = str.Substring(0, num1);
      if (preStr.Length == 0)
      {
        Debug.LogWarningFormat("GameStrings.ParseLanguageRule1() - invalid preStr, str:{0}, ruleIndex:{1}", (object) str, (object) num1);
        break;
      }
      int startIndex = str.IndexOf('(', num1);
      if (startIndex < 0)
      {
        Debug.LogWarningFormat("GameStrings.ParseLanguageRule1() - invalid openIndex, str:{0}, ruleIndex:{1}", (object) str, (object) num1);
        break;
      }
      int num2 = str.IndexOf(')', startIndex);
      if (num2 < 0)
      {
        Debug.LogWarningFormat("GameStrings.ParseLanguageRule1() - invalid closeIndex, str:{0}, ruleIndex:{1}, openIndex:{2}", (object) str, (object) num1);
        break;
      }
      string str1 = str.Substring(startIndex + 1, num2 - startIndex - 1);
      string[] strArray = str1.Split(',');
      if (strArray.Length != 2)
      {
        Debug.LogWarningFormat("GameStrings.ParseLanguageRule1() - invalid args, str:{0}, argStr:{1}", (object) str, (object) str1);
        break;
      }
      int precedingChar;
      if (!GameStrings.FindPrecedingChar(preStr, out precedingChar))
        Debug.LogWarningFormat("GameStrings.ParseLanguageRule1() - failed to find the preceding character, str:{0}, preStr{1}", (object) str, (object) preStr);
      if (precedingChar < 44032 || precedingChar > 55203)
      {
        Debug.LogWarningFormat("GameStrings.ParseLanguageRule1() - invalid precedingChar, str:{0}, precedingChar:{1}", (object) str, (object) precedingChar);
        break;
      }
      int num3 = (precedingChar - 44032) % 28;
      int index = num3 == 0 || strArray[1][0] == '로' && num3 == 8 ? 1 : 0;
      stringBuilder.Append(preStr);
      stringBuilder.Append(strArray[index]);
      str = str.Substring(num2 + 1);
    }
    stringBuilder.Append(str);
    return stringBuilder.ToString();
  }

  private static string ParseLanguageRule4(string str, GameStrings.PluralNumber[] pluralNumbers = null)
  {
    StringBuilder stringBuilder = (StringBuilder) null;
    int? nullable = new int?();
    int startIndex1 = 0;
    int num = 0;
    for (int ruleIndex = str.IndexOf("|4"); ruleIndex >= 0; ruleIndex = str.IndexOf("|4", ruleIndex + 2))
    {
      ++num;
      int argEndIndex;
      string[] languageRuleArgs = GameStrings.ParseLanguageRuleArgs(str, ruleIndex, out int _, out argEndIndex);
      if (languageRuleArgs != null)
      {
        int startIndex2 = startIndex1;
        int length = ruleIndex - startIndex1;
        string betweenRulesStr = str.Substring(startIndex2, length);
        GameStrings.PluralNumber pluralNumber = (GameStrings.PluralNumber) null;
        if (pluralNumbers != null)
        {
          int pluralArgIndex = num - 1;
          pluralNumber = Array.Find<GameStrings.PluralNumber>(pluralNumbers, (Predicate<GameStrings.PluralNumber>) (currPluralNumber => currPluralNumber.m_index == pluralArgIndex));
        }
        if (pluralNumber != null)
        {
          nullable = new int?(pluralNumber.m_number);
        }
        else
        {
          int number;
          if (GameStrings.ParseLanguageRule4Number(languageRuleArgs, betweenRulesStr, out number))
            nullable = new int?(number);
          else if (!nullable.HasValue)
          {
            Debug.LogWarning((object) string.Format("GameStrings.ParseLanguageRule4() - failed to parse a number in substring \"{0}\" (indexes {1}-{2}) for rule {3} in string \"{4}\"", (object) betweenRulesStr, (object) startIndex2, (object) length, (object) num, (object) str));
            continue;
          }
        }
        int pluralIndex = GameStrings.GetPluralIndex(nullable.Value);
        if (pluralIndex >= languageRuleArgs.Length)
        {
          Debug.LogWarning((object) string.Format("GameStrings.ParseLanguageRule4() - not enough arguments for rule {0} in string \"{1}\"", (object) num, (object) str));
        }
        else
        {
          string str1 = languageRuleArgs[pluralIndex];
          if (stringBuilder == null)
            stringBuilder = new StringBuilder();
          stringBuilder.Append(betweenRulesStr);
          stringBuilder.Append(str1);
          startIndex1 = argEndIndex + 1;
        }
        if (pluralNumber != null && pluralNumber.m_useForOnlyThisIndex)
          nullable = new int?();
      }
    }
    if (stringBuilder == null)
      return str;
    stringBuilder.Append(str, startIndex1, str.Length - startIndex1);
    return stringBuilder.ToString();
  }

  private static bool ParseLanguageRule4Number(
    string[] args,
    string betweenRulesStr,
    out int number)
  {
    if (GameStrings.ParseLanguageRule4Number_Foreward(args[0], out number) || GameStrings.ParseLanguageRule4Number_Backward(betweenRulesStr, out number))
      return true;
    number = 0;
    return false;
  }

  private static bool ParseLanguageRule4Number_Foreward(string str, out int number)
  {
    number = 0;
    Match match = Regex.Match(str, "(?<!\\/)(?:[0-9]+,)*[0-9]+(?!\\/)");
    if (!match.Success)
      match = Regex.Match(str, "(?<!\\/)(?:[0-9]+,)*[0-9]+");
    return match.Success && GeneralUtils.TryParseInt(match.Value, out number);
  }

  private static bool ParseLanguageRule4Number_Backward(string str, out int number)
  {
    number = 0;
    MatchCollection matchCollection = Regex.Matches(str, "(?<!\\/)(?:[0-9]+,)*[0-9]+(?!\\/)");
    if (matchCollection.Count == 0)
      matchCollection = Regex.Matches(str, "(?<!\\/)(?:[0-9]+,)*[0-9]+");
    return matchCollection.Count != 0 && GeneralUtils.TryParseInt(matchCollection[matchCollection.Count - 1].Value, out number);
  }

  private static int GetPluralIndex(int number)
  {
    switch (Localization.GetLocale())
    {
      case Locale.frFR:
      case Locale.koKR:
      case Locale.zhTW:
      case Locale.zhCN:
        return number <= 1 ? 0 : 1;
      case Locale.ruRU:
        switch (number % 100)
        {
          case 11:
          case 12:
          case 13:
          case 14:
            return 2;
          default:
            switch (number % 10)
            {
              case 1:
                return 0;
              case 2:
              case 3:
              case 4:
                return 1;
              default:
                return 2;
            }
        }
      case Locale.plPL:
        if (number == 1)
          return 0;
        if (number == 0)
          return 2;
        switch (number % 100)
        {
          case 11:
          case 12:
          case 13:
          case 14:
            return 2;
          default:
            switch (number % 10)
            {
              case 2:
              case 3:
              case 4:
                return 1;
              default:
                return 2;
            }
        }
      default:
        return number == 1 ? 0 : 1;
    }
  }

  public class PluralNumber
  {
    public int m_index;
    public int m_number;
    public bool m_useForOnlyThisIndex;
  }
}
