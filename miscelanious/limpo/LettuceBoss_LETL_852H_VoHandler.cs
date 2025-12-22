using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_852H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Popsicooler_Female_Mech_Attack_01 = new AssetReference("VO_Popsicooler_Female_Mech_Attack_01.prefab:c3f1bb2b85f45634fa6a2ef4e382f303");
  private static readonly AssetReference VO_Popsicooler_Female_Mech_Death_01 = new AssetReference("VO_Popsicooler_Female_Mech_Death_01.prefab:bf912d623c8eae242b4ddba68235e333");
  private static readonly AssetReference VO_Popsicooler_Female_Mech_Intro_01 = new AssetReference("VO_Popsicooler_Female_Mech_Intro_01.prefab:bfc1babc2aa43d14a87f4f61cf98ec4e");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_852H_VoHandler.VO_Popsicooler_Female_Mech_Intro_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_852H_VoHandler.VO_Popsicooler_Female_Mech_Attack_01,
      (string) LettuceBoss_LETL_852H_VoHandler.VO_Popsicooler_Female_Mech_Death_01,
      (string) LettuceBoss_LETL_852H_VoHandler.VO_Popsicooler_Female_Mech_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_852H_VoHandler.VO_Popsicooler_Female_Mech_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_852H_VoHandler.VO_Popsicooler_Female_Mech_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_852H_VoHandler letl852HVoHandler = this;
    while (letl852HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl852HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_852H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_852H" && cardID == "LETL_852P3")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl852HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_852H_VoHandler.VO_Popsicooler_Female_Mech_Attack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_852H_VoHandler letl852HVoHandler = this;
    while (letl852HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl852HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_852H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl852HVoHandler.MissionPlayVO(playByDesignCode, letl852HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl852HVoHandler.MissionPlayVO(playByDesignCode, letl852HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl852HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_852H_VoHandler letl852HVoHandler = this;
    while (letl852HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl852HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_852H");
    if (entity.GetCardId() == "LETL_852H")
      yield return (object) letl852HVoHandler.MissionPlaySound(playByDesignCode, letl852HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_852H_VoHandler letl852HVoHandler = this;
    while (letl852HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl852HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_852H");
    if (turn == 1)
      yield return (object) letl852HVoHandler.MissionPlayVOOnce(playByDesignCode, letl852HVoHandler.m_introLine);
  }
}
