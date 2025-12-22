using Assets;
using Blizzard.T5.Core;
using Hearthstone.Progression;
using PegasusGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TB_BaconShop : MissionEntity
{
  private static Map<GameEntityOption, bool> s_booleanOptions = TB_BaconShop.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = TB_BaconShop.InitStringOptions();
  private readonly WaitForSeconds MAX_DESTROY_HERO_TIME = new WaitForSeconds(10f);
  private AssetReference BACON_PHASE_POPUP = new AssetReference("BaconTurnIndicator.prefab:6342ffe02abc782459036566466d277c");
  private static readonly AssetReference Bob_BrassRing_Quote = new AssetReference("Bob_BrassRing_Quote.prefab:89385ff7d67aa1e49bcf25bc15ca61f6");
  protected int m_gamePhase = 1;
  private GameObject m_phasePopup;
  private bool m_gameplaySceneLoaded;
  private Coroutine m_destroyHeroTrackingCoroutine;
  private Notification m_techLevelCounter;
  private int m_displayedTechLevelNumber;
  private List<BaconHeroMulliganBestPlaceVisual> m_mulliganBestPlaceVisuals = new List<BaconHeroMulliganBestPlaceVisual>();
  private readonly EmoteType[] m_gameNotificationEmotes = new EmoteType[13]
  {
    EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_01,
    EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_02,
    EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_03,
    EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_04,
    EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_05,
    EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_06,
    EmoteType.BATTLEGROUNDS_VISUAL_TRIPLE,
    EmoteType.BATTLEGROUNDS_VISUAL_HOT_STREAK,
    EmoteType.BATTLEGROUNDS_VISUAL_KNOCK_OUT,
    EmoteType.BATTLEGROUNDS_VISUAL_BANANA,
    EmoteType.BATTLEGROUNDS_VISUAL_HERO_BUDDY,
    EmoteType.BATTLEGROUNDS_VISUAL_DOUBLE_HERO_BUDDY,
    EmoteType.BATTLEGROUNDS_VISUAL_QUEST_COMPLETE
  };
  private readonly EmoteType[] m_priorityEmotes = new EmoteType[1]
  {
    EmoteType.BATTLEGROUNDS_VISUAL_BANANA
  };
  private Map<int, bool> m_emotesAllowedForPlayer = new Map<int, bool>();
  private Map<int, QueueList<NotificationManager.SpeechBubbleOptions>> m_emotesQueuedForPlayer = new Map<int, QueueList<NotificationManager.SpeechBubbleOptions>>();
  private Map<int, LinkedList<NotificationManager.SpeechBubbleOptions>> m_gameNotificationsQueuedForPlayer = new Map<int, LinkedList<NotificationManager.SpeechBubbleOptions>>();
  private bool m_gameNotificationEmotesAllowed = true;
  private HashSet<string> m_heroesGreeted = new HashSet<string>();
  private HashSet<string> m_greetedByHeroes = new HashSet<string>();
  private GameObject m_duckObj;
  private SoundDucker m_fxDucker;
  private static readonly PlatformDependentValue<Vector3> BATTLEGROUNDS_MULLIGAN_ACTOR_SCALE = new PlatformDependentValue<Vector3>(PlatformCategory.Screen)
  {
    PC = new Vector3(1.5f, 1.1f, 1.5f),
    Phone = new Vector3(0.9f, 1.1f, 0.9f)
  };
  protected Notification m_buyButtonTutorialNotification;
  protected Notification m_enemyMinionTutorialNotification;
  protected Notification m_playMinionTutorialNotification;
  protected bool m_hasSeenBuyButtonTutorial;
  protected bool m_hasSeenEnemyMinionTutorial;
  protected bool m_hasSeenPlayMinionTutorial;
  protected Coroutine m_buyButtonTutorialCoroutine;
  protected Coroutine m_enemyMinionTutorialCoroutine;
  protected Coroutine m_playMinionTutorialCoroutine;
  private BaconGuideConfig m_GuideConfig;
  private string m_FavoriteGuideCardId;
  private long m_hasSeenInGameWinVO;
  private long m_hasSeenInGameLoseVO;
  private static readonly AssetReference GuideConfigManager = new AssetReference("GuideConfigManager.prefab:0ce1cf2cade0b7a4aab2f7eeda97b768");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_FirstDefeat_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_FirstDefeat_01.prefab:4ddd2298c91dc9649b98c65a0cef0760");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_FirstVictory_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_FirstVictory_01.prefab:e40b154f86185d3428ffa48867241f76");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_Hire_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_Hire_01.prefab:bfd9513b46b92e84da5f22e01a0387a4");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_RecruitWork_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_RecruitWork_01.prefab:a5e1a6db102be6d4495aa1cd7dc7ddfc");
  private static readonly AssetReference VO_DALA_BOSS_99h_Male_Human_ShopFirstTime_01 = new AssetReference("VO_DALA_BOSS_99h_Male_Human_ShopFirstTime_01.prefab:8070938a2c3ba2f4ea92b7f0b5fdf280");

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.ALWAYS_SHOW_MULLIGAN_TIMER,
      true
    },
    {
      GameEntityOption.MULLIGAN_IS_CHOOSE_ONE,
      true
    },
    {
      GameEntityOption.MULLIGAN_TIMER_HAS_ALTERNATE_POSITION,
      true
    },
    {
      GameEntityOption.CARDS_IN_TOOLTIP_SHIFTED_DURING_MULLIGAN,
      true
    },
    {
      GameEntityOption.MULLIGAN_HAS_HERO_LOBBY,
      true
    },
    {
      GameEntityOption.DIM_OPPOSING_HERO_DURING_MULLIGAN,
      true
    },
    {
      GameEntityOption.HANDLE_COIN,
      false
    },
    {
      GameEntityOption.MULLIGAN_USES_ALTERNATE_ACTORS,
      true
    },
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
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
      GameEntityOption.USE_COMPACT_ENCHANTMENT_BANNERS,
      true
    },
    {
      GameEntityOption.ALLOW_FATIGUE,
      false
    },
    {
      GameEntityOption.MOUSEOVER_DELAY_OVERRIDDEN,
      true
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
      GameEntityOption.HAS_ALTERNATE_ENEMY_EMOTE_ACTOR,
      true
    },
    {
      GameEntityOption.USES_PREMIUM_EMOTES,
      true
    },
    {
      GameEntityOption.CAN_SQUELCH_OPPONENT,
      true
    },
    {
      GameEntityOption.USES_BIG_CARDS,
      false
    },
    {
      GameEntityOption.DISPLAY_MULLIGAN_DETAIL_LABEL,
      true
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>()
  {
    {
      GameEntityOption.ALTERNATE_MULLIGAN_ACTOR_NAME,
      "Bacon_Leaderboard_Hero.prefab:776977f5238a24647adcd67933f7d4b0"
    },
    {
      GameEntityOption.ALTERNATE_MULLIGAN_LOBBY_ACTOR_NAME,
      "Bacon_Leaderboard_Hero.prefab:776977f5238a24647adcd67933f7d4b0"
    },
    {
      GameEntityOption.VICTORY_SCREEN_PREFAB_PATH,
      "BaconTwoScoop.prefab:1e3e06c045e65674f9a8afccb8bcdec4"
    },
    {
      GameEntityOption.DEFEAT_SCREEN_PREFAB_PATH,
      "BaconTwoScoop.prefab:1e3e06c045e65674f9a8afccb8bcdec4"
    },
    {
      GameEntityOption.RULEBOOK_POPUP_PREFAB_PATH,
      "BaconInfoPopup.prefab:d5b6f1d5443d48947891de53cdd6c323"
    },
    {
      GameEntityOption.DEFEAT_AUDIO_PATH,
      (string) null
    }
  };

  public BattlegroundsRatingChange RatingChangeData { get; set; }

  public TB_BaconShop()
    : base()
  {
    this.m_gameOptions.AddOptions(TB_BaconShop.s_booleanOptions, TB_BaconShop.s_stringOptions);
    HistoryManager.Get().DisableHistory();
    PlayerLeaderboardManager.Get().SetEnabled(true);
    if (GameMgr.Get().IsBattlegroundVsAIGame())
      PlayerLeaderboardManager.Get().SetAllowFakePlayers(true);
    EndTurnButton.Get().SetDisabled(true);
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
    this.InitializePhasePopup();
    this.InitializeTurnTimer();
    this.m_gamePhase = 1;
    GameEntity.Coroutines.StartCoroutine(this.OnShopPhase(false));
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.BACON, GameSaveKeySubkeyId.BACON_HAS_SEEN_FIRST_VICTORY_TUTORIAL, out this.m_hasSeenInGameWinVO);
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.BACON, GameSaveKeySubkeyId.BACON_HAS_SEEN_FIRST_DEFEAT_TUTORIAL, out this.m_hasSeenInGameLoseVO);
    Network.Get().RequestGameRoundHistory();
    Network.Get().RequestRealtimeBattlefieldRaces();
    Network.Get().RegisterNetHandler((object) BattlegroundsRatingChange.PacketID.ID, new Network.NetHandler(this.OnBattlegroundsRatingChange));
    if (GameState.Get() == null)
      return;
    GameState.Get().RegisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnEnded));
  }

  protected virtual string GetFavoriteBattlegroundsGuideSkinCardId() => CollectionManager.Get().GetFavoriteBattlegroundsGuideSkinCardId();

  private BaconGuideConfig GetGuideConfig()
  {
    if ((UnityEngine.Object) this.m_GuideConfig != (UnityEngine.Object) null)
      return this.m_GuideConfig;
    this.m_FavoriteGuideCardId = this.GetFavoriteBattlegroundsGuideSkinCardId();
    this.m_GuideConfig = TB_BaconShop.LoadGuideConfig(this.m_FavoriteGuideCardId);
    return this.m_GuideConfig;
  }

  public static BaconGuideConfig LoadGuideConfig(string cardId)
  {
    BaconGuideConfigManager component = AssetLoader.Get().InstantiatePrefab(TB_BaconShop.GuideConfigManager).GetComponent<BaconGuideConfigManager>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      Log.Gameplay.PrintError("TB_BaconShop: failed to load GuideConfigManager");
    BaconGuideConfig configForSkinCardId = component.GetGuideConfigForSkinCardId(cardId);
    UnityEngine.Object.Destroy((UnityEngine.Object) component);
    return configForSkinCardId;
  }

  public override void OnDecommissionGame()
  {
    if ((UnityEngine.Object) BaconBoard.Get() != (UnityEngine.Object) null)
      BaconBoard.Get().RemoveStateChangeCallback(new BaconBoard.StateChangeCallback(this.OnStateChange));
    if (SceneMgr.Get() != null)
      SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
    if (Network.Get() != null)
      Network.Get().RemoveNetHandler((object) BattlegroundsRatingChange.PacketID.ID, new Network.NetHandler(this.OnBattlegroundsRatingChange));
    if (GameState.Get() != null)
      GameState.Get().UnregisterTurnChangedListener(new GameState.TurnChangedCallback(this.OnTurnEnded));
    base.OnDecommissionGame();
  }

  private void OnGameplaySceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (mode != SceneMgr.Mode.GAMEPLAY)
      return;
    this.m_gameplaySceneLoaded = true;
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnGameplaySceneLoaded));
    ManaCrystalMgr.Get().SetEnemyManaCounterActive(false);
    this.OverrideZonePlayBaseTransitionTime();
    int tag = GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.NEXT_OPPONENT_PLAYER_ID);
    PlayerLeaderboardManager.Get().SetNextOpponent(tag);
    GameState.Get().GetOpposingSidePlayer().SetCardBackId(GameState.Get().GetFriendlySidePlayer().GetOriginalCardBackId());
    if (!((UnityEngine.Object) BaconBoard.Get() != (UnityEngine.Object) null))
      return;
    BaconBoard.Get().AddStateChangeCallback(new BaconBoard.StateChangeCallback(this.OnStateChange));
  }

  protected bool GetEnemyDeckTooltipContent(ref string headline, ref string description, int index)
  {
    switch (index)
    {
      case 0:
        List<TAG_RACE> list = ((IEnumerable<TAG_RACE>) GameState.Get().GetAvailableRacesInBattlegroundsExcludingAmalgam()).ToList<TAG_RACE>();
        list.Sort((Comparison<TAG_RACE>) ((a, b) => string.Compare(GameStrings.GetRaceNameBattlegrounds(a), GameStrings.GetRaceNameBattlegrounds(b), StringComparison.Ordinal)));
        if (list.Count == 5)
        {
          headline = GameStrings.Get("GAMEPLAY_TOOLTIP_BACON_AVAILABLE_RACES_HEADLINE");
          description = GameStrings.Format("GAMEPLAY_TOOLTIP_BACON_AVAILABLE_RACES_DESC", (object) GameStrings.GetRaceNameBattlegrounds(list[0]), (object) GameStrings.GetRaceNameBattlegrounds(list[1]), (object) GameStrings.GetRaceNameBattlegrounds(list[2]), (object) GameStrings.GetRaceNameBattlegrounds(list[3]), (object) GameStrings.GetRaceNameBattlegrounds(list[4]));
          return true;
        }
        break;
      case 1:
        List<TAG_RACE> racesInBattlegrounds = GameState.Get().GetMissingRacesInBattlegrounds();
        racesInBattlegrounds.Sort((Comparison<TAG_RACE>) ((a, b) => string.Compare(GameStrings.GetRaceNameBattlegrounds(a), GameStrings.GetRaceNameBattlegrounds(b), StringComparison.Ordinal)));
        headline = GameStrings.Get("GAMEPLAY_TOOLTIP_BACON_UNAVAILABLE_RACES_HEADLINE");
        string str = GameStrings.Get("GAMEPLAY_SEPARATOR") + " ";
        description = "";
        for (int index1 = 0; index1 < racesInBattlegrounds.Count; ++index1)
          description = string.Format("{0}{1}{2}", (object) description, index1 != 0 ? (object) str : (object) "", (object) GameStrings.GetRaceNameBattlegrounds(racesInBattlegrounds[index1]));
        return true;
    }
    return false;
  }

  protected bool GetFriendlyDeckTooltipContent(
    ref string headline,
    ref string description,
    int index)
  {
    if (index != 0)
      return false;
    int num = 4 - GameState.Get().GetFriendlySidePlayer().GetDeckZone().GetCards().Count;
    headline = GameStrings.Get("GAMEPLAY_TOOLTIP_BACON_DARKMOON_PRIZES_HEADLINE");
    description = GameStrings.Format("GAMEPLAY_TOOLTIP_BACON_DARKMOON_PRIZES_DESC", (object) num);
    return true;
  }

  protected bool GetFriendlyManaTooltipContent(
    ref string headline,
    ref string description,
    int index)
  {
    if (index != 0)
      return false;
    headline = GameStrings.Get("GAMEPLAY_TOOLTIP_MANA_COIN_HEADLINE");
    description = GameStrings.Get("GAMEPLAY_TOOLTIP_BACON_GOLD");
    return true;
  }

  public override InputManager.ZoneTooltipSettings GetZoneTooltipSettings()
  {
    bool allowed = GameState.Get().GetGameEntity().GetTag(GAME_TAG.DARKMOON_FAIRE_PRIZES_ACTIVE) == 1;
    return new InputManager.ZoneTooltipSettings()
    {
      EnemyDeck = new InputManager.TooltipSettings(true, new InputManager.TooltipContentDelegate(this.GetEnemyDeckTooltipContent)),
      EnemyHand = new InputManager.TooltipSettings(false),
      EnemyMana = new InputManager.TooltipSettings(false),
      FriendlyDeck = new InputManager.TooltipSettings(allowed, new InputManager.TooltipContentDelegate(this.GetFriendlyDeckTooltipContent)),
      FriendlyMana = new InputManager.TooltipSettings(true, new InputManager.TooltipContentDelegate(this.GetFriendlyManaTooltipContent))
    };
  }

  public override string GetMulliganDetailText()
  {
    List<TAG_RACE> list = ((IEnumerable<TAG_RACE>) GameState.Get().GetAvailableRacesInBattlegroundsExcludingAmalgam()).ToList<TAG_RACE>();
    if (list.Contains(TAG_RACE.INVALID))
      return (string) null;
    list.Sort((Comparison<TAG_RACE>) ((a, b) => string.Compare(GameStrings.GetRaceNameBattlegrounds(a), GameStrings.GetRaceNameBattlegrounds(b), StringComparison.Ordinal)));
    if (list.Count != 5)
      return (string) null;
    return GameStrings.Format("GAMEPLAY_BACON_MULLIGAN_AVAILABLE_RACES", (object) GameStrings.GetRaceNameBattlegrounds(list[0]), (object) GameStrings.GetRaceNameBattlegrounds(list[1]), (object) GameStrings.GetRaceNameBattlegrounds(list[2]), (object) GameStrings.GetRaceNameBattlegrounds(list[3]), (object) GameStrings.GetRaceNameBattlegrounds(list[4]));
  }

  public override Vector3 NameBannerPosition(Player.Side side) => side == Player.Side.FRIENDLY ? new Vector3(0.0f, 5f, 11f) : base.NameBannerPosition(side);

  public override Vector3 GetMulliganTimerAlternatePosition()
  {
    if ((UnityEngine.Object) MulliganManager.Get() == (UnityEngine.Object) null || (UnityEngine.Object) MulliganManager.Get().GetMulliganBanner() == (UnityEngine.Object) null)
      return new Vector3(100f, 0.0f, 0.0f);
    if (GameState.Get().IsInChoiceMode() && (UnityEngine.Object) MulliganManager.Get().GetMulliganButton() != (UnityEngine.Object) null)
      return MulliganManager.Get().GetMulliganButton().transform.position;
    return (bool) UniversalInputManager.UsePhoneUI ? MulliganManager.Get().GetMulliganBanner().transform.position + new Vector3(-1.8f, 0.0f, -0.91f) : MulliganManager.Get().GetMulliganBanner().transform.position;
  }

  protected override Spell BlowUpHero(Card card, SpellType spellType)
  {
    if ((UnityEngine.Object) card != (UnityEngine.Object) null && (UnityEngine.Object) card.GetActor() != (UnityEngine.Object) null)
    {
      PlayMakerFSM component = card.GetActor().GetComponent<PlayMakerFSM>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.enabled = false;
    }
    if (GameState.Get().IsMulliganManagerActive())
    {
      Transform parent = card.GetActor().gameObject.transform.parent;
      parent.position = new Vector3(-7.7726f, 0.0055918f, -8.054f);
      parent.localScale = new Vector3(1.134f, 1.134f, 1.134f);
      MulliganManager.Get().StopAllCoroutines();
    }
    return base.BlowUpHero(card, spellType);
  }

  protected override Spell ActivateSpellForDestroyedHero(Card card, SpellType spellType)
  {
    if (spellType != SpellType.ENDGAME_LOSE || this.m_gamePhase != 2)
      return base.ActivateSpellForDestroyedHero(card, spellType);
    Entity hero1 = GameState.Get().GetOpposingSidePlayer().GetHero();
    FinisherGameplaySettings gameplaySettings = FinisherGameplaySettings.GetFinisherGameplaySettings(hero1);
    string destroyPlayerPrefab;
    if (!string.IsNullOrEmpty(gameplaySettings.FirstPlaceVictoryDestroyPlayerPrefab) && GameState.Get().CountPlayersAlive() == 1)
    {
      destroyPlayerPrefab = gameplaySettings.FirstPlaceVictoryDestroyPlayerPrefab;
    }
    else
    {
      if (string.IsNullOrEmpty(gameplaySettings.DestroyPlayerPrefab))
        return card.ActivateActorSpell(spellType);
      destroyPlayerPrefab = gameplaySettings.DestroyPlayerPrefab;
    }
    Spell spell = SpellManager.Get().GetSpell(destroyPlayerPrefab);
    if ((UnityEngine.Object) spell != (UnityEngine.Object) null)
      spell.SetSpellType(spellType);
    Entity hero2 = GameState.Get().GetFriendlySidePlayer().GetHero();
    GameObject gameObject1 = hero1.GetCard().gameObject;
    GameObject gameObject2 = hero2.GetCard().gameObject;
    spell.SetSource(gameObject1);
    spell.m_Location = SpellLocation.SOURCE;
    if (spell is SuperSpell)
      (spell as SuperSpell).m_TargetInfo.m_Behavior = SpellTargetBehavior.DEFAULT;
    spell.AddTarget(gameObject2);
    spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnFriendlyHeroDestroyed));
    this.m_destroyHeroTrackingCoroutine = spell.StartCoroutine(this.EnsureHeroDestroyedCompletes(spell));
    spell.Activate();
    return spell;
  }

  private void OnFriendlyHeroDestroyed(Spell spell, object _)
  {
    if (this.m_destroyHeroTrackingCoroutine == null)
      return;
    spell.StopCoroutine(this.m_destroyHeroTrackingCoroutine);
    this.m_destroyHeroTrackingCoroutine = (Coroutine) null;
  }

  private IEnumerator EnsureHeroDestroyedCompletes(Spell spell)
  {
    yield return (object) this.MAX_DESTROY_HERO_TIME;
    this.m_destroyHeroTrackingCoroutine = (Coroutine) null;
    Log.Spells.PrintError("Destroy hero spell " + spell.gameObject.name + " did not terminate and was killed to prevent game hang. Run the finisher in the authoring scene to diagnose potential problems.");
    spell.ReleaseSpell();
  }

  public override bool ShouldDelayShowingCardInTooltip() => !GameState.Get().IsMulliganManagerActive();

  public override ActorStateType GetMulliganChoiceHighlightState() => ActorStateType.CARD_SELECTABLE;

  public override bool IsHeroMulliganLobbyFinished() => !GameState.Get().IsMulliganPhase() || this.CountPlayersFinishedMulligan() == this.CountPlayersInGame();

  private int CountPlayersFinishedMulligan()
  {
    int num = 0;
    foreach (SharedPlayerInfo sharedPlayerInfo in GameState.Get().GetPlayerInfoMap().Values)
    {
      if (sharedPlayerInfo.GetPlayerHero() != null)
        ++num;
    }
    return num;
  }

  private int CountPlayersInGame() => GameState.Get().GetPlayerInfoMap().Values.Count;

  public override bool ShouldDoAlternateMulliganIntro() => true;

  public override bool DoAlternateMulliganIntro()
  {
    if (!this.ShouldDoAlternateMulliganIntro())
      return false;
    GameEntity.Coroutines.StartCoroutine(this.DoBaconAlternateMulliganIntroWithTiming());
    return true;
  }

  protected override void HandleMulliganTagChange() => MulliganManager.Get().BeginMulligan();

  public override Vector3 GetAlternateMulliganActorScale() => (Vector3) TB_BaconShop.BATTLEGROUNDS_MULLIGAN_ACTOR_SCALE;

  public override int GetNumberOfFakeMulliganCardsToShowOnLeft(int numOriginalCards) => numOriginalCards >= 3 ? 0 : 1;

  public override int GetNumberOfFakeMulliganCardsToShowOnRight(int numOriginalCards) => numOriginalCards >= 4 ? 0 : 1;

  public override void ConfigureFakeMulliganCardActor(Actor actor, bool shown)
  {
    PlayerLeaderboardMainCardActor leaderboardMainCardActor = actor as PlayerLeaderboardMainCardActor;
    if ((UnityEngine.Object) leaderboardMainCardActor == (UnityEngine.Object) null)
      return;
    leaderboardMainCardActor.ToggleLockedHeroView(shown);
  }

  public override bool IsGameSpeedupConditionInEffect() => !((UnityEngine.Object) Gameplay.Get() == (UnityEngine.Object) null) && GameState.Get() != null && GameState.Get().GetGameEntity() != null && GameState.Get().GetGameEntity().HasTag(GAME_TAG.ALLOW_GAME_SPEEDUP) && this.m_gamePhase == 2;

  public override void ApplyMulliganActorStateChanges(Actor baseActor)
  {
    PlayerLeaderboardMainCardActor leaderboardMainCardActor = (PlayerLeaderboardMainCardActor) baseActor;
    leaderboardMainCardActor.SetAlternateNameTextActive(false);
    leaderboardMainCardActor.m_playerNameBackground.SetActive(false);
    leaderboardMainCardActor.m_nameTextMesh.gameObject.SetActive(true);
  }

  public override void ApplyMulliganActorLobbyStateChanges(Actor baseActor)
  {
    PlayerLeaderboardMainCardActor leaderboardMainCardActor = (PlayerLeaderboardMainCardActor) baseActor;
    leaderboardMainCardActor.SetAlternateNameTextActive(false);
    leaderboardMainCardActor.m_nameTextMesh.gameObject.SetActive(false);
    leaderboardMainCardActor.m_playerNameBackground.SetActive(true);
    leaderboardMainCardActor.SetFullyHighlighted(false);
  }

  public override void ClearMulliganActorStateChanges(Actor baseActor)
  {
    PlayerLeaderboardMainCardActor leaderboardMainCardActor = (PlayerLeaderboardMainCardActor) baseActor;
    leaderboardMainCardActor.SetAlternateNameTextActive(false);
    leaderboardMainCardActor.m_nameTextMesh.gameObject.SetActive(false);
    leaderboardMainCardActor.m_playerNameBackground.SetActive(false);
    leaderboardMainCardActor.m_playerNameText.gameObject.SetActive(false);
    leaderboardMainCardActor.SetFullyHighlighted(false);
  }

  public override string GetMulliganBannerText() => GameStrings.Get("GAMEPLAY_BACON_MULLIGAN_CHOOSE_HERO_BANNER");

  public override string GetMulliganBannerSubtitleText() => (string) null;

  public override string GetMulliganWaitingText() => string.Format(GameStrings.Get("GAMEPLAY_BACON_MULLIGAN_WAITING_BANNER"), (object) this.CountPlayersFinishedMulligan(), (object) this.CountPlayersInGame());

  public override string GetMulliganWaitingSubtitleText() => (UnityEngine.Object) MulliganManager.Get() != (UnityEngine.Object) null && MulliganManager.Get().IsMulliganTimerActive() ? GameStrings.Get("GAMEPLAY_BACON_MULLIGAN_WAITING_BANNER_SUBTITLE") : (string) null;

  public override void QueueEntityForRemoval(Entity entity) => GameState.Get().QueueEntityForRemoval(entity);

  protected IEnumerator DoBaconAlternateMulliganIntroWithTiming()
  {
    SceneMgr.Get().NotifySceneLoaded();
    MulliganManager.Get().LoadMulliganButton();
    while (LoadingScreen.Get().IsPreviousSceneActive() || LoadingScreen.Get().IsFadingOut())
      yield return (object) null;
    GameMgr.Get().UpdatePresence();
    GameState.Get().GetGameEntity().NotifyOfHeroesFinishedAnimatingInMulligan();
    ScreenEffectsMgr.Get().SetActive(true);
  }

  public override void OnMulliganCardsDealt(List<Card> startingCards)
  {
    foreach (Card startingCard in startingCards)
      AssetLoader.Get().InstantiatePrefab(new AssetReference("BaconHeroMulliganBestPlaceVisual.prefab:6e6437cf53cbc0e4fbf0b3d6ce5a6856"), new PrefabCallback<GameObject>(this.OnBestPlaceVisualLoaded), (object) startingCard);
  }

  private void OnBestPlaceVisualLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    Card card = (Card) callbackData;
    int dbId = GameUtils.TranslateCardIdToDbId(card.GetEntity().GetCardId());
    int bestPlaceForHero = this.GetBestPlaceForHero(dbId);
    BaconHeroMulliganBestPlaceVisual component = go.GetComponent<BaconHeroMulliganBestPlaceVisual>();
    this.m_mulliganBestPlaceVisuals.Add(component);
    component.SetVisualActive(bestPlaceForHero, dbId);
    GameUtils.SetParent(go, card.gameObject);
  }

  private int GetBestPlaceForHero(int heroId)
  {
    List<long> values1;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.BACON, GameSaveKeySubkeyId.BACON_BEST_HERO_PLACE, out values1);
    List<long> values2;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.BACON, GameSaveKeySubkeyId.BACON_BEST_HERO_PLACE_HERO, out values2);
    if (values1 == null || values2 == null)
      return int.MaxValue;
    if (values1.Count != values2.Count)
    {
      Debug.LogError((object) "Error in GetBestPlaceForHero: List size mismatch!");
      return int.MaxValue;
    }
    for (int index = 0; index < values2.Count; ++index)
    {
      if (values2[index] == (long) heroId && index < values1.Count)
        return (int) values1[index];
    }
    return int.MaxValue;
  }

  public override void OnMulliganBeginDealNewCards()
  {
    foreach (BaconHeroMulliganBestPlaceVisual mulliganBestPlaceVisual in this.m_mulliganBestPlaceVisuals)
    {
      if ((UnityEngine.Object) mulliganBestPlaceVisual != (UnityEngine.Object) null)
        mulliganBestPlaceVisual.Hide();
    }
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  private void OverrideZonePlayBaseTransitionTime()
  {
    if (GameState.Get() == null)
      return;
    ZonePlay battlefieldZone1 = GameState.Get().GetFriendlySidePlayer().GetBattlefieldZone();
    ZonePlay battlefieldZone2 = GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone();
    battlefieldZone1.OverrideBaseTransitionTime(0.5f);
    battlefieldZone1.ResetTransitionTime();
    battlefieldZone2.OverrideBaseTransitionTime(0.5f);
    battlefieldZone2.ResetTransitionTime();
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    TB_BaconShop tbBaconShop = this;
    if (missionEvent == 1)
    {
      tbBaconShop.m_gamePhase = 1;
      yield return (object) tbBaconShop.OnShopPhase(true);
    }
    if (missionEvent == 5)
    {
      tbBaconShop.m_gamePhase = 1;
      yield return (object) tbBaconShop.OnShopPhase(false);
    }
    if (missionEvent == 2)
    {
      tbBaconShop.m_gamePhase = 2;
      yield return (object) tbBaconShop.OnCombatPhase();
    }
    if (missionEvent == 3)
    {
      if (tbBaconShop.GetFreezeButtonCard().GetEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_1) >= tbBaconShop.GetFreezeButtonCard().GetEntity().GetTag(GAME_TAG.TAG_SCRIPT_DATA_NUM_2) - 1)
      {
        tbBaconShop.SetInputEnableForFrozenButton(false);
      }
      else
      {
        tbBaconShop.SetInputEnableForFrozenButton(false);
        yield return (object) new WaitForSeconds(0.75f);
        tbBaconShop.SetInputEnableForFrozenButton(true);
      }
    }
    if (missionEvent == 4)
    {
      tbBaconShop.SetInputEnableForRefreshButton(false);
      yield return (object) new WaitForSeconds(0.75f);
      tbBaconShop.SetInputEnableForRefreshButton(true);
    }
    while (tbBaconShop.m_enemySpeaking)
      yield return (object) null;
    Actor bobActor = tbBaconShop.GetBobActor();
    if ((UnityEngine.Object) bobActor == (UnityEngine.Object) null || bobActor.GetEntity() == null)
      yield return (object) null;
    Actor friendlyActor = (Actor) null;
    string voLine1 = (string) null;
    string voLine2;
    Actor heroActor;
    switch (missionEvent)
    {
      case 101:
        if (!tbBaconShop.ShouldPlayRateVO(0.25f) || tbBaconShop.m_enemySpeaking)
          break;
        string randomShopUpgradeLine = tbBaconShop.GetGuideConfig().GetRandomShopUpgradeLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(randomShopUpgradeLine);
        break;
      case 102:
        if (tbBaconShop.m_enemySpeaking)
          break;
        string highestTierLine = tbBaconShop.GetGuideConfig().GetHighestTierLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(highestTierLine);
        break;
      case 103:
        if (!tbBaconShop.ShouldPlayRateVO(0.15f) || tbBaconShop.m_enemySpeaking)
          break;
        string recruitSmallLine = tbBaconShop.GetGuideConfig().GetRandomRecruitSmallLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(recruitSmallLine);
        break;
      case 104:
        if (!tbBaconShop.ShouldPlayRateVO(0.2f) || tbBaconShop.m_enemySpeaking)
          break;
        string recruitMediumLine = tbBaconShop.GetGuideConfig().GetRandomRecruitMediumLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(recruitMediumLine);
        break;
      case 105:
        if (!tbBaconShop.ShouldPlayRateVO(0.25f) || tbBaconShop.m_enemySpeaking)
          break;
        string recruitLargeLine = tbBaconShop.GetGuideConfig().GetRandomRecruitLargeLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(recruitLargeLine);
        break;
      case 106:
        if (!tbBaconShop.ShouldPlayRateVO(0.25f) || tbBaconShop.m_enemySpeaking)
          break;
        string randomTripleLine = tbBaconShop.GetGuideConfig().GetRandomTripleLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(randomTripleLine);
        break;
      case 107:
        if (!tbBaconShop.ShouldPlayRateVO(0.15f) || tbBaconShop.m_enemySpeaking)
          break;
        string randomSellingLine = tbBaconShop.GetGuideConfig().GetRandomSellingLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(randomSellingLine);
        break;
      case 108:
        if (!tbBaconShop.ShouldPlayRateVO(0.1f) || tbBaconShop.m_enemySpeaking)
          break;
        string randomFreezingLine = tbBaconShop.GetGuideConfig().GetRandomFreezingLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(randomFreezingLine);
        break;
      case 109:
        if (!tbBaconShop.ShouldPlayRateVO(0.1f) || tbBaconShop.m_enemySpeaking)
          break;
        string randomRefreshLine = tbBaconShop.GetGuideConfig().GetRandomRefreshLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(randomRefreshLine);
        break;
      case 110:
        if (!tbBaconShop.ShouldPlayRateVO(0.25f) || tbBaconShop.m_enemySpeaking)
          break;
        string possibleTripleLine = tbBaconShop.GetGuideConfig().GetRandomPossibleTripleLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(possibleTripleLine);
        break;
      case 111:
        if (tbBaconShop.m_enemySpeaking)
          break;
        friendlyActor = tbBaconShop.GetFriendlyHeroActor();
        if ((UnityEngine.Object) friendlyActor != (UnityEngine.Object) null && (UnityEngine.Object) friendlyActor.LegendaryHeroSkinConfig != (UnityEngine.Object) null && !friendlyActor.LegendaryHeroSkinConfig.CheckBartenderGreetLine(tbBaconShop.GetFavoriteBattlegroundsGuideSkinCardId(), out voLine2) && friendlyActor.LegendaryHeroSkinConfig.CheckStartGameLine(out voLine1))
        {
          yield return (object) tbBaconShop.PlayVOLineWithOffsetBubble(voLine1, friendlyActor);
        }
        else
        {
          Entity hero = GameState.Get().GetFriendlySidePlayer().GetHero();
          string battlegroundsBaseHeroCardId = CollectionManager.Get().GetBattlegroundsBaseHeroCardId(hero.GetCardId());
          if (tbBaconShop.GetGuideConfig().CheckHeroSpecificLine(battlegroundsBaseHeroCardId, out voLine1))
          {
            yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(voLine1);
          }
          else
          {
            voLine1 = tbBaconShop.GetGuideConfig().GetRandomNewGameLine();
            yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(voLine1);
          }
        }
        string voLine3;
        if (!((UnityEngine.Object) friendlyActor != (UnityEngine.Object) null) || !((UnityEngine.Object) friendlyActor.LegendaryHeroSkinConfig != (UnityEngine.Object) null) || !friendlyActor.LegendaryHeroSkinConfig.CheckBartenderGreetLine(tbBaconShop.GetFavoriteBattlegroundsGuideSkinCardId(), out voLine3))
          break;
        yield return (object) tbBaconShop.PlayVOLineWithOffsetBubble(voLine3, friendlyActor);
        break;
      case 112:
        if (tbBaconShop.m_enemySpeaking)
          break;
        string combatGeneralLine = tbBaconShop.GetGuideConfig().GetRandomPostCombatGeneralLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(combatGeneralLine);
        break;
      case 113:
        friendlyActor = tbBaconShop.GetFriendlyHeroActor();
        int friendlyPlayerId = GameState.Get().GetFriendlyPlayerId();
        int winStreakForPlayer = PlayerLeaderboardManager.Get().GetLatestWinStreakForPlayer(friendlyPlayerId);
        if ((UnityEngine.Object) friendlyActor != (UnityEngine.Object) null && (UnityEngine.Object) friendlyActor.LegendaryHeroSkinConfig != (UnityEngine.Object) null)
          friendlyActor.LegendaryHeroSkinConfig.TryActivateVFX_WinStreak(winStreakForPlayer);
        if (tbBaconShop.m_enemySpeaking)
          break;
        if ((UnityEngine.Object) friendlyActor != (UnityEngine.Object) null && (UnityEngine.Object) friendlyActor.LegendaryHeroSkinConfig != (UnityEngine.Object) null && friendlyActor.LegendaryHeroSkinConfig.CheckWinStreakLine(winStreakForPlayer, out voLine1))
        {
          yield return (object) tbBaconShop.PlayVOLineWithOffsetBubble(voLine1, tbBaconShop.GetFriendlyHeroActor());
          break;
        }
        string postCombatWinLine = tbBaconShop.GetGuideConfig().GetRandomPostCombatWinLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(postCombatWinLine);
        break;
      case 114:
        if (tbBaconShop.m_enemySpeaking)
          break;
        string postCombatLoseLine = tbBaconShop.GetGuideConfig().GetRandomPostCombatLoseLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(postCombatLoseLine);
        break;
      case 115:
        if (tbBaconShop.m_enemySpeaking || tbBaconShop.CheckHeroGreet(out heroActor, out voLine2, false))
          break;
        string postShopGeneralLine = tbBaconShop.GetGuideConfig().GetRandomPostShopGeneralLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(postShopGeneralLine);
        break;
      case 116:
        if (tbBaconShop.m_enemySpeaking || tbBaconShop.CheckHeroGreet(out heroActor, out voLine2, false))
          break;
        string postShopLoseLine = tbBaconShop.GetGuideConfig().GetRandomPostShopLoseLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(postShopLoseLine);
        break;
      case 117:
        if (tbBaconShop.m_enemySpeaking || tbBaconShop.CheckHeroGreet(out heroActor, out voLine2, false))
          break;
        string randomPostShopWinLine = tbBaconShop.GetGuideConfig().GetRandomPostShopWinLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(randomPostShopWinLine);
        break;
      case 118:
        if (tbBaconShop.m_enemySpeaking || tbBaconShop.CheckHeroGreet(out heroActor, out voLine2, false))
          break;
        string postShopIsFirstLine = tbBaconShop.GetGuideConfig().GetRandomPostShopIsFirstLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(postShopIsFirstLine);
        break;
      case 119:
        if (tbBaconShop.m_enemySpeaking)
          break;
        string afkLine = tbBaconShop.GetGuideConfig().GetAFKLine();
        yield return (object) tbBaconShop.PlayBobLineWithOffsetBubble(afkLine);
        break;
      case 120:
        if (tbBaconShop.m_enemySpeaking)
          break;
        yield return (object) tbBaconShop.HandleKnockoutVO();
        break;
      case 121:
        if (tbBaconShop.m_enemySpeaking)
          break;
        yield return (object) tbBaconShop.PlayWisdomballVOLine("VO_DALA_BOSS_60h_Male_Human_FloatingHead_Trigger_HealPlayer_01.prefab:d0a3f9b5c01e04d458178ca8c5069d66");
        break;
      case 122:
        if (tbBaconShop.m_enemySpeaking)
          break;
        yield return (object) tbBaconShop.PlayWisdomballVOLine("VO_DALA_BOSS_60h_Male_Human_FloatingHead_Trigger_SelfDamage_01.prefab:ce7a5a15de006d041ad515427fc6f72f");
        break;
      case 123:
        if (tbBaconShop.m_enemySpeaking)
          break;
        yield return (object) tbBaconShop.PlayWisdomballVOLine("VO_DALA_BOSS_60h_Male_Human_FloatingHead_Trigger_MirrorImage_01.prefab:8789714bb9a92d143bb2024188b8ddd0");
        break;
      case 124:
        if (tbBaconShop.m_enemySpeaking)
          break;
        yield return (object) tbBaconShop.PlayWisdomballVOLine("VO_DALA_BOSS_60h_Male_Human_FloatingHead_Trigger_CopyCards_01.prefab:ad01bc4d23eab3e4f86c994d722cf247");
        break;
      case 125:
        if (tbBaconShop.m_enemySpeaking)
          break;
        yield return (object) tbBaconShop.PlayWisdomballVOLine("VO_DALA_BOSS_60h_Male_Human_FloatingHead_Trigger_Treasure_01.prefab:b9fae030ab3026a4bb17f592028c276d");
        break;
      case 126:
        if (tbBaconShop.m_enemySpeaking)
          break;
        yield return (object) tbBaconShop.PlayWisdomballVOLine("VO_DALA_BOSS_60h_Male_Human_FloatingHead_Trigger_RandomLegendary_01.prefab:9273a8457f705514f9755153f0c7abf6");
        break;
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    TB_BaconShop tbBaconShop = this;
    AchievementManager.Get().UnpauseToastNotifications();
    PlayerLeaderboardManager.Get().UpdateLayout();
    if (gameResult == TAG_PLAYSTATE.WON && tbBaconShop.m_hasSeenInGameWinVO == 0L)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) Gameplay.Get().StartCoroutine(tbBaconShop.PlayBigCharacterQuoteAndWait((string) TB_BaconShop.Bob_BrassRing_Quote, (string) TB_BaconShop.VO_DALA_BOSS_99h_Male_Human_FirstVictory_01));
    }
    int leaderboardPlace = GameState.Get().GetFriendlySidePlayer().GetHero().GetRealTimePlayerLeaderboardPlace();
    if (gameResult == TAG_PLAYSTATE.LOST && tbBaconShop.m_hasSeenInGameLoseVO == 0L && leaderboardPlace > 4)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) Gameplay.Get().StartCoroutine(tbBaconShop.PlayBigCharacterQuoteAndWait((string) TB_BaconShop.Bob_BrassRing_Quote, (string) TB_BaconShop.VO_DALA_BOSS_99h_Male_Human_FirstDefeat_01));
    }
  }

  protected virtual IEnumerator OnShopPhase(bool expectStateChangeCallback)
  {
    AchievementManager.Get().UnpauseToastNotifications();
    yield return (object) this.ShowPopup("Shop", expectStateChangeCallback);
    PlayerLeaderboardManager.Get().UpdateLayout();
    GameState.Get().GetOpposingSidePlayer().UpdateDisplayInfo();
    this.UpdateNameBanner();
    this.ShowTechLevelDisplay(true);
    yield return (object) new WaitForSeconds(3f);
    this.ShowShopTutorials();
    this.SetGameNotificationEmotesEnabled(true);
    GameState.Get().GetTimeTracker().ResetAccruedLostTime();
  }

  protected virtual IEnumerator OnCombatPhase()
  {
    ZoneMgr.Get().AutoCorrectZones(ZoneMgr.Get().GetCancellationToken(), false);
    this.HideShopTutorials();
    yield return (object) this.ShowPopup("Combat", true);
    GameEntity.Coroutines.StartCoroutine(this.WaitAndHideActiveSpeechBubble());
    this.ShowTechLevelDisplay(false);
    GameState.Get().GetOpposingSidePlayer().UpdateDisplayInfo();
    this.UpdateNameBanner();
    this.ForceShowFriendlyHeroActor();
    InputManager.Get().HidePhoneHand();
    GameState.Get().GetTimeTracker().ResetAccruedLostTime();
    Actor friendlyHeroActor = this.GetFriendlyHeroActor();
    Actor opposingHeroActor = this.GetOpposingHeroActor();
    this.TriggerCombatStartLegendaryVFX(friendlyHeroActor);
    this.TriggerCombatStartLegendaryVFX(opposingHeroActor);
  }

  public override void HandleRealTimeMissionEvent(int missionEvent)
  {
    if (missionEvent != 2)
      return;
    this.SetGameNotificationEmotesEnabled(false);
  }

  private bool TriggerCombatStartLegendaryVFX(Actor actor) => (UnityEngine.Object) actor != (UnityEngine.Object) null && (UnityEngine.Object) actor.LegendaryHeroSkinConfig != (UnityEngine.Object) null && actor.LegendaryHeroSkinConfig.TryActivateVFX_CombatStart();

  private bool CheckHeroGreet(out Actor heroActor, out string voLine, bool setGreeted = true)
  {
    Actor friendlyHeroActor = this.GetFriendlyHeroActor();
    Actor opposingHeroActor = this.GetOpposingHeroActor();
    string heroCardId = !((UnityEngine.Object) friendlyHeroActor != (UnityEngine.Object) null) || friendlyHeroActor.GetEntity() == null ? (string) null : friendlyHeroActor.GetEntity().GetCardId();
    string str = !((UnityEngine.Object) opposingHeroActor != (UnityEngine.Object) null) || opposingHeroActor.GetEntity() == null ? (string) null : opposingHeroActor.GetEntity().GetCardId();
    string line1 = (string) null;
    string line2 = (string) null;
    bool flag1 = (UnityEngine.Object) friendlyHeroActor != (UnityEngine.Object) null && (UnityEngine.Object) friendlyHeroActor.LegendaryHeroSkinConfig != (UnityEngine.Object) null && friendlyHeroActor.LegendaryHeroSkinConfig.CheckGreetLine(str, out line1) && !this.m_heroesGreeted.Contains(str);
    bool flag2 = (UnityEngine.Object) opposingHeroActor != (UnityEngine.Object) null && (UnityEngine.Object) opposingHeroActor.LegendaryHeroSkinConfig != (UnityEngine.Object) null && opposingHeroActor.LegendaryHeroSkinConfig.CheckGreetLine(heroCardId, out line2) && !this.m_greetedByHeroes.Contains(str);
    if (!flag1 && !flag2)
    {
      heroActor = (Actor) null;
      voLine = (string) null;
      return false;
    }
    if (flag1 & flag2)
    {
      int currentDefense1 = friendlyHeroActor.GetEntity().GetCurrentDefense();
      int currentDefense2 = opposingHeroActor.GetEntity().GetCurrentDefense();
      bool flag3;
      if (currentDefense1 == currentDefense2)
      {
        if (heroCardId.CompareTo(str) < 0)
          flag3 = false;
        else
          flag1 = false;
      }
      else if (currentDefense1 > currentDefense2)
        flag3 = false;
      else
        flag1 = false;
    }
    if (flag1)
    {
      voLine = line1;
      heroActor = friendlyHeroActor;
      if (setGreeted)
        this.m_heroesGreeted.Add(str);
    }
    else
    {
      voLine = line2;
      heroActor = opposingHeroActor;
      if (setGreeted)
        this.m_greetedByHeroes.Add(str);
    }
    return true;
  }

  private IEnumerator HandleKnockoutVO()
  {
    Actor friendlyHeroActor = this.GetFriendlyHeroActor();
    Actor opposingHeroActor = this.GetOpposingHeroActor();
    if ((UnityEngine.Object) friendlyHeroActor != (UnityEngine.Object) null && (UnityEngine.Object) opposingHeroActor != (UnityEngine.Object) null && ((UnityEngine.Object) friendlyHeroActor.LegendaryHeroSkinConfig != (UnityEngine.Object) null || (UnityEngine.Object) opposingHeroActor.LegendaryHeroSkinConfig != (UnityEngine.Object) null))
    {
      int atk1 = friendlyHeroActor.GetEntity().GetATK();
      int atk2 = opposingHeroActor.GetEntity().GetATK();
      int currentDefense1 = friendlyHeroActor.GetEntity().GetCurrentDefense();
      int currentDefense2 = opposingHeroActor.GetEntity().GetCurrentDefense();
      if ((atk1 > 0 || atk2 > 0) && currentDefense2 != 0 && currentDefense1 != 0)
      {
        if (atk1 > atk2)
          yield return (object) this.PlayVOIfLethal(friendlyHeroActor, GameState.Get().GetFriendlySidePlayer(), currentDefense2);
        else
          yield return (object) this.PlayVOIfLethal(opposingHeroActor, GameState.Get().GetOpposingSidePlayer(), currentDefense1);
      }
    }
  }

  private IEnumerator PlayVOIfLethal(Actor actor, Player player, int defense)
  {
    string line = (string) null;
    if (player != null && (UnityEngine.Object) actor != (UnityEngine.Object) null && (UnityEngine.Object) actor.LegendaryHeroSkinConfig != (UnityEngine.Object) null && actor.LegendaryHeroSkinConfig.CheckKnockoutLine(out line))
    {
      int num = player.GetTag(GAME_TAG.PLAYER_TECH_LEVEL) + this.GetPlayerCombinedMinionsTechLevel(player);
      int tag = GameState.Get().GetGameEntity().GetTag(GAME_TAG.BACON_COMBAT_DAMAGE_CAP);
      if (tag != 0 && num > tag)
        num = tag;
      if (num >= defense)
      {
        this.StartDuckingFx();
        yield return (object) this.PlayVOLineWithOffsetBubble(line, actor);
        this.StopDuckingFx();
      }
    }
  }

  private int GetPlayerCombinedMinionsTechLevel(Player player)
  {
    if (player == null)
      return 0;
    int minionsTechLevel = 0;
    foreach (Card card in player.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetController() == player && entity.IsMinion())
        minionsTechLevel += entity.GetTechLevel();
    }
    return minionsTechLevel;
  }

  private void StartDuckingFx()
  {
    if ((UnityEngine.Object) this.m_duckObj == (UnityEngine.Object) null)
      this.m_duckObj = new GameObject();
    if ((UnityEngine.Object) this.m_fxDucker == (UnityEngine.Object) null)
    {
      this.m_fxDucker = this.m_duckObj.AddComponent<SoundDucker>();
      List<SoundDuckedCategoryDef> duckedCategoryDef = new List<SoundDuckedCategoryDef>();
      duckedCategoryDef.Add(new SoundDuckedCategoryDef()
      {
        m_Category = Global.SoundCategory.FX
      });
      this.m_fxDucker.m_DuckAllCategories = false;
      this.m_fxDucker.SetDuckedCategoryDefs(duckedCategoryDef);
    }
    this.m_fxDucker.StartDucking();
  }

  private void StopDuckingFx()
  {
    if (!((UnityEngine.Object) this.m_fxDucker != (UnityEngine.Object) null))
      return;
    this.m_fxDucker.StopDucking();
  }

  private void OnTurnEnded(int oldTurn, int newTurn, object userData)
  {
    if (GameState.Get().IsFriendlySidePlayerTurn())
      return;
    HearthstonePerformance.Get()?.GetCurrentPerformanceFlow<FlowPerformanceBattlegrounds>()?.OnNewRoundStart();
    GameEntity.Coroutines.StartCoroutine(GameState.Get().RejectUnresolvedChangesAfterDelay());
  }

  public override string GetAttackSpellControllerOverride(Entity attacker)
  {
    if (attacker == null)
      return (string) null;
    return attacker.IsHero() ? "AttackSpellController_Battlegrounds_Hero.prefab:922da2c91f4cca1458b5901204d1d26c" : "AttackSpellController_Battlegrounds_Minion.prefab:922da2c91f4cca1458b5901204d1d26c";
  }

  public override string GetVictoryScreenBannerText()
  {
    int leaderboardPlace = GameState.Get().GetFriendlySidePlayer().GetHero().GetRealTimePlayerLeaderboardPlace();
    return leaderboardPlace == 0 ? string.Empty : GameStrings.Get("GAMEPLAY_END_OF_GAME_PLACE_" + (object) leaderboardPlace);
  }

  public override string GetBestNameForPlayer(int playerId)
  {
    string str1 = !GameState.Get().GetPlayerInfoMap().ContainsKey(playerId) || GameState.Get().GetPlayerInfoMap()[playerId] == null ? (string) null : GameState.Get().GetPlayerInfoMap()[playerId].GetName();
    string str2 = !GameState.Get().GetPlayerInfoMap().ContainsKey(playerId) || GameState.Get().GetPlayerInfoMap()[playerId] == null || GameState.Get().GetPlayerInfoMap()[playerId].GetHero() == null ? (string) null : GameState.Get().GetPlayerInfoMap()[playerId].GetHero().GetName();
    int num = !GameState.Get().GetPlayerMap().ContainsKey(playerId) ? 0 : (GameState.Get().GetPlayerMap()[playerId].IsFriendlySide() ? 1 : 0);
    bool flag = Options.Get().GetBool(Option.STREAMER_MODE);
    if (str2 == null)
      str2 = !((UnityEngine.Object) PlayerLeaderboardManager.Get() != (UnityEngine.Object) null) || !((UnityEngine.Object) PlayerLeaderboardManager.Get().GetTileForPlayerId(playerId) != (UnityEngine.Object) null) ? (string) null : PlayerLeaderboardManager.Get().GetTileForPlayerId(playerId).GetHeroName();
    return num != 0 ? (flag || str1 == null ? GameStrings.Get("GAMEPLAY_HIDDEN_PLAYER_NAME") : str1) : (flag ? str2 ?? GameStrings.Get("GAMEPLAY_MISSING_OPPONENT_NAME") : str1 ?? GameStrings.Get("GAMEPLAY_MISSING_OPPONENT_NAME"));
  }

  public override string GetNameBannerOverride(Player.Side side)
  {
    if (side != Player.Side.OPPOSING)
      return (string) null;
    if (GameState.Get() == null)
      return (string) null;
    if (!this.IsCustomGameModeAIHero())
      return this.GetBestNameForPlayer(GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.NEXT_OPPONENT_PLAYER_ID));
    if (this.m_gamePhase == 2)
    {
      if (!((UnityEngine.Object) PlayerLeaderboardManager.Get() == (UnityEngine.Object) null) && PlayerLeaderboardManager.Get().GetOddManOutOpponentHero() != null)
        return PlayerLeaderboardManager.Get().GetOddManOutOpponentHero().GetName();
      return GameState.Get().GetOpposingSidePlayer() == null || GameState.Get().GetOpposingSidePlayer().GetHero() == null ? (string) null : GameState.Get().GetOpposingSidePlayer().GetHero().GetName();
    }
    return GameState.Get().GetOpposingSidePlayer() == null || GameState.Get().GetOpposingSidePlayer().GetHero() == null ? (string) null : GameState.Get().GetOpposingSidePlayer().GetHero().GetName();
  }

  public override void PlayAlternateEnemyEmote(
    int playerId,
    EmoteType emoteType,
    int battlegroundsEmoteId = 0)
  {
    string localizedString = "";
    NotificationManager.VisualEmoteType visualEmoteType = NotificationManager.VisualEmoteType.NONE;
    PlayerLeaderboardCard tileForPlayerId = PlayerLeaderboardManager.Get().GetTileForPlayerId(playerId);
    if ((UnityEngine.Object) tileForPlayerId == (UnityEngine.Object) null)
      return;
    Actor tileActor = tileForPlayerId.m_tileActor;
    switch (emoteType)
    {
      case EmoteType.GREETINGS:
        localizedString = GameStrings.Get("GAMEPLAY_BACON_TEXT_EMOTE_GREETINGS");
        break;
      case EmoteType.WELL_PLAYED:
        localizedString = GameStrings.Get("GAMEPLAY_BACON_TEXT_EMOTE_WELL_PLAYED");
        break;
      case EmoteType.OOPS:
        localizedString = GameStrings.Get("GAMEPLAY_BACON_TEXT_EMOTE_OOPS");
        break;
      case EmoteType.THREATEN:
        localizedString = GameStrings.Get("GAMEPLAY_BACON_TEXT_EMOTE_THREATEN");
        break;
      case EmoteType.THANKS:
        localizedString = GameStrings.Get("GAMEPLAY_BACON_TEXT_EMOTE_THANKS");
        break;
      case EmoteType.SORRY:
        localizedString = GameStrings.Get("GAMEPLAY_BACON_TEXT_EMOTE_SORRY");
        break;
      case EmoteType.WOW:
        localizedString = GameStrings.Get("GAMEPLAY_BACON_TEXT_EMOTE_WOW");
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_ONE:
        visualEmoteType = NotificationManager.VisualEmoteType.BATTLEGROUNDS_01;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_TWO:
        visualEmoteType = NotificationManager.VisualEmoteType.BATTLEGROUNDS_02;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_THREE:
        visualEmoteType = NotificationManager.VisualEmoteType.BATTLEGROUNDS_03;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_FOUR:
        visualEmoteType = NotificationManager.VisualEmoteType.BATTLEGROUNDS_04;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_FIVE:
        visualEmoteType = NotificationManager.VisualEmoteType.BATTLEGROUNDS_05;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_SIX:
        visualEmoteType = NotificationManager.VisualEmoteType.BATTLEGROUNDS_06;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_HOT_STREAK:
        visualEmoteType = NotificationManager.VisualEmoteType.HOT_STREAK;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_TRIPLE:
        visualEmoteType = NotificationManager.VisualEmoteType.TRIPLE;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_01:
        visualEmoteType = NotificationManager.VisualEmoteType.TECH_UP_01;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_02:
        visualEmoteType = NotificationManager.VisualEmoteType.TECH_UP_02;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_03:
        visualEmoteType = NotificationManager.VisualEmoteType.TECH_UP_03;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_04:
        visualEmoteType = NotificationManager.VisualEmoteType.TECH_UP_04;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_05:
        visualEmoteType = NotificationManager.VisualEmoteType.TECH_UP_05;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_TECH_UP_06:
        visualEmoteType = NotificationManager.VisualEmoteType.TECH_UP_06;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_BANANA:
        visualEmoteType = NotificationManager.VisualEmoteType.BANANA;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_HERO_BUDDY:
        visualEmoteType = NotificationManager.VisualEmoteType.HERO_BUDDY;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_DOUBLE_HERO_BUDDY:
        visualEmoteType = NotificationManager.VisualEmoteType.DOUBLE_HERO_BUDDY;
        break;
      case EmoteType.COLLECTIBLE_BATTLEGROUNDS_EMOTE:
        visualEmoteType = NotificationManager.VisualEmoteType.COLLECTIBLE_BATTLEGROUNDS_EMOTE;
        break;
      case EmoteType.BATTLEGROUNDS_VISUAL_QUEST_COMPLETE:
        visualEmoteType = NotificationManager.VisualEmoteType.QUEST_COMPLETE;
        break;
      default:
        localizedString = GameStrings.Get("GAMEPLAY_BACON_TEXT_EMOTE_INVALID");
        break;
    }
    if (visualEmoteType == NotificationManager.VisualEmoteType.COLLECTIBLE_BATTLEGROUNDS_EMOTE)
    {
      this.RequestNotification(this.CreateBattlegroundsEmoteOptions(tileActor, playerId, battlegroundsEmoteId), emoteType);
    }
    else
    {
      if (localizedString == null && visualEmoteType == NotificationManager.VisualEmoteType.NONE)
        return;
      this.RequestNotification(this.CreateStandardEmoteOptions(tileActor, localizedString, playerId, visualEmoteType), emoteType);
    }
  }

  private NotificationManager.SpeechBubbleOptions CreateBattlegroundsEmoteOptions(
    Actor actor,
    int playerId,
    int battlegroundsEmoteId)
  {
    return new NotificationManager.SpeechBubbleOptions().WithActor(actor).WithSpeechBubbleDirection(Notification.SpeechBubbleDirection.TopLeft).WithParentToActor(true).WithSpeechBubbleGroup(playerId).WithVisualEmoteType(NotificationManager.VisualEmoteType.COLLECTIBLE_BATTLEGROUNDS_EMOTE).WithFinishCallback(new Action<int>(this.OnNotificationEnded)).WithBattlegroundsEmoteId(battlegroundsEmoteId);
  }

  private NotificationManager.SpeechBubbleOptions CreateStandardEmoteOptions(
    Actor actor,
    string localizedString,
    int playerId,
    NotificationManager.VisualEmoteType visualEmoteType)
  {
    return new NotificationManager.SpeechBubbleOptions().WithActor(actor).WithBubbleScale(0.3f).WithSpeechText(localizedString).WithSpeechBubbleDirection(Notification.SpeechBubbleDirection.MiddleLeft).WithParentToActor(false).WithDestroyWhenNewCreated(true).WithSpeechBubbleGroup(playerId).WithVisualEmoteType(visualEmoteType).WithEmoteDuration(1.5f).WithFinishCallback(new Action<int>(this.OnNotificationEnded));
  }

  private void RequestNotification(
    NotificationManager.SpeechBubbleOptions options,
    EmoteType emoteType)
  {
    int speechBubbleGroup = options.speechBubbleGroup;
    if (!this.m_emotesAllowedForPlayer.ContainsKey(speechBubbleGroup))
    {
      this.m_emotesAllowedForPlayer.Add(speechBubbleGroup, true);
      this.m_emotesQueuedForPlayer.Add(speechBubbleGroup, new QueueList<NotificationManager.SpeechBubbleOptions>());
      this.m_gameNotificationsQueuedForPlayer.Add(speechBubbleGroup, new LinkedList<NotificationManager.SpeechBubbleOptions>());
    }
    if (((IEnumerable<EmoteType>) this.m_gameNotificationEmotes).Contains<EmoteType>(emoteType))
    {
      if (((IEnumerable<EmoteType>) this.m_priorityEmotes).Contains<EmoteType>(emoteType))
        this.m_gameNotificationsQueuedForPlayer[speechBubbleGroup].AddFirst(options);
      else
        this.m_gameNotificationsQueuedForPlayer[speechBubbleGroup].AddLast(options);
    }
    else
      this.m_emotesQueuedForPlayer[speechBubbleGroup].Enqueue(options);
    this.PlayEmotesIfPossibleForPlayer(speechBubbleGroup);
  }

  private void OnNotificationEnded(int playerId)
  {
    if (!this.m_emotesAllowedForPlayer.ContainsKey(playerId))
      return;
    this.m_emotesAllowedForPlayer[playerId] = true;
    this.PlayEmotesIfPossibleForPlayer(playerId);
  }

  private void PlayEmotesIfPossibleForPlayer(int playerId)
  {
    if (!this.m_emotesAllowedForPlayer.ContainsKey(playerId) || !this.m_emotesAllowedForPlayer[playerId])
      return;
    if (this.m_emotesQueuedForPlayer.ContainsKey(playerId) && this.m_emotesQueuedForPlayer[playerId].Count > 0)
    {
      NotificationManager.Get().CreateSpeechBubble(this.m_emotesQueuedForPlayer[playerId].Dequeue());
      this.m_emotesAllowedForPlayer[playerId] = false;
    }
    else
    {
      if (!this.m_gameNotificationEmotesAllowed || !this.m_gameNotificationsQueuedForPlayer.ContainsKey(playerId) || this.m_gameNotificationsQueuedForPlayer[playerId].Count <= 0)
        return;
      NotificationManager.Get().CreateSpeechBubble(this.m_gameNotificationsQueuedForPlayer[playerId].First.Value);
      this.m_gameNotificationsQueuedForPlayer[playerId].RemoveFirst();
      this.m_emotesAllowedForPlayer[playerId] = false;
    }
  }

  private void SetGameNotificationEmotesEnabled(bool enabled)
  {
    this.m_gameNotificationEmotesAllowed = enabled;
    if (!this.m_gameNotificationEmotesAllowed)
      return;
    foreach (int playerId in this.m_emotesAllowedForPlayer.Keys.ToList<int>())
      this.PlayEmotesIfPossibleForPlayer(playerId);
  }

  public override bool ShouldUseAlternateNameForPlayer(Player.Side side) => side == Player.Side.OPPOSING;

  private bool IsCustomGameModeAIHero() => this.IsShopPhase() || GameState.Get().GetFriendlySidePlayer().HasTag(GAME_TAG.BACON_ODD_PLAYER_OUT);

  public override string GetTurnTimerCountdownText(float timeRemainingInTurn)
  {
    if (this.m_gamePhase == 2)
      return GameStrings.Get("GAMEPLAY_BACON_COMBAT_END_TURN_BUTTON_TEXT");
    if (this.m_gamePhase != 1)
      return "";
    if ((double) timeRemainingInTurn == 0.0)
      return !TurnTimer.Get().IsRopeActive() ? GameStrings.Get("GAMEPLAY_BACON_SHOP_END_TURN_BUTTON_TEXT") : "";
    AchievementManager achievementManager = AchievementManager.Get();
    if ((double) timeRemainingInTurn < (double) achievementManager.GetNotificationPauseBufferSeconds() && !achievementManager.ToastNotificationsPaused)
      achievementManager.PauseToastNotifications();
    return GameStrings.Format("GAMEPLAY_END_TURN_BUTTON_COUNTDOWN", (object) Mathf.CeilToInt(timeRemainingInTurn));
  }

  public override void NotifyOfGameOver(TAG_PLAYSTATE gameResult)
  {
    base.NotifyOfGameOver(gameResult);
    this.HideShopTutorials();
  }

  protected void InitializePhasePopup() => AssetLoader.Get().InstantiatePrefab(this.BACON_PHASE_POPUP, (PrefabCallback<GameObject>) ((assetRef, go, callbackData) =>
  {
    this.m_phasePopup = go;
    this.m_phasePopup.SetActive(false);
  }));

  protected IEnumerator ShowPopup(string playmakerState, bool expectStateChangeCallback)
  {
    if (this.m_gameplaySceneLoaded)
    {
      while ((UnityEngine.Object) this.m_phasePopup == (UnityEngine.Object) null)
        yield return (object) null;
      this.m_phasePopup.SetActive(true);
      PlayMakerFSM phaseFsm = this.m_phasePopup.GetComponent<PlayMakerFSM>();
      phaseFsm.SetState(playmakerState);
      if (!expectStateChangeCallback || (UnityEngine.Object) BaconBoard.Get() == (UnityEngine.Object) null)
      {
        while (phaseFsm.ActiveStateName != "Idle")
          yield return (object) null;
        yield return (object) null;
        phaseFsm.SetState("Death");
      }
    }
  }

  protected void OnStateChange(TAG_BOARD_VISUAL_STATE newState) => GameEntity.Coroutines.StartCoroutine(this.StateChangeCoroutine(newState));

  protected IEnumerator StateChangeCoroutine(TAG_BOARD_VISUAL_STATE newState)
  {
    while ((UnityEngine.Object) this.m_phasePopup == (UnityEngine.Object) null)
      yield return (object) null;
    while (!this.m_phasePopup.activeSelf)
      yield return (object) null;
    PlayMakerFSM phaseFsm = this.m_phasePopup.GetComponent<PlayMakerFSM>();
    while (phaseFsm.ActiveStateName != "Idle")
      yield return (object) null;
    yield return (object) null;
    phaseFsm.SetState("Death");
    if (newState == TAG_BOARD_VISUAL_STATE.COMBAT)
      yield return (object) this.TryPlayHeroGreet();
  }

  protected IEnumerator TryPlayHeroGreet()
  {
    Actor heroActor = (Actor) null;
    string voLine;
    if (this.CheckHeroGreet(out heroActor, out voLine))
      yield return (object) this.PlayVOLineWithOffsetBubble(voLine, heroActor, 1.5f);
  }

  protected void UpdateNameBanner()
  {
    if ((UnityEngine.Object) Gameplay.Get() == (UnityEngine.Object) null)
      return;
    NameBanner nameBannerForSide = Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING);
    if ((UnityEngine.Object) nameBannerForSide == (UnityEngine.Object) null)
      return;
    nameBannerForSide.UpdatePlayerNameBanner();
  }

  protected void InitializeTurnTimer() => TurnTimer.Get().SetGameModeSettings(new TurnTimerGameModeSettings()
  {
    m_RopeFuseVolume = 0.05f,
    m_EndTurnButtonExplosionVolume = 0.0f,
    m_RopeRolloutVolume = 0.3f,
    m_PlayMusicStinger = false,
    m_PlayTimeoutFx = false,
    m_PlayTickSound = true
  });

  public bool IsShopPhase() => this.m_gamePhase == 1;

  private void OnBattlegroundsRatingChange() => this.RatingChangeData = Network.Get().GetBattlegroundsRatingChange();

  public override void NotifyOfMinionDied(Entity minion)
  {
    base.NotifyOfMinionDied(minion);
    BaconBoard.Get().NotifyOfMinionDied(minion);
  }

  private int GetTechLevelInt() => GameState.Get() == null || GameState.Get().GetFriendlySidePlayer() == null ? 0 : GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.PLAYER_TECH_LEVEL);

  private void InitTurnCounter()
  {
    this.m_techLevelCounter = AssetLoader.Get().InstantiatePrefab((AssetReference) "BaconTechLevelRibbon.prefab:ad60cd0fe1c8eea4bb2f12cc280acda8").GetComponent<Notification>();
    PlayMakerFSM component = this.m_techLevelCounter.GetComponent<PlayMakerFSM>();
    component.FsmVariables.GetFsmInt("TechLevel").Value = this.GetTechLevelInt();
    component.SendEvent("Birth");
    this.m_techLevelCounter.transform.localPosition = ZoneMgr.Get().FindZoneOfType<ZoneHero>(Player.Side.OPPOSING).transform.position + new Vector3(-1.294f, 0.21f, -0.152f);
    this.m_techLevelCounter.transform.localScale = Vector3.one * 0.58f;
    GameEntity.Coroutines.StartCoroutine(this.KeepTechLevelUpToDateCoroutine());
  }

  protected void ShowTechLevelDisplay(bool shown)
  {
    if ((UnityEngine.Object) this.m_techLevelCounter == (UnityEngine.Object) null)
      this.InitTurnCounter();
    if (!((UnityEngine.Object) this.m_techLevelCounter != (UnityEngine.Object) null))
      return;
    this.m_techLevelCounter.gameObject.SetActive(shown);
  }

  private IEnumerator KeepTechLevelUpToDateCoroutine()
  {
    while (true)
    {
      if (!this.m_techLevelCounter.gameObject.activeInHierarchy)
        yield return (object) null;
      int techLevelInt = this.GetTechLevelInt();
      if (techLevelInt != this.m_displayedTechLevelNumber)
      {
        PlayMakerFSM component = this.m_techLevelCounter.GetComponent<PlayMakerFSM>();
        component.FsmVariables.GetFsmInt("TechLevel").Value = techLevelInt;
        component.SendEvent("Action");
        this.UpdateTechLevelDisplayText(techLevelInt);
      }
      yield return (object) null;
    }
  }

  public override void ToggleAlternateMulliganActorHighlight(Card card, bool highlighted)
  {
    PlayerLeaderboardMainCardActor actor = card.GetActor() as PlayerLeaderboardMainCardActor;
    if (!((UnityEngine.Object) actor != (UnityEngine.Object) null))
      return;
    actor.SetFullyHighlighted(highlighted);
  }

  public override bool ToggleAlternateMulliganActorHighlight(Actor actor, bool? highlighted = null)
  {
    PlayerLeaderboardMainCardActor leaderboardMainCardActor = actor as PlayerLeaderboardMainCardActor;
    if (!((UnityEngine.Object) leaderboardMainCardActor != (UnityEngine.Object) null))
      return false;
    bool highlighted1 = !highlighted.HasValue ? !leaderboardMainCardActor.m_fullSelectionHighlight.activeSelf : highlighted.Value;
    leaderboardMainCardActor.SetFullyHighlighted(highlighted1);
    return highlighted1;
  }

  private void UpdateTechLevelDisplayText(int techLevel)
  {
    this.m_techLevelCounter.ChangeDialogText(GameStrings.Get("GAMEPLAY_BACON_TAVERN_TIER"), "", "", "");
    this.m_displayedTechLevelNumber = techLevel;
  }

  protected void ShowShopTutorials() => this.HideShopTutorials();

  protected virtual void HideShopTutorials()
  {
    TB_BaconShop.StopCoroutine(this.m_buyButtonTutorialCoroutine);
    TB_BaconShop.StopCoroutine(this.m_enemyMinionTutorialCoroutine);
    TB_BaconShop.StopCoroutine(this.m_playMinionTutorialCoroutine);
    this.HideBuyButtonTutorial();
    this.HidePlayMinionTutorial();
    this.HideShopMinionTutorial();
  }

  private static void StopCoroutine(Coroutine coroutine)
  {
    if (coroutine == null)
      return;
    GameEntity.Coroutines.StopCoroutine(coroutine);
  }

  protected void HideBuyButtonTutorial(bool hideImmediately = false)
  {
    if (!((UnityEngine.Object) this.m_buyButtonTutorialNotification != (UnityEngine.Object) null))
      return;
    if (hideImmediately)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_buyButtonTutorialNotification);
    else
      NotificationManager.Get().DestroyNotification(this.m_buyButtonTutorialNotification, 0.0f);
  }

  protected void HideShopMinionTutorial()
  {
    if (!((UnityEngine.Object) this.m_enemyMinionTutorialNotification != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotification(this.m_enemyMinionTutorialNotification, 0.0f);
  }

  protected void HidePlayMinionTutorial()
  {
    if (!((UnityEngine.Object) this.m_playMinionTutorialNotification != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotification(this.m_playMinionTutorialNotification, 0.0f);
  }

  private IEnumerator WaitAndHideActiveSpeechBubble()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    TB_BaconShop tbBaconShop = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      NotificationManager.Get().DestroyNotification(tbBaconShop.m_ActiveSpeechBubble, 0.0f);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(1f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    HashSet<string> stringSet = new HashSet<string>();
    stringSet.UnionWith((IEnumerable<string>) this.SoundFilesForPreload());
    foreach (string soundPath in stringSet)
      this.PreloadSound(soundPath);
  }

  protected virtual List<string> SoundFilesForPreload()
  {
    List<string> stringList = new List<string>((IEnumerable<string>) new List<string>()
    {
      (string) TB_BaconShop.VO_DALA_BOSS_99h_Male_Human_ShopFirstTime_01,
      (string) TB_BaconShop.VO_DALA_BOSS_99h_Male_Human_FirstDefeat_01,
      (string) TB_BaconShop.VO_DALA_BOSS_99h_Male_Human_FirstVictory_01,
      (string) TB_BaconShop.VO_DALA_BOSS_99h_Male_Human_Hire_01,
      (string) TB_BaconShop.VO_DALA_BOSS_99h_Male_Human_RecruitWork_01
    });
    stringList.AddRange((IEnumerable<string>) this.GetGuideConfig().GetAllVOLines());
    return stringList;
  }

  public override void OnPlayThinkEmote()
  {
    if (this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (currentPlayer == null || !currentPlayer.IsFriendlySide())
      return;
    Card heroCard = currentPlayer.GetHeroCard();
    if ((UnityEngine.Object) heroCard == (UnityEngine.Object) null || heroCard.HasActiveEmoteSound())
      return;
    Actor bobActor = this.GetBobActor();
    if ((UnityEngine.Object) bobActor == (UnityEngine.Object) null || bobActor.GetEntity() == null)
      return;
    if (currentPlayer.GetNumAvailableResources() <= 2)
    {
      if (!this.ShouldPlayRateVO(0.1f))
        return;
      GameEntity.Coroutines.StartCoroutine(this.PlayBobLineWithOffsetBubble(this.GetGuideConfig().PopRandomSpecialIdleLine()));
    }
    else
    {
      if (!this.ShouldPlayRateVO(0.05f))
        return;
      GameEntity.Coroutines.StartCoroutine(this.PlayBobLineWithOffsetBubble(this.GetGuideConfig().GetRandomIdleLine()));
    }
  }

  protected Actor GetBobActor()
  {
    Entity hero = GameState.Get().GetOpposingSidePlayer().GetHero();
    return hero != null && hero.GetCardId() == this.m_FavoriteGuideCardId ? hero.GetHeroCard().GetActor() : (Actor) null;
  }

  protected Actor GetFriendlyHeroActor() => GameState.Get().GetFriendlySidePlayer()?.GetHero().GetHeroCard().GetActor();

  protected Actor GetOpposingHeroActor()
  {
    Entity hero = GameState.Get().GetOpposingSidePlayer().GetHero();
    if (hero != null && hero.GetCardId() != this.m_FavoriteGuideCardId)
      return hero.GetHeroCard().GetActor();
    int tag = GameState.Get().GetFriendlySidePlayer().GetTag(GAME_TAG.NEXT_OPPONENT_PLAYER_ID);
    Map<int, SharedPlayerInfo> playerInfoMap = GameState.Get().GetPlayerInfoMap();
    SharedPlayerInfo sharedPlayerInfo = (SharedPlayerInfo) null;
    if (playerInfoMap != null && playerInfoMap.ContainsKey(tag))
      sharedPlayerInfo = playerInfoMap[tag];
    return sharedPlayerInfo != null && sharedPlayerInfo.GetPlayerHero() != null && (UnityEngine.Object) sharedPlayerInfo.GetPlayerHero().GetCard() != (UnityEngine.Object) null ? sharedPlayerInfo.GetPlayerHero().GetCard().GetActor() : (Actor) null;
  }

  protected bool HasSeenAllTutorial() => this.m_hasSeenBuyButtonTutorial && this.m_hasSeenEnemyMinionTutorial && this.m_hasSeenPlayMinionTutorial;

  protected bool ShouldPlayRateVO(float chance)
  {
    float num = UnityEngine.Random.Range(0.0f, 1f);
    return (double) chance > (double) num;
  }

  protected IEnumerator PlayVOLineWithOffsetBubble(
    string voLine,
    Actor actor,
    float wait = 0.0f)
  {
    if ((UnityEngine.Object) actor != (UnityEngine.Object) null && actor.GetEntity() != null)
    {
      if ((double) wait != 0.0)
        yield return (object) new WaitForSeconds(wait);
      yield return (object) this.PlayVOLineWithoutText(voLine, actor);
    }
  }

  protected IEnumerator PlayVOLineWithoutText(string voLine, Actor actor)
  {
    TB_BaconShop tbBaconShop = this;
    if ((UnityEngine.Object) actor != (UnityEngine.Object) null && actor.GetEntity() != null)
    {
      Notification.SpeechBubbleDirection direction = !actor.GetEntity().IsControlledByFriendlySidePlayer() ? Notification.SpeechBubbleDirection.BottomLeft : Notification.SpeechBubbleDirection.TopLeft;
      tbBaconShop.m_enemySpeaking = true;
      yield return (object) tbBaconShop.PlaySoundAndWait(voLine, "", direction, actor, Time.timeScale);
      tbBaconShop.m_enemySpeaking = false;
    }
  }

  protected IEnumerator PlayBobLineWithoutText(string voLine)
  {
    TB_BaconShop tbBaconShop = this;
    Actor bobActor = tbBaconShop.GetBobActor();
    if ((UnityEngine.Object) bobActor != (UnityEngine.Object) null && bobActor.GetEntity() != null)
    {
      tbBaconShop.m_enemySpeaking = true;
      yield return (object) tbBaconShop.PlaySoundAndWait(voLine, "", Notification.SpeechBubbleDirection.TopLeft, bobActor, Time.timeScale);
      tbBaconShop.m_enemySpeaking = false;
    }
  }

  protected virtual IEnumerator PlayBobLineWithOffsetBubble(string voLine)
  {
    Actor bobActor = this.GetBobActor();
    if ((UnityEngine.Object) bobActor != (UnityEngine.Object) null && bobActor.GetEntity() != null)
      yield return (object) this.PlayBobLineWithoutText(voLine);
  }

  private IEnumerator PlayWisdomballVOLine(string voLine)
  {
    TB_BaconShop tbBaconShop = this;
    Actor wisdomballActor = (Actor) null;
    foreach (Card questRewardCard in TB_BaconShop.GetQuestRewardCards(Player.Side.FRIENDLY))
    {
      Actor actor = questRewardCard.GetActor();
      if (actor.CardDefName == "BG24_Reward_313" || actor.CardDefName == "BG24_Reward_313(Clone)")
        wisdomballActor = actor;
    }
    tbBaconShop.m_enemySpeaking = true;
    tbBaconShop.RemovePreloadedSound(voLine);
    tbBaconShop.PreloadSound(voLine);
    while (tbBaconShop.IsPreloadingAssets())
      yield return (object) null;
    yield return (object) tbBaconShop.PlayVOLineWithOffsetBubble(voLine, wisdomballActor);
    tbBaconShop.m_enemySpeaking = false;
  }

  protected Card GetGameModeButtonBySlot(int buttonSlot)
  {
    List<Zone> zonesForSide = ZoneMgr.Get().FindZonesForSide(Player.Side.FRIENDLY);
    Zone zone1 = (Zone) null;
    foreach (Zone zone2 in zonesForSide)
    {
      if (zone2 is ZoneGameModeButton && ((ZoneGameModeButton) zone2).m_ButtonSlot == buttonSlot)
        zone1 = zone2;
    }
    return (UnityEngine.Object) zone1 == (UnityEngine.Object) null ? (Card) null : zone1.GetFirstCard();
  }

  public static Card GetHeroBuddyCard(Player.Side playerSide)
  {
    List<Zone> zonesForSide = ZoneMgr.Get().FindZonesForSide(playerSide);
    Zone zone1 = (Zone) null;
    foreach (Zone zone2 in zonesForSide)
    {
      if (zone2 is ZoneBattlegroundHeroBuddy)
        zone1 = zone2;
    }
    return (UnityEngine.Object) zone1 == (UnityEngine.Object) null ? (Card) null : zone1.GetFirstCard();
  }

  public static List<Card> GetQuestRewardCards(Player.Side playerSide)
  {
    GameState gameState = GameState.Get();
    if (gameState != null)
    {
      Player player = playerSide == Player.Side.OPPOSING ? gameState.GetOpposingPlayer() : gameState.GetFriendlySidePlayer();
      if (player != null)
        return player.GetQuestRewardCards();
    }
    return new List<Card>();
  }

  protected Card GetFreezeButtonCard() => this.GetGameModeButtonBySlot(1);

  protected Card GetRefreshButtonCard() => this.GetGameModeButtonBySlot(2);

  protected Card GetTavernUpgradeButtonCard() => this.GetGameModeButtonBySlot(3);

  protected void SetInputEnableForBuy(bool isEnabled)
  {
    foreach (Card card in GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards())
      card.SetInputEnabled(isEnabled);
  }

  protected void SetInputEnableForRefreshButton(bool isEnabled)
  {
    Card refreshButtonCard = this.GetRefreshButtonCard();
    if (!((UnityEngine.Object) refreshButtonCard != (UnityEngine.Object) null))
      return;
    refreshButtonCard.SetInputEnabled(isEnabled);
  }

  protected void SetInputEnableForTavernUpgradeButton(bool isEnabled)
  {
    Card upgradeButtonCard = this.GetTavernUpgradeButtonCard();
    if (!((UnityEngine.Object) upgradeButtonCard != (UnityEngine.Object) null))
      return;
    upgradeButtonCard.SetInputEnabled(isEnabled);
  }

  protected void SetInputEnableForFrozenButton(bool isEnabled)
  {
    Card freezeButtonCard = this.GetFreezeButtonCard();
    if (!((UnityEngine.Object) freezeButtonCard != (UnityEngine.Object) null))
      return;
    freezeButtonCard.SetInputEnabled(isEnabled);
  }

  public override bool NotifyOfPlayError(
    PlayErrors.ErrorType error,
    int? errorParam,
    Entity errorSource)
  {
    return error == PlayErrors.ErrorType.REQ_ATTACK_GREATER_THAN_0;
  }

  private void ForceShowFriendlyHeroActor()
  {
    Card heroCard = GameState.Get().GetFriendlySidePlayer().GetHeroCard();
    if (!(bool) (UnityEngine.Object) heroCard)
      return;
    heroCard.ShowCard();
    if (!((UnityEngine.Object) heroCard.GetActor() != (UnityEngine.Object) null))
      return;
    heroCard.GetActor().Show();
  }
}
