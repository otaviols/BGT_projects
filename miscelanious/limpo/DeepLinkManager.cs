using Assets;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using Hearthstone.Core.Deeplinking;
using Hearthstone.DataModels;
using Hearthstone.InGameMessage;
using Hearthstone.Progression;
using PegasusShared;
using PegasusUtil;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeepLinkManager
{
  private static Dictionary<string, SceneMgr.Mode> modeMapping = new Dictionary<string, SceneMgr.Mode>()
  {
    {
      "hub",
      SceneMgr.Mode.HUB
    },
    {
      "play",
      SceneMgr.Mode.TOURNAMENT
    },
    {
      "ranked",
      SceneMgr.Mode.TOURNAMENT
    },
    {
      "adventure",
      SceneMgr.Mode.ADVENTURE
    },
    {
      "arena",
      SceneMgr.Mode.DRAFT
    },
    {
      "tb",
      SceneMgr.Mode.TAVERN_BRAWL
    },
    {
      "tavernbrawl",
      SceneMgr.Mode.TAVERN_BRAWL
    },
    {
      "packopening",
      SceneMgr.Mode.PACKOPENING
    },
    {
      "cm",
      SceneMgr.Mode.COLLECTIONMANAGER
    },
    {
      "collectionmanager",
      SceneMgr.Mode.COLLECTIONMANAGER
    },
    {
      "credits",
      SceneMgr.Mode.CREDITS
    },
    {
      "store",
      SceneMgr.Mode.HUB
    },
    {
      "fsg",
      SceneMgr.Mode.HUB
    },
    {
      "raf",
      SceneMgr.Mode.HUB
    },
    {
      "recruitafriend",
      SceneMgr.Mode.HUB
    },
    {
      "battlegrounds",
      SceneMgr.Mode.BACON
    },
    {
      "gamemode",
      SceneMgr.Mode.GAME_MODE
    },
    {
      "lettuce",
      SceneMgr.Mode.LETTUCE_VILLAGE
    },
    {
      "mercstore",
      SceneMgr.Mode.LETTUCE_VILLAGE
    },
    {
      "journal",
      SceneMgr.Mode.HUB
    },
    {
      "duels",
      SceneMgr.Mode.PVP_DUNGEON_RUN
    },
    {
      "practice",
      SceneMgr.Mode.ADVENTURE
    }
  };

  public static void TryExecuteDeepLinkOnStartup(bool fromUnpause)
  {
    DeepLinkManager.DeepLinkSource source = DeepLinkManager.DeepLinkSource.NONE;
    string[] deepLink = (string[]) null;
    string[] cheatsArgs = (string[]) null;
    DeeplinkService service;
    if (ServiceManager.TryGet<DeeplinkService>(out service))
      deepLink = service.GetDeeplink();
    else
      Log.DeepLink.PrintError("Could not get deeplink service!");
    if (deepLink != null && deepLink.Length != 0 && deepLink[0] != string.Empty)
    {
      source = DeepLinkManager.DeepLinkSource.PUSH_NOTIFICATION;
    }
    else
    {
      string[] commandLineArgs = HearthstoneApplication.CommandLineArgs;
      for (int end = 0; end < commandLineArgs.Length; ++end)
      {
        string str = commandLineArgs[end];
        DeepLinkManager.CommandLineVerbs commandLineVerbs = str == "--mode" ? DeepLinkManager.CommandLineVerbs.OPEN_MODE : (str == "--runcheats" ? DeepLinkManager.CommandLineVerbs.RUN_CHEATS : DeepLinkManager.CommandLineVerbs.NONE);
        if (commandLineVerbs != DeepLinkManager.CommandLineVerbs.NONE)
        {
          int start = ++end;
          while (end < commandLineArgs.Length && !commandLineArgs[end].StartsWith("-"))
            ++end;
          string[] strArray = commandLineArgs.Slice<string>(start, end);
          switch (commandLineVerbs)
          {
            case DeepLinkManager.CommandLineVerbs.OPEN_MODE:
              deepLink = strArray;
              source = DeepLinkManager.DeepLinkSource.COMMAND_LINE_ARGUMENTS;
              continue;
            case DeepLinkManager.CommandLineVerbs.RUN_CHEATS:
              cheatsArgs = strArray;
              continue;
            default:
              continue;
          }
        }
      }
    }
    Log.All.PrintDebug("Trying to execute deeplink '{0}' from source '{1}' (unpause:{2})", deepLink != null ? (object) string.Join(" ", deepLink) : (object) "null", (object) source.ToString(), (object) fromUnpause);
    if (deepLink == null || deepLink.Length == 0)
    {
      if (!fromUnpause && SceneMgr.Get().GetMode() == SceneMgr.Mode.LOGIN)
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    }
    else if (SetRotationManager.Get().ShouldShowSetRotationIntro() || FiresideGatheringManager.Get() != null && FiresideGatheringManager.Get().IsCheckedIn)
    {
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), false);
    }
    else if (!DeepLinkManager.ExecuteDeepLink(deepLink, source, fromUnpause))
    {
      if (!fromUnpause && SceneMgr.Get().GetMode() == SceneMgr.Mode.LOGIN)
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), false);
    }
    DeepLinkManager.ExecuteCheats(cheatsArgs);
  }

  public static bool TryParseUri(string uri, out string[] deepLinkArgs)
  {
    if (!uri.StartsWith("hearthstone://"))
    {
      deepLinkArgs = (string[]) null;
      return false;
    }
    deepLinkArgs = uri.Substring("hearthstone://".Length).Split('/');
    return true;
  }

  public static bool ExecuteDeepLink(
    string[] deepLink,
    DeepLinkManager.DeepLinkSource source,
    bool fromUnpause)
  {
    if (deepLink == null || deepLink.Length == 0)
      return false;
    string str = Vars.Key("Debug.OpenMode").GetStr(string.Empty);
    if (string.IsNullOrEmpty(str))
      str = deepLink[0];
    Action modeDelegate;
    switch (str)
    {
      case "adventure":
        modeDelegate = DeepLinkManager.ShowAdventure(deepLink, source);
        break;
      case "arena":
        modeDelegate = DeepLinkManager.ShowArena(deepLink, source);
        break;
      case "battlegrounds":
        modeDelegate = DeepLinkManager.ShowBattlegrounds(deepLink, source);
        break;
      case "cm":
      case "collectionmanager":
      case "credits":
      case "fsg":
      case "hub":
      case "packopening":
      case "play":
        modeDelegate = DeepLinkManager.ShowSceneMode(deepLink, source);
        break;
      case "duels":
        modeDelegate = DeepLinkManager.ShowDuelsMode(deepLink, source);
        break;
      case "gamemode":
        modeDelegate = DeepLinkManager.ShowGameMode(deepLink, source);
        break;
      case "journal":
        modeDelegate = DeepLinkManager.ShowJournal(deepLink, source);
        break;
      case "lettuce":
      case "mercenaries":
        modeDelegate = DeepLinkManager.ShowLettuce(deepLink, source);
        break;
      case "mercstore":
        modeDelegate = DeepLinkManager.ShowMercstore(deepLink, source);
        break;
      case "practice":
        modeDelegate = DeepLinkManager.ShowPracticeMode(deepLink, source);
        break;
      case "raf":
      case "recruitafriend":
        modeDelegate = DeepLinkManager.ShowRecruitAFriend(deepLink, source);
        break;
      case "ranked":
        modeDelegate = DeepLinkManager.ShowRankedMode(deepLink, source);
        break;
      case "store":
        modeDelegate = DeepLinkManager.ShowStore(deepLink, source);
        break;
      case "tavernbrawl":
      case "tb":
        modeDelegate = DeepLinkManager.ShowTavernBrawl(deepLink, source);
        break;
      default:
        return false;
    }
    if (modeDelegate != null)
      DeepLinkManager.GoToMode(modeDelegate, fromUnpause);
    return true;
  }

  private static void ExecuteCheats(string[] cheatsArgs)
  {
    string str = Vars.Key("Debug.RunCheats").GetStr(string.Empty);
    if (string.IsNullOrEmpty(str) && cheatsArgs != null)
      str = string.Join(" ", cheatsArgs);
    if (string.IsNullOrEmpty(str))
      return;
    Processor.RunCoroutine(DeepLinkManager.RunCheatCommands(str.Split(';')));
  }

  private static IEnumerator RunCheatCommands(string[] cheats)
  {
    string[] strArray = cheats;
    for (int index = 0; index < strArray.Length; ++index)
    {
      string inputCommand = strArray[index];
      CheatMgr.Get()?.ProcessCheat(inputCommand);
      yield return (object) new WaitForSecondsRealtime(0.5f);
    }
    strArray = (string[]) null;
  }

  private static void GoToMode(Action modeDelegate, bool fromUnpause)
  {
    if (!fromUnpause && SceneMgr.Get().GetMode() == SceneMgr.Mode.LOGIN)
    {
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(DeepLinkManager.OnSceneLoaded), (object) modeDelegate);
    }
    else if (!SceneMgr.Get().IsTransitioning())
      modeDelegate();
    else
      SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(DeepLinkManager.OnSceneLoaded), (object) modeDelegate);
  }

  private static Action ShowSceneMode(
    string[] deepLink,
    DeepLinkManager.DeepLinkSource source)
  {
    return (Action) (() =>
    {
      SceneMgr.Get().SetNextMode(DeepLinkManager.modeMapping[deepLink[0]]);
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
    });
  }

  private static Action ShowRankedMode(
    string[] deepLink,
    DeepLinkManager.DeepLinkSource source)
  {
    PegasusShared.FormatType formatType = PegasusShared.FormatType.FT_STANDARD;
    return deepLink.Length > 1 && !string.IsNullOrEmpty(deepLink[1]) && !EnumUtils.TryGetEnum<PegasusShared.FormatType>(deepLink[1], StringComparison.OrdinalIgnoreCase, out formatType) ? (Action) null : (Action) (() =>
    {
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.TOURNAMENT);
      Options.SetFormatType(formatType);
      Options.SetInRankedPlayMode(true);
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
    });
  }

  private static Action ShowDuelsMode(
    string[] deepLink,
    DeepLinkManager.DeepLinkSource source)
  {
    return (Action) (() =>
    {
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.PVP_DUNGEON_RUN);
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
    });
  }

  private static Action ShowPracticeMode(
    string[] deepLink,
    DeepLinkManager.DeepLinkSource source)
  {
    return (Action) (() =>
    {
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.ADVENTURE);
      AdventureConfig.Get().SetSelectedAdventureMode(AdventureDbId.PRACTICE, AdventureModeDbId.LINEAR);
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
    });
  }

  private static Action ShowJournal(string[] deepLink, DeepLinkManager.DeepLinkSource source)
  {
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.HUB)
      return (Action) null;
    JournalTrayDisplay.JournalTab journalTab = JournalTrayDisplay.JournalTab.Unknown;
    if (deepLink.Length <= 1 || string.IsNullOrEmpty(deepLink[1]))
      return (Action) null;
    return !EnumUtils.TryGetEnum<JournalTrayDisplay.JournalTab>(deepLink[1], StringComparison.OrdinalIgnoreCase, out journalTab) ? (Action) null : (Action) (() =>
    {
      JournalButton journalButton = Box.Get().GetJournalButton();
      if (!((UnityEngine.Object) journalButton != (UnityEngine.Object) null))
        return;
      if (!JournalPopup.s_isShowing)
      {
        JournalTrayDisplay.SetActiveTabForTrackType(Global.RewardTrackType.GLOBAL, journalTab);
        journalButton.ShowJournal();
        TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
      }
      else
      {
        JournalTrayDisplay journalTrayDisplay = journalButton.GetJournalTrayDisplay();
        if (!((UnityEngine.Object) journalTrayDisplay != (UnityEngine.Object) null))
          return;
        journalTrayDisplay.ForceChangeActiveTabViaDeepLink(journalTab);
        TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
      }
    });
  }

  private static Action ShowStore(string[] deepLink, DeepLinkManager.DeepLinkSource source)
  {
    if (StoreManager.Get().IsVintageStoreEnabled())
    {
      GeneralStoreMode mode = GeneralStoreMode.CARDS;
      if (deepLink.Length > 1)
      {
        string str = deepLink[1];
        AdventureDbId val1 = EnumUtils.SafeParse<AdventureDbId>(str, ignoreCase: true);
        HeroDbId val2 = EnumUtils.SafeParse<HeroDbId>(str, ignoreCase: true);
        int boosterId;
        StorePackType storePackType;
        DeepLinkManager.GetBoosterAndStorePackTypeFromGameAction(deepLink, out boosterId, out storePackType);
        if (boosterId != 0)
        {
          Options.Get().SetInt(Option.LAST_SELECTED_STORE_BOOSTER_ID, boosterId);
          Options.Get().SetInt(Option.LAST_SELECTED_STORE_PACK_TYPE, (int) storePackType);
          mode = GeneralStoreMode.CARDS;
        }
        else if (val2 != HeroDbId.INVALID)
        {
          Options.Get().SetInt(Option.LAST_SELECTED_STORE_HERO_ID, (int) val2);
          mode = GeneralStoreMode.HEROES;
        }
        else if (val1 != AdventureDbId.INVALID)
        {
          Options.Get().SetInt(Option.LAST_SELECTED_STORE_ADVENTURE_ID, (int) val1);
          mode = GeneralStoreMode.ADVENTURE;
        }
      }
      return (Action) (() =>
      {
        StoreManager.Get().StartGeneralTransaction(mode);
        TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
      });
    }
    long pmtProductId = 0;
    if (deepLink.Length > 1)
    {
      long.TryParse(deepLink[1], out pmtProductId);
      if (deepLink.Length > 2 && deepLink[2].ToLowerInvariant() != "pmt")
      {
        StorePackId storePackId;
        DeepLinkManager.GetBoosterAndStorePackTypeFromGameAction(deepLink, out storePackId.Id, out storePackId.Type);
        ProductType fromStorePackType = StorePackId.GetProductTypeFromStorePackType(storePackId);
        int dataFromStorePackId = GameUtils.GetProductDataFromStorePackId(storePackId);
        Network.Bundle bundle1 = StoreManager.Get().GetAllBundlesForProduct(fromStorePackType, false, dataFromStorePackId).FirstOrDefault<Network.Bundle>((Func<Network.Bundle, bool>) (bundle => StoreManager.Get().Catalog.GetTiers(ShopType.GENERAL_STORE).Any<ProductTierDataModel>((Func<ProductTierDataModel, bool>) (tier => tier.BrowserButtons.Any<ShopBrowserButtonDataModel>((Func<ShopBrowserButtonDataModel, bool>) (button => button.DisplayProduct.PmtId == bundle.PMTProductID.Value))))));
        pmtProductId = (Record) bundle1 != (Record) null ? bundle1.PMTProductID.Value : 0L;
      }
    }
    return (Action) (() =>
    {
      Shop.OpenToProductPageWhenReady(pmtProductId, false);
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
    });
  }

  private static Action ShowAdventure(
    string[] deepLink,
    DeepLinkManager.DeepLinkSource source)
  {
    AdventureDbId adventureId = AdventureDbId.INVALID;
    AdventureModeDbId adventureModeId = AdventureModeDbId.LINEAR;
    if (deepLink.Length > 1)
    {
      adventureId = EnumUtils.SafeParse<AdventureDbId>(deepLink[1], ignoreCase: true);
      adventureModeId = AdventureModeDbId.LINEAR;
      if (deepLink.Length > 2)
        adventureModeId = EnumUtils.SafeParse<AdventureModeDbId>(deepLink[2], AdventureModeDbId.LINEAR, true);
    }
    return (Action) (() =>
    {
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.ADVENTURE);
      if (adventureId != AdventureDbId.INVALID)
        AdventureConfig.Get().SetSelectedAdventureMode(adventureId, adventureModeId);
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
    });
  }

  private static Action ShowTavernBrawl(
    string[] deepLink,
    DeepLinkManager.DeepLinkSource source)
  {
    return (Action) (() =>
    {
      if (!TavernBrawlManager.Get().HasUnlockedTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL) || !TavernBrawlManager.Get().IsTavernBrawlActive(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
      {
        TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), false);
      }
      else
      {
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.TAVERN_BRAWL);
        TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
      }
    });
  }

  private static Action ShowArena(string[] deepLink, DeepLinkManager.DeepLinkSource source) => (Action) (() =>
  {
    if (AchieveManager.Get() != null && AchieveManager.Get().HasUnlockedArena() && AchieveManager.Get() != null && HealthyGamingMgr.Get().isArenaEnabled())
    {
      AchieveManager.Get().NotifyOfClick(Achievement.ClickTriggerType.BUTTON_ARENA);
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.DRAFT);
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
    }
    else
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), false);
  });

  private static Action ShowRecruitAFriend(
    string[] deepLink,
    DeepLinkManager.DeepLinkSource source)
  {
    return (Action) (() =>
    {
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
      RAFManager.Get().ShowRAFFrame();
    });
  }

  private static Action ShowBattlegrounds(
    string[] deepLink,
    DeepLinkManager.DeepLinkSource source)
  {
    return (Action) (() =>
    {
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.BACON);
    });
  }

  private static Action ShowGameMode(string[] deepLink, DeepLinkManager.DeepLinkSource source) => (Action) (() =>
  {
    TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAME_MODE);
  });

  private static Action ShowLettuce(string[] deepLink, DeepLinkManager.DeepLinkSource source) => (Action) (() =>
  {
    TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE);
  });

  private static Action ShowMercstore(
    string[] deepLink,
    DeepLinkManager.DeepLinkSource source)
  {
    if (!MercenaryMessageUtils.HasCompletedMercenaryVillageShopTutorial())
      return DeepLinkManager.ShowStore(deepLink, source);
    long pmtProductId = 0;
    if (deepLink.Length > 1)
    {
      long.TryParse(deepLink[1], out pmtProductId);
      if (deepLink.Length > 2 && deepLink[2].ToLowerInvariant() != "pmt")
      {
        StorePackId storePackId;
        DeepLinkManager.GetBoosterAndStorePackTypeFromGameAction(deepLink, out storePackId.Id, out storePackId.Type);
        ProductType fromStorePackType = StorePackId.GetProductTypeFromStorePackType(storePackId);
        int dataFromStorePackId = GameUtils.GetProductDataFromStorePackId(storePackId);
        Network.Bundle bundle1 = StoreManager.Get().GetAllBundlesForProduct(fromStorePackType, false, dataFromStorePackId).FirstOrDefault<Network.Bundle>((Func<Network.Bundle, bool>) (bundle => StoreManager.Get().Catalog.GetTiers(ShopType.MERCENARIES_STORE).Any<ProductTierDataModel>((Func<ProductTierDataModel, bool>) (tier => tier.BrowserButtons.Any<ShopBrowserButtonDataModel>((Func<ShopBrowserButtonDataModel, bool>) (button => button.DisplayProduct.PmtId == bundle.PMTProductID.Value))))));
        pmtProductId = (Record) bundle1 != (Record) null ? bundle1.PMTProductID.Value : 0L;
      }
    }
    Action goToStoreDelegate = (Action) (() =>
    {
      Shop.OpenToMercProductPageWhenReady(pmtProductId, false);
      TelemetryManager.Client().SendDeepLinkExecuted(string.Join(" ", deepLink), source.ToString(), true);
    });
    return SceneMgr.Get().GetMode() == SceneMgr.Mode.LETTUCE_VILLAGE ? goToStoreDelegate : (Action) (() =>
    {
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE);
      SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(DeepLinkManager.OnMercVillageSceneLoaded), (object) goToStoreDelegate);
    });
  }

  private static void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode != SceneMgr.Mode.HUB)
      return;
    ((Action) userData)();
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(DeepLinkManager.OnSceneLoaded), userData);
  }

  private static void OnMercVillageSceneLoaded(
    SceneMgr.Mode mode,
    PegasusScene scene,
    object userData)
  {
    if (mode != SceneMgr.Mode.LETTUCE_VILLAGE)
      return;
    ((Action) userData)();
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(DeepLinkManager.OnSceneLoaded), userData);
  }

  public static void GetBoosterAndStorePackTypeFromGameAction(
    string[] actionTokens,
    out int boosterId,
    out StorePackType storePackType)
  {
    string actionToken = actionTokens[1];
    storePackType = StorePackType.BOOSTER;
    if (actionTokens.Length > 2)
      storePackType = EnumUtils.SafeParse<StorePackType>(actionTokens[2], StorePackType.BOOSTER, true);
    boosterId = storePackType == StorePackType.MODULAR_BUNDLE ? int.Parse(actionToken) : (int) EnumUtils.SafeParse<BoosterDbId>(actionToken, ignoreCase: true);
  }

  public enum DeepLinkSource
  {
    NONE,
    PUSH_NOTIFICATION,
    COMMAND_LINE_ARGUMENTS,
    INNKEEPERS_SPECIAL,
    IN_GAME_MESSAGE,
    QUEST,
    LOCKED_HERO_TOOLTIP,
  }

  private enum CommandLineVerbs
  {
    NONE,
    OPEN_MODE,
    RUN_CHEATS,
  }
}
