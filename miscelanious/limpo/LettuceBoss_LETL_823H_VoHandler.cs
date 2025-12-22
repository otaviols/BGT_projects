using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_823H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Death_01 = new AssetReference("VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Death_01.prefab:82ef51bddcc36c6428db7199870653fe");
  private static readonly AssetReference VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7ExchangeC_02 = new AssetReference("VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7ExchangeC_02.prefab:c2b8afe65ef23c143bd1a34161cc187d");
  private static readonly AssetReference VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_01 = new AssetReference("VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_01.prefab:007b130ddcc08ca4e9924be455bc2734");
  private static readonly AssetReference VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_02 = new AssetReference("VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_02.prefab:8bc50b80bf51d6844b8cc4c89573eaaf");
  private static readonly AssetReference VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_03 = new AssetReference("VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_03.prefab:f05a7e6f1db294347a677c9c1cdb14fd");
  private static readonly AssetReference VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_01 = new AssetReference("VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_01.prefab:88b0f8abdf5585f47a6e744d8391fbf0");
  private static readonly AssetReference VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_02 = new AssetReference("VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_02.prefab:0bb351ffc71261d49a7c311b5c08f9dd");
  private static readonly AssetReference VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_03 = new AssetReference("VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_03.prefab:c7e17e42871307f4bae3b2d6be0f3fc3");
  private static readonly AssetReference VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Loss_01 = new AssetReference("VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Loss_01.prefab:e1544fc02d4059748aac550938999bac");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_01,
    (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_02,
    (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_03
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Death_01,
      (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7ExchangeC_02,
      (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_01,
      (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_02,
      (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_03,
      (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_01,
      (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_02,
      (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Idle_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_03;
    this.m_deathLine = (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_823H_VoHandler letl823HVoHandler = this;
    while (letl823HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl823HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_823H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_823H")
    {
      string str = cardID;
      if (!(str == "LETL_823P2_01"))
      {
        if (str == "LETL_823P1_01")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl823HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl823HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_823H_VoHandler.VO_Story_Hero_Neeru_Male_Orc_Story_Kurtrus_Mission7HeroPower_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_823H_VoHandler letl823HVoHandler = this;
    while (letl823HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl823HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_823H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl823HVoHandler.MissionPlayVO(playByDesignCode, letl823HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl823HVoHandler.MissionPlayVO(playByDesignCode, letl823HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl823HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_823H_VoHandler letl823HVoHandler = this;
    while (letl823HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl823HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_823H");
    if (entity.GetCardId() == "LETL_823H")
      yield return (object) letl823HVoHandler.MissionPlaySound(playByDesignCode, letl823HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_823H_VoHandler letl823HVoHandler = this;
    while (letl823HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl823HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_823H");
    if (turn == 1)
      yield return (object) letl823HVoHandler.MissionPlayVOOnce(playByDesignCode, letl823HVoHandler.m_introLine);
  }
}
