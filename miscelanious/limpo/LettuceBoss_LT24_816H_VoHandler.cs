using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_816H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference NightbaneBoss_Attack_1 = new AssetReference("NightbaneBoss_Attack_1.prefab:e2bf2eaf82c5ecf46922c19e35deb0ae");
  private static readonly AssetReference NightbaneBoss_Death_1 = new AssetReference("NightbaneBoss_Death_1.prefab:626bcca9d3329b04396c679564669a01");
  private static readonly AssetReference NightbaneBoss_Start_1 = new AssetReference("NightbaneBoss_Start_1.prefab:7968327ff2a98c64fa4abd973e1c4c56");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_816H_VoHandler.NightbaneBoss_Attack_1
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_816H_VoHandler.NightbaneBoss_Start_1,
      (string) LettuceBoss_LT24_816H_VoHandler.NightbaneBoss_Attack_1,
      (string) LettuceBoss_LT24_816H_VoHandler.NightbaneBoss_Death_1
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_816H_VoHandler.NightbaneBoss_Start_1;
    this.m_deathLine = (string) LettuceBoss_LT24_816H_VoHandler.NightbaneBoss_Death_1;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_816H_VoHandler lt24816HVoHandler = this;
    while (lt24816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24816HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_816H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_816H" && cardID == "LT24_816P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt24816HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_816H_VoHandler.NightbaneBoss_Attack_1);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_816H_VoHandler lt24816HVoHandler = this;
    while (lt24816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24816HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_816H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24816HVoHandler.MissionPlayVO(playByDesignCode, lt24816HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24816HVoHandler.MissionPlayVO(playByDesignCode, lt24816HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24816HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_816H_VoHandler lt24816HVoHandler = this;
    while (lt24816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24816HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_816H");
    if (entity.GetCardId() == "LT24_816H")
      yield return (object) lt24816HVoHandler.MissionPlaySound(playByDesignCode, lt24816HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_816H_VoHandler lt24816HVoHandler = this;
    while (lt24816HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24816HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_816H");
    if (turn == 1)
      yield return (object) lt24816HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24816HVoHandler.m_introLine);
  }
}
