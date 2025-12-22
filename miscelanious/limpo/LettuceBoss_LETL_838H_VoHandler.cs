using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_838H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_BRMA03_1_CARD_04 = new AssetReference("VO_BRMA03_1_CARD_04.prefab:2ebdf13895d3b4e4e8979764b99e89e0");
  private static readonly AssetReference VO_BRMA03_1_HERO_POWER_06 = new AssetReference("VO_BRMA03_1_HERO_POWER_06.prefab:2ad44580bf0939c4292a8a454a6fb859");
  private static readonly AssetReference VO_BRM_028_Death_08 = new AssetReference("VO_BRM_028_Death_08.prefab:b61171d1faa5daa4aadcc4743027bb50");
  private List<string> m_IdleLines = new List<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_838H_VoHandler.VO_BRMA03_1_CARD_04,
      (string) LettuceBoss_LETL_838H_VoHandler.VO_BRMA03_1_HERO_POWER_06,
      (string) LettuceBoss_LETL_838H_VoHandler.VO_BRM_028_Death_08
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_838H_VoHandler.VO_BRMA03_1_CARD_04;
    this.m_deathLine = (string) LettuceBoss_LETL_838H_VoHandler.VO_BRM_028_Death_08;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_838H_VoHandler letl838HVoHandler = this;
    while (letl838HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl838HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_838H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_838H")
    {
      string str = cardID;
      if (str == "LETL_838P1_03" || str == "LETL_838P1_04")
      {
        GameState.Get().SetBusy(true);
        yield return (object) letl838HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_838H_VoHandler.VO_BRMA03_1_HERO_POWER_06);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_838H_VoHandler letl838HVoHandler = this;
    while (letl838HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl838HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_838H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl838HVoHandler.MissionPlayVO(playByDesignCode, letl838HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl838HVoHandler.MissionPlayVO(playByDesignCode, letl838HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl838HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_838H_VoHandler letl838HVoHandler = this;
    while (letl838HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl838HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_838H");
    if (entity.GetCardId() == "LETL_838H")
      yield return (object) letl838HVoHandler.MissionPlaySound(playByDesignCode, letl838HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_838H_VoHandler letl838HVoHandler = this;
    while (letl838HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl838HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_838H");
    if (turn == 1)
      yield return (object) letl838HVoHandler.MissionPlayVOOnce(playByDesignCode, letl838HVoHandler.m_introLine);
  }
}
