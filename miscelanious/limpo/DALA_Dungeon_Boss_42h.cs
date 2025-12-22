using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_42h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_BossPogohopper_01 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_BossPogohopper_01.prefab:6c6e3e4336789c046b9e9535867cf85a");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_Death_02 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_Death_02.prefab:fdc9f12e07c9cfc44b3d4cb7d912c335");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_DefeatPlayer_01.prefab:a6d25ab360f39ad4e8b5c3530f7e13d2");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_EmoteResponse_01.prefab:c1e616b96cf8a7a4bb00a77f265a03a7");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_HeroPower_01 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_HeroPower_01.prefab:5fe596d383b8cae45951381d720b30e9");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_HeroPower_02 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_HeroPower_02.prefab:49d62834f322c774798fd93c64bd6e38");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_HeroPower_03 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_HeroPower_03.prefab:a76eea147c071324595a15a275518f47");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_Idle_01 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_Idle_01.prefab:96f5e3391e6fc2640a9da7208a69dbb5");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_Idle_02 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_Idle_02.prefab:47287fc18da1d0b41a072f8a619d3ad8");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_Intro_01 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_Intro_01.prefab:8d01c93bbe719f9439e4437aefd8a4de");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_PlayerPogohopper_01 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_PlayerPogohopper_01.prefab:3b32211ad7d75394aa45f1b72f7c3445");
  private static readonly AssetReference VO_DALA_BOSS_42h_Male_Mech_PlayerPogohopper2_01 = new AssetReference("VO_DALA_BOSS_42h_Male_Mech_PlayerPogohopper2_01.prefab:f210aca0e408f47479e8b816b2ba0ff1");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_Idle_01,
    (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_Idle_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_BossPogohopper_01,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_Death_02,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_HeroPower_01,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_HeroPower_02,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_HeroPower_03,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_Idle_01,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_Idle_02,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_Intro_01,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_PlayerPogohopper_01,
      (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_PlayerPogohopper2_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_HeroPower_01,
    (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_HeroPower_02,
    (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_HeroPower_03
  };

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_42h.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_Death_02;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_EmoteResponse_01;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_42h dalaDungeonBoss42h = this;
    while (dalaDungeonBoss42h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss42h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_PlayerPogohopper_01);
        break;
      case 102:
        yield return (object) dalaDungeonBoss42h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_PlayerPogohopper2_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss42h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_42h dalaDungeonBoss42h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss42h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss42h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss42h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss42h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss42h.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_42h dalaDungeonBoss42h = this;
    while (dalaDungeonBoss42h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss42h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss42h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss42h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss42h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "BOT_283")
        yield return (object) dalaDungeonBoss42h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_42h.VO_DALA_BOSS_42h_Male_Mech_BossPogohopper_01);
    }
  }
}
