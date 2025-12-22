using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_818H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_RingmasterWhatley_Male_Worgen_LETL_Attack_01 = new AssetReference("VO_RingmasterWhatley_Male_Worgen_LETL_Attack_01.prefab:2b96ef5f6b726b44483a874e706ffc43");
  private static readonly AssetReference VO_RingmasterWhatley_Male_Worgen_LETL_Attack_02 = new AssetReference("VO_RingmasterWhatley_Male_Worgen_LETL_Attack_02.prefab:a1787841f812b33448f6ecce17cc8af0");
  private static readonly AssetReference VO_RingmasterWhatley_Male_Worgen_LETL_Death_01 = new AssetReference("VO_RingmasterWhatley_Male_Worgen_LETL_Death_01.prefab:5456390430fffa34aa3435a62922a29c");
  private static readonly AssetReference VO_RingmasterWhatley_Male_Worgen_LETL_Idle_01 = new AssetReference("VO_RingmasterWhatley_Male_Worgen_LETL_Idle_01.prefab:8d56b944d6b561f438c0a51d8047ab45");
  private static readonly AssetReference VO_RingmasterWhatley_Male_Worgen_LETL_Idle_02 = new AssetReference("VO_RingmasterWhatley_Male_Worgen_LETL_Idle_02.prefab:f971734d8745a0944a7e88503199460a");
  private static readonly AssetReference VO_RingmasterWhatley_Male_Worgen_LETL_Intro_01 = new AssetReference("VO_RingmasterWhatley_Male_Worgen_LETL_Intro_01.prefab:b662b6bc3bc2acf43bbce3d4ccb6219c");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Idle_01,
    (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Idle_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Intro_01,
      (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Attack_01,
      (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Attack_02,
      (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Idle_01,
      (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Idle_02,
      (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_818H_VoHandler lt23818HVoHandler = this;
    while (lt23818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23818HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_818H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_818H")
    {
      string str = cardID;
      if (!(str == "LT23_818P1"))
      {
        if (str == "LT23_818P2")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt23818HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23818HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_818H_VoHandler.VO_RingmasterWhatley_Male_Worgen_LETL_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_818H_VoHandler lt23818HVoHandler = this;
    while (lt23818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23818HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_818H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23818HVoHandler.MissionPlayVO(playByDesignCode, lt23818HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23818HVoHandler.MissionPlayVO(playByDesignCode, lt23818HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23818HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_818H_VoHandler lt23818HVoHandler = this;
    while (lt23818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23818HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_818H");
    if (entity.GetCardId() == "LT23_818H")
      yield return (object) lt23818HVoHandler.MissionPlaySound(playByDesignCode, lt23818HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_818H_VoHandler lt23818HVoHandler = this;
    while (lt23818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23818HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_818H");
    if (turn == 1)
      yield return (object) lt23818HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23818HVoHandler.m_introLine);
  }
}
