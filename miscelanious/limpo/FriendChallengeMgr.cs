using Assets;
using Blizzard.GameService.SDK.Client.Integration;
using Blizzard.T5.Core.Utils;
using Hearthstone;
using Hearthstone.Core;
using PegasusShared;
using PegasusUtil;
using SpectatorProto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class FriendChallengeMgr
{
  private static FriendChallengeMgr s_instance;
  private bool m_netCacheReady;
  private bool m_myPlayerReady;
  private FriendlyChallengeData m_data = new FriendlyChallengeData();
  private bool m_hasPreSelectedDeckOrHero;
  private long m_preSelectedDeckId;
  private long m_preSelectedHeroId;
  private FriendChallengeMgr.ChallengeMethod m_challengeMethod;
  private List<FriendChallengeMgr.ChangedListener> m_changedListeners = new List<FriendChallengeMgr.ChangedListener>();
  private DialogBase m_challengeDialog;
  private bool m_hasSeenDeclinedReason;
  private bool m_canBeInvitedToGame;
  private bool m_canBeInvitedToBattlegrounds;
  private bool m_canBeInvitedToMercenaries;
  private bool m_updateMyAvailabilityCallbackScheduledThisFrame;

  public static FriendChallengeMgr Get()
  {
    if (FriendChallengeMgr.s_instance == null)
    {
      FriendChallengeMgr.s_instance = new FriendChallengeMgr();
      HearthstoneApplication.Get().WillReset += new System.Action(FriendChallengeMgr.s_instance.WillReset);
      AchieveManager.Get().RegisterAchievesUpdatedListener(new AchieveManager.AchievesUpdatedCallback(FriendChallengeMgr.s_instance.AchieveManager_OnAchievesUpdated));
      BnetParty.OnJoined += new BnetParty.JoinedHandler(FriendChallengeMgr.s_instance.BnetParty_OnJoined);
      BnetParty.OnReceivedInvite += new BnetParty.ReceivedInviteHandler(FriendChallengeMgr.s_instance.BnetParty_OnReceivedInvite);
      BnetParty.OnPartyAttributeChanged += new BnetParty.PartyAttributeChangedHandler(FriendChallengeMgr.s_instance.BnetParty_OnPartyAttributeChanged);
      BnetParty.OnMemberEvent += new BnetParty.MemberEventHandler(FriendChallengeMgr.s_instance.BnetParty_OnMemberEvent);
      BnetParty.OnSentInvite += new BnetParty.SentInviteHandler(FriendChallengeMgr.s_instance.BnetParty_OnSentInvite);
    }
    return FriendChallengeMgr.s_instance;
  }

  public void OnLoggedIn()
  {
    NetCache.Get().RegisterFriendChallenge(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    SceneMgr.Get().RegisterSceneUnloadedEvent(new SceneMgr.SceneUnloadedCallback(this.OnSceneUnloaded));
    SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
    BnetPresenceMgr.Get().AddPlayersChangedListener(new BnetPresenceMgr.PlayersChangedCallback(this.OnPlayersChanged));
    BnetFriendMgr.Get().AddChangeListener(new BnetFriendMgr.ChangeCallback(this.OnFriendsChanged));
    BnetNearbyPlayerMgr.Get().AddChangeListener(new BnetNearbyPlayerMgr.ChangeCallback(this.OnNearbyPlayersChanged));
    GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
    FatalErrorMgr.Get().AddErrorListener(new FatalErrorMgr.ErrorCallback(this.OnFatalError));
    LoginManager.Get().OnInitialClientStateReceived += new System.Action(this.OnReconnectLoginComplete);
    this.AddChangedListener(new FriendChallengeMgr.ChangedCallback(this.OnChallengeChanged));
    Network.Get().OnDisconnectedFromBattleNet += new System.Action<BattleNetErrors>(this.OnDisconnectedFromBattleNet);
    BnetPresenceMgr.Get().SetGameField(19U, BattleNet.GetVersion());
    BnetPresenceMgr.Get().SetGameField(20U, BattleNet.GetEnvironment());
  }

  private void BnetParty_OnJoined(OnlineEventType evt, PartyInfo party, LeaveReason? reason)
  {
    if (party.Type != PartyType.FRIENDLY_CHALLENGE)
      return;
    switch (evt)
    {
      case OnlineEventType.ADDED:
        if (this.DidSendChallenge() && !BnetParty.IsLeader(party.Id))
        {
          BnetParty.DissolveParty(party.Id);
          break;
        }
        if (this.m_data.m_partyId != (BnetPartyId) null && this.m_data.m_partyId != party.Id)
        {
          BnetParty.DissolveParty(party.Id);
          break;
        }
        this.m_data.m_partyId = party.Id;
        long num1;
        if (BattleNet.GetPartyAttribute<long>(party.Id, "WTCG.Game.ScenarioId", out num1))
          this.m_data.m_scenarioId = (int) num1;
        long num2;
        this.m_data.m_challengeFormatType = !BattleNet.GetPartyAttribute<long>(party.Id, "WTCG.Format.Type", out num2) ? PegasusShared.FormatType.FT_UNKNOWN : (PegasusShared.FormatType) num2;
        long num3;
        if (BattleNet.GetPartyAttribute<long>(party.Id, "WTCG.Brawl.Type", out num3))
        {
          if (num3 >= 1L && num3 < 3L)
            this.m_data.m_challengeBrawlType = (BrawlType) num3;
        }
        else
          this.m_data.m_challengeBrawlType = BrawlType.BRAWL_TYPE_UNKNOWN;
        int num4;
        this.m_data.m_seasonId = !BattleNet.GetPartyAttribute<int>(party.Id, "WTCG.Season.Id", out num4) ? 0 : num4;
        int num5;
        this.m_data.m_brawlLibraryItemId = !BattleNet.GetPartyAttribute<int>(party.Id, "WTCG.Brawl.LibraryItemId", out num5) ? 0 : num5;
        string name = this.DidSendChallenge() ? "s1" : "s2";
        BattleNet.SetPartyAttributes(party.Id, BnetAttribute.CreateAttribute(name, "wait"));
        this.UpdateMyFsgSharedSecret(party.Id, FiresideGatheringManager.Get().CurrentFsgSharedSecretKey);
        this.m_data.m_challengerDeckShareState = "none";
        this.m_data.m_challengeeDeckShareState = "none";
        this.m_data.m_sharedDecks = (List<CollectionDeck>) null;
        if (this.DidSendChallenge())
        {
          BnetParty.SendInvite(party.Id, this.m_data.m_challengee.GetHearthstoneGameAccountId(), true);
        }
        else
        {
          Blizzard.GameService.Protocol.V2.Client.Attribute[] attributes;
          BattleNet.GetAllPartyAttributes(party.Id, out attributes);
          foreach (Blizzard.GameService.Protocol.V2.Client.Attribute attribute in attributes)
            this.BnetParty_OnPartyAttributeChanged(party, attribute);
        }
        if (this.m_data.m_challengerDeckId != 0L)
          this.SelectDeck(this.m_data.m_challengerDeckId);
        if (this.m_data.m_challengerHeroId == 0L)
          break;
        this.SelectHero(this.m_data.m_challengerHeroId);
        break;
      case OnlineEventType.REMOVED:
        if (!((IEnumerable<PartyInfo>) BnetParty.GetJoinedParties()).Any<PartyInfo>((Func<PartyInfo, bool>) (i => i.Type == PartyType.FRIENDLY_CHALLENGE)))
          this.m_data.m_scenarioId = 2;
        if (!(party.Id == this.m_data.m_partyId))
          break;
        string data = reason.HasValue ? ((int) reason.Value).ToString() : "NO_SUPPLIED_REASON";
        this.PushPartyEvent(party.Id, "left", data);
        break;
    }
  }

  private void BnetParty_OnReceivedInvite(
    OnlineEventType evt,
    PartyInfo party,
    ulong inviteId,
    BnetGameAccountId inviter,
    string inviterBattletag,
    BnetGameAccountId invitee,
    InviteRemoveReason? reason)
  {
    if (party.Type != PartyType.FRIENDLY_CHALLENGE || evt != OnlineEventType.ADDED)
      return;
    if (!PartyManager.IsPartyTypeEnabledInGuardian(party.Type))
      BnetParty.DeclineReceivedInvite(inviteId);
    else if (BnetParty.IsInParty(this.m_data.m_partyId) || this.DidSendChallenge())
      BnetParty.DeclineReceivedInvite(inviteId);
    else if (!GameUtils.IsTraditionalTutorialComplete())
      BnetParty.DeclineReceivedInvite(inviteId);
    else
      BnetParty.AcceptReceivedInvite(inviteId);
  }

  private void BnetParty_OnPartyAttributeChanged(PartyInfo party, Blizzard.GameService.Protocol.V2.Client.Attribute attribute)
  {
    if (party.Type != PartyType.FRIENDLY_CHALLENGE || this.m_data.m_partyId != party.Id)
      return;
    switch (attribute.Name)
    {
      case "WTCG.Friendly.DeclineReason":
        this.BnetParty_OnPartyAttributeChanged_DeclineReason(party, attribute);
        break;
      case "d1":
        this.m_data.m_challengerDeckId = attribute.Value.HasIntValue ? attribute.Value.IntValue : 0L;
        this.m_data.m_challengerDeckOrHeroSelected = this.m_data.m_challengerDeckId > 0L;
        break;
      case "d2":
        this.m_data.m_challengeeDeckId = attribute.Value.HasIntValue ? attribute.Value.IntValue : 0L;
        this.m_data.m_challengeeDeckOrHeroSelected = this.m_data.m_challengeeDeckId > 0L;
        break;
      case "error":
        this.BnetParty_OnPartyAttributeChanged_Error(party, attribute);
        break;
      case "fsg1":
        this.m_data.m_challengerFsgSharedSecret = attribute.Value.HasBlobValue ? attribute.Value.BlobValue.ToByteArray() : (byte[]) null;
        break;
      case "fsg2":
        this.m_data.m_challengeeFsgSharedSecret = attribute.Value.HasBlobValue ? attribute.Value.BlobValue.ToByteArray() : (byte[]) null;
        break;
      case "hero1":
        this.m_data.m_challengerHeroId = attribute.Value.HasIntValue ? attribute.Value.IntValue : 0L;
        this.m_data.m_challengerDeckOrHeroSelected = this.m_data.m_challengerHeroId > 0L;
        break;
      case "hero2":
        this.m_data.m_challengeeHeroId = attribute.Value.HasIntValue ? attribute.Value.IntValue : 0L;
        this.m_data.m_challengeeDeckOrHeroSelected = this.m_data.m_challengeeHeroId > 0L;
        break;
      case "p1CardBack":
        this.m_data.m_challengerCardBackId = new long?();
        if (attribute.Value.HasIntValue)
        {
          this.m_data.m_challengerCardBackId = new long?(attribute.Value.IntValue);
          break;
        }
        break;
      case "p2CardBack":
        this.m_data.m_challengeeCardBackId = new long?();
        if (attribute.Value.HasIntValue)
        {
          this.m_data.m_challengeeCardBackId = new long?(attribute.Value.IntValue);
          break;
        }
        break;
      case "randomHero1":
        this.m_data.m_challengerRandomHeroCardId = new long?(attribute.Value.HasIntValue ? attribute.Value.IntValue : 0L);
        break;
      case "randomHero2":
        this.m_data.m_challengeeRandomHeroCardId = new long?(attribute.Value.HasIntValue ? attribute.Value.IntValue : 0L);
        break;
    }
    BnetGameAccountId otherPlayerGameAccountId = (BnetGameAccountId) null;
    if (this.DidSendChallenge())
    {
      if (this.m_data.m_challengee != null)
        otherPlayerGameAccountId = this.m_data.m_challengee.GetHearthstoneGameAccountId();
    }
    else if (this.m_data.m_challenger != null)
      otherPlayerGameAccountId = this.m_data.m_challenger.GetHearthstoneGameAccountId();
    if ((BnetEntityId) otherPlayerGameAccountId == (BnetEntityId) null)
    {
      BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
      foreach (BnetParty.PartyMember member in BnetParty.GetMembers(party.Id))
      {
        if ((BnetEntityId) member.GameAccountId != (BnetEntityId) myGameAccountId)
        {
          otherPlayerGameAccountId = member.GameAccountId;
          break;
        }
      }
    }
    string data = attribute.Value.HasStringValue ? attribute.Value.StringValue : string.Empty;
    this.PushPartyEvent(party.Id, attribute.Name, data, otherPlayerGameAccountId);
  }

  private void BnetParty_OnPartyAttributeChanged_DeclineReason(PartyInfo party, Blizzard.GameService.Protocol.V2.Client.Attribute attribute)
  {
    if (party.Type != PartyType.FRIENDLY_CHALLENGE || !this.DidSendChallenge() || !attribute.Value.HasIntValue)
      return;
    FriendChallengeMgr.DeclineReason intValue = (FriendChallengeMgr.DeclineReason) attribute.Value.IntValue;
    string key = (string) null;
    switch (intValue)
    {
      case FriendChallengeMgr.DeclineReason.NoValidDeck:
        key = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_DECK";
        break;
      case FriendChallengeMgr.DeclineReason.StandardNoValidDeck:
        key = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_STANDARD_DECK";
        break;
      case FriendChallengeMgr.DeclineReason.TavernBrawlNoValidDeck:
        key = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_TAVERN_BRAWL_DECK";
        break;
      case FriendChallengeMgr.DeclineReason.TavernBrawlNotUnlocked:
        key = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_TAVERN_BRAWL_LOCKED";
        break;
      case FriendChallengeMgr.DeclineReason.UserIsBusy:
        key = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_USER_IS_BUSY";
        break;
      case FriendChallengeMgr.DeclineReason.NotSeenWild:
        key = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NOT_SEEN_WILD";
        break;
      case FriendChallengeMgr.DeclineReason.BattlegroundsNoEarlyAccess:
        key = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_BATTLEGROUNDS_EARLY_ACCESS";
        break;
      case FriendChallengeMgr.DeclineReason.ClassicNoValidDeck:
        key = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_CLASSIC_DECK";
        break;
      case FriendChallengeMgr.DeclineReason.BattlegroundsTutorialNotComplete:
        key = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_BATTLEGROUNDS_TUTORIAL_COMPLETE";
        break;
      case FriendChallengeMgr.DeclineReason.MercsTutorialNotComplete:
        key = "GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGEE_NO_MERCS_TUTORIAL_COMPLETE";
        break;
    }
    if (key == null)
      return;
    if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null)
    {
      this.m_challengeDialog.Hide();
      this.m_challengeDialog = (DialogBase) null;
    }
    this.m_hasSeenDeclinedReason = true;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
      m_text = GameStrings.Get(key),
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  private void BnetParty_OnPartyAttributeChanged_Error(PartyInfo party, Blizzard.GameService.Protocol.V2.Client.Attribute attribute)
  {
    if (party.Type != PartyType.FRIENDLY_CHALLENGE)
      return;
    if (this.DidReceiveChallenge() && attribute.Value.HasIntValue)
    {
      Log.Party.Print(Blizzard.T5.Logging.LogLevel.Error, "BnetParty_OnPartyAttributeChanged_Error - code={0}", (object) attribute.Value.IntValue);
      BnetErrorInfo info = new BnetErrorInfo(BnetFeature.Games, BnetFeatureEvent.Games_OnCreated, (BattleNetErrors) attribute.Value.IntValue);
      GameMgr.Get().OnBnetError(info, (object) null);
    }
    if (!BnetParty.IsLeader(party.Id) || BnetAttribute.IsNone(attribute))
      return;
    BattleNet.ClearPartyAttribute(party.Id, attribute.Name);
  }

  private void BnetParty_OnMemberEvent(
    OnlineEventType evt,
    PartyInfo party,
    BnetGameAccountId memberId,
    bool isRolesUpdate,
    LeaveReason? reason)
  {
    if (party.Type != PartyType.FRIENDLY_CHALLENGE || evt != OnlineEventType.REMOVED || !BnetParty.IsInParty(party.Id))
      return;
    BnetParty.DissolveParty(party.Id);
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
    if (party.Type != PartyType.FRIENDLY_CHALLENGE || evt != OnlineEventType.REMOVED)
      return;
    InviteRemoveReason? nullable = reason;
    InviteRemoveReason inviteRemoveReason = InviteRemoveReason.DECLINED;
    if (!(nullable.GetValueOrDefault() == inviteRemoveReason & nullable.HasValue))
      return;
    this.DeclineFriendChallenge_Internal(party.Id);
    if (!(party.Id == this.m_data.m_partyId))
      return;
    this.FireChangedEvent(FriendChallengeEvent.OPPONENT_DECLINED_CHALLENGE, this.m_data.m_challengee, this.CleanUpChallengeData());
  }

  private void AchieveManager_OnAchievesUpdated(
    List<Achievement> updatedAchieves,
    List<Achievement> completedAchievements,
    object userData)
  {
    if (!completedAchievements.Any<Achievement>((Func<Achievement, bool>) (a => a.IsFriendlyChallengeQuest)))
      return;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.GAMEPLAY)
      this.m_data.m_updatePartyQuestInfoOnGameplaySceneUnload = true;
    else
      this.UpdatePartyQuestInfo();
  }

  private void UpdatePartyQuestInfo()
  {
    if (!this.DidSendChallenge() || !BnetParty.IsInParty(this.m_data.m_partyId))
      return;
    byte[] val = (byte[]) null;
    IEnumerable<Achievement> source = AchieveManager.Get().GetActiveQuests().Where<Achievement>((Func<Achievement, bool>) (q => q.IsFriendlyChallengeQuest));
    if (source.Any<Achievement>())
    {
      PartyQuestInfo protobuf = new PartyQuestInfo();
      protobuf.QuestIds.AddRange(source.Select<Achievement, int>((Func<Achievement, int>) (q => q.ID)));
      val = ProtobufUtil.ToByteArray((IProtoBuf) protobuf);
    }
    BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("quests", val));
  }

  public void OnStoreOpened() => this.UpdateMyAvailability();

  public void OnStoreClosed() => this.UpdateMyAvailability();

  public bool DidReceiveChallenge() => this.m_data.DidReceiveChallenge;

  public bool DidSendChallenge() => this.m_data.DidSendChallenge;

  public bool HasChallenge() => this.DidSendChallenge() || this.DidReceiveChallenge();

  public bool DidChallengeeAccept() => this.m_data.m_challengeeAccepted;

  public bool AmIInGameState() => this.DidSendChallenge() ? this.m_data.m_challengerInGameState : this.m_data.m_challengeeInGameState;

  public BnetPlayer GetOpponent(BnetPlayer player)
  {
    if (player == this.m_data.m_challenger)
      return this.m_data.m_challengee;
    return player == this.m_data.m_challengee ? this.m_data.m_challenger : (BnetPlayer) null;
  }

  public BnetPlayer GetMyOpponent() => this.GetOpponent(BnetPresenceMgr.Get().GetMyPlayer());

  public bool CanChallenge(BnetPlayer player)
  {
    if (player == null)
      return false;
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    return player != myPlayer && this.AmIAvailable() && this.IsOpponentAvailable(player) && !PartyManager.Get().IsPlayerInAnyParty(player.GetBestGameAccountId()) && (BnetFriendMgr.Get().IsFriend(player) || BnetNearbyPlayerMgr.Get().IsNearbyStranger(player));
  }

  public bool CanShowFriendlyChallenge(BnetPlayer player)
  {
    if (player == null)
      return false;
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    return player != myPlayer && !PopupDisplayManager.Get().IsShowing && !SpectatorManager.Get().IsSpectatingOrWatching && !PartyManager.Get().IsInParty() && myPlayer.GetHearthstoneGameAccount().CanBeInvitedToGame() && this.IsOpponentAvailable(player) && !PartyManager.Get().IsPlayerInAnyParty(player.GetBestGameAccountId()) && (BnetFriendMgr.Get().IsFriend(player) || BnetNearbyPlayerMgr.Get().IsNearbyStranger(player));
  }

  public bool IsHearthstoneFriendlyChallengeAvailable(BnetPlayer player)
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    return player != myPlayer && !myPlayer.IsAppearingOffline() && this.IsOpponentAvailable(player) && NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.Friendly && !PartyManager.Get().IsPlayerInAnyParty(player.GetBestGameAccountId()) && GameUtils.IsTraditionalTutorialComplete() && player.GetHearthstoneGameAccount().GetTutorialBeaten() >= 1;
  }

  public bool IsBattlegroundsFriendlyChallengeAvailable(BnetPlayer player)
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    return player != myPlayer && !myPlayer.IsAppearingOffline() && this.IsOpponentAvailable(player) && NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.BattlegroundsFriendlyChallenge && !PartyManager.Get().IsPlayerInAnyParty(player.GetBestGameAccountId()) && GameUtils.IsBattleGroundsTutorialComplete() && player.GetHearthstoneGameAccount().GetBattlegroundsTutorialComplete();
  }

  public bool IsMercenariesFriendlyChallengeAvailable(BnetPlayer player)
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    return player != myPlayer && !myPlayer.IsAppearingOffline() && this.IsOpponentAvailable(player) && NetCache.Get().GetNetObject<NetCache.NetCacheFeatures>().Games.MercenariesFriendly && !PartyManager.Get().IsPlayerInAnyParty(player.GetBestGameAccountId()) && GameUtils.IsMercenariesPrologueBountyComplete(NetCache.Get().GetNetObject<NetCache.NetCacheMercenariesPlayerInfo>()) && GameUtils.IsMercenariesVillageTutorialComplete() && player.GetHearthstoneGameAccount().GetMercenariesTutorialComplete();
  }

  public bool AmIAvailable()
  {
    if (!this.m_netCacheReady || !this.m_myPlayerReady || SpectatorManager.Get().IsSpectatingOrWatching)
      return false;
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    BnetGameAccount hearthstoneGameAccount = myPlayer.GetHearthstoneGameAccount();
    return !(hearthstoneGameAccount == (BnetGameAccount) null) && myPlayer.IsOnline() && !myPlayer.IsAppearingOffline() && Network.IsLoggedIn() && !PopupDisplayManager.Get().IsShowing && !PartyManager.Get().IsInParty() && hearthstoneGameAccount.CanBeInvitedToGame();
  }

  public bool IsOpponentAvailable(BnetPlayer player)
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    BnetGameAccount hearthstoneGameAccount1 = player.GetHearthstoneGameAccount();
    if (hearthstoneGameAccount1 == (BnetGameAccount) null || !hearthstoneGameAccount1.IsOnline() || !hearthstoneGameAccount1.CanBeInvitedToGame())
      return false;
    if (HearthstoneApplication.IsPublic())
    {
      BnetGameAccount hearthstoneGameAccount2 = myPlayer.GetHearthstoneGameAccount();
      if (string.Compare(hearthstoneGameAccount1.GetClientVersion(), hearthstoneGameAccount2.GetClientVersion()) != 0 || string.Compare(hearthstoneGameAccount1.GetClientEnv(), hearthstoneGameAccount2.GetClientEnv()) != 0)
        return false;
    }
    return true;
  }

  public bool DidISelectDeckOrHero()
  {
    if (this.DidSendChallenge())
      return this.m_data.m_challengerDeckOrHeroSelected;
    return !this.DidReceiveChallenge() || this.m_data.m_challengeeDeckOrHeroSelected;
  }

  public bool DidOpponentSelectDeckOrHero()
  {
    if (this.DidSendChallenge())
      return this.m_data.m_challengeeDeckOrHeroSelected;
    return !this.DidReceiveChallenge() || this.m_data.m_challengerDeckOrHeroSelected;
  }

  public static void ShowChallengerNeedsToCreateTavernBrawlDeckAlert()
  {
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
      m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_CHALLENGER_NO_TAVERN_BRAWL_DECK"),
      m_showAlertIcon = true,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    };
    DialogManager.Get().ShowPopup(info);
  }

  public void SendChallenge(BnetPlayer player, PegasusShared.FormatType formatType, bool enableDeckShare)
  {
    if (!this.CanChallenge(player))
      return;
    this.SendChallenge_Internal(player, formatType, BrawlType.BRAWL_TYPE_UNKNOWN, enableDeckShare, 0, 0, false);
  }

  public void SendTavernBrawlChallenge(
    BnetPlayer player,
    BrawlType brawlType,
    int seasonId,
    int brawlLibraryItemId)
  {
    if (!this.CanChallenge(player))
      return;
    TavernBrawlManager.Get().EnsureAllDataReady(brawlType, (TavernBrawlManager.CallbackEnsureServerDataReady) (() => this.TavernBrawl_SendChallenge_OnEnsureServerDataReady(player, brawlType, seasonId, brawlLibraryItemId)));
  }

  private void TavernBrawl_SendChallenge_OnEnsureServerDataReady(
    BnetPlayer player,
    BrawlType brawlType,
    int seasonId,
    int brawlLibraryItemId)
  {
    TavernBrawlManager tavernBrawlManager = TavernBrawlManager.Get();
    if (!this.CanChallenge(player) || !tavernBrawlManager.IsTavernBrawlActive(brawlType) || this.HasChallenge())
      return;
    if (!tavernBrawlManager.CanChallengeToTavernBrawl(brawlType))
    {
      AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
      {
        m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
        m_text = GameStrings.Format("GLOBAL_FRIENDLIST_CHALLENGE_TOOLTIP_TAVERN_BRAWL_NOT_CHALLENGEABLE"),
        m_showAlertIcon = true,
        m_responseDisplay = AlertPopup.ResponseDisplay.OK
      };
      DialogManager.Get().ShowPopup(info);
    }
    else if (tavernBrawlManager.GetMission(brawlType).canCreateDeck && !tavernBrawlManager.HasValidDeck(brawlType))
      FriendChallengeMgr.ShowChallengerNeedsToCreateTavernBrawlDeckAlert();
    else
      this.SendChallenge_Internal(player, PegasusShared.FormatType.FT_UNKNOWN, brawlType, false, seasonId, brawlLibraryItemId, false);
  }

  private void SendChallenge_Internal(
    BnetPlayer player,
    PegasusShared.FormatType formatType,
    BrawlType brawlType,
    bool enableDeckShare,
    int seasonId,
    int brawlLibraryItemId,
    bool isBaconGame)
  {
    if (this.m_data.m_partyId != (BnetPartyId) null)
      BnetParty.DissolveParty(this.m_data.m_partyId);
    this.CleanUpChallengeData();
    if (this.m_hasPreSelectedDeckOrHero)
    {
      this.m_data.m_challengerDeckId = this.m_preSelectedDeckId;
      this.m_data.m_challengerHeroId = this.m_preSelectedHeroId;
      this.m_data.m_challengerDeckOrHeroSelected = this.m_hasPreSelectedDeckOrHero;
    }
    this.m_data.m_challenger = BnetPresenceMgr.Get().GetMyPlayer();
    this.m_data.m_challengerId = this.m_data.m_challenger.GetHearthstoneGameAccount().GetId();
    this.m_data.m_challengee = player;
    this.m_hasSeenDeclinedReason = false;
    this.m_data.m_scenarioId = 2;
    this.m_data.m_seasonId = seasonId;
    this.m_data.m_brawlLibraryItemId = brawlLibraryItemId;
    this.m_data.m_challengeBrawlType = brawlType;
    this.m_data.m_challengeFormatType = formatType;
    if (isBaconGame)
      this.m_data.m_scenarioId = 3459;
    else if (this.IsChallengeTavernBrawl())
    {
      TavernBrawlManager.Get().CurrentBrawlType = this.m_data.m_challengeBrawlType;
      TavernBrawlMission mission = TavernBrawlManager.Get().GetMission(brawlType);
      mission.SetSelectedBrawlLibraryItemId(brawlLibraryItemId);
      this.m_data.m_scenarioId = mission.missionId;
      this.m_data.m_challengeFormatType = mission.formatType;
      PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING);
    }
    List<Blizzard.GameService.Protocol.V2.Client.Attribute> attributeCollection = BnetAttribute.CreateAttributeCollection(BnetAttribute.CreateAttribute("WTCG.Game.ScenarioId", this.m_data.m_scenarioId), BnetAttribute.CreateAttribute("WTCG.Format.Type", (long) this.m_data.m_challengeFormatType), BnetAttribute.CreateAttribute("WTCG.Season.Id", (long) this.m_data.m_seasonId));
    if (this.IsChallengeTavernBrawl())
    {
      attributeCollection.Add(BnetAttribute.CreateAttribute("WTCG.Brawl.Type", (long) this.m_data.m_challengeBrawlType));
      attributeCollection.Add(BnetAttribute.CreateAttribute("WTCG.Brawl.LibraryItemId", (long) this.m_data.m_brawlLibraryItemId));
    }
    IEnumerable<Achievement> source = AchieveManager.Get().GetActiveQuests().Where<Achievement>((Func<Achievement, bool>) (q => q.IsFriendlyChallengeQuest));
    if (source.Any<Achievement>())
    {
      PartyQuestInfo protobuf = new PartyQuestInfo();
      protobuf.QuestIds.AddRange(source.Select<Achievement, int>((Func<Achievement, int>) (q => q.ID)));
      byte[] byteArray = ProtobufUtil.ToByteArray((IProtoBuf) protobuf);
      attributeCollection.Add(BnetAttribute.CreateAttribute("quests", byteArray));
    }
    if (FiresideGatheringManager.Get().IsCheckedIn && FiresideGatheringManager.Get().CurrentFsgSharedSecretKey != null)
    {
      byte[] hash = SHA256.Create().ComputeHash(FiresideGatheringManager.Get().CurrentFsgSharedSecretKey, 0, FiresideGatheringManager.Get().CurrentFsgSharedSecretKey.Length);
      attributeCollection.Add(BnetAttribute.CreateAttribute("fsg1", hash));
    }
    if (this.m_data.m_challengerDeckId != 0L)
    {
      attributeCollection.Add(BnetAttribute.CreateAttribute("d1", this.m_data.m_challengerDeckId));
      attributeCollection.Add(BnetAttribute.CreateAttribute("hero1", this.m_data.m_challengerHeroId));
    }
    if (this.m_data.m_challengerDeckOrHeroSelected)
      attributeCollection.Add(BnetAttribute.CreateAttribute("s1", "ready"));
    string val = enableDeckShare ? "deckShareEnabled" : "deckShareDisabled";
    attributeCollection.Add(BnetAttribute.CreateAttribute("isDeckShareEnabled", val));
    attributeCollection.Add(BnetAttribute.CreateAttribute("p1DeckShareState", "none"));
    attributeCollection.Add(BnetAttribute.CreateAttribute("p2DeckShareState", "none"));
    BnetParty.CreateParty(PartyType.FRIENDLY_CHALLENGE, ChannelApi.PartyPrivacyLevel.OpenInvitation, (BnetParty.CreateSuccessCallback) null, attributeCollection);
    this.UpdateMyAvailability();
    this.FireChangedEvent(FriendChallengeEvent.I_SENT_CHALLENGE, player);
  }

  public void CancelChallenge()
  {
    if (!this.HasChallenge())
      return;
    if (this.DidSendChallenge())
    {
      this.RescindChallenge();
    }
    else
    {
      if (!this.DidReceiveChallenge())
        return;
      this.DeclineChallenge();
    }
  }

  public void AcceptChallenge()
  {
    if (!this.DidReceiveChallenge())
      return;
    this.m_data.m_challengeeAccepted = true;
    BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute(this.DidSendChallenge() ? "s1" : "s2", "deck"));
    this.FireChangedEvent(FriendChallengeEvent.I_ACCEPTED_CHALLENGE, this.m_data.m_challenger);
  }

  public void DeclineChallenge()
  {
    if (!this.DidReceiveChallenge())
      return;
    this.RevertTavernBrawlPresenceStatus();
    this.DeclineFriendChallenge_Internal(this.m_data.m_partyId);
    this.FireChangedEvent(FriendChallengeEvent.I_DECLINED_CHALLENGE, this.m_data.m_challenger, this.CleanUpChallengeData());
  }

  private void DeclineFriendChallenge_Internal(BnetPartyId partyId)
  {
    if (!BnetParty.IsInParty(partyId))
      return;
    BnetParty.DissolveParty(partyId);
  }

  public void QueueCanceled()
  {
    BnetPlayer player;
    if (this.DidReceiveChallenge())
    {
      player = this.m_data.m_challenger;
    }
    else
    {
      if (!this.DidSendChallenge())
        return;
      player = this.m_data.m_challengee;
    }
    FriendlyChallengeData challengeData = this.CleanUpChallengeData();
    this.FireChangedEvent(FriendChallengeEvent.QUEUE_CANCELED, player, challengeData);
  }

  private void PushPartyEvent(
    BnetPartyId partyId,
    string type,
    string data,
    BnetGameAccountId otherPlayerGameAccountId = null)
  {
    if ((BnetEntityId) otherPlayerGameAccountId == (BnetEntityId) null)
    {
      BnetPlayer bnetPlayer = this.DidSendChallenge() ? this.m_data.m_challenger : this.m_data.m_challengee;
      otherPlayerGameAccountId = bnetPlayer == null ? (BnetGameAccountId) null : bnetPlayer.GetHearthstoneGameAccountId();
    }
    this.OnPartyUpdate(new PartyEvent[1]
    {
      new PartyEvent()
      {
        partyId = partyId,
        eventName = type,
        eventData = data,
        otherMemberId = otherPlayerGameAccountId
      }
    });
  }

  public void RescindChallenge()
  {
    if (!this.DidSendChallenge())
      return;
    this.RevertTavernBrawlPresenceStatus();
    if (BnetParty.IsInParty(this.m_data.m_partyId))
      BnetParty.DissolveParty(this.m_data.m_partyId);
    this.FireChangedEvent(FriendChallengeEvent.I_RESCINDED_CHALLENGE, this.m_data.m_challengee, this.CleanUpChallengeData());
  }

  public bool IsChallengeFriendlyDuel => this.IsChallengeStandardDuel() || this.IsChallengeWildDuel() || this.IsChallengeClassicDuel();

  public bool IsChallengeStandardDuel() => this.HasChallenge() && !this.IsChallengeTavernBrawl() && this.m_data.m_challengeFormatType == PegasusShared.FormatType.FT_STANDARD;

  public bool IsChallengeWildDuel() => this.HasChallenge() && !this.IsChallengeTavernBrawl() && this.m_data.m_challengeFormatType == PegasusShared.FormatType.FT_WILD;

  public bool IsChallengeClassicDuel() => this.HasChallenge() && !this.IsChallengeTavernBrawl() && this.m_data.m_challengeFormatType == PegasusShared.FormatType.FT_CLASSIC;

  public bool IsChallengeTavernBrawl() => this.HasChallenge() && this.m_data.m_challengeBrawlType != 0;

  public bool IsChallengeFiresideBrawl() => this.IsChallengeTavernBrawl() && this.m_data.m_challengeBrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING;

  public bool IsChallengeBacon() => this.HasChallenge() && this.m_data.m_scenarioId == 3459;

  public bool IsChallengeMercenaries() => this.HasChallenge() && this.m_data.m_scenarioId == 3743;

  public BrawlType GetChallengeBrawlType() => !this.HasChallenge() ? BrawlType.BRAWL_TYPE_UNKNOWN : this.m_data.m_challengeBrawlType;

  public bool IsDeckShareEnabled()
  {
    string str;
    return this.HasChallenge() && BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "isDeckShareEnabled", out str) && str == "deckShareEnabled";
  }

  public void RequestDeckShare()
  {
    if (this.DidSendChallenge())
    {
      string str;
      if (!BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p1DeckShareState", out str))
        return;
      if (str == "sharingUnused")
      {
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1DeckShareState", "sharing"));
      }
      else
      {
        if (!(str == "none"))
          return;
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1DeckShareState", "requested"));
      }
    }
    else
    {
      string str;
      if (!this.DidReceiveChallenge() || !BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p2DeckShareState", out str))
        return;
      if (str == "sharingUnused")
      {
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2DeckShareState", "sharing"));
      }
      else
      {
        if (!(str == "none"))
          return;
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2DeckShareState", "requested"));
      }
    }
  }

  public void EndDeckShare()
  {
    if (this.DidSendChallenge())
    {
      string str;
      if (!BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p1DeckShareState", out str) || !(str == "sharing"))
        return;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1DeckShareState", "sharingUnused"));
    }
    else
    {
      string str;
      if (!this.DidReceiveChallenge() || !BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p2DeckShareState", out str) || !(str == "sharing"))
        return;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2DeckShareState", "sharingUnused"));
    }
  }

  private void ShareDecks_InternalParty()
  {
    byte[] val = this.SerializeSharedDecks(CollectionManager.Get().GetDecks(DeckType.NORMAL_DECK));
    if (val == null)
    {
      Log.Party.PrintError("{0}.ShareDecks_InternalParty(): Unable to Serialize decks!.", (object) this);
      if (this.DidSendChallenge())
      {
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2DeckShareState", "error"));
      }
      else
      {
        if (!this.DidReceiveChallenge())
          return;
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1DeckShareState", "error"));
      }
    }
    else if (this.DidSendChallenge())
    {
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1DeckShareDecks", val));
    }
    else
    {
      if (!this.DidReceiveChallenge())
        return;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2DeckShareDecks", val));
    }
  }

  public List<CollectionDeck> GetSharedDecks()
  {
    if (this.m_data.m_sharedDecks != null)
      return new List<CollectionDeck>((IEnumerable<CollectionDeck>) this.m_data.m_sharedDecks);
    byte[] blob = (byte[]) null;
    if (this.DidSendChallenge() && (this.m_data.m_challengerDeckShareState == "sharing" || this.m_data.m_challengerDeckShareState == "sharingUnused"))
      BattleNet.GetPartyAttribute<byte[]>(this.m_data.m_partyId, "p2DeckShareDecks", out blob);
    else if (this.DidReceiveChallenge() && (this.m_data.m_challengeeDeckShareState == "sharing" || this.m_data.m_challengeeDeckShareState == "sharingUnused"))
      BattleNet.GetPartyAttribute<byte[]>(this.m_data.m_partyId, "p1DeckShareDecks", out blob);
    return blob == null ? (List<CollectionDeck>) null : this.DeserializeSharedDecks(blob);
  }

  private byte[] SerializeSharedDecks(List<CollectionDeck> collectionDecks)
  {
    if (collectionDecks == null || collectionDecks.Count <= 0)
      return (byte[]) null;
    DeckList protobuf = new DeckList();
    PegasusShared.FormatType formatType = Options.GetFormatType();
    foreach (CollectionDeck collectionDeck in collectionDecks)
    {
      if (collectionDeck.IsValidForRuleset && collectionDeck.IsValidForFormat(formatType))
      {
        ulong num = 0;
        if (collectionDeck.NeedsName)
          num |= 512UL;
        if (formatType == PegasusShared.FormatType.FT_STANDARD)
          num |= 128UL;
        if (collectionDeck.Locked)
          num |= 1024UL;
        DeckInfo deckInfo = new DeckInfo()
        {
          Id = collectionDeck.ID,
          Name = collectionDeck.Name,
          Hero = GameUtils.TranslateCardIdToDbId(collectionDeck.HeroCardID),
          DeckType = collectionDeck.Type,
          CardBack = collectionDeck.CardBackID.GetValueOrDefault(),
          HeroOverride = collectionDeck.HeroOverridden,
          SeasonId = collectionDeck.SeasonId,
          BrawlLibraryItemId = collectionDeck.BrawlLibraryItemId,
          SortOrder = collectionDeck.SortOrder,
          FormatType = collectionDeck.FormatType,
          SourceType = collectionDeck.SourceType,
          Validity = num,
          Rune1 = collectionDeck.GetRuneAtIndex(0),
          Rune2 = collectionDeck.GetRuneAtIndex(1),
          Rune3 = collectionDeck.GetRuneAtIndex(2)
        };
        if (collectionDeck.HasUIHeroOverride())
        {
          deckInfo.UiHeroOverride = GameUtils.TranslateCardIdToDbId(collectionDeck.UIHeroOverrideCardID);
          deckInfo.UiHeroOverridePremium = (int) collectionDeck.UIHeroOverridePremium;
        }
        protobuf.Decks.Add(deckInfo);
      }
    }
    return ProtobufUtil.ToByteArray((IProtoBuf) protobuf);
  }

  private List<CollectionDeck> DeserializeSharedDecks(byte[] blob)
  {
    if (blob == null)
      return (List<CollectionDeck>) null;
    try
    {
      DeckList from = ProtobufUtil.ParseFrom<DeckList>(blob);
      this.m_data.m_sharedDecks = new List<CollectionDeck>();
      foreach (DeckInfo deck in from.Decks)
      {
        CollectionDeck collectionDeck = new CollectionDeck()
        {
          ID = deck.Id,
          Name = deck.Name,
          HeroCardID = GameUtils.TranslateDbIdToCardId(deck.Hero),
          Type = deck.DeckType,
          CardBackID = new int?(deck.CardBack),
          HeroOverridden = deck.HeroOverride,
          SeasonId = deck.SeasonId,
          BrawlLibraryItemId = deck.BrawlLibraryItemId,
          NeedsName = Network.DeckNeedsName(deck.Validity),
          SortOrder = deck.HasSortOrder ? deck.SortOrder : deck.Id,
          FormatType = deck.FormatType,
          SourceType = deck.HasSourceType ? deck.SourceType : DeckSourceType.DECK_SOURCE_TYPE_UNKNOWN,
          Locked = Network.AreDeckFlagsLocked(deck.Validity),
          IsShared = true
        };
        collectionDeck.SetRuneOrder(deck.Rune1, deck.Rune2, deck.Rune3);
        if (deck.HasUiHeroOverride)
        {
          collectionDeck.UIHeroOverrideCardID = GameUtils.TranslateDbIdToCardId(deck.UiHeroOverride);
          collectionDeck.UIHeroOverridePremium = (TAG_PREMIUM) deck.UiHeroOverridePremium;
        }
        this.m_data.m_sharedDecks.Add(collectionDeck);
      }
    }
    catch
    {
      Log.Party.PrintError("{0}.ShareDecks_InternalParty(): Unable to Deserialize decks!.", (object) this);
      this.m_data.m_sharedDecks = (List<CollectionDeck>) null;
    }
    return this.m_data.m_sharedDecks;
  }

  public bool HasOpponentSharedDecks() => this.GetSharedDecks() != null;

  public bool ShouldUseSharedDecks() => this.HasOpponentSharedDecks() && (!this.DidSendChallenge() || !(this.m_data.m_challengerDeckShareState != "sharing")) && (!this.DidReceiveChallenge() || !(this.m_data.m_challengeeDeckShareState != "sharing"));

  private void OnFriendChallengeDeckShareRequestDialogWaitingResponse(
    AlertPopup.Response response,
    object userData)
  {
    if (response != AlertPopup.Response.CANCEL)
      return;
    if (this.DidSendChallenge())
    {
      string str;
      if (!BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p1DeckShareState", out str) || !(str == "requested"))
        return;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1DeckShareState", "none"));
    }
    else
    {
      string str;
      if (!this.DidReceiveChallenge() || !BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p2DeckShareState", out str) || !(str == "requested"))
        return;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2DeckShareState", "none"));
    }
  }

  private void OnFriendChallengeDeckShareRequestDialogResponse(
    AlertPopup.Response response,
    object userData)
  {
    string val = response == AlertPopup.Response.CANCEL ? "declined" : "sharing";
    if (this.DidSendChallenge())
    {
      string str1;
      if (BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p2DeckShareState", out str1) && str1 == "requested")
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2DeckShareState", val));
      string str2;
      if (!BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p1DeckShareState", out str2) || !(str2 == "requested"))
        return;
      FriendlyChallengeHelper.Get().ShowDeckShareRequestWaitingDialog(new AlertPopup.ResponseCallback(this.OnFriendChallengeDeckShareRequestDialogWaitingResponse));
    }
    else
    {
      if (!this.DidReceiveChallenge())
        return;
      string str3;
      if (BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p1DeckShareState", out str3) && str3 == "requested")
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1DeckShareState", val));
      string str4;
      if (!BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p2DeckShareState", out str4) || !(str4 == "requested"))
        return;
      FriendlyChallengeHelper.Get().ShowDeckShareRequestWaitingDialog(new AlertPopup.ResponseCallback(this.OnFriendChallengeDeckShareRequestDialogWaitingResponse));
    }
  }

  private DeckShareState GetDeckShareStateEnumFromAttribute(
    string deckShareStateAttribute)
  {
    DeckShareState enumFromAttribute = DeckShareState.NO_DECK_SHARE;
    if (deckShareStateAttribute == "sharingUnused")
      enumFromAttribute = DeckShareState.DECK_SHARED_UNUSED;
    else if (deckShareStateAttribute == "sharing")
      enumFromAttribute = DeckShareState.USING_SHARED_DECK;
    return enumFromAttribute;
  }

  public void SkipDeckSelection() => this.SelectDeck(1L);

  public void SelectDeck(long deckId)
  {
    if (this.DidSendChallenge())
    {
      this.m_data.m_challengerDeckOrHeroSelected = true;
    }
    else
    {
      if (!this.DidReceiveChallenge())
        return;
      this.m_data.m_challengeeDeckOrHeroSelected = true;
    }
    this.SelectMyDeck_InternalParty(deckId);
    this.FireChangedEvent(FriendChallengeEvent.SELECTED_DECK_OR_HERO, BnetPresenceMgr.Get().GetMyPlayer());
  }

  public void SelectDeckBeforeSendingChallenge(long deckId)
  {
    this.m_hasPreSelectedDeckOrHero = true;
    this.m_preSelectedDeckId = deckId;
  }

  public void ClearSelectedDeckAndHeroBeforeSendingChallenge()
  {
    this.m_hasPreSelectedDeckOrHero = false;
    this.m_preSelectedDeckId = 0L;
    this.m_preSelectedHeroId = 0L;
  }

  public void SelectHero(long heroCardDbId)
  {
    if (this.DidSendChallenge())
    {
      this.m_data.m_challengerDeckOrHeroSelected = true;
    }
    else
    {
      if (!this.DidReceiveChallenge())
        return;
      this.m_data.m_challengeeDeckOrHeroSelected = true;
    }
    this.SelectMyHero_InternalParty(heroCardDbId);
    this.FireChangedEvent(FriendChallengeEvent.SELECTED_DECK_OR_HERO, BnetPresenceMgr.Get().GetMyPlayer());
  }

  public void SelectHeroBeforeSendingChallenge(long heroCardDbId)
  {
    this.m_hasPreSelectedDeckOrHero = true;
    this.m_preSelectedHeroId = heroCardDbId;
  }

  public void DeselectDeckOrHero()
  {
    if (this.m_hasPreSelectedDeckOrHero)
    {
      this.m_hasPreSelectedDeckOrHero = false;
      this.m_preSelectedDeckId = 0L;
      this.m_preSelectedHeroId = 0L;
    }
    if (this.DidSendChallenge() && this.m_data.m_challengerDeckOrHeroSelected)
    {
      this.m_data.m_challengerDeckOrHeroSelected = false;
      this.m_data.m_challengerDeckId = 0L;
      this.m_data.m_challengerHeroId = 0L;
      this.m_data.m_challengerInGameState = false;
    }
    else
    {
      if (!this.DidReceiveChallenge() || !this.m_data.m_challengeeDeckOrHeroSelected)
        return;
      this.m_data.m_challengeeDeckOrHeroSelected = false;
      this.m_data.m_challengeeDeckId = 0L;
      this.m_data.m_challengeeHeroId = 0L;
      this.m_data.m_challengeeInGameState = false;
    }
    this.SelectMyDeck_InternalParty(0L);
    this.SelectMyHero_InternalParty(0L);
    this.FireChangedEvent(FriendChallengeEvent.DESELECTED_DECK_OR_HERO, BnetPresenceMgr.Get().GetMyPlayer());
  }

  public void SetChallengeMethod(FriendChallengeMgr.ChallengeMethod challengeMethod) => this.m_challengeMethod = challengeMethod;

  private bool ShouldTransitionToFriendlySceneAccordingToChallengeMethod() => this.m_challengeMethod != FriendChallengeMgr.ChallengeMethod.FROM_FIRESIDE_GATHERING_OPPONENT_PICKER;

  private void SelectMyDeck_InternalParty(long deckId)
  {
    string val = deckId == 0L ? "deck" : "ready";
    int cardBackToUse;
    int? deckCardBack;
    CardBackManager.Get().FindCardBackToUse(deckId, out cardBackToUse, out deckCardBack);
    long? nullable1 = new long?();
    if (deckId != 0L)
    {
      CollectionDeck deck = CollectionManager.Get().GetDeck(deckId);
      if (deck != null && !deck.HeroOverridden)
        nullable1 = new long?((long) CollectionManager.Get().GetRandomHeroIdOwnedByPlayer(deck.GetClass(), deck.RandomHeroUseFavorite));
    }
    if (this.DidSendChallenge())
    {
      this.m_data.m_challengerDeckId = deckId;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("s1", val), BnetAttribute.CreateAttribute("d1", deckId));
      int num = cardBackToUse;
      int? nullable2 = deckCardBack;
      int valueOrDefault = nullable2.GetValueOrDefault();
      if (!(num == valueOrDefault & nullable2.HasValue))
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1CardBack", cardBackToUse));
      if (!nullable1.HasValue)
        return;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("randomHero1", nullable1.Value));
    }
    else
    {
      this.m_data.m_challengeeDeckId = deckId;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("s2", val), BnetAttribute.CreateAttribute("d2", deckId));
      int num = cardBackToUse;
      int? nullable3 = deckCardBack;
      int valueOrDefault = nullable3.GetValueOrDefault();
      if (!(num == valueOrDefault & nullable3.HasValue))
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2CardBack", cardBackToUse));
      if (!nullable1.HasValue)
        return;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("randomHero2", nullable1.Value));
    }
  }

  private void SelectMyHero_InternalParty(long heroCardDbId)
  {
    string val = heroCardDbId == 0L ? "deck" : "ready";
    if (this.DidSendChallenge())
    {
      this.m_data.m_challengerHeroId = heroCardDbId;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("s1", val), BnetAttribute.CreateAttribute("hero1", heroCardDbId));
    }
    else
    {
      this.m_data.m_challengeeHeroId = heroCardDbId;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("s2", val), BnetAttribute.CreateAttribute("hero2", heroCardDbId));
    }
  }

  private void SetMyFsgSharedSecretKey_InternalParty(BnetPartyId partyId, byte[] fsgSharedSecretKey)
  {
    if (this.DidSendChallenge())
    {
      this.m_data.m_challengerFsgSharedSecret = fsgSharedSecretKey;
      BattleNet.SetPartyAttributes(partyId, BnetAttribute.CreateAttribute("fsg1", fsgSharedSecretKey));
    }
    else
    {
      this.m_data.m_challengeeFsgSharedSecret = fsgSharedSecretKey;
      BattleNet.SetPartyAttributes(partyId, BnetAttribute.CreateAttribute("fsg2", fsgSharedSecretKey));
    }
  }

  public int GetScenarioId() => this.m_data.m_scenarioId;

  public PegasusShared.FormatType GetFormatType() => this.m_data.m_challengeFormatType;

  public PartyQuestInfo GetPartyQuestInfo() => this.GetPartyQuestInfo(this.m_data.m_partyId, "quests");

  public PartyQuestInfo GetPartyQuestInfo(BnetPartyId partyId, string attributeKey)
  {
    PartyQuestInfo partyQuestInfo = (PartyQuestInfo) null;
    byte[] bytes;
    if (BattleNet.GetPartyAttribute<byte[]>(partyId, attributeKey, out bytes))
      partyQuestInfo = ProtobufUtil.ParseFrom<PartyQuestInfo>(bytes);
    return partyQuestInfo;
  }

  public bool PlayersInSameFiresideGathering() => this.m_data.m_challengerFsgSharedSecret != null && this.m_data.m_challengeeFsgSharedSecret != null && GeneralUtils.AreArraysEqual<byte>(this.m_data.m_challengerFsgSharedSecret, this.m_data.m_challengeeFsgSharedSecret);

  public void UpdateMyFsgSharedSecret(byte[] currentFsgSharedSecretKey) => this.UpdateMyFsgSharedSecret(this.m_data.m_partyId, currentFsgSharedSecretKey);

  public void UpdateMyFsgSharedSecret(BnetPartyId partyId, byte[] currentFsgSharedSecretKey)
  {
    if (partyId == (BnetPartyId) null)
      return;
    if (!FiresideGatheringManager.Get().IsCheckedIn || currentFsgSharedSecretKey == null)
    {
      this.SetMyFsgSharedSecretKey_InternalParty(partyId, (byte[]) null);
    }
    else
    {
      byte[] hash = SHA256.Create().ComputeHash(currentFsgSharedSecretKey, 0, currentFsgSharedSecretKey.Length);
      this.SetMyFsgSharedSecretKey_InternalParty(partyId, hash);
    }
  }

  public bool AddChangedListener(FriendChallengeMgr.ChangedCallback callback) => this.AddChangedListener(callback, (object) null);

  public bool AddChangedListener(FriendChallengeMgr.ChangedCallback callback, object userData)
  {
    FriendChallengeMgr.ChangedListener changedListener = new FriendChallengeMgr.ChangedListener();
    changedListener.SetCallback(callback);
    changedListener.SetUserData(userData);
    if (this.m_changedListeners.Contains(changedListener))
      return false;
    this.m_changedListeners.Add(changedListener);
    return true;
  }

  public bool RemoveChangedListener(FriendChallengeMgr.ChangedCallback callback) => this.RemoveChangedListener(callback, (object) null);

  private bool RemoveChangedListener(FriendChallengeMgr.ChangedCallback callback, object userData)
  {
    FriendChallengeMgr.ChangedListener changedListener = new FriendChallengeMgr.ChangedListener();
    changedListener.SetCallback(callback);
    changedListener.SetUserData(userData);
    return this.m_changedListeners.Remove(changedListener);
  }

  public static bool RemoveChangedListenerFromInstance(
    FriendChallengeMgr.ChangedCallback callback,
    object userData = null)
  {
    return FriendChallengeMgr.s_instance != null && FriendChallengeMgr.s_instance.RemoveChangedListener(callback, userData);
  }

  private void OnPartyUpdate(PartyEvent[] updates)
  {
    for (int index = 0; index < updates.Length; ++index)
    {
      PartyEvent update = updates[index];
      BnetPartyId partyId = update.partyId;
      BnetGameAccountId otherMemberId = update.otherMemberId;
      if (update.eventName == "s1")
      {
        if (update.eventData == "wait")
          this.OnPartyUpdate_CreatedParty(partyId, otherMemberId);
        else if (update.eventData == "deck")
        {
          if (this.DidReceiveChallenge() && this.m_data.m_challengerDeckOrHeroSelected)
          {
            this.m_data.m_challengerDeckOrHeroSelected = false;
            this.m_data.m_challengerInGameState = false;
            this.FireChangedEvent(FriendChallengeEvent.DESELECTED_DECK_OR_HERO, this.m_data.m_challenger);
          }
        }
        else if (update.eventData == "ready")
        {
          if (this.DidReceiveChallenge())
          {
            this.m_data.m_challengerDeckOrHeroSelected = true;
            this.FireChangedEvent(FriendChallengeEvent.SELECTED_DECK_OR_HERO, this.m_data.m_challenger);
            this.SetIAmInGameState();
          }
        }
        else if (update.eventData == "game")
        {
          if (this.DidReceiveChallenge())
          {
            this.m_data.m_challengerInGameState = true;
            this.SetIAmInGameState();
            this.StartFriendlyChallengeGameIfReady();
            FriendlyChallengeHelper.Get().WaitForFriendChallengeToStart();
            this.m_data.m_findGameErrorOccurred = false;
          }
        }
        else if (update.eventData == "goto")
        {
          this.m_data.m_challengerDeckOrHeroSelected = false;
          this.m_data.m_challengerInGameState = false;
        }
      }
      else if (update.eventName == "s2")
      {
        if (update.eventData == "wait")
          this.OnPartyUpdate_JoinedParty(partyId, otherMemberId);
        else if (update.eventData == "deck")
        {
          if (this.DidSendChallenge())
          {
            if (this.m_data.m_challengeeAccepted)
            {
              this.m_data.m_challengeeDeckOrHeroSelected = false;
              this.m_data.m_challengeeInGameState = false;
              this.FireChangedEvent(FriendChallengeEvent.DESELECTED_DECK_OR_HERO, this.m_data.m_challengee);
            }
            else
            {
              this.m_data.m_challengeeAccepted = true;
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_ACCEPTED_CHALLENGE, this.m_data.m_challengee);
            }
          }
        }
        else if (update.eventData == "ready")
        {
          if (this.DidSendChallenge())
          {
            this.m_data.m_challengeeDeckOrHeroSelected = true;
            this.FireChangedEvent(FriendChallengeEvent.SELECTED_DECK_OR_HERO, this.m_data.m_challengee);
            this.SetIAmInGameState();
          }
        }
        else if (update.eventData == "game")
        {
          if (this.DidSendChallenge())
          {
            this.m_data.m_challengeeInGameState = true;
            this.SetIAmInGameState();
            if (this.StartFriendlyChallengeGameIfReady())
              FriendlyChallengeHelper.Get().WaitForFriendChallengeToStart();
          }
        }
        else if (update.eventData == "goto")
        {
          this.m_data.m_challengeeDeckOrHeroSelected = false;
          this.m_data.m_challengeeInGameState = false;
        }
      }
      else if (update.eventName == "left")
      {
        if (this.DidSendChallenge())
        {
          BnetPlayer challengee = this.m_data.m_challengee;
          int num = this.m_data.m_challengeeAccepted ? 1 : 0;
          this.RevertTavernBrawlPresenceStatus();
          FriendlyChallengeData challengeData = this.CleanUpChallengeData();
          if (num != 0)
            this.FireChangedEvent(FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE, challengee, challengeData);
          else
            this.FireChangedEvent(FriendChallengeEvent.OPPONENT_DECLINED_CHALLENGE, challengee, challengeData);
        }
        else if (this.DidReceiveChallenge())
        {
          BnetPlayer challenger = this.m_data.m_challenger;
          bool challengeeAccepted = this.m_data.m_challengeeAccepted;
          this.RevertTavernBrawlPresenceStatus();
          FriendlyChallengeData challengeData = this.CleanUpChallengeData();
          if (challenger != null)
          {
            if (challengeeAccepted)
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE, challenger, challengeData);
            else
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_RESCINDED_CHALLENGE, challenger, challengeData);
          }
        }
        else
          this.CleanUpChallengeData();
      }
      else if (update.eventName == "p1DeckShareState")
      {
        if (this.m_data.m_challenger != null)
        {
          string challengerDeckShareState = this.m_data.m_challengerDeckShareState;
          this.m_data.m_challengerDeckShareState = update.eventData;
          if (challengerDeckShareState == "none" && this.m_data.m_challengerDeckShareState == "requested")
          {
            if (this.DidReceiveChallenge())
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_REQUESTED_DECK_SHARE, this.m_data.m_challenger);
            else if (this.DidSendChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_REQUESTED_DECK_SHARE, this.m_data.m_challenger);
          }
          else if (challengerDeckShareState == "requested" && this.m_data.m_challengerDeckShareState == "none")
          {
            if (this.DidSendChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_CANCELED_DECK_SHARE_REQUEST, this.m_data.m_challenger);
            if (this.DidReceiveChallenge())
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_CANCELED_DECK_SHARE_REQUEST, this.m_data.m_challenger);
          }
          else if (challengerDeckShareState == "requested" && this.m_data.m_challengerDeckShareState == "declined")
          {
            if (this.DidReceiveChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_DECLINED_DECK_SHARE_REQUEST, this.m_data.m_challenger);
            else if (this.DidSendChallenge())
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_DECLINED_DECK_SHARE_REQUEST, this.m_data.m_challenger);
          }
          else if (challengerDeckShareState == "requested" && this.m_data.m_challengerDeckShareState == "sharing")
          {
            if (this.DidReceiveChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_ACCEPTED_DECK_SHARE_REQUEST, this.m_data.m_challenger);
            else if (this.DidSendChallenge())
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_ACCEPTED_DECK_SHARE_REQUEST, this.m_data.m_challenger);
          }
          else if (challengerDeckShareState == "sharing" && this.m_data.m_challengerDeckShareState == "sharingUnused")
          {
            if (this.DidSendChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_ENDED_DECK_SHARE, this.m_data.m_challenger);
          }
          else if (challengerDeckShareState == "sharingUnused" && this.m_data.m_challengerDeckShareState == "sharing")
          {
            if (this.DidSendChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_RECEIVED_SHARED_DECKS, this.m_data.m_challenger);
          }
          else if (this.m_data.m_challengerDeckShareState == "error" && this.DidSendChallenge())
            this.FireChangedEvent(FriendChallengeEvent.DECK_SHARE_ERROR_OCCURED, this.m_data.m_challenger);
        }
      }
      else if (update.eventName == "p2DeckShareState")
      {
        if (this.m_data.m_challengee != null)
        {
          string challengeeDeckShareState = this.m_data.m_challengeeDeckShareState;
          this.m_data.m_challengeeDeckShareState = update.eventData;
          if (challengeeDeckShareState == "none" && this.m_data.m_challengeeDeckShareState == "requested")
          {
            if (this.DidReceiveChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_REQUESTED_DECK_SHARE, this.m_data.m_challengee);
            else if (this.DidSendChallenge())
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_REQUESTED_DECK_SHARE, this.m_data.m_challengee);
          }
          else if (challengeeDeckShareState == "requested" && this.m_data.m_challengeeDeckShareState == "none")
          {
            if (this.DidSendChallenge())
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_CANCELED_DECK_SHARE_REQUEST, this.m_data.m_challengee);
            else if (this.DidReceiveChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_CANCELED_DECK_SHARE_REQUEST, this.m_data.m_challengee);
          }
          else if (challengeeDeckShareState == "requested" && this.m_data.m_challengeeDeckShareState == "declined")
          {
            if (this.DidReceiveChallenge())
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_DECLINED_DECK_SHARE_REQUEST, this.m_data.m_challengee);
            else if (this.DidSendChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_DECLINED_DECK_SHARE_REQUEST, this.m_data.m_challengee);
          }
          else if (challengeeDeckShareState == "requested" && this.m_data.m_challengeeDeckShareState == "sharing")
          {
            if (this.DidReceiveChallenge())
              this.FireChangedEvent(FriendChallengeEvent.OPPONENT_ACCEPTED_DECK_SHARE_REQUEST, this.m_data.m_challengee);
            else if (this.DidSendChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_ACCEPTED_DECK_SHARE_REQUEST, this.m_data.m_challengee);
          }
          else if (challengeeDeckShareState == "sharing" && this.m_data.m_challengeeDeckShareState == "sharingUnused")
          {
            if (this.DidReceiveChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_ENDED_DECK_SHARE, this.m_data.m_challengee);
          }
          else if (challengeeDeckShareState == "sharingUnused" && this.m_data.m_challengeeDeckShareState == "sharing")
          {
            if (this.DidReceiveChallenge())
              this.FireChangedEvent(FriendChallengeEvent.I_RECEIVED_SHARED_DECKS, this.m_data.m_challengee);
          }
          else if (this.m_data.m_challengeeDeckShareState == "error" && this.DidReceiveChallenge())
            this.FireChangedEvent(FriendChallengeEvent.DECK_SHARE_ERROR_OCCURED, this.m_data.m_challengee);
        }
      }
      else if (update.eventName == "p1DeckShareDecks")
      {
        if (this.DidReceiveChallenge() && this.m_data.m_challengeeDeckShareState == "sharing")
        {
          if (this.HasOpponentSharedDecks())
            this.FireChangedEvent(FriendChallengeEvent.I_RECEIVED_SHARED_DECKS, this.m_data.m_challengee);
          else
            BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2DeckShareState", "error"));
        }
      }
      else if (update.eventName == "p2DeckShareDecks" && this.DidSendChallenge() && this.m_data.m_challengerDeckShareState == "sharing")
      {
        if (this.HasOpponentSharedDecks())
          this.FireChangedEvent(FriendChallengeEvent.I_RECEIVED_SHARED_DECKS, this.m_data.m_challenger);
        else
          BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1DeckShareState", "error"));
      }
    }
  }

  private void OnPartyUpdate_CreatedParty(BnetPartyId partyId, BnetGameAccountId otherMemberId) => this.UpdateChallengeSentDialog();

  private void OnPartyUpdate_JoinedParty(BnetPartyId partyId, BnetGameAccountId otherMemberId)
  {
    if (this.DidSendChallenge())
      return;
    if (!FriendChallengeMgr.CanReceiveChallengeFrom(otherMemberId, partyId))
      this.DeclineFriendChallenge_Internal(partyId);
    else if (!this.AmIAvailable())
      this.DeclineFriendChallenge_Internal(partyId);
    else
      this.HandleJoinedParty(partyId, otherMemberId);
  }

  private static bool CanReceiveChallengeFrom(
    BnetGameAccountId challengerPlayer,
    BnetPartyId challengerPartyId)
  {
    if (BnetFriendMgr.Get().IsFriend(challengerPlayer) || BnetNearbyPlayerMgr.Get().IsNearbyStranger(challengerPlayer))
      return true;
    FiresideGatheringManager gatheringManager = FiresideGatheringManager.Get();
    if (gatheringManager.IsCheckedIn)
    {
      if (gatheringManager.IsPlayerInMyFSG(BnetUtils.GetPlayer(challengerPlayer)))
        return true;
      if (gatheringManager.CurrentFsgSharedSecretKey != null)
      {
        byte[] hash = SHA256.Create().ComputeHash(gatheringManager.CurrentFsgSharedSecretKey, 0, gatheringManager.CurrentFsgSharedSecretKey.Length);
        byte[] arr1;
        if (BattleNet.GetPartyAttribute<byte[]>(challengerPartyId, "fsg1", out arr1) && GeneralUtils.AreArraysEqual<byte>(arr1, hash))
          return true;
      }
    }
    return false;
  }

  private bool StartFriendlyChallengeGameIfReady()
  {
    if (!this.DidSendChallenge() || !BnetParty.IsInParty(this.m_data.m_partyId))
      return false;
    bool flag1 = this.m_data.m_challengerDeckId != 0L && this.m_data.m_challengeeDeckId != 0L;
    bool flag2 = this.m_data.m_challengerHeroId != 0L && this.m_data.m_challengeeHeroId != 0L;
    if (!flag1 && !flag2 || !this.m_data.m_challengerInGameState || !this.m_data.m_challengeeInGameState)
      return false;
    this.m_data.m_findGameErrorOccurred = false;
    BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("s1", "goto"), BnetAttribute.CreateAttribute("s2", "goto"));
    PegasusShared.FormatType formatType = this.GetFormatType();
    if (this.IsChallengeBacon())
      Network.Get().EnterBattlegroundsWithFriend(this.m_data.m_challengee.GetHearthstoneGameAccountId(), this.m_data.m_scenarioId);
    else if (flag1)
    {
      string deckShareStateAttribute1;
      BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p1DeckShareState", out deckShareStateAttribute1);
      DeckShareState enumFromAttribute1 = this.GetDeckShareStateEnumFromAttribute(deckShareStateAttribute1);
      string deckShareStateAttribute2;
      BattleNet.GetPartyAttribute<string>(this.m_data.m_partyId, "p2DeckShareState", out deckShareStateAttribute2);
      DeckShareState enumFromAttribute2 = this.GetDeckShareStateEnumFromAttribute(deckShareStateAttribute2);
      GameMgr.Get().EnterFriendlyChallengeGameWithDecks(formatType, this.m_data.m_challengeBrawlType, this.m_data.m_scenarioId, this.m_data.m_seasonId, this.m_data.m_brawlLibraryItemId, this.m_data.m_challengee.GetHearthstoneGameAccountId(), enumFromAttribute1, this.m_data.m_challengerDeckId, enumFromAttribute2, this.m_data.m_challengeeDeckId, this.m_data.m_challengerRandomHeroCardId, this.m_data.m_challengeeRandomHeroCardId, this.m_data.m_challengerCardBackId, this.m_data.m_challengeeCardBackId);
    }
    else
      GameMgr.Get().EnterFriendlyChallengeGameWithHeroes(formatType, this.m_data.m_challengeBrawlType, this.m_data.m_scenarioId, this.m_data.m_seasonId, this.m_data.m_brawlLibraryItemId, this.m_data.m_challengee.GetHearthstoneGameAccountId(), this.m_data.m_challengerHeroId, this.m_data.m_challengeeHeroId, this.m_data.m_challengerCardBackId, this.m_data.m_challengeeCardBackId);
    if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null)
    {
      this.m_challengeDialog.Hide();
      this.m_challengeDialog = (DialogBase) null;
    }
    return true;
  }

  private void SetIAmInGameState()
  {
    if (!BnetParty.IsInParty(this.m_data.m_partyId) || !this.m_data.m_challengerDeckOrHeroSelected || !this.m_data.m_challengeeDeckOrHeroSelected || this.AmIInGameState())
      return;
    if (this.DidSendChallenge())
    {
      this.m_data.m_challengerInGameState = true;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("s1", "game"));
    }
    else
    {
      this.m_data.m_challengeeInGameState = true;
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("s2", "game"));
    }
  }

  private void OnNetCacheReady()
  {
    NetCache.Get().UnregisterNetCacheHandler(new NetCache.NetCacheCallback(this.OnNetCacheReady));
    this.m_netCacheReady = true;
    if (SceneMgr.Get().GetMode() == SceneMgr.Mode.FATAL_ERROR)
      return;
    this.UpdateMyAvailability();
  }

  private void OnSceneUnloaded(SceneMgr.Mode prevMode, PegasusScene prevScene, object userData)
  {
    if (prevMode != SceneMgr.Mode.GAMEPLAY)
      this.UpdateMyAvailability();
    if (!this.m_data.m_updatePartyQuestInfoOnGameplaySceneUnload || prevMode != SceneMgr.Mode.GAMEPLAY)
      return;
    this.m_data.m_updatePartyQuestInfoOnGameplaySceneUnload = false;
    this.UpdatePartyQuestInfo();
  }

  private void OnSceneLoaded(SceneMgr.Mode mode, PegasusScene scene, object userData)
  {
    if (SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY || mode == SceneMgr.Mode.GAMEPLAY || mode == SceneMgr.Mode.FATAL_ERROR)
      return;
    this.m_netCacheReady = false;
    if (mode == SceneMgr.Mode.FRIENDLY || mode == SceneMgr.Mode.TAVERN_BRAWL && FriendChallengeMgr.Get().IsChallengeTavernBrawl())
      this.UpdateMyAvailability();
    else
      this.CancelChallenge();
    NetCache.Get().RegisterFriendChallenge(new NetCache.NetCacheCallback(this.OnNetCacheReady));
  }

  private void OnPlayersChanged(BnetPlayerChangelist changelist, object userData)
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    if (changelist.FindChange(myPlayer) != null)
    {
      bool flag = this.AmIAvailable();
      BnetGameAccount hearthstoneGameAccount = myPlayer.GetHearthstoneGameAccount();
      if (hearthstoneGameAccount != (BnetGameAccount) null && !this.m_myPlayerReady && hearthstoneGameAccount.HasGameField(20U) && hearthstoneGameAccount.HasGameField(19U))
      {
        this.m_myPlayerReady = true;
        if (!this.UpdateMyAvailability())
          flag = false;
      }
      if (!flag && this.m_data.m_challengerPending)
      {
        this.DeclineFriendChallenge_Internal(this.m_data.m_partyId);
        this.CleanUpChallengeData();
      }
    }
    if (!this.m_data.m_challengerPending)
      return;
    BnetPlayerChange change = changelist.FindChange(this.m_data.m_challengerId);
    if (change == null)
      return;
    BnetPlayer player = change.GetPlayer();
    if (!player.IsDisplayable())
      return;
    this.m_data.m_challenger = player;
    this.m_data.m_challengerPending = false;
    this.FireChangedEvent(FriendChallengeEvent.I_RECEIVED_CHALLENGE, this.m_data.m_challenger);
  }

  private void OnFriendsChanged(BnetFriendChangelist changelist, object userData)
  {
    if (!this.HasChallenge())
      return;
    List<BnetPlayer> removedFriends = changelist.GetRemovedFriends();
    if (removedFriends == null)
      return;
    BnetPlayer opponent = this.GetOpponent(BnetPresenceMgr.Get().GetMyPlayer());
    if (opponent == null)
      return;
    foreach (BnetPlayer bnetPlayer in removedFriends)
    {
      if (bnetPlayer == opponent)
      {
        PartyInfo[] joinedParties = BnetParty.GetJoinedParties();
        BnetGameAccountId hearthstoneGameAccountId = opponent.GetHearthstoneGameAccountId();
        foreach (PartyInfo partyInfo in joinedParties)
        {
          if (BnetParty.IsMember(partyInfo.Id, hearthstoneGameAccountId))
            BnetParty.Leave(partyInfo.Id);
        }
        this.RevertTavernBrawlPresenceStatus();
        FriendlyChallengeData challengeData = this.CleanUpChallengeData();
        this.FireChangedEvent(FriendChallengeEvent.OPPONENT_REMOVED_FROM_FRIENDS, opponent, challengeData);
        break;
      }
    }
  }

  private void OnNearbyPlayersChanged(
    BnetRecentOrNearbyPlayerChangelist changelist,
    object userData)
  {
    if (!this.HasChallenge())
      return;
    List<BnetPlayer> removedPlayers = changelist.GetRemovedPlayers();
    if (removedPlayers == null)
      return;
    BnetPlayer opponent = this.GetOpponent(BnetPresenceMgr.Get().GetMyPlayer());
    if (opponent == null)
      return;
    foreach (BnetPlayer bnetPlayer in removedPlayers)
    {
      if (bnetPlayer == opponent)
      {
        FriendlyChallengeData challengeData = this.CleanUpChallengeData();
        this.FireChangedEvent(FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE, opponent, challengeData);
        break;
      }
    }
  }

  private void OnDisconnectedFromBattleNet(BattleNetErrors error) => this.OnDisconnect();

  private void OnFatalError(FatalErrorMessage message, object userData) => this.OnDisconnect();

  private void OnDisconnect()
  {
    if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null)
    {
      this.m_challengeDialog.Hide();
      this.m_challengeDialog = (DialogBase) null;
    }
    this.CleanUpChallengeData();
  }

  private void OnReconnectLoginComplete() => this.UpdateMyAvailability();

  private void OnChallengeChanged(
    FriendChallengeEvent challengeEvent,
    BnetPlayer player,
    FriendlyChallengeData challengeData,
    object userData)
  {
    switch (challengeEvent)
    {
      case FriendChallengeEvent.I_SENT_CHALLENGE:
        this.ShowISentChallengeDialog(player);
        break;
      case FriendChallengeEvent.I_RESCINDED_CHALLENGE:
      case FriendChallengeEvent.I_DECLINED_CHALLENGE:
        this.OnChallengeCanceled();
        break;
      case FriendChallengeEvent.OPPONENT_ACCEPTED_CHALLENGE:
        this.StartChallengeProcess();
        break;
      case FriendChallengeEvent.OPPONENT_DECLINED_CHALLENGE:
        this.ShowOpponentDeclinedChallengeDialog(player, challengeData);
        break;
      case FriendChallengeEvent.I_RECEIVED_CHALLENGE:
        if (!this.CanPromptReceivedChallenge())
          break;
        if (this.IsChallengeTavernBrawl())
          PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING);
        this.ShowIReceivedChallengeDialog(player);
        break;
      case FriendChallengeEvent.I_ACCEPTED_CHALLENGE:
        this.StartChallengeProcess();
        break;
      case FriendChallengeEvent.OPPONENT_RESCINDED_CHALLENGE:
        this.OnChallengeCanceled();
        this.ShowOpponentCanceledChallengeDialog(player, challengeData);
        break;
      case FriendChallengeEvent.OPPONENT_CANCELED_CHALLENGE:
        FriendlyChallengeHelper.Get().HideAllDeckShareDialogs();
        this.OnChallengeCanceled();
        this.ShowOpponentCanceledChallengeDialog(player, challengeData);
        break;
      case FriendChallengeEvent.OPPONENT_REMOVED_FROM_FRIENDS:
        FriendlyChallengeHelper.Get().HideAllDeckShareDialogs();
        this.ShowOpponentRemovedFromFriendsDialog(player, challengeData);
        break;
      case FriendChallengeEvent.I_REQUESTED_DECK_SHARE:
        if (FriendlyChallengeHelper.Get().IsShowingDeckShareRequestDialog())
          break;
        FriendlyChallengeHelper.Get().ShowDeckShareRequestWaitingDialog(new AlertPopup.ResponseCallback(this.OnFriendChallengeDeckShareRequestDialogWaitingResponse));
        break;
      case FriendChallengeEvent.I_ACCEPTED_DECK_SHARE_REQUEST:
        FriendlyChallengeHelper.Get().HideDeckShareRequestDialog();
        this.ShareDecks_InternalParty();
        break;
      case FriendChallengeEvent.I_DECLINED_DECK_SHARE_REQUEST:
        FriendlyChallengeHelper.Get().HideDeckShareRequestDialog();
        break;
      case FriendChallengeEvent.I_CANCELED_DECK_SHARE_REQUEST:
        FriendlyChallengeHelper.Get().HideDeckShareRequestWaitingDialog();
        break;
      case FriendChallengeEvent.DECK_SHARE_ERROR_OCCURED:
        if (this.DidSendChallenge())
          BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1DeckShareState", "none"));
        else if (this.DidReceiveChallenge())
          BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2DeckShareState", "none"));
        FriendlyChallengeHelper.Get().HideAllDeckShareDialogs();
        FriendlyChallengeHelper.Get().ShowDeckShareErrorDialog();
        break;
      case FriendChallengeEvent.OPPONENT_REQUESTED_DECK_SHARE:
        FriendlyChallengeHelper.Get().HideAllDeckShareDialogs();
        FriendlyChallengeHelper.Get().HideFriendChallengeWaitingForOpponentDialog();
        FriendlyChallengeHelper.Get().ShowDeckShareRequestDialog(new AlertPopup.ResponseCallback(this.OnFriendChallengeDeckShareRequestDialogResponse));
        break;
      case FriendChallengeEvent.OPPONENT_DECLINED_DECK_SHARE_REQUEST:
        if (this.DidSendChallenge())
          BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p1DeckShareState", "none"));
        else if (this.DidReceiveChallenge())
          BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("p2DeckShareState", "none"));
        FriendlyChallengeHelper.Get().ShowDeckShareRequestDeclinedDialog();
        FriendlyChallengeHelper.Get().HideDeckShareRequestWaitingDialog();
        break;
      case FriendChallengeEvent.OPPONENT_ACCEPTED_DECK_SHARE_REQUEST:
        FriendlyChallengeHelper.Get().HideDeckShareRequestWaitingDialog();
        break;
      case FriendChallengeEvent.OPPONENT_CANCELED_DECK_SHARE_REQUEST:
        FriendlyChallengeHelper.Get().ShowDeckShareRequestCanceledDialog();
        FriendlyChallengeHelper.Get().HideDeckShareRequestDialog();
        break;
      case FriendChallengeEvent.QUEUE_CANCELED:
        this.OnChallengeCanceled();
        this.ShowQueueCanceledDialog(player, challengeData);
        break;
    }
  }

  private void OnChallengeCanceled()
  {
    if (SceneMgr.Get() != null && SceneMgr.Get().GetMode() != SceneMgr.Mode.FIRESIDE_GATHERING && FiresideGatheringManager.Get() != null)
      FiresideGatheringManager.Get().CurrentFiresideGatheringMode = FiresideGatheringManager.FiresideGatheringMode.NONE;
    GameMgr.Get().CancelFindGame();
    GameMgr.Get().HideTransitionPopup();
  }

  private bool CanPromptReceivedChallenge()
  {
    bool flag = !UserAttentionManager.CanShowAttentionGrabber("FriendlyChallengeMgr.CanPromptReceivedChallenge");
    if (!flag)
    {
      if (GameMgr.Get().IsFindingGame())
        flag = true;
      else if (RankMgr.Get().IsLegendRankInAnyFormat)
        flag = SceneMgr.Get().IsModeRequested(SceneMgr.Mode.TOURNAMENT);
    }
    if (flag)
    {
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("WTCG.Friendly.DeclineReason", 6L));
      this.DeclineChallenge();
      return false;
    }
    if (this.IsChallengeTavernBrawl())
    {
      if (!TavernBrawlManager.Get().HasUnlockedTavernBrawl(this.m_data.m_challengeBrawlType) && !this.PlayersInSameFiresideGathering())
      {
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("WTCG.Friendly.DeclineReason", 5L));
        this.DeclineChallenge();
        return false;
      }
      TavernBrawlManager.Get().EnsureAllDataReady(this.m_data.m_challengeBrawlType, new TavernBrawlManager.CallbackEnsureServerDataReady(this.TavernBrawl_ReceivedChallenge_OnEnsureServerDataReady));
      return false;
    }
    if (!CollectionManager.Get().AreAllDeckContentsReady())
    {
      CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded(new CollectionManager.DelOnAllDeckContents(this.CanPromptReceivedChallenge_OnDeckContentsLoaded));
      return false;
    }
    if (this.IsChallengeStandardDuel() && !CollectionManager.Get().AccountHasValidDeck(PegasusShared.FormatType.FT_STANDARD))
    {
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("WTCG.Friendly.DeclineReason", 3L));
      this.DeclineChallenge();
      return false;
    }
    if (this.IsChallengeWildDuel())
    {
      if (!CollectionManager.Get().ShouldAccountSeeStandardWild())
      {
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("WTCG.Friendly.DeclineReason", 7L));
        this.DeclineChallenge();
        return false;
      }
      if (!CollectionManager.Get().AccountHasValidDeck(PegasusShared.FormatType.FT_WILD))
      {
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("WTCG.Friendly.DeclineReason", 2L));
        this.DeclineChallenge();
        return false;
      }
    }
    else
    {
      if (this.IsChallengeClassicDuel() && !CollectionManager.Get().AccountHasValidDeck(PegasusShared.FormatType.FT_CLASSIC))
      {
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("WTCG.Friendly.DeclineReason", 9L));
        this.DeclineChallenge();
        return false;
      }
      if (this.IsChallengeBacon() && !GameUtils.IsBattleGroundsTutorialComplete())
      {
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("WTCG.Friendly.DeclineReason", 10L));
        this.DeclineChallenge();
        return false;
      }
      if (this.IsChallengeMercenaries() && !GameUtils.IsMercenariesVillageTutorialComplete())
      {
        BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("WTCG.Friendly.DeclineReason", 11L));
        this.DeclineChallenge();
        return false;
      }
    }
    return true;
  }

  private void CanPromptReceivedChallenge_OnDeckContentsLoaded()
  {
    if (!this.DidReceiveChallenge() || !this.CanPromptReceivedChallenge())
      return;
    this.ShowIReceivedChallengeDialog(this.m_data.m_challenger);
  }

  private void TavernBrawl_ReceivedChallenge_OnEnsureServerDataReady()
  {
    TavernBrawlMission mission = TavernBrawlManager.Get().GetMission(this.m_data.m_challengeBrawlType);
    FriendChallengeMgr.DeclineReason? nullable = new FriendChallengeMgr.DeclineReason?();
    if (mission == null)
      nullable = new FriendChallengeMgr.DeclineReason?(FriendChallengeMgr.DeclineReason.None);
    if (mission != null && mission.CanCreateDeck(this.m_data.m_brawlLibraryItemId) && !TavernBrawlManager.Get().HasValidDeck(this.m_data.m_challengeBrawlType))
      nullable = new FriendChallengeMgr.DeclineReason?(FriendChallengeMgr.DeclineReason.TavernBrawlNoValidDeck);
    if (nullable.HasValue)
    {
      BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("WTCG.Friendly.DeclineReason", (long) nullable.Value));
      this.DeclineChallenge();
    }
    else
    {
      if (this.IsChallengeTavernBrawl())
        PresenceMgr.Get().SetStatus((Enum) Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING);
      this.ShowIReceivedChallengeDialog(this.m_data.m_challenger);
    }
  }

  private bool RevertTavernBrawlPresenceStatus()
  {
    if (!this.IsChallengeTavernBrawl() || PresenceMgr.Get().CurrentStatus != Global.PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING)
      return false;
    PresenceMgr.Get().SetPrevStatus();
    return true;
  }

  private bool OnFindGameEvent(FindGameEventData eventData, object userData)
  {
    this.UpdateMyAvailability();
    switch (eventData.m_state)
    {
      case FindGameState.BNET_QUEUE_ENTERED:
      case FindGameState.SERVER_GAME_CONNECTING:
        if (this.HasChallenge())
        {
          this.DeselectDeckOrHero();
          break;
        }
        break;
      case FindGameState.BNET_QUEUE_CANCELED:
      case FindGameState.BNET_ERROR:
        this.m_data.m_findGameErrorOccurred = true;
        if (this.DidSendChallenge())
        {
          BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("error", (long) GameMgr.Get().GetLastEnterGameError()));
          BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("s1", "deck"));
        }
        else if (this.DidReceiveChallenge())
          BattleNet.SetPartyAttributes(this.m_data.m_partyId, BnetAttribute.CreateAttribute("s2", "deck"));
        SceneMgr.Mode mode = SceneMgr.Get().GetMode();
        int num;
        switch (mode)
        {
          case SceneMgr.Mode.FRIENDLY:
          case SceneMgr.Mode.TAVERN_BRAWL:
            num = 0;
            break;
          default:
            num = mode != SceneMgr.Mode.FIRESIDE_GATHERING ? 1 : 0;
            break;
        }
        bool flag = num != 0;
        if (this.DidSendChallenge() && this.IsChallengeFiresideBrawl())
          flag = true;
        if (flag)
        {
          this.QueueCanceled();
          break;
        }
        break;
    }
    return false;
  }

  private void WillReset()
  {
    this.CleanUpChallengeData(false);
    if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null)
    {
      this.m_challengeDialog.Hide();
      this.m_challengeDialog = (DialogBase) null;
    }
    FriendlyChallengeHelper.Get().HideAllDeckShareDialogs();
  }

  private void ShowISentChallengeDialog(BnetPlayer challengee) => DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
  {
    m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
    m_text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_OPPONENT_WAITING_RESPONSE", (object) FriendUtils.GetUniqueName(challengee)),
    m_showAlertIcon = false,
    m_responseDisplay = AlertPopup.ResponseDisplay.NONE,
    m_responseCallback = new AlertPopup.ResponseCallback(this.OnChallengeSentDialogResponse),
    m_layerToUse = new GameLayer?(GameLayer.UI)
  }, new DialogManager.DialogProcessCallback(this.OnChallengeSentDialogProcessed));

  private void ShowOpponentDeclinedChallengeDialog(
    BnetPlayer challengee,
    FriendlyChallengeData challengeData)
  {
    if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null)
    {
      this.m_challengeDialog.Hide();
      this.m_challengeDialog = (DialogBase) null;
    }
    if (this.m_hasSeenDeclinedReason)
      return;
    AlertPopup.PopupInfo info = new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
      m_text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_OPPONENT_DECLINED", (object) FriendUtils.GetUniqueName(challengee)),
      m_alertTextAlignment = UberText.AlignmentOptions.Center,
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnOpponentDeclinedChallengeDialogDismissed)
    };
    DialogManager.Get().ShowPopup(info);
  }

  private void OnOpponentDeclinedChallengeDialogDismissed(
    AlertPopup.Response response,
    object userData)
  {
    ChatMgr.Get().UpdateFriendItemsWhenAvailable();
  }

  private void ShowIReceivedChallengeDialog(BnetPlayer challenger)
  {
    if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null)
    {
      this.m_challengeDialog.Hide();
      this.m_challengeDialog = (DialogBase) null;
    }
    DialogManager.Get().ShowFriendlyChallenge(this.m_data.m_challengeFormatType, challenger, this.IsChallengeTavernBrawl(), PartyType.FRIENDLY_CHALLENGE, (PartyQuestInfo) null, new FriendlyChallengeDialog.ResponseCallback(this.OnChallengeReceivedDialogResponse), new DialogManager.DialogProcessCallback(this.OnChallengeReceivedDialogProcessed));
  }

  private void ShowOpponentCanceledChallengeDialog(
    BnetPlayer otherPlayer,
    FriendlyChallengeData challengeData)
  {
    if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null)
    {
      this.m_challengeDialog.Hide();
      this.m_challengeDialog = (DialogBase) null;
    }
    if (GameMgr.Get() != null && this.SuppressChallengeCanceledDialogByMissionId(GameMgr.Get().GetMissionId()) || SceneMgr.Get() != null && SceneMgr.Get().IsInGame() && GameState.Get() != null && !GameState.Get().IsGameOverNowOrPending() || (challengeData.m_challengeBrawlType == BrawlType.BRAWL_TYPE_FIRESIDE_GATHERING ? 1 : (SceneMgr.Get().GetMode() == SceneMgr.Mode.FIRESIDE_GATHERING ? 1 : 0)) != 0 && (!challengeData.m_challengeeAccepted || challengeData.IsPendingGotoGame || challengeData.m_findGameErrorOccurred))
      return;
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
      m_text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_OPPONENT_CANCELED", (object) FriendUtils.GetUniqueName(otherPlayer)),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK,
      m_responseCallback = new AlertPopup.ResponseCallback(this.OnOpponentCanceledChallengeDialogClosed)
    });
  }

  public void OnOpponentCanceledChallengeDialogClosed(AlertPopup.Response response, object userData)
  {
    if (!SceneMgr.Get().IsTransitionNowOrPending() || SceneMgr.Get().GetPrevMode() == SceneMgr.Mode.FRIENDLY)
      return;
    SceneMgr.Get().ReturnToPreviousMode();
  }

  private void ShowOpponentRemovedFromFriendsDialog(
    BnetPlayer otherPlayer,
    FriendlyChallengeData challengeData)
  {
    if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null)
    {
      this.m_challengeDialog.Hide();
      this.m_challengeDialog = (DialogBase) null;
    }
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
      m_text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_OPPONENT_FRIEND_REMOVED", (object) FriendUtils.GetUniqueName(otherPlayer)),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  private void ShowQueueCanceledDialog(BnetPlayer otherPlayer, FriendlyChallengeData challengeData)
  {
    if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null)
    {
      this.m_challengeDialog.Hide();
      this.m_challengeDialog = (DialogBase) null;
    }
    DialogManager.Get().ShowPopup(new AlertPopup.PopupInfo()
    {
      m_headerText = GameStrings.Get("GLOBAL_FRIEND_CHALLENGE_HEADER"),
      m_text = GameStrings.Format("GLOBAL_FRIEND_CHALLENGE_QUEUE_CANCELED"),
      m_showAlertIcon = false,
      m_responseDisplay = AlertPopup.ResponseDisplay.OK
    });
  }

  private bool OnChallengeSentDialogProcessed(DialogBase dialog, object userData)
  {
    if (!this.DidSendChallenge() || this.m_data.m_challengeeAccepted)
      return false;
    this.m_challengeDialog = dialog;
    this.UpdateChallengeSentDialog();
    return true;
  }

  private void UpdateChallengeSentDialog()
  {
    if (this.m_data.m_partyId == (BnetPartyId) null || (UnityEngine.Object) this.m_challengeDialog == (UnityEngine.Object) null)
      return;
    AlertPopup challengeDialog = (AlertPopup) this.m_challengeDialog;
    AlertPopup.PopupInfo info = challengeDialog.GetInfo();
    if (info == null)
      return;
    info.m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL;
    challengeDialog.UpdateInfo(info);
  }

  private void OnChallengeSentDialogResponse(AlertPopup.Response response, object userData)
  {
    this.m_challengeDialog = (DialogBase) null;
    this.RescindChallenge();
  }

  private bool OnChallengeReceivedDialogProcessed(DialogBase dialog, object userData)
  {
    if (!this.DidReceiveChallenge())
      return false;
    this.m_challengeDialog = dialog;
    PartyQuestInfo partyQuestInfo = this.GetPartyQuestInfo();
    if (partyQuestInfo != null)
      ((FriendlyChallengeDialog) dialog).SetQuestInfo(partyQuestInfo);
    return true;
  }

  private void OnChallengeReceivedDialogResponse(bool accept)
  {
    this.m_challengeDialog = (DialogBase) null;
    if (accept)
      this.AcceptChallenge();
    else
      this.DeclineChallenge();
  }

  private void HandleJoinedParty(BnetPartyId partyId, BnetGameAccountId otherMemberId)
  {
    this.m_data.m_partyId = partyId;
    this.m_data.m_challengerId = otherMemberId;
    this.m_data.m_challenger = BnetUtils.GetPlayer(this.m_data.m_challengerId);
    this.m_data.m_challengee = BnetPresenceMgr.Get().GetMyPlayer();
    this.m_hasSeenDeclinedReason = false;
    if (this.m_data.m_challenger == null || !this.m_data.m_challenger.IsDisplayable())
    {
      this.m_data.m_challengerPending = true;
      this.UpdateMyAvailability();
    }
    else
    {
      this.UpdateMyAvailability();
      this.FireChangedEvent(FriendChallengeEvent.I_RECEIVED_CHALLENGE, this.m_data.m_challenger);
    }
  }

  public bool UpdateMyAvailability()
  {
    if (!Network.ShouldBeConnectedToAurora() || !Network.IsLoggedIn())
      return false;
    bool flag1 = !this.HasAvailabilityBlocker();
    bool flag2 = GameUtils.CanCheckTutorialCompletion() && GameUtils.IsBattleGroundsTutorialComplete();
    bool flag3 = GameUtils.CanCheckTutorialCompletion() && GameUtils.IsMercenariesVillageTutorialComplete();
    Log.Presence.PrintDebug("UpdateMyAvailability: Available=" + flag1.ToString());
    this.m_canBeInvitedToGame = flag1;
    this.m_canBeInvitedToBattlegrounds = flag2;
    this.m_canBeInvitedToMercenaries = flag3;
    if (!this.m_updateMyAvailabilityCallbackScheduledThisFrame)
      Processor.ScheduleCallback(0.0f, false, new Processor.ScheduledCallback(this.UpdateMyAvailabilityScheduledCallback));
    this.m_updateMyAvailabilityCallbackScheduledThisFrame = true;
    return flag1;
  }

  private void UpdateMyAvailabilityScheduledCallback(object userData)
  {
    if (!this.m_updateMyAvailabilityCallbackScheduledThisFrame)
      return;
    this.m_updateMyAvailabilityCallbackScheduledThisFrame = false;
    Log.Presence.PrintDebug("UpdateMyAvailabilityScheduledCallback: Available=" + this.m_canBeInvitedToGame.ToString());
    BnetPresenceMgr.Get().SetGameField(1U, this.m_canBeInvitedToGame);
    BnetNearbyPlayerMgr.Get().SetAvailability(this.m_canBeInvitedToGame);
    BnetNearbyPlayerMgr.Get().SetBattlegroundsAvailability(this.m_canBeInvitedToBattlegrounds);
    BnetNearbyPlayerMgr.Get().SetMercenariesAvailability(this.m_canBeInvitedToMercenaries);
  }

  private bool HasAvailabilityBlocker() => this.GetAvailabilityBlockerReason() != AvailabilityBlockerReasons.NONE;

  private AvailabilityBlockerReasons GetAvailabilityBlockerReason()
  {
    AvailabilityBlockerReasons availabilityBlockerReason = AvailabilityBlockerReasons.NONE;
    if (!this.m_netCacheReady)
      availabilityBlockerReason = AvailabilityBlockerReasons.NETCACHE_NOT_READY;
    if (!this.m_myPlayerReady)
      availabilityBlockerReason = AvailabilityBlockerReasons.MY_PLAYER_NOT_READY;
    if (this.HasChallenge())
      availabilityBlockerReason = AvailabilityBlockerReasons.HAS_EXISTING_CHALLENGE;
    if (PartyManager.Get().HasPendingPartyInviteOrDialog())
      availabilityBlockerReason = AvailabilityBlockerReasons.HAS_PENDING_PARTY_INVITE;
    if (availabilityBlockerReason == AvailabilityBlockerReasons.NONE)
      availabilityBlockerReason = UserAttentionManager.GetAvailabilityBlockerReason(true);
    if (availabilityBlockerReason != AvailabilityBlockerReasons.NONE)
      Log.Presence.PrintDebug("GetAvailabilityBlockerReason: " + availabilityBlockerReason.ToString());
    return availabilityBlockerReason;
  }

  private void FireChangedEvent(
    FriendChallengeEvent challengeEvent,
    BnetPlayer player,
    FriendlyChallengeData challengeData = null)
  {
    if (challengeData == null)
      challengeData = this.m_data;
    foreach (FriendChallengeMgr.ChangedListener changedListener in this.m_changedListeners.ToArray())
      changedListener.Fire(challengeEvent, player, challengeData);
  }

  private FriendlyChallengeData CleanUpChallengeData(bool updateAvailability = true)
  {
    FriendlyChallengeData data = this.m_data;
    this.m_data = new FriendlyChallengeData();
    if (!updateAvailability)
      return data;
    this.UpdateMyAvailability();
    return data;
  }

  private void StartChallengeProcess()
  {
    bool flag1 = !this.DidSendChallenge() && this.m_data.m_challengeeDeckOrHeroSelected || this.DidSendChallenge() && this.m_data.m_challengerDeckOrHeroSelected;
    if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null && !flag1)
    {
      this.m_challengeDialog.Hide();
      this.m_challengeDialog = (DialogBase) null;
    }
    GameMgr.Get().SetPendingAutoConcede(true);
    if (CollectionManager.Get().IsInEditMode())
      CollectionManager.Get().GetEditedDeck()?.SendChanges(CollectionDeck.ChangeSource.StartChallengeProcess);
    if (this.IsChallengeTavernBrawl())
    {
      TavernBrawlManager.Get().CurrentBrawlType = this.m_data.m_challengeBrawlType;
      TavernBrawlManager.Get().CurrentMission()?.SetSelectedBrawlLibraryItemId(this.m_data.m_brawlLibraryItemId);
    }
    if (this.IsChallengeBacon())
      this.SkipDeckSelection();
    else if (this.IsChallengeTavernBrawl() && !TavernBrawlManager.Get().SelectHeroBeforeMission(this.m_data.m_challengeBrawlType))
    {
      if (TavernBrawlManager.Get().GetMission(this.m_data.m_challengeBrawlType).canCreateDeck)
      {
        if (TavernBrawlManager.Get().HasValidDeck(this.m_data.m_challengeBrawlType))
          this.SelectDeck(TavernBrawlManager.Get().GetDeck(this.m_data.m_challengeBrawlType).ID);
        else
          Debug.LogError((object) "Attempting to start a Tavern Brawl challenge without a valid deck!  How did this happen?");
      }
      else
        this.SkipDeckSelection();
    }
    else
    {
      if (!this.IsChallengeTavernBrawl())
      {
        if (this.m_data.m_challengeFormatType == PegasusShared.FormatType.FT_UNKNOWN)
        {
          RankMgr.LogMessage("m_data.m_challengeFormatType = FT_UNKOWN", nameof (StartChallengeProcess), "D:\\builders\\work\\source\\25.0.0\\Pegasus\\Client\\Assets\\Game\\Bnet\\Scripts\\FriendChallengeMgr.cs", 3707);
          return;
        }
        Options.SetFormatType(this.m_data.m_challengeFormatType);
      }
      bool flag2 = this.DidSendChallenge() && this.m_data.m_challengerDeckOrHeroSelected;
      if (!this.ShouldTransitionToFriendlySceneAccordingToChallengeMethod() & flag2)
        return;
      if ((UnityEngine.Object) this.m_challengeDialog != (UnityEngine.Object) null)
      {
        this.m_challengeDialog.Hide();
        this.m_challengeDialog = (DialogBase) null;
      }
      Navigation.Clear();
      SceneMgr.Get().SetNextMode(SceneMgr.Mode.FRIENDLY);
    }
  }

  private bool SuppressChallengeCanceledDialogByMissionId(int missionId) => missionId == 3459;

  public enum ChallengeMethod
  {
    INVALID,
    FROM_FRIEND_LIST,
    FROM_FIRESIDE_GATHERING_OPPONENT_PICKER,
  }

  public enum DeclineReason
  {
    None,
    UserDeclined,
    NoValidDeck,
    StandardNoValidDeck,
    TavernBrawlNoValidDeck,
    TavernBrawlNotUnlocked,
    UserIsBusy,
    NotSeenWild,
    BattlegroundsNoEarlyAccess,
    ClassicNoValidDeck,
    BattlegroundsTutorialNotComplete,
    MercsTutorialNotComplete,
  }

  public delegate void ChangedCallback(
    FriendChallengeEvent challengeEvent,
    BnetPlayer player,
    FriendlyChallengeData challengeData,
    object userData);

  private class ChangedListener : EventListener<FriendChallengeMgr.ChangedCallback>
  {
    public void Fire(
      FriendChallengeEvent challengeEvent,
      BnetPlayer player,
      FriendlyChallengeData challengeData)
    {
      this.m_callback(challengeEvent, player, challengeData, this.m_userData);
    }
  }
}
