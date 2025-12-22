using Blizzard.GameService.SDK.Client.Integration;

public class BnetInvitation
{
  private BnetInvitationId m_id;
  private BnetEntityId m_inviterId;
  private string m_inviterName;
  private BnetEntityId m_inviteeId;
  private string m_inviteeName;
  private string m_message;
  private ulong m_creationTimeMicrosec;
  private ulong m_expirationTimeMicrosec;

  public static BnetInvitation CreateFromFriendsUpdate(FriendsUpdate src)
  {
    BnetInvitation fromFriendsUpdate = new BnetInvitation();
    fromFriendsUpdate.m_id = new BnetInvitationId(src.long1);
    if (src.entity1 != (BnetEntityId) null)
      fromFriendsUpdate.m_inviterId = src.entity1.Clone();
    if (src.entity2 != (BnetEntityId) null)
      fromFriendsUpdate.m_inviteeId = src.entity2.Clone();
    fromFriendsUpdate.m_inviterName = src.string1;
    fromFriendsUpdate.m_inviteeName = src.string2;
    fromFriendsUpdate.m_message = src.string3;
    fromFriendsUpdate.m_creationTimeMicrosec = src.long2;
    fromFriendsUpdate.m_expirationTimeMicrosec = src.long3;
    return fromFriendsUpdate;
  }

  public BnetInvitationId GetId() => this.m_id;

  public BnetEntityId GetInviterId() => this.m_inviterId;

  public string GetInviterName() => this.m_inviterName;

  public ulong GetCreationTimeMicrosec() => this.m_creationTimeMicrosec;

  public override bool Equals(object obj)
  {
    if (obj == null)
      return false;
    BnetInvitation bnetInvitation = obj as BnetInvitation;
    return (object) bnetInvitation != null && this.m_id.Equals(bnetInvitation.m_id);
  }

  public override int GetHashCode() => this.m_id.GetHashCode();

  public static bool operator ==(BnetInvitation a, BnetInvitation b)
  {
    if ((object) a == (object) b)
      return true;
    return (object) a != null && (object) b != null && a.m_id == b.m_id;
  }

  public override string ToString()
  {
    if (this.m_id == (BnetInvitationId) null)
      return "UNKNOWN INVITATION";
    return string.Format("[id={0} inviterId={1} inviterName={2} inviteeId={3} inviteeName={4} message={5}]", (object) this.m_id, (object) this.m_inviterId, (object) this.m_inviterName, (object) this.m_inviteeId, (object) this.m_inviteeName, (object) this.m_message);
  }
}
