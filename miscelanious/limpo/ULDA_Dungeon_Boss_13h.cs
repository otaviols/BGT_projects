using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_13h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_BossHyenaAlpha_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_BossHyenaAlpha_01.prefab:fc14d47dc8353244d93b225bd0389c68");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_BossMarkedShot_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_BossMarkedShot_01.prefab:29f876551907be54a9db5b338b6c7bd2");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_BossSnakeTrapTrigger_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_BossSnakeTrapTrigger_01.prefab:ab431ecfefb581b4b89797fde7a7661c");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_Death_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_Death_01.prefab:bcad9230fd3554c408c6d5bdbbc8d2e9");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_DefeatPlayer_01.prefab:06ad541e8ea9f3048bf5edf41bc654c9");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_EmoteResponse_01.prefab:9a15c02ed8506bd42974df75bde1e269");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_01.prefab:b64af718a490c34448d870288a2c89d0");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_02.prefab:0abadc0695b27f74ea260c27ba85a77a");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_04.prefab:c3aac27f4d0a7124db98697baaf3a284");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_05.prefab:8badb20ec4bdc344488e0cb2f15621ab");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_Idle_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_Idle_01.prefab:1958c5c47397efc4bacecdb7a69cb021");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_Idle_02 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_Idle_02.prefab:868cda1fa725f9a429554cdfdf937c6e");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_Idle_03 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_Idle_03.prefab:ad7ff5e31b93cc34bb27cdf541147cba");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_Intro_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_Intro_01.prefab:d637f53e47dd1ca4f8252cf959248287");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_IntroBrannResponse_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_IntroBrannResponse_01.prefab:b0e46475ae10b1547ba3bfb3d2756cbf");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_PlayerBaku_GiantAnaconda_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_PlayerBaku_GiantAnaconda_01.prefab:f55b0076b4967ea419ad8781253b73ca");
  private static readonly AssetReference VO_ULDA_BOSS_13h_Male_Gnoll_PlayerSnakeTrap_01 = new AssetReference("VO_ULDA_BOSS_13h_Male_Gnoll_PlayerSnakeTrap_01.prefab:97f0e105405f8a04ea032721288798f1");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_01,
    (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_02,
    (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_04,
    (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_Idle_01,
    (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_Idle_02,
    (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_BossHyenaAlpha_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_BossMarkedShot_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_BossSnakeTrapTrigger_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_Death_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_02,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_04,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_HeroPower_05,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_Idle_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_Idle_02,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_Idle_03,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_Intro_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_IntroBrannResponse_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_PlayerBaku_GiantAnaconda_01,
      (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_PlayerSnakeTrap_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Brann")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_IntroBrannResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_13h uldaDungeonBoss13h = this;
    while (uldaDungeonBoss13h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) uldaDungeonBoss13h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_BossSnakeTrapTrigger_01);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss13h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_13h uldaDungeonBoss13h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss13h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss13h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss13h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss13h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss13h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_154"))
      {
        if (!(cardId == "DAL_371"))
        {
          if (!(cardId == "GIL_826") && !(cardId == "UNG_086"))
          {
            if (cardId == "EX1_554")
              yield return (object) uldaDungeonBoss13h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_PlayerSnakeTrap_01);
          }
          else
            yield return (object) uldaDungeonBoss13h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_PlayerBaku_GiantAnaconda_01);
        }
        else
          yield return (object) uldaDungeonBoss13h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_BossMarkedShot_01);
      }
      else
        yield return (object) uldaDungeonBoss13h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_13h.VO_ULDA_BOSS_13h_Male_Gnoll_BossHyenaAlpha_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_13h uldaDungeonBoss13h = this;
    while (uldaDungeonBoss13h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss13h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss13h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss13h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss13h.m_playedLines.Add(cardId);
    }
  }
}
