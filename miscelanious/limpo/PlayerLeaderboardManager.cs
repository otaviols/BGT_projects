using Blizzard.T5.Core;
using PegasusGame;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerLeaderboardManager : CardTileListDisplay
{
  private readonly PlatformDependentValue<float> SPACE_BETWEEN_TILES;
  private readonly PlatformDependentVector3 LEADERBOARD_TILE_SCALE;
  private static PlayerLeaderboardManager s_instance;
  private bool m_disabled;
  private List<PlayerLeaderboardCard> m_playerTiles;
  private PlayerLeaderboardCard m_currentlyMousedOverTile;
  private bool m_isMousedOver;
  private bool m_isNewMouseOver;
  private List<int> m_addedTileForPlayerId;
  private Map<int, List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>> m_combatHistory;
  private Map<int, List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>> m_incomingHistory;
  private Map<int, GameRealTimeBattlefieldRaces> m_pendingRaceCountUpdates;
  private Entity m_oddManOutOpponentHero;
  private const int NULL_PLAYER = 0;
  private bool m_allowFakePlayerTiles;
  private bool m_isLeaderboardDamageCapFXActive;
  private Dictionary<int, int> m_currentWinStreak;
  private Dictionary<int, int> m_currentLoseStreak;

  private static HistoryTileInitInfo CreateHistoryTileInitInfo(Entity entity)
  {
    HistoryTileInitInfo historyTileInitInfo = new HistoryTileInitInfo();
    historyTileInitInfo.m_entity = entity;
    historyTileInitInfo.m_cardDef = entity.ShareDisposableCardDef();
    using (historyTileInitInfo.m_cardDef)
    {
      if ((UnityEngine.Object) historyTileInitInfo.m_cardDef?.CardDef != (UnityEngine.Object) null)
      {
        TAG_PREMIUM premiumType = entity.GetPremiumType();
        historyTileInitInfo.m_portraitTexture = historyTileInitInfo.m_cardDef.CardDef.GetPortraitTexture(premiumType);
        historyTileInitInfo.m_portraitGoldenMaterial = historyTileInitInfo.m_cardDef.CardDef.GetPremiumPortraitMaterial();
        if ((UnityEngine.Object) historyTileInitInfo.m_cardDef.CardDef.GetLeaderboardTileFullPortrait() != (UnityEngine.Object) null)
          historyTileInitInfo.m_fullTileMaterial = historyTileInitInfo.m_cardDef.CardDef.GetLeaderboardTileFullPortrait();
        else
          historyTileInitInfo.m_cardDef.CardDef.TryGetHistoryTileFullPortrait(premiumType, out historyTileInitInfo.m_fullTileMaterial);
        historyTileInitInfo.m_cardDef.CardDef.TryGetHistoryTileHalfPortrait(premiumType, out historyTileInitInfo.m_halfTileMaterial);
      }
      return historyTileInitInfo;
    }
  }

  protected override void Awake()
  {
    base.Awake();
    PlayerLeaderboardManager.s_instance = this;
    this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 0.15f, this.transform.position.z);
    this.SetEnabled(false);
    GameState gameState = GameState.Get();
    if (gameState == null)
    {
      Debug.LogWarning((object) "PlayerLeaderboardManager.Awake() - GameState was not Initialized. Initializing now...");
      gameState = GameState.Initialize();
    }
    gameState.RegisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnChanged));
    gameState.RegisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
    gameState.RegisterDamageCapChangedListener(new GameState.DamageCapChangedCallback(this.OnDamageCapChanged));
    gameState.RegisterDiabloFightPlayerIDChangedListener(new GameState.DiabloFightPlayerIDChangedCallback(this.OnDiabloFightPlayerIDChanged));
  }

  protected override void OnDestroy()
  {
    PlayerLeaderboardManager.s_instance = (PlayerLeaderboardManager) null;
    if (GameState.Get() != null)
    {
      GameState.Get().UnregisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnChanged));
      GameState.Get().UnregisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
      GameState.Get().UnregisterDamageCapChangedListener(new GameState.DamageCapChangedCallback(this.OnDamageCapChanged));
      GameState.Get().UnregisterDamageCapChangedListener(new GameState.DamageCapChangedCallback(this.OnDamageCapChanged));
    }
    base.OnDestroy();
  }

  public static PlayerLeaderboardManager Get() => PlayerLeaderboardManager.s_instance;

  public void SetEnabled(bool enabled)
  {
    this.m_disabled = !enabled;
    this.GetComponent<Collider>().enabled = enabled;
  }

  public bool IsEnabled() => !this.m_disabled;

  public void SetAllowFakePlayers(bool enabled) => this.m_allowFakePlayerTiles = enabled;

  public void CreatePlayerTile(Entity playerHero)
  {
    if (this.m_disabled)
      return;
    int playerHeroId = playerHero.GetTag(GAME_TAG.PLAYER_ID);
    if (playerHeroId == 0)
      playerHeroId = playerHero.GetTag(GAME_TAG.CONTROLLER);
    if (!GameState.Get().GetPlayerInfoMap().ContainsKey(playerHeroId))
    {
      if (this.m_allowFakePlayerTiles)
      {
        SharedPlayerInfo playerInfo = new SharedPlayerInfo();
        playerInfo.SetPlayerId(playerHeroId);
        GameState.Get().AddPlayerInfo(playerInfo);
      }
      else
      {
        Log.Gameplay.PrintError(string.Format("PlayerLeaderboardManager.CreatePlayerTile() - Attempt to add player id {0} to leaderboard, but that is not a valid id.", (object) playerHeroId));
        return;
      }
    }
    if (this.m_addedTileForPlayerId.Any<int>((Func<int, bool>) (t => t == playerHeroId)))
      return;
    this.m_addedTileForPlayerId.Add(playerHeroId);
    AssetLoader.Get().InstantiatePrefab((AssetReference) "PlayerLeaderboardCard.prefab:d44578463b3005d4a938fb1bd2181a82", new PrefabCallback<GameObject>(this.TileLoadedCallback), (object) playerHero, AssetLoadingOptions.IgnorePrefabPosition);
  }

  public void UpdatePlayerTileHeroPower(Entity hero, int newHeroPowerId)
  {
    PlayerLeaderboardCard tileForPlayerId = this.GetTileForPlayerId(hero.GetTag(GAME_TAG.PLAYER_ID));
    if (!((UnityEngine.Object) tileForPlayerId != (UnityEngine.Object) null))
      return;
    tileForPlayerId.SetHeroPower(hero);
  }

  public void NotifyBattlegroundHeroBuddyEnabledDirty()
  {
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
      playerTile.SetBattlegroundHeroBuddyEnabledDirty();
  }

  public void NotifyBattlegroundsQuestRewardEnabledDirty()
  {
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
      playerTile.SetBGQuestRewardDirty();
  }

  public void NotifyPlayerTileEvent(
    int playerId,
    PlayerLeaderboardManager.PlayerTileEvent tileEvent)
  {
    if (!this.m_addedTileForPlayerId.Contains(playerId))
      return;
    PlayerLeaderboardCard tileForPlayerId = this.GetTileForPlayerId(playerId);
    EmoteType emoteType;
    switch (tileEvent)
    {
      case PlayerLeaderboardManager.PlayerTileEvent.TRIPLE:
        if ((UnityEngine.Object) tileForPlayerId != (UnityEngine.Object) null)
          tileForPlayerId.SetTriplesDirty();
        emoteType = EmoteType.BATTLEGROUNDS_VISUAL_TRIPLE;
        break;
      case PlayerLeaderboardManager.PlayerTileEvent.WIN_STREAK:
        emoteType = EmoteType.BATTLEGROUNDS_VISUAL_HOT_STREAK;
        break;
      case PlayerLeaderboardManager.PlayerTileEvent.TECH_LEVEL:
        int num1 = 1;
        if (GameState.Get().GetPlayerInfoMap().ContainsKey(playerId) && GameState.Get().GetPlayerInfoMap()[playerId].GetPlayerHero() != null)
          num1 = GameState.Get().GetPlayerInfoMap()[playerId].GetPlayerHero().GetRealTimePlayerTechLevel();
        int num2 = Mathf.Clamp(num1, 1, 6);
        if ((UnityEngine.Object) tileForPlayerId != (UnityEngine.Object) null)
          tileForPlayerId.SetTechLevelDirty();
        switch (num2)
        {
          case 2:
            emoteType = EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_02;
            break;
          case 3:
            emoteType = EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_03;
            break;
          case 4:
            emoteType = EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_04;
            break;
          case 5:
            emoteType = EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_05;
            break;
          case 6:
            emoteType = EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_06;
            break;
          default:
            return;
        }
        break;
      case PlayerLeaderboardManager.PlayerTileEvent.RECENT_COMBAT:
        if (!((UnityEngine.Object) tileForPlayerId != (UnityEngine.Object) null))
          return;
        tileForPlayerId.SetRecentCombatsDirty();
        return;
      case PlayerLeaderboardManager.PlayerTileEvent.KNOCK_OUT:
        return;
      case PlayerLeaderboardManager.PlayerTileEvent.RACES:
        if (!((UnityEngine.Object) tileForPlayerId != (UnityEngine.Object) null))
          return;
        tileForPlayerId.SetRacesDirty();
        return;
      case PlayerLeaderboardManager.PlayerTileEvent.BANANA:
        emoteType = EmoteType.BATTLEGROUNDS_VISUAL_BANANA;
        break;
      case PlayerLeaderboardManager.PlayerTileEvent.HERO_BUDDY:
        emoteType = EmoteType.BATTLEGROUNDS_VISUAL_HERO_BUDDY;
        break;
      case PlayerLeaderboardManager.PlayerTileEvent.DOUBLE_HERO_BUDDY:
        emoteType = EmoteType.BATTLEGROUNDS_VISUAL_DOUBLE_HERO_BUDDY;
        break;
      case PlayerLeaderboardManager.PlayerTileEvent.QUEST_COMPLETE:
        emoteType = EmoteType.BATTLEGROUNDS_VISUAL_QUEST_COMPLETE;
        break;
      case PlayerLeaderboardManager.PlayerTileEvent.QUEST_UPDATE:
        if (!((UnityEngine.Object) tileForPlayerId != (UnityEngine.Object) null))
          return;
        tileForPlayerId.SetBGQuestRewardDirty();
        return;
      default:
        return;
    }
    GameState.Get().GetGameEntity().PlayAlternateEnemyEmote(playerId, emoteType);
  }

  public void NotifyOfInput(Vector3 hitPoint)
  {
    this.m_isMousedOver = true;
    if (this.m_playerTiles.Count == 0)
    {
      this.CheckForMouseOff();
    }
    else
    {
      float num1 = 1000f;
      float num2 = -1000f;
      float num3 = 1000f;
      PlayerLeaderboardCard playerLeaderboardCard = (PlayerLeaderboardCard) null;
      foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
      {
        if (playerTile.HasBeenShown())
        {
          Collider tileCollider = playerTile.GetTileCollider();
          if (!((UnityEngine.Object) tileCollider == (UnityEngine.Object) null))
          {
            Bounds bounds = tileCollider.bounds;
            double z1 = (double) bounds.center.z;
            bounds = tileCollider.bounds;
            double z2 = (double) bounds.extents.z;
            float num4 = (float) (z1 - z2);
            bounds = tileCollider.bounds;
            double z3 = (double) bounds.center.z;
            bounds = tileCollider.bounds;
            double z4 = (double) bounds.extents.z;
            float num5 = (float) (z3 + z4);
            if ((double) num4 < (double) num1)
              num1 = num4;
            if ((double) num5 > (double) num2)
              num2 = num5;
            float num6 = Mathf.Abs(hitPoint.z - num4);
            if ((double) num6 < (double) num3)
            {
              num3 = num6;
              playerLeaderboardCard = playerTile;
            }
            float num7 = Mathf.Abs(hitPoint.z - num5);
            if ((double) num7 < (double) num3)
            {
              num3 = num7;
              playerLeaderboardCard = playerTile;
            }
          }
        }
      }
      if ((double) hitPoint.z < (double) num1 || (double) hitPoint.z > (double) num2)
        this.CheckForMouseOff();
      else if ((UnityEngine.Object) playerLeaderboardCard == (UnityEngine.Object) null)
      {
        this.CheckForMouseOff();
      }
      else
      {
        Collider component = (Collider) this.gameObject.GetComponent<BoxCollider>();
        Collider tileCollider = playerLeaderboardCard.GetTileCollider();
        float num8 = 0.0f;
        if (playerLeaderboardCard.GetNextOpponentState())
          num8 = playerLeaderboardCard.GetPoppedOutBoneX() * playerLeaderboardCard.m_tileActor.transform.localScale.x;
        double x1 = (double) hitPoint.x;
        double x2 = (double) component.bounds.center.x;
        Bounds bounds = tileCollider.bounds;
        double x3 = (double) bounds.extents.x;
        double num9 = x2 - x3;
        if (x1 >= num9)
        {
          double x4 = (double) hitPoint.x;
          bounds = component.bounds;
          double x5 = (double) bounds.center.x;
          bounds = tileCollider.bounds;
          double x6 = (double) bounds.extents.x;
          double num10 = x5 + x6 + (double) num8;
          if (x4 <= num10)
          {
            if ((UnityEngine.Object) playerLeaderboardCard == (UnityEngine.Object) this.m_currentlyMousedOverTile)
              return;
            if ((UnityEngine.Object) this.m_currentlyMousedOverTile != (UnityEngine.Object) null)
              this.m_currentlyMousedOverTile.NotifyMousedOut();
            else
              this.FadeVignetteIn();
            this.m_currentlyMousedOverTile = playerLeaderboardCard;
            playerLeaderboardCard.NotifyMousedOver();
            this.m_isNewMouseOver = false;
            return;
          }
        }
        this.CheckForMouseOff();
      }
    }
  }

  public void NotifyOfMouseOff() => this.CheckForMouseOff();

  public void SetNextOpponent(int opponentPlayerId)
  {
    if (opponentPlayerId == 0)
      return;
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
      playerTile.SetNextOpponentState(playerTile.m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID) == opponentPlayerId);
  }

  public void SetCurrentOpponent(int opponentPlayerId)
  {
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
      playerTile.SetCurrentOpponentState(playerTile.m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID) == opponentPlayerId);
  }

  public void ApplyEntityReplacement(int playerID, Entity replacementEntity)
  {
    for (int index = 0; index < this.m_playerTiles.Count; ++index)
    {
      PlayerLeaderboardCard playerTile = this.m_playerTiles[index];
      if (playerTile.m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID) == playerID)
      {
        HistoryTileInitInfo historyTileInitInfo = PlayerLeaderboardManager.CreateHistoryTileInitInfo(replacementEntity);
        playerTile.Initialize(replacementEntity);
        playerTile.RefreshTileVisuals(historyTileInitInfo);
        playerTile.RefreshMainCardActor();
        playerTile.RefreshMainCardName();
      }
      playerTile.RefreshRecentCombats();
    }
    if (this.m_oddManOutOpponentHero.GetTag(GAME_TAG.PLAYER_ID) != playerID)
      return;
    this.m_oddManOutOpponentHero = replacementEntity;
  }

  private void OnTurnChanged(int oldTurn, int newTurn, object userdata)
  {
    int tag = GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.NEXT_OPPONENT_PLAYER_ID);
    if (GameState.Get().GetCurrentPlayer().IsFriendlySide())
    {
      this.SetNextOpponent(tag);
      this.SetCurrentOpponent(-1);
      this.ApplyIncomingCombatHistory();
    }
    else
    {
      this.SetCurrentOpponent(tag);
      foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
        playerTile.PauseHealthUpdates();
    }
  }

  private void OnDamageCapChanged(int oldValue, int newValue, object userdata) => this.UpdateDamageCapFX(oldValue, newValue);

  private void OnDiabloFightPlayerIDChanged(int oldValue, int newValue, object userdata)
  {
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
      playerTile.UpdateDiabloPlayerFightFX(oldValue, newValue);
  }

  public void EnableDamageCapFX(bool enable)
  {
    this.m_isLeaderboardDamageCapFXActive = enable;
    this.UpdateDamageCapFX(forceUpdate: true);
  }

  private void UpdateDamageCapFX(int oldValue = -1, int newValue = -1, bool forceUpdate = false)
  {
    Spell leaderboardDamageCapFx = Board.Get().m_leaderboardDamageCapFX;
    if (!this.m_isLeaderboardDamageCapFXActive)
    {
      if (!((UnityEngine.Object) leaderboardDamageCapFx != (UnityEngine.Object) null))
        return;
      SpellUtils.ActivateDeathIfNecessary(leaderboardDamageCapFx);
    }
    else
    {
      if (newValue == -1)
      {
        if (GameState.Get() == null || GameState.Get().GetGameEntity() == null)
        {
          Debug.Log((object) "[PlayerLeaderboardManager::UpdateDamageCapFX] - Game State/Game Entity is null");
          return;
        }
        newValue = GameState.Get().GetGameEntity().GetTag(GAME_TAG.BACON_COMBAT_DAMAGE_CAP);
      }
      if (!forceUpdate && oldValue == newValue || !((UnityEngine.Object) leaderboardDamageCapFx != (UnityEngine.Object) null))
        return;
      if (newValue != 0)
      {
        leaderboardDamageCapFx.gameObject.SetActive(true);
        SpellUtils.ActivateBirthIfNecessary(leaderboardDamageCapFx);
      }
      else
        SpellUtils.ActivateDeathIfNecessary(leaderboardDamageCapFx);
    }
  }

  private void OnCreateGame(GameState.CreateGamePhase phase, object userData)
  {
    this.ApplyIncomingCombatHistory(true);
    this.UpdateDamageCapFX();
  }

  public bool IsMousedOver() => this.m_isMousedOver;

  public bool IsNewlyMousedOver() => this.m_isMousedOver && this.m_isNewMouseOver;

  private void CheckForMouseOff()
  {
    if (!this.m_isMousedOver)
      return;
    this.m_isMousedOver = false;
    this.m_isNewMouseOver = true;
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
      playerTile.NotifyMousedOut();
    if ((UnityEngine.Object) this.m_currentlyMousedOverTile != (UnityEngine.Object) null)
      this.FadeVignetteOut();
    this.m_currentlyMousedOverTile = (PlayerLeaderboardCard) null;
  }

  private void FadeVignetteIn()
  {
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
    {
      if (!((UnityEngine.Object) playerTile.m_tileActor == (UnityEngine.Object) null))
        LayerUtils.SetLayer(playerTile.m_tileActor.gameObject, GameLayer.IgnoreFullScreenEffects);
    }
    LayerUtils.SetLayer(this.gameObject, GameLayer.IgnoreFullScreenEffects);
    this.AnimateBlurVignetteIn();
  }

  private void FadeVignetteOut()
  {
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
    {
      if (!((UnityEngine.Object) playerTile.m_tileActor == (UnityEngine.Object) null))
        LayerUtils.SetLayer(playerTile.GetTileCollider().gameObject, GameLayer.Default);
    }
    LayerUtils.SetLayer(this.gameObject, GameLayer.CardRaycast);
    this.AnimateBlurVignetteOut();
  }

  protected override void OnFullScreenEffectOutFinished()
  {
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
    {
      if (!((UnityEngine.Object) playerTile.m_tileActor == (UnityEngine.Object) null))
        LayerUtils.SetLayer(playerTile.m_tileActor.gameObject, GameLayer.Default);
    }
  }

  private void TileLoadedCallback(AssetReference assetRef, GameObject go, object callbackData)
  {
    Entity entity = (Entity) callbackData;
    using (DefLoader.DisposableCardDef disposableCardDef = entity.ShareDisposableCardDef())
    {
      if ((UnityEngine.Object) disposableCardDef?.CardDef == (UnityEngine.Object) null)
      {
        this.m_addedTileForPlayerId.Remove(entity.GetTag(GAME_TAG.PLAYER_ID));
        return;
      }
    }
    go.transform.localScale = (Vector3) (PlatformDependentValue<Vector3>) this.LEADERBOARD_TILE_SCALE;
    PlayerLeaderboardCard component = go.GetComponent<PlayerLeaderboardCard>();
    component.Initialize(entity);
    this.m_playerTiles.Add(component);
    HistoryTileInitInfo historyTileInitInfo = PlayerLeaderboardManager.CreateHistoryTileInitInfo(entity);
    component.LoadTile(historyTileInitInfo);
    int tag = component.m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID);
    if (this.m_pendingRaceCountUpdates.ContainsKey(tag))
      this.UpdatePlayerRaces(this.m_pendingRaceCountUpdates[tag]);
    this.SetAsideTileAndTryToUpdate(component);
  }

  public PlayerLeaderboardCard GetTileForPlayerId(int playerId)
  {
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
    {
      if (playerTile.m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID) == playerId)
        return playerTile;
    }
    return (PlayerLeaderboardCard) null;
  }

  private void SetAsideTileAndTryToUpdate(PlayerLeaderboardCard tile)
  {
    Vector3 topTilePosition = this.GetTopTilePosition();
    int num1 = this.m_playerTiles.IndexOf(tile);
    Collider tileCollider = tile.GetTileCollider();
    float num2 = 0.0f;
    if ((UnityEngine.Object) tileCollider != (UnityEngine.Object) null)
      num2 = (tileCollider.bounds.size.z + (float) this.SPACE_BETWEEN_TILES) * (float) num1;
    tile.transform.position = new Vector3(topTilePosition.x, topTilePosition.y, topTilePosition.z - num2);
    if (GameState.Get().IsMulliganManagerActive())
    {
      tile.m_PlayerLeaderboardTile.SetTileRevealed(false, false);
    }
    else
    {
      tile.MarkAsShown();
      this.UpdateLayout(false);
    }
  }

  private Vector3 GetTopTilePosition() => new Vector3(this.transform.position.x, this.transform.position.y - 0.15f, this.transform.position.z);

  public void UpdateLayout(bool animate = true)
  {
    this.SortPlayers();
    this.UpdateHealthTotals();
    float num = 0.0f;
    Vector3 topTilePosition = this.GetTopTilePosition();
    for (int index = 0; index < this.m_playerTiles.Count; ++index)
    {
      Collider tileCollider = this.m_playerTiles[index].GetTileCollider();
      Vector3 position = new Vector3(topTilePosition.x, topTilePosition.y, topTilePosition.z - num);
      if (animate)
        iTween.MoveTo(this.m_playerTiles[index].gameObject, position, 1f);
      else
        this.m_playerTiles[index].gameObject.transform.position = position;
      bool isNextOpponent = this.m_playerTiles[index].m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID) == GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.NEXT_OPPONENT_PLAYER_ID);
      if (!this.m_playerTiles[index].HasBeenShown() && GameState.Get().IsMulliganManagerActive())
        this.m_playerTiles[index].m_PlayerLeaderboardTile.SetTileRevealed(true, isNextOpponent);
      this.m_playerTiles[index].MarkAsShown();
      this.m_playerTiles[index].UpdateOddPlayerOutFx(isNextOpponent);
      if ((UnityEngine.Object) tileCollider != (UnityEngine.Object) null)
        num += tileCollider.bounds.size.z + (float) this.SPACE_BETWEEN_TILES;
    }
  }

  public void UpdateRoundHistory(GameRoundHistory gameRoundHistory)
  {
    this.m_incomingHistory.Clear();
    for (int index = 0; index < gameRoundHistory.Rounds.Count; ++index)
      this.AddCombatRound(gameRoundHistory.Rounds[index]);
  }

  public void UpdatePlayerRaces(
    GameRealTimeBattlefieldRaces realTimeBattlefieldRaces)
  {
    PlayerLeaderboardCard tileForPlayerId = this.GetTileForPlayerId(realTimeBattlefieldRaces.PlayerId);
    if ((UnityEngine.Object) tileForPlayerId != (UnityEngine.Object) null)
      tileForPlayerId.UpdateRacesCount(realTimeBattlefieldRaces.Races);
    else if (!this.m_pendingRaceCountUpdates.ContainsKey(realTimeBattlefieldRaces.PlayerId))
      this.m_pendingRaceCountUpdates.Add(realTimeBattlefieldRaces.PlayerId, realTimeBattlefieldRaces);
    else
      this.m_pendingRaceCountUpdates[realTimeBattlefieldRaces.PlayerId] = realTimeBattlefieldRaces;
  }

  private void ApplyIncomingCombatHistory(bool suppressNotifications = false)
  {
    this.m_combatHistory.Clear();
    this.m_combatHistory = new Map<int, List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>>((IEnumerable<KeyValuePair<int, List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>>>) this.m_incomingHistory);
    int val1 = 0;
    foreach (KeyValuePair<int, List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>> keyValuePair in this.m_combatHistory)
    {
      if (keyValuePair.Value != null && keyValuePair.Value.Count != 0)
        val1 = Math.Max(val1, keyValuePair.Value.Count);
    }
    foreach (KeyValuePair<int, List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>> keyValuePair in this.m_combatHistory)
    {
      if (keyValuePair.Value != null && keyValuePair.Value.Count != 0)
      {
        PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo recentCombatInfo1 = new PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo();
        PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo recentCombatInfo2 = keyValuePair.Value.LastOrDefault<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>();
        if (keyValuePair.Value.Count > 1)
          recentCombatInfo1 = keyValuePair.Value[keyValuePair.Value.Count - 2];
        if (keyValuePair.Value.Count == val1)
        {
          int num = 0;
          if (this.m_currentWinStreak.ContainsKey(keyValuePair.Key))
            num = this.m_currentWinStreak[keyValuePair.Key];
          this.m_currentWinStreak[keyValuePair.Key] = recentCombatInfo2.winStreak;
          this.m_currentLoseStreak[keyValuePair.Key] = recentCombatInfo2.loseStreak;
          if ((recentCombatInfo2.winStreak <= 1 || recentCombatInfo2.winStreak <= recentCombatInfo1.winStreak || suppressNotifications ? 0 : (num != recentCombatInfo2.winStreak ? 1 : 0)) != 0)
            this.NotifyPlayerTileEvent(keyValuePair.Key, PlayerLeaderboardManager.PlayerTileEvent.WIN_STREAK);
        }
        this.NotifyPlayerTileEvent(keyValuePair.Key, PlayerLeaderboardManager.PlayerTileEvent.RECENT_COMBAT);
      }
    }
  }

  private void AddCombatRound(GameRoundHistoryEntry gameRound)
  {
    Dictionary<int, GameRoundHistoryPlayerEntry> dictionary = gameRound.Combats.ToDictionary<GameRoundHistoryPlayerEntry, int, GameRoundHistoryPlayerEntry>((Func<GameRoundHistoryPlayerEntry, int>) (combat => combat.PlayerId), (Func<GameRoundHistoryPlayerEntry, GameRoundHistoryPlayerEntry>) (combat => combat));
    foreach (KeyValuePair<int, GameRoundHistoryPlayerEntry> keyValuePair in dictionary)
    {
      int key = keyValuePair.Key;
      if (key != 0 && (!keyValuePair.Value.PlayerIsDead || keyValuePair.Value.PlayerDiedThisRound))
      {
        this.AddPlayerToCombatHistoryIfNeeded(key);
        GameRoundHistoryPlayerEntry playerEntry = keyValuePair.Value;
        GameRoundHistoryPlayerEntry opponentEntry = dictionary[playerEntry.PlayerOpponentId];
        this.m_incomingHistory[key].Add(this.ConvertGameRoundHistoryToRecentCombatInfo(playerEntry, opponentEntry));
      }
    }
  }

  private PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo ConvertGameRoundHistoryToRecentCombatInfo(
    GameRoundHistoryPlayerEntry playerEntry,
    GameRoundHistoryPlayerEntry opponentEntry)
  {
    PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo recentCombatInfo1 = new PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo();
    recentCombatInfo1.ownerId = playerEntry.PlayerId;
    recentCombatInfo1.opponentId = opponentEntry.PlayerId;
    recentCombatInfo1.damage = playerEntry.PlayerDamageTaken != 0 ? playerEntry.PlayerDamageTaken : opponentEntry.PlayerDamageTaken;
    recentCombatInfo1.isDefeated = playerEntry.PlayerIsDead || opponentEntry.PlayerIsDead;
    recentCombatInfo1.damageTarget = playerEntry.PlayerDamageTaken == 0 ? (opponentEntry.PlayerDamageTaken == 0 ? PlayerLeaderboardRecentCombatsPanel.NO_DAMAGE_TARGET : opponentEntry.PlayerId) : playerEntry.PlayerId;
    if (this.m_incomingHistory[playerEntry.PlayerId].Count > 0)
    {
      PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo recentCombatInfo2 = this.m_incomingHistory[playerEntry.PlayerId].Last<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>();
      recentCombatInfo1.loseStreak = recentCombatInfo2.loseStreak;
      recentCombatInfo1.winStreak = recentCombatInfo2.winStreak;
    }
    if (recentCombatInfo1.damageTarget == PlayerLeaderboardRecentCombatsPanel.NO_DAMAGE_TARGET)
      return recentCombatInfo1;
    if (recentCombatInfo1.damageTarget == playerEntry.PlayerId && (recentCombatInfo1.damage > 0 || recentCombatInfo1.isDefeated))
    {
      recentCombatInfo1.winStreak = 0;
      ++recentCombatInfo1.loseStreak;
    }
    else
    {
      ++recentCombatInfo1.winStreak;
      recentCombatInfo1.loseStreak = 0;
    }
    return recentCombatInfo1;
  }

  private void AddPlayerToCombatHistoryIfNeeded(int playerId)
  {
    if (playerId == 0 || this.m_incomingHistory.ContainsKey(playerId))
      return;
    this.m_incomingHistory.Add(playerId, new List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>());
  }

  public List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo> GetRecentCombatHistoryForPlayer(
    int playerId)
  {
    return this.m_combatHistory.ContainsKey(playerId) ? this.m_combatHistory[playerId] : (List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>) null;
  }

  public int GetNumTiles() => this.m_playerTiles.Count;

  public int GetLatestWinStreakForPlayer(int playerId)
  {
    int num;
    return !this.m_currentWinStreak.TryGetValue(playerId, out num) ? 0 : num;
  }

  public int GetLatestLoseStreakForPlayer(int playerId)
  {
    int num;
    return !this.m_currentLoseStreak.TryGetValue(playerId, out num) ? 0 : num;
  }

  public int GetIndexForTile(PlayerLeaderboardCard tile)
  {
    for (int index = 0; index < this.m_playerTiles.Count; ++index)
    {
      if ((UnityEngine.Object) this.m_playerTiles[index] == (UnityEngine.Object) tile)
        return index;
    }
    Debug.LogWarning((object) "PlayerLeaderboardManager.GetIndexForTile() - that Tile doesn't exist!");
    return -1;
  }

  private void SortPlayers()
  {
    this.m_playerTiles = this.m_playerTiles.OrderBy<PlayerLeaderboardCard, int>((Func<PlayerLeaderboardCard, int>) (t => t.m_playerHeroEntity.GetRealTimePlayerLeaderboardPlace())).ToList<PlayerLeaderboardCard>();
    for (int index = 0; index < this.m_playerTiles.Count; ++index)
      this.m_playerTiles[index].m_PlayerLeaderboardTile.SetPlaceIcon(index + 1);
  }

  private void UpdateHealthTotals()
  {
    foreach (PlayerLeaderboardCard playerTile in this.m_playerTiles)
      playerTile.UpdateTileHealth();
  }

  public void SetOddManOutOpponentHero(Entity entity) => this.m_oddManOutOpponentHero = entity;

  public Entity GetOddManOutOpponentHero() => this.m_oddManOutOpponentHero;

  public PlayerLeaderboardManager()
  {
    PlatformDependentVector3 dependentVector3 = new PlatformDependentVector3(PlatformCategory.Screen);
    dependentVector3.PC = new Vector3(1.2f, 1.2f, 1.2f);
    dependentVector3.Phone = new Vector3(1f, 1f, 1f);
    this.LEADERBOARD_TILE_SCALE = dependentVector3;
    this.m_playerTiles = new List<PlayerLeaderboardCard>();
    this.m_isNewMouseOver = true;
    this.m_addedTileForPlayerId = new List<int>();
    this.m_combatHistory = new Map<int, List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>>();
    this.m_incomingHistory = new Map<int, List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo>>();
    this.m_pendingRaceCountUpdates = new Map<int, GameRealTimeBattlefieldRaces>();
    this.m_isLeaderboardDamageCapFXActive = true;
    this.m_currentWinStreak = new Dictionary<int, int>();
    this.m_currentLoseStreak = new Dictionary<int, int>();
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public enum PlayerTileEvent
  {
    TRIPLE,
    WIN_STREAK,
    TECH_LEVEL,
    RECENT_COMBAT,
    KNOCK_OUT,
    RACES,
    BANANA,
    HERO_BUDDY,
    DOUBLE_HERO_BUDDY,
    QUEST_COMPLETE,
    QUEST_UPDATE,
  }
}
