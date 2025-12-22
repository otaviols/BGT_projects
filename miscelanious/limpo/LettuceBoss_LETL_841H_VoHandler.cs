using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_841H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_BRMA06_3_INTRO_01 = new AssetReference("VO_BRMA06_3_INTRO_01.prefab:ccee32264258cd14f9875a94ff81d0ea");
  private static readonly AssetReference VO_BRMA06_3_RESPONSE_03 = new AssetReference("VO_BRMA06_3_RESPONSE_03.prefab:3abe0ccef6f202a45b4727361bc704df");
  private static readonly AssetReference VO_BRMA06_3_TURN1_02 = new AssetReference("VO_BRMA06_3_TURN1_02.prefab:7d7272a7a2a62bf4f91488020ed8ab94");
  private static readonly AssetReference VO_EX1_298_Attack_02 = new AssetReference("VO_EX1_298_Attack_02.prefab:7dd0c364ae8f57049bf82c4c94b72292");
  private static readonly AssetReference VO_EX1_298_Death_04 = new AssetReference("VO_EX1_298_Death_04.prefab:5dd820a21c877bc4693cf0ec8837a555");
  private static readonly AssetReference VO_EX1_298_Play_01 = new AssetReference("VO_EX1_298_Play_01.prefab:2c96b78abf795554e9cfe5643cab2141");
  private static readonly AssetReference VO_EX1_298_Trigger_03 = new AssetReference("VO_EX1_298_Trigger_03.prefab:b95e45a2eafca924786a3e53344bc9f5");
  private static readonly AssetReference VO_BRMA06_1_DEATH_04 = new AssetReference("VO_BRMA06_1_DEATH_04.prefab:78c2973f7c025a641bb953654e358879");
  private static readonly AssetReference VO_BRMA06_1_RESPONSE_03 = new AssetReference("VO_BRMA06_1_RESPONSE_03.prefab:a908e5d8056a26b4dbdc0ea833f19a6e");
  private static readonly AssetReference VO_BRMA06_1_SUMMON_RAG_05 = new AssetReference("VO_BRMA06_1_SUMMON_RAG_05.prefab:e79eafab2edcfe2428e817359ec11c65");
  private static readonly AssetReference VO_BRMA06_1_TURN1_02 = new AssetReference("VO_BRMA06_1_TURN1_02.prefab:76b698614db27b14c8ebac0e4d01b6f9");
  private static readonly AssetReference VO_BRMA06_1_TURN1_02_ALT = new AssetReference("VO_BRMA06_1_TURN1_02_ALT.prefab:e0ae95e6abc774f4b9bc68f07f7bbc29");
  private static readonly AssetReference VO_BRMA05_1_CARD_05 = new AssetReference("VO_BRMA05_1_CARD_05.prefab:c0bc2f9cc3d3ae047ba80ffa0f70dcb8");
  private static readonly AssetReference VO_BRMA05_1_DEATH_04 = new AssetReference("VO_BRMA05_1_DEATH_04.prefab:48366fa92e2fb6648b45700ce40715b7");
  private static readonly AssetReference VO_BRMA05_1_HERO_POWER_06 = new AssetReference("VO_BRMA05_1_HERO_POWER_06.prefab:2792e43708ba1df48baa3a41d636097a");
  private static readonly AssetReference VO_BRMA05_1_RESPONSE_03 = new AssetReference("VO_BRMA05_1_RESPONSE_03.prefab:beac5b0620de49f42a2f2a66a906d4d6");
  private static readonly AssetReference VO_BRMA05_1_START_01 = new AssetReference("VO_BRMA05_1_START_01.prefab:590531d432b26ed46a1b36981630723d");
  private static readonly AssetReference VO_BRMA05_1_TURN1_02 = new AssetReference("VO_BRMA05_1_TURN1_02.prefab:b68353491d7f88a4a8479e7a031aec12");
  private static readonly AssetReference VO_BRMA04_1_CARD_04 = new AssetReference("VO_BRMA04_1_CARD_04.prefab:53f20ec5598fc8a459615f6a57c661be");
  private static readonly AssetReference VO_BRMA04_1_HERO_POWER_05 = new AssetReference("VO_BRMA04_1_HERO_POWER_05.prefab:1c2e947768a86424abf65a8b5ad573ec");
  private static readonly AssetReference VO_BRMA04_1_RESPONSE_03 = new AssetReference("VO_BRMA04_1_RESPONSE_03.prefab:75a029ecfd071914aaf0def7bc041b85");
  private static readonly AssetReference VO_BRMA04_1_DEATH_06 = new AssetReference("VO_BRMA04_1_DEATH_06.prefab:34e63d08fa3428e4091c5cdbe63dd894");
  private static readonly AssetReference VO_BRMA04_1_START_01 = new AssetReference("VO_BRMA04_1_START_01.prefab:5d9de41d8c48c924a88ff1a539711761");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_1_RESPONSE_03
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_3_INTRO_01,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_3_RESPONSE_03,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_3_TURN1_02,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_EX1_298_Attack_02,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_EX1_298_Death_04,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_EX1_298_Play_01,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_EX1_298_Trigger_03,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_1_DEATH_04,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_1_RESPONSE_03,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_1_SUMMON_RAG_05,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_1_TURN1_02,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_1_TURN1_02_ALT,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA05_1_CARD_05,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA05_1_DEATH_04,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA05_1_HERO_POWER_06,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA05_1_RESPONSE_03,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA05_1_START_01,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA05_1_TURN1_02,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA04_1_CARD_04,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA04_1_HERO_POWER_05,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA04_1_RESPONSE_03,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA04_1_DEATH_06,
      (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA04_1_START_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_1_TURN1_02_ALT;
    this.m_deathLine = (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_1_SUMMON_RAG_05;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_841H_VoHandler letl841HVoHandler = this;
    while (letl841HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl841HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_841H");
    Actor guest1bossActor = letl841HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_840H");
    Actor guest2bossActor = letl841HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_839H");
    string designCode = playedEntity.GetLettuceAbilityOwner().GetCardId();
    if (designCode == "LETL_841H")
    {
      string str = cardID;
      if (str == "LETL_452_03" || str == "LETL_452_05")
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl841HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA06_1_TURN1_02);
        GameState.Get().SetBusy(false);
      }
    }
    if (designCode == "LETL_840H")
    {
      string str = cardID;
      if (!(str == "LETL_030P2_04") && !(str == "LETL_030P2_05"))
      {
        if (str == "LETL_030P4_04" || str == "LETL_030P4_05")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl841HVoHandler.MissionPlayVOOnce(guest1bossActor, (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA05_1_RESPONSE_03);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl841HVoHandler.MissionPlayVOOnce(guest1bossActor, (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA05_1_HERO_POWER_06);
        GameState.Get().SetBusy(false);
      }
    }
    if (designCode == "LETL_839H")
    {
      string str = cardID;
      if (!(str == "LETL_839P1_01") && !(str == "LETL_839P1_03"))
      {
        if (str == "LETL_839P2_01" || str == "LETL_839P2_03")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl841HVoHandler.MissionPlayVOOnce(guest2bossActor, (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA04_1_HERO_POWER_05);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl841HVoHandler.MissionPlayVOOnce(guest2bossActor, (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA04_1_CARD_04);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_841H_VoHandler letl841HVoHandler = this;
    while (letl841HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl841HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_841H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl841HVoHandler.MissionPlayVO(playByDesignCode, letl841HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl841HVoHandler.MissionPlayVO(playByDesignCode, letl841HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl841HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_841H_VoHandler letl841HVoHandler = this;
    while (letl841HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode1 = letl841HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_841H");
    Actor playByDesignCode2 = letl841HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_840H");
    Actor playByDesignCode3 = letl841HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_839H");
    Actor playByDesignCode4 = letl841HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_841H2");
    Actor playByDesignCode5 = letl841HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_841H2_Heroic");
    string cardId = entity.GetCardId();
    if (!(cardId == "LETL_841H"))
    {
      if (!(cardId == "LETL_840H"))
      {
        if (!(cardId == "LETL_839H"))
        {
          if (!(cardId == "LETL_841H2"))
          {
            if (cardId == "LETL_841H2_Heroic")
              yield return (object) letl841HVoHandler.MissionPlaySound(playByDesignCode5, (string) LettuceBoss_LETL_841H_VoHandler.VO_EX1_298_Death_04);
          }
          else
            yield return (object) letl841HVoHandler.MissionPlaySound(playByDesignCode4, (string) LettuceBoss_LETL_841H_VoHandler.VO_EX1_298_Death_04);
        }
        else
          yield return (object) letl841HVoHandler.MissionPlaySound(playByDesignCode3, (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA04_1_DEATH_06);
      }
      else
        yield return (object) letl841HVoHandler.MissionPlaySound(playByDesignCode2, (string) LettuceBoss_LETL_841H_VoHandler.VO_BRMA05_1_DEATH_04);
    }
    else
      yield return (object) letl841HVoHandler.MissionPlaySound(playByDesignCode1, letl841HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_841H_VoHandler letl841HVoHandler = this;
    while (letl841HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl841HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_841H");
    if (turn == 1)
      yield return (object) letl841HVoHandler.MissionPlayVOOnce(playByDesignCode, letl841HVoHandler.m_introLine);
  }
}
