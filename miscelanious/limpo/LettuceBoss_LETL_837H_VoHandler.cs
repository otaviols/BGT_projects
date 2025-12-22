using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_837H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_837H_Male_Dwarf_Attack_01 = new AssetReference("VO_LETL_837H_Male_Dwarf_Attack_01.prefab:ec51c087926ec69458871c4fec4d9ef5");
  private static readonly AssetReference VO_LETL_837H_Male_Dwarf_Attack_02 = new AssetReference("VO_LETL_837H_Male_Dwarf_Attack_02.prefab:0dbcdf52419317e46a33ef2dc3b8e1b4");
  private static readonly AssetReference VO_LETL_837H_Male_Dwarf_Death_01 = new AssetReference("VO_LETL_837H_Male_Dwarf_Death_01.prefab:0ee0b213d9e941545add54425c48bc4d");
  private static readonly AssetReference VO_LETL_837H_Male_Dwarf_Idle_01 = new AssetReference("VO_LETL_837H_Male_Dwarf_Idle_01.prefab:bb9c8e9cafba4454da306f78abe3184f");
  private static readonly AssetReference VO_LETL_837H_Male_Dwarf_Intro_01 = new AssetReference("VO_LETL_837H_Male_Dwarf_Intro_01.prefab:005552efbb15d9844aac3686d5f03c23");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_837H_VoHandler.VO_LETL_837H_Male_Dwarf_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_837H_VoHandler.VO_LETL_837H_Male_Dwarf_Attack_01,
      (string) LettuceBoss_LETL_837H_VoHandler.VO_LETL_837H_Male_Dwarf_Attack_02,
      (string) LettuceBoss_LETL_837H_VoHandler.VO_LETL_837H_Male_Dwarf_Death_01,
      (string) LettuceBoss_LETL_837H_VoHandler.VO_LETL_837H_Male_Dwarf_Idle_01,
      (string) LettuceBoss_LETL_837H_VoHandler.VO_LETL_837H_Male_Dwarf_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_837H_VoHandler.VO_LETL_837H_Male_Dwarf_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_837H_VoHandler.VO_LETL_837H_Male_Dwarf_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_837H_VoHandler letl837HVoHandler = this;
    while (letl837HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl837HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_837H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_837H")
    {
      string str = cardID;
      if (!(str == "LETL_837_1") && !(str == "LETL_837_3"))
      {
        if (str == "LETL_837_2")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl837HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_837H_VoHandler.VO_LETL_837H_Male_Dwarf_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl837HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_837H_VoHandler.VO_LETL_837H_Male_Dwarf_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_837H_VoHandler letl837HVoHandler = this;
    while (letl837HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl837HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_837H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl837HVoHandler.MissionPlayVO(playByDesignCode, letl837HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl837HVoHandler.MissionPlayVO(playByDesignCode, letl837HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl837HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_837H_VoHandler letl837HVoHandler = this;
    while (letl837HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl837HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_837H");
    if (entity.GetCardId() == "LETL_837H")
      yield return (object) letl837HVoHandler.MissionPlaySound(playByDesignCode, letl837HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_837H_VoHandler letl837HVoHandler = this;
    while (letl837HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl837HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_837H");
    if (turn == 1)
      yield return (object) letl837HVoHandler.MissionPlayVOOnce(playByDesignCode, letl837HVoHandler.m_introLine);
  }
}
