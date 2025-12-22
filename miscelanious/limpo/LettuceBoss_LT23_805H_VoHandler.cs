using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_805H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_AcolyteOfNZoth_Male_Human_Attack_02 = new AssetReference("VO_AcolyteOfNZoth_Male_Human_Attack_02.prefab:04ca7c9a9e6808c42aeb6ed4a17f4636");
  private static readonly AssetReference VO_AcolyteOfNZoth_Male_Human_Death_01 = new AssetReference("VO_AcolyteOfNZoth_Male_Human_Death_01.prefab:8939de60b02c0cb4a87b3472d8832cae");
  private static readonly AssetReference VO_AcolyteOfNZoth_Male_Human_Idle_01 = new AssetReference("VO_AcolyteOfNZoth_Male_Human_Idle_01.prefab:fe5107aefac42dd46b2744734b48c104");
  private static readonly AssetReference VO_AcolyteOfNZoth_Male_Human_Intro_01 = new AssetReference("VO_AcolyteOfNZoth_Male_Human_Intro_01.prefab:4c61662228801f943a9924bca1474c80");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_805H_VoHandler.VO_AcolyteOfNZoth_Male_Human_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_805H_VoHandler.VO_AcolyteOfNZoth_Male_Human_Death_01,
      (string) LettuceBoss_LT23_805H_VoHandler.VO_AcolyteOfNZoth_Male_Human_Idle_01,
      (string) LettuceBoss_LT23_805H_VoHandler.VO_AcolyteOfNZoth_Male_Human_Intro_01,
      (string) LettuceBoss_LT23_805H_VoHandler.VO_AcolyteOfNZoth_Male_Human_Attack_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_805H_VoHandler.VO_AcolyteOfNZoth_Male_Human_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_805H_VoHandler.VO_AcolyteOfNZoth_Male_Human_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_805H_VoHandler lt23805HVoHandler = this;
    while (lt23805HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23805HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_805H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_805H" && cardID == "LT23_805P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23805HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_805H_VoHandler.VO_AcolyteOfNZoth_Male_Human_Attack_02);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_805H_VoHandler lt23805HVoHandler = this;
    while (lt23805HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23805HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_805H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23805HVoHandler.MissionPlayVO(playByDesignCode, lt23805HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23805HVoHandler.MissionPlayVO(playByDesignCode, lt23805HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23805HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_805H_VoHandler lt23805HVoHandler = this;
    while (lt23805HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23805HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_805H");
    if (entity.GetCardId() == "LT23_805H")
      yield return (object) lt23805HVoHandler.MissionPlaySound(playByDesignCode, lt23805HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_805H_VoHandler lt23805HVoHandler = this;
    while (lt23805HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23805HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_805H");
    if (turn == 1)
      yield return (object) lt23805HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23805HVoHandler.m_introLine);
  }
}
