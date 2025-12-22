using Blizzard.T5.MaterialService.Extensions;
using System.Collections;
using UnityEngine;

public abstract class BookPageManager : MonoBehaviour
{
  public GameObject m_pageRightArrowBone;
  public GameObject m_pageLeftArrowBone;
  public PegUIElement m_pageRightClickableRegion;
  public PegUIElement m_pageLeftClickableRegion;
  public PegUIElement m_pageDraggableRegion;
  public BookPageDisplay m_pageDisplayPrefab;
  public PageTurn m_pageTurn;
  public float m_turnLeftPageSwapTiming;
  public bool m_hideArrowsOnPageTurn;
  private static readonly Vector3 CURRENT_PAGE_LOCAL_POS = new Vector3(0.0f, 0.25f, 0.0f);
  private static readonly Vector3 NEXT_PAGE_LOCAL_POS = new Vector3(-300f, 0.0f, -300f);
  private static readonly float ARROW_SCALE_TIME = 0.6f;
  private static readonly string SHOW_ARROWS_COROUTINE_NAME = "WaitThenShowArrows";
  protected BookPageDisplay m_pageA;
  protected BookPageDisplay m_pageB;
  protected int m_currentPageNum;
  protected int m_transitionPageId;
  protected bool m_currentPageIsPageA;
  private bool m_fullyLoaded;
  protected bool m_skipNextPageTurn;
  protected PagingArrow m_pageRightArrow;
  protected PagingArrow m_pageLeftArrow;
  private bool m_rightArrowShown;
  private bool m_leftArrowShown;
  protected bool m_hasPreviousPage;
  protected bool m_hasNextPage;
  private bool m_delayShowingArrows;
  private bool m_pageTurnDisabled;
  protected bool m_pagesCurrentlyTurning;
  protected bool m_wasTouchModeEnabled;

  public event BookPageManager.PageTurnStartCallback PageTurnStart;

  public event BookPageManager.PageTurnCompleteCallback PageTurnComplete;

