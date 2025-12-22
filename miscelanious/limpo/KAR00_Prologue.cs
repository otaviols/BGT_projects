using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAR00_Prologue : KAR_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_KarazhanPrologue);

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_Malchezaar_Male_Demon_PrologueEmoteResponse_01.prefab:86f1f2ae80c38214ead0c43ba589b03c");
    this.PreloadSound("VO_Malchezaar_Male_Demon_PrologueHeroPower_01.prefab:34d993460aa6bf249874ef52c64bb8c8");
    this.PreloadSound("VO_Malchezaar_Male_Demon_PrologueWin_01.prefab:9d47723e51ef0fc469e66cb93ca6cba6");
    this.PreloadSound("VO_Malchezaar_Male_Demon_PrologueTurn1_04.prefab:335b8e61366fc9745b8b7cd0a61cc5c7");
    this.PreloadSound("VO_Malchezaar_Male_Demon_PrologueTurn11_02.prefab:fbd92ceb10f5ee94c9e2564aa6124df0");
    this.PreloadSound("VO_Medivh_Male_Human_PrologueTurn1_02.prefab:102f563763c38a249bdb734e6963ae37");
    this.PreloadSound("VO_Medivh_Male_Human_PrologueTurn3_02.prefab:da4713d2dea65a449be2485948df8f47");
    this.PreloadSound("VO_Medivh_Male_Human_PrologueTurn9_01.prefab:b92c01b534d9bc446999405722204a9e");
    this.PreloadSound("VO_Medivh_Male_Human_PrologueTurn11_01.prefab:e350d5335927c4c429be030ed6c3bda1");
    this.PreloadSound("VO_Medivh_Male_Human_PrologueHeroPower_01.prefab:2a948f10bad7a4f4190c2e9b7d1f5e7c");
    this.PreloadSound("VO_Medivh_Male_Human_PrologueWin_01.prefab:fa080d80cc356ac4587e802bbb856afb");
    this.PreloadSound("VO_Moroes_Male_Human_PrologueTurn5_03.prefab:024bdb88f10206046a9a929fa350d4b9");
    this.PreloadSound("VO_Moroes_Male_Human_PrologueTurn7_01.prefab:9b60f6c47c2a6c5498fbba1a1c6ec35a");
    this.PreloadSound("VO_Moroes_Male_Human_PrologueTurn9_02.prefab:3840159d3b8b5414e94ab693ede0c0d2");
    this.PreloadSound("VO_Moroes_Male_Human_PrologueTurn11_01.prefab:a5383482754ffb646a13daf16819a892");
    this.PreloadSound("VO_Moroes_Male_Human_PrologueWin_02.prefab:8d0d87405b3c8d5438a44faa1ed92d17");
  }

  protected override void InitEmoteResponses() => this.m_emoteResponseGroups = new List<MissionEntity.EmoteResponseGroup>()
  {
    new MissionEntity.EmoteResponseGroup()
    {
      m_triggers = new List<EmoteType>((IEnumerable<EmoteType>) MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS),
      m_responses = new List<MissionEntity.EmoteResponse>()
      {
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_Malchezaar_Male_Demon_PrologueEmoteResponse_01.prefab:86f1f2ae80c38214ead0c43ba589b03c",
          m_stringTag = "VO_Malchezaar_Male_Demon_PrologueEmoteResponse_01"
        }
      }
    }
  };

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override string GetVictoryScreenBannerText() => GameStrings.Get("GAMEPLAY_END_OF_GAME_VICTORY_MAYBE");

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    KAR00_Prologue kaR00Prologue = this;
    while (kaR00Prologue.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    if (missionEvent == 2)
    {
      GameState.Get().SetBusy(true);
      yield return (object) kaR00Prologue.PlayCriticalLine(actor, "VO_Medivh_Male_Human_PrologueWin_01.prefab:fa080d80cc356ac4587e802bbb856afb");
      yield return (object) new WaitForSeconds(1f);
      yield return (object) kaR00Prologue.PlayCriticalLine(enemyActor, "VO_Malchezaar_Male_Demon_PrologueWin_01.prefab:9d47723e51ef0fc469e66cb93ca6cba6");
      GameState.Get().SetBusy(false);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    KAR00_Prologue kaR00Prologue = this;
    while (kaR00Prologue.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    switch (turn)
    {
      case 1:
        GameState.Get().SetBusy(true);
        yield return (object) kaR00Prologue.PlayOpeningLine(actor, "VO_Malchezaar_Male_Demon_PrologueTurn1_04.prefab:335b8e61366fc9745b8b7cd0a61cc5c7");
        yield return (object) kaR00Prologue.PlayAdventureFlavorLine(friendlyActor, "VO_Medivh_Male_Human_PrologueTurn1_02.prefab:102f563763c38a249bdb734e6963ae37");
        GameState.Get().SetBusy(false);
        break;
      case 4:
        GameState.Get().SetBusy(true);
        yield return (object) kaR00Prologue.PlayAdventureFlavorLine(friendlyActor, "VO_Medivh_Male_Human_PrologueTurn3_02.prefab:da4713d2dea65a449be2485948df8f47");
        GameState.Get().SetBusy(false);
        break;
      case 6:
        GameState.Get().SetBusy(true);
        yield return (object) kaR00Prologue.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_PrologueTurn5_03.prefab:024bdb88f10206046a9a929fa350d4b9");
        GameState.Get().SetBusy(false);
        break;
      case 8:
        GameState.Get().SetBusy(true);
        yield return (object) kaR00Prologue.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_PrologueTurn7_01.prefab:9b60f6c47c2a6c5498fbba1a1c6ec35a");
        GameState.Get().SetBusy(false);
        break;
      case 10:
        GameState.Get().SetBusy(true);
        yield return (object) kaR00Prologue.PlayAdventureFlavorLine(friendlyActor, "VO_Medivh_Male_Human_PrologueTurn9_01.prefab:b92c01b534d9bc446999405722204a9e");
        yield return (object) kaR00Prologue.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_PrologueTurn9_02.prefab:3840159d3b8b5414e94ab693ede0c0d2");
        GameState.Get().SetBusy(false);
        break;
      case 12:
        GameState.Get().SetBusy(true);
        yield return (object) kaR00Prologue.PlayAdventureFlavorLine(actor, "VO_Malchezaar_Male_Demon_PrologueTurn11_02.prefab:fbd92ceb10f5ee94c9e2564aa6124df0");
        yield return (object) kaR00Prologue.PlayAdventureFlavorLine(friendlyActor, "VO_Medivh_Male_Human_PrologueTurn11_01.prefab:e350d5335927c4c429be030ed6c3bda1");
        yield return (object) kaR00Prologue.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_PrologueTurn11_01.prefab:a5383482754ffb646a13daf16819a892");
        GameState.Get().SetBusy(false);
        break;
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    KAR00_Prologue kaR00Prologue = this;
    while (kaR00Prologue.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR00Prologue.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR00Prologue.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
      if (cardId == "KARA_00_02")
      {
        GameState.Get().SetBusy(true);
        yield return (object) kaR00Prologue.PlayBossLine(actor, "VO_Malchezaar_Male_Demon_PrologueHeroPower_01.prefab:34d993460aa6bf249874ef52c64bb8c8");
        yield return (object) kaR00Prologue.PlayMissionFlavorLine(friendlyActor, "VO_Medivh_Male_Human_PrologueHeroPower_01.prefab:2a948f10bad7a4f4190c2e9b7d1f5e7c");
        GameState.Get().SetBusy(false);
      }
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    KAR00_Prologue kaR00Prologue = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) kaR00Prologue.PlayClosingLine("Moroes_Quote.prefab:ea3a21837aab2b0448ce4090103724cf", "VO_Moroes_Male_Human_PrologueWin_02.prefab:8d0d87405b3c8d5438a44faa1ed92d17");
    }
  }
}
