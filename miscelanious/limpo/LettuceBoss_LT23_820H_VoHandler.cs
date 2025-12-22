using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_820H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference DMF_070_DarkmoonRabbit_Attack = new AssetReference("DMF_070_DarkmoonRabbit_Attack.prefab:a488f54e9eb7c994eb7183081309a736");
  private static readonly AssetReference DMF_070_DarkmoonRabbit_Death = new AssetReference("DMF_070_DarkmoonRabbit_Death.prefab:29fe5ce453638aa46bd60d6b0a8f1d67");
  private static readonly AssetReference DMF_070_DarkmoonRabbit_Play = new AssetReference("DMF_070_DarkmoonRabbit_Play.prefab:80021d88e5bec2549b4753ea9d8c7a65");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_820H_VoHandler.DMF_070_DarkmoonRabbit_Play
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_820H_VoHandler.DMF_070_DarkmoonRabbit_Play,
      (string) LettuceBoss_LT23_820H_VoHandler.DMF_070_DarkmoonRabbit_Attack,
      (string) LettuceBoss_LT23_820H_VoHandler.DMF_070_DarkmoonRabbit_Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_820H_VoHandler.DMF_070_DarkmoonRabbit_Play;
    this.m_deathLine = (string) LettuceBoss_LT23_820H_VoHandler.DMF_070_DarkmoonRabbit_Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_820H_VoHandler lt23820HVoHandler = this;
    while (lt23820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23820HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_820H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_820H" && cardID == "LETLT_118_02")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23820HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_820H_VoHandler.DMF_070_DarkmoonRabbit_Attack);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_820H_VoHandler lt23820HVoHandler = this;
    while (lt23820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23820HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_820H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23820HVoHandler.MissionPlayVO(playByDesignCode, lt23820HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23820HVoHandler.MissionPlayVO(playByDesignCode, lt23820HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23820HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_820H_VoHandler lt23820HVoHandler = this;
    while (lt23820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23820HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_820H");
    if (entity.GetCardId() == "LT23_820H")
      yield return (object) lt23820HVoHandler.MissionPlaySound(playByDesignCode, lt23820HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_820H_VoHandler lt23820HVoHandler = this;
    while (lt23820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23820HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_820H");
    if (turn == 1)
      yield return (object) lt23820HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23820HVoHandler.m_introLine);
  }
}
