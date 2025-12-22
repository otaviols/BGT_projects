using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_819H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Sayge_Male_Worgen_LETL_Attack_01 = new AssetReference("VO_Sayge_Male_Worgen_LETL_Attack_01.prefab:6d2e139b5b565e141ad42c091c38bbe0");
  private static readonly AssetReference VO_Sayge_Male_Worgen_LETL_Attack_02 = new AssetReference("VO_Sayge_Male_Worgen_LETL_Attack_02.prefab:dcadfa8053fbc8a46b1667f71e7ca649");
  private static readonly AssetReference VO_Sayge_Male_Worgen_LETL_Death_01 = new AssetReference("VO_Sayge_Male_Worgen_LETL_Death_01.prefab:d91b98f921720c44280c65300443675d");
  private static readonly AssetReference VO_Sayge_Male_Worgen_LETL_Idle_01 = new AssetReference("VO_Sayge_Male_Worgen_LETL_Idle_01.prefab:4aae4ce13b6081a498c0b0e3ff02a06e");
  private static readonly AssetReference VO_Sayge_Male_Worgen_LETL_Intro_01 = new AssetReference("VO_Sayge_Male_Worgen_LETL_Intro_01.prefab:b3ceffbd3f497a84e8e517f126e24f2e");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_819H_VoHandler.VO_Sayge_Male_Worgen_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_819H_VoHandler.VO_Sayge_Male_Worgen_LETL_Intro_01,
      (string) LettuceBoss_LT23_819H_VoHandler.VO_Sayge_Male_Worgen_LETL_Attack_01,
      (string) LettuceBoss_LT23_819H_VoHandler.VO_Sayge_Male_Worgen_LETL_Attack_02,
      (string) LettuceBoss_LT23_819H_VoHandler.VO_Sayge_Male_Worgen_LETL_Idle_01,
      (string) LettuceBoss_LT23_819H_VoHandler.VO_Sayge_Male_Worgen_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_819H_VoHandler.VO_Sayge_Male_Worgen_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_819H_VoHandler.VO_Sayge_Male_Worgen_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_819H_VoHandler lt23819HVoHandler = this;
    while (lt23819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23819HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_819H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_819H")
    {
      string str = cardID;
      if (!(str == "LT23_819P1"))
      {
        if (str == "LT23_819P2")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt23819HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_819H_VoHandler.VO_Sayge_Male_Worgen_LETL_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23819HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_819H_VoHandler.VO_Sayge_Male_Worgen_LETL_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_819H_VoHandler lt23819HVoHandler = this;
    while (lt23819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23819HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_819H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23819HVoHandler.MissionPlayVO(playByDesignCode, lt23819HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23819HVoHandler.MissionPlayVO(playByDesignCode, lt23819HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23819HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_819H_VoHandler lt23819HVoHandler = this;
    while (lt23819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23819HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_819H");
    if (entity.GetCardId() == "LT23_819H")
      yield return (object) lt23819HVoHandler.MissionPlaySound(playByDesignCode, lt23819HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_819H_VoHandler lt23819HVoHandler = this;
    while (lt23819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23819HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_819H");
    if (turn == 1)
      yield return (object) lt23819HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23819HVoHandler.m_introLine);
  }
}
