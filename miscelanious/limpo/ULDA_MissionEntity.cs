using Hearthstone.DungeonCrawl;
using System.Collections;
using UnityEngine;

public abstract class ULDA_MissionEntity : GenericDungeonMissionEntity
{
  public override sealed AdventureDbId GetAdventureID() => AdventureDbId.ULDUM;

  protected override sealed bool CanPlayVOLines(
    Entity speakerEntity,
    GenericDungeonMissionEntity.VOSpeaker speaker)
  {
    return speaker == GenericDungeonMissionEntity.VOSpeaker.FRIENDLY_HERO ? speakerEntity.GetCardId().Contains("ULDA_") : base.CanPlayVOLines(speakerEntity, speaker);
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.LOST)
      yield return (object) new WaitForSeconds(5f);
  }

  public override bool ShouldShowHeroClassDuringMulligan(Player.Side playerSide) => false;

  public override void OnPlayThinkEmote()
  {
    if (this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide() || currentPlayer.GetHeroCard().HasActiveEmoteSound())
      return;
    EmoteType emoteType = EmoteType.THINK1;
    switch (Random.Range(1, 4))
    {
      case 1:
        emoteType = EmoteType.THINK1;
        break;
      case 2:
        emoteType = EmoteType.THINK2;
        break;
      case 3:
        emoteType = EmoteType.THINK3;
        break;
    }
    GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(emoteType);
  }

  public override void StartMulliganSoundtracks(bool soft)
  {
    if (soft)
      return;
    MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ULDMulligan);
  }

  public int GetDefeatedBossCountForFinalBoss()
  {
    switch (GameMgr.Get().GetMissionId())
    {
      case 3432:
      case 3437:
        return 0;
      default:
        return 7;
    }
  }

  public override void StartGameplaySoundtracks()
  {
    if (GameUtils.GetDefeatedBossCount() == this.GetDefeatedBossCountForFinalBoss())
      MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ULDFinalBoss);
    else
      base.StartGameplaySoundtracks();
  }

  public static bool GetIsFirstBoss()
  {
    AdventureDataDbfRecord adventureDataRecord = ULDA_MissionEntity.GetAdventureDataRecord(Options.Get().GetInt(Option.SELECTED_ADVENTURE), Options.Get().GetInt(Option.SELECTED_ADVENTURE_MODE));
    return adventureDataRecord == null || !DungeonCrawlUtil.IsDungeonRunActive((GameSaveKeyId) adventureDataRecord.GameSaveDataServerKey);
  }

  public static AdventureDataDbfRecord GetAdventureDataRecord(
    int adventureId,
    int modeId)
  {
    foreach (AdventureDataDbfRecord record in GameDbf.AdventureData.GetRecords())
    {
      if (record.AdventureId == adventureId && record.ModeId == modeId)
        return record;
    }
    return (AdventureDataDbfRecord) null;
  }
}
