using System.Collections;
using System.Collections.Generic;

public class BOM_01_Rokara_04 : BoM_01_Rokara_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeA_Brukan_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeA_Brukan_01.prefab:805a8c8f198a5c547adbc59d31cf459e");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeB_Brukan_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeB_Brukan_01.prefab:7b900d515f4a2654f89f00123dc828e2");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeC_Brukan_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeC_Brukan_01.prefab:d31cf1ee39e8c564cb216fa3d0121a47");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_01.prefab:c23831d5ddd01854299b41564c24f061");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_02 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_02.prefab:5fc92da67f8fd08498c75ce0b3e687d2");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeC_01 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeC_01.prefab:e92fbf563f6516749a0f736ee0ea4cec");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeD_01 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeD_01.prefab:2b4a1f10238bc1b4594e0dbcb7fa3245");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeD_02 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeD_02.prefab:a9c0db212d4457d42a3cfb7537f9ab16");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeA_Guff_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeA_Guff_01.prefab:a1f0a09f4a57d6447a13eb34a5444da0");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeB_Guff_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeB_Guff_01.prefab:f89c7961b989d904f958453d6531b12b");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeC_Guff_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeC_Guff_01.prefab:c35f1844a741a0948be6e1be14482039");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_01.prefab:fbb0d510b459b5b4f97b696fe6099218");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Brukan_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Brukan_02.prefab:4b6ef583a84bfb84dbbd3c82adb6f78d");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Guff_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Guff_02.prefab:e3990233feb46034c8c7ea28d4362c4a");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Tamsin_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Tamsin_02.prefab:9aa7efe0d9bdeb3418d4bba6fb8b39d9");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeD_03 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeD_03.prefab:a3c58fdd8d4c16542a1f31373f640e79");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4Intro_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4Intro_01.prefab:7cd3ba0bb58ed054ebdb10596c445735");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeA_Tamsin_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeA_Tamsin_01.prefab:a62626585e2215f4b81087a0dc1cf9c9");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeB_Tamsin_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeB_Tamsin_01.prefab:73effbfabf8c6e44eb1f36aa08136aba");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeC_Tamsin_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeC_Tamsin_01.prefab:cfaee2253ea461e4e893a00001c0a6d3");
  private static readonly AssetReference UNG_010_SatedThreshadon_Attack = new AssetReference("UNG_010_SatedThreshadon_Attack.prefab:4debdb75af9a8374abb65325b7a6602c");
  private static readonly AssetReference UNG_010_SatedThreshadon_Death = new AssetReference("UNG_010_SatedThreshadon_Death.prefab:d56a39571f3426e4daabaf0138df21a0");
  private static readonly AssetReference UNG_010_SatedThreshadon_Play = new AssetReference("UNG_010_SatedThreshadon_Play.prefab:b24069ee423dca6468dc095fd1a47e9a");
  private List<string> m_missionEventTriggerInGame_VictoryPostExplosionLines = new List<string>()
  {
    (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_01,
    (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_02
  };
  private List<string> m_VO_Story_Hero_Dawngrasp_NB_BloodElf_Story_Rokara_Mission4ExchangeDLines = new List<string>()
  {
    (string) BOM_01_Rokara_04.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeD_01,
    (string) BOM_01_Rokara_04.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeD_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeA_Brukan_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeB_Brukan_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeC_Brukan_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_02,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeC_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeD_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeD_02,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeA_Guff_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeB_Guff_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeC_Guff_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Brukan_02,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Guff_02,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Tamsin_02,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeD_03,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4Intro_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeA_Tamsin_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeB_Tamsin_01,
      (string) BOM_01_Rokara_04.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeC_Tamsin_01,
      (string) BOM_01_Rokara_04.UNG_010_SatedThreshadon_Attack,
      (string) BOM_01_Rokara_04.UNG_010_SatedThreshadon_Death,
      (string) BOM_01_Rokara_04.UNG_010_SatedThreshadon_Play
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_BAR;
    this.m_deathLine = (string) BOM_01_Rokara_04.UNG_010_SatedThreshadon_Death;
    this.m_standardEmoteResponseLine = (string) BOM_01_Rokara_04.UNG_010_SatedThreshadon_Attack;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_01_Rokara_04 bom01Rokara04 = this;
    while (bom01Rokara04.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
    switch (missionEvent)
    {
      case 100:
        yield return (object) bom01Rokara04.MissionPlayVO("BOM_01_Dawngrasp_04t", bom01Rokara04.Dawngrasp_BrassRing, (string) BOM_01_Rokara_04.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeC_01);
        if (bom01Rokara04.HeroPowerIsBrukan)
          yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeC_Brukan_01);
        if (bom01Rokara04.HeroPowerIsGuff)
          yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeC_Guff_01);
        if (!bom01Rokara04.HeroPowerIsTamsin)
          break;
        yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeC_Tamsin_01);
        break;
      case 101:
        yield return (object) bom01Rokara04.MissionPlayVO("BOM_01_Dawngrasp_04t", bom01Rokara04.Dawngrasp_BrassRing, (string) BOM_01_Rokara_04.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeD_01);
        yield return (object) bom01Rokara04.MissionPlayVO("BOM_01_Dawngrasp_04t", bom01Rokara04.Dawngrasp_BrassRing, (string) BOM_01_Rokara_04.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Rokara_Mission4ExchangeD_02);
        yield return (object) bom01Rokara04.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeD_03);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        if (bom01Rokara04.HeroPowerIsBrukan)
        {
          yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_01);
          yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_02);
        }
        else
        {
          yield return (object) bom01Rokara04.MissionPlayVO(bom01Rokara04.Brukan_BrassRing, (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_01);
          yield return (object) bom01Rokara04.MissionPlayVO(bom01Rokara04.Brukan_BrassRing, (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4Victory_02);
        }
        GameState.Get().SetBusy(false);
        break;
      case 506:
        yield return (object) bom01Rokara04.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_04.UNG_010_SatedThreshadon_Attack);
        break;
      case 514:
        yield return (object) bom01Rokara04.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4Intro_01);
        yield return (object) bom01Rokara04.MissionPlayVO(enemyActor, (string) BOM_01_Rokara_04.UNG_010_SatedThreshadon_Play);
        break;
      case 515:
        yield return (object) bom01Rokara04.MissionPlayVO(enemyActor, bom01Rokara04.m_standardEmoteResponseLine);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) bom01Rokara04.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_01_Rokara_04 bom01Rokara04 = this;
    while (bom01Rokara04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    // ISSUE: reference to a compiler-generated method
    yield return (object) bom01Rokara04.\u003C\u003En__1(entity);
    if (!bom01Rokara04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) bom01Rokara04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      bom01Rokara04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BOM_01_Rokara_04 bom01Rokara04 = this;
    while (bom01Rokara04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!bom01Rokara04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) bom01Rokara04.\u003C\u003En__2(entity);
      yield return (object) bom01Rokara04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      bom01Rokara04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BOM_01_Rokara_04 bom01Rokara04 = this;
    while (bom01Rokara04.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) bom01Rokara04.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_01);
        if (bom01Rokara04.HeroPowerIsBrukan)
        {
          yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeA_Brukan_01);
          yield return (object) bom01Rokara04.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Brukan_02);
        }
        if (bom01Rokara04.HeroPowerIsGuff)
        {
          yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeA_Guff_01);
          yield return (object) bom01Rokara04.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Guff_02);
        }
        if (!bom01Rokara04.HeroPowerIsTamsin)
          break;
        yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeA_Tamsin_01);
        yield return (object) bom01Rokara04.MissionPlayVO(friendlyActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Rokara_Female_Orc_Story_Rokara_Mission4ExchangeA_Tamsin_02);
        break;
      case 7:
        if (bom01Rokara04.HeroPowerIsBrukan)
          yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Brukan_Male_Troll_Story_Rokara_Mission4ExchangeB_Brukan_01);
        if (bom01Rokara04.HeroPowerIsGuff)
          yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Guff_Male_Tauren_Story_Rokara_Mission4ExchangeB_Guff_01);
        if (!bom01Rokara04.HeroPowerIsTamsin)
          break;
        yield return (object) bom01Rokara04.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_01_Rokara_04.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Rokara_Mission4ExchangeB_Tamsin_01);
        break;
    }
  }
}
