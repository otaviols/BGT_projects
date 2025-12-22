using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone.UI;
using UnityEngine;

[RequireComponent(typeof (Widget))]
public class BattlegroundsSuggestDialog : DialogBase
{
  public const string PrivatePartyInfo = "GLUE_BACON_PRIVATE_PARTY_INFO";
  public const string PartySuggestionIdFormat = "partysuggestion_{0}";
  [SerializeField]
  private UberText m_suggestionText;
  [SerializeField]
  private UberText m_playerToInviteName;
  [SerializeField]
  private UberText m_inviteNote;
  [SerializeField]
  private UIBButton m_acceptButton;
  [SerializeField]
  private UIBButton m_denyButton;
  private BnetGameAccountId m_playerToInvite;
  private BnetGameAccountId m_suggester;
  private BattlegroundsSuggestDialog.ResponseCallback m_responseCallback;

  protected override void Awake()
  {
    base.Awake();
    this.m_acceptButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.ConfirmButtonPress));
    this.m_denyButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.CancelButtonPress));
  }

  public void SetInfo(BattlegroundsSuggestDialog.Info info)
  {
    this.m_suggester = info.SuggesterGameAccountId;
    string str = info.SuggesterName;
    BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(info.SuggesterGameAccountId);
    if (player != null)
      str = FriendUtils.GetUniqueNameWithColor(player);
    this.m_suggestionText.Text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_BODY_BATTLEGROUNDS_SUGGESTION", (object) str);
    this.m_playerToInvite = info.PlayerToInviteGameAccountId;
    this.m_playerToInviteName.Text = info.PlayerToInviteName;
    if (PartyManager.Get().GetCurrentPartySize() == PartyManager.Get().GetBattlegroundsMaxRankedPartySize())
    {
      this.m_inviteNote.Text = GameStrings.Format("GLUE_BACON_PRIVATE_PARTY_INFO");
      this.m_inviteNote.gameObject.SetActive(true);
    }
    else
      this.m_inviteNote.gameObject.SetActive(false);
    this.m_responseCallback = info.Callback;
  }

  private void ConfirmButtonPress(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    if (this.m_responseCallback != null)
      this.m_responseCallback(true, this.m_playerToInvite);
    DialogManager.Get().RemoveUniquePopupRequestFromQueue(string.Format("partysuggestion_{0}", (object) this.m_playerToInvite.Low));
    this.Hide();
  }

  private void CancelButtonPress(UIEvent e)
  {
    SoundManager.Get().LoadAndPlay((AssetReference) "Small_Click.prefab:2a1c5335bf08dc84eb6e04fc58160681");
    if (this.m_responseCallback != null)
      this.m_responseCallback(false, this.m_playerToInvite);
    DialogManager.Get().RemoveUniquePopupRequestFromQueue(string.Format("partysuggestion_{0}", (object) this.m_playerToInvite.Low));
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

  public delegate void ResponseCallback(bool accept, BnetGameAccountId playerToInvite);

  public class Info : AlertPopup.PopupInfo
  {
    public BnetGameAccountId PlayerToInviteGameAccountId;
    public string PlayerToInviteName;
    public BnetGameAccountId SuggesterGameAccountId;
    public string SuggesterName;
    public BattlegroundsSuggestDialog.ResponseCallback Callback;
  }
}
