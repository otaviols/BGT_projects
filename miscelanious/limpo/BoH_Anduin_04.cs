using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Anduin_04 : BoH_Anduin_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Anduin_04.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeA_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeA_02.prefab:ae5f2899f8b49264f8941693121789d1");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeB_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeB_02.prefab:57035c975aac3634fbabee05bd2bfad5");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeC_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeC_02.prefab:f82248615609ea14f838ddd7f7fe4135");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4Intro_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4Intro_02.prefab:0c2129a03c27fe94ca40820ec0f388b0");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4Victory_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4Victory_02.prefab:ad84d064db74aa74386d7242bf7501e0");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4EmoteResponse_01 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4EmoteResponse_01.prefab:d6c339d5b9c66e845b4a8f2eb101a1c9");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeA_01 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeA_01.prefab:d6920c5f05cf06740892df2d691015f0");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeB_01 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeB_01.prefab:f9ae865db21f3084ea1eee5183e2dc2f");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeB_03 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeB_03.prefab:8f2527609dc832149be5b84e4cfae775");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeC_01 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeC_01.prefab:5f1540d482260404a86189779bec6989");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeC_03 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeC_03.prefab:10090a19ddb50c54ca7637c9ef440348");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_01 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_01.prefab:e4967dd3bb6552f48a2e33178227e572");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_02 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_02.prefab:2a4ee7df84637f0459370e5298116fdb");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_03 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_03.prefab:1531b88cbf9f6534fa780c210ce6e826");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_01 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_01.prefab:48b2044506f6a0a4c94dac5f518aba89");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_02 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_02.prefab:fd223d6d9f224ba49938d316971902c5");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_03 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_03.prefab:2778000859c493b40b1fa70a2b026e30");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Intro_01 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Intro_01.prefab:c123e6db3506ec647859edcbdf018be8");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Loss_01 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Loss_01.prefab:1f7b699d54d522c41a9d441330821b56");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Victory_01.prefab:d2b24436384676f499cd5fa5d7a287f6");
  private static readonly AssetReference VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Victory_02 = new AssetReference("VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Victory_02.prefab:bc33fbd89561db64dac6c2273f1455b5");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_01,
    (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_02,
    (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_01,
    (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_02,
    (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Anduin_04() => this.m_gameOptions.AddBooleanOptions(BoH_Anduin_04.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Anduin_04.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeA_02,
      (string) BoH_Anduin_04.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeB_02,
      (string) BoH_Anduin_04.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeC_02,
      (string) BoH_Anduin_04.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4Intro_02,
      (string) BoH_Anduin_04.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4Victory_02,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4EmoteResponse_01,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeA_01,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeB_01,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeB_03,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeC_01,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeC_03,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_01,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_02,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4HeroPower_03,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_01,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_02,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Idle_03,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Intro_01,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Loss_01,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Victory_01,
      (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Victory_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Anduin_04 boHAnduin04 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHAnduin04.MissionPlayVO(actor, (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Intro_01);
    yield return (object) boHAnduin04.MissionPlayVO(friendlyActor, (string) BoH_Anduin_04.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMulliganMusicTrack = MusicPlaylistType.InGame_BT;
    this.m_standardEmoteResponseLine = (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4EmoteResponse_01;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Anduin_04 boHAnduin04 = this;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHAnduin04.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      while (boHAnduin04.m_enemySpeaking)
        yield return (object) null;
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      switch (missionEvent)
      {
        case 504:
          GameState.Get().SetBusy(true);
          yield return (object) boHAnduin04.MissionPlayVO(enemyActor, (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Victory_01);
          yield return (object) boHAnduin04.MissionPlayVO(enemyActor, (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Victory_02);
          yield return (object) boHAnduin04.MissionPlayVO(friendlyActor, (string) BoH_Anduin_04.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4Victory_02);
          GameState.Get().SetBusy(false);
          break;
        case 507:
          GameState.Get().SetBusy(true);
          yield return (object) boHAnduin04.MissionPlayVO(enemyActor, (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4Loss_01);
          GameState.Get().SetBusy(false);
          break;
        case 515:
          yield return (object) boHAnduin04.MissionPlayVO(enemyActor, boHAnduin04.m_standardEmoteResponseLine);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHAnduin04.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Anduin_04 boHAnduin04 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHAnduin04.\u003C\u003En__1(entity);
    while (boHAnduin04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHAnduin04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHAnduin04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHAnduin04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Anduin_04 boHAnduin04 = this;
    while (boHAnduin04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHAnduin04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHAnduin04.\u003C\u003En__2(entity);
      yield return (object) boHAnduin04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHAnduin04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Anduin_04 boHAnduin04 = this;
    while (boHAnduin04.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHAnduin04.MissionPlayVO(enemyActor, (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeA_01);
        yield return (object) boHAnduin04.MissionPlayVO(friendlyActor, (string) BoH_Anduin_04.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeA_02);
        break;
      case 3:
        yield return (object) boHAnduin04.MissionPlayVO(enemyActor, (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeB_01);
        yield return (object) boHAnduin04.MissionPlayVO(friendlyActor, (string) BoH_Anduin_04.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeB_02);
        yield return (object) boHAnduin04.MissionPlayVO(enemyActor, (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeB_03);
        break;
      case 7:
        yield return (object) boHAnduin04.MissionPlayVO(enemyActor, (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeC_01);
        yield return (object) boHAnduin04.MissionPlayVO(friendlyActor, (string) BoH_Anduin_04.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission4ExchangeC_02);
        yield return (object) boHAnduin04.MissionPlayVO(enemyActor, (string) BoH_Anduin_04.VO_Story_Hero_Velen_Male_Draenei_Story_Anduin_Mission4ExchangeC_03);
        break;
    }
  }
}