  protected virtual void Awake()
  {
    this.m_pageLeftClickableRegion.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPageLeftPressed));
    this.m_pageLeftClickableRegion.SetCursorOver(PegCursor.Mode.LEFTARROW);
    this.m_pageLeftClickableRegion.SetCursorDown(PegCursor.Mode.LEFTARROW);
    this.m_pageRightClickableRegion.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPageRightPressed));
    this.m_pageRightClickableRegion.SetCursorOver(PegCursor.Mode.RIGHTARROW);
    this.m_pageRightClickableRegion.SetCursorDown(PegCursor.Mode.RIGHTARROW);
    this.m_wasTouchModeEnabled = UniversalInputManager.Get().IsTouchMode();
    if (UniversalInputManager.Get().IsTouchMode())
      this.gameObject.AddComponent<CollectionPageManagerTouchBehavior>();
    this.m_pageA = Object.Instantiate<BookPageDisplay>(this.m_pageDisplayPrefab);
    this.m_pageB = Object.Instantiate<BookPageDisplay>(this.m_pageDisplayPrefab);
    TransformUtil.AttachAndPreserveLocalTransform(this.m_pageA.transform, this.transform);
    TransformUtil.AttachAndPreserveLocalTransform(this.m_pageB.transform, this.transform);
  }

  protected virtual void Start() => this.StartCoroutine(this.WaitForPagesToBeFullyLoaded());

  private IEnumerator WaitForPagesToBeFullyLoaded()
  {
    while (!this.m_pageA.IsLoaded() || !this.m_pageB.IsLoaded())
      yield return (object) null;
    this.m_fullyLoaded = true;
    BookPageDisplay alternatePage = this.GetAlternatePage();
    BookPageDisplay currentPage = this.GetCurrentPage();
    this.AssembleEmptyPageUI(alternatePage);
    this.AssembleEmptyPageUI(currentPage);
    this.PositionNextPage(alternatePage);
    this.PositionCurrentPage(currentPage);
  }

  protected virtual void Update()
  {
    if (this.m_wasTouchModeEnabled == UniversalInputManager.Get().IsTouchMode())
      return;
    this.HandleTouchModeChanged();
  }

  public int CurrentPageNum => this.m_currentPageNum;

  public void OnBookOpening()
  {
    this.StopCoroutine(BookPageManager.SHOW_ARROWS_COROUTINE_NAME);
    this.StartCoroutine(BookPageManager.SHOW_ARROWS_COROUTINE_NAME);
  }

  public bool ArePagesTurning() => this.m_pagesCurrentlyTurning;

  public int GetTransitionPageId() => this.m_transitionPageId;

  public bool IsFullyLoaded() => this.m_fullyLoaded;

  protected virtual bool CanUserTurnPages() => !this.m_pagesCurrentlyTurning && !this.m_pageTurnDisabled;

  public void FlipToPage(
    int newPageNum,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData,
    BookPageManager.PageTransitionType transitionType)
  {
    this.m_currentPageNum = newPageNum;
    this.TransitionPageWhenReady(transitionType, true, callback, callbackData);
  }

  public void FlipToPage(
    int newPageNum,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    BookPageManager.PageTransitionType transitionType = newPageNum != this.m_currentPageNum - 1 ? (newPageNum != this.m_currentPageNum + 1 ? (newPageNum < this.m_currentPageNum ? BookPageManager.PageTransitionType.MANY_PAGE_LEFT : BookPageManager.PageTransitionType.MANY_PAGE_RIGHT) : BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT) : BookPageManager.PageTransitionType.SINGLE_PAGE_LEFT;
    this.FlipToPage(newPageNum, callback, callbackData, transitionType);
  }

  private void SwapCurrentAndAltPages() => this.m_currentPageIsPageA = !this.m_currentPageIsPageA;

  protected BookPageDisplay GetCurrentPage() => !this.m_currentPageIsPageA ? this.m_pageB : this.m_pageA;

  protected BookPageDisplay GetAlternatePage() => !this.m_currentPageIsPageA ? this.m_pageA : this.m_pageB;

  protected virtual void TransitionPageWhenReady(
    BookPageManager.PageTransitionType transitionType,
    bool useCurrentPageNum,
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    Log.CollectionManager.Print("transitionPageId={0} pagesTurning={1} transitionType={2} currentPageIsPageA={3}", (object) this.m_transitionPageId, (object) this.m_pagesCurrentlyTurning, (object) transitionType, (object) this.m_currentPageIsPageA);
    if (this.m_pagesCurrentlyTurning)
      Debug.LogWarning((object) "TransitionPageWhenReady() called when m_pagesCurrentlyTurning is already true! You probably don't want to allow this [see usages of CanUserTurnPages()].");
    if ((Object) HeroPickerDisplay.Get() != (Object) null && HeroPickerDisplay.Get().IsShown())
      transitionType = BookPageManager.PageTransitionType.NONE;
    this.m_pagesCurrentlyTurning = true;
    if (this.PageTurnStart != null)
      this.PageTurnStart(transitionType);
    if (this.m_hideArrowsOnPageTurn && transitionType != BookPageManager.PageTransitionType.NONE && (Object) this.m_pageLeftArrow != (Object) null && (Object) this.m_pageRightArrow != (Object) null)
    {
      this.m_pageLeftArrowBone.SetActive(false);
      this.m_pageRightArrowBone.SetActive(false);
    }
    this.SwapCurrentAndAltPages();
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData = new BookPageManager.TransitionReadyCallbackData()
    {
      m_assembledPage = this.GetCurrentPage(),
      m_otherPage = this.GetAlternatePage(),
      m_transitionType = transitionType,
      m_callback = callback,
      m_callbackData = callbackData
    };
    switch (transitionType)
    {
      case BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT:
      case BookPageManager.PageTransitionType.MANY_PAGE_RIGHT:
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_book_page_flip_forward.prefab:07282310dd70fee4ca2dfdb37c545acc");
        break;
      case BookPageManager.PageTransitionType.SINGLE_PAGE_LEFT:
      case BookPageManager.PageTransitionType.MANY_PAGE_LEFT:
        SoundManager.Get().LoadAndPlay((AssetReference) "collection_manager_book_page_flip_back.prefab:371e496e1cd371144abfec472e72d9a9");
        break;
    }
    this.AssemblePage(transitionReadyCallbackData, useCurrentPageNum);
    this.OnPageTransitionRequested();
  }

  protected abstract void AssemblePage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum);

  protected virtual void AssembleEmptyPageUI(BookPageDisplay page) => this.SetHasPreviousAndNextPages(false, false);

  private void PositionCurrentPage(BookPageDisplay page) => page.transform.localPosition = BookPageManager.CURRENT_PAGE_LOCAL_POS;

  private void PositionNextPage(BookPageDisplay page) => page.transform.localPosition = BookPageManager.NEXT_PAGE_LOCAL_POS;

  protected virtual void TransitionPage(object callbackData)
  {
    ++this.m_transitionPageId;
    int transitionPageId = this.m_transitionPageId;
    BookPageManager.TransitionReadyCallbackData callbackData1 = callbackData as BookPageManager.TransitionReadyCallbackData;
    BookPageDisplay assembledPage = callbackData1.m_assembledPage;
    BookPageDisplay otherPage = callbackData1.m_otherPage;
    Log.CollectionManager.Print("transitionPageId={0} pagesTurning={1} transitionType={2} currentPageIsPageA={3}", (object) this.m_transitionPageId, (object) this.m_pagesCurrentlyTurning, (object) callbackData1.m_transitionType, (object) this.m_currentPageIsPageA);
    if ((Object) assembledPage.m_basePageRenderer != (Object) null)
      this.m_pageTurn.SetBackPageMaterial(RendererExtension.GetMaterial((Renderer) assembledPage.m_basePageRenderer));
    else
      Debug.LogError((object) "No Base Page Renderer is set for the assembled page! Back Page Material cannot be properly set!");
    BookPageManager.PageTransitionType pageTransitionType = callbackData1.m_transitionType;
    if (TavernBrawlDisplay.IsTavernBrawlViewing() || SceneMgr.Get().IsInDuelsMode() && !PvPDungeonRunScene.IsEditingDeck())
      pageTransitionType = BookPageManager.PageTransitionType.NONE;
    if (this.m_skipNextPageTurn)
    {
      pageTransitionType = BookPageManager.PageTransitionType.NONE;
      this.m_skipNextPageTurn = false;
    }
    switch (pageTransitionType)
    {
      case BookPageManager.PageTransitionType.NONE:
        this.PositionNextPage(otherPage);
        this.PositionCurrentPage(assembledPage);
        this.OnPageTurnComplete((object) callbackData1, transitionPageId);
        break;
      case BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT:
      case BookPageManager.PageTransitionType.MANY_PAGE_RIGHT:
        this.m_pageTurn.TurnRight(otherPage.gameObject, assembledPage.gameObject, (PageTurn.DelOnPageTurnComplete) (data => this.OnPageTurnComplete(data, transitionPageId)), (PageTurn.DelPositionPages) (data => this.PositionPages(data, transitionPageId)), (object) callbackData1);
        break;
      case BookPageManager.PageTransitionType.SINGLE_PAGE_LEFT:
      case BookPageManager.PageTransitionType.MANY_PAGE_LEFT:
        this.m_pageTurn.TurnLeft(assembledPage.gameObject, otherPage.gameObject, (PageTurn.DelOnPageTurnComplete) (data => this.OnPageTurnComplete(data, transitionPageId)), (PageTurn.DelPositionPages) (data => this.PositionPages(data, transitionPageId)), (object) callbackData1);
        break;
    }
  }

  protected virtual void OnPageTurnComplete(object callbackData, int operationId)
  {
    BookPageManager.TransitionReadyCallbackData readyCallbackData = callbackData as BookPageManager.TransitionReadyCallbackData;
    Log.CollectionManager.Print("transitionPageId={0} vs {1} | assembledIsCurrent={2}, pagesTurning={3} currentPageIsPageA={4}", (object) operationId, (object) this.m_transitionPageId, (object) ((Object) readyCallbackData.m_assembledPage == (Object) this.GetCurrentPage()), (object) this.m_pagesCurrentlyTurning, (object) this.m_currentPageIsPageA);
    if (operationId != this.m_transitionPageId)
    {
      if (readyCallbackData.m_callback != null)
        readyCallbackData.m_callback(readyCallbackData.m_callbackData);
      if (this.PageTurnComplete != null)
        this.PageTurnComplete(this.m_currentPageNum);
      Log.CollectionManager.PrintWarning("transitionPageId={0} vs {1} | EARLY OUT!", (object) operationId, (object) this.m_transitionPageId);
    }
    else
    {
      BookPageDisplay assembledPage = readyCallbackData.m_assembledPage;
      BookPageDisplay otherPage = readyCallbackData.m_otherPage;
      if ((Object) otherPage != (Object) this.GetCurrentPage())
      {
        assembledPage.Show();
        otherPage.Hide();
      }
      if (readyCallbackData.m_callback != null)
        readyCallbackData.m_callback(readyCallbackData.m_callbackData);
      if ((Object) readyCallbackData.m_assembledPage == (Object) this.GetCurrentPage())
      {
        if (this.m_hideArrowsOnPageTurn && (Object) this.m_pageLeftArrow != (Object) null && (Object) this.m_pageRightArrow != (Object) null)
        {
          this.m_pageLeftArrowBone.SetActive(true);
          this.m_pageRightArrowBone.SetActive(true);
        }
        Log.CollectionManager.Print("transitionPageId={0} vs {1} | set m_pagesCurrentlyTurning = false", (object) operationId, (object) this.m_transitionPageId);
        this.m_pagesCurrentlyTurning = false;
      }
      if (this.PageTurnComplete == null)
        return;
      this.PageTurnComplete(this.m_currentPageNum);
    }
  }

  private void PositionPages(object callbackData, int operationId)
  {
    BookPageManager.TransitionReadyCallbackData readyCallbackData = callbackData as BookPageManager.TransitionReadyCallbackData;
    if (operationId != this.m_transitionPageId)
      return;
    this.PositionCurrentPage(readyCallbackData.m_assembledPage);
    this.PositionNextPage(readyCallbackData.m_otherPage);
  }

  protected virtual void PageRight(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    ++this.m_currentPageNum;
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT, true, callback, callbackData);
  }

  protected virtual void PageLeft(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    --this.m_currentPageNum;
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.SINGLE_PAGE_LEFT, true, callback, callbackData);
  }

  public void EnablePageTurn(bool enable)
  {
    this.m_pageTurnDisabled = !enable;
    this.RefreshPageTurnButtons();
  }

  public void EnablePageTurnArrows(bool enable)
  {
    this.ShowArrow(this.m_pageLeftArrow, enable && this.m_hasPreviousPage, false);
    this.ShowArrow(this.m_pageRightArrow, enable && this.m_hasNextPage, true);
  }

  protected void SetHasPreviousAndNextPages(bool hasPreviousPage, bool hasNextPage)
  {
    this.m_hasPreviousPage = hasPreviousPage;
    this.m_hasNextPage = hasNextPage;
    this.RefreshPageTurnButtons();
  }

  protected void RefreshPageTurnButtons()
  {
    bool enabled1 = !this.m_pageTurnDisabled && this.m_hasPreviousPage;
    this.m_pageLeftClickableRegion.enabled = enabled1;
    this.m_pageLeftClickableRegion.SetEnabled(enabled1);
    bool enabled2 = !this.m_pageTurnDisabled && this.m_hasNextPage;
    this.m_pageRightClickableRegion.enabled = enabled2;
    this.m_pageRightClickableRegion.SetEnabled(enabled2);
    this.ShowArrow(this.m_pageLeftArrow, this.m_hasPreviousPage, false);
    this.ShowArrow(this.m_pageRightArrow, this.m_hasNextPage, true);
  }

  private void OnPageLeftPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    this.PageLeft((BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  protected abstract void OnPageTransitionRequested();

  private void OnPageRightPressed(UIEvent e)
  {
    if (!this.CanUserTurnPages())
      return;
    this.PageRight((BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public void LoadPagingArrows()
  {
    if ((bool) (Object) this.m_pageLeftArrow && (bool) (Object) this.m_pageRightArrow)
      return;
    AssetLoader.Get().InstantiatePrefab((AssetReference) "PagingArrow.prefab:70d4430ff418d6d42a943e77dc98d523", new PrefabCallback<GameObject>(this.OnPagingArrowLoaded));
  }

  private void OnPagingArrowLoaded(AssetReference assetRef, GameObject go, object callbackData)
  {
    if ((Object) go == (Object) null)
      return;
    if (!(bool) (Object) this.m_pageLeftArrow)
    {
      this.m_pageLeftArrow = go.GetComponent<PagingArrow>();
      this.m_pageLeftArrow.transform.parent = this.m_pageLeftArrowBone.transform;
      this.m_pageLeftArrow.transform.localEulerAngles = Vector3.zero;
      this.m_pageLeftArrow.transform.position = this.m_pageLeftArrowBone.transform.position;
      this.m_pageLeftArrow.transform.localScale = Vector3.zero;
      LayerUtils.SetLayer((Component) this.m_pageLeftArrow, GameLayer.TransparentFX);
    }
    if (!(bool) (Object) this.m_pageRightArrow)
    {
      this.m_pageRightArrow = Object.Instantiate<PagingArrow>(this.m_pageLeftArrow);
      this.m_pageRightArrow.transform.parent = this.m_pageRightArrowBone.transform;
      this.m_pageRightArrow.transform.localEulerAngles = Vector3.zero;
      this.m_pageRightArrow.transform.position = this.m_pageRightArrowBone.transform.position;
      this.m_pageRightArrow.transform.localScale = Vector3.zero;
      LayerUtils.SetLayer((Component) this.m_pageRightArrow, GameLayer.TransparentFX);
    }
    this.RefreshPageTurnButtons();
  }

  protected void ShowPagingArrowHighlight() => this.m_pageRightArrow.ShowHighlight();

  private IEnumerator WaitThenShowArrows()
  {
    if (!((Object) this.m_pageLeftArrow == (Object) null) || !((Object) this.m_pageRightArrow == (Object) null))
    {
      this.m_delayShowingArrows = true;
      yield return (object) new WaitForSeconds(1f);
      this.m_delayShowingArrows = false;
      this.RefreshPageTurnButtons();
    }
  }

  private void ShowArrow(PagingArrow arrow, bool show, bool isRightArrow)
  {
    if ((Object) arrow == (Object) null || this.m_delayShowingArrows & show)
      return;
    if (isRightArrow)
    {
      if (this.m_rightArrowShown == show)
        return;
      this.m_rightArrowShown = show;
    }
    else
    {
      if (this.m_leftArrowShown == show)
        return;
      this.m_leftArrowShown = show;
    }
    Vector3 vector3 = show ? new Vector3(1f, 1f, 1f) : Vector3.zero;
    iTween.EaseType easeType = show ? iTween.EaseType.easeOutElastic : iTween.EaseType.linear;
    Hashtable args = iTween.Hash((object) "scale", (object) vector3, (object) "time", (object) BookPageManager.ARROW_SCALE_TIME, (object) "easetype", (object) easeType, (object) "name", (object) "ArrowScale");
    iTween.StopByName(arrow.gameObject, "ArrowScale");
    iTween.ScaleTo(arrow.gameObject, args);
  }

  protected virtual void HandleTouchModeChanged()
  {
    bool flag = UniversalInputManager.Get().IsTouchMode();
    if (this.m_wasTouchModeEnabled == flag)
    {
      Debug.LogWarning((object) "Touch mode did not change, why are you calling BookPageManager.HandleTouchModeChanged()?");
    }
    else
    {
      this.m_wasTouchModeEnabled = flag;
      if (flag)
        this.gameObject.AddComponent<CollectionPageManagerTouchBehavior>();
      else
        Object.Destroy((Object) this.gameObject.GetComponent<CollectionPageManagerTouchBehavior>());
    }
  }

  public enum PageTransitionType
  {
    NONE,
    SINGLE_PAGE_RIGHT,
    SINGLE_PAGE_LEFT,
    MANY_PAGE_RIGHT,
    MANY_PAGE_LEFT,
  }

  public delegate void DelOnPageTransitionComplete(object callbackData);

  public delegate void PageTurnStartCallback(BookPageManager.PageTransitionType transitionType);

  public delegate void PageTurnCompleteCallback(int currentPageNum);

  protected class TransitionReadyCallbackData
  {
    public BookPageDisplay m_assembledPage;
    public BookPageDisplay m_otherPage;
    public BookPageManager.PageTransitionType m_transitionType;
    public BookPageManager.DelOnPageTransitionComplete m_callback;
    public object m_callbackData;
  }
}
