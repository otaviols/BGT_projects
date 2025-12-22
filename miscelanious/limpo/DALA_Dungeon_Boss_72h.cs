using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_72h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_BossHellfire_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_BossHellfire_01.prefab:8988a82632401a140a15d8c1a660b4b4");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_BossHellfire_02 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_BossHellfire_02.prefab:3b8dd3526cb892244aa3b4ebba269eab");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_Death_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_Death_01.prefab:ce35bb6b5c7b393429b44bd08804d365");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_DefeatPlayer_01.prefab:500362ca05e78c344807b2accadc6fca");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_EmoteResponse_01.prefab:5be88a443b01d2642a381b4a160ffb38");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_01.prefab:abbf8555486928643821662eeb81389e");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_02 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_02.prefab:f4f991c53e2358445a892e731c654faa");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_03 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_03.prefab:dc090b44a77a8be4eb3ba785ff50fa38");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_05 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_05.prefab:f634efd6b5fe2584ab656871377ac8d1");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_06 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_06.prefab:a3ad8408b4dbb6c499a96fc6f0105caf");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_07 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_07.prefab:eedec59b161c03e4a820322fa1baef0c");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_Idle_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_Idle_01.prefab:be19e94b39060734393434f14fdd0178");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_Idle_02 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_Idle_02.prefab:04640dbd544b35d4e8ecaa76162857eb");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_Idle_03 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_Idle_03.prefab:16c74b8e5517fb5428f84203e8b06cf9");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_Intro_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_Intro_01.prefab:9438155b1c3ffa445a46c8f44981cd57");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_IntroChu_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_IntroChu_01.prefab:e0c7236fc90da914696de05a698688de");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_IntroGeorge_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_IntroGeorge_01.prefab:c2f8625463a268d489e4075236119797");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_IntroTekahn_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_IntroTekahn_01.prefab:6747b1561dc4d754284ce800882b2fa4");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_PlayerArmor_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_PlayerArmor_01.prefab:af04d08c416998f41bb6dc2736f5caab");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_PlayerCrystalizer_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_PlayerCrystalizer_01.prefab:96fcb64aa3048614d917a868a9704e10");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_01 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_01.prefab:c8af605cc72f4f942bc1a4b5f045582d");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_02 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_02.prefab:94b8acaa3d30a2345849afd1e2a66917");
  private static readonly AssetReference VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_04 = new AssetReference("VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_04.prefab:e1e7f8f30939a534fbea91897c5f305a");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_Idle_01,
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_Idle_02,
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_Idle_03
  };
  private static List<string> m_HeroPowerTriggers = new List<string>()
  {
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_01,
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_02,
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_03,
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_05,
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_06,
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_07
  };
  private static List<string> m_PlayerSelfDamage = new List<string>()
  {
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_01,
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_02,
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_04
  };
  private static List<string> m_BossHellfire = new List<string>()
  {
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_BossHellfire_01,
    (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_BossHellfire_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_BossHellfire_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_BossHellfire_02,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_Death_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_02,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_03,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_05,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_06,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_HeroPowerTrigger_07,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_Idle_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_Idle_02,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_Idle_03,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_Intro_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_IntroChu_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_IntroGeorge_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_IntroTekahn_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_PlayerArmor_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_PlayerCrystalizer_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_01,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_02,
      (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_PlayerSelfDamage_04
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_EmoteResponse_01;
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_72h.m_IdleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "DALA_George")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_IntroGeorge_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "DALA_Chu")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_IntroChu_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "DALA_Tekahn")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_IntroTekahn_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_introLine, Notification.SpeechBubbleDirection.TopRight, actor));
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
    DALA_Dungeon_Boss_72h dalaDungeonBoss72h = this;
    while (dalaDungeonBoss72h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss72h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_72h.m_HeroPowerTriggers);
        break;
      case 102:
        yield return (object) dalaDungeonBoss72h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_72h.m_PlayerSelfDamage);
        break;
      case 103:
        yield return (object) dalaDungeonBoss72h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_PlayerArmor_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss72h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_72h dalaDungeonBoss72h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss72h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss72h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss72h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      yield return (object) dalaDungeonBoss72h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss72h.m_playedLines.Add(cardId);
      if (cardId == "BOT_447")
        yield return (object) dalaDungeonBoss72h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_72h.VO_DALA_BOSS_72h_Male_Orc_PlayerCrystalizer_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_72h dalaDungeonBoss72h = this;
    while (dalaDungeonBoss72h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss72h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss72h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss72h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "CS2_062")
        yield return (object) dalaDungeonBoss72h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_72h.m_BossHellfire);
    }
  }
}
