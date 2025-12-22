using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_37h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_RandomLines = new List<string>()
  {
    "VO_GILA_BOSS_37h_Male_Murloc_EventPlayPlague_01.prefab:fb811185b39e06d499a8039905debc9e",
    "VO_GILA_BOSS_37h_Male_Murloc_EventPlayPlague_02.prefab:e050966b8d5bb954faaff70e4eac1c6c",
    "VO_GILA_BOSS_37h_Male_Murloc_EventPlayPlague_07.prefab:83a85909e80d4b1458b2a40ce4d1863f"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_37h_Male_Murloc_Intro_01.prefab:95c9eaeeb3d8f5d45a72fc8d0c9cc53c",
      "VO_GILA_BOSS_37h_Male_Murloc_EmoteResponse_01.prefab:a906592af0362154199937ff2e902c89",
      "VO_GILA_BOSS_37h_Male_Murloc_Death_01.prefab:43009ccd78aa83f4b95e8f92b2805996",
      "VO_GILA_BOSS_37h_Male_Murloc_HeroPower_01.prefab:ef813888de24ebf48ba25732b8ef4c2a",
      "VO_GILA_BOSS_37h_Male_Murloc_HeroPower_02.prefab:84e8fda79b594044f811488ceef4fc0d",
      "VO_GILA_BOSS_37h_Male_Murloc_HeroPower_03.prefab:66928b1220f93204eb04efb008f868f7",
      "VO_GILA_BOSS_37h_Male_Murloc_HeroPower_04.prefab:5f2d5528bf215734db16f77a1e13f4e5",
      "VO_GILA_BOSS_37h_Male_Murloc_EventPlayPlague_01.prefab:fb811185b39e06d499a8039905debc9e",
      "VO_GILA_BOSS_37h_Male_Murloc_EventPlayPlague_02.prefab:e050966b8d5bb954faaff70e4eac1c6c",
      "VO_GILA_BOSS_37h_Male_Murloc_EventPlayPlague_07.prefab:83a85909e80d4b1458b2a40ce4d1863f",
      "VO_GILA_BOSS_37h_Male_Murloc_EventPlayMurlocHolmes_02.prefab:b1295ac84416a8042a38440d7a6c9427"
    })
      this.PreloadSound(soundPath);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_37h_Male_Murloc_Intro_01.prefab:95c9eaeeb3d8f5d45a72fc8d0c9cc53c", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_37h_Male_Murloc_EmoteResponse_01.prefab:a906592af0362154199937ff2e902c89", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_37h_Male_Murloc_Death_01.prefab:43009ccd78aa83f4b95e8f92b2805996";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_37h gilDungeonBoss37h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) gilDungeonBoss37h.\u003C\u003En__0(entity);
    while (gilDungeonBoss37h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss37h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss37h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "GILA_BOSS_37t")
      {
        string line = gilDungeonBoss37h.PopRandomLineWithChance(gilDungeonBoss37h.m_RandomLines);
        if (line != null)
          yield return (object) gilDungeonBoss37h.PlayLineOnlyOnce(actor, line);
      }
    }
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_GILA_BOSS_37h_Male_Murloc_HeroPower_01.prefab:ef813888de24ebf48ba25732b8ef4c2a",
    "VO_GILA_BOSS_37h_Male_Murloc_HeroPower_02.prefab:84e8fda79b594044f811488ceef4fc0d",
    "VO_GILA_BOSS_37h_Male_Murloc_HeroPower_03.prefab:66928b1220f93204eb04efb008f868f7",
    "VO_GILA_BOSS_37h_Male_Murloc_HeroPower_04.prefab:5f2d5528bf215734db16f77a1e13f4e5"
  };

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_37h gilDungeonBoss37h = this;
    while (gilDungeonBoss37h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss37h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss37h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss37h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "GILA_827")
        yield return (object) gilDungeonBoss37h.PlayBossLine(actor, "VO_GILA_BOSS_37h_Male_Murloc_EventPlayMurlocHolmes_02.prefab:b1295ac84416a8042a38440d7a6c9427");
    }
  }
}
