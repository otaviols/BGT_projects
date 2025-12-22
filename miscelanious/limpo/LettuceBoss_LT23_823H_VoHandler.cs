using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_823H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_YoggSaron_Male_OldGod_LETL_Attack_03 = new AssetReference("VO_YoggSaron_Male_OldGod_LETL_Attack_03.prefab:a27f80f1fff20f84faab365e4a0e9737");
  private static readonly AssetReference VO_YoggSaron_Male_OldGod_LETL_Attack_04 = new AssetReference("VO_YoggSaron_Male_OldGod_LETL_Attack_04.prefab:954a5567ec701dc4d818272e3975d3fb");
  private static readonly AssetReference VO_YoggSaron_Male_OldGod_LETL_C02_T24_Dialogue_01 = new AssetReference("VO_YoggSaron_Male_OldGod_LETL_C02_T24_Dialogue_01.prefab:61989fc2efa100a498fa9408c985d33e");
  private static readonly AssetReference VO_YoggSaron_Male_OldGod_LETL_Death_02 = new AssetReference("VO_YoggSaron_Male_OldGod_LETL_Death_02.prefab:5546a12e2a7ebcb41af3efec2d501e45");
  private static readonly AssetReference VO_YoggSaron_Male_OldGod_LETL_Idle_01 = new AssetReference("VO_YoggSaron_Male_OldGod_LETL_Idle_01.prefab:ed8a43a97a40a804faaac06c5fd029ae");
  private static readonly AssetReference VO_YoggSaron_Male_OldGod_LETL_Intro_02 = new AssetReference("VO_YoggSaron_Male_OldGod_LETL_Intro_02.prefab:32aa9fc44fa0a8e42b2e866dfecc133e");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_Intro_02,
      (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_Attack_03,
      (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_Attack_04,
      (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_C02_T24_Dialogue_01,
      (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_Idle_01,
      (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_Death_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_Intro_02;
    this.m_deathLine = (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_Death_02;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_823H_VoHandler lt23823HVoHandler = this;
    while (lt23823HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23823HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_823H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_823H")
    {
      string str = cardID;
      if (!(str == "LT23_823P1"))
      {
        if (!(str == "LT23_823P2"))
        {
          if (str == "LT23_823P3")
          {
            GameState.Get().SetBusy(true);
            yield return (object) lt23823HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_C02_T24_Dialogue_01);
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt23823HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_Attack_04);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23823HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_823H_VoHandler.VO_YoggSaron_Male_OldGod_LETL_Attack_03);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_823H_VoHandler lt23823HVoHandler = this;
    while (lt23823HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23823HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_823H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23823HVoHandler.MissionPlayVO(playByDesignCode, lt23823HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23823HVoHandler.MissionPlayVO(playByDesignCode, lt23823HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23823HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_823H_VoHandler lt23823HVoHandler = this;
    while (lt23823HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23823HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_823H");
    if (entity.GetCardId() == "LT23_823H")
      yield return (object) lt23823HVoHandler.MissionPlaySound(playByDesignCode, lt23823HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_823H_VoHandler lt23823HVoHandler = this;
    while (lt23823HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23823HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_823H");
    if (turn == 1)
      yield return (object) lt23823HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23823HVoHandler.m_introLine);
  }
}
