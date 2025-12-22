using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_825H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_825H_Male_NightElf_Attack_01 = new AssetReference("VO_LETL_825H_Male_NightElf_Attack_01.prefab:1ff5afec68209014b9d85342a90211c5");
  private static readonly AssetReference VO_LETL_825H_Male_NightElf_Attack_02 = new AssetReference("VO_LETL_825H_Male_NightElf_Attack_02.prefab:03361106491539b4bb6981c48a715458");
  private static readonly AssetReference VO_LETL_825H_Male_NightElf_Death_01 = new AssetReference("VO_LETL_825H_Male_NightElf_Death_01.prefab:de5f6dfee266ea443b35048de5195b85");
  private static readonly AssetReference VO_LETL_825H_Male_NightElf_Idle_01 = new AssetReference("VO_LETL_825H_Male_NightElf_Idle_01.prefab:4918879783c682149b0ab19545120d94");
  private static readonly AssetReference VO_LETL_825H_Male_NightElf_Intro_01 = new AssetReference("VO_LETL_825H_Male_NightElf_Intro_01.prefab:defcdae7295849c418dc490ec0977659");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_825H_VoHandler.VO_LETL_825H_Male_NightElf_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_825H_VoHandler.VO_LETL_825H_Male_NightElf_Attack_01,
      (string) LettuceBoss_LETL_825H_VoHandler.VO_LETL_825H_Male_NightElf_Attack_02,
      (string) LettuceBoss_LETL_825H_VoHandler.VO_LETL_825H_Male_NightElf_Death_01,
      (string) LettuceBoss_LETL_825H_VoHandler.VO_LETL_825H_Male_NightElf_Idle_01,
      (string) LettuceBoss_LETL_825H_VoHandler.VO_LETL_825H_Male_NightElf_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_825H_VoHandler.VO_LETL_825H_Male_NightElf_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_825H_VoHandler.VO_LETL_825H_Male_NightElf_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_825H_VoHandler letl825HVoHandler = this;
    while (letl825HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl825HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_825H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_825H")
    {
      string str = cardID;
      if (!(str == "LETL_003P1_02") && !(str == "LETL_003P1_04") && !(str == "LETL_412_02") && !(str == "LETL_412_05"))
      {
        if (str == "LETL_019P1_03" || str == "LETL_019P1_05")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl825HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_825H_VoHandler.VO_LETL_825H_Male_NightElf_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl825HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_825H_VoHandler.VO_LETL_825H_Male_NightElf_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_825H_VoHandler letl825HVoHandler = this;
    while (letl825HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl825HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_825H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl825HVoHandler.MissionPlayVO(playByDesignCode, letl825HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl825HVoHandler.MissionPlayVO(playByDesignCode, letl825HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl825HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_825H_VoHandler letl825HVoHandler = this;
    while (letl825HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl825HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_825H");
    if (entity.GetCardId() == "LETL_825H")
      yield return (object) letl825HVoHandler.MissionPlaySound(playByDesignCode, letl825HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_825H_VoHandler letl825HVoHandler = this;
    while (letl825HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl825HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_825H");
    if (turn == 1)
      yield return (object) letl825HVoHandler.MissionPlayVOOnce(playByDesignCode, letl825HVoHandler.m_introLine);
  }
}
