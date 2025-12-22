using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_856H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_CommanderIchman_Male_Human_Intro_01 = new AssetReference("VO_CommanderIchman_Male_Human_Intro_01.prefab:50290f586dc98244c9c66c676114da38");
  private static readonly AssetReference VO_CommanderIchman_Male_Human_Idle_01 = new AssetReference("VO_CommanderIchman_Male_Human_Idle_01.prefab:362ec012070015f44a2fd2e008cb287a");
  private static readonly AssetReference VO_CommanderIchman_Male_Human_Attack_01 = new AssetReference("VO_CommanderIchman_Male_Human_Attack_01.prefab:3a75a2dc2cb40544395c79ff72d4b81b");
  private static readonly AssetReference VO_CommanderIchman_Male_Human_Death_01 = new AssetReference("VO_CommanderIchman_Male_Human_Death_01.prefab:4443357feb9eff24cbd96481439f4809");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_856H_VoHandler.VO_CommanderIchman_Male_Human_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_856H_VoHandler.VO_CommanderIchman_Male_Human_Intro_01,
      (string) LettuceBoss_LETL_856H_VoHandler.VO_CommanderIchman_Male_Human_Idle_01,
      (string) LettuceBoss_LETL_856H_VoHandler.VO_CommanderIchman_Male_Human_Attack_01,
      (string) LettuceBoss_LETL_856H_VoHandler.VO_CommanderIchman_Male_Human_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_856H_VoHandler.VO_CommanderIchman_Male_Human_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_856H_VoHandler.VO_CommanderIchman_Male_Human_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_856H_VoHandler letl856HVoHandler = this;
    while (letl856HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl856HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_856H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_856H" && cardID == "LETL_856P1_01")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl856HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_856H_VoHandler.VO_CommanderIchman_Male_Human_Attack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_856H_VoHandler letl856HVoHandler = this;
    while (letl856HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl856HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_856H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl856HVoHandler.MissionPlayVO(playByDesignCode, letl856HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl856HVoHandler.MissionPlayVO(playByDesignCode, letl856HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl856HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_856H_VoHandler letl856HVoHandler = this;
    while (letl856HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl856HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_856H");
    if (entity.GetCardId() == "LETL_856H")
      yield return (object) letl856HVoHandler.MissionPlaySound(playByDesignCode, letl856HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_856H_VoHandler letl856HVoHandler = this;
    while (letl856HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl856HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_856H");
    if (turn == 1)
      yield return (object) letl856HVoHandler.MissionPlayVOOnce(playByDesignCode, letl856HVoHandler.m_introLine);
  }
}
