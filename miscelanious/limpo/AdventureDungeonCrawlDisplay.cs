using Assets;
using Blizzard.T5.Configuration;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DataModels;
using Hearthstone.DungeonCrawl;
using Hearthstone.UI;
using PegasusClient;
using PegasusShared;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureDungeonCrawlDisplay : MonoBehaviour
{
  [CustomEditField(Sections = "UI")]
  public UberText m_AdventureTitle;
  [CustomEditField(Sections = "UI")]
  public UIBButton m_BackButton;
  [CustomEditField(Sections = "UI")]
  public AdventureDungeonCrawlDeckTray m_dungeonCrawlDeckTray;
  [CustomEditField(Sections = "UI")]
  public AsyncReference m_dungeonCrawlDeckSelectWidgetReference;
  [CustomEditField(Sections = "UI")]
  public AsyncReference m_dungeonCrawlPlayMatReference;
  [CustomEditField(Sections = "UI")]
  public AsyncReference m_heroClassIconsControllerReference;
  [CustomEditField(Sections = "UI")]
  public GameObject m_dungeonCrawlTray;
  [CustomEditField(Sections = "UI")]
  public DungeonCrawlBossKillCounter m_bossKillCounter;
  [CustomEditField(Sections = "UI")]
  public HighlightState m_backButtonHighlight;
  [CustomEditField(Sections = "UI")]
  public float m_RolloverTimeToHideBossHeroPowerTooltip = 0.35f;
  [CustomEditField(Sections = "UI")]
  public Material m_anomalyModeCardHighlightMaterial;
  [CustomEditField(Sections = "UI")]
  public float m_BigCardScale = 1f;
  [CustomEditField(Sections = "UI")]
  public AsyncReference m_retireButtonReference;
  [CustomEditField(Sections = "Animation")]
  public PlayMakerFSM m_HeroPowerPortraitPlayMaker;
  [CustomEditField(Sections = "Animation")]
  public string m_HeroPowerPotraitIntroStateName;
  [CustomEditField(Sections = "Bones")]
  public Transform m_socketHeroBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_heroPowerBone;
  [CustomEditField(Sections = "Bones")]
  public GameObject m_BossPowerBone;
  [CustomEditField(Sections = "Bones")]
  public GameObject m_HeroPowerBigCardBone;
  [CustomEditField(Sections = "Styles")]
  public List<AdventureDungeonCrawlDisplay.DungeonCrawlDisplayStyleOverride> m_DungeonCrawlDisplayStyle;
  [CustomEditField(Sections = "Phone")]
  public UIBButton m_ShowDeckButton;
  [CustomEditField(Sections = "Phone")]
  public GameObject m_ShowDeckButtonFrame;
  [CustomEditField(Sections = "Phone")]
  public GameObject m_ShowDeckNoButtonFrame;
  [CustomEditField(Sections = "Phone")]
  public GameObject m_PhoneDeckTray;
  [CustomEditField(Sections = "Phone")]
  public GameObject m_DeckTrayRunCompleteBone;
  [CustomEditField(Sections = "Phone")]
  public GameObject m_DeckListHeaderRunCompleteBone;
  [CustomEditField(Sections = "Phone")]
  public TraySection m_DeckListHeaderPrefab;
  [CustomEditField(Sections = "Phone")]
  public GameObject m_TrayFrameDefault;
  [CustomEditField(Sections = "Phone")]
  public GameObject m_TrayFrameRunComplete;
  [CustomEditField(Sections = "Phone")]
  public GameObject m_AdventureTitleRunCompleteBone;
  [CustomEditField(Sections = "Phone")]
  public Vector3 m_DeckBigCardOffsetForRunCompleteState;
  [CustomEditField(Sections = "Phone")]
  public GameObject m_ViewDeckTrayMesh;
  public AdventureDungeonCrawlPlayMat m_playMat;
  public static bool s_shouldShowWelcomeBanner = true;
  private bool m_subsceneTransitionComplete;
  private CollectionDeck m_dungeonCrawlDeck;
  private DungeonCrawlDeckSelect m_dungeonCrawlDeckSelect;
  private Actor m_heroActor;
  private AdventureDungeonCrawlDisplay.PlayerHeroData m_playerHeroData;
  private int m_numBossesDefeated;
  private List<long> m_defeatedBossIds;
  private long m_bossWhoDefeatedMeId;
  private long m_nextBossHealth;
  private string m_nextBossCardId;
  private long m_heroHealth;
  private bool m_isRunActive;
  private bool m_isRunRetired;
  private int m_selectedShrineIndex;
  private List<long> m_cardsAddedToDeckMap = new List<long>();
  private bool m_hasSeenLatestDungeonRunComplete;
  private List<long> m_shrineOptions;
  private long m_anomalyModeCardDbId;
  private long m_plotTwistCardDbId;
  private static GameSaveKeyId m_gameSaveDataServerKey;
  private static GameSaveKeyId m_gameSaveDataClientKey;
  private bool m_hasReceivedGameSaveDataServerKeyResponse;
  private bool m_hasReceivedGameSaveDataClientKeyResponse;
  private bool m_saveHeroDataUsingHeroId;
  private int m_numBossesInRun;
  private int m_bossCardBackId;
  private bool m_shouldSkipHeroSelect;
  private bool m_mustPickShrine;
  private bool m_mustSelectChapter;
  private Coroutine m_bossHeroPowerHideCoroutine;
  private IDungeonCrawlData m_dungeonCrawlData;
  private ISubsceneController m_subsceneController;
  private AssetLoadingHelper m_assetLoadingHelper;
  private Actor m_bossActor;
  private Actor m_bossPowerBigCard;
  private Actor m_heroPowerActor;
  private DefLoader.DisposableFullDef m_currentBossHeroPowerFullDef;
  private Actor m_heroPowerBigCard;
  private DefLoader.DisposableFullDef m_currentHeroPowerFullDef;
  private GameObject m_retireButton;
  private AdventureDungeonCrawlDisplay.DungeonRunLoadoutState m_currentLoadoutState;
  private static AdventureDungeonCrawlDisplay m_instance;
  private bool m_isPVPDR;
  private CollectionDeck m_realDuelSeedDeck;
  private bool m_seedDeckCreateRequested;
  private long m_rewardNoticeId;

  public static AdventureDungeonCrawlDisplay Get() => AdventureDungeonCrawlDisplay.m_instance;

  private void Awake() => AdventureDungeonCrawlDisplay.m_instance = this;

  private void Start()
  {
    CollectionManager.Get().RegisterDeckCreatedListener(new CollectionManager.DelOnDeckCreated(this.OnDeckCreated));
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.m_isPVPDR = SceneMgr.Get().IsInDuelsMode();
  }

  public void StartRun(DungeonCrawlServices services)
  {
    this.m_dungeonCrawlData = services.DungeonCrawlData;
    this.m_subsceneController = services.SubsceneController;
    this.m_assetLoadingHelper = services.AssetLoadingHelper;
    services.AssetLoadingHelper.AssetLoadingComplete += new EventHandler(this.OnSubSceneLoaded);
    this.m_subsceneController.TransitionComplete += new EventHandler(this.OnSubSceneTransitionComplete);
    AdventureDbId selectedAdv = this.m_dungeonCrawlData.GetSelectedAdventure();
    AdventureModeDbId selectedMode = this.m_dungeonCrawlData.GetSelectedMode();
    AdventureDataDbfRecord adventureDataRecord1 = GameUtils.GetAdventureDataRecord((int) selectedAdv, (int) selectedMode);
    this.m_playerHeroData = new AdventureDungeonCrawlDisplay.PlayerHeroData(this.m_dungeonCrawlData);
    this.m_playerHeroData.OnHeroDataChanged += (AdventureDungeonCrawlDisplay.PlayerHeroData.DataChangedEventHandler) (() => this.m_playMat.SetPlayerHeroDbId(this.m_playerHeroData.HeroCardDbId));
    this.m_AdventureTitle.Text = (string) adventureDataRecord1.Name;
    AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey = (GameSaveKeyId) adventureDataRecord1.GameSaveDataServerKey;
    AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey = (GameSaveKeyId) adventureDataRecord1.GameSaveDataClientKey;
    if (AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey <= ~GameSaveKeyId.INVALID)
      Debug.LogErrorFormat("Adventure {0} Mode {1} has no GameSaveDataKey set! This mode does not work without defining GAME_SAVE_DATA_SERVER_KEY in ADVENTURE.dbi!", (object) selectedAdv, (object) selectedMode);
    if (AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey <= ~GameSaveKeyId.INVALID)
      Debug.LogErrorFormat("Adventure {0} Mode {1} has no GameSaveDataKey set! This mode does not work without defining GAME_SAVE_DATA_CLIENT_KEY in ADVENTURE.dbi!", (object) selectedAdv, (object) selectedMode);
    if (AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey == AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey)
      Debug.LogErrorFormat("Adventure {0} Mode {1} has an equal GameSaveDataKey for Client and Server. These keys are not allowed to be equal!", (object) selectedAdv, (object) selectedMode);
    this.m_bossCardBackId = adventureDataRecord1.BossCardBack;
    if (this.m_bossCardBackId == 0)
      this.m_bossCardBackId = 0;
    this.m_saveHeroDataUsingHeroId = adventureDataRecord1.DungeonCrawlSaveHeroUsingHeroDbId;
    if (this.m_isPVPDR && PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().IsSessionRolledOver)
    {
      PvpdrSeasonDbfRecord record = GameDbf.PvpdrSeason.GetRecord(PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().Season);
      if (record != null && record.AdventureRecord != null)
      {
        AdventureDataDbfRecord adventureDataRecord2 = GameUtils.GetAdventureDataRecord(record.AdventureId, (int) selectedMode);
        if (adventureDataRecord2 != null)
          this.m_saveHeroDataUsingHeroId = adventureDataRecord2.DungeonCrawlSaveHeroUsingHeroDbId;
      }
    }
    List<ScenarioDbfRecord> records = GameDbf.Scenario.GetRecords((Predicate<ScenarioDbfRecord>) (r => (AdventureDbId) r.AdventureId == selectedAdv && (AdventureModeDbId) r.ModeId == selectedMode));
    if (records == null || records.Count < 1)
      Log.Adventures.PrintError("No Scenarios found for Adventure {0} and Mode {1}!", (object) selectedAdv, (object) selectedMode);
    else if (records.Count == 1)
    {
      ScenarioDbfRecord scenarioDbfRecord = records[0];
      this.m_dungeonCrawlData.SetMission((ScenarioDbId) scenarioDbfRecord.ID, false);
      Log.Adventures.Print("Owns wing for this Dungeon Run? {0}", (object) AdventureProgressMgr.Get().OwnsWing(scenarioDbfRecord.WingId));
    }
    else if (this.m_dungeonCrawlData.GetMission() == ScenarioDbId.INVALID)
    {
      Log.Adventures.Print("No selectedScenarioId currently set - this should come with the GameSaveData.");
    }
    else
    {
      ScenarioDbfRecord scenarioDbfRecord = records.Find((Predicate<ScenarioDbfRecord>) (x => (ScenarioDbId) x.ID == this.m_dungeonCrawlData.GetMission()));
      if (scenarioDbfRecord == null)
        Log.Adventures.PrintError("No matching Scenario for this Adventure has been set in AdventureConfig! AdventureConfig's mission: {0}", (object) this.m_dungeonCrawlData.GetMission());
      else
        Log.Adventures.Print("Owns wing for this Dungeon Run? {0}", (object) AdventureProgressMgr.Get().OwnsWing(scenarioDbfRecord.WingId));
    }
    this.m_shouldSkipHeroSelect = adventureDataRecord1.DungeonCrawlSkipHeroSelect;
    this.m_mustPickShrine = adventureDataRecord1.DungeonCrawlMustPickShrine;
    this.m_mustSelectChapter = adventureDataRecord1.DungeonCrawlSelectChapter;
    this.m_anomalyModeCardDbId = (long) adventureDataRecord1.AnomalyModeDefaultCardId;
    this.m_assetLoadingHelper.AddAssetToLoad();
    this.m_dungeonCrawlPlayMatReference.RegisterReadyListener<AdventureDungeonCrawlPlayMat>(new Action<AdventureDungeonCrawlPlayMat>(this.OnPlayMatReady));
    bool retireButtonSupported = adventureDataRecord1.DungeonCrawlIsRetireSupported;
    this.m_assetLoadingHelper.AddAssetToLoad();
    this.m_retireButtonReference.RegisterReadyListener<Widget>((Action<Widget>) (w =>
    {
      w.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
      {
        if (!(eventName == "Button_Framed_Clicked") || !retireButtonSupported)
          return;
        this.m_retireButton.SetActive(false);
        DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_RETIRE_CONFIRMATION_HEADER"),
          m_text = GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_RETIRE_CONFIRMATION_BODY"),
          m_showAlertIcon = true,
          m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_responseCallback = !this.m_isPVPDR ? new AlertPopup.ResponseCallback(this.OnRetirePopupResponse) : new AlertPopup.ResponseCallback(this.OnPVPDRRetirePopupResponse)
        });
      }));
      this.m_retireButton = w.gameObject;
      this.m_retireButton.SetActive(false);
      w.RegisterDoneChangingStatesListener((Action<object>) (_ => this.m_assetLoadingHelper.AssetLoadCompleted()), (object) null, true, true);
    }));
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_dungeonCrawlDeckSelectWidgetReference.RegisterReadyListener<DungeonCrawlDeckSelect>(new Action<DungeonCrawlDeckSelect>(this.OnDungeonCrawlDeckTrayReady));
    if ((UnityEngine.Object) this.m_dungeonCrawlDeckTray != (UnityEngine.Object) null && (UnityEngine.Object) this.m_dungeonCrawlDeckTray.m_deckBigCard != (UnityEngine.Object) null)
      this.m_dungeonCrawlDeckTray.m_deckBigCard.OnBigCardShown += new DeckBigCard.OnBigCardShownHandler(this.OnDeckTrayBigCardShown);
    this.EnableBackButton(true);
    if (this.m_isPVPDR)
      Navigation.PushUnique(new Navigation.NavigateBackHandler(PvPDungeonRunScene.Get().NavigateBackFromPlaymat));
    else
      Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureDungeonCrawlDisplay.OnNavigateBack));
    this.m_BackButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonPress));
    if ((UnityEngine.Object) this.m_ShowDeckButton != (UnityEngine.Object) null)
      this.m_ShowDeckButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnShowDeckButtonPress));
    this.DisableBackButtonIfInDemoMode();
    this.RequestOrLoadCachedGameSaveData();
    this.SetDungeonCrawlDisplayVisualStyle();
  }

  public void EnablePlayButton()
  {
    if (!((UnityEngine.Object) this.m_playMat != (UnityEngine.Object) null))
      return;
    this.m_playMat.PlayButton.Enable();
  }

  public void DisablePlayButton()
  {
    if (!((UnityEngine.Object) this.m_playMat != (UnityEngine.Object) null) || !this.m_playMat.PlayButton.IsEnabled())
      return;
    this.m_playMat.PlayButton.Disable();
  }

  public void EnableBackButton(bool enabled)
  {
    if (!((UnityEngine.Object) this.m_BackButton != (UnityEngine.Object) null) || this.m_BackButton.IsEnabled() == enabled)
      return;
    this.m_BackButton.SetEnabled(enabled);
    this.m_BackButton.Flip(enabled);
  }

  private void OnDeckTrayBigCardShown(Actor shownActor, EntityDef entityDef)
  {
    if ((UnityEngine.Object) shownActor == (UnityEngine.Object) null || entityDef == null || this.m_anomalyModeCardDbId != (long) GameUtils.TranslateCardIdToDbId(entityDef.GetCardId()))
      return;
    HighlightRender componentInChildren = shownActor.GetComponentInChildren<HighlightRender>();
    MeshRenderer meshRenderer = (UnityEngine.Object) componentInChildren != (UnityEngine.Object) null ? componentInChildren.GetComponent<MeshRenderer>() : (MeshRenderer) null;
    if (!((UnityEngine.Object) meshRenderer != (UnityEngine.Object) null) || !((UnityEngine.Object) this.m_anomalyModeCardHighlightMaterial != (UnityEngine.Object) null))
      return;
    RendererExtension.SetSharedMaterial((Renderer) meshRenderer, this.m_anomalyModeCardHighlightMaterial);
    meshRenderer.enabled = true;
  }

  private void OnPlayMatPlayButtonReady(PlayButton playButton)
  {
    if ((UnityEngine.Object) playButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "PlayButtonReference is null, or does not have a PlayButton component on it!");
    }
    else
    {
      playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonPress));
      Widget componentInParent = (Widget) playButton.GetComponentInParent<WidgetTemplate>();
      if ((UnityEngine.Object) componentInParent != (UnityEngine.Object) null)
      {
        componentInParent.RegisterDoneChangingStatesListener((Action<object>) (_ => this.m_assetLoadingHelper.AssetLoadCompleted()), (object) null, true, true);
      }
      else
      {
        Error.AddDevWarning("UI Error!", "Could not find PlayMat PlayButton WidgetTemplate!");
        this.m_assetLoadingHelper.AssetLoadCompleted();
      }
    }
  }

  private void OnDungeonCrawlDeckTrayReady(DungeonCrawlDeckSelect deckSelect)
  {
    this.m_dungeonCrawlDeckSelect = deckSelect;
    if ((UnityEngine.Object) this.m_dungeonCrawlDeckSelect == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "Could not find AdventureDungeonCrawlDeckTray in the AdventureDeckSelectWidget.");
    else if ((UnityEngine.Object) this.m_dungeonCrawlDeckSelect == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "Could not find SlidingTray in the AdventureDeckSelectWidget.");
    }
    else
    {
      deckSelect.playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonPress));
      deckSelect.heroDetails.AddHeroPowerListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.ShowBigCard(this.m_heroPowerBigCard, this.m_currentHeroPowerFullDef, this.m_HeroPowerBigCardBone)));
      deckSelect.heroDetails.AddHeroPowerListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => BigCardHelper.HideBigCard(this.m_heroPowerBigCard)));
      if (!((UnityEngine.Object) deckSelect.deckTray != (UnityEngine.Object) null) || !((UnityEngine.Object) deckSelect.deckTray.m_deckBigCard != (UnityEngine.Object) null))
        return;
      deckSelect.deckTray.m_deckBigCard.OnBigCardShown += new DeckBigCard.OnBigCardShownHandler(this.OnDeckTrayBigCardShown);
    }
  }

  private void OnPlayMatReady(AdventureDungeonCrawlPlayMat playMat)
  {
    if ((UnityEngine.Object) playMat == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "m_dungeonCrawlPlayMatReference is null, or does not have a AdventureDungeonCrawlPlayMat component on it!");
    }
    else
    {
      this.m_playMat = playMat;
      this.m_playMat.SetCardBack(this.m_bossCardBackId);
      this.m_BossPowerBone = this.m_playMat.m_BossPowerBone;
      this.m_assetLoadingHelper.AddAssetToLoad();
      this.m_playMat.m_PlayButtonReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayMatPlayButtonReady));
      this.LoadInitialAssets();
      Widget component = (Widget) playMat.GetComponent<WidgetTemplate>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        component.RegisterDoneChangingStatesListener((Action<object>) (_ => this.m_assetLoadingHelper.AssetLoadCompleted()), (object) null, true, true);
      }
      else
      {
        Error.AddDevWarning("UI Error!", "Could not find PlayMat WidgetTemplate!");
        this.m_assetLoadingHelper.AssetLoadCompleted();
      }
    }
  }

  private void Update()
  {
    if (this.m_dungeonCrawlData == null || !this.m_dungeonCrawlData.IsDevMode || !InputCollection.GetKeyDown(KeyCode.Z) || (UnityEngine.Object) this.m_playMat == (UnityEngine.Object) null)
      return;
    if (this.m_playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_BOSS_GRAVEYARD)
    {
      this.m_playMat.ShowNextBoss(this.GetPlayButtonTextForNextMission());
    }
    else
    {
      if (this.m_playMat.GetPlayMatState() != AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_NEXT_BOSS)
        return;
      this.ShowRunEnd(this.m_defeatedBossIds, this.m_bossWhoDefeatedMeId);
    }
  }

  private void OnDestroy()
  {
    AdventureDungeonCrawlDisplay.m_instance = (AdventureDungeonCrawlDisplay) null;
    this.m_currentBossHeroPowerFullDef?.Dispose();
    this.m_currentHeroPowerFullDef?.Dispose();
    if ((UnityEngine.Object) this.m_playMat != (UnityEngine.Object) null)
      this.m_playMat.HideBossHeroPowerTooltip(true);
    if ((UnityEngine.Object) this.m_dungeonCrawlDeckTray != (UnityEngine.Object) null && (UnityEngine.Object) this.m_dungeonCrawlDeckTray.m_deckBigCard != (UnityEngine.Object) null)
      this.m_dungeonCrawlDeckTray.m_deckBigCard.OnBigCardShown -= new DeckBigCard.OnBigCardShownHandler(this.OnDeckTrayBigCardShown);
    if ((UnityEngine.Object) this.m_dungeonCrawlDeckSelect != (UnityEngine.Object) null && (UnityEngine.Object) this.m_dungeonCrawlDeckSelect.deckTray != (UnityEngine.Object) null && (UnityEngine.Object) this.m_dungeonCrawlDeckSelect.deckTray.m_deckBigCard != (UnityEngine.Object) null)
      this.m_dungeonCrawlDeckSelect.deckTray.m_deckBigCard.OnBigCardShown -= new DeckBigCard.OnBigCardShownHandler(this.OnDeckTrayBigCardShown);
    GameMgr.Get()?.UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
  }

  private void OnBossActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_bossActor = AdventureDungeonCrawlDisplay.OnActorLoaded((string) assetRef, go, this.m_playMat.m_nextBossFaceBone.gameObject, true);
    if ((UnityEngine.Object) this.m_bossActor != (UnityEngine.Object) null)
    {
      PegUIElement pegUiElement = this.m_bossActor.GetCollider().gameObject.AddComponent<PegUIElement>();
      pegUiElement.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e =>
      {
        this.m_bossActor.SetActorState(ActorStateType.CARD_MOUSE_OVER);
        this.ShowBigCard(this.m_bossPowerBigCard, this.m_currentBossHeroPowerFullDef, this.m_HeroPowerBigCardBone);
        this.m_bossHeroPowerHideCoroutine = this.StartCoroutine(this.HideBossHeroPowerTooltipAfterHover());
      }));
      pegUiElement.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e =>
      {
        this.m_bossActor.SetActorState(ActorStateType.CARD_IDLE);
        BigCardHelper.HideBigCard(this.m_bossPowerBigCard);
        if (this.m_bossHeroPowerHideCoroutine == null)
          return;
        this.StopCoroutine(this.m_bossHeroPowerHideCoroutine);
      }));
    }
    this.m_playMat.SetBossActor(this.m_bossActor);
    this.m_assetLoadingHelper.AssetLoadCompleted();
  }

  private void LoadInitialAssets()
  {
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) this.m_dungeonCrawlData.GetSelectedAdventure(), (int) this.m_dungeonCrawlData.GetSelectedMode());
    if (adventureDataRecord == null)
    {
      Log.Adventures.PrintError("Tried to load assets but data record not found!");
    }
    else
    {
      IAssetLoader assetLoader = AssetLoader.Get();
      this.m_assetLoadingHelper.AddAssetToLoad();
      assetLoader.InstantiatePrefab((AssetReference) adventureDataRecord.DungeonCrawlBossCardPrefab, new PrefabCallback<GameObject>(this.OnBossActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
      this.m_assetLoadingHelper.AddAssetToLoad();
      assetLoader.InstantiatePrefab((AssetReference) "History_HeroPower_Opponent.prefab:a99d23d6e8630f94b96a8e096fffb16f", new PrefabCallback<GameObject>(this.OnBossPowerBigCardLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
      this.m_assetLoadingHelper.AddAssetToLoad();
      assetLoader.InstantiatePrefab((AssetReference) "Card_Dungeon_Play_Hero.prefab:183cb9cc59697844e911776ec349fe5e", new PrefabCallback<GameObject>(this.OnHeroActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
      this.m_assetLoadingHelper.AddAssetToLoad();
      assetLoader.InstantiatePrefab((AssetReference) "History_HeroPower.prefab:e73edf8ccea2b11429093f7a448eef53", new PrefabCallback<GameObject>(this.OnHeroPowerBigCardLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
      this.m_assetLoadingHelper.AddAssetToLoad();
      assetLoader.InstantiatePrefab((AssetReference) "Card_Play_HeroPower.prefab:a3794839abb947146903a26be13e09af", new PrefabCallback<GameObject>(this.OnHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    }
  }

  private IEnumerator HideBossHeroPowerTooltipAfterHover()
  {
    float timer = 0.0f;
    while ((double) timer < (double) this.m_RolloverTimeToHideBossHeroPowerTooltip)
    {
      timer += Time.unscaledDeltaTime;
      yield return (object) new WaitForEndOfFrame();
    }
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSS_HERO_POWER_TUTORIAL_PROGRESS, out num);
    if (num == 1L)
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSS_HERO_POWER_TUTORIAL_PROGRESS, new long[1]
      {
        2L
      }));
    this.m_playMat.HideBossHeroPowerTooltip();
  }

  private void OnBossPowerBigCardLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_bossPowerBigCard = AdventureDungeonCrawlDisplay.OnActorLoaded((string) assetRef, go, this.m_BossPowerBone);
    if ((UnityEngine.Object) this.m_bossPowerBigCard != (UnityEngine.Object) null)
      this.m_bossPowerBigCard.TurnOffCollider();
    this.m_assetLoadingHelper.AssetLoadCompleted();
  }

  private void OnHeroPowerBigCardLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_heroPowerBigCard = AdventureDungeonCrawlDisplay.OnActorLoaded((string) assetRef, go, this.m_HeroPowerBigCardBone);
    if ((UnityEngine.Object) this.m_heroPowerBigCard != (UnityEngine.Object) null)
      this.m_heroPowerBigCard.TurnOffCollider();
    this.m_assetLoadingHelper.AssetLoadCompleted();
  }

  private void RequestOrLoadCachedGameSaveData()
  {
    if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY)
      GameSaveDataManager.Get().ClearLocalData(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey);
    this.StartCoroutine(this.InitializeFromGameSaveDataWhenReady());
    if (!GameSaveDataManager.Get().IsDataReady(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey))
      GameSaveDataManager.Get().Request(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, new GameSaveDataManager.OnRequestDataResponseDelegate(this.OnRequestGameSaveDataServerResponse));
    else
      this.m_hasReceivedGameSaveDataServerKeyResponse = true;
    if (!GameSaveDataManager.Get().IsDataReady(AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey))
      GameSaveDataManager.Get().Request(AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey, new GameSaveDataManager.OnRequestDataResponseDelegate(this.OnRequestGameSaveDataClientResponse));
    else
      this.m_hasReceivedGameSaveDataClientKeyResponse = true;
  }

  private void OnRequestGameSaveDataServerResponse(bool success)
  {
    if (!success)
    {
      Debug.LogError((object) "OnRequestGameSaveDataResponse: Error requesting game save data for current adventure.");
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    }
    else
      this.m_hasReceivedGameSaveDataServerKeyResponse = true;
  }

  private void OnRequestGameSaveDataClientResponse(bool success)
  {
    if (!success)
    {
      Debug.LogError((object) "OnRequestGameSaveDataResponse: Error requesting game save data for current adventure.");
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    }
    else
      this.m_hasReceivedGameSaveDataClientKeyResponse = true;
  }

  private IEnumerator InitializeFromGameSaveDataWhenReady()
  {
    while ((UnityEngine.Object) this.m_playMat == (UnityEngine.Object) null || !this.m_playMat.IsReady())
    {
      Log.Adventures.Print("Waiting for Play Mat to be initialized before handling new Game Save Data.");
      yield return (object) null;
    }
    while (this.m_playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE)
    {
      Log.Adventures.Print("Waiting for Play Mat to be done transitioning before handling new Game Save Data.");
      yield return (object) null;
    }
    while (!this.m_hasReceivedGameSaveDataClientKeyResponse || !this.m_hasReceivedGameSaveDataServerKeyResponse)
      yield return (object) null;
    DungeonCrawlUtil.MigrateDungeonCrawlSubkeys(AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey, AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey);
    while ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null || (UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null || (UnityEngine.Object) this.m_heroPowerBigCard == (UnityEngine.Object) null)
      yield return (object) null;
    this.InitializeFromGameSaveData();
  }

  private bool IsScenarioValidForAdventureAndMode(ScenarioDbId selectedScenario)
  {
    if (AdventureUtils.IsMissionValidForAdventureMode(this.m_dungeonCrawlData.GetSelectedAdventure(), this.m_dungeonCrawlData.GetSelectedMode(), selectedScenario))
      return true;
    Debug.LogErrorFormat("Scenario {0} is not a part of Adventure {1} and mode {2}! Something is probably wrong.", (object) selectedScenario, (object) this.m_dungeonCrawlData.GetSelectedAdventure(), (object) this.m_dungeonCrawlData.GetSelectedMode());
    return false;
  }

  private void InitializeFromGameSaveData()
  {
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) this.m_dungeonCrawlData.GetSelectedAdventure(), (int) this.m_dungeonCrawlData.GetSelectedMode());
    this.m_playerHeroData.UpdateHeroDataFromClass(TAG_CLASS.INVALID);
    List<long> values1 = (List<long>) null;
    List<CardWithPremiumStatus> deckCardListPremium = (List<CardWithPremiumStatus>) null;
    List<long> values2 = (List<long>) null;
    List<long> values3 = (List<long>) null;
    List<long> values4 = (List<long>) null;
    List<long> values5 = (List<long>) null;
    List<long> values6 = (List<long>) null;
    long num1 = 0;
    long num2 = 0;
    if (GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSSES_DEFEATED, out this.m_defeatedBossIds))
      this.m_numBossesDefeated = this.m_defeatedBossIds.Count;
    List<long> values7 = (List<long>) null;
    List<long> values8 = (List<long>) null;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CARD_LIST, out values1);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSS_LOST_TO, out this.m_bossWhoDefeatedMeId);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_A, out values3);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_B, out values4);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_C, out values5);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_TREASURE_OPTION, out values2);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_SHRINE_OPTIONS, out this.m_shrineOptions);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEXT_BOSS_FIGHT, out values6);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_LOOT, out num1);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_TREASURE, out num2);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_NEXT_BOSS_HEALTH, out this.m_nextBossHealth);
    long num3;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_HEALTH, out num3);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_CARDS_ADDED_TO_DECK_MAP, out this.m_cardsAddedToDeckMap);
    long num4;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_SHRINE, out num4);
    long num5;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_SCENARIO_ID, out num5);
    long num6;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_SCENARIO_ID, out num6);
    long num7;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_SCENARIO_OVERRIDE, out num7);
    long num8;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_LOADOUT_TREASURE_ID, out num8);
    long num9;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_POWER, out num9);
    long num10;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_POWER, out num10);
    long num11;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_ANOMALY_MODE, out num11);
    long num12;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_ANOMALY_MODE, out num12);
    long num13;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_DECK, out num13);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_ENCHANTMENT_INDICES, out values7);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_ENCHANTMENTS, out values8);
    long num14;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_CURRENT_ANOMALY_MODE_CARD, out num14);
    long num15;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_ANOMALY_MODE_CARD_PREVIEW, out num15);
    long num16;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_LATEST_DUNGEON_RUN_COMPLETE, out num16);
    long heroClass;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_DECK_CLASS, out heroClass);
    long num17;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CLASS, out num17);
    long heroCardDbId;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HERO_CARD_DB_ID, out heroCardDbId);
    long num18;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CARD_DB_ID, out num18);
    long num19;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_RUN_RETIRED, out num19);
    if (this.m_saveHeroDataUsingHeroId)
      this.m_playerHeroData.UpdateHeroDataFromHeroCardDbId((int) heroCardDbId);
    else
      this.m_playerHeroData.UpdateHeroDataFromClass((TAG_CLASS) heroClass);
    this.m_selectedShrineIndex = (int) num4;
    if (values1 != null)
      deckCardListPremium = CardWithPremiumStatus.ConvertList(values1);
    this.m_isRunRetired = num19 > 0L;
    this.m_isRunActive = DungeonCrawlUtil.IsDungeonRunActive(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey);
    this.m_hasSeenLatestDungeonRunComplete = num16 > 0L;
    bool useLoadoutOfActiveRun = this.m_isRunActive || this.ShouldShowRunCompletedScreen();
    RuneType[] runeOrder = (RuneType[]) null;
    if (this.m_isPVPDR)
    {
      CollectionDeck duelsDeck = CollectionManager.Get().GetDuelsDeck();
      this.m_isRunActive = PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().IsSessionActive;
      if (duelsDeck != null)
      {
        runeOrder = duelsDeck.GetRuneOrder();
        if (this.m_isRunActive || values1 != null && values1.Count > duelsDeck.GetTotalCardCount())
        {
          List<CardWithPremiumStatus> withPremiumStatus = duelsDeck.GetCardsWithPremiumStatus();
          for (int i = 0; i < deckCardListPremium.Count; i++)
          {
            int index = withPremiumStatus.FindIndex((Predicate<CardWithPremiumStatus>) (r => r.cardId == deckCardListPremium[i].cardId));
            if (index >= 0)
            {
              deckCardListPremium[i].premium = withPremiumStatus[index].premium;
              withPremiumStatus.RemoveAt(index);
            }
          }
        }
        else if (!this.m_seedDeckCreateRequested)
        {
          this.m_realDuelSeedDeck = duelsDeck;
          deckCardListPremium = duelsDeck.GetCardsWithPremiumStatus();
          TAG_CLASS tagClass = duelsDeck.GetClass();
          this.m_dungeonCrawlData.SelectedHeroCardDbId = (long) AdventureUtils.GetHeroCardDbIdFromClassForDungeonCrawl(this.m_dungeonCrawlData, tagClass);
          this.m_playerHeroData.UpdateHeroDataFromClass(tagClass);
        }
      }
    }
    this.m_dungeonCrawlData.SelectedLoadoutTreasureDbId = num8;
    this.m_dungeonCrawlData.SelectedHeroPowerDbId = num9;
    this.m_dungeonCrawlData.SelectedDeckId = num13;
    if (this.m_saveHeroDataUsingHeroId && num18 != 0L)
    {
      this.m_dungeonCrawlData.SelectedHeroCardDbId = num18;
    }
    else
    {
      TAG_CLASS cardClass = (TAG_CLASS) num17;
      if (cardClass != TAG_CLASS.INVALID)
        this.m_dungeonCrawlData.SelectedHeroCardDbId = (long) GameUtils.TranslateCardIdToDbId(AdventureUtils.GetHeroCardIdFromClassForDungeonCrawl(this.m_dungeonCrawlData, cardClass));
    }
    ScenarioDbId scenarioDbId1 = (ScenarioDbId) num7;
    if (scenarioDbId1 != ScenarioDbId.INVALID && !this.IsScenarioValidForAdventureAndMode(scenarioDbId1))
      scenarioDbId1 = ScenarioDbId.INVALID;
    Log.Adventures.Print("Scenario Override set to {0}!", (object) scenarioDbId1);
    this.m_dungeonCrawlData.SetMissionOverride(scenarioDbId1);
    ScenarioDbId scenarioDbId2 = useLoadoutOfActiveRun ? (ScenarioDbId) num6 : (ScenarioDbId) num5;
    if (scenarioDbId2 != ScenarioDbId.INVALID && this.IsScenarioValidForAdventureAndMode(scenarioDbId2))
      this.m_dungeonCrawlData.SetMission(scenarioDbId2);
    bool flag1 = false;
    if (!useLoadoutOfActiveRun)
    {
      flag1 = this.m_dungeonCrawlData.HasValidLoadoutForSelectedAdventure();
      if (!flag1)
        AdventureDungeonCrawlDisplay.ResetDungeonCrawlSelections(this.m_dungeonCrawlData);
    }
    this.m_playMat.m_paperControllerReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnPlayMatPaperControllerReady));
    this.m_playMat.m_paperControllerReference_phone.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnPlayMatPaperControllerReady));
    if (useLoadoutOfActiveRun)
      this.m_dungeonCrawlData.AnomalyModeActivated = num12 > 0L;
    else if (flag1)
      this.m_dungeonCrawlData.AnomalyModeActivated = num11 > 0L;
    this.m_heroHealth = !useLoadoutOfActiveRun ? 0L : num3;
    if (this.HandleDemoModeReset())
      return;
    long num20 = useLoadoutOfActiveRun ? num14 : num15;
    if (num20 > 0L)
      this.m_anomalyModeCardDbId = num20;
    if (this.m_isRunActive && deckCardListPremium != null)
    {
      if (num1 != 0L)
      {
        List<long>[] longListArray = new List<long>[3]
        {
          values3,
          values4,
          values5
        };
        int index1 = (int) num1 - 1;
        if (index1 >= longListArray.Length || longListArray[index1] == null)
        {
          Log.Adventures.PrintError("Attempting to add Loot choice {0} to the deck list, but there is not corresponding list of Loot!", (object) index1);
        }
        else
        {
          List<long> longList = longListArray[index1];
          for (int index2 = 1; index2 < longList.Count; ++index2)
            deckCardListPremium.Add(new CardWithPremiumStatus(longList[index2], TAG_PREMIUM.NORMAL));
        }
      }
      if (num2 != 0L && values2 != null)
      {
        int index = (int) num2 - 1;
        if (values2.Count <= index)
          Log.Adventures.PrintError("Attempting to add Treasure choice {0} to the deck list, but treasureLootOptions only has {1} options!", (object) index, (object) values2.Count);
        else
          deckCardListPremium.Add(new CardWithPremiumStatus(values2[index], TAG_PREMIUM.NORMAL));
      }
    }
    ScenarioDbId mission = this.m_dungeonCrawlData.GetMission();
    int index3 = 0;
    WingDbfRecord recordFromMissionId = GameUtils.GetWingRecordFromMissionId((int) mission);
    this.m_numBossesInRun = this.m_dungeonCrawlData.GetAdventureBossesInRun(recordFromMissionId);
    if (recordFromMissionId != null)
    {
      index3 = Mathf.Max(0, GameUtils.GetSortedWingUnlockIndex(recordFromMissionId));
      this.m_plotTwistCardDbId = (long) recordFromMissionId.PlotTwistCardId;
    }
    int dbId1 = 0;
    if (values6 != null && values6.Count > index3 && !this.m_isRunRetired)
      dbId1 = (int) values6[index3];
    this.m_nextBossCardId = dbId1 == 0 ? GameUtils.GetMissionHeroCardId((int) mission) : GameUtils.TranslateDbIdToCardId(dbId1);
    if (this.m_nextBossCardId == null)
    {
      Log.Adventures.PrintWarning("AdventureDungeonCrawlDisplay.OnGameSaveDataResponse() - No cardId for boss dbId {0}!", (object) dbId1);
    }
    else
    {
      this.m_assetLoadingHelper.AddAssetToLoad();
      DefLoader.Get().LoadFullDef(this.m_nextBossCardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnBossFullDefLoaded));
    }
    long dbId2 = useLoadoutOfActiveRun ? num10 : this.m_dungeonCrawlData.SelectedHeroPowerDbId;
    if (dbId2 != 0L)
      this.SetHeroPower(GameUtils.TranslateDbIdToCardId((int) dbId2));
    if (this.m_isRunActive || this.ShouldShowRunCompletedScreen())
      AdventureDungeonCrawlDisplay.s_shouldShowWelcomeBanner = false;
    this.InitializePlayMat();
    if (this.ShouldShowRunCompletedScreen())
    {
      if (this.m_isPVPDR)
        this.ShowDuelsEndRun();
      else
        this.ShowRunEnd(this.m_defeatedBossIds, this.m_bossWhoDefeatedMeId);
      this.SetUpBossKillCounter(this.m_playerHeroData.HeroCardDbId);
      this.SetUpDeckList(deckCardListPremium, useLoadoutOfActiveRun, deckCardIndices: values7, deckCardEnchantments: values8);
      this.SetUpHeroPortrait(this.m_playerHeroData);
      if (!SceneMgr.Get().IsInDuelsMode())
        this.SetUpPhoneRunCompleteScreen();
    }
    else if (!this.m_isRunActive)
    {
      if (!this.m_dungeonCrawlData.HeroIsSelectedBeforeDungeonCrawlScreenForSelectedAdventure())
        this.TryShowWelcomeBanner();
      bool flag2 = !this.m_isPVPDR;
      if (this.m_mustPickShrine)
      {
        if (this.m_shrineOptions == null && this.m_dungeonCrawlData.GetSelectedAdventure() == AdventureDbId.TRL)
          this.m_shrineOptions = this.GetDefaultStartingShrineOptions_TRL();
        if (this.m_shrineOptions != null)
        {
          if (this.m_selectedShrineIndex == 0)
          {
            this.m_playerHeroData.UpdateHeroDataFromClass(TAG_CLASS.NEUTRAL);
            this.SetPlaymatStateForShrineSelection(this.m_shrineOptions);
            flag2 = false;
          }
          else
          {
            long shrineOption = this.m_shrineOptions[this.m_selectedShrineIndex - 1];
            TAG_CLASS classFromShrine = this.GetClassFromShrine(shrineOption);
            this.m_playerHeroData.UpdateHeroDataFromClass(classFromShrine);
            this.SetUpDeckListFromShrine(shrineOption, false);
            if (this.m_dungeonCrawlData.SelectedHeroCardDbId == 0L)
              this.m_dungeonCrawlData.SelectedHeroCardDbId = (long) AdventureUtils.GetHeroCardDbIdFromClassForDungeonCrawl(this.m_dungeonCrawlData, classFromShrine);
          }
          this.SetUpHeroPortrait(this.m_playerHeroData);
        }
        this.SetUpBossKillCounter((int) this.m_dungeonCrawlData.SelectedHeroCardDbId);
      }
      else
      {
        if (this.m_dungeonCrawlData.HeroIsSelectedBeforeDungeonCrawlScreenForSelectedAdventure() && this.m_dungeonCrawlData.SelectedHeroCardDbId != 0L)
        {
          this.m_playerHeroData.UpdateHeroDataFromHeroCardDbId((int) this.m_dungeonCrawlData.SelectedHeroCardDbId);
          this.SetUpHeroPortrait(this.m_playerHeroData);
          this.SetUpBossKillCounter(this.m_playerHeroData.HeroCardDbId);
        }
        if (this.m_dungeonCrawlData.SelectableLoadoutTreasuresExist() | this.m_dungeonCrawlData.SelectableHeroPowersAndDecksExist())
        {
          if (!this.m_dungeonCrawlData.HasValidLoadoutForSelectedAdventure())
          {
            this.m_currentLoadoutState = AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.INVALID;
            this.GoToNextLoadoutState();
            flag2 = false;
            if (this.m_plotTwistCardDbId != 0L || this.m_anomalyModeCardDbId != 0L && this.m_dungeonCrawlData.AnomalyModeActivated)
              this.SetUpDeckList((List<CardWithPremiumStatus>) null, useLoadoutOfActiveRun);
          }
          else if (this.m_isPVPDR)
          {
            this.SetUpDeckList(deckCardListPremium, useLoadoutOfActiveRun, runeOrder: runeOrder);
            this.m_playMat.ShowPVPDRActiveRun(this.GetPlayButtonTextForNextMission());
            this.m_BackButton.SetText("GLOBAL_LEAVE");
          }
          else if ((this.m_dungeonCrawlDeck == null ? 0 : (this.m_dungeonCrawlDeck.GetTotalCardCount() > 0 ? 1 : 0)) == 0 && this.m_dungeonCrawlData.SelectedDeckId != 0L && this.m_playerHeroData.HeroClasses[0] != TAG_CLASS.INVALID)
            this.SetUpDeckList(CardWithPremiumStatus.ConvertList(CollectionManager.Get().LoadDeckFromDBF((int) this.m_dungeonCrawlData.SelectedDeckId, out string _, out string _)), useLoadoutOfActiveRun);
        }
        else if (adventureDataRecord.DungeonCrawlDefaultToDeckFromUpcomingScenario)
          this.SetUpDeckListFromScenario(this.m_dungeonCrawlData.GetMission(), useLoadoutOfActiveRun);
      }
      if (flag2)
      {
        this.m_playMat.SetUpDefeatedBosses((List<long>) null, this.m_numBossesInRun);
        this.m_playMat.SetShouldShowBossHeroPowerTooltip(this.ShouldShowBossHeroPowerTutorial());
        this.m_assetLoadingHelper.AddAssetToLoad();
        this.m_playMat.SetUpCardBacks(this.m_numBossesInRun - 1, new AdventureDungeonCrawlPlayMat.AssetLoadCompletedCallback(this.m_assetLoadingHelper.AssetLoadCompleted));
        string playButtonText = "GLUE_CHOOSE";
        if (this.m_shouldSkipHeroSelect || this.m_dungeonCrawlData.HeroIsSelectedBeforeDungeonCrawlScreenForSelectedAdventure())
          playButtonText = this.GetPlayButtonTextForNextMission();
        this.m_playMat.ShowNextBoss(playButtonText);
        if (this.m_mustSelectChapter)
          this.m_BackButton.SetText("GLOBAL_LEAVE");
      }
      this.SetUpPhoneNewRunScreen();
    }
    else
    {
      this.SetUpBossKillCounter(this.m_playerHeroData.HeroCardDbId);
      if (adventureDataRecord.DungeonCrawlDefaultToDeckFromUpcomingScenario && (deckCardListPremium == null || deckCardListPremium.Count == 0))
      {
        if (values7 != null && values7.Count > 0 || values8 != null && values8.Count > 0)
          Debug.LogWarning((object) "AdventureDungeonCrawlDisplay.InitializeFromGameSaveData() - Setting the deck list using the deck from upcoming scenario, but you have deck card enchantments! Something is probably wrong. Enchantments being ignored.");
        this.SetUpDeckListFromScenario(this.m_dungeonCrawlData.GetMission(), useLoadoutOfActiveRun);
      }
      else
        this.SetUpDeckList(deckCardListPremium, useLoadoutOfActiveRun, deckCardIndices: values7, deckCardEnchantments: values8);
      this.SetUpHeroPortrait(this.m_playerHeroData);
      this.m_playMat.SetUpDefeatedBosses(this.m_defeatedBossIds, this.m_numBossesInRun);
      this.m_playMat.SetShouldShowBossHeroPowerTooltip(this.ShouldShowBossHeroPowerTutorial());
      this.m_assetLoadingHelper.AddAssetToLoad();
      this.m_playMat.SetUpCardBacks(this.m_numBossesInRun - (this.m_defeatedBossIds == null ? 0 : this.m_defeatedBossIds.Count) - 1, new AdventureDungeonCrawlPlayMat.AssetLoadCompletedCallback(this.m_assetLoadingHelper.AssetLoadCompleted));
      this.SetPlayMatStateFromGameSaveData();
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.m_dungeonCrawlDeckTray.gameObject.SetActive(false);
      this.m_retireButton.SetActive(adventureDataRecord.DungeonCrawlIsRetireSupported);
    }
    this.m_assetLoadingHelper.AssetLoadCompleted();
  }

  private void OnPlayMatPaperControllerReady(VisualController paperController)
  {
    if ((UnityEngine.Object) paperController == (UnityEngine.Object) null)
      Debug.LogError((object) "paperController was null in OnPlayMatPaperControllerReady!");
    this.m_assetLoadingHelper.AssetLoadCompleted();
  }

  private void InitializePlayMat()
  {
    this.m_assetLoadingHelper.AddAssetToLoad();
    this.m_playMat.Initialize(this.m_dungeonCrawlData);
    Widget component = (Widget) this.m_playMat.GetComponent<WidgetTemplate>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      component.RegisterDoneChangingStatesListener((Action<object>) (_ => this.m_assetLoadingHelper.AssetLoadCompleted()), (object) null, true, true);
    }
    else
    {
      Error.AddDevWarning("UI Error!", "Could not find PlayMat WidgetTemplate!");
      this.m_assetLoadingHelper.AssetLoadCompleted();
    }
  }

  private IEnumerator SetPlayMatStateFromGameSaveDataWhenReady()
  {
    while (GameSaveDataManager.Get().IsRequestPending(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey) || GameSaveDataManager.Get().IsRequestPending(AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey) || this.m_playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE)
      yield return (object) null;
    this.SetPlayMatStateFromGameSaveData();
  }

  private string GetPlayButtonTextForNextMission()
  {
    string textForNextMission = "";
    if (GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.PLAY_BUTTON_TEXT_OVERRIDE, out textForNextMission) && !string.IsNullOrEmpty(textForNextMission))
      return textForNextMission;
    return this.m_isPVPDR && !this.m_isRunActive && (this.m_realDuelSeedDeck == null || !this.m_realDuelSeedDeck.IsValidForRuleset) ? "GLUE_PVPDR_BUILD_DECK" : "GLOBAL_PLAY";
  }

  private bool IsNextMissionASpecialEncounter()
  {
    if (this.m_hasReceivedGameSaveDataServerKeyResponse)
      return this.m_dungeonCrawlData.GetMissionOverride() != 0;
    Debug.LogError((object) "GetPlayButtonTextForNextMission() - this cannot be called before we've gotten the Game Save Data Server Key response!");
    return false;
  }

  private void SetPlayMatStateFromGameSaveData()
  {
    List<long> values1 = (List<long>) null;
    List<long> values2 = (List<long>) null;
    List<long> values3 = (List<long>) null;
    List<long> values4 = (List<long>) null;
    long num1 = 0;
    long num2 = 0;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_A, out values2);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_B, out values3);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_LOOT_OPTION_C, out values4);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_TREASURE_OPTION, out values1);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_LOOT, out num1);
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_TREASURE, out num2);
    bool flag = DungeonCrawlUtil.IsDungeonRunActive(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey);
    this.m_playMat.IsNextMissionASpecialEncounter = this.IsNextMissionASpecialEncounter();
    if ((UnityEngine.Object) this.m_backButtonHighlight != (UnityEngine.Object) null)
      this.m_backButtonHighlight.ChangeState(ActorStateType.NONE);
    if (this.m_isPVPDR)
    {
      if (Cheats.Get().HasCheatTreasureIds() && values1 != null && values1.Count > 0)
      {
        List<int> addedTreasures;
        Cheats.Get().SaveDuelsCheatTreasures(out addedTreasures);
        Cheats.Get().ClearCheatTreasures();
        int num3 = Mathf.Min(addedTreasures.Count, values1.Count);
        for (int index = 0; index < num3; ++index)
          values1[index] = (long) addedTreasures[index];
      }
      if (Cheats.Get().HasCheatLootIds() && num1 == 0L && (values2 != null && values2.Count > 0 || values3 != null && values3.Count > 0 || values4 != null && values4.Count > 0))
      {
        List<int> addedLootA;
        List<int> addedLootB;
        List<int> addedLootC;
        Cheats.Get().SaveDuelsCheatLoot(out addedLootA, out addedLootB, out addedLootC);
        Cheats.Get().ClearCheatLoot();
        for (int index = 0; index < addedLootA.Count; ++index)
          values2[index + 1] = (long) addedLootA[index];
        for (int index = 0; index < addedLootB.Count; ++index)
          values3[index + 1] = (long) addedLootB[index];
        for (int index = 0; index < addedLootC.Count; ++index)
          values4[index + 1] = (long) addedLootC[index];
      }
    }
    if (flag && num2 == 0L && values1 != null && values1.Count > 0)
      this.m_playMat.ShowTreasureOptions(values1);
    else if (flag && num1 == 0L && (values2 != null && values2.Count > 0 || values3 != null && values3.Count > 0 || values4 != null && values4.Count > 0))
    {
      this.m_playMat.ShowLootOptions(values2, values3, values4);
    }
    else
    {
      if (!flag)
      {
        this.m_playMat.SetUpDefeatedBosses((List<long>) null, this.m_numBossesInRun);
        this.m_playMat.SetShouldShowBossHeroPowerTooltip(this.ShouldShowBossHeroPowerTutorial());
        this.m_playMat.SetUpCardBacks(this.m_numBossesInRun - 1, (AdventureDungeonCrawlPlayMat.AssetLoadCompletedCallback) null);
      }
      if (this.m_isPVPDR)
        this.m_playMat.ShowPVPDRActiveRun(this.GetPlayButtonTextForNextMission());
      else
        this.m_playMat.ShowNextBoss(this.GetPlayButtonTextForNextMission());
    }
  }

  private void SetPlaymatStateForShrineSelection(List<long> shrineOptions)
  {
    if (shrineOptions == null || shrineOptions.Count == 0)
    {
      Log.Adventures.PrintError("SetPlaymatStateForShrineSelection: No shrine options found for adventure.");
    }
    else
    {
      this.SetShowDeckButtonEnabled(false);
      long num;
      GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_DECK_SELECTION_TUTORIAL, out num);
      if (num == 0L)
      {
        this.m_playMat.ShowEmptyState();
        this.StartCoroutine(this.ShowDeckSelectionTutorialPopupWhenReady((Action) (() => this.m_playMat.ShowShrineOptions(shrineOptions))));
      }
      else
        this.m_playMat.ShowShrineOptions(shrineOptions);
    }
  }

  private List<long> GetDefaultStartingShrineOptions_TRL() => new List<long>()
  {
    52891L,
    51920L,
    53036L
  };

  private IEnumerator ShowDeckSelectionTutorialPopupWhenReady(
    Action popupDismissedCallback)
  {
    while (!this.m_subsceneTransitionComplete)
      yield return (object) new WaitForEndOfFrame();
    while (AdventureDungeonCrawlDisplay.s_shouldShowWelcomeBanner)
      yield return (object) new WaitForEndOfFrame();
    AdventureDef adventureDef = this.m_dungeonCrawlData.GetAdventureDef();
    if ((UnityEngine.Object) adventureDef != (UnityEngine.Object) null && !string.IsNullOrEmpty(adventureDef.m_AdventureDeckSelectionTutorialBannerPrefab))
      BannerManager.Get().ShowBanner(adventureDef.m_AdventureDeckSelectionTutorialBannerPrefab, (string) null, (string) null, (BannerManager.DelOnCloseBanner) (() =>
      {
        GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_HAS_SEEN_DECK_SELECTION_TUTORIAL, new long[1]
        {
          1L
        }));
        popupDismissedCallback();
      }));
    else
      popupDismissedCallback();
  }

  private bool HandleDemoModeReset()
  {
    if (!AdventureDungeonCrawlDisplay.IsInDemoMode() || this.m_numBossesDefeated < 3 && this.m_bossWhoDefeatedMeId == 0L)
      return false;
    this.m_isRunActive = false;
    this.m_defeatedBossIds = (List<long>) null;
    this.m_bossWhoDefeatedMeId = 0L;
    this.m_numBossesDefeated = 0;
    this.StartCoroutine(this.ShowDemoThankQuote());
    AdventureDungeonCrawlDisplay.s_shouldShowWelcomeBanner = false;
    return true;
  }

  private void TryShowWelcomeBanner()
  {
    if (!AdventureDungeonCrawlDisplay.s_shouldShowWelcomeBanner)
      return;
    AdventureDef adventureDef = this.m_dungeonCrawlData.GetAdventureDef();
    if ((UnityEngine.Object) adventureDef != (UnityEngine.Object) null && !string.IsNullOrEmpty(adventureDef.m_AdventureIntroBannerPrefab))
    {
      BannerManager.Get().ShowBanner(adventureDef.m_AdventureIntroBannerPrefab, (string) null, GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_INTRO_BANNER_BUTTON"), (BannerManager.DelOnCloseBanner) (() => AdventureDungeonCrawlDisplay.s_shouldShowWelcomeBanner = false));
      WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(this.m_dungeonCrawlData.GetMission());
      DungeonCrawlSubDef_VOLines.PlayVOLine(this.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, this.m_playerHeroData.HeroCardDbId, DungeonCrawlSubDef_VOLines.VOEventType.WELCOME_BANNER);
    }
    else
      AdventureDungeonCrawlDisplay.s_shouldShowWelcomeBanner = false;
  }

  private bool ShouldShowBossHeroPowerTutorial()
  {
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSS_HERO_POWER_TUTORIAL_PROGRESS, out num);
    if (num != 0L)
      return num == 1L;
    if (this.m_numBossesDefeated < 2)
      return false;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_BOSS_HERO_POWER_TUTORIAL_PROGRESS, new long[1]
    {
      1L
    }));
    return true;
  }

  private void ShowRunEnd(List<long> defeatedBossIds, long bossWhoDefeatedMeId)
  {
    this.m_BackButton.Flip(false, true);
    this.m_BackButton.SetEnabled(false);
    this.m_assetLoadingHelper.AddAssetToLoad();
    this.m_playMat.ShowRunEnd(defeatedBossIds, bossWhoDefeatedMeId, this.m_numBossesInRun, this.HasCompletedAdventureWithAllClasses(), this.GetRunWinsForClass(this.m_playerHeroData.HeroClasses[0]) == 1L, this.GetNumberOfClassesThatHaveCompletedAdventure(), AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey, new AdventureDungeonCrawlPlayMat.AssetLoadCompletedCallback(this.m_assetLoadingHelper.AssetLoadCompleted), new AdventureDungeonCrawlBossGraveyard.RunEndSequenceCompletedCallback(this.RunEndCompleted));
  }

  private int GetNumberOfClassesThatHaveCompletedAdventure()
  {
    int completedAdventure = 0;
    foreach (TAG_CLASS dungeonCrawlProgress in GameSaveDataManager.GetClassesFromDungeonCrawlProgressMap())
    {
      if (this.GetRunWinsForClass(dungeonCrawlProgress) > 0L)
        ++completedAdventure;
    }
    return completedAdventure;
  }

  private bool HasCompletedAdventureWithAllClasses()
  {
    List<GuestHero> currentAdventure = this.m_dungeonCrawlData.GetGuestHeroesForCurrentAdventure();
    if (currentAdventure.Count > 0)
    {
      foreach (GuestHero guestHero in currentAdventure)
      {
        TAG_CLASS classFromCardDbId = GameUtils.GetTagClassFromCardDbId(guestHero.cardDbId);
        if (GameSaveDataManager.GetClassesFromDungeonCrawlProgressMap().Contains(classFromCardDbId) && !this.HasCompletedAdventureWithClass(classFromCardDbId))
          return false;
      }
    }
    else
    {
      foreach (TAG_CLASS dungeonCrawlProgress in GameSaveDataManager.GetClassesFromDungeonCrawlProgressMap())
      {
        if (!this.HasCompletedAdventureWithClass(dungeonCrawlProgress))
          return false;
      }
    }
    return true;
  }

  private bool HasCompletedAdventureWithClass(TAG_CLASS tagClass) => this.GetRunWinsForClass(tagClass) > 0L;

  private void RunEndCompleted()
  {
    if ((UnityEngine.Object) this.m_BackButton == (UnityEngine.Object) null)
      return;
    this.m_dungeonCrawlData.SelectedHeroCardDbId = 0L;
    this.m_BackButton.Flip(true);
    this.m_BackButton.SetEnabled(true);
    if (!((UnityEngine.Object) this.m_backButtonHighlight != (UnityEngine.Object) null))
      return;
    this.m_backButtonHighlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
  }

  private void SetUpBossKillCounter(int heroCardDbId)
  {
    bool shouldSkipHeroSelect = this.m_shouldSkipHeroSelect;
    long wins = 0;
    long runWins = 0;
    this.m_bossKillCounter.SetDungeonRunData(this.m_dungeonCrawlData);
    TAG_CLASS classFromCardDbId = GameUtils.GetTagClassFromCardDbId(heroCardDbId);
    if (classFromCardDbId != TAG_CLASS.INVALID && !shouldSkipHeroSelect)
    {
      this.m_bossKillCounter.SetHeroClass(classFromCardDbId);
      AdventureDataDbfRecord adventureDataRecord = this.m_dungeonCrawlData.GetSelectedAdventureDataRecord();
      if (adventureDataRecord.DungeonCrawlSaveHeroUsingHeroDbId)
      {
        if (!this.GetBossWinsForGuestHero(AdventureUtils.GetGuestHeroIdFromHeroCardDbId(this.m_dungeonCrawlData, heroCardDbId), adventureDataRecord.AdventureId, out wins))
          wins = this.GetBossWinsForClass(classFromCardDbId);
      }
      else
        wins = this.GetBossWinsForClass(classFromCardDbId);
      runWins = this.GetRunWinsForClass(classFromCardDbId);
    }
    else if (shouldSkipHeroSelect)
    {
      GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_ALL_CLASSES_TOTAL_BOSS_WINS, out wins);
      GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_ALL_CLASSES_TOTAL_RUN_WINS, out runWins);
    }
    this.m_bossKillCounter.SetBossWins(wins);
    if (runWins > 0L)
      this.m_bossKillCounter.SetRunWins(runWins);
    this.m_bossKillCounter.UpdateLayout();
  }

  private long GetRunWinsForClass(TAG_CLASS tagClass)
  {
    long runWinsForClass = 0;
    GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys progressSubkeys;
    if (GameSaveDataManager.GetProgressSubkeyForDungeonCrawlClass(tagClass, out progressSubkeys))
      GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, progressSubkeys.runWins, out runWinsForClass);
    return runWinsForClass;
  }

  public bool IsCardLoadoutTreasureForCurrentHero(string cardID)
  {
    if (this.m_dungeonCrawlData == null)
      return false;
    List<AdventureLoadoutTreasuresDbfRecord> dungeonCrawlHero = AdventureUtils.GetTreasuresForDungeonCrawlHero(this.m_dungeonCrawlData, (int) this.m_dungeonCrawlData.SelectedHeroCardDbId);
    int dbId = GameUtils.TranslateCardIdToDbId(cardID);
    foreach (AdventureLoadoutTreasuresDbfRecord treasuresDbfRecord in dungeonCrawlHero)
    {
      if (treasuresDbfRecord.CardId == dbId)
        return true;
    }
    return false;
  }

  private bool GetBossWinsForGuestHero(int guestHeroId, int adventureId, out long wins)
  {
    int heroIdForAdventure = AdventureUtils.GetBaseGuestHeroIdForAdventure((AdventureDbId) adventureId, guestHeroId);
    GameSaveKeySubkeyId bossWinsSubkey;
    if (GameSaveDataManager.GetBossWinsSubkeyForDungeonCrawlGuestHero(heroIdForAdventure > 0 ? heroIdForAdventure : guestHeroId, out bossWinsSubkey))
    {
      GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, bossWinsSubkey, out wins);
      return true;
    }
    wins = 0L;
    return false;
  }

  private long GetBossWinsForClass(TAG_CLASS tagClass)
  {
    long bossWinsForClass = 0;
    GameSaveDataManager.AdventureDungeonCrawlClassProgressSubkeys progressSubkeys;
    if (GameSaveDataManager.GetProgressSubkeyForDungeonCrawlClass(tagClass, out progressSubkeys))
      GameSaveDataManager.Get().GetSubkeyValue(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, progressSubkeys.bossWins, out bossWinsForClass);
    return bossWinsForClass;
  }

  private void SetUpDeckListFromShrine(long shrineCardId, bool playDeckGlowAnimation)
  {
    List<long> cards = new List<long>();
    CardTagDbfRecord shrineDeckCardTagRecord = GameDbf.CardTag.GetRecord((Predicate<CardTagDbfRecord>) (r => r.CardId == (int) shrineCardId && r.TagId == 1099));
    foreach (DeckCardDbfRecord record in GameDbf.DeckCard.GetRecords((Predicate<DeckCardDbfRecord>) (r => r.DeckId == shrineDeckCardTagRecord.TagValue)))
      cards.Add((long) record.CardId);
    cards.Add(shrineCardId);
    this.SetUpDeckList(CardWithPremiumStatus.ConvertList(cards), false, playDeckGlowAnimation);
    this.SetShowDeckButtonEnabled(true);
  }

  private void SetUpDeckListFromScenario(ScenarioDbId scenario, bool useLoadoutOfActiveRun)
  {
    ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((int) scenario);
    if (record == null)
      return;
    this.SetUpDeckList(CardWithPremiumStatus.ConvertList(CollectionManager.Get().LoadDeckFromDBF(record.Player1DeckId, out string _, out string _)), useLoadoutOfActiveRun);
  }

  private void SetUpDeckList(
    List<CardWithPremiumStatus> deckCardList,
    bool useLoadoutOfActiveRun,
    bool playGlowAnimation = false,
    List<long> deckCardIndices = null,
    List<long> deckCardEnchantments = null,
    RuneType[] runeOrder = null)
  {
    if (this.m_playerHeroData.HeroClasses.Count <= 0 || this.m_playerHeroData.HeroClasses[0] == TAG_CLASS.INVALID)
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlDisplay.SetUpDeckList() - HeroClasses is INVALID!");
    }
    else
    {
      if (string.IsNullOrEmpty(this.m_playerHeroData.HeroCardId))
        return;
      this.m_dungeonCrawlDeck = new CollectionDeck()
      {
        HeroCardID = this.m_playerHeroData.HeroCardId
      };
      this.m_dungeonCrawlDeck.FormatType = PegasusShared.FormatType.FT_WILD;
      this.m_dungeonCrawlDeck.Type = DeckType.CLIENT_ONLY_DECK;
      if (this.m_isPVPDR)
      {
        this.m_dungeonCrawlDeck.Name = GameStrings.Get("GLUE_COLLECTION_DUEL_DECKNAME");
        this.m_dungeonCrawlDeck.HeroPowerCardID = GameUtils.TranslateDbIdToCardId((int) this.m_dungeonCrawlData.SelectedHeroPowerDbId);
        if (runeOrder != null)
          this.m_dungeonCrawlDeck.SetRuneOrder(runeOrder);
      }
      if (this.m_isPVPDR && !this.m_isRunActive)
      {
        this.m_dungeonCrawlDeck.Type = DeckType.PVPDR_DISPLAY_DECK;
        if (deckCardList != null && deckCardList.Count > this.m_dungeonCrawlDeck.GetMaxCardCount())
          this.m_dungeonCrawlDeck.Type = DeckType.CLIENT_ONLY_DECK;
      }
      if (this.m_anomalyModeCardDbId != 0L && this.m_dungeonCrawlData.AnomalyModeActivated)
      {
        string cardId = GameUtils.TranslateDbIdToCardId((int) this.m_anomalyModeCardDbId);
        if (cardId != null)
          this.m_dungeonCrawlDeck.AddCard(cardId, TAG_PREMIUM.NORMAL, false);
        else
          Log.Adventures.PrintWarning("AdventureDungeonCrawlDisplay.SetUpDeckList() - No cardId for anomalyCardDbId {0}!", (object) this.m_anomalyModeCardDbId);
      }
      if (this.m_plotTwistCardDbId != 0L)
      {
        string cardId = GameUtils.TranslateDbIdToCardId((int) this.m_plotTwistCardDbId);
        if (cardId != null)
          this.m_dungeonCrawlDeck.AddCard(cardId, TAG_PREMIUM.NORMAL, false);
        else
          Log.Adventures.PrintWarning("AdventureDungeonCrawlDisplay.SetUpDeckList() - No cardId for m_plotTwistCardDbId {0}!", (object) this.m_plotTwistCardDbId);
      }
      if (!useLoadoutOfActiveRun && this.m_dungeonCrawlData.SelectedLoadoutTreasureDbId != 0L)
      {
        string cardId = GameUtils.TranslateDbIdToCardId((int) this.m_dungeonCrawlData.SelectedLoadoutTreasureDbId);
        if (!string.IsNullOrEmpty(cardId))
        {
          CollectionDeckSlot firstSlotByCardId = this.m_dungeonCrawlDeck.FindFirstSlotByCardId(cardId);
          if (firstSlotByCardId == null || firstSlotByCardId.Count == 0)
            this.m_dungeonCrawlDeck.AddCard(cardId, TAG_PREMIUM.NORMAL, false);
        }
        else
          Log.Adventures.PrintWarning("AdventureDungeonCrawlDisplay.SetUpDeckList() - No cardId for SelectedLoadoutTreasureDbId {0}!", (object) this.m_dungeonCrawlData.SelectedLoadoutTreasureDbId);
      }
      if (deckCardList != null)
      {
        Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
        if (deckCardIndices != null && deckCardEnchantments != null && deckCardIndices.Count == deckCardEnchantments.Count)
        {
          for (int index = 0; index < deckCardIndices.Count; ++index)
          {
            List<int> intList;
            if (!dictionary.TryGetValue((int) deckCardIndices[index], out intList))
            {
              intList = new List<int>();
              dictionary.Add((int) deckCardIndices[index], intList);
            }
            intList.Add((int) deckCardEnchantments[index]);
          }
        }
        for (int index = 0; index < deckCardList.Count; ++index)
        {
          long cardId1 = deckCardList[index].cardId;
          TAG_PREMIUM premium = deckCardList[index].premium;
          if (cardId1 != 0L)
          {
            string cardId2 = GameUtils.TranslateDbIdToCardId((int) cardId1);
            if (cardId2 == null)
            {
              Log.Adventures.PrintWarning("AdventureDungeonCrawlDisplay.SetUpDeckList() - No cardId for dbId {0}!", (object) cardId1);
            }
            else
            {
              List<int> enchantments;
              if (dictionary.TryGetValue(index + 1, out enchantments))
                this.m_dungeonCrawlDeck.AddCard_DungeonCrawlBuff(cardId2, premium, enchantments);
              else
                this.m_dungeonCrawlDeck.AddCard(cardId2, premium, false);
            }
          }
        }
      }
      this.m_dungeonCrawlDeckTray.SetDungeonCrawlDeck(this.m_dungeonCrawlDeck, playGlowAnimation);
      this.SetUpCardsCreatedByTreasures();
      this.SetUpPhoneNewRunScreen();
    }
  }

  private void SetUpHeroPortrait(
    AdventureDungeonCrawlDisplay.PlayerHeroData playerHeroData)
  {
    if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("Unable to change hero portrait. No hero actor has been loaded.");
    }
    else
    {
      if (string.IsNullOrEmpty(playerHeroData.HeroCardId))
        return;
      bool flag1 = this.IsInDefeatScreen();
      NetCache.CardDefinition randomFavoriteHero = CollectionManager.Get().GetRandomFavoriteHero(playerHeroData.HeroClasses[0]);
      bool flag2 = this.m_dungeonCrawlData.GuestHeroesExistForCurrentAdventure();
      TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
      if (!flag1 && !flag2 && randomFavoriteHero != null && !GameUtils.IsVanillaHero(randomFavoriteHero.Name))
        premium = TAG_PREMIUM.GOLDEN;
      this.SetHero(playerHeroData.HeroCardId, premium);
      if (flag1)
        this.m_heroActor.GetComponent<Animation>().Play("AllyDefeat_Desat");
      if (this.m_heroHealth == 0L)
        this.m_heroHealth = (long) GameDbf.CardTag.GetRecord((Predicate<CardTagDbfRecord>) (r => r.CardId == playerHeroData.HeroCardDbId && r.TagId == 45)).TagValue;
      this.SetHeroHealthVisual(this.m_heroActor, !flag1);
      if (!((UnityEngine.Object) this.m_dungeonCrawlDeckSelect != (UnityEngine.Object) null))
        return;
      this.SetHeroHealthVisual(this.m_dungeonCrawlDeckSelect.heroDetails.HeroActor, !flag1);
    }
  }

  private void SetHero(string cardID, TAG_PREMIUM premium)
  {
    if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlDisplay.SetHero was called but m_heroActor was null");
    }
    else
    {
      using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardID))
      {
        if ((UnityEngine.Object) fullDef?.CardDef == (UnityEngine.Object) null || fullDef.EntityDef == null)
          return;
        this.m_heroActor.SetCardDef(fullDef.DisposableCardDef);
        this.m_heroActor.SetEntityDef(fullDef.EntityDef);
        fullDef.CardDef.m_AlwaysRenderPremiumPortrait = true;
        this.m_heroActor.SetPremium(premium);
        this.m_heroActor.UpdateAllComponents();
        this.m_heroActor.Show();
        this.m_heroClassIconsControllerReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnHeroClassIconsControllerReady));
      }
    }
  }

  private void SetHeroPower(string cardID)
  {
    if ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("AdventureDungeonCrawlDisplay.SetHeroPower was called but m_heroPowerActor was null.");
    }
    else
    {
      BoxCollider component = this.m_heroPowerActor.GetComponent<BoxCollider>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
        component.enabled = false;
      if (string.IsNullOrEmpty(cardID))
      {
        this.m_heroPowerActor.Hide();
      }
      else
      {
        DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(cardID);
        if ((UnityEngine.Object) fullDef?.CardDef == (UnityEngine.Object) null || fullDef?.EntityDef == null)
          return;
        this.m_currentHeroPowerFullDef?.Dispose();
        this.m_currentHeroPowerFullDef = fullDef;
        this.m_heroPowerActor.SetFullDef(fullDef);
        this.m_heroPowerActor.UpdateAllComponents();
        this.m_heroPowerActor.Show();
        if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
          return;
        component.enabled = true;
      }
    }
  }

  private void SetUpPhoneNewRunScreen()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_dungeonCrawlDeckTray.gameObject.SetActive(false);
    this.SetShowDeckButtonEnabled(this.m_dungeonCrawlDeck != null && this.m_dungeonCrawlDeck.GetTotalCardCount() > 0);
  }

  public void SetShowDeckButtonEnabled(bool enabled)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI || enabled == this.m_ShowDeckButton.IsEnabled())
      return;
    this.m_ShowDeckButton.SetEnabled(enabled);
    this.m_ShowDeckButton.Flip(enabled);
  }

  private void SetUpPhoneRunCompleteScreen()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.m_ShowDeckButtonFrame.SetActive(false);
    this.m_ShowDeckNoButtonFrame.SetActive(false);
    this.m_TrayFrameDefault.SetActive(false);
    this.m_TrayFrameRunComplete.SetActive(true);
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_dungeonCrawlDeckTray.gameObject.SetActive(false);
    GameUtils.SetParent((Component) this.m_AdventureTitle, this.m_AdventureTitleRunCompleteBone);
    this.m_PhoneDeckTray.SetActive(true);
    GameUtils.SetParent(this.m_PhoneDeckTray, this.m_DeckTrayRunCompleteBone);
    TraySection traySection = (TraySection) GameUtils.Instantiate((Component) this.m_DeckListHeaderPrefab, this.m_DeckListHeaderRunCompleteBone, true);
    this.m_dungeonCrawlDeckTray.OffsetDeckBigCardByVector(this.m_DeckBigCardOffsetForRunCompleteState);
    traySection.m_deckBox.m_neverUseGoldenPortraits = this.IsInDefeatScreen();
    traySection.m_deckBox.SetHeroCardID(this.m_playerHeroData.HeroCardId);
    traySection.m_deckBox.HideBanner();
    traySection.m_deckBox.SetDeckName(this.GetClassNameFromDeckClass(this.m_playerHeroData.HeroClasses[0]));
    traySection.m_deckBox.HideRenameVisuals();
    traySection.m_deckBox.SetDeckNameAsSingleLine(true);
    if (!this.IsInDefeatScreen())
      return;
    traySection.m_deckBox.PlayDesaturationAnimation();
  }

  private bool IsInDefeatScreen() => this.m_playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_BOSS_GRAVEYARD && this.m_numBossesDefeated < this.m_numBossesInRun;

  private void SetUpCardsCreatedByTreasures()
  {
    if (this.m_cardsAddedToDeckMap == null || this.m_cardsAddedToDeckMap.Count == 0 || this.m_cardsAddedToDeckMap.Count % 2 == 1)
      return;
    Dictionary<long, long> dictionary = new Dictionary<long, long>();
    for (int index = 0; index < this.m_cardsAddedToDeckMap.Count; index += 2)
      dictionary[this.m_cardsAddedToDeckMap[index]] = this.m_cardsAddedToDeckMap[index + 1];
    this.m_dungeonCrawlDeckTray.CardIdToCreatorMap = dictionary;
  }

  public static bool OnNavigateBack()
  {
    if ((UnityEngine.Object) AdventureDungeonCrawlDisplay.m_instance == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Trying to navigate back, but AdventureDungeonCrawlDisplay has been destroyed!");
      return false;
    }
    AdventureDungeonCrawlPlayMat playMat = AdventureDungeonCrawlDisplay.m_instance.m_playMat;
    if ((UnityEngine.Object) playMat != (UnityEngine.Object) null)
    {
      if (playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE)
        return false;
      playMat.HideBossHeroPowerTooltip(true);
    }
    if ((bool) UniversalInputManager.UsePhoneUI && (UnityEngine.Object) AdventureDungeonCrawlDisplay.m_instance.m_dungeonCrawlDeckTray != (UnityEngine.Object) null)
      AdventureDungeonCrawlDisplay.m_instance.m_dungeonCrawlDeckTray.gameObject.SetActive(false);
    if ((UnityEngine.Object) playMat != (UnityEngine.Object) null && playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_BOSS_GRAVEYARD && AdventureDungeonCrawlDisplay.m_instance.m_mustSelectChapter)
    {
      if (AdventureDungeonCrawlDisplay.m_instance.m_subsceneController != null)
        AdventureDungeonCrawlDisplay.m_instance.m_subsceneController.ChangeSubScene(AdventureData.Adventuresubscene.LOCATION_SELECT, false);
    }
    else if (AdventureDungeonCrawlDisplay.m_instance.m_subsceneController != null)
      AdventureDungeonCrawlDisplay.m_instance.m_subsceneController.SubSceneGoBack();
    return true;
  }

  private void OnBackButtonPress(UIEvent e)
  {
    this.EnableBackButton(false);
    Navigation.GoBack();
  }

  private void GoToHeroSelectSubscene()
  {
    int num = this.m_dungeonCrawlData.GuestHeroesExistForCurrentAdventure() ? 1 : 0;
    this.m_playMat.PlayButton.Disable();
    AdventureData.Adventuresubscene subscene = num != 0 ? AdventureData.Adventuresubscene.ADVENTURER_PICKER : AdventureData.Adventuresubscene.MISSION_DECK_PICKER;
    if (this.m_subsceneController == null)
      return;
    this.m_subsceneController.ChangeSubScene(subscene);
  }

  private void GoBackToHeroPower()
  {
    this.m_dungeonCrawlData.SelectedHeroPowerDbId = 0L;
    this.SetHeroPower((string) null);
    this.StartCoroutine(this.ShowHeroPowerOptionsWhenReady());
  }

  private void GoBackFromHeroPower() => this.m_playMat.PlayHeroPowerOptionSelected();

  private void GoBackToTreasureLoadoutSelection()
  {
    this.SetUpDeckList(new List<CardWithPremiumStatus>(), false, true);
    this.m_dungeonCrawlData.SelectedLoadoutTreasureDbId = 0L;
    this.StartCoroutine(this.ShowTreasureSatchelWhenReady());
  }

  private void GoBackFromTreasureLoadoutSelection() => this.m_playMat.PlayTreasureSatchelOptionHidden();

  private void GoBackFromDeckTemplateSelection()
  {
    this.m_dungeonCrawlData.SelectedDeckId = 0L;
    this.SetUpDeckList(new List<CardWithPremiumStatus>(), false, true);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if ((UnityEngine.Object) this.m_dungeonCrawlDeckSelect == (UnityEngine.Object) null || !this.m_dungeonCrawlDeckSelect.isReady)
      {
        Error.AddDevWarning("UI Error!", "AdventureDeckSelectWidget is not setup correctly or is not ready!");
        return;
      }
      this.m_dungeonCrawlDeckSelect.deckTray.SetDungeonCrawlDeck(this.m_dungeonCrawlDeck, false);
      this.m_dungeonCrawlDeckSelect.playButton.Disable();
    }
    else
      this.m_playMat.PlayButton.Disable();
    this.m_playMat.DeselectAllDeckOptionsWithoutId(0);
    this.m_playMat.PlayDeckOptionSelected();
  }

  private static bool OnNavigateBackFromCurrentLoadoutState()
  {
    AdventureDungeonCrawlPlayMat playMat = AdventureDungeonCrawlDisplay.m_instance.m_playMat;
    if ((UnityEngine.Object) playMat != (UnityEngine.Object) null && playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE)
      return false;
    if ((UnityEngine.Object) AdventureDungeonCrawlDisplay.m_instance == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "Trying to navigate back to previous loadout selection, but AdventureDungeonCrawlDisplay has been destroyed!");
      return false;
    }
    switch (AdventureDungeonCrawlDisplay.m_instance.m_currentLoadoutState)
    {
      case AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.HEROPOWER:
        AdventureDungeonCrawlDisplay.m_instance.GoBackFromHeroPower();
        if (AdventureDungeonCrawlDisplay.m_instance.m_dungeonCrawlData.SelectableLoadoutTreasuresExist() && !AdventureDungeonCrawlDisplay.m_instance.m_isPVPDR)
        {
          AdventureDungeonCrawlDisplay.m_instance.GoBackToTreasureLoadoutSelection();
          break;
        }
        break;
      case AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.TREASURE:
        AdventureDungeonCrawlDisplay.m_instance.GoBackFromTreasureLoadoutSelection();
        if (AdventureDungeonCrawlDisplay.m_instance.m_isPVPDR)
        {
          AdventureDungeonCrawlDisplay.m_instance.GoBackToHeroPower();
          break;
        }
        break;
      case AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.DECKTEMPLATE:
        AdventureDungeonCrawlDisplay.m_instance.GoBackFromDeckTemplateSelection();
        AdventureDungeonCrawlDisplay.m_instance.GoBackToHeroPower();
        break;
    }
    return true;
  }

  private void GoToNextLoadoutState()
  {
    switch (this.m_currentLoadoutState)
    {
      case AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.INVALID:
        if ((!this.m_dungeonCrawlData.SelectableLoadoutTreasuresExist() ? 0 : (!this.m_isPVPDR ? 1 : 0)) != 0)
        {
          this.StartCoroutine(this.ShowTreasureSatchelWhenReady());
          break;
        }
        if (!this.m_dungeonCrawlData.SelectableHeroPowersAndDecksExist())
          break;
        this.StartCoroutine(this.ShowHeroPowerOptionsWhenReady());
        break;
      case AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.HEROPOWER:
        if (this.m_isPVPDR)
        {
          this.SetUpDeckList((List<CardWithPremiumStatus>) null, false);
          this.StartCoroutine(this.ShowTreasureSatchelWhenReady());
          break;
        }
        this.StartCoroutine(this.ShowDeckOptionsWhenReady());
        break;
      case AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.TREASURE:
        if (this.m_isPVPDR)
        {
          this.SetUpDeckList((List<CardWithPremiumStatus>) null, false);
          this.LockInDuelsLoadoutSelections();
          this.StartCoroutine(this.ShowBuildDeckButtonWhenReady());
          break;
        }
        this.StartCoroutine(this.ShowHeroPowerOptionsWhenReady());
        break;
    }
  }

  private void LockInDuelsLoadoutSelections()
  {
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(AdventureDungeonCrawlDisplay.OnNavigateBackFromCurrentLoadoutState));
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(GuestHeroPickerTrayDisplay.OnNavigateBack));
    if (!this.m_dungeonCrawlData.HasValidLoadoutForSelectedAdventure())
      return;
    List<GameSaveDataManager.SubkeySaveRequest> requests = new List<GameSaveDataManager.SubkeySaveRequest>();
    this.m_dungeonCrawlData.GetSelectedAdventureDataRecord();
    requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_POWER, new long[1]
    {
      this.m_dungeonCrawlData.SelectedHeroPowerDbId
    }));
    requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_LOADOUT_TREASURE_ID, new long[1]
    {
      this.m_dungeonCrawlData.SelectedLoadoutTreasureDbId
    }));
    if (this.m_saveHeroDataUsingHeroId)
    {
      requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CARD_DB_ID, new long[1]
      {
        this.m_dungeonCrawlData.SelectedHeroCardDbId
      }));
    }
    else
    {
      TAG_CLASS heroClassFromHeroId = AdventureUtils.GetHeroClassFromHeroId((int) this.m_dungeonCrawlData.SelectedHeroCardDbId);
      requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CLASS, new long[1]
      {
        (long) heroClassFromHeroId
      }));
    }
    GameSaveDataManager.Get().SaveSubkeys(requests);
    this.m_BackButton.SetText("GLOBAL_LEAVE");
  }

  private void LockInNewRunSelectionsAndTransition()
  {
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(AdventureDungeonCrawlDisplay.OnNavigateBackFromCurrentLoadoutState));
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(GuestHeroPickerTrayDisplay.OnNavigateBack));
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(AdventureLocationSelectBook.OnNavigateBack));
    Navigation.RemoveHandler(new Navigation.NavigateBackHandler(AdventureBookPageManager.NavigateToMapPage));
    if (this.m_subsceneController != null)
      this.m_subsceneController.RemoveSubScenesFromStackUntilTargetReached(AdventureData.Adventuresubscene.CHOOSER);
    if (this.m_dungeonCrawlData.HasValidLoadoutForSelectedAdventure())
    {
      List<GameSaveDataManager.SubkeySaveRequest> requests = new List<GameSaveDataManager.SubkeySaveRequest>();
      AdventureDataDbfRecord adventureDataRecord = this.m_dungeonCrawlData.GetSelectedAdventureDataRecord();
      if (adventureDataRecord.DungeonCrawlSelectChapter)
        requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_SCENARIO_ID, new long[1]
        {
          (long) this.m_dungeonCrawlData.GetMission()
        }));
      if (adventureDataRecord.DungeonCrawlPickHeroFirst)
      {
        if (this.m_saveHeroDataUsingHeroId)
        {
          requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CARD_DB_ID, new long[1]
          {
            this.m_dungeonCrawlData.SelectedHeroCardDbId
          }));
        }
        else
        {
          TAG_CLASS heroClassFromHeroId = AdventureUtils.GetHeroClassFromHeroId((int) this.m_dungeonCrawlData.SelectedHeroCardDbId);
          requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_CLASS, new long[1]
          {
            (long) heroClassFromHeroId
          }));
        }
      }
      if (this.m_dungeonCrawlData.SelectableHeroPowersExist())
        requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_HERO_POWER, new long[1]
        {
          this.m_dungeonCrawlData.SelectedHeroPowerDbId
        }));
      if (!this.m_isPVPDR && this.m_dungeonCrawlData.SelectableDecksExist())
        requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_DECK, new long[1]
        {
          this.m_dungeonCrawlData.SelectedDeckId
        }));
      if (this.m_dungeonCrawlData.SelectableLoadoutTreasuresExist())
        requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_LOADOUT_TREASURE_ID, new long[1]
        {
          this.m_dungeonCrawlData.SelectedLoadoutTreasureDbId
        }));
      long num = this.m_dungeonCrawlData.AnomalyModeActivated ? 1L : 0L;
      requests.Add(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_SELECTED_ANOMALY_MODE, new long[1]
      {
        num
      }));
      GameSaveDataManager.Get().SaveSubkeys(requests);
      this.SetShowDeckButtonEnabled(true);
      this.m_BackButton.SetText("GLOBAL_LEAVE");
      this.StartCoroutine(this.SetPlayMatStateFromGameSaveDataWhenReady());
    }
    else
      Navigation.GoBack();
  }

  private void OnPlayButtonPress(UIEvent e)
  {
    PlayButton element = e.GetElement() as PlayButton;
    if ((UnityEngine.Object) element != (UnityEngine.Object) null)
      element.Disable();
    this.m_playMat.HideBossHeroPowerTooltip(true);
    if (this.m_dungeonCrawlData.DoesSelectedMissionRequireDeck() && !this.m_dungeonCrawlData.HeroIsSelectedBeforeDungeonCrawlScreenForSelectedAdventure() && !this.m_shouldSkipHeroSelect && (this.m_numBossesDefeated == 0 || !this.m_isRunActive))
      this.GoToHeroSelectSubscene();
    else if (this.m_playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS && this.m_playMat.GetPlayMatOptionType() == AdventureDungeonCrawlPlayMat.OptionType.DECK)
    {
      this.m_playMat.PlayDeckOptionSelected();
      this.LockInNewRunSelectionsAndTransition();
    }
    else if (this.m_isPVPDR && (this.m_realDuelSeedDeck == null || !this.m_realDuelSeedDeck.IsValidForRuleset) && !this.m_isRunActive)
    {
      if (this.m_realDuelSeedDeck == null)
      {
        this.CreateDuelsSeedDeck();
      }
      else
      {
        element.Enable();
        this.EditCurrentDeck();
      }
    }
    else
      this.QueueForGame();
  }

  private int GetHeroCardIdToUse()
  {
    if ((this.m_dungeonCrawlData.GetGuestHeroesForCurrentAdventure().Count <= 0 ? 0 : (!this.m_shouldSkipHeroSelect ? 1 : (this.m_mustPickShrine ? 1 : 0))) == 0)
      return GameUtils.GetFavoriteHeroCardDBIdFromClass(this.m_playerHeroData.HeroClasses[0]);
    int heroCardIdToUse = (int) this.m_dungeonCrawlData.SelectedHeroCardDbId;
    if (this.m_isRunActive || this.m_mustPickShrine)
      heroCardIdToUse = this.m_playerHeroData.HeroCardDbId;
    return heroCardIdToUse;
  }

  private void QueueForGame()
  {
    int heroCardIdToUse = this.GetHeroCardIdToUse();
    long deckid = 0;
    bool flag = false;
    if (this.m_isPVPDR)
    {
      CollectionDeck duelsDeck = CollectionManager.Get().GetDuelsDeck();
      if (this.m_realDuelSeedDeck != null)
        deckid = this.m_realDuelSeedDeck.ID;
      else if (duelsDeck != null)
      {
        deckid = duelsDeck.ID;
      }
      else
      {
        Debug.LogError((object) "Valid duels deck not found, canceling queue");
        flag = true;
      }
    }
    if (flag)
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_PVPDR_DECK_ERROR_TITLE"),
        m_text = GameStrings.Get("GLUE_PVPDR_DECK_ERROR_DESC"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      });
    }
    else
    {
      if (this.m_isPVPDR)
      {
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DUELS_QUEUE);
        PVPDRLobbyDataModel pvpdrLobbyDataModel = PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel();
        if (pvpdrLobbyDataModel != null)
          BnetPresenceMgr.Get().SetGameFieldBlob(22U, (IProtoBuf) new SessionRecord()
          {
            Wins = (uint) pvpdrLobbyDataModel.Wins,
            Losses = (uint) pvpdrLobbyDataModel.Losses,
            RunFinished = false,
            SessionRecordType = SessionRecordType.DUELS
          });
      }
      GameMgr.Get().FindGameWithHero(this.m_dungeonCrawlData.GameType, PegasusShared.FormatType.FT_WILD, (int) this.m_dungeonCrawlData.GetMissionToPlay(), 0, heroCardIdToUse, deckid);
    }
  }

  private void OnShowDeckButtonPress(UIEvent e) => this.ShowMobileDeckTray(this.m_dungeonCrawlDeckTray.gameObject.GetComponent<SlidingTray>());

  protected void OnSubSceneLoaded(object sender, EventArgs args)
  {
    this.m_playMat.OnSubSceneLoaded();
    this.m_playMat.SetRewardOptionSelectedCallback(new AdventureDungeonCrawlPlayMat.RewardOptionSelectedCallback(this.OnRewardOptionSelected));
    this.m_playMat.SetTreasureSatchelOptionSelectedCallback(new AdventureDungeonCrawlTreasureOption.TreasureSelectedOptionCallback(this.OnTreasureSatchelOptionSelected));
    this.m_playMat.SetHeroPowerOptionCallback(new AdventureDungeonCrawlHeroPowerOption.HeroPowerSelectedOptionCallback(this.OnHeroPowerOptionSelected), new AdventureDungeonCrawlHeroPowerOption.HeroPowerHoverOptionCallback(this.OnHeroPowerOptionRollover), new AdventureDungeonCrawlHeroPowerOption.HeroPowerHoverOptionCallback(this.OnHeroPowerOptionRollout));
    this.m_playMat.SetDeckOptionSelectedCallback(new AdventureDungeonCrawlDeckOption.DeckOptionSelectedCallback(this.OnDeckOptionSelected));
  }

  protected void OnSubSceneTransitionComplete(object sender, EventArgs args)
  {
    this.m_subsceneTransitionComplete = true;
    if ((UnityEngine.Object) this.m_dungeonCrawlDeckTray != (UnityEngine.Object) null)
      this.m_dungeonCrawlDeckTray.gameObject.SetActive(true);
    if (!((UnityEngine.Object) this.m_playMat != (UnityEngine.Object) null))
      return;
    this.m_playMat.OnSubSceneTransitionComplete();
  }

  private void OnHeroActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("AdventureDungeonCrawlDisplay.OnHeroActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AdventureDungeonCrawlDisplay.OnHeroActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        this.m_heroActor.transform.parent = this.m_socketHeroBone.transform;
        this.m_heroActor.transform.localPosition = Vector3.zero;
        this.m_heroActor.transform.localScale = Vector3.one;
        this.m_heroActor.Hide();
      }
    }
  }

  private void OnHeroPowerActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((UnityEngine.Object) go == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) string.Format("AdventureDungeonCrawlDisplay.OnHeroPowerActorLoaded() - FAILED to load actor \"{0}\"", (object) assetRef));
    }
    else
    {
      this.m_heroPowerActor = go.GetComponent<Actor>();
      if ((UnityEngine.Object) this.m_heroPowerActor == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) string.Format("AdventureDungeonCrawlDisplay.OnHeroPowerActorLoaded() - ERROR actor \"{0}\" has no Actor component", (object) assetRef));
      }
      else
      {
        this.m_heroPowerActor.transform.parent = this.m_heroPowerBone;
        this.m_heroPowerActor.transform.localPosition = Vector3.zero;
        this.m_heroPowerActor.transform.localRotation = Quaternion.identity;
        this.m_heroPowerActor.transform.localScale = Vector3.one;
        this.m_heroPowerActor.Hide();
        this.m_heroPowerActor.SetUnlit();
        PegUIElement pegUiElement = go.AddComponent<PegUIElement>();
        go.AddComponent<BoxCollider>().enabled = false;
        pegUiElement.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.ShowBigCard(this.m_heroPowerBigCard, this.m_currentHeroPowerFullDef, this.m_HeroPowerBigCardBone)));
        pegUiElement.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => BigCardHelper.HideBigCard(this.m_heroPowerBigCard)));
      }
    }
  }

  private void SetHeroHealthVisual(Actor actor, bool show)
  {
    if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("SetHeroHealthVisual: actor provided is null!");
    }
    else
    {
      actor.GetHealthObject().gameObject.SetActive(show);
      if (!show)
        return;
      actor.GetHealthText().Text = Convert.ToString(this.m_heroHealth);
      actor.GetHealthText().AmbientLightBlend = 0.0f;
    }
  }

  private IEnumerator ShowTreasureSatchelWhenReady()
  {
    while ((UnityEngine.Object) this.m_playMat == (UnityEngine.Object) null || this.m_playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE)
      yield return (object) null;
    this.m_playMat.ShowTreasureSatchel(AdventureUtils.GetTreasuresForDungeonCrawlHero(this.m_dungeonCrawlData, (int) this.m_dungeonCrawlData.SelectedHeroCardDbId), AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey);
    this.m_currentLoadoutState = AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.TREASURE;
    this.EnableBackButton(true);
    if (this.m_isPVPDR)
      Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureDungeonCrawlDisplay.OnNavigateBackFromCurrentLoadoutState));
  }

  private IEnumerator ShowHeroPowerOptionsWhenReady()
  {
    while ((UnityEngine.Object) this.m_playMat == (UnityEngine.Object) null || this.m_playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE)
      yield return (object) null;
    this.m_playMat.ShowHeroPowers(AdventureUtils.GetHeroPowersForDungeonCrawlHero(this.m_dungeonCrawlData, (int) this.m_dungeonCrawlData.SelectedHeroCardDbId), AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey);
    this.m_currentLoadoutState = AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.HEROPOWER;
    this.EnableBackButton(true);
    if (!this.m_isPVPDR && AdventureDungeonCrawlDisplay.m_instance.m_dungeonCrawlData.SelectableLoadoutTreasuresExist())
      Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureDungeonCrawlDisplay.OnNavigateBackFromCurrentLoadoutState));
  }

  private IEnumerator ShowDeckOptionsWhenReady()
  {
    while ((UnityEngine.Object) this.m_playMat == (UnityEngine.Object) null || this.m_playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE)
      yield return (object) null;
    this.m_playMat.ShowDecks(this.m_dungeonCrawlData.GetDecksForClass(this.m_playerHeroData.HeroClasses[0]), AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, AdventureDungeonCrawlDisplay.m_gameSaveDataClientKey);
    this.m_currentLoadoutState = AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.DECKTEMPLATE;
    this.EnableBackButton(true);
    Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureDungeonCrawlDisplay.OnNavigateBackFromCurrentLoadoutState));
  }

  private IEnumerator ShowBuildDeckButtonWhenReady()
  {
    while ((UnityEngine.Object) this.m_playMat == (UnityEngine.Object) null || this.m_playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE)
      yield return (object) null;
    this.m_currentLoadoutState = AdventureDungeonCrawlDisplay.DungeonRunLoadoutState.LOADOUTCOMPLETE;
    this.m_playMat.ShowPVPDRActiveRun(this.GetPlayButtonTextForNextMission());
    this.m_playMat.PlayButton.SetText(this.GetPlayButtonTextForNextMission());
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_dungeonCrawlDeckSelect.deckTray.SetDungeonCrawlDeck(this.m_dungeonCrawlDeck, false);
      using (DefLoader.DisposableCardDef cardDef = this.m_heroActor.ShareDisposableCardDef())
        this.m_dungeonCrawlDeckSelect.heroDetails.UpdateHeroInfo(this.m_heroActor.GetEntityDef(), cardDef);
      using (DefLoader.DisposableCardDef cardDef = this.m_heroPowerActor.ShareDisposableCardDef())
        this.m_dungeonCrawlDeckSelect.heroDetails.UpdateHeroPowerInfo(this.m_heroPowerActor.GetEntityDef(), cardDef);
    }
  }

  private void OnBossFullDefLoaded(string cardId, DefLoader.DisposableFullDef def, object userData)
  {
    using (def)
    {
      if (def == null)
      {
        Log.Adventures.PrintError("Unable to load {0} hero def for Dungeon Crawl boss.", (object) cardId);
        this.m_assetLoadingHelper.AssetLoadCompleted();
      }
      else
      {
        string cardId1 = (string) null;
        string cardId2 = def.EntityDef.GetCardId();
        if (GameUtils.IsModeHeroic(this.m_dungeonCrawlData.GetSelectedMode()))
        {
          int cardTagValue = GameUtils.GetCardTagValue(cardId2, GAME_TAG.HEROIC_HERO_POWER);
          if (cardTagValue != 0)
            cardId1 = GameUtils.TranslateDbIdToCardId(cardTagValue);
        }
        if (string.IsNullOrEmpty(cardId1))
          cardId1 = GameUtils.GetHeroPowerCardIdFromHero(cardId2);
        if (!string.IsNullOrEmpty(cardId1))
        {
          this.m_assetLoadingHelper.AddAssetToLoad();
          DefLoader.Get().LoadFullDef(cardId1, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnBossPowerFullDefLoaded));
        }
        EntityDef entityDef = def.EntityDef;
        if (entityDef != null && this.m_nextBossHealth != 0L && !this.m_isRunRetired)
        {
          entityDef = entityDef.Clone();
          entityDef.SetTag<long>(GAME_TAG.HEALTH, this.m_nextBossHealth);
        }
        if (this.IsNextMissionASpecialEncounter() && (UnityEngine.Object) this.m_bossActor != (UnityEngine.Object) null && (UnityEngine.Object) this.m_bossActor.GetHealthObject() != (UnityEngine.Object) null)
          this.m_bossActor.GetHealthObject().Hide();
        this.m_playMat.SetBossFullDef(def.DisposableCardDef, entityDef);
        this.m_assetLoadingHelper.AssetLoadCompleted();
      }
    }
  }

  private void OnBossPowerFullDefLoaded(
    string cardId,
    DefLoader.DisposableFullDef def,
    object userData)
  {
    if (def == null)
    {
      Debug.LogError((object) string.Format("Unable to load {0} hero power def for Dungeon Crawl boss.", (object) cardId), (UnityEngine.Object) this.gameObject);
      this.m_assetLoadingHelper.AssetLoadCompleted();
    }
    else
    {
      this.m_currentBossHeroPowerFullDef?.Dispose();
      this.m_currentBossHeroPowerFullDef = def;
      this.m_assetLoadingHelper.AssetLoadCompleted();
    }
  }

  private void OnTreasureSatchelOptionSelected(long treasureLoadoutDbId)
  {
    this.m_dungeonCrawlData.SelectedLoadoutTreasureDbId = treasureLoadoutDbId;
    AdventureConfig.Get().SelectedLoadoutTreasureDbId = treasureLoadoutDbId;
    if (this.m_dungeonCrawlData.SelectableHeroPowersAndDecksExist())
    {
      List<AdventureLoadoutTreasuresDbfRecord> dungeonCrawlHero = AdventureUtils.GetTreasuresForDungeonCrawlHero(this.m_dungeonCrawlData, (int) this.m_dungeonCrawlData.SelectedHeroCardDbId);
      int index1 = dungeonCrawlHero.FindIndex((Predicate<AdventureLoadoutTreasuresDbfRecord>) (r => (long) r.CardId == treasureLoadoutDbId));
      AdventureDungeonCrawlRewardOption.OptionData optionData;
      ref AdventureDungeonCrawlRewardOption.OptionData local = ref optionData;
      List<long> options = new List<long>();
      options.Add(treasureLoadoutDbId);
      int index2 = index1;
      local = new AdventureDungeonCrawlRewardOption.OptionData(AdventureDungeonCrawlPlayMat.OptionType.TREASURE_SATCHEL, options, index2);
      if (index1 >= 0 && index1 < dungeonCrawlHero.Count)
      {
        AdventureLoadoutTreasuresDbfRecord selectedTreasure = dungeonCrawlHero[index1];
        if (this.m_isPVPDR && selectedTreasure.GuestHeroVariantId != 0)
          this.UpdateHeroFromTreasure(selectedTreasure);
      }
      this.OnRewardOptionSelected(optionData);
      this.m_playMat.PlayTreasureSatchelOptionSelected();
      this.GoToNextLoadoutState();
    }
    else
      Debug.LogWarning((object) "AdventureDungeonCrawlDisplay.OnTreasureLoadoutOptionSelected: Selecting a Treasure Loadout but no Hero Power or Deck is not supported!");
  }

  private int GetGuestHeroCardDbIdForCurrentAdventure(int guestHeroId)
  {
    foreach (GuestHero guestHero in this.m_dungeonCrawlData.GetGuestHeroesForCurrentAdventure())
    {
      if (guestHero.guestHeroId == guestHeroId)
        return guestHero.cardDbId;
    }
    return 0;
  }

  private void UpdateHeroFromTreasure(
    AdventureLoadoutTreasuresDbfRecord selectedTreasure)
  {
    this.m_dungeonCrawlData.SelectedHeroCardDbId = (long) this.GetGuestHeroCardDbIdForCurrentAdventure(selectedTreasure.GuestHeroVariantId);
    this.m_playerHeroData.UpdateHeroDataFromHeroCardDbId((int) this.m_dungeonCrawlData.SelectedHeroCardDbId);
    this.SetUpHeroPortrait(this.m_playerHeroData);
  }

  private void OnHeroPowerOptionSelected(long heroPowerDbId, bool isLocked)
  {
    if (isLocked)
      return;
    this.m_dungeonCrawlData.SelectedHeroPowerDbId = heroPowerDbId;
    AdventureConfig.Get().SelectedHeroPowerDbId = heroPowerDbId;
    this.SetHeroPower(GameUtils.TranslateDbIdToCardId((int) heroPowerDbId));
    if ((UnityEngine.Object) this.m_HeroPowerPortraitPlayMaker != (UnityEngine.Object) null && !string.IsNullOrEmpty(this.m_HeroPowerPotraitIntroStateName))
      this.m_HeroPowerPortraitPlayMaker.SendEvent(this.m_HeroPowerPotraitIntroStateName);
    this.m_playMat.PlayHeroPowerOptionSelected();
    this.GoToNextLoadoutState();
  }

  private void OnHeroPowerOptionRollover(long heroPowerDbId, GameObject bone)
  {
    GameUtils.SetParent((Component) this.m_heroPowerBigCard, bone);
    using (DefLoader.DisposableFullDef fullDef = DefLoader.Get().GetFullDef(GameUtils.TranslateDbIdToCardId((int) heroPowerDbId)))
      this.ShowBigCard(this.m_heroPowerBigCard, fullDef, this.m_HeroPowerBigCardBone);
  }

  private void OnHeroPowerOptionRollout(long heroPowerDbId, GameObject bone)
  {
    BigCardHelper.HideBigCard(this.m_heroPowerBigCard);
    GameUtils.SetParent((Component) this.m_heroPowerBigCard, this.m_HeroPowerBigCardBone);
  }

  private void OnDeckOptionSelected(int deckId, List<long> deckContent, bool deckIsLocked)
  {
    this.m_playMat.DeselectAllDeckOptionsWithoutId(deckId);
    this.m_dungeonCrawlData.SelectedDeckId = (long) deckId;
    this.SetUpDeckList(CardWithPremiumStatus.ConvertList(deckContent), false, true);
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if ((UnityEngine.Object) this.m_dungeonCrawlDeckSelect == (UnityEngine.Object) null || !this.m_dungeonCrawlDeckSelect.isReady)
      {
        Error.AddDevWarning("UI Error!", "AdventureDeckSelectWidget is not setup correctly or is not ready!");
        return;
      }
      this.m_dungeonCrawlDeckSelect.deckTray.SetDungeonCrawlDeck(this.m_dungeonCrawlDeck, false);
      using (DefLoader.DisposableCardDef cardDef = this.m_heroActor.ShareDisposableCardDef())
        this.m_dungeonCrawlDeckSelect.heroDetails.UpdateHeroInfo(this.m_heroActor.GetEntityDef(), cardDef);
      using (DefLoader.DisposableCardDef cardDef = this.m_heroPowerActor.ShareDisposableCardDef())
        this.m_dungeonCrawlDeckSelect.heroDetails.UpdateHeroPowerInfo(this.m_heroPowerActor.GetEntityDef(), cardDef);
      if (deckIsLocked)
        this.m_dungeonCrawlDeckSelect.playButton.Disable();
      else
        this.m_dungeonCrawlDeckSelect.playButton.Enable();
      this.ShowMobileDeckTray(this.m_dungeonCrawlDeckSelect.slidingTray);
    }
    else if (deckIsLocked)
      this.m_playMat.PlayButton.Disable();
    else
      this.m_playMat.PlayButton.Enable();
    this.GoToNextLoadoutState();
  }

  private void OnRewardOptionSelected(
    AdventureDungeonCrawlRewardOption.OptionData optionData)
  {
    if (!GameSaveDataManager.Get().IsDataReady(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey))
      Log.Adventures.PrintError("Attempting to make a selection, but no data is ready yet!");
    else if (this.m_playMat.GetPlayMatState() != AdventureDungeonCrawlPlayMat.PlayMatState.SHOWING_OPTIONS)
    {
      Log.Adventures.PrintError("Attempting to choose a reward, but the Play Mat is not currently in the 'SHOWING_OPTIONS' state!");
    }
    else
    {
      GameSaveKeySubkeyId subkey = GameSaveKeySubkeyId.INVALID;
      switch (optionData.optionType)
      {
        case AdventureDungeonCrawlPlayMat.OptionType.LOOT:
          subkey = GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_LOOT;
          break;
        case AdventureDungeonCrawlPlayMat.OptionType.TREASURE:
          subkey = GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_TREASURE;
          break;
        case AdventureDungeonCrawlPlayMat.OptionType.SHRINE_TREASURE:
          subkey = GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_SHRINE;
          break;
      }
      this.m_dungeonCrawlDeckTray.gameObject.SetActive(true);
      Action onCompleteCallback = (Action) null;
      if (optionData.optionType == AdventureDungeonCrawlPlayMat.OptionType.SHRINE_TREASURE)
      {
        if (this.m_shrineOptions == null || this.m_shrineOptions.Count == 0)
        {
          Log.Adventures.PrintError("OnRewardOptionSelected: Player selected a shrine, but there are no shrine options!");
          return;
        }
        long shrineCardId = this.m_shrineOptions[optionData.index];
        this.m_playerHeroData.UpdateHeroDataFromClass(this.GetClassFromShrine(shrineCardId));
        this.SetUpDeckList(new List<CardWithPremiumStatus>(), false);
        onCompleteCallback = (Action) (() =>
        {
          this.SetUpDeckListFromShrine(shrineCardId, true);
          this.ChangeHeroPortrait(this.m_playerHeroData.HeroCardId, TAG_PREMIUM.NORMAL);
        });
      }
      for (int index = subkey == GameSaveKeySubkeyId.DUNGEON_CRAWL_PLAYER_CHOSEN_LOOT ? 1 : 0; index < optionData.options.Count; ++index)
      {
        string cardId = GameUtils.TranslateDbIdToCardId((int) optionData.options[index], true);
        if (!string.IsNullOrEmpty(cardId))
        {
          Actor animateFromActor = (Actor) null;
          if (!(bool) UniversalInputManager.UsePhoneUI)
            animateFromActor = this.m_playMat.GetActorToAnimateFrom(cardId, optionData.index);
          this.m_dungeonCrawlDeckTray.AddCard(cardId, animateFromActor, onCompleteCallback);
        }
      }
      TooltipPanelManager.Get().HideKeywordHelp();
      if (optionData.optionType != AdventureDungeonCrawlPlayMat.OptionType.TREASURE_SATCHEL)
      {
        GameSaveDataManager.Get().SaveSubkeys(new List<GameSaveDataManager.SubkeySaveRequest>()
        {
          new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, subkey, new long[1]
          {
            (long) (optionData.index + 1)
          })
        });
        this.m_playMat.PlayRewardOptionSelected(optionData);
        this.StartCoroutine(this.SetPlayMatStateFromGameSaveDataWhenReady());
      }
      this.PlayRewardSelectVO(optionData);
    }
  }

  private void PlayRewardSelectVO(
    AdventureDungeonCrawlRewardOption.OptionData optionData)
  {
    if ((optionData.optionType == AdventureDungeonCrawlPlayMat.OptionType.TREASURE ? 1 : (optionData.optionType == AdventureDungeonCrawlPlayMat.OptionType.SHRINE_TREASURE ? 1 : 0)) == 0)
      return;
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(this.m_dungeonCrawlData.GetMission());
    if (DungeonCrawlSubDef_VOLines.GetNextValidEventType(this.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, this.m_playerHeroData.HeroCardDbId, DungeonCrawlSubDef_VOLines.OFFER_LOOT_PACKS_EVENTS) != DungeonCrawlSubDef_VOLines.VOEventType.INVALID)
      return;
    int treasureDatabaseId = AdventureDungeonCrawlRewardOption.GetTreasureDatabaseID(optionData);
    if (treasureDatabaseId == 47251 && !Options.Get().GetBool(Option.HAS_JUST_SEEN_LOOT_NO_TAKE_CANDLE_VO))
      return;
    DungeonCrawlSubDef_VOLines.PlayVOLine(this.m_dungeonCrawlData.GetSelectedAdventure(), wingIdFromMissionId, this.m_playerHeroData.HeroCardDbId, DungeonCrawlSubDef_VOLines.VOEventType.TAKE_TREASURE_GENERAL, treasureDatabaseId);
  }

  private bool ShouldShowRunCompletedScreen()
  {
    if (this.m_isPVPDR)
      return DungeonCrawlUtil.IsPVPDRSessionComplete();
    return (this.m_defeatedBossIds != null || this.m_bossWhoDefeatedMeId != 0L) && !this.m_isRunActive && !this.m_isRunRetired && !this.m_hasSeenLatestDungeonRunComplete;
  }

  private void ShowMobileDeckTray(SlidingTray tray)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    if ((UnityEngine.Object) tray == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "ToggleMobileDeckTray: Could not find SlidingTray on Dungeon Crawl Deck Tray.");
    }
    else
    {
      this.m_playMat.HideBossHeroPowerTooltip(true);
      this.SetHeroHealthVisual(this.m_heroActor, true);
      SlidingTray.TrayToggledListener trayListener = (SlidingTray.TrayToggledListener) null;
      trayListener = (SlidingTray.TrayToggledListener) (shown => this.OnMobileDeckTrayToggled(tray, shown, trayListener));
      tray.RegisterTrayToggleListener(trayListener);
      tray.ToggleTraySlider(true);
    }
  }

  private void OnMobileDeckTrayToggled(
    SlidingTray tray,
    bool shown,
    SlidingTray.TrayToggledListener trayListener)
  {
    if (shown)
      return;
    tray.UnregisterTrayToggleListener(trayListener);
    this.m_playMat.ShowBossHeroPowerTooltip();
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CANCELED:
        this.HandleGameStartupFailure();
        break;
      case FindGameState.SERVER_GAME_STARTED:
        if (this.m_subsceneController != null)
        {
          this.m_subsceneController.RemoveSubSceneIfOnTopOfStack(AdventureData.Adventuresubscene.ADVENTURER_PICKER);
          this.m_subsceneController.RemoveSubSceneIfOnTopOfStack(AdventureData.Adventuresubscene.MISSION_DECK_PICKER);
          break;
        }
        break;
    }
    return false;
  }

  private void HandleGameStartupFailure()
  {
    if (SceneMgr.Get().IsInDuelsMode())
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DUELS_IDLE);
    this.EnablePlayButton();
  }

  private IEnumerator ShowDemoThankQuote()
  {
    string thankQuote = Vars.Key("Demo.DungeonThankQuote").GetStr("");
    float seconds = Vars.Key("Demo.DungeonThankQuoteDelaySeconds").GetFloat(1f);
    float blockSeconds = Vars.Key("Demo.DungeonThankQuoteDurationSeconds").GetFloat(5f);
    BannerPopup thankBanner = (BannerPopup) null;
    yield return (object) new WaitForSeconds(seconds);
    BannerManager.Get().ShowBanner("NewPopUp_LOOT.prefab:c1f1a158f539ad3428175ebcd948f138", (string) null, thankQuote, (BannerManager.DelOnCloseBanner) (() =>
    {
      AdventureDungeonCrawlDisplay.s_shouldShowWelcomeBanner = true;
      this.TryShowWelcomeBanner();
    }), (Action<BannerPopup>) (popup =>
    {
      thankBanner = popup;
      GameObject inputBlocker = CameraUtils.CreateInputBlocker(CameraUtils.FindFirstByLayer(thankBanner.gameObject.layer), "BannerInputBlocker", (Component) thankBanner.transform);
      inputBlocker.transform.localPosition = new Vector3(0.0f, 100f, 0.0f);
      inputBlocker.layer = 17;
    }));
    while ((UnityEngine.Object) thankBanner == (UnityEngine.Object) null)
      yield return (object) null;
    yield return (object) new WaitForSeconds(blockSeconds);
    thankBanner.Close();
  }

  private static bool IsInDemoMode() => DemoMgr.Get().GetMode() == DemoMode.BLIZZCON_2017_ADVENTURE;

  private void DisableBackButtonIfInDemoMode()
  {
    if (!AdventureDungeonCrawlDisplay.IsInDemoMode())
      return;
    this.m_BackButton.SetEnabled(false);
    this.m_BackButton.Flip(false);
  }

  private void SetDungeonCrawlDisplayVisualStyle()
  {
    DungeonRunVisualStyle visualStyle = this.m_dungeonCrawlData.VisualStyle;
    foreach (AdventureDungeonCrawlDisplay.DungeonCrawlDisplayStyleOverride displayStyleOverride in this.m_DungeonCrawlDisplayStyle)
    {
      if (displayStyleOverride.VisualStyle == visualStyle)
      {
        MeshRenderer component1 = this.m_dungeonCrawlTray.GetComponent<MeshRenderer>();
        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null && (UnityEngine.Object) displayStyleOverride.DungeonCrawlTrayMaterial != (UnityEngine.Object) null)
          RendererExtension.SetMaterial((Renderer) component1, displayStyleOverride.DungeonCrawlTrayMaterial);
        if (!(bool) UniversalInputManager.UsePhoneUI || !((UnityEngine.Object) this.m_ViewDeckTrayMesh != (UnityEngine.Object) null))
          break;
        MeshRenderer component2 = this.m_ViewDeckTrayMesh.GetComponent<MeshRenderer>();
        if (!((UnityEngine.Object) component2 != (UnityEngine.Object) null) || !((UnityEngine.Object) displayStyleOverride.PhoneDeckTrayMaterial != (UnityEngine.Object) null))
          break;
        RendererExtension.SetMaterial((Renderer) component2, displayStyleOverride.PhoneDeckTrayMaterial);
        break;
      }
    }
  }

  private string GetClassNameFromDeckClass(TAG_CLASS deckClass)
  {
    List<GuestHero> currentAdventure = this.m_dungeonCrawlData.GetGuestHeroesForCurrentAdventure();
    if (currentAdventure.Count == 0)
      return GameStrings.GetClassName(deckClass);
    foreach (GuestHero guestHero in currentAdventure)
    {
      GuestHero guest = guestHero;
      if (GameUtils.GetTagClassFromCardDbId(guest.cardDbId) == deckClass)
        return (string) GameDbf.GuestHero.GetRecord((Predicate<GuestHeroDbfRecord>) (r => r.CardId == guest.cardDbId)).Name;
    }
    return string.Empty;
  }

  private TAG_CLASS GetClassFromShrine(long shrineCardId) => GameUtils.GetTagClassFromCardDbId((int) shrineCardId);

  private void ChangeHeroPortrait(string newHeroCardId, TAG_PREMIUM premium)
  {
    if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
      Log.Adventures.PrintError(string.Format("Unable to change hero portrait to cardId={0}. No actor has been loaded.", (object) newHeroCardId));
    else
      DefLoader.Get().LoadFullDef(newHeroCardId, (DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>) ((cardId, fullDef, userData) =>
      {
        using (fullDef)
        {
          this.m_heroActor.SetFullDef(fullDef);
          this.m_heroActor.SetPremium(premium);
          this.m_heroActor.GetComponent<PlayMakerFSM>().SendEvent(fullDef.EntityDef.GetClass().ToString());
        }
      }));
  }

  public static Actor OnActorLoaded(
    string actorName,
    GameObject actorObject,
    GameObject container,
    bool withRotation = false)
  {
    Actor component = actorObject.GetComponent<Actor>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      if ((UnityEngine.Object) container != (UnityEngine.Object) null)
      {
        GameUtils.SetParent((Component) component, container, withRotation);
        LayerUtils.SetLayer((Component) component, container.layer);
      }
      component.SetUnlit();
      component.Hide();
    }
    else
      Debug.LogWarning((object) string.Format("ERROR actor \"{0}\" has no Actor component", (object) actorName));
    return component;
  }

  private void ShowBigCard(
    Actor heroPowerBigCard,
    DefLoader.DisposableFullDef heroPowerFullDef,
    GameObject bone)
  {
    Vector3? origin = new Vector3?();
    if ((UnityEngine.Object) this.m_heroPowerActor != (UnityEngine.Object) null)
      origin = new Vector3?(this.m_heroPowerActor.gameObject.transform.position);
    BigCardHelper.ShowBigCard(heroPowerBigCard, heroPowerFullDef, bone, this.m_BigCardScale, origin);
  }

  private void OnHeroClassIconsControllerReady(Widget widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      Debug.LogWarning((object) "AdventureDungeonCrawlDisplay.OnHeroIconsControllerReady - widget was null!");
    else if ((UnityEngine.Object) this.m_heroActor == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "AdventureDungeonCrawlDisplay.OnHeroIconsControllerReady - m_heroActor was null!");
    }
    else
    {
      HeroClassIconsDataModel classIconsDataModel = new HeroClassIconsDataModel();
      EntityDef entityDef = this.m_heroActor.GetEntityDef();
      if (entityDef == null)
      {
        Debug.LogWarning((object) "AdventureDungeonCrawlDisplay.OnHeroIconsControllerReady - m_heroActor did not contain an entity def!");
      }
      else
      {
        classIconsDataModel.Classes.Clear();
        entityDef.GetClasses((IList<TAG_CLASS>) classIconsDataModel.Classes);
        widget.BindDataModel((IDataModel) classIconsDataModel);
      }
    }
  }

  private static void ResetDungeonCrawlSelections(IDungeonCrawlData data)
  {
    data.SelectedLoadoutTreasureDbId = 0L;
    data.SelectedHeroPowerDbId = 0L;
    data.SelectedDeckId = 0L;
  }

  private void OnRetirePopupResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
    {
      this.m_retireButton.SetActive(true);
    }
    else
    {
      Navigation.GoBack();
      if ((bool) UniversalInputManager.UsePhoneUI)
        Navigation.GoBack();
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_IS_RUN_RETIRED, new long[1]
      {
        1L
      }), (GameSaveDataManager.OnSaveDataResponseDelegate) (dataWrittenSuccessfully => this.HandleRetireSuccessOrFail(dataWrittenSuccessfully)));
    }
  }

  private void HandleRetireSuccessOrFail(bool success)
  {
    if (success)
      return;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_RETIRE_CONFIRMATION_HEADER"),
      m_text = GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_RETIRE_FAILURE_BODY"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
    this.m_retireButton.SetActive(true);
  }

  public void CreateDuelsSeedDeck()
  {
    if (this.m_seedDeckCreateRequested)
      return;
    TAG_CLASS classFromCardDbId = GameUtils.GetTagClassFromCardDbId((int) this.m_dungeonCrawlData.SelectedHeroCardDbId);
    string cardId = GameUtils.TranslateDbIdToCardId((int) this.m_dungeonCrawlData.SelectedHeroCardDbId);
    CollectionDeckTray.Get().GetDecksContent().CreateNewDeckFromUserSelection(classFromCardDbId, cardId, "PVPDR DECK");
    this.m_seedDeckCreateRequested = true;
  }

  private void OnDeckCreated(long deckID, string name)
  {
    if (!this.m_seedDeckCreateRequested)
      return;
    CollectionDeck deck = CollectionManager.Get().GetDeck(deckID);
    if (deck == null || deckID != deck.ID || !this.m_isPVPDR)
      return;
    this.m_realDuelSeedDeck = deck;
    this.m_dungeonCrawlDeck.CardBackID = deck.CardBackID;
    Network.Get().SendPVPDRSessionInfoRequest();
    this.m_playMat.PlayButton.Enable();
    this.EditCurrentDeck();
    this.m_seedDeckCreateRequested = false;
  }

  public bool BackFromDeckEdit(CollectionDeck deck)
  {
    this.SyncDeckList();
    this.SaveDuelsDeckList();
    this.StartCoroutine(this.EnablePlayButtonWhenDeckChangesAreSaved(20f));
    this.m_BackButton.SetText("GLOBAL_LEAVE");
    this.m_dungeonCrawlDeckTray.m_cardsContent.UpdateCardList();
    if (!this.m_realDuelSeedDeck.IsValidForRuleset)
      return true;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_PVPDR_LOCK_IN"),
      m_text = GameStrings.Get("GLUE_PVPDR_LOCK_IN_DESC"),
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if (response != AlertPopup.Response.CONFIRM)
          return;
        this.LockInNewRunSelectionsAndTransition();
        Network.Get().SendPVPDRSessionInfoRequest();
        CollectionDeckTray.Get().OnConfirmBackOutOfDeckContentsDuel();
        Navigation.RemoveHandler(new Navigation.NavigateBackHandler(this.OnExitCollection));
        PvPDungeonRunScene.Get().ShowDungeonCrawlDisplay((Action<object>) (e =>
        {
          if (!(bool) UniversalInputManager.UsePhoneUI)
            return;
          this.m_dungeonCrawlDeckTray.gameObject.SetActive(true);
        }));
      })
    });
    return false;
  }

  private IEnumerator EnablePlayButtonWhenDeckChangesAreSaved(float timeout)
  {
    bool didTimeout = false;
    while (this.m_realDuelSeedDeck.IsSavingChanges())
    {
      this.DisablePlayButton();
      timeout -= Time.deltaTime;
      if ((double) timeout <= 0.0)
      {
        didTimeout = true;
        break;
      }
      yield return (object) null;
    }
    if (didTimeout)
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_PVPDR_DECK_ERROR_TITLE"),
        m_text = GameStrings.Get("GLUE_COLLECTION_GENERIC_ERROR"),
        m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_responseCallback = (AlertPopup.ResponseCallback) ((poopupResponse, userData) => SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB))
      });
    else
      this.EnablePlayButton();
  }

  private void EditCurrentDeck()
  {
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnExitCollection));
    if (this.m_dungeonCrawlDeck == null)
      return;
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_dungeonCrawlDeckTray.gameObject.SetActive(false);
    CollectionManagerDisplay collectionManagerDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    collectionManagerDisplay.EnableInput(true);
    CollectionDeckTray.Get().EnterDeckEditForPVPDR(this.m_dungeonCrawlDeck);
    PvPDungeonRunScene.Get().HideDungeonCrawlDisplay((Action) (() => collectionManagerDisplay.CheckClipboardAndPromptPlayerToPaste()));
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DUELS_BUILDING_DECK);
  }

  public bool IsDuelsDeckValid() => this.m_realDuelSeedDeck != null && this.m_realDuelSeedDeck.IsValidForRuleset;

  public void SyncDeckList()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (this.m_realDuelSeedDeck == null || editedDeck == null)
      return;
    this.m_realDuelSeedDeck.RemoveAllCards();
    foreach (CollectionDeckSlot slot in editedDeck.GetSlots())
      this.m_realDuelSeedDeck.AddCard(slot.CardID, slot.PreferredPremium, false, DeckRule.RuleType.DEATHKNIGHT_RUNE_LIMIT);
    int? cardBackId1 = editedDeck.CardBackID;
    int? cardBackId2 = this.m_realDuelSeedDeck.CardBackID;
    if (!(cardBackId1.GetValueOrDefault() == cardBackId2.GetValueOrDefault() & cardBackId1.HasValue == cardBackId2.HasValue))
      this.m_realDuelSeedDeck.CardBackID = editedDeck.CardBackID;
    this.m_realDuelSeedDeck.SetRuneOrder(editedDeck.GetRuneOrder());
  }

  public void SaveDuelsDeckList()
  {
    CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
    if (this.m_realDuelSeedDeck == null || editedDeck == null)
      return;
    CollectionManager.Get().SetEditedDeck(this.m_realDuelSeedDeck);
    CollectionDeckTray.SaveCurrentDeck();
    CollectionManager.Get().SetEditedDeck(editedDeck);
  }

  private bool OnExitCollection()
  {
    if (!CollectionDeckTray.Get().OnBackOutOfContainerContents())
      return false;
    PvPDungeonRunScene.Get().ShowDungeonCrawlDisplay((Action<object>) (e =>
    {
      if (!(bool) UniversalInputManager.UsePhoneUI)
        return;
      this.m_dungeonCrawlDeckTray.gameObject.SetActive(true);
    }));
    this.m_dungeonCrawlDeckTray.GetCardsContent().UpdateTileVisuals();
    return true;
  }

  private void OnPVPDRRetirePopupResponse(AlertPopup.Response response, object userData)
  {
    if (response == AlertPopup.Response.CANCEL)
    {
      this.m_retireButton.SetActive(true);
    }
    else
    {
      Network.Get().RegisterNetHandler((object) PVPDRRetireResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRRetireResponse));
      Network.Get().SendPVPDRRetireRequest();
    }
  }

  private void OnPVPDRRetireResponse()
  {
    GameSaveDataManager.Get().Request(AdventureDungeonCrawlDisplay.m_gameSaveDataServerKey);
    Network.Get().SendPVPDRSessionInfoRequest();
    Network.Get().RemoveNetHandler((object) PVPDRRetireResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRRetireResponse));
    PVPDRRetireResponse pvpdrRetireResponse = Network.Get().GetPVPDRRetireResponse();
    bool success = pvpdrRetireResponse != null && pvpdrRetireResponse.ErrorCode == PegasusShared.ErrorCode.ERROR_OK;
    this.HandleRetireSuccessOrFail(success);
    if (!success)
      return;
    PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().RecentWin = false;
    PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().RecentLoss = false;
    if (PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().IsPaidEntry)
    {
      this.ShowDuelsEndRun();
    }
    else
    {
      if ((bool) UniversalInputManager.UsePhoneUI)
        Navigation.GoBack();
      this.EndDuelsSession();
    }
  }

  private void ShowDuelsEndRun()
  {
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DUELS_REWARD);
    this.m_playMat.ShowPVPDRReward();
    this.m_BackButton.SetEnabled(false);
    this.m_BackButton.Flip(false);
    Navigation.PushBlockBackingOut();
  }

  public void EndDuelsSession(long noticeId = 0)
  {
    this.m_rewardNoticeId = noticeId;
    Network.Get().RegisterNetHandler((object) PVPDRSessionEndResponse.PacketID.ID, new Network.NetHandler(this.OnSessionEndResponse));
    Network.Get().SendPVPDRSessionEndRequest();
  }

  private void OnSessionEndResponse()
  {
    Network.Get().RemoveNetHandler((object) PVPDRSessionEndResponse.PacketID.ID, new Network.NetHandler(this.OnSessionEndResponse));
    PVPDRSessionEndResponse sessionEndResponse = Network.Get().GetPVPDRSessionEndResponse();
    if (sessionEndResponse.ErrorCode != PegasusShared.ErrorCode.ERROR_OK)
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_PVPDR"),
        m_text = GameStrings.Get("GLUE_PVPDR_SESSION_END_ERROR"),
        m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_responseCallback = (AlertPopup.ResponseCallback) ((poopupResponse, userData) => SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB))
      });
    }
    else
    {
      if (this.m_rewardNoticeId > 0L)
      {
        Network.Get().AckNotice(this.m_rewardNoticeId);
        this.m_rewardNoticeId = 0L;
      }
      DuelsConfig.Get().SetRecentEnd(true);
      int num1 = sessionEndResponse.NewRating - PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().Rating;
      int num2 = sessionEndResponse.NewPaidRating - PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().PaidRating;
      Network.Get().RequestPVPDRStatsInfo();
      DuelsConfig.Get().LastRunWins = PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().Wins;
      if (!PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().IsPaidEntry)
      {
        PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().LastPlayedMode = 1;
        PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().LastRatingChange = num1;
        this.StartCoroutine(this.ShowRatingChangePopupWhenReady((Action) (() => this.OnSessionEndComplete())));
      }
      else
      {
        PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().LastPlayedMode = 2;
        PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().LastRatingChange = num2;
        this.OnSessionEndComplete();
      }
    }
  }

  private IEnumerator ShowRatingChangePopupWhenReady(Action callback)
  {
    PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().IsRatingNotice = true;
    while ((UnityEngine.Object) this.m_playMat == (UnityEngine.Object) null || !this.m_playMat.IsReady() || this.m_playMat.GetPlayMatState() == AdventureDungeonCrawlPlayMat.PlayMatState.TRANSITIONING_FROM_PREV_STATE || !Navigation.CanGoBack)
    {
      Log.Adventures.Print("Waiting for Play Mat to be initialized before showing rating change popup");
      yield return (object) null;
    }
    int lastRatingChange = PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().LastRatingChange;
    int lastRunWins = DuelsConfig.Get().LastRunWins;
    string key = "GLUE_PVPDR_END_OF_RUN_TIER_0_WIN";
    string str = string.Concat((object) lastRatingChange);
    if (lastRatingChange >= 0)
      str = "+" + (object) lastRatingChange;
    if (lastRunWins >= 12)
      key = "GLUE_PVPDR_END_OF_RUN_TIER_MAX_WIN";
    else if (lastRunWins >= 6)
      key = "GLUE_PVPDR_END_OF_RUN_TIER_2_WIN";
    else if (lastRunWins >= 1)
      key = "GLUE_PVPDR_END_OF_RUN_TIER_1_WIN";
    PvPDungeonRunScene.ShowDuelsMessagePopup(GameStrings.Format("GLUE_PVPDR_END_OF_RUN_HEADER", (object) lastRunWins), GameStrings.Get(key), GameStrings.Format("GLUE_PVPDR_RATING_CHANGE", (object) str), callback);
  }

  private void OnSessionEndComplete()
  {
    PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().HasSession = false;
    PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().IsSessionActive = false;
    Navigation.PopBlockBackingOut();
    Navigation.GoBack();
  }

  [Serializable]
  public class DungeonCrawlDisplayStyleOverride
  {
    public DungeonRunVisualStyle VisualStyle;
    public Material DungeonCrawlTrayMaterial;
    public Material PhoneDeckTrayMaterial;
  }

  public class PlayerHeroData
  {
    private readonly IDungeonCrawlData m_dungeonCrawlData;

    public event AdventureDungeonCrawlDisplay.PlayerHeroData.DataChangedEventHandler OnHeroDataChanged;

    public PlayerHeroData(IDungeonCrawlData dungeonCrawlData) => this.m_dungeonCrawlData = dungeonCrawlData;

    public List<TAG_CLASS> HeroClasses { get; private set; }

    public int HeroCardDbId { get; private set; }

    public string HeroCardId { get; private set; }

    public void UpdateHeroDataFromHeroCardDbId(int heroCardDbId)
    {
      if (heroCardDbId == 0)
      {
        this.ClearHeroData();
      }
      else
      {
        this.HeroCardDbId = heroCardDbId;
        CardDbfRecord record = GameDbf.Card.GetRecord(heroCardDbId);
        if (record == null)
        {
          Debug.LogWarning((object) string.Format("AdventureDungeonCrawlDisplay.UpdateHeroDataFromHeroCardDbId: Unable to find hero for: heroCardDBId [{0}]", (object) heroCardDbId));
          this.ClearHeroData();
        }
        else
        {
          this.HeroCardId = record.NoteMiniGuid;
          EntityDef entityDef = DefLoader.Get().GetEntityDef(heroCardDbId);
          if (entityDef == null)
          {
            Debug.LogWarning((object) string.Format("AdventureDungeonCrawlDisplay.UpdateHeroDataFromHeroCardDbId: No entity found for id: {0}", (object) heroCardDbId));
            this.ClearHeroData();
          }
          else
          {
            this.HeroClasses = new List<TAG_CLASS>();
            entityDef.GetClasses((IList<TAG_CLASS>) this.HeroClasses);
            if (this.HeroClasses == null || this.HeroClasses.Count < 1)
            {
              Debug.LogWarning((object) string.Format("AdventureDungeonCrawlDisplay.UpdateHeroDataFromHeroCardDbId: Unable to find classes for: heroCardDBId [{0}]", (object) heroCardDbId));
              this.ClearHeroData();
            }
            else
            {
              AdventureDungeonCrawlDisplay.PlayerHeroData.DataChangedEventHandler onHeroDataChanged = this.OnHeroDataChanged;
              if (onHeroDataChanged == null)
                return;
              onHeroDataChanged();
            }
          }
        }
      }
    }

    public void UpdateHeroDataFromClass(TAG_CLASS heroClass)
    {
      if (heroClass == TAG_CLASS.INVALID)
      {
        this.ClearHeroData();
      }
      else
      {
        this.HeroClasses = new List<TAG_CLASS>()
        {
          heroClass
        };
        this.HeroCardId = AdventureUtils.GetHeroCardIdFromClassForDungeonCrawl(this.m_dungeonCrawlData, heroClass);
        this.HeroCardDbId = GameUtils.TranslateCardIdToDbId(this.HeroCardId);
        AdventureDungeonCrawlDisplay.PlayerHeroData.DataChangedEventHandler onHeroDataChanged = this.OnHeroDataChanged;
        if (onHeroDataChanged == null)
          return;
        onHeroDataChanged();
      }
    }

    private void ClearHeroData()
    {
      this.HeroCardDbId = 0;
      this.HeroCardId = string.Empty;
      if (this.HeroClasses != null)
        this.HeroClasses.Clear();
      else
        this.HeroClasses = new List<TAG_CLASS>();
      AdventureDungeonCrawlDisplay.PlayerHeroData.DataChangedEventHandler onHeroDataChanged = this.OnHeroDataChanged;
      if (onHeroDataChanged == null)
        return;
      onHeroDataChanged();
    }

    public delegate void DataChangedEventHandler();
  }

  private enum DungeonRunLoadoutState
  {
    INVALID,
    HEROPOWER,
    TREASURE,
    DECKTEMPLATE,
    LOADOUTCOMPLETE,
  }
}
