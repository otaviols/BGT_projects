using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_851H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossDeath_01 = new AssetReference("VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossDeath_01.prefab:78a5b8027f353824b92abe21ad1b3394");
  private static readonly AssetReference VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_01 = new AssetReference("VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_01.prefab:b92063736bbfd314fae75bfac8ba5bc3");
  private static readonly AssetReference VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_02 = new AssetReference("VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_02.prefab:d505b53db39386e439575bc6176646c2");
  private static readonly AssetReference VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_03 = new AssetReference("VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_03.prefab:7732f05989fca7840a83ddc57975fefe");
  private static readonly AssetReference VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossUsesHeroPower_01 = new AssetReference("VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossUsesHeroPower_01.prefab:23ddfe9c2d687324680898daf8a58e66");
  private static readonly AssetReference VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossUsesHeroPower_02 = new AssetReference("VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossUsesHeroPower_02.prefab:cc1a7cbafbd0d87448cfa99cd1c699b4");
  private static readonly AssetReference VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossUsesHeroPower_03 = new AssetReference("VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossUsesHeroPower_03.prefab:06050c29e87b9a645b8a0a882892ff90");
  private static readonly AssetReference VO_BOM_08_005_Male_Orc_Galvangar_InGame_EmoteResponse_01 = new AssetReference("VO_BOM_08_005_Male_Orc_Galvangar_InGame_EmoteResponse_01.prefab:ef25f6e32f36c95448994eaebe4b38a3");
  private static readonly AssetReference VO_BOM_08_005_Male_Orc_Galvangar_InGame_Introduction_05_01_B = new AssetReference("VO_BOM_08_005_Male_Orc_Galvangar_InGame_Introduction_05_01_B.prefab:aa7a60e0b9bc84f4c96682925dd5d15e");
  private static readonly AssetReference VO_BOM_08_005_Male_Orc_Galvangar_InGame_PlayerLoss_01 = new AssetReference("VO_BOM_08_005_Male_Orc_Galvangar_InGame_PlayerLoss_01.prefab:1e4e7e8132b10da4fa1d4c5d9fc8bfcd");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_01,
    (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossDeath_01,
      (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_01,
      (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_02,
      (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_03,
      (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossUsesHeroPower_01,
      (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossUsesHeroPower_02,
      (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossUsesHeroPower_03,
      (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_EmoteResponse_01,
      (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_Introduction_05_01_B,
      (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_PlayerLoss_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_Introduction_05_01_B;
    this.m_deathLine = (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossDeath_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_851H_VoHandler letl851HVoHandler = this;
    while (letl851HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl851HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_851H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_851H")
    {
      string str = cardID;
      if (!(str == "LETL_1117_03") && !(str == "LETL_1117_05"))
      {
        if (str == "LETL_234_03" || str == "LETL_234_05")
        {
          GameState.Get().SetBusy(true);
          yield return (object) letl851HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossIdle_03);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl851HVoHandler.MissionPlayVO(playByDesignCode, (string) LettuceBoss_LETL_851H_VoHandler.VO_BOM_08_005_Male_Orc_Galvangar_InGame_BossUsesHeroPower_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_851H_VoHandler letl851HVoHandler = this;
    while (letl851HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl851HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_851H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl851HVoHandler.MissionPlayVO(playByDesignCode, letl851HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl851HVoHandler.MissionPlayVO(playByDesignCode, letl851HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl851HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_851H_VoHandler letl851HVoHandler = this;
    while (letl851HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl851HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_851H");
    if (entity.GetCardId() == "LETL_851H")
      yield return (object) letl851HVoHandler.MissionPlaySound(playByDesignCode, letl851HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_851H_VoHandler letl851HVoHandler = this;
    while (letl851HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl851HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_851H");
    if (turn == 1)
      yield return (object) letl851HVoHandler.MissionPlayVOOnce(playByDesignCode, letl851HVoHandler.m_introLine);
  }
}
