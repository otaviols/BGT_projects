using Hearthstone.Core;
using PegasusShared;
using System.Collections.Generic;
using UnityEngine;

[CustomEditClass]
public abstract class CollectiblePageManager : TabbedBookPageManager
{
  public int m_numPlageFlipsBeforeStopShowingArrows;
  public static readonly float SELECT_TAB_ANIM_TIME = 0.2f;
  protected static readonly int NUM_PAGE_FLIPS_UNTIL_UNLOAD_UNUSED_ASSETS = 15;
  protected static readonly Vector3 TAB_LOCAL_EULERS = new Vector3(0.0f, 180f, 0.0f);
  protected static readonly float HIDDEN_TAB_LOCAL_Z_POS = -0.42f;
  protected static readonly string ANIMATE_TABS_COROUTINE_NAME = "AnimateTabs";
  protected static readonly string SELECT_TAB_COROUTINE_NAME = "SelectTabWhenReady";
  protected Coroutine m_turnPageCoroutine;
  protected bool m_initializedTabPositions;
  protected float m_deselectedTabHalfWidth;
  protected CollectibleCardFilter m_cardsCollection;

  protected override void Awake()
  {
    base.Awake();
    CollectibleDisplay collectibleDisplay = CollectionManager.Get()?.GetCollectibleDisplay();
    if (!((Object) collectibleDisplay != (Object) null))
      return;
    collectibleDisplay.OnViewModeChanged += new CollectibleDisplay.ViewModeChangedListener(this.OnCollectionManagerViewModeChanged);
  }

  public virtual void OnDestroy()
  {
    CollectibleDisplay collectibleDisplay = CollectionManager.Get()?.GetCollectibleDisplay();
    if (!((Object) collectibleDisplay != (Object) null))
      return;
    collectibleDisplay.OnViewModeChanged -= new CollectibleDisplay.ViewModeChangedListener(this.OnCollectionManagerViewModeChanged);
  }

  protected override void Update()
  {
    base.Update();
    this.UpdateMouseWheel();
  }

  public virtual void Exit()
  {
    CollectiblePageDisplay currentCollectiblePage = this.GetCurrentCollectiblePage();
    if ((Object) currentCollectiblePage == (Object) null)
      return;
    currentCollectiblePage.MarkAllShownCardsSeen();
  }

  public void OnCollectionLoaded() => this.ShowOnlyCardsIOwn(new BookPageManager.PageTransitionType?(BookPageManager.PageTransitionType.NONE));

  public void UpdateCurrentPageCardLocks(bool playSound) => this.GetCurrentCollectiblePage().UpdateCurrentPageCardLocks(playSound);

  public void RefreshCurrentPageContents() => this.RefreshCurrentPageContents(BookPageManager.PageTransitionType.NONE, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);

  public void RefreshCurrentPageContents(BookPageManager.PageTransitionType transition) => this.RefreshCurrentPageContents(transition, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);

