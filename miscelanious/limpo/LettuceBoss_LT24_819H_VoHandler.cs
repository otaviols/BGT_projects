using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_819H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Netherspite_Male_Dragon_LETL_C03_T09_Dialogue_01 = new AssetReference("VO_Netherspite_Male_Dragon_LETL_C03_T09_Dialogue_01.prefab:9a08d22f3b02d954a8c2be097c18c992");
  private static readonly AssetReference VO_Netherspite_Male_Dragon_LETL_Attack = new AssetReference("VO_Netherspite_Male_Dragon_LETL_Attack.prefab:0b82f9d579e8149469c6bf9b14b0150d");
  private static readonly AssetReference VO_Netherspite_Male_Dragon_LETL_Death = new AssetReference("VO_Netherspite_Male_Dragon_LETL_Death.prefab:1112a8ce589bc2143a49611d3fca61c7");
  private static readonly AssetReference VO_Netherspite_Male_Dragon_LETL_Idle = new AssetReference("VO_Netherspite_Male_Dragon_LETL_Idle.prefab:c8fe009cca27be64ca964e051cbd6bc0");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_819H_VoHandler.VO_Netherspite_Male_Dragon_LETL_Idle
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_819H_VoHandler.VO_Netherspite_Male_Dragon_LETL_C03_T09_Dialogue_01,
      (string) LettuceBoss_LT24_819H_VoHandler.VO_Netherspite_Male_Dragon_LETL_Attack,
      (string) LettuceBoss_LT24_819H_VoHandler.VO_Netherspite_Male_Dragon_LETL_Idle,
      (string) LettuceBoss_LT24_819H_VoHandler.VO_Netherspite_Male_Dragon_LETL_Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_819H_VoHandler.VO_Netherspite_Male_Dragon_LETL_C03_T09_Dialogue_01;
    this.m_deathLine = (string) LettuceBoss_LT24_819H_VoHandler.VO_Netherspite_Male_Dragon_LETL_Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_819H_VoHandler lt24819HVoHandler = this;
    while (lt24819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24819HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_819H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_819H" && cardID == "LT24_819P2")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt24819HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_819H_VoHandler.VO_Netherspite_Male_Dragon_LETL_Attack);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_819H_VoHandler lt24819HVoHandler = this;
    while (lt24819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24819HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_819H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24819HVoHandler.MissionPlayVO(playByDesignCode, lt24819HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24819HVoHandler.MissionPlayVO(playByDesignCode, lt24819HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24819HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_819H_VoHandler lt24819HVoHandler = this;
    while (lt24819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24819HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_819H");
    if (entity.GetCardId() == "LT24_819H")
      yield return (object) lt24819HVoHandler.MissionPlaySound(playByDesignCode, lt24819HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_819H_VoHandler lt24819HVoHandler = this;
    while (lt24819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24819HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_819H");
    if (turn == 1)
      yield return (object) lt24819HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24819HVoHandler.m_introLine);
  }
}
