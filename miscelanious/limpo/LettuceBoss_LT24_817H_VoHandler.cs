using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_817H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_TheCurator_Male_Mech_LETL_Attack_01 = new AssetReference("VO_TheCurator_Male_Mech_LETL_Attack_01.prefab:344f3c4b70e39904195fbfdc01cdebc3");
  private static readonly AssetReference VO_TheCurator_Male_Mech_LETL_Attack_02 = new AssetReference("VO_TheCurator_Male_Mech_LETL_Attack_02.prefab:04d0e6d4f626c5b4dbf09247c96abf6d");
  private static readonly AssetReference VO_TheCurator_Male_Mech_LETL_Death_01 = new AssetReference("VO_TheCurator_Male_Mech_LETL_Death_01.prefab:9ac49afd140927349befb4ef3ddb4ad9");
  private static readonly AssetReference VO_TheCurator_Male_Mech_LETL_Idle_01 = new AssetReference("VO_TheCurator_Male_Mech_LETL_Idle_01.prefab:67f7eba207c43cf489ac86a3f3832ca8");
  private static readonly AssetReference VO_TheCurator_Male_Mech_LETL_Idle_02 = new AssetReference("VO_TheCurator_Male_Mech_LETL_Idle_02.prefab:99dc89f8ecbb4794892e9eea3f1b341b");
  private static readonly AssetReference VO_TheCurator_Male_Mech_LETL_Intro_01 = new AssetReference("VO_TheCurator_Male_Mech_LETL_Intro_01.prefab:8f0a0733f41d65f40bf8d29c8bb72ed9");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Idle_01,
    (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Idle_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Intro_01,
      (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Attack_01,
      (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Attack_02,
      (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Idle_01,
      (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Idle_02,
      (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_817H_VoHandler lt24817HVoHandler = this;
    while (lt24817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24817HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_817H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_817H")
    {
      string str = cardID;
      if (!(str == "LT24_817P1"))
      {
        if (str == "LT24_817P2")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24817HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24817HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_817H_VoHandler.VO_TheCurator_Male_Mech_LETL_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_817H_VoHandler lt24817HVoHandler = this;
    while (lt24817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24817HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_817H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24817HVoHandler.MissionPlayVO(playByDesignCode, lt24817HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24817HVoHandler.MissionPlayVO(playByDesignCode, lt24817HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24817HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_817H_VoHandler lt24817HVoHandler = this;
    while (lt24817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24817HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_817H");
    if (entity.GetCardId() == "LT24_817H")
      yield return (object) lt24817HVoHandler.MissionPlaySound(playByDesignCode, lt24817HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_817H_VoHandler lt24817HVoHandler = this;
    while (lt24817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24817HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_817H");
    if (turn == 1)
      yield return (object) lt24817HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24817HVoHandler.m_introLine);
  }
}
