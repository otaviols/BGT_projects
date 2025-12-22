using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_814H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_TheCrone_Female_Troll_LETL_Attack_01 = new AssetReference("VO_TheCrone_Female_Troll_LETL_Attack_01.prefab:cf12f060b6b570a44aca02ca5fc42695");
  private static readonly AssetReference VO_TheCrone_Female_Troll_LETL_Attack_02 = new AssetReference("VO_TheCrone_Female_Troll_LETL_Attack_02.prefab:88c2d1b42348b164a8d3ff2ec6624d9c");
  private static readonly AssetReference VO_TheCrone_Female_Troll_LETL_Death_01 = new AssetReference("VO_TheCrone_Female_Troll_LETL_Death_01.prefab:5e016d46563961f43a21fa397fa9b168");
  private static readonly AssetReference VO_TheCrone_Female_Troll_LETL_Idle_01 = new AssetReference("VO_TheCrone_Female_Troll_LETL_Idle_01.prefab:9119cca1c2d92e04ab89b9f4356aecce");
  private static readonly AssetReference VO_TheCrone_Female_Troll_LETL_Idle_02 = new AssetReference("VO_TheCrone_Female_Troll_LETL_Idle_02.prefab:1d38b49b0c1fb3645af8455b9f213606");
  private static readonly AssetReference VO_TheCrone_Female_Troll_LETL_Intro_01 = new AssetReference("VO_TheCrone_Female_Troll_LETL_Intro_01.prefab:a4359fe0647297a43a7857e7524e90d9");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Idle_01,
    (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Idle_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Intro_01,
      (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Idle_01,
      (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Idle_02,
      (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Attack_01,
      (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Attack_02,
      (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_814H_VoHandler lt24814HVoHandler = this;
    while (lt24814HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24814HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_814H")
    {
      string str = cardID;
      if (!(str == "LT24_814P1"))
      {
        if (str == "LT24_814P2")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24814HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Attack_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24814HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_814H_VoHandler.VO_TheCrone_Female_Troll_LETL_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_814H_VoHandler lt24814HVoHandler = this;
    while (lt24814HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24814HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24814HVoHandler.MissionPlayVO(playByDesignCode, lt24814HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24814HVoHandler.MissionPlayVO(playByDesignCode, lt24814HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24814HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_814H_VoHandler lt24814HVoHandler = this;
    while (lt24814HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24814HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H");
    if (entity.GetCardId() == "LT24_814H")
      yield return (object) lt24814HVoHandler.MissionPlaySound(playByDesignCode, lt24814HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_814H_VoHandler lt24814HVoHandler = this;
    while (lt24814HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24814HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H");
    if (turn == 1)
      yield return (object) lt24814HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24814HVoHandler.m_introLine);
  }
}
