using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_859H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_VanndarStormpike_Male_Dwarf_Attack_01 = new AssetReference("VO_VanndarStormpike_Male_Dwarf_Attack_01.prefab:e9bd71b4a3611d641a3449a8cf8131aa");
  private static readonly AssetReference VO_VanndarStormpike_Male_Dwarf_Bark_10 = new AssetReference("VO_VanndarStormpike_Male_Dwarf_Bark_10.prefab:0bc51d6c0b9aa3d44880a5e3e4296e2b");
  private static readonly AssetReference VO_VanndarStormpike_Male_Dwarf_Death_01 = new AssetReference("VO_VanndarStormpike_Male_Dwarf_Death_01.prefab:f68fc7b7e88c0864db1d6e470c97489c");
  private static readonly AssetReference VO_VanndarStormpike_Male_Dwarf_Play_01 = new AssetReference("VO_VanndarStormpike_Male_Dwarf_Play_01.prefab:191a9ca0e4a2e4244a98eaf5478bff1d");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_859H_VoHandler.VO_VanndarStormpike_Male_Dwarf_Bark_10
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_859H_VoHandler.VO_VanndarStormpike_Male_Dwarf_Attack_01,
      (string) LettuceBoss_LETL_859H_VoHandler.VO_VanndarStormpike_Male_Dwarf_Bark_10,
      (string) LettuceBoss_LETL_859H_VoHandler.VO_VanndarStormpike_Male_Dwarf_Death_01,
      (string) LettuceBoss_LETL_859H_VoHandler.VO_VanndarStormpike_Male_Dwarf_Play_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_859H_VoHandler.VO_VanndarStormpike_Male_Dwarf_Play_01;
    this.m_deathLine = (string) LettuceBoss_LETL_859H_VoHandler.VO_VanndarStormpike_Male_Dwarf_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_859H_VoHandler letl859HVoHandler = this;
    while (letl859HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl859HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_859H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_859H" && cardID == "LETL_859P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl859HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_859H_VoHandler.VO_VanndarStormpike_Male_Dwarf_Attack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_859H_VoHandler letl859HVoHandler = this;
    while (letl859HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl859HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_859H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl859HVoHandler.MissionPlayVO(playByDesignCode, letl859HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl859HVoHandler.MissionPlayVO(playByDesignCode, letl859HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl859HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_859H_VoHandler letl859HVoHandler = this;
    while (letl859HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl859HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_859H");
    if (entity.GetCardId() == "LETL_859H")
      yield return (object) letl859HVoHandler.MissionPlaySound(playByDesignCode, letl859HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_859H_VoHandler letl859HVoHandler = this;
    while (letl859HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl859HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_859H");
    if (turn == 1)
      yield return (object) letl859HVoHandler.MissionPlayVOOnce(playByDesignCode, letl859HVoHandler.m_introLine);
  }
}
