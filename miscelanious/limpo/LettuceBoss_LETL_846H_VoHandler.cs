using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_846H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_846H_Male_Dragon_Attack_01 = new AssetReference("VO_LETL_846H_Male_Dragon_Attack_01.prefab:3753e37b081972f4794f890a095b54ed");
  private static readonly AssetReference VO_LETL_846H_Male_Dragon_Attack_02 = new AssetReference("VO_LETL_846H_Male_Dragon_Attack_02.prefab:9912dbea914f6014d888d084a5de343c");
  private static readonly AssetReference VO_LETL_846H_Male_Dragon_Death_01 = new AssetReference("VO_LETL_846H_Male_Dragon_Death_01.prefab:bf61206cd0f7c0343b5f8b8641f4afb9");
  private static readonly AssetReference VO_LETL_846H_Male_Dragon_Idle_01 = new AssetReference("VO_LETL_846H_Male_Dragon_Idle_01.prefab:712f13da3a24e284385e2f1b5e77f4cf");
  private static readonly AssetReference VO_LETL_846H_Male_Dragon_Intro_01 = new AssetReference("VO_LETL_846H_Male_Dragon_Intro_01.prefab:60a635df010d16d4cb359f1320cd3938");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_846H_VoHandler.VO_LETL_846H_Male_Dragon_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_846H_VoHandler.VO_LETL_846H_Male_Dragon_Attack_01,
      (string) LettuceBoss_LETL_846H_VoHandler.VO_LETL_846H_Male_Dragon_Attack_02,
      (string) LettuceBoss_LETL_846H_VoHandler.VO_LETL_846H_Male_Dragon_Death_01,
      (string) LettuceBoss_LETL_846H_VoHandler.VO_LETL_846H_Male_Dragon_Idle_01,
      (string) LettuceBoss_LETL_846H_VoHandler.VO_LETL_846H_Male_Dragon_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_846H_VoHandler.VO_LETL_846H_Male_Dragon_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_846H_VoHandler.VO_LETL_846H_Male_Dragon_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_846H_VoHandler letl846HVoHandler = this;
    while (letl846HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl846HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_846H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_846H")
    {
      string str = cardID;
      if (!(str == "LETL_846P3_01"))
      {
        if (str == "LETL_846P4_01")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl846HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_846H_VoHandler.VO_LETL_846H_Male_Dragon_Attack_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl846HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_846H_VoHandler.VO_LETL_846H_Male_Dragon_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_846H_VoHandler letl846HVoHandler = this;
    while (letl846HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl846HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_846H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl846HVoHandler.MissionPlayVO(playByDesignCode, letl846HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl846HVoHandler.MissionPlayVO(playByDesignCode, letl846HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl846HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_846H_VoHandler letl846HVoHandler = this;
    while (letl846HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl846HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_846H");
    if (entity.GetCardId() == "LETL_846H")
      yield return (object) letl846HVoHandler.MissionPlaySound(playByDesignCode, letl846HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_846H_VoHandler letl846HVoHandler = this;
    while (letl846HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl846HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_846H");
    if (turn == 1)
      yield return (object) letl846HVoHandler.MissionPlayVOOnce(playByDesignCode, letl846HVoHandler.m_introLine);
  }
}
