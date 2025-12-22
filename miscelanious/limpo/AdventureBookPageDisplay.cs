using Blizzard.T5.Core;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureBookPageDisplay : BookPageDisplay
{
  public AsyncReference m_AdventureBookPageContentsReference;
  public float m_popupEffectFadeTime = 0.25f;
  private const string CHAPTER_UNLOCK_ANIMATION_COMPLETE = "CODE_UNLOCKED_ANIMATION_COMPLETE";
  private const string CHAPTER_BUTTON_POPUP_DISMISS_EVENT_NAME = "CODE_HIDE_AND_DISMISS";
  private const string BOOK_MAP_EVENT_NAME = "ShowBookMapPage";
  private const string BOOK_CARD_BACK_EVENT_NAME = "ShowCardBackPage";
  private const string BOOK_CHAPTER_EVENT_NAME = "ShowChapterPage";
  private const string REWARD_CHEST_OPEN_ANIMATION_EVENT_NAME = "OPEN_CHAPTER_CHEST_REWARD";
  private const string REWARD_CHEST_READY_TO_SHOW_POPUP_EVENT_NAME = "READY_TO_SHOW_POPUP";
  private const string CHAPTER_NEWLY_COMPLETED_ANIMATION_EVENT_NAME = "CHAPTER_NEWLY_COMPLETED";
  private const string CHAPTER_NEWLY_COMPLETED_ANIM_FINISHED_EVENT_NAME = "CHAPTER_NEWLY_COMPLETED_ANIM_FINISHED";
  private const string ADVENTURE_NEWLY_COMPLETED_SEQUENCE_EVENT_NAME = "AdventureNewlyCompletedSequence";
  private const string PLAY_ADVENTURE_NEWLY_COMPLETED_VO_EVENT_NAME = "PlayAdventureNewlyCompletedVO";
  private const string ADVENTURE_NEWLY_COMPLETED_SEQUENCE_FINISHED_EVENT_NAME = "AdventureNewlyCompletedSequenceFinished";
  private const string MISSION_NEWLY_COMPLETED_ANIM_EVENT_NAME = "MISSION_NEWLY_COMPLETED";
  private const string MISSION_NEWLY_COMPLETED_ANIM_FINISHED_EVENT_NAME = "MISSION_NEWLY_COMPLETED_ANIM_FINISHED";
  private const string MISSION_NEWLY_UNLOCKED_ANIM_EVENT_NAME = "MISSION_NEWLY_UNLOCKED";
  private const string MISSION_NEWLY_UNLOCKED_ANIM_FINISHED_EVENT_NAME = "MISSION_NEWLY_UNLOCKED_ANIM_FINISHED";
  private const string CHAPTER_NEWLY_PURCHASED_SEQUENCE_EVENT_NAME = "PLAY_CHAPTER_NEWLY_PURCHASED_ANIM";
  private const string CHAPTER_NEWLY_PURCHASED_ANIM_FINISHED_EVENT_NAME = "CHAPTER_NEWLY_PURCHASED_ANIM_FINISHED";
  private const string PURCHASE_INDIVIDUAL_WING_EVENT_NAME = "chapter_selected";
  private const string PURCHASE_BOOK_EVENT_NAME = "book_selected";
  private const string CHAPTER_UNLOCK_BUTTON_CLICKED_EVENT_NAME = "CHAPTER_UNLOCK_BUTTON_CLICKED";
  private const string BOSS_1_SELECTED_EVENT_NAME = "BOSS_1_SELECTED";
  private const string BOSS_2_SELECTED_EVENT_NAME = "BOSS_2_SELECTED";
  private const string BOSS_3_SELECTED_EVENT_NAME = "BOSS_3_SELECTED";
  private const string BOSS_4_SELECTED_EVENT_NAME = "BOSS_4_SELECTED";
  private const string BOSS_5_SELECTED_EVENT_NAME = "BOSS_5_SELECTED";
  private Widget m_adventureBookPageContentsWidget;
  private Widget.EventListenerDelegate m_pageEventListener;
  private AdventureBookPageDisplay.FlipToChapterCallback m_flipToChapterCallback;
  private PageData m_pageData;
  private AdventureBookPageDataModel m_pageDataModel;
  private bool m_allInitialTransitionsComplete;
  private AdventureBookPageDisplay.EnableInteractionCallback m_enableInteractionCallback;
  private Map<string, Clickable> m_chapterButtonClickablesNameMap = new Map<string, Clickable>();
  private Queue<string> m_chapterNewlyUnlockedMapSequenceQueue = new Queue<string>();
  private bool m_isInUnlockedSequence;
  private bool m_readyToPlayAdventureNewlyCompletedVO;
  private bool m_adventureNewlyCompletedSequenceFinished;
  private string m_currentUnlockButtonName;
  private List<AdventureChapterDataModel> m_sortedChapterDataModels = new List<AdventureChapterDataModel>();
  private bool m_needToShowRewardChestAnim;
  private bool m_rewardChestReadyToShowPopup;
  private bool m_needToShowChapterCompletionAnim;
  private bool m_chapterCompletionAnimFinished;
  private bool m_needToShowMissionCompleteAnim;
  private bool m_missionCompleteAnimFinished;
  private bool m_needToShowMissionUnlockAnim;
  private bool m_missionUnlockAnimFinished;
  private bool m_chapterNewlyPurchasedAnimFinished;
  private static AssetReference m_chooseStoreWidgetPrefab = new AssetReference("AdventureStorymodeChooseStore.prefab:22dcec0cce5b1ec4ba4ea2e5048934fb");
  private Widget m_storeChooseWidget;
  private UIBButton m_storeChooseBackButton;
  private UIBPopup m_storeChoosePopup;
  private ScreenEffectsHandle m_screenEffectsHandle;

  public static bool NeedToShowAdventureSectionCompletionSequence { get; private set; }

  private void Start()
  {
    this.m_AdventureBookPageContentsReference.RegisterReadyListener<Widget>(new Action<Widget>(this.AdventureBookPageContentsIsReady));
    AdventureProgressMgr.Get().RegisterProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.UpdateMapOnWingProgressUpdated));
    AdventureConfig.Get().AddAdventureMissionSetListener(new AdventureConfig.AdventureMissionSet(this.OnMissionSet));
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void OnDestroy()
  {
    AdventureProgressMgr.Get()?.RemoveProgressUpdatedListener(new AdventureProgressMgr.AdventureProgressUpdatedCallback(this.UpdateMapOnWingProgressUpdated));
    AdventureConfig.Get().RemoveAdventureMissionSetListener(new AdventureConfig.AdventureMissionSet(this.OnMissionSet));
  }

  private void Update() => this.CheckForInputForCheats();

  public override bool IsLoaded()
  {
    if (!((UnityEngine.Object) this.m_basePageRenderer == (UnityEngine.Object) null))
      return true;
    Log.Adventures.Print("Currently waiting on m_basePageRenderer to get set before IsLoaded() becomes true.");
    return false;
  }

  public void SetUpPage(
    PageData pageData,
    AdventureBookPageDisplay.PageReadyCallback callback)
  {
    this.StartCoroutine(this.SetUpPageWhenReady(pageData, callback));
  }

  public void SetPageEventListener(Widget.EventListenerDelegate listener) => this.m_pageEventListener = listener;

  public void SetFlipToChapterCallback(
    AdventureBookPageDisplay.FlipToChapterCallback callback)
  {
    this.m_flipToChapterCallback = callback;
  }

  public void SetEnableInteractionCallback(
    AdventureBookPageDisplay.EnableInteractionCallback callback)
  {
    this.m_enableInteractionCallback = callback;
  }

  public AdventureBookPageDataModel GetAdventurePageDataModel() => this.m_pageDataModel;

  public void AllInitialTransitionsComplete() => this.m_allInitialTransitionsComplete = true;

  public override void Show()
  {
    base.Show();
    ScenarioDbId mission1 = ScenarioDbId.INVALID;
    if (this.m_pageData.PageType == AdventureBookPageType.CHAPTER)
    {
      if (!(this.m_pageData is ChapterPageData pageData))
        Debug.LogErrorFormat("Showing a Book Chapter, but it has no data associated with it!");
      else if (pageData.ScenarioRecords.Count == 0)
        Debug.LogErrorFormat("Showing Book Chapter {0}, but it has no ScenarioIds associated with it!", (object) pageData.WingRecord.Name);
      else if (GameUtils.DoesAdventureModeUseDungeonCrawlFormat(AdventureConfig.Get().GetSelectedMode()))
      {
        mission1 = (ScenarioDbId) pageData.ScenarioRecords[0].ID;
      }
      else
      {
        ScenarioDbId mission2 = AdventureConfig.Get().GetMission();
        if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY && mission2 != ScenarioDbId.INVALID && !AdventureProgressMgr.Get().HasDefeatedScenario((int) mission2))
          mission1 = mission2;
      }
    }
    AdventureConfig.Get().SetMission(mission1);
    this.StartCoroutine(this.ShowPageUpdateVisualsWhenReady());
  }

  public bool DoesBundleApplyToPage(Network.Bundle bundle)
  {
    if (this.m_pageData == null)
    {
      Debug.LogError((object) "DoesBundleApplyToPage: No pageData defined for page!");
      return false;
    }
    return this.m_pageData.PageType == AdventureBookPageType.CHAPTER && this.m_pageData is ChapterPageData pageData ? AdventureUtils.DoesBundleIncludeWing(bundle, pageData.WingRecord.ID) : AdventureUtils.DoesBundleIncludeWingForAdventure(bundle, this.m_pageData.Adventure);
  }

  private IEnumerator SetUpPageWhenReady(
    PageData pageData,
    AdventureBookPageDisplay.PageReadyCallback callback)
  {
    this.m_pageData = pageData;
    this.m_needToShowRewardChestAnim = false;
    this.m_rewardChestReadyToShowPopup = false;
    this.m_needToShowChapterCompletionAnim = false;
    this.m_chapterCompletionAnimFinished = false;
    this.m_readyToPlayAdventureNewlyCompletedVO = false;
    this.m_adventureNewlyCompletedSequenceFinished = false;
    this.m_needToShowMissionCompleteAnim = false;
    this.m_missionCompleteAnimFinished = false;
    this.m_needToShowMissionUnlockAnim = false;
    this.m_missionUnlockAnimFinished = false;
    while ((UnityEngine.Object) this.m_adventureBookPageContentsWidget == (UnityEngine.Object) null)
      yield return (object) null;
    this.SetupPageDataModels(pageData);
    string eventName = "ShowBookMapPage";
    if (pageData.PageType == AdventureBookPageType.CHAPTER)
      eventName = "ShowChapterPage";
    else if (pageData.PageType == AdventureBookPageType.REWARD)
      eventName = "ShowCardBackPage";
    if (this.m_adventureBookPageContentsWidget.TriggerEvent(eventName))
    {
      while (this.m_adventureBookPageContentsWidget.IsChangingStates)
        yield return (object) null;
    }
    if (callback != null)
      callback();
  }

  private static AdventureBookPageMoralAlignment ConvertBookSectionToMoralAlignment(
    int section)
  {
    return (AdventureBookPageMoralAlignment) section;
  }

  private void SetupPageDataModels(PageData pageData)
  {
    IDataModel model;
    this.m_adventureBookPageContentsWidget.GetDataModel(2, out model);
    this.m_pageDataModel = model as AdventureBookPageDataModel;
    if (this.m_pageDataModel == null)
    {
      this.m_pageDataModel = new AdventureBookPageDataModel();
      this.m_pageDataModel.ChapterData = new AdventureChapterDataModel();
      this.m_adventureBookPageContentsWidget.BindDataModel((IDataModel) this.m_pageDataModel);
    }
    else
      this.m_pageDataModel.ChapterData = new AdventureChapterDataModel();
    this.m_pageDataModel.PageType = pageData.PageType;
    this.m_pageDataModel.MoralAlignment = AdventureBookPageDisplay.ConvertBookSectionToMoralAlignment(pageData.BookSection);
    this.m_pageDataModel.ChapterData.TimeLocked = false;
    this.m_pageDataModel.ChapterData.FirstHeroBundledWithChapter = 0;
    this.m_pageDataModel.ChapterData.SecondHeroBundledWithChapter = 0;
    this.m_pageDataModel.ChapterData.CompletionRewardType = Reward.Type.NONE;
    this.m_pageDataModel.AllChaptersData.Clear();
    if (pageData.PageType == AdventureBookPageType.CHAPTER)
    {
      if (!(pageData is ChapterPageData chapterData1))
      {
        Debug.LogError((object) "SetupDataModelsAndRefresh(): PageData is not a valid ChapterPageData! Cannot cast properly.");
      }
      else
      {
        AdventureBookPageDisplay.UpdateChapterDataModelWithChapterData(this.m_pageDataModel.ChapterData, chapterData1);
        this.m_needToShowRewardChestAnim = this.m_pageDataModel.ChapterData.CompletionRewardsNewlyEarned;
        this.m_needToShowChapterCompletionAnim = this.m_pageDataModel.ChapterData.NewlyCompleted;
        if (this.m_needToShowChapterCompletionAnim && AdventureProgressMgr.Get().IsAdventureModeAndSectionComplete((AdventureDbId) chapterData1.WingRecord.AdventureId, chapterData1.AdventureMode, chapterData1.BookSection))
        {
          Log.Adventures.Print("You've completed your final Chapter! Setting up Adventure Complete sequence.");
          AdventureBookPageDisplay.NeedToShowAdventureSectionCompletionSequence = true;
        }
        if (this.m_pageDataModel.ChapterData.NewlyUnlocked)
          Log.Adventures.Print("Chapter {0} is newly unlocked!", (object) this.m_pageDataModel.ChapterData.ChapterNumber);
        if (this.m_pageDataModel.ChapterData.NewlyCompleted)
          Log.Adventures.Print("Chapter {0} is newly completed!", (object) this.m_pageDataModel.ChapterData.ChapterNumber);
        if (GameUtils.DoesAdventureModeUseDungeonCrawlFormat(pageData.AdventureMode))
          return;
        foreach (AdventureMissionDataModel mission in this.m_pageDataModel.ChapterData.Missions)
        {
          bool flag = mission.Rewards != null && mission.Rewards.Items != null && mission.Rewards.Items.Count > 0;
          if (mission.NewlyCompleted)
          {
            this.m_needToShowMissionCompleteAnim = true;
            if (flag)
              this.m_needToShowRewardChestAnim = true;
          }
          if (mission.NewlyUnlocked)
            this.m_needToShowMissionUnlockAnim = true;
        }
      }
    }
    else if (pageData.PageType == AdventureBookPageType.MAP)
    {
      if (!(pageData is MapPageData mapPageData))
      {
        Debug.LogError((object) "SetupDataModelsAndRefresh(): PageData is not a valid MapPageData! Cannot cast properly.");
      }
      else
      {
        this.m_pageDataModel.NumChaptersCompletedText = mapPageData.NumChaptersCompletedText;
        while (this.m_sortedChapterDataModels.Count < mapPageData.ChapterData.Values.Count)
          this.m_sortedChapterDataModels.Add(new AdventureChapterDataModel());
        int[] numArray1 = new int[mapPageData.NumSectionsInBook];
        int[] numArray2 = new int[mapPageData.NumSectionsInBook];
        int index1 = 0;
        foreach (ChapterPageData chapterData2 in mapPageData.ChapterData.Values)
        {
          AdventureBookPageDisplay.UpdateChapterDataModelWithChapterData(this.m_sortedChapterDataModels[index1], chapterData2);
          if (chapterData2.BookSection < 0 || chapterData2.BookSection >= mapPageData.NumSectionsInBook)
          {
            Debug.LogErrorFormat("AdventureBookPageDisplay.SetupDataModelsAndRefresh() - chapterData.BookSection {0} is not within the bounds of the number of sections {1}", (object) chapterData2.BookSection, (object) mapPageData.NumSectionsInBook);
          }
          else
          {
            ++numArray2[chapterData2.BookSection];
            if (this.m_sortedChapterDataModels[index1].PlayerOwnsChapter)
              ++numArray1[chapterData2.BookSection];
          }
          ++index1;
        }
        this.m_sortedChapterDataModels.Sort((Comparison<AdventureChapterDataModel>) ((a, b) => a.ChapterNumber - b.ChapterNumber));
        this.m_pageDataModel.AllChaptersData.AddRange((IEnumerable<AdventureChapterDataModel>) this.m_sortedChapterDataModels);
        while (this.m_pageDataModel.NumChaptersOwnedText.Count < numArray1.Length)
          this.m_pageDataModel.NumChaptersOwnedText.Add("");
        for (int index2 = 0; index2 < numArray1.Length; ++index2)
        {
          if (numArray1[index2] < numArray2[index2])
            this.m_pageDataModel.NumChaptersOwnedText[index2] = GameStrings.Format("GLUE_ADVENTURE_NUM_CHAPTERS_OWNED", (object) numArray1[index2]);
          else
            this.m_pageDataModel.NumChaptersOwnedText[index2] = "";
        }
        this.UpdateMapButtonData();
      }
    }
    else
    {
      if (pageData.PageType != AdventureBookPageType.REWARD)
        return;
      this.UpdateRewardPageData(pageData);
    }
  }

  private void OnChapterClickableRelease(UIEvent e)
  {
    if (!(e.GetElement().GetData() is AdventureBookPageDisplay.ChapterButtonData data))
    {
      Log.Adventures.PrintError("Chapter Button pressed, but the button has no data!");
    }
    else
    {
      Log.Adventures.Print("Released {0}!", (object) data.ButtonName);
      if (this.m_flipToChapterCallback == null)
        return;
      this.m_flipToChapterCallback(data.ChapterData.ChapterNumber);
    }
  }

  public void HideAndSuppressChapterUnlockSequence()
  {
    if (!this.m_isInUnlockedSequence)
      return;
    this.m_screenEffectsHandle.StopEffect();
    if (string.IsNullOrEmpty(this.m_currentUnlockButtonName) || !this.m_chapterButtonClickablesNameMap.ContainsKey(this.m_currentUnlockButtonName))
      return;
    Clickable buttonClickablesName = this.m_chapterButtonClickablesNameMap[this.m_currentUnlockButtonName];
    if ((UnityEngine.Object) buttonClickablesName == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("Chapter Button {0} is missing!", (object) this.m_currentUnlockButtonName);
    }
    else
    {
      VisualController component = buttonClickablesName.GetComponent<VisualController>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Error.AddDevWarning("Missing Visual Controller", "{0} does not have a visual controller!", (object) this.m_currentUnlockButtonName);
      }
      else
      {
        IDataModel dataModel;
        buttonClickablesName.GetDataModel(3, out dataModel);
        if (dataModel is AdventureChapterDataModel chapterDataModel)
          chapterDataModel.WantsNewlyUnlockedSequence = false;
        component.Owner.TriggerEvent("CODE_HIDE_AND_DISMISS", new Widget.TriggerEventParameters());
        this.m_currentUnlockButtonName = (string) null;
      }
    }
  }

  private void OnChapterUnlockButtonClicked()
  {
    if (!(this.m_pageData is ChapterPageData pageData))
      return;
    if (AdventureProgressMgr.Get().OwnsWing(pageData.WingRecord.ID) && pageData.WingRecord.PmtProductIdForSingleWingPurchase == 0 && AdventureConfig.Get().ShouldSeeFirstTimeFlow)
    {
      AdventureUtils.DisplayFirstChapterFreePopup(pageData);
    }
    else
    {
      if (AdventureConfig.Get().ShouldSeeFirstTimeFlow)
        return;
      bool flag = AdventureConfig.Get().GetSelectedAdventure() != AdventureDbId.DALARAN && AdventureConfig.Get().GetSelectedAdventure() != AdventureDbId.ULDUM;
      if (this.m_pageDataModel.ChapterData.AvailableForPurchase && pageData.WingRecord.PmtProductIdForThisAndRestOfAdventure == 0)
        AdventureBookPageDisplay.StartSingleWingPurchaseTransaction(this.m_pageData, this.m_pageDataModel);
      else if (this.m_pageDataModel.ChapterData.AvailableForPurchase | flag)
        this.SetupAdventurePurchaseChoiceDialog(pageData);
      else
        AdventureBookPageDisplay.StartFullBookPurchaseTransaction(this.m_pageData, this.m_pageDataModel);
    }
  }

  private void OnBossSelected(int bossOffset)
  {
    if ((UnityEngine.Object) this.m_adventureBookPageContentsWidget == (UnityEngine.Object) null)
    {
      Debug.LogError((object) "AdventureBookPageDisplay: OnBossSelected() called when m_adventureBookPageContentsWidget is null!");
    }
    else
    {
      IDataModel model;
      this.m_adventureBookPageContentsWidget.GetDataModel(2, out model);
      if (!(model is AdventureBookPageDataModel bookPageDataModel))
        Error.AddDevWarning("UI Error", "No AdventureBookPageDataModel bound to the AdventureBookPageContents widget when the boss was selected!");
      else if (bookPageDataModel.ChapterData == null)
        Error.AddDevWarning("UI Error", "AdventureBookPageDataModel's ChapterData is null when the boss was selected!");
      else if (bookPageDataModel.ChapterData.Missions.Count <= bossOffset)
      {
        Error.AddDevWarning("UI Error", "Selected boss index {0} but there are only {1} missions defined for Chapter {2}!", (object) bossOffset, (object) bookPageDataModel.ChapterData.Missions.Count, (object) bookPageDataModel.ChapterData.Name);
      }
      else
      {
        AdventureMissionDataModel mission = bookPageDataModel.ChapterData.Missions[bossOffset];
        if (mission == null)
          Error.AddDevWarning("UI Error", "AdventureMissionDataModel at index {0} for Chapter {1} is not valid!", (object) bossOffset, (object) bookPageDataModel.ChapterData.Name);
        else
          AdventureConfig.Get().SetMission(mission.ScenarioId);
      }
    }
  }

  private void OnMissionSet(ScenarioDbId mission, bool showDetails)
  {
    IDataModel model;
    this.m_adventureBookPageContentsWidget.GetDataModel(2, out model);
    if (!(model is AdventureBookPageDataModel bookPageDataModel) || bookPageDataModel.ChapterData == null)
      return;
    foreach (AdventureMissionDataModel mission1 in bookPageDataModel.ChapterData.Missions)
      mission1.Selected = mission1.ScenarioId == mission;
  }

  private void OnChapterUnlockAnimationComplete(string eventName)
  {
    if (eventName != "CODE_UNLOCKED_ANIMATION_COMPLETE")
      return;
    this.m_isInUnlockedSequence = false;
    if (string.IsNullOrEmpty(this.m_currentUnlockButtonName))
    {
      Log.Adventures.PrintWarning("AdventureBookPageDisplay.OnChapterUnlockAnimationComplete: Current unlock button was not set, if this was manually activated outside the normal flow then this can be ignored.");
    }
    else
    {
      Clickable clickable = (Clickable) null;
      if (!this.m_chapterButtonClickablesNameMap.TryGetValue(this.m_currentUnlockButtonName, out clickable))
      {
        Log.Adventures.PrintError("AdventureBookPageDisplay.OnChapterUnlockAnimationComplete: Could not find current unlock button {0}.", (object) this.m_currentUnlockButtonName);
      }
      else
      {
        if (clickable.GetData() is AdventureBookPageDisplay.ChapterButtonData data)
        {
          AdventureConfig.Get().SetHasSeenUnlockedChapterPage((WingDbId) data.ChapterData.WingRecord.ID, false);
          AdventureConfig.AckCurrentWingProgress(data.ChapterData.WingRecord.ID);
          Log.Adventures.Print("Pressed {0} {1}!", (object) data.ButtonName, (object) this.m_currentUnlockButtonName);
        }
        IDataModel dataModel;
        clickable.GetDataModel(3, out dataModel);
        if (dataModel is AdventureChapterDataModel chapterDataModel)
        {
          chapterDataModel.WantsNewlyUnlockedSequence = false;
          chapterDataModel.NewlyUnlocked = false;
          chapterDataModel.ShowNewlyUnlockedHighlight = true;
        }
        this.m_currentUnlockButtonName = (string) null;
        this.ShowChapterNewlyUnlockedMapSequenceIfNecessary();
        this.EnableInteraction(true);
      }
    }
  }

  private void SetupAdventurePurchaseChoiceDialog(ChapterPageData pageData)
  {
    if ((UnityEngine.Object) this.m_storeChooseWidget == (UnityEngine.Object) null)
    {
      this.m_storeChooseWidget = (Widget) WidgetInstance.Create((string) AdventureBookPageDisplay.m_chooseStoreWidgetPrefab);
      this.m_storeChooseWidget.transform.parent = this.transform;
    }
    this.m_storeChooseWidget.RegisterReadyListener((Action<object>) (_ =>
    {
      this.m_storeChooseWidget.BindDataModel((IDataModel) this.m_pageDataModel);
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = this.m_popupEffectFadeTime
      });
      if ((UnityEngine.Object) this.m_storeChooseBackButton == (UnityEngine.Object) null)
        this.m_storeChooseBackButton = this.m_storeChooseWidget.GetComponentInChildren<UIBButton>();
      if ((UnityEngine.Object) this.m_storeChoosePopup == (UnityEngine.Object) null)
        this.m_storeChoosePopup = this.m_storeChooseWidget.GetComponentInChildren<UIBPopup>();
      this.m_storeChoosePopup.Show(false);
      Navigation.Push(new Navigation.NavigateBackHandler(this.HideStoreChoosePopup));
      this.m_storeChooseBackButton.AddEventListener(UIEventType.RELEASE, (UIEvent.Handler) (chooseEvent => Navigation.GoBack()));
      this.m_storeChooseWidget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnBookStoreChosenEvent));
    }), (object) null, true);
    StoreManager.Get().RegisterSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
  }

  private void OnSuccessfulPurchaseAck(Network.Bundle bundle, PaymentMethod method)
  {
    if (!this.DoesBundleApplyToPage(bundle) || !((UnityEngine.Object) this.m_storeChoosePopup != (UnityEngine.Object) null) || !this.m_storeChoosePopup.IsShown())
      return;
    Navigation.GoBack();
  }

  private bool HideStoreChoosePopup()
  {
    StoreManager.Get().RemoveSuccessfulPurchaseAckListener(new Action<Network.Bundle, PaymentMethod>(this.OnSuccessfulPurchaseAck));
    if ((UnityEngine.Object) this.m_storeChoosePopup == (UnityEngine.Object) null)
      return false;
    this.m_screenEffectsHandle.StopEffect();
    this.m_storeChoosePopup.Hide();
    return true;
  }

  private static void StartFullBookPurchaseTransaction(
    PageData pageData,
    AdventureBookPageDataModel pageDataModel)
  {
    WingDbfRecord wingDbfRecord = pageData is ChapterPageData chapterPageData ? chapterPageData.WingRecord : (WingDbfRecord) null;
    if (wingDbfRecord == null)
    {
      Debug.LogError((object) "AdventureBookPageDisplay.StartFullBookPurchaseTransaction: could not get the wing record from page data when trying to purchase the entire adventure book.");
    }
    else
    {
      WingDbfRecord unownedAdventureWing = AdventureProgressMgr.Get().GetFirstUnownedAdventureWing((AdventureDbId) wingDbfRecord.AdventureId);
      if (unownedAdventureWing == null)
        Debug.LogError((object) "AdventureBookPageDisplay.StartFullBookPurchaseTransaction: could not find a first unowned wing - something went wrong!");
      else if (!AdventureProgressMgr.Get().OwnershipPrereqWingIsOwned(unownedAdventureWing))
        Debug.LogErrorFormat("AdventureBookPageDisplay.StartFullBookPurchaseTransaction: You do not own wing {0}, you cannot purchase the entire adventure book starting at wing {1}!", (object) wingDbfRecord.OwnershipPrereqWingId, (object) unownedAdventureWing.ID);
      else
        StoreManager.Get().StartAdventureTransaction(ProductType.PRODUCT_TYPE_WING, wingDbfRecord.ID, (Store.ExitCallback) null, (object) null, ShopType.ADVENTURE_STORE_FULL_PURCHASE_WIDGET, dataModel: ((IDataModel) pageDataModel), pmtProductId: unownedAdventureWing.PmtProductIdForThisAndRestOfAdventure);
    }
  }

  private static void StartSingleWingPurchaseTransaction(
    PageData pageData,
    AdventureBookPageDataModel pageDataModel)
  {
    WingDbfRecord wingRecord = pageData is ChapterPageData chapterPageData ? chapterPageData.WingRecord : (WingDbfRecord) null;
    if (wingRecord == null)
      Debug.LogError((object) "AdventureBookPageDisplay.OnBookStoreChosenEvent: could not get the wing record from page data when trying to purchase a specific wing.");
    else if (!AdventureProgressMgr.Get().OwnershipPrereqWingIsOwned(wingRecord))
      Debug.LogErrorFormat("AdventureBookPageDisplay.OnBookStoreChosenEvent: You do not own wing {0}, you cannot purchase the wing on this page!", (object) wingRecord.OwnershipPrereqWingId);
    else
      StoreManager.Get().StartAdventureTransaction(ProductType.PRODUCT_TYPE_WING, wingRecord.ID, (Store.ExitCallback) null, (object) null, ShopType.ADVENTURE_STORE_WING_PURCHASE_WIDGET, dataModel: ((IDataModel) pageDataModel), pmtProductId: wingRecord.PmtProductIdForSingleWingPurchase);
  }

  private void OnBookStoreChosenEvent(string eventName)
  {
    if (eventName == "book_selected")
    {
      if ((UnityEngine.Object) this.m_storeChoosePopup != (UnityEngine.Object) null && this.m_storeChoosePopup.IsShown())
        Navigation.GoBack();
      AdventureBookPageDisplay.StartFullBookPurchaseTransaction(this.m_pageData, this.m_pageDataModel);
    }
    else
    {
      if (!(eventName == "chapter_selected"))
        return;
      if ((UnityEngine.Object) this.m_storeChoosePopup != (UnityEngine.Object) null && this.m_storeChoosePopup.IsShown())
        Navigation.GoBack();
      AdventureBookPageDisplay.StartSingleWingPurchaseTransaction(this.m_pageData, this.m_pageDataModel);
    }
  }

  private void AdventureBookPageContentsIsReady(Widget bookPageContents)
  {
    this.m_adventureBookPageContentsWidget = bookPageContents;
    if ((UnityEngine.Object) bookPageContents == (UnityEngine.Object) null)
      Error.AddDevWarning("Error", "Error: Adventure Book Page Contents Reference not hooked up to a Widget!");
    else
      this.StartCoroutine(this.SetUpBookPageReferencesWhenResolved(bookPageContents));
  }

  private IEnumerator SetUpBookPageReferencesWhenResolved(Widget bookPageContents)
  {
    AdventureBookPageDisplay adventureBookPageDisplay = this;
    while (bookPageContents.IsChangingStates)
      yield return (object) null;
    bookPageContents.RegisterEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.BookPageContentsEventListener));
    AdventureBookPageDisplayRefContainer componentInChildren = bookPageContents.gameObject.GetComponentInChildren<AdventureBookPageDisplayRefContainer>();
    if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "There is no AdventureBookPageDisplayRefContainer component on your AdventureBookPageContents Widget! This is necessary to initialize things like the Map Page.");
    }
    else
    {
      componentInChildren.m_AdventureBookMapReference.RegisterReadyListener<Widget>(new Action<Widget>(adventureBookPageDisplay.AdventureBookMapIsReady));
      componentInChildren.m_BasePageRendererReference.RegisterReadyListener<MeshRenderer>(new Action<MeshRenderer>(adventureBookPageDisplay.BasePageRendererIsReady));
    }
  }

  private void BasePageRendererIsReady(MeshRenderer basePageRenderer) => this.m_basePageRenderer = basePageRenderer;

  private void AdventureBookMapIsReady(Widget widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null || !widget.IsReady)
    {
      Log.Adventures.PrintError("AdventureBookMap should be ready, but it's not!  Something terrible is happening!");
    }
    else
    {
      widget.RegisterEventListener(new Widget.EventListenerDelegate(this.OnChapterUnlockAnimationComplete));
      this.StartCoroutine(this.InitializeMapButtonsWhenResolved(widget));
    }
  }

  public IEnumerator InitializeMapButtonsWhenResolved(Widget bookMapWidget)
  {
    while (bookMapWidget.IsChangingStates)
      yield return (object) null;
    MapPageData mapData = this.m_pageData as MapPageData;
    if (mapData == null)
    {
      Log.Adventures.PrintError("SetUpPageWhenReady(): m_pageData is not a valid MapPageData! Cannot cast properly.");
    }
    else
    {
      ListOfChapterButtons componentInChildren = bookMapWidget.gameObject.GetComponentInChildren<ListOfChapterButtons>();
      if (!((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null))
      {
        List<AsyncReference> clickableReferences = componentInChildren.m_ChapterButtonClickableReferences;
        if (clickableReferences.Count != mapData.ChapterData.Count)
          Error.AddDevWarning("Missing Adventure Buttons", "Error: there are not the same number of Chapter Buttons ({0}) as there are Chapters ({1}) defined for this Adventure!", (object) clickableReferences.Count, (object) mapData.ChapterData.Count);
        this.m_chapterButtonClickablesNameMap.Clear();
        for (int i = 0; i < clickableReferences.Count; ++i)
          clickableReferences[i].RegisterReadyListener<Clickable>((Action<Clickable>) (chapterButton =>
          {
            int key1 = i + 1;
            if ((UnityEngine.Object) chapterButton == (UnityEngine.Object) null)
            {
              Debug.LogErrorFormat("The reference to a ChapterButton at index {0} in the ListOfChapterButtons component is not a valid Clickable!", (object) key1);
            }
            else
            {
              ChapterPageData chapterPageData;
              mapData.ChapterData.TryGetValue(key1, out chapterPageData);
              if (chapterPageData == null)
              {
                Log.Adventures.PrintError("No ChapterData in the MapPageData for Chapter {0}!", (object) key1);
              }
              else
              {
                string key2 = chapterButton.gameObject.name + (object) key1;
                AdventureBookPageDisplay.ChapterButtonData data = new AdventureBookPageDisplay.ChapterButtonData()
                {
                  ChapterData = chapterPageData,
                  ButtonName = key2
                };
                this.m_chapterButtonClickablesNameMap.Add(key2, chapterButton);
                chapterButton.SetData((object) data);
                chapterButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnChapterClickableRelease));
              }
            }
          }));
        this.UpdateMapButtonData();
      }
    }
  }

  private void UpdateMapOnWingProgressUpdated(
    bool isStartupAction,
    AdventureMission.WingProgress oldProgress,
    AdventureMission.WingProgress newProgress,
    object userData)
  {
    this.UpdateMapButtonData(true);
  }

  private void UpdateMapButtonData(bool forceUpdate = false)
  {
    if (this.m_sortedChapterDataModels == null)
    {
      Debug.LogError((object) "AdventureBookPageDisplay.UpdateMapButtonData() - m_sortedChapterDataModels is null!");
    }
    else
    {
      this.m_chapterNewlyUnlockedMapSequenceQueue.Clear();
      foreach (Clickable clickable in this.m_chapterButtonClickablesNameMap.Values)
      {
        AdventureBookPageDisplay.ChapterButtonData chapterButtonData = clickable.GetData() as AdventureBookPageDisplay.ChapterButtonData;
        if (chapterButtonData == null)
        {
          Log.Adventures.PrintError("Data on Chapter Button is not valid ChapterButtonData!");
        }
        else
        {
          AdventureChapterDataModel chapterDataModel = this.m_sortedChapterDataModels.Find((Predicate<AdventureChapterDataModel>) (x => x.ChapterNumber == chapterButtonData.ChapterData.ChapterNumber));
          if (chapterDataModel == null)
          {
            Debug.LogErrorFormat("AdventureBookPageDisplay.UpdateMapButtonData() - No ChapterDataModel for Chapter {0} found in m_sortedChapterDataModels!", (object) chapterButtonData.ChapterData.ChapterNumber);
          }
          else
          {
            if (forceUpdate)
              AdventureBookPageDisplay.UpdateChapterDataModelWithChapterData(chapterDataModel, chapterButtonData.ChapterData);
            clickable.BindDataModel((IDataModel) chapterDataModel);
            if (chapterDataModel.NewlyUnlocked)
            {
              Log.Adventures.Print("Chapter {0} is newly unlocked!", (object) chapterDataModel.ChapterNumber);
              this.m_chapterNewlyUnlockedMapSequenceQueue.Enqueue(chapterButtonData.ButtonName);
            }
            if (chapterDataModel.NewlyCompleted)
              Log.Adventures.Print("Chapter {0} is newly completed!", (object) chapterDataModel.ChapterNumber);
          }
        }
      }
    }
  }

  private static void UpdateChapterDataModelWithChapterData(
    AdventureChapterDataModel chapterDataModel,
    ChapterPageData chapterData)
  {
    WingDbfRecord wingRecord = chapterData.WingRecord;
    chapterDataModel.Name = (string) wingRecord.Name;
    chapterDataModel.Description = (string) wingRecord.Description;
    chapterDataModel.ChapterNumber = chapterData.ChapterNumber;
    chapterDataModel.WingId = wingRecord.ID;
    chapterDataModel.ChapterState = AdventureProgressMgr.Get().AdventureBookChapterStateForWing(wingRecord, chapterData.AdventureMode);
    chapterDataModel.TimeLocked = !AdventureProgressMgr.IsWingEventActive(wingRecord.ID);
    chapterDataModel.UnlockChapterText = (string) wingRecord.StoreBuyWingButtonLabel;
    chapterDataModel.StoreDescriptionText = (string) wingRecord.StoreBuyWingDesc;
    chapterDataModel.IsAnomalyModeAvailable = AdventureUtils.IsAnomalyModeAvailable(chapterData.Adventure, chapterData.AdventureMode, (WingDbId) wingRecord.ID);
    if (chapterDataModel.TimeLocked)
      chapterDataModel.TimeLockInfoMessage = (string) wingRecord.ComingSoonLabel;
    chapterDataModel.PlayerOwnsChapter = AdventureProgressMgr.Get().OwnsWing(wingRecord.ID);
    AdventureDbfRecord record1 = GameDbf.Adventure.GetRecord((int) chapterData.Adventure);
    if (chapterDataModel.PlayerOwnsChapter && wingRecord.PmtProductIdForSingleWingPurchase == 0 && AdventureConfig.Get().ShouldSeeFirstTimeFlow && record1 != null && record1.MapPageHasButtonsToChapters)
      chapterDataModel.PlayerOwnsChapter = false;
    chapterDataModel.IsPreviousChapterOwned = AdventureProgressMgr.Get().OwnershipPrereqWingIsOwned(wingRecord);
    WingDbfRecord record2 = GameDbf.Wing.GetRecord(wingRecord.OwnershipPrereqWingId);
    if (record2 != null && record2.PmtProductIdForSingleWingPurchase == 0 && AdventureConfig.Get().ShouldSeeFirstTimeFlow)
      chapterDataModel.IsPreviousChapterOwned = false;
    chapterDataModel.AvailableForPurchase = !chapterDataModel.PlayerOwnsChapter && chapterDataModel.IsPreviousChapterOwned && !GameUtils.IsModeHeroic(chapterData.AdventureMode);
    chapterDataModel.FinalPurchasableChapter = wingRecord.PmtProductIdForThisAndRestOfAdventure == 0 && wingRecord.PmtProductIdForSingleWingPurchase != 0;
    int ack;
    AdventureProgressMgr.Get().GetWingAck(wingRecord.ID, out ack);
    chapterDataModel.NewlyUnlocked = chapterDataModel.ChapterState == AdventureChapterState.UNLOCKED && ack == 0;
    chapterDataModel.ShowNewlyUnlockedHighlight = !AdventureConfig.Get().GetHasSeenUnlockedChapterPage((WingDbId) chapterData.WingRecord.ID);
    List<int> guestHeroesForWing = AdventureConfig.GetGuestHeroesForWing(wingRecord.ID);
    if (guestHeroesForWing != null && guestHeroesForWing.Count != 0)
    {
      chapterDataModel.FirstHeroBundledWithChapter = guestHeroesForWing[0];
      if (guestHeroesForWing.Count >= 2)
        chapterDataModel.SecondHeroBundledWithChapter = guestHeroesForWing[1];
      if (guestHeroesForWing.Count > 2)
        Log.Adventures.Print("{0} Guest Heroes defined for Wing {0}, but we only have room in the data model for 2!", (object) guestHeroesForWing.Count, (object) wingRecord.ID);
    }
    chapterDataModel.DisplayRaidBossHealth = wingRecord.DisplayRaidBossHealth;
    chapterDataModel.RaidBossHealthAmount = 0;
    if (chapterDataModel.DisplayRaidBossHealth)
    {
      string cardId = GameUtils.TranslateDbIdToCardId(wingRecord.RaidBossCardId);
      if (cardId == null || wingRecord.RaidBossCardId == 0)
      {
        Log.Adventures.PrintWarning("AdventureBookPageDisplay.UpdateChapterDataModelWithChapterData() - No cardId for raid boss dbId {0}!", (object) wingRecord.RaidBossCardId);
      }
      else
      {
        EntityDef entityDef = DefLoader.Get().GetEntityDef(cardId);
        if (entityDef == null)
        {
          Log.Adventures.PrintWarning("AdventureBookPageDisplay.UpdateChapterDataModelWithChapterData() - No EntityDef for raid boss card ID {0}!", (object) cardId);
        }
        else
        {
          chapterDataModel.RaidBossStartingHealthAmount = entityDef.GetTag(GAME_TAG.HEALTH);
          chapterDataModel.RaidBossHealthAmount = chapterDataModel.RaidBossStartingHealthAmount;
        }
      }
      AdventureDataDbfRecord adventureDataRecord = GameUtils.GetAdventureDataRecord((int) chapterData.Adventure, (int) chapterData.AdventureMode);
      if (adventureDataRecord != null)
      {
        GameSaveKeyId saveDataServerKey = (GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey;
        List<long> values;
        GameSaveDataManager.Get().GetSubkeyValue(saveDataServerKey, GameSaveKeySubkeyId.DUNGEON_CRAWL_FINAL_BOSS_HEALTH, out values);
        int sortedWingUnlockIndex = GameUtils.GetSortedWingUnlockIndex(wingRecord);
        if (values != null && values.Count > sortedWingUnlockIndex)
          chapterDataModel.RaidBossHealthAmount = Mathf.Clamp((int) values[sortedWingUnlockIndex], 0, chapterDataModel.RaidBossStartingHealthAmount);
      }
    }
    chapterDataModel.CompletionRewards = new RewardListDataModel();
    HashSet<Assets.Achieve.RewardTiming> rewardTimings1 = new HashSet<Assets.Achieve.RewardTiming>()
    {
      Assets.Achieve.RewardTiming.ADVENTURE_CHEST
    };
    List<RewardData> wingCompletionRewards = new List<RewardData>();
    List<Achievement> forAdventureWing = AchieveManager.Get().GetAchievesForAdventureWing(wingRecord.ID);
    foreach (Achievement achievement in forAdventureWing)
    {
      if (achievement.Scenarios.Count <= 0)
        wingCompletionRewards.AddRange((IEnumerable<RewardData>) AchieveManager.Get().GetRewardsForAchieve(achievement.ID, rewardTimings1));
    }
    chapterDataModel.CompletionRewardsEarned = false;
    chapterDataModel.CompletionRewardsNewlyEarned = false;
    AdventureBookPageDisplay.Legacy_SetChapterCompletionRewardData(chapterDataModel, wingCompletionRewards);
    foreach (RewardData rewardData in wingCompletionRewards)
    {
      RewardItemDataModel rewardItemDataModel = RewardUtils.RewardDataToRewardItemDataModel(rewardData);
      if (rewardItemDataModel != null)
        chapterDataModel.CompletionRewards.Items.Add(rewardItemDataModel);
      if (rewardData.Origin == NetCache.ProfileNotice.NoticeOrigin.ACHIEVEMENT)
      {
        Achievement achievement = AchieveManager.Get().GetAchievement((int) rewardData.OriginData);
        chapterDataModel.CompletionRewardsEarned |= achievement.IsCompleted();
        chapterDataModel.CompletionRewardsNewlyEarned |= achievement.IsNewlyCompleted();
      }
      else
        Error.AddDevWarning("Reward Error!", "Wing Reward is from origin {0}, but we expected origin == ACHIEVEMENT!", (object) rewardData.Origin);
    }
    chapterDataModel.PurchaseRewards = new RewardListDataModel();
    List<RewardData> rewardDataList = new List<RewardData>();
    HashSet<Assets.Achieve.RewardTiming> rewardTimings2 = new HashSet<Assets.Achieve.RewardTiming>()
    {
      Assets.Achieve.RewardTiming.IMMEDIATE
    };
    foreach (Achievement achievement in forAdventureWing)
    {
      if (achievement.AchieveTrigger == Assets.Achieve.Trigger.LICENSEDETECTED)
        rewardDataList.AddRange((IEnumerable<RewardData>) AchieveManager.Get().GetRewardsForAchieve(achievement.ID, rewardTimings2));
    }
    foreach (RewardData rewardData in rewardDataList)
    {
      RewardItemDataModel rewardItemDataModel = RewardUtils.RewardDataToRewardItemDataModel(rewardData);
      if (rewardItemDataModel != null)
        chapterDataModel.PurchaseRewards.Items.Add(rewardItemDataModel);
    }
    int mission = (int) AdventureConfig.Get().GetMission();
    chapterDataModel.Missions.Clear();
    bool flag1 = false;
    foreach (ScenarioDbfRecord scenarioRecord in chapterData.ScenarioRecords)
    {
      AdventureMissionDataModel missionDataModel = new AdventureMissionDataModel();
      missionDataModel.Rewards = new RewardListDataModel();
      missionDataModel.ScenarioId = (ScenarioDbId) scenarioRecord.ID;
      missionDataModel.Selected = mission == scenarioRecord.ID;
      missionDataModel.MissionState = AdventureProgressMgr.Get().AdventureMissionStateForScenario(scenarioRecord.ID);
      HashSet<Assets.Achieve.RewardTiming> rewardTimings3 = new HashSet<Assets.Achieve.RewardTiming>()
      {
        Assets.Achieve.RewardTiming.ADVENTURE_CHEST,
        Assets.Achieve.RewardTiming.IMMEDIATE
      };
      List<RewardData> defeatingScenario = AdventureProgressMgr.Get().GetRewardsForDefeatingScenario(scenarioRecord.ID, rewardTimings3);
      missionDataModel.Rewards.Items.Clear();
      foreach (RewardData rewardData in defeatingScenario)
      {
        RewardItemDataModel rewardItemDataModel = RewardUtils.RewardDataToRewardItemDataModel(rewardData);
        if (rewardItemDataModel != null)
          missionDataModel.Rewards.Items.Add(rewardItemDataModel);
      }
      AdventureConfig.Get().LoadBossDef((ScenarioDbId) scenarioRecord.ID, (AdventureConfig.DelBossDefLoaded) ((bossDef, success) =>
      {
        if (!((UnityEngine.Object) bossDef != (UnityEngine.Object) null))
          return;
        missionDataModel.CoinPortraitMaterial = bossDef.m_CoinPortraitMaterial.GetMaterial();
      }));
      int wingId = 0;
      int missionReqProgress = 0;
      bool reqs = AdventureConfig.IsMissionNewlyAvailableAndGetReqs((int) missionDataModel.ScenarioId, ref wingId, ref missionReqProgress);
      missionDataModel.NewlyUnlocked = missionDataModel.MissionState == AdventureMissionState.UNLOCKED & reqs;
      bool flag2 = false;
      if (AdventureConfig.Get().IsScenarioDefeatedAndInitCache((ScenarioDbId) scenarioRecord.ID))
        flag2 = AdventureConfig.Get().IsScenarioJustDefeated((ScenarioDbId) scenarioRecord.ID);
      missionDataModel.NewlyCompleted = missionDataModel.MissionState == AdventureMissionState.COMPLETED & flag2;
      if (missionDataModel.NewlyCompleted)
        flag1 = true;
      chapterDataModel.Missions.Add(missionDataModel);
    }
    chapterDataModel.NewlyCompleted = chapterDataModel.ChapterState == AdventureChapterState.COMPLETED & flag1;
    chapterDataModel.MoralAlignment = AdventureBookPageDisplay.ConvertBookSectionToMoralAlignment(chapterData.BookSection);
  }

  private static void Legacy_SetChapterCompletionRewardData(
    AdventureChapterDataModel chapterDataModel,
    List<RewardData> wingCompletionRewards)
  {
    RewardData rewardData = (RewardData) null;
    if (wingCompletionRewards.Count > 0)
      rewardData = wingCompletionRewards[0];
    if (rewardData is BoosterPackRewardData)
    {
      chapterDataModel.CompletionRewardType = rewardData.RewardType;
      BoosterPackRewardData boosterPackRewardData = rewardData as BoosterPackRewardData;
      chapterDataModel.CompletionRewardId = boosterPackRewardData.Id;
      chapterDataModel.CompletionRewardQuantity = boosterPackRewardData.Count;
    }
    else
    {
      chapterDataModel.CompletionRewardType = Reward.Type.NONE;
      chapterDataModel.CompletionRewardId = 0;
      chapterDataModel.CompletionRewardQuantity = 0;
    }
  }

  private IEnumerator ShowPageUpdateVisualsWhenReady()
  {
    AdventureBookPageDisplay adventureBookPageDisplay = this;
    while (!adventureBookPageDisplay.m_allInitialTransitionsComplete)
      yield return (object) null;
    if (adventureBookPageDisplay.m_pageData.PageType == AdventureBookPageType.MAP)
    {
      if (AdventureBookPageDisplay.NeedToShowAdventureSectionCompletionSequence)
        adventureBookPageDisplay.StartCoroutine(adventureBookPageDisplay.AnimateAdventureSectionComplete());
      else
        adventureBookPageDisplay.ShowChapterNewlyUnlockedMapSequenceIfNecessary();
    }
    else if (adventureBookPageDisplay.m_pageData.PageType == AdventureBookPageType.CHAPTER)
      adventureBookPageDisplay.StartCoroutine(adventureBookPageDisplay.AnimateChapterRewardsAndCompletionIfNecessary());
  }

  private void ShowChapterNewlyUnlockedMapSequenceIfNecessary()
  {
    if (this.m_chapterNewlyUnlockedMapSequenceQueue.Count <= 0 || this.m_isInUnlockedSequence || AdventureConfig.Get().ShouldSeeFirstTimeFlow)
      return;
    string key = this.m_chapterNewlyUnlockedMapSequenceQueue.Dequeue();
    Clickable buttonClickablesName = this.m_chapterButtonClickablesNameMap[key];
    if ((UnityEngine.Object) buttonClickablesName == (UnityEngine.Object) null)
    {
      Log.Adventures.PrintError("m_chapterNewlyUnlockedPopupQueue had an invalid button name! Skipping...");
      this.ShowChapterNewlyUnlockedMapSequenceIfNecessary();
    }
    else if ((UnityEngine.Object) buttonClickablesName.GetComponent<VisualController>() == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("Missing Visual Controller", "{0} does not have a visual controller!", (object) key);
    }
    else
    {
      IDataModel dataModel;
      buttonClickablesName.GetDataModel(3, out dataModel);
      if (dataModel is AdventureChapterDataModel chapterDataModel)
        chapterDataModel.WantsNewlyUnlockedSequence = true;
      this.m_currentUnlockButtonName = key;
      this.m_isInUnlockedSequence = true;
    }
  }

  public void ShowNewlyPurchasedSequenceOnChapterPage()
  {
    if (this.m_pageData.PageType != AdventureBookPageType.CHAPTER)
      Debug.LogWarning((object) "AdventureBookPageDisplay.ShowNewlyPurchasedSequenceOnChapterPage() called on a non-Chapter page!  This is not supported!");
    else
      this.StartCoroutine(this.AnimateNewlyPurchasedSequenceOnChapterPage());
  }

  private IEnumerator AnimateNewlyPurchasedSequenceOnChapterPage()
  {
    AdventureBookPageDisplay adventureBookPageDisplay = this;
    adventureBookPageDisplay.m_chapterNewlyPurchasedAnimFinished = false;
    adventureBookPageDisplay.m_adventureBookPageContentsWidget.RegisterEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.ChapterNewlyPurchasedAnimEventListener));
    adventureBookPageDisplay.m_adventureBookPageContentsWidget.TriggerEvent("PLAY_CHAPTER_NEWLY_PURCHASED_ANIM");
    while (!adventureBookPageDisplay.m_chapterNewlyPurchasedAnimFinished)
      yield return (object) null;
    adventureBookPageDisplay.m_adventureBookPageContentsWidget.RemoveEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.ChapterNewlyPurchasedAnimEventListener));
    adventureBookPageDisplay.RefreshPage();
  }

  private void RefreshPage()
  {
    this.SetupPageDataModels(this.m_pageData);
    this.StartCoroutine(this.ShowPageUpdateVisualsWhenReady());
    AdventureConfig.Get().SetMission(AdventureConfig.Get().GetMission());
  }

  private IEnumerator AnimateAdventureSectionComplete()
  {
    AdventureBookPageDisplay adventureBookPageDisplay = this;
    adventureBookPageDisplay.EnableInteraction(false);
    AdventureBookPageDisplay.NeedToShowAdventureSectionCompletionSequence = false;
    adventureBookPageDisplay.m_readyToPlayAdventureNewlyCompletedVO = false;
    adventureBookPageDisplay.m_adventureNewlyCompletedSequenceFinished = false;
    adventureBookPageDisplay.m_adventureBookPageContentsWidget.RegisterEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.AdventureNewlyCompletedEventListener));
    adventureBookPageDisplay.m_adventureBookPageContentsWidget.TriggerEvent("AdventureNewlyCompletedSequence");
    while (!adventureBookPageDisplay.m_readyToPlayAdventureNewlyCompletedVO)
      yield return (object) null;
    WingDbId wingId = adventureBookPageDisplay.m_pageData is ChapterPageData pageData ? (WingDbId) pageData.WingRecord.ID : WingDbId.INVALID;
    AdventureModeDbId selectedMode = AdventureConfig.Get().GetSelectedMode();
    DungeonCrawlSubDef_VOLines.VOEventType eventType = DungeonCrawlSubDef_VOLines.VOEventType.INVALID;
    eventType = pageData == null || pageData.BookSection == 0 ? (GameUtils.IsModeHeroic(selectedMode) ? DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_ALL_WINGS_HEROIC : DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_ALL_WINGS) : (GameUtils.IsModeHeroic(selectedMode) ? DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_ALL_WINGS_SECOND_BOOK_SECTION_HEROIC : DungeonCrawlSubDef_VOLines.VOEventType.COMPLETE_ALL_WINGS_SECOND_BOOK_SECTION);
    while (NotificationManager.Get().IsQuotePlaying)
      yield return (object) null;
    DungeonCrawlSubDef_VOLines.PlayVOLine(AdventureConfig.Get().GetSelectedAdventure(), wingId, 0, eventType);
    while (!adventureBookPageDisplay.m_adventureNewlyCompletedSequenceFinished)
      yield return (object) null;
    adventureBookPageDisplay.m_adventureBookPageContentsWidget.RemoveEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.AdventureNewlyCompletedEventListener));
    if (UserAttentionManager.CanShowAttentionGrabber("AdventureBookPageDisplay.AnimateAdventureComplete"))
    {
      bool allPopupsShown = false;
      PopupDisplayManager.Get().ShowAnyOutstandingPopups((Action) (() => allPopupsShown = true));
      while (!allPopupsShown)
        yield return (object) null;
    }
    adventureBookPageDisplay.EnableInteraction(true);
  }

  private IEnumerator AnimateChapterRewardsAndCompletionIfNecessary()
  {
    AdventureBookPageDisplay adventureBookPageDisplay = this;
    if (adventureBookPageDisplay.m_needToShowMissionCompleteAnim)
    {
      adventureBookPageDisplay.EnableInteraction(false);
      adventureBookPageDisplay.m_missionCompleteAnimFinished = false;
      adventureBookPageDisplay.m_adventureBookPageContentsWidget.TriggerEvent("MISSION_NEWLY_COMPLETED");
      while (!adventureBookPageDisplay.m_missionCompleteAnimFinished)
        yield return (object) null;
      adventureBookPageDisplay.EnableInteraction(true);
    }
    if (adventureBookPageDisplay.m_needToShowRewardChestAnim)
    {
      adventureBookPageDisplay.EnableInteraction(false);
      adventureBookPageDisplay.m_rewardChestReadyToShowPopup = false;
      adventureBookPageDisplay.m_adventureBookPageContentsWidget.RegisterEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.RewardChestAnimEventListener));
      adventureBookPageDisplay.m_adventureBookPageContentsWidget.TriggerEvent("OPEN_CHAPTER_CHEST_REWARD");
      while (!adventureBookPageDisplay.m_rewardChestReadyToShowPopup)
        yield return (object) null;
      adventureBookPageDisplay.m_adventureBookPageContentsWidget.RemoveEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.RewardChestAnimEventListener));
      if (UserAttentionManager.CanShowAttentionGrabber("AdventureBookPageDisplay.AnimateChapterRewardsAndCompletionIfNecessary"))
      {
        bool allPopupsShown = false;
        if (AdventureScene.Get().IsDevMode)
          DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
          {
            m_headerText = "Dummy Reward Popup",
            m_text = "This is when the reward popup would be shown if you had actually earned it!",
            m_showAlertIcon = false,
            m_responseDisplay = AlertPopup.ResponseDisplay.OK,
            m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => allPopupsShown = true)
          });
        else
          PopupDisplayManager.Get().ShowAnyOutstandingPopups((Action) (() => allPopupsShown = true));
        while (!allPopupsShown)
          yield return (object) null;
      }
      adventureBookPageDisplay.EnableInteraction(true);
    }
    if (adventureBookPageDisplay.m_needToShowMissionCompleteAnim || adventureBookPageDisplay.m_needToShowRewardChestAnim)
    {
      foreach (AdventureMissionDataModel mission in adventureBookPageDisplay.m_pageDataModel.ChapterData.Missions)
        mission.NewlyCompleted = false;
    }
    if (adventureBookPageDisplay.m_needToShowMissionUnlockAnim)
    {
      adventureBookPageDisplay.EnableInteraction(false);
      adventureBookPageDisplay.m_missionUnlockAnimFinished = false;
      adventureBookPageDisplay.m_adventureBookPageContentsWidget.RegisterEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.MissionNewlyUnlockedAnimEventListener));
      adventureBookPageDisplay.m_adventureBookPageContentsWidget.TriggerEvent("MISSION_NEWLY_UNLOCKED");
      adventureBookPageDisplay.AckMissionUnlocksOnCurrentPage();
      while (!adventureBookPageDisplay.m_missionUnlockAnimFinished)
        yield return (object) null;
      adventureBookPageDisplay.m_adventureBookPageContentsWidget.RemoveEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.MissionNewlyUnlockedAnimEventListener));
      foreach (AdventureMissionDataModel mission in adventureBookPageDisplay.m_pageDataModel.ChapterData.Missions)
        mission.NewlyUnlocked = false;
      adventureBookPageDisplay.EnableInteraction(true);
    }
    if (adventureBookPageDisplay.m_needToShowChapterCompletionAnim)
    {
      adventureBookPageDisplay.EnableInteraction(false);
      adventureBookPageDisplay.m_chapterCompletionAnimFinished = false;
      adventureBookPageDisplay.m_adventureBookPageContentsWidget.RegisterEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.ChapterNewlyCompletedAnimEventListener));
      adventureBookPageDisplay.m_adventureBookPageContentsWidget.TriggerEvent("CHAPTER_NEWLY_COMPLETED");
      ChapterPageData chapterData = adventureBookPageDisplay.m_pageData as ChapterPageData;
      if (chapterData != null && chapterData.ChapterToFlipToWhenCompleted == 0)
        AdventureConfig.AckCurrentWingProgress(chapterData.WingRecord.ID);
      while (!adventureBookPageDisplay.m_chapterCompletionAnimFinished)
        yield return (object) null;
      adventureBookPageDisplay.m_adventureBookPageContentsWidget.RemoveEventListener(new Widget.EventListenerDelegate(adventureBookPageDisplay.ChapterNewlyCompletedAnimEventListener));
      if (GameUtils.GetNormalModeFromHeroicMode(AdventureConfig.Get().GetSelectedMode()) != AdventureModeDbId.DUNGEON_CRAWL)
      {
        adventureBookPageDisplay.PlayChapterCompleteVO();
        while (NotificationManager.Get().IsQuotePlaying)
          yield return (object) null;
      }
      AdventureDbfRecord adventureDbfRecord = chapterData == null ? (AdventureDbfRecord) null : GameDbf.Adventure.GetRecord((int) chapterData.Adventure);
      bool flag = adventureDbfRecord != null && adventureDbfRecord.MapPageHasButtonsToChapters;
      if (flag && AdventureConfig.Get().HasUnacknowledgedChapterUnlocks())
        AdventureBookPageManager.NavigateToMapPage();
      else if (AdventureBookPageDisplay.NeedToShowAdventureSectionCompletionSequence)
      {
        if (flag)
          AdventureBookPageManager.NavigateToMapPage();
        else
          adventureBookPageDisplay.StartCoroutine(adventureBookPageDisplay.AnimateAdventureSectionComplete());
      }
      else if (chapterData != null && chapterData.ChapterToFlipToWhenCompleted != 0)
      {
        if (adventureBookPageDisplay.m_flipToChapterCallback != null)
          adventureBookPageDisplay.m_flipToChapterCallback(chapterData.ChapterToFlipToWhenCompleted);
        adventureBookPageDisplay.EnableInteraction(true);
      }
      else
        adventureBookPageDisplay.EnableInteraction(true);
      chapterData = (ChapterPageData) null;
    }
  }

  private void AckMissionUnlocksOnCurrentPage()
  {
    HashSet<int> intSet = new HashSet<int>();
    foreach (AdventureMissionDataModel mission in this.m_pageDataModel.ChapterData.Missions)
    {
      if (mission.NewlyUnlocked)
      {
        int missionReqProgress = 0;
        int wingId = 0;
        if (AdventureConfig.GetMissionPlayableParameters((int) mission.ScenarioId, ref wingId, ref missionReqProgress))
          intSet.Add(wingId);
      }
    }
    foreach (int wingId in intSet)
      AdventureConfig.AckCurrentWingProgress(wingId);
  }

  private void PlayChapterCompleteVO()
  {
    WingDbId wingId = this.m_pageData is ChapterPageData pageData ? (WingDbId) pageData.WingRecord.ID : WingDbId.INVALID;
    AdventureWingDef wingDef = AdventureScene.Get().GetWingDef(wingId);
    if (!AdventureUtils.CanPlayWingCompleteQuote(wingDef))
      return;
    string legacyAssetName = new AssetReference(wingDef.m_CompleteQuoteVOLine).GetLegacyAssetName();
    NotificationManager.Get().CreateCharacterQuote(wingDef.m_CompleteQuotePrefab, GameStrings.Get(legacyAssetName), wingDef.m_CompleteQuoteVOLine, false);
  }

  private void BookPageContentsEventListener(string eventName)
  {
    switch (eventName)
    {
      case "BOSS_1_SELECTED":
        this.OnBossSelected(0);
        break;
      case "BOSS_2_SELECTED":
        this.OnBossSelected(1);
        break;
      case "BOSS_3_SELECTED":
        this.OnBossSelected(2);
        break;
      case "BOSS_4_SELECTED":
        this.OnBossSelected(3);
        break;
      case "BOSS_5_SELECTED":
        this.OnBossSelected(4);
        break;
      case "CHAPTER_UNLOCK_BUTTON_CLICKED":
        this.OnChapterUnlockButtonClicked();
        break;
      case "MISSION_NEWLY_COMPLETED_ANIM_FINISHED":
        this.m_missionCompleteAnimFinished = true;
        break;
    }
    if (this.m_pageEventListener != null)
      this.m_pageEventListener(eventName);
    if (!(this.m_pageData is ChapterPageData pageData))
      return;
    UIVoiceLinesManager.Get().ExecuteTrigger(UIVoiceLinesManager.UIVoiceLineCategory.ADVENTURE, UIVoiceLinesManager.TriggerType.BOSS_COIN_CLICKED, -1, eventName + "_" + (object) pageData.WingRecord.ID);
  }

  private void RewardChestAnimEventListener(string eventName)
  {
    if (!"READY_TO_SHOW_POPUP".Equals(eventName))
      return;
    this.m_rewardChestReadyToShowPopup = true;
  }

  private void ChapterNewlyCompletedAnimEventListener(string eventName)
  {
    if (!"CHAPTER_NEWLY_COMPLETED_ANIM_FINISHED".Equals(eventName))
      return;
    this.m_chapterCompletionAnimFinished = true;
  }

  private void AdventureNewlyCompletedEventListener(string eventName)
  {
    if ("PlayAdventureNewlyCompletedVO".Equals(eventName))
    {
      this.m_readyToPlayAdventureNewlyCompletedVO = true;
    }
    else
    {
      if (!"AdventureNewlyCompletedSequenceFinished".Equals(eventName))
        return;
      this.m_adventureNewlyCompletedSequenceFinished = true;
    }
  }

  private void MissionNewlyUnlockedAnimEventListener(string eventName)
  {
    if (!"MISSION_NEWLY_UNLOCKED_ANIM_FINISHED".Equals(eventName))
      return;
    this.m_missionUnlockAnimFinished = true;
  }

  private void ChapterNewlyPurchasedAnimEventListener(string eventName)
  {
    if (!"CHAPTER_NEWLY_PURCHASED_ANIM_FINISHED".Equals(eventName))
      return;
    this.m_chapterNewlyPurchasedAnimFinished = true;
  }

  private void EnableInteraction(bool enable)
  {
    if (this.m_enableInteractionCallback == null)
      return;
    this.m_enableInteractionCallback(enable);
  }

  private void UpdateRewardPageData(PageData pageData)
  {
    if (!(pageData is RewardPageData rewardPageData))
    {
      Debug.LogError((object) "UpdateRewardPageData(): PageData is not a valid RewardPageData! Cannot cast properly.");
    }
    else
    {
      this.m_pageDataModel.AllChaptersCompletedInCurrentSection = true;
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      foreach (ChapterPageData chapterPageData in rewardPageData.ChapterData.Values)
      {
        if (chapterPageData.BookSection == pageData.BookSection)
        {
          num2 += chapterPageData.ScenarioRecords.Count;
          foreach (ScenarioDbfRecord scenarioRecord in chapterPageData.ScenarioRecords)
          {
            bool flag = AdventureProgressMgr.Get().HasDefeatedScenario(scenarioRecord.ID);
            if (flag)
              ++num1;
            else
              this.m_pageDataModel.AllChaptersCompletedInCurrentSection = false;
            HashSet<Assets.Achieve.RewardTiming> rewardTimings = new HashSet<Assets.Achieve.RewardTiming>()
            {
              Assets.Achieve.RewardTiming.ADVENTURE_CHEST,
              Assets.Achieve.RewardTiming.IMMEDIATE
            };
            foreach (RewardData rewardData in AdventureProgressMgr.Get().GetRewardsForDefeatingScenario(scenarioRecord.ID, rewardTimings))
            {
              if (rewardData.RewardType == Reward.Type.CARD)
              {
                if (!(rewardData is CardRewardData cardRewardData))
                {
                  Debug.LogErrorFormat("AdventureBookPageDisplay.UpdateRewardPageData() - reward {0} is type CARD but is not a CardRewardData!", (object) rewardData);
                }
                else
                {
                  num4 += cardRewardData.Count;
                  if (flag)
                    num3 += cardRewardData.Count;
                }
              }
            }
          }
        }
      }
      this.m_pageDataModel.NumBossesDefeatedText = GameStrings.Format("GLUE_ADVENTURE_NUM_BOSSES_DEFEATED", (object) num1, (object) num2);
      this.m_pageDataModel.NumCardsCollectedText = GameStrings.Format("GLUE_ADVENTURE_NUM_CARDS_COLLECTED", (object) num3, (object) num4);
    }
    Reward.Type completionRewardType = AdventureConfig.Get().CompletionRewardType;
    switch (completionRewardType)
    {
      case Reward.Type.NONE:
        break;
      case Reward.Type.CARD_BACK:
        int completionRewardId = AdventureConfig.Get().CompletionRewardId;
        if (CardBackManager.Get().LoadCardBackByIndex(completionRewardId, new CardBackManager.LoadCardBackData.LoadCardBackCallback(this.OnCardBackLoaded)))
          break;
        Log.Adventures.PrintError("AdventureBookPageDisplay.SetCardBack() - failed to load CardBack {0}", (object) completionRewardId);
        break;
      default:
        Log.Adventures.PrintWarning("Unsupported reward type for Reward Page = {0}", (object) completionRewardType);
        break;
    }
  }

  private void OnCardBackLoaded(CardBackManager.LoadCardBackData cardbackData)
  {
    Actor componentInChildren = this.GetComponentInChildren<Actor>();
    if (!((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null))
      return;
    CardBackManager.SetCardBack(componentInChildren.m_cardMesh, cardbackData.m_CardBack);
    componentInChildren.SetCardbackUpdateIgnore(true);
  }

  private void CheckForInputForCheats()
  {
    if (!AdventureScene.Get().IsDevMode || !this.IsShown)
      return;
    if (this.m_pageData.PageType == AdventureBookPageType.CHAPTER)
    {
      AdventureChapterDataModel chapterData = this.m_pageDataModel.ChapterData;
      if (InputCollection.GetKeyDown(KeyCode.Z))
      {
        AdventureBookPageDisplay.NeedToShowAdventureSectionCompletionSequence = !AdventureBookPageDisplay.NeedToShowAdventureSectionCompletionSequence;
        if (AdventureBookPageDisplay.NeedToShowAdventureSectionCompletionSequence)
          this.m_needToShowChapterCompletionAnim = true;
        UIStatus.Get().AddInfo(string.Format("Adventure Completion anim {0} be played when you press Spacebar.", AdventureBookPageDisplay.NeedToShowAdventureSectionCompletionSequence ? (object) "WILL" : (object) "will NOT"));
        chapterData.NewlyCompleted = this.m_needToShowChapterCompletionAnim;
      }
      else if (InputCollection.GetKeyDown(KeyCode.V))
      {
        this.m_needToShowChapterCompletionAnim = !this.m_needToShowChapterCompletionAnim;
        UIStatus.Get().AddInfo(string.Format("Chapter Completion anim {0} be played when you press Spacebar.", this.m_needToShowChapterCompletionAnim ? (object) "WILL" : (object) "will NOT"));
        chapterData.NewlyCompleted = this.m_needToShowChapterCompletionAnim;
      }
      else if (InputCollection.GetKeyDown(KeyCode.C))
      {
        this.m_needToShowRewardChestAnim = !this.m_needToShowRewardChestAnim;
        UIStatus.Get().AddInfo(string.Format("Reward Chest anim {0} be played when you press Spacebar.", this.m_needToShowRewardChestAnim ? (object) "WILL" : (object) "will NOT"));
        chapterData.CompletionRewardsEarned = true;
        chapterData.CompletionRewardsNewlyEarned = this.m_needToShowRewardChestAnim;
        if (chapterData.Missions.Count > 0)
          chapterData.Missions[0].NewlyCompleted = this.m_needToShowRewardChestAnim;
        if (!this.m_needToShowRewardChestAnim || chapterData.ChapterState != AdventureChapterState.COMPLETED)
          return;
        chapterData.NewlyCompleted = true;
      }
      else
      {
        if (!InputCollection.GetKeyDown(KeyCode.Space))
          return;
        if (!this.m_needToShowChapterCompletionAnim && !this.m_needToShowRewardChestAnim)
        {
          UIStatus.Get().AddInfo("You attempted to play the reward sequence, but you have not enabled\nthe Reward Chest anim (key C) or Chapter Complete anim (key V).");
        }
        else
        {
          this.StopCoroutine(this.AnimateChapterRewardsAndCompletionIfNecessary());
          this.StartCoroutine(this.AnimateChapterRewardsAndCompletionIfNecessary());
        }
      }
    }
    else
    {
      if (this.m_pageData.PageType != AdventureBookPageType.MAP || !InputCollection.GetKeyDown(KeyCode.Z))
        return;
      AdventureDbfRecord record = GameDbf.Adventure.GetRecord((int) AdventureConfig.Get().GetSelectedAdventure());
      if (record == null || !record.MapPageHasButtonsToChapters)
        return;
      this.StopCoroutine(this.AnimateAdventureSectionComplete());
      this.StartCoroutine(this.AnimateAdventureSectionComplete());
    }
  }

  public delegate void PageReadyCallback();

  public delegate void FlipToChapterCallback(int chapterNumber);

  public delegate void EnableInteractionCallback(bool enable);

  private class ChapterButtonData
  {
    public string ButtonName;
    public ChapterPageData ChapterData;
  }
}
