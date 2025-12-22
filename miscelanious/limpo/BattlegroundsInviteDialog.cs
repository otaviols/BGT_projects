using Hearthstone.UI;
using UnityEngine;

[RequireComponent(typeof (Widget))]
public class BattlegroundsInviteDialog : DialogBase
{
  [SerializeField]
  private UberText m_challengerName;
  [SerializeField]
  private UberText m_inviteNote;
  [SerializeField]
  private UIBButton m_acceptButton;
  [SerializeField]
  private UIBButton m_denyButton;
  private FriendlyChallengeDialog.ResponseCallback m_responseCallback;

  protected override void Awake()
  {
    base.Awake();
    this.m_acceptButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ConfirmButtonPress));
    this.m_denyButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CancelButtonPress));
  }

  public void SetInfo(FriendlyChallengeDialog.Info info)
  {
    this.m_challengerName.Text = FriendUtils.GetUniqueName(info.m_challenger);
    this.m_inviteNote.gameObject.SetActive(BnetNearbyPlayerMgr.Get().IsNearbyStranger(info.m_challenger));
    this.m_responseCallback = info.m_callback;
  }

  private void ConfirmButtonPress(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    if (this.m_responseCallback != null)
      this.m_responseCallback(true);
    this.Hide();
  }

  private void CancelButtonPress(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    if (this.m_responseCallback != null)
      this.m_responseCallback(false);
    this.Hide();
  }

  public override void Show()
  {
    base.Show();
    BnetBar.Get().DisableButtonsByDialog((DialogBase) this);
    if ((bool) UniversalInputManager.UsePhoneUI && this.m_inviteNote.gameObject.activeSelf)
      this.transform.localPosition = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y + 50f, this.transform.localPosition.z);
    this.DoShowAnimation();
    UniversalInputManager.Get().SetSystemDialogActive(true);
    SoundManager.Get().LoadAndPlay((AssetReference) "friendly_challenge.prefab:649e070117bcd0d45bac691a03bf2dec");
    DialogBase.DoBlur();
  }

  public override void Hide()
  {
    base.Hide();
    SoundManager.Get().LoadAndPlay((AssetReference) "banner_shrink.prefab:d9de7386a7f2017429d126e972232123");
    DialogBase.EndBlur();
  }
}
