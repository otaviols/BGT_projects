using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Jaina_06 : BoH_Jaina_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Jaina_06.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6EmoteResponse_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6EmoteResponse_01.prefab:0008dcdf6af7a634a95520271a435cca");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6ExchangeA_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6ExchangeA_01.prefab:70108c0bc54581b45824711a1339d995");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6ExchangeB_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6ExchangeB_01.prefab:fd52a2692a7029a489fe89d01bb2e700");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_01.prefab:19bed1ab98b7f074c8e060ab12c82392");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_02.prefab:eb49ba598eea48369e524a76b6c20d46");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_03.prefab:6565ac890eee41428f3eeef7f2e3c063");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_01.prefab:d0894223340afec4780d9b3bb2a6164e");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_02.prefab:ba5271a4f8fb42f4d9114d0d168a29bb");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_03.prefab:56a99a8ebdad8ca48a998840731a002c");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Intro_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Intro_01.prefab:5b6fd62f56bf2ed49b7521cb031accde");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Loss_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Loss_01.prefab:18b2ce084f187ef4aba70e8c84fba6b1");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Victory_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Victory_01.prefab:3853008ffe2d76644a6267865191a8c4");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeA_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeA_01.prefab:61bacbf4a566b464bb984cc9dec7ff1a");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeB_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeB_01.prefab:46dd9feda9fb8ec44ad324269cfda61a");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeC_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeC_01.prefab:eebaeeda2975d8d4a836540db88e8202");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesAttacked_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesAttacked_01.prefab:0ffd0e856e107194e930bfadde42634a");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesAttacked_02 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesAttacked_02.prefab:c452ecd2cdc46cc4384f876c28d0dc0b");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesGone_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesGone_01.prefab:aba652abd2add6b4492e0343539be428");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6Intro_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6Intro_01.prefab:999a6a662df578943afb152b1fdc9451");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6Victory_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6Victory_01.prefab:951cdabc40fe62d4cacaee2fa730b402");
  private static readonly AssetReference VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6ExchangeC_01 = new AssetReference("VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6ExchangeC_01.prefab:7fbd004de4db7804a8c7139eaebe9ba5");
  private static readonly AssetReference VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6Victory_01 = new AssetReference("VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6Victory_01.prefab:72ff20259dc92d147841a3ad16995802");
  private static readonly AssetReference VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6Victory_02 = new AssetReference("VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6Victory_02.prefab:16deb7e0336609145b5b2da4de482ef9");
  public static readonly AssetReference KalecBrassRing = new AssetReference("Kalec_BrassRing_Quote.prefab:b96062478a5eccd47bd5e94f1ad7312f");
  private List<string> m_VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPowerLines = new List<string>()
  {
    (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_01,
    (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_02,
    (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_03
  };
  private List<string> m_VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6IdleLines = new List<string>()
  {
    (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_01,
    (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_02,
    (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_03
  };
  private List<string> m_VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesAttackedLines = new List<string>()
  {
    (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesAttacked_01,
    (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesAttacked_02,
    (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesGone_01
  };
  private int GatesAttackedCounter;
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Jaina_06() => this.m_gameOptions.AddBooleanOptions(BoH_Jaina_06.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6EmoteResponse_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6ExchangeA_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6ExchangeB_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_02,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPower_03,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_02,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Idle_03,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Intro_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Loss_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Victory_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeA_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeB_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeC_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesAttacked_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesAttacked_02,
      (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesGone_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6Intro_01,
      (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6Victory_01,
      (string) BoH_Jaina_06.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6ExchangeC_01,
      (string) BoH_Jaina_06.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6Victory_01,
      (string) BoH_Jaina_06.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6Victory_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Jaina_06 boHJaina06 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHJaina06.PlayLineAlways(actor, (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6Intro_01);
    yield return (object) boHJaina06.PlayLineAlways(enemyActor, (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6HeroPowerLines;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6EmoteResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Jaina_06 boHJaina06 = this;
    while (boHJaina06.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        CameraShakeMgr.Shake(Camera.main, new Vector3(0.3f, 0.3f, 0.3f), 1f);
        yield return (object) new WaitForSeconds(0.5f);
        CameraShakeMgr.Shake(Camera.main, new Vector3(0.2f, 0.2f, 0.2f), 5f);
        yield return (object) new WaitForSeconds(5f);
        CameraShakeMgr.Shake(Camera.main, new Vector3(0.25f, 0.25f, 0.25f), 5f);
        yield return (object) new WaitForSeconds(5.5f);
        CameraShakeMgr.Shake(Camera.main, new Vector3(0.8f, 0.8f, 0.8f), 0.5f);
        break;
      case 102:
        yield return (object) new WaitForSeconds(5f);
        CameraShakeMgr.Shake(Camera.main, new Vector3(0.4f, 0.4f, 0.4f), 5f);
        break;
      case 103:
        yield return (object) new WaitForSeconds(10f);
        CameraShakeMgr.Shake(Camera.main, new Vector3(0.6f, 0.6f, 0.6f), 5f);
        break;
      case 104:
        yield return (object) new WaitForSeconds(15f);
        CameraShakeMgr.Shake(Camera.main, new Vector3(0.8f, 0.8f, 0.8f), 5f);
        break;
      case 105:
        yield return (object) new WaitForSeconds(20f);
        CameraShakeMgr.Shake(Camera.main, new Vector3(1f, 1f, 1f), 5f);
        break;
      case 106:
        GameState.Get().SetBusy(true);
        yield return (object) boHJaina06.PlayLineAlways((string) BoH_Jaina_06.KalecBrassRing, (string) BoH_Jaina_06.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 107:
        GameState.Get().SetBusy(true);
        yield return (object) boHJaina06.PlayLineAlways((string) BoH_Jaina_06.KalecBrassRing, (string) BoH_Jaina_06.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6Victory_02);
        GameState.Get().SetBusy(false);
        break;
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) boHJaina06.PlayLineAlways(actor, (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Victory_01);
        yield return (object) boHJaina06.PlayLineAlways((string) BoH_Jaina_06.KalecBrassRing, (string) BoH_Jaina_06.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6Victory_01);
        yield return (object) boHJaina06.PlayLineAlways(friendlyActor, (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6Victory_01);
        yield return (object) boHJaina06.PlayLineAlways((string) BoH_Jaina_06.KalecBrassRing, (string) BoH_Jaina_06.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6Victory_02);
        GameState.Get().SetBusy(false);
        break;
      case 502:
        ++boHJaina06.GatesAttackedCounter;
        if (boHJaina06.GatesAttackedCounter == 3)
        {
          yield return (object) null;
          break;
        }
        yield return (object) boHJaina06.PlayLineInOrderOnce(friendlyActor, boHJaina06.m_VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6GatesAttackedLines);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHJaina06.PlayLineAlways(actor, (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 911:
        GameState.Get().SetBusy(true);
        while (boHJaina06.m_enemySpeaking)
          yield return (object) null;
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHJaina06.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Jaina_06 boHJaina06 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHJaina06.\u003C\u003En__1(entity);
    while (boHJaina06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHJaina06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHJaina06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHJaina06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Jaina_06 boHJaina06 = this;
    while (boHJaina06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHJaina06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHJaina06.\u003C\u003En__2(entity);
      yield return (object) boHJaina06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHJaina06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Jaina_06 boHJaina06 = this;
    while (boHJaina06.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHJaina06.PlayLineAlways(friendlyActor, (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeA_01);
        yield return (object) boHJaina06.PlayLineAlways(enemyActor, (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6ExchangeA_01);
        break;
      case 5:
        yield return (object) boHJaina06.PlayLineAlways(friendlyActor, (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeB_01);
        yield return (object) boHJaina06.PlayLineAlways(enemyActor, (string) BoH_Jaina_06.VO_Story_Hero_Garrosh_Male_Orc_Story_Jaina_Mission6ExchangeB_01);
        break;
      case 15:
        yield return (object) boHJaina06.PlayLineAlways((string) BoH_Jaina_06.KalecBrassRing, (string) BoH_Jaina_06.VO_Story_Minion_Kalec_Male_Dragon_Story_Jaina_Mission6ExchangeC_01);
        yield return (object) boHJaina06.PlayLineAlways(friendlyActor, (string) BoH_Jaina_06.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission6ExchangeC_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BT);
}
