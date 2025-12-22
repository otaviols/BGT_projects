using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT24_814H6_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_Romulo_Male_Human_LETL_Attack_01 = new AssetReference("VO_Romulo_Male_Human_LETL_Attack_01.prefab:c2877026223c8c649b59bed0e1d81e88");
  private static readonly AssetReference VO_Romulo_Male_Human_LETL_Idle_01 = new AssetReference("VO_Romulo_Male_Human_LETL_Idle_01.prefab:102c9a075aa2d7e4a89fcccb441079cf");
  private static readonly AssetReference VO_Romulo_Male_Human_LETL_Idle_02 = new AssetReference("VO_Romulo_Male_Human_LETL_Idle_02.prefab:6540f245af9e44549a506e87b107e692");
  private static readonly AssetReference VO_Julianne_Female_Human_LETL_Attack_01 = new AssetReference("VO_Julianne_Female_Human_LETL_Attack_01.prefab:8ecc86de9b3d4d041af2f6e000fed9d0");
  private static readonly AssetReference VO_Julianne_Female_Human_LETL_Idle_01 = new AssetReference("VO_Julianne_Female_Human_LETL_Idle_01.prefab:1a7272aad4a93334599d43c829256551");
  private static readonly AssetReference VO_Julianne_Female_Human_LETL_Idle_02 = new AssetReference("VO_Julianne_Female_Human_LETL_Idle_02.prefab:15aba187fd4ba8a478b10a2ab9b994d8");
  private List<string> m_IdleLine = new List<string>()
  {
    (string) LettuceBoss_LT24_814H6_VoHandler.VO_Romulo_Male_Human_LETL_Idle_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT24_814H6_VoHandler.VO_Romulo_Male_Human_LETL_Attack_01,
      (string) LettuceBoss_LT24_814H6_VoHandler.VO_Romulo_Male_Human_LETL_Idle_01,
      (string) LettuceBoss_LT24_814H6_VoHandler.VO_Romulo_Male_Human_LETL_Idle_02,
      (string) LettuceBoss_LT24_814H6_VoHandler.VO_Julianne_Female_Human_LETL_Attack_01,
      (string) LettuceBoss_LT24_814H6_VoHandler.VO_Julianne_Female_Human_LETL_Idle_01,
      (string) LettuceBoss_LT24_814H6_VoHandler.VO_Julianne_Female_Human_LETL_Idle_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT24_814H6_VoHandler lt24814H6VoHandler = this;
    while (lt24814H6VoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode1 = lt24814H6VoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H6");
    Actor playByDesignCode2 = lt24814H6VoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H7");
    string cardId = playedEntity.GetLettuceAbilityOwner().GetCardId();
    if (cardId == "LT24_814H6")
    {
      if (cardID == "LT24_814P5")
      {
        GameState.Get().SetBusy(true);
        yield return (object) lt24814H6VoHandler.MissionPlayVOOnce(playByDesignCode1, (string) LettuceBoss_LT24_814H6_VoHandler.VO_Romulo_Male_Human_LETL_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
    else if (cardId == "LT24_814H7" && cardID == "LT24_814P6")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt24814H6VoHandler.MissionPlayVOOnce(playByDesignCode2, (string) LettuceBoss_LT24_814H6_VoHandler.VO_Julianne_Female_Human_LETL_Attack_01);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLine;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT24_814H6_VoHandler lt24814H6VoHandler = this;
    while (lt24814H6VoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt24814H6VoHandler.FindEnemyActorInPlayByDesignCode("LT24_814H6");
    if (missionEvent == 517)
    {
      yield return (object) lt24814H6VoHandler.MissionPlayVO(playByDesignCode, lt24814H6VoHandler.m_IdleLine);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) lt24814H6VoHandler.\u003C\u003En__0(missionEvent);
    }
  }
}
