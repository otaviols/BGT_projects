using UnityEngine;

public class FriendListRequestFrame : MonoBehaviour
{
  public GameObject m_Background;
  public FriendListUIElement m_AcceptButton;
  public FriendListUIElement m_DeclineButton;
  public UberText m_PlayerNameText;
  public UberText m_TimeText;
  private BnetInvitation m_invite;

  private void Awake()
  {
    this.m_AcceptButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnAcceptButtonPressed));
    this.m_DeclineButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeclineButtonPressed));
  }

  private void Update() => this.UpdateTimeText();

  private void OnEnable() => this.UpdateInvite();

  public BnetInvitation GetInvite() => this.m_invite;

  public void SetInvite(BnetInvitation invite)
  {
    if (this.m_invite == invite)
      return;
    this.m_invite = invite;
    this.UpdateInvite();
  }

  public void UpdateInvite()
  {
    if (!this.gameObject.activeSelf || this.m_invite == (BnetInvitation) null)
      return;
    this.m_PlayerNameText.Text = this.m_invite.GetInviterName();
    this.UpdateTimeText();
  }

  private void UpdateTimeText() => this.m_TimeText.Text = GameStrings.Format("GLOBAL_FRIENDLIST_REQUEST_SENT_TIME", (object) FriendUtils.GetRequestElapsedTimeString((long) this.m_invite.GetCreationTimeMicrosec()));

  private void OnAcceptButtonPressed(UIEvent e) => BnetFriendMgr.Get().AcceptInvite(this.m_invite);

  private void OnDeclineButtonPressed(UIEvent e) => BnetFriendMgr.Get().IgnoreInvite(this.m_invite.GetId());
}
