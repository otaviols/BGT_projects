using System.Collections;
using System.Collections.Generic;

public class BOM_03_Guff_Fight_06 : BOM_03_Guff_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Death_01 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Death_01.prefab:c7b01c316c80c6a4c8d2938a5c81159c");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6EmoteResponse_01 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6EmoteResponse_01.prefab:7e58f5bf25f72954f81e227ac51b7cf0");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeA_01 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeA_01.prefab:4d5c53bf5b5f6a246aa00f2a1b6a5190");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeB_01 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeB_01.prefab:60bdd8ed1d2fce643a46556fe20ffe07");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeB_02 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeB_02.prefab:1350ad587f6b5cb4d9aec95e78003eb8");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeC_01 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeC_01.prefab:cfbb53804f648a54ea764a33ff4ac769");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeC_02 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeC_02.prefab:0c39b1de3ae27df46ba78d05c61824ef");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeD_01 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeD_01.prefab:94190167b4651564da952a7e481684f0");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6HeroPower_01 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6HeroPower_01.prefab:75dc94ef2217bf743a84848508e60048");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Idle_01 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Idle_01.prefab:53be7af3670453f44b3bacc948f22814");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Idle_02 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Idle_02.prefab:8dbc28d9f58be3a47b36311534d7068b");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Intro_02 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Intro_02.prefab:757ac176823ba3545bbcc84be3756a4e");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Loss_01 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Loss_01.prefab:cd2f19d8dd10d3e4c9ecdd522ab6cc1d");
  private static readonly AssetReference VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Victory_01 = new AssetReference("VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Victory_01.prefab:28270276c56053f4180b88411e9a2cfe");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission6ExchangeD_Brukan_02 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission6ExchangeD_Brukan_02.prefab:6fe17bb476aa09d4eb5dc1f350f13a53");
  private static readonly AssetReference VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission6ExchangeE_Brukan_01 = new AssetReference("VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission6ExchangeE_Brukan_01.prefab:1e36ce2ca9d3edb43b2bf27fea634c34");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission6ExchangeD_Dawngrasp_02 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission6ExchangeD_Dawngrasp_02.prefab:145435b33a574e6409fe340596001524");
  private static readonly AssetReference VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission6ExchangeE_Dawngrasp_01 = new AssetReference("VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission6ExchangeE_Dawngrasp_01.prefab:84b1524d0976c564cb22d41961da6cd6");
  private static readonly AssetReference VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission6Intro_01 = new AssetReference("VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission6Intro_01.prefab:8b30d639b829c05449276cdadbd506da");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission6ExchangeD_Rokara_02 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission6ExchangeD_Rokara_02.prefab:351dbf5ec39bb15469b03492e4b338b3");
  private static readonly AssetReference VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission6ExchangeE_Rokara_01 = new AssetReference("VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission6ExchangeE_Rokara_01.prefab:657f4989ad6b8034abf3b7922c398241");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission6ExchangeD_Tamsin_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission6ExchangeD_Tamsin_02.prefab:c4bad0adfb13aca45a3dcb201cbf61ea");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission6ExchangeE_Tamsin_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission6ExchangeE_Tamsin_01.prefab:386b737ab7275514086362e5a9cc2c90");
  private List<string> m_InGame_BossIdle = new List<string>()
  {
    (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Idle_01,
    (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Idle_02
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Death_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6EmoteResponse_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeA_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeB_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeB_02,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeC_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeC_02,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeD_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6HeroPower_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Idle_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Idle_02,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Intro_02,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Loss_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Victory_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission6ExchangeD_Brukan_02,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission6ExchangeE_Brukan_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission6ExchangeD_Dawngrasp_02,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission6ExchangeE_Dawngrasp_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission6Intro_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission6ExchangeD_Rokara_02,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission6ExchangeE_Rokara_01,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission6ExchangeD_Tamsin_02,
      (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission6ExchangeE_Tamsin_01
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
    BOM_03_Guff_Fight_06 bom03GuffFight06 = this;
    while (bom03GuffFight06.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 506:
        GameState.Get().SetBusy(true);
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 510:
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6HeroPower_01);
        break;
      case 514:
        yield return (object) bom03GuffFight06.MissionPlayVO(actor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Guff_Male_Tauren_Story_Guff_Mission6Intro_01);
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Intro_02);
        break;
      case 515:
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6EmoteResponse_01);
        break;
      case 516:
        yield return (object) bom03GuffFight06.MissionPlaySound(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6Death_01);
        break;
      case 517:
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, bom03GuffFight06.m_InGame_BossIdle);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) bom03GuffFight06.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_03_Guff_Fight_06 bom03GuffFight06 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) bom03GuffFight06.\u003C\u003En__1(entity);
    while (bom03GuffFight06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!bom03GuffFight06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) bom03GuffFight06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      bom03GuffFight06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BOM_03_Guff_Fight_06 bom03GuffFight06 = this;
    while (bom03GuffFight06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!bom03GuffFight06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) bom03GuffFight06.\u003C\u003En__2(entity);
      yield return (object) bom03GuffFight06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      bom03GuffFight06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BOM_03_Guff_Fight_06 bom03GuffFight06 = this;
    while (bom03GuffFight06.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyHeroPowerActor = GameState.Get().GetFriendlySidePlayer().GetHeroPower().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeA_01);
        break;
      case 7:
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeB_01);
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeB_02);
        break;
      case 11:
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeC_01);
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeC_02);
        break;
      case 15:
        yield return (object) bom03GuffFight06.MissionPlayVO(enemyActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Anacondra_Female_NightElf_Story_Guff_Mission6ExchangeD_01);
        if (bom03GuffFight06.HeroPowerBrukan)
          yield return (object) bom03GuffFight06.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission6ExchangeD_Brukan_02);
        if (bom03GuffFight06.HeroPowerDawngrasp)
          yield return (object) bom03GuffFight06.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission6ExchangeD_Dawngrasp_02);
        if (bom03GuffFight06.HeroPowerRokara)
          yield return (object) bom03GuffFight06.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission6ExchangeD_Rokara_02);
        if (!bom03GuffFight06.HeroPowerTamsin)
          break;
        yield return (object) bom03GuffFight06.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission6ExchangeD_Tamsin_02);
        break;
      case 19:
        if (bom03GuffFight06.HeroPowerBrukan)
          yield return (object) bom03GuffFight06.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Brukan_Male_Troll_Story_Guff_Mission6ExchangeE_Brukan_01);
        if (bom03GuffFight06.HeroPowerDawngrasp)
          yield return (object) bom03GuffFight06.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Dawngrasp_X_BloodElf_Story_Guff_Mission6ExchangeE_Dawngrasp_01);
        if (bom03GuffFight06.HeroPowerRokara)
          yield return (object) bom03GuffFight06.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Rokara_Female_Orc_Story_Guff_Mission6ExchangeE_Rokara_01);
        if (!bom03GuffFight06.HeroPowerTamsin)
          break;
        yield return (object) bom03GuffFight06.MissionPlayVO(friendlyHeroPowerActor, (string) BOM_03_Guff_Fight_06.VO_Story_Hero_Tamsin_Female_Forsaken_Story_Guff_Mission6ExchangeE_Tamsin_01);
        break;
    }
  }
}
