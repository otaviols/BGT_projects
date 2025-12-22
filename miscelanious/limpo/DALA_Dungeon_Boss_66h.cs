using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_66h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_BossBigMinion_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_BossBigMinion_01.prefab:be8b725ef2070034f8c607950263517d");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_BossBurnCard_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_BossBurnCard_01.prefab:64a439b572181544fa3f55a7840743fe");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_BossCurseofWeakness_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_BossCurseofWeakness_01.prefab:4285b2cf2f0ccf74584a462585b7209a");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_Death_02 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_Death_02.prefab:1e3d237a2bc837b43b8d6498e7f11cd4");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_DefeatPlayer_01.prefab:d104b352b3b73de4db563c6d218be072");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_EmoteResponse_01.prefab:59d031ce9099fa64cb418e56a1d4ee47");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_HeroPower_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_HeroPower_01.prefab:b5f2b059ccd707843b60b5471aa23b1f");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_HeroPower_02 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_HeroPower_02.prefab:d0c6fd89c1792a84b90f88af6993b085");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_HeroPower_03 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_HeroPower_03.prefab:be9a6db46fb3e284a9ab70f7fd38364c");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_HeroPower_04 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_HeroPower_04.prefab:578b6b9e8f8f5064fbed39488147c45e");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_HeroPower_05 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_HeroPower_05.prefab:26d56c9852721d94ba6f11ce8f2bbbb0");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_HeroPower_06 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_HeroPower_06.prefab:6efed8044a876d74dad708480f3df43e");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_HeroPowerEmpty_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_HeroPowerEmpty_01.prefab:88671044a4728d447bb2e3a27fcf38b4");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_Idle_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_Idle_01.prefab:cda3f1b374d8cc449b3c9ff831574e75");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_Idle_02 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_Idle_02.prefab:7f728494ef4de8542af4df37a0da72d5");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_Idle_03 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_Idle_03.prefab:13a74c53478e80f4aa8504954cade4b9");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_Intro_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_Intro_01.prefab:73beceeb89a2af44691589133437ec11");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_PlayerBurnCard_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_PlayerBurnCard_01.prefab:823fbe07a5f04c94c88ba5ff6c9d2f71");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_PlayerLeeroy_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_PlayerLeeroy_01.prefab:f1f94b7de565f4a4c9a2bca9dd23cbd9");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_PlayerRushMinion_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_PlayerRushMinion_01.prefab:42bbc558956026c4b838fbecda71b155");
  private static readonly AssetReference VO_DALA_BOSS_66h_Female_Undead_TriggerBomb_01 = new AssetReference("VO_DALA_BOSS_66h_Female_Undead_TriggerBomb_01.prefab:580c04fd2042d8649ba267cbac635356");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_Idle_01,
    (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_Idle_02,
    (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_Idle_03
  };
  private static List<string> m_HeroPower = new List<string>()
  {
    (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_01,
    (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_02,
    (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_03,
    (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_04,
    (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_05,
    (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_06
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_BossBigMinion_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_BossBurnCard_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_BossCurseofWeakness_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_Death_02,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_02,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_03,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_04,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_05,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPower_06,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPowerEmpty_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_Idle_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_Idle_02,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_Idle_03,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_Intro_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_PlayerBurnCard_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_PlayerLeeroy_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_PlayerRushMinion_01,
      (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_TriggerBomb_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_Death_02;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_66h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_66h dalaDungeonBoss66h = this;
    while (dalaDungeonBoss66h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss66h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_66h.m_HeroPower);
        break;
      case 102:
        yield return (object) dalaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_HeroPowerEmpty_01);
        break;
      case 103:
        yield return (object) dalaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_BossBigMinion_01);
        break;
      case 104:
        yield return (object) dalaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_BossBurnCard_01);
        break;
      case 105:
        yield return (object) dalaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_PlayerBurnCard_01);
        break;
      case 106:
        yield return (object) dalaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_PlayerRushMinion_01);
        break;
      case 107:
        yield return (object) dalaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_TriggerBomb_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss66h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_66h dalaDungeonBoss66h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss66h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss66h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss66h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss66h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss66h.m_playedLines.Add(cardId);
      if (cardId == "EX1_116")
        yield return (object) dalaDungeonBoss66h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_PlayerLeeroy_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_66h dalaDungeonBoss66h = this;
    while (dalaDungeonBoss66h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss66h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss66h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss66h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "GIL_665")
        yield return (object) dalaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_66h.VO_DALA_BOSS_66h_Female_Undead_BossCurseofWeakness_01);
    }
  }
}
