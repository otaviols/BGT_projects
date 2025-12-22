using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core;
using Blizzard.T5.Core.Time;
using Blizzard.T5.Core.Utils;
using Blizzard.T5.Services;
using Hearthstone;
using Hearthstone.Core;
using PegasusGame;
using PegasusShared;
using SpectatorProto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class SpectatorManager
{
  private static readonly PlatformDependentValue<float> WAITING_FOR_NEXT_GAME_AUTO_LEAVE_SECONDS = new PlatformDependentValue<float>(PlatformCategory.OS)
  {
    iOS = 300f,
    Android = 300f,
    PC = -1f,
    Mac = -1f
  };
  private static readonly PlatformDependentValue<bool> DISABLE_MENU_BUTTON_WHILE_WAITING = new PlatformDependentValue<bool>(PlatformCategory.OS)
  {
    iOS = true,
    Android = true,
    PC = false,
    Mac = false
  };
  private static SpectatorManager s_instance = (SpectatorManager) null;
  private bool m_initialized;
  private BnetGameAccountId m_spectateeFriendlySide;
  private BnetGameAccountId m_spectateeOpposingSide;
  private BnetPartyId m_spectatorPartyIdMain;
  private BnetPartyId m_spectatorPartyIdOpposingSide;
  private Map<BnetPartyId, BnetGameAccountId> m_knownPartyCreatorIds = new Map<BnetPartyId, BnetGameAccountId>();
  private SpectatorManager.IntendedSpectateeParty m_requestedInvite;
  private AlertPopup m_waitingForNextGameDialog;
  private HashSet<BnetPartyId> m_leavePartyIdsRequested;
  private SpectatorManager.PendingSpectatePlayer m_pendingSpectatePlayerAfterLeave;
  private HashSet<BnetGameAccountId> m_userInitiatedOutgoingInvites;
  private HashSet<BnetGameAccountId> m_kickedPlayers;
  private Map<BnetGameAccountId, uint> m_kickedFromSpectatingList;
  private int? m_expectedDisconnectReason;
  private bool m_isExpectingArriveInGameplayAsSpectator;
  private bool m_isShowingRemovedAsSpectatorPopup;
  private HashSet<BnetGameAccountId> m_gameServerKnownSpectators = new HashSet<BnetGameAccountId>();
  private Map<BnetGameAccountId, SpectatorManager.ReceivedInvite> m_receivedSpectateMeInvites = new Map<BnetGameAccountId, SpectatorManager.ReceivedInvite>();
  private Map<BnetGameAccountId, float> m_sentSpectateMeInvites = new Map<BnetGameAccountId, float>();

  public event SpectatorManager.InviteReceivedHandler OnInviteReceived;

  public event SpectatorManager.InviteSentHandler OnInviteSent;

  public event System.Action OnSpectateRejected;

  public event SpectatorManager.SpectatorToMyGameHandler OnSpectatorToMyGame;

  public event SpectatorManager.SpectatorModeChangedHandler OnSpectatorModeChanged;

  public static SpectatorManager Get()
  {
    if (SpectatorManager.s_instance == null && SceneMgr.Get() != null)
      SpectatorManager.CreateInstance();
    return SpectatorManager.s_instance;
  }

  public static bool InstanceExists() => SpectatorManager.s_instance != null;

  public static JoinInfo GetSpectatorJoinInfo(BnetGameAccount gameAccount)
  {
    if (gameAccount == (BnetGameAccount) null)
      return (JoinInfo) null;
    byte[] gameFieldBytes1 = gameAccount.GetGameFieldBytes(21U);
    if (gameFieldBytes1 != null && gameFieldBytes1.Length != 0)
      return ProtobufUtil.ParseFrom<JoinInfo>(gameFieldBytes1);
    byte[] gameFieldBytes2 = gameAccount.GetGameFieldBytes(23U);
    if (gameFieldBytes2 != null)
    {
      if (gameFieldBytes2.Length != 0)
      {
        try
        {
          SecretJoinInfo from = ProtobufUtil.ParseFrom<SecretJoinInfo>(gameFieldBytes2);
          if (from != null)
          {
            byte[] buffer = (byte[]) null;
            if (from.Source == SecretSource.SECRET_SOURCE_FIRESIDE_GATHERING && from.HasSpecificSourceIdentity && from.SpecificSourceIdentity == FiresideGatheringManager.Get().CurrentFsgId)
              buffer = FiresideGatheringManager.Get().CurrentFsgSharedSecretKey;
            if (buffer != null)
            {
              byte[] hash = SHA256.Create().ComputeHash(buffer, 0, buffer.Length);
              return ProtobufUtil.ParseFrom<JoinInfo>(Crypto.Rijndael.Decrypt(from.EncryptedMessage, hash));
            }
          }
        }
        catch (Exception ex)
        {
          Log.All.PrintError("{0} parsing/decrypting secret JoinInfo, isInFsg={1}: {2}", (object) ex.GetType().Name, (object) FiresideGatheringManager.Get().IsCheckedIn, (object) ex.ToString());
        }
      }
    }
    return PartyManager.Get().GetSpectatorJoinInfoForPlayer(gameAccount.GetId()) ?? (JoinInfo) null;
  }

  public static int GetSpectatorGameHandleFromPlayer(BnetPlayer player)
  {
    JoinInfo spectatorJoinInfo = SpectatorManager.GetSpectatorJoinInfo(player.GetHearthstoneGameAccount());
    return spectatorJoinInfo == null ? -1 : spectatorJoinInfo.GameHandle;
  }

  public static bool IsSpectatorSlotAvailable(JoinInfo info) => info != null && (info.HasPartyId || info.HasServerIpAddress && info.HasSecretKey && !string.IsNullOrEmpty(info.SecretKey)) && (!info.HasIsJoinable || info.IsJoinable) && (!info.HasMaxNumSpectators || !info.HasCurrentNumSpectators || info.CurrentNumSpectators < info.MaxNumSpectators);

  public void InitializeConnectedToBnet()
  {
    if (this.m_initialized)
      return;
    this.m_initialized = true;
    foreach (PartyInfo joinedParty in BnetParty.GetJoinedParties())
      this.BnetParty_OnJoined(OnlineEventType.ADDED, joinedParty, new LeaveReason?());
    foreach (PartyInvite receivedInvite in BnetParty.GetReceivedInvites())
      this.BnetParty_OnReceivedInvite(OnlineEventType.ADDED, new PartyInfo(receivedInvite.PartyId, receivedInvite.PartyType), receivedInvite.InviteId, receivedInvite.InviterId, receivedInvite.InviterName, receivedInvite.InviteeId, new InviteRemoveReason?());
  }

  private bool IsInSpectableContextWithPlayer(BnetGameAccountId gameAccountId) => this.IsInSpectableContextWithPlayer(BnetPresenceMgr.Get().GetPlayer(gameAccountId));

  private bool IsInSpectableContextWithPlayer(BnetPlayer player) => player != null && (BnetFriendMgr.Get().IsFriend(player) || FiresideGatheringManager.Get().IsPlayerInMyFSG(player) || PartyManager.Get().IsPlayerInCurrentPartyOrPending(player.GetBestGameAccountId()));

  public bool CanSpectate(BnetPlayer player)
  {
    if (player == null)
      return false;
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    if (myPlayer == null || player == myPlayer || !this.IsInSpectableContextWithPlayer(player))
      return false;
    BnetGameAccount hearthstoneGameAccount1 = player.GetHearthstoneGameAccount();
    BnetGameAccount hearthstoneGameAccount2 = myPlayer.GetHearthstoneGameAccount();
    return !(hearthstoneGameAccount1 == (BnetGameAccount) null) && !(hearthstoneGameAccount2 == (BnetGameAccount) null) && hearthstoneGameAccount1.IsOnline() && hearthstoneGameAccount2.IsOnline() && (!HearthstoneApplication.IsPublic() || string.Compare(hearthstoneGameAccount1.GetClientVersion(), hearthstoneGameAccount2.GetClientVersion()) == 0 && string.Compare(hearthstoneGameAccount1.GetClientEnv(), hearthstoneGameAccount2.GetClientEnv()) == 0) && this.CanSpectate(player.GetHearthstoneGameAccountId(), SpectatorManager.GetSpectatorJoinInfo(hearthstoneGameAccount1));
  }

  private bool CanSpectateMultiplePlayersSimultaneously(GameType gameType) => !GameUtils.IsMercenariesGameType(gameType);

  public bool CanSpectate(BnetGameAccountId gameAccountId, JoinInfo joinInfo)
  {
    if (this.IsSpectatingPlayer(gameAccountId) || (BnetEntityId) this.m_spectateeOpposingSide != (BnetEntityId) null || this.HasPreviouslyKickedMeFromGame(gameAccountId, joinInfo == null ? -1 : joinInfo.GameHandle) && !this.HasInvitedMeToSpectate(gameAccountId) || GameMgr.Get().IsFindingGame() || GameMgr.Get().IsNextSpectator() || FriendChallengeMgr.Get().HasChallenge() || !SpectatorManager.IsSpectatorSlotAvailable(joinInfo) && !this.HasInvitedMeToSpectate(gameAccountId))
      return false;
    if (GameMgr.Get().IsSpectator())
    {
      if (!this.IsPlayerInGame(gameAccountId) || joinInfo != null && !this.CanSpectateMultiplePlayersSimultaneously(joinInfo.GameType))
        return false;
    }
    else if (SceneMgr.Get().IsInGame())
      return false;
    return GameUtils.IsAnyTutorialComplete() && SceneMgr.Get().GetMode() != SceneMgr.Mode.LOGIN && !BnetPresenceMgr.Get().GetMyPlayer().IsAppearingOffline() && (!PartyManager.Get().IsInParty() || PartyManager.Get().IsPlayerInCurrentPartyOrPending(gameAccountId));
  }

  public bool IsSpectatingOrWatching => GameMgr.Get() != null && GameMgr.Get().IsSpectator() || this.IsInSpectatorMode();

  public bool IsInSpectatorMode() => !((BnetEntityId) this.m_spectateeFriendlySide == (BnetEntityId) null) && !(this.m_spectatorPartyIdMain == (BnetPartyId) null) && this.IsStillInParty(this.m_spectatorPartyIdMain) && this.m_initialized && !((BnetEntityId) this.GetPartyCreator(this.m_spectatorPartyIdMain) == (BnetEntityId) null) && !this.ShouldBePartyLeader(this.m_spectatorPartyIdMain);

  public bool IsSpectatingPlayer(BnetGameAccountId gameAccountId) => (BnetEntityId) this.m_spectateeFriendlySide != (BnetEntityId) null && (BnetEntityId) this.m_spectateeFriendlySide == (BnetEntityId) gameAccountId || (BnetEntityId) this.m_spectateeOpposingSide != (BnetEntityId) null && (BnetEntityId) this.m_spectateeOpposingSide == (BnetEntityId) gameAccountId;

  public bool IsSpectatingMe(BnetGameAccountId gameAccountId) => !this.IsInSpectatorMode() && (this.m_gameServerKnownSpectators.Contains(gameAccountId) || (BnetEntityId) gameAccountId != (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId() && BnetParty.IsMember(this.m_spectatorPartyIdMain, gameAccountId));

  public int GetCountSpectatingMe()
  {
    if (this.m_spectatorPartyIdMain != (BnetPartyId) null && !this.ShouldBePartyLeader(this.m_spectatorPartyIdMain))
      return 0;
    int count = this.m_gameServerKnownSpectators.Count;
    return Mathf.Max(BnetParty.CountMembers(this.m_spectatorPartyIdMain) - 1, count);
  }

  public bool IsBeingSpectated() => this.GetCountSpectatingMe() > 0;

  public BnetGameAccountId[] GetSpectatorPartyMembers(
    bool friendlySide = true,
    bool includeSelf = false)
  {
    List<BnetGameAccountId> bnetGameAccountIdList = new List<BnetGameAccountId>((IEnumerable<BnetGameAccountId>) this.m_gameServerKnownSpectators);
    BnetParty.PartyMember[] members = BnetParty.GetMembers(friendlySide ? this.m_spectatorPartyIdMain : this.m_spectatorPartyIdOpposingSide);
    BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
    foreach (BnetParty.PartyMember partyMember in members)
    {
      if ((includeSelf || (BnetEntityId) partyMember.GameAccountId != (BnetEntityId) myGameAccountId) && !bnetGameAccountIdList.Contains(partyMember.GameAccountId))
        bnetGameAccountIdList.Add(partyMember.GameAccountId);
    }
    return bnetGameAccountIdList.ToArray();
  }

  public bool IsInSpectatableGame() => SceneMgr.Get().IsInGame() && !GameMgr.Get().IsSpectator() && !SpectatorManager.IsGameOver;

  private bool IsInSpectatableScene(bool alsoCheckRequestedScene) => SceneMgr.Get().IsInGame() || SpectatorManager.IsSpectatableScene(SceneMgr.Get().GetMode()) || alsoCheckRequestedScene && SpectatorManager.IsSpectatableScene(SceneMgr.Get().GetNextMode());

  private static bool IsSpectatableScene(SceneMgr.Mode sceneMode) => sceneMode == SceneMgr.Mode.GAMEPLAY;

  public bool CanAddSpectators()
  {
    if (GameMgr.Get() != null && GameMgr.Get().IsSpectator() || (BnetEntityId) this.m_spectateeFriendlySide != (BnetEntityId) null || (BnetEntityId) this.m_spectateeOpposingSide != (BnetEntityId) null)
      return false;
    int countSpectatingMe = this.GetCountSpectatingMe();
    if (!this.IsInSpectatableGame() && (this.m_spectatorPartyIdMain == (BnetPartyId) null || countSpectatingMe <= 0) || countSpectatingMe >= 10)
      return false;
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    return myPlayer == null || !myPlayer.IsAppearingOffline();
  }

  public bool CanInviteToSpectateMyGame(BnetGameAccountId gameAccountId)
  {
    if (!this.CanAddSpectators())
      return false;
    BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
    if ((BnetEntityId) gameAccountId == (BnetEntityId) myGameAccountId || !this.IsInSpectableContextWithPlayer(gameAccountId) || this.IsSpectatingMe(gameAccountId) || this.IsInvitedToSpectateMyGame(gameAccountId) || PartyManager.Get().IsPlayerInAnyParty(gameAccountId))
      return false;
    BnetGameAccount gameAccount = BnetPresenceMgr.Get().GetGameAccount(gameAccountId);
    if (gameAccount == (BnetGameAccount) null || !gameAccount.IsOnline())
      return false;
    if (!gameAccount.CanBeInvitedToGame())
    {
      if (!this.IsPlayerSpectatingMyGamesOpposingSide(gameAccountId))
        return false;
      JoinInfo spectatorJoinInfo = SpectatorManager.GetSpectatorJoinInfo(gameAccount);
      if (spectatorJoinInfo != null && this.CanSpectateMultiplePlayersSimultaneously(spectatorJoinInfo.GameType))
        return false;
    }
    BnetGameAccount hearthstoneGameAccount = BnetPresenceMgr.Get().GetMyPlayer().GetHearthstoneGameAccount();
    return string.Compare(gameAccount.GetClientVersion(), hearthstoneGameAccount.GetClientVersion()) == 0 && (!HearthstoneApplication.IsPublic() || string.Compare(gameAccount.GetClientEnv(), hearthstoneGameAccount.GetClientEnv()) == 0) && SceneMgr.Get().IsInGame();
  }

  public bool IsPlayerSpectatingMyGamesOpposingSide(BnetGameAccountId gameAccountId)
  {
    BnetGameAccount gameAccount = BnetPresenceMgr.Get().GetGameAccount(gameAccountId);
    if (gameAccount == (BnetGameAccount) null)
      return false;
    BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
    bool flag = false;
    if (this.IsInSpectableContextWithPlayer(gameAccountId))
    {
      JoinInfo spectatorJoinInfo = SpectatorManager.GetSpectatorJoinInfo(gameAccount);
      Map<int, Player>.ValueCollection source = GameState.Get() == null ? (Map<int, Player>.ValueCollection) null : GameState.Get().GetPlayerMap().Values;
      if (spectatorJoinInfo != null && spectatorJoinInfo.SpectatedPlayers.Count > 0 && source != null && source.Count > 0)
      {
        for (int index = 0; index < spectatorJoinInfo.SpectatedPlayers.Count; ++index)
        {
          BnetGameAccountId spectatedPlayerId = BnetGameAccountId.CreateFromNet(spectatorJoinInfo.SpectatedPlayers[index]);
          if ((BnetEntityId) spectatedPlayerId != (BnetEntityId) myGameAccountId && source.Any<Player>((Func<Player, bool>) (p => (BnetEntityId) p.GetGameAccountId() == (BnetEntityId) spectatedPlayerId)))
          {
            flag = true;
            break;
          }
        }
      }
    }
    return flag;
  }

  public bool IsInvitedToSpectateMyGame(BnetGameAccountId gameAccountId) => ((IEnumerable<PartyInvite>) BnetParty.GetSentInvites(this.m_spectatorPartyIdMain)).FirstOrDefault<PartyInvite>((Func<PartyInvite, bool>) (i => (BnetEntityId) i.InviteeId == (BnetEntityId) gameAccountId)) != null;

  public bool CanKickSpectator(BnetGameAccountId gameAccountId) => this.IsSpectatingMe(gameAccountId);

  public bool HasInvitedMeToSpectate(BnetGameAccountId gameAccountId) => BnetParty.GetReceivedInviteFrom(gameAccountId, PartyType.SPECTATOR_PARTY) != null;

  public bool HasAnyReceivedInvites() => ((IEnumerable<PartyInvite>) BnetParty.GetReceivedInvites()).Where<PartyInvite>((Func<PartyInvite, bool>) (i => i.PartyType == PartyType.SPECTATOR_PARTY)).ToArray<PartyInvite>().Length != 0;

  public bool MyGameHasSpectators() => SceneMgr.Get().IsInGame() && this.m_gameServerKnownSpectators.Count > 0;

  public BnetGameAccountId GetSpectateeFriendlySide() => this.m_spectateeFriendlySide;

  public bool IsSpectatingOpposingSide() => (BnetEntityId) this.m_spectateeOpposingSide != (BnetEntityId) null;

  public bool HasPreviouslyKickedMeFromGame(BnetGameAccountId playerId, int currentGameHandle)
  {
    if (this.m_kickedFromSpectatingList == null)
      return false;
    uint num = 0;
    if (this.m_kickedFromSpectatingList.TryGetValue(playerId, out num))
    {
      if ((long) num == (long) currentGameHandle)
        return true;
      this.m_kickedFromSpectatingList.Remove(playerId);
      if (this.m_kickedFromSpectatingList.Count == 0)
        this.m_kickedFromSpectatingList = (Map<BnetGameAccountId, uint>) null;
    }
    return false;
  }

  public void SpectatePlayer(BnetPlayer player)
  {
    if (!this.CanSpectate(player))
      return;
    this.SpectatePlayer(player.GetHearthstoneGameAccountId(), SpectatorManager.GetSpectatorJoinInfo(player.GetHearthstoneGameAccount()));
  }

  public void SpectatePlayer(BnetGameAccountId gameAccountId, JoinInfo joinInfo)
  {
    if (this.m_pendingSpectatePlayerAfterLeave != null)
      return;
    PartyInvite receivedInviteFrom = BnetParty.GetReceivedInviteFrom(gameAccountId, PartyType.SPECTATOR_PARTY);
    if (receivedInviteFrom != null)
    {
      if ((BnetEntityId) this.m_spectateeFriendlySide == (BnetEntityId) null || (BnetEntityId) this.m_spectateeOpposingSide == (BnetEntityId) null && this.IsPlayerInGame(gameAccountId))
      {
        this.CloseWaitingForNextGameDialog();
        if ((BnetEntityId) this.m_spectateeFriendlySide != (BnetEntityId) null && (BnetEntityId) gameAccountId != (BnetEntityId) this.m_spectateeFriendlySide)
          this.m_spectateeOpposingSide = gameAccountId;
        BnetParty.AcceptReceivedInvite(receivedInviteFrom.InviteId);
        this.HideShownUI();
      }
      else
      {
        this.LogInfoParty("SpectatePlayer: trying to accept an invite even though there is no room for another spectatee: player={0} spectatee1={1} spectatee2={2} isPlayerInGame={3} inviteId={4}", (object) (gameAccountId.ToString() + " (" + BnetUtils.GetPlayerBestName(gameAccountId) + ")"), (object) this.m_spectateeFriendlySide, (object) this.m_spectateeOpposingSide, (object) this.IsPlayerInGame(gameAccountId), (object) receivedInviteFrom.InviteId);
        BnetParty.DeclineReceivedInvite(receivedInviteFrom.InviteId);
      }
    }
    else if (joinInfo == null)
      Error.AddWarningLoc("Bad Spectator Key", "Spectator key is blank!");
    else if (!joinInfo.HasPartyId && string.IsNullOrEmpty(joinInfo.SecretKey))
      Error.AddWarningLoc("No Party/Bad Spectator Key", "No party information and Spectator key is blank!");
    else if (joinInfo.HasPartyId && this.m_requestedInvite != null)
    {
      this.LogInfoParty("SpectatePlayer: already requesting invite from {0}:party={1}, cannot request another from {2}:party={3}", (object) this.m_requestedInvite.SpectateeId, (object) this.m_requestedInvite.PartyId, (object) gameAccountId, (object) BnetUtils.CreatePartyId(joinInfo.PartyId));
    }
    else
    {
      this.HideShownUI();
      if ((!((BnetEntityId) this.m_spectateeFriendlySide != (BnetEntityId) null) || !((BnetEntityId) this.m_spectateeOpposingSide == (BnetEntityId) null) || GameMgr.Get() == null ? 0 : (GameMgr.Get().IsSpectator() ? 1 : 0)) == 0)
      {
        if (this.m_spectatorPartyIdMain != (BnetPartyId) null)
        {
          if (this.IsInSpectatorMode())
            this.EndSpectatorMode(true);
          else
            this.LeaveParty(this.m_spectatorPartyIdMain, this.ShouldBePartyLeader(this.m_spectatorPartyIdMain));
          this.m_pendingSpectatePlayerAfterLeave = new SpectatorManager.PendingSpectatePlayer(gameAccountId, joinInfo);
          return;
        }
        if (this.m_spectatorPartyIdOpposingSide != (BnetPartyId) null)
        {
          this.m_pendingSpectatePlayerAfterLeave = new SpectatorManager.PendingSpectatePlayer(gameAccountId, joinInfo);
          this.LeaveParty(this.m_spectatorPartyIdOpposingSide, false);
          return;
        }
      }
      this.SpectatePlayer_Internal(gameAccountId, joinInfo);
    }
  }

  private void HideShownUI()
  {
    ShownUIMgr shownUiMgr = ShownUIMgr.Get();
    if (shownUiMgr == null)
      return;
    switch (shownUiMgr.GetShownUI())
    {
      case ShownUIMgr.UI_WINDOW.GENERAL_STORE:
        if (!((UnityEngine.Object) GeneralStore.Get() != (UnityEngine.Object) null))
          break;
        GeneralStore.Get().Close(false);
        break;
      case ShownUIMgr.UI_WINDOW.ARENA_STORE:
        if (!((UnityEngine.Object) ArenaStore.Get() != (UnityEngine.Object) null))
          break;
        ArenaStore.Get().Hide();
        break;
      case ShownUIMgr.UI_WINDOW.TAVERN_BRAWL_STORE:
        if (!((UnityEngine.Object) TavernBrawlStore.Get() != (UnityEngine.Object) null))
          break;
        TavernBrawlStore.Get().Hide();
        break;
      case ShownUIMgr.UI_WINDOW.QUEST_LOG:
        if (!((UnityEngine.Object) QuestLog.Get() != (UnityEngine.Object) null))
          break;
        QuestLog.Get().Hide();
        break;
    }
  }

  private void FireSpectatorModeChanged(OnlineEventType evt, BnetPlayer spectatee)
  {
    if (FriendChallengeMgr.Get() != null)
      FriendChallengeMgr.Get().UpdateMyAvailability();
    if (this.OnSpectatorModeChanged == null)
      return;
    this.OnSpectatorModeChanged(evt, spectatee);
  }

  private void SpectatePlayer_Internal(BnetGameAccountId gameAccountId, JoinInfo joinInfo)
  {
    if (!this.m_initialized)
      this.LogInfoParty("ERROR: SpectatePlayer_Internal called before initialized; spectatee={0}", (object) gameAccountId);
    this.m_pendingSpectatePlayerAfterLeave = (SpectatorManager.PendingSpectatePlayer) null;
    if ((UnityEngine.Object) WelcomeQuests.Get() != (UnityEngine.Object) null)
      WelcomeQuests.Hide();
    PartyInvite receivedInviteFrom = BnetParty.GetReceivedInviteFrom(gameAccountId, PartyType.SPECTATOR_PARTY);
    if ((BnetEntityId) this.m_spectateeFriendlySide == (BnetEntityId) null)
    {
      this.LogInfoPower("================== Begin Spectating 1st player ==================");
      this.m_spectateeFriendlySide = gameAccountId;
      if (receivedInviteFrom != null)
      {
        this.CloseWaitingForNextGameDialog();
        BnetParty.AcceptReceivedInvite(receivedInviteFrom.InviteId);
      }
      else if (joinInfo.HasPartyId)
      {
        BnetPartyId partyId = BnetUtils.CreatePartyId(joinInfo.PartyId);
        this.m_requestedInvite = new SpectatorManager.IntendedSpectateeParty(gameAccountId, partyId);
        BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
        BnetParty.RequestInvite(partyId, gameAccountId, myGameAccountId, PartyType.SPECTATOR_PARTY);
        Processor.ScheduleCallback(5f, true, new Processor.ScheduledCallback(this.SpectatePlayer_RequestInvite_FriendlySide_Timeout));
      }
      else
      {
        this.CloseWaitingForNextGameDialog();
        this.m_isExpectingArriveInGameplayAsSpectator = true;
        GameMgr.Get().SpectateGame(joinInfo);
      }
    }
    else if ((BnetEntityId) this.m_spectateeOpposingSide == (BnetEntityId) null)
    {
      if (!this.IsPlayerInGame(gameAccountId))
        Error.AddWarning(GameStrings.Get("GLOBAL_ERROR_GENERIC_HEADER"), GameStrings.Get("GLOBAL_SPECTATOR_ERROR_CANNOT_SPECTATE_2_GAMES"));
      else if ((BnetEntityId) this.m_spectateeFriendlySide == (BnetEntityId) gameAccountId)
      {
        this.LogInfoParty("SpectatePlayer: already spectating player {0}", (object) gameAccountId);
        if (receivedInviteFrom == null)
          return;
        BnetParty.AcceptReceivedInvite(receivedInviteFrom.InviteId);
      }
      else
      {
        this.LogInfoPower("================== Begin Spectating 2nd player ==================");
        this.m_spectateeOpposingSide = gameAccountId;
        if (receivedInviteFrom != null)
          BnetParty.AcceptReceivedInvite(receivedInviteFrom.InviteId);
        else if (joinInfo.HasPartyId)
        {
          BnetPartyId partyId = BnetUtils.CreatePartyId(joinInfo.PartyId);
          this.m_requestedInvite = new SpectatorManager.IntendedSpectateeParty(gameAccountId, partyId);
          BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
          BnetParty.RequestInvite(partyId, gameAccountId, myGameAccountId, PartyType.SPECTATOR_PARTY);
          Processor.ScheduleCallback(5f, true, new Processor.ScheduledCallback(this.SpectatePlayer_RequestInvite_OpposingSide_Timeout));
        }
        else
          this.SpectateSecondPlayer_Network(joinInfo);
      }
    }
    else if ((BnetEntityId) this.m_spectateeFriendlySide == (BnetEntityId) gameAccountId || (BnetEntityId) this.m_spectateeOpposingSide == (BnetEntityId) gameAccountId)
      this.LogInfoParty("SpectatePlayer: already spectating player {0}", (object) gameAccountId);
    else
      Error.AddDevFatal("Cannot spectate more than 2 players.");
  }

  private void SpectatePlayer_RequestInvite_FriendlySide_Timeout(object userData)
  {
    if (this.m_requestedInvite == null)
      return;
    this.m_spectateeFriendlySide = (BnetGameAccountId) null;
    this.EndSpectatorMode(true);
    SpectatorManager.DisplayErrorDialog(GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_HEADER"), GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_TEXT"));
    if (this.OnSpectateRejected == null)
      return;
    this.OnSpectateRejected();
  }

  private void SpectatePlayer_RequestInvite_OpposingSide_Timeout(object userData)
  {
    if (this.m_requestedInvite == null)
      return;
    this.m_requestedInvite = (SpectatorManager.IntendedSpectateeParty) null;
    this.m_spectateeOpposingSide = (BnetGameAccountId) null;
    SpectatorManager.DisplayErrorDialog(GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_HEADER"), GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_TEXT"));
    if (this.OnSpectateRejected == null)
      return;
    this.OnSpectateRejected();
  }

  private static JoinInfo CreateJoinInfo(PartyServerInfo serverInfo)
  {
    JoinInfo joinInfo = new JoinInfo();
    joinInfo.ServerIpAddress = serverInfo.ServerIpAddress;
    joinInfo.ServerPort = serverInfo.ServerPort;
    joinInfo.GameHandle = serverInfo.GameHandle;
    joinInfo.SecretKey = serverInfo.SecretKey;
    if (serverInfo.HasGameType)
      joinInfo.GameType = serverInfo.GameType;
    if (serverInfo.HasFormatType)
      joinInfo.FormatType = serverInfo.FormatType;
    if (serverInfo.HasMissionId)
      joinInfo.MissionId = serverInfo.MissionId;
    return joinInfo;
  }

  private static bool IsSameGameAndServer(PartyServerInfo a, GameServerInfo b)
  {
    if (a == null)
      return b == null;
    return b != null && a.ServerIpAddress == b.Address && (long) a.GameHandle == (long) b.GameHandle;
  }

  private void SpectateSecondPlayer_Network(JoinInfo joinInfo) => Network.Get().SpectateSecondPlayer(new GameServerInfo()
  {
    Address = joinInfo.ServerIpAddress,
    Port = joinInfo.ServerPort,
    GameHandle = (uint) joinInfo.GameHandle,
    SpectatorPassword = joinInfo.SecretKey,
    SpectatorMode = true
  });

  private void JoinPartyGame(BnetPartyId partyId)
  {
    if (partyId == (BnetPartyId) null)
      return;
    PartyInfo joinedParty = BnetParty.GetJoinedParty(partyId);
    if (joinedParty == null)
      return;
    Blizzard.GameService.Protocol.V2.Client.Attribute attribute;
    BattleNet.GetPartyAttribute(partyId, "WTCG.Party.ServerInfo", out attribute);
    this.BnetParty_OnPartyAttributeChanged_ServerInfo(joinedParty, attribute);
  }

  public void LeaveSpectatorMode()
  {
    GameMgr gameMgr = GameMgr.Get();
    int num = gameMgr.IsSpectator() ? 1 : 0;
    if (num != 0)
    {
      if (Network.Get().IsConnectedToGameServer())
        Network.Get().DisconnectFromGameServer();
      else
        this.LeaveGameScene();
    }
    if (num == 0 && !gameMgr.WasSpectator())
      return;
    if (this.m_spectatorPartyIdOpposingSide != (BnetPartyId) null)
      this.LeaveParty(this.m_spectatorPartyIdOpposingSide, false);
    if (!(this.m_spectatorPartyIdMain != (BnetPartyId) null))
      return;
    this.LeaveParty(this.m_spectatorPartyIdMain, false);
  }

  private void OnDisconnect(BattleNetErrors error)
  {
    this.m_leavePartyIdsRequested?.Clear();
    this.m_spectateeFriendlySide = (BnetGameAccountId) null;
    this.m_spectateeOpposingSide = (BnetGameAccountId) null;
    this.m_spectatorPartyIdMain = (BnetPartyId) null;
    this.m_spectatorPartyIdOpposingSide = (BnetPartyId) null;
    this.m_requestedInvite = (SpectatorManager.IntendedSpectateeParty) null;
    this.m_waitingForNextGameDialog = (AlertPopup) null;
    this.m_pendingSpectatePlayerAfterLeave = (SpectatorManager.PendingSpectatePlayer) null;
    this.m_isExpectingArriveInGameplayAsSpectator = false;
    this.CloseWaitingForNextGameDialog();
  }

  public void InviteToSpectateMe(BnetPlayer player)
  {
    if (player == null)
      return;
    BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
    if (this.m_kickedPlayers != null && this.m_kickedPlayers.Contains(hearthstoneGameAccountId))
      this.m_kickedPlayers.Remove(hearthstoneGameAccountId);
    if (!this.CanInviteToSpectateMyGame(hearthstoneGameAccountId))
      return;
    if (this.m_userInitiatedOutgoingInvites == null)
      this.m_userInitiatedOutgoingInvites = new HashSet<BnetGameAccountId>();
    this.m_userInitiatedOutgoingInvites.Add(hearthstoneGameAccountId);
    if (this.m_spectatorPartyIdMain == (BnetPartyId) null)
      BnetParty.CreateParty(PartyType.SPECTATOR_PARTY, ChannelApi.PartyPrivacyLevel.OpenInvitation, ProtobufUtil.ToByteArray((IProtoBuf) BnetUtils.CreatePegasusBnetId((BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId())), (BnetParty.CreateSuccessCallback) null);
    else
      BnetParty.SendInvite(this.m_spectatorPartyIdMain, hearthstoneGameAccountId, false);
  }

  public void KickSpectator(BnetPlayer player, bool regenerateSpectatorPassword) => this.KickSpectator_Internal(player, regenerateSpectatorPassword, true);

  private void KickSpectator_Internal(
    BnetPlayer player,
    bool regenerateSpectatorPassword,
    bool addToKickList)
  {
    if (player == null)
      return;
    BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
    if (!this.CanKickSpectator(hearthstoneGameAccountId))
      return;
    if (addToKickList)
    {
      if (this.m_kickedPlayers == null)
        this.m_kickedPlayers = new HashSet<BnetGameAccountId>();
      this.m_kickedPlayers.Add(hearthstoneGameAccountId);
    }
    if (Network.Get().IsConnectedToGameServer())
      Network.Get().SendRemoveSpectators((regenerateSpectatorPassword ? 1 : 0) != 0, hearthstoneGameAccountId);
    if (!(this.m_spectatorPartyIdMain != (BnetPartyId) null) || !this.ShouldBePartyLeader(this.m_spectatorPartyIdMain) || !BnetParty.IsMember(this.m_spectatorPartyIdMain, hearthstoneGameAccountId))
      return;
    BnetParty.KickMember(this.m_spectatorPartyIdMain, hearthstoneGameAccountId);
  }

  public void UpdateMySpectatorInfo()
  {
    this.UpdateSpectatorPresence();
    this.UpdateSpectatorPartyServerInfo();
  }

  private JoinInfo GetMyGameJoinInfo()
  {
    JoinInfo myGameJoinInfo = (JoinInfo) null;
    JoinInfo joinInfo = new JoinInfo();
    if (this.IsInSpectatorMode())
    {
      if ((BnetEntityId) this.m_spectateeFriendlySide != (BnetEntityId) null)
      {
        BnetId pegasusBnetId = BnetUtils.CreatePegasusBnetId((BnetEntityId) this.m_spectateeFriendlySide);
        joinInfo.SpectatedPlayers.Add(pegasusBnetId);
      }
      if ((BnetEntityId) this.m_spectateeOpposingSide != (BnetEntityId) null)
      {
        BnetId pegasusBnetId = BnetUtils.CreatePegasusBnetId((BnetEntityId) this.m_spectateeOpposingSide);
        joinInfo.SpectatedPlayers.Add(pegasusBnetId);
      }
      if (joinInfo.SpectatedPlayers.Count > 0)
        myGameJoinInfo = joinInfo;
    }
    else if (SceneMgr.Get().IsInGame())
    {
      int countSpectatingMe = this.GetCountSpectatingMe();
      if (this.CanAddSpectators())
      {
        GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
        if (this.m_spectatorPartyIdMain == (BnetPartyId) null && gameServerJoined != null && SceneMgr.Get().IsInGame() && !SpectatorManager.IsGameOver)
        {
          joinInfo.ServerIpAddress = gameServerJoined.Address;
          joinInfo.ServerPort = gameServerJoined.Port;
          joinInfo.GameHandle = (int) gameServerJoined.GameHandle;
          joinInfo.SecretKey = gameServerJoined.SpectatorPassword ?? "";
        }
        if (this.m_spectatorPartyIdMain != (BnetPartyId) null)
        {
          BnetId pegasusBnetId = BnetUtils.CreatePegasusBnetId(this.m_spectatorPartyIdMain);
          joinInfo.PartyId = pegasusBnetId;
          joinInfo.GameHandle = (int) gameServerJoined.GameHandle;
        }
      }
      joinInfo.CurrentNumSpectators = countSpectatingMe;
      joinInfo.MaxNumSpectators = 10;
      joinInfo.IsJoinable = joinInfo.CurrentNumSpectators < joinInfo.MaxNumSpectators;
      joinInfo.GameType = GameMgr.Get().GetGameType();
      joinInfo.FormatType = GameMgr.Get().GetFormatType();
      joinInfo.MissionId = GameMgr.Get().GetMissionId();
      myGameJoinInfo = joinInfo;
    }
    return myGameJoinInfo;
  }

  private static PartyServerInfo GetPartyServerInfo(BnetPartyId partyId)
  {
    byte[] bytes;
    return BattleNet.GetPartyAttribute<byte[]>(partyId, "WTCG.Party.ServerInfo", out bytes) ? ProtobufUtil.ParseFrom<PartyServerInfo>(bytes) : (PartyServerInfo) null;
  }

  public bool HandleDisconnectFromGameplay()
  {
    int num = this.m_expectedDisconnectReason.HasValue ? 1 : 0;
    this.EndCurrentSpectatedGame(false);
    if (num == 0)
      return num != 0;
    if (GameMgr.Get().IsTransitionPopupShown())
    {
      GameMgr.Get().GetTransitionPopup().Cancel();
      return num != 0;
    }
    this.LeaveGameScene();
    return num != 0;
  }

  public void OnRealTimeGameOver() => this.UpdateMySpectatorInfo();

  private void EndCurrentSpectatedGame(bool isLeavingGameplay)
  {
    if (isLeavingGameplay && this.IsInSpectatorMode())
      SoundManager.Get().LoadAndPlay((AssetReference) "SpectatorMode_Exit.prefab:f1d7dab96facdc64fb6648ff1dd22073");
    this.m_expectedDisconnectReason = new int?();
    this.m_isExpectingArriveInGameplayAsSpectator = false;
    this.ClearAllGameServerKnownSpectators();
    HearthstoneApplication hearthstoneApplication = HearthstoneApplication.Get();
    if ((UnityEngine.Object) hearthstoneApplication != (UnityEngine.Object) null && !hearthstoneApplication.IsResetting())
      this.UpdateSpectatorPresence();
    if (GameMgr.Get() == null || !((UnityEngine.Object) GameMgr.Get().GetTransitionPopup() != (UnityEngine.Object) null))
      return;
    GameMgr.Get().GetTransitionPopup().OnHidden -= new System.Action<TransitionPopup>(this.EnterSpectatorMode_OnTransitionPopupHide);
  }

  private void EndSpectatorMode(bool wasKnownSpectating = false)
  {
    bool gameplayAsSpectator = this.m_isExpectingArriveInGameplayAsSpectator;
    int num = wasKnownSpectating || (BnetEntityId) this.m_spectateeFriendlySide != (BnetEntityId) null ? 1 : ((BnetEntityId) this.m_spectateeOpposingSide != (BnetEntityId) null ? 1 : 0);
    this.LeaveSpectatorMode();
    this.EndCurrentSpectatedGame(false);
    this.m_spectateeFriendlySide = (BnetGameAccountId) null;
    this.m_spectateeOpposingSide = (BnetGameAccountId) null;
    this.m_requestedInvite = (SpectatorManager.IntendedSpectateeParty) null;
    this.CloseWaitingForNextGameDialog();
    this.m_pendingSpectatePlayerAfterLeave = (SpectatorManager.PendingSpectatePlayer) null;
    this.m_isExpectingArriveInGameplayAsSpectator = false;
    if (num != 0)
    {
      this.LogInfoPower("================== End Spectator Mode ==================");
      this.FireSpectatorModeChanged(OnlineEventType.REMOVED, BnetUtils.GetPlayer(this.m_spectateeFriendlySide));
    }
    switch (GameMgr.Get().GetPostGameSceneMode())
    {
      case SceneMgr.Mode.INVALID:
      case SceneMgr.Mode.HUB:
        if (PartyManager.Get().IsInParty() || HearthstoneApplication.Get().IsResetting())
          break;
        if (gameplayAsSpectator)
        {
          this.ReturnToHub(true);
          break;
        }
        this.ReturnToHub();
        break;
    }
  }

  public void ReturnToHub(bool allowReloadHub = false)
  {
    SceneMgr.Mode mode = SceneMgr.Mode.HUB;
    bool flag = SceneMgr.Get().GetMode() == mode;
    if (!GameUtils.IsAnyTutorialComplete() && Network.ShouldBeConnectedToAurora())
      Network.Get().ShowBreakingNewsOrError("GLOBAL_ERROR_NETWORK_LOST_GAME_CONNECTION");
    else if (!SceneMgr.Get().IsModeRequested(mode))
      SceneMgr.Get().SetNextMode(mode);
    else if (flag & allowReloadHub)
      SceneMgr.Get().ReloadMode();
    if (!flag || allowReloadHub)
      return;
    this.CheckShowWaitingForNextGameDialog();
  }

  private void ClearAllCacheForReset()
  {
    this.EndSpectatorMode();
    this.m_initialized = false;
    this.m_spectatorPartyIdMain = (BnetPartyId) null;
    this.m_spectatorPartyIdOpposingSide = (BnetPartyId) null;
    this.m_requestedInvite = (SpectatorManager.IntendedSpectateeParty) null;
    this.m_waitingForNextGameDialog = (AlertPopup) null;
    this.m_pendingSpectatePlayerAfterLeave = (SpectatorManager.PendingSpectatePlayer) null;
    this.m_userInitiatedOutgoingInvites = (HashSet<BnetGameAccountId>) null;
    this.m_kickedPlayers = (HashSet<BnetGameAccountId>) null;
    this.m_kickedFromSpectatingList = (Map<BnetGameAccountId, uint>) null;
    this.m_expectedDisconnectReason = new int?();
    this.m_isExpectingArriveInGameplayAsSpectator = false;
    this.m_isShowingRemovedAsSpectatorPopup = false;
    this.m_gameServerKnownSpectators.Clear();
  }

  private void WillReset()
  {
    this.ClearAllCacheForReset();
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.SpectatorManager_UpdatePresenceNextFrame));
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    switch (eventData.m_state)
    {
      case FindGameState.CLIENT_CANCELED:
      case FindGameState.CLIENT_ERROR:
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
        if (this.IsInSpectatorMode())
        {
          this.EndSpectatorMode(true);
          break;
        }
        break;
      case FindGameState.SERVER_GAME_CANCELED:
        if (this.IsInSpectatorMode())
        {
          SpectatorManager.DisplayErrorDialog(GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_HEADER"), GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_TEXT"));
          this.EndSpectatorMode(true);
          if (this.OnSpectateRejected != null)
          {
            this.OnSpectateRejected();
            break;
          }
          break;
        }
        break;
    }
    return false;
  }

  private void GameState_InitializedEvent(GameState instance, object userData)
  {
    if (!(this.m_spectatorPartyIdOpposingSide != (BnetPartyId) null))
      return;
    GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.GameState_CreateGameEvent), (object) null);
  }

  private void GameState_CreateGameEvent(GameState.CreateGamePhase createGamePhase, object userData)
  {
    if (createGamePhase < GameState.CreateGamePhase.CREATED)
      return;
    GameState.Get().UnregisterCreateGameListener(new GameState.CreateGameCallback(this.GameState_CreateGameEvent));
    if (!(this.m_spectatorPartyIdOpposingSide != (BnetPartyId) null))
      return;
    this.AutoSpectateOpposingSide();
  }

  private void AutoSpectateOpposingSide()
  {
    if (GameState.Get() == null)
      return;
    if (GameState.Get().GetCreateGamePhase() < GameState.CreateGamePhase.CREATED)
    {
      GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.GameState_CreateGameEvent), (object) null);
    }
    else
    {
      if (SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
        return;
      if ((UnityEngine.Object) GameMgr.Get().GetTransitionPopup() != (UnityEngine.Object) null && GameMgr.Get().GetTransitionPopup().IsShown())
      {
        GameMgr.Get().GetTransitionPopup().OnHidden += new System.Action<TransitionPopup>(this.EnterSpectatorMode_OnTransitionPopupHide);
      }
      else
      {
        if (!(this.m_spectatorPartyIdOpposingSide != (BnetPartyId) null) || !((BnetEntityId) this.m_spectateeOpposingSide != (BnetEntityId) null) || !this.IsStillInParty(this.m_spectatorPartyIdOpposingSide))
          return;
        if (this.IsPlayerInGame(this.m_spectateeOpposingSide))
        {
          PartyServerInfo partyServerInfo = SpectatorManager.GetPartyServerInfo(this.m_spectatorPartyIdOpposingSide);
          JoinInfo joinInfo = partyServerInfo == null ? (JoinInfo) null : SpectatorManager.CreateJoinInfo(partyServerInfo);
          if (joinInfo == null)
            return;
          this.SpectateSecondPlayer_Network(joinInfo);
        }
        else
        {
          this.LogInfoPower("================== End Spectating 2nd player ==================");
          this.LeaveParty(this.m_spectatorPartyIdOpposingSide, false);
        }
      }
    }
  }

  private void OnSceneUnloaded(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    SceneMgr.Mode mode = SceneMgr.Get().GetMode();
    if (mode == SceneMgr.Mode.GAMEPLAY)
      this.m_gameServerKnownSpectators.Clear();
    if (mode == SceneMgr.Mode.GAMEPLAY && prevMode != SceneMgr.Mode.GAMEPLAY)
    {
      if ((BnetEntityId) this.m_spectateeFriendlySide != (BnetEntityId) null)
        BnetBar.Get().HideFriendList();
      if (GameMgr.Get().IsSpectator())
      {
        if ((UnityEngine.Object) GameMgr.Get().GetTransitionPopup() != (UnityEngine.Object) null)
          GameMgr.Get().GetTransitionPopup().OnHidden += new System.Action<TransitionPopup>(this.EnterSpectatorMode_OnTransitionPopupHide);
        this.FireSpectatorModeChanged(OnlineEventType.ADDED, BnetUtils.GetPlayer(this.m_spectateeOpposingSide ?? this.m_spectateeFriendlySide));
      }
      else
        this.m_kickedPlayers = (HashSet<BnetGameAccountId>) null;
      this.CloseWaitingForNextGameDialog();
      this.DeclineAllReceivedInvitations();
      this.UpdateMySpectatorInfo();
    }
    else
    {
      if (prevMode != SceneMgr.Mode.GAMEPLAY || mode == SceneMgr.Mode.GAMEPLAY)
        return;
      if (this.IsInSpectatorMode())
      {
        this.LogInfoPower("================== End Spectator Game ==================");
        TimeScaleMgr.Get().SetGameTimeScale(1f);
      }
      this.EndCurrentSpectatedGame(true);
      this.UpdateMySpectatorInfo();
      if (!this.IsInSpectatorMode())
        return;
      PartyServerInfo partyServerInfo = SpectatorManager.GetPartyServerInfo(this.m_spectatorPartyIdMain);
      if (partyServerInfo == null)
      {
        this.ShowWaitingForNextGameDialog();
      }
      else
      {
        GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
        if (!SpectatorManager.IsSameGameAndServer(partyServerInfo, gameServerJoined))
        {
          this.LogInfoPower("================== OnSceneUnloaded: auto-spectating game after leaving game ==================");
          Blizzard.GameService.Protocol.V2.Client.Attribute attribute;
          BattleNet.GetPartyAttribute(this.m_spectatorPartyIdMain, "WTCG.Party.ServerInfo", out attribute);
          this.BnetParty_OnPartyAttributeChanged_ServerInfo(new PartyInfo(this.m_spectatorPartyIdMain, PartyType.SPECTATOR_PARTY), attribute);
        }
        else
          this.ShowWaitingForNextGameDialog();
      }
    }
  }

  public void CheckShowWaitingForNextGameDialog()
  {
    bool flag = true;
    if (!this.IsInSpectatorMode())
      flag = false;
    else if (SceneMgr.Get().GetNextMode() != SceneMgr.Mode.INVALID)
      flag = false;
    else if (this.IsInSpectatableScene(true))
      flag = false;
    if (flag)
      this.ShowWaitingForNextGameDialog();
    else
      this.CloseWaitingForNextGameDialog();
  }

  public void ShowWaitingForNextGameDialog()
  {
    if (!Network.IsLoggedIn())
      return;
    DialogManager.Get().ShowUniquePopup(new AlertPopup.PopupInfo()
    {
      m_id = "SPECTATOR_WAITING_FOR_NEXT_GAME",
      m_layerToUse = new GameLayer?(GameLayer.UI),
      m_headerText = GameStrings.Get("GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_HEADER"),
      m_text = this.GetWaitingForNextGameDialogText(),
      m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL,
      m_cancelText = GameStrings.Get("GLOBAL_LEAVE_SPECTATOR_MODE"),
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnSceneUnloaded_AwaitingNextGame_LeaveSpectatorMode),
      m_keyboardEscIsCancel = false
    }, new DialogManager.DialogProcessCallback(this.OnSceneUnloaded_AwaitingNextGame_DialogProcessCallback));
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(SpectatorManager.WaitingForNextGame_AutoLeaveSpectatorMode));
    if ((double) (float) SpectatorManager.WAITING_FOR_NEXT_GAME_AUTO_LEAVE_SECONDS < 0.0)
      return;
    Processor.ScheduleCallback((float) SpectatorManager.WAITING_FOR_NEXT_GAME_AUTO_LEAVE_SECONDS, true, new Processor.ScheduledCallback(SpectatorManager.WaitingForNextGame_AutoLeaveSpectatorMode));
  }

  private void CloseWaitingForNextGameDialog()
  {
    if ((bool) SpectatorManager.DISABLE_MENU_BUTTON_WHILE_WAITING)
      BnetBar.Get().m_menuButton.SetEnabled(true, false);
    if ((UnityEngine.Object) DialogManager.Get() != (UnityEngine.Object) null)
      DialogManager.Get().RemoveUniquePopupRequestFromQueue("SPECTATOR_WAITING_FOR_NEXT_GAME");
    if ((UnityEngine.Object) this.m_waitingForNextGameDialog != (UnityEngine.Object) null)
    {
      this.m_waitingForNextGameDialog.Hide();
      this.m_waitingForNextGameDialog = (AlertPopup) null;
    }
    Processor.CancelScheduledCallback(new Processor.ScheduledCallback(SpectatorManager.WaitingForNextGame_AutoLeaveSpectatorMode));
  }

  private void UpdateWaitingForNextGameDialog()
  {
    if ((UnityEngine.Object) this.m_waitingForNextGameDialog == (UnityEngine.Object) null)
      return;
    this.m_waitingForNextGameDialog.BodyText = this.GetWaitingForNextGameDialogText();
  }

  private string GetWaitingForNextGameDialogText()
  {
    BnetPlayer player = BnetUtils.GetPlayer(this.m_spectateeFriendlySide);
    string playerBestName = BnetUtils.GetPlayerBestName(this.m_spectateeFriendlySide);
    string str;
    string key;
    if (player != null && player.IsOnline())
    {
      str = PresenceMgr.Get().GetStatusText(player) ?? "";
      if (!string.IsNullOrEmpty(str))
      {
        str = str.Trim();
        key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT";
      }
      else
        key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT_ONLINE";
      Enum[] statusEnums = PresenceMgr.Get().GetStatusEnums(player);
      if (statusEnums.Length != 0 && (Global.PresenceStatus) statusEnums[0] == Global.PresenceStatus.ADVENTURE_SCENARIO_SELECT)
      {
        if (statusEnums.Length > 1 && (PresenceAdventureMode) statusEnums[1] < PresenceAdventureMode.RETURNING_PLAYER_CHALLENGE)
          key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT_ENTERING";
      }
      else if (statusEnums.Length != 0 && (Global.PresenceStatus) statusEnums[0] == Global.PresenceStatus.ADVENTURE_SCENARIO_PLAYING_GAME)
      {
        if (statusEnums.Length > 1 && GameUtils.IsHeroicAdventureMission((int) statusEnums[1]))
          key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT_BATTLING";
        else if (statusEnums.Length > 1 && GameUtils.IsClassChallengeMission((int) statusEnums[1]))
          key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT_PLAYING";
      }
    }
    else
    {
      key = "GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TEXT_OFFLINE";
      str = GameStrings.Get("GLOBAL_OFFLINE");
    }
    return GameStrings.Format(key, (object) playerBestName, (object) str);
  }

  private bool OnSceneUnloaded_AwaitingNextGame_DialogProcessCallback(
    DialogBase dialog,
    object userData)
  {
    if (SceneMgr.Get().IsInGame() || GameMgr.Get() != null && GameMgr.Get().IsFindingGame())
      return false;
    this.m_waitingForNextGameDialog = (AlertPopup) dialog;
    this.UpdateWaitingForNextGameDialog();
    if ((bool) SpectatorManager.DISABLE_MENU_BUTTON_WHILE_WAITING)
      BnetBar.Get().m_menuButton.SetEnabled(false, false);
    return true;
  }

  private static void WaitingForNextGame_AutoLeaveSpectatorMode(object userData)
  {
    if (!SpectatorManager.Get().IsInSpectatorMode() || SceneMgr.Get().IsInGame())
      return;
    SpectatorManager.Get().LeaveSpectatorMode();
    SpectatorManager.DisplayErrorDialog(GameStrings.Get("GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_HEADER"), GameStrings.Format("GLOBAL_SPECTATOR_WAITING_FOR_NEXT_GAME_TIMEOUT"));
  }

  private void OnSceneUnloaded_AwaitingNextGame_LeaveSpectatorMode(
    AlertPopup.Response response,
    object userData)
  {
    this.LeaveSpectatorMode();
  }

  private void EnterSpectatorMode_OnTransitionPopupHide(TransitionPopup popup)
  {
    popup.OnHidden -= new System.Action<TransitionPopup>(this.EnterSpectatorMode_OnTransitionPopupHide);
    if (SoundManager.Get() != null)
      SoundManager.Get().LoadAndPlay((AssetReference) "SpectatorMode_Enter.prefab:e0c11cb0f554e6c4cb9f24994bf13e1c");
    if (!((BnetEntityId) this.m_spectateeOpposingSide != (BnetEntityId) null))
      return;
    this.AutoSpectateOpposingSide();
  }

  private void OnSpectatorOpenJoinOptionChanged(
    Option option,
    object prevValue,
    bool existed,
    object userData)
  {
    bool flag = Options.Get().GetBool(Option.SPECTATOR_OPEN_JOIN);
    if ((!existed ? 1 : ((bool) prevValue != flag ? 1 : 0)) == 0 || !ServiceManager.IsAvailable<SceneMgr>() || !SceneMgr.Get().IsInGame() || GameMgr.Get() != null && GameMgr.Get().IsSpectator())
      return;
    JoinInfo joinInfo = !flag ? (JoinInfo) null : this.GetMyGameJoinInfo();
    if (!Network.ShouldBeConnectedToAurora() || !Network.IsLoggedIn())
      return;
    BnetPresenceMgr.Get().SetPresenceSpectatorJoinInfo(joinInfo);
  }

  private void Network_OnSpectatorNotifyEvent()
  {
    SpectatorNotify spectatorNotify = Network.Get().GetSpectatorNotify();
    if (spectatorNotify == null)
    {
      TelemetryManager.Client().SendLiveIssue("Network_OnSpectatorNotifyEvent Exception", "'notify' is null.");
    }
    else
    {
      if (spectatorNotify.HasSpectatorPasswordUpdate && !string.IsNullOrEmpty(spectatorNotify.SpectatorPasswordUpdate))
      {
        GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
        if (gameServerJoined == null)
          TelemetryManager.Client().SendLiveIssue("Network_OnSpectatorNotifyEvent Exception", "'serverInfo' is null.");
        else if (!spectatorNotify.SpectatorPasswordUpdate.Equals(gameServerJoined.SpectatorPassword))
        {
          gameServerJoined.SpectatorPassword = spectatorNotify.SpectatorPasswordUpdate;
          this.UpdateMySpectatorInfo();
          this.RevokeAllSentInvitations();
        }
      }
      if (spectatorNotify.HasSpectatorRemoved)
      {
        this.m_expectedDisconnectReason = new int?(spectatorNotify.SpectatorRemoved.ReasonCode);
        GameMgr gameMgr = GameMgr.Get();
        if (gameMgr == null)
          TelemetryManager.Client().SendLiveIssue("Network_OnSpectatorNotifyEvent Exception", "GameMgr is null.");
        bool flag = gameMgr != null && gameMgr.IsTransitionPopupShown();
        if (spectatorNotify.SpectatorRemoved.ReasonCode == 0)
        {
          if (spectatorNotify.SpectatorRemoved.HasRemovedBy)
          {
            GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
            if (gameServerJoined != null)
            {
              if (this.m_kickedFromSpectatingList == null)
                this.m_kickedFromSpectatingList = new Map<BnetGameAccountId, uint>();
              this.m_kickedFromSpectatingList[BnetGameAccountId.CreateFromNet(spectatorNotify.SpectatorRemoved.RemovedBy)] = gameServerJoined.GameHandle;
            }
          }
          if (!this.m_isShowingRemovedAsSpectatorPopup)
          {
            AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
            info.m_headerText = GameStrings.Get("GLOBAL_SPECTATOR_REMOVED_PROMPT_HEADER");
            info.m_text = GameStrings.Get("GLOBAL_SPECTATOR_REMOVED_PROMPT_TEXT");
            info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
            info.m_responseCallback = !flag ? (AlertPopup.ResponseCallback) ((r, data) =>
            {
              SpectatorManager spectatorManager = SpectatorManager.Get();
              if (spectatorManager != null)
                spectatorManager.m_isShowingRemovedAsSpectatorPopup = false;
              else
                TelemetryManager.Client().SendLiveIssue("Network_OnSpectatorNotifyEvent Exception", "SpectatorManager is null in response callback.");
            }) : new AlertPopup.ResponseCallback(this.Network_OnSpectatorNotifyEvent_Removed_GoToNextMode);
            this.m_isShowingRemovedAsSpectatorPopup = true;
            DialogManager dialogManager = DialogManager.Get();
            if ((UnityEngine.Object) dialogManager != (UnityEngine.Object) null)
              dialogManager.ShowPopup(info);
            else
              TelemetryManager.Client().SendLiveIssue("Network_OnSpectatorNotifyEvent Exception", "DialogManager is null.");
          }
        }
        else if (flag)
          this.Network_OnSpectatorNotifyEvent_Removed_GoToNextMode(AlertPopup.Response.OK, (object) null);
        SoundManager soundManager = SoundManager.Get();
        if (soundManager != null)
          soundManager.LoadAndPlay((AssetReference) "SpectatorMode_Exit.prefab:f1d7dab96facdc64fb6648ff1dd22073");
        else
          TelemetryManager.Client().SendLiveIssue("Network_OnSpectatorNotifyEvent Exception", "SoundManager is null.");
        this.EndSpectatorMode(true);
        this.m_expectedDisconnectReason = new int?(spectatorNotify.SpectatorRemoved.ReasonCode);
      }
      if (spectatorNotify == null || spectatorNotify.SpectatorChange.Count == 0 || GameMgr.Get() != null && GameMgr.Get().IsSpectator())
        return;
      foreach (SpectatorChange spectatorChange in spectatorNotify.SpectatorChange)
      {
        BnetGameAccountId fromNet = BnetGameAccountId.CreateFromNet(spectatorChange.GameAccountId);
        if (spectatorChange.IsRemoved)
        {
          this.RemoveKnownSpectator(fromNet);
        }
        else
        {
          this.AddKnownSpectator(fromNet);
          this.ReinviteKnownSpectatorsNotInParty();
        }
      }
    }
  }

  private void Network_OnSpectatorNotifyEvent_Removed_GoToNextMode(
    AlertPopup.Response response,
    object userData)
  {
    this.m_isShowingRemovedAsSpectatorPopup = false;
  }

  private void Presence_OnGameAccountPresenceChange(PresenceUpdate[] updates)
  {
    foreach (PresenceUpdate update in updates)
    {
      BnetGameAccountId entityId = BnetGameAccountId.CreateFromBnetEntityId(update.entityId);
      bool flag1 = update.fieldId == 1U && update.programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.BNET;
      bool flag2 = update.programId == (Blizzard.GameService.SDK.Client.Integration.FourCC) BnetProgramId.HEARTHSTONE && update.fieldId == 17U;
      if ((UnityEngine.Object) this.m_waitingForNextGameDialog != (UnityEngine.Object) null && (BnetEntityId) this.m_spectateeFriendlySide != (BnetEntityId) null && flag1 | flag2 && (BnetEntityId) entityId == (BnetEntityId) this.m_spectateeFriendlySide)
        this.UpdateWaitingForNextGameDialog();
      if (flag1 && update.boolVal)
      {
        foreach (BnetPartyId joinedPartyId in BnetParty.GetJoinedPartyIds())
        {
          if (BnetParty.IsLeader(joinedPartyId) && !BnetParty.IsMember(joinedPartyId, entityId))
          {
            BnetGameAccountId partyCreator = this.GetPartyCreator(joinedPartyId);
            if ((BnetEntityId) partyCreator != (BnetEntityId) null && (BnetEntityId) partyCreator == (BnetEntityId) entityId && !((IEnumerable<PartyInvite>) BnetParty.GetSentInvites(joinedPartyId)).Any<PartyInvite>((Func<PartyInvite, bool>) (i => (BnetEntityId) i.InviteeId == (BnetEntityId) entityId)))
              BnetParty.SendInvite(joinedPartyId, entityId, false);
          }
        }
      }
    }
  }

  private void BnetFriendMgr_OnFriendsChanged(BnetFriendChangelist changelist, object userData)
  {
    if (changelist == null)
      return;
    this.CheckSpectatorsOnChangedContext(changelist.GetRemovedFriends());
  }

  private void FiresideGatheringManager_OnPatronListUpdated(
    List<BnetPlayer> addedList,
    List<BnetPlayer> removedList)
  {
    this.CheckSpectatorsOnChangedContext(removedList);
  }

  private void CheckSpectatorsOnChangedContext(List<BnetPlayer> players)
  {
    if (!this.IsBeingSpectated() || players == null)
      return;
    foreach (BnetPlayer player in players)
    {
      BnetGameAccountId hearthstoneGameAccountId = player.GetHearthstoneGameAccountId();
      if (this.IsSpectatingMe(hearthstoneGameAccountId) && !this.IsInSpectableContextWithPlayer(hearthstoneGameAccountId))
        this.KickSpectator_Internal(player, true, false);
    }
  }

  private void EndGameScreen_OnTwoScoopsShown(bool shown, EndGameTwoScoop twoScoops)
  {
    if (!this.IsSpectatingOrWatching)
      return;
    if (shown)
      Processor.ScheduleCallback(5f, false, new Processor.ScheduledCallback(this.EndGameScreen_OnTwoScoopsShown_AutoClose));
    else
      Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.EndGameScreen_OnTwoScoopsShown_AutoClose));
  }

  private void EndGameScreen_OnTwoScoopsShown_AutoClose(object userData)
  {
    if ((UnityEngine.Object) EndGameScreen.Get() == (UnityEngine.Object) null)
      return;
    if ((double) (float) SpectatorManager.WAITING_FOR_NEXT_GAME_AUTO_LEAVE_SECONDS >= 0.0)
    {
      int num = 0;
      while (EndGameScreen.Get().ContinueEvents())
      {
        ++num;
        if (num > 100)
          break;
      }
    }
    else
      EndGameScreen.Get().ContinueEvents();
  }

  private void EndGameScreen_OnBackOutOfGameplay()
  {
    if (!PartyManager.Get().IsInParty())
      return;
    this.LeaveSpectatorMode();
  }

  private void BnetParty_OnError(PartyError error)
  {
    if (!error.IsOperationCallback)
      return;
    switch (error.FeatureEvent)
    {
      case BnetFeatureEvent.Party_Create_Callback:
        if (error.ErrorCode == BattleNetErrors.ERROR_OK)
          break;
        this.m_userInitiatedOutgoingInvites = (HashSet<BnetGameAccountId>) null;
        SpectatorManager.DisplayErrorDialog(GameStrings.Get("GLOBAL_ERROR_GENERIC_HEADER"), GameStrings.Format("GLOBAL_SPECTATOR_ERROR_CREATE_PARTY_TEXT"));
        break;
      case BnetFeatureEvent.Party_Leave_Callback:
      case BnetFeatureEvent.Party_Dissolve_Callback:
        if (this.m_leavePartyIdsRequested != null)
          this.m_leavePartyIdsRequested.Remove(error.PartyId);
        if (this.m_pendingSpectatePlayerAfterLeave == null || error.ErrorCode == BattleNetErrors.ERROR_OK)
          break;
        string playerBestName = BnetUtils.GetPlayerBestName(this.m_pendingSpectatePlayerAfterLeave.SpectateeId);
        SpectatorManager.DisplayErrorDialog(GameStrings.Get("GLOBAL_ERROR_GENERIC_HEADER"), GameStrings.Format("GLOBAL_SPECTATOR_ERROR_LEAVE_FOR_SPECTATE_PLAYER_TEXT", (object) playerBestName));
        this.m_pendingSpectatePlayerAfterLeave = (SpectatorManager.PendingSpectatePlayer) null;
        break;
    }
  }

  private static void DisplayErrorDialog(string header, string body) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = header,
    m_text = body,
    m_responseDisplay = AlertPopup.ResponseDisplay.OK
  });

  private void BnetParty_OnJoined(OnlineEventType evt, PartyInfo party, LeaveReason? reason)
  {
    if (!this.m_initialized || party.Type != PartyType.SPECTATOR_PARTY)
      return;
    if (evt == OnlineEventType.REMOVED)
    {
      bool flag1 = false;
      if (this.m_leavePartyIdsRequested != null)
        flag1 = this.m_leavePartyIdsRequested.Remove(party.Id);
      this.LogInfoParty("SpectatorParty_OnLeft: left party={0} current={1} reason={2} wasRequested={3}", (object) party, (object) this.m_spectatorPartyIdMain, reason.HasValue ? (object) reason.Value.ToString() : (object) "null", (object) flag1);
      bool flag2 = false;
      if (party.Id == this.m_spectatorPartyIdOpposingSide)
      {
        this.m_spectatorPartyIdOpposingSide = (BnetPartyId) null;
        flag2 = true;
      }
      else if ((BnetEntityId) this.m_spectateeFriendlySide != (BnetEntityId) null)
      {
        if (party.Id == this.m_spectatorPartyIdMain)
        {
          this.m_spectatorPartyIdMain = (BnetPartyId) null;
          flag2 = true;
        }
      }
      else if ((BnetEntityId) this.m_spectateeFriendlySide == (BnetEntityId) null && (BnetEntityId) this.m_spectateeOpposingSide == (BnetEntityId) null)
      {
        if (party.Id != this.m_spectatorPartyIdMain)
        {
          this.CreatePartyIfNecessary();
          return;
        }
        this.m_userInitiatedOutgoingInvites = (HashSet<BnetGameAccountId>) null;
        this.m_spectatorPartyIdMain = (BnetPartyId) null;
        this.UpdateSpectatorPresence();
        if (reason.HasValue && reason.Value != LeaveReason.MEMBER_LEFT && reason.Value != LeaveReason.DISSOLVED_BY_MEMBER)
          Processor.ScheduleCallback(1f, true, (Processor.ScheduledCallback) (userData => this.CreatePartyIfNecessary()));
      }
      if (this.m_pendingSpectatePlayerAfterLeave != null && this.m_spectatorPartyIdMain == (BnetPartyId) null && this.m_spectatorPartyIdOpposingSide == (BnetPartyId) null)
        this.SpectatePlayer_Internal(this.m_pendingSpectatePlayerAfterLeave.SpectateeId, this.m_pendingSpectatePlayerAfterLeave.JoinInfo);
      else if (flag2 && this.m_spectatorPartyIdMain == (BnetPartyId) null)
      {
        if (flag1)
        {
          this.EndSpectatorMode(true);
        }
        else
        {
          bool flag3 = reason.HasValue && reason.Value == LeaveReason.MEMBER_KICKED;
          bool flag4 = this.m_expectedDisconnectReason.HasValue && this.m_expectedDisconnectReason.Value == 0;
          this.EndSpectatorMode(true);
          if (flag3 && !flag4)
          {
            if (flag3)
            {
              BnetGameAccountId key = this.GetPartyCreator(party.Id);
              if ((BnetEntityId) key == (BnetEntityId) null)
              {
                BnetParty.PartyMember leader = BnetParty.GetLeader(party.Id);
                key = leader == null ? (BnetGameAccountId) null : leader.GameAccountId;
              }
              if ((BnetEntityId) key != (BnetEntityId) null)
              {
                GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
                if (gameServerJoined != null)
                {
                  if (this.m_kickedFromSpectatingList == null)
                    this.m_kickedFromSpectatingList = new Map<BnetGameAccountId, uint>();
                  this.m_kickedFromSpectatingList[key] = gameServerJoined.GameHandle;
                }
              }
            }
            if (!this.m_isShowingRemovedAsSpectatorPopup)
            {
              int num = GameMgr.Get().IsTransitionPopupShown() ? 1 : 0;
              AlertPopup.PopupInfo info = new AlertPopup.PopupInfo();
              info.m_headerText = GameStrings.Get("GLOBAL_SPECTATOR_REMOVED_PROMPT_HEADER");
              info.m_text = BnetPresenceMgr.Get().GetMyPlayer().IsAppearingOffline() ? GameStrings.Get("GLOBAL_SPECTATOR_REMOVED_PROMPT_APPEAR_OFFLINE_TEXT") : GameStrings.Get("GLOBAL_SPECTATOR_REMOVED_PROMPT_TEXT");
              info.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
              info.m_responseCallback = num == 0 ? (AlertPopup.ResponseCallback) ((r, data) => SpectatorManager.Get().m_isShowingRemovedAsSpectatorPopup = false) : new AlertPopup.ResponseCallback(this.Network_OnSpectatorNotifyEvent_Removed_GoToNextMode);
              this.m_isShowingRemovedAsSpectatorPopup = true;
              DialogManager.Get().ShowPopup(info);
            }
          }
        }
      }
      Processor.ScheduleCallback(0.5f, false, new Processor.ScheduledCallback(this.BnetParty_OnLostPartyReference_RemoveKnownCreator), (object) party.Id);
    }
    if (evt != OnlineEventType.ADDED)
      return;
    BnetGameAccountId partyCreator = this.GetPartyCreator(party.Id);
    if ((BnetEntityId) partyCreator == (BnetEntityId) null)
    {
      this.LogInfoParty("SpectatorParty_OnJoined: joined party={0} without creator.", (object) party.Id);
      this.LeaveParty(party.Id, BnetParty.IsLeader(party.Id));
    }
    else
    {
      if (this.m_requestedInvite != null && this.m_requestedInvite.PartyId == party.Id)
      {
        this.m_requestedInvite = (SpectatorManager.IntendedSpectateeParty) null;
        Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.SpectatePlayer_RequestInvite_FriendlySide_Timeout));
        Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.SpectatePlayer_RequestInvite_OpposingSide_Timeout));
      }
      bool flag5 = this.ShouldBePartyLeader(party.Id);
      bool flag6 = this.m_spectatorPartyIdMain == (BnetPartyId) null;
      bool flag7 = flag6;
      if (this.m_spectatorPartyIdMain != (BnetPartyId) null && this.m_spectatorPartyIdMain != party.Id && (flag5 || (BnetEntityId) partyCreator != (BnetEntityId) this.m_spectateeOpposingSide))
      {
        flag7 = true;
        this.LogInfoParty("SpectatorParty_OnJoined: joined party={0} when different current={1} (will be clobbered) joinedParties={2}", (object) party.Id, (object) this.m_spectatorPartyIdMain, (object) string.Join(", ", ((IEnumerable<PartyInfo>) BnetParty.GetJoinedParties()).Select<PartyInfo, string>((Func<PartyInfo, string>) (i => i.ToString())).ToArray<string>()));
      }
      if (flag5)
      {
        this.m_spectatorPartyIdMain = party.Id;
        if (flag7)
          this.UpdateSpectatorPresence();
        this.UpdateSpectatorPartyServerInfo();
        this.ReinviteKnownSpectatorsNotInParty();
        if (this.m_userInitiatedOutgoingInvites != null)
        {
          foreach (BnetGameAccountId initiatedOutgoingInvite in this.m_userInitiatedOutgoingInvites)
            BnetParty.SendInvite(this.m_spectatorPartyIdMain, initiatedOutgoingInvite, false);
        }
        if (!flag6 || this.OnSpectatorToMyGame == null)
          return;
        foreach (BnetParty.PartyMember member in BnetParty.GetMembers(this.m_spectatorPartyIdMain))
        {
          if (!((BnetEntityId) member.GameAccountId == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId()))
          {
            Processor.RunCoroutine(this.WaitForPresenceThenToast(member.GameAccountId, SocialToastMgr.TOAST_TYPE.SPECTATOR_ADDED));
            this.OnSpectatorToMyGame(OnlineEventType.ADDED, BnetUtils.GetPlayer(member.GameAccountId));
          }
        }
      }
      else
      {
        bool flag8 = true;
        if ((BnetEntityId) this.m_spectateeFriendlySide == (BnetEntityId) null)
        {
          this.m_spectateeFriendlySide = partyCreator;
          this.m_spectatorPartyIdMain = party.Id;
          flag8 = false;
        }
        else if ((BnetEntityId) partyCreator == (BnetEntityId) this.m_spectateeFriendlySide)
          this.m_spectatorPartyIdMain = party.Id;
        else if ((BnetEntityId) partyCreator == (BnetEntityId) this.m_spectateeOpposingSide)
          this.m_spectatorPartyIdOpposingSide = party.Id;
        if (BattleNet.GetPartyAttribute<byte[]>(party.Id, "WTCG.Party.ServerInfo", out byte[] _))
        {
          this.LogInfoParty("SpectatorParty_OnJoined: joined party={0} as spectator, begin spectating game.", (object) party.Id);
          if (!flag8)
          {
            if ((BnetEntityId) partyCreator == (BnetEntityId) this.m_spectateeOpposingSide)
              this.LogInfoPower("================== Begin Spectating 2nd player ==================");
            else
              this.LogInfoPower("================== Begin Spectating 1st player ==================");
          }
          this.JoinPartyGame(party.Id);
        }
        else
        {
          if (PartyManager.Get().IsInParty())
          {
            SpectatorManager.DisplayErrorDialog(GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_HEADER"), GameStrings.Get("GLOBAL_SPECTATOR_SERVER_REJECTED_TEXT"));
            this.EndSpectatorMode(true);
            if (this.OnSpectateRejected != null)
              this.OnSpectateRejected();
          }
          else if (!SceneMgr.Get().IsInGame())
            this.ShowWaitingForNextGameDialog();
          this.FireSpectatorModeChanged(OnlineEventType.ADDED, BnetUtils.GetPlayer(partyCreator));
        }
      }
    }
  }

  private void BnetParty_OnLostPartyReference_RemoveKnownCreator(object userData)
  {
    BnetPartyId partyId = userData as BnetPartyId;
    if (!(partyId != (BnetPartyId) null) || BnetParty.IsInParty(partyId) || ((IEnumerable<PartyInvite>) BnetParty.GetReceivedInvites()).Any<PartyInvite>((Func<PartyInvite, bool>) (i => i.PartyId == partyId)))
      return;
    SpectatorManager.Get().m_knownPartyCreatorIds.Remove(partyId);
  }

  private void BnetParty_OnReceivedInvite(
    OnlineEventType evt,
    PartyInfo party,
    ulong inviteId,
    BnetGameAccountId inviterId,
    string inviterBattletag,
    BnetGameAccountId inviteeId,
    InviteRemoveReason? reason)
  {
    if (!this.m_initialized || party.Type != PartyType.SPECTATOR_PARTY)
      return;
    PartyInvite receivedInvite = BnetParty.GetReceivedInvite(inviteId);
    bool flag1 = receivedInvite != null && (receivedInvite.IsRejoin || (BnetEntityId) receivedInvite.InviterId == (BnetEntityId) receivedInvite.InviteeId && (BnetEntityId) receivedInvite.InviteeId == (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId());
    BnetGameAccountId bnetGameAccountId = receivedInvite == null ? (BnetGameAccountId) null : this.GetPartyCreator(receivedInvite.PartyId);
    BnetPlayer inviter = receivedInvite == null ? (BnetPlayer) null : BnetUtils.GetPlayer(receivedInvite.InviterId);
    bool flag2 = false;
    bool flag3 = false;
    string str = string.Empty;
    switch (evt)
    {
      case OnlineEventType.ADDED:
        if (receivedInvite == null)
          return;
        if (flag1 || this.ShouldBePartyLeader(receivedInvite.PartyId))
        {
          if (this.ShouldBePartyLeader(receivedInvite.PartyId))
          {
            flag2 = true;
            str = "should_be_leader";
            break;
          }
          if (this.m_spectatorPartyIdMain != (BnetPartyId) null)
          {
            if (this.m_spectatorPartyIdMain == receivedInvite.PartyId)
            {
              flag2 = true;
              str = "spectating_this_party";
              break;
            }
            flag3 = true;
            str = "spectating_other_party";
            break;
          }
          flag3 = true;
          str = "not_spectating";
          if ((BnetEntityId) bnetGameAccountId != (BnetEntityId) null && (BnetEntityId) this.m_spectateeFriendlySide == (BnetEntityId) null)
          {
            this.m_spectateeFriendlySide = bnetGameAccountId;
            flag2 = true;
            flag3 = false;
            str = "rejoin_spectating";
            break;
          }
          break;
        }
        if ((BnetEntityId) receivedInvite.InviterId == (BnetEntityId) this.m_spectateeFriendlySide || (BnetEntityId) receivedInvite.InviterId == (BnetEntityId) this.m_spectateeOpposingSide || this.m_requestedInvite != null && this.m_requestedInvite.PartyId == receivedInvite.PartyId)
        {
          flag2 = true;
          str = "spectating_this_player";
          if (this.m_requestedInvite != null)
          {
            this.m_requestedInvite = (SpectatorManager.IntendedSpectateeParty) null;
            Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.SpectatePlayer_RequestInvite_FriendlySide_Timeout));
            Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.SpectatePlayer_RequestInvite_OpposingSide_Timeout));
            break;
          }
          break;
        }
        if (!UserAttentionManager.CanShowAttentionGrabber("SpectatorManager.BnetParty_OnReceivedInvite:" + (object) evt))
        {
          flag3 = true;
          str = "user_attention_blocked";
          break;
        }
        if (this.m_kickedFromSpectatingList != null)
          this.m_kickedFromSpectatingList.Remove(receivedInvite.InviterId);
        if ((UnityEngine.Object) SocialToastMgr.Get() != (UnityEngine.Object) null)
        {
          string inviterBestName = BnetUtils.GetInviterBestName(receivedInvite);
          SocialToastMgr.Get().AddToast(UserAttentionBlocker.NONE, inviterBestName, SocialToastMgr.TOAST_TYPE.SPECTATOR_INVITE_RECEIVED);
          break;
        }
        break;
      case OnlineEventType.REMOVED:
        if (!reason.HasValue || reason.Value == InviteRemoveReason.ACCEPTED)
        {
          Processor.ScheduleCallback(0.5f, false, new Processor.ScheduledCallback(this.BnetParty_OnLostPartyReference_RemoveKnownCreator), (object) party.Id);
          break;
        }
        break;
    }
    this.LogInfoParty("Spectator_OnReceivedInvite {0} rejoin={1} partyId={2} creatorId={3} accept={4} decline={5} acceptDeclineReason={6} removeReason={7}", (object) evt, (object) flag1, (object) party.Id, (object) bnetGameAccountId, (object) flag2, (object) flag3, (object) str, (object) reason);
    if (flag2)
      BnetParty.AcceptReceivedInvite(inviteId);
    else if (flag3)
    {
      BnetParty.DeclineReceivedInvite(inviteId);
    }
    else
    {
      if (this.OnInviteReceived == null)
        return;
      this.OnInviteReceived(evt, inviter);
    }
  }

  private void BnetParty_OnSentInvite(
    OnlineEventType evt,
    PartyInfo party,
    ulong inviteId,
    BnetGameAccountId inviterId,
    BnetGameAccountId inviteeId,
    bool senderIsMyself,
    InviteRemoveReason? reason)
  {
    if (party.Type != PartyType.SPECTATOR_PARTY || !senderIsMyself)
      return;
    PartyInvite sentInvite = BnetParty.GetSentInvite(party.Id, inviteId);
    BnetPlayer invitee = sentInvite == null ? (BnetPlayer) null : BnetUtils.GetPlayer(sentInvite.InviteeId);
    if (evt == OnlineEventType.ADDED)
    {
      bool flag = false;
      if (this.m_userInitiatedOutgoingInvites != null && sentInvite != null)
        flag = this.m_userInitiatedOutgoingInvites.Remove(sentInvite.InviteeId);
      if (flag && sentInvite != null && this.ShouldBePartyLeader(party.Id) && !this.m_gameServerKnownSpectators.Contains(sentInvite.InviteeId) && (UnityEngine.Object) SocialToastMgr.Get() != (UnityEngine.Object) null)
      {
        string playerBestName = BnetUtils.GetPlayerBestName(sentInvite.InviteeId);
        SocialToastMgr.Get().AddToast(UserAttentionBlocker.NONE, playerBestName, SocialToastMgr.TOAST_TYPE.SPECTATOR_INVITE_SENT);
      }
    }
    if (sentInvite == null || this.m_gameServerKnownSpectators.Contains(sentInvite.InviteeId) || this.OnInviteSent == null)
      return;
    this.OnInviteSent(evt, invitee);
  }

  private void BnetParty_OnReceivedInviteRequest(
    OnlineEventType evt,
    PartyInfo party,
    InviteRequest request,
    InviteRequestRemovedReason? reason)
  {
    if (party.Type != PartyType.SPECTATOR_PARTY || evt != OnlineEventType.ADDED)
      return;
    bool flag = false;
    if (party.Id != this.m_spectatorPartyIdMain)
      flag = true;
    if ((BnetEntityId) request.RequesterId != (BnetEntityId) null && (BnetEntityId) request.RequesterId == (BnetEntityId) request.TargetId && !Options.Get().GetBool(Option.SPECTATOR_OPEN_JOIN))
      flag = true;
    if (!this.IsInSpectableContextWithPlayer(request.RequesterId))
      flag = true;
    if (!this.IsInSpectableContextWithPlayer(request.TargetId))
      flag = true;
    if (this.m_kickedPlayers != null && (this.m_kickedPlayers.Contains(request.RequesterId) || this.m_kickedPlayers.Contains(request.TargetId)))
      flag = true;
    if (flag)
      BnetParty.IgnoreInviteRequest(party.Id, request.TargetId);
    else
      BnetParty.AcceptInviteRequest(party.Id, request.TargetId, false);
  }

  private void BnetParty_OnMemberEvent(
    OnlineEventType evt,
    PartyInfo party,
    BnetGameAccountId memberId,
    bool isRolesUpdate,
    LeaveReason? reason)
  {
    if (party.Id == (BnetPartyId) null || party.Id != this.m_spectatorPartyIdMain && party.Id != this.m_spectatorPartyIdOpposingSide)
      return;
    if (evt == OnlineEventType.ADDED && BnetParty.IsLeader(party.Id))
    {
      BnetGameAccountId partyCreator = this.GetPartyCreator(party.Id);
      if ((BnetEntityId) partyCreator != (BnetEntityId) null && (BnetEntityId) partyCreator == (BnetEntityId) memberId)
        BnetParty.SetLeader(party.Id, memberId);
    }
    if (!this.m_initialized || evt == OnlineEventType.UPDATED || !((BnetEntityId) memberId != (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId()) || !this.ShouldBePartyLeader(party.Id) || (!SceneMgr.Get().IsInGame() || !Network.Get().IsConnectedToGameServer() ? 1 : (!this.m_gameServerKnownSpectators.Contains(memberId) ? 1 : 0)) == 0)
      return;
    SocialToastMgr.TOAST_TYPE toastType = evt == OnlineEventType.ADDED ? SocialToastMgr.TOAST_TYPE.SPECTATOR_ADDED : SocialToastMgr.TOAST_TYPE.SPECTATOR_REMOVED;
    Processor.RunCoroutine(this.WaitForPresenceThenToast(memberId, toastType));
    if (this.OnSpectatorToMyGame == null)
      return;
    BnetPlayer player = BnetUtils.GetPlayer(memberId);
    this.OnSpectatorToMyGame(evt, player);
  }

  private void BnetParty_OnChatMessage(
    PartyInfo party,
    BnetGameAccountId speakerId,
    string chatMessage)
  {
  }

  private void BnetParty_OnPartyAttributeChanged_ServerInfo(PartyInfo party, Blizzard.GameService.Protocol.V2.Client.Attribute attribute)
  {
    byte[] attributeValue;
    if (party.Type != PartyType.SPECTATOR_PARTY || !BnetAttribute.GetAttributeValue<byte[]>(attribute, out attributeValue))
      return;
    PartyServerInfo from = ProtobufUtil.ParseFrom<PartyServerInfo>(attributeValue);
    if (from == null)
      return;
    if (!from.HasSecretKey || string.IsNullOrEmpty(from.SecretKey))
    {
      this.LogInfoParty("BnetParty_OnPartyAttributeChanged_ServerInfo: no secret key in serverInfo.");
    }
    else
    {
      GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
      bool flag = Network.Get().IsConnectedToGameServer() && SpectatorManager.IsSameGameAndServer(from, gameServerJoined);
      if (!flag && SceneMgr.Get().IsInGame())
      {
        this.LogInfoParty("BnetParty_OnPartyAttributeChanged_ServerInfo: cannot join game while in gameplay new={0} curr={1}.", (object) from.GameHandle, (object) gameServerJoined.GameHandle);
      }
      else
      {
        JoinInfo joinInfo = SpectatorManager.CreateJoinInfo(from);
        if (party.Id == this.m_spectatorPartyIdOpposingSide)
        {
          if (!((UnityEngine.Object) GameMgr.Get().GetTransitionPopup() == (UnityEngine.Object) null) || !GameMgr.Get().IsSpectator())
            return;
          this.SpectateSecondPlayer_Network(joinInfo);
        }
        else
        {
          if (flag || !(party.Id == this.m_spectatorPartyIdMain))
            return;
          this.LogInfoPower("================== Start Spectator Game ==================");
          this.m_isExpectingArriveInGameplayAsSpectator = true;
          GameMgr.Get().SpectateGame(joinInfo);
          this.CloseWaitingForNextGameDialog();
        }
      }
    }
  }

  private static bool IsGameOver => GameState.Get() != null && GameState.Get().IsGameOverNowOrPending();

  private void LogInfoParty(string format, params object[] args) => Log.Party.Print(format, args);

  private void LogInfoPower(string format, params object[] args)
  {
    Log.Party.Print(format, args);
    Log.Power.Print(format, args);
  }

  private bool IsPlayerInGame(BnetGameAccountId gameAccountId)
  {
    GameState gameState = GameState.Get();
    if (gameState == null)
      return false;
    foreach (KeyValuePair<int, Player> player in gameState.GetPlayerMap())
    {
      BnetPlayer bnetPlayer = player.Value.GetBnetPlayer();
      if (bnetPlayer != null && (BnetEntityId) bnetPlayer.GetHearthstoneGameAccountId() == (BnetEntityId) gameAccountId)
        return true;
    }
    return false;
  }

  private bool IsStillInParty(BnetPartyId partyId) => BnetParty.IsInParty(partyId) && (this.m_leavePartyIdsRequested == null || !this.m_leavePartyIdsRequested.Contains(partyId));

  private void BnetPresenceMgr_OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
    BnetPlayerChange myOwnChange = changelist.FindChange(myGameAccountId);
    if (myOwnChange != null)
    {
      bool flag1 = myOwnChange.GetNewPlayer().IsAppearingOffline();
      bool flag2 = myOwnChange.GetOldPlayer().IsAppearingOffline();
      if (flag1 && !flag2 && this.MyGameHasSpectators())
      {
        foreach (BnetGameAccountId id in ((IEnumerable<BnetGameAccountId>) this.GetSpectatorPartyMembers()).ToArray<BnetGameAccountId>())
          this.KickSpectator_Internal(BnetPresenceMgr.Get().GetPlayer(id), true, false);
      }
      else if (flag2 && !flag1)
        this.UpdateMySpectatorInfo();
    }
    if (!this.IsBeingSpectated())
      return;
    foreach (BnetPlayerChange bnetPlayerChange in changelist.GetChanges().Where<BnetPlayerChange>((Func<BnetPlayerChange, bool>) (c => c != myOwnChange && c.GetOldPlayer() != null && c.GetOldPlayer().IsOnline() && !c.GetNewPlayer().IsOnline())))
      this.KickSpectator_Internal(BnetPresenceMgr.Get().GetPlayer(bnetPlayerChange.GetPlayer().GetAccountId()), true, false);
  }

  private void RemoveReceivedInvitation(BnetGameAccountId inviterId)
  {
    if ((BnetEntityId) inviterId == (BnetEntityId) null || !this.m_receivedSpectateMeInvites.Remove(inviterId))
      return;
    BnetPlayer player = BnetUtils.GetPlayer(inviterId);
    if (this.OnInviteReceived == null)
      return;
    this.OnInviteReceived(OnlineEventType.REMOVED, player);
  }

  private void RemoveSentInvitation(BnetGameAccountId inviteeId)
  {
    if ((BnetEntityId) inviteeId == (BnetEntityId) null || !this.m_sentSpectateMeInvites.Remove(inviteeId))
      return;
    BnetPlayer player = BnetUtils.GetPlayer(inviteeId);
    if (this.OnInviteSent == null)
      return;
    this.OnInviteSent(OnlineEventType.REMOVED, player);
  }

  private void DeclineAllReceivedInvitations()
  {
    foreach (PartyInvite receivedInvite in BnetParty.GetReceivedInvites())
    {
      if (receivedInvite.PartyType == PartyType.SPECTATOR_PARTY)
        BnetParty.DeclineReceivedInvite(receivedInvite.InviteId);
    }
  }

  private void RevokeAllSentInvitations()
  {
    this.ClearAllSentInvitations();
    BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
    BnetPartyId[] bnetPartyIdArray = new BnetPartyId[2]
    {
      this.m_spectatorPartyIdMain,
      this.m_spectatorPartyIdOpposingSide
    };
    foreach (BnetPartyId partyId in bnetPartyIdArray)
    {
      if (!(partyId == (BnetPartyId) null))
      {
        foreach (PartyInvite sentInvite in BnetParty.GetSentInvites(partyId))
        {
          if (!((BnetEntityId) sentInvite.InviterId != (BnetEntityId) myGameAccountId))
            BnetParty.RevokeSentInvite(partyId, sentInvite.InviteId);
        }
      }
    }
  }

  private void ClearAllSentInvitations()
  {
    BnetGameAccountId[] array = this.m_sentSpectateMeInvites.Keys.ToArray<BnetGameAccountId>();
    this.m_sentSpectateMeInvites.Clear();
    if (this.OnInviteSent == null)
      return;
    foreach (BnetGameAccountId id in array)
      this.OnInviteSent(OnlineEventType.REMOVED, BnetUtils.GetPlayer(id));
  }

  private void AddKnownSpectator(BnetGameAccountId gameAccountId)
  {
    if ((BnetEntityId) gameAccountId == (BnetEntityId) null)
      return;
    int num1 = this.m_gameServerKnownSpectators.Add(gameAccountId) ? 1 : 0;
    this.CreatePartyIfNecessary();
    this.RemoveSentInvitation(gameAccountId);
    this.RemoveReceivedInvitation(gameAccountId);
    if (num1 == 0)
      return;
    if (SceneMgr.Get().IsInGame() && Network.Get().IsConnectedToGameServer())
    {
      int num2 = BnetParty.IsMember(this.m_spectatorPartyIdMain, gameAccountId) ? 1 : 0;
      BnetPlayer player = BnetUtils.GetPlayer(gameAccountId);
      if (num2 == 0)
        Processor.RunCoroutine(this.WaitForPresenceThenToast(gameAccountId, SocialToastMgr.TOAST_TYPE.SPECTATOR_ADDED));
      if (this.OnSpectatorToMyGame != null)
        this.OnSpectatorToMyGame(OnlineEventType.ADDED, player);
    }
    this.UpdateSpectatorPresence();
  }

  private void RemoveKnownSpectator(BnetGameAccountId gameAccountId)
  {
    if ((BnetEntityId) gameAccountId == (BnetEntityId) null || !this.m_gameServerKnownSpectators.Remove(gameAccountId))
      return;
    if (SceneMgr.Get().IsInGame() && Network.Get().IsConnectedToGameServer())
    {
      int num = BnetParty.IsMember(this.m_spectatorPartyIdMain, gameAccountId) ? 1 : 0;
      BnetPlayer player = BnetUtils.GetPlayer(gameAccountId);
      if (num == 0)
        Processor.RunCoroutine(this.WaitForPresenceThenToast(gameAccountId, SocialToastMgr.TOAST_TYPE.SPECTATOR_REMOVED));
      if (this.OnSpectatorToMyGame != null)
        this.OnSpectatorToMyGame(OnlineEventType.REMOVED, player);
    }
    this.UpdateSpectatorPresence();
  }

  private void ClearAllGameServerKnownSpectators()
  {
    BnetGameAccountId[] array = this.m_gameServerKnownSpectators.ToArray<BnetGameAccountId>();
    this.m_gameServerKnownSpectators.Clear();
    if (this.OnSpectatorToMyGame != null && SceneMgr.Get().IsInGame() && Network.Get().IsConnectedToGameServer())
    {
      foreach (BnetGameAccountId id in array)
        this.OnSpectatorToMyGame(OnlineEventType.REMOVED, BnetUtils.GetPlayer(id));
    }
    if (array.Length == 0)
      return;
    this.UpdateSpectatorPresence();
  }

  private void UpdateSpectatorPresence()
  {
    if ((UnityEngine.Object) HearthstoneApplication.Get() != (UnityEngine.Object) null)
    {
      Processor.CancelScheduledCallback(new Processor.ScheduledCallback(this.SpectatorManager_UpdatePresenceNextFrame));
      Processor.ScheduleCallback(0.0f, true, new Processor.ScheduledCallback(this.SpectatorManager_UpdatePresenceNextFrame));
    }
    else
      this.SpectatorManager_UpdatePresenceNextFrame((object) null);
  }

  private void SpectatorManager_UpdatePresenceNextFrame(object userData)
  {
    bool flag = Options.Get().GetBool(Option.SPECTATOR_OPEN_JOIN) || this.IsInSpectatorMode();
    JoinInfo myGameJoinInfo = this.GetMyGameJoinInfo();
    if (Network.ShouldBeConnectedToAurora() && Network.IsLoggedIn())
      BnetPresenceMgr.Get().SetPresenceSpectatorJoinInfo(flag ? myGameJoinInfo : (JoinInfo) null);
    PartyManager.Get().UpdateSpectatorJoinInfo(myGameJoinInfo);
  }

  private void UpdateSpectatorPartyServerInfo()
  {
    if (this.m_spectatorPartyIdMain == (BnetPartyId) null)
      return;
    if (!this.ShouldBePartyLeader(this.m_spectatorPartyIdMain))
    {
      if (!BnetParty.IsLeader(this.m_spectatorPartyIdMain))
        return;
      BattleNet.ClearPartyAttribute(this.m_spectatorPartyIdMain, "WTCG.Party.ServerInfo");
    }
    else
    {
      byte[] arr2;
      BattleNet.GetPartyAttribute<byte[]>(this.m_spectatorPartyIdMain, "WTCG.Party.ServerInfo", out arr2);
      GameServerInfo gameServerJoined = Network.Get().GetLastGameServerJoined();
      if (SpectatorManager.IsGameOver || !SceneMgr.Get().IsInGame() || !Network.Get().IsConnectedToGameServer() || gameServerJoined == null || string.IsNullOrEmpty(gameServerJoined.Address))
      {
        if (arr2 == null)
          return;
        BattleNet.ClearPartyAttribute(this.m_spectatorPartyIdMain, "WTCG.Party.ServerInfo");
      }
      else
      {
        byte[] byteArray = ProtobufUtil.ToByteArray((IProtoBuf) new PartyServerInfo()
        {
          ServerIpAddress = gameServerJoined.Address,
          ServerPort = gameServerJoined.Port,
          GameHandle = (int) gameServerJoined.GameHandle,
          SecretKey = (gameServerJoined.SpectatorPassword ?? ""),
          GameType = GameMgr.Get().GetGameType(),
          FormatType = GameMgr.Get().GetFormatType(),
          MissionId = GameMgr.Get().GetMissionId()
        });
        if (GeneralUtils.AreArraysEqual<byte>(byteArray, arr2))
          return;
        BattleNet.SetPartyAttributes(this.m_spectatorPartyIdMain, BnetAttribute.CreateAttribute("WTCG.Party.ServerInfo", byteArray));
      }
    }
  }

  private bool ShouldBePartyLeader(BnetPartyId partyId)
  {
    if (GameMgr.Get().IsSpectator() || (BnetEntityId) this.m_spectateeFriendlySide != (BnetEntityId) null || (BnetEntityId) this.m_spectateeOpposingSide != (BnetEntityId) null)
      return false;
    BnetGameAccountId partyCreator = this.GetPartyCreator(partyId);
    return !((BnetEntityId) partyCreator == (BnetEntityId) null) && !((BnetEntityId) partyCreator != (BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId());
  }

  private BnetGameAccountId GetPartyCreator(BnetPartyId partyId)
  {
    if (partyId == (BnetPartyId) null)
      return (BnetGameAccountId) null;
    BnetGameAccountId partyCreator = (BnetGameAccountId) null;
    if (this.m_knownPartyCreatorIds.TryGetValue(partyId, out partyCreator) && (BnetEntityId) partyCreator != (BnetEntityId) null)
      return partyCreator;
    byte[] bytes;
    if (!BattleNet.GetPartyAttribute<byte[]>(partyId, "WTCG.Party.Creator", out bytes))
      return (BnetGameAccountId) null;
    partyCreator = BnetGameAccountId.CreateFromNet(ProtobufUtil.ParseFrom<BnetId>(bytes));
    if (partyCreator.IsValid())
      this.m_knownPartyCreatorIds[partyId] = partyCreator;
    return partyCreator;
  }

  private bool CreatePartyIfNecessary()
  {
    if (!Network.ShouldBeConnectedToAurora())
      return false;
    if (this.m_spectatorPartyIdMain != (BnetPartyId) null)
    {
      if ((BnetEntityId) this.GetPartyCreator(this.m_spectatorPartyIdMain) != (BnetEntityId) null && !this.ShouldBePartyLeader(this.m_spectatorPartyIdMain))
        return false;
      PartyInfo[] joinedParties = BnetParty.GetJoinedParties();
      if (((IEnumerable<PartyInfo>) joinedParties).FirstOrDefault<PartyInfo>((Func<PartyInfo, bool>) (i => i.Id == this.m_spectatorPartyIdMain && i.Type == PartyType.SPECTATOR_PARTY)) == null)
      {
        this.LogInfoParty("CreatePartyIfNecessary stored PartyId={0} is not in joined party list: {1}", (object) this.m_spectatorPartyIdMain, (object) string.Join(", ", ((IEnumerable<PartyInfo>) joinedParties).Select<PartyInfo, string>((Func<PartyInfo, string>) (i => i.ToString())).ToArray<string>()));
        this.m_spectatorPartyIdMain = (BnetPartyId) null;
        this.UpdateSpectatorPresence();
      }
      PartyInfo partyInfo = ((IEnumerable<PartyInfo>) joinedParties).FirstOrDefault<PartyInfo>((Func<PartyInfo, bool>) (i => i.Type == PartyType.SPECTATOR_PARTY));
      if (partyInfo != null && this.m_spectatorPartyIdMain != partyInfo.Id)
      {
        this.LogInfoParty("CreatePartyIfNecessary repairing mismatching PartyIds current={0} new={1}", (object) this.m_spectatorPartyIdMain, (object) partyInfo.Id);
        this.m_spectatorPartyIdMain = partyInfo.Id;
        this.UpdateSpectatorPresence();
      }
      if (this.m_spectatorPartyIdMain != (BnetPartyId) null)
        return false;
    }
    if (this.GetCountSpectatingMe() <= 0)
      return false;
    BnetParty.CreateParty(PartyType.SPECTATOR_PARTY, ChannelApi.PartyPrivacyLevel.OpenInvitation, ProtobufUtil.ToByteArray((IProtoBuf) BnetUtils.CreatePegasusBnetId((BnetEntityId) BnetPresenceMgr.Get().GetMyGameAccountId())), (BnetParty.CreateSuccessCallback) null);
    return true;
  }

  private void ReinviteKnownSpectatorsNotInParty()
  {
    if (this.m_spectatorPartyIdMain == (BnetPartyId) null || !this.ShouldBePartyLeader(this.m_spectatorPartyIdMain))
      return;
    BnetParty.PartyMember[] members = BnetParty.GetMembers(this.m_spectatorPartyIdMain);
    foreach (BnetGameAccountId serverKnownSpectator in this.m_gameServerKnownSpectators)
    {
      BnetGameAccountId knownSpectator = serverKnownSpectator;
      if (((IEnumerable<BnetParty.PartyMember>) members).FirstOrDefault<BnetParty.PartyMember>((Func<BnetParty.PartyMember, bool>) (m => (BnetEntityId) m.GameAccountId == (BnetEntityId) knownSpectator)) == null)
        BnetParty.SendInvite(this.m_spectatorPartyIdMain, knownSpectator, false);
    }
  }

  private void LeaveParty(BnetPartyId partyId, bool dissolve)
  {
    if (partyId == (BnetPartyId) null)
      return;
    if (this.m_leavePartyIdsRequested == null)
      this.m_leavePartyIdsRequested = new HashSet<BnetPartyId>();
    this.m_leavePartyIdsRequested.Add(partyId);
    if (dissolve)
      BnetParty.DissolveParty(partyId);
    else
      BnetParty.Leave(partyId);
  }

  public void LeaveGameScene()
  {
    if ((UnityEngine.Object) EndGameScreen.Get() != (UnityEngine.Object) null)
    {
      EndGameScreen.Get().m_hitbox.TriggerPress();
      EndGameScreen.Get().m_hitbox.TriggerRelease();
    }
    else
    {
      if (HearthstoneApplication.Get().IsResetting())
        return;
      SceneMgr.Mode postGameSceneMode = GameMgr.Get().GetPostGameSceneMode();
      SceneMgr.Get().SetNextMode(postGameSceneMode);
    }
  }

  private IEnumerator WaitForPresenceThenToast(
    BnetGameAccountId gameAccountId,
    SocialToastMgr.TOAST_TYPE toastType)
  {
    float timeStarted = UnityEngine.Time.time;
    for (float num = UnityEngine.Time.time - timeStarted; (double) num < 30.0 && !BnetUtils.HasPlayerBestNamePresence(gameAccountId); num = UnityEngine.Time.time - timeStarted)
      yield return (object) null;
    if ((UnityEngine.Object) SocialToastMgr.Get() != (UnityEngine.Object) null)
    {
      string playerBestName = BnetUtils.GetPlayerBestName(gameAccountId);
      SocialToastMgr.Get().AddToast(UserAttentionBlocker.NONE, playerBestName, toastType);
    }
  }

  private SpectatorManager()
  {
  }

  private static SpectatorManager CreateInstance()
  {
    SpectatorManager.s_instance = new SpectatorManager();
    HearthstoneApplication.Get().WillReset += new System.Action(SpectatorManager.s_instance.WillReset);
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(SpectatorManager.s_instance.OnFindGameEvent));
    SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(SpectatorManager.s_instance.OnSceneUnloaded));
    GameState.RegisterGameStateInitializedListener(new GameState.GameStateInitializedCallback(SpectatorManager.s_instance.GameState_InitializedEvent));
    Options.Get().RegisterChangedListener(Option.SPECTATOR_OPEN_JOIN, new Options.ChangedCallback(SpectatorManager.s_instance.OnSpectatorOpenJoinOptionChanged));
    BnetPresenceMgr.Get().OnGameAccountPresenceChange += new System.Action<PresenceUpdate[]>(SpectatorManager.s_instance.Presence_OnGameAccountPresenceChange);
    BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(SpectatorManager.s_instance.BnetFriendMgr_OnFriendsChanged));
    FiresideGatheringManager.OnPatronListUpdated += new FiresideGatheringManager.OnPatronListUpdatedCallback(SpectatorManager.s_instance.FiresideGatheringManager_OnPatronListUpdated);
    EndGameScreen.OnTwoScoopsShown += new EndGameScreen.OnTwoScoopsShownHandler(SpectatorManager.s_instance.EndGameScreen_OnTwoScoopsShown);
    EndGameScreen.OnBackOutOfGameplay += new System.Action(SpectatorManager.s_instance.EndGameScreen_OnBackOutOfGameplay);
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(SpectatorManager.s_instance.BnetPresenceMgr_OnPlayersChanged));
    Network.Get().OnDisconnectedFromBattleNet += new System.Action<BattleNetErrors>(SpectatorManager.s_instance.OnDisconnect);
    Network.Get().RegisterNetHandler((object) SpectatorNotify.PacketID.ID, new Network.NetHandler(SpectatorManager.s_instance.Network_OnSpectatorNotifyEvent));
    BnetParty.OnError += new BnetParty.PartyErrorHandler(SpectatorManager.s_instance.BnetParty_OnError);
    BnetParty.OnJoined += new BnetParty.JoinedHandler(SpectatorManager.s_instance.BnetParty_OnJoined);
    BnetParty.OnReceivedInvite += new BnetParty.ReceivedInviteHandler(SpectatorManager.s_instance.BnetParty_OnReceivedInvite);
    BnetParty.OnSentInvite += new BnetParty.SentInviteHandler(SpectatorManager.s_instance.BnetParty_OnSentInvite);
    BnetParty.OnReceivedInviteRequest += new BnetParty.ReceivedInviteRequestHandler(SpectatorManager.s_instance.BnetParty_OnReceivedInviteRequest);
    BnetParty.OnMemberEvent += new BnetParty.MemberEventHandler(SpectatorManager.s_instance.BnetParty_OnMemberEvent);
    BnetParty.OnChatMessage += new BnetParty.ChatMessageHandler(SpectatorManager.s_instance.BnetParty_OnChatMessage);
    BnetParty.RegisterAttributeChangedHandler("WTCG.Party.ServerInfo", new BnetParty.PartyAttributeChangedHandler(SpectatorManager.s_instance.BnetParty_OnPartyAttributeChanged_ServerInfo));
    return SpectatorManager.s_instance;
  }

  public delegate void InviteReceivedHandler(OnlineEventType evt, BnetPlayer inviter);

  public delegate void InviteSentHandler(OnlineEventType evt, BnetPlayer invitee);

  public delegate void SpectatorToMyGameHandler(OnlineEventType evt, BnetPlayer spectator);

  public delegate void SpectatorModeChangedHandler(OnlineEventType evt, BnetPlayer spectatee);

  private struct ReceivedInvite
  {
    public float m_timestamp;
    public JoinInfo m_joinInfo;
  }

  private class IntendedSpectateeParty
  {
    public BnetGameAccountId SpectateeId;
    public BnetPartyId PartyId;

    public IntendedSpectateeParty(BnetGameAccountId spectateeId, BnetPartyId partyId)
    {
      this.SpectateeId = spectateeId;
      this.PartyId = partyId;
    }
  }

  private class PendingSpectatePlayer
  {
    public BnetGameAccountId SpectateeId;
    public JoinInfo JoinInfo;

    public PendingSpectatePlayer(BnetGameAccountId spectateeId, JoinInfo joinInfo)
    {
      this.SpectateeId = spectateeId;
      this.JoinInfo = joinInfo;
    }
  }
}
