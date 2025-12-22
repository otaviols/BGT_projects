using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAR01_Pantry : KAR_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_SilverwareGolem_Male_SilverwareGolem_SilverwareEmoteResponse_01.prefab:79a423cdcbf24144e886d12dba0a5422");
    this.PreloadSound("VO_SilverwareGolem_Male_SilverwareGolem_SilverwareKnifeJuggler_01.prefab:0d1779295cc055244982e057e6656dd0");
    this.PreloadSound("VO_Moroes_Male_Human_SilverwareResponse_01.prefab:f0b8b095a9b178d4d9acaf294c95a172");
    this.PreloadSound("VO_Moroes_Male_Human_SilverwareTurn3_02.prefab:be3de0e6492393345a175bb487db70a1");
    this.PreloadSound("VO_SilverwareGolem_Male_SilverwareGolem_SilverwareForkedLightning_01.prefab:dff5ceda9dcf03741a2444f78f0f0e23");
    this.PreloadSound("VO_Moroes_Male_Human_MedivhSkinResponse_01.prefab:74cd0ae7e1f7b9c4ca755b74c156406d");
    this.PreloadSound("VO_Moroes_Male_Human_SilverwareWin_01.prefab:123fc6f36a45c6e468dcd3faeb80a109");
    this.PreloadSound("VO_SilverwareGolem_Male_SilverwareGolem_SilverwarePlateTossing_01.prefab:f2ff07a0e24c0ea4cbda1e154de5462b");
    this.PreloadSound("VO_SilverwareGolem_Male_SilverwareGolem_SilverwareHeroPower_01.prefab:1ae8be96b941b384aabb4487fc24fecb");
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
          m_soundName = "VO_SilverwareGolem_Male_SilverwareGolem_SilverwareEmoteResponse_01.prefab:79a423cdcbf24144e886d12dba0a5422",
          m_stringTag = "VO_SilverwareGolem_Male_SilverwareGolem_SilverwareEmoteResponse_01"
        }
      }
    }
  };

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    KAR01_Pantry kaR01Pantry = this;
    while (kaR01Pantry.m_enemySpeaking)
      yield return (object) null;
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!kaR01Pantry.m_playedLines.Contains(str))
    {
      kaR01Pantry.m_playedLines.Add(str);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      switch (missionEvent)
      {
        case 1:
          yield return (object) kaR01Pantry.PlayEasterEggLine(actor, "VO_SilverwareGolem_Male_SilverwareGolem_SilverwareKnifeJuggler_01.prefab:0d1779295cc055244982e057e6656dd0");
          break;
        case 2:
          yield return (object) kaR01Pantry.PlayMissionFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_SilverwareResponse_01.prefab:f0b8b095a9b178d4d9acaf294c95a172");
          break;
      }
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    KAR01_Pantry kaR01Pantry = this;
    while (kaR01Pantry.m_enemySpeaking)
      yield return (object) null;
    if (turn == 1)
      yield return (object) kaR01Pantry.PlayOpeningLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_SilverwareTurn3_02.prefab:be3de0e6492393345a175bb487db70a1");
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    KAR01_Pantry kaR01Pantry = this;
    while (kaR01Pantry.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR01Pantry.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR01Pantry.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_251"))
      {
        if (cardId == "CS2_034_H1")
          yield return (object) kaR01Pantry.PlayEasterEggLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_MedivhSkinResponse_01.prefab:74cd0ae7e1f7b9c4ca755b74c156406d");
      }
      else
        yield return (object) kaR01Pantry.PlayEasterEggLine(actor, "VO_SilverwareGolem_Male_SilverwareGolem_SilverwareForkedLightning_01.prefab:dff5ceda9dcf03741a2444f78f0f0e23");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    KAR01_Pantry kaR01Pantry = this;
    while (kaR01Pantry.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR01Pantry.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR01Pantry.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "KAR_A02_13") && !(cardId == "KAR_A02_13H"))
      {
        if (cardId == "KAR_A02_11")
          yield return (object) kaR01Pantry.PlayBossLine(actor, "VO_SilverwareGolem_Male_SilverwareGolem_SilverwarePlateTossing_01.prefab:f2ff07a0e24c0ea4cbda1e154de5462b");
      }
      else
        yield return (object) kaR01Pantry.PlayBossLine(actor, "VO_SilverwareGolem_Male_SilverwareGolem_SilverwareHeroPower_01.prefab:1ae8be96b941b384aabb4487fc24fecb");
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    KAR01_Pantry kaR01Pantry = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) kaR01Pantry.PlayClosingLine("Moroes_Quote.prefab:ea3a21837aab2b0448ce4090103724cf", "VO_Moroes_Male_Human_SilverwareWin_01.prefab:123fc6f36a45c6e468dcd3faeb80a109");
    }
  }
}
