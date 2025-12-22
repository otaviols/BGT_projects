using Blizzard.GameService.SDK.Client.Integration;
using System.Collections.Generic;
using UnityEngine;

public class BnetRecentPlayerMgr
{
  private static readonly int MAX_NUMBER_ENTRIES_PC_TABLET = 5;
  private static readonly int MAX_NUMBER_ENTRIES_PHONE = 2;
  private static BnetRecentPlayerMgr s_instance;
  private int m_maxNumberOfEntries;
  private List<BnetPlayer> m_recentPlayers = new List<BnetPlayer>();
  private Dictionary<BnetPlayer, BnetRecentPlayerMgr.RecentReason> m_recentPlayerData = new Dictionary<BnetPlayer, BnetRecentPlayerMgr.RecentReason>();
  private List<BnetPlayer> m_recentFriends = new List<BnetPlayer>();
  private List<BnetPlayer> m_recentStrangers = new List<BnetPlayer>();
  private List<BnetRecentPlayerMgr.ChangeListener> m_changeListeners = new List<BnetRecentPlayerMgr.ChangeListener>();
  private BnetRecentOrNearbyPlayerChangelist m_changelist = new BnetRecentOrNearbyPlayerChangelist();
  private HashSet<BnetEntityId> m_pendingFriendsById = new HashSet<BnetEntityId>();
  private HashSet<string> m_pendingFriendsByBattleTag = new HashSet<string>();
  private BnetPlayer m_lastOpponent;

  private BnetRecentPlayerMgr() => this.m_maxNumberOfEntries = (bool) UniversalInputManager.UsePhoneUI ? BnetRecentPlayerMgr.MAX_NUMBER_ENTRIES_PHONE : BnetRecentPlayerMgr.MAX_NUMBER_ENTRIES_PC_TABLET;

  public static BnetRecentPlayerMgr Get()
  {
    if (BnetRecentPlayerMgr.s_instance == null)
      BnetRecentPlayerMgr.s_instance = new BnetRecentPlayerMgr();
    return BnetRecentPlayerMgr.s_instance;
  }

