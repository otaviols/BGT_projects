using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Guldan_01 : BoH_Guldan_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Guldan_01.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1EmoteResponse_01 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1EmoteResponse_01.prefab:3d6d08a08f854484cab7fdf670fe96f5");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1ExchangeA_01 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1ExchangeA_01.prefab:ee2caef077e4569478160403f96d94a0");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1ExchangeC_01 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1ExchangeC_01.prefab:b71b0496ffc4b9a4bb3e2814123441db");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_01 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_01.prefab:38c36a27982006d468451a3ed35333c5");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_02 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_02.prefab:ad838686f01e4664f958b1fbd3c83b2c");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_03 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_03.prefab:2e9d9a0f13aee794aa1960e1e7a89109");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_01 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_01.prefab:c064fa16da5a1504cb5ea9b4a33c63e3");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_02 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_02.prefab:e7164f22ee5194e4fa79435e1ebc18fc");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_03 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_03.prefab:04fd7dab2532eb14ea73e86415d8f63f");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Intro_01 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Intro_01.prefab:d1a77a6aa34251b4983d679839fa3682");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Loss_01 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Loss_01.prefab:75ac3be967664014fb53214d4ea2af24");
  private static readonly AssetReference VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Victory_03 = new AssetReference("VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Victory_03.prefab:ae727ee151014da4493afc2ac1b210d2");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeB_01 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeB_01.prefab:f4cb514566de85d47a612467cb678d86");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeC_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeC_02.prefab:326ebb565be06084db9ad503d145b3c0");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeD_01 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeD_01.prefab:62799953c1f2ce14f8093252008a4151");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1Intro_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1Intro_02.prefab:0e2d5c928cf563f48b89922b4e582e67");
  private static readonly AssetReference VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1ExchangeE_01 = new AssetReference("VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1ExchangeE_01.prefab:f1b8d4d6b669c494eb21f6745c3500ae");
  private static readonly AssetReference VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1Victory_01 = new AssetReference("VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1Victory_01.prefab:e211569e6f4bc8e4fa7c1dad0f50e214");
  private static readonly AssetReference VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1Victory_02 = new AssetReference("VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1Victory_02.prefab:94782207d8b00e84fac55c37b2920e80");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_01,
    (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_02,
    (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_01,
    (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_02,
    (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_03
  };
  private List<string> m_EmoteResponseLines = new List<string>()
  {
    (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1EmoteResponse_01
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

  public BoH_Guldan_01() => this.m_gameOptions.AddBooleanOptions(BoH_Guldan_01.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1EmoteResponse_01,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1ExchangeA_01,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1ExchangeC_01,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_01,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_02,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1HeroPower_03,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_01,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_02,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Idle_03,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Intro_01,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Loss_01,
      (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Victory_03,
      (string) BoH_Guldan_01.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeB_01,
      (string) BoH_Guldan_01.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeC_02,
      (string) BoH_Guldan_01.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeD_01,
      (string) BoH_Guldan_01.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1Intro_02,
      (string) BoH_Guldan_01.VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1ExchangeE_01,
      (string) BoH_Guldan_01.VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1Victory_01,
      (string) BoH_Guldan_01.VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1Victory_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Guldan_01 boHGuldan01 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHGuldan01.MissionPlayVO(actor, (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Intro_01);
    yield return (object) boHGuldan01.MissionPlayVO(friendlyActor, (string) BoH_Guldan_01.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

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
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_BT;
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1EmoteResponse_01;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Guldan_01 boHGuldan01 = this;
    while (boHGuldan01.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan01.MissionPlayVO("Story_09_ForgottenShaman", (string) BoH_Guldan_01.VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1Victory_01);
        yield return (object) boHGuldan01.MissionPlayVO("Story_09_ForgottenShaman", (string) BoH_Guldan_01.VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1Victory_02);
        yield return (object) boHGuldan01.MissionPlayVO(enemyActor, (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Victory_03);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan01.MissionPlayVO(enemyActor, (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHGuldan01.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Guldan_01 boHGuldan01 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHGuldan01.\u003C\u003En__1(entity);
    while (boHGuldan01.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGuldan01.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHGuldan01.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGuldan01.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "Story_09_ForgottenShaman")
        yield return (object) boHGuldan01.PlayLineOnlyOnce(boHGuldan01.GetFriendlyActorByCardId("Story_09_ForgottenShaman"), (string) BoH_Guldan_01.VO_Story_Minion_ForgottenShaman_Male_Orc_Story_Guldan_Mission1ExchangeE_01);
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Guldan_01 boHGuldan01 = this;
    while (boHGuldan01.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGuldan01.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHGuldan01.\u003C\u003En__2(entity);
      yield return (object) boHGuldan01.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGuldan01.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Guldan_01 boHGuldan01 = this;
    while (boHGuldan01.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHGuldan01.MissionPlayVO(actor, (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1ExchangeA_01);
        break;
      case 3:
        yield return (object) boHGuldan01.MissionPlayVO(friendlyActor, (string) BoH_Guldan_01.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeB_01);
        break;
      case 5:
        yield return (object) boHGuldan01.MissionPlayVO(actor, (string) BoH_Guldan_01.VO_Story_Hero_ForgottenWarrior_Male_Orc_Story_Guldan_Mission1ExchangeC_01);
        yield return (object) boHGuldan01.MissionPlayVO(friendlyActor, (string) BoH_Guldan_01.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeC_02);
        break;
      case 7:
        yield return (object) boHGuldan01.MissionPlayVO(friendlyActor, (string) BoH_Guldan_01.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission1ExchangeD_01);
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
