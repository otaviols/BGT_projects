using System.Collections;
using System.Collections.Generic;

public class LettuceBoss_LETL_848H_Heroic_VoHandler : VoPlaybackHandler
{
  private static readonly AssetReference VO_LETL_848H_Male_Dragon_Attack_01 = new AssetReference("VO_LETL_848H_Male_Dragon_Attack_01.prefab:f55ed48d239634e4abd8f4e085ac4454");
  private static readonly AssetReference VO_LETL_848H_Male_Dragon_Attack_02 = new AssetReference("VO_LETL_848H_Male_Dragon_Attack_02.prefab:598ed5033b12a294d9898622a46f27d6");
  private static readonly AssetReference VO_LETL_848H_Male_Dragon_Death_01 = new AssetReference("VO_LETL_848H_Male_Dragon_Death_01.prefab:33dd5718a67c9694992450d43085bce2");
  private static readonly AssetReference VO_LETL_848H_Male_Dragon_Idle_01 = new AssetReference("VO_LETL_848H_Male_Dragon_Idle_01.prefab:f745f9529a6a0b1458dd691a2597b47f");
  private static readonly AssetReference VO_LETL_848H_Male_Dragon_Intro_01 = new AssetReference("VO_LETL_848H_Male_Dragon_Intro_01.prefab:f4a87ff5f174b6642b8bc30f6e582fe4");
  private static readonly AssetReference VO_LETL_848H_Male_Dragon_Intro_02 = new AssetReference("VO_LETL_848H_Male_Dragon_Intro_02.prefab:64af15efdd377204ab98f047f42f2dbb");
  private static readonly AssetReference VO_HERO_03q_Female_BloodElf_MIRROR_START_01 = new AssetReference("VO_HERO_03q_Female_BloodElf_MIRROR_START_01.prefab:31df6829fcf37d145bd546f76f1a029f");
  private List<string> m_IdleLines = new List<string>()
  {
    (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Idle_01
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Attack_01,
      (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Attack_02,
      (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Death_01,
      (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Idle_01,
      (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Intro_01,
      (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Intro_02,
      (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_HERO_03q_Female_BloodElf_MIRROR_START_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      GameState.Get().GetGameEntity().PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Intro_01;
    this.m_deathLine = (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Death_01;
    this.m_standardEmoteResponseLine = (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_HERO_03q_Female_BloodElf_MIRROR_START_01;
  }

  public override IEnumerator RespondToWillPlayCardWithTiming(
    string cardID,
    Entity playedEntity)
  {
    LettuceBoss_LETL_848H_Heroic_VoHandler hHeroicVoHandler = this;
    while (hHeroicVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = hHeroicVoHandler.FindEnemyActorInPlayByDesignCode("LETL_848H4_Heroic");
    if (playedEntity.GetLettuceAbilityOwner().GetCardId() == "LETL_848H4_Heroic")
    {
      string str = cardID;
      if (!(str == "LETL_848P1_01"))
      {
        if (str == "LETL_848P1_03")
        {
          GameState.Get().SetBusy(true);
          yield return (object) hHeroicVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Attack_02);
          GameState.Get().SetBusy(false);
        }
      }
      else
      {
        GameState.Get().SetBusy(true);
        yield return (object) hHeroicVoHandler.MissionPlayVOOnce(playByDesignCode, (string) LettuceBoss_LETL_848H_Heroic_VoHandler.VO_LETL_848H_Male_Dragon_Attack_01);
        GameState.Get().SetBusy(false);
      }
    }
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LettuceBoss_LETL_848H_Heroic_VoHandler hHeroicVoHandler = this;
    while (hHeroicVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = hHeroicVoHandler.FindEnemyActorInPlayByDesignCode("LETL_848H_Heroic");
    Actor valeeraActor = hHeroicVoHandler.FindActorInPlayByDesignCode("LETL_848H6");
    switch (missionEvent)
    {
      case 514:
        yield return (object) hHeroicVoHandler.MissionPlayVO(playByDesignCode, hHeroicVoHandler.m_introLine);
        yield return (object) hHeroicVoHandler.MissionPlayVO(valeeraActor, hHeroicVoHandler.m_standardEmoteResponseLine);
        break;
      case 517:
        yield return (object) hHeroicVoHandler.MissionPlayVO(playByDesignCode, hHeroicVoHandler.m_IdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) hHeroicVoHandler.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override void NotifyOfMinionDied(Entity entity) => Gameplay.Get().StartCoroutine(this.NotifyOfMinionDiedWithTiming(entity));

  public IEnumerator NotifyOfMinionDiedWithTiming(Entity entity)
  {
    LettuceBoss_LETL_848H_Heroic_VoHandler hHeroicVoHandler = this;
    while (hHeroicVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = hHeroicVoHandler.FindEnemyActorInPlayByDesignCode("LETL_848H_Heroic");
    if (entity.GetCardId() == "LETL_848H_Heroic")
      yield return (object) hHeroicVoHandler.MissionPlaySound(playByDesignCode, hHeroicVoHandler.m_deathLine);
  }

  public override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LettuceBoss_LETL_848H_Heroic_VoHandler hHeroicVoHandler = this;
    while (hHeroicVoHandler.m_enemySpeaking)
      yield return (object) null;
    Actor playByDesignCode = hHeroicVoHandler.FindEnemyActorInPlayByDesignCode("LETL_848H_Heroic");
    Actor valeeraActor = hHeroicVoHandler.FindActorInPlayByDesignCode("LETL_848H6");
    if (turn == 1)
    {
      yield return (object) hHeroicVoHandler.MissionPlayVOOnce(playByDesignCode, hHeroicVoHandler.m_introLine);
      yield return (object) hHeroicVoHandler.MissionPlayVOOnce(valeeraActor, hHeroicVoHandler.m_standardEmoteResponseLine);
    }
  }
}
