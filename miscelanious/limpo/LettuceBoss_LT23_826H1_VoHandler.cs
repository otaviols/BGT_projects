using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_826H1_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_QueenAzshara_Female_Naga_LETL_Death_02 = new AssetReference("VO_QueenAzshara_Female_Naga_LETL_Death_02.prefab:84763ca6799ad3d4fbc0c51918e35dc6");
  private static readonly AssetReference VO_QueenAzshara_Female_Naga_LETL_Special_01 = new AssetReference("VO_QueenAzshara_Female_Naga_LETL_Special_01.prefab:145a720717c30eb49aea146f6a2b4bd5");
  private static readonly AssetReference VO_QueenAzshara_Female_Naga_LETL_Idle_01 = new AssetReference("VO_QueenAzshara_Female_Naga_LETL_Idle_01.prefab:35fc3059e56ea7f43853c636b00cc888");
  private static readonly AssetReference VO_QueenAzshara_Female_Naga_LETL_Attack_01 = new AssetReference("VO_QueenAzshara_Female_Naga_LETL_Attack_01.prefab:f978e9b5b96321441b9575b492aef691");
  private static readonly AssetReference VO_QueenAzshara_Female_Naga_LETL_Attack_02 = new AssetReference("VO_QueenAzshara_Female_Naga_LETL_Attack_02.prefab:406561acf76d05741a1e3eb9366114e2");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_826H1_VoHandler.VO_QueenAzshara_Female_Naga_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_826H1_VoHandler.VO_QueenAzshara_Female_Naga_LETL_Special_01,
      (string) LettuceBoss_LT23_826H1_VoHandler.VO_QueenAzshara_Female_Naga_LETL_Death_02,
      (string) LettuceBoss_LT23_826H1_VoHandler.VO_QueenAzshara_Female_Naga_LETL_Idle_01,
      (string) LettuceBoss_LT23_826H1_VoHandler.VO_QueenAzshara_Female_Naga_LETL_Attack_01,
      (string) LettuceBoss_LT23_826H1_VoHandler.VO_QueenAzshara_Female_Naga_LETL_Attack_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_826H1_VoHandler.VO_QueenAzshara_Female_Naga_LETL_Special_01;
    this.m_deathLine = (string) LettuceBoss_LT23_826H1_VoHandler.VO_QueenAzshara_Female_Naga_LETL_Death_02;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_826H1_VoHandler lt23826H1VoHandler = this;
    while (lt23826H1VoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23826H1VoHandler.FindEnemyActorInPlayByDesignCode("LT23_826H1");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_826H1")
    {
      string str = cardID;
      if (!(str == "LT23_826P3"))
      {
        if (str == "LT23_826P1")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt23826H1VoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_826H1_VoHandler.VO_QueenAzshara_Female_Naga_LETL_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23826H1VoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_826H1_VoHandler.VO_QueenAzshara_Female_Naga_LETL_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_826H1_VoHandler lt23826H1VoHandler = this;
    while (lt23826H1VoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23826H1VoHandler.FindEnemyActorInPlayByDesignCode("LT23_826H1");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23826H1VoHandler.MissionPlayVO(playByDesignCode, lt23826H1VoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23826H1VoHandler.MissionPlayVO(playByDesignCode, lt23826H1VoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23826H1VoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_826H1_VoHandler lt23826H1VoHandler = this;
    while (lt23826H1VoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23826H1VoHandler.FindEnemyActorInPlayByDesignCode("LT23_826H1");
    if (entity.GetCardId() == "LT23_826H1")
      yield return (object) lt23826H1VoHandler.MissionPlaySound(playByDesignCode, lt23826H1VoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_826H1_VoHandler lt23826H1VoHandler = this;
    while (lt23826H1VoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23826H1VoHandler.FindEnemyActorInPlayByDesignCode("LT23_826H1");
    if (turn == 1)
      yield return (object) lt23826H1VoHandler.MissionPlayVOOnce(playByDesignCode, lt23826H1VoHandler.m_introLine);
  }
}
