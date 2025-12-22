using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_800H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_TSC_962_Male_Murloc_Attack_01 = new AssetReference("VO_TSC_962_Male_Murloc_Attack_01.prefab:aa994c5abc5e2cd4ab4180dc6b455a18");
  private static readonly AssetReference VO_TSC_962_Male_Murloc_Death_01 = new AssetReference("VO_TSC_962_Male_Murloc_Death_01.prefab:7bef4971902ef174dabeb1fe59a91302");
  private static readonly AssetReference VO_TSC_962_Male_Murloc_Play_01 = new AssetReference("VO_TSC_962_Male_Murloc_Play_01.prefab:8d5ea0dcfceedc249ae3a45c3264f790");
  private static readonly AssetReference VO_TSC_962t_Male_Murloc_Play_01 = new AssetReference("VO_TSC_962t_Male_Murloc_Play_01.prefab:230cb701b1ca1cf4b84929e8c3b4f4f3");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_800H_VoHandler.VO_TSC_962_Male_Murloc_Play_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_800H_VoHandler.VO_TSC_962_Male_Murloc_Attack_01,
      (string) LettuceBoss_LT23_800H_VoHandler.VO_TSC_962_Male_Murloc_Death_01,
      (string) LettuceBoss_LT23_800H_VoHandler.VO_TSC_962_Male_Murloc_Play_01,
      (string) LettuceBoss_LT23_800H_VoHandler.VO_TSC_962t_Male_Murloc_Play_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_800H_VoHandler.VO_TSC_962_Male_Murloc_Play_01;
    this.m_deathLine = (string) LettuceBoss_LT23_800H_VoHandler.VO_TSC_962_Male_Murloc_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_800H_VoHandler lt23800HVoHandler = this;
    while (lt23800HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23800HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_800H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_800H" && cardID == "LT23_800P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23800HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_800H_VoHandler.VO_TSC_962t_Male_Murloc_Play_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_800H_VoHandler lt23800HVoHandler = this;
    while (lt23800HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23800HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_800H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23800HVoHandler.MissionPlayVO(playByDesignCode, lt23800HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23800HVoHandler.MissionPlayVO(playByDesignCode, lt23800HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23800HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_800H_VoHandler lt23800HVoHandler = this;
    while (lt23800HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23800HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_800H");
    if (entity.GetCardId() == "LT23_800H")
      yield return (object) lt23800HVoHandler.MissionPlaySound(playByDesignCode, lt23800HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_800H_VoHandler lt23800HVoHandler = this;
    while (lt23800HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23800HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_800H");
    if (turn == 1)
      yield return (object) lt23800HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23800HVoHandler.m_introLine);
  }
}
