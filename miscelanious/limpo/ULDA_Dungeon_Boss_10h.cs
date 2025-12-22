using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_10h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_BossAcademicEspionage_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_BossAcademicEspionage_01.prefab:3eebb80606f52284a86b3dce9d5ec5bf");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_BossCleverDisguise_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_BossCleverDisguise_01.prefab:f011e9085bae8184e9982faa617f6774");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_BossPilfer_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_BossPilfer_01.prefab:f59853bffd2f57144924e29b11e71f5a");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_DeathALT_01.prefab:39b91cc8dfd2f354f9b066e27c9f402d");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_DefeatPlayer_01.prefab:a8eff39bd9feb0947bd2fc78373b363e");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_EmoteResponse_01.prefab:602ab4a5c17f7044b864939949421e10");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_01.prefab:07f72ecf6ace34a4f9fef973ba1f6fc6");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_02.prefab:e0e7062cebe3fe9478a8c9ec7eee9c4a");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_03.prefab:47f442e454443214a8c2c99c4d845bde");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_04.prefab:04b6820f614bd894fa206681160ac409");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_05.prefab:8e5ef34df5b3e1245840396f17eac880");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_Idle_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_Idle_01.prefab:13dc47e2dd1943c4b8f65aa23979e17f");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_Idle_02 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_Idle_02.prefab:e7f1fc9d5aeded44289e7d3c1a050cfa");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_Idle_03 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_Idle_03.prefab:ede54ce4c96f67542a3ba31b0339468a");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_Intro_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_Intro_01.prefab:adbea0c8ff17d69488f53c0f11e65df0");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_IntroElise_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_IntroElise_01.prefab:2c723f8cea4717f4abf9143c628e989a");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_IntroFinley_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_IntroFinley_01.prefab:48f59995ac2eb24449c9762d3c03a37f");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_PlayerEVILRecruiter_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_PlayerEVILRecruiter_01.prefab:208faf9a40d42854c9dac0aa27f881f8");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_PlayerHyenaAlpha_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_PlayerHyenaAlpha_01.prefab:0ba0ae053c833754bb2ef3bd42d839d8");
  private static readonly AssetReference VO_ULDA_BOSS_10h_Male_Sethrak_PlayerSackofLamps_01 = new AssetReference("VO_ULDA_BOSS_10h_Male_Sethrak_PlayerSackofLamps_01.prefab:66f57f09cfd1e29488556480c921f283");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_01,
    (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_02,
    (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_03,
    (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_04,
    (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_Idle_01,
    (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_Idle_02,
    (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_BossAcademicEspionage_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_BossCleverDisguise_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_BossPilfer_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_DeathALT_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_02,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_03,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_04,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_HeroPower_05,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_Idle_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_Idle_02,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_Idle_03,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_Intro_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_IntroElise_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_IntroFinley_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_PlayerEVILRecruiter_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_PlayerHyenaAlpha_01,
      (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_PlayerSackofLamps_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_DeathALT_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Elise")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_IntroElise_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "ULDA_Finley")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_IntroFinley_01, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "ULDA_Reno"))
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
    ULDA_Dungeon_Boss_10h uldaDungeonBoss10h = this;
    while (uldaDungeonBoss10h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss10h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_10h uldaDungeonBoss10h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss10h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss10h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss10h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss10h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss10h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_162"))
      {
        if (!(cardId == "ULD_154"))
        {
          if (cardId == "ULDA_040")
            yield return (object) uldaDungeonBoss10h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_PlayerSackofLamps_01);
        }
        else
          yield return (object) uldaDungeonBoss10h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_PlayerHyenaAlpha_01);
      }
      else
        yield return (object) uldaDungeonBoss10h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_PlayerEVILRecruiter_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_10h uldaDungeonBoss10h = this;
    while (uldaDungeonBoss10h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss10h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss10h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss10h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss10h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BOT_087"))
      {
        if (!(cardId == "ULD_328"))
        {
          if (cardId == "EX1_182")
            yield return (object) uldaDungeonBoss10h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_BossPilfer_01);
        }
        else
          yield return (object) uldaDungeonBoss10h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_BossCleverDisguise_01);
      }
      else
        yield return (object) uldaDungeonBoss10h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_10h.VO_ULDA_BOSS_10h_Male_Sethrak_BossAcademicEspionage_01);
    }
  }
}
