using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_812H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_TaranZhu_Male_Pandaren_LETL_Attack_01 = new AssetReference("VO_TaranZhu_Male_Pandaren_LETL_Attack_01.prefab:51d880e5e2691a848a3ff40bfa1ac3d1");
  private static readonly AssetReference VO_TaranZhu_Male_Pandaren_LETL_Attack_02 = new AssetReference("VO_TaranZhu_Male_Pandaren_LETL_Attack_02.prefab:9435a50bafff3894c9aeda14d0a357af");
  private static readonly AssetReference VO_TaranZhu_Male_Pandaren_LETL_Death_01 = new AssetReference("VO_TaranZhu_Male_Pandaren_LETL_Death_01.prefab:df78807536bbd5f4c90e9e5d2a9d822c");
  private static readonly AssetReference VO_TaranZhu_Male_Pandaren_LETL_Idle_01 = new AssetReference("VO_TaranZhu_Male_Pandaren_LETL_Idle_01.prefab:da4ab8b755cc257499e1cd6571ebef3a");
  private static readonly AssetReference VO_TaranZhu_Male_Pandaren_LETL_Intro_01 = new AssetReference("VO_TaranZhu_Male_Pandaren_LETL_Intro_01.prefab:ebc4380e2cb8cbc469b30f3120c89106");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Intro_01,
      (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Attack_01,
      (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Attack_02,
      (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Idle_01,
      (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_812H_VoHandler lt23812HVoHandler = this;
    while (lt23812HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23812HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_812H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_812H")
    {
      string str = cardID;
      if (!(str == "LETL_024P3_03"))
      {
        if (!(str == "LETL_024P3_05"))
        {
          if (!(str == "LT22_007P2_03"))
          {
            if (str == "LT22_007P2_05")
            {
              GameState.Get().SetBusy(true);
              yield return (object) lt23812HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Attack_01);
              GameState.Get().SetBusy(false);
            }
          }
          else
          {
            GameState.Get().SetBusy(true);
            yield return (object) lt23812HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Attack_01);
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt23812HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23812HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_812H_VoHandler.VO_TaranZhu_Male_Pandaren_LETL_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_812H_VoHandler lt23812HVoHandler = this;
    while (lt23812HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23812HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_812H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23812HVoHandler.MissionPlayVO(playByDesignCode, lt23812HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23812HVoHandler.MissionPlayVO(playByDesignCode, lt23812HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23812HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_812H_VoHandler lt23812HVoHandler = this;
    while (lt23812HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23812HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_812H");
    if (entity.GetCardId() == "LT23_812H")
      yield return (object) lt23812HVoHandler.MissionPlaySound(playByDesignCode, lt23812HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_812H_VoHandler lt23812HVoHandler = this;
    while (lt23812HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23812HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_812H");
    if (turn == 1)
      yield return (object) lt23812HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23812HVoHandler.m_introLine);
  }
}
