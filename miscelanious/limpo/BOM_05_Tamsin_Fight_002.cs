using System.Collections;
using System.Collections.Generic;

public class BOM_05_Tamsin_Fight_002 : BOM_05_Tamsin_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Death_01 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Death_01.prefab:019ed3146ad7637499a0598475103bc5");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2EmoteResponse_01 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2EmoteResponse_01.prefab:f9408131fa2c2d0479cf18ed37b98639");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeA_02 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeA_02.prefab:3995ed5f2e5222343bafb50524be2c02");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeB_02 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeB_02.prefab:07112aa6332d7b845be316d7242df573");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeC_01 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeC_01.prefab:5133b9900e96cf640b2df9598d0f0140");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeD_01 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeD_01.prefab:5f44ea9a1fc3b3641a3ed5558dd08085");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeE_01 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeE_01.prefab:b01b25076e601d7489094c7e22ecf11b");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeF_01 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeF_01.prefab:d51e4b2a86f70684dbc873fd81cba54f");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_01 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_01.prefab:74cdf3149fe613544a3b1c176d214a1e");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_02.prefab:11f435e36f4c64248990dc3131b253c1");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_03 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_03.prefab:78646c74433741d449353bf7696858ca");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_04 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_04.prefab:5a495e9106825c946a27c79c95fac2d9");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_05 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_05.prefab:56abf6c599d6a4a46b97f618773bfa1c");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_06 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_06.prefab:e5bd7f45b579e944b93a41b75527f04c");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Idle_01 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Idle_01.prefab:f5091473b8e65db4987933f9d842a1c6");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Idle_02 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Idle_02.prefab:7caa2ccbcb731e449b58acc6e3581e16");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Idle_03 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Idle_03.prefab:d4ea510fb99e3b84b80bcf46b2b0eb0e");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Intro_02 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Intro_02.prefab:1ee57c4d06adefa4e939fa0227962cba");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Loss_01 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Loss_01.prefab:41dfde00623ee9b49babfd31c3c561cd");
  private static readonly AssetReference VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Victory_01 = new AssetReference("VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Victory_01.prefab:0ebcf52c77685ef4f8c69e9769e04cef");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeA_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeA_01.prefab:1fb49aca7a1b05c45959e3559b846d5d");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeB_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeB_01.prefab:50adacfb7a4815d47a955de07a8e870d");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeC_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeC_02.prefab:1ab19f9e9b354254eabea587421769b9");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeD_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeD_02.prefab:d7a4c970e8a33204ebefc569659ee1d3");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2Intro_01 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2Intro_01.prefab:5458c32076167ba4c89b2e4513806407");
  private static readonly AssetReference VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2Victory_02 = new AssetReference("VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2Victory_02.prefab:4aa8b383b6716bc469a25bef6de948f9");
  private static readonly AssetReference VO_PVPDR_Hero_Tamsin_Female_Forsaken_Death_01 = new AssetReference("VO_PVPDR_Hero_Tamsin_Female_Forsaken_Death_01.prefab:0261caf1dfbfae146bf927a03851b086");
  private List<string> m_InGame_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_01,
    (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_02,
    (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_03,
    (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_04,
    (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_05,
    (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_06
  };
  private List<string> m_InGame_BossIdleLines = new List<string>()
  {
    (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Idle_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Death_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2EmoteResponse_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeA_02,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeB_02,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeC_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeD_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeE_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeF_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_02,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_03,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_04,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_05,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2HeroPower_06,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Idle_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Idle_02,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Idle_03,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Intro_02,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Loss_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Victory_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeA_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeB_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeC_02,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeD_02,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2Intro_01,
      (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2Victory_02,
      (string) BOM_05_Tamsin_Fight_002.VO_PVPDR_Hero_Tamsin_Female_Forsaken_Death_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override void OnCreateGame() => base.OnCreateGame();

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BOM_05_Tamsin_Fight_002 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 504:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Victory_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2Victory_02);
        obj.MissionPause(false);
        break;
      case 507:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Loss_01);
        obj.MissionPause(false);
        break;
      case 510:
        yield return (object) obj.MissionPlayVO(enemyActor, obj.m_InGame_BossUsesHeroPowerLines);
        break;
      case 514:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2Intro_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2Intro_02);
        break;
      case 515:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2EmoteResponse_01);
        break;
      case 516:
        break;
      case 517:
        yield return (object) obj.MissionPlayVO(enemyActor, obj.m_InGame_BossIdleLines);
        break;
      case 519:
        obj.MissionPause(true);
        yield return (object) obj.MissionPlaySound(friendlyActor, (string) BOM_05_Tamsin_Fight_002.VO_PVPDR_Hero_Tamsin_Female_Forsaken_Death_01);
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
    BOM_05_Tamsin_Fight_002 obj = this;
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
    BOM_05_Tamsin_Fight_002 obj = this;
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
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CORE_CS2_077"))
      {
        if (cardId == "DRG_025")
          yield return (object) obj.MissionPlayVO(actor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeE_01);
      }
      else
        yield return (object) obj.MissionPlayVO(actor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeF_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BOM_05_Tamsin_Fight_002 obj = this;
    while (obj.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeA_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeA_02);
        break;
      case 5:
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeB_01);
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeB_02);
        break;
      case 9:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeC_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeC_02);
        break;
      case 11:
        yield return (object) obj.MissionPlayVO(enemyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Hooktusk_Female_Troll_BOM_Tamsin_Mission2ExchangeD_01);
        yield return (object) obj.MissionPlayVO(friendlyActor, (string) BOM_05_Tamsin_Fight_002.VO_Story_Hero_Tamsin_Female_Undead_BOM_Tamsin_Mission2ExchangeD_02);
        break;
    }
  }
}
