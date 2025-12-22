using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Jaina_05 : BoH_Jaina_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Jaina_05.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Death_01 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Death_01.prefab:7a57a90fef864a748816495f7847a00f");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5EmoteResponse_01 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5EmoteResponse_01.prefab:e1cda8373702d1049bfd320bea8d1eca");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeA_01 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeA_01.prefab:220ba39a782dcac4a84ab439784f1be5");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeA_02 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeA_02.prefab:ec7fc41e6a9dbc24587a38a6cf439ddd");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeB_01 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeB_01.prefab:730aae8456054c8428eee28a3d1cab85");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_01 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_01.prefab:2c1dc3f5ecc665a498e2afaa600e73d7");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_02 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_02.prefab:14668126189787347852cc18bce4ddbf");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_03 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_03.prefab:c1ba5dfdd9ade824ab49ae8066d2b87b");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_01 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_01.prefab:ab6fed133f9cb6b44b208a698d744ae5");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_02 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_02.prefab:3e124d842b73a534e9302947b6be8e1e");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_03 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_03.prefab:8826376ee2ecd424abd82d2f513dbfb3");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Intro_01 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Intro_01.prefab:c54a545aaa9585545a7d1b1d6a2cd579");
  private static readonly AssetReference VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Loss_01 = new AssetReference("VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Loss_01.prefab:7e05b1b514dbb834bae98367dc8c4589");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5ExchangeA_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5ExchangeA_01.prefab:e044be60c9daf6b468d2eec0bc961587");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5ExchangeB_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5ExchangeB_01.prefab:4ec641caa9027ef478939dc16d7fda36");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5Intro_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5Intro_01.prefab:6b3d5621d1aadce49a35e96c2b63e831");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5Victory_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5Victory_01.prefab:fb05d18c25c21c94caa6ac2b1fbb2762");
  private static readonly AssetReference VO_Story_Hero_Malfurion_Male_NightElf_Story_Jaina_Mission5Malfurion_01 = new AssetReference("VO_Story_Hero_Malfurion_Male_NightElf_Story_Jaina_Mission5Malfurion_01.prefab:f3d7618d6d384f0f814fb40f04da4f6b");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission5Thrall_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission5Thrall_01.prefab:29ede513ae5b45ae8bd2cfa59f32937b");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission5Victory_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission5Victory_01.prefab:28dfa0839ba94acca781c893253d0510");
  private static readonly AssetReference VO_Story_Hero_Tyrande_Female_NightElf_Story_Jaina_Mission5Tyrande_01 = new AssetReference("VO_Story_Hero_Tyrande_Female_NightElf_Story_Jaina_Mission5Tyrande_01.prefab:21a832149b36441197d5d2a14101eea2");
  public static readonly AssetReference ThrallBrassRing = new AssetReference("Thrall_BrassRing_Quote.prefab:962e58c9390b0f842a8b64d0d44cf7b4");
  public static readonly AssetReference MalfurionBrassRing = new AssetReference("YoungMalfurion_Popup_BrassRing.prefab:5544ac85196277542a7fa0b1a9b578df");
  public static readonly AssetReference TyrandeBrassRing = new AssetReference("YoungTyrande_Popup_BrassRing.prefab:79f13833a3f5e97449ef744f460e9fbd");
  private List<string> m_VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPowerLines = new List<string>()
  {
    (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_01,
    (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_02,
    (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_03
  };
  private List<string> m_VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5IdleLines = new List<string>()
  {
    (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_01,
    (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_02,
    (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Jaina_05() => this.m_gameOptions.AddBooleanOptions(BoH_Jaina_05.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Death_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5EmoteResponse_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeA_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeA_02,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeB_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_02,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPower_03,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_02,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Idle_03,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Intro_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Loss_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5ExchangeA_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5ExchangeB_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5Intro_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5Victory_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Malfurion_Male_NightElf_Story_Jaina_Mission5Malfurion_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission5Thrall_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission5Victory_01,
      (string) BoH_Jaina_05.VO_Story_Hero_Tyrande_Female_NightElf_Story_Jaina_Mission5Tyrande_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Jaina_05 boHJaina05 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHJaina05.PlayLineAlways(actor, (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Intro_01);
    yield return (object) boHJaina05.PlayLineAlways(friendlyActor, (string) BoH_Jaina_05.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Death_01;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5EmoteResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Jaina_05 boHJaina05 = this;
    while (boHJaina05.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) boHJaina05.PlayLineAlways((string) BoH_Jaina_05.ThrallBrassRing, (string) BoH_Jaina_05.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission5Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 502:
        yield return (object) boHJaina05.PlayLineAlways((string) BoH_Jaina_05.MalfurionBrassRing, (string) BoH_Jaina_05.VO_Story_Hero_Malfurion_Male_NightElf_Story_Jaina_Mission5Malfurion_01);
        break;
      case 503:
        yield return (object) boHJaina05.PlayLineAlways((string) BoH_Jaina_05.TyrandeBrassRing, (string) BoH_Jaina_05.VO_Story_Hero_Tyrande_Female_NightElf_Story_Jaina_Mission5Tyrande_01);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHJaina05.PlayLineAlways(actor, (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 505:
        yield return (object) boHJaina05.PlayLineAlways((string) BoH_Jaina_05.ThrallBrassRing, (string) BoH_Jaina_05.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission5Thrall_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHJaina05.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Jaina_05 boHJaina05 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHJaina05.\u003C\u003En__1(entity);
    while (boHJaina05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHJaina05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHJaina05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHJaina05.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Jaina_05 boHJaina05 = this;
    while (boHJaina05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHJaina05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHJaina05.\u003C\u003En__2(entity);
      yield return (object) boHJaina05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHJaina05.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Jaina_05 boHJaina05 = this;
    while (boHJaina05.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) boHJaina05.PlayLineAlways(enemyActor, (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeA_01);
        yield return (object) boHJaina05.PlayLineAlways(enemyActor, (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeA_02);
        yield return (object) boHJaina05.PlayLineAlways(friendlyActor, (string) BoH_Jaina_05.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5ExchangeA_01);
        break;
      case 7:
        yield return (object) boHJaina05.PlayLineAlways(enemyActor, (string) BoH_Jaina_05.VO_Story_Hero_Archimonde_Male_Demon_Story_Jaina_Mission5ExchangeB_01);
        yield return (object) boHJaina05.PlayLineAlways(friendlyActor, (string) BoH_Jaina_05.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission5ExchangeB_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_DRGLOEBoss);
}
