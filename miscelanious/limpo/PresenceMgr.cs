using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PresenceMgr
{
  private static readonly Map<Enum, Enum> s_richPresenceMap = new Map<Enum, Enum>()
  {
    {
      (Enum) Global.PresenceStatus.LOGIN,
      (Enum) null
    },
    {
      (Enum) Global.PresenceStatus.WELCOMEQUESTS,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.STORE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.QUESTLOG,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.PACKOPENING,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.COLLECTION,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.DECKEDITOR,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.CRAFTING,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.PLAY_DECKPICKER,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.PLAY_QUEUE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.PRACTICE_DECKPICKER,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.ARENA_PURCHASE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.ARENA_FORGE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.ARENA_IDLE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.ARENA_QUEUE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.ARENA_REWARD,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.FRIENDLY_DECKPICKER,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.ADVENTURE_CHOOSING_MODE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.ADVENTURE_SCENARIO_SELECT,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.TAVERN_BRAWL_SCREEN,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.TAVERN_BRAWL_DECKEDITOR,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.TAVERN_BRAWL_QUEUE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.FIRESIDE_BRAWL_SCREEN,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.BATTLEGROUNDS_QUEUE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.DUELS_QUEUE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_QUEUE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_PLAY_SCREEN,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_COLLECTION,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_TEAM_EDITOR,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_TASKBOARD,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_BUILDING_MANAGER,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_PVE_ZONES,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_PVE_BOUNTIES,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_PVP,
      (Enum) Global.PresenceStatus.HUB
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_MAILBOX,
      (Enum) Global.PresenceStatus.HUB
    }
  };
  private static readonly Map<Enum, string> s_stringKeyMap = new Map<Enum, string>()
  {
    {
      (Enum) Global.PresenceStatus.LOGIN,
      "PRESENCE_STATUS_LOGIN"
    },
    {
      (Enum) Global.PresenceStatus.TUTORIAL_PREGAME,
      "PRESENCE_STATUS_TUTORIAL_PREGAME"
    },
    {
      (Enum) Global.PresenceStatus.TUTORIAL_GAME,
      "PRESENCE_STATUS_TUTORIAL_GAME"
    },
    {
      (Enum) Global.PresenceStatus.WELCOMEQUESTS,
      "PRESENCE_STATUS_WELCOMEQUESTS"
    },
    {
      (Enum) Global.PresenceStatus.HUB,
      "PRESENCE_STATUS_HUB"
    },
    {
      (Enum) Global.PresenceStatus.STORE,
      "PRESENCE_STATUS_STORE"
    },
    {
      (Enum) Global.PresenceStatus.QUESTLOG,
      "PRESENCE_STATUS_QUESTLOG"
    },
    {
      (Enum) Global.PresenceStatus.PACKOPENING,
      "PRESENCE_STATUS_PACKOPENING"
    },
    {
      (Enum) Global.PresenceStatus.COLLECTION,
      "PRESENCE_STATUS_COLLECTION"
    },
    {
      (Enum) Global.PresenceStatus.DECKEDITOR,
      "PRESENCE_STATUS_DECKEDITOR"
    },
    {
      (Enum) Global.PresenceStatus.CRAFTING,
      "PRESENCE_STATUS_CRAFTING"
    },
    {
      (Enum) Global.PresenceStatus.PLAY_DECKPICKER,
      "PRESENCE_STATUS_PLAY_DECKPICKER"
    },
    {
      (Enum) Global.PresenceStatus.PLAY_QUEUE,
      "PRESENCE_STATUS_PLAY_QUEUE"
    },
    {
      (Enum) Global.PresenceStatus.PLAY_GAME,
      "PRESENCE_STATUS_PLAY_GAME"
    },
    {
      (Enum) Global.PresenceStatus.WAIT_FOR_OPPONENT_RECONNECT,
      "PRESENCE_STATUS_WAIT_FOR_OPPONENT_RECONNECT"
    },
    {
      (Enum) Global.PresenceStatus.PRACTICE_DECKPICKER,
      "PRESENCE_STATUS_PRACTICE_DECKPICKER"
    },
    {
      (Enum) Global.PresenceStatus.PRACTICE_GAME,
      "PRESENCE_STATUS_PRACTICE_GAME"
    },
    {
      (Enum) Global.PresenceStatus.ARENA_PURCHASE,
      "PRESENCE_STATUS_ARENA_PURCHASE"
    },
    {
      (Enum) Global.PresenceStatus.ARENA_FORGE,
      "PRESENCE_STATUS_ARENA_FORGE"
    },
    {
      (Enum) Global.PresenceStatus.ARENA_IDLE,
      "PRESENCE_STATUS_ARENA_IDLE"
    },
    {
      (Enum) Global.PresenceStatus.ARENA_QUEUE,
      "PRESENCE_STATUS_ARENA_QUEUE"
    },
    {
      (Enum) Global.PresenceStatus.ARENA_GAME,
      "PRESENCE_STATUS_ARENA_GAME"
    },
    {
      (Enum) Global.PresenceStatus.ARENA_REWARD,
      "PRESENCE_STATUS_ARENA_REWARD"
    },
    {
      (Enum) Global.PresenceStatus.FRIENDLY_DECKPICKER,
      "PRESENCE_STATUS_FRIENDLY_DECKPICKER"
    },
    {
      (Enum) Global.PresenceStatus.FRIENDLY_GAME,
      "PRESENCE_STATUS_FRIENDLY_GAME"
    },
    {
      (Enum) Global.PresenceStatus.ADVENTURE_CHOOSING_MODE,
      "PRESENCE_STATUS_ADVENTURE_CHOOSING_MODE"
    },
    {
      (Enum) Global.PresenceStatus.ADVENTURE_SCENARIO_SELECT,
      "PRESENCE_STATUS_ADVENTURE_SCENARIO_SELECT"
    },
    {
      (Enum) Global.PresenceStatus.ADVENTURE_SCENARIO_PLAYING_GAME,
      "PRESENCE_STATUS_ADVENTURE_SCENARIO_PLAYING_GAME"
    },
    {
      (Enum) Global.PresenceStatus.TAVERN_BRAWL_SCREEN,
      "PRESENCE_STATUS_TAVERN_BRAWL_SCREEN"
    },
    {
      (Enum) Global.PresenceStatus.TAVERN_BRAWL_DECKEDITOR,
      "PRESENCE_STATUS_TAVERN_BRAWL_DECKEDITOR"
    },
    {
      (Enum) Global.PresenceStatus.TAVERN_BRAWL_QUEUE,
      "PRESENCE_STATUS_TAVERN_BRAWL_QUEUE"
    },
    {
      (Enum) Global.PresenceStatus.TAVERN_BRAWL_GAME,
      "PRESENCE_STATUS_TAVERN_BRAWL_GAME"
    },
    {
      (Enum) Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING,
      "PRESENCE_STATUS_TAVERN_BRAWL_FRIENDLY_WAITING"
    },
    {
      (Enum) Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_GAME,
      "PRESENCE_STATUS_TAVERN_BRAWL_FRIENDLY_GAME"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_TUTORIAL,
      "PRESENCE_STATUS_SPECTATING_GAME_TUTORIAL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_PRACTICE,
      "PRESENCE_STATUS_SPECTATING_GAME_PRACTICE"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY,
      "PRESENCE_STATUS_SPECTATING_GAME_PLAY"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ARENA,
      "PRESENCE_STATUS_SPECTATING_GAME_ARENA"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_FRIENDLY,
      "PRESENCE_STATUS_SPECTATING_GAME_FRIENDLY"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_TAVERN_BRAWL,
      "PRESENCE_STATUS_SPECTATING_GAME_TAVERN_BRAWL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_RETURNING_PLAYER_CHALLENGE,
      "PRESENCE_STATUS_SPECTATING_GAME_RETURNING_PLAYER_CHALLENGE"
    },
    {
      (Enum) Global.PresenceStatus.FIRESIDE_BRAWL_SCREEN,
      "PRESENCE_STATUS_FIRESIDE_BRAWL_SCREEN"
    },
    {
      (Enum) Global.PresenceStatus.PLAY_RANKED_STANDARD,
      "PRESENCE_STATUS_PLAY_RANKED_STANDARD"
    },
    {
      (Enum) Global.PresenceStatus.PLAY_RANKED_WILD,
      "PRESENCE_STATUS_PLAY_RANKED_WILD"
    },
    {
      (Enum) Global.PresenceStatus.PLAY_RANKED_CLASSIC,
      "PRESENCE_STATUS_PLAY_RANKED_CLASSIC"
    },
    {
      (Enum) Global.PresenceStatus.PLAY_CASUAL_STANDARD,
      "PRESENCE_STATUS_PLAY_CASUAL_STANDARD"
    },
    {
      (Enum) Global.PresenceStatus.PLAY_CASUAL_WILD,
      "PRESENCE_STATUS_PLAY_CASUAL_WILD"
    },
    {
      (Enum) Global.PresenceStatus.PLAY_CASUAL_CLASSIC,
      "PRESENCE_STATUS_PLAY_CASUAL_CLASSIC"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_RANKED_STANDARD,
      "PRESENCE_STATUS_SPECTATING_GAME_PLAY_RANKED_STANDARD"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_RANKED_WILD,
      "PRESENCE_STATUS_SPECTATING_GAME_PLAY_RANKED_WILD"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_RANKED_CLASSIC,
      "PRESENCE_STATUS_SPECTATING_GAME_PLAY_RANKED_CLASSIC"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_CASUAL_STANDARD,
      "PRESENCE_STATUS_SPECTATING_GAME_PLAY_CASUAL_STANDARD"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_CASUAL_WILD,
      "PRESENCE_STATUS_SPECTATING_GAME_PLAY_CASUAL_WILD"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_CASUAL_CLASSIC,
      "PRESENCE_STATUS_SPECTATING_GAME_PLAY_CASUAL_CLASSIC"
    },
    {
      (Enum) PresenceTutorial.HOGGER,
      "PRESENCE_TUTORIAL_HOGGER"
    },
    {
      (Enum) PresenceTutorial.MILLHOUSE,
      "PRESENCE_TUTORIAL_MILLHOUSE"
    },
    {
      (Enum) PresenceTutorial.MUKLA,
      "PRESENCE_TUTORIAL_MUKLA"
    },
    {
      (Enum) PresenceTutorial.HEMET,
      "PRESENCE_TUTORIAL_HEMET"
    },
    {
      (Enum) PresenceTutorial.ILLIDAN,
      "PRESENCE_TUTORIAL_ILLIDAN"
    },
    {
      (Enum) PresenceTutorial.CHO,
      "PRESENCE_TUTORIAL_CHO"
    },
    {
      (Enum) PresenceAdventureMode.RETURNING_PLAYER_CHALLENGE,
      "PRESENCE_ADVENTURE_MODE_RETURNING_PLAYER_CHALLENGE"
    },
    {
      (Enum) PresenceAdventureMode.NAXX_NORMAL,
      "PRESENCE_ADVENTURE_MODE_NAXX_NORMAL"
    },
    {
      (Enum) PresenceAdventureMode.NAXX_HEROIC,
      "PRESENCE_ADVENTURE_MODE_NAXX_HEROIC"
    },
    {
      (Enum) PresenceAdventureMode.NAXX_CLASS_CHALLENGE,
      "PRESENCE_ADVENTURE_MODE_NAXX_CLASS_CHALLENGE"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_NORMAL,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_NAXX_NORMAL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_HEROIC,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_NAXX_HEROIC"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_CLASS_CHALLENGE,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_NAXX_CLASS_CHALLENGE"
    },
    {
      (Enum) PresenceAdventureMode.BRM_NORMAL,
      "PRESENCE_ADVENTURE_MODE_BRM_NORMAL"
    },
    {
      (Enum) PresenceAdventureMode.BRM_HEROIC,
      "PRESENCE_ADVENTURE_MODE_BRM_HEROIC"
    },
    {
      (Enum) PresenceAdventureMode.BRM_CLASS_CHALLENGE,
      "PRESENCE_ADVENTURE_MODE_BRM_CLASS_CHALLENGE"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_NORMAL,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_BRM_NORMAL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_HEROIC,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_BRM_HEROIC"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_CLASS_CHALLENGE,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_BRM_CLASS_CHALLENGE"
    },
    {
      (Enum) PresenceAdventureMode.LOE_NORMAL,
      "PRESENCE_ADVENTURE_MODE_LOE_NORMAL"
    },
    {
      (Enum) PresenceAdventureMode.LOE_HEROIC,
      "PRESENCE_ADVENTURE_MODE_LOE_HEROIC"
    },
    {
      (Enum) PresenceAdventureMode.LOE_CLASS_CHALLENGE,
      "PRESENCE_ADVENTURE_MODE_LOE_CLASS_CHALLENGE"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_NORMAL,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_LOE_NORMAL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_HEROIC,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_LOE_HEROIC"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_CLASS_CHALLENGE,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_LOE_CLASS_CHALLENGE"
    },
    {
      (Enum) PresenceAdventureMode.KAR_NORMAL,
      "PRESENCE_ADVENTURE_MODE_KAR_NORMAL"
    },
    {
      (Enum) PresenceAdventureMode.KAR_HEROIC,
      "PRESENCE_ADVENTURE_MODE_KAR_HEROIC"
    },
    {
      (Enum) PresenceAdventureMode.KAR_CLASS_CHALLENGE,
      "PRESENCE_ADVENTURE_MODE_KAR_CLASS_CHALLENGE"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_KAR_NORMAL,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_KAR_NORMAL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_KAR_HEROIC,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_KAR_HEROIC"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_KAR_CLASS_CHALLENGE,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_KAR_CLASS_CHALLENGE"
    },
    {
      (Enum) PresenceAdventureMode.ICC_NORMAL,
      "PRESENCE_ADVENTURE_MODE_ICC_NORMAL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_ICC_NORMAL,
      "PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_ICC_NORMAL"
    },
    {
      (Enum) PresenceAdventureMode.LOOT,
      "PRESENCE_ADVENTURE_MODE_LOOT"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_LOOT,
      "PRESENCE_STATUS_SPECTATING_GAME_LOOT"
    },
    {
      (Enum) PresenceAdventureMode.GIL,
      "PRESENCE_ADVENTURE_MODE_GIL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_GIL,
      "PRESENCE_STATUS_SPECTATING_GAME_GIL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_GIL_BONUS_CHALLENGE,
      "PRESENCE_STATUS_SPECTATING_GAME_GIL_BONUS_CHALLENGE"
    },
    {
      (Enum) PresenceAdventureMode.BOT,
      "PRESENCE_ADVENTURE_MODE_BOT"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BOT,
      "PRESENCE_STATUS_SPECTATING_GAME_BOT"
    },
    {
      (Enum) PresenceAdventureMode.TRL,
      "PRESENCE_ADVENTURE_MODE_TRL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_TRL,
      "PRESENCE_STATUS_SPECTATING_GAME_TRL"
    },
    {
      (Enum) PresenceAdventureMode.DAL,
      "PRESENCE_ADVENTURE_MODE_DAL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_DAL,
      "PRESENCE_STATUS_SPECTATING_GAME_DAL"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_DAL_HEROIC,
      "PRESENCE_STATUS_SPECTATING_GAME_DAL_HEROIC"
    },
    {
      (Enum) PresenceAdventureMode.ULD,
      "PRESENCE_ADVENTURE_MODE_ULD"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_ULD,
      "PRESENCE_STATUS_SPECTATING_GAME_ULD"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_ULD_HEROIC,
      "PRESENCE_STATUS_SPECTATING_GAME_ULD_HEROIC"
    },
    {
      (Enum) PresenceAdventureMode.DRG,
      "PRESENCE_ADVENTURE_MODE_DRG"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_DRG,
      "PRESENCE_STATUS_SPECTATING_GAME_DRG"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_DRG_HEROIC,
      "PRESENCE_STATUS_SPECTATING_GAME_DRG_HEROIC"
    },
    {
      (Enum) PresenceAdventureMode.BTP,
      "PRESENCE_ADVENTURE_MODE_BTP"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BTP,
      "PRESENCE_STATUS_SPECTATING_GAME_BTP"
    },
    {
      (Enum) PresenceAdventureMode.BTA,
      "PRESENCE_ADVENTURE_MODE_BTA"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BTA,
      "PRESENCE_STATUS_SPECTATING_GAME_BTA"
    },
    {
      (Enum) PresenceAdventureMode.BTA_HEROIC,
      "PRESENCE_ADVENTURE_MODE_BTA_HEROIC"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BTA_HEROIC,
      "PRESENCE_STATUS_SPECTATING_GAME_BTA_HEROIC"
    },
    {
      (Enum) PresenceAdventureMode.BOH,
      "PRESENCE_ADVENTURE_MODE_BOH"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BOH,
      "PRESENCE_STATUS_SPECTATING_GAME_BOH"
    },
    {
      (Enum) PresenceAdventureMode.BOM,
      "PRESENCE_ADVENTURE_MODE_BOM"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BOM,
      "PRESENCE_STATUS_SPECTATING_GAME_BOM"
    },
    {
      (Enum) PresenceAdventureMode.RLK,
      "PRESENCE_ADVENTURE_MODE_RLK"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_RLK,
      "PRESENCE_STATUS_SPECTATING_GAME_RLK"
    },
    {
      (Enum) ScenarioDbId.NAXX_ANUBREKHAN,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_ANUBREKHAN"
    },
    {
      (Enum) ScenarioDbId.NAXX_FAERLINA,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_FAERLINA"
    },
    {
      (Enum) ScenarioDbId.NAXX_MAEXXNA,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_MAEXXNA"
    },
    {
      (Enum) ScenarioDbId.NAXX_NOTH,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_NOTH"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEIGAN,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_HEIGAN"
    },
    {
      (Enum) ScenarioDbId.NAXX_LOATHEB,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_LOATHEB"
    },
    {
      (Enum) ScenarioDbId.NAXX_RAZUVIOUS,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_RAZUVIOUS"
    },
    {
      (Enum) ScenarioDbId.NAXX_GOTHIK,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_GOTHIK"
    },
    {
      (Enum) ScenarioDbId.NAXX_HORSEMEN,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_HORSEMEN"
    },
    {
      (Enum) ScenarioDbId.NAXX_PATCHWERK,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_PATCHWERK"
    },
    {
      (Enum) ScenarioDbId.NAXX_GROBBULUS,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_GROBBULUS"
    },
    {
      (Enum) ScenarioDbId.NAXX_GLUTH,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_GLUTH"
    },
    {
      (Enum) ScenarioDbId.NAXX_THADDIUS,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_THADDIUS"
    },
    {
      (Enum) ScenarioDbId.NAXX_SAPPHIRON,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_SAPPHIRON"
    },
    {
      (Enum) ScenarioDbId.NAXX_KELTHUZAD,
      "PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_KELTHUZAD"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_ANUBREKHAN,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_ANUBREKHAN"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_FAERLINA,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_FAERLINA"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_MAEXXNA,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_MAEXXNA"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_NOTH,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_NOTH"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_HEIGAN,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_HEIGAN"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_LOATHEB,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_LOATHEB"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_RAZUVIOUS,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_RAZUVIOUS"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_GOTHIK,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_GOTHIK"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_HORSEMEN,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_HORSEMEN"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_PATCHWERK,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_PATCHWERK"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_GROBBULUS,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_GROBBULUS"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_GLUTH,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_GLUTH"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_THADDIUS,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_THADDIUS"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_SAPPHIRON,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_SAPPHIRON"
    },
    {
      (Enum) ScenarioDbId.NAXX_HEROIC_KELTHUZAD,
      "PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_KELTHUZAD"
    },
    {
      (Enum) ScenarioDbId.NAXX_CHALLENGE_HUNTER_V_LOATHEB,
      "PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_HUNTER"
    },
    {
      (Enum) ScenarioDbId.NAXX_CHALLENGE_WARRIOR_V_GROBBULUS,
      "PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_WARRIOR"
    },
    {
      (Enum) ScenarioDbId.NAXX_CHALLENGE_ROGUE_V_MAEXXNA,
      "PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_ROGUE"
    },
    {
      (Enum) ScenarioDbId.NAXX_CHALLENGE_DRUID_V_FAERLINA,
      "PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_DRUID"
    },
    {
      (Enum) ScenarioDbId.NAXX_CHALLENGE_PRIEST_V_THADDIUS,
      "PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_PRIEST"
    },
    {
      (Enum) ScenarioDbId.NAXX_CHALLENGE_SHAMAN_V_GOTHIK,
      "PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_SHAMAN"
    },
    {
      (Enum) ScenarioDbId.NAXX_CHALLENGE_MAGE_V_HEIGAN,
      "PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_MAGE"
    },
    {
      (Enum) ScenarioDbId.NAXX_CHALLENGE_PALADIN_V_KELTHUZAD,
      "PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_PALADIN"
    },
    {
      (Enum) ScenarioDbId.NAXX_CHALLENGE_WARLOCK_V_HORSEMEN,
      "PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_WARLOCK"
    },
    {
      (Enum) ScenarioDbId.BRM_GRIM_GUZZLER,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_GRIM_GUZZLER"
    },
    {
      (Enum) ScenarioDbId.BRM_DARK_IRON_ARENA,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_DARK_IRON_ARENA"
    },
    {
      (Enum) ScenarioDbId.BRM_THAURISSAN,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_THAURISSAN"
    },
    {
      (Enum) ScenarioDbId.BRM_GARR,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_GARR"
    },
    {
      (Enum) ScenarioDbId.BRM_MAJORDOMO,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_MAJORDOMO"
    },
    {
      (Enum) ScenarioDbId.BRM_BARON_GEDDON,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_BARON_GEDDON"
    },
    {
      (Enum) ScenarioDbId.BRM_OMOKK,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_OMOKK"
    },
    {
      (Enum) ScenarioDbId.BRM_DRAKKISATH,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_DRAKKISATH"
    },
    {
      (Enum) ScenarioDbId.BRM_REND_BLACKHAND,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_REND_BLACKHAND"
    },
    {
      (Enum) ScenarioDbId.BRM_RAZORGORE,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_RAZORGORE"
    },
    {
      (Enum) ScenarioDbId.BRM_VAELASTRASZ,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_VAELASTRASZ"
    },
    {
      (Enum) ScenarioDbId.BRM_CHROMAGGUS,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_CHROMAGGUS"
    },
    {
      (Enum) ScenarioDbId.BRM_NEFARIAN,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_NEFARIAN"
    },
    {
      (Enum) ScenarioDbId.BRM_OMNOTRON,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_OMNOTRON"
    },
    {
      (Enum) ScenarioDbId.BRM_MALORIAK,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_MALORIAK"
    },
    {
      (Enum) ScenarioDbId.BRM_ATRAMEDES,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_ATRAMEDES"
    },
    {
      (Enum) ScenarioDbId.BRM_ZOMBIE_NEF,
      "PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_ZOMBIE_NEF"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_GRIM_GUZZLER,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_GRIM_GUZZLER"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_DARK_IRON_ARENA,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_DARK_IRON_ARENA"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_THAURISSAN,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_THAURISSAN"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_GARR,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_GARR"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_MAJORDOMO,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_MAJORDOMO"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_BARON_GEDDON,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_BARON_GEDDON"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_OMOKK,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_OMOKK"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_DRAKKISATH,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_DRAKKISATH"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_REND_BLACKHAND,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_REND_BLACKHAND"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_RAZORGORE,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_RAZORGORE"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_VAELASTRASZ,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_VAELASTRASZ"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_CHROMAGGUS,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_CHROMAGGUS"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_NEFARIAN,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_NEFARIAN"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_OMNOTRON,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_OMNOTRON"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_MALORIAK,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_MALORIAK"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_ATRAMEDES,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_ATRAMEDES"
    },
    {
      (Enum) ScenarioDbId.BRM_HEROIC_ZOMBIE_NEF,
      "PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_ZOMBIE_NEF"
    },
    {
      (Enum) ScenarioDbId.BRM_CHALLENGE_HUNTER_V_GUZZLER,
      "PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_HUNTER"
    },
    {
      (Enum) ScenarioDbId.BRM_CHALLENGE_WARRIOR_V_GARR,
      "PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_WARRIOR"
    },
    {
      (Enum) ScenarioDbId.BRM_CHALLENGE_ROGUE_V_VAELASTRASZ,
      "PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_ROGUE"
    },
    {
      (Enum) ScenarioDbId.BRM_CHALLENGE_DRUID_V_BLACKHAND,
      "PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_DRUID"
    },
    {
      (Enum) ScenarioDbId.BRM_CHALLENGE_PRIEST_V_DRAKKISATH,
      "PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_PRIEST"
    },
    {
      (Enum) ScenarioDbId.BRM_CHALLENGE_SHAMAN_V_GEDDON,
      "PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_SHAMAN"
    },
    {
      (Enum) ScenarioDbId.BRM_CHALLENGE_MAGE_V_DARK_IRON_ARENA,
      "PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_MAGE"
    },
    {
      (Enum) ScenarioDbId.BRM_CHALLENGE_PALADIN_V_OMNOTRON,
      "PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_PALADIN"
    },
    {
      (Enum) ScenarioDbId.BRM_CHALLENGE_WARLOCK_V_RAZORGORE,
      "PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_WARLOCK"
    },
    {
      (Enum) ScenarioDbId.LOE_ZINAAR,
      "PRESENCE_SCENARIO_LOE_NORMAL_ZINAAR"
    },
    {
      (Enum) ScenarioDbId.LOE_SUN_RAIDER_PHAERIX,
      "PRESENCE_SCENARIO_LOE_NORMAL_SUN_RAIDER_PHAERIX"
    },
    {
      (Enum) ScenarioDbId.LOE_TEMPLE_ESCAPE,
      "PRESENCE_SCENARIO_LOE_NORMAL_TEMPLE_ESCAPE"
    },
    {
      (Enum) ScenarioDbId.LOE_SCARVASH,
      "PRESENCE_SCENARIO_LOE_NORMAL_SCARVASH"
    },
    {
      (Enum) ScenarioDbId.LOE_MINE_CART,
      "PRESENCE_SCENARIO_LOE_NORMAL_MINE_CART"
    },
    {
      (Enum) ScenarioDbId.LOE_ARCHAEDAS,
      "PRESENCE_SCENARIO_LOE_NORMAL_ARCHAEDAS"
    },
    {
      (Enum) ScenarioDbId.LOE_SLITHERSPEAR,
      "PRESENCE_SCENARIO_LOE_NORMAL_SLITHERSPEAR"
    },
    {
      (Enum) ScenarioDbId.LOE_GIANTFIN,
      "PRESENCE_SCENARIO_LOE_NORMAL_GIANTFIN"
    },
    {
      (Enum) ScenarioDbId.LOE_LADY_NAZJAR,
      "PRESENCE_SCENARIO_LOE_NORMAL_LADY_NAZJAR"
    },
    {
      (Enum) ScenarioDbId.LOE_SKELESAURUS,
      "PRESENCE_SCENARIO_LOE_NORMAL_SKELESAURUS"
    },
    {
      (Enum) ScenarioDbId.LOE_STEEL_SENTINEL,
      "PRESENCE_SCENARIO_LOE_NORMAL_STEEL_SENTINEL"
    },
    {
      (Enum) ScenarioDbId.LOE_RAFAAM_1,
      "PRESENCE_SCENARIO_LOE_NORMAL_RAFAAM_1"
    },
    {
      (Enum) ScenarioDbId.LOE_RAFAAM_2,
      "PRESENCE_SCENARIO_LOE_NORMAL_RAFAAM_2"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_ZINAAR,
      "PRESENCE_SCENARIO_LOE_HEROIC_ZINAAR"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_SUN_RAIDER_PHAERIX,
      "PRESENCE_SCENARIO_LOE_HEROIC_SUN_RAIDER_PHAERIX"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_TEMPLE_ESCAPE,
      "PRESENCE_SCENARIO_LOE_HEROIC_TEMPLE_ESCAPE"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_SCARVASH,
      "PRESENCE_SCENARIO_LOE_HEROIC_SCARVASH"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_MINE_CART,
      "PRESENCE_SCENARIO_LOE_HEROIC_MINE_CART"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_ARCHAEDAS,
      "PRESENCE_SCENARIO_LOE_HEROIC_ARCHAEDAS"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_SLITHERSPEAR,
      "PRESENCE_SCENARIO_LOE_HEROIC_SLITHERSPEAR"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_GIANTFIN,
      "PRESENCE_SCENARIO_LOE_HEROIC_GIANTFIN"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_LADY_NAZJAR,
      "PRESENCE_SCENARIO_LOE_HEROIC_LADY_NAZJAR"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_SKELESAURUS,
      "PRESENCE_SCENARIO_LOE_HEROIC_SKELESAURUS"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_STEEL_SENTINEL,
      "PRESENCE_SCENARIO_LOE_HEROIC_STEEL_SENTINEL"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_RAFAAM_1,
      "PRESENCE_SCENARIO_LOE_HEROIC_RAFAAM_1"
    },
    {
      (Enum) ScenarioDbId.LOE_HEROIC_RAFAAM_2,
      "PRESENCE_SCENARIO_LOE_HEROIC_RAFAAM_2"
    },
    {
      (Enum) ScenarioDbId.LOE_CHALLENGE_WARRIOR_V_ZINAAR,
      "PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_WARRIOR"
    },
    {
      (Enum) ScenarioDbId.LOE_CHALLENGE_WARLOCK_V_SUN_RAIDER,
      "PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_WARLOCK"
    },
    {
      (Enum) ScenarioDbId.LOE_CHALLENGE_DRUID_V_SCARVASH,
      "PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_DRUID"
    },
    {
      (Enum) ScenarioDbId.LOE_CHALLENGE_PALADIN_V_ARCHAEDUS,
      "PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_PALADIN"
    },
    {
      (Enum) ScenarioDbId.LOE_CHALLENGE_HUNTER_V_SLITHERSPEAR,
      "PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_HUNTER"
    },
    {
      (Enum) ScenarioDbId.LOE_CHALLENGE_SHAMAN_V_GIANTFIN,
      "PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_SHAMAN"
    },
    {
      (Enum) ScenarioDbId.LOE_CHALLENGE_PRIEST_V_NAZJAR,
      "PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_PRIEST"
    },
    {
      (Enum) ScenarioDbId.LOE_CHALLENGE_ROGUE_V_SKELESAURUS,
      "PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_ROGUE"
    },
    {
      (Enum) ScenarioDbId.LOE_CHALLENGE_MAGE_V_SENTINEL,
      "PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_MAGE"
    },
    {
      (Enum) ScenarioDbId.KAR_PROLOGUE,
      "PRESENCE_SCENARIO_KAR_NORMAL_PROLOGUE"
    },
    {
      (Enum) ScenarioDbId.KAR_PANTRY,
      "PRESENCE_SCENARIO_KAR_NORMAL_PANTRY"
    },
    {
      (Enum) ScenarioDbId.KAR_MIRROR,
      "PRESENCE_SCENARIO_KAR_NORMAL_MIRROR"
    },
    {
      (Enum) ScenarioDbId.KAR_CHESS,
      "PRESENCE_SCENARIO_KAR_NORMAL_CHESS"
    },
    {
      (Enum) ScenarioDbId.KAR_JULIANNE,
      "PRESENCE_SCENARIO_KAR_NORMAL_JULIANNE"
    },
    {
      (Enum) ScenarioDbId.KAR_WOLF,
      "PRESENCE_SCENARIO_KAR_NORMAL_WOLF"
    },
    {
      (Enum) ScenarioDbId.KAR_CRONE,
      "PRESENCE_SCENARIO_KAR_NORMAL_CRONE"
    },
    {
      (Enum) ScenarioDbId.KAR_CURATOR,
      "PRESENCE_SCENARIO_KAR_NORMAL_CURATOR"
    },
    {
      (Enum) ScenarioDbId.KAR_NIGHTBANE,
      "PRESENCE_SCENARIO_KAR_NORMAL_NIGHTBANE"
    },
    {
      (Enum) ScenarioDbId.KAR_ILLHOOF,
      "PRESENCE_SCENARIO_KAR_NORMAL_ILLHOOF"
    },
    {
      (Enum) ScenarioDbId.KAR_ARAN,
      "PRESENCE_SCENARIO_KAR_NORMAL_ARAN"
    },
    {
      (Enum) ScenarioDbId.KAR_NETHERSPITE,
      "PRESENCE_SCENARIO_KAR_NORMAL_NETHERSPITE"
    },
    {
      (Enum) ScenarioDbId.KAR_PORTALS,
      "PRESENCE_SCENARIO_KAR_NORMAL_PORTALS"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_PROLOGUE,
      "PRESENCE_SCENARIO_KAR_HEROIC_PROLOGUE"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_PANTRY,
      "PRESENCE_SCENARIO_KAR_HEROIC_PANTRY"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_MIRROR,
      "PRESENCE_SCENARIO_KAR_HEROIC_MIRROR"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_CHESS,
      "PRESENCE_SCENARIO_KAR_HEROIC_CHESS"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_JULIANNE,
      "PRESENCE_SCENARIO_KAR_HEROIC_JULIANNE"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_WOLF,
      "PRESENCE_SCENARIO_KAR_HEROIC_WOLF"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_CRONE,
      "PRESENCE_SCENARIO_KAR_HEROIC_CRONE"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_CURATOR,
      "PRESENCE_SCENARIO_KAR_HEROIC_CURATOR"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_NIGHTBANE,
      "PRESENCE_SCENARIO_KAR_HEROIC_NIGHTBANE"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_ILLHOOF,
      "PRESENCE_SCENARIO_KAR_HEROIC_ILLHOOF"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_ARAN,
      "PRESENCE_SCENARIO_KAR_HEROIC_ARAN"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_NETHERSPITE,
      "PRESENCE_SCENARIO_KAR_HEROIC_NETHERSPITE"
    },
    {
      (Enum) ScenarioDbId.KAR_HEROIC_PORTALS,
      "PRESENCE_SCENARIO_KAR_HEROIC_PORTALS"
    },
    {
      (Enum) ScenarioDbId.KAR_CHALLENGE_SHAMAN_V_MIRROR,
      "PRESENCE_SCENARIO_KAR_CLASS_CHALLENGE_SHAMAN"
    },
    {
      (Enum) ScenarioDbId.KAR_CHALLENGE_PRIEST_V_PANTRY,
      "PRESENCE_SCENARIO_KAR_CLASS_CHALLENGE_PRIEST"
    },
    {
      (Enum) ScenarioDbId.KAR_CHALLENGE_PALADIN_V_WOLF,
      "PRESENCE_SCENARIO_KAR_CLASS_CHALLENGE_PALADIN"
    },
    {
      (Enum) ScenarioDbId.KAR_CHALLENGE_WARLOCK_V_JULIANNE,
      "PRESENCE_SCENARIO_KAR_CLASS_CHALLENGE_WARLOCK"
    },
    {
      (Enum) ScenarioDbId.KAR_CHALLENGE_HUNTER_V_CURATOR,
      "PRESENCE_SCENARIO_KAR_CLASS_CHALLENGE_HUNTER"
    },
    {
      (Enum) ScenarioDbId.KAR_CHALLENGE_WARRIOR_V_ILLHOOF,
      "PRESENCE_SCENARIO_KAR_CLASS_CHALLENGE_WARRIOR"
    },
    {
      (Enum) ScenarioDbId.KAR_CHALLENGE_MAGE_V_NIGHTBANE,
      "PRESENCE_SCENARIO_KAR_CLASS_CHALLENGE_MAGE"
    },
    {
      (Enum) ScenarioDbId.KAR_CHALLENGE_ROGUE_V_ARAN,
      "PRESENCE_SCENARIO_KAR_CLASS_CHALLENGE_ROGUE"
    },
    {
      (Enum) ScenarioDbId.KAR_CHALLENGE_DRUID_V_NETHERSPITE,
      "PRESENCE_SCENARIO_KAR_CLASS_CHALLENGE_DRUID"
    },
    {
      (Enum) ScenarioDbId.RETURNING_PLAYER_CHALLENGE_1,
      "PRESENCE_SCENARIO_RETURNING_PLAYER_CHALLENGE_1"
    },
    {
      (Enum) ScenarioDbId.RETURNING_PLAYER_CHALLENGE_2,
      "PRESENCE_SCENARIO_RETURNING_PLAYER_CHALLENGE_2"
    },
    {
      (Enum) ScenarioDbId.RETURNING_PLAYER_CHALLENGE_3,
      "PRESENCE_SCENARIO_RETURNING_PLAYER_CHALLENGE_3"
    },
    {
      (Enum) ScenarioDbId.ICC_01_LICHKING,
      "PRESENCE_SCENARIO_ICC_NORMAL_LICHKING"
    },
    {
      (Enum) ScenarioDbId.ICC_04_SINDRAGOSA,
      "PRESENCE_SCENARIO_ICC_NORMAL_SINDRAGOSA"
    },
    {
      (Enum) ScenarioDbId.ICC_06_MARROWGAR,
      "PRESENCE_SCENARIO_ICC_NORMAL_MARROWGAR"
    },
    {
      (Enum) ScenarioDbId.ICC_05_LANATHEL,
      "PRESENCE_SCENARIO_ICC_NORMAL_LANATHEL"
    },
    {
      (Enum) ScenarioDbId.ICC_07_PUTRICIDE,
      "PRESENCE_SCENARIO_ICC_NORMAL_PUTRICIDE"
    },
    {
      (Enum) ScenarioDbId.ICC_08_FINALE,
      "PRESENCE_SCENARIO_ICC_NORMAL_FINALE"
    },
    {
      (Enum) ScenarioDbId.ICC_09_SAURFANG,
      "PRESENCE_SCENARIO_ICC_NORMAL_SAURFANG"
    },
    {
      (Enum) ScenarioDbId.ICC_10_DEATHWHISPER,
      "PRESENCE_SCENARIO_ICC_NORMAL_DEATHWHISPER"
    },
    {
      (Enum) ScenarioDbId.LOOT_DUNGEON,
      "PRESENCE_SCENARIO_LOOT_DUNGEON"
    },
    {
      (Enum) ScenarioDbId.GIL_DUNGEON,
      "PRESENCE_SCENARIO_GIL_DUNGEON"
    },
    {
      (Enum) ScenarioDbId.GIL_BONUS_CHALLENGE,
      "PRESENCE_SCENARIO_GIL_BONUS_CHALLENGE"
    },
    {
      (Enum) ScenarioDbId.BOTA_MIRROR_PUZZLE_1,
      "PRESENCE_SCENARIO_BOTA_MIRROR_PUZZLE_1"
    },
    {
      (Enum) ScenarioDbId.BOTA_MIRROR_PUZZLE_2,
      "PRESENCE_SCENARIO_BOTA_MIRROR_PUZZLE_2"
    },
    {
      (Enum) ScenarioDbId.BOTA_MIRROR_PUZZLE_3,
      "PRESENCE_SCENARIO_BOTA_MIRROR_PUZZLE_3"
    },
    {
      (Enum) ScenarioDbId.BOTA_MIRROR_PUZZLE_4,
      "PRESENCE_SCENARIO_BOTA_MIRROR_PUZZLE_4"
    },
    {
      (Enum) ScenarioDbId.BOTA_MIRROR_BOOM,
      "PRESENCE_SCENARIO_BOTA_MIRROR_BOOM"
    },
    {
      (Enum) ScenarioDbId.BOTA_LETHAL_PUZZLE_1,
      "PRESENCE_SCENARIO_BOTA_LETHAL_PUZZLE_1"
    },
    {
      (Enum) ScenarioDbId.BOTA_LETHAL_PUZZLE_2,
      "PRESENCE_SCENARIO_BOTA_LETHAL_PUZZLE_2"
    },
    {
      (Enum) ScenarioDbId.BOTA_LETHAL_PUZZLE_3,
      "PRESENCE_SCENARIO_BOTA_LETHAL_PUZZLE_3"
    },
    {
      (Enum) ScenarioDbId.BOTA_LETHAL_PUZZLE_4,
      "PRESENCE_SCENARIO_BOTA_LETHAL_PUZZLE_4"
    },
    {
      (Enum) ScenarioDbId.BOTA_LETHAL_BOOM,
      "PRESENCE_SCENARIO_BOTA_LETHAL_BOOM"
    },
    {
      (Enum) ScenarioDbId.BOTA_CLEAR_PUZZLE_1,
      "PRESENCE_SCENARIO_BOTA_CLEAR_PUZZLE_1"
    },
    {
      (Enum) ScenarioDbId.BOTA_CLEAR_PUZZLE_2,
      "PRESENCE_SCENARIO_BOTA_CLEAR_PUZZLE_2"
    },
    {
      (Enum) ScenarioDbId.BOTA_CLEAR_PUZZLE_3,
      "PRESENCE_SCENARIO_BOTA_CLEAR_PUZZLE_3"
    },
    {
      (Enum) ScenarioDbId.BOTA_CLEAR_PUZZLE_4,
      "PRESENCE_SCENARIO_BOTA_CLEAR_PUZZLE_4"
    },
    {
      (Enum) ScenarioDbId.BOTA_CLEAR_BOOM,
      "PRESENCE_SCENARIO_BOTA_CLEAR_BOOM"
    },
    {
      (Enum) ScenarioDbId.BOTA_SURVIVAL_PUZZLE_1,
      "PRESENCE_SCENARIO_BOTA_SURVIVAL_PUZZLE_1"
    },
    {
      (Enum) ScenarioDbId.BOTA_SURVIVAL_PUZZLE_2,
      "PRESENCE_SCENARIO_BOTA_SURVIVAL_PUZZLE_2"
    },
    {
      (Enum) ScenarioDbId.BOTA_SURVIVAL_PUZZLE_3,
      "PRESENCE_SCENARIO_BOTA_SURVIVAL_PUZZLE_3"
    },
    {
      (Enum) ScenarioDbId.BOTA_SURVIVAL_PUZZLE_4,
      "PRESENCE_SCENARIO_BOTA_SURVIVAL_PUZZLE_4"
    },
    {
      (Enum) ScenarioDbId.BOTA_SURVIVAL_BOOM,
      "PRESENCE_SCENARIO_BOTA_SURVIVAL_BOOM"
    },
    {
      (Enum) ScenarioDbId.TRL_DUNGEON,
      "PRESENCE_SCENARIO_TRL_DUNGEON"
    },
    {
      (Enum) ScenarioDbId.DALA_01_BANK,
      "PRESENCE_SCENARIO_DALA_01_BANK"
    },
    {
      (Enum) ScenarioDbId.DALA_01_BANK_HEROIC,
      "PRESENCE_SCENARIO_DALA_01_BANK_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DALA_02_VIOLET_HOLD,
      "PRESENCE_SCENARIO_DALA_02_VIOLET_HOLD"
    },
    {
      (Enum) ScenarioDbId.DALA_02_VIOLET_HOLD_HEROIC,
      "PRESENCE_SCENARIO_DALA_02_VIOLET_HOLD_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DALA_03_STREETS,
      "PRESENCE_SCENARIO_DALA_03_STREETS"
    },
    {
      (Enum) ScenarioDbId.DALA_03_STREETS_HEROIC,
      "PRESENCE_SCENARIO_DALA_03_STREETS_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DALA_04_UNDERBELLY,
      "PRESENCE_SCENARIO_DALA_04_UNDERBELLY"
    },
    {
      (Enum) ScenarioDbId.DALA_04_UNDERBELLY_HEROIC,
      "PRESENCE_SCENARIO_DALA_04_UNDERBELLY_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DALA_05_CITADEL,
      "PRESENCE_SCENARIO_DALA_05_CITADEL"
    },
    {
      (Enum) ScenarioDbId.DALA_05_CITADEL_HEROIC,
      "PRESENCE_SCENARIO_DALA_05_CITADEL_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DALA_TAVERN,
      "PRESENCE_SCENARIO_DALA_TAVERN"
    },
    {
      (Enum) ScenarioDbId.DALA_TAVERN_HEROIC,
      "PRESENCE_SCENARIO_DALA_TAVERN_HEROIC"
    },
    {
      (Enum) ScenarioDbId.ULDA_CITY,
      "PRESENCE_SCENARIO_ULD_01_CITY"
    },
    {
      (Enum) ScenarioDbId.ULDA_CITY_HEROIC,
      "PRESENCE_SCENARIO_ULD_01_CITY_HEROIC"
    },
    {
      (Enum) ScenarioDbId.ULDA_DESERT,
      "PRESENCE_SCENARIO_ULD_02_DESERT"
    },
    {
      (Enum) ScenarioDbId.ULDA_DESERT_HEROIC,
      "PRESENCE_SCENARIO_ULD_02_DESERT_HEROIC"
    },
    {
      (Enum) ScenarioDbId.ULDA_TOMB,
      "PRESENCE_SCENARIO_ULD_03_TOMB"
    },
    {
      (Enum) ScenarioDbId.ULDA_TOMB_HEROIC,
      "PRESENCE_SCENARIO_ULD_03_TOMB_HEROIC"
    },
    {
      (Enum) ScenarioDbId.ULDA_HALLS,
      "PRESENCE_SCENARIO_ULD_04_HALLS"
    },
    {
      (Enum) ScenarioDbId.ULDA_HALLS_HEROIC,
      "PRESENCE_SCENARIO_ULD_04_HALLS_HEROIC"
    },
    {
      (Enum) ScenarioDbId.ULDA_SANCTUM,
      "PRESENCE_SCENARIO_ULD_05_SANCTUM"
    },
    {
      (Enum) ScenarioDbId.ULDA_SANCTUM_HEROIC,
      "PRESENCE_SCENARIO_ULD_05_SANCTUM_HEROIC"
    },
    {
      (Enum) ScenarioDbId.ULDA_TAVERN,
      "PRESENCE_SCENARIO_ULD_TAVERN"
    },
    {
      (Enum) ScenarioDbId.ULDA_TAVERN_HEROIC,
      "PRESENCE_SCENARIO_ULD_TAVERN_HEROIC"
    },
    {
      (Enum) Global.PresenceStatus.BATTLEGROUNDS_QUEUE,
      "PRESENCE_STATUS_BATTLEGROUNDS_QUEUE"
    },
    {
      (Enum) Global.PresenceStatus.BATTLEGROUNDS_GAME,
      "PRESENCE_STATUS_BATTLEGROUNDS_GAME"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_BATTLEGROUNDS,
      "PRESENCE_STATUS_SPECTATING_GAME_BATTLEGROUNDS"
    },
    {
      (Enum) Global.PresenceStatus.BATTLEGROUNDS_SCREEN,
      "PRESENCE_STATUS_BATTLEGROUNDS_SCREEN"
    },
    {
      (Enum) Global.PresenceStatus.DUELS_QUEUE,
      "PRESENCE_STATUS_DUELS_QUEUE"
    },
    {
      (Enum) Global.PresenceStatus.DUELS_GAME,
      "PRESENCE_STATUS_DUELS_GAME"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_DUELS,
      "PRESENCE_STATUS_SPECTATING_GAME_DUELS"
    },
    {
      (Enum) Global.PresenceStatus.DUELS_IDLE,
      "PRESENCE_STATUS_DUELS_IDLE"
    },
    {
      (Enum) Global.PresenceStatus.DUELS_BUILDING_DECK,
      "PRESENCE_STATUS_DUELS_FORGE"
    },
    {
      (Enum) Global.PresenceStatus.DUELS_PURCHASE,
      "PRESENCE_STATUS_DUELS_PURCHASE"
    },
    {
      (Enum) Global.PresenceStatus.DUELS_REWARD,
      "PRESENCE_STATUS_DUELS_REWARD"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_QUEUE,
      "PRESENCE_STATUS_MERCENARIES_QUEUE"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_PLAY_SCREEN,
      "PRESENCE_STATUS_MERCENARIES_PLAY_SCREEN"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_GAME,
      "PRESENCE_STATUS_MERCENARIES_GAME"
    },
    {
      (Enum) Global.PresenceStatus.SPECTATING_GAME_MERCENARIES,
      "PRESENCE_STATUS_SPECTATING_GAME_MERCENARIES"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_COLLECTION,
      "PRESENCE_STATUS_MERCENARIES_COLLECTION"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_TEAM_EDITOR,
      "PRESENCE_STATUS_MERCENARIES_TEAM_EDITOR"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE,
      "PRESENCE_STATUS_MERCENARIES_VILLAGE"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_TASKBOARD,
      "PRESENCE_STATUS_MERCENARIES_VILLAGE_TASKBOARD"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_BUILDING_MANAGER,
      "PRESENCE_STATUS_MERCENARIES_VILLAGE_BUILDING_MANAGER"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_PVE_ZONES,
      "PRESENCE_STATUS_MERCENARIES_VILLAGE_PVE_ZONES"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_PVE_BOUNTIES,
      "PRESENCE_STATUS_MERCENARIES_VILLAGE_PVE_BOUNTIES"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_PVP,
      "PRESENCE_STATUS_MERCENARIES_VILLAGE_PVP"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_VILLAGE_MAILBOX,
      "PRESENCE_STATUS_MERCENARIES_VILLAGE_MAILBOX"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_MAP,
      "PRESENCE_STATUS_MERCENARIES_MAP"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_FRIENDLY_LOBBY,
      "PRESENCE_STATUS_MERCENARIES_FRIENDLY_LOBBY"
    },
    {
      (Enum) Global.PresenceStatus.MERCENARIES_FRIENDLY_GAME,
      "PRESENCE_STATUS_MERCENARIES_FRIENDLY_GAME"
    },
    {
      (Enum) Global.PresenceStatus.VIEWING_JOURNAL,
      "PRESENCE_STATUS_VIEWING_JOURNAL"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_01,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_01_COIN_01"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_01_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_01_COIN_01_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_02,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_01_COIN_02"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_02_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_01_COIN_02_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_03,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_01_COIN_03"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_03_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_01_COIN_03_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_04,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_02_COIN_01"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_04_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_02_COIN_01_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_05,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_02_COIN_02"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_05_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_02_COIN_02_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_06,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_02_COIN_03"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_06_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_02_COIN_03_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_07,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_03_COIN_01"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_07_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_03_COIN_01_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_08,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_03_COIN_02"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_08_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_03_COIN_02_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_09,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_03_COIN_03"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_09_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_03_COIN_03_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_10,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_04_COIN_01"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_10_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_04_COIN_01_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_11,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_04_COIN_02"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_11_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_04_COIN_02_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_12,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_04_COIN_03"
    },
    {
      (Enum) ScenarioDbId.DRGA_Evil_12_Heroic,
      "PRESENCE_SCENARIO_DRGA_EVIL_CHAPTER_04_COIN_03_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_01,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_01_COIN_01"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_01_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_01_COIN_01_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_02,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_01_COIN_02"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_02_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_01_COIN_02_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_03,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_01_COIN_03"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_03_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_01_COIN_03_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_04,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_02_COIN_01"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_04_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_02_COIN_01_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_05,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_02_COIN_02"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_05_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_02_COIN_02_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_06,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_02_COIN_03"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_06_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_02_COIN_03_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_07,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_03_COIN_01"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_07_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_03_COIN_01_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_08,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_03_COIN_02"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_08_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_03_COIN_02_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_09,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_03_COIN_03"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_09_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_03_COIN_03_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_10,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_04_COIN_01"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_10_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_04_COIN_01_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_11,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_04_COIN_02"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_11_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_04_COIN_02_HEROIC"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_12,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_04_COIN_03"
    },
    {
      (Enum) ScenarioDbId.DRGA_Good_12_Heroic,
      "PRESENCE_SCENARIO_DRGA_GOOD_CHAPTER_04_COIN_03_HEROIC"
    },
    {
      (Enum) ScenarioDbId.BTP_01_AZZINOTH,
      "PRESENCE_SCENARIO_BTP_COIN_01_AZZINOTH"
    },
    {
      (Enum) ScenarioDbId.BTP_02_XAVIUS,
      "PRESENCE_SCENARIO_BTP_COIN_02_XAVIUS"
    },
    {
      (Enum) ScenarioDbId.BTP_03_MANNOROTH,
      "PRESENCE_SCENARIO_BTP_COIN_03_MANNOROTH"
    },
    {
      (Enum) ScenarioDbId.BTP_04_CENARIUS,
      "PRESENCE_SCENARIO_BTP_COIN_04_CENARIUS"
    },
    {
      (Enum) ScenarioDbId.BTA_01_INQUISITOR_DAKREL,
      "PRESENCE_SCENARIO_BTA_COIN_01_INQUISITOR_DAKREL"
    },
    {
      (Enum) ScenarioDbId.BTA_02_XUR_GOTH,
      "PRESENCE_SCENARIO_BTA_COIN_02_XUR_GOTH"
    },
    {
      (Enum) ScenarioDbId.BTA_03_ZIXOR,
      "PRESENCE_SCENARIO_BTA_COIN_03_ZIXOR"
    },
    {
      (Enum) ScenarioDbId.BTA_04_BALTHARAK,
      "PRESENCE_SCENARIO_BTA_COIN_04_BALTHARAK"
    },
    {
      (Enum) ScenarioDbId.BTA_05_KANRETHAD_PRIME,
      "PRESENCE_SCENARIO_BTA_COIN_05_KANRETHAD_PRIME"
    },
    {
      (Enum) ScenarioDbId.BTA_06_BURGRAK_CRUELCHAIN,
      "PRESENCE_SCENARIO_BTA_COIN_06_BURGRAK_CRUELCHAIN"
    },
    {
      (Enum) ScenarioDbId.BTA_07_FELSTORM_RUN,
      "PRESENCE_SCENARIO_BTA_COIN_07_FELSTORM_RUN"
    },
    {
      (Enum) ScenarioDbId.BTA_08_MOTHER_SHAHRAZ,
      "PRESENCE_SCENARIO_BTA_COIN_08_MOTHER_SHAHRAZ"
    },
    {
      (Enum) ScenarioDbId.BTA_09_SHAL_JA_OUTCAST,
      "PRESENCE_SCENARIO_BTA_COIN_09_SHAL_JA_OUTCAST"
    },
    {
      (Enum) ScenarioDbId.BTA_10_KARNUK_OUTCAST,
      "PRESENCE_SCENARIO_BTA_COIN_10_KARNUK_OUTCAST"
    },
    {
      (Enum) ScenarioDbId.BTA_11_JEK_HAZ,
      "PRESENCE_SCENARIO_BTA_COIN_11_JEK_HAZ"
    },
    {
      (Enum) ScenarioDbId.BTA_12_MAGTHERIDON_PRIME,
      "PRESENCE_SCENARIO_BTA_COIN_12_MAGTHERIDON_PRIME"
    },
    {
      (Enum) ScenarioDbId.BTA_13_GOK_AMOK,
      "PRESENCE_SCENARIO_BTA_COIN_13_GOK_AMOK"
    },
    {
      (Enum) ScenarioDbId.BTA_14_FLIKK,
      "PRESENCE_SCENARIO_BTA_COIN_14_FLIKK"
    },
    {
      (Enum) ScenarioDbId.BTA_15_BADUU_CORRUPTED,
      "PRESENCE_SCENARIO_BTA_COIN_15_BADUU_CORRUPTED"
    },
    {
      (Enum) ScenarioDbId.BTA_16_MECHA_JARAXXUS,
      "PRESENCE_SCENARIO_BTA_COIN_16_MECHA_JARAXXUS"
    },
    {
      (Enum) ScenarioDbId.BTA_17_ILLIDAN_STORMRAGE,
      "PRESENCE_SCENARIO_BTA_COIN_17_ILLIDAN_STORMRAGE"
    },
    {
      (Enum) ScenarioDbId.BTA_Heroic_KAZZAK,
      "PRESENCE_SCENARIO_BTA_HEROIC_COIN_01_KAZZAK"
    },
    {
      (Enum) ScenarioDbId.BTA_Heroic_GRUUL,
      "PRESENCE_SCENARIO_BTA_HEROIC_COIN_02_GRUUL"
    },
    {
      (Enum) ScenarioDbId.BTA_Heroic_MAGTHERIDON,
      "PRESENCE_SCENARIO_BTA_HEROIC_COIN_03_MAGTHERIDON"
    },
    {
      (Enum) ScenarioDbId.BTA_Heroic_SUPREMUS,
      "PRESENCE_SCENARIO_BTA_HEROIC_COIN_04_SUPREMUS"
    },
    {
      (Enum) ScenarioDbId.BTA_Heroic_TERON_GOREFIEND,
      "PRESENCE_SCENARIO_BTA_HEROIC_COIN_05_TERON_GOREFIEND"
    },
    {
      (Enum) ScenarioDbId.BTA_Heroic_MOTHER_SHARAZ,
      "PRESENCE_SCENARIO_BTA_HEROIC_COIN_06_MOTHER_SHARAZ"
    },
    {
      (Enum) ScenarioDbId.BTA_Heroic_LADY_VASHJ,
      "PRESENCE_SCENARIO_BTA_HEROIC_COIN_07_LADY_VASHJ"
    },
    {
      (Enum) ScenarioDbId.BTA_Heroic_KAELTHAS,
      "PRESENCE_SCENARIO_BTA_HEROIC_COIN_08_KAELTHAS"
    },
    {
      (Enum) ScenarioDbId.BTA_Heroic_ILLIDAN,
      "PRESENCE_SCENARIO_BTA_HEROIC_COIN_09_ILLIDAN"
    },
    {
      (Enum) ScenarioDbId.BOH_JAINA_01,
      "PRESENCE_SCENARIO_BOH_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_JAINA_02,
      "PRESENCE_SCENARIO_BOH_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_JAINA_03,
      "PRESENCE_SCENARIO_BOH_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_JAINA_04,
      "PRESENCE_SCENARIO_BOH_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_JAINA_05,
      "PRESENCE_SCENARIO_BOH_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOH_JAINA_06,
      "PRESENCE_SCENARIO_BOH_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_JAINA_07,
      "PRESENCE_SCENARIO_BOH_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_JAINA_08,
      "PRESENCE_SCENARIO_BOH_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_REXXAR_01,
      "PRESENCE_SCENARIO_BOH2_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_REXXAR_02,
      "PRESENCE_SCENARIO_BOH2_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_REXXAR_03,
      "PRESENCE_SCENARIO_BOH2_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_REXXAR_04,
      "PRESENCE_SCENARIO_BOH2_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_REXXAR_05,
      "PRESENCE_SCENARIO_BOH2_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOH_REXXAR_06,
      "PRESENCE_SCENARIO_BOH2_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_REXXAR_07,
      "PRESENCE_SCENARIO_BOH2_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_REXXAR_08,
      "PRESENCE_SCENARIO_BOH2_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_GARROSH_01,
      "PRESENCE_SCENARIO_BOH3_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_GARROSH_02,
      "PRESENCE_SCENARIO_BOH3_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_GARROSH_03,
      "PRESENCE_SCENARIO_BOH3_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_GARROSH_04,
      "PRESENCE_SCENARIO_BOH3_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_GARROSH_05,
      "PRESENCE_SCENARIO_BOH3_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOH_GARROSH_06,
      "PRESENCE_SCENARIO_BOH3_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_GARROSH_07,
      "PRESENCE_SCENARIO_BOH3_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_GARROSH_08,
      "PRESENCE_SCENARIO_BOH3_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_UTHER_01,
      "PRESENCE_SCENARIO_BOH4_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_UTHER_02,
      "PRESENCE_SCENARIO_BOH4_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_UTHER_03,
      "PRESENCE_SCENARIO_BOH4_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_UTHER_04,
      "PRESENCE_SCENARIO_BOH4_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_UTHER_05,
      "PRESENCE_SCENARIO_BOH4_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOH_UTHER_06,
      "PRESENCE_SCENARIO_BOH4_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_UTHER_07,
      "PRESENCE_SCENARIO_BOH4_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_UTHER_08,
      "PRESENCE_SCENARIO_BOH4_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_ANDUIN_01,
      "PRESENCE_SCENARIO_BOH5_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_ANDUIN_02,
      "PRESENCE_SCENARIO_BOH5_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_ANDUIN_03,
      "PRESENCE_SCENARIO_BOH5_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_ANDUIN_04,
      "PRESENCE_SCENARIO_BOH5_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_ANDUIN_05,
      "PRESENCE_SCENARIO_BOH5_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOH_ANDUIN_06,
      "PRESENCE_SCENARIO_BOH5_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_ANDUIN_07,
      "PRESENCE_SCENARIO_BOH5_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_ANDUIN_08,
      "PRESENCE_SCENARIO_BOH5_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_VALEERA_01,
      "PRESENCE_SCENARIO_BOH6_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_VALEERA_02,
      "PRESENCE_SCENARIO_BOH6_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_VALEERA_03,
      "PRESENCE_SCENARIO_BOH6_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_VALEERA_04,
      "PRESENCE_SCENARIO_BOH6_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_VALEERA_05,
      "PRESENCE_SCENARIO_BOH6_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOH_VALEERA_06,
      "PRESENCE_SCENARIO_BOH6_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_VALEERA_07,
      "PRESENCE_SCENARIO_BOH6_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_VALEERA_08,
      "PRESENCE_SCENARIO_BOH6_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_THRALL_01,
      "PRESENCE_SCENARIO_BOH7_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_THRALL_02,
      "PRESENCE_SCENARIO_BOH7_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_THRALL_03,
      "PRESENCE_SCENARIO_BOH7_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_THRALL_04,
      "PRESENCE_SCENARIO_BOH7_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_THRALL_05,
      "PRESENCE_SCENARIO_BOH7_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOH_THRALL_06,
      "PRESENCE_SCENARIO_BOH7_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_THRALL_07,
      "PRESENCE_SCENARIO_BOH7_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_THRALL_08,
      "PRESENCE_SCENARIO_BOH7_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_MALFURION_01,
      "PRESENCE_SCENARIO_BOH8_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_MALFURION_02,
      "PRESENCE_SCENARIO_BOH8_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_MALFURION_03,
      "PRESENCE_SCENARIO_BOH8_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_MALFURION_04,
      "PRESENCE_SCENARIO_BOH8_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_MALFURION_05,
      "PRESENCE_SCENARIO_BOH8_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOH_MALFURION_06,
      "PRESENCE_SCENARIO_BOH8_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_MALFURION_07,
      "PRESENCE_SCENARIO_BOH8_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_MALFURION_08,
      "PRESENCE_SCENARIO_BOH8_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_GULDAN_01,
      "PRESENCE_SCENARIO_BOH9_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_GULDAN_02,
      "PRESENCE_SCENARIO_BOH9_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_GULDAN_03,
      "PRESENCE_SCENARIO_BOH9_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_GULDAN_04,
      "PRESENCE_SCENARIO_BOH9_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_GULDAN_05,
      "PRESENCE_SCENARIO_BOH9_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOH_GULDAN_06,
      "PRESENCE_SCENARIO_BOH9_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_GULDAN_07,
      "PRESENCE_SCENARIO_BOH9_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_GULDAN_08,
      "PRESENCE_SCENARIO_BOH9_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_ILLIDAN_01,
      "PRESENCE_SCENARIO_BOH10_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_ILLIDAN_02,
      "PRESENCE_SCENARIO_BOH10_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_ILLIDAN_03,
      "PRESENCE_SCENARIO_BOH10_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_ILLIDAN_04,
      "PRESENCE_SCENARIO_BOH10_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_ILLIDAN_05,
      "PRESENCE_SCENARIO_BOH10_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOH_ILLIDAN_06,
      "PRESENCE_SCENARIO_BOH10_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_ILLIDAN_07,
      "PRESENCE_SCENARIO_BOH10_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_ILLIDAN_08,
      "PRESENCE_SCENARIO_BOH10_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_01,
      "PRESENCE_SCENARIO_BOH11_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_02,
      "PRESENCE_SCENARIO_BOH11_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_03,
      "PRESENCE_SCENARIO_BOH11_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_04,
      "PRESENCE_SCENARIO_BOH11_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_05A,
      "PRESENCE_SCENARIO_BOH11_FIGHT_05A"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_05B,
      "PRESENCE_SCENARIO_BOH11_FIGHT_05B"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_06,
      "PRESENCE_SCENARIO_BOH11_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_07,
      "PRESENCE_SCENARIO_BOH11_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_08,
      "PRESENCE_SCENARIO_BOH11_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_09A,
      "PRESENCE_SCENARIO_BOH11_FIGHT_09A"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_09B,
      "PRESENCE_SCENARIO_BOH11_FIGHT_09B"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_10A,
      "PRESENCE_SCENARIO_BOH11_FIGHT_10A"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_10B,
      "PRESENCE_SCENARIO_BOH11_FIGHT_10B"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_11,
      "PRESENCE_SCENARIO_BOH11_FIGHT_11"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_12,
      "PRESENCE_SCENARIO_BOH11_FIGHT_12"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_13,
      "PRESENCE_SCENARIO_BOH11_FIGHT_13"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_14,
      "PRESENCE_SCENARIO_BOH11_FIGHT_14"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_15,
      "PRESENCE_SCENARIO_BOH11_FIGHT_15"
    },
    {
      (Enum) ScenarioDbId.BOH_FAELIN_16,
      "PRESENCE_SCENARIO_BOH11_FIGHT_16"
    },
    {
      (Enum) ScenarioDbId.BOM_01_Rokara_01,
      "PRESENCE_SCENARIO_BOM_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOM_01_Rokara_02,
      "PRESENCE_SCENARIO_BOM_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOM_01_Rokara_03,
      "PRESENCE_SCENARIO_BOM_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOM_01_Rokara_04,
      "PRESENCE_SCENARIO_BOM_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOM_01_Rokara_05,
      "PRESENCE_SCENARIO_BOM_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOM_01_Rokara_06,
      "PRESENCE_SCENARIO_BOM_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOM_01_Rokara_07,
      "PRESENCE_SCENARIO_BOM_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOM_01_Rokara_08,
      "PRESENCE_SCENARIO_BOM_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOM_02_Xyrella_01,
      "PRESENCE_SCENARIO_BOM2_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOM_02_Xyrella_02,
      "PRESENCE_SCENARIO_BOM2_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOM_02_Xyrella_03,
      "PRESENCE_SCENARIO_BOM2_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOM_02_Xyrella_04,
      "PRESENCE_SCENARIO_BOM2_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOM_02_Xyrella_05,
      "PRESENCE_SCENARIO_BOM2_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOM_02_Xyrella_06,
      "PRESENCE_SCENARIO_BOM2_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOM_02_Xyrella_07,
      "PRESENCE_SCENARIO_BOM2_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOM_02_Xyrella_08,
      "PRESENCE_SCENARIO_BOM2_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOM_03_Guff_01,
      "PRESENCE_SCENARIO_BOM3_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOM_03_Guff_02,
      "PRESENCE_SCENARIO_BOM3_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOM_03_Guff_03,
      "PRESENCE_SCENARIO_BOM3_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOM_03_Guff_04,
      "PRESENCE_SCENARIO_BOM3_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOM_03_Guff_05,
      "PRESENCE_SCENARIO_BOM3_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOM_03_Guff_06,
      "PRESENCE_SCENARIO_BOM3_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOM_03_Guff_07,
      "PRESENCE_SCENARIO_BOM3_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOM_03_Guff_08,
      "PRESENCE_SCENARIO_BOM3_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOM_04_Kurtrus_01,
      "PRESENCE_SCENARIO_BOM4_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOM_04_Kurtrus_02,
      "PRESENCE_SCENARIO_BOM4_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOM_04_Kurtrus_03,
      "PRESENCE_SCENARIO_BOM4_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOM_04_Kurtrus_04,
      "PRESENCE_SCENARIO_BOM4_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOM_04_Kurtrus_05,
      "PRESENCE_SCENARIO_BOM4_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOM_04_Kurtrus_06,
      "PRESENCE_SCENARIO_BOM4_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOM_04_Kurtrus_07,
      "PRESENCE_SCENARIO_BOM4_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOM_04_Kurtrus_08,
      "PRESENCE_SCENARIO_BOM4_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOM_05_Tamsin_001,
      "PRESENCE_SCENARIO_BOM5_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOM_05_Tamsin_002,
      "PRESENCE_SCENARIO_BOM5_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOM_05_Tamsin_003,
      "PRESENCE_SCENARIO_BOM5_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOM_05_Tamsin_004,
      "PRESENCE_SCENARIO_BOM5_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOM_05_Tamsin_005,
      "PRESENCE_SCENARIO_BOM5_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOM_05_Tamsin_006,
      "PRESENCE_SCENARIO_BOM5_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOM_05_Tamsin_007,
      "PRESENCE_SCENARIO_BOM5_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOM_05_Tamsin_008,
      "PRESENCE_SCENARIO_BOM5_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.BOM_06_Cariel_001,
      "PRESENCE_SCENARIO_BOM6_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.BOM_06_Cariel_002,
      "PRESENCE_SCENARIO_BOM6_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.BOM_06_Cariel_003,
      "PRESENCE_SCENARIO_BOM6_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.BOM_06_Cariel_004,
      "PRESENCE_SCENARIO_BOM6_FIGHT_04"
    },
    {
      (Enum) ScenarioDbId.BOM_06_Cariel_005,
      "PRESENCE_SCENARIO_BOM6_FIGHT_05"
    },
    {
      (Enum) ScenarioDbId.BOM_06_Cariel_006,
      "PRESENCE_SCENARIO_BOM6_FIGHT_06"
    },
    {
      (Enum) ScenarioDbId.BOM_06_Cariel_007,
      "PRESENCE_SCENARIO_BOM6_FIGHT_07"
    },
    {
      (Enum) ScenarioDbId.BOM_06_Cariel_008,
      "PRESENCE_SCENARIO_BOM6_FIGHT_08"
    },
    {
      (Enum) ScenarioDbId.RLK_PROLOGUE_01,
      "PRESENCE_SCENARIO_RLK_FIGHT_01"
    },
    {
      (Enum) ScenarioDbId.RLK_PROLOGUE_02,
      "PRESENCE_SCENARIO_RLK_FIGHT_02"
    },
    {
      (Enum) ScenarioDbId.RLK_PROLOGUE_03,
      "PRESENCE_SCENARIO_RLK_FIGHT_03"
    },
    {
      (Enum) ScenarioDbId.RLK_PROLOGUE_04,
      "PRESENCE_SCENARIO_RLK_FIGHT_04"
    }
  };
  private static readonly Map<KeyValuePair<AdventureDbId, AdventureModeDbId>, PresenceMgr.PresenceTargets> s_adventurePresenceMap = new Map<KeyValuePair<AdventureDbId, AdventureModeDbId>, PresenceMgr.PresenceTargets>()
  {
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.NAXXRAMAS, AdventureModeDbId.LINEAR),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.NAXX_NORMAL, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_NORMAL)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.NAXXRAMAS, AdventureModeDbId.LINEAR_HEROIC),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.NAXX_HEROIC, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_HEROIC)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.NAXXRAMAS, AdventureModeDbId.CLASS_CHALLENGE),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.NAXX_CLASS_CHALLENGE, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_CLASS_CHALLENGE)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BRM, AdventureModeDbId.LINEAR),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.BRM_NORMAL, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_NORMAL)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BRM, AdventureModeDbId.LINEAR_HEROIC),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.BRM_HEROIC, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_HEROIC)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BRM, AdventureModeDbId.CLASS_CHALLENGE),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.BRM_CLASS_CHALLENGE, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_CLASS_CHALLENGE)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.LOE, AdventureModeDbId.LINEAR),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.LOE_NORMAL, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_NORMAL)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.LOE, AdventureModeDbId.LINEAR_HEROIC),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.LOE_HEROIC, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_HEROIC)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.LOE, AdventureModeDbId.CLASS_CHALLENGE),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.LOE_CLASS_CHALLENGE, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_CLASS_CHALLENGE)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.KARA, AdventureModeDbId.LINEAR),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.KAR_NORMAL, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_KAR_NORMAL)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.KARA, AdventureModeDbId.LINEAR_HEROIC),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.KAR_HEROIC, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_KAR_HEROIC)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.KARA, AdventureModeDbId.CLASS_CHALLENGE),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.KAR_CLASS_CHALLENGE, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_KAR_CLASS_CHALLENGE)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.ICC, AdventureModeDbId.LINEAR),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.ICC_NORMAL, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_ICC_NORMAL)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.LOOT, AdventureModeDbId.DUNGEON_CRAWL),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.LOOT, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_LOOT)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.GIL, AdventureModeDbId.DUNGEON_CRAWL),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.GIL, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_GIL)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.GIL, AdventureModeDbId.BONUS_CHALLENGE),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.GIL, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_GIL_BONUS_CHALLENGE)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BOT, AdventureModeDbId.LINEAR),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.BOT, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BOT)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.TRL, AdventureModeDbId.DUNGEON_CRAWL),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.TRL, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_TRL)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.DALARAN, AdventureModeDbId.DUNGEON_CRAWL),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.DAL, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_DAL)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.DALARAN, AdventureModeDbId.DUNGEON_CRAWL_HEROIC),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.DAL, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_DAL_HEROIC)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.ULDUM, AdventureModeDbId.DUNGEON_CRAWL),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.ULD, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_ULD)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.ULDUM, AdventureModeDbId.DUNGEON_CRAWL_HEROIC),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.ULD, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_ULD_HEROIC)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.DRAGONS, AdventureModeDbId.LINEAR),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.DRG, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_DRG)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.DRAGONS, AdventureModeDbId.LINEAR_HEROIC),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.DRG, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_DRG_HEROIC)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BTP, AdventureModeDbId.LINEAR),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.BTP, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BTP)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BTA, AdventureModeDbId.LINEAR),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.BTA, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BTA)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BTA_HEROIC, AdventureModeDbId.LINEAR_HEROIC),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.BTA_HEROIC, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BTA_HEROIC)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BOH, AdventureModeDbId.DUNGEON_CRAWL),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.BOH, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BOH)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BOM, AdventureModeDbId.DUNGEON_CRAWL),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.BOM, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_BOM)
    },
    {
      new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.ROTLK, AdventureModeDbId.LINEAR),
      new PresenceMgr.PresenceTargets(PresenceAdventureMode.RLK, Global.PresenceStatus.SPECTATING_GAME_ADVENTURE_RLK)
    }
  };
  private static readonly System.Type[] s_enumIdList = new System.Type[4]
  {
    typeof (Global.PresenceStatus),
    typeof (PresenceTutorial),
    typeof (PresenceAdventureMode),
    typeof (ScenarioDbId)
  };
  private static PresenceMgr s_instance;
  private Map<System.Type, byte> m_enumToIdMap = new Map<System.Type, byte>();
  private Map<byte, System.Type> m_idToEnumMap = new Map<byte, System.Type>();
  private Enum[] m_prevStatus;
  private Enum[] m_status;
  private long m_timeStartStatusMs;
  private Blizzard.Telemetry.WTCG.Client.PresenceStatus m_currentStatus;
  private Enum[] m_richPresence;

  public static PresenceMgr Get()
  {
    if (PresenceMgr.s_instance == null)
    {
      PresenceMgr.s_instance = new PresenceMgr();
      PresenceMgr.s_instance.Initialize();
    }
    return PresenceMgr.s_instance;
  }

  public static bool IsInitialized() => PresenceMgr.s_instance != null;

  public bool SetStatus(params Enum[] args) => this.SetStatusImpl(args);

  public bool SetStatus_EnteringAdventure(
    AdventureDbId adventureId,
    AdventureModeDbId adventureModeId)
  {
    KeyValuePair<AdventureDbId, AdventureModeDbId> key = new KeyValuePair<AdventureDbId, AdventureModeDbId>(adventureId, adventureModeId);
    PresenceMgr.PresenceTargets presenceTargets;
    if (!PresenceMgr.s_adventurePresenceMap.TryGetValue(key, out presenceTargets))
      return false;
    this.SetStatus((Enum) Global.PresenceStatus.ADVENTURE_SCENARIO_SELECT, (Enum) presenceTargets.EnteringAdventureValue);
    return true;
  }

  public bool SetStatus_PlayingMission(ScenarioDbId missionId)
  {
    if (!PresenceMgr.s_stringKeyMap.ContainsKey((Enum) missionId))
      return false;
    return this.SetStatus((Enum) Global.PresenceStatus.ADVENTURE_SCENARIO_PLAYING_GAME, (Enum) missionId);
  }

  public bool SetStatus_SpectatingMission(ScenarioDbId missionId)
  {
    KeyValuePair<AdventureDbId, AdventureModeDbId> key = new KeyValuePair<AdventureDbId, AdventureModeDbId>(GameUtils.GetAdventureId((int) missionId), GameUtils.GetAdventureModeId((int) missionId));
    PresenceMgr.PresenceTargets presenceTargets;
    if (!PresenceMgr.s_adventurePresenceMap.TryGetValue(key, out presenceTargets))
      return false;
    return this.SetStatus((Enum) presenceTargets.SpectatingValue);
  }

  public Enum[] GetStatus() => this.m_status;

  public Global.PresenceStatus CurrentStatus => this.m_status != null && this.m_status.Length != 0 ? (Global.PresenceStatus) this.m_status[0] : Global.PresenceStatus.UNKNOWN;

  public bool SetPrevStatus() => this.SetStatusImpl(this.m_prevStatus);

  public string GetStatusText(BnetPlayer player)
  {
    List<string> stringArgs = new List<string>();
    string statusKey = (string) null;
    if (this.GetStatus_Internal(player, ref statusKey, stringArgs) == Global.PresenceStatus.UNKNOWN || !BnetPresenceMgr.Get().IsSubscribedToPlayer(player.GetHearthstoneGameAccountId()))
    {
      BnetGameAccount bestGameAccount = player.GetBestGameAccount();
      return !(bestGameAccount == (BnetGameAccount) null) ? bestGameAccount.GetRichPresence() : (string) null;
    }
    string[] array = stringArgs.ToArray();
    try
    {
      return GameStrings.Format(statusKey, (object[]) array);
    }
    catch (FormatException ex)
    {
      Log.Presence.PrintWarning("PresenceMgr.GetStatusText: Arguments were expected for presence string, but none were provided.");
      return GameStrings.Get("GLOBAL_PROGRAMNAME_HEARTHSTONE");
    }
  }

  public string GetStatusText(byte[] presenceFieldBlobValue)
  {
    List<string> stringArgs = new List<string>();
    string statusKey = (string) null;
    int statusInternal = (int) this.GetStatus_Internal(presenceFieldBlobValue, ref statusKey, stringArgs);
    string[] array = stringArgs.ToArray();
    try
    {
      return GameStrings.Format(statusKey, (object[]) array);
    }
    catch (FormatException ex)
    {
      Log.Presence.PrintWarning("PresenceMgr.GetStatusText: Arguments were expected for presence string, but none were provided.");
      return GameStrings.Get("GLOBAL_PROGRAMNAME_HEARTHSTONE");
    }
  }

  public Global.PresenceStatus GetStatus(BnetPlayer player)
  {
    string statusKey = (string) null;
    return this.GetStatus_Internal(player, ref statusKey);
  }

  public Enum[] GetStatusEnums(BnetPlayer player)
  {
    string statusKey = (string) null;
    List<Enum> enumVals = new List<Enum>();
    int statusInternal = (int) this.GetStatus_Internal(player, ref statusKey, enumVals: enumVals);
    return enumVals.ToArray();
  }

  public void OnShutdown() => this.ReportPresenceToTelemetry(new Enum[1]
  {
    (Enum) Global.PresenceStatus.UNKNOWN
  });

  public void ResetTelemetry()
  {
    this.ReportPresenceToTelemetry(new Enum[1]
    {
      (Enum) Global.PresenceStatus.UNKNOWN
    });
    this.m_currentStatus = (Blizzard.Telemetry.WTCG.Client.PresenceStatus) null;
    this.m_timeStartStatusMs = (long) ((double) Time.realtimeSinceStartup * 1000.0);
  }

  private Global.PresenceStatus GetStatus_Internal(
    BnetPlayer player,
    ref string statusKey,
    List<string> stringArgs = null,
    List<Enum> enumVals = null)
  {
    Global.PresenceStatus statusInternal = Global.PresenceStatus.UNKNOWN;
    if (player == null || player.GetBestGameAccount() == (BnetGameAccount) null)
      return statusInternal;
    BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
    byte[] val;
    return hearthstoneGameAccount == (BnetGameAccount) null || !hearthstoneGameAccount.TryGetGameFieldBytes(17U, out val) ? statusInternal : this.GetStatus_Internal(val, ref statusKey, stringArgs, enumVals);
  }

  private Global.PresenceStatus GetStatus_Internal(
    byte[] bytes,
    ref string statusKey,
    List<string> stringArgs = null,
    List<Enum> enumVals = null)
  {
    Global.PresenceStatus statusInternal = Global.PresenceStatus.UNKNOWN;
    if (bytes == null)
      return statusInternal;
    Enum enumVal = (Enum) null;
    using (MemoryStream input = new MemoryStream(bytes))
    {
      using (BinaryReader reader = new BinaryReader((Stream) input))
      {
        if (!this.DecodeStatusVal(reader, ref enumVal, ref statusKey))
          return statusInternal;
        statusInternal = (Global.PresenceStatus) enumVal;
        enumVals?.Add((Enum) statusInternal);
        if (stringArgs == null)
        {
          if (enumVals == null)
            goto label_23;
        }
        while (input.Position < (long) bytes.Length)
        {
          string key = (string) null;
          if (!this.DecodeStatusVal(reader, ref enumVal, ref key))
            return statusInternal;
          enumVals?.Add(enumVal);
          if (stringArgs != null)
          {
            string str = GameStrings.Get(key);
            stringArgs.Add(str);
          }
        }
      }
    }
label_23:
    return statusInternal;
  }

  private void Initialize()
  {
    for (int index = 0; index < PresenceMgr.s_enumIdList.Length; ++index)
    {
      System.Type enumId = PresenceMgr.s_enumIdList[index];
      if (Enum.GetUnderlyingType(enumId) != typeof (int))
        throw new Exception(string.Format("Underlying type of enum {0} (underlying={1}) must {2} be to used by Presence system.", (object) enumId.FullName, (object) Enum.GetUnderlyingType(enumId).FullName, (object) typeof (int).Name));
      byte key = (byte) (index + 1);
      this.m_enumToIdMap.Add(enumId, key);
      this.m_idToEnumMap.Add(key, enumId);
    }
  }

  private bool SetStatusImpl(Enum[] status)
  {
    if (!Network.ShouldBeConnectedToAurora())
      return false;
    if (!Network.IsLoggedIn())
      return true;
    if (status == null || status.Length == 0)
    {
      Error.AddDevFatal("PresenceMgr.SetStatusImpl() - Received status of length 0. Setting empty status is not supported.");
      return false;
    }
    if (GeneralUtils.AreArraysEqual<Enum>(this.m_status, status))
      return true;
    if (!this.SetRichPresence(status) || !this.SetGamePresence(status))
      return false;
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject != null && netObject.SendTelemetryPresence)
      this.ReportPresenceToTelemetry(status);
    this.m_prevStatus = this.m_status;
    int num = this.m_prevStatus == null ? 1 : (this.m_prevStatus.Length == 0 ? 1 : 0);
    this.m_status = new Enum[status.Length];
    Array.Copy((Array) status, (Array) this.m_status, status.Length);
    if (num != 0 || !PresenceMgr.IsStatusPlayingGame((Global.PresenceStatus) status[0]))
      SpectatorManager.Get().UpdateMySpectatorInfo();
    return true;
  }

  private void ReportPresenceToTelemetry(Enum[] status)
  {
    if (status.Length == 0)
      return;
    long num = (long) ((double) Time.realtimeSinceStartup * 1000.0);
    long millisecondsSincePrev = num - this.m_timeStartStatusMs;
    this.m_timeStartStatusMs = num;
    Blizzard.Telemetry.WTCG.Client.PresenceStatus newPresenceStatus = new Blizzard.Telemetry.WTCG.Client.PresenceStatus()
    {
      PresenceId = (long) (Global.PresenceStatus) status[0]
    };
    if (status.Length > 1)
      newPresenceStatus.PresenceSubId = Convert.ToInt64((object) ((IEnumerable<Enum>) status).Skip<Enum>(1).FirstOrDefault<Enum>());
    TelemetryManager.Client().SendPresenceChanged(newPresenceStatus, this.m_currentStatus, millisecondsSincePrev);
    this.m_timeStartStatusMs = num;
    this.m_currentStatus = newPresenceStatus;
  }

  private bool SetRichPresence(Enum[] status)
  {
    Enum[] enumArray = new Enum[status.Length];
    for (int index = 0; index < status.Length; ++index)
    {
      Enum statu = status[index];
      Enum @enum;
      if (PresenceMgr.s_richPresenceMap.TryGetValue(statu, out @enum))
      {
        if (@enum == null)
          return false;
      }
      else
        @enum = statu;
      enumArray[index] = @enum;
    }
    if (((IEnumerable<Enum>) enumArray).Any<Enum>((Func<Enum, bool>) (e => !RichPresence.s_streamIds.ContainsKey(e.GetType()))))
      enumArray = new Enum[1]{ enumArray[0] };
    if (GeneralUtils.AreArraysEqual<Enum>(this.m_richPresence, enumArray))
      return true;
    this.m_richPresence = enumArray;
    if (!Network.ShouldBeConnectedToAurora())
    {
      Error.AddDevFatal(string.Format("Caller should check for Battle.net connection before calling SetRichPresence {0}", enumArray == null ? (object) "" : (object) string.Join(", ", ((IEnumerable<Enum>) enumArray).Select<Enum, string>((Func<Enum, string>) (x => x.ToString())).ToArray<string>())));
      return false;
    }
    if (enumArray == null || enumArray.Length == 0)
      return false;
    RichPresenceUpdate[] updates = new RichPresenceUpdate[enumArray.Length];
    for (int index = 0; index < enumArray.Length; ++index)
    {
      Enum @enum = enumArray[index];
      System.Type type = @enum.GetType();
      FourCC streamId = RichPresence.s_streamIds[type];
      updates[index] = new RichPresenceUpdate()
      {
        presenceFieldIndex = index == 0 ? 0UL : (ulong) (uint) (458752 + index),
        programId = BnetProgramId.HEARTHSTONE.GetValue(),
        streamId = streamId.GetValue(),
        index = Convert.ToUInt32((object) @enum)
      };
    }
    BattleNet.SetRichPresence(updates);
    return true;
  }

  private bool SetGamePresence(Enum[] status)
  {
    using (MemoryStream output = new MemoryStream())
    {
      using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
      {
        for (int index = 0; index < status.Length; ++index)
        {
          byte id;
          int intVal;
          if (!this.EncodeStatusVal(status, index, out id, out intVal))
            return false;
          binaryWriter.Write(id);
          binaryWriter.Write(intVal);
        }
        byte[] buffer = output.GetBuffer();
        byte[] val = new byte[output.Position];
        byte[] destinationArray = val;
        int length = val.Length;
        Array.Copy((Array) buffer, (Array) destinationArray, length);
        return BnetPresenceMgr.Get().SetGameField(17U, val);
      }
    }
  }

  private bool EncodeStatusVal(Enum[] status, int index, out byte id, out int intVal)
  {
    Enum statu = status[index];
    System.Type type = statu.GetType();
    intVal = Convert.ToInt32((object) statu);
    if (this.m_enumToIdMap.TryGetValue(type, out id))
      return true;
    Error.AddDevFatal("PresenceMgr.EncodeStatusVal() - {0} at index {1} belongs to type {2}, which has no id", (object) statu, (object) index, (object) type);
    return false;
  }

  private bool DecodeStatusVal(BinaryReader reader, ref Enum enumVal, ref string key)
  {
    key = (string) null;
    byte key1 = 0;
    int position1 = (int) reader.BaseStream.Position;
    int num1 = position1 + 1;
    int position2;
    try
    {
      key1 = reader.ReadByte();
      position2 = (int) reader.BaseStream.Position;
    }
    catch (Exception ex)
    {
      Log.Presence.Print("PresenceMgr.DecodeStatusVal - unable to decode enum id {0} at index {1} : {2} {3}", (object) key1, (object) position1, (object) ex.GetType().FullName, (object) ex.Message);
      return false;
    }
    System.Type type;
    if (!this.m_idToEnumMap.TryGetValue(key1, out type))
    {
      Log.Presence.Print("PresenceMgr.DecodeStatusVal - id {0} at index {1}, has no enum type", (object) key1, (object) position1);
      return false;
    }
    int num2;
    try
    {
      num2 = reader.ReadInt32();
    }
    catch (Exception ex)
    {
      Log.Presence.Print("PresenceMgr.DecodeStatusVal - unable to decode enum value {0} at index {1} : {2} {3}", (object) key1, (object) position2, (object) ex.GetType().FullName, (object) ex.Message);
      return false;
    }
    if (type == typeof (Global.PresenceStatus))
    {
      Global.PresenceStatus key2 = (Global.PresenceStatus) num2;
      enumVal = (Enum) key2;
      if (!PresenceMgr.s_stringKeyMap.TryGetValue((Enum) key2, out key))
      {
        Log.Presence.Print("PresenceMgr.DecodeStatusVal - value {0}.{1} at index {2}, has no string", (object) type, (object) key2, (object) position2);
        return false;
      }
    }
    else if (type == typeof (PresenceTutorial))
    {
      PresenceTutorial key3 = (PresenceTutorial) num2;
      enumVal = (Enum) key3;
      if (!PresenceMgr.s_stringKeyMap.TryGetValue((Enum) key3, out key))
      {
        Log.Presence.Print("PresenceMgr.DecodeStatusVal - value {0}.{1} at index {2}, has no string", (object) type, (object) key3, (object) position2);
        return false;
      }
    }
    else if (type == typeof (PresenceAdventureMode))
    {
      PresenceAdventureMode key4 = (PresenceAdventureMode) num2;
      enumVal = (Enum) key4;
      if (!PresenceMgr.s_stringKeyMap.TryGetValue((Enum) key4, out key))
      {
        Log.Presence.Print("PresenceMgr.DecodeStatusVal - value {0}.{1} at index {2}, has no string", (object) type, (object) key4, (object) position2);
        return false;
      }
    }
    else if (type == typeof (ScenarioDbId))
    {
      ScenarioDbId key5 = (ScenarioDbId) num2;
      enumVal = (Enum) key5;
      if (!PresenceMgr.s_stringKeyMap.TryGetValue((Enum) key5, out key))
      {
        Log.Presence.Print("PresenceMgr.DecodeStatusVal - value {0}.{1} at index {2}, has no string", (object) type, (object) key5, (object) position2);
        return false;
      }
    }
    return true;
  }

  public static bool IsStatusPlayingGame(Global.PresenceStatus status)
  {
    switch (status)
    {
      case Global.PresenceStatus.TUTORIAL_GAME:
      case Global.PresenceStatus.PLAY_GAME:
      case Global.PresenceStatus.PRACTICE_GAME:
      case Global.PresenceStatus.ARENA_GAME:
      case Global.PresenceStatus.FRIENDLY_GAME:
      case Global.PresenceStatus.ADVENTURE_SCENARIO_PLAYING_GAME:
      case Global.PresenceStatus.TAVERN_BRAWL_GAME:
      case Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_GAME:
      case Global.PresenceStatus.WAIT_FOR_OPPONENT_RECONNECT:
      case Global.PresenceStatus.BATTLEGROUNDS_GAME:
      case Global.PresenceStatus.DUELS_GAME:
      case Global.PresenceStatus.PLAY_RANKED_STANDARD:
      case Global.PresenceStatus.PLAY_RANKED_WILD:
      case Global.PresenceStatus.PLAY_RANKED_CLASSIC:
      case Global.PresenceStatus.PLAY_CASUAL_STANDARD:
      case Global.PresenceStatus.PLAY_CASUAL_WILD:
      case Global.PresenceStatus.PLAY_CASUAL_CLASSIC:
      case Global.PresenceStatus.MERCENARIES_GAME:
      case Global.PresenceStatus.MERCENARIES_FRIENDLY_GAME:
        return true;
      default:
        return false;
    }
  }

  private struct PresenceTargets
  {
    public PresenceAdventureMode EnteringAdventureValue;
    public Global.PresenceStatus SpectatingValue;

    public PresenceTargets(
      PresenceAdventureMode enteringAdventureValue,
      Global.PresenceStatus spectatingValue)
    {
      this.EnteringAdventureValue = enteringAdventureValue;
      this.SpectatingValue = spectatingValue;
    }
  }
}
