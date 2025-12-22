using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_41h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_BossDireFrenzy_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_BossDireFrenzy_01.prefab:a2ba893031e9c3e49ad4fff8edba1c03");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_BossSecretPlan_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_BossSecretPlan_01.prefab:c10e4ee33b24c5d4a8d5edc29466cd83");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_BossWanderingMonsterTrigger_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_BossWanderingMonsterTrigger_01.prefab:e35367dbb10d9854e919d883c0e73b67");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_DeathALT_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_DeathALT_01.prefab:3c0c9f997c866b34d8cdf181e361cfff");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_DefeatPlayer_01.prefab:f5944adbbace90b45a8e0545e1335700");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_EmoteResponse_01.prefab:f294257c72044f641b993d1412b244fc");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_01.prefab:9c0eb31129fac98428dffcdaf65653e9");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_03.prefab:89758c4ccc1ff1246afff0ecb65c6edc");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_04.prefab:d591c3de2e664434ab20239febff7d80");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_HeroPowerBeast_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_HeroPowerBeast_01.prefab:9fa688bbcb242ef4abf456d3dd4a6dbb");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_Idle_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_Idle_01.prefab:3a3d36299883a4f4595f18210eda093e");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_Idle_02 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_Idle_02.prefab:f8d2b77dd01f2ce479d4acc5a08b76cf");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_Idle_03 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_Idle_03.prefab:84ff7f9128664534fb0775be58ff001f");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_Intro_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_Intro_01.prefab:9aa0b82ee212b25438804f67cc46b672");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_PlayerAldorPeacekeeper_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_PlayerAldorPeacekeeper_01.prefab:c2c7bcc0da5ffd14fa24c87699117877");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_PlayerBazaarMugger_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_PlayerBazaarMugger_01.prefab:9abc1886e3503ab49a82aefd9e762aad");
  private static readonly AssetReference VO_ULDA_BOSS_41h_Female_Worgen_TurnOne_01 = new AssetReference("VO_ULDA_BOSS_41h_Female_Worgen_TurnOne_01.prefab:230edc73c690b4b42a34ad4583fce1fb");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_01,
    (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_03,
    (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_04
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_Idle_01,
    (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_Idle_02,
    (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_BossDireFrenzy_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_BossSecretPlan_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_BossWanderingMonsterTrigger_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_DeathALT_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_03,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_HeroPower_04,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_HeroPowerBeast_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_Idle_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_Idle_02,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_Idle_03,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_Intro_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_PlayerAldorPeacekeeper_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_PlayerBazaarMugger_01,
      (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_TurnOne_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_DeathALT_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_EmoteResponse_01;
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
    ULDA_Dungeon_Boss_41h uldaDungeonBoss41h = this;
    while (uldaDungeonBoss41h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss41h.PlayBossLine(actor, (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_TurnOne_01);
        break;
      case 102:
        yield return (object) uldaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_HeroPowerBeast_01);
        break;
      case 103:
        yield return (object) uldaDungeonBoss41h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss41h.m_HeroPowerLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss41h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_41h uldaDungeonBoss41h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss41h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss41h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss41h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss41h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss41h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_382"))
      {
        if (cardId == "ULD_327")
          yield return (object) uldaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_PlayerBazaarMugger_01);
      }
      else
        yield return (object) uldaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_PlayerAldorPeacekeeper_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_41h uldaDungeonBoss41h = this;
    while (uldaDungeonBoss41h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss41h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss41h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss41h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss41h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "GIL_828"))
      {
        if (!(cardId == "BOT_402"))
        {
          if (cardId == "LOOT_079")
            yield return (object) uldaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_BossWanderingMonsterTrigger_01);
        }
        else
          yield return (object) uldaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_BossSecretPlan_01);
      }
      else
        yield return (object) uldaDungeonBoss41h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_41h.VO_ULDA_BOSS_41h_Female_Worgen_BossDireFrenzy_01);
    }
  }
}
