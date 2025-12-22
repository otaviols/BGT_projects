using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_804H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Chogall_Male_Ogre_LETL_Attack_01 = new AssetReference("VO_Chogall_Male_Ogre_LETL_Attack_01.prefab:e5892e80ee1ce2b4780b3e683861b3ae");
  private static readonly AssetReference VO_Chogall_Male_Ogre_LETL_Bark_10 = new AssetReference("VO_Chogall_Male_Ogre_LETL_Bark_10.prefab:634c29628d74b184ea0f002825b125b5");
  private static readonly AssetReference VO_Chogall_Male_Ogre_LETL_Death_01 = new AssetReference("VO_Chogall_Male_Ogre_LETL_Death_01.prefab:952b9f8ac3a59264788dd9c2fb2ea3ad");
  private static readonly AssetReference VO_Chogall_Male_Ogre_LETL_Intro_01 = new AssetReference("VO_Chogall_Male_Ogre_LETL_Intro_01.prefab:e91e8f71c07e801479c142da012a3de7");
  private static readonly AssetReference VO_Chogall_Male_Ogre_LETL_Bark_03 = new AssetReference("VO_Chogall_Male_Ogre_LETL_Bark_03.prefab:3e8041fbd1a87e04782637267cfa7c5f");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_804H_VoHandler.VO_Chogall_Male_Ogre_LETL_Bark_10
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_804H_VoHandler.VO_Chogall_Male_Ogre_LETL_Attack_01,
      (string) LettuceBoss_LT23_804H_VoHandler.VO_Chogall_Male_Ogre_LETL_Bark_10,
      (string) LettuceBoss_LT23_804H_VoHandler.VO_Chogall_Male_Ogre_LETL_Death_01,
      (string) LettuceBoss_LT23_804H_VoHandler.VO_Chogall_Male_Ogre_LETL_Intro_01,
      (string) LettuceBoss_LT23_804H_VoHandler.VO_Chogall_Male_Ogre_LETL_Bark_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_804H_VoHandler.VO_Chogall_Male_Ogre_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_804H_VoHandler.VO_Chogall_Male_Ogre_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_804H_VoHandler lt23804HVoHandler = this;
    while (lt23804HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode1 = lt23804HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_804H");
    Actor playByDesignCode2 = lt23804HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_804H2");
    string cardId = playedEntity.GetLettuceAbilityOwner().GetCardId();
    if (cardId == "LT23_804H")
    {
      if (cardID == "LT23_804P1")
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt23804HVoHandler.MissionPlayVO(playByDesignCode1, (string) LettuceBoss_LT23_804H_VoHandler.VO_Chogall_Male_Ogre_LETL_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
    else if (cardId == "LT23_804H2" && cardID == "LT23_804P2")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23804HVoHandler.MissionPlayVO(playByDesignCode2, (string) LettuceBoss_LT23_804H_VoHandler.VO_Chogall_Male_Ogre_LETL_Bark_03);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_804H_VoHandler lt23804HVoHandler = this;
    while (lt23804HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23804HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_804H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23804HVoHandler.MissionPlayVO(playByDesignCode, lt23804HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23804HVoHandler.MissionPlayVO(playByDesignCode, lt23804HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23804HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_804H_VoHandler lt23804HVoHandler = this;
    while (lt23804HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23804HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_804H");
    if (entity.GetCardId() == "LT23_804H")
      yield return (object) lt23804HVoHandler.MissionPlaySound(playByDesignCode, lt23804HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_804H_VoHandler lt23804HVoHandler = this;
    while (lt23804HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23804HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_804H");
    if (turn == 1)
      yield return (object) lt23804HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23804HVoHandler.m_introLine);
  }
}
