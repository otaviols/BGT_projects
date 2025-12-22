using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTA_Fight_07 : BTA_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BTA_Fight_07.InitBooleanOptions();
  private static Map<GameEntityOption, string> s_stringOptions = BTA_Fight_07.InitStringOptions();
  private static readonly AssetReference VO_BTA_05_Male_Sporelok_Mission_Fight_07_Hero_SelfRepairsReply_01 = new AssetReference("VO_BTA_05_Male_Sporelok_Mission_Fight_07_Hero_SelfRepairsReply_01.prefab:4017403984d282041a161261b674aa2f");
  private static readonly AssetReference VO_BTA_05_Male_Sporelok_Mission_Fight_07_VictoryB_01 = new AssetReference("VO_BTA_05_Male_Sporelok_Mission_Fight_07_VictoryB_01.prefab:c51713e191ee2184493bbab10e8ab8ba");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_07_BrassRing_TurboBoostReply_01 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_07_BrassRing_TurboBoostReply_01.prefab:6ab4ecb2ab304e4488143a38ac6f8dbb");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannons_02 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannons_02.prefab:c15127a0157c203439fece5e4b3a4e2f");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannonsReply_01 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannonsReply_01.prefab:84137aaf5131fe24a880e295f270a5aa");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannonsReply_02 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannonsReply_02.prefab:0a2c2580a5f0074488f82a54b9cc3a55");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_07_Misc_02 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_07_Misc_02.prefab:572d1de818e589f4f9043d9929bad47f");
  private static readonly AssetReference VO_BTA_09_Female_Naga_Mission_Fight_07_BrassRing_Refueling_01 = new AssetReference("VO_BTA_09_Female_Naga_Mission_Fight_07_BrassRing_Refueling_01.prefab:0a50f58e6466a3f4a832cb778dd97144");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_01 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_01.prefab:1bca6ee372b507b40a04e2c9f0b26bb7");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_02 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_02.prefab:7caab9d6af7df6a47af7570ec903202d");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_03 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_03.prefab:04873e433a5619a4ea3c26bcc6d98024");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_FelCannons_01 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_FelCannons_01.prefab:9b1c350c6ab01824ebc72aba0ffca8f6");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_FelCannons_02 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_FelCannons_02.prefab:95055fee3ad738b44a48534879b8585d");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_01 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_01.prefab:baf60ad5b99e596488e33dd0efa5c4f2");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_02 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_02.prefab:7517f5bf99c78f646b151ace0e56c7f2");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_03 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_03.prefab:c8e2df79b70087c499b3fbac46102400");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_01 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_01.prefab:6959ec06d9fce674399e88e0c27de165");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_02 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_02.prefab:3c6fc137bfe16db44b55a7d96367a36c");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_03 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_03.prefab:1fe4fed3bcf786b4789a3b98f6c4716b");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_01 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_01.prefab:7a7283343530d134eb88522f9214240d");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_02 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_02.prefab:e2f1c7e8866649e42acf265e3e6df3b7");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_03 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_03.prefab:0a1772545ff208946969653626be596e");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_04 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_04.prefab:443d4dec43022954ebf6231f0f124957");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Misc_03 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Misc_03.prefab:eff630d1ff4e877448bf249d3097165c");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_PlayerStart_01 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_PlayerStart_01.prefab:616c9e57bb7fce54684e26be50462d17");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_01 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_01.prefab:e7f979d7c3d0a6748aae01e75b588fc7");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_02 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_02.prefab:e56d157cb67db4e4097487bdb61f4d41");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_03 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_03.prefab:d87d224d117aa8d449e0080d7d1afeef");
  private static readonly AssetReference VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_VictoryA_01 = new AssetReference("VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_VictoryA_01.prefab:06093d41aab91bf4ca4322c5189ba5a5");
  private static readonly AssetReference VO_BTA_03_Female_Broken_Mission_Fight_07_Misc_01 = new AssetReference("VO_BTA_03_Female_Broken_Mission_Fight_07_Misc_01.prefab:b9a7f440e929f954a90bb76c906e8346");
  private List<string> m_VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_FelCannons_Lines = new List<string>()
  {
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_FelCannons_01,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_FelCannons_02
  };
  private List<string> m_VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_Lines = new List<string>()
  {
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_01,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_02
  };
  private List<string> m_VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannonsReply_Lines = new List<string>()
  {
    (string) BTA_Fight_07.VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannonsReply_01,
    (string) BTA_Fight_07.VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannonsReply_02
  };
  private List<string> m_missionEventTrigger501_Lines = new List<string>()
  {
    (string) BTA_Fight_07.VO_BTA_05_Male_Sporelok_Mission_Fight_07_VictoryB_01,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_VictoryA_01
  };
  private List<string> m_missionEventTrigger103_Lines = new List<string>()
  {
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_01,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_02
  };
  private List<string> m_VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_Lines = new List<string>()
  {
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_01,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_02,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_03
  };
  private List<string> m_VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_Lines = new List<string>()
  {
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_01,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_02,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_03,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_04
  };
  private List<string> m_missionEventTrigger101_Lines = new List<string>()
  {
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_01,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_02,
    (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();
  private Notification m_turnCounter;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.HANDLE_COIN,
      false
    }
  };

  private static Map<GameEntityOption, string> InitStringOptions() => new Map<GameEntityOption, string>();

  public BTA_Fight_07() => this.m_gameOptions.AddOptions(BTA_Fight_07.s_booleanOptions, BTA_Fight_07.s_stringOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_07.VO_BTA_03_Female_Broken_Mission_Fight_07_Misc_01,
      (string) BTA_Fight_07.VO_BTA_07_Male_Orc_Mission_Fight_07_Misc_02,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Misc_03,
      (string) BTA_Fight_07.VO_BTA_05_Male_Sporelok_Mission_Fight_07_Hero_SelfRepairsReply_01,
      (string) BTA_Fight_07.VO_BTA_05_Male_Sporelok_Mission_Fight_07_VictoryB_01,
      (string) BTA_Fight_07.VO_BTA_07_Male_Orc_Mission_Fight_07_BrassRing_TurboBoostReply_01,
      (string) BTA_Fight_07.VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannons_02,
      (string) BTA_Fight_07.VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannonsReply_01,
      (string) BTA_Fight_07.VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannonsReply_02,
      (string) BTA_Fight_07.VO_BTA_09_Female_Naga_Mission_Fight_07_BrassRing_Refueling_01,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_01,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_02,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_03,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_FelCannons_01,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_FelCannons_02,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_01,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_02,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_03,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_01,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_02,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_03,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_01,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_02,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_03,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_04,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_PlayerStart_01,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_01,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_02,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_TurboBoost_03,
      (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_VictoryA_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame() => base.OnCreateGame();

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
      return;
    MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType);
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BTA_Fight_07 btaFight07 = this;
    while (btaFight07.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) btaFight07.PlayRandomLineAlways(actor, btaFight07.m_missionEventTrigger101_Lines);
        break;
      case 102:
        yield return (object) btaFight07.PlayLineAlwaysWithBrassRing(btaFight07.GetFriendlyActorByCardId("BTA_07"), BTA_Dungeon.KarnukBrassRing, (string) BTA_Fight_07.VO_BTA_07_Male_Orc_Mission_Fight_07_BrassRing_TurboBoostReply_01);
        break;
      case 103:
        if (Random.Range(1, 4) == 1)
        {
          yield return (object) btaFight07.PlayLineAlways(actor, (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_Refueling_03);
          yield return (object) btaFight07.PlayLineAlwaysWithBrassRing(btaFight07.GetFriendlyActorByCardId("BTA_09"), BTA_Dungeon.ShaljaBrassRing, (string) BTA_Fight_07.VO_BTA_09_Female_Naga_Mission_Fight_07_BrassRing_Refueling_01);
          break;
        }
        yield return (object) btaFight07.PlayRandomLineAlways(actor, btaFight07.m_missionEventTrigger103_Lines);
        break;
      case 106:
        yield return (object) btaFight07.PlayLineAlways(actor, (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Misc_03);
        break;
      case 107:
        yield return (object) btaFight07.PlayLineAlwaysWithBrassRing(btaFight07.GetFriendlyActorByCardId("BTA_03"), BTA_Dungeon.BaduuBrassRing, (string) BTA_Fight_07.VO_BTA_03_Female_Broken_Mission_Fight_07_Misc_01);
        break;
      case 108:
        yield return (object) btaFight07.PlayLineAlwaysWithBrassRing(btaFight07.GetFriendlyActorByCardId("BTA_07"), BTA_Dungeon.KarnukBrassRing, (string) BTA_Fight_07.VO_BTA_07_Male_Orc_Mission_Fight_07_Misc_02);
        break;
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) btaFight07.PlayLineAlways(actor, (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_VictoryA_01);
        yield return (object) btaFight07.PlayLineAlwaysWithBrassRing(btaFight07.GetFriendlyActorByCardId("BTA_05"), BTA_Dungeon.SklibbBrassRing, (string) BTA_Fight_07.VO_BTA_05_Male_Sporelok_Mission_Fight_07_VictoryB_01);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        yield return (object) btaFight07.PlayAndRemoveRandomLineOnlyOnce(actor, btaFight07.m_VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_HeroPower_Lines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight07.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_07 btaFight07 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight07.\u003C\u003En__1(entity);
    while (btaFight07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BTA_BOSS_07s2"))
      {
        if (!(cardId == "BTA_BOSS_07s3"))
        {
          if (cardId == "BTA_BOSS_07s4")
          {
            if (Random.Range(1, 4) == 1)
            {
              yield return (object) btaFight07.PlayLineAlways(actor, (string) BTA_Fight_07.VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_03);
              yield return (object) btaFight07.PlayLineAlwaysWithBrassRing(btaFight07.GetFriendlyActorByCardId("BTA_05"), BTA_Dungeon.SklibbBrassRing, (string) BTA_Fight_07.VO_BTA_05_Male_Sporelok_Mission_Fight_07_Hero_SelfRepairsReply_01);
            }
            else
              yield return (object) btaFight07.PlayRandomLineAlways(actor, btaFight07.m_VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_SelfRepairs_Lines);
          }
        }
        else
        {
          yield return (object) btaFight07.PlayRandomLineAlways(actor, btaFight07.m_VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_FelCannons_Lines);
          yield return (object) btaFight07.PlayAndRemoveRandomLineOnlyOnceWithBrassRing(btaFight07.GetFriendlyActorByCardId("BTA_07"), BTA_Dungeon.KarnukBrassRing, btaFight07.m_VO_BTA_07_Male_Orc_Mission_Fight_07_Hero_FelCannonsReply_Lines);
        }
      }
      else
        yield return (object) btaFight07.PlayRandomLineAlways(actor, btaFight07.m_VO_BTA_BOSS_07h2_Female_NightElf_Mission_Fight_07_Hero_ExhaustBackfire_Lines);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_07 btaFight07 = this;
    while (btaFight07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight07.\u003C\u003En__2(entity);
      yield return (object) btaFight07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_07 btaFight07 = this;
    while (btaFight07.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
  }

  public override void NotifyOfMulliganEnded()
  {
    base.NotifyOfMulliganEnded();
    this.InitVisuals();
  }

  private void InitVisuals() => this.InitTurnCounter(this.GetCost());

  public override void OnTagChanged(TagDelta change)
  {
    base.OnTagChanged(change);
    if (change.tag != 48 || change.newValue == change.oldValue)
      return;
    this.UpdateVisuals(change.newValue);
  }

  private void InitTurnCounter(int cost)
  {
    this.m_turnCounter = AssetLoader.Get().InstantiatePrefab((AssetReference) "BTA_Destroyer_Turn_Timer.prefab:5641f96ff71c06c4e8416952d878e3c7").GetComponent<Notification>();
    PlayMakerFSM component = this.m_turnCounter.GetComponent<PlayMakerFSM>();
    component.FsmVariables.GetFsmBool("RunningMan").Value = false;
    component.FsmVariables.GetFsmBool("MineCart").Value = false;
    component.FsmVariables.GetFsmBool("Airship").Value = false;
    component.FsmVariables.GetFsmBool("Destroyer").Value = true;
    component.SendEvent("Birth");
    this.m_turnCounter.transform.parent = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor().gameObject.transform;
    this.m_turnCounter.transform.localPosition = new Vector3(-1.4f, 0.187f, -0.11f);
    this.m_turnCounter.transform.localScale = Vector3.one * 0.52f;
    this.UpdateTurnCounterText(cost);
  }

  private void UpdateVisuals(int cost) => this.UpdateTurnCounter(cost);

  private void UpdateTurnCounter(int cost)
  {
    this.m_turnCounter.GetComponent<PlayMakerFSM>().SendEvent("Action");
    this.UpdateTurnCounterText(cost);
  }

  private void UpdateTurnCounterText(int cost) => this.m_turnCounter.ChangeDialogText(GameStrings.FormatPlurals("MISSION_DEFAULTCOUNTERNAME", new GameStrings.PluralNumber[1]
  {
    new GameStrings.PluralNumber()
    {
      m_index = 0,
      m_number = cost
    }
  }), cost.ToString(), "", "");
}
