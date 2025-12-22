using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BTA_Fight_08 : BTA_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BTA_Fight_08.InitBooleanOptions();
  private static readonly AssetReference VO_BTA_03_Female_Broken_Mission_Fight_08_Baduu_LeavesA_01 = new AssetReference("VO_BTA_03_Female_Broken_Mission_Fight_08_Baduu_LeavesA_01.prefab:09fe292b6a1b94b428adfbe9db8809c5");
  private static readonly AssetReference VO_BTA_03_Female_Broken_Mission_Fight_08_Baduu_LeavesB_01 = new AssetReference("VO_BTA_03_Female_Broken_Mission_Fight_08_Baduu_LeavesB_01.prefab:9e98b1db22ef8294792a8bb7102ae810");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_08_Baduu_LeavesC_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_08_Baduu_LeavesC_01.prefab:4a3c1e53c527a3146a9476b393e5f983");
  private static readonly AssetReference VO_BTA_01_Female_NightElf_Mission_Fight_08_PlayerStart_01 = new AssetReference("VO_BTA_01_Female_NightElf_Mission_Fight_08_PlayerStart_01.prefab:9efa7a47b4707e44e9d6ef375738d65c");
  private static readonly AssetReference VO_BTA_07_Male_Orc_Mission_Fight_08_Baduu_LeavesD_01 = new AssetReference("VO_BTA_07_Male_Orc_Mission_Fight_08_Baduu_LeavesD_01.prefab:b32f4880489990646ac268acf7848bad");
  private static readonly AssetReference VO_BTA_09_Female_Naga_Mission_Fight_08_Baduu_LeavesE_01 = new AssetReference("VO_BTA_09_Female_Naga_Mission_Fight_08_Baduu_LeavesE_01.prefab:ef97d0921d3ecb243b90874aaaeffb66");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_Attack_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_Attack_01.prefab:c5202afd72c29ae48b1872f7d3439c90");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_Bladestorm_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_Bladestorm_01.prefab:21ac1b9cd0ea61e49b4173d194b0549c");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_WeaponEquip_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_WeaponEquip_01.prefab:38a7b80155a260041aaa9c35c5c94afe");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossDeathA_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossDeathA_01.prefab:d9fc0fc33abebcb4dbd141f919d1aaf8");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossDeathB_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossDeathB_01.prefab:147fa644024309242a993347c2c40f69");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossStart_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossStart_01.prefab:20aa52e8516ccf840ab33041a1b6b156");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Emote_Response_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Emote_Response_01.prefab:27ac88d8e363d3d48b760fc63670547f");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_ScavengersCunningTrigger_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_ScavengersCunningTrigger_01.prefab:a590375c66d72ee4a80667cf33cd835f");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_StolenSteel_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_StolenSteel_01.prefab:f1f91f8e6ac14224989bd6aaba134f56");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_WeaponEquip_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_WeaponEquip_01.prefab:5ba775ab6ec34014886b35bd244db8fe");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_01.prefab:97d67acd04e9a2f4a9b23ce157c6319a");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_02 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_02.prefab:b81ebfcf3f854a445b79910b9036433f");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_03 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_03.prefab:a1ebf1a8089d55847b53527e8e4a9fa6");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_04 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_04.prefab:8aef94c85e692ce4882f7f28319b29d1");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleA_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleA_01.prefab:3bda4a141204bd04cb4f998fd2c1aa5f");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleB_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleB_01.prefab:cb997a4b6e649ba42b8f7eacb40e1db6");
  private static readonly AssetReference VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleC_01 = new AssetReference("VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleC_01.prefab:7589621788aab454fbe8152adf9985e4");
  private static readonly AssetReference VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_08_TurnOneBrassRing_01 = new AssetReference("VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_08_TurnOneBrassRing_01.prefab:451dc64e05ca02943b20628e58626815");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_08_Illidans_Challenge_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_08_Illidans_Challenge_01.prefab:ceae815f01901664aa8eea462fec6677");
  private static readonly AssetReference VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_08_VictoryA_01 = new AssetReference("VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_08_VictoryA_01.prefab:a08cc7c7b46c59840b966ccd1ce5b436");
  private static readonly AssetReference VO_BTA_03_Female_Broken_Mission_Fight_08_TurnOneBrassRing_02 = new AssetReference("VO_BTA_03_Female_Broken_Mission_Fight_08_TurnOneBrassRing_02.prefab:ccbd1a293888a554b96911496dec8b11");
  private List<string> m_VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_Lines = new List<string>()
  {
    (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_01,
    (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_02,
    (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_03,
    (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_04
  };
  private List<string> m_VO_BTA_BOSS_08h_IdleLines = new List<string>()
  {
    (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleA_01,
    (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleB_01,
    (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleC_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BTA_Fight_08() => this.m_gameOptions.AddBooleanOptions(BTA_Fight_08.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BTA_Fight_08.VO_BTA_03_Female_Broken_Mission_Fight_08_Baduu_LeavesA_01,
      (string) BTA_Fight_08.VO_BTA_03_Female_Broken_Mission_Fight_08_Baduu_LeavesB_01,
      (string) BTA_Fight_08.VO_BTA_01_Female_NightElf_Mission_Fight_08_Baduu_LeavesC_01,
      (string) BTA_Fight_08.VO_BTA_01_Female_NightElf_Mission_Fight_08_PlayerStart_01,
      (string) BTA_Fight_08.VO_BTA_07_Male_Orc_Mission_Fight_08_Baduu_LeavesD_01,
      (string) BTA_Fight_08.VO_BTA_09_Female_Naga_Mission_Fight_08_Baduu_LeavesE_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_Attack_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_Bladestorm_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_WeaponEquip_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossDeathA_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossDeathB_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossStart_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Emote_Response_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_ScavengersCunningTrigger_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_StolenSteel_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_WeaponEquip_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_02,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_03,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_04,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleA_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleB_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_IdleC_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_08_TurnOneBrassRing_01,
      (string) BTA_Fight_08.VO_BTA_03_Female_Broken_Mission_Fight_08_TurnOneBrassRing_02,
      (string) BTA_Fight_08.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_08_Illidans_Challenge_01,
      (string) BTA_Fight_08.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_08_VictoryA_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetIdleLines() => this.m_VO_BTA_BOSS_08h_IdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossDeathB_01;
  }

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BTA_Fight_08 btaFight08 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) btaFight08.PlayLineAlways(actor, (string) BTA_Fight_08.VO_BTA_01_Female_NightElf_Mission_Fight_08_PlayerStart_01);
    yield return (object) btaFight08.PlayLineAlways(enemyActor, (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_BossStart_01);
    GameState.Get().SetBusy(false);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Emote_Response_01, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BTA_Fight_08 btaFight08 = this;
    while (btaFight08.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 100:
        yield return (object) btaFight08.PlayLineAlways(actor, (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_WeaponEquip_01);
        break;
      case 101:
        yield return (object) btaFight08.PlayLineAlways(actor, (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_WeaponEquip_01);
        break;
      case 102:
        yield return (object) btaFight08.PlayLineAlways(actor, (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_ScavengersCunningTrigger_01);
        break;
      case 103:
        GameState.Get().SetBusy(true);
        yield return (object) btaFight08.PlayLineAlwaysWithBrassRing(btaFight08.GetFriendlyActorByCardId("BTA_03"), BTA_Dungeon.BaduuBrassRing, (string) BTA_Fight_08.VO_BTA_03_Female_Broken_Mission_Fight_08_Baduu_LeavesB_01);
        GameState.Get().SetBusy(false);
        yield return (object) btaFight08.PlayLineAlways(friendlyActor, (string) BTA_Fight_08.VO_BTA_01_Female_NightElf_Mission_Fight_08_Baduu_LeavesC_01);
        yield return (object) btaFight08.PlayLineAlwaysWithBrassRing(btaFight08.GetFriendlyActorByCardId("BTA_07"), BTA_Dungeon.KarnukBrassRing, (string) BTA_Fight_08.VO_BTA_07_Male_Orc_Mission_Fight_08_Baduu_LeavesD_01);
        yield return (object) btaFight08.PlayLineAlwaysWithBrassRing(btaFight08.GetFriendlyActorByCardId("BTA_09"), BTA_Dungeon.ShaljaBrassRing, (string) BTA_Fight_08.VO_BTA_09_Female_Naga_Mission_Fight_08_Baduu_LeavesE_01);
        break;
      case 500:
        btaFight08.PlaySound((string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_Attack_01);
        break;
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) btaFight08.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_08.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_08_VictoryA_01);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        yield return (object) btaFight08.PlayAndRemoveRandomLineOnlyOnce(actor, btaFight08.m_VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_HeroPower_Lines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) btaFight08.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_08 btaFight08 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) btaFight08.\u003C\u003En__1(entity);
    while (btaFight08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) btaFight08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight08.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "TRL_156")
        yield return (object) btaFight08.PlayLineAlways(actor, (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Hero_StolenSteel_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BTA_Fight_08 btaFight08 = this;
    while (btaFight08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!btaFight08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) btaFight08.\u003C\u003En__2(entity);
      yield return (object) btaFight08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      btaFight08.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "BT_117")
        yield return (object) btaFight08.PlayLineAlways(actor, (string) BTA_Fight_08.VO_BTA_BOSS_08h_Female_Demon_Mission_Fight_08_Boss_Bladestorm_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BTA_Fight_08 btaFight08 = this;
    while (btaFight08.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) btaFight08.PlayLineAlways((string) BTA_Dungeon.IllidanBrassRing, (string) BTA_Fight_08.VO_BTA_BOSS_17h_Male_NightElf_Mission_Fight_08_Illidans_Challenge_01);
        break;
      case 3:
        yield return (object) btaFight08.PlayLineAlwaysWithBrassRing(btaFight08.GetFriendlyActorByCardId("BTA_09"), BTA_Dungeon.ShaljaBrassRing, (string) BTA_Fight_08.VO_BTA_BOSS_09h_Female_Naga_Mission_Fight_08_TurnOneBrassRing_01);
        yield return (object) btaFight08.PlayLineAlwaysWithBrassRing(btaFight08.GetFriendlyActorByCardId("BTA_03"), BTA_Dungeon.BaduuBrassRing, (string) BTA_Fight_08.VO_BTA_03_Female_Broken_Mission_Fight_08_TurnOneBrassRing_02);
        break;
      case 5:
        yield return (object) btaFight08.PlayLineAlwaysWithBrassRing(btaFight08.GetFriendlyActorByCardId("BTA_03"), BTA_Dungeon.BaduuBrassRing, (string) BTA_Fight_08.VO_BTA_03_Female_Broken_Mission_Fight_08_Baduu_LeavesA_01);
        break;
    }
  }
}
