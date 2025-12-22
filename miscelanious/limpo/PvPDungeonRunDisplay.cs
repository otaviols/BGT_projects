using Assets;
using Hearthstone.DataModels;
using Hearthstone.DungeonCrawl;
using Hearthstone.UI;
using PegasusUtil;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PvPDungeonRunDisplay : MonoBehaviour
{
  public AsyncReference m_PlayButtonReference;
  public AsyncReference m_PlayButtonPhoneReference;
  public AsyncReference m_BackButtonReference;
  public AsyncReference m_BackButtonPhoneReference;
  public AsyncReference m_SeasonNameTextReference;
  public AsyncReference m_SeasonNameTextPhoneReference;
  public AsyncReference m_StickyContainerReference;
  public AsyncReference m_StickyContainerPhoneReference;
  public PlayButton m_playButton;
  public UIBButton m_backButton;
  public DuelsPopupManager m_duelsPopupManager;
  public Widget m_stickyContainer;
  public UberText m_seasonName;
  private bool m_playButtonFinishedLoading;
  private bool m_playButtonWasEnabled;
  private bool m_backButtonFinishedLoading;
  private bool m_dataModelLoaded;
  private bool m_stickyContainerFinishedLoading;
  private bool m_seasonNameTextFinishedLoading;
  private bool m_isContentRoll = true;
  private bool m_isStartingSession;
  private PVPDRLobbyDataModel m_dataModel;
  private DateTime m_seasonEndDate;
  private bool m_isSeasonActive;
  private static PvPDungeonRunDisplay m_instance;

  public static PvPDungeonRunDisplay Get() => PvPDungeonRunDisplay.m_instance;

  public bool IsFinishedLoading => this.m_playButtonFinishedLoading && this.m_backButtonFinishedLoading && this.m_dataModelLoaded && this.m_seasonNameTextFinishedLoading;

  private void Awake() => PvPDungeonRunDisplay.m_instance = this;

  private void Start()
  {
    Navigation.Push(new Navigation.NavigateBackHandler(PvPDungeonRunDisplay.OnNavigateBack));
    this.m_BackButtonReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnBackButtonReady));
    this.m_BackButtonPhoneReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnBackButtonReady));
    this.m_PlayButtonReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayButtonReady));
    this.m_PlayButtonPhoneReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayButtonReady));
    this.m_StickyContainerReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnLobbyStickiesReady));
    this.m_StickyContainerPhoneReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnLobbyStickiesReady));
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_SeasonNameTextPhoneReference.RegisterReadyListener<UberText>(new Action<UberText>(this.OnSeasonNameTextReady));
    else
      this.m_SeasonNameTextReference.RegisterReadyListener<UberText>(new Action<UberText>(this.OnSeasonNameTextReady));
    this.m_seasonEndDate = DateTime.Now;
    this.InitializeLobbyData();
    Network.Get().RegisterNetHandler((object) PVPDRSessionInfoResponse.PacketID.ID, new Network.NetHandler(this.OnPVPDRSessionInfoResponse));
    Network.Get().SendPVPDRSessionInfoRequest();
    this.m_duelsPopupManager = PvPDungeonRunScene.Get().GetPopupManager();
    if (!((UnityEngine.Object) this.m_duelsPopupManager != (UnityEngine.Object) null))
      return;
    this.m_duelsPopupManager.AddOnNormalButtonPressedDelegate((Action) (() => this.StartSession(false)));
    this.m_duelsPopupManager.AddOnSuccessfulPurchaseDelegate((Action) (() => this.StartSession(true)));
  }

  public void InitializeLobbyData()
  {
    this.m_dataModel = this.GetPVPDRLobbyDataModel();
    if (this.m_dataModel == null)
    {
      Log.Net.PrintError("Could not retrieve PVPDRLobby data model.");
    }
    else
    {
      NetCache netCache = NetCache.Get();
      if (netCache == null)
      {
        Log.Net.PrintError("Could not retrieve NetCache.");
      }
      else
      {
        NetCache.NetCachePVPDRStatsInfo netObject = netCache.GetNetObject<NetCache.NetCachePVPDRStatsInfo>();
        if (netObject != null)
        {
          this.m_dataModel.Rating = netObject.Rating;
          this.m_dataModel.PaidRating = netObject.PaidRating;
          this.m_dataModel.HighWatermark = netObject.HighWatermark;
        }
        else
          Log.Net.PrintError("No PVPDR rating info in NetCache.");
        this.m_dataModel.IsEarlyAccess = DuelsConfig.IsEarlyAccess();
        this.m_dataModel.IsFreeUnlocked = DuelsConfig.IsFreeUnlocked();
        this.m_dataModel.IsPaidUnlocked = DuelsConfig.IsPaidUnlocked();
        GameModeDisplay gameModeDisplay = GameModeDisplay.Get();
        if ((UnityEngine.Object) gameModeDisplay != (UnityEngine.Object) null)
          gameModeDisplay.RegisterOnHideTrayListener(new Action(PvPDungeonRunDisplay.OnGameModeTrayHidden));
        else
          Log.Net.PrintError("GameModeDisplay not instantiated.");
      }
    }
  }

  public void OnPVPDRSessionInfoResponse()
  {
    PVPDRSessionInfoResponse sessionInfoResponse = Network.Get().GetPVPDRSessionInfoResponse();
    if (sessionInfoResponse.HasSession)
    {
      this.m_dataModel.Wins = (int) sessionInfoResponse.Session.Wins;
      this.m_dataModel.Losses = (int) sessionInfoResponse.Session.Losses;
      this.m_dataModel.HasSession = sessionInfoResponse.Session.HasSession;
      this.m_dataModel.IsSessionActive = sessionInfoResponse.Session.IsActive;
      this.m_dataModel.IsPaidEntry = sessionInfoResponse.Session.IsPaidEntry;
      this.m_dataModel.IsSessionRolledOver = sessionInfoResponse.Session.DidSeasonRollover;
      if (this.m_dataModel.IsSessionActive)
        this.m_dataModel.LastPlayedMode = this.m_dataModel.IsPaidEntry ? 2 : 1;
    }
    this.m_isSeasonActive = sessionInfoResponse.HasCurrentSeason;
    if (this.m_isSeasonActive)
    {
      int adventureIdForSeason1 = DuelsConfig.GetAdventureIdForSeason(sessionInfoResponse.CurrentSeason.Season.GameContentSeason.SeasonId);
      int adventureIdForSeason2 = DuelsConfig.GetAdventureIdForSeason(sessionInfoResponse.CurrentSeason.NextSeasonId);
      this.m_isContentRoll = adventureIdForSeason2 == 0 || adventureIdForSeason1 != adventureIdForSeason2;
      TimeUtils.ElapsedStringSet stringSet = new TimeUtils.ElapsedStringSet()
      {
        m_seconds = this.m_isContentRoll ? "GLUE_PVPDR_LABEL_SEASON_ENDING_SECONDS" : "GLUE_PVPDR_LABEL_RATING_RESET_SECONDS",
        m_minutes = this.m_isContentRoll ? "GLUE_PVPDR_LABEL_SEASON_ENDING_MINUTES" : "GLUE_PVPDR_LABEL_RATING_RESET_MINUTES",
        m_hours = this.m_isContentRoll ? "GLUE_PVPDR_LABEL_SEASON_ENDING_HOURS" : "GLUE_PVPDR_LABEL_RATING_RESET_HOURS",
        m_yesterday = (string) null,
        m_days = this.m_isContentRoll ? "GLUE_PVPDR_LABEL_SEASON_ENDING_DAYS" : "GLUE_PVPDR_LABEL_RATING_RESET_DAYS",
        m_weeks = this.m_isContentRoll ? "GLUE_PVPDR_LABEL_SEASON_ENDING_WEEKS" : "GLUE_PVPDR_LABEL_RATING_RESET_WEEKS",
        m_monthAgo = this.m_isContentRoll ? "GLUE_PVPDR_LABEL_SEASON_ENDING_OVER_1_MONTH" : "GLUE_PVPDR_LABEL_RATING_RESET_OVER_1_MONTH"
      };
      long endSecondsFromNow = (long) sessionInfoResponse.CurrentSeason.Season.GameContentSeason.EndSecondsFromNow;
      this.m_seasonEndDate = DateTime.Now.AddSeconds((double) endSecondsFromNow);
      this.m_dataModel.TimeRemainingString = TimeUtils.GetElapsedTimeString(endSecondsFromNow, stringSet, true);
      this.m_dataModel.Season = sessionInfoResponse.CurrentSeason.Season.GameContentSeason.SeasonId;
      if ((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null && !this.m_playButtonWasEnabled)
      {
        this.m_playButton.Enable();
        this.m_playButtonWasEnabled = true;
      }
      if ((UnityEngine.Object) this.m_seasonName != (UnityEngine.Object) null)
        this.m_seasonName.Text = (string) GameDbf.Adventure.GetRecord(adventureIdForSeason1).Name;
    }
    this.m_dataModelLoaded = true;
  }

  public static void OnGameModeTrayHidden()
  {
    if (!((UnityEngine.Object) PvPDungeonRunDisplay.m_instance != (UnityEngine.Object) null))
      return;
    PvPDungeonRunDisplay.m_instance.ShowNewUnlocksPopupIfNecessary();
    GameModeDisplay.Get().UnRegisterOnHideTrayListener(new Action(PvPDungeonRunDisplay.OnGameModeTrayHidden));
  }

  public void EnableButtons(bool enabled = true)
  {
    if (this.m_isStartingSession)
      return;
    this.EnablePlayButton(enabled);
    this.EnableBackButton(enabled);
  }

  public void EnablePlayButton(bool enabled, bool textEnabled = false)
  {
    if (!((UnityEngine.Object) this.m_playButton != (UnityEngine.Object) null))
      return;
    if (enabled)
      this.m_playButton.Enable();
    else
      this.m_playButton.Disable(textEnabled);
  }

  public void EnableBackButton(bool enabled)
  {
    if (!((UnityEngine.Object) this.m_backButton != (UnityEngine.Object) null))
      return;
    this.m_backButton.SetEnabled(enabled);
    this.m_backButton.Flip(enabled);
  }

  public void OnBackButtonReady(UIBButton button)
  {
    if ((UnityEngine.Object) button == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "BackButton could not be found! You will not be able to click 'Back'!");
    }
    else
    {
      this.m_backButton = button;
      this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonRelease));
      this.m_backButtonFinishedLoading = true;
    }
  }

  public void BackButtonRelease(UIEvent e)
  {
    this.EnablePlayButton(false);
    Navigation.GoBack();
  }

  public void OnPlayButtonReady(PlayButton button)
  {
    if ((UnityEngine.Object) button == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "PlayButton could not be found! You will not be able to click 'Play'!");
    }
    else
    {
      this.m_playButton = button;
      this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayButtonRelease));
      if (this.m_isSeasonActive)
      {
        this.m_playButton.Enable();
        this.m_playButtonWasEnabled = true;
      }
      else
        this.m_playButton.Disable(true);
      this.m_playButtonFinishedLoading = true;
    }
  }

  public void PlayButtonRelease(UIEvent e)
  {
    this.EnableButtons(false);
    if (!this.m_dataModel.HasSession)
    {
      double totalSeconds = (this.m_seasonEndDate - DateTime.Now).TotalSeconds;
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      if (this.m_isContentRoll && totalSeconds <= (double) netObject.PVPDRClosedToNewSessionsSeconds)
      {
        DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
        {
          m_headerText = GameStrings.Get("GLUE_PVPDR"),
          m_text = GameStrings.Get("GLUE_PVPDR_SIGNUPS_CLOSED"),
          m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
          m_responseDisplay = AlertPopup.ResponseDisplay.OK,
          m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => this.EnableButtons())
        });
      }
      else
      {
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.DUELS_PURCHASE);
        this.m_duelsPopupManager.Show();
      }
    }
    else
      this.TransitionToNextScreen();
  }

  public void OnLobbyStickiesReady(Widget stickiesContainer)
  {
    if ((UnityEngine.Object) stickiesContainer == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "Stickies widget could not be found!");
    }
    else
    {
      this.m_stickyContainer = stickiesContainer;
      this.m_stickyContainerFinishedLoading = true;
    }
  }

  public void OnSeasonNameTextReady(UberText uberText)
  {
    this.m_seasonName = uberText;
    this.m_seasonNameTextFinishedLoading = true;
  }

  public PVPDRLobbyDataModel GetPVPDRLobbyDataModel()
  {
    if (this.m_dataModel != null)
      return this.m_dataModel;
    Widget component = this.GetComponent<Widget>();
    IDataModel model;
    if (!component.GetDataModel(181, out model))
    {
      model = (IDataModel) new PVPDRLobbyDataModel();
      component.BindDataModel(model);
    }
    return model as PVPDRLobbyDataModel;
  }

  public void OnHeroPickerShown() => this.m_isStartingSession = false;

  private void StartSession(bool paidEntry)
  {
    Network.Get().RegisterNetHandler((object) PVPDRSessionStartResponse.PacketID.ID, new Network.NetHandler(this.OnSessionStartResponse));
    Network.Get().SendPVPDRSessionStartRequest(paidEntry);
    this.m_isStartingSession = true;
  }

  private void OnSessionStartResponse()
  {
    Network.Get().RemoveNetHandler((object) PVPDRSessionStartResponse.PacketID.ID, new Network.NetHandler(this.OnSessionStartResponse));
    if (Network.Get().GetPVPDRSessionStartResponse().ErrorCode == PegasusShared.ErrorCode.ERROR_OK)
    {
      DuelsConfig.Get().SetRecentEnd(false);
      AdventureConfig.Get().ChangeSubScene(AdventureData.Adventuresubscene.CHOOSER);
      Network.Get().RegisterNetHandler((object) PVPDRSessionInfoResponse.PacketID.ID, new Network.NetHandler(this.OnAfterStartSessionInfoResponse));
      Network.Get().SendPVPDRSessionInfoRequest();
    }
    else
      this.OnSessionStartRequestFailed();
  }

  private void OnAfterStartSessionInfoResponse()
  {
    Network.Get().RemoveNetHandler((object) PVPDRSessionInfoResponse.PacketID.ID, new Network.NetHandler(this.OnAfterStartSessionInfoResponse));
    this.m_dataModel.IsPaidEntry = Network.Get().GetPVPDRSessionInfoResponse().Session.IsPaidEntry;
    GameSaveDataManager.Get().Request(PvPDungeonRunScene.Get().GetGSDKeyForAdventure(), new GameSaveDataManager.OnRequestDataResponseDelegate(this.CheckForTransition));
  }

  private void OnSessionStartRequestFailed()
  {
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_PVPDR"),
      m_text = GameStrings.Get("GLUE_PVPDR_SESSION_START_FAILED_BODY"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) => SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB))
    });
    this.m_isStartingSession = false;
  }

  private void TransitionToNextScreen()
  {
    bool flag = DungeonCrawlUtil.IsPVPDRSessionComplete();
    if (!DuelsConfig.IsInitialLoadoutComplete() && !flag)
    {
      if (PvPDungeonRunScene.Get().TransitionToGuestHeroPicker())
        return;
      this.EnableButtons();
    }
    else
      PvPDungeonRunScene.Get().TransitionToDungeonCrawlPlayMat();
  }

  private void CheckForTransition(bool success)
  {
    if (!success)
      return;
    this.TransitionToNextScreen();
  }

  private static bool OnNavigateBack()
  {
    if ((UnityEngine.Object) PvPDungeonRunDisplay.m_instance != (UnityEngine.Object) null)
      PvPDungeonRunDisplay.m_instance.EnableButtons(false);
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.GAME_MODE, SceneMgr.TransitionHandlerType.NEXT_SCENE);
    return true;
  }

  public void CheckForStatsChanged()
  {
    if (!this.m_stickyContainerFinishedLoading || !((UnityEngine.Object) this.m_stickyContainer != (UnityEngine.Object) null))
      return;
    this.m_stickyContainer.TriggerEvent("STATS_CHANGED");
    DuelsConfig.Get().SetRecentEnd(false);
  }

  private void ShowNewUnlocksPopupIfNecessary()
  {
    AdventureDbId advId = AdventureConfig.Get().GetSelectedAdventure();
    List<long> newHeroPowers;
    List<long> newTreasures;
    HashSet<int> unlocks = DungeonCrawlUtil.GetAchievementsForRecentUnlocks(advId, out newHeroPowers, out newTreasures);
    if (unlocks.Count <= 0)
      return;
    int num = newHeroPowers.Count + newTreasures.Count;
    PvPDungeonRunDisplay.Get().GetPVPDRLobbyDataModel().IsRatingNotice = false;
    PvPDungeonRunScene.ShowDuelsMessagePopup(GameStrings.Get("GLUE_DUELS_NEW_UNLOCKS_HEADER"), GameStrings.Format("GLUE_DUELS_NEW_UNLOCKS_BODY", (object) num), "", (Action) (() =>
    {
      DungeonCrawlUtil.MarkUnlocksAsNew(advId, AdventureModeDbId.DUNGEON_CRAWL, newHeroPowers, newTreasures);
      DungeonCrawlUtil.AcknowledgeUnlocks(unlocks);
    }));
  }
}
