using Assets;
using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusLettuce;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LettuceVillageTrainingHall : MonoBehaviour
{
  public const string SHOW_MERC_PORTRAIT_WHEN_DRAGGING_EVENT = "HOLD_MERC_OVER_TRAINING_code";
  public const string SHOW_MERC_TILE_WHEN_DRAGGING_EVENT = "HOLD_MERC_OVER_TEAM_TRAY_code";
  public const string MERC_DROPPED_IN_SLOT_1_EVENT = "MERC_DROPPED_SLOT_1";
  public const string MERC_DROPPED_IN_SLOT_2_EVENT = "MERC_DROPPED_SLOT_2";
  public const string MERC_DROPPED_IN_LIST_EVENT = "MERC_DROPPED_LIST";
  public const string ENABLE_INPUT_EVENT = "UNBLOCK_SCREEN";
  public const string DISABLE_INPUT_EVENT = "BLOCK_SCREEN";
  public const string MAKE_VISIBLE = "MAKE_VISIBLE";
  public const string MAKE_HIDDEN = "MAKE_HIDDEN";
  public const string MERC_LOADOUT_RELEASED = "MERC_LOADOUT_RELEASED";
  public const string TEAM_MERC_drag_started = "TEAM_MERC_drag_started";
  public const string SLOT_BUTTON_PRESSED = "SLOT_BUTTON_PRESSED";
  public const int FALLBACK_MAX_LEVEL = 30;
  public const float TIME_UNTIL_SERVER_TIMEOUT = 30f;
  public const int MAX_MERCS_TO_LOAD_AT_ONCE = 10;
  public const float TIME_TO_LOAD_NEXT_MERC_BATCH = 0.1f;
  public AsyncReference m_listVCReference;
  public AsyncReference m_draggableReference;
  public AsyncReference m_searchReference;
  public AsyncReference m_trainingWindowReference;
  public AsyncReference m_screenBlockerReference;
  public AsyncReference m_trainingSlot1;
  public AsyncReference m_trainingSlot2;
  [SerializeField]
  private Collider m_dragPlaneCollider;
  [SerializeField]
  private Collider m_dropZoneCollider;
  [SerializeField]
  private GameObject m_expRewardContainer;
  [SerializeField]
  private float m_progressUpdateFrequencyInSeconds = 60f;
  private Widget m_widget;
  private Widget m_trainingWindowWidget;
  private Widget m_screenBlocker;
  private LettuceTrainingHallPopupDataModel m_dataModel;
  private UIBScrollable m_scrollbar;
  private VisualController m_listVC;
  private Listable m_mercListable;
  private Widget m_mercenariesDraggablesWidget;
  private Vector3 m_offScreenPosition;
  public LettuceVillageTrainingHall.OnCardDroppedCallback m_cardDroppedCallback;
  private GameObject m_dragColliderRoot;
  private LettuceMercenaryDataModel m_draggedMerc;
  private bool m_draggedMercIsMaxLevel;
  private string m_searchText;
  private float m_responseTimeout;
  private float m_timeToUpdateProgress;
  private bool m_processingUpdate;
  private bool m_didInitialLoad;
  private bool m_isShowingMercenariesExperienceRewards;
  private bool m_draggedMercIsOverTrainingArea;
  private LettuceVillageTrainingHall.TrainingMetrics m_trainingMetrics;
  private int m_lastSlotInteracted;
  private long m_collectedMercInitialExp = -1;
  private List<long> m_GSDMercenaries = new List<long>()
  {
    0L,
    0L
  };
  private Dictionary<int, LettuceMercenaryDataModel> m_mercenaryDMCache = new Dictionary<int, LettuceMercenaryDataModel>();
  private List<int> m_disabledMercenaries;
  private List<LettuceMercenary> m_mercenariesToLoad;

  private void Start()
  {
    this.m_dragColliderRoot = this.m_dragPlaneCollider.gameObject;
    this.m_widget = this.GetComponent<Widget>();
    this.m_dragColliderRoot.SetActive(false);
    this.m_widget.RegisterEventListener(new Widget.EventListenerDelegate(this.HandleEvent));
    this.ValidateGSDState();
    this.GenerateTrainingMetrics();
    this.SetUpDataModel();
    this.m_listVCReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnListSubwidgetReady));
    this.m_draggableReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnMercenariesDraggablesReady));
    this.m_searchReference.RegisterReadyListener<CollectionSearch>(new Action<CollectionSearch>(this.OnSearchReady));
    this.m_trainingWindowReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnTrainingWindowReady));
    this.m_screenBlockerReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnScreenBlockerReady));
    this.m_trainingSlot1.RegisterReadyListener<Widget>(new Action<Widget>(this.OnTrainingSlot1Ready));
    this.m_trainingSlot2.RegisterReadyListener<Widget>(new Action<Widget>(this.OnTrainingSlot2Ready));
    CollectionManager.Get().OnMercenariesTrainingAddResponseReceived += new Action(this.OnMercenariesTrainingAddResponse);
    CollectionManager.Get().OnMercenariesTrainingRemoveResponseReceived += new Action(this.OnMercenariesTrainingRemoveResponse);
    CollectionManager.Get().OnMercenariesTrainingCollectResponseReceived += new Action(this.OnMercenariesTrainingCollectResponse);
  }

  private void HandleEvent(string eventName)
  {
    if (!(eventName == "MAKE_VISIBLE"))
    {
      if (!(eventName == "MAKE_HIDDEN"))
      {
        if (!(eventName == "MERC_LOADOUT_RELEASED"))
        {
          if (!(eventName == "TEAM_MERC_drag_started"))
          {
            if (!(eventName == "SLOT_BUTTON_PRESSED"))
              return;
            EventDataModel dataModel = this.m_widget.GetDataModel<EventDataModel>();
            if (!(dataModel.Payload is IConvertible))
              return;
            this.OnSlotButtonPressed(Convert.ToInt32(dataModel.Payload));
          }
          else
            this.OnMercDragStarted();
        }
        else
          this.MercDropped();
      }
      else
        this.OnHidden();
    }
    else
      this.OnShown();
  }

  private void OnShown()
  {
    this.m_dataModel.IsPopupVisible = true;
    this.GenerateTrainingMetrics();
    this.AddTrainingDataToDataModel();
    this.UpdateDataModelForSlot();
    this.UpdateDataModelForSlot(1);
    this.PopulateMercList();
    if (!((UnityEngine.Object) LettuceVillagePopupManager.Get() != (UnityEngine.Object) null) || !((UnityEngine.Object) LettuceVillagePopupManager.Get().GetTutorialGameObject() != (UnityEngine.Object) null))
      return;
    LettuceTutorialUtils.FireEvent(LettuceTutorialVo.LettuceTutorialEvent.VILLAGE_BUILDING_START_TRAINING, LettuceVillagePopupManager.Get().GetTutorialGameObject());
  }

  private void OnHidden()
  {
    this.m_dataModel.IsPopupVisible = false;
    this.m_responseTimeout = 0.0f;
    this.m_timeToUpdateProgress = 0.0f;
    if (!this.m_dataModel.Slot2.Locked)
      this.SetTrainingHallFlag(LettuceVillageTrainingHall.TrainingHallFlags.SLOT_2_DONE_SHOWING_NEW_DECORATION, true);
    Action<LettuceVillagePopupManager.PopupType> onPopupClosed = LettuceVillagePopupManager.Get().OnPopupClosed;
    if (onPopupClosed == null)
      return;
    onPopupClosed(LettuceVillagePopupManager.PopupType.TRAININGHALL);
  }

  private void Update()
  {
    if ((double) this.m_responseTimeout > 0.0 && (double) Time.time > (double) this.m_responseTimeout)
    {
      this.m_responseTimeout = 0.0f;
      this.m_processingUpdate = false;
      PopupDisplayManager.SuppressPopupsTemporarily = false;
      this.m_dataModel.ErrorText = GameStrings.Get("GLUE_LETTUCE_VILLAGE_TIMEOUT_ERROR");
    }
    if ((double) Time.time > (double) this.m_timeToUpdateProgress && (double) this.m_timeToUpdateProgress > 0.0)
    {
      this.UpdateDataModelForSlot(progressOnly: true);
      this.UpdateDataModelForSlot(1, true);
    }
    LettuceTrainingHallPopupDataModel dataModel = this.m_dataModel;
    if ((dataModel != null ? (dataModel.IsPlayerDragging ? 1 : 0) : 0) == 0)
      return;
    this.UpdateDraggedMerc();
  }

  private void SetUpDataModel()
  {
    this.m_dataModel = new LettuceTrainingHallPopupDataModel();
    this.m_dataModel.Slot1 = new LettuceTrainingHallSlotDataModel();
    this.m_dataModel.Slot2 = new LettuceTrainingHallSlotDataModel();
    this.AddTrainingDataToDataModel();
    this.UpdateDataModelForSlot();
    this.UpdateDataModelForSlot(1);
    this.m_dataModel.IsPopupVisible = false;
    this.m_widget.BindDataModel((IDataModel) this.m_dataModel);
  }

  private void UpdateDataModelForSlot(
    int slot = 0,
    bool progressOnly = false,
    bool checkMercExp = false,
    long xpAwarded = 0)
  {
    if (this.m_dataModel == null)
      return;
    LettuceTrainingHallSlotDataModel hallSlotDataModel = slot == 0 ? this.m_dataModel.Slot1 : this.m_dataModel.Slot2;
    hallSlotDataModel.Locked = this.m_trainingMetrics.numSlotsAvailable <= slot;
    hallSlotDataModel.SlotIndex = slot;
    hallSlotDataModel.MaxExp = this.m_trainingMetrics.maxExpGained;
    hallSlotDataModel.PreparationTime = this.m_trainingMetrics.minTrainingTime;
    hallSlotDataModel.IsNewlyUnlocked = slot == 1 && !hallSlotDataModel.Locked && !this.GetTrainingHallFlag(LettuceVillageTrainingHall.TrainingHallFlags.SLOT_2_DONE_SHOWING_NEW_DECORATION);
    LettuceMercenary mercenaryInSlot = this.GetMercenaryInSlot(slot);
    if (mercenaryInSlot == null)
    {
      hallSlotDataModel.SlotIsEmpty = true;
      hallSlotDataModel.ShowAnimatedTraining = false;
      hallSlotDataModel.TrainingIsComplete = false;
      hallSlotDataModel.TotalTimeInTraining = 0;
      hallSlotDataModel.MercIsMaxLevel = false;
      hallSlotDataModel.Mercenary = (LettuceMercenaryDataModel) null;
      hallSlotDataModel.Progress = 0;
    }
    else
    {
      Date trainingStartDate = mercenaryInSlot.m_trainingStartDate;
      hallSlotDataModel.SlotIsEmpty = false;
      if (!progressOnly)
      {
        LettuceMercenaryDataModel dataModelForMerc = this.GetDataModelForMerc(mercenaryInSlot);
        if (dataModelForMerc != null)
        {
          dataModelForMerc.ChildUpgradeAvailable = false;
          if (checkMercExp && this.m_collectedMercInitialExp != -1L)
          {
            int experience = Mathf.Max((int) this.m_collectedMercInitialExp + (int) xpAwarded, (int) mercenaryInSlot.m_experience);
            int levelFromExperience = GameUtils.GetMercenaryLevelFromExperience(experience);
            CollectionUtils.SetMercenaryStatsByLevel(dataModelForMerc, mercenaryInSlot.ID, levelFromExperience, mercenaryInSlot.m_isFullyUpgraded);
            dataModelForMerc.MercenaryLevel = levelFromExperience;
            dataModelForMerc.ExperienceInitial = experience;
          }
        }
        hallSlotDataModel.Mercenary = dataModelForMerc;
      }
      int gainedFromTimestamp = LettuceVillageDataUtil.CalculateExpGainedFromTimestamp(trainingStartDate, this.m_trainingMetrics.expPerHour);
      hallSlotDataModel.Progress = gainedFromTimestamp;
      hallSlotDataModel.MercIsMaxLevel = mercenaryInSlot.IsMaxLevel();
      hallSlotDataModel.TrainingIsComplete = gainedFromTimestamp >= this.m_trainingMetrics.maxExpGained;
      hallSlotDataModel.TotalTimeInTraining = LettuceVillageDataUtil.GetTimeTrainingInSeconds(trainingStartDate);
      if (!this.IsMercenaryFinishedPreparing(trainingStartDate))
      {
        int totalSeconds = (int) this.GetTimeLeftPreparing(trainingStartDate).TotalSeconds;
        if ((double) totalSeconds > 90.0)
          hallSlotDataModel.PreparationText = GameStrings.Format("GLUE_TRAINING_HALL_PREP_TIME_MIN", (object) ((totalSeconds + 30) / 60));
        else
          hallSlotDataModel.PreparationText = GameStrings.Format("GLUE_TRAINING_HALL_PREP_TIME_SEC", (object) totalSeconds);
        hallSlotDataModel.ShowAnimatedTraining = false;
      }
      else
      {
        hallSlotDataModel.PreparationText = (string) null;
        hallSlotDataModel.ShowAnimatedTraining = !hallSlotDataModel.TrainingIsComplete && !hallSlotDataModel.MercIsMaxLevel;
      }
      this.m_timeToUpdateProgress = Time.time + this.m_progressUpdateFrequencyInSeconds;
    }
  }

  private void OnTrainingWindowReady(Widget widget)
  {
    this.m_trainingWindowWidget = widget;
    this.m_trainingWindowWidget.WillLoadSynchronously = true;
  }

  private bool GetTrainingHallFlag(LettuceVillageTrainingHall.TrainingHallFlags flag)
  {
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARY_TRAINING_GROUND_FLAGS, out num);
    return ((LettuceVillageTrainingHall.TrainingHallFlags) num & flag) == flag;
  }

  private void SetTrainingHallFlag(
    LettuceVillageTrainingHall.TrainingHallFlags flagsToSet,
    bool valueToSet)
  {
    long num1 = (long) flagsToSet;
    GameSaveDataManager gameSaveDataManager = GameSaveDataManager.Get();
    long num2;
    gameSaveDataManager.GetSubkeyValue(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARY_TRAINING_GROUND_FLAGS, out num2);
    long num3 = !valueToSet ? num2 & ~num1 : num2 | num1;
    if (num3 == num2)
      return;
    gameSaveDataManager.SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARY_TRAINING_GROUND_FLAGS, new long[1]
    {
      num3
    }));
  }

  private void OnSlotButtonPressed(int slot = 0)
  {
    LettuceMercenary mercenaryInSlot = this.GetMercenaryInSlot(slot);
    if (mercenaryInSlot == null)
      Debug.LogWarning((object) "LettuceVillageTrainingHall.OnSlotButtonPressed - button was pressed when no mercenary was present");
    else if (mercenaryInSlot.IsMaxLevel())
      this.RemoveMercenaryFromTraining(slot);
    else if (this.IsMercenaryFinishedPreparing(mercenaryInSlot.m_trainingStartDate))
      this.CollectMercenaryFromTraining(slot);
    else
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_VILLAGE_REMOVE_MERCENARY_TRAINING"),
        m_text = GameStrings.Get("GLUE_LETTUCE_VILLAGE_REMOVE_MERCENARY_TRAINING_DESCRIPTION"),
        m_showAlertIcon = false,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
        {
          if (response != AlertPopup.Response.CONFIRM)
            return;
          this.RemoveMercenaryFromTraining(slot);
        })
      });
  }

  public void PlaceMercenaryInTraining(int mercId, int slot = 0)
  {
    if (this.m_processingUpdate)
      return;
    Network.Get().MercenariesTrainingAddRequest(mercId);
    this.m_lastSlotInteracted = slot;
    this.m_processingUpdate = true;
    this.m_responseTimeout = Time.time + 30f;
    LettuceVillagePopupManager.Get().HideHelpPopupsInVillage();
  }

  public void RemoveMercenaryFromTraining(int slot = 0)
  {
    if (this.m_processingUpdate)
      return;
    LettuceMercenary mercenaryInSlot = this.GetMercenaryInSlot(slot);
    if (mercenaryInSlot == null)
      return;
    Network.Get().MercenariesTrainingRemoveRequest(mercenaryInSlot.ID);
    this.m_lastSlotInteracted = slot;
    this.m_processingUpdate = true;
    this.m_responseTimeout = Time.time + 30f;
  }

  public void CollectMercenaryFromTraining(int slot = 0)
  {
    if (this.m_processingUpdate)
      return;
    LettuceMercenary mercenaryInSlot = this.GetMercenaryInSlot(slot);
    if (mercenaryInSlot == null)
      return;
    Network.Get().MercenariesTrainingCollectRequest(mercenaryInSlot.ID);
    this.m_lastSlotInteracted = slot;
    this.m_collectedMercInitialExp = mercenaryInSlot.m_experience;
    PopupDisplayManager.SuppressPopupsTemporarily = true;
    this.m_processingUpdate = true;
    this.m_responseTimeout = Time.time + 30f;
  }

  private void ShowSimpleError(string message) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLUE_COLLECTION_ERROR_HEADER"),
    m_text = GameStrings.Get(message),
    m_showAlertIcon = true,
    m_responseDisplay = AlertPopup.ResponseDisplay.OK
  });

  private void OnMercenariesTrainingAddResponse()
  {
    this.m_processingUpdate = false;
    this.m_responseTimeout = 0.0f;
    MercenariesTrainingAddResponse trainingAddResponse = Network.Get().MercenariesTrainingAddResponse();
    if (trainingAddResponse != null && trainingAddResponse.Success)
    {
      this.SetMercenaryInSlotGSD(trainingAddResponse.MercenaryId, this.m_lastSlotInteracted);
      if (this.m_lastSlotInteracted == 1)
        this.SetTrainingHallFlag(LettuceVillageTrainingHall.TrainingHallFlags.SLOT_2_DONE_SHOWING_NEW_DECORATION, true);
      this.UpdateDataModelForSlot(this.m_lastSlotInteracted);
    }
    else
      this.ShowSimpleError("GLUE_COLLECTION_GENERIC_ERROR");
    this.PopulateMercList();
  }

  private void OnMercenariesTrainingRemoveResponse()
  {
    this.m_processingUpdate = false;
    this.m_responseTimeout = 0.0f;
    MercenariesTrainingRemoveResponse trainingRemoveResponse = Network.Get().MercenariesTrainingRemoveResponse();
    if (trainingRemoveResponse == null || !trainingRemoveResponse.Success)
    {
      this.ShowSimpleError("GLUE_COLLECTION_GENERIC_ERROR");
    }
    else
    {
      this.SetMercenaryInSlotGSD(slot: this.m_lastSlotInteracted);
      this.UpdateDataModelForSlot(this.m_lastSlotInteracted);
      this.PopulateMercList();
    }
  }

  private void OnMercenariesTrainingCollectResponse()
  {
    this.m_processingUpdate = false;
    this.m_responseTimeout = 0.0f;
    MercenariesTrainingCollectResponse trainingCollectResponse = Network.Get().MercenariesTrainingCollectResponse();
    if (trainingCollectResponse == null || !trainingCollectResponse.Success)
    {
      this.ShowSimpleError("GLUE_COLLECTION_GENERIC_ERROR");
    }
    else
    {
      this.UpdateDataModelForSlot(this.m_lastSlotInteracted, checkMercExp: true, xpAwarded: trainingCollectResponse.XpAwarded);
      this.ShowMercenaryExperienceReward(trainingCollectResponse.MercenaryId, trainingCollectResponse.XpAwarded);
      this.m_collectedMercInitialExp = -1L;
      this.PopulateMercList();
    }
  }

  protected bool ShowMercenaryExperienceReward(int mercenaryId, long expGained)
  {
    if (this.m_isShowingMercenariesExperienceRewards)
      return true;
    if ((UnityEngine.Object) this.m_expRewardContainer == (UnityEngine.Object) null)
      return false;
    List<MercenaryExpRewardData> mercenaryExpRewards = new List<MercenaryExpRewardData>();
    mercenaryExpRewards.Add(new MercenaryExpRewardData(mercenaryId, (int) this.m_collectedMercInitialExp, (int) (this.m_collectedMercInitialExp + expGained), (int) expGained));
    MercenariesTrainingHallExpRewardPopup component = this.m_expRewardContainer.GetComponent<MercenariesTrainingHallExpRewardPopup>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
    {
      Log.Lettuce.PrintError("MercenariesExperienceTrainingHall game object had no script attached!");
      this.m_isShowingMercenariesExperienceRewards = false;
      return false;
    }
    this.m_expRewardContainer.SetActive(true);
    component.Initialize(mercenaryExpRewards, new Action(this.OnMercenariesExperienceRewardReady), new Action(this.OnMercenariesExperienceRewardClosed));
    return true;
  }

  private void OnMercenariesExperienceRewardReady()
  {
    UIContext.GetRoot().ShowPopup(this.m_expRewardContainer);
    this.m_isShowingMercenariesExperienceRewards = true;
  }

  private void OnMercenariesExperienceRewardClosed()
  {
    this.m_isShowingMercenariesExperienceRewards = false;
    UIContext.GetRoot().DismissPopup(this.m_expRewardContainer);
    this.m_expRewardContainer.GetComponent<MercenariesTrainingHallExpRewardPopup>();
    this.m_expRewardContainer.SetActive(false);
    this.ShowNextAbilityRewardUntilAllShown((Action) (() => PopupDisplayManager.SuppressPopupsTemporarily = false));
  }

  private void ShowNextAbilityRewardUntilAllShown(Action doneCallback = null)
  {
    NetCache.ProfileNoticeMercenariesAbilityUnlock abilityUnlockReward = PopupDisplayManager.Get().RewardPopups.GetNextMercenariesAbilityUnlockReward();
    if (abilityUnlockReward == null)
    {
      if (doneCallback == null)
        return;
      doneCallback();
    }
    else
      PopupDisplayManager.Get().RewardPopups.ShowNextMercenariesAbilityUnlockReward(abilityUnlockReward, (Action) (() => this.ShowNextAbilityRewardUntilAllShown()));
  }

  public bool IsProcessingUpdate() => this.m_processingUpdate;

  private void OnTrainingSlot1Ready(Widget slotWidget) => slotWidget.BindDataModel((IDataModel) this.m_dataModel.Slot1);

  private void OnTrainingSlot2Ready(Widget slotWidget) => slotWidget.BindDataModel((IDataModel) this.m_dataModel.Slot2);

  private void OnScreenBlockerReady(Widget widget) => this.m_screenBlocker = widget;

  private void EnableInput(bool value) => this.m_screenBlocker.TriggerEvent(value ? "UNBLOCK_SCREEN" : "BLOCK_SCREEN");

  private void AddTrainingDataToDataModel()
  {
    BuildingTierDbfRecord recordFromBuilding = LettuceVillageDataUtil.GetCurrentTierRecordFromBuilding(MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL);
    if (recordFromBuilding == null || this.m_dataModel == null)
      return;
    this.m_dataModel.MaxTrainingHours = this.m_trainingMetrics.expPerHour <= 0 ? 0 : this.m_trainingMetrics.maxExpGained / this.m_trainingMetrics.expPerHour;
    this.m_dataModel.TrainingHallLevel = recordFromBuilding.ID;
  }

  private void GenerateTrainingMetrics()
  {
    BuildingTierDbfRecord recordFromBuilding = LettuceVillageDataUtil.GetCurrentTierRecordFromBuilding(MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL);
    this.m_trainingMetrics = new LettuceVillageTrainingHall.TrainingMetrics();
    this.m_trainingMetrics.expPerHour = LettuceVillageDataUtil.GetCurrentTierPropertyForBuilding(MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL, TierProperties.Buildingtierproperty.TRAININGXPPERHOUR, recordFromBuilding);
    this.m_trainingMetrics.maxExpGained = LettuceVillageDataUtil.GetCurrentTierPropertyForBuilding(MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL, TierProperties.Buildingtierproperty.TRAININGXPPOOLSIZE, recordFromBuilding);
    this.m_trainingMetrics.numSlotsAvailable = LettuceVillageDataUtil.GetCurrentTierPropertyForBuilding(MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL, TierProperties.Buildingtierproperty.TRAININGSLOTS, recordFromBuilding);
    this.m_trainingMetrics.minTrainingTime = LettuceVillageDataUtil.GetCurrentTierPropertyForBuilding(MercenaryBuilding.Mercenarybuildingtype.TRAININGHALL, TierProperties.Buildingtierproperty.TRAININGMINSECONDS, recordFromBuilding);
  }

  private TimeSpan GetTimeLeftPreparing(Date startDate) => startDate == null ? new TimeSpan() : new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hours, startDate.Min, startDate.Sec).AddSeconds((double) this.m_trainingMetrics.minTrainingTime) - DateTime.UtcNow;

  private bool IsMercenaryFinishedPreparing(Date startDate) => startDate != null && (DateTime.UtcNow - new DateTime(startDate.Year, startDate.Month, startDate.Day, startDate.Hours, startDate.Min, startDate.Sec)).TotalSeconds > (double) this.m_trainingMetrics.minTrainingTime;

  private LettuceMercenary GetMercenaryInSlot(int slot = 0)
  {
    LettuceMercenary mercenaryInSlot = (LettuceMercenary) null;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_TRAINING_SLOTS, this.m_GSDMercenaries);
    int num1 = slot;
    List<long> gsdMercenaries = this.m_GSDMercenaries;
    // ISSUE: explicit non-virtual call
    int num2 = gsdMercenaries != null ? __nonvirtual (gsdMercenaries.Count) : 0;
    if (num1 < num2 && this.m_GSDMercenaries[slot] > 0L)
      mercenaryInSlot = CollectionManager.Get().GetMercenary(this.m_GSDMercenaries[slot]);
    return mercenaryInSlot;
  }

  private bool SetMercenaryInSlotGSD(int mercID = 0, int slot = 0)
  {
    if (slot < 0 || slot > 1)
      return false;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_TRAINING_SLOTS, this.m_GSDMercenaries);
    List<long> longList = this.m_GSDMercenaries;
    if (longList == null)
      longList = new List<long>() { 0L, 0L };
    this.m_GSDMercenaries = longList;
    this.m_GSDMercenaries[slot] = (long) mercID;
    return GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_TRAINING_SLOTS, this.m_GSDMercenaries.ToArray()));
  }

  private void ValidateGSDState()
  {
    bool flag = false;
    (LettuceMercenary, LettuceMercenary) mercenariesInTraining = CollectionManager.Get().GetMercenariesInTraining();
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_TRAINING_SLOTS, this.m_GSDMercenaries);
    List<long> longList = this.m_GSDMercenaries;
    if (longList == null)
      longList = new List<long>() { 0L, 0L };
    this.m_GSDMercenaries = longList;
    while (this.m_GSDMercenaries.Count < 2)
    {
      this.m_GSDMercenaries.Add(0L);
      flag = true;
    }
    LettuceMercenary lettuceMercenary1 = mercenariesInTraining.Item1;
    int num1 = lettuceMercenary1 != null ? lettuceMercenary1.ID : -1;
    LettuceMercenary lettuceMercenary2 = mercenariesInTraining.Item2;
    int num2 = lettuceMercenary2 != null ? lettuceMercenary2.ID : -1;
    int num3;
    if (this.m_GSDMercenaries[0] == (long) num1)
    {
      num3 = num1;
      num1 = -1;
    }
    else if (this.m_GSDMercenaries[0] == (long) num2)
    {
      num3 = num2;
      num2 = -1;
    }
    else
      num3 = 0;
    int num4;
    if (this.m_GSDMercenaries[1] == (long) num1)
    {
      num4 = num1;
      num1 = -1;
    }
    else if (this.m_GSDMercenaries[1] == (long) num2)
    {
      num4 = num2;
      num2 = -1;
    }
    else
      num4 = 0;
    if (num1 != -1)
    {
      if (num3 == 0)
        num3 = num1;
      else
        num4 = num1;
    }
    if (num2 != -1)
    {
      if (num3 == 0)
        num3 = num2;
      else
        num4 = num2;
    }
    if (this.m_GSDMercenaries[0] != (long) num3 || this.m_GSDMercenaries[1] != (long) num4)
    {
      this.m_GSDMercenaries[0] = (long) num3;
      this.m_GSDMercenaries[1] = (long) num4;
      flag = true;
    }
    if (!flag)
      return;
    GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_TRAINING_SLOTS, this.m_GSDMercenaries.ToArray()));
  }

  private LettuceMercenaryDataModel GetDataModelForMerc(
    LettuceMercenary merc)
  {
    LettuceMercenaryDataModel dataModelForMerc = (LettuceMercenaryDataModel) null;
    if (merc == null)
      return (LettuceMercenaryDataModel) null;
    if (this.m_mercenaryDMCache.ContainsKey(merc.ID))
    {
      dataModelForMerc = this.m_mercenaryDMCache[merc.ID];
      dataModelForMerc.MercenaryLevel = GameUtils.GetMercenaryLevelFromExperience((int) merc.m_experience);
      dataModelForMerc.ExperienceInitial = (int) merc.m_experience;
    }
    else
    {
      LettuceMercenaryDataModel mercenaryDataModel = MercenaryFactory.CreateMercenaryDataModel(merc);
      if (mercenaryDataModel != null)
      {
        this.m_mercenaryDMCache.Add(merc.ID, mercenaryDataModel);
        dataModelForMerc = mercenaryDataModel;
      }
    }
    return dataModelForMerc;
  }

  private void OnSearchReady(CollectionSearch obj)
  {
    obj.RegisterActivatedListener(new CollectionSearch.ActivatedListener(this.OnSearchActivated));
    obj.RegisterDeactivatedListener(new CollectionSearch.DeactivatedListener(this.OnSearchDeactivated));
    obj.RegisterClearedListener(new CollectionSearch.ClearedListener(this.OnSearchCleared));
  }

  private void OnSearchActivated()
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    this.EnableInput(false);
  }

  private void OnSearchDeactivated(string oldSearchText, string newSearchText)
  {
    if (!string.Equals(newSearchText, this.m_searchText, StringComparison.OrdinalIgnoreCase))
    {
      this.m_searchText = newSearchText;
      this.PopulateMercList();
    }
    this.EnableInput(true);
  }

  private void OnSearchCleared(bool transitionPage)
  {
    if (string.IsNullOrEmpty(this.m_searchText))
      return;
    this.m_searchText = (string) null;
    this.PopulateMercList();
  }

  private void OnListSubwidgetReady(VisualController obj)
  {
    GameObject gameObject = obj.gameObject;
    this.m_listVC = obj;
    this.m_scrollbar = gameObject.GetComponentInChildren<UIBScrollable>(true);
    this.m_mercListable = gameObject.GetComponentInChildren<Listable>(true);
    this.m_scrollbar.m_HeightMode = UIBScrollable.HeightMode.UseHeightCallback;
    this.m_scrollbar.SetScrollHeightCallback(new UIBScrollable.ScrollHeightCallback(this.ScrollHeightCallback));
  }

  private static int CompareMercenaries(LettuceMercenary a, LettuceMercenary b)
  {
    if (a.m_level == b.m_level)
      return string.Compare(a.m_mercName, b.m_mercName);
    return a.m_level <= b.m_level ? -1 : 1;
  }

  private void PopulateMercList()
  {
    if (this.m_disabledMercenaries == null)
      this.m_disabledMercenaries = LettuceVillageDataUtil.GetDisabledMercenaryList();
    CollectionManager.FindMercenariesResult mercenaries = CollectionManager.Get().FindMercenaries(this.m_searchText, new bool?(true), excludeCraftableFromOwned: new bool?(true), ordered: false);
    mercenaries.m_mercenaries.Sort(new Comparison<LettuceMercenary>(LettuceVillageTrainingHall.CompareMercenaries));
    this.m_dataModel.MercenaryList.Clear();
    if (!this.m_didInitialLoad)
    {
      this.m_didInitialLoad = true;
      this.m_scrollbar.SetScrollImmediate(0.0f);
      Debug.Log((object) "populating merc list in batches");
      this.m_mercenariesToLoad = mercenaries.m_mercenaries;
      this.StartCoroutine(this.BatchLoadRemainingMercenaryTiles());
    }
    else
    {
      foreach (LettuceMercenary mercenary in mercenaries.m_mercenaries)
      {
        if (!LettuceVillageDataUtil.IsMercenaryDisabled(mercenary.ID, this.m_disabledMercenaries) && (mercenary.m_trainingStartDate == null || mercenary.m_trainingStartDate.Year == 0))
        {
          LettuceMercenaryDataModel dataModelForMerc = this.GetDataModelForMerc(mercenary);
          dataModelForMerc.ShowLevelInList = true;
          dataModelForMerc.ChildUpgradeAvailable = false;
          this.m_dataModel.MercenaryList.Add(dataModelForMerc);
        }
      }
      this.m_scrollbar.UpdateScroll();
    }
  }

  private IEnumerator BatchLoadRemainingMercenaryTiles()
  {
    yield return (object) new WaitForSeconds(0.1f);
    int countCheck = 0;
    while (this.m_mercenariesToLoad.Count > 0)
    {
      if (countCheck > 10)
      {
        countCheck = 0;
        yield return (object) new WaitForSeconds(0.1f);
      }
      if (this.m_mercenariesToLoad.Count > 0)
      {
        LettuceMercenary merc = this.m_mercenariesToLoad[0];
        if (!LettuceVillageDataUtil.IsMercenaryDisabled(merc.ID, this.m_disabledMercenaries) && (merc.m_trainingStartDate == null || merc.m_trainingStartDate.Year == 0))
        {
          LettuceMercenaryDataModel dataModelForMerc = this.GetDataModelForMerc(merc);
          dataModelForMerc.ShowLevelInList = true;
          dataModelForMerc.ChildUpgradeAvailable = false;
          this.m_dataModel.MercenaryList.Add(dataModelForMerc);
        }
        this.m_mercenariesToLoad.RemoveAt(0);
        ++countCheck;
      }
      else
        break;
    }
    this.m_scrollbar.UpdateScroll();
  }

  private float ScrollHeightCallback()
  {
    Bounds boundsOfChildren = TransformUtil.GetBoundsOfChildren((Component) this.m_mercListable, true);
    return boundsOfChildren.max.z - boundsOfChildren.min.z;
  }

  private void OnMercenariesDraggablesReady(Widget widget)
  {
    if (!((UnityEngine.Object) widget != (UnityEngine.Object) null))
      return;
    this.m_offScreenPosition = widget.transform.localPosition;
    this.m_mercenariesDraggablesWidget = widget;
    widget.Hide();
  }

  private void OnMercDragStarted()
  {
    this.m_dragColliderRoot.SetActive(true);
    LettuceMercenaryDataModel payload = WidgetUtils.GetEventDataModel(this.m_listVC).Payload as LettuceMercenaryDataModel;
    if (this.GrabMerc(payload))
    {
      this.m_dataModel.IsPlayerDragging = true;
      this.m_dataModel.MercenaryList.Remove(payload);
    }
    else
      this.m_dragColliderRoot.SetActive(false);
  }

  private void DisableDraggableColliders()
  {
    BoxCollider[] componentsInChildren = this.m_mercenariesDraggablesWidget.gameObject.GetComponentsInChildren<BoxCollider>(true);
    if (componentsInChildren == null)
      return;
    foreach (Collider collider in componentsInChildren)
      collider.enabled = false;
  }

  private bool GrabMerc(LettuceMercenaryDataModel mercData)
  {
    Ray ray = UniversalInputManager.Get().MousePositionToRay(Box.Get().GetCamera());
    RaycastHit hitInfo;
    if (!this.m_dragPlaneCollider.Raycast(ray, out hitInfo, 1000f))
      return false;
    this.m_draggedMerc = mercData;
    this.m_mercenariesDraggablesWidget.Show();
    this.m_mercenariesDraggablesWidget.BindDataModel((IDataModel) mercData);
    this.DisableDraggableColliders();
    this.CheckMercOverDropArea(ray, true);
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercData.MercenaryId, ReportError: false);
    this.m_draggedMercIsMaxLevel = mercenary != null ? mercenary.IsMaxLevel() : mercData.MercenaryLevel >= 30;
    this.m_mercenariesDraggablesWidget.transform.position = hitInfo.point;
    this.m_dataModel.ErrorText = (string) null;
    return true;
  }

  private void UpdateDraggedMerc()
  {
    Ray ray = UniversalInputManager.Get().MousePositionToRay(Box.Get().GetCamera());
    RaycastHit hitInfo;
    if (!this.m_dragPlaneCollider.Raycast(ray, out hitInfo, 1000f))
      return;
    this.m_mercenariesDraggablesWidget.gameObject.transform.position = hitInfo.point;
    this.CheckMercOverDropArea(ray);
    if (!InputCollection.GetMouseButtonUp(0))
      return;
    this.MercDropped();
  }

  private void CheckMercOverDropArea(Ray mouseRay, bool forceModeEvent = false)
  {
    this.m_draggedMercIsOverTrainingArea = this.m_dropZoneCollider.Raycast(mouseRay, out RaycastHit _, 1000f);
    bool flag1 = this.m_dataModel.Slot1.SlotIsEmpty && !this.m_dataModel.Slot1.Locked || this.m_dataModel.Slot2.SlotIsEmpty && !this.m_dataModel.Slot2.Locked;
    bool flag2 = ((!this.m_draggedMercIsOverTrainingArea ? 0 : (!this.m_draggedMercIsMaxLevel ? 1 : 0)) & (flag1 ? 1 : 0)) != 0;
    if (!(flag2 != this.m_dataModel.IsMercOverTrainingWindow | forceModeEvent))
      return;
    if (flag2)
      this.m_mercenariesDraggablesWidget.TriggerEvent("HOLD_MERC_OVER_TRAINING_code");
    else
      this.m_mercenariesDraggablesWidget.TriggerEvent("HOLD_MERC_OVER_TEAM_TRAY_code");
    this.m_dataModel.IsMercOverTrainingWindow = flag2;
  }

  private void MercDropped()
  {
    if (this.m_dataModel.IsMercOverTrainingWindow)
    {
      this.PlaceMercenaryInTraining(this.m_draggedMerc.MercenaryId, this.m_dataModel.Slot1.SlotIsEmpty ? 0 : 1);
      this.m_trainingWindowWidget.TriggerEvent(this.m_lastSlotInteracted == 0 ? "MERC_DROPPED_SLOT_1" : "MERC_DROPPED_SLOT_2");
    }
    else
    {
      this.PopulateMercList();
      this.m_trainingWindowWidget.TriggerEvent("MERC_DROPPED_LIST");
      if (this.m_draggedMercIsOverTrainingArea)
        this.m_dataModel.ErrorText = GameStrings.Get(this.m_draggedMercIsMaxLevel ? "GLUE_TRAINING_HALL_MAX_LEVEL_MSG" : "GLUE_TRAINING_HALL_NO_SLOTS_MSG");
    }
    this.m_draggedMerc = (LettuceMercenaryDataModel) null;
    this.m_dataModel.IsPlayerDragging = false;
    this.m_dataModel.IsMercOverTrainingWindow = false;
    this.m_draggedMercIsOverTrainingArea = false;
    this.m_draggedMercIsMaxLevel = false;
    this.m_mercenariesDraggablesWidget.transform.localPosition = this.m_offScreenPosition;
    this.m_mercenariesDraggablesWidget.Hide();
    this.m_dragColliderRoot.SetActive(false);
  }

  public void OnDestroy()
  {
    CollectionManager.Get().OnMercenariesTrainingAddResponseReceived -= new Action(this.OnMercenariesTrainingAddResponse);
    CollectionManager.Get().OnMercenariesTrainingRemoveResponseReceived -= new Action(this.OnMercenariesTrainingRemoveResponse);
    CollectionManager.Get().OnMercenariesTrainingCollectResponseReceived -= new Action(this.OnMercenariesTrainingCollectResponse);
  }

  [System.Flags]
  private enum TrainingHallFlags : long
  {
    SLOT_2_DONE_SHOWING_NEW_DECORATION = 1,
  }

  public delegate void OnCardDroppedCallback();

  private struct TrainingMetrics
  {
    public int numSlotsAvailable;
    public int expPerHour;
    public int maxExpGained;
    public int minTrainingTime;
  }
}
