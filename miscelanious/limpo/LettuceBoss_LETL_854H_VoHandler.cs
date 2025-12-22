using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_854H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_BOM_08_006_Male_Orc_DrekThar_InGame_EmoteResponse_01 = new AssetReference("VO_BOM_08_006_Male_Orc_DrekThar_InGame_EmoteResponse_01.prefab:88b4179bdd910244c929ac7940c5c65b");
  private static readonly AssetReference VO_BOM_08_006_Male_Orc_DrekThar_InGame_BossIdle_01 = new AssetReference("VO_BOM_08_006_Male_Orc_DrekThar_InGame_BossIdle_01.prefab:7a35c9e81b641ef41b3da1d66591c5a4");
  private static readonly AssetReference VO_BOM_08_006_Male_Orc_DrekThar_InGame_BossIdle_02 = new AssetReference("VO_BOM_08_006_Male_Orc_DrekThar_InGame_BossIdle_02.prefab:5db1e882a4b902b44b28f862c8e5e239");
  private static readonly AssetReference Death = new AssetReference("Death.prefab:76a4ff0c9ea3bea4daff6d9c21dd1e9a");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_854H_VoHandler.VO_BOM_08_006_Male_Orc_DrekThar_InGame_BossIdle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_854H_VoHandler.VO_BOM_08_006_Male_Orc_DrekThar_InGame_EmoteResponse_01,
      (string) LettuceBoss_LETL_854H_VoHandler.VO_BOM_08_006_Male_Orc_DrekThar_InGame_BossIdle_01,
      (string) LettuceBoss_LETL_854H_VoHandler.VO_BOM_08_006_Male_Orc_DrekThar_InGame_BossIdle_02,
      (string) LettuceBoss_LETL_854H_VoHandler.Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_854H_VoHandler.VO_BOM_08_006_Male_Orc_DrekThar_InGame_EmoteResponse_01;
    this.m_deathLine = (string) LettuceBoss_LETL_854H_VoHandler.Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_854H_VoHandler letl854HVoHandler = this;
    while (letl854HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl854HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_854H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_854H" && cardID == "LETL_854P1_01")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl854HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_854H_VoHandler.VO_BOM_08_006_Male_Orc_DrekThar_InGame_BossIdle_02);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_854H_VoHandler letl854HVoHandler = this;
    while (letl854HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl854HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_854H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl854HVoHandler.MissionPlayVO(playByDesignCode, letl854HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl854HVoHandler.MissionPlayVO(playByDesignCode, letl854HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl854HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_854H_VoHandler letl854HVoHandler = this;
    while (letl854HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl854HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_854H");
    if (entity.GetCardId() == "LETL_854H")
      yield return (object) letl854HVoHandler.MissionPlaySound(playByDesignCode, letl854HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_854H_VoHandler letl854HVoHandler = this;
    while (letl854HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl854HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_854H");
    if (turn == 1)
      yield return (object) letl854HVoHandler.MissionPlayVOOnce(playByDesignCode, letl854HVoHandler.m_introLine);
  }
}
