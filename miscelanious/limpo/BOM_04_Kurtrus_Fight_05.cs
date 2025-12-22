using System.Collections;
using System.Collections.Generic;

public class BOM_04_Kurtrus_Fight_05 : BOM_04_Kurtrus_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeB_Cariel_01 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeB_Cariel_01.prefab:808ae39a92f8e3640b6cc56227e7923b");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeC_Cariel_01 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeC_Cariel_01.prefab:346e9a98ff34b8c4faa9800d923e1ef9");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeD_Cariel_01 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeD_Cariel_01.prefab:fa680f21a61aaa94597f9cbe3d475410");
  private static readonly AssetReference VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5Victory_02 = new AssetReference("VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5Victory_02.prefab:63e2ce956f9acf042855eff7b5cc5822");
  private static readonly AssetReference VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeA_01 = new AssetReference("VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeA_01.prefab:9eb9ce40d4db8b74099f2876ebe90cd3");
  private static readonly AssetReference VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Cariel_02 = new AssetReference("VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Cariel_02.prefab:91e375dfc1f3fa9478665cd694f51fdf");
  private static readonly AssetReference VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Scabbs_01 = new AssetReference("VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Scabbs_01.prefab:13b7ed9d385dd614296fb996f57e5367");
  private static readonly AssetReference VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Xyrella_02 = new AssetReference("VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Xyrella_02.prefab:0063d1eadc167984ca74233cd41d2cc6");
  private static readonly AssetReference VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeD_01 = new AssetReference("VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeD_01.prefab:c3ef6dc6e0897ba41b9e0074a825fc49");
  private static readonly AssetReference VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5Intro_02 = new AssetReference("VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5Intro_02.prefab:8aa5b2513ee07e74c81426b3eb4e762a");
  private static readonly AssetReference VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5Victory_01 = new AssetReference("VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5Victory_01.prefab:934fe98334b66054ab12cf3db5e0c8f7");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Death_01 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Death_01.prefab:2f624a257f5519f4691c621727ebfd8e");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5EmoteResponse_01 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5EmoteResponse_01.prefab:77a0407104fd79b4abbffbcc1cf26677");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeA_02 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeA_02.prefab:38343d8f31b11064dbf5be7c99ae5c52");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeB_Tavish_02 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeB_Tavish_02.prefab:cfc37ee3b88063d4c8662c81a18927bf");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeC_01 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeC_01.prefab:01285d0fd145dbf429abde3ff2491ab8");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_01 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_01.prefab:49b025d328fdb7543b86918b5759880e");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_02 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_02.prefab:8512e17f1ceeeca4fa1e5bdba2628dc1");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_03 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_03.prefab:3e05b49c386223d44a2927d1da84a5f1");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_01 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_01.prefab:830c452c2b181cc4e994a9a9ca60c5a1");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_02 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_02.prefab:9106d2ecbbde3694ead02b7344d9acc4");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_03 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_03.prefab:6107450710d40254daf7565760aab2be");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Intro_01 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Intro_01.prefab:b39a5b9b05809854aa3c3b64845952c3");
  private static readonly AssetReference VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Loss_01 = new AssetReference("VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Loss_01.prefab:f59867f317ba50e4c8856c340f4c8362");
  private static readonly AssetReference VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_Scabbs_01 = new AssetReference("VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_Scabbs_01.prefab:cf48b6004475aee418b0bcf6f5de4d65");
  private static readonly AssetReference VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_Scabbs_02 = new AssetReference("VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_Scabbs_02.prefab:b5f83ca1d63086c488d862afa7645100");
  private static readonly AssetReference VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeC_Scabbs_01 = new AssetReference("VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeC_Scabbs_01.prefab:61f6afa975dc0ea42aae93525c4f1d1e");
  private static readonly AssetReference VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeD_Scabbs_01 = new AssetReference("VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeD_Scabbs_01.prefab:36cb8c6bd347fdd439f8324e2770ba84");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeB_Tavish_01 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeB_Tavish_01.prefab:937d4aa2cf3ee5347aac6137731be753");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeC_Tavish_01 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeC_Tavish_01.prefab:b6fad5a9fd2d835459f63228aba42c37");
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeD_Tavish_01 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeD_Tavish_01.prefab:c42785ca5aa4493438331104c6e51a0e");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_Xyrella_01 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_Xyrella_01.prefab:fbf13dd63e76ac048ab45ff4a03666bc");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_Xyrella_03 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_Xyrella_03.prefab:bc0450e61a39e194798d24715eedc321");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeC_Xyrella_01 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeC_Xyrella_01.prefab:d14995179bd67764f8b7c860ff67b548");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeD_Xyrella_01 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeD_Xyrella_01.prefab:5aa95afaa95f9e542892c21dc25b4832");
  private List<string> m_missionEventTriggerInGame_VictoryPreExplosionLines = new List<string>()
  {
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5Victory_02,
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5Victory_01
  };
  private List<string> m_VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_ScabbsLines = new List<string>()
  {
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_Scabbs_01,
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_Scabbs_02
  };
  private List<string> m_VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_XyrellaLines = new List<string>()
  {
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_Xyrella_01,
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_Xyrella_03
  };
  private List<string> m_InGame_BossIdleLines = new List<string>()
  {
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_01,
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_02,
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_03
  };
  private List<string> m_InGame_BossHeroPowerLines = new List<string>()
  {
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_01,
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_02,
    (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeB_Cariel_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeC_Cariel_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeD_Cariel_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5Victory_02,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeA_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Cariel_02,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Scabbs_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Xyrella_02,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeD_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5Intro_02,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5Victory_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Death_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5EmoteResponse_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeA_02,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeB_Tavish_02,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeC_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_02,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5HeroPower_03,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_02,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Idle_03,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Intro_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Loss_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_Scabbs_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_Scabbs_02,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeC_Scabbs_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeD_Scabbs_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeB_Tavish_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeC_Tavish_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeD_Tavish_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_Xyrella_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_Xyrella_03,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeC_Xyrella_01,
      (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeD_Xyrella_01
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
    BOM_04_Kurtrus_Fight_05 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
    switch (missionEvent)
    {
      case 505:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5Victory_01);
        if (obj.CarielIsHeroPower)
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5Victory_02);
        else
          yield return (object) obj.MissionPlayVO(obj.Cariel_BrassRing_Quote, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5Victory_02);
        obj.MissionPause(false);
        break;
      case 506:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlayVO(actor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Loss_01);
        obj.MissionPause(false);
        break;
      case 510:
        yield return (object) obj.MissionPlayVO(actor, obj.m_InGame_BossHeroPowerLines);
        break;
      case 514:
        yield return (object) obj.MissionPlayVO(actor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Intro_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5Intro_02);
        break;
      case 515:
        yield return (object) obj.MissionPlayVO(actor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5EmoteResponse_01);
        break;
      case 516:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlaySound(actor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5Death_01);
        obj.MissionPause(false);
        break;
      case 517:
        yield return (object) obj.MissionPlayVO(actor, obj.m_InGame_BossIdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) obj.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_04_Kurtrus_Fight_05 obj = this;
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
    BOM_04_Kurtrus_Fight_05 obj = this;
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
    BOM_04_Kurtrus_Fight_05 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeA_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeA_02);
        break;
      case 5:
        if (obj.CarielIsHeroPower)
        {
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeB_Cariel_01);
          yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Cariel_02);
        }
        if (obj.ScabbsIsHeroPower)
        {
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_Scabbs_01);
          yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Scabbs_01);
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeB_Scabbs_02);
        }
        if (obj.TavishIsHeroPower)
        {
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeB_Tavish_01);
          yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeB_Tavish_02);
        }
        if (!obj.XyrellaIsHeroPower)
          break;
        yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_Xyrella_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeB_Xyrella_02);
        yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeB_Xyrella_03);
        break;
      case 9:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Rathorian_Male_Demon_Story_Kurtrus_Mission5ExchangeC_01);
        if (obj.CarielIsHeroPower)
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeC_Cariel_01);
        if (obj.ScabbsIsHeroPower)
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeC_Scabbs_01);
        if (obj.TavishIsHeroPower)
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeC_Tavish_01);
        if (!obj.XyrellaIsHeroPower)
          break;
        yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeC_Xyrella_01);
        break;
      case 13:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Kurtrus_Male_NightElf_Story_Kurtrus_Mission5ExchangeD_01);
        if (obj.CarielIsHeroPower)
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Cariel_Female_Human_Story_Kurtrus_Mission5ExchangeD_Cariel_01);
        if (obj.ScabbsIsHeroPower)
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Scabbs_Male_Gnome_Story_Kurtrus_Mission5ExchangeD_Scabbs_01);
        if (obj.TavishIsHeroPower)
          yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Tavish_Male_Dwarf_Story_Kurtrus_Mission5ExchangeD_Tavish_01);
        if (!obj.XyrellaIsHeroPower)
          break;
        yield return (object) obj.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_04_Kurtrus_Fight_05.VO_Story_Hero_Xyrella_Female_Draenei_Story_Kurtrus_Mission5ExchangeD_Xyrella_01);
        break;
    }
  }
}
