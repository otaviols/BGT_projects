using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_835H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_835H_Male_Elemental_Attack_01 = new AssetReference("VO_LETL_835H_Male_Elemental_Attack_01.prefab:4d133d250746fc2469efd49962d52383");
  private static readonly AssetReference VO_LETL_835H_Male_Elemental_Attack_02 = new AssetReference("VO_LETL_835H_Male_Elemental_Attack_02.prefab:f6478eda37755454b87f3168bde0ae3b");
  private static readonly AssetReference VO_LETL_835H_Male_Elemental_Death_01 = new AssetReference("VO_LETL_835H_Male_Elemental_Death_01.prefab:978a33228da84e542a7426da7b229643");
  private static readonly AssetReference VO_LETL_835H_Male_Elemental_Idle_01 = new AssetReference("VO_LETL_835H_Male_Elemental_Idle_01.prefab:7cdef781eadc7f24f8db8b9a085ee72c");
  private static readonly AssetReference VO_LETL_835H_Male_Elemental_Intro_01 = new AssetReference("VO_LETL_835H_Male_Elemental_Intro_01.prefab:f1392c836ff71254d80ab87bb4c3d256");
  private static readonly AssetReference VO_LETL_835H_Male_Elemental_Intro_02 = new AssetReference("VO_LETL_835H_Male_Elemental_Intro_02.prefab:d311daa691097384a9e3f369807d0fa2");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Attack_01,
      (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Attack_02,
      (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Death_01,
      (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Idle_01,
      (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Intro_01,
      (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Intro_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_835H_VoHandler letl835HVoHandler = this;
    while (letl835HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl835HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_835H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_835H")
    {
      string str = cardID;
      if (!(str == "LETL_835P1_01") && !(str == "LETL_835P1_02"))
      {
        if (str == "LETL_835P2_01" || str == "LETL_835P2_03")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl835HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl835HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_835H_VoHandler.VO_LETL_835H_Male_Elemental_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_835H_VoHandler letl835HVoHandler = this;
    while (letl835HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl835HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_835H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl835HVoHandler.MissionPlayVO(playByDesignCode, letl835HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl835HVoHandler.MissionPlayVO(playByDesignCode, letl835HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl835HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_835H_VoHandler letl835HVoHandler = this;
    while (letl835HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl835HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_835H");
    if (entity.GetCardId() == "LETL_835H")
      yield return (object) letl835HVoHandler.MissionPlaySound(playByDesignCode, letl835HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_835H_VoHandler letl835HVoHandler = this;
    while (letl835HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl835HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_835H");
    if (turn == 1)
      yield return (object) letl835HVoHandler.MissionPlayVOOnce(playByDesignCode, letl835HVoHandler.m_introLine);
  }
}
