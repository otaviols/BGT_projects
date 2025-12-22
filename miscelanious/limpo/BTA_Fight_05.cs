using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTA_Fight_05 : BTA_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BTA_Fight_05.InitBooleanOptions();
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_05_Hero_HalfHealth_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_05_Hero_HalfHealth_01.prefab:c5c04c5dee72be34e95fdc69a817b641");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_05_Minion_BaduuArrivesE_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_05_Minion_BaduuArrivesE_01.prefab:fe4f991f6d2a1b14690e1b6e14f5ee32");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_05_PlayerStart_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_05_PlayerStart_01.prefab:cbbdef9bb61fd1f478451af8463cae8e");
  private static readonly AssetReference VO_BTA_05_Male_Sporelok_Mission_Fight_05_Minion_BaduuArrivesC_01 = new AssetReference("VO_BTA_05_Male_Sporelok_Mission_Fight_05_Minion_BaduuArrivesC_01.prefab:4dda4e29c4c540f4e8a52e6e40789ebb");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_05_Minion_BaduuArrivesB_01 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_05_Minion_BaduuArrivesB_01.prefab:343ebc20ee2d71647820df3efaea5821");
  private static readonly AssetReference VO_BTA_09_Female_Naga_Mission_Fight_05_Minion_BaduuArrivesD_01 = new AssetReference("VO_BTA_09_Female_Naga_Mission_Fight_05_Minion_BaduuArrivesD_01.prefab:c85a8bc5e26272d4dba740b640e15394");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_Attack_01.prefab:5fd92fe65b14de7428032eaecf56c145");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_Hellfire_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_Hellfire_01.prefab:78440e87696d2d74baebc54162e68f93");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_MortalCoilKill_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_MortalCoilKill_01.prefab:250c96848ac395d4c93963a4132ce306");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_RustswornChampion_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_RustswornChampion_01.prefab:42ea5291394b26f47bd7a0fcc991ad6c");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BossDeath_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BossDeath_01.prefab:9f833c29dddfb4442b7bde1d8f8a2588");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BossStart_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BossStart_01.prefab:34b68752ecbb8574694f1e9c84499991");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_01.prefab:aff4fe69dcd180b40a99dd6b016c7ff7");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_02 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_02.prefab:7f7e8ae3202d18842b9480783b30713d");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_03 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_03.prefab:dec355da407f82e499c5a15cf3b9250e");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Emote_Response_01.prefab:5e88889a325d4eb438ff5ce34d680ee7");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_01.prefab:08d2b599be3e16e42ac16524bad02a75");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_02 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_02.prefab:ad3757e9fb5957f40aacabab91dd1877");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_03 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_03.prefab:c7703ba0f79108743a87d0c10ed29735");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_04 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_04.prefab:c78358a33ef9f054f935bbc3d3dd5e8b");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleA_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleA_01.prefab:d1727cf00bf37d34b997ed928b66425b");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleB_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleB_01.prefab:a2469495e2eb61f4bb6505316dd0ac5e");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleC_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleC_01.prefab:f14c7d5670f249f489994e70f6762f1f");
  private static readonly AssetReference VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_TurnOne_01 = new AssetReference("VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_TurnOne_01.prefab:3f517a03417e2aa4c925ebeae1aaa1de");
  private static readonly AssetReference VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleA_01 = new AssetReference("VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleA_01.prefab:5c77cbbfc70d21b4ea45b086bb27f156");
  private static readonly AssetReference VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleB_01 = new AssetReference("VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleB_01.prefab:444a752a58748b5448db17badf5d56d4");
  private static readonly AssetReference VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleC_01 = new AssetReference("VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleC_01.prefab:e1ad308612baeba49887f8f93ce6318c");
  private static readonly AssetReference VO_BTA_03_Female_Broken_Mission_Fight_05_Minion_BaduuArrivesA_01 = new AssetReference("VO_BTA_03_Female_Broken_Mission_Fight_05_Minion_BaduuArrivesA_01.prefab:5b689f915f277f345ace5509a742f6d7");
  private static readonly AssetReference VO_BTA_03_Female_Broken_Mission_Fight_05_Minion_BaduuHand_01 = new AssetReference("VO_BTA_03_Female_Broken_Mission_Fight_05_Minion_BaduuHand_01.prefab:5ffe3c59c188c924295b977333012ee0");
  private List<string> m_VO_BTA_BOSS_05h_dleLines = new List<string>()
  {
    (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleA_01,
    (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleB_01,
    (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleC_01
  };
  private List<string> m_missionEventTrigger100_Lines = new List<string>()
  {
    (string) BTA_Fight_05.VO_BTA_01_Female_NightElf_Mission_Fight_05_Minion_BaduuArrivesE_01,
    (string) BTA_Fight_05.VO_BTA_05_Male_Sporelok_Mission_Fight_05_Minion_BaduuArrivesC_01,
    (string) BTA_Fight_05.VO_BTA_07_Male_Orc_Mission_Fight_05_Minion_BaduuArrivesB_01,
    (string) BTA_Fight_05.VO_BTA_09_Female_Naga_Mission_Fight_05_Minion_BaduuArrivesD_01
  };
  private List<string> m_VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_Lines = new List<string>()
  {
    (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_01,
    (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_02,
    (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_03
  };
  private List<string> m_VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_Lines = new List<string>()
  {
    (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_01,
    (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_02,
    (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_03,
    (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_04
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BTA_Fight_05() => this.m_gameOptions.AddBooleanOptions(BTA_Fight_05.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_05.VO_BTA_03_Female_Broken_Mission_Fight_05_Minion_BaduuArrivesA_01,
      (string) BTA_Fight_05.VO_BTA_07_Male_Orc_Mission_Fight_05_Minion_BaduuArrivesB_01,
      (string) BTA_Fight_05.VO_BTA_05_Male_Sporelok_Mission_Fight_05_Minion_BaduuArrivesC_01,
      (string) BTA_Fight_05.VO_BTA_09_Female_Naga_Mission_Fight_05_Minion_BaduuArrivesD_01,
      (string) BTA_Fight_05.VO_BTA_01_Female_NightElf_Mission_Fight_05_Minion_BaduuArrivesE_01,
      (string) BTA_Fight_05.VO_BTA_01_Female_NightElf_Mission_Fight_05_Hero_HalfHealth_01,
      (string) BTA_Fight_05.VO_BTA_01_Female_NightElf_Mission_Fight_05_PlayerStart_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_Attack_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_Hellfire_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_MortalCoilKill_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_RustswornChampion_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BossDeath_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BossStart_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_02,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_03,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Emote_Response_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_02,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_03,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_04,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleA_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleB_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_IdleC_01,
      (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_TurnOne_01,
      (string) BTA_Fight_05.VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleA_01,
      (string) BTA_Fight_05.VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleB_01,
      (string) BTA_Fight_05.VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleC_01,
      (string) BTA_Fight_05.VO_BTA_03_Female_Broken_Mission_Fight_05_Minion_BaduuHand_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_05h_dleLines;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BossDeath_01;
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BTA_Fight_05 btaFight05 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) btaFight05.PlayLineAlways(actor, (string) BTA_Fight_05.VO_BTA_01_Female_NightElf_Mission_Fight_05_PlayerStart_01);
    yield return (object) btaFight05.PlayLineAlways(enemyActor, (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BossStart_01);
    GameState.Get().SetBusy(false);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Emote_Response_01, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BTA_Fight_05 btaFight05 = this;
    while (btaFight05.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 100:
        yield return (object) new WaitForSeconds(2f);
        yield return (object) btaFight05.PlayLineAlwaysWithBrassRing(btaFight05.GetFriendlyActorByCardId("BTA_07"), BTA_Dungeon.KarnukBrassRing, (string) BTA_Fight_05.VO_BTA_07_Male_Orc_Mission_Fight_05_Minion_BaduuArrivesB_01);
        yield return (object) btaFight05.PlayLineAlwaysWithBrassRing(btaFight05.GetFriendlyActorByCardId("BTA_05"), BTA_Dungeon.SklibbBrassRing, (string) BTA_Fight_05.VO_BTA_05_Male_Sporelok_Mission_Fight_05_Minion_BaduuArrivesC_01);
        yield return (object) btaFight05.PlayLineAlwaysWithBrassRing(btaFight05.GetFriendlyActorByCardId("BTA_09"), BTA_Dungeon.ShaljaBrassRing, (string) BTA_Fight_05.VO_BTA_09_Female_Naga_Mission_Fight_05_Minion_BaduuArrivesD_01);
        yield return (object) btaFight05.PlayLineAlways(friendlyActor, (string) BTA_Fight_05.VO_BTA_01_Female_NightElf_Mission_Fight_05_Minion_BaduuArrivesE_01);
        yield return (object) btaFight05.PlayAndRemoveRandomLineOnlyOnce(enemyActor, btaFight05.m_VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_Lines);
        break;
      case 101:
        yield return (object) btaFight05.PlayLineAlways(friendlyActor, (string) BTA_Fight_05.VO_BTA_01_Female_NightElf_Mission_Fight_05_Hero_HalfHealth_01);
        break;
      case 500:
        btaFight05.PlaySound((string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_Attack_01);
        break;
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) btaFight05.PlayLineAlwaysWithBrassRing(btaFight05.GetFriendlyActorByCardId("BTA_03"), BTA_Dungeon.BaduuBrassRing, (string) BTA_Fight_05.VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleA_01);
        yield return (object) btaFight05.PlayLineAlwaysWithBrassRing(btaFight05.GetFriendlyActorByCardId("BTA_03"), BTA_Dungeon.BaduuBrassRing, (string) BTA_Fight_05.VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleB_01);
        yield return (object) btaFight05.PlayLineAlwaysWithBrassRing(btaFight05.GetFriendlyActorByCardId("BTA_03"), BTA_Dungeon.BaduuBrassRing, (string) BTA_Fight_05.VO_BTA_03_Female_Broken_Mission_Fight_05_Baduu_BlackTempleC_01);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        yield return (object) btaFight05.PlayAndRemoveRandomLineOnlyOnce(enemyActor, btaFight05.m_VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_HeroPower_Lines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight05.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_05 btaFight05 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight05.\u003C\u003En__1(entity);
    while (btaFight05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight05.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BT_707t"))
      {
        if (cardId == "BTA_03")
          yield return (object) btaFight05.PlayLineAlwaysWithBrassRing(btaFight05.GetFriendlyActorByCardId("BTA_03"), BTA_Dungeon.BaduuBrassRing, (string) BTA_Fight_05.VO_BTA_03_Female_Broken_Mission_Fight_05_Minion_BaduuHand_01);
      }
      else
        yield return (object) btaFight05.PlayRandomLineAlways(actor, btaFight05.m_VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_BrokenAppear_Lines);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_05 btaFight05 = this;
    while (btaFight05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight05.\u003C\u003En__2(entity);
      yield return (object) btaFight05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight05.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "BTA_11"))
      {
        if (!(cardId == "CS2_062"))
        {
          if (cardId == "EX1_302")
            yield return (object) btaFight05.PlayLineAlways(actor, (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_MortalCoilKill_01);
        }
        else
          yield return (object) btaFight05.PlayLineAlways(actor, (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_Hellfire_01);
      }
      else
        yield return (object) btaFight05.PlayLineAlways(actor, (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_Boss_RustswornChampion_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_05 btaFight05 = this;
    while (btaFight05.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    if (turn == 1)
      yield return (object) btaFight05.PlayLineAlways(actor, (string) BTA_Fight_05.VO_BTA_BOSS_05h_Male_Human_Mission_Fight_05_TurnOne_01);
  }
}
