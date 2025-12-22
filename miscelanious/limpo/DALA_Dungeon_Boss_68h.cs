using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_68h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_BossPortal_01 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_BossPortal_01.prefab:a873bb5e7365cdc4c915b4b650a01dbc");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_Death_01 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_Death_01.prefab:76c49765d2c812643bcb00d7a92d6112");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_DefeatPlayer_01.prefab:d39518f20db49a745a80013ad9b4139b");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_EmoteResponse_01.prefab:0ec964c2034842a479fa06e9e81ce122");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_02 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_02.prefab:9d9a010736697b44db997de8e1f6c930");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_03 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_03.prefab:6be21abc0ed74f443a19711cd2a3a1b4");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_04 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_04.prefab:586f7f44c7675b342b58b9a9d7055bd7");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_05 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_05.prefab:598761b5a02cc504da2f14a8deae30ff");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_07 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_07.prefab:6aae75afaf0b63f48b206406a3f6eda1");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_08 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_08.prefab:43908666d428c294aaf5469ac22883e8");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_Idle_01 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_Idle_01.prefab:c6f0ed7e5efa54b4ea0faeaed6907ce2");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_Idle_02 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_Idle_02.prefab:7ed2e1ee86dff144386d2a50733e8cef");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_Idle_03 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_Idle_03.prefab:149ce7a0eaaeb4c4391212f44f091749");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_Intro_01 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_Intro_01.prefab:02a6b55bd379de848aa4e2271fab4fe3");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_Player10CostMinion_01 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_Player10CostMinion_01.prefab:ac364d1574d7fb34ba153514c9ed047d");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_Player1CostMinion_01 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_Player1CostMinion_01.prefab:501bbfe0d99f0f544b74712260f9d650");
  private static readonly AssetReference VO_DALA_BOSS_68h_Female_BloodElf_PlayerPortal_01 = new AssetReference("VO_DALA_BOSS_68h_Female_BloodElf_PlayerPortal_01.prefab:73d103d2c3238064eaa756bc8e48f62e");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Idle_01,
    (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Idle_02,
    (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Idle_03
  };
  private static List<string> m_HeroPowerTrigger = new List<string>()
  {
    (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_02,
    (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_03,
    (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_04,
    (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_05,
    (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_07,
    (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_08
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_BossPortal_01,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Death_01,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_02,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_03,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_04,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_05,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_07,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_HeroPowerTrigger_08,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Idle_01,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Idle_02,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Idle_03,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Intro_01,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Player10CostMinion_01,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Player1CostMinion_01,
      (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_PlayerPortal_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_68h.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_EmoteResponse_01;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_68h dalaDungeonBoss68h = this;
    while (dalaDungeonBoss68h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss68h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Player10CostMinion_01);
        break;
      case 102:
        yield return (object) dalaDungeonBoss68h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_Player1CostMinion_01);
        break;
      case 103:
        yield return (object) dalaDungeonBoss68h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_68h.m_HeroPowerTrigger);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss68h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_68h dalaDungeonBoss68h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss68h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss68h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss68h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss68h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss68h.m_playedLines.Add(cardId);
      if (cardId == "GVG_003" || cardId == "KAR_073" || cardId == "KAR_075" || cardId == "KAR_076" || cardId == "KAR_077" || cardId == "KAR_091")
        yield return (object) dalaDungeonBoss68h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_PlayerPortal_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_68h dalaDungeonBoss68h = this;
    while (dalaDungeonBoss68h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss68h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss68h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss68h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "GVG_003" || cardId == "KAR_073" || cardId == "KAR_075" || cardId == "KAR_076" || cardId == "KAR_077" || cardId == "KAR_091")
        yield return (object) dalaDungeonBoss68h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_68h.VO_DALA_BOSS_68h_Female_BloodElf_BossPortal_01);
    }
  }
}
