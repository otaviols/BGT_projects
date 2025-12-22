using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_866H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Mida_Female_Naaru_LETL_Intro_01 = new AssetReference("VO_Mida_Female_Naaru_LETL_Intro_01.prefab:00f3be348907ca94db4c49c4225d85a0");
  private static readonly AssetReference VO_Mida_Female_Naaru_LETL_Idle_03 = new AssetReference("VO_Mida_Female_Naaru_LETL_Idle_03.prefab:6fa7b23a5cd77af4f91ae2f18b6c880d");
  private static readonly AssetReference VO_Mida_Female_Naaru_LETL_Idle_02 = new AssetReference("VO_Mida_Female_Naaru_LETL_Idle_02.prefab:e1d8614bcb6cc694ebeb5c93608ea050");
  private static readonly AssetReference VO_Mida_Female_Naaru_LETL_Idle_01 = new AssetReference("VO_Mida_Female_Naaru_LETL_Idle_01.prefab:66679ed14d285f841a7f8830cc1f2b55");
  private static readonly AssetReference VO_Mida_Female_Naaru_LETL_Death_01 = new AssetReference("VO_Mida_Female_Naaru_LETL_Death_01.prefab:1c039ebea8a6f734ba5b9cf2618a6cef");
  private static readonly AssetReference VO_Mida_Female_Naaru_LETL_Ability_02 = new AssetReference("VO_Mida_Female_Naaru_LETL_Ability_02.prefab:1704cffae7bb9ff4c93b7b4e60c89ff1");
  private static readonly AssetReference VO_Mida_Female_Naaru_LETL_Ability_01 = new AssetReference("VO_Mida_Female_Naaru_LETL_Ability_01.prefab:be18da211e0914642aebed0c921e2019");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Idle_03,
    (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Idle_02,
    (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Intro_01,
      (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Idle_03,
      (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Idle_02,
      (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Idle_01,
      (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Death_01,
      (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Ability_02,
      (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Ability_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_866H_VoHandler letl866HVoHandler = this;
    while (letl866HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl866HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_866H");
    string cardId = playedEntity.GetLettuceAbilityOwner().GetCardId();
    if (cardId == "LETL_866H")
    {
      if (cardID == "LETL_866P1_01")
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl866HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Ability_02);
        GameState.Get().SetBusy(false);
      }
    }
    else if (cardId == "LETL_866H" && cardID == "LETL_866P2_01")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl866HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_866H_VoHandler.VO_Mida_Female_Naaru_LETL_Ability_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_866H_VoHandler letl866HVoHandler = this;
    while (letl866HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl866HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_866H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl866HVoHandler.MissionPlayVO(playByDesignCode, letl866HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl866HVoHandler.MissionPlayVO(playByDesignCode, letl866HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl866HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_866H_VoHandler letl866HVoHandler = this;
    while (letl866HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl866HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_866H");
    if (entity.GetCardId() == "LETL_866H")
      yield return (object) letl866HVoHandler.MissionPlaySound(playByDesignCode, letl866HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_866H_VoHandler letl866HVoHandler = this;
    while (letl866HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl866HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_866H");
    if (turn == 1)
      yield return (object) letl866HVoHandler.MissionPlayVOOnce(playByDesignCode, letl866HVoHandler.m_introLine);
  }
}
