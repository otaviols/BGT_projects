using System.Collections;
using System.Collections.Generic;

public class DALA_Dungeon_Boss_14h : DALA_Dungeon
{
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_BossBrawl_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_BossBrawl_01.prefab:0387c38aaa4828946bf1afe52d5b57d7");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_BossBrawl_02 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_BossBrawl_02.prefab:1055c46b195ef0b429b98c7f148ca354");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_01.prefab:3529c6612f600d945ba2a63b20cfac30");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_02 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_02.prefab:c559926c12ee89a468ff1f560e6c7357");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_03 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_03.prefab:2130ce377fd22474aa87e5f7bd86034b");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_04 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_04.prefab:e20958b1ba708404fb103bbc82d682e4");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_Death_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_Death_01.prefab:7e25486f6ed6630468c87ca07757b98c");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_DefeatPlayer_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_DefeatPlayer_01.prefab:ba15377f29b7aed44922214b13a53cb0");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_EmoteResponse_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_EmoteResponse_01.prefab:0c5757ee092f237499e93532dabb4862");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_01.prefab:9616a94095992594ab4e26cb130a84bb");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_02 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_02.prefab:33f12ace376cdd4439784dbf89c15372");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_03 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_03.prefab:37a0706354e36fe49966a1efc4f06034");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_04 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_04.prefab:085da2b55d007914aa390c165073bcd9");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_Idle_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_Idle_01.prefab:7741e3f93c779f04fa7f6b0492330c37");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_Idle_02 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_Idle_02.prefab:dad14c9ae939c1648970bfb583a481cc");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_Idle_03 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_Idle_03.prefab:a10f0db716b36914e80189de9837b05c");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_Intro_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_Intro_01.prefab:e4c01fcf044ade142af03ede6bdc6aa5");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_IntroChu_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_IntroChu_01.prefab:761a30e5ac47dce4ebdb58bc89d5aa97");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_IntroGeorge_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_IntroGeorge_01.prefab:a9a436580dab9f1428fc79e0cd301149");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_IntroOlBarkeye_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_IntroOlBarkeye_01.prefab:cbcaeacaba94312429e33d6daa659829");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_PlayerBarista_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_PlayerBarista_01.prefab:6d2e50276607aa34db9c2a5a7a2641fa");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_PlayerBelligerentGnome_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_PlayerBelligerentGnome_01.prefab:296063de5dab691408cf8659b9fac8b7");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_PlayerBrawl_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_PlayerBrawl_01.prefab:aba174b9c03c2b542859aa3902ffb5c0");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_PlayerBrawl_02 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_PlayerBrawl_02.prefab:37273bbda399c7e4696b3eade4e5df6d");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_PlayerFriendlyBartender_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_PlayerFriendlyBartender_01.prefab:baaa7281105b85f499284e3513b3e834");
  private static readonly AssetReference VO_DALA_BOSS_14h_Female_KulTiran_PlayerInnkeeper_01 = new AssetReference("VO_DALA_BOSS_14h_Female_KulTiran_PlayerInnkeeper_01.prefab:9d19a40eedb9dac47ae55a62077a3e73");
  private static List<string> m_IdleLines = new List<string>()
  {
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Idle_01,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Idle_02,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();
  private static List<string> m_PlayerBrawl = new List<string>()
  {
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Idle_01,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Idle_02,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Idle_03
  };
  private static List<string> m_BossMinions = new List<string>()
  {
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_01,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_02,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_03,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_04
  };
  private static List<string> m_BossBrawl = new List<string>()
  {
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossBrawl_01,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossBrawl_02
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossBrawl_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossBrawl_02,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_02,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_03,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_BossMinions_04,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Death_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_DefeatPlayer_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_EmoteResponse_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_02,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_03,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_04,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Idle_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Idle_02,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Idle_03,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Intro_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_IntroChu_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_IntroGeorge_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_IntroOlBarkeye_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_PlayerBarista_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_PlayerBelligerentGnome_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_PlayerBrawl_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_PlayerBrawl_02,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_PlayerFriendlyBartender_01,
      (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_PlayerInnkeeper_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_01,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_02,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_03,
    (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_HeroPower_04
  };

  public override List<string> GetIdleLines() => DALA_Dungeon_Boss_14h.m_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_introLine = (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Intro_01;
    this.m_deathLine = (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_Death_01;
    this.m_standardEmoteResponseLine = (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_EmoteResponse_01;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "DALA_Barkeye")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_IntroOlBarkeye_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "DALA_George")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_IntroGeorge_01, Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
      {
        if (!(cardId != "DALA_Chu"))
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

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_14h dalaDungeonBoss14h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) dalaDungeonBoss14h.\u003C\u003En__0(entity);
    while (dalaDungeonBoss14h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss14h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      yield return (object) dalaDungeonBoss14h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss14h.m_playedLines.Add(cardId);
      if (!(cardId == "EX1_407"))
      {
        if (!(cardId == "CFM_654"))
        {
          if (!(cardId == "DAL_546"))
          {
            if (cardId == "DAL_560")
              yield return (object) dalaDungeonBoss14h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_PlayerInnkeeper_01);
          }
          else
            yield return (object) dalaDungeonBoss14h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_PlayerBarista_01);
        }
        else
          yield return (object) dalaDungeonBoss14h.PlayLineOnlyOnce(enemyActor, (string) DALA_Dungeon_Boss_14h.VO_DALA_BOSS_14h_Female_KulTiran_PlayerFriendlyBartender_01);
      }
      else
        yield return (object) dalaDungeonBoss14h.PlayAndRemoveRandomLineOnlyOnce(enemyActor, DALA_Dungeon_Boss_14h.m_PlayerBrawl);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    DALA_Dungeon_Boss_14h dalaDungeonBoss14h = this;
    while (dalaDungeonBoss14h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!dalaDungeonBoss14h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) dalaDungeonBoss14h.\u003C\u003En__1(entity);
      yield return (object) dalaDungeonBoss14h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      dalaDungeonBoss14h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_407"))
      {
        if (cardId == "UNG_929" || cardId == "LOOT_367" || cardId == "EX1_604" || cardId == "BRM_019")
          yield return (object) dalaDungeonBoss14h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_14h.m_BossMinions);
      }
      else
        yield return (object) dalaDungeonBoss14h.PlayAndRemoveRandomLineOnlyOnce(actor, DALA_Dungeon_Boss_14h.m_BossBrawl);
    }
  }
}
