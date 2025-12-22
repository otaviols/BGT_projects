using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureBookPageManager : BookPageManager
{
  public Transform m_UnlockChapterTooltipBone;
  private const string PAGE_CLICKED_EVENT_NAME = "PAGE_CLICKED";
  private const string FLIP_TO_SECTION_1_EVENT_NAME = "FLIP_TO_SECTION_1";
  private const string FLIP_TO_SECTION_2_EVENT_NAME = "FLIP_TO_SECTION_2";
  private List<PageNode> m_pageNodes = new List<PageNode>();
  private int m_mapPageNumber;
  private Notification m_unlockChapterTooltip;
  private bool m_allInitialTransitionsComplete;
  public static AdventureBookPageManager m_instance;

  public event AdventureBookPageManager.PageClickCallback PageClicked;

  private int CurrentPageIndex => Mathf.Max(0, this.m_currentPageNum - 1);

  public int NumChapters { get; private set; }

  private int DefaultPageNum => this.m_mapPageNumber <= 0 ? 1 : this.m_mapPageNumber;

  private bool HasMapPage => this.m_mapPageNumber > 0;

  protected override void Start()
  {
    AdventureBookPageManager.m_instance = this;
    base.Start();
    this.LoadPagingArrows();
    AdventureBookPageDisplay adventureBookPageDisplay1 = this.PageAsAdventureBookPage(this.m_pageA);
    if ((UnityEngine.Object) adventureBookPageDisplay1 != (UnityEngine.Object) null)
    {
      adventureBookPageDisplay1.SetPageEventListener(new Widget.EventListenerDelegate(this.PageEventListener));
      adventureBookPageDisplay1.SetFlipToChapterCallback(new AdventureBookPageDisplay.FlipToChapterCallback(this.FlipToChapter));
    }
    AdventureBookPageDisplay adventureBookPageDisplay2 = this.PageAsAdventureBookPage(this.m_pageB);
    if ((UnityEngine.Object) adventureBookPageDisplay2 != (UnityEngine.Object) null)
    {
      adventureBookPageDisplay2.SetPageEventListener(new Widget.EventListenerDelegate(this.PageEventListener));
      adventureBookPageDisplay2.SetFlipToChapterCallback(new AdventureBookPageDisplay.FlipToChapterCallback(this.FlipToChapter));
    }
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
  }

  private void OnDestroy()
  {
    this.StopCoroutine("ShowUnlockChapterTooltipWhenNoQuotePlaying");
    if ((UnityEngine.Object) this.m_unlockChapterTooltip != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_unlockChapterTooltip);
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
  }

  public void Initialize(List<PageNode> pageNodes, int numChapters)
  {
    if (!this.IsFullyLoaded())
      Debug.LogWarning((object) "AdventureBookPageManager is not fully loaded yet, you should not be calling Initialize()!");
    for (int index = 0; index < pageNodes.Count; ++index)
    {
      if (pageNodes[index].PageData.PageType == AdventureBookPageType.MAP)
      {
        this.m_mapPageNumber = AdventureBookPageManager.PageNodeIndexToPageNum(index);
        break;
      }
    }
    this.m_pageNodes = pageNodes;
    this.NumChapters = numChapters;
    ScenarioDbId mission = AdventureConfig.Get().GetMission();
    int pageNum = 0;
    if (mission != ScenarioDbId.INVALID)
      this.m_currentPageNum = this.GetPageNumFromScenarioId(mission);
    else if (this.PageExistsWithUnAckedCompletion(out pageNum))
      this.m_currentPageNum = pageNum;
    else
      this.m_currentPageNum = this.DefaultPageNum;
    this.TransitionPageWhenReady(BookPageManager.PageTransitionType.NONE, true, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  public PageData GetPageDataForCurrentPage() => this.NumPages == 0 || this.CurrentPageIndex < 0 || this.CurrentPageIndex >= this.NumPages ? (PageData) null : this.m_pageNodes[this.CurrentPageIndex].PageData;

  public int GetNumChaptersOwned()
  {
    int numChaptersOwned = 0;
    foreach (PageNode pageNode in this.m_pageNodes)
    {
      if (pageNode.PageData is ChapterPageData pageData && AdventureProgressMgr.Get().OwnsWing(pageData.WingRecord.ID))
        ++numChaptersOwned;
    }
    return numChaptersOwned;
  }

  public AdventureBookPageDataModel GetCurrentPageDataModel() => ((AdventureBookPageDisplay) this.GetCurrentPage()).GetAdventurePageDataModel();

  public void AllInitialTransitionsComplete()
  {
    ((AdventureBookPageDisplay) this.m_pageA).AllInitialTransitionsComplete();
    ((AdventureBookPageDisplay) this.m_pageB).AllInitialTransitionsComplete();
    this.m_allInitialTransitionsComplete = true;
    this.ShowUnlockChapterTooltipIfNecessary();
  }

  public void SetEnableInteractionCallback(
    AdventureBookPageDisplay.EnableInteractionCallback callback)
  {
    ((AdventureBookPageDisplay) this.m_pageA).SetEnableInteractionCallback(callback);
    ((AdventureBookPageDisplay) this.m_pageB).SetEnableInteractionCallback(callback);
  }

  public void HideAllPopups()
  {
    this.StopCoroutine("ShowUnlockChapterTooltipWhenNoQuotePlaying");
    if ((UnityEngine.Object) this.m_unlockChapterTooltip != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotification(this.m_unlockChapterTooltip, 0.0f);
    AdventureBookPageDisplay currentPage = this.GetCurrentPage() as AdventureBookPageDisplay;
    if (!((UnityEngine.Object) currentPage != (UnityEngine.Object) null))
      return;
    currentPage.HideAndSuppressChapterUnlockSequence();
  }

  protected override void AssemblePage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum)
  {
    if (this.CurrentPageIndex < 0 || this.CurrentPageIndex >= this.m_pageNodes.Count)
    {
      Debug.LogErrorFormat("AdventureBookPageManager.AssemblePage - CurrentPageIndex ({0}) is out of bounds! Unable to assemble the current page.", (object) this.CurrentPageIndex);
    }
    else
    {
      PageNode pageNode = this.m_pageNodes[this.CurrentPageIndex];
      if (pageNode == null)
      {
        Debug.LogErrorFormat("AdventureBookPageManager.AssemblePage - PageNode object at index {0} is null! Unable to assemble the current page.", (object) this.CurrentPageIndex);
      }
      else
      {
        PageData pageData = pageNode.PageData;
        if (pageData == null)
        {
          Debug.LogErrorFormat("AdventureBookPageManager.AssemblePage - PageData object at index {0} is null! Unable to assemble the current page.", (object) this.CurrentPageIndex);
        }
        else
        {
          AdventureDataModel adventureDataModel = AdventureConfig.Get().GetAdventureDataModel();
          if (adventureDataModel != null)
            adventureDataModel.IsAdventureComplete = AdventureProgressMgr.Get().IsAdventureComplete(pageData.Adventure);
          if (pageData.PageType == AdventureBookPageType.MAP)
          {
            AdventureBookPageManager.RemoveMapPageNavigation();
            this.AssembleMapPage(transitionReadyCallbackData, useCurrentPageNum);
          }
          else if (pageData.PageType == AdventureBookPageType.REWARD)
          {
            AdventureBookPageManager.SaveMapPageNavigation();
            this.AssembleCardBackRewardPage(transitionReadyCallbackData, useCurrentPageNum);
          }
          else
          {
            AdventureBookPageManager.SaveMapPageNavigation();
            this.AssembleChapterPage(transitionReadyCallbackData, useCurrentPageNum);
          }
          this.SetHasPreviousAndNextPages(pageNode.PageToLeft != null, pageNode.PageToRight != null);
        }
      }
    }
  }

  private void AssembleMapPage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum)
  {
    Log.Adventures.Print("Assembling Map page.");
    AdventureBookPageDisplay adventureBookPageDisplay = this.PageAsAdventureBookPage(transitionReadyCallbackData.m_assembledPage);
    int num = 0;
    foreach (PageNode pageNode in this.m_pageNodes)
    {
      if (pageNode.PageData.PageType == AdventureBookPageType.CHAPTER && pageNode.PageData is ChapterPageData pageData)
      {
        int id = pageData.WingRecord.ID;
        if (AdventureProgressMgr.Get().IsWingComplete((AdventureDbId) pageData.WingRecord.AdventureId, pageData.AdventureMode, (WingDbId) id))
          ++num;
      }
    }
    AdventureDataModel adventureDataModel = AdventureConfig.Get().GetAdventureDataModel();
    if (adventureDataModel != null)
    {
      adventureDataModel.AllChaptersCompleted = num >= this.NumChapters;
      adventureDataModel.MapNewlyCompleted = adventureDataModel.AllChaptersCompleted && AdventureBookPageDisplay.NeedToShowAdventureSectionCompletionSequence;
    }
    if (!(this.m_pageNodes[this.CurrentPageIndex].PageData is MapPageData pageData1))
    {
      Debug.LogErrorFormat("Page Data at index {0} is not a MapPageData object!", (object) this.CurrentPageIndex);
    }
    else
    {
      pageData1.NumChaptersCompletedText = GameStrings.Format("GLUE_NUM_CHAPTERS_COMPLETED", (object) num, (object) this.NumChapters);
      adventureBookPageDisplay.SetUpPage((PageData) pageData1, (AdventureBookPageDisplay.PageReadyCallback) (() => this.TransitionPage((object) transitionReadyCallbackData)));
    }
  }

  private void AssembleChapterPage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum)
  {
    if (this.CurrentPageIndex < 0 || this.CurrentPageIndex >= this.m_pageNodes.Count)
    {
      Log.Adventures.PrintError("Page Index {0} is invalid; there are only {1} Chapter Pages!", (object) this.CurrentPageIndex, (object) this.m_pageNodes.Count);
    }
    else
    {
      ChapterPageData pageData = this.m_pageNodes[this.CurrentPageIndex].PageData as ChapterPageData;
      Log.Adventures.Print("Assembling Chapter page for chapter {0}, Wing {1}.", (object) pageData.ChapterNumber, (object) pageData.WingRecord.Name);
      this.PageAsAdventureBookPage(transitionReadyCallbackData.m_assembledPage).SetUpPage((PageData) pageData, (AdventureBookPageDisplay.PageReadyCallback) (() => this.TransitionPage((object) transitionReadyCallbackData)));
    }
  }

  private void AssembleCardBackRewardPage(
    BookPageManager.TransitionReadyCallbackData transitionReadyCallbackData,
    bool useCurrentPageNum)
  {
    Log.Adventures.Print("Assembling CardBack page.");
    this.PageAsAdventureBookPage(transitionReadyCallbackData.m_assembledPage).SetUpPage(this.m_pageNodes[this.CurrentPageIndex].PageData, (AdventureBookPageDisplay.PageReadyCallback) (() => this.TransitionPage((object) transitionReadyCallbackData)));
  }

  protected override void OnPageTurnComplete(object callbackData, int operationId)
  {
    base.OnPageTurnComplete(callbackData, operationId);
    this.ShowUnlockChapterTooltipIfNecessary();
  }

  private int GetPageNumFromScenarioId(ScenarioDbId scenarioId)
  {
    if (scenarioId == ScenarioDbId.INVALID)
      return this.DefaultPageNum;
    for (int index = 0; index < this.m_pageNodes.Count; ++index)
    {
      if (this.m_pageNodes[index].PageData.PageType == AdventureBookPageType.CHAPTER && this.m_pageNodes[index].PageData is ChapterPageData pageData && pageData.ScenarioRecords.Exists((Predicate<ScenarioDbfRecord>) (r => (ScenarioDbId) r.ID == scenarioId)))
        return AdventureBookPageManager.PageNodeIndexToPageNum(index);
    }
    return this.DefaultPageNum;
  }

  private bool PageExistsWithUnAckedCompletion(out int pageNum)
  {
    pageNum = this.DefaultPageNum;
    for (int index = 0; index < this.m_pageNodes.Count; ++index)
    {
      PageData pageData = this.m_pageNodes[index].PageData;
      if (pageData != null && pageData.PageType == AdventureBookPageType.CHAPTER && pageData is ChapterPageData chapterPageData)
      {
        int id = chapterPageData.WingRecord.ID;
        bool wingHasUnackedProgress;
        if (AdventureProgressMgr.Get().IsWingComplete(chapterPageData.Adventure, chapterPageData.AdventureMode, (WingDbId) id, out wingHasUnackedProgress) & wingHasUnackedProgress)
        {
          pageNum = AdventureBookPageManager.PageNodeIndexToPageNum(index);
          return true;
        }
      }
    }
    return false;
  }

  private static int PageNodeIndexToPageNum(int pageNodeIndex) => pageNodeIndex + 1;

  private int ChapterNumberToPageNum(int chapterNumber) => this.m_mapPageNumber == 1 ? chapterNumber + 1 : chapterNumber;

  private void FlipToChapter(int chapterNumber)
  {
    if (!this.CanUserTurnPages())
      return;
    this.FlipToPage(this.ChapterNumberToPageNum(chapterNumber), (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
  }

  private void FlipToFirstUncompletedPage(int section)
  {
    int chapterNumber = 0;
    ChapterPageData chapterPageData = (ChapterPageData) null;
    foreach (PageNode pageNode in this.m_pageNodes)
    {
      if (pageNode.PageData is ChapterPageData pageData && pageData.BookSection == section)
      {
        chapterPageData = pageData;
        bool wingHasUnackedProgress = false;
        if (!AdventureProgressMgr.Get().IsWingComplete(pageData.Adventure, pageData.AdventureMode, (WingDbId) pageData.WingRecord.ID, out wingHasUnackedProgress) | wingHasUnackedProgress)
        {
          chapterNumber = pageData.ChapterNumber;
          break;
        }
      }
    }
    if (chapterNumber == 0 && chapterPageData != null)
      chapterNumber = chapterPageData.ChapterNumber;
    this.FlipToChapter(chapterNumber);
  }

  private void PageClickedCallback()
  {
    if (this.PageClicked == null)
      return;
    this.PageClicked();
  }

  protected override void OnPageTransitionRequested()
  {
    if ((UnityEngine.Object) this.m_pageRightArrow != (UnityEngine.Object) null)
      this.m_pageRightArrow.HideHighlight();
    this.HideAllPopups();
  }

  private void PageEventListener(string eventName)
  {
    if (!(eventName == "FLIP_TO_SECTION_1"))
    {
      if (!(eventName == "FLIP_TO_SECTION_2"))
      {
        if (!(eventName == "PAGE_CLICKED"))
          return;
        this.PageClickedCallback();
      }
      else
        this.FlipToFirstUncompletedPage(1);
    }
    else
      this.FlipToFirstUncompletedPage(0);
  }

  private AdventureBookPageDisplay PageAsAdventureBookPage(
    BookPageDisplay page)
  {
    AdventureBookPageDisplay adventureBookPageDisplay = page as AdventureBookPageDisplay;
    if (!((UnityEngine.Object) adventureBookPageDisplay == (UnityEngine.Object) null))
      return adventureBookPageDisplay;
    Log.CollectionManager.PrintError("Page in AdventureBookPageManager is not a AdventureBookPageDisplay!  This should not happen!");
    return adventureBookPageDisplay;
  }

  private int NumPages => this.m_pageNodes.Count;

  private void OnSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod purchaseMethod)
  {
    AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
    AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) selectedAdventure);
    if (record != null && record.MapPageHasButtonsToChapters)
    {
      if (!AdventureUtils.DoesBundleIncludeWingForAdventure(bundle, selectedAdventure))
        return;
      AdventureBookPageManager.NavigateToMapPage();
    }
    else
    {
      AdventureBookPageDisplay adventureBookPageDisplay = this.PageAsAdventureBookPage(this.GetCurrentPage());
      if ((UnityEngine.Object) adventureBookPageDisplay == (UnityEngine.Object) null)
      {
        Debug.LogWarning((object) "AdventureBookPageManager.OnSuccessfulPurchaseAck() - No current page on which to play an unlock sequence!");
      }
      else
      {
        if (!adventureBookPageDisplay.DoesBundleApplyToPage(bundle))
          return;
        adventureBookPageDisplay.ShowNewlyPurchasedSequenceOnChapterPage();
      }
    }
  }

  public static bool NavigateToMapPage()
  {
    if ((UnityEngine.Object) AdventureBookPageManager.m_instance == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("Trying to navigate to map page, but AdventureBookPageManager has been destroyed!");
      return false;
    }
    if (!AdventureBookPageManager.m_instance.HasMapPage)
    {
      Debug.LogError((object) "This Adventure Book does not have a Map Page, you should not be calling NavigateToMapPage()!");
      return false;
    }
    if (AdventureBookPageManager.m_instance.m_currentPageNum == AdventureBookPageManager.m_instance.m_mapPageNumber)
      return false;
    AdventureBookPageManager.m_instance.FlipToPage(AdventureBookPageManager.m_instance.m_mapPageNumber, (BookPageManager.DelOnPageTransitionComplete) null, (object) null);
    return true;
  }

  private static void SaveMapPageNavigation()
  {
    if ((UnityEngine.Object) AdventureBookPageManager.m_instance == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("Trying to add map page to navigation stack, but AdventureBookPageManager has been destroyed!");
    }
    else
    {
      if (!AdventureBookPageManager.m_instance.HasMapPage)
        return;
      Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureBookPageManager.NavigateToMapPage));
    }
  }

  private static void RemoveMapPageNavigation()
  {
    if ((UnityEngine.Object) AdventureBookPageManager.m_instance == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("Trying to remove map page from navigation stack, but AdventureBookPageManager has been destroyed!");
    }
    else
    {
      if (!AdventureBookPageManager.m_instance.HasMapPage)
        return;
      Navigation.RemoveHandler(new Navigation.NavigateBackHandler(AdventureBookPageManager.NavigateToMapPage));
    }
  }

  private void ShowUnlockChapterTooltipIfNecessary()
  {
    if (!this.m_allInitialTransitionsComplete || !AdventureConfig.Get().ShouldSeeFirstTimeFlow || this.m_currentPageNum != this.m_mapPageNumber)
      return;
    this.StopCoroutine("ShowUnlockChapterTooltipWhenNoQuotePlaying");
    this.StartCoroutine("ShowUnlockChapterTooltipWhenNoQuotePlaying");
  }

  private IEnumerator ShowUnlockChapterTooltipWhenNoQuotePlaying()
  {
    AdventureBookPageManager adventureBookPageManager = this;
    while (NotificationManager.Get().IsQuotePlaying)
      yield return (object) null;
    if ((UnityEngine.Object) adventureBookPageManager.m_unlockChapterTooltip != (UnityEngine.Object) null)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(adventureBookPageManager.m_unlockChapterTooltip);
    Notification.PopUpArrowDirection direction = Notification.PopUpArrowDirection.Right;
    VisualController component = adventureBookPageManager.m_UnlockChapterTooltipBone.GetComponent<VisualController>();
    if ((UnityEngine.Object) component != (UnityEngine.Object) null)
    {
      string state = component.State;
      if (!(state == "SECTION_SELECT_LEFT_ARROW"))
      {
        if (state == "SECTION_SELECT_DOWN_ARROW")
          direction = Notification.PopUpArrowDirection.Down;
      }
      else
        direction = Notification.PopUpArrowDirection.Left;
    }
    if (direction == Notification.PopUpArrowDirection.Right)
      adventureBookPageManager.m_pageRightArrow.ShowHighlight();
    adventureBookPageManager.m_unlockChapterTooltip = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, adventureBookPageManager.m_UnlockChapterTooltipBone.position, adventureBookPageManager.m_UnlockChapterTooltipBone.localScale, GameStrings.Get("GLUE_ADVENTURE_ADVENTUREBOOK_DAL_UNLOCK_CHAPTER1"));
    adventureBookPageManager.m_unlockChapterTooltip.ShowPopUpArrow(direction);
    adventureBookPageManager.m_unlockChapterTooltip.PulseReminderEveryXSeconds(3f);
  }

  protected override void PageRight(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    if (this.NumPages == 0 || this.CurrentPageIndex < 0 || this.CurrentPageIndex >= this.NumPages || this.m_pageNodes[this.CurrentPageIndex] == null)
    {
      Debug.LogError((object) "AdventureBookPageManager.PageRight - No current page found! Cannot turn page without more info!");
    }
    else
    {
      PageNode pageToRight = this.m_pageNodes[this.CurrentPageIndex].PageToRight;
      if (pageToRight == null)
      {
        Debug.LogError((object) "AdventureBookPageManager.PageRight - No page to right!  You shouldn't be able to turn the page in this direction!");
      }
      else
      {
        this.m_currentPageNum = AdventureBookPageManager.PageNodeIndexToPageNum(this.m_pageNodes.IndexOf(pageToRight));
        this.TransitionPageWhenReady(BookPageManager.PageTransitionType.SINGLE_PAGE_RIGHT, true, callback, callbackData);
      }
    }
  }

  protected override void PageLeft(
    BookPageManager.DelOnPageTransitionComplete callback,
    object callbackData)
  {
    if (this.NumPages == 0 || this.CurrentPageIndex < 0 || this.CurrentPageIndex >= this.NumPages || this.m_pageNodes[this.CurrentPageIndex] == null)
    {
      Debug.LogError((object) "AdventureBookPageManager.PageLeft - No current page found! Cannot turn page without more info!");
    }
    else
    {
      PageNode pageToLeft = this.m_pageNodes[this.CurrentPageIndex].PageToLeft;
      if (pageToLeft == null)
      {
        Debug.LogError((object) "AdventureBookPageManager.PageLeft - No page to left!  You shouldn't be able to turn the page in this direction!");
      }
      else
      {
        this.m_currentPageNum = AdventureBookPageManager.PageNodeIndexToPageNum(this.m_pageNodes.IndexOf(pageToLeft));
        this.TransitionPageWhenReady(BookPageManager.PageTransitionType.SINGLE_PAGE_LEFT, true, callback, callbackData);
      }
    }
  }

  public delegate void PageClickCallback();
}
