using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DataModels;
using Hearthstone.DungeonCrawl;
using Hearthstone.Progression;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class AdventureDungeonCrawlPlayMat : MonoBehaviour
{
  [CustomEditField(Sections = "UI")]
  public UberText m_headerText;
  [CustomEditField(Sections = "UI")]
  public AsyncReference m_PlayButtonReference;
  [CustomEditField(Sections = "UI")]
  public GameObject m_PlayButtonRoot;
  [CustomEditField(Sections = "UI")]
  public GameObject m_PlayButtonPlate;
  [CustomEditField(Sections = "UI")]
  public List<NestedPrefabPlatformOverride> m_rewardOptionNestedPrefabs = new List<NestedPrefabPlatformOverride>();
  [CustomEditField(Sections = "UI")]
  public List<AdventureDungeonCrawlHeroPowerOption> m_heroPowerOptions;
  [CustomEditField(Sections = "UI")]
  public List<AdventureDungeonCrawlDeckOption> m_deckOptions;
  [CustomEditField(Sections = "UI")]
  public Widget m_treasureSatchelWidget;
  [CustomEditField(Sections = "UI")]
  public GameObject m_optionsPane;
  [CustomEditField(Sections = "UI")]
  public GameObject m_nextBossPane;
  [CustomEditField(Sections = "UI")]
  public NestedPrefabBase m_bossGraveyardPane;
  [CustomEditField(Sections = "UI")]
  public GameObject m_allCards;
  [CustomEditField(Sections = "UI")]
  public GameObject m_facedownCards;
  [CustomEditField(Sections = "UI")]
  public GameObject m_bossHeroPowerTooltipBone;
  [CustomEditField(Sections = "UI")]
  public float m_bossHeroPowerTooltipPulseRate;
  [CustomEditField(Sections = "UI")]
  public float m_bossHeroPowerTooltipDelayAfterVo;
  [CustomEditField(Sections = "UI")]
  public PlayNewParticles m_nextBossPlayNewParticlesScript;
  [CustomEditField(Sections = "UI")]
  public PlayNewParticles m_facedownBossesPlayNewParticlesScript;
  [CustomEditField(Sections = "UI")]
  public AsyncReference m_treasureSatchelReference;
  [CustomEditField(Sections = "UI")]
  public AsyncReference m_treasureInspectReference;
  [CustomEditField(Sections = "UI")]
  public AsyncReference m_platformControllerReference;
  [CustomEditField(Sections = "UI")]
  public AsyncReference m_paperControllerReference;
  [CustomEditField(Sections = "UI")]
  public AsyncReference m_paperControllerReference_phone;
  [CustomEditField(Sections = "UI")]
  public GameObject m_selectedOptionClickBlocker;
  [CustomEditField(Sections = "Animations")]
  public Animation m_nextBossFlipAnimation;
  [CustomEditField(Sections = "Animations")]
  public string m_nextBossFlipSmallName;
  [CustomEditField(Sections = "Animations")]
  public string m_nextBossFlipLargeName;
  [CustomEditField(Sections = "Animations")]
  public Animation m_bossDeckDropAnimation;
  [CustomEditField(Sections = "Animations")]
  public float m_delayAfterDeckDrop = 1f;
  [CustomEditField(Sections = "Animations")]
  public float m_lootDropDelay = 0.05f;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_nextBossFlipSmallSFXDefault;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_nextBossFlipLargeSFXDefault;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_nextBossFlipCrowdReactionSmallSFXDefault;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_nextBossFlipCrowdReactionMediumSFXDefault;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_nextBossFlipCrowdReactionLargeSFXDefault;
  [CustomEditField(Sections = "SFX")]
  public float m_nextBossFlipCrowdReactionDelay = 0.5f;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_bossDeckDropSFXDefault;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_bossDeckMagicallyAppearSFXDefault;
  [CustomEditField(Sections = "SFX", T = EditType.SOUND_PREFAB)]
  public string m_bossMouseOverSFXDefault;
  [CustomEditField(Sections = "Styles")]
  public List<AdventureDungeonCrawlPlayMat.PlaymatStyleOverride> m_playmatStyleOverride;
  [CustomEditField(Sections = "Bones")]
  public Transform m_nextBossBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_nextBossFaceBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_nextBossBackBone;
  [CustomEditField(Sections = "Bones")]
  public List<Transform> m_bossCardBones = new List<Transform>();
  [CustomEditField(Sections = "Bones")]
  public GameObject m_BossPowerBone;
  [CustomEditField(Sections = "Bones")]
  public List<Transform> m_cardBackBones = new List<Transform>();
  [CustomEditField(Sections = "Bones")]
  public SlidingTray m_MobilePlayButtonSlidingTrayBone;
  [CustomEditField(Sections = "PVPDR")]
  public AsyncReference m_duelsPlayMatReference;
  private AdventureDungeonCrawlPlayMat.PlayMatState m_playMatState;
  private AdventureDungeonCrawlPlayMat.PlayMatState m_lastVisualPlayMatState;
  private bool m_startCallFinished;
  private Actor m_bossActor;
  private DefLoader.DisposableCardDef m_bossCardDef;
  private EntityDef m_bossEntityDef;
  private List<Actor> m_defeatedBossActors = new List<Actor>();
  private GameObject m_nextBossCardBack;
  private Actor m_topDefeatedBoss;
  private List<GameObject> m_cardBacks = new List<GameObject>();
  private PlayButton m_playButton;
  private List<AdventureDungeonCrawlRewardOption> m_rewardOptions;
  private AdventureDungeonCrawlBossGraveyard m_bossGraveyard;
  private bool m_subsceneTransitionComplete;
  private CardBack m_cardBack;
  private AdventureDungeonCrawlPlayMat.OptionType m_currentOptionType;
  private int m_numBossesDefeated;
  private int m_bossesPerRun;
  private bool m_allowPlayButtonAnimation;
  private bool m_setUpDefeatedBossesCompleted;
  private int m_playerHeroDbId;
  private AdventureDungeonCrawlPlayMat.PlaymatStyleOverride m_matchingPlaymatStyle;
  private string m_nextBossFlipSmallSFXOverride;
  private string m_nextBossFlipLargeSFXOverride;
  private string m_nextBossFlipCrowdReactionSmallSFXOverride;
  private string m_nextBossFlipCrowdReactionMediumSFXOverride;
  private string m_nextBossFlipCrowdReactionLargeSFXOverride;
  private string m_bossDeckDropSFXOverride;
  private string m_bossDeckMagicallyAppearSFXOverride;
  private string m_chooseTreasureHeaderStringOverride;
  private string m_chooseLootHeaderStringOverride;
  private List<AdventureDungeonCrawlTreasureOption> m_treasureSatchelOptions;
  public Widget m_treasureInspectWidget;
  private bool m_loadingCardback;
  private Notification m_bossHeroPowerTooltip;
  private bool m_shouldShowBossHeroPowerTooltip;
  private VisualController m_paperController;
  private IDungeonCrawlData m_dungeonCrawlData;
  private static readonly PlatformDependentValue<string> HERO_POWER_TOOLTIP_STRING = new PlatformDependentValue<string>(PlatformCategory.Screen)
  {
    PC = "GLUE_ADVENTURE_DUNGEON_CRAWL_BOSS_HERO_POWER_TOOLTIP",
    Phone = "GLUE_ADVENTURE_DUNGEON_CRAWL_BOSS_HERO_POWER_TOOLTIP_PHONE"
  };
  private const string TREASURE_SATCHEL_OPTION_SELECTED_EVENT = "CODE_TREASURE_OPTION_SELECTED";
  private const string TREASURE_SATCHEL_SHOW_EVENT = "CODE_TREASURE_SATCHEL_SHOW";
  private const string TREASURE_SATCHEL_OUTRO_COMPLETE_EVENT = "CODE_TREASURE_SATCHEL_OUTRO_COMPLETE";
  private bool m_playMatStateInitialized;
  private Widget m_duelsPlayWidget;
  private DuelsPlayMat m_duelsPlayMat;
  private bool m_duelsReadyToShowRewards;

  public bool IsNextMissionASpecialEncounter { get; set; }

  public PlayButton PlayButton => this.m_playButton;

  private void Awake()
  {
    this.m_treasureSatchelReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnTreasureSatchelReady));
    this.m_treasureInspectReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnTreasureInspectReady));
    this.m_duelsPlayMatReference.RegisterReadyListener<DuelsPlayMat>(new Action<DuelsPlayMat>(this.OnPVPDRPlayMatReady));
    if (!((UnityEngine.Object) this.m_treasureSatchelWidget != (UnityEngine.Object) null))
      return;
    this.m_treasureSatchelWidget.gameObject.SetActive(false);
  }

  private void Start()
  {
    this.m_rewardOptions = new List<AdventureDungeonCrawlRewardOption>(this.m_rewardOptionNestedPrefabs.Count);
    for (int index = 0; index < this.m_rewardOptionNestedPrefabs.Count; ++index)
    {
      NestedPrefabBase optionNestedPrefab = (NestedPrefabBase) this.m_rewardOptionNestedPrefabs[index];
      if ((UnityEngine.Object) optionNestedPrefab == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("AdventureDungeonCrawlPlayMat.Start - m_rewardOptionNestedPrefabs have null values. Skipping index {0}...", (object) index);
      }
      else
      {
        AdventureDungeonCrawlRewardOption component = optionNestedPrefab.PrefabGameObject(true).GetComponent<AdventureDungeonCrawlRewardOption>();
        switch (index)
        {
          case 0:
            TransformUtil.SetLocalPosX((Component) component.m_deckTray.m_deckBigCard, 0.27f);
            component.m_deckTray.m_deckBigCard.m_flipHeroPowerHorizontalPosition = true;
            break;
          case 1:
            component.m_deckTray.m_deckBigCard.m_flipHeroPowerHorizontalPosition = true;
            break;
        }
        component.m_deckTray.m_deckBigCard.m_disableCollidersOnHeroPower = true;
        component.m_deckTray.m_deckBigCard.m_showTooltipsForAdventure = true;
        this.m_rewardOptions.Add(component);
      }
    }
    this.m_PlayButtonReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayButtonReady));
    this.SetUpPlayButton();
    this.m_startCallFinished = true;
  }

  private void OnDestroy()
  {
    this.m_bossCardDef?.Dispose();
    this.m_bossCardDef = (DefLoader.DisposableCardDef) null;
  }

  public void Initialize(IDungeonCrawlData data)
  {
    this.m_dungeonCrawlData = data;
    AdventureConfig.Get().GetAdventureDataModel().SelectedHeroId = GameUtils.TranslateDbIdToCardId((int) data.SelectedHeroCardDbId);
    this.SetPlaymatVisualStyle();
    foreach (AdventureDungeonCrawlRewardOption rewardOption in this.m_rewardOptions)
      rewardOption.Initalize(this.m_dungeonCrawlData);
    this.m_paperControllerReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnPaperControllerReady));
    this.m_paperControllerReference_phone.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnPaperControllerReady));
  }

  private void Update()
  {
    if (this.m_playMatState != AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE)
      return;
    bool flag = true;
    if (this.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.INVALID)
      return;
    IEnumerable<AdventureOptionWidget> adventureOptionWidgets = (IEnumerable<AdventureOptionWidget>) null;
    if (this.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.HERO_POWER)
      adventureOptionWidgets = this.m_heroPowerOptions.Cast<AdventureOptionWidget>();
    else if (this.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.DECK)
      adventureOptionWidgets = this.m_deckOptions.Cast<AdventureOptionWidget>();
    else if (this.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.TREASURE_SATCHEL)
    {
      adventureOptionWidgets = this.m_treasureSatchelOptions.Cast<AdventureOptionWidget>();
      if ((UnityEngine.Object) this.m_treasureSatchelWidget == (UnityEngine.Object) null || !this.m_treasureSatchelWidget.IsReady || this.m_treasureSatchelWidget.HasPendingActions)
        flag = false;
    }
    if (adventureOptionWidgets != null)
    {
      foreach (AdventureOptionWidget adventureOptionWidget in adventureOptionWidgets)
      {
        if (adventureOptionWidget.IsOutroPlaying || !adventureOptionWidget.IsReady)
        {
          flag = false;
          break;
        }
      }
    }
    else
    {
      for (int index = 0; index < this.m_rewardOptions.Count; ++index)
      {
        if (this.m_rewardOptions[index].OutroIsPlaying())
        {
          flag = false;
          break;
        }
      }
    }
    if (!flag)
      return;
    this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.READY_FOR_DATA, true);
  }

  public bool IsReady()
  {
    if (!this.m_startCallFinished || (UnityEngine.Object) this.m_bossActor == (UnityEngine.Object) null)
      return false;
    if (!((UnityEngine.Object) this.m_playButton == (UnityEngine.Object) null))
      return true;
    Log.Adventures.PrintWarning("PlayButton not ready yet!");
    return false;
  }

  public void SetTreasureSatchelOptionSelectedCallback(
    AdventureDungeonCrawlTreasureOption.TreasureSelectedOptionCallback callback)
  {
    if (this.m_treasureSatchelReference == null)
      Debug.LogError((object) "AdventureDungeonCrawlPlayMat.SetTreasureSatchelOptionSelectedCallback - m_treasureSatchelReference was null!");
    else
      this.m_treasureSatchelReference.RegisterReadyListener<Widget>((Action<Widget>) (widget =>
      {
        if (this.m_treasureSatchelOptions == null)
          return;
        foreach (AdventureDungeonCrawlTreasureOption treasureSatchelOption in this.m_treasureSatchelOptions)
        {
          if ((UnityEngine.Object) treasureSatchelOption != (UnityEngine.Object) null)
            treasureSatchelOption.SetOptionCallbacks((Delegate) callback);
        }
      }));
  }

  public void SetDeckOptionSelectedCallback(
    AdventureDungeonCrawlDeckOption.DeckOptionSelectedCallback callback)
  {
    foreach (AdventureOptionWidget deckOption in this.m_deckOptions)
      deckOption.SetOptionCallbacks((Delegate) callback);
  }

  public void SetHeroPowerOptionCallback(
    AdventureDungeonCrawlHeroPowerOption.HeroPowerSelectedOptionCallback selectedCallback,
    AdventureDungeonCrawlHeroPowerOption.HeroPowerHoverOptionCallback rolloverCallback,
    AdventureDungeonCrawlHeroPowerOption.HeroPowerHoverOptionCallback rolloutCallback)
  {
    foreach (AdventureOptionWidget heroPowerOption in this.m_heroPowerOptions)
      heroPowerOption.SetOptionCallbacks((Delegate) selectedCallback, (Delegate) rolloverCallback, (Delegate) rolloutCallback);
  }

  public void SetRewardOptionSelectedCallback(
    AdventureDungeonCrawlPlayMat.RewardOptionSelectedCallback callback)
  {
    foreach (AdventureDungeonCrawlRewardOption rewardOption1 in this.m_rewardOptions)
    {
      AdventureDungeonCrawlRewardOption rewardOption = rewardOption1;
      rewardOption.SetOptionChosenCallback((AdventureDungeonCrawlRewardOption.OptionChosenCallback) (() => callback(rewardOption.GetOptionData())));
    }
  }

  public void DeselectAllDeckOptionsWithoutId(int deckId)
  {
    foreach (AdventureDungeonCrawlDeckOption deckOption in this.m_deckOptions)
    {
      if (deckOption.DeckId != (long) deckId)
        deckOption.Deselect();
    }
  }

  public void SetBossActor(Actor bossActor)
  {
    this.m_bossActor = bossActor;
    if (!((UnityEngine.Object) this.m_bossActor != (UnityEngine.Object) null) || this.m_bossCardDef == null || this.m_bossEntityDef == null)
      return;
    this.SetUpBossCard();
  }

  public void SetBossFullDef(DefLoader.DisposableCardDef cardDef, EntityDef entityDef)
  {
    this.m_bossCardDef?.Dispose();
    this.m_bossCardDef = cardDef;
    this.m_bossEntityDef = entityDef;
    if (!((UnityEngine.Object) this.m_bossActor != (UnityEngine.Object) null) || this.m_bossCardDef == null || this.m_bossEntityDef == null)
      return;
    this.SetUpBossCard();
  }

  private void SetUpBossCard()
  {
    if ((UnityEngine.Object) this.m_bossActor == (UnityEngine.Object) null)
      Log.Adventures.PrintError("AdventureDungeonCrawlDisplay.SetUpBossCard - m_BossActor is null!");
    else if (this.m_bossCardDef == null || this.m_bossEntityDef == null)
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlDisplay.SetUpBossCard - m_bossFullDef is null!");
    }
    else
    {
      this.m_bossActor.SetCardDef(this.m_bossCardDef);
      this.m_bossActor.SetEntityDef(this.m_bossEntityDef);
      this.m_bossActor.SetPremium(TAG_PREMIUM.NORMAL);
      PegUIElement component = this.m_bossActor.GetCollider().gameObject.GetComponent<PegUIElement>();
      if ((bool) (UnityEngine.Object) component)
        component.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit(this.m_bossMouseOverSFXDefault))));
      else
        Debug.LogError((object) "Could not find PegUIElement for Boss");
      this.m_bossActor.SetCardbackUpdateIgnore(true);
      if ((UnityEngine.Object) this.m_cardBack != (UnityEngine.Object) null)
        this.m_bossActor.m_cardMesh.GetComponent<Renderer>().GetMaterial(this.m_bossActor.m_cardBackMatIdx).mainTexture = (Texture) this.m_cardBack.m_CardBackTexture;
      this.m_bossActor.Show();
    }
  }

  public void SetCardBack(int cardBackId)
  {
    this.m_loadingCardback = true;
    if (CardBackManager.Get().LoadCardBackByIndex(cardBackId, new CardBackManager.LoadCardBackData.LoadCardBackCallback(this.OnCardBackLoaded)))
      return;
    Log.Adventures.PrintError("AdventureDungeonCrawlPlayMat.SetCardBack() - failed to load CardBack {0}", (object) cardBackId);
    this.m_loadingCardback = false;
  }

  public void SetPlayerHeroDbId(int heroDbId) => this.m_playerHeroDbId = heroDbId;

  private void OnCardBackLoaded(CardBackManager.LoadCardBackData cardbackData)
  {
    this.m_loadingCardback = false;
    this.m_cardBack = cardbackData.m_CardBack;
    if ((UnityEngine.Object) this.m_bossActor != (UnityEngine.Object) null)
      this.m_bossActor.m_cardMesh.GetComponent<Renderer>().GetMaterial(this.m_bossActor.m_cardBackMatIdx).mainTexture = (Texture) this.m_cardBack.m_CardBackTexture;
    if (this.m_cardBackBones.Count < 1)
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlPlayMat.OnCardBackLoaded() - Can't attach the cardbacks to a bone, as m_cardBackBones are not defined!");
    }
    else
    {
      this.m_nextBossCardBack = cardbackData.m_GameObject;
      Actor component = this.m_nextBossCardBack.GetComponent<Actor>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.SetCardbackUpdateIgnore(true);
      GameUtils.SetParent(this.m_nextBossCardBack, (Component) this.m_nextBossBackBone, true);
      this.m_cardBacks.Clear();
    }
  }

  public void SetUpDefeatedBosses(List<long> defeatedBossIds, int bossesPerRun)
  {
    if (this.m_setUpDefeatedBossesCompleted)
    {
      Debug.LogError((object) "Calling SetUpDefeatedBosses, when this has already been called! Please investigate - you should not be doing this!");
    }
    else
    {
      this.m_numBossesDefeated = defeatedBossIds == null ? 0 : defeatedBossIds.Count;
      this.m_bossesPerRun = bossesPerRun;
      int a = Mathf.Min(this.m_bossCardBones.Count - 1, this.m_numBossesDefeated);
      if (this.m_numBossesDefeated >= bossesPerRun)
      {
        Log.Adventures.PrintWarning("AdventureDungeonCrawlPlayMat.SetUpDefeatedBosses() - Your run is done!  Why are you trying to set up defeated bosses?");
      }
      else
      {
        if (this.m_defeatedBossActors.Count < this.m_numBossesDefeated)
        {
          if ((UnityEngine.Object) this.m_bossActor == (UnityEngine.Object) null)
          {
            Log.Adventures.PrintError("AdventureDungeonCrawlDisplay attempting to clone from m_BossActor, but it is null!");
          }
          else
          {
            while (this.m_defeatedBossActors.Count < a)
            {
              Actor component = UnityEngine.Object.Instantiate<GameObject>(this.m_bossActor.gameObject).GetComponent<Actor>();
              GameUtils.SetParent((Component) component, (Component) this.m_bossCardBones[this.m_defeatedBossActors.Count], true);
              component.GetHealthObject().Hide();
              this.m_defeatedBossActors.Add(component);
            }
          }
        }
        if (a > 0 && this.m_defeatedBossActors.Count >= a)
        {
          int defeatedBossId = (int) defeatedBossIds[defeatedBossIds.Count - 1];
          string cardId = GameUtils.TranslateDbIdToCardId(defeatedBossId);
          if (cardId == null)
          {
            Log.Adventures.PrintWarning("AdventureDungeonCrawlPlayMat.SetUpDefeatedBosses() - No cardId for last defeated boss dbId {0}!", (object) defeatedBossId);
          }
          else
          {
            this.m_topDefeatedBoss = this.m_defeatedBossActors[this.m_defeatedBossActors.Count - 1];
            using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardId))
            {
              this.m_topDefeatedBoss.SetEntityDef(fullDef.EntityDef);
              this.m_topDefeatedBoss.SetCardDef(fullDef.DisposableCardDef);
              this.m_topDefeatedBoss.SetPremium(TAG_PREMIUM.NORMAL);
              this.m_topDefeatedBoss.UpdateAllComponents();
            }
          }
        }
        TransformUtil.AttachAndPreserveLocalTransform(this.m_nextBossBone, this.m_bossCardBones[Mathf.Min(a, this.m_bossCardBones.Count - 1)]);
        if (a == 0)
          this.m_allCards.SetActive(false);
        this.m_setUpDefeatedBossesCompleted = true;
      }
    }
  }

  public void SetUpCardBacks(
    int numUndefeatedBosses,
    AdventureDungeonCrawlPlayMat.AssetLoadCompletedCallback callback)
  {
    this.StartCoroutine(this.SetUpCardBacksWhenReady(numUndefeatedBosses, callback));
  }

  private IEnumerator SetUpCardBacksWhenReady(
    int numUndefeatedBosses,
    AdventureDungeonCrawlPlayMat.AssetLoadCompletedCallback callback)
  {
    while (this.m_loadingCardback)
      yield return (object) null;
    if ((UnityEngine.Object) this.m_nextBossCardBack == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlPlayMat.SetUpCardBacksWhenReady() - done loading cardback, but it must have failed!  Can't make more cardbacks!");
      if (callback != null)
        callback();
    }
    else
    {
      int num = Mathf.Min(numUndefeatedBosses, this.m_cardBackBones.Count);
      if (num == 0)
      {
        this.m_facedownCards.SetActive(false);
      }
      else
      {
        while (this.m_cardBacks.Count < num)
        {
          GameObject child = UnityEngine.Object.Instantiate<GameObject>(this.m_nextBossCardBack);
          Actor component = child.GetComponent<Actor>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            component.SetCardbackUpdateIgnore(true);
          GameUtils.SetParent(child, (Component) this.m_cardBackBones[this.m_cardBacks.Count], true);
          this.m_cardBacks.Add(child);
        }
      }
      if (callback != null)
        callback();
    }
  }

  private void SetUpPlayButton()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI || !((UnityEngine.Object) this.m_MobilePlayButtonSlidingTrayBone != (UnityEngine.Object) null))
      return;
    GameUtils.SetParent(this.m_PlayButtonRoot, (Component) this.m_MobilePlayButtonSlidingTrayBone);
    this.m_PlayButtonPlate.SetActive(true);
  }

  private void EnablePlayButton(bool enabled)
  {
    if (enabled)
      this.m_playButton.Enable();
    else
      this.m_playButton.Disable();
    if (!(bool) UniversalInputManager.UsePhoneUI || !((UnityEngine.Object) this.m_MobilePlayButtonSlidingTrayBone != (UnityEngine.Object) null))
      return;
    this.m_MobilePlayButtonSlidingTrayBone.ToggleTraySlider(enabled, animate: this.m_allowPlayButtonAnimation);
  }

  public void ShowTreasureOptions(List<long> treasureOptions)
  {
    if (treasureOptions == null || treasureOptions.Count == 0)
    {
      Log.Adventures.PrintWarning("AdventureDungeonCrawlPlayMat - Attempting to show Treasure, but no treasure was passed in!");
    }
    else
    {
      this.m_currentOptionType = AdventureDungeonCrawlPlayMat.OptionType.TREASURE;
      this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS, false);
      for (int index = 0; index < treasureOptions.Count; ++index)
      {
        if (index < this.m_rewardOptions.Count && (UnityEngine.Object) this.m_rewardOptions[index] != (UnityEngine.Object) null)
          this.m_rewardOptions[index].SetRewardData(new AdventureDungeonCrawlRewardOption.OptionData(AdventureDungeonCrawlPlayMat.OptionType.TREASURE, new List<long>()
          {
            treasureOptions[index]
          }, index));
      }
      this.SetPlayMatStateAsInitializedAndPlayTransition();
    }
  }

  public void ShowLootOptions(
    List<long> classLootOptionsA,
    List<long> classLootOptionsB,
    List<long> classLootOptionsC)
  {
    if ((classLootOptionsA == null || classLootOptionsA.Count == 0) && (classLootOptionsB == null || classLootOptionsB.Count == 0) && (classLootOptionsC == null || classLootOptionsC.Count == 0))
    {
      Log.Adventures.PrintWarning("AdventureDungeonCrawlPlayMat - Attempting to show Loot, but no loot was passed in!");
    }
    else
    {
      List<List<long>> longListList = new List<List<long>>()
      {
        classLootOptionsA,
        classLootOptionsB,
        classLootOptionsC
      };
      this.m_currentOptionType = AdventureDungeonCrawlPlayMat.OptionType.LOOT;
      this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS, false);
      for (int index = 0; index < this.m_rewardOptions.Count; ++index)
      {
        AdventureDungeonCrawlRewardOption rewardOption = this.m_rewardOptions[index];
        if (index < longListList.Count)
          rewardOption.SetRewardData(new AdventureDungeonCrawlRewardOption.OptionData(AdventureDungeonCrawlPlayMat.OptionType.LOOT, longListList[index], index));
        else
          break;
      }
      this.SetPlayMatStateAsInitializedAndPlayTransition();
    }
  }

  public void ShowShrineOptions(List<long> shrineOptions)
  {
    this.m_currentOptionType = AdventureDungeonCrawlPlayMat.OptionType.SHRINE_TREASURE;
    this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS, false);
    if (shrineOptions == null || shrineOptions.Count == 0)
    {
      Debug.LogError((object) "ShowShrineOptions - No shrines provided.");
    }
    else
    {
      for (int index = 0; index < this.m_rewardOptions.Count && shrineOptions.Count > index; ++index)
        this.m_rewardOptions[index].SetRewardData(new AdventureDungeonCrawlRewardOption.OptionData(AdventureDungeonCrawlPlayMat.OptionType.SHRINE_TREASURE, new List<long>()
        {
          shrineOptions[index]
        }, index));
      this.SetPlayMatStateAsInitializedAndPlayTransition();
    }
  }

  public void ShowTreasureSatchel(
    List<AdventureLoadoutTreasuresDbfRecord> adventureLoadoutTreasures,
    GameSaveKeyId adventureGameSaveServerKey,
    GameSaveKeyId adventureGameSaveClientKey)
  {
    if ((UnityEngine.Object) this.m_treasureSatchelWidget == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "AdventureDungeonCrawlPlayMat.ShowTreasureSatchel - m_treasureSatchel is null!");
    }
    else
    {
      this.m_currentOptionType = AdventureDungeonCrawlPlayMat.OptionType.TREASURE_SATCHEL;
      this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS, false);
      this.m_treasureSatchelWidget.gameObject.SetActive(true);
      this.m_treasureSatchelWidget.Hide();
      this.StartCoroutine(this.ShowTreasureSatchelWhenReady(adventureLoadoutTreasures, adventureGameSaveServerKey, adventureGameSaveClientKey));
    }
  }

  private IEnumerator ShowTreasureSatchelWhenReady(
    List<AdventureLoadoutTreasuresDbfRecord> adventureLoadoutTreasures,
    GameSaveKeyId adventureGameSaveServerKey,
    GameSaveKeyId adventureGameSaveClientKey)
  {
    while (!this.m_subsceneTransitionComplete || !this.m_treasureSatchelWidget.IsReady || this.m_treasureSatchelWidget.IsChangingStates)
      yield return (object) null;
    this.m_treasureSatchelWidget.TriggerEvent("CODE_TREASURE_SATCHEL_SHOW");
    while (this.m_treasureSatchelWidget.IsChangingStates)
      yield return (object) null;
    this.m_treasureSatchelWidget.Show();
    if (adventureLoadoutTreasures.Count > this.m_treasureSatchelOptions.Count)
      Log.Adventures.PrintWarning("AdventureDungeonCrawlPlayMat.ShowTreasureSatchelWhenReady - there are more Adventure Loadout Treasures than option visuals to show them! Number of Loadout Treasures: {0} Number of PlayMat options: {1}", (object) adventureLoadoutTreasures.Count, (object) this.m_treasureSatchelOptions.Count);
    IDataModel model;
    this.m_treasureSatchelWidget.GetDataModel(32, out model);
    if (!(model is AdventureTreasureSatchelDataModel satchelDataModel))
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlPlayMat.ShowTreasureSatchelWhenReady - satchel has no data model!");
    }
    else
    {
      satchelDataModel.Cards.Clear();
      List<long> treasureRunWins = this.TreasureWinsForScenario(adventureGameSaveServerKey, (int) this.m_dungeonCrawlData.GetMission());
      List<long> newlyUnlockedTreasures;
      GameSaveDataManager.Get().GetSubkeyValue(adventureGameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEWLY_UNLOCKED_LOADOUT_TREASURES, out newlyUnlockedTreasures);
      for (int i = 0; i < adventureLoadoutTreasures.Count; ++i)
      {
        if (i > this.m_treasureSatchelOptions.Count - 1)
        {
          Log.Adventures.PrintWarning("AdventureDungeonCrawlPlayMat.ShowTreasureSatchelWhenReady - there are not enough Adventure Loadout Treasures to fill the PlayMat options!  Number of CardDataModels: {0} Number of PlayMat options: {1}", (object) adventureLoadoutTreasures.Count, (object) this.m_treasureSatchelOptions.Count);
          break;
        }
        bool locked = false;
        bool upgraded = false;
        string lockedText = string.Empty;
        string unlockCriteriaText = (string) adventureLoadoutTreasures[i].UnlockCriteriaText;
        int cardDbId = adventureLoadoutTreasures[i].CardId;
        if (adventureLoadoutTreasures[i].UnlockValue > 0 | adventureLoadoutTreasures[i].UnlockAchievement > 0)
        {
          long unlockProgress;
          locked = !this.m_dungeonCrawlData.AdventureTreasureIsUnlocked(adventureGameSaveServerKey, adventureLoadoutTreasures[i], out unlockProgress, out bool _);
          if (locked && !string.IsNullOrEmpty(unlockCriteriaText))
          {
            int num1 = 0;
            if (adventureLoadoutTreasures[i].UnlockAchievement > 0)
              num1 = AchievementManager.Get().GetAchievementDataModel(adventureLoadoutTreasures[i].UnlockAchievement).Quota;
            int num2 = adventureLoadoutTreasures[i].UnlockValue + num1;
            lockedText = string.Format(unlockCriteriaText, (object) unlockProgress, (object) num2);
          }
        }
        if (adventureLoadoutTreasures[i].UpgradeValue > 0)
        {
          upgraded = this.m_dungeonCrawlData.AdventureTreasureIsUpgraded(adventureGameSaveServerKey, adventureLoadoutTreasures[i], out long _);
          if (upgraded)
            cardDbId = adventureLoadoutTreasures[i].UpgradedCardId;
        }
        bool completed = treasureRunWins != null && treasureRunWins.Contains((long) cardDbId);
        bool newlyUnlocked = newlyUnlockedTreasures != null && newlyUnlockedTreasures.Contains((long) cardDbId);
        AdventureDungeonCrawlTreasureOption treasureSatchelOption = this.m_treasureSatchelOptions[i];
        CardDataModel cardDataModel = new CardDataModel();
        satchelDataModel.Cards.Add(cardDataModel);
        if (cardDbId != 0 && cardDataModel != null)
        {
          string cardId = GameUtils.TranslateDbIdToCardId(cardDbId);
          CardDbfRecord cardRecord = GameDbf.GetIndex().GetCardRecord(cardId);
          cardDataModel.CardId = cardId;
          cardDataModel.FlavorText = (string) cardRecord?.FlavorText;
        }
        while (!treasureSatchelOption.IsReady)
          yield return (object) null;
        treasureSatchelOption.Init((long) cardDbId, locked, lockedText, upgraded, completed, newlyUnlocked, (AdventureOptionWidget.OptionAcknowledgedCallback) (() =>
        {
          if (!treasureSatchelOption.IsNewlyUnlocked)
            return;
          GameSaveDataManager.SubkeySaveRequest subkeyIfItExists = GameSaveDataManager.Get().GenerateSaveRequestToRemoveValueFromSubkeyIfItExists(adventureGameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEWLY_UNLOCKED_LOADOUT_TREASURES, (long) cardDbId);
          if (subkeyIfItExists == null)
            return;
          Log.Adventures.Print("Treasure Card {0} was Newly Unlocked but the player just acknowledged it, so saving that it is no longer Newly Unlocked.", (object) cardDbId);
          GameSaveDataManager.Get().SaveSubkey(subkeyIfItExists);
          treasureSatchelOption.IsNewlyUnlocked = false;
        }));
        lockedText = (string) null;
      }
      foreach (AdventureDungeonCrawlTreasureOption treasureSatchelOption in this.m_treasureSatchelOptions)
      {
        while (!treasureSatchelOption.IsReady)
          yield return (object) null;
      }
      this.SetPlayMatStateAsInitializedAndPlayTransition();
    }
  }

  public void ShowHeroPowers(
    List<AdventureHeroPowerDbfRecord> adventureHeroPowers,
    GameSaveKeyId adventureGameSaveServerKey,
    GameSaveKeyId adventureGameSaveClientKey)
  {
    this.m_currentOptionType = AdventureDungeonCrawlPlayMat.OptionType.HERO_POWER;
    this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS, false);
    if (adventureHeroPowers.Count > this.m_heroPowerOptions.Count)
      Log.Adventures.PrintWarning("There are more Adventure Hero Powers than option visuals to shown them! Number of Hero Powers: {0} Number of PlayMat options: {1}", (object) adventureHeroPowers.Count, (object) this.m_heroPowerOptions.Count);
    List<long> longList = this.HeroPowerWinsForScenario(adventureGameSaveServerKey, (int) this.m_dungeonCrawlData.GetMission());
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(adventureGameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEWLY_UNLOCKED_HERO_POWERS, out values);
    for (int index = 0; index < this.m_heroPowerOptions.Count; ++index)
    {
      if (index > adventureHeroPowers.Count - 1)
      {
        Log.Adventures.PrintWarning("There are not enough Adventure Hero Powers to fill the PlayMat options!  Number of Hero Powers: {0} Number of PlayMat options: {1}", (object) adventureHeroPowers.Count, (object) this.m_heroPowerOptions.Count);
        break;
      }
      bool locked = false;
      string lockedText = string.Empty;
      string unlockCriteriaText = (string) adventureHeroPowers[index].UnlockCriteriaText;
      int heroPowerDbId = adventureHeroPowers[index].CardId;
      bool flag = adventureHeroPowers[index].UnlockAchievement > 0;
      if (adventureHeroPowers[index].UnlockValue > 0 | flag)
      {
        long unlockProgress;
        locked = !this.m_dungeonCrawlData.AdventureHeroPowerIsUnlocked(adventureGameSaveServerKey, adventureHeroPowers[index], out unlockProgress, out bool _);
        if (locked && !string.IsNullOrEmpty(unlockCriteriaText))
        {
          int num1 = 0;
          if (adventureHeroPowers[index].UnlockAchievement > 0)
            num1 = AchievementManager.Get().GetAchievementDataModel(adventureHeroPowers[index].UnlockAchievement).Quota;
          int num2 = adventureHeroPowers[index].UnlockValue + num1;
          lockedText = string.Format(unlockCriteriaText, (object) unlockProgress, (object) num2);
        }
      }
      bool completed = longList != null && longList.Contains((long) heroPowerDbId);
      bool newlyUnlocked = values != null && values.Contains((long) heroPowerDbId);
      AdventureDungeonCrawlHeroPowerOption heroPowerOption = this.m_heroPowerOptions[index];
      heroPowerOption.Init((long) heroPowerDbId, locked, lockedText, completed, newlyUnlocked, (AdventureOptionWidget.OptionAcknowledgedCallback) (() =>
      {
        if (!heroPowerOption.IsNewlyUnlocked)
          return;
        GameSaveDataManager.SubkeySaveRequest subkeyIfItExists = GameSaveDataManager.Get().GenerateSaveRequestToRemoveValueFromSubkeyIfItExists(adventureGameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEWLY_UNLOCKED_HERO_POWERS, (long) heroPowerDbId);
        if (subkeyIfItExists == null)
          return;
        Log.Adventures.Print("Hero Power {0} was Newly Unlocked but the player just acknowledged it, so saving that it is no longer Newly Unlocked.", (object) heroPowerDbId);
        GameSaveDataManager.Get().SaveSubkey(subkeyIfItExists);
        heroPowerOption.IsNewlyUnlocked = false;
      }));
    }
    this.SetPlayMatStateAsInitializedAndPlayTransition();
  }

  public void ShowDecks(
    List<AdventureDeckDbfRecord> adventureDecks,
    GameSaveKeyId adventureGameSaveServerKey,
    GameSaveKeyId adventureGameSaveClientKey)
  {
    this.m_currentOptionType = AdventureDungeonCrawlPlayMat.OptionType.DECK;
    this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS, false);
    if (adventureDecks.Count > this.m_deckOptions.Count)
      Log.Adventures.PrintWarning("There are more Adventure Decks than option visuals to shown them! Number of Decks: {0} Number of PlayMat options: {1}", (object) adventureDecks.Count, (object) this.m_deckOptions.Count);
    List<long> longList = this.DeckWinsForScenario(adventureGameSaveServerKey, (int) this.m_dungeonCrawlData.GetMission());
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(adventureGameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEWLY_UNLOCKED_DECKS, out values);
    for (int index = 0; index < this.m_deckOptions.Count; ++index)
    {
      if (index > adventureDecks.Count - 1)
      {
        Log.Adventures.PrintWarning("There are not enough Adventure Decks to fill the PlayMat options!  Number of Decks: {0} Number of PlayMat options: {1}", (object) adventureDecks.Count, (object) this.m_deckOptions.Count);
        break;
      }
      bool locked = false;
      string lockedText = string.Empty;
      string unlockCriteriaText = (string) adventureDecks[index].UnlockCriteriaText;
      if (adventureDecks[index].UnlockValue > 0)
      {
        long unlockProgress;
        locked = !this.m_dungeonCrawlData.AdventureDeckIsUnlocked(adventureGameSaveServerKey, adventureDecks[index], out unlockProgress, out bool _);
        if (locked && !string.IsNullOrEmpty(unlockCriteriaText))
          lockedText = string.Format(unlockCriteriaText, (object) unlockProgress, (object) adventureDecks[index].UnlockValue);
      }
      AdventureDeckDbfRecord deckRecord = adventureDecks[index];
      bool completed = longList != null && longList.Contains((long) deckRecord.DeckId);
      bool newlyUnlocked = values != null && values.Contains((long) deckRecord.DeckId);
      AdventureDungeonCrawlDeckOption deckOption = this.m_deckOptions[index];
      deckOption.Init(adventureDecks[index], locked, lockedText, completed, newlyUnlocked, (AdventureOptionWidget.OptionAcknowledgedCallback) (() =>
      {
        if (!deckOption.IsNewlyUnlocked)
          return;
        GameSaveDataManager.SubkeySaveRequest subkeyIfItExists = GameSaveDataManager.Get().GenerateSaveRequestToRemoveValueFromSubkeyIfItExists(adventureGameSaveClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEWLY_UNLOCKED_DECKS, (long) deckRecord.DeckId);
        if (subkeyIfItExists == null)
          return;
        Log.Adventures.Print("Deck {0} was Newly Unlocked but the player just acknowledged it, so saving that it is no longer Newly Unlocked.", (object) deckRecord.DeckId);
        GameSaveDataManager.Get().SaveSubkey(subkeyIfItExists);
        deckOption.IsNewlyUnlocked = false;
      }));
    }
    this.m_playButton.SetText("GLUE_CHOOSE");
    this.SetPlayMatStateAsInitializedAndPlayTransition();
  }

  private GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys WingProgressSubkeysForScenario(
    int scenarioId)
  {
    GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys progressSubkeys;
    GameSaveDataManager.GetProgressSubkeysForDungeonCrawlWing(GameUtils.GetWingRecordFromMissionId(scenarioId), out progressSubkeys);
    return progressSubkeys;
  }

  private List<long> HeroPowerWinsForScenario(
    GameSaveKeyId adventureGameSaveServerKey,
    int scenarioId)
  {
    GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys wingProgressSubkeys = this.WingProgressSubkeysForScenario(scenarioId);
    if (wingProgressSubkeys.heroPowerWins == ~GameSaveKeySubkeyId.INVALID)
      return new List<long>();
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(adventureGameSaveServerKey, wingProgressSubkeys.heroPowerWins, out values);
    return values;
  }

  private List<long> DeckWinsForScenario(
    GameSaveKeyId adventureGameSaveServerKey,
    int scenarioId)
  {
    GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys wingProgressSubkeys = this.WingProgressSubkeysForScenario(scenarioId);
    if (wingProgressSubkeys.deckWins == ~GameSaveKeySubkeyId.INVALID)
      return new List<long>();
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(adventureGameSaveServerKey, wingProgressSubkeys.deckWins, out values);
    return values;
  }

  private List<long> TreasureWinsForScenario(
    GameSaveKeyId adventureGameSaveServerKey,
    int scenarioId)
  {
    GameSaveDataManager.AdventureDungeonCrawlWingProgressSubkeys wingProgressSubkeys = this.WingProgressSubkeysForScenario(scenarioId);
    if (wingProgressSubkeys.treasureWins == ~GameSaveKeySubkeyId.INVALID)
      return new List<long>();
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(adventureGameSaveServerKey, wingProgressSubkeys.treasureWins, out values);
    return values;
  }

  public void ShowNextBoss(string playButtonText)
  {
    this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_NEXT_BOSS, true);
    this.m_playButton.SetText(playButtonText);
  }

  public void ShowEmptyState() => this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.READY_FOR_DATA, true);

  public void ShowPVPDRActiveRun(string playButtonText)
  {
    this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_ACTIVE, true);
    this.m_playButton.SetText(playButtonText);
  }

  public void ShowPVPDRReward() => this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_REWARD, true);

  public void SetShouldShowBossHeroPowerTooltip(bool show) => this.m_shouldShowBossHeroPowerTooltip = show;

  public void ShowBossHeroPowerTooltip() => this.StartCoroutine(this.ShowBossHeroPowerTooltipWhenReady());

  private IEnumerator ShowBossHeroPowerTooltipWhenReady()
  {
    yield return (object) new WaitForSeconds(0.5f);
    bool wasWaitingOnVO = false;
    while (NotificationManager.Get().IsQuotePlaying)
    {
      wasWaitingOnVO = true;
      yield return (object) new WaitForEndOfFrame();
    }
    if (wasWaitingOnVO)
      yield return (object) new WaitForSeconds(this.m_bossHeroPowerTooltipDelayAfterVo);
    if ((!((UnityEngine.Object) this.m_bossHeroPowerTooltip != (UnityEngine.Object) null) || this.m_bossHeroPowerTooltip.IsDying()) && this.m_shouldShowBossHeroPowerTooltip)
    {
      this.m_bossHeroPowerTooltip = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.m_bossHeroPowerTooltipBone.transform.localPosition, this.m_bossHeroPowerTooltipBone.transform.localScale, GameStrings.Get((string) AdventureDungeonCrawlPlayMat.HERO_POWER_TOOLTIP_STRING));
      this.m_bossHeroPowerTooltip.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
      this.m_bossHeroPowerTooltip.PulseReminderEveryXSeconds(this.m_bossHeroPowerTooltipPulseRate);
    }
  }

  public void HideBossHeroPowerTooltip(bool immediate = false)
  {
    this.m_shouldShowBossHeroPowerTooltip = false;
    if (!((UnityEngine.Object) this.m_bossHeroPowerTooltip != (UnityEngine.Object) null))
      return;
    if (immediate)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_bossHeroPowerTooltip.gameObject);
      this.m_bossHeroPowerTooltip = (Notification) null;
    }
    else
    {
      this.m_bossHeroPowerTooltip.OnFinishDeathState += (Action<int>) (groupId => this.m_bossHeroPowerTooltip = (Notification) null);
      this.m_bossHeroPowerTooltip.PlayDeath();
    }
  }

  public AdventureDungeonCrawlPlayMat.PlayMatState GetPlayMatState() => this.m_playMatState;

  public AdventureDungeonCrawlPlayMat.OptionType GetPlayMatOptionType() => this.m_currentOptionType;

  private void SetPlayMatState(
    AdventureDungeonCrawlPlayMat.PlayMatState state,
    bool setAsInitialized)
  {
    if (AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE == this.m_playMatState && state != AdventureDungeonCrawlPlayMat.PlayMatState.READY_FOR_DATA)
    {
      Log.Adventures.PrintError("Attempting to set Adventure Dungeon Crawl Play Mat to state {0}, but still in state TRANSITIONING_FROM_PREV_STATE! This is not allowed!", (object) state);
    }
    else
    {
      Log.Adventures.Print("Setting Adventure Dungeon Crawl Play Mat to state {0}", (object) state);
      this.m_playMatStateInitialized = false;
      if (AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE != state)
      {
        this.m_nextBossPane.SetActive(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_NEXT_BOSS == state);
        this.m_optionsPane.SetActive(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS == state);
        this.m_bossGraveyardPane.gameObject.SetActive(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_BOSS_GRAVEYARD == state);
        this.m_duelsPlayMat.gameObject.SetActive(AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_ACTIVE == state || AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_REWARD == state);
        this.SetHeaderTextForState(state);
      }
      this.HandleDuelsPlayMatStateChange(state);
      if (AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS == state && (UnityEngine.Object) this.m_selectedOptionClickBlocker != (UnityEngine.Object) null)
        this.m_selectedOptionClickBlocker.SetActive(true);
      this.EnablePlayButton(false);
      if (this.m_playMatState != AdventureDungeonCrawlPlayMat.PlayMatState.READY_FOR_DATA && this.m_playMatState != AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE)
        this.m_lastVisualPlayMatState = this.m_playMatState;
      this.m_playMatState = state;
      if (!setAsInitialized)
        return;
      this.SetPlayMatStateAsInitializedAndPlayTransition();
    }
  }

  private void SetPlayMatStateAsInitializedAndPlayTransition()
  {
    this.m_playMatStateInitialized = true;
    if (!this.m_subsceneTransitionComplete)
      return;
    this.PlayStateTransition(this.m_playMatState);
  }

  private void SetHeaderTextForState(AdventureDungeonCrawlPlayMat.PlayMatState state)
  {
    this.SetHeaderOverrideStrings();
    this.m_headerText.Show();
    switch (state)
    {
      case AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS:
        switch (this.m_currentOptionType)
        {
          case AdventureDungeonCrawlPlayMat.OptionType.LOOT:
            this.m_headerText.Text = GameStrings.Get(string.IsNullOrEmpty(this.m_chooseLootHeaderStringOverride) ? "GLUE_ADVENTURE_DUNGEON_CRAWL_CHOOSE_LOOT" : this.m_chooseLootHeaderStringOverride);
            return;
          case AdventureDungeonCrawlPlayMat.OptionType.TREASURE:
            this.m_headerText.Text = GameStrings.Get(string.IsNullOrEmpty(this.m_chooseTreasureHeaderStringOverride) ? "GLUE_ADVENTURE_DUNGEON_CRAWL_CHOOSE_TREASURE" : this.m_chooseTreasureHeaderStringOverride);
            return;
          case AdventureDungeonCrawlPlayMat.OptionType.SHRINE_TREASURE:
            this.m_headerText.Text = GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_CHOOSE_SHRINE");
            return;
          case AdventureDungeonCrawlPlayMat.OptionType.HERO_POWER:
            this.m_headerText.Text = GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_CHOOSE_HERO_POWER");
            return;
          case AdventureDungeonCrawlPlayMat.OptionType.DECK:
            this.m_headerText.Text = GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_CHOOSE_DECK");
            return;
          case AdventureDungeonCrawlPlayMat.OptionType.TREASURE_SATCHEL:
            this.m_headerText.Hide();
            return;
          default:
            return;
        }
      case AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_NEXT_BOSS:
        if (this.IsNextMissionASpecialEncounter)
        {
          this.m_headerText.Text = GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_SPECIAL_ENCOUNTER");
          break;
        }
        this.m_headerText.Text = GameStrings.Format("GLUE_ADVENTURE_DUNGEON_CRAWL_CHALLENGE_COUNT", (object) (this.m_numBossesDefeated + 1), (object) this.m_bossesPerRun);
        break;
      case AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_ACTIVE:
      case AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_REWARD:
        this.m_headerText.Hide();
        break;
    }
  }

  public void ShowRunEnd(
    List<long> defeatedBossIds,
    long bossWhoDefeatedMeId,
    int numTotalBosses,
    bool hasCompletedAdventureWithAllClasses,
    bool firstTimeCompletedAsClass,
    int numClassesCompleted,
    GameSaveKeyId adventureGameSaveDataServerKey,
    GameSaveKeyId adventureGameSaveDataClientKey,
    AdventureDungeonCrawlPlayMat.AssetLoadCompletedCallback loadCompletedCallback,
    AdventureDungeonCrawlBossGraveyard.RunEndSequenceCompletedCallback sequenceCompletedCallback)
  {
    this.StartCoroutine(this.ShowRunEndAfterGraveyardIsInitialized(defeatedBossIds, bossWhoDefeatedMeId, numTotalBosses, hasCompletedAdventureWithAllClasses, firstTimeCompletedAsClass, numClassesCompleted, adventureGameSaveDataServerKey, adventureGameSaveDataClientKey, loadCompletedCallback, sequenceCompletedCallback));
  }

  public void OnSubSceneLoaded() => this.HideContentBeforeIntroAnims();

  public void OnSubSceneTransitionComplete() => this.StartCoroutine(this.ProcessSubsceneTransitionCompleteWhenReady());

  private IEnumerator ProcessSubsceneTransitionCompleteWhenReady()
  {
    while (GameUtils.IsAnyTransitionActive() || PopupDisplayManager.Get().IsShowing)
      yield return (object) null;
    this.m_subsceneTransitionComplete = true;
    this.m_allowPlayButtonAnimation = true;
    if ((UnityEngine.Object) this.m_bossGraveyard != (UnityEngine.Object) null)
      this.m_bossGraveyard.OnSubSceneTransitionComplete();
    this.PlayStateTransition(this.m_playMatState);
  }

  public void PlayRewardOptionSelected(
    AdventureDungeonCrawlRewardOption.OptionData optionData)
  {
    this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE, true);
    for (int index = 0; index < this.m_rewardOptions.Count; ++index)
    {
      this.m_rewardOptions[index].DisableInteraction();
      this.m_rewardOptions[index].PlayOutro(optionData.index == index);
    }
  }

  public void PlayDeckOptionSelected() => this.PlayWidgetOptionSelected(this.m_deckOptions.Cast<AdventureOptionWidget>());

  public void PlayHeroPowerOptionSelected() => this.PlayWidgetOptionSelected(this.m_heroPowerOptions.Cast<AdventureOptionWidget>());

  public void PlayTreasureSatchelOptionSelected() => this.PlayWidgetOptionSelected(this.m_treasureSatchelOptions.Cast<AdventureOptionWidget>());

  public void PlayTreasureSatchelOptionHidden()
  {
    this.PlayWidgetOptionSelected(this.m_treasureSatchelOptions.Cast<AdventureOptionWidget>());
    if (!((UnityEngine.Object) this.m_treasureSatchelWidget != (UnityEngine.Object) null))
      return;
    this.m_treasureSatchelWidget.TriggerEvent("PLAY_SATCHEL_MOTE_OUT");
  }

  private void PlayWidgetOptionSelected(IEnumerable<AdventureOptionWidget> options)
  {
    this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE, true);
    foreach (AdventureOptionWidget option in options)
      option.PlayOutro();
  }

  public Actor GetActorToAnimateFrom(string cardId, int index) => this.m_currentOptionType != AdventureDungeonCrawlPlayMat.OptionType.TREASURE_SATCHEL ? (index < 0 || index > this.m_rewardOptions.Count ? (Actor) null : this.m_rewardOptions[index].GetActorFromCardId(cardId)) : (index < 0 || index > this.m_treasureSatchelOptions.Count ? (Actor) null : this.m_treasureSatchelOptions[index].CardActor);

  private void PlayStateTransition(AdventureDungeonCrawlPlayMat.PlayMatState state)
  {
    if (!this.m_playMatStateInitialized)
      return;
    switch (state)
    {
      case AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS:
        this.StartCoroutine(this.HandleOptionIntroAnimations());
        break;
      case AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_NEXT_BOSS:
        this.StartCoroutine(this.PlayNextBossAnimations(this.m_lastVisualPlayMatState == AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS));
        break;
      case AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_ACTIVE:
        this.EnablePlayButton(true);
        break;
    }
  }

  private IEnumerator PlayNextBossAnimations(bool transitionFromPrevState)
  {
    int num = this.m_defeatedBossActors.Count == 0 ? 1 : 0;
    bool finalBoss = this.m_defeatedBossActors.Count == this.m_bossesPerRun - 1;
    if (num != 0)
    {
      this.m_allCards.SetActive(true);
      this.m_bossDeckDropAnimation.Play();
      SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit(this.m_bossDeckDropSFXOverride));
      while (this.m_bossDeckDropAnimation.isPlaying)
        yield return (object) null;
      yield return (object) new WaitForSeconds(this.m_delayAfterDeckDrop);
    }
    else if (transitionFromPrevState)
    {
      if ((UnityEngine.Object) this.m_nextBossCardBack != (UnityEngine.Object) null)
      {
        Actor component = this.m_nextBossCardBack.GetComponent<Actor>();
        if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        {
          component.ActivateSpellBirthState(SpellType.SUMMON_IN_DUNGEON_CRAWL);
          component.ActivateSpellBirthState(DraftDisplay.GetSpellTypeForRarity(TAG_RARITY.RARE));
        }
      }
      if ((UnityEngine.Object) this.m_topDefeatedBoss != (UnityEngine.Object) null)
      {
        this.m_topDefeatedBoss.ActivateSpellBirthState(SpellType.SUMMON_IN_DUNGEON_CRAWL);
        this.m_topDefeatedBoss.ActivateSpellBirthState(DraftDisplay.GetSpellTypeForRarity(TAG_RARITY.RARE));
      }
      SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit(this.m_bossDeckMagicallyAppearSFXOverride));
      yield return (object) new WaitForSeconds(0.7f);
    }
    this.m_nextBossFlipAnimation.Play(finalBoss ? this.m_nextBossFlipLargeName : this.m_nextBossFlipSmallName);
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit(finalBoss ? this.m_nextBossFlipLargeSFXOverride : this.m_nextBossFlipSmallSFXOverride));
    yield return (object) new WaitForSeconds(this.m_nextBossFlipCrowdReactionDelay);
    string str = this.m_nextBossFlipCrowdReactionMediumSFXOverride;
    if (this.m_numBossesDefeated == this.m_bossesPerRun - 1)
      str = this.m_nextBossFlipCrowdReactionLargeSFXOverride;
    else if (this.m_numBossesDefeated <= 3)
      str = this.m_nextBossFlipCrowdReactionSmallSFXOverride;
    SoundManager.Get().LoadAndPlay(AssetReference.op_Implicit(str));
    while (this.m_nextBossFlipAnimation.isPlaying)
      yield return (object) null;
    this.EnablePlayButton(true);
    this.ShowBossHeroPowerTooltip();
    this.PlayNextBossVO();
  }

  private void PlayNextBossVO()
  {
    if (this.m_bossActor.GetEntityDef() == null)
      return;
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(this.m_dungeonCrawlData.GetMission());
    int dbId = GameUtils.TranslateCardIdToDbId(this.m_bossActor.GetEntityDef().GetCardId());
    if ((this.m_numBossesDefeated + 1 < this.m_bossesPerRun ? 0 : (DungeonCrawlSubDef_VOLines.PlayVOLine(this.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, this.m_playerHeroDbId, DungeonCrawlSubDef_VOLines.VOEventType.FINAL_BOSS_REVEAL, dbId, false) ? 1 : 0)) != 0)
      return;
    DungeonCrawlSubDef_VOLines.PlayVOLine(this.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, this.m_playerHeroDbId, DungeonCrawlSubDef_VOLines.BOSS_REVEAL_EVENTS, dbId, false);
  }

  private IEnumerator HandleOptionIntroAnimations()
  {
    AdventureDungeonCrawlPlayMat dungeonCrawlPlayMat = this;
    if (dungeonCrawlPlayMat.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.TREASURE || dungeonCrawlPlayMat.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.SHRINE_TREASURE)
      yield return (object) dungeonCrawlPlayMat.StartCoroutine(dungeonCrawlPlayMat.PlayRewardOptionAnimations((IEnumerable<AdventureDungeonCrawlRewardOption>) dungeonCrawlPlayMat.m_rewardOptions, 0.0f));
    else if (dungeonCrawlPlayMat.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.LOOT)
    {
      List<AdventureDungeonCrawlRewardOption> options = new List<AdventureDungeonCrawlRewardOption>((IEnumerable<AdventureDungeonCrawlRewardOption>) dungeonCrawlPlayMat.m_rewardOptions);
      if (dungeonCrawlPlayMat.m_rewardOptions.Count >= 2)
      {
        options[0] = dungeonCrawlPlayMat.m_rewardOptions[1];
        options[1] = dungeonCrawlPlayMat.m_rewardOptions[0];
      }
      yield return (object) dungeonCrawlPlayMat.StartCoroutine(dungeonCrawlPlayMat.PlayRewardOptionAnimations((IEnumerable<AdventureDungeonCrawlRewardOption>) options, dungeonCrawlPlayMat.m_lootDropDelay));
    }
    else if (dungeonCrawlPlayMat.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.HERO_POWER)
      yield return (object) dungeonCrawlPlayMat.StartCoroutine(dungeonCrawlPlayMat.PlayWidgetOptionAnimations(dungeonCrawlPlayMat.m_heroPowerOptions.Cast<AdventureOptionWidget>(), 0.0f));
    else if (dungeonCrawlPlayMat.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.DECK)
      yield return (object) dungeonCrawlPlayMat.StartCoroutine(dungeonCrawlPlayMat.PlayWidgetOptionAnimations(dungeonCrawlPlayMat.m_deckOptions.Cast<AdventureOptionWidget>(), dungeonCrawlPlayMat.m_lootDropDelay));
    else if (dungeonCrawlPlayMat.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.TREASURE_SATCHEL)
      yield return (object) dungeonCrawlPlayMat.StartCoroutine(dungeonCrawlPlayMat.PlayWidgetOptionAnimations(dungeonCrawlPlayMat.m_treasureSatchelOptions.Cast<AdventureOptionWidget>(), 0.0f));
    if ((UnityEngine.Object) dungeonCrawlPlayMat.m_selectedOptionClickBlocker != (UnityEngine.Object) null)
      dungeonCrawlPlayMat.m_selectedOptionClickBlocker.SetActive(false);
    dungeonCrawlPlayMat.PlaySelectedOptionVO();
  }

  private IEnumerator PlayRewardOptionAnimations(
    IEnumerable<AdventureDungeonCrawlRewardOption> options,
    float dropDelay)
  {
    this.HideContentBeforeIntroAnims();
    yield return (object) new WaitForSeconds(0.5f);
    AdventureDungeonCrawlRewardOption option;
    foreach (AdventureDungeonCrawlRewardOption option1 in options)
    {
      option = option1;
      while (!option.IsInitialized())
        yield return (object) null;
      option = (AdventureDungeonCrawlRewardOption) null;
    }
    foreach (AdventureDungeonCrawlRewardOption option2 in options)
    {
      option2.gameObject.SetActive(true);
      option2.PlayIntro();
      yield return (object) new WaitForSeconds(dropDelay);
    }
    foreach (AdventureDungeonCrawlRewardOption option3 in options)
    {
      option = option3;
      while (option.IntroIsPlaying())
        yield return (object) null;
      option = (AdventureDungeonCrawlRewardOption) null;
    }
    int num = 0;
    foreach (AdventureDungeonCrawlRewardOption option4 in options)
    {
      if (!option4.gameObject.activeInHierarchy)
      {
        Debug.LogWarning((object) ("AdventureDungeonCrawlPlayMat: The reward option at " + (object) num + " was inactive when it was supposed to show"));
        option4.gameObject.SetActive(true);
      }
      ++num;
    }
  }

  private IEnumerator PlayWidgetOptionAnimations(
    IEnumerable<AdventureOptionWidget> options,
    float dropDelay)
  {
    AdventureOptionWidget option;
    foreach (AdventureOptionWidget option1 in options)
    {
      option = option1;
      while (!option.IsReady)
        yield return (object) null;
      option = (AdventureOptionWidget) null;
    }
    foreach (AdventureOptionWidget option2 in options)
    {
      option2.PlayIntro();
      if ((double) dropDelay > 0.0)
        yield return (object) new WaitForSeconds(dropDelay);
    }
    foreach (AdventureOptionWidget option3 in options)
    {
      option = option3;
      while (option.IsIntroPlaying)
        yield return (object) null;
      option = (AdventureOptionWidget) null;
    }
  }

  private void PlaySelectedOptionVO()
  {
    if (this.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.TREASURE || this.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.SHRINE_TREASURE || this.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.TREASURE_SATCHEL)
      this.PlayTreasureOfferVO();
    else if (this.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.LOOT)
      this.PlayLootPackOfferVO();
    else if (this.m_currentOptionType == AdventureDungeonCrawlPlayMat.OptionType.HERO_POWER)
    {
      this.PlayHeroPowerOfferVO();
    }
    else
    {
      if (this.m_currentOptionType != AdventureDungeonCrawlPlayMat.OptionType.DECK)
        return;
      this.PlayDeckOfferVO();
    }
  }

  private void PlayTreasureOfferVO()
  {
    Options.Get().SetBool(Option.HAS_JUST_SEEN_LOOT_NO_TAKE_CANDLE_VO, false);
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(this.m_dungeonCrawlData.GetMission());
    if (DungeonCrawlSubDef_VOLines.PlayVOLine(this.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, this.m_playerHeroDbId, DungeonCrawlSubDef_VOLines.OFFER_TREASURE_EVENTS))
      return;
    foreach (AdventureDungeonCrawlRewardOption rewardOption in this.m_rewardOptions)
    {
      int treasureDatabaseId = rewardOption.GetTreasureDatabaseID();
      if (DungeonCrawlSubDef_VOLines.PlayVOLine(this.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, this.m_playerHeroDbId, DungeonCrawlSubDef_VOLines.OFFER_TREASURE_EVENTS, treasureDatabaseId))
      {
        if (treasureDatabaseId != 47251)
          break;
        Options.Get().SetBool(Option.HAS_JUST_SEEN_LOOT_NO_TAKE_CANDLE_VO, true);
        break;
      }
    }
  }

  private void PlayLootPackOfferVO()
  {
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(this.m_dungeonCrawlData.GetMission());
    DungeonCrawlSubDef_VOLines.PlayVOLine(this.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, this.m_playerHeroDbId, DungeonCrawlSubDef_VOLines.OFFER_LOOT_PACKS_EVENTS);
  }

  private void PlayHeroPowerOfferVO()
  {
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(this.m_dungeonCrawlData.GetMission());
    DungeonCrawlSubDef_VOLines.PlayVOLine(this.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, this.m_playerHeroDbId, DungeonCrawlSubDef_VOLines.OFFER_HERO_POWER_EVENTS, (int) this.m_dungeonCrawlData.SelectedHeroPowerDbId);
  }

  private void PlayDeckOfferVO()
  {
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(this.m_dungeonCrawlData.GetMission());
    DungeonCrawlSubDef_VOLines.PlayVOLine(this.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, this.m_playerHeroDbId, DungeonCrawlSubDef_VOLines.OFFER_DECK_EVENTS, (int) this.m_dungeonCrawlData.SelectedDeckId);
  }

  private void HideContentBeforeIntroAnims()
  {
    if (this.m_playMatState != AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS)
      return;
    foreach (AdventureDungeonCrawlRewardOption rewardOption in this.m_rewardOptions)
    {
      if ((UnityEngine.Object) rewardOption != (UnityEngine.Object) null)
        rewardOption.gameObject.SetActive(false);
    }
  }

  private IEnumerator ShowRunEndAfterGraveyardIsInitialized(
    List<long> defeatedBossIds,
    long bossWhoDefeatedMeId,
    int numTotalBosses,
    bool hasCompletedAdventureWithAllClasses,
    bool firstTimeCompletedAsClass,
    int numClassesCompleted,
    GameSaveKeyId adventureGameSaveDataServerKey,
    GameSaveKeyId adventureGameSaveDataClientKey,
    AdventureDungeonCrawlPlayMat.AssetLoadCompletedCallback loadCompletedCallback,
    AdventureDungeonCrawlBossGraveyard.RunEndSequenceCompletedCallback sequenceCompletedCallback)
  {
    this.SetPlayMatState(AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_BOSS_GRAVEYARD, false);
    while (!this.m_bossGraveyardPane.PrefabIsLoaded() || (UnityEngine.Object) this.m_paperController == (UnityEngine.Object) null)
      yield return (object) null;
    yield return (object) new WaitForEndOfFrame();
    if ((UnityEngine.Object) this.m_bossGraveyard == (UnityEngine.Object) null)
    {
      this.m_bossGraveyard = this.m_bossGraveyardPane.PrefabGameObject().GetComponent<AdventureDungeonCrawlBossGraveyard>();
      if (this.m_subsceneTransitionComplete)
        this.m_bossGraveyard.OnSubSceneTransitionComplete();
    }
    if ((UnityEngine.Object) this.m_paperController != (UnityEngine.Object) null && (bool) UniversalInputManager.UsePhoneUI)
      this.m_paperController.gameObject.SetActive(false);
    this.SetPlayMatStateAsInitializedAndPlayTransition();
    this.m_bossGraveyard.ShowRunEnd(this.m_dungeonCrawlData, defeatedBossIds, bossWhoDefeatedMeId, numTotalBosses, hasCompletedAdventureWithAllClasses, firstTimeCompletedAsClass, numClassesCompleted, this.m_playerHeroDbId, adventureGameSaveDataServerKey, adventureGameSaveDataClientKey, loadCompletedCallback, sequenceCompletedCallback);
  }

  private void SetPlaymatVisualStyle()
  {
    DungeonRunVisualStyle visualStyle = this.m_dungeonCrawlData.VisualStyle;
    this.m_nextBossFlipSmallSFXOverride = this.m_nextBossFlipSmallSFXDefault;
    this.m_nextBossFlipLargeSFXOverride = this.m_nextBossFlipLargeSFXDefault;
    this.m_nextBossFlipCrowdReactionSmallSFXOverride = this.m_nextBossFlipCrowdReactionSmallSFXDefault;
    this.m_nextBossFlipCrowdReactionMediumSFXOverride = this.m_nextBossFlipCrowdReactionMediumSFXDefault;
    this.m_nextBossFlipCrowdReactionLargeSFXOverride = this.m_nextBossFlipCrowdReactionLargeSFXDefault;
    this.m_bossDeckDropSFXOverride = this.m_bossDeckDropSFXDefault;
    this.m_bossDeckMagicallyAppearSFXOverride = this.m_bossDeckMagicallyAppearSFXDefault;
    foreach (AdventureDungeonCrawlPlayMat.PlaymatStyleOverride playmatStyleOverride in this.m_playmatStyleOverride)
    {
      if (playmatStyleOverride.VisualStyle == visualStyle)
      {
        this.m_matchingPlaymatStyle = playmatStyleOverride;
        if ((bool) UniversalInputManager.UsePhoneUI)
        {
          this.m_headerText.TextColor = playmatStyleOverride.PhoneHeaderTextColor;
          this.m_headerText.OutlineColor = playmatStyleOverride.PhoneHeaderOutlineColor;
        }
        if ((UnityEngine.Object) this.m_nextBossPlayNewParticlesScript != (UnityEngine.Object) null)
        {
          this.m_nextBossPlayNewParticlesScript.m_Target = playmatStyleOverride.NextBossDustEffectSmall.gameObject;
          this.m_nextBossPlayNewParticlesScript.m_Target2 = playmatStyleOverride.NextBossDustEffectLargeMotes.gameObject;
          this.m_nextBossPlayNewParticlesScript.m_Target3 = playmatStyleOverride.NextBossDustEffectLarge.gameObject;
        }
        if ((UnityEngine.Object) this.m_facedownBossesPlayNewParticlesScript != (UnityEngine.Object) null)
          this.m_facedownBossesPlayNewParticlesScript.m_Target = playmatStyleOverride.FacedownBossesDustEffect.gameObject;
        if (!string.IsNullOrEmpty(playmatStyleOverride.NextBossFlipSmallSFX))
          this.m_nextBossFlipSmallSFXOverride = playmatStyleOverride.NextBossFlipSmallSFX;
        if (!string.IsNullOrEmpty(playmatStyleOverride.NextBossFlipLargeSFX))
          this.m_nextBossFlipLargeSFXOverride = playmatStyleOverride.NextBossFlipLargeSFX;
        if (!string.IsNullOrEmpty(playmatStyleOverride.NextBossFlipCrowdReactionSmallSFX))
          this.m_nextBossFlipCrowdReactionSmallSFXOverride = playmatStyleOverride.NextBossFlipCrowdReactionSmallSFX;
        if (!string.IsNullOrEmpty(playmatStyleOverride.NextBossFlipCrowdReactionMediumSFX))
          this.m_nextBossFlipCrowdReactionMediumSFXOverride = playmatStyleOverride.NextBossFlipCrowdReactionMediumSFX;
        if (!string.IsNullOrEmpty(playmatStyleOverride.NextBossFlipCrowdReactionLargeSFX))
          this.m_nextBossFlipCrowdReactionLargeSFXOverride = playmatStyleOverride.NextBossFlipCrowdReactionLargeSFX;
        if (!string.IsNullOrEmpty(playmatStyleOverride.BossDeckDropSFX))
          this.m_bossDeckDropSFXOverride = playmatStyleOverride.BossDeckDropSFX;
        if (string.IsNullOrEmpty(playmatStyleOverride.BossDeckMagicallyAppearSFX))
          break;
        this.m_bossDeckMagicallyAppearSFXOverride = playmatStyleOverride.BossDeckMagicallyAppearSFX;
        break;
      }
    }
  }

  private void SetHeaderOverrideStrings()
  {
    if (this.m_matchingPlaymatStyle == null)
      return;
    AdventureDungeonCrawlPlayMat.HeaderStringOverride headerStringOverride1 = (AdventureDungeonCrawlPlayMat.HeaderStringOverride) null;
    AdventureDungeonCrawlPlayMat.HeaderStringOverride headerStringOverride2 = (AdventureDungeonCrawlPlayMat.HeaderStringOverride) null;
    if (this.m_matchingPlaymatStyle.ChooseTreasureHeaderString.Any<AdventureDungeonCrawlPlayMat.HeaderStringOverride>())
      headerStringOverride1 = this.m_matchingPlaymatStyle.ChooseTreasureHeaderString.OrderByDescending<AdventureDungeonCrawlPlayMat.HeaderStringOverride, int>((Func<AdventureDungeonCrawlPlayMat.HeaderStringOverride, int>) (s => s.MinimumDefeatedBosses)).First<AdventureDungeonCrawlPlayMat.HeaderStringOverride>((Func<AdventureDungeonCrawlPlayMat.HeaderStringOverride, bool>) (s => s.MinimumDefeatedBosses <= this.m_numBossesDefeated));
    if (this.m_matchingPlaymatStyle.ChooseLootHeaderString.Any<AdventureDungeonCrawlPlayMat.HeaderStringOverride>())
      headerStringOverride2 = this.m_matchingPlaymatStyle.ChooseLootHeaderString.OrderByDescending<AdventureDungeonCrawlPlayMat.HeaderStringOverride, int>((Func<AdventureDungeonCrawlPlayMat.HeaderStringOverride, int>) (s => s.MinimumDefeatedBosses)).First<AdventureDungeonCrawlPlayMat.HeaderStringOverride>((Func<AdventureDungeonCrawlPlayMat.HeaderStringOverride, bool>) (s => s.MinimumDefeatedBosses <= this.m_numBossesDefeated));
    if (headerStringOverride1 != null)
      this.m_chooseTreasureHeaderStringOverride = headerStringOverride1.HeaderString;
    if (headerStringOverride2 == null)
      return;
    this.m_chooseLootHeaderStringOverride = headerStringOverride2.HeaderString;
  }

  private void OnPlayButtonReady(PlayButton playButton)
  {
    if ((UnityEngine.Object) playButton == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "PlayButtonReference is null, or does not have a PlayButton component on it!");
    else
      this.m_playButton = playButton;
  }

  private void OnPaperControllerReady(VisualController paperController)
  {
    if ((UnityEngine.Object) paperController == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Issue!", "PlayMat's m_paperControllerReference is null! Can't set the correct PlayMat texture!.");
    else if (this.m_dungeonCrawlData == null)
    {
      Error.AddDevWarning("UI Issue!", "PlayMat's m_dungeonCrawlData is null! Can't set the correct PlayMat texture!.");
    }
    else
    {
      this.m_paperController = paperController;
      int mission = (int) this.m_dungeonCrawlData.GetMission();
      WingDbfRecord recordFromMissionId = GameUtils.GetWingRecordFromMissionId(mission);
      if (recordFromMissionId == null)
        Log.Adventures.PrintError("No WingDbfRecord found for ScenarioDbId {0}!", (object) mission);
      else
        paperController.SetState(recordFromMissionId.VisualStateName);
    }
  }

  private void OnTreasureSatchelReady(Widget widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "AdventureDungeonCrawlPlayMat.OnTreasureSatchelReady - widget was null!");
    }
    else
    {
      AdventureTreasureSatchelDataModel satchelDataModel = new AdventureTreasureSatchelDataModel();
      this.m_treasureSatchelOptions = new List<AdventureDungeonCrawlTreasureOption>((IEnumerable<AdventureDungeonCrawlTreasureOption>) widget.GetComponentsInChildren<AdventureDungeonCrawlTreasureOption>());
      foreach (AdventureDungeonCrawlTreasureOption treasureSatchelOption in this.m_treasureSatchelOptions)
        satchelDataModel.LoadoutOptions.Add(treasureSatchelOption.GetDataModel());
      widget.BindDataModel((IDataModel) satchelDataModel);
      widget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
      {
        if (eventName == "CODE_TREASURE_SATCHEL_OUTRO_COMPLETE")
        {
          widget.Hide();
        }
        else
        {
          if (!(eventName == "CODE_TREASURE_OPTION_SELECTED"))
            return;
          IDataModel model;
          if ((UnityEngine.Object) this.m_treasureInspectWidget == (UnityEngine.Object) null || !this.m_treasureInspectWidget.GetDataModel(27, out model))
            Debug.LogError((object) "AdventureDungeonCrawlPlayMat.OnTreasureSatchelReady - selected event called with no CardDataModel found or treasure inspect widget didn't load!");
          else if (!(model is CardDataModel))
          {
            Debug.LogError((object) "AdventureDungeonCrawlPlayMat.OnTreasureSatchelReady - selected event called but CardDataModel was null!");
          }
          else
          {
            EventDataModel dataModel = widget.GetDataModel<EventDataModel>();
            int num = 0;
            if (dataModel.Payload is IConvertible)
              num = Convert.ToInt32(dataModel.Payload);
            for (int index = 0; index < this.m_treasureSatchelOptions.Count; ++index)
            {
              AdventureDungeonCrawlTreasureOption treasureSatchelOption = this.m_treasureSatchelOptions[index];
              if (index == num)
                treasureSatchelOption.Select();
              else
                treasureSatchelOption.Deselect();
            }
          }
        }
      }));
    }
  }

  private void OnTreasureInspectReady(Widget widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      Debug.LogError((object) "AdventureDungeonCrawlPlayMat.OnTreasureSatchelReady - widget was null!");
    else
      this.m_treasureInspectWidget = widget;
  }

  public bool IsPaperControllerReady() => (UnityEngine.Object) this.m_paperController != (UnityEngine.Object) null && !this.m_paperController.IsChangingStates;

  public void OnPVPDRPlayMatReady(DuelsPlayMat playMat)
  {
    if ((UnityEngine.Object) playMat == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "AdventureDungeonCrawlPlayMat.OnPVPDRPlayMatReady - widget loaded did not have DuelsPlayMat script!");
    }
    else
    {
      this.m_duelsPlayMat = playMat;
      this.m_duelsPlayWidget = this.m_duelsPlayMat.GetComponent<Widget>();
      if (!((UnityEngine.Object) PvPDungeonRunDisplay.Get() != (UnityEngine.Object) null))
        return;
      this.m_duelsPlayWidget.BindDataModel((IDataModel) PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel());
    }
  }

  public bool IsReadyToShowDuelsRewards() => this.m_duelsReadyToShowRewards;

  private void OnDuelsVaultOpened()
  {
    this.m_duelsReadyToShowRewards = true;
    this.m_duelsPlayMat.SetLeverButtonEnabled(false);
    this.m_duelsPlayMat.RemoveVaultDoorOpenedListener(new Action(this.OnDuelsVaultOpened));
    if (DuelsConfig.Get().GetRewardNoticeToShow() != null)
      return;
    AdventureDungeonCrawlDisplay.Get().EndDuelsSession();
  }

  private void OnDuelsVaultClicked()
  {
    AdventureDungeonCrawlDisplay.Get().SetShowDeckButtonEnabled(false);
    this.m_duelsPlayMat.RemoveVaultDoorClickedListener(new Action(this.OnDuelsVaultClicked));
  }

  public void OnDuelsRewardsAccepted() => this.m_duelsReadyToShowRewards = false;

  private void HandleDuelsPlayMatStateChange(AdventureDungeonCrawlPlayMat.PlayMatState state)
  {
    if (AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_ACTIVE != state && AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_REWARD != state)
      return;
    bool isPaidEntry = PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().IsPaidEntry;
    this.m_duelsPlayWidget.TriggerEvent(isPaidEntry ? DuelsConfig.ANIMATE_PAID_STATE : DuelsConfig.ANIMATE_FREE_STATE);
    DuelsConfig.Get().ResetLastGameResult();
    this.m_duelsPlayMat.SetLeverButtonEnabled(AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_REWARD == state & isPaidEntry);
    if (AdventureDungeonCrawlPlayMat.PlayMatState.PVPDR_REWARD != state)
      return;
    this.m_playButton.Disable();
    if (isPaidEntry)
    {
      this.m_duelsPlayMat.RegisterVaultDoorOpenedListener(new Action(this.OnDuelsVaultOpened));
      this.m_duelsPlayMat.RegisterVaultDoorClickedListener(new Action(this.OnDuelsVaultClicked));
    }
    else
      AdventureDungeonCrawlDisplay.Get().EndDuelsSession();
  }

  public delegate void RewardOptionSelectedCallback(
    AdventureDungeonCrawlRewardOption.OptionData rewardData);

  public delegate void AssetLoadCompletedCallback();

  public enum PlayMatState
  {
    READY_FOR_DATA,
    SHOWING_OPTIONS,
    TRANSITIONING_FROM_PREV_STATE,
    SHOWING_NEXT_BOSS,
    SHOWING_BOSS_GRAVEYARD,
    PVPDR_ACTIVE,
    PVPDR_REWARD,
  }

  public enum OptionType
  {
    INVALID,
    LOOT,
    TREASURE,
    SHRINE_TREASURE,
    HERO_POWER,
    DECK,
    TREASURE_SATCHEL,
  }

  [Serializable]
  public class PlaymatStyleOverride
  {
    public DungeonRunVisualStyle VisualStyle;
    public Color PhoneHeaderTextColor;
    public Color PhoneHeaderOutlineColor;
    public ParticleSystem NextBossDustEffectSmall;
    public ParticleSystem NextBossDustEffectLarge;
    public ParticleSystem NextBossDustEffectLargeMotes;
    public ParticleSystem FacedownBossesDustEffect;
    [CustomEditField(Sections = "SFX Overrides", T = EditType.SOUND_PREFAB)]
    public string NextBossFlipSmallSFX;
    [CustomEditField(Sections = "SFX Overrides", T = EditType.SOUND_PREFAB)]
    public string NextBossFlipLargeSFX;
    [CustomEditField(Sections = "SFX Overrides", T = EditType.SOUND_PREFAB)]
    public string NextBossFlipCrowdReactionSmallSFX;
    [CustomEditField(Sections = "SFX Overrides", T = EditType.SOUND_PREFAB)]
    public string NextBossFlipCrowdReactionMediumSFX;
    [CustomEditField(Sections = "SFX Overrides", T = EditType.SOUND_PREFAB)]
    public string NextBossFlipCrowdReactionLargeSFX;
    [CustomEditField(Sections = "SFX Overrides", T = EditType.SOUND_PREFAB)]
    public string BossDeckDropSFX;
    [CustomEditField(Sections = "SFX Overrides", T = EditType.SOUND_PREFAB)]
    public string BossDeckMagicallyAppearSFX;
    [CustomEditField(Sections = "String Overrides")]
    public List<AdventureDungeonCrawlPlayMat.HeaderStringOverride> ChooseTreasureHeaderString;
    [CustomEditField(Sections = "String Overrides")]
    public List<AdventureDungeonCrawlPlayMat.HeaderStringOverride> ChooseLootHeaderString;
  }

  [Serializable]
  public class HeaderStringOverride
  {
    public int MinimumDefeatedBosses;
    public string HeaderString;
  }
}
