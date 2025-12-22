using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_810H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_OG_321_Male_Dwarf_Play_01 = new AssetReference("VO_OG_321_Male_Dwarf_Play_01.prefab:89430659a930d5740b8ac7fc26fdcd79");
  private static readonly AssetReference VO_OG_321_Male_Dwarf_Attack_01 = new AssetReference("VO_OG_321_Male_Dwarf_Attack_01.prefab:45109a26787b1ff418f175e1b9b2fb09");
  private static readonly AssetReference VO_OG_321_Male_Dwarf_Death_01 = new AssetReference("VO_OG_321_Male_Dwarf_Death_01.prefab:19f9d2e895b74034e94ba4ab534bd318");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_810H_VoHandler.VO_OG_321_Male_Dwarf_Play_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_810H_VoHandler.VO_OG_321_Male_Dwarf_Play_01,
      (string) LettuceBoss_LT24_810H_VoHandler.VO_OG_321_Male_Dwarf_Attack_01,
      (string) LettuceBoss_LT24_810H_VoHandler.VO_OG_321_Male_Dwarf_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_810H_VoHandler.VO_OG_321_Male_Dwarf_Play_01;
    this.m_deathLine = (string) LettuceBoss_LT24_810H_VoHandler.VO_OG_321_Male_Dwarf_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_810H_VoHandler lt24810HVoHandler = this;
    while (lt24810HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24810HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_810H2");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_810H2")
    {
      string str = cardID;
      if (!(str == "LETL_033P5_03"))
      {
        if (!(str == "LETL_033P5_05"))
        {
          if (str == "LT23T_126_02")
          {
            GameState.Get().SetBusy(true);
            yield return (object) lt24810HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_810H_VoHandler.VO_OG_321_Male_Dwarf_Attack_01);
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24810HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_810H_VoHandler.VO_OG_321_Male_Dwarf_Attack_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24810HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_810H_VoHandler.VO_OG_321_Male_Dwarf_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_810H_VoHandler lt24810HVoHandler = this;
    while (lt24810HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24810HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_810H2");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24810HVoHandler.MissionPlayVO(playByDesignCode, lt24810HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24810HVoHandler.MissionPlayVO(playByDesignCode, lt24810HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24810HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_810H_VoHandler lt24810HVoHandler = this;
    while (lt24810HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24810HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_810H2");
    if (entity.GetCardId() == "LT24_810H2")
      yield return (object) lt24810HVoHandler.MissionPlaySound(playByDesignCode, lt24810HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_810H_VoHandler lt24810HVoHandler = this;
    while (lt24810HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24810HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_810H2");
    if (turn == 1)
      yield return (object) lt24810HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24810HVoHandler.m_introLine);
  }
}
