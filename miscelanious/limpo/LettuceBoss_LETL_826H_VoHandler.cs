using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_826H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_826H_Male_NightElf_Attack_01 = new AssetReference("VO_LETL_826H_Male_NightElf_Attack_01.prefab:5fdbf6b5e00013d4e99fdfb21583736b");
  private static readonly AssetReference VO_LETL_826H_Male_NightElf_Attack_02 = new AssetReference("VO_LETL_826H_Male_NightElf_Attack_02.prefab:814e61f095bc4e44f8d87899ef2dffad");
  private static readonly AssetReference VO_LETL_826H_Male_NightElf_Death_01 = new AssetReference("VO_LETL_826H_Male_NightElf_Death_01.prefab:f6e029176ef306d49b48386a4336e932");
  private static readonly AssetReference VO_LETL_826H_Male_NightElf_Idle_01 = new AssetReference("VO_LETL_826H_Male_NightElf_Idle_01.prefab:37f6eb04fb6eede488f2a557c96ea225");
  private static readonly AssetReference VO_LETL_826H_Male_NightElf_Intro_01 = new AssetReference("VO_LETL_826H_Male_NightElf_Intro_01.prefab:2cad5dc95601ae64d9a405703431d717");
  private static readonly AssetReference VO_LETL_826H2_Male_Tauren_Attack_01 = new AssetReference("VO_LETL_826H2_Male_Tauren_Attack_01.prefab:74d4fb515ee47f649ba82032b4796c84");
  private static readonly AssetReference VO_LETL_826H2_Male_Tauren_Attack_02 = new AssetReference("VO_LETL_826H2_Male_Tauren_Attack_02.prefab:d90c73f52533c0d4a9e2148aba28a056");
  private static readonly AssetReference VO_LETL_826H2_Male_Tauren_Death_01 = new AssetReference("VO_LETL_826H2_Male_Tauren_Death_01.prefab:cd42d0bbdd9ed2c43824f7df0a2a103e");
  private static readonly AssetReference VO_LETL_826H2_Male_Tauren_Idle_01 = new AssetReference("VO_LETL_826H2_Male_Tauren_Idle_01.prefab:fb5bc3ed3a3faa64bbffb2b2c7051aff");
  private static readonly AssetReference VO_LETL_826H2_Male_Tauren_Intro_01 = new AssetReference("VO_LETL_826H2_Male_Tauren_Intro_01.prefab:e24a53542fb86584181dc2cf317c6421");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H_Male_NightElf_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H_Male_NightElf_Attack_01,
      (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H_Male_NightElf_Attack_02,
      (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H_Male_NightElf_Death_01,
      (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H_Male_NightElf_Idle_01,
      (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H_Male_NightElf_Intro_01,
      (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H2_Male_Tauren_Attack_01,
      (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H2_Male_Tauren_Attack_02,
      (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H2_Male_Tauren_Death_01,
      (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H2_Male_Tauren_Idle_01,
      (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H2_Male_Tauren_Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H_Male_NightElf_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H_Male_NightElf_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_826H_VoHandler letl826HVoHandler = this;
    while (letl826HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl826HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_826H");
    Actor bossguestActor = letl826HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_826H2");
    string designCode = playedEntity.GetLettuceAbilityOwner().GetCardId();
    if (designCode == "LETL_826H")
    {
      string str = cardID;
      if (!(str == "LETL_029P6_02"))
      {
        if (str == "LETL_463_02")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl826HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H_Male_NightElf_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl826HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H_Male_NightElf_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
    if (designCode == "LETL_826H2")
    {
      string str = cardID;
      if (!(str == "LETL_471_02"))
      {
        if (str == "LETL_472_03")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl826HVoHandler.MissionPlayVOOnce(bossguestActor, (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H2_Male_Tauren_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl826HVoHandler.MissionPlayVOOnce(bossguestActor, (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H2_Male_Tauren_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_826H_VoHandler letl826HVoHandler = this;
    while (letl826HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl826HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_826H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl826HVoHandler.MissionPlayVO(playByDesignCode, letl826HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl826HVoHandler.MissionPlayVO(playByDesignCode, letl826HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl826HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_826H_VoHandler letl826HVoHandler = this;
    while (letl826HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode1 = letl826HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_826H");
    Actor playByDesignCode2 = letl826HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_826H2");
    string cardId = entity.GetCardId();
    if (!(cardId == "LETL_826H"))
    {
      if (cardId == "LETL_826H2")
        yield return (object) letl826HVoHandler.MissionPlaySound(playByDesignCode2, (string) LettuceBoss_LETL_826H_VoHandler.VO_LETL_826H2_Male_Tauren_Death_01);
    }
    else
      yield return (object) letl826HVoHandler.MissionPlaySound(playByDesignCode1, letl826HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_826H_VoHandler letl826HVoHandler = this;
    while (letl826HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl826HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_826H");
    if (turn == 1)
      yield return (object) letl826HVoHandler.MissionPlayVOOnce(playByDesignCode, letl826HVoHandler.m_introLine);
  }
}
