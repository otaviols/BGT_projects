using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusLettuce;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LettuceMapDisplay : AbsSceneDisplay
{
  private const float DEFAULT_MAP_SCROLL_TIME = 0.5f;
  private const float INTRO_MAP_SCROLL_TIME = 2f;
  private const float VICTORY_MAP_SCROLL_TIME = 2f;
  private const float CHEST_ANIMATION_TIME = 3f;
  private const float DELAY_BEFORE_MAP_SCROLL = 0.25f;
  private const float DELAY_AFTER_MAP_SCROLL = 0.25f;
  private const float COIN_FLIP_TIME = 1f;
  private const float FULLSCREEN_FX_TIME = 0.25f;
  private const float DELAY_BEFORE_SHOWING_CAMPFIRE = 1f;
  public static readonly AssetReference REWARD_PREFAB = new AssetReference("MercenariesRewardScroll.prefab:b8b2a8f8d472c5945aafd50c39464e4c");
  public static readonly AssetReference VISITOR_FALLBACK_REWARD_PREFAB = new AssetReference("MercenariesVisitorFallbackRewardScroll.prefab:726d3a412ad3d0b46a66a18f2289e41d");
  public static readonly AssetReference VISITOR_TASK_REWARD_PREFAB = new AssetReference("MercenariesNewTaskRewardScroll.prefab:24bc75ecdd51b344daa0830c9041207b");
  public AsyncReference m_PlayButtonReference;
  public AsyncReference m_PlayButtonPhoneReference;
  public AsyncReference m_BackButtonReference;
  public AsyncReference m_BackButtonPhoneReference;
  public AsyncReference m_LettuceMapReference;
  public AsyncReference m_TeamPreviewReference;
  public AsyncReference m_TeamPreviewPhoneReference;
  public AsyncReference m_RetireButtonReference;
  public AsyncReference m_RetireButtonPhoneReference;
  public AsyncReference m_FinalBossChestReference;
  public AsyncReference m_MapMaskableReference;
  public AsyncReference m_EndOfRunReference;
  public AsyncReference m_EndOfRunBackButtonReference;
  public AsyncReference m_TreasureTeamViewReference;
  public UIBScrollable m_Scrollable;
  private PlayButton m_playButton;
  private LettuceMap m_lettuceMap;
  private VisualController m_teamPreviewVisualController;
  private VisualController m_finalBossChestVisualController;
  private VisualController m_endOfRunVisualController;
  private VisualController m_treasureTeamViewVisualController;
  private Maskable m_mapMaskable;
  private int m_selectedTreasureIndex = -1;
  private int m_selectedVisitorIndex = -1;
  private bool m_playButtonFinishedLoading;
  private bool m_backButtonFinishedLoading;
  private bool m_lettuceMapFinishedLoading;
  private bool m_lettuceMapFinishedChangingStates;
  private bool m_teamPreviewFinishedLoading;
  private bool m_retireButtonFinishedLoading;
  private bool m_finalBossChestFinishedLoading;
  private bool m_endOfRunFinishedLoading;
  private bool m_treasureTeamViewLoading;
  private bool m_mapMaskableFinishedLoading;
  private bool m_lettuceMapDataInitialized;
  private bool m_loadingScreenTransitionFromGameplayStarted;
  private bool m_loadingScreenTransitionFromGameplayComplete;
  private PegasusLettuce.LettuceMap m_lettuceMapProto;
  private LettuceMapCoinDataModel m_selectedMapCoin;
  private RewardPresenter m_rewardPresenter = new RewardPresenter()
  {
    m_rewardPrefab = LettuceMapDisplay.REWARD_PREFAB
  };
  private bool m_isNewMap;
  private bool m_waitingForTreasureSelection;
  private bool m_waitingForVisitorSelectionServerResponse;
  private bool m_waitingForVisualControllerState;
  private List<IDisposable> m_disposables = new List<IDisposable>();
  private bool m_isTeamViewVisible;
  private bool m_taskBoardIsOpen;
  private LettuceMapDisplay.CurrentResultState m_currentMapResult;
  private bool m_currentMapIsComplete;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private bool LettuceVillageVisitorDataInitialized
  {
    get
    {
      NetCache.NetCacheMercenariesVillageInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageInfo>();
      return netObject != null && netObject.Initialized;
    }
  }

  public override void Start()
  {
    base.Start();
    CollectionManager.Get().StartInitialMercenaryLoadIfRequired();
    this.m_sceneDisplayWidgetReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnLettuceMapDisplayReady));
    this.m_LettuceMapReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnLettuceMapReady));
    this.m_FinalBossChestReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnFinalBossChestReady));
    this.m_MapMaskableReference.RegisterReadyListener<Maskable>(new System.Action<Maskable>(this.OnMapMaskableReady));
    this.m_EndOfRunReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnEndOfRunReady));
    this.m_EndOfRunBackButtonReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnEndOfRunBackButtonReady));
    this.m_TreasureTeamViewReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnTreasureTeamViewReady));
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_TeamPreviewPhoneReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnTeamPreviewReady));
      this.m_PlayButtonPhoneReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnPlayButtonReady));
      this.m_BackButtonPhoneReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnBackButtonReady));
      this.m_RetireButtonPhoneReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnRetireButtonReady));
    }
    else
    {
      this.m_TeamPreviewReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnTeamPreviewReady));
      this.m_PlayButtonReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnPlayButtonReady));
      this.m_BackButtonReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnBackButtonReady));
      this.m_RetireButtonReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnRetireButtonReady));
    }
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheLettuceMap), new System.Action(this.OnLettuceMapReceived));
    Network.Get().RegisterNetHandler((object) LettuceMapChooseNodeResponse.PacketID.ID, new Network.NetHandler(this.OnLettuceMapChooseNodeResponseReceived));
    Network.Get().RegisterNetHandler((object) LettuceMapRetireResponse.PacketID.ID, new Network.NetHandler(this.OnLettuceMapRetireResponseReceived));
    Network.Get().RegisterNetHandler((object) MercenariesMapTreasureSelectionResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesMapTreasureSelectionResponseReceived));
    Network.Get().RegisterNetHandler((object) MercenariesMapVisitorSelectionResponse.PacketID.ID, new Network.NetHandler(this.OnVisitorSelectionResponseReceived));
    PartyManager.Get().AddChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
    PartyManager.Get().AddPartyAttributeChangedListener(new PartyManager.PartyAttributeChangedCallback(this.OnPartyAttributeChanged));
    if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY)
    {
      LoadingScreen.Get().RegisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFromGameplayFinished));
      LoadingScreen.Get().OnFadeInStart += new System.Action(this.OnLoadingScreenFadeInStarted);
    }
    else
    {
      this.m_loadingScreenTransitionFromGameplayStarted = true;
      this.m_loadingScreenTransitionFromGameplayComplete = true;
    }
    PegUI.Get().RegisterForRenderPassPriorityHitTest((Component) this);
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MercenariesSubMenus);
    if (!GameUtils.IsMercenariesVillageTutorialComplete())
      NarrativeManager.Get().PreloadMercenaryTutorialDialogue();
    this.StartCoroutine(this.InitializeWhenReady());
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  public void OnDestroy()
  {
    if (NetCache.Get() != null)
      NetCache.Get().RemoveUpdatedListener(typeof (NetCache.NetCacheLettuceMap), new System.Action(this.OnLettuceMapReceived));
    if (Network.Get() != null)
    {
      Network.Get().RemoveNetHandler((object) LettuceMapChooseNodeResponse.PacketID.ID, new Network.NetHandler(this.OnLettuceMapChooseNodeResponseReceived));
      Network.Get().RemoveNetHandler((object) LettuceMapRetireResponse.PacketID.ID, new Network.NetHandler(this.OnLettuceMapRetireResponseReceived));
      Network.Get().RemoveNetHandler((object) MercenariesMapTreasureSelectionResponse.PacketID.ID, new Network.NetHandler(this.OnMercenariesMapTreasureSelectionResponseReceived));
      Network.Get().RemoveNetHandler((object) MercenariesMapVisitorSelectionResponse.PacketID.ID, new Network.NetHandler(this.OnVisitorSelectionResponseReceived));
    }
    if (PartyManager.Get() != null)
    {
      PartyManager.Get().RemoveChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
      PartyManager.Get().RemovePartyAttributeChangedListener(new PartyManager.PartyAttributeChangedCallback(this.OnPartyAttributeChanged));
    }
    if ((UnityEngine.Object) LoadingScreen.Get() != (UnityEngine.Object) null)
    {
      LoadingScreen.Get().UnregisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFromGameplayFinished));
      LoadingScreen.Get().OnFadeInStart -= new System.Action(this.OnLoadingScreenFadeInStarted);
    }
    if ((UnityEngine.Object) PegUI.Get() != (UnityEngine.Object) null)
      PegUI.Get().UnregisterFromRenderPassPriorityHitTest((Component) this);
    this.m_disposables.DisposeValuesAndClear<IDisposable>();
  }

  public override bool IsFinishedLoading(out string failureMessage)
  {
    if (!this.AsyncAssetsFinishedLoading())
    {
      failureMessage = "LettuceMapDisplay - Widget references never loaded.";
      return false;
    }
    if (!this.m_lettuceMapFinishedChangingStates)
    {
      failureMessage = "LettuceMapDisplay - Map never finished changing states.";
      return false;
    }
    if (!this.m_lettuceMapDataInitialized)
    {
      failureMessage = "LettuceMapDisplay - Map data was never initialized.";
      return false;
    }
    if (!this.LettuceVillageVisitorDataInitialized)
    {
      failureMessage = "LettuceMapDisplay - Village visitor data was never initialized.";
      return false;
    }
    if ((UnityEngine.Object) this.m_lettuceMap == (UnityEngine.Object) null || !this.m_lettuceMap.IsFinishedLoading())
    {
      failureMessage = "LettuceMapDisplay - Map never finished loading.";
      return false;
    }
    if (!CollectionManager.Get().IsLettuceLoaded())
    {
      failureMessage = "LettuceMapDisplay - Lettuce Collection Manager never loaded.";
      return false;
    }
    failureMessage = string.Empty;
    return true;
  }

  public bool AsyncAssetsFinishedLoading() => this.m_playButtonFinishedLoading && this.m_backButtonFinishedLoading && this.m_lettuceMapFinishedLoading && this.m_teamPreviewFinishedLoading && this.m_retireButtonFinishedLoading && this.m_finalBossChestFinishedLoading && this.m_mapMaskableFinishedLoading && this.m_endOfRunFinishedLoading && this.m_treasureTeamViewLoading;

  public override bool IsBlockingPopupDisplayManager()
  {
    if (!this.m_loadingScreenTransitionFromGameplayComplete || this.m_waitingForTreasureSelection || this.m_waitingForVisualControllerState || this.m_rewardPresenter.IsShowingReward() || this.IsCurrentBountyTutorial())
      return true;
    return this.m_lettuceMapProto != null && !this.m_lettuceMapProto.Active;
  }

  public static LettuceMapNodeTypeAnomaly.MercenariesBonusRewardType GetBonusRewardTypeForCardId(
    int cardId)
  {
    LettuceMapNodeTypeAnomalyDbfRecord record = GameDbf.LettuceMapNodeTypeAnomaly.GetRecord((Predicate<LettuceMapNodeTypeAnomalyDbfRecord>) (r => r.AnomalyCard == cardId));
    return record != null ? record.BonusRewardType : LettuceMapNodeTypeAnomaly.MercenariesBonusRewardType.NONE;
  }

  private void OnPlayButtonRelease(UIEvent e) => this.ExecutePlayLogic();

  private void OnEndOfRunButtonRelease(UIEvent e)
  {
    this.m_screenEffectsHandle.StopEffect();
    if (this.ShouldShowTaskboard())
      this.StartCoroutine(this.ShowAndWaitForTaskBoard(true, e));
    else
      this.OnBackButtonRelease(e);
  }

  private void OnBackButtonRelease(UIEvent e)
  {
    SceneMgr.Mode nextMode = SceneMgr.Mode.LETTUCE_VILLAGE;
    if (this.IsCurrentBountyTutorial())
    {
      if (this.m_currentMapIsComplete)
      {
        LettuceVillageDisplay.LettuceSceneTransitionPayload transitionPayload = new LettuceVillageDisplay.LettuceSceneTransitionPayload();
        LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord((int) this.m_lettuceMapProto.BountyId);
        transitionPayload.m_SelectedBounty = record;
        transitionPayload.m_SelectedBountySet = record.BountySetRecord;
        this.m_sceneTransitionPayload = (object) transitionPayload;
        this.BackOutOfScene(SceneMgr.Mode.LETTUCE_VILLAGE);
      }
      else
        this.BackOutOfScene(SceneMgr.Mode.HUB);
    }
    else
    {
      if (this.m_currentMapIsComplete)
      {
        if (PartyManager.Get().IsInMercenariesCoOpParty())
        {
          PartyManager.Get().LeaveParty();
          this.BackOutOfScene(SceneMgr.Mode.LETTUCE_VILLAGE);
          return;
        }
        nextMode = SceneMgr.Mode.LETTUCE_BOUNTY_BOARD;
        LettuceVillageDisplay.LettuceSceneTransitionPayload transitionPayload = new LettuceVillageDisplay.LettuceSceneTransitionPayload();
        LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord((int) this.m_lettuceMapProto.BountyId);
        transitionPayload.m_SelectedBounty = record;
        transitionPayload.m_SelectedBountySet = record.BountySetRecord;
        transitionPayload.m_DifficultyMode = record.DifficultyMode;
        this.m_sceneTransitionPayload = (object) transitionPayload;
      }
      else if (PartyManager.Get().IsInMercenariesCoOpParty())
      {
        DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
        {
          m_headerText = "Leave Party?",
          m_text = "Would you like to leave the party and end the run?",
          m_iconSet = AlertPopup.PopupInfo.IconSet.Default,
          m_showAlertIcon = true,
          m_alertTextAlignment = UberText.AlignmentOptions.Center,
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_confirmText = "Leave",
          m_cancelText = "Stay",
          m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
          {
            if (response != AlertPopup.Response.CONFIRM)
              return;
            PartyManager.Get().LeaveParty();
            this.BackOutOfScene(SceneMgr.Mode.LETTUCE_VILLAGE);
          })
        });
        return;
      }
      this.BackOutOfScene(nextMode);
    }
  }

  private void BackOutOfScene(SceneMgr.Mode nextMode)
  {
    if (NetCache.Get() != null)
      NetCache.Get().RemoveUpdatedListener(typeof (NetCache.NetCacheLettuceMap), new System.Action(this.OnLettuceMapReceived));
    if (nextMode == SceneMgr.Mode.HUB)
    {
      SceneMgr.Get().SetNextMode(nextMode);
    }
    else
    {
      SceneMgr.TransitionHandlerType type = SceneMgr.TransitionHandlerType.NEXT_SCENE;
      if (nextMode == SceneMgr.Mode.LETTUCE_VILLAGE)
        type = SceneMgr.TransitionHandlerType.CURRENT_SCENE;
      this.SetNextModeAndHandleTransition(nextMode, type, this.m_sceneTransitionPayload);
    }
  }

  private void OnRetireButtonRelease(UIEvent e)
  {
    if (!Network.IsLoggedIn())
      DialogManager.Get().ShowReconnectHelperDialog();
    else
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_MAP_RETIRE_DIALOG_HEADER"),
        m_text = GameStrings.Get("GLUE_LETTUCE_MAP_RETIRE_DIALOG_BODY"),
        m_iconSet = AlertPopup.PopupInfo.IconSet.Default,
        m_showAlertIcon = false,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
        m_confirmText = GameStrings.Get("GLUE_LETTUCE_MAP_RETIRE_DIALOG_CONFIRM"),
        m_cancelText = GameStrings.Get("GLUE_LETTUCE_MAP_RETIRE_DIALOG_CANCEL"),
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
        {
          if (response != AlertPopup.Response.CONFIRM)
            return;
          this.m_clickBlocker.SetActive(true);
          Network.Get().RetireLettuceMap();
        })
      });
  }

  private void LettuceMapEventListener(string eventName)
  {
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(eventName))
    {
      case 41058832:
        if (!(eventName == "TREASURE_SELECTED"))
          break;
        this.OnTreasureSelected();
        break;
      case 1110928211:
        if (!(eventName == "VISITOR_SELECTED"))
          break;
        this.OnVisitorSelected();
        break;
      case 1790750517:
        if (!(eventName == "HIDE_TEAM_code"))
          break;
        this.OnTeamViewHide();
        break;
      case 2367139770:
        if (!(eventName == "VISUAL_CONTROLLER_STATE_COMPLETE"))
          break;
        this.m_waitingForVisualControllerState = false;
        break;
      case 2379933476:
        if (!(eventName == "SHOW_TEAM_code"))
          break;
        this.OnTeamViewShow();
        break;
      case 2391491787:
        if (!(eventName == "LETTUCE_COIN_RELEASED"))
          break;
        this.OnCoinSelected();
        break;
      case 3377302564:
        if (!(eventName == "VISITOR_CHOSEN"))
          break;
        this.OnVisitorChosen();
        break;
      case 4185022471:
        if (!(eventName == "TREASURE_CHOSEN"))
          break;
        this.OnTreasureChosen();
        break;
    }
  }

  private LettuceMapDisplayDataModel GetDisplayDataModel()
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return (LettuceMapDisplayDataModel) null;
    Widget owner = (Widget) component.Owner;
    IDataModel model;
    if (!owner.GetDataModel(201, out model))
    {
      model = (IDataModel) new LettuceMapDisplayDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceMapDisplayDataModel;
  }

  public void OnPlayButtonReady(VisualController buttonVisualController)
  {
    if ((UnityEngine.Object) buttonVisualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "PlayButton could not be found! You will not be able to click 'Play'!");
    }
    else
    {
      this.m_playButton = buttonVisualController.gameObject.GetComponent<PlayButton>();
      this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonRelease));
      this.m_playButton.Disable();
      this.m_playButtonFinishedLoading = true;
    }
  }

  public void OnBackButtonReady(VisualController buttonVisualController)
  {
    if ((UnityEngine.Object) buttonVisualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "BackButton could not be found! You will not be able to click 'Back'!");
    }
    else
    {
      buttonVisualController.gameObject.GetComponent<UIBButton>().AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonRelease));
      this.m_backButtonFinishedLoading = true;
    }
  }

  public void OnEndOfRunBackButtonReady(VisualController buttonVisualController)
  {
    if ((UnityEngine.Object) buttonVisualController == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "End of Run BackButton could not be found! You will not be able to click 'Back'!");
    else
      buttonVisualController.gameObject.GetComponent<UIBButton>().AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnEndOfRunButtonRelease));
  }

  private void OnLettuceMapDisplayReady(VisualController lettuceMapDisplayController)
  {
    if ((UnityEngine.Object) lettuceMapDisplayController == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "LettuceMapDisplay could not be found!");
    else
      lettuceMapDisplayController.GetComponent<Widget>().RegisterEventListener(new Widget.EventListenerDelegate(this.LettuceMapEventListener));
  }

  private void OnLettuceMapReady(VisualController lettuceMapController)
  {
    if ((UnityEngine.Object) lettuceMapController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "LettuceMap could not be found!");
    }
    else
    {
      this.m_lettuceMap = lettuceMapController.GetComponent<LettuceMap>();
      this.m_lettuceMapFinishedLoading = true;
      Widget component = lettuceMapController.GetComponent<Widget>();
      component.RegisterEventListener(new Widget.EventListenerDelegate(this.LettuceMapEventListener));
      component.RegisterDoneChangingStatesListener(new System.Action<object>(this.OnMapWidgetDoneChangingStates), (object) null, true, false);
    }
  }

  private void OnMapWidgetDoneChangingStates(object widget) => this.m_lettuceMapFinishedChangingStates = true;

  public void OnTeamPreviewReady(VisualController previewController)
  {
    if ((UnityEngine.Object) previewController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "TeamPreview could not be found!");
    }
    else
    {
      this.m_teamPreviewVisualController = previewController;
      this.m_teamPreviewFinishedLoading = true;
    }
  }

  public void OnEndOfRunReady(VisualController previewController)
  {
    if ((UnityEngine.Object) previewController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "EndOfRun could not be found!");
    }
    else
    {
      this.m_endOfRunVisualController = previewController;
      this.m_endOfRunFinishedLoading = true;
    }
  }

  public void OnTreasureTeamViewReady(VisualController previewController)
  {
    if ((UnityEngine.Object) previewController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "TreasureTeamVioew could not be found!");
    }
    else
    {
      this.m_treasureTeamViewVisualController = previewController;
      this.m_treasureTeamViewLoading = true;
    }
  }

  public void OnRetireButtonReady(VisualController buttonVisualController)
  {
    if ((UnityEngine.Object) buttonVisualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "RetireButton could not be found! You will not be able to click 'Retire'!");
    }
    else
    {
      buttonVisualController.gameObject.GetComponent<UIBButton>().AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRetireButtonRelease));
      this.m_retireButtonFinishedLoading = true;
    }
  }

  public void OnFinalBossChestReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "FinalBossChest could not be found!");
    }
    else
    {
      this.m_finalBossChestVisualController = visualController;
      this.m_finalBossChestFinishedLoading = true;
    }
  }

  public void OnMapMaskableReady(Maskable maskable)
  {
    this.m_mapMaskable = maskable;
    this.m_mapMaskableFinishedLoading = true;
  }

  public EventDataModel GetLettuceMapEventDataModel()
  {
    Widget component = this.m_lettuceMap.GetComponent<Widget>();
    return (UnityEngine.Object) component == (UnityEngine.Object) null ? (EventDataModel) null : component.GetDataModel<EventDataModel>();
  }

  private LettuceTeamDataModel GetTeamPreviewDataModel()
  {
    if ((UnityEngine.Object) this.m_teamPreviewVisualController == (UnityEngine.Object) null)
      return (LettuceTeamDataModel) null;
    Widget owner = (Widget) this.m_teamPreviewVisualController.Owner;
    IDataModel model;
    if (!owner.GetDataModel(217, out model))
    {
      model = (IDataModel) new LettuceTeamDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceTeamDataModel;
  }

  private LettuceBountyBoardDataModel GetBountyBoardDataModel()
  {
    if ((UnityEngine.Object) this.m_endOfRunVisualController == (UnityEngine.Object) null)
      return (LettuceBountyBoardDataModel) null;
    Widget owner = (Widget) this.m_endOfRunVisualController.Owner;
    IDataModel model;
    if (!owner.GetDataModel(194, out model))
    {
      model = (IDataModel) new LettuceBountyBoardDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceBountyBoardDataModel;
  }

  private LettuceBountyDataModel GetBountyDataModel()
  {
    if ((UnityEngine.Object) this.m_endOfRunVisualController == (UnityEngine.Object) null)
      return (LettuceBountyDataModel) null;
    Widget owner = (Widget) this.m_endOfRunVisualController.Owner;
    IDataModel model;
    if (!owner.GetDataModel(193, out model))
    {
      model = (IDataModel) new LettuceBountyDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceBountyDataModel;
  }

  protected override bool ShouldStartShown() => SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_VILLAGE;

  private IEnumerator InitializeWhenReady()
  {
    LettuceMapDisplay lettuceMapDisplay = this;
    lettuceMapDisplay.m_clickBlocker.SetActive(true);
    while (!lettuceMapDisplay.AsyncAssetsFinishedLoading())
      yield return (object) null;
    LettuceVillageDataUtil.InitializeData();
    lettuceMapDisplay.InitializeLettuceMapData();
    while (!lettuceMapDisplay.m_lettuceMapDataInitialized || !lettuceMapDisplay.LettuceVillageVisitorDataInitialized)
      yield return (object) null;
    if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY)
      lettuceMapDisplay.CheckAndEnqueueMercenaryGrant();
    lettuceMapDisplay.PopulateTeamPreviewData(lettuceMapDisplay.GetCurrentPlayerData());
    PartyManager.Get().SetSceneAttribute(SceneMgr.Get().GetMode().ToString());
    while (SceneMgr.Get().IsTransitioning())
      yield return (object) null;
    while (true)
    {
      GameToastMgr gameToastMgr = GameToastMgr.Get();
      if ((gameToastMgr != null ? (gameToastMgr.AreToastsActive() ? 1 : 0) : 0) == 0)
        goto label_13;
label_11:
      yield return (object) null;
      continue;
label_13:
      QuestToastManager questToastManager = QuestToastManager.Get();
      if ((questToastManager != null ? (questToastManager.AreToastsActive() ? 1 : 0) : 0) != 0)
        goto label_11;
      else
        break;
    }
    if (lettuceMapDisplay.ShouldShowTreasureSelection(lettuceMapDisplay.m_lettuceMapProto))
    {
      lettuceMapDisplay.m_treasureTeamViewVisualController.BindDataModel((IDataModel) lettuceMapDisplay.GetTeamPreviewDataModel());
      lettuceMapDisplay.m_treasureTeamViewVisualController.BindDataModel((IDataModel) lettuceMapDisplay.GetDisplayDataModel());
      while (!lettuceMapDisplay.m_loadingScreenTransitionFromGameplayStarted)
        yield return (object) null;
      lettuceMapDisplay.m_waitingForTreasureSelection = true;
      ScreenEffectParameters desaturatePerspective = ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = 0.25f
      };
      lettuceMapDisplay.m_screenEffectsHandle.StartEffect(desaturatePerspective);
      bool tutorialDone = false;
      LettuceTutorialUtils.FireEvent(LettuceTutorialVo.LettuceTutorialEvent.MAP_PRE_TREASURE_SELECTION, lettuceMapDisplay.gameObject, bountyRecordId: ((int) lettuceMapDisplay.m_lettuceMapProto.BountyId), onComplete: ((System.Action) (() => tutorialDone = true)));
      while (!tutorialDone)
        yield return (object) null;
      lettuceMapDisplay.InitializeTreasureSelectionData(lettuceMapDisplay.m_lettuceMapProto);
      while (lettuceMapDisplay.m_waitingForTreasureSelection)
        yield return (object) null;
    }
    yield return (object) lettuceMapDisplay.TryShowingVisitorSelection(lettuceMapDisplay.m_lettuceMapProto);
    NetCache.NetCacheMercenariesVillageVisitorInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesVillageVisitorInfo>();
    List<MercenariesTaskState> completedTasks = (List<MercenariesTaskState>) null;
    if (netObject != null && netObject.CompletedTasks != null && netObject.CompletedTasks.Count > 0)
    {
      completedTasks = netObject.CompletedTasks.ToList<MercenariesTaskState>();
      netObject.CompletedTasks.Clear();
      yield return (object) LettuceVillageDataUtil.ShowTaskToast(completedTasks);
    }
    if (lettuceMapDisplay.m_lettuceMapProto.WonLastCombatWithNoLivingMercenaries)
    {
      while (!lettuceMapDisplay.m_loadingScreenTransitionFromGameplayComplete)
        yield return (object) null;
      bool waitingForRunEndedDialog = true;
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_MAP_WIPEOUT_DIALOG_HEADER"),
        m_text = GameStrings.Get("GLUE_LETTUCE_MAP_WIPEOUT_DIALOG_BODY"),
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_okText = GameStrings.Get("GLOBAL_OKAY"),
        m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => waitingForRunEndedDialog = false)
      });
      while (waitingForRunEndedDialog)
        yield return (object) null;
    }
    lettuceMapDisplay.m_endOfRunVisualController.BindDataModel((IDataModel) lettuceMapDisplay.GetTeamPreviewDataModel());
    lettuceMapDisplay.m_endOfRunVisualController.BindDataModel((IDataModel) lettuceMapDisplay.GetDisplayDataModel());
    bool waitingForRewardPopupToClose = false;
    if (lettuceMapDisplay.m_currentMapResult != LettuceMapDisplay.CurrentResultState.WON_MAP && PopupDisplayManager.Get().RewardPopups.HasNonAutoRetireMercenariesRewardsToShow())
    {
      while (!lettuceMapDisplay.m_loadingScreenTransitionFromGameplayStarted)
        yield return (object) null;
      ScreenEffectParameters desaturatePerspective = ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = 0.25f
      };
      lettuceMapDisplay.m_screenEffectsHandle.StartEffect(desaturatePerspective);
      NetCache.ProfileNoticeMercenariesRewards mercenariesRewardToShow1 = PopupDisplayManager.Get().RewardPopups.GetNextNonAutoRetireRewardMercenariesRewardToShow();
      NetCache.ProfileNoticeMercenariesRewards mercenariesRewardToShow2 = PopupDisplayManager.Get().RewardPopups.GetNextBonusMercenariesRewardToShow();
      waitingForRewardPopupToClose = true;
      PopupDisplayManager.Get().RewardPopups.ShowMercenariesRewards(true, mercenariesRewardToShow1, mercenariesRewardToShow2, (System.Action) (() =>
      {
        this.m_screenEffectsHandle.StopEffect();
        waitingForRewardPopupToClose = false;
      }));
    }
    while (!lettuceMapDisplay.m_loadingScreenTransitionFromGameplayComplete)
      yield return (object) null;
    switch (lettuceMapDisplay.m_currentMapResult)
    {
      case LettuceMapDisplay.CurrentResultState.NEW_MAP:
        yield return (object) lettuceMapDisplay.PlayIntroMapScroll();
        break;
      case LettuceMapDisplay.CurrentResultState.WON_MAP:
        yield return (object) lettuceMapDisplay.PlayVictoryMapScroll();
        break;
      default:
        yield return (object) lettuceMapDisplay.PlayDefaultMapScroll();
        break;
    }
    while (waitingForRewardPopupToClose)
      yield return (object) null;
    if (lettuceMapDisplay.ShouldShowTaskboard() && completedTasks != null && lettuceMapDisplay.m_currentMapResult != LettuceMapDisplay.CurrentResultState.WON_MAP)
    {
      completedTasks.Sort((Comparison<MercenariesTaskState>) ((a, b) =>
      {
        VisitorTaskDbfRecord taskRecordById1 = LettuceVillageDataUtil.GetTaskRecordByID(a.TaskId);
        VisitorTaskDbfRecord taskRecordById2 = LettuceVillageDataUtil.GetTaskRecordByID(b.TaskId);
        MercenaryVisitorDbfRecord visitorRecordById1 = LettuceVillageDataUtil.GetVisitorRecordByID(taskRecordById1 != null ? taskRecordById1.MercenaryVisitorId : 0);
        MercenaryVisitorDbfRecord visitorRecordById2 = LettuceVillageDataUtil.GetVisitorRecordByID(taskRecordById2 != null ? taskRecordById2.MercenaryVisitorId : 0);
        return visitorRecordById1 == null || visitorRecordById2 == null ? (visitorRecordById1 == null).CompareTo(visitorRecordById2 == null) : (visitorRecordById2.VisitorType == MercenaryVisitor.VillageVisitorType.STANDARD).CompareTo(visitorRecordById1.VisitorType == MercenaryVisitor.VillageVisitorType.STANDARD);
      }));
      int focusedVisitorStateId = 0;
      if (completedTasks.Count > 0 && completedTasks[0] != null)
      {
        VisitorTaskDbfRecord taskRecordById = LettuceVillageDataUtil.GetTaskRecordByID(completedTasks[0].TaskId);
        if (taskRecordById != null)
          focusedVisitorStateId = taskRecordById.MercenaryVisitorId;
      }
      yield return (object) lettuceMapDisplay.ShowAndWaitForTaskBoard(false, (UIEvent) null, focusedVisitorStateId);
    }
    if (lettuceMapDisplay.m_currentMapResult == LettuceMapDisplay.CurrentResultState.WON_MAP || lettuceMapDisplay.m_currentMapResult == LettuceMapDisplay.CurrentResultState.WON_NODE)
    {
      yield return (object) lettuceMapDisplay.CheckLastCompletedNodeTutorialEvents();
      yield return (object) lettuceMapDisplay.CheckForNodeDialogueEvents();
    }
    lettuceMapDisplay.m_clickBlocker.SetActive(false);
    if (lettuceMapDisplay.m_currentMapIsComplete)
      lettuceMapDisplay.HandleEndOfRun();
  }

  private void HandleEndOfRun()
  {
    if (this.m_currentMapResult == LettuceMapDisplay.CurrentResultState.WON_MAP)
    {
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = 0.25f
      });
      LettuceBountyBoardDataModel bountyBoardDataModel = this.GetBountyBoardDataModel();
      LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord((int) this.m_lettuceMapProto.BountyId);
      int finalBossCardId = record.FinalBossCardId;
      string name = DefLoader.Get().GetEntityDef(finalBossCardId).GetName();
      bountyBoardDataModel.BossName = name;
      this.InitializeBountyDataModel(this.GetBountyDataModel(), record);
      this.m_endOfRunVisualController.SetState("SHOW");
    }
    else
      this.OnBackButtonRelease((UIEvent) null);
  }

  private bool ShouldShowTaskboard()
  {
    SceneMgr sceneMgr = SceneMgr.Get();
    return (sceneMgr != null ? (int) sceneMgr.GetPrevMode() : 0) == 4 && LettuceVillageDataUtil.GetNotificationStatusForBuilding(MercenaryBuilding.Mercenarybuildingtype.TASKBOARD);
  }

  private void OnTaskboardClosed(LettuceVillagePopupManager.PopupType popupType)
  {
    if (popupType != LettuceVillagePopupManager.PopupType.TASKBOARD)
      return;
    this.m_taskBoardIsOpen = false;
    LettuceVillagePopupManager villagePopupManager = LettuceVillagePopupManager.Get();
    if ((UnityEngine.Object) villagePopupManager != (UnityEngine.Object) null)
      villagePopupManager.OnPopupClosed -= new System.Action<LettuceVillagePopupManager.PopupType>(this.OnTaskboardClosed);
    LettuceVillageDataUtil.MarkNotificationAsSeenForBuilding(MercenaryBuilding.Mercenarybuildingtype.TASKBOARD);
  }

  private IEnumerator ShowAndWaitForTaskBoard(
    bool exitWhenComplete,
    UIEvent e,
    int focusedVisitorStateId = 0)
  {
    LettuceMapDisplay lettuceMapDisplay = this;
    yield return (object) new WaitForSeconds(1f);
    LettuceVillagePopupManager villagePopupManager = LettuceVillagePopupManager.Get();
    if ((UnityEngine.Object) villagePopupManager != (UnityEngine.Object) null)
    {
      lettuceMapDisplay.m_taskBoardIsOpen = true;
      villagePopupManager.OnPopupClosed += new System.Action<LettuceVillagePopupManager.PopupType>(lettuceMapDisplay.OnTaskboardClosed);
      villagePopupManager.FocusedVisitorId = focusedVisitorStateId;
      villagePopupManager.Show(LettuceVillagePopupManager.PopupType.TASKBOARD);
      while (lettuceMapDisplay.m_taskBoardIsOpen)
        yield return (object) null;
    }
    if (exitWhenComplete)
      lettuceMapDisplay.OnBackButtonRelease(e);
  }

  private void InitializeLettuceMapData()
  {
    int bountyId = 0;
    long num1 = 0;
    long num2 = 0;
    if (this.m_sceneTransitionPayload != null)
    {
      LettuceBountyDbfRecord selectedBounty = ((LettuceVillageDisplay.LettuceSceneTransitionPayload) this.m_sceneTransitionPayload).m_SelectedBounty;
      bountyId = selectedBounty == null ? 0 : selectedBounty.ID;
      num1 = ((LettuceVillageDisplay.LettuceSceneTransitionPayload) this.m_sceneTransitionPayload).m_TeamId;
      num2 = ((LettuceVillageDisplay.LettuceSceneTransitionPayload) this.m_sceneTransitionPayload).m_CoOpPartnerTeamId;
    }
    if (PartyManager.Get().IsInMercenariesCoOpParty() && !PartyManager.Get().IsPartyLeader())
    {
      BnetGameAccountId leaderGameAccountId = PartyManager.Get().GetPartyLeaderGameAccountId();
      if ((BnetEntityId) leaderGameAccountId == (BnetEntityId) null)
        Log.Lettuce.PrintError("InitializeLettuceMapData - No party leader for co-op map!");
      else
        Network.Get().RequestLettuceMap((uint) bountyId, coopLeaderGameAccountId: leaderGameAccountId);
    }
    else
    {
      NetCache.NetCacheLettuceMap netObject = NetCache.Get().GetNetObject<NetCache.NetCacheLettuceMap>();
      if (netObject != null && netObject.Map != null && (netObject.Map.Active || bountyId == 0) && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY)
      {
        this.InitializeMapDataFromProto(netObject.Map);
      }
      else
      {
        List<LettuceMapPlayerData> playerDataList = new List<LettuceMapPlayerData>();
        if (PartyManager.Get().IsInMercenariesCoOpParty())
        {
          if (PartyManager.Get().GetCurrentPartySize() < 2)
          {
            Log.Lettuce.PrintError("InitializeLettuceMapData - Not enough party members!");
            return;
          }
          BnetId bnetId1 = new BnetId()
          {
            Hi = PartyManager.Get().GetPartyLeaderGameAccountId().High,
            Lo = PartyManager.Get().GetPartyLeaderGameAccountId().Low
          };
          BnetId bnetId2 = new BnetId()
          {
            Hi = PartyManager.Get().GetMembers()[1].GameAccountId.High,
            Lo = PartyManager.Get().GetMembers()[1].GameAccountId.Low
          };
          playerDataList.Add(new LettuceMapPlayerData()
          {
            PlayerId = bnetId1,
            TeamId = num1
          });
          playerDataList.Add(new LettuceMapPlayerData()
          {
            PlayerId = bnetId2,
            TeamId = num2
          });
        }
        else
        {
          BnetId bnetId = new BnetId()
          {
            Hi = BnetPresenceMgr.Get().GetMyGameAccountId().High,
            Lo = BnetPresenceMgr.Get().GetMyGameAccountId().Low
          };
          playerDataList.Add(new LettuceMapPlayerData()
          {
            PlayerId = bnetId,
            TeamId = num1
          });
        }
        this.m_isNewMap = bountyId > 0;
        Network.Get().RequestLettuceMap((uint) bountyId, playerDataList);
      }
    }
  }

  private void OnLettuceMapReceived()
  {
    NetCache.NetCacheLettuceMap netObject = NetCache.Get().GetNetObject<NetCache.NetCacheLettuceMap>();
    if (netObject.Map == null)
      return;
    this.InitializeMapDataFromProto(netObject.Map);
  }

  private void InitializeMapDataFromProto(PegasusLettuce.LettuceMap map)
  {
    this.m_lettuceMapProto = map;
    this.m_lettuceMap.CreateMapFromProto(map);
    LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord((int) map.BountyId);
    LettuceMapDisplayDataModel dataModel = this.GetDisplayDataModel();
    dataModel.FinalBossRewardList = new RewardListDataModel();
    foreach (LettuceBountyFinalRewardsDbfRecord finalBossReward in record.FinalBossRewards)
    {
      string idFromMercenaryId = GameUtils.GetCardIdFromMercenaryId(finalBossReward.RewardMercenaryId);
      EntityDef entityDef = DefLoader.Get().GetEntityDef(idFromMercenaryId);
      RewardItemDataModel rewardItemDataModel = new RewardItemDataModel()
      {
        ItemType = RewardItemType.MERCENARY_COIN,
        MercenaryCoin = new LettuceMercenaryCoinDataModel()
        {
          MercenaryId = finalBossReward.RewardMercenaryId,
          MercenaryName = entityDef.GetName(),
          Quantity = 0,
          GlowActive = false,
          NameActive = true
        }
      };
      dataModel.FinalBossRewardList.Items.Add(rewardItemDataModel);
    }
    dataModel.MapSeed = map.Seed;
    dataModel.RunEnded = !map.Active;
    dataModel.RunLost = !map.Active && !this.m_lettuceMap.IsFinalBossDefeated();
    dataModel.Heroic = record.Heroic;
    dataModel.Tutorial = this.IsCurrentBountyTutorial();
    if (record.BountySetRecord != null)
    {
      dataModel.ZoneIdentifier = record.BountySetRecord.ShortGuid;
      if (!string.IsNullOrEmpty(record.BountySetRecord.WatermarkTexture))
        AssetLoader.Get().LoadTexture((AssetReference) record.BountySetRecord.WatermarkTexture, (ObjectCallback) ((assetRef, obj, callbackData) => dataModel.BountySetWatermark = obj as Texture));
    }
    NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
    if (netObject != null && netObject.BountyInfoMap != null && netObject.BountyInfoMap.ContainsKey(record.ID))
      dataModel.FewestTurns = netObject.BountyInfoMap[record.ID].FewestTurns;
    dataModel.CurrentTurns = (int) map.TurnsTaken;
    foreach (LettuceMapAnomalyAssignment anomalyCard in map.AnomalyCards)
    {
      if (LettuceMapDisplay.GetBonusRewardTypeForCardId(anomalyCard.AnomalyCard) != LettuceMapNodeTypeAnomaly.MercenariesBonusRewardType.NONE)
      {
        dataModel.BonusRewardsActive = true;
        break;
      }
    }
    this.m_currentMapIsComplete = !this.m_lettuceMapProto.Active;
    this.m_currentMapResult = !this.m_isNewMap ? (!this.m_currentMapIsComplete ? LettuceMapDisplay.CurrentResultState.WON_NODE : (this.m_lettuceMap.IsFinalBossDefeated() ? LettuceMapDisplay.CurrentResultState.WON_MAP : LettuceMapDisplay.CurrentResultState.LOST_MAP)) : LettuceMapDisplay.CurrentResultState.NEW_MAP;
    int rowToFocusOn = 0;
    if (this.m_currentMapResult == LettuceMapDisplay.CurrentResultState.WON_MAP)
    {
      this.ScrollMapToRow(rowToFocusOn);
      this.DisplayBossPortraitForCoin(this.m_lettuceMap.GetFinalBossCoinDataModel());
      this.RequestAndUpdateTutorialTeam();
    }
    else
    {
      if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY && !this.m_currentMapIsComplete)
        this.ScrollMapToRow(Mathf.Max(0, this.DetermineRowToFocusOn() - 1));
      else if (this.m_currentMapResult != LettuceMapDisplay.CurrentResultState.NEW_MAP)
        this.ScrollMapToRow(this.DetermineRowToFocusOn());
      if (this.m_currentMapResult == LettuceMapDisplay.CurrentResultState.LOST_MAP)
      {
        LettuceMapCoinDataModel defeatCoinDataModel = this.m_lettuceMap.GetDefeatCoinDataModel();
        if (defeatCoinDataModel != null)
          this.DisplayBossPortraitForCoin(defeatCoinDataModel);
      }
    }
    this.m_lettuceMapDataInitialized = true;
  }

  private bool ShouldShowTreasureSelection(PegasusLettuce.LettuceMap map)
  {
    bool flag = PartyManager.Get().IsInMercenariesCoOpParty() && !PartyManager.Get().IsPartyLeader();
    return map.HasPendingTreasureSelection && map.PendingTreasureSelection.TreasureOptions.Count > 0 && !flag;
  }

  private bool ShouldShowVisitorSelection(PegasusLettuce.LettuceMap map)
  {
    bool flag = PartyManager.Get().IsInMercenariesCoOpParty() && !PartyManager.Get().IsPartyLeader();
    return map.HasPendingVisitorSelection && map.PendingVisitorSelection.VisitorOptions.Count > 0 && !flag;
  }

  private void InitializeTreasureSelectionData(PegasusLettuce.LettuceMap map)
  {
    LettuceMapDisplayDataModel displayDataModel = this.GetDisplayDataModel();
    displayDataModel.TreasureSelectionData = new LettuceTreasureSelectionDataModel();
    displayDataModel.TreasureSelectionData.TreasureOptions = new DataModelList<CardDataModel>();
    foreach (int treasureOption in map.PendingTreasureSelection.TreasureOptions)
      displayDataModel.TreasureSelectionData.TreasureOptions.Add(new CardDataModel()
      {
        CardId = GameUtils.TranslateDbIdToCardId(treasureOption)
      });
    displayDataModel.TreasureSelectionData.MercenaryTreasure = new DataModelList<CardDataModel>();
    LettuceMapPlayerData currentPlayerData = this.GetCurrentPlayerData();
    if (currentPlayerData == null || !currentPlayerData.HasTeamId)
    {
      Log.Lettuce.PrintError("InitializeTreasureSelectionData - no player data or teamId!");
    }
    else
    {
      PegasusLettuce.LettuceTeam teamForPlayer = this.GetTeamForPlayer(currentPlayerData);
      if (teamForPlayer == null)
      {
        Log.Lettuce.PrintError("InitializeTreasureSelectionData - no team found for player!");
      }
      else
      {
        foreach (LettuceTeamMercenary mercenary1 in teamForPlayer.MercenaryList.Mercenaries)
        {
          LettuceTeamMercenary teamMercenary = mercenary1;
          LettuceMercenary mercenary2 = CollectionManager.Get().GetMercenary((long) teamMercenary.MercenaryId);
          displayDataModel.TreasureSelectionData.Mercenaries.Add(MercenaryFactory.CreateMercenaryDataModel(mercenary2.ID, teamMercenary.SelectedArtVariationId, (TAG_PREMIUM) teamMercenary.SelectedArtVariationPremium, mercenary2));
          LettuceMapTreasureAssignment treasureAssignment = (LettuceMapTreasureAssignment) null;
          if (map.TreasureAssignmentList?.TreasureAssignments != null)
            treasureAssignment = map.TreasureAssignmentList.TreasureAssignments.FirstOrDefault<LettuceMapTreasureAssignment>((Func<LettuceMapTreasureAssignment, bool>) (e => e.AssignedMercenary == teamMercenary.MercenaryId));
          if (treasureAssignment != null)
            displayDataModel.TreasureSelectionData.MercenaryTreasure.Add(new CardDataModel()
            {
              CardId = GameUtils.TranslateDbIdToCardId(treasureAssignment.TreasureCard)
            });
          else
            displayDataModel.TreasureSelectionData.MercenaryTreasure.Add(new CardDataModel());
        }
        this.PopulateChoiceMercenaryData(displayDataModel, map);
      }
    }
  }

  private void PopulateChoiceMercenaryData(LettuceMapDisplayDataModel dataModel, PegasusLettuce.LettuceMap map)
  {
    int mercenaryId = map.PendingTreasureSelection.MercenaryId;
    MercenaryDetailed mercenaryDetailed = map.RecruitedMercenaries.FirstOrDefault<MercenaryDetailed>((Func<MercenaryDetailed, bool>) (m => m.Mercenary.AssetId == mercenaryId));
    LettuceMercenaryDataModel dataModel1;
    if (mercenaryDetailed != null)
    {
      dataModel1 = MercenaryFactory.CreateEmptyMercenaryDataModel();
      CollectionUtils.PopulateMercenaryCardDataModel(dataModel1, LettuceMercenary.CreateDefaultArtVariation(mercenaryId));
      dataModel1.MercenaryId = mercenaryDetailed.Mercenary.AssetId;
      dataModel1.MercenaryLevel = GameUtils.GetMercenaryLevelFromExperience((int) mercenaryDetailed.Mercenary.Exp);
      dataModel1.ExperienceInitial = (int) mercenaryDetailed.Mercenary.Exp;
      dataModel1.FullyUpgradedInitial = mercenaryDetailed.IsFullyUpgraded;
      dataModel1.Owned = true;
      CollectionUtils.SetMercenaryStatsByLevel(dataModel1, mercenaryId, dataModel1.MercenaryLevel, mercenaryDetailed.IsFullyUpgraded);
    }
    else
      dataModel1 = dataModel.TreasureSelectionData.Mercenaries.Where<LettuceMercenaryDataModel>((Func<LettuceMercenaryDataModel, bool>) (m => m.MercenaryId == mercenaryId)).FirstOrDefault<LettuceMercenaryDataModel>();
    if (dataModel1 == null)
    {
      Log.Lettuce.PrintError(string.Format("PopulateChoiceMercenaryData - no mercenary {0} found in team or recruit list!", (object) mercenaryId));
    }
    else
    {
      dataModel.TreasureSelectionData.ChoiceMercenary = dataModel1;
      LettuceMapTreasureAssignment treasureAssignment = (LettuceMapTreasureAssignment) null;
      if (map.TreasureAssignmentList?.TreasureAssignments != null)
        treasureAssignment = map.TreasureAssignmentList.TreasureAssignments.Where<LettuceMapTreasureAssignment>((Func<LettuceMapTreasureAssignment, bool>) (e => e.AssignedMercenary == mercenaryId)).FirstOrDefault<LettuceMapTreasureAssignment>();
      if (treasureAssignment != null)
        dataModel.TreasureSelectionData.ChoiceMercenaryTreasure = new CardDataModel()
        {
          CardId = GameUtils.TranslateDbIdToCardId(treasureAssignment.TreasureCard)
        };
      dataModel.TreasureSelectionData.ChoiceMercenaryHasTreasure = treasureAssignment != null;
    }
  }

  private IEnumerator TryShowingVisitorSelection(PegasusLettuce.LettuceMap map)
  {
    if (this.ShouldShowVisitorSelection(map))
    {
      LettuceMapDisplayDataModel displayDataModel = this.GetDisplayDataModel();
      displayDataModel.VisitorSelectionData = new LettuceVisitorSelectionDataModel();
      foreach (LettuceMapVisitorSelectionOption visitorOption in map.PendingVisitorSelection.VisitorOptions)
      {
        int mercenaryDbId = 0;
        if (visitorOption.HasVisitorId)
          mercenaryDbId = LettuceVillageDataUtil.GetMercenaryIdForVisitor(GameDbf.MercenaryVisitor.GetRecord(visitorOption.VisitorId));
        if (mercenaryDbId == 0 && visitorOption.HasFallbackMercenaryId)
          mercenaryDbId = visitorOption.FallbackMercenaryId;
        LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) mercenaryDbId);
        if (mercenary == null)
        {
          Debug.LogError((object) "TryShowingVisitorSelection() - Invalid Mercenary in selection.");
        }
        else
        {
          LettuceMercenaryDataModel mercenaryDataModel = MercenaryFactory.CreateMercenaryDataModel(mercenary);
          mercenaryDataModel.Card.Premium = TAG_PREMIUM.NORMAL;
          displayDataModel.VisitorSelectionData.VisitorOptions.Add(mercenaryDataModel);
          if (visitorOption.HasTaskId)
          {
            VisitorTaskDbfRecord record = GameDbf.VisitorTask.GetRecord(visitorOption.TaskId);
            if (record != null)
            {
              int chainIndexForTask = GameDbf.GetIndex().GetTaskChainIndexForTask(visitorOption.TaskId);
              int taskChainProgress = chainIndexForTask < 0 ? 0 : chainIndexForTask;
              MercenaryVillageTaskItemDataModel taskModel = LettuceVillageDataUtil.CreateTaskModel(record, 0, taskChainProgress, MercenariesTaskState.Status.ACTIVE);
              displayDataModel.VisitorSelectionData.TaskOptions.Add(taskModel);
            }
            else
              Debug.LogError((object) string.Format("TryShowingVisitorSelection() - Invalid Task {0} in selection.", (object) visitorOption.TaskId));
          }
        }
      }
      if (displayDataModel.VisitorSelectionData.VisitorOptions.Count == 0)
      {
        Debug.LogError((object) "TryShowingVisitorSelection() - Was not able to create any visitors choosing default instead");
        Network.Get().MakeMercenariesMapVisitorSelection(0);
      }
      else
      {
        this.m_waitingForVisitorSelectionServerResponse = true;
        while (this.m_waitingForVisitorSelectionServerResponse)
          yield return (object) null;
      }
    }
  }

  private void InitializeBountyDataModel(
    LettuceBountyDataModel dataModel,
    LettuceBountyDbfRecord lettuceBountyRecord)
  {
    Material bossCoinMaterial = (Material) null;
    DefLoader.Get().LoadCardDef(GameUtils.TranslateDbIdToCardId(lettuceBountyRecord.FinalBossCardId), (DefLoader.LoadDefCallback<DefLoader.DisposableCardDef>) ((cardId, def, userData) =>
    {
      if (def == null)
        return;
      bossCoinMaterial = def.CardDef.m_MercenaryMapBossCoinPortrait;
      this.m_disposables.Add((IDisposable) def);
    }));
    if ((UnityEngine.Object) bossCoinMaterial == (UnityEngine.Object) null)
      bossCoinMaterial = AssetLoader.Get().LoadMaterial((AssetReference) "LOE_08CoinPortrait.mat:b5cdfac2e9672f9479083d73014858c6");
    dataModel.BountyId = lettuceBountyRecord.ID;
    dataModel.PosterText = GameStrings.Format("GLUE_LETTUCE_BOUNTY_POSTER_TEXT", (object) lettuceBountyRecord.BountyLevel);
    dataModel.Available = true;
    dataModel.AdventureMission = new AdventureMissionDataModel()
    {
      CoinPortraitMaterial = bossCoinMaterial,
      MissionState = AdventureMissionState.UNLOCKED
    };
  }

  private int DetermineRowToFocusOn()
  {
    if ((UnityEngine.Object) this.m_Scrollable == (UnityEngine.Object) null)
      return 0;
    int rowToFocusOn = 0;
    foreach (LettuceMapNode lettuceMapNode in this.m_lettuceMap.NodeData)
    {
      if (lettuceMapNode.NodeState_ == LettuceMapNode.NodeState.UNLOCKED || lettuceMapNode.NodeState_ == LettuceMapNode.NodeState.DEFEAT)
      {
        rowToFocusOn = (int) lettuceMapNode.Row;
        break;
      }
    }
    return rowToFocusOn;
  }

  private bool ScrollMapToRow(int rowToFocusOn, float tweenTime = 0.0f, System.Action onCompleteCallback = null)
  {
    if ((UnityEngine.Object) this.m_Scrollable == (UnityEngine.Object) null)
      return false;
    int num = 3;
    float percentage = 1f - ((float) rowToFocusOn - 1f) / (float) (this.m_lettuceMap.NumberOfRows - num);
    if ((double) tweenTime == 0.0)
    {
      this.m_Scrollable.SetScrollImmediate(percentage);
      System.Action action = onCompleteCallback;
      if (action != null)
        action();
    }
    else
    {
      if ((double) this.m_Scrollable.ScrollValue == (double) percentage)
      {
        System.Action action = onCompleteCallback;
        if (action != null)
          action();
        return false;
      }
      this.m_Scrollable.SetScroll(percentage, (UIBScrollable.OnScrollComplete) (_ =>
      {
        System.Action action = onCompleteCallback;
        if (action == null)
          return;
        action();
      }), iTween.EaseType.easeInOutCubic, tweenTime, true);
    }
    return true;
  }

  private void OnLettuceMapChooseNodeResponseReceived()
  {
    LettuceMapChooseNodeResponse chooseNodeResponse = Network.Get().GetLettuceMapChooseNodeResponse();
    if (chooseNodeResponse == null)
      Debug.LogError((object) "OnLettuceMapChooseNodeResponseReceived() - No response received.");
    else if (!chooseNodeResponse.Success)
    {
      Debug.LogError((object) "OnLettuceMapChooseNodeResponseReceived() - Choice was not successful!");
    }
    else
    {
      this.m_lettuceMapProto = chooseNodeResponse.UpdatedMap;
      this.StartCoroutine(this.HandleChooseNodeResponseFlowWithTiming(chooseNodeResponse.ChosenNode));
    }
  }

  private void OnLettuceMapRetireResponseReceived()
  {
    LettuceMapRetireResponse mapRetireResponse = Network.Get().GetLettuceMapRetireResponse();
    if (mapRetireResponse == null)
      Debug.LogError((object) "OnLettuceMapRetireResponseReceived() - No response received.");
    else if (!mapRetireResponse.Success)
    {
      Debug.LogError((object) "OnLettuceMapRetireResponseReceived() - Retire was not successful!");
    }
    else
    {
      this.m_lettuceMapProto = mapRetireResponse.UpdatedMap;
      if (!mapRetireResponse.HasReceivedConsolationReward || !mapRetireResponse.ReceivedConsolationReward)
        this.TransitionToBountyBoardAfterRetire();
      this.StartCoroutine(this.DisplayMercenariesConsolationRewardsWhenReady());
    }
  }

  private IEnumerator DisplayMercenariesConsolationRewardsWhenReady()
  {
    LettuceMapDisplay lettuceMapDisplay = this;
    while (!PopupDisplayManager.Get().RewardPopups.HasNonAutoRetireMercenariesRewardsToShow())
      yield return (object) null;
    NetCache.ProfileNoticeMercenariesRewards mercenariesRewardToShow1 = PopupDisplayManager.Get().RewardPopups.GetNextNonAutoRetireRewardMercenariesRewardToShow();
    NetCache.ProfileNoticeMercenariesRewards mercenariesRewardToShow2 = PopupDisplayManager.Get().RewardPopups.GetNextBonusMercenariesRewardToShow();
    if (!PopupDisplayManager.Get().RewardPopups.ShowMercenariesRewards(false, mercenariesRewardToShow1, mercenariesRewardToShow2, new System.Action(lettuceMapDisplay.TransitionToBountyBoardAfterRetire)))
    {
      Log.Lettuce.PrintError("GetMercenariesConsolationRewards() - Could not get Consolation reward!");
      lettuceMapDisplay.TransitionToBountyBoardAfterRetire();
    }
  }

  private void TransitionToBountyBoardAfterRetire()
  {
    NetCache.Get().UnloadNetObject<NetCache.NetCacheLettuceMap>();
    this.GetComponent<VisualController>().SetState("HIDE_TEAM_TRAY");
    if (PartyManager.Get().IsInMercenariesCoOpParty())
      PartyManager.Get().LeaveParty();
    LettuceVillageDisplay.LettuceSceneTransitionPayload transitionPayload = new LettuceVillageDisplay.LettuceSceneTransitionPayload();
    LettuceBountyDbfRecord record = GameDbf.LettuceBounty.GetRecord((int) this.m_lettuceMapProto.BountyId);
    transitionPayload.m_SelectedBounty = record;
    transitionPayload.m_SelectedBountySet = record.BountySetRecord;
    transitionPayload.m_DifficultyMode = record.DifficultyMode;
    this.m_sceneTransitionPayload = (object) transitionPayload;
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_BOUNTY_BOARD, SceneMgr.TransitionHandlerType.NEXT_SCENE, sceneTransitionPayload: this.m_sceneTransitionPayload);
  }

  private void OnMercenariesMapTreasureSelectionResponseReceived()
  {
    MercenariesMapTreasureSelectionResponse selectionResponse = Network.Get().GetMercenariesMapTreasureSelectionResponse();
    if (selectionResponse == null)
      Debug.LogError((object) "OnMercenariesMapTreasureSelectionResponseReceived() - No response received.");
    else if (!selectionResponse.Success)
    {
      Debug.LogError((object) "OnMercenariesMapTreasureSelectionResponseReceived() - Choice was not successful!");
    }
    else
    {
      this.m_lettuceMapProto = selectionResponse.UpdatedMap;
      CollectionUtils.PopulateTeamTreasures(this.GetTeamPreviewDataModel(), this.m_lettuceMapProto.TreasureAssignmentList?.TreasureAssignments);
    }
  }

  private void OnVisitorSelectionResponseReceived()
  {
    MercenariesMapVisitorSelectionResponse selectionResponse = Network.Get().GetMercenariesMapVisitorSelectionResponse();
    if (selectionResponse == null)
    {
      Log.Lettuce.PrintError("OnVisitorSelectionResponse() - No response received.");
      this.m_waitingForVisitorSelectionServerResponse = false;
    }
    else if (!selectionResponse.Success)
    {
      Log.Lettuce.PrintError("OnVisitorSelectionResponse() - Choice was not successful!");
      this.m_waitingForVisitorSelectionServerResponse = false;
    }
    else
    {
      if (selectionResponse.HasReward && selectionResponse.Reward.Components.Count != 0)
      {
        LettuceRewardComponent component = selectionResponse.Reward.Components[0];
        int mercenaryId = component.MercenaryId;
        long amount = component.Amount;
        this.OnVisitorSelectionFallbackReward(mercenaryId, selectionResponse);
      }
      else if (selectionResponse.HasVisitorState)
      {
        this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
        {
          Time = 0.25f
        });
        MercenaryVillageTaskItemDataModel modelFromTaskState = LettuceVillageDataUtil.CreateTaskModelFromTaskState(selectionResponse.VisitorState.ActiveTaskState, selectionResponse.VisitorState);
        Widget rewardWidget = (Widget) WidgetInstance.Create((string) LettuceMapDisplay.VISITOR_TASK_REWARD_PREFAB);
        rewardWidget.BindDataModel((IDataModel) modelFromTaskState);
        rewardWidget.RegisterDoneChangingStatesListener((System.Action<object>) (_ =>
        {
          RewardScroll componentInChildren = rewardWidget.GetComponentInChildren<RewardScroll>();
          componentInChildren.Initialize((System.Action) (() =>
          {
            this.m_waitingForVisitorSelectionServerResponse = false;
            this.m_screenEffectsHandle.StopEffect();
          }));
          componentInChildren.Show();
        }), (object) null, true, true);
      }
      else
        this.m_waitingForVisitorSelectionServerResponse = false;
      this.GetDisplayDataModel().VisitorSelectionData = (LettuceVisitorSelectionDataModel) null;
    }
  }

  private void OnVisitorSelectionFallbackReward(
    int mercenaryId,
    MercenariesMapVisitorSelectionResponse response)
  {
    string idFromMercenaryId = GameUtils.GetCardIdFromMercenaryId(mercenaryId);
    EntityDef entityDef = DefLoader.Get().GetEntityDef(idFromMercenaryId);
    if (entityDef == null)
    {
      Log.Lettuce.PrintError("OnVisitorSelectionFallbackReward - Failed to load def for card {0}", (object) idFromMercenaryId);
      this.m_waitingForVisitorSelectionServerResponse = false;
    }
    else
    {
      long num = 0;
      if (response.HasReward && response.Reward.Components.Count != 0)
        num = response.Reward.Components[0].Amount;
      string str;
      switch (response.FallbackReason_)
      {
        case MercenariesMapVisitorSelectionResponse.FallbackReason.REASON_FULL:
          str = GameStrings.Get("GLUE_LETTUCE_MAP_VISITOR_FALLBACK_FULL");
          break;
        case MercenariesMapVisitorSelectionResponse.FallbackReason.REASON_DUPLICATE_VISITOR:
          str = GameStrings.Get("GLUE_LETTUCE_MAP_VISITOR_FALLBACK_DUPLICATE");
          break;
        default:
          str = GameStrings.Get("GLUE_LETTUCE_MAP_VISITOR_FALLBACK_NO_TASKS");
          break;
      }
      this.GetDisplayDataModel();
      RewardScrollDataModel rewardScrollDataModel = new RewardScrollDataModel()
      {
        DisplayName = GameStrings.Get("GLUE_LETTUCE_MAP_VISITOR_FALLBACK_REWARD_TITLE"),
        Description = str,
        RewardList = new RewardListDataModel()
        {
          Items = new DataModelList<RewardItemDataModel>()
          {
            new RewardItemDataModel()
            {
              ItemType = RewardItemType.MERCENARY_COIN,
              MercenaryCoin = new LettuceMercenaryCoinDataModel()
              {
                MercenaryId = mercenaryId,
                MercenaryName = entityDef.GetName(),
                Quantity = (int) num,
                GlowActive = true,
                NameActive = true
              }
            }
          }
        }
      };
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = 0.25f
      });
      Widget rewardWidget = (Widget) WidgetInstance.Create((string) LettuceMapDisplay.VISITOR_FALLBACK_REWARD_PREFAB);
      rewardWidget.BindDataModel((IDataModel) rewardScrollDataModel);
      rewardWidget.RegisterDoneChangingStatesListener((System.Action<object>) (_ =>
      {
        RewardScroll componentInChildren = rewardWidget.GetComponentInChildren<RewardScroll>();
        componentInChildren.Initialize((System.Action) (() =>
        {
          this.m_waitingForVisitorSelectionServerResponse = false;
          this.m_screenEffectsHandle.StopEffect();
        }));
        componentInChildren.Show();
      }), (object) null, true, true);
    }
  }

  private void OnTransitionFromGameplayFinished(bool cutoff, object userData)
  {
    LoadingScreen.Get().UnregisterFinishedTransitionListener(new LoadingScreen.FinishedTransitionCallback(this.OnTransitionFromGameplayFinished));
    this.m_loadingScreenTransitionFromGameplayComplete = true;
  }

  private void OnLoadingScreenFadeInStarted() => this.m_loadingScreenTransitionFromGameplayStarted = true;

  private void OnCoinSelected()
  {
    if (PartyManager.Get().IsInMercenariesCoOpParty() && !PartyManager.Get().IsPartyLeader() || !this.m_lettuceMapDataInitialized || this.m_currentMapIsComplete)
      return;
    EventDataModel mapEventDataModel = this.GetLettuceMapEventDataModel();
    if (mapEventDataModel == null)
    {
      Log.All.PrintError("No event data model attached to the LettuceMapDisplay.");
    }
    else
    {
      LettuceMapCoinDataModel payload = (LettuceMapCoinDataModel) mapEventDataModel.Payload;
      if (payload.CoinState == LettuceMapNode.NodeState.UNLOCKED)
        LettuceTutorialUtils.FireEvent(LettuceTutorialVo.LettuceTutorialEvent.MAP_ACTIVE_COIN_RELEASED, this.gameObject, payload.NodeTypeId, (int) this.m_lettuceMapProto.BountyId);
      this.SelectCoinInternal(payload);
    }
  }

  private void SelectCoinInternal(LettuceMapCoinDataModel selectedCoin)
  {
    this.m_selectedMapCoin = selectedCoin;
    this.m_lettuceMap.SelectCoin(this.m_selectedMapCoin);
    this.SetPlayButtonText();
    this.DisplayBossPortraitForCoin(selectedCoin);
    if (PartyManager.Get().IsInMercenariesCoOpParty())
    {
      if (!PartyManager.Get().IsPartyLeader())
        return;
      PartyManager.Get().SetSelectedMercenariesCoOpMapNodeId(this.m_selectedMapCoin.Id);
    }
    this.TryEnablePlayButton();
    if (!GameDbf.LettuceMapNodeType.GetRecord(this.m_selectedMapCoin.NodeTypeId).AutoPlay)
      return;
    this.ExecutePlayLogic();
  }

  private void TryAutoNextSelectCoin()
  {
    List<LettuceMapCoinDataModel> unlockedCoinDataModels = this.m_lettuceMap.GetUnlockedCoinDataModels();
    if (unlockedCoinDataModels.Count != 1)
      return;
    this.SelectCoinInternal(unlockedCoinDataModels.FirstOrDefault<LettuceMapCoinDataModel>());
  }

  private void DisplayBossPortraitForCoin(LettuceMapCoinDataModel selectedCoin)
  {
    string cardIdFromNodeId = this.GetBossCardIdFromNodeId(selectedCoin.Id);
    if (string.IsNullOrEmpty(cardIdFromNodeId))
      return;
    EntityDef entityDef = DefLoader.Get().GetEntityDef(cardIdFromNodeId);
    LettuceMapDisplayDataModel displayDataModel = this.GetDisplayDataModel();
    displayDataModel.BossCard = new CardDataModel()
    {
      CardId = cardIdFromNodeId
    };
    displayDataModel.BossName = entityDef.GetName();
  }

  private void OnTreasureSelected()
  {
    EventDataModel mapEventDataModel = this.GetLettuceMapEventDataModel();
    if (mapEventDataModel == null)
      Log.All.PrintError("No event data model attached to the LettuceMapDisplay.");
    else
      this.m_selectedTreasureIndex = Convert.ToInt32(mapEventDataModel.Payload);
  }

  private void OnTreasureChosen()
  {
    if (!Network.IsLoggedIn())
      DialogManager.Get().ShowReconnectHelperDialog(new System.Action(this.OnTreasureChosen), (System.Action) (() => this.BackOutOfScene(SceneMgr.Mode.LETTUCE_VILLAGE)));
    this.m_waitingForTreasureSelection = false;
    if (this.m_selectedTreasureIndex < 0)
      Log.Lettuce.PrintError("OnTreasureChosen() - No treasure selected!");
    else if (this.m_lettuceMapProto.PendingTreasureSelection == null)
    {
      Log.Lettuce.PrintError("OnTreasureChosen() - No pending treasure selection!");
    }
    else
    {
      this.m_screenEffectsHandle.StopEffect();
      Network.Get().MakeMercenariesMapTreasureSelection(this.m_selectedTreasureIndex);
      this.GetDisplayDataModel().TreasureSelectionData = (LettuceTreasureSelectionDataModel) null;
      NetCache.ProfileNoticeMercenariesRewards mercenariesRewardToShow1 = PopupDisplayManager.Get().RewardPopups.GetNextNonAutoRetireRewardMercenariesRewardToShow();
      NetCache.ProfileNoticeMercenariesRewards mercenariesRewardToShow2 = PopupDisplayManager.Get().RewardPopups.GetNextBonusMercenariesRewardToShow();
      if (PopupDisplayManager.Get().RewardPopups.ShowMercenariesRewards(true, mercenariesRewardToShow1, mercenariesRewardToShow2, new System.Action(this.OnTreasureChosenMercenaryRewardsComplete)))
        return;
      this.OnTreasureChosenMercenaryRewardsComplete();
    }
  }

  private void OnTreasureChosenMercenaryRewardsComplete() => PopupDisplayManager.Get().RewardPopups.ShowMercenariesFullyUpgraded();

  private void OnVisitorSelected()
  {
    EventDataModel mapEventDataModel = this.GetLettuceMapEventDataModel();
    if (mapEventDataModel == null)
      Log.All.PrintError("No event data model attached to the LettuceMapDisplay.");
    else
      this.m_selectedVisitorIndex = Convert.ToInt32(mapEventDataModel.Payload);
  }

  private void OnVisitorChosen()
  {
    if (!Network.IsLoggedIn())
      DialogManager.Get().ShowReconnectHelperDialog(new System.Action(this.OnVisitorChosen), (System.Action) (() => this.BackOutOfScene(SceneMgr.Mode.LETTUCE_VILLAGE)));
    if (this.m_selectedVisitorIndex < 0)
      Log.Lettuce.PrintError("OnVisitorChosen() - No visitor selected!");
    else if (this.m_lettuceMapProto.PendingVisitorSelection == null)
    {
      Log.Lettuce.PrintError("OnVisitorChosen() - No pending visitor selection!");
    }
    else
    {
      this.m_screenEffectsHandle.StopEffect();
      Network.Get().MakeMercenariesMapVisitorSelection(this.m_selectedVisitorIndex);
    }
  }

  private void OnTeamViewShow()
  {
    this.m_isTeamViewVisible = true;
    this.m_playButton.Disable();
  }

  private void OnTeamViewHide()
  {
    this.m_isTeamViewVisible = false;
    this.TryEnablePlayButton();
  }

  private void SetPlayButtonText()
  {
    LettuceMapNodeTypeDbfRecord record = GameDbf.LettuceMapNodeType.GetRecord(this.m_selectedMapCoin.NodeTypeId);
    if (record.PlayButtonText != null && !string.IsNullOrWhiteSpace(record.PlayButtonText.GetString()))
      this.m_playButton.SetText(record.PlayButtonText.GetString());
    else
      this.m_playButton.SetText(GameStrings.Get("GLOBAL_PLAY"));
  }

  private bool ShouldEnablePlayButton()
  {
    if (this.m_isTeamViewVisible || this.m_selectedMapCoin == null)
      return false;
    LettuceMapNodeTypeDbfRecord record = GameDbf.LettuceMapNodeType.GetRecord(this.m_selectedMapCoin.NodeTypeId);
    if (this.m_selectedMapCoin.CoinState == LettuceMapNode.NodeState.UNLOCKED)
      return true;
    return this.m_selectedMapCoin.CoinState == LettuceMapNode.NodeState.COMPLETE && record.Repeatable;
  }

  private void TryEnablePlayButton()
  {
    if (this.ShouldEnablePlayButton())
      this.m_playButton.Enable();
    else
      this.m_playButton.Disable();
  }

  private string GetBossCardIdFromNodeId(int nodeId)
  {
    if (this.m_lettuceMapProto == null)
    {
      Debug.LogError((object) "GetBossCardIdFromNodeId called before the proto has been received!");
      return (string) null;
    }
    LettuceMapNode lettuceMapNode = this.m_lettuceMapProto.Nodes.Find((Predicate<LettuceMapNode>) (n => (long) n.NodeId == (long) nodeId));
    if (lettuceMapNode == null)
    {
      Debug.LogErrorFormat("GetBossCardIdFromNodeId - Node {0} not found in the proto!", (object) nodeId);
      return (string) null;
    }
    if (lettuceMapNode.BossCard.Asset != 0)
      return GameUtils.TranslateDbIdToCardId(lettuceMapNode.BossCard.Asset);
    Debug.LogErrorFormat("GetBossCardIdFromNodeId - Node {0} has no boss card set!", (object) nodeId);
    return (string) null;
  }

  private LettuceMapPlayerData GetCurrentPlayerData()
  {
    if (this.m_lettuceMapProto == null)
    {
      Log.Lettuce.PrintError("GetCurrentPlayerData - No map proto.");
      return (LettuceMapPlayerData) null;
    }
    if (this.m_lettuceMapProto.PlayerData == null || this.m_lettuceMapProto.PlayerData.Count == 0)
    {
      Log.Lettuce.PrintError("GetCurrentPlayerData - No player data in map.");
      return (LettuceMapPlayerData) null;
    }
    if (!PartyManager.Get().IsInMercenariesCoOpParty() || PartyManager.Get().IsPartyLeader())
      return this.m_lettuceMapProto.PlayerData.FirstOrDefault<LettuceMapPlayerData>();
    if (this.m_lettuceMapProto.PlayerData.Count >= 2)
      return this.m_lettuceMapProto.PlayerData[1];
    Log.Lettuce.PrintError("GetCurrentPlayerData - No co-op partner in map.");
    return (LettuceMapPlayerData) null;
  }

  private PegasusLettuce.LettuceTeam GetTeamForPlayer(LettuceMapPlayerData playerData)
  {
    if (this.m_lettuceMapProto == null)
    {
      Log.Lettuce.PrintError("GetTeamForPlayer - No map proto.");
      return (PegasusLettuce.LettuceTeam) null;
    }
    if (playerData == null)
    {
      Log.Lettuce.PrintError("GetTeamForPlayer - No playerData.");
      return (PegasusLettuce.LettuceTeam) null;
    }
    if (this.m_lettuceMapProto.TeamData == null || this.m_lettuceMapProto.TeamData.Count == 0)
    {
      Log.Lettuce.PrintError("GetTeamForPlayer - No team data in map.");
      return (PegasusLettuce.LettuceTeam) null;
    }
    foreach (PegasusLettuce.LettuceTeam teamForPlayer in this.m_lettuceMapProto.TeamData)
    {
      if (teamForPlayer.HasTeamId && teamForPlayer.TeamId == playerData.TeamId)
        return teamForPlayer;
    }
    return (PegasusLettuce.LettuceTeam) null;
  }

  private void PopulateTeamPreviewData(LettuceMapPlayerData playerData)
  {
    LettuceTeamDataModel previewDataModel = this.GetTeamPreviewDataModel();
    if (playerData == null)
    {
      Log.Lettuce.PrintError("PopulateTeamPreviewData - Unable to retrieve playerData");
    }
    else
    {
      PegasusLettuce.LettuceTeam teamForPlayer = this.GetTeamForPlayer(playerData);
      if (teamForPlayer == null)
      {
        Log.Lettuce.PrintError("PopulateTeamPreviewData - Unable to retrieve team");
      }
      else
      {
        int index = 2;
        if (PartyManager.Get().IsInMercenariesCoOpParty() && !PartyManager.Get().IsPartyLeader())
          index = 3;
        if (this.m_lettuceMapProto.DeadMercenaries.Count < index)
        {
          Log.Lettuce.PrintError(string.Format("PopulateTeamPreviewData - Unable to retrieve dead mercenaries for index={0}", (object) index));
        }
        else
        {
          previewDataModel.TeamId = teamForPlayer.TeamId;
          previewDataModel.TeamName = teamForPlayer.Name;
          LettuceTeam team = LettuceTeam.Convert(teamForPlayer);
          CollectionUtils.PopulateTeamPreviewData(previewDataModel, team, this.m_lettuceMapProto.DeadMercenaries[index].MercenaryIds, true);
          CollectionUtils.PopulateTeamTreasures(previewDataModel, this.m_lettuceMapProto.TreasureAssignmentList?.TreasureAssignments);
        }
      }
    }
  }

  private IEnumerator PlayIntroMapScroll()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    LettuceMapDisplay lettuceMapDisplay = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      // ISSUE: reference to a compiler-generated method
      lettuceMapDisplay.ScrollMapToRow(lettuceMapDisplay.DetermineRowToFocusOn(), 2f, new System.Action(lettuceMapDisplay.\u003CPlayIntroMapScroll\u003Eb__138_0));
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) new WaitForSeconds(0.25f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private IEnumerator PlayIntroMapScroll_OnScrollFinished()
  {
    yield return (object) new WaitForSeconds(0.25f);
    this.m_lettuceMap.FlipUnlockedCoins();
    this.TryAutoNextSelectCoin();
    yield return (object) this.WaitForTutorialEvent(LettuceTutorialVo.LettuceTutorialEvent.MAP_STARTED, 0);
  }

  private IEnumerator PlayDefaultMapScroll()
  {
    if (this.ScrollMapToRow(this.DetermineRowToFocusOn(), 0.5f, new System.Action(this.m_lettuceMap.FlipUnlockedCoins)))
      yield return (object) new WaitForSeconds(0.5f);
    if (this.m_currentMapResult != LettuceMapDisplay.CurrentResultState.LOST_MAP)
    {
      yield return (object) new WaitForSeconds(1f);
      this.TryAutoNextSelectCoin();
    }
  }

  private IEnumerator PlayVictoryMapScroll()
  {
    this.m_Scrollable.SetScrollImmediate(1f);
    while (SceneMgr.Get().IsTransitionNowOrPending())
      yield return (object) null;
    yield return (object) new WaitForSeconds(0.25f);
    this.m_Scrollable.SetScroll(0.0f, iTween.EaseType.easeInOutCubic, 2f, true);
    List<LettuceMapCoin> completedCoins = this.m_lettuceMap.GetCompletedCoins();
    foreach (LettuceMapCoin lettuceMapCoin in completedCoins)
    {
      lettuceMapCoin.FlashCheckMark();
      yield return (object) new WaitForSeconds(2f / (float) completedCoins.Count);
    }
    this.m_finalBossChestVisualController.SetState("OPEN_REWARD");
    yield return (object) new WaitForSeconds(3f);
    NetCache.ProfileNoticeMercenariesRewards mercenariesRewardToShow1 = PopupDisplayManager.Get().RewardPopups.GetNextNonAutoRetireRewardMercenariesRewardToShow();
    NetCache.ProfileNoticeMercenariesRewards mercenariesRewardToShow2 = PopupDisplayManager.Get().RewardPopups.GetNextBonusMercenariesRewardToShow();
    bool popupDone = !PopupDisplayManager.Get().RewardPopups.ShowMercenariesRewards(true, mercenariesRewardToShow1, mercenariesRewardToShow2, new System.Action(OnMercenariesRewardsPopupHidden));
    while (!popupDone)
      yield return (object) null;
    popupDone = !PopupDisplayManager.Get().RewardPopups.ShowMercenariesFullyUpgraded(new System.Action(OnMercenariesRewardsPopupHidden));
    while (!popupDone)
      yield return (object) null;

    void OnMercenariesRewardsPopupHidden() => popupDone = true;
  }

  private void OnPartyChanged(
    PartyManager.PartyInviteEvent inviteEvent,
    BnetGameAccountId playerGameAccountId,
    PartyManager.PartyData challengeData,
    object userData)
  {
    Log.Party.PrintDebug("LettuceCoOpDisplay.OnPartyChanged(): Event={0}, gameAccountId={1}", (object) inviteEvent, (object) playerGameAccountId);
    switch (inviteEvent)
    {
      case PartyManager.PartyInviteEvent.I_RESCINDED_INVITE:
      case PartyManager.PartyInviteEvent.FRIEND_DECLINED_INVITE:
      case PartyManager.PartyInviteEvent.INVITE_EXPIRED:
      case PartyManager.PartyInviteEvent.FRIEND_LEFT:
      case PartyManager.PartyInviteEvent.LEADER_DISSOLVED_PARTY:
        DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_LETTUCE_PARTY_DISBANDED_HEADER"),
          m_text = GameStrings.Get("GLUE_LETTUCE_PARTY_DISBANDED_BODY"),
          m_iconSet = AlertPopup.PopupInfo.IconSet.Default,
          m_showAlertIcon = false,
          m_alertTextAlignment = UberText.AlignmentOptions.Center,
          m_responseDisplay = AlertPopup.ResponseDisplay.OK,
          m_okText = GameStrings.Get("GLOBAL_OKAY")
        });
        SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.NEXT_SCENE, sceneTransitionPayload: this.m_sceneTransitionPayload);
        break;
    }
  }

  private void OnPartyAttributeChanged(Blizzard.GameService.Protocol.V2.Client.Attribute attribute, object userData)
  {
    if (!(attribute.Name == "node_id") || !attribute.Value.HasIntValue || PartyManager.Get().IsPartyLeader())
      return;
    LettuceMapCoinDataModel coinDataModelById = this.m_lettuceMap.GetCoinDataModelById((int) attribute.Value.IntValue);
    if (coinDataModelById == null)
      Log.Lettuce.PrintError("OnPartyAttributeChanged - Invalid map node id={0} in attributes.", (object) attribute.Value.IntValue);
    this.SelectCoinInternal(coinDataModelById);
  }

  private bool CheckAndEnqueueMercenaryGrant()
  {
    LettuceMapCoinDataModel completedCoinDataModel = this.m_lettuceMap.GetLastCompletedCoinDataModel();
    if (completedCoinDataModel == null)
      return false;
    LettuceMapNodeTypeDbfRecord record = GameDbf.LettuceMapNodeType.GetRecord(completedCoinDataModel.NodeTypeId);
    if (record == null || record.GrantMercenary <= 0)
      return false;
    LettuceMercenary mercenary = CollectionManager.Get().GetMercenary((long) record.GrantMercenary);
    if (mercenary == null)
    {
      Log.Lettuce.PrintError("CheckAndEnqueueMercenaryGrant - Unable to get mercenary {0}", (object) record.GrantMercenary);
      return false;
    }
    mercenary.m_owned = true;
    this.m_rewardPresenter.EnqueueReward(new RewardScrollDataModel()
    {
      DisplayName = GameStrings.Get("GLUE_LETTUCE_MERCENARY_REWARD_TITLE"),
      Description = GameStrings.Get("GLUE_LETTUCE_MERCENARY_REWARD_DESC"),
      RewardList = new RewardListDataModel()
      {
        Items = new DataModelList<RewardItemDataModel>()
        {
          new RewardItemDataModel()
          {
            Quantity = 1,
            ItemType = RewardItemType.MERCENARY,
            Mercenary = MercenaryFactory.CreateMercenaryDataModel(mercenary)
          }
        }
      }
    }, (System.Action) (() => { }));
    return true;
  }

  private IEnumerator DisplayNewlyGrantedAnomalyCards(
    PegasusLettuce.LettuceMap lettuceMap,
    int completedNodeId)
  {
    LettuceMapDisplay lettuceMapDisplay = this;
    if (lettuceMap == null)
    {
      Log.Lettuce.PrintError("DisplayNewlyGrantedAnomalyCards - null map proto was provided.");
    }
    else
    {
      int num = 0;
      foreach (LettuceMapAnomalyAssignment anomalyCard in lettuceMap.AnomalyCards)
      {
        if (anomalyCard.SourceNodeId == completedNodeId)
          num = anomalyCard.AnomalyCard;
      }
      if (num != 0)
      {
        if (LettuceMapDisplay.GetBonusRewardTypeForCardId(num) != LettuceMapNodeTypeAnomaly.MercenariesBonusRewardType.NONE)
        {
          LettuceMapDisplayDataModel displayDataModel = lettuceMapDisplay.GetDisplayDataModel();
          if (displayDataModel != null)
            displayDataModel.BonusRewardsActive = true;
        }
        Vector3 spacePositionOfCoin = lettuceMapDisplay.m_lettuceMap.GetWorldSpacePositionOfCoin(completedNodeId);
        LettuceMapAnomalyGrantDataModel anomalyGrantDataModel = new LettuceMapAnomalyGrantDataModel()
        {
          GrantedCard = new CardDataModel()
          {
            CardId = GameUtils.TranslateDbIdToCardId(num),
            Premium = TAG_PREMIUM.NORMAL
          },
          SourceNodePosition = new DataModelList<float>()
          {
            spacePositionOfCoin.x,
            spacePositionOfCoin.y,
            spacePositionOfCoin.z
          }
        };
        SendEventUpwardStateAction.SendEventUpward(lettuceMapDisplay.gameObject, "ANOMALY_GRANTED_FROM_CODE", new EventDataModel()
        {
          Payload = (object) anomalyGrantDataModel
        });
        lettuceMapDisplay.m_waitingForVisualControllerState = true;
        while (lettuceMapDisplay.m_waitingForVisualControllerState)
          yield return (object) null;
      }
    }
  }

  private IEnumerator HandleChooseNodeResponseFlowWithTiming(int chosenNodeId)
  {
    LettuceMapDisplay lettuceMapDisplay = this;
    lettuceMapDisplay.m_clickBlocker.SetActive(true);
    yield return (object) lettuceMapDisplay.HandleVisitResponseByNodeType(lettuceMapDisplay.GetNodeTypeRecordFromNodeId(chosenNodeId));
    yield return (object) lettuceMapDisplay.DisplayNewlyGrantedAnomalyCards(lettuceMapDisplay.m_lettuceMapProto, chosenNodeId);
    yield return (object) lettuceMapDisplay.TryShowingVisitorSelection(lettuceMapDisplay.m_lettuceMapProto);
    lettuceMapDisplay.m_lettuceMap.RefreshWithNewData(lettuceMapDisplay.m_lettuceMapProto);
    lettuceMapDisplay.CheckAndEnqueueMercenaryGrant();
    lettuceMapDisplay.PopulateTeamPreviewData(lettuceMapDisplay.GetCurrentPlayerData());
    lettuceMapDisplay.ScrollMapToRow(lettuceMapDisplay.DetermineRowToFocusOn(), 0.5f);
    yield return (object) new WaitForSeconds(0.5f);
    lettuceMapDisplay.m_lettuceMap.FlipUnlockedCoins();
    yield return (object) new WaitForSeconds(1f);
    lettuceMapDisplay.TryAutoNextSelectCoin();
    yield return (object) lettuceMapDisplay.CheckLastCompletedNodeTutorialEvents();
    yield return (object) lettuceMapDisplay.CheckForNodeDialogueEvents();
    lettuceMapDisplay.m_clickBlocker.SetActive(false);
  }

  private IEnumerator HandleVisitResponseByNodeType(
    LettuceMapNodeTypeDbfRecord nodeTypeRecord)
  {
    LettuceMapDisplay lettuceMapDisplay = this;
    switch (nodeTypeRecord.VisitLogic)
    {
      case LettuceMapNodeType.Visitlogictype.HEAL_TEAM:
        SendEventUpwardStateAction.SendEventUpward(lettuceMapDisplay.gameObject, "PLAY_SPIRIT_HEALER_FX");
        lettuceMapDisplay.m_waitingForVisualControllerState = true;
        while (lettuceMapDisplay.m_waitingForVisualControllerState)
          yield return (object) null;
        break;
      case LettuceMapNodeType.Visitlogictype.SKIP_TO_FINAL_BOSS:
        SendEventUpwardStateAction.SendEventUpward(lettuceMapDisplay.gameObject, "PLAY_PORTAL_FX");
        lettuceMapDisplay.m_waitingForVisualControllerState = true;
        while (lettuceMapDisplay.m_waitingForVisualControllerState)
          yield return (object) null;
        break;
      case LettuceMapNodeType.Visitlogictype.REASSIGN_MAP_ROLE:
        SendEventUpwardStateAction.SendEventUpward(lettuceMapDisplay.gameObject, "PLAY_ROLE_RUSH_FX");
        lettuceMapDisplay.m_waitingForVisualControllerState = true;
        while (lettuceMapDisplay.m_waitingForVisualControllerState)
          yield return (object) null;
        break;
    }
  }

  private IEnumerator WaitForTutorialEvent(
    LettuceTutorialVo.LettuceTutorialEvent tutorialEvent,
    int nodeTypeId)
  {
    LettuceMapDisplay lettuceMapDisplay = this;
    bool done = false;
    LettuceTutorialUtils.FireEvent(tutorialEvent, lettuceMapDisplay.gameObject, nodeTypeId, (int) lettuceMapDisplay.m_lettuceMapProto.BountyId, (System.Action) (() => done = true));
    while (!done)
      yield return (object) null;
  }

  private IEnumerator CheckForNodeDialogueEvents()
  {
    foreach (LettuceMapCoinDataModel unlockedCoinDataModel in this.m_lettuceMap.GetUnlockedCoinDataModels())
      yield return (object) this.WaitForTutorialEvent(LettuceTutorialVo.LettuceTutorialEvent.MAP_NODE_COMPLETED_PRE_REWARDS, unlockedCoinDataModel.NodeTypeId);
    LettuceMapCoinDataModel completedCoinDataModel = this.m_lettuceMap.GetLastCompletedCoinDataModel();
    if (completedCoinDataModel != null)
      yield return (object) this.WaitForTutorialEvent(LettuceTutorialVo.LettuceTutorialEvent.MAP_NODE_REVEALED, completedCoinDataModel.NodeTypeId);
  }

  private IEnumerator CheckLastCompletedNodeTutorialEvents()
  {
    LettuceMapCoinDataModel completedCoinDataModel = this.m_lettuceMap.GetLastCompletedCoinDataModel();
    if (completedCoinDataModel != null)
    {
      int lastNodeTypeId = completedCoinDataModel.NodeTypeId;
      yield return (object) this.WaitForTutorialEvent(LettuceTutorialVo.LettuceTutorialEvent.MAP_NODE_COMPLETED_PRE_MERC_GRANT, lastNodeTypeId);
      bool rewardDone = !this.m_rewardPresenter.ShowNextReward((System.Action) (() => rewardDone = true));
      while (!rewardDone)
        yield return (object) null;
      yield return (object) this.WaitForTutorialEvent(LettuceTutorialVo.LettuceTutorialEvent.MAP_NODE_COMPLETED_POST_MERC_GRANT, lastNodeTypeId);
    }
  }

  private void RequestAndUpdateTutorialTeam()
  {
    if (!this.IsCurrentBountyTutorial())
    {
      Network.Get().MercenariesTeamListRequest();
    }
    else
    {
      NetCache.Get().RegisterUpdatedListener(typeof (LettuceTeamList), new System.Action(this.RequestAndUpdateTutorialTeam_OnMercenariesTeamListResponse));
      Network.Get().MercenariesTeamListRequest();
    }
  }

  private void RequestAndUpdateTutorialTeam_OnMercenariesTeamListResponse()
  {
    NetCache.Get().RemoveUpdatedListener(typeof (LettuceTeamList), new System.Action(this.RequestAndUpdateTutorialTeam_OnMercenariesTeamListResponse));
    if (this.m_lettuceMapProto.PlayerData.Count == 0)
    {
      Log.Lettuce.PrintError("RequestAndUpdateTutorialTeam - Player not found in map.");
    }
    else
    {
      LettuceMapPlayerData lettuceMapPlayerData = this.m_lettuceMapProto.PlayerData.FirstOrDefault<LettuceMapPlayerData>();
      LettuceTeam team = CollectionManager.Get().GetTeam(lettuceMapPlayerData.TeamId);
      if (team == null)
      {
        Log.Lettuce.PrintError("RequestAndUpdateTutorialTeam - Team not found! Team!d={0}", (object) lettuceMapPlayerData.TeamId);
      }
      else
      {
        team.Name = GameStrings.Get("GLUE_LETTUCE_MERCENARY_TUTORIAL_TEAM_NAME");
        team.SendChanges();
      }
    }
  }

  private bool IsCurrentBountyTutorial() => this.m_lettuceMapProto != null && LettuceVillageDataUtil.IsBountyTutorial(GameDbf.LettuceBounty.GetRecord((int) this.m_lettuceMapProto.BountyId));

  private LettuceMapNodeTypeDbfRecord GetNodeTypeRecordFromNodeId(
    int nodeId)
  {
    if (this.m_lettuceMapProto == null)
      return (LettuceMapNodeTypeDbfRecord) null;
    foreach (LettuceMapNode node in this.m_lettuceMapProto.Nodes)
    {
      if ((long) node.NodeId == (long) nodeId)
        return GameDbf.LettuceMapNodeType.GetRecord((int) node.NodeTypeId);
    }
    return (LettuceMapNodeTypeDbfRecord) null;
  }

  private void ExecutePlayLogic()
  {
    if (this.m_selectedMapCoin == null)
      Debug.LogError((object) "OnPlayButtonRelease() - No coin selected!");
    else if (!Network.IsLoggedIn())
    {
      DialogManager.Get().ShowReconnectHelperDialog();
    }
    else
    {
      if ((UnityEngine.Object) this.m_mapMaskable != (UnityEngine.Object) null)
        this.m_mapMaskable.enabled = false;
      LettuceMapNodeTypeDbfRecord record = GameDbf.LettuceMapNodeType.GetRecord(this.m_selectedMapCoin.NodeTypeId);
      if (record.UsesGameplayScene)
      {
        this.m_playButton.Disable();
        int missionId = 3790;
        if (record.ScenarioOverride != 0)
          missionId = record.ScenarioOverride;
        if (PartyManager.Get().IsInMercenariesCoOpParty())
          PartyManager.Get().FindGame();
        else
          GameMgr.Get().FindGame(GameType.GT_MERCENARIES_PVE, PegasusShared.FormatType.FT_WILD, missionId, lettuceMapNodeId: new int?(this.m_selectedMapCoin.Id));
      }
      else if (record.VisitLogic == LettuceMapNodeType.Visitlogictype.VIEW_TASK_LIST)
      {
        LettuceVillagePopupManager villagePopupManager = LettuceVillagePopupManager.Get();
        villagePopupManager.OnPopupClosed += new System.Action<LettuceVillagePopupManager.PopupType>(this.OnTaskboardClosed);
        villagePopupManager.Show(LettuceVillagePopupManager.PopupType.TASKBOARD);
      }
      else
      {
        this.m_playButton.Disable();
        Network.Get().ChooseLettuceMapNode((uint) this.m_selectedMapCoin.Id);
        this.m_clickBlocker.SetActive(true);
      }
    }
  }

  private enum CurrentResultState
  {
    NEW_MAP,
    LOST_MAP,
    WON_NODE,
    WON_MAP,
  }
}
