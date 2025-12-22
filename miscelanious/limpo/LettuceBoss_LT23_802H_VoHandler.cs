using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_802H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LadyAlisstra_Female_Naga_Attack_01 = new AssetReference("VO_LadyAlisstra_Female_Naga_Attack_01.prefab:3a715ab13b403df4697aaf2ae1527aa2");
  private static readonly AssetReference VO_LadyAlisstra_Female_Naga_Death_01 = new AssetReference("VO_LadyAlisstra_Female_Naga_Death_01.prefab:bc08263bb78b56847a46aee46c3915a1");
  private static readonly AssetReference VO_LadyAlisstra_Female_Naga_Idle_01 = new AssetReference("VO_LadyAlisstra_Female_Naga_Idle_01.prefab:832c6cbf53d71074d9d38274641ef024");
  private static readonly AssetReference VO_LadyAlisstra_Female_Naga_Intro_01 = new AssetReference("VO_LadyAlisstra_Female_Naga_Intro_01.prefab:39e0cf37548eed342af52f99bb6a5cc0");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_802H_VoHandler.VO_LadyAlisstra_Female_Naga_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_802H_VoHandler.VO_LadyAlisstra_Female_Naga_Attack_01,
      (string) LettuceBoss_LT23_802H_VoHandler.VO_LadyAlisstra_Female_Naga_Death_01,
      (string) LettuceBoss_LT23_802H_VoHandler.VO_LadyAlisstra_Female_Naga_Idle_01,
      (string) LettuceBoss_LT23_802H_VoHandler.VO_LadyAlisstra_Female_Naga_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_802H_VoHandler.VO_LadyAlisstra_Female_Naga_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_802H_VoHandler.VO_LadyAlisstra_Female_Naga_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_802H_VoHandler lt23802HVoHandler = this;
    while (lt23802HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23802HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_802H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_802H" && cardID == "LT23_802P2")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23802HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_802H_VoHandler.VO_LadyAlisstra_Female_Naga_Attack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_802H_VoHandler lt23802HVoHandler = this;
    while (lt23802HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23802HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_802H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23802HVoHandler.MissionPlayVO(playByDesignCode, lt23802HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23802HVoHandler.MissionPlayVO(playByDesignCode, lt23802HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23802HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_802H_VoHandler lt23802HVoHandler = this;
    while (lt23802HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23802HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_802H");
    if (entity.GetCardId() == "LT23_802H")
      yield return (object) lt23802HVoHandler.MissionPlaySound(playByDesignCode, lt23802HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_802H_VoHandler lt23802HVoHandler = this;
    while (lt23802HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23802HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_802H");
    if (turn == 1)
      yield return (object) lt23802HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23802HVoHandler.m_introLine);
  }
}
