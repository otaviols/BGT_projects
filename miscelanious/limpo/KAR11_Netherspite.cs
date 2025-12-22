using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAR11_Netherspite : KAR_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_Netherspite_Male_Dragon_NetherspiteEmoteResponse_01.prefab:f0a08435dd8aedb4b9d4b7f8b27a4d4f");
    this.PreloadSound("VO_Netherspite_Male_Dragon_NetherspiteTurn3_02.prefab:909c9170498e2ff458bb5e607ae35fe1");
    this.PreloadSound("VO_Netherspite_Male_Dragon_NetherspiteTurn5_01.prefab:5c5e1d24755bfe34f825ce62f9deb6fa");
    this.PreloadSound("VO_Netherspite_Male_Dragon_NetherspiteTurn7_01.prefab:714c8bbda55d73b4a96f6c08ad3f2372");
    this.PreloadSound("VO_Netherspite_Male_Dragon_NetherspiteEmpowerment_01.prefab:066618c460879fc4f95694560aede66a");
    this.PreloadSound("VO_Netherspite_Male_Dragon_NetherspiteShadowBreath_01.prefab:3ec25ccdaa47c06428c1e4119b00a6e0");
    this.PreloadSound("VO_Netherspite_Male_Dragon_NetherspiteUnstablePortal_02.prefab:59a7527262c5e2d45b25672b8a2150f8");
    this.PreloadSound("VO_Netherspite_Male_Dragon_NetherspiteAngryChicken_01.prefab:4364929242fd7864eb2f681df1ab4f9e");
    this.PreloadSound("VO_Moroes_Male_Human_NetherspiteWin_01.prefab:012fd06d66e4106409d1cc9179f3a25b");
    this.PreloadSound("VO_Moroes_Male_Human_NetherspiteTurn1_01.prefab:f5a7ea32cbace6448ba0c29b44018bbb");
    this.PreloadSound("VO_Moroes_Male_Human_NetherspiteTurn7_01.prefab:4ab83e21ea834994498395f0678806ea");
    this.PreloadSound("VO_Moroes_Male_Human_NetherspiteTurn5_01.prefab:19d16638e6ae56f4b8c915d28b0b882f");
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
          m_soundName = "VO_Netherspite_Male_Dragon_NetherspiteEmoteResponse_01.prefab:f0a08435dd8aedb4b9d4b7f8b27a4d4f",
          m_stringTag = "VO_Netherspite_Male_Dragon_NetherspiteEmoteResponse_01"
        }
      }
    }
  };

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    KAR11_Netherspite kaR11Netherspite = this;
    while (kaR11Netherspite.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) kaR11Netherspite.PlayOpeningLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_NetherspiteTurn1_01.prefab:f5a7ea32cbace6448ba0c29b44018bbb");
        break;
      case 6:
        GameState.Get().SetBusy(true);
        if (kaR11Netherspite.ShouldPlayMissionFlavorLine("VO_Netherspite_Male_Dragon_NetherspiteTurn5_01.prefab:5c5e1d24755bfe34f825ce62f9deb6fa"))
        {
          yield return (object) kaR11Netherspite.PlayMissionFlavorLine(actor, "VO_Netherspite_Male_Dragon_NetherspiteTurn5_01.prefab:5c5e1d24755bfe34f825ce62f9deb6fa");
          yield return (object) kaR11Netherspite.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_NetherspiteTurn5_01.prefab:19d16638e6ae56f4b8c915d28b0b882f");
        }
        GameState.Get().SetBusy(false);
        break;
      case 10:
        GameState.Get().SetBusy(true);
        yield return (object) kaR11Netherspite.PlayAdventureFlavorLine(actor, "VO_Netherspite_Male_Dragon_NetherspiteTurn7_01.prefab:714c8bbda55d73b4a96f6c08ad3f2372");
        yield return (object) kaR11Netherspite.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_NetherspiteTurn7_01.prefab:4ab83e21ea834994498395f0678806ea");
        GameState.Get().SetBusy(false);
        break;
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    KAR11_Netherspite kaR11Netherspite = this;
    while (kaR11Netherspite.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR11Netherspite.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR11Netherspite.m_playedLines.Add(cardId);
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "KARA_08_02") && !(cardId == "KARA_08_02H"))
      {
        if (cardId == "KARA_08_05" || cardId == "KARA_08_05H")
          yield return (object) kaR11Netherspite.PlayBossLine(enemyActor, "VO_Netherspite_Male_Dragon_NetherspiteShadowBreath_01.prefab:3ec25ccdaa47c06428c1e4119b00a6e0");
      }
      else
      {
        yield return (object) new WaitForSeconds(0.2f);
        GameState.Get().SetBusy(true);
        yield return (object) kaR11Netherspite.PlayMissionFlavorLine(enemyActor, "VO_Netherspite_Male_Dragon_NetherspiteEmpowerment_01.prefab:066618c460879fc4f95694560aede66a");
        GameState.Get().SetBusy(false);
      }
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    KAR11_Netherspite kaR11Netherspite = this;
    while (kaR11Netherspite.m_enemySpeaking)
      yield return (object) null;
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!kaR11Netherspite.m_playedLines.Contains(str))
    {
      kaR11Netherspite.m_playedLines.Add(str);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (missionEvent == 1)
      {
        GameState.Get().SetBusy(true);
        yield return (object) kaR11Netherspite.PlayMissionFlavorLine(actor, "VO_Netherspite_Male_Dragon_NetherspiteTurn3_02.prefab:909c9170498e2ff458bb5e607ae35fe1");
        GameState.Get().SetBusy(false);
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    KAR11_Netherspite kaR11Netherspite = this;
    while (kaR11Netherspite.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR11Netherspite.m_playedLines.Contains(entity.GetCardId()))
    {
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      string cardId = entity.GetCardId();
      kaR11Netherspite.m_playedLines.Add(cardId);
      if (!(cardId == "GVG_003"))
      {
        if (cardId == "EX1_009")
          yield return (object) kaR11Netherspite.PlayBossLine(actor, "VO_Netherspite_Male_Dragon_NetherspiteAngryChicken_01.prefab:4364929242fd7864eb2f681df1ab4f9e");
      }
      else
        yield return (object) kaR11Netherspite.PlayBossLine(actor, "VO_Netherspite_Male_Dragon_NetherspiteUnstablePortal_02.prefab:59a7527262c5e2d45b25672b8a2150f8");
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    KAR11_Netherspite kaR11Netherspite = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) kaR11Netherspite.PlayClosingLine("Moroes_Quote.prefab:ea3a21837aab2b0448ce4090103724cf", "VO_Moroes_Male_Human_NetherspiteWin_01.prefab:012fd06d66e4106409d1cc9179f3a25b");
    }
  }
}
