using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_832H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Boss_Death_01 = new AssetReference("VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Boss_Death_01.prefab:da1b64ec5dcd7694eac0d67c240e4880");
  private static readonly AssetReference VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_BossAttack_01 = new AssetReference("VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_BossAttack_01.prefab:484551f72cc900e478af602332b3a57e");
  private static readonly AssetReference VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_BossStart_01 = new AssetReference("VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_BossStart_01.prefab:92219bb8f6e482f4ebe0ff4577c3ddfc");
  private static readonly AssetReference VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_EmoteResponse_01 = new AssetReference("VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_EmoteResponse_01.prefab:145f4cbc25254034f9a5ad8fd60539ec");
  private static readonly AssetReference VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_01_01 = new AssetReference("VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_01_01.prefab:849043988be8d9846a718f7fd239bc0e");
  private static readonly AssetReference VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_02_01 = new AssetReference("VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_02_01.prefab:d85dc236aeb873d4fa10374d9300d21c");
  private static readonly AssetReference VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_03_01 = new AssetReference("VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_03_01.prefab:6374771862d99fd4994ff3c74b2b9c36");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_01_01,
    (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_02_01,
    (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_03_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Boss_Death_01,
      (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_BossAttack_01,
      (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_BossStart_01,
      (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_EmoteResponse_01,
      (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_01_01,
      (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_02_01,
      (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Idle_03_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_BossStart_01;
    this.m_deathLine = (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_Boss_Death_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_832H_VoHandler letl832HVoHandler = this;
    while (letl832HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl832HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_832H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_832H" && cardID == "LETL_832P1_01")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl832HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_832H_VoHandler.VO_DRGA_BOSS_10h_Male_Elemental_Good_Fight_02_BossAttack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_832H_VoHandler letl832HVoHandler = this;
    while (letl832HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl832HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_832H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl832HVoHandler.MissionPlayVO(playByDesignCode, letl832HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl832HVoHandler.MissionPlayVO(playByDesignCode, letl832HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl832HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_832H_VoHandler letl832HVoHandler = this;
    while (letl832HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl832HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_832H");
    if (entity.GetCardId() == "LETL_832H")
      yield return (object) letl832HVoHandler.MissionPlaySound(playByDesignCode, letl832HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_832H_VoHandler letl832HVoHandler = this;
    while (letl832HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl832HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_832H");
    if (turn == 1)
      yield return (object) letl832HVoHandler.MissionPlayVOOnce(playByDesignCode, letl832HVoHandler.m_introLine);
  }
}
