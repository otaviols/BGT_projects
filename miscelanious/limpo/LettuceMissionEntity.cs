using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using PegasusGame;
using PegasusLettuce;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class LettuceMissionEntity : MissionEntity
{
  private static readonly AssetReference LETTUCE_PHASE_POPUP = new AssetReference("LettuceTurnIndicator.prefab:bb1b08b3add6d3047bf4b787e266c26e");
  private GameObject m_phasePopup;
  protected int m_gamePhase = 1;
  private Entity m_entityThatJustCancelledAttack;
  private int m_prevSelectedCharacterZonePosition;
  private int m_numPlayActorShifting;
  private bool m_isCameraShifting;
  protected bool m_abilityOrderSpeechBubblesEnabled = true;
  protected bool m_enemyAbilityOrderSpeechBubblesEnabled = true;
  private List<MercenariesExperienceUpdate> m_endGameExperienceUpdates = new List<MercenariesExperienceUpdate>();
  private InputManager.ZoneTooltipSettings m_zoneTooltipSettings;
  private MercenariesBenchVisualController m_benchVisualController;
  private int m_blockingPowerProcessingCount;
  private LettuceFakeHandController m_fakeHandController = new LettuceFakeHandController();
  private ScreenEffectsHandle m_screenEffectsHandle;
  private readonly List<LettuceMissionEntity.OnEmoteBanterPlayedDelegate> m_onEmoteBanterPlayedCallbacks = new List<LettuceMissionEntity.OnEmoteBanterPlayedDelegate>();
  private static Map<GameEntityOption, bool> s_booleanOptions = LettuceMissionEntity.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = LettuceMissionEntity.InitStringOptions();
  protected Notification m_popupTutorialNotification;

  public MercenariesPvPRatingUpdate RatingChangeData { get; set; }

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DIM_OPPOSING_HERO_DURING_MULLIGAN,
      true
    },
    {
      GameEntityOption.HANDLE_COIN,
      false
    },
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    },
    {
      GameEntityOption.SKIP_HERO_LOAD,
      true
    },
    {
      GameEntityOption.SUPPRESS_CLASS_NAMES,
      true
    },
    {
      GameEntityOption.ALLOW_NAME_BANNER_MODE_ICONS,
      false
    },
    {
      GameEntityOption.DISABLE_RESTART_BUTTON,
      true
    },
    {
      GameEntityOption.DISABLE_CARD_TYPE_BANNER,
      true
    },
    {
      GameEntityOption.USE_COMPACT_ENCHANTMENT_BANNERS,
      true
    },
    {
      GameEntityOption.ALLOW_FATIGUE,
      false
    },
    {
      GameEntityOption.ALLOW_ENCHANTMENT_SPARKLES,
      false
    },
    {
      GameEntityOption.ALLOW_SLEEP_FX,
      false
    },
    {
      GameEntityOption.DISABLE_NONMERC_MANA_GEM,
      true
    },
    {
      GameEntityOption.DISABLE_SPELL_MANA_GEM,
      true
    },
    {
      GameEntityOption.SHOW_SPEED_WING_ON_ACTOR,
      true
    },
    {
      GameEntityOption.FLIP_END_TURN_BUTTON_WHEN_ENTERING_NO_MORE_PLAY,
      true
    },
    {
      GameEntityOption.ALWAYS_USE_FAST_CARD_DRAW_SCALE,
      true
    },
    {
      GameEntityOption.DISABLE_DELAY_BETWEEN_BIG_CARD_DISPLAY_AND_POWER_PROCESSING,
      true
    },
    {
      GameEntityOption.USE_FASTER_ATTACK_SPELL_BIRTH_STATE,
      true
    },
    {
      GameEntityOption.EARLY_CONCEDE_PROCESS_SUB_SPELL_IN_FINAL_WRAPUP_STEP,
      true
    },
    {
      GameEntityOption.CAN_ADJUST_BIG_CARD_HORIZONTALLY,
      true
    },
    {
      GameEntityOption.USE_BONES_FOR_BIG_CARD_PLACEMENT,
      true
    },
    {
      GameEntityOption.USE_BONES_FOR_TOOLTIP_PLACEMENT,
      true
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>()
  {
    {
      GameEntityOption.VICTORY_SCREEN_PREFAB_PATH,
      "VictoryTwoScoop_Lettuce.prefab:8cc3d04e21ce8334eb7c5a97d0d61086"
    },
    {
      GameEntityOption.DEFEAT_SCREEN_PREFAB_PATH,
      "DefeatTwoScoop_Lettuce.prefab:126f120867ecadd448b08d33a1f50ae9"
    },
    {
      GameEntityOption.END_OF_GAME_SPELL_PREFAB_PATH,
      "Lettuce_EndOfGameSpell.prefab:a739ec7b56e6bd14f825ba03fc0ebbfe"
    },
    {
      GameEntityOption.VICTORY_AUDIO_PATH,
      (string) null
    },
    {
      GameEntityOption.DEFEAT_AUDIO_PATH,
      (string) null
    }
  };

  public LettuceMissionEntity(VoPlaybackHandler voHandler = null)
    : base(voHandler)
  {
    this.m_gameOptions.AddOptions(LettuceMissionEntity.s_booleanOptions, LettuceMissionEntity.s_stringOptions);
    if (GameMgr.Get().GetGameType() == GameType.GT_MERCENARIES_PVE)
      this.m_gameOptions.SetBooleanOption(GameEntityOption.DISABLE_OPPONENT_NAME_BANNER, true);
    this.m_zoneTooltipSettings = new InputManager.ZoneTooltipSettings()
    {
      EnemyDeck = new InputManager.TooltipSettings(true, new InputManager.TooltipContentDelegate(this.GetEnemyDeckTooltipContent)),
      EnemyHand = new InputManager.TooltipSettings(false),
      EnemyMana = new InputManager.TooltipSettings(false),
      FriendlyDeck = InputManager.TooltipSettings.CreateCustomHandler(new InputManager.OnTooltipShownDelegate(this.OnFriendlyDeckMouseOver), new InputManager.OnTooltipHiddenDelegate(this.OnFriendlyDeckMouseOut)),
      FriendlyHand = new InputManager.TooltipSettings(false),
      FriendlyMana = new InputManager.TooltipSettings(false)
    };
    this.InitializePhasePopup();
    this.InitializePlayZonePosition();
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
    GameState.Get().RegisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
    GameState.Get().RegisterOptionsSentListener(new GameState.OptionsSentCallback(this.OnOptionsSent));
    GameState.Get().RegisterFriendlyTurnStartedListener(new GameState.FriendlyTurnStartedCallback(this.OnFriendlyTurnStarted));
    EndTurnButton.Get().RegisterButtonUnblockedListener(new EndTurnButton.OnButtonUnblocked(this.OnEndTurnButtonUnblocked));
    Network.Get().RegisterNetHandler((object) MercenariesRewardUpdate.PacketID.ID, new Network.NetHandler(this.OnMercenariesRewardUpdate));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void InitializePlayZonePosition()
  {
    BoardLayout boardLayout = Gameplay.Get().GetBoardLayout();
    Transform bone1 = boardLayout.FindBone("FriendlyPlay_Combat");
    ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY).transform.position = bone1.transform.position;
    Transform bone2 = boardLayout.FindBone("OpposingPlay_Combat");
    ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.OPPOSING).transform.position = bone2.transform.position;
  }

  private void OnFriendlyDeckMouseOver(Action<string, string> showRegularTooltip) => this.m_benchVisualController?.OnFriendlyBenchMouseOver(showRegularTooltip);

  private void OnFriendlyDeckMouseOut() => this.m_benchVisualController?.OnFriendlyBenchMouseOut();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    this.PreloadPrefab((AssetReference) "MercenariesBenchVisualController.prefab:320ca7517518ebe4f88977bd4291da36", (PrefabCallback<GameObject>) ((assetRef, gameObject, callbackData) => this.m_benchVisualController = gameObject.GetComponent<MercenariesBenchVisualController>()));
  }

  public override void OnDecommissionGame()
  {
    if (SceneMgr.Get() != null)
      SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
    if (GameState.Get() != null)
    {
      GameState.Get().UnregisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
      GameState.Get().UnregisterOptionsSentListener(new GameState.OptionsSentCallback(this.OnOptionsSent));
      GameState.Get().UnregisterFriendlyTurnStartedListener(new GameState.FriendlyTurnStartedCallback(this.OnFriendlyTurnStarted));
    }
    if ((UnityEngine.Object) EndTurnButton.Get() != (UnityEngine.Object) null)
      EndTurnButton.Get().UnregisterButtonUnblockedListener(new EndTurnButton.OnButtonUnblocked(this.OnEndTurnButtonUnblocked));
    if (Network.Get() != null)
      Network.Get().RemoveNetHandler((object) MercenariesRewardUpdate.PacketID.ID, new Network.NetHandler(this.OnMercenariesRewardUpdate));
    base.OnDecommissionGame();
  }

  public override void OnTagChanged(TagDelta change)
  {
    base.OnTagChanged(change);
    if (change.tag != 2224)
      return;
    if (change.newValue == 0)
      this.HideOpposingFakeHand();
    else
      this.ShowOpposingFakeHand();
  }

  private void StartBlockingPowerProcessing()
  {
    ++this.m_blockingPowerProcessingCount;
    GameState.Get().SetBusy(true);
  }

  private void StopBlockingPowerProcessingIfPossible()
  {
    --this.m_blockingPowerProcessingCount;
    if (this.m_blockingPowerProcessingCount != 0)
      return;
    GameState.Get().SetBusy(false);
  }

  private void InitializePhasePopup() => AssetLoader.Get().LoadGameObject(LettuceMissionEntity.LETTUCE_PHASE_POPUP, (GameObjectCallback) ((assetRef, go, callbackData) =>
  {
    this.m_phasePopup = go;
    this.m_phasePopup.SetActive(false);
  }));

  private void ShowPopup(string playmakerState) => GameEntity.Coroutines.StartCoroutine(this.ShowPopupCoroutine(playmakerState));

  private IEnumerator ShowPopupCoroutine(string playmakerState)
  {
    LettuceMissionEntity lettuceMissionEntity = this;
    lettuceMissionEntity.StartBlockingPowerProcessing();
    lettuceMissionEntity.AddInputBlocker();
    while ((UnityEngine.Object) lettuceMissionEntity.m_phasePopup == (UnityEngine.Object) null)
      yield return (object) null;
    lettuceMissionEntity.m_phasePopup.SetActive(true);
    PlayMakerFSM playmaker = lettuceMissionEntity.m_phasePopup.GetComponent<PlayMakerFSM>();
    playmaker.SetState(playmakerState);
    while (playmaker.ActiveStateName != "Hide")
      yield return (object) null;
    lettuceMissionEntity.RemoveInputBlocker();
    lettuceMissionEntity.AttemptAutoInput();
    Entity abilitiesSourceEntity = ZoneMgr.Get().GetLettuceAbilitiesSourceEntity();
    if (abilitiesSourceEntity != null)
    {
      foreach (int lettuceAbilityEntityId in abilitiesSourceEntity.GetLettuceAbilityEntityIDs())
      {
        Card card = GameState.Get().GetEntity(lettuceAbilityEntityId)?.GetCard();
        if ((UnityEngine.Object) card != (UnityEngine.Object) null)
          card.UpdateActorState();
      }
    }
    lettuceMissionEntity.StopBlockingPowerProcessingIfPossible();
  }

  private void AttemptAutoInput()
  {
    if (!InputManager.Get().PermitDecisionMakingInput() || GameState.Get().IsResponsePacketBlocked() || SceneMgr.Get().IsTransitioning() || this.m_gamePhase == 3 || this.m_isCameraShifting)
      return;
    switch (GameState.Get().GetGameEntity().GetTag<ACTION_STEP_TYPE>(GAME_TAG.ACTION_STEP_TYPE))
    {
      case ACTION_STEP_TYPE.DEFAULT:
        this.AutoSelectNextPendingMercenary();
        break;
      case ACTION_STEP_TYPE.LETTUCE_MERCENARY_SELECTION:
        this.AutoEndTurn();
        break;
    }
  }

  private void AutoSelectNextPendingMercenary()
  {
    if (this.HasTag(GAME_TAG.LETTUCE_DISABLE_AUTO_SELECT_NEXT_MERC) || ZoneMgr.Get().GetLettuceAbilitiesSourceEntity() != null)
      return;
    if (this.m_entityThatJustCancelledAttack != null)
    {
      ZoneMgr.Get().DisplayLettuceAbilitiesForEntity(this.m_entityThatJustCancelledAttack);
      RemoteActionHandler.Get().NotifyOpponentOfSelection(this.m_entityThatJustCancelledAttack.GetEntityId());
      this.m_entityThatJustCancelledAttack = (Entity) null;
    }
    else
    {
      Entity pendingMercenary = this.GetNextPendingMercenary();
      if (pendingMercenary == null)
        return;
      ZoneMgr.Get().DisplayLettuceAbilitiesForEntity(pendingMercenary);
      RemoteActionHandler.Get().NotifyOpponentOfSelection(pendingMercenary.GetEntityId());
    }
  }

  private Entity GetNextPendingMercenary()
  {
    Network.Options optionsPacket = GameState.Get()?.GetOptionsPacket();
    if (optionsPacket == null)
      return (Entity) null;
    ZonePlay zoneOfType = ZoneMgr.Get()?.FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY);
    if ((UnityEngine.Object) zoneOfType == (UnityEngine.Object) null)
      return (Entity) null;
    List<Card> cardList = new List<Card>();
    foreach (Card card in zoneOfType.GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity != null && entity.IsMinion() && entity.GetController() != null && entity.GetController().IsTeamLeader() && (!entity.HasSelectedLettuceAbility() || !entity.HasTag(GAME_TAG.LETTUCE_HAS_MANUALLY_SELECTED_ABILITY)))
        cardList.Add(card);
    }
    int totalCount = zoneOfType.GetCards().Count;
    cardList.Sort((Comparison<Card>) ((lhs, rhs) =>
    {
      int num1 = lhs.GetEntity().GetZonePosition() - this.m_prevSelectedCharacterZonePosition;
      if (num1 <= 0)
        num1 += totalCount;
      int num2 = rhs.GetEntity().GetZonePosition() - this.m_prevSelectedCharacterZonePosition;
      if (num2 <= 0)
        num2 += totalCount;
      return num1 - num2;
    }));
    foreach (Card card in cardList)
    {
      Entity entity = card.GetEntity();
      bool flag = false;
      foreach (int lettuceAbilityEntityId in entity.GetLettuceAbilityEntityIDs())
      {
        Network.Options.Option optionFromEntityId = optionsPacket.GetOptionFromEntityID(lettuceAbilityEntityId);
        if (optionFromEntityId != null && (optionFromEntityId.Main.PlayErrorInfo.IsValid() || optionFromEntityId.HasValidSubOption()))
        {
          flag = true;
          break;
        }
      }
      if (flag)
        return entity;
    }
    return (Entity) null;
  }

  private void AutoEndTurn()
  {
    Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
    if (optionsPacket == null)
      return;
    bool flag = false;
    foreach (Network.Options.Option option in optionsPacket.List)
    {
      if (option.Type != Network.Options.Option.OptionType.END_TURN)
      {
        flag = true;
        break;
      }
    }
    if (flag)
      return;
    InputManager.Get().DoEndTurnButton();
  }

  private void OnGameplaySceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode != SceneMgr.Mode.GAMEPLAY)
      return;
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
    ACTION_STEP_TYPE tag1 = this.GetTag<ACTION_STEP_TYPE>(GAME_TAG.ACTION_STEP_TYPE);
    bool flag = GameState.Get().IsActionStep();
    if (flag)
    {
      switch (tag1)
      {
        case ACTION_STEP_TYPE.DEFAULT:
          GameEntity.Coroutines.StartCoroutine(this.OnPreparationPhase());
          break;
        case ACTION_STEP_TYPE.LETTUCE_MERCENARY_SELECTION:
          GameEntity.Coroutines.StartCoroutine(this.OnNominationPhase());
          break;
      }
    }
    if (Board.Get() is MercenariesBoard mercenariesBoard)
    {
      int tag2 = this.GetTag(GAME_TAG.GAME_SEED);
      bool isFinalBoss = GameUtils.IsFinalBossNodeType(this.GetTag(GAME_TAG.LETTUCE_NODE_TYPE));
      bool allowLightingChanges = true;
      int tag3 = this.GetTag(GAME_TAG.LETTUCE_CURRENT_BOUNTY_ID);
      LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord(tag3);
      if (record != null && record.BountySetRecord != null && record.BountySetRecord.IsTutorial)
        allowLightingChanges = false;
      mercenariesBoard.RandomizeVisuals(isFinalBoss, allowLightingChanges, tag2);
    }
    int tag4 = this.GetTag(GAME_TAG.TURN);
    if (tag4 > 0)
    {
      if (flag && tag1 == ACTION_STEP_TYPE.DEFAULT)
        this.UpdateAllMercenaryAbilityOrderBubbleText();
      if (this.HasTag(GAME_TAG.LETTUCE_SHOW_OPPOSING_FAKE_HAND))
        this.ShowOpposingFakeHand();
      this.OnLettuceMissionEntityReconnect(tag4);
    }
    this.OnLettuceMissionEntityGameSceneLoaded();
  }

  protected virtual void OnLettuceMissionEntityGameSceneLoaded()
  {
  }

  protected virtual void OnLettuceMissionEntityReconnect(int currentTurn)
  {
  }

  private void OnOptionsSent(Network.Options.Option option, object userData) => this.HideWeaknessSplats();

  private void OnOptionsReceived(object userData) => this.AttemptAutoInput();

  private void OnFriendlyTurnStarted(object userData) => this.AttemptAutoInput();

  private void OnEndTurnButtonUnblocked(object userData) => this.AttemptAutoInput();

  private void OnMercenariesRewardUpdate()
  {
    MercenariesRewardUpdate mercenariesRewardUpdate = Network.Get().MercenariesRewardUpdate();
    if (mercenariesRewardUpdate == null || mercenariesRewardUpdate.RewardType == LettuceRewardContents.Type.TYPE_PVE_CHEST || mercenariesRewardUpdate.RewardType == LettuceRewardContents.Type.TYPE_PVE_BOSS_CHEST || mercenariesRewardUpdate.RewardType == LettuceRewardContents.Type.TYPE_PVP_CHEST || mercenariesRewardUpdate.RewardType == LettuceRewardContents.Type.TYPE_PVE_CONSOLATION || mercenariesRewardUpdate.RewardType == LettuceRewardContents.Type.TYPE_PVE_AUTO_RETIRED)
      return;
    foreach (MercenariesExperienceUpdate experienceUpdate in mercenariesRewardUpdate.ExperienceUpdates)
    {
      if (experienceUpdate.HasMercenaryId && experienceUpdate.HasExpDelta)
        this.m_endGameExperienceUpdates.Add(experienceUpdate);
    }
  }

  public override InputManager.ZoneTooltipSettings GetZoneTooltipSettings() => this.m_zoneTooltipSettings;

  private bool GetEnemyDeckTooltipContent(ref string headline, ref string description, int index)
  {
    if (index != 0)
      return false;
    headline = GameStrings.Get("GAMEPLAY_TOOLTIP_LETTUCE_ENEMYBENCH_HEADLINE");
    ZoneDeck zoneOfType1 = ZoneMgr.Get().FindZoneOfType<ZoneDeck>(Player.Side.OPPOSING);
    ZoneHand zoneOfType2 = ZoneMgr.Get().FindZoneOfType<ZoneHand>(Player.Side.OPPOSING);
    description = GameStrings.Format("GAMEPLAY_TOOLTIP_LETTUCE_ENEMYBENCH_DESCRIPTION", (object) (zoneOfType1.GetCardCount() + zoneOfType2.GetCardCount()));
    return true;
  }

  public List<MercenariesExperienceUpdate> GetMercenaryExperienceUpdates() => this.m_endGameExperienceUpdates;

  public override Entity GetExtraMouseOverBigCardEntity(Entity source)
  {
    if (source == null)
      return (Entity) null;
    Entity overBigCardEntity = (Entity) null;
    int lettuceAbilityId = source.GetSelectedLettuceAbilityID();
    if (lettuceAbilityId != 0)
      overBigCardEntity = GameState.Get().GetEntity(lettuceAbilityId);
    else if (!source.ShouldShowEquipmentTextOnMerc())
      overBigCardEntity = source.GetEquipmentEntity();
    return overBigCardEntity;
  }

  public override bool ShowMouseOverBigCardImmediately(Entity mouseOverEntity) => mouseOverEntity != null && !UniversalInputManager.Get().IsTouchMode() && (mouseOverEntity.IsMinion() || mouseOverEntity.IsLettuceAbility());

  public override bool SuppressMousedOverCardTooltip(out bool resetTimer)
  {
    resetTimer = false;
    MercenariesAbilityTray abilityTray = ZoneMgr.Get().GetLettuceZoneController().GetAbilityTray();
    if ((UnityEngine.Object) abilityTray == (UnityEngine.Object) null || !abilityTray.IsAnimating())
      return false;
    resetTimer = true;
    return true;
  }

  public override bool ShouldSuppressCardMouseOver(Entity mouseOverEntity) => this.m_gamePhase == 3 || this.m_isCameraShifting;

  public override bool ShouldSuppressHistoryMouseOver() => this.m_gamePhase == 3 || this.m_isCameraShifting;

  public override bool NotifyOfTooltipDisplay(TooltipZone tooltip) => this.m_gamePhase == 3 || this.m_isCameraShifting;

  public override bool ShouldSuppressOptionHighlight(Entity entity)
  {
    if (GameState.Get().GetGameEntity().HasTag(GAME_TAG.ALLOW_MOVE_MINION) || entity == null || !entity.IsMinion() || !entity.IsControlledByFriendlySidePlayer())
      return false;
    Card card = entity.GetCard();
    int num;
    if (card == null)
    {
      num = 0;
    }
    else
    {
      TAG_ZONE? serverTag = card.GetZone()?.m_ServerTag;
      TAG_ZONE tagZone = TAG_ZONE.PLAY;
      num = serverTag.GetValueOrDefault() == tagZone & serverTag.HasValue ? 1 : 0;
    }
    return num != 0;
  }

  public virtual void OnAbilityTrayShown(Entity entity)
  {
  }

  public virtual void OnAbilityTrayDismissed()
  {
  }

  public void SetEntityThatJustCancelledAbilitySelection(Entity entity) => this.m_entityThatJustCancelledAttack = entity;

  public void SetPrevSelectedCharacterZonePosition(int zonePos) => this.m_prevSelectedCharacterZonePosition = zonePos;

  public override List<TooltipPanelManager.TooltipPanelData> GetOverwriteKeywordHelpPanelDisplay(
    Entity entity)
  {
    if (!entity.IsControlledByFriendlySidePlayer() || !GameState.Get().IsActionStep() || this.GetTag<ACTION_STEP_TYPE>(GAME_TAG.ACTION_STEP_TYPE) != ACTION_STEP_TYPE.LETTUCE_MERCENARY_SELECTION)
      return (List<TooltipPanelManager.TooltipPanelData>) null;
    List<TooltipPanelManager.TooltipPanelData> helpPanelDisplay = new List<TooltipPanelManager.TooltipPanelData>();
    foreach (int lettuceAbilityEntityId in entity.GetLettuceAbilityEntityIDs())
    {
      Entity entity1 = GameState.Get().GetEntity(lettuceAbilityEntityId);
      if (entity1 != null && entity1.HasTag(GAME_TAG.LETTUCE_IS_TREASURE_CARD))
        helpPanelDisplay.Add(new TooltipPanelManager.TooltipPanelData()
        {
          m_title = entity1.GetName(),
          m_description = UberText.RemoveMarkupAndCollapseWhitespaces(entity1.GetCardTextInHand(), true, true)
        });
    }
    foreach (int lettuceAbilityEntityId in entity.GetLettuceAbilityEntityIDs())
    {
      Entity entity2 = GameState.Get().GetEntity(lettuceAbilityEntityId);
      if (entity2 != null && !entity2.HasTag(GAME_TAG.LETTUCE_IS_TREASURE_CARD) && !entity2.IsLettuceEquipment())
        helpPanelDisplay.Add(new TooltipPanelManager.TooltipPanelData()
        {
          m_title = entity2.GetName(),
          m_description = UberText.RemoveMarkupAndCollapseWhitespaces(entity2.GetCardTextInHand(), true, true)
        });
    }
    return helpPanelDisplay;
  }

  public override bool GetEntityBaseForKeywordTooltips(
    Entity source,
    bool isHistoryTile,
    out EntityBase entityBaseForTooltips,
    out List<EntityBase> additionalEntityBaseForTooltips)
  {
    entityBaseForTooltips = (EntityBase) null;
    additionalEntityBaseForTooltips = (List<EntityBase>) null;
    if (isHistoryTile || !source.IsMinion())
      return false;
    Zone zone = source.GetCard().GetZone();
    if ((zone != null ? (zone.m_ServerTag != TAG_ZONE.PLAY ? 1 : 0) : 1) != 0)
      return false;
    int lettuceAbilityId = source.GetSelectedLettuceAbilityID();
    Entity entity = GameState.Get().GetEntity(lettuceAbilityId);
    Entity equipmentEntity = source.GetEquipmentEntity();
    if (entity != null)
    {
      entityBaseForTooltips = (EntityBase) entity;
      additionalEntityBaseForTooltips = new List<EntityBase>()
      {
        (EntityBase) source
      };
      if (equipmentEntity != null)
        additionalEntityBaseForTooltips.Add((EntityBase) equipmentEntity);
      return true;
    }
    if (equipmentEntity == null)
      return false;
    entityBaseForTooltips = (EntityBase) equipmentEntity;
    additionalEntityBaseForTooltips = new List<EntityBase>()
    {
      (EntityBase) source
    };
    return true;
  }

  private int GetNominatedMercenariesCount()
  {
    int mercenariesCount = 0;
    ZonePlay zoneOfType = ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY);
    if ((UnityEngine.Object) zoneOfType != (UnityEngine.Object) null)
    {
      foreach (Card card in zoneOfType.GetCards())
      {
        Entity entity = card.GetEntity();
        if (entity != null && entity.IsMercenary() && entity.IsControlledByFriendlySidePlayer())
          ++mercenariesCount;
      }
    }
    return mercenariesCount;
  }

  private int GetBenchedMercenariesCount()
  {
    int mercenariesCount = 0;
    ZoneHand zoneOfType = ZoneMgr.Get().FindZoneOfType<ZoneHand>(Player.Side.FRIENDLY);
    if ((UnityEngine.Object) zoneOfType != (UnityEngine.Object) null)
    {
      foreach (Card card in zoneOfType.GetCards())
      {
        Entity entity = card.GetEntity();
        if (entity != null && entity.IsMercenary() && entity.IsControlledByFriendlySidePlayer())
          ++mercenariesCount;
      }
    }
    return mercenariesCount;
  }

  public override string GetTurnTimerCountdownText(float timeRemainingInTurn) => this.m_gamePhase == 3 ? GameStrings.Get("GAMEPLAY_LETTUCE_COMBAT_BUTTON") : (string) null;

  public override bool GetAlternativeEndTurnButtonText(
    out string myTurnText,
    out string waitingText)
  {
    myTurnText = GameStrings.Get("GAMEPLAY_LETTUCE_READY_BUTTON");
    waitingText = GameStrings.Get("GAMEPLAY_LETTUCE_WAIT_BUTTON");
    if (this.m_gamePhase == 1)
    {
      int mercenariesCount1 = this.GetNominatedMercenariesCount();
      int mercenariesCount2 = this.GetBenchedMercenariesCount();
      int tag = GameState.Get().GetLocalSidePlayer().GetTag(GAME_TAG.LETTUCE_MAX_IN_PLAY_MERCENARIES);
      int num = Math.Min(mercenariesCount1 + mercenariesCount2, tag);
      if (mercenariesCount1 < num)
        myTurnText = GameStrings.Format("GAMEPLAY_LETTUCE_MERCENARY_SELECT_BUTTON", (object) mercenariesCount1, (object) num);
    }
    else if (this.m_gamePhase == 2)
      myTurnText = GameStrings.Get("GAMEPLAY_LETTUCE_FIGHT_BUTTON");
    else if (this.m_gamePhase == 3)
      waitingText = string.Empty;
    return true;
  }

  public override bool ShouldOverwriteEndTurnButtonNoMorePlaysState(out bool hasNoMorePlay)
  {
    hasNoMorePlay = false;
    if (GameState.Get().IsActionStep())
    {
      switch (this.GetTag<ACTION_STEP_TYPE>(GAME_TAG.ACTION_STEP_TYPE))
      {
        case ACTION_STEP_TYPE.DEFAULT:
          if (this.GetNextPendingMercenary() == null)
          {
            hasNoMorePlay = true;
            return true;
          }
          break;
        case ACTION_STEP_TYPE.LETTUCE_MERCENARY_SELECTION:
          int mercenariesCount1 = this.GetNominatedMercenariesCount();
          int mercenariesCount2 = this.GetBenchedMercenariesCount();
          int tag = GameState.Get().GetLocalSidePlayer().GetTag(GAME_TAG.LETTUCE_MAX_IN_PLAY_MERCENARIES);
          int num1 = mercenariesCount1;
          int num2 = Math.Min(mercenariesCount2 + num1, tag);
          if (mercenariesCount1 >= num2)
          {
            hasNoMorePlay = true;
            return true;
          }
          break;
      }
    }
    return false;
  }

  public override bool ShouldAutoCorrectZone(Zone zone)
  {
    switch (zone)
    {
      case ZoneLettuceAbility _:
        return false;
      case ZoneDeck _:
        return false;
      case ZoneHand _:
        return false;
      default:
        return true;
    }
  }

  public override bool OverwriteZoneDeckToAcceptEntity(
    ZoneDeck deckZone,
    int controllerId,
    TAG_ZONE zoneTag,
    TAG_CARDTYPE cardType,
    Entity entity)
  {
    if ((zoneTag == TAG_ZONE.DECK ? 1 : (zoneTag == TAG_ZONE.SETASIDE ? 1 : 0)) == 0 || !entity.IsMercenary())
      return false;
    Player player = GameState.Get().GetPlayer(controllerId);
    return player != null && player.IsOpposingSide() && (UnityEngine.Object) deckZone == (UnityEngine.Object) ZoneMgr.Get().FindZoneOfType<ZoneDeck>(Player.Side.OPPOSING);
  }

  public override bool OverwriteEndTurnReminder(Entity entity, out bool showReminder)
  {
    showReminder = false;
    return true;
  }

  public void AddInputBlockerFriendlyAbilityZone() => GameState.Get().GetFriendlySidePlayer().GetLettuceAbilityZone().AddInputBlocker();

  public void RemoveInputBlockerFriendlyAbilityZone() => GameState.Get().GetFriendlySidePlayer().GetLettuceAbilityZone().RemoveInputBlocker();

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    Board.Get().ChangeBoardVisualState(TAG_BOARD_VISUAL_STATE.SHOP);
    this.SetFullScreenFXForPreparation();
    yield return (object) this.TweenCameraZoom(false);
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    switch (missionEvent)
    {
      case 1:
        this.m_gamePhase = missionEvent;
        yield return (object) this.OnNominationPhase();
        break;
      case 2:
        this.m_gamePhase = missionEvent;
        yield return (object) this.OnPreparationPhase();
        break;
      case 3:
        this.m_gamePhase = missionEvent;
        yield return (object) this.OnCombatPhase();
        break;
    }
  }

  private IEnumerator OnNominationPhase()
  {
    EndTurnButton.Get().UpdateButtonText();
    TurnTimer.Get().OnMercenariesPhaseChange();
    ZoneMgr.Get().ClearLocalChangeListHistory();
    this.ShiftPlayZoneForGamePhase(1);
    this.SetFullScreenFXForPreparation();
    Board.Get().ChangeBoardVisualState(TAG_BOARD_VISUAL_STATE.COMBAT);
    GameEntity.Coroutines.StartCoroutine(this.TweenCameraZoom(false));
    yield break;
  }

  private IEnumerator OnPreparationPhase()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LettuceMissionEntity lettuceMissionEntity = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    EndTurnButton.Get().UpdateButtonText();
    TurnTimer.Get().OnMercenariesPhaseChange();
    ZoneMgr.Get().ClearLocalChangeListHistory();
    lettuceMissionEntity.ShiftPlayZoneForGamePhase(2);
    string playmakerState = !lettuceMissionEntity.HasTag(GAME_TAG.LETTUCE_OVERTIME) ? "Prep" : "Fatigue";
    lettuceMissionEntity.ShowPopup(playmakerState);
    lettuceMissionEntity.SetFullScreenFXForPreparation();
    lettuceMissionEntity.ShowAllMercenaryAbilityOrderBubbles();
    Board.Get().ChangeBoardVisualState(TAG_BOARD_VISUAL_STATE.SHOP);
    GameEntity.Coroutines.StartCoroutine(lettuceMissionEntity.TweenCameraZoom(false));
    return false;
  }

  private IEnumerator OnCombatPhase()
  {
    EndTurnButton.Get().UpdateButtonText();
    TurnTimer.Get().OnMercenariesPhaseChange();
    ZoneMgr.Get().ClearLocalChangeListHistory();
    ZoneMgr.Get().DismissMercenariesAbilityTray();
    this.m_prevSelectedCharacterZonePosition = 0;
    this.ShiftPlayZoneForGamePhase(3);
    this.ShowPopup("Combat");
    this.SetFullScreenFXForCombat();
    this.HideAllMercenaryAbilityOrderBubbles();
    Board.Get().ChangeBoardVisualState(TAG_BOARD_VISUAL_STATE.COMBAT);
    GameEntity.Coroutines.StartCoroutine(this.TweenCameraZoom(true));
    yield break;
  }

  private void ShowOpposingFakeHand()
  {
    this.StartBlockingPowerProcessing();
    this.m_fakeHandController.ShowOpposingFakeHand(new Action(this.StopBlockingPowerProcessingIfPossible));
  }

  private void HideOpposingFakeHand()
  {
    this.StartBlockingPowerProcessing();
    this.m_fakeHandController.HideOpposingFakeHand(new Action(this.StopBlockingPowerProcessingIfPossible));
  }

  private IEnumerator TweenCameraZoom(bool zoomedIn)
  {
    if (!((UnityEngine.Object) BoardCameras.Get() == (UnityEngine.Object) null))
    {
      float finalFieldOfView = zoomedIn ? BoardCameras.Get().m_FieldOfViewZoomed : BoardCameras.Get().m_FieldOfViewDefault;
      this.m_isCameraShifting = true;
      yield return (object) BoardCameras.Get().TweenCameraFieldOfView(finalFieldOfView, 0.5f);
      this.m_isCameraShifting = false;
    }
  }

  private void ShiftPlayZoneForGamePhase(int phase)
  {
    if ((UnityEngine.Object) Gameplay.Get() == (UnityEngine.Object) null || (UnityEngine.Object) ZoneMgr.Get() == (UnityEngine.Object) null)
      return;
    Transform bone1;
    Transform bone2;
    if (phase == 2)
    {
      bone1 = Gameplay.Get().GetBoardLayout().FindBone("FriendlyPlay_Prep");
      bone2 = Gameplay.Get().GetBoardLayout().FindBone("OpposingPlay_Prep");
    }
    else
    {
      bone1 = Gameplay.Get().GetBoardLayout().FindBone("FriendlyPlay_Combat");
      bone2 = Gameplay.Get().GetBoardLayout().FindBone("OpposingPlay_Combat");
    }
    ZonePlay zoneOfType1 = ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY);
    ZonePlay zoneOfType2 = ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.OPPOSING);
    this.m_numPlayActorShifting = 0;
    List<Tuple<Card, Spell>> cardsToPlaySpell = new List<Tuple<Card, Spell>>();
    if (!Mathf.Approximately(zoneOfType1.transform.position.z, bone1.transform.position.z))
    {
      SpellType spellType = phase == 2 ? SpellType.MERCENARIES_PHASE_TRANSITION_MOVE_DOWN : SpellType.MERCENARIES_PHASE_TRANSITION_MOVE_UP;
      foreach (Card card in zoneOfType1.GetCards())
      {
        Spell actorSpell = card.GetActorSpell(spellType);
        if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
          cardsToPlaySpell.Add(new Tuple<Card, Spell>(card, actorSpell));
      }
    }
    if (!Mathf.Approximately(zoneOfType2.transform.position.z, bone2.transform.position.z))
    {
      SpellType spellType = phase == 2 ? SpellType.MERCENARIES_PHASE_TRANSITION_MOVE_UP : SpellType.MERCENARIES_PHASE_TRANSITION_MOVE_DOWN;
      foreach (Card card in zoneOfType2.GetCards())
      {
        Spell actorSpell = card.GetActorSpell(spellType);
        if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
          cardsToPlaySpell.Add(new Tuple<Card, Spell>(card, actorSpell));
      }
    }
    zoneOfType1.transform.position = bone1.transform.position;
    zoneOfType2.transform.position = bone2.transform.position;
    this.m_numPlayActorShifting = cardsToPlaySpell.Count;
    if (this.m_numPlayActorShifting > 0)
    {
      this.StartBlockingPowerProcessing();
      GameEntity.Coroutines.StartCoroutine(this.WaitForZoneThenShiftActorsInPlay(cardsToPlaySpell));
    }
    else
    {
      zoneOfType1.UpdateLayout();
      zoneOfType2.UpdateLayout();
    }
  }

  private IEnumerator WaitForZoneThenShiftActorsInPlay(
    List<Tuple<Card, Spell>> cardsToPlaySpell)
  {
    LettuceMissionEntity lettuceMissionEntity = this;
    ZonePlay friendlyPlayZone = ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY);
    while (friendlyPlayZone.IsUpdatingLayout())
      yield return (object) null;
    ZonePlay opposingPlayZone = ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.OPPOSING);
    while (opposingPlayZone.IsUpdatingLayout())
      yield return (object) null;
    foreach (Tuple<Card, Spell> tuple in cardsToPlaySpell)
    {
      Card card = tuple.Item1;
      Spell spell = tuple.Item2;
      card.SetTransitionStyle(ZoneTransitionStyle.INSTANT);
      spell.AddFinishedCallback(new Spell.FinishedCallback(lettuceMissionEntity.OnSpellFinished_ShiftActorInPlay));
      spell.ActivateState(SpellStateType.BIRTH);
    }
  }

  private void OnSpellFinished_ShiftActorInPlay(Spell spell, object userData)
  {
    --this.m_numPlayActorShifting;
    if (this.m_numPlayActorShifting > 0)
      return;
    ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY).UpdateLayout();
    ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.OPPOSING).UpdateLayout();
    this.StopBlockingPowerProcessingIfPossible();
  }

  protected bool ShouldSortAbilitiesLowToHigh()
  {
    bool high = true;
    GameEntity gameEntity = GameState.Get()?.GetGameEntity();
    if (gameEntity != null)
      high = !gameEntity.HasTag(GAME_TAG.LETTUCE_COMBAT_FROM_HIGH_TO_LOW);
    return high;
  }

  public virtual void UpdateAllMercenaryAbilityOrderBubbleText(bool hideUnselectedAbilityBubbles = false)
  {
    if (this.m_gamePhase == 3 || !this.m_abilityOrderSpeechBubblesEnabled)
      return;
    List<Card> allMinionsInPlay = this.GetAllMinionsInPlay();
    allMinionsInPlay.Sort((IComparer<Card>) new LettuceMissionEntity.CardSpeedCamparer(this.ShouldSortAbilitiesLowToHigh()));
    for (int index = 0; index < allMinionsInPlay.Count; ++index)
    {
      Card card1 = allMinionsInPlay[index];
      int order = allMinionsInPlay.IndexOf(card1) + 1;
      bool isTied = false;
      if (index > 0)
      {
        Card card2 = allMinionsInPlay[index - 1];
        if (card1.GetPreparedLettuceAbilitySpeedValue() == card2.GetPreparedLettuceAbilitySpeedValue())
        {
          isTied = true;
          order = card2.GetLettuceAbilityActionOrder();
          card2.SetLettuceAbilityActionOrder(order, isTied);
        }
      }
      card1.SetLettuceAbilityActionOrder(order, isTied);
    }
    foreach (Card card in allMinionsInPlay)
    {
      if (this.m_enemyAbilityOrderSpeechBubblesEnabled || !card.GetEntity().IsControlledByOpposingSidePlayer())
        card.UpdateLettuceSpeechBubbleText(hideUnselectedAbilityBubbles);
    }
  }

  private void HideAllMercenaryAbilityOrderBubbles()
  {
    foreach (Card card in this.GetAllMinionsInPlay())
    {
      Spell actorSpell = card.GetActorSpell(SpellType.MERCENARIES_SPEECH_BUBBLE, false);
      if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
        actorSpell.ActivateState(SpellStateType.DEATH);
    }
  }

  public void ShowAllMercenaryAbilityOrderBubbles()
  {
    if (!this.m_abilityOrderSpeechBubblesEnabled)
      return;
    foreach (Card card in this.GetAllMinionsInPlay())
    {
      if (this.m_enemyAbilityOrderSpeechBubblesEnabled || !card.GetEntity().IsControlledByOpposingSidePlayer())
      {
        Spell actorSpell = card.GetActorSpell(SpellType.MERCENARIES_SPEECH_BUBBLE);
        if ((UnityEngine.Object) actorSpell != (UnityEngine.Object) null)
        {
          actorSpell.GetComponent<PlayMakerFSM>().FsmVariables.GetFsmString("Text").Value = string.Empty;
          actorSpell.ActivateState(SpellStateType.BIRTH);
        }
      }
    }
  }

  protected List<Card> GetAllMinionsInPlay()
  {
    if (GameState.Get() == null || GameState.Get().GetFriendlySidePlayer() == null || (UnityEngine.Object) GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone() == (UnityEngine.Object) null || GameState.Get().GetOpposingSidePlayer() == null || (UnityEngine.Object) GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone() == (UnityEngine.Object) null)
      return new List<Card>();
    List<Card> allMinionsInPlay = new List<Card>();
    allMinionsInPlay.AddRange((IEnumerable<Card>) GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards());
    allMinionsInPlay.AddRange((IEnumerable<Card>) GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards());
    return allMinionsInPlay;
  }

  private void SetFullScreenFXForCombat() => this.m_screenEffectsHandle.StartEffect(new ScreenEffectParameters(ScreenEffectType.VIGNETTE, vignette: new VignetteParameters?(new VignetteParameters(1.25f))));

  private void SetFullScreenFXForPreparation() => this.m_screenEffectsHandle.StopEffect(0.5f, iTween.EaseType.easeOutCirc);

  public override void NotifyOfResetGameStarted()
  {
    base.NotifyOfResetGameStarted();
    this.SetFullScreenFXForPreparation();
  }

  public override string GetAttackSpellControllerOverride(Entity attacker)
  {
    if (attacker == null)
      return (string) null;
    if (!attacker.IsLettuceMercenary())
      return (string) null;
    int levelFromExperience = GameUtils.GetMercenaryLevelFromExperience(attacker.GetTag(GAME_TAG.LETTUCE_MERCENARY_EXPERIENCE));
    if (levelFromExperience > 20)
      return "AttackSpellController_Mercenaries_HighLevel.prefab:a1d93a294c041f740ba2ea9e2756a3ce";
    if (levelFromExperience > 10)
      return "AttackSpellController_Mercenaries_MidLevel.prefab:ee14d0ca7c274cd49a45beab7d4bc422";
    return levelFromExperience > 0 ? "AttackSpellController_Mercenaries_LowLevel.prefab:f63eae2726570f548984f477386eca7e" : (string) null;
  }

  public override ZonePlay.PlayZoneSizeOverride GetPlayZoneSizeOverride()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return (ZonePlay.PlayZoneSizeOverride) null;
    return new ZonePlay.PlayZoneSizeOverride()
    {
      m_scale = 1.15f,
      m_slotWidthModifier = 1.15f
    };
  }

  public void ShowWeaknessSplatsForMercenary(Entity pointOfViewMercenary)
  {
    ZonePlay zoneOfType = ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.OPPOSING);
    if (!((UnityEngine.Object) zoneOfType != (UnityEngine.Object) null))
      return;
    foreach (Card card in zoneOfType.GetCards())
    {
      bool flag = pointOfViewMercenary.IsMyLettuceRoleStrongAgainst(card.GetEntity());
      if (!flag)
      {
        foreach (Entity enchantment in card.GetEntity().GetEnchantments())
        {
          if (enchantment.GetCardId().Equals("LETL_000_07e") && enchantment.GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) == pointOfViewMercenary.GetEntityId())
          {
            flag = true;
            break;
          }
        }
      }
      if (flag && !card.GetEntity().IsDormant() && !card.GetEntity().HasTag(GAME_TAG.UNTOUCHABLE))
        card.ShowWeaknessSplat();
    }
  }

  public void HideWeaknessSplats()
  {
    ZonePlay zoneOfType1 = ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.OPPOSING);
    if ((UnityEngine.Object) zoneOfType1 != (UnityEngine.Object) null)
    {
      foreach (Card card in zoneOfType1.GetCards())
        card.HideWeaknessSplat();
    }
    ZonePlay zoneOfType2 = ZoneMgr.Get().FindZoneOfType<ZonePlay>(Player.Side.FRIENDLY);
    if (!((UnityEngine.Object) zoneOfType2 != (UnityEngine.Object) null))
      return;
    foreach (Card card in zoneOfType2.GetCards())
      card.HideWeaknessSplat();
  }

  public override void NotifyOfCardMousedOver(Entity mousedOverEntity)
  {
    base.NotifyOfCardMousedOver(mousedOverEntity);
    if (!(mousedOverEntity.GetCard().GetZone() is ZoneHand) || !mousedOverEntity.IsMercenary())
      return;
    this.ShowWeaknessSplatsForMercenary(mousedOverEntity);
  }

  public override void NotifyOfCardMousedOff(Entity mousedOffEntity)
  {
    base.NotifyOfCardMousedOff(mousedOffEntity);
    if (!(mousedOffEntity.GetCard().GetZone() is ZoneHand) || !mousedOffEntity.IsMercenary())
      return;
    this.HideWeaknessSplats();
  }

  public override void NotifyOfCardGrabbed(Entity entity)
  {
    base.NotifyOfCardGrabbed(entity);
    if (!(entity.GetCard().GetZone() is ZoneHand) || !entity.IsMercenary())
      return;
    this.ShowWeaknessSplatsForMercenary(entity);
  }

  public override void NotifyOfCardDropped(Entity entity)
  {
    base.NotifyOfCardDropped(entity);
    if (!(entity.GetCard().GetZone() is ZoneHand) || !entity.IsMercenary())
      return;
    this.HideWeaknessSplats();
  }

  public override bool OverwriteCurrentPlayer(Player player, out bool isCurrentPlayer)
  {
    isCurrentPlayer = true;
    return true;
  }

  public override bool Overwrite_IsInZone_ForInputManager(
    Entity entity,
    TAG_ZONE zoneTag,
    TAG_ZONE finalZoneTag,
    out bool isInZone)
  {
    isInZone = false;
    if (zoneTag == TAG_ZONE.PLAY && finalZoneTag == TAG_ZONE.LETTUCE_ABILITY && entity.IsLettuceAbility())
    {
      Entity abilitiesSourceEntity = ZoneMgr.Get().GetLettuceAbilitiesSourceEntity();
      if (abilitiesSourceEntity != null && abilitiesSourceEntity == entity.GetLettuceAbilityOwner())
      {
        isInZone = true;
        return true;
      }
    }
    return false;
  }

  protected void CreateTutorialDialog(
    AssetReference assetPrefab,
    string headlineGameString,
    string bodyTextGameString,
    string buttonGameString,
    UIEvent.Handler buttonHandler,
    UnityEngine.Vector2 materialOffset)
  {
    GameObject gameObject = AssetLoader.Get().InstantiatePrefab(assetPrefab);
    if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Unable to load tutorial dialog TutorialIntroDialog prefab.");
    }
    else
    {
      TutorialNotification notification = gameObject.GetComponent<TutorialNotification>();
      if ((UnityEngine.Object) notification == (UnityEngine.Object) null)
      {
        Debug.LogError((object) "TutorialNotification component does not exist on TutorialIntroDialog prefab.");
      }
      else
      {
        TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, OverlayUI.Get().m_heightScale.m_Center);
        if ((bool) UniversalInputManager.UsePhoneUI)
          gameObject.transform.localScale = 1.5f * gameObject.transform.localScale;
        this.m_popupTutorialNotification = (Notification) notification;
        notification.headlineUberText.Text = GameStrings.Get(headlineGameString);
        notification.speechUberText.Text = GameStrings.Get(bodyTextGameString);
        notification.m_ButtonStart.SetText(GameStrings.Get(buttonGameString));
        RendererExtension.GetMaterial((Renderer) notification.artOverlay).mainTextureOffset = materialOffset;
        notification.m_ButtonStart.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e =>
        {
          UIEvent.Handler handler = buttonHandler;
          if (handler != null)
            handler(e);
          notification.m_ButtonStart.ClearEventListeners();
          NotificationManager.Get().DestroyNotification((Notification) notification, 0.0f);
          this.UpdateAllMercenaryAbilityOrderBubbleText();
        }));
        this.m_popupTutorialNotification.PlayBirth();
        UniversalInputManager.Get().SetGameDialogActive(true);
      }
    }
  }

  protected void DestroyNotification(Notification notification, bool hideImmediately = false)
  {
    if (!((UnityEngine.Object) notification != (UnityEngine.Object) null))
      return;
    if (hideImmediately)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(notification);
    else
      NotificationManager.Get().DestroyNotification(notification, 0.0f);
  }

  protected bool IsAnyFriendlyAbilitySelected()
  {
    foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity != null && entity.GetSelectedLettuceAbilityID() != 0)
        return true;
    }
    return false;
  }

  protected Card GetLeftMostMinionInFriendlyPlay()
  {
    foreach (Card card in GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards())
    {
      if (card.GetEntity().GetTag(GAME_TAG.ZONE_POSITION) == 1)
        return card;
    }
    return (Card) null;
  }

  protected Card GetRightMostMinionInFriendlyPlay()
  {
    List<Card> cards = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards();
    foreach (Card minionInFriendlyPlay in cards)
    {
      if (minionInFriendlyPlay.GetEntity().GetTag(GAME_TAG.ZONE_POSITION) == cards.Count)
        return minionInFriendlyPlay;
    }
    return (Card) null;
  }

  protected Card GetAbilityButtonBySlot(int abilitySlot)
  {
    List<Card> lettuceAbilityCards = ZoneMgr.Get().GetDisplayedLettuceAbilityCards();
    return abilitySlot >= lettuceAbilityCards.Count ? (Card) null : lettuceAbilityCards[abilitySlot];
  }

  protected void GetSpeakersForTeams(
    List<int> teams,
    EmoteType emoteType,
    out Card enemySpeaker,
    out Card friendlySpeaker)
  {
    enemySpeaker = friendlySpeaker = (Card) null;
    if (teams == null || teams.Count == 0 || emoteType == EmoteType.INVALID)
      return;
    List<Card> cardList1 = new List<Card>();
    cardList1.AddRange((IEnumerable<Card>) GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone().GetCards());
    cardList1.AddRange((IEnumerable<Card>) GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards());
    List<Card> cardList2 = new List<Card>();
    List<Card> cardList3 = new List<Card>();
    foreach (Card card in cardList1)
    {
      if (!((UnityEngine.Object) card == (UnityEngine.Object) null))
      {
        Entity entity = card.GetEntity();
        if (entity.IsMercenary() && entity.GetTag<TAG_PREMIUM>(GAME_TAG.PREMIUM) == TAG_PREMIUM.DIAMOND && card.GetEmoteEntry(emoteType) != null && (emoteType != EmoteType.START || card.GetEmoteEntry(EmoteType.MIRROR_START) != null))
        {
          int teamId = card.GetController().GetTeamId();
          if (teams.Contains(teamId))
          {
            if (card.GetEntity().IsControlledByFriendlySidePlayer())
              cardList2.Add(card);
            else
              cardList3.Add(card);
          }
        }
      }
    }
    if (cardList2.Count > 0)
      friendlySpeaker = cardList2[UnityEngine.Random.Range(0, cardList2.Count)];
    if (cardList3.Count <= 0)
      return;
    enemySpeaker = cardList3[UnityEngine.Random.Range(0, cardList3.Count)];
  }

  protected EmoteType ConvertHistoryVoBanterEventToEmoteType(
    PowerHistoryVoBanter.ClientEmoteEvent emoteType)
  {
    switch (emoteType)
    {
      case PowerHistoryVoBanter.ClientEmoteEvent.INVALID:
        return EmoteType.INVALID;
      case PowerHistoryVoBanter.ClientEmoteEvent.START:
        return EmoteType.START;
      case PowerHistoryVoBanter.ClientEmoteEvent.THREATEN:
        return EmoteType.THREATEN;
      case PowerHistoryVoBanter.ClientEmoteEvent.WELL_PLAYED:
        return EmoteType.WELL_PLAYED;
      default:
        Log.Gameplay.PrintWarning(MethodBase.GetCurrentMethod().ReflectedType.Name + "." + MethodBase.GetCurrentMethod().Name + "(): " + string.Format("Unknown Vo Banter Emote type: {0}. Unable to convert to {1}.", (object) emoteType, (object) typeof (EmoteType)));
        return EmoteType.INVALID;
    }
  }

  public void RegisterOnEmoteBanterPlayedEvent(
    LettuceMissionEntity.OnEmoteBanterPlayedDelegate callback)
  {
    if (this.m_onEmoteBanterPlayedCallbacks.Contains(callback))
      return;
    this.m_onEmoteBanterPlayedCallbacks.Add(callback);
  }

  public void UnregisterOnEmoteBanterPlayedEvent(
    LettuceMissionEntity.OnEmoteBanterPlayedDelegate callback)
  {
    this.m_onEmoteBanterPlayedCallbacks.Remove(callback);
  }

  private void OnEmoteBanterPlayed(EmoteType emoteType, AudioSource audioSource)
  {
    foreach (LettuceMissionEntity.OnEmoteBanterPlayedDelegate banterPlayedDelegate in this.m_onEmoteBanterPlayedCallbacks.ToArray())
      banterPlayedDelegate(this, emoteType, audioSource);
  }

  protected IEnumerator PlayEmoteBanterWithTiming(
    EmoteType emoteType,
    params Card[] speakers)
  {
    LettuceMissionEntity lettuceMissionEntity = this;
    if (speakers != null)
    {
      while (GameState.Get().IsBusy())
        yield return (object) null;
      lettuceMissionEntity.m_enemySpeaking = true;
      GameState.Get().SetBusy(true);
      for (int i = 0; i < speakers.Length; ++i)
      {
        Card speaker = speakers[i];
        if (!((UnityEngine.Object) speaker == (UnityEngine.Object) null))
        {
          if (i >= 1 && emoteType == EmoteType.START && (UnityEngine.Object) speakers[i - 1] != (UnityEngine.Object) null)
          {
            EntityDef entityDef1 = speakers[i].GetEntity().GetEntityDef();
            int num1 = int.MinValue;
            if (entityDef1 != null)
              num1 = GameUtils.GetMercenaryIdFromCardId(GameUtils.TranslateCardIdToDbId(entityDef1.GetCardId()));
            EntityDef entityDef2 = speakers[i - 1].GetEntity().GetEntityDef();
            int num2 = int.MinValue;
            if (entityDef1 != null)
              num2 = GameUtils.GetMercenaryIdFromCardId(GameUtils.TranslateCardIdToDbId(entityDef2.GetCardId()));
            if (num1 != int.MinValue && num2 != int.MinValue && num1 == num2)
              emoteType = EmoteType.MIRROR_START;
          }
          CardSoundSpell cardSoundSpell = speaker.PlayEmote(emoteType);
          if ((UnityEngine.Object) cardSoundSpell != (UnityEngine.Object) null && (UnityEngine.Object) cardSoundSpell.GetActiveAudioSource() != (UnityEngine.Object) null)
          {
            lettuceMissionEntity.OnEmoteBanterPlayed(emoteType, cardSoundSpell.GetActiveAudioSource());
            yield return (object) new WaitForSeconds(cardSoundSpell.GetActiveAudioSource().clip.length);
          }
          if (i < speakers.Length - 1)
            yield return (object) new WaitForSeconds(0.25f);
        }
      }
      GameState.Get().SetBusy(false);
      lettuceMissionEntity.m_enemySpeaking = false;
    }
  }

  public bool OnVoBanter_TeamDialogue(
    List<int> teams,
    PowerHistoryVoBanter.ClientEmoteEvent emoteEvent)
  {
    EmoteType emoteType = this.ConvertHistoryVoBanterEventToEmoteType(emoteEvent);
    if (emoteType == EmoteType.INVALID || teams == null || teams.Count == 0)
      return false;
    Card enemySpeaker;
    Card friendlySpeaker;
    this.GetSpeakersForTeams(teams, emoteType, out enemySpeaker, out friendlySpeaker);
    GameEntity.Coroutines.StartCoroutine(this.PlayEmoteBanterWithTiming(emoteType, enemySpeaker, friendlySpeaker));
    return true;
  }

  public bool OnVoBanter_OneSpeaker(int speakerId, PowerHistoryVoBanter.ClientEmoteEvent emoteEvent)
  {
    EmoteType emoteType = this.ConvertHistoryVoBanterEventToEmoteType(emoteEvent);
    if (emoteType == EmoteType.INVALID)
      return false;
    Entity entity = GameState.Get().GetEntity(speakerId);
    if (entity == null || entity.GetZone() != TAG_ZONE.PLAY)
      return false;
    GameEntity.Coroutines.StartCoroutine(this.PlayEmoteBanterWithTiming(emoteType, entity.GetCard()));
    return true;
  }

  public delegate void OnEmoteBanterPlayedDelegate(
    LettuceMissionEntity letlMissionEntity,
    EmoteType emoteType,
    AudioSource audioSource);

  protected class CardSpeedCamparer : IComparer<int>, IComparer<Card>
  {
    private bool m_lowToHigh;

    public CardSpeedCamparer(bool lowToHigh = true) => this.m_lowToHigh = lowToHigh;

    public int Compare(Card c1, Card c2) => this.Compare(c1.GetPreparedLettuceAbilitySpeedValue(), c2.GetPreparedLettuceAbilitySpeedValue());

    public int Compare(int speed1, int speed2) => this.m_lowToHigh ? speed1.CompareTo(speed2) : speed2.CompareTo(speed1);
  }
}
