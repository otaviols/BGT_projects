using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_815H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Barnes_Male_Human_LETL_C03_T05_Dialogue_01 = new AssetReference("VO_Barnes_Male_Human_LETL_C03_T05_Dialogue_01.prefab:e2311d48f799a044d92d18ccd7976be0");
  private static readonly AssetReference KAR_114_Male_Human_Attack_01 = new AssetReference("KAR_114_Male_Human_Attack_01.prefab:fa49b48d923085f499520d64f32e2807");
  private static readonly AssetReference KAR_114_Male_Human_Death_01 = new AssetReference("KAR_114_Male_Human_Death_01.prefab:fd181cc333977384db5c38e503a700fc");
  private static readonly AssetReference KAR_114_Male_Human_Idle_01 = new AssetReference("KAR_114_Male_Human_Idle_01.prefab:1427869adc205c045bb07c8e5eb0235b");
  private static readonly AssetReference KAR_114_Male_Human_Idle_02 = new AssetReference("KAR_114_Male_Human_Idle_02.prefab:741b0ec8194d05049a13d93672155523");
  private static readonly AssetReference KAR_114_Male_Human_Idle_03 = new AssetReference("KAR_114_Male_Human_Idle_03.prefab:586f74f59db5d1249bd6e8d52c6cafe3");
  private static readonly AssetReference KAR_114_Male_Human_Idle_04 = new AssetReference("KAR_114_Male_Human_Idle_04.prefab:de5de7465397c8c4caabd9f6398b43ee");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Idle_01,
    (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Idle_02,
    (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Idle_04
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_815H_VoHandler.VO_Barnes_Male_Human_LETL_C03_T05_Dialogue_01,
      (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Idle_01,
      (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Idle_02,
      (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Idle_04,
      (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Attack_01,
      (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Idle_03,
      (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_815H_VoHandler.VO_Barnes_Male_Human_LETL_C03_T05_Dialogue_01;
    this.m_deathLine = (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_815H_VoHandler lt24815HVoHandler = this;
    while (lt24815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24815HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_815H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_815H")
    {
      string str = cardID;
      if (!(str == "LT24_815P1"))
      {
        if (str == "LT23_820P1")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24815HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Attack_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24815HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_815H_VoHandler.KAR_114_Male_Human_Idle_03);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_815H_VoHandler lt24815HVoHandler = this;
    while (lt24815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24815HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_815H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24815HVoHandler.MissionPlayVO(playByDesignCode, lt24815HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24815HVoHandler.MissionPlayVO(playByDesignCode, lt24815HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24815HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_815H_VoHandler lt24815HVoHandler = this;
    while (lt24815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24815HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_815H");
    if (entity.GetCardId() == "LT24_815H")
      yield return (object) lt24815HVoHandler.MissionPlaySound(playByDesignCode, lt24815HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_815H_VoHandler lt24815HVoHandler = this;
    while (lt24815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24815HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_815H");
    if (turn == 1)
      yield return (object) lt24815HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24815HVoHandler.m_introLine);
  }
}
