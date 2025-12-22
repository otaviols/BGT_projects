using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Cysharp.Threading.Tasks;
using PegasusGame;
using SpectatorProto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using UnityEngine;

public class GameState
{
  public const int DEFAULT_SUBOPTION = -1;
  public const int RACE_COUNT_IN_BATTLEGROUNDS_EXCLUDING_AMALGAM = 5;
  private const string INDENT = "    ";
  private const float BLOCK_REPORT_START_SEC = 10f;
  private const float BLOCK_REPORT_INTERVAL_SEC = 3f;
  private static GameState s_instance;
  private static List<GameState.GameStateInitializedListener> s_gameStateInitializedListeners;
  private TAG_RACE[] m_availableRacesInBattlegroundsExcludingAmalgam = new TAG_RACE[5];
  private List<TAG_RACE> m_missingRacesInBattlegrounds = new List<TAG_RACE>();
  private Map<int, Entity> m_entityMap = new Map<int, Entity>();
  private Map<int, Player> m_playerMap = new Map<int, Player>();
  private Map<int, SharedPlayerInfo> m_playerInfoMap = new Map<int, SharedPlayerInfo>();
  private GameEntity m_gameEntity;
  private Queue<Entity> m_removedFromGameEntities = new Queue<Entity>();
  private HashSet<int> m_removedFromGameEntityLog = new HashSet<int>();
  private GameState.CreateGamePhase m_createGamePhase;
  private Network.HistResetGame m_realTimeResetGame;
  private Network.HistTagChange m_realTimeGameOverTagChange;
  private bool m_gameOver;
  private bool m_concedeRequested;
  private bool m_restartRequested;
  private int m_maxSecretZoneSizePerPlayer;
  private int m_maxSecretsPerPlayer;
  private int m_maxQuestsPerPlayer;
  private int m_maxFriendlySlotsPerPlayer;
  private GameState.ResponseMode m_responseMode;
  private Map<int, Network.EntityChoices> m_choicesMap = new Map<int, Network.EntityChoices>();
  private Queue<GameState.QueuedChoice> m_queuedChoices = new Queue<GameState.QueuedChoice>();
  private List<Entity> m_chosenEntities = new List<Entity>();
  private Network.Options m_options;
  private GameState.SelectedOption m_selectedOption = new GameState.SelectedOption();
  private Network.Options m_lastOptions;
  private GameState.SelectedOption m_lastSelectedOption;
  private bool m_coinHasSpawned;
  private Card m_friendlyCardBeingDrawn;
  private Card m_opponentCardBeingDrawn;
  private int m_lastTurnRemindedOfFullHand;
  private bool m_usingFastActorTriggers;
  private List<GameState.CreateGameListener> m_createGameListeners = new List<GameState.CreateGameListener>();
  private List<GameState.OptionsReceivedListener> m_optionsReceivedListeners = new List<GameState.OptionsReceivedListener>();
  private List<GameState.OptionsSentListener> m_optionsSentListeners = new List<GameState.OptionsSentListener>();
  private List<GameState.OptionRejectedListener> m_optionRejectedListeners = new List<GameState.OptionRejectedListener>();
  private List<GameState.EntityChoicesReceivedListener> m_entityChoicesReceivedListeners = new List<GameState.EntityChoicesReceivedListener>();
  private List<GameState.EntitiesChosenReceivedListener> m_entitiesChosenReceivedListeners = new List<GameState.EntitiesChosenReceivedListener>();
  private List<GameState.CurrentPlayerChangedListener> m_currentPlayerChangedListeners = new List<GameState.CurrentPlayerChangedListener>();
  private List<GameState.FriendlyTurnStartedListener> m_friendlyTurnStartedListeners = new List<GameState.FriendlyTurnStartedListener>();
  private List<GameState.TurnChangedListener> m_turnChangedListeners = new List<GameState.TurnChangedListener>();
  private List<GameState.SpectatorNotifyListener> m_spectatorNotifyListeners = new List<GameState.SpectatorNotifyListener>();
  private List<GameState.GameOverListener> m_gameOverListeners = new List<GameState.GameOverListener>();
  private List<GameState.HeroChangedListener> m_heroChangedListeners = new List<GameState.HeroChangedListener>();
  private List<GameState.BusyStateChangedListener> m_busyStateChangedListeners = new List<GameState.BusyStateChangedListener>();
  private List<GameState.CantPlayListener> m_cantPlayListeners = new List<GameState.CantPlayListener>();
  private List<GameState.DamageCapChangedListener> m_damageCapChangedListeners = new List<GameState.DamageCapChangedListener>();
  private List<GameState.DiabloFightPlayerIDChangedListener> m_diabloFightPlayerIDChangedListeners = new List<GameState.DiabloFightPlayerIDChangedListener>();
  private PowerProcessor m_powerProcessor = new PowerProcessor();
  private float m_reconnectIfStuckTimer;
  private float m_lastBlockedReportTimestamp;
  private bool m_busy;
  private bool m_mulliganBusy;
  private List<Spell> m_serverBlockingSpells = new List<Spell>();
  private List<SpellController> m_serverBlockingSpellControllers = new List<SpellController>();
  private List<GameState.TurnTimerUpdateListener> m_turnTimerUpdateListeners = new List<GameState.TurnTimerUpdateListener>();
  private List<GameState.TurnTimerUpdateListener> m_mulliganTimerUpdateListeners = new List<GameState.TurnTimerUpdateListener>();
  private Map<int, TurnTimerUpdate> m_turnTimerUpdates = new Map<int, TurnTimerUpdate>();
  private AlertPopup m_waitForOpponentReconnectPopup;
  private AlertPopup.PopupInfo m_waitForOpponentReconnectPopupInfo;
  private int m_friendlyDrawCounter;
  private int m_opponentDrawCounter;
  private GameStateFrameTimeTracker m_lostFrameTimeTracker = GameState.CreateFrameTimeTracker();
  private GameStateSlushTimeTracker m_lostSlushTimeTracker = GameState.CreateSlushTimeTracker();
  private float m_clientLostTimeCatchUpThreshold;
  private bool m_useSlushTimeCatchUp;
  private bool m_restrictClientLostTimeCatchUpToLowEndDevices;
  private bool m_allowDeferredPowers = true;
  private bool m_allowBatchedPowers = true;
  private bool m_allowDiamondCards = true;
  private bool m_allowSignatureCards = true;
  private bool m_battlegroundAllowBuddies = true;
  private bool m_battlegroundsAllowQuestRewards = true;
  private bool m_mercenariesUseBonesForBigCard = true;
  private string m_battlegroundMinionPool = "";
  private string m_battlegroundDenyList = "";
  private string m_battlegroundHeroArmorTierList = "";
  private bool m_printBattlegroundMinionPoolOnUpdate;
  private bool m_printBattlegroundDenyListOnUpdate;
  private bool m_printBattlegroundHeroArmorTierListUpdate;

  public static GameState Get() => GameState.s_instance;

  public static GameState Initialize()
  {
    if (GameState.s_instance == null)
    {
      GameState.s_instance = new GameState();
      GameState.FireGameStateInitializedEvent();
      GameState.s_instance.m_powerProcessor.AddTaskEventListener(new PowerProcessor.OnTaskEvent(GameState.s_instance.HandleTaskTimeEvent));
    }
    return GameState.s_instance;
  }

  public static void Shutdown()
  {
    if (GameState.s_instance == null)
      return;
    if (SoundManager.Get() != null)
      SoundManager.Get().DestroyAll(Global.SoundCategory.FX);
    GameState.s_instance.GetGameEntity()?.OnDecommissionGame();
    GameState.s_instance.ClearEntityMap();
    GameState.s_instance.HideWaitForOpponentReconnectPopup();
    GameState.s_instance.m_powerProcessor.RemoveTaskEventListener(new PowerProcessor.OnTaskEvent(GameState.s_instance.HandleTaskTimeEvent));
    GameState.s_instance = (GameState) null;
  }

  public void Update()
  {
    this.m_lostFrameTimeTracker.Update();
    this.m_lostSlushTimeTracker.Update();
    if (this.CheckReconnectIfStuck())
      return;
    this.m_powerProcessor.ProcessPowerQueue();
    this.m_lostFrameTimeTracker.AdjustAccruedLostTime(-0.016667f);
  }

  public PowerProcessor GetPowerProcessor() => this.m_powerProcessor;

  public IGameStateTimeTracker GetTimeTracker() => this.m_useSlushTimeCatchUp ? (IGameStateTimeTracker) this.GetSlushTimeTracker() : (IGameStateTimeTracker) this.GetFrameTimeTracker();

  public GameStateSlushTimeTracker GetSlushTimeTracker() => this.m_lostSlushTimeTracker;

  public GameStateFrameTimeTracker GetFrameTimeTracker() => this.m_lostFrameTimeTracker;

  public void HandleTaskTimeEvent(float diff) => this.m_lostSlushTimeTracker.AdjustAccruedLostTime(diff);

  private static GameStateSlushTimeTracker CreateSlushTimeTracker() => new GameStateSlushTimeTracker();

  private static GameStateFrameTimeTracker CreateFrameTimeTracker() => new GameStateFrameTimeTracker(15, 0.033333f);

  public bool AreLostTimeGuardianConditionsMet()
  {
    if ((double) this.m_clientLostTimeCatchUpThreshold <= 0.0)
      return false;
    return !this.m_restrictClientLostTimeCatchUpToLowEndDevices || PlatformSettings.Memory != MemoryCategory.High;
  }

  public bool AllowDeferredPowers() => this.m_allowDeferredPowers;

  public bool AllowBatchedPowers() => this.m_allowBatchedPowers;

  public bool AllowDiamondCards() => this.m_allowDiamondCards;

  public bool AllowSignatureCards() => this.m_allowSignatureCards;

  public bool MercenariesAllowBigCardBones() => this.m_mercenariesUseBonesForBigCard;

  public bool BattlegroundAllowBuddies() => this.m_battlegroundAllowBuddies & (this.m_gameEntity == null || this.m_gameEntity.GetTag(GAME_TAG.BACON_BUDDY_ENABLED) != 0);

  public bool BattlegroundsAllowQuests() => this.m_battlegroundsAllowQuestRewards & (this.m_gameEntity == null || this.m_gameEntity.GetTag(GAME_TAG.BACON_QUESTS_ACTIVE) != 0);

  public bool PrintBattlegroundMinionPoolOnUpdate() => this.m_printBattlegroundMinionPoolOnUpdate;

  public bool PrintBattlegroundDenyListOnUpdate() => this.m_printBattlegroundDenyListOnUpdate;

  public void SetPrintBattlegroundMinionPoolOnUpdate(bool isPrinting) => this.m_printBattlegroundMinionPoolOnUpdate = isPrinting;

  public void SetPrintBattlegroundDenyListOnUpdate(bool isPrinting) => this.m_printBattlegroundDenyListOnUpdate = isPrinting;

  public void SetPrintBattlegroundHeroArmorTierListOnUpdate(bool isPrinting) => this.m_printBattlegroundHeroArmorTierListUpdate = isPrinting;

  public string BattlegroundDenyList() => this.m_battlegroundDenyList;

  public string BattlegroundMinionPool() => this.m_battlegroundMinionPool;

  public string BattlegroundHeroArmorTierList() => this.m_battlegroundHeroArmorTierList;

  public bool HasPowersToProcess() => this.m_powerProcessor.GetCurrentTaskList() != null || this.m_powerProcessor.GetPowerQueue().Count > 0;

  public Entity GetEntity(int id)
  {
    Entity entity;
    this.m_entityMap.TryGetValue(id, out entity);
    return entity;
  }

  public Player GetPlayer(int id)
  {
    Player player;
    this.m_playerMap.TryGetValue(id, out player);
    return player;
  }

  public GameEntity GetGameEntity() => this.m_gameEntity;

  public bool GetBooleanGameOption(GameEntityOption option)
  {
    GameEntityOptions gameOptions = this.m_gameEntity?.GetGameOptions();
    return gameOptions != null && gameOptions.GetBooleanOption(option);
  }

  public string GetStringGameOption(GameEntityOption option) => this.m_gameEntity?.GetGameOptions()?.GetStringOption(option);

  [Conditional("UNITY_EDITOR")]
  public void DebugSetGameEntity(GameEntity gameEntity) => this.m_gameEntity = gameEntity;

  public bool WasGameCreated() => this.m_gameEntity != null;

  public Player GetPlayerBySide(Player.Side playerSide)
  {
    foreach (Player playerBySide in this.m_playerMap.Values)
    {
      if (playerBySide.GetSide() == playerSide)
        return playerBySide;
    }
    return (Player) null;
  }

  public Player GetLocalSidePlayer()
  {
    bool spectatingOrWatching = SpectatorManager.Get().IsSpectatingOrWatching;
    foreach (Player localSidePlayer in this.m_playerMap.Values)
    {
      if (localSidePlayer.IsLocalUser() || spectatingOrWatching && (BnetEntityId) localSidePlayer.GetGameAccountId() == (BnetEntityId) SpectatorManager.Get().GetSpectateeFriendlySide())
        return localSidePlayer;
    }
    return (Player) null;
  }

  public List<Player> GetOpposingBackseatPlayers()
  {
    List<Player> opposingBackseatPlayers = new List<Player>();
    foreach (Player player in this.m_playerMap.Values)
    {
      if (player.GetSide() == Player.Side.OPPOSING && !player.IsTeamLeader())
        opposingBackseatPlayers.Add(player);
    }
    return opposingBackseatPlayers;
  }

  public List<Player> GetOpposingPlayers()
  {
    List<Player> opposingPlayers = new List<Player>();
    foreach (Player player in this.m_playerMap.Values)
    {
      if (player.GetSide() == Player.Side.OPPOSING)
        opposingPlayers.Add(player);
    }
    return opposingPlayers;
  }

  public int GetFriendlySideTeamId()
  {
    Player localSidePlayer = this.GetLocalSidePlayer();
    if (localSidePlayer == null)
      return 0;
    int teamId = localSidePlayer.GetTeamId();
    return teamId <= 0 ? localSidePlayer.GetPlayerId() : teamId;
  }

  public Player GetFriendlySidePlayer()
  {
    foreach (KeyValuePair<int, Player> player in this.m_playerMap)
    {
      Player friendlySidePlayer = player.Value;
      if (friendlySidePlayer.IsFriendlySide() && friendlySidePlayer.IsTeamLeader())
        return friendlySidePlayer;
    }
    return (Player) null;
  }

  public void HideZzzEffects()
  {
    Player friendlySidePlayer = this.GetFriendlySidePlayer();
    if (friendlySidePlayer != null)
    {
      ZonePlay battlefieldZone = friendlySidePlayer.GetBattlefieldZone();
      if ((UnityEngine.Object) battlefieldZone != (UnityEngine.Object) null)
        battlefieldZone.HideCardZzzEffects();
    }
    Player opposingSidePlayer = this.GetOpposingSidePlayer();
    if (opposingSidePlayer == null)
      return;
    ZonePlay battlefieldZone1 = opposingSidePlayer.GetBattlefieldZone();
    if (!((UnityEngine.Object) battlefieldZone1 != (UnityEngine.Object) null))
      return;
    battlefieldZone1.HideCardZzzEffects();
  }

  public void UnhideZzzEffects()
  {
    Player friendlySidePlayer = this.GetFriendlySidePlayer();
    if (friendlySidePlayer != null)
    {
      ZonePlay battlefieldZone = friendlySidePlayer.GetBattlefieldZone();
      if ((UnityEngine.Object) battlefieldZone != (UnityEngine.Object) null)
        battlefieldZone.UnhideCardZzzEffects();
    }
    Player opposingSidePlayer = this.GetOpposingSidePlayer();
    if (opposingSidePlayer == null)
      return;
    ZonePlay battlefieldZone1 = opposingSidePlayer.GetBattlefieldZone();
    if (!((UnityEngine.Object) battlefieldZone1 != (UnityEngine.Object) null))
      return;
    battlefieldZone1.UnhideCardZzzEffects();
  }

  public Player GetOpposingPlayer()
  {
    List<Player> opposingBackseatPlayers = this.GetOpposingBackseatPlayers();
    return opposingBackseatPlayers.Count > 0 ? opposingBackseatPlayers[0] : this.GetOpposingSidePlayer();
  }

  public Player GetOpposingSidePlayer()
  {
    foreach (KeyValuePair<int, Player> player in this.m_playerMap)
    {
      Player opposingSidePlayer = player.Value;
      if (opposingSidePlayer.IsOpposingSide() && opposingSidePlayer.IsTeamLeader())
        return opposingSidePlayer;
    }
    return (Player) null;
  }

  public int GetFriendlyPlayerId()
  {
    Player friendlySidePlayer = this.GetFriendlySidePlayer();
    return friendlySidePlayer == null ? 0 : friendlySidePlayer.GetPlayerId();
  }

  public int GetOpposingPlayerId()
  {
    Player opposingSidePlayer = this.GetOpposingSidePlayer();
    return opposingSidePlayer == null ? 0 : opposingSidePlayer.GetPlayerId();
  }

  public bool IsFriendlySidePlayerTurn()
  {
    Player friendlySidePlayer = this.GetFriendlySidePlayer();
    return friendlySidePlayer != null && friendlySidePlayer.IsCurrentPlayer();
  }

  public bool IsLocalSidePlayerTurn()
  {
    Player localSidePlayer = this.GetLocalSidePlayer();
    return localSidePlayer != null && localSidePlayer.IsCurrentPlayer();
  }

  public Player GetCurrentPlayer()
  {
    foreach (KeyValuePair<int, Player> player in this.m_playerMap)
    {
      Player currentPlayer = player.Value;
      if (currentPlayer.IsCurrentPlayer())
        return currentPlayer;
    }
    return (Player) null;
  }

  public bool IsCurrentPlayerRevealed()
  {
    Player currentPlayer = this.GetCurrentPlayer();
    return currentPlayer != null && currentPlayer.IsRevealed();
  }

  public Player GetFirstOpponentPlayer(Player player)
  {
    foreach (KeyValuePair<int, Player> player1 in this.m_playerMap)
    {
      Player firstOpponentPlayer = player1.Value;
      if (firstOpponentPlayer.GetSide() != player.GetSide())
        return firstOpponentPlayer;
    }
    return (Player) null;
  }

  public int GetNumFriendlyMinionsInPlay(bool includeUntouchables) => this.GetNumMinionsInPlay(this.GetFriendlySidePlayer(), includeUntouchables);

  public int GetNumEnemyMinionsInPlay(bool includeUntouchables) => this.GetNumMinionsInPlay(this.GetOpposingSidePlayer(), includeUntouchables);

