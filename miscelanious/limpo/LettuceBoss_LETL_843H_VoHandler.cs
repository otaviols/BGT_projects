using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_843H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_843H_Male_Dragonkin_Attack_01 = new AssetReference("VO_LETL_843H_Male_Dragonkin_Attack_01.prefab:a2b0065e82e378745a05bfe528a54422");
  private static readonly AssetReference VO_LETL_843H_Male_Dragonkin_Attack_02 = new AssetReference("VO_LETL_843H_Male_Dragonkin_Attack_02.prefab:47756de9ab0498d408a007a4986ec465");
  private static readonly AssetReference VO_LETL_843H_Male_Dragonkin_Death_01 = new AssetReference("VO_LETL_843H_Male_Dragonkin_Death_01.prefab:450b37c081fa37843a7cf1aeec27cf2f");
  private static readonly AssetReference VO_LETL_843H_Male_Dragonkin_Idle_01 = new AssetReference("VO_LETL_843H_Male_Dragonkin_Idle_01.prefab:9cb173a8359417e4b99d2273551a4c62");
  private static readonly AssetReference VO_LETL_843H_Male_Dragonkin_Intro_01 = new AssetReference("VO_LETL_843H_Male_Dragonkin_Intro_01.prefab:aab8cacd0c5d0cc429914009291ee574");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_843H_VoHandler.VO_LETL_843H_Male_Dragonkin_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_843H_VoHandler.VO_LETL_843H_Male_Dragonkin_Attack_01,
      (string) LettuceBoss_LETL_843H_VoHandler.VO_LETL_843H_Male_Dragonkin_Attack_02,
      (string) LettuceBoss_LETL_843H_VoHandler.VO_LETL_843H_Male_Dragonkin_Death_01,
      (string) LettuceBoss_LETL_843H_VoHandler.VO_LETL_843H_Male_Dragonkin_Idle_01,
      (string) LettuceBoss_LETL_843H_VoHandler.VO_LETL_843H_Male_Dragonkin_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_843H_VoHandler.VO_LETL_843H_Male_Dragonkin_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_843H_VoHandler.VO_LETL_843H_Male_Dragonkin_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_843H_VoHandler letl843HVoHandler = this;
    while (letl843HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl843HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_843H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_843H")
    {
      string str = cardID;
      if (!(str == "LETL_843P2_01") && !(str == "LETL_843P2_02"))
      {
        if (str == "LETL_843P1_01" || str == "LETL_842P1_02")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl843HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_843H_VoHandler.VO_LETL_843H_Male_Dragonkin_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl843HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_843H_VoHandler.VO_LETL_843H_Male_Dragonkin_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_843H_VoHandler letl843HVoHandler = this;
    while (letl843HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl843HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_843H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl843HVoHandler.MissionPlayVO(playByDesignCode, letl843HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl843HVoHandler.MissionPlayVO(playByDesignCode, letl843HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl843HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_843H_VoHandler letl843HVoHandler = this;
    while (letl843HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl843HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_843H");
    if (entity.GetCardId() == "LETL_843H")
      yield return (object) letl843HVoHandler.MissionPlaySound(playByDesignCode, letl843HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_843H_VoHandler letl843HVoHandler = this;
    while (letl843HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl843HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_843H");
    if (turn == 1)
      yield return (object) letl843HVoHandler.MissionPlayVOOnce(playByDesignCode, letl843HVoHandler.m_introLine);
  }
}
