using Assets;
using Blizzard.T5.AssetManager;
using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public class AdventureMissionDisplay : AdventureSubSceneDisplay
{
  [CustomEditField(Sections = "Boss Layout Settings")]
  public GameObject m_BossWingContainer;
  [CustomEditField(Sections = "Boss Info")]
  public UberText m_BossTitle;
  [CustomEditField(Sections = "Boss Info")]
  public UberText m_BossDescription;
  [CustomEditField(Sections = "UI")]
  public UberText m_AdventureTitle;
  [CustomEditField(Sections = "UI")]
  public PegUIElement m_BackButton;
  [CustomEditField(Sections = "UI")]
  public PlayButton m_ChooseButton;
  [CustomEditField(Sections = "UI")]
  public GameObject m_BossPortraitContainer;
  [CustomEditField(Sections = "UI")]
  public GameObject m_BossPowerContainer;
  [CustomEditField(Sections = "UI")]
  public PegUIElement m_BossPowerHoverArea;
  [CustomEditField(Sections = "UI", T = EditType.GAME_OBJECT)]
  public MeshRenderer m_WatermarkIcon;
  [CustomEditField(Sections = "UI")]
  public AdventureRewardsDisplayArea m_RewardsDisplay;
  [CustomEditField(Sections = "UI")]
  public GameObject m_ClickBlocker;
  [CustomEditField(Sections = "UI/Animation")]
  public float m_CoinFlipDelayTime = 1.25f;
  [CustomEditField(Sections = "UI/Animation")]
  public float m_CoinFlipAnimationTime = 1f;
  [CustomEditField(Sections = "UI/Scroll Bar")]
  public UIBScrollable m_ScrollBar;
  [CustomEditField(Sections = "UI/Preview Pane")]
  public AdventureRewardsPreview m_PreviewPane;
  public AdventureMissionDisplayTray m_advMissionDisplayTray;
  [SerializeField]
  private Vector3 m_BossWingOffset = Vector3.zero;
  private static AdventureMissionDisplay s_instance;
  private AdventureWingProgressDisplay m_progressDisplay;
  private List<AdventureWing> m_BossWings = new List<AdventureWing>();
  private GameObject m_BossWingBorder;
  private AdventureBossCoin m_SelectedCoin;
  private Map<ScenarioDbId, DefLoader.DisposableFullDef> m_BossPortraitDefCache = new Map<ScenarioDbId, DefLoader.DisposableFullDef>();
  private Map<ScenarioDbId, DefLoader.DisposableFullDef> m_BossPowerDefCache = new Map<ScenarioDbId, DefLoader.DisposableFullDef>();
  private int m_DisableSelectionCount;
  private List<AdventureWing> m_WingsToGiveBigChest = new List<AdventureWing>();
  private bool m_ShowingRewardsPreview;
  private int m_TotalBosses;
  private int m_TotalBossesDefeated;
  private bool m_BossJustDefeated;
  private bool m_WaitingForClassChallengeUnlocks;
  private int m_ClassChallengeUnlockShowing;
  private Map<ScenarioDbId, AdventureMissionDisplay.BossInfo> m_BossInfoCache = new Map<ScenarioDbId, AdventureMissionDisplay.BossInfo>();
  private MusicPlaylistType m_mainMusic;
  private List<AdventureWing> m_wingsToFocus = new List<AdventureWing>();
  private Coroutine m_scheduledBringWingsToFocusCallback;
  private AssetHandle<Texture> m_watermarkTexture;
  private List<AdventureMissionDisplay.ProgressStepCompletedCallback> m_ProgressStepCompletedListeners = new List<AdventureMissionDisplay.ProgressStepCompletedCallback>();
  private bool m_waitingOnExternalLock;
  private const float s_ScreenBackTransitionDelay = 1.8f;
  private bool m_showingAdventureCompletePopup;
  private static int s_cheat_nextWingToGrantChest;

  [CustomEditField(Sections = "Boss Layout Settings")]
  public Vector3 BossWingOffset
  {
    get => this.m_BossWingOffset;
    set
    {
      this.m_BossWingOffset = value;
      this.UpdateWingPositions();
    }
  }

  public static AdventureMissionDisplay Get() => AdventureMissionDisplay.s_instance;

  private void Awake()
  {
    AdventureMissionDisplay.s_instance = this;
    this.m_mainMusic = MusicManager.Get().GetCurrentPlaylist();
    AdventureConfig adventureConfig = AdventureConfig.Get();
    AdventureDbId selectedAdventure = adventureConfig.GetSelectedAdventure();
    AdventureModeDbId selectedMode = adventureConfig.GetSelectedMode();
    this.m_AdventureTitle.Text = (string) GameUtils.GetAdventureDataRecord((int) selectedAdventure, (int) selectedMode).Name;
    List<AdventureMissionDisplay.WingCreateParams> paramsList = this.BuildWingCreateParamsList();
    this.m_WingsToGiveBigChest.Clear();
    AdventureDef adventureDef = AdventureScene.Get().GetAdventureDef(selectedAdventure);
    AdventureSubDef subDef = adventureDef.GetSubDef(selectedMode);
    if (!string.IsNullOrEmpty(adventureDef.m_WingBottomBorderPrefab))
    {
      this.m_BossWingBorder = AssetLoader.Get().InstantiatePrefab((AssetReference) adventureDef.m_WingBottomBorderPrefab);
      GameUtils.SetParent(this.m_BossWingBorder, this.m_BossWingContainer);
    }
    this.AddAssetToLoad(3);
    foreach (AdventureMissionDisplay.WingCreateParams wingCreateParams in paramsList)
      this.AddAssetToLoad(wingCreateParams.m_BossCreateParams.Count * 2);
    this.m_TotalBosses = 0;
    this.m_TotalBossesDefeated = 0;
    if (!string.IsNullOrEmpty((string) (MobileOverrideValue<string>) adventureDef.m_ProgressDisplayPrefab))
    {
      this.m_progressDisplay = GameUtils.LoadGameObjectWithComponent<AdventureWingProgressDisplay>((string) (MobileOverrideValue<string>) adventureDef.m_ProgressDisplayPrefab);
      if ((UnityEngine.Object) this.m_progressDisplay != (UnityEngine.Object) null && (UnityEngine.Object) this.m_BossWingContainer != (UnityEngine.Object) null)
        GameUtils.SetParent((Component) this.m_progressDisplay, this.m_BossWingContainer);
    }
    foreach (AdventureMissionDisplay.WingCreateParams wingCreateParams in paramsList)
    {
      WingDbId wingId = wingCreateParams.m_WingDef.GetWingId();
      AdventureWingDef wingDef = wingCreateParams.m_WingDef;
      AdventureWing wing = GameUtils.LoadGameObjectWithComponent<AdventureWing>((string) (MobileOverrideValue<string>) wingDef.m_WingPrefab);
      if (!((UnityEngine.Object) wing == (UnityEngine.Object) null))
      {
        if ((UnityEngine.Object) this.m_BossWingContainer != (UnityEngine.Object) null)
          GameUtils.SetParent((Component) wing, this.m_BossWingContainer);
        wing.Initialize(wingDef);
        wing.SetBigChestRewards(wingId);
        wing.AddBossSelectedListener((AdventureWing.BossSelected) ((c, m) => this.OnBossSelected(c, m, true)));
        wing.AddOpenPlateStartListener(new AdventureWing.OpenPlateStart(this.OnStartUnlockPlate));
        wing.AddOpenPlateEndListener(new AdventureWing.OpenPlateEnd(this.OnEndUnlockPlate));
        wing.AddTryPurchaseWingListener((AdventureWing.TryPurchaseWing) (() => this.ShowAdventureStore(wing)));
        wing.AddShowRewardsListener((AdventureWing.ShowRewards) ((r, o) => this.m_RewardsDisplay.ShowRewards(r, o)));
        wing.AddHideRewardsListener((AdventureWing.HideRewards) (r => this.m_RewardsDisplay.HideRewards()));
        List<int> wingScenarios = new List<int>();
        int num = 0;
        foreach (AdventureMissionDisplay.BossCreateParams bossCreateParam in wingCreateParams.m_BossCreateParams)
        {
          bool enabled = AdventureConfig.IsMissionAvailable((int) bossCreateParam.m_MissionId) || wingCreateParams.m_WingDef.CoinsStartFaceUp;
          AdventureBossCoin coin = wing.CreateBoss(wingDef.m_CoinPrefab, wingDef.m_RewardsPrefab, bossCreateParam.m_MissionId, enabled);
          AdventureConfig.Get().LoadBossDef(bossCreateParam.m_MissionId, (AdventureConfig.DelBossDefLoaded) ((bossDef, y) =>
          {
            if ((UnityEngine.Object) bossDef != (UnityEngine.Object) null)
              coin.SetPortraitMaterial(bossDef);
            this.AssetLoadCompleted();
          }));
          if (AdventureConfig.Get().GetLastSelectedMission() == bossCreateParam.m_MissionId)
            this.StartCoroutine(this.RememberLastBossSelection(coin, bossCreateParam.m_MissionId));
          if (AdventureProgressMgr.Get().HasDefeatedScenario((int) bossCreateParam.m_MissionId))
          {
            ++num;
            ++this.m_TotalBossesDefeated;
          }
          ++this.m_TotalBosses;
          DefLoader.Get().LoadFullDef(bossCreateParam.m_CardDefId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroFullDefLoaded), (object) bossCreateParam.m_MissionId);
          wingScenarios.Add((int) bossCreateParam.m_MissionId);
        }
        bool flag1 = adventureConfig.GetWingBossesDefeated(selectedAdventure, selectedMode, wingId, num) != num;
        if (flag1)
          this.m_BossJustDefeated = true;
        if ((UnityEngine.Object) wing.m_BigChest != (UnityEngine.Object) null)
        {
          bool flag2 = num == wingCreateParams.m_BossCreateParams.Count;
          if (!wing.HasBigChestRewards())
            wing.HideBigChest();
          else if (flag2)
          {
            if (flag1)
              this.m_WingsToGiveBigChest.Add(wing);
            else
              wing.BigChestStayOpen();
          }
        }
        if ((UnityEngine.Object) this.m_progressDisplay != (UnityEngine.Object) null)
        {
          bool linearComplete = AdventureProgressMgr.Get().IsWingComplete(selectedAdventure, AdventureModeDbId.LINEAR, wingId);
          this.m_progressDisplay.UpdateProgress(wingCreateParams.m_WingDef.GetWingId(), linearComplete);
        }
        adventureConfig.UpdateWingBossesDefeated(selectedAdventure, selectedMode, wingId, num);
        wing.AddShowRewardsPreviewListeners((AdventureWing.ShowRewardsPreview) (() => this.ShowRewardsPreview(wing, wingScenarios.ToArray(), wing.GetBigChestRewards(), wing.GetWingName())));
        wing.UpdateRewardsPreviewCover();
        wing.RandomizeBackground();
        wing.SetBringToFocusCallback(new AdventureWing.BringToFocusCallback(this.BatchBringWingToFocus));
        this.m_BossWings.Add(wing);
        if (AdventureScene.Get().IsDevMode)
          wing.InitializeDevMode();
      }
    }
    AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Play_Hero.prefab:42cbbd2c4969afb46b3887bb628de19d", new PrefabCallback<GameObject>(this.OnHeroActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    AssetLoader.Get().InstantiatePrefab((AssetReference) "Card_Play_HeroPower.prefab:a3794839abb947146903a26be13e09af", new PrefabCallback<GameObject>(this.OnHeroPowerActorLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    AssetLoader.Get().InstantiatePrefab((AssetReference) "History_HeroPower_Opponent.prefab:a99d23d6e8630f94b96a8e096fffb16f", new PrefabCallback<GameObject>(this.OnHeroPowerBigCardLoaded), options: AssetLoadingOptions.IgnorePrefabPosition);
    if ((UnityEngine.Object) this.m_BossPowerHoverArea != (UnityEngine.Object) null)
    {
      this.m_BossPowerHoverArea.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e => this.ShowBossPowerBigCard()));
      this.m_BossPowerHoverArea.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => this.HideBossPowerBigCard()));
    }
    this.m_BackButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonPress));
    this.m_ChooseButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ChangeToDeckPicker()));
    this.UpdateWingPositions();
    this.m_ChooseButton.Disable();
    StoreManager.Get().RegisterStoreShownListener(new Action(this.OnStoreShown));
    StoreManager.Get().RegisterStoreHiddenListener(new Action(this.OnStoreHidden));
    AdventureProgressMgr.Get().RegisterProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdate));
    if ((UnityEngine.Object) this.m_WatermarkIcon != (UnityEngine.Object) null && !string.IsNullOrEmpty(subDef.m_WatermarkTexture))
    {
      AssetLoader.Get().LoadAsset<Texture>(ref this.m_watermarkTexture, (AssetReference) subDef.m_WatermarkTexture);
      if (this.m_watermarkTexture != null)
        RendererExtension.GetMaterial((Renderer) this.m_WatermarkIcon).mainTexture = (Texture) this.m_watermarkTexture;
      else
        Debug.LogWarning((object) string.Format("Adventure Watermark texture is null: {0}", (object) subDef.m_WatermarkTexture));
    }
    else
      Debug.LogWarning((object) string.Format("Adventure Watermark texture is null: m_WatermarkIcon: {0},  advSubDef.m_WatermarkTexture: {1}", (object) this.m_WatermarkIcon, (object) subDef.m_WatermarkTexture));
    this.m_BackButton.gameObject.SetActive(true);
    this.m_PreviewPane.AddHideListener(new AdventureRewardsPreview.OnHide(this.OnHideRewardsPreview));
    AdventureDataDbfRecord adventureDataRecord = AdventureConfig.Get().GetSelectedAdventureDataRecord();
    if (adventureDataRecord.GameSaveDataServerKey == 0)
      return;
    this.AddAssetToLoad();
    GameSaveDataManager.Get().Request((GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey, new GameSaveDataManager.OnRequestDataResponseDelegate(this.OnGameSaveDataReceived));
  }

  private void Start()
  {
    Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureMissionDisplay.OnNavigateBack));
    AdventureWing adventureWing = (AdventureWing) null;
    foreach (AdventureWing bossWing in this.m_BossWings)
    {
      if ((UnityEngine.Object) adventureWing == (UnityEngine.Object) null || bossWing.GetWingDef().GetUnlockOrder() < adventureWing.GetWingDef().GetUnlockOrder())
        adventureWing = bossWing;
    }
    if ((UnityEngine.Object) this.m_ScrollBar != (UnityEngine.Object) null)
    {
      this.m_ScrollBar.UpdateScroll();
      if ((UnityEngine.Object) adventureWing != (UnityEngine.Object) null && (UnityEngine.Object) adventureWing != (UnityEngine.Object) this.m_BossWings[0])
        adventureWing.BringToFocus();
      this.m_ScrollBar.LoadScroll(AdventureConfig.Get().GetSelectedAdventureAndModeString(), false);
    }
    AdventureConfig.Get().OnAdventureSceneUnloadEvent += new Action(this.OnAdventureSceneUnloaded);
  }

  protected override void OnDestroy()
  {
    AdventureProgressMgr.Get()?.RemoveProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.OnAdventureProgressUpdate));
    StoreManager.Get()?.RemoveStoreHiddenListener(new Action(this.OnStoreHidden));
    StoreManager.Get()?.RemoveStoreShownListener(new Action(this.OnStoreShown));
    if ((UnityEngine.Object) AdventureConfig.Get() != (UnityEngine.Object) null)
      AdventureConfig.Get().OnAdventureSceneUnloadEvent -= new Action(this.OnAdventureSceneUnloaded);
    this.SaveScrollbarValue();
    AdventureMissionDisplay.s_instance = (AdventureMissionDisplay) null;
    this.m_BossPortraitDefCache.DisposeValuesAndClear<ScenarioDbId, DefLoader.DisposableFullDef>();
    this.m_BossPowerDefCache.DisposeValuesAndClear<ScenarioDbId, DefLoader.DisposableFullDef>();
    AssetHandle.SafeDispose<Texture>(ref this.m_watermarkTexture);
    base.OnDestroy();
  }

  private void OnAdventureSceneUnloaded() => this.SaveScrollbarValue();

  private void Update()
  {
    if (!AdventureScene.Get().IsDevMode)
      return;
    if (InputCollection.GetKeyDown(KeyCode.Z))
      this.StartCoroutine(this.AnimateAdventureCompleteCheckmarksAndPopups(true));
    if (InputCollection.GetKeyDown(KeyCode.X))
      this.ShowAdventureCompletePopup();
    if (InputCollection.GetKeyDown(KeyCode.C))
      this.Cheat_OpenNextWing();
    if (!InputCollection.GetKeyDown(KeyCode.V))
      return;
    this.Cheat_OpenNextChest();
  }

  public bool IsDisabledSelection() => this.m_DisableSelectionCount > 0;

  public void AddProgressStepCompletedListener(
    AdventureMissionDisplay.ProgressStepCompletedCallback callback)
  {
    this.m_ProgressStepCompletedListeners.Add(callback);
  }

  private void FireProgressStepCompletedListeners(AdventureMissionDisplay.ProgressStep progress)
  {
    foreach (AdventureMissionDisplay.ProgressStepCompletedCallback completedCallback in this.m_ProgressStepCompletedListeners.ToArray())
      completedCallback(progress);
  }

  private void UpdateWingPositions()
  {
    float num = 0.0f;
    if ((UnityEngine.Object) this.m_progressDisplay != (UnityEngine.Object) null)
    {
      this.m_progressDisplay.transform.localPosition = this.m_BossWingOffset;
      TransformUtil.SetLocalPosZ((Component) this.m_progressDisplay, this.m_BossWingOffset.z - num);
      num += this.HeightForScrollableItem(this.m_progressDisplay.gameObject);
    }
    foreach (AdventureWing bossWing in this.m_BossWings)
    {
      bossWing.transform.localPosition = this.m_BossWingOffset;
      TransformUtil.SetLocalPosZ((Component) bossWing, this.m_BossWingOffset.z - num);
      num += this.HeightForScrollableItem(bossWing.gameObject);
    }
    if (!((UnityEngine.Object) this.m_BossWingBorder != (UnityEngine.Object) null))
      return;
    this.m_BossWingBorder.transform.localPosition = this.m_BossWingOffset;
    TransformUtil.SetLocalPosZ(this.m_BossWingBorder, this.m_BossWingOffset.z - num);
  }

  private float HeightForScrollableItem(GameObject go)
  {
    UIBScrollableItem component = go.GetComponent<UIBScrollableItem>();
    if (!((UnityEngine.Object) component == (UnityEngine.Object) null))
      return component.m_size.z;
    Log.All.PrintError("No UIBScrollableItem component on the GameObject {0}!", (object) go);
    return 0.0f;
  }

  private void OnHeroFullDefLoaded(string cardId, DefLoader.DisposableFullDef def, object userData)
  {
    if (def == null)
    {
      Debug.LogError((object) string.Format("Unable to load {0} hero def for Adventure boss.", (object) cardId), (UnityEngine.Object) this.gameObject);
      this.AssetLoadCompleted();
    }
    else
    {
      ScenarioDbId scenarioDbId = (ScenarioDbId) userData;
      this.m_BossPortraitDefCache.SetOrReplaceDisposable<ScenarioDbId, DefLoader.DisposableFullDef>(scenarioDbId, def);
      string missionHeroPowerCardId = GameUtils.GetMissionHeroPowerCardId((int) scenarioDbId);
      if (!string.IsNullOrEmpty(missionHeroPowerCardId))
      {
        this.AddAssetToLoad();
        DefLoader.Get().LoadFullDef(missionHeroPowerCardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnHeroPowerFullDefLoaded), (object) scenarioDbId);
      }
      this.AssetLoadCompleted();
    }
  }

  private void OnHeroPowerFullDefLoaded(
    string cardId,
    DefLoader.DisposableFullDef def,
    object userData)
  {
    if (def == null)
    {
      Debug.LogError((object) string.Format("Unable to load {0} hero power def for Adventure boss.", (object) cardId), (UnityEngine.Object) this.gameObject);
      this.AssetLoadCompleted();
    }
    else
    {
      this.m_BossPowerDefCache.SetOrReplaceDisposable<ScenarioDbId, DefLoader.DisposableFullDef>((ScenarioDbId) userData, def);
      this.AssetLoadCompleted();
    }
  }

  private void OnHeroActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_BossActor = AdventureSubSceneDisplay.OnActorLoaded((string) assetRef, go, this.m_BossPortraitContainer);
    if ((UnityEngine.Object) this.m_BossActor != (UnityEngine.Object) null && (UnityEngine.Object) this.m_BossActor.GetHealthObject() != (UnityEngine.Object) null)
      this.m_BossActor.GetHealthObject().Hide();
    this.AssetLoadCompleted();
  }

  private void OnHeroPowerActorLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    this.m_HeroPowerActor = AdventureSubSceneDisplay.OnActorLoaded((string) assetRef, go, this.m_BossPowerContainer);
    this.AssetLoadCompleted();
  }

  private void OnHeroPowerBigCardLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_BossPowerBigCard = AdventureSubSceneDisplay.OnActorLoaded((string) assetRef, go, (UnityEngine.Object) this.m_HeroPowerActor == (UnityEngine.Object) null ? (GameObject) null : this.m_HeroPowerActor.gameObject);
    if ((UnityEngine.Object) this.m_BossPowerBigCard != (UnityEngine.Object) null)
      this.m_BossPowerBigCard.TurnOffCollider();
    this.AssetLoadCompleted();
  }

  private void OnGameSaveDataReceived(bool success)
  {
    this.AssetLoadCompleted();
    this.UpdateCoinsWithGameSaveData();
  }

  private void UpdateCoinsWithGameSaveData()
  {
    foreach (AdventureWing bossWing in this.m_BossWings)
      bossWing.UpdateAllBossCoinChests();
  }

  protected override void OnSubSceneLoaded()
  {
    if ((UnityEngine.Object) this.m_BossPowerBigCard != (UnityEngine.Object) null && (UnityEngine.Object) this.m_HeroPowerActor != (UnityEngine.Object) null && (UnityEngine.Object) this.m_BossPowerBigCard.transform.parent != (UnityEngine.Object) this.m_HeroPowerActor.transform)
      GameUtils.SetParent((Component) this.m_BossPowerBigCard, this.m_HeroPowerActor.gameObject);
    base.OnSubSceneLoaded();
  }

  protected override void OnSubSceneTransitionComplete()
  {
    base.OnSubSceneTransitionComplete();
    this.TryShowWelcomeBanner((BannerManager.DelOnCloseBanner) (() => this.StartCoroutine(this.UpdateAndAnimateProgress(this.m_BossWings, true))));
  }

  private static bool OnNavigateBack()
  {
    AdventureConfig.Get().SubSceneGoBack();
    return true;
  }

  private void OnBackButtonPress(UIEvent e)
  {
    foreach (AdventureWing bossWing in this.m_BossWings)
      bossWing.NavigateBackCleanup();
    Navigation.GoBack();
  }

  private void OnZeroCostTransactionStoreExit(bool authorizationBackButtonPressed, object userData)
  {
    if (!authorizationBackButtonPressed)
      return;
    this.OnBackButtonPress((UIEvent) null);
  }

  private void OnBossSelected(AdventureBossCoin coin, ScenarioDbId mission, bool playerSelected)
  {
    if (this.IsDisabledSelection())
      return;
    if ((UnityEngine.Object) this.m_SelectedCoin != (UnityEngine.Object) null)
      this.m_SelectedCoin.Select(false);
    this.m_SelectedCoin = coin;
    this.m_SelectedCoin.Select(true);
    if ((UnityEngine.Object) this.m_ChooseButton != (UnityEngine.Object) null)
    {
      if (!this.m_ChooseButton.IsEnabled())
        this.m_ChooseButton.Enable();
      this.m_ChooseButton.SetText(GameStrings.Get(AdventureConfig.DoesMissionRequireDeck(mission) ? "GLUE_CHOOSE" : "GLOBAL_PLAY"));
    }
    this.ShowBossFrame(mission);
    AdventureConfig.Get().SetMission(mission, playerSelected);
    AdventureBossDef bossDef = AdventureConfig.Get().GetBossDef(mission);
    if (bossDef.m_MissionMusic != MusicPlaylistType.Invalid && !MusicManager.Get().StartPlaylist(bossDef.m_MissionMusic))
      this.ResumeMainMusic();
    if (!playerSelected || bossDef.m_IntroLinePlayTime != AdventureBossDef.IntroLinePlayTime.MissionSelect)
      return;
    AdventureUtils.PlayMissionQuote(bossDef, this.DetermineCharacterQuotePos(coin.gameObject));
  }

  private Vector3 DetermineCharacterQuotePos(GameObject coin)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return NotificationManager.PHONE_CHARACTER_POS;
    Bounds boundsOfChildren = TransformUtil.GetBoundsOfChildren(coin);
    Vector3 center = boundsOfChildren.center;
    center.y -= boundsOfChildren.extents.y;
    Camera camera = Box.Get().GetCamera();
    return (double) camera.WorldToScreenPoint(center).y < (double) (0.4f * (float) camera.pixelHeight) ? NotificationManager.ALT_ADVENTURE_SCREEN_POS : NotificationManager.DEFAULT_CHARACTER_POS;
  }

  private void ShowBossFrame(ScenarioDbId mission)
  {
    AdventureMissionDisplay.BossInfo bossInfo;
    if (this.m_BossInfoCache.TryGetValue(mission, out bossInfo))
    {
      this.m_BossTitle.Text = bossInfo.m_Title;
      if ((UnityEngine.Object) this.m_BossDescription != (UnityEngine.Object) null)
        this.m_BossDescription.Text = bossInfo.m_Description;
    }
    DefLoader.DisposableFullDef disposableFullDef;
    if (this.m_BossPortraitDefCache.TryGetValue(mission, out disposableFullDef))
    {
      this.m_BossActor.SetPremium(TAG_PREMIUM.NORMAL);
      this.m_BossActor.SetEntityDef(disposableFullDef.EntityDef);
      this.m_BossActor.SetCardDef(disposableFullDef.DisposableCardDef);
      this.m_BossActor.UpdateAllComponents();
      this.m_BossActor.SetUnlit();
      this.m_BossActor.Show();
    }
    if (this.m_BossPowerDefCache.TryGetValue(mission, out disposableFullDef))
    {
      this.m_HeroPowerActor.SetPremium(TAG_PREMIUM.NORMAL);
      this.m_HeroPowerActor.SetEntityDef(disposableFullDef.EntityDef);
      this.m_HeroPowerActor.SetCardDef(disposableFullDef.DisposableCardDef);
      this.m_HeroPowerActor.UpdateAllComponents();
      this.m_HeroPowerActor.SetUnlit();
      this.m_HeroPowerActor.Show();
      this.m_CurrentBossHeroPowerFullDef?.Dispose();
      this.m_CurrentBossHeroPowerFullDef = disposableFullDef?.Share();
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((int) mission);
      if (!((UnityEngine.Object) this.m_BossPowerContainer != (UnityEngine.Object) null) || this.m_BossPowerContainer.activeSelf || record.HideBossHeroPowerInUi)
        return;
      this.m_BossPowerContainer.SetActive(true);
    }
    else
    {
      if (!((UnityEngine.Object) this.m_BossPowerContainer != (UnityEngine.Object) null))
        return;
      this.m_BossPowerContainer.SetActive(false);
    }
  }

  private void UnselectBoss()
  {
    if ((UnityEngine.Object) this.m_BossTitle != (UnityEngine.Object) null)
      this.m_BossTitle.Text = string.Empty;
    if ((UnityEngine.Object) this.m_BossDescription != (UnityEngine.Object) null)
      this.m_BossDescription.Text = string.Empty;
    this.m_BossActor.Hide();
    if ((UnityEngine.Object) this.m_BossPowerContainer != (UnityEngine.Object) null)
      this.m_BossPowerContainer.SetActive(false);
    if ((UnityEngine.Object) this.m_SelectedCoin != (UnityEngine.Object) null)
      this.m_SelectedCoin.Select(false);
    this.m_SelectedCoin = (AdventureBossCoin) null;
    AdventureConfig.Get().SetMission(ScenarioDbId.INVALID);
    if (!this.m_ChooseButton.IsEnabled())
      return;
    this.m_ChooseButton.Disable();
  }

  private void ChangeToDeckPicker()
  {
    ScenarioDbId mission = AdventureConfig.Get().GetMission();
    AdventureBossDef bossDef = AdventureConfig.Get().GetBossDef(mission);
    if ((UnityEngine.Object) bossDef != (UnityEngine.Object) null && bossDef.m_IntroLinePlayTime == AdventureBossDef.IntroLinePlayTime.MissionStart)
      AdventureUtils.PlayMissionQuote(bossDef, this.DetermineCharacterQuotePos(this.m_ChooseButton.gameObject));
    if (AdventureConfig.Get().DoesSelectedMissionRequireDeck())
    {
      this.m_ChooseButton.Disable();
      this.DisableSelection(true);
      AdventureConfig.Get().ChangeSubScene(AdventureData.Adventuresubscene.MISSION_DECK_PICKER);
    }
    else
    {
      if ((UnityEngine.Object) this.m_advMissionDisplayTray != (UnityEngine.Object) null)
        this.m_advMissionDisplayTray.EnableRewardsChest(false);
      GameMgr.Get().FindGame(GameType.GT_VS_AI, PegasusShared.FormatType.FT_WILD, (int) AdventureConfig.Get().GetMission());
    }
  }

  public void GetExternalUILock()
  {
    this.m_waitingOnExternalLock = true;
    this.DisableSelection(true);
  }

  public void ReleaseExternalUILock()
  {
    this.m_waitingOnExternalLock = false;
    this.DisableSelection(false);
  }

  private void DisableSelection(bool yes)
  {
    if ((UnityEngine.Object) this.m_ClickBlocker == (UnityEngine.Object) null)
      return;
    this.m_DisableSelectionCount += yes ? 1 : -1;
    bool flag = this.IsDisabledSelection();
    if (this.m_ClickBlocker.gameObject.activeSelf == flag)
      return;
    this.m_ClickBlocker.gameObject.SetActive(flag);
    this.m_ScrollBar.Enable(!flag);
  }

  private void TryShowWelcomeBanner(BannerManager.DelOnCloseBanner OnCloseBanner)
  {
    bool flag = true;
    foreach (AdventureWing bossWing in this.m_BossWings)
    {
      int ack;
      AdventureProgressMgr.Get().GetWingAck((int) bossWing.GetWingId(), out ack);
      if (ack > 0)
      {
        flag = false;
        break;
      }
    }
    if (flag)
    {
      AdventureDef adventureDef = AdventureScene.Get().GetAdventureDef(AdventureConfig.Get().GetSelectedAdventure());
      if ((UnityEngine.Object) adventureDef != (UnityEngine.Object) null && !string.IsNullOrEmpty(adventureDef.m_AdventureEntryQuotePrefab) && !string.IsNullOrEmpty(adventureDef.m_AdventureEntryQuoteVOLine))
      {
        string legacyAssetName = new AssetReference(adventureDef.m_AdventureEntryQuoteVOLine).GetLegacyAssetName();
        NotificationManager.Get().CreateCharacterQuote(adventureDef.m_AdventureEntryQuotePrefab, GameStrings.Get(legacyAssetName), adventureDef.m_AdventureEntryQuoteVOLine);
      }
      if ((UnityEngine.Object) adventureDef != (UnityEngine.Object) null && !string.IsNullOrEmpty(adventureDef.m_AdventureIntroBannerPrefab))
      {
        BannerManager.Get().ShowBanner(adventureDef.m_AdventureIntroBannerPrefab, (string) null, (string) null, OnCloseBanner);
        return;
      }
    }
    OnCloseBanner();
  }

  private IEnumerator UpdateAndAnimateProgress(
    List<AdventureWing> wings,
    bool scrollToCoin,
    bool forceCoinAnimation = false)
  {
    AdventureMissionDisplay adventureMissionDisplay = this;
    yield return (object) adventureMissionDisplay.StartCoroutine(adventureMissionDisplay.UpdateAndAnimateWingCoinsAndChests(wings, scrollToCoin, forceCoinAnimation));
    adventureMissionDisplay.FireProgressStepCompletedListeners(AdventureMissionDisplay.ProgressStep.WING_COINS_AND_CHESTS_UPDATED);
    while (adventureMissionDisplay.m_waitingOnExternalLock)
      yield return (object) null;
    yield return (object) adventureMissionDisplay.StartCoroutine(adventureMissionDisplay.AnimateWingCompleteBigChests());
    yield return (object) adventureMissionDisplay.StartCoroutine(adventureMissionDisplay.AnimateProgressDisplay());
    yield return (object) adventureMissionDisplay.StartCoroutine(adventureMissionDisplay.AnimateAdventureCompleteCheckmarksAndPopups());
    adventureMissionDisplay.CheckForWingUnlocks();
  }

  public bool HasWingJustAckedRequiredProgress(int wingId, int requiredProgress)
  {
    foreach (AdventureWing bossWing in this.m_BossWings)
    {
      if (bossWing.GetWingId() == (WingDbId) wingId)
        return bossWing.HasJustAckedRequiredProgress(requiredProgress);
    }
    return false;
  }

  public void SetWingHasJustAckedProgress(int wingId, bool hasJustAckedProgress)
  {
    foreach (AdventureWing bossWing in this.m_BossWings)
    {
      if (bossWing.GetWingId() == (WingDbId) wingId)
      {
        bossWing.SetHasJustAckedProgress(hasJustAckedProgress);
        break;
      }
    }
  }

  private IEnumerator UpdateAndAnimateWingCoinsAndChests(
    List<AdventureWing> wings,
    bool scrollToCoin,
    bool forceCoinAnimation)
  {
    this.DisableSelection(true);
    if (AdventureScene.Get().IsInitialScreen())
      yield return (object) new WaitForSeconds(1.8f);
    int num = 0;
    foreach (AdventureWing wing in wings)
    {
      AdventureWing.DelOnCoinAnimateCallback dlg = (AdventureWing.DelOnCoinAnimateCallback) null;
      if (scrollToCoin)
      {
        AdventureWing thisWing = wing;
        dlg = (AdventureWing.DelOnCoinAnimateCallback) (p => thisWing.BringToFocus());
      }
      if (wing.UpdateAndAnimateCoinsAndChests(this.m_CoinFlipDelayTime * (float) num, forceCoinAnimation, dlg))
        ++num;
    }
    if (num > 0)
      yield return (object) new WaitForSeconds(this.m_CoinFlipDelayTime * (float) num + this.m_CoinFlipAnimationTime);
    this.DisableSelection(false);
  }

  private IEnumerator AnimateWingCompleteBigChests()
  {
    AdventureMissionDisplay adventureMissionDisplay1 = this;
    if (adventureMissionDisplay1.m_WingsToGiveBigChest.Count != 0)
    {
      AdventureMissionDisplay adventureMissionDisplay = adventureMissionDisplay1;
      adventureMissionDisplay1.DisableSelection(true);
      if (AdventureScene.Get().IsInitialScreen())
        yield return (object) new WaitForSeconds(1.8f);
      int animDone = 0;
      foreach (AdventureWing adventureWing in adventureMissionDisplay1.m_WingsToGiveBigChest)
      {
        ++animDone;
        adventureWing.m_WingEventTable.AddOpenChestEndEventListener((StateEventTable.StateEventTrigger) (s => --animDone), true);
        adventureWing.OpenBigChest();
      }
      while (animDone > 0)
        yield return (object) null;
      adventureMissionDisplay1.StartCoroutine(adventureMissionDisplay1.PlayWingNotifications());
      List<int> wingIds = new List<int>();
      foreach (AdventureWing adventureWing in adventureMissionDisplay1.m_WingsToGiveBigChest)
      {
        List<AdventureMissionDbfRecord> missionDbfRecordList = ClassChallengeUnlock.AdventureMissionsUnlockedByWingId((int) adventureWing.GetWingId());
        if (missionDbfRecordList != null && missionDbfRecordList.Count > 0)
          wingIds.Add((int) adventureWing.GetWingId());
      }
      if (UserAttentionManager.CanShowAttentionGrabber("AdventureMissionDisplay.ShowFixedRewards"))
      {
        adventureMissionDisplay1.m_WaitingForClassChallengeUnlocks = true;
        PopupDisplayManager.Get().ShowAnyOutstandingPopups((Action) (() => adventureMissionDisplay.ShowClassChallengeUnlock(wingIds)));
      }
      while (adventureMissionDisplay1.m_WaitingForClassChallengeUnlocks)
        yield return (object) null;
      foreach (AdventureWing adventureWing in adventureMissionDisplay1.m_WingsToGiveBigChest)
      {
        AdventureWing nextUnlockedWing = adventureMissionDisplay1.GetNextUnlockedWing(adventureWing.GetWingDef());
        string completeQuotePrefab;
        string completeQuoteVOLine;
        adventureWing.GetCompleteQuoteAssetsFromTargetWingEventTiming((UnityEngine.Object) nextUnlockedWing == (UnityEngine.Object) null ? 0 : (int) nextUnlockedWing.GetWingDef().GetWingId(), out completeQuotePrefab, out completeQuoteVOLine);
        string legacyAssetName = new AssetReference(completeQuoteVOLine).GetLegacyAssetName();
        if (!string.IsNullOrEmpty(completeQuotePrefab) && !string.IsNullOrEmpty(completeQuoteVOLine))
          NotificationManager.Get().CreateCharacterQuote(completeQuotePrefab, GameStrings.Get(legacyAssetName), completeQuoteVOLine);
        adventureWing.BigChestStayOpen();
      }
      adventureMissionDisplay1.m_WingsToGiveBigChest.Clear();
      adventureMissionDisplay1.DisableSelection(false);
    }
  }

  private IEnumerator AnimateProgressDisplay()
  {
    AdventureMissionDisplay adventureMissionDisplay1 = this;
    if ((UnityEngine.Object) adventureMissionDisplay1.m_progressDisplay != (UnityEngine.Object) null)
    {
      while (adventureMissionDisplay1.m_progressDisplay.HasProgressAnimationToPlay())
      {
        AdventureMissionDisplay adventureMissionDisplay = adventureMissionDisplay1;
        adventureMissionDisplay1.m_ScrollBar.SetScroll(0.0f);
        adventureMissionDisplay1.DisableSelection(true);
        bool isAnimComplete = false;
        adventureMissionDisplay1.m_progressDisplay.PlayProgressAnimation((AdventureWingProgressDisplay.OnAnimationComplete) (() =>
        {
          adventureMissionDisplay.DisableSelection(false);
          isAnimComplete = true;
        }));
        while (!isAnimComplete)
          yield return (object) null;
      }
    }
  }

  private void CheckForWingUnlocks()
  {
    foreach (AdventureWing bossWing in this.m_BossWings)
      bossWing.UpdatePlateState();
  }

  private IEnumerator AnimateAdventureCompleteCheckmarksAndPopups(bool forceAnimation = false)
  {
    AdventureMissionDisplay adventureMissionDisplay = this;
    if (adventureMissionDisplay.m_TotalBosses == adventureMissionDisplay.m_TotalBossesDefeated && adventureMissionDisplay.m_BossJustDefeated || forceAnimation)
    {
      List<KeyValuePair<AdventureRewardsChest, float>> chestAnimates = new List<KeyValuePair<AdventureRewardsChest, float>>();
      float num = 0.7f;
      float totalAnimTime = 0.0f;
      List<AdventureWing> adventureWingList = new List<AdventureWing>((IEnumerable<AdventureWing>) adventureMissionDisplay.m_BossWings);
      adventureWingList.Sort(new Comparison<AdventureWing>(adventureMissionDisplay.WingUnlockOrderSortComparison));
      foreach (AdventureWing adventureWing in adventureWingList)
      {
        foreach (AdventureRewardsChest chest in adventureWing.GetChests())
        {
          num *= 0.9f;
          if ((double) num < 0.100000001490116)
            num = 0.1f;
          totalAnimTime += num;
          chestAnimates.Add(new KeyValuePair<AdventureRewardsChest, float>(chest, num));
        }
      }
      adventureMissionDisplay.DisableSelection(true);
      float percentage = 0.0f;
      float endScroll = 1f;
      if ((UnityEngine.Object) adventureMissionDisplay.m_progressDisplay != (UnityEngine.Object) null)
      {
        totalAnimTime -= num;
        percentage = 1f / (float) adventureMissionDisplay.m_BossWings.Count;
      }
      else if (adventureMissionDisplay.m_BossWings[0].GetWingDef().GetUnlockOrder() > adventureMissionDisplay.m_BossWings[adventureMissionDisplay.m_BossWings.Count - 1].GetWingDef().GetUnlockOrder())
      {
        percentage = 1f;
        endScroll = 0.0f;
      }
      adventureMissionDisplay.m_ScrollBar.SetScroll(percentage, iTween.EaseType.easeOutSine, 0.25f, true);
      yield return (object) new WaitForSeconds(0.3f);
      adventureMissionDisplay.m_ScrollBar.SetScroll(endScroll, iTween.EaseType.easeInQuart, totalAnimTime - 0.1f, true);
      foreach (KeyValuePair<AdventureRewardsChest, float> keyValuePair in chestAnimates)
      {
        keyValuePair.Key.BurstCheckmark();
        yield return (object) new WaitForSeconds(keyValuePair.Value);
      }
      adventureMissionDisplay.DisableSelection(false);
      adventureMissionDisplay.ShowAdventureCompletePopup();
      while (adventureMissionDisplay.m_showingAdventureCompletePopup)
        yield return (object) null;
    }
  }

  public void ShowClassChallengeUnlock(List<int> classChallengeUnlocks)
  {
    if (classChallengeUnlocks == null || classChallengeUnlocks.Count == 0)
    {
      this.m_WaitingForClassChallengeUnlocks = false;
    }
    else
    {
      foreach (int classChallengeUnlock in classChallengeUnlocks)
      {
        ++this.m_ClassChallengeUnlockShowing;
        new ClassChallengeUnlockData(classChallengeUnlock).LoadRewardObject((Reward.DelOnRewardLoaded) ((reward, data) =>
        {
          reward.RegisterHideListener((Reward.OnHideCallback) (userData =>
          {
            --this.m_ClassChallengeUnlockShowing;
            if (this.m_ClassChallengeUnlockShowing != 0)
              return;
            this.m_WaitingForClassChallengeUnlocks = false;
          }));
          this.OnRewardObjectLoaded(reward, data);
        }));
      }
    }
  }

  private void ShowAdventureCompletePopup()
  {
    AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
    AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
    this.DisableSelection(true);
    AdventureDef adventureDef = AdventureScene.Get().GetAdventureDef(selectedAdventure);
    AdventureSubDef subDef = adventureDef.GetSubDef(selectedMode);
    switch (adventureDef.m_BannerRewardType)
    {
      case AdventureDef.BannerRewardType.AdventureCompleteReward:
        this.m_showingAdventureCompletePopup = true;
        new AdventureCompleteRewardData(selectedMode, adventureDef.m_BannerRewardPrefab, subDef.GetCompleteBannerText()).LoadRewardObject((Reward.DelOnRewardLoaded) ((reward, data) =>
        {
          reward.RegisterHideListener(new Reward.OnHideCallback(this.AdventureCompletePopupDismissed));
          this.OnRewardObjectLoaded(reward, data);
        }));
        break;
      case AdventureDef.BannerRewardType.BannerManagerPopup:
        this.m_showingAdventureCompletePopup = true;
        BannerManager.Get().ShowBanner(adventureDef.m_BannerRewardPrefab, (string) null, subDef.GetCompleteBannerText(), (BannerManager.DelOnCloseBanner) (() => this.AdventureCompletePopupDismissed((object) null)));
        break;
    }
    if (string.IsNullOrEmpty(adventureDef.m_AdventureCompleteQuotePrefab) || string.IsNullOrEmpty(adventureDef.m_AdventureCompleteQuoteVOLine))
      return;
    string legacyAssetName = new AssetReference(adventureDef.m_AdventureCompleteQuoteVOLine).GetLegacyAssetName();
    NotificationManager.Get().CreateCharacterQuote(adventureDef.m_AdventureCompleteQuotePrefab, GameStrings.Get(legacyAssetName), adventureDef.m_AdventureCompleteQuoteVOLine);
  }

  private void AdventureCompletePopupDismissed(object userData)
  {
    this.m_showingAdventureCompletePopup = false;
    this.DisableSelection(false);
  }

  private void PositionReward(Reward reward) => GameUtils.SetParent((Component) reward, (Component) this.transform);

  private void OnRewardObjectLoaded(Reward reward, object callbackData)
  {
    this.PositionReward(reward);
    reward.Show(false);
  }

  private List<AdventureMissionDisplay.WingCreateParams> BuildWingCreateParamsList()
  {
    AdventureConfig adventureConfig = AdventureConfig.Get();
    AdventureDbId selectedAdventure = adventureConfig.GetSelectedAdventure();
    AdventureModeDbId selectedMode = adventureConfig.GetSelectedMode();
    int adventureDbId = (int) selectedAdventure;
    int modeDbId = (int) selectedMode;
    List<AdventureMissionDisplay.WingCreateParams> paramsList = new List<AdventureMissionDisplay.WingCreateParams>();
    int num = 0;
    foreach (ScenarioDbfRecord record1 in GameDbf.Scenario.GetRecords((Predicate<ScenarioDbfRecord>) (r => r.AdventureId == adventureDbId && r.ModeId == modeDbId)))
    {
      ScenarioDbId id = (ScenarioDbId) record1.ID;
      WingDbId wingId = (WingDbId) record1.WingId;
      int player2HeroCardId = record1.ClientPlayer2HeroCardId;
      if (player2HeroCardId == 0)
        player2HeroCardId = record1.Player2HeroCardId;
      AdventureWingDef wingDef = AdventureScene.Get().GetWingDef(wingId);
      if ((UnityEngine.Object) wingDef == (UnityEngine.Object) null)
      {
        Debug.LogError((object) string.Format("Unable to find wing record for scenario {0} with ID: {1}", (object) id, (object) wingId));
      }
      else
      {
        CardDbfRecord record2 = GameDbf.Card.GetRecord(player2HeroCardId);
        AdventureMissionDisplay.WingCreateParams wingCreateParams = paramsList.Find((Predicate<AdventureMissionDisplay.WingCreateParams>) (currParams => wingId == currParams.m_WingDef.GetWingId()));
        if (wingCreateParams == null)
        {
          wingCreateParams = new AdventureMissionDisplay.WingCreateParams();
          wingCreateParams.m_WingDef = wingDef;
          if ((UnityEngine.Object) wingCreateParams.m_WingDef == (UnityEngine.Object) null)
          {
            Error.AddDevFatal("AdventureDisplay.BuildWingCreateParamsMap() - failed to find a WingDef for adventure {0} wing {1}", (object) selectedAdventure, (object) wingId);
            continue;
          }
          paramsList.Add(wingCreateParams);
        }
        AdventureMissionDisplay.BossCreateParams bossCreateParams = new AdventureMissionDisplay.BossCreateParams();
        bossCreateParams.m_ScenarioRecord = record1;
        bossCreateParams.m_MissionId = id;
        bossCreateParams.m_CardDefId = record2.NoteMiniGuid;
        if (!this.m_BossInfoCache.ContainsKey(id))
        {
          AdventureMissionDisplay.BossInfo bossInfo = new AdventureMissionDisplay.BossInfo()
          {
            m_Title = (string) record1.ShortName,
            m_Description = (string) (!(bool) UniversalInputManager.UsePhoneUI || string.IsNullOrEmpty((string) record1.ShortDescription) ? record1.Description : record1.ShortDescription)
          };
          this.m_BossInfoCache[id] = bossInfo;
        }
        wingCreateParams.m_BossCreateParams.Add(bossCreateParams);
        ++num;
      }
    }
    if (num == 0)
      Debug.LogError((object) string.Format("Unable to find any bosses associated with wing {0} and mode {1}.\nCheck if the scenario DBF has valid entries!", (object) selectedAdventure, (object) selectedMode));
    paramsList.Sort(new Comparison<AdventureMissionDisplay.WingCreateParams>(this.WingCreateParamsSortComparison));
    foreach (AdventureMissionDisplay.WingCreateParams wingCreateParams in paramsList)
      wingCreateParams.m_BossCreateParams.Sort(new Comparison<AdventureMissionDisplay.BossCreateParams>(this.BossCreateParamsSortComparison));
    return paramsList;
  }

  private int WingCreateParamsSortComparison(
    AdventureMissionDisplay.WingCreateParams params1,
    AdventureMissionDisplay.WingCreateParams params2)
  {
    return params1.m_WingDef.GetSortOrder() - params2.m_WingDef.GetSortOrder();
  }

  private int BossCreateParamsSortComparison(
    AdventureMissionDisplay.BossCreateParams params1,
    AdventureMissionDisplay.BossCreateParams params2)
  {
    return GameUtils.MissionSortComparison(params1.m_ScenarioRecord, params2.m_ScenarioRecord);
  }

  private int WingUnlockOrderSortComparison(AdventureWing wing1, AdventureWing wing2) => wing1.GetWingDef().GetUnlockOrder() - wing2.GetWingDef().GetUnlockOrder();

  private void ShowAdventureStore(AdventureWing selectedWing)
  {
    if (SetRotationManager.Get().CheckForSetRotationRollover() || PlayerMigrationManager.Get() != null && PlayerMigrationManager.Get().CheckForPlayerMigrationRequired())
      return;
    StoreManager.Get().StartAdventureTransaction(selectedWing.GetProductType(), selectedWing.GetProductData(), (Store.ExitCallback) null, (object) null, ShopType.ADVENTURE_STORE, 1);
  }

  private void OnStoreShown() => this.DisableSelection(true);

  private void OnStoreHidden() => this.DisableSelection(false);

  private void OnAdventureProgressUpdate(
    bool isStartupAction,
    AdventureMission.WingProgress oldProgress,
    AdventureMission.WingProgress newProgress,
    object userData)
  {
    if ((oldProgress == null ? 0 : (oldProgress.IsOwned() ? 1 : 0)) == (newProgress == null ? (false ? 1 : 0) : (newProgress.IsOwned() ? 1 : 0)))
      return;
    this.StartCoroutine(this.UpdateWingPlateStates());
  }

  private IEnumerator UpdateWingPlateStates()
  {
    while (StoreManager.Get().IsShown())
      yield return (object) null;
    foreach (AdventureWing bossWing in this.m_BossWings)
      bossWing.UpdatePlateState();
  }

  private void ShowRewardsPreview(
    AdventureWing wing,
    int[] scenarioids,
    List<RewardData> wingRewards,
    string wingName)
  {
    if (this.m_ShowingRewardsPreview)
      return;
    if ((UnityEngine.Object) this.m_ClickBlocker != (UnityEngine.Object) null)
      this.m_ClickBlocker.SetActive(true);
    this.m_ShowingRewardsPreview = true;
    this.m_PreviewPane.Reset();
    this.m_PreviewPane.SetHeaderText(wingName);
    List<string> rewardsPreviewCards = wing.GetWingDef().m_SpecificRewardsPreviewCards;
    List<int> previewCardBacks = wing.GetWingDef().m_SpecificRewardsPreviewCardBacks;
    List<BoosterDbId> rewardsPreviewBoosters = wing.GetWingDef().m_SpecificRewardsPreviewBoosters;
    int rewardsPreviewCount = wing.GetWingDef().m_HiddenRewardsPreviewCount;
    int num = rewardsPreviewCards == null ? 0 : (rewardsPreviewCards.Count > 0 ? 1 : 0);
    bool flag1 = previewCardBacks != null && previewCardBacks.Count > 0;
    bool flag2 = rewardsPreviewBoosters != null && rewardsPreviewBoosters.Count > 0;
    if (num != 0)
      this.m_PreviewPane.AddSpecificCards(rewardsPreviewCards);
    if (flag1)
      this.m_PreviewPane.AddSpecificCardBacks(previewCardBacks);
    if (flag2)
      this.m_PreviewPane.AddSpecificBoosters(rewardsPreviewBoosters);
    if (num == 0 && !flag1 && !flag2)
    {
      foreach (int scenarioid in scenarioids)
        this.m_PreviewPane.AddRewardBatch(scenarioid);
      if (wingRewards != null && wingRewards.Count > 0)
        this.m_PreviewPane.AddRewardBatch(wingRewards);
    }
    this.m_PreviewPane.SetHiddenCardCount(rewardsPreviewCount);
    this.m_PreviewPane.Show(true);
  }

  private void OnHideRewardsPreview()
  {
    if ((UnityEngine.Object) this.m_ClickBlocker != (UnityEngine.Object) null)
      this.m_ClickBlocker.SetActive(false);
    this.m_ShowingRewardsPreview = false;
  }

  private void OnStartUnlockPlate(AdventureWing wing)
  {
    if (!wing.ContainsBossCoin(this.m_SelectedCoin))
      this.UnselectBoss();
    this.DisableSelection(true);
  }

  private void OnEndUnlockPlate(AdventureWing wing)
  {
    this.DisableSelection(false);
    if (!string.IsNullOrEmpty(wing.GetWingDef().m_WingOpenPopup))
    {
      AdventureWingOpenBanner adventureWingOpenBanner = GameUtils.LoadGameObjectWithComponent<AdventureWingOpenBanner>(wing.GetWingDef().m_WingOpenPopup);
      if (!((UnityEngine.Object) adventureWingOpenBanner != (UnityEngine.Object) null))
        return;
      adventureWingOpenBanner.ShowBanner((AdventureWingOpenBanner.OnBannerHidden) (() => this.StartCoroutine(this.UpdateAndAnimateProgress(new List<AdventureWing>()
      {
        wing
      }, false))));
    }
    else
      this.StartCoroutine(this.UpdateAndAnimateProgress(new List<AdventureWing>()
      {
        wing
      }, false));
  }

  private void BatchBringWingToFocus(AdventureWing wing)
  {
    if (!this.m_wingsToFocus.Contains(wing))
      this.m_wingsToFocus.Add(wing);
    if (this.m_scheduledBringWingsToFocusCallback != null)
      return;
    this.m_scheduledBringWingsToFocusCallback = this.StartCoroutine(this.WaitThenBringWingsToFocus());
  }

  private IEnumerator WaitThenBringWingsToFocus()
  {
    AdventureMissionDisplay adventureMissionDisplay = this;
    yield return (object) new WaitForEndOfFrame();
    if (adventureMissionDisplay.m_wingsToFocus.Count != 0)
    {
      adventureMissionDisplay.m_wingsToFocus.Sort(new Comparison<AdventureWing>(adventureMissionDisplay.WingUnlockOrderSortComparison));
      adventureMissionDisplay.BringWingToFocus(adventureMissionDisplay.m_wingsToFocus[0]);
      adventureMissionDisplay.m_scheduledBringWingsToFocusCallback = (Coroutine) null;
      adventureMissionDisplay.m_wingsToFocus.Clear();
    }
  }

  private void BringWingToFocus(AdventureWing wing)
  {
    if ((UnityEngine.Object) this.m_ScrollBar == (UnityEngine.Object) null)
      return;
    float positionOffset = 0.0f;
    UIBScrollableItem component = wing.GetComponent<UIBScrollableItem>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      positionOffset = component.m_offset.z * wing.gameObject.transform.lossyScale.z;
    this.m_ScrollBar.CenterObjectInView(wing.gameObject, positionOffset, (UIBScrollable.OnScrollComplete) null, this.m_ScrollBar.m_ScrollEaseType, this.m_ScrollBar.m_ScrollTweenTime, true);
  }

  private IEnumerator RememberLastBossSelection(
    AdventureBossCoin coin,
    ScenarioDbId mission)
  {
    AdventureMissionDisplay adventureMissionDisplay = this;
    while (adventureMissionDisplay.AssetLoadingHelper.AssetsLoading > 0)
      yield return (object) null;
    adventureMissionDisplay.OnBossSelected(coin, mission, false);
  }

  private IEnumerator PlayWingNotifications()
  {
    yield return (object) new WaitForSeconds(3f);
    foreach (AdventureWing adventureWing in this.m_WingsToGiveBigChest)
    {
      if (adventureWing.GetAdventureId() == AdventureDbId.NAXXRAMAS && adventureWing.GetWingId() == WingDbId.NAXX_ARACHNID)
        NotificationManager.Get().CreateKTQuote("VO_KT_MAEXXNA5_50", "VO_KT_MAEXXNA5_50.prefab:71879e77d87e0e745be507be968067bf");
    }
  }

  private void ResumeMainMusic()
  {
    if (this.m_mainMusic == MusicPlaylistType.Invalid)
      return;
    MusicManager.Get().StartPlaylist(this.m_mainMusic);
  }

  private AdventureWing GetNextUnlockedWing(AdventureWingDef currentWingDef)
  {
    AdventureWing nextUnlockedWing = (AdventureWing) null;
    foreach (AdventureWing bossWing in this.m_BossWings)
    {
      if (bossWing.GetWingDef().GetUnlockOrder() > currentWingDef.GetUnlockOrder() && ((UnityEngine.Object) nextUnlockedWing == (UnityEngine.Object) null || bossWing.GetWingDef().GetUnlockOrder() < nextUnlockedWing.GetWingDef().GetUnlockOrder()))
        nextUnlockedWing = bossWing;
    }
    return nextUnlockedWing;
  }

  private void SaveScrollbarValue()
  {
    if (!((UnityEngine.Object) this.m_ScrollBar != (UnityEngine.Object) null) || !((UnityEngine.Object) AdventureConfig.Get() != (UnityEngine.Object) null))
      return;
    this.m_ScrollBar.SaveScroll(AdventureConfig.Get().GetSelectedAdventureAndModeString());
  }

  private void Cheat_OpenNextWing()
  {
    if (!AdventureScene.Get().IsDevMode)
      return;
    AdventureWing adventureWing = (AdventureWing) null;
    foreach (AdventureWing bossWing in this.m_BossWings)
    {
      if (bossWing.m_WingEventTable.IsPlateInOrGoingToAnActiveState() && ((UnityEngine.Object) adventureWing == (UnityEngine.Object) null || bossWing.GetWingDef().GetUnlockOrder() < adventureWing.GetWingDef().GetUnlockOrder()))
        adventureWing = bossWing;
    }
    if (!((UnityEngine.Object) adventureWing != (UnityEngine.Object) null))
      return;
    adventureWing.UnlockPlate();
  }

  private void Cheat_OpenNextChest()
  {
    if (!AdventureScene.Get().IsDevMode)
      return;
    this.m_WingsToGiveBigChest.Clear();
    if (AdventureMissionDisplay.s_cheat_nextWingToGrantChest >= this.m_BossWings.Count)
      AdventureMissionDisplay.s_cheat_nextWingToGrantChest = 0;
    AdventureWing bossWing = this.m_BossWings[AdventureMissionDisplay.s_cheat_nextWingToGrantChest];
    this.m_WingsToGiveBigChest.Add(bossWing);
    this.StartCoroutine(this.UpdateAndAnimateProgress(new List<AdventureWing>()
    {
      bossWing
    }, false, true));
    ++AdventureMissionDisplay.s_cheat_nextWingToGrantChest;
  }

  public bool Cheat_AdventureEvent(string eventName)
  {
    if (!AdventureScene.Get().IsDevMode)
      return false;
    foreach (AdventureWing bossWing in this.m_BossWings)
      bossWing.m_WingEventTable.TriggerState(eventName);
    return true;
  }

  protected class BossCreateParams
  {
    public ScenarioDbfRecord m_ScenarioRecord;
    public ScenarioDbId m_MissionId;
    public string m_CardDefId;
  }

  protected class WingCreateParams
  {
    public AdventureWingDef m_WingDef;
    [CustomEditField(ListTable = true)]
    public List<AdventureMissionDisplay.BossCreateParams> m_BossCreateParams = new List<AdventureMissionDisplay.BossCreateParams>();
  }

  protected class BossInfo
  {
    public string m_Title;
    public string m_Description;
  }

  public enum ProgressStep
  {
    INVALID,
    WING_COINS_AND_CHESTS_UPDATED,
  }

  public delegate void ProgressStepCompletedCallback(AdventureMissionDisplay.ProgressStep progress);
}
