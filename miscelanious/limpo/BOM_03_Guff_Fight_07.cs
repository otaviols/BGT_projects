using System.Collections;
using System.Collections.Generic;

public class BOM_03_Guff_Fight_07 : BOM_03_Guff_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission7ExchangeA_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission7ExchangeA_01.prefab:84b1d82b68cb74347a92c87ab78a04e9");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission7ExchangeC_Brukan_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission7ExchangeC_Brukan_01.prefab:327b88880551431488a8fdec11b70644");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission7ExchangeC_Dawngrasp_01 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission7ExchangeC_Dawngrasp_01.prefab:fc9e9a61bd8f1a54a88283dd1efc16dc");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7ExchangeB_02 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7ExchangeB_02.prefab:eebd57b5c56b88642a147443465f68e6");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7ExchangeD_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7ExchangeD_01.prefab:013a0e275a8a4f142be585e2241fc95f");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7Intro_02 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7Intro_02.prefab:91d50ca8166d4114dad5b2e6dd22b4b0");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Death_01 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Death_01.prefab:0cb325af761c01d4cb83b889b33fb061");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7EmoteResponse_01 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7EmoteResponse_01.prefab:008192737c3582e48bb0dae0d4b05382");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7ExchangeB_01 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7ExchangeB_01.prefab:6827d45dba179b0429d0b2007f0ce413");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_01 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_01.prefab:30d7f99ebb85b864497dd1c49e7d99f9");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_01_alt = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_01_alt.prefab:d60b207145209084cae996be491364cf");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_02 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_02.prefab:7a1b0b9daecef2444b01823a093f4906");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_03 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_03.prefab:a7518ede0264f5446a3f9cf932d30ec9");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Idle_01 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Idle_01.prefab:5b84c39a670647d4fa7ca8d2939a715b");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Idle_02 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Idle_02.prefab:c9b6ee59079b43842ad12454ad628272");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Intro_01 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Intro_01.prefab:39f9312ff0f9430408cf7a3a398a4bae");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Loss_01 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Loss_01.prefab:234cb7e606744ff4e9cb2cc359314d6a");
  private static readonly AssetReference VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Victory_01 = new AssetReference("VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Victory_01.prefab:9794e17d6e2206542850dc397b9ff535");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission7ExchangeC_Rokara_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission7ExchangeC_Rokara_01.prefab:ba6685e9c9ad1a9459630660819e58cb");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission7ExchangeA_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission7ExchangeA_02.prefab:f39f0766e1b9565469a60a353f281fe6");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission7ExchangeC_Tamsin_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission7ExchangeC_Tamsin_01.prefab:f7d8b0dcf1d601f44a1689796c962cde");
  private static readonly AssetReference VO_Story_Hero_VoidNaralex_Male_NightElf_Story_Guff_Mission7Victory_02 = new AssetReference("VO_Story_Hero_VoidNaralex_Male_NightElf_Story_Guff_Mission7Victory_02.prefab:060bcede5ee29734c8ebb88a7e3ccd5f");
  private List<string> m_InGame_BossIdle = new List<string>()
  {
    (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Idle_01,
    (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Idle_02
  };
  private List<string> m_InGame_BossUsesHeroPower = new List<string>()
  {
    (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_01,
    (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_01_alt,
    (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_02,
    (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission7ExchangeA_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission7ExchangeC_Brukan_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission7ExchangeC_Dawngrasp_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7ExchangeB_02,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7ExchangeD_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7Intro_02,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Death_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7EmoteResponse_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7ExchangeB_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_01_alt,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_02,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7HeroPower_03,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Idle_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Idle_02,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Intro_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Loss_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Victory_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission7ExchangeC_Rokara_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission7ExchangeA_02,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission7ExchangeC_Tamsin_01,
      (string) BOM_03_Guff_Fight_07.VO_Story_Hero_VoidNaralex_Male_NightElf_Story_Guff_Mission7Victory_02
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
    BOM_03_Guff_Fight_07 bom03GuffFight07 = this;
    while (bom03GuffFight07.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 505:
        GameState.Get().SetBusy(true);
        yield return (object) bom03GuffFight07.MissionPlayVO(bom03GuffFight07.Naralex_BrassRing, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_VoidNaralex_Male_NightElf_Story_Guff_Mission7Victory_02);
        GameState.Get().SetBusy(false);
        break;
      case 506:
        GameState.Get().SetBusy(true);
        yield return (object) bom03GuffFight07.MissionPlayVO(actor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 510:
        yield return (object) bom03GuffFight07.MissionPlayVO(actor, bom03GuffFight07.m_InGame_BossUsesHeroPower);
        break;
      case 514:
        yield return (object) bom03GuffFight07.MissionPlayVO(actor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Intro_01);
        yield return (object) bom03GuffFight07.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7Intro_02);
        break;
      case 515:
        yield return (object) bom03GuffFight07.MissionPlayVO(actor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7EmoteResponse_01);
        break;
      case 516:
        yield return (object) bom03GuffFight07.MissionPlaySound(actor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7Victory_01);
        break;
      case 517:
        yield return (object) bom03GuffFight07.MissionPlayVO(actor, bom03GuffFight07.m_InGame_BossIdle);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) bom03GuffFight07.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_03_Guff_Fight_07 bom03GuffFight07 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) bom03GuffFight07.\u003C\u003En__1(entity);
    while (bom03GuffFight07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!bom03GuffFight07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) bom03GuffFight07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      bom03GuffFight07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BOM_03_Guff_Fight_07 bom03GuffFight07 = this;
    while (bom03GuffFight07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!bom03GuffFight07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) bom03GuffFight07.\u003C\u003En__2(entity);
      yield return (object) bom03GuffFight07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      bom03GuffFight07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BOM_03_Guff_Fight_07 bom03GuffFight07 = this;
    while (bom03GuffFight07.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        if (bom03GuffFight07.HeroPowerBrukan)
          yield return (object) bom03GuffFight07.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission7ExchangeA_01);
        else
          yield return (object) bom03GuffFight07.MissionPlayVO(bom03GuffFight07.Brukan_BrassRing, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission7ExchangeA_01);
        if (bom03GuffFight07.HeroPowerTamsin)
        {
          yield return (object) bom03GuffFight07.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission7ExchangeA_02);
          break;
        }
        yield return (object) bom03GuffFight07.MissionPlayVO(bom03GuffFight07.Tamsin_BrassRing, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission7ExchangeA_02);
        break;
      case 5:
        yield return (object) bom03GuffFight07.MissionPlayVO(actor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Mutanus_Male_Murloc_Story_Guff_Mission7ExchangeB_01);
        yield return (object) bom03GuffFight07.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7ExchangeB_02);
        break;
      case 9:
        if (bom03GuffFight07.HeroPowerBrukan)
          yield return (object) bom03GuffFight07.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission7ExchangeC_Brukan_01);
        if (bom03GuffFight07.HeroPowerDawngrasp)
          yield return (object) bom03GuffFight07.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission7ExchangeC_Dawngrasp_01);
        if (bom03GuffFight07.HeroPowerRokara)
          yield return (object) bom03GuffFight07.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission7ExchangeC_Rokara_01);
        if (!bom03GuffFight07.HeroPowerTamsin)
          break;
        yield return (object) bom03GuffFight07.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission7ExchangeC_Tamsin_01);
        break;
      case 13:
        yield return (object) bom03GuffFight07.MissionPlayVO(friendlyActor, (string) BOM_03_Guff_Fight_07.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission7ExchangeD_01);
        break;
    }
  }
}
