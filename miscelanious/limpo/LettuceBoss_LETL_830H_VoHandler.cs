using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_830H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_830H_Male_Furbolg_Attack_01 = new AssetReference("VO_LETL_830H_Male_Furbolg_Attack_01.prefab:aba0a4e9dde66c14b8b52730d39b5a11");
  private static readonly AssetReference VO_LETL_830H_Male_Furbolg_Attack_02 = new AssetReference("VO_LETL_830H_Male_Furbolg_Attack_02.prefab:65296488cfeafd24db0ab1d1429bbcce");
  private static readonly AssetReference VO_LETL_830H_Male_Furbolg_Death_01 = new AssetReference("VO_LETL_830H_Male_Furbolg_Death_01.prefab:9b9afc28f664394409635d64bedfe4a7");
  private static readonly AssetReference VO_LETL_830H_Male_Furbolg_Idle_01 = new AssetReference("VO_LETL_830H_Male_Furbolg_Idle_01.prefab:afb99a8946e2a544993a8899187658e6");
  private static readonly AssetReference VO_LETL_830H_Male_Furbolg_Intro_01 = new AssetReference("VO_LETL_830H_Male_Furbolg_Intro_01.prefab:74558d35e57a3594593f8f8efc30dfad");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_830H_VoHandler.VO_LETL_830H_Male_Furbolg_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_830H_VoHandler.VO_LETL_830H_Male_Furbolg_Attack_01,
      (string) LettuceBoss_LETL_830H_VoHandler.VO_LETL_830H_Male_Furbolg_Attack_02,
      (string) LettuceBoss_LETL_830H_VoHandler.VO_LETL_830H_Male_Furbolg_Death_01,
      (string) LettuceBoss_LETL_830H_VoHandler.VO_LETL_830H_Male_Furbolg_Idle_01,
      (string) LettuceBoss_LETL_830H_VoHandler.VO_LETL_830H_Male_Furbolg_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_830H_VoHandler.VO_LETL_830H_Male_Furbolg_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_830H_VoHandler.VO_LETL_830H_Male_Furbolg_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_830H_VoHandler letl830HVoHandler = this;
    while (letl830HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl830HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_830H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_830H")
    {
      string str = cardID;
      if (!(str == "LETL_830P1_01") && !(str == "LETL_830P1_03"))
      {
        if (str == "LETL_830P2_01" || str == "LETL_830P2_03")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl830HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_830H_VoHandler.VO_LETL_830H_Male_Furbolg_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl830HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_830H_VoHandler.VO_LETL_830H_Male_Furbolg_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_830H_VoHandler letl830HVoHandler = this;
    while (letl830HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl830HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_830H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl830HVoHandler.MissionPlayVO(playByDesignCode, letl830HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl830HVoHandler.MissionPlayVO(playByDesignCode, letl830HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl830HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_830H_VoHandler letl830HVoHandler = this;
    while (letl830HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl830HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_830H");
    if (entity.GetCardId() == "LETL_830H")
      yield return (object) letl830HVoHandler.MissionPlaySound(playByDesignCode, letl830HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_830H_VoHandler letl830HVoHandler = this;
    while (letl830HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl830HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_830H");
    if (turn == 1)
      yield return (object) letl830HVoHandler.MissionPlayVOOnce(playByDesignCode, letl830HVoHandler.m_introLine);
  }
}
