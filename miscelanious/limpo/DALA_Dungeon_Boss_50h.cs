using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_50h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_Death_01 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_Death_01.prefab:cd20248dc32c0ce40a12c5e1b35db06e");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_DefeatPlayer_01.prefab:6c501ac6d12542d4484cbc4e74b56263");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_EmoteResponse_01.prefab:899064816af908f40ad51b82ab08387e");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_01 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_01.prefab:7914715124540484aa92d9c27766874a");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_02 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_02.prefab:9a8d640a3bab2974ba783592e90abf9c");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_03 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_03.prefab:d1acebe4ad6e0dc4b95b783eead4b89a");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_HeroPower_02 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_HeroPower_02.prefab:f6d989f13b306c043b7631968b784d24");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_HeroPower_03 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_HeroPower_03.prefab:d77ca39562cf9c7429688fcb969eda08");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_HeroPower_09 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_HeroPower_09.prefab:6df64686110d4f44790a3852ffe6caa1");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_Idle_02 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_Idle_02.prefab:80e6c652c9243974588dc88b3569ab0d");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_Idle_03 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_Idle_03.prefab:3a5ea39ac902e0b4aa81496597b35b16");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_Idle_05 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_Idle_05.prefab:d7a38495b2a726f4f882998e0b146fca");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_Intro_01 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_Intro_01.prefab:9aa5fa82bcface24e85281074c104bc5");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_PlayerDragon_01 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_PlayerDragon_01.prefab:792291cfc262f524b85f6937b934ca0f");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_PlayerNozdormu_01 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_PlayerNozdormu_01.prefab:c64936961dd2cea4e80948a1c8ed42ee");
  private static readonly AssetReference VO_DALA_BOSS_50h_Female_Dragon_PlayerTimeWarp_01 = new AssetReference("VO_DALA_BOSS_50h_Female_Dragon_PlayerTimeWarp_01.prefab:437ac4f3cb4e35f4caa3a83aeaf0e046");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_Idle_02,
    (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_Idle_03,
    (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_Idle_05
  };
  private List<string> m_BossHeroicTrigger = new List<string>()
  {
    (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_01,
    (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_02,
    (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_Death_01,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_01,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_02,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroicTrigger_03,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroPower_02,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroPower_03,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroPower_09,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_Idle_02,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_Idle_03,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_Idle_05,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_Intro_01,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_PlayerDragon_01,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_PlayerNozdormu_01,
      (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_PlayerTimeWarp_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroPower_02,
    (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroPower_03,
    (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_HeroPower_09
  };

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_50h.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_EmoteResponse_01;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_50h dalaDungeonBoss50h = this;
    while (dalaDungeonBoss50h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss50h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_PlayerDragon_01);
        break;
      case 102:
        yield return (object) dalaDungeonBoss50h.PlayAndRemoveRandomLineOnlyOnce(actor, dalaDungeonBoss50h.m_BossHeroicTrigger);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss50h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_50h dalaDungeonBoss50h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss50h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss50h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss50h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss50h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss50h.m_playedLines.Add(cardId);
      if (!(cardId == "EX1_560"))
      {
        if (cardId == "UNG_028t")
          yield return (object) dalaDungeonBoss50h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_PlayerTimeWarp_01);
      }
      else
        yield return (object) dalaDungeonBoss50h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_50h.VO_DALA_BOSS_50h_Female_Dragon_PlayerNozdormu_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_50h dalaDungeonBoss50h = this;
    while (dalaDungeonBoss50h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss50h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss50h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss50h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss50h.m_playedLines.Add(cardId);
    }
  }
}
