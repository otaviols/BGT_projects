using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_801H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_WarlordRekes_Male_Naga_Attack_02 = new AssetReference("VO_WarlordRekes_Male_Naga_Attack_02.prefab:3166cbc5855d63943b471dfc6e9b3936");
  private static readonly AssetReference VO_WarlordRekes_Male_Naga_Death_01 = new AssetReference("VO_WarlordRekes_Male_Naga_Death_01.prefab:899aa139d9174434eb83c4962f2445f9");
  private static readonly AssetReference VO_WarlordRekes_Male_Naga_Idle_01 = new AssetReference("VO_WarlordRekes_Male_Naga_Idle_01.prefab:3ba9fcd6b7f5da1479129ba585f88eae");
  private static readonly AssetReference VO_WarlordRekes_Male_Naga_Intro_01 = new AssetReference("VO_WarlordRekes_Male_Naga_Intro_01.prefab:486107c2ed26c6746bb94c8afde57c95");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_801H_VoHandler.VO_WarlordRekes_Male_Naga_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_801H_VoHandler.VO_WarlordRekes_Male_Naga_Death_01,
      (string) LettuceBoss_LT23_801H_VoHandler.VO_WarlordRekes_Male_Naga_Intro_01,
      (string) LettuceBoss_LT23_801H_VoHandler.VO_WarlordRekes_Male_Naga_Attack_02,
      (string) LettuceBoss_LT23_801H_VoHandler.VO_WarlordRekes_Male_Naga_Idle_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_801H_VoHandler.VO_WarlordRekes_Male_Naga_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_801H_VoHandler.VO_WarlordRekes_Male_Naga_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_801H_VoHandler lt23801HVoHandler = this;
    while (lt23801HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23801HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_801H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_801H" && cardID == "LT23_801P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23801HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_801H_VoHandler.VO_WarlordRekes_Male_Naga_Attack_02);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_801H_VoHandler lt23801HVoHandler = this;
    while (lt23801HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23801HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_801H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23801HVoHandler.MissionPlayVO(playByDesignCode, lt23801HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23801HVoHandler.MissionPlayVO(playByDesignCode, lt23801HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23801HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_801H_VoHandler lt23801HVoHandler = this;
    while (lt23801HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23801HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_801H");
    if (entity.GetCardId() == "LT23_801H")
      yield return (object) lt23801HVoHandler.MissionPlaySound(playByDesignCode, lt23801HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_801H_VoHandler lt23801HVoHandler = this;
    while (lt23801HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23801HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_801H");
    if (turn == 1)
      yield return (object) lt23801HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23801HVoHandler.m_introLine);
  }
}
