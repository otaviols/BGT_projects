using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_818H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_ShadeofAran_Male_Undead_LETL_Attack_01 = new AssetReference("VO_ShadeofAran_Male_Undead_LETL_Attack_01.prefab:b4917909bf4823942995ec35065c8594");
  private static readonly AssetReference VO_ShadeofAran_Male_Undead_LETL_Attack_02 = new AssetReference("VO_ShadeofAran_Male_Undead_LETL_Attack_02.prefab:01eafa730a650cc40a5110ef3972dfa1");
  private static readonly AssetReference VO_ShadeofAran_Male_Undead_LETL_Death_01 = new AssetReference("VO_ShadeofAran_Male_Undead_LETL_Death_01.prefab:0519375b43afaa941b4da6374d3933a8");
  private static readonly AssetReference VO_ShadeofAran_Male_Undead_LETL_Idle_01 = new AssetReference("VO_ShadeofAran_Male_Undead_LETL_Idle_01.prefab:af2aee734a9518c4883fb77c8eaeb48c");
  private static readonly AssetReference VO_ShadeofAran_Male_Undead_LETL_Idle_02 = new AssetReference("VO_ShadeofAran_Male_Undead_LETL_Idle_02.prefab:7db9ff3a4b902254597138b69b70ad81");
  private static readonly AssetReference VO_ShadeofAran_Male_Undead_LETL_Intro_01 = new AssetReference("VO_ShadeofAran_Male_Undead_LETL_Intro_01.prefab:7322048d76f42964bb67a78a01cd5ab7");
  private static readonly AssetReference VO_ShadeofAran_Male_Undead_LETL_Intro_02 = new AssetReference("VO_ShadeofAran_Male_Undead_LETL_Intro_02.prefab:cf36cf4ccbc257742822063a8327f505");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Idle_01,
    (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Idle_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Intro_01,
      (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Attack_01,
      (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Attack_02,
      (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Idle_01,
      (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Idle_02,
      (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_818H_VoHandler lt24818HVoHandler = this;
    while (lt24818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24818HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_818H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_818H")
    {
      string str = cardID;
      if (!(str == "LT24_818P1"))
      {
        if (str == "LT23_803P2")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24818HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24818HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_818H_VoHandler.VO_ShadeofAran_Male_Undead_LETL_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_818H_VoHandler lt24818HVoHandler = this;
    while (lt24818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24818HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_818H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24818HVoHandler.MissionPlayVO(playByDesignCode, lt24818HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24818HVoHandler.MissionPlayVO(playByDesignCode, lt24818HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24818HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_818H_VoHandler lt24818HVoHandler = this;
    while (lt24818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24818HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_818H");
    if (entity.GetCardId() == "LT24_818H")
      yield return (object) lt24818HVoHandler.MissionPlaySound(playByDesignCode, lt24818HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_818H_VoHandler lt24818HVoHandler = this;
    while (lt24818HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24818HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_818H");
    if (turn == 1)
      yield return (object) lt24818HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24818HVoHandler.m_introLine);
  }
}
