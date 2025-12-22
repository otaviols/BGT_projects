using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Utils;
using Hearthstone;
using PegasusFSG;
using SpectatorProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class BnetPresenceMgr
{
  private static BnetPresenceMgr s_instance;
  private Map<BnetAccountId, BnetAccount> m_accounts = new Map<BnetAccountId, BnetAccount>();
  private Map<BnetGameAccountId, BnetGameAccount> m_gameAccounts = new Map<BnetGameAccountId, BnetGameAccount>();
  private Map<BnetAccountId, BnetPlayer> m_players = new Map<BnetAccountId, BnetPlayer>();
  private BnetAccountId m_myBattleNetAccountId;
  private BnetGameAccountId m_myGameAccountId;
  private BnetPlayer m_myPlayer;
  private List<BnetPresenceMgr.PlayersChangedListener> m_playersChangedListeners = new List<BnetPresenceMgr.PlayersChangedListener>();

  public event System.Action<PresenceUpdate[]> OnGameAccountPresenceChange;

  public static BnetPresenceMgr Get()
  {
    if (BnetPresenceMgr.s_instance == null)
    {
      BnetPresenceMgr.s_instance = new BnetPresenceMgr();
      HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
      if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null)
        hearthstoneApplication.WillReset += (System.Action) (() =>
        {
          BnetPresenceMgr instance = BnetPresenceMgr.s_instance;
          BnetPresenceMgr.s_instance = new BnetPresenceMgr();
          BnetPresenceMgr.s_instance.m_playersChangedListeners = instance.m_playersChangedListeners;
          BnetPresenceMgr.s_instance.OnGameAccountPresenceChange = instance.OnGameAccountPresenceChange;
        });
      else
        Log.BattleNet.PrintWarning("BnetPresenceMgr.Get(): HearthstoneApplication.Get() returned null. Unable to subscribe to HearthstoneApplication.WillReset.");
    }
    return BnetPresenceMgr.s_instance;
  }

  public void Initialize()
  {
    Network.Get().SetPresenceHandler(new Network.PresenceHandler(this.OnPresenceUpdate));
    Network.Get().OnDisconnectedFromBattleNet += new System.Action<BattleNetErrors>(this.OnDisconnectedFromBattleNet);
    this.m_myGameAccountId = BattleNet.GetMyGameAccountId();
    this.m_myBattleNetAccountId = BattleNet.GetMyAccoundId();
  }

  public BnetAccountId GetMyAccountId() => this.m_myBattleNetAccountId;

  public BnetGameAccountId GetMyGameAccountId() => this.m_myGameAccountId;

  public BnetPlayer GetMyPlayer() => this.m_myPlayer;

  public BnetAccount GetAccount(BnetAccountId id)
  {
    if ((BnetEntityId) id == (BnetEntityId) null)
      return (BnetAccount) null;
    BnetAccount account = (BnetAccount) null;
    this.m_accounts.TryGetValue(id, out account);
    return account;
  }

  public BnetGameAccount GetGameAccount(BnetGameAccountId id)
  {
    if ((BnetEntityId) id == (BnetEntityId) null)
      return (BnetGameAccount) null;
    BnetGameAccount gameAccount = (BnetGameAccount) null;
    this.m_gameAccounts.TryGetValue(id, out gameAccount);
    return gameAccount;
  }

  public BnetPlayer GetPlayer(BnetAccountId id)
  {
    if ((BnetEntityId) id == (BnetEntityId) null)
      return (BnetPlayer) null;
    BnetPlayer player = (BnetPlayer) null;
    this.m_players.TryGetValue(id, out player);
    return player;
  }

  public BnetPlayer GetPlayer(BnetGameAccountId id)
  {
    BnetGameAccount gameAccount = this.GetGameAccount(id);
    return gameAccount == (BnetGameAccount) null ? (BnetPlayer) null : this.GetPlayer(gameAccount.GetOwnerId());
  }

  public BnetPlayer RegisterPlayer(
    BnetPlayerSource source,
    BnetAccountId accountId,
    BnetGameAccountId gameAccountId = null,
    BnetProgramId programId = null)
  {
    BnetPlayer player1 = this.GetPlayer(accountId);
    if (player1 != null)
      return player1;
    BnetPlayer player2 = new BnetPlayer(source);
    player2.SetAccountId(accountId);
    this.m_players[accountId] = player2;
    BnetAccount account = new BnetAccount();
    this.m_accounts.Add(accountId, account);
    account.SetId(accountId);
    player2.SetAccount(account);
    if ((BnetEntityId) gameAccountId != (BnetEntityId) null)
    {
      BnetGameAccount gameAccount;
      if (!this.m_gameAccounts.TryGetValue(gameAccountId, out gameAccount))
      {
        gameAccount = new BnetGameAccount();
        gameAccount.SetId(gameAccountId);
        gameAccount.SetOwnerId(accountId);
        this.m_gameAccounts.Add(gameAccountId, gameAccount);
        if ((Blizzard.GameService.SDK.Client.Integration.FourCC) programId != (Blizzard.GameService.SDK.Client.Integration.FourCC) null)
          gameAccount.SetProgramId(programId);
      }
      player2.AddGameAccount(gameAccount);
    }
    BnetPlayerChange change = new BnetPlayerChange();
    change.SetNewPlayer(player2);
    BnetPlayerChangelist changelist = new BnetPlayerChangelist();
    changelist.AddChange(change);
    this.FirePlayersChangedEvent(changelist);
    return player2;
  }

  public void RegisterBnetPlayer(BnetPlayer player)
  {
    if (player == null || player.GetAccount() == (BnetAccount) null || (BnetEntityId) player.GetAccountId() == (BnetEntityId) null)
      return;
    bool flag = false;
    BnetAccountId accountId = player.GetAccountId();
    BnetPlayer bnetPlayer;
    if (this.m_players.TryGetValue(accountId, out bnetPlayer))
    {
      if (bnetPlayer != player)
      {
        flag = true;
        Log.All.PrintWarning("Already registered BnetPlayer accountId={0} newSrc={1} - will overwrite.", (object) accountId.Low, (object) player.Source);
      }
    }
    else
      flag = true;
    this.m_players[accountId] = player;
    BnetAccount bnetAccount;
    if (this.m_accounts.TryGetValue(accountId, out bnetAccount))
    {
      if ((object) bnetAccount != (object) player.GetAccount())
      {
        flag = true;
        Log.All.PrintWarning("Already registered BnetAccount accountId={0} newSrc={1} - will overwrite.", (object) accountId.Low, (object) player.Source);
      }
    }
    else
      flag = true;
    this.m_accounts[accountId] = player.GetAccount();
    foreach (KeyValuePair<BnetGameAccountId, BnetGameAccount> gameAccount in player.GetGameAccounts())
    {
      BnetGameAccountId key = gameAccount.Key;
      BnetGameAccount bnetGameAccount1 = gameAccount.Value;
      BnetGameAccount bnetGameAccount2;
      if (this.m_gameAccounts.TryGetValue(key, out bnetGameAccount2))
      {
        if ((object) bnetGameAccount2 != (object) bnetGameAccount1)
        {
          flag = true;
          Log.All.PrintWarning("Already registered BnetAccount accountId={0} newSrc={1} - will overwrite.", (object) accountId.Low, (object) player.Source);
        }
      }
      else
        flag = true;
      this.m_gameAccounts[key] = bnetGameAccount1;
    }
    if (!flag)
      return;
    BnetPlayerChange change = new BnetPlayerChange();
    change.SetNewPlayer(player);
    BnetPlayerChangelist changelist = new BnetPlayerChangelist();
    changelist.AddChange(change);
    this.FirePlayersChangedEvent(changelist);
  }

  public bool IsSubscribedToPlayer(BnetGameAccountId id) => BattleNet.IsSubscribedToEntity((BnetEntityId) id);

  public void CheckSubscriptionsAndClearTransientStatus(BnetAccountId accountId)
  {
    BnetPlayer bnetPlayer;
    if (!this.m_players.TryGetValue(accountId, out bnetPlayer))
      return;
    foreach (KeyValuePair<BnetGameAccountId, BnetGameAccount> gameAccount in bnetPlayer.GetGameAccounts())
      this.CheckSubscriptionsAndClearTransientStatus_Internal(gameAccount.Value);
  }

  public void CheckSubscriptionsAndClearTransientStatus(BnetGameAccountId gameAccountId)
  {
    BnetGameAccount gameAccount;
    if (!this.m_gameAccounts.TryGetValue(gameAccountId, out gameAccount))
      return;
    this.CheckSubscriptionsAndClearTransientStatus_Internal(gameAccount);
  }

  private void CheckSubscriptionsAndClearTransientStatus_Internal(BnetGameAccount gameAccount)
  {
    if (this.IsSubscribedToPlayer(gameAccount.GetId()))
      return;
    this.ClearTransientStatus(gameAccount);
    gameAccount.SetOnline(BnetNearbyPlayerMgr.Get().IsNearbyPlayer(gameAccount.GetId()));
    gameAccount.SetBusy(false);
    gameAccount.SetAway(false);
    gameAccount.SetAwayTimeMicrosec(0L);
    gameAccount.SetRichPresence((string) null);
  }

  private void ClearTransientStatus(BnetGameAccount gameAccount)
  {
    foreach (uint transientStatusField in GamePresenceField.TransientStatusFields)
      gameAccount.SetGameField(transientStatusField, (object) null);
  }

  public static void RequestPlayerBattleTag(BnetAccountId id)
  {
    PresenceFieldKey[] array = new List<PresenceFieldKey>()
    {
      new PresenceFieldKey()
      {
        programId = BnetProgramId.BNET.GetValue(),
        groupId = 1U,
        fieldId = 4U,
        uniqueId = 0UL
      }
    }.ToArray();
    BattleNet.RequestPresenceFields(false, (BnetEntityId) id, array);
    Log.Presence.Print("Requesting BattleTag for player {0}!", (object) id);
  }

  public bool SetGameField(uint fieldId, bool val)
  {
    if (!Network.ShouldBeConnectedToAurora())
    {
      Error.AddDevFatal("Caller should check for Battle.net connection before calling SetGameField {0}={1}", (object) fieldId, (object) val);
      return false;
    }
    BnetGameAccount hsGameAccount;
    if (!this.ShouldUpdateGameField(fieldId, (object) val, out hsGameAccount))
      return false;
    if (fieldId == 2U)
    {
      hsGameAccount.SetBusy(val);
      int val1 = val ? 1 : 0;
      BattleNet.SetPresenceInt(fieldId, (long) val1);
    }
    else
      BattleNet.SetPresenceBool(fieldId, val);
    BnetPlayerChangelist changelist = this.ChangeGameField(hsGameAccount, fieldId, (object) val);
    switch (fieldId)
    {
      case 2:
        if (val)
        {
          hsGameAccount.SetAway(false);
          break;
        }
        break;
      case 10:
        if (val)
        {
          hsGameAccount.SetBusy(false);
          break;
        }
        break;
    }
    this.FirePlayersChangedEvent(changelist);
    return true;
  }

  public bool SetAccountField(uint fieldId, bool val)
  {
    if (!Network.ShouldBeConnectedToAurora())
    {
      Error.AddDevFatal("Caller should check for Battle.net connection before calling SetGameField {0}={1}", (object) fieldId, (object) val);
      return false;
    }
    BattleNet.SetAccountLevelPresenceBool(fieldId, val);
    BnetAccountId myAccoundId = BattleNet.GetMyAccoundId();
    this.OnPresenceUpdate(new PresenceUpdate[1]
    {
      new PresenceUpdate()
      {
        entityId = (BnetEntityId) myAccoundId,
        programId = BnetProgramId.BNET.GetValue(),
        groupId = 1U,
        fieldId = fieldId,
        boolVal = val
      }
    });
    return true;
  }

  public bool SetGameField(uint fieldId, int val)
  {
    if (!Network.ShouldBeConnectedToAurora())
    {
      Error.AddDevFatal("Caller should check for Battle.net connection before calling SetGameField {0}={1}", (object) fieldId, (object) val);
      return false;
    }
    BnetGameAccount hsGameAccount;
    if (!this.ShouldUpdateGameField(fieldId, (object) val, out hsGameAccount))
      return false;
    BattleNet.SetPresenceInt(fieldId, (long) val);
    this.FirePlayersChangedEvent(this.ChangeGameField(hsGameAccount, fieldId, (object) val));
    return true;
  }

  public bool SetGameField(uint fieldId, string val)
  {
    if (!Network.ShouldBeConnectedToAurora())
    {
      Error.AddDevFatal("Caller should check for Battle.net connection before calling SetGameField {0}={1}", (object) fieldId, (object) val);
      return false;
    }
    BnetGameAccount hsGameAccount;
    if (!this.ShouldUpdateGameField(fieldId, (object) val, out hsGameAccount))
      return false;
    BattleNet.SetPresenceString(fieldId, val);
    this.FirePlayersChangedEvent(this.ChangeGameField(hsGameAccount, fieldId, (object) val));
    return true;
  }

  public bool SetGameField(uint fieldId, byte[] val)
  {
    if (!Network.ShouldBeConnectedToAurora())
    {
      Error.AddDevFatal("Caller should check for Battle.net connection before calling SetGameField {0}=[{1}]", (object) fieldId, val == null ? (object) "" : (object) val.Length.ToString());
      return false;
    }
    BnetGameAccount hsGameAccount;
    if (!this.ShouldUpdateGameFieldBlob(fieldId, val, out hsGameAccount))
      return false;
    BattleNet.SetPresenceBlob(fieldId, val);
    this.FirePlayersChangedEvent(this.ChangeGameField(hsGameAccount, fieldId, (object) val));
    return true;
  }

  public bool SetGameFieldBlob(uint fieldId, IProtoBuf protoMessage)
  {
    if (fieldId == 21U || fieldId == 23U)
    {
      this.SetPresenceSpectatorJoinInfo(protoMessage as JoinInfo);
      return true;
    }
    byte[] val = protoMessage == null ? (byte[]) null : ProtobufUtil.ToByteArray(protoMessage);
    return this.SetGameField(fieldId, val);
  }

  public bool SetGameField(uint fieldId, BnetEntityId val)
  {
    if (!Network.ShouldBeConnectedToAurora())
    {
      Error.AddDevFatal("Caller should check for Battle.net connection before calling SetGameField {0}=[{1}]", (object) fieldId, val == (BnetEntityId) null ? (object) "" : (object) val.ToString());
      return false;
    }
    BnetGameAccount hsGameAccount;
    if (!this.ShouldUpdateGameField(fieldId, (object) val, out hsGameAccount))
      return false;
    BattleNet.SetPresenceEntityId(fieldId, val);
    this.FirePlayersChangedEvent(this.ChangeGameField(hsGameAccount, fieldId, (object) val));
    return true;
  }

  public void SetPresenceSpectatorJoinInfo(JoinInfo joinInfo)
  {
    byte[] numArray = joinInfo == null ? (byte[]) null : ProtobufUtil.ToByteArray((IProtoBuf) joinInfo);
    this.SetGameField(21U, numArray);
    byte[] val = (byte[]) null;
    if (joinInfo != null && FiresideGatheringManager.Get().IsCheckedIn && FiresideGatheringManager.Get().CurrentFsgSharedSecretKey != null)
    {
      byte[] fsgSharedSecretKey = FiresideGatheringManager.Get().CurrentFsgSharedSecretKey;
      byte[] hash = SHA256.Create().ComputeHash(fsgSharedSecretKey, 0, fsgSharedSecretKey.Length);
      val = ProtobufUtil.ToByteArray((IProtoBuf) new SecretJoinInfo()
      {
        Source = SecretSource.SECRET_SOURCE_FIRESIDE_GATHERING,
        SpecificSourceIdentity = FiresideGatheringManager.Get().CurrentFsgId,
        EncryptedMessage = Crypto.Rijndael.Encrypt(numArray, hash)
      });
    }
    this.SetGameField(23U, val);
  }

  public void SetDeckValidity(DeckValidity deckValidity) => this.SetGameFieldBlob(24U, (IProtoBuf) deckValidity);

  public bool AddPlayersChangedListener(BnetPresenceMgr.PlayersChangedCallback callback) => this.AddPlayersChangedListener(callback, (object) null);

  public bool AddPlayersChangedListener(
    BnetPresenceMgr.PlayersChangedCallback callback,
    object userData)
  {
    BnetPresenceMgr.PlayersChangedListener playersChangedListener = new BnetPresenceMgr.PlayersChangedListener();
    playersChangedListener.SetCallback(callback);
    playersChangedListener.SetUserData(userData);
    if (this.m_playersChangedListeners.Contains(playersChangedListener))
      return false;
    this.m_playersChangedListeners.Add(playersChangedListener);
    return true;
  }

  public bool RemovePlayersChangedListener(BnetPresenceMgr.PlayersChangedCallback callback) => this.RemovePlayersChangedListener(callback, (object) null);

  private bool RemovePlayersChangedListener(
    BnetPresenceMgr.PlayersChangedCallback callback,
    object userData)
  {
    BnetPresenceMgr.PlayersChangedListener playersChangedListener = new BnetPresenceMgr.PlayersChangedListener();
    playersChangedListener.SetCallback(callback);
    playersChangedListener.SetUserData(userData);
    return this.m_playersChangedListeners.Remove(playersChangedListener);
  }

  public static bool RemovePlayersChangedListenerFromInstance(
    BnetPresenceMgr.PlayersChangedCallback callback,
    object userData = null)
  {
    return BnetPresenceMgr.s_instance != null && BnetPresenceMgr.s_instance.RemovePlayersChangedListener(callback, userData);
  }

  private void OnPresenceUpdate(PresenceUpdate[] updates)
  {
    BnetPlayerChangelist changelist1 = new BnetPlayerChangelist();
    foreach (PresenceUpdate update1 in ((IEnumerable<PresenceUpdate>) updates).Where<PresenceUpdate>((Func<PresenceUpdate, bool>) (u => u.programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.BNET && u.groupId == 2U && u.fieldId == 7U)))
    {
      BnetGameAccountId fromBnetEntityId1 = BnetGameAccountId.CreateFromBnetEntityId(update1.entityId);
      BnetAccountId fromBnetEntityId2 = BnetAccountId.CreateFromBnetEntityId(update1.entityIdVal);
      if (!fromBnetEntityId2.IsEmpty())
      {
        if (this.GetAccount(fromBnetEntityId2) == (BnetAccount) null)
        {
          PresenceUpdate update2 = new PresenceUpdate();
          BnetPlayerChangelist changelist2 = new BnetPlayerChangelist();
          this.CreateAccount(fromBnetEntityId2, update2, changelist2);
        }
        if (!fromBnetEntityId1.IsEmpty() && this.GetGameAccount(fromBnetEntityId1) == (BnetGameAccount) null)
          this.CreateGameAccount(fromBnetEntityId1, update1, changelist1);
      }
    }
    List<PresenceUpdate> presenceUpdateList = (List<PresenceUpdate>) null;
    foreach (PresenceUpdate update in updates)
    {
      if (update.programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.BNET)
      {
        if (update.groupId == 1U)
          this.OnAccountUpdate(update, changelist1);
        else if (update.groupId == 2U)
          this.OnGameAccountUpdate(update, changelist1);
      }
      else if (update.programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE)
        this.OnGameUpdate(update, changelist1);
      if ((update.programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE || update.programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.BNET && update.groupId == 2U) && this.OnGameAccountPresenceChange != null)
      {
        if (presenceUpdateList == null)
          presenceUpdateList = new List<PresenceUpdate>();
        presenceUpdateList.Add(update);
      }
    }
    BnetPresenceMgr.LogPresenceUpdates(updates);
    if (presenceUpdateList != null)
      this.OnGameAccountPresenceChange(presenceUpdateList.ToArray());
    this.FirePlayersChangedEvent(changelist1);
  }

  private static void LogPresenceUpdates(PresenceUpdate[] updates)
  {
    Blizzard.T5.Logging.LogLevel level = Blizzard.T5.Logging.LogLevel.Debug;
    bool flag = true;
    StringBuilder buffer = (StringBuilder) null;
    foreach (PresenceUpdate update in updates)
      BnetPresenceMgr.LogPresenceUpdate(ref buffer, level, flag, update);
    if (buffer == null)
      return;
    Log.Presence.Print(level, flag, buffer.ToString());
  }

  private static void LogPresenceUpdate(
    ref StringBuilder buffer,
    Blizzard.T5.Logging.LogLevel level,
    bool verbosity,
    PresenceUpdate update)
  {
    if (HearthstoneApplication.IsPublic() || !Log.Presence.CanPrint(level, new bool?(verbosity)))
      return;
    BnetAccountId fromBnetEntityId = BnetAccountId.CreateFromBnetEntityId(update.entityId);
    BnetGameAccountId gameAccountId = BnetGameAccountId.CreateFromBnetEntityId(update.entityId);
    int num = (BnetEntityId) fromBnetEntityId == (BnetEntityId) BattleNet.GetMyAccoundId() ? 1 : ((BnetEntityId) gameAccountId == (BnetEntityId) BattleNet.GetMyGameAccountId() ? 1 : 0);
    BnetPlayer player = BnetPresenceMgr.Get().GetPlayer(gameAccountId) ?? BnetPresenceMgr.Get().GetPlayer(fromBnetEntityId);
    bool flag1 = num == 0 && BnetFriendMgr.Get().IsFriend(player);
    bool flag2 = num == 0 && GameState.Get() != null && (GameMgr.Get() == null || !GameMgr.Get().IsSpectator()) && GameState.Get().GetOpposingSidePlayer() != null && (BnetEntityId) GameState.Get().GetOpposingSidePlayer().GetGameAccountId() == (BnetEntityId) gameAccountId;
    string str1 = num != 0 ? "myself" : (flag2 ? "opponent" : (flag1 ? "friend" : string.Empty));
    if (num == 0 && !flag2 && !flag1)
    {
      if (FiresideGatheringManager.Get() != null && FiresideGatheringManager.Get().IsPlayerInMyFSG(player))
        str1 = "fsgpatron";
      else if (BnetNearbyPlayerMgr.Get().IsNearbyPlayer(player))
        str1 = "nearbyplayer";
      else if (((IEnumerable<PartyInfo>) BnetParty.GetJoinedParties()).Any<PartyInfo>((Func<PartyInfo, bool>) (p => p.Type == PartyType.SPECTATOR_PARTY && BnetParty.IsMember(p.Id, gameAccountId))))
        str1 = "fellowspecator";
    }
    string str2 = player == null || player.GetBattleTag() == (BnetBattleTag) null ? "" : player.GetBattleTag().ToString();
    if (string.IsNullOrEmpty(str2) && update.programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.BNET && (update.groupId == 1U && update.fieldId == 4U || update.groupId == 2U && update.fieldId == 5U))
      str2 = update.stringVal;
    string str3 = !string.IsNullOrEmpty(str2) || !string.IsNullOrEmpty(str1) ? string.Format("{0}{1}", (object) str2, string.IsNullOrEmpty(str2) || string.IsNullOrEmpty(str1) ? (object) str1 : (object) string.Format("({0})", (object) str1)) : "someone";
    BnetProgramId bnetProgramId = new BnetProgramId(update.programId);
    string str4;
    string fieldName;
    if ((Blizzard.GameService.SDK.Client.Integration.FourCC) bnetProgramId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.BNET)
    {
      str4 = BnetPresenceField.GetGroupName(update.groupId);
      fieldName = BnetPresenceField.GetFieldName(update.groupId, update.fieldId);
    }
    else if ((Blizzard.GameService.SDK.Client.Integration.FourCC) bnetProgramId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE)
    {
      str4 = "GameAccount";
      fieldName = GamePresenceField.GetFieldName(update.fieldId);
    }
    else
    {
      str4 = update.groupId.ToString();
      fieldName = update.fieldId.ToString();
    }
    string fieldValue = BnetPresenceMgr.GetFieldValue(update);
    if (buffer == null)
      buffer = new StringBuilder();
    else
      buffer.Append("\n");
    buffer.AppendFormat("Update entity={0} who={1} {2}.{3}.{4}={5}", (object) string.Format("{{hi:{0} lo:{1}}}", (object) update.entityId?.High, (object) update.entityId?.Low), (object) str3, (object) bnetProgramId, (object) str4, (object) fieldName, (object) fieldValue);
  }

  private static string GetFieldValue(PresenceUpdate update) => update.programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE ? GamePresenceField.GetFieldValue(update) : BnetPresenceField.GetFieldValue(update);

  private void OnDisconnectedFromBattleNet(BattleNetErrors error)
  {
    this.m_accounts.Clear();
    this.m_gameAccounts.Clear();
    this.m_players.Clear();
  }

  private void OnAccountUpdate(PresenceUpdate update, BnetPlayerChangelist changelist)
  {
    BnetAccountId fromBnetEntityId = BnetAccountId.CreateFromBnetEntityId(update.entityId);
    BnetAccount account = (BnetAccount) null;
    if (!this.m_accounts.TryGetValue(fromBnetEntityId, out account))
      this.CreateAccount(fromBnetEntityId, update, changelist);
    else
      this.UpdateAccount(account, update, changelist);
  }

  private void CreateAccount(
    BnetAccountId id,
    PresenceUpdate update,
    BnetPlayerChangelist changelist)
  {
    BnetAccount account = new BnetAccount();
    this.m_accounts.Add(id, account);
    account.SetId(id);
    BnetPlayer player = (BnetPlayer) null;
    if (!this.m_players.TryGetValue(id, out player))
    {
      player = new BnetPlayer(BnetPlayerSource.PRESENCE_UPDATE);
      this.m_players.Add(id, player);
      BnetPlayerChange change = new BnetPlayerChange();
      change.SetNewPlayer(player);
      changelist.AddChange(change);
    }
    player.SetAccount(account);
    this.UpdateAccount(account, update, changelist);
  }

  private void UpdateAccount(
    BnetAccount account,
    PresenceUpdate update,
    BnetPlayerChangelist changelist)
  {
    BnetPlayer player = this.m_players[account.GetId()];
    if (update.fieldId == 7U)
    {
      bool boolVal = update.boolVal;
      if (boolVal == account.IsAway())
        return;
      this.AddChangedPlayer(player, changelist);
      account.SetAway(boolVal);
      if (!boolVal)
        return;
      account.SetBusy(false);
    }
    else if (update.fieldId == 8U)
    {
      long intVal = update.intVal;
      if (intVal == account.GetAwayTimeMicrosec())
        return;
      this.AddChangedPlayer(player, changelist);
      account.SetAwayTimeMicrosec(intVal);
    }
    else if (update.fieldId == 11U)
    {
      bool boolVal = update.boolVal;
      if (boolVal == account.IsBusy())
        return;
      this.AddChangedPlayer(player, changelist);
      account.SetBusy(boolVal);
      if (!boolVal)
        return;
      account.SetAway(false);
    }
    else if (update.fieldId == 4U)
    {
      BnetBattleTag fromString = BnetBattleTag.CreateFromString(update.stringVal);
      if (fromString == (BnetBattleTag) null)
        Log.All.Print("Failed to parse BattleTag={0} for account={1}", (object) update.stringVal, (object) update.entityId?.Low);
      if (fromString == account.GetBattleTag())
        return;
      this.AddChangedPlayer(player, changelist);
      account.SetBattleTag(fromString);
    }
    else if (update.fieldId == 1U)
    {
      string fullName = update.stringVal;
      if (fullName == null)
      {
        Error.AddDevFatal("BnetPresenceMgr.UpdateAccount() - Failed to convert full name to native string for {0}.", (object) account);
      }
      else
      {
        if (fullName == account.GetFullName())
          return;
        if (fullName == string.Empty && update.valCleared)
          fullName = (string) null;
        this.AddChangedPlayer(player, changelist);
        account.SetFullName(fullName);
      }
    }
    else if (update.fieldId == 6U)
    {
      long intVal = update.intVal;
      if (intVal == account.GetLastOnlineMicrosec())
        return;
      this.AddChangedPlayer(player, changelist);
      account.SetLastOnlineMicrosec(intVal);
    }
    else
    {
      if (update.fieldId == 3U || update.fieldId != 12U)
        return;
      bool boolVal = update.boolVal;
      if (boolVal == account.IsAppearingOffline())
        return;
      this.AddChangedPlayer(player, changelist);
      account.SetAppearingOffline(boolVal);
    }
  }

  private void OnGameAccountUpdate(PresenceUpdate update, BnetPlayerChangelist changelist)
  {
    BnetGameAccountId fromBnetEntityId = BnetGameAccountId.CreateFromBnetEntityId(update.entityId);
    BnetGameAccount gameAccount = (BnetGameAccount) null;
    if (!this.m_gameAccounts.TryGetValue(fromBnetEntityId, out gameAccount))
      this.CreateGameAccount(fromBnetEntityId, update, changelist);
    else
      this.UpdateGameAccount(gameAccount, update, changelist);
  }

  private void CreateGameAccount(
    BnetGameAccountId id,
    PresenceUpdate update,
    BnetPlayerChangelist changelist)
  {
    BnetGameAccount gameAccount = new BnetGameAccount();
    this.m_gameAccounts.Add(id, gameAccount);
    gameAccount.SetId(id);
    this.UpdateGameAccount(gameAccount, update, changelist);
  }

  private void UpdateGameAccount(
    BnetGameAccount gameAccount,
    PresenceUpdate update,
    BnetPlayerChangelist changelist)
  {
    BnetPlayer player = (BnetPlayer) null;
    BnetAccountId ownerId = gameAccount.GetOwnerId();
    if ((BnetEntityId) ownerId != (BnetEntityId) null)
      this.m_players.TryGetValue(ownerId, out player);
    if (update.fieldId == 2U)
    {
      int num = gameAccount.IsBusy() ? 1 : 0;
      int intVal = (int) update.intVal;
      if (intVal == num)
        return;
      this.AddChangedPlayer(player, changelist);
      bool busy = intVal == 1;
      gameAccount.SetBusy(busy);
      if (busy)
        gameAccount.SetAway(false);
      this.HandleGameAccountChange(player, update);
    }
    else if (update.fieldId == 10U)
    {
      bool boolVal = update.boolVal;
      if (boolVal == gameAccount.IsAway())
        return;
      this.AddChangedPlayer(player, changelist);
      gameAccount.SetAway(boolVal);
      if (boolVal)
        gameAccount.SetBusy(false);
      this.HandleGameAccountChange(player, update);
    }
    else if (update.fieldId == 11U)
    {
      long intVal = update.intVal;
      if (intVal == gameAccount.GetAwayTimeMicrosec())
        return;
      this.AddChangedPlayer(player, changelist);
      gameAccount.SetAwayTimeMicrosec(intVal);
      this.HandleGameAccountChange(player, update);
    }
    else if (update.fieldId == 5U)
    {
      BnetBattleTag fromString = BnetBattleTag.CreateFromString(update.stringVal);
      if (fromString == (BnetBattleTag) null)
        Log.All.Print("Failed to parse BattleTag={0} for gameAccount={1}", (object) update.stringVal, (object) update.entityId?.Low);
      if (fromString == gameAccount.GetBattleTag())
        return;
      this.AddChangedPlayer(player, changelist);
      gameAccount.SetBattleTag(fromString);
      this.HandleGameAccountChange(player, update);
    }
    else if (update.fieldId == 1U)
    {
      bool boolVal = update.boolVal;
      if (boolVal == gameAccount.IsOnline())
        return;
      this.AddChangedPlayer(player, changelist);
      gameAccount.SetOnline(boolVal);
      if (!boolVal)
        this.ClearTransientStatus(gameAccount);
      this.HandleGameAccountChange(player, update);
    }
    else if (update.fieldId == 3U)
    {
      BnetProgramId programId = new BnetProgramId(update.stringVal);
      if ((Blizzard.GameService.SDK.Client.Integration.FourCC) programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) gameAccount.GetProgramId())
        return;
      this.AddChangedPlayer(player, changelist);
      gameAccount.SetProgramId(programId);
      this.HandleGameAccountChange(player, update);
    }
    else if (update.fieldId == 4U)
    {
      long intVal = update.intVal;
      if (intVal == gameAccount.GetLastOnlineMicrosec())
        return;
      this.AddChangedPlayer(player, changelist);
      gameAccount.SetLastOnlineMicrosec(intVal);
      this.HandleGameAccountChange(player, update);
    }
    else if (update.fieldId == 7U)
    {
      BnetAccountId fromBnetEntityId = BnetAccountId.CreateFromBnetEntityId(update.entityIdVal);
      if ((BnetEntityId) fromBnetEntityId == (BnetEntityId) gameAccount.GetOwnerId())
        return;
      this.UpdateGameAccountOwner(fromBnetEntityId, gameAccount, changelist);
    }
    else if (update.fieldId == 9U)
    {
      if (!update.valCleared || gameAccount.GetRichPresence() == null)
        return;
      this.AddChangedPlayer(player, changelist);
      gameAccount.SetRichPresence((string) null);
      this.HandleGameAccountChange(player, update);
    }
    else
    {
      if (update.fieldId != 1000U)
        return;
      string richPresence = update.stringVal ?? "";
      if (richPresence == gameAccount.GetRichPresence())
        return;
      this.AddChangedPlayer(player, changelist);
      gameAccount.SetRichPresence(richPresence);
      this.HandleGameAccountChange(player, update);
    }
  }

  private void UpdateGameAccountOwner(
    BnetAccountId ownerId,
    BnetGameAccount gameAccount,
    BnetPlayerChangelist changelist)
  {
    BnetPlayer player1 = (BnetPlayer) null;
    BnetAccountId ownerId1 = gameAccount.GetOwnerId();
    if ((BnetEntityId) ownerId1 != (BnetEntityId) null && this.m_players.TryGetValue(ownerId1, out player1))
    {
      player1.RemoveGameAccount(gameAccount.GetId());
      this.AddChangedPlayer(player1, changelist);
    }
    BnetPlayer player2 = (BnetPlayer) null;
    if (this.m_players.TryGetValue(ownerId, out player2))
    {
      this.AddChangedPlayer(player2, changelist);
    }
    else
    {
      player2 = new BnetPlayer(BnetPlayerSource.PRESENCE_UPDATE);
      this.m_players.Add(ownerId, player2);
      BnetPlayerChange change = new BnetPlayerChange();
      change.SetNewPlayer(player2);
      changelist.AddChange(change);
    }
    gameAccount.SetOwnerId(ownerId);
    player2.AddGameAccount(gameAccount);
    this.CacheMyself(gameAccount, player2);
  }

  private void HandleGameAccountChange(BnetPlayer player, PresenceUpdate update) => player?.OnGameAccountChanged(update.fieldId);

  private void OnGameUpdate(PresenceUpdate update, BnetPlayerChangelist changelist)
  {
    BnetGameAccountId fromBnetEntityId = BnetGameAccountId.CreateFromBnetEntityId(update.entityId);
    BnetGameAccount gameAccount = (BnetGameAccount) null;
    if (!this.m_gameAccounts.TryGetValue(fromBnetEntityId, out gameAccount))
      this.CreateGameInfo(fromBnetEntityId, update, changelist);
    else
      this.UpdateGameInfo(gameAccount, update, changelist);
  }

  private void CreateGameInfo(
    BnetGameAccountId id,
    PresenceUpdate update,
    BnetPlayerChangelist changelist)
  {
    BnetGameAccount gameAccount = new BnetGameAccount();
    this.m_gameAccounts.Add(id, gameAccount);
    gameAccount.SetId(id);
    this.UpdateGameInfo(gameAccount, update, changelist);
  }

  private void UpdateGameInfo(
    BnetGameAccount gameAccount,
    PresenceUpdate update,
    BnetPlayerChangelist changelist)
  {
    BnetPlayer player = (BnetPlayer) null;
    BnetAccountId ownerId = gameAccount.GetOwnerId();
    if ((BnetEntityId) ownerId != (BnetEntityId) null)
      this.m_players.TryGetValue(ownerId, out player);
    if (update.valCleared)
    {
      if (!gameAccount.HasGameField(update.fieldId))
        return;
      this.AddChangedPlayer(player, changelist);
      gameAccount.RemoveGameField(update.fieldId);
      this.HandleGameAccountChange(player, update);
    }
    else
    {
      switch (update.fieldId)
      {
        case 1:
          if (update.boolVal == gameAccount.GetGameFieldBool(update.fieldId))
            break;
          this.AddChangedPlayer(player, changelist);
          gameAccount.SetGameField(update.fieldId, (object) update.boolVal);
          this.HandleGameAccountChange(player, update);
          break;
        case 2:
        case 4:
        case 19:
        case 20:
          if (update.stringVal == gameAccount.GetGameFieldString(update.fieldId))
            break;
          this.AddChangedPlayer(player, changelist);
          gameAccount.SetGameField(update.fieldId, (object) update.stringVal);
          this.HandleGameAccountChange(player, update);
          break;
        case 5:
        case 6:
        case 7:
        case 8:
        case 9:
        case 10:
        case 11:
        case 12:
        case 13:
        case 14:
        case 15:
        case 16:
        case 27:
        case 28:
        case 29:
          if ((int) update.intVal == gameAccount.GetGameFieldInt(update.fieldId))
            break;
          this.AddChangedPlayer(player, changelist);
          gameAccount.SetGameField(update.fieldId, (object) (int) update.intVal);
          this.HandleGameAccountChange(player, update);
          break;
        case 17:
        case 18:
        case 21:
        case 22:
        case 23:
        case 24:
        case 25:
          if (GeneralUtils.AreBytesEqual(update.blobVal, gameAccount.GetGameFieldBytes(update.fieldId)))
            break;
          this.AddChangedPlayer(player, changelist);
          gameAccount.SetGameField(update.fieldId, (object) update.blobVal);
          this.HandleGameAccountChange(player, update);
          break;
        case 26:
          if (update.entityIdVal == gameAccount.GetGameFieldEntityId(update.fieldId))
            break;
          this.AddChangedPlayer(player, changelist);
          gameAccount.SetGameField(update.fieldId, (object) update.entityIdVal);
          this.HandleGameAccountChange(player, update);
          break;
        default:
          Log.Presence.PrintWarning("Unknown HS game account fieldId={0} - not saved into presence cache.", (object) update.fieldId);
          break;
      }
    }
  }

  private void CacheMyself(BnetGameAccount gameAccount, BnetPlayer player)
  {
    if (player == this.m_myPlayer || (BnetEntityId) gameAccount.GetId() != (BnetEntityId) this.m_myGameAccountId)
      return;
    this.m_myPlayer = player;
  }

  private void AddChangedPlayer(BnetPlayer player, BnetPlayerChangelist changelist)
  {
    if (player == null || changelist.HasChange(player))
      return;
    BnetPlayerChange change = new BnetPlayerChange();
    change.SetOldPlayer(player.Clone());
    change.SetNewPlayer(player);
    changelist.AddChange(change);
  }

  private void FirePlayersChangedEvent(BnetPlayerChangelist changelist)
  {
    if (changelist == null || changelist.GetChanges().Count == 0)
      return;
    foreach (BnetPresenceMgr.PlayersChangedListener playersChangedListener in this.m_playersChangedListeners.ToArray())
      playersChangedListener.Fire(changelist);
  }

  private bool ShouldUpdateGameField(uint fieldId, object val, out BnetGameAccount hsGameAccount)
  {
    hsGameAccount = (BnetGameAccount) null;
    if (this.m_myPlayer == null)
      return true;
    hsGameAccount = this.m_myPlayer.GetHearthstoneGameAccount();
    object val1;
    return hsGameAccount == (BnetGameAccount) null || !hsGameAccount.TryGetGameField(fieldId, out val1) || !val.Equals(val1);
  }

  private bool ShouldUpdateGameFieldBlob(
    uint fieldId,
    byte[] val,
    out BnetGameAccount hsGameAccount)
  {
    hsGameAccount = (BnetGameAccount) null;
    if (this.m_myPlayer == null)
      return true;
    hsGameAccount = this.m_myPlayer.GetHearthstoneGameAccount();
    byte[] val1;
    return hsGameAccount == (BnetGameAccount) null || !hsGameAccount.TryGetGameFieldBytes(fieldId, out val1) || !GeneralUtils.AreArraysEqual<byte>(val, val1);
  }

  private BnetPlayerChangelist ChangeGameField(
    BnetGameAccount hsGameAccount,
    uint fieldId,
    object val)
  {
    if (hsGameAccount == (BnetGameAccount) null)
      return (BnetPlayerChangelist) null;
    BnetPlayerChange change = new BnetPlayerChange();
    change.SetOldPlayer(this.m_myPlayer.Clone());
    change.SetNewPlayer(this.m_myPlayer);
    hsGameAccount.SetGameField(fieldId, val);
    BnetPlayerChangelist playerChangelist = new BnetPlayerChangelist();
    playerChangelist.AddChange(change);
    return playerChangelist;
  }

  public delegate void PlayersChangedCallback(BnetPlayerChangelist changelist, object userData);

  private class PlayersChangedListener : EventListener<BnetPresenceMgr.PlayersChangedCallback>
  {
    public void Fire(BnetPlayerChangelist changelist) => this.m_callback(changelist, this.m_userData);
  }
}
