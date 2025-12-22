using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Thrall_06 : BoH_Thrall_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Thrall_06.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6EmoteResponse_01 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6EmoteResponse_01.prefab:92b91b27228aec34b8b757611ef74bd4");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeA_01 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeA_01.prefab:73a5fb568fa366b4c9904861016a9145");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeB_01 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeB_01.prefab:19e5e2193d33adf49bc92537ceec0472");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeC_01 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeC_01.prefab:b30cd3ba963151b4fb33a35ad068e4ff");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_01 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_01.prefab:0d6af03e113ef2f42bf20ccdbd979e80");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_02 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_02.prefab:b6568430774a8534e82fb19876e637a7");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_03 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_03.prefab:6c0782548c43df243bc0d9046498dadc");
  private static readonly AssetReference VO__Male_Dragon_Death_01 = new AssetReference("VO__Male_Dragon_Death_01.prefab:321728cfd9ce8574fa48b0774c2512bb");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_01 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_01.prefab:d605daf6e3c8b6e499fab7dc3374034d");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_02 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_02.prefab:964549b2c57d67144b93e2d315511425");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_03 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_03.prefab:3e93e62d068fedd42a1ab1c5ab9fb7f3");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Intro_01 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Intro_01.prefab:cd55b9f860b573b4283466f1b0ec6316");
  private static readonly AssetReference VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Loss_01 = new AssetReference("VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Loss_01.prefab:9e77d973df735dc41bb3f32e73b6252e");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6ExchangeD_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6ExchangeD_01.prefab:2c5a654c942263b48917a0459c785320");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6Intro_02 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6Intro_02.prefab:53ba9fde0fe84f5429e38f29190b8b48");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6Victory_02 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6Victory_02.prefab:51ec8ac6cb0941f49952dffb934efc63");
  private static readonly AssetReference VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeE_01 = new AssetReference("VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeE_01.prefab:99a41c61c39ca6645b4151f2d75bf4b8");
  private static readonly AssetReference VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeF_01 = new AssetReference("VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeF_01.prefab:1e052585567ff7844b8ac4bf8324756b");
  private static readonly AssetReference VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeG_01 = new AssetReference("VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeG_01.prefab:223f14d322a4c724a8f9ad8d229a0aab");
  private static readonly AssetReference VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6Victory_03 = new AssetReference("VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6Victory_03.prefab:61bffbf62b059f94693c504e5fa1e7ec");
  private static readonly AssetReference VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6Victory_04 = new AssetReference("VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6Victory_04.prefab:823c1ceb303626b458af2574c260858d");
  private static readonly AssetReference VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeD_01 = new AssetReference("VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeD_01.prefab:63b57afdcabf7834891d0a24a7fa989d");
  private static readonly AssetReference VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeD_02 = new AssetReference("VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeD_02.prefab:e7e6581c52ede504e9a6c0587aec018a");
  private static readonly AssetReference VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeE_02 = new AssetReference("VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeE_02.prefab:cc779132bdcc70f489679151906a43eb");
  private static readonly AssetReference VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6Victory_01 = new AssetReference("VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6Victory_01.prefab:2417b626e021b0547ba62dad704d7319");
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      228,
      new string[1]{ "BOH_THRALL_06" }
    }
  };
  private float popUpScale = 1.25f;
  private Vector3 popUpPos;
  public static readonly AssetReference AlexstraszaBrassRing = new AssetReference("Alexstrasza_BrassRing_Quote.prefab:329e7a3e53fbf9f4997fea9db57921ba");
  public static readonly AssetReference YseraBrassRing = new AssetReference("Ysera_BrassRing_Quote.prefab:1b5ee7911e0cc0f48bff1d9ea60a95e1");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_01,
    (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_02,
    (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_01,
    (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_02,
    (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Thrall_06() => this.m_gameOptions.AddBooleanOptions(BoH_Thrall_06.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6EmoteResponse_01,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeA_01,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeB_01,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeC_01,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_01,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_02,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6HeroPower_03,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_01,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_02,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Idle_03,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Intro_01,
      (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Loss_01,
      (string) BoH_Thrall_06.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6ExchangeD_01,
      (string) BoH_Thrall_06.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6Intro_02,
      (string) BoH_Thrall_06.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6Victory_02,
      (string) BoH_Thrall_06.VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeE_01,
      (string) BoH_Thrall_06.VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeF_01,
      (string) BoH_Thrall_06.VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeG_01,
      (string) BoH_Thrall_06.VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6Victory_03,
      (string) BoH_Thrall_06.VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6Victory_04,
      (string) BoH_Thrall_06.VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeD_01,
      (string) BoH_Thrall_06.VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeD_02,
      (string) BoH_Thrall_06.VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeE_02,
      (string) BoH_Thrall_06.VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Thrall_06 boHThrall06 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHThrall06.MissionPlayVO(actor, (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Intro_01);
    yield return (object) boHThrall06.MissionPlayVO(friendlyActor, (string) BoH_Thrall_06.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_DRGEVILBoss;
    this.m_standardEmoteResponseLine = (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6EmoteResponse_01;
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
    BoH_Thrall_06 boHThrall06 = this;
    while (boHThrall06.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    boHThrall06.popUpPos = new Vector3(0.0f, 0.0f, -40f);
    switch (missionEvent)
    {
      case 101:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall06.PlayLineAlways(friendlyActor, (string) BoH_Thrall_06.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6ExchangeD_01);
        yield return (object) boHThrall06.PlayLineAlways((string) BoH_Thrall_06.YseraBrassRing, (string) BoH_Thrall_06.VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeE_02);
        GameState.Get().SetBusy(false);
        break;
      case 102:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall06.PlayLineAlways((string) BoH_Thrall_06.AlexstraszaBrassRing, (string) BoH_Thrall_06.VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeF_01);
        GameState.Get().SetBusy(false);
        break;
      case 103:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall06.PlayLineAlways((string) BoH_Thrall_06.YseraBrassRing, (string) BoH_Thrall_06.VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6Victory_01);
        yield return (object) boHThrall06.PlayLineAlways(friendlyActor, (string) BoH_Thrall_06.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission6Victory_02);
        GameState.Get().SetBusy(false);
        break;
      case 228:
        yield return (object) new WaitForSeconds(2f);
        Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, boHThrall06.popUpPos, TutorialEntity.GetTextScale() * boHThrall06.popUpScale, GameStrings.Get(boHThrall06.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
        NotificationManager.Get().DestroyNotification(popupText, 7.5f);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall06.PlayLineAlways((string) BoH_Thrall_06.AlexstraszaBrassRing, (string) BoH_Thrall_06.VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6Victory_03);
        yield return (object) boHThrall06.PlayLineAlways((string) BoH_Thrall_06.AlexstraszaBrassRing, (string) BoH_Thrall_06.VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6Victory_04);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall06.PlayLineAlways(actor, (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHThrall06.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Thrall_06 boHThrall06 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHThrall06.\u003C\u003En__1(entity);
    while (boHThrall06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHThrall06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHThrall06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHThrall06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Thrall_06 boHThrall06 = this;
    while (boHThrall06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHThrall06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHThrall06.\u003C\u003En__2(entity);
      yield return (object) boHThrall06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHThrall06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Thrall_06 boHThrall06 = this;
    while (boHThrall06.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 2:
        yield return (object) boHThrall06.PlayLineAlways(actor, (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeA_01);
        yield return (object) boHThrall06.PlayLineAlways((string) BoH_Thrall_06.AlexstraszaBrassRing, (string) BoH_Thrall_06.VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeE_01);
        break;
      case 7:
        yield return (object) boHThrall06.PlayLineAlways(actor, (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeB_01);
        break;
      case 9:
        yield return (object) boHThrall06.PlayLineAlways(actor, (string) BoH_Thrall_06.VO_Story_Hero_Deathwing_Male_Dragon_Story_Thrall_Mission6ExchangeC_01);
        break;
      case 13:
        yield return (object) boHThrall06.PlayLineAlways((string) BoH_Thrall_06.YseraBrassRing, (string) BoH_Thrall_06.VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeD_01);
        yield return (object) boHThrall06.PlayLineAlways((string) BoH_Thrall_06.YseraBrassRing, (string) BoH_Thrall_06.VO_Story_Minion_Ysera_Female_Dragon_Story_Thrall_Mission6ExchangeD_02);
        break;
      case 17:
        yield return (object) boHThrall06.PlayLineAlways((string) BoH_Thrall_06.AlexstraszaBrassRing, (string) BoH_Thrall_06.VO_Story_Minion_Alexstrasza_Female_Dragon_Story_Thrall_Mission6ExchangeG_01);
        break;
    }
  }
}
