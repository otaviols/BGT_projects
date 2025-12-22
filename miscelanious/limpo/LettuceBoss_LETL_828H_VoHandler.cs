using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_828H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_828H_Male_Ancient_Attack_01 = new AssetReference("VO_LETL_828H_Male_Ancient_Attack_01.prefab:894586fb0af2e8e448b8ce05d29aa0f4");
  private static readonly AssetReference VO_LETL_828H_Male_Ancient_Attack_02 = new AssetReference("VO_LETL_828H_Male_Ancient_Attack_02.prefab:3763ab6f1bf94774db4082d2e2c18735");
  private static readonly AssetReference VO_LETL_828H_Male_Ancient_Death_01 = new AssetReference("VO_LETL_828H_Male_Ancient_Death_01.prefab:ccfba26268e7d5240b23f754437b3020");
  private static readonly AssetReference VO_LETL_828H_Male_Ancient_Idle_01 = new AssetReference("VO_LETL_828H_Male_Ancient_Idle_01.prefab:c25937487d86d944a8436a09f687d833");
  private static readonly AssetReference VO_LETL_828H_Male_Ancient_Intro_01 = new AssetReference("VO_LETL_828H_Male_Ancient_Intro_01.prefab:6067716e6019bec419a1bf0d60849f93");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_828H_VoHandler.VO_LETL_828H_Male_Ancient_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_828H_VoHandler.VO_LETL_828H_Male_Ancient_Attack_01,
      (string) LettuceBoss_LETL_828H_VoHandler.VO_LETL_828H_Male_Ancient_Attack_02,
      (string) LettuceBoss_LETL_828H_VoHandler.VO_LETL_828H_Male_Ancient_Death_01,
      (string) LettuceBoss_LETL_828H_VoHandler.VO_LETL_828H_Male_Ancient_Idle_01,
      (string) LettuceBoss_LETL_828H_VoHandler.VO_LETL_828H_Male_Ancient_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_828H_VoHandler.VO_LETL_828H_Male_Ancient_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_828H_VoHandler.VO_LETL_828H_Male_Ancient_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_828H_VoHandler letl828HVoHandler = this;
    while (letl828HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl828HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_828H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_828H")
    {
      string str = cardID;
      if (!(str == "LETL_828P1_02") && !(str == "LETL_828P1_05"))
      {
        if (str == "LETL_828P2_01" || str == "LETL_828P2_03")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl828HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_828H_VoHandler.VO_LETL_828H_Male_Ancient_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl828HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_828H_VoHandler.VO_LETL_828H_Male_Ancient_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_828H_VoHandler letl828HVoHandler = this;
    while (letl828HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl828HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_828H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl828HVoHandler.MissionPlayVO(playByDesignCode, letl828HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl828HVoHandler.MissionPlayVO(playByDesignCode, letl828HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl828HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_828H_VoHandler letl828HVoHandler = this;
    while (letl828HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl828HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_828H");
    if (entity.GetCardId() == "LETL_828H")
      yield return (object) letl828HVoHandler.MissionPlaySound(playByDesignCode, letl828HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_828H_VoHandler letl828HVoHandler = this;
    while (letl828HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl828HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_828H");
    if (turn == 1)
      yield return (object) letl828HVoHandler.MissionPlayVOOnce(playByDesignCode, letl828HVoHandler.m_introLine);
  }
}
