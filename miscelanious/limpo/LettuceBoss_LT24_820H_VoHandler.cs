using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_820H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_WhiteKing_LETL_Attack = new AssetReference("VO_WhiteKing_LETL_Attack.prefab:4ead53188c2fb934b830fd8b37b78118");
  private static readonly AssetReference VO_WhiteKing_LETL_Death = new AssetReference("VO_WhiteKing_LETL_Death.prefab:88f9fe615cfa00a47bcce2f593ccf1a2");
  private static readonly AssetReference VO_WhiteKing_LETL_Idle = new AssetReference("VO_WhiteKing_LETL_Idle.prefab:739a12af67d94f44fa7872344d46b888");
  private static readonly AssetReference VO_WhiteKing_LETL_Intro = new AssetReference("VO_WhiteKing_LETL_Intro.prefab:ab96a6b632c39594faa8d7a61e5117dd");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_820H_VoHandler.VO_WhiteKing_LETL_Idle
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_820H_VoHandler.VO_WhiteKing_LETL_Intro,
      (string) LettuceBoss_LT24_820H_VoHandler.VO_WhiteKing_LETL_Attack,
      (string) LettuceBoss_LT24_820H_VoHandler.VO_WhiteKing_LETL_Idle,
      (string) LettuceBoss_LT24_820H_VoHandler.VO_WhiteKing_LETL_Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_820H_VoHandler.VO_WhiteKing_LETL_Intro;
    this.m_deathLine = (string) LettuceBoss_LT24_820H_VoHandler.VO_WhiteKing_LETL_Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_820H_VoHandler lt24820HVoHandler = this;
    while (lt24820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24820HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_820H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_820H" && cardID == "LT24_820P3")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt24820HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_820H_VoHandler.VO_WhiteKing_LETL_Attack);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_820H_VoHandler lt24820HVoHandler = this;
    while (lt24820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24820HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_820H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24820HVoHandler.MissionPlayVO(playByDesignCode, lt24820HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24820HVoHandler.MissionPlayVO(playByDesignCode, lt24820HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24820HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_820H_VoHandler lt24820HVoHandler = this;
    while (lt24820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24820HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_820H");
    if (entity.GetCardId() == "LT24_820H")
      yield return (object) lt24820HVoHandler.MissionPlaySound(playByDesignCode, lt24820HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_820H_VoHandler lt24820HVoHandler = this;
    while (lt24820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24820HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_820H");
    if (turn == 1)
      yield return (object) lt24820HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24820HVoHandler.m_introLine);
  }
}
