using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_849H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_RavakGrimtotem_Male_Tauren_Attack_01 = new AssetReference("VO_RavakGrimtotem_Male_Tauren_Attack_01.prefab:04b90286281623b40a39a02043b3824f");
  private static readonly AssetReference VO_RavakGrimtotem_Male_Tauren_Attack_02 = new AssetReference("VO_RavakGrimtotem_Male_Tauren_Attack_02.prefab:60d3946b33c590a49ac3def1016f9cf3");
  private static readonly AssetReference VO_RavakGrimtotem_Male_Tauren_Death_01 = new AssetReference("VO_RavakGrimtotem_Male_Tauren_Death_01.prefab:03622b1b601cdaf4f89edb67f9e697e2");
  private static readonly AssetReference VO_RavakGrimtotem_Male_Tauren_Idle_01 = new AssetReference("VO_RavakGrimtotem_Male_Tauren_Idle_01.prefab:36d69fd3299ee25488ca68dc8ad53202");
  private static readonly AssetReference VO_RavakGrimtotem_Male_Tauren_Intro_01 = new AssetReference("VO_RavakGrimtotem_Male_Tauren_Intro_01.prefab:1ef7373f761e7cd45b4d9e0e5f4974e6");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_849H_VoHandler.VO_RavakGrimtotem_Male_Tauren_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_849H_VoHandler.VO_RavakGrimtotem_Male_Tauren_Attack_01,
      (string) LettuceBoss_LETL_849H_VoHandler.VO_RavakGrimtotem_Male_Tauren_Attack_02,
      (string) LettuceBoss_LETL_849H_VoHandler.VO_RavakGrimtotem_Male_Tauren_Death_01,
      (string) LettuceBoss_LETL_849H_VoHandler.VO_RavakGrimtotem_Male_Tauren_Idle_01,
      (string) LettuceBoss_LETL_849H_VoHandler.VO_RavakGrimtotem_Male_Tauren_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_849H_VoHandler.VO_RavakGrimtotem_Male_Tauren_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_849H_VoHandler.VO_RavakGrimtotem_Male_Tauren_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_849H_VoHandler letl849HVoHandler = this;
    while (letl849HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl849HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_849H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_849H")
    {
      string str = cardID;
      if (!(str == "LETL_849P2_05") && !(str == "LETL_849P2_04"))
      {
        if (str == "LETL_849P5")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl849HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_849H_VoHandler.VO_RavakGrimtotem_Male_Tauren_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl849HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_849H_VoHandler.VO_RavakGrimtotem_Male_Tauren_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_849H_VoHandler letl849HVoHandler = this;
    while (letl849HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl849HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_849H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl849HVoHandler.MissionPlayVO(playByDesignCode, letl849HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl849HVoHandler.MissionPlayVO(playByDesignCode, letl849HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl849HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_849H_VoHandler letl849HVoHandler = this;
    while (letl849HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl849HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_849H");
    if (entity.GetCardId() == "LETL_849H")
      yield return (object) letl849HVoHandler.MissionPlaySound(playByDesignCode, letl849HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_849H_VoHandler letl849HVoHandler = this;
    while (letl849HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl849HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_849H");
    if (turn == 1)
      yield return (object) letl849HVoHandler.MissionPlayVOOnce(playByDesignCode, letl849HVoHandler.m_introLine);
  }
}
