using System.Collections;
using System.Collections.Generic;

public class BOTA_Mirror_Puzzle_3 : BOTA_MissionEntity
{
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Complete_01 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Complete_01.prefab:de9ecbc3f75cbdc4bac83192eca8f0f2");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_01 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_01.prefab:b044edc26aafb9642a67961319817eca");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_02 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_02.prefab:17d25e1b7dcbb8446a839739b8294b23");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_03 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_03.prefab:536f84845df25264b99da8ac2669690e");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Idle_01 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Idle_01.prefab:47b7fbd6fb558fd40bd354c426408aff");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Idle_03 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Idle_03.prefab:00940d90da91b304f9ae498e0798bd18");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Idle_04 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Idle_04.prefab:59582a9d62f15d748901e3e85af44986");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Idle_05 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Idle_05.prefab:b3c79cdace023b74c8cc2355538bd3af");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Idle_07 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Idle_07.prefab:f95327813868ad34bb46b14fb608bd2c");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Idle_08 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Idle_08.prefab:296e7e89fd6a5c14f8068aef31656f93");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Idle_09 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Idle_09.prefab:dbdb3b276936ec64993eb3d8944ec19d");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Intro_01 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Intro_01.prefab:e0abe8e94b60cc94885b8a569eba2e23");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_01 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_01.prefab:1da8155e6c2d6b742b29a2fc0a73ec01");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_02 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_02.prefab:bada34dac1f65904386694d1dfc0adf5");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_03 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_03.prefab:265f1f8901fc88945b262c73642bcf4c");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_04 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_04.prefab:3158c3332ac67fc46af92b3310a84335");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_05 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_05.prefab:07ceac8974cce294995ea36bb200c346");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_06 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_06.prefab:748eeeaedee9c6741ba5db18f62ed6b7");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Restart_01 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Restart_01.prefab:11119bcf728cd15498711b521abe4cc3");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Restart_02 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Restart_02.prefab:aec95d129df680a4fb323d4892851a47");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Restart_03 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Restart_03.prefab:11ae6160be493ea40a43fe9e5e4fd005");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Restart_05 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Restart_05.prefab:8e10ad6004c7c51409b0aa0559bf1f04");
  private static readonly AssetReference VO_BOTA_BOSS_04h_Female_Draenei_Return_01 = new AssetReference("VO_BOTA_BOSS_04h_Female_Draenei_Return_01.prefab:b24c9edeebfe82a42af22aa150986528");
  private string COMPLETE_LINE = (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Complete_01;

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Complete_01,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_01,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_02,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_03,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_01,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_03,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_04,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_05,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_07,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_08,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_09,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Intro_01,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_01,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_02,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_03,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_04,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_05,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_06,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Restart_01,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Restart_02,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Restart_03,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Restart_05,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Return_01
    })
      this.PreloadSound(soundPath);
  }

  protected override float ChanceToPlayRandomVOLine() => 1f;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    BOTA_MissionEntity.s_introLine = (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Intro_01;
    BOTA_MissionEntity.s_returnLine = (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Return_01;
    this.s_victoryLine_1 = (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_01;
    this.s_victoryLine_2 = (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_04;
    this.s_victoryLine_3 = (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_03;
    this.s_victoryLine_4 = (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_02;
    this.s_victoryLine_5 = (string) null;
    this.s_victoryLine_6 = (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_05;
    this.s_victoryLine_7 = (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Puzzle_06;
    this.s_victoryLine_8 = (string) null;
    this.s_victoryLine_9 = (string) null;
    this.s_emoteLines = new List<string>()
    {
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_01,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_02,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_EmoteResponse_03
    };
    this.s_idleLines = new List<string>()
    {
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_01,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_03,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_04,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_05,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_07,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_08,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Idle_09
    };
    this.s_restartLines = new List<string>()
    {
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Restart_01,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Restart_02,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Restart_03,
      (string) BOTA_Mirror_Puzzle_3.VO_BOTA_BOSS_04h_Female_Draenei_Restart_05
    };
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOTA_Mirror_Puzzle_3 botaMirrorPuzzle3 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (missionEvent == 99)
    {
      while (botaMirrorPuzzle3.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(true);
      yield return (object) botaMirrorPuzzle3.PlayBossLine(enemyActor, botaMirrorPuzzle3.COMPLETE_LINE);
      GameState.Get().SetBusy(false);
    }
  }
}
