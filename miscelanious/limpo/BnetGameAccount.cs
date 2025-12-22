using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using PegasusClient;
using PegasusFSG;
using System.Collections.Generic;

public class BnetGameAccount
{
  private BnetGameAccountId m_id;
  private BnetAccountId m_ownerId;
  private BnetProgramId m_programId;
  private BnetBattleTag m_battleTag;
  private bool m_away;
  private long m_awayTimeMicrosec;
  private bool m_busy;
  private bool m_online;
  private long m_lastOnlineMicrosec;
  private string m_richPresence;
  private Map<uint, object> m_gameFields = new Map<uint, object>();

  public BnetGameAccount Clone()
  {
    BnetGameAccount bnetGameAccount = (BnetGameAccount) this.MemberwiseClone();
    if ((BnetEntityId) this.m_id != (BnetEntityId) null)
      bnetGameAccount.m_id = this.m_id.Clone();
    if ((BnetEntityId) this.m_ownerId != (BnetEntityId) null)
      bnetGameAccount.m_ownerId = this.m_ownerId.Clone();
    if ((Blizzard.GameService.SDK.Client.Integration.FourCC) this.m_programId != (Blizzard.GameService.SDK.Client.Integration.FourCC) null)
      bnetGameAccount.m_programId = this.m_programId.Clone();
    if (this.m_battleTag != (BnetBattleTag) null)
      bnetGameAccount.m_battleTag = this.m_battleTag.Clone();
    bnetGameAccount.m_gameFields = new Map<uint, object>();
    foreach (KeyValuePair<uint, object> gameField in this.m_gameFields)
      bnetGameAccount.m_gameFields.Add(gameField.Key, gameField.Value);
    return bnetGameAccount;
  }

  public BnetGameAccountId GetId() => this.m_id;

  public void SetId(BnetGameAccountId id) => this.m_id = id;

  public BnetAccountId GetOwnerId() => this.m_ownerId;

  public void SetOwnerId(BnetAccountId id) => this.m_ownerId = id;

  public BnetProgramId GetProgramId() => this.m_programId;

  public void SetProgramId(BnetProgramId programId) => this.m_programId = programId;

  public BnetBattleTag GetBattleTag() => this.m_battleTag;

  public void SetBattleTag(BnetBattleTag battleTag) => this.m_battleTag = battleTag;

  public bool IsAway() => this.m_away;

  public void SetAway(bool away) => this.m_away = away;

  public long GetAwayTimeMicrosec() => this.m_awayTimeMicrosec;

  public void SetAwayTimeMicrosec(long awayTimeMicrosec) => this.m_awayTimeMicrosec = awayTimeMicrosec;

  public bool IsBusy() => this.m_busy;

  public void SetBusy(bool busy) => this.m_busy = busy;

  public bool IsOnline() => this.m_online;

  public void SetOnline(bool online) => this.m_online = online;

  public long GetLastOnlineMicrosec() => this.m_lastOnlineMicrosec;

  public void SetLastOnlineMicrosec(long microsec) => this.m_lastOnlineMicrosec = microsec;

  public string GetRichPresence() => this.m_richPresence;

  public void SetRichPresence(string richPresence) => this.m_richPresence = richPresence;

  public Map<uint, object> GetGameFields() => this.m_gameFields;

  public bool HasGameField(uint fieldId) => this.m_gameFields.ContainsKey(fieldId);

  public void SetGameField(uint fieldId, object val) => this.m_gameFields[fieldId] = val;

  public bool RemoveGameField(uint fieldId) => this.m_gameFields.Remove(fieldId);

  public bool TryGetGameField(uint fieldId, out object val) => this.m_gameFields.TryGetValue(fieldId, out val);

  public bool TryGetGameFieldBool(uint fieldId, out bool val)
  {
    val = false;
    object obj = (object) null;
    if (!this.m_gameFields.TryGetValue(fieldId, out obj))
      return false;
    val = (bool) obj;
    return true;
  }

  public bool TryGetGameFieldInt(uint fieldId, out int val)
  {
    val = 0;
    object obj = (object) null;
    if (!this.m_gameFields.TryGetValue(fieldId, out obj))
      return false;
    val = (int) obj;
    return true;
  }

  public bool TryGetGameFieldString(uint fieldId, out string val)
  {
    val = (string) null;
    object obj = (object) null;
    if (!this.m_gameFields.TryGetValue(fieldId, out obj))
      return false;
    val = (string) obj;
    return true;
  }

  public bool TryGetGameFieldBytes(uint fieldId, out byte[] val)
  {
    val = (byte[]) null;
    object obj = (object) null;
    if (!this.m_gameFields.TryGetValue(fieldId, out obj))
      return false;
    val = (byte[]) obj;
    return true;
  }

  public object GetGameField(uint fieldId)
  {
    object gameField = (object) null;
    this.m_gameFields.TryGetValue(fieldId, out gameField);
    return gameField;
  }

  public bool GetGameFieldBool(uint fieldId)
  {
    object obj = (object) null;
    return this.m_gameFields.TryGetValue(fieldId, out obj) && obj != null && (bool) obj;
  }

