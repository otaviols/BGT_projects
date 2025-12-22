using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone.UI;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof (WidgetTemplate))]
public class FriendListChallengeButton : MonoBehaviour
{
  public WidgetInstance m_flyoutMenuWidget;
  public TooltipZone m_tooltipZone;
  private BnetPlayer m_player;
  private Widget m_widget;
  private FriendListFlyoutMenu m_flyoutMenu;

  public bool IsChallengeMenuOpen { get; private set; }

  public FriendListFriendFrame FriendFrame { get; set; }

  protected void Awake()
  {
    this.m_widget = (Widget) this.GetComponent<WidgetTemplate>();
    this.m_widget.RegisterReadyListener((System.Action<object>) (_ => this.m_tooltipZone.gameObject.layer = 26), (object) null, true);
    this.m_flyoutMenuWidget.transform.position = Vector3.zero;
    this.m_flyoutMenuWidget.RegisterReadyListener((System.Action<object>) (_ =>
    {
      this.m_flyoutMenu = this.m_flyoutMenuWidget.GetComponentInChildren<FriendListFlyoutMenu>();
      this.StartCoroutine(this.InitializeFlyoutMenu());
    }), (object) null, true);
  }

  private void OnDestroy()
  {
    if (!this.IsChallengeMenuOpen)
      return;
    FriendlyChallengeHelper.Get().ActiveChallengeMenu = (BnetAccountId) null;
  }

  private IEnumerator InitializeFlyoutMenu()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    FriendListChallengeButton listChallengeButton = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      listChallengeButton.m_flyoutMenuWidget.SetLayerOverride(GameLayer.BattleNetDialog);
      FriendListFrame friendListFrame = ChatMgr.Get().FriendListFrame;
      listChallengeButton.m_flyoutMenuWidget.transform.SetParent(friendListFrame.friendFlyoutBone.transform);
      listChallengeButton.m_flyoutMenuWidget.transform.position = friendListFrame.friendFlyoutBone.transform.position;
      // ISSUE: reference to a compiler-generated method
      listChallengeButton.m_flyoutMenuWidget.RegisterEventListener(new Widget.EventListenerDelegate(listChallengeButton.\u003CInitializeFlyoutMenu\u003Eb__15_0));
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) null;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public bool SetPlayer(BnetPlayer player)
  {
    if (this.m_player == player)
      return false;
    this.m_player = player;
    return true;
  }

  public BnetPlayer GetPlayer() => this.m_player;

  public void UpdateFlyoutMenu()
  {
    if (!((UnityEngine.Object) this.m_flyoutMenu != (UnityEngine.Object) null))
      return;
    this.m_flyoutMenu.UpdateFlyoutMenu();
  }

  public void DismissPopups(bool showAlert = false)
  {
    if (!((UnityEngine.Object) this.m_flyoutMenu != (UnityEngine.Object) null))
      return;
    this.m_flyoutMenu.DismissPopups(showAlert);
  }
}
