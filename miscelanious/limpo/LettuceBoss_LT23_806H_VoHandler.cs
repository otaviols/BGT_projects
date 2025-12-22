using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_806H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference WC_026_KreshLordofTurtlin_Attack = new AssetReference("WC_026_KreshLordofTurtlin_Attack.prefab:129b5c8bf04306a4c986afb085f30fda");
  private static readonly AssetReference WC_026_KreshLordofTurtlin_Death = new AssetReference("WC_026_KreshLordofTurtlin_Death.prefab:003900f61abaf0849b9960bc6c96caba");
  private static readonly AssetReference WC_026_KreshLordofTurtlin_Play = new AssetReference("WC_026_KreshLordofTurtlin_Play.prefab:f58cd70d59197454f9d2e09bfcc50bda");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_806H_VoHandler.WC_026_KreshLordofTurtlin_Play
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_806H_VoHandler.WC_026_KreshLordofTurtlin_Play,
      (string) LettuceBoss_LT23_806H_VoHandler.WC_026_KreshLordofTurtlin_Attack,
      (string) LettuceBoss_LT23_806H_VoHandler.WC_026_KreshLordofTurtlin_Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_806H_VoHandler.WC_026_KreshLordofTurtlin_Play;
    this.m_deathLine = (string) LettuceBoss_LT23_806H_VoHandler.WC_026_KreshLordofTurtlin_Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_806H_VoHandler lt23806HVoHandler = this;
    while (lt23806HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23806HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_806H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_806H")
    {
      string str = cardID;
      if (!(str == "LT23_806P1"))
      {
        if (str == "LT23_806P2")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt23806HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_806H_VoHandler.WC_026_KreshLordofTurtlin_Attack);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23806HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_806H_VoHandler.WC_026_KreshLordofTurtlin_Attack);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_806H_VoHandler lt23806HVoHandler = this;
    while (lt23806HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23806HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_806H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23806HVoHandler.MissionPlayVO(playByDesignCode, lt23806HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23806HVoHandler.MissionPlayVO(playByDesignCode, lt23806HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23806HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_806H_VoHandler lt23806HVoHandler = this;
    while (lt23806HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23806HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_806H");
    if (entity.GetCardId() == "LT23_806H")
      yield return (object) lt23806HVoHandler.MissionPlaySound(playByDesignCode, lt23806HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_806H_VoHandler lt23806HVoHandler = this;
    while (lt23806HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23806HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_806H");
    if (turn == 1)
      yield return (object) lt23806HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23806HVoHandler.m_introLine);
  }
}
