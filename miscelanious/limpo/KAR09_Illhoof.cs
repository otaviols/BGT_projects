using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAR09_Illhoof : KAR_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_Illhoof_Male_Demon_IllhoofSummonImps_01.prefab:cc3a062bf710c454ca87743d4bccc7d6");
    this.PreloadSound("VO_Illhoof_Male_Demon_IllhoofSummoningPortal_01.prefab:36097ec5b99abda439a1606c2270fced");
    this.PreloadSound("VO_Illhoof_Male_Demon_IllhoofEmoteResponse_01.prefab:c3681690f22db464d8e796bf98a02d57");
    this.PreloadSound("VO_Illhoof_Male_Demon_IllhoofWounded_01.prefab:b46f0340df9465e4f840d1303fc3b940");
    this.PreloadSound("VO_Illhoof_Male_Demon_IllhoofTurn1_01.prefab:bb152da0f208c2342baa7ea5bf44e68d");
    this.PreloadSound("VO_Illhoof_Male_Demon_IlhoofKilrek_01.prefab:57aeb9e4838443e4b9e25968c7db9045");
    this.PreloadSound("VO_Moroes_Male_Human_IllhoofKilrekResponse_01.prefab:dc74ecda46619da4abe6e30c0b555e12");
    this.PreloadSound("VO_Curator_Male_ArcaneGolem_IllhoofKilrekResponse_01.prefab:d0dc54e5fd4f0ca41a54a9f2d7e56a03");
    this.PreloadSound("VO_Curator_Male_ArcaneGolem_IllhoofSenseDemons_01.prefab:0f0c06c276d2a9443933b7ce3daa39a2");
    this.PreloadSound("VO_Curator_Male_ArcaneGolem_IllhoofWin_01.prefab:c088ea75a2aba3844b4a5676e5eac371");
    this.PreloadSound("VO_Curator_Male_ArcaneGolem_IllhoofTurn1_01.prefab:b4039f0c7dade924ba570485545f46cd");
    this.PreloadSound("VO_Curator_Male_ArcaneGolem_IllhoofTurn5_01.prefab:3f292bf97725dce4e9aa224975fc1ba0");
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
          m_soundName = "VO_Illhoof_Male_Demon_IllhoofEmoteResponse_01.prefab:c3681690f22db464d8e796bf98a02d57",
          m_stringTag = "VO_Illhoof_Male_Demon_IllhoofEmoteResponse_01"
        }
      }
    }
  };

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    KAR09_Illhoof kaR09Illhoof = this;
    while (kaR09Illhoof.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!kaR09Illhoof.m_playedLines.Contains(str))
    {
      kaR09Illhoof.m_playedLines.Add(str);
      if (missionEvent == 1)
      {
        GameState.Get().SetBusy(true);
        yield return (object) kaR09Illhoof.PlayMissionFlavorLine(actor, "VO_Illhoof_Male_Demon_IllhoofWounded_01.prefab:b46f0340df9465e4f840d1303fc3b940");
        GameState.Get().SetBusy(false);
      }
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    KAR09_Illhoof kaR09Illhoof = this;
    while (kaR09Illhoof.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (turn == 1 && kaR09Illhoof.ShouldPlayOpeningLine("VO_Illhoof_Male_Demon_IllhoofTurn1_01.prefab:bb152da0f208c2342baa7ea5bf44e68d"))
    {
      yield return (object) kaR09Illhoof.PlayOpeningLine(actor, "VO_Illhoof_Male_Demon_IllhoofTurn1_01.prefab:bb152da0f208c2342baa7ea5bf44e68d");
      yield return (object) kaR09Illhoof.PlayMissionFlavorLine("Curator_BigQuote.prefab:f01875528133988418925bd870aa7b81", "VO_Curator_Male_ArcaneGolem_IllhoofTurn1_01.prefab:b4039f0c7dade924ba570485545f46cd");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    KAR09_Illhoof kaR09Illhoof = this;
    while (kaR09Illhoof.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR09Illhoof.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR09Illhoof.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "KARA_09_05") && !(cardId == "KARA_09_05heroic"))
      {
        if (cardId == "KARA_09_03" || cardId == "KARA_09_03heroic")
          yield return (object) kaR09Illhoof.PlayBossLine(actor, "VO_Illhoof_Male_Demon_IllhoofSummonImps_01.prefab:cc3a062bf710c454ca87743d4bccc7d6");
      }
      else if (kaR09Illhoof.ShouldPlayBossLine("VO_Illhoof_Male_Demon_IlhoofKilrek_01.prefab:57aeb9e4838443e4b9e25968c7db9045"))
      {
        GameState.Get().SetBusy(true);
        yield return (object) kaR09Illhoof.PlayBossLine(actor, "VO_Illhoof_Male_Demon_IlhoofKilrek_01.prefab:57aeb9e4838443e4b9e25968c7db9045");
        yield return (object) kaR09Illhoof.PlayMissionFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_IllhoofKilrekResponse_01.prefab:dc74ecda46619da4abe6e30c0b555e12");
        yield return (object) kaR09Illhoof.PlayMissionFlavorLine("Curator_BigQuote.prefab:f01875528133988418925bd870aa7b81", "VO_Curator_Male_ArcaneGolem_IllhoofKilrekResponse_01.prefab:d0dc54e5fd4f0ca41a54a9f2d7e56a03");
        GameState.Get().SetBusy(false);
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    KAR09_Illhoof kaR09Illhoof = this;
    while (kaR09Illhoof.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR09Illhoof.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR09Illhoof.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_315"))
      {
        if (cardId == "EX1_317")
          yield return (object) kaR09Illhoof.PlayEasterEggLine("Curator_BigQuote.prefab:f01875528133988418925bd870aa7b81", "VO_Curator_Male_ArcaneGolem_IllhoofSenseDemons_01.prefab:0f0c06c276d2a9443933b7ce3daa39a2");
      }
      else
        yield return (object) kaR09Illhoof.PlayEasterEggLine(actor, "VO_Illhoof_Male_Demon_IllhoofSummoningPortal_01.prefab:36097ec5b99abda439a1606c2270fced");
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    KAR09_Illhoof kaR09Illhoof = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) kaR09Illhoof.PlayClosingLine("Curator_Quote.prefab:ab58be80382875e4cbaa766fda73cd39", "VO_Curator_Male_ArcaneGolem_IllhoofWin_01.prefab:c088ea75a2aba3844b4a5676e5eac371");
    }
  }
}
