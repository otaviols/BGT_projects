using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_71h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_BossForceOfNature_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_BossForceOfNature_01.prefab:4645fa632d0b43c40ab2882239999533");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_BossSoulOfTheForest_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_BossSoulOfTheForest_01.prefab:d27c2b80ff3ccbe4189e803dce82b8f0");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_BossTreeSpeaker_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_BossTreeSpeaker_01.prefab:96abaac2bbbe6834cb59133bb86de706");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_Death_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_Death_01.prefab:f42a4ea6ddc7ee64d8ccdbced4b8db8c");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_DefeatPlayer_01.prefab:51f415d552af26640bfb9657a041a36e");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_EmoteResponse_01.prefab:ada70a577a9e23e41b4c7c39c6202064");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_HeroPower_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_HeroPower_01.prefab:7146c0cfa5880bb4aad1a829737b5f11");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_HeroPower_02 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_HeroPower_02.prefab:cdd29f02e83b0264c976bee1ed4c3bb2");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_HeroPower_03 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_HeroPower_03.prefab:58184868872afbb4daaa542409be1a49");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_HeroPower_04 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_HeroPower_04.prefab:beb32b4b3d5f44a4486f4f016f1df916");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_HeroPower_05 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_HeroPower_05.prefab:83f214dabce3b964abb15030fd7c8d2d");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_HeroPower_06 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_HeroPower_06.prefab:dc09a3e7b5e9b7f468a62042b46c08f5");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_Idle_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_Idle_01.prefab:eb135af0c5bcc8d469e6dee9ace22aa0");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_Idle_02 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_Idle_02.prefab:70bda963e55394c4797e1aa4218dc7b8");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_Idle_03 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_Idle_03.prefab:ca21eae7ed86c714699671118bcc2353");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_Idle_04 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_Idle_04.prefab:fba6fc265304d0441be7c834e220822c");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_Intro_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_Intro_01.prefab:aa39897d00f4d0b4382f9a0fa9beb94d");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_PlayerFlamestrike_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_PlayerFlamestrike_01.prefab:033b669252bb62344aae7536e8b70e9e");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_PlayerNaturalize_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_PlayerNaturalize_01.prefab:042aa2e8afb130c41af3f565e54a9aa8");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_PlayerSap_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_PlayerSap_01.prefab:defb484a05fd7f1448988c1e94e9d3d8");
  private static readonly AssetReference VO_DALA_BOSS_71h_Female_NightElf_PlayerTreant_01 = new AssetReference("VO_DALA_BOSS_71h_Female_NightElf_PlayerTreant_01.prefab:ac454ad4346f53e41838ed237f9c22d8");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Idle_01,
    (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Idle_02,
    (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Idle_03,
    (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Idle_04
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_BossForceOfNature_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_BossSoulOfTheForest_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_BossTreeSpeaker_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Death_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_02,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_03,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_04,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_05,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_06,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Idle_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Idle_02,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Idle_03,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Idle_04,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Intro_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_PlayerFlamestrike_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_PlayerNaturalize_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_PlayerSap_01,
      (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_PlayerTreant_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_EmoteResponse_01;
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_01,
    (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_02,
    (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_03,
    (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_04,
    (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_05,
    (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_HeroPower_06
  };

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_71h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_71h dalaDungeonBoss71h = this;
    while (dalaDungeonBoss71h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss71h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_71h dalaDungeonBoss71h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss71h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss71h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss71h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss71h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss71h.m_playedLines.Add(cardId);
      switch (cardId)
      {
        case "CS2_032":
          yield return (object) dalaDungeonBoss71h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_PlayerFlamestrike_01);
          break;
        case "DAL_256t2":
        case "EX1_158t":
        case "FP1_019t":
        case "GIL_663t":
          yield return (object) dalaDungeonBoss71h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_PlayerTreant_01);
          break;
        case "EX1_161":
          yield return (object) dalaDungeonBoss71h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_PlayerNaturalize_01);
          break;
        case "EX1_581":
          yield return (object) dalaDungeonBoss71h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_PlayerSap_01);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_71h dalaDungeonBoss71h = this;
    while (dalaDungeonBoss71h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss71h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss71h.\u003C\u003En__2(entity);
      yield return (object) dalaDungeonBoss71h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss71h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_571"))
      {
        if (!(cardId == "EX1_158"))
        {
          if (cardId == "TRL_341")
            yield return (object) dalaDungeonBoss71h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_BossTreeSpeaker_01);
        }
        else
          yield return (object) dalaDungeonBoss71h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_BossSoulOfTheForest_01);
      }
      else
        yield return (object) dalaDungeonBoss71h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_71h.VO_DALA_BOSS_71h_Female_NightElf_BossForceOfNature_01);
    }
  }
}
