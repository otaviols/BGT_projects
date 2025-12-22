using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Guldan_05 : BoH_Guldan_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Guldan_05.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeA_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeA_02.prefab:dd2e00d0521b280468331c77d2981425");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeB_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeB_02.prefab:7d9185b945e46d841827a4769eb0864d");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeC_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeC_02.prefab:84abf07fcbbd7d64d8ed34ec1a48ff64");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeD_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeD_02.prefab:94810835864559240bc1cc590df57fe0");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5Intro_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5Intro_02.prefab:2c9b7b92df7b0894b9e2637a7d6e0b16");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5Victory_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5Victory_02.prefab:d0ebd4a38a10b5b4ab18a9c298476e56");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5EmoteResponse_01 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5EmoteResponse_01.prefab:1c8c46966b42fad45a606eae41e9f80c");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeA_01 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeA_01.prefab:e752331cc91fc654bab5439dc5844e1b");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeA_03 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeA_03.prefab:5cc229216b433de4b943e3b217d063a8");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeB_01 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeB_01.prefab:3513be1e095067645960fd2308b9a3c6");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeC_01 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeC_01.prefab:3f0adb0ce9653c14fa2ba216c02f8452");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeD_01 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeD_01.prefab:25b6dd2926f017c4591511a24b6b4125");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_01 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_01.prefab:14deae51b27ec684393dec271b2ba7e9");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_02 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_02.prefab:da12e237ae31abe41b22d7cf6cbe093d");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_03 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_03.prefab:1027f5d65c4bb6a458d57319251affd7");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_01 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_01.prefab:00a4edce57051204caede1c13363135d");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_02 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_02.prefab:66ee126138752be43a1ea56e296ebbe0");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_03 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_03.prefab:b6e85386950dcc6419d5782cbaf58d85");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Intro_01 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Intro_01.prefab:81cf08a836b7ada4ba138339f81f9f85");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Loss_01 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Loss_01.prefab:30fa8b9688aee69488c35bbef4dd27e4");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Victory_01 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Victory_01.prefab:5db5ff7b08042cb479aad2c07b189038");
  private static readonly AssetReference VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Victory_03 = new AssetReference("VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Victory_03.prefab:95e61a8f9fd405744ae05e89a23ae24a");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_01,
    (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_02,
    (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_01,
    (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_02,
    (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_03
  };
  private List<string> m_EmoteResponseLines = new List<string>()
  {
    (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5EmoteResponse_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Guldan_05() => this.m_gameOptions.AddBooleanOptions(BoH_Guldan_05.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeA_02,
      (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeB_02,
      (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeC_02,
      (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeD_02,
      (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5Intro_02,
      (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5Victory_02,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5EmoteResponse_01,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeA_01,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeA_03,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeB_01,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeC_01,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeD_01,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_01,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_02,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5HeroPower_03,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_01,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_02,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Idle_03,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Intro_01,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Loss_01,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Victory_01,
      (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Victory_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Guldan_05 boHGuldan05 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHGuldan05.MissionPlayVO(actor, (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Intro_01);
    yield return (object) boHGuldan05.MissionPlayVO(friendlyActor, (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  public override void OnCreateGame()
  {
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_SCH_FinalLevels;
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5EmoteResponse_01;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Guldan_05 boHGuldan05 = this;
    while (boHGuldan05.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan05.MissionPlayVO(enemyActor, (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Victory_01);
        yield return (object) boHGuldan05.MissionPlayVO(friendlyActor, (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5Victory_02);
        yield return (object) boHGuldan05.MissionPlayVO(enemyActor, (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Victory_03);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan05.MissionPlayVO(enemyActor, (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHGuldan05.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Guldan_05 boHGuldan05 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHGuldan05.\u003C\u003En__1(entity);
    while (boHGuldan05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGuldan05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHGuldan05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGuldan05.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Guldan_05 boHGuldan05 = this;
    while (boHGuldan05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGuldan05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHGuldan05.\u003C\u003En__2(entity);
      yield return (object) boHGuldan05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGuldan05.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Guldan_05 boHGuldan05 = this;
    while (boHGuldan05.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHGuldan05.MissionPlayVO(enemyActor, (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeA_01);
        yield return (object) boHGuldan05.MissionPlayVO(friendlyActor, (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeA_02);
        yield return (object) boHGuldan05.MissionPlayVO(enemyActor, (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeA_03);
        break;
      case 3:
        yield return (object) boHGuldan05.MissionPlayVO(enemyActor, (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeB_01);
        yield return (object) boHGuldan05.MissionPlayVO(friendlyActor, (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeB_02);
        break;
      case 5:
        yield return (object) boHGuldan05.MissionPlayVO(enemyActor, (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeC_01);
        yield return (object) boHGuldan05.MissionPlayVO(friendlyActor, (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeC_02);
        break;
      case 11:
        yield return (object) boHGuldan05.MissionPlayVO(enemyActor, (string) BoH_Guldan_05.VO_Story_Hero_Medivh_Male_Human_Story_Guldan_Mission5ExchangeD_01);
        yield return (object) boHGuldan05.MissionPlayVO(friendlyActor, (string) BoH_Guldan_05.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission5ExchangeD_02);
        break;
    }
  }
}
