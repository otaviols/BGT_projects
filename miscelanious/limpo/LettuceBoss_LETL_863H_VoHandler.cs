using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_863H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_ONY_030_Female_Human_Attack_01 = new AssetReference("VO_ONY_030_Female_Human_Attack_01.prefab:7c4a9b15ca2909446b31f9e19c5bb033");
  private static readonly AssetReference VO_ONY_030_Female_Human_Death_01 = new AssetReference("VO_ONY_030_Female_Human_Death_01.prefab:0e3da644e0e19de48aa5087847722734");
  private static readonly AssetReference VO_ONY_030_Female_Human_Play_01 = new AssetReference("VO_ONY_030_Female_Human_Play_01.prefab:1410616ecc4a1db4d9a7381d4c341d7f");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_863H_VoHandler.VO_ONY_030_Female_Human_Play_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_863H_VoHandler.VO_ONY_030_Female_Human_Attack_01,
      (string) LettuceBoss_LETL_863H_VoHandler.VO_ONY_030_Female_Human_Death_01,
      (string) LettuceBoss_LETL_863H_VoHandler.VO_ONY_030_Female_Human_Play_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_863H_VoHandler.VO_ONY_030_Female_Human_Play_01;
    this.m_deathLine = (string) LettuceBoss_LETL_863H_VoHandler.VO_ONY_030_Female_Human_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_863H_VoHandler letl863HVoHandler = this;
    while (letl863HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl863HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_863H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_863H" && cardID == "LETL_863P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl863HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_863H_VoHandler.VO_ONY_030_Female_Human_Attack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_863H_VoHandler letl863HVoHandler = this;
    while (letl863HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl863HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_863H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl863HVoHandler.MissionPlayVO(playByDesignCode, letl863HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl863HVoHandler.MissionPlayVO(playByDesignCode, letl863HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl863HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_863H_VoHandler letl863HVoHandler = this;
    while (letl863HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl863HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_863H");
    if (entity.GetCardId() == "LETL_863H")
      yield return (object) letl863HVoHandler.MissionPlaySound(playByDesignCode, letl863HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_863H_VoHandler letl863HVoHandler = this;
    while (letl863HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl863HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_863H");
    if (turn == 1)
      yield return (object) letl863HVoHandler.MissionPlayVOOnce(playByDesignCode, letl863HVoHandler.m_introLine);
  }
}
