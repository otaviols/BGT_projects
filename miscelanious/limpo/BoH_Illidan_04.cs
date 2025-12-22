using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Illidan_04 : BoH_Illidan_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Illidan_04.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeA_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeA_01.prefab:f5a82cc3d576f0548a3bdeb3414f65ed");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeB_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeB_01.prefab:7933ab4569a9f714fb4b6ba0cbe05396");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_01.prefab:1a68008d92b374b40b7a4f6e22b3f9ad");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_03 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_03.prefab:289e7007346acd349b5a50f44fe6c410");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_06 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_06.prefab:d7db0ff7290a0d04e905e5b28d4a68fa");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeD_02 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeD_02.prefab:7a6196250be677f46b9de49eced169ed");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeE_02 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeE_02.prefab:d4ac4e882694a2c4d949a5e640ac989a");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4Intro_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4Intro_01.prefab:e1964478aae463e47b240f97887df418");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4Victory_02 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4Victory_02.prefab:f490c4f6718caf545832025510efe811");
  private static readonly AssetReference VO_Story_Hero_Maiev_Female_NightElf_Story_Illidan_Mission4ExchangeC_04 = new AssetReference("VO_Story_Hero_Maiev_Female_NightElf_Story_Illidan_Mission4ExchangeC_04.prefab:8a1dd1371c827ca4c9b145ff06ccf318");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4EmoteResponse_01 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4EmoteResponse_01.prefab:964098760308fed4abb95889c8d64ab3");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4ExchangeC_02 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4ExchangeC_02.prefab:cb4c7a7855ae2be42a83cda6e8edea32");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4ExchangeD_01 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4ExchangeD_01.prefab:1e4ca04814567944296bceb4139391bf");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_01 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_01.prefab:0209962e7f3ed9c46b14915f19d5a80b");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_02 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_02.prefab:a8d69c262d6eb8645ba9eb78e5e08dac");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_03 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_03.prefab:2375b5a5709100641940ca2be29668b2");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_01 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_01.prefab:18c9681b13c6e09449757fe858e57fa5");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_02 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_02.prefab:961f6254dedc91940b5354d5f2f0983d");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_03 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_03.prefab:5fe7cfa865ea38443b82e6c7e01153ac");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Intro_02 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Intro_02.prefab:61578e45906e8864f8de1c4ae92cac7f");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Loss_01 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Loss_01.prefab:6a3ad45a7f5d23c4190eaa40bd8b0c2e");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Victory_03 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Victory_03.prefab:c738a900c3a259747809b0c9f28f09ba");
  private static readonly AssetReference VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeE_01 = new AssetReference("VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeE_01.prefab:c10e34f7a28e496fbaf8888fdf894ab2");
  private static readonly AssetReference VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeE_03 = new AssetReference("VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeE_03.prefab:6a41f8964c9cef5468a7c762d1a80960");
  private static readonly AssetReference VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeF_01 = new AssetReference("VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeF_01.prefab:a773d6999289b6440a70fd15d7e3e622");
  private static readonly AssetReference VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeG_01 = new AssetReference("VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeG_01.prefab:66e843c98a89f2d43a0dc5f3154eb47f");
  private static readonly AssetReference VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4Victory_01.prefab:9b8b8d6530b247018eb7dbcedcb5fced");
  private static readonly AssetReference VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Death_01 = new AssetReference("VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Death_01.prefab:9dccc5c20783d30418ff1b06f432f6ad");
  private static readonly AssetReference VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4EmoteResponse_01 = new AssetReference("VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4EmoteResponse_01.prefab:b57352639607f314e94cbd51926775e2");
  private static readonly AssetReference VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_01 = new AssetReference("VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_01.prefab:8323ac222d0f9bc4aa9a405717f9721f");
  private static readonly AssetReference VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_02 = new AssetReference("VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_02.prefab:7c8c5d741ff6c8f4da16f241e4a1c2e7");
  private static readonly AssetReference VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_03 = new AssetReference("VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_03.prefab:da46300bd19f8bd4b9c2e8b6ee194d43");
  private static readonly AssetReference VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Loss_01 = new AssetReference("VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Loss_01.prefab:fe1a1dad440cdbf4bac25fc07ce55bbe");
  private static readonly AssetReference VO_Story_Hero_Velas_Male_Undead_Story_IllidanMission4Attack_01 = new AssetReference("VO_Story_Hero_Velas_Male_Undead_Story_IllidanMission4Attack_01.prefab:20dacb5666a7a3e47aa6ae0234d272b1");
  private static readonly AssetReference VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission4ExchangeC_05 = new AssetReference("VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission4ExchangeC_05.prefab:7dda5072364e940418fcd07b6d078ef8");
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      228,
      new string[1]{ "BOH_ILLIDAN_04b" }
    }
  };
  private Notification.SpeechBubbleDirection m_IllidanSpeechBubbleDirection = Notification.SpeechBubbleDirection.BottomRight;
  private float popUpScale = 1.25f;
  private Vector3 popUpPos;
  public static readonly AssetReference MalfurionBrassRing = new AssetReference("Malfurion_BrassRing_Quote.prefab:854afc33ad3808447935b8cb9753d3a8");
  public static readonly AssetReference TyrandeBrassRing = new AssetReference("YoungTyrande_Popup_BrassRing.prefab:79f13833a3f5e97449ef744f460e9fbd");
  public static readonly AssetReference MaievBrassRing = new AssetReference("Maiev_BrassRing_Quote.prefab:32a15dc6f5ca637499225d598df88188");
  public static readonly AssetReference KaelthasBrassRing = new AssetReference("Kaelthas_BrassRing_Quote.prefab:e2c98e804ab04dd49bfbd665c1647eca");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_01,
    (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_02,
    (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_01,
    (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_02,
    (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_03
  };
  private new List<string> m_BossIdleLinesCopy = new List<string>()
  {
    (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_01,
    (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_02,
    (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_03
  };
  private List<string> m_BossVelasIdleLines = new List<string>()
  {
    (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_01,
    (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_02,
    (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_03
  };
  private List<string> m_BossVelasIdleLinesCopy = new List<string>()
  {
    (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_01,
    (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_02,
    (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();
  private Notification m_turnCounter;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Illidan_04() => this.m_gameOptions.AddBooleanOptions(BoH_Illidan_04.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeA_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeB_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_03,
      (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_06,
      (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeD_02,
      (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeE_02,
      (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4Intro_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4Victory_02,
      (string) BoH_Illidan_04.VO_Story_Hero_Maiev_Female_NightElf_Story_Illidan_Mission4ExchangeC_04,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4EmoteResponse_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4ExchangeC_02,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4ExchangeD_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_02,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4HeroPower_03,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_02,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Idle_03,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Intro_02,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Loss_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Victory_03,
      (string) BoH_Illidan_04.VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeE_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeE_03,
      (string) BoH_Illidan_04.VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeF_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeG_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4Victory_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Death_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4EmoteResponse_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_02,
      (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Idle_03,
      (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Loss_01,
      (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_IllidanMission4Attack_01,
      (string) BoH_Illidan_04.VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission4ExchangeC_05
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_DRG;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Illidan_04 boHIllidan04 = this;
    while (boHIllidan04.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).SetName("");
    Gameplay.Get().GetNameBannerForSide(Player.Side.OPPOSING).UpdateHeroNameBanner();
    switch (missionEvent)
    {
      case 101:
        yield return (object) boHIllidan04.MissionPlayVOOnce(enemyActor, boHIllidan04.m_BossUsesHeroPowerLines);
        break;
      case 102:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan04.MissionPlayVO(friendlyActor, (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_01);
        yield return (object) boHIllidan04.MissionPlayVO(enemyActor, (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4ExchangeC_02);
        yield return (object) boHIllidan04.MissionPlayVO(friendlyActor, (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_03);
        yield return (object) boHIllidan04.MissionPlayVO(BoH_Illidan_04.MaievBrassRing, (string) BoH_Illidan_04.VO_Story_Hero_Maiev_Female_NightElf_Story_Illidan_Mission4ExchangeC_04);
        yield return (object) boHIllidan04.MissionPlayVO(BoH_Illidan_04.KaelthasBrassRing, (string) BoH_Illidan_04.VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission4ExchangeC_05);
        yield return (object) boHIllidan04.MissionPlayVO(friendlyActor, (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeC_06);
        GameState.Get().SetBusy(false);
        break;
      case 103:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan04.PlayLineAlways(friendlyActor, (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4ExchangeD_01);
        yield return (object) boHIllidan04.PlayLineAlways(friendlyActor, (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeD_02, boHIllidan04.m_IllidanSpeechBubbleDirection);
        GameState.Get().SetBusy(false);
        break;
      case 104:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan04.PlayLineAlways((string) BoH_Illidan_04.TyrandeBrassRing, (string) BoH_Illidan_04.VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeE_01);
        yield return (object) boHIllidan04.PlayLineAlways(friendlyActor, (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeE_02, boHIllidan04.m_IllidanSpeechBubbleDirection);
        yield return (object) boHIllidan04.PlayLineAlways((string) BoH_Illidan_04.TyrandeBrassRing, (string) BoH_Illidan_04.VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeE_03);
        GameState.Get().SetBusy(false);
        break;
      case 105:
        yield return (object) boHIllidan04.MissionPlayVO(BoH_Illidan_04.TyrandeBrassRing, (string) BoH_Illidan_04.VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeF_01);
        break;
      case 106:
        yield return (object) boHIllidan04.MissionPlayVO(BoH_Illidan_04.TyrandeBrassRing, (string) BoH_Illidan_04.VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4ExchangeG_01);
        break;
      case 228:
        yield return (object) new WaitForSeconds(2f);
        Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, boHIllidan04.popUpPos, TutorialEntity.GetTextScale() * boHIllidan04.popUpScale, GameStrings.Get(boHIllidan04.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
        NotificationManager.Get().DestroyNotification(popupText, 7.5f);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan04.PlayLineAlways((string) BoH_Illidan_04.TyrandeBrassRing, (string) BoH_Illidan_04.VO_Story_Hero_Tyrande_Female_NightElf_Story_Illidan_Mission4Victory_01);
        yield return (object) boHIllidan04.PlayLineAlways(friendlyActor, (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4Victory_02, boHIllidan04.m_IllidanSpeechBubbleDirection);
        yield return (object) boHIllidan04.PlayLineAlways(friendlyActor, (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Victory_03);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan04.MissionPlayVO(enemyActor, (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 508:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan04.MissionPlayVO(enemyActor, (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 514:
        yield return (object) boHIllidan04.MissionPlayVO(enemyActor, (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4Intro_02);
        yield return (object) boHIllidan04.MissionPlayVO(friendlyActor, (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4Intro_01);
        break;
      case 515:
        if (GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId() == "Story_10_Malfurion_004hb")
        {
          yield return (object) boHIllidan04.MissionPlayVO(enemyActor, (string) BoH_Illidan_04.VO_Story_Hero_Malfurion_Male_NightElf_Story_Illidan_Mission4EmoteResponse_01);
          break;
        }
        yield return (object) boHIllidan04.MissionPlayVO(enemyActor, (string) BoH_Illidan_04.VO_Story_Hero_Velas_Male_Undead_Story_Illidan_Mission4EmoteResponse_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHIllidan04.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  public override Notification.SpeechBubbleDirection GetEmoteDirectionOverride(
    EmoteType emoteType)
  {
    string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    return cardId == "Story_10_Velas_004hb" && (emoteType == EmoteType.THANKS || emoteType == EmoteType.WELL_PLAYED || emoteType == EmoteType.GREETINGS) ? Notification.SpeechBubbleDirection.BottomRight : Notification.SpeechBubbleDirection.None;
  }

  public override IEnumerator OnPlayThinkEmoteWithTiming()
  {
    BoH_Illidan_04 boHIllidan04 = this;
    if (!boHIllidan04.m_enemySpeaking)
    {
      Player currentPlayer = GameState.Get().GetCurrentPlayer();
      if (currentPlayer.IsFriendlySide() && !currentPlayer.GetHeroCard().HasActiveEmoteSound())
      {
        string cardId = GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId();
        if ((double) boHIllidan04.GetThinkEmoteBossIdleChancePercentage() > (double) Random.Range(0.0f, 1f) || !boHIllidan04.m_Mission_FriendlyPlayIdleLines && boHIllidan04.m_Mission_EnemyPlayIdleLines)
        {
          Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
          if (cardId == "Story_10_Malfurion_004hb")
          {
            string line = boHIllidan04.PopRandomLine(boHIllidan04.m_BossIdleLinesCopy);
            if (boHIllidan04.m_BossIdleLinesCopy.Count == 0)
              boHIllidan04.m_BossIdleLinesCopy = new List<string>((IEnumerable<string>) boHIllidan04.m_BossIdleLines);
            yield return (object) boHIllidan04.MissionPlayVO(actor, line);
          }
          else if (cardId == "Story_10_Velas_004hb")
          {
            string line = boHIllidan04.PopRandomLine(boHIllidan04.m_BossVelasIdleLinesCopy);
            if (boHIllidan04.m_BossVelasIdleLinesCopy.Count == 0)
              boHIllidan04.m_BossVelasIdleLinesCopy = new List<string>((IEnumerable<string>) boHIllidan04.m_BossVelasIdleLines);
            yield return (object) boHIllidan04.MissionPlayVO(actor, line);
          }
        }
        else if (boHIllidan04.m_Mission_FriendlyPlayIdleLines)
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
          GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(emoteType).GetActiveAudioSource();
        }
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Illidan_04 boHIllidan04 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHIllidan04.\u003C\u003En__1(entity);
    while (boHIllidan04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHIllidan04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHIllidan04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHIllidan04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Illidan_04 boHIllidan04 = this;
    while (boHIllidan04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHIllidan04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHIllidan04.\u003C\u003En__2(entity);
      yield return (object) boHIllidan04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHIllidan04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Illidan_04 boHIllidan04 = this;
    while (boHIllidan04.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) boHIllidan04.MissionPlayVO(actor, (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeA_01);
        break;
      case 5:
        yield return (object) boHIllidan04.MissionPlayVO(actor, (string) BoH_Illidan_04.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission4ExchangeB_01);
        break;
    }
  }

  protected IEnumerator PlayLineAlways(
    Actor speaker,
    string line,
    Notification.SpeechBubbleDirection direction,
    float duration = 2.5f)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Illidan_04 boHIllidan04 = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) boHIllidan04.PlayLine(speaker, line, new MissionEntity.ShouldPlay(((MissionEntity) boHIllidan04).InternalShouldPlayBossLine), duration, direction);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  protected IEnumerator PlayLine(
    Actor speaker,
    string line,
    MissionEntity.ShouldPlay shouldPlay,
    float duration,
    Notification.SpeechBubbleDirection direction)
  {
    BoH_Illidan_04 boHIllidan04 = this;
    if (boHIllidan04.m_enemySpeaking)
      yield return (object) null;
    boHIllidan04.m_enemySpeaking = true;
    if (boHIllidan04.m_forceAlwaysPlayLine)
      yield return (object) GameEntity.Coroutines.StartCoroutine(boHIllidan04.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
    else if (shouldPlay() == MissionEntity.ShouldPlayValue.Always)
      yield return (object) GameEntity.Coroutines.StartCoroutine(boHIllidan04.PlaySoundAndBlockSpeech(line, direction, speaker, duration));
    else if (shouldPlay() == MissionEntity.ShouldPlayValue.Once)
      yield return (object) GameEntity.Coroutines.StartCoroutine(boHIllidan04.PlaySoundAndBlockSpeechOnce(line, direction, speaker, duration));
    NotificationManager.Get().ForceAddSoundToPlayedList(line);
    boHIllidan04.m_enemySpeaking = false;
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

  private void UpdateVisuals(int cost) => this.UpdateTurnCounter(cost);

  private void UpdateTurnCounter(int cost)
  {
    this.m_turnCounter.GetComponent<PlayMakerFSM>().SendEvent("Action");
    if (cost <= 0)
      Object.Destroy((Object) this.m_turnCounter.gameObject);
    else
      this.UpdateTurnCounterText(cost);
  }

  private void UpdateTurnCounterText(int cost) => this.m_turnCounter.ChangeDialogText(GameStrings.FormatPlurals("BOH_ILLIDAN_04", new GameStrings.PluralNumber[1]
  {
    new GameStrings.PluralNumber()
    {
      m_index = 0,
      m_number = cost
    }
  }), cost.ToString(), "", "");

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ICCLichKing);
}
