using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Guldan_06 : BoH_Guldan_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Guldan_06.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeA_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeA_02.prefab:d02b73708d70f6647806a91e91fc9852");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeB_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeB_02.prefab:f46a6251450ef6845a3c29f80e0544cf");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeC_03 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeC_03.prefab:f0e4e57717f07df41b7265c7ecf69677");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Intro_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Intro_02.prefab:e98dd0e3c2149364eb6850cf794f09b1");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Victory_01 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Victory_01.prefab:c52ef8e93af9b4f4da8e20e4c16b9210");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Victory_03 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Victory_03.prefab:30750d611fba514428fb45d3cf267082");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeA_01 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeA_01.prefab:716d77ffe1d568b4481d617e94ea291b");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeB_01 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeB_01.prefab:aff68f116a02edb44909d83af5b6e499");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeB_03 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeB_03.prefab:6cba2a0f8f0203448b4ab56c5d4a80d0");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeC_01 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeC_01.prefab:8a9571fbfed1a0341b1c53e0b22667f2");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeC_02 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeC_02.prefab:7ac9955dabafd104a9175899021b7e82");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_01 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_01.prefab:977e9a6b8fe0f544aa45601ffe2eb63f");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_02 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_02.prefab:da9581005b7052f44bb369792a2ec9ca");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_03 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_03.prefab:5f70abcb08c3b0e4b9ad4bf449be3cf0");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_01 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_01.prefab:8021f1193d6b087429bd1b2b76eeadfa");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_02 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_02.prefab:5992baea089bcfc4bab32d2d3604373a");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_03 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_03.prefab:54b52465eeba51e41a127ef5144ba2e1");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Intro_01 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Intro_01.prefab:13e8f550236307449b9a6a73cf14936f");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Loss_01 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Loss_01.prefab:637e154960b389e44b18ee7e150637cf");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Victory_02 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Victory_02.prefab:4760acecf9924664abb31abdfec23e1c");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Victory_04 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Victory_04.prefab:601b203cf07e8d74d98703727ebc81c4");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7EmoteResponse_01 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7EmoteResponse_01.prefab:1faa804e3bfa3df49905770bcaa71bc1");
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_01,
    (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_02,
    (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_03
  };
  private List<string> m_EmoteResponseLines = new List<string>()
  {
    (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7EmoteResponse_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();
  private Notification m_turnCounter;
  private MineCartRushArt m_mineCartArt;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Guldan_06() => this.m_gameOptions.AddBooleanOptions(BoH_Guldan_06.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeA_02,
      (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeB_02,
      (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeC_03,
      (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Intro_02,
      (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Victory_01,
      (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Victory_03,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeA_01,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeB_01,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeB_03,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeC_01,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeC_02,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_01,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_02,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_03,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_01,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_02,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Idle_03,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Intro_01,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Loss_01,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Victory_02,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Victory_04,
      (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7EmoteResponse_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Guldan_06 boHGuldan06 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHGuldan06.MissionPlayVO(actor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Intro_01);
    yield return (object) boHGuldan06.MissionPlayVO(friendlyActor, (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  public override void OnCreateGame()
  {
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_BRMAdventure;
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7EmoteResponse_01;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Guldan_06 boHGuldan06 = this;
    while (boHGuldan06.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan06.MissionPlayVO(friendlyActor, (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Victory_01);
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Victory_02);
        yield return (object) boHGuldan06.MissionPlayVO(friendlyActor, (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6Victory_03);
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Victory_04);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHGuldan06.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Guldan_06 boHGuldan06 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHGuldan06.\u003C\u003En__1(entity);
    while (boHGuldan06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGuldan06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHGuldan06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGuldan06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Guldan_06 boHGuldan06 = this;
    while (boHGuldan06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGuldan06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHGuldan06.\u003C\u003En__2(entity);
      yield return (object) boHGuldan06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGuldan06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Guldan_06 boHGuldan06 = this;
    while (boHGuldan06.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeA_01);
        yield return (object) boHGuldan06.MissionPlayVO(friendlyActor, (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeA_02);
        break;
      case 7:
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeB_01);
        yield return (object) boHGuldan06.MissionPlayVO(friendlyActor, (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeB_02);
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeB_03);
        break;
      case 9:
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeC_01);
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6ExchangeC_02);
        yield return (object) boHGuldan06.MissionPlayVO(friendlyActor, (string) BoH_Guldan_06.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission6ExchangeC_03);
        break;
      case 12:
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_01);
        break;
      case 14:
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_02);
        break;
      case 18:
        yield return (object) boHGuldan06.MissionPlayVO(enemyActor, (string) BoH_Guldan_06.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission6HeroPower_03);
        break;
    }
  }

  public override void NotifyOfMulliganEnded()
  {
    base.NotifyOfMulliganEnded();
    this.InitVisuals();
  }

  private void InitVisuals() => this.InitTurnCounter(this.GetCost());

  public override void OnTagChanged(TagDelta change)
  {
    base.OnTagChanged(change);
    if (change.tag != 48 || change.newValue == change.oldValue)
      return;
    this.UpdateVisuals(change.newValue);
  }

  private void InitTurnCounter(int cost)
  {
    this.m_turnCounter = AssetLoader.Get().InstantiatePrefab((AssetReference) "LOE_Turn_Timer.prefab:b05530aa55868554fb8f0c66632b3c22").GetComponent<Notification>();
    PlayMakerFSM component = this.m_turnCounter.GetComponent<PlayMakerFSM>();
    component.FsmVariables.GetFsmBool("RunningMan").Value = true;
    component.FsmVariables.GetFsmBool("MineCart").Value = false;
    component.FsmVariables.GetFsmBool("Airship").Value = false;
    component.FsmVariables.GetFsmBool("Destroyer").Value = false;
    component.SendEvent("Birth");
    this.m_turnCounter.transform.parent = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor().gameObject.transform;
    this.m_turnCounter.transform.localPosition = new Vector3(-1.4f, 0.187f, -0.11f);
    this.m_turnCounter.transform.localScale = Vector3.one * 0.52f;
    this.UpdateTurnCounterText(cost);
  }

  private void UpdateVisuals(int cost) => this.UpdateTurnCounter(cost);

  private void UpdateMineCartArt() => this.m_mineCartArt.DoPortraitSwap(GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor());

  private void UpdateTurnCounter(int cost)
  {
    this.m_turnCounter.GetComponent<PlayMakerFSM>().SendEvent("Action");
    if (cost <= 0)
      Object.Destroy((Object) this.m_turnCounter.gameObject);
    else
      this.UpdateTurnCounterText(cost);
  }

  private void UpdateTurnCounterText(int cost) => this.m_turnCounter.ChangeDialogText(GameStrings.FormatPlurals("BOH_GULDAN_01", new GameStrings.PluralNumber[1]
  {
    new GameStrings.PluralNumber()
    {
      m_index = 0,
      m_number = cost
    }
  }), cost.ToString(), "", "");
}
