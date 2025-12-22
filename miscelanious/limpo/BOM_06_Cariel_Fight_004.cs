using System.Collections;
using System.Collections.Generic;

public class BOM_06_Cariel_Fight_004 : BOM_06_Cariel_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeA_01 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeA_01.prefab:77ee2610d4842d44cbc088918daa62c8");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeB_01 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeB_01.prefab:f008a9c1c82a9804dac085e590076402");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeC_01 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeC_01.prefab:0b3d603d9e594d54abb9146bab7eb978");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeD_01 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeD_01.prefab:e622e000da19d994c9612fca89de2b13");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4Intro_01 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4Intro_01.prefab:37faf219509492c4fb0f8f0efb722814");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4Victory_02 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4Victory_02.prefab:5e255eb1e2cf6be4696aeaa2b0b640a2");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeA_02 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeA_02.prefab:def142bcbab7ed348b9c5cd8ec080203");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeB_02 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeB_02.prefab:b9785a0b999292342bda3ded9131669e");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeB_03 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeB_03.prefab:c4bb2b6e2fb9b3a42a79dba0f96e15ab");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeD_02 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeD_02.prefab:63c7af451d7059743923e65a38bbf2f3");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4Victory_01.prefab:acbe31adc8a57364faef262a61aabf11");
  private static readonly AssetReference VO_Story_Minion_Tavish_Male_Dwarf_BOM_Cariel_Mission4Death_01 = new AssetReference("VO_Story_Minion_Tavish_Male_Dwarf_BOM_Cariel_Mission4Death_01.prefab:4667b74c2b699c64c89a16702996e762");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Death_01 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Death_01.prefab:b8313888ae371724f98a7753dd6dff35");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4EmoteResponse_01 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4EmoteResponse_01.prefab:b7e3b3b82cb31d641a27b73fed794382");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4ExchangeC_02 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4ExchangeC_02.prefab:32fc07a79d8e7b64ab19ffae4eb3b074");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_01 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_01.prefab:6bef8cd135d7777408902ba33f7f2b6d");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_02 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_02.prefab:541d0a8006e73e4428ebe42fa8ae24e8");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_03 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_03.prefab:c34cdf33fc6f85f4c8280e17484de3c6");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_01 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_01.prefab:9bd27204f7210c548b3c28c9ed52d263");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_02 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_02.prefab:9b612d918e73a4e4eaa31f4f6c42118d");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_03 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_03.prefab:343451949bb68a14ba62a84989206997");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Intro_02 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Intro_02.prefab:9efea2ed3ba142b43a94ec98f87bbb79");
  private static readonly AssetReference VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Loss_01 = new AssetReference("VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Loss_01.prefab:3d12d285fbadd36428c02d13567c8c9e");
  private static readonly AssetReference VO_PVPDR_Hero_Cariel_Female_Human_Death_01 = new AssetReference("VO_PVPDR_Hero_Cariel_Female_Human_Death_01.prefab:523b6a53885a78244ab354f929dbd2b0");
  private List<string> m_InGame_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_01,
    (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_02,
    (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_03
  };
  private List<string> m_InGame_BossIdleLines = new List<string>()
  {
    (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_01,
    (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_02,
    (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeA_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeB_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeC_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeD_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4Intro_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4Victory_02,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Minion_Tavish_Male_Dwarf_BOM_Cariel_Mission4Death_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeA_02,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeB_02,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeB_03,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeD_02,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4Victory_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Death_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4EmoteResponse_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4ExchangeC_02,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_02,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4HeroPower_03,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_01,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_02,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Idle_03,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Intro_02,
      (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Loss_01,
      (string) BOM_06_Cariel_Fight_004.VO_PVPDR_Hero_Cariel_Female_Human_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_BRMAdventure;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_06_Cariel_Fight_004 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 505:
        GameState.Get().SetBusy(true);
        yield return (object) obj.MissionPlayVO(obj.Tavish4_BrassRing_Quote, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4Victory_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4Victory_02);
        GameState.Get().SetBusy(false);
        break;
      case 506:
        GameState.Get().SetBusy(true);
        yield return (object) obj.MissionPlayVO(obj.Tavish4_BrassRing_Quote, (string) BOM_06_Cariel_Fight_004.VO_Story_Minion_Tavish_Male_Dwarf_BOM_Cariel_Mission4Death_01);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 510:
        yield return (object) obj.MissionPlayVO(enemyActor, obj.m_InGame_BossUsesHeroPowerLines);
        break;
      case 514:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4Intro_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Intro_02);
        break;
      case 515:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4EmoteResponse_01);
        break;
      case 516:
        yield return (object) obj.MissionPlaySound(enemyActor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4Death_01);
        break;
      case 517:
        yield return (object) obj.MissionPlayVO(enemyActor, obj.m_InGame_BossIdleLines);
        break;
      case 519:
        yield return (object) obj.MissionPlaySound(enemyActor, (string) BOM_06_Cariel_Fight_004.VO_PVPDR_Hero_Cariel_Female_Human_Death_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) obj.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_06_Cariel_Fight_004 obj = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) obj.\u003C\u003En__1(entity);
    while (obj.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!obj.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) obj.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      obj.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "BTA_15")
        yield return (object) obj.MissionPlayVO(actor, obj.m_InGame_BossUsesHeroPowerLines);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BOM_06_Cariel_Fight_004 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!obj.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) obj.\u003C\u003En__2(entity);
      yield return (object) obj.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      obj.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BOM_06_Cariel_Fight_004 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) obj.MissionPlayVO(actor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeA_01);
        yield return (object) obj.MissionPlayVO(obj.Tavish4_BrassRing_Quote, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeA_02);
        break;
      case 5:
        yield return (object) obj.MissionPlayVO(actor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeB_01);
        yield return (object) obj.MissionPlayVO(obj.Tavish4_BrassRing_Quote, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeB_02);
        yield return (object) obj.MissionPlayVO(obj.Tavish4_BrassRing_Quote, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeB_03);
        break;
      case 9:
        yield return (object) obj.MissionPlayVO(actor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeC_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Xurgon_Female_Demon_BOM_Cariel_Mission4ExchangeC_02);
        break;
      case 11:
        yield return (object) obj.MissionPlayVO(actor, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Cariel_Female_Human_BOM_Cariel_Mission4ExchangeD_01);
        yield return (object) obj.MissionPlayVO(obj.Tavish4_BrassRing_Quote, (string) BOM_06_Cariel_Fight_004.VO_Story_Hero_Tavish_Male_Dwarf_BOM_Cariel_Mission4ExchangeD_02);
        break;
    }
  }
}
