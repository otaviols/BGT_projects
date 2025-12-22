using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_22h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerColossusoftheMoon_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerColossusoftheMoon_01.prefab:9259ea2253654b141be338f1f6076bee");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerPlagueofWrath_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerPlagueofWrath_01.prefab:9dbe5fa83d15c5b45a7c96e8211ead4d");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerWhirlwind_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerWhirlwind_01.prefab:b820478293db40b4db21fb618a123425");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_Death_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_Death_01.prefab:68bb7ce0753c67147a4486322687cb54");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_DefeatPlayer_01.prefab:40d3f74e4297a5a428a0ff991098e2ee");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_EmoteResponse_01.prefab:0f31018a56fb287418c6facbcf56b70f");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_01.prefab:5c395f44e48c879408ca6166e55c1c2f");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_02.prefab:eb91a602cecc11648b8f52b968118af7");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_03.prefab:820107cdf7a9c82498fe7924df52a846");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_04.prefab:53254ced693d02142a3682e00fca15f4");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_Idle_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_Idle_01.prefab:b0c8522d14ca1ab479e52e465b87aa68");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_Idle_02 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_Idle_02.prefab:abdaaf5c642080e46a9f874af1964579");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_Idle_03 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_Idle_03.prefab:6db522e445aedc147abfb056c79e8aa1");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_Intro_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_Intro_01.prefab:f47f899252b20df4593256132352aff7");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_PlayerTriggerColossusoftheMoon_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_PlayerTriggerColossusoftheMoon_01.prefab:025ebb40085e43a4391e6795d4c47dec");
  private static readonly AssetReference VO_ULDA_BOSS_22h_Male_Colossus_PlayerTriggerDesertObelisk_01 = new AssetReference("VO_ULDA_BOSS_22h_Male_Colossus_PlayerTriggerDesertObelisk_01.prefab:3d59177a4432427478bf4c2a32f31193");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_01,
    (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_02,
    (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_03,
    (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_04
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_Idle_01,
    (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_Idle_02,
    (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerColossusoftheMoon_01,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerPlagueofWrath_01,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerWhirlwind_01,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_Death_01,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_01,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_02,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_03,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_HeroPower_04,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_Idle_01,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_Idle_02,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_Idle_03,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_Intro_01,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_PlayerTriggerColossusoftheMoon_01,
      (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_PlayerTriggerDesertObelisk_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
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
    ULDA_Dungeon_Boss_22h uldaDungeonBoss22h = this;
    while (uldaDungeonBoss22h.m_enemySpeaking)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss22h.\u003C\u003En__0(missionEvent);
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_22h uldaDungeonBoss22h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss22h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss22h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss22h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss22h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss22h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_721"))
      {
        if (cardId == "ULD_703")
          yield return (object) uldaDungeonBoss22h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_PlayerTriggerDesertObelisk_01);
      }
      else
        yield return (object) uldaDungeonBoss22h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_PlayerTriggerColossusoftheMoon_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_22h uldaDungeonBoss22h = this;
    while (uldaDungeonBoss22h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss22h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss22h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss22h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss22h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_721"))
      {
        if (!(cardId == "ULD_707"))
        {
          if (cardId == "DAL_742")
            yield return (object) uldaDungeonBoss22h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerWhirlwind_01);
        }
        else
          yield return (object) uldaDungeonBoss22h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerPlagueofWrath_01);
      }
      else
        yield return (object) uldaDungeonBoss22h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_22h.VO_ULDA_BOSS_22h_Male_Colossus_BossTriggerColossusoftheMoon_01);
    }
  }
}
