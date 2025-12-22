using System.Collections;
using System.Collections.Generic;

public class BoH_Garrosh_07 : BoH_Garrosh_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7EmoteResponse_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7EmoteResponse_01.prefab:a932c77d70720da4589486633ed1e7e9");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeB_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeB_01.prefab:9571179a848137b43a0458654e27365d");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_01.prefab:e043aa0297f133e4289883dcdd532ea8");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_02.prefab:b1d5670fbbb1e4649bb92078e71120c6");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_01.prefab:27e1ba1890eb9bb4fb374a12c35dd7a5");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_02.prefab:b5cf47dc7e289014988bee1b5eb5dd83");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_03 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_03.prefab:f5ff18556f3d32942820785cf22ea405");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_01.prefab:9a19642236c02fe418745e7e11602195");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_02.prefab:0dc91717346316847b1888503adf81ad");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_03 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_03.prefab:5c721ed34bf80b94ca22d377a903cfe8");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Intro_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Intro_01.prefab:7d5847a2087e80941beddff135e0ce41");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Loss_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Loss_01.prefab:66f6e13da484def49ae357f5bddddf37");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_01.prefab:30f58743aba7f9e4e9c2c6c77ab5034d");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_02.prefab:e427713bab99d9e47ab6a960bb182063");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeB_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeB_01.prefab:6c5ffb3161419ae4490dbe6cfd1a15aa");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Intro_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Intro_01.prefab:13f81fc1c76cba04e8b15ad99a81d423");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_01.prefab:5b39f80bab51d9c4b80eb758a6f72183");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_02.prefab:64e3347a98a90114d8089589e8f51b30");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_03.prefab:b29bb9bb3386f1d42a4b4b7e54173c39");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Victory_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Victory_01.prefab:eebbe511e35670b48bc483037a87dc6d");
  private List<string> m_VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPowerLines = new List<string>()
  {
    (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_01,
    (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_02,
    (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_03
  };
  private List<string> m_VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7IdleLines = new List<string>()
  {
    (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_01,
    (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_02,
    (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_03
  };
  private List<string> m_missionEventTrigger502Lines = new List<string>()
  {
    (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_01,
    (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_02,
    (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7EmoteResponse_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeB_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_02,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_02,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPower_03,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_02,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Idle_03,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Intro_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Loss_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_02,
      (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeB_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Intro_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_01,
      (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_02,
      (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_03,
      (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Garrosh_07 boHGarrosh07 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHGarrosh07.PlayLineAlways(actor, (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Intro_01);
    yield return (object) boHGarrosh07.PlayLineAlways(enemyActor, (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7HeroPowerLines;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7EmoteResponse_01;
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
    BoH_Garrosh_07 boHGarrosh07 = this;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHGarrosh07.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      while (boHGarrosh07.m_enemySpeaking)
        yield return (object) null;
      Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      switch (missionEvent)
      {
        case 112:
          yield return (object) boHGarrosh07.PlayAndRemoveRandomLineOnlyOnce(actor2, boHGarrosh07.m_missionEventTrigger502Lines);
          break;
        case 501:
          GameState.Get().SetBusy(true);
          yield return (object) boHGarrosh07.PlayLineAlways(actor2, (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Victory_01);
          GameState.Get().SetBusy(false);
          break;
        case 504:
          GameState.Get().SetBusy(true);
          yield return (object) boHGarrosh07.PlayLineAlways(actor1, (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Loss_01);
          GameState.Get().SetBusy(false);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHGarrosh07.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_07 boHGarrosh07 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHGarrosh07.\u003C\u003En__1(entity);
    while (boHGarrosh07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGarrosh07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHGarrosh07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGarrosh07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_07 boHGarrosh07 = this;
    while (boHGarrosh07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGarrosh07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHGarrosh07.\u003C\u003En__2(entity);
      yield return (object) boHGarrosh07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGarrosh07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Garrosh_07 boHGarrosh07 = this;
    while (boHGarrosh07.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHGarrosh07.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_01);
        yield return (object) boHGarrosh07.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_02);
        break;
      case 5:
        yield return (object) boHGarrosh07.PlayLineAlways(enemyActor, (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeB_01);
        yield return (object) boHGarrosh07.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeB_01);
        break;
      case 9:
        yield return (object) boHGarrosh07.PlayLineAlways(enemyActor, (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_01);
        yield return (object) boHGarrosh07.PlayLineAlways(enemyActor, (string) BoH_Garrosh_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_02);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_DRG);
}
