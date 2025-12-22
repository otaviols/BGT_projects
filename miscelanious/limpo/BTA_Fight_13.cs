using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTA_Fight_13 : BTA_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BTA_Fight_13.InitBooleanOptions();
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_13_PlayerStart_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_13_PlayerStart_01.prefab:bae570e3f5e92504dafd2ec014f95469");
  private static readonly AssetReference VO_BTA_08_Male_Orc_Mission_Fight_13_Misc_02 = new AssetReference("VO_BTA_08_Male_Orc_Mission_Fight_13_Misc_02.prefab:db1012ddaade0eb4c80841a7979488a9");
  private static readonly AssetReference VO_BTA_08_Male_Orc_Mission_Fight_13_VictoryA_01 = new AssetReference("VO_BTA_08_Male_Orc_Mission_Fight_13_VictoryA_01.prefab:3f7c607714785d14a90a5d6f9e9942df");
  private static readonly AssetReference VO_BTA_10_Female_Naga_Mission_Fight_13_Misc_01 = new AssetReference("VO_BTA_10_Female_Naga_Mission_Fight_13_Misc_01.prefab:04ab7d9bb0134fb43b507218e50093c9");
  private static readonly AssetReference VO_BTA_10_Female_Naga_Mission_Fight_13_VictoryB_01 = new AssetReference("VO_BTA_10_Female_Naga_Mission_Fight_13_VictoryB_01.prefab:832f26ddd691333499c84771674e9729");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_Attack_01.prefab:1ea54479130e6e74b8486ceacca9e138");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_RustedBasilisk_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_RustedBasilisk_01.prefab:45d04acd3518701488e6ecc12f075a01");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_RustedFungalGiant_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_RustedFungalGiant_01.prefab:a4884c283694ef9439867c3d95f8fe0a");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossDeath_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossDeath_01.prefab:5d7d05fa5f5e7f840a75b4997f715254");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_01.prefab:393f6dd3c7f07704b8c09513b8fede3b");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_02 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_02.prefab:6365b6327d264b940a47b433328f16d7");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Emote_Response_01.prefab:9b94ae21a9aba4446b87d1bf7997e806");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_FelSummoner_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_FelSummoner_01.prefab:300df50b2d1aa594e911513834cfd40e");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_GlaiveboundAdeptTrigger_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_GlaiveboundAdeptTrigger_01.prefab:dd4999b21305f404c994e01585b277bd");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_WrathspikeBrute_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_WrathspikeBrute_01.prefab:53c16de44f503914995c745164c42360");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_01.prefab:f6bcaa55d4a8ddd46b51ed06755dacdc");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_02 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_02.prefab:06affe6dba6315a4994e4cffcbfbedf9");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_03 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_03.prefab:3f006288048bea248b9116041f7600ff");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_IdleA_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_IdleA_01.prefab:815d3d852c6f7484896ca5aa7ce0408a");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_IdleC_01 = new AssetReference("VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_IdleC_01.prefab:01620e4baee2a4c4d81940a48586365b");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_Boss_EndlessLegion_01 = new AssetReference("VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_Boss_EndlessLegion_01.prefab:0903eb35b0e73794aad8171c1eb0c72b");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_Hero_ImmolationAura_01 = new AssetReference("VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_Hero_ImmolationAura_01.prefab:bdc5f5b29b1db2d408bdd8ce273ba905");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_01 = new AssetReference("VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_01.prefab:766c798064cdb204f8591f2698884b8e");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_02 = new AssetReference("VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_02.prefab:f0f69149f5582114e818dfc7cb485f6f");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_IdleA_01 = new AssetReference("VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_IdleA_01.prefab:5a4a77be23df1e647b603812a049dbe8");
  private static readonly AssetReference VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_IdleB_01 = new AssetReference("VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_IdleB_01.prefab:42ddb238f95e9c04e9c9615e3133008c");
  private Notification.SpeechBubbleDirection m_OgreMechSpeechBubbleDirection = Notification.SpeechBubbleDirection.TopLeft;
  private List<string> m_VO_BTA_BOSS_13h_IdleLines = new List<string>()
  {
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_IdleA_01,
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_IdleB_01,
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_IdleA_01,
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_IdleC_01
  };
  private List<string> m_missionEventTrigger501_Lines = new List<string>()
  {
    (string) BTA_Fight_13.VO_BTA_08_Male_Orc_Mission_Fight_13_VictoryA_01,
    (string) BTA_Fight_13.VO_BTA_10_Female_Naga_Mission_Fight_13_VictoryB_01
  };
  private List<string> m_VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_Lines = new List<string>()
  {
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_01,
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_02
  };
  private List<string> m_VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_Lines = new List<string>()
  {
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_01,
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_02,
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_03
  };
  private List<string> m_VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_Lines = new List<string>()
  {
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_01,
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_02
  };
  private List<string> m_VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_Lines_Copy = new List<string>()
  {
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_01,
    (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BTA_Fight_13() => this.m_gameOptions.AddBooleanOptions(BTA_Fight_13.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_13.VO_BTA_01_Female_NightElf_Mission_Fight_13_PlayerStart_01,
      (string) BTA_Fight_13.VO_BTA_08_Male_Orc_Mission_Fight_13_Misc_02,
      (string) BTA_Fight_13.VO_BTA_08_Male_Orc_Mission_Fight_13_VictoryA_01,
      (string) BTA_Fight_13.VO_BTA_10_Female_Naga_Mission_Fight_13_Misc_01,
      (string) BTA_Fight_13.VO_BTA_10_Female_Naga_Mission_Fight_13_VictoryB_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_Attack_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_RustedBasilisk_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_RustedFungalGiant_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossDeath_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_02,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Emote_Response_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_FelSummoner_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_GlaiveboundAdeptTrigger_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_WrathspikeBrute_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_Boss_EndlessLegion_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_Hero_ImmolationAura_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_IdleA_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_IdleB_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_IdleA_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_IdleC_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_02,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_03,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_01,
      (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_13h_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_Lines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossDeath_01;
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BTA_Fight_13 btaFight13 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) btaFight13.PlayLineAlways(actor, (string) BTA_Fight_13.VO_BTA_01_Female_NightElf_Mission_Fight_13_PlayerStart_01);
    yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_01);
    yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_BossStart_02, btaFight13.m_OgreMechSpeechBubbleDirection);
    GameState.Get().SetBusy(false);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Emote_Response_01, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BTA_Fight_13 btaFight13 = this;
    while (btaFight13.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) btaFight13.PlayLineAlwaysWithBrassRing(btaFight13.GetFriendlyActorByCardId("BTA_08"), BTA_Dungeon.KarnukBrassRingDemonHunter, (string) BTA_Fight_13.VO_BTA_08_Male_Orc_Mission_Fight_13_Misc_02);
        break;
      case 500:
        btaFight13.PlaySound((string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_Attack_01);
        break;
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) btaFight13.PlayLineAlwaysWithBrassRing(btaFight13.GetFriendlyActorByCardId("BTA_08"), BTA_Dungeon.KarnukBrassRingDemonHunter, (string) BTA_Fight_13.VO_BTA_08_Male_Orc_Mission_Fight_13_VictoryA_01);
        yield return (object) btaFight13.PlayLineAlwaysWithBrassRing(btaFight13.GetFriendlyActorByCardId("BTA_10"), BTA_Dungeon.ShaljaBrassRingDemonHunter, (string) BTA_Fight_13.VO_BTA_10_Female_Naga_Mission_Fight_13_VictoryB_01);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        switch (Random.Range(1, 3))
        {
          case 1:
            yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_02);
            yield break;
          case 2:
            yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_03);
            yield break;
          default:
            yield break;
        }
      case 508:
        switch (Random.Range(1, 3))
        {
          case 1:
            yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_02);
            yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_02, btaFight13.m_OgreMechSpeechBubbleDirection);
            yield break;
          case 2:
            yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_HeroPower_01);
            yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_HeroPower_01, btaFight13.m_OgreMechSpeechBubbleDirection);
            yield break;
          default:
            yield break;
        }
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight13.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_13 btaFight13 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight13.\u003C\u003En__1(entity);
    while (btaFight13.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    yield return (object) btaFight13.WaitForEntitySoundToFinish(entity);
    string cardId = entity.GetCardId();
    btaFight13.m_playedLines.Add(cardId);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (!(cardId == "BT_495"))
    {
      if (!(cardId == "BT_509"))
      {
        if (!(cardId == "BT_510"))
        {
          if (!(cardId == "BT_514"))
          {
            if (!(cardId == "BTA_10"))
            {
              if (cardId == "BTA_08")
                yield return (object) btaFight13.PlayLineAlwaysWithBrassRing(btaFight13.GetFriendlyActorByCardId("BTA_08"), BTA_Dungeon.KarnukBrassRingDemonHunter, (string) BTA_Fight_13.VO_BTA_08_Male_Orc_Mission_Fight_13_Misc_02);
            }
            else
              yield return (object) btaFight13.PlayLineAlwaysWithBrassRing(btaFight13.GetFriendlyActorByCardId("BTA_10"), BTA_Dungeon.ShaljaBrassRingDemonHunter, (string) BTA_Fight_13.VO_BTA_10_Female_Naga_Mission_Fight_13_Misc_01);
          }
          else
            yield return (object) btaFight13.PlayLineAlways(actor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_Hero_ImmolationAura_01, btaFight13.m_OgreMechSpeechBubbleDirection);
        }
        else
          yield return (object) btaFight13.PlayLineAlways(actor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_WrathspikeBrute_01);
      }
      else
        yield return (object) btaFight13.PlayLineAlways(actor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_FelSummoner_01);
    }
    else
      yield return (object) btaFight13.PlayLineAlways(actor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Hero_GlaiveboundAdeptTrigger_01);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_13 btaFight13 = this;
    while (btaFight13.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight13.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight13.\u003C\u003En__2(entity);
      yield return (object) btaFight13.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight13.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BTA_14"))
      {
        if (!(cardId == "BTA_15"))
        {
          if (cardId == "BTA_16")
            yield return (object) btaFight13.PlayLineAlways(actor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_RustedFungalGiant_01);
        }
        else
          yield return (object) btaFight13.PlayLineAlways(actor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_Boss_EndlessLegion_01, btaFight13.m_OgreMechSpeechBubbleDirection);
      }
      else
        yield return (object) btaFight13.PlayLineAlways(actor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_Boss_RustedBasilisk_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_13 btaFight13 = this;
    while (btaFight13.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
  }

  public override float GetThinkEmoteBossThinkChancePercentage() => 0.5f;

  public override void OnPlayThinkEmote()
  {
    if (this.m_enemySpeaking)
      return;
    Player currentPlayer = GameState.Get().GetCurrentPlayer();
    if (!currentPlayer.IsFriendlySide() || currentPlayer.GetHeroCard().HasActiveEmoteSound())
      return;
    if ((double) this.GetThinkEmoteBossThinkChancePercentage() > (double) Random.Range(0.0f, 1f) && this.m_BossIdleLines != null && this.m_BossIdleLines.Count != 0)
    {
      GameEntity.Coroutines.StartCoroutine(this.PlayPairedBossIdleLines());
    }
    else
    {
      EmoteType emoteType = EmoteType.THINK1;
      switch (Random.Range(1, 4))
      {
        case 1:
          emoteType = EmoteType.THINK1;
          break;
        case 2:
          emoteType = EmoteType.THINK2;
          break;
        case 3:
          emoteType = EmoteType.THINK3;
          break;
      }
      GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(emoteType);
    }
  }

  protected IEnumerator PlayPairedBossIdleLines()
  {
    BTA_Fight_13 btaFight13 = this;
    int num = Random.Range(1, 3);
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (num == 1)
    {
      yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_IdleA_01);
      yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_IdleA_01, btaFight13.m_OgreMechSpeechBubbleDirection);
    }
    else
    {
      yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_OgreMech_Mission_Fight_13_IdleB_01, btaFight13.m_OgreMechSpeechBubbleDirection);
      yield return (object) btaFight13.PlayLineAlways(enemyActor, (string) BTA_Fight_13.VO_BTA_BOSS_13h_Male_Ogre_Mission_Fight_13_IdleC_01);
    }
  }
}
