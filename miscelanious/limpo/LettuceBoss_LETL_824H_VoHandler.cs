using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_824H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_824H_Male_Furbolg_Attack_01 = new AssetReference("VO_LETL_824H_Male_Furbolg_Attack_01.prefab:e2a6c3b3556edfc44b40e34ee4f1ae7a");
  private static readonly AssetReference VO_LETL_824H_Male_Furbolg_Attack_02 = new AssetReference("VO_LETL_824H_Male_Furbolg_Attack_02.prefab:21e87934c573de64cbcfac43c6616096");
  private static readonly AssetReference VO_LETL_824H_Male_Furbolg_Death_01 = new AssetReference("VO_LETL_824H_Male_Furbolg_Death_01.prefab:88cf91b7cd5685d4e8dcc1d74ff621af");
  private static readonly AssetReference VO_LETL_824H_Male_Furbolg_Idle_01 = new AssetReference("VO_LETL_824H_Male_Furbolg_Idle_01.prefab:7dfd4fef40026e34b820595c14e8fbcc");
  private static readonly AssetReference VO_LETL_824H_Male_Furbolg_Intro_01 = new AssetReference("VO_LETL_824H_Male_Furbolg_Intro_01.prefab:cc2be53dff13ddf499b42ee90b8a86e6");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_824H_VoHandler.VO_LETL_824H_Male_Furbolg_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_824H_VoHandler.VO_LETL_824H_Male_Furbolg_Attack_01,
      (string) LettuceBoss_LETL_824H_VoHandler.VO_LETL_824H_Male_Furbolg_Attack_02,
      (string) LettuceBoss_LETL_824H_VoHandler.VO_LETL_824H_Male_Furbolg_Death_01,
      (string) LettuceBoss_LETL_824H_VoHandler.VO_LETL_824H_Male_Furbolg_Idle_01,
      (string) LettuceBoss_LETL_824H_VoHandler.VO_LETL_824H_Male_Furbolg_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_824H_VoHandler.VO_LETL_824H_Male_Furbolg_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_824H_VoHandler.VO_LETL_824H_Male_Furbolg_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_824H_VoHandler letl824HVoHandler = this;
    while (letl824HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl824HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_824H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_824H")
    {
      string str = cardID;
      if (!(str == "LETL_824P1_01"))
      {
        if (str == "LETL_82P3_02")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl824HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_824H_VoHandler.VO_LETL_824H_Male_Furbolg_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl824HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_824H_VoHandler.VO_LETL_824H_Male_Furbolg_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_824H_VoHandler letl824HVoHandler = this;
    while (letl824HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl824HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_824H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl824HVoHandler.MissionPlayVO(playByDesignCode, letl824HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl824HVoHandler.MissionPlayVO(playByDesignCode, letl824HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl824HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_824H_VoHandler letl824HVoHandler = this;
    while (letl824HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl824HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_824H");
    if (entity.GetCardId() == "LETL_824H")
      yield return (object) letl824HVoHandler.MissionPlaySound(playByDesignCode, letl824HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_824H_VoHandler letl824HVoHandler = this;
    while (letl824HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl824HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_824H");
    if (turn == 1)
      yield return (object) letl824HVoHandler.MissionPlayVOOnce(playByDesignCode, letl824HVoHandler.m_introLine);
  }
}
