using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTA_Fight_04 : BTA_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BTA_Fight_04.InitBooleanOptions();
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_04_PlayerStart_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_04_PlayerStart_01.prefab:b99e8ebb8836363418e17dbd92fa7cd7");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_04_VictoryA_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_04_VictoryA_01.prefab:5305888bb582e254bb858c5ce5d7f9e3");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_04_VictoryB_Alt_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_04_VictoryB_Alt_01.prefab:47f10b6b49af9fc44b9e6f2bc80b62a7");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_04_Hero_Karnuk_01 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_04_Hero_Karnuk_01.prefab:5c03c208ff92d4642964d1b7175022f6");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_04_TurnOne_01 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_04_TurnOne_01.prefab:ac3c840da6e4ed04795865006e16cfbf");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_04_TurnOneBrassRing_01 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_04_TurnOneBrassRing_01.prefab:03b2b3317fea65844b0f619703be372c");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryA_Alt_01 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryA_Alt_01.prefab:3180d3c3e9efbd74bbcdb6e9353c9f6c");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryB_01 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryB_01.prefab:950571ab6cf8f0345bb9b1334c8e0a0e");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryC_Alt_01 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryC_Alt_01.prefab:2d5b6008b6890ab41a2a6fa751ec0c74");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_Attack_01.prefab:e5fba2dd14dc48747bab9016996e123e");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_EnhancedDreadlord_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_EnhancedDreadlord_01.prefab:f4dce4b2bc0d49145a2a6e766fc14541");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_ImprisonedImp_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_ImprisonedImp_01.prefab:44af927cdea31e44997761e20adfb605");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_UnstableFelbolt_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_UnstableFelbolt_01.prefab:1a8bfab4b13796647a2b39391cf0d46f");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_BossDeath_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_BossDeath_01.prefab:54f93764ae0beb74e9d345f347462ccd");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_BossStart_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_BossStart_01.prefab:dadfc8cc24c537c41b71501dd2f323fc");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Emote_Response_01.prefab:921d4735151291e48b162409197f6c54");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_InfectiousSporeling_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_InfectiousSporeling_01.prefab:bf25dfecb6641c444b84ca636e1140b7");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_Shalja_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_Shalja_01.prefab:862be978ec1a6cb4d92d4db93b29f0e7");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_Sklibb_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_Sklibb_01.prefab:ce41f3411985ec642885cc8fe02375ec");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_01.prefab:dbbc642c42fbdb443aa9914cc6c69cf1");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_02 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_02.prefab:64f73c485ab96f740bf28c9bc9466872");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_03 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_03.prefab:4ab532bf70fd35346bd2c627b9e7a4b0");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_04 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_04.prefab:501d0d5c1d9dbba44be9081523fe72e0");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleA_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleA_01.prefab:48918c4a1a0841f46be43b0bfcf47da7");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleB_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleB_01.prefab:8f601c1b96c97ef418ec2a825d543fa9");
  private static readonly AssetReference VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleC_01 = new AssetReference("VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleC_01.prefab:92c33f8d9667cc245b6ecd5e1c06a21c");
  public bool m_boolean_DisplayVictory;
  private List<string> m_VO_BTA_BOSS_04h_IdleLines = new List<string>()
  {
    (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleA_01,
    (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleB_01,
    (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleC_01
  };
  private List<string> m_VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_Lines = new List<string>()
  {
    (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_01,
    (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_03,
    (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_04
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BTA_Fight_04() => this.m_gameOptions.AddBooleanOptions(BTA_Fight_04.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_04.VO_BTA_01_Female_NightElf_Mission_Fight_04_PlayerStart_01,
      (string) BTA_Fight_04.VO_BTA_01_Female_NightElf_Mission_Fight_04_VictoryA_01,
      (string) BTA_Fight_04.VO_BTA_01_Female_NightElf_Mission_Fight_04_VictoryB_Alt_01,
      (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_Hero_Karnuk_01,
      (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_TurnOne_01,
      (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_TurnOneBrassRing_01,
      (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryA_Alt_01,
      (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryB_01,
      (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryC_Alt_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_Attack_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_EnhancedDreadlord_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_ImprisonedImp_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_UnstableFelbolt_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_BossDeath_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_BossStart_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Emote_Response_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_InfectiousSporeling_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_Shalja_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_Sklibb_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_02,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_03,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_04,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleA_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleB_01,
      (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_IdleC_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_04h_IdleLines;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState)
  {
    if (playState == TAG_PLAYSTATE.WON)
      this.m_boolean_DisplayVictory = true;
    return base.ShouldPlayHeroBlowUpSpells(playState);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_BossDeath_01;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BTA_Fight_04 btaFight04 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) btaFight04.PlayLineAlways(actor, (string) BTA_Fight_04.VO_BTA_01_Female_NightElf_Mission_Fight_04_PlayerStart_01);
    yield return (object) btaFight04.PlayLineAlways(enemyActor, (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_BossStart_01);
    GameState.Get().SetBusy(false);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Emote_Response_01, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BTA_Fight_04 btaFight04 = this;
    while (btaFight04.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) btaFight04.PlayLineAlways(actor1, (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_HeroPowerTrigger_02);
        break;
      case 102:
        yield return (object) btaFight04.PlayLineAlways(actor2, (string) BTA_Fight_04.VO_BTA_01_Female_NightElf_Mission_Fight_04_VictoryA_01);
        yield return (object) btaFight04.PlayLineAlwaysWithBrassRing(btaFight04.GetFriendlyActorByCardId("BTA_07"), BTA_Dungeon.KarnukBrassRing, (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryB_01);
        break;
      case 500:
        btaFight04.PlaySound((string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_Attack_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight04.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_04 btaFight04 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight04.\u003C\u003En__1(entity);
    while (btaFight04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight04.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BT_731"))
      {
        if (!(cardId == "BTA_05"))
        {
          if (cardId == "BTA_07")
            yield return (object) btaFight04.PlayLineAlwaysWithBrassRing(btaFight04.GetFriendlyActorByCardId("BTA_07"), BTA_Dungeon.KarnukBrassRing, (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_Hero_Karnuk_01);
        }
        else
          yield return (object) btaFight04.PlayLineAlways(actor, (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_Sklibb_01);
      }
      else
        yield return (object) btaFight04.PlayLineAlways(actor, (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Hero_InfectiousSporeling_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_04 btaFight04 = this;
    while (btaFight04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight04.\u003C\u003En__2(entity);
      yield return (object) btaFight04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight04.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BT_199"))
      {
        if (!(cardId == "BT_304"))
        {
          if (cardId == "BT_305")
            yield return (object) btaFight04.PlayLineAlways(actor, (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_ImprisonedImp_01);
        }
        else
          yield return (object) btaFight04.PlayLineAlways(actor, (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_EnhancedDreadlord_01);
      }
      else
        yield return (object) btaFight04.PlayLineAlways(actor, (string) BTA_Fight_04.VO_BTA_BOSS_04h_Male_Dreadlord_Mission_Fight_04_Boss_UnstableFelbolt_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_04 btaFight04 = this;
    while (btaFight04.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (turn == 1)
      yield return (object) btaFight04.PlayLineAlways(btaFight04.GetFriendlyActorByCardId("BTA_07"), (string) BTA_Dungeon.KarnukBrassRing, (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_TurnOneBrassRing_01);
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    BTA_Fight_04 btaFight04 = this;
    while (btaFight04.m_enemySpeaking)
      yield return (object) null;
    yield return (object) new WaitForSeconds(5f);
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (btaFight04.m_boolean_DisplayVictory)
    {
      GameState.Get().SetBusy(true);
      yield return (object) btaFight04.PlayLineAlwaysWithBrassRing(btaFight04.GetFriendlyActorByCardId("BTA_07"), BTA_Dungeon.KarnukBrassRing, (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryA_Alt_01);
      yield return (object) btaFight04.PlayLineAlways(friendlyActor, (string) BTA_Fight_04.VO_BTA_01_Female_NightElf_Mission_Fight_04_VictoryB_Alt_01);
      yield return (object) btaFight04.PlayLineAlwaysWithBrassRing(btaFight04.GetFriendlyActorByCardId("BTA_07"), BTA_Dungeon.KarnukBrassRing, (string) BTA_Fight_04.VO_BTA_07_Male_Orc_Mission_Fight_04_VictoryC_Alt_01);
      GameState.Get().SetBusy(false);
    }
  }
}
