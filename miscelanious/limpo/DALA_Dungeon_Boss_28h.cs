using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_28h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_9Cost_TurnNine_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_9Cost_TurnNine_01.prefab:851c0528f4a2c6f47b91d4b94179037e");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_Death_02 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_Death_02.prefab:cc321e4037a345b44b1ed5a3ddbeaac4");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_DefeatPlayer_01.prefab:1ae91868c5f54514f8cb5b16083989c4");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_EmoteResponse_01.prefab:450a954b9b14c6744b175e0da9c96de4");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_01.prefab:4ea6893c98e286b44abbdd7398dc0486");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_02 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_02.prefab:924346856bee558488e9c5a084ffa17f");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_03 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_03.prefab:8f8e49619f8511d4992c4af34f222c40");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_01.prefab:2efbabf2c8fa1d04bbb07f1cf2f7d34e");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_02 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_02.prefab:c03a5b79e1177094ab994e98a884271e");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_03 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_03.prefab:5c0c723ad23b03a48aefef87a0c57dc7");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_Idle_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_Idle_01.prefab:2bf8a022c0c3e594e97c2797d064fc5c");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_Idle_02 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_Idle_02.prefab:fd67534e228e2a64f9f217f1532e85be");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_Idle_03 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_Idle_03.prefab:031ff6b2cc3c6a241843821a65b1ba65");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_Idle_04 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_Idle_04.prefab:0bf28d1ace312ed4db0344766c0a0623");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_Idle_05 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_Idle_05.prefab:9df996bba71610c4ebdf8bf6c7fe095a");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_Intro_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_Intro_01.prefab:38266a546ec3f1f4486c0c960a427053");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_IntroHunter_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_IntroHunter_01.prefab:21524542671d7aa49b2bacff7332c2b8");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_IntroWarlock_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_IntroWarlock_01.prefab:5b5ce988cd6843a46a6910cde4f2fbd7");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_IntroWarrior_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_IntroWarrior_01.prefab:dac080b958fab404fb6b13e668aaa270");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_PlayerCoin_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_PlayerCoin_01.prefab:1c1a88eca2769f94a8e6965bf41ec3ee");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_PlayerDarkness_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_PlayerDarkness_01.prefab:c5d15d3a07fdbc744a96a8d1d6999f4e");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_PlayerLegendaryWeapon_01 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_PlayerLegendaryWeapon_01.prefab:3d8d86f7a80cbed4191e4a799362a605");
  private static readonly AssetReference VO_DALA_BOSS_28h_Male_Ethereal_PlayerLegendaryWeapon_02 = new AssetReference("VO_DALA_BOSS_28h_Male_Ethereal_PlayerLegendaryWeapon_02.prefab:edae63901b06d8d45a039239ec757d0a");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Idle_01,
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Idle_02,
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Idle_03,
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Idle_04,
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Idle_05
  };
  private static List<string> m_PlayerLegendaryWeapon = new List<string>()
  {
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_PlayerLegendaryWeapon_01,
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_PlayerLegendaryWeapon_02
  };
  private static List<string> m_PlayerHeroPower = new List<string>()
  {
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_01,
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_02,
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_03
  };
  private static List<string> m_BossHeroPower = new List<string>()
  {
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_01,
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_02,
    (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_9Cost_TurnNine_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Death_02,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_02,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerBoss_03,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_02,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_HeroPowerPlayer_03,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Idle_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Idle_02,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Idle_03,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Idle_04,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Idle_05,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Intro_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_IntroHunter_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_IntroWarlock_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_IntroWarrior_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_PlayerCoin_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_PlayerDarkness_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_PlayerLegendaryWeapon_01,
      (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_PlayerLegendaryWeapon_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_Death_02;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_28h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => false;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "DALA_Barkeye")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_IntroHunter_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "DALA_Chu")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_IntroWarrior_01, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "DALA_Tekahn"))
          return;
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_28h dalaDungeonBoss28h = this;
    while (dalaDungeonBoss28h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss28h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_28h.m_BossHeroPower);
        break;
      case 102:
        yield return (object) dalaDungeonBoss28h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_28h.m_PlayerHeroPower);
        break;
      case 103:
        yield return (object) dalaDungeonBoss28h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_28h.m_PlayerLegendaryWeapon);
        break;
      case 104:
        yield return (object) dalaDungeonBoss28h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_9Cost_TurnNine_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss28h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_28h dalaDungeonBoss28h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss28h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss28h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss28h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss28h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss28h.m_playedLines.Add(cardId);
      if (!(cardId == "GAME_005") && !(cardId == "GVG_028t"))
      {
        if (cardId == "LOOT_526")
          yield return (object) dalaDungeonBoss28h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_PlayerDarkness_01);
      }
      else
        yield return (object) dalaDungeonBoss28h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_28h.VO_DALA_BOSS_28h_Male_Ethereal_PlayerCoin_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_28h dalaDungeonBoss28h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss28h.\u003C\u003En__2(entity);
    while (dalaDungeonBoss28h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss28h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss28h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss28h.m_playedLines.Add(cardId);
    }
  }
}
