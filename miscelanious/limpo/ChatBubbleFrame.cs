using Blizzard.GameService.SDK.Client.Integration;
using UnityEngine;

public class ChatBubbleFrame : MonoBehaviour
{
  public GameObject m_VisualRoot;
  public GameObject m_MyDecoration;
  public GameObject m_TheirDecoration;
  public UberText m_NameText;
  public UberText m_MessageText;
  public Vector3_MobileOverride m_ScaleOverride;
  private BnetWhisper m_whisper;

  private void Awake() => BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));

  private void OnDestroy() => BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));

  public BnetWhisper GetWhisper() => this.m_whisper;

  public void SetWhisper(BnetWhisper whisper)
  {
    if (this.m_whisper == whisper)
      return;
    this.m_whisper = whisper;
    this.UpdateWhisper();
  }

  public bool DoesMessageFit() => !this.m_MessageText.IsEllipsized();

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(WhisperUtil.GetTheirAccountId(this.m_whisper));
    BnetPlayerChange change = changelist.FindChange(player);
    if (change == null)
      return;
    BnetPlayer oldPlayer = change.GetOldPlayer();
    BnetPlayer newPlayer = change.GetNewPlayer();
    if (oldPlayer != null && oldPlayer.IsOnline() == newPlayer.IsOnline())
      return;
    this.UpdateWhisper();
  }

  private void UpdateWhisper()
  {
    if (this.m_whisper == null)
      return;
    if ((BnetEntityId) this.m_whisper.GetSpeakerId() == (BnetEntityId) BnetPresenceMgr.Get().GetMyAccountId())
    {
      this.m_MyDecoration.SetActive(true);
      this.m_TheirDecoration.SetActive(false);
      this.m_NameText.Text = GameStrings.Format("GLOBAL_CHAT_BUBBLE_RECEIVER_NAME", (object) WhisperUtil.GetReceiver(this.m_whisper).GetBestName());
    }
    else
    {
      this.m_MyDecoration.SetActive(false);
      this.m_TheirDecoration.SetActive(true);
      BnetPlayer speaker = WhisperUtil.GetSpeaker(this.m_whisper);
      this.m_NameText.TextColor = !speaker.IsOnline() ? GameColors.PLAYER_NAME_OFFLINE : GameColors.PLAYER_NAME_ONLINE;
      this.m_NameText.Text = speaker.GetBestName();
    }
    string message = ChatUtils.GetMessage(this.m_whisper);
    string formattedDeckcodeMessage;
    this.m_MessageText.Text = !ChatUtils.TryGetFormattedDeckcodeMessage(message, false, out formattedDeckcodeMessage) ? message : formattedDeckcodeMessage;
    this.m_MessageText.Text += " ";
  }
}
