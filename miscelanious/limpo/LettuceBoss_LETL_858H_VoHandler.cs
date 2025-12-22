using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_858H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_IvusForestLord_Male_Elemental_Attack_01 = new AssetReference("VO_IvusForestLord_Male_Elemental_Attack_01.prefab:fcec32ed74edaa1489183abcfc082777");
  private static readonly AssetReference VO_IvusForestLord_Male_Elemental_Attack_02 = new AssetReference("VO_IvusForestLord_Male_Elemental_Attack_02.prefab:e32c9da1ba658e443a1aefba638d4b4a");
  private static readonly AssetReference VO_IvusForestLord_Male_Elemental_Death_01 = new AssetReference("VO_IvusForestLord_Male_Elemental_Death_01.prefab:f191132e0fe93964ea43dbd8ba26c9ed");
  private static readonly AssetReference VO_IvusForestLord_Male_Elemental_Idle_01 = new AssetReference("VO_IvusForestLord_Male_Elemental_Idle_01.prefab:bb90fe7bfc9560b4387ad9adf100cf32");
  private static readonly AssetReference VO_IvusForestLord_Male_Elemental_Intro_01 = new AssetReference("VO_IvusForestLord_Male_Elemental_Intro_01.prefab:bd66bfac62c752749bc2574cab3c09d0");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_858H_VoHandler.VO_IvusForestLord_Male_Elemental_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_858H_VoHandler.VO_IvusForestLord_Male_Elemental_Attack_01,
      (string) LettuceBoss_LETL_858H_VoHandler.VO_IvusForestLord_Male_Elemental_Death_01,
      (string) LettuceBoss_LETL_858H_VoHandler.VO_IvusForestLord_Male_Elemental_Idle_01,
      (string) LettuceBoss_LETL_858H_VoHandler.VO_IvusForestLord_Male_Elemental_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_858H_VoHandler.VO_IvusForestLord_Male_Elemental_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_858H_VoHandler.VO_IvusForestLord_Male_Elemental_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_858H_VoHandler letl858HVoHandler = this;
    while (letl858HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl858HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_858H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_858H" && cardID == "LETL_858P1_01")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl858HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_858H_VoHandler.VO_IvusForestLord_Male_Elemental_Attack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_858H_VoHandler letl858HVoHandler = this;
    while (letl858HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl858HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_858H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl858HVoHandler.MissionPlayVO(playByDesignCode, letl858HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl858HVoHandler.MissionPlayVO(playByDesignCode, letl858HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl858HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_858H_VoHandler letl858HVoHandler = this;
    while (letl858HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl858HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_858H");
    if (entity.GetCardId() == "LETL_858H")
      yield return (object) letl858HVoHandler.MissionPlaySound(playByDesignCode, letl858HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_858H_VoHandler letl858HVoHandler = this;
    while (letl858HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl858HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_858H");
    if (turn == 1)
      yield return (object) letl858HVoHandler.MissionPlayVOOnce(playByDesignCode, letl858HVoHandler.m_introLine);
  }
}