  public int GetGameFieldInt(uint fieldId)
  {
    object obj = (object) null;
    return this.m_gameFields.TryGetValue(fieldId, out obj) && obj != null ? (int) obj : 0;
  }

  public string GetGameFieldString(uint fieldId)
  {
    object obj = (object) null;
    return this.m_gameFields.TryGetValue(fieldId, out obj) && obj != null ? (string) obj : (string) null;
  }

  public byte[] GetGameFieldBytes(uint fieldId)
  {
    object obj = (object) null;
    return this.m_gameFields.TryGetValue(fieldId, out obj) && obj != null ? (byte[]) obj : (byte[]) null;
  }

  public BnetEntityId GetGameFieldEntityId(uint fieldId)
  {
    object obj = (object) null;
    return this.m_gameFields.TryGetValue(fieldId, out obj) && obj != null ? (BnetEntityId) obj : new BnetEntityId(0UL, 0UL);
  }

  public bool CanBeInvitedToGame() => this.GetGameFieldBool(1U);

  public string GetClientVersion() => this.GetGameFieldString(19U);

  public string GetClientEnv() => this.GetGameFieldString(20U);

  public string GetDebugString() => this.GetGameFieldString(2U);

  public BnetPartyId GetPartyId()
  {
    BnetEntityId gameFieldEntityId = this.GetGameFieldEntityId(26U);
    return new BnetPartyId(gameFieldEntityId.High, gameFieldEntityId.Low);
  }

  public SessionRecord GetSessionRecord()
  {
    byte[] gameFieldBytes = this.GetGameFieldBytes(22U);
    return gameFieldBytes != null && gameFieldBytes.Length != 0 ? ProtobufUtil.ParseFrom<SessionRecord>(gameFieldBytes) : (SessionRecord) null;
  }

  public string GetCardsOpened() => this.GetGameFieldString(4U);

  public int GetLastAchievement() => this.GetGameFieldInt(27U);

  public int GetDruidLevel() => this.GetGameFieldInt(5U);

  public int GetHunterLevel() => this.GetGameFieldInt(6U);

  public int GetMageLevel() => this.GetGameFieldInt(7U);

  public int GetPaladinLevel() => this.GetGameFieldInt(8U);

  public int GetPriestLevel() => this.GetGameFieldInt(9U);

  public int GetRogueLevel() => this.GetGameFieldInt(10U);

  public int GetShamanLevel() => this.GetGameFieldInt(11U);

  public int GetWarlockLevel() => this.GetGameFieldInt(12U);

  public int GetWarriorLevel() => this.GetGameFieldInt(13U);

  public int GetGainMedal() => this.GetGameFieldInt(14U);

  public int GetTutorialBeaten() => this.GetGameFieldInt(15U);

  public bool GetBattlegroundsTutorialComplete() => this.GetGameFieldInt(28U) > 0;

  public bool GetMercenariesTutorialComplete() => this.GetGameFieldInt(29U) > 0;

  public int GetCollectionEvent() => this.GetGameFieldInt(16U);

  public DeckValidity GetDeckValidity()
  {
    byte[] gameFieldBytes = this.GetGameFieldBytes(24U);
    return gameFieldBytes != null && gameFieldBytes.Length != 0 ? ProtobufUtil.ParseFrom<DeckValidity>(gameFieldBytes) : (DeckValidity) null;
  }

  public override bool Equals(object obj)
  {
    if (obj == null)
      return false;
    BnetGameAccount bnetGameAccount = obj as BnetGameAccount;
    return (object) bnetGameAccount != null && this.m_id.Equals((BnetEntityId) bnetGameAccount.m_id);
  }

  public bool Equals(BnetGameAccountId other) => other != null && this.m_id.Equals((BnetEntityId) other);

  public override int GetHashCode() => this.m_id.GetHashCode();

  public static bool operator ==(BnetGameAccount a, BnetGameAccount b)
  {
    if ((object) a == (object) b)
      return true;
    return (object) a != null && (object) b != null && (BnetEntityId) a.m_id == (BnetEntityId) b.m_id;
  }

  public static bool operator !=(BnetGameAccount a, BnetGameAccount b) => !(a == b);

  public override string ToString()
  {
    if ((BnetEntityId) this.m_id == (BnetEntityId) null)
      return "UNKNOWN GAME ACCOUNT";
    return string.Format("[id={0} programId={1} battleTag={2} online={3}]", (object) this.m_id, (object) this.m_programId, (object) this.m_battleTag, (object) this.m_online);
  }

  public string FullPresenceSummary => string.Format("GameAccount [id={0} battleTag={1} {2} {3} richPresence={4} away={5} busy={6} lastOnline={7} awayTime={8}]", (object) this.m_id, (object) this.m_battleTag, (object) this.m_programId, this.m_online ? (object) "online" : (object) "offline", (object) this.m_richPresence, (object) this.m_away, (object) this.m_busy, this.m_lastOnlineMicrosec == 0L ? (object) "null" : (object) TimeUtils.ConvertEpochMicrosecToDateTime(this.m_lastOnlineMicrosec).ToString("R"), this.m_awayTimeMicrosec == 0L ? (object) "null" : (object) TimeUtils.ConvertEpochMicrosecToDateTime(this.m_awayTimeMicrosec).ToString("R"));
}
