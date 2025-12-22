using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_847H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference ChromaggusBoss_Death_1 = new AssetReference("ChromaggusBoss_Death_1.prefab:0af71a3749e50c842a1b7faac6b11b7f");
  private static readonly AssetReference ChromaggusBoss_Start_1 = new AssetReference("ChromaggusBoss_Start_1.prefab:9658c158e9c81094180c1e07bf337dd7");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_847H_VoHandler.ChromaggusBoss_Start_1
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_847H_VoHandler.ChromaggusBoss_Death_1,
      (string) LettuceBoss_LETL_847H_VoHandler.ChromaggusBoss_Start_1
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_847H_VoHandler.ChromaggusBoss_Start_1;
    this.m_deathLine = (string) LettuceBoss_LETL_847H_VoHandler.ChromaggusBoss_Death_1;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_847H_VoHandler letl847HVoHandler = this;
    while (letl847HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl847HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_847H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_847H" && cardID == "LETL_847P2_01")
    {
      GameState.Get().SetBusy(true);
      yield return (object) letl847HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_847H_VoHandler.ChromaggusBoss_Start_1);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_847H_VoHandler letl847HVoHandler = this;
    while (letl847HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl847HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_847H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) letl847HVoHandler.MissionPlayVO(playByDesignCode, letl847HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) letl847HVoHandler.MissionPlayVO(playByDesignCode, letl847HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) letl847HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_847H_VoHandler letl847HVoHandler = this;
    while (letl847HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl847HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_847H");
    if (entity.GetCardId() == "LETL_847H")
      yield return (object) letl847HVoHandler.MissionPlaySound(playByDesignCode, letl847HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_847H_VoHandler letl847HVoHandler = this;
    while (letl847HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = letl847HVoHandler.FindEnemyActorInPlayByDesignCode("LETL_847H");
    if (turn == 1)
      yield return (object) letl847HVoHandler.MissionPlayVOOnce(playByDesignCode, letl847HVoHandler.m_introLine);
  }
}
