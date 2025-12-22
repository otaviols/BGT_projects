using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Thrall_07 : BoH_Thrall_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Thrall_07.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7EmoteResponse_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7EmoteResponse_01.prefab:29d04d98f2139ee4b8ed9be78849a6c3");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_01.prefab:a1771c2114113dc4799fe2950b7fe962");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_02 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_02.prefab:4590b4043aebd2049bd41a17b6048dd5");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_03 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_03.prefab:e687658b25852074f939282780a2be5e");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_01.prefab:2f9f0c222d89c60468637b8c00d8dd47");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_02 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_02.prefab:8e901a4089eaa1b47a844a2e6b14edb0");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_03 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_03.prefab:2a2e0b4e4e51fc14fa6799029155bfc8");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Loss_01 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Loss_01.prefab:9e45336303bd3734ea4055deee1078eb");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeA_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeA_01.prefab:980dcad4747710348b8e2a7117577207");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeB_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeB_01.prefab:4fc9e8f8c3aac4148bc046d43ce9d350");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeC_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeC_01.prefab:48fa0915dce887740a4e4c4b9d2823da");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Intro_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Intro_01.prefab:f28cd05ec9568ce4384e017c39d3cf13");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Victory_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Victory_01.prefab:525262ba5777e8b48af81a62307d2ff7");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Victory_02 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Victory_02.prefab:af80af5ad0c24bd43a1d04f54833ce0d");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7ExchangeA_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7ExchangeA_01.prefab:e70ae11b2378456468d6c9c0cbdccb3e");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7ExchangeB_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7ExchangeB_01.prefab:043d0360388b6c24c9426d3e0a4f6bc7");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7Intro_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7Intro_01.prefab:c6244eb1776cd42448f80845893eff5c");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission7Victory_05 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission7Victory_05.prefab:b7638baf36b08374ba3562e6752b4b0b");
  private static readonly AssetReference VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission7Victory_01 = new AssetReference("VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission7Victory_01.prefab:c997a6c7c909d90469040fc6b5523ff2");
  private static readonly AssetReference VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission7Victory_02 = new AssetReference("VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission7Victory_02.prefab:499dc5892f61dd3459174735dd2a4f7c");
  public static readonly AssetReference KalecBrassRing = new AssetReference("Kalec_BrassRing_Quote.prefab:b96062478a5eccd47bd5e94f1ad7312f");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_01,
    (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_02,
    (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_01,
    (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_02,
    (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Thrall_07() => this.m_gameOptions.AddBooleanOptions(BoH_Thrall_07.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7EmoteResponse_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_02,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7HeroPower_03,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_02,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Idle_03,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Loss_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeA_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeB_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeC_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Intro_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Victory_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Victory_02,
      (string) BoH_Thrall_07.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7ExchangeA_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7ExchangeB_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7Intro_01,
      (string) BoH_Thrall_07.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission7Victory_05,
      (string) BoH_Thrall_07.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission7Victory_01,
      (string) BoH_Thrall_07.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission7Victory_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Thrall_07 boHThrall07 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHThrall07.MissionPlayVO(actor, (string) BoH_Thrall_07.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7Intro_01);
    yield return (object) boHThrall07.MissionPlayVO(enemyActor, (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_DRG;
    this.m_standardEmoteResponseLine = (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7EmoteResponse_01;
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
    BoH_Thrall_07 boHThrall07 = this;
    while (boHThrall07.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall07.PlayLineAlways((string) BoH_Thrall_07.KalecBrassRing, (string) BoH_Thrall_07.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission7Victory_01);
        yield return (object) boHThrall07.PlayLineAlways(enemyActor, (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Victory_01);
        yield return (object) boHThrall07.PlayLineAlways((string) BoH_Thrall_07.KalecBrassRing, (string) BoH_Thrall_07.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission7Victory_02);
        yield return (object) boHThrall07.PlayLineAlways(enemyActor, (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7Victory_02);
        yield return (object) boHThrall07.PlayLineAlways(friendlyActor, (string) BoH_Thrall_07.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission7Victory_05);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall07.PlayLineAlways(enemyActor, (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Female_Human_Story_Thrall_Mission7Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHThrall07.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Thrall_07 boHThrall07 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHThrall07.\u003C\u003En__1(entity);
    while (boHThrall07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHThrall07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHThrall07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHThrall07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Thrall_07 boHThrall07 = this;
    while (boHThrall07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHThrall07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHThrall07.\u003C\u003En__2(entity);
      yield return (object) boHThrall07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHThrall07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Thrall_07 boHThrall07 = this;
    while (boHThrall07.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) boHThrall07.PlayLineAlways(enemyActor, (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeA_01);
        yield return (object) boHThrall07.PlayLineAlways(friendlyActor, (string) BoH_Thrall_07.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7ExchangeA_01);
        break;
      case 7:
        yield return (object) boHThrall07.PlayLineAlways(friendlyActor, (string) BoH_Thrall_07.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission7ExchangeB_01);
        yield return (object) boHThrall07.PlayLineAlways(enemyActor, (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeB_01);
        break;
      case 11:
        yield return (object) boHThrall07.PlayLineAlways(enemyActor, (string) BoH_Thrall_07.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission7ExchangeC_01);
        break;
    }
  }
}
