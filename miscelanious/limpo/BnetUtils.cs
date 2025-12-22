using Blizzard.GameService.SDK.Client.Integration;
using PegasusShared;
using UnityEngine;

public static class BnetUtils
{
  public static BnetPlayer GetPlayer(BnetAccountId id) => (BnetEntityId) id == (BnetEntityId) null ? (BnetPlayer) null : BnetNearbyPlayerMgr.Get().FindNearbyStranger(id) ?? BnetPresenceMgr.Get().GetPlayer(id);

  public static BnetPlayer GetPlayer(BnetGameAccountId id) => (BnetEntityId) id == (BnetEntityId) null ? (BnetPlayer) null : BnetNearbyPlayerMgr.Get().FindNearbyStranger(id) ?? BnetPresenceMgr.Get().GetPlayer(id);

  public static string GetPlayerBestName(BnetGameAccountId id)
  {
    BnetPlayer player = BnetUtils.GetPlayer(id);
    string playerBestName = player == null ? (string) null : player.GetBestName();
    if (string.IsNullOrEmpty(playerBestName))
      playerBestName = GameStrings.Get("GLOBAL_PLAYER_PLAYER");
    return playerBestName;
  }

  public static bool HasPlayerBestNamePresence(BnetGameAccountId id)
  {
    BnetPlayer player = BnetUtils.GetPlayer(id);
    return !string.IsNullOrEmpty(player == null ? (string) null : player.GetBestName());
  }

  public static string GetInviterBestName(PartyInvite invite)
  {
    if (invite != null && !string.IsNullOrEmpty(invite.InviterName))
      return invite.InviterName;
    BnetPlayer bnetPlayer = invite == null ? (BnetPlayer) null : BnetUtils.GetPlayer(invite.InviterId);
    string inviterBestName = bnetPlayer == null ? (string) null : bnetPlayer.GetBestName();
    if (string.IsNullOrEmpty(inviterBestName))
      inviterBestName = GameStrings.Get("GLOBAL_PLAYER_PLAYER");
    return inviterBestName;
  }

  public static bool CanReceiveWhisperFrom(BnetAccountId id) => !BnetPresenceMgr.Get().GetMyPlayer().IsBusy() && BnetFriendMgr.Get().IsFriend(id);

  public static BnetPartyId CreatePartyId(BnetId protoEntityId) => new BnetPartyId(protoEntityId.Hi, protoEntityId.Lo);

  public static BnetId CreatePegasusBnetId(BnetPartyId partyId)
  {
    BnetId pegasusBnetId = new BnetId();
    BnetEntityId bnetEntityId = partyId.ToBnetEntityId();
    pegasusBnetId.Hi = bnetEntityId.High;
    pegasusBnetId.Lo = bnetEntityId.Low;
    return pegasusBnetId;
  }

  public static BnetId CreatePegasusBnetId(BnetEntityId src) => new BnetId()
  {
    Hi = src.High,
    Lo = src.Low
  };

  public static string GetNameForProgramId(BnetProgramId programId)
  {
    string nameTag = BnetProgramId.GetNameTag(programId);
    return nameTag != null ? GameStrings.Get(nameTag) : (string) null;
  }

  public static ulong? TryGetGameAccountId() => !BattleNet.IsInitialized() ? new ulong?() : new ulong?(BattleNet.GetMyGameAccountId().Low);

  public static ulong? TryGetBnetAccountId() => !BattleNet.IsInitialized() ? new ulong?() : new ulong?(BattleNet.GetMyAccoundId().Low);

  public static BnetRegion? TryGetBnetRegion() => !BattleNet.IsInitialized() ? new BnetRegion?() : new BnetRegion?(BattleNet.GetAccountRegion());

  public static BnetRegion? TryGetGameRegion() => !BattleNet.IsInitialized() ? new BnetRegion?() : new BnetRegion?(BattleNet.GetCurrentRegion());

  public static bool IsPlayerPartOfSamplingPercentage(float samplingPercentage)
  {
    ulong? gameAccountId = BnetUtils.TryGetGameAccountId();
    float? nullable = gameAccountId.HasValue ? new float?((float) gameAccountId.GetValueOrDefault()) : new float?();
    if (nullable.HasValue)
      return (double) nullable.Value % 100.0 / 100.0 < (double) samplingPercentage;
    Debug.LogError((object) "Could Not Retrieve Game Account Id");
    return false;
  }
}
