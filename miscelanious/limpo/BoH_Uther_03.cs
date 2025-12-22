using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Uther_03 : BoH_Uther_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Uther_03.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeA_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeA_01.prefab:9a4fae4ac32fb0b46b16eba1b2ebdc86");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeB_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeB_01.prefab:78c46583c71beb746b1e8e104c43f9eb");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeC_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeC_01.prefab:e01cb8c967c195548b8cf5e7bde23e5b");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3Victory_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3Victory_01.prefab:1556c58b0e16e3a449c092853a1dddb6");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3EmoteResponse_01 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3EmoteResponse_01.prefab:04487e78b0fbcbd48a6f47f4b10b2659");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeA_01 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeA_01.prefab:93d30bf1ea7f890439e39667336e069a");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeB_01 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeB_01.prefab:8e5a52dcbadfe334cb54ab8df1a4c314");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeC_01 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeC_01.prefab:d403b61cdae436340b0f899996472321");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_01 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_01.prefab:c3624cd3f84813d40a351159ae5afa67");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_02 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_02.prefab:36af4d8ed0b262f48a43e64f3adbe5ab");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_03 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_03.prefab:f29115de713965447b4e0faa02bd07dc");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_01 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_01.prefab:b347788f2de75e1419ef935427fba50f");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_02 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_02.prefab:eee9893ee5ed9e04c906fd42bc42e5fb");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_03 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_03.prefab:3004a634f1bf79045a32ec76ab637f7a");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Loss_01 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Loss_01.prefab:5f6dde007919868458e07991e8515ad1");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Victory_01 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Victory_01.prefab:bb8152842925e054bb6bb24f897595bf");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3Intro_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3Intro_01.prefab:e0cf704594324854f88cbc1707377ee5");
  private static readonly AssetReference VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Intro_01 = new AssetReference("VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Intro_01.prefab:d690374e721eff644acfc5b0d3b2360d");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_01,
    (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_02,
    (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_03
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_01,
    (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_02,
    (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Uther_03.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeA_01,
      (string) BoH_Uther_03.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeB_01,
      (string) BoH_Uther_03.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeC_01,
      (string) BoH_Uther_03.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3Intro_01,
      (string) BoH_Uther_03.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3Victory_01,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3EmoteResponse_01,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeA_01,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeB_01,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeC_01,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_01,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_02,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3HeroPower_03,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_01,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_02,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Idle_03,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Loss_01,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Victory_01,
      (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Uther_03 boHUther03 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHUther03.PlayLineAlways(actor, (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Intro_01);
    yield return (object) boHUther03.PlayLineAlways(friendlyActor, (string) BoH_Uther_03.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3EmoteResponse_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START || !MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Uther_03 boHUther03 = this;
    while (boHUther03.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 501:
        yield return (object) boHUther03.PlayLineAlways(actor, (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Victory_01);
        yield return (object) boHUther03.PlayLineAlways(friendlyActor, (string) BoH_Uther_03.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3Victory_01);
        break;
      case 504:
        yield return (object) boHUther03.PlayLineAlways(actor, (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3Loss_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHUther03.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Uther_03 boHUther03 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHUther03.\u003C\u003En__1(entity);
    while (boHUther03.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHUther03.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHUther03.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHUther03.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Uther_03 boHUther03 = this;
    while (boHUther03.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHUther03.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHUther03.\u003C\u003En__2(entity);
      yield return (object) boHUther03.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHUther03.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Uther_03 boHUther03 = this;
    while (boHUther03.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 5:
        yield return (object) boHUther03.PlayLineAlways(friendlyActor, (string) BoH_Uther_03.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeA_01);
        yield return (object) boHUther03.PlayLineAlways(enemyActor, (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeA_01);
        break;
      case 7:
        yield return (object) boHUther03.PlayLineAlways(enemyActor, (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeB_01);
        yield return (object) boHUther03.PlayLineAlways(friendlyActor, (string) BoH_Uther_03.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeB_01);
        break;
      case 11:
        yield return (object) boHUther03.PlayLineAlways(enemyActor, (string) BoH_Uther_03.VO_Story_Minion_Varok_Male_Orc_Story_Uther_Mission3ExchangeC_01);
        yield return (object) boHUther03.PlayLineAlways(friendlyActor, (string) BoH_Uther_03.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission3ExchangeC_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BRMAdventure);
}
