using Assets;
using Blizzard.T5.Core;
using Blizzard.T5.MaterialService.Extensions;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class AdventureLocationSelectBook : MonoBehaviour
{
  public AdventureBookPageManager m_BookPageManager;
  public AsyncReference m_AdventureBookCoverReference;
  public Material m_anomalyModeCardHighlightMaterial;
  public float m_anomalyModeCardHideAnimTime = 0.25f;
  public float m_anomalyModeCardDriftScale = 2f;
  public float m_anomalyModeTooltipScale = 6f;
  private PlayButton m_playButton;
  private VisualController m_playButtonController;
  private Widget m_bookCover;
  private Widget m_anomalyModeButton;
  private Widget m_deckTrayWidget;
  private List<WingDbfRecord> m_wingRecords = new List<WingDbfRecord>();
  private Actor m_anomalyModeCardActor;
  private Transform m_anomalyModeCardSourceBone;
  private Transform m_anomalyModeCardBone;
  private bool m_anomalyModeCardShown;
  private bool m_justSawDungeonCrawlSubScene;
  private const string BOOK_COVER_OPEN_EVENT = "PlayBookCoverOpen";
  private const string ANOMALY_BUTTON_UNLOCKED_STATE = "UNLOCKED_ANOMALY";
  private const string ANOMALY_BUTTON_ACTIVATED_STATE = "ACTIVATED_ANOMALY";
  private const string ANOMALY_BUTTON_LOCKED_STATE = "LOCKED_ANOMALY";
  private const string PLAY_BUTTON_BURST_FX = "BURST";
  private const string ENABLE_INTERACTION_EVENT = "EnableInteraction";
  private const string DISABLE_INTERACTION_EVENT = "DisableInteraction";
  private const string SHOW_BOOK_COVER_EVENT = "ShowBookCover";
  private const string SHOW_ANOMALY_MODE_BIG_CARD_EVENT_NAME = "ShowAnomalyModeBigCard";
  private const string HIDE_ANOMALY_MODE_BIG_CARD_EVENT_NAME = "HideAnomalyModeBigCard";
  private static AdventureLocationSelectBook m_instance;

  private void Awake() => AdventureLocationSelectBook.m_instance = this;

  private void Start()
  {
    this.GetComponent<AdventureSubScene>().AddSubSceneTransitionFinishedListener(new AdventureSubScene.SubSceneTransitionFinished(this.OnSubSceneTransitionFinished));
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    WidgetTemplate widget = this.GetComponent<WidgetTemplate>();
    widget.RegisterReadyListener((Action<object>) (_ => this.OnTopLevelWidgetReady((Widget) widget)), (object) null, true);
    this.m_AdventureBookCoverReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnBookCoverReady));
    this.m_BookPageManager.PageTurnStart += new BookPageManager.PageTurnStartCallback(this.OnPageTurnStart);
    this.m_BookPageManager.PageTurnComplete += new BookPageManager.PageTurnCompleteCallback(this.OnPageTurnComplete);
    this.m_BookPageManager.PageClicked += new AdventureBookPageManager.PageClickCallback(this.OnPageClicked);
    this.m_BookPageManager.SetEnableInteractionCallback(new AdventureBookPageDisplay.EnableInteractionCallback(this.EnableInteraction));
    AdventureConfig.Get().AddAdventureMissionSetListener(new AdventureConfig.AdventureMissionSet(this.OnMissionSet));
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
    this.m_justSawDungeonCrawlSubScene = AdventureConfig.Get().PreviousSubScene == AdventureData.Adventuresubscene.DUNGEON_CRAWL;
    if (this.ShouldShowBookCoverOpeningAnim())
      widget.TriggerEvent("ShowBookCover", new Widget.TriggerEventParameters());
    Navigation.PushUnique(new Navigation.NavigateBackHandler(AdventureLocationSelectBook.OnNavigateBack));
    this.StartCoroutine(this.InitChapterDataWhenReady());
  }

  private void OnDestroy()
  {
    GameMgr.Get()?.UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.m_BookPageManager.PageTurnStart -= new BookPageManager.PageTurnStartCallback(this.OnPageTurnStart);
    this.m_BookPageManager.PageTurnComplete -= new BookPageManager.PageTurnCompleteCallback(this.OnPageTurnComplete);
    this.m_BookPageManager.PageClicked -= new AdventureBookPageManager.PageClickCallback(this.OnPageClicked);
    AdventureConfig.Get()?.RemoveAdventureMissionSetListener(new AdventureConfig.AdventureMissionSet(this.OnMissionSet));
    StoreManager.Get()?.RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
    AdventureLocationSelectBook.m_instance = (AdventureLocationSelectBook) null;
  }

  private void OnTopLevelWidgetReady(Widget topLevelWidget) => this.StartCoroutine(this.SetUpAdventureBookTrayOnceWidgetIsReady(topLevelWidget));

  private IEnumerator SetUpAdventureBookTrayOnceWidgetIsReady(Widget topLevelWidget)
  {
    while (topLevelWidget.IsChangingStates)
      yield return (object) null;
    AdventureBookDeckTray componentInChildren = topLevelWidget.GetComponentInChildren<AdventureBookDeckTray>(false);
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "No AdventureBookDeckTray exists, or they're all hidden!");
    else
      this.SetUpAdventureBookTray(componentInChildren);
  }

  private void OnSubSceneTransitionFinished() => this.StartCoroutine(this.StartAnimsWhenAllTransitionsComplete());

  private IEnumerator StartAnimsWhenAllTransitionsComplete()
  {
    while (GameUtils.IsAnyTransitionActive() || PopupDisplayManager.Get().IsShowing)
      yield return (object) null;
    this.m_BookPageManager.OnBookOpening();
    if (this.ShouldShowBookCoverOpeningAnim() && (UnityEngine.Object) this.m_bookCover != (UnityEngine.Object) null)
    {
      this.m_bookCover.TriggerEvent("PlayBookCoverOpen");
      Log.Adventures.Print("Waiting for Book Cover Opening animation to complete...");
    }
    else
      this.AllInitialTransitionsComplete();
  }

  private bool ShouldShowBookCoverOpeningAnim() => AdventureConfig.Get().PreviousSubScene == AdventureData.Adventuresubscene.CHOOSER && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY;

  private void OnCoverOpened(UnityEngine.Object callbackData)
  {
    if ((UnityEngine.Object) this.m_BookPageManager == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("OnCoverOpen: m_BookPageManager was null!");
    }
    else
    {
      Log.Adventures.Print("Book Cover Opening animation now complete!");
      PageData dataForCurrentPage = this.m_BookPageManager.GetPageDataForCurrentPage();
      if (dataForCurrentPage != null && dataForCurrentPage.PageType == AdventureBookPageType.MAP)
      {
        DungeonCrawlSubDef_VOLines.VOEventType voEvent = AdventureConfig.Get().GetSelectedMode() == AdventureModeDbId.DUNGEON_CRAWL_HEROIC ? DungeonCrawlSubDef_VOLines.VOEventType.BOOK_REVEAL_HEROIC : DungeonCrawlSubDef_VOLines.VOEventType.BOOK_REVEAL;
        DungeonCrawlSubDef_VOLines.PlayVOLine(AdventureConfig.Get().GetSelectedAdventure(), WingDbId.INVALID, 0, voEvent);
      }
      this.AllInitialTransitionsComplete();
    }
  }

  private void AllInitialTransitionsComplete()
  {
    this.EnableInteraction(true);
    this.m_BookPageManager.AllInitialTransitionsComplete();
  }

  private IEnumerator InitChapterDataWhenReady()
  {
    AdventureLocationSelectBook locationSelectBook = this;
    while (!locationSelectBook.m_BookPageManager.IsFullyLoaded())
      yield return (object) null;
    while ((UnityEngine.Object) locationSelectBook.m_playButton == (UnityEngine.Object) null)
      yield return (object) null;
    AdventureConfig adventureConfig = AdventureConfig.Get();
    AdventureDbId selectedAdv = adventureConfig.GetSelectedAdventure();
    AdventureModeDbId selectedMode = adventureConfig.GetSelectedMode();
    List<ScenarioDbfRecord> records = GameDbf.Scenario.GetRecords((Predicate<ScenarioDbfRecord>) (r => (AdventureDbId) r.AdventureId == selectedAdv && (AdventureModeDbId) r.ModeId == selectedMode && r.WingId != 0));
    int numChapters = 0;
    Map<int, List<ChapterPageData>> map = new Map<int, List<ChapterPageData>>();
    foreach (ScenarioDbfRecord scenarioDbfRecord in records)
    {
      ScenarioDbfRecord scenarioRecord = scenarioDbfRecord;
      ChapterPageData chapterPageData1 = (ChapterPageData) null;
      foreach (List<ChapterPageData> chapterPageDataList in map.Values)
      {
        chapterPageData1 = chapterPageDataList.Find((Predicate<ChapterPageData>) (x => x.WingRecord.ID == scenarioRecord.WingId));
        if (chapterPageData1 != null)
          break;
      }
      if (chapterPageData1 == null)
      {
        WingDbfRecord record = GameDbf.Wing.GetRecord(scenarioRecord.WingId);
        if (record == null)
        {
          Log.Adventures.PrintError("No Wing record found for ID {0}, referenced by Scenario {1}", (object) scenarioRecord.WingId, (object) scenarioRecord.ID);
          continue;
        }
        ChapterPageData chapterPageData2 = new ChapterPageData();
        chapterPageData2.Adventure = selectedAdv;
        chapterPageData2.AdventureMode = selectedMode;
        chapterPageData2.WingRecord = record;
        chapterPageData2.BookSection = record.BookSection;
        chapterPageData1 = chapterPageData2;
        if (!map.ContainsKey(record.BookSection))
          map.Add(record.BookSection, new List<ChapterPageData>());
        map[record.BookSection].Add(chapterPageData1);
        locationSelectBook.m_wingRecords.Add(record);
        ++numChapters;
      }
      chapterPageData1.ScenarioRecords.Add(scenarioRecord);
    }
    int count = map.Count;
    List<List<ChapterPageData>> chapterPageDataListList = new List<List<ChapterPageData>>();
    foreach (int key in map.Keys)
    {
      chapterPageDataListList.Add(map[key]);
      foreach (ChapterPageData chapterPageData in map[key])
        chapterPageData.ScenarioRecords.Sort(new Comparison<ScenarioDbfRecord>(GameUtils.MissionSortComparison));
    }
    chapterPageDataListList.Sort((Comparison<List<ChapterPageData>>) ((a, b) =>
    {
      if (a.Count >= 1 && b.Count >= 1)
        return a[0].WingRecord.BookSection - b[0].WingRecord.BookSection;
      Debug.LogError((object) "AdventureLocationSelectBook: chapterDataBySection has a section with 0 chapters in it!");
      return 0;
    }));
    foreach (List<ChapterPageData> chapterPageDataList in chapterPageDataListList)
      chapterPageDataList.Sort((Comparison<ChapterPageData>) ((a, b) => a.WingRecord.SortOrder - b.WingRecord.SortOrder));
    List<PageNode> pageNodes = new List<PageNode>();
    bool flag1 = true;
    bool flag2 = true;
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) selectedAdv, (int) selectedMode);
    if (adventureDataRecord != null)
    {
      if (adventureDataRecord.AdventureBookMapPageLocation == AdventureData.Adventurebooklocation.END)
        Debug.LogErrorFormat("Adventure {0} and Mode {1} has the Map Page Location at END, but that is not yet supported by the code!", (object) selectedAdv, (object) selectedMode);
      flag1 = adventureDataRecord.AdventureBookMapPageLocation != AdventureData.Adventurebooklocation.NOWHERE;
      if (adventureDataRecord.AdventureBookRewardPageLocation == AdventureData.Adventurebooklocation.BEGINNING)
        Debug.LogErrorFormat("Adventure {0} and Mode {1} has the Reward Page Location at BEGINNING, but that is not yet supported by the code!", (object) selectedAdv, (object) selectedMode);
      flag2 = adventureDataRecord.AdventureBookRewardPageLocation != AdventureData.Adventurebooklocation.NOWHERE;
    }
    Map<int, ChapterPageData> chapterNumberToChapterDataMap = new Map<int, ChapterPageData>();
    PageNode pageNode1 = (PageNode) null;
    if (flag1)
    {
      MapPageData data = new MapPageData();
      data.Adventure = selectedAdv;
      data.AdventureMode = selectedMode;
      data.NumSectionsInBook = count;
      data.BookSection = -1;
      data.ChapterData = chapterNumberToChapterDataMap;
      pageNode1 = new PageNode((PageData) data);
      pageNodes.Add(pageNode1);
    }
    List<List<PageNode>> pageNodeListList = new List<List<PageNode>>();
    int num = 1;
    foreach (List<ChapterPageData> chapterPageDataList in chapterPageDataListList)
    {
      List<PageNode> pageNodeList = new List<PageNode>();
      pageNodeListList.Add(pageNodeList);
      foreach (ChapterPageData data in chapterPageDataList)
      {
        data.ChapterNumber = num++;
        pageNodeList.Add(new PageNode((PageData) data));
        chapterNumberToChapterDataMap.Add(data.ChapterNumber, data);
      }
      pageNodes.AddRange((IEnumerable<PageNode>) pageNodeList.ToArray());
    }
    locationSelectBook.UpdateRelationalChapterData(chapterNumberToChapterDataMap);
    List<PageNode> pageNodeList1 = new List<PageNode>();
    if (flag2)
    {
      for (int index = 0; index < count; ++index)
      {
        RewardPageData data = new RewardPageData();
        data.Adventure = selectedAdv;
        data.AdventureMode = selectedMode;
        data.BookSection = index;
        data.ChapterData = chapterNumberToChapterDataMap;
        PageNode pageNode2 = new PageNode((PageData) data);
        pageNodeList1.Add(pageNode2);
        pageNodes.Add(pageNodeList1[index]);
      }
    }
    if (count == 1 && pageNodes.Count > 1 && pageNode1 != null && pageNodes[0] == pageNode1)
      pageNode1.PageToRight = pageNodeListList[0][0];
    for (int index1 = 0; index1 < pageNodeListList.Count; ++index1)
    {
      List<PageNode> pageNodeList2 = pageNodeListList[index1];
      for (int index2 = 0; index2 < pageNodeList2.Count; ++index2)
      {
        PageNode pageNode3 = pageNodeList2[index2];
        pageNode3.PageToLeft = index2 != 0 ? pageNodeList2[index2 - 1] : pageNode1;
        if (index2 == pageNodeList2.Count - 1)
        {
          if (index1 < pageNodeList1.Count)
          {
            pageNode3.PageToRight = pageNodeList1[index1];
            pageNodeList1[index1].PageToLeft = pageNode3;
          }
          else
            pageNode3.PageToRight = (PageNode) null;
        }
        else if (index2 + 1 < pageNodeList2.Count)
          pageNode3.PageToRight = pageNodeList2[index2 + 1];
        else
          Log.Adventures.PrintWarning("No page to set for PageToRight for Chapter index {0} in section {1}!", (object) index2, (object) index1);
      }
    }
    locationSelectBook.m_BookPageManager.Initialize(pageNodes, numChapters);
    while (locationSelectBook.m_BookPageManager.ArePagesTurning())
      yield return (object) null;
    AdventureDbId currentSelectedAdventure = AdventureConfig.Get().GetAdventureDataModel().SelectedAdventure;
    while (AchieveManager.Get().HasActiveLicenseForAdventure(currentSelectedAdventure))
    {
      Log.Adventures.Print("Waiting on active license added achieves before entering the current Adventure subscene!");
      yield return (object) null;
    }
    locationSelectBook.GetComponent<AdventureSubScene>().SetIsLoaded(true);
  }

  private void UpdateRelationalChapterData(
    Map<int, ChapterPageData> chapterNumberToChapterDataMap)
  {
    foreach (ChapterPageData chapterPageData1 in chapterNumberToChapterDataMap.Values)
    {
      foreach (ScenarioDbfRecord scenarioRecord in chapterPageData1.ScenarioRecords)
      {
        int missionReqProgress = 0;
        int wingId = 0;
        if (AdventureConfig.GetMissionPlayableParameters(scenarioRecord.ID, ref wingId, ref missionReqProgress) && wingId != chapterPageData1.WingRecord.ID)
        {
          foreach (ChapterPageData chapterPageData2 in chapterNumberToChapterDataMap.Values)
          {
            if (chapterPageData2.WingRecord.ID == wingId)
            {
              if (chapterPageData2.ChapterToFlipToWhenCompleted != 0)
                Debug.LogWarningFormat("Chapter {0} already had a ChapterToFlipToWhenCompleted value of {1}, setting it to {2}!  Having scenarios from multiple wings that rely on the progress of a single wing is not currently supported!", (object) chapterPageData2.ChapterNumber, (object) chapterPageData2.ChapterToFlipToWhenCompleted, (object) chapterPageData1.ChapterNumber);
              chapterPageData2.ChapterToFlipToWhenCompleted = chapterPageData1.ChapterNumber;
              Log.Adventures.Print("ChapterToFlipToWhenCompleted for Chapter {0} set to Chapter {1}", (object) chapterPageData2.ChapterNumber, (object) chapterPageData1.ChapterNumber);
              break;
            }
          }
        }
      }
    }
  }

  private void SetUpAdventureBookTray(AdventureBookDeckTray deckTray)
  {
    if ((UnityEngine.Object) deckTray == (UnityEngine.Object) null || deckTray.m_PlayButtonReference == null || (UnityEngine.Object) deckTray.m_BackButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "DeckTray was not properly configured!");
    }
    else
    {
      this.m_deckTrayWidget = deckTray.GetComponent<Widget>();
      if ((UnityEngine.Object) this.m_deckTrayWidget != (UnityEngine.Object) null)
        this.m_deckTrayWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.DeckTrayEventListener));
      deckTray.m_PlayButtonReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnPlayButtonReady));
      deckTray.m_AnomalyModeButtonReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnAnomalyModeButtonReady));
      deckTray.m_BackButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (e => this.OnBackButtonPress()));
      this.m_anomalyModeCardSourceBone = deckTray.m_anomalyModeCardSourceBone;
      this.m_anomalyModeCardBone = deckTray.m_anomalyModeCardBone;
      this.LoadAnomalyModeCard();
    }
  }

  private void OnPlayButtonReady(VisualController buttonVisualController)
  {
    if ((UnityEngine.Object) buttonVisualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "PlayButton could not be found! You will not be able to click 'Play'!");
    }
    else
    {
      this.m_playButtonController = buttonVisualController;
      this.m_playButton = buttonVisualController.gameObject.GetComponent<PlayButton>();
      this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayButtonRelease));
      this.SetPlayButtonStateForCurrentPage(false);
    }
  }

  private void SetPlayButtonEnabled(bool enable)
  {
    if (!((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null))
      return;
    if (enable)
    {
      if (this.m_playButton.IsEnabled())
        return;
      this.m_playButton.Enable();
    }
    else
    {
      if (!this.m_playButton.IsEnabled())
        return;
      this.m_playButton.Disable();
    }
  }

  private void OnAnomalyModeButtonReady(Widget button)
  {
    this.m_anomalyModeButton = button;
    if ((UnityEngine.Object) button == (UnityEngine.Object) null)
      return;
    Clickable componentInChildren = this.m_anomalyModeButton.GetComponentInChildren<Clickable>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "Anomaly Mode Button has no Clickable!  Unable to attach listeners.");
    }
    else
    {
      componentInChildren.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.AnomalyModeButtonRelease));
      TooltipZone tooltipZone = this.m_anomalyModeButton.GetComponentInChildren<TooltipZone>();
      if ((UnityEngine.Object) tooltipZone == (UnityEngine.Object) null)
      {
        Error.AddDevWarning("UI Error!", "Anomaly Mode Button has no TooltipZone!  Unable to attach tooltip.");
      }
      else
      {
        componentInChildren.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (e =>
        {
          AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
          AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
          WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(AdventureConfig.Get().GetMission());
          if (wingIdFromMissionId == WingDbId.INVALID)
            return;
          if (!AdventureUtils.IsAnomalyModeAllowed(wingIdFromMissionId))
          {
            tooltipZone.ShowTooltip(GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_ANOMALY_MODE_BUTTON_LOCKED_TOOLTIP_HEADER"), GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_ANOMALY_MODE_UNAVAILABLE_TOOLTIP_BODY"), this.m_anomalyModeTooltipScale);
          }
          else
          {
            if (!AdventureUtils.IsAnomalyModeLocked(selectedAdventure, selectedMode))
              return;
            tooltipZone.ShowTooltip(GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_ANOMALY_MODE_BUTTON_LOCKED_TOOLTIP_HEADER"), GameStrings.Get("GLUE_ADVENTURE_DUNGEON_CRAWL_ANOMALY_MODE_BUTTON_LOCKED_TOOLTIP_BODY"), this.m_anomalyModeTooltipScale);
          }
        }));
        componentInChildren.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (e => tooltipZone.HideTooltip()));
      }
    }
  }

  private void OnBookCoverReady(Widget bookCover)
  {
    this.m_bookCover = bookCover;
    this.StartCoroutine(this.SetUpBookCoverReferencesWhenResolved(bookCover));
  }

  private IEnumerator SetUpBookCoverReferencesWhenResolved(Widget bookCover)
  {
    AdventureLocationSelectBook locationSelectBook = this;
    if ((UnityEngine.Object) bookCover == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Issue!", "m_AdventureBookCover is not hooked up on AdventureLocationSelectBook, so things won't load!");
    }
    else
    {
      while (bookCover.IsChangingStates)
        yield return (object) null;
      AnimationEventDispatcher componentInChildren = bookCover.GetComponentInChildren<AnimationEventDispatcher>();
      if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
        componentInChildren.RegisterAnimationEventListener(new OnAnimationEvent(locationSelectBook.OnCoverOpened));
    }
  }

  public static bool OnNavigateBack()
  {
    if ((UnityEngine.Object) AdventureLocationSelectBook.m_instance == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("Trying to navigate back, but AdventureLocationSelectBook has been destroyed!");
      return false;
    }
    AdventureConfig.Get().SetMission(ScenarioDbId.INVALID);
    AdventureConfig.Get().SubSceneGoBack();
    AdventureBookPageManager bookPageManager = AdventureLocationSelectBook.m_instance.m_BookPageManager;
    if ((UnityEngine.Object) bookPageManager != (UnityEngine.Object) null)
      bookPageManager.HideAllPopups();
    return true;
  }

  private void OnBackButtonPress() => Navigation.GoBack();

  private void OnPageTurnStart(BookPageManager.PageTransitionType transitionType)
  {
    this.SetPlayButtonEnabled(false);
    if (transitionType == BookPageManager.PageTransitionType.NONE)
      return;
    AdventureConfig.Get().AnomalyModeActivated = false;
    AdventureConfig.Get().SetMission(ScenarioDbId.INVALID);
    ChapterPageData dataForCurrentPage = this.m_BookPageManager.GetPageDataForCurrentPage() as ChapterPageData;
    AdventureChapterState adventureChapterState = AdventureChapterState.LOCKED;
    if (dataForCurrentPage != null && dataForCurrentPage.WingRecord != null)
      adventureChapterState = AdventureProgressMgr.Get().AdventureBookChapterStateForWing(dataForCurrentPage.WingRecord, dataForCurrentPage.AdventureMode);
    if (dataForCurrentPage == null || adventureChapterState == AdventureChapterState.LOCKED)
      return;
    AdventureConfig.Get().SetHasSeenUnlockedChapterPage((WingDbId) dataForCurrentPage.WingRecord.ID, true);
  }

  private void OnPageTurnComplete(int currentPageNum)
  {
    AdventureBookPageDataModel currentPageDataModel = this.m_BookPageManager.GetCurrentPageDataModel();
    if ((UnityEngine.Object) this.m_deckTrayWidget != (UnityEngine.Object) null && currentPageDataModel != null)
      this.m_deckTrayWidget.BindDataModel((IDataModel) currentPageDataModel);
    AdventureChapterDataModel chapterData = currentPageDataModel.ChapterData;
    ChapterPageData dataForCurrentPage = this.m_BookPageManager.GetPageDataForCurrentPage() as ChapterPageData;
    AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) AdventureConfig.Get().GetSelectedAdventure());
    if (dataForCurrentPage != null && dataForCurrentPage.PageType == AdventureBookPageType.CHAPTER && chapterData.ChapterState != AdventureChapterState.LOCKED && dataForCurrentPage.WingRecord.PmtProductIdForSingleWingPurchase == 0 && AdventureConfig.Get().ShouldSeeFirstTimeFlow && record != null && !record.MapPageHasButtonsToChapters)
    {
      if (AdventureUtils.IsEntireAdventureFree((AdventureDbId) record.ID))
      {
        if (dataForCurrentPage.ChapterNumber == 1)
          AdventureConfig.Get().MarkHasSeenFirstTimeFlowComplete();
      }
      else
        AdventureUtils.DisplayFirstChapterFreePopup(dataForCurrentPage, new AdventureUtils.FirstChapterFreePopupCompleteCallback(this.OnFirstChapterFreePopupDisplayed));
    }
    if (!this.m_justSawDungeonCrawlSubScene)
      this.PlayPageSpecificVO();
    this.m_justSawDungeonCrawlSubScene = false;
  }

  private void OnFirstChapterFreePopupDisplayed() => this.PlayPageSpecificVO();

  private void OnMissionSet(ScenarioDbId mission, bool showDetails)
  {
    this.SetPlayButtonStateForCurrentPage(true);
    if ((UnityEngine.Object) this.m_deckTrayWidget != (UnityEngine.Object) null)
    {
      ScenarioDbfRecord record = GameDbf.Scenario.GetRecord((int) mission);
      string missionHeroCardId = GameUtils.GetMissionHeroCardId((int) mission);
      IDataModel model;
      this.m_deckTrayWidget.GetDataModel(111, out model);
      if (!(model is HeroDataModel heroDataModel))
      {
        heroDataModel = new HeroDataModel();
        this.m_deckTrayWidget.BindDataModel((IDataModel) heroDataModel);
      }
      if (heroDataModel.HeroCard == null)
        heroDataModel.HeroCard = new CardDataModel();
      heroDataModel.HeroCard.CardId = missionHeroCardId;
      string missionHeroPowerCardId = GameUtils.GetMissionHeroPowerCardId((int) mission);
      if (heroDataModel.HeroPowerCard == null)
        heroDataModel.HeroPowerCard = new CardDataModel();
      heroDataModel.HeroPowerCard.CardId = missionHeroPowerCardId;
      if (record == null)
      {
        heroDataModel.Name = (string) null;
        heroDataModel.Description = (string) null;
      }
      else
      {
        heroDataModel.Name = (string) record.ShortName;
        heroDataModel.Description = (string) (!(bool) UniversalInputManager.UsePhoneUI || string.IsNullOrEmpty((string) record.ShortDescription) ? record.Description : record.ShortDescription);
      }
    }
    if (mission == ScenarioDbId.INVALID || !AdventureProgressMgr.Get().CanPlayScenario((int) mission) || SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY && (ScenarioDbId) GameMgr.Get().GetPreviousMissionId() == mission)
      return;
    AdventureBossDef bossDef = AdventureConfig.Get().GetBossDef(mission);
    if (!((UnityEngine.Object) bossDef != (UnityEngine.Object) null) || bossDef.m_IntroLinePlayTime != AdventureBossDef.IntroLinePlayTime.MissionSelect)
      return;
    AdventureUtils.PlayMissionQuote(bossDef, NotificationManager.DEFAULT_CHARACTER_POS);
  }

  private void OnSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod purchaseMethod)
  {
    if (!AdventureUtils.DoesBundleIncludeWingForAdventure(bundle, AdventureConfig.Get().GetSelectedAdventure()))
      return;
    this.PlayPageSpecificVO();
  }

  private void BurstPlayButton()
  {
    if ((UnityEngine.Object) this.m_playButton == (UnityEngine.Object) null || !this.m_playButton.IsEnabled())
      return;
    if ((UnityEngine.Object) this.m_playButtonController == (UnityEngine.Object) null)
      Log.Adventures.PrintError("Attempting to burst Play Button, but m_playButtonController is null!");
    else
      this.m_playButtonController.Owner.TriggerEvent("BURST", new Widget.TriggerEventParameters());
  }

  private void OnPageClicked() => this.BurstPlayButton();

  private void SetPlayButtonStateForCurrentPage(bool showBurst)
  {
    if (this.PlayButtonShouldBeEnabled())
    {
      this.SetPlayButtonEnabled(true);
      if (!showBurst)
        return;
      this.BurstPlayButton();
    }
    else
      this.SetPlayButtonEnabled(false);
  }

  private void PlayButtonRelease(UIEvent e)
  {
    this.SetPlayButtonEnabled(false);
    if (!this.PlayButtonShouldBeEnabled())
    {
      Log.Adventures.PrintError("Play Button should be disabled, but you clicked it anyway!");
    }
    else
    {
      ScenarioDbId mission = AdventureConfig.Get().GetMission();
      AdventureBossDef bossDef = AdventureConfig.Get().GetBossDef(mission);
      if ((UnityEngine.Object) bossDef != (UnityEngine.Object) null && bossDef.m_IntroLinePlayTime == AdventureBossDef.IntroLinePlayTime.MissionStart)
        AdventureUtils.PlayMissionQuote(bossDef, NotificationManager.DEFAULT_CHARACTER_POS);
      if (AdventureConfig.DoesMissionRequireDeck(AdventureConfig.Get().GetMission()))
      {
        AdventureData.Adventuresubscene subscene = !GameUtils.DoesAdventureModeUseDungeonCrawlFormat(AdventureConfig.Get().GetSelectedMode()) || AdventureConfig.Get().IsHeroSelectedBeforeDungeonCrawlScreenForSelectedAdventure() ? AdventureConfig.Get().SubSceneForPickingHeroForCurrentAdventure() : AdventureData.Adventuresubscene.DUNGEON_CRAWL;
        AdventureConfig.Get().ChangeSubScene(subscene);
      }
      else
        GameMgr.Get().FindGame(GameType.GT_VS_AI, PegasusShared.FormatType.FT_WILD, (int) AdventureConfig.Get().GetMissionToPlay());
    }
  }

  private bool PlayButtonShouldBeEnabled()
  {
    if (AdventureConfig.Get().ShouldSeeFirstTimeFlow)
      return false;
    ScenarioDbId mission = AdventureConfig.Get().GetMission();
    return mission != ScenarioDbId.INVALID && AdventureProgressMgr.Get().CanPlayScenario((int) mission);
  }

  private void AnomalyModeButtonRelease(UIEvent e)
  {
    if (!this.IsAnomalyModeAvailable())
      return;
    AdventureConfig.Get().AnomalyModeActivated = !AdventureConfig.Get().AnomalyModeActivated;
  }

  private bool IsAnomalyModeAvailable()
  {
    int selectedAdventure = (int) AdventureConfig.Get().GetSelectedAdventure();
    AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
    WingDbId wingIdFromMissionId = GameUtils.GetWingIdFromMissionId(AdventureConfig.Get().GetMission());
    int modeDbId = (int) selectedMode;
    int num = (int) wingIdFromMissionId;
    return AdventureUtils.IsAnomalyModeAvailable((AdventureDbId) selectedAdventure, (AdventureModeDbId) modeDbId, (WingDbId) num);
  }

  private void LoadAnomalyModeCard()
  {
    AdventureConfig adventureConfig = AdventureConfig.Get();
    AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) adventureConfig.GetSelectedAdventure(), (int) adventureConfig.GetSelectedMode());
    long dbId = 0;
    if (adventureDataRecord.GameSaveDataServerKey > 0)
      GameSaveDataManager.Get().GetSubkeyValue((GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_ANOMALY_MODE_CARD_PREVIEW, out dbId);
    if (dbId <= 0L)
    {
      if (adventureDataRecord.AnomalyModeDefaultCardId == 0)
        return;
      dbId = (long) adventureDataRecord.AnomalyModeDefaultCardId;
    }
    string cardId = GameUtils.TranslateDbIdToCardId((int) dbId);
    DefLoader.Get().LoadFullDef(cardId, new DefLoader.LoadDefCallback<DefLoader.DisposableFullDef>(this.OnAnomalyModeFullDefLoaded));
  }

  private void OnAnomalyModeFullDefLoaded(
    string cardId,
    DefLoader.DisposableFullDef fullDef,
    object userData)
  {
    if (fullDef == null)
      Debug.LogWarningFormat("OnAnomalyModeFullDefLoaded: No FullDef found for cardId {0}!", (object) cardId);
    else
      AssetLoader.Get().InstantiatePrefab((AssetReference) ActorNames.GetHandActor(fullDef.EntityDef, TAG_PREMIUM.NORMAL), new PrefabCallback<GameObject>(this.OnAnomalyModeActorLoaded), (object) fullDef, AssetLoadingOptions.IgnorePrefabPosition);
  }

  private void OnAnomalyModeActorLoaded(
    AssetReference assetRef,
    GameObject go,
    object callbackData)
  {
    DefLoader.DisposableFullDef fullDef = callbackData as DefLoader.DisposableFullDef;
    using (fullDef)
    {
      Actor component = go.GetComponent<Actor>();
      this.m_anomalyModeCardActor = component;
      if (fullDef == null)
        Debug.LogWarning((object) "OnAnomalyModeActorLoaded: no FullDef passed in!");
      else if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Debug.LogWarningFormat("OnAnomalyModeActorLoaded: actor \"{0}\" has no Actor component", (object) assetRef);
      }
      else
      {
        GameUtils.SetParent((Component) component, this.gameObject);
        LayerUtils.SetLayer((Component) component, this.gameObject.layer);
        component.TurnOffCollider();
        component.SetFullDef(fullDef);
        component.UpdateAllComponents();
        component.SetUnlit();
        component.Hide();
      }
    }
  }

  private void DeckTrayEventListener(string eventName)
  {
    if (eventName.Equals("ShowAnomalyModeBigCard") && this.IsAnomalyModeAvailable())
    {
      this.ShowAnomalyModeBigCard();
    }
    else
    {
      if (!eventName.Equals("HideAnomalyModeBigCard"))
        return;
      this.HideAnomalyModeBigCard();
    }
  }

  private void ShowAnomalyModeBigCard()
  {
    if ((UnityEngine.Object) this.m_anomalyModeCardActor == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "ShowAnomalyModeBigCard: m_anomalyModeCardActor is not loaded!");
    }
    else
    {
      if (this.m_anomalyModeCardShown)
        return;
      this.m_anomalyModeCardShown = true;
      iTween.Stop(this.m_anomalyModeCardActor.gameObject);
      this.m_anomalyModeCardActor.Show();
      HighlightRender componentInChildren = this.m_anomalyModeCardActor.GetComponentInChildren<HighlightRender>();
      MeshRenderer meshRenderer = (UnityEngine.Object) componentInChildren != (UnityEngine.Object) null ? componentInChildren.GetComponent<MeshRenderer>() : (MeshRenderer) null;
      if ((UnityEngine.Object) meshRenderer != (UnityEngine.Object) null && (UnityEngine.Object) this.m_anomalyModeCardHighlightMaterial != (UnityEngine.Object) null)
      {
        RendererExtension.SetSharedMaterial((Renderer) meshRenderer, this.m_anomalyModeCardHighlightMaterial);
        meshRenderer.enabled = true;
      }
      this.m_anomalyModeCardActor.gameObject.transform.position = this.m_anomalyModeCardBone.position;
      this.m_anomalyModeCardActor.gameObject.transform.localScale = this.m_anomalyModeCardBone.localScale;
      AnimationUtil.GrowThenDrift(this.m_anomalyModeCardActor.gameObject, this.m_anomalyModeCardSourceBone.position, this.m_anomalyModeCardDriftScale);
    }
  }

  private void HideAnomalyModeBigCard()
  {
    if ((UnityEngine.Object) this.m_anomalyModeCardActor == (UnityEngine.Object) null)
    {
      Debug.LogWarning((object) "ShowAnomalyModeBigCard: m_anomalyModeCardActor is not loaded!");
    }
    else
    {
      if (!this.m_anomalyModeCardShown)
        return;
      this.m_anomalyModeCardShown = false;
      iTween.Stop(this.m_anomalyModeCardActor.gameObject);
      iTween.MoveTo(this.m_anomalyModeCardActor.gameObject, iTween.Hash((object) "position", (object) this.m_anomalyModeCardSourceBone.position, (object) "time", (object) this.m_anomalyModeCardHideAnimTime, (object) "easeType", (object) iTween.EaseType.easeOutQuart));
      iTween.ScaleTo(this.m_anomalyModeCardActor.gameObject, iTween.Hash((object) "scale", (object) (Vector3.one * 0.05f), (object) "time", (object) this.m_anomalyModeCardHideAnimTime, (object) "oncomplete", (object) "AnomalyModeCardShrinkComplete", (object) "oncompletetarget", (object) this.gameObject));
    }
  }

  private void AnomalyModeCardShrinkComplete() => this.m_anomalyModeCardActor.Hide();

  private void EnableInteraction(bool enable)
  {
    Widget component = this.GetComponent<Widget>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Issue!", "The component AdventureLocationSelectBook is not attached to a Widget!");
    else
      component.TriggerEvent(enable ? nameof (EnableInteraction) : "DisableInteraction");
  }

  private void PlayPageSpecificVO()
  {
    PageData dataForCurrentPage = this.m_BookPageManager.GetPageDataForCurrentPage();
    if (dataForCurrentPage == null)
    {
      Log.Adventures.PrintWarning("AdventureBookPageManager.PlayPageSpecificVO was called but no page data exists.");
    }
    else
    {
      AdventureDbId selectedAdventure = AdventureConfig.Get().GetSelectedAdventure();
      AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
      DungeonCrawlSubDef_VOLines.VOEventType voEvent = DungeonCrawlSubDef_VOLines.VOEventType.INVALID;
      WingDbfRecord wingRecord = dataForCurrentPage is ChapterPageData chapterPageData ? chapterPageData.WingRecord : (WingDbfRecord) null;
      AdventureChapterState adventureChapterState = wingRecord != null ? AdventureProgressMgr.Get().AdventureBookChapterStateForWing(wingRecord, selectedMode) : AdventureChapterState.LOCKED;
      WingDbId wingDbId = wingRecord != null ? (WingDbId) wingRecord.ID : WingDbId.INVALID;
      this.StopCoroutine("PlayChapterPageQuoteAfterDelay");
      switch (dataForCurrentPage.PageType)
      {
        case AdventureBookPageType.CHAPTER:
          int numChaptersOwned = this.m_BookPageManager.GetNumChaptersOwned();
          if (adventureChapterState == AdventureChapterState.LOCKED || AdventureConfig.Get().ShouldSeeFirstTimeFlow)
            voEvent = DungeonCrawlSubDef_VOLines.GetNextValidEventType(selectedAdventure, wingDbId, 0, new DungeonCrawlSubDef_VOLines.VOEventType[1]
            {
              DungeonCrawlSubDef_VOLines.VOEventType.WING_UNLOCK
            });
          if (voEvent == DungeonCrawlSubDef_VOLines.VOEventType.INVALID && numChaptersOwned == this.m_BookPageManager.NumChapters)
            voEvent = DungeonCrawlSubDef_VOLines.GetNextValidEventType(selectedAdventure, wingDbId, 0, new DungeonCrawlSubDef_VOLines.VOEventType[1]
            {
              DungeonCrawlSubDef_VOLines.VOEventType.ANOMALY_UNLOCK
            });
          if (voEvent == DungeonCrawlSubDef_VOLines.VOEventType.INVALID && !AdventureConfig.Get().ShouldSeeFirstTimeFlow && adventureChapterState == AdventureChapterState.LOCKED)
            voEvent = DungeonCrawlSubDef_VOLines.VOEventType.CALL_TO_ACTION;
          if (voEvent == DungeonCrawlSubDef_VOLines.VOEventType.INVALID && !AdventureConfig.Get().ShouldSeeFirstTimeFlow && adventureChapterState != AdventureChapterState.LOCKED)
          {
            AdventureMission.WingProgress progress = AdventureProgressMgr.Get().GetProgress((int) wingDbId);
            if (progress != null && progress.IsOwned())
            {
              this.StartCoroutine("PlayChapterPageQuoteAfterDelay", (object) AdventureScene.Get().GetWingDef(wingDbId));
              return;
            }
            break;
          }
          break;
        case AdventureBookPageType.REWARD:
          voEvent = DungeonCrawlSubDef_VOLines.GetNextValidEventType(selectedAdventure, WingDbId.INVALID, 0, new DungeonCrawlSubDef_VOLines.VOEventType[1]
          {
            DungeonCrawlSubDef_VOLines.VOEventType.REWARD_PAGE_REVEAL
          });
          break;
      }
      if (voEvent == DungeonCrawlSubDef_VOLines.VOEventType.INVALID)
        return;
      DungeonCrawlSubDef_VOLines.PlayVOLine(AdventureConfig.Get().GetSelectedAdventure(), wingDbId, 0, voEvent);
    }
  }

  private IEnumerator PlayChapterPageQuoteAfterDelay(AdventureWingDef wingDef)
  {
    if ((UnityEngine.Object) wingDef == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "AdventureLocationSelectBook.PlayChapterPageQuoteAfterDelay() called with no AdventureWingDef passed in!");
    }
    else
    {
      yield return (object) new WaitForSeconds(wingDef.m_OpenQuoteDelay);
      if (NotificationManager.Get().IsQuotePlaying)
      {
        if (AdventureUtils.CanPlayWingOpenQuote(wingDef))
          NotificationManager.Get().ForceAddSoundToPlayedList(wingDef.m_OpenQuoteVOLine);
      }
      else
      {
        WingDbId wingId = wingDef.GetWingId();
        ScenarioDbId mission = AdventureConfig.Get().GetMission();
        if (wingId != WingDbId.INVALID)
        {
          if (DungeonCrawlSubDef_VOLines.PlayVOLine(AdventureConfig.Get().GetSelectedAdventure(), wingId, 0, DungeonCrawlSubDef_VOLines.VOEventType.CHAPTER_PAGE))
          {
            while (NotificationManager.Get().IsQuotePlaying)
              yield return (object) null;
          }
          else if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY && mission != ScenarioDbId.INVALID && !AdventureProgressMgr.Get().HasDefeatedScenario((int) mission))
          {
            DungeonCrawlSubDef_VOLines.VOEventType voEvent = (this.m_BookPageManager.GetPageDataForCurrentPage() as ChapterPageData).BookSection == 0 ? DungeonCrawlSubDef_VOLines.VOEventType.BOSS_LOSS_1 : DungeonCrawlSubDef_VOLines.VOEventType.BOSS_LOSS_1_SECOND_BOOK_SECTION;
            if (DungeonCrawlSubDef_VOLines.PlayVOLine(AdventureConfig.Get().GetSelectedAdventure(), wingId, 0, voEvent))
              yield break;
          }
        }
        if (AdventureUtils.CanPlayWingOpenQuote(wingDef))
        {
          string legacyAssetName = new AssetReference(wingDef.m_OpenQuoteVOLine).GetLegacyAssetName();
          NotificationManager.Get().CreateCharacterQuote(wingDef.m_OpenQuotePrefab, GameStrings.Get(legacyAssetName), wingDef.m_OpenQuoteVOLine, false);
        }
      }
    }
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
    }
    return false;
  }

  private void HandleGameStartupFailure() => this.SetPlayButtonEnabled(this.PlayButtonShouldBeEnabled());
}
