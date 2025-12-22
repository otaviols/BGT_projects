using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_76h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerAttackPlayerFace_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerAttackPlayerFace_01.prefab:9b4c06729e6039c4780c734741b37d74");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerDuel_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerDuel_01.prefab:3bf7905f1cbcd4141b8f1e59da315c15");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerPharoahsBlessing_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerPharoahsBlessing_01.prefab:fa86f12201d231046833af91c1860bd4");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerSubdue_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerSubdue_01.prefab:8de52c1facb71234a9e3b3ee08a846ae");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerTruesilverChampion_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerTruesilverChampion_01.prefab:dd7819bb7c633554d9215357c51c01ed");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_Death_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_Death_01.prefab:a3f1d2d0d18732147b57a6de8bd7f49c");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_DefeatPlayer_01.prefab:62c754f4349233140bb7ce0b59a35645");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_EmoteResponse_01.prefab:df4d2803cef85a94eb991c35fbdab650");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_01.prefab:c7ab1d95a90160546b01bce377f11cd1");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_02.prefab:47f6e36096b69e44a9a0b9dd98d83cee");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_03.prefab:a036c2e85ae2fa54ea8df93102fe8578");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_05.prefab:f9aa8eb50f2f46542a91932685b20e49");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_01.prefab:41ef35d280ac8db419c8d0a5840a111c");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_02 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_02.prefab:05e5998d4dfa6ad40af674e41707574a");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_03 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_03.prefab:3d3ee22babf316c4999a4b644a4f6258");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_Intro_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_Intro_01.prefab:a30c11b19fd170f4e82d06fcd4cd7343");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroBrann_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroBrann_01.prefab:756d9a9b80d8c91448de51e1b67c3cf4");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroEliseFirst_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroEliseFirst_01.prefab:7d486b9e0447d0e44a25fe90bac28290");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroFinley_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroFinley_01.prefab:9397da5a8dd81e04ebf9283da03f7c63");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroReno_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroReno_01.prefab:e0c8919a5ab02814299c0e13f0604394");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTrigger_Phalanx_Commander_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTrigger_Phalanx_Commander_01.prefab:a170e3c8678052540a514672a6006ccb");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTrigger_Pressure_Plate_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTrigger_Pressure_Plate_01.prefab:a52a618cd4899e446858892f4aea6510");
  private static readonly AssetReference VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTriggerBrawl_01 = new AssetReference("VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTriggerBrawl_01.prefab:99471727957a2d04689c26ce214916a6");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_01,
    (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_02,
    (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_03,
    (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_01,
    (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_02,
    (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerAttackPlayerFace_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerDuel_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerPharoahsBlessing_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerSubdue_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerTruesilverChampion_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_Death_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_02,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_03,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_HeroPower_05,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_02,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_Idle_03,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_Intro_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroBrann_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroEliseFirst_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroFinley_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroReno_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTrigger_Phalanx_Commander_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTrigger_Pressure_Plate_01,
      (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTriggerBrawl_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Elise")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_IntroEliseFirst_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_76h uldaDungeonBoss76h = this;
    while (uldaDungeonBoss76h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) uldaDungeonBoss76h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerAttackPlayerFace_01);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss76h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_76h uldaDungeonBoss76h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss76h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss76h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss76h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss76h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss76h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_179"))
      {
        if (!(cardId == "ULD_152"))
        {
          if (cardId == "EX1_407")
            yield return (object) uldaDungeonBoss76h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTriggerBrawl_01);
        }
        else
          yield return (object) uldaDungeonBoss76h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTrigger_Pressure_Plate_01);
      }
      else
        yield return (object) uldaDungeonBoss76h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_PlayerTrigger_Phalanx_Commander_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_76h uldaDungeonBoss76h = this;
    while (uldaDungeonBoss76h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss76h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss76h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss76h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss76h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "DAL_731"))
      {
        if (!(cardId == "ULD_143"))
        {
          if (!(cardId == "ULD_728"))
          {
            if (cardId == "CS2_097")
              yield return (object) uldaDungeonBoss76h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerTruesilverChampion_01);
          }
          else
            yield return (object) uldaDungeonBoss76h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerSubdue_01);
        }
        else
          yield return (object) uldaDungeonBoss76h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerPharoahsBlessing_01);
      }
      else
        yield return (object) uldaDungeonBoss76h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_76h.VO_ULDA_BOSS_76h_Male_NefersetTolvir_BossTriggerDuel_01);
    }
  }
}
