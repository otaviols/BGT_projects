using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_834H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference LETL_833H_Attack_01 = new AssetReference("LETL_833H_Attack_01.prefab:e76fcb9c88febcc4bb798121706f9e30");
  private static readonly AssetReference LETL_833H_Death_01 = new AssetReference("LETL_833H_Death_01.prefab:e54c160a014371444a8d26ab25f637a9");
  private static readonly AssetReference LETL_833H_Intro_01 = new AssetReference("LETL_833H_Intro_01.prefab:37071039ab31c7f4b8845641e0ec8a84");
  private List<string> m_IdleLines = new List<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_834H_VoHandler.LETL_833H_Attack_01,
      (string) LettuceBoss_LETL_834H_VoHandler.LETL_833H_Death_01,
      (string) LettuceBoss_LETL_834H_VoHandler.LETL_833H_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_834H_VoHandler.LETL_833H_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_834H_VoHandler.LETL_833H_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_834H_VoHandler letl834HVoHandler = this;
    while (letl834HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl834HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_834H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_834H")
    {
      string str = cardID;
      if (str == "LETL_8342P1" || str == "LETL_8342P2" || str == "LETL_8342P3")
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl834HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_834H_VoHandler.LETL_833H_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_834H_VoHandler letl834HVoHandler = this;
    while (letl834HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl834HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_834H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl834HVoHandler.MissionPlayVO(playByDesignCode, letl834HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl834HVoHandler.MissionPlayVO(playByDesignCode, letl834HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl834HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_834H_VoHandler letl834HVoHandler = this;
    while (letl834HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl834HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_834H");
    if (entity.GetCardId() == "LETL_834H")
      yield return (object) letl834HVoHandler.MissionPlaySound(playByDesignCode, letl834HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_834H_VoHandler letl834HVoHandler = this;
    while (letl834HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl834HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_834H");
    if (turn == 1)
      yield return (object) letl834HVoHandler.MissionPlayVOOnce(playByDesignCode, letl834HVoHandler.m_introLine);
  }
}
