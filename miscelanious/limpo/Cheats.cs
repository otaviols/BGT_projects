using Assets;
using Blizzard.BlizzardErrorMobile;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Time;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Jobs;
using Blizzard.T5.Logging;
using Blizzard.T5.MaterialService;
using Blizzard.T5.Services;
using CSharpZombieDetector;
using Hearthstone;
using Hearthstone.APIGateway;
using Hearthstone.Attribution;
using Hearthstone.Commerce;
using Hearthstone.Core;
using Hearthstone.CRM;
using Hearthstone.DataModels;
using Hearthstone.Http;
using Hearthstone.InGameMessage;
using Hearthstone.InGameMessage.UI;
using Hearthstone.Login;
using Hearthstone.Progression;
using Hearthstone.Streaming;
using Hearthstone.UI;
using Hearthstone.Util;
using MiniJSON;
using PegasusGame;
using PegasusLettuce;
using PegasusShared;
using PegasusUtil;
using SpectatorProto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cheats : IService
{
  public readonly Vector3 SPEECH_BUBBLE_HIDDEN_POSITION = new Vector3(15000f, 0.0f, 0.0f);
  private static Cheats s_instance;
  private bool m_isInGameplayScene;
  private int m_boardId;
  private string m_playerTags;
  private bool m_speechBubblesEnabled = true;
  private bool m_cardTextEnabled = true;
  private bool m_cardNamesEnabled = true;
  private bool m_cardRaceTextEnabled = true;
  private bool m_playerNamesEnabled = true;
  private bool m_battlegroundHeroBuddyEnabled = true;
  private Map<Global.SoundCategory, bool> m_audioChannelEnabled = Cheats.InitAudioChannelMap();
  private System.Collections.Generic.Queue<int> m_pvpdrTreasureIds = new System.Collections.Generic.Queue<int>();
  private System.Collections.Generic.Queue<int> m_pvpdrLootIds = new System.Collections.Generic.Queue<int>();
  private Map<string, List<Global.SoundCategory>> m_audioChannelGroups = new Map<string, List<Global.SoundCategory>>()
  {
    {
      "VO",
      new List<Global.SoundCategory>()
      {
        Global.SoundCategory.VO,
        Global.SoundCategory.SPECIAL_VO,
        Global.SoundCategory.BOSS_VO,
        Global.SoundCategory.TRIGGER_VO
      }
    },
    {
      "MUSIC",
      new List<Global.SoundCategory>()
      {
        Global.SoundCategory.MUSIC,
        Global.SoundCategory.SPECIAL_MUSIC,
        Global.SoundCategory.HERO_MUSIC
      }
    },
    {
      "FX",
      new List<Global.SoundCategory>()
      {
        Global.SoundCategory.FX,
        Global.SoundCategory.NONE,
        Global.SoundCategory.SPECIAL_CARD
      }
    },
    {
      "BACKGROUND",
      new List<Global.SoundCategory>()
      {
        Global.SoundCategory.AMBIENCE,
        Global.SoundCategory.RESET_GAME
      }
    }
  };
  private bool m_loadingStoreChallengePrompt;
  private StoreChallengePrompt m_storeChallengePrompt;
  private bool m_isNewCardInPackOpeningEnabled;
  private AlertPopup m_alert;
  private static readonly Map<KeyCode, ScenarioDbId> s_quickPlayKeyMap = new Map<KeyCode, ScenarioDbId>()
  {
    {
      KeyCode.F1,
      ScenarioDbId.PRACTICE_EXPERT_MAGE
    },
    {
      KeyCode.F2,
      ScenarioDbId.PRACTICE_EXPERT_HUNTER
    },
    {
      KeyCode.F3,
      ScenarioDbId.PRACTICE_EXPERT_WARRIOR
    },
    {
      KeyCode.F4,
      ScenarioDbId.PRACTICE_EXPERT_SHAMAN
    },
    {
      KeyCode.F5,
      ScenarioDbId.PRACTICE_EXPERT_DRUID
    },
    {
      KeyCode.F6,
      ScenarioDbId.PRACTICE_EXPERT_PRIEST
    },
    {
      KeyCode.F7,
      ScenarioDbId.PRACTICE_EXPERT_ROGUE
    },
    {
      KeyCode.F8,
      ScenarioDbId.PRACTICE_EXPERT_PALADIN
    },
    {
      KeyCode.F9,
      ScenarioDbId.PRACTICE_EXPERT_WARLOCK
    },
    {
      KeyCode.F10,
      ScenarioDbId.PRACTICE_EXPERT_DEMONHUNTER
    },
    {
      KeyCode.F11,
      ScenarioDbId.PRACTICE_EXPERT_DEATHKNIGHT
    },
    {
      KeyCode.T,
      ScenarioDbId.TEST_BLANK_STATE
    },
    {
      KeyCode.M,
      ScenarioDbId.LETTUCE_DEV_TEST_VS_AI
    },
    {
      KeyCode.B,
      ScenarioDbId.TB_BACONSHOP_VS_AI
    }
  };
  private static readonly Map<ScenarioDbId, GameType> s_scenarioToGameTypeMap = new Map<ScenarioDbId, GameType>()
  {
    {
      ScenarioDbId.TB_BACONSHOP_VS_AI,
      GameType.GT_BATTLEGROUNDS_PLAYER_VS_AI
    }
  };
  private static readonly List<ScenarioDbId> s_quickPlayNotSkipMulligan = new List<ScenarioDbId>()
  {
    ScenarioDbId.TB_BACONSHOP_VS_AI
  };
  private static readonly Map<KeyCode, string> s_opponentHeroKeyMap = new Map<KeyCode, string>()
  {
    {
      KeyCode.F1,
      "HERO_08"
    },
    {
      KeyCode.F2,
      "HERO_05"
    },
    {
      KeyCode.F3,
      "HERO_01"
    },
    {
      KeyCode.F4,
      "HERO_02"
    },
    {
      KeyCode.F5,
      "HERO_06"
    },
    {
      KeyCode.F6,
      "HERO_09"
    },
    {
      KeyCode.F7,
      "HERO_03"
    },
    {
      KeyCode.F8,
      "HERO_04"
    },
    {
      KeyCode.F9,
      "HERO_07"
    },
    {
      KeyCode.F10,
      "HERO_10"
    },
    {
      KeyCode.F11,
      "HERO_11"
    },
    {
      KeyCode.T,
      "HERO_01"
    },
    {
      KeyCode.M,
      string.Empty
    },
    {
      KeyCode.B,
      "TB_BaconShop_HERO_PH"
    }
  };
  private Cheats.QuickLaunchState m_quickLaunchState = new Cheats.QuickLaunchState();
  private bool m_skipSendingGetGameState;
  public static float VOChanceOverride = -1f;
  private ProfilerMarker m_CacheProfilerMarker = new ProfilerMarker("SpellManager.Spawn50.Cached()");
  private ProfilerMarker m_AssetLoaderProfilerMarker = new ProfilerMarker("SpellManager.Spawn50.AssetLoader()");
  private ProfilerMarker m_CacheProfilerMarker1 = new ProfilerMarker("SpellManager.Spawn1.Cached()");
  private ProfilerMarker m_AssetLoaderProfilerMarker1 = new ProfilerMarker("SpellManager.Spawn1.AssetLoader()");
  private float m_waitTime = 10f;
  private bool m_showedMessage;
  private static readonly ChangeMessageItemInformation[] m_changeMessageCardsExamples = new ChangeMessageItemInformation[5]
  {
    new ChangeMessageItemInformation()
    {
      ItemType = InGameMessageItemDisplayContent.ItemType.Card,
      ItemId = "BAR_024"
    },
    new ChangeMessageItemInformation()
    {
      ItemType = InGameMessageItemDisplayContent.ItemType.Card,
      ItemId = "BAR_745"
    },
    new ChangeMessageItemInformation()
    {
      ItemType = InGameMessageItemDisplayContent.ItemType.Card,
      ItemId = "BAR_327"
    },
    new ChangeMessageItemInformation()
    {
      ItemType = InGameMessageItemDisplayContent.ItemType.Card,
      ItemId = "BAR_082"
    },
    new ChangeMessageItemInformation()
    {
      ItemType = InGameMessageItemDisplayContent.ItemType.Card,
      ItemId = "BAR_025"
    }
  };
  private static readonly ChangeMessageItemInformation[] m_changeMessageHeroExamples = new ChangeMessageItemInformation[1]
  {
    new ChangeMessageItemInformation()
    {
      ItemType = InGameMessageItemDisplayContent.ItemType.Hero,
      ItemId = "HERO_09a"
    }
  };
  private List<WidgetInstance> s_createdWidgets = new List<WidgetInstance>();
  private static bool s_hasSubscribedToPartyEvents = false;
  private string[] m_lastMercsServerCmd;
  private string[] m_lastUtilServerCmd;
  private static WidgetInstance exampleUI = (WidgetInstance) null;

  private static Map<Global.SoundCategory, bool> InitAudioChannelMap()
  {
    Map<Global.SoundCategory, bool> map = new Map<Global.SoundCategory, bool>();
    foreach (int key in System.Enum.GetValues(typeof (Global.SoundCategory)))
      map.Add((Global.SoundCategory) key, true);
    return map;
  }

  private IGameDownloadManager DownloadManager => GameDownloadManagerProvider.Get();

  public static bool ShowFakeBreakingNews => Vars.Key("Cheats.ShowFakeBreakingNews").GetBool(false);

  public static Cheats Get()
  {
    if (Cheats.s_instance == null)
      Cheats.s_instance = ServiceManager.Get<Cheats>();
    return Cheats.s_instance;
  }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    Cheats cheats = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    CheatMgr cheatMgr = serviceLocator.Get<CheatMgr>();
    if (HearthstoneApplication.IsInternal())
    {
      cheatMgr.RegisterCategory("help");
      cheatMgr.RegisterCheatHandler("help", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_help), "Get help for a specific command or list of commands", "<command name>", "");
      cheatMgr.RegisterCheatHandler("example", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_example), "Run an example of this command if one exists", "<command name>");
      cheatMgr.RegisterCheatHandler("error", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_error), "Make the client throw an arbitrary error.", "<warning | fatal | exception> <optional error message>", "warning This is an example warning message.");
      cheatMgr.RegisterCategory("bug");
      if (!RegionUtils.IsCNLegalRegion)
      {
        cheatMgr.RegisterCheatHandler("bug", new CheatMgr.ProcessCheatCallback(cheats.On_ProcessCheat_bug));
        cheatMgr.RegisterCheatHandler("Bug", new CheatMgr.ProcessCheatCallback(cheats.On_ProcessCheat_bug));
      }
      cheatMgr.RegisterCheatHandler("crash", new CheatMgr.ProcessCheatCallback(cheats.On_ProcessCheat_crash));
      cheatMgr.RegisterCheatHandler("anr", new CheatMgr.ProcessCheatCallback(cheats.On_ProcessCheat_ANR));
      cheatMgr.RegisterCategory("general");
      cheatMgr.RegisterCheatHandler("cheat", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_cheat), "Send a cheat command to the server", "<command> <arguments>");
      cheatMgr.RegisterCheatAlias("cheat", "c");
      cheatMgr.RegisterCheatHandler("timescale", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_timescale), "Cheat to change the timescale", "<timescale>", "0.5");
      cheatMgr.RegisterCheatHandler("util", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_utilservercmd), "Run a cheat on the UTIL server you're connected to.", "[subcommand] [subcommand args]", "help");
      cheatMgr.RegisterCheatHandler("game", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_gameservercmd), "[NYI] Run a cheat on the GAME server you're connected to.", "[subcommand] [subcommand args]", "help");
      Network.Get().RegisterNetHandler((object) DebugCommandResponse.PacketID.ID, new Network.NetHandler(cheats.OnProcessCheat_utilservercmd_OnResponse));
      cheatMgr.RegisterCheatHandler("event", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_EventTiming), "View event timings to see if they're active.", "[event=event_name]", "");
      cheatMgr.RegisterCheatHandler("audiochannel", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_audioChannel), "Turn on/off an audio channel.", "[audio channel name] [on/off]", "fx off");
      cheatMgr.RegisterCheatHandler("audiochannelgroup", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_audioChannelGroup), "Turn on/off a group of audio channels.", "[audio channel group name] [on/off]", "vo off");
      cheatMgr.RegisterCheatHandler("tracert", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_tracert));
      cheatMgr.RegisterCategory("igm");
      cheatMgr.RegisterCheatHandler("igm", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_igm), "Register the content type and show it by using the debug UI", "<content_type>");
      cheatMgr.RegisterCheatHandler("msgui", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_msgui), "Message popup ui", "<register|show> [<text|shop|launch|change>]");
      cheatMgr.RegisterCategory("program");
      cheatMgr.RegisterCheatHandler("reset", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_reset), "Reset the client");
      cheatMgr.RegisterCategory("gameplay");
      cheatMgr.RegisterCheatHandler("board", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_board), "Set which board will be loaded on the next game", "<BRM|STW|GVG>", "BRM");
      cheatMgr.RegisterCheatHandler("playertags", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_playerTags), "Set these tags on your player in the next game (limit 20)", "<TagId1=TagValue1,TagId2=TagValue2,...,TagIdN=TagValueN>", "427=10,419=1");
      cheatMgr.RegisterCheatHandler("togglespeechbubbles", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_speechBubbles), "Toggle on/off speech bubbles.", "", "");
      cheatMgr.RegisterCheatHandler("disconnect", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_disconnect), "Disconnects you from a game in progress (disconnects from game server only). If you want to disconnect from just battle.net, use 'disconnect bnet'.");
      cheatMgr.RegisterCheatHandler("restart", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_restart), "Restarts any non-PvP game.");
      cheatMgr.RegisterCheatHandler("autohand", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_autohand), "Set whether PhoneUI automatically hides your hand after playing a card", "<true/false>", "true");
      cheatMgr.RegisterCheatHandler("endturn", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_endturn), "End your turn");
      cheatMgr.RegisterCheatHandler("scenario", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_scenario), "Launch a scenario.", "<scenario_id> [<game_type_id>] [<deck_name>|<deck_id>] [<game_format>]");
      cheatMgr.RegisterCheatAlias("scenario", "mission");
      cheatMgr.RegisterCheatHandler("aigame", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_aigame), "Launch a game vs an AI using specified deck code.", "<deck_code_string> [<game_format>]");
      cheatMgr.RegisterCheatHandler("loadsnapshot", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_loadSnapshot), "Load a snapshot file from local disk.", "<replayfilename>");
      cheatMgr.RegisterCheatHandler("skipgetgamestate", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SkipSendingGetGameState), "Skip sending GetGameState packet in Gameplay.Start().");
      cheatMgr.RegisterCheatHandler("sendgetgamestate", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SendGetGameState), "Send GetGameState packet.");
      cheatMgr.RegisterCheatHandler("auto_exportgamestate", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_autoexportgamestate), "Save JSON file serializing some of GameState");
      cheatMgr.RegisterCheatHandler("opponentname", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_OpponentName), "Set the Opponent name", "", "The Innkeeper");
      cheatMgr.RegisterCheatHandler("history", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_History), "disable/enable history", "", "true");
      cheatMgr.RegisterCheatHandler("settag", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_settag), "Sets a tag on an entity to a value", "settag <tag_id> <entity_id> <tag_value>");
      cheatMgr.RegisterCheatHandler("thinkemotes", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_playAllThinkEmotes), "Plays all of the think lines for the specified player's hero");
      cheatMgr.RegisterCheatHandler("playemote", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_playEmote), "Play the emote for the specified player's hero", "playemote <emote_type> <player>");
      cheatMgr.RegisterCheatHandler("heropowervo", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_playAllMissionHeroPowerLines), "Plays all the hero power lines associated with this mission");
      cheatMgr.RegisterCheatHandler("idlevo", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_playAllMissionIdleLines), "Plays all idle lines associated with this mission");
      cheatMgr.RegisterCheatHandler("playbgguidevo", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_playBattlegroundsGuideVO), "Play a guide vo line");
      cheatMgr.RegisterCheatHandler("playbglegendaryherovfx", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_playLegendaryHeroVFX), "Play a legendary hero VFX");
      cheatMgr.RegisterCheatHandler("playbglegendaryherovo", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_playLegendaryHeroVO), "Play a legendary hero vo line");
      cheatMgr.RegisterCheatHandler("debugscript", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_debugscript), "Toggles script debugging for a specific power", "debugscript <power_guid>");
      cheatMgr.RegisterCheatHandler("scriptdebug", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_debugscript), "Toggles script debugging for a specific power", "scriptdebug <power_guid>");
      cheatMgr.RegisterCheatHandler("disablescriptdebug", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_disablescriptdebug), "Disables all script debugging on the server", "disablescriptdebug");
      cheatMgr.RegisterCheatHandler("disabledebugscript", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_disablescriptdebug), "Disables all script debugging on the server", "disabledebugscript");
      cheatMgr.RegisterCheatHandler("printpersistentlist", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_printpersistentlist), "Prints all persistent lists for a particular entity. Call it with no entity to print ALL persistent lists on ALL entities", "printpersistentlist [entity_id]");
      cheatMgr.RegisterCheatHandler("printpersistentlists", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_printpersistentlist), "Prints all persistent lists for a particular entity. Call it with no entity to print ALL persistent lists on ALL entities", "printpersistentlists [entity_id]");
      cheatMgr.RegisterCheatHandler("togglecardtext", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_togglecardtext), "Enables/Disables all card powers text", "togglecardtext");
      cheatMgr.RegisterCheatHandler("togglecardnames", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_togglecardnames), "Enables/Disables all card names", "togglecardnames");
      cheatMgr.RegisterCheatHandler("toggleracetext", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_toggleracetext), "Enables/Disables all race and spell school text", "toggleracetext");
      cheatMgr.RegisterCheatHandler("removeplayernames", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_removeplayernames), "Disables player name banners", "removeplayernames");
      cheatMgr.RegisterCategory("collection");
      cheatMgr.RegisterCheatHandler("collectionfirstxp", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_collectionfirstxp), "Set the number of page and cover flips to zero", "", "");
      cheatMgr.RegisterCheatHandler("resethasseencollectionmanager", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_HasSeenCollectionManager), "Resets Innkeeper tips for collection manager", "", "");
      cheatMgr.RegisterCheatHandler("cardchangereset", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_cardchangereset), "Reset the record of which changed cards have already been seen.", "<event_name>");
      cheatMgr.RegisterCheatHandler("loginpopupsequence", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_loginpopupsequence), "Show any active login popup sequences.");
      cheatMgr.RegisterCheatHandler("loginpopupreset", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_loginpopupreset), "Reset game save data for login popup sequences.");
      cheatMgr.RegisterCheatHandler("onlygold", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_onlygold), "In collection manager, do you want to see gold, nogold, or both?", "<command name>", "");
      cheatMgr.RegisterCheatHandler("exportcards", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_exportcards), "Export images of cards");
      cheatMgr.RegisterCategory("cosmetics");
      cheatMgr.RegisterCheatHandler("defaultcardback", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_favoritecardback), "Set your favorite cardback as if through the collection manager", "<cardback id>");
      cheatMgr.RegisterCheatHandler("favoritecardback", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_favoritecardback), "Set your favorite cardback as if through the collection manager", "<cardback id>");
      cheatMgr.RegisterCheatHandler("favoritehero", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_favoritehero), "Change your favorite hero for a class (only works from CollectionManager)", "<class_id> <hero_card_id> <hero_premium>");
      cheatMgr.RegisterCheatHandler("exportcardbacks", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_exportcardbacks), "Export images of card backs");
      cheatMgr.RegisterCheatHandler("finisher", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_PlayFinisher), "Requests a specific finisher to play.");
      cheatMgr.RegisterCategory("legacy quests and rewards");
      cheatMgr.RegisterCheatHandler("questcompletepopup", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_questcompletepopup), "Shows the quest complete achievement screen", "<quest_id>", "58");
      cheatMgr.RegisterCheatHandler("questprogresspopup", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_questprogresspopup), "Pop up a quest progress toast", "<title> <description> <progress> <maxprogress>", "Hello World 3 10");
      cheatMgr.RegisterCheatHandler("questwelcome", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_questwelcome), "Open list of daily quests", "<fromLogin>", "true");
      cheatMgr.RegisterCheatHandler("newquestvisual", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_newquestvisual), "Shows a new quest tile, only usable while a quest popup is active");
      cheatMgr.RegisterCheatHandler("fixedrewardcomplete", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_fixedrewardcomplete), "Shows the visual for a fixed reward", "<fixed_reward_map_id>");
      cheatMgr.RegisterCheatHandler("rewardboxes", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_rewardboxes), "Open the reward box screen with example rewards", "<card|cardback|gold|dust|random> <num_boxes>", "");
      cheatMgr.RegisterCategory("shop");
      cheatMgr.RegisterCheatHandler("storepassword", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_storepassword), "Show store challenge popup", "", "");
      cheatMgr.RegisterCheatHandler("testproduct", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_testproduct), "Fill Shop with a product", "<pmt_product_id>");
      cheatMgr.RegisterCheatHandler("testproducttag", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_testproducttag), "Fill Shop with products matching a tag", "<tag name>");
      cheatMgr.RegisterCheatHandler("testadventurestore", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_testadventurestore), "Open adventure store for a wing", "<wing_id> <is_full_adventure>");
      cheatMgr.RegisterCheatHandler("refreshcurrency", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_refreshcurrency), "Refresh currency balance", "<runestones|arcane_orbs>");
      cheatMgr.RegisterCheatHandler("loadpersonalizedshop", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_loadpersonalizedshop), "Load personalized shop", "<page_id>");
      cheatMgr.RegisterCheatHandler("mercpackgrantdiamondcard", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_mercpackgrantdiamondcard), "Grant a specific diamond card in a merc pack", "<merc_id> <art_variant_id>");
      cheatMgr.RegisterCheatHandler("mercpackduplicate", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_mercpackduplicate), "Force a specific merc duplicate in a merc pack", "<merc_id> <amount>");
      cheatMgr.RegisterCheatHandler("mercpackforcemercskin", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_mercpackforcemercskin), "Force a specific merc skin to appear, regardless if it is owned", "<merc_id> <art_variant_id> <premium_type>");
      cheatMgr.RegisterCategory("iks");
      cheatMgr.RegisterCheatHandler("iks", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_iks), "Open InnKeepersSpecial with a custom url", "<url>");
      cheatMgr.RegisterCheatHandler("iksaction", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_iksgameaction), "Execute a game action as if IKS was clicked.");
      cheatMgr.RegisterCheatHandler("iksseen", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_iksseen), "Determine if an IKS message should be seen by its game action.");
      cheatMgr.RegisterCategory("rank");
      cheatMgr.RegisterCheatHandler("seasondialog", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_seasondialog), "Open the season end dialog", "<rank> [standard|wild|classic]", "bronze5 wild");
      cheatMgr.RegisterCheatHandler("rankrefresh", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_rankrefresh), "Request medalinfo from server and show rankchange twoscoop after receiving it");
      cheatMgr.RegisterCheatHandler("rankchange", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_rankchange), "Show a fake rankchange twoscoop", "[rank] [up|down|win|loss] [wild] [winstreak] [chest]", "bronze5 up chest");
      cheatMgr.RegisterCheatHandler("rankreward", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_rankreward), "Show a fake RankedRewardDisplay for rank (or all ranks up to a rank)", "<rank> [standard|wild|classic|all]", "bronze5 all");
      cheatMgr.RegisterCheatHandler("rankcardback", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_rankcardback), "Show a fake RankedCardBackProgressDisplay", "<wins> [season_id]", "5 75");
      cheatMgr.RegisterCheatHandler("easyrank", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_easyrank), "Easier cheat command to set your rank on the util server", "<rank>", "16");
      cheatMgr.RegisterCheatHandler("resetrotationtutorial", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_resetrotationtutorial), "Cause the user to see the Set Rotation Tutorial again.", "<newbie|veteran>", "newbie|veteran");
      cheatMgr.RegisterCheatHandler("ratingdebug", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ratingdebug), "Display debug information regarding rating", "<#> or <standard/wild/classic>", "standard");
      cheatMgr.RegisterCheatHandler("resetrankedintro", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_resetrankedintro), "Reset game save data values for various tutorial elements for ranked play.");
      cheatMgr.RegisterCheatHandler("localmedaloverride", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_localmedaloverride), "Sets LOCAL ONLY medal data for a given format type to specified value", "[ft_standard|ft_wild|ft_classic] legend_rank=9001", "off");
      cheatMgr.RegisterCategory("sound/vo");
      cheatMgr.RegisterCheatHandler("playnullsound", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_playnullsound), "Tell SoundManager to play a null sound.");
      cheatMgr.RegisterCheatHandler("playaudio", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_playaudio), "Play an audio file by name");
      cheatMgr.RegisterCheatHandler("quote", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_quote), "", "<character> <line> [sound]", "Innkeeper VO_INNKEEPER_FORGE_COMPLETE_22 VO_INNKEEPER_ARENA_COMPLETE");
      cheatMgr.RegisterCheatHandler("narrative", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_narrative), "Show a narrative popup from an achievement");
      cheatMgr.RegisterCheatHandler("narrativedialog", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_narrativedialog), "Show a narrative dialog sequence popup");
      cheatMgr.RegisterCategory("game modes");
      cheatMgr.RegisterCheatHandler("arena", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_arena), "Runs various arena cheats.", "[subcommand] [subcommand args]", "help");
      cheatMgr.RegisterCheatHandler("retire", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_retire), "Retires your draft deck", "", "");
      cheatMgr.RegisterCheatHandler("battlegrounds", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_battlegrounds), "Queue for a game of Battlegrounds.");
      cheatMgr.RegisterCheatHandler("tb", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_tavernbrawl), "Run a variety of Tavern Brawl related commands", "[subcommand] [subcommand args]", "view");
      cheatMgr.RegisterCheatHandler("resetTavernBrawlAdventure", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetTavernBrawlAdventure), "Reset the current Tavern Brawl Adventure progress");
      cheatMgr.RegisterCheatHandler("returningplayer", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_returningplayer), "Set the Returning Player progress", "<0|1|2|3>", "1");
      cheatMgr.RegisterCheatHandler("duels", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_duels), "Run a variety of Duels related commands", "[subcommand] [subcommand args]", "help");
      cheatMgr.RegisterCheatHandler("randomizemercenariesboard", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_randomizemercenariesboard), "Randomize the mercenaries board visuals", "<isFinalBoss> [seed]", "false 1");
      cheatMgr.RegisterCheatHandler("mercs", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_mercs), "Run a variety of mercenaries commands.", "[subcommand] [subcommand args]", "help");
      Network.Get().RegisterNetHandler((object) MercenariesDebugCommandResponse.PacketID.ID, new Network.NetHandler(cheats.OnProcessCheat_mercs_OnResponse));
      cheatMgr.RegisterCategory("ui");
      cheatMgr.RegisterCheatHandler("demotext", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_demotext), "", "<line>", "HelloWorld!");
      cheatMgr.RegisterCheatHandler("popuptext", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_popuptext), "show a popup notification", "<line>", "HelloWorld!");
      cheatMgr.RegisterCheatHandler("alerttext", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_alerttext), "show a popup alert", "<line>", "HelloWorld!");
      cheatMgr.RegisterCheatHandler("logtext", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_logtext), "log a line of text", "<level> <line>", "warning WatchOutWorld!");
      cheatMgr.RegisterCheatHandler("logenable", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_logenable), "temporarily enables a logger", "<logger> <subtype> <enabled>", "Store file/screen/console true");
      cheatMgr.RegisterCheatHandler("loglevel", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_loglevel), "temporarily sets the min level of a logger", "<logger> <level>", "Store debug");
      cheatMgr.RegisterCheatHandler("reloadgamestrings", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_reloadgamestrings), "Reload all game strings from GLUE/GLOBAL/etc.");
      cheatMgr.RegisterCheatHandler("attn", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_userattentionmanager), "Prints out what UserAttentionBlockers, if any, are currently active.");
      cheatMgr.RegisterCheatHandler("banner", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_banner), "Shows the specified wooden banner (supply a banner_id). If none is supplied, it'll show the latest known banner. Use 'banner list' to view all known banners.", "<banner_id> | list", "33");
      cheatMgr.RegisterCheatHandler("notice", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_notice), "Show a notice", "<gold|runestones|arcane_orbs|dust|booster|card|cardback|tavern_brawl_rewards|event|license> [data]");
      cheatMgr.RegisterCheatHandler("load_widget", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_LoadWidget), "Show a widget given a specific guid. If `CHEATED_STATE` exists on a visual controller in the widget, it will be triggered and should be used to help get the widget into the proper location on the screen or any other special test only setup that is needed.");
      cheatMgr.RegisterCheatHandler("clear_widgets", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ClearWidgets), "Remove any widgets that were created via the load_widget cheat");
      cheatMgr.RegisterCheatHandler("serverlog", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ServerLog), "Log a ServerScript message");
      cheatMgr.RegisterCheatHandler("dialogevent", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_dialogEvent), "Choose a category of dialog event, and force it to be run again.", "<event_type> or \"reset\"");
      cheatMgr.RegisterCheatHandler("showtip", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ShowTip), "Shows a tip from a chosen category (or default)", "[category] [index(optional)]", "4 25");
      cheatMgr.RegisterCategory("social");
      cheatMgr.RegisterCheatHandler("spectate", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_spectate), "Connects to a game server to spectate", "<ip_address> <port> <game_handle> <spectator_password> [gameType] [missionId]");
      cheatMgr.RegisterCheatHandler("party", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_party), "Run a variety of party related commands", "[sub command] [subcommand args]", "list");
      cheatMgr.RegisterCheatHandler("raf", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_raf), "Run a RAF UI related commands", "[subcommand]", "showprogress");
      cheatMgr.RegisterCheatHandler("flist", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_friendlist), "Run various friends list cheats.", "[subcommand] [subcommand args]", "add remove");
      cheatMgr.RegisterCheatHandler("fsg", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_fsg), "Run a variety of Fireside Gathering related commands", "[subcommand] [subcommand args]", "view");
      cheatMgr.RegisterCheatHandler("gps", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_GPS), "Modify GPS information in editor", "[subcommand] [subcommand args]", "view");
      cheatMgr.RegisterCheatHandler("wifi", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_Wifi), "Modify WIFI information in editor", "[subcommand] [subcommand args]", "view");
      cheatMgr.RegisterCheatHandler("social", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_social), "View information about the social list (friends, nearby players, FSG patrons, etc)", "[subcommand] [subcommand args]", "list");
      cheatMgr.RegisterCheatHandler("playstartemote", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_playStartEmote), " the appropriate start, mirror start, or custom start emote on first the enemy hero, then the friendly hero");
      cheatMgr.RegisterCheatHandler("getbgdenylist", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_getBattlegroundDenyList), "Get Battleground deny list");
      cheatMgr.RegisterCheatHandler("getbgminionpool", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_getBattlegroundMinionPool), "Get Battleground minion pool");
      cheatMgr.RegisterCheatHandler("getbgheroarmortierlist", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_getBattlegroundHeroArmorTierList), "Get Battleground Hero Armor Tier List");
      cheatMgr.RegisterCheatHandler("setbgbuddyprog", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetBattlegroundHeroBuddyProgress), "Set the progress of Battleground Hero Buddy");
      cheatMgr.RegisterCheatHandler("setbgbuddygained", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetBattlegroundHeroBuddyGained), "Set number Battleground Hero Buddy Gained");
      cheatMgr.RegisterCheatHandler("replacebghero", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ReplaceBattlegroundHero), "Replace Battleground Hero");
      cheatMgr.RegisterCheatHandler("enablebgherobuddy", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_EnableBattlegroundHeroBuddy), "Enable Battleground Hero Buddy Locally");
      cheatMgr.RegisterCheatHandler("bgboard", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_BattlegroundsBoardFSMManipulate), "Manipulate FSMs for Battlegrounds Board cosmetic effects");
      cheatMgr.RegisterCheatHandler("setbgluckydrawendtime", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetBattlegroundsLuckyDrawEndTime), "Set the end time when lucky draw will ends (in seconds)");
      cheatMgr.RegisterCategory("device");
      cheatMgr.RegisterCheatHandler("lowmemorywarning", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_lowmemorywarning), "Simulate a low memory warning from mobile.");
      cheatMgr.RegisterCheatHandler("mobile", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_mobile), "Run Mobile related commands", "subcommand [subcommand args]", "subcommand:login|push|ngdp subcommand args:clear|register|logout");
      cheatMgr.RegisterCheatHandler("edittextdebug", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_edittextdebug), "Toggle EditText debugging");
      cheatMgr.RegisterCategory("streaming");
      cheatMgr.RegisterCheatHandler("setupdateintention", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_UpdateIntention), "Set the next \"goal\" for the runtime update manager", "[UpdateIntention]");
      cheatMgr.RegisterCheatHandler("updater", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_Updater), "Modify the properties of Updater", "[subcommand] [subcommand args]", "speed");
      cheatMgr.RegisterCategory("assets");
      cheatMgr.RegisterCheatHandler("printassethandles", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_Assets), "Prints outstanding AssetHandles", "[filter]");
      cheatMgr.RegisterCheatHandler("printassetbundles", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_Assets), "Prints open AssetBundles", "[filter]");
      cheatMgr.RegisterCheatHandler("dumpassets", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_Assets), "Dumps AssetHandles and AssetBundles to CSV files", "[filter]");
      cheatMgr.RegisterCheatHandler("orphanasset", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_Assets), "Orphans an AssetHandle");
      cheatMgr.RegisterCheatHandler("orphanprefab", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_Assets), "Orphans a shared prefab");
      cheatMgr.RegisterCategory("account data");
      cheatMgr.RegisterCheatHandler("account", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_account), "Account management cheat");
      cheatMgr.RegisterCheatHandler("cloud", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_cloud), "Run Cloud Storage related commands", "[subcommand]", "set");
      cheatMgr.RegisterCheatHandler("tempaccount", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_tempaccount), "Run Temporary Account related commands", "[subcommand]", "dialog");
      cheatMgr.RegisterCheatHandler("getgsd", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_GetGameSaveData), "Request the value of a particular Game Save Data subkey.", "[key] [subkey]", "24 13");
      cheatMgr.RegisterCheatHandler("gsd", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_GetGameSaveData), "Request the value of a particular Game Save Data subkey.", "[key] [subkey]", "24 13");
      cheatMgr.RegisterCheatHandler("setgsd", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetGameSaveData), "Set the value(s) of a Game Save Data subkey. Can provide multiple values to set a list.", "[key] [subkey] [int_value]", "24 13 2");
      cheatMgr.RegisterCategory("adventure");
      cheatMgr.RegisterCheatHandler("adventureChallengeUnlock", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_adventureChallengeUnlock), "Show adventure challenge unlock", "<wing number>");
      cheatMgr.RegisterCheatHandler("advevent", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_advevent), "Trigger an AdventureWingEventTable event.", "<event name>", "PlateOpen");
      cheatMgr.RegisterCheatHandler("showadventureloadingpopup", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ShowAdventureLoadingPopup), "Show the popup for loading into the currently-set Adventure mission.");
      cheatMgr.RegisterCheatHandler("hidegametransitionpopup", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_HideGameTransitionPopup), "Hide any currently shown game transition popup.");
      cheatMgr.RegisterCheatHandler("setallpuzzlesinprogress", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetAllPuzzlesInProgress), "Set the sub-puzzle progress for each puzzle to be on the final puzzle.");
      cheatMgr.RegisterCheatHandler("unlockhagatha", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_UnlockHagatha), "Set up the hagatha unlock flow. After running the cheat, complete a monster hunt to unlock.");
      cheatMgr.RegisterCheatHandler("setadventurecomingsoon", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetAdventureComingSoon), "Set the Coming Soon state of an adventure.");
      cheatMgr.RegisterCheatHandler("resetsessionvo", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetSession_VO), "Reset the fact that you've seen once per session related VO, to be able to hear it again.");
      cheatMgr.RegisterCheatHandler("setvochance", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetVOChance_VO), "Set an override on the chance to play a VO line in the adventure. This will only override the chance on VO that won't always play.", "<chance>", "0.1");
      cheatMgr.RegisterCategory("adventure:dungeon run");
      cheatMgr.RegisterCheatHandler("setdrprogress", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetDungeonRunProgress), "Set how many bosses you've defeated during an active run in the provided Adventure.", "[adventure abbreviation] [num bosses] [next boss id (optional)]", "uld 7 46589");
      cheatMgr.RegisterCheatHandler("setdrvictory", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetDungeonRunVictory), "Set victory in the provided Adventure.", "<adventure abbreviation>", "uld");
      cheatMgr.RegisterCheatHandler("setdrdefeat", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetDungeonRunDefeat), "Set defeat and how many bosses you've defeated in the provided Adventure.", "[adventure abbreviation] [num bosses]", "uld 7");
      cheatMgr.RegisterCheatHandler("resetdradventure", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetDungeonRunAdventure), "Reset the current run for the provided Adventure.", "[adventure abbreviation]", "uld");
      cheatMgr.RegisterCheatAlias("resetdradventure", "resetdrrun");
      cheatMgr.RegisterCheatHandler("resetdrvo", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetDungeonRun_VO), "Reset the fact that you've seen all VO related to the provided Adventure, to be able to hear it again.", "[adventure abbreviation] [optional:value to set subkeys to]", "uld 1");
      cheatMgr.RegisterCheatHandler("unlockloadout", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_UnlockLoadout), "Unlock all loadout options for the provided Adventure.", "[adventure abbreviation]", "uld");
      cheatMgr.RegisterCheatHandler("lockloadout", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_LockLoadout), "Lock all loadout options for the provided Adventure.", "[adventure abbreviation]", "uld");
      cheatMgr.RegisterCategory("adventure:k&c");
      cheatMgr.RegisterCheatHandler("setkcprogress", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetKCProgress), "Set how many bosses you've defeated during an active run in Kobolds & Catacombs.", "[num bosses] [next boss id (optional)]", "7 46589");
      cheatMgr.RegisterCheatHandler("setkcvictory", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetKCVictory), "Set victory in Kobolds & Catacombs.");
      cheatMgr.RegisterCheatHandler("setkcdefeat", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetKCDefeat), "Set defeat and how many bosses you've defeated in Kobolds & Catacombs.", "<num bosses>", "7");
      cheatMgr.RegisterCheatHandler("resetkcvo", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetKC_VO), "Reset the fact that you've seen all K&C related VO, to be able to hear it again.");
      cheatMgr.RegisterCategory("adventure:witchwood");
      cheatMgr.RegisterCheatHandler("setgilprogress", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetGILProgress), "Set how many bosses you've defeated during an active run in Witchwood.", "[num bosses] [next boss id (optional)]", "7 46589");
      cheatMgr.RegisterCheatHandler("setgilvictory", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetGILVictory), "Set victory in Witchwood.");
      cheatMgr.RegisterCheatHandler("setgildefeat", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetGILDefeat), "Set defeat and how many bosses you've defeated in Witchwood.", "<num bosses>", "7");
      cheatMgr.RegisterCheatHandler("setgilbonus", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetGILBonus), "Set the Witchwood bonus challenge to be active.");
      cheatMgr.RegisterCheatHandler("resetGilAdventure", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetGILAdventure), "Reset the current Witchwood Adventure run.");
      cheatMgr.RegisterCheatHandler("resetgilvo", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetGIL_VO), "Reset the fact that you've seen all Witchwood related VO, to be able to hear it again.");
      cheatMgr.RegisterCategory("adventure:rastakhan");
      cheatMgr.RegisterCheatHandler("settrlprogress", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetTRLProgress), "Set how many bosses you've defeated during an active run in Rastakhan.", "[num bosses] [next boss id (optional)]", "7 46589");
      cheatMgr.RegisterCheatHandler("settrlvictory", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetTRLVictory), "Set victory in Rastakhan.");
      cheatMgr.RegisterCheatHandler("settrldefeat", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetTRLDefeat), "Set defeat and how many bosses you've defeated in Rastakhan.", "<num bosses>", "7");
      cheatMgr.RegisterCheatHandler("resettrlvo", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetTRL_VO), "Reset the fact that you've seen all Rastakhan related VO, to be able to hear it again.");
      cheatMgr.RegisterCategory("adventure:dalaran");
      cheatMgr.RegisterCheatHandler("setdalprogress", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetDALProgress), "Set how many bosses you've defeated during an active run in Dalaran.", "[num bosses] [next boss id (optional)]", "7 46589");
      cheatMgr.RegisterCheatHandler("setdalvictory", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetDALVictory), "Set victory in Dalaran.");
      cheatMgr.RegisterCheatHandler("setdaldefeat", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetDALDefeat), "Set defeat and how many bosses you've defeated in Dalaran.", "<num bosses>", "7");
      cheatMgr.RegisterCheatHandler("resetDalaranAdventure", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetDalaranAdventure), "Reset the current Dalaran Adventure run, so you can start at the location selection again.");
      cheatMgr.RegisterCheatHandler("setdalheroicprogress", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetDALHeroicProgress), "Set how many bosses you've defeated during an active run in Dalaran Heroic.", "[num bosses] [next boss id (optional)]", "7 46589");
      cheatMgr.RegisterCheatHandler("setdalheroicvictory", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetDALHeroicVictory), "Set victory in Dalaran Heroic.");
      cheatMgr.RegisterCheatHandler("setdalheroicdefeat", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetDALHeroicDefeat), "Set defeat and how many bosses you've defeated in Dalaran Heroic.", "<num bosses>", "7");
      cheatMgr.RegisterCheatHandler("resetDalaranHeroicAdventure", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetDalaranHeroicAdventure), "Reset the current Dalaran Heroic Adventure run, so you can start at the location selection again.");
      cheatMgr.RegisterCheatHandler("resetdalvo", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetDAL_VO), "Reset the fact that you've seen all Dalaran related VO, to be able to hear it again.");
      cheatMgr.RegisterCategory("adventure:uldum");
      cheatMgr.RegisterCheatHandler("setuldprogress", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetULDProgress), "Set how many bosses you've defeated during an active run in Uldum.", "[num bosses] [next boss id (optional)]", "7 46589");
      cheatMgr.RegisterCheatHandler("setuldvictory", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetULDVictory), "Set victory in Uldum.");
      cheatMgr.RegisterCheatHandler("setulddefeat", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetULDDefeat), "Set defeat and how many bosses you've defeated in Uldum.", "<num bosses>", "7");
      cheatMgr.RegisterCheatHandler("resetuldrun", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetUldumAdventure), "Reset the current Uldum Adventure run, so you can start at the location selection again.");
      cheatMgr.RegisterCheatAlias("resetuldrun", "resetUldumAdventure");
      cheatMgr.RegisterCheatHandler("setuldheroicprogress", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetULDHeroicProgress), "Set how many bosses you've defeated during an active run in Uldum Heroic.", "[num bosses] [next boss id (optional)]", "7 46589");
      cheatMgr.RegisterCheatHandler("setuldheroicvictory", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetULDHeroicVictory), "Set victory in Uldum Heroic.");
      cheatMgr.RegisterCheatHandler("setuldheroicdefeat", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SetULDHeroicDefeat), "Set defeat and how many bosses you've defeated in Uldum Heroic.", "<num bosses>", "7");
      cheatMgr.RegisterCheatHandler("resetuldheroicrun", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetUldumHeroicAdventure), "Reset the current Uldum Heroic Adventure run, so you can start at the location selection again.");
      cheatMgr.RegisterCheatAlias("resetuldheroicrun", "resetUldumHeroicAdventure");
      cheatMgr.RegisterCheatHandler("resetuldvo", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ResetULD_VO), "Reset the fact that you've seen all Uldum related VO, to be able to hear it again.");
      cheatMgr.DefaultCategory();
      cheatMgr.RegisterCheatHandler("brode", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_brode), "Brode's personal cheat", "", "");
      cheatMgr.RegisterCheatHandler("freeyourmind", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_freeyourmind), "And the rest will follow");
    }
    cheatMgr.RegisterCategory("config");
    cheatMgr.RegisterCheatHandler("has", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_HasOption), "Query whether a Game Option exists.");
    cheatMgr.RegisterCheatHandler("get", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_GetOption), "Get the value of a Game Option.");
    cheatMgr.RegisterCheatHandler("set", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_SetOption), "Set the value of a Game Option.");
    cheatMgr.RegisterCheatHandler("getvar", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_GetVar), "Get the value of a client.config var.");
    cheatMgr.RegisterCheatHandler("setvar", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_SetVar), "Set the value of a client.config var.");
    cheatMgr.RegisterCheatHandler("delete", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_DeleteOption), "Delete a Game Option; the absence of option may trigger default behavior");
    cheatMgr.RegisterCheatAlias("delete", "del");
    cheatMgr.RegisterCategory("ui");
    cheatMgr.RegisterCheatHandler("nav", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_navigation), "Debug Navigation.GoBack");
    cheatMgr.RegisterCheatAlias("nav", "navigate");
    cheatMgr.RegisterCheatHandler("warning", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_warning), "Show a warning message", "<message>", "Test You're a cheater and you've been warned!");
    cheatMgr.RegisterCheatHandler("fatal", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_fatal), "Brings up the Fatal Error screen", "<error to display>", "Hearthstone cheated and failed!");
    cheatMgr.RegisterCheatHandler("alert", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_alert), "Show a popup alert", "header=<string> text=<string> icon=<bool> response=<ok|confirm|cancel|confirm_cancel> oktext=<string> confirmtext=<string>", "header=header text=body text icon=true response=confirm");
    cheatMgr.RegisterCheatAlias("alert", "popup", "dialog");
    cheatMgr.RegisterCheatHandler("exampleui", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ExampleUI));
    cheatMgr.RegisterCheatHandler("rankedintropopup", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_rankedIntroPopup), "Show the Ranked Intro Popup");
    cheatMgr.RegisterCheatHandler("setrotationrotatedboosterspopup", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_setRotationRotatedBoostersPopup), "Show the Set Rotation Tutorial Popup");
    cheatMgr.RegisterCategory("game modes");
    cheatMgr.RegisterCheatHandler("autodraft", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_autodraft), "Sets Arena autodraft on/off.", "<on | off>", "on");
    cheatMgr.RegisterCategory("program");
    cheatMgr.RegisterCheatHandler("exit", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_exit), "Exit the application", "", "");
    cheatMgr.RegisterCheatAlias("exit", "quit");
    cheatMgr.RegisterCheatHandler("pause", (CheatMgr.ProcessCheatCallback) ((a, b, c) =>
    {
      HearthstoneApplication.Get().OnApplicationPause(true);
      return true;
    }));
    cheatMgr.RegisterCheatHandler("unpause", (CheatMgr.ProcessCheatCallback) ((a, b, c) =>
    {
      HearthstoneApplication.Get().OnApplicationPause(false);
      return true;
    }));
    cheatMgr.RegisterCategory("account data");
    cheatMgr.RegisterCheatHandler("clearofflinelocalcache", (CheatMgr.ProcessCheatCallback) ((a, b, c) =>
    {
      OfflineDataCache.ClearLocalCacheFile();
      return true;
    }));
    cheatMgr.RegisterCheatHandler("herocount", new CheatMgr.ProcessCheatAutofillCallback(cheats.OnProcessCheat_HeroCount), "Set the hero picker count and reload UI", "number of heroes to display 1-12", "12");
    cheatMgr.DefaultCategory();
    cheatMgr.RegisterCheatHandler("attribution", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_Attribution));
    cheatMgr.RegisterCheatHandler("crm", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_CRM));
    cheatMgr.RegisterCategory("progression");
    cheatMgr.RegisterCheatHandler("checkfornewquests", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_checkfornewquests), "Trigger a check for next quests after n secs (default 0)", "[delaySecs]", "1");
    cheatMgr.RegisterCheatHandler("checkforexpiredquests", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_checkforexpiredquests), "Trigger a check for expired quests after n secs (default 0)", "[delaySecs]", "1");
    cheatMgr.RegisterCheatHandler("showachievementreward", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_showachievementreward), "show a fake achievement reward scroll");
    cheatMgr.RegisterCheatHandler("showquestreward", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_showquestreward), "show a fake quest reward scroll");
    cheatMgr.RegisterCheatHandler("showtrackreward", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_showtrackreward), "show a fake track reward scroll");
    cheatMgr.RegisterCheatHandler("showquestprogresstoast", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_showquestprogresstoast), "Pop up a quest progress toast widget", "<quest id>", "2");
    cheatMgr.RegisterCheatHandler("showquestnotification", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_showquestnotification), "Shows the quest notification popup widget", "<daily|weekly>", "daily");
    cheatMgr.RegisterCheatHandler("showachievementtoast", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_showachievementtoast), "Pop up a achievement complete toast widget", "<achieve id>", "2");
    cheatMgr.RegisterCheatHandler("showprogtileids", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_showprogtileids), "Show the quest id or achievement id on quest and achievement tiles");
    cheatMgr.RegisterCheatHandler("showhiddenachievements", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_showhiddenachievements), "Show hidden achievements in the UI");
    cheatMgr.RegisterCheatHandler("earlyconcedeconfirmationdisabled", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_earlyConcedeConfirmationDisabled), "Disable the early concede confirmation popup warning");
    cheatMgr.RegisterCheatHandler("simendofgamexp", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_simendofgamexp), "Simulate different end of game situations and show end of game xp screen.", "<scenario id>", "1");
    cheatMgr.RegisterCheatHandler("terminateendofgamexp", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_terminateendofgamexp), "Terminate the current end of game xp or simulation");
    cheatMgr.RegisterCheatHandler("showunclaimedtrackrewards", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_showunclaimedtrackrewards), "Show the reward track's unclaimed rewards popup.");
    cheatMgr.RegisterCategory("general");
    cheatMgr.RegisterCheatHandler("log", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_log));
    cheatMgr.RegisterCheatHandler("ip", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_IPAddress));
    cheatMgr.RegisterCheatHandler("shownotavernpasswarning", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_shownotavernpasswarning), "Shows the warning popup when no tavern pass is available");
    cheatMgr.RegisterCheatHandler("setlastrewardtrackseasonseen", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_setlastrewardtrackseasonseen), "Sets the GSD value of Rewards Track: Season Last Seen");
    cheatMgr.RegisterCheatHandler("apprating", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ShowAppRatingPrompt), "Shows the app review popup (Android and iOS only).");
    cheatMgr.RegisterCheatHandler("optin", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_UpdateAADCSetting), "Gets and sets AADC opt-ins.");
    cheatMgr.RegisterCheatHandler("showpresence", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ShowPresence), "Shows the current presence string for the local player");
    cheatMgr.RegisterCheatHandler("showvillagehelppopups", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ShowVillageHelpPopups), "Shows all help popups that appear during the village tutorial at once");
    cheatMgr.RegisterCheatHandler("merctraining", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_MercTraining), "Client side merc training commands to circumvent UI");
    cheatMgr.RegisterCheatHandler("showmercenariestaskcompletetoasts", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_ShowMercenariesTaskToasts), "Shows X num of task complete toasts for testing UI");
    cheatMgr.RegisterCheatHandler("dumpmaterials", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_LogMaterialService), "Logs important information regarding the material service");
    cheatMgr.RegisterCheatHandler("logzombies", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_LogZombies), "Logs zombie objects");
    cheatMgr.RegisterCheatHandler("sendreport", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SendReport), "Sends a CS report", "<account id> <issue type> <user source>");
    cheatMgr.RegisterCheatHandler("mercdetails", new CheatMgr.ProcessCheatCallback(cheats.OnProgressCheat_MercDetails), "Show the Merc Details for a specific merc");
    cheatMgr.RegisterCheatHandler("logspellusage", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_LogSpellUsage), "logs spell usage");
    cheatMgr.RegisterCheatHandler("spawnspellscache", new CheatMgr.ProcessCheatCallback(cheats.OnSpawnSpellsCache_Test), "spawn 50 sleep spells using cache");
    cheatMgr.RegisterCheatHandler("spawnspellsassetloader", new CheatMgr.ProcessCheatCallback(cheats.OnSpawnSpellsAssetLoader_Test), "50 sleep spells not using cach");
    cheatMgr.RegisterCheatHandler("ackallnotices", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_AckAllNotices), "Acks all pending notices. Requires client restart to dismiss popups in queue.");
    cheatMgr.RegisterCheatHandler("appsflyer", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_AppsFlyer), "Apps flyer cheats");
    cheatMgr.RegisterCheatHandler("soundmono", new CheatMgr.ProcessCheatCallback(cheats.OnProcessCheat_SoundMono), "Sound Mono");
    cheatMgr.RegisterCheatHandler("task_board", new CheatMgr.ProcessCheatCallback(cheats.OnProgressCheat_TaskBoardCheat), "Cheats related to the task board UI", "<nav_type|search_all>");
    return false;
  }

  private bool OnSpawnSpellsAssetLoader_Test(string func, string[] args, string rawArgs)
  {
    SpellManager spellManager = SpellManager.Get();
    string spellAssetRef = "Card_Play_Ally_Zzz.prefab:734e352ca20a1494e8ec18226bf49f4c";
    int num = 50;
    for (int index = 0; index < num; ++index)
      spellManager.GetSpell(spellAssetRef);
    return true;
  }

  private bool OnSpawnSpellsCache_Test(string func, string[] args, string rawArgs)
  {
    SpellManager spellManager = SpellManager.Get();
    string spellAssetRef = "Card_Play_Ally_Zzz.prefab:734e352ca20a1494e8ec18226bf49f4c";
    int num = 50;
    for (int index = 0; index < num; ++index)
      spellManager.GetSpell(spellAssetRef, true);
    return true;
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (CheatMgr),
    typeof (Network)
  };

  public void Shutdown() => Cheats.s_instance = (Cheats) null;

  public int GetBoardId() => this.m_boardId;

  public void ClearBoardId() => this.m_boardId = 0;

  public bool HasCheatTreasureIds() => this.m_pvpdrTreasureIds.Count > 0;

  public void ClearCheatTreasures() => this.m_pvpdrTreasureIds.Clear();

  public bool HasCheatLootIds() => this.m_pvpdrLootIds.Count > 0;

  public void ClearCheatLoot() => this.m_pvpdrLootIds.Clear();

  public bool IsSpeechBubbleEnabled() => this.m_speechBubblesEnabled;

  public bool IsSoundCategoryEnabled(Global.SoundCategory sc) => !this.m_audioChannelEnabled.ContainsKey(sc) || this.m_audioChannelEnabled[sc];

  public string GetPlayerTags() => this.m_playerTags;

  public void ClearAllPlayerTags() => this.m_playerTags = "";

  public bool IsNewCardInPackOpeningEnabed() => this.m_isNewCardInPackOpeningEnabled;

  public bool IsLaunchingQuickGame() => this.m_quickLaunchState.m_launching;

  public bool ShouldSkipMulligan() => Options.Get().GetBool(Option.SKIP_ALL_MULLIGANS) || this.m_quickLaunchState.m_skipMulligan;

  public bool ShouldSkipSendingGetGameState() => this.m_skipSendingGetGameState;

  public bool HandleKeyboardInput() => HearthstoneApplication.IsInternal() && this.HandleQuickPlayInput();

  public void SaveDuelsCheatTreasures(out List<int> addedTreasures)
  {
    AdventureDataDbfRecord adventureDataRecord = AdventureConfig.Get().GetSelectedAdventureDataRecord();
    addedTreasures = new List<int>();
    if (this.m_pvpdrTreasureIds.Count<int>() <= 0 || adventureDataRecord == null)
      return;
    List<long> values = (List<long>) null;
    GameSaveDataManager.Get().GetSubkeyValue((GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_TREASURE_OPTION, out values);
    if (values == null)
      return;
    int num1 = Math.Min(this.m_pvpdrTreasureIds.Count, values.Count);
    for (int index = 0; index < num1; ++index)
    {
      int num2 = this.m_pvpdrTreasureIds.Dequeue();
      if (num2 > 0)
      {
        values[index] = (long) num2;
        addedTreasures.Add(num2);
      }
    }
    this.InvokeSetGameSaveDataCheat((GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_TREASURE_OPTION, values.ToArray());
  }

  private bool AddCheatLootToBucket(
    AdventureDataDbfRecord dataRecord,
    GameSaveKeySubkeyId subkey,
    List<int> addedLoot)
  {
    List<long> values = (List<long>) null;
    GameSaveDataManager.Get().GetSubkeyValue((GameSaveKeyId) dataRecord.GameSaveDataServerKey, subkey, out values);
    if (values == null || values.Count < 4)
      return false;
    for (int index = 0; index < 3 && this.m_pvpdrLootIds.Count != 0; ++index)
    {
      int num = this.m_pvpdrLootIds.Dequeue();
      if (num > 0)
      {
        values[index + 1] = (long) num;
        addedLoot.Add(num);
      }
    }
    this.InvokeSetGameSaveDataCheat((GameSaveKeyId) dataRecord.GameSaveDataServerKey, subkey, values.ToArray());
    return true;
  }

  public void SaveDuelsCheatLoot(
    out List<int> addedLootA,
    out List<int> addedLootB,
    out List<int> addedLootC)
  {
    AdventureDataDbfRecord adventureDataRecord = AdventureConfig.Get().GetSelectedAdventureDataRecord();
    addedLootA = new List<int>();
    addedLootB = new List<int>();
    addedLootC = new List<int>();
    if (this.m_pvpdrLootIds.Count <= 0 || adventureDataRecord == null || !this.AddCheatLootToBucket(adventureDataRecord, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_A, addedLootA) || this.m_pvpdrLootIds.Count <= 0 || !this.AddCheatLootToBucket(adventureDataRecord, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_B, addedLootB) || this.m_pvpdrLootIds.Count <= 0)
      return;
    this.AddCheatLootToBucket(adventureDataRecord, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_C, addedLootC);
  }

  private void ParseErrorText(
    string[] args,
    string rawArgs,
    out string header,
    out string message)
  {
    header = args.Length == 0 ? "[PH] Header" : args[0];
    if (args.Length <= 1)
    {
      message = "[PH] Message";
    }
    else
    {
      int startIndex = 0;
      bool flag = false;
      for (int index = 0; index < rawArgs.Length; ++index)
      {
        if (char.IsWhiteSpace(rawArgs[index]))
        {
          if (flag)
          {
            startIndex = index;
            break;
          }
        }
        else
          flag = true;
      }
      message = rawArgs.Substring(startIndex).Trim();
    }
  }

  private AlertPopup.PopupInfo GenerateAlertInfo(string rawArgs)
  {
    Map<string, string> alertArgs = this.ParseAlertArgs(rawArgs);
    AlertPopup.PopupInfo alertInfo = new AlertPopup.PopupInfo();
    alertInfo.m_showAlertIcon = false;
    alertInfo.m_headerText = "Header";
    alertInfo.m_text = "Message";
    alertInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
    alertInfo.m_okText = "OK";
    alertInfo.m_confirmText = "Confirm";
    alertInfo.m_cancelText = "Cancel";
    foreach (KeyValuePair<string, string> keyValuePair in alertArgs)
    {
      string key = keyValuePair.Key;
      string str1 = keyValuePair.Value;
      if (key.Equals("header"))
        alertInfo.m_headerText = str1;
      else if (key.Equals("text"))
        alertInfo.m_text = str1;
      else if (key.Equals("response"))
      {
        string lowerInvariant = str1.ToLowerInvariant();
        if (lowerInvariant.Equals("ok"))
          alertInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
        else if (lowerInvariant.Equals("confirm"))
          alertInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM;
        else if (lowerInvariant.Equals("cancel"))
          alertInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL;
        else if (lowerInvariant.Equals("confirm_cancel") || lowerInvariant.Equals("cancel_confirm"))
          alertInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
      }
      else if (key.Equals("icon"))
        alertInfo.m_showAlertIcon = GeneralUtils.ForceBool(str1);
      else if (key.Equals("oktext"))
        alertInfo.m_okText = str1;
      else if (key.Equals("confirmtext"))
        alertInfo.m_confirmText = str1;
      else if (key.Equals("canceltext"))
        alertInfo.m_cancelText = str1;
      else if (key.Equals("offset"))
      {
        string[] strArray = str1.Split();
        Vector3 vector3 = new Vector3();
        if (strArray.Length % 2 == 0)
        {
          for (int index = 0; index < strArray.Length; index += 2)
          {
            string lowerInvariant = strArray[index].ToLowerInvariant();
            string str2 = strArray[index + 1];
            if (lowerInvariant.Equals("x"))
              vector3.x = GeneralUtils.ForceFloat(str2);
            else if (lowerInvariant.Equals("y"))
              vector3.y = GeneralUtils.ForceFloat(str2);
            else if (lowerInvariant.Equals("z"))
              vector3.z = GeneralUtils.ForceFloat(str2);
          }
        }
        alertInfo.m_offset = vector3;
      }
      else if (key.Equals("padding"))
        alertInfo.m_padding = GeneralUtils.ForceFloat(str1);
      else if (key.Equals("align"))
      {
        string str3 = str1;
        char[] chArray = new char[1]{ '|' };
        foreach (string str4 in str3.Split(chArray))
        {
          string lower = str4.ToLower();
          if (!(lower == "left"))
          {
            if (!(lower == "center"))
            {
              if (!(lower == "right"))
              {
                if (!(lower == "top"))
                {
                  if (!(lower == "middle"))
                  {
                    if (lower == "bottom")
                      alertInfo.m_alertTextAlignmentAnchor = UberText.AnchorOptions.Lower;
                  }
                  else
                    alertInfo.m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle;
                }
                else
                  alertInfo.m_alertTextAlignmentAnchor = UberText.AnchorOptions.Upper;
              }
              else
                alertInfo.m_alertTextAlignment = UberText.AlignmentOptions.Right;
            }
            else
              alertInfo.m_alertTextAlignment = UberText.AlignmentOptions.Center;
          }
          else
            alertInfo.m_alertTextAlignment = UberText.AlignmentOptions.Left;
        }
      }
    }
    return alertInfo;
  }

  private Map<string, string> ParseAlertArgs(string rawArgs)
  {
    Map<string, string> alertArgs = new Map<string, string>();
    int startIndex1 = -1;
    string key = (string) null;
    for (int index1 = 0; index1 < rawArgs.Length; ++index1)
    {
      if (rawArgs[index1] == '=')
      {
        int startIndex2 = -1;
        for (int index2 = index1 - 1; index2 >= 0; --index2)
        {
          int rawArg1 = (int) rawArgs[index2];
          char rawArg2 = rawArgs[index2 + 1];
          if (!char.IsWhiteSpace((char) rawArg1))
            startIndex2 = index2;
          if (char.IsWhiteSpace((char) rawArg1) && !char.IsWhiteSpace(rawArg2))
            break;
        }
        if (startIndex2 >= 0)
        {
          int num = startIndex2 - 2;
          if (key != null)
            alertArgs[key] = rawArgs.Substring(startIndex1, num - startIndex1 + 1);
          startIndex1 = index1 + 1;
          key = rawArgs.Substring(startIndex2, index1 - startIndex2).Trim().ToLowerInvariant().Replace("\\n", "\n");
        }
      }
    }
    int num1 = rawArgs.Length - 1;
    if (key != null)
      alertArgs[key] = rawArgs.Substring(startIndex1, num1 - startIndex1 + 1).Replace("\\n", "\n");
    return alertArgs;
  }

  private bool OnAlertProcessed(DialogBase dialog, object userData)
  {
    this.m_alert = (AlertPopup) dialog;
    return true;
  }

  private void HideAlert()
  {
    if (!((UnityEngine.Object) this.m_alert != (UnityEngine.Object) null))
      return;
    this.m_alert.Hide();
    this.m_alert = (AlertPopup) null;
  }

  private bool HandleQuickPlayInput()
  {
    if (!ServiceManager.IsAvailable<SceneMgr>() || !InputCollection.GetKey(KeyCode.LeftShift) && !InputCollection.GetKey(KeyCode.RightShift))
      return false;
    if (InputCollection.GetKeyDown(KeyCode.F12))
    {
      this.PrintQuickPlayLegend();
      return false;
    }
    if (this.GetQuickLaunchAvailability() != Cheats.QuickLaunchAvailability.OK)
      return false;
    ScenarioDbId scenarioDbId1 = ScenarioDbId.INVALID;
    string str = (string) null;
    foreach (KeyValuePair<KeyCode, ScenarioDbId> quickPlayKey in Cheats.s_quickPlayKeyMap)
    {
      KeyCode key = quickPlayKey.Key;
      ScenarioDbId scenarioDbId2 = quickPlayKey.Value;
      if (InputCollection.GetKeyDown(key))
      {
        scenarioDbId1 = scenarioDbId2;
        str = Cheats.s_opponentHeroKeyMap[key];
        break;
      }
    }
    if (scenarioDbId1 == ScenarioDbId.INVALID)
      return false;
    this.m_quickLaunchState.m_mirrorHeroes = false;
    this.m_quickLaunchState.m_flipHeroes = false;
    this.m_quickLaunchState.m_skipMulligan = true;
    this.m_quickLaunchState.m_opponentHeroCardId = str;
    if ((InputCollection.GetKey(KeyCode.RightAlt) || InputCollection.GetKey(KeyCode.LeftAlt)) && (InputCollection.GetKey(KeyCode.RightControl) || InputCollection.GetKey(KeyCode.LeftControl)))
    {
      this.m_quickLaunchState.m_mirrorHeroes = true;
      this.m_quickLaunchState.m_skipMulligan = false;
      this.m_quickLaunchState.m_flipHeroes = false;
    }
    else if (InputCollection.GetKey(KeyCode.RightControl) || InputCollection.GetKey(KeyCode.LeftControl))
    {
      this.m_quickLaunchState.m_flipHeroes = false;
      this.m_quickLaunchState.m_skipMulligan = false;
      this.m_quickLaunchState.m_mirrorHeroes = false;
    }
    else if (InputCollection.GetKey(KeyCode.RightAlt) || InputCollection.GetKey(KeyCode.LeftAlt))
    {
      this.m_quickLaunchState.m_flipHeroes = true;
      this.m_quickLaunchState.m_skipMulligan = false;
      this.m_quickLaunchState.m_mirrorHeroes = false;
    }
    if (Cheats.s_quickPlayNotSkipMulligan.Contains(scenarioDbId1))
      this.m_quickLaunchState.m_skipMulligan = false;
    GameType gameType = GameType.GT_VS_AI;
    if (Cheats.s_scenarioToGameTypeMap.ContainsKey(scenarioDbId1))
      gameType = Cheats.s_scenarioToGameTypeMap[scenarioDbId1];
    this.LaunchQuickGame((int) scenarioDbId1, gameType);
    return true;
  }

  private void PrintQuickPlayLegend()
  {
    string message = string.Format("F1: {0}\nF2: {1}\nF3: {2}\nF4: {3}\nF5: {4}\nF6: {5}\nF7: {6}\nF8: {7}\nF9: {8}\nF10: {9}\n(CTRL and ALT will Show mulligan)\nSHIFT + CTRL = Hero on players side\nSHIFT + ALT = Hero on opponent side\nSHIFT + ALT + CTRL = Hero on both sides", (object) this.GetQuickPlayMissionName(KeyCode.F1), (object) this.GetQuickPlayMissionName(KeyCode.F2), (object) this.GetQuickPlayMissionName(KeyCode.F3), (object) this.GetQuickPlayMissionName(KeyCode.F4), (object) this.GetQuickPlayMissionName(KeyCode.F5), (object) this.GetQuickPlayMissionName(KeyCode.F6), (object) this.GetQuickPlayMissionName(KeyCode.F7), (object) this.GetQuickPlayMissionName(KeyCode.F8), (object) this.GetQuickPlayMissionName(KeyCode.F9), (object) this.GetQuickPlayMissionName(KeyCode.F10));
    if ((UnityEngine.Object) UIStatus.Get() != (UnityEngine.Object) null)
      UIStatus.Get().AddInfo(message);
    Debug.Log((object) string.Format("F1: {0}  F2: {1}  F3: {2}  F4: {3}  F5: {4}  F6: {5}  F7: {6}  F8: {7}  F9: {8}\nF10: {9}\n(CTRL and ALT will Show mulligan) -- SHIFT + CTRL = Hero on players side -- SHIFT + ALT = Hero on opponent side -- SHIFT + ALT + CTRL = Hero on both sides", (object) this.GetQuickPlayMissionShortName(KeyCode.F1), (object) this.GetQuickPlayMissionShortName(KeyCode.F2), (object) this.GetQuickPlayMissionShortName(KeyCode.F3), (object) this.GetQuickPlayMissionShortName(KeyCode.F4), (object) this.GetQuickPlayMissionShortName(KeyCode.F5), (object) this.GetQuickPlayMissionShortName(KeyCode.F6), (object) this.GetQuickPlayMissionShortName(KeyCode.F7), (object) this.GetQuickPlayMissionShortName(KeyCode.F8), (object) this.GetQuickPlayMissionShortName(KeyCode.F9), (object) this.GetQuickPlayMissionShortName(KeyCode.F10)));
  }

  private string GetQuickPlayMissionName(KeyCode keyCode) => this.GetQuickPlayMissionName((int) Cheats.s_quickPlayKeyMap[keyCode]);

  private string GetQuickPlayMissionShortName(KeyCode keyCode) => this.GetQuickPlayMissionShortName((int) Cheats.s_quickPlayKeyMap[keyCode]);

  private string GetQuickPlayMissionName(int missionId) => this.GetQuickPlayMissionNameImpl(missionId, "NAME");

  private string GetQuickPlayMissionShortName(int missionId) => this.GetQuickPlayMissionNameImpl(missionId, "SHORT_NAME");

  private string GetQuickPlayMissionNameImpl(int missionId, string columnName)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
    if (record != null)
    {
      DbfLocValue var = (DbfLocValue) record.GetVar(columnName);
      if (var != null)
        return var.GetString();
    }
    string playMissionNameImpl = missionId.ToString();
    try
    {
      playMissionNameImpl = ((ScenarioDbId) missionId).ToString();
    }
    catch (Exception ex)
    {
    }
    return playMissionNameImpl;
  }

  private Cheats.QuickLaunchAvailability GetQuickLaunchAvailability()
  {
    if (this.m_quickLaunchState.m_launching || SceneMgr.Get().IsInGame())
      return Cheats.QuickLaunchAvailability.ACTIVE_GAME;
    if (GameMgr.Get().IsFindingGame())
      return Cheats.QuickLaunchAvailability.FINDING_GAME;
    if (SceneMgr.Get().GetNextMode() != SceneMgr.Mode.INVALID || !SceneMgr.Get().IsSceneLoaded())
      return Cheats.QuickLaunchAvailability.SCENE_TRANSITION;
    if (LoadingScreen.Get().IsTransitioning())
      return Cheats.QuickLaunchAvailability.ACTIVE_GAME;
    return CollectionManager.Get() == null || !CollectionManager.Get().IsFullyLoaded() ? Cheats.QuickLaunchAvailability.COLLECTION_NOT_READY : Cheats.QuickLaunchAvailability.OK;
  }

  private void LaunchQuickGame(
    int missionId,
    GameType gameType = GameType.GT_VS_AI,
    PegasusShared.FormatType formatType = PegasusShared.FormatType.FT_WILD,
    CollectionDeck deck = null,
    string aiDeck = null,
    GameType progFilterOverride = GameType.GT_UNKNOWN)
  {
    string str = "";
    long num = 0;
    if (gameType != GameType.GT_BATTLEGROUNDS_PLAYER_VS_AI)
    {
      if (deck == null)
      {
        CollectionManager collectionManager = CollectionManager.Get();
        num = Options.Get().GetLong(Option.LAST_CUSTOM_DECK_CHOSEN);
        deck = collectionManager.GetDeck(num);
        if (deck == null)
        {
          TAG_CLASS defaultClass = TAG_CLASS.MAGE;
          List<CollectionDeck> decks = collectionManager.GetDecks(DeckType.NORMAL_DECK);
          deck = decks.Where<CollectionDeck>((Func<CollectionDeck, bool>) (x => x.GetClass() == defaultClass)).FirstOrDefault<CollectionDeck>();
          if (deck == null)
          {
            deck = decks.FirstOrDefault<CollectionDeck>();
            if (deck == null)
            {
              Debug.LogError((object) "Could not launch quick game because the account has no decks. Please add at least one deck to your account");
              return;
            }
          }
          num = deck.ID;
          str = deck.Name;
        }
        else
          str = deck.Name;
      }
      else
      {
        num = deck.ID;
        str = deck.Name;
      }
    }
    ReconnectMgr.Get().SetBypassReconnect(true);
    this.m_quickLaunchState.m_launching = true;
    string message = string.Format("Launching {0}\nDeck: {1}", (object) this.GetQuickPlayMissionName(missionId), (object) str);
    UIStatus.Get().AddInfo(message);
    TimeScaleMgr.Get().PushTemporarySpeedIncrease(4f);
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    GameMgr.Get().SetPendingAutoConcede(true);
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    GameMgr.Get().FindGame(gameType, formatType, missionId, deckId: num, aiDeck: aiDeck, progFilterOverride: progFilterOverride);
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode == SceneMgr.Mode.GAMEPLAY)
    {
      this.HideAlert();
      this.m_isInGameplayScene = true;
    }
    if (!this.m_isInGameplayScene || mode == SceneMgr.Mode.GAMEPLAY)
      return;
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    this.m_quickLaunchState = new Cheats.QuickLaunchState();
    this.m_isInGameplayScene = false;
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CANCELED:
        GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
        this.m_quickLaunchState = new Cheats.QuickLaunchState();
        break;
    }
    return false;
  }

  private JsonList GetCardlistJson(List<Card> list)
  {
    JsonList cardlistJson = new JsonList();
    for (int index = 0; index < list.Count; ++index)
    {
      JsonNode cardJson = this.GetCardJson(list[index].GetEntity());
      cardlistJson.Add((object) cardJson);
    }
    return cardlistJson;
  }

  private JsonNode GetCardJson(Entity card)
  {
    if (card == null)
      return (JsonNode) null;
    JsonNode cardJson1 = new JsonNode();
    cardJson1["cardName"] = (object) card.GetName();
    cardJson1["cardID"] = (object) card.GetCardId();
    cardJson1["entityID"] = (object) (long) card.GetEntityId();
    JsonList jsonList1 = new JsonList();
    if (card.GetTags() != null)
    {
      foreach (KeyValuePair<int, int> keyValuePair in card.GetTags().GetMap())
      {
        JsonNode jsonNode = new JsonNode();
        string key = System.Enum.GetName(typeof (GAME_TAG), (object) keyValuePair.Key) ?? "NOTAG_" + keyValuePair.Key.ToString();
        jsonNode[key] = (object) (long) keyValuePair.Value;
        jsonList1.Add((object) jsonNode);
      }
      cardJson1["tags"] = (object) jsonList1;
    }
    JsonList jsonList2 = new JsonList();
    List<Entity> enchantments = card.GetEnchantments();
    for (int index = 0; index < enchantments.Count<Entity>(); ++index)
    {
      JsonNode cardJson2 = this.GetCardJson(enchantments[index]);
      jsonList2.Add((object) cardJson2);
    }
    cardJson1["enchantments"] = (object) jsonList2;
    return cardJson1;
  }

  private bool OnProcessCheat_error(string func, string[] args, string rawArgs)
  {
    int num = args.Length == 0 ? 0 : (args[0] == "ex" || "except".Equals(args[0], StringComparison.InvariantCultureIgnoreCase) ? 1 : ("exception".Equals(args[0], StringComparison.InvariantCultureIgnoreCase) ? 1 : 0));
    bool flag = args.Length != 0 && (args[0] == "f" || "fatal".Equals(args[0], StringComparison.InvariantCultureIgnoreCase));
    string str = args.Length <= 1 ? (string) null : string.Join(" ", ((IEnumerable<string>) args).Skip<string>(1).ToArray<string>());
    if (num != 0)
    {
      str = str == null ? "This is a simulated Exception." : throw new Exception(str);
    }
    else
    {
      if (flag)
      {
        if (str == null)
          str = "This is a simulated Fatal Error.";
        Error.AddFatal(FatalErrorReason.CHEAT, str);
      }
      else
      {
        if (str == null)
          str = "This is a simulated Warning message.";
        Error.AddWarning("Warning", str);
      }
      return true;
    }
  }

  public static bool ProcessAutofillParam(
    IEnumerable<string> values,
    string searchTerm,
    AutofillData autofillData)
  {
    values = (IEnumerable<string>) values.OrderBy<string, string>((Func<string, string>) (v => v));
    string prefix = autofillData.m_lastAutofillParamPrefix ?? searchTerm ?? string.Empty;
    List<string> stringList = !string.IsNullOrEmpty(prefix.Trim()) ? values.Where<string>((Func<string, bool>) (v => v.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))).ToList<string>() : values.ToList<string>();
    int index = 0;
    if (autofillData.m_lastAutofillParamMatch != null)
    {
      index = stringList.IndexOf(autofillData.m_lastAutofillParamMatch);
      if (index >= 0)
      {
        index += autofillData.m_isShiftTab ? -1 : 1;
        if (index >= stringList.Count)
          index = 0;
        else if (index < 0)
          index = stringList.Count - 1;
      }
    }
    if (index < 0)
      index = 0;
    else if (index >= stringList.Count)
    {
      autofillData.m_lastAutofillParamPrefix = (string) null;
      autofillData.m_lastAutofillParamMatch = (string) null;
      float delay = (5f + Mathf.Max(0.0f, (float) (stringList.Count - 3))) * UnityEngine.Time.timeScale;
      string str = string.Join("   ", values.ToArray<string>());
      UIStatus.Get().AddError(string.Format("No match for '{0}'. Available params:\n{1}", (object) searchTerm, (object) str), delay);
      return false;
    }
    autofillData.m_lastAutofillParamPrefix = prefix;
    autofillData.m_lastAutofillParamMatch = stringList[index];
    if (stringList.Count > 0)
    {
      float delay = (5f + Mathf.Max(0.0f, (float) (stringList.Count - 3))) * UnityEngine.Time.timeScale;
      string str = string.Join("   ", stringList.ToArray());
      UIStatus.Get().AddInfoNoRichText("Available params:\n" + str, delay);
    }
    return true;
  }

  private bool OnProcessCheat_HasOption(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string str = args[0];
    if (autofillData != null)
      return args.Length == 1 && Cheats.ProcessAutofillParam(System.Enum.GetValues(typeof (Option)).Cast<Option>().Select<Option, string>((Func<Option, string>) (v => Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(v))), str, autofillData);
    Option option;
    try
    {
      option = Blizzard.T5.Core.Utils.EnumUtils.GetEnum<Option>(str, StringComparison.OrdinalIgnoreCase);
    }
    catch (ArgumentException ex)
    {
      return false;
    }
    string message = string.Format("HasOption: {0} = {1}", (object) Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(option), (object) Options.Get().HasOption(option));
    Debug.Log((object) message);
    UIStatus.Get().AddInfo(message);
    return true;
  }

  private bool OnProcessCheat_GetOption(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string str = args[0];
    if (autofillData != null)
      return args.Length == 1 && Cheats.ProcessAutofillParam(System.Enum.GetValues(typeof (Option)).Cast<Option>().Select<Option, string>((Func<Option, string>) (v => Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(v))), str, autofillData);
    Option option;
    try
    {
      option = Blizzard.T5.Core.Utils.EnumUtils.GetEnum<Option>(str, StringComparison.OrdinalIgnoreCase);
    }
    catch (ArgumentException ex)
    {
      return false;
    }
    string message = string.Format("GetOption: {0} = {1}", (object) Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(option), Options.Get().GetOption(option));
    Debug.Log((object) message);
    UIStatus.Get().AddInfo(message);
    return true;
  }

  private bool OnProcessCheat_SetOption(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string str1 = args[0];
    if (autofillData != null)
      return args.Length == 1 && Cheats.ProcessAutofillParam(System.Enum.GetValues(typeof (Option)).Cast<Option>().Select<Option, string>((Func<Option, string>) (v => Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(v))), str1, autofillData);
    Option option;
    try
    {
      option = Blizzard.T5.Core.Utils.EnumUtils.GetEnum<Option>(str1, StringComparison.OrdinalIgnoreCase);
    }
    catch (ArgumentException ex)
    {
      return false;
    }
    if (args.Length < 2)
      return false;
    string str2 = Options.Get().HasOption(option) ? Options.Get().GetOption(option).ToString() : "<null>";
    string str3 = Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(option);
    string str4 = args[1];
    System.Type optionType = Options.Get().GetOptionType(option);
    if (optionType == typeof (bool))
    {
      bool boolVal;
      if (!GeneralUtils.TryParseBool(str4, out boolVal))
        return false;
      Options.Get().SetBool(option, boolVal);
    }
    else if (optionType == typeof (int))
    {
      int val;
      if (!GeneralUtils.TryParseInt(str4, out val))
        return false;
      Options.Get().SetInt(option, val);
    }
    else if (optionType == typeof (long))
    {
      long val;
      if (!GeneralUtils.TryParseLong(str4, out val))
        return false;
      Options.Get().SetLong(option, val);
    }
    else if (optionType == typeof (float))
    {
      float val;
      if (!GeneralUtils.TryParseFloat(str4, out val))
        return false;
      Options.Get().SetFloat(option, val);
    }
    else if (optionType == typeof (string))
    {
      str4 = rawArgs.Remove(0, str1.Length + 1);
      Options.Get().SetString(option, str4);
    }
    else
    {
      string message = string.Format("SetOption: {0} has unsupported underlying type {1}", (object) str3, (object) optionType);
      UIStatus.Get().AddError(message);
      return true;
    }
    switch (option)
    {
      case Option.CURSOR:
        Cursor.visible = Options.Get().GetBool(Option.CURSOR);
        break;
      case Option.GFX_TARGET_FRAME_RATE:
        ServiceManager.Get<IGraphicsManager>().UpdateTargetFramerate(Options.Get().GetInt(Option.GFX_TARGET_FRAME_RATE));
        break;
    }
    string str5 = Options.Get().HasOption(option) ? Options.Get().GetOption(option).ToString() : "<null>";
    string message1 = string.Format("SetOption: {0} to {1}.\nPrevious value: {2}\nNew GetOption: {3}", (object) str3, (object) str4, (object) str2, (object) str5);
    Debug.Log((object) message1);
    NetCache.Get().DispatchClientOptionsToServer();
    UIStatus.Get().AddInfo(message1);
    return true;
  }

  private bool OnProcessCheat_GetVar(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string str = args[0];
    if (autofillData != null)
      return args.Length == 1 && Cheats.ProcessAutofillParam(Vars.AllKeys, str, autofillData);
    string message = string.Format("Var: {0} = {1}", (object) str, (object) (Vars.Key(str).GetStr((string) null) ?? "(null)"));
    Debug.Log((object) message);
    UIStatus.Get().AddInfo(message);
    return true;
  }

  private bool OnProcessCheat_SetVar(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string str1 = args[0];
    if (autofillData != null)
      return args.Length == 1 && Cheats.ProcessAutofillParam(Vars.AllKeys, str1, autofillData);
    string str2 = args.Length < 2 ? (string) null : args[1];
    Vars.Key(str1).Set(str2, false);
    string message = string.Format("Var: {0} = {1}", (object) str1, (object) (str2 ?? "(null)"));
    Debug.Log((object) message);
    UIStatus.Get().AddInfo(message);
    if (str1.Equals("Arena.AutoDraft", StringComparison.InvariantCultureIgnoreCase) && (UnityEngine.Object) DraftDisplay.Get() != (UnityEngine.Object) null)
      DraftDisplay.Get().StartCoroutine(DraftDisplay.Get().RunAutoDraftCheat());
    return true;
  }

  private bool OnProcessCheat_autodraft(string func, string[] args, string rawArgs)
  {
    string strVal = args[0];
    bool flag = string.IsNullOrEmpty(strVal) || GeneralUtils.ForceBool(strVal);
    Vars.Key("Arena.AutoDraft").Set(flag ? "true" : "false", false);
    if (flag && (UnityEngine.Object) DraftDisplay.Get() != (UnityEngine.Object) null)
    {
      TimeScaleMgr.Get().PushTemporarySpeedIncrease(4f);
      DraftDisplay.Get().StartCoroutine(DraftDisplay.Get().RunAutoDraftCheat());
    }
    else if (!flag)
    {
      double num = (double) TimeScaleMgr.Get().PopTemporarySpeedIncrease();
    }
    string message = string.Format("Arena autodraft turned {0}.", flag ? (object) "on" : (object) "off");
    Debug.Log((object) message);
    UIStatus.Get().AddInfo(message);
    return true;
  }

  private bool OnProcessCheat_HeroCount(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    try
    {
      int result;
      int.TryParse(args[0], out result);
      switch (SceneMgr.Get().GetMode())
      {
        case SceneMgr.Mode.COLLECTIONMANAGER:
          HeroPickerDisplay.Get().CheatLoadHeroButtons(result);
          break;
        case SceneMgr.Mode.ADVENTURE:
          GuestHeroPickerTrayDisplay.Get().CheatLoadHeroButtons(result);
          break;
        case SceneMgr.Mode.TAVERN_BRAWL:
          DeckPickerTrayDisplay.Get().CheatLoadHeroButtons(result);
          break;
        default:
          return false;
      }
    }
    catch (ArgumentException ex)
    {
      return false;
    }
    return true;
  }

  private bool OnProcessCheat_onlygold(string func, string[] args, string rawArgs)
  {
    string lowerInvariant = args[0].ToLowerInvariant();
    if (!(lowerInvariant == "gold") && !(lowerInvariant == "normal") && !(lowerInvariant == "standard"))
    {
      if (lowerInvariant == "both")
      {
        Options.Get().DeleteOption(Option.COLLECTION_PREMIUM_TYPE);
      }
      else
      {
        UIStatus.Get().AddError("Unknown cmd: " + (string.IsNullOrEmpty(lowerInvariant) ? "(blank)" : lowerInvariant) + "\nValid cmds: gold, standard, both");
        return false;
      }
    }
    else
      Options.Get().SetString(Option.COLLECTION_PREMIUM_TYPE, lowerInvariant);
    return true;
  }

  private bool OnProcessCheat_navigation(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
      return true;
    string[] values = new string[7]
    {
      "debug",
      "dump",
      "back",
      "pop",
      "stack",
      "history",
      "show"
    };
    string lowerInvariant = args[0].ToLowerInvariant();
    if (autofillData != null)
      return HearthstoneApplication.IsInternal() && Cheats.ProcessAutofillParam((IEnumerable<string>) values, lowerInvariant, autofillData);
    switch (lowerInvariant)
    {
      case "back":
      case "pop":
        if (!HearthstoneApplication.IsInternal())
          return false;
        if (!Navigation.CanGoBack)
        {
          string str = Navigation.IsEmpty ? " Stack is empty." : string.Empty;
          UIStatus.Get().AddInfo("Cannot go back at this time." + str);
          return true;
        }
        Navigation.GoBack();
        break;
      case "debug":
        Navigation.NAVIGATION_DEBUG = args.Length < 2 || GeneralUtils.ForceBool(args[1]);
        if (Navigation.NAVIGATION_DEBUG)
        {
          Navigation.DumpStack();
          UIStatus.Get().AddInfo("Navigation debugging turned on - see Console or output log for nav dump.");
          break;
        }
        UIStatus.Get().AddInfo("Navigation debugging turned off.");
        break;
      case "dump":
        Navigation.DumpStack();
        UIStatus.Get().AddInfo("Navigation dumped, see Console or output log.");
        break;
      case "history":
      case "show":
      case "stack":
        if (!HearthstoneApplication.IsInternal())
          return false;
        string stackDumpString = Navigation.StackDumpString;
        float delay = (float) (5 + 3 * stackDumpString.Count<char>((Func<char, bool>) (c => c == '\n'))) * UnityEngine.Time.timeScale;
        UIStatus.Get().AddInfo(Navigation.IsEmpty ? "Stack is empty." : stackDumpString, delay);
        break;
      default:
        string message = "Unknown cmd: " + (string.IsNullOrEmpty(lowerInvariant) ? "(blank)" : lowerInvariant);
        if (HearthstoneApplication.IsInternal())
          message = message + "\nValid cmds: " + string.Join(", ", values);
        UIStatus.Get().AddError(message);
        break;
    }
    return true;
  }

  private bool OnProcessCheat_DeleteOption(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string str1 = args[0];
    if (autofillData != null)
      return args.Length == 1 && Cheats.ProcessAutofillParam(System.Enum.GetValues(typeof (Option)).Cast<Option>().Select<Option, string>((Func<Option, string>) (v => Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(v))), str1, autofillData);
    Option option;
    try
    {
      option = Blizzard.T5.Core.Utils.EnumUtils.GetEnum<Option>(str1, StringComparison.OrdinalIgnoreCase);
    }
    catch (ArgumentException ex)
    {
      return false;
    }
    string str2 = Options.Get().HasOption(option) ? Options.Get().GetOption(option).ToString() : "<null>";
    Options.Get().DeleteOption(option);
    string str3 = Options.Get().HasOption(option) ? Options.Get().GetOption(option).ToString() : "<null>";
    string message = string.Format("DeleteOption: {0}\nPrevious Value: {1}\nNew Value: {2}", (object) Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(option), (object) str2, (object) str3);
    Debug.Log((object) message);
    UIStatus.Get().AddInfo(message);
    return true;
  }

  private bool OnProcessCheat_collectionfirstxp(string func, string[] args, string rawArgs)
  {
    Options.Get().SetInt(Option.COVER_MOUSE_OVERS, 0);
    Options.Get().SetInt(Option.PAGE_MOUSE_OVERS, 0);
    return true;
  }

  private bool OnProcessCheat_board(string func, string[] args, string rawArgs)
  {
    int result = 0;
    this.m_boardId = int.TryParse(args[0], out result) ? result : 0;
    UIStatus.Get().AddInfo(string.Format("Board for next game set to id {0}.", (object) this.m_boardId));
    return true;
  }

  private bool OnProcessCheat_playerTags(string func, string[] args, string rawArgs)
  {
    this.TryParsePlayerTags(args[0], out this.m_playerTags);
    if (PartyManager.Get().IsInBattlegroundsParty() && !string.IsNullOrEmpty(this.m_playerTags))
      PartyManager.Get().SetMyPlayerTagsAttribute();
    return true;
  }

  private bool OnProcessCheat_speechBubbles(string func, string[] args, string rawArgs)
  {
    this.m_speechBubblesEnabled = !this.m_speechBubblesEnabled;
    UIStatus.Get().AddInfo(string.Format("Speech bubbles {0}.", this.m_speechBubblesEnabled ? (object) "enabled" : (object) "disabled"));
    return true;
  }

  private bool OnProcessCheat_playAllThinkEmotes(string func, string[] args, string rawArgs)
  {
    if (args.Length != 1)
    {
      UIStatus.Get().AddError("Invalid params for " + func);
      Log.Gameplay.PrintError("Unrecognized number of arguments. Expected \"" + func + " <player>\"");
      return false;
    }
    string lower = args[0].ToLower();
    int id;
    if (!(lower == "1") && !(lower == "friendly"))
    {
      if (lower == "2" || lower == "opponent")
      {
        id = 2;
      }
      else
      {
        UIStatus.Get().AddError("Invalid params for " + func);
        Log.Gameplay.PrintError("Unrecognized player: \"" + args[0] + "\". Expected \"1\", \"2\", \"friendly\", or \"opponent\"");
        return false;
      }
    }
    else
      id = 1;
    Entity hero = GameState.Get()?.GetPlayer(id)?.GetHero();
    if (hero == null)
    {
      Log.Gameplay.PrintError(string.Format("Unable to find Hero for player {0}", (object) id));
      return false;
    }
    Processor.RunCoroutine(this.PlayEmotesInOrder(hero.GetCard(), EmoteType.THINK1, EmoteType.THINK2, EmoteType.THINK3));
    return true;
  }

  private IEnumerator PlayEmotesInOrder(Card heroCard, params EmoteType[] emoteTypes)
  {
    if (!((UnityEngine.Object) heroCard == (UnityEngine.Object) null) && emoteTypes != null)
    {
      for (int i = 0; i < emoteTypes.Length; ++i)
      {
        if (heroCard.GetEmoteEntry(emoteTypes[i]) == null)
        {
          string str = string.Format("Unable to locate {0} emote for {1}", (object) emoteTypes[i], (object) heroCard);
          UIStatus.Get().AddError(str);
          Log.Gameplay.PrintError(str);
        }
        else
        {
          heroCard.PlayEmote(emoteTypes[i]);
          if (i < emoteTypes.Length - 1)
            yield return (object) new WaitForSeconds(5f);
        }
      }
    }
  }

  private bool OnProcessCheat_playEmote(string func, string[] args, string rawArgs)
  {
    if (args.Length != 1 && args.Length != 2)
    {
      UIStatus.Get().AddError("Provide 1 to 2 params for " + func + ".");
      Log.Gameplay.PrintError("Unrecognized number of arguments. Expected \"" + func + " <enum_type> <player>\"");
      return true;
    }
    EmoteType result1 = EmoteType.INVALID;
    System.Enum.TryParse<EmoteType>(args[0], true, out result1);
    if (!System.Enum.IsDefined(typeof (EmoteType), (object) result1) || result1 == EmoteType.INVALID)
    {
      if (GameMgr.Get().IsBattlegrounds())
      {
        int result2 = 0;
        int.TryParse(args[0], out result2);
        if (result2 >= 101 && result2 <= 119)
        {
          GameState.Get().GetGameEntity().SendCustomEvent(result2);
          return true;
        }
      }
      Array names = (Array) System.Enum.GetNames(typeof (EmoteType));
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      foreach (string str in names)
      {
        if (num != 0)
        {
          stringBuilder.Append(num);
          stringBuilder.Append(" = ");
          stringBuilder.Append(str);
          stringBuilder.Append('\n');
        }
        ++num;
      }
      string str1 = stringBuilder.ToString();
      UIStatus.Get().AddError("Invalid first param for " + func + ". See \"Messages\".");
      Log.Gameplay.PrintError("Unrecognized <enum_type>.\nFor Battlegrounds, try a num [101-119]. Some don't play every time you call it.\n" + string.Format("Try a num [1-{0}] or a string:\n", (object) (names.Length - 1)) + str1);
      return true;
    }
    int result3 = 1;
    if (args.Length == 2)
    {
      if (GameMgr.Get().IsBattlegrounds())
      {
        int.TryParse(args[1], out result3);
        GameState.Get().GetGameEntity().PlayAlternateEnemyEmote(result3, result1);
        return true;
      }
      string lower = args[1].ToLower();
      if (!(lower == "1") && !(lower == "friendly"))
      {
        if (lower == "2" || lower == "opponent")
        {
          result3 = 2;
        }
        else
        {
          UIStatus.Get().AddError("Invalid second param for " + func + ". See \"Messages\".");
          Log.Gameplay.PrintError("Unrecognized player: \"" + args[1] + "\". Expected \"1\", \"2\", \"friendly\", or \"opponent\"");
          return true;
        }
      }
      else
        result3 = 1;
    }
    Card card = GameState.Get()?.GetPlayer(result3)?.GetHero()?.GetCard();
    if ((UnityEngine.Object) card == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("Unable to find Hero for current player");
      return false;
    }
    card.PlayEmote(result1);
    return true;
  }

  private bool OnProcessCheat_playAllMissionHeroPowerLines(
    string func,
    string[] args,
    string rawArgs)
  {
    if (args.Length > 1 || args[0] != string.Empty)
    {
      UIStatus.Get().AddError("Invalid params for " + func);
      Log.Gameplay.PrintError("Unrecognized number of arguments. Expected 0 arguments.");
      return false;
    }
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    if (gameEntity == null)
      return false;
    string name = "GetBossHeroPowerRandomLines";
    MethodInfo method = gameEntity.GetType().GetMethod(name);
    if (method == (MethodInfo) null)
    {
      Log.Gameplay.PrintError("This game mode lacks hero power lines.");
      return false;
    }
    if (!(method.Invoke((object) gameEntity, (object[]) null) is List<string> assets))
      return false;
    Gameplay.Get().StartCoroutine(this.LoadAndPlayVO(assets));
    return true;
  }

  private bool OnProcessCheat_playAllMissionIdleLines(string func, string[] args, string rawArgs)
  {
    if (args.Length > 1 || args[0] != string.Empty)
    {
      UIStatus.Get().AddError("Invalid params for " + func);
      Log.Gameplay.PrintError("Unrecognized number of arguments. Expected 0 arguments.");
      return false;
    }
    GameEntity gameEntity = GameState.Get().GetGameEntity();
    if (gameEntity == null)
      return false;
    string name = "GetIdleLines";
    MethodInfo method = gameEntity.GetType().GetMethod(name);
    if (method == (MethodInfo) null)
    {
      Log.Gameplay.PrintError("This game mode lacks idle lines.");
      return false;
    }
    if (!(method.Invoke((object) gameEntity, (object[]) null) is List<string> assets))
      return false;
    Gameplay.Get().StartCoroutine(this.LoadAndPlayVO(assets));
    return true;
  }

  private bool OnProcessCheat_playLegendaryHeroVFX(string func, string[] args, string rawArgs)
  {
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    if (friendlySidePlayer == null)
    {
      Log.Gameplay.PrintError("Player doesn't exist.  Make sure to run from within battlegrounds match");
      return false;
    }
    Entity hero = friendlySidePlayer.GetHero();
    if (hero == null)
    {
      Log.Gameplay.PrintError("Hero doesn't exist.  Make sure to run from within battlegrounds match");
      return false;
    }
    Card card = hero.GetCard();
    BaconLHSConfig baconLhsConfig = (BaconLHSConfig) null;
    if ((UnityEngine.Object) card != (UnityEngine.Object) null)
      baconLhsConfig = card.LegendaryHeroSkinConfig;
    if ((UnityEngine.Object) baconLhsConfig == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("Unable to load legendary hero skin config");
      return false;
    }
    if (args.Length == 0)
    {
      Log.Gameplay.PrintError("playLegendaryHeroVFX must have at least one argument");
      return false;
    }
    string lower = args[0].ToLower();
    if (!(lower == "socketin"))
    {
      if (!(lower == "combatstart"))
      {
        if (lower == "winstreak")
        {
          int result;
          if (int.TryParse(args[1], out result))
          {
            if (baconLhsConfig.TryActivateVFX_WinStreak(result))
              return true;
            Log.Gameplay.PrintError(string.Format("Could not activate winstreak with id {0}", (object) result));
            return false;
          }
          Log.Gameplay.PrintError("second argument of winstreak not a number:  " + args[1]);
          return false;
        }
        Log.Gameplay.PrintError("invalid vfx " + args[0] + ".  valid vfx options are socketin, combatstart, winstreak");
        return false;
      }
      if (baconLhsConfig.TryActivateVFX_CombatStart())
        return true;
      Log.Gameplay.PrintError("Could not play combat start in VFX");
      return false;
    }
    if (baconLhsConfig.TryActivateVFX_SocketIn())
      return true;
    Log.Gameplay.PrintError("Could not play socket in VFX");
    return false;
  }

  private bool OnProcessCheat_playLegendaryHeroVO(string func, string[] args, string rawArgs)
  {
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    if (friendlySidePlayer == null)
    {
      Log.Gameplay.PrintError("Player doesn't exist.  Make sure to run from within battlegrounds match");
      return false;
    }
    Entity hero = friendlySidePlayer.GetHero();
    if (hero == null)
    {
      Log.Gameplay.PrintError("Hero doesn't exist.  Make sure to run from within battlegrounds match");
      return false;
    }
    Card card = hero.GetCard();
    BaconLHSConfig baconLhsConfig = (BaconLHSConfig) null;
    if ((UnityEngine.Object) card != (UnityEngine.Object) null)
      baconLhsConfig = card.LegendaryHeroSkinConfig;
    if ((UnityEngine.Object) baconLhsConfig == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("Unable to load legendary hero skin config");
      return false;
    }
    List<string> assets = new List<string>();
    if (args.Length == 0 || args[0] == "")
      assets = baconLhsConfig.GetAllVOLines();
    string lower = args[0].ToLower();
    if (!(lower == "picked"))
    {
      if (!(lower == "startgame"))
      {
        if (!(lower == "winstreak"))
        {
          if (!(lower == "greet"))
          {
            if (!(lower == "bartendergreet"))
            {
              if (lower == "herogreet")
              {
                if (args.Length == 1)
                {
                  foreach (BaconLHSConfig.CardSpecificLine cardSpecificLine in baconLhsConfig.m_VOHeroGreet)
                    assets.Add(cardSpecificLine.m_VOLine);
                }
                else
                {
                  List<string> voLines;
                  if (baconLhsConfig.TryGetAllHeroGreet(args[1], out voLines))
                  {
                    if (args.Length == 2)
                    {
                      assets = voLines;
                    }
                    else
                    {
                      int result;
                      if (int.TryParse(args[2], out result))
                      {
                        if (result >= 0 && result < voLines.Count)
                        {
                          assets.Add(voLines[result]);
                        }
                        else
                        {
                          Log.Gameplay.PrintError(string.Format("attempt to access herogreet VO number {0} for hero {1} but max is {2}", (object) args[2], (object) args[1], (object) (voLines.Count - 1)));
                          return false;
                        }
                      }
                      else
                      {
                        Log.Gameplay.PrintError("third argument of herogreet not a number:  " + args[2]);
                        return false;
                      }
                    }
                  }
                  else
                  {
                    Log.Gameplay.PrintError("second argument of herogreet not in dictionary:  " + args[1]);
                    return false;
                  }
                }
              }
              else
                Log.Gameplay.PrintError("invalid sfx " + args[0] + ".  valid sfx options are picked, startgame, winstreak, greet, bartendergreet, herogreet");
            }
            else if (args.Length == 1)
            {
              foreach (BaconLHSConfig.CardSpecificLine cardSpecificLine in baconLhsConfig.m_VOBartenderGreet)
                assets.Add(cardSpecificLine.m_VOLine);
            }
            else
            {
              List<string> voLines;
              if (baconLhsConfig.TryGetAllBartenderGreet(args[1], out voLines))
              {
                if (args.Length == 2)
                {
                  assets = voLines;
                }
                else
                {
                  int result;
                  if (int.TryParse(args[2], out result))
                  {
                    if (result >= 0 && result < voLines.Count)
                    {
                      assets.Add(voLines[result]);
                    }
                    else
                    {
                      Log.Gameplay.PrintError(string.Format("attempt to access bartendergreet VO number {0} for bartender {1} but max is {2}", (object) args[2], (object) args[1], (object) (voLines.Count - 1)));
                      return false;
                    }
                  }
                  else
                  {
                    Log.Gameplay.PrintError("third argument of bartendergreet not a number:  " + args[2]);
                    return false;
                  }
                }
              }
              else
              {
                Log.Gameplay.PrintError("second argument of bartendergreet not in dictionary:  " + args[1]);
                return false;
              }
            }
          }
          else if (args.Length == 1)
          {
            assets = baconLhsConfig.m_VOGreet;
          }
          else
          {
            int result;
            if (int.TryParse(args[1], out result))
            {
              if (result >= 0 && result < baconLhsConfig.m_VOGreet.Count)
              {
                assets.Add(baconLhsConfig.m_VOGreet[result]);
              }
              else
              {
                Log.Gameplay.PrintError(string.Format("attempt to access greet VO number {0} but max is {1}", (object) args[1], (object) (baconLhsConfig.m_VOGreet.Count - 1)));
                return false;
              }
            }
            else
            {
              Log.Gameplay.PrintError("second argument of greet not a number:  " + args[1]);
              return false;
            }
          }
        }
        else if (args.Length == 1)
        {
          foreach (BaconLHSConfig.ValueLine valueLine in baconLhsConfig.m_VOWinStreak)
            assets.Add(valueLine.m_VOLine);
        }
        else
        {
          int result;
          if (int.TryParse(args[1], out result))
          {
            string line;
            if (baconLhsConfig.CheckWinStreakLine(result, out line))
            {
              assets.Add(line);
            }
            else
            {
              Log.Gameplay.PrintError("No VO available for winstreak of " + args[1]);
              return false;
            }
          }
          else
          {
            Log.Gameplay.PrintError("second argument of winstreak not a number:  " + args[1]);
            return false;
          }
        }
      }
      else
        assets.Add(baconLhsConfig.m_VOStartOfGame);
    }
    else
      assets.Add(baconLhsConfig.m_VOPicked);
    Gameplay.Get().StartCoroutine(this.LoadAndPlayVO(assets, 3f));
    return true;
  }

  private bool OnProcessCheat_playBattlegroundsGuideVO(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    if (autofillData != null)
    {
      if ((!rawArgs.EndsWith(" ") ? 0 : (args.Length == 1 ? 1 : 0)) == 0 && args.Length != 2)
        return false;
      string searchTerm = args.Length == 1 ? string.Empty : args[1];
      return Cheats.ProcessAutofillParam((IEnumerable<string>) System.Enum.GetNames(typeof (BaconGuideConfig.HumanReadableVOLineCategory)), searchTerm, autofillData);
    }
    if (args.Length > 3 || args.Length < 2)
    {
      UIStatus.Get().AddError("Invalid params for " + func);
      Log.Gameplay.PrintError("Unrecognized number of arguments. Expected 2 or 3 arguments.");
      return false;
    }
    BaconGuideConfig baconGuideConfig = TB_BaconShop.LoadGuideConfig(args[0]);
    if ((UnityEngine.Object) null == (UnityEngine.Object) baconGuideConfig)
    {
      Log.Gameplay.PrintError("Unable to load guide config for " + args[0]);
      return false;
    }
    List<string> assets = baconGuideConfig.GetLinesByHumanReadableName(args[1]);
    if (assets.Count == 0)
    {
      Log.Gameplay.PrintError("No VO lines found for category " + args[1]);
      return false;
    }
    if (((IEnumerable<string>) args).Count<string>() == 3)
    {
      int result = 0;
      if (!int.TryParse(args[2], out result))
      {
        Log.Gameplay.PrintError("Unable to parse index from third argument " + args[2]);
        return false;
      }
      --result;
      if (result < 0 || result >= assets.Count)
      {
        Log.Gameplay.PrintError("Invalid index in third argument " + args[2]);
        return false;
      }
      string str = assets[result];
      assets = new List<string>();
      assets.Add(str);
    }
    Gameplay.Get().StartCoroutine(this.LoadAndPlayVO(assets));
    return true;
  }

  private IEnumerator LoadAndPlayVO(List<string> assets, float delayBetweenVo = 10f)
  {
    Cheats cheats = this;
    if (assets != null && assets.Count != 0)
    {
      foreach (string asset in assets)
      {
        if (SoundLoader.LoadSound((AssetReference) asset, new PrefabCallback<GameObject>(cheats.OnVoLoaded)))
        {
          if (asset != assets.Last<string>())
            yield return (object) new WaitForSeconds(delayBetweenVo);
        }
        else
        {
          string str = "Error loading asset " + asset.ToString();
          Log.Gameplay.PrintError(str);
          UIStatus.Get().AddError(str);
        }
      }
    }
  }

  private void OnVoLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null || string.IsNullOrEmpty((string) assetRef))
      return;
    Debug.LogFormat("Now playing \"{0}\"", (object) assetRef.ToString());
    AudioSource component = go.GetComponent<AudioSource>();
    SoundManager.Get().PlayPreloaded(component);
    string[] strArray = assetRef.ToString().Split(':');
    string key = strArray[0].Substring(0, strArray[0].Length - ".prefab".Length);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    NotificationManager notificationManager = NotificationManager.Get();
    notificationManager.DestroyNotification(notificationManager.CreateSpeechBubble(GameStrings.Get(key), Notification.SpeechBubbleDirection.TopRight, actor, false), component.clip.length);
  }

  private bool OnProcessCheat_audioChannel(string func, string[] args, string rawArgs)
  {
    if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (Global.SoundCategory key in System.Enum.GetValues(typeof (Global.SoundCategory)))
        stringBuilder.Append(string.Format("\n{0}: {1}", (object) key, !this.m_audioChannelEnabled.ContainsKey(key) || this.m_audioChannelEnabled[key] ? (object) "enabled" : (object) "disabled"));
      UIStatus.Get().AddInfo(string.Format("Audio channels:{0}", (object) stringBuilder.ToString()), 5f);
      return true;
    }
    if (args.Length > 2)
    {
      UIStatus.Get().AddError(string.Format("Argument format: [audio channel name] [on/off]"));
      return true;
    }
    try
    {
      Global.SoundCategory soundCategory = (Global.SoundCategory) System.Enum.Parse(typeof (Global.SoundCategory), args[0], true);
      if (args.Length == 1 || string.IsNullOrEmpty(args[1]))
      {
        UIStatus.Get().AddInfo(string.Format("Audio channel {0} is {1}", (object) soundCategory, this.m_audioChannelEnabled[soundCategory] ? (object) "on" : (object) "off"));
        return true;
      }
      if (args[1].ToLower() != "on" && args[1].ToLower() != "off")
      {
        UIStatus.Get().AddError(string.Format("Second argument must be \"on\" or \"off\""));
        return true;
      }
      this.m_audioChannelEnabled[soundCategory] = args[1].ToLower() == "on";
      SoundManager.Get().UpdateCategoryVolume(soundCategory);
      UIStatus.Get().AddInfo(string.Format("Audio channel {0} has been {1}", (object) soundCategory, this.m_audioChannelEnabled[soundCategory] ? (object) "enabled" : (object) "disabled"));
    }
    catch (ArgumentException ex)
    {
      UIStatus.Get().AddError(string.Format("{0} is not an audio channel. Type audiochannel to see a list of channels.", (object) args[0]));
    }
    return true;
  }

  private bool OnProcessCheat_audioChannelGroup(string func, string[] args, string rawArgs)
  {
    if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
    {
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string key in this.m_audioChannelGroups.Keys)
        stringBuilder.Append(string.Format("\n{0}", (object) key));
      UIStatus.Get().AddInfo(string.Format("Audio channel groups:{0}", (object) stringBuilder.ToString()), 5f);
      return true;
    }
    if (args.Length != 2)
    {
      UIStatus.Get().AddError(string.Format("Argument format: [audio channel group name] [on/off]"));
      return true;
    }
    if (!this.m_audioChannelGroups.ContainsKey(args[0].ToUpper()))
    {
      UIStatus.Get().AddError(string.Format("{0} is not an audio channel group. Type audiochannelgroup to see a list of channel groups.", (object) args[0]));
      return true;
    }
    if (args[1].ToLower() != "on" && args[1].ToLower() != "off")
    {
      UIStatus.Get().AddError(string.Format("Second argument must be \"on\" or \"off\""));
      return true;
    }
    foreach (Global.SoundCategory soundCategory in this.m_audioChannelGroups[args[0].ToUpper()])
    {
      if (this.m_audioChannelEnabled.ContainsKey(soundCategory))
      {
        this.m_audioChannelEnabled[soundCategory] = args[1].ToLower() == "on";
        SoundManager.Get().UpdateCategoryVolume(soundCategory);
      }
    }
    UIStatus.Get().AddInfo(string.Format("Audio channel group {0} has been {1}", (object) args[0], args[1].ToLower() == "on" ? (object) "enabled" : (object) "disabled"));
    return true;
  }

  private bool OnProcessCheat_tracert(string func, string[] args, string rawArgs)
  {
    string host = string.Empty;
    if (args.Length < 1 || string.IsNullOrEmpty(rawArgs))
    {
      if (Network.Get() != null)
      {
        GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
        if (gameServerJoined != null)
          host = gameServerJoined.Address;
      }
      if (string.IsNullOrEmpty(host))
      {
        UIStatus.Get().AddError("No host is defined yet! Please make a game first or set host argument!");
        return true;
      }
    }
    else
      host = args[0];
    if (host.Equals("help"))
    {
      UIStatus.Get().AddInfo("USAGE: tracert [host]\n 'host' can be omitted if game is connected to server");
    }
    else
    {
      TracertReporter.ReportTracertInfo(host);
      UIStatus.Get().AddInfo("It's called with '" + host + "'.");
    }
    return true;
  }

  private bool TryParsePlayerTags(string input, out string output)
  {
    if (string.IsNullOrEmpty(input))
    {
      UIStatus.Get().AddInfo(string.Format("Player tags cleared."));
      output = input;
      return true;
    }
    string[] strArray1 = input.Split(',');
    if (strArray1.Length > 20)
    {
      output = "";
      UIStatus.Get().AddError(string.Format("{0} tag values found, but only {1} tag values can be passed.", (object) strArray1.Length, (object) 20));
      return false;
    }
    foreach (string str in strArray1)
    {
      if (!string.IsNullOrEmpty(str))
      {
        string[] strArray2 = str.Split('=');
        if (strArray2.Length != 2)
        {
          output = "";
          UIStatus.Get().AddError(string.Format("Invalid tag/value entry: \"{0}\". Format is \"TagId=Value\".", (object) str));
          return false;
        }
        int result1 = 0;
        int result2 = 0;
        if (!int.TryParse(strArray2[0], out result1))
        {
          output = "";
          UIStatus.Get().AddError(string.Format("Invalid tagId: \"{0}\". Must be an integer.", (object) strArray2[0]));
          return false;
        }
        if (!int.TryParse(strArray2[1], out result2))
        {
          result2 = GameUtils.TranslateCardIdToDbId(strArray2[1], true);
          if (result2 == 0)
          {
            output = "";
            UIStatus.Get().AddError(string.Format("Invalid tagValue: \"{0}\". Must be an integer.", (object) strArray2[1]));
            return false;
          }
        }
        if (result1 > 999999)
        {
          output = "";
          UIStatus.Get().AddError(string.Format("Invalid tagId: \"{0}\". Must be < {1}.", (object) result1, (object) 999999));
          return false;
        }
        if (result1 <= 0)
        {
          output = "";
          UIStatus.Get().AddError(string.Format("Invalid tagId: \"{0}\". Must be > 0.", (object) result1));
          return false;
        }
        if (result2 > 999999)
        {
          output = "";
          UIStatus.Get().AddError(string.Format("Invalid tagValue: \"{0}\". Must be < {1}.", (object) result2, (object) 999999));
          return false;
        }
      }
    }
    UIStatus.Get().AddInfo(string.Format("Player tags set for next game."));
    output = input;
    return true;
  }

  private bool TryParseArenaChoices(string[] input, out string[] output)
  {
    List<string> stringList = new List<string>();
    bool arenaChoices = input.Length != 0;
    foreach (string str1 in input)
    {
      string str2 = str1.Replace(",", "");
      int result = 0;
      if (!int.TryParse(str2, out result))
      {
        result = GameUtils.TranslateCardIdToDbId(str2);
        if (result == 0)
        {
          UIStatus.Get().AddError(string.Format("Invalid tagValue: \"{0}\". Must be an integer or valid card Id.", (object) str2));
          arenaChoices = false;
          break;
        }
        str2 = result.ToString();
      }
      if (result > 999999)
      {
        UIStatus.Get().AddError(string.Format("Invalid card ID: \"{0}\". Must be < {1}.", (object) result, (object) 999999));
        arenaChoices = false;
        break;
      }
      if (result <= 0)
      {
        UIStatus.Get().AddError(string.Format("Invalid card ID: \"{0}\". Must be > 0.", (object) result));
        arenaChoices = false;
        break;
      }
      stringList.Add(str2);
    }
    output = stringList.ToArray();
    return arenaChoices;
  }

  private bool TryParseNamedArgs(string[] args, out Map<string, Cheats.NamedParam> values)
  {
    values = new Map<string, Cheats.NamedParam>();
    foreach (string str in args)
    {
      string[] strArray = str.Trim().Split('=');
      if (strArray.Length > 1)
        values.Add(strArray[0], new Cheats.NamedParam(strArray[1]));
    }
    return values.Count > 0;
  }

  private bool OnProcessCheat_HasSeenCollectionManager(string func, string[] args, string rawArgs)
  {
    Options.Get().SetBool(Option.HAS_SEEN_COLLECTIONMANAGER, false);
    return true;
  }

  private bool OnProcessCheat_brode(string func, string[] args, string rawArgs)
  {
    NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.ALL, new Vector3(133.1f, NotificationManager.DEPTH, 54.2f), GameStrings.Get("VO_INNKEEPER_FORGE_1WIN"), "VO_INNKEEPER_ARENA_1WIN.prefab:31bb13e800c74c0439ee1a7bfc1e3499");
    return true;
  }

  private bool On_ProcessCheat_bug(string func, string[] args, string rawArgs) => true;

  private bool On_ProcessCheat_ANR(string func, string[] args, string rawArgs)
  {
    if (!ExceptionReporter.Get().IsEnabledANRMonitor)
    {
      UIStatus.Get().AddInfo("ANR Monitor of ExceptionReporter is disabled");
      return true;
    }
    try
    {
      this.m_waitTime = float.Parse(args[0]);
    }
    catch
    {
    }
    this.m_showedMessage = false;
    Processor.RegisterUpdateDelegate(new System.Action(this.SimulatorPauseUpdate));
    return true;
  }

  private void SimulatorPauseUpdate()
  {
    UIStatus.Get().AddInfo("Wait for " + (object) this.m_waitTime + " seconds");
    if (this.m_showedMessage)
    {
      Thread.Sleep((int) ((double) this.m_waitTime * 1000.0));
      Processor.UnregisterUpdateDelegate(new System.Action(this.SimulatorPauseUpdate));
    }
    this.m_showedMessage = true;
  }

  private bool OnProcessCheat_igm(string func, string[] args, string rawArgs) => true;

  private bool OnProcessCheat_msgui(string func, string[] args, string rawArgs)
  {
    string str = "show";
    if (args.Length != 0 && !string.IsNullOrEmpty(args[0]))
      str = args[0];
    if ("add".StartsWith(str))
      this.AddMessagePopupForArgs(args);
    else if ("help".StartsWith(str))
      UIStatus.Get().AddInfo("USAGE: msgui [add] [text|shop|launch|change|empty] [imageType|pid|launchEffectId|launchEffectColor|launchEffectSoundId|url|changeObjecTtype|cardChangeCount]");
    return true;
  }

  private void AddMessagePopupForArgs(string[] args)
  {
    MessageUIData messageUiData = Cheats.ConstructUIDataFromArgs(args);
    if (messageUiData == null)
    {
      Log.InGameMessage.PrintDebug("Failed to construct UI Data for test IGM");
    }
    else
    {
      MessagePopupDisplay messagePopupDisplay = ServiceManager.Get<MessagePopupDisplay>();
      if (messagePopupDisplay == null)
        UIStatus.Get().AddError("Message Popup Display was not available to show a message");
      else
        messagePopupDisplay.AddMessages(new List<MessageUIData>()
        {
          messageUiData
        });
    }
  }

  private static MessageUIData ConstructUIDataFromArgs(string[] args)
  {
    MessageLayoutType layoutTypeIfAvailable = Cheats.GetLayoutTypeIfAvailable(args);
    if (layoutTypeIfAvailable == MessageLayoutType.INVALID)
      return (MessageUIData) null;
    MessageUIData messageUiData = new MessageUIData()
    {
      LayoutType = layoutTypeIfAvailable,
      MessageData = Cheats.ConstructContentDataForMessage(layoutTypeIfAvailable, args)
    };
    return messageUiData.MessageData == null ? (MessageUIData) null : messageUiData;
  }

  private static IMessageContent ConstructContentDataForMessage(
    MessageLayoutType layoutType,
    string[] args)
  {
    switch (layoutType)
    {
      case MessageLayoutType.TEXT:
        return (IMessageContent) Cheats.ConstructTestTextMsg(args);
      case MessageLayoutType.SHOP:
        return (IMessageContent) Cheats.ConstructTestShopMsg(args);
      case MessageLayoutType.LAUNCH:
        return (IMessageContent) Cheats.ConstructTestLaunchMessage(args);
      case MessageLayoutType.CHANGE:
        return (IMessageContent) Cheats.ConstructTestChangeMessage(args);
      case MessageLayoutType.EMPTY:
        return (IMessageContent) Cheats.ConstructEmptyMailboxMessage(args);
      default:
        UIStatus.Get().AddError(string.Format("Unsupported content type {0}", (object) layoutType));
        return (IMessageContent) null;
    }
  }

  private static ChangeMessageContent ConstructTestChangeMessage(string[] args)
  {
    string empty1 = string.Empty;
    int num = 0;
    string empty2 = string.Empty;
    if (args.Length > 2 && !string.IsNullOrEmpty(args[2]))
      empty1 = args[2];
    if (args.Length > 3 && !string.IsNullOrEmpty(args[3]))
    {
      empty2 = args[3];
      if (empty2 != "card" && empty2 != "hero")
      {
        UIStatus.Get().AddError("The third argument for a change in game message should be either 'card' or 'hero' to determine what object type we want to display.");
        return (ChangeMessageContent) null;
      }
    }
    List<ChangeMessageItemInformation> messageItemInformationList = new List<ChangeMessageItemInformation>();
    if (args.Length > 4 && !string.IsNullOrEmpty(args[4]))
      num = int.Parse(args[4]);
    if (empty2.Equals("hero", StringComparison.InvariantCultureIgnoreCase) && num > 1 || empty2.Equals("card") && num > 5)
      UIStatus.Get().AddError(string.Format("The card count given ({0}) is too high for the object type given({1})", (object) num, (object) empty2));
    for (int index = 0; index < num; ++index)
    {
      if (!(empty2 == "hero"))
      {
        if (empty2 == "card")
          messageItemInformationList.Add(Cheats.m_changeMessageCardsExamples[index]);
        else
          UIStatus.Get().AddError("Unrecognized change object type passed in for in game message.");
      }
      else
        messageItemInformationList.Add(Cheats.m_changeMessageHeroExamples[index]);
    }
    return new ChangeMessageContent()
    {
      Title = "Lorem Ipsum",
      BodyText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec ut rhoncus ante. Donec in pretium felis. Duis mollis purus a ante mollis luctus. Nulla hendrerit gravida nulla non convallis. Vivamus vel ligula a mi porta porta et at magna. Nulla euismod diam eget arcu pharetra scelerisque. In id sem a ipsum maximus cursus. In pulvinar fermentum dolor, at ultrices ipsum congue nec.",
      Url = empty1,
      ChangeItems = messageItemInformationList
    };
  }

  private static LaunchMessageContent ConstructTestLaunchMessage(string[] args)
  {
    string str = "Logo";
    string empty1 = string.Empty;
    string empty2 = string.Empty;
    string empty3 = string.Empty;
    if (args.Length > 2 && !string.IsNullOrEmpty(args[2]))
      str = args[2];
    if (args.Length > 3 && !string.IsNullOrEmpty(args[3]))
      empty1 = args[3];
    if (args.Length > 4 && !string.IsNullOrEmpty(args[4]))
      empty2 = args[4];
    if (args.Length > 5 && !string.IsNullOrEmpty(args[5]))
      empty3 = args[5];
    return new LaunchMessageContent()
    {
      IconType = str,
      Title = "Lorem Ipsum",
      TextBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec ut rhoncus ante. Donec in pretium felis. Duis mollis purus a ante mollis luctus. Nulla hendrerit gravida nulla non convallis. Vivamus vel ligula a mi porta porta et at magna. Nulla euismod diam eget arcu pharetra scelerisque. In id sem a ipsum maximus cursus. In pulvinar fermentum dolor, at ultrices ipsum congue nec.",
      Effect = new LaunchMessageEffectContent()
      {
        EffectId = empty1,
        EffectColor = empty2,
        EffectSoundId = empty3
      }
    };
  }

  private static TextMessageContent ConstructTestTextMsg(string[] args)
  {
    string str = "Logo";
    if (args.Length > 2 && !string.IsNullOrEmpty(args[2]))
      str = args[2];
    return new TextMessageContent()
    {
      ImageType = str,
      ImageMaterial = (string) null,
      Title = "Lorem Ipsum",
      TextBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec ut rhoncus ante. Donec in pretium felis. Duis mollis purus a ante mollis luctus. Nulla hendrerit gravida nulla non convallis. Vivamus vel ligula a mi porta porta et at magna. Nulla euismod diam eget arcu pharetra scelerisque. In id sem a ipsum maximus cursus. In pulvinar fermentum dolor, at ultrices ipsum congue nec."
    };
  }

  private static ShopMessageContent ConstructTestShopMsg(string[] args)
  {
    long result = 10747;
    if (args.Length > 2 && !string.IsNullOrEmpty(args[2]) && !long.TryParse(args[2], out result))
    {
      UIStatus.Get().AddError("Invalid product id for show igm: " + args[2]);
      return (ShopMessageContent) null;
    }
    return new ShopMessageContent()
    {
      Title = "Lorem Ipsum",
      TextBody = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec ut rhoncus ante. Donec in pretium felis. Duis mollis purus a ante mollis luctus. Nulla hendrerit gravida nulla non convallis. Vivamus vel ligula a mi porta porta et at magna. Nulla euismod diam eget arcu pharetra scelerisque. In id sem a ipsum maximus cursus. In pulvinar fermentum dolor, at ultrices ipsum congue nec.",
      ProductID = result
    };
  }

  private static MessageLayoutType GetLayoutTypeIfAvailable(string[] args)
  {
    MessageLayoutType layoutTypeIfAvailable = MessageLayoutType.TEXT;
    if (args.Length > 1 && !string.IsNullOrEmpty(args[1]))
    {
      string lower = args[1].ToLower();
      if (!(lower == "text"))
      {
        if (!(lower == "shop"))
        {
          if (!(lower == "launch"))
          {
            if (!(lower == "change"))
            {
              if (lower == "empty")
              {
                layoutTypeIfAvailable = MessageLayoutType.EMPTY;
              }
              else
              {
                layoutTypeIfAvailable = MessageLayoutType.INVALID;
                UIStatus.Get().AddError("Invalid message type to show " + lower);
              }
            }
            else
              layoutTypeIfAvailable = MessageLayoutType.CHANGE;
          }
          else
            layoutTypeIfAvailable = MessageLayoutType.LAUNCH;
        }
        else
          layoutTypeIfAvailable = MessageLayoutType.SHOP;
      }
      else
        layoutTypeIfAvailable = MessageLayoutType.TEXT;
    }
    return layoutTypeIfAvailable;
  }

  private static TextMessageContent ConstructEmptyMailboxMessage(string[] args)
  {
    string str = "mercs";
    if (args.Length > 2 && !string.IsNullOrEmpty(args[2]))
      str = args[2];
    return new TextMessageContent()
    {
      ImageType = str,
      Title = "Lorem Ipsum"
    };
  }

  private bool On_ProcessCheat_crash(string func, string[] args, string rawArgs)
  {
    string[] strArray = new string[6]
    {
      "help",
      "cs",
      "plugin",
      "nativeinlib",
      "javainlib",
      "report"
    };
    if (args.Length < 1 || string.IsNullOrEmpty(rawArgs))
      throw new Exception("User requested exception");
    string lower = args[0].ToLower();
    string str1 = (string) null;
    string str2 = (string) null;
    if (args.Length > 1)
    {
      str1 = args[1];
      str2 = args[1];
    }
    if (string.IsNullOrEmpty(str1))
      str1 = "User requested exception";
    if ("plugin".StartsWith(lower))
    {
      if (PlatformSettings.IsMobileRuntimeOS)
        MobileCallbackManager.CreateCrashPlugInLayer(str1);
      else
        UIStatus.Get().AddInfo("Plug-in crash is only for Android platform");
    }
    else if ("javainlib".StartsWith(lower))
    {
      if (PlatformSettings.RuntimeOS == OSCategory.Android)
        MobileCallbackManager.CreateCrashInNativeLayer("java:" + str1);
      else
        UIStatus.Get().AddInfo("Java crash is only for Android platforms");
    }
    else if ("nativeinlib".StartsWith(lower))
    {
      if (PlatformSettings.IsMobileRuntimeOS)
        MobileCallbackManager.CreateCrashInNativeLayer(str1);
      else
        UIStatus.Get().AddInfo("Native crash is only for mobile platforms");
    }
    else
    {
      if ("cs".StartsWith(lower))
        throw new Exception(str1);
      if ("restricted".StartsWith(lower))
      {
        if (string.IsNullOrEmpty(str2))
          str2 = ExceptionReporterControl.Get().IsRestrictedReport ? "off" : "on";
        ExceptionReporterControl.Get().IsRestrictedReport = str2 == "on";
        UIStatus.Get().AddInfo("Exception report restriction: " + str2);
      }
      else if ("report".StartsWith(lower))
      {
        if (str1.Length < 36)
          UIStatus.Get().AddInfo(str1 + " seems not UUID format!");
      }
      else if ("t5report".StartsWith(lower))
      {
        if (string.IsNullOrEmpty(str2))
          str2 = ExceptionReporterControl.Get().IsEnabledT5MobileReport ? "off" : "on";
        ExceptionReporterControl.Get().IsEnabledT5MobileReport = str2 == "on";
        UIStatus.Get().AddInfo("Register as exception in t5/mobile: " + str2);
      }
      else if (lower == "help")
        UIStatus.Get().AddInfo("USAGE: crash [where] [exception title]\n Where(substring): " + string.Join(" | ", strArray) + "\ncrash t5report on/off\ncrash restricted on/off");
    }
    return true;
  }

  private bool OnProcessCheat_questcompletepopup(string func, string[] args, string rawArgs)
  {
    int result = 0;
    Achievement quest = int.TryParse(rawArgs, out result) ? AchieveManager.Get().GetAchievement(result) : (Achievement) null;
    if (quest == null)
    {
      UIStatus.Get().AddError(string.Format("{0}: please specify a valid Quest ID", (object) func));
      return true;
    }
    QuestToast.ShowQuestToast(UserAttentionBlocker.ALL, (QuestToast.DelOnCloseQuestToast) null, false, quest);
    return true;
  }

  private bool OnProcessCheat_narrative(string func, string[] args, string rawArgs)
  {
    if (args.Length == 1 && args[0] == "clear")
    {
      string message = string.Format("Narrative seen options cleared:\n{0}", (object) string.Join(", ", NarrativeManager.Get().Cheat_ClearAllSeen().Select<Option, string>((Func<Option, string>) (o => Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(o))).ToArray<string>()));
      UIStatus.Get().AddInfo(message);
      return true;
    }
    int result = 0;
    if ((int.TryParse(rawArgs, out result) ? AchieveManager.Get().GetAchievement(result) : (Achievement) null) == null)
    {
      UIStatus.Get().AddError(string.Format("{0}: please specify a valid Quest ID", (object) func));
      return true;
    }
    NarrativeManager.Get().OnQuestCompleteShown(result);
    NarrativeManager.Get().ShowOutstandingQuestDialogs();
    return true;
  }

  private bool OnProcessCheat_narrativedialog(string func, string[] args, string rawArgs)
  {
    int result = 0;
    CharacterDialogSequence sequence = int.TryParse(rawArgs, out result) ? new CharacterDialogSequence(result) : (CharacterDialogSequence) null;
    if (sequence == null)
    {
      UIStatus.Get().AddError(string.Format("{0}: please specify a valid Dialog ID", (object) func));
      return true;
    }
    NarrativeManager.Get().PushDialogSequence(sequence);
    return true;
  }

  private bool OnProcessCheat_questwelcome(string func, string[] args, string rawArgs)
  {
    bool boolVal = true;
    if (args.Length != 0 && !string.IsNullOrEmpty(args[0]))
      GeneralUtils.TryParseBool(args[0], out boolVal);
    WelcomeQuests.Show(UserAttentionBlocker.ALL, boolVal);
    return true;
  }

  private bool OnProcessCheat_newquestvisual(string func, string[] args, string rawArgs)
  {
    if ((UnityEngine.Object) WelcomeQuests.Get() == (UnityEngine.Object) null)
    {
      UIStatus.Get().AddError("WelcomeQuests object is not active - try using 'questwelcome' cheat first.");
      return true;
    }
    int result = 0;
    Achievement quest = int.TryParse(rawArgs, out result) ? AchieveManager.Get().GetAchievement(result) : (Achievement) null;
    if (quest == null)
    {
      UIStatus.Get().AddError(string.Format("{0}: please specify a valid Quest ID", (object) func));
      return true;
    }
    WelcomeQuests.Get().GetFirstQuestTile().SetupTile(quest, QuestTile.FsmEvent.QuestGranted);
    return true;
  }

  private bool OnProcessCheat_questprogresspopup(string func, string[] args, string rawArgs)
  {
    int result1 = 0;
    Achievement achievement = args.Length == 0 || !int.TryParse(args[0], out result1) ? (Achievement) null : AchieveManager.Get().GetAchievement(result1);
    int result2 = 1;
    string name;
    string description;
    int result3;
    int result4;
    if (achievement == null)
    {
      if (result1 != 0)
      {
        UIStatus.Get().AddError("unknown Achieve with ID " + (object) result1);
        return true;
      }
      if (args.Length < 4)
      {
        UIStatus.Get().AddError("please specify an Achieve ID or the following params:\n<title> <description> <progress> <maxprogress>");
        return true;
      }
      name = args[0];
      description = args[1];
      int.TryParse(args[2], out result3);
      int.TryParse(args[3], out result4);
    }
    else
    {
      name = achievement.Name;
      description = achievement.Description;
      result3 = achievement.Progress;
      result4 = achievement.MaxProgress;
    }
    for (int index = 0; index < args.Length; ++index)
    {
      string[] strArray = args[index].Split('=');
      if (strArray.Length >= 2)
      {
        string str = strArray[0];
        string s = strArray[1];
        if (str == "count" && !int.TryParse(s, out result2))
        {
          UIStatus.Get().AddError(string.Format("Unable to parse parameter #{0} as integer: {1}", (object) (index + 1), (object) s));
          return true;
        }
      }
    }
    if ((UnityEngine.Object) GameToastMgr.Get() != (UnityEngine.Object) null)
    {
      if (result3 >= result4)
        result3 = result4 - 1;
      for (int index = 0; index < result2; ++index)
        GameToastMgr.Get().AddQuestProgressToast(result1, name, description, result3, result4);
      return true;
    }
    UIStatus.Get().AddError("GameToastMgr is null!");
    return true;
  }

  private bool OnProcessCheat_retire(string func, string[] args, string rawArgs)
  {
    if (DemoMgr.Get().GetMode() != DemoMode.BLIZZCON_2013)
      return false;
    DraftManager draftManager = DraftManager.Get();
    if (draftManager == null)
      return false;
    Network.Get().DraftRetire(draftManager.GetDraftDeck().ID, draftManager.GetSlot(), draftManager.CurrentSeasonId);
    return true;
  }

  private bool OnProcessCheat_storepassword(string func, string[] args, string rawArgs)
  {
    if (this.m_loadingStoreChallengePrompt)
      return true;
    if ((UnityEngine.Object) this.m_storeChallengePrompt == (UnityEngine.Object) null)
    {
      this.m_loadingStoreChallengePrompt = true;
      PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) => Processor.RunCoroutine(this.StorePasswordCoroutine(assetRef, go, callbackData)));
      AssetLoader.Get().InstantiatePrefab((AssetReference) "StoreChallengePrompt.prefab:43f02a51d311c214aa25232228ccefef", callback);
    }
    else if (this.m_storeChallengePrompt.IsShown())
      this.m_storeChallengePrompt.Hide();
    else
      Processor.RunCoroutine(this.StorePasswordCoroutine((AssetReference) this.m_storeChallengePrompt.name, this.m_storeChallengePrompt.gameObject, (object) null));
    return true;
  }

  private bool OnProcessCheat_notice(string func, string[] args, string rawArgs)
  {
    if (((IEnumerable<string>) args).Count<string>() < 2)
    {
      UIStatus.Get().AddError("notice cheat requires 2 params: [string]type [int]data [OPTIONAL int]data2 [OPTIONAL bool]quest toast?");
      return true;
    }
    int data = -1;
    int.TryParse(args[1], out data);
    if (data < 0)
    {
      UIStatus.Get().AddError(string.Format("{0}: please specify a valid Notice Data Value", (object) data));
      return true;
    }
    string s = (string) null;
    if (args.Length > 2)
      s = args[2];
    bool flag = false;
    if (args.Length > 3)
      flag = GeneralUtils.ForceBool(args[3]);
    NetCache.ProfileNotice notice = (NetCache.ProfileNotice) null;
    Achievement quest = new Achievement();
    List<RewardData> rewards1 = quest.Rewards;
    switch (args[0])
    {
      case "arcane_orbs":
        if (flag)
        {
          rewards1.Add((RewardData) RewardUtils.CreateArcaneOrbRewardData(data));
          break;
        }
        notice = (NetCache.ProfileNotice) CreateCurrencyNotice(PegasusShared.CurrencyType.CURRENCY_TYPE_CN_ARCANE_ORBS);
        break;
      case "booster":
        int result1 = 1;
        if (!string.IsNullOrEmpty(s))
          int.TryParse(s, out result1);
        if (GameDbf.Booster.GetRecord(result1) == null)
        {
          UIStatus.Get().AddError(string.Format("Booster ID is invalid: {0}", (object) result1));
          return true;
        }
        if (flag)
        {
          rewards1.Add((RewardData) new BoosterPackRewardData()
          {
            Id = result1,
            Count = data
          });
          break;
        }
        notice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardBooster()
        {
          Count = data,
          Id = result1
        };
        break;
      case "card":
        string cardId = "NEW1_040";
        if (!string.IsNullOrEmpty(s))
        {
          int result2 = -1;
          int.TryParse(s, out result2);
          cardId = result2 <= 0 ? s : GameUtils.TranslateDbIdToCardId(result2);
        }
        if (GameUtils.GetCardRecord(cardId) == null)
        {
          UIStatus.Get().AddError(string.Format("Card ID is invalid: {0}", (object) cardId));
          return true;
        }
        if (flag)
        {
          rewards1.Add((RewardData) new CardRewardData()
          {
            CardID = cardId,
            Count = Mathf.Clamp(data, 1, 2)
          });
          break;
        }
        notice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardCard()
        {
          CardID = cardId,
          Quantity = Mathf.Clamp(data, 1, 2)
        };
        break;
      case "cardback":
        if (GameDbf.CardBack.GetRecord(data) == null)
        {
          UIStatus.Get().AddError(string.Format("Cardback ID is invalid: {0}", (object) data));
          return true;
        }
        if (flag)
        {
          rewards1.Add((RewardData) new CardBackRewardData()
          {
            CardBackID = data
          });
          break;
        }
        notice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardCardBack()
        {
          CardBackID = data
        };
        break;
      case "dust":
        if (flag)
        {
          rewards1.Add((RewardData) new ArcaneDustRewardData()
          {
            Amount = data
          });
          break;
        }
        notice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardDust()
        {
          Amount = data
        };
        break;
      case "event":
        flag = true;
        rewards1.Add((RewardData) new EventRewardData()
        {
          EventType = data
        });
        break;
      case "gold":
        if (flag)
        {
          rewards1.Add((RewardData) new GoldRewardData()
          {
            Amount = (long) data
          });
          break;
        }
        notice = (NetCache.ProfileNotice) CreateCurrencyNotice(PegasusShared.CurrencyType.CURRENCY_TYPE_GOLD);
        break;
      case "license":
        flag = false;
        NetCache.NetCacheAccountLicenses netObject = NetCache.Get().GetNetObject<NetCache.NetCacheAccountLicenses>();
        NetCache.ProfileNoticeAcccountLicense noticeAcccountLicense = new NetCache.ProfileNoticeAcccountLicense();
        noticeAcccountLicense.License = (long) data;
        noticeAcccountLicense.Origin = NetCache.ProfileNotice.NoticeOrigin.ACCOUNT_LICENSE_FLAGS;
        noticeAcccountLicense.OriginData = 1L;
        if (netObject.AccountLicenses.ContainsKey(noticeAcccountLicense.License))
          noticeAcccountLicense.CasID = netObject.AccountLicenses[noticeAcccountLicense.License].CasId + 1L;
        notice = (NetCache.ProfileNotice) noticeAcccountLicense;
        break;
      case "mercenaries_ability_unlock":
        NetCache.ProfileNoticeMercenariesAbilityUnlock mercenariesAbilityUnlock1 = new NetCache.ProfileNoticeMercenariesAbilityUnlock();
        mercenariesAbilityUnlock1.Origin = NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_MERCENARIES;
        mercenariesAbilityUnlock1.MercenaryId = data;
        int result3 = 1;
        if (!string.IsNullOrEmpty(s))
          int.TryParse(s, out result3);
        mercenariesAbilityUnlock1.AbilityId = result3;
        if (mercenariesAbilityUnlock1 != null)
        {
          RewardUtils.LoadAndDisplayRewards(RewardUtils.GetRewards(new List<NetCache.ProfileNotice>()
          {
            (NetCache.ProfileNotice) mercenariesAbilityUnlock1
          }));
          break;
        }
        break;
      case "mercenaries_ability_unlock_notice":
        NetCache.ProfileNoticeMercenariesAbilityUnlock mercenariesAbilityUnlock2 = new NetCache.ProfileNoticeMercenariesAbilityUnlock();
        mercenariesAbilityUnlock2.Origin = NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_MERCENARIES;
        mercenariesAbilityUnlock2.MercenaryId = data;
        int result4 = 1;
        if (!string.IsNullOrEmpty(s))
          int.TryParse(s, out result4);
        mercenariesAbilityUnlock2.AbilityId = result4;
        notice = (NetCache.ProfileNotice) mercenariesAbilityUnlock2;
        break;
      case "mercenaries_autoretire_reward":
        NetCache.ProfileNoticeMercenariesRewards mercenariesRewards1 = new NetCache.ProfileNoticeMercenariesRewards();
        mercenariesRewards1.Chest = RewardUtils.GenerateMercenariesConsolationReward_CHEAT();
        mercenariesRewards1.Origin = NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_MERCENARIES;
        mercenariesRewards1.RewardType = PegasusShared.ProfileNoticeMercenariesRewards.RewardType.REWARD_TYPE_PVE_AUTO_RETIRE;
        notice = (NetCache.ProfileNotice) mercenariesRewards1;
        break;
      case "mercenaries_booster":
        notice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesBoosterLicense()
        {
          Count = data
        };
        break;
      case "mercenaries_coin":
        notice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesCurrencyLicense()
        {
          MercenaryId = 1,
          CurrencyAmount = 100L
        };
        break;
      case "mercenaries_consolation_reward":
        NetCache.ProfileNoticeMercenariesRewards mercenariesRewards2 = new NetCache.ProfileNoticeMercenariesRewards();
        mercenariesRewards2.Chest = RewardUtils.GenerateMercenariesConsolationReward_CHEAT();
        mercenariesRewards2.Origin = NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_MERCENARIES;
        mercenariesRewards2.RewardType = PegasusShared.ProfileNoticeMercenariesRewards.RewardType.REWARD_TYPE_PVE_CONSOLATION;
        notice = (NetCache.ProfileNotice) mercenariesRewards2;
        break;
      case "mercenaries_equipment_unlock":
        List<RewardData> rewards2 = new List<RewardData>();
        int equipmentId = 1;
        if (!string.IsNullOrEmpty(s))
          int.TryParse(s, out equipmentId);
        LettuceEquipmentDbfRecord record = GameDbf.LettuceEquipment.GetRecord((Predicate<LettuceEquipmentDbfRecord>) (r => r.ID == equipmentId));
        if (record != null && record.LettuceEquipmentTiers.Count > 0)
        {
          rewards2.Add((RewardData) new MercenariesEquipmentRewardData(data, equipmentId, record.LettuceEquipmentTiers[0].Tier));
          RewardUtils.LoadAndDisplayRewards(rewards2);
          break;
        }
        break;
      case "mercenaries_map_chest":
        NetCache.ProfileNoticeMercenariesRewards mercenariesRewards3 = new NetCache.ProfileNoticeMercenariesRewards();
        mercenariesRewards3.Chest = RewardUtils.GenerateMercenariesMapRewardChest_CHEAT();
        mercenariesRewards3.Origin = NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_MERCENARIES;
        notice = (NetCache.ProfileNotice) mercenariesRewards3;
        break;
      case "mercenaries_mercenary":
        notice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesMercenaryLicense()
        {
          MercenaryId = 1,
          ArtVariationId = 0,
          ArtVariationPremium = 2U,
          CurrencyAmount = 100L
        };
        break;
      case "mercenaries_season_reward":
        NetCache.ProfileNoticeMercenariesSeasonRewards mercenariesSeasonRewards = new NetCache.ProfileNoticeMercenariesSeasonRewards();
        mercenariesSeasonRewards.Chest = RewardUtils.GenerateMercenariesSeasonReward_CHEAT();
        mercenariesSeasonRewards.Origin = NetCache.ProfileNotice.NoticeOrigin.NOTICE_ORIGIN_MERCENARIES;
        mercenariesSeasonRewards.RewardAssetId = LettucePlayDisplay.SortedRewardRecords[data].ID;
        notice = (NetCache.ProfileNotice) mercenariesSeasonRewards;
        break;
      case "mercenaries_zone_unlock":
        notice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeMercenariesZoneUnlock()
        {
          ZoneId = data
        };
        break;
      case "runestones":
        notice = ShopUtils.IsMainVirtualCurrencyType(CurrencyType.CN_RUNESTONES) ? (NetCache.ProfileNotice) CreateCurrencyNotice(PegasusShared.CurrencyType.CURRENCY_TYPE_CN_RUNESTONES) : (NetCache.ProfileNotice) CreateCurrencyNotice(PegasusShared.CurrencyType.CURRENCY_TYPE_ROW_RUNESTONES);
        break;
      case "tavern_brawl_rewards":
        NetCache.ProfileNoticeTavernBrawlRewards tavernBrawlRewards = new NetCache.ProfileNoticeTavernBrawlRewards();
        tavernBrawlRewards.Wins = data;
        TavernBrawlMode mode = s.Equals("heroic") ? TavernBrawlMode.TB_MODE_HEROIC : TavernBrawlMode.TB_MODE_NORMAL;
        tavernBrawlRewards.Mode = mode;
        tavernBrawlRewards.Chest = RewardUtils.GenerateTavernBrawlRewardChest_CHEAT(data, mode);
        notice = (NetCache.ProfileNotice) tavernBrawlRewards;
        break;
      default:
        UIStatus.Get().AddError(string.Format("{0}: please specify a valid Notice Type.\nValid Types are: 'gold','arcane_orbs','dust','booster','card','cardback','tavern_brawl_rewards','event','license'", (object) args[0]));
        return true;
    }
    if (flag)
    {
      quest.SetDescription("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", "");
      quest.SetName("Title Text", "");
      QuestToast.ShowQuestToast(UserAttentionBlocker.ALL, (QuestToast.DelOnCloseQuestToast) null, false, quest);
    }
    else if (notice != null)
      NetCache.Get().Cheat_AddNotice(notice);
    return true;

    NetCache.ProfileNoticeRewardCurrency CreateCurrencyNotice(PegasusShared.CurrencyType currency) => new NetCache.ProfileNoticeRewardCurrency()
    {
      CurrencyType = currency,
      Amount = data
    };
  }

  private bool OnProcessCheat_LoadWidget(string func, string[] args, string rawArgs)
  {
    string assetString = args[0];
    if (string.IsNullOrEmpty(assetString))
    {
      UIStatus.Get().AddError("First parameter must be the GUID of a valid widget template.");
      return false;
    }
    WidgetInstance widgetInstance = WidgetInstance.Create(assetString);
    if ((UnityEngine.Object) widgetInstance == (UnityEngine.Object) null)
    {
      UIStatus.Get().AddError("First parameter must be the GUID of a valid widget template.");
      return false;
    }
    this.s_createdWidgets.Add(widgetInstance);
    widgetInstance.TriggerEvent("CHEATED_STATE", new Widget.TriggerEventParameters());
    return true;
  }

  private bool OnProcessCheat_ClearWidgets(string func, string[] args, string rawArgs)
  {
    foreach (Component createdWidget in this.s_createdWidgets)
      UnityEngine.Object.Destroy((UnityEngine.Object) createdWidget.gameObject);
    this.s_createdWidgets.Clear();
    return true;
  }

  private bool OnProcessCheat_ServerLog(string func, string[] args, string rawArgs)
  {
    SceneDebugger.Get().AddServerScriptLogMessage(new ScriptLogMessage()
    {
      Message = rawArgs,
      Event = "Cheat",
      Severity = 1
    });
    return true;
  }

  private bool OnProcessCheat_dialogEvent(string func, string[] args, string rawArgs)
  {
    if (args.Length != 1)
    {
      UIStatus.Get().AddError("Provide 1 param for " + func + ".");
      return true;
    }
    NarrativeManager narrativeManager = NarrativeManager.Get();
    if ((UnityEngine.Object) narrativeManager == (UnityEngine.Object) null)
      return false;
    if (args[0] == "reset")
    {
      UIStatus.Get().AddInfo("All ScheduledCharacterDialogEvent's have been reset.");
      narrativeManager.ResetScheduledCharacterDialogEvent_Debug();
      return true;
    }
    ScheduledCharacterDialogEvent result = ScheduledCharacterDialogEvent.INVALID;
    System.Enum.TryParse<ScheduledCharacterDialogEvent>(args[0], true, out result);
    if (!System.Enum.IsDefined(typeof (ScheduledCharacterDialogEvent), (object) result) || result == ScheduledCharacterDialogEvent.INVALID)
    {
      Array names = (Array) System.Enum.GetNames(typeof (ScheduledCharacterDialogEvent));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("reset -- this allows events to run again");
      stringBuilder.Append('\n');
      int num = 0;
      foreach (string str in names)
      {
        if (num != 0)
        {
          stringBuilder.Append(num);
          stringBuilder.Append(" = ");
          stringBuilder.Append(str);
          stringBuilder.Append('\n');
        }
        ++num;
      }
      string str1 = stringBuilder.ToString();
      UIStatus.Get().AddError("Invalid param for " + func + ". See \"Messages\".");
      Log.Gameplay.PrintError("Unrecognized <event_type>.\n" + string.Format("Try a num [1-{0}] or a string:\n", (object) (names.Length - 1)) + str1);
      return true;
    }
    narrativeManager.TriggerScheduledCharacterDialogEvent_Debug(result);
    return true;
  }

  private bool OnProcessCheat_account(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string str = "add, remove, set, skip, unlock";
    if (autofillData != null)
    {
      if ((!rawArgs.EndsWith(" ") ? 0 : (args.Length == 0 ? 1 : 0)) == 0 && args.Length != 1)
        return false;
      return Cheats.ProcessAutofillParam((IEnumerable<string>) str.Split(new char[2]
      {
        ' ',
        ','
      }, StringSplitOptions.RemoveEmptyEntries), args.Length == 0 ? string.Empty : args[0], autofillData);
    }
    string message = "account cheat requires one of the following valid sub-commands: " + str;
    if (args.Length == 0)
    {
      UIStatus.Get().AddError(message);
      return true;
    }
    string lower = args[0].ToLower();
    string[] array = ((IEnumerable<string>) args).Skip<string>(1).ToArray<string>();
    if (!(lower == "add"))
    {
      if (!(lower == "remove"))
      {
        if (!(lower == "set"))
        {
          if (!(lower == "skip"))
          {
            if (lower == "unlock")
              HttpCheater.Get().RunUnlockResourceCommand(array);
            else
              UIStatus.Get().AddError(message);
          }
          else
            HttpCheater.Get().RunSkipResourceCommand(array);
        }
        else
          HttpCheater.Get().RunSetResourceCommand(array);
      }
      else
        HttpCheater.Get().RunRemoveResourceCommand(array);
    }
    else
      HttpCheater.Get().RunAddResourceCommand(array);
    return true;
  }

  private bool OnProcessCheat_SkipSendingGetGameState(string func, string[] args, string rawArgs)
  {
    int result = 0;
    if (args.Length == 0 || !int.TryParse(args[0], out result))
      return false;
    this.m_skipSendingGetGameState = result != 0;
    return true;
  }

  private bool OnProcessCheat_SendGetGameState(string func, string[] args, string rawArgs)
  {
    if (!this.m_skipSendingGetGameState)
      return false;
    Network.Get().GetGameState();
    return true;
  }

  private string GetChallengeUrl(string type) => string.Format("{0}?email={1}&programId={2}&platformId={3}&redirectUrl={4}&messageKey={5}&notifyRisk={6}&chooseChallenge={7}&challengeType={8}&riskTransId={9}", (object) string.Format("https://login-qa-us.web.blizzard.net/login/admin/challenge/create/ct_{0}", (object) type.ToLower()), (object) "joe_balance@zmail.blizzard.com", (object) "wtcg", (object) "*", (object) "none", (object) "", (object) false, (object) false, (object) "", (object) "");

  private IEnumerator StorePasswordCoroutine(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_loadingStoreChallengePrompt = false;
    this.m_storeChallengePrompt = go.GetComponent<StoreChallengePrompt>();
    this.m_storeChallengePrompt.Hide();
    Dictionary<string, string> headers = new Dictionary<string, string>();
    headers["Accept"] = "application/json;charset=UTF-8";
    headers["Accept-Language"] = Localization.GetBnetLocaleName();
    string challengeUrl1 = this.GetChallengeUrl("cvv");
    Debug.Log((object) ("creating challenge with url " + challengeUrl1));
    IHttpRequest createChallenge = HttpRequestFactory.Get().CreateGetRequest(challengeUrl1);
    createChallenge.SetRequestHeaders((IEnumerable<KeyValuePair<string, string>>) headers);
    yield return (object) createChallenge.SendRequest();
    Debug.Log((object) ("challenge response is " + createChallenge.ResponseAsString));
    string challengeUrl2 = (string) (Json.Deserialize(createChallenge.ResponseAsString) as JsonNode)["challenge_url"];
    Debug.Log((object) ("challenge url is " + challengeUrl2));
    yield return (object) this.m_storeChallengePrompt.StartCoroutine(this.m_storeChallengePrompt.Show(challengeUrl2));
  }

  private bool OnProcessCheat_favoritecardback(string func, string[] args, string rawArgs)
  {
    int result;
    if (args.Length == 0 || !int.TryParse(args[0].ToLowerInvariant(), out result))
      return false;
    Network.Get().SetFavoriteCardBack(result);
    return true;
  }

  private bool OnProcessCheat_disconnect(string func, string[] args, string rawArgs)
  {
    if ((args == null || args.Length < 1 ? 0 : (args[0] == "bnet" ? 1 : 0)) != 0)
    {
      if (Network.BattleNetStatus() != ConnectionState.Ready)
      {
        UIStatus.Get().AddError("Not connected to Battle.net, status=" + (object) Network.BattleNetStatus());
        return true;
      }
      BattleNet.RequestCloseAurora();
      UIStatus.Get().AddInfo("Disconnecting from Battle.net.");
      return true;
    }
    if (!Network.Get().IsConnectedToGameServer())
    {
      UIStatus.Get().AddError("Not connected to game server.");
      return true;
    }
    if ((args == null || args.Length < 1 ? 0 : (args[0] == "pong" ? 1 : 0)) != 0)
    {
      UIStatus.Get().AddInfo("Pong responses now being ignored.");
      Network.Get().SetShouldIgnorePong(true);
      return true;
    }
    int num1 = args == null || args.Length < 1 ? 0 : (args[0] == "internet" ? 1 : 0);
    NetworkReachabilityManager reachabilityManager = ServiceManager.Get<NetworkReachabilityManager>();
    if (num1 != 0)
    {
      reachabilityManager?.SetForceUnreachable(!reachabilityManager.GetForceUnreachable());
      UIStatus.Get().AddInfo(reachabilityManager.GetForceUnreachable() ? "Forcing unreachable network." : "Network reachable.");
      return true;
    }
    if ((args == null || args.Length < 2 ? 0 : (args[0] == "duration" ? 1 : 0)) != 0)
    {
      int delay = int.Parse(args[1]);
      reachabilityManager?.SetForceUnreachable(true);
      Network.Get().SetSpoofDisconnected(true);
      Network.Get().OverrideKeepAliveSeconds(5U);
      UIStatus.Get().AddInfo(string.Format("All network disconnected for {0} seconds", (object) delay));
      HearthstoneApplication.Get().StartCoroutine(this.EnableNetworkAfterDelay(delay));
      return true;
    }
    int num2 = args == null || args.Length == 0 ? 1 : (args[0] != "force" ? 1 : 0);
    Log.LoadingScreen.Print("Cheats.OnProcessCheat_disconnect() - reconnect=true");
    if (num2 != 0)
      Network.Get().DisconnectFromGameServer();
    else
      Network.Get().SimulateUncleanDisconnectFromGameServer();
    return true;
  }

  private IEnumerator EnableNetworkAfterDelay(int delay)
  {
    yield return (object) new WaitForSeconds((float) delay);
    ServiceManager.Get<NetworkReachabilityManager>()?.SetForceUnreachable(false);
    Network.Get().SetSpoofDisconnected(false);
    Network.Get().OverrideKeepAliveSeconds(0U);
  }

  private bool OnProcessCheat_restart(string func, string[] args, string rawArgs)
  {
    if (!Network.Get().IsConnectedToGameServer())
    {
      UIStatus.Get().AddError("Not connected to game server.");
      return true;
    }
    if (!GameUtils.CanRestartCurrentMission(false))
    {
      UIStatus.Get().AddError("This game cannot be restarted.");
      return true;
    }
    GameState.Get().Restart();
    return true;
  }

  private bool OnProcessCheat_warning(string func, string[] args, string rawArgs)
  {
    string header;
    string message;
    this.ParseErrorText(args, rawArgs, out header, out message);
    Error.AddWarning(header, message);
    return true;
  }

  private bool OnProcessCheat_fatal(string func, string[] args, string rawArgs)
  {
    Error.AddFatal(FatalErrorReason.CHEAT, rawArgs);
    return true;
  }

  private bool OnProcessCheat_exit(string func, string[] args, string rawArgs)
  {
    GeneralUtils.ExitApplication();
    return true;
  }

  private bool OnProcessCheat_log(string func, string[] args, string rawArgs)
  {
    string message = "unknown log command, please use 'log help'";
    float delay = 5f;
    string lowerInvariant = args[0].ToLowerInvariant();
    string str1 = args.Length >= 2 ? args[1] : string.Empty;
    if (lowerInvariant == "help")
    {
      message = "available log commands: load reload line";
      if (str1 == "load" || str1 == "reload")
        message = "reloads the log.config";
      else if (str1 == "line")
      {
        message = "prints a simple long line to log, useful for debugging\nto visually differentiate between test results.\nyou can specify a parameter like\n'log warn' to call Debug.LogWarning. you can\nalso add a note/context to your line\nby adding words afterwards, like 'log test 2 start'\nor 'log error (test 3 starting)'.";
        delay = 10f;
      }
    }
    else if (lowerInvariant == "load" || lowerInvariant == "reload")
      LogSystem.Get().ReloadLogConfig();
    else if (lowerInvariant == "line")
    {
      Cheats.LogFormatFunc logFormatFunc = new Cheats.LogFormatFunc(Debug.LogFormat);
      string empty = string.Empty;
      int count = 1;
      if (str1 == "warn" || str1 == "warning")
      {
        logFormatFunc = new Cheats.LogFormatFunc(Debug.LogWarningFormat);
        ++count;
      }
      else if (str1 == "err" || str1 == "error")
      {
        logFormatFunc = new Cheats.LogFormatFunc(Debug.LogErrorFormat);
        ++count;
      }
      string str2 = string.Join(" ", ((IEnumerable<string>) args).Skip<string>(count).ToArray<string>());
      if (str2.Length > 0)
        str2 = string.Format(" {0} ", (object) str2);
      logFormatFunc("====={0}{1}", new object[2]
      {
        (object) str2,
        (object) new string('=', Mathf.Max(5, 75 - str2.Length))
      });
      message = "printed line to " + logFormatFunc.Method.Name;
      delay = 2f;
    }
    UIStatus.Get().AddInfo(message, delay);
    return true;
  }

  private bool OnProcessCheat_alert(string func, string[] args, string rawArgs)
  {
    AlertPopup.PopupInfo alertInfo = this.GenerateAlertInfo(rawArgs);
    if ((UnityEngine.Object) this.m_alert == (UnityEngine.Object) null)
      DialogManager.Get().ShowPopup(alertInfo, new DialogManager.DialogProcessCallback(this.OnAlertProcessed));
    else
      this.m_alert.UpdateInfo(alertInfo);
    return true;
  }

  private bool OnProcessCheat_rankedIntroPopup(string func, string[] args, string rawArgs)
  {
    DialogManager.Get().ShowRankedIntroPopUp((System.Action) null);
    MedalInfoTranslator localPlayerMedalInfo = RankMgr.Get().GetLocalPlayerMedalInfo();
    DialogManager.Get().ShowBonusStarsPopup(localPlayerMedalInfo.CreateDataModel(PegasusShared.FormatType.FT_STANDARD, RankedMedal.DisplayMode.Default), (System.Action) null);
    return true;
  }

  private bool OnProcessCheat_setRotationRotatedBoostersPopup(
    string func,
    string[] args,
    string rawArgs)
  {
    SetRotationRotatedBoostersPopup.SetRotationRotatedBoostersPopupInfo info = new SetRotationRotatedBoostersPopup.SetRotationRotatedBoostersPopupInfo();
    DialogManager.Get().ShowSetRotationTutorialPopup(UserAttentionBlocker.SET_ROTATION_INTRO, info);
    return true;
  }

  private bool OnProcessCheat_seasondialog(string func, string[] args, string rawArgs)
  {
    string cheatName = "bronze10";
    if (args.Length != 0 && !string.IsNullOrEmpty(args[0]))
      cheatName = args[0];
    LeagueRankDbfRecord recordByCheatName = RankMgr.Get().GetLeagueRankRecordByCheatName(cheatName);
    if (recordByCheatName == null)
      return false;
    PegasusShared.FormatType formatType = PegasusShared.FormatType.FT_STANDARD;
    if (args.Length >= 2)
    {
      string lower = args[1].ToLower();
      if (lower == "1" || lower == "wild")
        formatType = PegasusShared.FormatType.FT_WILD;
      else if (lower == "2" || lower == "standard")
        formatType = PegasusShared.FormatType.FT_STANDARD;
      else if (lower == "3" || lower == "classic")
      {
        formatType = PegasusShared.FormatType.FT_CLASSIC;
      }
      else
      {
        UIStatus.Get().AddInfo("please enter a valid value for 2nd parameter <format type>");
        return true;
      }
    }
    SeasonEndDialog.SeasonEndInfo seasonEndInfo = new SeasonEndDialog.SeasonEndInfo();
    seasonEndInfo.m_leagueId = recordByCheatName.LeagueId;
    seasonEndInfo.m_starLevelAtEndOfSeason = recordByCheatName.StarLevel;
    seasonEndInfo.m_bestStarLevelAtEndOfSeason = recordByCheatName.StarLevel;
    seasonEndInfo.m_formatType = formatType;
    MedalInfoTranslator medalInfoForLeagueId = MedalInfoTranslator.CreateMedalInfoForLeagueId(recordByCheatName.LeagueId, recordByCheatName.StarLevel, 0);
    medalInfoForLeagueId.GetPreviousMedal(formatType).starLevel = 1;
    medalInfoForLeagueId.GetCurrentMedal(formatType).bestStarLevel = recordByCheatName.StarLevel;
    seasonEndInfo.m_rankedRewards = new List<RewardData>();
    List<List<RewardData>> rewardsEarned = new List<List<RewardData>>();
    if (!medalInfoForLeagueId.GetRankedRewardsEarned(formatType, ref rewardsEarned))
      return false;
    foreach (List<RewardData> collection in rewardsEarned)
      seasonEndInfo.m_rankedRewards.AddRange((IEnumerable<RewardData>) collection);
    for (int index = 0; index < seasonEndInfo.m_rankedRewards.Count; ++index)
    {
      if (seasonEndInfo.m_rankedRewards[index] is RandomCardRewardData rankedReward)
      {
        string cardID = "GAME_005";
        switch (rankedReward.Rarity)
        {
          case TAG_RARITY.COMMON:
            cardID = "EX1_096";
            break;
          case TAG_RARITY.RARE:
            cardID = "EX1_274";
            break;
          case TAG_RARITY.EPIC:
            cardID = "EX1_586";
            break;
          case TAG_RARITY.LEGENDARY:
            cardID = "EX1_562";
            break;
        }
        seasonEndInfo.m_rankedRewards[index] = (RewardData) new CardRewardData(cardID, rankedReward.Premium, 1);
      }
    }
    NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
    if (netObject != null)
      seasonEndInfo.m_seasonID = netObject.Season;
    DialogManager.Get().AddToQueue(new DialogManager.DialogRequest()
    {
      m_type = DialogManager.DialogType.SEASON_END,
      m_info = (object) seasonEndInfo,
      m_isFake = true
    });
    return true;
  }

  private bool OnProcessCheat_playnullsound(string func, string[] args, string rawArgs)
  {
    SoundManager.Get().Play((AudioSource) null);
    return true;
  }

  private bool OnProcessCheat_playaudio(string func, string[] args, string rawArgs)
  {
    // ISSUE: reference to a compiler-generated field
    if (Cheats.PlayAudioByName != null)
    {
      // ISSUE: reference to a compiler-generated field
      Cheats.PlayAudioByName(args);
    }
    return true;
  }

  private bool OnProcessCheat_spectate(string func, string[] args, string rawArgs)
  {
    if (args.Length >= 1 && args[0] == "waiting")
    {
      SpectatorManager.Get().ShowWaitingForNextGameDialog();
      return true;
    }
    if (args.Length < 4 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      Error.AddWarning("Spectate Cheat Error", "spectate cheat must have the following args:\n\nspectate ipaddress port game_handle spectator_password [gameType] [missionId]");
      return false;
    }
    JoinInfo joinInfo = new JoinInfo();
    joinInfo.ServerIpAddress = args[0];
    joinInfo.SecretKey = args[3];
    uint result1;
    if (!uint.TryParse(args[1], out result1))
    {
      Error.AddWarning("Spectate Cheat Error", "error parsing the port # (uint) argument: " + args[1]);
      return false;
    }
    joinInfo.ServerPort = result1;
    int result2;
    if (!int.TryParse(args[2], out result2))
    {
      Error.AddWarning("Spectate Cheat Error", "error parsing the game_handle (int) argument: " + args[2]);
      return false;
    }
    joinInfo.GameHandle = result2;
    joinInfo.GameType = GameType.GT_UNKNOWN;
    joinInfo.MissionId = 2;
    if (args.Length >= 5 && int.TryParse(args[4], out result2))
      joinInfo.GameType = (GameType) result2;
    if (args.Length >= 6 && int.TryParse(args[5], out result2))
      joinInfo.MissionId = result2;
    GameMgr.Get().SpectateGame(joinInfo);
    return true;
  }

  private static void SubscribePartyEvents()
  {
    if (Cheats.s_hasSubscribedToPartyEvents)
      return;
    BnetParty.OnError += (BnetParty.PartyErrorHandler) (error => Log.Party.Print("{0} code={1} feature={2} party={3} str={4}", (object) error.DebugContext, (object) error.ErrorCode, (object) error.FeatureEvent.ToString(), (object) new PartyInfo(error.PartyId, error.PartyType), (object) error.StringData));
    BnetParty.OnJoined += (BnetParty.JoinedHandler) ((e, party, reason) => Log.Party.Print("Party.OnJoined {0} party={1} reason={2}", (object) e, (object) party, reason.HasValue ? (object) reason.Value.ToString() : (object) "null"));
    BnetParty.OnPrivacyLevelChanged += (BnetParty.PrivacyLevelChangedHandler) ((party, privacy) => Log.Party.Print("Party.OnPrivacyLevelChanged party={0} privacy={1}", (object) party, (object) privacy));
    BnetParty.OnMemberEvent += (BnetParty.MemberEventHandler) ((e, party, memberId, isRolesUpdate, reason) => Log.Party.Print("Party.OnMemberEvent {0} party={1} memberId={2} isRolesUpdate={3} reason={4}", (object) e, (object) party, (object) memberId, (object) isRolesUpdate, reason.HasValue ? (object) reason.Value.ToString() : (object) "null"));
    BnetParty.OnReceivedInvite += (BnetParty.ReceivedInviteHandler) ((e, party, inviteId, inviter, inviterBattletag, invitee, reason) => Log.Party.Print("Party.OnReceivedInvite {0} party={1} inviteId={2} reason={3}", (object) e, (object) party, (object) inviteId, reason.HasValue ? (object) reason.Value.ToString() : (object) "null"));
    BnetParty.OnSentInvite += (BnetParty.SentInviteHandler) ((e, party, inviteId, inviter, invitee, senderIsMyself, reason) =>
    {
      PartyInvite sentInvite = BnetParty.GetSentInvite(party.Id, inviteId);
      Log.Party.Print("Party.OnSentInvite {0} party={1} inviteId={2} senderIsMyself={3} isRejoin={4} reason={5}", (object) e, (object) party, (object) inviteId, (object) senderIsMyself, sentInvite == null ? (object) "null" : (object) sentInvite.IsRejoin.ToString(), reason.HasValue ? (object) reason.Value.ToString() : (object) "null");
    });
    BnetParty.OnReceivedInviteRequest += (BnetParty.ReceivedInviteRequestHandler) ((e, party, request, reason) => Log.Party.Print("Party.OnReceivedInviteRequest {0} party={1} target={2} {3} requester={4} {5} reason={6}", (object) e, (object) party, (object) request.TargetName, (object) request.TargetId, (object) request.RequesterName, (object) request.RequesterId, reason.HasValue ? (object) reason.Value.ToString() : (object) "null"));
    BnetParty.OnChatMessage += (BnetParty.ChatMessageHandler) ((party, speakerId, msg) => Log.Party.Print("Party.OnChatMessage party={0} speakerId={1} msg={2}", (object) party, (object) speakerId, (object) msg));
    BnetParty.OnPartyAttributeChanged += (BnetParty.PartyAttributeChangedHandler) ((party, attr) =>
    {
      string str1 = "null";
      if (attr.Value.HasIntValue)
        str1 = "[long]" + attr.Value.IntValue.ToString();
      else if (attr.Value.HasStringValue)
        str1 = "[string]" + attr.Value.StringValue;
      else if (attr.Value.HasBlobValue)
      {
        byte[] byteArray = attr.Value.BlobValue.ToByteArray();
        if (byteArray != null)
        {
          str1 = "blobLength=" + (object) byteArray.Length;
          try
          {
            string str2 = Encoding.UTF8.GetString(byteArray);
            if (str2 != null)
              str1 = str1 + " decodedUtf8=" + str2;
          }
          catch (ArgumentException ex)
          {
          }
        }
      }
      Log.Party.Print("BnetParty.OnPartyAttributeChanged party={0} key={1} value={2}", (object) party, (object) attr.Name, (object) str1);
    });
    BnetParty.OnMemberAttributeChanged += (BnetParty.MemberAttributeChangedHandler) ((party, partyMember, attr) =>
    {
      string str3 = "null";
      if (attr.Value.HasIntValue)
        str3 = "[long]" + attr.Value.IntValue.ToString();
      else if (attr.Value.HasStringValue)
        str3 = "[string]" + attr.Value.StringValue;
      else if (attr.Value.HasBlobValue)
      {
        byte[] byteArray = attr.Value.BlobValue.ToByteArray();
        if (byteArray != null)
        {
          str3 = "blobLength=" + (object) byteArray.Length;
          try
          {
            string str4 = Encoding.UTF8.GetString(byteArray);
            if (str4 != null)
              str3 = str3 + " decodedUtf8=" + str4;
          }
          catch (ArgumentException ex)
          {
          }
        }
      }
      Log.Party.Print("BnetParty.OnMemberAttributeChanged party={0} member={1} key={2} value={3}", (object) party, (object) partyMember, (object) attr.Name, (object) str3);
    });
    Cheats.s_hasSubscribedToPartyEvents = true;
  }

  private static BnetPartyId ParsePartyId(
    string cmd,
    string arg,
    int argIndex,
    ref string errorMsg)
  {
    BnetPartyId partyId = (BnetPartyId) null;
    ulong low;
    if (ulong.TryParse(arg, out low))
    {
      BnetPartyId[] joinedPartyIds = BnetParty.GetJoinedPartyIds();
      partyId = low < 0UL || joinedPartyIds.Length == 0 || low >= (ulong) joinedPartyIds.Length ? ((IEnumerable<BnetPartyId>) joinedPartyIds).FirstOrDefault<BnetPartyId>((Func<BnetPartyId, bool>) (p => (long) p.ChannelId.Id == (long) low)) : joinedPartyIds[low];
      if (partyId == (BnetPartyId) null)
        errorMsg = "party " + cmd + ": couldn't find party at index, or with PartyId low bits: " + (object) low;
    }
    else
    {
      PartyType type;
      if (!Blizzard.T5.Core.Utils.EnumUtils.TryGetEnum<PartyType>(arg, out type))
      {
        errorMsg = "party " + cmd + ": unable to parse party (index or LowBits or type)" + (argIndex >= 0 ? " at arg index=" + (object) argIndex : "") + " (" + arg + "), please specify the Low bits of a PartyId or a PartyType.";
      }
      else
      {
        partyId = ((IEnumerable<PartyInfo>) BnetParty.GetJoinedParties()).Where<PartyInfo>((Func<PartyInfo, bool>) (info => info.Type == type)).Select<PartyInfo, BnetPartyId>((Func<PartyInfo, BnetPartyId>) (info => info.Id)).FirstOrDefault<BnetPartyId>();
        if (partyId == (BnetPartyId) null)
          errorMsg = "party " + cmd + ": no joined party with PartyType: " + arg;
      }
    }
    return partyId;
  }

  private bool OnProcessCheat_party(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      Error.AddWarning("Party Cheat Error", "USAGE: party [cmd] [args]\nCommands: create | join | leave | dissolve | list | invite | accept | decline | revoke | requestinvite | ignorerequest | setleader | kick | chat | setprivacy | setlong | setstring | setblob | clearattr | subscribe | unsubscribe");
      return false;
    }
    string cmd = args[0];
    if (cmd == "unsubscribe")
    {
      BnetParty.RemoveFromAllEventHandlers((object) this);
      Cheats.s_hasSubscribedToPartyEvents = false;
      Log.Party.Print("party {0}: unsubscribed.", (object) cmd);
      return true;
    }
    bool flag1 = true;
    string[] array1 = ((IEnumerable<string>) args).Skip<string>(1).ToArray<string>();
    string errorMsg1 = (string) null;
    Cheats.SubscribePartyEvents();
    switch (cmd)
    {
      case "accept":
      case "decline":
        bool flag2 = cmd == "accept";
        PartyInvite[] receivedInvites1 = BnetParty.GetReceivedInvites();
        if (receivedInvites1.Length == 0)
        {
          errorMsg1 = "party " + cmd + ": no received party invites.";
          break;
        }
        if (array1.Length == 0)
        {
          Log.Party.Print("NOTE: party {0} without any arguments will {0} all received invites.", (object) cmd);
          foreach (PartyInvite partyInvite in receivedInvites1)
          {
            Log.Party.Print("party {0}: {1} inviteId={2} from {3} for party {4}.", (object) cmd, flag2 ? (object) "accepting" : (object) "declining", (object) partyInvite.InviteId, (object) partyInvite.InviterName, (object) new PartyInfo(partyInvite.PartyId, partyInvite.PartyType));
            if (flag2)
              BnetParty.AcceptReceivedInvite(partyInvite.InviteId);
            else
              BnetParty.DeclineReceivedInvite(partyInvite.InviteId);
          }
          break;
        }
        for (int index = 0; index < array1.Length; ++index)
        {
          ulong indexOrId;
          if (ulong.TryParse(array1[index], out indexOrId))
          {
            PartyInvite partyInvite;
            if (indexOrId < (ulong) receivedInvites1.Length)
            {
              partyInvite = receivedInvites1[indexOrId];
            }
            else
            {
              partyInvite = ((IEnumerable<PartyInvite>) receivedInvites1).FirstOrDefault<PartyInvite>((Func<PartyInvite, bool>) (inv => (long) inv.InviteId == (long) indexOrId));
              if (partyInvite == null)
                Log.Party.Print("party {0}: unable to find received invite (id or index): {1}", (object) cmd, (object) array1[index]);
            }
            if (partyInvite != null)
            {
              Log.Party.Print("party {0}: {1} inviteId={2} from {3} for party {4}.", (object) cmd, flag2 ? (object) "accepting" : (object) "declining", (object) partyInvite.InviteId, (object) partyInvite.InviterName, (object) new PartyInfo(partyInvite.PartyId, partyInvite.PartyType));
              if (flag2)
                BnetParty.AcceptReceivedInvite(partyInvite.InviteId);
              else
                BnetParty.DeclineReceivedInvite(partyInvite.InviteId);
            }
          }
          else
            Log.Party.Print("party {0}: unable to parse invite (id or index): {1}", (object) cmd, (object) array1[index]);
        }
        break;
      case "chat":
        BnetPartyId[] joinedPartyIds1 = BnetParty.GetJoinedPartyIds();
        if (array1.Length < 1)
        {
          errorMsg1 = "party chat: must specify 1-2 arguments: party (index or LowBits or type) or a message to send.";
          break;
        }
        int count1 = 1;
        BnetPartyId partyId1 = Cheats.ParsePartyId(cmd, array1[0], -1, ref errorMsg1);
        if (partyId1 == (BnetPartyId) null && joinedPartyIds1.Length != 0)
        {
          errorMsg1 = (string) null;
          partyId1 = joinedPartyIds1[0];
          count1 = 0;
        }
        if (partyId1 != (BnetPartyId) null)
        {
          BnetParty.SendChatMessage(partyId1, string.Join(" ", ((IEnumerable<string>) array1).Skip<string>(count1).ToArray<string>()));
          break;
        }
        break;
      case "clearattr":
        BnetPartyId partyId2 = (BnetPartyId) null;
        if (array1.Length < 2)
        {
          errorMsg1 = "party " + cmd + ": must specify attributeKey.";
        }
        else
        {
          partyId2 = Cheats.ParsePartyId(cmd, array1[0], -1, ref errorMsg1);
          if (partyId2 == (BnetPartyId) null)
          {
            BnetPartyId[] joinedPartyIds2 = BnetParty.GetJoinedPartyIds();
            if (joinedPartyIds2.Length != 0)
            {
              Log.Party.Print("party {0}: treating first argument as attributeKey (and not PartyId) - will use PartyId at index 0", (object) cmd);
              errorMsg1 = (string) null;
              partyId2 = joinedPartyIds2[0];
            }
          }
          else
            Log.Party.Print("party {0}: treating first argument as PartyId (second argument will be attributeKey)", (object) cmd);
        }
        if (partyId2 != (BnetPartyId) null)
        {
          string attributeKey = array1[1];
          BattleNet.ClearPartyAttribute(partyId2, attributeKey);
          Log.Party.Print("party {0}: cleared key={1} party={2}", (object) cmd, (object) attributeKey, (object) BnetParty.GetJoinedParty(partyId2));
          break;
        }
        break;
      case "create":
        if (array1.Length < 1)
        {
          errorMsg1 = "party create: requires a PartyType: " + string.Join(" | ", System.Enum.GetValues(typeof (PartyType)).Cast<PartyType>().Select<PartyType, string>((Func<PartyType, string>) (v => v.ToString() + " (" + (object) (int) v + ")")).ToArray<string>());
          break;
        }
        int result1;
        PartyType outVal1;
        if (int.TryParse(array1[0], out result1))
          outVal1 = (PartyType) result1;
        else if (!Blizzard.T5.Core.Utils.EnumUtils.TryGetEnum<PartyType>(array1[0], out outVal1))
          errorMsg1 = "party create: unknown PartyType specified: " + array1[0];
        if (errorMsg1 == null)
        {
          byte[] byteArray = ProtobufUtil.ToByteArray((IProtoBuf) BnetUtils.CreatePegasusBnetId((BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId()));
          BnetParty.CreateParty(outVal1, ChannelApi.PartyPrivacyLevel.OpenInvitation, byteArray, (BnetParty.CreateSuccessCallback) ((t, partyId) => Log.Party.Print("BnetParty.CreateSuccessCallback type={0} partyId={1}", (object) t, (object) partyId)));
          break;
        }
        break;
      case "dissolve":
      case "leave":
        bool flag3 = cmd == "dissolve";
        if (array1.Length == 0)
        {
          Log.Party.Print("NOTE: party {0} without any arguments will {0} all joined parties.", (object) cmd);
          PartyInfo[] joinedParties = BnetParty.GetJoinedParties();
          if (joinedParties.Length == 0)
            Log.Party.Print("No joined parties.");
          foreach (PartyInfo partyInfo in joinedParties)
          {
            Log.Party.Print("party {0}: {1} party {2}", (object) cmd, flag3 ? (object) "dissolving" : (object) "leaving", (object) partyInfo);
            if (flag3)
              BnetParty.DissolveParty(partyInfo.Id);
            else
              BnetParty.Leave(partyInfo.Id);
          }
          break;
        }
        for (int argIndex = 0; argIndex < array1.Length; ++argIndex)
        {
          string str = array1[argIndex];
          string errorMsg2 = (string) null;
          BnetPartyId partyId3 = Cheats.ParsePartyId(cmd, str, argIndex, ref errorMsg2);
          if (errorMsg2 != null)
            Log.Party.Print(errorMsg2);
          if (partyId3 != (BnetPartyId) null)
          {
            Log.Party.Print("party {0}: {1} party {2}", (object) cmd, flag3 ? (object) "dissolving" : (object) "leaving", (object) BnetParty.GetJoinedParty(partyId3));
            if (flag3)
              BnetParty.DissolveParty(partyId3);
            else
              BnetParty.Leave(partyId3);
          }
        }
        break;
      case "ignorerequest":
        BnetPartyId[] joinedPartyIds3 = BnetParty.GetJoinedPartyIds();
        if (joinedPartyIds3.Length == 0)
        {
          Log.Party.Print("party {0}: no joined parties.", (object) cmd);
          break;
        }
        foreach (BnetPartyId partyId4 in joinedPartyIds3)
        {
          foreach (InviteRequest inviteRequest in BnetParty.GetInviteRequests(partyId4))
          {
            Log.Party.Print("party {0}: ignoring request to invite {0} {1} from {2} {3}.", (object) inviteRequest.TargetName, (object) inviteRequest.TargetId, (object) inviteRequest.RequesterName, (object) inviteRequest.RequesterId);
            BnetParty.IgnoreInviteRequest(partyId4, inviteRequest.TargetId);
          }
        }
        break;
      case "invite":
        BnetPartyId bnetPartyId = (BnetPartyId) null;
        int count2 = 1;
        if (array1.Length == 0)
        {
          BnetPartyId[] joinedPartyIds4 = BnetParty.GetJoinedPartyIds();
          if (joinedPartyIds4.Length != 0)
          {
            bnetPartyId = joinedPartyIds4[0];
            count2 = 0;
          }
          else
            errorMsg1 = "party invite: no joined parties to invite to.";
        }
        else
          bnetPartyId = Cheats.ParsePartyId(cmd, array1[0], -1, ref errorMsg1);
        if (bnetPartyId != (BnetPartyId) null)
        {
          string[] array2 = ((IEnumerable<string>) array1).Skip<string>(count2).ToArray<string>();
          HashSet<BnetPlayer> bnetPlayerSet = new HashSet<BnetPlayer>();
          IEnumerable<BnetPlayer> source1 = BnetFriendMgr.Get().GetFriends().Where<BnetPlayer>((Func<BnetPlayer, bool>) (p => p.IsOnline() && p.GetHearthstoneGameAccount() != (BnetGameAccount) null));
          if (array2.Length == 0)
          {
            Log.Party.Print("NOTE: party invite without any arguments will pick the first online friend.");
            BnetPlayer bnetPlayer = source1.FirstOrDefault<BnetPlayer>();
            if (bnetPlayer == null)
              errorMsg1 = "party invite: no online Hearthstone friend found.";
            else
              bnetPlayerSet.Add(bnetPlayer);
          }
          else
          {
            for (int index = 0; index < array2.Length; ++index)
            {
              string arg = array2[index];
              int result2;
              if (int.TryParse(arg, out result2))
              {
                BnetPlayer bnetPlayer = source1.ElementAtOrDefault<BnetPlayer>(result2);
                if (bnetPlayer == null)
                  errorMsg1 = "party invite: no online Hearthstone friend index " + (object) result2;
                else
                  bnetPlayerSet.Add(bnetPlayer);
              }
              else
              {
                IEnumerable<BnetPlayer> source2 = source1.Where<BnetPlayer>((Func<BnetPlayer, bool>) (p =>
                {
                  if (p.GetBattleTag().ToString().Contains(arg, StringComparison.OrdinalIgnoreCase))
                    return true;
                  return p.GetFullName() != null && p.GetFullName().Contains(arg, StringComparison.OrdinalIgnoreCase);
                }));
                if (!source2.Any<BnetPlayer>())
                {
                  errorMsg1 = "party invite: no online Hearthstone friend matching name " + arg + " (arg index " + (object) index + ")";
                }
                else
                {
                  foreach (BnetPlayer bnetPlayer in source2)
                  {
                    if (!bnetPlayerSet.Contains(bnetPlayer))
                    {
                      bnetPlayerSet.Add(bnetPlayer);
                      break;
                    }
                  }
                }
              }
            }
          }
          using (HashSet<BnetPlayer>.Enumerator enumerator = bnetPlayerSet.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              BnetPlayer current = enumerator.Current;
              BnetGameAccountId hearthstoneGameAccountId = current.GetHearthstoneGameAccountId();
              if (BnetParty.IsMember(bnetPartyId, hearthstoneGameAccountId))
              {
                Log.Party.Print("party invite: already a party member of {0}: {1}", (object) current, (object) BnetParty.GetJoinedParty(bnetPartyId));
              }
              else
              {
                Log.Party.Print("party invite: inviting {0} {1} to party {2}", (object) hearthstoneGameAccountId, (object) current, (object) BnetParty.GetJoinedParty(bnetPartyId));
                BnetParty.SendInvite(bnetPartyId, hearthstoneGameAccountId, true);
              }
            }
            break;
          }
        }
        else
          break;
      case "join":
        if (array1.Length < 1)
        {
          errorMsg1 = "party " + cmd + ": must specify an online friend index or a partyId (Hi-Lo format)";
          break;
        }
        PartyType partyType1 = PartyType.DEFAULT;
        foreach (string s1 in array1)
        {
          int length = s1.IndexOf('-');
          int result3 = -1;
          BnetPartyId partyId5 = (BnetPartyId) null;
          if (length >= 0)
          {
            string s2 = s1.Substring(0, length);
            string s3 = s1.Length > length ? s1.Substring(length + 1) : "";
            ulong high;
            ref ulong local = ref high;
            ulong result4;
            if (ulong.TryParse(s2, out local) && ulong.TryParse(s3, out result4))
              partyId5 = new BnetPartyId(high, result4);
            else
              errorMsg1 = "party " + cmd + ": unable to parse partyId (in format Hi-Lo).";
          }
          else if (int.TryParse(s1, out result3))
          {
            BnetPlayer[] array3 = BnetFriendMgr.Get().GetFriends().Where<BnetPlayer>((Func<BnetPlayer, bool>) (p => p.IsOnline() && p.GetHearthstoneGameAccount() != (BnetGameAccount) null)).ToArray<BnetPlayer>();
            if (result3 < 0 || result3 >= array3.Length)
              errorMsg1 = "party " + cmd + ": no online friend at index " + (object) result3;
            else
              errorMsg1 = "party " + cmd + ": Not-Yet-Implemented: find partyId from online friend's presence.";
          }
          else
            errorMsg1 = "party " + cmd + ": unable to parse online friend index.";
          if (partyId5 != (BnetPartyId) null)
            BnetParty.JoinParty(partyId5, partyType1);
        }
        break;
      case "kick":
        BnetPartyId partyId6 = (BnetPartyId) null;
        if (array1.Length == 0)
        {
          Log.Party.Print("NOTE: party {0} without any arguments will {0} all members for all parties (other than self).", (object) cmd);
          BnetPartyId[] joinedPartyIds5 = BnetParty.GetJoinedPartyIds();
          if (joinedPartyIds5.Length == 0)
            Log.Party.Print("party {0}: no joined parties.", (object) cmd);
          foreach (BnetPartyId partyId7 in joinedPartyIds5)
          {
            foreach (BnetParty.PartyMember member in BnetParty.GetMembers(partyId7))
            {
              if (!((BnetEntityId) member.GameAccountId == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId()))
              {
                Log.Party.Print("party {0}: kicking memberId={1} from party {2}.", (object) cmd, (object) member.GameAccountId, (object) BnetParty.GetJoinedParty(partyId7));
                BnetParty.KickMember(partyId7, member.GameAccountId);
              }
            }
          }
        }
        else
          partyId6 = Cheats.ParsePartyId(cmd, array1[0], -1, ref errorMsg1);
        if (partyId6 != (BnetPartyId) null)
        {
          PartyInfo joinedParty = BnetParty.GetJoinedParty(partyId6);
          BnetParty.PartyMember[] members = BnetParty.GetMembers(partyId6);
          if (members.Length == 1)
          {
            errorMsg1 = "party " + cmd + ": no members (other than self) for party " + (object) joinedParty;
            break;
          }
          string[] array4 = ((IEnumerable<string>) array1).Skip<string>(1).ToArray<string>();
          if (array4.Length == 0)
          {
            Log.Party.Print("NOTE: party {0} without specifying member index will {0} all members (other than self).", (object) cmd);
            foreach (BnetParty.PartyMember partyMember in members)
            {
              if (!((BnetEntityId) partyMember.GameAccountId == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId()))
              {
                Log.Party.Print("party {0}: kicking memberId={1} from party {2}.", (object) cmd, (object) partyMember.GameAccountId, (object) joinedParty);
                BnetParty.KickMember(partyId6, partyMember.GameAccountId);
              }
            }
            break;
          }
          for (int index = 0; index < array4.Length; ++index)
          {
            ulong indexOrId;
            if (ulong.TryParse(array4[index], out indexOrId))
            {
              BnetParty.PartyMember partyMember;
              if (indexOrId < (ulong) members.Length)
              {
                partyMember = members[indexOrId];
              }
              else
              {
                partyMember = ((IEnumerable<BnetParty.PartyMember>) members).FirstOrDefault<BnetParty.PartyMember>((Func<BnetParty.PartyMember, bool>) (m => (long) m.GameAccountId.Low == (long) indexOrId));
                if (partyMember == null)
                  Log.Party.Print("party {0}: unable to find member (id or index): {1} for party {2}", (object) cmd, (object) array4[index], (object) joinedParty);
              }
              if (partyMember != null)
              {
                if ((BnetEntityId) partyMember.GameAccountId == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId())
                {
                  Log.Party.Print("party {0}: cannot kick yourself (argIndex={1}); party={2}", (object) cmd, (object) index, (object) joinedParty);
                }
                else
                {
                  Log.Party.Print("party {0}: kicking memberId={1} from party {2}.", (object) cmd, (object) partyMember.GameAccountId, (object) joinedParty);
                  BnetParty.KickMember(partyId6, partyMember.GameAccountId);
                }
              }
            }
            else
              Log.Party.Print("party {0}: unable to parse member (id or index): {1}", (object) cmd, (object) array4[index]);
          }
          break;
        }
        break;
      case "list":
      case "subscribe":
        IEnumerable<BnetPartyId> bnetPartyIds1 = (IEnumerable<BnetPartyId>) null;
        if (array1.Length == 0)
        {
          PartyInfo[] joinedParties = BnetParty.GetJoinedParties();
          if (joinedParties.Length == 0)
          {
            Log.Party.Print("party list: no joined parties.");
          }
          else
          {
            Log.Party.Print("party list: listing all joined parties and the details of the party at index 0.");
            bnetPartyIds1 = (IEnumerable<BnetPartyId>) new BnetPartyId[1]
            {
              joinedParties[0].Id
            };
          }
          for (int index = 0; index < joinedParties.Length; ++index)
            Log.Party.Print("   {0}", (object) Cheats.GetPartySummary(joinedParties[index], index));
        }
        else
          bnetPartyIds1 = ((IEnumerable<string>) array1).Select<string, BnetPartyId>((Func<string, int, BnetPartyId>) ((a, i) =>
          {
            string errorMsg3 = (string) null;
            BnetPartyId partyId8 = Cheats.ParsePartyId(cmd, a, i, ref errorMsg3);
            if (errorMsg3 == null)
              return partyId8;
            Log.Party.Print(errorMsg3);
            return partyId8;
          })).Where<BnetPartyId>((Func<BnetPartyId, bool>) (p => p != (BnetPartyId) null));
        if (bnetPartyIds1 != null)
        {
          int index1 = -1;
          foreach (BnetPartyId partyId9 in bnetPartyIds1)
          {
            ++index1;
            PartyInfo joinedParty = BnetParty.GetJoinedParty(partyId9);
            Log.Party.Print("party {0}: {1}", (object) cmd, (object) Cheats.GetPartySummary(BnetParty.GetJoinedParty(partyId9), index1));
            BnetParty.PartyMember[] members = BnetParty.GetMembers(partyId9);
            if (members.Length == 0)
              Log.Party.Print("   no members.");
            else
              Log.Party.Print("   members:");
            for (int index2 = 0; index2 < members.Length; ++index2)
            {
              bool flag4 = (BnetEntityId) members[index2].GameAccountId == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId();
              Log.Party.Print("      [{0}] {1} isMyself={2} isLeader={3} roleIds={4}", (object) index2, (object) members[index2].GameAccountId, (object) flag4, (object) members[index2].IsLeader(joinedParty.Type), (object) string.Join(",", ((IEnumerable<uint>) members[index2].RoleIds).Select<uint, string>((Func<uint, string>) (r => r.ToString())).ToArray<string>()));
            }
            PartyInvite[] sentInvites = BnetParty.GetSentInvites(partyId9);
            if (sentInvites.Length == 0)
              Log.Party.Print("   no sent invites.");
            else
              Log.Party.Print("   sent invites:");
            for (int index3 = 0; index3 < sentInvites.Length; ++index3)
              Log.Party.Print("      {0}", (object) Cheats.GetPartyInviteSummary(sentInvites[index3], index3));
            Blizzard.GameService.Protocol.V2.Client.Attribute[] attributes;
            BattleNet.GetAllPartyAttributes(partyId9, out attributes);
            if (attributes.Length == 0)
              Log.Party.Print("   no party attributes.");
            else
              Log.Party.Print("   party attributes:");
            for (int index4 = 0; index4 < attributes.Length; ++index4)
            {
              Blizzard.GameService.Protocol.V2.Client.Attribute attribute = attributes[index4];
              string str1 = attribute.Value == null ? "<null>" : string.Format("[{0}]{1}", (object) attribute.Value.GetType().Name, (object) attribute.Value.ToString());
              if (attribute.Value.HasBlobValue)
              {
                byte[] byteArray = attribute.Value.BlobValue.ToByteArray();
                str1 = "blobLength=" + (object) byteArray.Length;
                try
                {
                  string str2 = Encoding.UTF8.GetString(byteArray);
                  if (str2 != null)
                    str1 = str1 + " decodedUtf8=" + str2;
                }
                catch (ArgumentException ex)
                {
                }
              }
              Log.Party.Print("      {0}={1}", (object) (attribute.Name ?? "<null>"), (object) str1);
            }
          }
        }
        PartyInvite[] receivedInvites2 = BnetParty.GetReceivedInvites();
        if (receivedInvites2.Length == 0)
          Log.Party.Print("party list: no received party invites.");
        else
          Log.Party.Print("party list: received party invites:");
        for (int index = 0; index < receivedInvites2.Length; ++index)
          Log.Party.Print("   {0}", (object) Cheats.GetPartyInviteSummary(receivedInvites2[index], index));
        break;
      case "requestinvite":
        if (array1.Length < 2)
        {
          errorMsg1 = "party " + cmd + ": must specify a partyId (Hi-Lo format) and an online friend index";
          break;
        }
        PartyType partyType2 = PartyType.DEFAULT;
        foreach (string s4 in array1)
        {
          int length = s4.IndexOf('-');
          int result5 = -1;
          BnetPartyId partyId10 = (BnetPartyId) null;
          BnetGameAccountId whomToAskForApproval = (BnetGameAccountId) null;
          if (length >= 0)
          {
            string s5 = s4.Substring(0, length);
            string s6 = s4.Length > length ? s4.Substring(length + 1) : "";
            ulong high;
            ref ulong local = ref high;
            ulong result6;
            if (ulong.TryParse(s5, out local) && ulong.TryParse(s6, out result6))
              partyId10 = new BnetPartyId(high, result6);
            else
              errorMsg1 = "party " + cmd + ": unable to parse partyId (in format Hi-Lo).";
          }
          else if (int.TryParse(s4, out result5))
          {
            BnetPlayer[] array5 = BnetFriendMgr.Get().GetFriends().Where<BnetPlayer>((Func<BnetPlayer, bool>) (p => p.IsOnline() && p.GetHearthstoneGameAccount() != (BnetGameAccount) null)).ToArray<BnetPlayer>();
            if (result5 < 0 || result5 >= array5.Length)
              errorMsg1 = "party " + cmd + ": no online friend at index " + (object) result5;
            else
              whomToAskForApproval = array5[result5].GetHearthstoneGameAccountId();
          }
          else
            errorMsg1 = "party " + cmd + ": unable to parse online friend index.";
          if (partyId10 != (BnetPartyId) null && (BnetEntityId) whomToAskForApproval != (BnetEntityId) null)
            BnetParty.RequestInvite(partyId10, whomToAskForApproval, BnetPresenceMgr.Get().GetMyGameAccountId(), partyType2);
        }
        break;
      case "revoke":
        BnetPartyId partyId11 = (BnetPartyId) null;
        if (array1.Length == 0)
        {
          Log.Party.Print("NOTE: party {0} without any arguments will {0} all sent invites for all parties.", (object) cmd);
          BnetPartyId[] joinedPartyIds6 = BnetParty.GetJoinedPartyIds();
          if (joinedPartyIds6.Length == 0)
            Log.Party.Print("party {0}: no joined parties.", (object) cmd);
          foreach (BnetPartyId partyId12 in joinedPartyIds6)
          {
            foreach (PartyInvite sentInvite in BnetParty.GetSentInvites(partyId12))
            {
              Log.Party.Print("party {0}: revoking inviteId={1} from {2} for party {3}.", (object) cmd, (object) sentInvite.InviteId, (object) sentInvite.InviterName, (object) BnetParty.GetJoinedParty(partyId12));
              BnetParty.RevokeSentInvite(partyId12, sentInvite.InviteId);
            }
          }
        }
        else
          partyId11 = Cheats.ParsePartyId(cmd, array1[0], -1, ref errorMsg1);
        if (partyId11 != (BnetPartyId) null)
        {
          PartyInfo joinedParty = BnetParty.GetJoinedParty(partyId11);
          PartyInvite[] sentInvites = BnetParty.GetSentInvites(partyId11);
          if (sentInvites.Length == 0)
          {
            errorMsg1 = "party " + cmd + ": no sent invites for party " + (object) joinedParty;
            break;
          }
          string[] array6 = ((IEnumerable<string>) array1).Skip<string>(1).ToArray<string>();
          if (array6.Length == 0)
          {
            Log.Party.Print("NOTE: party {0} without specifying InviteId (or index) will {0} all sent invites.", (object) cmd);
            foreach (PartyInvite partyInvite in sentInvites)
            {
              Log.Party.Print("party {0}: revoking inviteId={1} from {2} for party {3}.", (object) cmd, (object) partyInvite.InviteId, (object) partyInvite.InviterName, (object) joinedParty);
              BnetParty.RevokeSentInvite(partyId11, partyInvite.InviteId);
            }
            break;
          }
          for (int index = 0; index < array6.Length; ++index)
          {
            ulong indexOrId;
            if (ulong.TryParse(array6[index], out indexOrId))
            {
              PartyInvite partyInvite;
              if (indexOrId < (ulong) sentInvites.Length)
              {
                partyInvite = sentInvites[indexOrId];
              }
              else
              {
                partyInvite = ((IEnumerable<PartyInvite>) sentInvites).FirstOrDefault<PartyInvite>((Func<PartyInvite, bool>) (inv => (long) inv.InviteId == (long) indexOrId));
                if (partyInvite == null)
                  Log.Party.Print("party {0}: unable to find sent invite (id or index): {1} for party {2}", (object) cmd, (object) array6[index], (object) joinedParty);
              }
              if (partyInvite != null)
              {
                Log.Party.Print("party {0}: revoking inviteId={1} from {2} for party {3}.", (object) cmd, (object) partyInvite.InviteId, (object) partyInvite.InviterName, (object) joinedParty);
                BnetParty.RevokeSentInvite(partyId11, partyInvite.InviteId);
              }
            }
            else
              Log.Party.Print("party {0}: unable to parse invite (id or index): {1}", (object) cmd, (object) array6[index]);
          }
          break;
        }
        break;
      case "setblob":
      case "setlong":
      case "setstring":
        bool flag5 = cmd == "setlong";
        bool flag6 = cmd == "setstring";
        bool flag7 = cmd == "setblob";
        int index5 = 1;
        BnetPartyId partyId13 = (BnetPartyId) null;
        if (array1.Length < 2)
        {
          errorMsg1 = "party " + cmd + ": must specify attributeKey and a value.";
        }
        else
        {
          partyId13 = Cheats.ParsePartyId(cmd, array1[0], -1, ref errorMsg1);
          if (partyId13 == (BnetPartyId) null)
          {
            BnetPartyId[] joinedPartyIds7 = BnetParty.GetJoinedPartyIds();
            if (joinedPartyIds7.Length != 0)
            {
              Log.Party.Print("party {0}: treating first argument as attributeKey (and not PartyId) - will use PartyId at index 0", (object) cmd);
              errorMsg1 = (string) null;
              partyId13 = joinedPartyIds7[0];
            }
          }
          else
            Log.Party.Print("party {0}: treating first argument as PartyId (second argument will be attributeKey)", (object) cmd);
        }
        if (partyId13 != (BnetPartyId) null)
        {
          bool flag8 = false;
          string name = array1[index5];
          string str = string.Join(" ", ((IEnumerable<string>) array1).Skip<string>(index5 + 1).ToArray<string>());
          if (flag5)
          {
            long result7;
            if (long.TryParse(str, out result7))
            {
              BattleNet.SetPartyAttributes(partyId13, BnetAttribute.CreateAttribute(name, result7));
              flag8 = true;
            }
          }
          else if (flag6)
          {
            BattleNet.SetPartyAttributes(partyId13, BnetAttribute.CreateAttribute(name, str));
            flag8 = true;
          }
          else if (flag7)
          {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            BattleNet.SetPartyAttributes(partyId13, BnetAttribute.CreateAttribute(name, bytes));
            flag8 = true;
          }
          else
            errorMsg1 = "party " + cmd + ": unhandled attribute type!";
          if (flag8)
          {
            Log.Party.Print("party {0}: complete key={1} val={2} party={3}", (object) cmd, (object) name, (object) str, (object) BnetParty.GetJoinedParty(partyId13));
            break;
          }
          break;
        }
        break;
      case "setleader":
        IEnumerable<BnetPartyId> bnetPartyIds2 = (IEnumerable<BnetPartyId>) null;
        int result8 = -1;
        if (array1.Length >= 2 && (!int.TryParse(array1[1], out result8) || result8 < 0))
          errorMsg1 = string.Format("party {0}: invalid memberIndex={1}", (object) cmd, (object) array1[1]);
        if (array1.Length == 0)
        {
          Log.Party.Print("NOTE: party {0} without any arguments will {0} to first member in all parties.", (object) cmd);
          BnetPartyId[] joinedPartyIds8 = BnetParty.GetJoinedPartyIds();
          if (joinedPartyIds8.Length == 0)
            Log.Party.Print("party {0}: no joined parties.", (object) cmd);
          else
            bnetPartyIds2 = (IEnumerable<BnetPartyId>) joinedPartyIds8;
        }
        else
        {
          BnetPartyId partyId14 = Cheats.ParsePartyId(cmd, array1[0], -1, ref errorMsg1);
          if (partyId14 != (BnetPartyId) null)
            bnetPartyIds2 = (IEnumerable<BnetPartyId>) new BnetPartyId[1]
            {
              partyId14
            };
        }
        if (bnetPartyIds2 != null)
        {
          using (IEnumerator<BnetPartyId> enumerator = bnetPartyIds2.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              BnetPartyId current = enumerator.Current;
              BnetParty.PartyMember[] members = BnetParty.GetMembers(current);
              if (result8 >= 0)
              {
                if (result8 >= members.Length)
                {
                  Log.Party.Print("party {0}: party={1} has no member at index={2}", (object) cmd, (object) BnetParty.GetJoinedParty(current), (object) result8);
                }
                else
                {
                  BnetParty.PartyMember partyMember = members[result8];
                  BnetParty.SetLeader(current, partyMember.GameAccountId);
                }
              }
              else if (((IEnumerable<BnetParty.PartyMember>) members).Any<BnetParty.PartyMember>((Func<BnetParty.PartyMember, bool>) (m => (BnetEntityId) m.GameAccountId != (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId())))
                BnetParty.SetLeader(current, ((IEnumerable<BnetParty.PartyMember>) members).First<BnetParty.PartyMember>((Func<BnetParty.PartyMember, bool>) (m => (BnetEntityId) m.GameAccountId != (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId())).GameAccountId);
              else
                Log.Party.Print("party {0}: party={1} has no member not myself to set as leader.", (object) cmd, (object) BnetParty.GetJoinedParty(current));
            }
            break;
          }
        }
        else
          break;
      case "setprivacy":
        BnetPartyId partyId15 = (BnetPartyId) null;
        if (array1.Length < 2)
          errorMsg1 = "party setprivacy: must specify a party (index or LowBits or type) and a PrivacyLevel: " + string.Join(" | ", System.Enum.GetValues(typeof (ChannelApi.PartyPrivacyLevel)).Cast<ChannelApi.PartyPrivacyLevel>().Select<ChannelApi.PartyPrivacyLevel, string>((Func<ChannelApi.PartyPrivacyLevel, string>) (v => v.ToString() + " (" + (object) (int) v + ")")).ToArray<string>());
        else
          partyId15 = Cheats.ParsePartyId(cmd, array1[0], -1, ref errorMsg1);
        if (partyId15 != (BnetPartyId) null)
        {
          ChannelApi.PartyPrivacyLevel? nullable = new ChannelApi.PartyPrivacyLevel?();
          int result9;
          if (int.TryParse(array1[1], out result9))
          {
            nullable = new ChannelApi.PartyPrivacyLevel?((ChannelApi.PartyPrivacyLevel) result9);
          }
          else
          {
            ChannelApi.PartyPrivacyLevel outVal2;
            if (!Blizzard.T5.Core.Utils.EnumUtils.TryGetEnum<ChannelApi.PartyPrivacyLevel>(array1[1], out outVal2))
              errorMsg1 = "party setprivacy: unknown PrivacyLevel specified: " + array1[1];
            else
              nullable = new ChannelApi.PartyPrivacyLevel?(outVal2);
          }
          if (nullable.HasValue)
          {
            Log.Party.Print("party setprivacy: setting PrivacyLevel={0} for party {1}.", (object) nullable.Value, (object) BnetParty.GetJoinedParty(partyId15));
            BnetParty.SetPrivacy(partyId15, nullable.Value);
            break;
          }
          break;
        }
        break;
      default:
        errorMsg1 = "party: unknown party cmd: " + cmd;
        break;
    }
    if (errorMsg1 != null)
    {
      Log.Party.Print(errorMsg1);
      Error.AddWarning("Party Cheat Error", errorMsg1);
      flag1 = false;
    }
    return flag1;
  }

  private static string GetPartyInviteSummary(PartyInvite invite, int index) => string.Format("{0}: inviteId={1} sender={2} recipient={3} party={4}", index >= 0 ? (object) string.Format("[{0}] ", (object) index) : (object) "", (object) invite.InviteId, (object) (invite.InviterId.ToString() + " " + invite.InviterName), (object) invite.InviteeId, (object) new PartyInfo(invite.PartyId, invite.PartyType));

  private static string GetPartySummary(PartyInfo info, int index)
  {
    BnetParty.PartyMember leader = BnetParty.GetLeader(info.Id);
    return string.Format("{0}{1}: members={2} invites={3} privacy={4} leader={5}", index >= 0 ? (object) string.Format("[{0}] ", (object) index) : (object) "", (object) info, (object) (BnetParty.CountMembers(info.Id).ToString() + (BnetParty.IsPartyFull(info.Id) ? (object) "(full)" : (object) "")), (object) BnetParty.GetSentInvites(info.Id).Length, (object) BnetParty.GetPrivacyLevel(info.Id), leader == null ? (object) "null" : (object) leader.GameAccountId.ToString());
  }

  private bool OnProcessCheat_cheat(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string str = "spawncard, drawcard, loadcard, cyclehand, shuffle, addmana, readymana, maxmana, nocosts, healhero, healentity, nuke, damage, settag, ready, exhaust, freeze, move, undo, destroyhero, tiegame, getgsd, aiplaylastspawnedcard, forcestallingprevention, endturn, logrelay";
    if (autofillData != null)
    {
      string[] values = (string[]) null;
      string[] strArray1 = new string[2]
      {
        "friendly",
        "opponent"
      };
      string[] strArray2 = new string[7]
      {
        "InPlay",
        "InDeck",
        "InHand",
        "InGraveyard",
        "InRemovedFromGame",
        "InSetAside",
        "InSecret"
      };
      Func<string[]> func1 = (Func<string[]>) (() => GameDbf.GetIndex().GetAllCardIds().ToArray());
      string searchTerm = autofillData.m_lastAutofillParamPrefix ?? (args.Length == 0 ? string.Empty : ((IEnumerable<string>) args).Last<string>());
      int length = args.Length;
      if (rawArgs.EndsWith(" "))
      {
        searchTerm = string.Empty;
        ++length;
      }
      if (length > 1 && !string.IsNullOrEmpty(args[0]))
      {
        str = (string) null;
        switch (args[0])
        {
          case "addmana":
          case "cyclehand":
          case "destroyhero":
          case "drawcard":
          case "healhero":
          case "maxmana":
          case "nuke":
          case "readymana":
          case "shuffle":
            if (length == 2)
            {
              values = strArray1;
              break;
            }
            break;
          case "getgsd":
            if (length == 2)
            {
              values = strArray1;
              break;
            }
            break;
          case "loadcard":
            if (length == 2)
            {
              values = func1();
              break;
            }
            break;
          case "move":
            if (length == 3)
            {
              values = strArray2;
              break;
            }
            break;
          case "spawncard":
            switch (length)
            {
              case 2:
                values = func1();
                break;
              case 3:
                values = strArray2;
                break;
              case 4:
                values = new string[2]{ "1", "0" };
                break;
            }
            break;
        }
      }
      if (values == null)
      {
        if (str == null)
          return false;
        values = str.Split(new char[2]{ ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
      }
      return Cheats.ProcessAutofillParam((IEnumerable<string>) values, searchTerm, autofillData);
    }
    if (!Network.Get().IsConnectedToGameServer())
    {
      UIStatus.Get().AddInfoNoRichText("Not connected to a game. Cannot send cheat command.");
      return true;
    }
    string command = rawArgs;
    Network.Get().SendDebugConsoleCommand(command);
    return true;
  }

  private bool OnProcessCheat_autohand(string func, string[] args, string rawArgs)
  {
    bool boolVal;
    if (args.Length == 0 || !GeneralUtils.TryParseBool(args[0], out boolVal) || (UnityEngine.Object) InputManager.Get() == (UnityEngine.Object) null)
      return false;
    string message = !boolVal ? "auto hand hiding is off" : "auto hand hiding is on";
    Debug.Log((object) message);
    UIStatus.Get().AddInfo(message);
    InputManager.Get().SetHideHandAfterPlayingCard(boolVal);
    return true;
  }

  private bool OnProcessCheat_adventureChallengeUnlock(string func, string[] args, string rawArgs)
  {
    int result;
    if (args.Length < 1 || !int.TryParse(args[0].ToLowerInvariant(), out result))
      return false;
    AdventureMissionDisplay.Get().ShowClassChallengeUnlock(new List<int>()
    {
      result
    });
    return true;
  }

  private bool OnProcessCheat_iks(string func, string[] args, string rawArgs)
  {
    InnKeepersSpecial.Get().InitializeJsonURL(args[0]);
    InnKeepersSpecial.Get().ResetAdUrl();
    Processor.RunCoroutine(this.TriggerWelcomeQuestShow());
    return true;
  }

  private IEnumerator TriggerWelcomeQuestShow()
  {
    yield return (object) new WaitForSeconds(1f);
    while (InnKeepersSpecial.Get().ProcessingResponse)
      yield return (object) new WaitForSeconds(1f);
    QuestManager.Get().SimulateQuestNotificationPopup(QuestPool.QuestPoolType.DAILY);
  }

  private bool OnProcessCheat_iksgameaction(string func, string[] args, string rawArgs)
  {
    if (string.IsNullOrEmpty(rawArgs))
    {
      UIStatus.Get().AddError("Please specify a game action.");
      return true;
    }
    DeepLinkManager.ExecuteDeepLink(args, DeepLinkManager.DeepLinkSource.INNKEEPERS_SPECIAL, false);
    return true;
  }

  private bool OnProcessCheat_iksseen(string func, string[] args, string rawArgs)
  {
    if (string.IsNullOrEmpty(rawArgs))
    {
      UIStatus.Get().AddError("Please specify a game action.");
      return true;
    }
    string gameAction = string.Join(" ", args);
    UIStatus.Get().AddInfo("Has Interacted With Product: " + InnKeepersSpecial.Get().HasInteractedWithAdvertisedProduct(gameAction).ToString());
    return true;
  }

  private bool OnProcessCheat_quote(string func, string[] args, string rawArgs)
  {
    string prefabPath = "innkeeper";
    string key = "VO_INNKEEPER_FIRST_100_GOLD";
    string soundPath = "VO_INNKEEPER_FIRST_100_GOLD.prefab:c6a50337099a454488acd96d2f37320f";
    if ((args.Length < 1 ? 0 : (args[0] == "default" ? 1 : 0)) == 0)
    {
      if (args.Length < 2)
      {
        UIStatus.Get().AddError("Please specify 2 arguments: CharacterPrefabAssetRef GameStringsKey [AudioAssetRef]\nExamples:\nquote default\nquote innkeeper VO_TUTORIAL_01_ANNOUNCER_05 VO_TUTORIAL_01_ANNOUNCER_05.prefab:635b33010e4704a42a87c7625b5b5ada\nquote Barnes_Quote.prefab:2e7e9f28b5bc37149a12b2e5feaa244a VO_Barnes_Male_Human_JulianneWin_01 VO_Barnes_Male_Human_JulianneWin_01.prefab:09d4c4aaf43ac634aaf325c2badc72a8", 5f * UnityEngine.Time.timeScale);
        return true;
      }
      prefabPath = args[0];
      key = args[1];
      soundPath = key;
      if (args.Length > 2)
        soundPath = args[2];
    }
    if (prefabPath.ToLowerInvariant().Contains("innkeeper"))
      NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.ALL, NotificationManager.DEFAULT_CHARACTER_POS, GameStrings.Get(key), soundPath);
    else
      NotificationManager.Get().CreateCharacterQuote(prefabPath, NotificationManager.DEFAULT_CHARACTER_POS, GameStrings.Get(key), soundPath);
    return true;
  }

  private bool OnProcessCheat_popuptext(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1)
      return false;
    string text = args[0];
    NotificationManager.Get().CreatePopupText(UserAttentionBlocker.ALL, Box.Get().m_LeftDoor.transform.position, TutorialEntity.GetTextScale(), text);
    return true;
  }

  private bool OnProcessCheat_demotext(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1)
      return false;
    string demoText = args[0];
    DemoMgr.Get().CreateDemoText(demoText);
    return true;
  }

  private bool OnProcessCheat_alerttext(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1)
      return false;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_text = rawArgs,
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.NONE
    });
    return true;
  }

  private bool OnProcessCheat_logtext(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1)
      return false;
    if (args.Length > 1)
    {
      string format = rawArgs.Substring(rawArgs.IndexOf(' ') + 1);
      string str = args[0];
      if (!(str == "debug"))
      {
        if (!(str == "info"))
        {
          if (!(str == "warning"))
          {
            if (str == "error")
            {
              Log.All.PrintError(format);
              return true;
            }
          }
          else
          {
            Log.All.PrintWarning(format);
            return true;
          }
        }
        else
        {
          Log.All.PrintInfo(format);
          return true;
        }
      }
      else
      {
        Log.All.PrintDebug(format);
        return true;
      }
    }
    Log.All.Print(rawArgs);
    return true;
  }

  private bool OnProcessCheat_logenable(string func, string[] args, string rawArgs)
  {
    if (((IEnumerable<string>) args).Count<string>() < 3)
      return false;
    string logName = args[0];
    LogInfo logInfo = LogSystem.Get().GetLogInfo(logName);
    if (logInfo == null)
      return false;
    string str1 = args[1];
    string str2 = args[2];
    bool flag = !str2.Equals("false", StringComparison.OrdinalIgnoreCase) && str2 != "0";
    if (!(str1 == "file"))
    {
      if (!(str1 == "screen"))
      {
        if (!(str1 == "console"))
          return false;
        logInfo.m_consolePrinting = flag;
      }
      else
        logInfo.m_screenPrinting = flag;
    }
    else
      logInfo.m_filePrinting = flag;
    LogSystem.Get().SetLogInfo(logName, logInfo);
    return true;
  }

  private bool OnProcessCheat_loglevel(string func, string[] args, string rawArgs)
  {
    if (((IEnumerable<string>) args).Count<string>() < 2)
      return false;
    string logName = ((IEnumerable<string>) args).ElementAtOrDefault<string>(0);
    Blizzard.T5.Logging.LogLevel result;
    if (!Blizzard.T5.Core.Utils.EnumUtils.TryGetEnum<Blizzard.T5.Logging.LogLevel>(((IEnumerable<string>) args).ElementAtOrDefault<string>(1), StringComparison.OrdinalIgnoreCase, out result))
      return false;
    LogInfo logInfo = LogSystem.Get().GetLogInfo(logName);
    if (logInfo == null)
      return false;
    logInfo.m_minLevel = result;
    LogSystem.Get().SetLogInfo(logName, logInfo);
    return true;
  }

  private bool OnProcessCheat_cardchangereset(string func, string[] args, string rawArgs)
  {
    if (args.Length == 1)
    {
      string eventName = args[0];
      long eventIdFromEventName = SpecialEventManager.Get().GetEventIdFromEventName(eventName);
      List<long> values;
      GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LIST_OF_SEEN_CARD_CHANGES, out values);
      if (values != null && values.Contains(eventIdFromEventName))
      {
        values.Remove(eventIdFromEventName);
        GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LIST_OF_SEEN_CARD_CHANGES, values.ToArray()));
        UIStatus.Get().AddInfo("Card Change popup for " + eventName + " will be displayed on next login", 10f);
        return true;
      }
      UIStatus.Get().AddInfo("Error: ${eventName} does not exist or subkey is empty", 10f);
      return false;
    }
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LIST_OF_SEEN_CARD_CHANGES, new long[0]));
    UIStatus.Get().AddInfo("Card Change popup for all events will be displayed on next login", 10f);
    return true;
  }

  private bool OnProcessCheat_loginpopupsequence(string func, string[] args, string rawArgs)
  {
    bool popupsForNewPlayer = PopupDisplayManager.SuppressPopupsForNewPlayer;
    bool shouldDisableNotificationOnLogin = PopupDisplayManager.ShouldDisableNotificationOnLogin();
    PopupDisplayManager.Get().LoginPopups.ShowLoginPopupSequence(popupsForNewPlayer, shouldDisableNotificationOnLogin, PopupDisplayManager.Get().CardPopups);
    return true;
  }

  private bool OnProcessCheat_loginpopupreset(string func, string[] args, string rawArgs)
  {
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.PLAYER_OPTIONS, GameSaveKeySubkeyId.LOGIN_POPUP_SEQUENCE_SEEN_POPUPS, new long[0]));
    return true;
  }

  private bool OnProcessCheat_favoritehero(string func, string[] args, string rawArgs)
  {
    if (!(SceneMgr.Get().GetScene() is CollectionManagerScene))
    {
      Debug.LogWarning((object) "OnProcessCheat_favoritehero must be used from the CollectionManagaer!");
      return false;
    }
    int result1;
    TAG_CLASS outVal1;
    if (args.Length != 3 || !int.TryParse(args[0].ToLowerInvariant(), out result1) || !Blizzard.T5.Core.Utils.EnumUtils.TryCast<TAG_CLASS>((object) result1, out outVal1))
      return false;
    string str = args[1];
    int result2;
    TAG_PREMIUM outVal2;
    if (!int.TryParse(args[2].ToLowerInvariant(), out result2) || !Blizzard.T5.Core.Utils.EnumUtils.TryCast<TAG_PREMIUM>((object) result2, out outVal2))
      return false;
    NetCache.CardDefinition hero = new NetCache.CardDefinition()
    {
      Name = str,
      Premium = outVal2
    };
    Log.All.Print("OnProcessCheat_favoritehero setting favorite hero to {0} for class {1}", (object) hero, (object) outVal1);
    Network.Get().SetFavoriteHero(outVal1, hero, true);
    return true;
  }

  private bool OnProcessCheat_PlayFinisher(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string[] values = new string[3]
    {
      "help",
      "player",
      "opponent"
    };
    if (args.Length < 1 || string.IsNullOrEmpty(rawArgs))
    {
      if (autofillData != null)
        return Cheats.ProcessAutofillParam((IEnumerable<string>) values, string.Empty, autofillData);
      UIStatus.Get().AddError("Must specify a sub-command.");
      return true;
    }
    string lower = args[0].ToLower();
    string[] array = ((IEnumerable<string>) args).Skip<string>(1).ToArray<string>();
    if (autofillData != null && args.Length == 1 && !rawArgs.EndsWith(" "))
    {
      string searchTerm = lower;
      return Cheats.ProcessAutofillParam((IEnumerable<string>) values, searchTerm, autofillData);
    }
    if (lower == "help" || !(lower == "player") && !(lower == "opponent"))
    {
      StringBuilder stringBuilder = new StringBuilder("finisher help - show finisher cheats");
      stringBuilder.AppendLine("finisher player id=X large - player does large finisher X");
      stringBuilder.AppendLine("finisher opponent id=X small - opponent does small finisher X");
      UIStatus.Get().AddInfo(stringBuilder.ToString(), 10f);
      return true;
    }
    GameState gameState = GameState.Get();
    if (gameState == null)
    {
      UIStatus.Get().AddError("Cannot play a finisher. GameState is null. Are you in a game?", 10f);
      return true;
    }
    GameMgr gameMgr = GameMgr.Get();
    if (gameMgr == null)
    {
      UIStatus.Get().AddError("Cannot play a finisher. GameMgr is somehow null.", 10f);
      return true;
    }
    if (!gameMgr.IsBattlegrounds() && !gameMgr.IsBattlegroundsTutorial())
    {
      UIStatus.Get().AddError("Cannot play a finisher. Not in a Battlegrounds game or Battlegrounds tutorial.", 10f);
      return true;
    }
    Actor actor1 = gameState.GetFriendlySidePlayer().GetHeroCard().GetActor();
    Actor actor2 = gameState.GetOpposingSidePlayer().GetHeroCard().GetActor();
    Actor actor3 = lower == "player" ? actor1 : actor2;
    Actor actor4 = lower == "player" ? actor2 : actor1;
    string[] source = new string[3]
    {
      "id",
      "small",
      "large"
    };
    bool flag = false;
    int result = 0;
    foreach (string str1 in array)
    {
      string str2 = str1;
      string s = (string) null;
      int length = str1.IndexOf('=');
      if (length >= 0)
      {
        str2 = str1.Substring(0, length);
        s = str1.Substring(length + 1);
      }
      if (!((IEnumerable<string>) source).Contains<string>(str2))
        UIStatus.Get().AddError("Unrecognized sub command \"" + str2 + "\". Enter cheat \"finisher help\" for more information.", 10f);
      else if (!(str2 == "id"))
      {
        if (!(str2 == "small"))
        {
          if (str2 == "large")
            flag = true;
        }
        else
          flag = false;
      }
      else
      {
        if (!int.TryParse(s, out result))
        {
          UIStatus.Get().AddError("Could not parse \"" + s + "\" as an integer. Enter cheat \"finisher help\" for more information.");
          return true;
        }
        if (GameDbf.BattlegroundsFinisher.GetRecord(result) == null)
        {
          UIStatus.Get().AddError(string.Format("No finisher with ID:\"{0}\". Enter cheat \"finisher help\" for more information.", (object) result));
          return true;
        }
      }
    }
    PowerTaskList taskList = new PowerTaskList();
    Network.HistBlockStart blockStart = new Network.HistBlockStart(HistoryBlock.Type.ATTACK);
    Network.HistBlockEnd blockEnd = new Network.HistBlockEnd();
    Network.HistTagChange netPower1 = new Network.HistTagChange();
    netPower1.Tag = 36;
    netPower1.Value = 1;
    netPower1.Entity = actor4.GetEntity().GetEntityId();
    Network.HistTagChange netPower2 = new Network.HistTagChange();
    netPower2.Tag = 38;
    netPower2.Value = 1;
    netPower2.Entity = actor3.GetEntity().GetEntityId();
    actor3.GetEntity().SetTag(GAME_TAG.BATTLEGROUNDS_FAVORITE_FINISHER, result);
    int tagValue = flag ? 999 : 1;
    actor3.GetEntity().SetTag(GAME_TAG.ATK, tagValue);
    actor4.GetEntity().SetTag(GAME_TAG.DAMAGE, tagValue);
    taskList.SetBlockStart(blockStart);
    taskList.SetBlockEnd(blockEnd);
    taskList.CreateTask((Network.PowerHistory) netPower1);
    taskList.CreateTask((Network.PowerHistory) netPower2);
    GameState.Get().GetPowerProcessor().PerformTaskListOnCurrentGameState(taskList);
    return true;
  }

  private bool OnProcessCheat_settag(string func, string[] args, string rawArgs)
  {
    if (args.Length != 3)
      return false;
    int tagID = int.Parse(args[0]);
    if (tagID <= 0)
      return false;
    int tagValue = int.Parse(args[2]);
    if (tagValue < 0)
      return false;
    int result = 0;
    if (!int.TryParse(args[1], out result))
    {
      string entityIdentifier = args[1];
      Network.Get().SetTag(tagID, entityIdentifier, tagValue);
      return true;
    }
    Network.Get().SetTag(tagID, result, tagValue);
    return true;
  }

  private bool OnProcessCheat_debugscript(string func, string[] args, string rawArgs)
  {
    ScriptDebugDisplay.Get().ToggleDebugDisplay(true);
    if (args.Length != 1)
      return false;
    string powerGUID = args[0];
    Network.Get().DebugScript(powerGUID);
    return true;
  }

  private bool OnProcessCheat_disablescriptdebug(string func, string[] args, string rawArgs)
  {
    ScriptDebugDisplay.Get().ToggleDebugDisplay(false);
    Network.Get().DisableScriptDebug();
    return true;
  }

  private bool OnProcessCheat_printpersistentlist(string func, string[] args, string rawArgs)
  {
    if (args.Length == 0 || args[0] == "")
    {
      Network.Get().PrintPersistentList(0);
      return true;
    }
    for (int index = 0; index < args.Length; ++index)
    {
      int entityID = int.Parse(args[index]);
      Network.Get().PrintPersistentList(entityID);
    }
    return true;
  }

  private bool OnProcessCheat_togglecardtext(string func, string[] args, string rawArgs)
  {
    this.m_cardTextEnabled = !this.m_cardTextEnabled;
    if (SceneMgr.Get().IsInGame())
    {
      foreach (Zone zone in ZoneMgr.Get().GetZones())
      {
        foreach (Card card in zone.GetCards())
        {
          if ((UnityEngine.Object) card.GetActor() != (UnityEngine.Object) null)
            card.GetActor().UpdatePowersText();
        }
      }
    }
    return true;
  }

  private bool OnProcessCheat_togglecardnames(string func, string[] args, string rawArgs)
  {
    this.m_cardNamesEnabled = !this.m_cardNamesEnabled;
    if (SceneMgr.Get().IsInGame())
    {
      foreach (Zone zone in ZoneMgr.Get().GetZones())
      {
        foreach (Card card in zone.GetCards())
        {
          if ((UnityEngine.Object) card.GetActor() != (UnityEngine.Object) null)
            card.GetActor().UpdateNameText();
        }
      }
    }
    return true;
  }

  private bool OnProcessCheat_toggleracetext(string func, string[] args, string rawArgs)
  {
    this.m_cardRaceTextEnabled = !this.m_cardRaceTextEnabled;
    if (SceneMgr.Get().IsInGame())
    {
      foreach (Zone zone in ZoneMgr.Get().GetZones())
      {
        foreach (Card card in zone.GetCards())
        {
          if ((UnityEngine.Object) card.GetActor() != (UnityEngine.Object) null && card.GetEntity() != null)
            card.GetActor().UpdateTextComponents(card.GetEntity());
        }
      }
    }
    return true;
  }

  private bool OnProcessCheat_removeplayernames(string func, string[] args, string rawArgs)
  {
    this.m_playerNamesEnabled = false;
    if (SceneMgr.Get().IsInGame())
      Gameplay.Get().RemoveNameBanners();
    return true;
  }

  private bool OnProcessCheat_help(string func, string[] args, string rawArgs)
  {
    StringBuilder stringBuilder = new StringBuilder();
    string key = (string) null;
    if (args.Length != 0 && !string.IsNullOrEmpty(args[0]))
      key = args[0];
    List<string> stringList = new List<string>();
    if (key != null)
    {
      foreach (string cheatCommand in CheatMgr.Get().GetCheatCommands())
      {
        if (cheatCommand.Contains(key))
          stringList.Add(cheatCommand);
      }
    }
    else
    {
      foreach (string cheatCommand in CheatMgr.Get().GetCheatCommands())
        stringList.Add(cheatCommand);
    }
    Debug.Log((object) ("found commands " + (object) stringList + " " + (object) stringList.Count));
    if (stringList.Count == 1)
      key = stringList[0];
    if (key == null || stringList.Count != 1)
    {
      if (key == null)
        stringBuilder.Append("All available cheat commands:\n");
      else
        stringBuilder.Append("Cheat commands containing: \"" + key + "\"\n");
      int num = 0;
      string str1 = "";
      foreach (string str2 in stringList)
      {
        str1 = str1 + str2 + ", ";
        ++num;
        if (num > 4)
        {
          num = 0;
          stringBuilder.Append(str1);
          str1 = "";
        }
      }
      if (!string.IsNullOrEmpty(str1))
        stringBuilder.Append(str1);
      UIStatus.Get().AddInfo(stringBuilder.ToString(), 10f);
    }
    else
    {
      string str3 = "";
      CheatMgr.Get().cheatDesc.TryGetValue(key, out str3);
      string str4 = "";
      CheatMgr.Get().cheatArgs.TryGetValue(key, out str4);
      stringBuilder.Append("Usage: ");
      stringBuilder.Append(key);
      if (!string.IsNullOrEmpty(str4))
        stringBuilder.Append(" " + str4);
      if (!string.IsNullOrEmpty(str3))
        stringBuilder.Append("\n(" + str3 + ")");
      UIStatus.Get().AddInfo(stringBuilder.ToString(), 10f);
    }
    return true;
  }

  private bool OnProcessCheat_fixedrewardcomplete(string func, string[] args, string rawArgs)
  {
    int val;
    return args.Length >= 1 && !string.IsNullOrEmpty(args[0]) && GeneralUtils.TryParseInt(args[0], out val) && FixedRewardsMgr.Get().Cheat_ShowFixedReward(val, new FixedRewardsMgr.DelPositionNonToastReward(this.PositionLoginFixedReward));
  }

  private void PositionLoginFixedReward(Reward reward)
  {
    PegasusScene scene = SceneMgr.Get().GetScene();
    reward.transform.parent = scene.transform;
    reward.transform.localRotation = Quaternion.identity;
    reward.transform.localPosition = PopupDisplayManager.Get().RewardPopups.GetRewardLocalPos();
  }

  private bool OnProcessCheat_example(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1 || string.IsNullOrEmpty(args[0]))
      return false;
    string key = args[0];
    string str = "";
    if (!CheatMgr.Get().cheatExamples.TryGetValue(key, out str))
      return false;
    CheatMgr.Get().ProcessCheat(key + " " + str);
    return true;
  }

  private bool OnProcessCheat_tavernbrawl(string func, string[] args, string rawArgs)
  {
    string message1 = "USAGE: tb [cmd] [args]\nCommands: view, get, set, refresh, scenario, reset";
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      UIStatus.Get().AddInfo(message1, 10f);
      return true;
    }
    string str1 = args[0];
    string[] array = ((IEnumerable<string>) args).Skip<string>(1).ToArray<string>();
    string message2 = (string) null;
    switch (str1)
    {
      case "do_rewards":
        int result1 = 0;
        int.TryParse(array[0], out result1);
        TavernBrawlMode mode = TavernBrawlMode.TB_MODE_NORMAL;
        if (array.Length > 1)
          mode = array[1].Equals("heroic") ? TavernBrawlMode.TB_MODE_HEROIC : TavernBrawlMode.TB_MODE_NORMAL;
        TavernBrawlManager.Get().Cheat_DoHeroicRewards(result1, mode);
        message2 = "Doing reward animation and ending fake session if one exists.";
        break;
      case "fake_active_session":
        int result2 = 0;
        int.TryParse(args[1], out result2);
        TavernBrawlManager.Get().Cheat_SetActiveSession(result2);
        message2 = "Fake Tavern Brawl Session set.";
        break;
      case "get":
      case "set":
        bool flag = str1 == "set";
        string str2 = ((IEnumerable<string>) array).FirstOrDefault<string>();
        if (string.IsNullOrEmpty(str2))
        {
          message2 = string.Format("Please specify a TB variable to {0}. Variables:RefreshTime", (object) str1);
          break;
        }
        string str3 = (string) null;
        string lower = str2.ToLower();
        if (!(lower == "refreshtime"))
        {
          if (!(lower == "wins"))
          {
            if (lower == "losses")
            {
              int result3 = 0;
              int.TryParse(args[2], out result3);
              TavernBrawlManager.Get().Cheat_SetLosses(result3);
              message2 = string.Format("tb set losses {0} successful", (object) result3);
            }
          }
          else
          {
            int result4 = 0;
            int.TryParse(args[2], out result4);
            TavernBrawlManager.Get().Cheat_SetWins(result4);
            message2 = string.Format("tb set wins {0} successful", (object) result4);
          }
        }
        else if (flag)
          message2 = "cannot set RefreshTime";
        else
          str3 = TavernBrawlManager.Get().CurrentScheduledSecondsToRefresh.ToString() + " secs";
        if (flag)
        {
          message2 = string.Format("tb set {0} {1} successful.", (object) str2, array.Length >= 2 ? (object) array[1] : (object) "null");
          break;
        }
        if (string.IsNullOrEmpty(message2))
        {
          message2 = string.Format("tb variable {0}: {1}", (object) str2, (object) (str3 ?? "null"));
          break;
        }
        break;
      case "help":
        message2 = "usage";
        break;
      case "refresh":
        for (BrawlType brawlType = BrawlType.BRAWL_TYPE_TAVERN_BRAWL; brawlType < BrawlType.BRAWL_TYPE_COUNT; ++brawlType)
          TavernBrawlManager.Get().RefreshServerData(brawlType);
        message2 = "TB refreshing";
        break;
      case "reset":
        if (array.Length == 0)
        {
          message2 = "Please specify what to reset: seen, toserver";
          break;
        }
        if ("toserver".Equals(array[0], StringComparison.InvariantCultureIgnoreCase))
        {
          if (TavernBrawlManager.Get().IsCheated)
          {
            TavernBrawlManager.Get().Cheat_ResetToServerData();
            TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
            message2 = tavernBrawlMission != null ? "TB settings reset to server-specified Scenario ID " + (object) tavernBrawlMission.missionId : "TB settings reset to server-specified Scenario ID <null>";
            break;
          }
          message2 = "TB not locally cheated. Already using server-specified data.";
          break;
        }
        if ("seen".Equals(array[0], StringComparison.InvariantCultureIgnoreCase))
        {
          int result5 = 0;
          if (array.Length > 1 && !int.TryParse(array[1], out result5))
            message2 = "Error parsing new seen value: " + array[1];
          if (message2 == null)
          {
            TavernBrawlManager.Get().Cheat_ResetSeenStuff(result5);
            message2 = "all \"seentb*\" client-options reset to " + (object) result5;
            break;
          }
          break;
        }
        message2 = "Unknown reset parameter: " + array[0];
        break;
      case "scen":
      case "scenario":
        if (array.Length < 1)
        {
          message2 = "tb scenario: requires an ID parameter";
          break;
        }
        BrawlType brawlType1 = BrawlType.BRAWL_TYPE_TAVERN_BRAWL;
        if (array.Length > 1)
        {
          int result6 = -1;
          if (int.TryParse(array[1], out result6) && result6 >= 1 && result6 < 3)
            brawlType1 = (BrawlType) result6;
        }
        int result7;
        if (!int.TryParse(array[0], out result7))
          message2 = "tb scenario: invalid non-integer Scenario ID " + array[0];
        if (message2 == null)
        {
          TavernBrawlManager.Get().Cheat_SetScenario(result7, brawlType1);
          message2 = "tb scenario: set on client to ID: " + (object) result7 + " for type: " + (object) brawlType1;
          break;
        }
        break;
      case "view":
        TavernBrawlMission tavernBrawlMission1 = TavernBrawlManager.Get().CurrentMission();
        if (tavernBrawlMission1 == null)
        {
          message2 = "No active Tavern Brawl at this time.";
          break;
        }
        string str4 = "";
        string str5 = "";
        ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(tavernBrawlMission1.missionId);
        if (record != null)
        {
          str4 = (string) record.Name;
          str5 = (string) record.Description;
        }
        message2 = string.Format("Active TB: [{0}] {1}\n{2}", (object) tavernBrawlMission1.missionId, (object) str4, (object) str5);
        break;
    }
    if (message2 != null)
      UIStatus.Get().AddInfo(message2, 5f);
    return true;
  }

  private bool OnProcessCheat_duels(string func, string[] args, string rawArgs)
  {
    string message1 = "USAGE: duels [cmd] [args]\nCommands: nexttreasures nextloot";
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      UIStatus.Get().AddInfo(message1, 10f);
      return true;
    }
    string str1 = args[0];
    string[] array = ((IEnumerable<string>) args).Skip<string>(1).ToArray<string>();
    string message2 = (string) null;
    if (!(str1 == "help"))
    {
      if (!(str1 == "nexttreasures"))
      {
        if (str1 == "nextloot")
        {
          if (array.Length < 1)
          {
            message2 = "duels nextloot: requires at least 1 id to add";
          }
          else
          {
            int num = 0;
            foreach (string str2 in array)
            {
              int result = 0;
              if (!int.TryParse(str2, out result))
              {
                result = GameUtils.TranslateCardIdToDbId(str2);
                if (result == 0)
                {
                  message2 = "invalid card id: " + str2;
                  break;
                }
              }
              this.m_pvpdrLootIds.Enqueue(result);
              ++num;
            }
            if (message2 == null)
              message2 = "Added " + (object) num + " cards to next loot list";
          }
        }
      }
      else if (array.Length < 1)
      {
        message2 = "duels nexttreasures: requires 1-3 card ids to add";
      }
      else
      {
        int num = 0;
        foreach (string str3 in array)
        {
          int result = 0;
          if (!int.TryParse(str3, out result))
          {
            result = GameUtils.TranslateCardIdToDbId(str3);
            if (result == 0)
            {
              message2 = "invalid card id: " + str3;
              break;
            }
          }
          this.m_pvpdrTreasureIds.Enqueue(result);
          ++num;
        }
        if (message2 == null)
          message2 = "Added " + (object) num + " cards to next treasures list";
      }
    }
    else
      message2 = message1;
    if (message2 != null)
      UIStatus.Get().AddInfo(message2, 5f);
    return true;
  }

  private bool OnProcessCheat_randomizemercenariesboard(string func, string[] args, string rawArgs)
  {
    bool isFinalBoss = false;
    if (args.Length != 0 && !string.IsNullOrEmpty(args[0]))
      isFinalBoss = Convert.ToBoolean(args[0]);
    int seed = 0;
    if (args.Length > 1 && !string.IsNullOrEmpty(args[0]))
      seed = Convert.ToInt32(args[1]);
    if (Board.Get() is MercenariesBoard mercenariesBoard)
      mercenariesBoard.RandomizeVisuals(isFinalBoss, true, seed);
    return true;
  }

  private bool OnProcessCheat_mercs(string func, string[] args, string rawArgs)
  {
    this.m_lastMercsServerCmd = args;
    MercenariesDebugCommandRequest request = new MercenariesDebugCommandRequest();
    request.Args.AddRange((IEnumerable<string>) ((IEnumerable<string>) args).ToArray<string>());
    Network.Get().SendMercenariesDebugCommandRequest(request);
    return true;
  }

  private void OnProcessCheat_mercs_OnResponse()
  {
    MercenariesDebugCommandResponse debugCommandResponse = Network.Get().MercenariesDebugCommandResponse();
    bool flag = false;
    string str1 = "null response";
    if (this.m_lastMercsServerCmd != null && this.m_lastMercsServerCmd.Length != 0)
    {
      string str2 = this.m_lastMercsServerCmd[0];
    }
    string[] strArray = this.m_lastMercsServerCmd == null ? new string[0] : ((IEnumerable<string>) this.m_lastMercsServerCmd).Skip<string>(1).ToArray<string>();
    if (strArray.Length != 0)
    {
      string str3 = strArray[0];
    }
    if (strArray.Length >= 2)
      strArray[1].ToLower();
    this.m_lastMercsServerCmd = (string[]) null;
    if (debugCommandResponse != null)
    {
      flag = debugCommandResponse.Success;
      str1 = string.Format("{0} {1}", debugCommandResponse.Success ? (object) "" : (object) "FAILED:", debugCommandResponse.HasMessage ? (object) debugCommandResponse.Message : (object) "reply=<blank>");
    }
    Log.Net.Print(flag ? Blizzard.T5.Logging.LogLevel.Info : Blizzard.T5.Logging.LogLevel.Error, str1);
    if (flag)
    {
      float delay = 5f;
      UIStatus.Get().AddInfo(str1, delay);
    }
    else
      UIStatus.Get().AddError(str1);
  }

  private bool OnProcessCheat_fsg(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string str1 = "checkin, checkout, fake_gatherings, no_fake_gatherings, fake_large_scale, nearby_notice, sign, view, gps_offset, gps_set, gps_reset, find, finalize, vars, player, refreshpatrons, returntooltip";
    string[] values = str1.Split(new char[2]{ ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
    string message1 = "USAGE: fsg [cmd] [args]\nCommands: " + str1;
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      if (autofillData != null)
        return Cheats.ProcessAutofillParam((IEnumerable<string>) values, string.Empty, autofillData);
      UIStatus.Get().AddInfo(message1, 10f * UnityEngine.Time.timeScale);
      return true;
    }
    if (autofillData != null)
      return args.Length == 1 && Cheats.ProcessAutofillParam((IEnumerable<string>) values, args[0], autofillData);
    float delay = 5f * UnityEngine.Time.timeScale;
    string str2 = args[0];
    string message2 = (string) null;
    switch (str2)
    {
      case "checkin":
        FiresideGatheringManager.Get().Cheat_CheckInToFakeFSG();
        message2 = "Checked in to fake FSG";
        break;
      case "checkout":
        FiresideGatheringManager.Get().CheckOutOfFSG();
        message2 = "Checked out from FSG";
        break;
      case "debug_sign":
        TavernSignData lastSign = FiresideGatheringManager.Get().LastSign;
        if (lastSign == null)
        {
          message2 = "No Sign has been shown";
          break;
        }
        message2 = string.Format("Last FSG Sign:\nSign: {0}\nBackground: {1}\nMajor: {2}\nMinor: {3}", (object) lastSign.Sign, (object) lastSign.Background, (object) lastSign.Major, (object) lastSign.Minor);
        break;
      case "fake_gatherings":
        int result1 = 2;
        bool boolVal = false;
        if (args.Length > 1)
        {
          int.TryParse(args[1], out result1);
          if (result1 < 1)
            result1 = 1;
        }
        if (args.Length > 2)
          GeneralUtils.TryParseBool(args[2], out boolVal);
        FiresideGatheringManager.Get().Cheat_CreateFakeGatherings(result1, boolVal);
        message2 = string.Format("Created {0} fake gatherings", (object) result1);
        break;
      case "fake_large_scale":
        if (!FiresideGatheringManager.Get().IsCheckedIn)
        {
          message2 = "Check into an FSG first, to toggle Large Scale FSG.";
          break;
        }
        FiresideGatheringManager.Get().Cheat_ToggleLargeScaleFSG();
        message2 = "Large Scale FSG toggled to " + FiresideGatheringManager.Get().CurrentFSG.IsLargeScaleFsg.ToString();
        break;
      case "finalize":
        if (!FiresideGatheringManager.Get().HasFSGToInnkeeperSetup)
        {
          UIStatus.Get().AddError("There is no FSG to call InnkeeperSetup on - make sure there is an FSG you've created on the website with this Battle.net account.", delay);
          return true;
        }
        FiresideGatheringManager.Get().InnkeeperSetupFSG(true);
        message2 = "InnkeeperSetupFSG sent to server for FSG ID: " + (object) FiresideGatheringManager.Get().FSGToInnkeeperSetup.FsgId;
        break;
      case "find":
        if (!FiresideGatheringManager.CanRequestNearbyFSG)
        {
          UIStatus.Get().AddError("Cannot make request for NearbyFSGs either because FSG is disabled or the location features are disabled for this player's country.", delay);
          return true;
        }
        if (args.Length > 1)
        {
          bool flag = false;
          double latitude = 0.0;
          double longitude = 0.0;
          if ("irvine".Equals(args[1], StringComparison.InvariantCultureIgnoreCase))
          {
            flag = true;
            latitude = 33.6578341;
            longitude = -117.7674501;
          }
          if (flag)
            FiresideGatheringManager.Get().Cheat_GPSSet(latitude, longitude);
        }
        FiresideGatheringManager.Get().PlayerAccountShouldAutoCheckin.Set(true);
        FiresideGatheringManager.Get().RequestNearbyFSGs();
        message2 = "RequestNearbyFSGs sent to server.";
        break;
      case "gps_offset":
        double result2 = 0.0;
        if (args.Length > 1)
          double.TryParse(args[1], out result2);
        FiresideGatheringManager.Get().Cheat_GPSOffset(result2);
        message2 = "Set GPS Offset to: " + (object) result2;
        break;
      case "gps_reset":
        FiresideGatheringManager.Get().Cheat_ResetGPSCheating();
        message2 = "GPS cheats have been reset.";
        break;
      case "gps_set":
        double result3 = 0.0;
        double result4 = 0.0;
        if (args.Length > 1)
          double.TryParse(args[1], out result3);
        if (args.Length > 2)
          double.TryParse(args[2], out result4);
        FiresideGatheringManager.Get().Cheat_GPSSet(result3, result4);
        message2 = string.Format("Set GPS Set to: [{0}, {1}]", (object) result3, (object) result4);
        if (args.Length >= 4 && "find".Equals(args[3], StringComparison.InvariantCultureIgnoreCase))
        {
          FiresideGatheringManager.Get().PlayerAccountShouldAutoCheckin.Set(true);
          FiresideGatheringManager.Get().RequestNearbyFSGs();
          break;
        }
        break;
      case "help":
        message2 = message1;
        break;
      case "nearby_notice":
        FiresideGatheringManager.Get().Cheat_NearbyFSGNotice();
        message2 = "Simulating nearby FSGs when checked out";
        break;
      case "no_fake_gatherings":
        FiresideGatheringManager.Get().Cheat_RemoveFakeGatherings();
        message2 = "Removed fake gatherings";
        break;
      case "player":
        StringBuilder builder1 = new StringBuilder();
        int lines1 = 0;
        System.Action<string, object> action1 = (System.Action<string, object>) ((displayName, value) =>
        {
          if (lines1 != 0)
            builder1.Append("\n");
          builder1.AppendFormat("{0}={1}", (object) displayName, value);
          ++lines1;
        });
        action1(Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(Option.SHOULD_AUTO_CHECK_IN_TO_FIRESIDE_GATHERINGS), (object) FiresideGatheringManager.Get().PlayerAccountShouldAutoCheckin);
        action1(Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(Option.HAS_INITIATED_FIRESIDE_GATHERING_SCAN), (object) FiresideGatheringManager.Get().HasManuallyInitiatedFSGScanBefore);
        action1(Blizzard.T5.Core.Utils.EnumUtils.GetString<Option>(Option.LAST_TAVERN_JOINED), (object) FiresideGatheringManager.Get().LastTavernID);
        string format1 = builder1.ToString();
        Log.All.Print(format1);
        message2 = format1.Replace("\n", ", ") + "\n";
        delay = Mathf.Min(30f, 5f * (float) lines1) * UnityEngine.Time.timeScale;
        break;
      case "refreshpatrons":
        Network.Get().RequestFSGPatronListUpdate();
        break;
      case "returntooltip":
        FiresideGatheringManager.Get().HasSeenReturnToFSGSceneTooltip = false;
        FiresideGatheringManager.Get().ShowReturnToFSGSceneTooltip();
        break;
      case "sign":
        int result5 = UnityEngine.Random.Range(1, 8);
        int result6 = UnityEngine.Random.Range(1, 15);
        int result7 = UnityEngine.Random.Range(1, 85);
        int result8 = UnityEngine.Random.Range(1, 43);
        TavernSignType outVal = TavernSignType.TAVERN_SIGN_TYPE_CUSTOM;
        if (args.Length > 1)
        {
          if (!Blizzard.T5.Core.Utils.EnumUtils.TryGetEnum<TavernSignType>(("TAVERN_SIGN_TYPE_" + args[1]).ToLower(), out outVal))
            outVal = TavernSignType.TAVERN_SIGN_TYPE_CUSTOM;
          int.TryParse(args[1], out result5);
          if (result5 < 1)
            result5 = 1;
        }
        if (args.Length > 2)
        {
          int.TryParse(args[2], out result6);
          if (result6 < 1)
            result6 = 1;
        }
        if (args.Length > 3)
        {
          int.TryParse(args[3], out result7);
          if (result7 < 1)
            result7 = 1;
        }
        if (args.Length > 4)
        {
          int.TryParse(args[4], out result8);
          if (result8 < 1)
            result8 = 1;
        }
        string tavernName;
        if (args.Length > 5)
          tavernName = string.Join(" ", args.Slice<string>(5));
        else
          tavernName = string.Format("fsg sign {0} {1} {2} {3}", (object) result5, (object) result6, (object) result7, (object) result8);
        FiresideGatheringManager.Get().Cheat_ShowSign(outVal, result5, result6, result7, result8, tavernName);
        break;
      case "vars":
        StringBuilder builder2 = new StringBuilder();
        int lines2 = 0;
        System.Action<string, object> action2 = (System.Action<string, object>) ((displayName, value) =>
        {
          if (lines2 != 0)
            builder2.Append("\n");
          int num = displayName.LastIndexOf('.');
          if (num >= 0 && num < displayName.Length - 1)
            displayName = displayName.Substring(num + 1);
          builder2.AppendFormat("{0}={1}", (object) displayName, value);
          ++lines2;
        });
        string[] strArray = new string[3]
        {
          "Location.Latitude",
          "Location.Longitude",
          "Location.BSSID"
        };
        foreach (string key in strArray)
        {
          VarKey varKey = Vars.Key(key);
          if (varKey.HasValue)
            action2(key, (object) varKey.GetStr("null"));
        }
        NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
        action2("FSGEnabled", (object) netObject.FSGEnabled);
        action2("AutoCheckin", (object) FiresideGatheringManager.Get().AutoCheckInEnabled);
        action2("LoginScan", (object) netObject.FSGLoginScanEnabled);
        action2("MaxPubscribedPatrons", (object) netObject.FsgMaxPresencePubscribedPatronCount);
        action2("PatronCountLimit", (object) FiresideGatheringManager.Get().FriendListPatronCountLimit);
        action2("FSG.PeriodicPrunePatronOldSubscriptionsSeconds", (object) (FiresideGatheringPresenceManager.PERIODIC_SUBSCRIBE_CHECK_SECONDS.ToString() + "s"));
        action2("FSG.PatronOldSubscriptionThresholdSeconds", (object) (FiresideGatheringPresenceManager.PATRON_OLD_SUBSCRIPTION_THRESHOLD_SECONDS.ToString() + "s"));
        action2("FSG.PresenceSubscriptionsVerboseLog", (object) FiresideGatheringPresenceManager.IsVerboseLogging);
        action2("ToScreen", (object) FiresideGatheringPresenceManager.IsVerboseLoggingToScreen);
        string format2 = builder2.ToString();
        Log.All.Print(format2);
        message2 = format2.Replace("\n", ", ") + "\n";
        delay = Mathf.Min(30f, 5f * (float) lines2) * UnityEngine.Time.timeScale;
        break;
      case "view":
        FSGConfig currentFsg = FiresideGatheringManager.Get().CurrentFSG;
        string str3;
        DateTime dateTime1;
        if (currentFsg == null)
        {
          str3 = "No FSG currently checked in to.";
        }
        else
        {
          object[] objArray = new object[4]
          {
            (object) currentFsg.FsgId,
            (object) currentFsg.TavernName,
            null,
            null
          };
          DateTime dateTime2 = TimeUtils.UnixTimeStampToDateTimeUtc(currentFsg.UnixStartTimeWithSlush);
          dateTime2 = dateTime2.ToLocalTime();
          objArray[2] = (object) dateTime2.ToString("R");
          dateTime1 = TimeUtils.UnixTimeStampToDateTimeUtc(currentFsg.UnixEndTimeWithSlush).ToLocalTime();
          objArray[3] = (object) dateTime1.ToString("R");
          str3 = string.Format("Checked into FSG: [{0}] {1}\nStart w/ Slush: {2}\nEnd w/ Slush: {3}", objArray);
        }
        string str4 = str3 + "\n";
        string str5 = "No Data";
        ClientLocationData bestLocationData = ClientLocationManager.Get().GetBestLocationData();
        if (bestLocationData != null)
          str5 = bestLocationData.ToString();
        bool isCheatingGPS;
        double latitude1;
        double longitude1;
        double offset;
        FiresideGatheringManager.Get().Cheat_GetGPSCheats(out isCheatingGPS, out latitude1, out longitude1, out offset);
        if (isCheatingGPS || offset != 0.0)
        {
          if (isCheatingGPS)
            str5 += string.Format("GPS overridden w/ [{0}, {1}]", (object) latitude1, (object) longitude1);
          if (offset != 0.0)
            str5 += string.Format(" offset={0}", (object) offset);
        }
        message2 = str4 + string.Format("FSG: {0} GPS: {1} WIFI: {2}\nClient Location Data:\n{3}", FiresideGatheringManager.IsFSGFeatureEnabled ? (object) "enabled" : (object) "disabled", FiresideGatheringManager.IsGpsFeatureEnabled ? (object) "enabled" : (object) "disabled", FiresideGatheringManager.IsWifiFeatureEnabled ? (object) "enabled" : (object) "disabled", (object) str5);
        if (FiresideGatheringManager.Get().HasFSGToInnkeeperSetup)
        {
          FSGConfig toInnkeeperSetup = FiresideGatheringManager.Get().FSGToInnkeeperSetup;
          if (!toInnkeeperSetup.IsSetupComplete)
          {
            string str6 = message2;
            object[] objArray = new object[4]
            {
              (object) toInnkeeperSetup.FsgId,
              (object) toInnkeeperSetup.TavernName,
              null,
              null
            };
            dateTime1 = TimeUtils.UnixTimeStampToDateTimeUtc(toInnkeeperSetup.UnixStartTimeWithSlush);
            dateTime1 = dateTime1.ToLocalTime();
            objArray[2] = (object) dateTime1.ToString("R");
            dateTime1 = TimeUtils.UnixTimeStampToDateTimeUtc(toInnkeeperSetup.UnixEndTimeWithSlush);
            dateTime1 = dateTime1.ToLocalTime();
            objArray[3] = (object) dateTime1.ToString("R");
            string str7 = string.Format("Innkeeper of FSG: [{0}] {1}\nStart w/ Slush: {2}\nEnd w/ Slush: {3}", objArray);
            message2 = str6 + str7;
          }
        }
        delay = 20f * UnityEngine.Time.timeScale;
        break;
    }
    if (message2 != null)
      UIStatus.Get().AddInfo(message2, delay);
    return true;
  }

  private bool OnProcessCheat_GPS(string func, string[] args, string rawArgs)
  {
    string message1 = "USAGE: gps [cmd] [args]\nCommands: on/off";
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      UIStatus.Get().AddInfo(message1, 10f);
      return true;
    }
    string str = args[0];
    string message2 = (string) null;
    if (!(str == "off") && !(str == "on"))
    {
      if (str == "view")
      {
        double num1 = 0.0;
        double num2 = 0.0;
        GpsCoordinate location = ClientLocationManager.Get().GetBestLocationData().location;
        if (location != null)
        {
          num1 = location.Latitude;
          num2 = location.Longitude;
        }
        message2 = string.Format("GPS Services: {0}\nLatitude: {1}\nLongitude: {2}", ClientLocationManager.Get().GPSServicesEnabled ? (object) "enabled" : (object) "disabled", (object) num1, (object) num2);
      }
    }
    else
    {
      ClientLocationManager.Get().Cheat_SetGPSEnabled(str == "on");
      message2 = "GPS turned " + str;
    }
    if (message2 != null)
      UIStatus.Get().AddInfo(message2, 5f);
    return true;
  }

  private bool OnProcessCheat_Wifi(string func, string[] args, string rawArgs)
  {
    string message1 = "USAGE: wifi [cmd] [args]\nCommands: on/off";
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      UIStatus.Get().AddInfo(message1, 10f);
      return true;
    }
    string str = args[0];
    string message2 = (string) null;
    if (!(str == "off") && !(str == "on"))
    {
      if (str == "view")
        message2 = string.Format("WIFI Services: {0}", ClientLocationManager.Get().WifiEnabled ? (object) "enabled" : (object) "disabled");
    }
    else
    {
      ClientLocationManager.Get().Cheat_SetWifiEnabled(str == "on");
      message2 = "WIFI turned " + str;
    }
    if (message2 != null)
      UIStatus.Get().AddInfo(message2, 5f);
    return true;
  }

  private bool OnProcessCheat_utilservercmd(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    string[] values = new string[22]
    {
      "help",
      "tb",
      "fsg",
      "arena",
      "ranked",
      "deck",
      "freedeck",
      "banner",
      "quest",
      "legacyachieve",
      "prog",
      "setgsd",
      "returningplayer",
      "curl",
      "coin",
      "bgheroskin",
      "bgguideskin",
      "bgboardskin",
      "bgfinisher",
      "bgemote",
      "reward",
      "playerflag"
    };
    if (args.Length < 1 || string.IsNullOrEmpty(rawArgs))
    {
      if (autofillData != null)
        return Cheats.ProcessAutofillParam((IEnumerable<string>) values, string.Empty, autofillData);
      UIStatus.Get().AddError("Must specify a sub-command.");
      return true;
    }
    string[] source = this.OnProcessCheat_utilservercmd_OverwriteArgsForAliasing(args);
    string cmd = source[0].ToLower();
    string[] cmdArgs = ((IEnumerable<string>) source).Skip<string>(1).ToArray<string>();
    string searchTerm1 = cmdArgs.Length == 0 ? (string) null : cmdArgs[0].ToLower();
    bool flag1 = cmd == "fsg" && searchTerm1 == "tb";
    if (autofillData != null && source.Length == 1 && !rawArgs.EndsWith(" "))
    {
      string searchTerm2 = cmd;
      return Cheats.ProcessAutofillParam((IEnumerable<string>) values, searchTerm2, autofillData);
    }
    bool requiresConfirm = true;
    switch (cmd)
    {
      case "arena":
        requiresConfirm = false;
        string searchTerm3 = cmdArgs.Length < 2 ? (string) null : cmdArgs[1].ToLower();
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) "view_player, reward, ticket, set, view, list, season, scenario, end_offset, start_offset, active, dormant, choices".Split(new char[2]
          {
            ' ',
            ','
          }, StringSplitOptions.RemoveEmptyEntries), searchTerm3, autofillData);
        }
        if (searchTerm3 == "reward" && !((IEnumerable<string>) cmdArgs).Any<string>((Func<string, bool>) (arg => "justids".Equals(arg))))
        {
          List<string> list = ((IEnumerable<string>) cmdArgs).ToList<string>();
          list.Add("justids");
          cmdArgs = list.ToArray();
          break;
        }
        break;
      case "banner":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) "list, reset".Split(new char[2]
          {
            ' ',
            ','
          }, StringSplitOptions.RemoveEmptyEntries), searchTerm1, autofillData);
        }
        if (string.IsNullOrEmpty(searchTerm1) || searchTerm1 == "help")
        {
          UIStatus.Get().AddInfo("Usage: util banner <list | reset bannerId=#>\n\nClear seen banners (wooden signs at login) with IDs >= bannerId arg. If no parameters, clears out just latest known bannerId. If bannerId=0, all seen banners are cleared.", 5f);
          return true;
        }
        if (searchTerm1 == "list")
        {
          this.Cheat_ShowBannerList();
          return true;
        }
        break;
      case "bgboardskin":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) new string[12]
          {
            "help",
            "view",
            "favorite",
            "grant",
            "clear",
            "remove",
            "grantall",
            "removeall",
            "setseen",
            "setallseen",
            "clearseen",
            "clearallseen"
          }, searchTerm1, autofillData);
        }
        break;
      case "bgemote":
        requiresConfirm = false;
        if (autofillData != null && ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) != 0 || cmdArgs.Length == 1))
          return Cheats.ProcessAutofillParam((IEnumerable<string>) new string[13]
          {
            "help",
            "view",
            "grant",
            "remove",
            "grantall",
            "removeall",
            "setseen",
            "clearseen",
            "setallseen",
            "clearallseen",
            "loadout",
            "setloadoutslot",
            "clearloadoutslot"
          }, searchTerm1, autofillData);
        break;
      case "bgfinisher":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) new string[12]
          {
            "help",
            "view",
            "favorite",
            "grant",
            "clear",
            "remove",
            "grantall",
            "removeall",
            "setseen",
            "clearseen",
            "setallseen",
            "clearallseen"
          }, searchTerm1, autofillData);
        }
        break;
      case "bgguideskin":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) new string[12]
          {
            "help",
            "view",
            "favorite",
            "grant",
            "clear",
            "remove",
            "grantall",
            "removeall",
            "setseen",
            "setallseen",
            "clearseen",
            "clearallseen"
          }, searchTerm1, autofillData);
        }
        break;
      case "bgheroskin":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) new string[13]
          {
            "help",
            "view",
            "favorite",
            "grant",
            "grantall",
            "clear",
            "remove",
            "grantall",
            "removeall",
            "setseen",
            "setallseen",
            "clearseen",
            "clearallseen"
          }, searchTerm1, autofillData);
        }
        break;
      case "coin":
        requiresConfirm = false;
        if (searchTerm1 == "quickfavorite")
        {
          string str = ((IEnumerable<string>) source).FirstOrDefault<string>((Func<string, bool>) (x => x.StartsWith("id=")));
          int newFavoriteCoinID = 1;
          if (str != null)
            newFavoriteCoinID = Convert.ToInt32(str.Substring("id=".Length));
          CoinManager.Get().RequestSetFavoriteCoin(newFavoriteCoinID);
          return true;
        }
        break;
      case "curl":
      case "getgsd":
      case "grant":
      case "setgsd":
        requiresConfirm = false;
        break;
      case "deck":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) "view, test, grant".Split(new char[2]
          {
            ' ',
            ','
          }, StringSplitOptions.RemoveEmptyEntries), searchTerm1, autofillData);
        }
        if (searchTerm1 == "view" && !((IEnumerable<string>) cmdArgs).Any<string>((Func<string, bool>) (arg => arg.StartsWith("details=", StringComparison.InvariantCultureIgnoreCase))))
        {
          cmdArgs = new List<string>((IEnumerable<string>) cmdArgs)
          {
            "details=0"
          }.ToArray();
          break;
        }
        break;
      case "freedeck":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) "start, end, claim, view, reset".Split(new char[2]
          {
            ' ',
            ','
          }, StringSplitOptions.RemoveEmptyEntries), searchTerm1, autofillData);
        }
        break;
      case "fsg":
      case "tb":
        int index = 1;
        if (flag1)
        {
          searchTerm1 = cmdArgs.Length < 2 ? (string) null : cmdArgs[1].ToLower();
          index = 2;
        }
        if (autofillData != null)
        {
          bool flag2 = rawArgs.EndsWith(" ") && cmdArgs.Length == (flag1 ? 1 : 0);
          if (cmd == "tb" | flag1 && (flag2 || cmdArgs.Length == (flag1 ? 2 : 1)))
            return Cheats.ProcessAutofillParam((IEnumerable<string>) "view, list, season, scenario, end_offset, start_offset, active, dormant, ticket, reset_ticket, reset, wins, losses, reward".Split(new char[2]
            {
              ' ',
              ','
            }, StringSplitOptions.RemoveEmptyEntries), searchTerm1, autofillData);
          if (!(cmd == "fsg") || !flag2 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) "config, setconfig, tb, find, finalize, checkin, checkout, patrons".Split(new char[2]
          {
            ' ',
            ','
          }, StringSplitOptions.RemoveEmptyEntries), searchTerm1, autofillData);
        }
        if (!(searchTerm1 == "help") && !(searchTerm1 == "view") && !(searchTerm1 == "list"))
        {
          if (searchTerm1 == "reset")
          {
            requiresConfirm = (cmdArgs.Length < index + 1 ? (string) null : cmdArgs[index].ToLower()) != "help";
            break;
          }
          break;
        }
        requiresConfirm = false;
        break;
      case "help":
        requiresConfirm = false;
        break;
      case "hero":
        requiresConfirm = false;
        break;
      case "legacyachieve":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) "cancel, resetdaily, resetreroll, grant, complete, progress".Split(new char[2]
          {
            ' ',
            ','
          }, StringSplitOptions.RemoveEmptyEntries), searchTerm1, autofillData);
        }
        this.OnProcessCheat_util_achieves_ReplaceSlotWithAchieve(cmdArgs);
        int achievementFromArgs = this.OnProcessCheat_util_achieves_GetAchievementFromArgs(cmdArgs);
        if (searchTerm1 == "grant")
        {
          Achievement achievement = AchieveManager.Get().GetAchievement(achievementFromArgs);
          if (achievement != null && AchieveManager.Get().GetActiveQuests().Count >= 3 && achievement.CanShowInQuestLog)
          {
            UIStatus.Get().AddInfo(string.Format("{0} {1}: Quest log is full.", (object) func, (object) cmd), 5f);
            return true;
          }
        }
        if (!(searchTerm1 == "cancel"))
        {
          if (!(searchTerm1 == "resetdaily") && !(searchTerm1 == "resetreroll"))
          {
            if (searchTerm1 == "grant" || searchTerm1 == "complete" || searchTerm1 == "progress")
            {
              this.OnProcessCheat_util_achieves_ShowQuestPopupsWhenAchieveUpdated(achievementFromArgs);
              break;
            }
            UIStatus.Get().AddInfo("USAGE: quest [subcmd] [subcmd args]\nCommands: grant, complete, progress, cancel, resetdaily\n Subcommands: achieve=[achieveId] (required for grant), slot=[slot#] (Either achieveId or slot required for complete, progress, cancel), amount=[X] (for progress only- optional), offset=[X] (in hours from current time, for resetdaily and resetreroll", 10f);
            return true;
          }
          break;
        }
        this.OnProcessCheat_util_achieves_ShowQuestLog();
        break;
      case "logrelay":
        if (string.IsNullOrEmpty(searchTerm1))
        {
          UIStatus.Get().AddInfo("USAGE: logrelay [logName]", 10f);
          return true;
        }
        requiresConfirm = false;
        break;
      case "playerflag":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) new string[2]
          {
            "check",
            "set"
          }, searchTerm1, autofillData);
        }
        break;
      case "prog":
        bool autoFillResult = false;
        if (this.ProcessAutofillParam_util_prog(rawArgs, cmdArgs, autofillData, ref autoFillResult, ref requiresConfirm))
          return autoFillResult;
        break;
      case "ranked":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) "view, season, set, reward, medal, win, lose, games, seasonroll".Split(new char[2]
          {
            ' ',
            ','
          }, StringSplitOptions.RemoveEmptyEntries), searchTerm1, autofillData);
        }
        if (searchTerm1 == "seasonroll")
        {
          requiresConfirm = true;
          break;
        }
        break;
      case "returningplayer":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) "start, test_group, optout, complete, reset".Split(new char[2]
          {
            ' ',
            ','
          }, StringSplitOptions.RemoveEmptyEntries), searchTerm1, autofillData);
        }
        break;
      case "reward":
        requiresConfirm = false;
        if (autofillData != null)
        {
          if ((!rawArgs.EndsWith(" ") ? 0 : (cmdArgs.Length == 0 ? 1 : 0)) == 0 && cmdArgs.Length != 1)
            return false;
          return Cheats.ProcessAutofillParam((IEnumerable<string>) new string[12]
          {
            "grantlist",
            "grantitem",
            "gold",
            "dust",
            "orbs",
            "booster",
            "card",
            "randomcard",
            "tavernticket",
            "cardback",
            "heroskin",
            "customcoin"
          }, searchTerm1, autofillData);
        }
        break;
    }
    if (autofillData != null)
      return false;
    AlertPopup.ResponseCallback responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
    {
      if (response != AlertPopup.Response.CONFIRM && response != AlertPopup.Response.OK)
        return;
      DebugCommandRequest packet = new DebugCommandRequest();
      packet.Command = cmd;
      packet.Args.AddRange((IEnumerable<string>) cmdArgs);
      Network.Get().SendDebugCommandRequest(packet);
    });
    this.m_lastUtilServerCmd = source;
    if (requiresConfirm)
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = "Run UTIL server command?",
        m_text = "You are about to run a UTIL Server command - this may affect other players on this environment and possibly change configuration on this environment.\n\nPlease confirm you want to do this.",
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = responseCallback
      });
    else
      responseCallback(AlertPopup.Response.OK, (object) null);
    return true;
  }

  private string[] OnProcessCheat_utilservercmd_OverwriteArgsForAliasing(string[] args)
  {
    string str = args[0];
    if (!(str == "quest") && !(str == "achieve"))
      return args;
    string[] strArray = new string[1 + args.Length];
    strArray[0] = "prog";
    args.CopyTo((Array) strArray, 1);
    return strArray;
  }

  private void OnProcessCheat_utilservercmd_OnResponse()
  {
    DebugCommandResponse debugCommandResponse = Network.Get().GetDebugCommandResponse();
    bool flag1 = false;
    string str1 = "null response";
    string str2 = this.m_lastUtilServerCmd == null || this.m_lastUtilServerCmd.Length == 0 ? "" : this.m_lastUtilServerCmd[0];
    string[] strArray1 = this.m_lastUtilServerCmd == null ? new string[0] : ((IEnumerable<string>) this.m_lastUtilServerCmd).Skip<string>(1).ToArray<string>();
    string str3 = strArray1.Length == 0 ? (string) null : strArray1[0];
    string str4 = strArray1.Length < 2 ? (string) null : strArray1[1].ToLower();
    this.m_lastUtilServerCmd = (string[]) null;
    if (debugCommandResponse != null)
    {
      flag1 = debugCommandResponse.Success;
      str1 = string.Format("{0} {1}", debugCommandResponse.Success ? (object) "" : (object) "FAILED:", debugCommandResponse.HasResponse ? (object) debugCommandResponse.Response : (object) "reply=<blank>");
    }
    Log.Net.Print(flag1 ? Blizzard.T5.Logging.LogLevel.Info : Blizzard.T5.Logging.LogLevel.Error, str1);
    bool flag2 = true;
    float delay = 5f;
    if (flag1)
    {
      bool flag3 = str2 == "fsg" && str3 == "tb";
      if (str2 == "tb" | flag3)
      {
        if (flag3)
        {
          str3 = str4;
          str4 = strArray1.Length < 3 ? (string) null : strArray1[2].ToLower();
        }
        if (str3 == "scenario" || str3 == "scen" || str3 == "season" || str3 == "end_offset" || str3 == "start_offset" || str3 == "wins" || str3 == "losses" || str3 == "ticket" || str3 == "reset" && str4 != "help")
        {
          for (BrawlType brawlType = BrawlType.BRAWL_TYPE_TAVERN_BRAWL; brawlType < BrawlType.BRAWL_TYPE_COUNT; ++brawlType)
            TavernBrawlManager.Get().RefreshServerData(brawlType);
        }
      }
      else if (str2 == "ranked")
      {
        if (str3 == "medal" || str3 == "seasonroll")
        {
          flag1 = flag1 && (!debugCommandResponse.HasResponse || !debugCommandResponse.Response.StartsWith("Error"));
          if (flag1)
          {
            str1 = "Success";
            delay = 0.5f;
          }
          else if (debugCommandResponse.HasResponse)
            str1 = debugCommandResponse.Response;
        }
        if (str3 == "set" || str3 == "win" || str3 == "lose" || str3 == "games")
          NetCache.Get().RefreshNetObject<NetCache.NetCacheMedalInfo>();
      }
      else if (str2 == "hero")
      {
        if (str3 == "addxp")
          NetCache.Get().RefreshNetObject<NetCache.NetCacheHeroLevels>();
      }
      else if (str2 == "banner")
      {
        if (str3 == "reset")
        {
          NetCache.Get().ReloadNetObject<NetCache.NetCacheProfileProgress>();
          bool flag4 = false;
          int result = 0;
          foreach (string str5 in strArray1)
          {
            string[] strArray2;
            if (str5 != null)
              strArray2 = str5.Split('=');
            else
              strArray2 = (string[]) null;
            string[] strArray3 = strArray2;
            if (strArray3 != null && strArray3.Length >= 2 && (strArray3[0].Equals("banner", StringComparison.InvariantCultureIgnoreCase) || strArray3[0].Equals("bannerId", StringComparison.InvariantCultureIgnoreCase)))
            {
              flag4 = true;
              int.TryParse(strArray3[1], out result);
            }
          }
          if (flag4)
            BannerManager.Get().Cheat_ClearSeenBannersNewerThan(result);
          else
            BannerManager.Get().Cheat_ClearSeenBanners();
        }
      }
      else if (str2 == "returningplayer")
      {
        flag1 = flag1 && (!debugCommandResponse.HasResponse || !debugCommandResponse.Response.StartsWith("Error"));
        if (flag1)
        {
          ReturningPlayerMgr.Get().Cheat_ResetReturningPlayer();
          if (true)
            str1 += "\nYou may want to log out/in to take effect.";
        }
      }
      else if (str2 == "logrelay")
      {
        if (str3 == "*")
          flag2 = false;
      }
      else if (str2 == "prog")
      {
        if ((str3 == "achieve" || str3 == "quest" || str3 == "task") && str4 == "listen")
        {
          if (strArray1.Length < 3)
            return;
          string lower = strArray1[2].ToLower();
          LuaLogs luaLogs = ServiceManager.Get<LuaLogs>();
          if (luaLogs == null)
            return;
          int valueOrDefault = (int) SceneDebugger.Get().GetPlayerId_DebugOnly().GetValueOrDefault();
          if (lower == "all")
          {
            luaLogs.ClearListenOnGameServer(valueOrDefault);
            return;
          }
          int result = 0;
          foreach (string str6 in strArray1)
          {
            string[] strArray4;
            if (str6 == null)
            {
              strArray4 = (string[]) null;
            }
            else
            {
              char[] chArray = new char[1]{ '=' };
              strArray4 = str6.Split(chArray);
            }
            string[] strArray5 = strArray4;
            if (strArray5 != null && strArray5.Length >= 2 && strArray5[0].Equals("id", StringComparison.InvariantCultureIgnoreCase))
              int.TryParse(strArray5[1], out result);
          }
          try
          {
            LuaLogs.ListenableScriptType scriptType = Blizzard.T5.Core.Utils.EnumUtils.GetEnum<LuaLogs.ListenableScriptType>(str3.ToUpper());
            luaLogs.ListenOnGameServer(valueOrDefault, result, scriptType);
          }
          catch (ArgumentException ex)
          {
            Error.AddWarning("Prog listen Cheat Error", string.Format("Type is not configured to be listenable {0}. Error Message: {1}", (object) str3.ToUpper(), (object) ex));
          }
        }
      }
      else if (str2 == "bgemote")
        NetCache.Get().RefreshNetObject<NetCache.NetCacheBattlegroundsEmotes>();
      else if ((str2 == "bgheroskin" || str2 == "bgguideskin" || str2 == "bgboardskin" || str2 == "bgfinisher") && str3 != null && str3.Contains("seen"))
      {
        if (str2 == "bgheroskin")
          NetCache.Get().RefreshNetObject<NetCache.NetCacheBattlegroundsHeroSkins>();
        else if (str2 == "bgguideskin")
          NetCache.Get().RefreshNetObject<NetCache.NetCacheBattlegroundsGuideSkins>();
        else if (str2 == "bgboardskin")
          NetCache.Get().RefreshNetObject<NetCache.NetCacheBattlegroundsBoardSkins>();
        else if (str2 == "bgfinisher")
          NetCache.Get().RefreshNetObject<NetCache.NetCacheBattlegroundsFinishers>();
      }
      if ((str2 == "ranked" || str2 == "arena") && str3 == "reward")
      {
        flag1 = flag1 && (!debugCommandResponse.HasResponse || !debugCommandResponse.Response.StartsWith("Error"));
        if (flag1)
        {
          str1 = Cheats.Cheat_ShowRewardBoxes(str1);
          if (str1 == null)
          {
            delay = 0.5f;
            str1 = "Success";
          }
          else
            flag1 = false;
        }
      }
      if (str2 == "arena" && str3 == "season")
        DraftManager.Get().RefreshCurrentSeasonFromServer();
    }
    if (!flag2)
      return;
    if (flag1)
      UIStatus.Get().AddInfo(str1, delay);
    else
      UIStatus.Get().AddError(str1);
  }

  private int OnProcessCheat_util_achieves_GetAchievementFromArgs(string[] args)
  {
    string str = ((IEnumerable<string>) args).FirstOrDefault<string>((Func<string, bool>) (x => x.StartsWith("achieve=")));
    return str != null ? Convert.ToInt32(str.Substring("achieve=".Length)) : 0;
  }

  private int OnProcessCheat_util_achieves_GetAchieveFromSlotId(int slotId)
  {
    List<Achievement> activeQuests = AchieveManager.Get().GetActiveQuests();
    return slotId > 0 && slotId <= activeQuests.Count ? activeQuests[slotId - 1].ID : 0;
  }

  private void OnProcessCheat_util_achieves_ReplaceSlotWithAchieve(string[] args)
  {
    for (int index = 0; index < args.Length; ++index)
    {
      if (args[index].StartsWith("slot=", true, CultureInfo.CurrentCulture))
      {
        int achieveFromSlotId = this.OnProcessCheat_util_achieves_GetAchieveFromSlotId(Convert.ToInt32(args[index].Substring("slot=".Length)));
        args[index] = string.Format("achieve={0}", (object) achieveFromSlotId);
      }
    }
  }

  private void OnProcessCheat_util_achieves_ShowQuestPopupsWhenAchieveUpdated(int achieveId)
  {
    AchieveManager.AchievesUpdatedCallback action = (AchieveManager.AchievesUpdatedCallback) null;
    AchieveManager.Get().RegisterAchievesUpdatedListener(action = (AchieveManager.AchievesUpdatedCallback) ((updatedAchieves, completedAchieves, userdata) =>
    {
      if (achieveId != 0 && !updatedAchieves.Any<Achievement>((Func<Achievement, bool>) (x => x.ID == achieveId)) && !completedAchieves.Any<Achievement>((Func<Achievement, bool>) (x => x.ID == achieveId)))
        return;
      if (AchieveManager.Get().HasQuestsToShow(true))
        WelcomeQuests.Show(UserAttentionBlocker.ALL, true);
      else if ((UnityEngine.Object) GameToastMgr.Get() != (UnityEngine.Object) null)
        GameToastMgr.Get().UpdateQuestProgressToasts();
      AchieveManager.Get().RemoveAchievesUpdatedListener(action);
    }));
  }

  private void OnProcessCheat_util_achieves_ShowQuestLog()
  {
    if (!((UnityEngine.Object) QuestLog.Get() != (UnityEngine.Object) null) || QuestLog.Get().IsShown())
      return;
    QuestLog.Get().Show();
  }

  private bool ProcessAutofillParam_util_prog(
    string rawArgs,
    string[] cmdArgs,
    AutofillData autofillData,
    ref bool autoFillResult,
    ref bool requiresConfirm)
  {
    requiresConfirm = false;
    if (autofillData == null || cmdArgs.Length > 2)
      return false;
    string searchTerm1 = cmdArgs.Length < 1 ? (string) null : cmdArgs[0].ToLower();
    string searchTerm2 = cmdArgs.Length < 2 ? (string) null : cmdArgs[1].ToLower();
    bool flag = rawArgs.EndsWith(" ");
    if (searchTerm1 == null & flag || searchTerm1 != null && searchTerm2 == null && !flag)
    {
      string[] values = new string[5]
      {
        "quest",
        "pool",
        "achieve",
        "track",
        "task"
      };
      autoFillResult = Cheats.ProcessAutofillParam((IEnumerable<string>) values, searchTerm1, autofillData);
      return true;
    }
    if (searchTerm1 == null)
      return false;
    if ((searchTerm2 == null & flag || searchTerm2 != null && !flag) && searchTerm1 == "quest")
    {
      string[] values = new string[8]
      {
        "help",
        "view",
        "grant",
        "ack",
        "advance",
        "complete",
        "reset",
        "listen"
      };
      autoFillResult = Cheats.ProcessAutofillParam((IEnumerable<string>) values, searchTerm2, autofillData);
      return true;
    }
    if ((searchTerm2 == null & flag || searchTerm2 != null && !flag) && searchTerm1 == "pool")
    {
      string[] values = new string[11]
      {
        "help",
        "view",
        "grant",
        "login",
        "lastcheckdate",
        "lastgrantdate",
        "reroll",
        "reset",
        "set",
        "testcalcnumquests",
        "testcalctimeuntil"
      };
      autoFillResult = Cheats.ProcessAutofillParam((IEnumerable<string>) values, searchTerm2, autofillData);
      return true;
    }
    if ((searchTerm2 == null & flag || searchTerm2 != null && !flag) && searchTerm1 == "achieve")
    {
      string[] values = new string[9]
      {
        "help",
        "view",
        "score",
        "advance",
        "complete",
        "claim",
        "ack",
        "reset",
        "listen"
      };
      autoFillResult = Cheats.ProcessAutofillParam((IEnumerable<string>) values, searchTerm2, autofillData);
      return true;
    }
    if (!(searchTerm2 == null & flag) && (searchTerm2 == null || flag) || !(searchTerm1 == "track"))
      return false;
    string[] values1 = new string[9]
    {
      "help",
      "view",
      "set",
      "gamexp",
      "addxp",
      "levelup",
      "claim",
      "ack",
      "reset"
    };
    autoFillResult = Cheats.ProcessAutofillParam((IEnumerable<string>) values1, searchTerm2, autofillData);
    return true;
  }

  private static string Cheat_ShowRewardBoxes(string parsableRewardBags)
  {
    if (SceneMgr.Get().IsInGame())
      return "Cannot display reward boxes in gameplay.";
    string[] source = parsableRewardBags.Trim().Split(new char[1]
    {
      ' '
    }, StringSplitOptions.RemoveEmptyEntries);
    if (source.Length < 2)
      return "Error parsing reply, should start with 'Success:' then player_id: " + parsableRewardBags;
    if (source.Length < 3)
      return "No rewards returned by server: reply=" + parsableRewardBags;
    List<NetCache.ProfileNotice> notices = new List<NetCache.ProfileNotice>();
    string[] array = ((IEnumerable<string>) source).Skip<string>(1).ToArray<string>();
    for (int index1 = 0; index1 < array.Length; ++index1)
    {
      int result1 = 0;
      int index2 = index1 * 2;
      if (index2 < array.Length)
      {
        if (!int.TryParse(array[index2], out result1))
          return "Reward at index " + (object) index2 + " (" + array[index2] + ") is not an int: reply=" + parsableRewardBags;
        if (result1 != 0)
        {
          int index3 = index2 + 1;
          if (index3 >= array.Length)
            return "No reward bag data at index " + (object) index3 + ": reply=" + parsableRewardBags;
          long result2 = 0;
          if (!long.TryParse(array[index3], out result2))
            return "Reward Data at index " + (object) index3 + " (" + array[index3] + ") is not a long int: reply=" + parsableRewardBags;
          NetCache.ProfileNotice profileNotice = (NetCache.ProfileNotice) null;
          TAG_PREMIUM tagPremium = TAG_PREMIUM.NORMAL;
          switch (result1)
          {
            case 1:
            case 12:
            case 14:
            case 15:
            case 24:
              profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardBooster()
              {
                Id = (int) result2,
                Count = 1
              };
              break;
            case 2:
              profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardCurrency()
              {
                CurrencyType = PegasusShared.CurrencyType.CURRENCY_TYPE_GOLD,
                Amount = (int) result2
              };
              break;
            case 3:
              profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardDust()
              {
                Amount = (int) result2
              };
              break;
            case 4:
            case 5:
            case 6:
            case 7:
              profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardCard()
              {
                CardID = GameUtils.TranslateDbIdToCardId((int) result2),
                Premium = tagPremium
              };
              break;
            case 8:
            case 9:
            case 10:
            case 11:
              tagPremium = TAG_PREMIUM.GOLDEN;
              goto case 4;
            case 13:
              profileNotice = (NetCache.ProfileNotice) new NetCache.ProfileNoticeRewardCardBack()
              {
                CardBackID = (int) result2
              };
              break;
            default:
              Debug.LogError((object) ("Unknown Reward Bag Type: " + (object) result1 + " (data=" + (object) result2 + ") at index " + (object) index3 + ": reply=" + parsableRewardBags));
              break;
          }
          if (profileNotice != null)
            notices.Add(profileNotice);
        }
      }
      else
        break;
    }
    RewardBoxesDisplay objectOfType = UnityEngine.Object.FindObjectOfType<RewardBoxesDisplay>();
    if ((UnityEngine.Object) objectOfType != (UnityEngine.Object) null)
    {
      float secondsToWait = 0.0f;
      if (objectOfType.IsClosing)
        secondsToWait = 0.1f;
      else
        objectOfType.Close();
      Processor.ScheduleCallback(secondsToWait, false, (Processor.ScheduledCallback) (userData => Cheats.Cheat_ShowRewardBoxes(parsableRewardBags)));
      return (string) null;
    }
    List<RewardData> rewards = RewardUtils.GetRewards(notices);
    PrefabCallback<GameObject> callback = (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
    {
      RewardBoxesDisplay component = go.GetComponent<RewardBoxesDisplay>();
      component.SetRewards(callbackData as List<RewardData>);
      component.m_Root.transform.position = (bool) UniversalInputManager.UsePhoneUI ? new Vector3(0.0f, 14.7f, 3f) : new Vector3(0.0f, 131.2f, -3.2f);
      if ((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null && (UnityEngine.Object) Box.Get().GetBoxCamera() != (UnityEngine.Object) null && Box.Get().GetBoxCamera().GetState() == BoxCamera.State.OPENED)
      {
        component.m_Root.transform.position += new Vector3(-3f, 0.0f, 4.6f);
        if ((bool) UniversalInputManager.UsePhoneUI)
          component.m_Root.transform.position += new Vector3(0.0f, 0.0f, -7f);
        else
          component.transform.localScale = Vector3.one * 0.6f;
      }
      component.AnimateRewards();
    });
    AssetLoader.Get().InstantiatePrefab((AssetReference) RewardBoxesDisplay.GetPrefab(rewards), callback, (object) rewards);
    return (string) null;
  }

  private bool OnProcessCheat_gameservercmd(string func, string[] args, string rawArgs) => true;

  private bool OnProcessCheat_rewardboxes(string func, string[] args, string rawArgs)
  {
    string.IsNullOrEmpty(args[0].ToLower());
    int val = 5;
    if (args.Length > 1)
      GeneralUtils.TryParseInt(args[1], out val);
    BoosterDbId[] array = System.Enum.GetValues(typeof (BoosterDbId)).Cast<BoosterDbId>().Where<BoosterDbId>((Func<BoosterDbId, bool>) (i => i != 0)).ToArray<BoosterDbId>();
    BoosterDbId boosterDbId = array[UnityEngine.Random.Range(0, array.Length)];
    string message = Cheats.Cheat_ShowRewardBoxes("Success: 123456" + " " + (object) 13 + " " + (object) UnityEngine.Random.Range(1, 34) + " " + (object) 1 + " " + (object) (int) boosterDbId + " " + (object) 3 + " " + (object) (UnityEngine.Random.Range(1, 31) * 5) + " " + (object) 2 + " " + (object) (UnityEngine.Random.Range(1, 31) * 5) + " " + (object) (UnityEngine.Random.Range(0, 2) == 0 ? 6 : 10) + " " + (object) GameUtils.TranslateCardIdToDbId("EX1_279"));
    if (message != null)
      UIStatus.Get().AddError(message);
    return true;
  }

  private bool OnProcessCheat_rankrefresh(string func, string[] args, string rawArgs)
  {
    NetCache.Get().RegisterScreenEndOfGame(new NetCache.NetCacheCallback(this.OnNetCacheReady_CallRankChangeTwoScoopDebugShow));
    return true;
  }

  private void OnNetCacheReady_CallRankChangeTwoScoopDebugShow()
  {
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady_CallRankChangeTwoScoopDebugShow));
    RankChangeTwoScoop_NEW.DebugShowHelper(RankMgr.Get().GetLocalPlayerMedalInfo(), Options.GetFormatType());
  }

  private bool OnProcessCheat_rankchange(string func, string[] args, string rawArgs)
  {
    string cheatName = "bronze10";
    if (args.Length != 0 && !string.IsNullOrEmpty(args[0]))
      cheatName = args[0];
    LeagueRankDbfRecord recordByCheatName = RankMgr.Get().GetLeagueRankRecordByCheatName(cheatName);
    if (recordByCheatName == null)
      return false;
    PegasusShared.FormatType formatType = PegasusShared.FormatType.FT_STANDARD;
    bool isWinStreak = false;
    int stars = 0;
    int starsPerWin = 1;
    bool showWin = true;
    for (int index = 0; index < args.Length; ++index)
    {
      string lower = args[index].ToLower();
      if (lower == "winstreak" || lower == "streak")
        isWinStreak = true;
      else if (lower == "win")
        showWin = true;
      else if (lower == "loss")
        showWin = false;
      else if (lower == "wild")
        formatType = PegasusShared.FormatType.FT_WILD;
      else if (lower == "classic")
        formatType = PegasusShared.FormatType.FT_CLASSIC;
      else if (lower.StartsWith("x") || lower.EndsWith("x"))
        starsPerWin = int.Parse(lower.Trim('x'));
      else if (lower.StartsWith("*") || lower.EndsWith("*"))
        stars = int.Parse(lower.Trim('*'));
    }
    RankChangeTwoScoop_NEW.DebugShowFake(recordByCheatName.LeagueId, recordByCheatName.StarLevel, stars, starsPerWin, formatType, isWinStreak, showWin);
    return true;
  }

  private bool OnProcessCheat_rankreward(string func, string[] args, string rawArgs)
  {
    string cheatName = "bronze5";
    if (args.Length != 0 && !string.IsNullOrEmpty(args[0]))
      cheatName = args[0];
    LeagueRankDbfRecord recordByCheatName = RankMgr.Get().GetLeagueRankRecordByCheatName(cheatName);
    if (recordByCheatName == null)
      return false;
    PegasusShared.FormatType formatType = PegasusShared.FormatType.FT_STANDARD;
    bool flag = false;
    for (int index = 0; index < args.Length; ++index)
    {
      string lower = args[index].ToLower();
      if (lower == "standard")
        formatType = PegasusShared.FormatType.FT_STANDARD;
      else if (lower == "wild")
        formatType = PegasusShared.FormatType.FT_WILD;
      else if (lower == "classic")
        formatType = PegasusShared.FormatType.FT_CLASSIC;
      else if (lower == "all")
        flag = true;
    }
    MedalInfoTranslator medalInfoForLeagueId = MedalInfoTranslator.CreateMedalInfoForLeagueId(recordByCheatName.LeagueId, recordByCheatName.StarLevel, 1337);
    medalInfoForLeagueId.GetPreviousMedal(formatType).starLevel = flag ? 1 : recordByCheatName.StarLevel - 1;
    TranslatedMedalInfo currentMedal = medalInfoForLeagueId.GetCurrentMedal(formatType);
    currentMedal.bestStarLevel = recordByCheatName.StarLevel;
    NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
    if (netObject != null)
      currentMedal.seasonId = netObject.Season;
    List<List<RewardData>> rewardsEarned = new List<List<RewardData>>();
    if (!medalInfoForLeagueId.GetRankedRewardsEarned(formatType, ref rewardsEarned) || rewardsEarned.Count == 0)
      return false;
    RankedRewardDisplay.DebugShowFake(recordByCheatName.LeagueId, recordByCheatName.StarLevel, formatType, rewardsEarned);
    return true;
  }

  private bool OnProcessCheat_rankcardback(string func, string[] args, string rawArgs)
  {
    string cheatName = "bronze10";
    LeagueRankDbfRecord recordByCheatName = RankMgr.Get().GetLeagueRankRecordByCheatName(cheatName);
    if (recordByCheatName == null)
      return false;
    int val1 = 0;
    if (args.Length != 0 && !string.IsNullOrEmpty(args[0]) && !GeneralUtils.TryParseInt(args[0], out val1))
    {
      UIStatus.Get().AddInfo("please enter a valid int value for 1st parameter <wins>");
      return true;
    }
    int val2 = 0;
    if (args.Length >= 2 && !GeneralUtils.TryParseInt(args[1], out val2))
    {
      UIStatus.Get().AddInfo("please enter a valid int value for 2nd parameter <season_id>");
      return true;
    }
    if (val2 == 0)
    {
      NetCache.NetCacheRewardProgress netObject = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>();
      if (netObject != null)
        val2 = netObject.Season;
    }
    MedalInfoTranslator medalInfoForLeagueId = MedalInfoTranslator.CreateMedalInfoForLeagueId(recordByCheatName.LeagueId, recordByCheatName.StarLevel, 1337);
    TranslatedMedalInfo previousMedal = medalInfoForLeagueId.GetPreviousMedal(PegasusShared.FormatType.FT_STANDARD);
    TranslatedMedalInfo currentMedal = medalInfoForLeagueId.GetCurrentMedal(PegasusShared.FormatType.FT_STANDARD);
    previousMedal.seasonWins = Mathf.Max(0, val1 - 1);
    currentMedal.seasonWins = val1;
    currentMedal.seasonId = val2;
    RankedCardBackProgressDisplay.DebugShowFake(medalInfoForLeagueId);
    return true;
  }

  private bool OnProcessCheat_easyrank(string func, string[] args, string rawArgs)
  {
    string lower = args[0].ToLower();
    CheatMgr.Get().ProcessCheat(string.Format("util ranked set rank={0}", (object) lower));
    return true;
  }

  private bool OnProcessCheat_localmedaloverride(string func, string[] args, string rawArgs)
  {
    string str = "localmedaloverride";
    if (args.Length < 1 || string.IsNullOrEmpty(args[0]))
    {
      UIStatus.Get().AddError("expected use: " + str + " [ft_standard|ft_wild|ft_classic] star_level=# legend_rank=# OR off");
      return true;
    }
    if (args[0].ToLower() == "off")
    {
      NetCache.NetCacheMedalInfo.CheatLocalOverrideClear();
      return true;
    }
    string upper = args[0].ToUpper();
    PegasusShared.FormatType result;
    if (!System.Enum.TryParse<PegasusShared.FormatType>(upper, out result))
    {
      UIStatus.Get().AddError(str + " error: Unknown FormatType '" + upper + "'");
      return true;
    }
    if (result == PegasusShared.FormatType.FT_UNKNOWN)
    {
      UIStatus.Get().AddError(str + " error: Cannot use FormatType 'FT_UNKNOWN'");
      return true;
    }
    string[] strArray = new string[2]
    {
      "star_level",
      "legend_rank"
    };
    Map<string, Cheats.NamedParam> values;
    this.TryParseNamedArgs(args, out values);
    for (int index = 0; index < strArray.Length; ++index)
    {
      string key1 = strArray[index];
      Cheats.NamedParam namedParam1;
      if (!values.TryGetValue(key1, out namedParam1))
      {
        Map<string, Cheats.NamedParam> map = values;
        string key2 = key1;
        namedParam1 = new Cheats.NamedParam();
        Cheats.NamedParam namedParam2 = namedParam1;
        map.Add(key2, namedParam2);
      }
    }
    NetCache.NetCacheMedalInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMedalInfo>();
    Cheats.NamedParam namedParam3 = values["legend_rank"];
    int legendRank = 0;
    if (namedParam3.HasNumber)
    {
      legendRank = namedParam3.Number;
      netObject.CheatLocalOverrideLegendRank(result, legendRank);
    }
    Cheats.NamedParam namedParam4 = values["star_level"];
    int starLevel = 0;
    if (namedParam4.HasNumber || namedParam3.HasNumber)
    {
      starLevel = namedParam4.HasNumber ? namedParam4.Number : 51;
      netObject.CheatLocalOverrideStarLevel(result, starLevel);
    }
    if (namedParam3.HasNumber)
      UIStatus.Get().AddInfo(string.Format("Setting local medal {0} to star_level={1} and legend_rank={2}", (object) result, (object) starLevel, (object) legendRank));
    else if (namedParam4.HasNumber)
      UIStatus.Get().AddInfo(string.Format("Setting local medal {0} to star_level={1}", (object) result, (object) starLevel));
    this.OnProcessCheat_rankrefresh(func, args, rawArgs);
    return true;
  }

  private bool OnProcessCheat_timescale(string func, string[] args, string rawArgs)
  {
    string lower = args[0].ToLower();
    if (string.IsNullOrEmpty(lower))
    {
      float timeScale = UnityEngine.Time.timeScale;
      float timescaleMultiplier = SceneDebugger.GetDevTimescaleMultiplier();
      string message = (double) timeScale != (double) timescaleMultiplier ? string.Format("Current timeScale is: {0}\nDev timescale: {1}\nGame timescale: {2}", (object) timeScale, (object) timescaleMultiplier, (object) TimeScaleMgr.Get().GetGameTimeScale()) : string.Format("Current timeScale is: {0}", (object) timeScale);
      UIStatus.Get().AddInfo(message, 3f * SceneDebugger.GetDevTimescaleMultiplier());
      return true;
    }
    float result = 1f;
    if (!float.TryParse(lower, out result))
      return false;
    SceneDebugger.SetDevTimescaleMultiplier(result);
    UIStatus.Get().AddInfo(string.Format("Setting timescale to: {0}", (object) result), 3f * result);
    return true;
  }

  private bool OnProcessCheat_reset(string func, string[] args, string rawArgs)
  {
    HearthstoneApplication.Get().Reset();
    return true;
  }

  private bool OnProcessCheat_endturn(string func, string[] args, string rawArgs)
  {
    UIStatus.Get().AddError("Deprecated. Use \"cheat endturn\" instead.");
    return true;
  }

  private bool OnProcessCheat_battlegrounds(string func, string[] args, string rawArgs)
  {
    if (SceneMgr.Get().IsInGame())
    {
      UIStatus.Get().AddError("Cannot queue for a battlegrounds game while in gameplay.");
      return true;
    }
    if (DialogManager.Get().ShowingDialog())
    {
      UIStatus.Get().AddError("Cannot queue for a battlegrounds game while a dialog is active.");
      return true;
    }
    GameMgr.Get().FindGame(GameType.GT_BATTLEGROUNDS, PegasusShared.FormatType.FT_WILD, 3459);
    return true;
  }

  private bool OnProcessCheat_scenario(string func, string[] args, string rawArgs)
  {
    string[] strArray = new string[5]
    {
      "id",
      "game_type",
      "deck_id",
      "format_type",
      "prog_override"
    };
    Map<string, Cheats.NamedParam> values;
    bool namedArgs = this.TryParseNamedArgs(args, out values);
    for (int index = 0; index < strArray.Length; ++index)
    {
      string key1 = strArray[index];
      Cheats.NamedParam namedParam1;
      if (!values.TryGetValue(key1, out namedParam1))
      {
        if (!namedArgs && args.Length > index)
        {
          values.Add(key1, new Cheats.NamedParam(args[index]));
        }
        else
        {
          Map<string, Cheats.NamedParam> map = values;
          string key2 = key1;
          namedParam1 = new Cheats.NamedParam();
          Cheats.NamedParam namedParam2 = namedParam1;
          map.Add(key2, namedParam2);
        }
      }
    }
    Cheats.NamedParam namedParam3 = values["id"];
    int num = 260;
    if (namedParam3.HasNumber)
    {
      num = namedParam3.Number;
      if (GameDbf.Scenario.GetRecord(num) == null)
      {
        Error.AddWarning("scenario Cheat Error", "Error reading a scenario id from \"{0}\"", (object) num);
        return false;
      }
    }
    Cheats.NamedParam namedParam4 = values["game_type"];
    GameType gameType = GameType.GT_VS_AI;
    if (namedParam4.HasNumber)
    {
      gameType = (GameType) namedParam4.Number;
      if (gameType == GameType.GT_UNKNOWN)
      {
        Error.AddWarning("scenario Cheat Error", "Error reading a game type from \"{0}\"", (object) gameType);
        return false;
      }
    }
    else if (Cheats.s_scenarioToGameTypeMap.ContainsKey((ScenarioDbId) num))
      gameType = Cheats.s_scenarioToGameTypeMap[(ScenarioDbId) num];
    Cheats.NamedParam deckParam = values["deck_id"];
    CollectionDeck deck = (CollectionDeck) null;
    if (deckParam.HasNumber)
      deck = CollectionManager.Get().GetDeck((long) deckParam.Number);
    if (deckParam.HasNumber && deck == null)
    {
      deck = CollectionManager.Get().GetDecks().Where<KeyValuePair<long, CollectionDeck>>((Func<KeyValuePair<long, CollectionDeck>, bool>) (x => x.Value.Name.Equals(deckParam.Text, StringComparison.CurrentCultureIgnoreCase))).FirstOrDefault<KeyValuePair<long, CollectionDeck>>().Value;
      if (deck == null)
      {
        Error.AddWarning("scenario Cheat Error", "Error reading a deck id from \"{0}\"", (object) deck);
        return false;
      }
    }
    Cheats.NamedParam namedParam5 = values["format_type"];
    PegasusShared.FormatType formatType = PegasusShared.FormatType.FT_WILD;
    if (namedParam5.HasNumber)
    {
      formatType = (PegasusShared.FormatType) namedParam5.Number;
      if (formatType == PegasusShared.FormatType.FT_UNKNOWN)
      {
        Error.AddWarning("scenario Cheat Error", "Error reading a format type from \"{0}\"", (object) formatType);
        return false;
      }
    }
    Cheats.NamedParam namedParam6 = values["prog_override"];
    GameType progFilterOverride = GameType.GT_UNKNOWN;
    if (namedParam6.HasNumber)
    {
      progFilterOverride = (GameType) namedParam6.Number;
      if (progFilterOverride == GameType.GT_UNKNOWN)
      {
        Error.AddWarning("scenario Cheat Error", "Error reading a prog override from \"{0}\"", (object) progFilterOverride);
        return false;
      }
    }
    Cheats.QuickLaunchAvailability launchAvailability = this.GetQuickLaunchAvailability();
    switch (launchAvailability)
    {
      case Cheats.QuickLaunchAvailability.OK:
        this.LaunchQuickGame(num, gameType, formatType, deck, progFilterOverride: progFilterOverride);
        return true;
      case Cheats.QuickLaunchAvailability.FINDING_GAME:
        Error.AddDevWarning("scenario Cheat Error", "You are already finding a game.");
        break;
      case Cheats.QuickLaunchAvailability.ACTIVE_GAME:
        Error.AddDevWarning("scenario Cheat Error", "You are already in a game.");
        break;
      case Cheats.QuickLaunchAvailability.SCENE_TRANSITION:
        Error.AddDevWarning("scenario Cheat Error", "Can't start a game because a scene transition is active.");
        break;
      case Cheats.QuickLaunchAvailability.COLLECTION_NOT_READY:
        Error.AddDevWarning("scenario Cheat Error", "Can't start a game because your collection is not fully loaded.");
        break;
      default:
        Error.AddDevWarning("scenario Cheat Error", "Can't start a game: {0}", (object) launchAvailability);
        break;
    }
    return false;
  }

  private bool OnProcessCheat_aigame(string func, string[] args, string rawArgs)
  {
    int missionId = 3680;
    GameType gameType = GameType.GT_VS_AI;
    string str1 = args[0];
    if (string.IsNullOrEmpty(str1))
    {
      Error.AddWarning("aigame Cheat Error", "No deck string supplied");
      return false;
    }
    if (ShareableDeck.Deserialize(str1) == null)
    {
      Error.AddWarning("aigame Cheat Error", "Invalid deck string supplied \"{0}\"", (object) str1);
      return false;
    }
    PegasusShared.FormatType outVal = PegasusShared.FormatType.FT_WILD;
    if (args.Length > 1)
    {
      string str2 = args[1];
      int result;
      if (int.TryParse(str2, out result))
        outVal = (PegasusShared.FormatType) result;
      else if (!Blizzard.T5.Core.Utils.EnumUtils.TryGetEnum<PegasusShared.FormatType>(str2, out outVal))
      {
        string lower = str2.ToLower();
        if (!(lower == "wild"))
        {
          if (lower == "standard" || lower == "std")
          {
            outVal = PegasusShared.FormatType.FT_STANDARD;
          }
          else
          {
            Error.AddWarning("scenario Cheat Error", "Error reading a parameter for FormatType \"{0}\", please use \"wild\" or \"standard\"", (object) str2);
            return false;
          }
        }
        else
          outVal = PegasusShared.FormatType.FT_WILD;
      }
    }
    Cheats.QuickLaunchAvailability launchAvailability = this.GetQuickLaunchAvailability();
    switch (launchAvailability)
    {
      case Cheats.QuickLaunchAvailability.OK:
        this.LaunchQuickGame(missionId, gameType, outVal, aiDeck: str1);
        return true;
      case Cheats.QuickLaunchAvailability.FINDING_GAME:
        Error.AddDevWarning("scenario Cheat Error", "You are already finding a game.");
        break;
      case Cheats.QuickLaunchAvailability.ACTIVE_GAME:
        Error.AddDevWarning("scenario Cheat Error", "You are already in a game.");
        break;
      case Cheats.QuickLaunchAvailability.SCENE_TRANSITION:
        Error.AddDevWarning("scenario Cheat Error", "Can't start a game because a scene transition is active.");
        break;
      case Cheats.QuickLaunchAvailability.COLLECTION_NOT_READY:
        Error.AddDevWarning("scenario Cheat Error", "Can't start a game because your collection is not fully loaded.");
        break;
      default:
        Error.AddDevWarning("scenario Cheat Error", "Can't start a game: {0}", (object) launchAvailability);
        break;
    }
    return false;
  }

  private bool OnProcessCheat_loadSnapshot(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1)
      return false;
    string path = args[0];
    if (!path.EndsWith(".replay"))
      path += ".replay";
    if (!System.IO.File.Exists(path))
    {
      Error.AddDevWarning("loadsnapshot Cheat Error", string.Format("Replay file {0}\nnot found!", (object) path));
      return false;
    }
    byte[] numArray = System.IO.File.ReadAllBytes(path);
    GameSnapshot gameSnapshot = new GameSnapshot();
    gameSnapshot.Deserialize((Stream) new MemoryStream(numArray));
    Cheats.QuickLaunchAvailability launchAvailability = this.GetQuickLaunchAvailability();
    switch (launchAvailability)
    {
      case Cheats.QuickLaunchAvailability.OK:
        GameType gameType = gameSnapshot.GameType;
        PegasusShared.FormatType formatType = gameSnapshot.FormatType;
        int scenarioId = gameSnapshot.ScenarioId;
        this.m_quickLaunchState.m_launching = true;
        string message = string.Format("Launching game from replay file\n{0}", (object) path);
        UIStatus.Get().AddInfo(message);
        SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
        GameMgr.Get().SetPendingAutoConcede(true);
        GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
        GameMgr.Get().FindGame(gameType, formatType, scenarioId, snapshot: numArray);
        return true;
      case Cheats.QuickLaunchAvailability.FINDING_GAME:
        Error.AddDevWarning("loadsnapshot Cheat Error", "You are already finding a game.");
        break;
      case Cheats.QuickLaunchAvailability.ACTIVE_GAME:
        Error.AddDevWarning("loadsnapshot Cheat Error", "You are already in a game.");
        break;
      case Cheats.QuickLaunchAvailability.SCENE_TRANSITION:
        Error.AddDevWarning("loadsnapshot Cheat Error", "Can't start a game because a scene transition is active.");
        break;
      case Cheats.QuickLaunchAvailability.COLLECTION_NOT_READY:
        Error.AddDevWarning("loadsnapshot Cheat Error", "Can't start a game because your collection is not fully loaded.");
        break;
      default:
        Error.AddDevWarning("loadsnapshot Cheat Error", "Can't start a game: {0}", (object) launchAvailability);
        break;
    }
    return false;
  }

  private bool OnProcessCheat_exportcards(string func, string[] args, string rawArgs)
  {
    SceneManager.LoadScene("ExportCards");
    return true;
  }

  private bool OnProcessCheat_exportcardbacks(string func, string[] args, string rawArgs)
  {
    SceneManager.LoadScene("ExportCardBacks");
    return true;
  }

  private bool OnProcessCheat_freeyourmind(string func, string[] args, string rawArgs)
  {
    this.m_isNewCardInPackOpeningEnabled = true;
    return true;
  }

  private bool OnProcessCheat_reloadgamestrings(string func, string[] args, string rawArgs)
  {
    GameStrings.ReloadAll();
    return true;
  }

  private bool OnProcessCheat_userattentionmanager(string func, string[] args, string rawArgs)
  {
    string str = UserAttentionManager.DumpUserAttentionBlockers(nameof (OnProcessCheat_userattentionmanager));
    UIStatus.Get().AddInfo(string.Format("Current UserAttentionBlockers: {0}", (object) str));
    return true;
  }

  private void Cheat_ShowBannerList()
  {
    StringBuilder stringBuilder = new StringBuilder();
    bool flag = true;
    foreach (BannerDbfRecord bannerDbfRecord in (IEnumerable<BannerDbfRecord>) GameDbf.Banner.GetRecords().OrderByDescending<BannerDbfRecord, int>((Func<BannerDbfRecord, int>) (r => r.ID)))
    {
      if (!flag)
        stringBuilder.Append("\n");
      flag = false;
      stringBuilder.AppendFormat("{0}. {1}", (object) bannerDbfRecord.ID, (object) bannerDbfRecord.NoteDesc);
    }
    UIStatus.Get().AddInfo(stringBuilder.ToString(), 5f);
  }

  private bool OnProcessCheat_banner(string func, string[] args, string rawArgs)
  {
    int result = 0;
    if (args.Length < 1 || string.IsNullOrEmpty(args[0]))
      result = GameDbf.Banner.GetRecords().Max<BannerDbfRecord>((Func<BannerDbfRecord, int>) (r => r.ID));
    else if (int.TryParse(args[0], out result))
    {
      if (GameDbf.Banner.GetRecord(result) == null)
      {
        UIStatus.Get().AddInfo(string.Format("Unknown bannerId: {0}", (object) result));
        return true;
      }
    }
    else
    {
      if (args[0].Equals("list", StringComparison.InvariantCultureIgnoreCase))
      {
        this.Cheat_ShowBannerList();
        return true;
      }
      UIStatus.Get().AddInfo(string.Format("Unknown parameter: {0}", (object) args[0]));
      return true;
    }
    BannerManager.Get().ShowBanner(result);
    return true;
  }

  private bool OnProcessCheat_raf(string func, string[] args, string rawArgs)
  {
    string lower = args[0].ToLower();
    if (string.Equals(lower, "showhero"))
      RAFManager.Get().ShowRAFHeroFrame();
    else if (string.Equals(lower, "showprogress"))
      RAFManager.Get().ShowRAFProgressFrame();
    else if (string.Equals(lower, "setprogress"))
    {
      if (args.Length > 1)
      {
        int int32 = Convert.ToInt32(args[1]);
        RAFManager.Get().SetRAFProgress(int32);
      }
    }
    else if (string.Equals(lower, "showglows"))
    {
      Options.Get().SetBool(Option.HAS_SEEN_RAF, false);
      Options.Get().SetBool(Option.HAS_SEEN_RAF_RECRUIT_URL, false);
      FriendListFrame friendListFrame = ChatMgr.Get().FriendListFrame;
      if ((UnityEngine.Object) friendListFrame != (UnityEngine.Object) null)
        friendListFrame.UpdateRAFButtonGlow();
      RAFFrame rafFrame = RAFManager.Get().GetRAFFrame();
      if ((UnityEngine.Object) rafFrame != (UnityEngine.Object) null)
        rafFrame.UpdateRecruitFriendsButtonGlow();
      RAFManager.Get().ShowRAFProgressFrame();
    }
    return true;
  }

  private bool OnProcessCheat_returningplayer(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1)
      Error.AddWarning("returningplayer Cheat Error", "No parameter provided.");
    int val;
    if (!GeneralUtils.TryParseInt(args[0], out val))
    {
      Error.AddWarning("returningplayer Cheat Error", "Error reading an int from \"{0}\"", (object) args[0]);
      return false;
    }
    ReturningPlayerMgr.Get().Cheat_SetReturningPlayerProgress(val);
    return true;
  }

  private bool OnProcessCheat_ratingdebug(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1 || string.IsNullOrEmpty(args[0]))
    {
      UIStatus.Get().AddError("ratingdebug cheat must have rating id # or [standard/wild/classic/bg/mercs]");
      return true;
    }
    string lower = args[0].ToLower();
    int val;
    if (!GeneralUtils.TryParseInt(lower, out val))
    {
      if (string.Equals(lower, "standard"))
        val = 1;
      else if (string.Equals(lower, "wild"))
        val = 5;
      else if (string.Equals(lower, "classic"))
        val = 12;
      else if (string.Equals(lower, "bg"))
        val = 8;
      else if (string.Equals(lower, "mercs"))
      {
        val = 14;
      }
      else
      {
        UIStatus.Get().AddError("ratingdebug error: Unknown argument '" + lower + "'");
        return true;
      }
    }
    if (!System.Enum.IsDefined(typeof (RatingDebugOption), (object) val) || val == 0)
    {
      UIStatus.Get().AddError(string.Format("ratingdebug error: Unknown rating id '{0}'", (object) val));
      return true;
    }
    Options.Get().SetEnum<RatingDebugOption>(Option.RATING_DEBUG, (RatingDebugOption) val);
    SceneDebugger.Get().RequestDebugRatingInfo();
    return true;
  }

  private bool OnProcessCheat_resetrankedintro(string func, string[] args, string rawArgs)
  {
    List<GameSaveDataManager.SubkeySaveRequest> requests = new List<GameSaveDataManager.SubkeySaveRequest>()
    {
      new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_INTRO_SEEN_COUNT, new long[1]),
      new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_LAST_SEASON_BONUS_STARS_POPUP_SEEN, new long[1]),
      new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_BONUS_STARS_POPUP_SEEN_COUNT, new long[1]),
      new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RANKED_PLAY, GameSaveKeySubkeyId.RANKED_PLAY_LAST_REWARDS_VERSION_SEEN, new long[1])
    };
    if (GameSaveDataManager.Get().SaveSubkeys(requests))
    {
      UIStatus.Get().AddInfo("Ranked intro game save data keys reset.");
      return true;
    }
    UIStatus.Get().AddInfo("Failed to reset ranked intro game save data keys!");
    return false;
  }

  private bool OnProcessCheat_advevent(string func, string[] args, string rawArgs)
  {
    if ((UnityEngine.Object) AdventureScene.Get() == (UnityEngine.Object) null || (UnityEngine.Object) AdventureMissionDisplay.Get() == (UnityEngine.Object) null || SceneMgr.Get().GetMode() != SceneMgr.Mode.ADVENTURE)
    {
      UIStatus.Get().AddError("You must be viewing an Adventure to use this cheat!");
      return true;
    }
    if (args.Length < 1 || string.IsNullOrEmpty(args[0]))
    {
      UIStatus.Get().AddError("You must provide an event from AdventureWingEventTable as a parameter!");
      return true;
    }
    if (AdventureMissionDisplay.Get().Cheat_AdventureEvent(args[0]))
      UIStatus.Get().AddInfo(string.Format("Triggered event {0} on each wing's AdventureWingEventTable.", (object) args[0]));
    else
      UIStatus.Get().AddInfo("Could not activate cheat 'advevent', perhaps 'advdev' has not been enabled yet?");
    return true;
  }

  private bool OnProcessCheat_lowmemorywarning(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1)
      MobileCallbackManager.Get().LowMemoryWarning("");
    else
      MobileCallbackManager.Get().LowMemoryWarning(args[0]);
    return true;
  }

  private bool OnProcessCheat_mobile(string func, string[] args, string rawArgs)
  {
    string lower1 = args[0].ToLower();
    if (string.Equals(lower1, "login"))
    {
      if (args.Length > 1 && string.Equals(args[1].ToLower(), "clear"))
      {
        ServiceManager.Get<ILoginService>()?.WipeAllAuthenticationData();
        UIStatus.Get().AddInfo("Mobile Login Cleared!");
      }
    }
    else if (string.Equals(lower1, "push"))
    {
      if (args.Length > 1)
      {
        string lower2 = args[1].ToLower();
        if (string.Equals(lower2, "register"))
        {
          PushNotificationManager.Get().RegisterPushNotifications();
          UIStatus.Get().AddInfo("Registered for Push!");
        }
        else if (string.Equals(lower2, "logout"))
        {
          PushNotificationManager.Get().UnregisterPushNotifications();
          UIStatus.Get().AddInfo("Logged Out for Push!");
        }
      }
    }
    else if (string.Equals(lower1, "ngdp") && args.Length > 1 && string.Equals(args[1].ToLower(), "clear"))
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = "GameDownloadManager",
        m_text = "Hearthstone can crash after clearing data. Do you still want to clear the data? Please re-launch Hearthstone after clearing data.",
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
        {
          if (response == AlertPopup.Response.CANCEL || this.DownloadManager == null)
            return;
          this.DownloadManager.DeleteDownloadedData();
        })
      });
    return true;
  }

  private bool OnProcessCheat_edittextdebug(string func, string[] args, string rawArgs)
  {
    if (PlatformSettings.RuntimeOS == OSCategory.Android)
      TextField.ToggleDebug();
    else
      UIStatus.Get().AddInfo("EditText debug is only for Android platforms");
    return true;
  }

  private bool OnProcessCheat_resetrotationtutorial(string func, string[] args, string rawArgs)
  {
    bool flag = true;
    if (args.Length != 0)
    {
      string lower = args[0].ToLower();
      if (string.Equals(lower, "veteran"))
        flag = false;
      else if (!string.IsNullOrEmpty(lower) && !string.Equals(lower, "newbie"))
      {
        string message = string.Format("resetrotationtutorial: {0} is not a valid parameter!", (object) lower);
        UIStatus.Get().AddError(message);
        return true;
      }
    }
    if (flag)
    {
      Options.Get().SetBool(Option.HAS_SEEN_STANDARD_MODE_TUTORIAL, false);
      Options.Get().SetInt(Option.SET_ROTATION_INTRO_PROGRESS, 0);
      Options.Get().SetInt(Option.SET_ROTATION_INTRO_PROGRESS_NEW_PLAYER, 0);
      Options.Get().SetBool(Option.NEEDS_TO_MAKE_STANDARD_DECK, true);
    }
    else
    {
      Options.Get().SetBool(Option.HAS_SEEN_STANDARD_MODE_TUTORIAL, true);
      Options.Get().SetInt(Option.SET_ROTATION_INTRO_PROGRESS, DateTime.Now.Year - 1);
      Options.Get().SetInt(Option.SET_ROTATION_INTRO_PROGRESS_NEW_PLAYER, DateTime.Now.Year - 1);
    }
    Options.Get().SetBool(Option.DISABLE_SET_ROTATION_INTRO, false);
    string message1 = string.Format("Set Rotation tutorial progress reset as a {0}!\nReset disableSetRotationIntro to false. Restart client to trigger the flow.", flag ? (object) "newbie" : (object) "veteran");
    UIStatus.Get().AddInfo(message1);
    return true;
  }

  private bool OnProcessCheat_cloud(string func, string[] args, string rawArgs)
  {
    string lower = args[0].ToLower();
    if (string.Equals(lower, "set"))
    {
      if (args.Length > 2)
      {
        string key = args[1];
        string str = args[2];
        if (string.Equals(str.ToLower(), "blank"))
          str = "";
        CloudStorageManager.Get().SetString(key, str);
        UIStatus.Get().AddInfo("Cloud Storage Set: (" + key + ", " + str + ")");
      }
    }
    else if (string.Equals(lower, "get"))
    {
      if (args.Length > 1)
      {
        string key = args[1];
        string str = CloudStorageManager.Get().GetString(key);
        UIStatus.Get().AddInfo("Cloud Storage Get: Value for " + key + " is " + (str == null ? "NULL" : str));
      }
    }
    else if (string.Equals(lower, "reset"))
    {
      Options.Get().SetBool(Option.DISALLOWED_CLOUD_STORAGE, false);
      UIStatus.Get().AddInfo("Cloud Storage Disallow Reset!");
    }
    return true;
  }

  private bool OnProcessCheat_tempaccount(string func, string[] args, string rawArgs)
  {
    string lower1 = args[0].ToLower();
    if (string.Equals(lower1, "dialog"))
    {
      if (args.Length > 1)
      {
        string lower2 = args[1].ToLower();
        if (string.Equals(lower2, "skip"))
          CreateSkipHelper.ShowCreateSkipDialog((System.Action) null);
        else if (string.Equals(lower2, "clear"))
        {
          Options.Get().SetBool(Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_TRADITIONAL, false);
          Options.Get().SetBool(Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_BATTLEGROUNDS, false);
          Options.Get().SetBool(Option.HAS_SEEN_HEAL_UP_POPUP_AFTER_TUTORIAL_MERCENARIES, false);
          UIStatus.Get().AddInfo("Create Skip Helper Options cleared");
        }
      }
      else
        TemporaryAccountManager.Get().ShowHealUpDialog(GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_HEADER_01"), GameStrings.Get("GLUE_TEMPORARY_ACCOUNT_DIALOG_BODY_03"), TemporaryAccountManager.HealUpReason.UNKNOWN, true, (TemporaryAccountManager.OnHealUpDialogDismissed) null);
    }
    else if (string.Equals(lower1, "cheat"))
    {
      if (args.Length > 1)
      {
        string lower3 = args[1].ToLower();
        if (string.Equals(lower3, "on"))
        {
          Options.Get().SetBool(Option.IS_TEMPORARY_ACCOUNT_CHEAT, true);
          UIStatus.Get().AddInfo("Temporary Account CHEAT is now ON");
        }
        else if (string.Equals(lower3, "off"))
        {
          Options.Get().SetBool(Option.IS_TEMPORARY_ACCOUNT_CHEAT, false);
          UIStatus.Get().AddInfo("Temporary Account CHEAT is now OFF");
        }
        else if (string.Equals(lower3, "clear"))
        {
          Options.Get().DeleteOption(Option.IS_TEMPORARY_ACCOUNT_CHEAT);
          UIStatus.Get().AddInfo("Temporary Account CHEAT is now CLEARED");
        }
      }
    }
    else if (string.Equals(lower1, "status"))
    {
      string str = "Temporary Account status is " + (BattleNet.IsHeadlessAccount() ? "ON" : "OFF") + " Cheat is ";
      string message = !Options.Get().HasOption(Option.IS_TEMPORARY_ACCOUNT_CHEAT) ? str + "CLEARED" : str + (Options.Get().GetBool(Option.IS_TEMPORARY_ACCOUNT_CHEAT) ? "ON" : "OFF");
      UIStatus.Get().AddInfo(message);
    }
    else if (string.Equals(lower1, "tutorial"))
    {
      if (args.Length > 1)
      {
        string lower4 = args[1].ToLower();
        if (string.Equals(lower4, "skip"))
        {
          Options.Get().SetBool(Option.CONNECT_TO_AURORA, true);
          Options.Get().SetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS, TutorialProgress.ILLIDAN_COMPLETE);
          UIStatus.Get().AddInfo("Set to Skip No Account Tutorial");
        }
        else if (string.Equals(lower4, "reset"))
        {
          Options.Get().SetBool(Option.CONNECT_TO_AURORA, false);
          Options.Get().SetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS, TutorialProgress.NOTHING_COMPLETE);
          UIStatus.Get().AddInfo("Set to Reset No Account Tutorial");
        }
      }
    }
    else if (string.Equals(lower1, "id"))
    {
      string temporaryAccountId = TemporaryAccountManager.Get().GetSelectedTemporaryAccountId();
      UIStatus.Get().AddInfo("Selected Temporary Account ID is " + (temporaryAccountId == null ? "NULL" : temporaryAccountId));
    }
    else if (string.Equals(lower1, "healupachievement"))
      AchieveManager.Get().NotifyOfAccountCreation();
    else if (string.Equals(lower1, "showswitchaccount"))
      TemporaryAccountManager.Get().ShowSwitchAccountMenu((SwitchAccountMenu.OnSwitchAccountLogInPressed) null, false);
    else if (string.Equals(lower1, "data"))
    {
      if (args.Length > 1)
      {
        string lower5 = args[1].ToLower();
        if (string.Equals(lower5, "print"))
        {
          TemporaryAccountManager.Get().PrintTemporaryAccountData();
          UIStatus.Get().AddInfo("Temporary Account Data Printed");
        }
        else if (string.Equals(lower5, "clear"))
        {
          TemporaryAccountManager.Get().DeleteTemporaryAccountData();
          UIStatus.Get().AddInfo("Temporary Account Data Deleted");
        }
      }
    }
    else if (string.Equals(lower1, "nag"))
    {
      if (args.Length > 1)
      {
        string lower6 = args[1].ToLower();
        if (string.Equals(lower6, "time"))
        {
          string str = TemporaryAccountManager.Get().NagTimeDebugLog();
          Log.TemporaryAccount.Print(str);
          UIStatus.Get().AddInfo(str);
        }
        else if (string.Equals(lower6, "clear"))
        {
          Options.Get().DeleteOption(Option.LAST_HEAL_UP_EVENT_DATE);
          UIStatus.Get().AddInfo("Last Heal Up Event Time Cleared!");
        }
      }
    }
    else if (string.Equals(lower1, "test"))
      TemporaryAccountManager.Get().Test();
    else if (string.Equals(lower1, "lazy"))
    {
      ServiceManager.Get<ILoginService>()?.ClearAuthentication();
      TemporaryAccountManager.Get().DeleteTemporaryAccountData();
      Options.Get().SetBool(Option.CONNECT_TO_AURORA, true);
      Options.Get().SetEnum<TutorialProgress>(Option.LOCAL_TUTORIAL_PROGRESS, TutorialProgress.ILLIDAN_COMPLETE);
    }
    return true;
  }

  private bool OnProcessCheat_arena(string func, string[] args, string rawArgs)
  {
    string str1 = args.Length >= 1 ? args[0] : (string) null;
    string str2 = args.Length >= 2 ? args[1] : (string) null;
    string s = args.Length >= 3 ? args[2] : (string) null;
    float delay = 5f * UnityEngine.Time.timeScale;
    if (string.IsNullOrEmpty(str1) || str1 == "help")
    {
      string message = str2 == "popup" ? "Valid arena popup args: clear, comingsoon [#days], endingsoon [#days]" : (str2 == "refresh" ? "refreshes Arena season info from server" : "Valid arena commands: popup refresh\n\nUse 'util arena' to execute cheats on server, e.g. 'util arena season x' to switch season to x.");
      UIStatus.Get().AddInfo(message, delay);
      return true;
    }
    string message1 = (string) null;
    if (str1 == "popup")
    {
      switch (str2)
      {
        case null:
        case "help":
          UIStatus.Get().AddInfo("Valid arena popup args: clear, comingsoon [#days], endingsoon [#days]", delay);
          return true;
        case "1":
        case "comingsoon":
          double result1;
          if (!double.TryParse(s, out result1))
            result1 = 13.0;
          DraftManager.Get().ShowArenaPopup_SeasonComingSoon((long) (result1 * 86400.0), (System.Action) null);
          message1 = string.Empty;
          break;
        case "2":
        case "endingsoon":
          double result2;
          if (!double.TryParse(s, out result2))
            result2 = 5.0;
          DraftManager.Get().ShowArenaPopup_SeasonEndingSoon((long) (result2 * 86400.0), (System.Action) null);
          message1 = string.Empty;
          break;
        case "clear":
        case "clearpopups":
        case "clearseen":
          if (s == "innkeeper")
          {
            DraftManager.Get().ClearAllInnkeeperPopups();
            message1 = "All arena innkeeper popups cleared.";
          }
          else
          {
            DraftManager.Get().ClearAllSeenPopups();
            message1 = "All arena popups cleared.";
          }
          NetCache.Get().DispatchClientOptionsToServer();
          break;
      }
    }
    else if (str1 == "refresh")
    {
      DraftManager.Get().RefreshCurrentSeasonFromServer();
      message1 = "Refreshing Arena season info from server.";
    }
    else if (str1 == "season")
      message1 = string.Format("Please use 'util arena {0}' instead.", (object) rawArgs);
    else if (str1 == "choices")
    {
      List<string> stringList1 = new List<string>();
      for (int index = 1; index < args.Length; ++index)
        stringList1.Add(args[index]);
      string[] output;
      if (this.TryParseArenaChoices(stringList1.ToArray(), out output))
      {
        List<string> stringList2 = new List<string>();
        stringList2.Add("arena");
        stringList2.Add("choices");
        foreach (string str3 in output)
          stringList2.Add(str3);
        this.OnProcessCheat_utilservercmd("util", stringList2.ToArray(), rawArgs, (AutofillData) null);
      }
      message1 = string.Empty;
    }
    NetCache.Get().DispatchClientOptionsToServer();
    if (message1 == null)
      message1 = string.Format("Unknown subcmd: {0}", (object) rawArgs);
    UIStatus.Get().AddInfo(message1, delay);
    return true;
  }

  private bool OnProcessCheat_EventTiming(
    string func,
    string[] args,
    string rawArgs,
    AutofillData autofillData)
  {
    args = ((IEnumerable<string>) args).Where<string>((Func<string, bool>) (a => !string.IsNullOrEmpty(a.Trim()))).ToArray<string>();
    if (autofillData != null)
    {
      List<string> list = SpecialEventManager.Get().AllKnownEvents.Select<SpecialEventType, string>((Func<SpecialEventType, string>) (e => SpecialEventManager.Get().GetName(e))).ToList<string>();
      if (args.Length <= 1)
        list.InsertRange(0, (IEnumerable<string>) new string[3]
        {
          "list",
          "listall",
          "help"
        });
      return Cheats.ProcessAutofillParam((IEnumerable<string>) list, args.Length == 0 ? string.Empty : ((IEnumerable<string>) args).Last<string>(), autofillData);
    }
    if (args.Length != 0 && args[0] == "help")
    {
      UIStatus.Get().AddInfoNoRichText("Lists events and whether or not they're Active.\nValid args: list | listall | [event names]\n", 5f * UnityEngine.Time.timeScale);
      return true;
    }
    List<SpecialEventType> specialEventTypeList = new List<SpecialEventType>();
    bool flag1 = false;
    bool flag2 = true;
    bool flag3 = false;
    foreach (string str1 in args)
    {
      if (str1 == "list")
        flag3 = true;
      else if (str1 == "listall")
      {
        flag3 = true;
        specialEventTypeList.AddRange((IEnumerable<SpecialEventType>) SpecialEventManager.Get().AllKnownEvents);
        flag2 = false;
      }
      else
      {
        string str2 = str1;
        if (str1.StartsWith("event=") && str1.Length > 6)
          str2 = str1.Substring(6);
        Func<string, string, bool> fnSubstringMatch = (Func<string, string, bool>) ((evtName, userInput) => evtName.Contains(userInput, StringComparison.InvariantCultureIgnoreCase));
        Func<string, string, bool> fnStartsWithMatch = (Func<string, string, bool>) ((evtName, userInput) => evtName.StartsWith(userInput, StringComparison.InvariantCultureIgnoreCase));
        Func<string, string, bool> fnEndsWithMatch = (Func<string, string, bool>) ((evtName, userInput) => evtName.EndsWith(userInput, StringComparison.InvariantCultureIgnoreCase));
        Func<string, string, bool> fnExactMatch = (Func<string, string, bool>) ((evtName, userInput) => evtName.Equals(userInput, StringComparison.InvariantCultureIgnoreCase));
        string[] names = str2.Split(',');
        Func<string, bool> fnIsMatch = (Func<string, bool>) (evtName => ((IEnumerable<string>) names).Any<string>((Func<string, bool>) (userInput =>
        {
          Func<string, string, bool> func1 = fnSubstringMatch;
          bool flag4 = false;
          bool flag5 = false;
          if (userInput.StartsWith("^"))
          {
            userInput = userInput.Substring(1);
            flag4 = true;
          }
          if (userInput.EndsWith("$"))
          {
            userInput = userInput.Substring(0, userInput.Length - 1);
            flag5 = true;
          }
          if (userInput.Length == 0)
            return false;
          if (flag4 & flag5)
            func1 = fnExactMatch;
          else if (flag4)
            func1 = fnStartsWithMatch;
          else if (flag5)
            func1 = fnEndsWithMatch;
          return func1(evtName, userInput);
        })));
        IEnumerable<SpecialEventType> collection = SpecialEventManager.Get().AllKnownEvents.Select(evt => new
        {
          evt = evt,
          evtName = SpecialEventManager.Get().GetName(evt)
        }).Where(_param1 => fnIsMatch(_param1.evtName)).Select(_param1 => _param1.evt);
        specialEventTypeList.AddRange(collection);
        flag2 = false;
      }
    }
    if (flag2)
    {
      specialEventTypeList = SpecialEventManager.Get().AllKnownEvents.Where<SpecialEventType>((Func<SpecialEventType, bool>) (e => SpecialEventManager.Get().IsEventActive(e, false))).ToList<SpecialEventType>();
      flag1 = true;
    }
    DateTime utcNow = DateTime.UtcNow;
    if (flag1)
      specialEventTypeList.RemoveAll((Predicate<SpecialEventType>) (e =>
      {
        DateTime? eventStartTimeUtc = SpecialEventManager.Get().GetEventStartTimeUtc(e);
        DateTime? eventEndTimeUtc = SpecialEventManager.Get().GetEventEndTimeUtc(e);
        TimeSpan timeSpan1 = eventStartTimeUtc.HasValue ? (eventStartTimeUtc.Value > utcNow ? eventStartTimeUtc.Value - utcNow : utcNow - eventStartTimeUtc.Value) : TimeSpan.MaxValue;
        TimeSpan timeSpan2 = eventEndTimeUtc.HasValue ? (eventEndTimeUtc.Value > utcNow ? eventEndTimeUtc.Value - utcNow : utcNow - eventEndTimeUtc.Value) : TimeSpan.MaxValue;
        int num = timeSpan1.TotalDays <= 120.0 ? 1 : 0;
        bool flag6 = timeSpan2.TotalDays <= 120.0;
        return num == 0 && !flag6;
      }));
    if (specialEventTypeList.Count <= 0)
    {
      UIStatus.Get().AddInfoNoRichText("No events to show (check event names).");
      return true;
    }
    specialEventTypeList.Sort((Comparison<SpecialEventType>) ((lhs, rhs) =>
    {
      bool flag7 = SpecialEventManager.Get().IsEventActive(lhs, false);
      bool flag8 = SpecialEventManager.Get().IsEventActive(rhs, false);
      if (flag7 != flag8)
        return !flag7 ? 1 : -1;
      DateTime? eventStartTimeUtc1 = SpecialEventManager.Get().GetEventStartTimeUtc(lhs);
      DateTime? eventStartTimeUtc2 = SpecialEventManager.Get().GetEventStartTimeUtc(rhs);
      DateTime? nullable1 = eventStartTimeUtc1;
      DateTime? nullable2 = eventStartTimeUtc2;
      if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != nullable2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      {
        if (!eventStartTimeUtc1.HasValue)
          return -1;
        return eventStartTimeUtc2.HasValue ? eventStartTimeUtc1.Value.CompareTo(eventStartTimeUtc2.Value) : 1;
      }
      DateTime? eventEndTimeUtc1 = SpecialEventManager.Get().GetEventEndTimeUtc(lhs);
      DateTime? eventEndTimeUtc2 = SpecialEventManager.Get().GetEventEndTimeUtc(rhs);
      nullable2 = eventEndTimeUtc1;
      nullable1 = eventEndTimeUtc2;
      if ((nullable2.HasValue == nullable1.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != nullable1.GetValueOrDefault() ? 1 : 0) : 0) : 1) == 0)
        return SpecialEventManager.Get().GetName(lhs).CompareTo(SpecialEventManager.Get().GetName(rhs));
      if (!eventEndTimeUtc1.HasValue)
        return 1;
      return eventEndTimeUtc2.HasValue ? eventEndTimeUtc1.Value.CompareTo(eventEndTimeUtc2.Value) : -1;
    }));
    StringBuilder stringBuilder = new StringBuilder();
    foreach (SpecialEventType eventType in specialEventTypeList)
    {
      if (flag3)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append(", ");
        stringBuilder.Append(SpecialEventManager.Get().GetName(eventType));
      }
      else
      {
        bool flag9 = SpecialEventManager.Get().IsEventActive(eventType, false);
        DateTime? eventStartTimeUtc = SpecialEventManager.Get().GetEventStartTimeUtc(eventType);
        DateTime? eventEndTimeUtc = SpecialEventManager.Get().GetEventEndTimeUtc(eventType);
        DateTime? nullable3 = eventStartTimeUtc;
        DateTime? nullable4 = eventEndTimeUtc;
        DateTime dateTime;
        if (nullable3.HasValue)
        {
          ref DateTime? local = ref nullable3;
          dateTime = nullable3.Value;
          dateTime = dateTime.AddSeconds((double) SpecialEventManager.Get().DevTimeOffsetSeconds);
          DateTime localTime = dateTime.ToLocalTime();
          local = new DateTime?(localTime);
        }
        if (nullable4.HasValue)
        {
          ref DateTime? local = ref nullable4;
          dateTime = nullable4.Value;
          dateTime = dateTime.AddSeconds((double) SpecialEventManager.Get().DevTimeOffsetSeconds);
          DateTime localTime = dateTime.ToLocalTime();
          local = new DateTime?(localTime);
        }
        if (stringBuilder.Length != 0)
          stringBuilder.Append("\n");
        string str3;
        if (!nullable3.HasValue)
        {
          str3 = "<always>";
        }
        else
        {
          dateTime = nullable3.Value;
          str3 = dateTime.ToString("yyyy/MM/dd");
        }
        string str4 = str3;
        string str5;
        if (!nullable4.HasValue)
        {
          str5 = "<forever>";
        }
        else
        {
          dateTime = nullable4.Value;
          str5 = dateTime.ToString("yyyy/MM/dd");
        }
        string str6 = str5;
        stringBuilder.AppendFormat("{0} {1} {2}-{3}", (object) SpecialEventManager.Get().GetName(eventType), flag9 ? (object) "Active" : (object) "Inactive", (object) str4, (object) str6);
        if (flag9)
        {
          TimeSpan? nullable5 = !eventEndTimeUtc.HasValue || eventEndTimeUtc.Value < utcNow ? new TimeSpan?() : new TimeSpan?(eventEndTimeUtc.Value - utcNow);
          if (nullable5.HasValue && nullable5.Value.TotalDays < 3.0)
            stringBuilder.AppendFormat(" ends in {0}", (object) TimeUtils.GetElapsedTimeString((int) nullable5.Value.TotalSeconds, TimeUtils.SPLASHSCREEN_DATETIME_STRINGSET, true));
        }
        else
        {
          TimeSpan? nullable6 = !eventStartTimeUtc.HasValue || eventStartTimeUtc.Value < utcNow ? new TimeSpan?() : new TimeSpan?(eventStartTimeUtc.Value - utcNow);
          if (nullable6.HasValue && nullable6.Value.TotalDays < 3.0)
            stringBuilder.AppendFormat(" starts in {0}", (object) TimeUtils.GetElapsedTimeString((int) nullable6.Value.TotalSeconds, TimeUtils.SPLASHSCREEN_DATETIME_STRINGSET, true));
        }
      }
    }
    stringBuilder.Append("\n");
    float delay = (float) Mathf.Max(5, 2 * Mathf.Min(20, specialEventTypeList.Count)) * UnityEngine.Time.timeScale;
    string str = stringBuilder.ToString();
    Log.EventTiming.PrintInfo(str);
    UIStatus.Get().AddInfoNoRichText(str, delay);
    return true;
  }

  private bool OnProcessCheat_UpdateIntention(string func, string[] args, string rawArgs)
  {
    Options.Get().SetInt(Option.UPDATE_STATE, int.Parse(args[0]));
    return true;
  }

  private bool OnProcessCheat_autoexportgamestate(string func, string[] args, string rawArgs)
  {
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
      return false;
    string str = string.IsNullOrEmpty(args[0]) ? "GameStateExportFile" : args[0];
    JsonNode jsonNode1 = new JsonNode();
    foreach (KeyValuePair<int, Player> player in GameState.Get().GetPlayerMap())
    {
      string key = "Player" + (object) player.Key;
      JsonNode jsonNode2 = new JsonNode();
      jsonNode1.Add(key, (object) jsonNode2);
      jsonNode2["Hero"] = (object) this.GetCardJson(player.Value.GetHero());
      jsonNode2["HeroPower"] = (object) this.GetCardJson(player.Value.GetHeroPower());
      if (player.Value.HasWeapon())
        jsonNode2["Weapon"] = (object) this.GetCardJson(player.Value.GetWeaponCard().GetEntity());
      jsonNode2["CardsInBattlefield"] = (object) this.GetCardlistJson(player.Value.GetBattlefieldZone().GetCards());
      if (player.Value.GetSide() == Player.Side.FRIENDLY)
      {
        jsonNode2["CardsInHand"] = (object) this.GetCardlistJson(player.Value.GetHandZone().GetCards());
        jsonNode2["ActiveSecrets"] = (object) this.GetCardlistJson(player.Value.GetSecretZone().GetCards());
      }
    }
    System.IO.File.WriteAllText(string.Format("{0}\\{1}.json", (object) Environment.GetFolderPath(Environment.SpecialFolder.Desktop), (object) str), Json.Serialize((object) jsonNode1));
    return true;
  }

  private bool OnProcessCheat_social(string func, string[] args, string rawArgs)
  {
    List<BnetPlayer> friends = BnetFriendMgr.Get().GetFriends();
    List<BnetPlayer> recentPlayers = BnetRecentPlayerMgr.Get().GetRecentPlayers();
    List<BnetPlayer> nearbyPlayers = BnetNearbyPlayerMgr.Get().GetNearbyPlayers();
    List<BnetPlayer> fullPatronList = FiresideGatheringManager.Get().FullPatronList;
    friends.Sort(new Comparison<BnetPlayer>(FriendUtils.FriendSortCompare));
    recentPlayers.Sort(new Comparison<BnetPlayer>(FriendUtils.RecentFriendSortCompare));
    nearbyPlayers.Sort(new Comparison<BnetPlayer>(FriendUtils.FriendSortCompare));
    fullPatronList.Sort(new Comparison<BnetPlayer>(FriendUtils.FriendSortCompare));
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    bool flag4 = false;
    bool printFullPresence = false;
    string str1 = "USAGE: social [cmd] [args]\nCommands: help, list";
    float delay = 5f;
    string str2 = args == null || args.Length == 0 ? "list" : args[0];
    string message = (string) null;
    if (!(str2 == "help"))
    {
      if (str2 == "list")
      {
        if (args.Length >= 2 && args[1] == "help")
        {
          message = "Lists all players in the various social lists. Can specific specific lists: friend, nearby, fsg|patron";
        }
        else
        {
          for (int index = 1; index < args.Length; ++index)
          {
            switch (args[index] == null ? "" : args[index].ToLower())
            {
              case "all":
              case "full":
              case "presence":
                printFullPresence = true;
                break;
              case "fireside":
              case "firesidegathering":
              case "fsg":
              case "patron":
              case "patronlist":
              case "patrons":
                flag4 = true;
                break;
              case "friend":
              case "friends":
                flag1 = true;
                break;
              case "local":
              case "localplayer":
              case "localplayers":
              case "nearby":
              case "nearbyplayer":
              case "nearbyplayers":
              case "subnet":
                flag3 = true;
                break;
              case "recent":
              case "recentplayer":
              case "recentplayers":
                flag2 = true;
                break;
            }
          }
          if (!flag1 && !flag2 && !flag3 && !flag4)
          {
            int num;
            flag4 = (num = 1) != 0;
            flag3 = num != 0;
            flag2 = num != 0;
            flag1 = num != 0;
          }
          Log.Presence.PrintInfo("Cheat: print social list executed.");
          if (flag4)
          {
            FSGConfig currentFsg = FiresideGatheringManager.Get().CurrentFSG;
            if (currentFsg == null)
              Log.Presence.PrintInfo("FSG patrons: not checked in.");
            else
              Log.Presence.PrintInfo("FSG {0}-{1} patrons: {2}", (object) currentFsg.FsgId, (object) currentFsg.TavernName, (object) fullPatronList.Count);
            foreach (BnetPlayer player in fullPatronList)
              Cheats.OnProcessCheat_social_PrintPlayer(printFullPresence, player);
          }
          if (flag1)
          {
            Log.Presence.PrintInfo("Friends: {0}", (object) friends.Count);
            foreach (BnetPlayer player in friends)
              Cheats.OnProcessCheat_social_PrintPlayer(printFullPresence, player);
          }
          if (flag2)
          {
            Log.Presence.PrintInfo("Recent Players: {0}", (object) recentPlayers.Count);
            foreach (BnetPlayer player in recentPlayers)
              Cheats.OnProcessCheat_social_PrintPlayer(printFullPresence, player);
          }
          if (flag3)
          {
            Log.Presence.PrintInfo("Nearby Players: {0}", (object) nearbyPlayers.Count);
            foreach (BnetPlayer player in nearbyPlayers)
              Cheats.OnProcessCheat_social_PrintPlayer(printFullPresence, player);
          }
          message = "Printed to Presence Log.";
        }
      }
    }
    else
      message = str1;
    if (message != null)
      UIStatus.Get().AddInfo(message, delay);
    return true;
  }

  private bool OnProcessCheat_playStartEmote(string func, string[] args, string rawArgs)
  {
    Gameplay gameplay = Gameplay.Get();
    if ((UnityEngine.Object) gameplay == (UnityEngine.Object) null)
      return false;
    gameplay.StartCoroutine(this.PlayStartingTaunts());
    return true;
  }

  private bool OnProcessCheat_getBattlegroundHeroArmorTierList(
    string func,
    string[] args,
    string rawArgs)
  {
    Network.Get().UpdateBattlegroundHeroArmorTierList();
    GameState gameState = GameState.Get();
    if (gameState == null)
      return false;
    gameState.SetPrintBattlegroundHeroArmorTierListOnUpdate(true);
    return true;
  }

  private bool OnProcessCheat_SetBattlegroundHeroBuddyProgress(
    string func,
    string[] args,
    string rawArgs)
  {
    int result1 = 0;
    if (args.Length >= 1 && !int.TryParse(args[0], out result1))
    {
      Log.Gameplay.PrintError("[OnProcessCheat_SetBattlegroundHeroBuddyProgress] Unable to parse buddy progress " + args[0]);
      return false;
    }
    int result2 = 0;
    if (args.Length >= 2 && !int.TryParse(args[1], out result2))
      result2 = 0;
    Network.Get().SetBattlegroundHeroBuddyProgress(result1, result2);
    return true;
  }

  private bool OnProcessCheat_EnableBattlegroundHeroBuddy(
    string func,
    string[] args,
    string rawArgs)
  {
    int result = 0;
    if (!int.TryParse(args[0], out result))
      return false;
    bool flag = result != 0;
    int num = this.m_battlegroundHeroBuddyEnabled != flag ? 1 : 0;
    this.m_battlegroundHeroBuddyEnabled = flag;
    if (num != 0)
      PlayerLeaderboardManager.Get().NotifyBattlegroundHeroBuddyEnabledDirty();
    return true;
  }

  private bool OnProcessCheat_ReplaceBattlegroundHero(string func, string[] args, string rawArgs)
  {
    int result1 = 0;
    if (!int.TryParse(args[0], out result1))
    {
      Log.Gameplay.PrintError("[OnProcessCheat_ReplaceBattlegroundHero] Unable to parse new hero " + args[0]);
      return false;
    }
    int result2 = 0;
    if (!int.TryParse(args[1], out result2))
      result2 = 0;
    Network.Get().ReplaceBattlegroundHero(result1, result2);
    return true;
  }

  private bool OnProcessCheat_SetBattlegroundsLuckyDrawEndTime(
    string func,
    string[] args,
    string rawArgs)
  {
    return false;
  }

  private bool OnProcessCheat_BattlegroundsBoardFSMManipulate(
    string func,
    string[] args,
    string rawArgs)
  {
    BaconBoard baconBoard = BaconBoard.Get();
    if ((UnityEngine.Object) baconBoard == (UnityEngine.Object) null)
    {
      Log.Gameplay.PrintError("BaconBoard not available");
      return false;
    }
    string str1 = args[0];
    switch (str1)
    {
      case "adddefeatedminion":
        if (args.Length < 2)
        {
          Log.Gameplay.PrintError("[OnProcessCheat_BattlegroundsBoardFSMManipulate] No minion CardId provided");
          break;
        }
        if (GameUtils.GetCardRecord(args[1]) == null)
        {
          Log.Gameplay.PrintError("[OnProcessCheat_BattlegroundsBoardFSMManipulate] Provided invalid CardId");
          return false;
        }
        baconBoard.CheatAddDefeatedMinion(args[1]);
        break;
      case "setdefeatedminioncount":
        int result1 = 0;
        if (args.Length < 2 || !int.TryParse(args[1], out result1))
        {
          Log.Gameplay.PrintError("[OnProcessCheat_BattlegroundsBoardFSMManipulate] Unable to parse count argument");
          return false;
        }
        baconBoard.CheatSetDefeatedMinionCount(result1);
        break;
      case "setopponentdefeated":
        baconBoard.CheatSetHasDefeatedOpponent();
        break;
      case "setracedefeated":
        int result2 = 0;
        if (args.Length < 2 || !int.TryParse(args[1], out result2))
        {
          Log.Gameplay.PrintError("[OnProcessCheat_BattlegroundsBoardFSMManipulate] Unable to parse race enum number");
          return false;
        }
        if (!System.Enum.IsDefined(typeof (TAG_RACE), (object) result2))
        {
          Log.Gameplay.PrintError("[OnProcessCheat_BattlegroundsBoardFSMManipulate] " + args[1] + " does not correspond to a valid race");
          return false;
        }
        baconBoard.CheatAddDefeatedRace((TAG_RACE) result2);
        break;
      case "setwinstreak":
        int result3 = 0;
        if (args.Length < 2 || !int.TryParse(args[1], out result3))
        {
          Log.Gameplay.PrintError("[OnProcessCheat_BattlegroundsBoardFSMManipulate] Unable to parse new streak");
          return false;
        }
        baconBoard.CheatSetWinstreak(result3);
        break;
      case "triggerall":
        if (args.Length > 1 && args[1].Contains("finisher"))
        {
          string str2 = "finisher player ";
          string inputCommand = (args.Length <= 2 || !args[2].Contains("id=") ? str2 + "id=2" : str2 + args[2]) + " large";
          CheatMgr.Get().ProcessCheat(inputCommand, true);
        }
        if (!baconBoard.CheatTriggerAllBoardEffects())
        {
          Log.Gameplay.PrintError("Attempted to trigger all board effects without an instance");
          return false;
        }
        break;
      case "triggerdefeatminion":
        if (args.Length < 2)
        {
          Log.Gameplay.PrintError("[OnProcessCheat_BattlegroundsBoardFSMManipulate] No minion CardId provided");
          break;
        }
        string str3 = args[1];
        if (GameUtils.GetCardRecord(str3) == null)
        {
          Log.Gameplay.PrintError("[OnProcessCheat_BattlegroundsBoardFSMManipulate] Provided invalid CardId");
          return false;
        }
        if (!baconBoard.CheatTriggerDefeatedMinion(str3))
        {
          Log.Gameplay.PrintError("Attempted to trigger board effect without an instance");
          return false;
        }
        break;
      case "triggerdefeatopponenthero":
        if (!baconBoard.SetOpponentHeroDefeated())
        {
          Log.Gameplay.PrintError("Attempted to trigger board effect without an instance");
          return false;
        }
        break;
      case "triggerheavyhit":
        if (!baconBoard.CheatTriggerHeroHeavyHitEffects())
        {
          Log.Gameplay.PrintError("Attempted to trigger board effect without an instance");
          return false;
        }
        break;
      case "triggerminionheavyhit":
        if (!baconBoard.CheatTriggerMinionHeavyHitEffects())
        {
          Log.Gameplay.PrintError("Attempted to trigger board effect without an instance");
          return false;
        }
        break;
      default:
        UIStatus.Get().AddError(str1 + " is not a valid command");
        break;
    }
    return true;
  }

  private bool OnProcessCheat_SetBattlegroundHeroBuddyGained(
    string func,
    string[] args,
    string rawArgs)
  {
    int result1 = 0;
    if (!int.TryParse(args[0], out result1))
    {
      Log.Gameplay.PrintError("[OnProcessCheat_SetBattlegroundHeroBuddyGained] Unable to parse buddy progress " + args[0]);
      return false;
    }
    int result2 = 0;
    if (!int.TryParse(args[1], out result2))
      result2 = 0;
    Network.Get().SetBattlegroundHeroBuddyGained(result1, result2);
    return true;
  }

  private bool OnProcessCheat_getBattlegroundDenyList(string func, string[] args, string rawArgs)
  {
    Network.Get().UpdateBattlegroundInfo();
    GameState gameState = GameState.Get();
    if (gameState == null)
      return false;
    gameState.SetPrintBattlegroundDenyListOnUpdate(true);
    return true;
  }

  private bool OnProcessCheat_getBattlegroundMinionPool(string func, string[] args, string rawArgs)
  {
    Network.Get().UpdateBattlegroundInfo();
    GameState gameState = GameState.Get();
    if (gameState == null)
      return false;
    gameState.SetPrintBattlegroundMinionPoolOnUpdate(true);
    return true;
  }

  private IEnumerator PlayStartingTaunts() => EmoteHandler.Get().PlayStartingTaunts((GameObject) null);

  private static void OnProcessCheat_social_PrintPlayer(bool printFullPresence, BnetPlayer player)
  {
    string str1 = player == null ? "<null>" : (printFullPresence ? player.FullPresenceSummary : player.ShortSummary);
    SortedList<string, bool> sortedList = new SortedList<string, bool>();
    if (FiresideGatheringManager.Get().IsPlayerInMyFSG(player))
      sortedList["fsg"] = true;
    if (BnetRecentPlayerMgr.Get().IsRecentPlayer(player))
      sortedList["recent"] = true;
    if (BnetNearbyPlayerMgr.Get().IsNearbyPlayer(player))
      sortedList["nearby"] = true;
    if (BnetFriendMgr.Get().IsFriend(player))
      sortedList["friend"] = true;
    string str2 = string.Join(", ", sortedList.Keys.ToArray<string>());
    if (!string.IsNullOrEmpty(str2))
      str2 = string.Format("[{0}]", (object) str2);
    Log.Presence.PrintInfo("    {0} {1}", (object) str1, (object) str2);
  }

  private bool OnProcessCheat_OpponentName(string func, string[] args, string rawArgs)
  {
    Gameplay gameplay = Gameplay.Get();
    if ((UnityEngine.Object) gameplay == (UnityEngine.Object) null)
      return false;
    NameBanner nameBannerForSide = gameplay.GetNameBannerForSide(Player.Side.OPPOSING);
    if ((UnityEngine.Object) nameBannerForSide == (UnityEngine.Object) null)
      return false;
    nameBannerForSide.m_playerName.Text = args[0];
    return true;
  }

  private bool OnProcessCheat_friendlist(string func, string[] args, string rawArgs)
  {
    string message1 = "USAGE: flist [cmd] [args]\nCommands: fill, add, remove";
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      UIStatus.Get().AddInfo(message1, 10f);
      return true;
    }
    float delay = 5f;
    string message2 = (string) null;
    string str1 = args[0];
    if (!(str1 == "fill"))
    {
      if (!(str1 == "add"))
      {
        if (str1 == "remove")
        {
          BnetRecentPlayerMgr.Get().Cheat_RemoveCheatFriends();
          BnetNearbyPlayerMgr.Get().Cheat_RemoveCheatFriends();
          FiresideGatheringManager.Get().Cheat_RemoveCheatFriends();
          BnetFriendMgr.Get().Cheat_RemoveCheatFriends();
          message2 = string.Format("Removed cheat friends");
        }
      }
      else
      {
        int result = 1;
        string str2 = "Player";
        Cheats.FriendListType type = Cheats.FriendListType.FRIEND;
        int season = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>().Season;
        int leagueId = RankMgr.Get().GetLeagueRecordForType(League.LeagueType.NORMAL, season).ID;
        int starLevel = 1;
        BnetProgramId programID = BnetProgramId.HEARTHSTONE;
        bool boolVal1 = true;
        bool boolVal2 = true;
        bool boolVal3 = false;
        foreach (string str3 in args)
        {
          string[] strArray1;
          if (str3 != null)
            strArray1 = str3.Split('=');
          else
            strArray1 = (string[]) null;
          string[] strArray2 = strArray1;
          if (strArray2 != null && strArray2.Length >= 2)
          {
            if (strArray2[0].Equals("num", StringComparison.InvariantCultureIgnoreCase))
            {
              int.TryParse(strArray2[1], out result);
              if (result < 1)
                result = 1;
            }
            else if (strArray2[0].Equals("name", StringComparison.InvariantCultureIgnoreCase))
              str2 = strArray2[1];
            else if (strArray2[0].Equals("type", StringComparison.InvariantCultureIgnoreCase))
            {
              string str4 = strArray2[1];
              if (!string.IsNullOrEmpty(str4))
                type = Blizzard.T5.Core.Utils.EnumUtils.SafeParse<Cheats.FriendListType>(str4, ignoreCase: true);
            }
            else if (strArray2[0].Equals("rank", StringComparison.InvariantCultureIgnoreCase))
            {
              LeagueRankDbfRecord recordByCheatName = RankMgr.Get().GetLeagueRankRecordByCheatName(strArray2[1]);
              if (recordByCheatName != null)
              {
                leagueId = recordByCheatName.LeagueId;
                starLevel = recordByCheatName.StarLevel;
              }
            }
            else if (strArray2[0].Equals("program", StringComparison.InvariantCultureIgnoreCase))
            {
              string stringVal = strArray2[1];
              if (!string.IsNullOrEmpty(stringVal))
              {
                programID = new BnetProgramId(stringVal);
                leagueId = 0;
                starLevel = 0;
              }
            }
            else if (strArray2[0].Equals("friend", StringComparison.InvariantCultureIgnoreCase))
              GeneralUtils.TryParseBool(strArray2[1], out boolVal1);
            else if (strArray2[0].Equals("online", StringComparison.InvariantCultureIgnoreCase))
              GeneralUtils.TryParseBool(strArray2[1], out boolVal2);
            else if (strArray2[0].Equals("away", StringComparison.InvariantCultureIgnoreCase))
              GeneralUtils.TryParseBool(strArray2[1], out boolVal3);
          }
        }
        for (int index = 0; index < result; ++index)
          this.CreateCheatFriendlistItem(str2 + (object) index, type, leagueId, starLevel, programID, boolVal1, boolVal2, boolVal3);
        message2 = string.Format("Created {0} players", (object) result);
      }
    }
    else
    {
      int season = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>().Season;
      int id = RankMgr.Get().GetLeagueRecordForType(League.LeagueType.NORMAL, season).ID;
      int maxStarLevel = RankMgr.Get().GetMaxStarLevel(id);
      foreach (Cheats.FriendListType type in System.Enum.GetValues(typeof (Cheats.FriendListType)))
      {
        for (int starLevel = 1; starLevel < maxStarLevel; ++starLevel)
          this.CreateCheatFriendlistItem(string.Format("{0} friend{1}", (object) type, (object) starLevel), type, id, starLevel, BnetProgramId.HEARTHSTONE, true, true, false);
      }
      message2 = string.Format("Filled friend list");
    }
    BnetBarFriendButton.Get().UpdateOnlineCount();
    if (message2 != null)
      UIStatus.Get().AddInfo(message2, delay);
    return true;
  }

  private bool OnProcessCheat_SetGameSaveData(string func, string[] args, string rawArgs)
  {
    GameSaveKeyId key = ~GameSaveKeyId.INVALID;
    GameSaveKeySubkeyId subkey = ~GameSaveKeySubkeyId.INVALID;
    if (!this.ValidateAndParseGameSaveDataKeyAndSubkey(args, out key, out subkey))
    {
      UIStatus.Get().AddError("You must provide valid key and subkeys!");
      return true;
    }
    long num = 0;
    int index = 2;
    string str = string.Empty;
    List<long> longList = new List<long>();
    for (; index < ((IEnumerable<string>) args).Count<string>(); ++index)
    {
      if (!this.ValidateAndParseLongAtIndex(index, args, out num))
      {
        num = (long) GameUtils.TranslateCardIdToDbId(args[index], true);
        if (num == 0L)
          break;
      }
      longList.Add(num);
      str = str + (object) num + ";";
    }
    args = new string[4]
    {
      "setgsd",
      "key=" + args[0],
      "subkey=" + args[1],
      "values=" + str
    };
    GameSaveDataManager.Get().Cheat_SaveSubkeyToLocalCache(key, subkey, longList.ToArray());
    UIStatus.Get().AddInfo(string.Format("Set key {0} subkey {1} to {2}", (object) key, (object) subkey, (object) str));
    return this.OnProcessCheat_utilservercmd("util", args, rawArgs, (AutofillData) null);
  }

  private bool OnProcessCheat_ShowTip(string func, string[] args, string rawArgs)
  {
    TipCategory tipCategory = TipCategory.DEFAULT;
    int? tipIndex = new int?();
    if (args[0].Length > 0)
    {
      switch (args[0].ToUpper())
      {
        case "ADVENTURE":
          tipCategory = TipCategory.ADVENTURE;
          break;
        case "BACON":
          tipCategory = TipCategory.BACON;
          break;
        case "DEFAULT":
          tipCategory = TipCategory.DEFAULT;
          break;
        case "DUELS":
          tipCategory = TipCategory.DUELS;
          break;
        case "FORGE":
          tipCategory = TipCategory.FORGE;
          break;
        case "HEROICBRAWL":
          tipCategory = TipCategory.HEROICBRAWL;
          break;
        case "LETTUCE":
          tipCategory = TipCategory.LETTUCE;
          break;
        case "PLAY":
          tipCategory = TipCategory.PLAY;
          break;
        case "PRACTICE":
          tipCategory = TipCategory.PRACTICE;
          break;
        case "QUEST_LOG":
          tipCategory = TipCategory.QUEST_LOG;
          break;
        case "QUEST_LOG_RANDOM":
          tipCategory = TipCategory.QUEST_LOG_RANDOM;
          break;
        case "TAVERNBRAWL":
          tipCategory = TipCategory.TAVERNBRAWL;
          break;
        default:
          UIStatus.Get().AddInfo("Valid Categories: INVALID, PRACTICE, PLAY, FORGE, DEFAULT, QUEST_LOG, QUEST_LOG_RANDOM, ADVENTURE, TAVERNBRAWL, HEROICBRAWL, BACON, DUELS, LETTUCE");
          return true;
      }
      int result;
      if (args.Length == 2 && int.TryParse(args[1], out result))
        tipIndex = new int?(result);
    }
    UIStatus.Get().AddInfo(GameStrings.GetTip(tipCategory, tipIndex));
    return true;
  }

  private bool OnProcessCheat_SetDungeonRunProgress(string func, string[] args, string rawArgs) => this.ParseAdventureThenSetProgress(args, Cheats.SetAdventureProgressMode.Progress);

  private bool OnProcessCheat_SetDungeonRunVictory(string func, string[] args, string rawArgs) => this.ParseAdventureThenSetProgress(args, Cheats.SetAdventureProgressMode.Victory);

  private bool OnProcessCheat_SetDungeonRunDefeat(string func, string[] args, string rawArgs) => this.ParseAdventureThenSetProgress(args, Cheats.SetAdventureProgressMode.Defeat);

  private bool OnProcessCheat_ResetDungeonRunAdventure(string func, string[] args, string rawArgs)
  {
    AdventureDbId adventureDbIdFromArgs = Cheats.ParseAdventureDbIdFromArgs(args, 0);
    return adventureDbIdFromArgs == AdventureDbId.INVALID || this.ResetDungeonRunAdventure(adventureDbIdFromArgs, AdventureModeDbId.DUNGEON_CRAWL);
  }

  private bool ResetDungeonRunAdventure(AdventureDbId adventure, AdventureModeDbId mode)
  {
    if (adventure == AdventureDbId.INVALID)
      return true;
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) adventure, (int) mode);
    if (adventureDataRecord == null)
    {
      UIStatus.Get().AddError(string.Format("No Adventure data found for Adventure {0} Mode {1}", (object) adventure, (object) mode));
      return true;
    }
    if (adventureDataRecord.GameSaveDataServerKey == 0)
    {
      UIStatus.Get().AddError(string.Format("No GameSaveDataServerKey for Adventure {0} Mode {1}!", (object) adventure, (object) mode));
      return true;
    }
    this.ResetAdventureRunCommon_Server(adventureDataRecord.GameSaveDataServerKey);
    if (adventureDataRecord.GameSaveDataClientKey != 0)
      this.ResetAdventureRunCommon_Client(adventureDataRecord.GameSaveDataClientKey);
    UIStatus.Get().AddInfo(string.Format("Reset current run for Adventure {0} Mode {1}", (object) adventure, (object) mode));
    return true;
  }

  private bool OnProcessCheat_ResetDungeonRun_VO(string func, string[] args, string rawArgs)
  {
    AdventureDbId adventureDbIdFromArgs = Cheats.ParseAdventureDbIdFromArgs(args, 0);
    if (adventureDbIdFromArgs == AdventureDbId.INVALID)
      return true;
    long subkeyValue = 0;
    this.ValidateAndParseLongAtIndex(1, args, out subkeyValue);
    return this.ResetDungeonRun_VO(adventureDbIdFromArgs, subkeyValue);
  }

  private bool ResetDungeonRun_VO(AdventureDbId adventure, long subkeyValue)
  {
    AdventureDungeonCrawlDisplay.s_shouldShowWelcomeBanner = true;
    switch (adventure)
    {
      case AdventureDbId.LOOT:
        Options.Get().SetBool(Option.HAS_JUST_SEEN_LOOT_NO_TAKE_CANDLE_VO, false);
        break;
      case AdventureDbId.GIL:
        Options.Get().SetBool(Option.HAS_SEEN_PLAYED_TESS, false);
        Options.Get().SetBool(Option.HAS_SEEN_PLAYED_DARIUS, false);
        Options.Get().SetBool(Option.HAS_SEEN_PLAYED_SHAW, false);
        Options.Get().SetBool(Option.HAS_SEEN_PLAYED_TOKI, false);
        break;
    }
    AdventureModeDbId modeId = AdventureModeDbId.DUNGEON_CRAWL;
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) adventure, (int) modeId);
    if (adventureDataRecord == null)
    {
      UIStatus.Get().AddError(string.Format("No Adventure data found for Adventure {0} Mode {1}", (object) adventure, (object) modeId));
      return true;
    }
    if (adventureDataRecord.GameSaveDataClientKey == 0)
    {
      UIStatus.Get().AddError(string.Format("No GameSaveDataClientKey for Adventure {0} Mode {1}!", (object) adventure, (object) modeId));
      return true;
    }
    this.ResetVOSubkeysForAdventure((GameSaveKeyId) adventureDataRecord.GameSaveDataClientKey, subkeyValue);
    if (adventureDataRecord.GameSaveDataServerKey != 0)
      this.ResetVOSubkeysForAdventure((GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey, subkeyValue);
    UIStatus.Get().AddInfo(string.Format("You can now see all {0} VO again.", (object) adventure));
    return true;
  }

  private bool ParseAdventureThenSetProgress(
    string[] args,
    Cheats.SetAdventureProgressMode progressMode)
  {
    AdventureDbId adventureDbIdFromArgs = Cheats.ParseAdventureDbIdFromArgs(args, 0);
    if (adventureDbIdFromArgs == AdventureDbId.INVALID)
      return true;
    string[] strArray = new string[args.Length - 1];
    Array.Copy((Array) args, 1, (Array) strArray, 0, args.Length - 1);
    if (this.SetAdventureProgressCommon(adventureDbIdFromArgs, AdventureModeDbId.DUNGEON_CRAWL, strArray, progressMode))
      UIStatus.Get().AddInfo(string.Format("Set Dungeon Run {0} for {1}", (object) progressMode, (object) adventureDbIdFromArgs));
    return true;
  }

  private bool OnProcessCheat_SetKCVictory(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.LOOT, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Victory))
      UIStatus.Get().AddInfo(string.Format("Set KC victory"));
    return true;
  }

  private bool OnProcessCheat_SetKCProgress(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.LOOT, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Progress))
      UIStatus.Get().AddInfo(string.Format("Set KC progress"));
    return true;
  }

  private bool OnProcessCheat_SetKCDefeat(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.LOOT, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Defeat))
      UIStatus.Get().AddInfo(string.Format("Set KC defeat"));
    return true;
  }

  private bool OnProcessCheat_SetGILVictory(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.GIL, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Victory))
      UIStatus.Get().AddInfo(string.Format("Set Witchwood victory"));
    return true;
  }

  private bool OnProcessCheat_SetGILProgress(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.GIL, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Progress))
      UIStatus.Get().AddInfo(string.Format("Set Witchwood progress"));
    return true;
  }

  private bool OnProcessCheat_SetGILDefeat(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.GIL, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Defeat))
      UIStatus.Get().AddInfo(string.Format("Set Witchwood defeat"));
    return true;
  }

  private bool OnProcessCheat_SetGILBonus(string func, string[] args, string rawArgs)
  {
    this.OnProcessCheat_utilservercmd("util", new string[4]
    {
      "quest",
      "progress",
      "achieve=1010",
      "amount=4"
    }, "util quest progress achieve=1010 amount=4", (AutofillData) null);
    UIStatus.Get().AddInfo(string.Format("Set Witchwood Bonus Challenge Active"));
    Options.Get().SetBool(Option.HAS_SEEN_GIL_BONUS_CHALLENGE, false);
    return true;
  }

  private bool OnProcessCheat_SetTRLVictory(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.TRL, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Victory))
      UIStatus.Get().AddInfo(string.Format("Set Rastakhan's Rumble victory"));
    return true;
  }

  private bool OnProcessCheat_SetTRLProgress(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.TRL, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Progress))
      UIStatus.Get().AddInfo(string.Format("Set Rastakhan's Rumble progress"));
    return true;
  }

  private bool OnProcessCheat_SetTRLDefeat(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.TRL, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Defeat))
      UIStatus.Get().AddInfo(string.Format("Set Rastakhan's Rumble defeat"));
    return true;
  }

  private bool OnProcessCheat_SetDALProgress(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.DALARAN, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Progress))
      UIStatus.Get().AddInfo(string.Format("Set Dalaran progress"));
    return true;
  }

  private bool OnProcessCheat_SetDALVictory(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.DALARAN, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Victory))
      UIStatus.Get().AddInfo(string.Format("Set Dalaran victory"));
    return true;
  }

  private bool OnProcessCheat_SetDALDefeat(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.DALARAN, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Defeat))
      UIStatus.Get().AddInfo(string.Format("Set Dalaran defeat"));
    return true;
  }

  private bool OnProcessCheat_ResetDalaranAdventure(string func, string[] args, string rawArgs) => this.ResetDungeonRunAdventure(AdventureDbId.DALARAN, AdventureModeDbId.DUNGEON_CRAWL);

  private bool OnProcessCheat_ResetTavernBrawlAdventure(string func, string[] args, string rawArgs)
  {
    if (TavernBrawlManager.Get() == null)
    {
      UIStatus.Get().AddError("TavernBrawlManager is not initialized!");
      return true;
    }
    TavernBrawlMission mission = TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
    if (mission == null)
    {
      UIStatus.Get().AddError("No Tavern Brawl Mission found");
      return true;
    }
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(mission.missionId);
    if (record == null)
    {
      UIStatus.Get().AddError("Could not find scenario for current tavern brawl mission");
      return true;
    }
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord(record.AdventureId, record.ModeId);
    if (adventureDataRecord == null)
    {
      UIStatus.Get().AddError("Could not find adventure data for current tavern brawl mission");
      return true;
    }
    this.ResetAdventureRunCommon_Server(adventureDataRecord.GameSaveDataServerKey);
    this.ResetAdventureRunCommon_Client(adventureDataRecord.GameSaveDataClientKey);
    UIStatus.Get().AddInfo(string.Format("Reset Tavern Brawl Adventure Progress"));
    return true;
  }

  private bool OnProcessCheat_SetDALHeroicProgress(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.DALARAN, AdventureModeDbId.DUNGEON_CRAWL_HEROIC, args, Cheats.SetAdventureProgressMode.Progress))
      UIStatus.Get().AddInfo(string.Format("Set Dalaran Heroic progress"));
    return true;
  }

  private bool OnProcessCheat_SetDALHeroicVictory(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.DALARAN, AdventureModeDbId.DUNGEON_CRAWL_HEROIC, args, Cheats.SetAdventureProgressMode.Victory))
      UIStatus.Get().AddInfo(string.Format("Set Dalaran Heroic victory"));
    return true;
  }

  private bool OnProcessCheat_SetDALHeroicDefeat(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.DALARAN, AdventureModeDbId.DUNGEON_CRAWL_HEROIC, args, Cheats.SetAdventureProgressMode.Defeat))
      UIStatus.Get().AddInfo(string.Format("Set Dalaran Heroic defeat"));
    return true;
  }

  private bool OnProcessCheat_ResetDalaranHeroicAdventure(
    string func,
    string[] args,
    string rawArgs)
  {
    return this.ResetDungeonRunAdventure(AdventureDbId.DALARAN, AdventureModeDbId.DUNGEON_CRAWL_HEROIC);
  }

  private bool OnProcessCheat_SetULDProgress(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.ULDUM, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Progress))
      UIStatus.Get().AddInfo(string.Format("Set Uldum progress"));
    return true;
  }

  private bool OnProcessCheat_SetULDVictory(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.ULDUM, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Victory))
      UIStatus.Get().AddInfo(string.Format("Set Uldum victory"));
    return true;
  }

  private bool OnProcessCheat_SetULDDefeat(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.ULDUM, AdventureModeDbId.DUNGEON_CRAWL, args, Cheats.SetAdventureProgressMode.Defeat))
      UIStatus.Get().AddInfo(string.Format("Set Uldum defeat"));
    return true;
  }

  private bool OnProcessCheat_ResetUldumAdventure(string func, string[] args, string rawArgs) => this.ResetDungeonRunAdventure(AdventureDbId.ULDUM, AdventureModeDbId.DUNGEON_CRAWL);

  private bool OnProcessCheat_SetULDHeroicProgress(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.ULDUM, AdventureModeDbId.DUNGEON_CRAWL_HEROIC, args, Cheats.SetAdventureProgressMode.Progress))
      UIStatus.Get().AddInfo(string.Format("Set Uldum Heroic progress"));
    return true;
  }

  private bool OnProcessCheat_SetULDHeroicVictory(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.ULDUM, AdventureModeDbId.DUNGEON_CRAWL_HEROIC, args, Cheats.SetAdventureProgressMode.Victory))
      UIStatus.Get().AddInfo(string.Format("Set Uldum Heroic victory"));
    return true;
  }

  private bool OnProcessCheat_SetULDHeroicDefeat(string func, string[] args, string rawArgs)
  {
    if (this.SetAdventureProgressCommon(AdventureDbId.ULDUM, AdventureModeDbId.DUNGEON_CRAWL_HEROIC, args, Cheats.SetAdventureProgressMode.Defeat))
      UIStatus.Get().AddInfo(string.Format("Set Uldum Heroic defeat"));
    return true;
  }

  private bool OnProcessCheat_ResetUldumHeroicAdventure(string func, string[] args, string rawArgs) => this.ResetDungeonRunAdventure(AdventureDbId.ULDUM, AdventureModeDbId.DUNGEON_CRAWL_HEROIC);

  private bool OnProcessCheat_ResetGILAdventure(string func, string[] args, string rawArgs) => this.ResetDungeonRunAdventure(AdventureDbId.GIL, AdventureModeDbId.DUNGEON_CRAWL);

  private static AdventureDbId ParseAdventureDbIdFromArgs(string[] args, int index)
  {
    AdventureDbId adventureDbIdFromArgs = AdventureDbId.INVALID;
    if (args.Length <= index || string.IsNullOrEmpty(args[index]))
    {
      UIStatus.Get().AddError("You must provide an Adventure to operate on!  Ex: 'uld'");
      return adventureDbIdFromArgs;
    }
    AdventureDbId adventureDbIdFromString = Cheats.GetAdventureDbIdFromString(args[index]);
    if (adventureDbIdFromString != AdventureDbId.INVALID)
      return adventureDbIdFromString;
    UIStatus.Get().AddError(string.Format("{0} does not map to a valid Adventure!", (object) args[index]));
    return adventureDbIdFromString;
  }

  private static AdventureDbId GetAdventureDbIdFromString(string adventureString)
  {
    if (string.IsNullOrEmpty(adventureString))
      return AdventureDbId.INVALID;
    AdventureDbId adventureDbIdFromString = AdventureDbId.INVALID;
    try
    {
      adventureDbIdFromString = (AdventureDbId) System.Enum.Parse(typeof (AdventureDbId), adventureString, true);
    }
    catch (ArgumentException ex)
    {
    }
    if (adventureDbIdFromString != AdventureDbId.INVALID)
      return adventureDbIdFromString;
    switch (adventureString.ToLower())
    {
      case "dal":
        return AdventureDbId.DALARAN;
      case "drg":
      case "ga":
        return AdventureDbId.DRAGONS;
      case "icecrown":
        return AdventureDbId.ICC;
      case "k&c":
      case "kc":
      case "knc":
        return AdventureDbId.LOOT;
      case "karazhan":
        return AdventureDbId.KARA;
      case "league":
        return AdventureDbId.LOE;
      case "nax":
      case "naxx":
        return AdventureDbId.NAXXRAMAS;
      case "rastakhan":
        return AdventureDbId.TRL;
      case "tot":
      case "uld":
        return AdventureDbId.ULDUM;
      case "witchwood":
        return AdventureDbId.GIL;
      default:
        return AdventureDbId.INVALID;
    }
  }

  private bool OnProcessCheat_UnlockLoadout(string func, string[] args, string rawArgs) => this.UpdateAdventureLoadoutOptionsLockStateFromArgs(args, false);

  private bool OnProcessCheat_LockLoadout(string func, string[] args, string rawArgs) => this.UpdateAdventureLoadoutOptionsLockStateFromArgs(args, true);

  private bool OnProcessCheat_ShowAdventureLoadingPopup(string func, string[] args, string rawArgs)
  {
    GameMgr.Get().Cheat_ShowTransitionPopup(GameType.GT_VS_AI, PegasusShared.FormatType.FT_WILD, (int) AdventureConfig.Get().GetMission());
    if (AdventureConfig.Get().GetMission() == ScenarioDbId.INVALID)
      UIStatus.Get().AddInfo("Showing generic popup, navigate to an Adventure scenario to customize the popup");
    else
      UIStatus.Get().AddInfo(string.Format("Showing loading popup for scenario {0}", (object) (int) AdventureConfig.Get().GetMission()));
    return true;
  }

  private bool OnProcessCheat_HideGameTransitionPopup(string func, string[] args, string rawArgs)
  {
    GameMgr.Get().HideTransitionPopup();
    UIStatus.Get().AddInfo("Hiding Transition Popup");
    return true;
  }

  private static GameSaveKeyId GetGameSaveServerKeyForAdventure(
    AdventureDbId adventureDbId,
    AdventureModeDbId adventureMode)
  {
    AdventureDataDbfRecord record = GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureDbId && (AdventureModeDbId) r.ModeId == adventureMode));
    if (record != null)
      return (GameSaveKeyId) record.GameSaveDataServerKey;
    Debug.LogErrorFormat("No AdventureDataDbfRecord found for Adventure {0} Mode {1}, unable to unlock loadout options!", (object) adventureDbId, (object) adventureMode);
    return GameSaveKeyId.INVALID;
  }

  private bool UpdateAdventureLoadoutOptionsLockStateFromArgs(string[] args, bool shouldLock)
  {
    AdventureDbId adventure = Cheats.ParseAdventureDbIdFromArgs(args, 0);
    if (adventure == AdventureDbId.INVALID)
      return true;
    GameSaveKeyId normalServerKey = Cheats.GetGameSaveServerKeyForAdventure(adventure, AdventureModeDbId.DUNGEON_CRAWL);
    if (normalServerKey == ~GameSaveKeyId.INVALID)
    {
      UIStatus.Get().AddError("No ServerKey found for Adventure " + (object) adventure + " Mode " + (object) AdventureModeDbId.DUNGEON_CRAWL + ", unable to unlock loadout options!");
      return true;
    }
    List<GameSaveKeyId> keys = new List<GameSaveKeyId>()
    {
      normalServerKey
    };
    GameSaveKeyId heroicServerKey = Cheats.GetGameSaveServerKeyForAdventure(adventure, AdventureModeDbId.DUNGEON_CRAWL_HEROIC);
    if (heroicServerKey != ~GameSaveKeyId.INVALID)
      keys.Add(heroicServerKey);
    GameSaveDataManager.Get().Request(keys, (GameSaveDataManager.OnRequestDataResponseDelegate) (success =>
    {
      this.UpdateAdventureLoadoutOptionsLockStateCommon(adventure, normalServerKey, shouldLock);
      if (heroicServerKey != ~GameSaveKeyId.INVALID)
        this.UpdateAdventureLoadoutOptionsLockStateCommon(adventure, heroicServerKey, shouldLock);
      if (!success)
        UIStatus.Get().AddInfo("Failed to request ServerKeys for Adventure " + (object) adventure + ", not all loadout options may be unlocked properly!");
      else
        UIStatus.Get().AddInfo(string.Format("{0} Loadout {1}", shouldLock ? (object) "Lock" : (object) "Unlock", (object) adventure));
    }));
    return true;
  }

  private void UpdateLockSubkey(
    GameSaveKeyId serverKey,
    GameSaveKeySubkeyId subkey,
    long unlockValue,
    bool shouldLock)
  {
    if (serverKey == ~GameSaveKeyId.INVALID || subkey == ~GameSaveKeySubkeyId.INVALID)
      return;
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(serverKey, subkey, out num);
    if (shouldLock && num != 0L)
    {
      this.InvokeSetGameSaveDataCheat(serverKey, subkey, 0L);
    }
    else
    {
      if (shouldLock || num >= unlockValue)
        return;
      this.InvokeSetGameSaveDataCheat(serverKey, subkey, unlockValue);
    }
  }

  private void UpdateAdventureLoadoutOptionsLockStateCommon(
    AdventureDbId adventureDbId,
    GameSaveKeyId serverKey,
    bool shouldLock)
  {
    foreach (AdventureHeroPowerDbfRecord record in GameDbf.AdventureHeroPower.GetRecords((Predicate<AdventureHeroPowerDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureDbId)))
      this.UpdateLockSubkey(serverKey, (GameSaveKeySubkeyId) record.UnlockGameSaveSubkey, (long) record.UnlockValue, shouldLock);
    foreach (AdventureDeckDbfRecord record in GameDbf.AdventureDeck.GetRecords((Predicate<AdventureDeckDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureDbId)))
      this.UpdateLockSubkey(serverKey, (GameSaveKeySubkeyId) record.UnlockGameSaveSubkey, (long) record.UnlockValue, shouldLock);
    foreach (AdventureLoadoutTreasuresDbfRecord record in GameDbf.AdventureLoadoutTreasures.GetRecords((Predicate<AdventureLoadoutTreasuresDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureDbId)))
    {
      this.UpdateLockSubkey(serverKey, (GameSaveKeySubkeyId) record.UnlockGameSaveSubkey, (long) record.UnlockValue, shouldLock);
      this.UpdateLockSubkey(serverKey, (GameSaveKeySubkeyId) record.UpgradeGameSaveSubkey, (long) record.UpgradeValue, shouldLock);
    }
  }

  private void ResetAdventureRunCommon_Server(int key)
  {
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_SCENARIO_ID, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_SCENARIO_ID, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_LOADOUT_TREASURE_ID, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_POWER, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_POWER, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_DECK, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_ANOMALY_MODE, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_ANOMALY_MODE, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSS_LOST_TO, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSSES_DEFEATED, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CARD_LIST, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CLASS, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSSES_FOUGHT_LIST, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEXT_BOSS_FIGHT, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_NEXT_BOSS_FIGHT_UNDEFEATED, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_A, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_B, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_C, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_TREASURE_OPTION, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_SHRINE_OPTIONS, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_HISTORY, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_RUN_ACTIVE, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_RUN_RETIRED, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_LOOT, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_TREASURE, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_SHRINE, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEXT_BOSS_HEALTH, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_HEALTH, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_EVENT_1, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_EVENT_2, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_SCENARIO_OVERRIDE, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CLASS, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_LOADOUT_TREASURE_ID, new long[0]);
    this.InvokeSetGameSaveDataCheat(key, GameSaveKeySubkeyId.DUELS_DRAFT_HERO_CHOICES, new long[0]);
  }

  private void ResetAdventureRunCommon_Client(int key)
  {
  }

  private bool SetAdventureProgressCommon(
    AdventureDbId adventureDbId,
    AdventureModeDbId adventureMode,
    string[] args,
    Cheats.SetAdventureProgressMode mode)
  {
    AdventureDataDbfRecord record1 = GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureDbId && (AdventureModeDbId) r.ModeId == adventureMode));
    if (record1 == null)
    {
      UIStatus.Get().AddError("No AdventureDataDbfRecord found for Adventure " + (object) adventureDbId + " Mode " + (object) adventureMode + ", unable to set Adventure progress!");
      return false;
    }
    long val1 = 0;
    if (mode != Cheats.SetAdventureProgressMode.Victory && !this.ValidateAndParseLongAtIndex(0, args, out val1))
    {
      UIStatus.Get().AddError("You must provide a valid number of bosses defeated!");
      return false;
    }
    GameSaveKeyId saveDataServerKey = (GameSaveKeyId) record1.GameSaveDataServerKey;
    long num1 = 0;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_RUN_ACTIVE, out num1);
    bool flag = num1 > 0L;
    if (!flag)
    {
      long num2 = 0;
      if (GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CLASS, out num2) && (int) num2 != 0)
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CLASS, new long[1]
        {
          num2
        });
    }
    long deckClass = 0;
    if (!GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CLASS, out deckClass) || (int) deckClass == 0)
    {
      deckClass = 4L;
      HashSet<TAG_CLASS> tagClassSet = new HashSet<TAG_CLASS>((IEnumerable<TAG_CLASS>) GameUtils.ORDERED_HERO_CLASSES);
      List<AdventureGuestHeroesDbfRecord> records = GameDbf.AdventureGuestHeroes.GetRecords((Predicate<AdventureGuestHeroesDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureDbId));
      GuestHeroDbfRecord guestHeroDbfRecord = (GuestHeroDbfRecord) null;
      foreach (AdventureGuestHeroesDbfRecord guestHeroesDbfRecord in records)
      {
        GuestHeroDbfRecord record2 = GameDbf.GuestHero.GetRecord(guestHeroesDbfRecord.GuestHeroId);
        if (tagClassSet.Contains(GameUtils.GetTagClassFromCardDbId(record2.CardId)))
        {
          guestHeroDbfRecord = record2;
          break;
        }
      }
      if (guestHeroDbfRecord != null)
      {
        TAG_CLASS classFromCardDbId = GameUtils.GetTagClassFromCardDbId(guestHeroDbfRecord.CardId);
        if (classFromCardDbId != TAG_CLASS.INVALID)
          deckClass = (long) classFromCardDbId;
      }
      this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CLASS, new long[1]
      {
        deckClass
      });
    }
    long missionId1;
    GameSaveDataManager.Get().GetSubkeyValue((GameSaveKeyId) record1.GameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_SCENARIO_ID, out missionId1);
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId((ScenarioDbId) missionId1);
    long missionId2 = 0;
    if (record1 != null && record1.DungeonCrawlSelectChapter)
    {
      if (!flag)
      {
        missionId2 = (long) AdventureConfig.Get().GetMission();
        if (missionId2 <= 0L)
          GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_SCENARIO_ID, out missionId2);
        if (missionId2 > 0L)
          this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_SCENARIO_ID, new long[1]
          {
            missionId2
          });
      }
    }
    else if (adventureDbId == AdventureDbId.BOH || adventureDbId == AdventureDbId.BOM)
    {
      ScenarioDbId[] scenarioDbIdArray;
      if (adventureDbId == AdventureDbId.BOH)
      {
        switch (wingIdFromMissionId)
        {
          case WingDbId.BOH_REXXAR:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOH_REXXAR_01,
              ScenarioDbId.BOH_REXXAR_02,
              ScenarioDbId.BOH_REXXAR_03,
              ScenarioDbId.BOH_REXXAR_04,
              ScenarioDbId.BOH_REXXAR_05,
              ScenarioDbId.BOH_REXXAR_06,
              ScenarioDbId.BOH_REXXAR_07,
              ScenarioDbId.BOH_REXXAR_08
            };
            break;
          case WingDbId.BOH_GARROSH:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOH_GARROSH_01,
              ScenarioDbId.BOH_GARROSH_02,
              ScenarioDbId.BOH_GARROSH_03,
              ScenarioDbId.BOH_GARROSH_04,
              ScenarioDbId.BOH_GARROSH_05,
              ScenarioDbId.BOH_GARROSH_06,
              ScenarioDbId.BOH_GARROSH_07,
              ScenarioDbId.BOH_GARROSH_08
            };
            break;
          case WingDbId.BOH_UTHER:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOH_UTHER_01,
              ScenarioDbId.BOH_UTHER_02,
              ScenarioDbId.BOH_UTHER_03,
              ScenarioDbId.BOH_UTHER_04,
              ScenarioDbId.BOH_UTHER_05,
              ScenarioDbId.BOH_UTHER_06,
              ScenarioDbId.BOH_UTHER_07,
              ScenarioDbId.BOH_UTHER_08
            };
            break;
          case WingDbId.BOH_ANDUIN:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOH_ANDUIN_01,
              ScenarioDbId.BOH_ANDUIN_02,
              ScenarioDbId.BOH_ANDUIN_03,
              ScenarioDbId.BOH_ANDUIN_04,
              ScenarioDbId.BOH_ANDUIN_05,
              ScenarioDbId.BOH_ANDUIN_06,
              ScenarioDbId.BOH_ANDUIN_07,
              ScenarioDbId.BOH_ANDUIN_08
            };
            break;
          case WingDbId.BOH_VALEERA:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOH_VALEERA_01,
              ScenarioDbId.BOH_VALEERA_02,
              ScenarioDbId.BOH_VALEERA_03,
              ScenarioDbId.BOH_VALEERA_04,
              ScenarioDbId.BOH_VALEERA_05,
              ScenarioDbId.BOH_VALEERA_06,
              ScenarioDbId.BOH_VALEERA_07,
              ScenarioDbId.BOH_VALEERA_08
            };
            break;
          case WingDbId.BOH_THRALL:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOH_THRALL_01,
              ScenarioDbId.BOH_THRALL_02,
              ScenarioDbId.BOH_THRALL_03,
              ScenarioDbId.BOH_THRALL_04,
              ScenarioDbId.BOH_THRALL_05,
              ScenarioDbId.BOH_THRALL_06,
              ScenarioDbId.BOH_THRALL_07,
              ScenarioDbId.BOH_THRALL_08
            };
            break;
          case WingDbId.BOH_MALFURION:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOH_MALFURION_01,
              ScenarioDbId.BOH_MALFURION_02,
              ScenarioDbId.BOH_MALFURION_03,
              ScenarioDbId.BOH_MALFURION_04,
              ScenarioDbId.BOH_MALFURION_05,
              ScenarioDbId.BOH_MALFURION_06,
              ScenarioDbId.BOH_MALFURION_07,
              ScenarioDbId.BOH_MALFURION_08
            };
            break;
          case WingDbId.BOH_GULDAN:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOH_GULDAN_01,
              ScenarioDbId.BOH_GULDAN_02,
              ScenarioDbId.BOH_GULDAN_03,
              ScenarioDbId.BOH_GULDAN_04,
              ScenarioDbId.BOH_GULDAN_05,
              ScenarioDbId.BOH_GULDAN_06,
              ScenarioDbId.BOH_GULDAN_07,
              ScenarioDbId.BOH_GULDAN_08
            };
            break;
          case WingDbId.BOH_ILLIDAN:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOH_ILLIDAN_01,
              ScenarioDbId.BOH_ILLIDAN_02,
              ScenarioDbId.BOH_ILLIDAN_03,
              ScenarioDbId.BOH_ILLIDAN_04,
              ScenarioDbId.BOH_ILLIDAN_05,
              ScenarioDbId.BOH_ILLIDAN_06,
              ScenarioDbId.BOH_ILLIDAN_07,
              ScenarioDbId.BOH_ILLIDAN_08
            };
            break;
          case WingDbId.BOH_FAELIN:
            scenarioDbIdArray = new ScenarioDbId[19]
            {
              ScenarioDbId.BOH_FAELIN_01,
              ScenarioDbId.BOH_FAELIN_02,
              ScenarioDbId.BOH_FAELIN_03,
              ScenarioDbId.BOH_FAELIN_04,
              ScenarioDbId.BOH_FAELIN_05A,
              ScenarioDbId.BOH_FAELIN_05B,
              ScenarioDbId.BOH_FAELIN_06,
              ScenarioDbId.BOH_FAELIN_07,
              ScenarioDbId.BOH_FAELIN_08,
              ScenarioDbId.BOH_FAELIN_09A,
              ScenarioDbId.BOH_FAELIN_09B,
              ScenarioDbId.BOH_FAELIN_10A,
              ScenarioDbId.BOH_FAELIN_10B,
              ScenarioDbId.BOH_FAELIN_11,
              ScenarioDbId.BOH_FAELIN_12,
              ScenarioDbId.BOH_FAELIN_13,
              ScenarioDbId.BOH_FAELIN_14,
              ScenarioDbId.BOH_FAELIN_15,
              ScenarioDbId.BOH_FAELIN_16
            };
            break;
          default:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOH_JAINA_01,
              ScenarioDbId.BOH_JAINA_02,
              ScenarioDbId.BOH_JAINA_03,
              ScenarioDbId.BOH_JAINA_04,
              ScenarioDbId.BOH_JAINA_05,
              ScenarioDbId.BOH_JAINA_06,
              ScenarioDbId.BOH_JAINA_07,
              ScenarioDbId.BOH_JAINA_08
            };
            break;
        }
      }
      else
      {
        switch (wingIdFromMissionId)
        {
          case WingDbId.BOM_Xyrella:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOM_02_Xyrella_01,
              ScenarioDbId.BOM_02_Xyrella_02,
              ScenarioDbId.BOM_02_Xyrella_03,
              ScenarioDbId.BOM_02_Xyrella_04,
              ScenarioDbId.BOM_02_Xyrella_05,
              ScenarioDbId.BOM_02_Xyrella_06,
              ScenarioDbId.BOM_02_Xyrella_07,
              ScenarioDbId.BOM_02_Xyrella_08
            };
            break;
          case WingDbId.BOM_Guff:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOM_03_Guff_01,
              ScenarioDbId.BOM_03_Guff_02,
              ScenarioDbId.BOM_03_Guff_03,
              ScenarioDbId.BOM_03_Guff_04,
              ScenarioDbId.BOM_03_Guff_05,
              ScenarioDbId.BOM_03_Guff_06,
              ScenarioDbId.BOM_03_Guff_07,
              ScenarioDbId.BOM_03_Guff_08
            };
            break;
          case WingDbId.BOM_Kurtrus:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOM_04_Kurtrus_01,
              ScenarioDbId.BOM_04_Kurtrus_02,
              ScenarioDbId.BOM_04_Kurtrus_03,
              ScenarioDbId.BOM_04_Kurtrus_04,
              ScenarioDbId.BOM_04_Kurtrus_05,
              ScenarioDbId.BOM_04_Kurtrus_06,
              ScenarioDbId.BOM_04_Kurtrus_07,
              ScenarioDbId.BOM_04_Kurtrus_08
            };
            break;
          case WingDbId.BOM_Tamsin:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOM_05_Tamsin_001,
              ScenarioDbId.BOM_05_Tamsin_002,
              ScenarioDbId.BOM_05_Tamsin_003,
              ScenarioDbId.BOM_05_Tamsin_004,
              ScenarioDbId.BOM_05_Tamsin_005,
              ScenarioDbId.BOM_05_Tamsin_006,
              ScenarioDbId.BOM_05_Tamsin_007,
              ScenarioDbId.BOM_05_Tamsin_008
            };
            break;
          case WingDbId.BOM_Cariel:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOM_06_Cariel_001,
              ScenarioDbId.BOM_06_Cariel_002,
              ScenarioDbId.BOM_06_Cariel_003,
              ScenarioDbId.BOM_06_Cariel_004,
              ScenarioDbId.BOM_06_Cariel_005,
              ScenarioDbId.BOM_06_Cariel_006,
              ScenarioDbId.BOM_06_Cariel_007,
              ScenarioDbId.BOM_06_Cariel_008
            };
            break;
          case WingDbId.BOM_Scabbs:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOM_07_Scabbs_Fight_001,
              ScenarioDbId.BOM_07_Scabbs_Fight_002,
              ScenarioDbId.BOM_07_Scabbs_Fight_003,
              ScenarioDbId.BOM_07_Scabbs_Fight_004,
              ScenarioDbId.BOM_07_Scabbs_Fight_005,
              ScenarioDbId.BOM_07_Scabbs_Fight_006,
              ScenarioDbId.BOM_07_Scabbs_Fight_007,
              ScenarioDbId.BOM_07_Scabbs_Fight_008
            };
            break;
          case WingDbId.BOM_Tavish:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOM_08_Tavish_Fight_001,
              ScenarioDbId.BOM_08_Tavish_Fight_002,
              ScenarioDbId.BOM_08_Tavish_Fight_003,
              ScenarioDbId.BOM_08_Tavish_Fight_004,
              ScenarioDbId.BOM_08_Tavish_Fight_005,
              ScenarioDbId.BOM_08_Tavish_Fight_006,
              ScenarioDbId.BOM_08_Tavish_Fight_007,
              ScenarioDbId.BOM_08_Tavish_Fight_008
            };
            break;
          case WingDbId.BOM_Brukan:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOM_09_Brukan_Fight_001,
              ScenarioDbId.BOM_09_Brukan_Fight_002,
              ScenarioDbId.BOM_09_Brukan_Fight_003,
              ScenarioDbId.BOM_09_Brukan_Fight_004,
              ScenarioDbId.BOM_09_Brukan_Fight_005,
              ScenarioDbId.BOM_09_Brukan_Fight_006,
              ScenarioDbId.BOM_09_Brukan_Fight_007,
              ScenarioDbId.BOM_09_Brukan_Fight_008
            };
            break;
          case WingDbId.BOM_Dawngrasp:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOM_10_Dawngrasp_Fight_001,
              ScenarioDbId.BOM_10_Dawngrasp_Fight_002,
              ScenarioDbId.BOM_10_Dawngrasp_Fight_003,
              ScenarioDbId.BOM_10_Dawngrasp_Fight_004,
              ScenarioDbId.BOM_10_Dawngrasp_Fight_005,
              ScenarioDbId.BOM_10_Dawngrasp_Fight_006,
              ScenarioDbId.BOM_10_Dawngrasp_Fight_007,
              ScenarioDbId.BOM_10_Dawngrasp_Fight_008
            };
            break;
          default:
            scenarioDbIdArray = new ScenarioDbId[8]
            {
              ScenarioDbId.BOM_01_Rokara_01,
              ScenarioDbId.BOM_01_Rokara_02,
              ScenarioDbId.BOM_01_Rokara_03,
              ScenarioDbId.BOM_01_Rokara_04,
              ScenarioDbId.BOM_01_Rokara_05,
              ScenarioDbId.BOM_01_Rokara_06,
              ScenarioDbId.BOM_01_Rokara_07,
              ScenarioDbId.BOM_01_Rokara_08
            };
            break;
        }
      }
      if (val1 >= 0L && val1 < (long) scenarioDbIdArray.Length)
        missionId2 = (long) scenarioDbIdArray[val1];
      if (missionId2 > 0L)
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_SCENARIO_ID, new long[1]
        {
          missionId2
        });
    }
    if (!GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_SCENARIO_ID, out missionId2) || missionId2 <= 0L)
    {
      ScenarioDbfRecord record3 = GameDbf.Scenario.GetRecord((Predicate<ScenarioDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureDbId && (AdventureModeDbId) r.ModeId == adventureMode));
      if (record3 != null && record3.ID > 0)
      {
        missionId2 = (long) record3.ID;
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_SCENARIO_ID, new long[1]
        {
          missionId2
        });
      }
    }
    if (AdventureUtils.SelectableHeroPowersExistForAdventure(adventureDbId))
    {
      long num3 = 0;
      if (!flag && GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_POWER, out num3) && num3 > 0L)
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_POWER, new long[1]
        {
          num3
        });
      if (!GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_POWER, out num3) || num3 <= 0L)
      {
        AdventureHeroPowerDbfRecord record4 = GameDbf.AdventureHeroPower.GetRecord((Predicate<AdventureHeroPowerDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureDbId && (long) r.ClassId == deckClass));
        if (record4 != null)
        {
          long cardId = (long) record4.CardId;
          this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_POWER, new long[1]
          {
            cardId
          });
        }
      }
    }
    if (AdventureUtils.SelectableDecksExistForAdventure(adventureDbId))
    {
      long adventureDeckId = 0;
      List<long> values = (List<long>) null;
      if (!flag && GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_DECK, out adventureDeckId) && adventureDeckId > 0L)
      {
        values = GameDbf.DeckCard.GetRecords((Predicate<DeckCardDbfRecord>) (r => (long) r.DeckId == adventureDeckId)).Select<DeckCardDbfRecord, long>((Func<DeckCardDbfRecord, long>) (r => (long) r.CardId)).ToList<long>();
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CARD_LIST, values.ToArray());
      }
      if (!GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CARD_LIST, out values) || values == null || values.Count <= 0)
      {
        AdventureDeckDbfRecord record5 = GameDbf.AdventureDeck.GetRecord((Predicate<AdventureDeckDbfRecord>) (r => (AdventureDbId) r.AdventureId == adventureDbId && (long) r.ClassId == deckClass));
        if (record5 != null)
        {
          adventureDeckId = (long) record5.DeckId;
          if (adventureDeckId > 0L)
          {
            List<long> list = GameDbf.DeckCard.GetRecords((Predicate<DeckCardDbfRecord>) (r => (long) r.DeckId == adventureDeckId)).Select<DeckCardDbfRecord, long>((Func<DeckCardDbfRecord, long>) (r => (long) r.CardId)).ToList<long>();
            this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CARD_LIST, list.ToArray());
          }
        }
      }
    }
    if (!flag && AdventureUtils.SelectableLoadoutTreasuresExistForAdventure(adventureDbId))
    {
      long num4 = 0;
      if (GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_LOADOUT_TREASURE_ID, out num4) && num4 > 0L)
      {
        List<long> values;
        GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CARD_LIST, out values);
        if (values == null)
          values = new List<long>();
        values.Add(num4);
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CARD_LIST, values.ToArray());
      }
    }
    long[] numArray;
    switch (adventureDbId)
    {
      case AdventureDbId.LOOT:
        numArray = new long[8]
        {
          47316L,
          46311L,
          46915L,
          46338L,
          46371L,
          47307L,
          47001L,
          47210L
        };
        break;
      case AdventureDbId.GIL:
        numArray = new long[8]
        {
          47903L,
          48311L,
          48182L,
          48151L,
          48196L,
          48600L,
          48942L,
          48315L
        };
        break;
      case AdventureDbId.TRL:
        numArray = new long[8]
        {
          53222L,
          53223L,
          53224L,
          53225L,
          53226L,
          53227L,
          53228L,
          53229L
        };
        break;
      case AdventureDbId.DALARAN:
        numArray = new long[12]
        {
          53750L,
          53779L,
          53667L,
          53558L,
          53572L,
          53636L,
          53607L,
          53309L,
          53562L,
          53483L,
          53714L,
          53783L
        };
        break;
      case AdventureDbId.BOH:
        switch (wingIdFromMissionId)
        {
          case WingDbId.BOH_REXXAR:
            numArray = new long[8]
            {
              63834L,
              63835L,
              63836L,
              61384L,
              63837L,
              61385L,
              63838L,
              63839L
            };
            break;
          case WingDbId.BOH_GARROSH:
            numArray = new long[8]
            {
              61390L,
              64757L,
              64758L,
              64759L,
              64760L,
              64761L,
              64762L,
              64763L
            };
            break;
          case WingDbId.BOH_UTHER:
            numArray = new long[8]
            {
              61388L,
              65557L,
              65558L,
              65559L,
              61389L,
              65560L,
              65561L,
              65562L
            };
            break;
          case WingDbId.BOH_ANDUIN:
            numArray = new long[8]
            {
              66904L,
              66902L,
              66903L,
              66904L,
              66905L,
              66906L,
              66908L,
              66909L
            };
            break;
          case WingDbId.BOH_VALEERA:
            numArray = new long[8]
            {
              68015L,
              68016L,
              68017L,
              68018L,
              68019L,
              68020L,
              68021L,
              68022L
            };
            break;
          case WingDbId.BOH_THRALL:
            numArray = new long[8]
            {
              71187L,
              71188L,
              71189L,
              71190L,
              71191L,
              71192L,
              71193L,
              71194L
            };
            break;
          case WingDbId.BOH_MALFURION:
            numArray = new long[8]
            {
              71857L,
              71865L,
              71866L,
              71867L,
              71868L,
              71869L,
              71870L,
              71871L
            };
            break;
          case WingDbId.BOH_GULDAN:
            numArray = new long[8]
            {
              73910L,
              73918L,
              73919L,
              73920L,
              73921L,
              73922L,
              73923L,
              73924L
            };
            break;
          case WingDbId.BOH_ILLIDAN:
            numArray = new long[8]
            {
              75649L,
              75657L,
              75658L,
              75659L,
              75661L,
              75662L,
              75663L,
              75664L
            };
            break;
          case WingDbId.BOH_FAELIN:
            numArray = new long[19]
            {
              79358L,
              79364L,
              79365L,
              79359L,
              79366L,
              79367L,
              79368L,
              79369L,
              79360L,
              79371L,
              79370L,
              79372L,
              79377L,
              79373L,
              80032L,
              79376L,
              79361L,
              79379L,
              79380L
            };
            break;
          default:
            numArray = new long[8]
            {
              63199L,
              63201L,
              63204L,
              63205L,
              63206L,
              63207L,
              63208L,
              61382L
            };
            break;
        }
        break;
      case AdventureDbId.BOM:
        switch (wingIdFromMissionId)
        {
          case WingDbId.BOM_Xyrella:
            numArray = new long[8]
            {
              71943L,
              71946L,
              71947L,
              71948L,
              71951L,
              71955L,
              71957L,
              71958L
            };
            break;
          case WingDbId.BOM_Guff:
            numArray = new long[8]
            {
              73323L,
              73324L,
              73325L,
              73326L,
              73327L,
              73328L,
              73329L,
              73330L
            };
            break;
          case WingDbId.BOM_Kurtrus:
            numArray = new long[8]
            {
              74770L,
              74771L,
              74772L,
              74773L,
              74774L,
              74775L,
              74777L,
              74778L
            };
            break;
          case WingDbId.BOM_Tamsin:
            numArray = new long[8]
            {
              76424L,
              76425L,
              76426L,
              76427L,
              76430L,
              76431L,
              76432L,
              76433L
            };
            break;
          case WingDbId.BOM_Cariel:
            numArray = new long[8]
            {
              78435L,
              78437L,
              78438L,
              78439L,
              78440L,
              78441L,
              78442L,
              78443L
            };
            break;
          case WingDbId.BOM_Scabbs:
            numArray = new long[8]
            {
              80896L,
              80897L,
              80898L,
              80899L,
              80900L,
              80901L,
              80902L,
              80903L
            };
            break;
          case WingDbId.BOM_Tavish:
            numArray = new long[8]
            {
              82407L,
              82409L,
              82412L,
              82430L,
              82416L,
              82417L,
              82419L,
              82420L
            };
            break;
          case WingDbId.BOM_Brukan:
            numArray = new long[8]
            {
              85837L,
              85838L,
              85839L,
              85840L,
              85841L,
              85842L,
              85843L,
              85844L
            };
            break;
          case WingDbId.BOM_Dawngrasp:
            numArray = new long[8]
            {
              86336L,
              86337L,
              86338L,
              68339L,
              86340L,
              86341L,
              86342L,
              86343L
            };
            break;
          default:
            numArray = new long[8]
            {
              67655L,
              67656L,
              67657L,
              67658L,
              67659L,
              67660L,
              67661L,
              67662L
            };
            break;
        }
        break;
      default:
        numArray = new long[8]
        {
          57319L,
          57378L,
          57322L,
          57397L,
          57573L,
          53810L,
          57387L,
          56176L
        };
        break;
    }
    switch (mode)
    {
      case Cheats.SetAdventureProgressMode.Victory:
        val1 = (long) numArray.Length;
        break;
      case Cheats.SetAdventureProgressMode.Progress:
        val1 = Math.Min(val1, (long) (numArray.Length - 1));
        break;
    }
    int adventureBossesInRun = AdventureConfig.GetAdventureBossesInRun(GameUtils.GetWingRecordFromMissionId((int) missionId2));
    if (adventureBossesInRun > 0)
      val1 = Math.Min(val1, (long) (adventureBossesInRun - 1));
    long num5 = 0;
    this.ValidateAndParseLongAtIndex(1, args, out num5);
    List<long> values1;
    GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSSES_DEFEATED, out values1);
    if (values1 == null)
      values1 = new List<long>();
    if ((long) values1.Count > val1)
    {
      int count = values1.Count - (int) val1;
      values1.RemoveRange(values1.Count - count, count);
    }
    else
    {
      while ((long) values1.Count < val1)
        values1.Add(numArray[values1.Count]);
    }
    this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSSES_DEFEATED, values1.ToArray());
    this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_RUN_ACTIVE, new long[1]
    {
      mode == Cheats.SetAdventureProgressMode.Progress ? 1L : 0L
    });
    this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_RUN_RETIRED, new long[0]);
    if (mode == Cheats.SetAdventureProgressMode.Victory || mode == Cheats.SetAdventureProgressMode.Defeat)
    {
      this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_TREASURE, new long[0]);
      this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_LOOT, new long[0]);
    }
    switch (mode)
    {
      case Cheats.SetAdventureProgressMode.Victory:
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSS_LOST_TO, new long[0]);
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEXT_BOSS_FIGHT, new long[0]);
        break;
      case Cheats.SetAdventureProgressMode.Defeat:
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSS_LOST_TO, new long[1]
        {
          numArray[values1.Count]
        });
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEXT_BOSS_FIGHT, new long[0]);
        break;
      default:
        if (num5 == 0L && values1.Count < numArray.Length)
          num5 = numArray[values1.Count];
        this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEXT_BOSS_FIGHT, new long[1]
        {
          num5
        });
        break;
    }
    this.InvokeSetGameSaveDataCheat(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_LATEST_DUNGEON_RUN_COMPLETE, new long[1]);
    return true;
  }

  private bool OnProcessCheat_SetAllPuzzlesInProgress(string func, string[] args, string rawArgs)
  {
    int saveDataServerKey = GameDbf.AdventureData.GetRecord((Predicate<AdventureDataDbfRecord>) (r => r.AdventureId == 429)).GameSaveDataServerKey;
    foreach (ScenarioDbfRecord record in GameDbf.Scenario.GetRecords((Predicate<ScenarioDbfRecord>) (r => r.AdventureId == 429)))
    {
      int dataProgressSubkey = record.GameSaveDataProgressSubkey;
      int saveDataProgressMax = record.GameSaveDataProgressMax;
      this.InvokeSetGameSaveDataCheat((GameSaveKeyId) saveDataServerKey, (GameSaveKeySubkeyId) dataProgressSubkey, new long[1]
      {
        (long) saveDataProgressMax
      });
    }
    UIStatus.Get().AddInfo(string.Format("Set All Boomsday Puzzles To Their Last Sub-Puzzle"));
    return true;
  }

  private void InvokeSetGameSaveDataCheat(
    GameSaveKeyId key,
    GameSaveKeySubkeyId subkey,
    long value)
  {
    this.InvokeSetGameSaveDataCheat(key, subkey, new long[1]
    {
      value
    });
  }

  private void InvokeSetGameSaveDataCheat(
    GameSaveKeyId key,
    GameSaveKeySubkeyId subkey,
    long[] values)
  {
    this.InvokeSetGameSaveDataCheat((int) key, subkey, values);
  }

  private void InvokeSetGameSaveDataCheat(int key, GameSaveKeySubkeyId subkey, long[] values)
  {
    List<string> stringList = new List<string>()
    {
      key.ToString(),
      ((int) subkey).ToString()
    };
    if (values != null)
    {
      foreach (long num in values)
        stringList.Add(num.ToString());
    }
    this.OnProcessCheat_SetGameSaveData("setgsd", stringList.ToArray(), string.Join(" ", stringList.ToArray()));
  }

  private bool OnProcessCheat_GetGameSaveData(string func, string[] args, string rawArgs)
  {
    GameSaveKeyId key = ~GameSaveKeyId.INVALID;
    GameSaveKeySubkeyId subkey = ~GameSaveKeySubkeyId.INVALID;
    if (!this.ValidateAndParseGameSaveDataKeyAndSubkey(args, out key, out subkey))
    {
      UIStatus.Get().AddError("You must provide valid key and subkeys!");
      return true;
    }
    args = new string[3]
    {
      "getgsd",
      "key=" + args[0],
      "subkey=" + args[1]
    };
    return this.OnProcessCheat_utilservercmd("util", args, rawArgs, (AutofillData) null);
  }

  private bool ValidateAndParseLongAtIndex(int index, string[] args, out long value)
  {
    value = 0L;
    long result = 0;
    if (args.Length <= index || !long.TryParse(args[index], out result))
      return false;
    value = result;
    return true;
  }

  private bool ValidateAndParseGameSaveDataKeyAndSubkey(
    string[] args,
    out GameSaveKeyId key,
    out GameSaveKeySubkeyId subkey)
  {
    key = ~GameSaveKeyId.INVALID;
    subkey = ~GameSaveKeySubkeyId.INVALID;
    long result1 = 0;
    if (args.Length < 1 || !long.TryParse(args[0], out result1) || result1 == 0L)
    {
      UIStatus.Get().AddError("You must provide a valid non-zero id for the key!");
      return false;
    }
    key = (GameSaveKeyId) result1;
    long result2 = 0;
    if (args.Length < 2 || !long.TryParse(args[1], out result2) || result2 == 0L)
    {
      UIStatus.Get().AddError("You must provide a valid non-zero id for the key!");
      return false;
    }
    subkey = (GameSaveKeySubkeyId) result2;
    return true;
  }

  private bool OnProcessCheat_ResetKC_VO(string func, string[] args, string rawArgs)
  {
    long subkeyValue;
    this.ValidateAndParseLongAtIndex(0, args, out subkeyValue);
    this.ResetDungeonRun_VO(AdventureDbId.LOOT, subkeyValue);
    return true;
  }

  private bool OnProcessCheat_ResetGIL_VO(string func, string[] args, string rawArgs)
  {
    long subkeyValue;
    this.ValidateAndParseLongAtIndex(0, args, out subkeyValue);
    this.ResetDungeonRun_VO(AdventureDbId.GIL, subkeyValue);
    return true;
  }

  private bool OnProcessCheat_UnlockHagatha(string func, string[] args, string rawArgs)
  {
    this.InvokeSetGameSaveDataCheat(GameSaveKeyId.ADVENTURE_DATA_SERVER_GIL, GameSaveKeySubkeyId.DUNGEON_CRAWL_HUNTER_RUN_WINS, new long[1]
    {
      1L
    });
    this.InvokeSetGameSaveDataCheat(GameSaveKeyId.ADVENTURE_DATA_SERVER_GIL, GameSaveKeySubkeyId.DUNGEON_CRAWL_WARRIOR_RUN_WINS, new long[1]
    {
      1L
    });
    this.InvokeSetGameSaveDataCheat(GameSaveKeyId.ADVENTURE_DATA_SERVER_GIL, GameSaveKeySubkeyId.DUNGEON_CRAWL_ROGUE_RUN_WINS, new long[1]
    {
      1L
    });
    this.OnProcessCheat_utilservercmd("util", new string[4]
    {
      "quest",
      "progress",
      "achieve=1010",
      "amount=3"
    }, "util quest progress achieve=1010 amount=3", (AutofillData) null);
    this.SetAdventureProgressCommon(AdventureDbId.GIL, AdventureModeDbId.DUNGEON_CRAWL, new string[1]
    {
      "7"
    }, Cheats.SetAdventureProgressMode.Progress);
    this.InvokeSetGameSaveDataCheat(GameSaveKeyId.ADVENTURE_DATA_CLIENT_GIL, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_CHARACTER_SELECT_VO, new long[1]
    {
      1L
    });
    this.InvokeSetGameSaveDataCheat(GameSaveKeyId.ADVENTURE_DATA_CLIENT_GIL, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_1_VO, new long[1]
    {
      1L
    });
    this.InvokeSetGameSaveDataCheat(GameSaveKeyId.ADVENTURE_DATA_CLIENT_GIL, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_2_VO, new long[1]
    {
      1L
    });
    this.InvokeSetGameSaveDataCheat(GameSaveKeyId.ADVENTURE_DATA_CLIENT_GIL, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_3_VO, new long[1]
    {
      1L
    });
    this.InvokeSetGameSaveDataCheat(GameSaveKeyId.ADVENTURE_DATA_CLIENT_GIL, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_TREASURE_1_VO, new long[1]
    {
      1L
    });
    this.InvokeSetGameSaveDataCheat(GameSaveKeyId.ADVENTURE_DATA_CLIENT_GIL, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_LOOT_PACKS_1_VO, new long[1]
    {
      1L
    });
    this.InvokeSetGameSaveDataCheat(GameSaveKeyId.ADVENTURE_DATA_CLIENT_GIL, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_LOOT_PACKS_2_VO, new long[1]
    {
      1L
    });
    return true;
  }

  private bool OnProcessCheat_ResetTRL_VO(string func, string[] args, string rawArgs)
  {
    long subkeyValue;
    this.ValidateAndParseLongAtIndex(0, args, out subkeyValue);
    this.ResetDungeonRun_VO(AdventureDbId.TRL, subkeyValue);
    return true;
  }

  private bool OnProcessCheat_ResetDAL_VO(string func, string[] args, string rawArgs)
  {
    long subkeyValue;
    this.ValidateAndParseLongAtIndex(0, args, out subkeyValue);
    this.ResetDungeonRun_VO(AdventureDbId.DALARAN, subkeyValue);
    return true;
  }

  private bool OnProcessCheat_ResetULD_VO(string func, string[] args, string rawArgs)
  {
    long subkeyValue;
    this.ValidateAndParseLongAtIndex(0, args, out subkeyValue);
    this.ResetDungeonRun_VO(AdventureDbId.ULDUM, subkeyValue);
    return true;
  }

  private void ResetVOSubkeysForAdventure(GameSaveKeyId adventureGameSaveKey, long subkeyValue = 0)
  {
    List<GameSaveKeySubkeyId> gameSaveKeySubkeyIdList1 = new List<GameSaveKeySubkeyId>();
    gameSaveKeySubkeyIdList1.Add(GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_WING_COMPLETE_VO);
    gameSaveKeySubkeyIdList1.Add(GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_COMPLETE_ALL_CLASSES_VO);
    List<GameSaveKeySubkeyId> gameSaveKeySubkeyIdList2 = new List<GameSaveKeySubkeyId>()
    {
      GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSS_HERO_POWER_TUTORIAL_PROGRESS,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_CHARACTER_SELECT_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_WELCOME_BANNER_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_1_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_2_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_3_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_4_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_5_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_TREASURE_1_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_TREASURE_2_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_TREASURE_3_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_TREASURE_4_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_HERO_POWER_1_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_DECK_1_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_LOOT_PACKS_1_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_LOOT_PACKS_2_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOOK_REVEAL_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOOK_REVEAL_HEROIC_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_WING_UNLOCK_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_COMPLETE_ALL_WINGS_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_COMPLETE_ALL_WINGS_HEROIC_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_ANOMALY_UNLOCK_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_REWARD_PAGE_REVEAL_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_FINAL_BOSS_LOSS_1_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_FINAL_BOSS_LOSS_2_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_LOSS_1_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_WIN_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_2_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_1_VO,
      GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_2_VO,
      GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_ADVENTURE,
      GameSaveKeySubkeyId.TRL_DUNGEON_HAS_SEEN_SHRINE_TUTORIAL_1_VO,
      GameSaveKeySubkeyId.TRL_DUNGEON_HAS_SEEN_SHRINE_TUTORIAL_2_VO,
      GameSaveKeySubkeyId.TRL_DUNGEON_HAS_SEEN_ENEMY_SHRINE_DIES_TUTORIAL_VO,
      GameSaveKeySubkeyId.TRL_DUNGEON_HAS_SEEN_ENEMY_SHRINE_REVIVES_TUTORIAL_VO,
      GameSaveKeySubkeyId.TRL_DUNGEON_HAS_SEEN_PLAYER_SHRINE_DIES_TUTORIAL_VO,
      GameSaveKeySubkeyId.TRL_DUNGEON_HAS_SEEN_PLAYER_SHRINE_TIMER_TICK_TUTORIAL_VO,
      GameSaveKeySubkeyId.TRL_DUNGEON_HAS_SEEN_PLAYER_SHRINE_LOST_TUTORIAL_VO,
      GameSaveKeySubkeyId.TRL_DUNGEON_HAS_SEEN_PLAYER_SHRINE_TRANSFORMED_TUTORIAL_VO,
      GameSaveKeySubkeyId.TRL_DUNGEON_HAS_SEEN_PLAYER_SHRINE_BOUNCED_TUTORIAL_VO
    };
    foreach (GameSaveKeySubkeyId subkey in gameSaveKeySubkeyIdList1)
    {
      long[] values = (long[]) null;
      this.InvokeSetGameSaveDataCheat(adventureGameSaveKey, subkey, values);
    }
    foreach (GameSaveKeySubkeyId subkey in gameSaveKeySubkeyIdList2)
    {
      long[] values = (long[]) null;
      if (subkeyValue != 0L)
        values = new long[1]{ subkeyValue };
      this.InvokeSetGameSaveDataCheat(adventureGameSaveKey, subkey, values);
    }
  }

  private bool OnProcessCheat_SetAdventureComingSoon(string func, string[] args, string rawArgs)
  {
    if (args.Length < 2)
    {
      UIStatus.Get().AddInfo("Usage: setadventurecomingsoon [ADVENTURE] [TRUE/FALSE]\nExample: setadventurecomingsoon GIL true");
      return false;
    }
    AdventureDbId adventureDbIdFromArgs = Cheats.ParseAdventureDbIdFromArgs(args, 0);
    if (adventureDbIdFromArgs == AdventureDbId.INVALID)
      return false;
    bool result = false;
    if (!bool.TryParse(args[1], out result))
    {
      UIStatus.Get().AddError(string.Format("Unable to parse \"{0}\". Please enter True or False.", (object) args[1]));
      return false;
    }
    AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) adventureDbIdFromArgs);
    record.SetVar("COMING_SOON_EVENT", result ? (object) "always" : (object) "never");
    GameDbf.Adventure.ReplaceRecordByRecordId(record);
    string message = (UnityEngine.Object) AdventureScene.Get() == (UnityEngine.Object) null ? "Success!" : "Success!\nBack out and re-enter to see the change.";
    UIStatus.Get().AddInfo(message);
    return true;
  }

  private bool OnProcessCheat_ResetSession_VO(string func, string[] args, string rawArgs)
  {
    NotificationManager.Get().ResetSoundsPlayedThisSession();
    return true;
  }

  private bool OnProcessCheat_SetVOChance_VO(string func, string[] args, string rawArgs)
  {
    float result = -1f;
    if (args.Length != 0 && float.TryParse(args[0], out result) && (double) result >= 0.0)
      result = Mathf.Clamp(result, 0.0f, 1f);
    Cheats.VOChanceOverride = result;
    return true;
  }

  private BnetPlayer CreateCheatFriendlistItem(
    string name,
    Cheats.FriendListType type,
    int leagueId,
    int starLevel,
    BnetProgramId programID,
    bool isFriend,
    bool isOnline,
    bool isAway)
  {
    switch (type)
    {
      case Cheats.FriendListType.FRIEND:
        return BnetFriendMgr.Get().Cheat_CreateFriend(name, leagueId, starLevel, programID, isOnline, isAway);
      case Cheats.FriendListType.RECENT:
        return BnetRecentPlayerMgr.Get().Cheat_CreateRecentPlayer(name, leagueId, starLevel, programID, isFriend, isOnline);
      case Cheats.FriendListType.NEARBY:
        return BnetNearbyPlayerMgr.Get().Cheat_CreateNearbyPlayer(name, leagueId, starLevel, programID, isFriend, isOnline);
      case Cheats.FriendListType.FSG:
        return FiresideGatheringManager.Get().Cheat_CreateFSGPatron(name, leagueId, starLevel, programID, isFriend, isOnline);
      default:
        return (BnetPlayer) null;
    }
  }

  private bool OnProcessCheat_History(string func, string[] args, string rawArgs)
  {
    HistoryManager historyManager = HistoryManager.Get();
    if ((UnityEngine.Object) historyManager == (UnityEngine.Object) null)
      return false;
    if (args[0].ToLower() == "true" || args[0].ToLower() == "on" || args[0] == "1")
      historyManager.EnableHistory();
    if (args[0].ToLower() == "false" || args[0].ToLower() == "off" || args[0] == "0")
      historyManager.DisableHistory();
    return true;
  }

  private bool OnProcessCheat_IPAddress(string func, string[] args, string rawArgs)
  {
    IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
    if (hostEntry.AddressList.Length != 0)
    {
      string message = "";
      foreach (IPAddress address in hostEntry.AddressList)
        message = message + address.ToString() + "\n";
      UIStatus.Get().AddInfo(message, 10f);
    }
    return true;
  }

  private bool OnProcessCheat_Attribution(string func, string[] args, string rawArgs)
  {
    string message = BlizzardAttributionManagerDebug.HandleCheat(func, args, rawArgs);
    UIStatus.Get().AddInfo(message);
    return true;
  }

  private bool OnProcessCheat_CRM(string func, string[] args, string rawArgs)
  {
    BlizzardCRMManager.Get().SendAllEventsForTest();
    UIStatus.Get().AddInfo("Test CRM telemetry sent!");
    return true;
  }

  private bool OnProcessCheat_Updater(string func, string[] args, string rawArgs)
  {
    string message1 = "USAGE: updater [cmd] [args]\\nCommands: speed, gamespeed\\nNotice: Unit of speed is bytes per second.\\n\n\\t0 = unlimited, -1 = turn off game streaming\\n\\tStore the speed permanently: speed 0 store";
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      UIStatus.Get().AddInfo(message1, 10f);
      return true;
    }
    if (this.DownloadManager == null)
    {
      UIStatus.Get().AddInfo("DownloadManager is not ready yet!");
      return true;
    }
    string str = args[0];
    bool flag1 = true;
    bool flag2 = false;
    int val = 0;
    if (args.Length > 1)
    {
      val = int.Parse(args[1]);
      flag2 = args.Length > 2 && args[2].Equals("store");
    }
    else
      flag1 = false;
    string message2 = (string) null;
    if (!(str == "help"))
    {
      if (!(str == "speed"))
      {
        if (str == "gamespeed")
        {
          if (flag1)
          {
            if (val < 0)
            {
              this.DownloadManager.InGameStreamingDefaultSpeed = val;
              message2 = "Turned off in game streaming";
            }
            else
            {
              this.DownloadManager.DownloadSpeedInGame = val;
              message2 = "Set the download speed in game to " + (object) val;
            }
          }
          else
            message2 = "The current speed in game is " + (object) this.DownloadManager.DownloadSpeedInGame;
          if (flag2 && val >= 0)
            Options.Get().SetInt(Option.STREAMING_SPEED_IN_GAME, val);
        }
      }
      else if (val < 0)
      {
        message2 = "Error: Cannot use the negative value!";
      }
      else
      {
        if (flag1)
        {
          this.DownloadManager.MaxDownloadSpeed = val;
          message2 = "Set the download speed to " + (object) val;
        }
        else
          message2 = "The current speed is " + (object) this.DownloadManager.MaxDownloadSpeed;
        if (flag2)
          Options.Get().SetInt(Option.MAX_DOWNLOAD_SPEED, val);
      }
    }
    else
      message2 = message1;
    if (message2 != null)
      UIStatus.Get().AddInfo(message2, 5f);
    return true;
  }

  private bool OnProcessCheat_Assets(string func, string[] args, string rawArgs)
  {
    string message = AssetLoaderDebug.HandleCheat(func, args, rawArgs);
    UIStatus.Get().AddInfo(message);
    return true;
  }

  private bool OnProcessCheat_testproducttag(string func, string[] args, string rawArgs)
  {
    string message = "USAGE: testproducttag <tag_name>";
    if (args.Length < 1)
    {
      UIStatus.Get().AddInfo(message, 10f);
      return true;
    }
    int num = StoreManager.Get().Catalog.DebugFillShopWithProductsByTag(args[0]);
    UIStatus.Get().AddInfo(string.Format("Shop filled with {0} products that have tag {1}", (object) num, (object) args[0]), 10f);
    return true;
  }

  private bool OnProcessCheat_testproduct(string func, string[] args, string rawArgs)
  {
    string message = "USAGE: testproduct <pmt_product_id>";
    long result;
    if (args.Length < 1 || !long.TryParse(args[0], out result))
    {
      UIStatus.Get().AddInfo(message, 10f);
      return true;
    }
    if (!ProductId.IsValid(result))
    {
      UIStatus.Get().AddInfo(string.Format("Product ID: {0} is out of range.", (object) result), 10f);
      return true;
    }
    ProductId from = ProductId.CreateFrom(result);
    string str = StoreManager.Get().Catalog.DebugFillShopWithProduct(from);
    if (str == null)
      UIStatus.Get().AddInfo(string.Format("Shop filled with product {0}", (object) result), 10f);
    else
      UIStatus.Get().AddInfo("Error: " + str, 10f);
    return true;
  }

  private bool OnProcessCheat_testadventurestore(string func, string[] args, string rawArgs)
  {
    string message = "USAGE: testadventurestore <wing_id> <is_full_adventure>";
    int result;
    if (args.Length < 1 || !int.TryParse(args[0], out result))
    {
      UIStatus.Get().AddInfo(message, 10f);
      return true;
    }
    bool boolVal = false;
    if (args.Length >= 2 && !GeneralUtils.TryParseBool(args[1], out boolVal))
    {
      UIStatus.Get().AddInfo(message, 10f);
      return true;
    }
    WingDbfRecord record = GameDbf.Wing.GetRecord(result);
    if (record == null)
    {
      UIStatus.Get().AddInfo(string.Format("wing {0} not found", (object) result), 10f);
      return true;
    }
    if (AdventureProgressMgr.Get() == null)
    {
      UIStatus.Get().AddInfo("AdventureProgressMgr not initialized", 10f);
      return true;
    }
    int adventureId = record.AdventureId;
    int numItemsRequired = 0;
    int pmtProductId = 0;
    ProductType product;
    ShopType shopType;
    switch ((AdventureDbId) adventureId)
    {
      case AdventureDbId.INVALID:
        UIStatus.Get().AddInfo(string.Format("wing {0} is not part of an adventure.", (object) result), 10f);
        return true;
      case AdventureDbId.TUTORIAL:
      case AdventureDbId.PRACTICE:
      case AdventureDbId.ICC:
      case AdventureDbId.GIL:
      case AdventureDbId.TRL:
        UIStatus.Get().AddInfo(string.Format("wing {0} is part of a free adventure.", (object) result), 10f);
        return true;
      case AdventureDbId.NAXXRAMAS:
        product = ProductType.PRODUCT_TYPE_NAXX;
        shopType = ShopType.ADVENTURE_STORE;
        numItemsRequired = 1;
        break;
      case AdventureDbId.BRM:
        product = ProductType.PRODUCT_TYPE_BRM;
        shopType = ShopType.ADVENTURE_STORE;
        numItemsRequired = 1;
        break;
      case AdventureDbId.LOE:
        product = ProductType.PRODUCT_TYPE_LOE;
        shopType = ShopType.ADVENTURE_STORE;
        numItemsRequired = 1;
        break;
      case AdventureDbId.KARA:
        product = ProductType.PRODUCT_TYPE_WING;
        shopType = ShopType.ADVENTURE_STORE;
        numItemsRequired = 1;
        break;
      default:
        product = ProductType.PRODUCT_TYPE_WING;
        if (boolVal)
        {
          shopType = ShopType.ADVENTURE_STORE_FULL_PURCHASE_WIDGET;
          pmtProductId = record.PmtProductIdForThisAndRestOfAdventure;
          if (pmtProductId == 0)
          {
            UIStatus.Get().AddInfo(string.Format("wing {0} has no product id defined to complete the adventure", (object) result), 10f);
            return true;
          }
          break;
        }
        shopType = ShopType.ADVENTURE_STORE_WING_PURCHASE_WIDGET;
        pmtProductId = record.PmtProductIdForSingleWingPurchase;
        if (pmtProductId == 0)
        {
          UIStatus.Get().AddInfo(string.Format("wing {0} has no product id defined by the single wing", (object) result), 10f);
          return true;
        }
        break;
    }
    ItemOwnershipStatus itemOwnershipStatus = StoreManager.GetProductItemOwnershipStatus(product, record.ID, out string _);
    if (itemOwnershipStatus == ItemOwnershipStatus.OWNED)
      UIStatus.Get().AddInfo(string.Format("Cannot show store where wing ownership status is {0}", (object) itemOwnershipStatus.ToString()), 10f);
    StoreManager.Get().StartAdventureTransaction(product, record.ID, (Store.ExitCallback) null, (object) null, shopType, numItemsRequired, pmtProductId: pmtProductId);
    return true;
  }

  private bool OnProcessCheat_refreshcurrency(string func, string[] args, string rawArgs)
  {
    CurrencyType currencyType = CurrencyType.NONE;
    if (args.Length != 0)
    {
      string a = (args[0] ?? string.Empty).Trim();
      if (string.Equals(a, "runestones", StringComparison.OrdinalIgnoreCase))
        currencyType = ShopUtils.IsMainVirtualCurrencyType(CurrencyType.CN_RUNESTONES) ? CurrencyType.CN_RUNESTONES : CurrencyType.ROW_RUNESTONES;
      else if (string.Equals(a, "arcane_orbs", StringComparison.OrdinalIgnoreCase))
        currencyType = CurrencyType.CN_ARCANE_ORBS;
    }
    if (currencyType == CurrencyType.NONE)
    {
      string message = "USAGE: refreshcurrency <runestones|arcane_orbs>";
      UIStatus.Get().AddInfo(message, 10f);
      return true;
    }
    CurrencyCache currencyCache = StoreManager.Get().GetCurrencyCache(currencyType);
    currencyCache.MarkDirty();
    currencyCache.TryRefresh();
    return true;
  }

  private bool OnProcessCheat_mercpackgrantdiamondcard(string func, string[] args, string rawArgs)
  {
    int result1 = 0;
    int result2 = 0;
    if (args.Length != 2 || !int.TryParse(args[0], out result1) || !int.TryParse(args[1], out result2))
    {
      string message = "USAGE: mercpackgrantdiamondcard <merc_id> <mercenary_art_variation id>";
      UIStatus.Get().AddInfo(message, 10f);
      return true;
    }
    if (!PackOpening.Get().CreateMockLettucePackComponent(result1, result2, 0, false, TAG_PREMIUM.DIAMOND))
    {
      string message = "Invalid Mercenary Art Variation Id";
      UIStatus.Get().AddInfo(message, 10f);
    }
    return true;
  }

  private bool OnProcessCheat_mercpackforcemercskin(string func, string[] args, string rawArgs)
  {
    int result1 = 0;
    int result2 = 0;
    int result3 = 0;
    if (args.Length != 3 || !int.TryParse(args[0], out result1) || !int.TryParse(args[1], out result2) || !int.TryParse(args[2], out result3))
    {
      string message = "USAGE: mercpackforcemercskin <merc_id> <mercenary_art_variation id> <premium_type>";
      UIStatus.Get().AddInfo(message, 10f);
      return true;
    }
    if (!PackOpening.Get().CreateMockLettucePackComponent(result1, result2, 0, false, (TAG_PREMIUM) result3))
    {
      string message = "Invalid Mercenary Art Variation Id";
      UIStatus.Get().AddInfo(message, 10f);
    }
    return true;
  }

  private bool OnProcessCheat_mercpackduplicate(string func, string[] args, string rawArgs)
  {
    int result1 = 0;
    int result2 = 0;
    if (args.Length != 2 || !int.TryParse(args[0], out result1) || !int.TryParse(args[1], out result2))
    {
      string message = "USAGE: mercpackduplicate <merc_id> <amount>";
      UIStatus.Get().AddInfo(message, 10f);
      return true;
    }
    PackOpening.Get().CreateMockLettucePackComponent(result1, 0, result2, true, TAG_PREMIUM.NORMAL);
    return true;
  }

  private bool OnProcessCheat_loadpersonalizedshop(string func, string[] args, string rawArgs)
  {
    if (args.Length < 1)
    {
      string message = "USAGE: loadpersonalizedshop <page_id>";
      UIStatus.Get().AddInfo(message, 10f);
      return true;
    }
    string str = args[0];
    if (str.Equals("false", StringComparison.CurrentCultureIgnoreCase) || str.Equals("null", StringComparison.CurrentCultureIgnoreCase))
    {
      StoreManager.Get().SetPersonalizedShopPageAndRefreshCatalog((List<BattlePayConfigShopPage>) null);
    }
    else
    {
      List<BattlePayConfigShopPage> pages = new List<BattlePayConfigShopPage>()
      {
        new BattlePayConfigShopPage()
        {
          ShopType = PegasusShared.ShopType.SHOP_TYPE_GENERAL,
          PersonalizedShopPageId = str
        }
      };
      StoreManager.Get().SetPersonalizedShopPageAndRefreshCatalog(pages);
    }
    return true;
  }

  private bool OnProcessCheat_checkfornewquests(string func, string[] args, string rawArgs)
  {
    float result = 0.0f;
    if (args.Length != 0 && !string.IsNullOrEmpty(args[0]) && !float.TryParse(args[0], out result))
    {
      UIStatus.Get().AddInfo("checkfornewquests [delaySeconds]");
      return true;
    }
    QuestManager.Get().DebugScheduleCheckForNewQuests(result);
    return true;
  }

  private bool OnProcessCheat_checkforexpiredquests(string func, string[] args, string rawArgs)
  {
    float result = 0.0f;
    if (args.Length != 0 && !string.IsNullOrEmpty(args[0]) && !float.TryParse(args[0], out result))
    {
      UIStatus.Get().AddInfo("checkforexpiredquests [delaySeconds]");
      return true;
    }
    QuestManager.Get().DebugScheduleCheckForExpiredQuests(result);
    return true;
  }

  private bool OnProcessCheat_showquestnotification(string func, string[] args, string rawArgs)
  {
    QuestPool.QuestPoolType poolType = QuestPool.QuestPoolType.DAILY;
    if (args.Length != 0)
      poolType = Blizzard.T5.Core.Utils.EnumUtils.SafeParse<QuestPool.QuestPoolType>(args[0], QuestPool.QuestPoolType.DAILY, true);
    QuestManager.Get().SimulateQuestNotificationPopup(poolType);
    return true;
  }

  private bool OnProcessCheat_showquestprogresstoast(string func, string[] args, string rawArgs)
  {
    string message = "showquestprogresstoast <quest_id>";
    int result;
    if (!int.TryParse(args[0], out result))
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    if (GameDbf.Quest.GetRecord(result) == null)
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    QuestManager.Get().SimulateQuestProgress(result);
    return true;
  }

  private bool OnProcessCheat_showachievementtoast(string func, string[] args, string rawArgs)
  {
    string message = "showachievementtoast <achieve_id>";
    int result;
    if (!int.TryParse(args[0], out result))
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    if (AchievementManager.Get().Debug_GetAchievementDataModel(result) == null)
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    AchievementManager.Get().ShowAchievementComplete(result);
    return true;
  }

  private bool OnProcessCheat_showachievementreward(string func, string[] args, string rawArgs)
  {
    string message = "showachievementeward <achievement_id>";
    int result;
    if (!int.TryParse(args[0], out result))
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    RewardScrollDataModel rewardScrollDataModel = AchievementFactory.CreateRewardScrollDataModel(result);
    if (rewardScrollDataModel == null)
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    RewardScroll.DebugShowFake(rewardScrollDataModel);
    return true;
  }

  private bool OnProcessCheat_showquestreward(string func, string[] args, string rawArgs)
  {
    string message = "showquestreward <quest_id>";
    int result;
    if (!int.TryParse(args[0], out result))
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    if (GameDbf.Quest.GetRecord(result) == null)
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    RewardScroll.DebugShowFake(QuestManager.Get().CreateRewardScrollDataModelByQuestId(result));
    return true;
  }

  private bool OnProcessCheat_showtrackreward(string func, string[] args, string rawArgs)
  {
    string message = "showtrackreward <level> <forPaidTrack>";
    int level;
    if (!int.TryParse(args[0], out level))
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    bool result = false;
    if (args.Length > 1)
      bool.TryParse(args[1], out result);
    Hearthstone.Progression.RewardTrack rewardTrack = RewardTrackManager.Get().GetRewardTrack(Assets.Achievement.RewardTrackType.GLOBAL);
    if (rewardTrack == null)
    {
      UIStatus.Get().AddInfo("No Reward Track Found");
      return false;
    }
    RewardTrackLevelDbfRecord trackLevelDbfRecord = rewardTrack.RewardTrackAsset.Levels.Where<RewardTrackLevelDbfRecord>((Func<RewardTrackLevelDbfRecord, bool>) (r => r.Level == level)).FirstOrDefault<RewardTrackLevelDbfRecord>();
    if (trackLevelDbfRecord == null)
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    int rewardListId = result ? trackLevelDbfRecord.PaidRewardList : trackLevelDbfRecord.FreeRewardList;
    if (rewardListId <= 0)
    {
      if (result)
        UIStatus.Get().AddInfo(string.Format("No paid rewards for level {0}.", (object) level));
      else
        UIStatus.Get().AddInfo(string.Format("No free rewards for level {0}.", (object) level));
      return true;
    }
    RewardTrackManager.Get().Cheat_ShowRewardScroll(rewardListId, level);
    return true;
  }

  private bool OnProcessCheat_showprogtileids(string func, string[] args, string rawArgs)
  {
    ProgressUtils.ShowDebugIds = !ProgressUtils.ShowDebugIds;
    return true;
  }

  private bool OnProcessCheat_showhiddenachievements(string func, string[] args, string rawArgs)
  {
    ProgressUtils.ShowHiddenAchievements = !ProgressUtils.ShowHiddenAchievements;
    return true;
  }

  private bool OnProcessCheat_earlyConcedeConfirmationDisabled(
    string func,
    string[] args,
    string rawArgs)
  {
    ProgressUtils.EarlyConcedeConfirmationDisabled = !ProgressUtils.EarlyConcedeConfirmationDisabled;
    return true;
  }

  private bool OnProcessCheat_simendofgamexp(string func, string[] args, string rawArgs)
  {
    string message = "simendofgamexp <scenario_id>";
    int result;
    if (args.Length != 1 || !int.TryParse(args[0], out result))
    {
      UIStatus.Get().AddInfo(message);
      return true;
    }
    RewardXpNotificationManager.Get().DebugSimScenario(result);
    return true;
  }

  private bool OnProcessCheat_terminateendofgamexp(string func, string[] args, string rawArgs)
  {
    RewardXpNotificationManager.Get().TerminateEndOfGameXp();
    return true;
  }

  private bool OnProcessCheat_shownotavernpasswarning(string func, string[] args, string rawArgs)
  {
    Shop.OpenTavernPassErrorPopup();
    return true;
  }

  private bool OnProcessCheat_showunclaimedtrackrewards(string func, string[] args, string rawArgs)
  {
    int trackId = 2;
    int trackLevel = 50;
    int result1;
    if (args.Length >= 1 && args[0] != "" && int.TryParse(args[0], out result1))
      trackId = result1;
    int result2;
    if (args.Length >= 2 && int.TryParse(args[1], out result2))
      trackLevel = result2;
    RewardTrackSeasonRoll.DebugShowFakeForgotTrackRewards(trackId, trackLevel);
    return true;
  }

  private bool OnProcessCheat_setlastrewardtrackseasonseen(
    string func,
    string[] args,
    string rawArgs)
  {
    if (args.Length < 2)
    {
      UIStatus.Get().AddInfo("setlastrewardtrackseasonseen <reward track type>('global' or 'battlegrounds')  <season_number>");
      return true;
    }
    Global.RewardTrackType result1 = Global.RewardTrackType.NONE;
    if (!System.Enum.TryParse<Global.RewardTrackType>(args[0], true, out result1))
    {
      UIStatus.Get().AddInfo("setlastrewardtrackseasonseen <reward track type>('global' or 'battlegrounds')  <season_number>");
      return true;
    }
    int result2 = 0;
    if (!int.TryParse(args[1], out result2))
    {
      UIStatus.Get().AddInfo("setlastrewardtrackseasonseen <reward track type>('global' or 'battlegrounds')  <season_number>");
      return true;
    }
    Hearthstone.Progression.RewardTrack rewardTrack = RewardTrackManager.Get().GetRewardTrack(result1);
    if (rewardTrack == null)
    {
      UIStatus.Get().AddInfo("Reward Track not found");
      return false;
    }
    if (!rewardTrack.SetRewardTrackSeasonLastSeen(result2))
    {
      UIStatus.Get().AddInfo("setlastrewardtrackseasonseen failed to set GSD value");
      return true;
    }
    UIStatus.Get().AddInfo(string.Format("Last reward track season seen = {0}", (object) result2));
    return true;
  }

  private bool OnProcessCheat_ShowAppRatingPrompt(string func, string[] args, string rawArgs)
  {
    string message = "USAGE: apprating [cmd] \nCommands: clear, show";
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      UIStatus.Get().AddInfo(message, 10f);
      return true;
    }
    string str = args[0];
    if (!(str == "clear"))
    {
      if (str == "show")
      {
        MobileCallbackManager.RequestAppReview(true);
        UIStatus.Get().AddInfo("Requesting app rating prompt.");
      }
    }
    else
    {
      Options.Get().SetInt(Option.APP_RATING_POPUP_COUNT, 0);
      UIStatus.Get().AddInfo("Resetting app rating prompt count.");
    }
    return true;
  }

  private bool OnProcessCheat_UpdateAADCSetting(string func, string[] args, string rawArgs)
  {
    string message1 = "USAGE: optin set <optInId> <value>\noptin get";
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      UIStatus.Get().AddInfo(message1, 10f);
      return true;
    }
    string str = args[0];
    if (!(str == "set"))
    {
      if (str == "get")
      {
        if (LoginManager.Get() != null && LoginManager.Get().OptInsReceivedDependency.IsReady())
        {
          string message2 = "";
          foreach (OptInApi.OptInType type in System.Enum.GetValues(typeof (OptInApi.OptInType)))
            message2 += string.Format("{0}({1}):{2}\n", (object) type, (object) (int) type, (object) LoginManager.Get().OptInApi.GetAccountOptIn(type));
          UIStatus.Get().AddError(message2, 10f);
        }
        else
          UIStatus.Get().AddError("Error: Opt-ins not ready yet, wait until login is complete.", 10f);
      }
    }
    else
    {
      int result1;
      bool result2;
      if (int.TryParse(args[1], out result1) && bool.TryParse(args[2], out result2))
      {
        if (System.Enum.IsDefined(typeof (OptInApi.OptInType), (object) result1))
        {
          OptInApi.OptInType type = (OptInApi.OptInType) result1;
          if (LoginManager.Get() != null && LoginManager.Get().OptInsReceivedDependency.IsReady())
          {
            LoginManager.Get().OptInApi.SetAccountOptIn(type, result2);
            UIStatus.Get().AddInfo(string.Format("Account opt in {0} set to {1}.", (object) type, (object) result2), 10f);
            PrivacyGate.Get().RefreshPrivacySettings();
          }
          else
            UIStatus.Get().AddError("Error: Opt-ins not ready yet, wait until login is complete.", 10f);
        }
        else
        {
          UIStatus.Get().AddError(string.Format("Error: No opt-in with id {0} found.", (object) result1), 10f);
          return true;
        }
      }
    }
    return true;
  }

  private bool OnProcessCheat_ShowPresence(string func, string[] args, string rawArgs)
  {
    BnetPresenceMgr bnetPresenceMgr = BnetPresenceMgr.Get();
    if (bnetPresenceMgr != null)
    {
      BnetPlayer myPlayer = bnetPresenceMgr.GetMyPlayer();
      string message = PresenceMgr.Get().GetStatusText(myPlayer) ?? "";
      if (!string.IsNullOrEmpty(message))
      {
        UIStatus.Get().AddInfo(message, 2f * SceneDebugger.GetDevTimescaleMultiplier());
        return true;
      }
    }
    return false;
  }

  private bool OnProcessCheat_ShowVillageHelpPopups(string func, string[] args, string rawArgs)
  {
    LettuceVillage objectOfType = UnityEngine.Object.FindObjectOfType<LettuceVillage>();
    if ((UnityEngine.Object) objectOfType != (UnityEngine.Object) null)
    {
      objectOfType.Dev_ShowTutorialPopups();
      return true;
    }
    UIStatus.Get().AddError("Village does not exist in scene, cannot show popups");
    return false;
  }

  private bool OnProcessCheat_ShowMercenariesTaskToasts(string func, string[] args, string rawArgs)
  {
    int result1;
    bool flag = int.TryParse(args[0], out result1);
    if (args.Length < 1 || !flag || result1 == 0)
      result1 = 1;
    int result2 = 27868;
    if (args.Length > 1)
      int.TryParse(args[1], out result2);
    int result3 = 1;
    if (args.Length > 2)
      int.TryParse(args[1], out result3);
    LettuceVillage objectOfType1 = UnityEngine.Object.FindObjectOfType<LettuceVillage>();
    LettuceMapDisplay objectOfType2 = UnityEngine.Object.FindObjectOfType<LettuceMapDisplay>();
    if ((UnityEngine.Object) objectOfType1 == (UnityEngine.Object) null && (UnityEngine.Object) objectOfType2 == (UnityEngine.Object) null)
    {
      UIStatus.Get().AddError("Village and map display do not exist in scene, cannot show toasts");
      return false;
    }
    List<MercenariesTaskState> CompletedTasks = new List<MercenariesTaskState>();
    for (int index = 0; index < result1; ++index)
      CompletedTasks.Add(new MercenariesTaskState()
      {
        Progress = result3,
        TaskId = result2
      });
    if ((UnityEngine.Object) objectOfType1 != (UnityEngine.Object) null)
    {
      objectOfType1.StartCoroutine(LettuceVillageDataUtil.ShowTaskToast(CompletedTasks, true));
      return true;
    }
    if (!((UnityEngine.Object) objectOfType2 != (UnityEngine.Object) null))
      return false;
    objectOfType2.StartCoroutine(LettuceVillageDataUtil.ShowTaskToast(CompletedTasks, true));
    return true;
  }

  private bool OnProcessCheat_MercTraining(string func, string[] args, string rawArgs)
  {
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append("USAGE: merctraining ").AppendLine("add <mercId>").AppendLine("remove <mercId>").AppendLine("claim <mercId>").AppendLine("debug");
    if (args.Length < 1 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      UIStatus.Get().AddInfo(stringBuilder.ToString(), 10f);
      return true;
    }
    int result = 0;
    if (args.Length > 1 && !int.TryParse(args[1], out result))
    {
      UIStatus.Get().AddError("Invalid merc ID");
      return false;
    }
    string str = args[0];
    if (!(str == "add"))
    {
      if (!(str == "remove"))
      {
        if (!(str == "claim"))
        {
          if (str == "debug")
          {
            (LettuceMercenary, LettuceMercenary) mercenariesInTraining = CollectionManager.Get().GetMercenariesInTraining();
            StringBuilder message = new StringBuilder();
            message.AppendLine("Mercenaries in training ...");
            if (mercenariesInTraining.Item1 == null)
              message.AppendLine("  Slot 1: Empty");
            else
              message.AppendFormat("  Slot 1: {0} ID:{1}\n", (object) mercenariesInTraining.Item1.m_mercName, (object) mercenariesInTraining.Item1.ID);
            if (mercenariesInTraining.Item2 == null)
              message.AppendLine("  Slot 2: Empty");
            else
              message.AppendFormat("  Slot 2: {0} ID:{1}\n", (object) mercenariesInTraining.Item2.m_mercName, (object) mercenariesInTraining.Item2.ID);
            Debug.Log((object) message);
          }
          else
            UIStatus.Get().AddError(str + " is not a valid command for merctraining");
        }
        else
          Network.Get().MercenariesTrainingCollectRequest(result);
      }
      else
        Network.Get().MercenariesTrainingRemoveRequest(result);
    }
    else
      Network.Get().MercenariesTrainingAddRequest(result);
    return true;
  }

  private bool OnProcessCheat_ExampleUI(string func, string[] args, string rawArgs)
  {
    if ((UnityEngine.Object) Cheats.exampleUI != (UnityEngine.Object) null)
      return true;
    Cheats.exampleUI = WidgetInstance.Create("UIFExamples.prefab:bce429027ad32fc4da9efe26c5362d6e");
    if ((UnityEngine.Object) Cheats.exampleUI == (UnityEngine.Object) null)
      return false;
    OverlayUI overlayUi = OverlayUI.Get();
    if ((UnityEngine.Object) overlayUi != (UnityEngine.Object) null)
      overlayUi.AddGameObject(Cheats.exampleUI.gameObject);
    return true;
  }

  private bool OnProcessCheat_LogMaterialService(string func, string[] args, string rawArgs)
  {
    DateTime utcNow = DateTime.UtcNow;
    string str1 = Path.Combine(Log.LogsPath, "MaterialService");
    try
    {
      Directory.CreateDirectory(str1);
    }
    catch (Exception ex)
    {
      Log.Asset.PrintInfo("Error creating CSV file directory: '" + str1 + "'\nError message: " + ex.Message);
      return false;
    }
    string str2 = utcNow.ToString("yy_MM_dd_hh_mm_ss");
    string path1 = Path.Combine(str1, str2 + "_hierarchyDump.csv");
    string path2 = Path.Combine(str1, str2 + "_stats.csv");
    string path3 = Path.Combine(str1, str2 + "_materials.csv");
    this.DumpRendererHierarchyToCsv(path1);
    this.DumpMaterialStatsToCsv(path2);
    this.DumpMaterialsToCsv(path3);
    Log.Asset.PrintInfo("Wrotedebug material service logs to: " + str1 + ".");
    return true;
  }

  private void DumpRendererHierarchyToCsv(string path)
  {
    using (FileStream fileStream = System.IO.File.Open(path, FileMode.Create))
    {
      using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream))
      {
        IMaterialService imaterialService = ServiceManager.Get<IMaterialService>();
        streamWriter.WriteLine("Renderer,Materials Count,PathToRoot");
        foreach (KeyValuePair<int, RegisteredRenderer> registeredRenderer1 in imaterialService.GetRegisteredRenderers())
        {
          RegisteredRenderer registeredRenderer2 = registeredRenderer1.Value;
          Transform transform = registeredRenderer2.Renderer.transform;
          if ((UnityEngine.Object) transform != (UnityEngine.Object) null)
            streamWriter.Write(transform.name);
          else
            streamWriter.Write("null");
          streamWriter.Write(",");
          streamWriter.Write(registeredRenderer2.Materials.Count);
          streamWriter.Write(",");
          for (; (UnityEngine.Object) transform != (UnityEngine.Object) null; transform = transform.parent)
          {
            streamWriter.Write(transform.name);
            streamWriter.Write("->");
          }
          streamWriter.WriteLine("null");
        }
      }
    }
  }

  private void DumpMaterialsToCsv(string path)
  {
    using (FileStream fileStream = System.IO.File.Open(path, FileMode.Create))
    {
      using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream))
      {
        IMaterialService imaterialService = ServiceManager.Get<IMaterialService>();
        streamWriter.WriteLine("Material,HashCode,TimesUsed");
        foreach (KeyValuePair<int, MaterialUsages> registeredMaterial in imaterialService.GetRegisteredMaterials())
        {
          MaterialUsages materialUsages = registeredMaterial.Value;
          streamWriter.WriteLine(string.Format("{0},{1},{2},{3}", (bool) (UnityEngine.Object) materialUsages.Material ? (object) materialUsages.Material.name : (object) "NULL", (object) materialUsages.HashCode, (object) materialUsages.TimesUsed, (object) materialUsages.TimeToRemove));
        }
      }
    }
  }

  private void DumpMaterialStatsToCsv(string path)
  {
    using (FileStream fileStream = System.IO.File.Open(path, FileMode.Create))
    {
      using (StreamWriter streamWriter = new StreamWriter((Stream) fileStream))
      {
        IMaterialService imaterialService = ServiceManager.Get<IMaterialService>();
        Dictionary<int, RegisteredRenderer> registeredRenderers = imaterialService.GetRegisteredRenderers();
        Dictionary<int, MaterialUsages> registeredMaterials = imaterialService.GetRegisteredMaterials();
        streamWriter.WriteLine("Custom Renderer Count,Custom Material Count,Unused Materials,Unused Renderers");
        streamWriter.WriteLine(string.Format("{0},{1},{2},{3}", (object) registeredRenderers.Count, (object) registeredMaterials.Count, (object) imaterialService.GetUnusedMaterials().Count, (object) imaterialService.GetUnusedRenderers().Count));
      }
    }
  }

  private bool OnProcessCheat_LogZombies(string func, string[] args, string rawArgs)
  {
    GameObject gameObject = UnityEngine.Object.FindObjectOfType<Processor>().gameObject;
    ZombieObjectDetector component1;
    if (!gameObject.TryGetComponent<ZombieObjectDetector>(out component1))
      component1 = gameObject.AddComponent<ZombieObjectDetector>();
    ZombieObjectDetector_Report_TTY component2;
    if (!gameObject.TryGetComponent<ZombieObjectDetector_Report_TTY>(out component2))
      component2 = gameObject.AddComponent<ZombieObjectDetector_Report_TTY>();
    string str1 = Path.Combine(Log.LogsPath, "Zombies");
    DateTime utcNow = DateTime.UtcNow;
    try
    {
      Directory.CreateDirectory(str1);
    }
    catch (Exception ex)
    {
      Log.Asset.PrintInfo("Error creating CSV file directory: '" + str1 + "'\nError message: " + ex.Message);
      return false;
    }
    string str2 = utcNow.ToString("yy_MM_dd_hh_mm_ss");
    string path = Path.Combine(str1, str2 + "_zombies.csv");
    component2.InitOutputFile(path);
    component1.RunZombieObjectDetection();
    return true;
  }

  private bool OnProcessCheat_SendReport(string func, string[] args, string rawArgs)
  {
    string message = "USAGE: sendreport <account id> <complaint type> <subcomplaint type>";
    if (args.Length < 3 || ((IEnumerable<string>) args).Any<string>((Func<string, bool>) (a => string.IsNullOrEmpty(a))))
    {
      UIStatus.Get().AddInfo(message, 5f);
      return true;
    }
    string s = args[0];
    string str1 = args[1];
    string str2 = args[2];
    ulong low;
    ref ulong local = ref low;
    Blizzard.GameService.SDK.Client.Integration.ReportType.ComplaintType result1;
    Blizzard.GameService.SDK.Client.Integration.ReportType.SubcomplaintType result2;
    if (ulong.TryParse(s, out local) && System.Enum.TryParse<Blizzard.GameService.SDK.Client.Integration.ReportType.ComplaintType>(str1, out result1) && System.Enum.TryParse<Blizzard.GameService.SDK.Client.Integration.ReportType.SubcomplaintType>(str2, out result2))
      BattleNet.SubmitReport(new BnetAccountId(0UL, low), result1, new List<Blizzard.GameService.SDK.Client.Integration.ReportType.SubcomplaintType>()
      {
        result2
      });
    else
      UIStatus.Get().AddInfo("Invalid arguments provided.", 5f);
    return true;
  }

  private bool OnProgressCheat_MercDetails(string func, string[] args, string rawArgs)
  {
    MercenaryDetailDisplay objectOfType = UnityEngine.Object.FindObjectOfType<MercenaryDetailDisplay>();
    if ((UnityEngine.Object) objectOfType == (UnityEngine.Object) null)
    {
      UIStatus.Get().AddInfo("MercenaryDetailDisplay component not found. Please add a LettuceMercDetailsDisplay object into the scene", 5f);
      return false;
    }
    CollectionManager collectionManager = CollectionManager.Get();
    int length = args.Length;
    string s = length >= 1 ? args[0] : (string) null;
    string cardId = length >= 2 ? args[1] : (string) null;
    LettuceTeam lettuceTeam;
    if (string.IsNullOrEmpty(s))
    {
      lettuceTeam = collectionManager.GetTeams().FirstOrDefault<LettuceTeam>();
    }
    else
    {
      long result;
      if (long.TryParse(s, out result))
      {
        lettuceTeam = collectionManager.GetTeam(result);
      }
      else
      {
        UIStatus.Get().AddInfo("Failed to parse team ID as a number.", 5f);
        return false;
      }
    }
    LettuceMercenary merc = !string.IsNullOrEmpty(cardId) ? collectionManager.GetMercenary(cardId) : lettuceTeam.GetMercs().FirstOrDefault<LettuceMercenary>();
    if (merc == null)
    {
      UIStatus.Get().AddInfo("Could not find a valid Mercenary to display", 5f);
      return false;
    }
    LettuceTeamDataModel teamModel = new LettuceTeamDataModel();
    CollectionUtils.PopulateMercenariesTeamDataModel(teamModel, lettuceTeam);
    objectOfType.GetComponent<Widget>().BindDataModel((IDataModel) teamModel);
    objectOfType.Show(merc, editingTeam: lettuceTeam);
    return true;
  }

  public string CreateSpellSystemLogDirectory()
  {
    string path = Path.Combine(Log.LogsPath, "SpellSystem");
    try
    {
      Directory.CreateDirectory(path);
    }
    catch (Exception ex)
    {
      Log.Asset.PrintInfo("Error creating CSV file directory: '" + path + "'\nError message: " + ex.Message);
    }
    return path;
  }

  private bool OnProcessCheat_LogSpellUsage(string func, string[] args, string rawArgs)
  {
    string str = DateTime.UtcNow.ToString("yy_MM_dd_hh_mm_ss");
    string systemLogDirectory = this.CreateSpellSystemLogDirectory();
    SpellStatistics.LogCurrentSpellCounts(new FileInfo(Path.Combine(systemLogDirectory, str + "_spell_counts.csv")));
    SpellStatistics.LogUnpooledSpellAcquiredCounts(new FileInfo(Path.Combine(systemLogDirectory, str + "_unpooled_spell_total_acquisitions.csv")));
    SpellStatistics.LogPooledSpellAcquiredCounts(new FileInfo(Path.Combine(systemLogDirectory, str + "_pooled_spell_total_acquisitions.csv")));
    SpellStatistics.LogPoolExpansions(new FileInfo(Path.Combine(systemLogDirectory, str + "_pool_expansions.csv")));
    SpellStatistics.LogPoolExceedsMaxSize(new FileInfo(Path.Combine(systemLogDirectory, str + "_pool_exceeds_max_counts.csv")));
    return true;
  }

  private bool OnProcessCheat_AckAllNotices(string func, string[] args, string rawArgs)
  {
    NetCache.NetCacheProfileNotices cacheProfileNotices = NetCache.Get() != null ? NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>() : (NetCache.NetCacheProfileNotices) null;
    if (cacheProfileNotices == null || cacheProfileNotices.Notices == null)
      return false;
    UIStatus.Get().AddInfo("Acknowledging all notices, restart client to dismiss.");
    try
    {
      Network network = Network.Get();
      foreach (NetCache.ProfileNotice notice in cacheProfileNotices.Notices)
        network.AckNotice(notice.NoticeID);
    }
    catch (Exception ex)
    {
      Log.All.PrintWarning("Error acknowledging notices: " + ex.Message);
    }
    return true;
  }

  private bool OnProcessCheat_AppsFlyer(string func, string[] args, string rawArgs)
  {
    if (args[0] == "resetoptions")
    {
      UIStatus.Get().AddInfo("Resetting Apps Flyer Options");
      Options.Get().SetBool(Option.AF_FIRST_BOX_AFTER_TUTORIAL, false);
      Options.Get().SetBool(Option.AF_FIRST_PACK_OPENED, false);
      Options.Get().SetBool(Option.AF_FIRST_SHOP_VISIT, false);
      Options.Get().SetBool(Option.AF_FIRST_NON_TUTORIAL_GAME_START_TRADITIONAL, false);
      Options.Get().SetBool(Option.AF_FIRST_NON_TUTORIAL_GAME_START_BATTLEGROUNDS, false);
      Options.Get().SetBool(Option.AF_FIRST_NON_TUTORIAL_GAME_START_MERCENARIES, false);
      Options.Get().SetBool(Option.AF_REWARD_TRACK_EVENT, false);
    }
    return true;
  }

  private bool OnProcessCheat_SoundMono(string func, string[] args, string rawArgs)
  {
    if (args[0].ToLower() == "on")
    {
      if (Options.Get().GetBool(Option.SOUND_MONO_ENABLED))
      {
        UIStatus.Get().AddInfo("Mono sound already enabled");
        return true;
      }
      Options.Get().SetBool(Option.SOUND_MONO_ENABLED, true);
      UIStatus.Get().AddInfo("Mono: ON | Stereo: OFF");
    }
    else if (args[0].ToLower() == "off")
    {
      if (!Options.Get().GetBool(Option.SOUND_MONO_ENABLED))
      {
        UIStatus.Get().AddInfo("Mono sound already disabled");
        return true;
      }
      Options.Get().SetBool(Option.SOUND_MONO_ENABLED, false);
      UIStatus.Get().AddInfo("Mono: OFF | Stereo: ON");
    }
    return true;
  }

  private bool OnProgressCheat_TaskBoardCheat(string func, string[] args, string rawArgs)
  {
    int length = args.Length;
    string str = length > 0 ? args[0].ToLower() : string.Empty;
    List<string> args1 = length > 1 ? ((IEnumerable<string>) args).ToList<string>().GetRange(1, length - 1) : new List<string>();
    if (str == "search_all")
      return this.SetTaskBoardSearchAll(args1);
    UIStatus.Get().AddInfo("Task board cheat not known", 5f);
    return false;
  }

  private bool SetTaskBoardSearchAll(List<string> args)
  {
    LettuceVillageTaskCollection objectOfType = UnityEngine.Object.FindObjectOfType<LettuceVillageTaskCollection>();
    if ((UnityEngine.Object) objectOfType == (UnityEngine.Object) null)
    {
      UIStatus.Get().AddInfo("LettuceVillageTaskCollection component not found. Please load into the merc scene", 5f);
      return false;
    }
    bool result;
    if (bool.TryParse(args.Count >= 1 ? args[0].ToLower() : string.Empty, out result))
    {
      objectOfType.DoSearchAllMercData = result;
      objectOfType.RefreshVisuals();
      return true;
    }
    UIStatus.Get().AddInfo("Param not known. Use True or False", 5f);
    return false;
  }

  private enum QuickLaunchAvailability
  {
    OK,
    FINDING_GAME,
    ACTIVE_GAME,
    SCENE_TRANSITION,
    COLLECTION_NOT_READY,
  }

  private enum FriendListType
  {
    FRIEND,
    RECENT,
    NEARBY,
    FSG,
  }

  private class QuickLaunchState
  {
    public bool m_launching;
    public bool m_skipMulligan;
    public bool m_flipHeroes;
    public bool m_mirrorHeroes;
    public string m_opponentHeroCardId;
  }

  private struct NamedParam
  {
    public NamedParam(string param)
    {
      this.Text = param;
      this.Number = 0;
      int val;
      if (!GeneralUtils.TryParseInt(param, out val))
        return;
      this.Number = val;
    }

    public string Text { get; private set; }

    public int Number { get; private set; }

    public bool HasNumber => this.Number > 0;
  }

  public delegate void LogFormatFunc(string format, params object[] args);

  private enum SetAdventureProgressMode
  {
    Victory,
    Defeat,
    Progress,
  }
}
