using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_842H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_BRMA07_1_CARD_04 = new AssetReference("VO_BRMA07_1_CARD_04.prefab:f498bd13724f67d48a0f0bc55034c44b");
  private static readonly AssetReference VO_BRMA07_1_HERO_POWER_05 = new AssetReference("VO_BRMA07_1_HERO_POWER_05.prefab:10f8a1b1fc7c9374b8c2b741f27694be");
  private static readonly AssetReference VO_BRMA07_1_RESPONSE_03 = new AssetReference("VO_BRMA07_1_RESPONSE_03.prefab:b43d82ce7a9bb59438b594dd3c185050");
  private static readonly AssetReference VO_BRMA07_1_TURN1_02 = new AssetReference("VO_BRMA07_1_TURN1_02.prefab:ac11bf2418c6e0f418f2216348b224c3");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_842H_VoHandler.VO_BRMA07_1_TURN1_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_842H_VoHandler.VO_BRMA07_1_CARD_04,
      (string) LettuceBoss_LETL_842H_VoHandler.VO_BRMA07_1_HERO_POWER_05,
      (string) LettuceBoss_LETL_842H_VoHandler.VO_BRMA07_1_RESPONSE_03,
      (string) LettuceBoss_LETL_842H_VoHandler.VO_BRMA07_1_TURN1_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_842H_VoHandler.VO_BRMA07_1_CARD_04;
    this.m_deathLine = (string) LettuceBoss_LETL_842H_VoHandler.VO_BRMA07_1_RESPONSE_03;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_842H_VoHandler letl842HVoHandler = this;
    while (letl842HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl842HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_842H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_842H")
    {
      string str = cardID;
      if (str == "LETL_842P1_01" || str == "LETL_842P1_02")
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl842HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_842H_VoHandler.VO_BRMA07_1_HERO_POWER_05);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_842H_VoHandler letl842HVoHandler = this;
    while (letl842HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl842HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_842H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl842HVoHandler.MissionPlayVO(playByDesignCode, letl842HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl842HVoHandler.MissionPlayVO(playByDesignCode, letl842HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl842HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_842H_VoHandler letl842HVoHandler = this;
    while (letl842HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl842HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_842H");
    if (entity.GetCardId() == "LETL_842H")
      yield return (object) letl842HVoHandler.MissionPlaySound(playByDesignCode, letl842HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_842H_VoHandler letl842HVoHandler = this;
    while (letl842HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl842HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_842H");
    if (turn == 1)
      yield return (object) letl842HVoHandler.MissionPlayVOOnce(playByDesignCode, letl842HVoHandler.m_introLine);
  }
}
