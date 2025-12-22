using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_845H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_845H_Male_Dragonkin_Attack_01 = new AssetReference("VO_LETL_845H_Male_Dragonkin_Attack_01.prefab:29cfb93ab47fa53429eae273eb82f4b8");
  private static readonly AssetReference VO_LETL_845H_Male_Dragonkin_Attack_02 = new AssetReference("VO_LETL_845H_Male_Dragonkin_Attack_02.prefab:17a6278444341804fa4f139a485307c4");
  private static readonly AssetReference VO_LETL_845H_Male_Dragonkin_Death_01 = new AssetReference("VO_LETL_845H_Male_Dragonkin_Death_01.prefab:4bc4aa1243216744eb2c0ec6076b31f1");
  private static readonly AssetReference VO_LETL_845H_Male_Dragonkin_Idle_01 = new AssetReference("VO_LETL_845H_Male_Dragonkin_Idle_01.prefab:8273ad8b09ecfbf4283955e65726fc5b");
  private static readonly AssetReference VO_LETL_845H_Male_Dragonkin_Intro_01 = new AssetReference("VO_LETL_845H_Male_Dragonkin_Intro_01.prefab:4438e0df50187164688d35ccf015c358");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_845H_VoHandler.VO_LETL_845H_Male_Dragonkin_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_845H_VoHandler.VO_LETL_845H_Male_Dragonkin_Attack_01,
      (string) LettuceBoss_LETL_845H_VoHandler.VO_LETL_845H_Male_Dragonkin_Attack_02,
      (string) LettuceBoss_LETL_845H_VoHandler.VO_LETL_845H_Male_Dragonkin_Death_01,
      (string) LettuceBoss_LETL_845H_VoHandler.VO_LETL_845H_Male_Dragonkin_Idle_01,
      (string) LettuceBoss_LETL_845H_VoHandler.VO_LETL_845H_Male_Dragonkin_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_845H_VoHandler.VO_LETL_845H_Male_Dragonkin_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_845H_VoHandler.VO_LETL_845H_Male_Dragonkin_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_845H_VoHandler letl845HVoHandler = this;
    while (letl845HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl845HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_845H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_845H")
    {
      string str = cardID;
      if (!(str == "LETL_844P1_01"))
      {
        if (str == "LETL_844P2_01")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl845HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_845H_VoHandler.VO_LETL_845H_Male_Dragonkin_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl845HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_845H_VoHandler.VO_LETL_845H_Male_Dragonkin_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_845H_VoHandler letl845HVoHandler = this;
    while (letl845HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl845HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_845H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl845HVoHandler.MissionPlayVO(playByDesignCode, letl845HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl845HVoHandler.MissionPlayVO(playByDesignCode, letl845HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl845HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_845H_VoHandler letl845HVoHandler = this;
    while (letl845HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl845HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_845H");
    if (entity.GetCardId() == "LETL_845H")
      yield return (object) letl845HVoHandler.MissionPlaySound(playByDesignCode, letl845HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_845H_VoHandler letl845HVoHandler = this;
    while (letl845HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl845HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_845H");
    if (turn == 1)
      yield return (object) letl845HVoHandler.MissionPlayVOOnce(playByDesignCode, letl845HVoHandler.m_introLine);
  }
}
