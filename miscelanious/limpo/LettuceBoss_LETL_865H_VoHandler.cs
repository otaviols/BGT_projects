using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_865H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Onyxia_Female_Dragon_Intro_01 = new AssetReference("VO_Onyxia_Female_Dragon_Intro_01.prefab:8214c49a90c32974a8d5cedd34f0b599");
  private static readonly AssetReference VO_Onyxia_Female_Dragon_Fluff_04 = new AssetReference("VO_Onyxia_Female_Dragon_Fluff_04.prefab:ab97e790e3b290b498d19500d3e6cdaa");
  private static readonly AssetReference VO_Onyxia_Female_Dragon_Attack_01 = new AssetReference("VO_Onyxia_Female_Dragon_Attack_01.prefab:4737e827b73ebb84da50b4748b066310");
  private static readonly AssetReference VO_Onyxia_Female_Dragon_PhaseTransition_02 = new AssetReference("VO_Onyxia_Female_Dragon_PhaseTransition_02.prefab:d0cec20304372a3468f28f3051a789a4");
  private static readonly AssetReference Death = new AssetReference("Death.prefab:b6bd9ea0e27442b4b8ee69edb1b18f41");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_865H_VoHandler.VO_Onyxia_Female_Dragon_PhaseTransition_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_865H_VoHandler.VO_Onyxia_Female_Dragon_Intro_01,
      (string) LettuceBoss_LETL_865H_VoHandler.VO_Onyxia_Female_Dragon_Fluff_04,
      (string) LettuceBoss_LETL_865H_VoHandler.VO_Onyxia_Female_Dragon_Attack_01,
      (string) LettuceBoss_LETL_865H_VoHandler.VO_Onyxia_Female_Dragon_PhaseTransition_02,
      (string) LettuceBoss_LETL_865H_VoHandler.Death
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_865H_VoHandler.VO_Onyxia_Female_Dragon_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_865H_VoHandler.Death;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_865H_VoHandler letl865HVoHandler = this;
    while (letl865HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl865HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_865H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_865H")
    {
      string str = cardID;
      if (!(str == "LETL_865P1"))
      {
        if (!(str == "LT22_024P1_03"))
        {
          if (str == "LT22_024P1_05")
          {
            GameState.Get().SetBusy(true);
            yield return (object) letl865HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_865H_VoHandler.VO_Onyxia_Female_Dragon_Attack_01);
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl865HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_865H_VoHandler.VO_Onyxia_Female_Dragon_Attack_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl865HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_865H_VoHandler.VO_Onyxia_Female_Dragon_Fluff_04);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_865H_VoHandler letl865HVoHandler = this;
    while (letl865HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl865HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_865H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl865HVoHandler.MissionPlayVO(playByDesignCode, letl865HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl865HVoHandler.MissionPlayVO(playByDesignCode, letl865HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl865HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_865H_VoHandler letl865HVoHandler = this;
    while (letl865HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl865HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_865H");
    if (entity.GetCardId() == "LETL_865H")
      yield return (object) letl865HVoHandler.MissionPlaySound(playByDesignCode, letl865HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_865H_VoHandler letl865HVoHandler = this;
    while (letl865HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl865HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_865H");
    if (turn == 1)
      yield return (object) letl865HVoHandler.MissionPlayVOOnce(playByDesignCode, letl865HVoHandler.m_introLine);
  }
}
