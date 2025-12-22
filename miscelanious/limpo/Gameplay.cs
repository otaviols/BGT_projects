using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core.Time;
using Blizzard.T5.MaterialService.Extensions;
using Cysharp.Threading.Tasks;
using Hearthstone;
using PegasusGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Gameplay : PegasusScene
{
  private static Gameplay s_instance;
  private bool m_unloading;
  private BnetErrorInfo m_lastFatalBnetErrorInfo;
  private bool m_handleLastFatalBnetErrorNow;
  private float m_boardProgress;
  private List<NameBanner> m_nameBanners = new List<NameBanner>();
  private NameBanner m_nameBannerGamePlayPhone;
  private int m_numBannersRequested;
  private Actor m_cardDrawStandIn;
  private BoardLayout m_boardLayout;
  private int m_baconFavoriteBoardSkin;
  private bool m_loadingBaconBoard;
  private bool m_criticalAssetsLoaded;
  private Queue<List<Network.PowerHistory>> m_queuedPowerHistory = new Queue<List<Network.PowerHistory>>();
  private float? m_originalTimeScale;
  private Camera m_inputCamera;
  private PrefabInstanceLoadTracker.Context m_prefabContext = new PrefabInstanceLoadTracker.Context();
  private CancellationTokenSource m_taskTokenSource;
  private CancellationTokenSource m_pausePowerTokenSource;
  private CancellationTokenSource m_waitForOpponentTokenSource;
  private CancellationTokenSource m_stateTokenSource;
  private CancellationTokenSource m_lettuceAbilityTokenSource;
  private CheatMgr m_cheatManager;
  private PrefabInstanceLoadTracker m_prefabInstanceLoaderTracker;

  protected override void Awake()
  {
    Log.LoadingScreen.Print("Gameplay.Awake()");
    Debug.LogFormat("Gameplay.Awake() - CurrentMode={0}, PrevMode={1}", (object) SceneMgr.Get().GetMode(), (object) SceneMgr.Get().GetPrevMode());
    base.Awake();
    Gameplay.s_instance = this;
    GameState gameState = GameState.Initialize();
    if (this.ShouldHandleDisconnect())
    {
      Log.LoadingScreen.PrintWarning("Gameplay.Awake() - DISCONNECTED");
      this.HandleDisconnect();
    }
    else
    {
      Network.Get().SetGameServerDisconnectEventListener(new Network.GameServerDisconnectEvent(this.OnDisconnect));
      this.m_cheatManager = CheatMgr.Get();
      this.m_cheatManager.RegisterCategory("gameplay:more");
      this.m_cheatManager.RegisterCheatHandler("saveme", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_saveme));
      if (!HearthstoneApplication.IsPublic())
      {
        this.m_cheatManager.RegisterCheatHandler("entitycount", new CheatMgr.ProcessCheatCallback(GameDebugDisplay.Get().ToggleEntityCount));
        this.m_cheatManager.RegisterCheatHandler("showtag", new CheatMgr.ProcessCheatCallback(GameDebugDisplay.Get().AddTagToDisplay));
        this.m_cheatManager.RegisterCheatHandler("hidetag", new CheatMgr.ProcessCheatCallback(GameDebugDisplay.Get().RemoveTagToDisplay));
        this.m_cheatManager.RegisterCheatHandler("hidetags", new CheatMgr.ProcessCheatCallback(GameDebugDisplay.Get().RemoveAllTags));
        this.m_cheatManager.RegisterCheatHandler("hidezerotags", new CheatMgr.ProcessCheatCallback(GameDebugDisplay.Get().ToggleHideZeroTags));
        this.m_cheatManager.RegisterCheatHandler("aidebug", new CheatMgr.ProcessCheatCallback(AIDebugDisplay.Get().ToggleDebugDisplay));
        this.m_cheatManager.RegisterCheatHandler("ropedebug", new CheatMgr.ProcessCheatCallback(RopeTimerDebugDisplay.Get().EnableDebugDisplay));
        this.m_cheatManager.RegisterCheatAlias("ropedebug", "ropetimerdebug", "timerdebug", "debugrope", "debugropetimer");
        this.m_cheatManager.RegisterCheatHandler("disableropedebug", new CheatMgr.ProcessCheatCallback(RopeTimerDebugDisplay.Get().DisableDebugDisplay));
        this.m_cheatManager.RegisterCheatAlias("disableropedebug", "disableropetimerdebug", "disabletimerdebug", "disabledebugrope", "disabledebugropetimer");
        this.m_cheatManager.RegisterCheatHandler("showbugs", new CheatMgr.ProcessCheatCallback(JiraBugDebugDisplay.Get().EnableDebugDisplay));
        this.m_cheatManager.RegisterCheatHandler("hidebugs", new CheatMgr.ProcessCheatCallback(JiraBugDebugDisplay.Get().DisableDebugDisplay));
        this.m_cheatManager.RegisterCheatHandler("concede", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_concede), "This is what happens when you Prep > Coin.");
        ZombeastDebugManager.Get();
        DrustvarHorrorDebugManager.Get();
        SmartDiscoverDebugManager.Get();
      }
      this.m_cheatManager.DefaultCategory();
      gameState.RegisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
      this.m_prefabInstanceLoaderTracker = PrefabInstanceLoadTracker.Get();
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "InputManager.prefab:909a8d3bcaaf7ea48a770ff400f4db32", new PrefabCallback<GameObject>(this.OnInputManagerLoaded));
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "MulliganManager.prefab:511d1cd9bce694c0a93778f083b47044", new PrefabCallback<GameObject>(this.OnMulliganManagerLoaded));
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "ThinkEmoteController.prefab:2163c9dc60486d74f8249ccf878b1742");
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "Card_Hidden.prefab:1a94649d257bc284ca6e2962f634a8b9", new PrefabCallback<GameObject>(this.OnCardDrawStandinLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "TurnStartManager.prefab:077d03854627944a695a7e86d67153ca", new PrefabCallback<GameObject>(this.OnTurnStartManagerLoaded));
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "TargetReticleManager.prefab:fcbd8bbbf8c5f4c0589fa9c1927bd018", new PrefabCallback<GameObject>(this.OnTargetReticleManagerLoaded));
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "RemoteActionHandler.prefab:69f5fe6e6c4af9e4aa51f7ffc10fb9b3", new PrefabCallback<GameObject>(this.OnRemoteActionHandlerLoaded));
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "ChoiceCardMgr.prefab:c78e5c81bb7cbaa4ca3f09e6dd732675", new PrefabCallback<GameObject>(this.OnChoiceCardMgrLoaded));
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "Actor_Tag_Visual_Table.prefab:7cbaaffc9f20b1e49a08e703944b1e04", new PrefabCallback<GameObject>(this.OnTagVisualConfigurationLoaded));
      LoadingScreen.Get().RegisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFinished));
      this.m_boardProgress = -1f;
      this.ProcessGameSetupPacket();
    }
  }

  private void OnDestroy()
  {
    Log.LoadingScreen?.Print("Gameplay.OnDestroy()");
    this.m_prefabInstanceLoaderTracker?.DestroyContext(this.m_prefabContext);
    if ((UnityEngine.Object) this.m_inputCamera != (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null)
        PegUI.Get().RemoveInputCamera(this.m_inputCamera);
      this.m_inputCamera = (Camera) null;
    }
    this.RestoreOriginalTimeScale();
    double num = (double) TimeScaleMgr.Get().PopTemporarySpeedIncrease();
    if (this.m_cheatManager != null)
    {
      this.m_cheatManager.UnregisterCheatHandler("saveme", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_saveme));
      if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null && !HearthstoneApplication.IsPublic())
      {
        GameDebugDisplay gameDebugDisplay = GameDebugDisplay.Get();
        AIDebugDisplay aiDebugDisplay = AIDebugDisplay.Get();
        RopeTimerDebugDisplay timerDebugDisplay = RopeTimerDebugDisplay.Get();
        JiraBugDebugDisplay jiraBugDebugDisplay = JiraBugDebugDisplay.Get();
        this.m_cheatManager.UnregisterCheatHandler("entitycount", new CheatMgr.ProcessCheatCallback(gameDebugDisplay.ToggleEntityCount));
        this.m_cheatManager.UnregisterCheatHandler("showtag", new CheatMgr.ProcessCheatCallback(gameDebugDisplay.AddTagToDisplay));
        this.m_cheatManager.UnregisterCheatHandler("hidetag", new CheatMgr.ProcessCheatCallback(gameDebugDisplay.RemoveTagToDisplay));
        this.m_cheatManager.UnregisterCheatHandler("hidetags", new CheatMgr.ProcessCheatCallback(gameDebugDisplay.RemoveAllTags));
        this.m_cheatManager.UnregisterCheatHandler("hidezerotags", new CheatMgr.ProcessCheatCallback(gameDebugDisplay.ToggleHideZeroTags));
        this.m_cheatManager.UnregisterCheatHandler("aidebug", new CheatMgr.ProcessCheatCallback(aiDebugDisplay.ToggleDebugDisplay));
        this.m_cheatManager.UnregisterCheatHandler("ropedebug", new CheatMgr.ProcessCheatCallback(timerDebugDisplay.EnableDebugDisplay));
        this.m_cheatManager.UnregisterCheatHandler("disableropedebug", new CheatMgr.ProcessCheatCallback(timerDebugDisplay.DisableDebugDisplay));
        this.m_cheatManager.UnregisterCheatHandler("showbugs", new CheatMgr.ProcessCheatCallback(jiraBugDebugDisplay.EnableDebugDisplay));
        this.m_cheatManager.UnregisterCheatHandler("hidebugs", new CheatMgr.ProcessCheatCallback(jiraBugDebugDisplay.DisableDebugDisplay));
        this.m_cheatManager.UnregisterCheatHandler("concede", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_concede));
      }
    }
    this.ReleaseCancellationTokenSources();
    Gameplay.s_instance = (Gameplay) null;
  }

  private void Start()
  {
    Log.LoadingScreen.Print("Gameplay.Start()");
    this.CheckBattleNetConnection();
    Network network = Network.Get();
    network.AddBnetErrorListener(new Network.BnetErrorCallback(this.OnBnetError));
    network.RegisterNetHandler((object) PegasusGame.PowerHistory.PacketID.ID, new Network.NetHandler(this.OnPowerHistory));
    network.RegisterNetHandler((object) AllOptions.PacketID.ID, new Network.NetHandler(this.OnAllOptions));
    network.RegisterNetHandler((object) PegasusGame.EntityChoices.PacketID.ID, new Network.NetHandler(this.OnEntityChoices));
    network.RegisterNetHandler((object) PegasusGame.EntitiesChosen.PacketID.ID, new Network.NetHandler(this.OnEntitiesChosen));
    network.RegisterNetHandler((object) PegasusGame.UserUI.PacketID.ID, new Network.NetHandler(this.OnUserUI));
    network.RegisterNetHandler((object) NAckOption.PacketID.ID, new Network.NetHandler(this.OnOptionRejected));
    network.RegisterNetHandler((object) PegasusGame.TurnTimer.PacketID.ID, new Network.NetHandler(this.OnTurnTimerUpdate));
    network.RegisterNetHandler((object) SpectatorNotify.PacketID.ID, new Network.NetHandler(this.OnSpectatorNotify));
    network.RegisterNetHandler((object) AIDebugInformation.PacketID.ID, new Network.NetHandler(this.OnAIDebugInformation));
    network.RegisterNetHandler((object) RopeTimerDebugInformation.PacketID.ID, new Network.NetHandler(this.OnRopeTimerDebugInformation));
    network.RegisterNetHandler((object) DebugMessage.PacketID.ID, new Network.NetHandler(this.OnDebugMessage));
    network.RegisterNetHandler((object) ScriptDebugInformation.PacketID.ID, new Network.NetHandler(this.OnScriptDebugInformation));
    network.RegisterNetHandler((object) GameRoundHistory.PacketID.ID, new Network.NetHandler(this.OnGameRoundHistory));
    network.RegisterNetHandler((object) GameRealTimeBattlefieldRaces.PacketID.ID, new Network.NetHandler(this.OnGameRealTimeBattlefieldRaces));
    network.RegisterNetHandler((object) GameGuardianVars.PacketID.ID, new Network.NetHandler(this.OnGameGuardianVars));
    network.RegisterNetHandler((object) ScriptLogMessage.PacketID.ID, new Network.NetHandler(this.OnScriptLogMessage));
    network.RegisterNetHandler((object) UpdateBattlegroundInfo.PacketID.ID, new Network.NetHandler(this.OnBattlegroundInfo));
    network.RegisterNetHandler((object) GetBattlegroundHeroArmorTierList.PacketID.ID, new Network.NetHandler(this.OnBattlegroundArmorTierList));
    if (!HearthstoneApplication.IsPublic() && Cheats.Get().ShouldSkipSendingGetGameState())
      return;
    network.GetGameState();
  }

  private void CheckBattleNetConnection()
  {
    if (Network.IsLoggedIn() || !Network.ShouldBeConnectedToAurora())
      return;
    this.OnBnetError(new BnetErrorInfo(BnetFeature.Bnet, BnetFeatureEvent.Bnet_OnDisconnected, BattleNetErrors.ERROR_RPC_DISCONNECT), (object) null);
  }

  private void Update()
  {
    this.CheckCriticalAssetLoads();
    Network.Get().ProcessNetwork();
    if (this.IsDoneUpdatingGame())
    {
      EndGameScreen endGameScreen = EndGameScreen.Get();
      if ((UnityEngine.Object) endGameScreen != (UnityEngine.Object) null && (endGameScreen.IsPlayingBlockingAnim() || endGameScreen.IsScoreScreenShown()))
        return;
      this.HandleLastFatalBnetError();
      PlayerMigrationManager migrationManager = PlayerMigrationManager.Get();
      if (migrationManager == null || !migrationManager.RestartRequired || migrationManager.IsShowingPlayerMigrationRelogPopup)
        return;
      migrationManager.ShowRestartAlert();
    }
    else
    {
      if (GameMgr.Get().IsFindingGame() || this.m_unloading || SceneMgr.Get().WillTransition() || !this.AreCriticalAssetsLoaded() || GameState.Get() == null)
        return;
      GameState.Get().Update();
    }
  }

  private void OnGUI() => this.LayoutProgressGUI();

  private void LayoutProgressGUI()
  {
    if ((double) this.m_boardProgress < 0.0)
      return;
    Vector2 vector2_1 = new Vector2(150f, 30f);
    Vector2 vector2_2 = new Vector2((float) ((double) Screen.width * 0.5 - (double) vector2_1.x * 0.5), (float) ((double) Screen.height * 0.5 - (double) vector2_1.y * 0.5));
    GUI.Box(new Rect(vector2_2.x, vector2_2.y, vector2_1.x, vector2_1.y), "");
    GUI.Box(new Rect(vector2_2.x, vector2_2.y, this.m_boardProgress * vector2_1.x, vector2_1.y), "");
    GUI.TextField(new Rect(vector2_2.x, vector2_2.y, vector2_1.x, vector2_1.y), string.Format("{0:0}%", (object) (float) ((double) this.m_boardProgress * 100.0)));
  }

  public static Gameplay Get() => Gameplay.s_instance;

  public CancellationToken TaskToken
  {
    get
    {
      if (this.m_taskTokenSource == null)
        this.m_taskTokenSource = new CancellationTokenSource();
      return this.m_taskTokenSource.Token;
    }
  }

  public CancellationToken PausePowerToken
  {
    get
    {
      if (this.m_pausePowerTokenSource == null)
        this.m_pausePowerTokenSource = new CancellationTokenSource();
      return this.m_pausePowerTokenSource.Token;
    }
  }

  public CancellationToken LettuceAbilityToken
  {
    get
    {
      if (this.m_lettuceAbilityTokenSource == null)
        this.m_lettuceAbilityTokenSource = new CancellationTokenSource();
      return this.m_lettuceAbilityTokenSource.Token;
    }
  }

  public CancellationToken WaitForOpponentToken
  {
    get
    {
      if (this.m_waitForOpponentTokenSource == null)
        this.m_waitForOpponentTokenSource = new CancellationTokenSource();
      return this.m_waitForOpponentTokenSource.Token;
    }
  }

  private CancellationToken GameStateToken
  {
    get
    {
      if (this.m_stateTokenSource == null)
        this.m_stateTokenSource = new CancellationTokenSource();
      return this.m_stateTokenSource.Token;
    }
  }

  public void StopIncreaseWaitForOpponentReconnectPeriod()
  {
    this.m_waitForOpponentTokenSource?.Cancel();
    this.m_waitForOpponentTokenSource?.Dispose();
    this.m_waitForOpponentTokenSource = (CancellationTokenSource) null;
  }

  public override void PreUnload()
  {
    this.m_unloading = true;
    if (!((UnityEngine.Object) Board.Get() != (UnityEngine.Object) null) || !((UnityEngine.Object) BoardCameras.Get() != (UnityEngine.Object) null))
      return;
    LoadingScreen.Get().SetFreezeFrameCamera(Camera.main);
    LoadingScreen.Get().SetTransitionAudioListener(BoardCameras.Get().GetAudioListener());
  }

  public override bool IsUnloading() => this.m_unloading;

  public override void Unload()
  {
    Log.LoadingScreen.Print("Gameplay.Unload()");
    int num = this.IsLeavingGameUnfinished() ? 1 : 0;
    GameState.Shutdown();
    Network network = Network.Get();
    if (network != null)
    {
      network.RemoveGameServerDisconnectEventListener(new Network.GameServerDisconnectEvent(this.OnDisconnect));
      network.RemoveBnetErrorListener(new Network.BnetErrorCallback(this.OnBnetError));
      network.RemoveNetHandler((object) PegasusGame.PowerHistory.PacketID.ID, new Network.NetHandler(this.OnPowerHistory));
      network.RemoveNetHandler((object) AllOptions.PacketID.ID, new Network.NetHandler(this.OnAllOptions));
      network.RemoveNetHandler((object) PegasusGame.EntityChoices.PacketID.ID, new Network.NetHandler(this.OnEntityChoices));
      network.RemoveNetHandler((object) PegasusGame.EntitiesChosen.PacketID.ID, new Network.NetHandler(this.OnEntitiesChosen));
      network.RemoveNetHandler((object) PegasusGame.UserUI.PacketID.ID, new Network.NetHandler(this.OnUserUI));
      network.RemoveNetHandler((object) NAckOption.PacketID.ID, new Network.NetHandler(this.OnOptionRejected));
      network.RemoveNetHandler((object) PegasusGame.TurnTimer.PacketID.ID, new Network.NetHandler(this.OnTurnTimerUpdate));
      network.RemoveNetHandler((object) SpectatorNotify.PacketID.ID, new Network.NetHandler(this.OnSpectatorNotify));
      network.RemoveNetHandler((object) AIDebugInformation.PacketID.ID, new Network.NetHandler(this.OnAIDebugInformation));
      network.RemoveNetHandler((object) RopeTimerDebugInformation.PacketID.ID, new Network.NetHandler(this.OnRopeTimerDebugInformation));
      network.RemoveNetHandler((object) DebugMessage.PacketID.ID, new Network.NetHandler(this.OnDebugMessage));
      network.RemoveNetHandler((object) ScriptDebugInformation.PacketID.ID, new Network.NetHandler(this.OnScriptDebugInformation));
      network.RemoveNetHandler((object) GameRoundHistory.PacketID.ID, new Network.NetHandler(this.OnGameRoundHistory));
      network.RemoveNetHandler((object) GameRealTimeBattlefieldRaces.PacketID.ID, new Network.NetHandler(this.OnGameRealTimeBattlefieldRaces));
      network.RemoveNetHandler((object) GameGuardianVars.PacketID.ID, new Network.NetHandler(this.OnGameGuardianVars));
      network.RemoveNetHandler((object) ScriptLogMessage.PacketID.ID, new Network.NetHandler(this.OnScriptLogMessage));
      network.RemoveNetHandler((object) UpdateBattlegroundInfo.PacketID.ID, new Network.NetHandler(this.OnBattlegroundInfo));
      network.RemoveNetHandler((object) GetBattlegroundHeroArmorTierList.PacketID.ID, new Network.NetHandler(this.OnBattlegroundArmorTierList));
    }
    this.m_cheatManager?.UnregisterCheatHandler("saveme", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_saveme));
    if (num != 0)
    {
      if (GameMgr.Get() != null && GameMgr.Get().IsPendingAutoConcede())
      {
        Network.Get()?.AutoConcede();
        GameMgr.Get().SetPendingAutoConcede(false);
      }
      Network.Get()?.DisconnectFromGameServer();
    }
    foreach (NameBanner nameBanner in this.m_nameBanners)
      nameBanner.Unload();
    if ((UnityEngine.Object) this.m_nameBannerGamePlayPhone != (UnityEngine.Object) null)
      this.m_nameBannerGamePlayPhone.Unload();
    if ((UnityEngine.Object) Board.Get() != (UnityEngine.Object) null && Board.Get().AreAllAssetsLoaded())
      this.m_unloading = false;
    else
      Board.Get()?.RegisterAllAssetsLoadedCallback(new Board.AllAssetsLoadedCallback(this.OnBoardAssetsFinishedLoadingDuringGameplayUnload));
  }

  private void OnBoardAssetsFinishedLoadingDuringGameplayUnload() => this.m_unloading = false;

  private void ReleaseCancellationTokenSources()
  {
    this.m_taskTokenSource?.Cancel();
    this.m_taskTokenSource?.Dispose();
    this.m_taskTokenSource = (CancellationTokenSource) null;
    this.m_pausePowerTokenSource?.Cancel();
    this.m_pausePowerTokenSource?.Dispose();
    this.m_pausePowerTokenSource = (CancellationTokenSource) null;
    this.m_waitForOpponentTokenSource?.Cancel();
    this.m_waitForOpponentTokenSource?.Dispose();
    this.m_waitForOpponentTokenSource = (CancellationTokenSource) null;
    this.m_stateTokenSource?.Cancel();
    this.m_stateTokenSource?.Dispose();
    this.m_stateTokenSource = (CancellationTokenSource) null;
    this.m_lettuceAbilityTokenSource?.Cancel();
    this.m_lettuceAbilityTokenSource?.Dispose();
    this.m_lettuceAbilityTokenSource = (CancellationTokenSource) null;
  }

  public void RemoveClassNames()
  {
    foreach (NameBanner nameBanner in this.m_nameBanners)
    {
      nameBanner.FadeOutSubtext();
      nameBanner.PositionNameText(true);
    }
  }

  public void RemoveNameBanners()
  {
    foreach (Component nameBanner in this.m_nameBanners)
      UnityEngine.Object.Destroy((UnityEngine.Object) nameBanner.gameObject);
    this.m_nameBanners.Clear();
  }

  public void AddGamePlayNameBannerPhone()
  {
    if (!((UnityEngine.Object) this.m_nameBannerGamePlayPhone == (UnityEngine.Object) null))
      return;
    this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "NameBannerGamePlay_phone.prefab:947928a8ac849b2408a621c97d3b9fa6", new PrefabCallback<GameObject>(this.OnPlayerBannerLoaded), (object) Player.Side.OPPOSING);
    ++this.m_numBannersRequested;
  }

  public void RemoveGamePlayNameBannerPhone()
  {
    if (!((UnityEngine.Object) this.m_nameBannerGamePlayPhone != (UnityEngine.Object) null))
      return;
    this.m_nameBannerGamePlayPhone.Unload();
  }

  public void UpdateFriendlySideMedalChange(MedalInfoTranslator medalInfo)
  {
    foreach (NameBanner nameBanner in this.m_nameBanners)
    {
      if (nameBanner.GetPlayerSide() == Player.Side.FRIENDLY)
        nameBanner.UpdateMedalChange(medalInfo);
    }
  }

  public void UpdateEnemySideNameBannerName(string newName)
  {
    foreach (NameBanner nameBanner in this.m_nameBanners)
    {
      if (nameBanner.GetPlayerSide() == Player.Side.OPPOSING)
        nameBanner.SetName(newName);
    }
  }

  public Actor GetCardDrawStandIn() => this.m_cardDrawStandIn;

  public NameBanner GetNameBannerForSide(Player.Side wantedSide) => (UnityEngine.Object) this.m_nameBannerGamePlayPhone != (UnityEngine.Object) null && this.m_nameBannerGamePlayPhone.GetPlayerSide() == wantedSide ? this.m_nameBannerGamePlayPhone : this.m_nameBanners.Find((Predicate<NameBanner>) (x => x.GetPlayerSide() == wantedSide));

  public void SetGameStateBusy(bool busy, float delay)
  {
    if ((double) delay <= (double) Mathf.Epsilon)
      GameState.Get().SetBusy(busy);
    else
      this.SetGameStateBusyWithDelay(busy, delay, this.GameStateToken).Forget();
  }

  public void SwapCardBacks()
  {
    int cardBackId1 = GameState.Get().GetOpposingSidePlayer().GetCardBackId();
    int cardBackId2 = GameState.Get().GetFriendlySidePlayer().GetCardBackId();
    GameState.Get().GetOpposingSidePlayer().SetCardBackId(cardBackId2);
    GameState.Get().GetFriendlySidePlayer().SetCardBackId(cardBackId1);
    CardBackManager.Get().SetGameCardBackIDs(cardBackId1, cardBackId2);
  }

  public bool HasBattleNetFatalError() => this.m_lastFatalBnetErrorInfo != null;

  public BoardLayout GetBoardLayout() => this.m_boardLayout;

  private void ProcessGameSetupPacket()
  {
    Network.GameSetup gameSetup = GameMgr.Get().GetGameSetup();
    if (gameSetup == null)
      Debug.LogError((object) ("Game Setup packet was null. Previous Scene=" + (object) SceneMgr.Get().GetPrevMode()));
    this.LoadBoard(gameSetup);
    GameState.Get().OnGameSetup(gameSetup);
  }

  private bool IsHandlingNetworkProblem() => this.ShouldHandleDisconnect() || this.m_handleLastFatalBnetErrorNow;

  private bool ShouldHandleDisconnect(bool onDisconnect = false) => (!Network.Get().IsConnectedToGameServer() || onDisconnect) && !Network.Get().WasGameConceded() && (Network.Get().WasDisconnectRequested() && GameMgr.Get() != null && GameMgr.Get().IsSpectator() && !GameState.Get().IsGameOverNowOrPending() || GameState.Get() == null || !GameState.Get().IsGameOverNowOrPending());

  private void OnDisconnect(BattleNetErrors error)
  {
    if (!this.ShouldHandleDisconnect(true))
      return;
    Network.Get().RemoveGameServerDisconnectEventListener(new Network.GameServerDisconnectEvent(this.OnDisconnect));
    PerformanceAnalytics.Get()?.DisconnectEvent(SceneMgr.Get().GetMode().ToString());
    GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
    if (gameServerJoined != null)
      TracertReporter.ReportTracertInfo(gameServerJoined.Address);
    this.HandleDisconnect();
  }

  private void HandleDisconnect()
  {
    Log.GameMgr.PrintWarning("Gameplay is handling a game disconnect.");
    if (Network.Get().GetLastGameServerJoined() != null && ReconnectMgr.Get().ReconnectToGameFromGameplay() || SpectatorManager.Get().HandleDisconnectFromGameplay())
      return;
    DisconnectMgr.Get().DisconnectFromGameplay();
  }

  private bool IsDoneUpdatingGame() => this.m_handleLastFatalBnetErrorNow || !Network.Get().IsConnectedToGameServer() && (GameState.Get() == null || !GameState.Get().HasPowersToProcess() && GameState.Get().IsGameOver());

  private bool OnBnetError(BnetErrorInfo info, object userData)
  {
    if (Network.Get().OnIgnorableBnetError(info) || this.m_handleLastFatalBnetErrorNow)
      return true;
    this.m_lastFatalBnetErrorInfo = info;
    switch (info.GetError())
    {
      case BattleNetErrors.ERROR_PARENTAL_CONTROL_RESTRICTION:
      case BattleNetErrors.ERROR_SESSION_DUPLICATE:
        this.m_handleLastFatalBnetErrorNow = true;
        break;
    }
    return true;
  }

  private void OnBnetErrorResponse(AlertPopup.Response response, object userData)
  {
    if ((bool) HearthstoneApplication.AllowResetFromFatalError)
      HearthstoneApplication.Get().Reset();
    else
      HearthstoneApplication.Get().Exit();
  }

  private void HandleLastFatalBnetError()
  {
    if (this.m_lastFatalBnetErrorInfo == null)
      return;
    if (this.m_handleLastFatalBnetErrorNow)
    {
      Network.Get().OnFatalBnetError(this.m_lastFatalBnetErrorInfo);
      this.m_handleLastFatalBnetErrorNow = false;
    }
    else
    {
      string key = (bool) HearthstoneApplication.AllowResetFromFatalError ? "GAMEPLAY_DISCONNECT_BODY_RESET" : "GAMEPLAY_DISCONNECT_BODY";
      if (GameMgr.Get().IsSpectator())
        key = (bool) HearthstoneApplication.AllowResetFromFatalError ? "GAMEPLAY_SPECTATOR_DISCONNECT_BODY_RESET" : "GAMEPLAY_SPECTATOR_DISCONNECT_BODY";
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GAMEPLAY_DISCONNECT_HEADER"),
        m_text = GameStrings.Get(key),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_responseCallback = new AlertPopup.ResponseCallback(this.OnBnetErrorResponse)
      });
    }
    this.m_lastFatalBnetErrorInfo = (BnetErrorInfo) null;
  }

  private void OnPowerHistory()
  {
    List<Network.PowerHistory> powerHistory = Network.Get().GetPowerHistory();
    Log.LoadingScreen.Print("Gameplay.OnPowerHistory() - powerList={0}", (object) powerHistory.Count);
    if (this.AreCriticalAssetsLoaded())
      GameState.Get().OnPowerHistory(powerHistory);
    else
      this.m_queuedPowerHistory.Enqueue(powerHistory);
  }

  private void OnAllOptions()
  {
    Network.Options options = Network.Get().GetOptions();
    Log.LoadingScreen.Print("Gameplay.OnAllOptions() - id={0}", (object) options.ID);
    GameState.Get().OnAllOptions(options);
  }

  private void OnEntityChoices()
  {
    Network.EntityChoices entityChoices = Network.Get().GetEntityChoices();
    Log.LoadingScreen.Print("Gameplay.OnEntityChoices() - id={0}", (object) entityChoices.ID);
    GameState.Get().OnEntityChoices(entityChoices);
  }

  private void OnEntitiesChosen()
  {
    Network.EntitiesChosen entitiesChosen = Network.Get().GetEntitiesChosen();
    GameState.Get().OnEntitiesChosen(entitiesChosen);
  }

  private void OnUserUI()
  {
    if (!(bool) (UnityEngine.Object) RemoteActionHandler.Get())
      return;
    RemoteActionHandler.Get().HandleAction(Network.Get().GetUserUI());
  }

  private void OnOptionRejected()
  {
    int nackOption = Network.Get().GetNAckOption();
    GameState.Get().OnOptionRejected(nackOption);
  }

  private void OnTurnTimerUpdate()
  {
    Network.TurnTimerInfo turnTimerInfo = Network.Get().GetTurnTimerInfo();
    GameState.Get().OnTurnTimerUpdate(turnTimerInfo);
  }

  private void OnSpectatorNotify()
  {
    SpectatorNotify spectatorNotify = Network.Get().GetSpectatorNotify();
    GameState.Get().OnSpectatorNotifyEvent(spectatorNotify);
  }

  private void OnAIDebugInformation()
  {
    AIDebugInformation debugInformation = Network.Get().GetAIDebugInformation();
    AIDebugDisplay.Get().OnAIDebugInformation(debugInformation);
  }

  private void OnRopeTimerDebugInformation()
  {
    RopeTimerDebugInformation debugInformation = Network.Get().GetRopeTimerDebugInformation();
    RopeTimerDebugDisplay.Get().OnRopeTimerDebugInformation(debugInformation);
  }

  private void OnDebugMessage()
  {
    DebugMessage debugMessage = Network.Get().GetDebugMessage();
    DebugMessageManager.Get().OnDebugMessage(debugMessage);
  }

  private void OnScriptDebugInformation()
  {
    ScriptDebugInformation debugInformation = Network.Get().GetScriptDebugInformation();
    ScriptDebugDisplay.Get().OnScriptDebugInfo(debugInformation);
  }

  private void OnGameRoundHistory()
  {
    GameRoundHistory gameRoundHistory = Network.Get().GetGameRoundHistory();
    if (!((UnityEngine.Object) PlayerLeaderboardManager.Get() != (UnityEngine.Object) null))
      return;
    PlayerLeaderboardManager.Get().UpdateRoundHistory(gameRoundHistory);
  }

  private void OnGameRealTimeBattlefieldRaces()
  {
    GameRealTimeBattlefieldRaces battlefieldRaces = Network.Get().GetGameRealTimeBattlefieldRaces();
    if ((UnityEngine.Object) PlayerLeaderboardManager.Get() != (UnityEngine.Object) null)
      PlayerLeaderboardManager.Get().UpdatePlayerRaces(battlefieldRaces);
    GameState gameState = GameState.Get();
    if (gameState == null)
      return;
    TAG_RACE[] excludingAmalgam = gameState.GetAvailableRacesInBattlegroundsExcludingAmalgam();
    List<TAG_RACE> racesInBattlegrounds = gameState.GetMissingRacesInBattlegrounds();
    if (!Array.Exists<TAG_RACE>(excludingAmalgam, (Predicate<TAG_RACE>) (race => race == TAG_RACE.INVALID)) && racesInBattlegrounds.Count != 0)
      return;
    racesInBattlegrounds.Clear();
    int index1 = 0;
    int index2 = 0;
    while (index2 < battlefieldRaces.Races.Count)
    {
      int race = battlefieldRaces.Races[index2].Race;
      int count = battlefieldRaces.Races[index2].Count;
      if (race != 0 && race != 26 && Enum.IsDefined(typeof (TAG_RACE), (object) race))
      {
        if (count >= 0)
        {
          if (index1 >= excludingAmalgam.Length)
          {
            Debug.LogError((object) "[OnGameRealTimeBattlefieldRaces] - available race length overflow!");
            continue;
          }
          excludingAmalgam[index1] = (TAG_RACE) race;
          ++index1;
        }
        else
          racesInBattlegrounds.Add((TAG_RACE) race);
      }
      ++index2;
    }
  }

  private void OnGameGuardianVars()
  {
    GameGuardianVars gameGuardianVars = Network.Get().GetGameGuardianVars();
    if (GameState.Get() == null)
      return;
    GameState.Get().UpdateGameGuardianVars(gameGuardianVars);
  }

  private void OnBattlegroundInfo()
  {
    UpdateBattlegroundInfo battlegroundInfo = Network.Get().GetBattlegroundInfo();
    if (GameState.Get() == null)
      return;
    GameState.Get().UpdateBattlegroundInfo(battlegroundInfo);
  }

  private void OnBattlegroundArmorTierList()
  {
    GetBattlegroundHeroArmorTierList heroArmorTierList = Network.Get().GetBattlegroundHeroArmorTierList();
    if (GameState.Get() == null)
      return;
    GameState.Get().UpdateBattlegroundArmorTierList(heroArmorTierList);
  }

  private void OnScriptLogMessage()
  {
    ScriptLogMessage scriptLogMessage = Network.Get().GetScriptLogMessage();
    if (SceneDebugger.Get() == null)
      return;
    SceneDebugger.Get().AddServerScriptLogMessage(scriptLogMessage);
  }

  private bool AreCriticalAssetsLoaded() => this.m_criticalAssetsLoaded;

  private bool CheckCriticalAssetLoads()
  {
    if (this.m_criticalAssetsLoaded)
      return true;
    if ((UnityEngine.Object) Board.Get() == (UnityEngine.Object) null || (UnityEngine.Object) BaconBoard.Get() == (UnityEngine.Object) null && this.m_loadingBaconBoard || (UnityEngine.Object) BoardCameras.Get() == (UnityEngine.Object) null || (UnityEngine.Object) this.GetBoardLayout() == (UnityEngine.Object) null || GameMgr.Get().IsTraditionalTutorial() && (UnityEngine.Object) BoardTutorial.Get() == (UnityEngine.Object) null || (UnityEngine.Object) MulliganManager.Get() == (UnityEngine.Object) null || (UnityEngine.Object) TurnStartManager.Get() == (UnityEngine.Object) null || (UnityEngine.Object) TargetReticleManager.Get() == (UnityEngine.Object) null || GameplayErrorManager.Get() == null || (UnityEngine.Object) EndTurnButton.Get() == (UnityEngine.Object) null || (UnityEngine.Object) BigCard.Get() == (UnityEngine.Object) null || (UnityEngine.Object) CardTypeBanner.Get() == (UnityEngine.Object) null || (UnityEngine.Object) TurnTimer.Get() == (UnityEngine.Object) null || (UnityEngine.Object) CardColorSwitcher.Get() == (UnityEngine.Object) null || (UnityEngine.Object) RemoteActionHandler.Get() == (UnityEngine.Object) null || (UnityEngine.Object) ChoiceCardMgr.Get() == (UnityEngine.Object) null || (UnityEngine.Object) InputManager.Get() == (UnityEngine.Object) null)
      return false;
    this.m_criticalAssetsLoaded = true;
    this.ProcessQueuedPowerHistory();
    return true;
  }

  private void InitCardBacks()
  {
    int friendlyCardBackID = 0;
    Player friendlySidePlayer = GameState.Get()?.GetFriendlySidePlayer();
    if (friendlySidePlayer != null)
      friendlyCardBackID = friendlySidePlayer.GetCardBackId();
    int opponentCardBackID = 0;
    Player opposingSidePlayer = GameState.Get()?.GetOpposingSidePlayer();
    if (opposingSidePlayer != null)
      opponentCardBackID = opposingSidePlayer.GetCardBackId();
    CardBackManager.Get().SetGameCardBackIDs(friendlyCardBackID, opponentCardBackID);
  }

  private void LoadBoard(Network.GameSetup setup)
  {
    BoardDbfRecord record = GameDbf.Board.GetRecord(setup.Board);
    this.m_baconFavoriteBoardSkin = setup.BaconFavoriteBoardSkin;
    if (record == null)
    {
      if (GameMgr.Get().IsBattlegrounds())
      {
        Debug.LogError((object) string.Format("Gameplay.LoadBoard() - FAILED to load board id: \"{0}\" for battelgrounds", (object) setup.Board));
        record = GameDbf.Board.GetRecord((Predicate<BoardDbfRecord>) (r => r.NoteDesc == "Bacon"));
        UIStatus.Get().AddInfo(string.Format("Failed to Load board ID: {0}, defaulting back to Bacon {1}.", (object) setup.Board, (object) record.ID));
      }
      else
      {
        Debug.LogError((object) string.Format("Gameplay.LoadBoard() - FAILED to load board id: \"{0}\"", (object) setup.Board));
        UIStatus.Get().AddInfo(string.Format("Failed to Load board ID: {0}, defaulting back to 1.", (object) setup.Board));
        record = GameDbf.Board.GetRecord(1);
      }
    }
    this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, new AssetReference(record.Prefab), new PrefabCallback<GameObject>(this.OnBoardLoaded));
  }

  private async UniTaskVoid NotifyPlayersOfBoardLoad(CancellationToken token = default (CancellationToken))
  {
    while ((UnityEngine.Object) this.GetBoardLayout() == (UnityEngine.Object) null)
      await UniTask.Yield(PlayerLoopTiming.Initialization, token);
    foreach (Player player in GameState.Get().GetPlayerMap().Values)
      player.OnBoardLoaded();
  }

  private void OnBoardLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_boardProgress = -1f;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnBoardLoaded() - FAILED to load board \"{0}\"", (object) go));
    else if (this.IsHandlingNetworkProblem())
    {
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    }
    else
    {
      go.GetComponent<Board>().SetBoardDbId(GameMgr.Get().GetGameSetup().Board);
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) ((bool) UniversalInputManager.UsePhoneUI ? "BoardCameras_phone.prefab:1e862adebb4fd4fca8b24249d32f8d86" : "BoardCameras.prefab:b4f3a6717904ff34985655c86149f06c"), new PrefabCallback<GameObject>(this.OnBoardCamerasLoaded));
      if (GameMgr.Get().IsTraditionalTutorial())
        this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "BoardTutorial.prefab:08bd830fc30e15e48a4b56bfc3abee15", new PrefabCallback<GameObject>(this.OnBoardTutorialLoaded));
      if ((UnityEngine.Object) BaconBoard.Get() != (UnityEngine.Object) null)
      {
        this.m_loadingBaconBoard = true;
        BaconBoard.Get().RegisterAllAssetsLoadedCallback(new Board.AllAssetsLoadedCallback(this.OnBaconFavoriteBoardLoaded));
        BaconBoard.Get().LoadInitialTavernBoard(this.m_baconFavoriteBoardSkin);
      }
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) BoardLayout.GetBoardLayoutPrefab((Scenario.BoardLayout) GameMgr.Get().GetGameSetup().BoardLayout), new PrefabCallback<GameObject>(this.OnBoardLayoutLoaded));
    }
  }

  private void OnBaconFavoriteBoardLoaded() => this.m_loadingBaconBoard = false;

  private void OnBoardCamerasLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnBoardCamerasLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
    {
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    }
    else
    {
      go.transform.parent = Board.Get().transform;
      this.m_inputCamera = Camera.main;
      PegUI.Get().AddInputCamera(this.m_inputCamera);
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "CardTypeBanner.prefab:3b446c3c5a48357438d8aa969b5c377d", AssetLoadingOptions.IgnorePrefabPosition);
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "BigCard.prefab:c938058e4609a1146b7ce8a115cc82df", AssetLoadingOptions.IgnorePrefabPosition);
    }
  }

  private void OnBoardLayoutLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnBoardLayoutLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
    {
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    }
    else
    {
      this.m_boardLayout = go.GetComponent<BoardLayout>();
      go.transform.parent = Board.Get().transform;
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "EndTurnButton.prefab:313ebd8bcb770a944be3633ad928096b", new PrefabCallback<GameObject>(this.OnEndTurnButtonLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "TurnTimer.prefab:aa1be1e4f5b36ca4aa6a38ac7d0538ce", new PrefabCallback<GameObject>(this.OnTurnTimerLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    }
  }

  private void OnBoardTutorialLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnBoardTutorialLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    else
      go.transform.parent = Board.Get().transform;
  }

  private void OnEndTurnButtonLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnEndTurnButtonLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
    {
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    }
    else
    {
      EndTurnButton component = go.GetComponent<EndTurnButton>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("Gameplay.OnEndTurnButtonLoaded() - ERROR \"{0}\" has no {1} component", (object) this.name, (object) typeof (EndTurnButton)));
      }
      else
      {
        component.transform.position = Board.Get().FindBone("EndTurnButton").position;
        foreach (Renderer componentsInChild in go.GetComponentsInChildren<Renderer>())
        {
          if (!(bool) (UnityEngine.Object) componentsInChild.gameObject.GetComponent<TextMesh>())
            RendererExtension.GetMaterial(componentsInChild).color = Board.Get().m_EndTurnButtonColor;
        }
      }
    }
  }

  private void OnTurnTimerLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnTurnTimerLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
    {
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    }
    else
    {
      TurnTimer component = go.GetComponent<TurnTimer>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        Debug.LogError((object) string.Format("Gameplay.OnTurnTimerLoaded() - ERROR \"{0}\" has no {1} component", (object) this.name, (object) typeof (TurnTimer)));
      else
        component.transform.position = Board.Get().FindBone("TurnTimerBone").position;
    }
  }

  private void OnRemoteActionHandlerLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnRemoteActionHandlerLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    else
      go.transform.parent = this.transform;
  }

  private void OnTagVisualConfigurationLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnTagVisualConfigurationLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    else
      go.transform.parent = this.transform;
  }

  private void OnChoiceCardMgrLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnChoiceCardMgrLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    else
      go.transform.parent = this.transform;
  }

  private void OnInputManagerLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnInputManagerLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    else
      go.transform.parent = this.transform;
  }

  private void OnMulliganManagerLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnMulliganManagerLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    else
      go.transform.parent = this.transform;
  }

  private void OnTurnStartManagerLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnTurnStartManagerLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    else
      go.transform.parent = this.transform;
  }

  private void OnTargetReticleManagerLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnTargetReticleManagerLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
    {
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    }
    else
    {
      go.transform.parent = this.transform;
      TargetReticleManager.Get().PreloadTargetArrows(this.m_prefabContext);
    }
  }

  private void OnPlayerBannerLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    Player.Side side = (Player.Side) callbackData;
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnPlayerBannerLoaded() - FAILED to load \"{0}\" side={1}", (object) assetRef, (object) side.ToString()));
    else if (this.IsHandlingNetworkProblem())
    {
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    }
    else
    {
      NameBanner component = go.GetComponent<NameBanner>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("Gameplay.OnPlayerBannerLoaded() - FAILED to to find NameBanner component on \"{0}\" side={1}", (object) assetRef, (object) side.ToString()));
      }
      else
      {
        this.m_nameBanners.Add(component);
        --this.m_numBannersRequested;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          if (this.name == "NameBannerGamePlay_phone")
          {
            this.m_nameBannerGamePlayPhone = component;
            this.m_nameBannerGamePlayPhone.Initialize(side);
          }
          else
            component.Initialize(side);
        }
        else
        {
          component.Initialize(side);
          if (!string.IsNullOrEmpty(GameState.Get().GetGameEntity().GetAlternatePlayerName()) && component.GetPlayerSide() == Player.Side.FRIENDLY)
            component.UseLongName();
        }
        this.ShowBannersWhenReady(this.GameStateToken).Forget();
      }
    }
  }

  private async UniTaskVoid ShowBannersWhenReady(CancellationToken token)
  {
    if (this.m_numBannersRequested > 0)
      return;
    foreach (NameBanner banner in this.m_nameBanners)
    {
      while (banner.IsWaitingForMedal)
        await UniTask.Yield(PlayerLoopTiming.Update, token);
    }
    foreach (NameBanner nameBanner in this.m_nameBanners)
      nameBanner.Show();
  }

  private void OnCardDrawStandinLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
      Debug.LogError((object) string.Format("Gameplay.OnCardDrawStandinLoaded() - FAILED to load \"{0}\"", (object) assetRef));
    else if (this.IsHandlingNetworkProblem())
    {
      this.m_prefabInstanceLoaderTracker.DestroyContext(this.m_prefabContext);
    }
    else
    {
      this.m_cardDrawStandIn = go.GetComponent<Actor>();
      go.GetComponentInChildren<CardBackDisplay>().SetCardBack(CardBackManager.CardBackSlot.FRIENDLY);
      this.m_cardDrawStandIn.Hide();
    }
  }

  private void OnTransitionFinished(bool cutoff, object userData)
  {
    LoadingScreen.Get().UnregisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFinished));
    if (cutoff || this.IsHandlingNetworkProblem())
      return;
    if (!GameMgr.Get().IsSpectator())
      BnetRecentPlayerMgr.Get().AddRecentPlayer(GameState.Get().GetOpposingPlayer()?.GetBnetPlayer(), BnetRecentPlayerMgr.RecentReason.CURRENT_OPPONENT);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if (GameState.Get() != null && GameState.Get().IsMulliganPhase())
      {
        this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "NameBannerRight_phone.prefab:8712bbdedd6fa4a45b18dc88226d67b3", new PrefabCallback<GameObject>(this.OnPlayerBannerLoaded), (object) Player.Side.FRIENDLY);
        this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "NameBanner_phone.prefab:c919b2370a8d748d38e2cb4708e15398", new PrefabCallback<GameObject>(this.OnPlayerBannerLoaded), (object) Player.Side.OPPOSING);
        this.m_numBannersRequested += 2;
      }
      else
      {
        if (GameMgr.Get() == null || GameMgr.Get().IsTraditionalTutorial())
          return;
        this.AddGamePlayNameBannerPhone();
      }
    }
    else
    {
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "NameBanner.prefab:f579c831653574d4da0437a5fcf0d58f", new PrefabCallback<GameObject>(this.OnPlayerBannerLoaded), (object) Player.Side.FRIENDLY);
      this.m_prefabInstanceLoaderTracker.InstantiatePrefab(this.m_prefabContext, (AssetReference) "NameBanner.prefab:f579c831653574d4da0437a5fcf0d58f", new PrefabCallback<GameObject>(this.OnPlayerBannerLoaded), (object) Player.Side.OPPOSING);
      this.m_numBannersRequested += 2;
    }
  }

  private void ProcessQueuedPowerHistory()
  {
    while (this.m_queuedPowerHistory.Count > 0)
    {
      List<Network.PowerHistory> powerList = this.m_queuedPowerHistory.Dequeue();
      GameState.Get().OnPowerHistory(powerList);
    }
  }

  private bool IsLeavingGameUnfinished() => (GameState.Get() == null || !GameState.Get().IsGameOver()) && !GameMgr.Get().IsReconnect() && !SceneMgr.Get().IsModeRequested(SceneMgr.Mode.FATAL_ERROR);

  private void OnCreateGame(GameState.CreateGamePhase phase, object userData)
  {
    if (phase == GameState.CreateGamePhase.CREATING)
    {
      this.InitCardBacks();
      this.NotifyPlayersOfBoardLoad(this.GameStateToken).Forget();
    }
    else
    {
      if (phase != GameState.CreateGamePhase.CREATED)
        return;
      CardBackManager.Get().UpdateAllCardBacksInSceneWhenReady();
    }
  }

  private async UniTaskVoid SetGameStateBusyWithDelay(
    bool busy,
    float delay,
    CancellationToken token)
  {
    await UniTask.Delay(TimeSpan.FromSeconds((double) delay), cancellationToken: token);
    GameState.Get().SetBusy(busy);
  }

  public void SaveOriginalTimeScale() => this.m_originalTimeScale = new float?(TimeScaleMgr.Get().GetGameTimeScale());

  public void RestoreOriginalTimeScale()
  {
    if (!this.m_originalTimeScale.HasValue)
      return;
    TimeScaleMgr.Get().SetGameTimeScale(this.m_originalTimeScale.Value);
    this.m_originalTimeScale = new float?();
  }

  public Coroutine RegisterCoroutine(IEnumerator routine) => this.StartCoroutine(routine);

  public void UnregisterCoroutine(Coroutine routine) => this.StopCoroutine(routine);

  private bool OnProcessCheat_saveme(string func, string[] args, string rawArgs)
  {
    GameState.Get().DebugNukeServerBlocks();
    return true;
  }

  private bool OnProcessCheat_concede(string func, string[] args, string rawArgs)
  {
    GameState gameState = GameState.Get();
    if (gameState == null)
    {
      UIStatus.Get().AddInfo("No active game found!", 2f);
      return true;
    }
    gameState.Concede();
    return true;
  }
}
