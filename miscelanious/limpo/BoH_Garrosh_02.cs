using System.Collections;
using System.Collections.Generic;

public class BoH_Garrosh_02 : BoH_Garrosh_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeC_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeC_02.prefab:869bea22224da1445996eb61993a3782");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeA_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeA_01.prefab:687968921db95754aac0242e8eb37022");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeB_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeB_01.prefab:ed49af2df2aa3704c8259e07ab595677");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2Intro_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2Intro_01.prefab:227299eca6d30cd47b58ce1c74430997");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2Victory_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2Victory_01.prefab:21a1375eaa3fd474daa0460e492293a7");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2EmoteResponse_01 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2EmoteResponse_01.prefab:3f54be3babd4c774a87e34a25b465011");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeA_01 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeA_01.prefab:4bea4a44553a3e9489f96650a41823ee");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeB_01 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeB_01.prefab:93de4687ad108e04b8580d160fa7ec93");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeC_01 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeC_01.prefab:7c274a42098bdaa46acff149e04d5de9");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_01 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_01.prefab:62c830ac8ef650f4eb45441f5de6b85a");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_02.prefab:b8ab4b1acd654404e88434cd8390222d");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_03 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_03.prefab:c2c4765be650fce4eac968d182ff3875");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_01 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_01.prefab:e45b053d508b2b04fbb6e6dae14c4571");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_02 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_02.prefab:7de72d720b95ec14da04f4e5c16b6d29");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_03 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_03.prefab:2a833e67e96b16440af9d7f78f8f11c3");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Intro_01 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Intro_01.prefab:848bac39ba97f25428b7de15ecb5d7fd");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Intro_02 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Intro_02.prefab:ad44613f5980f6f43bacc46f162ee276");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Loss_01 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Loss_01.prefab:5edb3d3489cd7af40ba0bfc9458fa0e4");
  private static readonly AssetReference VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Victory_01 = new AssetReference("VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Victory_01.prefab:50902668a081be54abde248907028f85");
  private List<string> m_VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPowerLines = new List<string>()
  {
    (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_01,
    (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_02,
    (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_03
  };
  private List<string> m_VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2IdleLines = new List<string>()
  {
    (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_01,
    (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_02,
    (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Garrosh_02.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeC_02,
      (string) BoH_Garrosh_02.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeA_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeB_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2Intro_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2Victory_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2EmoteResponse_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeA_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeB_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeC_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_02,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPower_03,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_02,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Idle_03,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Intro_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Intro_02,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Loss_01,
      (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Garrosh_02 boHGarrosh02 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHGarrosh02.PlayLineAlways(enemyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Intro_01);
    yield return (object) boHGarrosh02.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2Intro_01);
    yield return (object) boHGarrosh02.PlayLineAlways(enemyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Garrosh_02 boHGarrosh02 = this;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHGarrosh02.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      while (boHGarrosh02.m_enemySpeaking)
        yield return (object) null;
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      switch (missionEvent)
      {
        case 501:
          GameState.Get().SetBusy(true);
          yield return (object) boHGarrosh02.PlayLineAlways(actor, (string) BoH_Garrosh_02.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2Victory_01);
          yield return (object) boHGarrosh02.PlayLineAlways(enemyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Victory_01);
          GameState.Get().SetBusy(false);
          break;
        case 504:
          GameState.Get().SetBusy(true);
          yield return (object) boHGarrosh02.PlayLineAlways(enemyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2Loss_01);
          GameState.Get().SetBusy(false);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHGarrosh02.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_02 boHGarrosh02 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHGarrosh02.\u003C\u003En__1(entity);
    while (boHGarrosh02.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGarrosh02.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHGarrosh02.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGarrosh02.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_02 boHGarrosh02 = this;
    while (boHGarrosh02.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGarrosh02.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHGarrosh02.\u003C\u003En__2(entity);
      yield return (object) boHGarrosh02.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGarrosh02.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Garrosh_02 boHGarrosh02 = this;
    while (boHGarrosh02.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 5:
        yield return (object) boHGarrosh02.PlayLineAlways(enemyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeA_01);
        yield return (object) boHGarrosh02.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeA_01);
        break;
      case 7:
        yield return (object) boHGarrosh02.PlayLineAlways(enemyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeB_01);
        yield return (object) boHGarrosh02.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeB_01);
        break;
      case 9:
        yield return (object) boHGarrosh02.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission2ExchangeC_02);
        yield return (object) boHGarrosh02.PlayLineAlways(enemyActor, (string) BoH_Garrosh_02.VO_Story_Hero_Rehgar_Male_Orc_Story_Garrosh_Mission2ExchangeC_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BRMAdventure);
}
