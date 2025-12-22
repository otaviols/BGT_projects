using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Anduin_01 : BoH_Anduin_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Anduin_01.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeA_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeA_02.prefab:aa7cbf7409e919847b1ffae3db4df734");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeB_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeB_02.prefab:77fd621472b276747b8debf68792404f");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeC_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeC_01.prefab:24e9a1cd458be0540816b38f6d117770");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeC_03 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeC_03.prefab:260037a21752be048b754d15a57615e8");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1Intro_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1Intro_02.prefab:46b083e10f1d6a842b06ab8bc1f2b7c3");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1EmoteResponse_01 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1EmoteResponse_01.prefab:f0c14cdcc45ccb842967914e2ecbe99e");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeA_01 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeA_01.prefab:5882b1257c7f6ff4a8d2de154c0e42b8");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeB_01 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeB_01.prefab:dcdfd8a448d20cc41aeecfbacfb0293d");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeC_02 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeC_02.prefab:dd88a9e2bcfd57a498ab78a7bac4cc6e");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_01 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_01.prefab:9652ba1809af8014286b16d7a4526217");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_02 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_02.prefab:ca4b63ff57c78f84d9c00a2153640e00");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_03 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_03.prefab:5afe7607307634d40ad7640f43bae35b");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_01 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_01.prefab:225f2d73c513c7543bae3aa0847e458a");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_02 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_02.prefab:91f154fec9df19e4c9f205435f347df6");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_03 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_03.prefab:8009fa680afd8d64bb62c017601aaa1e");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Intro_01 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Intro_01.prefab:134d77bcce6b763429f503ec7911c0c1");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Loss_01 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Loss_01.prefab:aeb35ee10bcafdf43a751cb0694f4a29");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Victory_01 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Victory_01.prefab:a3c75474456c2eb46a0d5af2ba7dde7f");
  private static readonly AssetReference VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Victory_02 = new AssetReference("VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Victory_02.prefab:55f37cb0cf500bd4f92a0d798373d5a4");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_01,
    (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_02,
    (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_01,
    (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_02,
    (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Anduin_01() => this.m_gameOptions.AddBooleanOptions(BoH_Anduin_01.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Anduin_01.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeA_02,
      (string) BoH_Anduin_01.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeB_02,
      (string) BoH_Anduin_01.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeC_01,
      (string) BoH_Anduin_01.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeC_03,
      (string) BoH_Anduin_01.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1Intro_02,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1EmoteResponse_01,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeA_01,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeB_01,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeC_02,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_01,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_02,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1HeroPower_03,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_01,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_02,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Idle_03,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Intro_01,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Loss_01,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Victory_01,
      (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Victory_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Anduin_01 boHAnduin01 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHAnduin01.MissionPlayVO(actor, (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Intro_01);
    yield return (object) boHAnduin01.MissionPlayVO(friendlyActor, (string) BoH_Anduin_01.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMulliganMusicTrack = MusicPlaylistType.InGame_Default;
    this.m_standardEmoteResponseLine = (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1EmoteResponse_01;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Anduin_01 boHAnduin01 = this;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHAnduin01.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      while (boHAnduin01.m_enemySpeaking)
        yield return (object) null;
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      switch (missionEvent)
      {
        case 504:
          GameState.Get().SetBusy(true);
          yield return (object) boHAnduin01.MissionPlayVO(enemyActor, (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Victory_01);
          yield return (object) boHAnduin01.MissionPlayVO(enemyActor, (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Victory_02);
          GameState.Get().SetBusy(false);
          break;
        case 507:
          GameState.Get().SetBusy(true);
          yield return (object) boHAnduin01.MissionPlayVO(enemyActor, (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1Loss_01);
          GameState.Get().SetBusy(false);
          break;
        case 515:
          yield return (object) boHAnduin01.MissionPlayVO(enemyActor, boHAnduin01.m_standardEmoteResponseLine);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHAnduin01.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Anduin_01 boHAnduin01 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHAnduin01.\u003C\u003En__1(entity);
    while (boHAnduin01.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHAnduin01.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHAnduin01.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHAnduin01.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Anduin_01 boHAnduin01 = this;
    while (boHAnduin01.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHAnduin01.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHAnduin01.\u003C\u003En__2(entity);
      yield return (object) boHAnduin01.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHAnduin01.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Anduin_01 boHAnduin01 = this;
    while (boHAnduin01.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHAnduin01.MissionPlayVO(enemyActor, (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeA_01);
        yield return (object) boHAnduin01.MissionPlayVO(friendlyActor, (string) BoH_Anduin_01.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeA_02);
        break;
      case 5:
        yield return (object) boHAnduin01.MissionPlayVO(enemyActor, (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeB_01);
        yield return (object) boHAnduin01.MissionPlayVO(friendlyActor, (string) BoH_Anduin_01.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeB_02);
        break;
      case 11:
        yield return (object) boHAnduin01.MissionPlayVO(friendlyActor, (string) BoH_Anduin_01.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeC_01);
        yield return (object) boHAnduin01.MissionPlayVO(enemyActor, (string) BoH_Anduin_01.VO_Story_Hero_Varian_Male_Human_Story_Anduin_Mission1ExchangeC_02);
        yield return (object) boHAnduin01.MissionPlayVO(friendlyActor, (string) BoH_Anduin_01.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission1ExchangeC_03);
        break;
    }
  }
}
