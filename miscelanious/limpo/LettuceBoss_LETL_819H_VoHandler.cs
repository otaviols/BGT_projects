using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_819H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_819H_Male_Tauren_Attack_01 = new AssetReference("VO_LETL_819H_Male_Tauren_Attack_01.prefab:43f710e1033ddb74aba009e8814b93db");
  private static readonly AssetReference VO_LETL_819H_Male_Tauren_Attack_02 = new AssetReference("VO_LETL_819H_Male_Tauren_Attack_02.prefab:ec8a6a2ed71cfac4dad0541856a4597a");
  private static readonly AssetReference VO_LETL_819H_Male_Tauren_Death_01 = new AssetReference("VO_LETL_819H_Male_Tauren_Death_01.prefab:efdd409c719fe4b4399b819690a79975");
  private static readonly AssetReference VO_LETL_819H_Male_Tauren_Idle_01 = new AssetReference("VO_LETL_819H_Male_Tauren_Idle_01.prefab:81bf0779f04538b42bd6443e9f46e6ce");
  private static readonly AssetReference VO_LETL_819H_Male_Tauren_Intro_01 = new AssetReference("VO_LETL_819H_Male_Tauren_Intro_01.prefab:d78fb69a7d5c5ee40aadbde46774efcf");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_819H_VoHandler.VO_LETL_819H_Male_Tauren_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_819H_VoHandler.VO_LETL_819H_Male_Tauren_Attack_01,
      (string) LettuceBoss_LETL_819H_VoHandler.VO_LETL_819H_Male_Tauren_Attack_02,
      (string) LettuceBoss_LETL_819H_VoHandler.VO_LETL_819H_Male_Tauren_Death_01,
      (string) LettuceBoss_LETL_819H_VoHandler.VO_LETL_819H_Male_Tauren_Idle_01,
      (string) LettuceBoss_LETL_819H_VoHandler.VO_LETL_819H_Male_Tauren_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_819H_VoHandler.VO_LETL_819H_Male_Tauren_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_819H_VoHandler.VO_LETL_819H_Male_Tauren_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_819H_VoHandler letl819HVoHandler = this;
    while (letl819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl819HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_819H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_819H")
    {
      string str = cardID;
      if (!(str == "LETL_406_02") && !(str == "LETL_020P6_01"))
      {
        if (str == "LETL_441_02")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl819HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_819H_VoHandler.VO_LETL_819H_Male_Tauren_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl819HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_819H_VoHandler.VO_LETL_819H_Male_Tauren_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_819H_VoHandler letl819HVoHandler = this;
    while (letl819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl819HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_819H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl819HVoHandler.MissionPlayVO(playByDesignCode, letl819HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl819HVoHandler.MissionPlayVO(playByDesignCode, letl819HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl819HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_819H_VoHandler letl819HVoHandler = this;
    while (letl819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl819HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_819H");
    if (entity.GetCardId() == "LETL_819H")
      yield return (object) letl819HVoHandler.MissionPlaySound(playByDesignCode, letl819HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_819H_VoHandler letl819HVoHandler = this;
    while (letl819HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl819HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_819H");
    if (turn == 1)
      yield return (object) letl819HVoHandler.MissionPlayVOOnce(playByDesignCode, letl819HVoHandler.m_introLine);
  }
}
