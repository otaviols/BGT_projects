using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_809H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_FishOfNZoth_Male_OldGod_LETL_Attack_01 = new AssetReference("VO_FishOfNZoth_Male_OldGod_LETL_Attack_01.prefab:6f4190c99b082f442b52d0b34ed29251");
  private static readonly AssetReference VO_FishOfNZoth_Male_OldGod_LETL_Attack_02 = new AssetReference("VO_FishOfNZoth_Male_OldGod_LETL_Attack_02.prefab:3abc3951a6d245c4893170a522a285ec");
  private static readonly AssetReference VO_FishOfNZoth_Male_OldGod_LETL_Death_01 = new AssetReference("VO_FishOfNZoth_Male_OldGod_LETL_Death_01.prefab:9036f83f19a239846b91e3792b875b87");
  private static readonly AssetReference VO_FishOfNZoth_Male_OldGod_LETL_Idle_01 = new AssetReference("VO_FishOfNZoth_Male_OldGod_LETL_Idle_01.prefab:05d4dbdab05501a47b441666edaa02b7");
  private static readonly AssetReference VO_FishOfNZoth_Male_OldGod_LETL_Intro_01 = new AssetReference("VO_FishOfNZoth_Male_OldGod_LETL_Intro_01.prefab:abf5302d5ab27294186e9197b35da9ee");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_809H_VoHandler.VO_FishOfNZoth_Male_OldGod_LETL_Idle_01,
    (string) LettuceBoss_LT23_809H_VoHandler.VO_FishOfNZoth_Male_OldGod_LETL_Attack_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_809H_VoHandler.VO_FishOfNZoth_Male_OldGod_LETL_Death_01,
      (string) LettuceBoss_LT23_809H_VoHandler.VO_FishOfNZoth_Male_OldGod_LETL_Idle_01,
      (string) LettuceBoss_LT23_809H_VoHandler.VO_FishOfNZoth_Male_OldGod_LETL_Intro_01,
      (string) LettuceBoss_LT23_809H_VoHandler.VO_FishOfNZoth_Male_OldGod_LETL_Attack_01,
      (string) LettuceBoss_LT23_809H_VoHandler.VO_FishOfNZoth_Male_OldGod_LETL_Attack_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_809H_VoHandler.VO_FishOfNZoth_Male_OldGod_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_809H_VoHandler.VO_FishOfNZoth_Male_OldGod_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_809H_VoHandler lt23809HVoHandler = this;
    while (lt23809HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23809HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_809H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_809H" && cardID == "LT23_809P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23809HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_809H_VoHandler.VO_FishOfNZoth_Male_OldGod_LETL_Attack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_809H_VoHandler lt23809HVoHandler = this;
    while (lt23809HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23809HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_809H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23809HVoHandler.MissionPlayVO(playByDesignCode, lt23809HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23809HVoHandler.MissionPlayVO(playByDesignCode, lt23809HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23809HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_809H_VoHandler lt23809HVoHandler = this;
    while (lt23809HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23809HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_809H");
    if (entity.GetCardId() == "LT23_809H")
      yield return (object) lt23809HVoHandler.MissionPlaySound(playByDesignCode, lt23809HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_809H_VoHandler lt23809HVoHandler = this;
    while (lt23809HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23809HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_809H");
    if (turn == 1)
      yield return (object) lt23809HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23809HVoHandler.m_introLine);
  }
}
