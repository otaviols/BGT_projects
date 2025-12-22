using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_67h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_BossAluneth_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_BossAluneth_01.prefab:926c214e7240da94e8279ad11ce74fca");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_BossArcaneSpell_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_BossArcaneSpell_01.prefab:e86215487ae8d564ba7131e9b3f24d0a");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_BossArcaneSpell_02 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_BossArcaneSpell_02.prefab:b33c2ead502fad44d9c499ef77cc8dd6");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_BossSpellDamageMinion_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_BossSpellDamageMinion_01.prefab:aaaa69d2a47c22043b5335fa4bd6ab0c");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_Death_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_Death_01.prefab:799a46bae9df7bf4e845b8267e664e91");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_DefeatPlayer_01.prefab:4aeba524ab4f16f4d83ceec65d580673");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_EmoteResponse_01.prefab:be2521311d8bd7a49bcdbe25e2edacb2");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_HeroPower_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_HeroPower_01.prefab:3f475154a9755844eb8ab9cdcf78fbeb");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_HeroPower_02 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_HeroPower_02.prefab:8c4cb6dee85d4684a8493cd0c54db053");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_HeroPower_03 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_HeroPower_03.prefab:9064cf19e63c3b947a3cf51e9049193f");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_HeroPower_04 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_HeroPower_04.prefab:14b33d653d4ed8040ae9d5ec9e6eff11");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_HeroPower_05 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_HeroPower_05.prefab:9618e072fc851b24db3f50ca10a47671");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_Idle_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_Idle_01.prefab:2f0be226ceba22e438d65b14aee01e3e");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_Idle_02 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_Idle_02.prefab:d8349d70b48c23046a62b69a3085eef1");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_Idle_03 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_Idle_03.prefab:dd03ba71892196b4082316004bad9ab7");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_Intro_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_Intro_01.prefab:5291b993a47120146ba54cce9955a5c5");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_PlayerArcaneSpell_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_PlayerArcaneSpell_01.prefab:12f65551fdb1698428a6b80bea806bbd");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_PlayerArchmage_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_PlayerArchmage_01.prefab:018123b4f40d4924a8f74718b88258a5");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_PlayerKalecgos_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_PlayerKalecgos_01.prefab:4b3d74dc8450a8b4da6bc61d95004ae4");
  private static readonly AssetReference VO_DALA_BOSS_67h_Male_Human_PlayerMalygos_01 = new AssetReference("VO_DALA_BOSS_67h_Male_Human_PlayerMalygos_01.prefab:4ec69bf3eb81e79409ac2f5a0427c824");
  private HashSet<string> m_playedLines = new HashSet<string>();
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_Idle_01,
    (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_Idle_02,
    (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_Idle_03
  };
  private static List<string> m_BossArcaneSpells = new List<string>()
  {
    (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_BossArcaneSpell_01,
    (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_BossArcaneSpell_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_BossAluneth_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_BossArcaneSpell_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_BossArcaneSpell_02,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_BossSpellDamageMinion_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_Death_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_HeroPower_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_HeroPower_02,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_HeroPower_03,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_HeroPower_04,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_HeroPower_05,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_Idle_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_Idle_02,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_Idle_03,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_Intro_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_PlayerArcaneSpell_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_PlayerArchmage_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_PlayerKalecgos_01,
      (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_PlayerMalygos_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_HeroPower_01,
    (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_HeroPower_02,
    (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_HeroPower_03,
    (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_HeroPower_04,
    (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_HeroPower_05
  };

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_67h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_67h dalaDungeonBoss67h = this;
    while (dalaDungeonBoss67h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss67h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_67h dalaDungeonBoss67h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss67h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss67h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss67h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss67h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss67h.m_playedLines.Add(cardId);
      switch (cardId)
      {
        case "AT_004":
        case "CFM_623":
        case "CS2_023":
        case "CS2_025":
        case "DS1_185":
        case "EX1_277":
          yield return (object) dalaDungeonBoss67h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_PlayerArcaneSpell_01);
          break;
        case "DAL_553":
        case "DAL_558":
        case "EX1_559":
        case "GIL_691":
          yield return (object) dalaDungeonBoss67h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_PlayerArchmage_01);
          break;
        case "DAL_609":
          yield return (object) dalaDungeonBoss67h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_PlayerKalecgos_01);
          break;
        case "EX1_563":
          yield return (object) dalaDungeonBoss67h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_PlayerMalygos_01);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_67h dalaDungeonBoss67h = this;
    while (dalaDungeonBoss67h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss67h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss67h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss67h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss67h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "AT_004":
        case "CFM_623":
        case "CS2_023":
        case "CS2_025":
        case "DS1_185":
        case "EX1_277":
          yield return (object) dalaDungeonBoss67h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_67h.m_BossArcaneSpells);
          break;
        case "BRM_002":
        case "DAL_182":
        case "NEW1_020":
          yield return (object) dalaDungeonBoss67h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_BossSpellDamageMinion_01);
          break;
        case "LOOT_108":
          yield return (object) dalaDungeonBoss67h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_67h.VO_DALA_BOSS_67h_Male_Human_BossAluneth_01);
          break;
      }
    }
  }
}
