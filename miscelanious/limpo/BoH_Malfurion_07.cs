using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Malfurion_07 : BoH_Malfurion_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Malfurion_07.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7ExchangeA_01 = new AssetReference("VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7ExchangeA_01.prefab:90d506f6e68ede3488176813d4550746");
  private static readonly AssetReference VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7ExchangeD_01 = new AssetReference("VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7ExchangeD_01.prefab:a295e1a4542d1eb469e341690f98346a");
  private static readonly AssetReference VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7Intro_01 = new AssetReference("VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7Intro_01.prefab:db6210202fa3cb7499242f5041938586");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Malfurion_Mission7ExchangeB_01 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Malfurion_Mission7ExchangeB_01.prefab:9df52d023443eaf4f837e011f96fa3e3");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Malfurion_Mission7Intro_02 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Malfurion_Mission7Intro_02.prefab:7e1b0afab07582c46ae8457112ab8347");
  private static readonly AssetReference VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7_Victory_01 = new AssetReference("VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7_Victory_01.prefab:8f66f1279f881604f92002ce0223cbff");
  private static readonly AssetReference VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7EmoteResponse_01 = new AssetReference("VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7EmoteResponse_01.prefab:e1b2f8901e118df4fa80583229331b18");
  private static readonly AssetReference VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_01 = new AssetReference("VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_01.prefab:e0bbad6ff4ddc914ba3e2b5db44a6742");
  private static readonly AssetReference VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_02 = new AssetReference("VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_02.prefab:5d0ca78992ff24e4fb0c36c5885d49ef");
  private static readonly AssetReference VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_03 = new AssetReference("VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_03.prefab:2627eb43a4e966d4896ed8924a72eca0");
  private static readonly AssetReference VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_01 = new AssetReference("VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_01.prefab:e3d10881bfe5c5c4982fcab1fc36165d");
  private static readonly AssetReference VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_02 = new AssetReference("VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_02.prefab:0cb81c58c81c0e54ca19b3f966425f29");
  private static readonly AssetReference VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_03 = new AssetReference("VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_03.prefab:0f8f072f141707344a10ebd0a9d8cdcc");
  private static readonly AssetReference VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Intro_04 = new AssetReference("VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Intro_04.prefab:eb94840e17b872b4ab6d7356ab162368");
  private static readonly AssetReference VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Loss_01 = new AssetReference("VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Loss_01.prefab:886b18ed9f44ef94e9f34e5d1962892b");
  private static readonly AssetReference VO_Story_Minion_Hamuul_Male_Tauren_Story_Malfurion_Mission7ExchangeC_01 = new AssetReference("VO_Story_Minion_Hamuul_Male_Tauren_Story_Malfurion_Mission7ExchangeC_01.prefab:07545a5fea43da744b2d570ebe9915c2");
  private static readonly AssetReference VO_Story_Minion_Hamuul_Male_Tauren_Story_Malfurion_Mission7Intro_03 = new AssetReference("VO_Story_Minion_Hamuul_Male_Tauren_Story_Malfurion_Mission7Intro_03.prefab:d48df2c560d6cb54288c15481e206f93");
  public static readonly AssetReference CenariusBrassRing = new AssetReference("Cenarius_BrassRing_Quote.prefab:9157110d07b5b004fa0c0f651c71ef81");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_01,
    (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_02,
    (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_01,
    (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_02,
    (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Malfurion_07() => this.m_gameOptions.AddBooleanOptions(BoH_Malfurion_07.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Malfurion_07.VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7ExchangeA_01,
      (string) BoH_Malfurion_07.VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7ExchangeD_01,
      (string) BoH_Malfurion_07.VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7Intro_01,
      (string) BoH_Malfurion_07.VO_Story_Hero_Malfurion_Male_NightElf_Story_Malfurion_Mission7ExchangeB_01,
      (string) BoH_Malfurion_07.VO_Story_Hero_Malfurion_Male_NightElf_Story_Malfurion_Mission7Intro_02,
      (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7_Victory_01,
      (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7EmoteResponse_01,
      (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_01,
      (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_02,
      (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7HeroPower_03,
      (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_01,
      (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_02,
      (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Idle_03,
      (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Intro_04,
      (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Loss_01,
      (string) BoH_Malfurion_07.VO_Story_Minion_Hamuul_Male_Tauren_Story_Malfurion_Mission7ExchangeC_01,
      (string) BoH_Malfurion_07.VO_Story_Minion_Hamuul_Male_Tauren_Story_Malfurion_Mission7Intro_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Malfurion_07 boHMalfurion07 = this;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHMalfurion07.MissionPlayVO(BoH_Malfurion_07.CenariusBrassRing, (string) BoH_Malfurion_07.VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7Intro_01);
    yield return (object) boHMalfurion07.MissionPlayVO(friendlyActor, (string) BoH_Malfurion_07.VO_Story_Hero_Malfurion_Male_NightElf_Story_Malfurion_Mission7Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_BRMAdventure;
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7EmoteResponse_01;
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
    BoH_Malfurion_07 boHMalfurion07 = this;
    while (boHMalfurion07.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) boHMalfurion07.PlayLineAlways(boHMalfurion07.GetFriendlyActorByCardId("Story_08_Hamuul"), (string) BoH_Malfurion_07.VO_Story_Minion_Hamuul_Male_Tauren_Story_Malfurion_Mission7ExchangeC_01);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHMalfurion07.PlayLineAlways(actor, (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7_Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHMalfurion07.PlayLineAlways(actor, (string) BoH_Malfurion_07.VO_Story_Hero_Ragnaros_Male_Elemental_Story_Malfurion_Mission7Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHMalfurion07.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Malfurion_07 boHMalfurion07 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHMalfurion07.\u003C\u003En__1(entity);
    while (boHMalfurion07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHMalfurion07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHMalfurion07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHMalfurion07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Malfurion_07 boHMalfurion07 = this;
    while (boHMalfurion07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHMalfurion07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHMalfurion07.\u003C\u003En__2(entity);
      yield return (object) boHMalfurion07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHMalfurion07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Malfurion_07 boHMalfurion07 = this;
    while (boHMalfurion07.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHMalfurion07.MissionPlayVO(boHMalfurion07.GetFriendlyActorByCardId("Story_08_Hamuul"), (string) BoH_Malfurion_07.VO_Story_Minion_Hamuul_Male_Tauren_Story_Malfurion_Mission7Intro_03);
        break;
      case 3:
        yield return (object) boHMalfurion07.PlayLineAlways((string) BoH_Malfurion_07.CenariusBrassRing, (string) BoH_Malfurion_07.VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7ExchangeA_01);
        break;
      case 7:
        yield return (object) boHMalfurion07.PlayLineAlways(actor, (string) BoH_Malfurion_07.VO_Story_Hero_Malfurion_Male_NightElf_Story_Malfurion_Mission7ExchangeB_01);
        break;
      case 11:
        yield return (object) boHMalfurion07.PlayLineAlways((string) BoH_Malfurion_07.CenariusBrassRing, (string) BoH_Malfurion_07.VO_Story_Hero_Cenarius_Male_Demigod_Story_Malfurion_Mission7ExchangeD_01);
        break;
    }
  }
}
