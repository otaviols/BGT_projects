using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_69h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_BossCounterSpellTrigger_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_BossCounterSpellTrigger_01.prefab:bb2ebbe45205ce449bc9b823a559cb26");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_BossMirrorImage_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_BossMirrorImage_01.prefab:5d7f7e4a7ed524143b23a2de6c5a757a");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_BossTwinSpellCast_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_BossTwinSpellCast_01.prefab:5da20278e94be934ebc5cd77f1174eb7");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_Death_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_Death_01.prefab:05887be8c4fc6ab49b8533df44cda76e");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_DefeatPlayer_01.prefab:f14e2ae9608c6a94e9817f6c199284fc");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_EmoteResponse_01.prefab:2d781b3988de8b94a92147fcc1fb3a8f");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_01.prefab:c5e70422b9eded649b9d677743ad1af0");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_02.prefab:51380cf01ebd89b4e9e5825595e5e974");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_03.prefab:591bdf714d4c5a14da6f9240a9e1616a");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_04.prefab:d0ed7f49016f5f34586e060e4f6a1637");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_05.prefab:7e41020cd8dd48648933eecdd2935404");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_Idle_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_Idle_01.prefab:a45067e046c88774490809c0aa16d6e8");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_Idle_02 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_Idle_02.prefab:10691d5a7a0a5a84ab84d14c7694c87c");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_Intro_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_Intro_01.prefab:4a6e76347d22ec04d91056b1b1805ecf");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_IntroReno_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_IntroReno_01.prefab:34603d9007f39e746ab2f23cfb4070d9");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerMalygos_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerMalygos_01.prefab:fdb4640b034264c4a85df389492aca56");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerTitanDiscTreasure_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerTitanDiscTreasure_01.prefab:4b1e43b602234a44ba367a0ee9e28416");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerTriggerPyroblast_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerTriggerPyroblast_01.prefab:d2e5b23edbe326744a607c09c3dbe57d");
  private static readonly AssetReference VO_ULDA_BOSS_69h_Female_TitanConstruct_TurnOne_01 = new AssetReference("VO_ULDA_BOSS_69h_Female_TitanConstruct_TurnOne_01.prefab:adc2cbbca322bcc4e8e3d21adae56b9c");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_01,
    (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_02,
    (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_03,
    (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_04,
    (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_Idle_01,
    (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_Idle_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_BossCounterSpellTrigger_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_BossMirrorImage_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_BossTwinSpellCast_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_Death_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_02,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_03,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_04,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_HeroPower_05,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_Idle_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_Idle_02,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_Intro_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_IntroReno_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerMalygos_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerTitanDiscTreasure_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerTriggerPyroblast_01,
      (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_TurnOne_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Reno")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_IntroReno_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_69h uldaDungeonBoss69h = this;
    while (uldaDungeonBoss69h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss69h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss69h.m_HeroPowerLines);
        break;
      case 102:
        yield return (object) uldaDungeonBoss69h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_BossCounterSpellTrigger_01);
        break;
      case 103:
        yield return (object) uldaDungeonBoss69h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_BossTwinSpellCast_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss69h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_69h uldaDungeonBoss69h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss69h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss69h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss69h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss69h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss69h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_563"))
      {
        if (!(cardId == "ULDA_403"))
        {
          if (cardId == "EX1_279")
            yield return (object) uldaDungeonBoss69h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerTriggerPyroblast_01);
        }
        else
          yield return (object) uldaDungeonBoss69h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerTitanDiscTreasure_01);
      }
      else
        yield return (object) uldaDungeonBoss69h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_PlayerMalygos_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_69h uldaDungeonBoss69h = this;
    while (uldaDungeonBoss69h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss69h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss69h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss69h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss69h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "CS2_027")
        yield return (object) uldaDungeonBoss69h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_69h.VO_ULDA_BOSS_69h_Female_TitanConstruct_BossMirrorImage_01);
    }
  }
}
