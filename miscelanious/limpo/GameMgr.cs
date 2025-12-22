using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Logging;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using PegasusClient;
using PegasusGame;
using PegasusShared;
using PegasusUtil;
using SpectatorProto;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : IService
{
  private const string MATCHING_POPUP_PC_NAME = "MatchingPopup3D.prefab:4f4a40d14d907e94da1b81d97c18a44f";
  private const string MATCHING_POPUP_PHONE_NAME = "MatchingPopup3D_phone.prefab:a7a5cea6306a1fa4680a9782fd25be14";
  private const string LOADING_POPUP_NAME = "LoadingPopup.prefab:ff9266f7c55faa94b9cd0f1371df7168";
  private const int MINIMUM_SECONDS_TIL_TB_END_TO_RETURN_TO_TB_SCENE = 10;
  private PlatformDependentValue<string> MATCHING_POPUP_NAME;
  private readonly Map<string, System.Type> s_transitionPopupNameToType = new Map<string, System.Type>()
  {
    {
      "MatchingPopup3D.prefab:4f4a40d14d907e94da1b81d97c18a44f",
      typeof (MatchingPopupDisplay)
    },
    {
      "MatchingPopup3D_phone.prefab:a7a5cea6306a1fa4680a9782fd25be14",
      typeof (MatchingPopupDisplay)
    },
    {
      "LoadingPopup.prefab:ff9266f7c55faa94b9cd0f1371df7168",
      typeof (LoadingPopupDisplay)
    }
  };
  private LastGameData m_lastGameData = new LastGameData();
  private GameConnectionInfo m_connectionInfoForGameConnectingTo;
  private GameType m_gameType;
  private GameType m_prevGameType;
  private GameType m_nextGameType;
  private PegasusShared.FormatType m_formatType;
  private PegasusShared.FormatType m_prevFormatType;
  private PegasusShared.FormatType m_nextFormatType;
  private int m_missionId;
  private int m_prevMissionId;
  private int m_nextMissionId;
  private int m_brawlLibraryItemId;
  private int m_nextBrawlLibraryItemId;
  private ReconnectType m_reconnectType;
  private ReconnectType m_prevReconnectType;
  private ReconnectType m_nextReconnectType;
  private bool m_readyToProcessGameConnections;
  private GameConnectionInfo m_deferredGameConnectionInfo;
  private bool m_spectator;
  private bool m_prevSpectator;
  private bool m_nextSpectator;
  private long? m_lastDeckId;
  private string m_lastAIDeck;
  private int? m_lastHeroCardDbId;
  private int? m_lastSeasonId;
  private int m_gameHandleId;
  private uint m_lastEnterGameError;
  private bool m_pendingAutoConcede;
  private FindGameState m_findGameState;
  private List<GameMgr.FindGameListener> m_findGameListeners = new List<GameMgr.FindGameListener>();
  private TransitionPopup m_transitionPopup;
  private Vector3 m_initialTransitionPopupPos;
  private Network.GameSetup m_gameSetup;
  private Map<int, string> m_lastDisplayedPlayerNames = new Map<int, string>();
  private static Map<QueueEvent.Type, FindGameState?> s_bnetToFindGameResultMap = new Map<QueueEvent.Type, FindGameState?>()
  {
    {
      QueueEvent.Type.UNKNOWN,
      new FindGameState?()
    },
    {
      QueueEvent.Type.QUEUE_ENTER,
      new FindGameState?(FindGameState.BNET_QUEUE_ENTERED)
    },
    {
      QueueEvent.Type.QUEUE_LEAVE,
      new FindGameState?()
    },
    {
      QueueEvent.Type.QUEUE_DELAY,
      new FindGameState?(FindGameState.BNET_QUEUE_DELAYED)
    },
    {
      QueueEvent.Type.QUEUE_UPDATE,
      new FindGameState?(FindGameState.BNET_QUEUE_UPDATED)
    },
    {
      QueueEvent.Type.QUEUE_DELAY_ERROR,
      new FindGameState?(FindGameState.BNET_ERROR)
    },
    {
      QueueEvent.Type.QUEUE_AMM_ERROR,
      new FindGameState?(FindGameState.BNET_ERROR)
    },
    {
      QueueEvent.Type.QUEUE_WAIT_END,
      new FindGameState?()
    },
    {
      QueueEvent.Type.QUEUE_CANCEL,
      new FindGameState?(FindGameState.BNET_QUEUE_CANCELED)
    },
    {
      QueueEvent.Type.QUEUE_GAME_STARTED,
      new FindGameState?(FindGameState.SERVER_GAME_CONNECTING)
    },
    {
      QueueEvent.Type.ABORT_CLIENT_DROPPED,
      new FindGameState?(FindGameState.BNET_ERROR)
    }
  };
  public const int NO_BRAWL_LIBRARY_ITEM_ID = 0;

  public event System.Action OnTransitionPopupShown;

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    GameMgr gameMgr = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    HearthstoneApplication.Get().WillReset += new System.Action(gameMgr.WillReset);
    gameMgr.MATCHING_POPUP_NAME = new PlatformDependentValue<string>(PlatformCategory.Screen)
    {
      PC = "MatchingPopup3D.prefab:4f4a40d14d907e94da1b81d97c18a44f",
      Phone = "MatchingPopup3D_phone.prefab:a7a5cea6306a1fa4680a9782fd25be14"
    };
    Network network = serviceLocator.Get<Network>();
    network.RegisterGameQueueHandler(new Network.GameQueueHandler(gameMgr.OnGameQueueEvent));
    network.RegisterNetHandler((object) GameToConnectNotification.PacketID.ID, new Network.NetHandler(gameMgr.OnGameToJoinNotification));
    network.RegisterNetHandler((object) PegasusGame.GameSetup.PacketID.ID, new Network.NetHandler(gameMgr.OnGameSetup));
    network.RegisterNetHandler((object) GameCanceled.PacketID.ID, new Network.NetHandler(gameMgr.OnGameCanceled));
    network.RegisterNetHandler((object) ServerResult.PacketID.ID, new Network.NetHandler(gameMgr.OnServerResult));
    network.AddBnetErrorListener(BnetFeature.Games, new Network.BnetErrorCallback(gameMgr.OnBnetError));
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(gameMgr.OnFatalError));
    return false;
  }

  public System.Type[] GetDependencies() => new System.Type[1]
  {
    typeof (Network)
  };

  public void Shutdown()
  {
  }

  private void WillReset()
  {
    this.m_gameType = GameType.GT_UNKNOWN;
    this.m_prevGameType = GameType.GT_UNKNOWN;
    this.m_nextGameType = GameType.GT_UNKNOWN;
    this.m_formatType = PegasusShared.FormatType.FT_UNKNOWN;
    this.m_prevFormatType = PegasusShared.FormatType.FT_UNKNOWN;
    this.m_nextFormatType = PegasusShared.FormatType.FT_UNKNOWN;
    this.m_missionId = 0;
    this.m_prevMissionId = 0;
    this.m_nextMissionId = 0;
    this.m_brawlLibraryItemId = 0;
    this.m_nextBrawlLibraryItemId = 0;
    this.m_reconnectType = ReconnectType.INVALID;
    this.m_prevReconnectType = ReconnectType.INVALID;
    this.m_nextReconnectType = ReconnectType.INVALID;
    this.m_readyToProcessGameConnections = false;
    this.m_deferredGameConnectionInfo = (GameConnectionInfo) null;
    this.m_spectator = false;
    this.m_prevSpectator = false;
    this.m_nextSpectator = false;
    this.m_lastEnterGameError = 0U;
    this.m_findGameState = FindGameState.INVALID;
    this.m_gameSetup = (Network.GameSetup) null;
    this.m_lastDisplayedPlayerNames.Clear();
    this.m_connectionInfoForGameConnectingTo = (GameConnectionInfo) null;
    this.m_gameHandleId = 0;
    this.m_lastGameData.Clear();
  }

  public static GameMgr Get() => ServiceManager.Get<GameMgr>();

  public void OnLoggedIn()
  {
    SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
    SceneMgr.Get().RegisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(this.OnScenePreLoad));
    ReconnectMgr.Get().AddTimeoutListener(new ReconnectMgr.TimeoutCallback(this.OnReconnectTimeout));
  }

  public GameType GetGameType() => this.m_gameType;

  public GameType GetPreviousGameType() => this.m_prevGameType;

  public GameType GetNextGameType() => this.m_nextGameType;

  public PegasusShared.FormatType GetFormatType() => this.m_formatType;

  public PegasusShared.FormatType GetPreviousFormatType() => this.m_prevFormatType;

  public PegasusShared.FormatType GetNextFormatType() => this.m_nextFormatType;

  public int GetMissionId() => this.m_missionId;

  public int GetPreviousMissionId() => this.m_prevMissionId;

  public int GetNextMissionId() => this.m_nextMissionId;

  public ReconnectType GetReconnectType() => this.m_reconnectType;

  public ReconnectType GetPreviousReconnectType() => this.m_prevReconnectType;

  public ReconnectType GetNextReconnectType() => this.m_nextReconnectType;

  public bool IsReconnect() => this.m_reconnectType != 0;

  public bool IsPreviousReconnect() => this.m_prevReconnectType != 0;

  public bool IsNextReconnect() => this.m_nextReconnectType != 0;

  public bool IsSpectator() => this.m_spectator;

  public bool WasSpectator() => this.m_prevSpectator;

  public bool IsNextSpectator() => this.m_nextSpectator;

  public int GetGameHandle() => this.m_gameHandleId;

  public long? LastDeckId => this.m_lastDeckId;

  public int? LastHeroCardDbId => this.m_lastHeroCardDbId;

  public uint GetLastEnterGameError() => this.m_lastEnterGameError;

  public bool IsPendingAutoConcede() => this.m_pendingAutoConcede;

  public void SetPendingAutoConcede(bool pendingAutoConcede)
  {
    if (!Network.Get().IsConnectedToGameServer())
      return;
    this.m_pendingAutoConcede = pendingAutoConcede;
  }

  public Network.GameSetup GetGameSetup() => this.m_gameSetup;

  public LastGameData LastGameData => this.m_lastGameData;

  public bool ConnectToGame(GameConnectionInfo info)
  {
    if (info == null)
    {
      Log.GameMgr.PrintWarning("ConnectToGame() called with no GameConnectionInfo passed in!");
      return false;
    }
    if (!this.m_readyToProcessGameConnections)
    {
      Log.GameMgr.Print("Received a GameConnectionInfo packet before the game is finished initializing; deferring it until later.");
      if (this.m_deferredGameConnectionInfo != null)
      {
        Log.GameMgr.PrintWarning("Another deferredGameConnectionInfo packet already exists.  Older packet GameType: {0}  Newer packet GameType: {1}", (object) this.m_deferredGameConnectionInfo.GameType, (object) info.GameType);
        Log.GameMgr.PrintWarning("Stomping over another deferred GameConnectionInfo packet.");
      }
      this.m_deferredGameConnectionInfo = info;
      return false;
    }
    FindGameState? toFindGameResult = GameMgr.s_bnetToFindGameResultMap[QueueEvent.Type.QUEUE_GAME_STARTED];
    GameServerInfo gsInfo = new GameServerInfo();
    gsInfo.Address = info.Address;
    gsInfo.Port = (uint) info.Port;
    gsInfo.GameHandle = (uint) info.GameHandle;
    gsInfo.ClientHandle = info.ClientHandle;
    gsInfo.AuroraPassword = info.AuroraPassword;
    gsInfo.Mission = info.Scenario;
    this.m_nextGameType = info.GameType;
    this.m_nextFormatType = info.FormatType;
    this.m_nextMissionId = info.Scenario;
    this.m_connectionInfoForGameConnectingTo = info;
    gsInfo.Version = BattleNet.GetVersion();
    gsInfo.Resumable = true;
    QueueEvent queueEvent = new QueueEvent(QueueEvent.Type.QUEUE_GAME_STARTED, 0, 0, 0, gsInfo);
    this.ChangeFindGameState(toFindGameResult.Value, queueEvent, queueEvent.GameServer, (Network.GameCancelInfo) null);
    return true;
  }

  public bool ConnectToGameIfHaveDeferredConnectionPacket()
  {
    this.m_readyToProcessGameConnections = true;
    if (this.m_deferredGameConnectionInfo == null)
      return false;
    int num = this.ConnectToGame(this.m_deferredGameConnectionInfo) ? 1 : 0;
    this.m_deferredGameConnectionInfo = (GameConnectionInfo) null;
    return num != 0;
  }

  public FindGameState GetFindGameState() => this.m_findGameState;

  public bool IsFindingGame() => this.m_findGameState != 0;

  public bool IsAboutToStopFindingGame()
  {
    switch (this.m_findGameState)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_STARTED:
      case FindGameState.SERVER_GAME_CANCELED:
        return true;
      default:
        return false;
    }
  }

  public void RegisterFindGameEvent(GameMgr.FindGameCallback callback) => this.RegisterFindGameEvent(callback, (object) null);

  public void RegisterFindGameEvent(GameMgr.FindGameCallback callback, object userData)
  {
    GameMgr.FindGameListener findGameListener = new GameMgr.FindGameListener();
    findGameListener.SetCallback(callback);
    findGameListener.SetUserData(userData);
    if (this.m_findGameListeners.Contains(findGameListener))
      return;
    this.m_findGameListeners.Add(findGameListener);
  }

  public bool UnregisterFindGameEvent(GameMgr.FindGameCallback callback) => this.UnregisterFindGameEvent(callback, (object) null);

  public bool UnregisterFindGameEvent(GameMgr.FindGameCallback callback, object userData)
  {
    GameMgr.FindGameListener findGameListener = new GameMgr.FindGameListener();
    findGameListener.SetCallback(callback);
    findGameListener.SetUserData(userData);
    return this.m_findGameListeners.Remove(findGameListener);
  }

  private void FindGameInternal(
    GameType gameType,
    PegasusShared.FormatType formatType,
    int missionId,
    int brawlLibraryItemId,
    long deckId,
    string aiDeck,
    int heroCardDbId,
    int? seasonId,
    bool restoreSavedGameState,
    byte[] snapshot,
    int? lettuceMapNodeId,
    long lettuceTeamId,
    GameType progFilterOverride = GameType.GT_UNKNOWN,
    int deckTemplateId = 0)
  {
    this.m_lastEnterGameError = 0U;
    this.m_nextGameType = gameType;
    this.m_nextFormatType = formatType;
    this.m_nextMissionId = missionId;
    this.m_nextBrawlLibraryItemId = brawlLibraryItemId;
    this.m_lastDeckId = new long?(deckId);
    this.m_lastAIDeck = aiDeck;
    this.m_lastHeroCardDbId = new int?(heroCardDbId);
    this.m_lastSeasonId = seasonId;
    this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
    Network.Get().FindGame(gameType, formatType, missionId, brawlLibraryItemId, deckId, aiDeck, heroCardDbId, seasonId, restoreSavedGameState, snapshot, lettuceMapNodeId, lettuceTeamId, progFilterOverride, deckTemplateId);
    this.UpdateSessionPresence(gameType);
  }

  public void FindGame(
    GameType gameType,
    PegasusShared.FormatType formatType,
    int missionId,
    int brawlLibraryItemId = 0,
    long deckId = 0,
    string aiDeck = null,
    int? seasonId = null,
    bool restoreSavedGameState = false,
    byte[] snapshot = null,
    int? lettuceMapNodeId = null,
    long lettuceTeamId = 0,
    GameType progFilterOverride = GameType.GT_UNKNOWN,
    int deckTemplateId = 0)
  {
    this.FindGameInternal(gameType, formatType, missionId, brawlLibraryItemId, deckId, aiDeck, 0, seasonId, restoreSavedGameState, snapshot, lettuceMapNodeId, lettuceTeamId, progFilterOverride, deckTemplateId);
    if (!restoreSavedGameState)
    {
      string popupForFindGame = this.DetermineTransitionPopupForFindGame(gameType, missionId);
      if (popupForFindGame != null)
        this.ShowTransitionPopup(popupForFindGame, missionId);
    }
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager == null)
      return;
    CollectionDeck deck = collectionManager.GetDeck(deckId);
    if (deck == null)
      return;
    Log.Decks.PrintInfo("Finding Game With Deck:");
    deck.LogDeckStringInformation();
  }

  public void FindGameWithHero(
    GameType gameType,
    PegasusShared.FormatType formatType,
    int missionId,
    int brawlLibraryItemId,
    int heroCardDbId,
    long deckid = 0)
  {
    this.FindGameInternal(gameType, formatType, missionId, brawlLibraryItemId, deckid, (string) null, heroCardDbId, new int?(), false, (byte[]) null, new int?(), 0L);
    string popupForFindGame = this.DetermineTransitionPopupForFindGame(gameType, missionId);
    if (popupForFindGame != null)
      this.ShowTransitionPopup(popupForFindGame, missionId);
    Log.Decks.PrintInfo("Finding Game With Hero: {0}", (object) heroCardDbId);
  }

  public void Cheat_ShowTransitionPopup(GameType gameType, PegasusShared.FormatType formatType, int missionId)
  {
    if (!HearthstoneApplication.IsInternal())
      return;
    this.m_nextMissionId = missionId;
    this.m_nextFormatType = formatType;
    string popupForFindGame = this.DetermineTransitionPopupForFindGame(gameType, missionId);
    if (popupForFindGame == null)
      return;
    this.ShowTransitionPopup(popupForFindGame, missionId);
  }

  public void RestartGame()
  {
    int gameType = (int) this.m_gameType;
    int formatType = (int) this.m_formatType;
    int missionId = this.m_missionId;
    int brawlLibraryItemId = this.m_brawlLibraryItemId;
    long deckId = this.m_lastDeckId ?? 0L;
    string lastAiDeck = this.m_lastAIDeck;
    int? nullable = this.m_lastHeroCardDbId;
    int heroCardDbId = nullable ?? 0;
    int? lastSeasonId = this.m_lastSeasonId;
    nullable = new int?();
    int? lettuceMapNodeId = nullable;
    this.FindGameInternal((GameType) gameType, (PegasusShared.FormatType) formatType, missionId, brawlLibraryItemId, deckId, lastAiDeck, heroCardDbId, lastSeasonId, false, (byte[]) null, lettuceMapNodeId, 0L);
  }

  public bool HasLastPlayedDeckId() => this.m_lastDeckId.HasValue;

  public void EnterFriendlyChallengeGameWithDecks(
    PegasusShared.FormatType formatType,
    BrawlType brawlType,
    int missionId,
    int seasonId,
    int brawlLibraryItemId,
    BnetGameAccountId player2GameAccountId,
    DeckShareState player1DeckShareState,
    long player1DeckId,
    DeckShareState player2DeckShareState,
    long player2DeckId,
    long? player1RandomHeroCardDbId,
    long? player2RandomHeroCardDbId,
    long? player1CardBackId,
    long? player2CardBackId)
  {
    Network.Get().EnterFriendlyChallengeGame(formatType, brawlType, missionId, seasonId, brawlLibraryItemId, player2GameAccountId, player1DeckShareState, player1DeckId, player2DeckShareState, player2DeckId, new long?(), new long?(), player1RandomHeroCardDbId, player2RandomHeroCardDbId, player1CardBackId, player2CardBackId);
  }

  public void EnterFriendlyChallengeGameWithHeroes(
    PegasusShared.FormatType formatType,
    BrawlType brawlType,
    int missionId,
    int seasonId,
    int brawlLibraryItemId,
    BnetGameAccountId player2GameAccountId,
    long player1HeroCardDbId,
    long player2HeroCardDbId,
    long? player1CardBackId,
    long? player2CardBackId)
  {
    Network.Get().EnterFriendlyChallengeGame(formatType, brawlType, missionId, seasonId, brawlLibraryItemId, player2GameAccountId, DeckShareState.NO_DECK_SHARE, 0L, DeckShareState.NO_DECK_SHARE, 0L, new long?(player1HeroCardDbId), new long?(player2HeroCardDbId), new long?(), new long?(), player1CardBackId, player2CardBackId);
  }

  public void WaitForFriendChallengeToStart(
    PegasusShared.FormatType formatType,
    BrawlType brawlType,
    int missionId,
    int brawlLibraryItemId,
    PartyType partyType)
  {
    this.m_nextFormatType = formatType;
    this.m_nextMissionId = missionId;
    this.m_nextBrawlLibraryItemId = brawlLibraryItemId;
    this.m_lastEnterGameError = 0U;
    bool flag = FiresideGatheringManager.Get().CurrentFiresideGatheringMode != 0;
    if (brawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING || brawlType == BrawlType.BRAWL_TYPE_TAVERN_BRAWL & flag)
    {
      this.m_nextGameType = GameType.GT_FSG_BRAWL_VS_FRIEND;
      this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
    }
    else
    {
      switch (partyType)
      {
        case PartyType.BATTLEGROUNDS_PARTY:
          this.m_nextGameType = PartyManager.Get().GetCurrentPartySize() > PartyManager.Get().GetBattlegroundsMaxRankedPartySize() ? GameType.GT_BATTLEGROUNDS_FRIENDLY : GameType.GT_BATTLEGROUNDS;
          this.ChangeFindGameState(FindGameState.BNET_QUEUE_ENTERED);
          break;
        case PartyType.MERCENARIES_FRIENDLY_CHALLENGE:
          this.m_nextGameType = GameType.GT_MERCENARIES_FRIENDLY;
          this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
          break;
        case PartyType.MERCENARIES_COOP_PARTY:
          this.m_nextGameType = GameType.GT_MERCENARIES_PVE_COOP;
          this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
          break;
        default:
          this.m_nextGameType = GameType.GT_VS_FRIEND;
          this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
          break;
      }
    }
    string popupForFindGame = this.DetermineTransitionPopupForFindGame(this.m_nextGameType, missionId);
    if (popupForFindGame != null)
      this.ShowTransitionPopup(popupForFindGame, missionId);
    else
      Debug.LogError((object) "WaitForFriendChallengeToStart - No valid transition popup.");
  }

  public void SpectateGame(JoinInfo joinInfo)
  {
    GameServerInfo serverInfo = new GameServerInfo();
    serverInfo.Address = joinInfo.ServerIpAddress;
    serverInfo.Port = joinInfo.ServerPort;
    serverInfo.GameHandle = (uint) joinInfo.GameHandle;
    serverInfo.SpectatorPassword = joinInfo.SecretKey;
    serverInfo.SpectatorMode = true;
    this.m_nextGameType = joinInfo.GameType;
    this.m_nextFormatType = joinInfo.FormatType;
    this.m_nextMissionId = joinInfo.MissionId;
    this.m_brawlLibraryItemId = joinInfo.BrawlLibraryItemId;
    this.m_nextSpectator = true;
    this.m_lastEnterGameError = 0U;
    this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
    this.ShowTransitionPopup("LoadingPopup.prefab:ff9266f7c55faa94b9cd0f1371df7168", joinInfo.MissionId);
    this.ChangeFindGameState(FindGameState.SERVER_GAME_CONNECTING, serverInfo);
    if (!((UnityEngine.Object) Gameplay.Get() == (UnityEngine.Object) null))
      return;
    Network.Get().SetGameServerDisconnectEventListener(new Network.GameServerDisconnectEvent(this.OnGameServerDisconnect));
  }

  private void OnGameServerDisconnect(BattleNetErrors error) => this.OnGameCanceled();

  public void ReconnectGame(
    GameType gameType,
    PegasusShared.FormatType formatType,
    ReconnectType reconnectType,
    GameServerInfo serverInfo)
  {
    this.m_nextGameType = gameType;
    this.m_nextFormatType = formatType;
    this.m_nextMissionId = serverInfo.Mission;
    this.m_nextBrawlLibraryItemId = serverInfo.BrawlLibraryItemId;
    this.m_nextReconnectType = reconnectType;
    this.m_nextSpectator = serverInfo.SpectatorMode;
    this.m_lastEnterGameError = 0U;
    this.ChangeFindGameState(FindGameState.CLIENT_STARTED);
    this.ChangeFindGameState(FindGameState.SERVER_GAME_CONNECTING, serverInfo);
  }

  public bool CancelFindGame()
  {
    if (!GameUtils.IsMatchmadeGameType(this.m_nextGameType) || !Network.Get().IsFindingGame())
      return false;
    Network.Get().CancelFindGame();
    if (this.IsFindingGame())
      this.ChangeFindGameState(FindGameState.CLIENT_CANCELED);
    return true;
  }

  public void HideTransitionPopup()
  {
    if (!(bool) (UnityEngine.Object) this.m_transitionPopup)
      return;
    this.m_transitionPopup.Hide();
  }

  public GameEntity CreateGameEntity(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    FlowPerformanceGame currentPerformanceFlow = HearthstonePerformance.Get()?.GetCurrentPerformanceFlow<FlowPerformanceGame>();
    if (currentPerformanceFlow != null)
      currentPerformanceFlow.GameUuid = createGame.Uuid;
    GameEntity gameEntity;
    switch ((ScenarioDbId) this.m_missionId)
    {
      case ScenarioDbId.TUTORIAL_HOGGER:
        gameEntity = (GameEntity) new Tutorial_01();
        break;
      case ScenarioDbId.TUTORIAL_MILLHOUSE:
        gameEntity = (GameEntity) new Tutorial_02();
        break;
      case ScenarioDbId.TUTORIAL_MUKLA:
        gameEntity = (GameEntity) new Tutorial_03();
        break;
      case ScenarioDbId.TUTORIAL_NESINGWARY:
        gameEntity = (GameEntity) new Tutorial_04();
        break;
      case ScenarioDbId.TUTORIAL_ILLIDAN:
        gameEntity = (GameEntity) new Tutorial_05();
        break;
      case ScenarioDbId.TUTORIAL_CHO:
        gameEntity = (GameEntity) new Tutorial_06();
        break;
      case ScenarioDbId.NAXX_ANUBREKHAN:
      case ScenarioDbId.NAXX_HEROIC_ANUBREKHAN:
        gameEntity = (GameEntity) new NAX01_AnubRekhan();
        break;
      case ScenarioDbId.NAXX_FAERLINA:
      case ScenarioDbId.NAXX_CHALLENGE_DRUID_V_FAERLINA:
      case ScenarioDbId.NAXX_HEROIC_FAERLINA:
        gameEntity = (GameEntity) new NAX02_Faerlina();
        break;
      case ScenarioDbId.NAXX_NOTH:
      case ScenarioDbId.NAXX_HEROIC_NOTH:
        gameEntity = (GameEntity) new NAX04_Noth();
        break;
      case ScenarioDbId.NAXX_HEIGAN:
      case ScenarioDbId.NAXX_CHALLENGE_MAGE_V_HEIGAN:
      case ScenarioDbId.NAXX_HEROIC_HEIGAN:
        gameEntity = (GameEntity) new NAX05_Heigan();
        break;
      case ScenarioDbId.NAXX_LOATHEB:
      case ScenarioDbId.NAXX_CHALLENGE_HUNTER_V_LOATHEB:
      case ScenarioDbId.NAXX_HEROIC_LOATHEB:
        gameEntity = (GameEntity) new NAX06_Loatheb();
        break;
      case ScenarioDbId.NAXX_MAEXXNA:
      case ScenarioDbId.NAXX_CHALLENGE_ROGUE_V_MAEXXNA:
      case ScenarioDbId.NAXX_HEROIC_MAEXXNA:
        gameEntity = (GameEntity) new NAX03_Maexxna();
        break;
      case ScenarioDbId.NAXX_RAZUVIOUS:
      case ScenarioDbId.NAXX_HEROIC_RAZUVIOUS:
        gameEntity = (GameEntity) new NAX07_Razuvious();
        break;
      case ScenarioDbId.NAXX_GOTHIK:
      case ScenarioDbId.NAXX_CHALLENGE_SHAMAN_V_GOTHIK:
      case ScenarioDbId.NAXX_HEROIC_GOTHIK:
        gameEntity = (GameEntity) new NAX08_Gothik();
        break;
      case ScenarioDbId.NAXX_HORSEMEN:
      case ScenarioDbId.NAXX_CHALLENGE_WARLOCK_V_HORSEMEN:
      case ScenarioDbId.NAXX_HEROIC_HORSEMEN:
        gameEntity = (GameEntity) new NAX09_Horsemen();
        break;
      case ScenarioDbId.NAXX_PATCHWERK:
      case ScenarioDbId.NAXX_HEROIC_PATCHWERK:
        gameEntity = (GameEntity) new NAX10_Patchwerk();
        break;
      case ScenarioDbId.NAXX_GROBBULUS:
      case ScenarioDbId.NAXX_CHALLENGE_WARRIOR_V_GROBBULUS:
      case ScenarioDbId.NAXX_HEROIC_GROBBULUS:
        gameEntity = (GameEntity) new NAX11_Grobbulus();
        break;
      case ScenarioDbId.NAXX_GLUTH:
      case ScenarioDbId.NAXX_HEROIC_GLUTH:
        gameEntity = (GameEntity) new NAX12_Gluth();
        break;
      case ScenarioDbId.NAXX_THADDIUS:
      case ScenarioDbId.NAXX_CHALLENGE_PRIEST_V_THADDIUS:
      case ScenarioDbId.NAXX_HEROIC_THADDIUS:
        gameEntity = (GameEntity) new NAX13_Thaddius();
        break;
      case ScenarioDbId.NAXX_KELTHUZAD:
      case ScenarioDbId.NAXX_CHALLENGE_PALADIN_V_KELTHUZAD:
      case ScenarioDbId.NAXX_HEROIC_KELTHUZAD:
        gameEntity = (GameEntity) new NAX15_KelThuzad();
        break;
      case ScenarioDbId.NAXX_SAPPHIRON:
      case ScenarioDbId.NAXX_HEROIC_SAPPHIRON:
        gameEntity = (GameEntity) new NAX14_Sapphiron();
        break;
      case ScenarioDbId.BRM_GRIM_GUZZLER:
      case ScenarioDbId.BRM_HEROIC_GRIM_GUZZLER:
      case ScenarioDbId.BRM_CHALLENGE_HUNTER_V_GUZZLER:
        gameEntity = (GameEntity) new BRM01_GrimGuzzler();
        break;
      case ScenarioDbId.BRM_DARK_IRON_ARENA:
      case ScenarioDbId.BRM_HEROIC_DARK_IRON_ARENA:
      case ScenarioDbId.BRM_CHALLENGE_MAGE_V_DARK_IRON_ARENA:
        gameEntity = (GameEntity) new BRM02_DarkIronArena();
        break;
      case ScenarioDbId.BRM_THAURISSAN:
      case ScenarioDbId.BRM_HEROIC_THAURISSAN:
        gameEntity = (GameEntity) new BRM03_Thaurissan();
        break;
      case ScenarioDbId.BRM_GARR:
      case ScenarioDbId.BRM_HEROIC_GARR:
      case ScenarioDbId.BRM_CHALLENGE_WARRIOR_V_GARR:
        gameEntity = (GameEntity) new BRM04_Garr();
        break;
      case ScenarioDbId.BRM_BARON_GEDDON:
      case ScenarioDbId.BRM_HEROIC_BARON_GEDDON:
      case ScenarioDbId.BRM_CHALLENGE_SHAMAN_V_GEDDON:
        gameEntity = (GameEntity) new BRM05_BaronGeddon();
        break;
      case ScenarioDbId.BRM_MAJORDOMO:
      case ScenarioDbId.BRM_HEROIC_MAJORDOMO:
        gameEntity = (GameEntity) new BRM06_Majordomo();
        break;
      case ScenarioDbId.BRM_OMOKK:
      case ScenarioDbId.BRM_HEROIC_OMOKK:
        gameEntity = (GameEntity) new BRM07_Omokk();
        break;
      case ScenarioDbId.BRM_DRAKKISATH:
      case ScenarioDbId.BRM_CHALLENGE_PRIEST_V_DRAKKISATH:
      case ScenarioDbId.BRM_HEROIC_DRAKKISATH:
        gameEntity = (GameEntity) new BRM08_Drakkisath();
        break;
      case ScenarioDbId.BRM_REND_BLACKHAND:
      case ScenarioDbId.BRM_CHALLENGE_DRUID_V_BLACKHAND:
      case ScenarioDbId.BRM_HEROIC_REND_BLACKHAND:
        gameEntity = (GameEntity) new BRM09_RendBlackhand();
        break;
      case ScenarioDbId.BRM_RAZORGORE:
      case ScenarioDbId.BRM_HEROIC_RAZORGORE:
      case ScenarioDbId.BRM_CHALLENGE_WARLOCK_V_RAZORGORE:
        gameEntity = (GameEntity) new BRM10_Razorgore();
        break;
      case ScenarioDbId.BRM_VAELASTRASZ:
      case ScenarioDbId.BRM_HEROIC_VAELASTRASZ:
      case ScenarioDbId.BRM_CHALLENGE_ROGUE_V_VAELASTRASZ:
        gameEntity = (GameEntity) new BRM11_Vaelastrasz();
        break;
      case ScenarioDbId.BRM_CHROMAGGUS:
      case ScenarioDbId.BRM_HEROIC_CHROMAGGUS:
        gameEntity = (GameEntity) new BRM12_Chromaggus();
        break;
      case ScenarioDbId.BRM_NEFARIAN:
      case ScenarioDbId.BRM_HEROIC_NEFARIAN:
        gameEntity = (GameEntity) new BRM13_Nefarian();
        break;
      case ScenarioDbId.BRM_OMNOTRON:
      case ScenarioDbId.BRM_HEROIC_OMNOTRON:
      case ScenarioDbId.BRM_CHALLENGE_PALADIN_V_OMNOTRON:
        gameEntity = (GameEntity) new BRM14_Omnotron();
        break;
      case ScenarioDbId.BRM_MALORIAK:
      case ScenarioDbId.BRM_HEROIC_MALORIAK:
        gameEntity = (GameEntity) new BRM15_Maloriak();
        break;
      case ScenarioDbId.BRM_ATRAMEDES:
      case ScenarioDbId.BRM_HEROIC_ATRAMEDES:
        gameEntity = (GameEntity) new BRM16_Atramedes();
        break;
      case ScenarioDbId.BRM_ZOMBIE_NEF:
      case ScenarioDbId.BRM_HEROIC_ZOMBIE_NEF:
        gameEntity = (GameEntity) new BRM17_ZombieNef();
        break;
      case ScenarioDbId.TB_RAG_V_NEF:
        gameEntity = (GameEntity) new TB01_RagVsNef();
        break;
      case ScenarioDbId.TB_DECKBUILDING:
      case ScenarioDbId.TB_DECKBUILDING_1P_TEST:
        gameEntity = (GameEntity) new TB04_DeckBuilding();
        break;
      case ScenarioDbId.TB_CO_OP_TEST:
      case ScenarioDbId.TB_CO_OP:
      case ScenarioDbId.TB_CO_OP_TEST2:
      case ScenarioDbId.TB_CO_OP_PRECON:
      case ScenarioDbId.TB_CO_OP_1P_TEST:
      case ScenarioDbId.TB_CO_OP_V2:
        gameEntity = (GameEntity) new TB02_CoOp();
        break;
      case ScenarioDbId.LOE_GIANTFIN:
      case ScenarioDbId.LOE_CHALLENGE_SHAMAN_V_GIANTFIN:
      case ScenarioDbId.LOE_HEROIC_GIANTFIN:
        gameEntity = (GameEntity) new LOE10_Giantfin();
        break;
      case ScenarioDbId.LOE_SUN_RAIDER_PHAERIX:
      case ScenarioDbId.LOE_CHALLENGE_WARLOCK_V_SUN_RAIDER:
      case ScenarioDbId.LOE_HEROIC_SUN_RAIDER_PHAERIX:
        gameEntity = (GameEntity) new LOE02_Sun_Raider_Phaerix();
        break;
      case ScenarioDbId.LOE_ZINAAR:
      case ScenarioDbId.LOE_CHALLENGE_WARRIOR_V_ZINAAR:
      case ScenarioDbId.LOE_HEROIC_ZINAAR:
        gameEntity = (GameEntity) new LOE01_Zinaar();
        break;
      case ScenarioDbId.LOE_SCARVASH:
      case ScenarioDbId.LOE_CHALLENGE_DRUID_V_SCARVASH:
      case ScenarioDbId.LOE_HEROIC_SCARVASH:
        gameEntity = (GameEntity) new LOE04_Scarvash();
        break;
      case ScenarioDbId.LOE_TEMPLE_ESCAPE:
      case ScenarioDbId.LOE_HEROIC_TEMPLE_ESCAPE:
        gameEntity = (GameEntity) new LOE03_AncientTemple();
        break;
      case ScenarioDbId.LOE_MINE_CART:
      case ScenarioDbId.LOE_HEROIC_MINE_CART:
        gameEntity = (GameEntity) new LOE07_MineCart();
        break;
      case ScenarioDbId.LOE_ARCHAEDAS:
      case ScenarioDbId.LOE_CHALLENGE_PALADIN_V_ARCHAEDUS:
      case ScenarioDbId.LOE_HEROIC_ARCHAEDAS:
        gameEntity = (GameEntity) new LOE08_Archaedas();
        break;
      case ScenarioDbId.LOE_LADY_NAZJAR:
      case ScenarioDbId.LOE_CHALLENGE_PRIEST_V_NAZJAR:
      case ScenarioDbId.LOE_HEROIC_LADY_NAZJAR:
        gameEntity = (GameEntity) new LOE12_Naga();
        break;
      case ScenarioDbId.LOE_SKELESAURUS:
      case ScenarioDbId.LOE_CHALLENGE_ROGUE_V_SKELESAURUS:
      case ScenarioDbId.LOE_HEROIC_SKELESAURUS:
        gameEntity = (GameEntity) new LOE13_Skelesaurus();
        break;
      case ScenarioDbId.LOE_RAFAAM_1:
      case ScenarioDbId.LOE_HEROIC_RAFAAM_1:
        gameEntity = (GameEntity) new LOE15_Boss1();
        break;
      case ScenarioDbId.LOE_RAFAAM_2:
      case ScenarioDbId.LOE_HEROIC_RAFAAM_2:
        gameEntity = (GameEntity) new LOE16_Boss2();
        break;
      case ScenarioDbId.LOE_STEEL_SENTINEL:
      case ScenarioDbId.LOE_CHALLENGE_MAGE_V_SENTINEL:
      case ScenarioDbId.LOE_HEROIC_STEEL_SENTINEL:
        gameEntity = (GameEntity) new LOE14_Steel_Sentinel();
        break;
      case ScenarioDbId.LOE_SLITHERSPEAR:
      case ScenarioDbId.LOE_CHALLENGE_HUNTER_V_SLITHERSPEAR:
      case ScenarioDbId.LOE_HEROIC_SLITHERSPEAR:
        gameEntity = (GameEntity) new LOE09_LordSlitherspear();
        break;
      case ScenarioDbId.TB_GIFTEXCHANGE_1P_TEST:
      case ScenarioDbId.TB_GIFTEXCHANGE:
        gameEntity = (GameEntity) new TB05_GiftExchange();
        break;
      case ScenarioDbId.TB_CHOOSEFATEBUILD_1P_TEST:
      case ScenarioDbId.TB_CHOOSEFATEBUILD:
        gameEntity = (GameEntity) new TB_ChooseYourFateBuildaround();
        break;
      case ScenarioDbId.TB_CHOOSEFATERANDOM_1P_TEST:
      case ScenarioDbId.TB_CHOOSEFATERANDOM:
        gameEntity = (GameEntity) new TB_ChooseYourFateRandom();
        break;
      case ScenarioDbId.TB_KELTHUZADRAFAAM:
      case ScenarioDbId.TB_KELTHUZADRAFAAM_1P:
        gameEntity = (GameEntity) new TB_KelthuzadRafaam();
        break;
      case ScenarioDbId.KAR_CRONE:
      case ScenarioDbId.KAR_HEROIC_CRONE:
        gameEntity = (GameEntity) new KAR06_Crone();
        break;
      case ScenarioDbId.KAR_WOLF:
      case ScenarioDbId.KAR_HEROIC_WOLF:
      case ScenarioDbId.KAR_CHALLENGE_PALADIN_V_WOLF:
        gameEntity = (GameEntity) new KAR05_Wolf();
        break;
      case ScenarioDbId.KAR_CHESS:
      case ScenarioDbId.KAR_HEROIC_CHESS:
        gameEntity = (GameEntity) new KAR03_Chess();
        break;
      case ScenarioDbId.KAR_JULIANNE:
      case ScenarioDbId.KAR_HEROIC_JULIANNE:
      case ScenarioDbId.KAR_CHALLENGE_WARLOCK_V_JULIANNE:
        gameEntity = (GameEntity) new KAR04_Julianne();
        break;
      case ScenarioDbId.KAR_MIRROR:
      case ScenarioDbId.KAR_HEROIC_MIRROR:
      case ScenarioDbId.KAR_CHALLENGE_SHAMAN_V_MIRROR:
        gameEntity = (GameEntity) new KAR02_Mirror();
        break;
      case ScenarioDbId.KAR_CURATOR:
      case ScenarioDbId.KAR_HEROIC_CURATOR:
      case ScenarioDbId.KAR_CHALLENGE_HUNTER_V_CURATOR:
        gameEntity = (GameEntity) new KAR07_Curator();
        break;
      case ScenarioDbId.KAR_ILLHOOF:
      case ScenarioDbId.KAR_HEROIC_ILLHOOF:
      case ScenarioDbId.KAR_CHALLENGE_WARRIOR_V_ILLHOOF:
        gameEntity = (GameEntity) new KAR09_Illhoof();
        break;
      case ScenarioDbId.KAR_NIGHTBANE:
      case ScenarioDbId.KAR_HEROIC_NIGHTBANE:
      case ScenarioDbId.KAR_CHALLENGE_MAGE_V_NIGHTBANE:
        gameEntity = (GameEntity) new KAR08_Nightbane();
        break;
      case ScenarioDbId.KAR_ARAN:
      case ScenarioDbId.KAR_HEROIC_ARAN:
      case ScenarioDbId.KAR_CHALLENGE_ROGUE_V_ARAN:
        gameEntity = (GameEntity) new KAR10_Aran();
        break;
      case ScenarioDbId.KAR_NETHERSPITE:
      case ScenarioDbId.KAR_HEROIC_NETHERSPITE:
      case ScenarioDbId.KAR_CHALLENGE_DRUID_V_NETHERSPITE:
        gameEntity = (GameEntity) new KAR11_Netherspite();
        break;
      case ScenarioDbId.KAR_PANTRY:
      case ScenarioDbId.KAR_HEROIC_PANTRY:
      case ScenarioDbId.KAR_CHALLENGE_PRIEST_V_PANTRY:
        gameEntity = (GameEntity) new KAR01_Pantry();
        break;
      case ScenarioDbId.KAR_PROLOGUE:
      case ScenarioDbId.KAR_HEROIC_PROLOGUE:
        gameEntity = (GameEntity) new KAR00_Prologue();
        break;
      case ScenarioDbId.KAR_PORTALS:
      case ScenarioDbId.KAR_HEROIC_PORTALS:
        gameEntity = (GameEntity) new KAR12_Portals();
        break;
      case ScenarioDbId.TB_SHADOWTOWERS_1P_TEST:
      case ScenarioDbId.TB_SHADOWTOWERS:
      case ScenarioDbId.TB_SHADOWTOWERS_TEST:
        gameEntity = (GameEntity) new TB09_ShadowTowers();
        break;
      case ScenarioDbId.TB_COOPV3_1P_TEST:
      case ScenarioDbId.TB_COOPV3:
      case ScenarioDbId.TB_COOPV3_Score_1P_TEST:
      case ScenarioDbId.TB_COOPV3_Score:
        gameEntity = (GameEntity) new TB11_CoOpv3();
        break;
      case ScenarioDbId.TB_DECKRECIPE_1P_TEST:
      case ScenarioDbId.TB_DECKRECIPE:
        gameEntity = (GameEntity) new TB10_DeckRecipe();
        break;
      case ScenarioDbId.TB_KARAPORTALS_1P_TEST:
      case ScenarioDbId.TB_KARAPORTALS:
        gameEntity = (GameEntity) new TB12_PartyPortals();
        break;
      case ScenarioDbId.TB_JUGGERNAUT:
        gameEntity = (GameEntity) new TB_Juggernaut();
        break;
      case ScenarioDbId.TB_BATTLEROYALE_1P_TEST:
      case ScenarioDbId.TB_BATTLEROYALE:
        gameEntity = (GameEntity) new TB15_BossBattleRoyale();
        break;
      case ScenarioDbId.TB_BLIZZCON_2016_1P:
      case ScenarioDbId.TB_BLIZZCON_2016:
        gameEntity = (GameEntity) new TB_Blizzcon_2016();
        break;
      case ScenarioDbId.TB_LETHALPUZZLES:
        gameEntity = (GameEntity) new TB13_LethalPuzzles();
        break;
      case ScenarioDbId.TB_DECKRECIPE_MSG_1P_TEST:
      case ScenarioDbId.TB_DECKRECIPE_MSG:
        gameEntity = (GameEntity) new TB10_DeckRecipe();
        break;
      case ScenarioDbId.TB_DPROMO:
        gameEntity = (GameEntity) new TB14_DPromo();
        break;
      case ScenarioDbId.ICC_01_LICHKING:
        gameEntity = (GameEntity) new ICC_01_LICHKING();
        break;
      case ScenarioDbId.ICC_03_SECRETS:
        gameEntity = (GameEntity) new ICC_03_SECRETS();
        break;
      case ScenarioDbId.ICC_04_SINDRAGOSA:
        gameEntity = (GameEntity) new ICC_04_Sindragosa();
        break;
      case ScenarioDbId.TB_MAMMOTHPARTY_1P:
      case ScenarioDbId.TB_MAMMOTHPARTY:
      case ScenarioDbId.TB_MAMMOTHPARTY_ANYTIME:
        gameEntity = (GameEntity) new TB_MammothParty();
        break;
      case ScenarioDbId.ICC_05_LANATHEL:
        gameEntity = (GameEntity) new ICC_05_Lanathel();
        break;
      case ScenarioDbId.ICC_06_MARROWGAR:
        gameEntity = (GameEntity) new ICC_06_Marrowgar();
        break;
      case ScenarioDbId.ICC_07_PUTRICIDE:
        gameEntity = (GameEntity) new ICC_07_Putricide();
        break;
      case ScenarioDbId.ICC_08_FINALE:
        gameEntity = (GameEntity) new ICC_08_Finale();
        break;
      case ScenarioDbId.TB_MP_CROSSROADS_1P:
      case ScenarioDbId.TB_MP_CROSSROADS:
        gameEntity = (GameEntity) new TB_MP_Crossroads();
        break;
      case ScenarioDbId.TB_MAMMOTHPARTY_STORMWIND:
        gameEntity = (GameEntity) new TB16_MP_Stormwind();
        break;
      case ScenarioDbId.ICC_09_SAURFANG:
        gameEntity = (GameEntity) new ICC_09_Saurfang();
        break;
      case ScenarioDbId.TB_100TH:
      case ScenarioDbId.TB_100TH_1P:
        gameEntity = (GameEntity) new TB_100th();
        break;
      case ScenarioDbId.ICC_10_DEATHWHISPER:
        gameEntity = (GameEntity) new ICC_10_Deathwhisper();
        break;
      case ScenarioDbId.TB_FIREFEST_1P:
      case ScenarioDbId.TB_FIREFEST:
        gameEntity = (GameEntity) new TB_FireFest();
        break;
      case ScenarioDbId.TB_FROSTFEST_1P:
      case ScenarioDbId.TB_FROSTFEST:
        gameEntity = (GameEntity) new TB_FrostFestival();
        break;
      case ScenarioDbId.TB_LK_RAID:
        gameEntity = (GameEntity) new TB_LichKingRaid();
        break;
      case ScenarioDbId.LOOT_DUNGEON:
        gameEntity = (GameEntity) LOOT_Dungeon.InstantiateLootDungeonMissionEntityForBoss(powerList, createGame);
        break;
      case ScenarioDbId.TB_HEADLESSHORSEMAN:
        gameEntity = (GameEntity) new TB_HeadlessHorseman();
        break;
      case ScenarioDbId.FB_DUELERSBRAWL_1P:
      case ScenarioDbId.FB_DUELERSBRAWL:
      case ScenarioDbId.FB_EXPANSIONDRAFT:
        gameEntity = (GameEntity) new FB_DuelersBrawl();
        break;
      case ScenarioDbId.TB_HEADLESSREDUX:
        gameEntity = (GameEntity) new TB_HeadlessRedux();
        break;
      case ScenarioDbId.FB_ELOBRAWL:
        gameEntity = (GameEntity) new FB_ELObrawl();
        break;
      case ScenarioDbId.GIL_DUNGEON:
        gameEntity = (GameEntity) GIL_Dungeon.InstantiateGilDungeonMissionEntityForBoss(powerList, createGame);
        break;
      case ScenarioDbId.TB_KOBOLDGIFTS:
        gameEntity = (GameEntity) new TB_KoboldGifts();
        break;
      case ScenarioDbId.TB_MARIN:
        gameEntity = (GameEntity) new TB_Marin();
        break;
      case ScenarioDbId.FB_CHAMPS:
      case ScenarioDbId.FB_CHAMPS_1P:
      case ScenarioDbId.TB_DARWIN_CHAMPS:
        gameEntity = (GameEntity) new FB_Champs();
        break;
      case ScenarioDbId.FB_BUILDABRAWL_1P:
      case ScenarioDbId.FB_BUILDABRAWL:
        gameEntity = (GameEntity) new FB_BuildABrawl();
        break;
      case ScenarioDbId.TB_LETHALPUZZLES_RESTART:
        gameEntity = (GameEntity) new TB13_LethalPuzzles_Restart();
        break;
      case ScenarioDbId.TB_FOXBLESSING:
      case ScenarioDbId.TB_FOXBLESSING_1P:
        gameEntity = (GameEntity) new TB_NewYearRaven();
        break;
      case ScenarioDbId.GIL_BONUS_CHALLENGE:
        gameEntity = (GameEntity) GIL_Dungeon.InstantiateGilDungeonMissionEntityForBoss(powerList, createGame);
        break;
      case ScenarioDbId.FB_TOKICOOP:
      case ScenarioDbId.FB_TOKICOOP_1P:
        gameEntity = (GameEntity) new FB_TokiCoop();
        break;
      case ScenarioDbId.TRL_DUNGEON:
        gameEntity = (GameEntity) TRL_Dungeon.InstantiateTRLDungeonMissionEntityForBoss(powerList, createGame);
        break;
      case ScenarioDbId.TB_FIREFEST2_1P:
      case ScenarioDbId.TB_FIREFEST2:
        gameEntity = (GameEntity) new TB_Firefest2();
        break;
      case ScenarioDbId.BOTA_MIRROR_PUZZLE_1:
        gameEntity = (GameEntity) new BOTA_Mirror_Puzzle_1();
        break;
      case ScenarioDbId.BOTA_SURVIVAL_PUZZLE_1:
        gameEntity = (GameEntity) new BOTA_Survival_Puzzle_1();
        break;
      case ScenarioDbId.BOTA_MIRROR_PUZZLE_2:
        gameEntity = (GameEntity) new BOTA_Mirror_Puzzle_2();
        break;
      case ScenarioDbId.BOTA_MIRROR_PUZZLE_3:
        gameEntity = (GameEntity) new BOTA_Mirror_Puzzle_3();
        break;
      case ScenarioDbId.BOTA_MIRROR_PUZZLE_4:
        gameEntity = (GameEntity) new BOTA_Mirror_Puzzle_4();
        break;
      case ScenarioDbId.BOTA_SURVIVAL_PUZZLE_2:
        gameEntity = (GameEntity) new BOTA_Survival_Puzzle_2();
        break;
      case ScenarioDbId.BOTA_SURVIVAL_PUZZLE_3:
        gameEntity = (GameEntity) new BOTA_Survival_Puzzle_3();
        break;
      case ScenarioDbId.BOTA_SURVIVAL_PUZZLE_4:
        gameEntity = (GameEntity) new BOTA_Survival_Puzzle_4();
        break;
      case ScenarioDbId.BOTA_MIRROR_BOOM:
        gameEntity = (GameEntity) new BOTA_Mirror_Boom();
        break;
      case ScenarioDbId.BOTA_LETHAL_PUZZLE_1:
        gameEntity = (GameEntity) new BOTA_Lethal_Puzzle_1();
        break;
      case ScenarioDbId.BOTA_LETHAL_PUZZLE_2:
        gameEntity = (GameEntity) new BOTA_Lethal_Puzzle_2();
        break;
      case ScenarioDbId.BOTA_LETHAL_PUZZLE_3:
        gameEntity = (GameEntity) new BOTA_Lethal_Puzzle_3();
        break;
      case ScenarioDbId.BOTA_LETHAL_PUZZLE_4:
        gameEntity = (GameEntity) new BOTA_Lethal_Puzzle_4();
        break;
      case ScenarioDbId.BOTA_CLEAR_PUZZLE_1:
        gameEntity = (GameEntity) new BOTA_Clear_Puzzle_1();
        break;
      case ScenarioDbId.BOTA_CLEAR_PUZZLE_2:
        gameEntity = (GameEntity) new BOTA_Clear_Puzzle_2();
        break;
      case ScenarioDbId.BOTA_CLEAR_PUZZLE_3:
        gameEntity = (GameEntity) new BOTA_Clear_Puzzle_3();
        break;
      case ScenarioDbId.BOTA_CLEAR_PUZZLE_4:
        gameEntity = (GameEntity) new BOTA_Clear_Puzzle_4();
        break;
      case ScenarioDbId.BOTA_LETHAL_BOOM:
        gameEntity = (GameEntity) new BOTA_Lethal_Boom();
        break;
      case ScenarioDbId.BOTA_SURVIVAL_BOOM:
        gameEntity = (GameEntity) new BOTA_Survival_Boom();
        break;
      case ScenarioDbId.BOTA_CLEAR_BOOM:
        gameEntity = (GameEntity) new BOTA_Clear_Boom();
        break;
      case ScenarioDbId.DALA_01_BANK:
      case ScenarioDbId.DALA_02_VIOLET_HOLD:
      case ScenarioDbId.DALA_03_STREETS:
      case ScenarioDbId.DALA_04_UNDERBELLY:
      case ScenarioDbId.DALA_05_CITADEL:
      case ScenarioDbId.DALA_01_BANK_HEROIC:
      case ScenarioDbId.DALA_02_VIOLET_HOLD_HEROIC:
      case ScenarioDbId.DALA_03_STREETS_HEROIC:
      case ScenarioDbId.DALA_04_UNDERBELLY_HEROIC:
      case ScenarioDbId.DALA_05_CITADEL_HEROIC:
        gameEntity = (GameEntity) DALA_Dungeon.InstantiateDALADungeonMissionEntityForBoss(powerList, createGame);
        break;
      case ScenarioDbId.TB_TROLLSWEEK1_1P:
      case ScenarioDbId.TB_TROLLSWEEK1:
        gameEntity = (GameEntity) new TB_TrollsWeek1();
        break;
      case ScenarioDbId.DALA_TAVERN:
      case ScenarioDbId.DALA_TAVERN_HEROIC:
        gameEntity = (GameEntity) new DALA_Tavern();
        break;
      case ScenarioDbId.TB_ARCHIVIST_1P:
      case ScenarioDbId.TB_ARCHIVIST:
        gameEntity = (GameEntity) new TB_NoMulligan();
        break;
      case ScenarioDbId.TB_HENCHMANIA_1P:
      case ScenarioDbId.TB_HENCHMANIA:
        gameEntity = (GameEntity) new TB_Henchmania();
        break;
      case ScenarioDbId.TB_IGNOBLEGARDEN:
      case ScenarioDbId.TB_IGNOBLEGARDEN_1P:
        gameEntity = (GameEntity) new TB_Ignoblegarden();
        break;
      case ScenarioDbId.TB_207TH:
      case ScenarioDbId.TB_207TH_1P:
        gameEntity = (GameEntity) new TB_207();
        break;
      case ScenarioDbId.TB_RANDOM_DECK_KEEP_WINNER:
      case ScenarioDbId.TB_SEEDED_BRAWL:
      case ScenarioDbId.TB_CRAZY_DECK_KEEP_WINNER:
      case ScenarioDbId.TB_DUELS_DECK_KEEP_WINNER:
        gameEntity = (GameEntity) new TB_RandomDeckKeepWinnerDeck();
        break;
      case ScenarioDbId.TB_AUTOBRAWL_1P:
      case ScenarioDbId.TB_AUTOBRAWL:
        gameEntity = (GameEntity) new TB_AutoBrawl();
        break;
      case ScenarioDbId.TB_CAROUSEL_1P:
      case ScenarioDbId.TB_CAROUSEL:
        gameEntity = (GameEntity) new TB_Carousel();
        break;
      case ScenarioDbId.TB_DRAWNDISOVERY:
        gameEntity = (GameEntity) new TB_DrawnDiscovery();
        break;
      case ScenarioDbId.TB_FIREFEST3_1P:
      case ScenarioDbId.TB_FIREFEST3:
        gameEntity = (GameEntity) new TB_Firefest3();
        break;
      case ScenarioDbId.TB_BACON_1P:
      case ScenarioDbId.TB_BACONSHOP_8P:
      case ScenarioDbId.TB_BACONSHOP_VS_AI:
        gameEntity = (GameEntity) new TB_BaconShop();
        break;
      case ScenarioDbId.FB_RAGRAID:
        gameEntity = (GameEntity) new FB_RagRaidScript();
        break;
      case ScenarioDbId.TB_MARTINAUTOBRAWL:
        gameEntity = (GameEntity) new TB_MartinAutoBrawl();
        break;
      case ScenarioDbId.TB_BACONHAND_1P:
        gameEntity = (GameEntity) new TB_BaconHand();
        break;
      case ScenarioDbId.ULDA_CITY:
      case ScenarioDbId.ULDA_DESERT:
      case ScenarioDbId.ULDA_TOMB:
      case ScenarioDbId.ULDA_HALLS:
      case ScenarioDbId.ULDA_SANCTUM:
      case ScenarioDbId.ULDA_CITY_HEROIC:
      case ScenarioDbId.ULDA_DESERT_HEROIC:
      case ScenarioDbId.ULDA_TOMB_HEROIC:
      case ScenarioDbId.ULDA_HALLS_HEROIC:
      case ScenarioDbId.ULDA_SANCTUM_HEROIC:
        gameEntity = (GameEntity) ULDA_Dungeon.InstantiateULDADungeonMissionEntityForBoss(powerList, createGame);
        break;
      case ScenarioDbId.ULDA_TAVERN:
      case ScenarioDbId.ULDA_TAVERN_HEROIC:
        gameEntity = (GameEntity) new ULDA_Tavern();
        break;
      case ScenarioDbId.TB_EVILBRM_1:
      case ScenarioDbId.TB_EVILBRM_2:
      case ScenarioDbId.TB_EVILBRM_DEBUG:
        gameEntity = (GameEntity) new TB_EVILBRM();
        break;
      case ScenarioDbId.TB_LEAGUE_REVIVAL:
        gameEntity = (GameEntity) new TB_LEAGUE_REVIVAL();
        break;
      case ScenarioDbId.PVPDR_Season_1:
        gameEntity = (GameEntity) new WizardDuels();
        break;
      case ScenarioDbId.DRGA_Good_01:
      case ScenarioDbId.DRGA_Good_02:
      case ScenarioDbId.DRGA_Good_03:
      case ScenarioDbId.DRGA_Good_04:
      case ScenarioDbId.DRGA_Good_05:
      case ScenarioDbId.DRGA_Good_06:
      case ScenarioDbId.DRGA_Good_07:
      case ScenarioDbId.DRGA_Good_08:
      case ScenarioDbId.DRGA_Good_09:
      case ScenarioDbId.DRGA_Good_10:
      case ScenarioDbId.DRGA_Good_11:
      case ScenarioDbId.DRGA_Good_12:
      case ScenarioDbId.DRGA_Evil_01:
      case ScenarioDbId.DRGA_Evil_02:
      case ScenarioDbId.DRGA_Evil_03:
      case ScenarioDbId.DRGA_Evil_04:
      case ScenarioDbId.DRGA_Evil_05:
      case ScenarioDbId.DRGA_Evil_06:
      case ScenarioDbId.DRGA_Evil_07:
      case ScenarioDbId.DRGA_Evil_08:
      case ScenarioDbId.DRGA_Evil_09:
      case ScenarioDbId.DRGA_Evil_10:
      case ScenarioDbId.DRGA_Evil_11:
      case ScenarioDbId.DRGA_Evil_12:
      case ScenarioDbId.DRGA_Good_01_Heroic:
      case ScenarioDbId.DRGA_Good_02_Heroic:
      case ScenarioDbId.DRGA_Good_03_Heroic:
      case ScenarioDbId.DRGA_Good_04_Heroic:
      case ScenarioDbId.DRGA_Good_05_Heroic:
      case ScenarioDbId.DRGA_Good_06_Heroic:
      case ScenarioDbId.DRGA_Good_07_Heroic:
      case ScenarioDbId.DRGA_Good_08_Heroic:
      case ScenarioDbId.DRGA_Good_09_Heroic:
      case ScenarioDbId.DRGA_Good_10_Heroic:
      case ScenarioDbId.DRGA_Good_11_Heroic:
      case ScenarioDbId.DRGA_Good_12_Heroic:
      case ScenarioDbId.DRGA_Evil_01_Heroic:
      case ScenarioDbId.DRGA_Evil_02_Heroic:
      case ScenarioDbId.DRGA_Evil_03_Heroic:
      case ScenarioDbId.DRGA_Evil_04_Heroic:
      case ScenarioDbId.DRGA_Evil_05_Heroic:
      case ScenarioDbId.DRGA_Evil_06_Heroic:
      case ScenarioDbId.DRGA_Evil_07_Heroic:
      case ScenarioDbId.DRGA_Evil_08_Heroic:
      case ScenarioDbId.DRGA_Evil_09_Heroic:
      case ScenarioDbId.DRGA_Evil_10_Heroic:
      case ScenarioDbId.DRGA_Evil_11_Heroic:
      case ScenarioDbId.DRGA_Evil_12_Heroic:
        gameEntity = (GameEntity) DRGA_Dungeon.InstantiateDRGADungeonMissionEntityForBoss(powerList, createGame);
        break;
      case ScenarioDbId.TB_TEMPLEOUTRUN_1:
      case ScenarioDbId.TB_TEMPLEOUTRUN_2:
        gameEntity = (GameEntity) ULDA_Dungeon.InstantiateULDADungeonMissionEntityForBoss(powerList, createGame);
        break;
      case ScenarioDbId.TB_BACONSHOP_Tutorial:
        gameEntity = (GameEntity) new TB_BaconShop_Tutorial();
        break;
      case ScenarioDbId.ReturningPlayer_Challenge_1:
        gameEntity = (GameEntity) new RP_Fight_01();
        break;
      case ScenarioDbId.ReturningPlayer_Challenge_2:
        gameEntity = (GameEntity) new RP_Fight_02();
        break;
      case ScenarioDbId.ReturningPlayer_Challenge_3:
        gameEntity = (GameEntity) new RP_Fight_03();
        break;
      case ScenarioDbId.TB_ROAD_TO_NR1:
      case ScenarioDbId.TB_ROAD_TO_NR2:
      case ScenarioDbId.TB_ROAD_TO_NR3:
      case ScenarioDbId.TB_ROAD_TO_NR4:
      case ScenarioDbId.TB_ROAD_TO_NR5:
      case ScenarioDbId.TB_ROAD_TO_NR6:
      case ScenarioDbId.TB_ROAD_TO_NR7:
      case ScenarioDbId.TB_ROAD_TO_NR8:
        gameEntity = (GameEntity) new TB_RoadToNR();
        break;
      case ScenarioDbId.TB_ROAD_TO_NR_TAVERN:
        gameEntity = (GameEntity) new TB_RoadToNR_Tavern();
        break;
      case ScenarioDbId.BTA_01_INQUISITOR_DAKREL:
      case ScenarioDbId.BTA_02_XUR_GOTH:
      case ScenarioDbId.BTA_03_ZIXOR:
      case ScenarioDbId.BTA_04_BALTHARAK:
      case ScenarioDbId.BTA_05_KANRETHAD_PRIME:
      case ScenarioDbId.BTA_06_BURGRAK_CRUELCHAIN:
      case ScenarioDbId.BTA_07_FELSTORM_RUN:
      case ScenarioDbId.BTA_08_MOTHER_SHAHRAZ:
      case ScenarioDbId.BTA_09_SHAL_JA_OUTCAST:
      case ScenarioDbId.BTA_10_KARNUK_OUTCAST:
      case ScenarioDbId.BTA_11_JEK_HAZ:
      case ScenarioDbId.BTA_12_MAGTHERIDON_PRIME:
      case ScenarioDbId.BTA_13_GOK_AMOK:
      case ScenarioDbId.BTA_14_FLIKK:
      case ScenarioDbId.BTA_15_BADUU_CORRUPTED:
      case ScenarioDbId.BTA_16_MECHA_JARAXXUS:
      case ScenarioDbId.BTA_17_ILLIDAN_STORMRAGE:
        gameEntity = (GameEntity) BTA_Dungeon.InstantiateBTADungeonMissionEntityForBoss(powerList, createGame);
        break;
      case ScenarioDbId.BTA_Heroic_KAZZAK:
      case ScenarioDbId.BTA_Heroic_GRUUL:
      case ScenarioDbId.BTA_Heroic_MAGTHERIDON:
      case ScenarioDbId.BTA_Heroic_SUPREMUS:
      case ScenarioDbId.BTA_Heroic_TERON_GOREFIEND:
      case ScenarioDbId.BTA_Heroic_MOTHER_SHARAZ:
      case ScenarioDbId.BTA_Heroic_LADY_VASHJ:
      case ScenarioDbId.BTA_Heroic_KAELTHAS:
      case ScenarioDbId.BTA_Heroic_ILLIDAN:
        gameEntity = (GameEntity) BTA_Dungeon_Heroic.InstantiateBTADungeonMissionEntityForBoss(powerList, createGame);
        break;
      case ScenarioDbId.TB_SPT_DALA_1P:
      case ScenarioDbId.TB_SPT_DALA:
        gameEntity = (GameEntity) new TB_SPT_DALA();
        break;
      case ScenarioDbId.BTP_01_AZZINOTH:
        gameEntity = (GameEntity) new BTA_Prologue_Fight_01();
        break;
      case ScenarioDbId.BTP_02_XAVIUS:
        gameEntity = (GameEntity) new BTA_Prologue_Fight_02();
        break;
      case ScenarioDbId.BTP_03_MANNOROTH:
        gameEntity = (GameEntity) new BTA_Prologue_Fight_03();
        break;
      case ScenarioDbId.BTP_04_CENARIUS:
        gameEntity = (GameEntity) new BTA_Prologue_Fight_04();
        break;
      case ScenarioDbId.TB_RumbleDome:
      case ScenarioDbId.TB_Rumbledome_1p:
        gameEntity = (GameEntity) new TB_RumbleDome();
        break;
      case ScenarioDbId.BOH_JAINA_01:
        gameEntity = (GameEntity) new BoH_Jaina_01();
        break;
      case ScenarioDbId.BOH_JAINA_02:
        gameEntity = (GameEntity) new BoH_Jaina_02();
        break;
      case ScenarioDbId.BOH_JAINA_03:
        gameEntity = (GameEntity) new BoH_Jaina_03();
        break;
      case ScenarioDbId.BOH_JAINA_04:
        gameEntity = (GameEntity) new BoH_Jaina_04();
        break;
      case ScenarioDbId.BOH_JAINA_05:
        gameEntity = (GameEntity) new BoH_Jaina_05();
        break;
      case ScenarioDbId.BOH_JAINA_06:
        gameEntity = (GameEntity) new BoH_Jaina_06();
        break;
      case ScenarioDbId.BOH_JAINA_07:
        gameEntity = (GameEntity) new BoH_Jaina_07();
        break;
      case ScenarioDbId.BOH_JAINA_08:
        gameEntity = (GameEntity) new BoH_Jaina_08();
        break;
      case ScenarioDbId.LETTUCE_1v1:
      case ScenarioDbId.LETTUCE_PVP_VS_AI:
        gameEntity = (GameEntity) new LettucePvPMissionEntity();
        break;
      case ScenarioDbId.LETTUCE_DEV_TEST_VS_AI:
      case ScenarioDbId.LETTUCE_DEV_TEST_COOP_VS_AI:
        gameEntity = (GameEntity) new LettucePvEMissionEntity(true);
        break;
      case ScenarioDbId.BOH_REXXAR_01:
        gameEntity = (GameEntity) new BoH_Rexxar_01();
        break;
      case ScenarioDbId.BOH_REXXAR_02:
        gameEntity = (GameEntity) new BoH_Rexxar_02();
        break;
      case ScenarioDbId.BOH_REXXAR_03:
        gameEntity = (GameEntity) new BoH_Rexxar_03();
        break;
      case ScenarioDbId.BOH_REXXAR_04:
        gameEntity = (GameEntity) new BoH_Rexxar_04();
        break;
      case ScenarioDbId.BOH_REXXAR_05:
        gameEntity = (GameEntity) new BoH_Rexxar_05();
        break;
      case ScenarioDbId.BOH_REXXAR_06:
        gameEntity = (GameEntity) new BoH_Rexxar_06();
        break;
      case ScenarioDbId.BOH_REXXAR_07:
        gameEntity = (GameEntity) new BoH_Rexxar_07();
        break;
      case ScenarioDbId.BOH_REXXAR_08:
        gameEntity = (GameEntity) new BoH_Rexxar_08();
        break;
      case ScenarioDbId.LETTUCE_PVE_TUTORIAL_1:
        gameEntity = (GameEntity) new LettuceTutorialOneMissionEntity();
        break;
      case ScenarioDbId.LETTUCE_PVE_TUTORIAL_BOSS:
        gameEntity = (GameEntity) new LettuceTutorialBossMissionEntity();
        break;
      case ScenarioDbId.LETTUCE_MAP:
      case ScenarioDbId.LETTUCE_MAP_COOP:
        gameEntity = (GameEntity) LettuceBossMissionEntity.InstantiateLettuceBountyMissionEntityForBoss(powerList);
        break;
      case ScenarioDbId.BOH_GARROSH_01:
        gameEntity = (GameEntity) new BoH_Garrosh_01();
        break;
      case ScenarioDbId.BOH_GARROSH_02:
        gameEntity = (GameEntity) new BoH_Garrosh_02();
        break;
      case ScenarioDbId.BOH_GARROSH_03:
        gameEntity = (GameEntity) new BoH_Garrosh_03();
        break;
      case ScenarioDbId.BOH_GARROSH_04:
        gameEntity = (GameEntity) new BoH_Garrosh_04();
        break;
      case ScenarioDbId.BOH_GARROSH_05:
        gameEntity = (GameEntity) new BoH_Garrosh_05();
        break;
      case ScenarioDbId.BOH_GARROSH_06:
        gameEntity = (GameEntity) new BoH_Garrosh_06();
        break;
      case ScenarioDbId.BOH_GARROSH_07:
        gameEntity = (GameEntity) new BoH_Garrosh_07();
        break;
      case ScenarioDbId.BOH_GARROSH_08:
        gameEntity = (GameEntity) new BoH_Garrosh_08();
        break;
      case ScenarioDbId.BOH_UTHER_01:
        gameEntity = (GameEntity) new BoH_Uther_01();
        break;
      case ScenarioDbId.BOH_UTHER_02:
        gameEntity = (GameEntity) new BoH_Uther_02();
        break;
      case ScenarioDbId.BOH_UTHER_03:
        gameEntity = (GameEntity) new BoH_Uther_03();
        break;
      case ScenarioDbId.BOH_UTHER_04:
        gameEntity = (GameEntity) new BoH_Uther_04();
        break;
      case ScenarioDbId.BOH_UTHER_05:
        gameEntity = (GameEntity) new BoH_Uther_05();
        break;
      case ScenarioDbId.BOH_UTHER_06:
        gameEntity = (GameEntity) new BoH_Uther_06();
        break;
      case ScenarioDbId.BOH_UTHER_07:
        gameEntity = (GameEntity) new BoH_Uther_07();
        break;
      case ScenarioDbId.BOH_UTHER_08:
        gameEntity = (GameEntity) new BoH_Uther_08();
        break;
      case ScenarioDbId.BOH_ANDUIN_01:
        gameEntity = (GameEntity) new BoH_Anduin_01();
        break;
      case ScenarioDbId.BOH_ANDUIN_02:
        gameEntity = (GameEntity) new BoH_Anduin_02();
        break;
      case ScenarioDbId.BOH_ANDUIN_03:
        gameEntity = (GameEntity) new BoH_Anduin_03();
        break;
      case ScenarioDbId.BOH_ANDUIN_04:
        gameEntity = (GameEntity) new BoH_Anduin_04();
        break;
      case ScenarioDbId.BOH_ANDUIN_05:
        gameEntity = (GameEntity) new BoH_Anduin_05();
        break;
      case ScenarioDbId.BOH_ANDUIN_06:
        gameEntity = (GameEntity) new BoH_Anduin_06();
        break;
      case ScenarioDbId.BOH_ANDUIN_07:
        gameEntity = (GameEntity) new BoH_Anduin_07();
        break;
      case ScenarioDbId.BOH_ANDUIN_08:
        gameEntity = (GameEntity) new BoH_Anduin_08();
        break;
      case ScenarioDbId.BOM_01_Rokara_01:
        gameEntity = (GameEntity) new BOM_01_Rokara_01();
        break;
      case ScenarioDbId.BOM_01_Rokara_02:
        gameEntity = (GameEntity) new BOM_01_Rokara_02();
        break;
      case ScenarioDbId.BOM_01_Rokara_03:
        gameEntity = (GameEntity) new BOM_01_Rokara_03();
        break;
      case ScenarioDbId.BOM_01_Rokara_04:
        gameEntity = (GameEntity) new BOM_01_Rokara_04();
        break;
      case ScenarioDbId.BOM_01_Rokara_05:
        gameEntity = (GameEntity) new BOM_01_Rokara_05();
        break;
      case ScenarioDbId.BOM_01_Rokara_06:
        gameEntity = (GameEntity) new BOM_01_Rokara_06();
        break;
      case ScenarioDbId.BOM_01_Rokara_07:
        gameEntity = (GameEntity) new BOM_01_Rokara_07();
        break;
      case ScenarioDbId.BOM_01_Rokara_08:
        gameEntity = (GameEntity) new BOM_01_Rokara_08();
        break;
      case ScenarioDbId.BOH_VALEERA_01:
        gameEntity = (GameEntity) new BoH_Valeera_01();
        break;
      case ScenarioDbId.BOH_VALEERA_02:
        gameEntity = (GameEntity) new BoH_Valeera_02();
        break;
      case ScenarioDbId.BOH_VALEERA_03:
        gameEntity = (GameEntity) new BoH_Valeera_03();
        break;
      case ScenarioDbId.BOH_VALEERA_04:
        gameEntity = (GameEntity) new BoH_Valeera_04();
        break;
      case ScenarioDbId.BOH_VALEERA_05:
        gameEntity = (GameEntity) new BoH_Valeera_05();
        break;
      case ScenarioDbId.BOH_VALEERA_06:
        gameEntity = (GameEntity) new BoH_Valeera_06();
        break;
      case ScenarioDbId.BOH_VALEERA_07:
        gameEntity = (GameEntity) new BoH_Valeera_07();
        break;
      case ScenarioDbId.BOH_VALEERA_08:
        gameEntity = (GameEntity) new BoH_Valeera_08();
        break;
      case ScenarioDbId.LETTUCE_TAVERN:
        gameEntity = (GameEntity) new LettuceTavernMissionEntity();
        break;
      case ScenarioDbId.BOH_THRALL_01:
        gameEntity = (GameEntity) new BoH_Thrall_01();
        break;
      case ScenarioDbId.BOH_THRALL_02:
        gameEntity = (GameEntity) new BoH_Thrall_02();
        break;
      case ScenarioDbId.BOH_THRALL_03:
        gameEntity = (GameEntity) new BoH_Thrall_03();
        break;
      case ScenarioDbId.BOH_THRALL_04:
        gameEntity = (GameEntity) new BoH_Thrall_04();
        break;
      case ScenarioDbId.BOH_THRALL_05:
        gameEntity = (GameEntity) new BoH_Thrall_05();
        break;
      case ScenarioDbId.BOH_THRALL_06:
        gameEntity = (GameEntity) new BoH_Thrall_06();
        break;
      case ScenarioDbId.BOH_THRALL_07:
        gameEntity = (GameEntity) new BoH_Thrall_07();
        break;
      case ScenarioDbId.BOH_THRALL_08:
        gameEntity = (GameEntity) new BoH_Thrall_08();
        break;
      case ScenarioDbId.LETTUCE_PVE_TUTORIAL_2:
        gameEntity = (GameEntity) new LettuceTutorialTwoMissionEntity();
        break;
      case ScenarioDbId.LETTUCE_PVE_TUTORIAL_3:
        gameEntity = (GameEntity) new LettuceTutorialThreeMissionEntity();
        break;
      case ScenarioDbId.BOH_MALFURION_01:
        gameEntity = (GameEntity) new BoH_Malfurion_01();
        break;
      case ScenarioDbId.BOH_MALFURION_02:
        gameEntity = (GameEntity) new BoH_Malfurion_02();
        break;
      case ScenarioDbId.BOH_MALFURION_03:
        gameEntity = (GameEntity) new BoH_Malfurion_03();
        break;
      case ScenarioDbId.BOH_MALFURION_04:
        gameEntity = (GameEntity) new BoH_Malfurion_04();
        break;
      case ScenarioDbId.BOH_MALFURION_05:
        gameEntity = (GameEntity) new BoH_Malfurion_05();
        break;
      case ScenarioDbId.BOH_MALFURION_06:
        gameEntity = (GameEntity) new BoH_Malfurion_06();
        break;
      case ScenarioDbId.BOH_MALFURION_07:
        gameEntity = (GameEntity) new BoH_Malfurion_07();
        break;
      case ScenarioDbId.BOH_MALFURION_08:
        gameEntity = (GameEntity) new BoH_Malfurion_08();
        break;
      case ScenarioDbId.BOM_02_Xyrella_01:
        gameEntity = (GameEntity) new BOM_02_Xyrella_Fight_01();
        break;
      case ScenarioDbId.BOM_02_Xyrella_02:
        gameEntity = (GameEntity) new BOM_02_Xyrella_Fight_02();
        break;
      case ScenarioDbId.BOM_02_Xyrella_03:
        gameEntity = (GameEntity) new BOM_02_Xyrella_Fight_03();
        break;
      case ScenarioDbId.BOM_02_Xyrella_04:
        gameEntity = (GameEntity) new BOM_02_Xyrella_Fight_04();
        break;
      case ScenarioDbId.BOM_02_Xyrella_05:
        gameEntity = (GameEntity) new BOM_02_Xyrella_Fight_05();
        break;
      case ScenarioDbId.BOM_02_Xyrella_06:
        gameEntity = (GameEntity) new BOM_02_Xyrella_Fight_06();
        break;
      case ScenarioDbId.BOM_02_Xyrella_07:
        gameEntity = (GameEntity) new BOM_02_Xyrella_Fight_07();
        break;
      case ScenarioDbId.BOM_02_Xyrella_08:
        gameEntity = (GameEntity) new BOM_02_Xyrella_Fight_08();
        break;
      case ScenarioDbId.LETTUCE_PVE_TUTORIAL_4:
        gameEntity = (GameEntity) new LettuceTutorialFourMissionEntity();
        break;
      case ScenarioDbId.BOM_03_Guff_01:
        gameEntity = (GameEntity) new BOM_03_Guff_Fight_01();
        break;
      case ScenarioDbId.BOM_03_Guff_02:
        gameEntity = (GameEntity) new BOM_03_Guff_Fight_02();
        break;
      case ScenarioDbId.BOM_03_Guff_03:
        gameEntity = (GameEntity) new BOM_03_Guff_Fight_03();
        break;
      case ScenarioDbId.BOM_03_Guff_04:
        gameEntity = (GameEntity) new BOM_03_Guff_Fight_04();
        break;
      case ScenarioDbId.BOM_03_Guff_05:
        gameEntity = (GameEntity) new BOM_03_Guff_Fight_05();
        break;
      case ScenarioDbId.BOM_03_Guff_06:
        gameEntity = (GameEntity) new BOM_03_Guff_Fight_06();
        break;
      case ScenarioDbId.BOM_03_Guff_07:
        gameEntity = (GameEntity) new BOM_03_Guff_Fight_07();
        break;
      case ScenarioDbId.BOM_03_Guff_08:
        gameEntity = (GameEntity) new BOM_03_Guff_Fight_08();
        break;
      case ScenarioDbId.BOH_GULDAN_01:
        gameEntity = (GameEntity) new BoH_Guldan_01();
        break;
      case ScenarioDbId.BOH_GULDAN_02:
        gameEntity = (GameEntity) new BoH_Guldan_02();
        break;
      case ScenarioDbId.BOH_GULDAN_03:
        gameEntity = (GameEntity) new BoH_Guldan_03();
        break;
      case ScenarioDbId.BOH_GULDAN_04:
        gameEntity = (GameEntity) new BoH_Guldan_04();
        break;
      case ScenarioDbId.BOH_GULDAN_05:
        gameEntity = (GameEntity) new BoH_Guldan_05();
        break;
      case ScenarioDbId.BOH_GULDAN_06:
        gameEntity = (GameEntity) new BoH_Guldan_06();
        break;
      case ScenarioDbId.BOH_GULDAN_07:
        gameEntity = (GameEntity) new BoH_Guldan_07();
        break;
      case ScenarioDbId.BOH_GULDAN_08:
        gameEntity = (GameEntity) new BoH_Guldan_08();
        break;
      case ScenarioDbId.BOM_04_Kurtrus_01:
        gameEntity = (GameEntity) new BOM_04_Kurtrus_Fight_01();
        break;
      case ScenarioDbId.BOM_04_Kurtrus_02:
        gameEntity = (GameEntity) new BOM_04_Kurtrus_Fight_02();
        break;
      case ScenarioDbId.BOM_04_Kurtrus_03:
        gameEntity = (GameEntity) new BOM_04_Kurtrus_Fight_03();
        break;
      case ScenarioDbId.BOM_04_Kurtrus_04:
        gameEntity = (GameEntity) new BOM_04_Kurtrus_Fight_04();
        break;
      case ScenarioDbId.BOM_04_Kurtrus_05:
        gameEntity = (GameEntity) new BOM_04_Kurtrus_Fight_05();
        break;
      case ScenarioDbId.BOM_04_Kurtrus_06:
        gameEntity = (GameEntity) new BOM_04_Kurtrus_Fight_06();
        break;
      case ScenarioDbId.BOM_04_Kurtrus_07:
        gameEntity = (GameEntity) new BOM_04_Kurtrus_Fight_07();
        break;
      case ScenarioDbId.BOM_04_Kurtrus_08:
        gameEntity = (GameEntity) new BOM_04_Kurtrus_Fight_08();
        break;
      case ScenarioDbId.BOH_ILLIDAN_01:
        gameEntity = (GameEntity) new BoH_Illidan_01();
        break;
      case ScenarioDbId.BOH_ILLIDAN_02:
        gameEntity = (GameEntity) new BoH_Illidan_02();
        break;
      case ScenarioDbId.BOH_ILLIDAN_03:
        gameEntity = (GameEntity) new BoH_Illidan_03();
        break;
      case ScenarioDbId.BOH_ILLIDAN_04:
        gameEntity = (GameEntity) new BoH_Illidan_04();
        break;
      case ScenarioDbId.BOH_ILLIDAN_05:
        gameEntity = (GameEntity) new BoH_Illidan_05();
        break;
      case ScenarioDbId.BOH_ILLIDAN_06:
        gameEntity = (GameEntity) new BoH_Illidan_06();
        break;
      case ScenarioDbId.BOH_ILLIDAN_07:
        gameEntity = (GameEntity) new BoH_Illidan_07();
        break;
      case ScenarioDbId.BOH_ILLIDAN_08:
        gameEntity = (GameEntity) new BoH_Illidan_08();
        break;
      case ScenarioDbId.BOM_05_Tamsin_001:
        gameEntity = (GameEntity) new BOM_05_Tamsin_Fight_001();
        break;
      case ScenarioDbId.BOM_05_Tamsin_002:
        gameEntity = (GameEntity) new BOM_05_Tamsin_Fight_002();
        break;
      case ScenarioDbId.BOM_05_Tamsin_003:
        gameEntity = (GameEntity) new BOM_05_Tamsin_Fight_003();
        break;
      case ScenarioDbId.BOM_05_Tamsin_004:
        gameEntity = (GameEntity) new BOM_05_Tamsin_Fight_004();
        break;
      case ScenarioDbId.BOM_05_Tamsin_005:
        gameEntity = (GameEntity) new BOM_05_Tamsin_Fight_005();
        break;
      case ScenarioDbId.BOM_05_Tamsin_006:
        gameEntity = (GameEntity) new BOM_05_Tamsin_Fight_006();
        break;
      case ScenarioDbId.BOM_05_Tamsin_007:
        gameEntity = (GameEntity) new BOM_05_Tamsin_Fight_007();
        break;
      case ScenarioDbId.BOM_05_Tamsin_008:
        gameEntity = (GameEntity) new BOM_05_Tamsin_Fight_008();
        break;
      case ScenarioDbId.BOM_06_Cariel_001:
        gameEntity = (GameEntity) new BOM_06_Cariel_Fight_001();
        break;
      case ScenarioDbId.BOM_06_Cariel_002:
        gameEntity = (GameEntity) new BOM_06_Cariel_Fight_002();
        break;
      case ScenarioDbId.BOM_06_Cariel_003:
        gameEntity = (GameEntity) new BOM_06_Cariel_Fight_003();
        break;
      case ScenarioDbId.BOM_06_Cariel_004:
        gameEntity = (GameEntity) new BOM_06_Cariel_Fight_004();
        break;
      case ScenarioDbId.BOM_06_Cariel_005:
        gameEntity = (GameEntity) new BOM_06_Cariel_Fight_005();
        break;
      case ScenarioDbId.BOM_06_Cariel_006:
        gameEntity = (GameEntity) new BOM_06_Cariel_Fight_006();
        break;
      case ScenarioDbId.BOM_06_Cariel_007:
        gameEntity = (GameEntity) new BOM_06_Cariel_Fight_007();
        break;
      case ScenarioDbId.BOM_06_Cariel_008:
        gameEntity = (GameEntity) new BOM_06_Cariel_Fight_008();
        break;
      case ScenarioDbId.BOH_FAELIN_STORY_PROLOGUE2:
        gameEntity = (GameEntity) new BoH_Faelin_Story_Prologue2();
        break;
      case ScenarioDbId.BOH_FAELIN_01:
        gameEntity = (GameEntity) new BoH_Faelin_01();
        break;
      case ScenarioDbId.BOH_FAELIN_02:
        gameEntity = (GameEntity) new BoH_Faelin_02();
        break;
      case ScenarioDbId.BOH_FAELIN_03:
        gameEntity = (GameEntity) new BoH_Faelin_03();
        break;
      case ScenarioDbId.BOH_FAELIN_04:
        gameEntity = (GameEntity) new BoH_Faelin_04();
        break;
      case ScenarioDbId.BOH_FAELIN_05A:
        gameEntity = (GameEntity) new BoH_Faelin_05A();
        break;
      case ScenarioDbId.BOH_FAELIN_05B:
        gameEntity = (GameEntity) new BoH_Faelin_05B();
        break;
      case ScenarioDbId.BOH_FAELIN_06:
        gameEntity = (GameEntity) new BoH_Faelin_06();
        break;
      case ScenarioDbId.BOH_FAELIN_07:
        gameEntity = (GameEntity) new BoH_Faelin_07();
        break;
      case ScenarioDbId.BOH_FAELIN_08:
        gameEntity = (GameEntity) new BoH_Faelin_08();
        break;
      case ScenarioDbId.BOH_FAELIN_09A:
        gameEntity = (GameEntity) new BoH_Faelin_09A();
        break;
      case ScenarioDbId.BOH_FAELIN_09B:
        gameEntity = (GameEntity) new BoH_Faelin_09B();
        break;
      case ScenarioDbId.BOH_FAELIN_10A:
        gameEntity = (GameEntity) new BoH_Faelin_10A();
        break;
      case ScenarioDbId.BOH_FAELIN_10B:
        gameEntity = (GameEntity) new BoH_Faelin_10B();
        break;
      case ScenarioDbId.BOH_FAELIN_11:
        gameEntity = (GameEntity) new BoH_Faelin_11();
        break;
      case ScenarioDbId.BOH_FAELIN_12:
        gameEntity = (GameEntity) new BoH_Faelin_12();
        break;
      case ScenarioDbId.BOH_FAELIN_13:
        gameEntity = (GameEntity) new BoH_Faelin_13();
        break;
      case ScenarioDbId.BOH_FAELIN_14:
        gameEntity = (GameEntity) new BoH_Faelin_14();
        break;
      case ScenarioDbId.BOH_FAELIN_15:
        gameEntity = (GameEntity) new BoH_Faelin_15();
        break;
      case ScenarioDbId.BOH_FAELIN_16:
        gameEntity = (GameEntity) new BoH_Faelin_16();
        break;
      case ScenarioDbId.BOM_07_Scabbs_Fight_001:
        gameEntity = (GameEntity) new BOM_07_Scabbs_Fight_001();
        break;
      case ScenarioDbId.BOM_07_Scabbs_Fight_006:
        gameEntity = (GameEntity) new BOM_07_Scabbs_Fight_006();
        break;
      case ScenarioDbId.TB_01_BOOKOFMERCS:
        gameEntity = (GameEntity) new TB_01_BOM_Mercs_Fight_001();
        break;
      case ScenarioDbId.BOM_07_Scabbs_Fight_007:
        gameEntity = (GameEntity) new BOM_07_Scabbs_Fight_007();
        break;
      case ScenarioDbId.BOM_07_Scabbs_Fight_008:
        gameEntity = (GameEntity) new BOM_07_Scabbs_Fight_008();
        break;
      case ScenarioDbId.BOM_07_Scabbs_Fight_005:
        gameEntity = (GameEntity) new BOM_07_Scabbs_Fight_005();
        break;
      case ScenarioDbId.BOM_07_Scabbs_Fight_002:
        gameEntity = (GameEntity) new BOM_07_Scabbs_Fight_002();
        break;
      case ScenarioDbId.BOM_07_Scabbs_Fight_003:
        gameEntity = (GameEntity) new BOM_07_Scabbs_Fight_003();
        break;
      case ScenarioDbId.BOM_07_Scabbs_Fight_004:
        gameEntity = (GameEntity) new BOM_07_Scabbs_Fight_004();
        break;
      case ScenarioDbId.BOM_08_Tavish_Fight_001:
        gameEntity = (GameEntity) new BOM_08_Tavish_Fight_001();
        break;
      case ScenarioDbId.BOM_08_Tavish_Fight_002:
        gameEntity = (GameEntity) new BOM_08_Tavish_Fight_002();
        break;
      case ScenarioDbId.BOM_08_Tavish_Fight_003:
        gameEntity = (GameEntity) new BOM_08_Tavish_Fight_003();
        break;
      case ScenarioDbId.BOM_08_Tavish_Fight_004:
        gameEntity = (GameEntity) new BOM_08_Tavish_Fight_004();
        break;
      case ScenarioDbId.BOM_08_Tavish_Fight_005:
        gameEntity = (GameEntity) new BOM_08_Tavish_Fight_005();
        break;
      case ScenarioDbId.BOM_08_Tavish_Fight_006:
        gameEntity = (GameEntity) new BOM_08_Tavish_Fight_006();
        break;
      case ScenarioDbId.BOM_08_Tavish_Fight_007:
        gameEntity = (GameEntity) new BOM_08_Tavish_Fight_007();
        break;
      case ScenarioDbId.BOM_08_Tavish_Fight_008:
        gameEntity = (GameEntity) new BOM_08_Tavish_Fight_008();
        break;
      case ScenarioDbId.BOM_09_Brukan_Fight_001:
        gameEntity = (GameEntity) new BOM_09_Brukan_Fight_001();
        break;
      case ScenarioDbId.BOM_09_Brukan_Fight_002:
        gameEntity = (GameEntity) new BOM_09_Brukan_Fight_002();
        break;
      case ScenarioDbId.BOM_09_Brukan_Fight_003:
        gameEntity = (GameEntity) new BOM_09_Brukan_Fight_003();
        break;
      case ScenarioDbId.BOM_09_Brukan_Fight_004:
        gameEntity = (GameEntity) new BOM_09_Brukan_Fight_004();
        break;
      case ScenarioDbId.BOM_09_Brukan_Fight_005:
        gameEntity = (GameEntity) new BOM_09_Brukan_Fight_005();
        break;
      case ScenarioDbId.BOM_09_Brukan_Fight_006:
        gameEntity = (GameEntity) new BOM_09_Brukan_Fight_006();
        break;
      case ScenarioDbId.BOM_09_Brukan_Fight_007:
        gameEntity = (GameEntity) new BOM_09_Brukan_Fight_007();
        break;
      case ScenarioDbId.BOM_09_Brukan_Fight_008:
        gameEntity = (GameEntity) new BOM_09_Brukan_Fight_008();
        break;
      case ScenarioDbId.BOM_10_Dawngrasp_Fight_001:
        gameEntity = (GameEntity) new BOM_10_Dawngrasp_Fight_001();
        break;
      case ScenarioDbId.BOM_10_Dawngrasp_Fight_002:
        gameEntity = (GameEntity) new BOM_10_Dawngrasp_Fight_002();
        break;
      case ScenarioDbId.BOM_10_Dawngrasp_Fight_003:
        gameEntity = (GameEntity) new BOM_10_Dawngrasp_Fight_003();
        break;
      case ScenarioDbId.BOM_10_Dawngrasp_Fight_004:
        gameEntity = (GameEntity) new BOM_10_Dawngrasp_Fight_004();
        break;
      case ScenarioDbId.BOM_10_Dawngrasp_Fight_005:
        gameEntity = (GameEntity) new BOM_10_Dawngrasp_Fight_005();
        break;
      case ScenarioDbId.BOM_10_Dawngrasp_Fight_006:
        gameEntity = (GameEntity) new BOM_10_Dawngrasp_Fight_006();
        break;
      case ScenarioDbId.BOM_10_Dawngrasp_Fight_007:
        gameEntity = (GameEntity) new BOM_10_Dawngrasp_Fight_007();
        break;
      case ScenarioDbId.BOM_10_Dawngrasp_Fight_008:
        gameEntity = (GameEntity) new BOM_10_Dawngrasp_Fight_008();
        break;
      case ScenarioDbId.RLK_PROLOGUE_01:
        gameEntity = (GameEntity) new RLK_Prologue_Fight_001();
        break;
      case ScenarioDbId.RLK_PROLOGUE_02:
        gameEntity = (GameEntity) new RLK_Prologue_Fight_002();
        break;
      case ScenarioDbId.RLK_PROLOGUE_03:
        gameEntity = (GameEntity) new RLK_Prologue_Fight_003();
        break;
      case ScenarioDbId.RLK_PROLOGUE_04:
        gameEntity = (GameEntity) new RLK_Prologue_Fight_004();
        break;
      case ScenarioDbId.TB_MagicalGuardians_Fight_001:
        gameEntity = (GameEntity) new TB_MagicalGuardians_Fight_001();
        break;
      default:
        gameEntity = (GameEntity) new StandardGameEntity();
        break;
    }
    gameEntity.OnCreateGame();
    return gameEntity;
  }

  public bool IsAI() => GameUtils.IsAIMission(this.m_missionId);

  public bool WasAI() => GameUtils.IsAIMission(this.m_prevMissionId);

  public bool IsNextAI() => GameUtils.IsAIMission(this.m_nextMissionId);

  public bool IsTraditionalTutorial() => GameUtils.IsTutorialMission(this.m_missionId);

  public bool WasTutorial() => GameUtils.IsTutorialMission(this.m_prevMissionId);

  public bool IsNextTutorial() => GameUtils.IsTutorialMission(this.m_nextMissionId);

  public bool IsLettuceTutorial() => this.m_missionId == 3778 || this.m_missionId == 3900 || this.m_missionId == 3901 || this.m_missionId == 3779;

  public bool IsPractice() => GameUtils.IsPracticeMission(this.m_missionId);

  public bool WasPractice() => GameUtils.IsPracticeMission(this.m_prevMissionId);

  public bool IsNextPractice() => GameUtils.IsPracticeMission(this.m_nextMissionId);

  public bool IsClassChallengeMission() => GameUtils.IsClassChallengeMission(this.m_missionId);

  public bool IsHeroicMission() => GameUtils.IsHeroicAdventureMission(this.m_missionId);

  public bool IsExpansionMission() => GameUtils.IsExpansionMission(this.m_missionId);

  public bool WasExpansionMission() => GameUtils.IsExpansionMission(this.m_prevMissionId);

  public bool IsNextExpansionMission() => GameUtils.IsExpansionMission(this.m_nextMissionId);

  public bool IsDungeonCrawlMission() => GameUtils.IsDungeonCrawlMission(this.m_missionId);

  public bool WasDungeonCrawlMission() => GameUtils.IsDungeonCrawlMission(this.m_prevMissionId);

  public bool IsNextDungeonCrawlMission() => GameUtils.IsDungeonCrawlMission(this.m_nextMissionId);

  public bool IsPlay() => this.IsRankedPlay() || this.IsUnrankedPlay();

  public bool WasPlay() => this.WasRankedPlay() || this.WasUnrankedPlay();

  public bool IsNextPlay() => this.IsNextRankedPlay() || this.IsNextUnrankedPlay();

  public bool IsRankedPlay() => this.m_gameType == GameType.GT_RANKED;

  public bool WasRankedPlay() => this.m_prevGameType == GameType.GT_RANKED;

  public bool IsNextRankedPlay() => this.m_nextGameType == GameType.GT_RANKED;

  public bool IsUnrankedPlay() => this.m_gameType == GameType.GT_CASUAL;

  public bool WasUnrankedPlay() => this.m_prevGameType == GameType.GT_CASUAL;

  public bool IsNextUnrankedPlay() => this.m_nextGameType == GameType.GT_CASUAL;

  public bool IsArena() => this.m_gameType == GameType.GT_ARENA;

  public bool WasArena() => this.m_prevGameType == GameType.GT_ARENA;

  public bool IsNextArena() => this.m_nextGameType == GameType.GT_ARENA;

  public bool IsFriendly() => this.m_gameType == GameType.GT_VS_FRIEND || this.m_gameType == GameType.GT_FSG_BRAWL_VS_FRIEND;

  public bool WasFriendly() => this.m_prevGameType == GameType.GT_VS_FRIEND || this.m_gameType == GameType.GT_FSG_BRAWL_VS_FRIEND;

  public bool IsNextFriendly() => this.m_nextGameType == GameType.GT_VS_FRIEND || this.m_gameType == GameType.GT_FSG_BRAWL_VS_FRIEND;

  public bool WasTavernBrawl() => GameUtils.IsTavernBrawlGameType(this.m_prevGameType) && !this.WasFriendly();

  public bool IsTavernBrawl() => GameUtils.IsTavernBrawlGameType(this.m_gameType) && !this.IsFriendly();

  public bool IsNextTavernBrawl() => GameUtils.IsTavernBrawlGameType(this.m_nextGameType) && !this.IsNextFriendly();

  public bool IsBattlegrounds() => this.m_gameType == GameType.GT_BATTLEGROUNDS || this.m_gameType == GameType.GT_BATTLEGROUNDS_FRIENDLY || this.m_gameType == GameType.GT_BATTLEGROUNDS_PLAYER_VS_AI || this.m_gameType == GameType.GT_BATTLEGROUNDS_AI_VS_AI;

  public bool WasBattlegrounds() => this.m_prevGameType == GameType.GT_BATTLEGROUNDS || this.m_prevGameType == GameType.GT_BATTLEGROUNDS_FRIENDLY || this.m_prevGameType == GameType.GT_BATTLEGROUNDS_PLAYER_VS_AI || this.m_prevGameType == GameType.GT_BATTLEGROUNDS_AI_VS_AI;

  public bool IsBattlegroundsTutorial() => this.m_gameType == GameType.GT_VS_AI && this.m_missionId == 3539;

  public bool IsBattlegroundsMatchOrTutorial() => this.IsBattlegrounds() || this.IsBattlegroundsTutorial();

  public bool IsBattlegroundVsAIGame() => this.m_gameType == GameType.GT_BATTLEGROUNDS_PLAYER_VS_AI;

  public bool IsFriendlyBattlegrounds() => this.m_gameType == GameType.GT_BATTLEGROUNDS_FRIENDLY;

  public bool IsStandardFormatType() => this.m_formatType == PegasusShared.FormatType.FT_STANDARD;

  public bool IsWildFormatType() => this.m_formatType == PegasusShared.FormatType.FT_WILD;

  public bool IsClassicFormatType() => this.m_formatType == PegasusShared.FormatType.FT_CLASSIC;

  public bool IsNextWildFormatType() => this.m_nextFormatType == PegasusShared.FormatType.FT_WILD;

  public bool IsDuels() => this.m_gameType == GameType.GT_PVPDR || this.m_gameType == GameType.GT_PVPDR_PAID;

  public bool WasDuels() => this.m_prevGameType == GameType.GT_PVPDR || this.m_prevGameType == GameType.GT_PVPDR_PAID;

  public bool IsMercenaries() => this.m_gameType == GameType.GT_MERCENARIES_PVP || this.m_gameType == GameType.GT_MERCENARIES_PVE || this.m_gameType == GameType.GT_MERCENARIES_PVE_COOP || this.m_gameType == GameType.GT_MERCENARIES_FRIENDLY;

  private SceneMgr.Mode GetSpectatorPostGameSceneMode()
  {
    if (PartyManager.Get().IsInBattlegroundsParty())
      return SceneMgr.Mode.BACON;
    return GameUtils.IsAnyTutorialComplete() || !Network.ShouldBeConnectedToAurora() ? SceneMgr.Mode.HUB : SceneMgr.Mode.INVALID;
  }

  public SceneMgr.Mode GetPostGameSceneMode()
  {
    if (this.IsSpectator())
      return this.GetSpectatorPostGameSceneMode();
    SceneMgr.Mode postGameSceneMode = SceneMgr.Mode.HUB;
    bool flag = FiresideGatheringManager.Get().CurrentFiresideGatheringMode != 0;
    switch (this.m_gameType)
    {
      case GameType.GT_VS_AI:
        if (this.m_missionId == 3539)
        {
          PopupDisplayManager.Get().HealUpPopup.QueuePopupAterTutorialIfNotSeen(HealUpPopup.HealUpPopupCompletedTutorial.Battlegrounds);
          BnetPresenceMgr.Get().SetGameField(28U, 1);
          postGameSceneMode = SceneMgr.Mode.BACON;
          break;
        }
        if (this.m_missionId == 3790)
        {
          postGameSceneMode = SceneMgr.Mode.LETTUCE_MAP;
          break;
        }
        TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
        postGameSceneMode = tavernBrawlMission == null || tavernBrawlMission.missionId != this.m_missionId ? SceneMgr.Mode.ADVENTURE : SceneMgr.Mode.TAVERN_BRAWL;
        break;
      case GameType.GT_VS_FRIEND:
      case GameType.GT_FSG_BRAWL_VS_FRIEND:
        if (GameUtils.IsFiresideGatheringGameType(this.m_gameType) && GameUtils.IsTavernBrawlGameType(this.m_gameType))
          flag = true;
        postGameSceneMode = FriendChallengeMgr.Get().HasChallenge() ? (!flag || FiresideGatheringManager.Get().IsCheckedIn ? (!flag || !GameUtils.IsFiresideGatheringGameType(this.m_gameType) ? (!FriendChallengeMgr.Get().IsChallengeTavernBrawl() ? SceneMgr.Mode.FRIENDLY : (!FriendChallengeMgr.Get().IsChallengeFiresideBrawl() ? SceneMgr.Mode.TAVERN_BRAWL : SceneMgr.Mode.HUB)) : SceneMgr.Mode.FIRESIDE_GATHERING) : SceneMgr.Mode.HUB) : (flag ? SceneMgr.Mode.FIRESIDE_GATHERING : SceneMgr.Mode.HUB);
        break;
      case GameType.GT_ARENA:
        postGameSceneMode = SceneMgr.Mode.DRAFT;
        break;
      case GameType.GT_RANKED:
      case GameType.GT_CASUAL:
        postGameSceneMode = SceneMgr.Mode.TOURNAMENT;
        break;
      case GameType.GT_TAVERNBRAWL:
      case GameType.GT_FSG_BRAWL:
      case GameType.GT_FSG_BRAWL_2P_COOP:
        postGameSceneMode = flag ? SceneMgr.Mode.FIRESIDE_GATHERING : SceneMgr.Mode.TAVERN_BRAWL;
        if (TavernBrawlManager.Get().CurrentTavernBrawlSeasonEndInSeconds < 10L && !flag)
        {
          postGameSceneMode = SceneMgr.Mode.HUB;
          break;
        }
        break;
      case GameType.GT_FSG_BRAWL_1P_VS_AI:
        postGameSceneMode = flag ? SceneMgr.Mode.FIRESIDE_GATHERING : SceneMgr.Mode.HUB;
        break;
      case GameType.GT_BATTLEGROUNDS:
      case GameType.GT_BATTLEGROUNDS_FRIENDLY:
      case GameType.GT_BATTLEGROUNDS_AI_VS_AI:
      case GameType.GT_BATTLEGROUNDS_PLAYER_VS_AI:
        postGameSceneMode = SceneMgr.Mode.BACON;
        break;
      case GameType.GT_PVPDR_PAID:
      case GameType.GT_PVPDR:
        postGameSceneMode = SceneMgr.Mode.PVP_DUNGEON_RUN;
        break;
      case GameType.GT_MERCENARIES_PVP:
        postGameSceneMode = SceneMgr.Mode.LETTUCE_PLAY;
        break;
      case GameType.GT_MERCENARIES_PVE:
        postGameSceneMode = SceneMgr.Mode.LETTUCE_MAP;
        break;
      case GameType.GT_MERCENARIES_PVE_COOP:
        postGameSceneMode = SceneMgr.Mode.LETTUCE_MAP;
        break;
      case GameType.GT_MERCENARIES_FRIENDLY:
        postGameSceneMode = SceneMgr.Mode.LETTUCE_FRIENDLY;
        break;
    }
    return postGameSceneMode;
  }

  public SceneMgr.Mode GetPostDisconnectSceneMode()
  {
    if (this.IsSpectator())
      return this.GetSpectatorPostGameSceneMode();
    return this.IsTraditionalTutorial() ? SceneMgr.Mode.INVALID : this.GetPostGameSceneMode();
  }

  public void PreparePostGameSceneMode(SceneMgr.Mode mode)
  {
    if (mode != SceneMgr.Mode.ADVENTURE || AdventureConfig.Get().CurrentSubScene != AdventureData.Adventuresubscene.CHOOSER)
      return;
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(this.m_missionId);
    if (record == null)
      return;
    int adventureId = record.AdventureId;
    if (adventureId == 0)
      return;
    int modeId = record.ModeId;
    if (modeId == 0)
      return;
    AdventureConfig.Get().SetSelectedAdventureMode((AdventureDbId) adventureId, (AdventureModeDbId) modeId);
    AdventureConfig.Get().ChangeSubSceneToSelectedAdventure();
    AdventureConfig.Get().SetMission((ScenarioDbId) this.m_missionId, false);
  }

  public bool IsTransitionPopupShown() => !((UnityEngine.Object) this.m_transitionPopup == (UnityEngine.Object) null) && this.m_transitionPopup.IsShown();

  public TransitionPopup GetTransitionPopup() => this.m_transitionPopup;

  public void UpdatePresence()
  {
    if (!Network.ShouldBeConnectedToAurora() || !Network.IsLoggedIn())
      return;
    if (this.IsSpectator())
    {
      PresenceMgr presenceMgr = PresenceMgr.Get();
      if (this.IsTraditionalTutorial())
        presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_TUTORIAL);
      else if (this.IsBattlegrounds() || this.m_missionId == 3539)
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_BATTLEGROUNDS);
      else if (this.IsPractice())
        presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_PRACTICE);
      else if (this.IsPlay())
      {
        if (this.IsRankedPlay())
        {
          if (this.IsStandardFormatType())
            presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_RANKED_STANDARD);
          else if (this.IsWildFormatType())
            presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_RANKED_WILD);
          else if (this.IsClassicFormatType())
            presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_RANKED_CLASSIC);
          else
            presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY);
        }
        else if (this.IsStandardFormatType())
          presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_CASUAL_STANDARD);
        else if (this.IsWildFormatType())
          presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_CASUAL_WILD);
        else if (this.IsClassicFormatType())
          presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY_CASUAL_CLASSIC);
        else
          presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_PLAY);
      }
      else if (this.IsArena())
        presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_ARENA);
      else if (this.IsFriendly())
        presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_FRIENDLY);
      else if (this.IsTavernBrawl())
        presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_TAVERN_BRAWL);
      else if (this.IsDuels())
        presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_DUELS);
      else if (this.IsMercenaries())
        presenceMgr.SetStatus((Enum) Global.PresenceStatus.SPECTATING_GAME_MERCENARIES);
      else if (this.IsExpansionMission())
      {
        ScenarioDbId missionId = (ScenarioDbId) this.m_missionId;
        presenceMgr.SetStatus_SpectatingMission(missionId);
      }
      SpectatorManager.Get().UpdateMySpectatorInfo();
    }
    else
    {
      if (this.IsTraditionalTutorial())
      {
        switch ((ScenarioDbId) this.m_missionId)
        {
          case ScenarioDbId.TUTORIAL_HOGGER:
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TUTORIAL_GAME, (Enum) PresenceTutorial.HOGGER);
            break;
          case ScenarioDbId.TUTORIAL_MILLHOUSE:
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TUTORIAL_GAME, (Enum) PresenceTutorial.MILLHOUSE);
            break;
          case ScenarioDbId.TUTORIAL_MUKLA:
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TUTORIAL_GAME, (Enum) PresenceTutorial.MUKLA);
            break;
          case ScenarioDbId.TUTORIAL_NESINGWARY:
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TUTORIAL_GAME, (Enum) PresenceTutorial.HEMET);
            break;
          case ScenarioDbId.TUTORIAL_ILLIDAN:
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TUTORIAL_GAME, (Enum) PresenceTutorial.ILLIDAN);
            break;
          case ScenarioDbId.TUTORIAL_CHO:
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TUTORIAL_GAME, (Enum) PresenceTutorial.CHO);
            break;
        }
      }
      else if (this.IsBattlegrounds() || this.m_missionId == 3539)
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.BATTLEGROUNDS_GAME);
      else if (this.IsDuels())
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DUELS_GAME);
      else if (this.IsMercenaries())
      {
        if (this.m_gameType == GameType.GT_MERCENARIES_FRIENDLY)
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.MERCENARIES_FRIENDLY_GAME);
        else
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.MERCENARIES_GAME);
      }
      else if (this.IsPractice())
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PRACTICE_GAME);
      else if (this.IsPlay())
      {
        if (this.IsRankedPlay())
        {
          if (this.IsStandardFormatType())
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PLAY_RANKED_STANDARD);
          else if (this.IsWildFormatType())
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PLAY_RANKED_WILD);
          else if (this.IsClassicFormatType())
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PLAY_RANKED_CLASSIC);
          else
            PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PLAY_GAME);
        }
        else if (this.IsStandardFormatType())
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PLAY_CASUAL_STANDARD);
        else if (this.IsWildFormatType())
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PLAY_CASUAL_WILD);
        else if (this.IsClassicFormatType())
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PLAY_CASUAL_CLASSIC);
        else
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.PLAY_GAME);
      }
      else if (this.IsArena())
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.ARENA_GAME);
      else if (this.IsFriendly())
      {
        Global.PresenceStatus presenceStatus = Global.PresenceStatus.FRIENDLY_GAME;
        if (GameUtils.IsWaitingForOpponentReconnect())
          presenceStatus = Global.PresenceStatus.WAIT_FOR_OPPONENT_RECONNECT;
        else if (FriendChallengeMgr.Get().IsChallengeTavernBrawl())
          presenceStatus = Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_GAME;
        PresenceMgr.Get().SetStatus((Enum) presenceStatus);
      }
      else if (this.IsTavernBrawl())
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TAVERN_BRAWL_GAME);
      else if (this.IsExpansionMission())
      {
        ScenarioDbId missionId = (ScenarioDbId) this.m_missionId;
        PresenceMgr.Get().SetStatus_PlayingMission(missionId);
      }
      SpectatorManager.Get().UpdateMySpectatorInfo();
    }
  }

  public void UpdateSessionPresence(GameType gameType)
  {
    if (gameType == GameType.GT_ARENA)
    {
      int wins = DraftManager.Get().GetWins();
      int losses = DraftManager.Get().GetLosses();
      BnetPresenceMgr.Get().SetGameFieldBlob(22U, (IProtoBuf) new SessionRecord()
      {
        Wins = (uint) wins,
        Losses = (uint) losses,
        RunFinished = false,
        SessionRecordType = SessionRecordType.ARENA
      });
    }
    else
    {
      if (!GameUtils.IsTavernBrawlGameType(gameType) || !TavernBrawlManager.Get().IsCurrentSeasonSessionBased)
        return;
      int gamesWon = TavernBrawlManager.Get().GamesWon;
      int gamesLost = TavernBrawlManager.Get().GamesLost;
      BnetPresenceMgr.Get().SetGameFieldBlob(22U, (IProtoBuf) new SessionRecord()
      {
        Wins = (uint) gamesWon,
        Losses = (uint) gamesLost,
        RunFinished = false,
        SessionRecordType = (TavernBrawlManager.Get().CurrentSeasonBrawlMode == TavernBrawlMode.TB_MODE_NORMAL ? SessionRecordType.TAVERN_BRAWL : SessionRecordType.HEROIC_BRAWL)
      });
    }
  }

  public void SetLastDisplayedPlayerName(int playerId, string name) => this.m_lastDisplayedPlayerNames[playerId] = name;

  public string GetLastDisplayedPlayerName(int playerId)
  {
    string displayedPlayerName;
    this.m_lastDisplayedPlayerNames.TryGetValue(playerId, out displayedPlayerName);
    return displayedPlayerName;
  }

  private void OnSceneUnloaded(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    if (prevMode != SceneMgr.Mode.GAMEPLAY || SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
      return;
    this.OnGameEnded();
  }

  private void OnScenePreLoad(SceneMgr.Mode prevMode, SceneMgr.Mode mode, object userData)
  {
    this.PreloadTransitionPopup();
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.HUB)
      return;
    this.DestroyTransitionPopup();
  }

  private void OnServerResult()
  {
    if (!this.IsFindingGame())
      return;
    ServerResult serverResult = Network.Get().GetServerResult();
    if (serverResult.ResultCode == 1)
    {
      double secondsToWait = (double) Mathf.Max(serverResult.HasRetryDelaySeconds ? serverResult.RetryDelaySeconds : 2f, 0.5f);
      Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.OnServerResult_Retry));
      Processor.ScheduledCallback cb = new Processor.ScheduledCallback(this.OnServerResult_Retry);
      Processor.ScheduleCallback((float) secondsToWait, true, cb);
    }
    else
    {
      if (serverResult.ResultCode != 2)
        return;
      this.OnGameCanceled();
    }
  }

  private void OnServerResult_Retry(object userData) => Network.Get().RetryGotoGameServer();

  private void ChangeBoardIfNecessary()
  {
    int num = this.m_gameSetup.Board;
    if (DemoMgr.Get().IsExpoDemo())
    {
      string str = Vars.Key("Demo.ForceBoard").GetStr((string) null);
      if (str != null)
        num = GameUtils.GetBoardIdFromAssetName(str);
    }
    this.m_gameSetup.Board = num;
  }

  private void PreloadTransitionPopup()
  {
    switch (SceneMgr.Get().GetMode())
    {
      case SceneMgr.Mode.TOURNAMENT:
      case SceneMgr.Mode.DRAFT:
      case SceneMgr.Mode.TAVERN_BRAWL:
        this.LoadTransitionPopup((string) this.MATCHING_POPUP_NAME);
        break;
      case SceneMgr.Mode.FRIENDLY:
      case SceneMgr.Mode.ADVENTURE:
        this.LoadTransitionPopup("LoadingPopup.prefab:ff9266f7c55faa94b9cd0f1371df7168");
        break;
    }
  }

  private string DetermineTransitionPopupForFindGame(GameType gameType, int missionId)
  {
    if (gameType == GameType.GT_TUTORIAL)
      return (string) null;
    return GameUtils.IsMatchmadeGameType(gameType, new int?(missionId)) ? (string) this.MATCHING_POPUP_NAME : "LoadingPopup.prefab:ff9266f7c55faa94b9cd0f1371df7168";
  }

  private void LoadTransitionPopup(string prefabPath)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) prefabPath, AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Error.AddDevFatal("GameMgr.LoadTransitionPopup() - Failed to load {0}", (object) prefabPath);
    }
    else
    {
      if ((UnityEngine.Object) this.m_transitionPopup != (UnityEngine.Object) null)
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_transitionPopup.gameObject);
      this.m_transitionPopup = gameObject.GetComponent<TransitionPopup>();
      this.m_transitionPopup.OnPopupDestroyed += new System.Action(this.OnHandlePopupDestroyed);
      this.m_initialTransitionPopupPos = this.m_transitionPopup.transform.position;
      this.m_transitionPopup.RegisterMatchCanceledEvent(new TransitionPopup.MatchCanceledEvent(this.OnTransitionPopupCanceled));
      LayerUtils.SetLayer((Component) this.m_transitionPopup, GameLayer.IgnoreFullScreenEffects);
    }
  }

  private void OnHandlePopupDestroyed() => this.m_transitionPopup = (TransitionPopup) null;

  private void ShowTransitionPopup(string popupName, int scenarioId)
  {
    System.Type type = this.s_transitionPopupNameToType[popupName];
    if (!(bool) (UnityEngine.Object) this.m_transitionPopup || ((object) this.m_transitionPopup).GetType() != type)
    {
      this.DestroyTransitionPopup();
      this.LoadTransitionPopup(popupName);
    }
    if (this.m_transitionPopup.IsShown())
      return;
    if ((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null && Box.Get().GetState() != Box.State.OPEN)
    {
      Vector3 vector3 = Box.Get().m_Camera.GetCameraPosition(BoxCamera.State.OPENED) - this.m_initialTransitionPopupPos;
      this.m_transitionPopup.transform.position = CameraUtils.GetMainCamera().transform.position - vector3;
    }
    this.m_transitionPopup.SetAdventureId(GameUtils.GetAdventureId(this.m_nextMissionId));
    this.m_transitionPopup.SetFormatType(this.m_nextFormatType);
    this.m_transitionPopup.SetGameType(this.m_nextGameType);
    this.m_transitionPopup.SetDeckId(this.m_lastDeckId);
    this.m_transitionPopup.SetScenarioId(scenarioId);
    this.m_transitionPopup.Show();
    if (this.OnTransitionPopupShown == null)
      return;
    this.OnTransitionPopupShown();
  }

  private void OnTransitionPopupCanceled()
  {
    int num = Network.Get().IsFindingGame() ? 1 : 0;
    if (num != 0)
      Network.Get().CancelFindGame();
    this.ChangeFindGameState(FindGameState.CLIENT_CANCELED);
    if (num != 0)
      return;
    this.ChangeFindGameState(FindGameState.INVALID);
  }

  private void DestroyTransitionPopup()
  {
    if (!(bool) (UnityEngine.Object) this.m_transitionPopup)
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_transitionPopup.gameObject);
    this.m_transitionPopup = (TransitionPopup) null;
  }

  private bool GetFriendlyErrorMessage(
    int errorCode,
    ref string headerKey,
    ref string messageKey,
    ref object[] messageParams)
  {
    switch (errorCode)
    {
      case 1000500:
        headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
        messageKey = "GLOBAL_ERROR_FIND_GAME_SCENARIO_INCORRECT_NUM_PLAYERS";
        return true;
      case 1000501:
        headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
        messageKey = "GLOBAL_ERROR_FIND_GAME_SCENARIO_NO_DECK_SPECIFIED";
        return true;
      case 1000502:
      case 1002008:
        headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
        messageKey = "GLOBAL_ERROR_FIND_GAME_SCENARIO_MISCONFIGURED";
        return true;
      case 1001000:
        headerKey = "GLOBAL_TAVERN_BRAWL";
        messageKey = "GLOBAL_TAVERN_BRAWL_ERROR_SEASON_INCREMENTED";
        TavernBrawlManager.Get().RefreshServerData();
        return true;
      case 1001001:
        headerKey = "GLOBAL_TAVERN_BRAWL";
        messageKey = "GLOBAL_TAVERN_BRAWL_ERROR_NOT_ACTIVE";
        TavernBrawlManager.Get().RefreshServerData();
        return true;
      case 1002002:
        GameType gameType = this.GetGameType();
        if (gameType == GameType.GT_UNKNOWN)
          gameType = this.GetNextGameType();
        if (!GameUtils.IsMatchmadeGameType(gameType))
        {
          headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
          messageKey = "GLUE_ERROR_DECK_RULESET_RULE_VIOLATION";
          return true;
        }
        break;
      case 1002007:
        headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
        messageKey = "GLUE_ERROR_DECK_VALIDATION_WRONG_FORMAT";
        return true;
      case 1003005:
        if (this.m_nextGameType == GameType.GT_ARENA)
        {
          headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
          messageKey = "GLOBAL_ARENA_SEASON_ERROR_NOT_ACTIVE";
          DraftManager.Get().RefreshCurrentSeasonFromServer();
          if (SceneMgr.Get().GetMode() == SceneMgr.Mode.DRAFT)
            Processor.ScheduleCallback(0.0f, false, (Processor.ScheduledCallback) (userData => Navigation.GoBack()));
          return true;
        }
        if (this.m_nextGameType == GameType.GT_PVPDR || this.m_nextGameType == GameType.GT_PVPDR_PAID)
        {
          headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
          messageKey = "GLOBAL_PVPDR_SEASON_ERROR_NOT_ACTIVE";
          if (SceneMgr.Get().GetMode() == SceneMgr.Mode.PVP_DUNGEON_RUN)
            Processor.ScheduleCallback(0.0f, false, (Processor.ScheduledCallback) (userData => Navigation.GoBack()));
          return true;
        }
        break;
      case 1003015:
        headerKey = "GLOBAL_ERROR_GENERIC_HEADER";
        messageKey = "GLUE_ERROR_PLAY_GAME_PARTY_NOT_ALLOWED";
        return true;
    }
    return false;
  }

  private void OnGameQueueEvent(QueueEvent queueEvent)
  {
    FindGameState? nullable = new FindGameState?();
    GameMgr.s_bnetToFindGameResultMap.TryGetValue(queueEvent.EventType, out nullable);
    if (queueEvent.BnetError != 0)
      this.m_lastEnterGameError = (uint) queueEvent.BnetError;
    if (!nullable.HasValue)
      return;
    if (queueEvent.EventType == QueueEvent.Type.QUEUE_DELAY_ERROR)
    {
      if (queueEvent.BnetError == 25017)
        return;
      string headerKey = "";
      string messageKey = (string) null;
      object[] messageParams = new object[0];
      if (this.GetFriendlyErrorMessage(queueEvent.BnetError, ref headerKey, ref messageKey, ref messageParams))
      {
        Error.AddWarningLoc(headerKey, messageKey, messageParams);
        nullable = new FindGameState?(FindGameState.BNET_QUEUE_CANCELED);
        this.HandleGameCanceled();
      }
    }
    if (queueEvent.BnetError != 0)
    {
      string empty = string.Empty;
      if (Enum.IsDefined(typeof (BattleNetErrors), (object) (BattleNetErrors) queueEvent.BnetError))
        empty = ((BattleNetErrors) queueEvent.BnetError).ToString();
      else if (Enum.IsDefined(typeof (PegasusShared.ErrorCode), (object) (PegasusShared.ErrorCode) queueEvent.BnetError))
        empty = ((PegasusShared.ErrorCode) queueEvent.BnetError).ToString();
      string str = string.Format("OnGameQueueEvent error={0} {1}", (object) queueEvent.BnetError, (object) empty);
      if (HearthstoneApplication.IsInternal())
        Error.AddDevWarning(nameof (OnGameQueueEvent), str);
      else
        Log.BattleNet.PrintWarning(str);
    }
    if (queueEvent.EventType == QueueEvent.Type.QUEUE_GAME_STARTED)
    {
      queueEvent.GameServer.Mission = this.m_nextMissionId;
      this.ChangeFindGameState(nullable.Value, queueEvent, queueEvent.GameServer, (Network.GameCancelInfo) null);
    }
    else
      this.ChangeFindGameState(nullable.Value, queueEvent);
  }

  private void OnGameToJoinNotification() => this.ConnectToGame(Network.Get().GetGameToConnectNotification().Info);

  private void OnGameSetup()
  {
    if (SpectatorManager.Get().IsSpectatingOpposingSide() && this.m_gameSetup != null)
      return;
    this.m_gameSetup = Network.Get().GetGameSetupInfo();
    this.ChangeBoardIfNecessary();
    if (this.m_findGameState == FindGameState.INVALID && this.m_gameType == GameType.GT_UNKNOWN)
    {
      Debug.LogError((object) string.Format("GameMgr.OnGameStarting() - Received {0} packet even though we're not looking for a game.", (object) PegasusGame.GameSetup.PacketID.ID));
    }
    else
    {
      this.m_lastGameData.Clear();
      this.m_lastGameData.GameConnectionInfo = this.m_connectionInfoForGameConnectingTo;
      this.m_connectionInfoForGameConnectingTo = (GameConnectionInfo) null;
      this.m_prevGameType = this.m_gameType;
      this.m_gameType = this.m_nextGameType;
      this.m_nextGameType = GameType.GT_UNKNOWN;
      this.m_prevFormatType = this.m_formatType;
      this.m_formatType = this.m_nextFormatType;
      this.m_nextFormatType = PegasusShared.FormatType.FT_UNKNOWN;
      this.m_prevMissionId = this.m_missionId;
      this.m_missionId = this.m_nextMissionId;
      this.m_nextMissionId = 0;
      this.m_brawlLibraryItemId = this.m_nextBrawlLibraryItemId;
      this.m_nextBrawlLibraryItemId = 0;
      this.m_prevReconnectType = this.m_reconnectType;
      this.m_reconnectType = this.m_nextReconnectType;
      this.m_nextReconnectType = ReconnectType.INVALID;
      this.m_prevSpectator = this.m_spectator;
      this.m_spectator = this.m_nextSpectator;
      this.m_nextSpectator = false;
      if (!this.m_spectator)
      {
        HearthstonePerformance hearthstonePerformance = HearthstonePerformance.Get();
        if (hearthstonePerformance != null)
          hearthstonePerformance.StartPerformanceFlow((FlowPerformance.SetupConfig) new FlowPerformanceGame.GameSetupConfig()
          {
            GameType = this.m_gameType,
            BoardId = this.m_gameSetup.Board,
            ScenarioId = this.m_missionId,
            FormatType = this.m_formatType
          });
      }
      this.ChangeFindGameState(FindGameState.SERVER_GAME_STARTED);
    }
  }

  private void OnGameCanceled()
  {
    this.HandleGameCanceled();
    Network network = Network.Get();
    Network.GameCancelInfo gameCancelInfo = network.GetGameCancelInfo();
    network.DisconnectFromGameServer();
    this.ChangeFindGameState(FindGameState.SERVER_GAME_CANCELED, gameCancelInfo);
  }

  public bool OnBnetError(BnetErrorInfo info, object userData)
  {
    if (info.GetFeature() == BnetFeature.Games)
    {
      BattleNetErrors error = info.GetError();
      this.m_lastEnterGameError = (uint) error;
      string str = (string) null;
      bool flag = false;
      FindGameState state = FindGameState.BNET_ERROR;
      if (error == BattleNetErrors.ERROR_GAME_MASTER_INVALID_FACTORY || error == BattleNetErrors.ERROR_GAME_MASTER_NO_GAME_SERVER || error == BattleNetErrors.ERROR_GAME_MASTER_NO_FACTORY)
      {
        str = error.ToString();
        flag = true;
      }
      if (!flag)
      {
        string headerKey = "";
        string messageKey = (string) null;
        object[] messageParams = new object[0];
        ReconnectMgr reconnectMgr = ReconnectMgr.Get();
        if (this.GetFriendlyErrorMessage((int) this.m_lastEnterGameError, ref headerKey, ref messageKey, ref messageParams) && !reconnectMgr.IsReconnecting() && !reconnectMgr.IsRestoringGameStateFromDatabase())
        {
          Error.AddWarningLoc(headerKey, messageKey, messageParams);
          str = ((PegasusShared.ErrorCode) this.m_lastEnterGameError).ToString();
          state = FindGameState.BNET_QUEUE_CANCELED;
          flag = true;
        }
      }
      if (!flag && info.GetFeatureEvent() == BnetFeatureEvent.Games_OnFindGame)
        flag = true;
      if (flag)
      {
        string format = string.Format("GameMgr.OnBnetError() - received error {0} {1}", (object) this.m_lastEnterGameError, (object) str);
        Log.BattleNet.PrintError(format);
        if (!Log.BattleNet.CanPrint(LogTarget.CONSOLE, Blizzard.T5.Logging.LogLevel.Error, false))
          Debug.LogError((object) string.Format("[{0}] {1}", (object) "BattleNet", (object) format));
        this.HandleGameCanceled();
        this.ChangeFindGameState(state);
        return true;
      }
    }
    return false;
  }

  private void HandleGameCanceled()
  {
    this.m_nextGameType = GameType.GT_UNKNOWN;
    this.m_nextFormatType = PegasusShared.FormatType.FT_UNKNOWN;
    this.m_nextMissionId = 0;
    this.m_nextBrawlLibraryItemId = 0;
    this.m_nextReconnectType = ReconnectType.INVALID;
    this.m_nextSpectator = false;
    Network.Get().ClearLastGameServerJoined();
  }

  private bool OnReconnectTimeout(object userData)
  {
    this.HandleGameCanceled();
    this.ChangeFindGameState(FindGameState.CLIENT_CANCELED);
    this.ChangeFindGameState(FindGameState.INVALID);
    return false;
  }

  private void OnFatalError(FatalErrorMessage message, object userData)
  {
    if (!this.IsFindingGame())
      return;
    this.ChangeFindGameState(FindGameState.CLIENT_CANCELED);
    this.ChangeFindGameState(FindGameState.INVALID);
    if (message.m_reason == FatalErrorReason.MOBILE_GAME_SERVER_RPC_ERROR)
      return;
    DialogManager.Get().ShowReconnectHelperDialog();
  }

  private bool ChangeFindGameState(FindGameState state) => this.ChangeFindGameState(state, (QueueEvent) null, (GameServerInfo) null, (Network.GameCancelInfo) null);

  private bool ChangeFindGameState(FindGameState state, QueueEvent queueEvent) => this.ChangeFindGameState(state, queueEvent, (GameServerInfo) null, (Network.GameCancelInfo) null);

  private bool ChangeFindGameState(FindGameState state, GameServerInfo serverInfo) => this.ChangeFindGameState(state, (QueueEvent) null, serverInfo, (Network.GameCancelInfo) null);

  private bool ChangeFindGameState(FindGameState state, Network.GameCancelInfo cancelInfo) => this.ChangeFindGameState(state, (QueueEvent) null, (GameServerInfo) null, cancelInfo);

  private bool ChangeFindGameState(
    FindGameState state,
    QueueEvent queueEvent,
    GameServerInfo serverInfo,
    Network.GameCancelInfo cancelInfo)
  {
    FindGameState findGameState = this.m_findGameState;
    uint lastEnterGameError = this.m_lastEnterGameError;
    this.m_findGameState = state;
    if (serverInfo != null)
      this.m_gameHandleId = (int) serverInfo.GameHandle;
    FindGameEventData eventData = new FindGameEventData();
    eventData.m_state = state;
    eventData.m_gameServer = serverInfo;
    eventData.m_cancelInfo = cancelInfo;
    if (queueEvent != null)
    {
      eventData.m_queueMinSeconds = queueEvent.MinSeconds;
      eventData.m_queueMaxSeconds = queueEvent.MaxSeconds;
    }
    switch (state)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_STARTED:
      case FindGameState.SERVER_GAME_CANCELED:
        Network.Get().RemoveGameServerDisconnectEventListener(new Network.GameServerDisconnectEvent(this.OnGameServerDisconnect));
        break;
    }
    int num = this.FireFindGameEvent(eventData) ? 1 : 0;
    if (num == 0)
      this.DoDefaultFindGameEventBehavior(eventData);
    this.FinalizeState(eventData);
    if (findGameState == state)
      return num != 0;
    Network.Get().OnFindGameStateChanged(findGameState, state, lastEnterGameError);
    return num != 0;
  }

  private bool FireFindGameEvent(FindGameEventData eventData)
  {
    bool gameEvent = false;
    foreach (GameMgr.FindGameListener findGameListener in this.m_findGameListeners.ToArray())
      gameEvent = findGameListener.Fire(eventData) | gameEvent;
    return gameEvent;
  }

  private void DoDefaultFindGameEventBehavior(FindGameEventData eventData)
  {
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
        this.HideTransitionPopup();
        break;
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_ERROR:
        ReconnectMgr reconnectMgr = ReconnectMgr.Get();
        if (!reconnectMgr.IsReconnecting() && !reconnectMgr.IsRestoringGameStateFromDatabase())
          Error.AddWarningLoc("GLOBAL_ERROR_GENERIC_HEADER", "GLOBAL_ERROR_GAME_DENIED");
        this.HideTransitionPopup();
        break;
      case FindGameState.BNET_QUEUE_CANCELED:
        this.HideTransitionPopup();
        break;
      case FindGameState.SERVER_GAME_CONNECTING:
        Network.Get().GotoGameServer(eventData.m_gameServer, this.IsNextReconnect());
        break;
      case FindGameState.SERVER_GAME_STARTED:
        if ((UnityEngine.Object) Box.Get() != (UnityEngine.Object) null)
        {
          LoadingScreen.Get().SetFreezeFrameCamera(Box.Get().GetCamera());
          LoadingScreen.Get().SetTransitionAudioListener(Box.Get().GetAudioListener());
        }
        if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
        {
          if (SpectatorManager.Get().IsSpectatingOpposingSide())
            break;
          Debug.Log((object) "SERVER_GAME_STARTED event - Reloading Gameplay Scene");
          SceneMgr.Get().ReloadMode();
          break;
        }
        Debug.Log((object) "SERVER_GAME_STARTED event - Loading Gameplay Scene");
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAMEPLAY);
        break;
      case FindGameState.SERVER_GAME_CANCELED:
        if (eventData.m_cancelInfo != null)
        {
          switch (eventData.m_cancelInfo.CancelReason)
          {
            case Network.GameCancelInfo.Reason.OPPONENT_TIMEOUT:
            case Network.GameCancelInfo.Reason.PLAYER_LOADING_TIMEOUT:
            case Network.GameCancelInfo.Reason.PLAYER_LOADING_DISCONNECTED:
              Error.AddWarningLoc("GLOBAL_ERROR_GENERIC_HEADER", "GLOBAL_ERROR_GAME_OPPONENT_TIMEOUT");
              break;
            default:
              Error.AddDevWarning("GAME ERROR", "The Game Server canceled the game. Error: {0}", (object) eventData.m_cancelInfo.CancelReason);
              break;
          }
        }
        this.HideTransitionPopup();
        break;
    }
  }

  private void FinalizeState(FindGameEventData eventData)
  {
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_STARTED:
      case FindGameState.SERVER_GAME_CANCELED:
        this.ChangeFindGameState(FindGameState.INVALID);
        break;
    }
  }

  private void OnGameEnded()
  {
    if (!this.m_spectator)
      HearthstonePerformance.Get()?.StopCurrentFlow();
    this.m_prevGameType = this.m_gameType;
    this.m_gameType = GameType.GT_UNKNOWN;
    this.m_prevFormatType = this.m_formatType;
    this.m_formatType = PegasusShared.FormatType.FT_UNKNOWN;
    this.m_prevMissionId = this.m_missionId;
    this.m_missionId = 0;
    this.m_brawlLibraryItemId = 0;
    this.m_prevReconnectType = this.m_reconnectType;
    this.m_reconnectType = ReconnectType.INVALID;
    this.m_prevSpectator = this.m_spectator;
    this.m_spectator = false;
    this.m_lastEnterGameError = 0U;
    this.m_pendingAutoConcede = false;
    this.m_gameSetup = (Network.GameSetup) null;
    this.m_lastDisplayedPlayerNames.Clear();
  }

  public delegate bool FindGameCallback(FindGameEventData eventData, object userData);

  private class FindGameListener : EventListener<GameMgr.FindGameCallback>
  {
    public bool Fire(FindGameEventData eventData) => this.m_callback(eventData, this.m_userData);
  }
}
