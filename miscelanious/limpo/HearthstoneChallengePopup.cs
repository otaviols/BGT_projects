using Hearthstone.UI;
using PegasusShared;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class HearthstoneChallengePopup : MonoBehaviour
{
  [SerializeField]
  private UberText m_challengeFriendNameText;
  [SerializeField]
  private WidgetInstance m_standardButton;
  [SerializeField]
  private WidgetInstance m_wildButton;
  [SerializeField]
  private WidgetInstance m_classicButton;
  [SerializeField]
  private WidgetInstance m_tavernBrawlButton;
  public const string EnabledEvent = "ENABLED";
  public const string DisabledEvent = "DISABLED";
  public const string DismissPopupEvent = "DISMISS_POPUP";
  private Widget m_widget;
  private BnetPlayer m_player;
  private FriendListFriendFrame m_friendListFriendFrame;
  private FriendListFlyoutMenu m_flyoutMenu;
  private Dictionary<FriendListFlyoutMenu.ButtonOption, FriendListFlyoutMenu.ButtonEvent> m_buttonEvents;

  private void Awake()
  {
    this.m_widget = (Widget) this.gameObject.GetComponent<WidgetTemplate>();
    this.m_widget.SetLayerOverride(GameLayer.HighPriorityUI);
  }

  public void Init(
    BnetPlayer player,
    FriendListFriendFrame friendListFriendFrame,
    FriendListFlyoutMenu flyoutMenu)
  {
    this.m_player = player;
    this.m_friendListFriendFrame = friendListFriendFrame;
    this.m_flyoutMenu = flyoutMenu;
    UberText challengeFriendNameText = this.m_challengeFriendNameText;
    string str;
    if (!BnetFriendMgr.Get().IsFriend(this.m_player))
      str = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_MENU_HEADER_NONFRIEND", (object) this.m_player.GetBattleTag());
    else
      str = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_MENU_HEADER_FRIEND", (object) this.m_player.GetBattleTag());
    challengeFriendNameText.Text = str;
    this.InitializeButtonEvents();
    this.InitializePopup();
  }

  private void InitializeButtonEvents() => this.m_buttonEvents = new Dictionary<FriendListFlyoutMenu.ButtonOption, FriendListFlyoutMenu.ButtonEvent>()
  {
    {
      FriendListFlyoutMenu.ButtonOption.StandardHearthstone,
      new FriendListFlyoutMenu.ButtonEvent("STANDARD", (Widget) this.m_standardButton, new UIEvent.Handler(this.OnStandardButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnStandardButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.WildHearthstone,
      new FriendListFlyoutMenu.ButtonEvent("WILD", (Widget) this.m_wildButton, new UIEvent.Handler(this.OnWildButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnWildButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.ClassicHearthstone,
      new FriendListFlyoutMenu.ButtonEvent("CLASSIC", (Widget) this.m_classicButton, new UIEvent.Handler(this.OnClassicButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnClassicButtonOver))
    },
    {
      FriendListFlyoutMenu.ButtonOption.TavernBrawl,
      new FriendListFlyoutMenu.ButtonEvent("TAVERN_BRAWL", (Widget) this.m_tavernBrawlButton, new UIEvent.Handler(this.OnTavernBrawlButtonReleased), new FriendListFlyoutMenu.ShowTooltipEvent(this.OnTavernBrawlButtonOver))
    }
  };

  private void InitializePopup()
  {
    foreach (FriendListFlyoutMenu.ButtonOption key in this.m_buttonEvents.Keys)
    {
      FriendListFlyoutMenu.ButtonEvent buttonEvent = this.m_buttonEvents[key];
      if (buttonEvent != null)
        this.InitializeButton(buttonEvent.buttonWidget, key);
    }
  }

  private void InitializeButton(Widget buttonWidget, FriendListFlyoutMenu.ButtonOption option) => buttonWidget.RegisterReadyListener((Action<object>) (_ =>
  {
    buttonWidget.SetLayerOverride(GameLayer.HighPriorityUI);
    UIBButton uibButton = buttonWidget.GetComponentInChildren<UIBButton>();
    FriendListFlyoutMenu.ButtonEvent buttonEvent;
    if (!this.m_buttonEvents.TryGetValue(option, out buttonEvent))
      return;
    if (this.ShouldEnableOption(option))
    {
      buttonWidget.TriggerEvent("ENABLED");
      buttonEvent.isEnabled = true;
      if (buttonEvent.onRelease == null)
        return;
      uibButton.AddEventListener(UIEventType.RELEASE, buttonEvent.onRelease);
    }
    else
    {
      buttonWidget.TriggerEvent("DISABLED");
      buttonEvent.isEnabled = false;
      UIBHighlight componentInChildren = buttonWidget.GetComponentInChildren<UIBHighlight>();
      if ((UnityEngine.Object) componentInChildren.m_MouseOverHighlight != (UnityEngine.Object) null)
      {
        componentInChildren.m_MouseOverHighlight.SetActive(false);
        componentInChildren.m_MouseOverHighlight = (GameObject) null;
      }
      if (buttonEvent.onHover == null)
        return;
      TooltipZone tooltipZone = buttonWidget.GetComponentInChildren<TooltipZone>();
      uibButton.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (ventType => buttonEvent.onHover(uibButton, tooltipZone)));
      uibButton.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (ventType => tooltipZone.HideTooltip()));
    }
  }), (object) null, true);

  private bool ShouldEnableOption(FriendListFlyoutMenu.ButtonOption option)
  {
    switch (option)
    {
      case FriendListFlyoutMenu.ButtonOption.StandardHearthstone:
        return FriendChallengeMgr.Get().IsHearthstoneFriendlyChallengeAvailable(this.m_player) && CollectionManager.Get().AccountHasValidDeck(FormatType.FT_STANDARD);
      case FriendListFlyoutMenu.ButtonOption.WildHearthstone:
        return FriendChallengeMgr.Get().IsHearthstoneFriendlyChallengeAvailable(this.m_player) && CollectionManager.Get().ShouldAccountSeeStandardWild() && CollectionManager.Get().AccountHasValidDeck(FormatType.FT_WILD);
      case FriendListFlyoutMenu.ButtonOption.ClassicHearthstone:
        return FriendChallengeMgr.Get().IsHearthstoneFriendlyChallengeAvailable(this.m_player) && CollectionManager.Get().ShouldAccountSeeStandardWild() && CollectionManager.Get().AccountHasValidDeck(FormatType.FT_CLASSIC);
      case FriendListFlyoutMenu.ButtonOption.TavernBrawl:
        if ((!FriendChallengeMgr.Get().IsHearthstoneFriendlyChallengeAvailable(this.m_player) || !TavernBrawlManager.Get().IsTavernBrawlActive(BrawlType.BRAWL_TYPE_TAVERN_BRAWL) ? 0 : (TavernBrawlManager.Get().CanChallengeToTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL) ? 1 : 0)) == 0)
          return false;
        return !TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_TAVERN_BRAWL).canCreateDeck || TavernBrawlManager.Get().HasValidDeck(BrawlType.BRAWL_TYPE_TAVERN_BRAWL);
      default:
        return false;
    }
  }

  private void ShowTooltip(
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
    tooltipZone.ShowSocialTooltip((Component) button, headline, bodytext, 23f, GameLayer.HighPriorityUI);
    tooltipZone.AnchorTooltipTo(tooltipZone.gameObject, Anchor.TOP_RIGHT_XZ, Anchor.TOP_LEFT_XZ);
  }

  private void OnStandardButtonReleased(UIEvent e) => this.m_flyoutMenu.SendHearthstoneFriendlyChallenge();

  private void OnWildButtonReleased(UIEvent e)
  {
    if (!CollectionManager.Get().AccountHasValidDeck(FormatType.FT_WILD))
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
        m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_DECK"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
      DialogManager.Get().ShowPopup(info);
      this.m_friendListFriendFrame.CloseChallengeMenu();
    }
    else if (!FriendChallengeMgr.Get().IsOpponentAvailable(this.m_player))
    {
      this.m_flyoutMenu.ShowOpponentUnavailableAlert();
      this.m_friendListFriendFrame.CloseFriendsListMenu();
    }
    else
    {
      FriendChallengeMgr.Get().SetChallengeMethod(FriendChallengeMgr.ChallengeMethod.FROM_FRIEND_LIST);
      FriendChallengeMgr.Get().SendChallenge(this.m_player, FormatType.FT_WILD, true);
      this.m_friendListFriendFrame.CloseFriendsListMenu();
    }
  }

  private void OnClassicButtonReleased(UIEvent e)
  {
    if (!CollectionManager.Get().AccountHasValidDeck(FormatType.FT_CLASSIC))
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
        m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_CLASSIC_DECK"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
      DialogManager.Get().ShowPopup(info);
      this.m_friendListFriendFrame.CloseChallengeMenu();
    }
    else if (!FriendChallengeMgr.Get().IsOpponentAvailable(this.m_player))
    {
      this.m_flyoutMenu.ShowOpponentUnavailableAlert();
      this.m_friendListFriendFrame.CloseFriendsListMenu();
    }
    else
    {
      FriendChallengeMgr.Get().SetChallengeMethod(FriendChallengeMgr.ChallengeMethod.FROM_FRIEND_LIST);
      FriendChallengeMgr.Get().SendChallenge(this.m_player, FormatType.FT_CLASSIC, true);
      this.m_friendListFriendFrame.CloseFriendsListMenu();
    }
  }

  private void OnTavernBrawlButtonReleased(UIEvent e)
  {
    if (!FriendChallengeMgr.Get().IsOpponentAvailable(this.m_player))
    {
      this.m_flyoutMenu.ShowOpponentUnavailableAlert();
      this.m_friendListFriendFrame.CloseFriendsListMenu();
    }
    else if (!TavernBrawlManager.Get().HasUnlockedTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
        m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_TAVERN_BRAWL_LOCKED"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
      DialogManager.Get().ShowPopup(info);
      this.m_friendListFriendFrame.CloseChallengeMenu();
    }
    else if (!TavernBrawlManager.Get().CanChallengeToTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
        m_text = GameStrings.Format("GLOBAL_TAVERN_BRAWL_ERROR_SEASON_INCREMENTED"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
      DialogManager.Get().ShowPopup(info);
      this.m_friendListFriendFrame.CloseChallengeMenu();
    }
    else if (TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_TAVERN_BRAWL).canCreateDeck && !TavernBrawlManager.Get().HasValidDeck(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
    {
      FriendChallengeMgr.ShowChallengerNeedsToCreateTavernBrawlDeckAlert();
    }
    else
    {
      TavernBrawlManager.Get().CurrentBrawlType = BrawlType.BRAWL_TYPE_TAVERN_BRAWL;
      FriendChallengeMgr.Get().SendTavernBrawlChallenge(this.m_player, BrawlType.BRAWL_TYPE_TAVERN_BRAWL, TavernBrawlManager.Get().CurrentMission().seasonId, TavernBrawlManager.Get().CurrentMission().SelectedBrawlLibraryItemId);
      this.m_friendListFriendFrame.CloseFriendsListMenu();
    }
  }

  private bool ShouldShowGenericHearthstoneTooltip(out string reason)
  {
    if (!NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.Friendly)
    {
      reason = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_MODE_UNAVAILABLE";
      return true;
    }
    if (!GameUtils.IsTraditionalTutorialComplete())
    {
      reason = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_TRADITIONAL_LOCKED";
      return true;
    }
    if (this.m_player.GetHearthstoneGameAccount().GetTutorialBeaten() < 1)
    {
      reason = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_HEARTHSTONE_TUTORIAL_COMPLETE";
      return true;
    }
    return !FriendListFlyoutMenu.GetAvailability(this.m_player, out reason);
  }

  private void OnStandardButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    string headerKey = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
    string descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_AVAILABLE";
    string reason;
    if (this.ShouldShowGenericHearthstoneTooltip(out reason))
      descriptionFormat = reason;
    else if (!CollectionManager.Get().AccountHasValidDeck(FormatType.FT_STANDARD))
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_STANDARD_DECK";
    this.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
  }

  private void OnWildButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    string headerKey = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
    string descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_AVAILABLE";
    string reason;
    if (this.ShouldShowGenericHearthstoneTooltip(out reason))
      descriptionFormat = reason;
    else if (!CollectionManager.Get().ShouldAccountSeeStandardWild())
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_GAME_MODE_LOCKED";
    else if (!CollectionManager.Get().AccountHasValidDeck(FormatType.FT_WILD))
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_DECK";
    this.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
  }

  private void OnClassicButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    string headerKey = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
    string descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_AVAILABLE";
    string reason;
    if (this.ShouldShowGenericHearthstoneTooltip(out reason))
      descriptionFormat = reason;
    else if (!CollectionManager.Get().ShouldAccountSeeStandardWild())
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_GAME_MODE_LOCKED";
    else if (!CollectionManager.Get().AccountHasValidDeck(FormatType.FT_CLASSIC))
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_CLASSIC_DECK";
    this.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
  }

  private void OnTavernBrawlButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    string headerKey = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_HEADER";
    string descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_BUTTON_AVAILABLE";
    string reason;
    if (this.ShouldShowGenericHearthstoneTooltip(out reason))
      descriptionFormat = reason;
    else if (!TavernBrawlManager.Get().HasUnlockedTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_TAVERN_BRAWL_LOCKED";
    else if (!TavernBrawlManager.Get().IsTavernBrawlActive(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_TOOLTIP_NO_TAVERN_BRAWL";
    else if (!TavernBrawlManager.Get().CanChallengeToTavernBrawl(BrawlType.BRAWL_TYPE_TAVERN_BRAWL))
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_TOOLTIP_TAVERN_BRAWL_NOT_CHALLENGEABLE";
    else if ((!TavernBrawlManager.Get().GetMission(BrawlType.BRAWL_TYPE_TAVERN_BRAWL).canCreateDeck ? 1 : (TavernBrawlManager.Get().HasValidDeck(BrawlType.BRAWL_TYPE_TAVERN_BRAWL) ? 1 : 0)) == 0)
      descriptionFormat = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_TAVERN_BRAWL_DECK";
    this.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
  }
}
