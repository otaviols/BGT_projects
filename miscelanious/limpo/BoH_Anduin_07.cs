using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Anduin_07 : BoH_Anduin_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Anduin_07.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeB_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeB_01.prefab:d845d3bfb2f6fe04a9bb3741f0b5b713");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_01.prefab:2164189090f6aa94fb75474f122ad6ed");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_02 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_02.prefab:b78ac66284a921a4999d7532fd3bf8f8");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Intro_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Intro_01.prefab:345fe423e3c1cc748b05493f17fef94d");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission7Victory_01 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission7Victory_01.prefab:296ae0a731f235a46b149eb9b1924e9e");
  private static readonly AssetReference VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission7Victory_03 = new AssetReference("VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission7Victory_03.prefab:a34a3051225d65549a3ceced41813c2b");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7EmoteResponse_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7EmoteResponse_01.prefab:6283d04613c321442a92ae360aa6fcdc");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_01.prefab:ee51597d2de69e342bf4b28f7564a22d");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_02.prefab:854c94bdc042c6e4b9939f34f9172bb0");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeB_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeB_01.prefab:8a127d6e4493e7e4cb950dac500e94cf");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_01.prefab:b64cf8e781652a44aaf370799ed10a85");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_02.prefab:f1bc0f0d15f00fe4998bafd5945917cc");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_03.prefab:f8290f36cfb34e5458fd016b8b050a7f");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_01.prefab:0641e7c422bb3e140a3d4d471c760e49");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_02.prefab:fe4962465803a8246b989c0bd3a9290a");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_03.prefab:9c9223d9342edf34587fe9b899fbf297");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_01.prefab:a0fc887b9b603a74bb2787143873a5b3");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_02.prefab:028c060e4f253554c81ff691832b723b");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_03.prefab:041d9c3886db4794aa7428b5c47e7eb7");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Intro_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Intro_01.prefab:8b58e873ffa31e44a9ac79ccd47e8060");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Loss_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Loss_01.prefab:b56aa1f374b497a4aa31dd654030e220");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Victory_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Victory_01.prefab:0b235d11449ec4c44bfce28ec1506a66");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_01,
    (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_02,
    (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_01,
    (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_02,
    (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_03
  };
  private List<string> m_missionEventTrigger502Lines = new List<string>()
  {
    (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_01,
    (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_02,
    (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Anduin_07() => this.m_gameOptions.AddBooleanOptions(BoH_Anduin_07.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeB_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_02,
      (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Intro_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission7Victory_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission7Victory_03,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7EmoteResponse_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_02,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeB_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_02,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ShaCallout_03,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_02,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7HeroPower_03,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_02,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Idle_03,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Intro_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Loss_01,
      (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Anduin_07 boHAnduin07 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHAnduin07.MissionPlayVO(actor, (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Intro_01);
    yield return (object) boHAnduin07.MissionPlayVO(friendlyActor, (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMulliganMusicTrack = MusicPlaylistType.InGame_BT;
    this.m_standardEmoteResponseLine = (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7EmoteResponse_01;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Anduin_07 boHAnduin07 = this;
    while (boHAnduin07.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        GameState.Get().SetBusy(true);
        yield return (object) boHAnduin07.MissionPlayVO(actor, (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 103:
        GameState.Get().SetBusy(true);
        yield return (object) boHAnduin07.MissionPlayVO(friendlyActor, (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission7Victory_01);
        yield return (object) boHAnduin07.MissionPlayVO(friendlyActor, (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Anduin_Mission7Victory_03);
        GameState.Get().SetBusy(false);
        break;
      case 112:
        yield return (object) boHAnduin07.MissionPlayVOOnce(actor, boHAnduin07.m_missionEventTrigger502Lines);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHAnduin07.MissionPlayVO(actor, (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Anduin_Mission7Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 515:
        yield return (object) boHAnduin07.MissionPlayVO(actor, boHAnduin07.m_standardEmoteResponseLine);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHAnduin07.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Anduin_07 boHAnduin07 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHAnduin07.\u003C\u003En__1(entity);
    while (boHAnduin07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHAnduin07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHAnduin07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHAnduin07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Anduin_07 boHAnduin07 = this;
    while (boHAnduin07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHAnduin07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHAnduin07.\u003C\u003En__2(entity);
      yield return (object) boHAnduin07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHAnduin07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Anduin_07 boHAnduin07 = this;
    while (boHAnduin07.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHAnduin07.MissionPlayVO(enemyActor, (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_01);
        yield return (object) boHAnduin07.MissionPlayVO(enemyActor, (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeA_02);
        break;
      case 5:
        yield return (object) boHAnduin07.MissionPlayVO(friendlyActor, (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeB_01);
        yield return (object) boHAnduin07.MissionPlayVO(enemyActor, (string) BoH_Anduin_07.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission7ExchangeB_01);
        break;
      case 9:
        yield return (object) boHAnduin07.MissionPlayVO(friendlyActor, (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_01);
        yield return (object) boHAnduin07.MissionPlayVO(friendlyActor, (string) BoH_Anduin_07.VO_Story_Hero_Anduin_Male_Human_Story_Garrosh_Mission7ExchangeD_02);
        break;
    }
  }
}
