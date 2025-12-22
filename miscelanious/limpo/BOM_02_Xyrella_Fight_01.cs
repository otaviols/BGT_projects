using System.Collections;
using System.Collections.Generic;

public class BOM_02_Xyrella_Fight_01 : BOM_02_Xyrella_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1EmoteResponse_01 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1EmoteResponse_01.prefab:1fe368044aa8b4e4cac006302f30302b");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1ExchangeA_02 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1ExchangeA_02.prefab:48772f7bb231606469500a146063c6fb");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1ExchangeC_01 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1ExchangeC_01.prefab:dc542ec51bdbc97498754dec4125394b");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_01 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_01.prefab:cef6e200853b4374f97ca0abaf54694d");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_02 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_02.prefab:f7bab445b60dfac46905a4ee134efc3b");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_03 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_03.prefab:520f3c3ac227ecb4086fe5fb668277c8");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_01 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_01.prefab:1d5d92623344da240a1dc70c08a85fdf");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_02 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_02.prefab:a12f277c44a46c3418262551a8c9695e");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_03 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_03.prefab:842f91c36e776014ea7e4e605c588542");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Intro_02 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Intro_02.prefab:d9a54719e142d7545bf9a2358298c272");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Loss_01 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Loss_01.prefab:43da8e9cd3932ec4597363d50040f58c");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Victory_01 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Victory_01.prefab:1f24bd498b2325e4e8e8c1026371cc9a");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeB_01 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeB_01.prefab:ae51ed81d3f055049a524ef7beead920");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeB_03 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeB_03.prefab:663c3d4894946e94cab64ae4a93d4475");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeC_02 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeC_02.prefab:6d129dc44301efa4c8835bab75cf66dc");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeD_01 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeD_01.prefab:7fc7ecedc87115b4db95ca22f4519d39");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1Intro_01 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1Intro_01.prefab:db2d10b0c6deaae49897441b8340acb5");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1Victory_02 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1Victory_02.prefab:9a204822c5ab84c45bd792cd9742a61a");
  private static readonly AssetReference VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeA_01 = new AssetReference("VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeA_01.prefab:40e8594fdb81b604eb525c33c032712d");
  private static readonly AssetReference VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeB_02 = new AssetReference("VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeB_02.prefab:d4739b1eded50e64a8e50e2eb4fca707");
  private static readonly AssetReference VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeD_02 = new AssetReference("VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeD_02.prefab:02e9fc250f1188249a9cbab31f0a21a1");
  private static readonly AssetReference VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Death_01 = new AssetReference("VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Death_01.prefab:22f0d0660ef24254e9bf74a49d455632");
  private List<string> m_InGame_BossUsesHeroPower = new List<string>()
  {
    (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_01,
    (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_02,
    (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_03
  };
  private List<string> m_InGame_BossIdleLines = new List<string>()
  {
    (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_01,
    (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_02,
    (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1EmoteResponse_01,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1ExchangeA_02,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1ExchangeC_01,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_01,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_02,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1HeroPower_03,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_01,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_02,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Idle_03,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Intro_02,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Loss_01,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Victory_01,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeB_01,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeB_03,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeC_02,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeD_01,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1Intro_01,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1Victory_02,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeA_01,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeB_02,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeD_02,
      (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetBossIdleLines() => this.m_InGame_BossIdleLines;

  public override void OnCreateGame()
  {
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_BAR;
    base.OnCreateGame();
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_02_Xyrella_Fight_01 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Victory_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1Victory_02);
        GameState.Get().SetBusy(false);
        break;
      case 506:
        GameState.Get().SetBusy(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 510:
        yield return (object) obj.MissionPlayVO(enemyActor, obj.m_InGame_BossUsesHeroPower);
        break;
      case 514:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1Intro_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Intro_02);
        break;
      case 515:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1EmoteResponse_01);
        break;
      case 516:
        yield return (object) obj.MissionPlaySound(enemyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1Death_01);
        break;
      case 517:
        yield return (object) obj.MissionPlayVO(enemyActor, obj.m_InGame_BossIdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) obj.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_02_Xyrella_Fight_01 obj = this;
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
    BOM_02_Xyrella_Fight_01 obj = this;
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
    BOM_02_Xyrella_Fight_01 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) obj.MissionPlayVO("BOM_02_Tavish_01t", (string) BOM_02_Xyrella_Fight_01.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeA_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1ExchangeA_02);
        break;
      case 5:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeB_01);
        yield return (object) obj.MissionPlayVO("BOM_02_Tavish_01t", (string) BOM_02_Xyrella_Fight_01.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeB_02);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeB_03);
        break;
      case 9:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Kargal_Male_Orc_Story_Xyrella_Mission1ExchangeC_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeC_02);
        break;
      case 11:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_02_Xyrella_Fight_01.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission1ExchangeD_01);
        yield return (object) obj.MissionPlayVO("BOM_02_Tavish_01t", (string) BOM_02_Xyrella_Fight_01.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission1ExchangeD_02);
        break;
    }
  }
}
