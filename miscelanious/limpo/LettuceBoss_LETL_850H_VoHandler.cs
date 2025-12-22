using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_850H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LouisPhilips_Male_Undead_Attack_01 = new AssetReference("VO_LouisPhilips_Male_Undead_Attack_01.prefab:5da447897a591114c87b203093e3bd9a");
  private static readonly AssetReference VO_LouisPhilips_Male_Undead_Attack_02 = new AssetReference("VO_LouisPhilips_Male_Undead_Attack_02.prefab:89345aa697638e543baa652d53b65f44");
  private static readonly AssetReference VO_LouisPhilips_Male_Undead_Death_01 = new AssetReference("VO_LouisPhilips_Male_Undead_Death_01.prefab:481e90b300c4a9f4bb3c4ea4096f4750");
  private static readonly AssetReference VO_LouisPhilips_Male_Undead_Idle_01 = new AssetReference("VO_LouisPhilips_Male_Undead_Idle_01.prefab:a306338235bf99d4db3b6c9b4a520de7");
  private static readonly AssetReference VO_LouisPhilips_Male_Undead_Intro_01 = new AssetReference("VO_LouisPhilips_Male_Undead_Intro_01.prefab:5cf772bd7263c914e994904af5d6bbf6");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_850H_VoHandler.VO_LouisPhilips_Male_Undead_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_850H_VoHandler.VO_LouisPhilips_Male_Undead_Attack_01,
      (string) LettuceBoss_LETL_850H_VoHandler.VO_LouisPhilips_Male_Undead_Attack_02,
      (string) LettuceBoss_LETL_850H_VoHandler.VO_LouisPhilips_Male_Undead_Death_01,
      (string) LettuceBoss_LETL_850H_VoHandler.VO_LouisPhilips_Male_Undead_Idle_01,
      (string) LettuceBoss_LETL_850H_VoHandler.VO_LouisPhilips_Male_Undead_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_850H_VoHandler.VO_LouisPhilips_Male_Undead_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_850H_VoHandler.VO_LouisPhilips_Male_Undead_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_850H_VoHandler letl850HVoHandler = this;
    while (letl850HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl850HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_850H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_850H")
    {
      string str = cardID;
      if (!(str == "LETL_847P2_01"))
      {
        if (str == "LETL_850P1_01" || str == "LETL_850P1_02")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl850HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_850H_VoHandler.VO_LouisPhilips_Male_Undead_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl850HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_850H_VoHandler.VO_LouisPhilips_Male_Undead_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_850H_VoHandler letl850HVoHandler = this;
    while (letl850HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl850HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_850H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl850HVoHandler.MissionPlayVO(playByDesignCode, letl850HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl850HVoHandler.MissionPlayVO(playByDesignCode, letl850HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl850HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_850H_VoHandler letl850HVoHandler = this;
    while (letl850HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl850HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_850H");
    if (entity.GetCardId() == "LETL_850H")
      yield return (object) letl850HVoHandler.MissionPlaySound(playByDesignCode, letl850HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_850H_VoHandler letl850HVoHandler = this;
    while (letl850HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl850HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_850H");
    if (turn == 1)
      yield return (object) letl850HVoHandler.MissionPlayVOOnce(playByDesignCode, letl850HVoHandler.m_introLine);
  }
}
