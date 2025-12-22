using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_831H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_831H_Male_NightElf_Attack_01 = new AssetReference("VO_LETL_831H_Male_NightElf_Attack_01.prefab:ae8635d9bf07bd4409fc1774d89646f7");
  private static readonly AssetReference VO_LETL_831H_Male_NightElf_Attack_02 = new AssetReference("VO_LETL_831H_Male_NightElf_Attack_02.prefab:8ffc464b1ec07944c81ef6d35e65e450");
  private static readonly AssetReference VO_LETL_831H_Male_NightElf_Death_01 = new AssetReference("VO_LETL_831H_Male_NightElf_Death_01.prefab:b97ee5fb1e543974d92f4310fbd899ca");
  private static readonly AssetReference VO_LETL_831H_Male_NightElf_Idle_01 = new AssetReference("VO_LETL_831H_Male_NightElf_Idle_01.prefab:234f92d5bc87dbe429a9241995d7e905");
  private static readonly AssetReference VO_LETL_831H_Male_NightElf_Intro_01 = new AssetReference("VO_LETL_831H_Male_NightElf_Intro_01.prefab:55a6e14f344f6e545a55c3081566227e");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_831H_VoHandler.VO_LETL_831H_Male_NightElf_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_831H_VoHandler.VO_LETL_831H_Male_NightElf_Attack_01,
      (string) LettuceBoss_LETL_831H_VoHandler.VO_LETL_831H_Male_NightElf_Attack_02,
      (string) LettuceBoss_LETL_831H_VoHandler.VO_LETL_831H_Male_NightElf_Death_01,
      (string) LettuceBoss_LETL_831H_VoHandler.VO_LETL_831H_Male_NightElf_Idle_01,
      (string) LettuceBoss_LETL_831H_VoHandler.VO_LETL_831H_Male_NightElf_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_831H_VoHandler.VO_LETL_831H_Male_NightElf_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_831H_VoHandler.VO_LETL_831H_Male_NightElf_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_831H_VoHandler letl831HVoHandler = this;
    while (letl831HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl831HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_831H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_831H")
    {
      string str = cardID;
      if (!(str == "LETL_015P9_05"))
      {
        if (str == "LETL_013P3_05" || str == "LETL_017P7_05")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl831HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_831H_VoHandler.VO_LETL_831H_Male_NightElf_Attack_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl831HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_831H_VoHandler.VO_LETL_831H_Male_NightElf_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_831H_VoHandler letl831HVoHandler = this;
    while (letl831HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl831HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_831H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl831HVoHandler.MissionPlayVO(playByDesignCode, letl831HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl831HVoHandler.MissionPlayVO(playByDesignCode, letl831HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl831HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_831H_VoHandler letl831HVoHandler = this;
    while (letl831HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl831HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_831H");
    if (entity.GetCardId() == "LETL_831H")
      yield return (object) letl831HVoHandler.MissionPlaySound(playByDesignCode, letl831HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_831H_VoHandler letl831HVoHandler = this;
    while (letl831HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl831HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_831H");
    if (turn == 1)
      yield return (object) letl831HVoHandler.MissionPlayVOOnce(playByDesignCode, letl831HVoHandler.m_introLine);
  }
}
