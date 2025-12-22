using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_812H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_BabblingBook_Male_Book_LETL_Attack_01 = new AssetReference("VO_BabblingBook_Male_Book_LETL_Attack_01.prefab:4aa04bc94175ccf4f944b17223dbf371");
  private static readonly AssetReference VO_BabblingBook_Male_Book_LETL_Attack_02 = new AssetReference("VO_BabblingBook_Male_Book_LETL_Attack_02.prefab:475300d85a200414d8dda4b424fa3094");
  private static readonly AssetReference VO_BabblingBook_Male_Book_LETL_Death_01 = new AssetReference("VO_BabblingBook_Male_Book_LETL_Death_01.prefab:e9e557f30bc74384c818a379934e199a");
  private static readonly AssetReference VO_BabblingBook_Male_Book_LETL_Idle_01 = new AssetReference("VO_BabblingBook_Male_Book_LETL_Idle_01.prefab:ad4f2cbb8d12ac04d8ae6f0a905367da");
  private static readonly AssetReference VO_BabblingBook_Male_Book_LETL_Idle_02 = new AssetReference("VO_BabblingBook_Male_Book_LETL_Idle_02.prefab:d94b919d5199a0742afbb930f7c99432");
  private static readonly AssetReference VO_BabblingBook_Male_Book_LETL_Idle_03 = new AssetReference("VO_BabblingBook_Male_Book_LETL_Idle_03.prefab:4ac360177f629584eb633cbb323e5cb8");
  private static readonly AssetReference VO_BabblingBook_Male_Book_LETL_Intro_01 = new AssetReference("VO_BabblingBook_Male_Book_LETL_Intro_01.prefab:8c426406fefbb5a4ca332a09af199536");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Idle_01,
    (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Idle_02,
    (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Idle_03
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Intro_01,
      (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Idle_01,
      (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Idle_02,
      (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Idle_03,
      (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Attack_01,
      (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Attack_02,
      (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_812H_VoHandler lt24812HVoHandler = this;
    while (lt24812HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24812HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_812H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_812H")
    {
      string str = cardID;
      if (!(str == "LT24_812P1"))
      {
        if (str == "LETL_1139")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24812HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24812HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_812H_VoHandler.VO_BabblingBook_Male_Book_LETL_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_812H_VoHandler lt24812HVoHandler = this;
    while (lt24812HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24812HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_812H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24812HVoHandler.MissionPlayVO(playByDesignCode, lt24812HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24812HVoHandler.MissionPlayVO(playByDesignCode, lt24812HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24812HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_812H_VoHandler lt24812HVoHandler = this;
    while (lt24812HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24812HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_812H");
    if (entity.GetCardId() == "LT24_812H")
      yield return (object) lt24812HVoHandler.MissionPlaySound(playByDesignCode, lt24812HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_812H_VoHandler lt24812HVoHandler = this;
    while (lt24812HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24812HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_812H");
    if (turn == 1)
      yield return (object) lt24812HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24812HVoHandler.m_introLine);
  }
}
