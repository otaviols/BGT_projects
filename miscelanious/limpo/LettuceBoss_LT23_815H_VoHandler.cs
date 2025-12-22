using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_815H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_EmpressShekzara_Female_Mantid_LETL_Attack_02 = new AssetReference("VO_EmpressShekzara_Female_Mantid_LETL_Attack_02.prefab:8eb5a7d74717f4a4f959c59a19bfdf85");
  private static readonly AssetReference VO_EmpressShekzara_Female_Mantid_LETL_Attack_06 = new AssetReference("VO_EmpressShekzara_Female_Mantid_LETL_Attack_06.prefab:454b79078b8534447951ad5ebf4791a0");
  private static readonly AssetReference VO_EmpressShekzara_Female_Mantid_LETL_Death_01 = new AssetReference("VO_EmpressShekzara_Female_Mantid_LETL_Death_01.prefab:5f020428a1b886644abe43e4ca4c72e5");
  private static readonly AssetReference VO_EmpressShekzara_Female_Mantid_LETL_Idle_01 = new AssetReference("VO_EmpressShekzara_Female_Mantid_LETL_Idle_01.prefab:a2bb4606aa51152418320e6d621c03cf");
  private static readonly AssetReference VO_EmpressShekzara_Female_Mantid_LETL_Intro_01 = new AssetReference("VO_EmpressShekzara_Female_Mantid_LETL_Intro_01.prefab:8525dd76fb8f24f4986661863be34583");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_815H_VoHandler.VO_EmpressShekzara_Female_Mantid_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_815H_VoHandler.VO_EmpressShekzara_Female_Mantid_LETL_Intro_01,
      (string) LettuceBoss_LT23_815H_VoHandler.VO_EmpressShekzara_Female_Mantid_LETL_Attack_02,
      (string) LettuceBoss_LT23_815H_VoHandler.VO_EmpressShekzara_Female_Mantid_LETL_Attack_06,
      (string) LettuceBoss_LT23_815H_VoHandler.VO_EmpressShekzara_Female_Mantid_LETL_Idle_01,
      (string) LettuceBoss_LT23_815H_VoHandler.VO_EmpressShekzara_Female_Mantid_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_815H_VoHandler.VO_EmpressShekzara_Female_Mantid_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_815H_VoHandler.VO_EmpressShekzara_Female_Mantid_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_815H_VoHandler lt23815HVoHandler = this;
    while (lt23815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23815HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_815H");
    lt23815HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_815H3");
    string cardId = playedEntity.GetLettuceAbilityOwner().GetCardId();
    if (cardId == "LT23_815H")
    {
      if (cardID == "LT23_815P1")
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23815HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_815H_VoHandler.VO_EmpressShekzara_Female_Mantid_LETL_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
    else if (cardId == "LT23_815H3" && cardID == "LT23_815P2")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23815HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_815H_VoHandler.VO_EmpressShekzara_Female_Mantid_LETL_Attack_06);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_815H_VoHandler lt23815HVoHandler = this;
    while (lt23815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23815HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_815H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23815HVoHandler.MissionPlayVO(playByDesignCode, lt23815HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23815HVoHandler.MissionPlayVO(playByDesignCode, lt23815HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23815HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_815H_VoHandler lt23815HVoHandler = this;
    while (lt23815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23815HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_815H");
    if (entity.GetCardId() == "LT23_815H")
      yield return (object) lt23815HVoHandler.MissionPlaySound(playByDesignCode, lt23815HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_815H_VoHandler lt23815HVoHandler = this;
    while (lt23815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23815HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_815H");
    if (turn == 1)
      yield return (object) lt23815HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23815HVoHandler.m_introLine);
  }
}
