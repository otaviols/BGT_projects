using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_855H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LieutenantRotimer_Male_Dwarf_Attack_01 = new AssetReference("VO_LieutenantRotimer_Male_Dwarf_Attack_01.prefab:db2ca013e746e4a4996e5f240b7b7cc0");
  private static readonly AssetReference VO_LieutenantRotimer_Male_Dwarf_Death_01 = new AssetReference("VO_LieutenantRotimer_Male_Dwarf_Death_01.prefab:72029f03a0b937c43b6c19e04fbeb8ad");
  private static readonly AssetReference VO_LieutenantRotimer_Male_Dwarf_Idle_01 = new AssetReference("VO_LieutenantRotimer_Male_Dwarf_Idle_01.prefab:99afda9f89cfb934b857c6243a028f4a");
  private static readonly AssetReference VO_LieutenantRotimer_Male_Dwarf_Intro_01 = new AssetReference("VO_LieutenantRotimer_Male_Dwarf_Intro_01.prefab:5fa23c6ba4082d140a9337a22dfd9294");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_855H_VoHandler.VO_LieutenantRotimer_Male_Dwarf_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_855H_VoHandler.VO_LieutenantRotimer_Male_Dwarf_Attack_01,
      (string) LettuceBoss_LETL_855H_VoHandler.VO_LieutenantRotimer_Male_Dwarf_Death_01,
      (string) LettuceBoss_LETL_855H_VoHandler.VO_LieutenantRotimer_Male_Dwarf_Idle_01,
      (string) LettuceBoss_LETL_855H_VoHandler.VO_LieutenantRotimer_Male_Dwarf_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_855H_VoHandler.VO_LieutenantRotimer_Male_Dwarf_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_855H_VoHandler.VO_LieutenantRotimer_Male_Dwarf_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_855H_VoHandler letl855HVoHandler = this;
    while (letl855HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl855HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_855H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_855H")
    {
      string str = cardID;
      if (str == "LETL_855P1_04" || str == "LETL_855P1_05")
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl855HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_855H_VoHandler.VO_LieutenantRotimer_Male_Dwarf_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_855H_VoHandler letl855HVoHandler = this;
    while (letl855HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl855HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_855H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl855HVoHandler.MissionPlayVO(playByDesignCode, letl855HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl855HVoHandler.MissionPlayVO(playByDesignCode, letl855HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl855HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_855H_VoHandler letl855HVoHandler = this;
    while (letl855HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl855HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_855H");
    if (entity.GetCardId() == "LETL_855H")
      yield return (object) letl855HVoHandler.MissionPlaySound(playByDesignCode, letl855HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_855H_VoHandler letl855HVoHandler = this;
    while (letl855HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl855HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_855H");
    if (turn == 1)
      yield return (object) letl855HVoHandler.MissionPlayVOOnce(playByDesignCode, letl855HVoHandler.m_introLine);
  }
}
