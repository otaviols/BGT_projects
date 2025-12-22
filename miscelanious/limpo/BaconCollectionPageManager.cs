using Hearthstone;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CustomEditClass]
public class BaconCollectionPageManager : CollectiblePageManager
{
  public CollectionClassTab m_heroSkinsTab;
  public CollectionClassTab m_guideSkinsTab;
  public CollectionClassTab m_boardSkinsTab;
  public CollectionClassTab m_finishersTab;
  public CollectionClassTab m_emotesTab;
  public BaconClassFilterButton m_heroSkinsButton;
  public BaconClassFilterButton m_guideSkinsButton;
  public BaconClassFilterButton m_boardSkinsButton;
  public BaconClassFilterButton m_finishersButton;
  public BaconClassFilterButton m_emotesButton;
  public BaconClassFilterHeaderButton m_classFilterHeader;
  private static CollectionUtils.ViewMode[] TAG_ORDERING = new CollectionUtils.ViewMode[5]
  {
    CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS,
    CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS,
    CollectionUtils.ViewMode.BATTLEGROUNDS_BOARD_SKINS,
    CollectionUtils.ViewMode.BATTLEGROUNDS_FINISHERS,
    CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES
  };
  private static readonly int NUM_PAGE_FLIPS_BEFORE_SET_FILTER_TUTORIAL = 3;
  private CollectibleCardBaconHeroesFilter m_baconHeroesCollection = new CollectibleCardBaconHeroesFilter();
  private CollectibleCardBaconGuidesFilter m_baconGuidesCollection = new CollectibleCardBaconGuidesFilter();
  private CollectibleBattlegroundsBoardSet m_baconBoardsCollection = new CollectibleBattlegroundsBoardSet();
  private CollectibleBattlegroundsFinisherSet m_baconFinishersCollection = new CollectibleBattlegroundsFinisherSet();
  private CollectibleBattlegroundsEmoteSet m_baconEmotesCollection = new CollectibleBattlegroundsEmoteSet();
  private int m_numPageFlipsThisSession;
  private bool m_allowHoverHighlight = true;
  private List<Vector3> m_listTabPos = new List<Vector3>();
  private List<Vector3> m_listButtonPos = new List<Vector3>();
  private List<BaconClassFilterButton> m_listButton = new List<BaconClassFilterButton>();

  protected override void Start()
  {
    this.SetUpBookButtons();
    base.Start();
    NetCache.Get().RegisterScreenCollectionManager(new NetCache.NetCacheCallback(this.OnNetCacheReady));
  }

  protected override void Awake()
  {
    base.Awake();
    this.m_baconHeroesCollection.Init(CollectiblePageDisplay.GetMaxCardsPerPage(CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS));
    this.m_baconGuidesCollection.Init(CollectiblePageDisplay.GetMaxCardsPerPage(CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS));
    this.m_baconBoardsCollection.AddItemsFromDbf();
    this.m_baconBoardsCollection.ItemsPerPage = CollectiblePageDisplay.GetMaxCardsPerPage(CollectionUtils.ViewMode.BATTLEGROUNDS_BOARD_SKINS);
    this.m_baconFinishersCollection.AddItemsFromDbf();
    this.m_baconFinishersCollection.ItemsPerPage = CollectiblePageDisplay.GetMaxCardsPerPage(CollectionUtils.ViewMode.BATTLEGROUNDS_FINISHERS);
    this.m_baconEmotesCollection.AddItemsFromDbf();
    this.m_baconEmotesCollection.ItemsPerPage = CollectiblePageDisplay.GetMaxCardsPerPage(CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES);
    this.UpdateFilteredHeroes();
    this.UpdateFilteredGuides();
    this.UpdateFilteredBoards();
    this.UpdateFilteredFinishers();
    this.UpdateFilteredEmotes();
    this.UpdateTabNewItemCounts();
    NetCache.Get().FavoriteBattlegroundsHeroSkinChanged += new NetCache.DelFavoriteBattlegroundsHeroSkinChangedListener(this.OnFavoriteBattlegroundsHeroSkinChanged);
    NetCache.Get().FavoriteBattlegroundsGuideSkinChanged += new NetCache.DelFavoriteBattlegroundsGuideSkinChangedListener(this.OnFavoriteBattlegroundsGuideSkinChanged);
    NetCache.Get().FavoriteBattlegroundsBoardSkinChanged += new NetCache.DelFavoriteBattlegroundsBoardSkinChangedListener(this.OnFavoriteBattlegroundsBoardSkinChanged);
    NetCache.Get().FavoriteBattlegroundsFinisherChanged += new NetCache.DelFavoriteBattlegroundsFinisherChangedListener(this.OnFavoriteBattlegroundsFinisherChanged);
  }

