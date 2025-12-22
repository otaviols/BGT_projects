using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_821H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_821H_Male_Goblin_Attack_01 = new AssetReference("VO_LETL_821H_Male_Goblin_Attack_01.prefab:36e96a6853dabe04ca0705dc010b0731");
  private static readonly AssetReference VO_LETL_821H_Male_Goblin_Attack_02 = new AssetReference("VO_LETL_821H_Male_Goblin_Attack_02.prefab:e3b443fc732ece448a830702fe4400c0");
  private static readonly AssetReference VO_LETL_821H_Male_Goblin_Death_01 = new AssetReference("VO_LETL_821H_Male_Goblin_Death_01.prefab:3fb6a14829378dd40b802f2b9dd3cb17");
  private static readonly AssetReference VO_LETL_821H_Male_Goblin_Idle_01 = new AssetReference("VO_LETL_821H_Male_Goblin_Idle_01.prefab:1ed0b24571a0e1e439b840195f224499");
  private static readonly AssetReference VO_LETL_821H_Male_Goblin_Intro_01 = new AssetReference("VO_LETL_821H_Male_Goblin_Intro_01.prefab:e14a156414339c840a640a87a4ffab08");
  private static readonly AssetReference Death = new AssetReference("Death.prefab:f08e4880fab48f3468d9f3b778d2aea7");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_821H_VoHandler.VO_LETL_821H_Male_Goblin_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_821H_VoHandler.VO_LETL_821H_Male_Goblin_Attack_01,
      (string) LettuceBoss_LETL_821H_VoHandler.VO_LETL_821H_Male_Goblin_Attack_02,
      (string) LettuceBoss_LETL_821H_VoHandler.VO_LETL_821H_Male_Goblin_Idle_01,
      (string) LettuceBoss_LETL_821H_VoHandler.VO_LETL_821H_Male_Goblin_Intro_01,
      (string) LettuceBoss_LETL_821H_VoHandler.Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_821H_VoHandler.VO_LETL_821H_Male_Goblin_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_821H_VoHandler.Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_821H_VoHandler letl821HVoHandler = this;
    while (letl821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl821HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_821H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_821H")
    {
      string str = cardID;
      if (!(str == "LETL_821P1_02"))
      {
        if (str == "LETL_516_02")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl821HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_821H_VoHandler.VO_LETL_821H_Male_Goblin_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl821HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_821H_VoHandler.VO_LETL_821H_Male_Goblin_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_821H_VoHandler letl821HVoHandler = this;
    while (letl821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl821HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_821H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl821HVoHandler.MissionPlayVO(playByDesignCode, letl821HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl821HVoHandler.MissionPlayVO(playByDesignCode, letl821HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl821HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_821H_VoHandler letl821HVoHandler = this;
    while (letl821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl821HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_821H");
    if (entity.GetCardId() == "LETL_821H")
      yield return (object) letl821HVoHandler.MissionPlaySound(playByDesignCode, letl821HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_821H_VoHandler letl821HVoHandler = this;
    while (letl821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl821HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_821H");
    if (turn == 1)
      yield return (object) letl821HVoHandler.MissionPlayVOOnce(playByDesignCode, letl821HVoHandler.m_introLine);
  }
}
