using System.Collections;
using System.Collections.Generic;

public class BoH_Valeera_02 : BoH_Valeera_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2EmoteResponse_01 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2EmoteResponse_01.prefab:edcc9389fc36fc04795482d021a4db69");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeA_02 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeA_02.prefab:a18886530f128194a8e6c9cd551e15b2");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeB_02 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeB_02.prefab:b60465c7d85c5d54db8deaaa6499dba8");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeC_01 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeC_01.prefab:726e0df8f25c2db4d91a5b48fc425f31");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_01 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_01.prefab:88561e5cb1c2f334ba140bc4b096dd8a");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_02 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_02.prefab:a11ad0c7dadfaa04095e2c7ca2f6324c");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_03 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_03.prefab:1d39789c1bed6484d9af99d353465be8");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_01 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_01.prefab:9ab259f6312db614e90996f5f3847e48");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_02 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_02.prefab:ce15192b7c1cbe54aa0c3fe008985fd2");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_03 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_03.prefab:4431015f99b46d54ca86cd4e968b3343");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Intro_01 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Intro_01.prefab:792271b57c013194d929b312ddee74db");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Loss_01 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Loss_01.prefab:e80f100bf7ee29046ad65d3a85244618");
  private static readonly AssetReference VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Victory_02 = new AssetReference("VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Victory_02.prefab:1e95c910ff896224fb1880208d36798e");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeA_01 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeA_01.prefab:e94ad0fd247aabe46abd9f9e93eb007a");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeB_01 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeB_01.prefab:3988a17a277599f41a6fd18722430454");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeC_02 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeC_02.prefab:ecc7c3cd7f6b36e43927ef2fb4452f0a");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Intro_01 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Intro_01.prefab:fad09fc626a9b514ba7f4ae60911f519");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Victory_01 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Victory_01.prefab:4991e283ef256ca44b0ac1e45bce9ebe");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Victory_03 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Victory_03.prefab:d4ae3a322b1b85346808a2e8c311c449");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_01,
    (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_02,
    (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_01,
    (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_02,
    (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2EmoteResponse_01,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeA_02,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeB_02,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeC_01,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_01,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_02,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2HeroPower_03,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_01,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_02,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Idle_03,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Intro_01,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Loss_01,
      (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Victory_02,
      (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeA_01,
      (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeB_01,
      (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeC_02,
      (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Intro_01,
      (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Victory_01,
      (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Victory_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Valeera_02 boHValeera02 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHValeera02.MissionPlayVO(actor, (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Intro_01);
    yield return (object) boHValeera02.MissionPlayVO(friendlyActor, (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_TRL;
    this.m_standardEmoteResponseLine = (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2EmoteResponse_01;
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
    BoH_Valeera_02 boHValeera02 = this;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHValeera02.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      while (boHValeera02.m_enemySpeaking)
        yield return (object) null;
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      switch (missionEvent)
      {
        case 102:
          yield return (object) boHValeera02.PlayLineAlways(friendlyActor, (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeA_01);
          yield return (object) boHValeera02.PlayLineAlways(enemyActor, (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeA_02);
          break;
        case 504:
          GameState.Get().SetBusy(true);
          yield return (object) boHValeera02.PlayLineAlways(friendlyActor, (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Victory_01);
          yield return (object) boHValeera02.PlayLineAlways(enemyActor, (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Victory_02);
          yield return (object) boHValeera02.PlayLineAlways(friendlyActor, (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2Victory_03);
          GameState.Get().SetBusy(false);
          break;
        case 507:
          yield return (object) boHValeera02.PlayLineAlways(enemyActor, (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2Loss_01);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHValeera02.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Valeera_02 boHValeera02 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHValeera02.\u003C\u003En__1(entity);
    while (boHValeera02.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHValeera02.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHValeera02.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHValeera02.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Valeera_02 boHValeera02 = this;
    while (boHValeera02.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHValeera02.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHValeera02.\u003C\u003En__2(entity);
      yield return (object) boHValeera02.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHValeera02.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Valeera_02 boHValeera02 = this;
    while (boHValeera02.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 9:
        yield return (object) boHValeera02.PlayLineAlways(friendlyActor, (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeB_01);
        yield return (object) boHValeera02.PlayLineAlways(enemyActor, (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeB_02);
        break;
      case 13:
        yield return (object) boHValeera02.PlayLineAlways(enemyActor, (string) BoH_Valeera_02.VO_Story_Hero_Helka_Female_Tauren_Story_Valeera_Mission2ExchangeC_01);
        yield return (object) boHValeera02.PlayLineAlways(friendlyActor, (string) BoH_Valeera_02.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission2ExchangeC_02);
        break;
    }
  }
}
