using Blizzard.GameService.SDK.Client.Integration;
using Hearthstone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class BnetNearbyPlayerMgr
{
  private static BnetNearbyPlayerMgr s_instance;
  private bool m_enabled = true;
  private bool m_listening;
  private ulong m_myGameAccountLo;
  private string m_bnetVersion;
  private string m_bnetEnvironment;
  private string m_idString;
  private bool m_availability;
  private bool m_battlegroundsAvailability;
  private bool m_mercenariesAvailability;
  private BnetPartyId m_partyId = BnetPartyId.Empty;
  private UdpClient m_client;
  private int m_port;
  private float m_lastCallTime;
  private List<BnetNearbyPlayerMgr.NearbyPlayer> m_nearbyPlayers = new List<BnetNearbyPlayerMgr.NearbyPlayer>();
  private List<BnetPlayer> m_nearbyBnetPlayers = new List<BnetPlayer>();
  private List<BnetPlayer> m_nearbyFriends = new List<BnetPlayer>();
  private List<BnetPlayer> m_nearbyStrangers = new List<BnetPlayer>();
  private object m_mutex = new object();
  private object m_mutexClient = new object();
  private List<BnetNearbyPlayerMgr.NearbyPlayer> m_nearbyAdds = new List<BnetNearbyPlayerMgr.NearbyPlayer>();
  private List<BnetNearbyPlayerMgr.NearbyPlayer> m_nearbyUpdates = new List<BnetNearbyPlayerMgr.NearbyPlayer>();
  private List<BnetNearbyPlayerMgr.ChangeListener> m_changeListeners = new List<BnetNearbyPlayerMgr.ChangeListener>();
  private byte[] m_broadcastBuffer;
  private StringBuilder m_broadcastStringBuilder = new StringBuilder(128);
  private IPEndPoint m_broadcastEndpoint;
  private UdpClient m_broadcastSender;
  private bool m_isBroadcasting;
  private BnetRecentOrNearbyPlayerChangelist m_changelist = new BnetRecentOrNearbyPlayerChangelist();

  public static BnetNearbyPlayerMgr Get()
  {
    if (BnetNearbyPlayerMgr.s_instance == null)
    {
      BnetNearbyPlayerMgr.s_instance = new BnetNearbyPlayerMgr();
      HearthstoneApplication.Get().WillReset += new System.Action(BnetNearbyPlayerMgr.s_instance.Clear);
      FiresideGatheringManager.OnPatronListUpdated += new FiresideGatheringManager.OnPatronListUpdatedCallback(BnetNearbyPlayerMgr.NearbyPlayers_OnFSGPatronsUpdated);
    }
    return BnetNearbyPlayerMgr.s_instance;
  }

  public void Initialize()
  {
    this.m_bnetVersion = BattleNet.GetVersion();
    this.m_bnetEnvironment = BattleNet.GetEnvironment();
    this.UpdateEnabled();
    Options.Get().RegisterChangedListener(Option.NEARBY_PLAYERS, new Options.ChangedCallback(this.OnEnabledOptionChanged));
    BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    Network.Get().OnDisconnectedFromBattleNet += new System.Action<BattleNetErrors>(this.OnDisconnectedFromBattleNet);
  }

  public void Shutdown()
  {
    this.StopListening();
    Options.Get().UnregisterChangedListener(Option.NEARBY_PLAYERS, new Options.ChangedCallback(this.OnEnabledOptionChanged));
    BnetFriendMgr.Get().RemoveChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    Network.Get().OnDisconnectedFromBattleNet -= new System.Action<BattleNetErrors>(this.OnDisconnectedFromBattleNet);
  }

  public bool IsEnabled() => !TemporaryAccountManager.IsTemporaryAccount() && Options.Get().GetBool(Option.NEARBY_PLAYERS) && this.m_enabled;

  public void SetEnabled(bool enabled)
  {
    this.m_enabled = enabled;
    this.UpdateEnabled();
  }

  public bool GetNearbySessionStartTime(BnetPlayer bnetPlayer, out ulong sessionStartTime)
  {
    sessionStartTime = 0UL;
    if (bnetPlayer == null)
      return false;
    BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer = (BnetNearbyPlayerMgr.NearbyPlayer) null;
    lock (this.m_mutex)
      nearbyPlayer = this.m_nearbyPlayers.Find((Predicate<BnetNearbyPlayerMgr.NearbyPlayer>) (obj => (BnetEntityId) obj.m_bnetPlayer.GetAccountId() == (BnetEntityId) bnetPlayer.GetAccountId()));
    if (nearbyPlayer == null)
      return false;
    sessionStartTime = nearbyPlayer.m_sessionStartTime;
    return true;
  }

  public bool HasNearbyStrangers() => this.m_nearbyStrangers.Count > 0 && this.m_nearbyStrangers.Any<BnetPlayer>((Func<BnetPlayer, bool>) (p => p != null && p.IsOnline()));

  public List<BnetPlayer> GetNearbyPlayers() => this.m_nearbyBnetPlayers;

  public bool IsNearbyPlayer(BnetPlayer player) => this.FindNearbyPlayer(player) != null;

  public bool IsNearbyPlayer(BnetGameAccountId id) => this.FindNearbyPlayer(id) != null;

  public bool IsNearbyStranger(BnetPlayer player) => this.FindNearbyStranger(player) != null;

  public bool IsNearbyStranger(BnetGameAccountId id) => this.FindNearbyStranger(id) != null;

  public BnetPlayer FindNearbyPlayer(BnetPlayer player) => this.FindNearbyPlayer(player, this.m_nearbyBnetPlayers);

  public BnetPlayer FindNearbyPlayer(BnetGameAccountId id) => this.FindNearbyPlayer(id, this.m_nearbyBnetPlayers);

  public BnetPlayer FindNearbyStranger(BnetPlayer player) => this.FindNearbyPlayer(player, this.m_nearbyStrangers);

  public BnetPlayer FindNearbyStranger(BnetGameAccountId id) => this.FindNearbyPlayer(id, this.m_nearbyStrangers);

  public BnetPlayer FindNearbyStranger(BnetAccountId id) => this.FindNearbyPlayer(id, this.m_nearbyStrangers);

  public void SetAvailability(bool av)
  {
    this.m_availability = av;
    this.CreateBroadcastString();
  }

  public void SetBattlegroundsAvailability(bool av)
  {
    this.m_battlegroundsAvailability = av;
    this.CreateBroadcastString();
  }

  public void SetMercenariesAvailability(bool av)
  {
    this.m_mercenariesAvailability = av;
    this.CreateBroadcastString();
  }

  public void SetPartyId(BnetPartyId partyId)
  {
    BnetPartyId bnetPartyId = partyId;
    if ((object) bnetPartyId == null)
      bnetPartyId = BnetPartyId.Empty;
    this.m_partyId = bnetPartyId;
    this.CreateBroadcastString();
  }

  public bool AddChangeListener(BnetNearbyPlayerMgr.ChangeCallback callback) => this.AddChangeListener(callback, (object) null);

  public bool AddChangeListener(BnetNearbyPlayerMgr.ChangeCallback callback, object userData)
  {
    BnetNearbyPlayerMgr.ChangeListener changeListener = new BnetNearbyPlayerMgr.ChangeListener();
    changeListener.SetCallback(callback);
    changeListener.SetUserData(userData);
    if (this.m_changeListeners.Contains(changeListener))
      return false;
    this.m_changeListeners.Add(changeListener);
    return true;
  }

  public bool RemoveChangeListener(BnetNearbyPlayerMgr.ChangeCallback callback) => this.RemoveChangeListener(callback, (object) null);

  private bool RemoveChangeListener(BnetNearbyPlayerMgr.ChangeCallback callback, object userData)
  {
    BnetNearbyPlayerMgr.ChangeListener changeListener = new BnetNearbyPlayerMgr.ChangeListener();
    changeListener.SetCallback(callback);
    changeListener.SetUserData(userData);
    return this.m_changeListeners.Remove(changeListener);
  }

  public static bool RemoveChangeListenerFromInstance(
    BnetNearbyPlayerMgr.ChangeCallback callback,
    object userData = null)
  {
    return BnetNearbyPlayerMgr.s_instance != null && BnetNearbyPlayerMgr.s_instance.RemoveChangeListener(callback, userData);
  }

  private void BeginListening()
  {
    if (this.m_listening)
      return;
    this.m_listening = true;
    IPEndPoint localEP = new IPEndPoint(IPAddress.Any, 1228);
    UdpClient udpClient = new UdpClient();
    udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
    udpClient.Client.Bind((EndPoint) localEP);
    this.m_port = 1228;
    this.m_client = udpClient;
    this.m_broadcastSender = new UdpClient();
    this.m_broadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, this.m_port);
    BnetNearbyPlayerMgr.UdpState state = new BnetNearbyPlayerMgr.UdpState();
    state.e = localEP;
    state.u = this.m_client;
    this.m_lastCallTime = Time.realtimeSinceStartup;
    this.m_client.BeginReceive(new AsyncCallback(this.OnUdpReceive), (object) state);
  }

  private void OnUdpReceive(IAsyncResult ar)
  {
    lock (this.m_mutexClient)
    {
      if (!this.m_listening)
        return;
    }
    UdpClient u = ((BnetNearbyPlayerMgr.UdpState) ar.AsyncState).u;
    IPEndPoint e = ((BnetNearbyPlayerMgr.UdpState) ar.AsyncState).e;
    if (u == null || e == null)
      return;
    byte[] bytes = u.EndReceive(ar, ref e);
    u.BeginReceive(new AsyncCallback(this.OnUdpReceive), ar.AsyncState);
    string[] strArray1 = Encoding.UTF8.GetString(bytes).Split(',');
    ulong result1 = 0;
    ulong result2 = 0;
    ulong result3 = 0;
    ulong result4 = 0;
    ulong result5 = 0;
    ulong result6 = 0;
    ulong result7 = 0;
    int num1 = 0;
    if (num1 >= strArray1.Length)
      return;
    string[] strArray2 = strArray1;
    int index1 = num1;
    int num2 = index1 + 1;
    if (!ulong.TryParse(strArray2[index1], out result1) || num2 >= strArray1.Length)
      return;
    string[] strArray3 = strArray1;
    int index2 = num2;
    int num3 = index2 + 1;
    if (!ulong.TryParse(strArray3[index2], out result2) || num3 >= strArray1.Length)
      return;
    string[] strArray4 = strArray1;
    int index3 = num3;
    int num4 = index3 + 1;
    if (!ulong.TryParse(strArray4[index3], out result3) || num4 >= strArray1.Length)
      return;
    string[] strArray5 = strArray1;
    int index4 = num4;
    int num5 = index4 + 1;
    if (!ulong.TryParse(strArray5[index4], out result4) || (long) this.m_myGameAccountLo == (long) result4 || num5 >= strArray1.Length)
      return;
    string[] strArray6 = strArray1;
    int index5 = num5;
    int num6 = index5 + 1;
    string name = strArray6[index5];
    if (num6 >= strArray1.Length)
      return;
    string[] strArray7 = strArray1;
    int index6 = num6;
    int num7 = index6 + 1;
    string number = strArray7[index6];
    if (num7 >= strArray1.Length)
      return;
    string[] strArray8 = strArray1;
    int index7 = num7;
    int num8 = index7 + 1;
    string val1 = strArray8[index7];
    if (string.IsNullOrEmpty(val1) || val1 != this.m_bnetVersion || num8 >= strArray1.Length)
      return;
    string[] strArray9 = strArray1;
    int index8 = num8;
    int num9 = index8 + 1;
    string val2 = strArray9[index8];
    if (string.IsNullOrEmpty(val2) || val2 != this.m_bnetEnvironment || num9 >= strArray1.Length)
      return;
    string[] strArray10 = strArray1;
    int index9 = num9;
    int num10 = index9 + 1;
    string str1 = strArray10[index9];
    bool flag;
    if (str1 == "1")
    {
      flag = true;
    }
    else
    {
      if (!(str1 == "0"))
        return;
      flag = false;
    }
    if (num10 >= strArray1.Length)
      return;
    string[] strArray11 = strArray1;
    int index10 = num10;
    int num11 = index10 + 1;
    if (!ulong.TryParse(strArray11[index10], out result5) || num11 >= strArray1.Length)
      return;
    string[] strArray12 = strArray1;
    int index11 = num11;
    int num12 = index11 + 1;
    if (!ulong.TryParse(strArray12[index11], out result6) || num12 >= strArray1.Length)
      return;
    string[] strArray13 = strArray1;
    int index12 = num12;
    int num13 = index12 + 1;
    if (!ulong.TryParse(strArray13[index12], out result7) || num13 >= strArray1.Length)
      return;
    string[] strArray14 = strArray1;
    int index13 = num13;
    int num14 = index13 + 1;
    string str2 = strArray14[index13];
    bool battlegroundsAvailable;
    if (str2 == "1")
    {
      battlegroundsAvailable = true;
    }
    else
    {
      if (!(str2 == "0"))
        return;
      battlegroundsAvailable = false;
    }
    if (num14 >= strArray1.Length)
      return;
    string[] strArray15 = strArray1;
    int index14 = num14;
    int num15 = index14 + 1;
    string str3 = strArray15[index14];
    bool mercenariesAvailable;
    if (str3 == "1")
    {
      mercenariesAvailable = true;
    }
    else
    {
      if (!(str3 == "0"))
        return;
      mercenariesAvailable = false;
    }
    if (num15 >= strArray1.Length)
      return;
    string[] strArray16 = strArray1;
    int index15 = num15;
    int num16 = index15 + 1;
    string str4 = strArray16[index15];
    bool traditionalTutorialComplete;
    if (str4 == "1")
    {
      traditionalTutorialComplete = true;
    }
    else
    {
      if (!(str4 == "0"))
        return;
      traditionalTutorialComplete = false;
    }
    BnetBattleTag battleTag = new BnetBattleTag();
    battleTag.SetName(name);
    battleTag.SetNumber(number);
    BnetAccountId id1 = new BnetAccountId(result1, result2);
    BnetGameAccountId id2 = new BnetGameAccountId(result3, result4);
    BnetPartyId partyId = new BnetPartyId(result6, result7);
    BnetPlayer bnetPlayer = BnetPresenceMgr.Get().GetPlayer(id2);
    if (bnetPlayer == null)
    {
      BnetAccount account = new BnetAccount();
      account.SetId(id1);
      account.SetBattleTag(battleTag);
      account.SetAppearingOffline(false);
      BnetGameAccount gameAccount = new BnetGameAccount();
      gameAccount.SetId(id2);
      gameAccount.SetOwnerId(id1);
      gameAccount.SetBattleTag(battleTag);
      gameAccount.SetOnline(true);
      gameAccount.SetProgramId(BnetProgramId.HEARTHSTONE);
      gameAccount.SetGameField(1U, (object) flag);
      gameAccount.SetGameField(19U, (object) val1);
      gameAccount.SetGameField(20U, (object) val2);
      gameAccount.SetGameField(26U, (object) partyId.ToBnetEntityId());
      gameAccount.SetGameField(28U, (object) (battlegroundsAvailable ? 1 : 0));
      gameAccount.SetGameField(29U, (object) (mercenariesAvailable ? 1 : 0));
      gameAccount.SetGameField(15U, (object) (traditionalTutorialComplete ? 1 : 0));
      bnetPlayer = new BnetPlayer(BnetPlayerSource.NEARBY_PLAYER);
      bnetPlayer.SetAccount(account);
      bnetPlayer.AddGameAccount(gameAccount);
    }
    BnetNearbyPlayerMgr.NearbyPlayer other = new BnetNearbyPlayerMgr.NearbyPlayer();
    other.m_bnetPlayer = bnetPlayer;
    other.m_availability = flag;
    other.m_partyId = partyId;
    other.m_sessionStartTime = result5;
    lock (this.m_mutex)
    {
      if (!this.m_listening)
        return;
      foreach (BnetNearbyPlayerMgr.NearbyPlayer nearbyAdd in this.m_nearbyAdds)
      {
        if (nearbyAdd.Equals(other))
        {
          this.UpdateNearbyPlayer(nearbyAdd, flag, battlegroundsAvailable, mercenariesAvailable, traditionalTutorialComplete, result5, partyId);
          return;
        }
      }
      foreach (BnetNearbyPlayerMgr.NearbyPlayer nearbyUpdate in this.m_nearbyUpdates)
      {
        if (nearbyUpdate.Equals(other))
        {
          this.UpdateNearbyPlayer(nearbyUpdate, flag, battlegroundsAvailable, mercenariesAvailable, traditionalTutorialComplete, result5, partyId);
          return;
        }
      }
      foreach (BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer in this.m_nearbyPlayers)
      {
        if (nearbyPlayer.Equals(other))
        {
          this.UpdateNearbyPlayer(nearbyPlayer, flag, battlegroundsAvailable, mercenariesAvailable, traditionalTutorialComplete, result5, partyId);
          this.m_nearbyUpdates.Add(nearbyPlayer);
          return;
        }
      }
      this.m_nearbyAdds.Add(other);
    }
  }

  private void StopListening()
  {
    lock (this.m_mutexClient)
    {
      if (!this.m_listening)
        return;
      this.m_listening = false;
      this.m_client.Close();
    }
    BnetRecentOrNearbyPlayerChangelist changelist = new BnetRecentOrNearbyPlayerChangelist();
    lock (this.m_mutex)
    {
      foreach (BnetPlayer nearbyBnetPlayer in this.m_nearbyBnetPlayers)
        changelist.AddRemovedPlayer(nearbyBnetPlayer);
      foreach (BnetPlayer nearbyFriend in this.m_nearbyFriends)
        changelist.AddRemovedFriend(nearbyFriend);
      foreach (BnetPlayer nearbyStranger in this.m_nearbyStrangers)
        changelist.AddRemovedStranger(nearbyStranger);
      this.m_nearbyPlayers.Clear();
      this.m_nearbyBnetPlayers.Clear();
      this.m_nearbyFriends.Clear();
      this.m_nearbyStrangers.Clear();
      this.m_nearbyAdds.Clear();
      this.m_nearbyUpdates.Clear();
    }
    this.FireChangeEvent(changelist);
    this.m_broadcastSender.Close();
  }

  public void Update()
  {
    if (!this.m_listening)
      return;
    this.CacheMyAccountInfo();
    this.CheckIntervalAndBroadcast();
    this.ProcessPlayerChanges();
  }

  private void Clear()
  {
    lock (this.m_mutex)
    {
      this.m_nearbyPlayers.Clear();
      this.m_nearbyBnetPlayers.Clear();
      this.m_nearbyFriends.Clear();
      this.m_nearbyStrangers.Clear();
      this.m_nearbyAdds.Clear();
      this.m_nearbyUpdates.Clear();
    }
  }

  private void OnDisconnectedFromBattleNet(BattleNetErrors error) => this.Clear();

  private void UpdateEnabled()
  {
    bool flag = this.IsEnabled();
    if (flag == this.m_listening)
      return;
    if (flag)
      this.BeginListening();
    else
      this.StopListening();
  }

  private void FireChangeEvent(BnetRecentOrNearbyPlayerChangelist changelist)
  {
    if (changelist.IsEmpty())
      return;
    foreach (BnetNearbyPlayerMgr.ChangeListener changeListener in this.m_changeListeners.ToArray())
      changeListener.Fire(changelist);
  }

  private void CacheMyAccountInfo()
  {
    if (this.m_idString != null)
      return;
    BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
    if ((BnetEntityId) myGameAccountId == (BnetEntityId) null)
      return;
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    if (myPlayer == null)
      return;
    BnetAccountId accountId = myPlayer.GetAccountId();
    if ((BnetEntityId) accountId == (BnetEntityId) null)
      return;
    BnetBattleTag battleTag = myPlayer.GetBattleTag();
    if (battleTag == (BnetBattleTag) null)
      return;
    this.m_myGameAccountLo = myGameAccountId.Low;
    StringBuilder stringBuilder = new StringBuilder();
    stringBuilder.Append(accountId.High);
    stringBuilder.Append(',');
    stringBuilder.Append(accountId.Low);
    stringBuilder.Append(',');
    stringBuilder.Append(myGameAccountId.High);
    stringBuilder.Append(',');
    stringBuilder.Append(myGameAccountId.Low);
    stringBuilder.Append(',');
    stringBuilder.Append(battleTag.GetName());
    stringBuilder.Append(',');
    stringBuilder.Append(battleTag.GetNumber());
    stringBuilder.Append(',');
    stringBuilder.Append(BattleNet.GetVersion());
    stringBuilder.Append(',');
    stringBuilder.Append(BattleNet.GetEnvironment());
    this.m_idString = stringBuilder.ToString();
    this.CreateBroadcastString();
  }

  private void ProcessPlayerChanges()
  {
    this.m_changelist.Clear();
    lock (this.m_mutex)
    {
      this.ProcessAddedPlayers(this.m_changelist);
      this.ProcessUpdatedPlayers(this.m_changelist);
      this.RemoveInactivePlayers(this.m_changelist);
    }
    this.FireChangeEvent(this.m_changelist);
  }

  private void ProcessAddedPlayers(BnetRecentOrNearbyPlayerChangelist changelist)
  {
    if (this.m_nearbyAdds.Count == 0)
      return;
    for (int index = 0; index < this.m_nearbyAdds.Count; ++index)
    {
      BnetNearbyPlayerMgr.NearbyPlayer nearbyAdd = this.m_nearbyAdds[index];
      nearbyAdd.m_lastReceivedTime = Time.realtimeSinceStartup;
      BnetGameAccountId id = nearbyAdd.GetGameAccount().GetId();
      if (BnetPresenceMgr.Get().GetPlayer(id) == null)
        BnetPresenceMgr.Get().RegisterBnetPlayer(nearbyAdd.m_bnetPlayer);
      this.m_nearbyPlayers.Add(nearbyAdd);
      this.m_nearbyBnetPlayers.Add(nearbyAdd.m_bnetPlayer);
      changelist.AddAddedPlayer(nearbyAdd.m_bnetPlayer);
      if (nearbyAdd.IsFriend())
      {
        this.m_nearbyFriends.Add(nearbyAdd.m_bnetPlayer);
        changelist.AddAddedFriend(nearbyAdd.m_bnetPlayer);
      }
      else
      {
        this.m_nearbyStrangers.Add(nearbyAdd.m_bnetPlayer);
        changelist.AddAddedStranger(nearbyAdd.m_bnetPlayer);
      }
    }
    this.m_nearbyAdds.Clear();
  }

  private void ProcessUpdatedPlayers(BnetRecentOrNearbyPlayerChangelist changelist)
  {
    if (this.m_nearbyUpdates.Count == 0)
      return;
    for (int index = 0; index < this.m_nearbyUpdates.Count; ++index)
    {
      BnetNearbyPlayerMgr.NearbyPlayer nearbyUpdate = this.m_nearbyUpdates[index];
      nearbyUpdate.m_lastReceivedTime = Time.realtimeSinceStartup;
      changelist.AddUpdatedPlayer(nearbyUpdate.m_bnetPlayer);
      if (nearbyUpdate.IsFriend())
        changelist.AddUpdatedFriend(nearbyUpdate.m_bnetPlayer);
      else
        changelist.AddUpdatedStranger(nearbyUpdate.m_bnetPlayer);
    }
    this.m_nearbyUpdates.Clear();
  }

  private void RemoveInactivePlayers(BnetRecentOrNearbyPlayerChangelist changelist)
  {
    List<BnetNearbyPlayerMgr.NearbyPlayer> nearbyPlayerList = (List<BnetNearbyPlayerMgr.NearbyPlayer>) null;
    for (int index = 0; index < this.m_nearbyPlayers.Count; ++index)
    {
      BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer = this.m_nearbyPlayers[index];
      if ((double) Time.realtimeSinceStartup - (double) nearbyPlayer.m_lastReceivedTime >= 60.0)
      {
        if (nearbyPlayerList == null)
          nearbyPlayerList = new List<BnetNearbyPlayerMgr.NearbyPlayer>();
        nearbyPlayerList.Add(nearbyPlayer);
      }
    }
    if (nearbyPlayerList == null)
      return;
    foreach (BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer in nearbyPlayerList)
    {
      this.m_nearbyPlayers.Remove(nearbyPlayer);
      if (this.m_nearbyBnetPlayers.Remove(nearbyPlayer.m_bnetPlayer))
        changelist.AddRemovedPlayer(nearbyPlayer.m_bnetPlayer);
      if (this.m_nearbyFriends.Remove(nearbyPlayer.m_bnetPlayer))
        changelist.AddRemovedFriend(nearbyPlayer.m_bnetPlayer);
      if (this.m_nearbyStrangers.Remove(nearbyPlayer.m_bnetPlayer))
        changelist.AddRemovedStranger(nearbyPlayer.m_bnetPlayer);
    }
  }

  private bool CheckIntervalAndBroadcast()
  {
    if (!this.IsMyPlayerOnline() || (double) Time.realtimeSinceStartup - (double) this.m_lastCallTime < 12.0)
      return false;
    this.m_lastCallTime = Time.realtimeSinceStartup;
    this.Broadcast();
    return true;
  }

  private async void Broadcast()
  {
    if (this.m_isBroadcasting)
      return;
    this.m_isBroadcasting = true;
    try
    {
      this.m_broadcastSender.EnableBroadcast = true;
      int num = await this.m_broadcastSender.SendAsync(this.m_broadcastBuffer, this.m_broadcastBuffer.Length, this.m_broadcastEndpoint);
    }
    catch
    {
    }
    finally
    {
      this.m_isBroadcasting = false;
    }
  }

  private void CreateBroadcastString()
  {
    ulong sessionStartTime = HealthyGamingMgr.Get().GetSessionStartTime();
    BnetEntityId bnetEntityId = this.m_partyId.ToBnetEntityId();
    this.m_broadcastStringBuilder.Clear();
    this.m_broadcastStringBuilder.Append(this.m_idString);
    this.m_broadcastStringBuilder.Append(',');
    this.m_broadcastStringBuilder.Append(this.m_availability ? "1" : "0");
    this.m_broadcastStringBuilder.Append(',');
    this.m_broadcastStringBuilder.Append(sessionStartTime);
    this.m_broadcastStringBuilder.Append(',');
    this.m_broadcastStringBuilder.Append(bnetEntityId.High);
    this.m_broadcastStringBuilder.Append(',');
    this.m_broadcastStringBuilder.Append(bnetEntityId.Low);
    this.m_broadcastStringBuilder.Append(',');
    this.m_broadcastStringBuilder.Append(this.m_battlegroundsAvailability ? "1" : "0");
    this.m_broadcastStringBuilder.Append(',');
    this.m_broadcastStringBuilder.Append(this.m_mercenariesAvailability ? "1" : "0");
    this.m_broadcastStringBuilder.Append(',');
    this.m_broadcastStringBuilder.Append(GameUtils.IsTraditionalTutorialComplete() ? "1" : "0");
    this.m_broadcastBuffer = Encoding.UTF8.GetBytes(this.m_broadcastStringBuilder.ToString());
  }

  private int FindNearbyPlayerIndex(BnetPlayer bnetPlayer, List<BnetPlayer> bnetPlayers)
  {
    if (bnetPlayer == null)
      return -1;
    BnetAccountId accountId = bnetPlayer.GetAccountId();
    return (BnetEntityId) accountId != (BnetEntityId) null ? this.FindNearbyPlayerIndex(accountId, bnetPlayers) : this.FindNearbyPlayerIndex(bnetPlayer.GetHearthstoneGameAccountId(), bnetPlayers);
  }

  private int FindNearbyPlayerIndex(BnetGameAccountId id, List<BnetPlayer> bnetPlayers)
  {
    if ((BnetEntityId) id == (BnetEntityId) null)
      return -1;
    for (int index = 0; index < bnetPlayers.Count; ++index)
    {
      BnetPlayer bnetPlayer = bnetPlayers[index];
      if ((BnetEntityId) id == (BnetEntityId) bnetPlayer.GetHearthstoneGameAccountId())
        return index;
    }
    return -1;
  }

  private int FindNearbyPlayerIndex(BnetAccountId id, List<BnetPlayer> bnetPlayers)
  {
    if ((BnetEntityId) id == (BnetEntityId) null)
      return -1;
    for (int index = 0; index < bnetPlayers.Count; ++index)
    {
      BnetPlayer bnetPlayer = bnetPlayers[index];
      if ((BnetEntityId) id == (BnetEntityId) bnetPlayer.GetAccountId())
        return index;
    }
    return -1;
  }

  private BnetPlayer FindNearbyPlayer(BnetPlayer bnetPlayer, List<BnetPlayer> bnetPlayers)
  {
    if (bnetPlayer == null)
      return (BnetPlayer) null;
    BnetAccountId accountId = bnetPlayer.GetAccountId();
    return (BnetEntityId) accountId != (BnetEntityId) null ? this.FindNearbyPlayer(accountId, bnetPlayers) : this.FindNearbyPlayer(bnetPlayer.GetHearthstoneGameAccountId(), bnetPlayers);
  }

  private BnetPlayer FindNearbyPlayer(BnetGameAccountId id, List<BnetPlayer> bnetPlayers)
  {
    int nearbyPlayerIndex = this.FindNearbyPlayerIndex(id, bnetPlayers);
    return nearbyPlayerIndex < 0 ? (BnetPlayer) null : bnetPlayers[nearbyPlayerIndex];
  }

  private BnetPlayer FindNearbyPlayer(BnetAccountId id, List<BnetPlayer> bnetPlayers)
  {
    int nearbyPlayerIndex = this.FindNearbyPlayerIndex(id, bnetPlayers);
    return nearbyPlayerIndex < 0 ? (BnetPlayer) null : bnetPlayers[nearbyPlayerIndex];
  }

  private void UpdateNearbyPlayer(
    BnetNearbyPlayerMgr.NearbyPlayer player,
    bool available,
    bool battlegroundsAvailable,
    bool mercenariesAvailable,
    bool traditionalTutorialComplete,
    ulong sessionStartTime,
    BnetPartyId partyId)
  {
    BnetGameAccount gameAccount = player.GetGameAccount();
    int num = BnetPresenceMgr.Get().IsSubscribedToPlayer(gameAccount.GetId()) ? 1 : 0;
    BnetPlayer player1 = BnetPresenceMgr.Get().GetPlayer(gameAccount.GetId());
    if (num != 0 && player1 != null)
    {
      player.m_bnetPlayer = player1;
    }
    else
    {
      gameAccount.SetGameField(1U, (object) available);
      gameAccount.SetGameField(28U, (object) (battlegroundsAvailable ? 1 : 0));
      gameAccount.SetGameField(29U, (object) (mercenariesAvailable ? 1 : 0));
      gameAccount.SetGameField(15U, (object) (traditionalTutorialComplete ? 1 : 0));
      gameAccount.SetGameField(26U, (object) partyId.ToBnetEntityId());
    }
    player.m_sessionStartTime = sessionStartTime;
  }

  private bool IsMyPlayerOnline()
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    return myPlayer != null && myPlayer.IsOnline() && !myPlayer.IsAppearingOffline();
  }

  private void OnEnabledOptionChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    this.UpdateEnabled();
  }

  private void OnFriendsChanged(BnetFriendChangelist friendChangelist, object userData)
  {
    List<BnetPlayer> addedFriends = friendChangelist.GetAddedFriends();
    List<BnetPlayer> removedFriends = friendChangelist.GetRemovedFriends();
    int num = addedFriends == null ? 0 : (addedFriends.Count > 0 ? 1 : 0);
    bool flag = removedFriends != null && removedFriends.Count > 0;
    if (num == 0 && !flag)
      return;
    BnetRecentOrNearbyPlayerChangelist changelist = new BnetRecentOrNearbyPlayerChangelist();
    lock (this.m_mutex)
    {
      if (addedFriends != null)
      {
        foreach (BnetPlayer bnetPlayer in addedFriends)
        {
          int nearbyPlayerIndex = this.FindNearbyPlayerIndex(bnetPlayer, this.m_nearbyStrangers);
          if (nearbyPlayerIndex >= 0)
          {
            BnetPlayer nearbyStranger = this.m_nearbyStrangers[nearbyPlayerIndex];
            this.m_nearbyStrangers.RemoveAt(nearbyPlayerIndex);
            this.m_nearbyFriends.Add(nearbyStranger);
            changelist.AddAddedFriend(nearbyStranger);
            changelist.AddRemovedStranger(nearbyStranger);
          }
        }
      }
      if (removedFriends != null)
      {
        foreach (BnetPlayer bnetPlayer in removedFriends)
        {
          int nearbyPlayerIndex = this.FindNearbyPlayerIndex(bnetPlayer, this.m_nearbyFriends);
          if (nearbyPlayerIndex >= 0)
          {
            BnetPlayer nearbyFriend = this.m_nearbyFriends[nearbyPlayerIndex];
            this.m_nearbyFriends.RemoveAt(nearbyPlayerIndex);
            this.m_nearbyStrangers.Add(nearbyFriend);
            changelist.AddAddedStranger(nearbyFriend);
            changelist.AddRemovedFriend(nearbyFriend);
          }
        }
      }
    }
    this.FireChangeEvent(changelist);
  }

  private static void NearbyPlayers_OnFSGPatronsUpdated(
    List<BnetPlayer> addedPatrons,
    List<BnetPlayer> removedPatrons)
  {
    BnetRecentOrNearbyPlayerChangelist changelist = (BnetRecentOrNearbyPlayerChangelist) null;
    if (addedPatrons != null)
    {
      foreach (BnetPlayer addedPatron in addedPatrons)
      {
        if (BnetNearbyPlayerMgr.Get().IsNearbyPlayer(addedPatron))
        {
          if (changelist == null)
            changelist = new BnetRecentOrNearbyPlayerChangelist();
          changelist.AddRemovedPlayer(addedPatron);
        }
      }
    }
    if (removedPatrons != null)
    {
      foreach (BnetPlayer removedPatron in removedPatrons)
      {
        if (BnetNearbyPlayerMgr.Get().IsNearbyPlayer(removedPatron))
        {
          if (changelist == null)
            changelist = new BnetRecentOrNearbyPlayerChangelist();
          changelist.AddAddedPlayer(removedPatron);
        }
      }
    }
    if (changelist == null)
      return;
    BnetNearbyPlayerMgr.Get().FireChangeEvent(changelist);
  }

  public BnetPlayer Cheat_CreateNearbyPlayer(
    string fullName,
    int leagueId,
    int starLevel,
    BnetProgramId programId,
    bool isFriend,
    bool isOnline)
  {
    BnetPlayer player = BnetFriendMgr.Get().Cheat_CreatePlayer(fullName, leagueId, starLevel, programId, isFriend, isOnline);
    BnetRecentOrNearbyPlayerChangelist changelist = new BnetRecentOrNearbyPlayerChangelist();
    changelist.AddAddedPlayer(player);
    if (isFriend)
      changelist.AddAddedFriend(player);
    else
      changelist.AddAddedStranger(player);
    this.m_nearbyAdds.Add(new BnetNearbyPlayerMgr.NearbyPlayer()
    {
      m_bnetPlayer = player,
      m_availability = true,
      m_partyId = BnetPartyId.Empty
    });
    this.ProcessAddedPlayers(changelist);
    return player;
  }

  public int Cheat_RemoveCheatFriends()
  {
    int num = 0;
    BnetRecentOrNearbyPlayerChangelist changelist = new BnetRecentOrNearbyPlayerChangelist();
    for (int index = this.m_nearbyPlayers.Count - 1; index >= 0; --index)
    {
      BnetNearbyPlayerMgr.NearbyPlayer nearbyPlayer = this.m_nearbyPlayers[index];
      if (nearbyPlayer.m_bnetPlayer.IsCheatPlayer)
      {
        nearbyPlayer.m_lastReceivedTime = 0.0f;
        ++num;
      }
    }
    this.RemoveInactivePlayers(changelist);
    this.FireChangeEvent(changelist);
    return num;
  }

  public delegate void ChangeCallback(
    BnetRecentOrNearbyPlayerChangelist changelist,
    object userData);

  private class ChangeListener : EventListener<BnetNearbyPlayerMgr.ChangeCallback>
  {
    public void Fire(BnetRecentOrNearbyPlayerChangelist changelist) => this.m_callback(changelist, this.m_userData);
  }

  private class NearbyPlayer : IEquatable<BnetNearbyPlayerMgr.NearbyPlayer>
  {
    public float m_lastReceivedTime;
    public BnetPlayer m_bnetPlayer;
    public bool m_availability;
    public ulong m_sessionStartTime;
    public BnetPartyId m_partyId = BnetPartyId.Empty;

    public bool Equals(BnetNearbyPlayerMgr.NearbyPlayer other) => other != null && (BnetEntityId) this.GetGameAccountId() == (BnetEntityId) other.GetGameAccountId();

    public BnetAccountId GetAccountId() => this.m_bnetPlayer.GetAccountId();

    public BnetGameAccountId GetGameAccountId() => this.m_bnetPlayer.GetHearthstoneGameAccountId();

    public BnetGameAccount GetGameAccount() => this.m_bnetPlayer.GetHearthstoneGameAccount();

    public bool IsFriend()
    {
      BnetAccountId accountId = this.GetAccountId();
      return BnetFriendMgr.Get().IsFriend(accountId);
    }
  }

  private class UdpState
  {
    public UdpClient u;
    public IPEndPoint e;
  }
}
