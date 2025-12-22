using Blizzard.T5.Core;
using Hearthstone;
using Hearthstone.Core;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameSaveDataManager
{
  private static int s_clientToken = 0;
  private static readonly Map<TAG_CLASS, GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys> AdventureDungeonCrawlClassToSubkeyMapping = new Map<TAG_CLASS, GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys>()
  {
    {
      TAG_CLASS.PALADIN,
      new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys()
      {
        bossWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_PALADIN_BOSS_WINS,
        runWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_PALADIN_RUN_WINS
      }
    },
    {
      TAG_CLASS.HUNTER,
      new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys()
      {
        bossWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HUNTER_BOSS_WINS,
        runWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HUNTER_RUN_WINS
      }
    },
    {
      TAG_CLASS.MAGE,
      new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys()
      {
        bossWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_MAGE_BOSS_WINS,
        runWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_MAGE_RUN_WINS
      }
    },
    {
      TAG_CLASS.SHAMAN,
      new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys()
      {
        bossWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_SHAMAN_BOSS_WINS,
        runWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_SHAMAN_RUN_WINS
      }
    },
    {
      TAG_CLASS.WARRIOR,
      new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys()
      {
        bossWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_WARRIOR_BOSS_WINS,
        runWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_WARRIOR_RUN_WINS
      }
    },
    {
      TAG_CLASS.ROGUE,
      new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys()
      {
        bossWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_ROGUE_BOSS_WINS,
        runWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_ROGUE_RUN_WINS
      }
    },
    {
      TAG_CLASS.WARLOCK,
      new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys()
      {
        bossWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_WARLOCK_BOSS_WINS,
        runWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_WARLOCK_RUN_WINS
      }
    },
    {
      TAG_CLASS.PRIEST,
      new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys()
      {
        bossWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_PRIEST_BOSS_WINS,
        runWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_PRIEST_RUN_WINS
      }
    },
    {
      TAG_CLASS.DRUID,
      new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys()
      {
        bossWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_DRUID_BOSS_WINS,
        runWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_DRUID_RUN_WINS
      }
    },
    {
      TAG_CLASS.DEMONHUNTER,
      new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys()
      {
        bossWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_DEMON_HUNTER_BOSS_WINS,
        runWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_DEMON_HUNTER_RUN_WINS
      }
    }
  };
  private static readonly Map<int, GameSaveKeySubkeyId> AdventureDungeonCrawlGuestHeroToBossWinSubkeyMapping = new Map<int, GameSaveKeySubkeyId>()
  {
    {
      162,
      GameSaveKeySubkeyId.PVPDR_DIABLO_BOSS_WINS
    },
    {
      329,
      GameSaveKeySubkeyId.PVPDR_VANNDAR_BOSS_WINS
    },
    {
      333,
      GameSaveKeySubkeyId.PVPDR_DREKTHAR_BOSS_WINS
    },
    {
      345,
      GameSaveKeySubkeyId.PVPDR_FINLEY_BOSS_WINS
    },
    {
      346,
      GameSaveKeySubkeyId.PVPDR_ELISE_BOSS_WINS
    },
    {
      347,
      GameSaveKeySubkeyId.PVPDR_BRANN_BOSS_WINS
    },
    {
      348,
      GameSaveKeySubkeyId.PVPDR_RENO_BOSS_WINS
    },
    {
      362,
      GameSaveKeySubkeyId.PVPDR_DARIOUS_BOSS_WINS
    },
    {
      363,
      GameSaveKeySubkeyId.PVPDR_SHAW_BOSS_WINS
    },
    {
      364,
      GameSaveKeySubkeyId.PVPDR_TESS_BOSS_WINS
    },
    {
      369,
      GameSaveKeySubkeyId.PVPDR_SAI_BOSS_WINS
    },
    {
      370,
      GameSaveKeySubkeyId.PVPDR_SCARLET_BOSS_WINS
    }
  };
  private static readonly List<GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys> ProgressSubkeysForDungeonCrawlWings = new List<GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys>()
  {
    new GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys()
    {
      heroCardWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_CARD_WING_1_WINS,
      deckWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_WING_1_WINS,
      heroPowerWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_POWER_WING_1_WINS,
      treasureWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_TREASURE_WING_1_WINS
    },
    new GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys()
    {
      heroCardWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_CARD_WING_2_WINS,
      deckWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_WING_2_WINS,
      heroPowerWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_POWER_WING_2_WINS,
      treasureWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_TREASURE_WING_2_WINS
    },
    new GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys()
    {
      heroCardWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_CARD_WING_3_WINS,
      deckWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_WING_3_WINS,
      heroPowerWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_POWER_WING_3_WINS,
      treasureWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_TREASURE_WING_3_WINS
    },
    new GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys()
    {
      heroCardWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_CARD_WING_4_WINS,
      deckWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_WING_4_WINS,
      heroPowerWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_POWER_WING_4_WINS,
      treasureWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_TREASURE_WING_4_WINS
    },
    new GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys()
    {
      heroCardWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_CARD_WING_5_WINS,
      deckWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_WING_5_WINS,
      heroPowerWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_POWER_WING_5_WINS,
      treasureWins = GameSaveKeySubkeyId.DUNGEON_CRAWL_TREASURE_WING_5_WINS
    }
  };
  private const float BATCHED_SAVE_SUBKEY_REQUEST_RATE = 1f;
  private static GameSaveDataManager s_instance = (GameSaveDataManager) null;
  private Map<GameSaveKeyId, Map<GameSaveKeySubkeyId, GameSaveDataValue>> m_gameSaveDataMapByKey = new Map<GameSaveKeyId, Map<GameSaveKeySubkeyId, GameSaveDataValue>>();
  private Dictionary<GameSaveKeyId, bool> m_isRequestPendingForKey;
  private Dictionary<int, GameSaveDataManager.PendingRequestContext> m_pendingRequestsByClientToken = new Dictionary<int, GameSaveDataManager.PendingRequestContext>();
  private List<GameSaveDataUpdate> m_batchedSaveUpdates = new List<GameSaveDataUpdate>();
  private List<GameSaveDataManager.SubkeySaveRequest> m_batchedSubkeySaveRequests = new List<GameSaveDataManager.SubkeySaveRequest>();
  private List<GameSaveDataManager.OnSaveDataResponseDelegate> m_batchedSaveUpdateCallbacks = new List<GameSaveDataManager.OnSaveDataResponseDelegate>();
  private DateTime m_timeOfLastSetGameSaveDataRequest;

  public static bool IsGameSaveKeyValid(GameSaveKeyId key) => GameSaveKeyId.INVALID != key && key != 0;

  public bool IsDataReady(GameSaveKeyId key)
  {
    if (GameSaveDataManager.IsGameSaveKeyValid(key))
      return this.m_gameSaveDataMapByKey.ContainsKey(key);
    Debug.LogWarning((object) "GameSaveDataManager.IsDataReady() called with an invalid key ID!");
    return false;
  }

  public GameSaveDataManager()
  {
    Network.Get().RegisterNetHandler((object) GameSaveDataResponse.PacketID.ID, new Network.NetHandler(this.OnRequestGameSaveDataResponse));
    Network.Get().RegisterNetHandler((object) SetGameSaveDataResponse.PacketID.ID, new Network.NetHandler(this.OnSetGameSaveDataResponse));
    Network.Get().RegisterNetHandler((object) GameSaveDataStateUpdate.PacketID.ID, new Network.NetHandler(this.OnGameSaveDataStateUpdate));
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    this.HandleGameSaveDataMigration();
    HearthstoneApplication.Get().WillReset += new Action(GameSaveDataManager.OnWillReset);
    this.m_timeOfLastSetGameSaveDataRequest = DateTime.Now;
  }

  private void OnRequestGameSaveDataResponse()
  {
    bool flag = false;
    GameSaveDataResponse saveDataResponse = Network.Get().GetGameSaveDataResponse();
    if (saveDataResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.All.PrintError("GameSaveDataManager.OnRequestGameSaveDataResponse() - GameSaveDataResponse has error code {0} (error #{1})", (object) saveDataResponse.ErrorCode, (object) (int) saveDataResponse.ErrorCode);
      flag = true;
    }
    if (!flag)
      this.ReadGameSaveDataUpdates(saveDataResponse.Data);
    GameSaveDataManager.PendingRequestContext pendingRequestContext;
    if (!this.m_pendingRequestsByClientToken.TryGetValue(saveDataResponse.ClientToken, out pendingRequestContext))
      return;
    this.m_pendingRequestsByClientToken.Remove(saveDataResponse.ClientToken);
    if (pendingRequestContext.RequestCallback == null)
      return;
    pendingRequestContext.RequestCallback(!flag);
  }

  private void OnSetGameSaveDataResponse()
  {
    bool flag = false;
    SetGameSaveDataResponse saveDataResponse = Network.Get().GetSetGameSaveDataResponse();
    if (saveDataResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      Log.All.PrintError("GameSaveDataManager.OnSetGameSaveDataResponse() - SetGameSaveDataResponse has error code {0}", (object) saveDataResponse.ErrorCode);
      flag = true;
    }
    if (!flag)
      this.ReadGameSaveDataUpdates(saveDataResponse.Data);
    GameSaveDataManager.PendingRequestContext pendingRequestContext;
    if (!this.m_pendingRequestsByClientToken.TryGetValue(saveDataResponse.ClientToken, out pendingRequestContext))
      return;
    this.m_pendingRequestsByClientToken.Remove(saveDataResponse.ClientToken);
    if (pendingRequestContext.SaveCallback == null)
      return;
    pendingRequestContext.SaveCallback(!flag);
  }

  private void OnGameSaveDataStateUpdate()
  {
    GameSaveDataStateUpdate saveDataStateUpdate = Network.Get().GetGameSaveDataStateUpdate();
    if (saveDataStateUpdate == null)
      Debug.LogError((object) "OnGameSaveDataStateUpdate(): No response received.");
    else
      GameSaveDataManager.Get().ApplyGameSaveDataUpdate(saveDataStateUpdate.GameSaveData);
  }

  private void HandleGameSaveDataMigration()
  {
    List<GameSaveDataManager.SubkeySaveRequest> requests = new List<GameSaveDataManager.SubkeySaveRequest>();
    foreach (KeyValuePair<Option, GameSaveDataManager.ServerOptionFlagMigrationData> keyValuePair in new Dictionary<Option, GameSaveDataManager.ServerOptionFlagMigrationData>()
    {
      {
        Option.HAS_SEEN_LOOT_BOSS_HERO_POWER,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSS_HERO_POWER_TUTORIAL_PROGRESS, 2)
      },
      {
        Option.HAS_SEEN_LOOT_COMPLETE_ALL_CLASSES_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_COMPLETE_ALL_CLASSES_VO)
      },
      {
        Option.HAS_SEEN_LATEST_DUNGEON_RUN_COMPLETE,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_SERVER_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_LATEST_DUNGEON_RUN_COMPLETE)
      },
      {
        Option.HAS_SEEN_LOOT_CHARACTER_SELECT_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_CHARACTER_SELECT_VO)
      },
      {
        Option.HAS_SEEN_LOOT_WELCOME_BANNER_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_WELCOME_BANNER_VO)
      },
      {
        Option.HAS_SEEN_LOOT_BOSS_FLIP_1_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_1_VO)
      },
      {
        Option.HAS_SEEN_LOOT_BOSS_FLIP_2_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_2_VO)
      },
      {
        Option.HAS_SEEN_LOOT_BOSS_FLIP_3_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_BOSS_FLIP_3_VO)
      },
      {
        Option.HAS_SEEN_LOOT_OFFER_TREASURE_1_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_TREASURE_1_VO)
      },
      {
        Option.HAS_SEEN_LOOT_OFFER_LOOT_PACKS_1_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_LOOT_PACKS_1_VO)
      },
      {
        Option.HAS_SEEN_LOOT_OFFER_LOOT_PACKS_2_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_OFFER_LOOT_PACKS_2_VO)
      },
      {
        Option.HAS_SEEN_LOOT_IN_GAME_WIN_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_WIN_VO)
      },
      {
        Option.HAS_SEEN_LOOT_IN_GAME_LOSE_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_VO)
      },
      {
        Option.HAS_SEEN_LOOT_IN_GAME_MULLIGAN_1_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_1_VO)
      },
      {
        Option.HAS_SEEN_LOOT_IN_GAME_MULLIGAN_2_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_MULLIGAN_2_VO)
      },
      {
        Option.HAS_SEEN_LOOT_IN_GAME_LOSE_2_VO,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_IN_GAME_LOSE_2_VO)
      },
      {
        Option.HAS_SEEN_NAXX,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_NAXX, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_ADVENTURE)
      },
      {
        Option.HAS_SEEN_BRM,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_BRM, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_ADVENTURE)
      },
      {
        Option.HAS_SEEN_LOE,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOE, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_ADVENTURE)
      },
      {
        Option.HAS_SEEN_KARA,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_KARA, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_ADVENTURE)
      },
      {
        Option.HAS_SEEN_ICC,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_ICC, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_ADVENTURE)
      },
      {
        Option.HAS_SEEN_LOOT,
        new GameSaveDataManager.ServerOptionFlagMigrationData(GameSaveKeyId.ADVENTURE_DATA_CLIENT_LOOT, GameSaveKeySubkeyId.ADVENTURE_HAS_SEEN_ADVENTURE)
      }
    })
    {
      Option key = keyValuePair.Key;
      GameSaveKeyId keyId = keyValuePair.Value.KeyId;
      GameSaveKeySubkeyId subkeyId = keyValuePair.Value.SubkeyId;
      int flagTrueValue = keyValuePair.Value.FlagTrueValue;
      int flagFalseValue = keyValuePair.Value.FlagFalseValue;
      if (Options.Get().HasOption(key))
      {
        int num = Options.Get().GetBool(key) ? flagTrueValue : flagFalseValue;
        requests.Add(new GameSaveDataManager.SubkeySaveRequest(keyId, subkeyId, new long[1]
        {
          (long) num
        }));
        Options.Get().DeleteOption(key);
      }
    }
    if (requests.Count <= 0)
      return;
    this.SaveSubkeys(requests);
  }

  public void ApplyGameSaveDataUpdate(GameSaveDataUpdate gameSaveDataUpdate)
  {
    if (gameSaveDataUpdate == null || gameSaveDataUpdate.Tuple.Count <= 0)
      return;
    this.ReadGameSaveDataUpdates(new List<GameSaveDataUpdate>()
    {
      gameSaveDataUpdate
    }, false);
  }

  public void ApplyGameSaveDataFromInitialClientState()
  {
    InitialClientState initialClientState = Network.Get().GetInitialClientState();
    if (initialClientState?.GameSaveData == null)
      return;
    this.ReadGameSaveDataUpdates(initialClientState.GameSaveData);
  }

  private void ReadGameSaveDataUpdates(
    List<GameSaveDataUpdate> gameSaveDataUpdate,
    bool overrideExisting = true)
  {
    using (List<GameSaveDataUpdate>.Enumerator enumerator = gameSaveDataUpdate.GetEnumerator())
    {
label_13:
      while (enumerator.MoveNext())
      {
        GameSaveDataUpdate current = enumerator.Current;
        if (current.Tuple.Count < 1)
        {
          Log.All.PrintWarning("GameSaveDataManager.ReadGameSaveDataUpdates() - Received update that contains no key");
        }
        else
        {
          GameSaveKeyId id1 = (GameSaveKeyId) current.Tuple[0].Id;
          if (overrideExisting || !this.m_gameSaveDataMapByKey.ContainsKey(id1) || this.m_gameSaveDataMapByKey[id1] == null)
            this.m_gameSaveDataMapByKey[id1] = new Map<GameSaveKeySubkeyId, GameSaveDataValue>();
          if (!current.HasValue)
            Log.All.Print("GameSaveDataManager.ReadGameSaveDataUpdates() - Received update that contains no data for the requested key {0}", (object) id1);
          else if (current.Value.MapKeys.Count == 0 && current.Value.MapValues.Count == 0)
          {
            GameSaveKeySubkeyId id2 = (GameSaveKeySubkeyId) current.Tuple[1].Id;
            this.m_gameSaveDataMapByKey[id1][id2] = current.Value;
          }
          else
          {
            int index = 0;
            while (true)
            {
              if (index < current.Value.MapKeys.Count && index < current.Value.MapValues.Count)
              {
                GameSaveKeySubkeyId mapKey = (GameSaveKeySubkeyId) current.Value.MapKeys[index];
                this.m_gameSaveDataMapByKey[id1][mapKey] = current.Value.MapValues[index];
                ++index;
              }
              else
                goto label_13;
            }
          }
        }
      }
    }
  }

  private bool ValidateThereAreNoPendingRequestsForKey(GameSaveKeyId key, string loggingContext)
  {
    if (!this.IsRequestPending(key))
      return true;
    Log.All.PrintError("GameSaveDataManager.{0}() - Detected pending operation for key {1}", (object) loggingContext, (object) key);
    return false;
  }

  public void Request(
    GameSaveKeyId key,
    GameSaveDataManager.OnRequestDataResponseDelegate callback = null)
  {
    this.Request(new List<GameSaveKeyId>() { key }, callback);
  }

  public void Request(
    List<GameSaveKeyId> keys,
    GameSaveDataManager.OnRequestDataResponseDelegate callback = null)
  {
    List<long> keys1 = new List<long>();
    int num = ++GameSaveDataManager.s_clientToken;
    foreach (GameSaveKeyId key in keys)
    {
      if (this.ValidateThereAreNoPendingRequestsForKey(key, nameof (Request)))
        keys1.Add((long) key);
    }
    if (keys1.Count > 0)
    {
      this.m_pendingRequestsByClientToken.Add(num, new GameSaveDataManager.PendingRequestContext(keys, callback));
      Network.Get().RequestGameSaveData(keys1, num);
    }
    else
    {
      if (callback == null)
        return;
      callback(false);
    }
  }

  public bool SaveSubkey(
    GameSaveDataManager.SubkeySaveRequest request,
    GameSaveDataManager.OnSaveDataResponseDelegate callback = null)
  {
    return this.SaveSubkeys(new List<GameSaveDataManager.SubkeySaveRequest>()
    {
      request
    }, callback);
  }

  public bool SaveSubkeys(
    List<GameSaveDataManager.SubkeySaveRequest> requests,
    GameSaveDataManager.OnSaveDataResponseDelegate callback = null)
  {
    if (requests == null || requests.Count == 0)
    {
      Log.All.PrintError("GameSaveDataManager.SaveSubkeys() - No save requests specified");
      return false;
    }
    HashSet<GameSaveDataManager.GameSaveKeyTuple> gameSaveKeyTupleSet = new HashSet<GameSaveDataManager.GameSaveKeyTuple>();
    foreach (GameSaveDataManager.SubkeySaveRequest request in requests)
    {
      GameSaveDataManager.GameSaveKeyTuple gameSaveKeyTuple = new GameSaveDataManager.GameSaveKeyTuple(request.Key, request.Subkey);
      if (gameSaveKeyTupleSet.Contains(gameSaveKeyTuple))
      {
        Log.All.PrintError("GameSaveDataManager.SaveSubkeys() - Found multiple save requests for key {0} subkey {1}", (object) request.Key, (object) request.Subkey);
        return false;
      }
      gameSaveKeyTupleSet.Add(gameSaveKeyTuple);
    }
    List<GameSaveDataUpdate> saveUpdates = new List<GameSaveDataUpdate>();
    foreach (GameSaveDataManager.SubkeySaveRequest request in requests)
    {
      if (this.ValidateThereAreNoPendingRequestsForKey(request.Key, nameof (SaveSubkeys)))
      {
        GameSaveDataUpdate gameSaveDataUpdate = new GameSaveDataUpdate();
        GameSaveDataValue gameSaveDataValue = new GameSaveDataValue();
        gameSaveDataUpdate.Tuple.Add(new GameSaveKey()
        {
          Id = (long) request.Key
        });
        gameSaveDataUpdate.Tuple.Add(new GameSaveKey()
        {
          Id = (long) request.Subkey
        });
        this.SetGameSaveDataValueFromRequest(request, ref gameSaveDataValue);
        gameSaveDataUpdate.Value = gameSaveDataValue;
        saveUpdates.Add(gameSaveDataUpdate);
        this.SaveSubkeyToLocalCache(request);
        this.m_batchedSubkeySaveRequests.Add(request);
      }
    }
    if (callback != null && saveUpdates.Count > 0)
      this.m_batchedSaveUpdateCallbacks.Add(callback);
    this.BatchGameSaveUpdates(saveUpdates);
    return saveUpdates.Count > 0;
  }

  private void SetGameSaveDataValueFromRequest(
    GameSaveDataManager.SubkeySaveRequest request,
    ref GameSaveDataValue value)
  {
    if (request.Long_Values != null && request.String_Values != null)
      Log.All.PrintError("Error writing game save data: Attempting to write Long and String into the same key!");
    else if (request.Long_Values != null)
    {
      value.IntValue = ((IEnumerable<long>) request.Long_Values).ToList<long>();
    }
    else
    {
      if (request.String_Values == null)
        return;
      value.StringValue = ((IEnumerable<string>) request.String_Values).ToList<string>();
    }
  }

  private void BatchGameSaveUpdates(List<GameSaveDataUpdate> saveUpdates)
  {
    if (this.m_batchedSaveUpdates.Count == 0)
    {
      if ((DateTime.Now - this.m_timeOfLastSetGameSaveDataRequest).TotalSeconds > 1.0)
        Processor.RunCoroutine(this.SendAllBatchedGameSaveUpdatesNextFrame());
      else
        Processor.ScheduleCallback(1f, false, new Processor.ScheduledCallback(this.SendAllBatchedGameSaveDataUpdates));
    }
    foreach (GameSaveDataUpdate saveUpdate in saveUpdates)
    {
      GameSaveDataUpdate update = saveUpdate;
      GameSaveDataUpdate gameSaveDataUpdate = this.m_batchedSaveUpdates.FirstOrDefault<GameSaveDataUpdate>((Func<GameSaveDataUpdate, bool>) (u => u.Tuple[0].Id == update.Tuple[0].Id && u.Tuple[1].Id == update.Tuple[1].Id));
      if (gameSaveDataUpdate != null)
        this.m_batchedSaveUpdates.Remove(gameSaveDataUpdate);
      this.m_batchedSaveUpdates.Add(update);
    }
  }

  public GameSaveDataManager.SubkeySaveRequest GenerateSaveRequestToAddValuesToSubkeyIfTheyDoNotExist(
    GameSaveKeyId key,
    GameSaveKeySubkeyId subkeyId,
    List<long> valuesToAdd)
  {
    if (valuesToAdd == null)
      return (GameSaveDataManager.SubkeySaveRequest) null;
    List<long> values;
    this.GetSubkeyValue(key, subkeyId, out values);
    if (values == null)
      values = new List<long>();
    bool flag = false;
    foreach (long num in valuesToAdd)
    {
      if (!values.Contains(num))
      {
        values.Add(num);
        flag = true;
      }
    }
    return !flag ? (GameSaveDataManager.SubkeySaveRequest) null : new GameSaveDataManager.SubkeySaveRequest(key, subkeyId, values.ToArray());
  }

  public GameSaveDataManager.SubkeySaveRequest GenerateSaveRequestToRemoveValueFromSubkeyIfItExists(
    GameSaveKeyId key,
    GameSaveKeySubkeyId subkeyId,
    long valueToRemove)
  {
    List<long> values;
    if (!this.GetSubkeyValue(key, subkeyId, out values))
      return (GameSaveDataManager.SubkeySaveRequest) null;
    if (values == null)
      return (GameSaveDataManager.SubkeySaveRequest) null;
    return !values.Remove(valueToRemove) ? (GameSaveDataManager.SubkeySaveRequest) null : new GameSaveDataManager.SubkeySaveRequest(key, subkeyId, values.ToArray());
  }

  private IEnumerator SendAllBatchedGameSaveUpdatesNextFrame()
  {
    yield return (object) new WaitForEndOfFrame();
    this.SendAllBatchedGameSaveDataUpdates((object) null);
  }

  private void SendAllBatchedGameSaveDataUpdates(object userdata)
  {
    int num = ++GameSaveDataManager.s_clientToken;
    foreach (GameSaveDataManager.OnSaveDataResponseDelegate saveUpdateCallback in this.m_batchedSaveUpdateCallbacks)
      this.m_pendingRequestsByClientToken.Add(num, new GameSaveDataManager.PendingRequestContext(this.m_batchedSubkeySaveRequests, saveUpdateCallback));
    Network.Get().SetGameSaveData(this.m_batchedSaveUpdates, num);
    this.m_timeOfLastSetGameSaveDataRequest = DateTime.Now;
    this.m_batchedSaveUpdates.Clear();
    this.m_batchedSubkeySaveRequests.Clear();
    this.m_batchedSaveUpdateCallbacks.Clear();
  }

  private void SaveSubkeyToLocalCache(GameSaveDataManager.SubkeySaveRequest request)
  {
    Map<GameSaveKeySubkeyId, GameSaveDataValue> map;
    if (!this.m_gameSaveDataMapByKey.TryGetValue(request.Key, out map))
    {
      map = new Map<GameSaveKeySubkeyId, GameSaveDataValue>();
      this.m_gameSaveDataMapByKey.Add(request.Key, map);
    }
    GameSaveDataValue gameSaveDataValue;
    if (!map.TryGetValue(request.Subkey, out gameSaveDataValue))
    {
      gameSaveDataValue = new GameSaveDataValue();
      map.Add(request.Subkey, gameSaveDataValue);
    }
    this.SetGameSaveDataValueFromRequest(request, ref gameSaveDataValue);
  }

  public bool GetSubkeyValue(GameSaveKeyId key, GameSaveKeySubkeyId subkeyId, out long value)
  {
    value = 0L;
    List<long> values;
    if (!this.GetSubkeyValue(key, subkeyId, out values))
      return false;
    value = values[0];
    return true;
  }

  public bool GetSubkeyValue(GameSaveKeyId key, GameSaveKeySubkeyId subkeyId, out string value)
  {
    value = "";
    List<string> values;
    if (!this.GetSubkeyValue(key, subkeyId, out values))
      return false;
    value = values[0];
    return true;
  }

  public bool GetSubkeyValue(
    GameSaveKeyId key,
    GameSaveKeySubkeyId subkeyId,
    out List<long> values)
  {
    values = (List<long>) null;
    GameSaveDataValue subkeyValue = this.GetSubkeyValue(key, subkeyId);
    if (subkeyValue == null || subkeyValue.IntValue.Count <= 0)
      return false;
    values = new List<long>((IEnumerable<long>) subkeyValue.IntValue);
    return true;
  }

  public bool GetSubkeyValue(GameSaveKeyId key, GameSaveKeySubkeyId subkeyId, List<long> values)
  {
    if (values == null)
      values = new List<long>();
    values.Clear();
    GameSaveDataValue subkeyValue = this.GetSubkeyValue(key, subkeyId);
    if (subkeyValue == null || subkeyValue.IntValue.Count <= 0)
      return false;
    values.AddRange((IEnumerable<long>) subkeyValue.IntValue);
    return true;
  }

  public bool GetSubkeyValue(
    GameSaveKeyId key,
    GameSaveKeySubkeyId subkeyId,
    out List<string> values)
  {
    values = (List<string>) null;
    GameSaveDataValue subkeyValue = this.GetSubkeyValue(key, subkeyId);
    if (subkeyValue == null || subkeyValue.StringValue.Count <= 0)
      return false;
    values = new List<string>((IEnumerable<string>) subkeyValue.StringValue);
    return true;
  }

  public bool GetSubkeyValue(
    GameSaveKeyId key,
    GameSaveKeySubkeyId subkeyId,
    out List<double> values)
  {
    values = (List<double>) null;
    GameSaveDataValue subkeyValue = this.GetSubkeyValue(key, subkeyId);
    if (subkeyValue == null || subkeyValue.FloatValue.Count <= 0)
      return false;
    values = new List<double>((IEnumerable<double>) subkeyValue.FloatValue);
    return true;
  }

  public List<GameSaveKeySubkeyId> GetAllSubkeysForKey(GameSaveKeyId key)
  {
    List<GameSaveKeySubkeyId> allSubkeysForKey = new List<GameSaveKeySubkeyId>();
    Map<GameSaveKeySubkeyId, GameSaveDataValue> map = (Map<GameSaveKeySubkeyId, GameSaveDataValue>) null;
    if (this.m_gameSaveDataMapByKey.TryGetValue(key, out map))
      allSubkeysForKey = new List<GameSaveKeySubkeyId>((IEnumerable<GameSaveKeySubkeyId>) map.Keys);
    return allSubkeysForKey;
  }

  public void ClearLocalData(GameSaveKeyId key)
  {
    if (!this.ValidateThereAreNoPendingRequestsForKey(key, nameof (ClearLocalData)))
      return;
    this.m_gameSaveDataMapByKey.Remove(key);
  }

  public bool ValidateIfKeyCanBeAccessed(GameSaveKeyId key, string loggingContext)
  {
    if (!GameSaveDataManager.IsGameSaveKeyValid(key))
    {
      Log.All.PrintWarning("GameSaveDataManager.ValidateKeyCanBeAccessed() called with invalid key ID {0}!  Context: {1}\nStack Trace:\n{2}", (object) key, (object) loggingContext, (object) StackTraceUtility.ExtractStackTrace());
      return false;
    }
    if (this.IsRequestPending(key))
    {
      Log.All.PrintWarning("GameSaveDataManager.ValidateKeyCanBeAccessed() - Request for key {0} is pending!  Context: {1}\nStack Trace:\n{2}", (object) key, (object) loggingContext, (object) StackTraceUtility.ExtractStackTrace());
      return false;
    }
    if (this.IsDataReady(key))
      return true;
    Log.All.Print("GameSaveDataManager.ValidateKeyCanBeAccessed() - Key {0} has no data - it has either not been created yet, or has not been requested.  Context: {1}\nStack Trace:\n{2}", (object) key, (object) loggingContext, (object) StackTraceUtility.ExtractStackTrace());
    return false;
  }

  public bool IsRequestPending(GameSaveKeyId key)
  {
    foreach (GameSaveDataManager.PendingRequestContext pendingRequestContext in this.m_pendingRequestsByClientToken.Values)
    {
      if (pendingRequestContext.AffectedKeys.IndexOf(key) >= 0)
        return true;
    }
    return false;
  }

  public static GameSaveDataManager Get()
  {
    if (GameSaveDataManager.s_instance == null)
      GameSaveDataManager.s_instance = new GameSaveDataManager();
    return GameSaveDataManager.s_instance;
  }

  public static bool GetProgressSubkeysForDungeonCrawlWing(
    WingDbfRecord wingRecord,
    out GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys progressSubkeys)
  {
    if (wingRecord == null)
    {
      Log.Adventures.PrintWarning("GetProgressSubkeysForDungeonCrawlWing: wingRecord is null!");
      progressSubkeys = new GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys();
      return false;
    }
    int sortedWingUnlockIndex = GameUtils.GetSortedWingUnlockIndex(wingRecord);
    if (sortedWingUnlockIndex < 0 || sortedWingUnlockIndex >= GameSaveDataManager.ProgressSubkeysForDungeonCrawlWings.Count)
    {
      Log.Adventures.PrintWarning("GetProgressSubkeysForDungeonCrawlWing: could not find a valid Sorted Wing Unlock Index for WingDbfRecord {0} - WingIndex: {1}!", (object) wingRecord.ID, (object) sortedWingUnlockIndex);
      progressSubkeys = new GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys();
      return false;
    }
    progressSubkeys = GameSaveDataManager.ProgressSubkeysForDungeonCrawlWings[sortedWingUnlockIndex];
    return true;
  }

  public static bool GetProgressSubkeyForDungeonCrawlClass(
    TAG_CLASS tagClass,
    out GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys progressSubkeys)
  {
    if (GameSaveDataManager.AdventureDungeonCrawlClassToSubkeyMapping.ContainsKey(tagClass))
    {
      progressSubkeys = GameSaveDataManager.AdventureDungeonCrawlClassToSubkeyMapping[tagClass];
      return true;
    }
    progressSubkeys = new GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys();
    return false;
  }

  public static bool GetBossWinsSubkeyForDungeonCrawlGuestHero(
    int guestHeroId,
    out GameSaveKeySubkeyId bossWinsSubkey)
  {
    if (GameSaveDataManager.AdventureDungeonCrawlGuestHeroToBossWinSubkeyMapping.ContainsKey(guestHeroId))
    {
      bossWinsSubkey = GameSaveDataManager.AdventureDungeonCrawlGuestHeroToBossWinSubkeyMapping[guestHeroId];
      return true;
    }
    bossWinsSubkey = GameSaveKeySubkeyId.INVALID;
    return false;
  }

  public static List<TAG_CLASS> GetClassesFromDungeonCrawlProgressMap() => GameSaveDataManager.AdventureDungeonCrawlClassToSubkeyMapping.Keys.ToList<TAG_CLASS>();

  public void Cheat_SaveSubkeyToLocalCache(
    GameSaveKeyId key,
    GameSaveKeySubkeyId subkey,
    params long[] values)
  {
    if (!HearthstoneApplication.IsInternal())
      return;
    this.SaveSubkeyToLocalCache(new GameSaveDataManager.SubkeySaveRequest(key, subkey, values));
  }

  public bool MigrateSubkeyIntValue(
    GameSaveKeyId sourceKey,
    GameSaveKeyId destinationKey,
    GameSaveKeySubkeyId subkeyId,
    long emptyValueForSource = 0)
  {
    GameSaveDataValue subkeyValue = this.GetSubkeyValue(sourceKey, subkeyId);
    if (subkeyValue == null || subkeyValue.IntValue == null || subkeyValue.IntValue.Count < 1 || subkeyValue.IntValue[0] == emptyValueForSource)
      return false;
    long num = subkeyValue.IntValue[0];
    return this.SaveSubkeys(new List<GameSaveDataManager.SubkeySaveRequest>()
    {
      new GameSaveDataManager.SubkeySaveRequest(destinationKey, subkeyId, new long[1]
      {
        num
      }),
      new GameSaveDataManager.SubkeySaveRequest(sourceKey, subkeyId, new long[1]
      {
        emptyValueForSource
      })
    });
  }

  private GameSaveDataValue GetSubkeyValue(
    GameSaveKeyId key,
    GameSaveKeySubkeyId subkeyId)
  {
    if (!this.IsDataReady(key))
    {
      Debug.LogErrorFormat("Attempting to get subkey {0} from key {1} failed, key not received by client yet", (object) subkeyId, (object) key);
      return (GameSaveDataValue) null;
    }
    Map<GameSaveKeySubkeyId, GameSaveDataValue> map;
    GameSaveDataValue gameSaveDataValue;
    return this.m_gameSaveDataMapByKey.TryGetValue(key, out map) && map != null && map.TryGetValue(subkeyId, out gameSaveDataValue) ? gameSaveDataValue : (GameSaveDataValue) null;
  }

  private static void OnWillReset()
  {
    HearthstoneApplication.Get().WillReset -= new Action(GameSaveDataManager.OnWillReset);
    GameSaveDataManager.s_instance = new GameSaveDataManager();
  }

  private void OnFatalError(FatalErrorMessage message, object userData) => this.m_pendingRequestsByClientToken.Clear();

  public struct AdventureDungeonCrawlClassProgressSubkeys
  {
    public GameSaveKeySubkeyId bossWins;
    public GameSaveKeySubkeyId runWins;
  }

  public struct AdventureDungeonCrawlWingProgressSubkeys
  {
    public GameSaveKeySubkeyId heroCardWins;
    public GameSaveKeySubkeyId heroPowerWins;
    public GameSaveKeySubkeyId deckWins;
    public GameSaveKeySubkeyId treasureWins;
  }

  public struct GameSaveKeyTuple
  {
    public GameSaveKeyId Key;
    public GameSaveKeySubkeyId Subkey;

    public GameSaveKeyTuple(GameSaveKeyId key, GameSaveKeySubkeyId subkey)
    {
      this.Key = key;
      this.Subkey = subkey;
    }

    public override bool Equals(object obj) => obj is GameSaveDataManager.GameSaveKeyTuple p && this.Equals(p);

    public bool Equals(GameSaveDataManager.GameSaveKeyTuple p) => this.Key == p.Key && this.Subkey == p.Subkey;

    public override int GetHashCode() => (int) (this.Key ^ (GameSaveKeyId) this.Subkey);
  }

  public class SubkeySaveRequest
  {
    public readonly GameSaveKeyId Key;
    public readonly GameSaveKeySubkeyId Subkey;
    public readonly long[] Long_Values;
    public readonly string[] String_Values;

    public SubkeySaveRequest(GameSaveKeyId key, GameSaveKeySubkeyId subkey, params long[] values)
    {
      this.Key = key;
      this.Subkey = subkey;
      this.Long_Values = values;
    }

    public SubkeySaveRequest(GameSaveKeyId key, GameSaveKeySubkeyId subkey, params string[] values)
    {
      this.Key = key;
      this.Subkey = subkey;
      this.String_Values = values;
    }
  }

  private class PendingRequestContext
  {
    public readonly List<GameSaveKeyId> AffectedKeys = new List<GameSaveKeyId>();
    public readonly GameSaveDataManager.OnRequestDataResponseDelegate RequestCallback;
    public readonly GameSaveDataManager.OnSaveDataResponseDelegate SaveCallback;

    public PendingRequestContext(
      List<GameSaveKeyId> requestedKeys,
      GameSaveDataManager.OnRequestDataResponseDelegate requestCallback)
    {
      this.AffectedKeys.AddRange((IEnumerable<GameSaveKeyId>) requestedKeys);
      this.RequestCallback = requestCallback;
    }

    public PendingRequestContext(
      List<GameSaveDataManager.SubkeySaveRequest> requests,
      GameSaveDataManager.OnSaveDataResponseDelegate saveCallback)
    {
      foreach (GameSaveDataManager.SubkeySaveRequest request in requests)
        this.AffectedKeys.Add(request.Key);
      this.SaveCallback = saveCallback;
    }
  }

  private class ServerOptionFlagMigrationData
  {
    public readonly GameSaveKeyId KeyId;
    public readonly GameSaveKeySubkeyId SubkeyId;
    public readonly int FlagTrueValue;
    public readonly int FlagFalseValue;

    public ServerOptionFlagMigrationData(
      GameSaveKeyId keyId,
      GameSaveKeySubkeyId subkeyId,
      int flagTrueValue = 1,
      int flagFalseValue = 0)
    {
      this.FlagTrueValue = flagTrueValue;
      this.FlagFalseValue = flagFalseValue;
      this.KeyId = keyId;
      this.SubkeyId = subkeyId;
    }
  }

  public delegate void OnRequestDataResponseDelegate(bool success);

  public delegate void OnSaveDataResponseDelegate(bool success);
}
