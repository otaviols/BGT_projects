using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOTA_Survival_Puzzle_2 : BOTA_MissionEntity
{
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Complete_02 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Complete_02.prefab:ec34127b832ca334492d81beabe51ea2");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_01 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_01.prefab:dec59b2a83e84de4db1c9e0ee1f7695e");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_02 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_02.prefab:2703ab76dde328848a945ee8ffbe728b");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_03 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_03.prefab:976e13e6db6991146bd6fec7c4f2ccbf");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Idle_01 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Idle_01.prefab:f9a403c5224a68c48b14d1699d1ae383");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Idle_02 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Idle_02.prefab:78ca9f547a00fd54e915185e3d701f2f");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Idle_03 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Idle_03.prefab:c166b33c7c3ece44cad6ba861401eadf");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Idle_04 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Idle_04.prefab:6d76a3355f6bc354faa43b8f4383a77c");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Idle_05 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Idle_05.prefab:c2175b5010148bb469ae2cc69fa0b7e2");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Idle_06 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Idle_06.prefab:846d065380be8d449b9c3348d92fa1ca");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Idle_07 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Idle_07.prefab:e136e229ab37a504883c229a1298241f");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Idle_08 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Idle_08.prefab:6a295cf17c928ff40880e5043ce268c1");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Idle_09 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Idle_09.prefab:5eb01b739b48dc844b39696f206a7544");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Intro_01 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Intro_01.prefab:735d02a0910b9c54fb72c16da7c2c644");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_PuzzleA_01 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_PuzzleA_01.prefab:42f10ee5624a63c4db7c58cb813fcf68");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_PuzzleB_01 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_PuzzleB_01.prefab:aa241a50b597a804da2db166737c9b83");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Restart_01 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Restart_01.prefab:22352523a16d0de4397ef19bca949777");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Restart_02 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Restart_02.prefab:b6a4123faabc12a4eb180c5594ce7e9e");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Restart_03 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Restart_03.prefab:b708ab15a42fd6a488aa8a2e4279901d");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Restart_04 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Restart_04.prefab:1394d7e77382c0245b6438be7768628b");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Restart_05 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Restart_05.prefab:fdd7f44fe853eb94a92e0050a95a5216");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Restart_06 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Restart_06.prefab:731193994db2ab442965b205d004bccd");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Restart_07 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Restart_07.prefab:e37ea594abe480b4abef18b2533e239a");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Return_01 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Return_01.prefab:505a989d1afda44489910b10aab7fcb8");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Stuck_01 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Stuck_01.prefab:a81e8fa906a5ea5488e8ee680a38c315");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Stuck_02 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Stuck_02.prefab:c2a319a1319d73c4e9436900c00122e5");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Stuck_03 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Stuck_03.prefab:6f86c230f2d07c94c9efcb167e20e549");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Stuck_04 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Stuck_04.prefab:d341c3875b5abc048b27571788dde5ad");
  private static readonly AssetReference VO_BOTA_BOSS_16h_Male_Goblin_Puzzle_02 = new AssetReference("VO_BOTA_BOSS_16h_Male_Goblin_Puzzle_02.prefab:2c4b9e774308eea48ab33b21bcb0d07c");
  private string COMPLETE_LINE = (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Complete_02;

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Complete_02,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_02,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_03,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_02,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_03,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_04,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_05,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_06,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_07,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_08,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_09,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Intro_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_PuzzleA_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_PuzzleB_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_02,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_03,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_04,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_05,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_06,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_07,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Return_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Stuck_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Stuck_02,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Stuck_03,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Stuck_04,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Puzzle_02
    })
      this.PreloadSound(soundPath);
  }

  protected override float ChanceToPlayRandomVOLine() => 1f;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    BOTA_MissionEntity.s_introLine = (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Intro_01;
    BOTA_MissionEntity.s_returnLine = (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Return_01;
    this.s_victoryLine_1 = (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_PuzzleA_01;
    this.s_victoryLine_2 = (string) null;
    this.s_victoryLine_3 = (string) null;
    this.s_victoryLine_4 = (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Puzzle_02;
    this.s_victoryLine_5 = (string) null;
    this.s_victoryLine_6 = (string) null;
    this.s_victoryLine_7 = (string) null;
    this.s_victoryLine_8 = (string) null;
    this.s_victoryLine_9 = (string) null;
    this.s_emoteLines = new List<string>()
    {
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_02,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_EmoteResponse_03
    };
    this.s_idleLines = new List<string>()
    {
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_02,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_03,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_04,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_05,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_06,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_07,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_08,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Idle_09
    };
    this.s_restartLines = new List<string>()
    {
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_02,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_03,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_04,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_05,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_06,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Restart_07,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Stuck_01,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Stuck_02,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Stuck_03,
      (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_Stuck_04
    };
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOTA_Survival_Puzzle_2 botaSurvivalPuzzle2 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (missionEvent == 99)
    {
      while (botaSurvivalPuzzle2.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(true);
      yield return (object) botaSurvivalPuzzle2.PlayBossLine(enemyActor, botaSurvivalPuzzle2.COMPLETE_LINE);
      GameState.Get().SetBusy(false);
    }
  }

  public override IEnumerator OnTurnStartManagerFinishedWithTiming()
  {
    BOTA_Survival_Puzzle_2 botaSurvivalPuzzle2 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    int tag = GameState.Get().GetFriendlySidePlayer().GetSecretZone().GetPuzzleEntity().GetTag(GAME_TAG.PUZZLE_PROGRESS);
    string puzzleVictoryLine = botaSurvivalPuzzle2.GetPuzzleVictoryLine(tag);
    if (puzzleVictoryLine != null)
    {
      if (puzzleVictoryLine == (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_PuzzleA_01)
      {
        yield return (object) botaSurvivalPuzzle2.PlayBossLine(enemyActor, (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_PuzzleA_01);
        yield return (object) new WaitForSeconds(0.8f);
        yield return (object) botaSurvivalPuzzle2.PlayBossLine(enemyActor, (string) BOTA_Survival_Puzzle_2.VO_BOTA_BOSS_16h_Male_Goblin_PuzzleB_01);
      }
      else
        yield return (object) botaSurvivalPuzzle2.PlayBossLine(enemyActor, puzzleVictoryLine);
    }
  }

  private string GetPuzzleVictoryLine(int puzzleProgress)
  {
    switch (puzzleProgress)
    {
      case 1:
        return this.s_victoryLine_1;
      case 2:
        return this.s_victoryLine_2;
      case 3:
        return this.s_victoryLine_3;
      case 4:
        return this.s_victoryLine_4;
      case 5:
        return this.s_victoryLine_5;
      case 6:
        return this.s_victoryLine_6;
      case 7:
        return this.s_victoryLine_7;
      case 8:
        return this.s_victoryLine_8;
      case 9:
        return this.s_victoryLine_9;
      default:
        return (string) null;
    }
  }
}
