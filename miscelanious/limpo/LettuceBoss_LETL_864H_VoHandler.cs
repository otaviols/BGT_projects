using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_864H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Onyxia_Female_Dragon_Fluff_06 = new AssetReference("VO_Onyxia_Female_Dragon_Fluff_06.prefab:48a8ea05c4ba7bb4b846f4c766f96b6f");
  private static readonly AssetReference VO_Onyxia_Female_Dragon_Idle_01 = new AssetReference("VO_Onyxia_Female_Dragon_Idle_01.prefab:e8995350199836a45b27dfe78cd42496");
  private static readonly AssetReference VO_Onyxia_Female_Dragon_Attack_02 = new AssetReference("VO_Onyxia_Female_Dragon_Attack_02.prefab:3a4f16f62d51c1a4d90cdd1d79c6eb3d");
  private static readonly AssetReference VO_Onyxia_Female_Dragon_PhaseTransition_03 = new AssetReference("VO_Onyxia_Female_Dragon_PhaseTransition_03.prefab:11aad5ceae7899a4b8cff52e8d8ae8cc");
  private static readonly AssetReference Death = new AssetReference("Death.prefab:b6bd9ea0e27442b4b8ee69edb1b18f41");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_864H_VoHandler.VO_Onyxia_Female_Dragon_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_864H_VoHandler.VO_Onyxia_Female_Dragon_Fluff_06,
      (string) LettuceBoss_LETL_864H_VoHandler.VO_Onyxia_Female_Dragon_Idle_01,
      (string) LettuceBoss_LETL_864H_VoHandler.VO_Onyxia_Female_Dragon_Attack_02,
      (string) LettuceBoss_LETL_864H_VoHandler.VO_Onyxia_Female_Dragon_PhaseTransition_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_864H_VoHandler.VO_Onyxia_Female_Dragon_Fluff_06;
    this.m_deathLine = (string) LettuceBoss_LETL_864H_VoHandler.Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_864H_VoHandler letl864HVoHandler = this;
    while (letl864HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl864HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_864H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_864H")
    {
      string str = cardID;
      if (!(str == "LT22_024P2_03"))
      {
        if (!(str == "LT22_024P2_05"))
        {
          if (str == "LETL_864P1")
          {
            GameState.Get().SetBusy(true);
            yield return (object) letl864HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_864H_VoHandler.VO_Onyxia_Female_Dragon_PhaseTransition_03);
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl864HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_864H_VoHandler.VO_Onyxia_Female_Dragon_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl864HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_864H_VoHandler.VO_Onyxia_Female_Dragon_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_864H_VoHandler letl864HVoHandler = this;
    while (letl864HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl864HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_864H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl864HVoHandler.MissionPlayVO(playByDesignCode, letl864HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl864HVoHandler.MissionPlayVO(playByDesignCode, letl864HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl864HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_864H_VoHandler letl864HVoHandler = this;
    while (letl864HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl864HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_864H");
    if (entity.GetCardId() == "LETL_864H")
      yield return (object) letl864HVoHandler.MissionPlaySound(playByDesignCode, letl864HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_864H_VoHandler letl864HVoHandler = this;
    while (letl864HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl864HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_864H");
    if (turn == 1)
      yield return (object) letl864HVoHandler.MissionPlayVOOnce(playByDesignCode, letl864HVoHandler.m_introLine);
  }
}
