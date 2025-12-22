using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_857H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_AV_284_Female_Human_Attack_01 = new AssetReference("VO_AV_284_Female_Human_Attack_01.prefab:3f9f1fdb27458be4894f634b5df7f41b");
  private static readonly AssetReference VO_BalindaStonehearth_Female_Human_Bark_02 = new AssetReference("VO_BalindaStonehearth_Female_Human_Bark_02.prefab:ba3f8d60daa9d734185f2e94a210a49f");
  private static readonly AssetReference VO_AV_284_Female_Human_Death_01 = new AssetReference("VO_BalindaStonehearth_Female_Human_Death_01.prefab:98889acd8a4de0142bc8e5d3b46cf6f9");
  private static readonly AssetReference VO_BalindaStonehearth_Female_Human_Bark_08 = new AssetReference("VO_BalindaStonehearth_Female_Human_Bark_08.prefab:6fd08c045520fc0458367aaa6dd1281d");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_857H_VoHandler.VO_BalindaStonehearth_Female_Human_Bark_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_857H_VoHandler.VO_AV_284_Female_Human_Attack_01,
      (string) LettuceBoss_LETL_857H_VoHandler.VO_BalindaStonehearth_Female_Human_Bark_02,
      (string) LettuceBoss_LETL_857H_VoHandler.VO_AV_284_Female_Human_Death_01,
      (string) LettuceBoss_LETL_857H_VoHandler.VO_BalindaStonehearth_Female_Human_Bark_08
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_857H_VoHandler.VO_BalindaStonehearth_Female_Human_Bark_08;
    this.m_deathLine = (string) LettuceBoss_LETL_857H_VoHandler.VO_AV_284_Female_Human_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_857H_VoHandler letl857HVoHandler = this;
    while (letl857HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl857HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_857H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_857H" && cardID == "LETL_857P1_01")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl857HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_857H_VoHandler.VO_AV_284_Female_Human_Attack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_857H_VoHandler letl857HVoHandler = this;
    while (letl857HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl857HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_857H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl857HVoHandler.MissionPlayVO(playByDesignCode, letl857HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl857HVoHandler.MissionPlayVO(playByDesignCode, letl857HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl857HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_857H_VoHandler letl857HVoHandler = this;
    while (letl857HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl857HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_857H");
    if (entity.GetCardId() == "LETL_857H")
      yield return (object) letl857HVoHandler.MissionPlaySound(playByDesignCode, letl857HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_857H_VoHandler letl857HVoHandler = this;
    while (letl857HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl857HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_857H");
    if (turn == 1)
      yield return (object) letl857HVoHandler.MissionPlayVOOnce(playByDesignCode, letl857HVoHandler.m_introLine);
  }
}
