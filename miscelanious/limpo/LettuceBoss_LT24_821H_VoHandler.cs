using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_821H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_PrinceMalchezaar_Male_Demon_LETL_Attack_01 = new AssetReference("VO_PrinceMalchezaar_Male_Demon_LETL_Attack_01.prefab:52f5238e917074545b4f7a08023d278b");
  private static readonly AssetReference VO_PrinceMalchezaar_Male_Demon_LETL_Attack_02 = new AssetReference("VO_PrinceMalchezaar_Male_Demon_LETL_Attack_02.prefab:c0b35b268187bec49abcb1f4c44f1cc6");
  private static readonly AssetReference VO_PrinceMalchezaar_Male_Demon_LETL_Attack_03 = new AssetReference("VO_PrinceMalchezaar_Male_Demon_LETL_Attack_03.prefab:2d5770ac675f51549992ace3a08af4e7");
  private static readonly AssetReference VO_PrinceMalchezaar_Male_Demon_LETL_Death_02 = new AssetReference("VO_PrinceMalchezaar_Male_Demon_LETL_Death_02.prefab:1f23ef572082db84083cbcd07b15f846");
  private static readonly AssetReference VO_PrinceMalchezaar_Male_Demon_LETL_Idle_01 = new AssetReference("VO_PrinceMalchezaar_Male_Demon_LETL_Idle_01.prefab:68e00c20497ff5848a4a42e356ac0459");
  private static readonly AssetReference VO_PrinceMalchezaar_Male_Demon_LETL_Idle_02 = new AssetReference("VO_PrinceMalchezaar_Male_Demon_LETL_Idle_02.prefab:691de03bd17b1b04aad7e803b5f8e428");
  private static readonly AssetReference VO_PrinceMalchezaar_Male_Demon_LETL_Idle_03 = new AssetReference("VO_PrinceMalchezaar_Male_Demon_LETL_Idle_03.prefab:f916720a82e770341b834b9d0038273d");
  private static readonly AssetReference VO_PrinceMalchezaar_Male_Demon_LETL_Intro_05 = new AssetReference("VO_PrinceMalchezaar_Male_Demon_LETL_Intro_05.prefab:902d90a7004e6784dada2800213497f3");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Idle_01,
    (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Idle_02,
    (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Idle_03
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Intro_05,
      (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Attack_01,
      (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Attack_02,
      (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Attack_03,
      (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Idle_01,
      (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Idle_02,
      (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Idle_03,
      (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Death_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Intro_05;
    this.m_deathLine = (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Death_02;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_821H_VoHandler lt24821HVoHandler = this;
    while (lt24821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24821HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_821H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_821H")
    {
      string str = cardID;
      if (!(str == "LETL_006P1_03"))
      {
        if (!(str == "LETL_006P1_05"))
        {
          if (!(str == "LETL_006P9_03"))
          {
            if (!(str == "LETL_006P9_05"))
            {
              if (!(str == "LETL_006P8_03"))
              {
                if (str == "LETL_006P8_05")
                {
                  GameState.Get().SetBusy(true);
                  yield return (object) lt24821HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Attack_01);
                  GameState.Get().SetBusy(false);
                }
              }
              else
              {
                GameState.Get().SetBusy(true);
                yield return (object) lt24821HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Attack_01);
                GameState.Get().SetBusy(false);
              }
            }
            else
            {
              GameState.Get().SetBusy(true);
              yield return (object) lt24821HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Attack_03);
              GameState.Get().SetBusy(false);
            }
          }
          else
          {
            GameState.Get().SetBusy(true);
            yield return (object) lt24821HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Attack_03);
            GameState.Get().SetBusy(false);
          }
        }
        else
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24821HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24821HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_821H_VoHandler.VO_PrinceMalchezaar_Male_Demon_LETL_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_821H_VoHandler lt24821HVoHandler = this;
    while (lt24821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24821HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_821H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24821HVoHandler.MissionPlayVO(playByDesignCode, lt24821HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24821HVoHandler.MissionPlayVO(playByDesignCode, lt24821HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24821HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_821H_VoHandler lt24821HVoHandler = this;
    while (lt24821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24821HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_821H");
    if (entity.GetCardId() == "LT24_821H")
      yield return (object) lt24821HVoHandler.MissionPlaySound(playByDesignCode, lt24821HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_821H_VoHandler lt24821HVoHandler = this;
    while (lt24821HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24821HVoHandler.FindEnemyActorInPlayByDesignCode("LT24_821H");
    if (turn == 1)
      yield return (object) lt24821HVoHandler.MissionPlayVOOnce(playByDesignCode, lt24821HVoHandler.m_introLine);
  }
}