  private int GetNumMinionsInPlay(Player player, bool includeUntouchables)
  {
    if (player == null)
      return 0;
    int numMinionsInPlay = 0;
    foreach (Card card in player.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetController() == player && entity.IsMinion() && (includeUntouchables || !entity.HasTag(GAME_TAG.UNTOUCHABLE)))
        ++numMinionsInPlay;
    }
    return numMinionsInPlay;
  }

  public int GetTurn() => this.m_gameEntity != null ? this.m_gameEntity.GetTag(GAME_TAG.TURN) : 0;

  public bool IsTagBlockingInput() => this.m_gameEntity != null && this.m_gameEntity.HasTag(GAME_TAG.BLOCK_ALL_INPUT);

  public bool IsResponsePacketBlocked()
  {
    if (this.IsMulliganManagerIntroActive())
      return true;
    if (this.m_gameEntity.IsMulliganActiveRealTime())
      return false;
    if (this.IsMulliganManagerActive() || !this.IsCurrentPlayerRevealed() && !this.IsLocalSidePlayerTurn() || !this.m_gameEntity.IsCurrentTurnRealTime() || !this.m_gameEntity.IsInputEnabled() || this.IsTurnStartManagerBlockingInput() || this.IsTagBlockingInput())
      return true;
    if (this.IsResetGamePending())
      return false;
    switch (this.m_responseMode)
    {
      case GameState.ResponseMode.NONE:
        return true;
      case GameState.ResponseMode.OPTION:
      case GameState.ResponseMode.SUB_OPTION:
      case GameState.ResponseMode.OPTION_TARGET:
        if (this.m_options == null)
          return true;
        break;
      case GameState.ResponseMode.CHOICE:
        if (this.GetFriendlyEntityChoices() == null)
          return true;
        break;
      default:
        UnityEngine.Debug.LogWarning((object) string.Format("GameState.IsResponsePacketBlocked() - unhandled response mode {0}", (object) this.m_responseMode));
        break;
    }
    return false;
  }

  public TAG_RACE[] GetAvailableRacesInBattlegroundsExcludingAmalgam() => this.m_availableRacesInBattlegroundsExcludingAmalgam;

  public List<TAG_RACE> GetMissingRacesInBattlegrounds() => this.m_missingRacesInBattlegrounds;

  public Map<int, Entity> GetEntityMap() => this.m_entityMap;

  public Map<int, Player> GetPlayerMap() => this.m_playerMap;

  public Map<int, SharedPlayerInfo> GetPlayerInfoMap() => this.m_playerInfoMap;

  public void AddPlayerInfo(SharedPlayerInfo playerInfo)
  {
    int playerId = playerInfo.GetPlayerId();
    if (this.m_playerInfoMap.ContainsKey(playerId))
      UnityEngine.Debug.LogWarning((object) string.Format("GameState.AddPlayerInfo() - playerInfo {0} has already been added", (object) playerInfo));
    else
      this.m_playerInfoMap.Add(playerId, playerInfo);
  }

  public void AddPlayer(Player player)
  {
    this.m_playerMap.Add(player.GetPlayerId(), player);
    this.m_entityMap.Add(player.GetEntityId(), (Entity) player);
  }

  public void RemovePlayer(Player player)
  {
    player.Destroy();
    this.m_playerMap.Remove(player.GetPlayerId());
    this.m_entityMap.Remove(player.GetEntityId());
  }

  public int CountPlayersAlive()
  {
    int num = 0;
    foreach (SharedPlayerInfo sharedPlayerInfo in this.m_playerInfoMap.Values)
    {
      if (sharedPlayerInfo.GetPlayerHero() != null && sharedPlayerInfo.GetPlayerHero().GetRealTimeRemainingHP() > 0)
        ++num;
    }
    return num;
  }

  public void AddEntity(Entity entity) => this.m_entityMap.Add(entity.GetEntityId(), entity);

  public void RemoveEntity(Entity entity)
  {
    if (entity.IsPlayer())
      this.RemovePlayer(entity as Player);
    else if (entity.IsGame())
    {
      this.m_gameEntity.OnDecommissionGame();
      this.m_gameEntity = (GameEntity) null;
    }
    else
    {
      if (entity.IsAttached())
        this.GetEntity(entity.GetAttached())?.RemoveAttachment(entity);
      if (entity.IsHero())
      {
        Player player = this.GetPlayer(entity.GetControllerId());
        if (player != null && player.GetHero() == entity)
          player.SetHero((Entity) null);
      }
      else if (entity.IsHeroPower())
      {
        Player player = this.GetPlayer(entity.GetControllerId());
        if (player != null && player.GetHeroPower() == entity)
          player.SetHeroPower((Entity) null);
      }
      entity.Destroy();
      this.m_entityMap.Remove(entity.GetEntityId());
    }
  }

  public void RemoveQueuedEntitiesFromGame()
  {
    if (this.m_removedFromGameEntities.Count == 0)
      return;
    bool flag;
    do
    {
      Entity entity = this.m_removedFromGameEntities.Peek();
      flag = this.AttemptRemovalOfQueuedEntity(entity);
      if (flag)
      {
        this.m_removedFromGameEntities.Dequeue();
        this.m_removedFromGameEntityLog.Add(entity.GetEntityId());
      }
    }
    while (flag && this.m_removedFromGameEntities.Count > 0);
  }

  public bool EntityRemovedFromGame(int entityId) => this.m_removedFromGameEntityLog.Contains(entityId);

  private bool AttemptRemovalOfQueuedEntity(Entity entity)
  {
    if (this.GetPowerProcessor().EntityHasPendingTasks(entity))
      return false;
    GameState.Get().RemoveEntity(entity);
    return true;
  }

  public int GetMaxSecretZoneSizePerPlayer() => this.m_maxSecretZoneSizePerPlayer;

  public int GetMaxSecretsPerPlayer() => this.m_maxSecretsPerPlayer;

  public int GetMaxQuestsPerPlayer() => this.m_maxQuestsPerPlayer;

  public int GetMaxFriendlySlotsPerPlayer()
  {
    int tag = this.GetGameEntity().GetTag(GAME_TAG.MAX_SLOTS_PER_PLAYER_OVERRIDE);
    if (tag != this.m_maxFriendlySlotsPerPlayer && tag != 0)
      this.m_maxFriendlySlotsPerPlayer = tag;
    return this.m_maxFriendlySlotsPerPlayer;
  }

  public bool IsBusy() => this.m_busy;

  public void SetBusy(bool busy)
  {
    if (this.m_busy == busy)
      return;
    this.m_busy = busy;
    this.FireBusyStateChangedEvent(busy);
  }

  public bool IsMulliganBusy() => this.m_mulliganBusy;

  public void SetMulliganBusy(bool busy) => this.m_mulliganBusy = busy;

  public bool IsMulliganManagerActive() => !((UnityEngine.Object) MulliganManager.Get() == (UnityEngine.Object) null) && MulliganManager.Get().IsMulliganActive();

  public bool IsMulliganManagerIntroActive() => !((UnityEngine.Object) MulliganManager.Get() == (UnityEngine.Object) null) && MulliganManager.Get().IsMulliganIntroActive();

  public bool IsTurnStartManagerActive() => !((UnityEngine.Object) TurnStartManager.Get() == (UnityEngine.Object) null) && TurnStartManager.Get().IsListeningForTurnEvents();

  public bool IsTurnStartManagerBlockingInput() => !((UnityEngine.Object) TurnStartManager.Get() == (UnityEngine.Object) null) && TurnStartManager.Get().IsBlockingInput();

  public bool HasTheCoinBeenSpawned() => this.m_coinHasSpawned;

  public void NotifyOfCoinSpawn() => this.m_coinHasSpawned = true;

  public bool IsActionStep() => this.m_gameEntity != null && this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.STEP) == TAG_STEP.MAIN_ACTION;

  public ACTION_STEP_TYPE GetActionStepType() => (ACTION_STEP_TYPE) this.m_gameEntity.GetTag(GAME_TAG.ACTION_STEP_TYPE);

  public bool IsCombatStep() => this.m_gameEntity != null && this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.STEP) == TAG_STEP.MAIN_COMBAT;

  public bool IsFinalWrapupStep() => this.m_gameEntity != null && this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.STEP) == TAG_STEP.FINAL_WRAPUP;

  public bool IsBeginPhase() => this.m_gameEntity != null && GameUtils.IsBeginPhase(this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.STEP));

  public bool IsPastBeginPhase() => this.m_gameEntity != null && GameUtils.IsPastBeginPhase(this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.STEP));

  public bool IsMainPhase() => this.m_gameEntity != null && GameUtils.IsMainPhase((TAG_STEP) this.m_gameEntity.GetTag(GAME_TAG.STEP));

  public bool IsMulliganPhase() => this.m_gameEntity != null && this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.STEP) == TAG_STEP.BEGIN_MULLIGAN;

  public bool IsMulliganPhasePending()
  {
    if (this.m_gameEntity == null)
      return false;
    if (this.m_gameEntity.GetTag<TAG_STEP>(GAME_TAG.NEXT_STEP) == TAG_STEP.BEGIN_MULLIGAN)
      return true;
    bool foundMulliganStep = false;
    int gameEntityId = this.m_gameEntity.GetEntityId();
    this.m_powerProcessor.ForEachTaskList((System.Action<int, PowerTaskList>) ((queueIndex, taskList) =>
    {
      List<PowerTask> taskList1 = taskList.GetTaskList();
      for (int index = 0; index < taskList1.Count; ++index)
      {
        if (taskList1[index].GetPower() is Network.HistTagChange power2 && power2.Entity == gameEntityId)
        {
          switch ((GAME_TAG) power2.Tag)
          {
            case GAME_TAG.STEP:
            case GAME_TAG.NEXT_STEP:
              if (power2.Value == 4)
              {
                foundMulliganStep = true;
                return;
              }
              continue;
            default:
              continue;
          }
        }
      }
    }));
    return foundMulliganStep;
  }

  public bool IsMulliganPhaseNowOrPending() => this.IsMulliganPhase() || this.IsMulliganPhasePending();

  public bool IsResetGamePending() => this.m_realTimeResetGame != null;

  public GameState.CreateGamePhase GetCreateGamePhase() => this.m_createGamePhase;

  public bool IsGameCreating() => this.m_createGamePhase == GameState.CreateGamePhase.CREATING;

  public bool IsGameCreated() => this.m_createGamePhase == GameState.CreateGamePhase.CREATED;

  public bool IsGameCreatedOrCreating() => this.IsGameCreated() || this.IsGameCreating();

  public bool WasConcedeRequested() => this.m_concedeRequested;

  public void Concede()
  {
    if (this.m_concedeRequested)
      return;
    this.m_concedeRequested = true;
    Network.Get().Concede();
  }

  public bool WasRestartRequested() => this.m_restartRequested;

  public void Restart()
  {
    if (this.m_restartRequested)
      return;
    this.m_restartRequested = true;
    if (this.IsGameOverNowOrPending())
      this.CheckRestartOnRealTimeGameOver();
    else
      this.Concede();
  }

  private void CheckRestartOnRealTimeGameOver()
  {
    if (!this.WasRestartRequested())
      return;
    this.m_gameOver = true;
    this.m_realTimeGameOverTagChange = (Network.HistTagChange) null;
    Network.Get().DisconnectFromGameServer();
    NotificationManager.Get().DestroyAllNotificationsNowWithNoAnim();
    ReconnectMgr.Get().SetBypassReconnect(true);
    GameMgr.Get().RestartGame();
  }

  public bool IsGameOver() => this.m_gameOver;

  public bool IsGameOverPending() => this.m_realTimeGameOverTagChange != null;

  public bool IsGameOverNowOrPending() => this.IsGameOver() || this.IsGameOverPending();

  public Network.HistTagChange GetRealTimeGameOverTagChange() => this.m_realTimeGameOverTagChange;

  public void ShowEnemyTauntCharacters()
  {
    List<Zone> zones = ZoneMgr.Get().GetZones();
    for (int index1 = 0; index1 < zones.Count; ++index1)
    {
      Zone zone = zones[index1];
      if (zone.m_ServerTag == TAG_ZONE.PLAY && zone.m_Side == Player.Side.OPPOSING)
      {
        List<Card> cards = zone.GetCards();
        for (int index2 = 0; index2 < cards.Count; ++index2)
        {
          Card card = cards[index2];
          Entity entity = card.GetEntity();
          if (entity.HasTaunt() && !entity.IsStealthed())
            card.DoTauntNotification();
        }
      }
    }
  }

  public void GetTauntCounts(Player player, out int minionCount, out int heroCount)
  {
    minionCount = 0;
    heroCount = 0;
    List<Zone> zones = ZoneMgr.Get().GetZones();
    for (int index1 = 0; index1 < zones.Count; ++index1)
    {
      Zone zone = zones[index1];
      if (zone.m_ServerTag == TAG_ZONE.PLAY && player == zone.GetController())
      {
        List<Card> cards = zone.GetCards();
        for (int index2 = 0; index2 < cards.Count; ++index2)
        {
          Entity entity = cards[index2].GetEntity();
          if (entity.HasTaunt() && !entity.IsStealthed())
          {
            switch (entity.GetCardType())
            {
              case TAG_CARDTYPE.HERO:
                ++heroCount;
                continue;
              case TAG_CARDTYPE.MINION:
                ++minionCount;
                continue;
              default:
                continue;
            }
          }
        }
      }
    }
  }

  public Card GetFriendlyCardBeingDrawn() => this.m_friendlyCardBeingDrawn;

  public void SetFriendlyCardBeingDrawn(Card card) => this.m_friendlyCardBeingDrawn = card;

  public Card GetOpponentCardBeingDrawn() => this.m_opponentCardBeingDrawn;

  public void SetOpponentCardBeingDrawn(Card card) => this.m_opponentCardBeingDrawn = card;

  public bool IsBeingDrawn(Card card) => (UnityEngine.Object) card == (UnityEngine.Object) this.m_friendlyCardBeingDrawn || (UnityEngine.Object) card == (UnityEngine.Object) this.m_opponentCardBeingDrawn;

  public bool ClearCardBeingDrawn(Card card)
  {
    if ((UnityEngine.Object) card == (UnityEngine.Object) this.m_friendlyCardBeingDrawn)
    {
      this.m_friendlyCardBeingDrawn = (Card) null;
      return true;
    }
    if (!((UnityEngine.Object) card == (UnityEngine.Object) this.m_opponentCardBeingDrawn))
      return false;
    this.m_opponentCardBeingDrawn = (Card) null;
    return true;
  }

  public int GetLastTurnRemindedOfFullHand() => this.m_lastTurnRemindedOfFullHand;

  public void SetLastTurnRemindedOfFullHand(int turn) => this.m_lastTurnRemindedOfFullHand = turn;

  public bool IsUsingFastActorTriggers()
  {
    GameEntity gameEntity = this.GetGameEntity();
    return gameEntity != null && gameEntity.HasTag(GAME_TAG.ALWAYS_USE_FAST_ACTOR_TRIGGERS) || this.m_usingFastActorTriggers;
  }

  public void SetUsingFastActorTriggers(bool enable) => this.m_usingFastActorTriggers = enable;

  public bool HasHandPlays()
  {
    if (this.m_options == null)
      return false;
    foreach (Network.Options.Option option in this.m_options.List)
    {
      if (option.Type == Network.Options.Option.OptionType.POWER)
      {
        Entity entity = this.GetEntity(option.Main.ID);
        if (entity != null)
        {
          Card card = entity.GetCard();
          if (!((UnityEngine.Object) card == (UnityEngine.Object) null) && !((UnityEngine.Object) (card.GetZone() as ZoneHand) == (UnityEngine.Object) null))
            return true;
        }
      }
    }
    return false;
  }

  public bool CanShowScoreScreen() => this.HasScoreLabels((Entity) this.m_gameEntity) || this.HasScoreLabels((Entity) this.GetFriendlySidePlayer());

  private bool HasScoreLabels(Entity entity) => entity.HasTag(GAME_TAG.SCORE_LABELID_1) || entity.HasTag(GAME_TAG.SCORE_LABELID_2) || entity.HasTag(GAME_TAG.SCORE_LABELID_3) || entity.HasTag(GAME_TAG.SCORE_FOOTERID);

  public int GetFriendlyCardDrawCounter() => this.m_friendlyDrawCounter;

  public void IncrementFriendlyCardDrawCounter() => ++this.m_friendlyDrawCounter;

  public void ResetFriendlyCardDrawCounter() => this.m_friendlyDrawCounter = 0;

  public int GetOpponentCardDrawCounter() => this.m_opponentDrawCounter;

  public void IncrementOpponentCardDrawCounter() => ++this.m_opponentDrawCounter;

  public void ResetOpponentCardDrawCounter() => this.m_opponentDrawCounter = 0;

  private void PreprocessRealTimeTagChange(Entity entity, Network.HistTagChange change)
  {
    switch ((GAME_TAG) change.Tag)
    {
      case GAME_TAG.PLAYSTATE:
        if (!GameUtils.IsGameOverTag(change.Entity, change.Tag, change.Value))
          break;
        this.OnRealTimeGameOver(change);
        break;
      case GAME_TAG.CANT_PLAY:
        if (change.Value <= 0)
          break;
        this.OnCantPlay(entity);
        break;
      case GAME_TAG.WAIT_FOR_PLAYER_RECONNECT_PERIOD:
        this.HandleWaitForOpponentReconnectPeriod(change.Value);
        break;
    }
  }

  private void HandleWaitForOpponentReconnectPeriod(int periodInSeconds)
  {
    this.m_gameEntity.SetTag(GAME_TAG.WAIT_FOR_PLAYER_RECONNECT_PERIOD, periodInSeconds);
    if (periodInSeconds > 0)
    {
      this.ShowWaitForOpponentReconnectPopup(periodInSeconds);
      TurnTimerUpdate update = new TurnTimerUpdate();
      update.SetSecondsRemaining(float.PositiveInfinity);
      update.SetEndTimestamp(float.PositiveInfinity);
      update.SetShow(false);
      this.TriggerTurnTimerUpdate(update);
    }
    else
      this.HideWaitForOpponentReconnectPopup();
    GameMgr.Get().UpdatePresence();
  }

  private void ShowWaitForOpponentReconnectPopup(int periodInSeconds)
  {
    if (this.m_waitForOpponentReconnectPopupInfo == null)
    {
      this.m_waitForOpponentReconnectPopupInfo = new AlertPopup.PopupInfo();
      this.m_waitForOpponentReconnectPopupInfo.m_headerText = GameStrings.Get("GLOBAL_WAIT_FOR_OPPONENT_RECONNECT_HEADER");
      this.m_waitForOpponentReconnectPopupInfo.m_showAlertIcon = false;
      this.m_waitForOpponentReconnectPopupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.NONE;
      this.m_waitForOpponentReconnectPopupInfo.m_responseUserData = (object) periodInSeconds;
      this.m_waitForOpponentReconnectPopupInfo.m_alertTextAlignment = UberText.AlignmentOptions.Center;
      this.m_waitForOpponentReconnectPopupInfo.m_layerToUse = new GameLayer?(GameLayer.UI);
      DialogManager.Get().ShowPopup(this.m_waitForOpponentReconnectPopupInfo, new DialogManager.DialogProcessCallback(this.OnWaitForOpponentReconnectPopupProcessed));
      if (!((UnityEngine.Object) Gameplay.Get() != (UnityEngine.Object) null))
        return;
      this.IncreaseWaitForOpponentReconnectPeriod(Gameplay.Get().WaitForOpponentToken).Forget();
    }
    else
      this.UpdateWaitForOpponentReconnectPopup(periodInSeconds);
  }

  private bool OnWaitForOpponentReconnectPopupProcessed(DialogBase dialog, object userData)
  {
    this.m_waitForOpponentReconnectPopup = (AlertPopup) dialog;
    if (this.m_waitForOpponentReconnectPopupInfo == null)
      return false;
    this.UpdateWaitForOpponentReconnectPopup((int) this.m_waitForOpponentReconnectPopupInfo.m_responseUserData);
    return true;
  }

  private void HideWaitForOpponentReconnectPopup()
  {
    if ((UnityEngine.Object) Gameplay.Get() != (UnityEngine.Object) null)
      Gameplay.Get().StopIncreaseWaitForOpponentReconnectPeriod();
    if ((UnityEngine.Object) this.m_waitForOpponentReconnectPopup != (UnityEngine.Object) null)
      this.m_waitForOpponentReconnectPopup.Hide();
    this.m_waitForOpponentReconnectPopup = (AlertPopup) null;
    this.m_waitForOpponentReconnectPopupInfo = (AlertPopup.PopupInfo) null;
  }

  private void UpdateWaitForOpponentReconnectPopup(int periodInSeconds)
  {
    this.m_waitForOpponentReconnectPopupInfo.m_responseUserData = (object) periodInSeconds;
    int num1 = periodInSeconds / 60;
    int num2 = periodInSeconds % 60;
    this.m_waitForOpponentReconnectPopupInfo.m_text = string.Format(GameStrings.Get(GameMgr.Get().IsSpectator() ? "GLOBAL_WAIT_FOR_OPPONENT_RECONNECT_SPECTATOR" : "GLOBAL_WAIT_FOR_OPPONENT_RECONNECT"), (object) num1, (object) num2);
    if (!((UnityEngine.Object) this.m_waitForOpponentReconnectPopup != (UnityEngine.Object) null))
      return;
    this.m_waitForOpponentReconnectPopup.UpdateInfo(this.m_waitForOpponentReconnectPopupInfo);
  }

  private async UniTaskVoid IncreaseWaitForOpponentReconnectPeriod(
    CancellationToken token)
  {
    while (true)
    {
      await UniTask.Delay(TimeSpan.FromSeconds(1.0), cancellationToken: token);
      if (this.m_waitForOpponentReconnectPopupInfo != null)
      {
        int responseUserData = (int) this.m_waitForOpponentReconnectPopupInfo.m_responseUserData;
        int num;
        this.UpdateWaitForOpponentReconnectPopup(num = responseUserData + 1);
      }
      else
        break;
    }
  }

  private void PreprocessTagChange(Entity entity, TagDelta change)
  {
    switch ((GAME_TAG) change.tag)
    {
      case GAME_TAG.PLAYSTATE:
        if (!GameUtils.IsGameOverTag((Player) entity, change.tag, change.newValue))
          break;
        this.OnGameOver((TAG_PLAYSTATE) change.newValue);
        break;
      case GAME_TAG.TURN:
        this.OnTurnChanged(change.oldValue, change.newValue);
        break;
      case GAME_TAG.CURRENT_PLAYER:
        if (change.newValue != 1)
          break;
        this.OnCurrentPlayerChanged((Player) entity);
        break;
      case GAME_TAG.BACON_COMBAT_DAMAGE_CAP:
        this.OnDamageCapChanged(change.oldValue, change.newValue);
        break;
      case GAME_TAG.BACON_DIABLO_FIGHT_DIABLO_PLAYER_ID:
        this.OnDiabloFightPlayerIDChanged(change.oldValue, change.newValue);
        break;
    }
  }

  private void PreprocessEarlyConcedeTagChange(Entity entity, TagDelta change)
  {
    if (change.tag != 17 || !GameUtils.IsGameOverTag((Player) entity, change.tag, change.newValue))
      return;
    this.OnGameOver((TAG_PLAYSTATE) change.newValue);
  }

  private void ProcessEarlyConcedeTagChange(Entity entity, TagDelta change)
  {
    if (change.tag != 17)
      return;
    entity.OnTagChanged(change);
  }

  private void OnRealTimeGameOver(Network.HistTagChange change)
  {
    this.m_realTimeGameOverTagChange = change;
    if (Network.ShouldBeConnectedToAurora() && Network.IsLoggedIn())
      BnetPresenceMgr.Get().SetPresenceSpectatorJoinInfo((JoinInfo) null);
    SpectatorManager.Get().OnRealTimeGameOver();
    this.CheckRestartOnRealTimeGameOver();
  }

  private void OnGameOver(TAG_PLAYSTATE playState)
  {
    this.m_gameOver = true;
    this.m_realTimeGameOverTagChange = (Network.HistTagChange) null;
    this.m_gameEntity.NotifyOfGameOver(playState);
    this.FireGameOverEvent(playState);
    this.HideWaitForOpponentReconnectPopup();
    GameMgr.Get().LastGameData.GameResult = playState;
    if (this.GetFriendlySidePlayer() == null || this.GetFriendlySidePlayer().GetHero() == null)
      return;
    GameMgr.Get().LastGameData.BattlegroundsLeaderboardPlace = this.GetFriendlySidePlayer().GetHero().GetRealTimePlayerLeaderboardPlace();
  }

  private void OnCurrentPlayerChanged(Player player) => this.FireCurrentPlayerChangedEvent(player);

  private void OnTurnChanged(int oldTurn, int newTurn)
  {
    this.OnTurnChanged_TurnTimer(oldTurn, newTurn);
    this.FireTurnChangedEvent(oldTurn, newTurn);
  }

  private void OnDamageCapChanged(int oldValue, int newValue) => this.FireDamageCapChangedEvent(oldValue, newValue);

  private void OnDiabloFightPlayerIDChanged(int oldValue, int newValue) => this.FireDiabloFightPlayerIDChangedEvent(oldValue, newValue);

  public IEnumerator RejectUnresolvedChangesAfterDelay()
  {
    yield return (object) new WaitForSecondsRealtime(1f);
    this.RejectUnresolvedOptions();
  }

  private void RejectUnresolvedOptions()
  {
    if (this.m_lastSelectedOption == null || this.m_lastOptions == null || !ZoneMgr.Get().HasUnresolvedLocalChange())
      return;
    GameState.Get().OnOptionRejected(this.m_lastOptions.ID);
  }

  private void OnCantPlay(Entity entity) => this.FireCantPlayEvent(entity);

  public void AddServerBlockingSpell(Spell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null || this.m_serverBlockingSpells.Contains(spell))
      return;
    this.m_serverBlockingSpells.Add(spell);
  }

  public bool RemoveServerBlockingSpell(Spell spell) => this.m_serverBlockingSpells.Remove(spell);

  public void AddServerBlockingSpellController(SpellController spellController)
  {
    if ((UnityEngine.Object) spellController == (UnityEngine.Object) null || this.m_serverBlockingSpellControllers.Contains(spellController))
      return;
    this.m_serverBlockingSpellControllers.Add(spellController);
  }

  public bool RemoveServerBlockingSpellController(SpellController spellController) => this.m_serverBlockingSpellControllers.Remove(spellController);

  public void DebugNukeServerBlocks()
  {
    while (this.m_serverBlockingSpells.Count > 0)
      this.m_serverBlockingSpells[0].OnSpellFinished();
    while (this.m_serverBlockingSpellControllers.Count > 0)
      this.m_serverBlockingSpellControllers[0].ForceKill();
    this.m_powerProcessor.ForceStopHistoryBlocking();
    this.m_busy = false;
  }

  private bool IsBlockingPowerProcessor() => this.m_serverBlockingSpells.Count > 0 || this.m_serverBlockingSpellControllers.Count > 0 || this.m_powerProcessor.IsHistoryBlocking();

  private bool ShouldAdvanceReconnectIfStuckTimer()
  {
    foreach (Spell serverBlockingSpell in this.m_serverBlockingSpells)
    {
      if (serverBlockingSpell.ShouldReconnectIfStuck())
        return true;
    }
    foreach (SpellController blockingSpellController in this.m_serverBlockingSpellControllers)
    {
      if (blockingSpellController.ShouldReconnectIfStuck())
        return true;
    }
    return this.m_powerProcessor.IsHistoryBlocking();
  }

  public bool MustWaitForChoices()
  {
    if (!ChoiceCardMgr.Get().HasChoices())
      return false;
    PowerProcessor powerProcessor = GameState.Get().GetPowerProcessor();
    if (powerProcessor.HasGameOverTaskList())
      return false;
    foreach (int key in GameState.Get().GetPlayerMap().Keys)
    {
      PowerTaskList preChoiceTaskList = ChoiceCardMgr.Get().GetPreChoiceTaskList(key);
      if (preChoiceTaskList != null && !powerProcessor.HasTaskList(preChoiceTaskList))
        return true;
    }
    return false;
  }

  public bool CanProcessPowerQueue() => !this.IsBlockingPowerProcessor() && !this.IsBusy() && !this.MustWaitForChoices() && this.m_powerProcessor.GetCurrentTaskList() == null && this.m_powerProcessor.GetPowerQueue().Count != 0 && !this.WasRestartRequested();

  private bool CheckReconnectIfStuck()
  {
    if (!this.ShouldAdvanceReconnectIfStuckTimer())
    {
      this.m_reconnectIfStuckTimer = 0.0f;
      return false;
    }
    this.m_reconnectIfStuckTimer += Time.deltaTime;
    if (this.ReconnectIfStuck())
      return true;
    this.ReportStuck();
    return true;
  }

  private bool ReconnectIfStuck()
  {
    Network.GameSetup gameSetup = GameMgr.Get().GetGameSetup();
    if (gameSetup.DisconnectWhenStuckSeconds > 0U && (double) this.m_reconnectIfStuckTimer < (double) gameSetup.DisconnectWhenStuckSeconds)
      return false;
    Log.Power.PrintWarning("GameState.ReconnectIfStuck() - Blocked more than {0}. Cause:\n{1}", (object) TimeUtils.GetDevElapsedTimeString(this.m_reconnectIfStuckTimer), (object) this.BuildServerBlockingCausesString());
    PerformanceAnalytics.Get()?.ReconnectStart("STUCK");
    Network.Get().DisconnectFromGameServer();
    return true;
  }

  private void ReportStuck()
  {
    if ((double) this.m_reconnectIfStuckTimer < 10.0)
      return;
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    if ((double) realtimeSinceStartup - (double) this.m_lastBlockedReportTimestamp < 3.0)
      return;
    this.m_lastBlockedReportTimestamp = realtimeSinceStartup;
    Log.Power.PrintWarning("GameState.ReportStuck() - Stuck for {0}. {1}", (object) TimeUtils.GetDevElapsedTimeString(this.m_reconnectIfStuckTimer), (object) this.BuildServerBlockingCausesString());
  }

  private string BuildServerBlockingCausesString()
  {
    StringBuilder builder = new StringBuilder();
    int sectionCount = 0;
    this.AppendServerBlockingSection<Spell>(builder, "Spells:", this.m_serverBlockingSpells, new GameState.AppendBlockingServerItemCallback<Spell>(this.AppendServerBlockingSpell), ref sectionCount);
    this.AppendServerBlockingSection<SpellController>(builder, "SpellControllers:", this.m_serverBlockingSpellControllers, new GameState.AppendBlockingServerItemCallback<SpellController>(this.AppendServerBlockingSpellController), ref sectionCount);
    this.AppendServerBlockingHistory(builder, ref sectionCount);
    if (this.m_busy)
    {
      if (sectionCount > 0)
        builder.Append(' ');
      builder.Append("Busy=true");
      int num = sectionCount + 1;
    }
    return builder.ToString();
  }

  private void AppendServerBlockingSection<T>(
    StringBuilder builder,
    string sectionPrefix,
    List<T> items,
    GameState.AppendBlockingServerItemCallback<T> itemCallback,
    ref int sectionCount)
    where T : Component
  {
    if (items.Count == 0)
      return;
    if (sectionCount > 0)
      builder.Append(' ');
    builder.Append('{');
    builder.Append(sectionPrefix);
    for (int index = 0; index < items.Count; ++index)
    {
      builder.Append(' ');
      if (itemCallback == null)
        builder.Append(items[index].name);
      else
        itemCallback(builder, items[index]);
    }
    builder.Append('}');
    ++sectionCount;
  }

  private void AppendServerBlockingSpell(StringBuilder builder, Spell spell)
  {
    if ((UnityEngine.Object) spell == (UnityEngine.Object) null)
    {
      builder.Append("[null Spell (The Spell object may have been destroyed prematurely)]");
    }
    else
    {
      builder.Append('[');
      builder.Append(spell.name);
      builder.Append(' ');
      builder.AppendFormat("Source: {0}", (object) spell.GetSource());
      builder.Append(' ');
      builder.Append("Targets:");
      List<GameObject> targets = spell.GetTargets();
      if (targets.Count == 0)
      {
        builder.Append(' ');
        builder.Append("none");
      }
      else
      {
        for (int index = 0; index < targets.Count; ++index)
        {
          builder.Append(' ');
          GameObject gameObject = targets[index];
          builder.Append(((object) gameObject).ToString());
        }
      }
      builder.Append(']');
    }
  }

  private void AppendServerBlockingSpellController(
    StringBuilder builder,
    SpellController spellController)
  {
    builder.Append('[');
    builder.Append(spellController.name);
    builder.Append(' ');
    builder.AppendFormat("Source: {0}", (object) spellController.GetSource());
    builder.Append(' ');
    builder.Append("Targets:");
    List<Card> targets = spellController.GetTargets();
    if (targets.Count == 0)
    {
      builder.Append(' ');
      builder.Append("none");
    }
    else
    {
      for (int index = 0; index < targets.Count; ++index)
      {
        builder.Append(' ');
        Card card = targets[index];
        builder.Append(((object) card).ToString());
      }
    }
    builder.Append(']');
  }

  private void AppendServerBlockingHistory(StringBuilder builder, ref int sectionCount)
  {
    if (!this.m_powerProcessor.IsHistoryBlocking())
      return;
    Entity pendingBigCardEntity = HistoryManager.Get().GetPendingBigCardEntity();
    PowerTaskList blockingTaskList = this.m_powerProcessor.GetHistoryBlockingTaskList();
    PowerTaskList currentTaskList = this.m_powerProcessor.GetCurrentTaskList();
    if (sectionCount > 0)
      builder.Append(' ');
    builder.Append("History: ");
    builder.Append('{');
    builder.AppendFormat("PendingBigCard: {0}", (object) pendingBigCardEntity);
    builder.Append(' ');
    builder.AppendFormat("BlockingTaskList: ");
    this.PrintBlockingTaskList(builder, blockingTaskList);
    builder.Append(' ');
    builder.AppendFormat("CurrentTaskList: ");
    this.PrintBlockingTaskList(builder, currentTaskList);
    builder.Append('}');
    ++sectionCount;
  }

  public static bool RegisterGameStateInitializedListener(
    GameState.GameStateInitializedCallback callback,
    object userData = null)
  {
    if (callback == null)
      return false;
    GameState.GameStateInitializedListener initializedListener = new GameState.GameStateInitializedListener();
    initializedListener.SetCallback(callback);
    initializedListener.SetUserData(userData);
    if (GameState.s_gameStateInitializedListeners == null)
      GameState.s_gameStateInitializedListeners = new List<GameState.GameStateInitializedListener>();
    else if (GameState.s_gameStateInitializedListeners.Contains(initializedListener))
      return false;
    GameState.s_gameStateInitializedListeners.Add(initializedListener);
    return true;
  }

  public static bool UnregisterGameStateInitializedListener(
    GameState.GameStateInitializedCallback callback,
    object userData = null)
  {
    if (callback == null || GameState.s_gameStateInitializedListeners == null)
      return false;
    GameState.GameStateInitializedListener initializedListener = new GameState.GameStateInitializedListener();
    initializedListener.SetCallback(callback);
    initializedListener.SetUserData(userData);
    return GameState.s_gameStateInitializedListeners.Remove(initializedListener);
  }

  public bool RegisterCreateGameListener(GameState.CreateGameCallback callback) => this.RegisterCreateGameListener(callback, (object) null);

  public bool RegisterCreateGameListener(GameState.CreateGameCallback callback, object userData)
  {
    GameState.CreateGameListener createGameListener = new GameState.CreateGameListener();
    createGameListener.SetCallback(callback);
    createGameListener.SetUserData(userData);
    if (this.m_createGameListeners.Contains(createGameListener))
      return false;
    this.m_createGameListeners.Add(createGameListener);
    return true;
  }

  public bool UnregisterCreateGameListener(GameState.CreateGameCallback callback) => this.UnregisterCreateGameListener(callback, (object) null);

  public bool UnregisterCreateGameListener(GameState.CreateGameCallback callback, object userData)
  {
    GameState.CreateGameListener createGameListener = new GameState.CreateGameListener();
    createGameListener.SetCallback(callback);
    createGameListener.SetUserData(userData);
    return this.m_createGameListeners.Remove(createGameListener);
  }

  public bool RegisterOptionsReceivedListener(GameState.OptionsReceivedCallback callback) => this.RegisterOptionsReceivedListener(callback, (object) null);

  public bool RegisterOptionsReceivedListener(
    GameState.OptionsReceivedCallback callback,
    object userData)
  {
    GameState.OptionsReceivedListener receivedListener = new GameState.OptionsReceivedListener();
    receivedListener.SetCallback(callback);
    receivedListener.SetUserData(userData);
    if (this.m_optionsReceivedListeners.Contains(receivedListener))
      return false;
    this.m_optionsReceivedListeners.Add(receivedListener);
    return true;
  }

  public bool UnregisterOptionsReceivedListener(GameState.OptionsReceivedCallback callback) => this.UnregisterOptionsReceivedListener(callback, (object) null);

  public bool UnregisterOptionsReceivedListener(
    GameState.OptionsReceivedCallback callback,
    object userData)
  {
    GameState.OptionsReceivedListener receivedListener = new GameState.OptionsReceivedListener();
    receivedListener.SetCallback(callback);
    receivedListener.SetUserData(userData);
    return this.m_optionsReceivedListeners.Remove(receivedListener);
  }

  public bool RegisterOptionsSentListener(GameState.OptionsSentCallback callback, object userData = null)
  {
    GameState.OptionsSentListener optionsSentListener = new GameState.OptionsSentListener();
    optionsSentListener.SetCallback(callback);
    optionsSentListener.SetUserData(userData);
    if (this.m_optionsSentListeners.Contains(optionsSentListener))
      return false;
    this.m_optionsSentListeners.Add(optionsSentListener);
    return true;
  }

  public bool UnregisterOptionsSentListener(GameState.OptionsSentCallback callback, object userData = null)
  {
    GameState.OptionsSentListener optionsSentListener = new GameState.OptionsSentListener();
    optionsSentListener.SetCallback(callback);
    optionsSentListener.SetUserData(userData);
    return this.m_optionsSentListeners.Remove(optionsSentListener);
  }

  public bool RegisterOptionRejectedListener(
    GameState.OptionRejectedCallback callback,
    object userData = null)
  {
    GameState.OptionRejectedListener rejectedListener = new GameState.OptionRejectedListener();
    rejectedListener.SetCallback(callback);
    rejectedListener.SetUserData(userData);
    if (this.m_optionRejectedListeners.Contains(rejectedListener))
      return false;
    this.m_optionRejectedListeners.Add(rejectedListener);
    return true;
  }

  public bool UnregisterOptionRejectedListener(
    GameState.OptionRejectedCallback callback,
    object userData = null)
  {
    GameState.OptionRejectedListener rejectedListener = new GameState.OptionRejectedListener();
    rejectedListener.SetCallback(callback);
    rejectedListener.SetUserData(userData);
    return this.m_optionRejectedListeners.Remove(rejectedListener);
  }

  public bool RegisterEntityChoicesReceivedListener(GameState.EntityChoicesReceivedCallback callback) => this.RegisterEntityChoicesReceivedListener(callback, (object) null);

  public bool RegisterEntityChoicesReceivedListener(
    GameState.EntityChoicesReceivedCallback callback,
    object userData)
  {
    GameState.EntityChoicesReceivedListener receivedListener = new GameState.EntityChoicesReceivedListener();
    receivedListener.SetCallback(callback);
    receivedListener.SetUserData(userData);
    if (this.m_entityChoicesReceivedListeners.Contains(receivedListener))
      return false;
    this.m_entityChoicesReceivedListeners.Add(receivedListener);
    return true;
  }

  public bool UnregisterEntityChoicesReceivedListener(
    GameState.EntityChoicesReceivedCallback callback)
  {
    return this.UnregisterEntityChoicesReceivedListener(callback, (object) null);
  }

  public bool UnregisterEntityChoicesReceivedListener(
    GameState.EntityChoicesReceivedCallback callback,
    object userData)
  {
    GameState.EntityChoicesReceivedListener receivedListener = new GameState.EntityChoicesReceivedListener();
    receivedListener.SetCallback(callback);
    receivedListener.SetUserData(userData);
    return this.m_entityChoicesReceivedListeners.Remove(receivedListener);
  }

  public bool RegisterEntitiesChosenReceivedListener(
    GameState.EntitiesChosenReceivedCallback callback)
  {
    return this.RegisterEntitiesChosenReceivedListener(callback, (object) null);
  }

  public bool RegisterEntitiesChosenReceivedListener(
    GameState.EntitiesChosenReceivedCallback callback,
    object userData)
  {
    GameState.EntitiesChosenReceivedListener receivedListener = new GameState.EntitiesChosenReceivedListener();
    receivedListener.SetCallback(callback);
    receivedListener.SetUserData(userData);
    if (this.m_entitiesChosenReceivedListeners.Contains(receivedListener))
      return false;
    this.m_entitiesChosenReceivedListeners.Add(receivedListener);
    return true;
  }

  public bool UnregisterEntitiesChosenReceivedListener(
    GameState.EntitiesChosenReceivedCallback callback)
  {
    return this.UnregisterEntitiesChosenReceivedListener(callback, (object) null);
  }

  public bool UnregisterEntitiesChosenReceivedListener(
    GameState.EntitiesChosenReceivedCallback callback,
    object userData)
  {
    GameState.EntitiesChosenReceivedListener receivedListener = new GameState.EntitiesChosenReceivedListener();
    receivedListener.SetCallback(callback);
    receivedListener.SetUserData(userData);
    return this.m_entitiesChosenReceivedListeners.Remove(receivedListener);
  }

  public bool RegisterCurrentPlayerChangedListener(GameState.CurrentPlayerChangedCallback callback) => this.RegisterCurrentPlayerChangedListener(callback, (object) null);

  public bool RegisterCurrentPlayerChangedListener(
    GameState.CurrentPlayerChangedCallback callback,
    object userData)
  {
    GameState.CurrentPlayerChangedListener playerChangedListener = new GameState.CurrentPlayerChangedListener();
    playerChangedListener.SetCallback(callback);
    playerChangedListener.SetUserData(userData);
    if (this.m_currentPlayerChangedListeners.Contains(playerChangedListener))
      return false;
    this.m_currentPlayerChangedListeners.Add(playerChangedListener);
    return true;
  }

  public bool UnregisterCurrentPlayerChangedListener(GameState.CurrentPlayerChangedCallback callback) => this.UnregisterCurrentPlayerChangedListener(callback, (object) null);

  public bool UnregisterCurrentPlayerChangedListener(
    GameState.CurrentPlayerChangedCallback callback,
    object userData)
  {
    GameState.CurrentPlayerChangedListener playerChangedListener = new GameState.CurrentPlayerChangedListener();
    playerChangedListener.SetCallback(callback);
    playerChangedListener.SetUserData(userData);
    return this.m_currentPlayerChangedListeners.Remove(playerChangedListener);
  }

  public bool RegisterTurnChangedListener(GameState.TurnChangedCallback callback) => this.RegisterTurnChangedListener(callback, (object) null);

  public bool RegisterTurnChangedListener(GameState.TurnChangedCallback callback, object userData)
  {
    GameState.TurnChangedListener turnChangedListener = new GameState.TurnChangedListener();
    turnChangedListener.SetCallback(callback);
    turnChangedListener.SetUserData(userData);
    if (this.m_turnChangedListeners.Contains(turnChangedListener))
      return false;
    this.m_turnChangedListeners.Add(turnChangedListener);
    return true;
  }

  public bool UnregisterTurnChangedListener(GameState.TurnChangedCallback callback) => this.UnregisterTurnChangedListener(callback, (object) null);

  public bool UnregisterTurnChangedListener(GameState.TurnChangedCallback callback, object userData)
  {
    GameState.TurnChangedListener turnChangedListener = new GameState.TurnChangedListener();
    turnChangedListener.SetCallback(callback);
    turnChangedListener.SetUserData(userData);
    return this.m_turnChangedListeners.Remove(turnChangedListener);
  }

  public bool RegisterDamageCapChangedListener(GameState.DamageCapChangedCallback callback) => this.RegisterDamageCapChangedListener(callback, (object) null);

  public bool RegisterDamageCapChangedListener(
    GameState.DamageCapChangedCallback callback,
    object userData)
  {
    GameState.DamageCapChangedListener capChangedListener = new GameState.DamageCapChangedListener();
    capChangedListener.SetCallback(callback);
    capChangedListener.SetUserData(userData);
    if (this.m_damageCapChangedListeners.Contains(capChangedListener))
      return false;
    this.m_damageCapChangedListeners.Add(capChangedListener);
    return true;
  }

  public bool RegisterDiabloFightPlayerIDChangedListener(
    GameState.DiabloFightPlayerIDChangedCallback callback)
  {
    return this.RegisterDiabloFightPlayerIDChangedListener(callback, (object) null);
  }

  public bool RegisterDiabloFightPlayerIDChangedListener(
    GameState.DiabloFightPlayerIDChangedCallback callback,
    object userData)
  {
    GameState.DiabloFightPlayerIDChangedListener idChangedListener = new GameState.DiabloFightPlayerIDChangedListener();
    idChangedListener.SetCallback(callback);
    idChangedListener.SetUserData(userData);
    if (this.m_diabloFightPlayerIDChangedListeners.Contains(idChangedListener))
      return false;
    this.m_diabloFightPlayerIDChangedListeners.Add(idChangedListener);
    return true;
  }

  public bool UnregisterDamageCapChangedListener(GameState.DamageCapChangedCallback callback) => this.UnregisterDamageCapChangedListener(callback, (object) null);

  public bool UnregisterDamageCapChangedListener(
    GameState.DamageCapChangedCallback callback,
    object userData)
  {
    GameState.DamageCapChangedListener capChangedListener = new GameState.DamageCapChangedListener();
    capChangedListener.SetCallback(callback);
    capChangedListener.SetUserData(userData);
    return this.m_damageCapChangedListeners.Remove(capChangedListener);
  }

  public bool UnregisterDiabloFightPlayerIDChangedListener(
    GameState.DiabloFightPlayerIDChangedCallback callback)
  {
    return this.UnregisterDiabloFightPlayerIDChangedListener(callback, (object) null);
  }

  public bool UnregisterDiabloFightPlayerIDChangedListener(
    GameState.DiabloFightPlayerIDChangedCallback callback,
    object userData)
  {
    GameState.DiabloFightPlayerIDChangedListener idChangedListener = new GameState.DiabloFightPlayerIDChangedListener();
    idChangedListener.SetCallback(callback);
    idChangedListener.SetUserData(userData);
    return this.m_diabloFightPlayerIDChangedListeners.Remove(idChangedListener);
  }

  public bool RegisterFriendlyTurnStartedListener(
    GameState.FriendlyTurnStartedCallback callback,
    object userData = null)
  {
    GameState.FriendlyTurnStartedListener turnStartedListener = new GameState.FriendlyTurnStartedListener();
    turnStartedListener.SetCallback(callback);
    turnStartedListener.SetUserData(userData);
    if (this.m_friendlyTurnStartedListeners.Contains(turnStartedListener))
      return false;
    this.m_friendlyTurnStartedListeners.Add(turnStartedListener);
    return true;
  }

  public bool UnregisterFriendlyTurnStartedListener(
    GameState.FriendlyTurnStartedCallback callback,
    object userData = null)
  {
    GameState.FriendlyTurnStartedListener turnStartedListener = new GameState.FriendlyTurnStartedListener();
    turnStartedListener.SetCallback(callback);
    turnStartedListener.SetUserData(userData);
    return this.m_friendlyTurnStartedListeners.Remove(turnStartedListener);
  }

  public bool RegisterTurnTimerUpdateListener(GameState.TurnTimerUpdateCallback callback) => this.RegisterTurnTimerUpdateListener(callback, (object) null);

  public bool RegisterTurnTimerUpdateListener(
    GameState.TurnTimerUpdateCallback callback,
    object userData)
  {
    GameState.TurnTimerUpdateListener timerUpdateListener = new GameState.TurnTimerUpdateListener();
    timerUpdateListener.SetCallback(callback);
    timerUpdateListener.SetUserData(userData);
    if (this.m_turnTimerUpdateListeners.Contains(timerUpdateListener))
      return false;
    this.m_turnTimerUpdateListeners.Add(timerUpdateListener);
    return true;
  }

  public bool UnregisterTurnTimerUpdateListener(GameState.TurnTimerUpdateCallback callback) => this.UnregisterTurnTimerUpdateListener(callback, (object) null);

  public bool UnregisterTurnTimerUpdateListener(
    GameState.TurnTimerUpdateCallback callback,
    object userData)
  {
    GameState.TurnTimerUpdateListener timerUpdateListener = new GameState.TurnTimerUpdateListener();
    timerUpdateListener.SetCallback(callback);
    timerUpdateListener.SetUserData(userData);
    return this.m_turnTimerUpdateListeners.Remove(timerUpdateListener);
  }

  public bool RegisterMulliganTimerUpdateListener(GameState.TurnTimerUpdateCallback callback) => this.RegisterMulliganTimerUpdateListener(callback, (object) null);

  public bool RegisterMulliganTimerUpdateListener(
    GameState.TurnTimerUpdateCallback callback,
    object userData)
  {
    GameState.TurnTimerUpdateListener timerUpdateListener = new GameState.TurnTimerUpdateListener();
    timerUpdateListener.SetCallback(callback);
    timerUpdateListener.SetUserData(userData);
    if (this.m_mulliganTimerUpdateListeners.Contains(timerUpdateListener))
      return false;
    this.m_mulliganTimerUpdateListeners.Add(timerUpdateListener);
    return true;
  }

  public bool UnregisterMulliganTimerUpdateListener(GameState.TurnTimerUpdateCallback callback) => this.UnregisterMulliganTimerUpdateListener(callback, (object) null);

  public bool UnregisterMulliganTimerUpdateListener(
    GameState.TurnTimerUpdateCallback callback,
    object userData)
  {
    GameState.TurnTimerUpdateListener timerUpdateListener = new GameState.TurnTimerUpdateListener();
    timerUpdateListener.SetCallback(callback);
    timerUpdateListener.SetUserData(userData);
    return this.m_mulliganTimerUpdateListeners.Remove(timerUpdateListener);
  }

  public bool RegisterSpectatorNotifyListener(
    GameState.SpectatorNotifyEventCallback callback,
    object userData = null)
  {
    GameState.SpectatorNotifyListener spectatorNotifyListener = new GameState.SpectatorNotifyListener();
    spectatorNotifyListener.SetCallback(callback);
    spectatorNotifyListener.SetUserData(userData);
    if (this.m_spectatorNotifyListeners.Contains(spectatorNotifyListener))
      return false;
    this.m_spectatorNotifyListeners.Add(spectatorNotifyListener);
    return true;
  }

  public bool UnregisterSpectatorNotifyListener(
    GameState.SpectatorNotifyEventCallback callback,
    object userData = null)
  {
    GameState.SpectatorNotifyListener spectatorNotifyListener = new GameState.SpectatorNotifyListener();
    spectatorNotifyListener.SetCallback(callback);
    spectatorNotifyListener.SetUserData(userData);
    return this.m_spectatorNotifyListeners.Remove(spectatorNotifyListener);
  }

  public bool RegisterGameOverListener(GameState.GameOverCallback callback, object userData = null)
  {
    GameState.GameOverListener gameOverListener = new GameState.GameOverListener();
    gameOverListener.SetCallback(callback);
    gameOverListener.SetUserData(userData);
    if (this.m_gameOverListeners.Contains(gameOverListener))
      return false;
    this.m_gameOverListeners.Add(gameOverListener);
    return true;
  }

  public bool UnregisterGameOverListener(GameState.GameOverCallback callback, object userData = null)
  {
    GameState.GameOverListener gameOverListener = new GameState.GameOverListener();
    gameOverListener.SetCallback(callback);
    gameOverListener.SetUserData(userData);
    return this.m_gameOverListeners.Remove(gameOverListener);
  }

  public bool RegisterHeroChangedListener(GameState.HeroChangedCallback callback, object userData = null)
  {
    GameState.HeroChangedListener heroChangedListener = new GameState.HeroChangedListener();
    heroChangedListener.SetCallback(callback);
    heroChangedListener.SetUserData(userData);
    if (this.m_heroChangedListeners.Contains(heroChangedListener))
      return false;
    this.m_heroChangedListeners.Add(heroChangedListener);
    return true;
  }

  public bool UnregisterHeroChangedListener(GameState.HeroChangedCallback callback, object userData = null)
  {
    GameState.HeroChangedListener heroChangedListener = new GameState.HeroChangedListener();
    heroChangedListener.SetCallback(callback);
    heroChangedListener.SetUserData(userData);
    return this.m_heroChangedListeners.Remove(heroChangedListener);
  }

  public bool RegisterBusyStateChangedListener(
    GameState.BusyStateChangedCallback callback,
    object userData = null)
  {
    GameState.BusyStateChangedListener stateChangedListener = new GameState.BusyStateChangedListener();
    stateChangedListener.SetCallback(callback);
    stateChangedListener.SetUserData(userData);
    if (this.m_busyStateChangedListeners.Contains(stateChangedListener))
      return false;
    this.m_busyStateChangedListeners.Add(stateChangedListener);
    return true;
  }

  public bool UnregisterBusyStateChangedListener(
    GameState.BusyStateChangedCallback callback,
    object userData = null)
  {
    GameState.BusyStateChangedListener stateChangedListener = new GameState.BusyStateChangedListener();
    stateChangedListener.SetCallback(callback);
    stateChangedListener.SetUserData(userData);
    return this.m_busyStateChangedListeners.Remove(stateChangedListener);
  }

  public bool RegisterCantPlayListener(GameState.CantPlayCallback callback, object userData = null)
  {
    GameState.CantPlayListener cantPlayListener = new GameState.CantPlayListener();
    cantPlayListener.SetCallback(callback);
    cantPlayListener.SetUserData(userData);
    if (this.m_cantPlayListeners.Contains(cantPlayListener))
      return false;
    this.m_cantPlayListeners.Add(cantPlayListener);
    return true;
  }

  public bool UnregisterCantPlayListener(GameState.CantPlayCallback callback, object userData = null)
  {
    GameState.CantPlayListener cantPlayListener = new GameState.CantPlayListener();
    cantPlayListener.SetCallback(callback);
    cantPlayListener.SetUserData(userData);
    return this.m_cantPlayListeners.Remove(cantPlayListener);
  }

  private static void FireGameStateInitializedEvent()
  {
    if (GameState.s_gameStateInitializedListeners == null)
      return;
    foreach (GameState.GameStateInitializedListener initializedListener in GameState.s_gameStateInitializedListeners.ToArray())
      initializedListener.Fire(GameState.s_instance);
  }

  private void FireCreateGameEvent()
  {
    foreach (GameState.CreateGameListener createGameListener in this.m_createGameListeners.ToArray())
      createGameListener.Fire(this.m_createGamePhase);
  }

  private void FireOptionsReceivedEvent()
  {
    foreach (GameState.OptionsReceivedListener receivedListener in this.m_optionsReceivedListeners.ToArray())
      receivedListener.Fire();
  }

  private void FireOptionsSentEvent(Network.Options.Option option)
  {
    foreach (GameState.OptionsSentListener optionsSentListener in this.m_optionsSentListeners.ToArray())
      optionsSentListener.Fire(option);
  }

  private void FireOptionRejectedEvent(Network.Options.Option option)
  {
    foreach (GameState.OptionRejectedListener rejectedListener in this.m_optionRejectedListeners.ToArray())
      rejectedListener.Fire(option);
  }

  private void FireEntityChoicesReceivedEvent(
    Network.EntityChoices choices,
    PowerTaskList preChoiceTaskList)
  {
    foreach (GameState.EntityChoicesReceivedListener receivedListener in this.m_entityChoicesReceivedListeners.ToArray())
      receivedListener.Fire(choices, preChoiceTaskList);
  }

  private bool FireEntitiesChosenReceivedEvent(Network.EntitiesChosen chosen)
  {
    GameState.EntitiesChosenReceivedListener[] array = this.m_entitiesChosenReceivedListeners.ToArray();
    bool flag = false;
    foreach (GameState.EntitiesChosenReceivedListener receivedListener in array)
      flag = receivedListener.Fire(chosen) | flag;
    return flag;
  }

  private void FireTurnChangedEvent(int oldTurn, int newTurn)
  {
    foreach (GameState.TurnChangedListener turnChangedListener in this.m_turnChangedListeners.ToArray())
      turnChangedListener.Fire(oldTurn, newTurn);
  }

  private void FireDamageCapChangedEvent(int oldValue, int newValue)
  {
    foreach (GameState.DamageCapChangedListener capChangedListener in this.m_damageCapChangedListeners.ToArray())
      capChangedListener.Fire(oldValue, newValue);
  }

  private void FireDiabloFightPlayerIDChangedEvent(int oldValue, int newValue)
  {
    foreach (GameState.DiabloFightPlayerIDChangedListener idChangedListener in this.m_diabloFightPlayerIDChangedListeners.ToArray())
      idChangedListener.Fire(oldValue, newValue);
  }

  public void FireFriendlyTurnStartedEvent()
  {
    this.m_gameEntity.NotifyOfStartOfTurnEventsFinished();
    foreach (GameState.FriendlyTurnStartedListener turnStartedListener in this.m_friendlyTurnStartedListeners.ToArray())
      turnStartedListener.Fire();
  }

  private void FireTurnTimerUpdateEvent(TurnTimerUpdate update)
  {
    if (this.GetGameEntity() == null)
    {
      UnityEngine.Debug.LogWarning((object) "FireTurnTimerUpdateEvent - Turn timer update received before game entity created.");
    }
    else
    {
      foreach (GameState.TurnTimerUpdateListener timerUpdateListener in !this.GetGameEntity().IsMulliganActiveRealTime() ? this.m_turnTimerUpdateListeners.ToArray() : this.m_mulliganTimerUpdateListeners.ToArray())
        timerUpdateListener.Fire(update);
    }
  }

  private void FireCantPlayEvent(Entity entity)
  {
    foreach (GameState.CantPlayListener cantPlayListener in this.m_cantPlayListeners.ToArray())
      cantPlayListener.Fire(entity);
  }

  private void FireCurrentPlayerChangedEvent(Player player)
  {
    foreach (GameState.CurrentPlayerChangedListener playerChangedListener in this.m_currentPlayerChangedListeners.ToArray())
      playerChangedListener.Fire(player);
  }

  private void FireSpectatorNotifyEvent(SpectatorNotify notify)
  {
    foreach (GameState.SpectatorNotifyListener spectatorNotifyListener in this.m_spectatorNotifyListeners.ToArray())
      spectatorNotifyListener.Fire(notify);
  }

  private void FireGameOverEvent(TAG_PLAYSTATE playState)
  {
    foreach (GameState.GameOverListener gameOverListener in this.m_gameOverListeners.ToArray())
      gameOverListener.Fire(playState);
  }

  public void FireHeroChangedEvent(Player player)
  {
    foreach (GameState.HeroChangedListener heroChangedListener in this.m_heroChangedListeners.ToArray())
      heroChangedListener.Fire(player);
  }

  private void FireBusyStateChangedEvent(bool isBusy)
  {
    foreach (GameState.BusyStateChangedListener stateChangedListener in this.m_busyStateChangedListeners.ToArray())
      stateChangedListener.Fire(isBusy);
  }

  public GameState.ResponseMode GetResponseMode() => this.m_responseMode;

  public Network.EntityChoices GetFriendlyEntityChoices() => this.GetEntityChoices(this.GetFriendlyPlayerId());

  public Network.EntityChoices GetOpponentEntityChoices() => this.GetEntityChoices(this.GetOpposingPlayerId());

  public Network.EntityChoices GetEntityChoices(int playerId)
  {
    Network.EntityChoices entityChoices;
    this.m_choicesMap.TryGetValue(playerId, out entityChoices);
    return entityChoices;
  }

  public Map<int, Network.EntityChoices> GetEntityChoicesMap() => this.m_choicesMap;

  public bool IsChoosableEntity(Entity entity)
  {
    Network.EntityChoices friendlyEntityChoices = this.GetFriendlyEntityChoices();
    return friendlyEntityChoices != null && friendlyEntityChoices.Entities.Contains(entity.GetEntityId());
  }

  public bool IsChosenEntity(Entity entity) => this.GetFriendlyEntityChoices() != null && this.m_chosenEntities.Contains(entity);

  public bool AddChosenEntity(Entity entity)
  {
    if (this.m_chosenEntities.Contains(entity))
      return false;
    this.m_chosenEntities.Add(entity);
    ChoiceCardMgr.Get().OnChosenEntityAdded(entity);
    Card card = entity.GetCard();
    if ((UnityEngine.Object) card != (UnityEngine.Object) null)
      card.UpdateActorState();
    return true;
  }

  public bool RemoveChosenEntity(Entity entity)
  {
    if (!this.m_chosenEntities.Remove(entity))
      return false;
    ChoiceCardMgr.Get().OnChosenEntityRemoved(entity);
    Card card = entity.GetCard();
    if ((UnityEngine.Object) card != (UnityEngine.Object) null)
      card.UpdateActorState();
    return true;
  }

  public List<Entity> GetChosenEntities() => this.m_chosenEntities;

  public Network.Options GetOptionsPacket() => this.m_options;

  public void EnterChoiceMode()
  {
    this.m_responseMode = GameState.ResponseMode.CHOICE;
    this.UpdateOptionHighlights();
    this.UpdateChoiceHighlights();
  }

  public void EnterMainOptionMode()
  {
    GameState.ResponseMode responseMode = this.m_responseMode;
    this.m_responseMode = GameState.ResponseMode.OPTION;
    switch (responseMode)
    {
      case GameState.ResponseMode.SUB_OPTION:
        this.UpdateSubOptionHighlights(this.m_options.List[this.m_selectedOption.m_main]);
        break;
      case GameState.ResponseMode.OPTION_TARGET:
        Network.Options.Option option = this.m_options.List[this.m_selectedOption.m_main];
        this.UpdateTargetHighlights(option.Main);
        if (this.m_selectedOption.m_sub != -1)
        {
          this.UpdateTargetHighlights(option.Subs[this.m_selectedOption.m_sub]);
          break;
        }
        break;
    }
    this.UpdateOptionHighlights(this.m_lastOptions);
    this.UpdateOptionHighlights();
    this.m_selectedOption.Clear();
  }

  public void EnterSubOptionMode()
  {
    Network.Options.Option option = this.m_options.List[this.m_selectedOption.m_main];
    if (this.m_responseMode == GameState.ResponseMode.OPTION)
    {
      this.m_responseMode = GameState.ResponseMode.SUB_OPTION;
      this.UpdateOptionHighlights();
    }
    else if (this.m_responseMode == GameState.ResponseMode.OPTION_TARGET)
    {
      this.m_responseMode = GameState.ResponseMode.SUB_OPTION;
      this.UpdateTargetHighlights(option.Subs[this.m_selectedOption.m_sub]);
    }
    this.UpdateSubOptionHighlights(option);
  }

  public void EnterOptionTargetMode()
  {
    if (this.m_responseMode == GameState.ResponseMode.OPTION)
    {
      this.m_responseMode = GameState.ResponseMode.OPTION_TARGET;
      this.UpdateOptionHighlights();
      this.UpdateTargetHighlights(this.m_options.List[this.m_selectedOption.m_main].Main);
    }
    else
    {
      if (this.m_responseMode != GameState.ResponseMode.SUB_OPTION)
        return;
      this.m_responseMode = GameState.ResponseMode.OPTION_TARGET;
      Network.Options.Option option = this.m_options.List[this.m_selectedOption.m_main];
      this.UpdateSubOptionHighlights(option);
      this.UpdateTargetHighlights(option.Subs[this.m_selectedOption.m_sub]);
    }
  }

  public void EnterMoveMinionMode(Entity heldEntity, bool suppressGlow = false) => this.ActivateMoveMinionTargets(heldEntity, suppressGlow);

  public void ExitMoveMinionMode() => this.DeactivateMoveMinionTargetHighlights();

  public void CancelCurrentOptionMode()
  {
    if (this.IsInTargetMode())
      this.GetGameEntity().NotifyOfTargetModeCancelled();
    this.CancelSelectedOptionProposedMana();
    this.EnterMainOptionMode();
  }

  public bool IsInMainOptionMode() => this.m_responseMode == GameState.ResponseMode.OPTION;

  public bool IsInSubOptionMode() => this.m_responseMode == GameState.ResponseMode.SUB_OPTION;

  public bool IsInTargetMode() => this.m_responseMode == GameState.ResponseMode.OPTION_TARGET;

  public bool IsInChoiceMode() => this.m_responseMode == GameState.ResponseMode.CHOICE;

  public void SetSelectedOption(ChooseOption packet)
  {
    this.m_selectedOption.m_main = packet.Index;
    this.m_selectedOption.m_sub = packet.SubOption;
    this.m_selectedOption.m_target = packet.Target;
    this.m_selectedOption.m_position = packet.Position;
  }

  public void SetChosenEntities(ChooseEntities packet)
  {
    this.m_chosenEntities.Clear();
    foreach (int entity1 in packet.Entities)
    {
      Entity entity2 = this.GetEntity(entity1);
      if (entity2 != null)
        this.m_chosenEntities.Add(entity2);
    }
  }

  public void SetSelectedOption(int index) => this.m_selectedOption.m_main = index;

  public int GetSelectedOption() => this.m_selectedOption.m_main;

  public void SetSelectedSubOption(int index) => this.m_selectedOption.m_sub = index;

  public int GetSelectedSubOption() => this.m_selectedOption.m_sub;

  public void SetSelectedOptionTarget(int target) => this.m_selectedOption.m_target = target;

  public int GetSelectedOptionTarget() => this.m_selectedOption.m_target;

  public bool IsSelectedOptionFriendlyHero()
  {
    Entity hero = this.GetFriendlySidePlayer().GetHero();
    if (hero == null)
      return false;
    Network.Options.Option selectedNetworkOption = this.GetSelectedNetworkOption();
    return selectedNetworkOption != null && selectedNetworkOption.Main.ID == hero.GetEntityId();
  }

  public bool IsSelectedOptionFriendlyHeroPower()
  {
    Entity heroPower = this.GetFriendlySidePlayer().GetHeroPower();
    if (heroPower == null)
      return false;
    Network.Options.Option selectedNetworkOption = this.GetSelectedNetworkOption();
    return selectedNetworkOption != null && selectedNetworkOption.Main.ID == heroPower.GetEntityId();
  }

  public bool IsSelectedOptionMercenariesAbility()
  {
    Network.Options.Option selectedNetworkOption = this.GetSelectedNetworkOption();
    if (selectedNetworkOption == null)
      return false;
    Entity entity = this.GetEntity(selectedNetworkOption.Main.ID);
    return entity != null && entity.IsLettuceAbility();
  }

  public void SetSelectedOptionPosition(int position) => this.m_selectedOption.m_position = position;

  public int GetSelectedOptionPosition() => this.m_selectedOption.m_position;

  public Network.Options.Option GetSelectedNetworkOption() => this.m_selectedOption.m_main < 0 ? (Network.Options.Option) null : this.m_options.List[this.m_selectedOption.m_main];

  public Network.Options.Option.SubOption GetSelectedNetworkSubOption()
  {
    if (this.m_selectedOption.m_main < 0)
      return (Network.Options.Option.SubOption) null;
    Network.Options.Option option = this.m_options.List[this.m_selectedOption.m_main];
    return this.m_selectedOption.m_sub == -1 ? option.Main : option.Subs[this.m_selectedOption.m_sub];
  }

  public bool EntityHasSubOptions(Entity entity)
  {
    int entityId = entity.GetEntityId();
    Network.Options optionsPacket = this.GetOptionsPacket();
    if (optionsPacket == null)
      return false;
    for (int index = 0; index < optionsPacket.List.Count; ++index)
    {
      Network.Options.Option option = optionsPacket.List[index];
      if (option.Type == Network.Options.Option.OptionType.POWER && option.Main.ID == entityId)
        return option.Subs != null && option.Subs.Count > 0;
    }
    return false;
  }

  public bool EntityHasTargets(Entity entity) => this.EntityHasTargets(entity, false);

  public bool SubEntityHasTargets(Entity subEntity) => this.EntityHasTargets(subEntity, true);

  public bool EntityOnlyTrades(Entity entity)
  {
    int entityId = entity.GetEntityId();
    Network.Options optionsPacket = this.GetOptionsPacket();
    if (optionsPacket == null)
      return false;
    bool flag = false;
    for (int index = 0; index < optionsPacket.List.Count; ++index)
    {
      Network.Options.Option option = optionsPacket.List[index];
      if (option.Type == Network.Options.Option.OptionType.POWER && option.Main.ID == entityId)
      {
        if (option.Main.IsTradeOption())
          flag = true;
        else if (option.Main.HasValidTarget())
          return false;
      }
    }
    return flag;
  }

  public bool HasSubOptions(Entity entity)
  {
    if (!this.IsEntityInputEnabled(entity))
      return false;
    int entityId = entity.GetEntityId();
    Network.Options optionsPacket = this.GetOptionsPacket();
    for (int index = 0; index < optionsPacket.List.Count; ++index)
    {
      Network.Options.Option option = optionsPacket.List[index];
      if (option.Type == Network.Options.Option.OptionType.POWER && option.Main.ID == entityId)
        return option.Subs.Count > 0;
    }
    return false;
  }

  public int? GetErrorParam(Entity entity)
  {
    Network.Options optionsPacket = this.GetOptionsPacket();
    if (optionsPacket == null)
      return new int?();
    switch (this.GetResponseMode())
    {
      case GameState.ResponseMode.OPTION:
        Network.Options.Option optionFromEntityId1 = optionsPacket.GetOptionFromEntityID(entity.GetEntityId());
        if (optionFromEntityId1 != null && optionFromEntityId1.Type == Network.Options.Option.OptionType.POWER)
          return optionFromEntityId1.Main.PlayErrorInfo.PlayErrorParam;
        break;
      case GameState.ResponseMode.SUB_OPTION:
        Network.Options.Option.SubOption optionFromEntityId2 = this.GetSelectedNetworkOption().GetSubOptionFromEntityID(entity.GetEntityId());
        if (optionFromEntityId2 != null)
          return optionFromEntityId2.PlayErrorInfo.PlayErrorParam;
        break;
      case GameState.ResponseMode.OPTION_TARGET:
        return this.GetSelectedNetworkSubOption().GetErrorParamForTarget(entity.GetEntityId());
    }
    return new int?();
  }

  public PlayErrors.ErrorType GetErrorType(Entity entity)
  {
    Network.Options optionsPacket = this.GetOptionsPacket();
    if (optionsPacket == null || !GameState.Get().IsFriendlySidePlayerTurn())
      return PlayErrors.ErrorType.REQ_YOUR_TURN;
    switch (this.GetResponseMode())
    {
      case GameState.ResponseMode.OPTION:
        Network.Options.Option optionFromEntityId1 = optionsPacket.GetOptionFromEntityID(entity.GetEntityId());
        if (optionFromEntityId1 != null && optionFromEntityId1.Type == Network.Options.Option.OptionType.POWER)
          return optionFromEntityId1.Main.PlayErrorInfo.PlayError;
        break;
      case GameState.ResponseMode.SUB_OPTION:
        Network.Options.Option.SubOption optionFromEntityId2 = this.GetSelectedNetworkOption().GetSubOptionFromEntityID(entity.GetEntityId());
        if (optionFromEntityId2 != null)
          return optionFromEntityId2.PlayErrorInfo.PlayError;
        break;
      case GameState.ResponseMode.OPTION_TARGET:
        return this.GetSelectedNetworkSubOption().GetErrorForTarget(entity.GetEntityId());
    }
    return PlayErrors.ErrorType.INVALID;
  }

  public bool HasResponse(Entity entity, bool? wantTradeOption = null)
  {
    switch (this.GetResponseMode())
    {
      case GameState.ResponseMode.OPTION:
        return this.IsValidOption(entity, wantTradeOption);
      case GameState.ResponseMode.SUB_OPTION:
        return this.IsValidSubOption(entity);
      case GameState.ResponseMode.OPTION_TARGET:
        return this.IsValidOptionTarget(entity, true);
      case GameState.ResponseMode.CHOICE:
        return this.IsChoice(entity);
      default:
        return false;
    }
  }

  public bool IsChoice(Entity entity) => this.IsEntityInputEnabled(entity) && this.IsChoosableEntity(entity) && !this.IsChosenEntity(entity);

  public bool IsValidOption(Entity entity, bool? wantTradeOption = null)
  {
    if (!this.IsEntityInputEnabled(entity))
      return false;
    int entityId = entity.GetEntityId();
    Network.Options optionsPacket = this.GetOptionsPacket();
    if (optionsPacket == null)
      return false;
    for (int index = 0; index < optionsPacket.List.Count; ++index)
    {
      Network.Options.Option option = optionsPacket.List[index];
      if (option.Type == Network.Options.Option.OptionType.POWER && option.Main.PlayErrorInfo.IsValid() && option.Main.ID == entityId)
      {
        if (wantTradeOption.HasValue)
        {
          int num1 = option.Main.IsTradeOption() ? 1 : 0;
          bool? nullable = wantTradeOption;
          int num2 = nullable.GetValueOrDefault() ? 1 : 0;
          if (!(num1 == num2 & nullable.HasValue))
            continue;
        }
        return true;
      }
    }
    return false;
  }

  public bool IsValidSubOption(Entity entity)
  {
    if (!this.IsEntityInputEnabled(entity))
      return false;
    int entityId = entity.GetEntityId();
    Network.Options.Option selectedNetworkOption = this.GetSelectedNetworkOption();
    for (int index = 0; index < selectedNetworkOption.Subs.Count; ++index)
    {
      Network.Options.Option.SubOption sub = selectedNetworkOption.Subs[index];
      if (sub.ID == entityId)
        return sub.PlayErrorInfo.IsValid();
    }
    return false;
  }

  public bool IsValidOptionTarget(Entity entity, bool checkInputEnabled)
  {
    if (checkInputEnabled && !this.IsEntityInputEnabled(entity))
      return false;
    Network.Options.Option.SubOption networkSubOption = this.GetSelectedNetworkSubOption();
    return networkSubOption != null && networkSubOption.IsValidTarget(entity.GetEntityId());
  }

  public bool IsValidPotentialOptionTarget(Entity source, Entity target)
  {
    if (this.m_options == null)
      return false;
    int entityId = source.GetEntityId();
    foreach (Network.Options.Option option in this.m_options.List)
    {
      if (option.Type == Network.Options.Option.OptionType.POWER && option.Main.ID == entityId)
        return (option.Subs == null || option.Subs.Count <= 0) && option.Main.IsValidTarget(target.GetEntityId());
    }
    return false;
  }

  public bool IsEntityInputEnabled(Entity entity)
  {
    if (this.IsResponsePacketBlocked() || entity.IsBusy())
      return false;
    Card card = entity.GetCard();
    if ((UnityEngine.Object) card != (UnityEngine.Object) null)
    {
      if (!card.IsInputEnabled())
        return false;
      Zone zone = card.GetZone();
      if ((UnityEngine.Object) zone != (UnityEngine.Object) null && !zone.IsInputEnabled())
        return false;
    }
    return true;
  }

  private bool EntityHasTargets(Entity entity, bool isSubEntity)
  {
    int entityId = entity.GetEntityId();
    Network.Options optionsPacket = this.GetOptionsPacket();
    if (optionsPacket == null)
      return false;
    for (int index1 = 0; index1 < optionsPacket.List.Count; ++index1)
    {
      Network.Options.Option option = optionsPacket.List[index1];
      if (option.Type == Network.Options.Option.OptionType.POWER)
      {
        if (isSubEntity)
        {
          if (option.Subs != null)
          {
            for (int index2 = 0; index2 < option.Subs.Count; ++index2)
            {
              Network.Options.Option.SubOption sub = option.Subs[index2];
              if (sub.ID == entityId)
                return sub.HasValidTarget();
            }
          }
        }
        else if (option.Main.ID == entityId)
          return option.Main.HasValidTarget();
      }
    }
    return false;
  }

  private void CancelSelectedOptionProposedMana()
  {
    Network.Options.Option selectedNetworkOption = this.GetSelectedNetworkOption();
    if (selectedNetworkOption == null)
      return;
    this.GetFriendlySidePlayer().CancelAllProposedMana(this.GetEntity(selectedNetworkOption.Main.ID));
  }

  public void ClearResponseMode()
  {
    Log.Hand.Print(nameof (ClearResponseMode));
    this.m_responseMode = GameState.ResponseMode.NONE;
    ZoneMgr.Get().DismissMercenariesAbilityTray();
    RemoteActionHandler.Get().NotifyOpponentOfSelection(0);
    if (this.m_options != null)
    {
      for (int index = 0; index < this.m_options.List.Count; ++index)
      {
        Network.Options.Option option = this.m_options.List[index];
        if (option.Type == Network.Options.Option.OptionType.POWER)
          this.GetEntity(option.Main.ID)?.ClearBattlecryFlag();
      }
      this.UpdateHighlightsBasedOnSelection();
      this.UpdateOptionHighlights(this.m_options);
    }
    else
    {
      if (this.GetFriendlyEntityChoices() == null)
        return;
      this.UpdateChoiceHighlights();
    }
  }

  public void UpdateChoiceHighlights()
  {
    foreach (Network.EntityChoices entityChoices in this.m_choicesMap.Values)
    {
      Entity entity1 = this.GetEntity(entityChoices.Source);
      if (entity1 != null)
      {
        Card card = entity1.GetCard();
        if ((UnityEngine.Object) card != (UnityEngine.Object) null)
          card.UpdateActorState();
      }
      foreach (int entity2 in entityChoices.Entities)
      {
        Entity entity3 = this.GetEntity(entity2);
        if (entity3 != null)
        {
          Card card = entity3.GetCard();
          if (!((UnityEngine.Object) card == (UnityEngine.Object) null))
            card.UpdateActorState();
        }
      }
    }
    foreach (Entity chosenEntity in this.m_chosenEntities)
    {
      Card card = chosenEntity.GetCard();
      if (!((UnityEngine.Object) card == (UnityEngine.Object) null))
        card.UpdateActorState();
    }
  }

  private void UpdateHighlightsBasedOnSelection()
  {
    if (this.m_selectedOption.m_target != 0)
    {
      Network.Options.Option.SubOption networkSubOption = this.GetSelectedNetworkSubOption();
      if (networkSubOption == null)
        return;
      this.UpdateTargetHighlights(networkSubOption);
    }
    else
    {
      if (this.m_selectedOption.m_sub < 0)
        return;
      this.UpdateSubOptionHighlights(this.GetSelectedNetworkOption());
    }
  }

  public void UpdateOptionHighlights() => this.UpdateOptionHighlights(this.m_options);

  public void UpdateOptionHighlights(Network.Options options)
  {
    if (options == null || options.List == null)
      return;
    for (int index = 0; index < options.List.Count; ++index)
    {
      Network.Options.Option option = options.List[index];
      if (option.Type == Network.Options.Option.OptionType.POWER)
      {
        Entity entity = this.GetEntity(option.Main.ID);
        if (entity != null)
        {
          Card card = entity.GetCard();
          if (!((UnityEngine.Object) card == (UnityEngine.Object) null))
            card.UpdateActorState();
        }
      }
    }
  }

  private void UpdateSubOptionHighlights(Network.Options.Option option)
  {
    Entity entity1 = this.GetEntity(option.Main.ID);
    if (entity1 != null)
    {
      Card card = entity1.GetCard();
      if ((UnityEngine.Object) card != (UnityEngine.Object) null)
        card.UpdateActorState();
    }
    foreach (Network.Options.Option.SubOption sub in option.Subs)
    {
      Entity entity2 = this.GetEntity(sub.ID);
      if (entity2 != null)
      {
        Card card = entity2.GetCard();
        if (!((UnityEngine.Object) card == (UnityEngine.Object) null))
          card.UpdateActorState();
      }
    }
  }

  private void UpdateTargetHighlights(Network.Options.Option.SubOption subOption)
  {
    Entity entity1 = this.GetEntity(subOption.ID);
    if (entity1 != null)
    {
      Card card = entity1.GetCard();
      if ((UnityEngine.Object) card != (UnityEngine.Object) null)
        card.UpdateActorState();
    }
    foreach (Network.Options.Option.TargetOption target in subOption.Targets)
    {
      if (target.PlayErrorInfo.IsValid())
      {
        Entity entity2 = this.GetEntity(target.ID);
        if (entity2 != null)
        {
          Card card = entity2.GetCard();
          if (!((UnityEngine.Object) card == (UnityEngine.Object) null))
            card.UpdateActorState();
        }
      }
    }
  }

  public void DisableOptionHighlights(Network.Options options)
  {
    if (options == null || options.List == null)
      return;
    for (int index = 0; index < options.List.Count; ++index)
    {
      Network.Options.Option option = options.List[index];
      if (option.Type == Network.Options.Option.OptionType.POWER)
      {
        Entity entity = this.GetEntity(option.Main.ID);
        if (entity != null)
        {
          Card card = entity.GetCard();
          if (!((UnityEngine.Object) card == (UnityEngine.Object) null))
          {
            Actor actor = card.GetActor();
            if (!((UnityEngine.Object) actor == (UnityEngine.Object) null))
              actor.SetActorState(ActorStateType.CARD_IDLE);
          }
        }
      }
    }
  }

  public bool HasValidHoverTargetForMovedMinion(
    Entity movedEntity,
    out PlayErrors.ErrorType mainOptionPlayError)
  {
    mainOptionPlayError = PlayErrors.ErrorType.INVALID;
    List<Card> hoverTargetsInPlay = this.GetMoveMinionHoverTargetsInPlay();
    if (!hoverTargetsInPlay.Any<Card>() || this.m_options == null || this.m_options.List == null)
      return false;
    foreach (Network.Options.Option option1 in this.m_options.List)
    {
      Network.Options.Option option = option1;
      if (!((UnityEngine.Object) hoverTargetsInPlay.FirstOrDefault<Card>((Func<Card, bool>) (t => t.GetEntity().GetEntityId() == option.Main.ID)) == (UnityEngine.Object) null))
      {
        if (!option.Main.PlayErrorInfo.IsValid())
        {
          if (option.Main.PlayErrorInfo.PlayError != PlayErrors.ErrorType.INVALID)
            mainOptionPlayError = option.Main.PlayErrorInfo.PlayError;
        }
        else if (option.Main.IsValidTarget(movedEntity.GetEntityId()))
          return true;
      }
    }
    if (movedEntity.IsDormant())
      mainOptionPlayError = PlayErrors.ErrorType.REQ_TARGET_NOT_DORMANT;
    return false;
  }

  private void ActivateMoveMinionTargets(Entity movedEntity, bool suppressGlow = false)
  {
    if (movedEntity == null)
      return;
    this.DisableOptionHighlights(this.m_options);
    List<Card> hoverTargetsInPlay = this.GetMoveMinionHoverTargetsInPlay();
    if (!hoverTargetsInPlay.Any<Card>() || this.m_options == null || this.m_options.List == null)
      return;
    foreach (Network.Options.Option option1 in this.m_options.List)
    {
      Network.Options.Option option = option1;
      Card card = hoverTargetsInPlay.FirstOrDefault<Card>((Func<Card, bool>) (t => t.GetEntity().GetEntityId() == option.Main.ID));
      if (!((UnityEngine.Object) card == (UnityEngine.Object) null) && card.HasCardDef)
      {
        PlayMakerFSM cardDefComponent = card.GetCardDefComponent<PlayMakerFSM>();
        if (!((UnityEngine.Object) cardDefComponent == (UnityEngine.Object) null))
        {
          bool flag = option.Main.IsValidTarget(movedEntity.GetEntityId());
          cardDefComponent.Fsm.GetFsmGameObject("HoverTargetCard").Value = card.gameObject;
          cardDefComponent.Fsm.GetFsmBool("SuppressGlow").Value = suppressGlow || !flag;
          cardDefComponent.SendEvent("Action");
          if (flag)
            ManaCrystalMgr.Get().ProposeManaCrystalUsage(card.GetEntity());
        }
      }
    }
  }

  private void DeactivateMoveMinionTargetHighlights()
  {
    List<Card> hoverTargetsInPlay = this.GetMoveMinionHoverTargetsInPlay();
    if (!hoverTargetsInPlay.Any<Card>())
      return;
    foreach (Card card in hoverTargetsInPlay)
    {
      if (card.HasCardDef)
      {
        PlayMakerFSM cardDefComponent = card.GetCardDefComponent<PlayMakerFSM>();
        if (!((UnityEngine.Object) cardDefComponent == (UnityEngine.Object) null))
        {
          cardDefComponent.SendEvent("Death");
          ManaCrystalMgr.Get().CancelAllProposedMana(card.GetEntity());
        }
      }
    }
    this.UpdateOptionHighlights();
  }

  public bool HasEnoughManaForMoveMinionHoverTarget(Entity heldEntity)
  {
    Player friendlySidePlayer = this.GetFriendlySidePlayer();
    List<Card> hoverTargetsInPlay = this.GetMoveMinionHoverTargetsInPlay();
    foreach (Network.Options.Option option1 in this.m_options.List)
    {
      Network.Options.Option option = option1;
      if (option.Main.IsValidTarget(heldEntity.GetEntityId()))
      {
        Card card = hoverTargetsInPlay.FirstOrDefault<Card>((Func<Card, bool>) (t => t.GetEntity().GetEntityId() == option.Main.ID));
        if (!((UnityEngine.Object) card == (UnityEngine.Object) null) && friendlySidePlayer.GetNumAvailableResources() >= card.GetEntity().GetCost())
          return true;
      }
    }
    return hoverTargetsInPlay.Count <= 0;
  }

  private List<Card> GetMoveMinionHoverTargetsInPlay()
  {
    List<ZoneMoveMinionHoverTarget> zonesOfType = ZoneMgr.Get().FindZonesOfType<ZoneMoveMinionHoverTarget>(Player.Side.FRIENDLY);
    List<Card> moveMinionHoverTargets = new List<Card>();
    System.Action<ZoneMoveMinionHoverTarget> action = (System.Action<ZoneMoveMinionHoverTarget>) (z => moveMinionHoverTargets.AddRange((IEnumerable<Card>) z.GetCards()));
    zonesOfType.ForEach(action);
    return moveMinionHoverTargets;
  }

  public Network.Options GetLastOptions() => this.m_lastOptions;

  public bool FriendlyHeroIsTargetable()
  {
    if (this.m_responseMode == GameState.ResponseMode.OPTION_TARGET)
    {
      Network.Options.Option option = this.m_options.List[this.m_selectedOption.m_main];
      foreach (Network.Options.Option.TargetOption target in (this.m_selectedOption.m_sub != -1 ? option.Subs[this.m_selectedOption.m_sub] : option.Main).Targets)
      {
        if (target.PlayErrorInfo.IsValid())
        {
          Entity entity = this.GetEntity(target.ID);
          if (entity != null && !((UnityEngine.Object) entity.GetCard() == (UnityEngine.Object) null) && entity.IsHero() && entity.IsControlledByFriendlySidePlayer())
            return true;
        }
      }
    }
    return false;
  }

  private void ClearLastOptions()
  {
    this.m_lastOptions = (Network.Options) null;
    this.m_lastSelectedOption = (GameState.SelectedOption) null;
  }

  private void ClearOptions()
  {
    this.m_options = (Network.Options) null;
    this.m_selectedOption.Clear();
  }

  public void ClearFriendlyChoicesList() => this.m_chosenEntities.Clear();

  private void ClearFriendlyChoices()
  {
    this.m_chosenEntities.Clear();
    this.m_choicesMap.Remove(this.GetFriendlyPlayerId());
  }

  private void OnSelectedOptionsSent()
  {
    this.ClearResponseMode();
    this.m_lastOptions = new Network.Options();
    this.m_lastOptions.CopyFrom(this.m_options);
    this.m_lastSelectedOption = new GameState.SelectedOption();
    this.m_lastSelectedOption.CopyFrom(this.m_selectedOption);
    this.ClearOptions();
  }

  private void OnTimeout()
  {
    if (this.m_responseMode == GameState.ResponseMode.NONE)
      return;
    this.ClearResponseMode();
    this.ClearLastOptions();
    this.ClearOptions();
  }

  private void ClearEntityMap()
  {
    foreach (Entity entity in this.m_entityMap.Values.ToArray<Entity>())
      entity.Destroy();
    this.m_entityMap.Clear();
  }

  private void CleanGameState()
  {
    foreach (Zone zone in ZoneMgr.Get().GetZones())
      zone.Reset();
    ManaCrystalMgr.Get().Reset();
    foreach (Entity entity in this.m_entityMap.Values)
    {
      Card card = entity.GetCard();
      if ((UnityEngine.Object) card != (UnityEngine.Object) null)
      {
        card.DeactivatePlaySpell();
        card.CancelActiveSpells();
        card.CancelCustomSpells();
      }
    }
    foreach (Entity entity in this.m_entityMap.Values)
    {
      Card card = entity.GetCard();
      if ((UnityEngine.Object) card != (UnityEngine.Object) null)
        card.Destroy();
    }
    this.m_playerMap.Clear();
    this.m_entityMap.Clear();
    this.m_removedFromGameEntities.Clear();
    this.m_removedFromGameEntityLog.Clear();
  }

  private void CreateGameEntity(
    List<Network.PowerHistory> powerList,
    Network.HistCreateGame createGame)
  {
    this.m_gameEntity = GameMgr.Get().CreateGameEntity(powerList, createGame);
    this.m_gameEntity.Uuid = createGame.Uuid;
    this.m_gameEntity.SetTags(createGame.Game.Tags);
    this.m_gameEntity.InitRealTimeValues(createGame.Game.Tags);
    this.AddEntity((Entity) this.m_gameEntity);
    this.m_gameEntity.OnCreate();
    this.m_gameEntity.OnLoadActions.AddRange((IEnumerable<Network.HistCreateGame.ActionInfo>) createGame.ActionInfos);
  }

  public void OnRealTimeCreateGame(
    List<Network.PowerHistory> powerList,
    int index,
    Network.HistCreateGame createGame)
  {
    if (this.m_gameEntity != null)
    {
      Log.Power.PrintError("{0}.OnRealTimeCreateGame(): there is already a game entity!", (object) this);
      this.m_gameEntity.OnDecommissionGame();
      this.CleanGameState();
    }
    if (powerList.Count == 1)
    {
      string str = "Game Created without entries:" + string.Format(" BuildNumber={0}", (object) 158725) + string.Format(" GameType={0}", (object) GameMgr.Get().GetGameType()) + string.Format(" FormatType={0}", (object) GameMgr.Get().GetFormatType()) + string.Format(" ScenarioID={0}", (object) GameMgr.Get().GetMissionId()) + string.Format(" IsReconnect={0}", (object) GameMgr.Get().IsReconnect());
      if (GameMgr.Get().IsReconnect())
        str += string.Format(" ReconnectType={0}", (object) GameMgr.Get().GetReconnectType());
      Log.Power.Print(str);
      TelemetryManager.Client().SendLiveIssue("Gameplay_GameState", str);
    }
    this.CreateGameEntity(powerList, createGame);
    foreach (Network.HistCreateGame.PlayerData player1 in createGame.Players)
    {
      Player player2 = new Player();
      player2.InitPlayer(player1);
      this.AddPlayer(player2);
    }
    int friendlySideTeamId = this.GetFriendlySideTeamId();
    foreach (Player player in this.m_playerMap.Values)
      player.UpdateSide(friendlySideTeamId);
    foreach (Network.HistCreateGame.SharedPlayerInfo playerInfo1 in createGame.PlayerInfos)
    {
      SharedPlayerInfo playerInfo2 = new SharedPlayerInfo();
      playerInfo2.InitPlayerInfo(playerInfo1);
      this.AddPlayerInfo(playerInfo2);
    }
    this.m_createGamePhase = GameState.CreateGamePhase.CREATING;
    this.FireCreateGameEvent();
    if (this.m_gameEntity.HasTag(GAME_TAG.WAIT_FOR_PLAYER_RECONNECT_PERIOD))
      this.HandleWaitForOpponentReconnectPeriod(this.m_gameEntity.GetTag(GAME_TAG.WAIT_FOR_PLAYER_RECONNECT_PERIOD));
    this.DebugPrintGame();
  }

  public bool OnRealTimeFullEntity(Network.HistFullEntity fullEntity)
  {
    Entity entity = new Entity();
    entity.OnRealTimeFullEntity(fullEntity);
    this.AddEntity(entity);
    return true;
  }

  public bool OnFullEntity(Network.HistFullEntity fullEntity)
  {
    Network.Entity entity1 = fullEntity.Entity;
    Entity entity2 = this.GetEntity(entity1.ID);
    if (entity2 == null)
    {
      Log.Power.PrintWarning("GameState.OnFullEntity() - WARNING entity {0} DOES NOT EXIST!", (object) entity1.ID);
      return false;
    }
    entity2.OnFullEntity(fullEntity);
    return true;
  }

  public bool OnRealTimeShowEntity(Network.HistShowEntity showEntity)
  {
    if (this.EntityRemovedFromGame(showEntity.Entity.ID))
      return false;
    Network.Entity entity1 = showEntity.Entity;
    Entity entity2 = this.GetEntity(entity1.ID);
    if (entity2 == null)
    {
      Log.Power.PrintWarning("GameState.OnRealTimeShowEntity() - WARNING entity {0} DOES NOT EXIST!", (object) entity1.ID);
      return false;
    }
    entity2.OnRealTimeShowEntity(showEntity);
    return true;
  }

  public bool OnShowEntity(Network.HistShowEntity showEntity)
  {
    if (this.EntityRemovedFromGame(showEntity.Entity.ID))
      return false;
    Network.Entity entity1 = showEntity.Entity;
    Entity entity2 = this.GetEntity(entity1.ID);
    if (entity2 == null)
    {
      Log.Power.PrintWarning("GameState.OnShowEntity() - WARNING entity {0} DOES NOT EXIST!", (object) entity1.ID);
      return false;
    }
    entity2.OnShowEntity(showEntity);
    return true;
  }

  public bool OnEarlyConcedeShowEntity(Network.HistShowEntity showEntity)
  {
    if (this.EntityRemovedFromGame(showEntity.Entity.ID))
      return false;
    Network.Entity entity1 = showEntity.Entity;
    Entity entity2 = this.GetEntity(entity1.ID);
    if (entity2 == null)
    {
      Log.Power.PrintWarning("GameState.OnEarlyConcedeShowEntity() - WARNING entity {0} DOES NOT EXIST!", (object) entity1.ID);
      return false;
    }
    entity2.SetTags(entity1.Tags);
    return true;
  }

  public bool OnHideEntity(Network.HistHideEntity hideEntity)
  {
    if (this.EntityRemovedFromGame(hideEntity.Entity))
      return false;
    Entity entity = this.GetEntity(hideEntity.Entity);
    if (entity == null)
    {
      Log.Power.PrintWarning("GameState.OnHideEntity() - WARNING entity {0} DOES NOT EXIST! zone={1}", (object) hideEntity.Entity, (object) hideEntity.Zone);
      return false;
    }
    entity.OnHideEntity(hideEntity);
    return true;
  }

  public bool OnEarlyConcedeHideEntity(Network.HistHideEntity hideEntity)
  {
    if (this.EntityRemovedFromGame(hideEntity.Entity))
      return false;
    Entity entity = this.GetEntity(hideEntity.Entity);
    if (entity == null)
    {
      Log.Power.PrintWarning("GameState.OnEarlyConcedeHideEntity() - WARNING entity {0} DOES NOT EXIST! zone={1}", (object) hideEntity.Entity, (object) hideEntity.Zone);
      return false;
    }
    entity.SetTag(GAME_TAG.ZONE, hideEntity.Zone);
    return true;
  }

  public bool OnRealTimeChangeEntity(
    List<Network.PowerHistory> powerList,
    int index,
    Network.HistChangeEntity changeEntity)
  {
    if (this.EntityRemovedFromGame(changeEntity.Entity.ID))
      return false;
    Network.Entity entity1 = changeEntity.Entity;
    Entity entity2 = this.GetEntity(entity1.ID);
    if (entity2 == null)
    {
      Log.Power.PrintWarning("GameState.OnRealTimeChangeEntity() - WARNING entity {0} DOES NOT EXIST!", (object) entity1.ID);
      return false;
    }
    entity2.OnRealTimeChangeEntity(powerList, index, changeEntity);
    return true;
  }

  public bool OnChangeEntity(Network.HistChangeEntity changeEntity)
  {
    if (this.EntityRemovedFromGame(changeEntity.Entity.ID))
      return false;
    Network.Entity entity1 = changeEntity.Entity;
    Entity entity2 = this.GetEntity(entity1.ID);
    if (entity2 == null)
    {
      Log.Power.PrintWarning("GameState.OnChangeEntity() - WARNING entity {0} DOES NOT EXIST!", (object) entity1.ID);
      return false;
    }
    entity2.OnChangeEntity(changeEntity);
    return true;
  }

  public bool OnEarlyConcedeChangeEntity(Network.HistChangeEntity changeEntity)
  {
    if (this.EntityRemovedFromGame(changeEntity.Entity.ID))
      return false;
    Network.Entity entity1 = changeEntity.Entity;
    Entity entity2 = this.GetEntity(entity1.ID);
    if (entity2 == null)
    {
      Log.Power.PrintWarning("GameState.OnEarlyConcedeChangeEntity() - WARNING entity {0} DOES NOT EXIST!", (object) entity1.ID);
      return false;
    }
    entity2.SetTags(entity1.Tags);
    return true;
  }

  public bool OnRealTimeTagChange(Network.HistTagChange change)
  {
    if (change == null)
    {
      Log.Power.PrintError("GameState.OnRealTimeTagChange() - ERROR HistTagChange is NULL");
      return false;
    }
    if (this.EntityRemovedFromGame(change.Entity))
      return false;
    Entity entity = (Entity) null;
    if (!this.m_entityMap.TryGetValue(change.Entity, out entity))
    {
      Log.Power.PrintWarning(string.Format("GameState.OnRealTimeTagChange() - WARNING Entity {0} does not exist", (object) change.Entity));
      return false;
    }
    if (entity == null)
    {
      Log.Power.PrintWarning(string.Format("GameState.OnRealTimeTagChange() - WARNING Entity {0} is mapped to a NULL Entity", (object) change.Entity));
      return false;
    }
    if (change.ChangeDef)
      return false;
    this.PreprocessRealTimeTagChange(entity, change);
    if (this.m_gameEntity == null)
    {
      Log.Power.PrintWarning("GameState.OnRealTimeTagChange() - WARNING GameEntity has been removed during RealTimeTagChange");
      return false;
    }
    this.m_gameEntity.NotifyOfRealTimeTagChange(entity, change);
    entity.OnRealTimeTagChanged(change);
    return true;
  }

  public bool OnTagChange(Network.HistTagChange netChange)
  {
    if (this.EntityRemovedFromGame(netChange.Entity))
      return false;
    Entity entity = this.GetEntity(netChange.Entity);
    if (entity == null)
    {
      UnityEngine.Debug.LogWarningFormat("GameState.OnTagChange() - WARNING Entity {0} does not exist", (object) netChange.Entity);
      return false;
    }
    TagDelta change = new TagDelta();
    change.tag = netChange.Tag;
    change.oldValue = entity.GetTag(netChange.Tag);
    change.newValue = netChange.Value;
    if (netChange.ChangeDef)
      entity.GetOrCreateDynamicDefinition().SetTag(change.tag, change.newValue);
    else
      entity.SetTag(change.tag, change.newValue);
    this.PreprocessTagChange(entity, change);
    entity.OnTagChanged(change);
    return true;
  }

  public void OnRealTimeVoSpell(Network.HistVoSpell voSpell)
  {
    if (voSpell == null)
      return;
    SoundLoader.LoadSound(new AssetReference(voSpell.SpellPrefabGUID), new PrefabCallback<GameObject>(this.OnSoundLoaded), (object) voSpell, SoundManager.Get().GetPlaceholderSound());
  }

  public bool OnCachedTagForDormantChange(Network.HistCachedTagForDormantChange netChange)
  {
    if (this.EntityRemovedFromGame(netChange.Entity))
      return false;
    Entity entity = this.GetEntity(netChange.Entity);
    if (entity == null)
    {
      UnityEngine.Debug.LogWarningFormat("GameState.OnCachedTagForDormantChange() - WARNING Entity {0} does not exist", (object) netChange.Entity);
      return false;
    }
    entity.OnCachedTagForDormantChanged(new TagDelta()
    {
      tag = netChange.Tag,
      oldValue = entity.GetTag(netChange.Tag),
      newValue = netChange.Value
    });
    return true;
  }

  public bool OnShuffleDeck(Network.HistShuffleDeck shuffleDeck)
  {
    Player player = this.GetPlayer(shuffleDeck.PlayerID);
    if (player == null)
    {
      UnityEngine.Debug.LogWarningFormat("GameState.OnShuffleDeck() - WARNING Player for ID {0} does not exist", (object) shuffleDeck.PlayerID);
      return false;
    }
    if (this.EntityRemovedFromGame(player.GetEntityId()))
      return false;
    player.OnShuffleDeck();
    return true;
  }

  private void OnSoundLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      UnityEngine.Debug.LogWarning((object) string.Format("{0} - FAILED to load \"{1}\"", (object) MethodBase.GetCurrentMethod().Name, (object) assetRef));
    }
    else
    {
      AudioSource component = go.GetComponent<AudioSource>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        UnityEngine.Debug.LogWarning((object) string.Format("{0} - ERROR \"{1}\" has no {2} component", (object) assetRef, (object) MethodBase.GetCurrentMethod().Name, (object) "AudioSource"));
      }
      else
      {
        if (!(callbackData is Network.HistVoSpell histVoSpell))
          return;
        histVoSpell.m_audioSource = component;
        histVoSpell.m_ableToLoad = true;
      }
    }
  }

  public bool OnVoSpell(Network.HistVoSpell voSpell)
  {
    if (voSpell == null || !voSpell.m_ableToLoad)
      return false;
    AudioSource mAudioSource = voSpell.m_audioSource;
    if ((UnityEngine.Object) mAudioSource == (UnityEngine.Object) null || (UnityEngine.Object) mAudioSource.clip == (UnityEngine.Object) null)
      return false;
    double additionalDelayMs = (double) voSpell.AdditionalDelayMs;
    float num1 = mAudioSource.clip.length * 1000f;
    float num2 = (float) additionalDelayMs;
    if (voSpell.Blocking)
      num2 += num1;
    if ((double) num2 > 0.0)
    {
      if ((UnityEngine.Object) Gameplay.Get() != (UnityEngine.Object) null)
        this.m_powerProcessor.ArtificiallyPausePowerProcessor(num2, Gameplay.Get().PausePowerToken).Forget();
      if (this.m_gameEntity is MissionEntity)
        (this.m_gameEntity as MissionEntity).SetBlockVo(true, num2 / 1000f);
    }
    string[] strArray = voSpell.SpellPrefabGUID.Split(':');
    if (strArray.Length != 2)
      return false;
    string str = strArray[0];
    if (!str.EndsWith(".prefab"))
      return false;
    string localizedTextKey = str.Substring(0, str.Length - ".prefab".Length);
    if (voSpell.Speaker != 0)
    {
      Actor actor = this.GetEntity(voSpell.Speaker)?.GetCard()?.GetActor();
      if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
        this.CharacterInPlaySpeak(voSpell, actor, localizedTextKey, num2);
    }
    else if (!string.IsNullOrEmpty(voSpell.BrassRingGUID))
      this.BrassRingCharacterSpeak(voSpell, localizedTextKey, num2);
    return true;
  }

  private void CharacterInPlaySpeak(
    Network.HistVoSpell voSpell,
    Actor speakingActor,
    string localizedTextKey,
    float totalPauseTimeMs)
  {
    if (voSpell == null || (UnityEngine.Object) speakingActor == (UnityEngine.Object) null || string.IsNullOrEmpty(localizedTextKey) || (double) totalPauseTimeMs < 0.0)
      return;
    if ((UnityEngine.Object) voSpell.m_audioSource != (UnityEngine.Object) null)
      SoundManager.Get().PlayPreloaded(voSpell.m_audioSource);
    Entity entity = speakingActor.GetEntity();
    Notification.SpeechBubbleDirection direction = !entity.IsControlledByFriendlySidePlayer() ? (!entity.IsMinion() ? Notification.SpeechBubbleDirection.TopLeft : Notification.SpeechBubbleDirection.BottomLeft) : Notification.SpeechBubbleDirection.BottomLeft;
    if ((double) totalPauseTimeMs <= 0.0 || direction == Notification.SpeechBubbleDirection.None)
      return;
    NotificationManager notificationManager = NotificationManager.Get();
    bool parentToActor = !((UnityEngine.Object) speakingActor.GetCard() != (UnityEngine.Object) null) || speakingActor.GetCard().GetEntity() == null || !speakingActor.GetCard().GetEntity().IsHeroPower();
    notificationManager.DestroyNotification(notificationManager.CreateSpeechBubble(GameStrings.Get(localizedTextKey), direction, speakingActor, false, parentToActor), totalPauseTimeMs / 1000f);
  }

  private void BrassRingCharacterSpeak(
    Network.HistVoSpell voSpell,
    string localizedTextKey,
    float soundLengthMs)
  {
    if (voSpell == null || string.IsNullOrEmpty(localizedTextKey) || (double) soundLengthMs <= 0.0)
      return;
    NotificationManager notificationManager = NotificationManager.Get();
    if ((UnityEngine.Object) notificationManager == (UnityEngine.Object) null)
      return;
    Vector3 zero = Vector3.zero;
    Notification.SpeechBubbleDirection bubbleDir = Notification.SpeechBubbleDirection.None;
    notificationManager.CreateBigCharacterQuoteWithGameString(voSpell.BrassRingGUID, zero, voSpell.SpellPrefabGUID, localizedTextKey, durationSeconds: (soundLengthMs / 1000f), bubbleDir: bubbleDir);
  }

  public bool OnVoBanter(Network.HistVoBanter voBanter)
  {
    if (voBanter == null || voBanter.EmoteEvent == PowerHistoryVoBanter.ClientEmoteEvent.INVALID || !(this.m_gameEntity is LettuceMissionEntity gameEntity))
      return false;
    if (voBanter.Speaker != 0)
    {
      gameEntity.OnVoBanter_OneSpeaker(voBanter.Speaker, voBanter.EmoteEvent);
      return true;
    }
    if (voBanter.Teams == null || voBanter.Teams.Count <= 0)
      return false;
    gameEntity.OnVoBanter_TeamDialogue(voBanter.Teams, voBanter.EmoteEvent);
    return true;
  }

  public bool OnEarlyConcedeTagChange(Network.HistTagChange netChange)
  {
    if (this.EntityRemovedFromGame(netChange.Entity))
      return false;
    Entity entity = this.GetEntity(netChange.Entity);
    if (entity == null)
    {
      UnityEngine.Debug.LogWarningFormat("GameState.OnEarlyConcedeTagChange() - WARNING Entity {0} does not exist", (object) netChange.Entity);
      return false;
    }
    TagDelta change = new TagDelta();
    change.tag = netChange.Tag;
    change.oldValue = entity.GetTag(netChange.Tag);
    change.newValue = netChange.Value;
    entity.SetTag(change.tag, change.newValue);
    this.PreprocessEarlyConcedeTagChange(entity, change);
    this.ProcessEarlyConcedeTagChange(entity, change);
    return true;
  }

  public bool OnRealTimeResetGame(Network.HistResetGame resetGame)
  {
    if (this.m_realTimeResetGame != null)
      Log.Gameplay.PrintError("{0}.OnRealTimeResetGame: There is already a ResetGame task we're waiting to execute!", (object) this);
    this.m_realTimeResetGame = resetGame;
    foreach (Zone zone in ZoneMgr.Get().GetZones())
      zone.AddInputBlocker();
    return true;
  }

  public bool OnResetGame(Network.HistResetGame resetGame)
  {
    if (this.m_realTimeResetGame != resetGame)
      Log.Power.PrintError("{0}.OnResetGame(): Passed ResetGame Task {0} does not match the expected ResetGame Task {1}!", (object) this, (object) resetGame, (object) this.m_realTimeResetGame);
    if (this.m_gameEntity != null)
    {
      this.m_gameEntity.OnDecommissionGame();
      this.CleanGameState();
    }
    List<Network.PowerHistory> powerList = new List<Network.PowerHistory>();
    foreach (PowerTask task in this.m_powerProcessor.GetCurrentTaskList().GetTaskList())
      powerList.Add(task.GetPower());
    this.CreateGameEntity(powerList, resetGame.CreateGame);
    foreach (Network.HistCreateGame.PlayerData player1 in resetGame.CreateGame.Players)
    {
      Player player2 = new Player();
      player2.InitPlayer(player1);
      this.AddPlayer(player2);
    }
    int friendlySideTeamId = this.GetFriendlySideTeamId();
    foreach (Player player in this.m_playerMap.Values)
    {
      player.UpdateSide(friendlySideTeamId);
      player.OnBoardLoaded();
    }
    this.m_realTimeResetGame = (Network.HistResetGame) null;
    this.m_powerProcessor.FlushDelayedRealTimeTasks();
    return true;
  }

  public bool OnMetaData(Network.HistMetaData metaData)
  {
    this.m_powerProcessor.OnMetaData(metaData);
    switch (metaData.MetaType)
    {
      case HistoryMeta.Type.SHOW_BIG_CARD:
      case HistoryMeta.Type.CONTROLLER_AND_ZONE_CHANGE:
        if (metaData.Info.Count == 0)
          return false;
        int num = metaData.Info[0];
        Entity entity1 = this.GetEntity(num);
        if (entity1 == null)
        {
          if (!this.EntityRemovedFromGame(num))
            UnityEngine.Debug.LogWarning((object) string.Format("GameState.OnMetaData() - WARNING Entity {0} does not exist", (object) num));
          return false;
        }
        entity1.OnMetaData(metaData);
        break;
      default:
        using (List<int>.Enumerator enumerator = metaData.Info.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            int current = enumerator.Current;
            Entity entity2 = this.GetEntity(current);
            if (entity2 == null)
            {
              if (!this.EntityRemovedFromGame(current))
                UnityEngine.Debug.LogWarning((object) string.Format("GameState.OnMetaData() - WARNING Entity {0} does not exist", (object) current));
              return false;
            }
            entity2.OnMetaData(metaData);
          }
          break;
        }
    }
    return true;
  }

  public void OnTaskListEnded(PowerTaskList taskList)
  {
    if (taskList == null)
      return;
    foreach (PowerTask task in taskList.GetTaskList())
    {
      if (task.GetPower().Type == Network.PowerType.CREATE_GAME)
      {
        this.m_createGamePhase = GameState.CreateGamePhase.CREATED;
        this.FireCreateGameEvent();
        this.m_createGameListeners.Clear();
      }
    }
    this.RemoveQueuedEntitiesFromGame();
  }

  public void OnPowerHistory(List<Network.PowerHistory> powerList)
  {
    this.DebugPrintPowerList(powerList);
    int num = this.m_powerProcessor.HasEarlyConcedeTaskList() ? 1 : 0;
    this.m_powerProcessor.OnPowerHistory(powerList);
    this.ProcessAllQueuedChoices();
    bool flag = this.m_powerProcessor.HasEarlyConcedeTaskList();
    if (!(num == 0 & flag))
      return;
    this.OnReceivedEarlyConcede();
  }

  private void OnReceivedEarlyConcede()
  {
    this.ClearResponseMode();
    this.ClearLastOptions();
    this.ClearOptions();
  }

  public void OnAllOptions(Network.Options options)
  {
    this.m_responseMode = GameState.ResponseMode.OPTION;
    this.m_chosenEntities.Clear();
    if (this.m_options != null && (this.m_lastOptions == null || this.m_lastOptions.ID < this.m_options.ID))
    {
      this.m_lastOptions = new Network.Options();
      this.m_lastOptions.CopyFrom(this.m_options);
    }
    this.m_options = options;
    foreach (Network.Options.Option option in this.m_options.List)
    {
      if (option.Type == Network.Options.Option.OptionType.POWER)
      {
        Entity entity = this.GetEntity(option.Main.ID);
        if (entity != null && option.Main.Targets != null && option.Main.Targets.Count > 0)
          entity.UpdateUseBattlecryFlag(true);
      }
    }
    this.DebugPrintOptions(Log.Power);
    this.EnterMainOptionMode();
    this.FireOptionsReceivedEvent();
  }

  public void OnEntityChoices(Network.EntityChoices choices)
  {
    PowerTaskList lastTaskList = this.m_powerProcessor.GetLastTaskList();
    if (!this.CanProcessEntityChoices(choices))
    {
      Log.Power.Print("GameState.OnEntityChoices() - id={0} playerId={1} queued", (object) choices.ID, (object) choices.PlayerId);
      this.m_queuedChoices.Enqueue(new GameState.QueuedChoice()
      {
        m_type = GameState.QueuedChoice.PacketType.ENTITY_CHOICES,
        m_packet = (object) choices,
        m_eventData = (object) lastTaskList
      });
    }
    else
      this.ProcessEntityChoices(choices, lastTaskList);
  }

  public void OnEntitiesChosen(Network.EntitiesChosen chosen)
  {
    if (!this.CanProcessEntitiesChosen(chosen))
    {
      Log.Power.Print("GameState.OnEntitiesChosen() - id={0} playerId={1} queued", (object) chosen.ID, (object) chosen.PlayerId);
      this.m_queuedChoices.Enqueue(new GameState.QueuedChoice()
      {
        m_type = GameState.QueuedChoice.PacketType.ENTITIES_CHOSEN,
        m_packet = (object) chosen
      });
    }
    else
      this.ProcessEntitiesChosen(chosen);
  }

  public float GetClientLostTimeCatchUpThreshold() => this.m_clientLostTimeCatchUpThreshold;

  public bool ShouldUseSlushTimeTracker() => this.m_useSlushTimeCatchUp;

  public bool ShoudRestrictLostTimeCatchUpToLowEndDevices() => this.m_restrictClientLostTimeCatchUpToLowEndDevices;

  public void SetBattlegroundAllowBuddies(bool value)
  {
    int num = this.m_battlegroundAllowBuddies != value ? 1 : 0;
    this.m_battlegroundAllowBuddies = value;
    if (num == 0)
      return;
    PlayerLeaderboardManager.Get()?.NotifyBattlegroundHeroBuddyEnabledDirty();
  }

  public void SetBattlegroundsAllowQuestRewards(bool value)
  {
    if (this.m_battlegroundsAllowQuestRewards == value)
      return;
    this.m_battlegroundsAllowQuestRewards = value;
    PlayerLeaderboardManager.Get()?.NotifyBattlegroundsQuestRewardEnabledDirty();
  }

  public void UpdateGameGuardianVars(GameGuardianVars gameGuardianVars)
  {
    this.m_clientLostTimeCatchUpThreshold = gameGuardianVars.HasClientLostFrameTimeCatchUpThreshold ? gameGuardianVars.ClientLostFrameTimeCatchUpThreshold : 0.0f;
    this.m_useSlushTimeCatchUp = gameGuardianVars.HasClientLostFrameTimeCatchUpUseSlush && gameGuardianVars.ClientLostFrameTimeCatchUpUseSlush;
    this.m_restrictClientLostTimeCatchUpToLowEndDevices = gameGuardianVars.HasClientLostFrameTimeCatchUpLowEndOnly && gameGuardianVars.ClientLostFrameTimeCatchUpLowEndOnly;
    this.m_allowDeferredPowers = !gameGuardianVars.HasGameAllowDeferredPowers || gameGuardianVars.GameAllowDeferredPowers;
    this.m_allowBatchedPowers = !gameGuardianVars.HasGameAllowBatchedPowers || gameGuardianVars.GameAllowBatchedPowers;
    this.m_allowDiamondCards = !gameGuardianVars.HasGameAllowDiamondCards || gameGuardianVars.GameAllowDiamondCards;
    this.m_allowSignatureCards = !gameGuardianVars.HasGameAllowSignatureCards || gameGuardianVars.GameAllowSignatureCards;
    this.SetBattlegroundAllowBuddies(!gameGuardianVars.HasBattlegroundAllowBuddies || gameGuardianVars.BattlegroundAllowBuddies);
    this.m_mercenariesUseBonesForBigCard = !gameGuardianVars.HasGameMercenariesAllowBigCardBones || gameGuardianVars.GameMercenariesAllowBigCardBones;
    this.SetBattlegroundsAllowQuestRewards(!gameGuardianVars.HasBattlegroundsAllowQuestRewards || gameGuardianVars.BattlegroundsAllowQuestRewards);
  }

  public void UpdateBattlegroundInfo(
    PegasusGame.UpdateBattlegroundInfo battlegroundMinionPoolDenyList)
  {
    this.m_battlegroundMinionPool = battlegroundMinionPoolDenyList.HasBattlegroundMinionPool ? battlegroundMinionPoolDenyList.BattlegroundMinionPool : "Battleground minion pool not available";
    if (this.m_printBattlegroundMinionPoolOnUpdate)
    {
      Log.All.Print(this.m_battlegroundMinionPool);
      this.m_printBattlegroundMinionPoolOnUpdate = false;
    }
    this.m_battlegroundDenyList = battlegroundMinionPoolDenyList.HasBattlegroundDenyList ? battlegroundMinionPoolDenyList.BattlegroundDenyList : "Battle ground deny list not available";
    if (!this.m_printBattlegroundDenyListOnUpdate)
      return;
    Log.All.Print(this.m_battlegroundDenyList);
    this.m_printBattlegroundDenyListOnUpdate = false;
  }

  public void UpdateBattlegroundArmorTierList(
    GetBattlegroundHeroArmorTierList battlegroundHeroArmorTierList)
  {
    this.m_battlegroundHeroArmorTierList = battlegroundHeroArmorTierList.HasBattlegroundHeroArmorTierList ? battlegroundHeroArmorTierList.BattlegroundHeroArmorTierList : "Battle ground hero armor tier list not available";
    if (!this.m_printBattlegroundHeroArmorTierListUpdate)
      return;
    Log.All.Print(this.m_battlegroundHeroArmorTierList);
    this.m_printBattlegroundHeroArmorTierListUpdate = false;
  }

  private bool CanProcessEntityChoices(Network.EntityChoices choices)
  {
    int playerId = choices.PlayerId;
    if (!this.m_playerMap.ContainsKey(playerId))
      return false;
    foreach (int entity in choices.Entities)
    {
      if (!this.m_entityMap.ContainsKey(entity))
        return false;
    }
    return !this.m_choicesMap.ContainsKey(playerId);
  }

  private bool CanProcessEntitiesChosen(Network.EntitiesChosen chosen)
  {
    int playerId = chosen.PlayerId;
    if (!this.m_playerMap.ContainsKey(playerId))
      return false;
    foreach (int entity in chosen.Entities)
    {
      if (!this.m_entityMap.ContainsKey(entity))
        return false;
    }
    Network.EntityChoices entityChoices;
    return !this.m_choicesMap.TryGetValue(playerId, out entityChoices) || entityChoices.ID == chosen.ID;
  }

  private void ProcessAllQueuedChoices()
  {
    while (this.m_queuedChoices.Count > 0)
    {
      GameState.QueuedChoice queuedChoice = this.m_queuedChoices.Peek();
      switch (queuedChoice.m_type)
      {
        case GameState.QueuedChoice.PacketType.ENTITY_CHOICES:
          Network.EntityChoices packet1 = (Network.EntityChoices) queuedChoice.m_packet;
          if (!this.CanProcessEntityChoices(packet1))
            return;
          this.m_queuedChoices.Dequeue();
          PowerTaskList eventData = (PowerTaskList) queuedChoice.m_eventData;
          this.ProcessEntityChoices(packet1, eventData);
          continue;
        case GameState.QueuedChoice.PacketType.ENTITIES_CHOSEN:
          Network.EntitiesChosen packet2 = (Network.EntitiesChosen) queuedChoice.m_packet;
          if (!this.CanProcessEntitiesChosen(packet2))
            return;
          this.m_queuedChoices.Dequeue();
          this.ProcessEntitiesChosen(packet2);
          continue;
        default:
          continue;
      }
    }
  }

  private void ProcessEntityChoices(Network.EntityChoices choices, PowerTaskList preChoiceTaskList)
  {
    this.DebugPrintEntityChoices(choices, preChoiceTaskList);
    if (this.m_powerProcessor.HasEarlyConcedeTaskList())
      return;
    int playerId = choices.PlayerId;
    this.m_choicesMap[playerId] = choices;
    int friendlyPlayerId = this.GetFriendlyPlayerId();
    if (playerId == friendlyPlayerId)
    {
      this.m_responseMode = GameState.ResponseMode.CHOICE;
      this.m_chosenEntities.Clear();
      this.EnterChoiceMode();
    }
    this.FireEntityChoicesReceivedEvent(choices, preChoiceTaskList);
  }

  private void ProcessEntitiesChosen(Network.EntitiesChosen chosen)
  {
    this.DebugPrintEntitiesChosen(chosen);
    if (this.m_powerProcessor.HasEarlyConcedeTaskList() || this.FireEntitiesChosenReceivedEvent(chosen))
      return;
    this.OnEntitiesChosenProcessed(chosen);
  }

  public void OnGameSetup(Network.GameSetup setup)
  {
    this.m_maxSecretZoneSizePerPlayer = setup.MaxSecretZoneSizePerPlayer;
    this.m_maxSecretsPerPlayer = setup.MaxSecretsPerPlayer;
    this.m_maxQuestsPerPlayer = setup.MaxQuestsPerPlayer;
    this.m_maxFriendlySlotsPerPlayer = setup.MaxFriendlyMinionsPerPlayer;
  }

  public void QueueEntityForRemoval(Entity entity) => this.m_removedFromGameEntities.Enqueue(entity);

  public void OnOptionRejected(int optionId)
  {
    if (this.m_lastSelectedOption == null)
      UnityEngine.Debug.LogError((object) "GameState.OnOptionRejected() - got an option rejection without a last selected option");
    else if (this.m_lastOptions.ID != optionId)
    {
      UnityEngine.Debug.LogErrorFormat("GameState.OnOptionRejected() - rejected option id ({0}) does not match last option id ({1})", (object) optionId, (object) this.m_lastOptions.ID);
    }
    else
    {
      this.FireOptionRejectedEvent(this.m_lastOptions.List[this.m_lastSelectedOption.m_main]);
      this.ClearLastOptions();
    }
  }

  public void OnTurnTimerUpdate(Network.TurnTimerInfo info)
  {
    TurnTimerUpdate update = new TurnTimerUpdate();
    update.SetSecondsRemaining(info.Seconds);
    update.SetEndTimestamp(Time.realtimeSinceStartup + info.Seconds);
    update.SetShow(info.Show);
    if (this.IsMulliganManagerActive() && this.m_gameEntity != null && this.GetBooleanGameOption(GameEntityOption.ALWAYS_SHOW_MULLIGAN_TIMER))
      update.SetShow(true);
    int turn = this.GetTurn();
    if (info.Turn > turn)
      this.m_turnTimerUpdates[info.Turn] = update;
    else
      this.TriggerTurnTimerUpdate(update);
  }

  public void TriggerTurnTimerUpdateForTurn(int turn) => this.OnTurnChanged_TurnTimer(this.GetTurn(), turn);

  public void OnSpectatorNotifyEvent(SpectatorNotify notify) => this.FireSpectatorNotifyEvent(notify);

  public void SendChoices()
  {
    if (this.m_responseMode != GameState.ResponseMode.CHOICE)
      return;
    Network.EntityChoices friendlyEntityChoices = this.GetFriendlyEntityChoices();
    if (friendlyEntityChoices == null || this.m_chosenEntities.Count < friendlyEntityChoices.CountMin || this.m_chosenEntities.Count > friendlyEntityChoices.CountMax)
      return;
    ChoiceCardMgr.Get().OnSendChoices(friendlyEntityChoices, this.m_chosenEntities);
    Log.Power.Print("GameState.SendChoices() - id={0} ChoiceType={1}", (object) friendlyEntityChoices.ID, (object) friendlyEntityChoices.ChoiceType);
    List<int> picks = new List<int>();
    for (int index = 0; index < this.m_chosenEntities.Count; ++index)
    {
      Entity chosenEntity = this.m_chosenEntities[index];
      int entityId = chosenEntity.GetEntityId();
      Log.Power.Print("GameState.SendChoices() -   m_chosenEntities[{0}]={1}", (object) index, (object) chosenEntity);
      picks.Add(entityId);
    }
    if (!GameMgr.Get().IsSpectator())
      Network.Get().SendChoices(friendlyEntityChoices.ID, picks);
    this.ClearResponseMode();
  }

  public void OnEntitiesChosenProcessed(Network.EntitiesChosen chosen)
  {
    int playerId = chosen.PlayerId;
    int friendlyPlayerId = this.GetFriendlyPlayerId();
    if (playerId == friendlyPlayerId)
    {
      if (this.m_responseMode == GameState.ResponseMode.CHOICE)
        this.ClearResponseMode();
      this.ClearFriendlyChoices();
    }
    else
      this.m_choicesMap.Remove(playerId);
    this.ProcessAllQueuedChoices();
  }

  public void SendOption()
  {
    if (!GameMgr.Get().IsSpectator())
    {
      Network.Get().SendOption(this.m_options.ID, this.m_selectedOption.m_main, this.m_selectedOption.m_target, this.m_selectedOption.m_sub, this.m_selectedOption.m_position);
      Log.Power.Print("GameState.SendOption() - selectedOption={0} selectedSubOption={1} selectedTarget={2} selectedPosition={3}", (object) this.m_selectedOption.m_main, (object) this.m_selectedOption.m_sub, (object) this.m_selectedOption.m_target, (object) this.m_selectedOption.m_position);
    }
    this.OnSelectedOptionsSent();
    this.FireOptionsSentEvent(this.m_lastOptions.List[this.m_lastSelectedOption.m_main]);
  }

  private void OnTurnChanged_TurnTimer(int oldTurn, int newTurn)
  {
    TurnTimerUpdate update;
    if (this.m_turnTimerUpdates.Count == 0 || !this.m_turnTimerUpdates.TryGetValue(newTurn, out update))
      return;
    float realtimeSinceStartup = Time.realtimeSinceStartup;
    float sec = Mathf.Max(0.0f, update.GetEndTimestamp() - realtimeSinceStartup);
    update.SetSecondsRemaining(sec);
    this.TriggerTurnTimerUpdate(update);
    this.m_turnTimerUpdates.Remove(newTurn);
  }

  private void TriggerTurnTimerUpdate(TurnTimerUpdate update)
  {
    this.FireTurnTimerUpdateEvent(update);
    if ((double) update.GetSecondsRemaining() > (double) Mathf.Epsilon)
      return;
    this.OnTimeout();
  }

  private void DebugPrintGame()
  {
    if (!Log.Power.CanPrint())
      return;
    Log.Power.Print(string.Format("GameState.DebugPrintGame() - BuildNumber={0}", (object) 158725));
    Log.Power.Print(string.Format("GameState.DebugPrintGame() - GameType={0}", (object) GameMgr.Get().GetGameType()));
    Log.Power.Print(string.Format("GameState.DebugPrintGame() - FormatType={0}", (object) GameMgr.Get().GetFormatType()));
    Log.Power.Print(string.Format("GameState.DebugPrintGame() - ScenarioID={0}", (object) GameMgr.Get().GetMissionId()));
    foreach (Player player in this.m_playerMap.Values)
      Log.Power.Print(string.Format("GameState.DebugPrintGame() - PlayerID={0}, PlayerName={1}", (object) player.GetPlayerId(), (object) this.GetEntityLogName(player.GetEntityId())));
  }

  private void DebugPrintPowerList(List<Network.PowerHistory> powerList)
  {
    if (!Log.Power.CanPrint())
      return;
    string indentation = "";
    Log.Power.Print(string.Format("GameState.DebugPrintPowerList() - Count={0}", (object) powerList.Count));
    for (int index = 0; index < powerList.Count; ++index)
      this.DebugPrintPower(Log.Power, nameof (GameState), powerList[index], ref indentation);
  }

  public void DebugPrintPower(Logger logger, string callerName, Network.PowerHistory power)
  {
    string empty = string.Empty;
    this.DebugPrintPower(logger, callerName, power, ref empty);
  }

  public void DebugPrintPower(
    Logger logger,
    string callerName,
    Network.PowerHistory power,
    ref string indentation)
  {
    if (!Log.Power.CanPrint())
      return;
    switch (power.Type)
    {
      case Network.PowerType.FULL_ENTITY:
        Network.Entity entity1 = ((Network.HistFullEntity) power).Entity;
        Entity entity2 = this.GetEntity(entity1.ID);
        if (entity2 == null)
          logger.Print("{0}.DebugPrintPower() - {1}FULL_ENTITY - Creating ID={2} CardID={3}", (object) callerName, (object) indentation, (object) entity1.ID, (object) entity1.CardID);
        else
          logger.Print("{0}.DebugPrintPower() - {1}FULL_ENTITY - Updating {2} CardID={3}", (object) callerName, (object) indentation, (object) entity2, (object) entity1.CardID);
        this.DebugPrintTags(logger, callerName, indentation, entity1);
        break;
      case Network.PowerType.SHOW_ENTITY:
        Network.Entity entity3 = ((Network.HistShowEntity) power).Entity;
        logger.Print("{0}.DebugPrintPower() - {1}SHOW_ENTITY - Updating Entity={2} CardID={3}", (object) callerName, (object) indentation, (object) this.GetEntityLogName(entity3.ID), (object) entity3.CardID);
        this.DebugPrintTags(logger, callerName, indentation, entity3);
        break;
      case Network.PowerType.HIDE_ENTITY:
        Network.HistHideEntity histHideEntity = (Network.HistHideEntity) power;
        logger.Print("{0}.DebugPrintPower() - {1}HIDE_ENTITY - Entity={2} {3}", (object) callerName, (object) indentation, (object) this.GetEntityLogName(histHideEntity.Entity), (object) Tags.DebugTag(49, histHideEntity.Zone));
        break;
      case Network.PowerType.TAG_CHANGE:
        Network.HistTagChange histTagChange = (Network.HistTagChange) power;
        logger.Print("{0}.DebugPrintPower() - {1}TAG_CHANGE Entity={2} {3} {4}", (object) callerName, (object) indentation, (object) this.GetEntityLogName(histTagChange.Entity), (object) Tags.DebugTag(histTagChange.Tag, histTagChange.Value), histTagChange.ChangeDef ? (object) "DEF CHANGE" : (object) "");
        break;
      case Network.PowerType.BLOCK_START:
        Network.HistBlockStart histBlockStart = (Network.HistBlockStart) power;
        string str = string.Empty;
        if (histBlockStart.BlockType == HistoryBlock.Type.TRIGGER)
          str = string.Format("TriggerKeyword={0}", (object) ((GAME_TAG) histBlockStart.TriggerKeyword).ToString());
        logger.Print("{0}.DebugPrintPower() - {1}BLOCK_START BlockType={2} Entity={3} EffectCardId={4} EffectIndex={5} Target={6} SubOption={7} {8}", (object) callerName, (object) indentation, (object) histBlockStart.BlockType, (object) this.GetEntitiesLogNames(histBlockStart.Entities), (object) histBlockStart.EffectCardId, (object) histBlockStart.EffectIndex, (object) this.GetEntityLogName(histBlockStart.Target), (object) histBlockStart.SubOption, (object) str);
        indentation += "    ";
        break;
      case Network.PowerType.BLOCK_END:
        if (indentation.Length >= "    ".Length)
          indentation = indentation.Remove(indentation.Length - "    ".Length);
        logger.Print("{0}.DebugPrintPower() - {1}BLOCK_END", (object) callerName, (object) indentation);
        break;
      case Network.PowerType.CREATE_GAME:
        Network.HistCreateGame histCreateGame = (Network.HistCreateGame) power;
        logger.Print("{0}.DebugPrintPower() - {1}CREATE_GAME", (object) callerName, (object) indentation);
        indentation += "    ";
        logger.Print("{0}.DebugPrintPower() - {1}GameEntity EntityID={2}", (object) callerName, (object) indentation, (object) histCreateGame.Game.ID);
        this.DebugPrintTags(logger, callerName, indentation, histCreateGame.Game);
        foreach (Network.HistCreateGame.PlayerData player in histCreateGame.Players)
        {
          logger.Print("{0}.DebugPrintPower() - {1}Player EntityID={2} PlayerID={3} GameAccountId={4}", (object) callerName, (object) indentation, (object) player.Player.ID, (object) player.ID, (object) player.GameAccountId);
          this.DebugPrintTags(logger, callerName, indentation, player.Player);
        }
        indentation = indentation.Remove(indentation.Length - "    ".Length);
        break;
      case Network.PowerType.META_DATA:
        Network.HistMetaData histMetaData = (Network.HistMetaData) power;
        string entityLogName = histMetaData.Data.ToString();
        if (histMetaData.MetaType == HistoryMeta.Type.JOUST)
          entityLogName = this.GetEntityLogName(histMetaData.Data);
        logger.Print("{0}.DebugPrintPower() - {1}META_DATA - Meta={2} Data={3} InfoCount={4}", (object) callerName, (object) indentation, (object) histMetaData.MetaType, (object) entityLogName, (object) histMetaData.Info.Count);
        if (histMetaData.Info.Count <= 0 || !logger.IsVerbose())
          break;
        indentation += "    ";
        for (int index = 0; index < histMetaData.Info.Count; ++index)
        {
          int id = histMetaData.Info[index];
          logger.Print(true, "{0}.DebugPrintPower() - {1}        Info[{2}] = {3}", (object) callerName, (object) indentation, (object) index, (object) this.GetEntityLogName(id));
        }
        indentation = indentation.Remove(indentation.Length - "    ".Length);
        break;
      case Network.PowerType.CHANGE_ENTITY:
        Network.Entity entity4 = ((Network.HistChangeEntity) power).Entity;
        logger.Print("{0}.DebugPrintPower() - {1}CHANGE_ENTITY - Updating Entity={2} CardID={3}", (object) callerName, (object) indentation, (object) this.GetEntityLogName(entity4.ID), (object) entity4.CardID);
        this.DebugPrintTags(logger, callerName, indentation, entity4);
        break;
      case Network.PowerType.RESET_GAME:
        logger.Print("{0}.DebugPrintPower() - {1}RESET_GAME", (object) callerName, (object) indentation);
        break;
      case Network.PowerType.SUB_SPELL_START:
        Network.HistSubSpellStart histSubSpellStart = power as Network.HistSubSpellStart;
        logger.Print("{0}.DebugPrintPower() - {1}SUB_SPELL_START - SpellPrefabGUID={2} Source={3} TargetCount={4}", (object) callerName, (object) indentation, (object) histSubSpellStart.SpellPrefabGUID, (object) histSubSpellStart.SourceEntityID, (object) histSubSpellStart.TargetEntityIDS.Count);
        if (logger.IsVerbose())
        {
          if (histSubSpellStart.SourceEntityID != 0)
            logger.Print(true, "{0}.DebugPrintPower() - {1}                  Source = {2}", (object) callerName, (object) indentation, (object) this.GetEntityLogName(histSubSpellStart.SourceEntityID));
          for (int index = 0; index < histSubSpellStart.TargetEntityIDS.Count; ++index)
          {
            int id = histSubSpellStart.TargetEntityIDS[index];
            logger.Print(true, "{0}.DebugPrintPower() - {1}                  Targets[{2}] = {3}", (object) callerName, (object) indentation, (object) index, (object) this.GetEntityLogName(id));
          }
        }
        indentation += "    ";
        break;
      case Network.PowerType.SUB_SPELL_END:
        if (indentation.Length >= "    ".Length)
          indentation = indentation.Remove(indentation.Length - "    ".Length);
        logger.Print("{0}.DebugPrintPower() - {1}SUB_SPELL_END", (object) callerName, (object) indentation);
        break;
      case Network.PowerType.VO_SPELL:
        Network.HistVoSpell histVoSpell = power as Network.HistVoSpell;
        logger.Print("{0}.DebugPrintPower() - {1}VO_SPELL - BrassRingGuid={2} - VoSpellPrefabGUID={3} - Blocking={4} - AdditionalDelayInMs={5}", (object) callerName, (object) indentation, (object) histVoSpell.SpellPrefabGUID, (object) histVoSpell.BrassRingGUID, (object) histVoSpell.Blocking, (object) histVoSpell.AdditionalDelayMs);
        break;
      case Network.PowerType.CACHED_TAG_FOR_DORMANT_CHANGE:
        Network.HistCachedTagForDormantChange forDormantChange = (Network.HistCachedTagForDormantChange) power;
        logger.Print("{0}.DebugPrintPower() - {1}CACHED_TAG_FOR_DORMANT_CHANGE Entity={2} {3}", (object) callerName, (object) indentation, (object) this.GetEntityLogName(forDormantChange.Entity), (object) Tags.DebugTag(forDormantChange.Tag, forDormantChange.Value));
        break;
      case Network.PowerType.SHUFFLE_DECK:
        Network.HistShuffleDeck histShuffleDeck = (Network.HistShuffleDeck) power;
        logger.Print("{0}.DebugPrintPower() - {1}SHUFFLE_DECK PlayerID={2}", (object) callerName, (object) indentation, (object) histShuffleDeck.PlayerID);
        break;
      default:
        logger.Print("{0}.DebugPrintPower() - ERROR: unhandled PowType {1}", (object) callerName, (object) power.Type);
        break;
    }
  }

  private void DebugPrintTags(
    Logger logger,
    string callerName,
    string indentation,
    Network.Entity netEntity)
  {
    if (!Log.Power.CanPrint())
      return;
    if (indentation != null)
      indentation += "    ";
    for (int index = 0; index < netEntity.Tags.Count; ++index)
    {
      Network.Entity.Tag tag = netEntity.Tags[index];
      logger.Print("{0}.DebugPrintPower() - {1}{2}", (object) callerName, (object) indentation, (object) Tags.DebugTag(tag.Name, tag.Value));
    }
  }

  private void DebugPrintOptions(Logger logger)
  {
    if (!logger.CanPrint())
      return;
    logger.Print("GameState.DebugPrintOptions() - id={0}", (object) this.m_options.ID);
    for (int index1 = 0; index1 < this.m_options.List.Count; ++index1)
    {
      Network.Options.Option option = this.m_options.List[index1];
      Entity entity1 = this.GetEntity(option.Main.ID);
      logger.Print("GameState.DebugPrintOptions() -   option {0} type={1} mainEntity={2} error={3} errorParam={4}", (object) index1, (object) option.Type, (object) entity1, (object) option.Main.PlayErrorInfo.PlayError, (object) option.Main.PlayErrorInfo.PlayErrorParam);
      if (option.Main.Targets != null)
      {
        for (int index2 = 0; index2 < option.Main.Targets.Count; ++index2)
        {
          Network.Options.Option.TargetOption target = option.Main.Targets[index2];
          Entity entity2 = this.GetEntity(target.ID);
          logger.Print("GameState.DebugPrintOptions() -     target {0} entity={1} error={2} errorParam={3}", (object) index2, (object) entity2, (object) target.PlayErrorInfo.PlayError, (object) target.PlayErrorInfo.PlayErrorParam);
        }
      }
      for (int index3 = 0; index3 < option.Subs.Count; ++index3)
      {
        Network.Options.Option.SubOption sub = option.Subs[index3];
        Entity entity3 = this.GetEntity(sub.ID);
        logger.Print("GameState.DebugPrintOptions() -     subOption {0} entity={1} error={2} errorParam={3}", (object) index3, (object) entity3, (object) sub.PlayErrorInfo.PlayError, (object) sub.PlayErrorInfo.PlayErrorParam);
        if (sub.Targets != null)
        {
          for (int index4 = 0; index4 < sub.Targets.Count; ++index4)
          {
            Network.Options.Option.TargetOption target = sub.Targets[index4];
            Entity entity4 = this.GetEntity(target.ID);
            logger.Print("GameState.DebugPrintOptions() -       target {0} entity={1} error={2} errorParam={3}", (object) index4, (object) entity4, (object) target.PlayErrorInfo.PlayError, (object) target.PlayErrorInfo.PlayErrorParam);
          }
        }
      }
    }
  }

  private void DebugPrintEntityChoices(
    Network.EntityChoices choices,
    PowerTaskList preChoiceTaskList)
  {
    if (!Log.Power.CanPrint())
      return;
    Player player = this.GetPlayer(choices.PlayerId);
    object obj = (object) null;
    if (preChoiceTaskList != null)
      obj = (object) preChoiceTaskList.GetId();
    Log.Power.Print("GameState.DebugPrintEntityChoices() - id={0} Player={1} TaskList={2} ChoiceType={3} CountMin={4} CountMax={5}", (object) choices.ID, (object) this.GetEntityLogName(player.GetEntityId()), obj, (object) choices.ChoiceType, (object) choices.CountMin, (object) choices.CountMax);
    Log.Power.Print("GameState.DebugPrintEntityChoices() -   Source={0}", (object) this.GetEntityLogName(choices.Source));
    for (int index = 0; index < choices.Entities.Count; ++index)
      Log.Power.Print("GameState.DebugPrintEntityChoices() -   Entities[{0}]={1}", (object) index, (object) this.GetEntityLogName(choices.Entities[index]));
  }

  private void DebugPrintEntitiesChosen(Network.EntitiesChosen chosen)
  {
    if (!Log.Power.CanPrint())
      return;
    Player player = this.GetPlayer(chosen.PlayerId);
    Log.Power.Print("GameState.DebugPrintEntitiesChosen() - id={0} Player={1} EntitiesCount={2}", (object) chosen.ID, (object) this.GetEntityLogName(player.GetEntityId()), (object) chosen.Entities.Count);
    for (int index = 0; index < chosen.Entities.Count; ++index)
      Log.Power.Print("GameState.DebugPrintEntitiesChosen() -   Entities[{0}]={1}", (object) index, (object) this.GetEntityLogName(chosen.Entities[index]));
  }

  private string GetEntityLogName(int id)
  {
    Entity entity = this.GetEntity(id);
    if (entity == null)
      return id.ToString();
    if (entity.IsPlayer())
    {
      BnetPlayer bnetPlayer = (entity as Player).GetBnetPlayer();
      if (bnetPlayer != null && bnetPlayer.GetBattleTag() != (BnetBattleTag) null)
        return string.Format("{0}#{1}", (object) bnetPlayer.GetBattleTag().GetName(), (object) bnetPlayer.GetBattleTag().GetNumber());
    }
    return entity.ToString();
  }

  private string GetEntitiesLogNames(List<int> ids)
  {
    StringBuilder stringBuilder = new StringBuilder();
    foreach (int id in ids)
    {
      if (stringBuilder.Length > 0)
        stringBuilder.Append(",");
      stringBuilder.Append(this.GetEntityLogName(id));
    }
    return stringBuilder.ToString();
  }

  private void PrintBlockingTaskList(StringBuilder builder, PowerTaskList taskList)
  {
    if (taskList == null)
    {
      builder.Append("null");
    }
    else
    {
      builder.AppendFormat("ID={0} ", (object) taskList.GetId());
      builder.Append("Source=[");
      Network.HistBlockStart blockStart = taskList.GetBlockStart();
      if (blockStart == null)
      {
        builder.Append("null");
      }
      else
      {
        builder.AppendFormat("BlockType={0}", (object) blockStart.BlockType);
        builder.Append(' ');
        builder.AppendFormat("Entities={0}", (object) this.GetEntitiesLogNames(blockStart.Entities));
        builder.Append(' ');
        builder.AppendFormat("Target={0}", (object) this.GetEntityLogName(blockStart.Target));
      }
      builder.Append(']');
      builder.AppendFormat(" Tasks={0}", (object) taskList.GetTaskList().Count);
    }
  }

  private void QuickGameFlipHeroesCheat(List<Network.PowerHistory> powerList)
  {
  }

  public enum ResponseMode
  {
    NONE,
    OPTION,
    SUB_OPTION,
    OPTION_TARGET,
    CHOICE,
  }

  public enum CreateGamePhase
  {
    INVALID,
    CREATING,
    CREATED,
  }

  public delegate void GameStateInitializedCallback(GameState instance, object userData);

  public delegate void CreateGameCallback(GameState.CreateGamePhase phase, object userData);

  public delegate void OptionsReceivedCallback(object userData);

  public delegate void OptionsSentCallback(Network.Options.Option option, object userData);

  public delegate void OptionRejectedCallback(Network.Options.Option option, object userData);

  public delegate void EntityChoicesReceivedCallback(
    Network.EntityChoices choices,
    PowerTaskList preChoiceTaskList,
    object userData);

  public delegate bool EntitiesChosenReceivedCallback(
    Network.EntitiesChosen chosen,
    object userData);

  public delegate void CurrentPlayerChangedCallback(Player player, object userData);

  public delegate void TurnChangedCallback(int oldTurn, int newTurn, object userData);

  public delegate void FriendlyTurnStartedCallback(object userData);

  public delegate void TurnTimerUpdateCallback(TurnTimerUpdate update, object userData);

  public delegate void SpectatorNotifyEventCallback(SpectatorNotify notify, object userData);

  public delegate void GameOverCallback(TAG_PLAYSTATE playState, object userData);

  public delegate void HeroChangedCallback(Player player, object userData);

  public delegate void BusyStateChangedCallback(bool isBusy, object userData);

  public delegate void CantPlayCallback(Entity entity, object userData);

  public delegate void DamageCapChangedCallback(int oldValue, int newValue, object userData);

  public delegate void DiabloFightPlayerIDChangedCallback(
    int oldValue,
    int newValue,
    object userData);

  private delegate void AppendBlockingServerItemCallback<T>(StringBuilder builder, T item);

  private class SelectedOption
  {
    public int m_main = -1;
    public int m_sub = -1;
    public int m_target;
    public int m_position;

    public void Clear()
    {
      this.m_main = -1;
      this.m_sub = -1;
      this.m_target = 0;
      this.m_position = 0;
    }

    public void CopyFrom(GameState.SelectedOption original)
    {
      this.m_main = original.m_main;
      this.m_sub = original.m_sub;
      this.m_target = original.m_target;
      this.m_position = original.m_position;
    }
  }

  private class QueuedChoice
  {
    public GameState.QueuedChoice.PacketType m_type;
    public object m_packet;
    public object m_eventData;

    public enum PacketType
    {
      ENTITY_CHOICES,
      ENTITIES_CHOSEN,
    }
  }

  private class GameStateInitializedListener : EventListener<GameState.GameStateInitializedCallback>
  {
    public void Fire(GameState instance) => this.m_callback(instance, this.m_userData);
  }

  private class CreateGameListener : EventListener<GameState.CreateGameCallback>
  {
    public void Fire(GameState.CreateGamePhase phase) => this.m_callback(phase, this.m_userData);
  }

  private class OptionsReceivedListener : EventListener<GameState.OptionsReceivedCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }

  private class OptionsSentListener : EventListener<GameState.OptionsSentCallback>
  {
    public void Fire(Network.Options.Option option) => this.m_callback(option, this.m_userData);
  }

  private class OptionRejectedListener : EventListener<GameState.OptionRejectedCallback>
  {
    public void Fire(Network.Options.Option option) => this.m_callback(option, this.m_userData);
  }

  private class EntityChoicesReceivedListener : 
    EventListener<GameState.EntityChoicesReceivedCallback>
  {
    public void Fire(Network.EntityChoices choices, PowerTaskList preChoiceTaskList) => this.m_callback(choices, preChoiceTaskList, this.m_userData);
  }

  private class EntitiesChosenReceivedListener : 
    EventListener<GameState.EntitiesChosenReceivedCallback>
  {
    public bool Fire(Network.EntitiesChosen chosen) => this.m_callback(chosen, this.m_userData);
  }

  private class CurrentPlayerChangedListener : EventListener<GameState.CurrentPlayerChangedCallback>
  {
    public void Fire(Player player) => this.m_callback(player, this.m_userData);
  }

  private class TurnChangedListener : EventListener<GameState.TurnChangedCallback>
  {
    public void Fire(int oldTurn, int newTurn) => this.m_callback(oldTurn, newTurn, this.m_userData);
  }

  private class FriendlyTurnStartedListener : EventListener<GameState.FriendlyTurnStartedCallback>
  {
    public void Fire() => this.m_callback(this.m_userData);
  }

  private class TurnTimerUpdateListener : EventListener<GameState.TurnTimerUpdateCallback>
  {
    public void Fire(TurnTimerUpdate update) => this.m_callback(update, this.m_userData);
  }

  private class SpectatorNotifyListener : EventListener<GameState.SpectatorNotifyEventCallback>
  {
    public void Fire(SpectatorNotify notify) => this.m_callback(notify, this.m_userData);
  }

  private class GameOverListener : EventListener<GameState.GameOverCallback>
  {
    public void Fire(TAG_PLAYSTATE playState) => this.m_callback(playState, this.m_userData);
  }

  private class HeroChangedListener : EventListener<GameState.HeroChangedCallback>
  {
    public void Fire(Player player) => this.m_callback(player, this.m_userData);
  }

  private class BusyStateChangedListener : EventListener<GameState.BusyStateChangedCallback>
  {
    public void Fire(bool isBusy) => this.m_callback(isBusy, this.m_userData);
  }

  private class CantPlayListener : EventListener<GameState.CantPlayCallback>
  {
    public void Fire(Entity entity) => this.m_callback(entity, this.m_userData);
  }

  private class DamageCapChangedListener : EventListener<GameState.DamageCapChangedCallback>
  {
    public void Fire(int oldValue, int newValue) => this.m_callback(oldValue, newValue, this.m_userData);
  }

  private class DiabloFightPlayerIDChangedListener : 
    EventListener<GameState.DiabloFightPlayerIDChangedCallback>
  {
    public void Fire(int oldValue, int newValue) => this.m_callback(oldValue, newValue, this.m_userData);
  }
}
