using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAR02_Mirror : KAR_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_Mirror_Male_Mirror_MirrorEmoteResponse_01.prefab:11f1ede615326154fab38c0bc6a55b90");
    this.PreloadSound("VO_Mirror_Male_Mirror_MirrorWellPlayedResponse_01.prefab:5901ea5faab95e74aa79d4c5354d3cfe");
    this.PreloadSound("VO_Mirror_Male_Mirror_MirrorFirstCard_01.prefab:abb11971ce998394aab0bb5e4e9eee4a");
    this.PreloadSound("VO_Mirror_Male_Mirror_MirrorTurn1_01.prefab:b444a1efe9fa7ac4da92cc232f803abe");
    this.PreloadSound("VO_Mirror_Male_Mirror_MirrorTurn3_01.prefab:f0273f9553383c04fbe95034480cef93");
    this.PreloadSound("VO_Mirror_Male_Mirror_MirrorTurn3_03.prefab:56d9324d1a978c74ab39708c909dd16f");
    this.PreloadSound("VO_Mirror_Male_Mirror_MirrorTurn5_02.prefab:2d8f0ddf302831d4a9c0b5e815652981");
    this.PreloadSound("VO_Mirror_Male_Mirror_MirrorMirrorImage_01.prefab:3ff331329b643284ca06eb7a3fa0001d");
    this.PreloadSound("VO_Mirror_Male_Mirror_MirrorMedivhSkin_01.prefab:c9a9fce27cb32be46a0c6486d57bcdaf");
    this.PreloadSound("VO_Moroes_Male_Human_MirrorTurn5_01.prefab:5b9d0bea3bbe2df43a36cf4072a20586");
    this.PreloadSound("VO_Moroes_Male_Human_MirrorWin_02.prefab:ab4a3ef74dc68ec42b1d0538ce1caf14");
    this.PreloadSound("VO_Moroes_Male_Human_MirrorTurn3_01.prefab:b1dcc6a301543a04d91b532d9640255a");
  }

  protected override void InitEmoteResponses()
  {
    List<EmoteType> emoteTypeList = new List<EmoteType>((IEnumerable<EmoteType>) MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS);
    emoteTypeList.Remove(EmoteType.WELL_PLAYED);
    this.m_emoteResponseGroups = new List<MissionEntity.EmoteResponseGroup>()
    {
      new MissionEntity.EmoteResponseGroup()
      {
        m_triggers = emoteTypeList,
        m_responses = new List<MissionEntity.EmoteResponse>()
        {
          new MissionEntity.EmoteResponse()
          {
            m_soundName = "VO_Mirror_Male_Mirror_MirrorEmoteResponse_01.prefab:11f1ede615326154fab38c0bc6a55b90",
            m_stringTag = "VO_Mirror_Male_Mirror_MirrorEmoteResponse_01"
          }
        }
      },
      new MissionEntity.EmoteResponseGroup()
      {
        m_triggers = new List<EmoteType>()
        {
          EmoteType.WELL_PLAYED
        },
        m_responses = new List<MissionEntity.EmoteResponse>()
        {
          new MissionEntity.EmoteResponse()
          {
            m_soundName = "VO_Mirror_Male_Mirror_MirrorWellPlayedResponse_01.prefab:5901ea5faab95e74aa79d4c5354d3cfe",
            m_stringTag = "VO_Mirror_Male_Mirror_MirrorWellPlayedResponse_01"
          }
        }
      }
    };
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    KAR02_Mirror kaR02Mirror = this;
    while (kaR02Mirror.m_enemySpeaking)
      yield return (object) null;
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!kaR02Mirror.m_playedLines.Contains(str))
    {
      kaR02Mirror.m_playedLines.Add(str);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      if (missionEvent == 1)
        yield return (object) kaR02Mirror.PlayMissionFlavorLine(actor, "VO_Mirror_Male_Mirror_MirrorFirstCard_01.prefab:abb11971ce998394aab0bb5e4e9eee4a");
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    KAR02_Mirror kaR02Mirror = this;
    while (kaR02Mirror.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) kaR02Mirror.PlayOpeningLine(enemyActor, "VO_Mirror_Male_Mirror_MirrorTurn1_01.prefab:b444a1efe9fa7ac4da92cc232f803abe");
        break;
      case 6:
        GameState.Get().SetBusy(true);
        yield return (object) kaR02Mirror.PlayMissionFlavorLine(enemyActor, "VO_Mirror_Male_Mirror_MirrorTurn3_01.prefab:f0273f9553383c04fbe95034480cef93");
        yield return (object) kaR02Mirror.PlayMissionFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_MirrorTurn3_01.prefab:b1dcc6a301543a04d91b532d9640255a");
        yield return (object) kaR02Mirror.PlayMissionFlavorLine(enemyActor, "VO_Mirror_Male_Mirror_MirrorTurn3_03.prefab:56d9324d1a978c74ab39708c909dd16f");
        GameState.Get().SetBusy(false);
        break;
      case 10:
        GameState.Get().SetBusy(true);
        yield return (object) kaR02Mirror.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_MirrorTurn5_01.prefab:5b9d0bea3bbe2df43a36cf4072a20586");
        yield return (object) kaR02Mirror.PlayAdventureFlavorLine(enemyActor, "VO_Mirror_Male_Mirror_MirrorTurn5_02.prefab:2d8f0ddf302831d4a9c0b5e815652981");
        GameState.Get().SetBusy(false);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    KAR02_Mirror kaR02Mirror = this;
    while (kaR02Mirror.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR02Mirror.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR02Mirror.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CS2_027"))
      {
        if (cardId == "CS2_034_H1")
          yield return (object) kaR02Mirror.PlayEasterEggLine(actor, "VO_Mirror_Male_Mirror_MirrorMedivhSkin_01.prefab:c9a9fce27cb32be46a0c6486d57bcdaf");
      }
      else
        yield return (object) kaR02Mirror.PlayEasterEggLine(actor, "VO_Mirror_Male_Mirror_MirrorMirrorImage_01.prefab:3ff331329b643284ca06eb7a3fa0001d");
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    KAR02_Mirror kaR02Mirror = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) kaR02Mirror.PlayClosingLine("Moroes_Quote.prefab:ea3a21837aab2b0448ce4090103724cf", "VO_Moroes_Male_Human_MirrorWin_02.prefab:ab4a3ef74dc68ec42b1d0538ce1caf14");
    }
  }
}