  public void RefreshCurrentPageContents(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    this.RefreshCurrentPageContents(BookPageManager.PageTransitionType.NONE, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public void RefreshCurrentPageContents(
    BookPageManager.PageTransitionType transition,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    this.UpdateFilteredCards();
    this.TransitionPageWhenReady(transition, true, callback, callbackData);
  }

  public CollectionCardVisual GetCardVisual(string cardID, TAG_PREMIUM premium) => this.GetCurrentCollectiblePage().GetCardVisual(cardID, premium);

  public void FilterByCardSets(List<TAG_CARD_SET> cardSets, bool transitionPage = true) => this.FilterByCardSets(cardSets, (BookPageManager.DelOnPageTransitionComplete) null, (object) null, transitionPage);

  public void FilterByCardSets(
    List<TAG_CARD_SET> cardSets,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    bool transitionPage = true)
  {
    TAG_CARD_SET[] tagCardSetArray = (TAG_CARD_SET[]) null;
    if (cardSets != null && cardSets.Count > 0)
      tagCardSetArray = cardSets.ToArray();
    this.m_cardsCollection.ClearOutFiltersFromSetFilterDropdown();
    this.m_cardsCollection.FilterTheseCardSets(tagCardSetArray);
    this.UpdateFilteredCards();
    if (!transitionPage)
      return;
    this.TransitionPageWhenReady(SceneMgr.Get().IsTransitioning() ? BookPageManager.PageTransitionType.NONE : BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT, false, callback, callbackData);
  }

  public void FilterBySpecificCards(List<int> specificCards)
  {
    this.m_cardsCollection.ClearOutFiltersFromSetFilterDropdown();
    this.m_cardsCollection.FilterSpecificCards(specificCards);
    this.UpdateFilteredCards();
    this.TransitionPageWhenReady(SceneMgr.Get().IsTransitioning() ? BookPageManager.PageTransitionType.NONE : BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public bool CardSetFilterIncludesWild() => this.m_cardsCollection.CardSetFilterIncludesWild();

  public bool CardSetFilterIsClassic() => this.m_cardsCollection.CardSetFilterIsClassicSet();

  public void ChangeSearchTextFilter(string newSearchText, bool transitionPage = true) => this.ChangeSearchTextFilter(newSearchText, (BookPageManager.DelOnPageTransitionComplete) null, (object) null, transitionPage);

  public virtual void ChangeSearchTextFilter(
    string newSearchText,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    bool transitionPage = true)
  {
    this.m_cardsCollection.FilterSearchText(newSearchText);
    this.UpdateFilteredCards();
    if (!transitionPage)
      return;
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.MANY_PAGE_LEFT, false, callback, callbackData);
  }

  public void RemoveSearchTextFilter() => this.RemoveSearchTextFilter((BookPageManager.DelOnPageTransitionComplete) null, (object) null);

  public virtual void RemoveSearchTextFilter(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    bool transitionPage = true)
  {
    this.m_cardsCollection.FilterSearchText((string) null);
    this.UpdateFilteredCards();
    if (!transitionPage)
      return;
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.NONE, false, callback, callbackData);
  }

  public void ShowOnlyCardsIOwn(BookPageManager.PageTransitionType? pageTransition = null) => this.ShowOnlyCardsIOwn((BookPageManager.DelOnPageTransitionComplete) null, (object) null, pageTransition);

  public void ShowOnlyCardsIOwn(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    BookPageManager.PageTransitionType? pageTransition = null)
  {
    this.m_cardsCollection.FilterOnlyOwned(true);
    this.m_cardsCollection.FilterByMask((List<CollectibleCardFilter.FilterMask>) null);
    this.m_cardsCollection.FilterByCraftability(new bool?());
    this.UpdateFilteredCards();
    if (!pageTransition.HasValue)
      return;
    this.TransitionPageWhenReady(pageTransition.Value, false, callback, callbackData);
  }

  public void ShowCardsNotOwned(
    bool includePremiums,
    BookPageManager.PageTransitionType? pageTransition = null)
  {
    this.ShowCardsNotOwned(includePremiums, (BookPageManager.DelOnPageTransitionComplete) null, (object) null, pageTransition);
  }

  public void ShowCardsNotOwned(
    bool includePremiums,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    BookPageManager.PageTransitionType? pageTransition = null)
  {
    this.m_cardsCollection.FilterOnlyOwned(false);
    this.m_cardsCollection.FilterByMask((List<CollectibleCardFilter.FilterMask>) null);
    this.UpdateFilteredCards();
    if (!pageTransition.HasValue)
      return;
    this.TransitionPageWhenReady(pageTransition.Value, false, callback, callbackData);
  }

  public bool JumpToPageWithCard(string cardID, TAG_PREMIUM premium) => this.JumpToPageWithCard(cardID, premium, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);

  public virtual void HideCraftingModeCards(
    BookPageManager.PageTransitionType transitionType = BookPageManager.PageTransitionType.NONE,
    bool updatePage = true)
  {
    this.m_cardsCollection.FilterByCraftability(new bool?());
    this.m_cardsCollection.FilterByMask((List<CollectibleCardFilter.FilterMask>) null);
    this.m_cardsCollection.FilterOnlyOwned(true);
    this.m_cardsCollection.FilterLeagueBannedCardsSubset((HashSet<string>) null);
    this.UpdateFilteredCards();
    if (!updatePage)
      return;
    this.TransitionPageWhenReady(transitionType, false, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public abstract bool JumpToPageWithCard(
    string cardID,
    TAG_PREMIUM premium,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData);

  public abstract void NotifyOfCollectionChanged();

  protected CollectiblePageDisplay GetCurrentCollectiblePage() => this.GetCurrentPage() as CollectiblePageDisplay;

  protected void TransitionPageNextFrame(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData)
  {
    Processor.ScheduleCallback(0.0f, false, (Processor.ScheduledCallback) (userData => this.TransitionPage((object) transitionReadyCallbackData)));
  }

  protected bool AssembleCollectionBasePage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool emptyPage,
    FormatType formatType)
  {
    CollectiblePageDisplay page = transitionReadyCallbackData.m_assembledPage as CollectiblePageDisplay;
    if ((Object) page == (Object) null)
    {
      Log.CollectionManager.PrintError("CollectiblePageManager.AssembleCollectionBasePage - page is null!");
      return false;
    }
    page.UpdateBasePage();
    page.SetPageType(formatType);
    page.ActivatePageCountText(true);
    if (!emptyPage)
      return false;
    this.SetHasPreviousAndNextPages(false, false);
    this.AssembleEmptyPageUI(page, true);
    CollectionManager.Get().GetCollectibleDisplay().CollectionPageContentsChanged<ICollectible>((ICollection<ICollectible>) null, (CollectibleDisplay.CollectionActorsReadyCallback) ((actorList, nonActorCollectibleList, data) =>
    {
      page.UpdateCollectionItems(actorList, nonActorCollectibleList, CollectionManager.Get().GetCollectibleDisplay().GetViewMode());
      this.TransitionPage((object) transitionReadyCallbackData);
    }), (object) null);
    return true;
  }

  protected virtual bool AssembleCollectiblePage<TCollectible>(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    ICollection<TCollectible> collectiblesToDisplay,
    int totalNumPages)
    where TCollectible : ICollectible
  {
    bool emptyPage = collectiblesToDisplay == null || collectiblesToDisplay.Count == 0;
    Log.CollectionManager.Print("transitionPageId={0} pagesTurning={1} currentPageIsPageA={2} emptyPage={3}", (object) this.m_transitionPageId, (object) this.m_pagesCurrentlyTurning, (object) this.m_currentPageIsPageA, (object) emptyPage);
    FormatType themeShowing = CollectionManager.Get().GetThemeShowing();
    return this.AssembleCollectionBasePage(transitionReadyCallbackData, emptyPage, themeShowing);
  }

  protected virtual void UpdateFilteredCards() => this.m_cardsCollection.UpdateResults();

  protected virtual void UpdateMouseWheel()
  {
    if (UniversalInputManager.Get().IsTouchMode() || !this.CanUserTurnPages())
      return;
    double axis = (double) Input.GetAxis("Mouse ScrollWheel");
    if (this.m_hasNextPage && (double) Input.GetAxis("Mouse ScrollWheel") > 0.0)
    {
      if (!UniversalInputManager.Get().InputIsOver(this.GetCurrentPage().gameObject))
        return;
      this.PageRight((BookPageManager.DelOnPageTransitionComplete) null, (object) null);
    }
    else
    {
      if (!this.m_hasPreviousPage || (double) Input.GetAxis("Mouse ScrollWheel") >= 0.0 || !UniversalInputManager.Get().InputIsOver(this.GetCurrentPage().gameObject))
        return;
      this.PageLeft((BookPageManager.DelOnPageTransitionComplete) null, (object) null);
    }
  }

  protected abstract void AssembleEmptyPageUI(
    CollectiblePageDisplay page,
    bool displayNoMatchesText);

  protected abstract void OnCollectionManagerViewModeChanged(
    CollectionUtils.ViewMode prevMode,
    CollectionUtils.ViewMode mode,
    CollectionUtils.ViewModeData userdata,
    bool triggerResponse);
}
