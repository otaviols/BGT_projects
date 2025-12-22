using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_860H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Kazakus_Male_Troll_Bark_12 = new AssetReference("VO_Kazakus_Male_Troll_Bark_12.prefab:0ada67e6e7d73344899cea2a354b7631");
  private static readonly AssetReference VO_BOM_08_007_Male_Troll_Kazakus_InGame_Introduction_01_B = new AssetReference("VO_BOM_08_007_Male_Troll_Kazakus_InGame_Introduction_01_B.prefab:151dd32e1c9ba4d48922a963336322eb");
  private static readonly AssetReference VO_BOM_08_008_Male_Dragon_Kazakusan_InGame_Turn_07_01_A = new AssetReference("VO_BOM_08_008_Male_Dragon_Kazakusan_InGame_Turn_07_01_A.prefab:bdc62bdd39e32954dad4ecd20846fcd6");
  private static readonly AssetReference VO_Kazakus_Male_Troll_Bark_19 = new AssetReference("VO_Kazakus_Male_Troll_Bark_19.prefab:15fa5594bea41544f84f0d5a5b6727ca");
  private static readonly AssetReference VO_Kazakus_Male_Troll_Bark_20 = new AssetReference("VO_Kazakus_Male_Troll_Bark_20.prefab:2a126bc2e07b4c141887a955daccfbad");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_860H_VoHandler.VO_Kazakus_Male_Troll_Bark_12
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_860H_VoHandler.VO_Kazakus_Male_Troll_Bark_12,
      (string) LettuceBoss_LETL_860H_VoHandler.VO_BOM_08_007_Male_Troll_Kazakus_InGame_Introduction_01_B,
      (string) LettuceBoss_LETL_860H_VoHandler.VO_BOM_08_008_Male_Dragon_Kazakusan_InGame_Turn_07_01_A,
      (string) LettuceBoss_LETL_860H_VoHandler.VO_Kazakus_Male_Troll_Bark_19,
      (string) LettuceBoss_LETL_860H_VoHandler.VO_Kazakus_Male_Troll_Bark_20
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_860H_VoHandler.VO_BOM_08_007_Male_Troll_Kazakus_InGame_Introduction_01_B;
    this.m_deathLine = (string) LettuceBoss_LETL_860H_VoHandler.VO_Kazakus_Male_Troll_Bark_19;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_860H_VoHandler letl860HVoHandler = this;
    while (letl860HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode1 = letl860HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_860H");
    Actor playByDesignCode2 = letl860HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_860H4");
    string cardId = playedEntity.GetLettuceAbilityOwner().GetCardId();
    if (cardId == "LETL_860H")
    {
      string str = cardID;
      if (str == "LETL_860P2" || str == "LETL_860P2_05")
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl860HVoHandler.MissionPlayVO(playByDesignCode1, (string) LettuceBoss_LETL_860H_VoHandler.VO_Kazakus_Male_Troll_Bark_20);
        GameState.Get().SetBusy(false);
      }
    }
    else if (cardId == "LETL_860H4" && cardID == "LETL_860P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl860HVoHandler.MissionPlayVO(playByDesignCode2, (string) LettuceBoss_LETL_860H_VoHandler.VO_BOM_08_008_Male_Dragon_Kazakusan_InGame_Turn_07_01_A);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_860H_VoHandler letl860HVoHandler = this;
    while (letl860HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl860HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_860H");
    Actor bossActor2 = letl860HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_860H4");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl860HVoHandler.MissionPlayVO(playByDesignCode, letl860HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl860HVoHandler.MissionPlayVO(playByDesignCode, letl860HVoHandler.m_IdleLines);
        yield return (object) letl860HVoHandler.MissionPlayVO(bossActor2, letl860HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl860HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_860H_VoHandler letl860HVoHandler = this;
    while (letl860HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl860HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_860H");
    if (entity.GetCardId() == "LETL_860H")
      yield return (object) letl860HVoHandler.MissionPlaySound(playByDesignCode, letl860HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_860H_VoHandler letl860HVoHandler = this;
    while (letl860HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl860HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_860H");
    if (turn == 1)
      yield return (object) letl860HVoHandler.MissionPlayVOOnce(playByDesignCode, letl860HVoHandler.m_introLine);
  }
}
