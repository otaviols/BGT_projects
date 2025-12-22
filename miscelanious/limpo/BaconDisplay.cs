using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core.Utils;
using Hearthstone.Commerce;
using Hearthstone.DataModels;
using Hearthstone.Progression;
using Hearthstone.UI;
using PegasusShared;
using Shared.Scripts.Util.ValueTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class BaconDisplay : AbsSceneDisplay
{
  public AsyncReference m_PlayButtonReference;
  public AsyncReference m_PlayButtonPhoneReference;
  public AsyncReference m_BackButtonReference;
  public AsyncReference m_BackButtonPhoneReference;
  public AsyncReference m_StatsButtonReference;
  public AsyncReference m_StatsButtonPhoneReference;
  public AsyncReference m_StatsPageReference;
  public AsyncReference m_StatsPagePhoneReference;
  public AsyncReference m_luckyDrawButtonReference;
  public AsyncReference m_LobbyReference;
  public AsyncReference m_PartyReference;
  public AsyncReference m_LobbyPhoneReference;
  public AsyncReference m_PartyPhoneReference;
  public Transform m_OffScreenBonePC;
  public Transform m_OnScreenBonePC;
  public Transform m_OffScreenBoneMobile;
  public Transform m_OnScreenBoneMobile;
  private bool m_playButtonFinishedLoading;
  private bool m_backButtonFinishedLoading;
  private bool m_statsButtonFinishedLoading;
  private bool m_luckyDrawButtonFinishedLoading;
  private bool m_partyFinishedLoading;
  private bool m_partyPhoneFinishedLoading;
  private WidgetTemplate m_OwningWidget;
  private PlayButton m_playButton;
  private UIBButton m_statsButton;
  private Clickable m_statsButtonClickable;
  private LuckyDrawButton m_luckyDrawButton;
  private RewardPresenter m_rewardPresenter = new RewardPresenter();
  private const float LUCKY_DRAW_BUTTON_FANFARE_FX_DELAY_SEC = 0.5f;
  private const string LUCKY_DRAW_NEW_HAMMER_FX = "NewHammerFX";
  private const string LUCKY_DRAW_NEW_HAMMER_ANIM = "LuckyDrawNewHammer_Anim";
  private Notification m_luckyDrawFTUENotification;
  private const float LUCKY_DRAW_FTUE_POPUP_DELAY_SEC = 4f;
  private const float LUCKY_DRAW_POPUP_DELAY_SEC = 3f;
  private const float LUCKY_DRAW_POPUP_IN_BETWEEN_DELAY_SEC = 1f;
  private const string FTUE_TOOLTIP_BONE = "FTUETooltip";
  private readonly PlatformDependentValue<string> PLATFORM_DEPENDENT_BONE_SUFFIX = new PlatformDependentValue<string>(PlatformCategory.Screen)
  {
    PC = "PC",
    Tablet = "PC",
    Phone = "Mobile"
  };
  private LuckyDrawManager m_luckyDrawManager;
  private LuckyDrawButtonDataModel m_luckyDrawButtonDataModel;
  private readonly PlatformDependentValue<bool> ShowLowMemoryWarning = new PlatformDependentValue<bool>(PlatformCategory.Memory)
  {
    LowMemory = true,
    MediumMemory = true,
    HighMemory = false
  };
  private const int MIN_DAYS_LEFT_FOR_LUCKY_DRAW_END_SOON_POPUP = 1;
  private const int MAX_DAYS_LEFT_FOR_LUCKY_DRAW_END_SOON_POPUP = 3;
  private const int PAST_GAMES_TO_SHOW = 5;
  private const int MINIONS_PER_BOARD = 7;
  private const string OPEN_SHOP_EVENT = "OpenShop";
  private const string OPEN_COLLECTION_EVENT = "OpenCollection";
  private const string STATS_PANEL_SLIDE_COMPLETE = "CODE_STATS_SLIDE_FINISHED";
  private const string STATS_PANEL_PHONE_SLIDE_COMPLETE = "CODE_STATS_PHONE_SLIDE_FINISHED";
  private static bool m_hasSeenLowMemoryWarningThisSession;

  private void Awake()
  {
    this.InitSlidingTray();
    this.m_luckyDrawManager = LuckyDrawManager.Get();
    if (this.m_luckyDrawManager == null)
      Log.All.PrintError("BaconDisplay.Awake() - LuckyDrawManger is null");
    this.RegisterListeners();
    this.m_OwningWidget = this.GetComponent<WidgetTemplate>();
  }

  public override void Start()
  {
    base.Start();
    this.m_luckyDrawManager.InitializeOrUpdateData();
    this.m_luckyDrawButtonDataModel = this.m_luckyDrawManager.GetLuckyDrawButtonDataModel();
    this.m_PlayButtonReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnPlayButtonReady));
    this.m_BackButtonReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnBackButtonReady));
    this.m_PlayButtonPhoneReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnPlayButtonReady));
    this.m_BackButtonPhoneReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnBackButtonReady));
    this.m_StatsButtonReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnStatsButtonReady));
    this.m_StatsButtonPhoneReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnStatsButtonReady));
    this.m_StatsPageReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnStatsPagePCReady));
    this.m_StatsPagePhoneReference.RegisterReadyListener<VisualController>(new System.Action<VisualController>(this.OnStatsPagePhoneReady));
    this.m_LobbyReference.RegisterReadyListener<Widget>(new System.Action<Widget>(this.OnLobbyPCReady));
    this.m_LobbyPhoneReference.RegisterReadyListener<Widget>(new System.Action<Widget>(this.OnLobbyPhoneReady));
    this.m_LobbyReference.RegisterReadyListener<Widget>(new System.Action<Widget>(this.OnPartyPCReady));
    this.m_LobbyPhoneReference.RegisterReadyListener<Widget>(new System.Action<Widget>(this.OnPartyPhoneReady));
    this.m_luckyDrawButtonReference.RegisterReadyListener<WidgetTemplate>(new System.Action<WidgetTemplate>(this.OnLuckyDrawButtonReady));
    NetCache.Get().RegisterScreenBattlegrounds(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    PartyManager.Get().AddChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
    this.InitializeBaconLobbyData();
    NarrativeManager.Get().OnBattlegroundsEntered();
    MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_Battlegrounds);
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.BATTLEGROUNDS_SCREEN);
    StoreManager.Get().RegisterStatusChangedListener(new System.Action<bool>(this.OnStoreStatusChanged));
  }

  private void OnDestroy()
  {
    this.HideLuckyDrawPopups();
    this.UnregisterListeners();
  }

  private void BaconDisplayEventListener(string eventName)
  {
    if (!(eventName == "OpenShop"))
    {
      if (!(eventName == "OpenCollection"))
      {
        if (!(eventName == "CODE_STATS_SLIDE_FINISHED"))
        {
          if (!(eventName == "CODE_STATS_PHONE_SLIDE_FINISHED"))
            return;
          this.StatsPanelPhoneFinishedLoading();
        }
        else
          this.StatsPanelFinishedLoading();
      }
      else
      {
        this.HideLuckyDrawPopups();
        this.OpenBattlegroundsCollection();
      }
    }
    else
    {
      this.HideLuckyDrawPopups();
      this.OpenBattlegroundsShop();
    }
  }

  private void OnPartyPCReady(Widget widget)
  {
    if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.HUB || (bool) UniversalInputManager.UsePhoneUI)
      this.m_partyFinishedLoading = true;
    else
      this.m_partyFinishedLoading = false;
  }

  private void OnPartyPhoneReady(Widget widget)
  {
    if (SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.HUB || !(bool) UniversalInputManager.UsePhoneUI)
      this.m_partyPhoneFinishedLoading = true;
    else
      this.m_partyPhoneFinishedLoading = false;
  }

  private void StatsPanelFinishedLoading() => this.m_partyFinishedLoading = true;

  private void OnLuckyDrawExpired()
  {
    if (SceneMgr.Get().GetMode() != SceneMgr.Mode.BACON || !((UnityEngine.Object) this.m_luckyDrawButton != (UnityEngine.Object) null))
      return;
    this.m_luckyDrawButton.gameObject.SetActive(false);
    this.StartCoroutine(this.WaitThenShowLuckDrawEndPopupIfExistsUnusedHammers());
  }

  private IEnumerator WaitThenShowLuckDrawEndPopupIfExistsUnusedHammers()
  {
    BaconDisplay baconDisplay = this;
    while (baconDisplay.m_luckyDrawManager.IsDataDirty())
      yield return (object) new WaitForSeconds(0.1f);
    if (baconDisplay.m_luckyDrawManager.GetBattlegroundsLuckyDrawDataModel().Hammers > 0)
    {
      // ISSUE: reference to a compiler-generated method
      DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLUE_BATTLEBASH_ALERT_EVENT_END_TITLE"),
        m_text = GameStrings.Get("GLUE_BATTLEBASH_ALERT_EVENT_END_DESCRIPTION"),
        m_iconSet = AlertPopup.PopupInfo.IconSet.Default,
        m_showAlertIcon = true,
        m_alertTextAlignment = UberText.AlignmentOptions.Center,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK,
        m_responseCallback = new AlertPopup.ResponseCallback(baconDisplay.\u003CWaitThenShowLuckDrawEndPopupIfExistsUnusedHammers\u003Eb__58_0)
      });
    }
  }

  private void StatsPanelPhoneFinishedLoading() => this.m_partyPhoneFinishedLoading = true;

  public void OnPlayButtonReady(VisualController buttonVisualController)
  {
    if ((UnityEngine.Object) buttonVisualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "PlayButton could not be found! You will not be able to click 'Play'!");
    }
    else
    {
      this.m_playButton = buttonVisualController.gameObject.GetComponent<PlayButton>();
      this.m_playButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.PlayButtonRelease));
      this.UpdatePlayButtonBasedOnPartyInfo();
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
      buttonVisualController.gameObject.GetComponent<UIBButton>().AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.BackButtonRelease));
      this.m_backButtonFinishedLoading = true;
    }
  }

  public void OnStatsButtonReady(VisualController buttonVisualController)
  {
    if ((UnityEngine.Object) buttonVisualController == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "StatsButton could not be found! You will not be able to show 'Stats'!");
    }
    else
    {
      this.m_statsButton = buttonVisualController.gameObject.GetComponent<UIBButton>();
      this.m_statsButtonClickable = buttonVisualController.gameObject.GetComponent<Clickable>();
      this.UpdateStatsButtonState();
      this.m_statsButtonFinishedLoading = true;
    }
  }

  public void OnLuckyDrawButtonReady(WidgetTemplate button)
  {
    if ((UnityEngine.Object) button == (UnityEngine.Object) null)
    {
      Error.AddDevWarning("UI Error!", "Lucky Draw Button could not be found! Cant show Lucky Draw button!");
    }
    else
    {
      this.m_luckyDrawButton = button.GetComponentInChildren<LuckyDrawButton>();
      WidgetTemplate component = this.m_luckyDrawButton.GetComponent<WidgetTemplate>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      {
        Error.AddDevWarning("UI Error!", "Could not find widget component of Lucky Draw button! Cant show lucky Draw button!");
      }
      else
      {
        component.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
        {
          if (!(eventName == "BUTTON_CLICKED"))
            return;
          this.LuckyDrawButtonClicked();
        }));
        this.m_luckyDrawButtonFinishedLoading = true;
        this.StartCoroutine("WaitThenShowLuckyDrawPopups");
      }
    }
  }

  private IEnumerator WaitThenShowLuckyDrawPopups()
  {
    BaconDisplay baconDisplay = this;
    while (baconDisplay.m_luckyDrawManager.IsDataDirty())
      yield return (object) new WaitForSeconds(0.1f);
    if (baconDisplay.m_luckyDrawManager.HasActiveLuckyDrawBox() && LuckyDrawManager.Get().GetLuckyDrawButtonDataModel().LuckyDrawEnabled)
    {
      NarrativeManager.Get().OnBattlegroundsLuckyDrawButtonShown();
      long num;
      GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.FTUE, GameSaveKeySubkeyId.FTUE_HAS_SEEN_BATTLE_BASH_BUTTON_TOOLTIP, out num);
      if (num <= 0L)
      {
        yield return (object) new WaitForSeconds(4f);
        baconDisplay.m_luckyDrawManager.SetShowHighlight(true);
        baconDisplay.ShowLuckyDrawFTUENotification();
      }
      else
      {
        bool showUnackHammerPopup = baconDisplay.ShouldShowBattlegroundsUnacknowledgedEarnedHammersPopUp() && baconDisplay.m_rewardPresenter != null;
        if (showUnackHammerPopup)
        {
          yield return (object) new WaitForSeconds(3f);
          while (baconDisplay.m_rewardPresenter.IsShowingReward() || PopupDisplayManager.Get().IsShowing)
            yield return (object) new WaitForSeconds(0.1f);
          baconDisplay.m_luckyDrawManager.SetShowHighlight(true);
          RewardScrollDataModel dataModel = new RewardScrollDataModel()
          {
            DisplayName = GameStrings.Get("GLUE_BACON_REWARD_BATTLE_BASH_HAMMER"),
            Description = GameStrings.Get("GLUE_BACON_TOP_4_REWARD_DESC"),
            RewardList = new RewardListDataModel()
            {
              Items = new DataModelList<RewardItemDataModel>()
              {
                new RewardItemDataModel()
                {
                  Quantity = baconDisplay.m_luckyDrawManager.NumUnacknowledgedEarnedHammers(),
                  ItemType = RewardItemType.BATTLEGROUNDS_BATTLE_BASH_HAMMER
                }
              }
            }
          };
          baconDisplay.m_rewardPresenter.EnqueueReward(dataModel, (System.Action) (() => { }));
          baconDisplay.m_rewardPresenter.ShowNextReward(new System.Action(baconDisplay.OnLuckyDrawUnacknowledgedHammerPopupDismissed));
        }
        if (baconDisplay.ShouldShowLuckyDrawEndsSoonPopup())
        {
          yield return (object) new WaitForSeconds(showUnackHammerPopup ? 1f : 3f);
          while (baconDisplay.m_rewardPresenter.IsShowingReward() || PopupDisplayManager.Get().IsShowing)
            yield return (object) new WaitForSeconds(0.1f);
          DialogManager dialogManager = DialogManager.Get();
          if (!((UnityEngine.Object) dialogManager == (UnityEngine.Object) null))
          {
            // ISSUE: reference to a compiler-generated method
            dialogManager.ShowBattlegroundsLuckyDrawEndSoonPopup(baconDisplay.m_luckyDrawManager.GetBattlegroundsLuckyDrawDataModel(), new DialogManager.DialogProcessCallback(baconDisplay.\u003CWaitThenShowLuckyDrawPopups\u003Eb__64_1));
          }
        }
      }
    }
  }

  private void ShowLuckyDrawFTUENotification()
  {
    string key = GameStrings.Get("GLUE_BATTLEBASH_FTUE_HINT");
    GameObject childBySubstring = GameObjectUtils.FindChildBySubstring(this.m_luckyDrawButton.gameObject, "FTUETooltip" + (string) this.PLATFORM_DEPENDENT_BONE_SUFFIX);
    if ((UnityEngine.Object) childBySubstring == (UnityEngine.Object) null)
    {
      Log.All.PrintWarning("[BaconDisplay.ShowLuckyDrawFTUENotifiation] - Popup bone is missing");
    }
    else
    {
      NotificationManager notificationManager = NotificationManager.Get();
      this.m_luckyDrawFTUENotification = !(bool) UniversalInputManager.UsePhoneUI ? notificationManager.CreatePopupText(UserAttentionBlocker.NONE, childBySubstring.transform.position, childBySubstring.transform.localScale, GameStrings.Get(key)) : notificationManager.CreatePopupText(UserAttentionBlocker.NONE, childBySubstring.transform.position, childBySubstring.transform.localScale, GameStrings.Get(key));
      this.m_luckyDrawFTUENotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Up);
    }
  }

  private void HideLuckyDrawPopups(bool animate = false)
  {
    this.StopCoroutine("WaitThenShowLuckyDrawPopups");
    if ((UnityEngine.Object) this.m_luckyDrawFTUENotification == (UnityEngine.Object) null)
      return;
    if (animate)
      NotificationManager.Get()?.DestroyNotification(this.m_luckyDrawFTUENotification, 0.0f);
    else
      NotificationManager.Get()?.DestroyNotificationNowWithNoAnim(this.m_luckyDrawFTUENotification);
  }

  private void LuckyDrawButtonClicked()
  {
    this.SetNextModeAndHandleTransition(SceneMgr.Mode.LUCKY_DRAW, SceneMgr.TransitionHandlerType.CURRENT_SCENE, (object) null);
    this.HideLuckyDrawPopups();
    this.m_luckyDrawButton.SetUserInteractionEnabled(false);
  }

  private void UpdateStatsButtonState()
  {
    if ((UnityEngine.Object) this.m_statsButton == (UnityEngine.Object) null || (UnityEngine.Object) this.m_statsButtonClickable == (UnityEngine.Object) null)
      return;
    bool statsPage = this.HasAccessToStatsPage();
    this.m_statsButton.Flip(statsPage, true);
    this.m_statsButton.SetEnabled(statsPage);
    this.m_statsButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnStatsButtonReleased));
    this.m_statsButtonClickable.Active = statsPage;
  }

  private void OnStatsButtonReleased(UIEvent e) => this.HideLuckyDrawPopups();

  public void PlayButtonRelease(UIEvent e)
  {
    this.HideLuckyDrawPopups();
    if (!BattleNet.IsConnected() || GameMgr.Get().IsFindingGame())
      return;
    PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.BATTLEGROUNDS_QUEUE);
    bool flag = !NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.BattlegroundsTutorial;
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.BACON, GameSaveKeySubkeyId.BACON_HAS_SEEN_TUTORIAL, out num);
    PartyManager partyManager = PartyManager.Get();
    if (partyManager.IsInParty() && partyManager.IsInBattlegroundsParty() && partyManager.IsPartyLeader())
      partyManager.FindGame();
    else if (num == 0L && !flag)
      this.PlayBaconTutorial();
    else
      GameMgr.Get().FindGame(GameType.GT_BATTLEGROUNDS, PegasusShared.FormatType.FT_WILD, 3459);
  }

  public void PlayBaconTutorial() => GameMgr.Get().FindGame(GameType.GT_VS_AI, PegasusShared.FormatType.FT_WILD, 3539);

  public void BackButtonRelease(UIEvent e)
  {
    this.HideLuckyDrawPopups();
    if (PartyManager.Get().IsInBattlegroundsParty())
      this.ShowLeavePartyDialog();
    else
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
  }

  private void ShowLeavePartyDialog() => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLUE_BACON_LEAVE_PARTY_CONFIRMATION_HEADER"),
    m_text = PartyManager.Get().IsPartyLeader() ? GameStrings.Get("GLUE_BACON_DISBAND_PARTY_CONFIRMATION_BODY") : GameStrings.Get("GLUE_BACON_LEAVE_PARTY_CONFIRMATION_BODY"),
    m_iconSet = AlertPopup.PopupInfo.IconSet.Default,
    m_showAlertIcon = false,
    m_alertTextAlignment = UberText.AlignmentOptions.Center,
    m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
    m_confirmText = GameStrings.Get("GLUE_BACON_LEAVE_PARTY_CONFIRMATION_CONFIRM"),
    m_cancelText = GameStrings.Get("GLUE_BACON_LEAVE_PARTY_CONFIRMATION_CANCEL"),
    m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
    {
      if (response != AlertPopup.Response.CONFIRM)
        return;
      BaconParty.Get().LeaveParty();
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    })
  });

  private void OnNetCacheReady()
  {
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    if (NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.Battlegrounds || SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
      return;
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
    Error.AddWarningLoc("GLOBAL_FEATURE_DISABLED_TITLE", "GLOBAL_FEATURE_DISABLED_MESSAGE_BATTLEGROUNDS");
  }

  private void OnStatsPagePCReady(VisualController visualController)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "StatsPage could not be found! You will not be able to view stats!");
    else
      this.InitializeBaconStatsPageData(visualController);
  }

  private void OnStatsPagePhoneReady(VisualController visualController)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "StatsPage could not be found! You will not be able to view stats!");
    else
      this.InitializeBaconStatsPageData(visualController);
  }

  private void OnLobbyPCReady(Widget widget)
  {
    if ((bool) UniversalInputManager.UsePhoneUI)
      return;
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "LobbyReference could not be found!");
    else
      widget.RegisterEventListener(new Widget.EventListenerDelegate(this.BaconDisplayEventListener));
  }

  private void OnLobbyPhoneReady(Widget widget)
  {
    if (!(bool) UniversalInputManager.UsePhoneUI)
      return;
    if ((UnityEngine.Object) widget == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error!", "LobbyReference could not be found!");
    else
      widget.RegisterEventListener(new Widget.EventListenerDelegate(this.BaconDisplayEventListener));
  }

  public BaconLobbyDataModel GetBaconLobbyDataModel()
  {
    VisualController component = this.GetComponent<VisualController>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return (BaconLobbyDataModel) null;
    Widget owner = (Widget) component.Owner;
    IDataModel model;
    if (!owner.GetDataModel(43, out model))
    {
      model = (IDataModel) new BaconLobbyDataModel();
      owner.BindDataModel(model);
    }
    return model as BaconLobbyDataModel;
  }

  private void InitializeBaconLobbyData()
  {
    BaconLobbyDataModel baconLobbyDataModel = this.GetBaconLobbyDataModel();
    if (baconLobbyDataModel == null)
      return;
    NetCache.NetCacheBaconRatingInfo netObject1 = NetCache.Get().GetNetObject<NetCache.NetCacheBaconRatingInfo>();
    if (netObject1 != null)
      baconLobbyDataModel.Rating = netObject1.Rating;
    else
      Log.Net.PrintError("No bacon rating info in NetCache.");
    baconLobbyDataModel.Top4Finishes = (int) this.GetBaconGameSaveValue(GameSaveKeySubkeyId.BACON_TOP_4_FINISHES);
    baconLobbyDataModel.FirstPlaceFinishes = (int) this.GetBaconGameSaveValue(GameSaveKeySubkeyId.BACON_FIRST_PLACE_FINISHES);
    baconLobbyDataModel.ShopOpen = StoreManager.Get().IsOpen();
    NetCache.NetCacheFeatures netObject2 = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    baconLobbyDataModel.BattlegroundsSkinsEnabled = netObject2.BattlegroundsSkinsEnabled;
    baconLobbyDataModel.BattlegroundsRewardTrackEnabled = netObject2.BattlegroundsRewardTrackEnabled;
    baconLobbyDataModel.HasNewProducts = this.HasNewBattlegroundsProducts();
    baconLobbyDataModel.HasNewSkins = CollectionManager.Get().HasAnyNewBattlegroundsSkins();
    baconLobbyDataModel.LuckyDraw = this.m_luckyDrawManager.GetBattlegroundsLuckyDrawDataModel();
  }

  private void RegisterListeners()
  {
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    NetCache.Get().OwnedBattlegroundsSkinsChanged += new NetCache.DelOwnedBattlegroundsSkinsChanged(this.RefreshHasAnyNewSkins);
    GameMgr.Get().OnTransitionPopupShown += new System.Action(this.OnTransitionPopupShown);
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPresenceUpdated));
    BnetNearbyPlayerMgr.Get().AddChangeListener(new BnetNearbyPlayerMgr.ChangeCallback(this.OnNearbyPlayersUpdated));
    this.m_luckyDrawManager.RegisterOnEventEndsListeners(new System.Action(this.OnLuckyDrawExpired));
  }

  private void UnregisterListeners()
  {
    if (GameMgr.Get() != null)
    {
      GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
      GameMgr.Get().OnTransitionPopupShown -= new System.Action(this.OnTransitionPopupShown);
    }
    if (NetCache.Get() != null)
      NetCache.Get().OwnedBattlegroundsSkinsChanged -= new NetCache.DelOwnedBattlegroundsSkinsChanged(this.RefreshHasAnyNewSkins);
    if (PartyManager.Get() != null)
      PartyManager.Get().RemoveChangedListener(new PartyManager.ChangedCallback(this.OnPartyChanged));
    if (BnetPresenceMgr.Get() != null)
      BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPresenceUpdated));
    if (BnetNearbyPlayerMgr.Get() != null)
      BnetNearbyPlayerMgr.Get().RemoveChangeListener(new BnetNearbyPlayerMgr.ChangeCallback(this.OnNearbyPlayersUpdated));
    if (StoreManager.Get() != null)
      StoreManager.Get().RemoveStatusChangedListener(new System.Action<bool>(this.OnStoreStatusChanged));
    if (this.m_luckyDrawManager == null)
      return;
    this.m_luckyDrawManager.RemoveOnEventEndsListenders(new System.Action(this.OnLuckyDrawExpired));
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
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.BATTLEGROUNDS_SCREEN);
        this.UpdatePlayButtonBasedOnPartyInfo();
        break;
    }
    return false;
  }

  private void OnTransitionPopupShown()
  {
    Shop.Get().Close();
    DialogManager.Get().ClearAllImmediately();
  }

  private void ShowLowMemoryAlertMessage()
  {
    if (!(bool) this.ShowLowMemoryWarning || BaconDisplay.m_hasSeenLowMemoryWarningThisSession)
      return;
    BaconDisplay.m_hasSeenLowMemoryWarningThisSession = true;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_BACON_LOW_MEMORY_HEADER"),
      m_text = GameStrings.Get("GLUE_BACON_LOW_MEMORY_BODY"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  public BaconStatsPageDataModel GetBaconStatsPageDataModel(
    VisualController visualController)
  {
    if ((UnityEngine.Object) visualController == (UnityEngine.Object) null)
      return (BaconStatsPageDataModel) null;
    Widget owner = (Widget) visualController.Owner;
    IDataModel model;
    if (!owner.GetDataModel(122, out model))
    {
      model = (IDataModel) new BaconStatsPageDataModel();
      owner.BindDataModel(model);
    }
    return model as BaconStatsPageDataModel;
  }

  private void InitializeBaconStatsPageData(VisualController visualController)
  {
    BaconStatsPageDataModel dataModel = this.GetBaconStatsPageDataModel(visualController);
    if (dataModel == null)
      return;
    dataModel.Top4Finishes = (int) this.GetBaconGameSaveValue(GameSaveKeySubkeyId.BACON_TOP_4_FINISHES);
    dataModel.FirstPlaceFinishes = (int) this.GetBaconGameSaveValue(GameSaveKeySubkeyId.BACON_FIRST_PLACE_FINISHES);
    dataModel.TriplesCreated = (int) this.GetBaconGameSaveValue(GameSaveKeySubkeyId.BACON_TRIPLES_CREATED);
    dataModel.TavernUpgrades = (int) this.GetBaconGameSaveValue(GameSaveKeySubkeyId.BACON_TAVERN_UPGRADES);
    dataModel.DamageInOneTurn = (int) this.GetBaconGameSaveValue(GameSaveKeySubkeyId.BACON_MOST_DAMAGE_ONE_TURN);
    dataModel.LongestWinStreak = (int) this.GetBaconGameSaveValue(GameSaveKeySubkeyId.BACON_LONGEST_COMBAT_WIN_STREAK);
    dataModel.SecondsPlayed = (int) this.GetBaconGameSaveValue(GameSaveKeySubkeyId.BACON_TIME_PLAYED);
    List<long> gameSaveValueList1 = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_MINIONS_KILLED_COUNT);
    dataModel.MinionsDestroyed = gameSaveValueList1 == null ? 0 : (int) gameSaveValueList1.Sum();
    List<long> gameSaveValueList2 = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_HEROES_KILLED_COUNT);
    dataModel.PlayersEliminated = gameSaveValueList2 == null ? 0 : (int) gameSaveValueList2.Sum();
    List<long> gameSaveValueList3 = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_LARGEST_MINION_ATTACK_HEALTH);
    BaconStatsPageDataModel statsPageDataModel = dataModel;
    CardDataModel cardDataModel1;
    if (gameSaveValueList3 != null && gameSaveValueList3.Count<long>() >= 3)
      cardDataModel1 = new CardDataModel()
      {
        CardId = GameUtils.TranslateDbIdToCardId((int) gameSaveValueList3[0]),
        Premium = TAG_PREMIUM.NORMAL
      };
    else
      cardDataModel1 = (CardDataModel) null;
    statsPageDataModel.BiggestMinionId = cardDataModel1;
    dataModel.BiggestMinionAttack = gameSaveValueList3 == null || gameSaveValueList3.Count<long>() < 3 ? 0 : (int) gameSaveValueList3[1];
    dataModel.BiggestMinionHealth = gameSaveValueList3 == null || gameSaveValueList3.Count<long>() < 3 ? 0 : (int) gameSaveValueList3[2];
    dataModel.BiggestMinionString = GameStrings.Format("GLUE_BACON_STATS_VALUE_BIGGEST_MINION", (object) dataModel.BiggestMinionAttack, (object) dataModel.BiggestMinionHealth);
    if (dataModel.SecondsPlayed > 3600)
      dataModel.TimePlayedString = GameStrings.Format("GLUE_BACON_STATS_VALUE_HOURS_PLAYED", (object) Mathf.FloorToInt((float) (dataModel.SecondsPlayed / 3600)));
    else
      dataModel.TimePlayedString = GameStrings.Format("GLUE_BACON_STATS_VALUE_MINUTES_PLAYED", (object) Mathf.FloorToInt((float) (dataModel.SecondsPlayed / 60)));
    List<KeyValuePair<long, long>> gameSaveDataLists1 = this.GetSortedListFromGameSaveDataLists(this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_BOUGHT_MINIONS), this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_BOUGHT_MINIONS_COUNT));
    dataModel.MostBoughtMinionsCardIds = new DataModelList<CardDataModel>();
    dataModel.MostBoughtMinionsCardIds.AddRange(gameSaveDataLists1.Select<KeyValuePair<long, long>, CardDataModel>((Func<KeyValuePair<long, long>, CardDataModel>) (kvp => new CardDataModel()
    {
      CardId = GameUtils.TranslateDbIdToCardId((int) kvp.Key),
      Premium = TAG_PREMIUM.NORMAL
    })));
    dataModel.MostBoughtMinionsCount = new DataModelList<int>();
    dataModel.MostBoughtMinionsCount.AddRange(gameSaveDataLists1.Select<KeyValuePair<long, long>, int>((Func<KeyValuePair<long, long>, int>) (kvp => (int) kvp.Value)));
    List<KeyValuePair<long, long>> gameSaveDataLists2 = this.GetSortedListFromGameSaveDataLists(this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_HEROES_WON_WITH), this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_HEROES_WON_WITH_COUNT));
    dataModel.TopHeroesByWinCardIds = new DataModelList<CardDataModel>();
    dataModel.TopHeroesByWinCardIds.AddRange(gameSaveDataLists2.Select<KeyValuePair<long, long>, CardDataModel>((Func<KeyValuePair<long, long>, CardDataModel>) (kvp => new CardDataModel()
    {
      CardId = CollectionManager.Get().GetFavoriteBattleGroundsHeroSkinCardId((int) kvp.Key),
      Premium = TAG_PREMIUM.NORMAL
    })));
    dataModel.TopHeroesByWinCount = new DataModelList<int>();
    dataModel.TopHeroesByWinCount.AddRange(gameSaveDataLists2.Select<KeyValuePair<long, long>, int>((Func<KeyValuePair<long, long>, int>) (kvp => (int) kvp.Value)));
    List<KeyValuePair<long, long>> gameSaveDataLists3 = this.GetSortedListFromGameSaveDataLists(this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_HEROES_PICKED), this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_HEROES_PICKED_COUNT));
    dataModel.TopHeroesByGamesPlayedCardIds = new DataModelList<CardDataModel>();
    dataModel.TopHeroesByGamesPlayedCardIds.AddRange(gameSaveDataLists3.Select<KeyValuePair<long, long>, CardDataModel>((Func<KeyValuePair<long, long>, CardDataModel>) (kvp => new CardDataModel()
    {
      CardId = CollectionManager.Get().GetFavoriteBattleGroundsHeroSkinCardId((int) kvp.Key),
      Premium = TAG_PREMIUM.NORMAL
    })));
    dataModel.TopHeroesByGamesPlayedCount = new DataModelList<int>();
    dataModel.TopHeroesByGamesPlayedCount.AddRange(gameSaveDataLists3.Select<KeyValuePair<long, long>, int>((Func<KeyValuePair<long, long>, int>) (kvp => (int) kvp.Value)));
    dataModel.PastGames = new DataModelList<BaconPastGameStatsDataModel>();
    List<long> gameSaveValueList4 = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_HEROES);
    List<long> gameSaveValueList5 = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_PLACES);
    List<long> gameSaveValueList6 = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_MINIONS_ID);
    List<long> gameSaveValueList7 = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_MINIONS_ATTACK);
    List<long> gameSaveValueList8 = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_MINIONS_HEALTH);
    List<long> gameSaveValueList9 = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_MINIONS_GOLDEN);
    List<long> tauntList = new List<long>();
    List<long> divineShieldList = new List<long>();
    List<long> poisonousList = new List<long>();
    List<long> windfuryList = new List<long>();
    List<long> rebornList = new List<long>();
    List<long> questIDList = new List<long>();
    List<long> rewardIDList = new List<long>();
    List<long> rewardIsCompletedList = new List<long>();
    List<long> rewardCardDatabaseIDList = new List<long>();
    List<long> rewardMinionTypeList = new List<long>();
    List<long> questProgressTotalList = new List<long>();
    List<long> questRace1List = new List<long>();
    List<long> questRace2List = new List<long>();
    this.PopulateAdditionalInfoLists(gameSaveValueList4 == null ? 0 : gameSaveValueList4.Count, ref tauntList, ref divineShieldList, ref poisonousList, ref windfuryList, ref rebornList, ref questIDList, ref rewardIDList, ref rewardIsCompletedList, ref rewardCardDatabaseIDList, ref rewardMinionTypeList, ref questProgressTotalList, ref questRace1List, ref questRace2List);
    List<BaconPastGameStatsDataModel> gameStatsDataModelList = new List<BaconPastGameStatsDataModel>();
    for (int index1 = 0; index1 < 5 && gameSaveValueList4 != null && index1 < gameSaveValueList4.Count && index1 < gameSaveValueList5.Count; ++index1)
    {
      string cardId1 = GameUtils.TranslateDbIdToCardId((int) gameSaveValueList4[index1]);
      CardDataModel cardDataModel2 = new CardDataModel()
      {
        CardId = CollectionManager.Get().GetFavoriteBattleGroundsHeroSkinCardId((int) gameSaveValueList4[index1]),
        Premium = TAG_PREMIUM.NORMAL
      };
      CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardId1);
      string str = cardRecord == null ? "" : (string) cardRecord.Name;
      CardDataModel cardDataModel3 = new CardDataModel()
      {
        CardId = GameUtils.GetHeroPowerCardIdFromHero((int) gameSaveValueList4[index1]),
        Premium = TAG_PREMIUM.NORMAL,
        SpellTypes = new DataModelList<SpellType>()
        {
          SpellType.COIN_MANA_GEM
        }
      };
      string cardId2 = GameUtils.TranslateDbIdToCardId((int) questIDList[index1]);
      string cardId3 = GameUtils.TranslateDbIdToCardId((int) rewardIDList[index1]);
      CardDataModel cardDataModel4 = new CardDataModel()
      {
        CardId = cardId2,
        Premium = TAG_PREMIUM.NORMAL
      };
      CardDataModel cardDataModel5 = new CardDataModel()
      {
        CardId = cardId3,
        Premium = TAG_PREMIUM.NORMAL
      };
      DataModelList<CardDataModel> dataModelList1 = new DataModelList<CardDataModel>();
      for (int index2 = 0; index2 < 7; ++index2)
      {
        int index3 = index1 * 7 + index2;
        if (gameSaveValueList6.Count <= index3 || gameSaveValueList7.Count <= index3 || gameSaveValueList8.Count <= index3 || gameSaveValueList9.Count <= index3 || tauntList.Count <= index3 || divineShieldList.Count <= index3 || poisonousList.Count <= index3 || windfuryList.Count <= index3 || rebornList.Count <= index3)
        {
          Debug.LogErrorFormat("Missing Minion Data for GameIndex={0}, MinionIndex={1}", (object) index1, (object) index3);
          break;
        }
        if (gameSaveValueList6[index3] != 0L)
        {
          DataModelList<SpellType> dataModelList2 = new DataModelList<SpellType>();
          bool flag = gameSaveValueList9[index3] > 0L;
          if (tauntList[index3] > 0L)
            dataModelList2.Add(flag ? SpellType.TAUNT_INSTANT_PREMIUM : SpellType.TAUNT_INSTANT);
          if (divineShieldList[index3] > 0L)
            dataModelList2.Add(SpellType.DIVINE_SHIELD);
          if (poisonousList[index3] > 0L)
            dataModelList2.Add(SpellType.POISONOUS);
          if (windfuryList[index3] > 0L)
            dataModelList2.Add(SpellType.WINDFURY_IDLE);
          if (rebornList[index3] > 0L)
            dataModelList2.Add(SpellType.REBORN);
          dataModelList1.Add(new CardDataModel()
          {
            CardId = GameUtils.TranslateDbIdToCardId((int) gameSaveValueList6[index3]),
            Premium = flag ? TAG_PREMIUM.GOLDEN : TAG_PREMIUM.NORMAL,
            Attack = (int) gameSaveValueList7[index3],
            Health = (int) gameSaveValueList8[index3],
            SpellTypes = dataModelList2
          });
        }
        else
          break;
      }
      gameStatsDataModelList.Add(new BaconPastGameStatsDataModel()
      {
        Hero = cardDataModel2,
        HeroPower = cardDataModel3,
        HeroName = str,
        Place = (int) gameSaveValueList5[index1],
        Minions = dataModelList1,
        Reward = cardDataModel5,
        Quest = cardDataModel4,
        RewardCompleted = rewardIsCompletedList[index1] != 0L,
        RewardCardDatabaseID = (int) rewardCardDatabaseIDList[index1],
        RewardMinionType = (int) rewardMinionTypeList[index1],
        QuestProgressTotal = (int) questProgressTotalList[index1],
        QuestRace1 = (int) questRace1List[index1],
        QuestRace2 = (int) questRace2List[index1]
      });
    }
    gameStatsDataModelList.Reverse();
    gameStatsDataModelList.ForEach((System.Action<BaconPastGameStatsDataModel>) (g => dataModel.PastGames.Add(g)));
  }

  private void PopulateAdditionalInfoLists(
    int pastGames,
    ref List<long> tauntList,
    ref List<long> divineShieldList,
    ref List<long> poisonousList,
    ref List<long> windfuryList,
    ref List<long> rebornList,
    ref List<long> questIDList,
    ref List<long> rewardIDList,
    ref List<long> rewardIsCompletedList,
    ref List<long> rewardCardDatabaseIDList,
    ref List<long> rewardMinionTypeList,
    ref List<long> questProgressTotalList,
    ref List<long> questRace1List,
    ref List<long> questRace2List)
  {
    tauntList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_MINIONS_TAUNT);
    divineShieldList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_MINIONS_DIVINE_SHIELD);
    poisonousList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_MINIONS_POISONOUS);
    windfuryList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_MINIONS_WINDFURY);
    rebornList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_MINIONS_REBORN);
    questIDList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_QUEST_IDS);
    rewardIDList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_REWARD_IDS);
    rewardIsCompletedList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_REWARD_IS_COMPLETED);
    rewardCardDatabaseIDList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_REWARD_CARD_DATABASE_ID);
    rewardMinionTypeList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_REWARD_MINION_TYPE);
    questProgressTotalList = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_QUEST_PROGRESS_TOTAL);
    questRace1List = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_QUEST_RACE_1);
    questRace2List = this.GetBaconGameSaveValueList(GameSaveKeySubkeyId.BACON_PAST_GAME_QUEST_RACE_2);
    if (tauntList == null)
      tauntList = new List<long>();
    if (divineShieldList == null)
      divineShieldList = new List<long>();
    if (poisonousList == null)
      poisonousList = new List<long>();
    if (windfuryList == null)
      windfuryList = new List<long>();
    if (rebornList == null)
      rebornList = new List<long>();
    if (questIDList == null)
      questIDList = new List<long>();
    if (rewardIDList == null)
      rewardIDList = new List<long>();
    if (rewardIsCompletedList == null)
      rewardIsCompletedList = new List<long>();
    if (rewardCardDatabaseIDList == null)
      rewardCardDatabaseIDList = new List<long>();
    if (rewardMinionTypeList == null)
      rewardMinionTypeList = new List<long>();
    if (questProgressTotalList == null)
      questProgressTotalList = new List<long>();
    if (questRace1List == null)
      questRace1List = new List<long>();
    if (questRace2List == null)
      questRace2List = new List<long>();
    while (tauntList.Count < pastGames * 7)
      tauntList.Insert(0, 0L);
    while (divineShieldList.Count < pastGames * 7)
      divineShieldList.Insert(0, 0L);
    while (poisonousList.Count < pastGames * 7)
      poisonousList.Insert(0, 0L);
    while (windfuryList.Count < pastGames * 7)
      windfuryList.Insert(0, 0L);
    while (rebornList.Count < pastGames * 7)
      rebornList.Insert(0, 0L);
    while (questIDList.Count < pastGames)
      questIDList.Insert(0, 0L);
    while (rewardIDList.Count < pastGames)
      rewardIDList.Insert(0, 0L);
    while (rewardIsCompletedList.Count < pastGames)
      rewardIsCompletedList.Insert(0, 0L);
    while (rewardCardDatabaseIDList.Count < pastGames)
      rewardCardDatabaseIDList.Insert(0, 0L);
    while (rewardMinionTypeList.Count < pastGames)
      rewardMinionTypeList.Insert(0, 0L);
    while (questProgressTotalList.Count < pastGames)
      questProgressTotalList.Insert(0, 0L);
    while (questRace1List.Count < pastGames)
      questRace1List.Insert(0, 0L);
    while (questRace2List.Count < pastGames)
      questRace2List.Insert(0, 0L);
  }

  private long GetBaconGameSaveValue(GameSaveKeySubkeyId subkey)
  {
    long baconGameSaveValue;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.BACON, subkey, out baconGameSaveValue);
    return baconGameSaveValue;
  }

  private List<long> GetBaconGameSaveValueList(GameSaveKeySubkeyId subkey)
  {
    List<long> values;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.BACON, subkey, out values);
    return values;
  }

  private List<KeyValuePair<long, long>> GetSortedListFromGameSaveDataLists(
    List<long> keys,
    List<long> values)
  {
    List<KeyValuePair<long, long>> source = new List<KeyValuePair<long, long>>();
    if (keys == null || values == null)
      return source;
    if (keys.Count != values.Count)
    {
      Debug.LogError((object) "GetSortedListFromGameSaveDataLists: Stats Page Game Save Data Lists Length Not Equal!");
      return source;
    }
    for (int index = 0; index < keys.Count; ++index)
      source.Add(new KeyValuePair<long, long>(keys[index], values[index]));
    return source.OrderByDescending<KeyValuePair<long, long>, long>((Func<KeyValuePair<long, long>, long>) (kvp => kvp.Value)).ToList<KeyValuePair<long, long>>();
  }

  private bool HasAccessToStatsPage() => true;

  private void OpenBattlegroundsShop()
  {
    if (!this.GetBaconLobbyDataModel().ShopOpen)
      return;
    if (!this.HasBattlegroundsProductsAvailable())
      this.ShowBattlegroundsStoreEmptyPopup();
    else
      StoreManager.Get().StartBattlegroundsTransaction(new Store.ExitCallback(this.OnStoreBackButtonPressed), false);
  }

  private void OnStoreBackButtonPressed(bool authorizationBackButtonPressed, object userData) => this.GetBaconLobbyDataModel().HasNewProducts = this.HasNewBattlegroundsProducts();

  private void OpenBattlegroundsCollection()
  {
    CollectionManager.Get().NotifyOfBoxTransitionStart();
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.BACON_COLLECTION);
  }

  private void RefreshHasAnyNewSkins() => this.GetBaconLobbyDataModel().HasNewSkins = CollectionManager.Get().HasAnyNewBattlegroundsSkins();

  private void ShowBattlegroundsBonusErrorPopup()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_BACON_PERKS_ERROR_HEADER"),
      m_text = GameStrings.Get("GLUE_BACON_PERKS_ERROR_BODY"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  private bool ShouldShowLuckyDrawEndsSoonPopup()
  {
    int days = LuckyDrawUtils.GetLuckyDrawTimeRemaining(LuckyDrawManager.Get().GetActiveLuckyDrawBoxID()).Days;
    long num;
    GameSaveDataManager.Get().GetSubkeyValue(GameSaveKeyId.FTUE, GameSaveKeySubkeyId.LAST_BATTLE_BASH_END_SOON_BOX_ID_SEEN, out num);
    return days <= 3 && days >= 1 && num != (long) this.m_luckyDrawManager.GetActiveLuckyDrawBoxID();
  }

  private bool ShouldShowBattlegroundsUnacknowledgedEarnedHammersPopUp() => this.m_luckyDrawManager.NumUnacknowledgedEarnedHammers() > 0;

  private IEnumerator PlayLuckyDrawButtonFanfareFX()
  {
    yield return (object) new WaitForSeconds(0.5f);
    GameObject child = GameObjectUtils.FindChild(this.m_luckyDrawButton.gameObject, "NewHammerFX");
    if ((UnityEngine.Object) child == (UnityEngine.Object) null)
    {
      Log.All.PrintError("BaconDisplay.PlayLuckyDrawButtonFanfareFX - New Hammer FX object is null");
    }
    else
    {
      Animator component = child.GetComponent<Animator>();
      if ((UnityEngine.Object) component == (UnityEngine.Object) null)
        Log.All.PrintError("BaconDisplay.PlayLuckyDrawButtonFanfareFX - New Hammer FX object does not have animator component");
      else
        component.Play("LuckyDrawNewHammer_Anim");
    }
  }

  private void OnLuckyDrawUnacknowledgedHammerPopupDismissed()
  {
    this.m_luckyDrawManager.AcknowledgeAllHammers();
    this.StartCoroutine(this.PlayLuckyDrawButtonFanfareFX());
  }

  private void OnPartyChanged(
    PartyManager.PartyInviteEvent inviteEvent,
    BnetGameAccountId playerGameAccountId,
    PartyManager.PartyData data,
    object userData)
  {
    if (inviteEvent == PartyManager.PartyInviteEvent.I_CREATED_PARTY || inviteEvent == PartyManager.PartyInviteEvent.FRIEND_RECEIVED_INVITE)
      PartyManager.Get().SetReadyStatus(true);
    this.UpdatePlayButtonBasedOnPartyInfo();
  }

  private void OnPresenceUpdated(BnetPlayerChangelist changelist, object userData) => this.UpdatePlayButtonBasedOnPartyInfo();

  private void OnNearbyPlayersUpdated(
    BnetRecentOrNearbyPlayerChangelist changelist,
    object userData)
  {
    this.UpdatePlayButtonBasedOnPartyInfo();
  }

  private void UpdatePlayButtonBasedOnPartyInfo()
  {
    if ((UnityEngine.Object) this.m_playButton == (UnityEngine.Object) null)
      return;
    this.m_playButton.SetText(!PartyManager.Get().IsInBattlegroundsParty() || PartyManager.Get().IsPartyLeader() ? GameStrings.Get("GLOBAL_PLAY") : GameStrings.Get("GLOBAL_PLAY_WAITING"));
    int partyMemberCount = PartyManager.Get().GetReadyPartyMemberCount();
    int currentPartySize = PartyManager.Get().GetCurrentPartySize();
    string newText = "";
    if (PartyManager.Get().IsInBattlegroundsParty() && PartyManager.Get().IsPartyLeader() && !GameMgr.Get().IsFindingGame() && partyMemberCount < currentPartySize)
      newText = string.Format("{0}/{1}", (object) partyMemberCount, (object) currentPartySize);
    this.m_playButton.SetSecondaryText(newText);
    if (PartyManager.Get().IsInBattlegroundsParty() && (!PartyManager.Get().IsPartyLeader() || partyMemberCount < currentPartySize))
      this.m_playButton.Disable(true);
    else
      this.m_playButton.Enable();
  }

  private void OnStoreStatusChanged(bool isOpen) => this.GetBaconLobbyDataModel().ShopOpen = isOpen;

  private void ShowBattlegroundsStoreEmptyPopup()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_BACON_SHOP_EMPTY_HEADER"),
      m_text = GameStrings.Get("GLUE_BACON_SHOP_EMPTY_BODY"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  private bool HasBattlegroundsProductsAvailable()
  {
    if (StoreManager.Get().Catalog.CurrentTestDataMode == ProductCatalog.TestDataMode.TIER_TEST_DATA)
      return true;
    List<ShopType> shopTypeList = new List<ShopType>()
    {
      ShopType.BATTLEGROUNDS_STORE
    };
    if (!StoreManager.Get().CatalogNetworkPages.Contains((IEnumerable<ShopType>) shopTypeList))
      return false;
    foreach (Network.ShopSection section in StoreManager.Get().CatalogNetworkPages.Pages[ShopType.BATTLEGROUNDS_STORE].Sections)
    {
      foreach (Network.ShopSection.ProductRef product in section.Products)
      {
        Network.Bundle fromPmtProductId = StoreManager.Get().GetBundleFromPmtProductId(ProductId.CreateFrom(product.PmtId));
        if ((Record) fromPmtProductId != (Record) null && !StoreManager.Get().IsProductAlreadyOwned(fromPmtProductId))
          return true;
        ProductDataModel productByPmtId = StoreManager.Get().Catalog.GetProductByPmtId(ProductId.CreateFrom(product.PmtId));
        if (productByPmtId != null && productByPmtId.Availability == ProductAvailability.CAN_PURCHASE)
          return true;
      }
    }
    return false;
  }

  private bool HasNewBattlegroundsProducts()
  {
    List<ShopType> shopTypeList = new List<ShopType>()
    {
      ShopType.BATTLEGROUNDS_STORE
    };
    if (!StoreManager.Get().CatalogNetworkPages.Contains((IEnumerable<ShopType>) shopTypeList))
      return false;
    foreach (Network.ShopSection section in StoreManager.Get().CatalogNetworkPages.Pages[ShopType.BATTLEGROUNDS_STORE].Sections)
    {
      foreach (Network.ShopSection.ProductRef product in section.Products)
      {
        ProductDataModel productByPmtId = StoreManager.Get().Catalog.GetProductByPmtId(ProductId.CreateFrom(product.PmtId));
        if (productByPmtId != null && productByPmtId.Availability == ProductAvailability.CAN_PURCHASE && productByPmtId.Tags.Contains("new"))
          return true;
      }
    }
    return false;
  }

  protected override bool ShouldStartShown() => SceneMgr.Get().GetMode() != SceneMgr.Mode.LUCKY_DRAW && SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.LUCKY_DRAW;

  public override bool IsFinishedLoading(out string failureMessage)
  {
    if (!this.m_playButtonFinishedLoading)
    {
      failureMessage = "BaconDisplay - Play button never finished loading";
      return false;
    }
    if (!this.m_backButtonFinishedLoading)
    {
      failureMessage = "BaconDisplay - Back button never finished loading";
      return false;
    }
    if (!this.m_statsButtonFinishedLoading)
    {
      failureMessage = "BaconDisplay - Stats button never finished loading";
      return false;
    }
    if (!this.m_luckyDrawButtonFinishedLoading)
    {
      failureMessage = "BaconDisplay - Lucky draw button never finished loading";
      return false;
    }
    if ((bool) UniversalInputManager.UsePhoneUI)
    {
      if (!this.m_partyPhoneFinishedLoading)
      {
        failureMessage = "BaconDisplay - Lobby Phone not finished loading";
        return false;
      }
    }
    else if (!this.m_partyFinishedLoading)
    {
      failureMessage = "BaconDisplay - Lobby PC not finished loading";
      return false;
    }
    if (this.m_OwningWidget.IsChangingStates)
    {
      failureMessage = "BaconDisplay - owning widget is still transitioning";
      return false;
    }
    failureMessage = string.Empty;
    return true;
  }

  private void InitSlidingTray()
  {
    if ((UnityEngine.Object) this.m_slidingTray == (UnityEngine.Object) null)
      Error.AddDevWarning("UI Error", "Warning [BaconDisplay] InitSlidingTray() reference to the sliding tray is missing! This may lead to an improper layout.");
    else if (PlatformSettings.IsMobile() && (bool) UniversalInputManager.UsePhoneUI)
    {
      this.m_slidingTray.m_trayHiddenBone = this.m_OffScreenBoneMobile;
      this.m_slidingTray.m_trayShownBone = this.m_OnScreenBoneMobile;
    }
    else
    {
      this.m_slidingTray.m_trayHiddenBone = this.m_OffScreenBonePC;
      this.m_slidingTray.m_trayShownBone = this.m_OnScreenBonePC;
    }
  }
}
