using System.Collections;
using System.Collections.Generic;

public class ULDA_Dungeon_Boss_03h : ULDA_Dungeon
{
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_BossTriggerSinisterDeal_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_BossTriggerSinisterDeal_01.prefab:a51a01532d42ad146b2aa33565be4b34");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_BossTriggerWeaponizedWasp_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_BossTriggerWeaponizedWasp_01.prefab:d010cf5ae2f4dfc4898b4ed186806fa7");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_Death_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_Death_01.prefab:a6327c291f470f949bbf8cb5104218be");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_DefeatPlayer_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_DefeatPlayer_01.prefab:213adc426d7f1474a9deef70ff873cd9");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_EmoteResponse_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_EmoteResponse_01.prefab:82f4117350cc42447b084876b0992418");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_HeroPower_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_HeroPower_01.prefab:8a85ecb0a49cf0d42a5c7f78f095334d");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_HeroPower_02 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_HeroPower_02.prefab:c0fc67769fe151048b3d0e594ae978df");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_HeroPower_05 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_HeroPower_05.prefab:e467b975f9ed44347945d9ec4cd5320c");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_Idle_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_Idle_01.prefab:eee83ede7cb5c234d920db60baa9ce35");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_Idle_02 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_Idle_02.prefab:c785f9f3685821447af9360560687b3b");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_Idle_03 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_Idle_03.prefab:da256f8fe6d5dad4ba4597194bf33d57");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_Intro_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_Intro_01.prefab:d2328bc0879cd1f408381a0aba8822ab");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_IntroBrannResponse_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_IntroBrannResponse_01.prefab:00517485c0759c445b311efde42de66e");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_IntroEliseResponse_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_IntroEliseResponse_01.prefab:e980a990444f5f44798d5d123f42846c");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_PlayerTrigger_Expired_Merchant_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_PlayerTrigger_Expired_Merchant_01.prefab:081efa092c272794883e1217241e80da");
  private static readonly AssetReference VO_ULDA_BOSS_03h_Female_Human_PlayerTrigger_Frightened_Flunky_01 = new AssetReference("VO_ULDA_BOSS_03h_Female_Human_PlayerTrigger_Frightened_Flunky_01.prefab:187596b96e402164fbe57fd39100233d");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_HeroPower_01,
    (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_HeroPower_02,
    (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_HeroPower_05
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_Idle_01,
    (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_Idle_02,
    (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_BossTriggerSinisterDeal_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_BossTriggerWeaponizedWasp_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_Death_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_DefeatPlayer_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_EmoteResponse_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_HeroPower_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_HeroPower_02,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_HeroPower_05,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_Idle_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_Idle_02,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_Idle_03,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_Intro_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_IntroBrannResponse_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_IntroEliseResponse_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_PlayerTrigger_Expired_Merchant_01,
      (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_PlayerTrigger_Frightened_Flunky_01
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
    this.m_introLine = (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_Intro_01;
    this.m_deathLine = (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_Death_01;
    this.m_standardEmoteResponseLine = (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      if (cardId == "ULDA_Brann")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_IntroBrannResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
      else if (cardId == "ULDA_Elise")
      {
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_IntroEliseResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
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
    ULDA_Dungeon_Boss_03h uldaDungeonBoss03h = this;
    while (uldaDungeonBoss03h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 101)
    {
      yield return (object) uldaDungeonBoss03h.PlayAndRemoveRandomLineOnlyOnce(actor, uldaDungeonBoss03h.m_HeroPowerLines);
    }
    else
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss03h.\u003C\u003En__0(missionEvent);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_03h uldaDungeonBoss03h = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) uldaDungeonBoss03h.\u003C\u003En__1(entity);
    while (uldaDungeonBoss03h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss03h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) uldaDungeonBoss03h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss03h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_195"))
      {
        if (cardId == "ULD_163")
          yield return (object) uldaDungeonBoss03h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_PlayerTrigger_Expired_Merchant_01);
      }
      else
        yield return (object) uldaDungeonBoss03h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_PlayerTrigger_Frightened_Flunky_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    ULDA_Dungeon_Boss_03h uldaDungeonBoss03h = this;
    while (uldaDungeonBoss03h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!uldaDungeonBoss03h.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) uldaDungeonBoss03h.\u003C\u003En__2(entity);
      yield return (object) uldaDungeonBoss03h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      uldaDungeonBoss03h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "ULD_160"))
      {
        if (cardId == "ULD_170")
          yield return (object) uldaDungeonBoss03h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_BossTriggerWeaponizedWasp_01);
      }
      else
        yield return (object) uldaDungeonBoss03h.PlayLineOnlyOnce(actor, (string) ULDA_Dungeon_Boss_03h.VO_ULDA_BOSS_03h_Female_Human_BossTriggerSinisterDeal_01);
    }
  }
}
