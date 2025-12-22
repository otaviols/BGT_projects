using PegasusShared;

public class LastGameData
{
  public TAG_PLAYSTATE GameResult { get; set; }

  public int WhizbangDeckID { get; set; }

  public GameConnectionInfo GameConnectionInfo { get; set; }

  public int BattlegroundsLeaderboardPlace { get; set; }

  public LastGameData() => this.Clear();

  public void Clear()
  {
    this.GameResult = TAG_PLAYSTATE.INVALID;
    this.WhizbangDeckID = 0;
    this.GameConnectionInfo = (GameConnectionInfo) null;
  }

  public bool HasWhizbangDeckID() => this.WhizbangDeckID > 0;
}
