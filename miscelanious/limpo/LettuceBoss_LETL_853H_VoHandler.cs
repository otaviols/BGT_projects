using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_853H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_BOM_09_006_Male_Elemental_Lokholar_InGame_BossIdle_02 = new AssetReference("VO_BOM_09_006_Male_Elemental_Lokholar_InGame_BossIdle_02.prefab:fd6694af06b96c74ea92572c22fbc984");
  private static readonly AssetReference VO_BOM_09_006_Male_Elemental_Lokholar_InGame_BossIdle_03 = new AssetReference("VO_BOM_09_006_Male_Elemental_Lokholar_InGame_BossIdle_03.prefab:22ee7f95139c667448ef2c0420c509c7");
  private static readonly AssetReference VO_BOM_09_005_Male_Elemental_Lokholar_InGame_VictoryPreExplosion_01_B = new AssetReference("VO_BOM_09_005_Male_Elemental_Lokholar_InGame_VictoryPreExplosion_01_B.prefab:fe01006941e39b346a5a2e060875eda2");
  private static readonly AssetReference Death = new AssetReference("Death.prefab:76a4ff0c9ea3bea4daff6d9c21dd1e9a");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_853H_VoHandler.VO_BOM_09_006_Male_Elemental_Lokholar_InGame_BossIdle_03
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_853H_VoHandler.VO_BOM_09_006_Male_Elemental_Lokholar_InGame_BossIdle_02,
      (string) LettuceBoss_LETL_853H_VoHandler.VO_BOM_09_006_Male_Elemental_Lokholar_InGame_BossIdle_03,
      (string) LettuceBoss_LETL_853H_VoHandler.VO_BOM_09_005_Male_Elemental_Lokholar_InGame_VictoryPreExplosion_01_B,
      (string) LettuceBoss_LETL_853H_VoHandler.Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_853H_VoHandler.VO_BOM_09_005_Male_Elemental_Lokholar_InGame_VictoryPreExplosion_01_B;
    this.m_deathLine = (string) LettuceBoss_LETL_853H_VoHandler.Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_853H_VoHandler letl853HVoHandler = this;
    while (letl853HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl853HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_853H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_853H" && cardID == "LETL_853P3_01")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl853HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_853H_VoHandler.VO_BOM_09_006_Male_Elemental_Lokholar_InGame_BossIdle_02);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_853H_VoHandler letl853HVoHandler = this;
    while (letl853HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl853HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_853H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl853HVoHandler.MissionPlayVO(playByDesignCode, letl853HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl853HVoHandler.MissionPlayVO(playByDesignCode, letl853HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl853HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_853H_VoHandler letl853HVoHandler = this;
    while (letl853HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl853HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_853H");
    if (entity.GetCardId() == "LETL_853H")
      yield return (object) letl853HVoHandler.MissionPlaySound(playByDesignCode, letl853HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_853H_VoHandler letl853HVoHandler = this;
    while (letl853HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl853HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_853H");
    if (turn == 1)
      yield return (object) letl853HVoHandler.MissionPlayVOOnce(playByDesignCode, letl853HVoHandler.m_introLine);
  }
}
