using System.Collections;
using System.Collections.Generic;

public class BOM_10_Dawngrasp_Fight_002 : BOM_10_Dawngrasp_Dungeon
{
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_01 = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_01.prefab:18c237a05bd55e546ae216f90e2f11a2");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_02 = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_02.prefab:6b0e5eb429185574d88fb3c40190f9c8");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_03 = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_03.prefab:a5e1a45e1055c3c42ab61c2750646a5f");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_01 = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_01.prefab:a7a369ac163620146aa5c35c2e8bb61c");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_02 = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_02.prefab:625515a65f2cb9e42b7dbd993c5b0860");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_03 = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_03.prefab:fa3b5cc1073f72f4fa9bbdc91e1892bc");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_EmoteResponse_01 = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_EmoteResponse_01.prefab:be71c19c5c174754b9b6677458f21a09");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Introduction_01_A = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Introduction_01_A.prefab:a0b0b255c708e9b4a8f8c779805b6993");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_PlayerLoss_01 = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_PlayerLoss_01.prefab:7b4dab37a28a1ee40bd264061c4e3b45");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_01_01 = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_01_01.prefab:6dae2472cba0ef246ae394551b4a1114");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_05_01_A = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_05_01_A.prefab:a0b31db07bcf5fc49a17a0a8f916f115");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_07_01_B = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_07_01_B.prefab:f7780941aa0d955448ae99596d08ac1d");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_11_01_B = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_11_01_B.prefab:597fb41b5db7dac4dbe656376838f8f2");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_A = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_A.prefab:cb3b54137eb103d4f92ded8a929661b7");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_B = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_B.prefab:f956c49c4966d3f46a2a7ada486a2fc0");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_D = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_D.prefab:d08ad2ce3e7386b4d90c5047a6dd90d6");
  private static readonly AssetReference VO_BOM_10_002_Male_BloodElf_Lorthemar_UI_AWD_Boss_Reveal_General_02_01_C = new AssetReference("VO_BOM_10_002_Male_BloodElf_Lorthemar_UI_AWD_Boss_Reveal_General_02_01_C.prefab:2541b6f64434c094db9f75b2acd7fda2");
  private static readonly AssetReference VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Introduction_01_B = new AssetReference("VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Introduction_01_B.prefab:f9382b52bdd587049a23dc34589cdaa7");
  private static readonly AssetReference VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Turn_07_01_A = new AssetReference("VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Turn_07_01_A.prefab:4272c71220935bd49a2a08472f873497");
  private static readonly AssetReference VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Turn_11_01_A = new AssetReference("VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Turn_11_01_A.prefab:93071170a4f6d274884f800b91347556");
  private static readonly AssetReference VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_VictoryPreExplosion_01_C = new AssetReference("VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_VictoryPreExplosion_01_C.prefab:076dd16a691af0e40a1fb0951f9e228e");
  private static readonly AssetReference VO_BOM_10_002_X_BloodElf_Dawngrasp_UI_AWD_Boss_Reveal_General_02_01_A = new AssetReference("VO_BOM_10_002_X_BloodElf_Dawngrasp_UI_AWD_Boss_Reveal_General_02_01_A.prefab:06ec52f163051714198f7bd13cdedf92");
  private static readonly AssetReference VO_BOM_10_002_X_BloodElf_Dawngrasp_UI_AWD_Boss_Reveal_General_02_01_B = new AssetReference("VO_BOM_10_002_X_BloodElf_Dawngrasp_UI_AWD_Boss_Reveal_General_02_01_B.prefab:99002ae77d0f95849bdcfde6b6db4c11");
  private List<string> m_InGame_BossIdleLines = new List<string>()
  {
    (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_01,
    (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_02,
    (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_03
  };
  private List<string> m_InGame_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_01,
    (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_02,
    (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_01,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_02,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossIdle_03,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_01,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_02,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_BossUsesHeroPower_03,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_EmoteResponse_01,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Introduction_01_A,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_PlayerLoss_01,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_01_01,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_05_01_A,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_07_01_B,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_11_01_B,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_A,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_B,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_D,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_UI_AWD_Boss_Reveal_General_02_01_C,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Introduction_01_B,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Turn_07_01_A,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Turn_11_01_A,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_VictoryPreExplosion_01_C,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_X_BloodElf_Dawngrasp_UI_AWD_Boss_Reveal_General_02_01_A,
      (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_X_BloodElf_Dawngrasp_UI_AWD_Boss_Reveal_General_02_01_B
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame() => base.OnCreateGame();

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_10_Dawngrasp_Fight_002 dawngraspFight002 = this;
    while (dawngraspFight002.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_A);
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_B);
        yield return (object) dawngraspFight002.MissionPlayVO(friendlyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_VictoryPreExplosion_01_C);
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_VictoryPreExplosion_01_D);
        GameState.Get().SetBusy(false);
        break;
      case 506:
        GameState.Get().SetBusy(true);
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_PlayerLoss_01);
        GameState.Get().SetBusy(false);
        break;
      case 510:
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, dawngraspFight002.m_InGame_BossUsesHeroPowerLines);
        break;
      case 514:
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_UI_AWD_Boss_Reveal_General_02_01_C);
        break;
      case 515:
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_EmoteResponse_01);
        break;
      case 517:
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, dawngraspFight002.m_InGame_BossIdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dawngraspFight002.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_10_Dawngrasp_Fight_002 dawngraspFight002 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dawngraspFight002.\u003C\u003En__1(entity);
    while (dawngraspFight002.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dawngraspFight002.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dawngraspFight002.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dawngraspFight002.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BOM_10_Dawngrasp_Fight_002 dawngraspFight002 = this;
    while (dawngraspFight002.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dawngraspFight002.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dawngraspFight002.\u003C\u003En__2(entity);
      yield return (object) dawngraspFight002.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dawngraspFight002.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BOM_10_Dawngrasp_Fight_002 dawngraspFight002 = this;
    while (dawngraspFight002.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Introduction_01_A);
        yield return (object) dawngraspFight002.MissionPlayVO(friendlyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Introduction_01_B);
        break;
      case 3:
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_01_01);
        break;
      case 5:
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_05_01_A);
        break;
      case 7:
        yield return (object) dawngraspFight002.MissionPlayVO(friendlyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Turn_07_01_A);
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_07_01_B);
        break;
      case 11:
        yield return (object) dawngraspFight002.MissionPlayVO(friendlyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_X_BloodElf_Dawngrasp_InGame_Turn_11_01_A);
        yield return (object) dawngraspFight002.MissionPlayVO(enemyActor, (string) BOM_10_Dawngrasp_Fight_002.VO_BOM_10_002_Male_BloodElf_Lorthemar_InGame_Turn_11_01_B);
        break;
    }
  }
}
