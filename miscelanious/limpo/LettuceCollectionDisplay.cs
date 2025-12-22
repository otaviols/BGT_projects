using Assets;
using Blizzard.T5.Core;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class LettuceCollectionDisplay : CollectibleDisplay
{
  [CustomEditField(Sections = "Bones")]
  public Transform m_setFilterTutorialBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_showMercDetailsTutorialBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_hoverCardTopBone;
  [CustomEditField(Sections = "Bones")]
  public Transform m_hoverCardBottomBone;
  [CustomEditField(Sections = "Objects")]
  public LettuceCollectionPageManager m_pageManager;
  [CustomEditField(Sections = "Objects")]
  public NestedPrefab m_setFilterTrayContainer;
  [CustomEditField(Sections = "Objects")]
  public TooltipZone m_tooltipZone;
  [CustomEditField(Sections = "Objects")]
  public PositionTweenerComponent[] m_tuckTweens;
  [CustomEditField(Sections = "Controls")]
  public Texture m_allSetsTexture;
  [CustomEditField(Sections = "Controls")]
  public UnityEngine.Vector2 m_allSetsIconOffset;
  [CustomEditField(Sections = "Controls")]
  public Texture m_wildSetsTexture;
  [CustomEditField(Sections = "Controls")]
  public UnityEngine.Vector2 m_wildSetsIconOffset;
  [CustomEditField(Sections = "Controls")]
  public Texture m_featuredCardsTexture;
  [CustomEditField(Sections = "Controls")]
  public UnityEngine.Vector2 m_featuredCardsIconOffset;
  [CustomEditField(Sections = "Controls")]
  public float m_deckTrayAbilitySlotTooltipXOffset;
  [CustomEditField(Sections = "Settings")]
  public float m_secondsDelayBeforeTutorialPopups = 1f;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_mercDetailsDisplayReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_mercHoverCardReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_abilityHoverCardReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_mercsPopupReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_campfireButtonReference;
  private Map<TAG_CLASS, Texture> m_loadedClassTextures = new Map<TAG_CLASS, Texture>();
  private Map<TAG_CLASS, CollectibleDisplay.TextureRequests> m_requestedClassTextures = new Map<TAG_CLASS, CollectibleDisplay.TextureRequests>();
  private long m_showTeamContentsRequest;
  private Notification m_helpPopup;
  private Notification m_innkeeperLClickReminder;
  private List<CollectibleDisplay.FilterStateListener> m_setFilterListeners = new List<CollectibleDisplay.FilterStateListener>();
  private Notification m_setFilterTutorialPopup;
  private IEnumerator m_showSetFilterTutorialCoroutine;
  private SpecialEventType m_currentActiveFeaturedCardsEvent;
  private bool m_mercDetailsDisplayFinishedLoading;
  private MercenaryDetailDisplay m_mercenaryDetailDisplay;
  private Widget m_mercHoverCard;
  private Widget m_abilityHoverCard;
  private MercenaryCraftingPopup m_mercCraftingPopup;
  private bool m_catalogueVisible;
  private Coroutine m_ShowCollectionTipsCoroutine;
  private Coroutine m_ShowCampfireButtonCoroutine;
  private Widget m_campfireButton;
  private bool m_isExiting;
  private static readonly string CAMPFIRE_CLICKED_EVENT = "Campfire_Button_Clicked";
  private static readonly string CAMPFIRE_BUTTON_SHOW_EVENT = "SHOW";

  public LettuceCollectionDisplay.ITeamCopyingModule TeamCopying { get; private set; }

  public override void Start()
  {
    NetCache.Get().RegisterScreenCollectionManager(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    CollectionManager.Get().RegisterCollectionNetHandlers();
    CollectionManager.Get().RegisterCollectionLoadedListener(new CollectionManager.DelOnCollectionLoaded(((CollectibleDisplay) this).OnCollectionLoaded));
    CollectionManager.Get().RegisterCollectionChangedListener(new CollectionManager.DelOnCollectionChanged(((CollectibleDisplay) this).OnCollectionChanged));
    CollectionManager.Get().RegisterTeamCreatedListener(new CollectionManager.DelOnTeamCreated(this.OnTeamCreatedByPlayer));
    CollectionManager.Get().RegisterTeamContentsListener(new CollectionManager.DelOnTeamContents(this.OnTeamContents));
    CollectionManager.Get().RegisterNewCardSeenListener(new CollectionManager.DelOnNewCardSeen(this.OnNewCardSeen));
    CollectionManager.Get().RegisterCardRewardsInsertedListener(new CollectionManager.DelOnCardRewardsInserted(this.OnCardRewardsInserted));
    CollectionManager.Get().MercenaryArtVariationChangedEvent += new Action<int, int, TAG_PREMIUM>(this.OnMercArtVariationChanged);
    CardBackManager.Get().SetSearchText((string) null);
    this.m_mercDetailsDisplayReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnMercDetailsDisplayReady));
    this.m_mercHoverCardReference.RegisterReadyListener<Widget>((Action<Widget>) (w => this.m_mercHoverCard = w));
    this.m_abilityHoverCardReference.RegisterReadyListener<Widget>((Action<Widget>) (w => this.m_abilityHoverCard = w));
    this.m_mercsPopupReference.RegisterReadyListener<Widget>((Action<Widget>) (w => this.m_mercCraftingPopup = w.GetComponentInChildren<MercenaryCraftingPopup>()));
    this.m_campfireButtonReference.RegisterReadyListener<Widget>((Action<Widget>) (w =>
    {
      this.m_campfireButton = w;
      this.m_campfireButton?.RegisterEventListener(new Widget.EventListenerDelegate(this.OnCampfireButtonEvent));
    }));
    base.Start();
    bool show = Options.Get().GetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, false);
    this.ShowAdvancedCollectionManager(show);
    if (!show)
      Options.Get().RegisterChangedListener(Option.SHOW_ADVANCED_COLLECTIONMANAGER, new Options.ChangedCallback(this.OnShowAdvancedCMChanged));
    this.DoEnterCollectionManagerEvents();
    if (CollectionManager.Get().ShouldShowWildToStandardTutorial())
      UserAttentionManager.StartBlocking(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
    CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded();
    this.StartCoroutine(this.WaitUntilReady());
  }

  protected override void Awake()
  {
    this.TeamCopying = (LettuceCollectionDisplay.ITeamCopyingModule) new LettuceCollectionDisplay.TeamCopyingModule(this);
    if (ServiceManager.Get<IGraphicsManager>().RenderQualityLevel != GraphicsQuality.Low && PlatformSettings.Memory == MemoryCategory.High && (UnityEngine.Object) this.m_cover == (UnityEngine.Object) null)
    {
      this.m_isBookCoverLoading = true;
      AssetLoader.Get().InstantiatePrefab((AssetReference) "MercenariesBookCover.prefab:a9002069cee6a9a47beb0d2687aa83c5", new PrefabCallback<GameObject>(((CollectibleDisplay) this).OnBookCoverLoaded));
    }
    base.Awake();
    this.StartCoroutine(this.InitCollectionWhenReady());
  }

  protected override void OnDestroy()
  {
    UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
    base.OnDestroy();
  }

  public override CollectiblePageManager GetPageManager() => (CollectiblePageManager) this.m_pageManager;

  public override void Unload()
  {
    this.m_unloading = true;
    this.UnloadAllTextures();
    CollectionDeckTray.Get().GetCardsContent().UnregisterCardTileRightClickedListener(new DeckTrayCardListContent.CardTileRightClicked(this.OnCardTileRightClicked));
    CollectionDeckTray.Get().Unload();
    CollectionInputMgr.Get().Unload();
    this.m_mercenaryDetailDisplay.Unload();
    CollectionManager.Get().MercenaryArtVariationChangedEvent -= new Action<int, int, TAG_PREMIUM>(this.OnMercArtVariationChanged);
    CollectionManager.Get().RemoveCollectionLoadedListener(new CollectionManager.DelOnCollectionLoaded(((CollectibleDisplay) this).OnCollectionLoaded));
    CollectionManager.Get().RemoveCollectionChangedListener(new CollectionManager.DelOnCollectionChanged(((CollectibleDisplay) this).OnCollectionChanged));
    CollectionManager.Get().RemoveTeamCreatedListener(new CollectionManager.DelOnTeamCreated(this.OnTeamCreatedByPlayer));
    CollectionManager.Get().RemoveTeamContentsListener(new CollectionManager.DelOnTeamContents(this.OnTeamContents));
    CollectionManager.Get().RemoveNewCardSeenListener(new CollectionManager.DelOnNewCardSeen(this.OnNewCardSeen));
    CollectionManager.Get().RemoveCardRewardsInsertedListener(new CollectionManager.DelOnCardRewardsInserted(this.OnCardRewardsInserted));
    CollectionManager.Get().RemoveCollectionNetHandlers();
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    Options.Get().UnregisterChangedListener(Option.SHOW_ADVANCED_COLLECTIONMANAGER, new Options.ChangedCallback(this.OnShowAdvancedCMChanged));
    this.m_unloading = false;
  }

  public override void Exit()
  {
    this.m_isExiting = true;
    this.EnableInput(false);
    NotificationManager.Get().DestroyAllPopUps();
    if ((UnityEngine.Object) this.m_pageManager != (UnityEngine.Object) null)
      this.m_pageManager.Exit();
    SceneMgr.Mode nextMode = SceneMgr.Get().GetPrevMode();
    if (!Network.IsLoggedIn() && nextMode != SceneMgr.Mode.HUB)
    {
      DialogManager.Get().ShowReconnectHelperDialog();
      nextMode = SceneMgr.Mode.HUB;
      Navigation.Clear();
    }
    SceneMgr.TransitionHandlerType type = SceneMgr.TransitionHandlerType.NEXT_SCENE;
    if (nextMode == SceneMgr.Mode.LETTUCE_VILLAGE)
      type = SceneMgr.TransitionHandlerType.CURRENT_SCENE;
    this.SetNextModeAndHandleTransition(nextMode, type, SceneMgr.Get().GetScene().GetSceneTransitionPayload());
    LettuceVillagePopupManager.Get().OnPopupClosed -= new Action<LettuceVillagePopupManager.PopupType>(this.OnVillagePopupClosed);
  }

  protected override void OnBookCoverLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    this.m_isBookCoverLoading = false;
    if ((UnityEngine.Object) this.m_root != (UnityEngine.Object) null)
      go.transform.SetParent(this.m_root.transform, false);
    this.m_cover = go.GetComponent<CollectionCoverDisplay>();
    this.m_cover.DisplayCover();
  }

  public override void CollectionPageContentsChanged<TCollectible>(
    ICollection<TCollectible> collectiblesToDisplay,
    CollectibleDisplay.CollectionActorsReadyCallback callback,
    object callbackData)
  {
    Log.CollectionManager.Print("transitionPageId={0} pagesTurning={1}", (object) this.m_pageManager.GetTransitionPageId(), (object) this.m_pageManager.ArePagesTurning());
    bool flag = false;
    if (collectiblesToDisplay == null)
    {
      Log.CollectionManager.Print("collectiblesToDisplay is null!");
      flag = true;
    }
    else if (collectiblesToDisplay.Count == 0)
    {
      Log.CollectionManager.Print("collectiblesToDisplay has a count of 0!");
      flag = true;
    }
    if (!flag)
      return;
    callback(new List<CollectionCardActors>(), new List<ICollectible>(), callbackData);
  }

  public void UpdateCollectionMercenary(LettuceMercenary merc) => this.m_mercenaryDetailDisplay.UpdateMercenaryData(merc);

  public void RequestContentsToShowTeam(long teamID)
  {
    this.m_showTeamContentsRequest = teamID;
    CollectionManager.Get().RequestTeamContents(this.m_showTeamContentsRequest);
  }

  public override void SetViewMode(
    CollectionUtils.ViewMode mode,
    bool triggerResponse,
    CollectionUtils.ViewModeData userdata = null)
  {
    Log.CollectionManager.Print("mode={0}-->{1} triggerResponse={2} isUpdatingTrayMode={3}", (object) this.m_currentViewMode, (object) mode, (object) triggerResponse, (object) CollectionDeckTray.Get().IsUpdatingTrayMode());
    if (this.m_currentViewMode == mode || (mode == CollectionUtils.ViewMode.HERO_SKINS || mode == CollectionUtils.ViewMode.CARD_BACKS) && CollectionDeckTray.Get().IsUpdatingTrayMode())
      return;
    CollectionUtils.ViewMode currentViewMode = this.m_currentViewMode;
    this.m_currentViewMode = mode;
    this.OnSwitchViewModeResponse(triggerResponse, currentViewMode, mode, userdata);
  }

  public void OnDoneEditingTeam()
  {
    this.ShowAppropriateSetFilters();
    this.m_pageManager.OnDoneEditingTeam();
  }

  public override void FilterBySearchText(string newSearchText)
  {
    string text = this.m_search.GetText();
    base.FilterBySearchText(newSearchText);
    this.OnSearchDeactivated_Internal(text, newSearchText, true);
  }

  public override void HideAllTips()
  {
    if ((UnityEngine.Object) this.m_innkeeperLClickReminder != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_innkeeperLClickReminder);
    this.HideHelpPopups();
  }

  public void HideHelpPopups()
  {
    if (this.m_ShowCollectionTipsCoroutine != null)
    {
      this.StopCoroutine(this.m_ShowCollectionTipsCoroutine);
      this.m_ShowCollectionTipsCoroutine = (Coroutine) null;
    }
    if (!((UnityEngine.Object) this.m_helpPopup != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_helpPopup);
  }

  public override void ShowInnkeeperLClickHelp(EntityDef entityDef) => this.ShowInnkeeperLClickHelp(entityDef != null && entityDef.IsHeroSkin());

  private void ShowInnkeeperLClickHelp(bool isHero)
  {
    if (CollectionDeckTray.Get().IsShowingDeckContents())
      return;
    if (isHero)
      this.m_innkeeperLClickReminder = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_LCLICK_HERO"), "", 3f);
    else
      this.m_innkeeperLClickReminder = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_LCLICK"), "", 3f);
  }

  private void FeaturedCardsSetFilterCallback(
    List<TAG_CARD_SET> cardSets,
    List<int> specificCards,
    PegasusShared.FormatType formatType,
    SetFilterItem item,
    bool transitionPage)
  {
    this.SetLastSeenFeaturedCardsEvent(this.m_currentActiveFeaturedCardsEvent, GameSaveKeySubkeyId.COLLECTION_MANAGER_LAST_SEEN_FEATURED_CARDS_EVENT_ITEM);
    item.SetIconFxActive(false);
    this.SetFilterCallback(cardSets, specificCards, formatType, item, transitionPage);
  }

  public override void SetFilterCallback(
    List<TAG_CARD_SET> cardSets,
    List<int> specificCards,
    PegasusShared.FormatType formatType,
    SetFilterItem item,
    bool transitionPage)
  {
    if (formatType != PegasusShared.FormatType.FT_STANDARD)
      Log.CollectionManager.PrintWarning("LettuceCollectionDisplay only supports the Standard format, please add support to the class for other formats if needed.");
    this.m_search.SetWildModeActive(false);
    this.ShowSetFilterCards(cardSets, specificCards, transitionPage);
  }

  private void ShowSetFilterCards(
    List<TAG_CARD_SET> cardSets,
    List<int> specificCards,
    bool transitionPage = true)
  {
    if (specificCards != null)
      this.ShowSpecificCards(specificCards);
    else
      this.ShowSets(cardSets, transitionPage);
  }

  private void ShowSets(List<TAG_CARD_SET> cardSets, bool transitionPage = true)
  {
    this.m_pageManager.FilterByCardSets(cardSets, transitionPage);
    this.NotifyFilterUpdate(this.m_setFilterListeners, cardSets != null, (object) null);
  }

  protected override void ShowSpecificCards(List<int> specificCards)
  {
    base.ShowSpecificCards(specificCards);
    this.NotifyFilterUpdate(this.m_setFilterListeners, specificCards != null, (object) null);
  }

  public override void ResetFilters(bool updateVisuals = true)
  {
    base.ResetFilters(updateVisuals);
    if (!((UnityEngine.Object) this.m_setFilterTray != (UnityEngine.Object) null))
      return;
    this.m_setFilterTray.ClearFilter();
  }

  public void ShowAppropriateSetFilters()
  {
    bool flag1 = this.InCraftingMode();
    if (CollectionManager.Get().IsInEditMode())
    {
      CollectionDeck editedDeck = CollectionManager.Get().GetEditedDeck();
      int num = editedDeck == null ? 0 : (editedDeck.FormatType == PegasusShared.FormatType.FT_WILD ? 1 : 0);
    }
    else
    {
      bool flag2 = AchieveManager.Get().HasUnlockedFeature(Achieve.Unlocks.VANILLA_HEROES);
      if (RankMgr.Get().WildCardsAllowedInCurrentLeague())
      {
        int num1 = CollectionManager.Get().ShouldAccountSeeStandardWild() ? 1 : (flag1 & flag2 ? 1 : 0);
      }
      else
      {
        if (flag1)
          return;
        int num2 = !flag2 ? 0 : (CollectionManager.Get().AccountHasUnlockedWild() ? 1 : 0);
      }
    }
  }

  public void UpdateSetFilters(bool showWild, bool editingDeck, bool showUnownedSets = false) => this.m_setFilterTray.UpdateSetFilters(showWild ? PegasusShared.FormatType.FT_WILD : PegasusShared.FormatType.FT_STANDARD, editingDeck, showUnownedSets);

  private void OnCatalogueButtonReleased(UIEvent e)
  {
    bool enable = !this.m_catalogueVisible;
    this.EnableCatalogue(enable, new BookPageManager.PageTransitionType?(enable ? BookPageManager.PageTransitionType.SINGLE_PAGE_LEFT : BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT));
  }

  public void EnableCatalogue(bool enable, BookPageManager.PageTransitionType? pageTransition = null)
  {
    this.m_catalogueVisible = enable;
    this.m_craftingModeButton.ShowActiveGlow(this.m_catalogueVisible);
    if (enable)
      this.GetPageManager().ShowCardsNotOwned(true, pageTransition);
    else
      this.GetPageManager().ShowOnlyCardsIOwn(pageTransition);
  }

  public void ShowMercenaryDetailsDisplay(LettuceMercenary merc)
  {
    if ((UnityEngine.Object) this.m_mercenaryDetailDisplay == (UnityEngine.Object) null)
      return;
    CollectionDeckTray.Get()?.GetTeamsContent()?.CancelRenameEditingTeam();
    this.HideHelpPopups();
    for (int index = 0; index < this.m_tuckTweens.Length; ++index)
      this.m_tuckTweens[index].PlayForward();
    this.m_pageManager.PlayTabTuckAnimation(true, allowSFX: false);
    this.m_mercenaryDetailDisplay.Show(merc);
  }

  public void OnReturnFromMercenaryDetailsDisplay()
  {
    for (int index = 0; index < this.m_tuckTweens.Length; ++index)
      this.m_tuckTweens[index].PlayReverse();
    this.m_pageManager.PlayTabTuckAnimation(false, allowSFX: false);
    this.TryShowCollectionTips();
  }

  public void HideMercenaryDetailsDisplay()
  {
    if ((UnityEngine.Object) this.m_mercenaryDetailDisplay == (UnityEngine.Object) null || !this.m_mercenaryDetailDisplay.DisplayVisible)
      return;
    this.m_mercenaryDetailDisplay.Hide();
  }

  public MercenaryDetailDisplay GetMercenaryDetailsDisplay() => this.m_mercenaryDetailDisplay;

  public bool IsMercenaryDetailsDisplayActive() => (UnityEngine.Object) this.m_mercenaryDetailDisplay != (UnityEngine.Object) null && this.m_mercenaryDetailDisplay.DisplayVisible;

  public void SlotEquipmentOnActiveMercenary(string cardId) => this.m_mercenaryDetailDisplay?.SlotSelectedEquipment(cardId);

  public void HandleTileHoverEvents(string eventName, VisualController vc)
  {
    if (!(eventName == "MERC_OVER_code"))
    {
      if (!(eventName == "MERC_OUT_code"))
        return;
      this.HideMercHoverCard();
    }
    else
      this.ShowMercHoverCard(vc);
  }

  public void HideHoverCards() => this.HideMercHoverCard();

  public void ShowMercCraftingPopup(LettuceMercenaryDataModel mercData)
  {
    if ((UnityEngine.Object) this.m_mercCraftingPopup == (UnityEngine.Object) null)
    {
      Log.Lettuce.PrintError("LettuceCollectionDisplay.ShowMercCraftingPopup - merc crafting popup is null!");
    }
    else
    {
      CollectionDeckTray.Get()?.GetTeamsContent()?.CancelRenameEditingTeam();
      this.m_mercCraftingPopup.ShowCraftingPopup(mercData);
    }
  }

  public bool TutorialShouldShowAbilityUpgrade() => LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_VISIT_TAVERN_POPUP) && !LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_UPGRADE_ABILITY_END);

  public bool ShouldShowCampfireButton() => (UnityEngine.Object) this.m_campfireButton != (UnityEngine.Object) null && LettuceVillage.TaskboardIsOkayToShowVisitors() && LettuceTutorialUtils.IsEventTypeComplete(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_TUTORIAL_TASK_BOARD_END);

  private void OnTeamContents(long teamID)
  {
    if (teamID == this.m_showTeamContentsRequest)
    {
      this.m_showTeamContentsRequest = 0L;
      this.ShowTeam(teamID, false);
    }
    else
      CollectionDeckTray.Get().GetTeamsContent().OnTeamContentsUpdated(teamID);
  }

  private void OnTeamCreatedByPlayer(long teamID)
  {
    this.ShowTeam(teamID, true);
    this.TeamCopying.CheckClipboardAndPromptPlayerToPaste();
  }

  private void OnNewCardSeen(string cardID, TAG_PREMIUM premium) => this.m_pageManager?.UpdateTabNewCardCounts();

  private void OnCardRewardsInserted(List<string> cardID, List<TAG_PREMIUM> premium) => this.m_pageManager?.RefreshCurrentPageContents();

  protected override void OnCollectionChanged() => this.m_pageManager?.NotifyOfCollectionChanged();

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    if (this.m_sceneTransitionPayload == null)
      return;
    long teamId = ((LettuceVillageDisplay.LettuceSceneTransitionPayload) this.m_sceneTransitionPayload).m_TeamId;
    if (teamId <= 0L || CollectionManager.Get().GetTeam(teamId) == null)
      return;
    this.RequestContentsToShowTeam(teamId);
  }

  private void OnMercArtVariationChanged(
    int mercenaryDbId,
    int artVariationId,
    TAG_PREMIUM premium)
  {
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercenaryDbId);
    LettuceCollectionPageManager pageManager = this.GetPageManager() as LettuceCollectionPageManager;
    if ((UnityEngine.Object) pageManager != (UnityEngine.Object) null)
    {
      pageManager.UpdatePageMercenary(MercenaryFactory.CreateMercenaryDataModelWithCoin(mercenary));
      pageManager.UpdateCurrentPageCardLocks(false);
    }
    CollectionDeckTray.Get().GetTeamsContent().UpdateTeamTrayVisuals(true);
    CollectionDeckTray.Get().GetMercsContent().ChangeMercenaryArtVariation(mercenary.ID, mercenary.GetEquippedArtVariation());
    LettuceMercenaryDataModel displayDataModel = this.GetMercenaryDetailsDisplay().GetMercenaryDisplayDataModel();
    CollectionUtils.PopulateMercenaryCardDataModel(displayDataModel, mercenary.GetEquippedArtVariation());
    CollectionUtils.UpdateMercenaryCardStats(displayDataModel, mercenary);
  }

  protected override bool ShouldStartShown() => SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_VILLAGE;

  private IEnumerator WaitUntilReady()
  {
    LettuceCollectionDisplay collectionDisplay = this;
    while (!collectionDisplay.m_netCacheReady && Network.IsLoggedIn())
      yield return (object) 0;
    while (!collectionDisplay.m_mercDetailsDisplayFinishedLoading || (UnityEngine.Object) collectionDisplay.m_mercHoverCard == (UnityEngine.Object) null || (UnityEngine.Object) collectionDisplay.m_abilityHoverCard == (UnityEngine.Object) null)
      yield return (object) 0;
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    collectionDeckTray.Initialize();
    // ISSUE: reference to a compiler-generated method
    collectionDeckTray.RegisterModeSwitchedListener(new DeckTray.ModeSwitched(collectionDisplay.\u003CWaitUntilReady\u003Eb__93_0));
    collectionDeckTray.GetCardsContent().RegisterCardTileRightClickedListener(new DeckTrayCardListContent.CardTileRightClicked(collectionDisplay.OnCardTileRightClicked));
    LettuceVillagePopupManager.Get().OnPopupClosed += new Action<LettuceVillagePopupManager.PopupType>(collectionDisplay.OnVillagePopupClosed);
    collectionDisplay.m_isReady = true;
  }

  private IEnumerator InitCollectionWhenReady()
  {
    if ((UnityEngine.Object) this.m_pageManager == (UnityEngine.Object) null)
    {
      Log.CollectionManager.PrintError("LettuceCollectionDisplay.InitCollectionWhenReady - m_pageManager null!");
    }
    else
    {
      while (!this.m_pageManager.IsFullyLoaded())
        yield return (object) null;
      this.m_pageManager.OnCollectionLoaded();
    }
  }

  private void OnNetCacheReady()
  {
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Collection.Manager)
    {
      if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
        return;
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
      Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_COLLECTION");
    }
    else
      this.m_netCacheReady = true;
  }

  private void OnShowAdvancedCMChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    bool show = Options.Get().GetBool(Option.SHOW_ADVANCED_COLLECTIONMANAGER, false);
    if (show)
      Options.Get().UnregisterChangedListener(Option.SHOW_ADVANCED_COLLECTIONMANAGER, new Options.ChangedCallback(this.OnShowAdvancedCMChanged));
    this.ShowAdvancedCollectionManager(show);
  }

  private void OnCardTileRightClicked(DeckTrayDeckTileVisual cardTile)
  {
    if (this.GetViewMode() == CollectionUtils.ViewMode.DECK_TEMPLATE)
      return;
    if (!cardTile.GetSlot().Owned)
      CraftingManager.Get().EnterCraftMode((Actor) cardTile.GetActor());
    this.GoToPageWithCard(cardTile.GetCardID(), cardTile.GetPremium());
  }

  private void OnMercDetailsDisplayReady(Widget widget)
  {
    this.m_mercenaryDetailDisplay = widget.GetComponentInChildren<MercenaryDetailDisplay>();
    this.m_mercDetailsDisplayFinishedLoading = true;
  }

  private void OnCampfireButtonEvent(string eventName)
  {
    if (!(eventName == LettuceCollectionDisplay.CAMPFIRE_CLICKED_EVENT))
      return;
    Box box = Box.Get();
    if ((UnityEngine.Object) box != (UnityEngine.Object) null)
    {
      foreach (Collider outerPanelCollider in box.m_outerPanelColliders)
        outerPanelCollider.enabled = false;
    }
    LettuceVillagePopupManager.Get().Show(LettuceVillagePopupManager.PopupType.TASKBOARD);
    this.HideAllTips();
  }

  private void OnVillagePopupClosed(LettuceVillagePopupManager.PopupType type)
  {
    if (type != LettuceVillagePopupManager.PopupType.TASKBOARD)
      return;
    Box box = Box.Get();
    if ((UnityEngine.Object) box != (UnityEngine.Object) null)
    {
      foreach (Collider outerPanelCollider in box.m_outerPanelColliders)
        outerPanelCollider.enabled = true;
    }
    this.TryShowCollectionTips();
  }

  protected override void LoadAllTextures()
  {
  }

  protected override void UnloadAllTextures()
  {
  }

  private void ShowTeam(long teamID, bool isNewTeam, CollectionUtils.ViewMode? setNewViewMode = null)
  {
    if (CollectionManager.Get().GetTeam(teamID) == null)
      return;
    CollectionManager.Get().StartEditingTeam(teamID, (object) isNewTeam);
    CollectionDeckTray.Get().ShowTeam((CollectionUtils.ViewMode) ((int) setNewViewMode ?? (int) this.GetViewMode()));
    CollectionDeckTray.Get().UpdateDoneButtonText();
    if (!setNewViewMode.HasValue)
      return;
    this.SetViewMode(setNewViewMode.Value);
  }

  private void ShowHoverCard(Widget widget, IDataModel dataModel)
  {
    if (CollectionInputMgr.Get().HasHeldCard())
      return;
    widget.BindDataModel(dataModel);
    float z = Mathf.Clamp(PegUI.Get().GetMousedOverElement().transform.position.z, this.m_hoverCardBottomBone.position.z, this.m_hoverCardTopBone.position.z);
    TransformUtil.SetPosZ((Component) widget.transform, z);
  }

  private void ShowMercHoverCard(VisualController vc)
  {
    if (!(WidgetUtils.GetEventDataModel(vc).Payload is LettuceMercenaryDataModel payload))
      return;
    LettuceCollectionDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as LettuceCollectionDisplay;
    if ((UnityEngine.Object) collectibleDisplay != (UnityEngine.Object) null && collectibleDisplay.IsMercenaryDetailsDisplayActive() && collectibleDisplay.GetMercenaryDetailsDisplay().GetCurrentlyDisplayedMercenary().ID == payload.MercenaryId)
      return;
    this.ShowHoverCard(this.m_mercHoverCard, (IDataModel) payload);
  }

  private void HideMercHoverCard() => TransformUtil.SetPosZ((Component) this.m_mercHoverCard, 5000f);

  private IEnumerator DoBookOpeningAnimations()
  {
    LettuceCollectionDisplay collectionDisplay = this;
    while (collectionDisplay.m_isBookCoverLoading)
      yield return (object) null;
    if ((UnityEngine.Object) collectionDisplay.m_cover != (UnityEngine.Object) null)
      collectionDisplay.m_cover.Open(new CollectionCoverDisplay.DelOnOpened(((CollectibleDisplay) collectionDisplay).OnCoverOpened));
    else
      collectionDisplay.OnCoverOpened();
  }

  private IEnumerator SetBookToOpen()
  {
    LettuceCollectionDisplay collectionDisplay = this;
    while (collectionDisplay.m_isBookCoverLoading)
      yield return (object) null;
    if ((UnityEngine.Object) collectionDisplay.m_cover != (UnityEngine.Object) null)
      collectionDisplay.m_cover.SetOpenState();
  }

  private void ShowAdvancedCollectionManager(bool show)
  {
    show |= (bool) UniversalInputManager.UsePhoneUI;
    if ((UnityEngine.Object) this.m_setFilterTray != (UnityEngine.Object) null)
      this.m_setFilterTray.SetButtonShown(show && !(bool) UniversalInputManager.UsePhoneUI);
    if ((UnityEngine.Object) this.m_craftingTray == (UnityEngine.Object) null)
      AssetLoader.Get().LoadGameObject((AssetReference) ((bool) UniversalInputManager.UsePhoneUI ? "MercenariesCraftingTray_phone.prefab:0bd8ce68ce0ff094ba1786d1c62781ea" : "MercenariesCraftingTray.prefab:d0be6526e15d50a46868c8f503298a0b"), new GameObjectCallback(((CollectibleDisplay) this).OnCraftingTrayLoaded), usePrefabPosition: false);
    this.m_craftingModeButton.gameObject.SetActive(true);
    this.m_craftingModeButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCatalogueButtonReleased));
    if ((UnityEngine.Object) this.m_setFilterTray != (UnityEngine.Object) null & show && !this.m_setFilterTrayInitialized)
    {
      this.m_setFilterTray.AddItemUsingTexture(GameStrings.Get("GLUE_COLLECTION_ALL_STANDARD_CARDS"), this.m_allSetsTexture, new UnityEngine.Vector2?(this.m_allSetsIconOffset), new SetFilterItem.ItemSelectedCallback(((CollectibleDisplay) this).SetFilterCallback), new List<TAG_CARD_SET>((IEnumerable<TAG_CARD_SET>) GameUtils.GetStandardSets()), (List<int>) null, PegasusShared.FormatType.FT_STANDARD, true, tooltipHeadline: GameStrings.Get("GLUE_TOOLTIP_HEADER_ALL_STANDARD_CARDS"), tooltipDescription: GameStrings.Get("GLUE_TOOLTIP_DESCRIPTION_ALL_STANDARD_CARDS"));
      this.m_setFilterTray.AddItemUsingTexture(GameStrings.Get("GLUE_COLLECTION_ALL_CARDS"), this.m_wildSetsTexture, new UnityEngine.Vector2?(this.m_wildSetsIconOffset), new SetFilterItem.ItemSelectedCallback(((CollectibleDisplay) this).SetFilterCallback), (List<TAG_CARD_SET>) null, (List<int>) null, PegasusShared.FormatType.FT_WILD, tooltipHeadline: GameStrings.Get("GLUE_TOOLTIP_HEADER_ALL_CARDS"), tooltipDescription: GameStrings.Get("GLUE_TOOLTIP_DESCRIPTION_ALL_CARDS"));
      List<int> featuredCards = CollectionManager.GetFeaturedCards();
      if (featuredCards.Any<int>())
      {
        SetFilterItem setFilterItem = this.m_setFilterTray.AddItemUsingTexture(GameStrings.Get("GLUE_COLLECTION_FEATURED_CARDS"), this.m_featuredCardsTexture, new UnityEngine.Vector2?(this.m_featuredCardsIconOffset), new SetFilterItem.ItemSelectedCallback(this.FeaturedCardsSetFilterCallback), (List<TAG_CARD_SET>) null, featuredCards, PegasusShared.FormatType.FT_STANDARD, tooltipHeadline: GameStrings.Get("GLUE_TOOLTIP_HEADER_FEATURED_CARDS"), tooltipDescription: GameStrings.Get("GLUE_TOOLTIP_DESCRIPTION_FEATURED_CARDS"));
        this.m_currentActiveFeaturedCardsEvent = GameDbf.Card.GetRecord(featuredCards.First<int>()).FeaturedCardsEvent;
        this.StartCoroutine(this.SetIconFxIfFeaturedCardsEventNotSeen(setFilterItem, this.m_currentActiveFeaturedCardsEvent));
        this.StartCoroutine(this.SetFeaturedCardsSetFilterGlowIfNotSeen(this.m_currentActiveFeaturedCardsEvent));
      }
      this.m_setFilterTray.AddHeader(GameStrings.Get("GLUE_COLLECTION_STANDARD_SETS"), PegasusShared.FormatType.FT_STANDARD);
      this.AddSetFilters(false);
      this.m_setFilterTray.AddHeader(GameStrings.Get("GLUE_COLLECTION_WILD_SETS"), PegasusShared.FormatType.FT_WILD);
      this.AddSetFilters(true);
      this.AddSetFilter(TAG_CARD_SET.HOF);
      if (CollectionManager.Get().GetDisplayableCardSets().Contains(TAG_CARD_SET.SLUSH))
        this.AddSetFilter(TAG_CARD_SET.SLUSH);
      this.m_setFilterTray.SelectFirstItem();
      this.m_setFilterTrayInitialized = true;
    }
    else if (!show)
      this.ShowSets(new List<TAG_CARD_SET>((IEnumerable<TAG_CARD_SET>) GameUtils.GetStandardSets()));
    this.ShowAppropriateSetFilters();
    int num = show ? 1 : 0;
  }

  private void AddSetFilters(bool isWild)
  {
    foreach (TAG_CARD_SET cardSet in (IEnumerable<TAG_CARD_SET>) CollectionManager.Get().GetDisplayableCardSets().Where<TAG_CARD_SET>((Func<TAG_CARD_SET, bool>) (cardSetId => cardSetId != TAG_CARD_SET.HOF && cardSetId != TAG_CARD_SET.SLUSH && cardSetId != TAG_CARD_SET.NONE && GameUtils.IsSetRotated(cardSetId) == isWild)).OrderByDescending<TAG_CARD_SET, int>((Func<TAG_CARD_SET, int>) (cardSetId =>
    {
      CardSetDbfRecord cardSet = GameDbf.GetIndex().GetCardSet(cardSetId);
      return cardSet == null ? 0 : cardSet.ReleaseOrder;
    })))
      this.AddSetFilter(cardSet);
  }

  private void AddSetFilter(TAG_CARD_SET cardSet)
  {
    List<TAG_CARD_SET> data = new List<TAG_CARD_SET>();
    data.Add(cardSet);
    string iconTextureAssetRef = (string) null;
    UnityEngine.Vector2? iconOffset = new UnityEngine.Vector2?();
    CardSetDbfRecord cardSet1 = GameDbf.GetIndex().GetCardSet(cardSet);
    if (cardSet1 != null)
    {
      iconTextureAssetRef = cardSet1.FilterIconTexture;
      iconOffset = new UnityEngine.Vector2?(new UnityEngine.Vector2((float) cardSet1.FilterIconOffsetX, (float) cardSet1.FilterIconOffsetY));
    }
    this.m_setFilterTray.AddItem(GameStrings.GetCardSetNameShortened(cardSet), iconTextureAssetRef, iconOffset, new SetFilterItem.ItemSelectedCallback(((CollectibleDisplay) this).SetFilterCallback), data, GameUtils.GetCardSetFormat(cardSet));
  }

  private long GetLastSeenFeaturedCardsEvent(GameSaveKeySubkeyId gameSaveSubkeyId)
  {
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.COLLECTION_MANAGER, gameSaveSubkeyId, out values);
    long featuredCardsEvent = 0;
    if (values != null && values.Any<long>())
      featuredCardsEvent = values.First<long>();
    return featuredCardsEvent;
  }

  private IEnumerator SetIconFxIfFeaturedCardsEventNotSeen(
    SetFilterItem setFilterItem,
    SpecialEventType currentActiveFeaturedCardsEvent)
  {
    LettuceCollectionDisplay collectionDisplay = this;
    while (!collectionDisplay.m_isReady)
      yield return (object) null;
    long featuredCardsEvent = collectionDisplay.GetLastSeenFeaturedCardsEvent(GameSaveKeySubkeyId.COLLECTION_MANAGER_LAST_SEEN_FEATURED_CARDS_EVENT_ITEM);
    long eventIdFromEventName = SpecialEventManager.Get().GetEventIdFromEventName(currentActiveFeaturedCardsEvent);
    if (eventIdFromEventName != -1L && eventIdFromEventName != featuredCardsEvent)
      setFilterItem.SetIconFxActive(true);
  }

  private IEnumerator SetFeaturedCardsSetFilterGlowIfNotSeen(
    SpecialEventType currentActiveFeaturedCardsEvent)
  {
    LettuceCollectionDisplay collectionDisplay = this;
    while (!collectionDisplay.m_isReady)
      yield return (object) null;
    long featuredCardsEvent = collectionDisplay.GetLastSeenFeaturedCardsEvent(GameSaveKeySubkeyId.COLLECTION_MANAGER_LAST_SEEN_FEATURED_CARDS_EVENT_BUTTON);
    long eventIdFromEventName = SpecialEventManager.Get().GetEventIdFromEventName(currentActiveFeaturedCardsEvent);
    if (eventIdFromEventName != -1L && eventIdFromEventName != featuredCardsEvent)
    {
      collectionDisplay.m_setFilterTray.SetFilterButtonGlowActive(true);
      if ((UnityEngine.Object) collectionDisplay.m_filterButtonGlow != (UnityEngine.Object) null)
        collectionDisplay.m_filterButtonGlow.SetActive(true);
    }
  }

  private void SetLastSeenFeaturedCardsEvent(
    SpecialEventType currentActiveFeaturedCardsEvent,
    GameSaveKeySubkeyId subkeyId)
  {
    if (currentActiveFeaturedCardsEvent == SpecialEventType.UNKNOWN)
      return;
    long eventIdFromEventName = SpecialEventManager.Get().GetEventIdFromEventName(currentActiveFeaturedCardsEvent);
    if (eventIdFromEventName == -1L || this.GetLastSeenFeaturedCardsEvent(subkeyId) == eventIdFromEventName)
      return;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.COLLECTION_MANAGER, subkeyId, new long[1]
    {
      eventIdFromEventName
    }));
  }

  protected override void OnSearchDeactivated(string oldSearchText, string newSearchText) => this.OnSearchDeactivated_Internal(oldSearchText, newSearchText, true);

  private void OnSearchDeactivated_Internal(
    string oldSearchText,
    string newSearchText,
    bool updateManaFilterToMatchSearchText)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.EnableInput(true);
    if (oldSearchText == newSearchText)
    {
      this.OnSearchFilterComplete();
    }
    else
    {
      if (((IEnumerable<string>) newSearchText.ToLower().Split(CollectibleFilteredSet<ICollectible>.SearchTokenDelimiters, StringSplitOptions.RemoveEmptyEntries)).Contains<string>(GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MISSING")))
      {
        this.EnableCatalogue(true);
        this.m_searchTriggeredCrafting = true;
      }
      else
        this.ResetFilterSettingsFromSearch();
      this.NotifyFilterUpdate(this.m_searchFilterListeners, !string.IsNullOrEmpty(newSearchText), (object) newSearchText);
      this.m_pageManager.ChangeSearchTextFilter(newSearchText, new BookPageManager.DelOnPageTransitionComplete(((CollectibleDisplay) this).OnSearchFilterComplete), (object) null);
    }
  }

  protected override void OnSearchCleared(bool transitionPage)
  {
    this.ResetFilterSettingsFromSearch();
    this.NotifyFilterUpdate(this.m_searchFilterListeners, false, (object) "");
    this.m_pageManager.ChangeSearchTextFilter("", transitionPage);
    base.OnSearchCleared(transitionPage);
  }

  private void ResetFilterSettingsFromSearch()
  {
    if (this.m_searchTriggeredCrafting)
      this.EnableCatalogue(false);
    this.m_searchTriggeredCrafting = false;
  }

  private void DoEnterCollectionManagerEvents()
  {
    if (CollectionManager.Get().HasVisitedCollection() || SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.LETTUCE_BOUNTY_BOARD)
    {
      this.EnableInput(true);
      this.OpenBookImmediately();
    }
    else
    {
      CollectionManager.Get().SetHasVisitedCollection(true);
      this.EnableInput(false);
      this.StartCoroutine(this.OpenBookWhenReady());
    }
  }

  private void OpenBookImmediately()
  {
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.LETTUCE_COLLECTION)
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.MERCENARIES_COLLECTION);
    this.StartCoroutine(this.SetBookToOpen());
    this.TryShowCollectionTips();
    this.TryShowCampfireButton();
  }

  private IEnumerator OpenBookWhenReady()
  {
    LettuceCollectionDisplay collectionDisplay = this;
    while (SceneMgr.Get().IsTransitioning())
      yield return (object) null;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.LETTUCE_COLLECTION)
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.MERCENARIES_COLLECTION);
    collectionDisplay.m_pageManager.OnBookOpening();
    collectionDisplay.StartCoroutine(collectionDisplay.DoBookOpeningAnimations());
    collectionDisplay.TryShowCollectionTips();
    collectionDisplay.TryShowCampfireButton();
  }

  public void TryShowCollectionTips()
  {
    if (this.m_ShowCollectionTipsCoroutine != null)
    {
      this.StopCoroutine(this.m_ShowCollectionTipsCoroutine);
      this.m_ShowCollectionTipsCoroutine = (Coroutine) null;
    }
    this.m_ShowCollectionTipsCoroutine = this.StartCoroutine(this.ShowCollectionTipsIfNeeded());
  }

  public void TryShowCampfireButton()
  {
    if (this.m_ShowCampfireButtonCoroutine != null)
    {
      this.StopCoroutine(this.m_ShowCampfireButtonCoroutine);
      this.m_ShowCampfireButtonCoroutine = (Coroutine) null;
    }
    this.m_ShowCampfireButtonCoroutine = this.StartCoroutine(this.ShowCampfireButtonIfNeeded());
  }

  private IEnumerator ShowCollectionTipsIfNeeded()
  {
    LettuceCollectionDisplay collectionDisplay = this;
    while (CollectionManager.Get().IsWaitingForBoxTransition())
      yield return (object) null;
    yield return (object) new WaitForSeconds(collectionDisplay.m_secondsDelayBeforeTutorialPopups);
    if (!collectionDisplay.m_isExiting)
    {
      LettuceCollectionPageManager lcpm = collectionDisplay.GetPageManager() as LettuceCollectionPageManager;
      while (!lcpm.IsFullyLoaded())
        yield return (object) null;
      if (collectionDisplay.TutorialShouldShowAbilityUpgrade() && UserAttentionManager.CanShowAttentionGrabber("LettuceCollectionDisplay.ShowCollectionTipsIfNeeded:HAS_SEEN_SHOW_MERC_DETAILS_TUTORIAL"))
      {
        collectionDisplay.m_helpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, collectionDisplay.m_showMercDetailsTutorialBone.position, collectionDisplay.m_showMercDetailsTutorialBone.localScale, GameStrings.Get("GLUE_LETTUCE_COLLECTION_TUTORIAL01"));
        collectionDisplay.m_helpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
        collectionDisplay.m_helpPopup.PulseReminderEveryXSeconds(3f);
        for (int index = 0; index < collectionDisplay.m_tuckTweens.Length; ++index)
          collectionDisplay.m_tuckTweens[index].PlayForward();
      }
      else
        collectionDisplay.AttemptToShowAppearceTip();
    }
  }

  private IEnumerator ShowCampfireButtonIfNeeded()
  {
    while (!this.m_campfireButtonReference.IsReady)
      yield return (object) null;
    if (!this.m_isExiting && this.ShouldShowCampfireButton())
      this.m_campfireButton.TriggerEvent(LettuceCollectionDisplay.CAMPFIRE_BUTTON_SHOW_EVENT);
  }

  public bool CanShowAppearanceTip(bool checkForMercOnPage = true)
  {
    if (Options.Get().GetBool(Option.HAS_SEEN_MERC_APPEARANCE_TUTORIAL, false) || !UserAttentionManager.CanShowAttentionGrabber("LettuceCollectionDisplay.ShowCollectionTipsIfNeeded:" + (object) Option.HAS_SEEN_MERC_APPEARANCE_TUTORIAL))
      return false;
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary(18L);
    if (!mercenary.HasUnlockedGoldenOrBetter())
      return false;
    if (checkForMercOnPage)
    {
      LettuceCollectionPageManager pageManager = this.GetPageManager() as LettuceCollectionPageManager;
      if ((UnityEngine.Object) pageManager == (UnityEngine.Object) null || pageManager.GetMercenaryOnPage(mercenary.ID) == null)
        return false;
    }
    return true;
  }

  protected bool AttemptToShowAppearceTip()
  {
    if (!this.CanShowAppearanceTip())
      return false;
    this.m_helpPopup = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, this.m_showMercDetailsTutorialBone.position, this.m_showMercDetailsTutorialBone.localScale, GameStrings.Get("GLUE_LETTUCE_COLLECTION_TUTORIAL_PORTRAIT_01"));
    this.m_helpPopup.ShowPopUpArrow(Notification.PopUpArrowDirection.Left);
    this.m_helpPopup.PulseReminderEveryXSeconds(3f);
    return true;
  }

  protected override void OnSwitchViewModeResponse(
    bool triggerResponse,
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode newMode,
    CollectionUtils.ViewModeData userdata)
  {
    base.OnSwitchViewModeResponse(triggerResponse, prevMode, newMode, userdata);
    this.EnableSetAndManaFiltersByViewMode(newMode);
  }

  private void EnableSetAndManaFiltersByViewMode(CollectionUtils.ViewMode viewMode) => this.EnableSetAndManaFilters(viewMode == CollectionUtils.ViewMode.CARDS);

  private void EnableSetAndManaFilters(bool enabled)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_craftingModeButton.Enable(enabled);
    if ((UnityEngine.Object) this.m_setFilterTray != (UnityEngine.Object) null)
    {
      this.m_setFilterTray.SetButtonEnabled(enabled);
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.m_setFilterTray.gameObject.SetActive(enabled);
    }
    this.m_search.SetEnabled(true);
  }

  private void OnSetFilterButtonPressed()
  {
    this.SetLastSeenFeaturedCardsEvent(this.m_currentActiveFeaturedCardsEvent, GameSaveKeySubkeyId.COLLECTION_MANAGER_LAST_SEEN_FEATURED_CARDS_EVENT_BUTTON);
    this.m_setFilterTray.SetFilterButtonGlowActive(false);
  }

  private void OnPhoneFilterButtonPressed()
  {
    this.SetLastSeenFeaturedCardsEvent(this.m_currentActiveFeaturedCardsEvent, GameSaveKeySubkeyId.COLLECTION_MANAGER_LAST_SEEN_FEATURED_CARDS_EVENT_BUTTON);
    this.m_filterButtonGlow.SetActive(false);
  }

  protected override CraftingTrayBase GetCraftingTrayComponent(GameObject go) => (CraftingTrayBase) go.GetComponent<MercenariesCraftingTray>();

  protected override void OnCraftingTrayLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    base.OnCraftingTrayLoaded(assetRef, go, callbackData);
  }

  public override void ShowCraftingTray(
    bool? includeCraftable = null,
    bool? showOnlyPromotable = null,
    bool? unused1 = null,
    bool? unused2 = null,
    bool? unused3 = null,
    bool updatePage = true)
  {
    CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
    if ((UnityEngine.Object) collectionDeckTray != (UnityEngine.Object) null)
    {
      DeckTrayTeamListContent teamsContent = collectionDeckTray.GetTeamsContent();
      if ((UnityEngine.Object) teamsContent != (UnityEngine.Object) null)
        teamsContent.CancelRenameEditingTeam();
    }
    base.ShowCraftingTray(includeCraftable, showOnlyPromotable, updatePage: updatePage);
    this.ShowAppropriateSetFilters();
  }

  protected override CollectionUtils.ViewMode GetInitialViewMode() => CollectionUtils.ViewMode.CARDS;

  public override void HideCraftingTray()
  {
    base.HideCraftingTray();
    this.ShowAppropriateSetFilters();
  }

  public void Dev_ShowTutorialPopups()
  {
    if (this.IsMercenaryDetailsDisplayActive())
    {
      this.GetMercenaryDetailsDisplay().Dev_ShowTutorialPopups();
    }
    else
    {
      foreach (Transform transform in new List<Transform>()
      {
        this.m_setFilterTutorialBone,
        this.m_showMercDetailsTutorialBone
      })
        NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, transform.position, transform.localScale, transform.name);
    }
  }

  public interface ITeamCopyingModule
  {
    void CheckClipboardAndPromptPlayerToPaste();
  }

  public class TeamCopyingModule : LettuceCollectionDisplay.ITeamCopyingModule
  {
    private LettuceCollectionDisplay m_display;
    private ShareableMercenariesTeam m_cachedShareableTeam;

    private bool IsInEditingMode => CollectionManager.Get().IsInEditTeamMode();

    private bool IsCorrectMode => SceneMgr.Get().GetMode() == SceneMgr.Mode.LETTUCE_COLLECTION;

    private LettuceTeam EditedTeam => CollectionManager.Get().GetEditingTeam();

    public TeamCopyingModule(LettuceCollectionDisplay display) => this.m_display = display;

    public void CheckClipboardAndPromptPlayerToPaste()
    {
      if (!this.CheckIfClipboardNotificationHasBeenShown() || this.m_display.m_mercenaryDetailDisplay.DisplayVisible)
        return;
      string message;
      if (!this.TryCacheClipboardDataAndGetValidityMessaging(out message))
      {
        if (!(message != string.Empty))
          return;
        this.AlertPlayerOnInvalidPaste(message);
      }
      else
      {
        string str1 = GameStrings.Get("GLUE_COLLECTION_TEAM_VALID_PASTE_BODY");
        string str2 = GameStrings.Get("GLUE_COLLECTION_TEAM_VALID_PASTE_HEADER");
        if (this.IsInEditingMode && this.EditedTeam.GetMercCount() > 0)
        {
          str1 = GameStrings.Get("GLUE_COLLECTION_TEAM_OVERWRITE_BODY");
          str2 = GameStrings.Get("GLUE_COLLECTION_TEAM_OVERWRITE_HEADER");
        }
        AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
        {
          m_headerText = str2,
          m_text = str1,
          m_cancelText = GameStrings.Get("GLUE_COLLECTION_TEAM_SAVE_ANYWAY"),
          m_confirmText = GameStrings.Get("GLUE_COLLECTION_TEAM_FINISH_FOR_ME"),
          m_showAlertIcon = false,
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
          {
            if (response == AlertPopup.Response.CANCEL)
              this.RejectTeamFromClipboard();
            else
              this.CreateOrUpdateExistingTeamFromClipboard(this.m_cachedShareableTeam);
          })
        };
        DialogManager.Get().ShowPopup(info);
      }
    }

    private bool CheckIfClipboardNotificationHasBeenShown()
    {
      if (PlatformSettings.OS != OSCategory.iOS || Options.Get().GetBool(Option.HAS_SEEN_CLIPBOARD_NOTIFICATION, false))
        return true;
      DialogManager dialogManager = DialogManager.Get();
      if (dialogManager.ShowingDialog())
        return false;
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_COLLECTION_TEAM_CLIPBOARD_ACCESS_HEADER"),
        m_text = GameStrings.Get("GLUE_COLLECTION_TEAM_CLIPBOARD_ACCESS_BODY"),
        m_showAlertIcon = false,
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => Options.Get().SetBool(Option.HAS_SEEN_CLIPBOARD_NOTIFICATION, true))
      };
      dialogManager.ShowPopup(info);
      return false;
    }

    private bool TryCacheClipboardDataAndGetValidityMessaging(out string message)
    {
      message = string.Empty;
      ShareableMercenariesTeam shareableTeam = ShareableMercenariesTeam.DeserializeFromClipboard();
      if (shareableTeam == null)
        return false;
      DialogManager dialogManager = DialogManager.Get();
      if (dialogManager.ShowingDialog())
      {
        if (this.m_cachedShareableTeam != null && this.m_cachedShareableTeam.Equals((object) shareableTeam) || !this.CanPasteShareableTeam(shareableTeam))
          return false;
        dialogManager.ClearAllImmediately();
      }
      if (!this.CanPasteShareableTeam(shareableTeam, out message))
        return false;
      this.m_cachedShareableTeam = shareableTeam;
      return true;
    }

    private bool CanPasteShareableTeam(ShareableMercenariesTeam shareableTeam) => this.CanPasteShareableTeam(shareableTeam, out string _);

    private bool CanPasteShareableTeam(
      ShareableMercenariesTeam shareableTeam,
      out string alertMessage)
    {
      alertMessage = string.Empty;
      return (!this.IsCorrectMode || this.IsInEditingMode || CollectionDeckTray.Get().GetTeamsContent().CanShowNewTeamButton()) && (!((UnityEngine.Object) CraftingTray.Get() != (UnityEngine.Object) null) || !CraftingTray.Get().IsShown());
    }

    private void CreateOrUpdateExistingTeamFromClipboard(ShareableMercenariesTeam shareableTeam)
    {
      if (!this.IsInEditingMode)
      {
        CollectionManager collectionManager = CollectionManager.Get();
        collectionManager.RegisterTeamCreatedListener(new CollectionManager.DelOnTeamCreated(this.OnTeamCreatedFromClipboard));
        collectionManager.RemoveTeamCreatedListener(new CollectionManager.DelOnTeamCreated(this.m_display.OnTeamCreatedByPlayer));
        CollectionDeckTray.Get().GetTeamsContent().CreateNewTeam(shareableTeam.Team.Name, shareableTeam.Serialize(false));
      }
      else
        this.OnTeamCreatedFromClipboard(this.EditedTeam.ID);
    }

    private void OnTeamCreatedFromClipboard(long teamId)
    {
      CollectionManager collectionManager = CollectionManager.Get();
      collectionManager.RemoveTeamCreatedListener(new CollectionManager.DelOnTeamCreated(this.OnTeamCreatedFromClipboard));
      collectionManager.RegisterTeamCreatedListener(new CollectionManager.DelOnTeamCreated(this.m_display.OnTeamCreatedByPlayer));
      CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
      if (collectionDeckTray.GetCurrentContentType() != DeckTray.DeckContentTypes.Mercs)
      {
        collectionDeckTray.RegisterModeSwitchedListener(new DeckTray.ModeSwitched(this.OnCollectionDeckTrayModeSwitched));
        this.m_display.ShowTeam(teamId, true);
      }
      else
        this.PasteContentsIntoDeckTray();
    }

    private void OnCollectionDeckTrayModeSwitched()
    {
      CollectionDeckTray.Get().UnregisterModeSwitchedListener(new DeckTray.ModeSwitched(this.OnCollectionDeckTrayModeSwitched));
      this.PasteContentsIntoDeckTray();
    }

    private void PasteContentsIntoDeckTray()
    {
      if (this.m_cachedShareableTeam != null)
      {
        this.PasteTeamInEditModeFromShareableTeamInternal(this.m_cachedShareableTeam);
      }
      else
      {
        ShareableMercenariesTeam shareableTeam = ShareableMercenariesTeam.DeserializeFromClipboard();
        if (shareableTeam == null)
          return;
        this.PasteTeamInEditModeFromShareableTeamInternal(shareableTeam);
      }
      ClipboardUtils.CopyToClipboard(string.Empty);
      this.m_cachedShareableTeam = (ShareableMercenariesTeam) null;
    }

    private void RejectTeamFromClipboard()
    {
      ClipboardUtils.CopyToClipboard(string.Empty);
      this.m_cachedShareableTeam = (ShareableMercenariesTeam) null;
    }

    private void PasteTeamInEditModeFromShareableTeamInternal(ShareableMercenariesTeam shareableTeam)
    {
      if (!this.IsInEditingMode)
      {
        Debug.LogError((object) "Error trying to paste team. Collection Manager is not in edit mode.");
      }
      else
      {
        if (this.m_display.GetMercenaryDetailsDisplay().DisplayVisible)
          return;
        LettuceTeam editedTeam = this.EditedTeam;
        CollectionDeckTray collectionDeckTray = CollectionDeckTray.Get();
        CollectionManager collectionManager = CollectionManager.Get();
        string name = shareableTeam.Team.Name;
        if (!string.IsNullOrEmpty(name))
        {
          editedTeam.Name = name;
          collectionDeckTray.GetTeamsContent().UpdateTeamName(name);
        }
        DefLoader defLoader = DefLoader.Get();
        List<LettuceCollectionDisplay.TeamCopyingModule.TeamFill> fillCards = new List<LettuceCollectionDisplay.TeamCopyingModule.TeamFill>();
        foreach (LettuceMercenary merc in shareableTeam.Team.GetMercs())
        {
          LettuceMercenary mercenary = collectionManager.GetMercenary((long) merc.ID);
          if (mercenary != null && mercenary.m_owned)
          {
            LettuceMercenary.Loadout loadout1 = shareableTeam.Team.GetLoadout(merc);
            LettuceMercenary.Loadout loadout2 = new LettuceMercenary.Loadout();
            if (loadout1.m_equipmentRecord != null)
            {
              List<LettuceEquipmentTierDbfRecord> lettuceEquipmentTiers = loadout1.m_equipmentRecord.LettuceEquipmentTiers;
              for (int index = lettuceEquipmentTiers.Count - 1; index >= 0; --index)
              {
                LettuceEquipmentTierDbfRecord equipmentTierDbfRecord = lettuceEquipmentTiers[index];
                LettuceEquipmentDbfRecord record = GameDbf.LettuceEquipment.GetRecord(equipmentTierDbfRecord.LettuceEquipmentId);
                if (mercenary.CanSlotEquipment(record.ID))
                {
                  loadout2.m_equipmentRecord = record;
                  break;
                }
              }
            }
            if (loadout1.m_artVariationRecord != null && mercenary.IsArtVariationUnlocked(loadout1.m_artVariationRecord.ID, loadout1.m_artVariationPremium))
            {
              loadout2.m_artVariationRecord = loadout1.m_artVariationRecord;
              loadout2.m_artVariationPremium = loadout1.m_artVariationPremium;
            }
            else
            {
              LettuceMercenary.ArtVariation availableArtVariation = mercenary.GetDefaultOrFirstAvailableArtVariation();
              loadout2.m_artVariationRecord = availableArtVariation.m_record;
              loadout2.m_artVariationPremium = availableArtVariation.m_premium;
            }
            fillCards.Add(new LettuceCollectionDisplay.TeamCopyingModule.TeamFill()
            {
              m_addCard = defLoader.GetEntityDef(mercenary.GetCardId()),
              m_addLoadout = loadout2
            });
          }
        }
        collectionDeckTray.PopulateTeam((IEnumerable<LettuceCollectionDisplay.TeamCopyingModule.TeamFill>) fillCards, (CollectionDeckTray.PopuplateDeckCompleteCallback) null);
      }
    }

    private void AlertPlayerOnInvalidPaste(string errorReason)
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_COLLECTION_TEAM_INVALID_POPUP_HEADER"),
        m_text = errorReason,
        m_okText = GameStrings.Get("GLOBAL_OKAY"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
      DialogManager.Get().ShowPopup(info);
    }

    public struct TeamFill
    {
      public EntityDef m_addCard;
      public LettuceMercenary.Loadout m_addLoadout;
      public string m_reason;
    }
  }
}
