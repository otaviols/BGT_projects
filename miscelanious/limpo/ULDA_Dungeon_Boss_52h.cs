using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_52h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerFreezePlayer_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerFreezePlayer_01.prefab:159103d42df8b3b489c0e40cebbca7fc");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerFrostNova_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerFrostNova_01.prefab:fbc9363a6cf9a3c46b15dd45d3874fa5");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerRayofFrost_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerRayofFrost_01.prefab:eb728c7f61b17ad44a66b145fbf4fc1b");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerWaterElemental_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerWaterElemental_01.prefab:1bd3a2230ec022841970069845dab07e");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_Death_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_Death_01.prefab:de1fa7d300be606439a9668b59a167a8");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_DefeatPlayer_01.prefab:43e2f24e3185162459d0d113059f263b");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_EmoteResponse_01.prefab:588679b16be5a654b8ea21ffb5d8b360");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_01.prefab:e79b5719260d06a46a0a3c0e2552304c");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_03.prefab:c7d1cc7d5df059f46813b1c0c1f1b28d");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_05.prefab:8e1ae81702419cc498e8225e65bea36d");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_01.prefab:9a22082301cb4a04abeaa9f5b902202e");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_02 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_02.prefab:4486e499b77779e459d882ec683d7908");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_03 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_03.prefab:af72c89b9d5ae21429ec804375d0cefc");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_Intro_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_Intro_01.prefab:04ec01ffc764867469bc10aa1e86035e");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_IntroResponseReno_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_IntroResponseReno_01.prefab:a5da3c562d4e1e04b85298332f33834e");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerFreezeBoss_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerFreezeBoss_01.prefab:be198bfd9ec5b38438b0d22f642441d7");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Arfus_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Arfus_01.prefab:bb36b791d9954b8479ca6482a2273c07");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Jar_Dealer_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Jar_Dealer_01.prefab:5690869bd409deb4cb006b55ef7dd03b");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Kelthuzad_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Kelthuzad_01.prefab:c6515a9d41705494891a103ab2a27bed");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_LichKing_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_LichKing_01.prefab:34191563ddc4a664982a1e96e4357c44");
  private static readonly AssetReference VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Psychopomp_01 = new AssetReference("VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Psychopomp_01.prefab:2a6782f37af2f3444a2b81f9e43e697c");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_01,
    (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_03,
    (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_01,
    (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_02,
    (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerFreezePlayer_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerFrostNova_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerRayofFrost_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerWaterElemental_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_Death_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_03,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_HeroPower_05,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_02,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_Idle_03,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_Intro_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_IntroResponseReno_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerFreezeBoss_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Arfus_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Jar_Dealer_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Kelthuzad_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_LichKing_01,
      (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Psychopomp_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START && cardId != "ULDA_Reno")
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
    ULDA_Dungeon_Boss_52h uldaDungeonBoss52h = this;
    while (uldaDungeonBoss52h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss52h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerFreezeBoss_01);
        break;
      case 102:
        yield return (object) uldaDungeonBoss52h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerFreezePlayer_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss52h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_52h uldaDungeonBoss52h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss52h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss52h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss52h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss52h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss52h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ICC_854"))
      {
        if (!(cardId == "ULD_282"))
        {
          if (!(cardId == "FP1_013"))
          {
            if (!(cardId == "ICC_314"))
            {
              if (cardId == "ULD_268")
                yield return (object) uldaDungeonBoss52h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Psychopomp_01);
            }
            else
              yield return (object) uldaDungeonBoss52h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_LichKing_01);
          }
          else
            yield return (object) uldaDungeonBoss52h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Kelthuzad_01);
        }
        else
          yield return (object) uldaDungeonBoss52h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Jar_Dealer_01);
      }
      else
        yield return (object) uldaDungeonBoss52h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_PlayerTrigger_Arfus_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_52h uldaDungeonBoss52h = this;
    while (uldaDungeonBoss52h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss52h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss52h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss52h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss52h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CS2_026"))
      {
        if (!(cardId == "DAL_577"))
        {
          if (cardId == "CS2_033")
            yield return (object) uldaDungeonBoss52h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerWaterElemental_01);
        }
        else
          yield return (object) uldaDungeonBoss52h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerRayofFrost_01);
      }
      else
        yield return (object) uldaDungeonBoss52h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_52h.VO_ULDA_BOSS_52h_Female_UndeadLich_BossTriggerFrostNova_01);
    }
  }
}
