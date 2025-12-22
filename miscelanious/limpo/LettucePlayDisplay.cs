using Assets;
using Hearthstone.DataModels;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LettucePlayDisplay : AbsSceneDisplay, IMercDetailsDisplayProvider
{
  private static List<MercenariesRankedSeasonRewardRankDbfRecord> s_sortedRewardRecords;
  public AsyncReference m_PlayButtonReference;
  public AsyncReference m_PlayButtonPhoneReference;
  public AsyncReference m_BackButtonReference;
  public AsyncReference m_BackButtonPhoneReference;
  public AsyncReference m_collectionButtonReference;
  public AsyncReference m_TeamListDisplay;
  public AsyncReference m_TeamPreviewReference;
  public AsyncReference m_TeamPreviewPhoneReference;
  public AsyncReference m_TeamDisplayTrayMobileReference;
  public AsyncReference m_RewardChestReference;
  public AsyncReference m_RewardChestPhoneReference;
  public AsyncReference m_MercDetailsDisplayReference;
  public Vector3 m_RewardListPopupLocalPosition = new Vector3(0.0f, 100f, -3f);
  public float m_RewardListPopupLocalScale = 13.5f;
  public float m_delayBeforeChestAnimation = 2f;
  public float m_chestAnimationTime = 3f;
  private PlayButton m_playButton;
  private UIBButton m_backButton;
  private UIBButton m_collectionButton;
  private VisualController m_teamListVisualController;
  private Widget m_teamPreviewWidget;
  private VisualController m_teamDisplayTray;
  private VisualController m_rewardChestVisualController;
  private WidgetInstance m_seasonRewardsPopup;
  private bool m_showingRewardsPopup;
  private int m_highRatingTierIndex;
  private bool m_playButtonFinishedLoading;
  private bool m_backButtonFinishedLoading;
  private bool m_collectionButtonFinishedLoading;
  private bool m_teamListDisplayFinishedLoading;
  private bool m_teamPreviewFinishedLoading;
  private bool m_teamDisplayTrayFinishedLoading;
  private bool m_rewardChestFinishedLoading;
  private bool m_playerInfoReceived;
  private LettuceTeam m_selectedTeam;
  private LettuceTeamDataModel m_selectedTeamDataModel;
  private bool m_blockingPopupDisplayManager;

  public static List<MercenariesRankedSeasonRewardRankDbfRecord> SortedRewardRecords
  {
    get
    {
      if (LettucePlayDisplay.s_sortedRewardRecords == null)
        LettucePlayDisplay.s_sortedRewardRecords = GameDbf.MercenariesRankedSeasonRewardRank.GetRecords().OrderBy<MercenariesRankedSeasonRewardRankDbfRecord, int>((Func<MercenariesRankedSeasonRewardRankDbfRecord, int>) (r => r.MinPublicRatingUnlock)).ToList<MercenariesRankedSeasonRewardRankDbfRecord>();
      return LettucePlayDisplay.s_sortedRewardRecords;
    }
  }

  public MercenaryDetailDisplay MercenaryDetailDisplay { get; private set; }

  public override void Start()
  {
    base.Start();
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    this.m_sceneDisplayWidgetReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnSceneDisplayWidgetReady));
    this.m_collectionButtonReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnCollectionButtonReady));
    this.m_TeamListDisplay.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnTeamListDisplayReady));
    this.m_MercDetailsDisplayReference.RegisterReadyListener<MercenaryDetailDisplay>(new Action<MercenaryDetailDisplay>(this.OnMercDetailsDisplayReady));
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_PlayButtonPhoneReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayButtonReady));
      this.m_BackButtonPhoneReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnBackButtonReady));
      this.m_TeamPreviewPhoneReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnTeamPreviewReady));
      this.m_TeamDisplayTrayMobileReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnTeamDisplayTrayReady));
      this.m_RewardChestPhoneReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnRewardChestReady));
    }
    else
    {
      this.m_PlayButtonReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayButtonReady));
      this.m_BackButtonReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnBackButtonReady));
      this.m_TeamPreviewReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnTeamPreviewReady));
      this.m_RewardChestReference.RegisterReadyListener<VisualController>(new Action<VisualController>(this.OnRewardChestReady));
    }
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_MercenariesPVPLobby);
    Network.Get().MercenariesPlayerInfoRequest();
    NetCache.Get().RegisterUpdatedListener(typeof (NetCache.NetCacheMercenariesPlayerInfo), new Action(this.OnPlayerInfoReceived));
    CollectionManager.Get().MercenaryArtVariationChangedEvent += new Action<int, int, TAG_PREMIUM>(this.OnMercenaryArtVariationChangedEvent);
    this.StartCoroutine(this.InitializeWhenReady());
  }

  private void OnDestroy()
  {
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager != null)
      collectionManager.MercenaryArtVariationChangedEvent -= new Action<int, int, TAG_PREMIUM>(this.OnMercenaryArtVariationChangedEvent);
    NetCache.Get()?.RemoveUpdatedListener(typeof (NetCache.NetCacheMercenariesPlayerInfo), new Action(this.OnPlayerInfoReceived));
    GameMgr.Get()?.UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    if (!((UnityEngine.Object) this.m_seasonRewardsPopup != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_seasonRewardsPopup.gameObject);
  }

  private void TeamListEventListener(string eventName)
  {
    if (!(eventName == "TEAM_SELECTED"))
      return;
    this.OnTeamSelected();
  }

  private void PVPRatingEventListener(string eventName)
  {
    if (!(eventName == "PVP_RATING_CLICKED_code") || !this.IsFinishedLoading(out string _) || this.m_showingRewardsPopup)
      return;
    this.m_showingRewardsPopup = true;
    this.StartCoroutine(this.ShowSeasonRewardsPopup());
  }

  private IEnumerator ShowSeasonRewardsPopup()
  {
    LettucePlayDisplay lettucePlayDisplay = this;
    NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
    if (netObject == null)
    {
      Log.Lettuce.PrintError("InitializeChestData - No mercenaries player info in NetCache.");
    }
    else
    {
      if ((UnityEngine.Object) lettucePlayDisplay.m_seasonRewardsPopup == (UnityEngine.Object) null)
      {
        lettucePlayDisplay.m_seasonRewardsPopup = WidgetInstance.Create("MercenaryRewardListPopup.prefab:5df60e3fc26ac554685cbc730ea0a6ba");
        lettucePlayDisplay.m_seasonRewardsPopup.RegisterReadyListener(new Action<object>(lettucePlayDisplay.OnRewardsPopupReady), (object) null, true);
        lettucePlayDisplay.m_seasonRewardsPopup.WillLoadSynchronously = true;
        lettucePlayDisplay.m_seasonRewardsPopup.Initialize();
        MercenaryRewardListPopupDataModel listPopupDataModel = new MercenaryRewardListPopupDataModel()
        {
          Title = GameUtils.GetMercenariesSeasonName(netObject.PvpSeasonId)
        };
        bool flag = true;
        foreach (MercenariesRankedSeasonRewardRankDbfRecord sortedRewardRecord in LettucePlayDisplay.SortedRewardRecords)
        {
          MercenaryRewardListPopupTierDataModel popupTierDataModel = new MercenaryRewardListPopupTierDataModel()
          {
            Rating = sortedRewardRecord.MinPublicRatingUnlock.ToString(),
            Earned = netObject.PvpSeasonHighestRating >= sortedRewardRecord.MinPublicRatingUnlock,
            IsNextTier = flag && netObject.PvpSeasonHighestRating < sortedRewardRecord.MinPublicRatingUnlock
          };
          flag = popupTierDataModel.Earned;
          listPopupDataModel.Tiers.Add(popupTierDataModel);
        }
        lettucePlayDisplay.m_seasonRewardsPopup.BindDataModel((IDataModel) listPopupDataModel, false);
      }
      while (lettucePlayDisplay.m_seasonRewardsPopup.IsChangingStates)
        yield return (object) null;
      UIContext.GetRoot().ShowPopup(lettucePlayDisplay.m_seasonRewardsPopup.gameObject);
      lettucePlayDisplay.m_seasonRewardsPopup.Show();
      lettucePlayDisplay.m_seasonRewardsPopup.TriggerEvent("SHOW", new Widget.TriggerEventParameters());
      lettucePlayDisplay.m_seasonRewardsPopup.GetComponentInChildren<RewardListAutoScroller>().Init((Widget) lettucePlayDisplay.m_seasonRewardsPopup, lettucePlayDisplay.m_highRatingTierIndex);
    }
  }

  private void OnRewardsPopupReady(object o)
  {
    OverlayUI.Get().AddGameObject(this.m_seasonRewardsPopup.gameObject);
    this.m_seasonRewardsPopup.transform.localPosition = this.m_RewardListPopupLocalPosition;
    this.m_seasonRewardsPopup.transform.localScale = Vector3.one * this.m_RewardListPopupLocalScale;
    this.m_seasonRewardsPopup.RegisterEventListener(new Widget.EventListenerDelegate(this.RewardsPopupEventHandler));
    this.m_seasonRewardsPopup.Hide();
  }

  private void RewardsPopupEventHandler(string eventName)
  {
    if (!(eventName == "HIDE"))
      return;
    UIContext.GetRoot().DismissPopup(this.m_seasonRewardsPopup.gameObject);
    this.m_showingRewardsPopup = false;
  }

  public void OnPlayButtonReady(PlayButton playButton)
  {
    this.m_playButtonFinishedLoading = true;
    if ((UnityEngine.Object) playButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "PlayButton could not be found! You will not be able to click 'Play'!");
    }
    else
    {
      this.m_playButton = playButton;
      this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPlayButtonRelease));
      this.m_playButton.Disable(true);
    }
  }

  public void OnBackButtonReady(UIBButton backButton)
  {
    this.m_backButtonFinishedLoading = true;
    if ((UnityEngine.Object) backButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "BackButton could not be found! You will not be able to click 'Back'!");
    }
    else
    {
      this.m_backButton = backButton;
      this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnBackButtonRelease));
    }
  }

  public void OnCollectionButtonReady(UIBButton collectionButton)
  {
    this.m_collectionButtonFinishedLoading = true;
    if ((UnityEngine.Object) collectionButton == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "CollectionButton could not be found! You will not be able to click 'Mercenary Collection'!");
    }
    else
    {
      this.m_collectionButton = collectionButton;
      this.m_collectionButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnCollectionButtonRelease));
    }
  }

  public void OnTeamListDisplayReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController != (UnityEngine.Object) null)
      visualController.GetComponent<Widget>().RegisterEventListener(new Widget.EventListenerDelegate(this.TeamListEventListener));
    this.m_teamListVisualController = visualController;
    this.m_teamListDisplayFinishedLoading = true;
  }

  private void OnMercDetailsDisplayReady(MercenaryDetailDisplay display) => this.MercenaryDetailDisplay = display;

  public void OnTeamPreviewReady(Widget preview)
  {
    if ((UnityEngine.Object) preview == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "TeamPreview could not be found!");
    }
    else
    {
      this.m_teamPreviewWidget = preview;
      this.m_teamPreviewFinishedLoading = true;
      this.PopulateTeamPreviewData(new LettuceTeam());
    }
  }

  public void OnTeamDisplayTrayReady(VisualController teamDisplayTray)
  {
    this.m_teamDisplayTrayFinishedLoading = true;
    if ((UnityEngine.Object) teamDisplayTray == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "Team Display Tray could not be found!");
    else
      this.m_teamDisplayTray = teamDisplayTray;
  }

  public void OnRewardChestReady(VisualController visualController)
  {
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "FinalBossChest could not be found!");
    }
    else
    {
      this.m_rewardChestVisualController = visualController;
      this.m_rewardChestFinishedLoading = true;
    }
  }

  private void OnSceneDisplayWidgetReady(Widget widget) => widget.RegisterEventListener(new Widget.EventListenerDelegate(this.PVPRatingEventListener));

  public void OnPlayerInfoReceived() => this.m_playerInfoReceived = true;

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
      case FindGameState.SERVER_GAME_CANCELED:
        this.m_playButton.Enable();
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.MERCENARIES_PLAY_SCREEN);
        break;
    }
    return false;
  }

  public override bool IsFinishedLoading(out string failureMessage)
  {
    if (!this.m_playButtonFinishedLoading)
    {
      failureMessage = "LettucePlayDisplay - Play button never loaded.";
      return false;
    }
    if (!this.m_backButtonFinishedLoading)
    {
      failureMessage = "LettucePlayDisplay - Back button never loaded.";
      return false;
    }
    if (!this.m_collectionButtonFinishedLoading)
    {
      failureMessage = "LettucePlayDisplay - Collection button never loaded.";
      return false;
    }
    if (!this.m_teamListDisplayFinishedLoading)
    {
      failureMessage = "LettucePlayDisplay - Team list display never loaded.";
      return false;
    }
    if (!this.m_teamPreviewFinishedLoading)
    {
      failureMessage = "LettucePlayDisplay - Team preview button never loaded.";
      return false;
    }
    if (!this.m_playerInfoReceived)
    {
      failureMessage = "LettucePlayDisplay - Player Info never received.";
      return false;
    }
    if ((bool) UniversalInputManager.UsePhoneUI && !this.m_teamDisplayTrayFinishedLoading)
    {
      failureMessage = "LettucePlayDisplay - Team display tray never loaded.";
      return false;
    }
    if (!this.m_rewardChestFinishedLoading)
    {
      failureMessage = "LettucePlayDisplay - Reward chest never loaded.";
      return false;
    }
    failureMessage = string.Empty;
    return true;
  }

  private void OnPlayButtonRelease(UIEvent e)
  {
    if (!Network.IsLoggedIn())
      DialogManager.Get().ShowReconnectHelperDialog();
    else if (this.m_selectedTeam == null)
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_VALIDATE_TEAM_HEADER"),
        m_text = GameStrings.Get("GLUE_LETTUCE_VALIDATE_NO_TEAM_SELECTED"),
        m_showAlertIcon = true,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
    else if (!this.m_selectedTeam.IsValid())
    {
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_LETTUCE_VALIDATE_TEAM_HEADER"),
        m_text = GameStrings.Get("GLUE_LETTUCE_VALIDATE_TEAM_INVALID"),
        m_showAlertIcon = true,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_okText = GameStrings.Get("GLOBAL_OKAY")
      });
    }
    else
    {
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.MERCENARIES_QUEUE);
      this.m_playButton.Disable();
      if ((bool) UniversalInputManager.UsePhoneUI)
        this.m_teamDisplayTray.Owner.TriggerEvent("HIDE", new Widget.TriggerEventParameters());
      this.MercenaryDetailDisplay.Hide();
      GameMgr.Get().FindGame(GameType.GT_MERCENARIES_PVP, PegasusShared.FormatType.FT_WILD, 3743, lettuceTeamId: this.m_selectedTeam.ID);
    }
  }

  private void OnBackButtonRelease(UIEvent e) => this.SetNextModeAndHandleTransition(SceneMgr.Mode.LETTUCE_VILLAGE, SceneMgr.TransitionHandlerType.CURRENT_SCENE, (object) null);

  private void OnCollectionButtonRelease(UIEvent e)
  {
    if (!Network.IsLoggedIn())
      DialogManager.Get().ShowReconnectHelperDialog();
    else
      this.SetNextModeAndHandleTransition(SceneMgr.Mode.LETTUCE_COLLECTION, SceneMgr.TransitionHandlerType.CURRENT_SCENE, (object) null);
  }

  protected override bool ShouldStartShown() => SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_COLLECTION && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.COLLECTIONMANAGER && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LETTUCE_VILLAGE;

  private IEnumerator InitializeWhenReady()
  {
    LettucePlayDisplay lettucePlayDisplay = this;
    while (!lettucePlayDisplay.IsFinishedLoading(out string _))
      yield return (object) null;
    lettucePlayDisplay.InitializeChestData();
    lettucePlayDisplay.InitializeTeamListDataModel();
    RewardPopups rewardPopups = PopupDisplayManager.Get().RewardPopups;
    bool show = rewardPopups.HasNonAutoRetireMercenariesRewardsToShow();
    NetCache.ProfileNoticeMercenariesSeasonRewards seasonRewardsNotice = rewardPopups.GetNextMercenariesSeasonRewardsNotice();
    if (show || seasonRewardsNotice != null)
      lettucePlayDisplay.StartCoroutine(lettucePlayDisplay.ShowMercenariesRewards(show, seasonRewardsNotice));
  }

  private LettuceTeamListDataModel GetTeamListDataModel()
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return (LettuceTeamListDataModel) null;
    Widget owner = (Widget) component.Owner;
    IDataModel model;
    if (!owner.GetDataModel(218, out model))
    {
      model = (IDataModel) new LettuceTeamListDataModel();
      owner.BindDataModel(model);
    }
    return model as LettuceTeamListDataModel;
  }

  private LettuceTeamDataModel GetTeamPreviewDataModel()
  {
    if ((UnityEngine.Object) this.m_teamPreviewWidget == (UnityEngine.Object) null)
      return (LettuceTeamDataModel) null;
    IDataModel model;
    if (!this.m_teamPreviewWidget.GetDataModel(217, out model))
    {
      model = (IDataModel) new LettuceTeamDataModel();
      this.m_teamPreviewWidget.BindDataModel(model);
    }
    return model as LettuceTeamDataModel;
  }

  private void InitializeTeamListDataModel() => CollectionUtils.PopulateMercenariesTeamListDataModel(this.GetTeamListDataModel(), !(bool) UniversalInputManager.UsePhoneUI || SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.GAMEPLAY);

  private void InitializeChestData()
  {
    VisualController component = this.GetComponent<VisualController>();
    if (!((UnityEngine.Object) component != (UnityEngine.Object) null))
      return;
    NetCache.NetCacheMercenariesPlayerInfo netObject = NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>();
    if (netObject == null)
    {
      Log.Lettuce.PrintError("InitializeChestData - No mercenaries player info in NetCache.");
    }
    else
    {
      float num = (float) netObject.PvpRewardChestWinsProgress / (float) netObject.PvpRewardChestWinsRequired;
      this.m_highRatingTierIndex = 0;
      foreach (MercenariesRankedSeasonRewardRankDbfRecord sortedRewardRecord in LettucePlayDisplay.SortedRewardRecords)
      {
        if (sortedRewardRecord.MinPublicRatingUnlock <= netObject.PvpSeasonHighestRating)
          ++this.m_highRatingTierIndex;
        else
          break;
      }
      LettucePlayDisplayDataModel displayDataModel = new LettucePlayDisplayDataModel()
      {
        ChestCurrentWins = (int) netObject.PvpRewardChestWinsProgress,
        ChestMaxWins = (int) netObject.PvpRewardChestWinsRequired,
        ChestProgressPercent = num,
        ChestProgressBarText = GameStrings.Format("GLUE_LETTUCE_PVP_CHEST_PROGRESS_BAR_TEXT", (object) netObject.PvpRewardChestWinsProgress, (object) netObject.PvpRewardChestWinsRequired),
        Rating = netObject.PvpRating,
        HighRatingTierIndex = this.m_highRatingTierIndex
      };
      component.BindDataModel((IDataModel) displayDataModel);
    }
  }

  private void PopulateTeamPreviewData(LettuceTeam team)
  {
    if (team == null)
      return;
    CollectionUtils.PopulateTeamPreviewData(this.GetTeamPreviewDataModel(), team, (List<int>) null, false);
  }

  private void OnTeamSelected()
  {
    EventDataModel eventDataModel = WidgetUtils.GetEventDataModel(this.m_teamListVisualController);
    if (eventDataModel == null)
    {
      Log.Lettuce.PrintError("No event data model attached to the LettucePlayDisplay");
    }
    else
    {
      LettuceTeamDataModel payload = (LettuceTeamDataModel) eventDataModel.Payload;
      this.m_selectedTeam = CollectionManager.Get().GetTeam(payload.TeamId);
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.MERCENARIES, GameSaveKeySubkeyId.MERCENARIES_LAST_SELECTED_PVP_TEAM, new long[1]
      {
        payload.TeamId
      }));
      this.PopulateTeamPreviewData(this.m_selectedTeam);
      if (this.m_selectedTeam.IsValid() && !this.m_selectedTeam.DoesContainDisabledMerc())
      {
        this.m_playButton.Enable();
      }
      else
      {
        this.m_playButton.Disable();
        if (!this.m_selectedTeam.DoesContainDisabledMerc())
          return;
        DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_LETTUCE_DISABLED_TEAM_HEADER"),
          m_text = GameStrings.Get("GLUE_LETTUCE_DISABLED_TEAM"),
          m_alertTextAlignment = UberText.AlignmentOptions.Center,
          m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
          m_cancelText = GameStrings.Get("GLOBAL_BUTTON_NO"),
          m_confirmText = GameStrings.Get("GLUE_EDIT_TEAM"),
          m_blurWhenShown = true,
          m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
          {
            if (response != AlertPopup.Response.CONFIRM)
              return;
            this.OnCollectionButtonRelease((UIEvent) null);
          })
        });
      }
    }
  }

  private IEnumerator ShowMercenariesRewards(
    bool hasMercenariesRewards,
    NetCache.ProfileNoticeMercenariesSeasonRewards seasonRewardsNotice)
  {
    LettucePlayDisplay lettucePlayDisplay = this;
    lettucePlayDisplay.m_clickBlocker.SetActive(true);
    lettucePlayDisplay.m_blockingPopupDisplayManager = true;
    while (SceneMgr.Get().IsTransitionNowOrPending())
      yield return (object) null;
    yield return (object) new WaitForSeconds(lettucePlayDisplay.m_delayBeforeChestAnimation);
    while (PopupDisplayManager.Get().IsShowing)
      yield return (object) null;
    if (hasMercenariesRewards)
    {
      lettucePlayDisplay.m_rewardChestVisualController.SetState("OPEN_REWARD");
      yield return (object) new WaitForSeconds(lettucePlayDisplay.m_chestAnimationTime);
      if ((bool) UniversalInputManager.UsePhoneUI)
        lettucePlayDisplay.m_teamDisplayTray.Owner.TriggerEvent("HIDE", new Widget.TriggerEventParameters());
      NetCache.ProfileNoticeMercenariesRewards mercenariesRewardToShow1 = PopupDisplayManager.Get().RewardPopups.GetNextNonAutoRetireRewardMercenariesRewardToShow();
      NetCache.ProfileNoticeMercenariesRewards mercenariesRewardToShow2 = PopupDisplayManager.Get().RewardPopups.GetNextBonusMercenariesRewardToShow();
      PopupDisplayManager.Get().RewardPopups.ShowMercenariesRewards(true, mercenariesRewardToShow1, mercenariesRewardToShow2);
      while (PopupDisplayManager.Get().IsShowing)
        yield return (object) null;
      lettucePlayDisplay.m_rewardChestVisualController.SetState("CLOSE_REWARD");
    }
    if (seasonRewardsNotice != null)
      DialogManager.Get().ShowMercenariesSeasonRewardsDialog(seasonRewardsNotice);
    lettucePlayDisplay.m_clickBlocker.SetActive(false);
    lettucePlayDisplay.m_blockingPopupDisplayManager = false;
  }

  public override bool IsBlockingPopupDisplayManager() => this.m_blockingPopupDisplayManager;

  private void OnMercenaryArtVariationChangedEvent(
    int mercenaryDbId,
    int artVariationId,
    TAG_PREMIUM premium)
  {
    foreach (LettuceMercenary merc in this.m_selectedTeam.GetMercs())
    {
      if (merc.ID == mercenaryDbId)
      {
        CollectionUtils.PopulateTeamPreviewData(this.GetTeamPreviewDataModel(), this.m_selectedTeam, (List<int>) null, false);
        break;
      }
    }
  }

  public void ShowMercDetailsDisplay(LettuceMercenary mercenary)
  {
    if (this.m_selectedTeam != null)
    {
      LettuceTeamDataModel previewDataModel = this.GetTeamPreviewDataModel();
      CollectionUtils.PopulateMercenariesTeamDataModel(previewDataModel, this.m_selectedTeam);
      this.MercenaryDetailDisplay.GetComponent<Widget>().BindDataModel((IDataModel) previewDataModel);
    }
    this.MercenaryDetailDisplay.Show(mercenary, (bool) UniversalInputManager.UsePhoneUI ? "SHOW_PARTIAL" : "SHOW_FULL", this.m_selectedTeam);
  }
}
