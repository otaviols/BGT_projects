using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_36h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_BossArcaniteReaper_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_BossArcaniteReaper_01.prefab:7fa33d2ac741de44389bf1fb3fde4f46");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_BossPlagueofMadness_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_BossPlagueofMadness_01.prefab:af9893496aab0494b89e03d3dd97e8fb");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_BossWhirlwind_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_BossWhirlwind_01.prefab:bc9564972c7c56b4a87bdf04f0cc09a2");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_DeathALT_01.prefab:37dcc5cdaef41d14ea2e6a7ab2ab7b23");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_DefeatPlayer_01.prefab:c2f2811751473b44a8d658e52aaa8a99");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_EmoteResponse_01.prefab:a82d1d8269abcf64289e3397ee9b3fe4");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_01.prefab:3a9ebdb7977b9534898c3b91be5ba31a");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_02.prefab:c7a055caeb6454f49b9255996de9764e");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_03.prefab:ad8d27dcc440d2c4dba72fed6ebd0065");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_04.prefab:3cc2612b677eca746aaa2c774d57cd8a");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_05.prefab:0081e247d139daf419475f2297606ee9");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_Idle_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_Idle_01.prefab:cf9d73226a4ad8149b63f800f1587fc2");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_Idle_02 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_Idle_02.prefab:2948f69aa2a26a14ca7f89aac46676b2");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_Idle_03 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_Idle_03.prefab:b2c60d4cc959dec4fb022c8c9c16c8b9");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_Intro_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_Intro_01.prefab:1c1e7a9af96c7b445a4ad9baf31afe58");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_IntroFinley_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_IntroFinley_01.prefab:2af5a139e6863034e87f5d457358cd35");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_PlayerPlagueofMadness_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_PlayerPlagueofMadness_01.prefab:b55a4dc0d7e356e48b95995d97abee6a");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_PlayerSurrendertoMadness_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_PlayerSurrendertoMadness_01.prefab:a2f5e0bc65718e848a3bb1b71ef0eb19");
  private static readonly AssetReference VO_ULDA_BOSS_36h_Female_Tauren_PlayerWeapon_01 = new AssetReference("VO_ULDA_BOSS_36h_Female_Tauren_PlayerWeapon_01.prefab:6b975740d84034846bf7977f0f6efc5d");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_01,
    (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_02,
    (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_03,
    (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_04,
    (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_Idle_01,
    (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_Idle_02,
    (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_BossArcaniteReaper_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_BossPlagueofMadness_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_BossWhirlwind_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_DeathALT_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_02,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_03,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_04,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_HeroPower_05,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_Idle_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_Idle_02,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_Idle_03,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_Intro_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_IntroFinley_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_PlayerPlagueofMadness_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_PlayerSurrendertoMadness_01,
      (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_PlayerWeapon_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_DeathALT_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Finley")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_IntroFinley_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_36h uldaDungeonBoss36h = this;
    while (uldaDungeonBoss36h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) uldaDungeonBoss36h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_PlayerWeapon_01);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss36h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_36h uldaDungeonBoss36h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss36h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss36h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss36h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss36h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss36h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_715"))
      {
        if (cardId == "TRL_500")
          yield return (object) uldaDungeonBoss36h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_PlayerSurrendertoMadness_01);
      }
      else
        yield return (object) uldaDungeonBoss36h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_PlayerPlagueofMadness_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_36h uldaDungeonBoss36h = this;
    while (uldaDungeonBoss36h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss36h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss36h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss36h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss36h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CS2_112"))
      {
        if (!(cardId == "ULD_715"))
        {
          if (cardId == "EX1_400")
            yield return (object) uldaDungeonBoss36h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_BossWhirlwind_01);
        }
        else
          yield return (object) uldaDungeonBoss36h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_BossPlagueofMadness_01);
      }
      else
        yield return (object) uldaDungeonBoss36h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_36h.VO_ULDA_BOSS_36h_Female_Tauren_BossArcaniteReaper_01);
    }
  }
}
