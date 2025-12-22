using System.Collections;
using System.Collections.Generic;

public class BOTA_Lethal_Puzzle_3 : BOTA_MissionEntity
{
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Complete_01 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Complete_01.prefab:0aff580f66ac0144780e1a871839da04");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_01 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_01.prefab:8163056ed1098494cad9197f206f39d0");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_02 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_02.prefab:c601ed97699ed8e4c90965138e7be987");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_03 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_03.prefab:7052e872c483ae346a46242c9941de0e");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_01 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_01.prefab:e64d4dd9f38f6d247997d7d3b47db921");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_02 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_02.prefab:523f64d4a351c214591212e161c9145f");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_03 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_03.prefab:be25ff2d6503eac45ad5322e3bbd0580");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_04 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_04.prefab:7ee14a67e9d58834b86444f6a6a22f3b");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_05 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_05.prefab:f4cc404356bd47447ac5ef1fbe9ac3a3");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_06 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_06.prefab:7d8aab996b7fee542b59d9efd6313a35");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_07 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_07.prefab:dd0b1127174e7ae48aefca150aadf295");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_08 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_08.prefab:350b83cb3ae39004fa2ef5c9ce746076");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_09 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_09.prefab:d129ec81ba61ba648ad71e1a3472ca8e");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Intro_01 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Intro_01.prefab:96417cba41696094b9f6716dd829d1e8");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_PlayHealingRain_01 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_PlayHealingRain_01.prefab:1144fc3d94f024a47bd38f27fb6e78cf");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_01 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_01.prefab:3af0c14e01064994ebd4a8eb50dfbc99");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_02 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_02.prefab:a4ddc5db00322444e96e4c30f5628d3e");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_03 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_03.prefab:12e5874fe8a27504a9ea5ce42373c829");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_04 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_04.prefab:e6f5f99a3e1ba17458a998ad7c96c1ee");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_05 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_05.prefab:361c3e92056593a4e977301e8dfc09e7");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_06 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_06.prefab:fd567f7613795ea468c842d6f8cf8230");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_07 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_07.prefab:7db895f68a4d27b4284b4f8db1d91b8a");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_08 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_08.prefab:b20f72a888f95e04cb03b078ab02d984");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Return_01 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Return_01.prefab:86bf69c954dfa0a4fa58c20ccf2cc2d7");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_01 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_01.prefab:d73e725d417797445a8c0f05f718659c");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_02 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_02.prefab:9394636443fbcc945a79bc9f7276da92");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_04 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_04.prefab:6e87dd8a4dcb0d6448b67a849c864ed3");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_05 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_05.prefab:7d3c8c47c0fba244191cb5fbbada1221");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_06 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_06.prefab:e826e9d62df330d4b93803f5665d70a5");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_03 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_03.prefab:4b2fe859e8f1e54439bb89fdaa109905");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_05 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_05.prefab:dd28ae7ffef7f114ba721e488b41dd6c");
  private static readonly AssetReference VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_01 = new AssetReference("VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_01.prefab:e4a203d8b6acd1f488c1bc62d9ab7f99");
  private string COMPLETE_LINE = (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Complete_01;

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Complete_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_02,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_03,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_02,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_03,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_04,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_05,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_06,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_07,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_08,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_09,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Intro_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_PlayHealingRain_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_02,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_03,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_04,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_05,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_06,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_07,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_08,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Return_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_02,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_04,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_05,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_06,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_03,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_05,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_01
    })
      this.PreloadSound(soundPath);
  }

  protected override float ChanceToPlayRandomVOLine() => 1f;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    BOTA_MissionEntity.s_introLine = (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Intro_01;
    BOTA_MissionEntity.s_returnLine = (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Return_01;
    this.s_victoryLine_1 = (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_01;
    this.s_victoryLine_2 = (string) null;
    this.s_victoryLine_3 = (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_03;
    this.s_victoryLine_4 = (string) null;
    this.s_victoryLine_5 = (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Puzzle_05;
    this.s_victoryLine_6 = (string) null;
    this.s_victoryLine_7 = (string) null;
    this.s_victoryLine_8 = (string) null;
    this.s_victoryLine_9 = (string) null;
    this.s_emoteLines = new List<string>()
    {
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_02,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_EmoteResponse_03
    };
    this.s_idleLines = new List<string>()
    {
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_02,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_03,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_04,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_05,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_06,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_07,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_08,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Idle_09
    };
    this.s_restartLines = new List<string>()
    {
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_02,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_03,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_04,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_05,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_06,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_07,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Restart_08
    };
    this.s_lethalCompleteLines = new List<string>()
    {
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_01,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_02,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_04,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_05,
      (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_Lethal_06
    };
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOTA_Lethal_Puzzle_3 botaLethalPuzzle3 = this;
    while (botaLethalPuzzle3.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    yield return (object) botaLethalPuzzle3.WaitForEntitySoundToFinish(entity);
    string cardId = entity.GetCardId();
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (cardId == "LOOT_373")
      yield return (object) botaLethalPuzzle3.PlayLineOnlyOnce(actor, (string) BOTA_Lethal_Puzzle_3.VO_BOTA_BOSS_12h_Female_WaterSpirit_PlayHealingRain_01);
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOTA_Lethal_Puzzle_3 botaLethalPuzzle3 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 99:
        while (botaLethalPuzzle3.m_enemySpeaking)
          yield return (object) null;
        GameState.Get().SetBusy(true);
        yield return (object) botaLethalPuzzle3.PlayBossLine(enemyActor, botaLethalPuzzle3.COMPLETE_LINE);
        GameState.Get().SetBusy(false);
        break;
      case 777:
        string lethalCompleteLine = botaLethalPuzzle3.GetLethalCompleteLine();
        if (lethalCompleteLine == null)
          break;
        GameState.Get().SetBusy(true);
        yield return (object) botaLethalPuzzle3.PlayBossLine(enemyActor, lethalCompleteLine);
        GameState.Get().SetBusy(false);
        break;
    }
  }
}
