using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_821H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Maestra_Female_Orc_LETL_Attack_03 = new AssetReference("VO_Maestra_Female_Orc_LETL_Attack_03.prefab:ec8904d190c4e7c48969df0b42862cc1");
  private static readonly AssetReference VO_Maestra_Female_Orc_LETL_Death_01 = new AssetReference("VO_Maestra_Female_Orc_LETL_Death_01.prefab:36524b468e0f9cd4f9c493838706be93");
  private static readonly AssetReference VO_Maestra_Female_Orc_LETL_Idle_01 = new AssetReference("VO_Maestra_Female_Orc_LETL_Idle_01.prefab:c0d88723ec6aa104eaaa1ae55abafedf");
  private static readonly AssetReference VO_Maestra_Female_Orc_LETL_Intro_01 = new AssetReference("VO_Maestra_Female_Orc_LETL_Intro_01.prefab:0432e7581966f82439bce9d76cb1aa43");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_821H_VoHandler.VO_Maestra_Female_Orc_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_821H_VoHandler.VO_Maestra_Female_Orc_LETL_Intro_01,
      (string) LettuceBoss_LT23_821H_VoHandler.VO_Maestra_Female_Orc_LETL_Attack_03,
      (string) LettuceBoss_LT23_821H_VoHandler.VO_Maestra_Female_Orc_LETL_Idle_01,
      (string) LettuceBoss_LT23_821H_VoHandler.VO_Maestra_Female_Orc_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_821H_VoHandler.VO_Maestra_Female_Orc_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_821H_VoHandler.VO_Maestra_Female_Orc_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_821H_VoHandler lt23821HVoHandler = this;
    while (lt23821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23821HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_821H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_821H" && cardID == "LT23_821P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23821HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_821H_VoHandler.VO_Maestra_Female_Orc_LETL_Attack_03);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_821H_VoHandler lt23821HVoHandler = this;
    while (lt23821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23821HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_821H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23821HVoHandler.MissionPlayVO(playByDesignCode, lt23821HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23821HVoHandler.MissionPlayVO(playByDesignCode, lt23821HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23821HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_821H_VoHandler lt23821HVoHandler = this;
    while (lt23821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23821HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_821H");
    if (entity.GetCardId() == "LT23_821H")
      yield return (object) lt23821HVoHandler.MissionPlaySound(playByDesignCode, lt23821HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_821H_VoHandler lt23821HVoHandler = this;
    while (lt23821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23821HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_821H");
    if (turn == 1)
      yield return (object) lt23821HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23821HVoHandler.m_introLine);
  }
}
