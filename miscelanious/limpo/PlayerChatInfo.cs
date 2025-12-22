using Blizzard.GameService.SDK.Client.Integration;

public class PlayerChatInfo
{
  private BnetPlayer m_player;
  private float m_lastFocusTime;
  private BnetWhisper m_lastSeenWhisper;

  public BnetPlayer GetPlayer() => this.m_player;

  public void SetPlayer(BnetPlayer player) => this.m_player = player;

  public void SetLastFocusTime(float time) => this.m_lastFocusTime = time;

  public BnetWhisper GetLastSeenWhisper() => this.m_lastSeenWhisper;

  public void SetLastSeenWhisper(BnetWhisper whisper) => this.m_lastSeenWhisper = whisper;
}
