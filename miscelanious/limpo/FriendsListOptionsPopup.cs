using Hearthstone.UI;
using System;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class FriendsListOptionsPopup : MonoBehaviour
{
  [SerializeField]
  private UberText m_optionsMenuHeader;
  [SerializeField]
  private WidgetInstance m_removeFriendButton;
  [SerializeField]
  private WidgetInstance m_reportButton;
  public const string DismissPopupEvent = "DISMISS_POPUP";
  private Widget m_widget;
  private BnetPlayer m_player;
  private FriendListFlyoutMenu m_flyoutMenu;

  private void Awake()
  {
    this.m_widget = this.GetComponent<Widget>();
    this.m_removeFriendButton.RegisterReadyListener((Action<object>) (_ =>
    {
      this.m_removeFriendButton.SetLayerOverride(GameLayer.HighPriorityUI);
      this.m_removeFriendButton.GetComponentInChildren<UIBButton>().AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRemoveFriendButtonReleased));
    }), (object) null, true);
    this.m_reportButton.RegisterReadyListener((Action<object>) (_ =>
    {
      this.m_reportButton.SetLayerOverride(GameLayer.HighPriorityUI);
      UIBButton uibButton = this.m_reportButton.GetComponentInChildren<UIBButton>();
      NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
      if (netObject != null && !netObject.ReportPlayerEnabled)
      {
        this.m_reportButton.TriggerEvent("DISABLED", new Widget.TriggerEventParameters());
        TooltipZone tooltipZone = this.m_reportButton.GetComponentInChildren<TooltipZone>(true);
        uibButton.AddEventListener(UIEventType.ROLLOVER, (UIEvent.Handler) (eventType => this.OnReportButtonOver(uibButton, tooltipZone)));
        uibButton.AddEventListener(UIEventType.ROLLOUT, (UIEvent.Handler) (eventType => tooltipZone.HideTooltip()));
      }
      else
      {
        this.m_reportButton.TriggerEvent("ENABLED", new Widget.TriggerEventParameters());
        uibButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnReportButtonReleased));
      }
    }), (object) null, true);
  }

  public void Init(BnetPlayer player, FriendListFlyoutMenu flyoutMenu)
  {
    this.m_player = player;
    this.m_flyoutMenu = flyoutMenu;
    this.m_optionsMenuHeader.Text = player.GetBattleTag().ToString();
  }

  private void OnRemoveFriendButtonReleased(UIEvent e)
  {
    ChatMgr.Get().FriendListFrame.ShowRemoveFriendPopup(this.m_player);
    this.m_widget.TriggerEvent("DISMISS_POPUP");
  }

  private void OnReportButtonReleased(UIEvent e)
  {
    this.m_flyoutMenu.ShowReportingPopup();
    this.m_widget.TriggerEvent("DISMISS_POPUP");
  }

  private void OnReportButtonOver(UIBButton button, TooltipZone tooltipZone)
  {
    string headerKey = "GLOBAL_FRIENDLIST_REPORT_TOOLTIP_HEADER";
    string descriptionFormat = "GLOBAL_FRIENDLIST_REPORT_TOOLTIP_CURRENTLY_UNAVAILABLE_TEXT";
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    if (netObject == null || netObject.ReportPlayerEnabled)
      return;
    this.ShowTooltip(this.m_player, headerKey, descriptionFormat, tooltipZone, button);
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
}
