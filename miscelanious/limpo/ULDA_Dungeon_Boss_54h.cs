using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_54h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_BossLackey_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_BossLackey_01.prefab:05f62193a9aad884088fdb5924491605");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_BossTriggerHenchClanHogsteed_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_BossTriggerHenchClanHogsteed_01.prefab:d7a1489340d054e40b25873b8693a2a3");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_BossTriggerSunstruckHenchman_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_BossTriggerSunstruckHenchman_01.prefab:da0a9785deaf7664d9cfc855da1965da");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_Death_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_Death_01.prefab:6449ae72adcdba545be6a15582f71d51");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_DefeatPlayer_01.prefab:071b68bbe6c05034e9ba3e5cd6699c4f");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_EmoteResponse_01.prefab:b1cc725f42940474ab28afbde2f94442");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_01.prefab:a13be679590dbb4438d5fdf15fa7f399");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_02.prefab:5c9c0bad4fcf4ac458544755791f3ff2");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_03.prefab:7c2efe5ff6df8ee45a5e8de3b28072d5");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_04.prefab:80f19bff38adc1d44b0d5346bc9dea51");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_05.prefab:327381c0ac46c204884526b2cdb4b15b");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_Idle_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_Idle_01.prefab:5bbd069bf7f94d64e8e314b772c78a3e");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_Idle_02 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_Idle_02.prefab:513a143c16b1621479376d7675282347");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_Idle_03 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_Idle_03.prefab:836ab9ea69c499c4d9cfe47bc2e168ee");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_Intro_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_Intro_01.prefab:93c4480526367f841ba9c3ff78dac470");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_IntroSpecialElise_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_IntroSpecialElise_01.prefab:cb6b9e08404a3f14eaa557b9dd66e539");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_IntroSpecialFinley_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_IntroSpecialFinley_01.prefab:da4f6b2e4d54dbc4994b883bd431e417");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_BEEEES_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_BEEEES_01.prefab:55d6654af49ecf44f9d50859623d098b");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_Blatant_Decoy_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_Blatant_Decoy_01.prefab:b723694ed90bca645bbd037454fdbff6");
  private static readonly AssetReference VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_Frightened_Flunky_01 = new AssetReference("VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_Frightened_Flunky_01.prefab:6f7871a9b2658a6419172dba05b25df3");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_01,
    (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_02,
    (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_03,
    (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_04,
    (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Idle_01,
    (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Idle_02,
    (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_BossLackey_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_BossTriggerHenchClanHogsteed_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_BossTriggerSunstruckHenchman_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Death_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_02,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_03,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_04,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_HeroPower_05,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Idle_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Idle_02,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Idle_03,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Intro_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_IntroSpecialElise_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_IntroSpecialFinley_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_BEEEES_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_Blatant_Decoy_01,
      (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_Frightened_Flunky_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Finley")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_Intro_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "ULDA_Elise")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_IntroSpecialFinley_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_54h uldaDungeonBoss54h = this;
    while (uldaDungeonBoss54h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss54h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_54h uldaDungeonBoss54h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss54h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss54h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss54h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss54h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss54h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_706"))
      {
        if (!(cardId == "ULD_195"))
        {
          if (cardId == "ULD_134")
            yield return (object) uldaDungeonBoss54h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_BEEEES_01);
        }
        else
          yield return (object) uldaDungeonBoss54h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_Frightened_Flunky_01);
      }
      else
        yield return (object) uldaDungeonBoss54h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_PlayerTrigger_Blatant_Decoy_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_54h uldaDungeonBoss54h = this;
    while (uldaDungeonBoss54h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss54h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss54h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss54h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss54h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "DAL_413":
        case "DAL_613":
        case "DAL_614":
        case "DAL_615":
        case "DAL_739":
        case "DAL_741":
        case "ULD_616":
          yield return (object) uldaDungeonBoss54h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_BossLackey_01);
          break;
        case "DAL_743":
          yield return (object) uldaDungeonBoss54h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_BossTriggerHenchClanHogsteed_01);
          break;
        case "ULD_180":
          yield return (object) uldaDungeonBoss54h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_54h.VO_ULDA_BOSS_54h_Female_Hozen_BossTriggerSunstruckHenchman_01);
          break;
      }
    }
  }
}
