using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOM_03_Guff_Fight_02 : BOM_03_Guff_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeA_Brukan_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeA_Brukan_01.prefab:4048abecc5f5d9e4b9c3b502ed28ffa8");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeD_Brukan_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeD_Brukan_01.prefab:565147a0bf7bcaa45bd43c847a9be39d");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeF_Brukan_02 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeF_Brukan_02.prefab:93f10c08ef9a7be4b8807a7b932fffc3");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeG_Brukan_02 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeG_Brukan_02.prefab:123e7d8e8400b0a469009b6e6256a775");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeA_Dawngrasp_01 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeA_Dawngrasp_01.prefab:9114b5c918b70a24fa981b6eeafabe86");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeD_Dawngrasp_01 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeD_Dawngrasp_01.prefab:917b11e0abbb48f47bb0812b5322d853");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeF_Dawngrasp_02 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeF_Dawngrasp_02.prefab:197d2515b69357e48aece71eaec3b50f");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeG_Dawngrasp_02 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeG_Dawngrasp_02.prefab:5265fcb5214fc0e44bd7f53049e43d1d");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2Victory_02 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2Victory_02.prefab:b05764f2716d66747b5dd6aad46e8f85");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeA_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeA_01.prefab:a4ef7d2aea7a0b94f958a175f718cabe");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeB_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeB_01.prefab:eec31bf76b1ef8d4b9e78fa00c335d09");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeB_02 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeB_02.prefab:6fb2021a95cae5c4781942644fd847b8");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeC_02 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeC_02.prefab:70fe3692347fdbe46acba10b23147330");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeD_02 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeD_02.prefab:15eea1277afd90f43907a605cbae14a9");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeF_03 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeF_03.prefab:26fd16064dd46684da43f095cd07842d");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Brukan_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Brukan_01.prefab:6560a1f68a6984a4db44f3beed050da7");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Brukan_03 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Brukan_03.prefab:acfc74424c3cec248b051c6546b9a649");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Dawngrasp_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Dawngrasp_01.prefab:8b843f6c8dfa713408ae5c790906d9e0");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Dawngrasp_03 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Dawngrasp_03.prefab:24578ab27cbb0d047b2d969bd58320f3");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Rokara_02 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Rokara_02.prefab:f55ce1c57ba84664d9bc31127de3dd03");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Rokara_03 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Rokara_03.prefab:905827b20a57b564087e796090a55f21");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Tamsin_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Tamsin_01.prefab:b6c40c2b92c012a4f8fbfff313451169");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Tamsin_03 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Tamsin_03.prefab:f8689b519590e6d42bd68d3c123bda85");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2Intro_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2Intro_01.prefab:663e6d90e3ac35f42b6c90740cf2c3de");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2Victory_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2Victory_01.prefab:208d9d9ab5ae34d45a6028fa22ff23ec");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeA_Rokara_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeA_Rokara_01.prefab:94705348fc7268540beb369fc920af57");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeD_Rokara_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeD_Rokara_01.prefab:116a8bad22ddf0d47aa4145953283785");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeF_Rokara_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeF_Rokara_02.prefab:fd1091c831e18a346b42aa7c7d717712");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeG_Rokara_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeG_Rokara_01.prefab:a96c818325485c24f9618c1736897b56");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeA_Tamsin_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeA_Tamsin_01.prefab:04df5c8a687374144a3e28429e6070f7");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeD_Tamsin_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeD_Tamsin_01.prefab:35ccb5968b9445f43b8d2af2b08a1dd2");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeF_Tamsin_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeF_Tamsin_02.prefab:418486aff3a268c489665365412411eb");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeG_Tamsin_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeG_Tamsin_02.prefab:e2acfe14e8f78fd4d80883040987b263");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeG_Tamsin_04 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeG_Tamsin_04.prefab:0a57d476318b0a14f9e8fbb667e4bf95");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Death_01 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Death_01.prefab:d13da2377cb72f342abc00ca0643d762");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2EmoteResponse_01 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2EmoteResponse_01.prefab:0fadc5a7770d93c46926f8dba2549b71");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeC_01 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeC_01.prefab:9c281132a6c798f4291cefc779277ee8");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeE_01 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeE_01.prefab:5f05714849fc5154dac6eb1b96e7c4b9");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeF_01 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeF_01.prefab:7fa0be704639ea14fbdad9906d7abf76");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2HeroPower_01 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2HeroPower_01.prefab:c44d8017ffae8cf4e828d751a94eab5c");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2HeroPower_02.prefab:898f15f448bae1644a48af0f1c17334d");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_01 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_01.prefab:f7918814131d1eb48944e3a1cb791ff2");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_02 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_02.prefab:9585b601489e4e54ab83a2cec9f51ee0");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_03 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_03.prefab:d00ed71232615a141a82e2c1bdc7a7de");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Loss_01 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Loss_01.prefab:aecc46cfa733fd14eada7b4b3f68484f");
  private static readonly AssetReference VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission1Intro_02 = new AssetReference("VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission1Intro_02.prefab:4cb8d27be7ad7954d8525dec402b45f2");
  private List<string> m_InGame_BossIdle = new List<string>()
  {
    (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_01,
    (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_02,
    (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_03
  };
  private List<string> m_InGame_BossUsesHeroPower = new List<string>()
  {
    (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2HeroPower_01,
    (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2HeroPower_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();
  private Notification m_turnCounter;

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeA_Brukan_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeD_Brukan_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeF_Brukan_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeG_Brukan_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeA_Dawngrasp_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeD_Dawngrasp_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeF_Dawngrasp_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeG_Dawngrasp_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2Victory_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeA_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeB_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeB_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeC_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeD_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeF_03,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Brukan_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Brukan_03,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Dawngrasp_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Dawngrasp_03,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Rokara_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Rokara_03,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Tamsin_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Tamsin_03,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2Intro_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2Victory_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeA_Rokara_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeD_Rokara_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeF_Rokara_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeG_Rokara_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeA_Tamsin_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeD_Tamsin_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeF_Tamsin_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeG_Tamsin_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeG_Tamsin_04,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Death_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2EmoteResponse_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeC_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeE_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeF_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2HeroPower_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2HeroPower_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_02,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Idle_03,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Loss_01,
      (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission1Intro_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_BAR;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_03_Guff_Fight_02 bom03GuffFight02 = this;
    while (bom03GuffFight02.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
    switch (missionEvent)
    {
      case 100:
        if (!bom03GuffFight02.shouldPlayBanterVO())
          break;
        yield return (object) bom03GuffFight02.MissionPlayVOOnce(enemyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeE_01);
        break;
      case 101:
        GameState.Get().SetBusy(true);
        yield return (object) bom03GuffFight02.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeF_01);
        if (bom03GuffFight02.HeroPowerBrukan)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeF_Brukan_02);
        if (bom03GuffFight02.HeroPowerDawngrasp)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeF_Dawngrasp_02);
        if (bom03GuffFight02.HeroPowerRokara)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeF_Rokara_02);
        if (bom03GuffFight02.HeroPowerTamsin)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeF_Tamsin_02);
        GameState.Get().SetBusy(false);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) bom03GuffFight02.MissionPlayVO(actor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2Victory_01);
        if (bom03GuffFight02.HeroPowerDawngrasp)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2Victory_02);
        else
          yield return (object) bom03GuffFight02.MissionPlayVO(bom03GuffFight02.Dawngrasp_BrassRing, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2Victory_02);
        GameState.Get().SetBusy(false);
        break;
      case 506:
        GameState.Get().SetBusy(true);
        yield return (object) bom03GuffFight02.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 510:
        yield return (object) bom03GuffFight02.MissionPlayVO(enemyActor, bom03GuffFight02.m_InGame_BossUsesHeroPower);
        break;
      case 514:
        yield return (object) bom03GuffFight02.MissionPlayVO(actor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2Intro_01);
        yield return (object) bom03GuffFight02.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission1Intro_02);
        break;
      case 515:
        yield return (object) bom03GuffFight02.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2EmoteResponse_01);
        break;
      case 516:
        yield return (object) bom03GuffFight02.MissionPlaySound(enemyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2Death_01);
        break;
      case 517:
        yield return (object) bom03GuffFight02.MissionPlayVO(enemyActor, bom03GuffFight02.m_InGame_BossIdle);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) bom03GuffFight02.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_03_Guff_Fight_02 bom03GuffFight02 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) bom03GuffFight02.\u003C\u003En__1(entity);
    while (bom03GuffFight02.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!bom03GuffFight02.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) bom03GuffFight02.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      bom03GuffFight02.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BOM_03_Guff_Fight_02 bom03GuffFight02 = this;
    while (bom03GuffFight02.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!bom03GuffFight02.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) bom03GuffFight02.\u003C\u003En__2(entity);
      yield return (object) bom03GuffFight02.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      bom03GuffFight02.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BOM_03_Guff_Fight_02 bom03GuffFight02 = this;
    while (bom03GuffFight02.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        if (bom03GuffFight02.HeroPowerBrukan)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeA_Brukan_01);
        if (bom03GuffFight02.HeroPowerDawngrasp)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeA_Dawngrasp_01);
        if (bom03GuffFight02.HeroPowerRokara)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeA_Rokara_01);
        if (bom03GuffFight02.HeroPowerTamsin)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeA_Tamsin_01);
        yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeA_01);
        break;
      case 5:
        yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeB_01);
        yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeB_02);
        break;
      case 9:
        yield return (object) bom03GuffFight02.MissionPlayVO(actor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Vapos_Male_Elemental_Story_Guff_Mission2ExchangeC_01);
        yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeC_02);
        break;
      case 11:
        if (bom03GuffFight02.HeroPowerBrukan)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeD_Brukan_01);
        if (bom03GuffFight02.HeroPowerDawngrasp)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeD_Dawngrasp_01);
        if (bom03GuffFight02.HeroPowerRokara)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeD_Rokara_01);
        if (bom03GuffFight02.HeroPowerTamsin)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeD_Tamsin_01);
        yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeD_02);
        break;
      case 13:
        if (bom03GuffFight02.HeroPowerBrukan)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Brukan_01);
        if (bom03GuffFight02.HeroPowerDawngrasp)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Dawngrasp_01);
        if (bom03GuffFight02.HeroPowerTamsin)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Tamsin_01);
        if (bom03GuffFight02.HeroPowerRokara)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission2ExchangeG_Rokara_01);
        if (bom03GuffFight02.HeroPowerBrukan)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission2ExchangeG_Brukan_02);
        if (bom03GuffFight02.HeroPowerDawngrasp)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission2ExchangeG_Dawngrasp_02);
        if (bom03GuffFight02.HeroPowerRokara)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Rokara_02);
        if (bom03GuffFight02.HeroPowerTamsin)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeG_Tamsin_02);
        if (bom03GuffFight02.HeroPowerBrukan)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Brukan_03);
        if (bom03GuffFight02.HeroPowerDawngrasp)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Dawngrasp_03);
        if (bom03GuffFight02.HeroPowerRokara)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Rokara_03);
        if (bom03GuffFight02.HeroPowerTamsin)
          yield return (object) bom03GuffFight02.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission2ExchangeG_Tamsin_03);
        if (!bom03GuffFight02.HeroPowerTamsin)
          break;
        yield return (object) bom03GuffFight02.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_02.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission2ExchangeG_Tamsin_04);
        break;
    }
  }

  private void UpdateTurnCounterText(int cost) => this.m_turnCounter.ChangeDialogText(GameStrings.FormatPlurals("BOM_GUFF_02", new GameStrings.PluralNumber[1]
  {
    new GameStrings.PluralNumber()
    {
      m_index = 0,
      m_number = cost
    }
  }), cost.ToString(), "", "");

  private void InitTurnCounter(int cost)
  {
    this.m_turnCounter = AssetLoader.Get().InstantiatePrefab((AssetReference) "LOE_Turn_Timer.prefab:b05530aa55868554fb8f0c66632b3c22").GetComponent<Notification>();
    PlayMakerFSM component = this.m_turnCounter.GetComponent<PlayMakerFSM>();
    component.FsmVariables.GetFsmBool("RunningMan").Value = true;
    component.FsmVariables.GetFsmBool("MineCart").Value = false;
    component.FsmVariables.GetFsmBool("Airship").Value = false;
    component.FsmVariables.GetFsmBool("Destroyer").Value = false;
    component.SendEvent("Birth");
    this.m_turnCounter.transform.parent = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor().gameObject.transform;
    this.m_turnCounter.transform.localPosition = new Vector3(-1.4f, 0.187f, -0.11f);
    this.m_turnCounter.transform.localScale = Vector3.one * 0.52f;
    this.UpdateTurnCounterText(cost);
  }

  private void InitVisuals() => this.InitTurnCounter(this.GetCost());

  public override void NotifyOfMulliganEnded()
  {
    base.NotifyOfMulliganEnded();
    this.InitVisuals();
  }

  private void UpdateTurnCounter(int cost)
  {
    this.m_turnCounter.GetComponent<PlayMakerFSM>().SendEvent("Action");
    if (cost <= 0)
      Object.Destroy((Object) this.m_turnCounter.gameObject);
    else
      this.UpdateTurnCounterText(cost);
  }

  private void UpdateVisuals(int cost) => this.UpdateTurnCounter(cost);

  public override void OnTagChanged(TagDelta change)
  {
    base.OnTagChanged(change);
    if (change.tag != 48 || change.newValue == change.oldValue)
      return;
    this.UpdateVisuals(change.newValue);
  }
}
