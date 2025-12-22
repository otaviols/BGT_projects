using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_07h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_BossAttackHero_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_BossAttackHero_01.prefab:a03258f014f319e4b8bd1a4fae9a4ea9");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_BossAttackMinionKill_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_BossAttackMinionKill_01.prefab:33b9167dd5a37aa4697f74823060060c");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_BossBEES_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_BossBEES_01.prefab:40db176871369ba4496b075af53cb2a7");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_BossBiteSwipe_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_BossBiteSwipe_01.prefab:383c445752f316f45b13f8fc6021a6b4");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_Death_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_Death_01.prefab:18e20bb4303325c4c9fd9ec696f48539");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_DefeatPlayer_01.prefab:231e2cd36d4a518458e793acabd56aee");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_EmoteResponse_01.prefab:5b25a7c56a0aa1c4d914a3494ab8af10");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_01.prefab:4cda19fa34a7d06449425b5253a1d98e");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_02.prefab:d755eb8db32782547bb0a2a74d3b57ba");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_03.prefab:ac4212552f4f91a46b070a8f64fc3fa2");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_04.prefab:eb7a5b3975ecc6f428efaac41e6d6eae");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_Idle_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_Idle_01.prefab:f791cf60a565f374783cb778490d3c15");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_Idle_02 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_Idle_02.prefab:cb88a77feccd7ad4ca3e3d73c855ade8");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_Idle_03 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_Idle_03.prefab:180628446bd285a4a952b14433feb90d");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_Intro_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_Intro_01.prefab:4382039cf465d1747adfc522fdfcb53e");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_IntroElise_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_IntroElise_01.prefab:7872ed7eeedeafe4ba3592411c7f9f68");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_PlayerAcornbearer_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_PlayerAcornbearer_01.prefab:f569727d8074eac48a5502eb82f0e13f");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_PlayerForestsAid_PlayerForceofNature_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_PlayerForestsAid_PlayerForceofNature_01.prefab:e1f23943c129e054d8e173a0a4ddb1ac");
  private static readonly AssetReference VO_ULDA_BOSS_07h_Female_Kobold_PlayerSpreadingPlague_01 = new AssetReference("VO_ULDA_BOSS_07h_Female_Kobold_PlayerSpreadingPlague_01.prefab:1bf691f99e4400d4d9881e1102b70346");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_01,
    (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_02,
    (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_03,
    (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_04
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_Idle_01,
    (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_Idle_02,
    (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_BossAttackHero_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_BossAttackMinionKill_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_BossBEES_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_BossBiteSwipe_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_Death_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_02,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_03,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_HeroPower_04,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_Idle_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_Idle_02,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_Idle_03,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_Intro_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_IntroElise_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_PlayerAcornbearer_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_PlayerForestsAid_PlayerForceofNature_01,
      (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_PlayerSpreadingPlague_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Elise")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_IntroElise_01, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "ULDA_Finley"))
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
    ULDA_Dungeon_Boss_07h uldaDungeonBoss07h = this;
    while (uldaDungeonBoss07h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss07h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_BossAttackHero_01);
        break;
      case 102:
        yield return (object) uldaDungeonBoss07h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_BossAttackMinionKill_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss07h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_07h uldaDungeonBoss07h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss07h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss07h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss07h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss07h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss07h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "DAL_354"))
      {
        if (!(cardId == "ICC_054"))
        {
          if (cardId == "EX1_571" || cardId == "DAL_256" || cardId == "DAL_256ts")
            yield return (object) uldaDungeonBoss07h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_PlayerForestsAid_PlayerForceofNature_01);
        }
        else
          yield return (object) uldaDungeonBoss07h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_PlayerSpreadingPlague_01);
      }
      else
        yield return (object) uldaDungeonBoss07h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_PlayerAcornbearer_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_07h uldaDungeonBoss07h = this;
    while (uldaDungeonBoss07h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss07h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss07h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss07h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss07h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_134"))
      {
        if (cardId == "CS2_012")
          yield return (object) uldaDungeonBoss07h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_BossBiteSwipe_01);
      }
      else
        yield return (object) uldaDungeonBoss07h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_07h.VO_ULDA_BOSS_07h_Female_Kobold_BossBEES_01);
    }
  }
}
