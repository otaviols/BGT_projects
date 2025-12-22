using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_817H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_YShaarj_Male_OldGod_LETL_Attack_02 = new AssetReference("VO_YShaarj_Male_OldGod_LETL_Attack_02.prefab:c40fe49ecaf573149a66a2adbddd6d82");
  private static readonly AssetReference VO_YShaarj_Male_OldGod_LETL_Attack_03 = new AssetReference("VO_YShaarj_Male_OldGod_LETL_Attack_03.prefab:542abb4375d882d44bbe0f0fee02c421");
  private static readonly AssetReference VO_YShaarj_Male_OldGod_LETL_Death_02 = new AssetReference("VO_YShaarj_Male_OldGod_LETL_Death_02.prefab:251828353146b014aa7d3d834c0f5f26");
  private static readonly AssetReference VO_YShaarj_Male_OldGod_LETL_Idle_01 = new AssetReference("VO_YShaarj_Male_OldGod_LETL_Idle_01.prefab:4f7f8e0aba2cf43499d88093fd35d5b8");
  private static readonly AssetReference VO_YShaarj_Male_OldGod_LETL_Intro_02 = new AssetReference("VO_YShaarj_Male_OldGod_LETL_Intro_02.prefab:e1c687dba0681c1408e75128d862b05c");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Intro_02,
      (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Attack_02,
      (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Attack_03,
      (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Idle_01,
      (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Death_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Intro_02;
    this.m_deathLine = (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Death_02;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_817H_VoHandler lt23817HVoHandler = this;
    while (lt23817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23817HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_817H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_817H")
    {
      string str = cardID;
      if (!(str == "LT23_817P1"))
      {
        if (!(str == "LT23_028P2_03"))
        {
          if (str == "LT23_028P2_05")
          {
            GameState.Get().SetBusy(true);
            yield return (object) lt23817HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Attack_03);
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt23817HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Attack_03);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23817HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_817H_VoHandler.VO_YShaarj_Male_OldGod_LETL_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_817H_VoHandler lt23817HVoHandler = this;
    while (lt23817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23817HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_817H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23817HVoHandler.MissionPlayVO(playByDesignCode, lt23817HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23817HVoHandler.MissionPlayVO(playByDesignCode, lt23817HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23817HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_817H_VoHandler lt23817HVoHandler = this;
    while (lt23817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23817HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_817H");
    if (entity.GetCardId() == "LT23_817H")
      yield return (object) lt23817HVoHandler.MissionPlaySound(playByDesignCode, lt23817HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_817H_VoHandler lt23817HVoHandler = this;
    while (lt23817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23817HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_817H");
    if (turn == 1)
      yield return (object) lt23817HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23817HVoHandler.m_introLine);
  }
}
