using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_825H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference CowKing_TB_SPT_DPromo_Hero2_Death = new AssetReference("CowKing_TB_SPT_DPromo_Hero2_Death.prefab:62dd0de3b827da94c9550809489a97c6");
  private static readonly AssetReference CowKing_TB_SPT_DPromo_Hero2_Play = new AssetReference("CowKing_TB_SPT_DPromo_Hero2_Play.prefab:2e748a031af8a6d46a9aa1a35da82756");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_825H_VoHandler.CowKing_TB_SPT_DPromo_Hero2_Play
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_825H_VoHandler.CowKing_TB_SPT_DPromo_Hero2_Play,
      (string) LettuceBoss_LT23_825H_VoHandler.CowKing_TB_SPT_DPromo_Hero2_Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_825H_VoHandler.CowKing_TB_SPT_DPromo_Hero2_Play;
    this.m_deathLine = (string) LettuceBoss_LT23_825H_VoHandler.CowKing_TB_SPT_DPromo_Hero2_Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_825H_VoHandler lt23825HVoHandler = this;
    while (lt23825HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23825HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_825H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_825H" && cardID == "LT23_825P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23825HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_825H_VoHandler.CowKing_TB_SPT_DPromo_Hero2_Play);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_825H_VoHandler lt23825HVoHandler = this;
    while (lt23825HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23825HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_825H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23825HVoHandler.MissionPlayVO(playByDesignCode, lt23825HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23825HVoHandler.MissionPlayVO(playByDesignCode, lt23825HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23825HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_825H_VoHandler lt23825HVoHandler = this;
    while (lt23825HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23825HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_825H");
    if (entity.GetCardId() == "LT23_825H")
      yield return (object) lt23825HVoHandler.MissionPlaySound(playByDesignCode, lt23825HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_825H_VoHandler lt23825HVoHandler = this;
    while (lt23825HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23825HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_825H");
    if (turn == 1)
      yield return (object) lt23825HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23825HVoHandler.m_introLine);
  }
}