  public void Initialize()
  {
    BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    FriendChallengeMgr.Get().AddChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnChallengeChanged));
  }

  public void Shutdown()
  {
    BnetFriendMgr.Get().RemoveChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    FriendChallengeMgr.Get().RemoveChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnChallengeChanged));
  }

  public List<BnetPlayer> GetRecentPlayers() => this.m_recentPlayers;

  public string GetRecentReason(BnetPlayer player)
  {
    if (!this.m_recentPlayerData.ContainsKey(player))
      return (string) null;
    switch (this.m_recentPlayerData[player])
    {
      case BnetRecentPlayerMgr.RecentReason.RECENT_OPPONENT:
        return GameStrings.Get("GLOBAL_FRIENDLIST_RECENT_OPPONENT_STATUS");
      case BnetRecentPlayerMgr.RecentReason.FORMER_FRIEND:
        return GameStrings.Get("GLOBAL_FRIENDLIST_FORMER_FRIEND_STATUS");
      case BnetRecentPlayerMgr.RecentReason.NEW_FRIEND:
        return GameStrings.Get("GLOBAL_FRIENDLIST_NEW_FRIEND_STATUS");
      case BnetRecentPlayerMgr.RecentReason.RECENT_CHALLENGED:
        return GameStrings.Get("GLOBAL_FRIENDLIST_RECENT_CHALLENGED_STATUS");
      case BnetRecentPlayerMgr.RecentReason.RECENT_CHATTED:
        return GameStrings.Get("GLOBAL_FRIENDLIST_RECENT_CHATTED_STATUS");
      case BnetRecentPlayerMgr.RecentReason.LAST_OPPONENT:
        return GameStrings.Get("GLOBAL_FRIENDLIST_LAST_OPPONENT_STATUS");
      case BnetRecentPlayerMgr.RecentReason.CURRENT_OPPONENT:
        return GameStrings.Get("GLOBAL_FRIENDLIST_CURRENT_OPPONENT_STATUS");
      default:
        return (string) null;
    }
  }

  public bool IsCurrentOpponent(BnetPlayer player) => this.m_recentPlayerData.ContainsKey(player) && this.m_recentPlayerData[player] == BnetRecentPlayerMgr.RecentReason.CURRENT_OPPONENT;

  public bool IsRecentStranger(BnetPlayer player) => BnetRecentPlayerMgr.IsRecentInList(player, this.m_recentStrangers);

  public bool IsRecentPlayer(BnetPlayer player) => BnetRecentPlayerMgr.IsRecentInList(player, this.m_recentPlayers);

  private static bool IsRecentInList(BnetPlayer player, List<BnetPlayer> bnetPlayers)
  {
    if (player == null)
      return false;
    BnetAccountId accountId = player.GetAccountId();
    if ((BnetEntityId) accountId != (BnetEntityId) null)
    {
      for (int index = 0; index < bnetPlayers.Count; ++index)
      {
        if ((BnetEntityId) accountId == (BnetEntityId) bnetPlayers[index].GetAccountId())
          return true;
      }
      return false;
    }
    BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
    if (!((BnetEntityId) hearthstoneGameAccountId != (BnetEntityId) null))
      return false;
    for (int index = 0; index < bnetPlayers.Count; ++index)
    {
      if ((BnetEntityId) hearthstoneGameAccountId == (BnetEntityId) bnetPlayers[index].GetHearthstoneGameAccountId())
        return true;
    }
    return false;
  }

  public bool AddChangeListener(BnetRecentPlayerMgr.ChangeCallback callback)
  {
    BnetRecentPlayerMgr.ChangeListener changeListener = new BnetRecentPlayerMgr.ChangeListener();
    changeListener.SetCallback(callback);
    if (this.m_changeListeners.Contains(changeListener))
      return false;
    this.m_changeListeners.Add(changeListener);
    return true;
  }

  public bool RemoveChangeListenerFromInstance(BnetRecentPlayerMgr.ChangeCallback callback)
  {
    BnetRecentPlayerMgr.ChangeListener changeListener = new BnetRecentPlayerMgr.ChangeListener();
    changeListener.SetCallback(callback);
    return this.m_changeListeners.Remove(changeListener);
  }

  public void AddRecentPlayer(BnetPlayer player, BnetRecentPlayerMgr.RecentReason recentReason)
  {
    if (player == null || !NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().RecentFriendListDisplayEnabled)
      return;
    if (recentReason == BnetRecentPlayerMgr.RecentReason.LAST_OPPONENT)
    {
      if (this.m_lastOpponent != null)
        this.m_recentPlayerData[this.m_lastOpponent] = BnetRecentPlayerMgr.RecentReason.RECENT_OPPONENT;
      this.m_lastOpponent = player;
    }
    player.TimeLastAddedToRecentPlayers = Time.time;
    if (this.m_recentPlayers.Contains(player))
    {
      this.m_recentPlayerData[player] = recentReason;
    }
    else
    {
      BnetRecentOrNearbyPlayerChangelist changelist = new BnetRecentOrNearbyPlayerChangelist();
      this.m_recentPlayers.Add(player);
      changelist.AddAddedPlayer(player);
      this.m_recentPlayerData[player] = recentReason;
      if (BnetFriendMgr.Get().IsFriend(player))
      {
        this.m_recentFriends.Add(player);
        changelist.AddAddedFriend(player);
      }
      else
      {
        this.m_recentStrangers.Add(player);
        changelist.AddAddedStranger(player);
      }
      this.RemoveNoLongerRecentPlayers(changelist);
      this.FireChangeEvent(changelist);
    }
  }

  public void AddPendingFriend(BnetEntityId playerId)
  {
    if (this.m_pendingFriendsById.Contains(playerId))
      return;
    this.m_pendingFriendsById.Add(playerId);
  }

  public void AddPendingFriend(string playerBattleTag)
  {
    if (this.m_pendingFriendsByBattleTag.Contains(playerBattleTag))
      return;
    this.m_pendingFriendsByBattleTag.Add(playerBattleTag);
  }

  public BnetPlayer GetCurrentOpponent()
  {
    foreach (KeyValuePair<BnetPlayer, BnetRecentPlayerMgr.RecentReason> keyValuePair in this.m_recentPlayerData)
    {
      if (keyValuePair.Value == BnetRecentPlayerMgr.RecentReason.CURRENT_OPPONENT)
        return keyValuePair.Key;
    }
    return (BnetPlayer) null;
  }

  public void Update()
  {
    this.m_changelist.Clear();
    this.RemoveNoLongerRecentPlayers(this.m_changelist);
    this.FireChangeEvent(this.m_changelist);
  }

  private void RemoveNoLongerRecentPlayers(BnetRecentOrNearbyPlayerChangelist changelist)
  {
    List<BnetPlayer> bnetPlayerList = (List<BnetPlayer>) null;
    for (int index = this.m_recentPlayers.Count - 1; index >= 0; --index)
    {
      bool flag = false;
      BnetPlayer recentPlayer = this.m_recentPlayers[index];
      if (this.m_recentPlayers.Count - index > this.m_maxNumberOfEntries)
        flag = true;
      if (flag)
      {
        if (bnetPlayerList == null)
          bnetPlayerList = new List<BnetPlayer>();
        bnetPlayerList.Add(recentPlayer);
      }
    }
    if (bnetPlayerList == null)
      return;
    foreach (BnetPlayer recentPlayer in bnetPlayerList)
      this.RemoveRecentPlayer(recentPlayer, changelist);
  }

  private void RemoveRecentPlayer(
    BnetPlayer recentPlayer,
    BnetRecentOrNearbyPlayerChangelist changelist)
  {
    this.m_recentPlayers.Remove(recentPlayer);
    changelist.AddRemovedPlayer(recentPlayer);
    if (this.m_recentFriends.Remove(recentPlayer))
    {
      changelist.AddRemovedFriend(recentPlayer);
    }
    else
    {
      if (!this.m_recentStrangers.Remove(recentPlayer))
        return;
      changelist.AddRemovedStranger(recentPlayer);
    }
  }

  private void FireChangeEvent(BnetRecentOrNearbyPlayerChangelist changelist)
  {
    if (changelist.IsEmpty())
      return;
    foreach (BnetRecentPlayerMgr.ChangeListener changeListener in this.m_changeListeners.ToArray())
      changeListener.Fire(changelist);
  }

  private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
  {
    List<BnetPlayer> addedFriends = changelist.GetAddedFriends();
    if (addedFriends != null && (this.m_pendingFriendsById.Count > 0 || this.m_pendingFriendsByBattleTag.Count > 0))
    {
      foreach (BnetPlayer player in addedFriends)
      {
        BnetEntityId accountId = (BnetEntityId) player.GetAccountId();
        string str = player.GetBattleTag().ToString();
        if (this.m_pendingFriendsById.Contains(accountId))
        {
          this.m_pendingFriendsById.Remove(accountId);
          this.AddRecentPlayer(player, BnetRecentPlayerMgr.RecentReason.NEW_FRIEND);
        }
        else if (this.m_pendingFriendsByBattleTag.Contains(str))
        {
          this.m_pendingFriendsByBattleTag.Remove(str);
          this.AddRecentPlayer(player, BnetRecentPlayerMgr.RecentReason.NEW_FRIEND);
        }
      }
    }
    List<BnetPlayer> removedFriends = changelist.GetRemovedFriends();
    if (removedFriends == null)
      return;
    foreach (BnetPlayer player in removedFriends)
      this.AddRecentPlayer(player, BnetRecentPlayerMgr.RecentReason.FORMER_FRIEND);
  }

  private void OnChallengeChanged(
    FriendChallengeEvent challengeEvent,
    BnetPlayer player,
    FriendlyChallengeData challengeData,
    object userData)
  {
    if (challengeEvent != FriendChallengeEvent.I_SENT_CHALLENGE && challengeEvent != FriendChallengeEvent.I_RECEIVED_CHALLENGE)
      return;
    this.AddRecentPlayer(player, BnetRecentPlayerMgr.RecentReason.RECENT_CHALLENGED);
  }

  public BnetPlayer Cheat_CreateRecentPlayer(
    string fullName,
    int leagueId,
    int starLevel,
    BnetProgramId programId,
    bool isFriend,
    bool isOnline)
  {
    BnetPlayer player = BnetFriendMgr.Get().Cheat_CreatePlayer(fullName, leagueId, starLevel, programId, isFriend, isOnline);
    this.AddRecentPlayer(player, BnetRecentPlayerMgr.RecentReason.CHEAT);
    return player;
  }

  public int Cheat_RemoveCheatFriends()
  {
    int num = 0;
    BnetRecentOrNearbyPlayerChangelist changelist = new BnetRecentOrNearbyPlayerChangelist();
    for (int index = this.m_recentPlayers.Count - 1; index >= 0; --index)
    {
      BnetPlayer recentPlayer = this.m_recentPlayers[index];
      if (recentPlayer.IsCheatPlayer)
      {
        this.RemoveRecentPlayer(recentPlayer, changelist);
        ++num;
      }
    }
    this.FireChangeEvent(changelist);
    return num;
  }

  public delegate void ChangeCallback(
    BnetRecentOrNearbyPlayerChangelist changelist,
    object userData);

  private class ChangeListener : EventListener<BnetRecentPlayerMgr.ChangeCallback>
  {
    public void Fire(BnetRecentOrNearbyPlayerChangelist changelist) => this.m_callback(changelist, this.m_userData);
  }

  public enum RecentReason
  {
    INVALID,
    RECENT_OPPONENT,
    FORMER_FRIEND,
    NEW_FRIEND,
    RECENT_CHALLENGED,
    RECENT_CHATTED,
    LAST_OPPONENT,
    CURRENT_OPPONENT,
    RECENT_SPECTATED,
    CHEAT,
  }
}
