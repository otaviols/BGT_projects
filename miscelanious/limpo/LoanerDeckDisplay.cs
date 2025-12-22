using Assets;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoanerDeckDisplay : MonoBehaviour
{
  private const string CLOSE_DECK_DETAILS = "CloseDetails";
  private const string CONFIRM_DECK_CHOICE = "ConfirmDeckChoice";
  private const string SHOW_DECK_DETAILS = "ShowLoanerDeckDetails";
  private const string OPEN_DECK_DETAILS_STATE = "OPEN";
  private const string CLOSE_DECK_DETAILS_STATE = "CLOSE";
  private const string TRIAL_EXPIRED_STATE = "SHOW_EXPIRED_NOTIFICATION";
  private const string CONFIRM_FREE_DECK_SELECTION = "CONFIRM_FREE_DECK_SELECTION";
  public int MaximumLoanerDecksToDisplay = 6;
  [Header("Widget References")]
  public AsyncReference LoanerDeckDetailsWidget;
  public List<AsyncReference> ClassButtonReferences;
  private static LoanerDeckDisplay m_instance;
  private FreeDeckMgr m_freeDeckManager;
  private Widget m_loanerDeckDetails;
  private LoanerDeckDetailsController m_loanerDeckDetailsController;
  private AbsDeckPickerTrayDisplay m_deckPickerTray;
  private DeckTemplateDbfRecord m_currentlySelectedDeck;
  private TimeSpan m_trialPeriodTimeLeft;

  public LoanerDecksInfoDataModel LoanerDeckInfoDataModel { get; set; }

  private void Awake()
  {
    LoanerDeckDisplay.m_instance = this;
    this.m_freeDeckManager = FreeDeckMgr.Get();
    this.m_deckPickerTray = (AbsDeckPickerTrayDisplay) DeckPickerTrayDisplay.Get();
    this.m_freeDeckManager.GetLoanerDecksAsMap();
    this.LoanerDeckInfoDataModel = new LoanerDecksInfoDataModel();
    if (!this.ShouldLoanerDecksBeDisplayed())
    {
      this.gameObject.SetActive(false);
    }
    else
    {
      this.LoanerDeckDetailsWidget.RegisterReadyListener<Widget>(new Action<Widget>(this.InitializeDeckDetailsWidget));
      this.m_deckPickerTray.AddDeckTrayLoadedListener(new AbsDeckPickerTrayDisplay.DeckTrayLoaded(this.RefreshDisplayOnTrayLoaded));
      SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.SetSceneModePropertyOnSceneLoad));
      DateTime? nullable1 = new DateTime?(DateTime.Now);
      DateTime? trialPeriodEndTime = this.m_freeDeckManager.TrialPeriodEndTime;
      if (trialPeriodEndTime.HasValue)
      {
        trialPeriodEndTime = this.m_freeDeckManager.TrialPeriodEndTime;
        DateTime? nullable2 = nullable1;
        this.m_trialPeriodTimeLeft = (trialPeriodEndTime.HasValue & nullable2.HasValue ? new TimeSpan?(trialPeriodEndTime.GetValueOrDefault() - nullable2.GetValueOrDefault()) : new TimeSpan?()).Value;
        this.LoanerDeckInfoDataModel.RemainingDeckTrialTime = TimeUtils.GetCountdownTimerString(this.m_trialPeriodTimeLeft, true);
      }
      this.StartCoroutine(this.TickDownEligibilityTimer());
    }
  }

  private void OnDestroy()
  {
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.ShowExpiredNotificationSceneLoad));
    SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.SetSceneModePropertyOnSceneLoad));
    this.StopCoroutine(this.TickDownEligibilityTimer());
    if ((bool) (UnityEngine.Object) this.m_deckPickerTray)
      this.m_deckPickerTray.RemoveDeckTrayLoadedListener(new AbsDeckPickerTrayDisplay.DeckTrayLoaded(this.RefreshDisplayOnTrayLoaded));
    this.LoanerDeckInfoDataModel = (LoanerDecksInfoDataModel) null;
  }

  public static LoanerDeckDisplay Get() => LoanerDeckDisplay.m_instance;

  public void SetSelectedDeckInDataModel(bool isLoaner)
  {
    if (this.LoanerDeckInfoDataModel == null)
      return;
    this.LoanerDeckInfoDataModel.IsSelectedDeckLoaner = isLoaner;
  }

  public void SetCurrentPageStatusInDataModel(bool isLoaner)
  {
    if (this.LoanerDeckInfoDataModel == null)
      return;
    this.LoanerDeckInfoDataModel.IsCurrentPageLoaner = isLoaner;
  }

  public void OpenDeckDetailsWidget(string eventName)
  {
    if (eventName != "ShowLoanerDeckDetails" || !((UnityEngine.Object) this.m_loanerDeckDetails != (UnityEngine.Object) null))
      return;
    this.m_loanerDeckDetails.TriggerEvent("OPEN");
  }

  public void HideDeckDetailsWidget(string eventName)
  {
    if (eventName != "CloseDetails" || !((UnityEngine.Object) this.m_loanerDeckDetails != (UnityEngine.Object) null))
      return;
    this.m_loanerDeckDetails.TriggerEvent("CLOSE");
  }

  public void ShowTrialTimerExpiredState()
  {
    if (!((UnityEngine.Object) this.m_loanerDeckDetails != (UnityEngine.Object) null))
      return;
    this.m_loanerDeckDetails.TriggerEvent("SHOW_EXPIRED_NOTIFICATION");
  }

  public void SetCurrentlySelectedDeckTemplate(DeckTemplateDbfRecord templateRecord)
  {
    if (templateRecord == null)
      return;
    this.m_currentlySelectedDeck = templateRecord;
  }

  public void ConfirmDeckSelection(string eventName)
  {
    if (eventName != "ConfirmDeckChoice")
      return;
    bool hasClaimedDeck = false;
    TAG_CLASS tag = TAG_CLASS.INVALID;
    if (this.m_currentlySelectedDeck != null)
      tag = (TAG_CLASS) this.m_currentlySelectedDeck.ClassId;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.RETURNING_PLAYER_EXPERIENCE, GameSaveKeySubkeyId.HAS_SEEN_LOANER_DECKS_ON_FIRST_LOGIN_TRIAL_START, new long[1]));
    if (hasClaimedDeck)
      return;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_FREE_DECK_CONFIRMATION_HEADER"),
      m_text = GameStrings.Format("GLUE_FREE_DECK_CONFIRMATION_TEXT", (object) GameStrings.GetClassName(tag)) + GameStrings.Format("GLUE_LOANER_DECK_CLAIM_CONFIRM"),
      m_showAlertIcon = false,
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_id = "CONFIRM_FREE_DECK_SELECTION",
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if (response != AlertPopup.Response.CONFIRM)
          return;
        Network.Get().SendFreeDeckChoice(this.LoanerDeckInfoDataModel.DeckChoiceTemplateId);
        hasClaimedDeck = true;
        RewardUtils.CreateDeckRewardData(0, this.m_currentlySelectedDeck.ClassId, (string) this.m_currentlySelectedDeck.DeckRecord.Name);
        DialogManager.Get().RemoveUniquePopupRequestFromQueue("CONFIRM_FREE_DECK_SELECTION");
        this.StartCoroutine(this.DelayAndGoToCollectionManager());
      })
    };
    DialogManager.Get().ShowUniquePopup(info);
  }

  public void OnLoanerDeckTimeExpired() => this.OpenDeckDetailsWidget("ShowLoanerDeckDetails");

  public bool ShouldLoanerDecksBeDisplayed()
  {
    FreeDeckMgr.FreeDeckStatus status = this.m_freeDeckManager.Status;
    int num = status == FreeDeckMgr.FreeDeckStatus.AVAILABLE ? 1 : (status == FreeDeckMgr.FreeDeckStatus.TRIAL_PERIOD ? 1 : 0);
    bool flag1 = (UnityEngine.Object) this.m_deckPickerTray != (UnityEngine.Object) null && !this.m_deckPickerTray.IsChoosingHero();
    bool flag2 = SceneMgr.Get().GetMode() != SceneMgr.Mode.FRIENDLY;
    return num != 0 && flag1 && flag2 && this.m_freeDeckManager.GetLoanerDecksCount() != 0;
  }

  private IEnumerator TickDownEligibilityTimer()
  {
    LoanerDeckDisplay loanerDeckDisplay = this;
    while (loanerDeckDisplay.LoanerDeckInfoDataModel != null)
    {
      yield return (object) new WaitForSeconds(1f);
      loanerDeckDisplay.m_trialPeriodTimeLeft = loanerDeckDisplay.m_trialPeriodTimeLeft.Subtract(new TimeSpan(0, 0, 1));
      if (loanerDeckDisplay.m_trialPeriodTimeLeft.TotalSeconds <= 0.0)
        loanerDeckDisplay.m_trialPeriodTimeLeft = new TimeSpan(0, 0, 0);
      loanerDeckDisplay.LoanerDeckInfoDataModel.RemainingDeckTrialTime = TimeUtils.GetCountdownTimerString(loanerDeckDisplay.m_trialPeriodTimeLeft, true);
      if (loanerDeckDisplay.m_trialPeriodTimeLeft.TotalSeconds <= 0.0)
      {
        GameMgr gameMgr = GameMgr.Get();
        if (gameMgr.IsFindingGame())
          gameMgr.CancelFindGame();
        loanerDeckDisplay.LoanerDeckInfoDataModel.IsLoanerDeckAvailable = true;
        loanerDeckDisplay.ShowTrialTimerExpiredState();
        loanerDeckDisplay.StopCoroutine(loanerDeckDisplay.TickDownEligibilityTimer());
        break;
      }
    }
  }

  private void InitializeDeckDetailsWidget(Widget widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
    {
      Log.Decks.PrintWarning("No DeckDetails widget available");
    }
    else
    {
      this.m_loanerDeckDetailsController = widget.GetComponentInChildren<LoanerDeckDetailsController>();
      if ((UnityEngine.Object) this.m_loanerDeckDetailsController != (UnityEngine.Object) null)
        this.InitializeDeckSelectorButtons();
      this.LoanerDeckInfoDataModel.DeckChoiceTemplateId = 0;
      if (FreeDeckMgr.Get().IsLoanerDeckAvailableToClaim())
      {
        this.LoanerDeckInfoDataModel.IsLoanerDeckAvailable = true;
        if (SceneMgr.Get().IsSceneLoaded())
          this.ShowTrialTimerExpiredState();
        else
          SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.ShowExpiredNotificationSceneLoad));
      }
      else
        this.LoanerDeckInfoDataModel.IsLoanerDeckAvailable = false;
      this.m_loanerDeckDetails = widget;
      this.LoanerDeckInfoDataModel.DeckChoiceTemplateId = 0;
      widget.RegisterEventListener(new Widget.EventListenerDelegate(this.ConfirmDeckSelection));
      widget.BindDataModel((IDataModel) this.LoanerDeckInfoDataModel);
    }
  }

  private void InitializeDeckSelectorButtons()
  {
    Dictionary<int, CollectionDeck> collectionDeckMap = FreeDeckMgr.Get().GetLoanerDecksAsMap();
    Dictionary<int, DeckTemplateDbfRecord> loanerDeckTemplateMap = FreeDeckMgr.Get().GetLoanerDeckTemplateMap();
    if (this.ClassButtonReferences.Count < loanerDeckTemplateMap.Count)
    {
      Log.Decks.PrintError("Not enough button widgets for available decks");
    }
    else
    {
      int index1 = 0;
      LoanerDeckSelectButton defaultDeckSelectButton = (LoanerDeckSelectButton) null;
      foreach (KeyValuePair<int, DeckTemplateDbfRecord> keyValuePair in loanerDeckTemplateMap)
      {
        KeyValuePair<int, DeckTemplateDbfRecord> record = keyValuePair;
        TAG_CLASS classAsTag = (TAG_CLASS) record.Value.ClassId;
        CollectionManager.GetHeroCardId(classAsTag, CardHero.HeroType.VANILLA);
        this.ClassButtonReferences[index1].RegisterReadyListener<Widget>((Action<Widget>) (widget =>
        {
          widget.BindDataModel((IDataModel) new DeckChoiceDataModel()
          {
            ButtonClass = classAsTag.ToString()
          });
          LoanerDeckSelectButton componentInChildren = widget.GetComponentInChildren<LoanerDeckSelectButton>();
          if ((UnityEngine.Object) componentInChildren == (UnityEngine.Object) null)
          {
            Log.Decks.PrintError("Could not find LoanerDeckSelectButton for deck Selection button");
            widget.Hide();
          }
          else
          {
            componentInChildren.DeckTemplateRecord = record.Value;
            componentInChildren.DeckDetailsController = this.m_loanerDeckDetailsController;
            componentInChildren.DataModel = this.LoanerDeckInfoDataModel;
            componentInChildren.SetDeckSelectButtonIcon(collectionDeckMap[record.Key]);
            widget.RegisterEventListener(new Widget.EventListenerDelegate(componentInChildren.OnDeckChoiceButtonClicked));
            if (!((UnityEngine.Object) defaultDeckSelectButton == (UnityEngine.Object) null))
              return;
            defaultDeckSelectButton = componentInChildren;
            defaultDeckSelectButton.OnDeckChoiceButtonClicked("Selected");
          }
        }));
        ++index1;
      }
      for (int index2 = index1; index2 < this.ClassButtonReferences.Count; ++index2)
        this.ClassButtonReferences[index2].RegisterReadyListener<Widget>((Action<Widget>) (widget => widget.gameObject.SetActive(false)));
    }
  }

  private void ShowExpiredNotificationSceneLoad(
    SceneMgr.Mode mode,
    PegasusScene scene,
    object userData)
  {
    if (mode != SceneMgr.Mode.TOURNAMENT || !GameUtils.IsAnyTutorialComplete())
      return;
    this.ShowTrialTimerExpiredState();
  }

  private void SetSceneModePropertyOnSceneLoad(
    SceneMgr.Mode mode,
    PegasusScene scene,
    object userData)
  {
    this.LoanerDeckInfoDataModel.CurrentSceneMode = mode.ToString();
  }

  private void RefreshDisplayOnTrayLoaded()
  {
    if (this.ShouldLoanerDecksBeDisplayed())
      this.gameObject.SetActive(true);
    else
      this.gameObject.SetActive(false);
  }

  private IEnumerator DelayAndGoToCollectionManager()
  {
    yield return (object) new WaitForSeconds(0.5f);
    this.HideDeckDetailsWidget("CloseDetails");
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
  }
}