  public override void OnDestroy()
  {
    base.OnDestroy();
    if (NetCache.Get() == null)
      return;
    NetCache.Get().FavoriteBattlegroundsHeroSkinChanged -= new NetCache.DelFavoriteBattlegroundsHeroSkinChangedListener(this.OnFavoriteBattlegroundsHeroSkinChanged);
    NetCache.Get().FavoriteBattlegroundsGuideSkinChanged -= new NetCache.DelFavoriteBattlegroundsGuideSkinChangedListener(this.OnFavoriteBattlegroundsGuideSkinChanged);
    NetCache.Get().FavoriteBattlegroundsBoardSkinChanged -= new NetCache.DelFavoriteBattlegroundsBoardSkinChangedListener(this.OnFavoriteBattlegroundsBoardSkinChanged);
    NetCache.Get().FavoriteBattlegroundsFinisherChanged -= new NetCache.DelFavoriteBattlegroundsFinisherChangedListener(this.OnFavoriteBattlegroundsFinisherChanged);
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
  }

  public override bool JumpToPageWithCard(
    string cardID,
    TAG_PREMIUM premium,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    Debug.LogWarning((object) "Attempted to jump to a page with a card in Battlegrounds, which the collection screen does not allow.");
    return false;
  }

  public override void ChangeSearchTextFilter(
    string newSearchText,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    bool transitionPage = true)
  {
    this.m_baconHeroesCollection.FilterSearchText(newSearchText);
    this.m_baconGuidesCollection.FilterSearchText(newSearchText);
    this.m_baconBoardsCollection.SearchString = newSearchText;
    this.m_baconFinishersCollection.SearchString = newSearchText;
    this.m_baconEmotesCollection.SearchString = newSearchText;
    CardBackManager.Get().SetSearchText(newSearchText);
    this.UpdateFilteredHeroes();
    this.UpdateFilteredGuides();
    this.UpdateFilteredBoards();
    this.UpdateFilteredFinishers();
    this.UpdateFilteredEmotes();
    this.UpdateTabNewItemCounts();
    if (!transitionPage)
      return;
    this.m_currentPageNum = 1;
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.MANY_PAGE_LEFT, false, callback, callbackData);
  }

  public override void RemoveSearchTextFilter(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    bool transitionPage = true)
  {
    this.m_baconHeroesCollection.FilterSearchText((string) null);
    this.m_baconGuidesCollection.FilterSearchText((string) null);
    this.m_baconBoardsCollection.SearchString = (string) null;
    this.m_baconFinishersCollection.SearchString = (string) null;
    this.m_baconEmotesCollection.SearchString = (string) null;
    CardBackManager.Get().SetSearchText((string) null);
    this.UpdateFilteredHeroes();
    this.UpdateFilteredGuides();
    this.UpdateFilteredBoards();
    this.UpdateFilteredFinishers();
    this.UpdateFilteredEmotes();
    this.UpdateTabNewItemCounts();
    if (transitionPage)
      this.m_currentPageNum = 1;
    base.RemoveSearchTextFilter(callback, callbackData, transitionPage);
  }

  public void UpdateHeroSkinsFilterType(bool transitionPage = true)
  {
    this.UpdateFilteredHeroes();
    this.UpdateTabNewItemCounts();
    if (!transitionPage)
      return;
    this.m_currentPageNum = 1;
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.MANY_PAGE_LEFT, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public override void NotifyOfCollectionChanged()
  {
  }

  public void EnableEmoteHoverHighlights(bool enable)
  {
    this.m_allowHoverHighlight = enable;
    string eventName = this.m_allowHoverHighlight ? "ENABLE_HOVER_HIGHLIGHT" : "DISABLE_HOVER_HIGHLIGHT";
    (this.GetCurrentPage() as BaconCollectionPageDisplay).m_EmotesWidget.TriggerEvent(eventName);
  }

  protected override bool CanUserTurnPages()
  {
    if (CraftingManager.GetIsInCraftingMode() || SceneMgr.Get().IsInDuelsMode() && !PvPDungeonRunScene.IsEditingDeck())
      return false;
    CardBackInfoManager cardBackInfoManager = CardBackInfoManager.Get();
    if ((Object) cardBackInfoManager != (Object) null && cardBackInfoManager.IsPreviewing)
      return false;
    BaconHeroSkinInfoManager heroSkinInfoManager = BaconHeroSkinInfoManager.Get();
    return (!((Object) heroSkinInfoManager != (Object) null) || !heroSkinInfoManager.IsShowingPreview) && base.CanUserTurnPages();
  }

  private BaconCollectionPageDisplay PageAsCollectionPage(
    BookPageDisplay page)
  {
    BaconCollectionPageDisplay collectionPageDisplay = page as BaconCollectionPageDisplay;
    if (!((Object) collectionPageDisplay == (Object) null))
      return collectionPageDisplay;
    Log.CollectionManager.PrintError("Page in BaconCollectionPageManager is not a BaconCollectionPageDisplay!  This should not happen!");
    return collectionPageDisplay;
  }

  protected override bool ShouldShowTab(BookTab tab)
  {
    if (NetCache.Get() == null)
      return true;
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject == null)
    {
      Log.Net.PrintError("No NetCacheFeatures info in NetCache.");
      return true;
    }
    if ((Object) tab == (Object) this.m_boardSkinsTab)
      return netObject.BattlegroundsBoardSkinsEnabled;
    if ((Object) tab == (Object) this.m_finishersTab)
      return netObject.BattlegroundsFinishersEnabled;
    return !((Object) tab == (Object) this.m_emotesTab) || netObject.BattlegroundsEmotesEnabled;
  }

  protected override void SetUpBookTabs()
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    bool receiveReleaseWithoutMouseDown = UniversalInputManager.Get().IsTouchMode();
    CollectionClassTab[] collectionClassTabArray = new CollectionClassTab[5]
    {
      this.m_heroSkinsTab,
      this.m_guideSkinsTab,
      this.m_boardSkinsTab,
      this.m_finishersTab,
      this.m_emotesTab
    };
    UIEvent.Handler[] handlerArray = new UIEvent.Handler[5]
    {
      new UIEvent.Handler(this.OnHeroSkinsTabPressed),
      new UIEvent.Handler(this.OnGuideSkinsTabPressed),
      new UIEvent.Handler(this.OnBoardSkinsTabPressed),
      new UIEvent.Handler(this.OnFinishersTabPressed),
      new UIEvent.Handler(this.OnEmotesTabPressed)
    };
    int index = 0;
    foreach (CollectionClassTab key in collectionClassTabArray)
    {
      if ((Object) key != (Object) null)
      {
        key.Init(TAG_CLASS.NEUTRAL);
        key.AddEventListener(UIEventType.RELEASE, handlerArray[index]);
        key.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver));
        key.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut));
        key.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOver_Touch));
        key.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(((TabbedBookPageManager) this).OnTabOut_Touch));
        key.SetReceiveReleaseWithoutMouseDown(receiveReleaseWithoutMouseDown);
        this.m_allTabs.Add((BookTab) key);
        this.m_listTabPos.Add(key.transform.localPosition);
        this.m_tabVisibility[(BookTab) key] = true;
      }
      ++index;
    }
    this.PositionBookTabs(false);
    this.m_initializedTabPositions = true;
  }

  protected override void PositionBookTabs(bool animate)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    int num1 = this.m_allTabs.Count<BookTab>();
    int num2 = 0;
    for (int index = 0; index < num1; ++index)
    {
      CollectionClassTab allTab = (CollectionClassTab) this.m_allTabs[index];
      bool flag = this.ShouldShowTab((BookTab) allTab);
      allTab.SetIsVisible(flag);
      allTab.SetTargetVisibility(flag);
      this.m_tabVisibility[(BookTab) allTab] = flag;
      allTab.gameObject.SetActive(flag);
      if (flag)
        allTab.transform.localPosition = this.m_listTabPos[num2++];
    }
  }

  private bool ShouldShowButton(BaconClassFilterButton button)
  {
    if (NetCache.Get() == null)
      return true;
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject == null)
    {
      Log.Net.PrintError("No NetCacheFeatures info in NetCache.");
      return true;
    }
    if ((Object) button == (Object) this.m_boardSkinsButton)
      return netObject.BattlegroundsBoardSkinsEnabled;
    if ((Object) button == (Object) this.m_finishersButton)
      return netObject.BattlegroundsFinishersEnabled;
    return !((Object) button == (Object) this.m_emotesButton) || netObject.BattlegroundsEmotesEnabled;
  }

  private void SetUpBookButtons()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    BaconClassFilterButton[] classFilterButtonArray = new BaconClassFilterButton[5]
    {
      this.m_heroSkinsButton,
      this.m_guideSkinsButton,
      this.m_boardSkinsButton,
      this.m_finishersButton,
      this.m_emotesButton
    };
    foreach (BaconClassFilterButton classFilterButton in classFilterButtonArray)
    {
      if ((Object) classFilterButton != (Object) null)
      {
        this.m_listButton.Add(classFilterButton);
        this.m_listButtonPos.Add(classFilterButton.transform.localPosition);
      }
    }
    this.PositionBookButtons();
  }

  private void PositionBookButtons()
  {
    int num1 = this.m_listButton.Count<BaconClassFilterButton>();
    int num2 = 0;
    for (int index = 0; index < num1; ++index)
    {
      BaconClassFilterButton button = this.m_listButton[index];
      bool flag = this.ShouldShowButton(button);
      button.gameObject.SetActive(flag);
      if (flag)
        button.transform.localPosition = this.m_listButtonPos[num2++];
    }
  }

  private void OnNetCacheReady()
  {
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.PositionBookButtons();
    else
      this.PositionBookTabs(false);
  }

  public void ShowHeroSkins()
  {
    this.m_currentPageNum = 1;
    this.UpdateFilteredHeroes();
    this.UpdateTabNewItemCounts();
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS, false);
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.NONE, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public void ShowGuideSkins()
  {
    this.m_currentPageNum = 1;
    this.UpdateFilteredGuides();
    this.UpdateTabNewItemCounts();
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS, false);
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.NONE, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public void ShowBoardSkins()
  {
    this.m_currentPageNum = 1;
    this.UpdateFilteredBoards();
    this.UpdateTabNewItemCounts();
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.BATTLEGROUNDS_BOARD_SKINS, false);
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.NONE, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public void ShowFinishers()
  {
    this.m_currentPageNum = 1;
    this.UpdateFilteredFinishers();
    this.UpdateTabNewItemCounts();
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.BATTLEGROUNDS_FINISHERS, false);
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.NONE, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public void ShowEmotes()
  {
    this.m_currentPageNum = 1;
    this.UpdateFilteredEmotes();
    this.UpdateTabNewItemCounts();
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES, false);
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.NONE, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  protected override void AssembleEmptyPageUI(BookPageDisplay page)
  {
    base.AssembleEmptyPageUI(page);
    this.AssembleEmptyPageUI(page as CollectiblePageDisplay, false);
  }

  protected override void AssembleEmptyPageUI(
    CollectiblePageDisplay page,
    bool displayNoMatchesText)
  {
    BaconCollectionPageDisplay collectionPageDisplay = this.PageAsCollectionPage((BookPageDisplay) page);
    if ((Object) collectionPageDisplay == (Object) null)
    {
      Log.CollectionManager.PrintError("Page in CollectionPageManager is not a BaconCollectionPageDisplay!  This should not happen!");
    }
    else
    {
      collectionPageDisplay.ShowNoMatchesFound(displayNoMatchesText, this.m_baconHeroesCollection.FindCardsResult, false);
      collectionPageDisplay.SetPageCountText(GameStrings.Get("GLUE_COLLECTION_EMPTY_PAGE"));
    }
  }

  protected override bool AssembleCollectiblePage<TCollectible>(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    ICollection<TCollectible> collectiblesToDisplay,
    int totalNumPages)
  {
    bool flag = base.AssembleCollectiblePage<TCollectible>(transitionReadyCallbackData, collectiblesToDisplay, totalNumPages);
    CollectionUtils.ViewMode viewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
    BaconCollectionPageDisplay page = this.PageAsCollectionPage(transitionReadyCallbackData.m_assembledPage);
    switch (viewMode)
    {
      case CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS:
        page.SetGuideSkins();
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS:
        page.SetHeroSkins();
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_BOARD_SKINS:
        page.SetBoardSkins();
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_FINISHERS:
        page.SetFinishers();
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES:
        page.SetEmotes();
        break;
    }
    if (flag)
      return true;
    page.SetPageCountText(GameStrings.Format("GLUE_COLLECTION_PAGE_NUM", (object) this.m_currentPageNum));
    page.ShowNoMatchesFound(false, (CollectionManager.FindCardsResult) null, true);
    this.SetHasPreviousAndNextPages(this.m_currentPageNum > 1, this.m_currentPageNum < totalNumPages);
    CollectionManager.Get().GetCollectibleDisplay().CollectionPageContentsChanged<TCollectible>(collectiblesToDisplay, (CollectibleDisplay.CollectionActorsReadyCallback) ((actorList, nonActorCollectibleList, data) =>
    {
      page.UpdateCollectionItems(actorList, nonActorCollectibleList, viewMode);
      this.TransitionPageNextFrame(transitionReadyCallbackData);
    }), (object) null);
    return true;
  }

  protected override void AssemblePage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum)
  {
    switch (CollectionManager.Get().GetCollectibleDisplay().GetViewMode())
    {
      case CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS:
        List<CollectibleCard> pageContents1 = this.m_baconGuidesCollection.GetPageContents(this.m_currentPageNum);
        this.AssembleCollectiblePage<CollectibleCard>(transitionReadyCallbackData, (ICollection<CollectibleCard>) pageContents1, this.m_baconGuidesCollection.GetTotalNumPages());
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS:
        List<CollectibleCard> pageContents2 = this.m_baconHeroesCollection.GetPageContents(this.m_currentPageNum);
        this.AssembleCollectiblePage<CollectibleCard>(transitionReadyCallbackData, (ICollection<CollectibleCard>) pageContents2, this.m_baconHeroesCollection.GetTotalNumPages());
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_BOARD_SKINS:
        List<CollectibleBattlegroundsBoard> pageContents3 = this.m_baconBoardsCollection.GetPageContents(this.m_currentPageNum);
        this.AssembleCollectiblePage<CollectibleBattlegroundsBoard>(transitionReadyCallbackData, (ICollection<CollectibleBattlegroundsBoard>) pageContents3, this.m_baconBoardsCollection.TotalPages);
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_FINISHERS:
        List<CollectibleBattlegroundsFinisher> pageContents4 = this.m_baconFinishersCollection.GetPageContents(this.m_currentPageNum);
        this.AssembleCollectiblePage<CollectibleBattlegroundsFinisher>(transitionReadyCallbackData, (ICollection<CollectibleBattlegroundsFinisher>) pageContents4, this.m_baconFinishersCollection.TotalPages);
        break;
      case CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES:
        List<CollectibleBattlegroundsEmote> pageContents5 = this.m_baconEmotesCollection.GetPageContents(this.m_currentPageNum);
        this.AssembleCollectiblePage<CollectibleBattlegroundsEmote>(transitionReadyCallbackData, (ICollection<CollectibleBattlegroundsEmote>) pageContents5, this.m_baconEmotesCollection.TotalPages);
        break;
    }
  }

  private void UpdateFilteredHeroes() => this.m_baconHeroesCollection.UpdateResults();

  private void UpdateFilteredGuides() => this.m_baconGuidesCollection.UpdateResults();

  private void UpdateFilteredBoards() => this.m_baconBoardsCollection.UpdateFilters();

  private void UpdateFilteredFinishers() => this.m_baconFinishersCollection.UpdateFilters();

  private void UpdateFilteredEmotes() => this.m_baconEmotesCollection.UpdateFilters();

  protected override void UpdateFilteredCards() => Debug.LogWarning((object) "BaconCollectionPageManager.UpdateFilteredCards should not be used!");

  public void UpdateTabNewItemCounts()
  {
    if ((Object) this.m_heroSkinsTab != (Object) null)
      this.m_heroSkinsTab.UpdateNewItemCount(CollectionManager.Get().CountNewBattlegroundsHeroSkins());
    if ((Object) this.m_guideSkinsTab != (Object) null)
      this.m_guideSkinsTab.UpdateNewItemCount(CollectionManager.Get().CountNewBattlegroundsGuideSkins());
    if ((Object) this.m_boardSkinsTab != (Object) null)
      this.m_boardSkinsTab.UpdateNewItemCount(CollectionManager.Get().CountNewBattlegroundsBoardSkins());
    if ((Object) this.m_finishersTab != (Object) null)
      this.m_finishersTab.UpdateNewItemCount(CollectionManager.Get().CountNewBattlegroundsFinishers());
    if ((Object) this.m_emotesTab != (Object) null)
      this.m_emotesTab.UpdateNewItemCount(CollectionManager.Get().CountNewBattlegroundsEmotes());
    if ((Object) this.m_heroSkinsButton != (Object) null)
      this.m_heroSkinsButton.UpdateNewItemCount(CollectionManager.Get().CountNewBattlegroundsHeroSkins());
    if ((Object) this.m_guideSkinsButton != (Object) null)
      this.m_guideSkinsButton.UpdateNewItemCount(CollectionManager.Get().CountNewBattlegroundsGuideSkins());
    if ((Object) this.m_boardSkinsButton != (Object) null)
      this.m_boardSkinsButton.UpdateNewItemCount(CollectionManager.Get().CountNewBattlegroundsBoardSkins());
    if ((Object) this.m_finishersButton != (Object) null)
      this.m_finishersButton.UpdateNewItemCount(CollectionManager.Get().CountNewBattlegroundsFinishers());
    if (!((Object) this.m_emotesButton != (Object) null))
      return;
    this.m_emotesButton.UpdateNewItemCount(CollectionManager.Get().CountNewBattlegroundsEmotes());
  }

  protected override void TransitionPage(object callbackData)
  {
    base.TransitionPage(callbackData);
    this.SetCurrentModeTab();
    if (CollectionManager.Get().GetCollectibleDisplay().GetViewMode() != CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES)
      return;
    this.EnableEmoteHoverHighlights(this.m_allowHoverHighlight);
  }

  private void SetCurrentModeTab()
  {
    CollectionUtils.ViewMode viewMode = CollectionManager.Get().GetCollectibleDisplay().GetViewMode();
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_classFilterHeader.SetMode(viewMode);
    }
    else
    {
      BookTab bookTab;
      switch (viewMode)
      {
        case CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS:
          bookTab = (BookTab) this.m_guideSkinsTab;
          break;
        case CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS:
          bookTab = (BookTab) this.m_heroSkinsTab;
          break;
        case CollectionUtils.ViewMode.BATTLEGROUNDS_BOARD_SKINS:
          bookTab = (BookTab) this.m_boardSkinsTab;
          break;
        case CollectionUtils.ViewMode.BATTLEGROUNDS_FINISHERS:
          bookTab = (BookTab) this.m_finishersTab;
          break;
        case CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES:
          bookTab = (BookTab) this.m_emotesTab;
          break;
        default:
          bookTab = (BookTab) null;
          break;
      }
      if ((Object) bookTab == (Object) this.m_currentTab)
        return;
      this.DeselectCurrentTab();
      this.m_currentTab = bookTab;
      if (!((Object) this.m_currentTab != (Object) null))
        return;
      this.StopCoroutine(CollectiblePageManager.SELECT_TAB_COROUTINE_NAME);
      this.StartCoroutine(CollectiblePageManager.SELECT_TAB_COROUTINE_NAME, (object) this.m_currentTab);
    }
  }

  protected override void OnPageTransitionRequested()
  {
    ++this.m_numPageFlipsThisSession;
    int num = Options.Get().GetInt(Option.PAGE_MOUSE_OVERS);
    int val = num + 1;
    if (num < this.m_numPlageFlipsBeforeStopShowingArrows)
      Options.Get().SetInt(Option.PAGE_MOUSE_OVERS, val);
    this.ShowSetFilterTutorialIfNeeded();
  }

  protected override void OnPageTurnComplete(object callbackData, int operationId)
  {
    if (this.m_numPageFlipsThisSession % CollectiblePageManager.NUM_PAGE_FLIPS_UNTIL_UNLOAD_UNUSED_ASSETS == 0)
    {
      HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
      if ((Object) hearthstoneApplication != (Object) null)
        hearthstoneApplication.UnloadUnusedAssets();
    }
    base.OnPageTurnComplete(callbackData, operationId);
  }

  private void ShowSetFilterTutorialIfNeeded()
  {
    if (Options.Get().GetBool(Option.HAS_SEEN_SET_FILTER_TUTORIAL) || CollectionManager.Get().IsInEditMode() || CollectionManager.Get().GetCollectibleDisplay().GetViewMode() != CollectionUtils.ViewMode.CARDS || !this.m_cardsCollection.CardSetFilterIsAllStandardSets())
      return;
    CollectionManagerDisplay collectibleDisplay = CollectionManager.Get().GetCollectibleDisplay() as CollectionManagerDisplay;
    if ((Object) collectibleDisplay == (Object) null || collectibleDisplay.IsShowingSetFilterTray() || !CollectionManager.Get().AccountHasWildCards() || !RankMgr.Get().WildCardsAllowedInCurrentLeague() || this.m_numPageFlipsThisSession < BaconCollectionPageManager.NUM_PAGE_FLIPS_BEFORE_SET_FILTER_TUTORIAL)
      return;
    collectibleDisplay.ShowSetFilterTutorial(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
    Options.Get().SetBool(Option.HAS_SEEN_SET_FILTER_TUTORIAL, true);
  }

  protected override void OnCollectionManagerViewModeChanged(
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode mode,
    CollectionUtils.ViewModeData userdata,
    bool triggerResponse)
  {
    if (!triggerResponse)
      return;
    Log.CollectionManager.Print("transitionPageId={0} pagesTurning={1} mode={2}-->{3} triggerResponse={4}", (object) this.m_transitionPageId, (object) this.m_pagesCurrentlyTurning, (object) prevMode, (object) mode, (object) triggerResponse);
    this.m_currentPageNum = 1;
    int num1 = 0;
    int num2 = 0;
    for (int index = 0; index < BaconCollectionPageManager.TAG_ORDERING.Length; ++index)
    {
      if (prevMode == BaconCollectionPageManager.TAG_ORDERING[index])
        num1 = index;
      if (mode == BaconCollectionPageManager.TAG_ORDERING[index])
        num2 = index;
    }
    BookPageManager.PageTransitionType transition = num2 - num1 < 0 ? BookPageManager.PageTransitionType.SINGLE_PAGE_LEFT : BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT;
    BookPageManager.DelOnPageTransitionComplete callback = (BookPageManager.DelOnPageTransitionComplete) null;
    object callbackData = (object) null;
    if (userdata != null)
    {
      callback = userdata.m_pageTransitionCompleteCallback;
      callbackData = userdata.m_pageTransitionCompleteData;
    }
    if (this.m_turnPageCoroutine != null)
      this.StopCoroutine(this.m_turnPageCoroutine);
    this.m_turnPageCoroutine = this.StartCoroutine(this.ViewModeChangedWaitToTurnPage(transition, callback, callbackData));
  }

  private IEnumerator ViewModeChangedWaitToTurnPage(
    BookPageManager.PageTransitionType transition,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BaconCollectionPageManager collectionPageManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    collectionPageManager.TransitionPageWhenReady(transition, true, callback, callbackData);
    return false;
  }

  public void OnFavoriteBattlegroundsHeroSkinChanged(
    int baseSkinId,
    BattlegroundsHeroSkinId? newFavoriteBattlegroundsHeroSkin)
  {
    this.PageAsCollectionPage(this.GetCurrentPage()).UpdateFavoriteHeroSkins(CollectionManager.Get().GetCollectibleDisplay().GetViewMode());
  }

  public void OnFavoriteBattlegroundsGuideSkinChanged(
    BattlegroundsGuideSkinId? newFavoriteBattlegroundsGuideSkin)
  {
    this.PageAsCollectionPage(this.GetCurrentPage()).UpdateFavoriteGuideSkins(CollectionManager.Get().GetCollectibleDisplay().GetViewMode());
  }

  public void OnFavoriteBattlegroundsBoardSkinChanged(
    BattlegroundsBoardSkinId? newFavoriteBattlegroundsBoardSkin)
  {
    this.PageAsCollectionPage(this.GetCurrentPage()).UpdateFavoriteBoardSkins(CollectionManager.Get().GetCollectibleDisplay().GetViewMode());
  }

  public void OnFavoriteBattlegroundsFinisherChanged(
    BattlegroundsFinisherId? newFavoriteBattlegroundsFinisher)
  {
    this.PageAsCollectionPage(this.GetCurrentPage()).UpdateFavoriteFinisherSkins(CollectionManager.Get().GetCollectibleDisplay().GetViewMode());
  }

  public void SetEmoteEquippedState(BattlegroundsEmoteId emoteId, bool isEquipped) => this.PageAsCollectionPage(this.GetCurrentPage()).SetEmoteEquippedState(emoteId, isEquipped);

  private void OnHeroSkinsTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    CollectionClassTab element = e.GetElement() as CollectionClassTab;
    if ((Object) element == (Object) null || (Object) element == (Object) this.m_currentTab || !this.ShouldShowTab((BookTab) this.m_heroSkinsTab))
      return;
    this.UpdateFilteredHeroes();
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.BATTLEGROUNDS_HERO_SKINS);
    this.UpdateTabNewItemCounts();
  }

  private void OnGuideSkinsTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    CollectionClassTab element = e.GetElement() as CollectionClassTab;
    if ((Object) element == (Object) null || (Object) element == (Object) this.m_currentTab || !this.ShouldShowTab((BookTab) this.m_guideSkinsTab))
      return;
    this.UpdateFilteredGuides();
    this.UpdateTabNewItemCounts();
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.BATTLEGROUNDS_GUIDE_SKINS);
  }

  private void OnBoardSkinsTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    CollectionClassTab element = e.GetElement() as CollectionClassTab;
    if ((Object) element == (Object) null || (Object) element == (Object) this.m_currentTab || !this.ShouldShowTab((BookTab) this.m_boardSkinsTab))
      return;
    this.UpdateFilteredBoards();
    this.UpdateTabNewItemCounts();
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.BATTLEGROUNDS_BOARD_SKINS);
  }

  private void OnFinishersTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    CollectionClassTab element = e.GetElement() as CollectionClassTab;
    if ((Object) element == (Object) null || (Object) element == (Object) this.m_currentTab || !this.ShouldShowTab((BookTab) this.m_finishersTab))
      return;
    this.UpdateFilteredFinishers();
    this.UpdateTabNewItemCounts();
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.BATTLEGROUNDS_FINISHERS);
  }

  private void OnEmotesTabPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    CollectionClassTab element = e.GetElement() as CollectionClassTab;
    if ((Object) element == (Object) null || (Object) element == (Object) this.m_currentTab || !this.ShouldShowTab((BookTab) this.m_emotesTab))
      return;
    this.UpdateFilteredEmotes();
    this.UpdateTabNewItemCounts();
    CollectionManager.Get().GetCollectibleDisplay().SetViewMode(CollectionUtils.ViewMode.BATTLEGROUNDS_EMOTES);
  }

  private HashSet<int> GetCurrentDeckTrayModeCardBackIds() => CardBackManager.Get().GetCardBackIds(!CollectionManager.Get().IsInEditMode());
}
