using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_839H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_BRMA04_1_CARD_04 = new AssetReference("VO_BRMA04_1_CARD_04.prefab:53f20ec5598fc8a459615f6a57c661be");
  private static readonly AssetReference VO_BRMA04_1_HERO_POWER_05 = new AssetReference("VO_BRMA04_1_HERO_POWER_05.prefab:1c2e947768a86424abf65a8b5ad573ec");
  private static readonly AssetReference VO_BRMA04_1_RESPONSE_03 = new AssetReference("VO_BRMA04_1_RESPONSE_03.prefab:75a029ecfd071914aaf0def7bc041b85");
  private static readonly AssetReference VO_BRMA04_1_DEATH_06 = new AssetReference("VO_BRMA04_1_DEATH_06.prefab:34e63d08fa3428e4091c5cdbe63dd894");
  private static readonly AssetReference VO_BRMA04_1_START_01 = new AssetReference("VO_BRMA04_1_START_01.prefab:5d9de41d8c48c924a88ff1a539711761");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_839H_VoHandler.VO_BRMA04_1_RESPONSE_03
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_839H_VoHandler.VO_BRMA04_1_CARD_04,
      (string) LettuceBoss_LETL_839H_VoHandler.VO_BRMA04_1_HERO_POWER_05,
      (string) LettuceBoss_LETL_839H_VoHandler.VO_BRMA04_1_RESPONSE_03,
      (string) LettuceBoss_LETL_839H_VoHandler.VO_BRMA04_1_DEATH_06,
      (string) LettuceBoss_LETL_839H_VoHandler.VO_BRMA04_1_START_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_839H_VoHandler.VO_BRMA04_1_START_01;
    this.m_deathLine = (string) LettuceBoss_LETL_839H_VoHandler.VO_BRMA04_1_DEATH_06;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_839H_VoHandler letl839HVoHandler = this;
    while (letl839HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl839HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_839H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_839H")
    {
      string str = cardID;
      if (!(str == "LETL_839P1_01") && !(str == "LETL_839P1_03"))
      {
        if (str == "LETL_839P2_01" || str == "LETL_839P2_03")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl839HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_839H_VoHandler.VO_BRMA04_1_HERO_POWER_05);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl839HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_839H_VoHandler.VO_BRMA04_1_CARD_04);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_839H_VoHandler letl839HVoHandler = this;
    while (letl839HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl839HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_839H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl839HVoHandler.MissionPlayVO(playByDesignCode, letl839HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl839HVoHandler.MissionPlayVO(playByDesignCode, letl839HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl839HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_839H_VoHandler letl839HVoHandler = this;
    while (letl839HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl839HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_839H");
    if (entity.GetCardId() == "LETL_839H")
      yield return (object) letl839HVoHandler.MissionPlaySound(playByDesignCode, letl839HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_839H_VoHandler letl839HVoHandler = this;
    while (letl839HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl839HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_839H");
    if (turn == 1)
      yield return (object) letl839HVoHandler.MissionPlayVOOnce(playByDesignCode, letl839HVoHandler.m_introLine);
  }
}
