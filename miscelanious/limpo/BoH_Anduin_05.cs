using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Anduin_05 : BoH_Anduin_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Anduin_05.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeA_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeA_02.prefab:bc61c1e319bfb5d4b94720110a1189a7");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeB_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeB_01.prefab:8b2fdc4eac40aaa4d9cfe2cc67cbd72b");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeC_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeC_02.prefab:5376724e895eb404c8f1ad6083d68a38");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5Intro_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5Intro_02.prefab:2c701b7a27367c646850c2cdeed77346");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5Victory_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5Victory_02.prefab:dfd3c765dc71a4d479c24a5dda824df7");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5_Victory_02 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5_Victory_02.prefab:0ceb5ef3b9d04334da52899fb3a71d67");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5EmoteResponse_01 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5EmoteResponse_01.prefab:b316228640c03d149a9772f08640ce2d");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeA_01 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeA_01.prefab:ae486fe9671c0134dbdb96bc845907d3");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeB_03 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeB_03.prefab:f892e3d0f01a0b648b282ededb205e71");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeC_01 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeC_01.prefab:418169ac580637049a4a4a2566eb52e7");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeC_03 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeC_03.prefab:f876d8b651f658b47a1a90668f1d2db3");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_01 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_01.prefab:815967509a6616343848ab05b550690a");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_02 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_02.prefab:644895d0e04984947b918db903becc9e");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_03 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_03.prefab:9fe4ff8f29377ce42bf5cde8c36eeb35");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_01 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_01.prefab:13e69112490f47148918f0a6926b348b");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_02 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_02.prefab:07be5a64ffe5e7e41957030a96e9b798");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_03 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_03.prefab:e7bfd48dd09747a4ca4fbfaa1ebec1b3");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Intro_01 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Intro_01.prefab:68ae44cfd8533a14c9ace692fcd3e560");
  private static readonly AssetReference VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Loss_01 = new AssetReference("VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Loss_01.prefab:3cd28516b1aff7e4c89694f97ccf3658");
  private static readonly AssetReference VO_Story_Minion_HordeAdventurer_Female_Orc_Story_Anduin_Mission5_Victory_01 = new AssetReference("VO_Story_Minion_HordeAdventurer_Female_Orc_Story_Anduin_Mission5_Victory_01.prefab:c84554ab589247040b070341fa4fe95d");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_01,
    (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_02,
    (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_01,
    (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_02,
    (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Anduin_05() => this.m_gameOptions.AddBooleanOptions(BoH_Anduin_05.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Anduin_05.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeA_02,
      (string) BoH_Anduin_05.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeB_01,
      (string) BoH_Anduin_05.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeC_02,
      (string) BoH_Anduin_05.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5Intro_02,
      (string) BoH_Anduin_05.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5Victory_02,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5_Victory_02,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5EmoteResponse_01,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeA_01,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeB_03,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeC_01,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeC_03,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_01,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_02,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5HeroPower_03,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_01,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_02,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Idle_03,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Intro_01,
      (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Loss_01,
      (string) BoH_Anduin_05.VO_Story_Minion_HordeAdventurer_Female_Orc_Story_Anduin_Mission5_Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Anduin_05 boHAnduin05 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHAnduin05.MissionPlayVO(actor, (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Intro_01);
    yield return (object) boHAnduin05.MissionPlayVO(friendlyActor, (string) BoH_Anduin_05.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMulliganMusicTrack = MusicPlaylistType.InGame_BT;
    this.m_standardEmoteResponseLine = (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5EmoteResponse_01;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Anduin_05 boHAnduin05 = this;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHAnduin05.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      while (boHAnduin05.m_enemySpeaking)
        yield return (object) null;
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      switch (missionEvent)
      {
        case 101:
          GameState.Get().SetBusy(true);
          yield return (object) boHAnduin05.MissionPlayVO(boHAnduin05.GetEnemyActorByCardId("Story_05_HordeAdventurer"), (string) BoH_Anduin_05.VO_Story_Minion_HordeAdventurer_Female_Orc_Story_Anduin_Mission5_Victory_01);
          yield return (object) boHAnduin05.MissionPlayVO(enemyActor, (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5_Victory_02);
          yield return (object) boHAnduin05.MissionPlayVO(friendlyActor, (string) BoH_Anduin_05.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5Victory_02);
          GameState.Get().SetBusy(false);
          break;
        case 507:
          GameState.Get().SetBusy(true);
          yield return (object) boHAnduin05.MissionPlayVO(enemyActor, (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5Loss_01);
          GameState.Get().SetBusy(false);
          break;
        case 515:
          yield return (object) boHAnduin05.MissionPlayVO(enemyActor, boHAnduin05.m_standardEmoteResponseLine);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHAnduin05.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Anduin_05 boHAnduin05 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHAnduin05.\u003C\u003En__1(entity);
    while (boHAnduin05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHAnduin05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHAnduin05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHAnduin05.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Anduin_05 boHAnduin05 = this;
    while (boHAnduin05.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHAnduin05.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHAnduin05.\u003C\u003En__2(entity);
      yield return (object) boHAnduin05.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHAnduin05.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Anduin_05 boHAnduin05 = this;
    while (boHAnduin05.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHAnduin05.MissionPlayVO(enemyActor, (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeA_01);
        yield return (object) boHAnduin05.MissionPlayVO(friendlyActor, (string) BoH_Anduin_05.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeA_02);
        break;
      case 5:
        yield return (object) boHAnduin05.MissionPlayVO(friendlyActor, (string) BoH_Anduin_05.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeB_01);
        yield return (object) boHAnduin05.MissionPlayVO(enemyActor, (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeB_03);
        break;
      case 9:
        yield return (object) boHAnduin05.MissionPlayVO(enemyActor, (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeC_01);
        yield return (object) boHAnduin05.MissionPlayVO(friendlyActor, (string) BoH_Anduin_05.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission5ExchangeC_02);
        yield return (object) boHAnduin05.MissionPlayVO(enemyActor, (string) BoH_Anduin_05.VO_Story_Hero_Nazgrim_Male_Orc_Story_Anduin_Mission5ExchangeC_03);
        break;
    }
  }
}
