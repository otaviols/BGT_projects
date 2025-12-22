using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Uther_02 : BoH_Uther_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Uther_02.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2EmoteResponse_01 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2EmoteResponse_01.prefab:e99fa9cff27558d47bae5396f280ec76");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeC_01 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeC_01.prefab:5f009be2a90c50246bf32055d555aad1");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeF_01 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeF_01.prefab:6e75ca36e0f86bd438b50d62de39d241");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeF_02 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeF_02.prefab:cdf47d0e8f620ec4ca7d6d861e900754");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_01 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_01.prefab:a5ceb6e78bf885c4ab90a0f15c672e6c");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_02.prefab:198f953aeaaf2fd448958156bd462ebd");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_03 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_03.prefab:bcaf97cd0b101b84792ffa928b163ca5");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_01 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_01.prefab:29455d5e67785ef46962aa3e9ad5e20a");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_02 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_02.prefab:b813617484c062d428e14297c48a0be4");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_03 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_03.prefab:23608b80c9efffb4698596d449549941");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Loss_01 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Loss_01.prefab:77305932981d4884cbd2c916322470ef");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeA_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeA_01.prefab:705945e31c5b2534cb667afe73bb96b7");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeB_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeB_01.prefab:206092eb6790cb5408b9c694176c2ed6");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeC_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeC_01.prefab:1c27797e5d3f0e7478b6dcb140f4c84c");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeD_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeD_01.prefab:129cfddffac2b0b4cad6156153712af6");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeE_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeE_01.prefab:7eadab07c7c728a45a1a538d0236cc78");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeF_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeF_01.prefab:f4e3ef05bdce73c438ec4a22465f7180");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2Victory_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2Victory_01.prefab:f43a6f83dc1bdfd4e914b19b1f668d00");
  private static readonly AssetReference VO_Story_Minion_Terenas_Male_Human_Story_Uther_Mission2Victory_01 = new AssetReference("VO_Story_Minion_Terenas_Male_Human_Story_Uther_Mission2Victory_01.prefab:d8558cd406d23df4c9143c3b06ee5354");
  private static readonly AssetReference VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2ExchangeA_01 = new AssetReference("VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2ExchangeA_01.prefab:2e1c8b7265a9db244b0a71cefc95b28d");
  private static readonly AssetReference VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2ExchangeB_01 = new AssetReference("VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2ExchangeB_01.prefab:b623dc89eeff8ac478c6f36df301ef24");
  private static readonly AssetReference VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2Victory_01 = new AssetReference("VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2Victory_01.prefab:dce4d42c4936af84a9047c21e348e2ca");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2Intro_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2Intro_01.prefab:a027b0c170c35d9499f42984c238a427");
  private static readonly AssetReference VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Intro_01 = new AssetReference("VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Intro_01.prefab:0461a0b66c59be0458fc5397fb94a7b4");
  public static readonly AssetReference TerenasBrassRing = new AssetReference("Terenas_BrassRing_Quote.prefab:b640b1fdb81ce4942979cf91f8255eb1");
  public static readonly AssetReference TuralyonBrassRing = new AssetReference("Turalyon_BrassRing_Quote.prefab:40afbe0d5b4da0643baf2ebf5756548d");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_01,
    (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_02,
    (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_03
  };
  private List<string> m_IdleLines = new List<string>()
  {
    (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_01,
    (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_02,
    (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_03
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
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2EmoteResponse_01,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeC_01,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeF_01,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeF_02,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_01,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_02,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2HeroPower_03,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_01,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_02,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Idle_03,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Intro_01,
      (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Loss_01,
      (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeA_01,
      (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeB_01,
      (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeC_01,
      (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeD_01,
      (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeE_01,
      (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeF_01,
      (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2Victory_01,
      (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2Intro_01,
      (string) BoH_Uther_02.VO_Story_Minion_Terenas_Male_Human_Story_Uther_Mission2Victory_01,
      (string) BoH_Uther_02.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2ExchangeA_01,
      (string) BoH_Uther_02.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2ExchangeB_01,
      (string) BoH_Uther_02.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Uther_02 boHUther02 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHUther02.PlayLineAlways(actor, (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2Intro_01);
    yield return (object) boHUther02.PlayLineAlways(enemyActor, (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2EmoteResponse_01;
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
    BoH_Uther_02 boHUther02 = this;
    while (boHUther02.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 105:
        yield return (object) boHUther02.PlayLineAlways((string) BoH_Uther_02.TuralyonBrassRing, (string) BoH_Uther_02.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2ExchangeB_01);
        yield return (object) boHUther02.PlayLineAlways(friendlyActor, (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeB_01);
        break;
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) boHUther02.PlayLineAlways((string) BoH_Uther_02.TuralyonBrassRing, (string) BoH_Uther_02.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2Victory_01);
        yield return (object) boHUther02.PlayLineAlways((string) BoH_Uther_02.TerenasBrassRing, (string) BoH_Uther_02.VO_Story_Minion_Terenas_Male_Human_Story_Uther_Mission2Victory_01);
        yield return (object) boHUther02.PlayLineAlways(friendlyActor, (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 502:
        yield return (object) boHUther02.PlayLineOnlyOnce(friendlyActor, (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeE_01);
        break;
      case 503:
        GameState.Get().SetBusy(true);
        yield return (object) boHUther02.PlayLineAlways(enemyActor, (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeF_01);
        yield return (object) boHUther02.PlayLineAlways(enemyActor, (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeF_02);
        GameState.Get().SetBusy(false);
        break;
      case 504:
        yield return (object) boHUther02.PlayLineAlways(enemyActor, (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2Loss_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHUther02.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Uther_02 boHUther02 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHUther02.\u003C\u003En__1(entity);
    while (boHUther02.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHUther02.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHUther02.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHUther02.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Uther_02 boHUther02 = this;
    while (boHUther02.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHUther02.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHUther02.\u003C\u003En__2(entity);
      yield return (object) boHUther02.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHUther02.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Uther_02 boHUther02 = this;
    while (boHUther02.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHUther02.PlayLineAlways(friendlyActor, (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeD_01);
        break;
      case 3:
        yield return (object) boHUther02.PlayLineAlways((string) BoH_Uther_02.TuralyonBrassRing, (string) BoH_Uther_02.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission2ExchangeA_01);
        yield return (object) boHUther02.PlayLineAlways(friendlyActor, (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeA_01);
        break;
      case 9:
        yield return (object) boHUther02.PlayLineAlways(actor, (string) BoH_Uther_02.VO_Story_Hero_Orgrim_Male_Orc_Story_Uther_Mission2ExchangeC_01);
        yield return (object) boHUther02.PlayLineAlways(friendlyActor, (string) BoH_Uther_02.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission2ExchangeC_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BRMAdventure);
}
