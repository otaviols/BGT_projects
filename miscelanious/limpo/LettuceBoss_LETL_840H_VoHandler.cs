using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_840H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_BRMA05_1_CARD_05 = new AssetReference("VO_BRMA05_1_CARD_05.prefab:c0bc2f9cc3d3ae047ba80ffa0f70dcb8");
  private static readonly AssetReference VO_BRMA05_1_DEATH_04 = new AssetReference("VO_BRMA05_1_DEATH_04.prefab:48366fa92e2fb6648b45700ce40715b7");
  private static readonly AssetReference VO_BRMA05_1_HERO_POWER_06 = new AssetReference("VO_BRMA05_1_HERO_POWER_06.prefab:2792e43708ba1df48baa3a41d636097a");
  private static readonly AssetReference VO_BRMA05_1_RESPONSE_03 = new AssetReference("VO_BRMA05_1_RESPONSE_03.prefab:beac5b0620de49f42a2f2a66a906d4d6");
  private static readonly AssetReference VO_BRMA05_1_START_01 = new AssetReference("VO_BRMA05_1_START_01.prefab:590531d432b26ed46a1b36981630723d");
  private static readonly AssetReference VO_BRMA05_1_TURN1_02 = new AssetReference("VO_BRMA05_1_TURN1_02.prefab:b68353491d7f88a4a8479e7a031aec12");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_TURN1_02,
    (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_CARD_05
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_CARD_05,
      (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_DEATH_04,
      (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_HERO_POWER_06,
      (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_RESPONSE_03,
      (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_START_01,
      (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_TURN1_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_START_01;
    this.m_deathLine = (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_DEATH_04;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_840H_VoHandler letl840HVoHandler = this;
    while (letl840HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl840HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_840H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_840H")
    {
      string str = cardID;
      if (!(str == "LETL_030P2_04") && !(str == "LETL_030P2_05"))
      {
        if (str == "LETL_030P4_04" || str == "LETL_030P4_05")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl840HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_RESPONSE_03);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl840HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_840H_VoHandler.VO_BRMA05_1_HERO_POWER_06);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_840H_VoHandler letl840HVoHandler = this;
    while (letl840HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl840HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_840H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl840HVoHandler.MissionPlayVO(playByDesignCode, letl840HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl840HVoHandler.MissionPlayVO(playByDesignCode, letl840HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl840HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_840H_VoHandler letl840HVoHandler = this;
    while (letl840HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl840HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_840H");
    if (entity.GetCardId() == "LETL_840H")
      yield return (object) letl840HVoHandler.MissionPlaySound(playByDesignCode, letl840HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_840H_VoHandler letl840HVoHandler = this;
    while (letl840HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl840HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_840H");
    if (turn == 1)
      yield return (object) letl840HVoHandler.MissionPlayVOOnce(playByDesignCode, letl840HVoHandler.m_introLine);
  }
}
