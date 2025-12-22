using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_820H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Death_01 = new AssetReference("VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Death_01.prefab:cee9af5f93604f543ae52d62c22b7a25");
  private static readonly AssetReference VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3ExchangeA_01 = new AssetReference("VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3ExchangeA_01.prefab:6f38b4201afb1564997c88ea4f83ce4a");
  private static readonly AssetReference VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_01 = new AssetReference("VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_01.prefab:d6db2227c9dcb7345a05de10aea7b262");
  private static readonly AssetReference VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_02 = new AssetReference("VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_02.prefab:568e12955b775ba48b69f4dee0bd1c53");
  private static readonly AssetReference VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_03 = new AssetReference("VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_03.prefab:708e426e1a5f7bc48b90d0dd6ba20a79");
  private static readonly AssetReference VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_01 = new AssetReference("VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_01.prefab:2966bd6c1b4df3e4da8af264c7fa9b3c");
  private static readonly AssetReference VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_02 = new AssetReference("VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_02.prefab:417d4a0429e188d43b084fcf6337def6");
  private static readonly AssetReference VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_03 = new AssetReference("VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_03.prefab:107f51e08e2990a4aacbcda196c57ed3");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_01,
    (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_02,
    (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_03
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Death_01,
      (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3ExchangeA_01,
      (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_01,
      (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_02,
      (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_03,
      (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_01,
      (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_02,
      (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Idle_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3ExchangeA_01;
    this.m_deathLine = (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_820H_VoHandler letl820HVoHandler = this;
    while (letl820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl820HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_820H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_820H")
    {
      string str = cardID;
      if (!(str == "LETL_820P1_01"))
      {
        if (str == "LETL_820P2_01")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl820HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl820HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_820H_VoHandler.VO_Story_Hero_Barak_Male_Centaur_Story_Guff_Mission3HeroPower_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_820H_VoHandler letl820HVoHandler = this;
    while (letl820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl820HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_820H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl820HVoHandler.MissionPlayVO(playByDesignCode, letl820HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl820HVoHandler.MissionPlayVO(playByDesignCode, letl820HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl820HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_820H_VoHandler letl820HVoHandler = this;
    while (letl820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl820HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_820H");
    if (entity.GetCardId() == "LETL_820H")
      yield return (object) letl820HVoHandler.MissionPlaySound(playByDesignCode, letl820HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_820H_VoHandler letl820HVoHandler = this;
    while (letl820HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl820HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_820H");
    if (turn == 1)
      yield return (object) letl820HVoHandler.MissionPlayVOOnce(playByDesignCode, letl820HVoHandler.m_introLine);
  }
}
