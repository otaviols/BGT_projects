using System.Collections;
using System.Collections.Generic;

public class BOM_05_Tamsin_Fight_004 : BOM_05_Tamsin_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Death_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Death_01.prefab:7e7b500c719aeac46b1db6ee24aafe94");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4EmoteResponse_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4EmoteResponse_01.prefab:f488daaf30a9620488a9bbeb5cb244d0");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeA_02 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeA_02.prefab:7c53eade7b3ca554b899ff2e47b7b1e0");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeB_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeB_01.prefab:fd1f0cb9980a6f142a4e9d1462c1ad21");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeC_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeC_01.prefab:7f544d2af0bdedf4795ea968859d22cc");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeD_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeD_01.prefab:b1111bddd1094d54d85d77a16931fc2c");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4HeroPower_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4HeroPower_01.prefab:cbbfac8c36b906c49a0cae07d1de9af5");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Idle_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Idle_01.prefab:5e569526060d4d64c8a02f02be033bcc");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Intro_02 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Intro_02.prefab:55952bd2746114342958ffee9a0aee3f");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Loss_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Loss_01.prefab:9f1a59e172771274e8ddcef462780182");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Death_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Death_01.prefab:8944ecc497f00624fbd215b4e1180e7a");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4EmoteResponse_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4EmoteResponse_01.prefab:e54ef5722c8dc5f4583fcabb62707729");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeK_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeK_01.prefab:23f2f391c2e8a904482780b337fa2a72");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeL_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeL_01.prefab:d7e5d5cdd2ce34d4980226e9dab32f9a");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeM_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeM_01.prefab:521cbdebfeda7cb44b96924d1ac35847");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeN_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeN_01.prefab:b7de7630367d48e4fbb754847a8e74a7");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeO_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeO_01.prefab:2cf76d58b4006f74a9c82a924236fcf2");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4HeroPower_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4HeroPower_01.prefab:07991c9adad896a438071342d54c7361");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Idle_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Idle_01.prefab:1d6306e646772954e9fde0bd11cca983");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Loss_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Loss_01.prefab:0aef5ff1648fbef479eab84f03e8601f");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Victory_01.prefab:592d64e34d345d4478d2fbc3f12da893");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Death_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Death_01.prefab:7ed667f1dbc8fea489de0b45352e3a45");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4EmoteResponse_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4EmoteResponse_01.prefab:5a55cef9e9ecdaa4b8bde9a51cd46d79");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeE_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeE_01.prefab:fd79a5d1a0d82a346a4fe907813b8b7e");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeF_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeF_02.prefab:c50f49ae528c00e4e941ea96e4890495");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeH_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeH_01.prefab:333751ec4ef223c45988a3b866f5dfb6");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeI_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeI_02.prefab:5d2bfa20c1d52c245b5d891e562fa3bf");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeJ_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeJ_01.prefab:8ed23596c37565649928bae336c3742f");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4HeroPower_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4HeroPower_01.prefab:4efefe21010606342a7fcb78f9e1603b");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Idle_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Idle_01.prefab:591db8580a625f34cb057203cd00ade6");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Loss_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Loss_01.prefab:ad1e70a559716ae4193cd186595e10a1");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeA_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeA_01.prefab:063ff4c5f79138b409d3cac21fea255f");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeB_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeB_02.prefab:622db9043ef1cab47a98ef7ee0f5de61");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeD_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeD_02.prefab:50b0127f0ba94b54f912b3b28e343080");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeF_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeF_01.prefab:67b42efd16e909443b3f3d1ac6c002ea");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeG_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeG_01.prefab:5435638c4ec863a409f48ef2e98fd267");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeG_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeG_02.prefab:c659f49f8c4abe846ab321091a076a9d");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeH_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeH_02.prefab:97a841793a987b44d9f1601d98a376ae");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeI_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeI_01.prefab:70db38d3b5c34fc41a01ec34e6ae05da");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeI_03 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeI_03.prefab:f4ae83fc78d286b4b8248a83af131016");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeK_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeK_02.prefab:c576e318f6de02f4b9efe988004efe1e");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeL_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeL_02.prefab:32b397f0c7c1944458efc05b84b91bdd");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeM_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeM_02.prefab:e67b54c2e60e7914cbd20d7f595b3f29");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeP_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeP_01.prefab:640ebf4633ac3384abb0e286da639825");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4Intro_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4Intro_01.prefab:a60faacae76220d4fbc6ab3120c1ca39");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4Victory_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4Victory_02.prefab:8e2f44fb28235b34e83d6675ad2c5d5d");
  private static readonly AssetReference VO_PVPDR_Hero_Tamsin_Female_Forsaken_Death_01 = new AssetReference("VO_PVPDR_Hero_Tamsin_Female_Forsaken_Death_01.prefab:0261caf1dfbfae146bf927a03851b086");
  private List<string> m_InGame_IntroductionLines = new List<string>()
  {
    (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4Intro_01,
    (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Intro_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Death_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4EmoteResponse_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeA_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeB_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeC_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeD_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4HeroPower_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Idle_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Intro_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Loss_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Death_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4EmoteResponse_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeK_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeL_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeM_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeN_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeO_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4HeroPower_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Idle_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Loss_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Victory_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Death_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4EmoteResponse_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeE_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeF_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeH_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeI_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeJ_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4HeroPower_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Idle_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Loss_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeA_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeB_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeD_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeF_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeG_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeG_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeH_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeI_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeI_03,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeK_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeL_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeM_02,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeP_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4Intro_01,
      (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4Victory_02,
      (string) BOM_05_Tamsin_Fight_004.VO_PVPDR_Hero_Tamsin_Female_Forsaken_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame() => base.OnCreateGame();

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_05_Tamsin_Fight_004 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
    Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).SetName("");
    Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).UpdateHeroNameBanner();
    switch (missionEvent)
    {
      case 101:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeD_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeD_02);
        obj.MissionPause(false);
        break;
      case 102:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeE_01);
        obj.MissionPause(false);
        break;
      case 103:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeF_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeF_02);
        break;
      case 104:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeG_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeG_02);
        break;
      case 105:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeI_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeI_02);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeI_03);
        break;
      case 107:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4ExchangeJ_01);
        obj.MissionPause(false);
        break;
      case 108:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeK_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeK_02);
        obj.MissionPause(false);
        break;
      case 109:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeL_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeL_02);
        break;
      case 110:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeM_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeM_02);
        break;
      case 111:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4ExchangeN_01);
        break;
      case 504:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Victory_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4Victory_02);
        obj.MissionPause(false);
        break;
      case 507:
        obj.MissionPause(true);
        if (cardId == "BOM_05_Brukan_004hb")
          yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Loss_01);
        else if (cardId == "BOM_05_Rokara_004hb")
          yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Loss_01);
        else
          yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Loss_01);
        obj.MissionPause(false);
        break;
      case 510:
        if (cardId == "BOM_05_Brukan_004hb")
        {
          yield return (object) obj.MissionPlayVOOnce(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4HeroPower_01);
          break;
        }
        if (cardId == "BOM_05_Rokara_004hb")
        {
          yield return (object) obj.MissionPlayVOOnce(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4HeroPower_01);
          break;
        }
        yield return (object) obj.MissionPlayVOOnce(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4HeroPower_01);
        break;
      case 514:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4Intro_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Intro_02);
        break;
      case 515:
        if (cardId == "BOM_05_Brukan_004hb")
        {
          yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4EmoteResponse_01);
          break;
        }
        if (cardId == "BOM_05_Rokara_004hb")
        {
          yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4EmoteResponse_01);
          break;
        }
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4EmoteResponse_01);
        break;
      case 516:
        if (cardId == "BOM_05_Brukan_004hb" || !(cardId == "BOM_05_Rokara_004hb"))
          break;
        break;
      case 517:
        if (cardId == "BOM_05_Brukan_004hb")
        {
          yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4Idle_01);
          break;
        }
        if (cardId == "BOM_05_Rokara_004hb")
        {
          yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Rokara_Female_Orc_BOM_Tamsin_Mission4Idle_01);
          break;
        }
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Guff_Male_Tauren_BOM_Tamsin_Mission4Idle_01);
        break;
      case 519:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlaySound(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_PVPDR_Hero_Tamsin_Female_Forsaken_Death_01);
        obj.MissionPause(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) obj.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_05_Tamsin_Fight_004 obj = this;
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
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BOM_05_Tamsin_Fight_004 obj = this;
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
    BOM_05_Tamsin_Fight_004 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 5:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeA_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeA_02);
        break;
      case 9:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Brukan_Male_Troll_BOM_Tamsin_Mission4ExchangeB_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_004.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission4ExchangeB_02);
        break;
    }
  }
}
