using System.Collections.Generic;

public class BnetRecentOrNearbyPlayerChangelist
{
  private List<BnetPlayer> m_playersAdded = new List<BnetPlayer>();
  private List<BnetPlayer> m_playersUpdated = new List<BnetPlayer>();
  private List<BnetPlayer> m_playersRemoved = new List<BnetPlayer>();
  private List<BnetPlayer> m_friendsAdded = new List<BnetPlayer>();
  private List<BnetPlayer> m_friendsUpdated = new List<BnetPlayer>();
  private List<BnetPlayer> m_friendsRemoved = new List<BnetPlayer>();
  private List<BnetPlayer> m_strangersAdded = new List<BnetPlayer>();
  private List<BnetPlayer> m_strangersUpdated = new List<BnetPlayer>();
  private List<BnetPlayer> m_strangersRemoved = new List<BnetPlayer>();

  public List<BnetPlayer> GetAddedPlayers() => this.m_playersAdded;

  public List<BnetPlayer> GetRemovedPlayers() => this.m_playersRemoved;

  public List<BnetPlayer> GetAddedFriends() => this.m_friendsAdded;

  public List<BnetPlayer> GetRemovedFriends() => this.m_friendsRemoved;

  public List<BnetPlayer> GetAddedStrangers() => this.m_strangersAdded;

  public List<BnetPlayer> GetUpdatedStrangers() => this.m_strangersUpdated;

  public List<BnetPlayer> GetRemovedStrangers() => this.m_strangersRemoved;

  public bool IsEmpty() => (this.m_playersAdded == null || this.m_playersAdded.Count <= 0) && (this.m_playersUpdated == null || this.m_playersUpdated.Count <= 0) && (this.m_playersRemoved == null || this.m_playersRemoved.Count <= 0) && (this.m_friendsAdded == null || this.m_friendsAdded.Count <= 0) && (this.m_friendsUpdated == null || this.m_friendsUpdated.Count <= 0) && (this.m_friendsRemoved == null || this.m_friendsRemoved.Count <= 0) && (this.m_strangersAdded == null || this.m_strangersAdded.Count <= 0) && (this.m_strangersUpdated == null || this.m_strangersUpdated.Count <= 0) && (this.m_strangersRemoved == null || this.m_strangersRemoved.Count <= 0);

  public void Clear()
  {
    this.m_playersAdded.Clear();
    this.m_playersUpdated.Clear();
    this.m_playersRemoved.Clear();
    this.m_friendsAdded.Clear();
    this.m_friendsUpdated.Clear();
    this.m_friendsRemoved.Clear();
    this.m_strangersAdded.Clear();
    this.m_strangersUpdated.Clear();
    this.m_strangersRemoved.Clear();
  }

  public bool AddAddedPlayer(BnetPlayer player)
  {
    if (this.m_playersAdded.Contains(player))
      return false;
    this.m_playersAdded.Add(player);
    return true;
  }

  public bool AddUpdatedPlayer(BnetPlayer player)
  {
    if (this.m_playersUpdated.Contains(player))
      return false;
    this.m_playersUpdated.Add(player);
    return true;
  }

  public bool AddRemovedPlayer(BnetPlayer player)
  {
    if (this.m_playersRemoved.Contains(player))
      return false;
    this.m_playersRemoved.Add(player);
    return true;
  }

  public bool AddAddedFriend(BnetPlayer friend)
  {
    if (this.m_friendsAdded.Contains(friend))
      return false;
    this.m_friendsAdded.Add(friend);
    return true;
  }

  public bool AddUpdatedFriend(BnetPlayer friend)
  {
    if (this.m_friendsUpdated.Contains(friend))
      return false;
    this.m_friendsUpdated.Add(friend);
    return true;
  }

  public bool AddRemovedFriend(BnetPlayer friend)
  {
    if (this.m_friendsRemoved.Contains(friend))
      return false;
    this.m_friendsRemoved.Add(friend);
    return true;
  }

  public bool AddAddedStranger(BnetPlayer stranger)
  {
    if (this.m_strangersAdded.Contains(stranger))
      return false;
    this.m_strangersAdded.Add(stranger);
    return true;
  }

  public bool AddUpdatedStranger(BnetPlayer stranger)
  {
    if (this.m_strangersUpdated.Contains(stranger))
      return false;
    this.m_strangersUpdated.Add(stranger);
    return true;
  }

  public bool AddRemovedStranger(BnetPlayer stranger)
  {
    if (this.m_strangersRemoved.Contains(stranger))
      return false;
    this.m_strangersRemoved.Add(stranger);
    return true;
  }
}
