using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_844H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_844H_Male_Orc_Attack_01 = new AssetReference("VO_LETL_844H_Male_Orc_Attack_01.prefab:9b627bbeeeba8434995cc21a170d8614");
  private static readonly AssetReference VO_LETL_844H_Male_Orc_Attack_02 = new AssetReference("VO_LETL_844H_Male_Orc_Attack_02.prefab:fa840a9a5fc89d94ab5404ed9d3a83cf");
  private static readonly AssetReference VO_LETL_844H_Male_Orc_Death_01 = new AssetReference("VO_LETL_844H_Male_Orc_Death_01.prefab:5344bd97a54057e45a27fb97fd52a978");
  private static readonly AssetReference VO_LETL_844H_Male_Orc_Idle_01 = new AssetReference("VO_LETL_844H_Male_Orc_Idle_01.prefab:2ab3528369cab9f408cfeee79827e23a");
  private static readonly AssetReference VO_LETL_844H_Male_Orc_Intro_01 = new AssetReference("VO_LETL_844H_Male_Orc_Intro_01.prefab:ec04d635155da764f8759e507f26a481");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_844H_VoHandler.VO_LETL_844H_Male_Orc_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_844H_VoHandler.VO_LETL_844H_Male_Orc_Attack_01,
      (string) LettuceBoss_LETL_844H_VoHandler.VO_LETL_844H_Male_Orc_Attack_02,
      (string) LettuceBoss_LETL_844H_VoHandler.VO_LETL_844H_Male_Orc_Death_01,
      (string) LettuceBoss_LETL_844H_VoHandler.VO_LETL_844H_Male_Orc_Idle_01,
      (string) LettuceBoss_LETL_844H_VoHandler.VO_LETL_844H_Male_Orc_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_844H_VoHandler.VO_LETL_844H_Male_Orc_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_844H_VoHandler.VO_LETL_844H_Male_Orc_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_844H_VoHandler letl844HVoHandler = this;
    while (letl844HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl844HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_844H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_844H")
    {
      string str = cardID;
      if (!(str == "LETL_033P5_03"))
      {
        if (str == "LETLT_080_02")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl844HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_844H_VoHandler.VO_LETL_844H_Male_Orc_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl844HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_844H_VoHandler.VO_LETL_844H_Male_Orc_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_844H_VoHandler letl844HVoHandler = this;
    while (letl844HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl844HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_844H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl844HVoHandler.MissionPlayVO(playByDesignCode, letl844HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl844HVoHandler.MissionPlayVO(playByDesignCode, letl844HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl844HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_844H_VoHandler letl844HVoHandler = this;
    while (letl844HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl844HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_844H");
    if (entity.GetCardId() == "LETL_844H")
      yield return (object) letl844HVoHandler.MissionPlaySound(playByDesignCode, letl844HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_844H_VoHandler letl844HVoHandler = this;
    while (letl844HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl844HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_844H");
    if (turn == 1)
      yield return (object) letl844HVoHandler.MissionPlayVOOnce(playByDesignCode, letl844HVoHandler.m_introLine);
  }
}
