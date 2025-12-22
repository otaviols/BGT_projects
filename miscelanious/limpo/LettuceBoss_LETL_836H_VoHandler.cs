using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_836H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_836H_Male_Dwarf_Attack_01 = new AssetReference("VO_LETL_836H_Male_Dwarf_Attack_01.prefab:95133ab617b9c7341a90d2be35773593");
  private static readonly AssetReference VO_LETL_836H_Male_Dwarf_Attack_02 = new AssetReference("VO_LETL_836H_Male_Dwarf_Attack_02.prefab:ebed45970a8526e46a68dc30fbd79a5b");
  private static readonly AssetReference VO_LETL_836H_Male_Dwarf_Death_01 = new AssetReference("VO_LETL_836H_Male_Dwarf_Death_01.prefab:5fc5642a24c9ba3449115e79ecbd4a08");
  private static readonly AssetReference VO_LETL_836H_Male_Dwarf_Idle_01 = new AssetReference("VO_LETL_836H_Male_Dwarf_Idle_01.prefab:0d1a1a041dfa1ec4fac4faa165d25341");
  private static readonly AssetReference VO_LETL_836H_Male_Dwarf_Intro_01 = new AssetReference("VO_LETL_836H_Male_Dwarf_Intro_01.prefab:bf4c653a66c57274cae70371ef618c61");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_836H_VoHandler.VO_LETL_836H_Male_Dwarf_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_836H_VoHandler.VO_LETL_836H_Male_Dwarf_Attack_01,
      (string) LettuceBoss_LETL_836H_VoHandler.VO_LETL_836H_Male_Dwarf_Attack_02,
      (string) LettuceBoss_LETL_836H_VoHandler.VO_LETL_836H_Male_Dwarf_Death_01,
      (string) LettuceBoss_LETL_836H_VoHandler.VO_LETL_836H_Male_Dwarf_Idle_01,
      (string) LettuceBoss_LETL_836H_VoHandler.VO_LETL_836H_Male_Dwarf_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_836H_VoHandler.VO_LETL_836H_Male_Dwarf_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_836H_VoHandler.VO_LETL_836H_Male_Dwarf_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_836H_VoHandler letl836HVoHandler = this;
    while (letl836HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl836HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_836H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_836H")
    {
      string str = cardID;
      if (!(str == "LETL_836P1_01"))
      {
        if (str == "LETL_836P2_01")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl836HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_836H_VoHandler.VO_LETL_836H_Male_Dwarf_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl836HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_836H_VoHandler.VO_LETL_836H_Male_Dwarf_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_836H_VoHandler letl836HVoHandler = this;
    while (letl836HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl836HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_836H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl836HVoHandler.MissionPlayVO(playByDesignCode, letl836HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl836HVoHandler.MissionPlayVO(playByDesignCode, letl836HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl836HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_836H_VoHandler letl836HVoHandler = this;
    while (letl836HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl836HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_836H");
    if (entity.GetCardId() == "LETL_836H")
      yield return (object) letl836HVoHandler.MissionPlaySound(playByDesignCode, letl836HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_836H_VoHandler letl836HVoHandler = this;
    while (letl836HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl836HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_836H");
    if (turn == 1)
      yield return (object) letl836HVoHandler.MissionPlayVOOnce(playByDesignCode, letl836HVoHandler.m_introLine);
  }
}
