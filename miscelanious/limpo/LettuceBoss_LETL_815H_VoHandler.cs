using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_815H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4Death_01 = new AssetReference("VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4Death_01.prefab:360502f78bbb40d4b66dffc00d88620f");
  private static readonly AssetReference VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01 = new AssetReference("VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01.prefab:21be207809e8454488283ee3034b15a0");
  private static readonly AssetReference VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4HeroPower_03 = new AssetReference("VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4HeroPower_03.prefab:80a4e2d96e044b3d9dee1b7b8c261d38");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_815H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_815H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4Death_01,
      (string) LettuceBoss_LETL_815H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01,
      (string) LettuceBoss_LETL_815H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4HeroPower_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_815H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4HeroPower_03;
    this.m_deathLine = (string) LettuceBoss_LETL_815H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_815H_VoHandler letl815HVoHandler = this;
    while (letl815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl815HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_815H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_815H")
    {
      string str = cardID;
      if (!(str == "LETL_815P1_01") && !(str == "LETL_815P1_02"))
      {
        if (str == "LETL_815P2_01" || str == "LETL_815P2_02")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl815HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_815H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl815HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_815H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_815H_VoHandler letl815HVoHandler = this;
    while (letl815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl815HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_815H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl815HVoHandler.MissionPlayVO(playByDesignCode, letl815HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl815HVoHandler.MissionPlayVO(playByDesignCode, letl815HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl815HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_815H_VoHandler letl815HVoHandler = this;
    while (letl815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl815HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_815H");
    if (entity.GetCardId() == "LETL_815H")
      yield return (object) letl815HVoHandler.MissionPlaySound(playByDesignCode, letl815HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_815H_VoHandler letl815HVoHandler = this;
    while (letl815HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl815HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_815H");
    if (turn == 1)
      yield return (object) letl815HVoHandler.MissionPlayVOOnce(playByDesignCode, letl815HVoHandler.m_introLine);
  }
}
