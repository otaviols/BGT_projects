using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_73h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_BossWhirlwindTempest_01 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_BossWhirlwindTempest_01.prefab:466d89ef2c68efd40a75dd29520d9e0d");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_Death_01 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_Death_01.prefab:6c3f7866c2830d14c9e0720765f7be14");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_DefeatPlayer_01.prefab:d290b7b568e2d274eb443c34c852da8e");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_EmoteResponse_01.prefab:fccbfb7f0a8fb6b40a5c1d097c8cc2c4");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_HeroPower_01 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_HeroPower_01.prefab:3bfeab29fd637fc4cb2a0b03b6823c3a");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_HeroPower_02 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_HeroPower_02.prefab:b725ba947f2d85a45a380b691c43b42a");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_HeroPower_03 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_HeroPower_03.prefab:70585c8072ade0046852bb9b0ce3ffdf");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_HeroPower_04 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_HeroPower_04.prefab:d7fc0ae02711bce41a544cabf5b4388c");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_Idle_01 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_Idle_01.prefab:6bfaf6737324986409471b17ebcbad50");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_Idle_02 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_Idle_02.prefab:b6b64a9016f72bd46977288750bf94ec");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_Idle_04 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_Idle_04.prefab:98ad12a474b125a4386edab1e2a24981");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_Intro_01 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_Intro_01.prefab:1b3bc54107269e043bc16c5dfa84d3f2");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_PlayerShamanSpell_01 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_PlayerShamanSpell_01.prefab:6898248dc3f25de4e9019b2d25c02395");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_PlayerTotem_02 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_PlayerTotem_02.prefab:697c531ba6483354da24c1d3e5d542d4");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_PlayerVoltron_01 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_PlayerVoltron_01.prefab:ced4fb774876fff4a89684447fbfa74c");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_PlayerWindfury_01 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_PlayerWindfury_01.prefab:46239d226057db84880ecce10f7232f1");
  private static readonly AssetReference VO_DALA_BOSS_73h_Male_Tauren_PlayerWindfury_02 = new AssetReference("VO_DALA_BOSS_73h_Male_Tauren_PlayerWindfury_02.prefab:80b01e91fb23bfc419dc6921df59c9fe");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_Idle_01,
    (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_Idle_02,
    (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_Idle_04
  };
  private static List<string> m_PlayerWindfury = new List<string>()
  {
    (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_PlayerWindfury_01,
    (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_PlayerWindfury_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_BossWhirlwindTempest_01,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_Death_01,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_HeroPower_01,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_HeroPower_02,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_HeroPower_03,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_HeroPower_04,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_Idle_01,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_Idle_02,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_Idle_04,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_Intro_01,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_PlayerShamanSpell_01,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_PlayerTotem_02,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_PlayerVoltron_01,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_PlayerWindfury_01,
      (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_PlayerWindfury_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_HeroPower_01,
    (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_HeroPower_02,
    (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_HeroPower_03,
    (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_HeroPower_04
  };

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_73h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_73h dalaDungeonBoss73h = this;
    while (dalaDungeonBoss73h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss73h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_PlayerShamanSpell_01);
        break;
      case 102:
        yield return (object) dalaDungeonBoss73h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_PlayerTotem_02);
        break;
      case 103:
        yield return (object) dalaDungeonBoss73h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_PlayerVoltron_01);
        break;
      case 104:
        yield return (object) dalaDungeonBoss73h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_73h.m_PlayerWindfury);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss73h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_73h dalaDungeonBoss73h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss73h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss73h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss73h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss73h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss73h.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_73h dalaDungeonBoss73h = this;
    while (dalaDungeonBoss73h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss73h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss73h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss73h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss73h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "DAL_742")
        yield return (object) dalaDungeonBoss73h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_73h.VO_DALA_BOSS_73h_Male_Tauren_BossWhirlwindTempest_01);
    }
  }
}
