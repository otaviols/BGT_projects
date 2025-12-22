using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KAR04_Julianne : KAR_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_Julianne_Female_Human_JulianneHeroPower_01.prefab:52cf3ed754f5ae647a1fb2a27ae8e37d");
    this.PreloadSound("VO_Julianne_Female_Human_JulianneEmoteResponse_01.prefab:803a3576d6dd0a74fa6da433b25d638b");
    this.PreloadSound("VO_KARA_06_01_Male_Human_JulianneTurn1_01.prefab:2c91233e10b180441b1d8bf1a834e53a");
    this.PreloadSound("VO_Moroes_Male_Human_JulianneTurn5_01.prefab:d2e8c0e588e0cb045b1ad62cf02ac17f");
    this.PreloadSound("VO_Moroes_Male_Human_JulianneTurn9_02.prefab:bd1443ae4c72fb445a1fd9f558e9640e");
    this.PreloadSound("VO_Barnes_Male_Human_JulianneTurn5_01.prefab:614340cf7864460478f0984d527b5bba");
    this.PreloadSound("VO_KARA_06_01_Male_Human_JulianneDeadlyPoison_02.prefab:4b33353d9ef6009418520b1173a285e7");
    this.PreloadSound("VO_Julianne_Female_Human_JulianneFeignDeath_03.prefab:e28f8db1c88f12f4bba962b65e9ed936");
    this.PreloadSound("VO_Barnes_Male_Human_JulianneWin_01.prefab:09d4c4aaf43ac634aaf325c2badc72a8");
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
          m_soundName = "VO_Julianne_Female_Human_JulianneEmoteResponse_01.prefab:803a3576d6dd0a74fa6da433b25d638b",
          m_stringTag = "VO_Julianne_Female_Human_JulianneEmoteResponse_01"
        }
      }
    }
  };

  private Actor GetRomulo()
  {
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    foreach (Card card in opposingSidePlayer.GetBattlefieldZone().GetCards())
    {
      Entity entity = card.GetEntity();
      if (entity.GetControllerId() == opposingSidePlayer.GetPlayerId() && (entity.GetCardId() == "KARA_06_01" || entity.GetCardId() == "KARA_06_01heroic"))
        return entity.GetCard().GetActor();
    }
    return (Actor) null;
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    KAR04_Julianne kaR04Julianne = this;
    while (kaR04Julianne.m_enemySpeaking)
      yield return (object) null;
    switch (turn)
    {
      case 1:
        Actor romulo = kaR04Julianne.GetRomulo();
        if (!((Object) romulo != (Object) null))
          break;
        GameState.Get().SetBusy(true);
        yield return (object) kaR04Julianne.PlayOpeningLine(romulo, "VO_KARA_06_01_Male_Human_JulianneTurn1_01.prefab:2c91233e10b180441b1d8bf1a834e53a");
        GameState.Get().SetBusy(false);
        break;
      case 6:
        GameState.Get().SetBusy(true);
        yield return (object) kaR04Julianne.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_JulianneTurn5_01.prefab:d2e8c0e588e0cb045b1ad62cf02ac17f");
        yield return (object) kaR04Julianne.PlayAdventureFlavorLine("Barnes_BigQuote.prefab:15c396b2577ab09449f3721d23da3dba", "VO_Barnes_Male_Human_JulianneTurn5_01.prefab:614340cf7864460478f0984d527b5bba");
        GameState.Get().SetBusy(false);
        break;
      case 10:
        GameState.Get().SetBusy(true);
        yield return (object) kaR04Julianne.PlayAdventureFlavorLine("Moroes_BigQuote.prefab:321274c1b67d79a4ba421a965bbc9e6d", "VO_Moroes_Male_Human_JulianneTurn9_02.prefab:bd1443ae4c72fb445a1fd9f558e9640e");
        GameState.Get().SetBusy(false);
        break;
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    KAR04_Julianne kaR04Julianne = this;
    while (kaR04Julianne.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (missionEvent == 1)
    {
      GameState.Get().SetBusy(true);
      yield return (object) kaR04Julianne.PlayBossLine(actor, "VO_Julianne_Female_Human_JulianneHeroPower_01.prefab:52cf3ed754f5ae647a1fb2a27ae8e37d");
      GameState.Get().SetBusy(false);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    KAR04_Julianne kaR04Julianne = this;
    while (kaR04Julianne.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!kaR04Julianne.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      kaR04Julianne.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CS2_074"))
      {
        if (cardId == "GVG_026")
        {
          GameState.Get().SetBusy(true);
          yield return (object) kaR04Julianne.PlayEasterEggLine(actor, "VO_Julianne_Female_Human_JulianneFeignDeath_03.prefab:e28f8db1c88f12f4bba962b65e9ed936");
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        Actor romulo = kaR04Julianne.GetRomulo();
        if ((Object) romulo != (Object) null)
        {
          GameState.Get().SetBusy(true);
          yield return (object) kaR04Julianne.PlayEasterEggLine(romulo, "VO_KARA_06_01_Male_Human_JulianneDeadlyPoison_02.prefab:4b33353d9ef6009418520b1173a285e7");
          GameState.Get().SetBusy(false);
        }
      }
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    KAR04_Julianne kaR04Julianne = this;
    if (gameResult == TAG_PLAYSTATE.WON)
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) kaR04Julianne.PlayClosingLine("Barnes_Quote.prefab:2e7e9f28b5bc37149a12b2e5feaa244a", "VO_Barnes_Male_Human_JulianneWin_01.prefab:09d4c4aaf43ac634aaf325c2badc72a8");
    }
  }
}
