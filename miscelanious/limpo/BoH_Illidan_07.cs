using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Illidan_07 : BoH_Illidan_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Illidan_07.InitBooleanOptions();
  private static readonly AssetReference VO_EX1_614_Male_NightElf_HunterPrince_Start2Response_01 = new AssetReference("VO_EX1_614_Male_NightElf_HunterPrince_Start2Response_01.prefab:ccd6345a277977f47867c65a485a87cf");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7EmoteResponse_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7EmoteResponse_01.prefab:cade225bbda4a5a48a5127551daf6b0a");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7ExchangeA_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7ExchangeA_01.prefab:d413829bb23c380448f5ba1653bc4d44");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7ExchangeC_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7ExchangeC_01.prefab:90bc14b05879f764aa26dbdd038ea7cb");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_01.prefab:72358aa41973f9d4db1d32ffc04ae267");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_02.prefab:1abd0a82ab2e5264b83532ec144b687a");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_03 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_03.prefab:49011e222271aa84d8a2789427a55476");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_01.prefab:6e485fd7ad42cfe40b21128c0e4e9755");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_02.prefab:bd1d627ad3c471e46b172dab0e5484f3");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_03 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_03.prefab:bd9adebf6f7b8f449bf2b671e3defb9d");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Intro_02 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Intro_02.prefab:be8c9266db92ee544b5f648aa5f89b09");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Loss_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Loss_01.prefab:41ce798073bbea64b9a4183ac042e511");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Victory_01 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Victory_01.prefab:ecd57d9ab0608ac48b7a598d38f09b6a");
  private static readonly AssetReference VO_Story_Hero_Arthas_Male_Human_Story_IllidanMission7Obelisk_04 = new AssetReference("VO_Story_Hero_Arthas_Male_Human_Story_IllidanMission7Obelisk_04.prefab:e22623cfcc99dfa4a97d40d024d4a154");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7ExchangeA_02 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7ExchangeA_02.prefab:2aa56f90d40c400459f6f2462cc64f4d");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7ExchangeB_02 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7ExchangeB_02.prefab:834652c2178cfd14991dc6c13efe66ab");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7Intro_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7Intro_01.prefab:3b629bd15380b784f9d1ce0d0e8bc51f");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7Victory_04 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7Victory_04.prefab:7533c54477aea774aad30af9c2dcdf79");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_01 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_01.prefab:4f98f3aff84def34b943ff36da0dde4f");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_02 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_02.prefab:62e999f8771143047bcfc34131273632");
  private static readonly AssetReference VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_03 = new AssetReference("VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_03.prefab:38adb6e726286e340a70362df7058ad1");
  private static readonly AssetReference VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission7ExchangeB_01 = new AssetReference("VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission7ExchangeB_01.prefab:d730801b498f48d439c83cefc0f3d796");
  private static readonly AssetReference VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission7ExchangeC_02 = new AssetReference("VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission7ExchangeC_02.prefab:e2a040d54830f82439056582427890b1");
  private static readonly AssetReference VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Start2_01 = new AssetReference("VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Start2_01.prefab:c6f6c665e36826242abf83366398e5ef");
  private static readonly AssetReference VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Start3_01 = new AssetReference("VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Start3_01.prefab:e290f2d7e1bebac428d4a1d388aa5fcf");
  public static readonly AssetReference KaelthasBrassRing = new AssetReference("Kaelthas_BrassRing_Quote.prefab:e2c98e804ab04dd49bfbd665c1647eca");
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_01,
    (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_02,
    (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_03
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

  public BoH_Illidan_07() => this.m_gameOptions.AddBooleanOptions(BoH_Illidan_07.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Illidan_07.VO_EX1_614_Male_NightElf_HunterPrince_Start2Response_01,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7EmoteResponse_01,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7ExchangeA_01,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7ExchangeC_01,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_01,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_02,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_03,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_01,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_02,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Idle_03,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Intro_02,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Loss_01,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Victory_01,
      (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_IllidanMission7Obelisk_04,
      (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7ExchangeA_02,
      (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7ExchangeB_02,
      (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7Intro_01,
      (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7Victory_04,
      (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_01,
      (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_02,
      (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_03,
      (string) BoH_Illidan_07.VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission7ExchangeB_01,
      (string) BoH_Illidan_07.VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission7ExchangeC_02,
      (string) BoH_Illidan_07.VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Start2_01,
      (string) BoH_Illidan_07.VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Start3_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_ICCLichKing;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Illidan_07 boHIllidan07 = this;
    while (boHIllidan07.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan07.MissionPlayVO(friendlyActor, (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7Victory_04);
        yield return (object) boHIllidan07.MissionPlayVO(enemyActor, (string) BoH_Illidan_07.VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Start2_01);
        GameState.Get().SetBusy(false);
        break;
      case 102:
        yield return (object) boHIllidan07.MissionPlayVOOnce(friendlyActor, (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_02);
        break;
      case 103:
        yield return (object) boHIllidan07.MissionPlayVOOnce(friendlyActor, (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_03);
        break;
      case 104:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan07.MissionPlayVOOnce(enemyActor, (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_IllidanMission7Obelisk_04);
        GameState.Get().SetBusy(false);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan07.MissionPlayVO(enemyActor, (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Victory_01);
        yield return (object) boHIllidan07.MissionPlayVO(friendlyActor, (string) BoH_Illidan_07.VO_EX1_614_Male_NightElf_HunterPrince_Start2Response_01);
        yield return (object) boHIllidan07.MissionPlayVO(enemyActor, (string) BoH_Illidan_07.VO_TB_PrinceHunter_ArthasH_Male_Human_HunterPrince_Start3_01);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHIllidan07.MissionPlayVO(enemyActor, (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 514:
        yield return (object) boHIllidan07.MissionPlayVO(friendlyActor, (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7Intro_01);
        yield return (object) boHIllidan07.MissionPlayVO(enemyActor, (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7Intro_02);
        break;
      case 515:
        yield return (object) boHIllidan07.MissionPlayVO(enemyActor, (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7EmoteResponse_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHIllidan07.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Illidan_07 boHIllidan07 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHIllidan07.\u003C\u003En__1(entity);
    while (boHIllidan07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHIllidan07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHIllidan07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHIllidan07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Illidan_07 boHIllidan07 = this;
    while (boHIllidan07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHIllidan07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHIllidan07.\u003C\u003En__2(entity);
      yield return (object) boHIllidan07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHIllidan07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Illidan_07 boHIllidan07 = this;
    while (boHIllidan07.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHIllidan07.MissionPlayVO(friendlyActor, (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_IllidanMission7Obelisk_01);
        break;
      case 3:
        yield return (object) boHIllidan07.MissionPlayVO(actor, (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7ExchangeA_01);
        yield return (object) boHIllidan07.MissionPlayVO(friendlyActor, (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7ExchangeA_02);
        break;
      case 7:
        yield return (object) boHIllidan07.MissionPlayVO(BoH_Illidan_07.KaelthasBrassRing, (string) BoH_Illidan_07.VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission7ExchangeB_01);
        yield return (object) boHIllidan07.MissionPlayVO(friendlyActor, (string) BoH_Illidan_07.VO_Story_Hero_Illidan_Male_NightElf_Story_Illidan_Mission7ExchangeB_02);
        break;
      case 9:
        yield return (object) boHIllidan07.MissionPlayVO(actor, (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_01);
        break;
      case 11:
        yield return (object) boHIllidan07.MissionPlayVO(actor, (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_03);
        break;
      case 13:
        yield return (object) boHIllidan07.MissionPlayVO(actor, (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7ExchangeC_01);
        yield return (object) boHIllidan07.MissionPlayVO(BoH_Illidan_07.KaelthasBrassRing, (string) BoH_Illidan_07.VO_Story_Minion_Kaelthas_Male_BloodElf_Story_Illidan_Mission7ExchangeC_02);
        break;
      case 17:
        yield return (object) boHIllidan07.MissionPlayVO(actor, (string) BoH_Illidan_07.VO_Story_Hero_Arthas_Male_Human_Story_Illidan_Mission7HeroPower_02);
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
    this.m_turnCounter.GetComponent<PlayMakerFSM>().SendEvent("Action");
    if (cost <= 0)
      Object.Destroy((Object) this.m_turnCounter.gameObject);
    else
      this.UpdateTurnCounterText(cost);
  }

  private void UpdateTurnCounterText(int cost) => this.m_turnCounter.ChangeDialogText(GameStrings.FormatPlurals("BOH_ILLIDAN_07", new GameStrings.PluralNumber[1]
  {
    new GameStrings.PluralNumber()
    {
      m_index = 0,
      m_number = cost
    }
  }), cost.ToString(), "", "");

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_ICCLichKing);
}
