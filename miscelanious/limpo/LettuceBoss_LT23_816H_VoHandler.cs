using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_816H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_GarroshHellscream_Male_Orc_LETL_Attack_02 = new AssetReference("VO_GarroshHellscream_Male_Orc_LETL_Attack_02.prefab:5a7f3a0c0bd23ee40a7e80b530c88db4");
  private static readonly AssetReference VO_GarroshHellscream_Male_Orc_LETL_Attack_03 = new AssetReference("VO_GarroshHellscream_Male_Orc_LETL_Attack_03.prefab:c2bb2213bdfc1ce419ceb7fb9af29f5a");
  private static readonly AssetReference VO_GarroshHellscream_Male_Orc_LETL_Death_01 = new AssetReference("VO_GarroshHellscream_Male_Orc_LETL_Death_01.prefab:bac71eedd575d4c479f2d73eb42f806b");
  private static readonly AssetReference VO_GarroshHellscream_Male_Orc_LETL_Idle_01 = new AssetReference("VO_GarroshHellscream_Male_Orc_LETL_Idle_01.prefab:92a4e8187d63e34478fbe42114176acd");
  private static readonly AssetReference VO_GarroshHellscream_Male_Orc_LETL_Intro_01 = new AssetReference("VO_GarroshHellscream_Male_Orc_LETL_Intro_01.prefab:f3b824187d1e9b84488df31f0294c188");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_816H_VoHandler.VO_GarroshHellscream_Male_Orc_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_816H_VoHandler.VO_GarroshHellscream_Male_Orc_LETL_Intro_01,
      (string) LettuceBoss_LT23_816H_VoHandler.VO_GarroshHellscream_Male_Orc_LETL_Attack_02,
      (string) LettuceBoss_LT23_816H_VoHandler.VO_GarroshHellscream_Male_Orc_LETL_Attack_03,
      (string) LettuceBoss_LT23_816H_VoHandler.VO_GarroshHellscream_Male_Orc_LETL_Idle_01,
      (string) LettuceBoss_LT23_816H_VoHandler.VO_GarroshHellscream_Male_Orc_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_816H_VoHandler.VO_GarroshHellscream_Male_Orc_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_816H_VoHandler.VO_GarroshHellscream_Male_Orc_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_816H_VoHandler lt23816HVoHandler = this;
    while (lt23816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23816HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_816H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_816H")
    {
      string str = cardID;
      if (!(str == "LT23_816P1"))
      {
        if (str == "LT23_816P2")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt23816HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_816H_VoHandler.VO_GarroshHellscream_Male_Orc_LETL_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23816HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_816H_VoHandler.VO_GarroshHellscream_Male_Orc_LETL_Attack_03);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_816H_VoHandler lt23816HVoHandler = this;
    while (lt23816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23816HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_816H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23816HVoHandler.MissionPlayVO(playByDesignCode, lt23816HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23816HVoHandler.MissionPlayVO(playByDesignCode, lt23816HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23816HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_816H_VoHandler lt23816HVoHandler = this;
    while (lt23816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23816HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_816H");
    if (entity.GetCardId() == "LT23_816H")
      yield return (object) lt23816HVoHandler.MissionPlaySound(playByDesignCode, lt23816HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_816H_VoHandler lt23816HVoHandler = this;
    while (lt23816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23816HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_816H");
    if (turn == 1)
      yield return (object) lt23816HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23816HVoHandler.m_introLine);
  }
}
