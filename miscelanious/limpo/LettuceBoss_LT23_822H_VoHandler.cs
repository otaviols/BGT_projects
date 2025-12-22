using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LT23_822H_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_SilasDarkmoon_Male_Gnome_LETL_Attack_03 = new AssetReference("VO_SilasDarkmoon_Male_Gnome_LETL_Attack_03.prefab:7d2fa57d323c0b74c8447f78d9bc692b");
  private static readonly AssetReference VO_SilasDarkmoon_Male_Gnome_LETL_Death_02 = new AssetReference("VO_SilasDarkmoon_Male_Gnome_LETL_Death_02.prefab:f67706d79227de44090b5ec021f7ac6a");
  private static readonly AssetReference VO_SilasDarkmoon_Male_Gnome_LETL_Idle_01 = new AssetReference("VO_SilasDarkmoon_Male_Gnome_LETL_Idle_01.prefab:f2415e39042b2fd4aae7aa45e0d29107");
  private static readonly AssetReference VO_SilasDarkmoon_Male_Gnome_LETL_Intro_01 = new AssetReference("VO_SilasDarkmoon_Male_Gnome_LETL_Intro_01.prefab:86b9436434ae841418442a2470d58999");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LT23_822H_VoHandler.VO_SilasDarkmoon_Male_Gnome_LETL_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LT23_822H_VoHandler.VO_SilasDarkmoon_Male_Gnome_LETL_Intro_01,
      (string) LettuceBoss_LT23_822H_VoHandler.VO_SilasDarkmoon_Male_Gnome_LETL_Attack_03,
      (string) LettuceBoss_LT23_822H_VoHandler.VO_SilasDarkmoon_Male_Gnome_LETL_Idle_01,
      (string) LettuceBoss_LT23_822H_VoHandler.VO_SilasDarkmoon_Male_Gnome_LETL_Death_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LT23_822H_VoHandler.VO_SilasDarkmoon_Male_Gnome_LETL_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LT23_822H_VoHandler.VO_SilasDarkmoon_Male_Gnome_LETL_Death_02;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LT23_822H_VoHandler lt23822HVoHandler = this;
    while (lt23822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23822HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_822H");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LT23_822H" && cardID == "LT23_822P1")
    {
      GameState.Get().SetBusy(true);
      yield return (object) lt23822HVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LT23_822H_VoHandler.VO_SilasDarkmoon_Male_Gnome_LETL_Attack_03);
      GameState.Get().SetBusy(false);
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LT23_822H_VoHandler lt23822HVoHandler = this;
    while (lt23822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23822HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_822H");
    switch (missionEvent)
    {
      case 514:
        yield return (object) lt23822HVoHandler.MissionPlayVO(playByDesignCode, lt23822HVoHandler.m_introLine);
        break;
      case 517:
        yield return (object) lt23822HVoHandler.MissionPlayVO(playByDesignCode, lt23822HVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) lt23822HVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LT23_822H_VoHandler lt23822HVoHandler = this;
    while (lt23822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23822HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_822H");
    if (entity.GetCardId() == "LT23_822H")
      yield return (object) lt23822HVoHandler.MissionPlaySound(playByDesignCode, lt23822HVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LT23_822H_VoHandler lt23822HVoHandler = this;
    while (lt23822HVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = lt23822HVoHandler.FindEnemyActorInPlayByDesignCode("LT23_822H");
    if (turn == 1)
      yield return (object) lt23822HVoHandler.MissionPlayVOOnce(playByDesignCode, lt23822HVoHandler.m_introLine);
  }
}
