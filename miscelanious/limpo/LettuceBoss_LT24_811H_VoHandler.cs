using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_811H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_AttumenTheHuntsman_Male_Undead_LETL_Attack_01 = new AssetReference("VO_AttumenTheHuntsman_Male_Undead_LETL_Attack_01.prefab:3b1738abff9713f4ea338230f7e6fda3");
  private static readonly AssetReference VO_AttumenTheHuntsman_Male_Undead_LETL_Attack_02 = new AssetReference("VO_AttumenTheHuntsman_Male_Undead_LETL_Attack_02.prefab:024b8e27863310f4eaf7a55684ee25be");
  private static readonly AssetReference VO_AttumenTheHuntsman_Male_Undead_LETL_Death_01 = new AssetReference("VO_AttumenTheHuntsman_Male_Undead_LETL_Death_01.prefab:bc489218ac6b36d47812e6ec69039add");
  private static readonly AssetReference VO_AttumenTheHuntsman_Male_Undead_LETL_Idle_01 = new AssetReference("VO_AttumenTheHuntsman_Male_Undead_LETL_Idle_01.prefab:5d46b76c0d29ba343a942f366b99b838");
  private static readonly AssetReference VO_AttumenTheHuntsman_Male_Undead_LETL_Intro_01 = new AssetReference("VO_AttumenTheHuntsman_Male_Undead_LETL_Intro_01.prefab:85e92c0e8eb73d147b053c95c0f59c5c");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Intro_01,
      (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Idle_01,
      (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Attack_01,
      (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Attack_02,
      (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_811H_VoHandler lt24811HVoHandler = this;
    while (lt24811HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24811HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_811H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_811H")
    {
      string str = cardID;
      if (!(str == "LETL_262_03"))
      {
        if (!(str == "LETL_262_05"))
        {
          if (!(str == "LETL_015P9_03"))
          {
            if (str == "LETL_015P9_05")
            {
              GameState.Get().SetBusy(true);
              yield return (object) lt24811HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Attack_02);
              GameState.Get().SetBusy(false);
            }
          }
          else
          {
            GameState.Get().SetBusy(true);
            yield return (object) lt24811HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Attack_02);
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24811HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Attack_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24811HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_811H_VoHandler.VO_AttumenTheHuntsman_Male_Undead_LETL_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_811H_VoHandler lt24811HVoHandler = this;
    while (lt24811HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24811HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_811H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24811HVoHandler.MissionPlayVO(playByDesignCode, lt24811HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24811HVoHandler.MissionPlayVO(playByDesignCode, lt24811HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24811HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_811H_VoHandler lt24811HVoHandler = this;
    while (lt24811HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24811HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_811H");
    if (entity.GetCardId() == "LT24_811H")
      yield return (object) lt24811HVoHandler.MissionPlaySound(playByDesignCode, lt24811HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_811H_VoHandler lt24811HVoHandler = this;
    while (lt24811HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24811HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_811H");
    if (turn == 1)
      yield return (object) lt24811HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24811HVoHandler.m_introLine);
  }
}
