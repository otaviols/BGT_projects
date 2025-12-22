using Blizzard.T5.AssetManager;
using Blizzard.T5.Core.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaconBoard : Board
{
  public float m_ShopAmbientTransitionDelay = 0.5f;
  public float m_ShopAmbientTransitionTime = 0.25f;
  public GameObject m_LeaderboardFrame;
  public GameObject m_TableTop;
  private const int BOARD_SKIN_UNINITIALIZED = 0;
  private const int BOARD_SKIN_DEFAULT = 1;
  private readonly BaconBoard.BoardSkinStatus m_FullBoardSkin = new BaconBoard.BoardSkinStatus();
  private int m_ChosenBoardSkinId;
  private int m_ChosenCombatBoardSkinId;
  private bool m_InBattle;
  private int m_BattleRound;
  private int m_PendingLoads;
  private bool m_StartedLoadingChosenBoardThisRound;
  private TAG_BOARD_VISUAL_STATE m_currentBoardState;
  private HashSet<string> m_minionsDefeatedByPlayer = new HashSet<string>();
  private HashSet<TAG_RACE> m_racesDefeatedByPlayer = new HashSet<TAG_RACE>();
  private int m_minionsDefeatedCount;
  private static BaconBoard s_Instance;
  private int m_CheatWinstreak;
  private bool m_CheatHasDefeatedOpponent;
  private BaconBoard.StateChangeCallback m_stateChangeCallback;
  private DateTime m_LastBoardVisualStateChangeDateTime;

  public void CheatSetWinstreak(int streak) => this.m_CheatWinstreak = streak;

  public void CheatSetDefeatedMinionCount(int count) => this.m_minionsDefeatedCount = count;

  public void CheatSetHasDefeatedOpponent() => this.m_CheatHasDefeatedOpponent = true;

  public void CheatAddDefeatedRace(TAG_RACE race) => this.m_racesDefeatedByPlayer.Add(race);

  public void CheatAddDefeatedMinion(string cardID) => this.m_minionsDefeatedByPlayer.Add(cardID);

  public bool CheatTriggerDefeatedMinion(string cardID)
  {
    if (!((UnityEngine.Object) this.m_FullBoardSkin.m_CombatInstance != (UnityEngine.Object) null))
      return false;
    this.m_FullBoardSkin.m_CombatInstance.CheatTriggerDefeatMinion(cardID);
    return true;
  }

  public bool CheatTriggerHeroHeavyHitEffects()
  {
    if (!((UnityEngine.Object) this.m_FullBoardSkin.m_CombatInstance != (UnityEngine.Object) null))
      return false;
    this.m_FullBoardSkin.m_CombatInstance.CheatTriggerHeroHeavyHitBoardEffects();
    return true;
  }

  public bool CheatTriggerMinionHeavyHitEffects()
  {
    if (!((UnityEngine.Object) this.m_FullBoardSkin.m_CombatInstance != (UnityEngine.Object) null))
      return false;
    this.m_FullBoardSkin.m_CombatInstance.CheatTriggerMinionHeavyHitBoardEffects();
    return true;
  }

  public bool CheatTriggerAllBoardEffects()
  {
    if (!((UnityEngine.Object) this.m_FullBoardSkin.m_CombatInstance != (UnityEngine.Object) null))
      return false;
    this.m_FullBoardSkin.m_CombatInstance.CheatTriggerAllBoardEffects();
    return true;
  }

  public static BaconBoard Get() => BaconBoard.s_Instance;

  public override void Start()
  {
    base.Start();
    BaconBoard.s_Instance = this;
    this.m_LastBoardVisualStateChangeDateTime = DateTime.Now;
    this.m_currentBoardState = TAG_BOARD_VISUAL_STATE.SHOP;
  }

  protected override void OnDestroy()
  {
    base.OnDestroy();
    BaconBoard.s_Instance = (BaconBoard) null;
    this.m_InBattle = false;
    this.UnloadSkinAssets();
  }

  public void AddStateChangeCallback(BaconBoard.StateChangeCallback newCallback) => this.m_stateChangeCallback += newCallback;

  public void RemoveStateChangeCallback(BaconBoard.StateChangeCallback oldCallback) => this.m_stateChangeCallback -= oldCallback;

  public override bool AreAllAssetsLoaded() => this.m_PendingLoads <= 0;

  public override void ChangeBoardVisualState(TAG_BOARD_VISUAL_STATE boardState)
  {
    this.SendBoardVisualStatChangeTelmetry(this.m_currentBoardState, boardState, this.m_LastBoardVisualStateChangeDateTime);
    this.m_LastBoardVisualStateChangeDateTime = DateTime.Now;
    this.m_currentBoardState = boardState;
    if (this.m_currentBoardState == TAG_BOARD_VISUAL_STATE.COMBAT)
      this.OnTransitionToBattle();
    if (this.m_currentBoardState != TAG_BOARD_VISUAL_STATE.SHOP)
      return;
    this.OnTransitionToShop();
  }

  public void LoadInitialTavernBoard(int chosenBoardSkinId)
  {
    this.m_ChosenBoardSkinId = chosenBoardSkinId;
    this.TryToLoadTavernPrefab(this.m_FullBoardSkin);
  }

  public void OnBoardSkinChosen(int chosenBoardSkinId)
  {
    this.m_ChosenCombatBoardSkinId = chosenBoardSkinId;
    this.TryLoadChosenBoardSkinPrefabs();
  }

  public void ProcessUnloadRequest(BaconBoardSkinBehaviour sourceBehavior) => this.UnloadSkinAssets();

  public void NotifyOfMinionDied(Entity minion)
  {
    if (!this.m_InBattle || (UnityEngine.Object) this.m_FullBoardSkin.m_CombatInstance == (UnityEngine.Object) null || !minion.IsControlledByOpposingSidePlayer())
      return;
    ++this.m_minionsDefeatedCount;
    EntityDef entityDef = minion.GetEntityDef();
    this.m_minionsDefeatedByPlayer.Add(entityDef.GetCardId());
    foreach (TAG_RACE race in entityDef.GetRaces())
      this.m_racesDefeatedByPlayer.Add(race);
    this.m_FullBoardSkin.m_CombatInstance.PlayOpponentMinionDefeatedCount(this.m_minionsDefeatedCount);
    this.m_FullBoardSkin.m_CombatInstance.PlayOpponentMinionDefeated(entityDef);
  }

  private void ToggleLeaderboardFrame(bool visible)
  {
    if (!((UnityEngine.Object) this.m_LeaderboardFrame != (UnityEngine.Object) null))
      return;
    this.m_LeaderboardFrame.SetActive(visible);
  }

  private void ToggleTableTop(bool visible)
  {
    if (!((UnityEngine.Object) this.m_TableTop != (UnityEngine.Object) null))
      return;
    this.m_TableTop.SetActive(visible);
  }

  private void OnTransitionToBattle()
  {
    this.m_InBattle = true;
    this.TryLoadChosenBoardSkinPrefabs();
  }

  private void OnTransitionToShop()
  {
    this.m_InBattle = false;
    ++this.m_BattleRound;
    this.m_StartedLoadingChosenBoardThisRound = false;
    this.m_ChosenCombatBoardSkinId = 0;
    this.RunVisualStateAnimators(TAG_BOARD_VISUAL_STATE.SHOP);
    this.SetShopLighting();
    this.ToggleLeaderboardFrame(!this.m_FullBoardSkin.m_TavernInstance.HasOwnLeaderboardFrame());
    this.ToggleTableTop(!this.m_FullBoardSkin.m_TavernInstance.HasOwnTableTop());
    this.RunShopAnimation(this.m_FullBoardSkin);
  }

  public void ChangeBoardVisualStateForPreview(
    TAG_BOARD_VISUAL_STATE boardState,
    BaconBoardSkinBehaviour combatSkin,
    BaconBoardSkinBehaviour tavernSkin)
  {
    this.RunVisualStateAnimators(boardState);
    BaconBoardSkinBehaviour boardSkinBehaviour = boardState == TAG_BOARD_VISUAL_STATE.COMBAT ? combatSkin : tavernSkin;
    this.ToggleLeaderboardFrame(!boardSkinBehaviour.HasOwnLeaderboardFrame());
    this.ToggleTableTop(!boardSkinBehaviour.HasOwnTableTop());
    if (boardState == TAG_BOARD_VISUAL_STATE.COMBAT)
      combatSkin.CopyCornersFromSkin(tavernSkin);
    combatSkin.SetBoardState(boardState);
    tavernSkin.SetBoardState(boardState);
  }

  public void SetShopLighting() => iTween.ValueTo(this.gameObject, iTween.Hash((object) "from", (object) RenderSettings.ambientLight, (object) "to", (object) this.m_AmbientColor, (object) "delay", (object) this.m_ShopAmbientTransitionDelay, (object) "time", (object) this.m_ShopAmbientTransitionTime, (object) "easeType", (object) iTween.EaseType.easeInOutQuad, (object) "onupdate", (object) (Action<object>) (amount => RenderSettings.ambientLight = (Color) amount), (object) "onupdatetarget", (object) this.gameObject));

  private void TryLoadChosenBoardSkinPrefabs()
  {
    if (!this.m_InBattle || this.m_ChosenCombatBoardSkinId == 0 || this.m_StartedLoadingChosenBoardThisRound)
      return;
    this.m_StartedLoadingChosenBoardThisRound = true;
    this.TryToLoadPrefab(this.m_FullBoardSkin);
  }

  private void TryToLoadPrefab(BaconBoard.BoardSkinStatus skin)
  {
    if (this.m_ChosenCombatBoardSkinId == 0)
      this.m_ChosenCombatBoardSkinId = 1;
    BattlegroundsBoardSkinDbfRecord record = GameDbf.BattlegroundsBoardSkin.GetRecord(this.m_ChosenCombatBoardSkinId);
    string assetRef = PlatformSettings.Screen != ScreenCategory.Phone ? record.FullBoardPrefab : record.FullBoardPrefabPhone;
    ++this.m_PendingLoads;
    AssetLoader.Get().LoadAsset<GameObject>((AssetReference) assetRef, new AssetHandleCallback<GameObject>(this.OnSkinLoaded), (object) new BaconBoard.BoardSkinStatusAndRound(skin, this.m_BattleRound));
  }

  private void TryToLoadTavernPrefab(BaconBoard.BoardSkinStatus skin)
  {
    if (this.m_ChosenBoardSkinId == 0)
      this.m_ChosenBoardSkinId = 1;
    BattlegroundsBoardSkinDbfRecord record = GameDbf.BattlegroundsBoardSkin.GetRecord(this.m_ChosenBoardSkinId);
    string assetRef = PlatformSettings.Screen != ScreenCategory.Phone ? record.FullTavernBoardPrefab : record.FullTavernBoardPrefabPhone;
    ++this.m_PendingLoads;
    AssetLoader.Get().LoadAsset<GameObject>((AssetReference) assetRef, new AssetHandleCallback<GameObject>(this.OnTavernSkinLoaded), (object) skin);
  }

  private void OnTavernSkinLoaded(
    AssetReference assetRef,
    AssetHandle<GameObject> asset,
    object callbackData)
  {
    --this.m_PendingLoads;
    BaconBoard.BoardSkinStatus boardSkinStatus = (BaconBoard.BoardSkinStatus) callbackData;
    boardSkinStatus.m_AssetHandleTavern = asset;
    boardSkinStatus.m_TavernPrefab = asset.Asset;
    if (!this.AreAllAssetsLoaded())
      return;
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(boardSkinStatus.m_TavernPrefab);
    if (!gameObject.TryGetComponent<BaconBoardSkinBehaviour>(out boardSkinStatus.m_TavernInstance))
    {
      Debug.LogError((object) ("Attempting to get component BaconBoardSkinBehaviour but not found on " + (object) gameObject));
    }
    else
    {
      this.ToggleLeaderboardFrame(!boardSkinStatus.m_TavernInstance.HasOwnLeaderboardFrame());
      this.ToggleTableTop(!boardSkinStatus.m_TavernInstance.HasOwnTableTop());
      if (this.m_AllAssetsLoadedCallback == null)
        return;
      this.m_AllAssetsLoadedCallback();
    }
  }

  private void OnSkinLoaded(
    AssetReference assetRef,
    AssetHandle<GameObject> asset,
    object callbackData)
  {
    --this.m_PendingLoads;
    BaconBoard.BoardSkinStatusAndRound skinStatusAndRound = (BaconBoard.BoardSkinStatusAndRound) callbackData;
    BaconBoard.BoardSkinStatus skin = skinStatusAndRound?.m_Skin;
    if (skinStatusAndRound == null || skin == null)
      Log.All.PrintWarning(string.Format("[BaconBoard.OnSkinLoaded] skin or skinWithRound is null, assetRef:{0}", (object) assetRef));
    if (skinStatusAndRound.m_Round != this.m_BattleRound)
    {
      asset.Dispose();
    }
    else
    {
      skin.m_AssetHandleCombat = asset;
      skin.m_CombatPrefab = asset.Asset;
      if (!this.AreAllAssetsLoaded())
        return;
      this.TryStartBattleTransitionAnimations();
      if (this.m_AllAssetsLoadedCallback == null)
        return;
      this.m_AllAssetsLoadedCallback();
    }
  }

  private void RunShopAnimation(BaconBoard.BoardSkinStatus skin)
  {
    if ((UnityEngine.Object) skin.m_CombatInstance != (UnityEngine.Object) null)
    {
      skin.m_CombatInstance.SetBoardState(TAG_BOARD_VISUAL_STATE.SHOP);
      skin.m_CombatInstance.QueueToUnload(this);
    }
    if (!((UnityEngine.Object) skin.m_TavernInstance != (UnityEngine.Object) null))
      return;
    skin.m_TavernInstance.SetBoardState(TAG_BOARD_VISUAL_STATE.SHOP);
  }

  public void FriendlyPlayerFinisherCalled()
  {
    Entity hero = GameState.Get().GetOpposingSidePlayer().GetHero();
    if (this.m_currentBoardState != TAG_BOARD_VISUAL_STATE.COMBAT || !hero.HasTag(GAME_TAG.TRANSIENT_ENTITY) || hero.GetCurrentHealth() >= 0)
      return;
    this.SetOpponentHeroDefeated();
  }

  public bool SetOpponentHeroDefeated()
  {
    if (!((UnityEngine.Object) this.m_FullBoardSkin.m_CombatInstance != (UnityEngine.Object) null))
      return false;
    this.m_FullBoardSkin.m_CombatInstance.PlayOpponentHeroDefeated();
    return true;
  }

  private void TryStartBattleTransitionAnimations()
  {
    if (!this.m_InBattle || (UnityEngine.Object) this.m_FullBoardSkin.m_CombatInstance != (UnityEngine.Object) null)
      return;
    this.StartBattleTransitionAnimation(this.m_FullBoardSkin);
    this.ToggleLeaderboardFrame(!this.m_FullBoardSkin.m_CombatInstance.HasOwnLeaderboardFrame());
    this.ToggleTableTop(!this.m_FullBoardSkin.m_CombatInstance.HasOwnTableTop());
    this.RunVisualStateAnimators(TAG_BOARD_VISUAL_STATE.COMBAT);
  }

  private void StartBattleTransitionAnimation(BaconBoard.BoardSkinStatus skin)
  {
    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(skin.m_CombatPrefab);
    if (!gameObject.TryGetComponent<BaconBoardSkinBehaviour>(out skin.m_CombatInstance))
    {
      Debug.LogError((object) ("Attempting to get component BaconBoardSkinBehaviour but not found on " + (object) gameObject));
    }
    else
    {
      PlayerLeaderboardManager leaderboardManager = PlayerLeaderboardManager.Get();
      int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
      int winStreak = this.m_CheatWinstreak > 0 ? this.m_CheatWinstreak : leaderboardManager.GetLatestWinStreakForPlayer(friendlyPlayerId);
      if (winStreak > 0)
        skin.m_CombatInstance.RequestWinStreak(winStreak);
      int loseStreakForPlayer = leaderboardManager.GetLatestLoseStreakForPlayer(friendlyPlayerId);
      if (loseStreakForPlayer > 0)
        skin.m_CombatInstance.RequestLoseStreak(loseStreakForPlayer);
      if (this.m_minionsDefeatedCount > 0)
        skin.m_CombatInstance.RequestOpponentMinionPreviouslyDefeatedCount(this.m_minionsDefeatedCount);
      foreach (string minionCardID in this.m_minionsDefeatedByPlayer)
        skin.m_CombatInstance.RequestFriendlyPlayerHasDefeatedMinion(minionCardID);
      foreach (TAG_RACE race in this.m_racesDefeatedByPlayer)
        skin.m_CombatInstance.RequestFriendlyPlayerHasDefeatedRace(race);
      if (GameState.Get().GetFriendlySidePlayer().GetHero().GetRealTimePlayerLeaderboardPlace() <= 4)
        skin.m_CombatInstance.RequestTopFourPlacement();
      if (this.m_CheatHasDefeatedOpponent)
      {
        skin.m_CombatInstance.RequestHasFriendlyPlayerDefeatedOpponent();
      }
      else
      {
        List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo> historyForPlayer = leaderboardManager.GetRecentCombatHistoryForPlayer(friendlyPlayerId);
        if (historyForPlayer != null)
        {
          foreach (PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo recentCombatInfo in historyForPlayer)
          {
            if (recentCombatInfo.isDefeated)
            {
              skin.m_CombatInstance.RequestHasFriendlyPlayerDefeatedOpponent();
              break;
            }
          }
        }
      }
      Entity hero = GameState.Get().GetFriendlySidePlayer().GetHero();
      if (hero != null)
        skin.m_CombatInstance.RequestFriendlyPlayerHealthAtOrBelow(hero.GetDefHealth(), hero.GetCurrentHealth());
      if ((UnityEngine.Object) skin.m_TavernInstance != (UnityEngine.Object) null)
      {
        skin.m_CombatInstance.CopyCornersFromSkin(skin.m_TavernInstance);
        skin.m_TavernInstance.SetBoardState(TAG_BOARD_VISUAL_STATE.COMBAT);
      }
      skin.m_CombatInstance.SetBoardState(TAG_BOARD_VISUAL_STATE.COMBAT);
    }
  }

  public void CheckForHeroHeavyHitBoardEffects(Card sourceCard, Card targetCard)
  {
    if (!((UnityEngine.Object) this.m_FullBoardSkin.m_CombatInstance != (UnityEngine.Object) null))
      return;
    this.m_FullBoardSkin.m_CombatInstance.CheckForHeroHeavyHitBoardEffects(sourceCard, targetCard);
  }

  private void UnloadSkinAssets()
  {
    this.m_ChosenCombatBoardSkinId = 0;
    this.UnloadSkinAsset(this.m_FullBoardSkin);
  }

  private void UnloadSkinAsset(BaconBoard.BoardSkinStatus skin)
  {
    if ((UnityEngine.Object) skin.m_CombatInstance != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) skin.m_CombatInstance.gameObject);
      skin.m_CombatInstance = (BaconBoardSkinBehaviour) null;
    }
    skin.m_CombatPrefab = (GameObject) null;
    if (skin.m_AssetHandleCombat == null)
      return;
    skin.m_AssetHandleCombat.Dispose();
    skin.m_AssetHandleCombat = (AssetHandle<GameObject>) null;
  }

  public void RunVisualStateAnimators(TAG_BOARD_VISUAL_STATE boardState)
  {
    if (this.m_stateChangeCallback != null)
      this.m_stateChangeCallback(boardState);
    if (this.m_BoardStateChangingObjects == null || this.m_BoardStateChangingObjects.Count == 0)
      return;
    foreach (PlayMakerFSM stateChangingObject in this.m_BoardStateChangingObjects)
      stateChangingObject.SetState(EnumUtils.GetString<TAG_BOARD_VISUAL_STATE>(boardState));
  }

  protected void SendBoardVisualStatChangeTelmetry(
    TAG_BOARD_VISUAL_STATE fromBoardState,
    TAG_BOARD_VISUAL_STATE toBoardState,
    DateTime lastBoardStateChangeDateTime)
  {
    int totalSeconds = (int) (DateTime.Now - lastBoardStateChangeDateTime).TotalSeconds;
    TelemetryManager.Client().SendBoardVisualStateChanged(fromBoardState.ToString(), toBoardState.ToString(), totalSeconds);
  }

  private class BoardSkinStatus
  {
    public GameObject m_CombatPrefab;
    public GameObject m_TavernPrefab;
    public AssetHandle<GameObject> m_AssetHandleCombat;
    public AssetHandle<GameObject> m_AssetHandleTavern;
    public BaconBoardSkinBehaviour m_CombatInstance;
    public BaconBoardSkinBehaviour m_TavernInstance;
  }

  private class BoardSkinStatusAndRound
  {
    public BaconBoard.BoardSkinStatus m_Skin;
    public int m_Round;

    public BoardSkinStatusAndRound(BaconBoard.BoardSkinStatus skin, int round)
    {
      this.m_Skin = skin;
      this.m_Round = round;
    }
  }

  public delegate void StateChangeCallback(TAG_BOARD_VISUAL_STATE newState);
}
