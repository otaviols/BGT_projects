using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Illidan_01 : BoH_Illidan_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Illidan_01.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1EmoteResponse_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1EmoteResponse_01.prefab:f9601d01c5e07994e9eb88e3cb7c550c");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeA_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeA_02.prefab:b5c467bf1f33fe84a8ed67422ca29bda");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeB_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeB_02.prefab:dafdf5bca0ad13844a4134df7f504d36");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeC_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeC_02.prefab:5474e6902fbad214bbe664006b220125");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeD_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeD_02.prefab:1f56f81cc6762c74b890d174aba83a64");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_01.prefab:6d8ad256547dd1a4d8d431f9d5896cdb");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_02.prefab:23088aa7014eac647872d19772bc2de0");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_03 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_03.prefab:30bd3cc23f4a4f749852eefd7c7a6a82");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_01.prefab:a9af498a83691f540adc5433ac60ca3c");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_02.prefab:748a11ae9a19b4244bae1c22cf614885");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_03 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_03.prefab:05e82716959a2d74196be8e1a6c2b60c");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Intro_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Intro_02.prefab:f280f79900ed8de41aa2c3655004c98b");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Loss_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Loss_01.prefab:19bdb2fb2e79e6a40ab8efdbeb07b8bb");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Victory_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Victory_02.prefab:6f80d2126308930429e38822f14de477");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeA_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeA_01.prefab:56279125c6ff401438e61d49113bdebd");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeB_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeB_01.prefab:c269c55479b6b5b40aea46903b2f80ca");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeC_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeC_01.prefab:87eaf15fe5e96ae4bad4f366e7c27aa2");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeD_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeD_01.prefab:ae9ff5a3b8853bf4a99fbec1afb568d4");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1Intro_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1Intro_01.prefab:29a3491548830934db423c073e18e962");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1Victory_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1Victory_01.prefab:18afd49352ebe934f89d29574dae20e7");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_01,
    (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_02,
    (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_01,
    (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_02,
    (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();
  private Notification m_turnCounter;

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Illidan_01() => this.m_gameOptions.AddBooleanOptions(BoH_Illidan_01.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1EmoteResponse_01,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeA_02,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeB_02,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeC_02,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeD_02,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_01,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_02,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1HeroPower_03,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_01,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_02,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Idle_03,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Intro_02,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Loss_01,
      (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Victory_02,
      (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeA_01,
      (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeB_01,
      (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeC_01,
      (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeD_01,
      (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1Intro_01,
      (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_DRG;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Illidan_01 boHIllidan01 = this;
    while (boHIllidan01.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan01.MissionPlayVO(actor, (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1Victory_01);
        yield return (object) boHIllidan01.MissionPlayVO(enemyActor, (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Victory_02);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan01.MissionPlayVO(enemyActor, (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 514:
        yield return (object) boHIllidan01.MissionPlayVO(actor, (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1Intro_01);
        yield return (object) boHIllidan01.MissionPlayVO(enemyActor, (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1Intro_02);
        break;
      case 515:
        yield return (object) boHIllidan01.MissionPlayVO(enemyActor, (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1EmoteResponse_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHIllidan01.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Illidan_01 boHIllidan01 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHIllidan01.\u003C\u003En__1(entity);
    while (boHIllidan01.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHIllidan01.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHIllidan01.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHIllidan01.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Illidan_01 boHIllidan01 = this;
    while (boHIllidan01.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHIllidan01.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHIllidan01.\u003C\u003En__2(entity);
      yield return (object) boHIllidan01.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHIllidan01.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Illidan_01 boHIllidan01 = this;
    while (boHIllidan01.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHIllidan01.MissionPlayVO(actor, (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeA_01);
        yield return (object) boHIllidan01.MissionPlayVO(enemyActor, (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeA_02);
        break;
      case 3:
        yield return (object) boHIllidan01.MissionPlayVO(actor, (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeB_01);
        yield return (object) boHIllidan01.MissionPlayVO(enemyActor, (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeB_02);
        break;
      case 7:
        yield return (object) boHIllidan01.MissionPlayVO(actor, (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeC_01);
        yield return (object) boHIllidan01.MissionPlayVO(enemyActor, (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeC_02);
        break;
      case 11:
        yield return (object) boHIllidan01.MissionPlayVO(actor, (string) BoH_Illidan_01.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission1ExchangeD_01);
        yield return (object) boHIllidan01.MissionPlayVO(enemyActor, (string) BoH_Illidan_01.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission1ExchangeD_02);
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

  private void UpdateTurnCounter(int cost)
  {
    PlayMakerFSM component = this.m_turnCounter.GetComponent<PlayMakerFSM>();
    if (component.ActiveStateName.Equals("Idle"))
      component.SendEvent("Action");
    if (cost <= 0)
      Object.Destroy((Object) this.m_turnCounter.gameObject);
    else
      this.UpdateTurnCounterText(cost);
  }

  private void UpdateTurnCounterText(int cost) => this.m_turnCounter.ChangeDialogText(GameStrings.FormatPlurals("BOH_ILLIDAN_01", new GameStrings.PluralNumber[1]
  {
    new GameStrings.PluralNumber()
    {
      m_index = 0,
      m_number = cost
    }
  }), cost.ToString(), "", "");

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ICCLichKing);
}
