using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_829H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_829H_Male_Demon_Attack_01 = new AssetReference("VO_LETL_829H_Male_Demon_Attack_01.prefab:1dc063478901cf846ae820c98611e4d8");
  private static readonly AssetReference VO_LETL_829H_Male_Demon_Attack_02 = new AssetReference("VO_LETL_829H_Male_Demon_Attack_02.prefab:4fc3f2159b643d34494367700dbf4631");
  private static readonly AssetReference VO_LETL_829H_Male_Demon_Death_01 = new AssetReference("VO_LETL_829H_Male_Demon_Death_01.prefab:445efbb1910d80b4eb28d194429454e3");
  private static readonly AssetReference VO_LETL_829H_Male_Demon_Idle_01 = new AssetReference("VO_LETL_829H_Male_Demon_Idle_01.prefab:d4bcd1f1eda309347bee26c28c99c2f6");
  private static readonly AssetReference VO_LETL_829H_Male_Demon_Intro_01 = new AssetReference("VO_LETL_829H_Male_Demon_Intro_01.prefab:50a57448ed7ab5d4187ef5e2ccf01a2e");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_829H_VoHandler.VO_LETL_829H_Male_Demon_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_829H_VoHandler.VO_LETL_829H_Male_Demon_Attack_01,
      (string) LettuceBoss_LETL_829H_VoHandler.VO_LETL_829H_Male_Demon_Attack_02,
      (string) LettuceBoss_LETL_829H_VoHandler.VO_LETL_829H_Male_Demon_Death_01,
      (string) LettuceBoss_LETL_829H_VoHandler.VO_LETL_829H_Male_Demon_Idle_01,
      (string) LettuceBoss_LETL_829H_VoHandler.VO_LETL_829H_Male_Demon_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_829H_VoHandler.VO_LETL_829H_Male_Demon_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_829H_VoHandler.VO_LETL_829H_Male_Demon_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_829H_VoHandler letl829HVoHandler = this;
    while (letl829HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl829HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_829H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_829H")
    {
      string str = cardID;
      if (!(str == "LETL_829P1_02") && !(str == "LETL_829P1_05"))
      {
        if (str == "LETL_018P7_03")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl829HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_829H_VoHandler.VO_LETL_829H_Male_Demon_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl829HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_829H_VoHandler.VO_LETL_829H_Male_Demon_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_829H_VoHandler letl829HVoHandler = this;
    while (letl829HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl829HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_829H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl829HVoHandler.MissionPlayVO(playByDesignCode, letl829HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl829HVoHandler.MissionPlayVO(playByDesignCode, letl829HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl829HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_829H_VoHandler letl829HVoHandler = this;
    while (letl829HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl829HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_829H");
    if (entity.GetCardId() == "LETL_829H")
      yield return (object) letl829HVoHandler.MissionPlaySound(playByDesignCode, letl829HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_829H_VoHandler letl829HVoHandler = this;
    while (letl829HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl829HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_829H");
    if (turn == 1)
      yield return (object) letl829HVoHandler.MissionPlayVOOnce(playByDesignCode, letl829HVoHandler.m_introLine);
  }
}
