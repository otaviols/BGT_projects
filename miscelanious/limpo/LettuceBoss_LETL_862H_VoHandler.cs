using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_862H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_DragonboneGolem_Male_Construct_Attack_01 = new AssetReference("VO_DragonboneGolem_Male_Construct_Attack_01.prefab:cc7478f1188143f43afa3303a1b009bd");
  private static readonly AssetReference VO_DragonboneGolem_Male_Construct_Attack_02 = new AssetReference("VO_DragonboneGolem_Male_Construct_Attack_02.prefab:77f9b82372e11624da090239e1817b66");
  private static readonly AssetReference VO_DragonboneGolem_Male_Construct_Death_01 = new AssetReference("VO_DragonboneGolem_Male_Construct_Death_01.prefab:25524cca49884194883d9f2c38b3c53e");
  private static readonly AssetReference VO_DragonboneGolem_Male_Construct_Idle_01 = new AssetReference("VO_DragonboneGolem_Male_Construct_Idle_01.prefab:beec59dea175a5d4289bb7d33555bed7");
  private static readonly AssetReference VO_DragonboneGolem_Male_Construct_Intro_01 = new AssetReference("VO_DragonboneGolem_Male_Construct_Intro_01.prefab:ff5546e5387f6c7408bfee80d33f6f88");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_862H_VoHandler.VO_DragonboneGolem_Male_Construct_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_862H_VoHandler.VO_DragonboneGolem_Male_Construct_Attack_01,
      (string) LettuceBoss_LETL_862H_VoHandler.VO_DragonboneGolem_Male_Construct_Attack_02,
      (string) LettuceBoss_LETL_862H_VoHandler.VO_DragonboneGolem_Male_Construct_Death_01,
      (string) LettuceBoss_LETL_862H_VoHandler.VO_DragonboneGolem_Male_Construct_Idle_01,
      (string) LettuceBoss_LETL_862H_VoHandler.VO_DragonboneGolem_Male_Construct_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_862H_VoHandler.VO_DragonboneGolem_Male_Construct_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_862H_VoHandler.VO_DragonboneGolem_Male_Construct_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_862H_VoHandler letl862HVoHandler = this;
    while (letl862HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl862HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_862H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_862H" && cardID == "LETL_862P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl862HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_862H_VoHandler.VO_DragonboneGolem_Male_Construct_Attack_02);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_862H_VoHandler letl862HVoHandler = this;
    while (letl862HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl862HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_862H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl862HVoHandler.MissionPlayVO(playByDesignCode, letl862HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl862HVoHandler.MissionPlayVO(playByDesignCode, letl862HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl862HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_862H_VoHandler letl862HVoHandler = this;
    while (letl862HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl862HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_862H");
    if (entity.GetCardId() == "LETL_862H")
      yield return (object) letl862HVoHandler.MissionPlaySound(playByDesignCode, letl862HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_862H_VoHandler letl862HVoHandler = this;
    while (letl862HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl862HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_862H");
    if (turn == 1)
      yield return (object) letl862HVoHandler.MissionPlayVOOnce(playByDesignCode, letl862HVoHandler.m_introLine);
  }
}
