using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_40h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_BossHellfire_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_BossHellfire_01.prefab:eab07753d5b20ff4f88c59fa65ecaded");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_BossSoulfire_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_BossSoulfire_01.prefab:cd9ef1ba1cc9da94d864036103510664");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_Death_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_Death_01.prefab:ee93c75aa8c32f240be03d296981597b");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_DefeatPlayer_01.prefab:5b3f52e8fed509b4c99e376c259363ea");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_EmoteResponse_01.prefab:c037e2bea37831e48a0e5c784f106d60");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_01.prefab:7f99351632f125342b882f6b84f62ce3");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_02 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_02.prefab:92a2d289e65e24f4e83991381f7c90d2");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_03 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_03.prefab:62c2b85fb9a7a7b4796da98365788b8b");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_04 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_04.prefab:7107e6ba92469a248b568b899e18560b");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_05 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_05.prefab:7629c3883ea0ead4dab7e7c8ce5a4469");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_07 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_07.prefab:b8f4f41f80a949d4980866798a3fbb42");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_Idle_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_Idle_01.prefab:8c36b680bf54d8e46bc6151805f40dbf");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_Idle_02 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_Idle_02.prefab:8fff03a9803120d49a3a1e04ded78b80");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_Idle_03 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_Idle_03.prefab:736ec88177e65c5419c1e63d5f7a1677");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_Intro_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_Intro_01.prefab:6c1dea20918bacd4ea534f58b39bbb0b");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_IntroGeorge_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_IntroGeorge_01.prefab:0b2620bb77ce857438c21fab7a946ad6");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_IntroTekahn_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_IntroTekahn_01.prefab:da25ce0d10163834a9249d899d12dd27");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_PlayerDemon_01 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_PlayerDemon_01.prefab:85e969203fa0fa5439fd5dad5ff2c343");
  private static readonly AssetReference VO_DALA_BOSS_40h_Female_NightElf_PlayerDemon_02 = new AssetReference("VO_DALA_BOSS_40h_Female_NightElf_PlayerDemon_02.prefab:a5e4d15fe4ca80747856ef1926bc45d6");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_Idle_01,
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_Idle_02,
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_Idle_03
  };
  private static List<string> m_HeroPowerTrigger = new List<string>()
  {
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_01,
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_02,
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_03,
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_04,
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_05,
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_07
  };
  private static List<string> m_PlayerDemon = new List<string>()
  {
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_PlayerDemon_01,
    (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_PlayerDemon_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_BossHellfire_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_BossSoulfire_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_Death_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_02,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_03,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_04,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_05,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_HeroPowerTrigger_07,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_Idle_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_Idle_02,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_Idle_03,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_Intro_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_IntroGeorge_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_IntroTekahn_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_PlayerDemon_01,
      (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_PlayerDemon_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_40h.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "DALA_George")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_IntroGeorge_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "DALA_Tekahn")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_IntroTekahn_01, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "DALA_Rakanishu"))
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

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    DALA_Dungeon_Boss_40h dalaDungeonBoss40h = this;
    while (dalaDungeonBoss40h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) dalaDungeonBoss40h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_40h.m_HeroPowerTrigger);
        break;
      case 102:
        yield return (object) dalaDungeonBoss40h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_40h.m_PlayerDemon);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) dalaDungeonBoss40h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_40h dalaDungeonBoss40h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss40h.\u003C\u003En__1(entity);
    while (dalaDungeonBoss40h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss40h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss40h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss40h.m_playedLines.Add(cardId);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_40h dalaDungeonBoss40h = this;
    while (dalaDungeonBoss40h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss40h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) dalaDungeonBoss40h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss40h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CS2_062"))
      {
        if (cardId == "EX1_308")
          yield return (object) dalaDungeonBoss40h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_BossSoulfire_01);
      }
      else
        yield return (object) dalaDungeonBoss40h.PlayLineOnlyOnce(actor, (string) DALA_Dungeon_Boss_40h.VO_DALA_BOSS_40h_Female_NightElf_BossHellfire_01);
    }
  }
}
