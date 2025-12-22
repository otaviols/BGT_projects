using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_822H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_ImageOfMedivh_Male_Human_LETL_Attack_01 = new AssetReference("VO_ImageOfMedivh_Male_Human_LETL_Attack_01.prefab:a821189f68a733b4e89b4b91ba83b31b");
  private static readonly AssetReference VO_ImageOfMedivh_Male_Human_LETL_Attack_02 = new AssetReference("VO_ImageOfMedivh_Male_Human_LETL_Attack_02.prefab:6088ff795e1e6104a9123c53aca402c0");
  private static readonly AssetReference VO_ImageOfMedivh_Male_Human_LETL_Death_01 = new AssetReference("VO_ImageOfMedivh_Male_Human_LETL_Death_01.prefab:db536a843f699094d8740ffbfcb4770d");
  private static readonly AssetReference VO_ImageOfMedivh_Male_Human_LETL_Idle_01 = new AssetReference("VO_ImageOfMedivh_Male_Human_LETL_Idle_01.prefab:5ad12130d18a1594785cc22bd0a283b4");
  private static readonly AssetReference VO_ImageOfMedivh_Male_Human_LETL_Intro_01 = new AssetReference("VO_ImageOfMedivh_Male_Human_LETL_Intro_01.prefab:4c012cfe8eeae1b4aaa391a1f16030e9");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_822H_VoHandler.VO_ImageOfMedivh_Male_Human_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_822H_VoHandler.VO_ImageOfMedivh_Male_Human_LETL_Intro_01,
      (string) LettuceBoss_LT24_822H_VoHandler.VO_ImageOfMedivh_Male_Human_LETL_Attack_01,
      (string) LettuceBoss_LT24_822H_VoHandler.VO_ImageOfMedivh_Male_Human_LETL_Attack_02,
      (string) LettuceBoss_LT24_822H_VoHandler.VO_ImageOfMedivh_Male_Human_LETL_Idle_01,
      (string) LettuceBoss_LT24_822H_VoHandler.VO_ImageOfMedivh_Male_Human_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_822H_VoHandler.VO_ImageOfMedivh_Male_Human_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT24_822H_VoHandler.VO_ImageOfMedivh_Male_Human_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_822H_VoHandler lt24822HVoHandler = this;
    while (lt24822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24822HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_822H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_822H")
    {
      string str = cardID;
      if (!(str == "LT24_822P1"))
      {
        if (str == "LT24_822P2")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24822HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_822H_VoHandler.VO_ImageOfMedivh_Male_Human_LETL_Attack_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24822HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_822H_VoHandler.VO_ImageOfMedivh_Male_Human_LETL_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_822H_VoHandler lt24822HVoHandler = this;
    while (lt24822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24822HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_822H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24822HVoHandler.MissionPlayVO(playByDesignCode, lt24822HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24822HVoHandler.MissionPlayVO(playByDesignCode, lt24822HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24822HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_822H_VoHandler lt24822HVoHandler = this;
    while (lt24822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24822HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_822H");
    if (entity.GetCardId() == "LT24_822H")
      yield return (object) lt24822HVoHandler.MissionPlaySound(playByDesignCode, lt24822HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_822H_VoHandler lt24822HVoHandler = this;
    while (lt24822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24822HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_822H");
    if (turn == 1)
      yield return (object) lt24822HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24822HVoHandler.m_introLine);
  }
}
