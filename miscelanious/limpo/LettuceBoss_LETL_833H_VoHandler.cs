using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_833H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_833H_Female_Orc_Attack_01 = new AssetReference("VO_LETL_833H_Female_Orc_Attack_01.prefab:c161e569962766548b7532e8e541f78f");
  private static readonly AssetReference VO_LETL_833H_Female_Orc_Attack_02 = new AssetReference("VO_LETL_833H_Female_Orc_Attack_02.prefab:3765845fa8be180428a3ba22a3c8be4f");
  private static readonly AssetReference VO_LETL_833H_Female_Orc_Death_01 = new AssetReference("VO_LETL_833H_Female_Orc_Death_01.prefab:0a71fca7a26d26e4cb5d17e286aef353");
  private static readonly AssetReference VO_LETL_833H_Female_Orc_Idle_01 = new AssetReference("VO_LETL_833H_Female_Orc_Idle_01.prefab:d1eccba0d354c634d967599c3e748a1a");
  private static readonly AssetReference VO_LETL_833H_Female_Orc_Intro_01 = new AssetReference("VO_LETL_833H_Female_Orc_Intro_01.prefab:553a7ddb39249fb4d8126d305f1df53c");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_833H_VoHandler.VO_LETL_833H_Female_Orc_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_833H_VoHandler.VO_LETL_833H_Female_Orc_Attack_01,
      (string) LettuceBoss_LETL_833H_VoHandler.VO_LETL_833H_Female_Orc_Attack_02,
      (string) LettuceBoss_LETL_833H_VoHandler.VO_LETL_833H_Female_Orc_Death_01,
      (string) LettuceBoss_LETL_833H_VoHandler.VO_LETL_833H_Female_Orc_Idle_01,
      (string) LettuceBoss_LETL_833H_VoHandler.VO_LETL_833H_Female_Orc_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_833H_VoHandler.VO_LETL_833H_Female_Orc_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_833H_VoHandler.VO_LETL_833H_Female_Orc_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_833H_VoHandler letl833HVoHandler = this;
    while (letl833HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl833HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_833H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_833H")
    {
      string str = cardID;
      if (!(str == "LETL_003P1_05"))
      {
        if (str == "LETL_034P3_03")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl833HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_833H_VoHandler.VO_LETL_833H_Female_Orc_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl833HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_833H_VoHandler.VO_LETL_833H_Female_Orc_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_833H_VoHandler letl833HVoHandler = this;
    while (letl833HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl833HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_833H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl833HVoHandler.MissionPlayVO(playByDesignCode, letl833HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl833HVoHandler.MissionPlayVO(playByDesignCode, letl833HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl833HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_833H_VoHandler letl833HVoHandler = this;
    while (letl833HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl833HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_833H");
    if (entity.GetCardId() == "LETL_833H")
      yield return (object) letl833HVoHandler.MissionPlaySound(playByDesignCode, letl833HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_833H_VoHandler letl833HVoHandler = this;
    while (letl833HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl833HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_833H");
    if (turn == 1)
      yield return (object) letl833HVoHandler.MissionPlayVOOnce(playByDesignCode, letl833HVoHandler.m_introLine);
  }
}
