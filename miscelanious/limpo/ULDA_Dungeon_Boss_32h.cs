using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_32h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerDreamwayGuardians_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerDreamwayGuardians_01.prefab:5922f722b051c974dbe83c4e2318fcaf");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerGroveTender_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerGroveTender_01.prefab:dde8eae8cc992f24291f3373f7b88e9b");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerKeeperoftheGrove_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerKeeperoftheGrove_01.prefab:43e418c2f425ef74499558a3845ab164");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_Death_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_Death_01.prefab:adea5d33b8b88c946908a82b4eee3828");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_DefeatPlayer_01.prefab:bd6b96434b051e84bb3fbc494f711e37");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_EmoteResponse_01.prefab:71c60703ddf073b4d86bf76cd421646e");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_01.prefab:c652311aff11b7c46bc19cfde1e6511b");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_02.prefab:6f83fa87c2be53e42bc571e0bc874c6f");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_04.prefab:769154c7b56df3843ba6d1c2f6473f7e");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_05.prefab:50e69815bb85ef34897b69bf54fc7ba5");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_Idle_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_Idle_01.prefab:38dc9231669ec0e49bccedd3fca5748c");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_Idle_02 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_Idle_02.prefab:3d9fe1b16565e4e40a07e9be9f4204ec");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_Idle_03 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_Idle_03.prefab:c6614a4bb3105fd419229d1694b6d00d");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_Intro_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_Intro_01.prefab:7d3501d83239e3c45a51225c2b0ae57b");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_IntroResponseElise_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_IntroResponseElise_01.prefab:cc98403e13cb68b4c89206207c2cc830");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_IntroSpecial_Finley_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_IntroSpecial_Finley_01.prefab:1febfdf793f41ef44b03c3ce3dbe7e22");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_PlayerTrigger_Desert_Hare_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_PlayerTrigger_Desert_Hare_01.prefab:0ce977fda7ed0dc479176ed20f3308d4");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_PlayerTrigger_Pit_Crocolisk_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_PlayerTrigger_Pit_Crocolisk_01.prefab:8defb42f7b18b0347ba99ba6ecc8170e");
  private static readonly AssetReference VO_ULDA_BOSS_32h_Female_Dryad_TurnOne_01 = new AssetReference("VO_ULDA_BOSS_32h_Female_Dryad_TurnOne_01.prefab:f5d63c5adb2131946b2d033ed5b0109c");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_01,
    (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_02,
    (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_04,
    (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_Idle_01,
    (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_Idle_02,
    (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerDreamwayGuardians_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerGroveTender_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerKeeperoftheGrove_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_Death_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_02,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_04,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_HeroPower_05,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_Idle_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_Idle_02,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_Idle_03,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_Intro_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_IntroResponseElise_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_IntroSpecial_Finley_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_PlayerTrigger_Desert_Hare_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_PlayerTrigger_Pit_Crocolisk_01,
      (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_TurnOne_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Finley")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_IntroSpecial_Finley_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "ULDA_Elise")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_IntroResponseElise_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_32h uldaDungeonBoss32h = this;
    while (uldaDungeonBoss32h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) uldaDungeonBoss32h.PlayBossLine(actor, (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_TurnOne_01);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss32h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_32h uldaDungeonBoss32h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss32h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss32h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss32h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss32h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss32h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_190"))
      {
        if (cardId == "ULD_719")
          yield return (object) uldaDungeonBoss32h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_PlayerTrigger_Desert_Hare_01);
      }
      else
        yield return (object) uldaDungeonBoss32h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_PlayerTrigger_Pit_Crocolisk_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_32h uldaDungeonBoss32h = this;
    while (uldaDungeonBoss32h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss32h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss32h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss32h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss32h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "DAL_733"))
      {
        if (!(cardId == "GVG_032"))
        {
          if (cardId == "EX1_166")
            yield return (object) uldaDungeonBoss32h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerKeeperoftheGrove_01);
        }
        else
          yield return (object) uldaDungeonBoss32h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerGroveTender_01);
      }
      else
        yield return (object) uldaDungeonBoss32h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_32h.VO_ULDA_BOSS_32h_Female_Dryad_BossTriggerDreamwayGuardians_01);
    }
  }
}
