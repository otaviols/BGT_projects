using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class BaconCollectionDisplay : CollectibleDisplay
{
  [CustomEditField(Sections = "Bones")]
  public Transform m_setFilterTutorialBone;
  [CustomEditField(Sections = "Objects")]
  public BaconCollectionPageManager m_pageManager;
  [CustomEditField(Sections = "Objects")]
  public NestedPrefab m_setFilterTrayContainer;
  [CustomEditField(Sections = "Objects")]
  public BaconCollectionFilterButton m_baconFilterButton;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_boardDetailsDisplayReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_finisherDetailsDisplayReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_emoteDetailsDisplayReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_emoteLayoutDisplayReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_emoteTrayReference;
  [CustomEditField(Sections = "Widgets")]
  public AsyncReference m_emotePreviewButtonReference;
  private Notification m_innkeeperLClickReminder;
  private List<CollectibleDisplay.FilterStateListener> m_setFilterListeners = new List<CollectibleDisplay.FilterStateListener>();
  private List<CollectibleDisplay.FilterStateListener> m_manaFilterListeners = new List<CollectibleDisplay.FilterStateListener>();
  private ShareableDeck m_cachedShareableDeck;
  private CollectionUtils.BattlegroundsHeroSkinFilterMode m_heroSkinFilterMode;
  private bool m_searchTriggeredHeroSkinFilter;
  private bool m_boardDetailsDisplayFinishedLoading;
  private BaconBoardCollectionDetails m_boardDetailsDisplay;
  private bool m_finisherDetailsDisplayFinishedLoading;
  private BaconFinisherCollectionDetails m_finisherDetailsDisplay;
  private bool m_emoteDetailsDisplayFinishedLoading;
  private BaconEmoteCollectionDetails m_emoteDetailsDisplay;
  private bool m_emoteLayoutDisplayFinishedLoading;
  private BaconEmoteCollectionLayout m_emoteLayoutDisplay;
  private bool m_emoteTrayFinishedLoading;
  private BaconEmoteTray m_emoteTray;
  private UIBButton m_emoteLayoutDisplayButton;

  public override void Start()
  {
    NetCache.Get().RegisterScreenCollectionManager(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    CollectionManager.Get().RegisterCollectionNetHandlers();
    CollectionManager.Get().RegisterCollectionLoadedListener(new CollectionManager.DelOnCollectionLoaded(((CollectibleDisplay) this).OnCollectionLoaded));
    CollectionManager.Get().RegisterCollectionChangedListener(new CollectionManager.DelOnCollectionChanged(((CollectibleDisplay) this).OnCollectionChanged));
    CollectionManager.Get().RegisterCardRewardsInsertedListener(new CollectionManager.DelOnCardRewardsInserted(this.OnCardRewardsInserted));
    CollectionManager.Get().RegisterNewCardSeenListener(new CollectionManager.DelOnNewCardSeen(this.OnNewCardSeen));
    CardBackManager.Get().SetSearchText((string) null);
    Navigation.Push(new Navigation.NavigateBackHandler(this.OnBackOutOfCollectionScreen));
    this.m_boardDetailsDisplayReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnBoardDetailsDisplayReady));
    this.m_finisherDetailsDisplayReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnFinisherDetailsDisplayReady));
    this.m_emoteDetailsDisplayReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnEmoteDetailsDisplayReady));
    this.m_emoteLayoutDisplayReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnEmoteLayoutDisplayReady));
    this.m_emoteTrayReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnEmoteTrayReady));
    this.m_emotePreviewButtonReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnEmotePreviewButtonReady));
    base.Start();
    this.m_pageManager.ShowHeroSkins();
    this.DoEnterCollectionManagerEvents();
    MusicManager.Get().StartPlaylist(MusicPlaylistType.CollectionManager_Battlegrounds);
    CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded();
    this.StartCoroutine(this.WaitUntilReady());
  }

  public CollectionUtils.BattlegroundsHeroSkinFilterMode GetHeroSkinFilterMode() => this.m_heroSkinFilterMode;

  public void ToggleHeroSkinFilterMode()
  {
    int num = (int) (this.m_heroSkinFilterMode + 1);
    this.m_heroSkinFilterMode = num < 2 ? (CollectionUtils.BattlegroundsHeroSkinFilterMode) num : CollectionUtils.BattlegroundsHeroSkinFilterMode.DEFAULT;
    this.m_pageManager.UpdateHeroSkinsFilterType();
  }

  public bool TryCheckEmoteInLoadout(int emoteId, out bool inLoadout)
  {
    if ((UnityEngine.Object) this.m_emoteTray == (UnityEngine.Object) null || !this.m_emoteTray.IsLoadoutValid())
    {
      inLoadout = false;
      return false;
    }
    inLoadout = this.m_emoteTray.IsEmoteInLoadout(emoteId);
    return true;
  }

  protected override void Awake()
  {
    HearthstonePerformance hearthstonePerformance = HearthstonePerformance.Get();
    if (hearthstonePerformance != null)
      hearthstonePerformance.StartPerformanceFlow(new FlowPerformance.SetupConfig()
      {
        FlowType = Blizzard.Telemetry.WTCG.Client.FlowPerformance.FlowType.COLLECTION_MANAGER
      });
    base.Awake();
    this.StartCoroutine(this.InitCollectionWhenReady());
  }

  private BattlegroundsEmoteLoadoutDataModel GetOrCreateEmoteLoadoutDataModel() => this.m_emoteTray.GetLoadoutDataModel() ?? CollectionManager.Get().CreateEmoteLoadoutDataModel();

  public void SetEmoteLoadout(BattlegroundsEmoteLoadoutDataModel dataModel)
  {
    this.m_emoteTray.SetLoadoutDataModel(dataModel);
    this.m_emoteTray.UpdateImageWidgetVisibility(dataModel);
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
    NotificationManager.Get().DestroyAllPopUps();
    this.UnloadAllTextures();
    CollectionInputMgr.Get().Unload();
    if ((UnityEngine.Object) this.m_boardDetailsDisplay != (UnityEngine.Object) null)
      this.m_boardDetailsDisplay.Unload();
    if ((UnityEngine.Object) this.m_finisherDetailsDisplay != (UnityEngine.Object) null)
      this.m_finisherDetailsDisplay.Unload();
    if ((UnityEngine.Object) this.m_emoteDetailsDisplay != (UnityEngine.Object) null)
      this.m_emoteDetailsDisplay.Unload();
    if ((UnityEngine.Object) this.m_emoteLayoutDisplay != (UnityEngine.Object) null)
      this.m_emoteLayoutDisplay.Unload();
    if ((UnityEngine.Object) this.m_emoteTray != (UnityEngine.Object) null)
      this.m_emoteTray.Unload();
    CollectionManager.Get().RemoveCollectionLoadedListener(new CollectionManager.DelOnCollectionLoaded(((CollectibleDisplay) this).OnCollectionLoaded));
    CollectionManager.Get().RemoveCollectionChangedListener(new CollectionManager.DelOnCollectionChanged(((CollectibleDisplay) this).OnCollectionChanged));
    CollectionManager.Get().RemoveCardRewardsInsertedListener(new CollectionManager.DelOnCardRewardsInserted(this.OnCardRewardsInserted));
    CollectionManager.Get().RemoveCollectionNetHandlers();
    CollectionManager.Get().RemoveNewCardSeenListener(new CollectionManager.DelOnNewCardSeen(this.OnNewCardSeen));
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    this.m_unloading = false;
  }

  public override void Exit()
  {
    this.EnableInput(false);
    NotificationManager.Get().DestroyAllPopUps();
    if ((UnityEngine.Object) this.m_pageManager != (UnityEngine.Object) null)
      this.m_pageManager.Exit();
    SceneMgr.Mode prevMode = SceneMgr.Get().GetPrevMode();
    HearthstonePerformance.Get()?.StopCurrentFlow();
    SceneMgr.Get().SetNextMode(prevMode);
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
      Log.CollectionManager.Print("artStacksToDisplay is null!");
      flag = true;
    }
    else if (collectiblesToDisplay.Count == 0)
    {
      Log.CollectionManager.Print("artStacksToDisplay has a count of 0!");
      flag = true;
    }
    if (flag)
    {
      if (callback == null)
        return;
      callback(new List<CollectionCardActors>(), new List<ICollectible>(), callbackData);
    }
    else
    {
      if (this.m_unloading)
        return;
      foreach (CollectionCardActors previousCardActor in this.m_previousCardActors)
        previousCardActor.Destroy();
      this.m_previousCardActors.Clear();
      this.m_previousCardActors = this.m_cardActors;
      this.m_cardActors = new List<CollectionCardActors>();
      bool playerHasEarlyAccessHeroes = RewardTrackManager.Get().HasBattlegroundsPreviewHeroes();
      List<ICollectible> nonActorCollectibles = new List<ICollectible>();
      foreach (TCollectible collectible1 in (IEnumerable<TCollectible>) collectiblesToDisplay)
      {
        ICollectible collectible2 = (ICollectible) collectible1;
        if (!(collectible2 is CollectibleCard card))
        {
          nonActorCollectibles.Add(collectible2);
        }
        else
        {
          EntityDef entityDef = DefLoader.Get().GetEntityDef(card.CardId);
          using (DefLoader.DisposableCardDef cardDef = DefLoader.Get().GetCardDef(card.CardId, card.PremiumType))
          {
            string assetRef = this.m_currentViewMode == CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS ? "Card_Guide_Skin.prefab:cf2cadaa8c6f7244fb9500edb2046c8b" : "Card_Bacon_Hero_Skin.prefab:7b4af2ee64cfdf24e8ebc8fc817b9761";
            GameObject gameObject = AssetLoader.Get().InstantiatePrefab((AssetReference) assetRef, AssetLoadingOptions.IgnorePrefabPosition);
            if ((UnityEngine.Object) gameObject == (UnityEngine.Object) null)
            {
              Debug.LogError((object) "Unable to load card actor.");
            }
            else
            {
              Actor component1 = gameObject.GetComponent<Actor>();
              if ((UnityEngine.Object) component1 == (UnityEngine.Object) null)
              {
                Debug.LogError((object) "Actor object does not contain Actor component.");
              }
              else
              {
                component1.SetEntityDef(entityDef);
                component1.SetCardDef(cardDef);
                component1.SetPremium(card.PremiumType);
                component1.CreateBannedRibbon();
                BaconCollectionHeroSkin component2 = gameObject.GetComponent<BaconCollectionHeroSkin>();
                if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
                  component2.SetCardStateDisplay(card, entityDef, playerHasEarlyAccessHeroes);
                BaconCollectionGuideSkin component3 = gameObject.GetComponent<BaconCollectionGuideSkin>();
                if ((UnityEngine.Object) component3 != (UnityEngine.Object) null)
                  component3.SetCardStateDisplay(card);
                component1.UpdateAllComponents();
                this.m_cardActors.Add(new CollectionCardActors(component1));
              }
            }
          }
        }
      }
      if (callback == null)
        return;
      callback(this.m_cardActors, nonActorCollectibles, callbackData);
    }
  }

  private bool OnBackOutOfCollectionScreen()
  {
    if ((UnityEngine.Object) this == (UnityEngine.Object) null || (UnityEngine.Object) this.gameObject == (UnityEngine.Object) null)
      return true;
    this.Exit();
    return true;
  }

  public override void SetViewMode(
    CollectionUtils.ViewMode mode,
    bool triggerResponse,
    CollectionUtils.ViewModeData userdata = null)
  {
    Log.CollectionManager.Print("mode={0}-->{1} triggerResponse={2}", (object) this.m_currentViewMode, (object) mode, (object) triggerResponse);
    if (this.m_currentViewMode == mode)
      return;
    CollectionUtils.ViewMode currentViewMode = this.m_currentViewMode;
    this.m_currentViewMode = mode;
    this.OnSwitchViewModeResponse(triggerResponse, currentViewMode, mode, userdata);
    this.m_baconFilterButton.SetActive(mode == CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS);
    if (mode == CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES)
    {
      this.ShowEmoteTray();
      this.ToggleLayoutButton(true);
    }
    else
    {
      if (currentViewMode != CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES)
        return;
      this.HideEmoteTray();
      this.ToggleLayoutButton(false);
    }
  }

  private void ToggleLayoutButton(bool toggle)
  {
    if (!((UnityEngine.Object) this.m_emoteLayoutDisplayButton != (UnityEngine.Object) null))
      return;
    this.m_emoteLayoutDisplayButton.SetEnabled(toggle);
    this.m_emoteLayoutDisplayButton.Flip(!toggle);
  }

  public override void FilterBySearchText(string newSearchText)
  {
    string text = this.m_search.GetText();
    base.FilterBySearchText(newSearchText);
    this.OnSearchDeactivated_Internal(text, newSearchText);
  }

  public override void HideAllTips()
  {
    if (!((UnityEngine.Object) this.m_innkeeperLClickReminder != (UnityEngine.Object) null))
      return;
    NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_innkeeperLClickReminder);
  }

  public override void ShowInnkeeperLClickHelp(EntityDef entityDef) => this.ShowInnkeeperLClickHelp(entityDef != null && entityDef.IsHeroSkin());

  private void ShowInnkeeperLClickHelp(bool isHero)
  {
    if (isHero)
      this.m_innkeeperLClickReminder = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_LCLICK_HERO"), "", 3f);
    else
      this.m_innkeeperLClickReminder = NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_CM_LCLICK"), "", 3f);
  }

  public override void SetFilterCallback(
    List<TAG_CARD_SET> cardSets,
    List<int> specificCards,
    PegasusShared.FormatType formatType,
    SetFilterItem item,
    bool transitionPage)
  {
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

  public void ShowBoardDetailsDisplay(
    BattlegroundsBoardSkinDataModel dataModel,
    BattlegroundsBoardSkinCollectionPageDataModel pageModel)
  {
    if ((UnityEngine.Object) this.m_boardDetailsDisplay == (UnityEngine.Object) null || !this.m_boardDetailsDisplay.CanShow((IDataModel) dataModel, (IDataModel) pageModel))
      return;
    this.m_boardDetailsDisplay.AssignDataModels((IDataModel) dataModel, (IDataModel) pageModel);
    this.m_boardDetailsDisplay.Show();
  }

  public void ShowFinisherDetailsDisplay(
    BattlegroundsFinisherDataModel dataModel,
    BattlegroundsFinisherCollectionPageDataModel pageModel)
  {
    if ((UnityEngine.Object) this.m_finisherDetailsDisplay == (UnityEngine.Object) null || !this.m_finisherDetailsDisplay.CanShow((IDataModel) dataModel, (IDataModel) pageModel))
      return;
    this.m_finisherDetailsDisplay.AssignDataModels((IDataModel) dataModel, (IDataModel) pageModel);
    this.m_finisherDetailsDisplay.Show();
  }

  public void ShowEmoteDetailsDisplay(
    BattlegroundsEmoteDataModel dataModel,
    BattlegroundsEmoteCollectionPageDataModel pageModel)
  {
    if ((UnityEngine.Object) this.m_emoteDetailsDisplay == (UnityEngine.Object) null || !this.m_emoteDetailsDisplay.CanShow((IDataModel) dataModel, (IDataModel) pageModel))
      return;
    this.m_emoteDetailsDisplay.AssignDataModels((IDataModel) dataModel, (IDataModel) pageModel);
    this.m_emoteDetailsDisplay.Show();
  }

  public bool IsEmoteDetailsShowing() => this.m_emoteDetailsDisplay.isActiveAndEnabled;

  public void ShowEmoteLayoutDisplay()
  {
    if ((UnityEngine.Object) this.m_emoteLayoutDisplay == (UnityEngine.Object) null || this.m_currentViewMode != CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES)
      return;
    this.m_emoteLayoutDisplay.Show(this.GetOrCreateEmoteLoadoutDataModel(), this.m_emoteTray);
  }

  public void ShowEmoteTray()
  {
    if ((UnityEngine.Object) this.m_emoteTray == (UnityEngine.Object) null)
      return;
    this.m_emoteTray.Show(this.GetOrCreateEmoteLoadoutDataModel());
  }

  public void HideEmoteTray()
  {
    if ((UnityEngine.Object) this.m_emoteTray == (UnityEngine.Object) null)
      return;
    this.m_emoteTray.Hide();
  }

  private void OnCardRewardsInserted(List<string> cardID, List<TAG_PREMIUM> premium) => this.m_pageManager.RefreshCurrentPageContents();

  private void OnNewCardSeen(string cardID, TAG_PREMIUM premium)
  {
    if (!((UnityEngine.Object) this.m_pageManager != (UnityEngine.Object) null))
      return;
    this.m_pageManager.UpdateTabNewItemCounts();
  }

  protected override void OnCollectionChanged()
  {
    if (this.m_currentViewMode != CollectionUtils.ViewMode.MASS_DISENCHANT)
      this.m_pageManager.NotifyOfCollectionChanged();
    if (!((UnityEngine.Object) this.m_pageManager != (UnityEngine.Object) null))
      return;
    this.m_pageManager.UpdateTabNewItemCounts();
  }

  private IEnumerator WaitUntilReady()
  {
    BaconCollectionDisplay collectionDisplay = this;
    while (!collectionDisplay.m_netCacheReady && Network.IsLoggedIn())
      yield return (object) null;
    while (!collectionDisplay.m_boardDetailsDisplayFinishedLoading)
      yield return (object) null;
    while (!collectionDisplay.m_finisherDetailsDisplayFinishedLoading)
      yield return (object) null;
    while (!collectionDisplay.m_emoteDetailsDisplayFinishedLoading)
      yield return (object) null;
    while (!collectionDisplay.m_emoteLayoutDisplayFinishedLoading)
      yield return (object) null;
    while (!collectionDisplay.m_emoteTrayFinishedLoading)
      yield return (object) null;
    collectionDisplay.m_isReady = true;
  }

  private IEnumerator InitCollectionWhenReady()
  {
    while (!this.m_pageManager.IsFullyLoaded())
      yield return (object) null;
    this.m_pageManager.ShowHeroSkins();
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

  private void OnBoardDetailsDisplayReady(VisualController vc)
  {
    this.m_boardDetailsDisplay = vc.GetComponentInChildren<BaconBoardCollectionDetails>();
    this.m_boardDetailsDisplayFinishedLoading = true;
  }

  private void OnFinisherDetailsDisplayReady(VisualController vc)
  {
    this.m_finisherDetailsDisplay = vc.GetComponentInChildren<BaconFinisherCollectionDetails>();
    this.m_finisherDetailsDisplayFinishedLoading = true;
  }

  private void OnEmoteDetailsDisplayReady(VisualController vc)
  {
    this.m_emoteDetailsDisplay = vc.GetComponentInChildren<BaconEmoteCollectionDetails>();
    this.m_emoteDetailsDisplayFinishedLoading = true;
  }

  private void OnEmoteLayoutDisplayReady(VisualController vc)
  {
    this.m_emoteLayoutDisplay = vc.GetComponentInChildren<BaconEmoteCollectionLayout>();
    this.m_emoteLayoutDisplayFinishedLoading = true;
  }

  private void OnEmoteTrayReady(VisualController vc)
  {
    this.m_emoteTray = vc.GetComponentInChildren<BaconEmoteTray>();
    this.m_emoteTrayFinishedLoading = true;
  }

  private void OnEmotePreviewButtonReady(UIBButton button)
  {
    button.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.ShowEmoteLayoutDisplay()));
    this.m_emoteLayoutDisplayButton = button;
    this.ToggleLayoutButton(this.m_currentViewMode == CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES);
  }

  protected override void LoadAllTextures()
  {
  }

  protected override void UnloadAllTextures()
  {
  }

  private IEnumerator DoBookOpeningAnimations()
  {
    BaconCollectionDisplay collectionDisplay = this;
    while (collectionDisplay.m_isBookCoverLoading)
      yield return (object) null;
    if ((UnityEngine.Object) collectionDisplay.m_cover != (UnityEngine.Object) null)
      collectionDisplay.m_cover.Open(new CollectionCoverDisplay.DelOnOpened(((CollectibleDisplay) collectionDisplay).OnCoverOpened));
    else
      collectionDisplay.OnCoverOpened();
  }

  private IEnumerator SetBookToOpen()
  {
    BaconCollectionDisplay collectionDisplay = this;
    while (collectionDisplay.m_isBookCoverLoading)
      yield return (object) null;
    if ((UnityEngine.Object) collectionDisplay.m_cover != (UnityEngine.Object) null)
      collectionDisplay.m_cover.SetOpenState();
  }

  protected override void OnSearchDeactivated(string oldSearchText, string newSearchText) => this.OnSearchDeactivated_Internal(oldSearchText, newSearchText);

  private void OnSearchDeactivated_Internal(string oldSearchText, string newSearchText)
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
        this.m_heroSkinFilterMode = CollectionUtils.BattlegroundsHeroSkinFilterMode.ALL;
        this.m_pageManager.UpdateHeroSkinsFilterType(false);
        this.m_baconFilterButton.FilterUpdated();
        this.m_searchTriggeredHeroSkinFilter = true;
      }
      else
        this.ResetFilterSettingsFromSearch();
      this.NotifyFilterUpdate(this.m_searchFilterListeners, !string.IsNullOrEmpty(newSearchText), (object) newSearchText);
      this.m_pageManager.ChangeSearchTextFilter(newSearchText, new BookPageManager.DelOnPageTransitionComplete(((CollectibleDisplay) this).OnSearchFilterComplete), (object) null, true);
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
    if (this.m_searchTriggeredHeroSkinFilter)
    {
      this.m_heroSkinFilterMode = CollectionUtils.BattlegroundsHeroSkinFilterMode.DEFAULT;
      this.m_pageManager.UpdateHeroSkinsFilterType(false);
      this.m_baconFilterButton.FilterUpdated();
    }
    this.m_searchTriggeredHeroSkinFilter = false;
  }

  private void DoEnterCollectionManagerEvents()
  {
    if (CollectionManager.Get().HasVisitedCollection())
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
    int mode = (int) SceneMgr.Get().GetMode();
    this.StartCoroutine(this.SetBookToOpen());
  }

  private IEnumerator OpenBookWhenReady()
  {
    BaconCollectionDisplay collectionDisplay = this;
    while (CollectionManager.Get().IsWaitingForBoxTransition())
      yield return (object) null;
    int mode = (int) SceneMgr.Get().GetMode();
    collectionDisplay.m_pageManager.OnBookOpening();
    collectionDisplay.StartCoroutine(collectionDisplay.DoBookOpeningAnimations());
  }
}
