using System.Collections;
using System.Collections.Generic;

public class BOM_02_Xyrella_Fight_04 : BOM_02_Xyrella_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Tavish_Male_Dwarf_Story_Xyrella_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Tavish_Male_Dwarf_Story_Xyrella_Mission4Victory_01.prefab:3d73e4fcb44ec6a4bb96ced7a1bbe43f");
  private static readonly AssetReference VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Death_01 = new AssetReference("VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Death_01.prefab:3758c9bf126787241abe2afbc9e94c92");
  private static readonly AssetReference VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4EmoteResponse_01 = new AssetReference("VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4EmoteResponse_01.prefab:f2eb26fd2a8a88d44b05fbfe81cb725f");
  private static readonly AssetReference VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4HeroPower_01 = new AssetReference("VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4HeroPower_01.prefab:9d528ab2cd4b2f44b8915cbc9641ad1b");
  private static readonly AssetReference VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4HeroPower_02 = new AssetReference("VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4HeroPower_02.prefab:748679636a6ca9240a2fb81e3fbf71b7");
  private static readonly AssetReference VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_01 = new AssetReference("VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_01.prefab:ce2c98813ac6fe64a9d647d01a758e57");
  private static readonly AssetReference VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_02 = new AssetReference("VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_02.prefab:1311216202b233a489934d4cc3ceeb60");
  private static readonly AssetReference VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_03 = new AssetReference("VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_03.prefab:31c2763b1b883e3489f7d7225e7683ef");
  private static readonly AssetReference VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Intro_02 = new AssetReference("VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Intro_02.prefab:65650ced288819c42808354f51768fe4");
  private static readonly AssetReference VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Loss_01 = new AssetReference("VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Loss_01.prefab:0d47f72d52cf92e42a2573445fefb057");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4ExchangeA_02 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4ExchangeA_02.prefab:4655f1336d8b9a4419ab9365bb4d7160");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4ExchangeB_02 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4ExchangeB_02.prefab:5c8c1f43ccb393e4986cc4a9c799d552");
  private static readonly AssetReference VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4Intro_01 = new AssetReference("VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4Intro_01.prefab:dac9e8c0f05bf1246859e07edd07c096");
  private static readonly AssetReference VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeA_01 = new AssetReference("VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeA_01.prefab:0a45872bb9881fc4c933f41acdc7e5f3");
  private static readonly AssetReference VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeB_01 = new AssetReference("VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeB_01.prefab:b7dc4730e3c5b4c4595fc16122de4c5f");
  private static readonly AssetReference VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeC_01 = new AssetReference("VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeC_01.prefab:c2e6381189fd45c41b51d301072aa4d9");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4HeroPower_01,
    (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4HeroPower_02
  };
  private List<string> m_InGameBossIdleLines = new List<string>()
  {
    (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_01,
    (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_02,
    (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Tavish_Male_Dwarf_Story_Xyrella_Mission4Victory_01,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Death_01,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4EmoteResponse_01,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4HeroPower_01,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4HeroPower_02,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_01,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_02,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Idle_03,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Intro_02,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Loss_01,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4ExchangeA_02,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4ExchangeB_02,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4Intro_01,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeA_01,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeB_01,
      (string) BOM_02_Xyrella_Fight_04.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeC_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override List<string> GetBossIdleLines() => this.m_InGameBossIdleLines;

  public override void OnCreateGame()
  {
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_BAR;
    base.OnCreateGame();
    this.m_deathLine = (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Death_01;
    this.m_standardEmoteResponseLine = (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4EmoteResponse_01;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_02_Xyrella_Fight_04 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 505:
        GameState.Get().SetBusy(true);
        yield return (object) obj.MissionPlayVO("BOM_02_Tavish_01t", (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Tavish_Male_Dwarf_Story_Xyrella_Mission4Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 506:
        GameState.Get().SetBusy(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 514:
        yield return (object) obj.MissionPlayVO(actor, (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4Intro_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Intro_02);
        break;
      case 515:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4EmoteResponse_01);
        break;
      case 516:
        yield return (object) obj.MissionPlaySound(enemyActor, (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Whirleygig_Male_Goblin_Story_Xyrella_Mission4Death_01);
        break;
      case 517:
        yield return (object) obj.MissionPlayVO(enemyActor, obj.m_InGameBossIdleLines);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) obj.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BOM_02_Xyrella_Fight_04 obj = this;
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
    BOM_02_Xyrella_Fight_04 obj = this;
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
    BOM_02_Xyrella_Fight_04 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) obj.MissionPlayVO("BOM_02_Tavish_01t", (string) BOM_02_Xyrella_Fight_04.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeA_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4ExchangeA_02);
        break;
      case 5:
        yield return (object) obj.MissionPlayVO("BOM_02_Tavish_01t", (string) BOM_02_Xyrella_Fight_04.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeB_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_02_Xyrella_Fight_04.VO_Story_Hero_Xyrella_Female_Draenei_Story_Xyrella_Mission4ExchangeB_02);
        break;
      case 7:
        yield return (object) obj.MissionPlayVO("BOM_02_Tavish_01t", (string) BOM_02_Xyrella_Fight_04.VO_Story_Minion_Tavish_Male_Dwarf_Story_Xyrella_Mission4ExchangeC_01);
        break;
    }
  }
}
