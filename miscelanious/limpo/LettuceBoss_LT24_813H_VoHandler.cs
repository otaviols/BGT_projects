using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_813H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_KAR_044_Attack = new AssetReference("VO_KAR_044_Attack.prefab:2a09cd263390eda4186d3c094b1e8837");
  private static readonly AssetReference VO_KAR_044_Death = new AssetReference("VO_KAR_044_Death.prefab:aac1fb840a70d4f499bb1693f1ce3419");
  private static readonly AssetReference VO_KAR_044_Idle = new AssetReference("VO_KAR_044_Idle.prefab:0a4e68d54e47a5e4baaccf390faf7e8a");
  private static readonly AssetReference VO_KAR_044_Intro = new AssetReference("VO_KAR_044_Intro.prefab:3b90bfd5a76bdf341bce3de3d0604a7f");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Idle
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Intro,
      (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Idle,
      (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Attack,
      (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Intro;
    this.m_deathLine = (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_813H_VoHandler lt24813HVoHandler = this;
    while (lt24813HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24813HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_813H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_813H")
    {
      string str = cardID;
      if (!(str == "LT22_001P2_03"))
      {
        if (!(str == "LT22_001P2_05"))
        {
          if (!(str == "LETL_441_03"))
          {
            if (str == "LETL_441_05")
            {
              GameState.Get().SetBusy(true);
              yield return (object) lt24813HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Attack);
              GameState.Get().SetBusy(false);
            }
          }
          else
          {
            GameState.Get().SetBusy(true);
            yield return (object) lt24813HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Attack);
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24813HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Attack);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24813HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_813H_VoHandler.VO_KAR_044_Attack);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_813H_VoHandler lt24813HVoHandler = this;
    while (lt24813HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24813HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_813H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24813HVoHandler.MissionPlayVO(playByDesignCode, lt24813HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24813HVoHandler.MissionPlayVO(playByDesignCode, lt24813HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24813HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_813H_VoHandler lt24813HVoHandler = this;
    while (lt24813HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24813HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_813H");
    if (entity.GetCardId() == "LT24_813H")
      yield return (object) lt24813HVoHandler.MissionPlaySound(playByDesignCode, lt24813HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_813H_VoHandler lt24813HVoHandler = this;
    while (lt24813HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24813HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_813H");
    if (turn == 1)
      yield return (object) lt24813HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24813HVoHandler.m_introLine);
  }
}
