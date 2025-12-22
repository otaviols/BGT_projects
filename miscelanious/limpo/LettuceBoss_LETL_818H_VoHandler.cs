using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_818H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_818H_Male_Undead_Attack_01 = new AssetReference("VO_LETL_818H_Male_Undead_Attack_01.prefab:de0c7500417b65e409b7d57d05997302");
  private static readonly AssetReference VO_LETL_818H_Male_Undead_Attack_02 = new AssetReference("VO_LETL_818H_Male_Undead_Attack_02.prefab:8f5dc726acb7ecc409af1a63621bf6a0");
  private static readonly AssetReference VO_LETL_818H_Male_Undead_Death_01 = new AssetReference("VO_LETL_818H_Male_Undead_Death_01.prefab:96eae087ad449fe44b02ec9a5ce52c2a");
  private static readonly AssetReference VO_LETL_818H_Male_Undead_Idle_01 = new AssetReference("VO_LETL_818H_Male_Undead_Idle_01.prefab:1dc2a7b22e03894468bd547cd436d337");
  private static readonly AssetReference VO_LETL_818H_Male_Undead_Idle_02 = new AssetReference("VO_LETL_818H_Male_Undead_Idle_02.prefab:ed0d5f982f037d84b8c604e5db6831e0");
  private static readonly AssetReference VO_LETL_818H_Male_Undead_Intro_01 = new AssetReference("VO_LETL_818H_Male_Undead_Intro_01.prefab:c253249feecde494fa877b6224a52dd2");
  private static readonly AssetReference VO_LETL_818H_Male_Undead_Intro_02 = new AssetReference("VO_LETL_818H_Male_Undead_Intro_02.prefab:4304ddb2dfff225418212626151f9a3e");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Idle_01,
    (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Idle_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Attack_01,
      (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Attack_02,
      (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Death_01,
      (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Idle_01,
      (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Idle_02,
      (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Intro_01,
      (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Intro_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_818H_VoHandler letl818HVoHandler = this;
    while (letl818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl818HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_818H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_818H")
    {
      string str = cardID;
      if (!(str == "LETL_818P1_01"))
      {
        if (str == "LETL_818P2_01" || str == "LETL_818P3_01")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl818HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl818HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_818H_VoHandler.VO_LETL_818H_Male_Undead_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_818H_VoHandler letl818HVoHandler = this;
    while (letl818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl818HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_818H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl818HVoHandler.MissionPlayVO(playByDesignCode, letl818HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl818HVoHandler.MissionPlayVO(playByDesignCode, letl818HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl818HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_818H_VoHandler letl818HVoHandler = this;
    while (letl818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl818HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_818H");
    if (entity.GetCardId() == "LETL_818H")
      yield return (object) letl818HVoHandler.MissionPlaySound(playByDesignCode, letl818HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_818H_VoHandler letl818HVoHandler = this;
    while (letl818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl818HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_818H");
    if (turn == 1)
      yield return (object) letl818HVoHandler.MissionPlayVOOnce(playByDesignCode, letl818HVoHandler.m_introLine);
  }
}
