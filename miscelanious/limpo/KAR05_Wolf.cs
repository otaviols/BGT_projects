using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAR05_Wolf : KAR_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_Barnes_Male_Human_WolfBigMinion_01.prefab:22f80cdb7ab6b6f44a643994882fb42a");
    this.PreloadSound("VO_Barnes_Male_Human_WolfClaws_01.prefab:d1159c6b78ace3349a5040e46f92f7e4");
    this.PreloadSound("VO_Barnes_Male_Human_WolfTurn5_01.prefab:787048fde1485714fbdb2623e81ffcff");
    this.PreloadSound("VO_Barnes_Male_Human_WolfTurn9_01.prefab:9c2c7d0c1b1556849ba41e5a2d80a273");
    this.PreloadSound("VO_Barnes_Male_Human_WolfWin_01.prefab:5ea959aed7f1c3a4aa0389317a147030");
    this.PreloadSound("VO_BigBadWolf_Male_Worgen_WolfBigMinion_01.prefab:266cb01ccfe8a73449c10b141d69523c");
    this.PreloadSound("VO_BigBadWolf_Male_Worgen_WolfTurn1_01.prefab:532a2edae8723cb40a189f63a7d5af1e");
    this.PreloadSound("VO_BigBadWolf_Male_Worgen_WolfEmoteResponse_01.prefab:73c2f52a396fb5b498867c8d8e0b4a0a");
    this.PreloadSound("VO_BigBadWolf_Male_Worgen_WolfDireWolfAlpha_01.prefab:d4457c645514a6b49be8345218d13cf6");
    this.PreloadSound("VO_BigBadWolf_Male_Worgen_WolfDireWolfAlpha_02.prefab:af8f1f0f982c1e74789a3e358ec32f9e");
    this.PreloadSound("VO_BigBadWolf_Male_Worgen_WolfScarletCrusader_01.prefab:dcdcc54bc1a75374baf3a5237d0a7141");
    this.PreloadSound("VO_Moroes_Male_Human_WolfClaws_03.prefab:91c86f34e4a146f488899a60f8e4490b");
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
          m_soundName = "VO_BigBadWolf_Male_Worgen_WolfEmoteResponse_01.prefab:73c2f52a396fb5b498867c8d8e0b4a0a",
          m_stringTag = "VO_BigBadWolf_Male_Worgen_WolfEmoteResponse_01"
        }
      }
    }
  };

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    KAR05_Wolf kaR05Wolf = this;
    while (kaR05Wolf.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 1:
        GameState.Get().SetBusy(true);
        if (kaR05Wolf.ShouldPlayMissionFlavorLine("VO_Barnes_Male_Human_WolfBigMinion_01.prefab:22f80cdb7ab6b6f44a643994882fb42a"))
          yield return (object) new WaitForSeconds(0.8f);
        yield return (object) kaR05Wolf.PlayMissionFlavorLine("Barnes_BigQuote.prefab:15c396b2577ab09449f3721d23da3dba", "VO_Barnes_Male_Human_WolfBigMinion_01.prefab:22f80cdb7ab6b6f44a643994882fb42a");
        yield return (object) kaR05Wolf.PlayMissionFlavorLine(enemyActor, "VO_BigBadWolf_Male_Worgen_WolfBigMinion_01.prefab:266cb01ccfe8a73449c10b141d69523c");
        GameState.Get().SetBusy(false);
        break;
      case 2:
        GameState.Get().SetBusy(true);
        yield return (object) kaR05Wolf.PlayMissionFlavorLine(enemyActor, "VO_BigBadWolf_Male_Worgen_WolfTurn1_01.prefab:532a2edae8723cb40a189f63a7d5af1e");
        GameState.Get().SetBusy(false);
        break;
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    KAR05_Wolf kaR05Wolf = this;
    while (kaR05Wolf.m_enemySpeaking)
      yield return (object) null;
    switch (turn)
    {
      case 1:
        GameState.Get().SetBusy(true);
        yield return (object) kaR05Wolf.PlayOpeningLine("Barnes_BigQuote.prefab:15c396b2577ab09449f3721d23da3dba", "VO_Barnes_Male_Human_WolfTurn5_01.prefab:787048fde1485714fbdb2623e81ffcff");
        GameState.Get().SetBusy(false);
        break;
      case 10:
        GameState.Get().SetBusy(true);
        yield return (object) kaR05Wolf.PlayAdventureFlavorLine("Barnes_BigQuote.prefab:15c396b2577ab09449f3721d23da3dba", "VO_Barnes_Male_Human_WolfTurn9_01.prefab:9c2c7d0c1b1556849ba41e5a2d80a273");
        GameState.Get().SetBusy(false);
        break;
    }
  }

  private Actor GetDireWolf()
  {
    Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
    foreach (Card card in friendlySidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == friendlySidePlayer.GetPlayerId() && entity.GetCardId() == "EX1_162")
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    KAR05_Wolf kaR05Wolf = this;
    while (kaR05Wolf.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    string str = "ENEMY_" + entity.GetCardId();
    if (!kaR05Wolf.m_playedLines.Contains(str))
    {
      kaR05Wolf.m_playedLines.Add(str);
      if (str == "KARA_05_02")
      {
        GameState.Get().SetBusy(true);
        if (kaR05Wolf.ShouldPlayBossLine("VO_Barnes_Male_Human_WolfClaws_01.prefab:d1159c6b78ace3349a5040e46f92f7e4"))
        {
          yield return (object) new WaitForSeconds(3f);
          yield return (object) kaR05Wolf.PlayMissionFlavorLine("Barnes_BigQuote.prefab:15c396b2577ab09449f3721d23da3dba", "VO_Barnes_Male_Human_WolfClaws_01.prefab:d1159c6b78ace3349a5040e46f92f7e4");
        }
        GameState.Get().SetBusy(false);
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    KAR05_Wolf kaR05Wolf = this;
    while (kaR05Wolf.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR05Wolf.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR05Wolf.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor direWolf;
      if (!(cardId == "EX1_162"))
      {
        if (cardId == "EX1_020")
        {
          GameState.Get().SetBusy(true);
          yield return (object) kaR05Wolf.PlayEasterEggLine(actor, "VO_BigBadWolf_Male_Worgen_WolfScarletCrusader_01.prefab:dcdcc54bc1a75374baf3a5237d0a7141");
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        direWolf = kaR05Wolf.GetDireWolf();
        if ((Object) direWolf != (Object) null)
        {
          GameState.Get().SetBusy(true);
          yield return (object) kaR05Wolf.PlayEasterEggLine(actor, "VO_BigBadWolf_Male_Worgen_WolfDireWolfAlpha_01.prefab:d4457c645514a6b49be8345218d13cf6");
          yield return (object) kaR05Wolf.PlayEasterEggLine(direWolf, "VO_BigBadWolf_Male_Worgen_WolfDireWolfAlpha_02.prefab:af8f1f0f982c1e74789a3e358ec32f9e");
          GameState.Get().SetBusy(false);
        }
      }
      direWolf = (Actor) null;
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    KAR05_Wolf kaR05Wolf = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) kaR05Wolf.PlayClosingLine("Barnes_Quote.prefab:2e7e9f28b5bc37149a12b2e5feaa244a", "VO_Barnes_Male_Human_WolfWin_01.prefab:5ea959aed7f1c3a4aa0389317a147030");
    }
  }
}
