using Blizzard.GameService.SDK.Client.Integration;
using System.Collections.Generic;

public class PendingBnetFriendChangelist
{
  private List<BnetPlayer> m_friends = new List<BnetPlayer>();

  public List<BnetPlayer> GetFriends() => this.m_friends;

  public bool Add(BnetPlayer friend)
  {
    if (this.m_friends.Contains(friend))
      return false;
    this.m_friends.Add(friend);
    return true;
  }

  public bool Remove(BnetPlayer friend) => this.m_friends.Remove(friend);

  public void Clear() => this.m_friends.Clear();

  public int GetCount() => this.m_friends.Count;

  public BnetPlayer FindFriend(BnetAccountId id)
  {
    foreach (BnetPlayer friend in this.m_friends)
    {
      if ((BnetEntityId) friend.GetAccountId() == (BnetEntityId) id)
        return friend;
    }
    return (BnetPlayer) null;
  }

  public BnetPlayer FindFriend(BnetGameAccountId id)
  {
    foreach (BnetPlayer friend in this.m_friends)
    {
      if (friend.HasGameAccount(id))
        return friend;
    }
    return (BnetPlayer) null;
  }

  public bool IsFriend(BnetPlayer player)
  {
    if (this.m_friends.Contains(player))
      return true;
    if (player == null)
      return false;
    BnetAccountId accountId = player.GetAccountId();
    if ((BnetEntityId) accountId != (BnetEntityId) null)
      return this.IsFriend(accountId);
    foreach (BnetGameAccountId key in player.GetGameAccounts().Keys)
    {
      if (this.IsFriend(key))
        return true;
    }
    return false;
  }

  public bool IsFriend(BnetAccountId id) => this.FindFriend(id) != null;

  public bool IsFriend(BnetGameAccountId id) => this.FindFriend(id) != null;

  public BnetFriendChangelist CreateChangelist()
  {
    BnetFriendChangelist changelist = new BnetFriendChangelist();
    for (int index = this.m_friends.Count - 1; index >= 0; --index)
    {
      BnetPlayer friend = this.m_friends[index];
      if (friend.IsDisplayable())
      {
        changelist.AddAddedFriend(friend);
        this.m_friends.RemoveAt(index);
      }
    }
    return changelist;
  }
}
