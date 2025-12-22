using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_817H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2Death_01 = new AssetReference("VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2Death_01.prefab:29971ca128573ea488eb4479810e57d9");
  private static readonly AssetReference VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2EmoteResponse_01 = new AssetReference("VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2EmoteResponse_01.prefab:a3b2b5e278c44794f9833088ecc84cda");
  private static readonly AssetReference VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_01 = new AssetReference("VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_01.prefab:d441233485d1c704a93bfb861a2323ec");
  private static readonly AssetReference VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_02.prefab:88bcffef7c36ed04196478b1dda606f1");
  private static readonly AssetReference VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_03 = new AssetReference("VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_03.prefab:3b6f517ee3e5f00469259ab1f87a2c8e");
  private static readonly AssetReference VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_04 = new AssetReference("VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_04.prefab:9c1afe58d526d8940ad5d59660fbb504");
  private static readonly AssetReference VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_05 = new AssetReference("VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_05.prefab:abe095c594ea39e4a82e48844729f695");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_817H_VoHandler.VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_05
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_817H_VoHandler.VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2Death_01,
      (string) LettuceBoss_LETL_817H_VoHandler.VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2EmoteResponse_01,
      (string) LettuceBoss_LETL_817H_VoHandler.VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_02,
      (string) LettuceBoss_LETL_817H_VoHandler.VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_03,
      (string) LettuceBoss_LETL_817H_VoHandler.VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_05
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_817H_VoHandler.VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2EmoteResponse_01;
    this.m_deathLine = (string) LettuceBoss_LETL_817H_VoHandler.VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_817H_VoHandler letl817HVoHandler = this;
    while (letl817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl817HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_817H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_817H")
    {
      string str = cardID;
      if (!(str == "LETL_817P3_01"))
      {
        if (str == "LETL_817P2_01")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl817HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_817H_VoHandler.VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_03);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl817HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_817H_VoHandler.VO_Story_Hero_Serena_Female_Harpy_Story_Xyrella_Mission2HeroPower_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_817H_VoHandler letl817HVoHandler = this;
    while (letl817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl817HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_817H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl817HVoHandler.MissionPlayVO(playByDesignCode, letl817HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl817HVoHandler.MissionPlayVO(playByDesignCode, letl817HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl817HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_817H_VoHandler letl817HVoHandler = this;
    while (letl817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl817HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_817H");
    if (entity.GetCardId() == "LETL_817H")
      yield return (object) letl817HVoHandler.MissionPlaySound(playByDesignCode, letl817HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_817H_VoHandler letl817HVoHandler = this;
    while (letl817HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl817HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_817H");
    if (turn == 1)
      yield return (object) letl817HVoHandler.MissionPlayVOOnce(playByDesignCode, letl817HVoHandler.m_introLine);
  }
}
