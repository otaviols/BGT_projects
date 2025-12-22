using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_814H4_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_BigBadWolf_Male_Beast_LETL_Attack_01 = new AssetReference("VO_BigBadWolf_Male_Beast_LETL_Attack_01.prefab:b01dcd5a588dd1d4990316c9b85e3ad1");
  private static readonly AssetReference VO_BigBadWolf_Male_Beast_LETL_Attack_02 = new AssetReference("VO_BigBadWolf_Male_Beast_LETL_Attack_02.prefab:2cf0dff6908c98b468979a30288372bb");
  private static readonly AssetReference VO_BigBadWolf_Male_Beast_LETL_Death_01 = new AssetReference("VO_BigBadWolf_Male_Beast_LETL_Death_01.prefab:062f7d663111ed24eae395e3325be12f");
  private static readonly AssetReference VO_BigBadWolf_Male_Beast_LETL_Idle_01 = new AssetReference("VO_BigBadWolf_Male_Beast_LETL_Idle_01.prefab:abbc8f51dcf4eaa48ba5b2427230633b");
  private static readonly AssetReference VO_BigBadWolf_Male_Beast_LETL_Intro_01 = new AssetReference("VO_BigBadWolf_Male_Beast_LETL_Intro_01.prefab:87643dd8f103a3f4e81adace37af1a1f");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT24_814H4_VoHandler.VO_BigBadWolf_Male_Beast_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_814H4_VoHandler.VO_BigBadWolf_Male_Beast_LETL_Intro_01,
      (string) LettuceBoss_LT24_814H4_VoHandler.VO_BigBadWolf_Male_Beast_LETL_Idle_01,
      (string) LettuceBoss_LT24_814H4_VoHandler.VO_BigBadWolf_Male_Beast_LETL_Attack_01,
      (string) LettuceBoss_LT24_814H4_VoHandler.VO_BigBadWolf_Male_Beast_LETL_Attack_02,
      (string) LettuceBoss_LT24_814H4_VoHandler.VO_BigBadWolf_Male_Beast_LETL_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT24_814H4_VoHandler.VO_BigBadWolf_Male_Beast_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT24_814H4_VoHandler.VO_BigBadWolf_Male_Beast_LETL_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_814H4_VoHandler lt24814H4VoHandler = this;
    while (lt24814H4VoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24814H4VoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H4");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT24_814H4")
    {
      string str = cardID;
      if (!(str == "LT24_814P3"))
      {
        if (str == "LT24_814P4")
        {
          GameState.Get().SetBusy(true);
          yield return (object) lt24814H4VoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_814H4_VoHandler.VO_BigBadWolf_Male_Beast_LETL_Attack_01);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24814H4VoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT24_814H4_VoHandler.VO_BigBadWolf_Male_Beast_LETL_Attack_02);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_814H4_VoHandler lt24814H4VoHandler = this;
    while (lt24814H4VoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24814H4VoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H4");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt24814H4VoHandler.MissionPlayVO(playByDesignCode, lt24814H4VoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt24814H4VoHandler.MissionPlayVO(playByDesignCode, lt24814H4VoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt24814H4VoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT24_814H4_VoHandler lt24814H4VoHandler = this;
    while (lt24814H4VoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24814H4VoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H4");
    if (entity.GetCardId() == "LT24_814H4")
      yield return (object) lt24814H4VoHandler.MissionPlaySound(playByDesignCode, lt24814H4VoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT24_814H4_VoHandler lt24814H4VoHandler = this;
    while (lt24814H4VoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24814H4VoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H4");
    if (turn == 1)
      yield return (object) lt24814H4VoHandler.MissionPlayVOOnce(playByDesignCode, lt24814H4VoHandler.m_introLine);
  }
}
