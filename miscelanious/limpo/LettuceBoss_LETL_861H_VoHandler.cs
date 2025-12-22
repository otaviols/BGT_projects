using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_861H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Smolderwing_Male_Dragon_Attack_01 = new AssetReference("VO_Smolderwing_Male_Dragon_Attack_01.prefab:b037c3832239e18419af3f48c9a9ce41");
  private static readonly AssetReference VO_Smolderwing_Male_Dragon_Attack_02 = new AssetReference("VO_Smolderwing_Male_Dragon_Attack_02.prefab:aac48028d4af14344885197f3c41d392");
  private static readonly AssetReference VO_Smolderwing_Male_Dragon_Death_01 = new AssetReference("VO_Smolderwing_Male_Dragon_Death_01.prefab:e42799c9240f2df41ba9c078e3536416");
  private static readonly AssetReference VO_Smolderwing_Male_Dragon_Idle_01 = new AssetReference("VO_Smolderwing_Male_Dragon_Idle_01.prefab:1bd60de254a457b4994cac9286faf2ce");
  private static readonly AssetReference VO_Smolderwing_Male_Dragon_Intro_01 = new AssetReference("VO_Smolderwing_Male_Dragon_Intro_01.prefab:0468a727e7cce144f9f6dfdf1385b315");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_861H_VoHandler.VO_Smolderwing_Male_Dragon_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_861H_VoHandler.VO_Smolderwing_Male_Dragon_Attack_01,
      (string) LettuceBoss_LETL_861H_VoHandler.VO_Smolderwing_Male_Dragon_Attack_02,
      (string) LettuceBoss_LETL_861H_VoHandler.VO_Smolderwing_Male_Dragon_Death_01,
      (string) LettuceBoss_LETL_861H_VoHandler.VO_Smolderwing_Male_Dragon_Idle_01,
      (string) LettuceBoss_LETL_861H_VoHandler.VO_Smolderwing_Male_Dragon_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_861H_VoHandler.VO_Smolderwing_Male_Dragon_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_861H_VoHandler.VO_Smolderwing_Male_Dragon_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_861H_VoHandler letl861HVoHandler = this;
    while (letl861HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl861HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_861H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_861H")
    {
      string str = cardID;
      if (!(str == "LETL_861P2_01"))
      {
        if (str == "LETL_861P1_01")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl861HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_861H_VoHandler.VO_Smolderwing_Male_Dragon_Attack_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl861HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_861H_VoHandler.VO_Smolderwing_Male_Dragon_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_861H_VoHandler letl861HVoHandler = this;
    while (letl861HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl861HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_861H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl861HVoHandler.MissionPlayVO(playByDesignCode, letl861HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl861HVoHandler.MissionPlayVO(playByDesignCode, letl861HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl861HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_861H_VoHandler letl861HVoHandler = this;
    while (letl861HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl861HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_861H");
    if (entity.GetCardId() == "LETL_861H")
      yield return (object) letl861HVoHandler.MissionPlaySound(playByDesignCode, letl861HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_861H_VoHandler letl861HVoHandler = this;
    while (letl861HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl861HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_861H");
    if (turn == 1)
      yield return (object) letl861HVoHandler.MissionPlayVOOnce(playByDesignCode, letl861HVoHandler.m_introLine);
  }
}
