using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Jobs;
using Blizzard.T5.Services;
using Hearthstone;
using PegasusLettuce;
using PegasusShared;
using SpectatorProto;
using System;
using System.Collections.Generic;
using System.Linq;

public class PartyManager : IService
{
  private PartyManager.PartyData m_partyData = new PartyManager.PartyData();
  private DialogBase m_inviteDialog;
  private BnetPartyId m_pendingParty;
  public static int BATTLEGROUNDS_PARTY_LIMIT = 8;
  public static int BATTLEGROUNDS_MAX_RANKED_PARTY_SIZE_FALLBACK = 4;
  public static int MERCENARIES_PARTY_LIMIT = 2;
  private List<PartyManager.ChangedListener> m_changedListeners = new List<PartyManager.ChangedListener>();
  private List<PartyManager.MemberAttributeChangedListener> m_memberAttributeChangedListeners = new List<PartyManager.MemberAttributeChangedListener>();
  private List<PartyManager.PartyAttributeChangedListener> m_partyAttributeChangedListeners = new List<PartyManager.PartyAttributeChangedListener>();

  public IEnumerator<IAsyncJobResult> Initialize(
    ServiceLocator serviceLocator)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    PartyManager partyManager = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    partyManager.m_partyData = new PartyManager.PartyData();
    BnetParty.OnJoined += new BnetParty.JoinedHandler(partyManager.BnetParty_OnJoined);
    BnetParty.OnReceivedInvite += new BnetParty.ReceivedInviteHandler(partyManager.BnetParty_OnReceivedInvite);
    BnetParty.OnPartyAttributeChanged += new BnetParty.PartyAttributeChangedHandler(partyManager.BnetParty_OnPartyAttributeChanged);
    BnetParty.OnMemberAttributeChanged += new BnetParty.MemberAttributeChangedHandler(partyManager.BnetParty_OnMemberAttributeChanged);
    BnetParty.OnMemberEvent += new BnetParty.MemberEventHandler(partyManager.BnetParty_OnMemberEvent);
    BnetParty.OnSentInvite += new BnetParty.SentInviteHandler(partyManager.BnetParty_OnSentInvite);
    BnetParty.OnReceivedInviteRequest += new BnetParty.ReceivedInviteRequestHandler(partyManager.BnetParty_OnReceivedInviteRequest);
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(partyManager.OnPresenceUpdated));
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(partyManager.OnFatalError));
    LoginManager.Get().OnInitialClientStateReceived += new System.Action(partyManager.OnLoginComplete);
    HearthstoneApplication.Get().WillReset += new System.Action(partyManager.WillReset);
    return false;
  }

  public void Shutdown()
  {
    BnetParty.OnJoined -= new BnetParty.JoinedHandler(this.BnetParty_OnJoined);
    BnetParty.OnReceivedInvite -= new BnetParty.ReceivedInviteHandler(this.BnetParty_OnReceivedInvite);
    BnetParty.OnPartyAttributeChanged -= new BnetParty.PartyAttributeChangedHandler(this.BnetParty_OnPartyAttributeChanged);
    BnetParty.OnMemberAttributeChanged -= new BnetParty.MemberAttributeChangedHandler(this.BnetParty_OnMemberAttributeChanged);
    BnetParty.OnMemberEvent -= new BnetParty.MemberEventHandler(this.BnetParty_OnMemberEvent);
    BnetParty.OnSentInvite -= new BnetParty.SentInviteHandler(this.BnetParty_OnSentInvite);
    BnetParty.OnReceivedInviteRequest -= new BnetParty.ReceivedInviteRequestHandler(this.BnetParty_OnReceivedInviteRequest);
    BnetPresenceMgr.Get().RemovePlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPresenceUpdated));
    FatalErrorMgr.Get().RemoveErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    LoginManager.Get().OnInitialClientStateReceived -= new System.Action(this.OnLoginComplete);
    HearthstoneApplication.Get().WillReset -= new System.Action(this.WillReset);
  }

  public System.Type[] GetDependencies() => new System.Type[2]
  {
    typeof (LoginManager),
    typeof (Network)
  };

  public static PartyManager Get() => ServiceManager.Get<PartyManager>();

  private void WillReset() => this.ClearPartyData();

  public static bool IsPartyTypeEnabledInGuardian(PartyType partyType)
  {
    NetCache.NetCacheFeatures.CacheGames games = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games;
    if (games == null)
      return false;
    switch (partyType)
    {
      case PartyType.FRIENDLY_CHALLENGE:
        return games.Friendly;
      case PartyType.BATTLEGROUNDS_PARTY:
        return games.BattlegroundsFriendlyChallenge;
      case PartyType.MERCENARIES_FRIENDLY_CHALLENGE:
        return games.MercenariesFriendly;
      case PartyType.MERCENARIES_COOP_PARTY:
        return games.MercenariesCoOp;
      default:
        return true;
    }
  }

  public bool IsInParty() => this.m_partyData.m_partyId != (BnetPartyId) null;

  public BnetPartyId GetCurrentPartyId() => this.m_partyData.m_partyId;

  public bool IsInBattlegroundsParty() => this.IsInParty() && this.m_partyData.m_type == PartyType.BATTLEGROUNDS_PARTY;

  public bool IsInMercenariesFriendlyChallenge() => this.IsInParty() && this.m_partyData.m_type == PartyType.MERCENARIES_FRIENDLY_CHALLENGE;

  public bool IsInMercenariesCoOpParty() => this.IsInParty() && this.m_partyData.m_type == PartyType.MERCENARIES_COOP_PARTY;

  public bool IsPlayerInCurrentPartyOrPending(BnetGameAccountId playerGameAccountId) => this.IsPlayerInCurrentParty(playerGameAccountId) || this.IsPlayerPendingInCurrentParty(playerGameAccountId);

  public bool IsPlayerInCurrentParty(BnetGameAccountId playerGameAccountId) => BnetParty.IsMember(this.m_partyData.m_partyId, playerGameAccountId);

  public bool IsPlayerPendingInCurrentParty(BnetGameAccountId playerGameAccountId)
  {
    foreach (PartyInvite pendingInvite in this.GetPendingInvites())
    {
      if ((BnetEntityId) pendingInvite.InviteeId == (BnetEntityId) playerGameAccountId)
        return true;
    }
    return false;
  }

  public bool IsPlayerInAnyParty(BnetGameAccountId playerGameAccountId)
  {
    BnetPlayer player = BnetUtils.GetPlayer(playerGameAccountId);
    if (player == null)
      return false;
    BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
    return !(hearthstoneGameAccount == (BnetGameAccount) null) && hearthstoneGameAccount.GetGameFields() != null && hearthstoneGameAccount.GetPartyId() != BnetPartyId.Empty;
  }

  public bool IsPartyLeader() => BnetParty.IsLeader(this.m_partyData.m_partyId);

  public BnetParty.PartyMember GetPartyLeader()
  {
    foreach (BnetParty.PartyMember member in BnetParty.GetMembers(this.m_partyData.m_partyId))
    {
      if (member.IsLeader(this.m_partyData.m_type))
        return member;
    }
    return (BnetParty.PartyMember) null;
  }

  public BnetGameAccountId GetPartyLeaderGameAccountId()
  {
    BnetParty.PartyMember partyLeader = this.GetPartyLeader();
    if (partyLeader != null)
      return partyLeader.GameAccountId;
    Log.Party.PrintError("PartyManager - No party leader.");
    return (BnetGameAccountId) null;
  }

  public bool CanInvite(BnetGameAccountId playerGameAccountId)
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    if (!myPlayer.IsOnline() || myPlayer.IsAppearingOffline() || this.IsPlayerInAnyParty(playerGameAccountId) || this.IsPlayerPendingInCurrentParty(playerGameAccountId) || this.IsInParty() && this.GetCurrentPartySize() >= this.GetMaxPartySizeByPartyType(this.m_partyData.m_type))
      return false;
    BnetPlayer player = BnetUtils.GetPlayer(playerGameAccountId);
    return player != null && FriendChallengeMgr.Get().IsOpponentAvailable(player);
  }

  public bool CanKick(BnetGameAccountId playerGameAccountId) => (!this.IsInParty() || this.IsPartyLeader()) && this.IsPlayerInCurrentPartyOrPending(playerGameAccountId);

  public bool CanSpectatePartyMember(BnetGameAccountId gameAccountId)
  {
    JoinInfo joinInfoForPlayer = this.GetSpectatorJoinInfoForPlayer(gameAccountId);
    return joinInfoForPlayer != null && SpectatorManager.Get().CanSpectate(gameAccountId, joinInfoForPlayer);
  }

  public bool SpectatePartyMember(BnetGameAccountId gameAccountId)
  {
    JoinInfo joinInfoForPlayer = this.GetSpectatorJoinInfoForPlayer(gameAccountId);
    if (joinInfoForPlayer == null || !this.CanSpectatePartyMember(gameAccountId))
      return false;
    SpectatorManager.Get().SpectatePlayer(gameAccountId, joinInfoForPlayer);
    return true;
  }

  public void SendInvite(PartyType partyType, BnetGameAccountId playerGameAccountId)
  {
    if (!this.CanInvite(playerGameAccountId) || this.IsPlayerInCurrentPartyOrPending(playerGameAccountId))
      return;
    if (!this.IsInParty() && this.ShouldSupportPartyType(partyType))
      this.CreateParty(partyType, playerGameAccountId);
    else if (partyType == PartyType.BATTLEGROUNDS_PARTY)
      this.InvitePlayerToBattlegroundsParty(playerGameAccountId);
    else
      this.SendInvite_Internal(playerGameAccountId);
  }

  public void KickPlayerFromParty(BnetGameAccountId playerGameAccountId)
  {
    if (!this.IsInParty())
      return;
    if (BnetParty.IsMember(this.m_partyData.m_partyId, playerGameAccountId))
    {
      BnetParty.KickMember(this.m_partyData.m_partyId, playerGameAccountId);
    }
    else
    {
      ulong? idFromGameAccount = this.GetPendingInviteIdFromGameAccount(playerGameAccountId);
      if (idFromGameAccount.HasValue)
      {
        BnetNearbyPlayerMgr.Get().FindNearbyStranger(playerGameAccountId)?.GetHearthstoneGameAccount().SetGameField(1U, (object) false);
        BnetParty.RevokeSentInvite(this.m_partyData.m_partyId, idFromGameAccount.Value);
        this.FireChangedEvent(PartyManager.PartyInviteEvent.I_RESCINDED_INVITE, playerGameAccountId);
      }
      else
        Log.Party.PrintError("Unable to kick player {0} from party. Player not found in party.", (object) playerGameAccountId.ToString());
    }
  }

  public void SendInviteSuggestion(PartyType partyType, BnetGameAccountId playerGameAccountId)
  {
    if (!this.CanInvite(playerGameAccountId) || this.IsPlayerInCurrentPartyOrPending(playerGameAccountId))
      return;
    if (!this.IsInParty() && this.ShouldSupportPartyType(partyType))
    {
      this.CreateParty(partyType, playerGameAccountId);
    }
    else
    {
      BnetGameAccountId leaderGameAccountId = this.GetPartyLeaderGameAccountId();
      if ((BnetEntityId) leaderGameAccountId == (BnetEntityId) null || partyType != PartyType.BATTLEGROUNDS_PARTY)
        return;
      BnetParty.RequestInvite(this.m_partyData.m_partyId, leaderGameAccountId, playerGameAccountId, partyType);
    }
  }

  public void SetMyPlayerTagsAttribute()
  {
    if (Cheats.Get() == null || string.IsNullOrEmpty(Cheats.Get().GetPlayerTags()))
      return;
    BattleNet.SetMemberAttributes(this.m_partyData.m_partyId, BnetPresenceMgr.Get().GetMyPlayer().GetBestGameAccountId(), BnetAttribute.CreateAttribute("cheat_player_tags", Cheats.Get().GetPlayerTags()));
  }

  public BnetParty.PartyMember[] GetMembers() => this.m_partyData.m_partyId == (BnetPartyId) null ? new BnetParty.PartyMember[0] : BnetParty.GetMembers(this.m_partyData.m_partyId);

  public PartyInvite[] GetPendingInvites() => this.m_partyData.m_partyId == (BnetPartyId) null ? new PartyInvite[0] : BnetParty.GetSentInvites(this.m_partyData.m_partyId);

  public void FindGame()
  {
    if (!this.IsInParty() || !this.ShouldSupportPartyType(this.m_partyData.m_type))
    {
      Log.Party.PrintError("FindGame - Unable to enter game unless you are in a supported party.");
    }
    else
    {
      BattleNet.SetPartyAttributes(this.m_partyData.m_partyId, BnetAttribute.CreateAttribute("queue", "in_queue"));
      switch (this.m_partyData.m_type)
      {
        case PartyType.BATTLEGROUNDS_PARTY:
          Network.Get().EnterBattlegroundsWithParty(this.GetMembers(), 3459);
          break;
        case PartyType.MERCENARIES_FRIENDLY_CHALLENGE:
          BnetParty.PartyMember otherPartyMember1 = this.GetOtherPartyMember();
          if (otherPartyMember1 == null)
          {
            Log.Lettuce.PrintError("PartyManager.FindGame() - Not enough party members.");
            return;
          }
          long player1TeamId;
          BattleNet.GetMemberAttribute<long>(this.m_partyData.m_partyId, BnetPresenceMgr.Get().GetMyGameAccountId(), "team_id", out player1TeamId);
          long player2TeamId;
          BattleNet.GetMemberAttribute<long>(this.m_partyData.m_partyId, otherPartyMember1.GameAccountId, "team_id", out player2TeamId);
          long num1;
          BattleNet.GetMemberAttribute<long>(this.m_partyData.m_partyId, BnetPresenceMgr.Get().GetMyGameAccountId(), "ts_state", out num1);
          long num2;
          BattleNet.GetMemberAttribute<long>(this.m_partyData.m_partyId, otherPartyMember1.GameAccountId, "ts_state", out num2);
          if (player1TeamId == 0L || player2TeamId == 0L)
          {
            Log.Lettuce.PrintError(string.Format("PartyManager.FindGame() - Team not selected. Team1={0}, Team2={1}", (object) player1TeamId, (object) player2TeamId));
            return;
          }
          Network.Get().EnterMercenariesFriendlyChallenge(3743, player1TeamId, num1 == 2L, player2TeamId, num2 == 2L, otherPartyMember1.GameAccountId);
          break;
        case PartyType.MERCENARIES_COOP_PARTY:
          BnetParty.PartyMember otherPartyMember2 = this.GetOtherPartyMember();
          if (otherPartyMember2 == null)
          {
            Log.Lettuce.PrintError("PartyManager.FindGame() - Not enough party members.");
            return;
          }
          int num3;
          if (!BattleNet.GetPartyAttribute<int>(this.m_partyData.m_partyId, "node_id", out num3))
          {
            Log.Lettuce.PrintError("PartyManager.FindGame() - No map node selected.");
            return;
          }
          Network.Get().EnterMercenariesCoOpWithFriend(otherPartyMember2.GameAccountId, 3899, new int?(num3));
          break;
      }
      this.WaitForGame();
    }
  }

  public BnetGameAccountId GetLeader()
  {
    if (!this.IsInParty())
      return (BnetGameAccountId) null;
    BnetParty.PartyMember leader = BnetParty.GetLeader(this.m_partyData.m_partyId);
    if (leader != null)
      return leader.GameAccountId;
    Log.Party.PrintError("PartyManager.GetLeader() - Unable to get party leader!");
    return (BnetGameAccountId) null;
  }

  public BnetParty.PartyMember GetOtherPartyMember()
  {
    BnetParty.PartyMember[] members = this.GetMembers();
    if (members.Length == 2)
      return members[1];
    Log.Lettuce.PrintWarning("PartyManager.GetOtherPartyMember() - This function only works with a party size of 2.");
    return (BnetParty.PartyMember) null;
  }

  public string GetOpponentBestName()
  {
    BnetParty.PartyMember otherPartyMember = this.GetOtherPartyMember();
    return otherPartyMember == null ? string.Empty : this.GetPartyMemberName(otherPartyMember.GameAccountId);
  }

  public bool IsBaconParty() => this.m_partyData.m_partyId != (BnetPartyId) null && this.m_partyData.m_scenarioId == ScenarioDbId.TB_BACONSHOP_8P;

  public void LeaveParty()
  {
    if (!this.IsInParty())
      return;
    if (this.IsPartyLeader())
      BnetParty.DissolveParty(this.m_partyData.m_partyId);
    else
      BnetParty.Leave(this.m_partyData.m_partyId);
    this.ClearPartyData();
  }

  public void CancelQueue()
  {
    BnetGameAccountId hearthstoneGameAccountId = BnetPresenceMgr.Get().GetMyPlayer().GetHearthstoneGameAccountId();
    BattleNet.SetPartyAttributes(this.m_partyData.m_partyId, BnetAttribute.CreateAttribute("canceled_by", ProtobufUtil.ToByteArray((IProtoBuf) new BnetId()
    {
      Hi = hearthstoneGameAccountId.High,
      Lo = hearthstoneGameAccountId.Low
    })));
    BattleNet.SetPartyAttributes(this.m_partyData.m_partyId, BnetAttribute.CreateAttribute("queue", "cancel_queue"));
  }

  public int GetCurrentPartySize() => this.GetCurrentAndPendingPartyMembers().Count<BnetGameAccountId>();

  public int GetReadyPartyMemberCount()
  {
    int partyMemberCount = 0;
    BnetPresenceMgr.Get().GetMyPlayer();
    foreach (OnlinePlayer member in BnetParty.GetMembers(this.m_partyData.m_partyId))
    {
      switch (BaconParty.GetReadyStatusForPartyMember(member.GameAccountId))
      {
        case BaconParty.Status.Ready:
        case BaconParty.Status.Leader:
          ++partyMemberCount;
          break;
      }
    }
    return partyMemberCount;
  }

  public List<BnetGameAccountId> GetCurrentAndPendingPartyMembers()
  {
    List<BnetGameAccountId> pendingPartyMembers = new List<BnetGameAccountId>();
    foreach (BnetParty.PartyMember member in BnetParty.GetMembers(this.m_partyData.m_partyId))
      pendingPartyMembers.Add(member.GameAccountId);
    foreach (PartyInvite pendingInvite in this.GetPendingInvites())
    {
      BnetGameAccountId inviteeId = pendingInvite.InviteeId;
      if (!pendingPartyMembers.Contains(inviteeId))
        pendingPartyMembers.Add(inviteeId);
    }
    return pendingPartyMembers;
  }

  public int GetMaxPartySizeByPartyType(PartyType type)
  {
    switch (type)
    {
      case PartyType.BATTLEGROUNDS_PARTY:
        return PartyManager.BATTLEGROUNDS_PARTY_LIMIT;
      case PartyType.MERCENARIES_FRIENDLY_CHALLENGE:
      case PartyType.MERCENARIES_COOP_PARTY:
        return PartyManager.MERCENARIES_PARTY_LIMIT;
      default:
        Log.Party.PrintError("GetMaxPartySizeByPartyType() - Unsupported party type {0}.", (object) type.ToString());
        return 2;
    }
  }

  public void UpdateSpectatorJoinInfo(JoinInfo joinInfo)
  {
    if (!this.IsInParty())
      return;
    BattleNet.SetMemberAttributes(this.m_partyData.m_partyId, BnetPresenceMgr.Get().GetMyGameAccountId(), BnetAttribute.CreateAttribute("spectator_info", joinInfo == null ? (byte[]) null : ProtobufUtil.ToByteArray((IProtoBuf) joinInfo)));
  }

  public JoinInfo GetSpectatorJoinInfoForPlayer(BnetGameAccountId gameAccountId)
  {
    if (!this.IsInParty())
      return (JoinInfo) null;
    byte[] bytes;
    BattleNet.GetMemberAttribute<byte[]>(this.m_partyData.m_partyId, gameAccountId, "spectator_info", out bytes);
    return bytes != null && bytes.Length != 0 ? ProtobufUtil.ParseFrom<JoinInfo>(bytes) : (JoinInfo) null;
  }

  public string GetPartyMemberName(BnetGameAccountId playerGameAccountId)
  {
    BnetPlayer player = BnetUtils.GetPlayer(playerGameAccountId);
    if (player != null)
      return player.GetBestName();
    foreach (BnetParty.PartyMember member in this.GetMembers())
    {
      if ((BnetEntityId) member.GameAccountId == (BnetEntityId) playerGameAccountId)
      {
        if (!string.IsNullOrEmpty(member.BattleTag))
        {
          BnetBattleTag fromString = BnetBattleTag.CreateFromString(member.BattleTag);
          return !(fromString == (BnetBattleTag) null) ? fromString.GetName() : member.BattleTag;
        }
        Log.Party.PrintError("GetPartyMemberName() - No name for party member {0}.", (object) playerGameAccountId.ToString());
      }
    }
    foreach (PartyInvite pendingInvite in this.GetPendingInvites())
    {
      if ((BnetEntityId) pendingInvite.InviteeId == (BnetEntityId) playerGameAccountId)
      {
        if (!string.IsNullOrEmpty(pendingInvite.InviteeName))
        {
          BnetBattleTag fromString = BnetBattleTag.CreateFromString(pendingInvite.InviteeName);
          return !(fromString == (BnetBattleTag) null) ? fromString.GetName() : pendingInvite.InviteeName;
        }
        Log.Party.PrintError("GetPartyMemberName() - No name for pending invitee {0}.", (object) playerGameAccountId.ToString());
      }
    }
    return GameStrings.Get("GLUE_PARTY_MEMBER_NO_NAME");
  }

  public bool HasPendingPartyInviteOrDialog() => this.m_pendingParty != (BnetPartyId) null || (UnityEngine.Object) this.m_inviteDialog != (UnityEngine.Object) null;

  public void SetReadyStatus(bool ready)
  {
    if (!this.IsInParty())
      return;
    BattleNet.SetMemberAttributes(this.m_partyData.m_partyId, BnetPresenceMgr.Get().GetMyPlayer().GetBestGameAccountId(), BnetAttribute.CreateAttribute(nameof (ready), ready ? nameof (ready) : "not_ready"));
  }

  public void SetSceneAttribute(string scene)
  {
    if (!this.IsInParty())
      return;
    BattleNet.SetMemberAttributes(this.m_partyData.m_partyId, BnetPresenceMgr.Get().GetMyPlayer().GetBestGameAccountId(), BnetAttribute.CreateAttribute(nameof (scene), scene));
  }

  public bool AreAllPartyMembersReady()
  {
    foreach (OnlinePlayer member in this.GetMembers())
    {
      string str;
      if (!BattleNet.GetMemberAttribute<string>(this.m_partyData.m_partyId, member.GameAccountId, "ready", out str) || string.IsNullOrEmpty(str) || str == "not_ready")
        return false;
    }
    return true;
  }

  private string GetGameStringPartyTitleKey() => this.IsBaconParty() ? "GLUE_BACON_PRIVATE_PARTY_TITLE" : "GLOBAL_FRIEND_PARTY_INVITATION_TITLE";

  public int GetBattlegroundsMaxRankedPartySize()
  {
    NetCache.NetCacheFeatures netObject = NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>();
    return netObject != null ? netObject.BattlegroundsMaxRankedPartySize : PartyManager.BATTLEGROUNDS_MAX_RANKED_PARTY_SIZE_FALLBACK;
  }

  private void InvitePlayerToBattlegroundsParty(BnetGameAccountId playerGameAccountId)
  {
    if (this.GetCurrentPartySize() >= PartyManager.BATTLEGROUNDS_PARTY_LIMIT)
      return;
    this.SendInvite_Internal(playerGameAccountId);
  }

  private void ShowDeclinedInvitationPopup(BnetGameAccountId gameAccountId)
  {
    BnetPlayer player = BnetUtils.GetPlayer(gameAccountId);
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get(this.GetGameStringPartyTitleKey()),
      m_text = GameStrings.Format("GLOBAL_FRIEND_PARTY_INVITATION_BODY_DECLINED", (object) FriendUtils.GetUniqueName(player)),
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_showAlertIcon = false,
      m_okText = GameStrings.Get("GLOBAL_OKAY")
    });
  }

  public void SetSelectedMercenariesCoOpMapNodeId(int mapNodeId) => BattleNet.SetPartyAttributes(this.m_partyData.m_partyId, BnetAttribute.CreateAttribute("node_id", (long) mapNodeId));

  public void SetSelectedMercenariesTeamId(long teamId) => BattleNet.SetMemberAttributes(this.m_partyData.m_partyId, BnetPresenceMgr.Get()?.GetMyPlayer()?.GetBestGameAccountId(), BnetAttribute.CreateAttribute("team_id", teamId));

  public void SetTeamSharingMsg(PartyManager.MercTeamShareMSG msg) => BattleNet.SetMemberAttributes(this.m_partyData.m_partyId, BnetPresenceMgr.Get()?.GetMyPlayer()?.GetBestGameAccountId(), BnetAttribute.CreateAttribute("ts_MSG", (long) msg));

  public void SetOpponentTeamSharingButtonStatus(
    PartyManager.MercTeamSharingButtonStatus shareButtonStatus)
  {
    BattleNet.SetMemberAttributes(this.m_partyData.m_partyId, BnetPresenceMgr.Get()?.GetMyPlayer()?.GetBestGameAccountId(), BnetAttribute.CreateAttribute("ts_status", (long) shareButtonStatus));
  }

  public PartyManager.MercTeamSharingButtonStatus GetMyTeamSharingButtonStatus()
  {
    long sharingButtonStatus;
    BattleNet.GetMemberAttribute<long>(this.m_partyData.m_partyId, this.GetOtherPartyMember()?.GameAccountId, "ts_status", out sharingButtonStatus);
    return (PartyManager.MercTeamSharingButtonStatus) sharingButtonStatus;
  }

  public void SetTeamSharingState(PartyManager.MercTeamShareState shareState) => BattleNet.SetMemberAttributes(this.m_partyData.m_partyId, BnetPresenceMgr.Get()?.GetMyPlayer()?.GetBestGameAccountId(), BnetAttribute.CreateAttribute("ts_state", (long) shareState));

  public PartyManager.MercTeamShareState GetTeamSharingState(bool getOpponentState = false)
  {
    long teamSharingState;
    BattleNet.GetMemberAttribute<long>(this.m_partyData.m_partyId, !getOpponentState ? BnetPresenceMgr.Get()?.GetMyPlayer()?.GetBestGameAccountId() : this.GetOtherPartyMember()?.GameAccountId, "ts_state", out teamSharingState);
    return (PartyManager.MercTeamShareState) teamSharingState;
  }

  public void SetSharedTeams(LettuceTeamList teamList) => BattleNet.SetMemberAttributes(this.m_partyData.m_partyId, BnetPresenceMgr.Get()?.GetMyPlayer()?.GetBestGameAccountId(), BnetAttribute.CreateAttribute("ts_teams", ProtobufUtil.ToByteArray((IProtoBuf) teamList)));

  public LettuceTeamList GetSharedTeams()
  {
    byte[] bytes;
    return BattleNet.GetMemberAttribute<byte[]>(this.m_partyData.m_partyId, this.GetOtherPartyMember()?.GameAccountId, "ts_teams", out bytes) && bytes != null ? ProtobufUtil.ParseFrom<LettuceTeamList>(bytes) : (LettuceTeamList) null;
  }

  public long GetOpponentSelectedTeam()
  {
    BnetParty.PartyMember otherPartyMember = this.GetOtherPartyMember();
    long num;
    return otherPartyMember != null && BattleNet.GetMemberAttribute<long>(this.m_partyData.m_partyId, otherPartyMember.GameAccountId, "team_id", out num) ? num : 0L;
  }

  public void StartMercenariesFriendlyChallengeEntry(BnetPlayer opponent)
  {
    this.AddChangedListener(new PartyManager.ChangedCallback(this.HandleMercenaryFriendlyChallengeNotifications), (object) opponent);
    this.SendInvite(PartyType.MERCENARIES_FRIENDLY_CHALLENGE, opponent.GetBestGameAccountId());
  }

  private void ClearPartyData()
  {
    this.m_partyData = new PartyManager.PartyData();
    this.UpdateMyAvailability();
  }

  private bool ShouldSupportPartyType(PartyType partyType)
  {
    switch (partyType)
    {
      case PartyType.BATTLEGROUNDS_PARTY:
      case PartyType.MERCENARIES_FRIENDLY_CHALLENGE:
      case PartyType.MERCENARIES_COOP_PARTY:
        return true;
      default:
        return false;
    }
  }

  private void WaitForGame() => GameMgr.Get().WaitForFriendChallengeToStart(this.m_partyData.m_format, BrawlType.BRAWL_TYPE_UNKNOWN, (int) this.m_partyData.m_scenarioId, 0, this.m_partyData.m_type);

  private ScenarioDbId GetScenario(PartyType type)
  {
    switch (type)
    {
      case PartyType.BATTLEGROUNDS_PARTY:
        return ScenarioDbId.TB_BACONSHOP_8P;
      case PartyType.MERCENARIES_FRIENDLY_CHALLENGE:
        return ScenarioDbId.LETTUCE_1v1;
      case PartyType.MERCENARIES_COOP_PARTY:
        return ScenarioDbId.LETTUCE_MAP_COOP;
      default:
        Log.Party.PrintError("PartyManager.GetScenario() received an unsupported party type: {0}", (object) type);
        return ScenarioDbId.INVALID;
    }
  }

  private FormatType GetFormat(PartyType type) => FormatType.FT_UNKNOWN;

  private int GetSeason(PartyType type) => 0;

  private SceneMgr.Mode GetMode(PartyType type)
  {
    switch (type)
    {
      case PartyType.BATTLEGROUNDS_PARTY:
        return SceneMgr.Mode.BACON;
      case PartyType.MERCENARIES_FRIENDLY_CHALLENGE:
        return SceneMgr.Mode.LETTUCE_FRIENDLY;
      case PartyType.MERCENARIES_COOP_PARTY:
        return SceneMgr.Mode.LETTUCE_COOP;
      default:
        Log.Party.PrintError("PartyManager.GetMode() received an unsupported party type: {0}", (object) type);
        return SceneMgr.Mode.HUB;
    }
  }

  private bool HasCompletedRequiredTutorialForPartyType(PartyType type)
  {
    switch (type)
    {
      case PartyType.BATTLEGROUNDS_PARTY:
        return GameUtils.IsBattleGroundsTutorialComplete();
      case PartyType.MERCENARIES_FRIENDLY_CHALLENGE:
      case PartyType.MERCENARIES_COOP_PARTY:
        return GameUtils.IsMercenariesVillageTutorialComplete();
      default:
        Log.Party.PrintError("PartyManager.HasCompletedRequiredTutorialForPartyType() received an unsupported party type: {0}", (object) type);
        return true;
    }
  }

  private void CreateParty(PartyType type, BnetGameAccountId playerToInvite)
  {
    if (this.IsInParty())
      return;
    this.m_partyData.m_type = type;
    this.m_partyData.m_scenarioId = this.GetScenario(type);
    this.m_partyData.m_format = this.GetFormat(type);
    this.m_partyData.m_season = this.GetSeason(type);
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("WTCG.Game.ScenarioId", (long) this.m_partyData.m_scenarioId), BnetAttribute.CreateAttribute("WTCG.Format.Type", (long) this.m_partyData.m_format), BnetAttribute.CreateAttribute("WTCG.Season.Id", (long) this.m_partyData.m_season));
    byte[] questInfoBlob;
    if (this.GetPartyQuestInfoBlob(out questInfoBlob))
      attributeCollection.Add(BnetAttribute.CreateAttribute("WTCG.Party.QuestInfo", questInfoBlob));
    BnetParty.CreateParty(type, ChannelApi.PartyPrivacyLevel.OpenInvitation, (BnetParty.CreateSuccessCallback) ((pType, newlyCreatedPartyId) =>
    {
      this.m_partyData.m_partyId = newlyCreatedPartyId;
      this.UpdateMyAvailability();
      BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
      this.InitializePersonalPartyMemberAttributes(pType);
      this.FireChangedEvent(PartyManager.PartyInviteEvent.I_CREATED_PARTY, myGameAccountId);
      if (!((BnetEntityId) playerToInvite != (BnetEntityId) null))
        return;
      this.SendInvite(type, playerToInvite);
    }), attributeCollection);
  }

  private bool GetPartyQuestInfoBlob(out byte[] questInfoBlob)
  {
    questInfoBlob = (byte[]) null;
    IEnumerable<Achievement> source = AchieveManager.Get().GetActiveQuests().Where<Achievement>((Func<Achievement, bool>) (q => q.IsFriendlyChallengeQuest));
    if (!source.Any<Achievement>())
      return false;
    PartyQuestInfo protobuf = new PartyQuestInfo();
    protobuf.QuestIds.AddRange(source.Select<Achievement, int>((Func<Achievement, int>) (q => q.ID)));
    questInfoBlob = ProtobufUtil.ToByteArray((IProtoBuf) protobuf);
    return true;
  }

  private void InitializePersonalPartyMemberAttributes(PartyType partyType)
  {
    if (partyType != PartyType.BATTLEGROUNDS_PARTY)
      return;
    this.SetReadyStatus(false);
    this.SetMyPlayerTagsAttribute();
  }

  private void SendInvite_Internal(BnetGameAccountId bnetGameAccountId) => BnetParty.SendInvite(this.m_partyData.m_partyId, bnetGameAccountId, true);

  private void UpdateMyAvailability()
  {
    if (!Network.ShouldBeConnectedToAurora() || !Network.IsLoggedIn())
      return;
    BnetPartyId partyId1 = this.m_partyData.m_partyId;
    BnetPresenceMgr.Get().SetGameField(26U, partyId1 != (BnetPartyId) null ? partyId1.ToBnetEntityId() : BnetPartyId.Empty.ToBnetEntityId());
    BnetNearbyPlayerMgr bnetNearbyPlayerMgr = BnetNearbyPlayerMgr.Get();
    BnetPartyId partyId2 = partyId1;
    if ((object) partyId2 == null)
      partyId2 = BnetPartyId.Empty;
    bnetNearbyPlayerMgr.SetPartyId(partyId2);
  }

  private void ShowInviteDialog(
    BnetGameAccountId leaderGameAccountId,
    string inviterBattleTag,
    PartyType partyType)
  {
    BnetPlayer challenger = BnetUtils.GetPlayer(leaderGameAccountId);
    if (challenger == null)
    {
      Log.Party.PrintDebug("PartyManager.ShowInviteDialog() - Received invite from player {0} with no presence!", (object) leaderGameAccountId);
      BnetAccount account = new BnetAccount();
      BnetBattleTag battleTag = new BnetBattleTag();
      battleTag.SetString(inviterBattleTag);
      account.SetBattleTag(battleTag);
      challenger = new BnetPlayer(BnetPlayerSource.UNASSIGNED);
      challenger.SetAccount(account);
    }
    DialogManager.Get().ShowFriendlyChallenge(FormatType.FT_UNKNOWN, challenger, false, partyType, FriendChallengeMgr.Get().GetPartyQuestInfo(this.m_pendingParty, "WTCG.Party.QuestInfo"), new FriendlyChallengeDialog.ResponseCallback(this.OnInviteReceivedDialogResponse), new DialogManager.DialogProcessCallback(this.OnInviteReceivedDialogProcessed));
  }

  private void HandleMercenaryFriendlyChallengeNotifications(
    PartyManager.PartyInviteEvent challengeEvent,
    BnetGameAccountId playerGameAccountId,
    PartyManager.PartyData challengeData,
    object userData)
  {
    BnetPlayer opponent = (BnetPlayer) userData;
    string message = (string) null;
    switch (challengeEvent)
    {
      case PartyManager.PartyInviteEvent.I_CREATED_PARTY:
        this.ShowMercenaryFriendlyChallengeWaitForOpponent(opponent);
        break;
      case PartyManager.PartyInviteEvent.I_RESCINDED_INVITE:
      case PartyManager.PartyInviteEvent.INVITE_EXPIRED:
      case PartyManager.PartyInviteEvent.FRIEND_LEFT:
      case PartyManager.PartyInviteEvent.LEADER_DISSOLVED_PARTY:
        if ((BnetEntityId) playerGameAccountId == (BnetEntityId) opponent.GetBestGameAccountId())
        {
          message = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_QUEUE_CANCELED");
          break;
        }
        break;
      case PartyManager.PartyInviteEvent.FRIEND_ACCEPTED_INVITE:
        DialogManager.Get().ClearAllImmediately();
        this.RemoveChangedListener(new PartyManager.ChangedCallback(this.HandleMercenaryFriendlyChallengeNotifications), userData);
        this.NavigateToMercenaryFriendlyChallengeScreen();
        break;
      case PartyManager.PartyInviteEvent.FRIEND_DECLINED_INVITE:
        message = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_OPPONENT_DECLINED", (object) opponent.GetBestName());
        break;
    }
    if (message == null)
      return;
    this.ShowMercenaryFriendlyChallengeCanceled(message, userData);
  }

  private void ShowMercenaryFriendlyChallengeWaitForOpponent(BnetPlayer opponent) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
    m_text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_OPPONENT_WAITING_RESPONSE", (object) opponent.GetBestName()),
    m_showAlertIcon = false,
    m_responseCallback = new AlertPopup.ResponseCallback(this.OnMercenaryFriendlyChallengeCancelPressed),
    m_responseUserData = (object) opponent,
    m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL,
    m_layerToUse = new GameLayer?(GameLayer.UI)
  });

  private void OnMercenaryFriendlyChallengeCancelPressed(
    AlertPopup.Response response,
    object userData)
  {
    this.RemoveChangedListener(new PartyManager.ChangedCallback(this.HandleMercenaryFriendlyChallengeNotifications), userData);
    this.LeaveParty();
  }

  private void ShowMercenaryFriendlyChallengeCanceled(string message, object userData)
  {
    DialogManager.Get().ClearAllImmediately();
    this.RemoveChangedListener(new PartyManager.ChangedCallback(this.HandleMercenaryFriendlyChallengeNotifications), userData);
    this.ShowSimpleAlertDialog(GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"), message);
    this.LeaveParty();
  }

  private void NavigateToMercenaryFriendlyChallengeScreen()
  {
    GameMgr.Get().SetPendingAutoConcede(true);
    CollectionManager collectionManager = CollectionManager.Get();
    if (collectionManager.IsInEditMode())
      collectionManager.GetEditedDeck()?.SendChanges(CollectionDeck.ChangeSource.NavigateToSceneForPartyChallenge);
    SceneMgr.Get().SetNextMode(SceneMgr.Mode.LETTUCE_FRIENDLY);
  }

  private void ShowSimpleAlertDialog(string header, string body, bool showAlertIcon = false) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get(header),
    m_text = GameStrings.Get(body),
    m_responseDisplay = AlertPopup.ResponseDisplay.OK,
    m_showAlertIcon = showAlertIcon,
    m_okText = GameStrings.Get("GLOBAL_OKAY")
  });

  private bool OnInviteReceivedDialogProcessed(DialogBase dialog, object userData)
  {
    this.m_inviteDialog = dialog;
    return true;
  }

  private void OnInviteReceivedDialogResponse(bool accept)
  {
    BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
    if (accept)
    {
      if (BnetPresenceMgr.Get().GetMyPlayer().IsAppearingOffline())
      {
        this.DeclinePartyInvite(this.m_partyData.m_inviteId);
        this.ShowSimpleAlertDialog("GLUE_BACON_INVITE_WHILE_APPEARING_OFFLINE_HEADER", "GLUE_BACON_INVITE_WHILE_APPEARING_OFFLINE", true);
      }
      else if (this.m_pendingParty != (BnetPartyId) null && !this.IsInParty())
      {
        this.m_partyData.m_partyId = this.m_pendingParty;
        BnetParty.AcceptReceivedInvite(this.m_partyData.m_inviteId);
        this.UpdateMyAvailability();
        this.FireChangedEvent(PartyManager.PartyInviteEvent.I_ACCEPTED_INVITE, myGameAccountId);
        this.TransitionModeIfNeeded();
      }
      else if (this.IsInParty())
        this.ShowSimpleAlertDialog("GLUE_BACON_EXPIRED_INVITE_HEADER", "GLUE_BACON_PARTY_INVITE_WHILE_IN_PARTY");
      else
        this.ShowSimpleAlertDialog("GLUE_BACON_EXPIRED_INVITE_HEADER", "GLUE_BACON_EXPIRD_INVITE_BODY");
    }
    else
      this.DeclinePartyInvite(this.m_partyData.m_inviteId);
    this.m_inviteDialog = (DialogBase) null;
    this.m_pendingParty = (BnetPartyId) null;
    FriendChallengeMgr.Get().UpdateMyAvailability();
  }

  private void DeclinePartyInvite(ulong inviteId)
  {
    BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
    BnetParty.DeclineReceivedInvite(inviteId);
    this.FireChangedEvent(PartyManager.PartyInviteEvent.I_DECLINED_INVITE, myGameAccountId);
    this.m_pendingParty = (BnetPartyId) null;
  }

  private void OnBattlegroundsSuggestionReceivedResponse(
    bool accept,
    BnetGameAccountId playerToInvite)
  {
    if (!accept)
      return;
    this.InvitePlayerToBattlegroundsParty(playerToInvite);
  }

  private void TransitionModeIfNeeded()
  {
    SceneMgr.Mode mode1 = this.GetMode(this.m_partyData.m_type);
    SceneMgr.Mode mode2 = SceneMgr.Get().GetMode();
    if (mode1 == mode2)
      return;
    SceneMgr.Get().SetNextMode(mode1);
  }

  private void OnPresenceUpdated(BnetPlayerChangelist changelist, object userData)
  {
    foreach (BnetPlayerChange change in changelist.GetChanges())
    {
      BnetPlayer player = change.GetPlayer();
      BnetGameAccountId bestGameAccountId = player.GetBestGameAccountId();
      if (this.IsPlayerInCurrentPartyOrPending(bestGameAccountId) && !player.IsOnline())
        this.KickPlayerFromParty(bestGameAccountId);
    }
  }

  private void OnFatalError(FatalErrorMessage message, object userData) => this.ClearPartyData();

  private void OnLoginComplete() => this.UpdateMyAvailability();

  private ulong? GetPendingInviteIdFromGameAccount(BnetGameAccountId gameAccountId)
  {
    foreach (PartyInvite pendingInvite in this.GetPendingInvites())
    {
      if ((BnetEntityId) pendingInvite.InviteeId == (BnetEntityId) gameAccountId)
        return new ulong?(pendingInvite.InviteId);
    }
    return new ulong?();
  }

  private void BnetParty_OnJoined(OnlineEventType evt, PartyInfo party, LeaveReason? reason)
  {
    if (!this.ShouldSupportPartyType(party.Type) || party.Id != this.m_partyData.m_partyId)
      return;
    if (evt == OnlineEventType.ADDED)
    {
      this.m_partyData.m_partyId = party.Id;
      this.UpdateMyAvailability();
      this.InitializePersonalPartyMemberAttributes(party.Type);
      long num1;
      if (BattleNet.GetPartyAttribute<long>(party.Id, "WTCG.Game.ScenarioId", out num1))
        this.m_partyData.m_scenarioId = (ScenarioDbId) num1;
      long num2;
      if (BattleNet.GetPartyAttribute<long>(party.Id, "WTCG.Format.Type", out num2))
        this.m_partyData.m_format = (FormatType) num2;
      int num3;
      if (BattleNet.GetPartyAttribute<int>(party.Id, "WTCG.Season.Id", out num3))
        this.m_partyData.m_season = num3;
    }
    if (evt != OnlineEventType.REMOVED)
      return;
    this.ClearPartyData();
    this.UpdateMyAvailability();
    LeaveReason? nullable = reason;
    if (nullable.HasValue)
    {
      switch (nullable.GetValueOrDefault())
      {
        case LeaveReason.MEMBER_KICKED:
          this.ShowSimpleAlertDialog("GLUE_BACON_PARTY_KICKED_HEADER", "GLUE_BACON_PARTY_KICKED_BODY");
          break;
        case LeaveReason.DISSOLVED_BY_MEMBER:
        case LeaveReason.DISSOLVED_BY_SERVICE:
          this.ShowSimpleAlertDialog("GLUE_BACON_PARTY_DISBANDED_HEADER", "GLUE_BACON_PARTY_DISBANDED_BODY");
          break;
      }
    }
    this.FireChangedEvent(PartyManager.PartyInviteEvent.LEADER_DISSOLVED_PARTY, (BnetGameAccountId) null);
  }

  private void BnetParty_OnReceivedInvite(
    OnlineEventType evt,
    PartyInfo party,
    ulong inviteId,
    BnetGameAccountId inviter,
    string inviterBattleTag,
    BnetGameAccountId invitee,
    InviteRemoveReason? reason)
  {
    if (!this.ShouldSupportPartyType(party.Type))
      return;
    switch (evt)
    {
      case OnlineEventType.ADDED:
        if (!PartyManager.IsPartyTypeEnabledInGuardian(party.Type))
        {
          this.DeclinePartyInvite(inviteId);
          return;
        }
        if (!FriendChallengeMgr.Get().AmIAvailable())
        {
          this.DeclinePartyInvite(inviteId);
          return;
        }
        if (!this.HasCompletedRequiredTutorialForPartyType(party.Type))
        {
          this.DeclinePartyInvite(inviteId);
          return;
        }
        this.m_partyData.m_inviteId = inviteId;
        this.m_partyData.m_type = party.Type;
        this.m_pendingParty = party.Id;
        this.ShowInviteDialog(inviter, inviterBattleTag, party.Type);
        break;
      case OnlineEventType.REMOVED:
        this.m_pendingParty = (BnetPartyId) null;
        if ((UnityEngine.Object) this.m_inviteDialog != (UnityEngine.Object) null)
        {
          PartyType partyType = party.Type;
          this.m_inviteDialog.AddHiddenOrDestroyedListener((DialogBase.HideCallback) ((dialog, o) =>
          {
            this.m_inviteDialog = (DialogBase) null;
            FriendChallengeMgr.Get().UpdateMyAvailability();
            if (partyType != PartyType.MERCENARIES_FRIENDLY_CHALLENGE)
              return;
            this.m_pendingParty = (BnetPartyId) null;
            this.ShowSimpleAlertDialog(GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"), GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_QUEUE_CANCELED"));
          }));
          if (partyType == PartyType.MERCENARIES_FRIENDLY_CHALLENGE)
          {
            this.m_inviteDialog.Hide();
            break;
          }
          break;
        }
        break;
    }
    FriendChallengeMgr.Get().UpdateMyAvailability();
  }

  private void BnetParty_OnMemberEvent(
    OnlineEventType evt,
    PartyInfo party,
    BnetGameAccountId memberId,
    bool isRolesUpdate,
    LeaveReason? reason)
  {
    if (party == null)
    {
      Log.Party.PrintError("PartyManager.BnetParty_OnMemberEvent() received empty party info.");
      TelemetryManager.Client().SendLiveIssue("PartyManager.BnetParty_OnMemberEvent", "Party info is null.");
    }
    else
    {
      if (!this.ShouldSupportPartyType(party.Type) || party.Id != this.m_partyData.m_partyId)
        return;
      BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
      Log.Party.PrintDebug("PartyManager.BnetParty_OnMemberEvent() received event {0} for member {1}", (object) evt.ToString(), (object) memberId.ToString());
      if (evt == OnlineEventType.REMOVED && BnetParty.IsInParty(party.Id) && (BnetEntityId) memberId != (BnetEntityId) myGameAccountId)
      {
        this.FireChangedEvent(PartyManager.PartyInviteEvent.FRIEND_LEFT, memberId);
        BnetGameAccountId leader = this.GetLeader();
        if (!((BnetEntityId) leader == (BnetEntityId) null) && !((BnetEntityId) leader == (BnetEntityId) memberId))
          return;
        this.LeaveParty();
        this.FireChangedEvent(PartyManager.PartyInviteEvent.LEADER_DISSOLVED_PARTY, memberId);
      }
      else
      {
        if (evt != OnlineEventType.ADDED || !BnetParty.IsInParty(party.Id) || !((BnetEntityId) memberId != (BnetEntityId) myGameAccountId))
          return;
        this.FireChangedEvent(PartyManager.PartyInviteEvent.FRIEND_ACCEPTED_INVITE, memberId);
      }
    }
  }

  private void BnetParty_OnPartyAttributeChanged(PartyInfo party, Blizzard.GameService.Protocol.V2.Client.Attribute attribute)
  {
    if (party == null)
    {
      Log.Party.PrintError("PartyManager.BnetParty_OnPartyAttributeChanged() received empty party info.");
      TelemetryManager.Client().SendLiveIssue("PartyManager.BnetParty_OnPartyAttributeChanged", "Party info is null.");
    }
    else
    {
      if (!this.ShouldSupportPartyType(party.Type) || this.m_partyData.m_partyId != party.Id)
        return;
      string name = attribute.Name;
      if (!(name == "queue"))
      {
        if (name == "canceled_by" && attribute.Value.HasBlobValue)
        {
          BnetId from = ProtobufUtil.ParseFrom<BnetId>(attribute.Value.BlobValue.ToArray<byte>());
          BnetGameAccountId playerGameAccountId = new BnetGameAccountId(from.Hi, from.Lo);
          BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
          if (!((BnetEntityId) playerGameAccountId == (BnetEntityId) myGameAccountId))
          {
            string partyMemberName = this.GetPartyMemberName(playerGameAccountId);
            DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
            {
              m_headerText = GameStrings.Get("GLUE_BACON_PRIVATE_PARTY_TITLE"),
              m_text = GameStrings.Format("GLUE_BACON_QUEUE_CANCELED", (object) "5ecaf0ff", (object) partyMemberName),
              m_responseDisplay = AlertPopup.ResponseDisplay.OK,
              m_showAlertIcon = false,
              m_alertTextAlignment = UberText.AlignmentOptions.Center,
              m_okText = GameStrings.Get("GLOBAL_OKAY")
            });
          }
        }
      }
      else
      {
        if (attribute.Value.HasStringValue && attribute.Value.StringValue.Equals("in_queue"))
        {
          Shop.Get().Close(true);
          this.WaitForGame();
        }
        if (attribute.Value.HasStringValue && attribute.Value.StringValue.Equals("cancel_queue"))
          GameMgr.Get().CancelFindGame();
      }
      this.FirePartyAttributeChangedEvent(attribute);
    }
  }

  private void BnetParty_OnMemberAttributeChanged(
    PartyInfo party,
    BnetGameAccountId partyMember,
    Blizzard.GameService.Protocol.V2.Client.Attribute attribute)
  {
    if (party == null)
    {
      Log.Party.PrintError("PartyManager.BnetParty_OnMemberAttributeChanged() received empty party info.");
      TelemetryManager.Client().SendLiveIssue("PartyManager.BnetParty_OnMemberAttributeChanged", "Party info is null.");
    }
    else
    {
      if (!this.ShouldSupportPartyType(party.Type) || this.m_partyData.m_partyId != party.Id)
        return;
      Log.Party.PrintDebug("PartyManager.BnetParty_OnMemberAttributeChanged() - " + attribute.ToString());
      this.FireMemberAttributeChangedEvent(partyMember, attribute);
    }
  }

  private void BnetParty_OnSentInvite(
    OnlineEventType evt,
    PartyInfo party,
    ulong inviteId,
    BnetGameAccountId inviter,
    BnetGameAccountId invitee,
    bool senderIsMyself,
    InviteRemoveReason? reason)
  {
    if (!this.ShouldSupportPartyType(party.Type) || this.m_partyData.m_partyId != party.Id)
      return;
    if (evt == OnlineEventType.ADDED)
    {
      if ((BnetEntityId) inviter != (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId())
        this.FireChangedEvent(PartyManager.PartyInviteEvent.I_SENT_INVITE, invitee);
      else
        this.FireChangedEvent(PartyManager.PartyInviteEvent.FRIEND_RECEIVED_INVITE, invitee);
    }
    if (evt != OnlineEventType.REMOVED)
      return;
    InviteRemoveReason? nullable = reason;
    if (!nullable.HasValue)
      return;
    switch (nullable.GetValueOrDefault())
    {
      case InviteRemoveReason.DECLINED:
        this.FireChangedEvent(PartyManager.PartyInviteEvent.FRIEND_DECLINED_INVITE, invitee);
        this.ShowDeclinedInvitationPopup(invitee);
        break;
      case InviteRemoveReason.REVOKED:
      case InviteRemoveReason.EXPIRED:
      case InviteRemoveReason.CANCELED:
        this.FireChangedEvent(PartyManager.PartyInviteEvent.INVITE_EXPIRED, invitee);
        break;
    }
  }

  private void BnetParty_OnReceivedInviteRequest(
    OnlineEventType evt,
    PartyInfo party,
    InviteRequest request,
    InviteRequestRemovedReason? reason)
  {
    if (!this.ShouldSupportPartyType(party.Type) || this.m_partyData.m_partyId != party.Id || !BnetParty.IsLeader(party.Id) || BnetParty.IsMember(party.Id, request.TargetId))
      return;
    DialogManager.Get().ShowBattlegroundsSuggestion(request.TargetId, request.TargetName, request.RequesterId, request.RequesterName, new BattlegroundsSuggestDialog.ResponseCallback(this.OnBattlegroundsSuggestionReceivedResponse));
  }

  private void FireChangedEvent(
    PartyManager.PartyInviteEvent challengeEvent,
    BnetGameAccountId playerGameAccountId)
  {
    foreach (PartyManager.ChangedListener changedListener in this.m_changedListeners.ToArray())
      changedListener.Fire(challengeEvent, playerGameAccountId, this.m_partyData);
  }

  public bool AddChangedListener(PartyManager.ChangedCallback callback) => this.AddChangedListener(callback, (object) null);

  public bool AddChangedListener(PartyManager.ChangedCallback callback, object userData)
  {
    PartyManager.ChangedListener changedListener = new PartyManager.ChangedListener();
    changedListener.SetCallback(callback);
    changedListener.SetUserData(userData);
    if (this.m_changedListeners.Contains(changedListener))
      return false;
    this.m_changedListeners.Add(changedListener);
    return true;
  }

  public bool RemoveChangedListener(PartyManager.ChangedCallback callback) => this.RemoveChangedListener(callback, (object) null);

  public bool RemoveChangedListener(PartyManager.ChangedCallback callback, object userData)
  {
    PartyManager.ChangedListener changedListener = new PartyManager.ChangedListener();
    changedListener.SetCallback(callback);
    changedListener.SetUserData(userData);
    return this.m_changedListeners.Remove(changedListener);
  }

  private void FireMemberAttributeChangedEvent(
    BnetGameAccountId playerGameAccountId,
    Blizzard.GameService.Protocol.V2.Client.Attribute attribute)
  {
    foreach (PartyManager.MemberAttributeChangedListener attributeChangedListener in this.m_memberAttributeChangedListeners.ToArray())
      attributeChangedListener.Fire(playerGameAccountId, attribute);
  }

  public bool AddMemberAttributeChangedListener(
    PartyManager.MemberAttributeChangedCallback callback)
  {
    return this.AddMemberAttributeChangedListener(callback, (object) null);
  }

  public bool AddMemberAttributeChangedListener(
    PartyManager.MemberAttributeChangedCallback callback,
    object userData)
  {
    PartyManager.MemberAttributeChangedListener attributeChangedListener = new PartyManager.MemberAttributeChangedListener();
    attributeChangedListener.SetCallback(callback);
    attributeChangedListener.SetUserData(userData);
    if (this.m_memberAttributeChangedListeners.Contains(attributeChangedListener))
      return false;
    this.m_memberAttributeChangedListeners.Add(attributeChangedListener);
    return true;
  }

  public bool RemoveMemberAttributeChangedListener(
    PartyManager.MemberAttributeChangedCallback callback)
  {
    return this.RemoveMemberAttributeChangedListener(callback, (object) null);
  }

  public bool RemoveMemberAttributeChangedListener(
    PartyManager.MemberAttributeChangedCallback callback,
    object userData)
  {
    PartyManager.MemberAttributeChangedListener attributeChangedListener = new PartyManager.MemberAttributeChangedListener();
    attributeChangedListener.SetCallback(callback);
    attributeChangedListener.SetUserData(userData);
    return this.m_memberAttributeChangedListeners.Remove(attributeChangedListener);
  }

  private void FirePartyAttributeChangedEvent(Blizzard.GameService.Protocol.V2.Client.Attribute attribute)
  {
    foreach (PartyManager.PartyAttributeChangedListener attributeChangedListener in this.m_partyAttributeChangedListeners.ToArray())
      attributeChangedListener.Fire(attribute);
  }

  public bool AddPartyAttributeChangedListener(
    PartyManager.PartyAttributeChangedCallback callback)
  {
    return this.AddPartyAttributeChangedListener(callback, (object) null);
  }

  public bool AddPartyAttributeChangedListener(
    PartyManager.PartyAttributeChangedCallback callback,
    object userData)
  {
    PartyManager.PartyAttributeChangedListener attributeChangedListener = new PartyManager.PartyAttributeChangedListener();
    attributeChangedListener.SetCallback(callback);
    attributeChangedListener.SetUserData(userData);
    if (this.m_partyAttributeChangedListeners.Contains(attributeChangedListener))
      return false;
    this.m_partyAttributeChangedListeners.Add(attributeChangedListener);
    return true;
  }

  public bool RemovePartyAttributeChangedListener(
    PartyManager.PartyAttributeChangedCallback callback)
  {
    return this.RemovePartyAttributeChangedListener(callback, (object) null);
  }

  public bool RemovePartyAttributeChangedListener(
    PartyManager.PartyAttributeChangedCallback callback,
    object userData)
  {
    PartyManager.PartyAttributeChangedListener attributeChangedListener = new PartyManager.PartyAttributeChangedListener();
    attributeChangedListener.SetCallback(callback);
    attributeChangedListener.SetUserData(userData);
    return this.m_partyAttributeChangedListeners.Remove(attributeChangedListener);
  }

  public enum MercTeamShareState
  {
    NOT_SHARING,
    USING_LOCAL_TEAMS,
    USING_REMOTE_TEAMS,
  }

  public enum MercTeamSharingButtonStatus
  {
    ENABLED,
    DISABLED,
  }

  public enum MercTeamShareMSG
  {
    NO_MSG,
    REQUEST_SHARING,
    SHARING_REQUEST_CANCELLED,
    SHARING_REQUEST_DENIED,
  }

  public enum PartyInviteEvent
  {
    I_CREATED_PARTY,
    I_SENT_INVITE,
    I_RESCINDED_INVITE,
    FRIEND_RECEIVED_INVITE,
    FRIEND_ACCEPTED_INVITE,
    FRIEND_DECLINED_INVITE,
    INVITE_EXPIRED,
    I_ACCEPTED_INVITE,
    I_DECLINED_INVITE,
    FRIEND_RESCINDED_INVITE,
    FRIEND_LEFT,
    LEADER_DISSOLVED_PARTY,
  }

  public class PartyData
  {
    public PartyType m_type;
    public ulong m_inviteId;
    public BnetPartyId m_partyId;
    public ScenarioDbId m_scenarioId;
    public FormatType m_format;
    public int m_season;
  }

  private class ChangedListener : EventListener<PartyManager.ChangedCallback>
  {
    public void Fire(
      PartyManager.PartyInviteEvent challengeEvent,
      BnetGameAccountId playerGameAccountId,
      PartyManager.PartyData challengeData)
    {
      this.m_callback(challengeEvent, playerGameAccountId, challengeData, this.m_userData);
    }
  }

  public delegate void ChangedCallback(
    PartyManager.PartyInviteEvent challengeEvent,
    BnetGameAccountId playerGameAccountId,
    PartyManager.PartyData challengeData,
    object userData);

  private class MemberAttributeChangedListener : 
    EventListener<PartyManager.MemberAttributeChangedCallback>
  {
    public void Fire(BnetGameAccountId playerGameAccountId, Blizzard.GameService.Protocol.V2.Client.Attribute attribute) => this.m_callback(playerGameAccountId, attribute, this.m_userData);
  }

  public delegate void MemberAttributeChangedCallback(
    BnetGameAccountId playerGameAccountId,
    Blizzard.GameService.Protocol.V2.Client.Attribute attribute,
    object userData);

  private class PartyAttributeChangedListener : 
    EventListener<PartyManager.PartyAttributeChangedCallback>
  {
    public void Fire(Blizzard.GameService.Protocol.V2.Client.Attribute attribute) => this.m_callback(attribute, this.m_userData);
  }

  public delegate void PartyAttributeChangedCallback(Blizzard.GameService.Protocol.V2.Client.Attribute attribute, object userData);
}
