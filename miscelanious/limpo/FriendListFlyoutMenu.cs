using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Jobs;
using Hearthstone.Core;
using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class FriendListFlyoutMenu : MonoBehaviour
{
  public const string EnabledEvent = "ENABLED";
  public const string DisabledEvent = "DISABLED";
  public const string ChallengeFriendEvent = "IS_FRIEND";
  public const string ChallengeStrangerEvent = "IS_STRANGER";
  public const string CloseMenuEvent = "CODE_FLYOUT_MENU_DISMISSED";
  public const string GrowPopupEvent = "GROW";
  public const string ShrinkPopupEvent = "SHRINK";
  public static readonly Vector3 PopupOffset = new Vector3(0.0f, 25f, 0.0f);
  private const float FullscreenFxTime = 0.25f;
  private Dictionary<FriendListFlyoutMenu.ButtonOption, FriendListFlyoutMenu.ButtonEvent> m_buttonEvents;
  private static readonly HashSet<FriendListFlyoutMenu.ButtonOption> m_sectionedButtons = new HashSet<FriendListFlyoutMenu.ButtonOption>()
  {
    FriendListFlyoutMenu.ButtonOption.AddFriend,
    FriendListFlyoutMenu.ButtonOption.Options,
    FriendListFlyoutMenu.ButtonOption.Report
  };
  [SerializeField]
  private MultiSliceElement m_frameContainer;
  [SerializeField]
  private MultiSliceElement m_shadowContainer;
  [SerializeField]
  private MultiSliceElement m_menuList;
  [SerializeField]
  private MultiSliceElement m_middleFrame;
  [SerializeField]
  private GameObject m_middleShadow;
  [SerializeField]
  private GameObject m_challengeTitle;
  [SerializeField]
  private GameObject m_sectionDivider;
  [SerializeField]
  private Widget m_hearthstoneChallengePopupWidget;
  [SerializeField]
  private Widget m_reportingPopupWidget;
  [SerializeField]
  private Widget m_optionsPopupWidget;
  [SerializeField]
  private GameObject m_menuButtons;
  [SerializeField]
  private Widget m_hearthstoneButton;
  [SerializeField]
  private Widget m_battlegroundsButton;
  [SerializeField]
  private Widget m_mercenariesButton;
  [SerializeField]
  private Widget m_spectateButton;
  [SerializeField]
  private Widget m_inviteToSpectateButton;
  [SerializeField]
  private Widget m_kickSpectatorButton;
  [SerializeField]
  private Widget m_stopSpectatingButton;
  [SerializeField]
  private Widget m_inviteToPartyButton;
  [SerializeField]
  private Widget m_kickFromPartyButton;
  [SerializeField]
  private Widget m_addFriendButton;
  [SerializeField]
  private Widget m_reportButton;
  [SerializeField]
  private Widget m_optionsButton;
  private Widget m_widget;
  private BnetPlayer m_player;
  private FriendListFriendFrame m_friendListFrame;
  private HearthstoneChallengePopup m_hearthstoneChallengePopup;
  private ReportingPopup m_reportingPopup;
  private FriendsListOptionsPopup m_optionsPopup;
  private List<IJobDependency> m_buttonDependencies;
  private ScreenEffectsHandle m_screenEffectsHandle;

  private void Awake()
  {
    this.m_widget = (Widget) this.gameObject.GetComponent<WidgetTemplate>();
    this.m_friendListFrame = this.GetComponentInParent<FriendListFriendFrame>();
    this.m_player = this.m_friendListFrame.GetFriend();
    this.InitializeButtonEvents();
    this.InitializeHearthstoneChallengePopup();
    this.InitializeReportingPopup();
    this.InitializeOptionsPopup();
    this.InitializeFlyoutMenu();
    this.m_screenEffectsHandle = new ScreenEffectsHandle((object) this);
  }

  private void OnEnable()
  {
    if (this.m_player == null)
      return;
    this.UpdateFlyoutMenu();
  }

  private void OnDestroy()
  {
    this.DismissPopups(false);
    if ((UnityEngine.Object) this.m_hearthstoneChallengePopupWidget != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_hearthstoneChallengePopupWidget.gameObject);
    if ((UnityEngine.Object) this.m_optionsPopupWidget != (UnityEngine.Object) null)
      UnityEngine.Object.Destroy((UnityEngine.Object) this.m_optionsPopupWidget.gameObject);
    if (!((UnityEngine.Object) this.m_reportingPopupWidget != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.m_reportingPopupWidget.gameObject);
  }

  private void InitializeButtonEvents() => this.m_buttonEvents = new Dictionary<FriendListFlyoutMenu.ButtonOption, FriendListFlyoutMenu.ButtonEvent>()
  {
    {
      FriendListFlyoutMenu.ButtonOption.Hearthstone,
      new FriendListFlyoutMenu.ButtonEvent("HEARTHSTONE", this.m_hearthstoneButton, new UIEvent.Handler(this.OnHearthstoneButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnHearthstoneButtonOver), new FriendListFlyoutMenu.ButtonOverride(this.OnHearthstoneButtonOverride))
    },
    {
      FriendListFlyoutMenu.ButtonOption.Battlegrounds,
      new FriendListFlyoutMenu.ButtonEvent("BATTLEGROUNDS", this.m_battlegroundsButton, new UIEvent.Handler(this.OnBattlegroundsButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnBattlegroundsButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.Mercenaries,
      new FriendListFlyoutMenu.ButtonEvent("MERCENARIES", this.m_mercenariesButton, new UIEvent.Handler(this.OnMercenariesButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnMercenariesButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.Spectate,
      new FriendListFlyoutMenu.ButtonEvent("SPECTATE", this.m_spectateButton, new UIEvent.Handler(this.OnSpectateButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnSpectateButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.InviteToSpectate,
      new FriendListFlyoutMenu.ButtonEvent("SPECTATE_INVITE", this.m_inviteToSpectateButton, new UIEvent.Handler(this.OnInviteToSpectateButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnInviteToSpectateButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.KickSpectator,
      new FriendListFlyoutMenu.ButtonEvent("SPECTATE_KICK", this.m_kickSpectatorButton, new UIEvent.Handler(this.OnKickSpectatorButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnKickSpectatorButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.StopSpectating,
      new FriendListFlyoutMenu.ButtonEvent("SPECTATE_STOP", this.m_stopSpectatingButton, new UIEvent.Handler(this.OnStopSpectatingButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnStopSpectatingButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.InviteToParty,
      new FriendListFlyoutMenu.ButtonEvent("PARTY_INVITE", this.m_inviteToPartyButton, new UIEvent.Handler(this.OnInviteToPartyButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnInviteToPartyButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.KickFromParty,
      new FriendListFlyoutMenu.ButtonEvent("PARTY_KICK", this.m_kickFromPartyButton, new UIEvent.Handler(this.OnKickFromPartyButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnKickFromPartyButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.AddFriend,
      new FriendListFlyoutMenu.ButtonEvent("ADD_FRIEND", this.m_addFriendButton, new UIEvent.Handler(this.OnAddFriendButtonReleased))
    },
    {
      FriendListFlyoutMenu.ButtonOption.Options,
      new FriendListFlyoutMenu.ButtonEvent("OPTIONS", this.m_optionsButton, new UIEvent.Handler(this.OnOptionsButtonReleased))
    },
    {
      FriendListFlyoutMenu.ButtonOption.Report,
      new FriendListFlyoutMenu.ButtonEvent("REPORT", this.m_reportButton, new UIEvent.Handler(this.OnReportButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnReportButtonOver))
    }
  };

  private void InitializeButton(FriendListFlyoutMenu.ButtonOption buttonOption)
  {
    FriendListFlyoutMenu.ButtonEvent buttonEvent;
    if (!this.m_buttonEvents.TryGetValue(buttonOption, out buttonEvent) || !((UnityEngine.Object) buttonEvent.buttonWidget != (UnityEngine.Object) null))
      return;
    Widget buttonWidget = buttonEvent.buttonWidget;
    buttonWidget.RegisterReadyListener((System.Action<object>) (_ =>
    {
      buttonEvent.buttonWidget.TriggerEvent(buttonEvent.eventName);
      FriendListFlyoutMenu.ButtonOverride onOverride = buttonEvent.onOverride;
      if (onOverride != null)
        onOverride(buttonWidget);
      UIBButton uibButton = buttonWidget.GetComponentInChildren<UIBButton>(true);
      if (buttonEvent.onRelease != null)
        uibButton.AddEventListener(UIEventType.RELEASE, closure_0 ?? (closure_0 = (UIEvent.Handler) (e =>
        {
          if (!buttonEvent.isEnabled)
            return;
          buttonEvent.onRelease(e);
        })));
      if (buttonEvent.onHover == null)
        return;
      TooltipZone tooltipZone = buttonWidget.GetComponentInChildren<TooltipZone>(true);
      uibButton.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (eventType => buttonEvent.onHover(uibButton, tooltipZone)));
      uibButton.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (eventType => tooltipZone.HideTooltip()));
      this.m_menuList.m_ignore.Add(tooltipZone.gameObject);
      this.m_frameContainer.m_ignore.Add(tooltipZone.gameObject);
      this.m_shadowContainer.m_ignore.Add(tooltipZone.gameObject);
      this.m_middleFrame.m_ignore.Add(tooltipZone.gameObject);
    }), (object) null, true);
  }

  private void HideAllButtons()
  {
    foreach (KeyValuePair<FriendListFlyoutMenu.ButtonOption, FriendListFlyoutMenu.ButtonEvent> buttonEvent1 in this.m_buttonEvents)
    {
      FriendListFlyoutMenu.ButtonEvent buttonEvent2 = buttonEvent1.Value;
      if ((UnityEngine.Object) buttonEvent2.buttonWidget != (UnityEngine.Object) null)
        buttonEvent2.buttonWidget.gameObject.SetActive(false);
    }
  }

  private void InitializeFlyoutMenu()
  {
    this.m_buttonDependencies = new List<IJobDependency>();
    foreach (FriendListFlyoutMenu.ButtonOption buttonOption in Enum.GetValues(typeof (FriendListFlyoutMenu.ButtonOption)))
      this.InitializeButton(buttonOption);
  }

  public void UpdateFlyoutMenu()
  {
    this.HideAllButtons();
    this.m_buttonDependencies = new List<IJobDependency>();
    List<GameObject> topSectionButtons = new List<GameObject>();
    List<GameObject> bottomSectionButtons = new List<GameObject>();
    bool showChallengeHeader = false;
    foreach (FriendListFlyoutMenu.ButtonOption buttonOption in Enum.GetValues(typeof (FriendListFlyoutMenu.ButtonOption)))
    {
      FriendListFlyoutMenu.ButtonEvent buttonEvent;
      if (this.ShouldShowOption(buttonOption) && this.m_buttonEvents.TryGetValue(buttonOption, out buttonEvent) && (UnityEngine.Object) buttonEvent.buttonWidget != (UnityEngine.Object) null)
      {
        Widget buttonWidget = buttonEvent.buttonWidget;
        buttonWidget.gameObject.SetActive(true);
        this.m_buttonDependencies.Add((IJobDependency) new WaitForWidget(buttonWidget));
        if (this.ShouldEnableOption(buttonOption))
        {
          buttonWidget.TriggerEvent("ENABLED");
          buttonEvent.isEnabled = true;
        }
        else
        {
          buttonWidget.TriggerEvent("DISABLED");
          buttonEvent.isEnabled = false;
        }
        if (FriendListFlyoutMenu.m_sectionedButtons.Contains(buttonOption))
          bottomSectionButtons.Add(buttonWidget.gameObject);
        else
          topSectionButtons.Add(buttonWidget.gameObject);
        if (buttonOption == FriendListFlyoutMenu.ButtonOption.Hearthstone || buttonOption == FriendListFlyoutMenu.ButtonOption.Battlegrounds || buttonOption == FriendListFlyoutMenu.ButtonOption.Mercenaries)
          showChallengeHeader = true;
      }
    }
    Processor.QueueJob("FriendListChallengeMenu.FormatFlyoutMenu", this.Job_FormatFlyoutMenu(topSectionButtons, bottomSectionButtons, showChallengeHeader), this.m_buttonDependencies.ToArray());
  }

  private void InitializeHearthstoneChallengePopup()
  {
    OverlayUI.Get().AddGameObject(this.m_hearthstoneChallengePopupWidget.gameObject);
    this.m_hearthstoneChallengePopupWidget.transform.position += FriendListFlyoutMenu.PopupOffset;
    this.m_hearthstoneChallengePopupWidget.RegisterReadyListener((System.Action<object>) (_ =>
    {
      this.m_hearthstoneChallengePopup = this.m_hearthstoneChallengePopupWidget.GetComponentInChildren<HearthstoneChallengePopup>(true);
      this.m_hearthstoneChallengePopup.Init(this.m_player, this.m_friendListFrame, this);
      this.m_hearthstoneChallengePopupWidget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
      {
        if (!(eventName == "DISMISS_POPUP"))
          return;
        this.HidePopup(this.m_hearthstoneChallengePopupWidget);
      }));
    }), (object) null, true);
  }

  private void InitializeReportingPopup()
  {
    OverlayUI.Get().AddGameObject(this.m_reportingPopupWidget.gameObject);
    this.m_reportingPopupWidget.transform.position += FriendListFlyoutMenu.PopupOffset;
    Vector3 popupPosition = this.m_reportingPopupWidget.transform.position;
    this.m_reportingPopupWidget.transform.position = Vector3.zero;
    this.m_reportingPopupWidget.RegisterReadyListener((System.Action<object>) (_ =>
    {
      this.m_reportingPopupWidget.transform.position = popupPosition;
      this.m_reportingPopup = this.m_reportingPopupWidget.GetComponentInChildren<ReportingPopup>(true);
      this.m_reportingPopup.Init(this.m_player);
    }), (object) null, true);
    this.m_reportingPopupWidget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (!(eventName == "DISMISS_POPUP"))
        return;
      this.HidePopup(this.m_reportingPopupWidget);
    }));
  }

  private void InitializeOptionsPopup()
  {
    OverlayUI.Get().AddGameObject(this.m_optionsPopupWidget.gameObject);
    this.m_optionsPopupWidget.transform.position += FriendListFlyoutMenu.PopupOffset;
    this.m_optionsPopupWidget.RegisterReadyListener((System.Action<object>) (_ =>
    {
      this.m_optionsPopup = this.m_optionsPopupWidget.GetComponentInChildren<FriendsListOptionsPopup>(true);
      this.m_optionsPopup.Init(this.m_player, this);
    }), (object) null, true);
    this.m_optionsPopupWidget.RegisterEventListener((Widget.EventListenerDelegate) (eventName =>
    {
      if (!(eventName == "DISMISS_POPUP"))
        return;
      this.HidePopup(this.m_optionsPopupWidget);
    }));
  }

  private void ShowPopup(Widget popupWidget)
  {
    if (!popupWidget.gameObject.activeInHierarchy)
      this.m_screenEffectsHandle.StartEffect(ScreenEffectParameters.BlurVignetteDesaturatePerspective with
      {
        Time = 0.25f
      });
    popupWidget.gameObject.SetActive(true);
    popupWidget.TriggerEvent("GROW");
  }

  private void HidePopup(Widget popupWidget)
  {
    if (!((UnityEngine.Object) popupWidget != (UnityEngine.Object) null))
      return;
    if (popupWidget.gameObject.activeInHierarchy)
      this.m_screenEffectsHandle.StopEffect((System.Action) (() =>
      {
        if (!((UnityEngine.Object) popupWidget != (UnityEngine.Object) null))
          return;
        popupWidget.gameObject.SetActive(false);
      }));
    popupWidget.TriggerEvent("SHRINK");
  }

  private bool ShouldShowOption(FriendListFlyoutMenu.ButtonOption option)
  {
    bool flag1 = BnetFriendMgr.Get().IsFriend(this.m_player);
    if (option == FriendListFlyoutMenu.ButtonOption.AddFriend)
      return !flag1;
    if (option == FriendListFlyoutMenu.ButtonOption.Options)
      return flag1;
    if (option == FriendListFlyoutMenu.ButtonOption.Report)
      return !flag1;
    BnetGameAccountId hearthstoneGameAccountId = this.m_player.GetHearthstoneGameAccountId();
    if (option == FriendListFlyoutMenu.ButtonOption.Spectate)
      return SpectatorManager.Get().CanSpectate(this.m_player);
    if (option == FriendListFlyoutMenu.ButtonOption.StopSpectating)
      return SpectatorManager.Get().IsSpectatingPlayer(hearthstoneGameAccountId);
    if (option == FriendListFlyoutMenu.ButtonOption.InviteToSpectate)
      return SpectatorManager.Get().CanInviteToSpectateMyGame(hearthstoneGameAccountId) || SpectatorManager.Get().IsInvitedToSpectateMyGame(hearthstoneGameAccountId);
    if (option == FriendListFlyoutMenu.ButtonOption.KickSpectator)
      return SpectatorManager.Get().IsSpectatingMe(hearthstoneGameAccountId);
    bool flag2 = PartyManager.Get().IsInBattlegroundsParty() && !SceneMgr.Get().IsInGame() && !GameMgr.Get().IsFindingGame();
    switch (option)
    {
      case FriendListFlyoutMenu.ButtonOption.InviteToParty:
        return flag2 && PartyManager.Get().CanInvite(hearthstoneGameAccountId);
      case FriendListFlyoutMenu.ButtonOption.KickFromParty:
        return flag2 && PartyManager.Get().CanKick(hearthstoneGameAccountId);
      default:
        bool flag3 = FriendChallengeMgr.Get().CanShowFriendlyChallenge(this.m_player);
        return (option == FriendListFlyoutMenu.ButtonOption.Hearthstone || option == FriendListFlyoutMenu.ButtonOption.Battlegrounds || option == FriendListFlyoutMenu.ButtonOption.Mercenaries) && flag3;
    }
  }

  private bool ShouldEnableOption(FriendListFlyoutMenu.ButtonOption option)
  {
    switch (option)
    {
      case FriendListFlyoutMenu.ButtonOption.Hearthstone:
        if (this.ShouldSeeHearthstoneChallengePopup())
          return FriendChallengeMgr.Get().IsHearthstoneFriendlyChallengeAvailable(this.m_player);
        return FriendChallengeMgr.Get().IsHearthstoneFriendlyChallengeAvailable(this.m_player) && CollectionManager.Get().AccountHasValidDeck(FormatType.FT_STANDARD);
      case FriendListFlyoutMenu.ButtonOption.Battlegrounds:
        return FriendChallengeMgr.Get().IsBattlegroundsFriendlyChallengeAvailable(this.m_player);
      case FriendListFlyoutMenu.ButtonOption.Mercenaries:
        return FriendChallengeMgr.Get().IsMercenariesFriendlyChallengeAvailable(this.m_player);
      case FriendListFlyoutMenu.ButtonOption.Spectate:
      case FriendListFlyoutMenu.ButtonOption.InviteToSpectate:
      case FriendListFlyoutMenu.ButtonOption.KickSpectator:
      case FriendListFlyoutMenu.ButtonOption.StopSpectating:
      case FriendListFlyoutMenu.ButtonOption.Options:
        return true;
      case FriendListFlyoutMenu.ButtonOption.InviteToParty:
        return FriendChallengeMgr.Get().IsBattlegroundsFriendlyChallengeAvailable(this.m_player) && PartyManager.Get().GetCurrentPartySize() < PartyManager.Get().GetMaxPartySizeByPartyType(PartyType.BATTLEGROUNDS_PARTY) && !PartyManager.Get().IsPlayerPendingInCurrentParty(this.m_player.GetBestGameAccountId());
      case FriendListFlyoutMenu.ButtonOption.KickFromParty:
        return true;
      case FriendListFlyoutMenu.ButtonOption.AddFriend:
        return !BnetRecentPlayerMgr.Get().IsCurrentOpponent(this.m_player);
      case FriendListFlyoutMenu.ButtonOption.Report:
        NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
        return netObject != null && netObject.ReportPlayerEnabled;
      default:
        return false;
    }
  }

  private IEnumerator<IAsyncJobResult> Job_FormatFlyoutMenu(
    List<GameObject> topSectionButtons,
    List<GameObject> bottomSectionButtons,
    bool showChallengeHeader)
  {
    this.m_menuList.ClearSlices();
    if (showChallengeHeader)
    {
      this.m_challengeTitle.SetActive(true);
      this.m_menuList.AddSlice(this.m_challengeTitle);
      if (BnetFriendMgr.Get().IsFriend(this.m_player))
        this.m_widget.TriggerEvent("IS_FRIEND");
      else
        this.m_widget.TriggerEvent("IS_STRANGER");
    }
    else
      this.m_challengeTitle.SetActive(false);
    foreach (GameObject topSectionButton in topSectionButtons)
    {
      GameUtils.SetParent(topSectionButton, this.m_menuButtons);
      this.m_menuList.AddSlice(topSectionButton);
    }
    if (topSectionButtons.Count > 0 && bottomSectionButtons.Count > 0)
    {
      this.m_sectionDivider.SetActive(true);
      this.m_menuList.AddSlice(this.m_sectionDivider);
    }
    else
      this.m_sectionDivider.SetActive(false);
    foreach (GameObject bottomSectionButton in bottomSectionButtons)
    {
      GameUtils.SetParent(bottomSectionButton, this.m_menuButtons);
      this.m_menuList.AddSlice(bottomSectionButton);
    }
    this.m_menuList.UpdateSlices();
    float dimension = TransformUtil.ComputeOrientedWorldBounds(this.m_menuList.gameObject, true, true).Extents[1].magnitude * 2f;
    TransformUtil.SetLocalScaleToWorldDimension(this.m_middleFrame.gameObject, new WorldDimensionIndex(dimension, 1));
    TransformUtil.SetLocalScaleToWorldDimension(this.m_middleShadow, new WorldDimensionIndex(dimension, 1));
    this.m_frameContainer.UpdateSlices();
    this.m_shadowContainer.UpdateSlices();
    this.m_middleFrame.UpdateSlices();
    yield return (IAsyncJobResult) null;
  }

  public static void ShowTooltip(
    BnetPlayer player,
    string headerKey,
    string descriptionFormat,
    TooltipZone tooltipZone,
    UIBButton button)
  {
    if (UniversalInputManager.Get().IsTouchMode())
    {
      if (GameStrings.HasKey(headerKey + "_TOUCH"))
        headerKey += "_TOUCH";
      if (GameStrings.HasKey(descriptionFormat + "_TOUCH"))
        descriptionFormat += "_TOUCH";
    }
    string headline = GameStrings.Get(headerKey);
    string bodytext = GameStrings.Format(descriptionFormat, (object) player.GetBestName());
    tooltipZone.ShowSocialTooltip((Component) button, headline, bodytext, 18.75f, GameLayer.BattleNetDialog);
    tooltipZone.AnchorTooltipTo(button.gameObject, Anchor.TOP_RIGHT_XZ, Anchor.TOP_LEFT_XZ);
  }

  public static bool GetAvailability(BnetPlayer player, out string reason)
  {
    if (!FriendChallengeMgr.Get().AmIAvailable())
    {
      reason = !BnetPresenceMgr.Get().GetMyPlayer().IsAppearingOffline() ? (!PartyManager.Get().IsInBattlegroundsParty() || PartyManager.Get().IsPartyLeader() ? "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_IM_UNAVAILABLE" : "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_BATTLEGROUNDS_PARTY_MEMBER") : "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_IM_APPEARING_OFFLINE";
      return false;
    }
    if (!FriendChallengeMgr.Get().IsOpponentAvailable(player))
    {
      reason = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_THEYRE_UNAVAILABLE";
      return false;
    }
    reason = string.Empty;
    return true;
  }

  public void ShowReportingPopup()
  {
    this.ShowPopup(this.m_reportingPopupWidget);
    if ((UnityEngine.Object) this.m_reportingPopup != (UnityEngine.Object) null)
      this.m_reportingPopup.Init(this.m_player);
    this.m_friendListFrame.CloseChallengeMenu();
  }

  public void DismissPopups(bool showAlert)
  {
    if ((UnityEngine.Object) this.m_hearthstoneChallengePopupWidget != (UnityEngine.Object) null)
    {
      if (showAlert && this.m_hearthstoneChallengePopupWidget.gameObject.activeInHierarchy)
        this.ShowPlayerOfflineAlert();
      this.HidePopup(this.m_hearthstoneChallengePopupWidget);
      this.m_hearthstoneChallengePopupWidget.gameObject.SetActive(false);
    }
    if ((UnityEngine.Object) this.m_optionsPopupWidget != (UnityEngine.Object) null)
    {
      this.HidePopup(this.m_optionsPopupWidget);
      this.m_optionsPopupWidget.gameObject.SetActive(false);
    }
    if (!((UnityEngine.Object) this.m_reportingPopupWidget != (UnityEngine.Object) null))
      return;
    this.HidePopup(this.m_reportingPopupWidget);
    this.m_reportingPopupWidget.gameObject.SetActive(false);
  }

  public void SendHearthstoneFriendlyChallenge()
  {
    if (!CollectionManager.Get().AccountHasValidDeck(FormatType.FT_STANDARD))
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
        m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_STANDARD_DECK"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
      DialogManager.Get().ShowPopup(info);
      this.m_friendListFrame.CloseChallengeMenu();
    }
    else if (!FriendChallengeMgr.Get().IsOpponentAvailable(this.m_player))
    {
      this.ShowOpponentUnavailableAlert();
      this.m_friendListFrame.CloseFriendsListMenu();
    }
    else
    {
      FriendChallengeMgr.Get().SetChallengeMethod(FriendChallengeMgr.ChallengeMethod.FROM_FRIEND_LIST);
      FriendChallengeMgr.Get().SendChallenge(this.m_player, FormatType.FT_STANDARD, true);
      this.m_friendListFrame.CloseFriendsListMenu();
    }
  }

  public void ShowOpponentUnavailableAlert()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
      m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_THEYRE_UNAVAILABLE", (object) this.m_player.GetBestName()),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  private void NavigateToSceneForPartyChallenge(SceneMgr.Mode nextMode)
  {
    GameMgr.Get().SetPendingAutoConcede(true);
    if (CollectionManager.Get().IsInEditMode())
      CollectionManager.Get().GetEditedDeck()?.SendChanges(CollectionDeck.ChangeSource.NavigateToSceneForPartyChallenge);
    SceneMgr.Get().SetNextMode(nextMode);
  }

  private void ShowPlayerOfflineAlert()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_DEFAULT_ALERT_HEADER"),
      m_text = GameStrings.Format("GLOBAL_SOCIAL_ALERT_FRIEND_OFFLINE", (object) this.m_player.GetBattleTag()),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  private bool ShouldSeeHearthstoneChallengePopup() => CollectionManager.Get().ShouldAccountSeeStandardWild() || TavernBrawlManager.Get().HasUnlockedTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);

  private void ShowBattlegroundsPrivatePartyDialog()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLUE_BACON_PRIVATE_PARTY_TITLE"),
      m_text = GameStrings.Format("GLUE_BACON_PRIVATE_PARTY_WARNING", (object) PartyManager.Get().GetBattlegroundsMaxRankedPartySize()),
      m_iconSet = AlertPopup.PopupInfo.IconSet.Default,
      m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
      m_confirmText = GameStrings.Get("GLUE_COLLECTION_DECK_COMPLETE_POPUP_CONFIRM"),
      m_cancelText = GameStrings.Get("GLUE_COLLECTION_DECK_COMPLETE_POPUP_CANCEL"),
      m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
      {
        if (response != AlertPopup.Response.CONFIRM)
          return;
        PartyManager.Get().SendInvite(PartyType.BATTLEGROUNDS_PARTY, this.m_player.GetBestGameAccountId());
      })
    };
    DialogManager.Get().ShowPopup(info);
  }

  private void OnHearthstoneButtonReleased(UIEvent e)
  {
    if (this.ShouldSeeHearthstoneChallengePopup())
    {
      this.ShowPopup(this.m_hearthstoneChallengePopupWidget);
      if ((UnityEngine.Object) this.m_hearthstoneChallengePopup != (UnityEngine.Object) null)
        this.m_hearthstoneChallengePopup.Init(this.m_player, this.m_friendListFrame, this);
      this.m_friendListFrame.CloseChallengeMenu();
    }
    else
      this.SendHearthstoneFriendlyChallenge();
  }

  private void OnBattlegroundsButtonReleased(UIEvent e)
  {
    this.NavigateToSceneForPartyChallenge(SceneMgr.Mode.BACON);
    PartyManager.Get().SendInvite(PartyType.BATTLEGROUNDS_PARTY, this.m_player.GetBestGameAccountId());
    this.m_friendListFrame.CloseChallengeMenu();
  }

  private void OnMercenariesButtonReleased(UIEvent e)
  {
    PartyManager.Get().StartMercenariesFriendlyChallengeEntry(this.m_player);
    this.m_friendListFrame.CloseChallengeMenu();
    this.m_friendListFrame.CloseFriendsListMenu();
  }

  private void OnSpectateButtonReleased(UIEvent e)
  {
    SpectatorManager.Get().SpectatePlayer(this.m_player);
    this.m_friendListFrame.CloseChallengeMenu();
  }

  private void OnInviteToSpectateButtonReleased(UIEvent e) => SpectatorManager.Get().InviteToSpectateMe(this.m_player);

  private void OnKickSpectatorButtonReleased(UIEvent e)
  {
    BnetGameAccountId hearthstoneGameAccountId = this.m_player.GetHearthstoneGameAccountId();
    if (!SpectatorManager.Get().IsSpectatingMe(hearthstoneGameAccountId))
      return;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLOBAL_SPECTATOR_KICK_PROMPT_HEADER");
    info.m_text = GameStrings.Format("GLOBAL_SPECTATOR_KICK_PROMPT_TEXT", (object) FriendUtils.GetUniqueName(this.m_player));
    info.m_showAlertIcon = true;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
    info.m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
    {
      BnetPlayer player = (BnetPlayer) userData;
      if (response != AlertPopup.Response.CONFIRM)
        return;
      SpectatorManager.Get().KickSpectator(player, true);
    });
    info.m_responseUserData = (object) this.m_player;
    DialogManager.DialogProcessCallback callback = (DialogManager.DialogProcessCallback) ((dialog, userData) =>
    {
      BnetGameAccountId gameAccountId = (BnetGameAccountId) userData;
      return SpectatorManager.Get().IsSpectatingMe(gameAccountId);
    });
    DialogManager.Get().ShowPopup(info, callback, (object) hearthstoneGameAccountId);
  }

  private void OnStopSpectatingButtonReleased(UIEvent e)
  {
    BnetGameAccountId gameAccountId = this.m_player.GetHearthstoneGameAccountId();
    SpectatorManager spectator = SpectatorManager.Get();
    if (GameMgr.Get().IsFindingGame() || SceneMgr.Get().IsTransitioning() || GameMgr.Get().IsTransitionPopupShown())
      return;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
    info.m_headerText = GameStrings.Get("GLOBAL_SPECTATOR_LEAVE_PROMPT_HEADER");
    info.m_text = GameStrings.Get("GLOBAL_SPECTATOR_LEAVE_PROMPT_TEXT");
    info.m_showAlertIcon = true;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
    info.m_responseCallback = (AlertPopup.ResponseCallback) ((response, userData) =>
    {
      if (response != AlertPopup.Response.CONFIRM)
        return;
      SpectatorManager.Get().LeaveSpectatorMode();
    });
    DialogManager.DialogProcessCallback callback = (DialogManager.DialogProcessCallback) ((dialog, userData) => spectator.IsSpectatingPlayer(gameAccountId));
    DialogManager.Get().ShowPopup(info, callback);
  }

  private void OnInviteToPartyButtonReleased(UIEvent e)
  {
    if (PartyManager.Get().IsPartyLeader())
    {
      if (PartyManager.Get().GetCurrentPartySize() == PartyManager.Get().GetBattlegroundsMaxRankedPartySize())
        this.ShowBattlegroundsPrivatePartyDialog();
      else
        PartyManager.Get().SendInvite(PartyType.BATTLEGROUNDS_PARTY, this.m_player.GetBestGameAccountId());
    }
    else
      PartyManager.Get().SendInviteSuggestion(PartyType.BATTLEGROUNDS_PARTY, this.m_player.GetBestGameAccountId());
  }

  private void OnKickFromPartyButtonReleased(UIEvent e) => PartyManager.Get().KickPlayerFromParty(this.m_player.GetBestGameAccountId());

  private void OnAddFriendButtonReleased(UIEvent e)
  {
    BnetFriendMgr.Get().SendInvite(this.m_player.GetBattleTag().GetString());
    this.m_friendListFrame.CloseChallengeMenu();
  }

  private void OnReportButtonReleased(UIEvent e) => this.ShowReportingPopup();

  private void OnOptionsButtonReleased(UIEvent e)
  {
    this.ShowPopup(this.m_optionsPopupWidget);
    if ((UnityEngine.Object) this.m_optionsPopup != (UnityEngine.Object) null)
      this.m_optionsPopup.Init(this.m_player, this);
    this.m_friendListFrame.CloseChallengeMenu();
  }

  private void OnHearthstoneButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    string headerKey = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
    string descriptionFormat;
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.Friendly)
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_MODE_UNAVAILABLE";
    else if (!GameUtils.IsTraditionalTutorialComplete())
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_TRADITIONAL_LOCKED";
    else if (this.m_player.GetHearthstoneGameAccount().GetTutorialBeaten() < 1)
    {
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_HEARTHSTONE_TUTORIAL_COMPLETE";
    }
    else
    {
      string reason;
      if (!FriendListFlyoutMenu.GetAvailability(this.m_player, out reason))
      {
        descriptionFormat = reason;
      }
      else
      {
        if (this.ShouldSeeHearthstoneChallengePopup() || CollectionManager.Get().AccountHasValidDeck(FormatType.FT_STANDARD))
          return;
        descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_STANDARD_DECK";
      }
    }
    FriendListFlyoutMenu.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
  }

  private void OnBattlegroundsButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    string headerKey = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
    string descriptionFormat;
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.BattlegroundsFriendlyChallenge)
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_MODE_UNAVAILABLE";
    else if (!GameUtils.IsBattleGroundsTutorialComplete())
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_BATTLEGROUNDS_LOCKED";
    else if (!this.m_player.GetHearthstoneGameAccount().GetBattlegroundsTutorialComplete())
    {
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_BATTLEGROUNDS_TUTORIAL_COMPLETE";
    }
    else
    {
      string reason;
      if (FriendListFlyoutMenu.GetAvailability(this.m_player, out reason))
        return;
      descriptionFormat = reason;
    }
    FriendListFlyoutMenu.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
  }

  private void OnMercenariesButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    string headerKey = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
    string descriptionFormat;
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.BattlegroundsFriendlyChallenge)
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_MODE_UNAVAILABLE";
    else if (!GameUtils.IsMercenariesVillageTutorialComplete())
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_MERCS_LOCKED";
    else if (!this.m_player.GetHearthstoneGameAccount().GetMercenariesTutorialComplete())
    {
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_MERCS_TUTORIAL_COMPLETE";
    }
    else
    {
      string reason;
      if (FriendListFlyoutMenu.GetAvailability(this.m_player, out reason))
        return;
      descriptionFormat = reason;
    }
    FriendListFlyoutMenu.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
  }

  private void OnInviteToPartyButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    string headerKey = "GLOBAL_FRIENDLIST_BATTLEGROUNDS_TOOLTIP_INVITE_HEADER";
    string descriptionFormat = "GLOBAL_FRIENDLIST_BATTLEGROUNDS_TOOLTIP_INVITE_BODY";
    if (!this.m_player.GetHearthstoneGameAccount().GetBattlegroundsTutorialComplete())
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_BATTLEGROUNDS_TUTORIAL_COMPLETE";
    else if (!FriendChallengeMgr.Get().IsOpponentAvailable(this.m_player))
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_USER_IS_BUSY";
    else if (PartyManager.Get().GetCurrentPartySize() >= PartyManager.Get().GetMaxPartySizeByPartyType(PartyType.BATTLEGROUNDS_PARTY))
    {
      headerKey = "GLOBAL_FRIENDLIST_BATTLEGROUNDS_TOOLTIP_INVITE_FULL_PARTY_HEADER";
      descriptionFormat = "GLOBAL_FRIENDLIST_BATTLEGROUNDS_TOOLTIP_INVITE_FULL_PARTY_BODY";
    }
    else if (PartyManager.Get().IsPlayerPendingInCurrentParty(this.m_player.GetBestGameAccountId()))
    {
      headerKey = "GLOBAL_FRIENDLIST_BATTLEGROUNDS_TOOLTIP_INVITE_ALREADY_SENT_HEADER";
      descriptionFormat = "GLOBAL_FRIENDLIST_BATTLEGROUNDS_TOOLTIP_INVITE_ALREADY_SENT_BODY";
    }
    FriendListFlyoutMenu.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
  }

  private void OnKickFromPartyButtonOver(UIBButton button, TooltipZone tooltipZone) => FriendListFlyoutMenu.ShowTooltip(this.m_player, "GLOBAL_FRIENDLIST_BATTLEGROUNDS_TOOLTIP_KICK_HEADER", "GLOBAL_FRIENDLIST_BATTLEGROUNDS_TOOLTIP_KICK_BODY", tooltipZone, button);

  private void OnSpectateButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    BnetGameAccountId bestGameAccountId = this.m_player.GetBestGameAccountId();
    if (SpectatorManager.Get().HasPreviouslyKickedMeFromGame(bestGameAccountId, SpectatorManager.GetSpectatorGameHandleFromPlayer(this.m_player)))
    {
      FriendListFlyoutMenu.ShowTooltip(this.m_player, "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_PREVIOUSLY_KICKED_HEADER", "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_PREVIOUSLY_KICKED_TEXT", tooltipZone, button);
    }
    else
    {
      if (!SpectatorManager.Get().HasInvitedMeToSpectate(bestGameAccountId))
        return;
      FriendListFlyoutMenu.ShowTooltip(this.m_player, "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_AVAILABLE_HEADER", "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_RECEIVED_INVITE_TEXT", tooltipZone, button);
    }
  }

  private void OnInviteToSpectateButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    BnetGameAccountId bestGameAccountId = this.m_player.GetBestGameAccountId();
    string headerKey;
    string descriptionFormat;
    if (SpectatorManager.Get().IsInvitedToSpectateMyGame(bestGameAccountId))
    {
      headerKey = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITED_HEADER";
      descriptionFormat = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITED_TEXT";
    }
    else if (SpectatorManager.Get().IsPlayerSpectatingMyGamesOpposingSide(bestGameAccountId))
    {
      headerKey = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITE_OTHER_SIDE_HEADER";
      descriptionFormat = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITE_OTHER_SIDE_TEXT";
    }
    else
    {
      headerKey = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITE_HEADER";
      descriptionFormat = "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_INVITE_TEXT";
    }
    FriendListFlyoutMenu.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
  }

  private void OnKickSpectatorButtonOver(UIBButton button, TooltipZone tooltipZone) => FriendListFlyoutMenu.ShowTooltip(this.m_player, "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_KICK_HEADER", "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_KICK_TEXT", tooltipZone, button);

  private void OnStopSpectatingButtonOver(UIBButton button, TooltipZone tooltipZone) => FriendListFlyoutMenu.ShowTooltip(this.m_player, "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_SPECTATING_HEADER", "GLOBAL_FRIENDLIST_SPECTATE_TOOLTIP_SPECTATING_TEXT", tooltipZone, button);

  private void OnReportButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    string headerKey = "GLOBAL_FRIENDLIST_REPORT_TOOLTIP_HEADER";
    string descriptionFormat = "GLOBAL_FRIENDLIST_REPORT_TOOLTIP_CURRENTLY_UNAVAILABLE_TEXT";
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject == null || netObject.ReportPlayerEnabled)
      return;
    FriendListFlyoutMenu.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
  }

  private void OnHearthstoneButtonOverride(Widget buttonWidget)
  {
    if (this.ShouldSeeHearthstoneChallengePopup())
      buttonWidget.TriggerEvent("HEARTHSTONE_EXTENDED_MENU");
    else
      buttonWidget.TriggerEvent("HEARTHSTONE");
  }

  public delegate void ShowTooltipEvent(UIBButton button, TooltipZone tooltipZone);

  public delegate void ButtonOverride(Widget widget);

  public class ButtonEvent
  {
    public bool isEnabled;

    public string eventName { get; }

    public Widget buttonWidget { get; }

    public UIEvent.Handler onRelease { get; }

    public FriendListFlyoutMenu.ShowTooltipEvent onHover { get; }

    public FriendListFlyoutMenu.ButtonOverride onOverride { get; }

    public ButtonEvent(
      string eventName,
      Widget buttonWidget,
      UIEvent.Handler onRelease,
      FriendListFlyoutMenu.ShowTooltipEvent onHover = null,
      FriendListFlyoutMenu.ButtonOverride onOverride = null)
    {
      this.eventName = eventName;
      this.buttonWidget = buttonWidget;
      this.onRelease = onRelease;
      this.onHover = onHover;
      this.onOverride = onOverride;
    }
  }

  public enum ButtonOption
  {
    Invalid = -1, // 0xFFFFFFFF
    Hearthstone = 0,
    Battlegrounds = 1,
    Mercenaries = 2,
    Spectate = 3,
    InviteToSpectate = 4,
    KickSpectator = 5,
    StopSpectating = 6,
    InviteToParty = 7,
    KickFromParty = 8,
    AddFriend = 9,
    Report = 10, // 0x0000000A
    Options = 11, // 0x0000000B
    StandardHearthstone = 12, // 0x0000000C
    WildHearthstone = 13, // 0x0000000D
    ClassicHearthstone = 14, // 0x0000000E
    TavernBrawl = 15, // 0x0000000F
  }
}
