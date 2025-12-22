using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_813H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Xaril_Male_Mantid_LETL_Attack_01 = new AssetReference("VO_Xaril_Male_Mantid_LETL_Attack_01.prefab:7d0d29d594294e748aa2767018aaa703");
  private static readonly AssetReference VO_Xaril_Male_Mantid_LETL_Attack_02 = new AssetReference("VO_Xaril_Male_Mantid_LETL_Attack_02.prefab:76962cebd9f527f479dd7527c0eed4d3");
  private static readonly AssetReference VO_Xaril_Male_Mantid_LETL_Death_01 = new AssetReference("VO_Xaril_Male_Mantid_LETL_Death_01.prefab:7de9c877d1413cb45a60cf47e648ce0a");
  private static readonly AssetReference VO_Xaril_Male_Mantid_LETL_Idle_01 = new AssetReference("VO_Xaril_Male_Mantid_LETL_Idle_01.prefab:aeac2603aec1f594abe820b430690265");
  private static readonly AssetReference VO_Xaril_Male_Mantid_LETL_Intro_01 = new AssetReference("VO_Xaril_Male_Mantid_LETL_Intro_01.prefab:323182c2b89f6f044af32da43ae81564");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Intro_01,
      (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Attack_01,
      (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Attack_02,
      (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Idle_01,
      (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_813H_VoHandler lt23813HVoHandler = this;
    while (lt23813HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23813HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_813H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_813H")
    {
      string str = cardID;
      if (!(str == "LT23_813P1"))
      {
        if (!(str == "LETL_019P1_03"))
        {
          if (str == "LETL_019P1_05")
          {
            GameState.Get().SetBusy(true);
            yield return (object) lt23813HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Attack_02);
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt23813HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23813HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_813H_VoHandler.VO_Xaril_Male_Mantid_LETL_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_813H_VoHandler lt23813HVoHandler = this;
    while (lt23813HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23813HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_813H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23813HVoHandler.MissionPlayVO(playByDesignCode, lt23813HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23813HVoHandler.MissionPlayVO(playByDesignCode, lt23813HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23813HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_813H_VoHandler lt23813HVoHandler = this;
    while (lt23813HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23813HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_813H");
    if (entity.GetCardId() == "LT23_813H")
      yield return (object) lt23813HVoHandler.MissionPlaySound(playByDesignCode, lt23813HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_813H_VoHandler lt23813HVoHandler = this;
    while (lt23813HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23813HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_813H");
    if (turn == 1)
      yield return (object) lt23813HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23813HVoHandler.m_introLine);
  }
}
