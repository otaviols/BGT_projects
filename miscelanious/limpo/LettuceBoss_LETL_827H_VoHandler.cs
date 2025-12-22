using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_827H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_827H_Male_Satyr_Attack_01 = new AssetReference("VO_LETL_827H_Male_Satyr_Attack_01.prefab:f1adae6989959ea45967a4ffbd3e49ea");
  private static readonly AssetReference VO_LETL_827H_Male_Satyr_Attack_02 = new AssetReference("VO_LETL_827H_Male_Satyr_Attack_02.prefab:53d135560ef09cb4dba5f87145d86b78");
  private static readonly AssetReference VO_LETL_827H_Male_Satyr_Death_01 = new AssetReference("VO_LETL_827H_Male_Satyr_Death_01.prefab:d2470aeb83d83da458e72089b031b5ce");
  private static readonly AssetReference VO_LETL_827H_Male_Satyr_Idle_01 = new AssetReference("VO_LETL_827H_Male_Satyr_Idle_01.prefab:774dde4d0fcad194cbdac154ccb52599");
  private static readonly AssetReference VO_LETL_827H_Male_Satyr_Intro_01 = new AssetReference("VO_LETL_827H_Male_Satyr_Intro_01.prefab:c2ad2e1bc00e3624fb966195753ff3dc");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_827H_VoHandler.VO_LETL_827H_Male_Satyr_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_827H_VoHandler.VO_LETL_827H_Male_Satyr_Attack_01,
      (string) LettuceBoss_LETL_827H_VoHandler.VO_LETL_827H_Male_Satyr_Attack_02,
      (string) LettuceBoss_LETL_827H_VoHandler.VO_LETL_827H_Male_Satyr_Death_01,
      (string) LettuceBoss_LETL_827H_VoHandler.VO_LETL_827H_Male_Satyr_Idle_01,
      (string) LettuceBoss_LETL_827H_VoHandler.VO_LETL_827H_Male_Satyr_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_827H_VoHandler.VO_LETL_827H_Male_Satyr_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_827H_VoHandler.VO_LETL_827H_Male_Satyr_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_827H_VoHandler letl827HVoHandler = this;
    while (letl827HVoHandler.m_enemySpeaking)
      yield return (object) null;
    letl827HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_827H");
    Entity lettuceAbilityOwner = playedEntity.GetLettuceAbilityOwner();
    Actor actor = lettuceAbilityOwner.GetCard().GetActor();
    if (lettuceAbilityOwner.GetCardId() == "LETL_827H")
    {
      string str = cardID;
      if (!(str == "LETL_827P1_01") && !(str == "LETL_827P1_05") && !(str == "LETL_257_03") && !(str == "LETL_257_05"))
      {
        if (str == "LETL_009P9_01" || str == "LETL_009P9_03")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl827HVoHandler.MissionPlayVOOnce(actor, (string) LettuceBoss_LETL_827H_VoHandler.VO_LETL_827H_Male_Satyr_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl827HVoHandler.MissionPlayVOOnce(actor, (string) LettuceBoss_LETL_827H_VoHandler.VO_LETL_827H_Male_Satyr_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_827H_VoHandler letl827HVoHandler = this;
    while (letl827HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl827HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_827H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl827HVoHandler.MissionPlayVO(playByDesignCode, letl827HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl827HVoHandler.MissionPlayVO(playByDesignCode, letl827HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl827HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_827H_VoHandler letl827HVoHandler = this;
    while (letl827HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl827HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_827H");
    if (entity.GetCardId() == "LETL_827H")
      yield return (object) letl827HVoHandler.MissionPlaySound(playByDesignCode, letl827HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_827H_VoHandler letl827HVoHandler = this;
    while (letl827HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl827HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_827H");
    if (turn == 1)
      yield return (object) letl827HVoHandler.MissionPlayVOOnce(playByDesignCode, letl827HVoHandler.m_introLine);
  }
}
