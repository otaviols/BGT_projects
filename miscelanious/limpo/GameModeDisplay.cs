using Blizzard.T5.Core.Utils;
using Hearthstone.DataModels;
using Hearthstone.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameModeDisplay : MonoBehaviour
{
  public AsyncReference m_DisplayReference;
  public AsyncReference m_PlayButtonReference;
  public AsyncReference m_BackButtonReference;
  public AsyncReference m_BackButtonMobileReference;
  public SlidingTray m_slidingTray;
  public GameObject m_clickBlocker;
  public GameObject m_nameText;
  public UberText m_lockedNameText;
  public GameObject m_lockedPlateMesh;
  public VisualController m_gameModeButtonController;
  public List<string> m_tavernBrawlEnterCrowdSounds;
  private PlayButton m_playButton;
  private UIBButton m_backButton;
  private bool m_playButtonFinishedLoading;
  private bool m_backButtonFinishedLoading;
  private Action m_onSceneTransitionCompleteCallback;
  private List<GameModeDbfRecord> m_activeGameModeRecords = new List<GameModeDbfRecord>();
  private GameModeButtonDataModel m_selectedGameModeButtonDataModel;
  private List<long> m_seenGameModes = new List<long>();
  private static bool s_hasAlreadyShownTavernBrawlNewBanner;
  private static GameModeDisplay m_instance;
  private const string GAME_MODE_LOCKED_EVENT_NAME = "GAME_MODE_LOCKED";
  private const string GAME_MODE_ACTIVE_EVENT_NAME = "GAME_MODE_ACTIVE";
  private static Comparison<GameModeDbfRecord> OrderGameModes = (Comparison<GameModeDbfRecord>) ((a, b) => a.SortOrder.CompareTo(b.SortOrder));

  public static GameModeDisplay Get() => GameModeDisplay.m_instance;

  public bool IsFinishedLoading => this.m_playButtonFinishedLoading && this.m_backButtonFinishedLoading;

  private void Awake() => GameModeDisplay.m_instance = this;

  private void Start()
  {
    this.m_DisplayReference.RegisterReadyListener<Widget>(new Action<Widget>(this.OnDisplayReady));
    this.m_PlayButtonReference.RegisterReadyListener<PlayButton>(new Action<PlayButton>(this.OnPlayButtonReady));
    if ((bool) UniversalInputManager.UsePhoneUI)
      this.m_BackButtonMobileReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnBackButtonReady));
    else
      this.m_BackButtonReference.RegisterReadyListener<UIBButton>(new Action<UIBButton>(this.OnBackButtonReady));
    this.InitializeGameModeSceneData();
    this.m_slidingTray.OnTransitionComplete += new Action(this.OnSlidingTrayAnimationComplete);
    this.InitializeSlidingTray();
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_Tournament);
  }

  private void Update()
  {
  }

  public void RegisterOnHideTrayListener(Action action)
  {
    if (!((UnityEngine.Object) this.m_slidingTray != (UnityEngine.Object) null))
      return;
    this.m_slidingTray.OnTransitionComplete += action;
  }

  public void UnRegisterOnHideTrayListener(Action action)
  {
    if (!((UnityEngine.Object) this.m_slidingTray != (UnityEngine.Object) null))
      return;
    this.m_slidingTray.OnTransitionComplete -= action;
  }

  private void GameModeDisplayEventListener(string eventName)
  {
    if (!(eventName == "CHOOSE"))
    {
      if (!(eventName == "BACK"))
      {
        if (!(eventName == "GAME_MODE_CLICKED"))
          return;
        this.OnGameModeSelected();
      }
      else
        this.GoToHub();
    }
    else
      this.NavigateToSelectedMode();
  }

  private void OnDisplayReady(Widget widget)
  {
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "DisplayReference could not be found!");
    else
      widget.RegisterEventListener(new Widget.EventListenerDelegate(this.GameModeDisplayEventListener));
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
      this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayButtonRelease));
      this.m_playButton.Disable();
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
      this.m_backButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonRelease));
    }
  }

  public GameModeSceneDataModel GetGameModeSceneDataModel()
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return (GameModeSceneDataModel) null;
    Widget owner = (Widget) component.Owner;
    IDataModel model;
    if (!owner.GetDataModel(173, out model))
    {
      model = (IDataModel) new GameModeSceneDataModel();
      owner.BindDataModel(model);
    }
    return model as GameModeSceneDataModel;
  }

  public EventDataModel GetEventDataModel()
  {
    VisualController component = this.GetComponent<VisualController>();
    return (UnityEngine.Object) component == (UnityEngine.Object) null ? (EventDataModel) null : component.Owner.GetDataModel<EventDataModel>();
  }

  private void InitializeGameModeSceneData()
  {
    GameModeSceneDataModel modeSceneDataModel = this.GetGameModeSceneDataModel();
    if (modeSceneDataModel == null)
      return;
    SpecialEventManager specialEventManager = SpecialEventManager.Get();
    this.m_activeGameModeRecords.Clear();
    foreach (GameModeDbfRecord record in GameDbf.GameMode.GetRecords())
    {
      if (specialEventManager.IsEventActive(record.Event, false))
        this.m_activeGameModeRecords.Add(record);
    }
    this.m_activeGameModeRecords.Sort(GameModeDisplay.OrderGameModes);
    GameSaveDataManager gameSaveDataManager = GameSaveDataManager.Get();
    long num;
    gameSaveDataManager.GetSubkeyValue(GameSaveKeyId.GAME_MODE_SCENE, GameSaveKeySubkeyId.GAME_MODE_SCENE_LAST_SELECTED_GAME_MODE, out num);
    modeSceneDataModel.LastSelectedGameModeRecordId = (int) num;
    gameSaveDataManager.GetSubkeyValue(GameSaveKeyId.GAME_MODE_SCENE, GameSaveKeySubkeyId.GAME_MODE_SCENE_SEEN_GAME_MODES, out this.m_seenGameModes);
    if (this.m_seenGameModes == null)
      this.m_seenGameModes = new List<long>();
    modeSceneDataModel.GameModeButtons = new DataModelList<GameModeButtonDataModel>();
    foreach (GameModeDbfRecord activeGameModeRecord in this.m_activeGameModeRecords)
    {
      bool flag1 = this.ShouldShowNewBanner(activeGameModeRecord);
      bool flag2 = specialEventManager.IsEventActive(activeGameModeRecord.ShowAsEarlyAccessEvent, false);
      bool flag3 = specialEventManager.IsEventActive(activeGameModeRecord.ShowAsBetaEvent, false);
      modeSceneDataModel.GameModeButtons.Add(new GameModeButtonDataModel()
      {
        GameModeRecordId = activeGameModeRecord.ID,
        Name = (string) activeGameModeRecord.Name,
        Description = (string) activeGameModeRecord.Description,
        ButtonState = activeGameModeRecord.GameModeButtonState,
        IsNew = flag1,
        IsEarlyAccess = flag2,
        IsBeta = flag3
      });
    }
  }

  private bool ShouldShowNewBanner(GameModeDbfRecord gameModeRecord)
  {
    if (gameModeRecord == null)
    {
      Debug.LogError((object) "GameModeDisplay:ShouldShowNewBanner received a null gameModeRecord value");
      return false;
    }
    if (SpecialEventManager.Get().IsEventActive(gameModeRecord.ShowAsNewEvent, false) && !this.m_seenGameModes.Contains((long) gameModeRecord.ID))
      return true;
    switch (EnumUtils.Parse<SceneMgr.Mode>(gameModeRecord.LinkedScene))
    {
      case SceneMgr.Mode.ADVENTURE:
        return GameModeDisplay.ShouldSeeNewSoloAdventureBanner();
      case SceneMgr.Mode.TAVERN_BRAWL:
        if (!UserAttentionManager.CanShowAttentionGrabber("Hub.TavernBrawl.IsFirstTimeSeeingCurrentSeason") || !GameModeDisplay.ShouldSeeNewTavernBrawlBanner())
          return false;
        GameModeDisplay.s_hasAlreadyShownTavernBrawlNewBanner = true;
        return true;
      default:
        return false;
    }
  }

  private bool CanEnterMode(out string reason)
  {
    reason = "";
    GameModeDbfRecord gameModeDbfRecord = (GameModeDbfRecord) null;
    foreach (GameModeDbfRecord activeGameModeRecord in this.m_activeGameModeRecords)
    {
      if (activeGameModeRecord.ID == this.m_selectedGameModeButtonDataModel.GameModeRecordId)
      {
        gameModeDbfRecord = activeGameModeRecord;
        break;
      }
    }
    if (gameModeDbfRecord == null)
      return false;
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject == null)
    {
      reason = GameStrings.Get("GLUE_TOOLTIP_GAME_MODE_DATA_NOT_LOADED");
      return false;
    }
    int num = netObject.Games.GetFeatureFlag((NetCache.NetCacheFeatures.CacheGames.FeatureFlags) gameModeDbfRecord.FeatureUnlockId) ? 1 : 0;
    bool flag = gameModeDbfRecord.FeatureUnlockId2 == 0 || netObject.Games.GetFeatureFlag((NetCache.NetCacheFeatures.CacheGames.FeatureFlags) gameModeDbfRecord.FeatureUnlockId2);
    if (num == 0 && !flag)
    {
      reason = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
      return false;
    }
    SceneMgr.Mode mode = EnumUtils.Parse<SceneMgr.Mode>(gameModeDbfRecord.LinkedScene);
    if ((mode == SceneMgr.Mode.DRAFT || mode == SceneMgr.Mode.PVP_DUNGEON_RUN) && !AchieveManager.Get().HasUnlockedDefaultHeroes())
    {
      reason = GameStrings.Format("GLUE_GAME_MODE_UNLOCK_DEFAULT_HEROES", (object) this.m_selectedGameModeButtonDataModel.Name);
      return false;
    }
    if (mode == SceneMgr.Mode.TAVERN_BRAWL)
    {
      TavernBrawlManager tavernBrawlManager = TavernBrawlManager.Get();
      if (tavernBrawlManager == null)
      {
        reason = GameStrings.Get("GLUE_TOOLTIP_GAME_MODE_DATA_NOT_LOADED");
        return false;
      }
      if (!tavernBrawlManager.CanEnterStandardTavernBrawl(out reason))
        return false;
    }
    return true;
  }

  private void InitializeSlidingTray() => this.m_slidingTray.ToggleTraySlider(SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.HUB, animate: false);

  private void PlayButtonRelease(UIEvent e) => this.NavigateToSelectedMode();

  private void BackButtonRelease(UIEvent e) => this.GoToHub();

  private void GoToHub() => SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);

  private void NavigateToSelectedMode()
  {
    this.m_playButton.Disable();
    if (this.m_selectedGameModeButtonDataModel == null)
    {
      Log.All.PrintError("No game mode selected!");
    }
    else
    {
      string reason;
      if (!this.CanEnterMode(out reason))
      {
        this.ShowDisabledPopupForCurrentMode(reason);
      }
      else
      {
        GameModeDbfRecord record = GameDbf.GameMode.GetRecord(this.m_selectedGameModeButtonDataModel.GameModeRecordId);
        if (record == null)
        {
          Log.All.PrintError(string.Format("Game mode with invalid id {0} selected!", (object) this.m_selectedGameModeButtonDataModel.GameModeRecordId));
        }
        else
        {
          if (!this.m_seenGameModes.Contains((long) record.ID))
          {
            this.m_seenGameModes.Add((long) record.ID);
            GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.GAME_MODE_SCENE, GameSaveKeySubkeyId.GAME_MODE_SCENE_SEEN_GAME_MODES, this.m_seenGameModes.ToArray()));
          }
          SceneMgr.Mode mode = EnumUtils.Parse<SceneMgr.Mode>(record.LinkedScene);
          if (mode == SceneMgr.Mode.DRAFT)
          {
            ulong untilEndOfSeason = DraftManager.Get().SecondsUntilEndOfSeason;
            NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
            if (!DraftManager.Get().HasActiveRun && untilEndOfSeason <= (ulong) netObject.ArenaClosedToNewSessionsSeconds)
            {
              DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
              {
                m_headerText = GameStrings.Get("GLUE_ARENA_1ST_TIME_HEADER"),
                m_text = GameStrings.Get("GLUE_ARENA_SIGNUPS_CLOSED"),
                m_alertTextAlignmentAnchor = UberText.AnchorOptions.Middle,
                m_responseDisplay = AlertPopup.ResponseDisplay.OK
              });
              return;
            }
          }
          this.m_clickBlocker.SetActive(true);
          SceneMgr.Get().SetNextMode(mode, SceneMgr.TransitionHandlerType.CURRENT_SCENE, new SceneMgr.OnSceneLoadCompleteForSceneDrivenTransition(this.OnSceneLoadCompleteHandleTransition));
          if (mode == SceneMgr.Mode.DRAFT)
            AchieveManager.Get().NotifyOfClick(Achievement.ClickTriggerType.BUTTON_ARENA);
          if (mode != SceneMgr.Mode.TAVERN_BRAWL)
            return;
          if (TavernBrawlManager.Get().IsFirstTimeSeeingThisFeature)
            this.DoTavernBrawlIntroVO();
          else
            this.PlayTavernBrawlCrowdSFX();
        }
      }
    }
  }

  private void DoTavernBrawlIntroVO()
  {
    if (NotificationManager.Get().HasSoundPlayedThisSession("VO_INNKEEPER_TAVERNBRAWL_PUSH_32.prefab:4f57cd2af5fe5194fbc46c91171ab135"))
      return;
    Action<int> finishCallback = (Action<int>) (groupId => NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_TAVERNBRAWL_DESC1_29"), "VO_INNKEEPER_TAVERNBRAWL_DESC1_29.prefab:44d1a6b322c3dcf4c950e68eb4f4a05f"));
    NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.NONE, GameStrings.Get("VO_INNKEEPER_TAVERNBRAWL_PUSH_32"), "VO_INNKEEPER_TAVERNBRAWL_PUSH_32.prefab:4f57cd2af5fe5194fbc46c91171ab135", finishCallback);
    NotificationManager.Get().ForceAddSoundToPlayedList("VO_INNKEEPER_TAVERNBRAWL_PUSH_32.prefab:4f57cd2af5fe5194fbc46c91171ab135");
  }

  public void PlayTavernBrawlCrowdSFX()
  {
    if (this.m_tavernBrawlEnterCrowdSounds.Count < 1)
      return;
    int index = UnityEngine.Random.Range(0, this.m_tavernBrawlEnterCrowdSounds.Count);
    SoundManager.Get().LoadAndPlay((AssetReference) this.m_tavernBrawlEnterCrowdSounds[index]);
  }

  public static bool ShouldSeeNewSoloAdventureBanner()
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    bool flag1 = AdventureConfig.GetAdventurePlayerShouldSee() != 0;
    bool flag2 = AchieveManager.Get().HasUnlockedDefaultHeroes() & flag1;
    return netObject.Games.Practice & flag2;
  }

  public static bool ShouldSeeNewTavernBrawlBanner() => !GameModeDisplay.s_hasAlreadyShownTavernBrawlNewBanner && TavernBrawlManager.Get() != null && TavernBrawlManager.Get().IsFirstTimeSeeingCurrentSeason;

  private void OnSceneLoadCompleteHandleTransition(Action onTransitionComplete)
  {
    this.m_onSceneTransitionCompleteCallback = onTransitionComplete;
    this.m_slidingTray.HideTray();
  }

  public void ShowSlidingTrayAfterSceneLoad(Action onCompleteCallback)
  {
    this.m_clickBlocker.SetActive(true);
    this.m_onSceneTransitionCompleteCallback = onCompleteCallback;
    this.m_slidingTray.ShowTray();
  }

  private void OnSlidingTrayAnimationComplete()
  {
    this.m_clickBlocker.SetActive(false);
    if (this.m_onSceneTransitionCompleteCallback == null)
      return;
    this.m_onSceneTransitionCompleteCallback();
    this.m_onSceneTransitionCompleteCallback = (Action) null;
  }

  private void OnGameModeSelected()
  {
    EventDataModel eventDataModel = this.GetEventDataModel();
    if (eventDataModel == null)
    {
      Log.All.PrintError("No event data model attached to the GameModeDisplay.");
    }
    else
    {
      this.m_selectedGameModeButtonDataModel = (GameModeButtonDataModel) eventDataModel.Payload;
      GameSaveDataManager.Get().SaveSubkey(new GameSaveDataManager.SubkeySaveRequest(GameSaveKeyId.GAME_MODE_SCENE, GameSaveKeySubkeyId.GAME_MODE_SCENE_LAST_SELECTED_GAME_MODE, new long[1]
      {
        (long) this.m_selectedGameModeButtonDataModel.GameModeRecordId
      }));
      GameModeSceneDataModel modeSceneDataModel = this.GetGameModeSceneDataModel();
      if (modeSceneDataModel != null)
        modeSceneDataModel.LastSelectedGameModeRecordId = this.m_selectedGameModeButtonDataModel.GameModeRecordId;
      string reason;
      if (!this.CanEnterMode(out reason))
      {
        this.m_playButton.Disable(true);
        this.m_lockedNameText.Text = GameStrings.Format(reason, (object) this.m_selectedGameModeButtonDataModel.Name);
        this.m_gameModeButtonController.SetState("GAME_MODE_LOCKED");
      }
      else
      {
        this.m_playButton.Enable();
        this.m_gameModeButtonController.SetState("GAME_MODE_ACTIVE");
      }
    }
  }

  private void ShowDisabledPopupForCurrentMode(string lockReason)
  {
    if (string.IsNullOrEmpty(lockReason))
      return;
    this.ShowDisabledPopup(GameStrings.Get(this.m_selectedGameModeButtonDataModel.Name), lockReason);
  }

  private void ShowDisabledPopup(string header, string description)
  {
    if (string.IsNullOrEmpty(description))
      description = GameStrings.Get("GLUE_TOOLTIP_BUTTON_DISABLED_DESC");
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = header,
      m_text = description,
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }
}
