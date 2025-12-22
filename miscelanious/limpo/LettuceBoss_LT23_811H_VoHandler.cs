using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_811H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Nzoth_Male_OldGod_LETL_Attack_02 = new AssetReference("VO_Nzoth_Male_OldGod_LETL_Attack_02.prefab:e4720d82d6fd43c40b4fbcdc63da9aa1");
  private static readonly AssetReference VO_Nzoth_Male_OldGod_LETL_Attack_03 = new AssetReference("VO_Nzoth_Male_OldGod_LETL_Attack_03.prefab:1665d46ff6166744093a431c1a363e17");
  private static readonly AssetReference VO_Nzoth_Male_OldGod_LETL_Death_02 = new AssetReference("VO_Nzoth_Male_OldGod_LETL_Death_02.prefab:518ff870305e0d843832a22aa2a91805");
  private static readonly AssetReference VO_Nzoth_Male_OldGod_LETL_Idle_01 = new AssetReference("VO_Nzoth_Male_OldGod_LETL_Idle_01.prefab:ec2de2a7dac642d4d9dad9c527ad851d");
  private static readonly AssetReference VO_Nzoth_Male_OldGod_LETL_Intro_02 = new AssetReference("VO_Nzoth_Male_OldGod_LETL_Intro_02.prefab:624c8ec0b7a4e6b40a1c46129de3aacf");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_811H_VoHandler.VO_Nzoth_Male_OldGod_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_811H_VoHandler.VO_Nzoth_Male_OldGod_LETL_Intro_02,
      (string) LettuceBoss_LT23_811H_VoHandler.VO_Nzoth_Male_OldGod_LETL_Attack_02,
      (string) LettuceBoss_LT23_811H_VoHandler.VO_Nzoth_Male_OldGod_LETL_Attack_03,
      (string) LettuceBoss_LT23_811H_VoHandler.VO_Nzoth_Male_OldGod_LETL_Idle_01,
      (string) LettuceBoss_LT23_811H_VoHandler.VO_Nzoth_Male_OldGod_LETL_Death_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_811H_VoHandler.VO_Nzoth_Male_OldGod_LETL_Intro_02;
    this.m_deathLine = (string) LettuceBoss_LT23_811H_VoHandler.VO_Nzoth_Male_OldGod_LETL_Death_02;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_811H_VoHandler lt23811HVoHandler = this;
    while (lt23811HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23811HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_811H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_811H")
    {
      string str = cardID;
      if (!(str == "LT23_811P1"))
      {
        if (str == "LT23_811P2")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt23811HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_811H_VoHandler.VO_Nzoth_Male_OldGod_LETL_Attack_03);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23811HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_811H_VoHandler.VO_Nzoth_Male_OldGod_LETL_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_811H_VoHandler lt23811HVoHandler = this;
    while (lt23811HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23811HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_811H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23811HVoHandler.MissionPlayVO(playByDesignCode, lt23811HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23811HVoHandler.MissionPlayVO(playByDesignCode, lt23811HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23811HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_811H_VoHandler lt23811HVoHandler = this;
    while (lt23811HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23811HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_811H");
    if (entity.GetCardId() == "LT23_811H")
      yield return (object) lt23811HVoHandler.MissionPlaySound(playByDesignCode, lt23811HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_811H_VoHandler lt23811HVoHandler = this;
    while (lt23811HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23811HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_811H");
    if (turn == 1)
      yield return (object) lt23811HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23811HVoHandler.m_introLine);
  }
}
