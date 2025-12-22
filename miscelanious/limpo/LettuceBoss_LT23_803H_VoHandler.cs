using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_803H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_CaptainShivers_Male_Human_Attack_01 = new AssetReference("VO_CaptainShivers_Male_Human_Attack_01.prefab:3c25aeb1338cf7444a3abbf13dcd0c46");
  private static readonly AssetReference VO_CaptainShivers_Male_Human_Death_01 = new AssetReference("VO_CaptainShivers_Male_Human_Death_01.prefab:bd6541155bd0a9c4284825ad2db91b5a");
  private static readonly AssetReference VO_CaptainShivers_Male_Human_Idle_01 = new AssetReference("VO_CaptainShivers_Male_Human_Idle_01.prefab:8fc3b272745000043a3ebb2896b58c7f");
  private static readonly AssetReference VO_CaptainShivers_Male_Human_Intro_01 = new AssetReference("VO_CaptainShivers_Male_Human_Intro_01.prefab:b979d11e222915844ba95b4690328e58");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_803H_VoHandler.VO_CaptainShivers_Male_Human_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_803H_VoHandler.VO_CaptainShivers_Male_Human_Attack_01,
      (string) LettuceBoss_LT23_803H_VoHandler.VO_CaptainShivers_Male_Human_Death_01,
      (string) LettuceBoss_LT23_803H_VoHandler.VO_CaptainShivers_Male_Human_Idle_01,
      (string) LettuceBoss_LT23_803H_VoHandler.VO_CaptainShivers_Male_Human_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_803H_VoHandler.VO_CaptainShivers_Male_Human_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_803H_VoHandler.VO_CaptainShivers_Male_Human_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_803H_VoHandler lt23803HVoHandler = this;
    while (lt23803HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23803HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_803H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_803H" && cardID == "LT23_803P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23803HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_803H_VoHandler.VO_CaptainShivers_Male_Human_Attack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_803H_VoHandler lt23803HVoHandler = this;
    while (lt23803HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23803HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_803H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23803HVoHandler.MissionPlayVO(playByDesignCode, lt23803HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23803HVoHandler.MissionPlayVO(playByDesignCode, lt23803HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23803HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_803H_VoHandler lt23803HVoHandler = this;
    while (lt23803HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23803HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_803H");
    if (entity.GetCardId() == "LT23_803H")
      yield return (object) lt23803HVoHandler.MissionPlaySound(playByDesignCode, lt23803HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_803H_VoHandler lt23803HVoHandler = this;
    while (lt23803HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23803HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_803H");
    if (turn == 1)
      yield return (object) lt23803HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23803HVoHandler.m_introLine);
  }
}
