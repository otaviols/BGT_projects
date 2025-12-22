using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_822H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_822H_Male_Quilboar_Attack_01 = new AssetReference("VO_LETL_822H_Male_Quilboar_Attack_01.prefab:5cedb94f231cdc046840154a790830d7");
  private static readonly AssetReference VO_LETL_822H_Male_Quilboar_Attack_02 = new AssetReference("VO_LETL_822H_Male_Quilboar_Attack_02.prefab:c9c546eb243e9b54eb44234e6510a4c9");
  private static readonly AssetReference VO_LETL_822H_Male_Quilboar_Death_01 = new AssetReference("VO_LETL_822H_Male_Quilboar_Death_01.prefab:50fcd304ea21236498127207daab4d73");
  private static readonly AssetReference VO_LETL_822H_Male_Quilboar_Idle_01 = new AssetReference("VO_LETL_822H_Male_Quilboar_Idle_01.prefab:c790d69513d81e547befe33741a891e1");
  private static readonly AssetReference VO_LETL_822H_Male_Quilboar_Intro_01 = new AssetReference("VO_LETL_822H_Male_Quilboar_Intro_01.prefab:78ae619f5af43ef4f975e01328ee46fb");
  private static readonly AssetReference VO_LETL_822H_Male_Quilboar_Intro_02 = new AssetReference("VO_LETL_822H_Male_Quilboar_Intro_02.prefab:0c16f8b62b34fba4ea6c94ae1fedb625");
  private static readonly AssetReference VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4Death_01 = new AssetReference("VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4Death_01.prefab:360502f78bbb40d4b66dffc00d88620f");
  private static readonly AssetReference VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01 = new AssetReference("VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01.prefab:21be207809e8454488283ee3034b15a0");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Attack_01,
      (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Attack_02,
      (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Death_01,
      (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Idle_01,
      (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Intro_01,
      (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Intro_02,
      (string) LettuceBoss_LETL_822H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4Death_01,
      (string) LettuceBoss_LETL_822H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_822H_VoHandler letl822HVoHandler = this;
    while (letl822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor bossActor = letl822HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_822H");
    letl822HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_815H");
    Entity lettuceAbilityOwner = playedEntity.GetLettuceAbilityOwner();
    Actor actor = lettuceAbilityOwner.GetCard().GetActor();
    string designCode = lettuceAbilityOwner.GetCardId();
    if (designCode == "LETL_815H")
    {
      string str = cardID;
      if (!(str == "LETL_815P1_01") && !(str == "LETL_815P1_02"))
      {
        if (str == "LETL_815P2_01" || str == "LETL_815P2_02")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl822HVoHandler.MissionPlayVO(actor, (string) LettuceBoss_LETL_822H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl822HVoHandler.MissionPlayVO(actor, (string) LettuceBoss_LETL_822H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4EmoteResponse_01);
        GameState.Get().SetBusy(false);
      }
    }
    if (designCode == "LETL_822H")
    {
      string str = cardID;
      if (!(str == "LETL_822P1_01"))
      {
        if (str == "LETL_822P2_01")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl822HVoHandler.MissionPlayVOOnce(bossActor, (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl822HVoHandler.MissionPlayVOOnce(bossActor, (string) LettuceBoss_LETL_822H_VoHandler.VO_LETL_822H_Male_Quilboar_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_822H_VoHandler letl822HVoHandler = this;
    while (letl822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl822HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_822H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl822HVoHandler.MissionPlayVO(playByDesignCode, letl822HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl822HVoHandler.MissionPlayVO(playByDesignCode, letl822HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl822HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_822H_VoHandler letl822HVoHandler = this;
    while (letl822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode1 = letl822HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_822H");
    Actor playByDesignCode2 = letl822HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_815H");
    string cardId = entity.GetCardId();
    if (!(cardId == "LETL_822H"))
    {
      if (cardId == "LETL_815H")
        yield return (object) letl822HVoHandler.MissionPlaySound(playByDesignCode2, (string) LettuceBoss_LETL_822H_VoHandler.VO_Story_02_Quilboar_Male_Quillboar_Story_Rexxar_Mission4Death_01);
    }
    else
      yield return (object) letl822HVoHandler.MissionPlaySound(playByDesignCode1, letl822HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_822H_VoHandler letl822HVoHandler = this;
    while (letl822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl822HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_822H");
    if (turn == 1)
      yield return (object) letl822HVoHandler.MissionPlayVOOnce(playByDesignCode, letl822HVoHandler.m_introLine);
  }
}
