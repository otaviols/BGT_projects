using System.Collections;
using System.Collections.Generic;

public class BoH_Garrosh_08 : BoH_Garrosh_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeA_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeA_01.prefab:e5c5b670db416714c9b00b5135f3b3be");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_01.prefab:1168cbcb23827e44896f0cab1580d8b6");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_02.prefab:57b33b0e3629d4743860c5ecd806e43f");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeC_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeC_01.prefab:e53520db3376e6842b91969fcdb16fe7");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8Intro_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8Intro_01.prefab:a3a2d35f3727f144dac3df3258f1a938");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8EmoteResponse_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8EmoteResponse_01.prefab:74551dbaf11c0924b9571c8c77d93405");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeA_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeA_01.prefab:18971804117b5f346bf4901f7319f2c7");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeC_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeC_01.prefab:a490641c7d4016c418b5b005ec1e2427");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_01.prefab:dfad08cd8266489478a4186af6f330a0");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_02 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_02.prefab:9654500b7882a4241823e66435dee83f");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_03 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_03.prefab:b1f25de0583b4473acce86e0176f9dd6");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_01.prefab:2a5a7d9c5e03292439894ff0e6292629");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_02 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_02.prefab:ebad60e3d5c19d348a7ced60b9dfb99e");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_03 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_03.prefab:c2e60147c7a1369459491c54859e38be");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Intro_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Intro_01.prefab:829d2a0cfaa6caa4ab12c9d9de5b0ae2");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Loss_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Loss_01.prefab:72c8a96e92e979d4291d5765c7c672f4");
  private List<string> m_VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPowerLines = new List<string>()
  {
    (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_01,
    (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_02,
    (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_03
  };
  private List<string> m_VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8IdleLines = new List<string>()
  {
    (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_01,
    (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_02,
    (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Garrosh_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeA_01,
      (string) BoH_Garrosh_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_01,
      (string) BoH_Garrosh_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_02,
      (string) BoH_Garrosh_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeC_01,
      (string) BoH_Garrosh_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8Intro_01,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8EmoteResponse_01,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeA_01,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeC_01,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_01,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_02,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPower_03,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_01,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_02,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Idle_03,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Intro_01,
      (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Loss_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Garrosh_08 boHGarrosh08 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHGarrosh08.PlayLineAlways(actor, (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Intro_01);
    yield return (object) boHGarrosh08.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8EmoteResponse_01;
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
    BoH_Garrosh_08 boHGarrosh08 = this;
    while (boHGarrosh08.m_enemySpeaking)
      yield return (object) null;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHGarrosh08.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      switch (missionEvent)
      {
        case 502:
          GameState.Get().SetBusy(true);
          yield return (object) boHGarrosh08.PlayLineAlways(actor, (string) BoH_Garrosh_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeC_01);
          yield return (object) boHGarrosh08.PlayLineAlways(enemyActor, (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeC_01);
          GameState.Get().SetBusy(false);
          break;
        case 504:
          GameState.Get().SetBusy(true);
          yield return (object) boHGarrosh08.PlayLineAlways(enemyActor, (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Loss_01);
          GameState.Get().SetBusy(false);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHGarrosh08.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_08 boHGarrosh08 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHGarrosh08.\u003C\u003En__1(entity);
    while (boHGarrosh08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGarrosh08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHGarrosh08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGarrosh08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_08 boHGarrosh08 = this;
    while (boHGarrosh08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGarrosh08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHGarrosh08.\u003C\u003En__2(entity);
      yield return (object) boHGarrosh08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGarrosh08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Garrosh_08 boHGarrosh08 = this;
    while (boHGarrosh08.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) boHGarrosh08.PlayLineAlways(actor, (string) BoH_Garrosh_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeA_01);
        yield return (object) boHGarrosh08.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeA_01);
        break;
      case 9:
        yield return (object) boHGarrosh08.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_01);
        yield return (object) boHGarrosh08.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_02);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_TRLFinalBoss);
}
