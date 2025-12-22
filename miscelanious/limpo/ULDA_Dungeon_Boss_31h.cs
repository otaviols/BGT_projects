using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_31h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_BossTrogg_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_BossTrogg_01.prefab:af8e1ec08a124fb46a20c492676a5db7");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_BossTroggzor_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_BossTroggzor_01.prefab:6467fcc50a6d23149b52054aeb195789");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_BossWeapon_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_BossWeapon_01.prefab:1b349a0654244d0488d4b240f74d9dec");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_Death_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_Death_01.prefab:6f4de3aa5ce367d4387602fb9e89e90e");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_DefeatPlayer_01.prefab:34a0ba2a1b7031045bdd2af2c555f49a");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_EmoteResponse_01.prefab:763ed9d4ad281e1428b9f080bff02c2f");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_02.prefab:d1bc8f03029419e46b6fd2716d944def");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_03 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_03.prefab:861afd2c7a51a2a45b54993291594283");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_04 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_04.prefab:f550a7e782a72fd4cbf532dad15f5eb8");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_HeroPowerTrogg_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_HeroPowerTrogg_01.prefab:e374b8b270aa2fb44a6d2bb440954f69");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_Idle_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_Idle_01.prefab:5e3957d6745c69e4b84dc6d4455676ee");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_Idle_02 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_Idle_02.prefab:8dabb442472b4474a853c4403007d637");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_Intro_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_Intro_01.prefab:25ddf98b329d38c47afe26585f4958b9");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_PlayerRefreshmentVendor_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_PlayerRefreshmentVendor_01.prefab:05ac247388bc6344281ed563a9325ba6");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_PlayerSpell_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_PlayerSpell_01.prefab:f6403839cf952e94fb7c4b58753150c1");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_PlayerTrogg_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_PlayerTrogg_01.prefab:963adacdbb3000c468b551d7991da02b");
  private static readonly AssetReference VO_ULDA_BOSS_31h_Female_Trogg_PlayerUndercityHuckster_01 = new AssetReference("VO_ULDA_BOSS_31h_Female_Trogg_PlayerUndercityHuckster_01.prefab:912264c6ae3c2aa4e9a1e1f5a70c6213");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_02,
    (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_03,
    (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_04
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_Idle_01,
    (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_Idle_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_BossTrogg_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_BossTroggzor_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_BossWeapon_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_Death_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_02,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_03,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_HeroPower_04,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_HeroPowerTrogg_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_Idle_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_Idle_02,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_Intro_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_PlayerRefreshmentVendor_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_PlayerSpell_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_PlayerTrogg_01,
      (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_PlayerUndercityHuckster_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (!(cardId != "ULDA_Reno"))
        return;
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
    ULDA_Dungeon_Boss_31h uldaDungeonBoss31h = this;
    while (uldaDungeonBoss31h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) uldaDungeonBoss31h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss31h.m_HeroPowerLines);
        break;
      case 102:
        yield return (object) uldaDungeonBoss31h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_HeroPowerTrogg_01);
        break;
      case 103:
        yield return (object) uldaDungeonBoss31h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_BossWeapon_01);
        break;
      case 104:
        yield return (object) uldaDungeonBoss31h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_PlayerSpell_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) uldaDungeonBoss31h.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_31h uldaDungeonBoss31h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss31h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss31h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss31h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss31h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss31h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "AT_111":
          yield return (object) uldaDungeonBoss31h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_PlayerRefreshmentVendor_01);
          break;
        case "CFM_338":
        case "GVG_067":
        case "GVG_068":
        case "LOE_018":
        case "LOOTA_109":
        case "LOOT_315":
          yield return (object) uldaDungeonBoss31h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_PlayerTrogg_01);
          break;
        case "OG_330":
          yield return (object) uldaDungeonBoss31h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_PlayerUndercityHuckster_01);
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_31h uldaDungeonBoss31h = this;
    while (uldaDungeonBoss31h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss31h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss31h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss31h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss31h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "CFM_338":
        case "GVG_067":
        case "GVG_068":
        case "LOE_018":
        case "LOOTA_109":
        case "LOOT_315":
          yield return (object) uldaDungeonBoss31h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_BossTrogg_01);
          break;
        case "GVG_118":
          yield return (object) uldaDungeonBoss31h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_31h.VO_ULDA_BOSS_31h_Female_Trogg_BossTroggzor_01);
          break;
      }
    }
  }
}
