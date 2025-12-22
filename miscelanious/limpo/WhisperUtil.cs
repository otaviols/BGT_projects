using Blizzard.GameService.SDK.Client.Integration;

public static class WhisperUtil
{
  public static BnetPlayer GetSpeaker(BnetWhisper whisper) => BnetUtils.GetPlayer(whisper.GetSpeakerId());

  public static BnetPlayer GetReceiver(BnetWhisper whisper) => BnetUtils.GetPlayer(whisper.GetReceiverId());

  public static BnetPlayer GetTheirPlayer(BnetWhisper whisper)
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    if (myPlayer == null)
      return (BnetPlayer) null;
    BnetPlayer speaker = WhisperUtil.GetSpeaker(whisper);
    BnetPlayer receiver = WhisperUtil.GetReceiver(whisper);
    if (myPlayer == speaker)
      return receiver;
    return myPlayer == receiver ? speaker : (BnetPlayer) null;
  }

  public static BnetAccountId GetTheirAccountId(BnetWhisper whisper)
  {
    BnetPlayer myPlayer = BnetPresenceMgr.Get().GetMyPlayer();
    if (myPlayer == null)
      return (BnetAccountId) null;
    if (myPlayer.HasAccount((BnetEntityId) whisper.GetSpeakerId()))
      return whisper.GetReceiverId();
    return myPlayer.HasAccount((BnetEntityId) whisper.GetReceiverId()) ? whisper.GetSpeakerId() : (BnetAccountId) null;
  }

  public static bool IsDisplayable(BnetWhisper whisper)
  {
    BnetPlayer speaker = WhisperUtil.GetSpeaker(whisper);
    BnetPlayer receiver = WhisperUtil.GetReceiver(whisper);
    return speaker != null && speaker.IsDisplayable() && receiver != null && receiver.IsDisplayable();
  }

  public static bool IsSpeaker(BnetPlayer player, BnetWhisper whisper) => player != null && player.HasAccount((BnetEntityId) whisper.GetSpeakerId());

  public static bool IsSpeakerOrReceiver(BnetPlayer player, BnetWhisper whisper)
  {
    if (player == null)
      return false;
    return player.HasAccount((BnetEntityId) whisper.GetSpeakerId()) || player.HasAccount((BnetEntityId) whisper.GetReceiverId());
  }
}
