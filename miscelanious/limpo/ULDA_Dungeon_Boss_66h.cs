using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_66h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerBenevolentDjinnLowHealth_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerBenevolentDjinnLowHealth_01.prefab:341aacf7352bf024188831e4ab716f45");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerLightningSpell_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerLightningSpell_01.prefab:b7b7df2179a59b3418e88ba4e11acdb3");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerPolymorph_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerPolymorph_01.prefab:af97b4b2bc6b77f4eb3980fbac310e58");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_DeathALT_01.prefab:64a68f83742b9634bac6edc7854dbc6e");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_DefeatPlayer_01.prefab:3fbc7019ec1d7c843b5403a727edfd13");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_EmoteResponse_01.prefab:ac9e6e3eea022b44b902efd6d70ae1a8");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_02.prefab:fe361a03a2671384c9ebe4725f49ebca");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_03.prefab:7ff78a906890abc4d8cbf4ae191b2831");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_04.prefab:320c88ba420ade145a3be54220f9a62d");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_05.prefab:2767250aaae90114487ef3f34a240431");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_Idle1_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_Idle1_01.prefab:e6a8452fb2ca1fa4692bdeade843a628");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_Idle2_02 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_Idle2_02.prefab:fcded398091442c4ea930bc1bc016792");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_Idle3_03 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_Idle3_03.prefab:818c1915a901a5f43b5ffbe479f52757");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_Idle4_04 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_Idle4_04.prefab:23826da75864bb14c88332332926e3b0");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_Intro_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_Intro_01.prefab:3ba6f3d90baae2f4eb5a270f3414a1e8");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_IntroRenoResponse_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_IntroRenoResponse_01.prefab:0e3aa5202fdd39c44b168590f8813f6d");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_PlayerSiamat_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_PlayerSiamat_01.prefab:ecf4efdb0c1d6504da43ee67eaee854d");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_PlayerWishTreasure_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_PlayerWishTreasure_01.prefab:8c507084f7c11ef43969f0688e07bb05");
  private static readonly AssetReference VO_ULDA_BOSS_66h_Male_Djinn_PlayerZephrys_01 = new AssetReference("VO_ULDA_BOSS_66h_Male_Djinn_PlayerZephrys_01.prefab:685670079405c9347bc2e10681759c03");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_02,
    (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_03,
    (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_04,
    (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_05
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerBenevolentDjinnLowHealth_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerLightningSpell_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerPolymorph_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_DeathALT_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_02,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_03,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_04,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_HeroPower_05,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_Idle1_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_Idle2_02,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_Idle3_03,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_Idle4_04,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_Intro_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_IntroRenoResponse_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_PlayerSiamat_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_PlayerWishTreasure_01,
      (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_PlayerZephrys_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_DeathALT_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Reno")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_IntroRenoResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_66h uldaDungeonBoss66h = this;
    while (uldaDungeonBoss66h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss66h.PlayBossLine(actor, (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_Idle1_01);
        break;
      case 102:
        yield return (object) uldaDungeonBoss66h.PlayBossLine(actor, (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_Idle2_02);
        break;
      case 103:
        yield return (object) uldaDungeonBoss66h.PlayBossLine(actor, (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_Idle3_03);
        break;
      case 104:
        yield return (object) uldaDungeonBoss66h.PlayBossLine(actor, (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_Idle4_04);
        break;
      case 105:
        yield return (object) uldaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerBenevolentDjinnLowHealth_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss66h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_66h uldaDungeonBoss66h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss66h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss66h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss66h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss66h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss66h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_178"))
      {
        if (!(cardId == "LOOTA_814"))
        {
          if (cardId == "ULD_003")
            yield return (object) uldaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_PlayerZephrys_01);
        }
        else
          yield return (object) uldaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_PlayerWishTreasure_01);
      }
      else
        yield return (object) uldaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_PlayerSiamat_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_66h uldaDungeonBoss66h = this;
    while (uldaDungeonBoss66h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss66h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss66h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss66h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss66h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_251") && !(cardId == "CFM_707") && !(cardId == "EX1_238") && !(cardId == "EX1_259") && !(cardId == "OG_206"))
      {
        if (cardId == "CS2_022")
          yield return (object) uldaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerPolymorph_01);
      }
      else
        yield return (object) uldaDungeonBoss66h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_66h.VO_ULDA_BOSS_66h_Male_Djinn_BossTriggerLightningSpell_01);
    }
  }
}
