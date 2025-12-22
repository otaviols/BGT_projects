using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.UI;
using PegasusGame;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLeaderboardCard : HistoryItem
{
  public Color m_deadColor;
  public Color m_enemyBorderColor;
  public Color m_selfBorderColor;
  public bool m_useMergedRecentCombatPanelPrefab;
  private const float RECENT_ACTION_PANEL_SCALE = 0.35f;
  private const string HISTORY_PLAYER_TILE_PREFAB = "HistoryTile_Player.prefab:8a2a1b0cd86ca4d4ba3e4b565ca00e0c";
  private const string RECENT_COMBAT_PANEL_PREFAB = "PlayerLeaderBoardRecentActionsPanel.prefab:c4b73d23d6a0cd6469360d1436ac5529";
  private const string RECENT_COMBAT_PANEL_PREFAB_DAMAGE_CAP = "PlayerLeaderBoardRecentActionsPanel_DamageCap.prefab:da3a2bf5cefb4a0439f93a0897ffce59";
  private const string RECENT_COMBAT_PANEL_PREFAB_MERGED = "PlayerLeaderBoardRecentActionsPanelWidget.prefab:83cf143fe4ebe7e4eb5c5a79341c280f";
  private const string HISTORY_MOUSEOVER_AUDIO_PREFAB = "history_event_mouseover.prefab:0bc4f1638257a264a9b02e811c0a61b5";
  private const string CARD_PREFAB = "Card_Hand_Ability.prefab:3c3f5189f0d0b3745a1c1ca21d41efe0";
  private const string MAIN_ACTOR_BONE_NAME = "MainActorBone";
  private const string HERO_POWER_ACTOR_BONE_NAME = "HeroPowerActorBone";
  private const string HERO_POWER_QUEST_REWARD_ACTOR_BONE_NAME = "HeroPowerActorBone";
  private const string CUSTOM_CARD_ACTOR_BONE_NAME = "CustomCardActorBone";
  private const string HERO_BUDDY_ACTOR_BONE_NAME = "HeroBuddyActorBone";
  private const string QUEST_REWARD_ACTOR_BONE_NAME = "CustomCardActorBone";
  private const string HISTORY_ACTOR_BONE_NAME = "HistoryPanelBone";
  private const string HISTORY_ACTOR_WITH_CUSTOM_CARD_BONE_NAME = "HistoryPanelBoneWithCustomCard";
  private const string MAIN_ACTOR_UPPER_LIMIT_BONE_NAME = "HighestMainActorZWorld";
  private const string MAIN_ACTOR_LOWER_LIMIT_BONE_NAME = "LowestMainActorZWorld";
  private const string BG_REWARD_VFX = "BGRewardVFX";
  private bool m_useDamageCapPanel;
  private readonly PlatformDependentValue<string> PLATFORM_DEPENDENT_BONE_SUFFIX = new PlatformDependentValue<string>(PlatformCategory.Screen)
  {
    PC = "PC",
    Tablet = "PC",
    Phone = "Phone"
  };
  public Player m_player;
  public Entity m_playerHeroEntity;
  private Material m_fullTileMaterial;
  private bool m_mousedOver;
  private bool m_halfSize;
  private bool m_hasBeenShown;
  private bool m_isShowingOddPlayerFx;
  private bool m_isShowingDiabloPlayerFx;
  private bool m_gameEntityMousedOver;
  private bool m_heroNameInitialized;
  private bool m_techLevelDirty = true;
  private bool m_triplesDirty = true;
  private bool m_racesDirty = true;
  private bool m_recentCombatsDirty = true;
  private bool m_heroBuddyEnabledDirty = true;
  private bool m_questRewardDirty = true;
  private bool m_bigCardFinishedCallbackHasRun;
  private HistoryManager.BigCardFinishedCallback m_bigCardFinishedCallback;
  private bool m_bigCardCountered;
  private bool m_bigCardWaitingForSecret;
  private bool m_bigCardFromMetaData;
  private Entity m_bigCardPostTransformedEntity;
  private int m_displayTimeMS;
  private Actor m_heroPowerActor;
  private Actor m_heroBuddyActor;
  private Actor m_heroPowerQuestRewardActor;
  private Actor m_questRewardActor;
  private VisualController m_recentCombatsPanelController;
  private PlayerLeaderboardRecentCombatsPanel m_recentCombatsPanel;
  private PlayerLeaderboardRecentCombatsPanel m_recentCombatsPanelNormal;
  private PlayerLeaderboardRecentCombatsPanel m_recentCombatsPanelDamageCap;
  private List<PlayerLeaderboardInformationPanel> m_additionalInfoPanels;
  private Map<TAG_RACE, int> m_raceCounts = new Map<TAG_RACE, int>();
  private bool m_isNextOpponent;

  public PlayerLeaderboardTile m_PlayerLeaderboardTile => this.m_tileActor.GetComponent<PlayerLeaderboardTile>();

  public void Initialize(Entity playerHeroEntity) => this.m_playerHeroEntity = playerHeroEntity;

  public bool HasBeenShown() => this.m_hasBeenShown;

  public void MarkAsShown()
  {
    if (this.m_hasBeenShown)
      return;
    this.m_hasBeenShown = true;
  }

  public void SetTechLevelDirty() => this.m_techLevelDirty = true;

  public void SetTriplesDirty() => this.m_triplesDirty = true;

  public void SetBattlegroundHeroBuddyEnabledDirty() => this.m_heroBuddyEnabledDirty = true;

  public void SetBGQuestRewardDirty() => this.m_questRewardDirty = true;

  public void SetRacesDirty() => this.m_racesDirty = true;

  public void SetRecentCombatsDirty() => this.m_recentCombatsDirty = true;

  public void LoadTile(HistoryTileInitInfo info)
  {
    this.m_entity = info.m_entity;
    this.m_portraitTexture = info.m_portraitTexture;
    this.m_portraitGoldenMaterial = info.m_portraitGoldenMaterial;
    this.SetCardDef(info.m_cardDef);
    this.m_fullTileMaterial = info.m_fullTileMaterial;
    this.m_splatAmount = info.m_splatAmount;
    this.m_dead = info.m_dead;
    this.LoadTileImpl("HistoryTile_Player.prefab:8a2a1b0cd86ca4d4ba3e4b565ca00e0c");
    this.LoadMainCardActor();
    this.m_mainCardActor.SetVisibility(false, false);
  }

  public void RefreshTileVisuals(HistoryTileInitInfo info)
  {
    this.m_entity = info.m_entity;
    this.m_portraitTexture = info.m_portraitTexture;
    this.m_portraitGoldenMaterial = info.m_portraitGoldenMaterial;
    this.m_fullTileMaterial = info.m_fullTileMaterial;
    this.SetCardDef(info.m_cardDef);
    this.m_splatAmount = info.m_splatAmount;
    this.m_dead = info.m_dead;
    this.RefreshTileVisuals();
  }

  public void PauseHealthUpdates()
  {
    if (!(this.m_mainCardActor is PlayerLeaderboardMainCardActor))
      return;
    ((PlayerLeaderboardMainCardActor) this.m_mainCardActor).PauseHealthUpdates();
  }

  public void UpdateTileHealth()
  {
    if (this.m_mainCardActor is PlayerLeaderboardMainCardActor)
      ((PlayerLeaderboardMainCardActor) this.m_mainCardActor).ResumeHealthUpdates();
    float healthPercent = Mathf.Clamp01((float) this.m_playerHeroEntity.GetRealTimeRemainingHP() / (float) this.m_playerHeroEntity.GetHealth());
    if ((double) healthPercent == 0.0)
      this.m_tileActor.GetMeshRenderer().GetMaterial(1).color = this.m_deadColor;
    this.m_PlayerLeaderboardTile.SetCurrentHealth(healthPercent);
  }

  public void SetNextOpponentState(bool active)
  {
    this.m_isNextOpponent = active;
    this.m_PlayerLeaderboardTile.SetTilePopOutActive(active);
    this.UpdateDiabloPlayerFightFX();
    this.UpdateOddPlayerOutFx(active);
  }

  public void UpdateDiabloPlayerFightFX(int oldValue = -1, int newValue = -1)
  {
    if (newValue == -1)
      newValue = GameState.Get().GetGameEntity().GetTag(GAME_TAG.BACON_DIABLO_FIGHT_DIABLO_PLAYER_ID);
    if (oldValue == newValue)
      return;
    int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
    bool flag = false;
    if (this.m_playerHeroEntity.GetCurrentHealth() > 0)
    {
      if (friendlyPlayerId != newValue && newValue == this.m_PlayerLeaderboardTile.GetOwnerId())
        flag = true;
      else if (friendlyPlayerId == newValue && newValue != this.m_PlayerLeaderboardTile.GetOwnerId())
        flag = true;
    }
    if (flag)
    {
      Card card = this.m_playerHeroEntity.GetCard();
      if ((UnityEngine.Object) card == (UnityEngine.Object) null)
        return;
      Spell spell = this.m_tileActor.GetSpell(SpellType.BACON_DIABLO_PLAYER);
      spell.RemoveAllTargets();
      spell.AddTarget(card.gameObject);
      spell.SetSource(card.gameObject);
      Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
      if (friendlySidePlayer != null && friendlySidePlayer.GetHero() != null && (UnityEngine.Object) friendlySidePlayer.GetHero().GetCard() != (UnityEngine.Object) null)
        spell.SetSource(friendlySidePlayer.GetHero().GetCard().gameObject);
      if (!this.m_isShowingDiabloPlayerFx)
      {
        spell.ChangeState(SpellStateType.BIRTH);
        this.m_isShowingDiabloPlayerFx = true;
      }
      else
        spell.ActivateState(SpellStateType.ACTION);
    }
    else
    {
      if (flag || !this.m_isShowingDiabloPlayerFx)
        return;
      this.m_tileActor.ActivateSpellDeathState(SpellType.BACON_DIABLO_PLAYER);
      this.m_isShowingDiabloPlayerFx = false;
    }
  }

  public void UpdateOddPlayerOutFx(bool isNextOpponent)
  {
    if (!this.HasBeenShown())
      return;
    bool flag = isNextOpponent && GameState.Get().GetFriendlySidePlayer().HasTag(GAME_TAG.BACON_ODD_PLAYER_OUT);
    if (flag && !this.m_isShowingOddPlayerFx)
    {
      Card card = this.m_playerHeroEntity.GetCard();
      if ((UnityEngine.Object) card == (UnityEngine.Object) null)
        return;
      this.m_isShowingOddPlayerFx = true;
      Spell spell = this.m_tileActor.GetSpell(SpellType.BACON_ODD_PLAYER);
      spell.AddTarget(card.gameObject);
      spell.ChangeState(SpellStateType.BIRTH);
    }
    else
    {
      if (flag || !this.m_isShowingOddPlayerFx)
        return;
      this.m_tileActor.ActivateSpellDeathState(SpellType.BACON_ODD_PLAYER);
      this.m_isShowingOddPlayerFx = false;
    }
  }

  public bool GetNextOpponentState() => this.m_isNextOpponent;

  public void SetCurrentOpponentState(bool active) => this.m_PlayerLeaderboardTile.SetSwordsIconActive(active);

  public void SetBorderColor(bool isEnemy) => this.m_tileActor.GetMeshRenderer().GetMaterial().color = isEnemy ? this.m_enemyBorderColor : this.m_selfBorderColor;

  public void RefreshTileVisuals()
  {
    if ((UnityEngine.Object) this.m_tileActor == (UnityEngine.Object) null)
      return;
    Material[] materials = new Material[2]
    {
      this.m_tileActor.GetMeshRenderer().GetMaterial(),
      null
    };
    if ((UnityEngine.Object) this.m_fullTileMaterial != (UnityEngine.Object) null)
    {
      materials[1] = this.m_fullTileMaterial;
      this.m_tileActor.GetMeshRenderer().SetMaterials(materials);
    }
    else
      this.m_tileActor.GetMeshRenderer().GetMaterial(1).mainTexture = this.m_portraitTexture;
    this.SetupHeroBuddy();
  }

  private void LoadTileImpl(string actorPath)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit(actorPath), (AssetLoadingOptions) 2);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Debug.LogWarningFormat("HistoryCard.LoadTileImpl() - FAILED to load actor \"{0}\"", (object) actorPath);
    }
    else
    {
      Actor component = gameObject.GetComponent<Actor>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("HistoryCard.LoadTileImpl() - ERROR actor \"{0}\" has no Actor component", (object) actorPath);
      }
      else
      {
        this.m_tileActor = component;
        this.m_tileActor.transform.parent = this.transform;
        TransformUtil.Identity((Component) this.m_tileActor.transform);
        this.m_tileActor.transform.localScale = PlayerLeaderboardManager.Get().transform.localScale;
        this.RefreshTileVisuals();
        foreach (Renderer componentsInChild in this.m_tileActor.GetMeshRenderer().GetComponentsInChildren<Renderer>())
        {
          if (!componentsInChild.CompareTag(HistoryItem.RENDERER_TAG))
            componentsInChild.GetMaterial().color = Board.Get().m_HistoryTileColor;
        }
        this.m_tileActor.GetMeshRenderer().GetMaterial(1).color = Board.Get().m_HistoryTileColor;
        this.UpdateTileHealth();
        int playerId = GameState.Get().GetFriendlySidePlayer().GetPlayerId();
        int tag = this.m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID);
        if (tag == 0)
          tag = this.m_playerHeroEntity.GetTag(GAME_TAG.CONTROLLER);
        this.SetBorderColor(tag != playerId);
        this.m_PlayerLeaderboardTile.SetOwnerId(tag);
      }
    }
  }

  public void NotifyMousedOver()
  {
    if (this.m_mousedOver || (UnityEngine.Object) this == (UnityEngine.Object) HistoryManager.Get().GetCurrentBigCard())
      return;
    this.m_mousedOver = true;
    if (PlayerLeaderboardManager.Get().IsNewlyMousedOver())
      SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit("history_event_mouseover.prefab:0bc4f1638257a264a9b02e811c0a61b5"), this.m_tileActor.gameObject);
    this.UpdateDamageCapState();
    this.LoadMainCardActor();
    this.ShowTile();
  }

  private void UpdateDamageCapState()
  {
    bool useDamageCapPanel = this.m_useDamageCapPanel;
    this.m_useDamageCapPanel = GameState.Get().GetGameEntity().GetTag(GAME_TAG.BACON_COMBAT_DAMAGE_CAP) != 0;
    if (!this.m_useMergedRecentCombatPanelPrefab)
      this.m_recentCombatsPanel = this.m_useDamageCapPanel ? this.m_recentCombatsPanelDamageCap : this.m_recentCombatsPanelNormal;
    if (this.m_useMergedRecentCombatPanelPrefab && (UnityEngine.Object) this.m_recentCombatsPanelController == (UnityEngine.Object) null)
    {
      this.LoadRecentCombatsPanel();
      LayerUtils.SetLayer((Component) this.m_recentCombatsPanelController, GameLayer.Tooltip);
    }
    else if (!this.m_useMergedRecentCombatPanelPrefab && (UnityEngine.Object) this.m_recentCombatsPanel == (UnityEngine.Object) null)
    {
      this.LoadRecentCombatsPanel();
      LayerUtils.SetLayer((Component) this.m_recentCombatsPanel, GameLayer.Tooltip);
    }
    if (this.m_useDamageCapPanel == useDamageCapPanel)
      return;
    if (this.m_useMergedRecentCombatPanelPrefab)
      this.m_recentCombatsPanelController.SetState(this.m_useDamageCapPanel ? "DAMAGE_CAP" : "DEFAULT");
    this.SetTechLevelDirty();
    this.SetRacesDirty();
    this.SetRecentCombatsDirty();
    this.SetTriplesDirty();
  }

  public void NotifyMousedOut()
  {
    if (!this.m_mousedOver)
      return;
    this.m_mousedOver = false;
    if (this.m_gameEntityMousedOver)
    {
      GameState.Get().GetGameEntity().NotifyOfHistoryTokenMousedOut();
      this.m_gameEntityMousedOver = false;
    }
    TooltipPanelManager.Get().HideKeywordHelp();
    if ((bool) (UnityEngine.Object) this.m_mainCardActor)
    {
      this.m_mainCardActor.ActivateAllSpellsDeathStates();
      this.m_mainCardActor.Hide();
    }
    if ((bool) (UnityEngine.Object) this.m_heroPowerActor)
    {
      this.m_heroPowerActor.ActivateAllSpellsDeathStates();
      this.m_heroPowerActor.Hide();
    }
    if ((bool) (UnityEngine.Object) this.m_heroBuddyActor)
      this.m_heroBuddyActor.gameObject.SetActive(false);
    if ((bool) (UnityEngine.Object) this.m_questRewardActor)
      this.m_questRewardActor.gameObject.SetActive(false);
    if ((bool) (UnityEngine.Object) this.m_heroPowerQuestRewardActor)
      this.m_heroPowerQuestRewardActor.gameObject.SetActive(false);
    if ((bool) (UnityEngine.Object) this.m_recentCombatsPanelController && this.m_useMergedRecentCombatPanelPrefab)
    {
      this.m_recentCombatsPanelController.gameObject.SetActive(false);
    }
    else
    {
      if (!(bool) (UnityEngine.Object) this.m_recentCombatsPanel || this.m_useMergedRecentCombatPanelPrefab)
        return;
      this.m_recentCombatsPanel.gameObject.SetActive(false);
    }
  }

  public void LoadRecentCombatsPanel()
  {
    string str = "PlayerLeaderBoardRecentActionsPanelWidget.prefab:83cf143fe4ebe7e4eb5c5a79341c280f";
    if (!this.m_useMergedRecentCombatPanelPrefab)
      str = this.m_useDamageCapPanel ? "PlayerLeaderBoardRecentActionsPanel_DamageCap.prefab:da3a2bf5cefb4a0439f93a0897ffce59" : "PlayerLeaderBoardRecentActionsPanel.prefab:c4b73d23d6a0cd6469360d1436ac5529";
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit(str), (AssetLoadingOptions) 0);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Debug.LogWarningFormat("PlayerLeaderboardCard.LoadRecentCombatsPanel() - FAILED to load GameObject \"{0}\"", (object) str);
    }
    else
    {
      if (this.m_useMergedRecentCombatPanelPrefab)
      {
        for (int index = 0; index < gameObject.transform.childCount && (UnityEngine.Object) this.m_recentCombatsPanel == (UnityEngine.Object) null; ++index)
          this.m_recentCombatsPanel = gameObject.transform.GetChild(index).GetComponent<PlayerLeaderboardRecentCombatsPanel>();
        if ((UnityEngine.Object) this.m_recentCombatsPanel == (UnityEngine.Object) null)
          Debug.Log((object) "PlayerLeaderboardCard - LoadRecentCombatsPanel - recentCombatPanel not loaded");
        this.m_recentCombatsPanelController = gameObject.GetComponent<VisualController>();
        if ((UnityEngine.Object) this.m_recentCombatsPanelController == (UnityEngine.Object) null)
        {
          Debug.LogWarningFormat("PlayerLeaderboardCard.LoadRecentCombatsPanel() - FAILED to find Visual Controller");
          return;
        }
        if (this.m_useDamageCapPanel)
          this.m_recentCombatsPanelController.SetState("DAMAGE_CAP");
        else
          this.m_recentCombatsPanelController.SetState("DEFAULT");
      }
      else if (this.m_useDamageCapPanel)
      {
        this.m_recentCombatsPanelDamageCap = gameObject.GetComponent<PlayerLeaderboardRecentCombatsPanel>();
        this.m_recentCombatsPanel = this.m_recentCombatsPanelDamageCap;
      }
      else
      {
        this.m_recentCombatsPanelNormal = gameObject.GetComponent<PlayerLeaderboardRecentCombatsPanel>();
        this.m_recentCombatsPanel = this.m_recentCombatsPanelNormal;
      }
      if ((UnityEngine.Object) this.m_recentCombatsPanel == (UnityEngine.Object) null)
        Debug.LogWarningFormat("PlayerLeaderboardCard.LoadRecentCombatsPanel() - ERROR GameObject \"{0}\" has no PlayerLeaderboardRecentCombatsPanel component", (object) str);
      else if (this.m_useMergedRecentCombatPanelPrefab)
      {
        this.m_recentCombatsPanelController.transform.parent = this.transform;
        TransformUtil.Identity((Component) this.m_recentCombatsPanelController.transform);
        TransformUtil.SetLocalScaleX(this.m_recentCombatsPanelController.gameObject, 0.35f);
        TransformUtil.SetLocalScaleY(this.m_recentCombatsPanelController.gameObject, 0.35f);
        TransformUtil.SetLocalScaleZ(this.m_recentCombatsPanelController.gameObject, 0.35f);
      }
      else
      {
        this.m_recentCombatsPanel.transform.parent = this.transform;
        TransformUtil.Identity((Component) this.m_recentCombatsPanel.transform);
        TransformUtil.SetLocalScaleX(this.m_recentCombatsPanel.gameObject, 0.35f);
        TransformUtil.SetLocalScaleY(this.m_recentCombatsPanel.gameObject, 0.35f);
        TransformUtil.SetLocalScaleZ(this.m_recentCombatsPanel.gameObject, 0.35f);
      }
    }
  }

  public void RefreshMainCardActor()
  {
    if ((UnityEngine.Object) this.m_mainCardActor == (UnityEngine.Object) null)
      return;
    this.m_mainCardActor.SetCardDefFromEntity(this.m_entity);
    this.m_mainCardActor.SetPremium(this.m_entity.GetPremiumType());
    this.m_mainCardActor.SetWatermarkCardSetOverride(this.m_entity.GetWatermarkCardSetOverride());
    this.m_mainCardActor.SetHistoryItem((HistoryItem) this);
    this.m_mainCardActor.UpdateAllComponents();
    this.m_mainCardActor.GetAttackObject().Hide();
  }

  public void LoadMainCardActor()
  {
    if ((bool) (UnityEngine.Object) this.m_mainCardActor)
      return;
    string str1 = "Bacon_Leaderboard_Hero.prefab:776977f5238a24647adcd67933f7d4b0";
    string str2 = "History_HeroPower.prefab:e73edf8ccea2b11429093f7a448eef53";
    GameObject gameObject1 = AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit(str1), (AssetLoadingOptions) 2);
    GameObject gameObject2 = AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit(str2), (AssetLoadingOptions) 2);
    if ((UnityEngine.Object) gameObject1 == (UnityEngine.Object) null)
    {
      Debug.LogWarningFormat("PlayerLeaderboardCard.LoadMainCardActor() - FAILED to load actor \"{0}\"", (object) str1);
    }
    else
    {
      Actor component1 = gameObject1.GetComponent<Actor>();
      Actor component2 = gameObject2.GetComponent<Actor>();
      if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("PlayerLeaderboardCard.LoadMainCardActor() - ERROR actor \"{0}\" has no Actor component", (object) str1);
      }
      else
      {
        this.m_mainCardActor = component1;
        this.m_heroPowerActor = component2;
        this.RefreshMainCardActor();
        LayerUtils.SetLayer((Component) this.m_mainCardActor, GameLayer.Tooltip);
        if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
        {
          LayerUtils.SetLayer((Component) component2, GameLayer.Tooltip);
          component2.Hide();
          this.SetHeroPower(this.m_entity);
        }
        this.SetupHeroBuddy();
        this.m_questRewardDirty = !this.SetupBGQuestRewardCards();
      }
    }
  }

  public void SetHeroPower(Entity hero)
  {
    if ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
      return;
    Player controller = hero.GetController();
    if (controller.GetHero() == hero)
    {
      Entity heroPower = controller.GetHeroPower();
      if (heroPower == null)
        return;
      using (DefLoader.DisposableCardDef cardDef = heroPower.ShareDisposableCardDef())
        this.SetHeroPower(heroPower, cardDef, heroPower.GetEntityDef());
    }
    else
    {
      Entity entity = (Entity) null;
      if (hero.HasTag(GAME_TAG.HERO_POWER_ENTITY))
        entity = GameState.Get().GetEntity(hero.GetTag(GAME_TAG.HERO_POWER_ENTITY));
      if (entity != null)
      {
        using (DefLoader.DisposableCardDef cardDef = entity.ShareDisposableCardDef())
          this.SetHeroPower(entity, cardDef, entity.GetEntityDef());
      }
      else
      {
        string cardId = GameUtils.GetHeroPowerCardIdFromHero(hero.GetCardId());
        if (hero.HasTag(GAME_TAG.HERO_POWER))
          cardId = GameUtils.TranslateDbIdToCardId(hero.GetTag(GAME_TAG.HERO_POWER));
        if (cardId == null)
          return;
        using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardId))
          this.SetHeroPower((Entity) null, fullDef?.DisposableCardDef, fullDef?.EntityDef);
      }
    }
  }

  private void SetupActor(
    Actor actor,
    Entity entity,
    DefLoader.DisposableCardDef cardDef,
    DefLoader.DisposableFullDef fullDef,
    EntityDef entityDef,
    int rewardMinionType = 0,
    int rewardCardDBID = 0)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
      return;
    if (entity == null)
      actor.SetEntityDef(entityDef);
    else
      actor.SetEntityDef((EntityDef) null);
    actor.SetEntity(entity);
    if (cardDef != null)
      actor.SetCardDef(cardDef);
    if (fullDef != null)
      actor.SetFullDef(fullDef);
    if (entity == null)
      actor.SetPremium(this.m_entity.GetPremiumType());
    actor.transform.parent = this.transform;
    TransformUtil.Identity((Component) actor.transform);
    if (rewardMinionType != 0)
    {
      string text = string.Format(fullDef.EntityDef.GetCardTextInHand(), (object) GameStrings.GetRaceNameBattlegrounds((TAG_RACE) rewardMinionType));
      actor.SetCardDefPowerTextOverride(text);
    }
    if (rewardCardDBID != 0)
    {
      string cardTextInHand = fullDef.EntityDef.GetCardTextInHand();
      CardDbfRecord record = GameDbf.Card.GetRecord(rewardCardDBID);
      if (record != null)
      {
        string name = (string) record.Name;
        string text = string.Format(cardTextInHand, (object) name);
        actor.SetCardDefPowerTextOverride(text);
      }
    }
    actor.UpdateAllComponents();
    if (!this.m_mousedOver)
      return;
    this.UpdateHoverStatePosition();
  }

  private bool InitQuestRewards(
    ref Actor actor,
    int cardId,
    int rewardMinionType,
    int rewardCardDBID)
  {
    if (cardId == 0)
      return true;
    using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardId))
    {
      if (fullDef == null || fullDef.EntityDef == null || (UnityEngine.Object) fullDef.CardDef == (UnityEngine.Object) null)
      {
        Log.Spells.PrintError("PlayerLeaderboardCard.LoadMainCardActor(): Unable to load def for card ID {0}.", (object) cardId);
        return false;
      }
      GameObject gameObject = AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit("Card_Hand_Ability.prefab:3c3f5189f0d0b3745a1c1ca21d41efe0"), (AssetLoadingOptions) 2);
      if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
      {
        Log.Spells.PrintError("PlayerLeaderboardCard.LoadMainCardActor(): Unable to load Hand Actor for entity def {0}.", (object) fullDef.EntityDef);
        return false;
      }
      actor = gameObject.GetComponentInChildren<Actor>();
      actor.Hide();
      if ((UnityEngine.Object) actor != (UnityEngine.Object) null)
        this.SetupActor(actor, (Entity) null, (DefLoader.DisposableCardDef) null, fullDef, fullDef.EntityDef, rewardMinionType, rewardCardDBID);
      LayerUtils.SetLayer((Component) actor, GameLayer.Tooltip);
      return true;
    }
  }

  private bool SetupBGQuestRewardCards()
  {
    if ((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_questRewardActor.gameObject);
      this.m_questRewardActor = (Actor) null;
    }
    if ((UnityEngine.Object) this.m_heroPowerQuestRewardActor != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_heroPowerQuestRewardActor.gameObject);
      this.m_heroPowerQuestRewardActor = (Actor) null;
    }
    if (GameState.Get() == null || !GameState.Get().BattlegroundsAllowQuests())
      return true;
    if (this.m_playerHeroEntity == null)
    {
      Debug.LogWarning((object) "SetupBGQuestRewardCards - Player Hero Entity is null");
      return false;
    }
    Actor actor1 = (Actor) null;
    Actor actor2 = (Actor) null;
    int tag1 = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_HEROPOWER_QUEST_REWARD_DATABASE_ID);
    int tag2 = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_QUEST_REWARD_DATABASE_ID);
    int tag3 = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_REWARD_MINION_TYPE);
    int tag4 = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_HEROPOWER_REWARD_MINION_TYPE);
    int tag5 = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_REWARD_CARD_DBID);
    int tag6 = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_HEROPOWER_REWARD_CARD_DBID);
    int num = this.InitQuestRewards(ref actor1, tag1, tag4, tag6) & this.InitQuestRewards(ref actor2, tag2, tag3, tag5) ? 1 : 0;
    this.m_heroPowerQuestRewardActor = actor1;
    this.m_questRewardActor = actor2;
    return num != 0;
  }

  private void SetupHeroBuddy()
  {
    if ((UnityEngine.Object) this.m_heroBuddyActor != (UnityEngine.Object) null)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_heroBuddyActor.gameObject);
      this.m_heroBuddyActor = (Actor) null;
    }
    if (GameState.Get() != null && !GameState.Get().BattlegroundAllowBuddies())
      return;
    Actor c = (Actor) null;
    int heroBuddyCardId = this.m_entity.GetHeroBuddyCardId();
    if (heroBuddyCardId != 0)
    {
      using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(heroBuddyCardId))
      {
        if (fullDef?.EntityDef == null || (UnityEngine.Object) fullDef?.CardDef == (UnityEngine.Object) null)
        {
          Log.Spells.PrintError("PlayerLeaderboardCard.LoadMainCardActor(): Unable to load def for card ID {0}.", (object) heroBuddyCardId);
          return;
        }
        GameObject gameObject = AssetLoader.Get().InstantiatePrefab(AssetReference.op_Implicit(ActorNames.GetHandActor(fullDef.EntityDef, this.m_entity.GetPremiumType())), (AssetLoadingOptions) 2);
        if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
        {
          Log.Spells.PrintError("PlayerLeaderboardCard.LoadMainCardActor(): Unable to load Hand Actor for entity def {0}.", (object) fullDef.EntityDef);
          return;
        }
        c = gameObject.GetComponentInChildren<Actor>();
        if ((UnityEngine.Object) c != (UnityEngine.Object) null)
        {
          LayerUtils.SetLayer((Component) c, GameLayer.Tooltip);
          c.SetFullDef(fullDef);
          c.SetPremium(this.m_entity.GetPremiumType());
          if (c.UseCoinManaGem())
            c.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
          c.transform.parent = this.transform;
          TransformUtil.Identity((Component) c.transform);
          c.UpdateAllComponents();
          if (this.m_mousedOver)
            this.UpdateHoverStatePosition();
          c.gameObject.SetActive(false);
        }
      }
    }
    this.m_heroBuddyActor = c;
    this.m_heroBuddyEnabledDirty = false;
  }

  private void SetHeroPower(
    Entity entity,
    DefLoader.DisposableCardDef cardDef,
    EntityDef entityDef)
  {
    this.SetupActor(this.m_heroPowerActor, entity, cardDef, (DefLoader.DisposableFullDef) null, entityDef);
  }

  private void ShowTile()
  {
    if (!this.m_mousedOver)
    {
      this.m_mainCardActor.Hide();
      if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
        this.m_heroPowerActor.Hide();
      if ((UnityEngine.Object) this.m_heroBuddyActor != (UnityEngine.Object) null)
        this.m_heroBuddyActor.gameObject.SetActive(false);
      if ((UnityEngine.Object) this.m_recentCombatsPanel != (UnityEngine.Object) null && !this.m_useMergedRecentCombatPanelPrefab)
      {
        this.m_recentCombatsPanel.gameObject.SetActive(false);
      }
      else
      {
        if (!((UnityEngine.Object) this.m_recentCombatsPanelController != (UnityEngine.Object) null) || !this.m_useMergedRecentCombatPanelPrefab)
          return;
        this.m_recentCombatsPanelController.gameObject.SetActive(false);
      }
    }
    else
    {
      this.m_mainCardActor.Show();
      if (GameState.Get().GetGameEntity() is TB_BaconShop_Tutorial)
      {
        if ((UnityEngine.Object) this.m_recentCombatsPanel != (UnityEngine.Object) null && !this.m_useMergedRecentCombatPanelPrefab)
          this.m_recentCombatsPanel.gameObject.SetActive(false);
        else if ((UnityEngine.Object) this.m_recentCombatsPanelController != (UnityEngine.Object) null && this.m_useMergedRecentCombatPanelPrefab)
          this.m_recentCombatsPanelController.gameObject.SetActive(false);
        if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
        {
          this.m_heroPowerActor.Hide();
          this.m_heroPowerActor.UseCoinManaGem();
        }
        if ((UnityEngine.Object) this.m_heroBuddyActor != (UnityEngine.Object) null)
          this.m_heroBuddyActor.gameObject.SetActive(false);
      }
      else
      {
        if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
          this.m_heroPowerActor.Show();
        if (this.m_heroBuddyEnabledDirty)
          this.SetupHeroBuddy();
        if (this.m_questRewardDirty)
          this.m_questRewardDirty = !this.SetupBGQuestRewardCards();
        this.UpdateBGQuestRewards();
        if ((UnityEngine.Object) this.m_heroBuddyActor != (UnityEngine.Object) null)
        {
          this.m_heroBuddyActor.gameObject.SetActive(true);
          this.m_heroBuddyActor.Show();
          Spell spell = this.m_heroBuddyActor.GetSpell(SpellType.TECH_LEVEL_MANA_GEM);
          if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
          {
            spell.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("TechLevel").Value = this.m_heroBuddyActor.GetEntityDef().GetTechLevel();
            spell.ActivateState(SpellStateType.BIRTH);
          }
        }
        if ((UnityEngine.Object) this.m_recentCombatsPanel != (UnityEngine.Object) null && !this.m_useMergedRecentCombatPanelPrefab)
          this.m_recentCombatsPanel.gameObject.SetActive(true);
        else if ((UnityEngine.Object) this.m_recentCombatsPanelController != (UnityEngine.Object) null && this.m_useMergedRecentCombatPanelPrefab)
          this.m_recentCombatsPanelController.gameObject.SetActive(true);
      }
      this.InitializeMainCardActor();
      this.m_mainCardActor.SetActorState(ActorStateType.CARD_IDLE);
      this.DisplaySpells();
      if (!this.m_heroNameInitialized)
        this.m_heroNameInitialized = !this.RefreshMainCardName();
      this.UpdateHoverStatePosition();
      if (this.m_recentCombatsDirty)
        this.m_recentCombatsDirty = !this.UpdateRecentCombats();
      if (this.m_techLevelDirty)
        this.m_techLevelDirty = !this.UpdateTechLevel();
      if (this.m_triplesDirty)
        this.m_triplesDirty = !this.UpdateTriples();
      if (!this.m_racesDirty)
        return;
      this.m_racesDirty = !this.UpdateRaces();
    }
  }

  private string GetHistoryActorBoneName()
  {
    GameState gameState = GameState.Get();
    if (gameState == null || this.m_playerHeroEntity == null)
      return "HistoryPanelBone";
    if (gameState.BattlegroundAllowBuddies())
      return "HistoryPanelBoneWithCustomCard";
    if (!gameState.BattlegroundsAllowQuests())
      return "HistoryPanelBone";
    int num = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_HEROPOWER_QUEST_REWARD_DATABASE_ID) != 0 ? 1 : 0;
    bool flag = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_QUEST_REWARD_DATABASE_ID) != 0;
    return num == 0 && !flag ? "HistoryPanelBone" : "HistoryPanelBoneWithCustomCard";
  }

  private string GetHeroPowerQuestRewardBoneName() => GameState.Get() != null && GameState.Get().BattlegroundsAllowQuests() && this.m_playerHeroEntity != null && this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_QUEST_REWARD_DATABASE_ID) == 0 ? "CustomCardActorBone" : "HeroPowerActorBone";

  private void UpdateHoverStatePosition()
  {
    GameObject bone1 = this.m_tileActor.FindBone("MainActorBone" + (string) this.PLATFORM_DEPENDENT_BONE_SUFFIX);
    GameObject bone2 = this.m_tileActor.FindBone("HeroPowerActorBone" + (string) this.PLATFORM_DEPENDENT_BONE_SUFFIX);
    GameObject bone3 = this.m_tileActor.FindBone("HeroBuddyActorBone" + (string) this.PLATFORM_DEPENDENT_BONE_SUFFIX);
    GameObject bone4 = this.m_tileActor.FindBone(this.GetHistoryActorBoneName() + (string) this.PLATFORM_DEPENDENT_BONE_SUFFIX);
    GameObject bone5 = this.m_tileActor.FindBone("HighestMainActorZWorld" + (string) this.PLATFORM_DEPENDENT_BONE_SUFFIX);
    GameObject bone6 = this.m_tileActor.FindBone("LowestMainActorZWorld" + (string) this.PLATFORM_DEPENDENT_BONE_SUFFIX);
    GameObject bone7 = this.m_tileActor.FindBone("CustomCardActorBone" + (string) this.PLATFORM_DEPENDENT_BONE_SUFFIX);
    GameObject bone8 = this.m_tileActor.FindBone(this.GetHeroPowerQuestRewardBoneName() + (string) this.PLATFORM_DEPENDENT_BONE_SUFFIX);
    this.m_mainCardActor.transform.position = new Vector3(this.transform.position.x + bone1.transform.localPosition.x, this.transform.position.y + bone1.transform.localPosition.y, this.GetZForThisTilesMouseOverCard(this.transform.position.z, bone1.transform.localPosition.z, bone5.transform.localPosition.z, bone6.transform.localPosition.z));
    this.m_mainCardActor.transform.localScale = bone1.transform.localScale;
    if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null && this.m_heroPowerActor.IsShown())
    {
      this.m_heroPowerActor.transform.position = new Vector3(this.transform.position.x + bone2.transform.localPosition.x, this.transform.position.y + bone2.transform.localPosition.y, this.m_mainCardActor.transform.position.z + bone2.transform.localPosition.z);
      this.m_heroPowerActor.transform.localScale = bone2.transform.localScale;
      if (this.m_heroPowerActor.UseCoinManaGem())
        this.m_heroPowerActor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
    }
    if ((UnityEngine.Object) this.m_heroBuddyActor != (UnityEngine.Object) null && this.m_heroBuddyActor.IsShown())
    {
      this.m_heroBuddyActor.transform.position = new Vector3(this.transform.position.x + bone3.transform.localPosition.x, this.transform.position.y + bone3.transform.localPosition.y, this.m_mainCardActor.transform.position.z + bone3.transform.localPosition.z);
      this.m_heroBuddyActor.transform.localScale = bone3.transform.localScale;
      if (this.m_heroBuddyActor.UseCoinManaGem())
        this.m_heroBuddyActor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
      this.m_heroBuddyActor.ActivateSpellBirthState(SpellType.GHOSTMODE);
    }
    if ((UnityEngine.Object) this.m_heroPowerQuestRewardActor != (UnityEngine.Object) null && this.m_heroPowerQuestRewardActor.IsShown())
    {
      this.m_heroPowerQuestRewardActor.transform.position = new Vector3(this.transform.position.x + bone8.transform.localPosition.x, this.transform.position.y + bone8.transform.localPosition.y, this.m_mainCardActor.transform.position.z + bone8.transform.localPosition.z);
      this.m_heroPowerQuestRewardActor.transform.localScale = bone8.transform.localScale;
    }
    if ((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null && this.m_questRewardActor.IsShown())
    {
      this.m_questRewardActor.transform.position = new Vector3(this.transform.position.x + bone7.transform.localPosition.x, this.transform.position.y + bone7.transform.localPosition.y, this.m_mainCardActor.transform.position.z + bone7.transform.localPosition.z);
      this.m_questRewardActor.transform.localScale = bone7.transform.localScale;
    }
    if (this.m_useMergedRecentCombatPanelPrefab && (bool) (UnityEngine.Object) this.m_recentCombatsPanelController)
    {
      this.m_recentCombatsPanelController.transform.position = new Vector3(this.transform.position.x + bone4.transform.localPosition.x, this.transform.position.y + bone4.transform.localPosition.y, this.m_mainCardActor.transform.position.z + bone4.transform.localPosition.z);
      this.m_recentCombatsPanelController.transform.localScale = bone4.transform.localScale;
    }
    else
    {
      if (!((UnityEngine.Object) this.m_recentCombatsPanel != (UnityEngine.Object) null) || this.m_useMergedRecentCombatPanelPrefab)
        return;
      this.m_recentCombatsPanel.transform.position = new Vector3(this.transform.position.x + bone4.transform.localPosition.x, this.transform.position.y + bone4.transform.localPosition.y, this.m_mainCardActor.transform.position.z + bone4.transform.localPosition.z);
      this.m_recentCombatsPanel.transform.localScale = bone4.transform.localScale;
    }
  }

  private float GetZForThisTilesMouseOverCard(
    float tileZPosition,
    float desiredZOffset,
    float globalTop,
    float globalBottom)
  {
    if ((double) tileZPosition + (double) desiredZOffset > (double) globalTop)
      return globalTop;
    return (double) tileZPosition + (double) desiredZOffset < (double) globalBottom ? globalBottom : tileZPosition + desiredZOffset;
  }

  private bool UpdateTechLevel()
  {
    if ((UnityEngine.Object) this.m_recentCombatsPanel == (UnityEngine.Object) null)
      return false;
    int num = 1;
    int tag = this.m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID);
    if (GameState.Get().GetPlayerInfoMap().ContainsKey(tag) && GameState.Get().GetPlayerInfoMap()[tag].GetPlayerHero() != null)
      num = GameState.Get().GetPlayerInfoMap()[tag].GetPlayerHero().GetRealTimePlayerTechLevel();
    this.m_recentCombatsPanel.SetTechLevel(Mathf.Clamp(num, 1, 6));
    return true;
  }

  private bool UpdateTriples()
  {
    if ((UnityEngine.Object) this.m_recentCombatsPanel == (UnityEngine.Object) null)
      return false;
    int triples = 0;
    int tag = this.m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID);
    if (GameState.Get().GetPlayerInfoMap().ContainsKey(tag) && GameState.Get().GetPlayerInfoMap()[tag].GetPlayerHero() != null)
      triples = GameState.Get().GetPlayerInfoMap()[tag].GetPlayerHero().GetTag(GAME_TAG.PLAYER_TRIPLES);
    if (this.m_recentCombatsPanel.GetTripleCount() == triples)
      return false;
    this.m_recentCombatsPanel.SetTriples(triples);
    return true;
  }

  private bool UpdateBGQuestRewards()
  {
    if (GameState.Get() == null)
      return false;
    if (!GameState.Get().BattlegroundsAllowQuests())
    {
      this.m_heroPowerActor.gameObject.SetActive(true);
      if ((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null)
        this.m_questRewardActor.gameObject.SetActive(false);
      if ((UnityEngine.Object) this.m_heroPowerQuestRewardActor != (UnityEngine.Object) null)
        this.m_heroPowerQuestRewardActor.gameObject.SetActive(false);
      return true;
    }
    if (this.m_playerHeroEntity == null)
    {
      Debug.LogWarning((object) "UpdateBGQuestRewards - Player Hero Entity is null");
      return false;
    }
    bool flag1 = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_HEROPOWER_QUEST_REWARD_DATABASE_ID) != 0;
    bool flag2 = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_HEROPOWER_QUEST_REWARD_COMPLETED) != 0;
    bool flag3 = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_QUEST_REWARD_DATABASE_ID) != 0;
    bool flag4 = this.m_playerHeroEntity.GetTag(GAME_TAG.BACON_HERO_QUEST_REWARD_COMPLETED) != 0;
    if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
    {
      bool flag5 = this.m_heroPowerActor.GetEntity() != null && this.m_heroPowerActor.GetEntity().HasTag(GAME_TAG.BACON_DOUBLE_QUEST_HERO_POWER);
      if (flag1 & flag2 && !flag5)
        this.m_heroPowerActor.gameObject.SetActive(false);
      else
        this.m_heroPowerActor.gameObject.SetActive(!flag1 || !flag3);
    }
    if ((UnityEngine.Object) this.m_heroPowerQuestRewardActor != (UnityEngine.Object) null)
    {
      this.m_heroPowerQuestRewardActor.gameObject.SetActive(flag1);
      this.m_heroPowerQuestRewardActor.Show();
      GameObjectUtils.FindChild(this.m_heroPowerQuestRewardActor?.gameObject, "BGRewardVFX")?.gameObject.SetActive(!flag2);
      if ((UnityEngine.Object) this.m_heroPowerQuestRewardActor != (UnityEngine.Object) null && this.m_heroPowerQuestRewardActor.UseCoinManaGem())
        this.m_heroPowerQuestRewardActor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
    }
    if ((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null)
    {
      this.m_questRewardActor.gameObject.SetActive(flag3);
      this.m_questRewardActor.Show();
      GameObjectUtils.FindChild(this.m_questRewardActor?.gameObject, "BGRewardVFX")?.gameObject.SetActive(!flag4);
      if ((UnityEngine.Object) this.m_questRewardActor != (UnityEngine.Object) null && this.m_questRewardActor.UseCoinManaGem())
        this.m_questRewardActor.ActivateSpellBirthState(SpellType.COIN_MANA_GEM);
    }
    return true;
  }

  private bool UpdateRaces() => !((UnityEngine.Object) this.m_recentCombatsPanel == (UnityEngine.Object) null) && this.m_recentCombatsPanel.SetRaces(this.m_raceCounts);

  public string GetHeroName() => this.m_playerHeroEntity != null ? this.m_playerHeroEntity.GetName() : (string) null;

  public bool RefreshMainCardName()
  {
    if ((UnityEngine.Object) this.m_mainCardActor == (UnityEngine.Object) null)
      return false;
    PlayerLeaderboardMainCardActor mainCardActor = this.m_mainCardActor as PlayerLeaderboardMainCardActor;
    int tag = this.m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID);
    mainCardActor.UpdatePlayerNameText(GameState.Get().GetGameEntity().GetBestNameForPlayer(tag));
    if (mainCardActor.GetEntity() != null && !Options.Get().GetBool(Option.STREAMER_MODE))
      mainCardActor.UpdateAlternateNameText(mainCardActor.GetEntity().GetName());
    else
      mainCardActor.SetAlternateNameTextActive(false);
    return true;
  }

  internal void UpdateRacesCount(List<GameRealTimeRaceCount> races)
  {
    foreach (GameRealTimeRaceCount race1 in races)
    {
      TAG_RACE race2 = (TAG_RACE) race1.Race;
      if (!this.m_raceCounts.ContainsKey(race2) && race1.Count >= 0)
        this.m_raceCounts.Add(race2, 0);
      this.m_raceCounts[race2] = race1.Count;
    }
    this.m_racesDirty = true;
  }

  public void RefreshRecentCombats()
  {
    if ((UnityEngine.Object) this.m_recentCombatsPanel == (UnityEngine.Object) null)
      return;
    int tag = this.m_playerHeroEntity.GetTag(GAME_TAG.PLAYER_ID);
    List<PlayerLeaderboardRecentCombatsPanel.RecentCombatInfo> historyForPlayer = PlayerLeaderboardManager.Get().GetRecentCombatHistoryForPlayer(tag);
    this.m_recentCombatsPanel.ClearRecentCombats();
    if (historyForPlayer == null)
      return;
    int num = (int) Math.Max(0L, (long) historyForPlayer.Count - (long) (this.m_recentCombatsPanel.m_maxDisplayItems + 1U));
    int count = historyForPlayer.Count;
    for (int index = num; index < count; ++index)
      this.m_recentCombatsPanel.AddRecentCombat(this, historyForPlayer[index]);
  }

  public bool UpdateRecentCombats()
  {
    if ((UnityEngine.Object) this.m_recentCombatsPanelController == (UnityEngine.Object) null && this.m_useMergedRecentCombatPanelPrefab)
    {
      this.LoadRecentCombatsPanel();
      LayerUtils.SetLayer((Component) this.m_recentCombatsPanelController, GameLayer.Tooltip);
    }
    else if ((UnityEngine.Object) this.m_recentCombatsPanel == (UnityEngine.Object) null && !this.m_useMergedRecentCombatPanelPrefab)
    {
      this.LoadRecentCombatsPanel();
      LayerUtils.SetLayer((Component) this.m_recentCombatsPanel, GameLayer.Tooltip);
    }
    this.RefreshRecentCombats();
    return true;
  }

  public float GetPoppedOutBoneX()
  {
    GameObject bone = this.m_tileActor.FindBone("PoppedOutBone");
    return (UnityEngine.Object) bone == (UnityEngine.Object) null ? 0.0f : bone.transform.localPosition.x;
  }
}
