using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_816H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_816H_Male_Elemental_Attack_01 = new AssetReference("VO_LETL_816H_Male_Elemental_Attack_01.prefab:7a37cda844e99754cb63defec34fa2f7");
  private static readonly AssetReference VO_LETL_816H_Male_Elemental_Attack_02 = new AssetReference("VO_LETL_816H_Male_Elemental_Attack_02.prefab:1a7ebd8913bc70e499d2a2d0d117a014");
  private static readonly AssetReference VO_LETL_816H_Male_Elemental_Death_01 = new AssetReference("VO_LETL_816H_Male_Elemental_Death_01.prefab:f1528eb6e8224ef4abfce3fdf19fa532");
  private static readonly AssetReference VO_LETL_816H_Male_Elemental_Idle_01 = new AssetReference("VO_LETL_816H_Male_Elemental_Idle_01.prefab:da4dd45a4ee171e46a47e5336d76ca3b");
  private static readonly AssetReference VO_LETL_816H_Male_Elemental_Intro_01 = new AssetReference("VO_LETL_816H_Male_Elemental_Intro_01.prefab:fc1eb6f6a155248458b305c17efedb73");
  private static readonly AssetReference VO_LETL_816H_Male_Elemental_Intro_02 = new AssetReference("VO_LETL_816H_Male_Elemental_Intro_02.prefab:233a9d840e918a04b89c5e8b9ceb04d5");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Attack_01,
      (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Attack_02,
      (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Death_01,
      (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Idle_01,
      (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Intro_01,
      (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Intro_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_816H_VoHandler letl816HVoHandler = this;
    while (letl816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl816HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_816H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_816H")
    {
      string str = cardID;
      if (!(str == "LETL_816P1_01") && !(str == "LETL_816P1_03"))
      {
        if (str == "LETL_816P2_01")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl816HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl816HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_816H_VoHandler.VO_LETL_816H_Male_Elemental_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_816H_VoHandler letl816HVoHandler = this;
    while (letl816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl816HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_816H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl816HVoHandler.MissionPlayVO(playByDesignCode, letl816HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl816HVoHandler.MissionPlayVO(playByDesignCode, letl816HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl816HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_816H_VoHandler letl816HVoHandler = this;
    while (letl816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl816HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_816H");
    if (entity.GetCardId() == "LETL_816H")
      yield return (object) letl816HVoHandler.MissionPlaySound(playByDesignCode, letl816HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_816H_VoHandler letl816HVoHandler = this;
    while (letl816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl816HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_816H");
    if (turn == 1)
      yield return (object) letl816HVoHandler.MissionPlayVOOnce(playByDesignCode, letl816HVoHandler.m_introLine);
  }
}
