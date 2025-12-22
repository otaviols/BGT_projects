using Blizzard.GameService.SDK.Client.Integration;

public class BnetAccount
{
  private BnetAccountId m_id;
  private string m_fullName;
  private BnetBattleTag m_battleTag;
  private long m_lastOnlineMicrosec;
  private bool m_away;
  private long m_awayTimeMicrosec;
  private bool m_busy;
  private bool m_appearingOffline;

  public BnetAccount Clone()
  {
    BnetAccount bnetAccount = (BnetAccount) this.MemberwiseClone();
    if ((BnetEntityId) this.m_id != (BnetEntityId) null)
      bnetAccount.m_id = this.m_id.Clone();
    if (this.m_battleTag != (BnetBattleTag) null)
      bnetAccount.m_battleTag = this.m_battleTag.Clone();
    return bnetAccount;
  }

  public BnetAccountId GetId() => this.m_id;

  public void SetId(BnetAccountId id) => this.m_id = id;

  public string GetFullName() => this.m_fullName;

  public void SetFullName(string fullName) => this.m_fullName = fullName;

  public BnetBattleTag GetBattleTag() => this.m_battleTag;

  public void SetBattleTag(BnetBattleTag battleTag) => this.m_battleTag = battleTag;

  public long GetLastOnlineMicrosec() => this.m_lastOnlineMicrosec;

  public void SetLastOnlineMicrosec(long microsec) => this.m_lastOnlineMicrosec = microsec;

  public bool IsAway() => this.m_away;

  public void SetAway(bool away) => this.m_away = away;

  public long GetAwayTimeMicrosec() => this.m_awayTimeMicrosec;

  public void SetAwayTimeMicrosec(long awayTimeMicrosec) => this.m_awayTimeMicrosec = awayTimeMicrosec;

  public bool IsBusy() => this.m_busy;

  public void SetBusy(bool busy) => this.m_busy = busy;

  public bool IsAppearingOffline() => this.m_appearingOffline;

  public void SetAppearingOffline(bool appearingOffline) => this.m_appearingOffline = appearingOffline;

  public override bool Equals(object obj)
  {
    if (obj == null)
      return false;
    BnetAccount bnetAccount = obj as BnetAccount;
    return (object) bnetAccount != null && this.m_id.Equals((BnetEntityId) bnetAccount.m_id);
  }

  public override int GetHashCode() => this.m_id.GetHashCode();

  public static bool operator ==(BnetAccount a, BnetAccount b)
  {
    if ((object) a == (object) b)
      return true;
    return (object) a != null && (object) b != null && (BnetEntityId) a.m_id == (BnetEntityId) b.m_id;
  }

  public static bool operator !=(BnetAccount a, BnetAccount b) => !(a == b);

  public override string ToString()
  {
    if ((BnetEntityId) this.m_id == (BnetEntityId) null)
      return "UNKNOWN ACCOUNT";
    return string.Format("[id={0} m_fullName={1} battleTag={2} lastOnline={3}]", (object) this.m_id, (object) this.m_fullName, (object) this.m_battleTag, (object) TimeUtils.ConvertEpochMicrosecToDateTime(this.m_lastOnlineMicrosec));
  }

  public string FullPresenceSummary => string.Format("BnetAccount [id={0} fullName={1} battleTag={2} away={3} busy={4} lastOnline={5} awayTime={6}]", (object) this.m_id, (object) this.m_fullName, (object) this.m_battleTag, (object) this.m_away, (object) this.m_busy, this.m_lastOnlineMicrosec == 0L ? (object) "null" : (object) TimeUtils.ConvertEpochMicrosecToDateTime(this.m_lastOnlineMicrosec).ToString("R"), this.m_awayTimeMicrosec == 0L ? (object) "null" : (object) TimeUtils.ConvertEpochMicrosecToDateTime(this.m_awayTimeMicrosec).ToString("R"));
}
