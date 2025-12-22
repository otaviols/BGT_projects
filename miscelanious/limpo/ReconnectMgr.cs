using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Configuration;
using Blizzard.T5.Core;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ReconnectMgr : IService, IHasUpdate
{
  private Map<GameType, string> m_gameTypeNameKeys = new Map<GameType, string>()
  {
    {
      GameType.GT_VS_FRIEND,
      "GLUE_RECONNECT_GAME_TYPE_FRIENDLY"
    },
    {
      GameType.GT_ARENA,
      "GLUE_RECONNECT_GAME_TYPE_ARENA"
    },
    {
      GameType.GT_CASUAL,
      "GLUE_RECONNECT_GAME_TYPE_UNRANKED"
    },
    {
      GameType.GT_RANKED,
      "GLUE_RECONNECT_GAME_TYPE_RANKED"
    },
    {
      GameType.GT_TAVERNBRAWL,
      "GLUE_RECONNECT_GAME_TYPE_TAVERN_BRAWL"
    },
    {
      GameType.GT_FSG_BRAWL_VS_FRIEND,
      "GLUE_RECONNECT_GAME_TYPE_FRIENDLY"
    },
    {
      GameType.GT_FSG_BRAWL,
      "GLUE_RECONNECT_GAME_TYPE_TAVERN_BRAWL"
    },
    {
      GameType.GT_FSG_BRAWL_1P_VS_AI,
      "GLUE_RECONNECT_GAME_TYPE_TAVERN_BRAWL"
    },
    {
      GameType.GT_FSG_BRAWL_2P_COOP,
      "GLUE_RECONNECT_GAME_TYPE_TAVERN_BRAWL"
    }
  };
  private float[] RECONNECT_RATE_SECONDS = new float[4]
  {
    1f,
    2f,
    3f,
    5f
  };
  private AlertPopup m_gameplayReconnectDialog;
  private ReconnectType m_reconnectType;
  private float m_reconnectStartTimestamp;
  private float m_retryStartTimestamp;
  private float m_reconnectTimer;
  private int m_reconnectNumAttempts;
  private bool m_bypassReconnect;
  private ReconnectMgr.SavedStartGameParameters m_savedStartGameParams = new ReconnectMgr.SavedStartGameParameters();
  private List<ReconnectMgr.TimeoutListener> m_timeoutListeners = new List<ReconnectMgr.TimeoutListener>();
  private bool m_allowOfflineActivity;
  private bool m_initializedForOfflineAccess;
  private System.Action m_nextReLoginCallback;
  private IEnumerator m_reconnectCoroutine;
  private NetworkReachabilityManager m_networkReachabilityManager;
  private Stopwatch m_stopwatch = new Stopwatch();
  private string m_disconnectReason = string.Empty;
  private bool m_hasCompletedFirstLogin;
  private Coroutine m_introPopupCoroutine;
  private bool m_hadPause;
  private DateTime m_lastPauseTime;
  private DateTime m_lastUnpauseTime;
  private bool m_suppressUtilReconnect;

  public event System.Action OnReconnectComplete;

  public bool FullResetRequired { get; set; }

  public bool UpdateRequired { get; set; }

  public bool ReconnectBlockedByInactivity { get; set; }

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    ReconnectMgr reconnectMgr = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    reconnectMgr.m_networkReachabilityManager = ServiceManager.Get<NetworkReachabilityManager>();
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(reconnectMgr.OnFindGameEvent));
    Network network = Network.Get();
    network.AddBnetErrorListener(new Network.BnetErrorCallback(reconnectMgr.OnBnetError));
    network.OnDisconnectedFromBattleNet += new System.Action<BattleNetErrors>(reconnectMgr.OnDisconnectedFromBattleNet);
    LoginManager loginManager = serviceLocator.Get<LoginManager>();
    loginManager.OnAchievesLoaded += new System.Action(reconnectMgr.OnAchievesLoaded);
    loginManager.OnLoginCompleted += new System.Action(reconnectMgr.OnLoginComplete);
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    hearthstoneApplication.WillReset += new System.Action(reconnectMgr.WillReset);
    hearthstoneApplication.Paused += new System.Action(reconnectMgr.Paused);
    hearthstoneApplication.Unpaused += new System.Action(reconnectMgr.Unpaused);
    return false;
  }

  public void Update()
  {
    this.CheckGameplayReconnectTimeout();
    this.CheckGameplayReconnectRetry();
    if (!Network.IsLoggedIn() && Network.ShouldBeConnectedToAurora())
    {
      this.UpdateWhileDisconnectedFromBattleNet();
    }
    else
    {
      if (!Network.IsLoggedIn() || !this.m_initializedForOfflineAccess)
        return;
      this.OnBoxReconnectComplete();
    }
  }

  public System.Type[] GetDependencies() => new System.Type[3]
  {
    typeof (LoginManager),
    typeof (GameMgr),
    typeof (NetworkReachabilityManager)
  };

  public void Shutdown()
  {
    Network service1;
    if (ServiceManager.TryGet<Network>(out service1))
    {
      service1.RemoveBnetErrorListener(new Network.BnetErrorCallback(this.OnBnetError));
      service1.OnDisconnectedFromBattleNet -= new System.Action<BattleNetErrors>(this.OnDisconnectedFromBattleNet);
    }
    GameMgr service2;
    if (ServiceManager.TryGet<GameMgr>(out service2))
      service2.UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    LoginManager service3;
    if (ServiceManager.TryGet<LoginManager>(out service3))
      service3.OnAchievesLoaded -= new System.Action(this.OnAchievesLoaded);
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if (!((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null))
      return;
    hearthstoneApplication.WillReset -= new System.Action(this.WillReset);
    hearthstoneApplication.Paused -= new System.Action(this.Paused);
    hearthstoneApplication.Unpaused -= new System.Action(this.Unpaused);
  }

  public static ReconnectMgr Get() => ServiceManager.Get<ReconnectMgr>();

  public bool IsReconnecting() => this.m_reconnectType != 0;

  public bool IsRestoringGameStateFromDatabase() => this.m_savedStartGameParams != null && this.m_savedStartGameParams.LoadGame;

  public bool IsStartingReconnectGame() => GameMgr.Get().IsReconnect() && (SceneMgr.Get().GetNextMode() == SceneMgr.Mode.GAMEPLAY || SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY && !SceneMgr.Get().IsSceneLoaded());

  public static bool IsReconnectAllowed(FatalErrorMessage fatalErrorMessage)
  {
    if (fatalErrorMessage != null)
      Log.Offline.PrintDebug("ReconnectMgr.IsReconnectAllowed() - Checking Fatal Error Reason: {0}", (object) fatalErrorMessage.m_reason);
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject == null)
    {
      Log.Offline.PrintDebug("ReconnectMgr.IsReconnectAllowed() - Unable to retrieve guardian vars.");
      return false;
    }
    if (!netObject.AllowOfflineClientActivity)
    {
      Log.Offline.PrintDebug("ReconnectMgr.IsReconnectAllowed() - Reconnect disabled by guardian var.");
      return false;
    }
    if (fatalErrorMessage == null || FatalErrorMgr.IsReconnectAllowedBasedOnFatalErrorReason(fatalErrorMessage.m_reason))
      return true;
    Log.Offline.PrintDebug("ReconnectMgr.IsReconnectAllowed() - Reconnect not allowed because of Fatal Error Reason. Reason={0}", (object) fatalErrorMessage.m_reason);
    return false;
  }

  public float GetTimeout() => HearthstoneApplication.IsInternal() ? Options.Get().GetFloat(Option.RECONNECT_TIMEOUT) : (float) OptionDataTables.s_defaultsMap[Option.RECONNECT_TIMEOUT];

  public float GetRetryTime() => HearthstoneApplication.IsInternal() ? Options.Get().GetFloat(Option.RECONNECT_RETRY_TIME) : (float) OptionDataTables.s_defaultsMap[Option.RECONNECT_RETRY_TIME];

  public bool AddTimeoutListener(ReconnectMgr.TimeoutCallback callback) => this.AddTimeoutListener(callback, (object) null);

  public bool AddTimeoutListener(ReconnectMgr.TimeoutCallback callback, object userData)
  {
    ReconnectMgr.TimeoutListener timeoutListener = new ReconnectMgr.TimeoutListener();
    timeoutListener.SetCallback(callback);
    timeoutListener.SetUserData(userData);
    if (this.m_timeoutListeners.Contains(timeoutListener))
      return false;
    this.m_timeoutListeners.Add(timeoutListener);
    return true;
  }

  private void HandleDisconnectedGameResult()
  {
    NetCache.ProfileNoticeDisconnectedGame dcGameNotice = this.GetDCGameNotice();
    if (dcGameNotice == null || dcGameNotice.GameResult == ProfileNoticeDisconnectedGameResult.GameResult.GR_PLAYING)
      return;
    this.OnGameResult(dcGameNotice);
  }

  public void StartUtilReconnect()
  {
    if (ReconnectMgr.IsReconnectingToUtil())
      return;
    Network.Get().ResetForNewAuroraConnection();
    LoginManager.Get().BeginLoginProcess();
    LoginManager.Get().OnFullLoginFlowComplete += new System.Action(this.OnReconnectLoginComplete);
    LoginManager.Get().OnAchievesLoaded += new System.Action(this.OnReconnectAchievesLoaded);
  }

  private static bool IsReconnectingToUtil()
  {
    IBattleNet battleNet = BattleNet.Get();
    return ReconnectMgr.IsConnectedOrConnectingState(battleNet != null ? battleNet.BattleNetStatus() : ConnectionState.Disconnected);
  }

  private static bool IsConnectedOrConnectingState(ConnectionState state)
  {
    switch (state)
    {
      case ConnectionState.Disconnected:
      case ConnectionState.Error:
        return false;
      case ConnectionState.Connecting:
      case ConnectionState.WaitForLogon:
      case ConnectionState.Ready:
        return true;
      default:
        throw new ArgumentOutOfRangeException(nameof (state), (object) state, "Unknown Bnet connection state for reconnect");
    }
  }

  public bool ReconnectToGameFromLogin()
  {
    this.HandleDisconnectedGameResult();
    NetCache.NetCacheDisconnectedGame netObject = NetCache.Get().GetNetObject<NetCache.NetCacheDisconnectedGame>();
    if (netObject == null || netObject.ServerInfo == null || HearthstoneApplication.IsInternal() && !Vars.Key("Developer.ReconnectToGameFromLogin").GetBool(true))
      return false;
    this.StartReconnectingToGame(ReconnectType.LOGIN);
    this.ReconnectToGameFromLogin_RequestRequiredData(netObject);
    return true;
  }

  private void ReconnectToGameFromLogin_RequestRequiredData(NetCache.NetCacheDisconnectedGame dcGame)
  {
    switch (dcGame.GameType)
    {
      case GameType.GT_BATTLEGROUNDS:
        Network.Get().RegisterNetHandler((object) BattlegroundsRatingInfoResponse.PacketID.ID, new Network.NetHandler(this.ReconnectToGameFromLogin_OnBaconRatingInfo));
        Network.Get().RequestBaconRatingInfo();
        break;
      case GameType.GT_MERCENARIES_PVP:
      case GameType.GT_MERCENARIES_PVE:
      case GameType.GT_MERCENARIES_PVE_COOP:
      case GameType.GT_MERCENARIES_FRIENDLY:
        CollectionManager collectionManager = CollectionManager.Get();
        if (collectionManager.IsLettuceLoaded())
        {
          this.ReconnectToGameFromLogin_StartGame(dcGame);
          break;
        }
        collectionManager.OnLettuceLoaded += (System.Action) (() => this.ReconnectToGameFromLogin_StartGame(dcGame));
        collectionManager.StartInitialMercenaryLoadIfRequired();
        break;
      default:
        this.ReconnectToGameFromLogin_StartGame(dcGame);
        break;
    }
  }

  private void ReconnectToGameFromLogin_OnBaconRatingInfo()
  {
    Network.Get().RemoveNetHandler((object) BattlegroundsRatingInfoResponse.PacketID.ID, new Network.NetHandler(this.ReconnectToGameFromLogin_OnBaconRatingInfo));
    this.ReconnectToGameFromLogin_StartGame(NetCache.Get().GetNetObject<NetCache.NetCacheDisconnectedGame>());
  }

  private void ReconnectToGameFromLogin_StartGame(NetCache.NetCacheDisconnectedGame dcGame) => this.StartGame(dcGame.GameType, dcGame.FormatType, ReconnectType.LOGIN, dcGame.ServerInfo, dcGame.ServerInfo.Mission, dcGame.LoadGameState);

  public bool ReconnectToGameFromGameplay()
  {
    GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
    if (gameServerJoined == null)
    {
      Log.Offline.PrintError("serverInfo in ReconnectMgr.ReconnectFromGameplay is null and should not be!");
      return false;
    }
    if (!gameServerJoined.Resumable)
      return false;
    this.HideGameplayReconnectDialog();
    GameType gameType = GameMgr.Get().GetGameType();
    FormatType formatType = GameMgr.Get().GetFormatType();
    ReconnectType reconnectType = ReconnectType.GAMEPLAY;
    this.StartReconnectingToGame(reconnectType);
    this.m_reconnectCoroutine = this.WaitForInternetAndReconnect(gameType, formatType, reconnectType, gameServerJoined);
    HearthstoneApplication.Get().StartCoroutine(this.m_reconnectCoroutine);
    return true;
  }

  public void StopReconnectCoroutine()
  {
    if (this.m_reconnectCoroutine == null)
      return;
    HearthstoneApplication.Get().StopCoroutine(this.m_reconnectCoroutine);
    this.m_reconnectCoroutine = (IEnumerator) null;
  }

  private IEnumerator WaitForInternetAndReconnect(
    GameType gameType,
    FormatType formatType,
    ReconnectType reconnectType,
    GameServerInfo serverInfo)
  {
    while (!this.m_networkReachabilityManager.InternetAvailable_Cached)
      yield return (object) new WaitForSeconds(1f);
    this.StartGame(gameType, formatType, reconnectType, serverInfo);
    this.m_reconnectCoroutine = (IEnumerator) null;
  }

  public bool ShowDisconnectedGameResult(NetCache.ProfileNoticeDisconnectedGame dcGame)
  {
    if (!GameUtils.IsMatchmadeGameType(dcGame.GameType))
      return false;
    TimeSpan timeSpan = DateTime.UtcNow - DateTime.FromFileTimeUtc(dcGame.Date);
    Log.Offline.Print("This user disconnected from his or her last game {0} minutes ago.", (object) timeSpan.TotalMinutes);
    if (timeSpan.TotalHours > 24.0)
    {
      Log.Offline.Print("Not showing the Disconnected Game Result because the game was disconnected from {0} hours ago.", (object) timeSpan.TotalHours);
      return false;
    }
    switch (dcGame.GameResult)
    {
      case ProfileNoticeDisconnectedGameResult.GameResult.GR_WINNER:
      case ProfileNoticeDisconnectedGameResult.GameResult.GR_TIE:
        if (dcGame.GameType == GameType.GT_UNKNOWN)
          return false;
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
        info.m_headerText = GameStrings.Get("GLUE_RECONNECT_RESULT_HEADER");
        string key;
        if (dcGame.GameResult == ProfileNoticeDisconnectedGameResult.GameResult.GR_TIE)
        {
          key = "GLUE_RECONNECT_RESULT_TIE";
        }
        else
        {
          switch (dcGame.YourResult)
          {
            case ProfileNoticeDisconnectedGameResult.PlayerResult.PR_WON:
              key = "GLUE_RECONNECT_RESULT_WIN";
              break;
            case ProfileNoticeDisconnectedGameResult.PlayerResult.PR_LOST:
            case ProfileNoticeDisconnectedGameResult.PlayerResult.PR_QUIT:
              key = "GLUE_RECONNECT_RESULT_LOSE";
              break;
            case ProfileNoticeDisconnectedGameResult.PlayerResult.PR_DISCONNECTED:
              key = "GLUE_RECONNECT_RESULT_DISCONNECT";
              break;
            default:
              Log.Offline.PrintError(string.Format("ReconnectMgr.ShowDisconnectedGameResult() - unhandled player result {0}", (object) dcGame.YourResult));
              return false;
          }
        }
        info.m_text = GameStrings.Format(key, (object) this.GetGameTypeName(dcGame.GameType, dcGame.MissionId));
        info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
        info.m_showAlertIcon = true;
        DialogManager.Get().ShowPopup(info);
        return true;
      default:
        Log.Offline.PrintError(string.Format("ReconnectMgr.ShowDisconnectedGameResult() - unhandled game result {0}", (object) dcGame.GameResult));
        return false;
    }
  }

  private string GetGameTypeName(GameType gameType, int missionId)
  {
    if (gameType == GameType.GT_BATTLEGROUNDS)
      return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_BATTLEGROUNDS");
    AdventureDbfRecord recordFromMissionId = GameUtils.GetAdventureRecordFromMissionId(missionId);
    if (recordFromMissionId != null)
    {
      switch (recordFromMissionId.ID)
      {
        case 1:
          return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_TUTORIAL");
        case 2:
          return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_PRACTICE");
        case 3:
          return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_NAXXRAMAS");
        case 4:
          return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_BRM");
        case 7:
          return GameStrings.Get("GLUE_RECONNECT_GAME_TYPE_TAVERN_BRAWL");
        default:
          return (string) recordFromMissionId.Name;
      }
    }
    else
    {
      string key;
      if (this.m_gameTypeNameKeys.TryGetValue(gameType, out key))
        return GameStrings.Get(key);
      Error.AddDevFatal("ReconnectMgr.GetGameTypeName() - no name for mission {0} gameType {1}", (object) missionId, (object) gameType);
      return string.Empty;
    }
  }

  public void SetNextReLoginCallback(System.Action nextCallback) => this.m_nextReLoginCallback = nextCallback;

  private void WillReset()
  {
    this.m_gameplayReconnectDialog = (AlertPopup) null;
    this.FullResetRequired = false;
    this.UpdateRequired = false;
    this.m_initializedForOfflineAccess = false;
    this.m_hasCompletedFirstLogin = false;
    this.ClearReconnectData();
    this.m_timeoutListeners.Clear();
    this.StopIntroPopupCoroutineIfRunning();
  }

  private void Paused()
  {
    this.m_hadPause = true;
    this.m_lastPauseTime = DateTime.Now;
  }

  private void Unpaused() => this.m_lastUnpauseTime = DateTime.Now;

  private void StartReconnectingToGame(ReconnectType reconnectType)
  {
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    this.m_reconnectType = reconnectType;
    this.m_reconnectStartTimestamp = realtimeSinceStartup;
    this.m_retryStartTimestamp = realtimeSinceStartup;
    PerformanceAnalytics.Get()?.ReconnectStart(reconnectType.ToString());
    this.ShowGameplayReconnectingDialog();
  }

  private void CheckGameplayReconnectTimeout()
  {
    if (!this.IsReconnecting() || (double) Time.realtimeSinceStartup - (double) this.m_reconnectStartTimestamp < (double) this.GetTimeout() || Network.Get().IsConnectedToGameServer())
      return;
    this.OnReconnectTimeout();
  }

  private void CheckGameplayReconnectRetry()
  {
    if (!this.m_networkReachabilityManager.InternetAvailable_Cached || !this.IsReconnecting() || Network.Get().IsConnectedToGameServer() || Network.Get().GameServerHasEvents())
      return;
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    if ((double) realtimeSinceStartup - (double) this.m_retryStartTimestamp < (double) this.GetRetryTime())
      return;
    if (this.m_savedStartGameParams.ServerInfo == null)
    {
      Log.Offline.PrintError(string.Format("m_savedStartGameParams.ServerInfo in CheckGameplayReconnectRetry is null and should not be! {0}", (object) this.m_savedStartGameParams.ToString()));
    }
    else
    {
      this.m_retryStartTimestamp = realtimeSinceStartup;
      this.StartGame_Internal();
    }
  }

  private void OnReconnectTimeout()
  {
    this.SetBypassReconnect(true);
    this.ClearReconnectData();
    this.FireTimeoutEvent();
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY && GameMgr.Get().GetGameType() != GameType.GT_UNKNOWN)
      Error.AddFatal(FatalErrorReason.RECONNECT_TIME_OUT, GameMgr.Get().IsAI() && !GameMgr.Get().IsTavernBrawl() ? "GLOBAL_ERROR_NETWORK_ADVENTURE_RECONNECT_TIMEOUT" : "GLOBAL_ERROR_NETWORK_LOST_GAME_CONNECTION");
    else
      this.AttemptToRestoreGameState();
  }

  private void AttemptToRestoreGameState()
  {
    if (this.m_savedStartGameParams.LoadGame)
    {
      GameMgr.Get().SetPendingAutoConcede(false);
      GameMgr.Get().FindGame(this.m_savedStartGameParams.GameType, this.m_savedStartGameParams.FormatType, this.m_savedStartGameParams.ScenarioId, restoreSavedGameState: this.m_savedStartGameParams.LoadGame);
    }
    else
    {
      this.ClearReconnectData();
      this.ChangeGameplayDialogToTimeout();
    }
  }

  private bool OnBnetError(BnetErrorInfo info, object userData)
  {
    if (!this.IsReconnecting() && !this.IsRestoringGameStateFromDatabase())
      return false;
    this.ChangeGameplayDialogToTimeout();
    if (this.m_savedStartGameParams != null)
      this.m_savedStartGameParams.LoadGame = false;
    return true;
  }

  private void OnDisconnectedFromBattleNet(BattleNetErrors error)
  {
    this.m_initializedForOfflineAccess = false;
    this.InitializeForOfflineAccess(new FatalErrorMessage()
    {
      m_reason = FatalErrorReason.UNKNOWN
    }, error.ToString());
  }

  private void OnGameResult(
    NetCache.ProfileNoticeDisconnectedGame dcGameNotice)
  {
    this.ShowDisconnectedGameResult(dcGameNotice);
    this.AckNotice(dcGameNotice);
  }

  public void SetBypassReconnect(bool shouldBypass) => this.m_bypassReconnect = shouldBypass;

  public bool GetBypassReconnect() => this.m_bypassReconnect;

  private void ClearReconnectData()
  {
    this.m_reconnectType = ReconnectType.INVALID;
    this.m_reconnectStartTimestamp = 0.0f;
    this.m_retryStartTimestamp = 0.0f;
  }

  private void InitializeForOfflineAccess(FatalErrorMessage fatalErrorMessage, string reason)
  {
    if (this.m_initializedForOfflineAccess || !ReconnectMgr.IsReconnectAllowed(fatalErrorMessage))
      return;
    Log.Offline.PrintDebug("ReconnectMgr: Initializing for offline box access.");
    if (!this.m_stopwatch.IsRunning)
    {
      this.m_stopwatch.Start();
      this.m_disconnectReason = reason;
      (int num, int secSpentPaused) = this.GetPauseResumeTimesInSeconds();
      TelemetryManager.Client().SendSeamlessReconnectStart(this.m_disconnectReason, num, secSpentPaused);
    }
    this.m_initializedForOfflineAccess = true;
  }

  private (int secSinceResume, int secSpentPaused) GetPauseResumeTimesInSeconds() => !this.m_hadPause ? (0, 0) : ((DateTime.Now - this.m_lastUnpauseTime).Seconds, (this.m_lastUnpauseTime - this.m_lastPauseTime).Seconds);

  private void UpdateWhileDisconnectedFromBattleNet()
  {
    if (!this.m_hasCompletedFirstLogin || this.FullResetRequired || this.ReconnectBlockedByInactivity || !this.m_networkReachabilityManager.InternetAvailable_Cached || BattleNet.Get().BattleNetStatus() != ConnectionState.Disconnected || this.m_suppressUtilReconnect)
      return;
    this.m_reconnectTimer -= Time.deltaTime;
    if ((double) this.m_reconnectTimer > 0.0)
      return;
    this.m_reconnectTimer = this.RECONNECT_RATE_SECONDS[Mathf.Min(this.m_reconnectNumAttempts, this.RECONNECT_RATE_SECONDS.Length - 1)];
    ++this.m_reconnectNumAttempts;
    Log.Offline.PrintDebug(string.Format("Attempting to reconnect (Attempt {0}).", (object) this.m_reconnectNumAttempts));
    this.StartUtilReconnect();
  }

  private void OnLoginComplete()
  {
    Log.Offline.PrintDebug("OnLoginComplete: Stored web token provided to BattleNet successfully, login completed.");
    this.m_reconnectNumAttempts = 0;
    this.m_reconnectTimer = 0.0f;
    this.m_hasCompletedFirstLogin = true;
  }

  private void OnBoxReconnectComplete()
  {
    Log.Offline.PrintDebug("ReconnectMgr: Reconnect Successful!");
    (int num, int secSpentPaused) = this.GetPauseResumeTimesInSeconds();
    TelemetryManager.Client().SendSeamlessReconnectEnd((float) this.m_stopwatch.ElapsedMilliseconds, this.m_disconnectReason, num, secSpentPaused);
    this.m_disconnectReason = string.Empty;
    this.m_stopwatch.Reset();
    this.m_initializedForOfflineAccess = false;
    FatalErrorMgr.Get().ClearAllErrors();
  }

  private void OnReconnectAchievesLoaded()
  {
    LoginManager.Get().OnAchievesLoaded -= new System.Action(this.OnReconnectAchievesLoaded);
    if (this.OnReconnectComplete != null)
      this.OnReconnectComplete();
    if (LoginManager.Get().AttemptToReconnectToGame(new ReconnectMgr.TimeoutCallback(this.OnLoginReconnectToGameTimeout)))
      return;
    this.ShowIntroPopups();
  }

  private bool OnLoginReconnectToGameTimeout(object userData)
  {
    this.ShowIntroPopups();
    return true;
  }

  private void ShowIntroPopups()
  {
    this.StopIntroPopupCoroutineIfRunning();
    this.m_introPopupCoroutine = Processor.RunCoroutine(this.ShowIntroPopupsCoroutine());
  }

  private void StopIntroPopupCoroutineIfRunning()
  {
    if (this.m_introPopupCoroutine == null)
      return;
    Processor.CancelCoroutine(this.m_introPopupCoroutine);
    this.m_introPopupCoroutine = (Coroutine) null;
  }

  private IEnumerator ShowIntroPopupsCoroutine()
  {
    while (DialogManager.Get().ShowingDialog())
      yield return (object) new WaitForEndOfFrame();
    while (CollectionManager.Get() == null || !CollectionManager.Get().IsFullyLoaded())
      yield return (object) new WaitForEndOfFrame();
    JobDefinition jobDefinition = Processor.QueueJob("LoginManager.ShowIntroPopups", LoginManager.Get().ShowIntroPopups());
    Processor.QueueJob("LoginManager.CompleteLoginFlow", LoginManager.Get().CompleteLoginFlow(), (IJobDependency) jobDefinition.CreateDependency());
    this.m_introPopupCoroutine = (Coroutine) null;
  }

  private void OnReconnectLoginComplete()
  {
    LoginManager.Get().OnFullLoginFlowComplete -= new System.Action(this.OnReconnectLoginComplete);
    PopupDisplayManager.Get().ShowAnyOutstandingPopups();
    if (this.m_nextReLoginCallback == null)
      return;
    this.m_nextReLoginCallback();
    this.m_nextReLoginCallback = (System.Action) null;
  }

  private void ShowGameplayReconnectingDialog()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLOBAL_RECONNECT_RECONNECTING_HEADER");
    info.m_text = this.m_reconnectType != ReconnectType.LOGIN ? GameStrings.Get("GLOBAL_RECONNECT_RECONNECTING") : GameStrings.Get("GLOBAL_RECONNECT_RECONNECTING_LOGIN");
    if ((bool) HearthstoneApplication.CanQuitGame)
    {
      info.m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL;
      info.m_cancelText = GameStrings.Get("GLOBAL_RECONNECT_EXIT_BUTTON");
    }
    else
      info.m_responseDisplay = AlertPopup.ResponseDisplay.NONE;
    info.m_showAlertIcon = true;
    info.m_responseCallback = new AlertPopup.ResponseCallback(this.OnGameplayReconnectingDialogResponse);
    DialogManager.Get().ShowPopup(info, new DialogManager.DialogProcessCallback(this.OnGameplayReconnectingDialogProcessed));
  }

  private bool OnGameplayReconnectingDialogProcessed(DialogBase dialog, object userData)
  {
    if (!this.IsReconnecting())
      return false;
    this.m_gameplayReconnectDialog = (AlertPopup) dialog;
    if (this.IsStartingReconnectGame())
      this.ChangeGameplayDialogToReconnected();
    return true;
  }

  private void OnGameplayReconnectingDialogResponse(AlertPopup.Response response, object userData)
  {
    this.m_gameplayReconnectDialog = (AlertPopup) null;
    HearthstoneApplication.Get().Exit();
  }

  private void ChangeGameplayDialogToReconnected()
  {
    if ((UnityEngine.Object) this.m_gameplayReconnectDialog == (UnityEngine.Object) null)
      return;
    this.m_gameplayReconnectDialog.UpdateInfo(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_RECONNECT_RECONNECTED_HEADER"),
      m_text = this.m_reconnectType != ReconnectType.LOGIN ? GameStrings.Get("GLOBAL_RECONNECT_RECONNECTED") : GameStrings.Get("GLOBAL_RECONNECT_RECONNECTED_LOGIN"),
      m_responseDisplay = AlertPopup.ResponseDisplay.NONE,
      m_showAlertIcon = true
    });
    LoadingScreen.Get().RegisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnPreviousSceneDestroyed));
  }

  private void OnPreviousSceneDestroyed(object userData)
  {
    LoadingScreen.Get().UnregisterPreviousSceneDestroyedListener(new LoadingScreen.PreviousSceneDestroyedCallback(this.OnPreviousSceneDestroyed));
    this.HideGameplayReconnectDialog();
  }

  private void ChangeGameplayDialogToTimeout()
  {
    if ((UnityEngine.Object) this.m_gameplayReconnectDialog == (UnityEngine.Object) null)
      return;
    this.m_gameplayReconnectDialog.UpdateInfo(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_RECONNECT_TIMEOUT_HEADER"),
      m_text = GameStrings.Get("GLOBAL_RECONNECT_TIMEOUT"),
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_showAlertIcon = true,
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnTimeoutGameplayDialogResponse)
    });
  }

  private void OnTimeoutGameplayDialogResponse(AlertPopup.Response response, object userData)
  {
    this.m_gameplayReconnectDialog = (AlertPopup) null;
    if (Network.IsLoggedIn())
      return;
    if ((bool) HearthstoneApplication.AllowResetFromFatalError)
      HearthstoneApplication.Get().Reset();
    else
      HearthstoneApplication.Get().Exit();
  }

  public void HideGameplayReconnectDialog()
  {
    if ((UnityEngine.Object) this.m_gameplayReconnectDialog == (UnityEngine.Object) null)
      return;
    this.m_gameplayReconnectDialog.Hide();
    this.m_gameplayReconnectDialog = (AlertPopup) null;
  }

  public void SetSuppressUtilReconnect(bool value) => this.m_suppressUtilReconnect = value;

  private NetCache.ProfileNoticeDisconnectedGame GetDCGameNotice()
  {
    NetCache.NetCacheProfileNotices netObject = NetCache.Get().GetNetObject<NetCache.NetCacheProfileNotices>();
    if (netObject == null || netObject.Notices == null || netObject.Notices.Count == 0)
      return (NetCache.ProfileNoticeDisconnectedGame) null;
    NetCache.ProfileNoticeDisconnectedGame dcGameNotice = (NetCache.ProfileNoticeDisconnectedGame) null;
    List<NetCache.ProfileNoticeDisconnectedGame> disconnectedGameList = new List<NetCache.ProfileNoticeDisconnectedGame>();
    foreach (NetCache.ProfileNotice notice in netObject.Notices)
    {
      if (notice is NetCache.ProfileNoticeDisconnectedGame)
      {
        NetCache.ProfileNoticeDisconnectedGame disconnectedGame = notice as NetCache.ProfileNoticeDisconnectedGame;
        disconnectedGameList.Add(disconnectedGame);
        if (dcGameNotice == null)
          dcGameNotice = disconnectedGame;
        else if (disconnectedGame.NoticeID > dcGameNotice.NoticeID)
          dcGameNotice = disconnectedGame;
      }
    }
    if (dcGameNotice == null)
      return (NetCache.ProfileNoticeDisconnectedGame) null;
    foreach (NetCache.ProfileNoticeDisconnectedGame notice in disconnectedGameList)
    {
      if (notice.NoticeID != dcGameNotice.NoticeID)
        this.AckNotice(notice);
    }
    return dcGameNotice;
  }

  private void AckNotice(NetCache.ProfileNoticeDisconnectedGame notice) => Network.Get().AckNotice(notice.NoticeID);

  private void StartGame(
    GameType gameType,
    FormatType formatType,
    ReconnectType reconnectType,
    GameServerInfo serverInfo,
    int scenarioId = 0,
    bool loadGameState = false)
  {
    this.m_savedStartGameParams.GameType = gameType;
    this.m_savedStartGameParams.FormatType = formatType;
    this.m_savedStartGameParams.ReconnectType = reconnectType;
    this.m_savedStartGameParams.ServerInfo = serverInfo;
    this.m_savedStartGameParams.ScenarioId = scenarioId;
    this.m_savedStartGameParams.LoadGame = loadGameState;
    this.StartGame_Internal();
  }

  private void StartGame_Internal()
  {
    this.StopReconnectCoroutine();
    GameMgr.Get().ReconnectGame(this.m_savedStartGameParams.GameType, this.m_savedStartGameParams.FormatType, this.m_savedStartGameParams.ReconnectType, this.m_savedStartGameParams.ServerInfo);
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    switch (eventData.m_state)
    {
      case FindGameState.SERVER_GAME_STARTED:
        if (this.IsReconnecting() || this.IsRestoringGameStateFromDatabase())
        {
          this.m_timeoutListeners.Clear();
          this.ChangeGameplayDialogToReconnected();
          this.ClearReconnectData();
          break;
        }
        break;
      case FindGameState.SERVER_GAME_CANCELED:
        if (this.IsReconnecting() || this.IsRestoringGameStateFromDatabase())
        {
          this.OnReconnectTimeout();
          return true;
        }
        break;
    }
    return false;
  }

  private void FireTimeoutEvent()
  {
    PerformanceAnalytics.Get()?.ReconnectEnd(false);
    ReconnectMgr.TimeoutListener[] array = this.m_timeoutListeners.ToArray();
    this.m_timeoutListeners.Clear();
    bool flag = false;
    for (int index = 0; index < array.Length; ++index)
      flag = array[index].Fire() | flag;
    if (flag || !Network.IsLoggedIn())
      return;
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
  }

  private void OnAchievesLoaded() => this.m_allowOfflineActivity = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().AllowOfflineClientActivity;

  public delegate bool TimeoutCallback(object userData);

  private class TimeoutListener : EventListener<ReconnectMgr.TimeoutCallback>
  {
    public bool Fire() => this.m_callback(this.m_userData);
  }

  private class SavedStartGameParameters
  {
    public GameType GameType;
    public FormatType FormatType;
    public ReconnectType ReconnectType;
    public GameServerInfo ServerInfo;
    public int ScenarioId;
    public bool LoadGame;

    public override string ToString() => string.Format("GameType: {0}, FormatType: {1}, ReconnectType: {2}, ScenarioId: {3}, LoadGame: {4}", (object) this.GameType, (object) this.FormatType, (object) this.ReconnectType, (object) this.ScenarioId, (object) this.LoadGame);
  }
}
