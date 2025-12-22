using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone;
using PegasusClient;
using PegasusShared;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class BnetFriendMgr
{
  private static BnetFriendMgr s_instance;
  private int m_maxFriends;
  private int m_maxReceivedInvites;
  private int m_maxSentInvites;
  private List<BnetPlayer> m_friends = new List<BnetPlayer>();
  private List<BnetInvitation> m_receivedInvites = new List<BnetInvitation>();
  private List<BnetInvitation> m_sentInvites = new List<BnetInvitation>();
  private List<BnetFriendMgr.ChangeListener> m_changeListeners = new List<BnetFriendMgr.ChangeListener>();
  private PendingBnetFriendChangelist m_pendingChangelist = new PendingBnetFriendChangelist();
  private bool m_isRegisteredToFriendHandler;
  private bool m_isFriendInviteFeatureEnabled;
  private static ulong nextIdToken;

  public bool IsFriendInviteFeatureEnabled => this.m_isFriendInviteFeatureEnabled;

  public static BnetFriendMgr Get()
  {
    if (BnetFriendMgr.s_instance == null)
    {
      BnetFriendMgr.s_instance = new BnetFriendMgr();
      HearthstoneApplication.Get().WillReset += new System.Action(BnetFriendMgr.s_instance.Clear);
    }
    return BnetFriendMgr.s_instance;
  }

  public void Initialize()
  {
    FriendMgr.Get().Initialize();
    Network.Get().OnDisconnectedFromBattleNet += new System.Action<BattleNetErrors>(this.OnDisconnectedFromBattleNet);
    this.RegisterFriendHandler();
    this.InitMaximums();
  }

  public void SetFriendInviteFeatureStatus(bool isEnabled)
  {
    this.m_isFriendInviteFeatureEnabled = isEnabled;
    Log.Privacy.PrintDebug("BnetFriendMgr SetFriendInviteFeatureStatus m_isFriendInviteFeatureEnabled " + string.Format(" {0}, m_isRegisteredToFriendHandler {1}", (object) this.m_isFriendInviteFeatureEnabled, (object) this.m_isRegisteredToFriendHandler));
    if (!this.m_isRegisteredToFriendHandler)
      this.RegisterFriendHandler();
    if (this.m_isFriendInviteFeatureEnabled)
    {
      BnetFriendChangelist changelist = new BnetFriendChangelist();
      foreach (BnetInvitation receivedInvite in this.m_receivedInvites)
        changelist.AddAddedReceivedInvite(receivedInvite);
      if (changelist.IsEmpty())
        return;
      this.FireChangeEvent(changelist);
    }
    else
    {
      BnetFriendChangelist changelist = new BnetFriendChangelist();
      foreach (BnetInvitation receivedInvite in this.m_receivedInvites)
        changelist.AddRemovedReceivedInvite(receivedInvite);
      if (changelist.IsEmpty())
        return;
      this.FireChangeEvent(changelist);
    }
  }

  private void RegisterFriendHandler()
  {
    if (this.m_isRegisteredToFriendHandler)
      return;
    Log.Privacy.PrintDebug("BnetFriendMgr RegisterFriendHandler");
    this.m_isRegisteredToFriendHandler = true;
    Network.Get().SetFriendsHandler(new Network.FriendsHandler(this.OnFriendsUpdate));
    Network.Get().AddBnetErrorListener(BnetFeature.Friends, new Network.BnetErrorCallback(this.OnBnetError));
  }

  public BnetPlayer FindFriend(BnetAccountId id) => this.FindNonPendingFriend(id) ?? this.FindPendingFriend(id) ?? (BnetPlayer) null;

  public bool IsFriend(BnetPlayer player) => this.IsNonPendingFriend(player) || this.IsPendingFriend(player);

  public bool IsFriend(BnetAccountId id) => this.IsNonPendingFriend(id) || this.IsPendingFriend(id);

  public bool IsFriend(BnetGameAccountId id) => this.IsNonPendingFriend(id) || this.IsPendingFriend(id);

  public List<BnetPlayer> GetFriends() => this.m_friends;

  public bool HasOnlineFriends()
  {
    foreach (BnetPlayer friend in this.m_friends)
    {
      if (friend.IsOnline())
        return true;
    }
    return false;
  }

  public int GetOnlineFriendCount()
  {
    int onlineFriendCount = 0;
    foreach (BnetPlayer friend in this.m_friends)
    {
      if (friend.IsOnline() || friend.IsAway())
        ++onlineFriendCount;
    }
    return onlineFriendCount;
  }

  public BnetPlayer FindNonPendingFriend(BnetAccountId id)
  {
    foreach (BnetPlayer friend in this.m_friends)
    {
      if ((BnetEntityId) friend.GetAccountId() == (BnetEntityId) id)
        return friend;
    }
    return (BnetPlayer) null;
  }

  public BnetPlayer FindNonPendingFriend(BnetGameAccountId id)
  {
    foreach (BnetPlayer friend in this.m_friends)
    {
      if (friend.HasGameAccount(id))
        return friend;
    }
    return (BnetPlayer) null;
  }

  public bool IsNonPendingFriend(BnetPlayer player)
  {
    if (player == null)
      return false;
    if (this.m_friends.Contains(player))
      return true;
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

  public bool IsNonPendingFriend(BnetAccountId id) => this.FindNonPendingFriend(id) != null;

  public bool IsNonPendingFriend(BnetGameAccountId id) => this.FindNonPendingFriend(id) != null;

  public BnetPlayer FindPendingFriend(BnetAccountId id) => this.m_pendingChangelist.FindFriend(id);

  public bool IsPendingFriend(BnetPlayer player) => this.m_pendingChangelist.IsFriend(player);

  public bool IsPendingFriend(BnetAccountId id) => this.m_pendingChangelist.IsFriend(id);

  public bool IsPendingFriend(BnetGameAccountId id) => this.m_pendingChangelist.IsFriend(id);

  public List<BnetInvitation> GetReceivedInvites() => this.m_isFriendInviteFeatureEnabled ? this.m_receivedInvites : (List<BnetInvitation>) null;

  public void AcceptInvite(BnetInvitation invite)
  {
    Network.AcceptFriendInvite(invite.GetId());
    BnetRecentPlayerMgr.Get().AddPendingFriend(invite.GetInviterId());
  }

  public void IgnoreInvite(BnetInvitationId inviteId) => Network.IgnoreFriendInvite(inviteId);

  public bool SendInvite(string name)
  {
    Log.Privacy.PrintDebug(string.Format("BnetFriendMgr m_isFriendInviteFeatureEnabled {0}", (object) this.m_isFriendInviteFeatureEnabled));
    if (!this.m_isFriendInviteFeatureEnabled)
      return true;
    if (name.Contains("@"))
      return this.SendInviteByEmail(name);
    return name.Contains("#") && this.SendInviteByBattleTag(name);
  }

  public bool SendInviteByEmail(string email)
  {
    if (!new Regex("^\\S[^@]+@[A-Za-z0-9-]+(\\.[A-Za-z0-9-]+)+$").IsMatch(email))
      return false;
    Network.SendFriendInviteByEmail(BnetPresenceMgr.Get().GetMyPlayer().GetFullName(), email);
    return true;
  }

  public bool SendInviteByBattleTag(string battleTagString)
  {
    if (!new Regex("^[^\\W\\d_][^\\W_]{1,11}#\\d+$").IsMatch(battleTagString))
      return false;
    Network.SendFriendInviteByBattleTag(BnetPresenceMgr.Get().GetMyPlayer().GetBattleTag().GetString(), battleTagString);
    BnetRecentPlayerMgr.Get().AddPendingFriend(battleTagString);
    return true;
  }

  public bool RemoveFriend(BnetPlayer friend)
  {
    bool flag = false;
    for (int index = 0; index < this.m_friends.Count; ++index)
    {
      if (this.m_friends[index].GetAccountId().Equals((BnetEntityId) friend.GetAccountId()))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
      return false;
    Network.RemoveFriend(friend.GetAccountId());
    return true;
  }

  public bool AddChangeListener(BnetFriendMgr.ChangeCallback callback) => this.AddChangeListener(callback, (object) null);

  public bool AddChangeListener(BnetFriendMgr.ChangeCallback callback, object userData)
  {
    BnetFriendMgr.ChangeListener changeListener = new BnetFriendMgr.ChangeListener();
    changeListener.SetCallback(callback);
    changeListener.SetUserData(userData);
    if (this.m_changeListeners.Contains(changeListener))
      return false;
    this.m_changeListeners.Add(changeListener);
    return true;
  }

  public bool RemoveChangeListener(BnetFriendMgr.ChangeCallback callback) => this.RemoveChangeListener(callback, (object) null);

  private bool RemoveChangeListener(BnetFriendMgr.ChangeCallback callback, object userData)
  {
    BnetFriendMgr.ChangeListener changeListener = new BnetFriendMgr.ChangeListener();
    changeListener.SetCallback(callback);
    changeListener.SetUserData(userData);
    return this.m_changeListeners.Remove(changeListener);
  }

  public static bool RemoveChangeListenerFromInstance(
    BnetFriendMgr.ChangeCallback callback,
    object userData = null)
  {
    return BnetFriendMgr.s_instance != null && BnetFriendMgr.s_instance.RemoveChangeListener(callback, userData);
  }

  private void InitMaximums()
  {
    FriendsInfo info = new FriendsInfo();
    BattleNet.GetFriendsInfo(ref info);
    this.m_maxFriends = info.maxFriends;
    this.m_maxReceivedInvites = info.maxRecvInvites;
    this.m_maxSentInvites = info.maxSentInvites;
  }

  private void ProcessPendingFriends()
  {
    bool flag = false;
    foreach (BnetPlayer friend in this.m_pendingChangelist.GetFriends())
    {
      if (friend.IsDisplayable())
      {
        flag = true;
        this.m_friends.Add(friend);
      }
    }
    if (!flag)
      return;
    this.FirePendingFriendsChangedEvent();
  }

  private void OnDisconnectedFromBattleNet(BattleNetErrors error) => this.Clear();

  private void OnFriendsUpdate(FriendsUpdate[] updates)
  {
    BnetFriendChangelist changelist = new BnetFriendChangelist();
    foreach (FriendsUpdate update in updates)
    {
      switch ((FriendsUpdate.Action) update.action)
      {
        case FriendsUpdate.Action.FRIEND_ADDED:
          BnetAccountId fromBnetEntityId1 = BnetAccountId.CreateFromBnetEntityId(update.entity1);
          BnetPlayer friend = BnetPresenceMgr.Get().RegisterPlayer(BnetPlayerSource.FRIENDLIST, fromBnetEntityId1);
          if (friend.IsDisplayable())
          {
            this.m_friends.Add(friend);
            changelist.AddAddedFriend(friend);
            break;
          }
          this.AddPendingFriend(friend);
          break;
        case FriendsUpdate.Action.FRIEND_REMOVED:
          BnetAccountId fromBnetEntityId2 = BnetAccountId.CreateFromBnetEntityId(update.entity1);
          BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(fromBnetEntityId2);
          this.m_friends.Remove(player);
          changelist.AddRemovedFriend(player);
          this.RemovePendingFriend(player);
          BnetPresenceMgr.Get().CheckSubscriptionsAndClearTransientStatus(fromBnetEntityId2);
          break;
        case FriendsUpdate.Action.FRIEND_INVITE:
          BnetInvitation fromFriendsUpdate1 = BnetInvitation.CreateFromFriendsUpdate(update);
          this.m_receivedInvites.Add(fromFriendsUpdate1);
          changelist.AddAddedReceivedInvite(fromFriendsUpdate1);
          break;
        case FriendsUpdate.Action.FRIEND_INVITE_REMOVED:
          BnetInvitation fromFriendsUpdate2 = BnetInvitation.CreateFromFriendsUpdate(update);
          this.m_receivedInvites.Remove(fromFriendsUpdate2);
          changelist.AddRemovedReceivedInvite(fromFriendsUpdate2);
          break;
        case FriendsUpdate.Action.FRIEND_SENT_INVITE:
          BnetInvitation fromFriendsUpdate3 = BnetInvitation.CreateFromFriendsUpdate(update);
          this.m_sentInvites.Add(fromFriendsUpdate3);
          changelist.AddAddedSentInvite(fromFriendsUpdate3);
          break;
        case FriendsUpdate.Action.FRIEND_SENT_INVITE_REMOVED:
          BnetInvitation fromFriendsUpdate4 = BnetInvitation.CreateFromFriendsUpdate(update);
          this.m_sentInvites.Remove(fromFriendsUpdate4);
          changelist.AddRemovedSentInvite(fromFriendsUpdate4);
          break;
      }
    }
    if (changelist.IsEmpty())
      return;
    if (this.m_isFriendInviteFeatureEnabled)
    {
      this.FireChangeEvent(changelist);
    }
    else
    {
      foreach (BnetInvitation receivedInvite in this.m_receivedInvites)
      {
        changelist.AddRemovedReceivedInvite(receivedInvite);
        changelist.RemoveAddedReceivedInvite(receivedInvite);
      }
      this.FireChangeEvent(changelist);
    }
  }

  private void OnPendingPlayersChanged(BnetPlayerChangelist changelist, object userData) => this.ProcessPendingFriends();

  private bool OnBnetError(BnetErrorInfo info, object userData) => true;

  private void Clear()
  {
    this.m_friends.Clear();
    this.m_receivedInvites.Clear();
    this.m_sentInvites.Clear();
    this.m_pendingChangelist.Clear();
    BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPendingPlayersChanged));
  }

  private void FireChangeEvent(BnetFriendChangelist changelist)
  {
    foreach (BnetFriendMgr.ChangeListener changeListener in this.m_changeListeners.ToArray())
      changeListener.Fire(changelist);
  }

  private void AddPendingFriend(BnetPlayer friend)
  {
    if (!this.m_pendingChangelist.Add(friend) || this.m_pendingChangelist.GetCount() != 1)
      return;
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPendingPlayersChanged));
  }

  private void RemovePendingFriend(BnetPlayer friend)
  {
    if (!this.m_pendingChangelist.Remove(friend))
      return;
    if (this.m_pendingChangelist.GetCount() == 0)
      BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPendingPlayersChanged));
    else
      this.ProcessPendingFriends();
  }

  private void FirePendingFriendsChangedEvent()
  {
    BnetFriendChangelist changelist = this.m_pendingChangelist.CreateChangelist();
    if (this.m_pendingChangelist.GetCount() == 0)
      BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPendingPlayersChanged));
    this.FireChangeEvent(changelist);
  }

  public BnetPlayer Cheat_CreatePlayer(
    string fullName,
    int leagueId,
    int starLevel,
    BnetProgramId programId,
    bool isFriend,
    bool isOnline,
    bool isAway = false)
  {
    BnetBattleTag battleTag = new BnetBattleTag();
    battleTag.SetString(string.Format("friend#{0}", (object) BnetFriendMgr.nextIdToken));
    BnetAccountId id1 = new BnetAccountId(BnetFriendMgr.nextIdToken++, BnetFriendMgr.nextIdToken++);
    BnetAccount account = new BnetAccount();
    account.SetId(id1);
    account.SetFullName(fullName);
    account.SetBattleTag(battleTag);
    BnetGameAccountId id2 = new BnetGameAccountId(BnetFriendMgr.nextIdToken++, BnetFriendMgr.nextIdToken++);
    BnetGameAccount gameAccount = new BnetGameAccount();
    gameAccount.SetId(id2);
    gameAccount.SetBattleTag(battleTag);
    gameAccount.SetOnline(isOnline);
    gameAccount.SetAway(isAway);
    gameAccount.SetProgramId(programId);
    GamePresenceRank protobuf = new GamePresenceRank();
    foreach (FormatType formatType in Enum.GetValues(typeof (FormatType)))
    {
      if (formatType != FormatType.FT_UNKNOWN)
      {
        GamePresenceRankData presenceRankData = new GamePresenceRankData()
        {
          FormatType = formatType,
          LeagueId = leagueId,
          StarLevel = starLevel,
          LegendRank = UnityEngine.Random.Range(1, 99999)
        };
        protobuf.Values.Add(presenceRankData);
      }
    }
    byte[] byteArray = ProtobufUtil.ToByteArray((IProtoBuf) protobuf);
    gameAccount.SetGameField(18U, (object) byteArray);
    BnetPlayer player = new BnetPlayer(BnetPlayerSource.CREATED_BY_CHEAT);
    player.SetAccount(account);
    player.AddGameAccount(gameAccount);
    player.IsCheatPlayer = true;
    if (isFriend)
      this.m_friends.Add(player);
    return player;
  }

  public BnetPlayer Cheat_CreateFriend(
    string fullName,
    int leagueId,
    int starLevel,
    BnetProgramId programId,
    bool isOnline,
    bool isAway)
  {
    return this.Cheat_CreatePlayer(fullName, leagueId, starLevel, programId, true, isOnline, isAway);
  }

  public int Cheat_RemoveCheatFriends()
  {
    int num = 0;
    for (int index = this.m_friends.Count - 1; index >= 0; --index)
    {
      if (this.m_friends[index].IsCheatPlayer)
      {
        this.m_friends.RemoveAt(index);
        ++num;
      }
    }
    return num;
  }

  public delegate void ChangeCallback(BnetFriendChangelist changelist, object userData);

  private class ChangeListener : EventListener<BnetFriendMgr.ChangeCallback>
  {
    public void Fire(BnetFriendChangelist changelist) => this.m_callback(changelist, this.m_userData);
  }
}
