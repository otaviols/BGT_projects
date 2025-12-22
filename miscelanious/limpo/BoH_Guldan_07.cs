using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Guldan_07 : BoH_Guldan_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Guldan_07.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeA_01 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeA_01.prefab:3ddc791d0cfb3ff4085da803f07022e1");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeB_01 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeB_01.prefab:6026cc3c3478b8f47ab68cc1aa8f8c5e");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeC_01 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeC_01.prefab:ccff8061c84acf24d8a82467b0ef3279");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeD_01 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeD_01.prefab:4ee37e8b88e1b4e4f96772be0899194c");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeE_01 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeE_01.prefab:83686c9e29fb81c4e861d7f70c8d867c");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7Intro_01 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7Intro_01.prefab:e306625e97472c449b69aa1cfc904bd9");
  private static readonly AssetReference VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7Victory_02 = new AssetReference("VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7Victory_02.prefab:58f71880e5f75a24f9d0b095c4d25662");
  private static readonly AssetReference VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7EmoteResponse_01 = new AssetReference("VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7EmoteResponse_01.prefab:2ffeea2e12fbfa1418f6b891a6f0f4d6");
  private static readonly AssetReference VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7ExchangeE_02 = new AssetReference("VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7ExchangeE_02.prefab:52f2eb141c0f46740ae15b9c41f4d1b5");
  private static readonly AssetReference VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_01 = new AssetReference("VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_01.prefab:a7709b940c8d2eb46a0754bb9fff1990");
  private static readonly AssetReference VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_02 = new AssetReference("VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_02.prefab:46bcabce91aaeb14e8b227b86487b2cc");
  private static readonly AssetReference VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_03 = new AssetReference("VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_03.prefab:74418e8737f3cc243b592fcbe3100a55");
  private static readonly AssetReference VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Loss_01 = new AssetReference("VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Loss_01.prefab:cab03f4c04c2d6448951105827751b6e");
  private static readonly AssetReference VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Victory_03 = new AssetReference("VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Victory_03.prefab:03fa3fd7de54cb244b8f478fe4c52f68");
  private static readonly AssetReference VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Victory_05 = new AssetReference("VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Victory_05.prefab:c90ba1ea9d12b2345b000eed4f892aaf");
  private static readonly AssetReference VO_Story_Minion_Necrolyte_Male_Orc_Story_Guldan_Mission7Intro_02 = new AssetReference("VO_Story_Minion_Necrolyte_Male_Orc_Story_Guldan_Mission7Intro_02.prefab:defd8bb0ef43486a95c3007cf3047146");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_01 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_01.prefab:dab9f01ad0f6b04478221940ae97f1f8");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_04 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_04.prefab:445d7373aa583a74c8161dfb44b3c14b");
  private static readonly AssetReference VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_06 = new AssetReference("VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_06.prefab:dffb03a8df088294cacb092fe803dddc");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_01,
    (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_02,
    (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_03
  };
  private List<string> m_EmoteResponseLines = new List<string>()
  {
    (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7EmoteResponse_01
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Guldan_07() => this.m_gameOptions.AddBooleanOptions(BoH_Guldan_07.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeA_01,
      (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeB_01,
      (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeC_01,
      (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeD_01,
      (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeE_01,
      (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7Intro_01,
      (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7Victory_02,
      (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7EmoteResponse_01,
      (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7ExchangeE_02,
      (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_01,
      (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_02,
      (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7HeroPower_03,
      (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Loss_01,
      (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Victory_03,
      (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Victory_05,
      (string) BoH_Guldan_07.VO_Story_Minion_Necrolyte_Male_Orc_Story_Guldan_Mission7Intro_02,
      (string) BoH_Guldan_07.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_01,
      (string) BoH_Guldan_07.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_04,
      (string) BoH_Guldan_07.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_06
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Guldan_07 boHGuldan07 = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      GameState.Get().SetBusy(false);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E2__current = (object) boHGuldan07.MissionPlayVO(actor, (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7Intro_01);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

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
    this.m_Mission_EnemyPlayIdleLines = false;
    this.m_standardEmoteResponseLine = (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7EmoteResponse_01;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Guldan_07 boHGuldan07 = this;
    while (boHGuldan07.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        yield return (object) boHGuldan07.MissionPlayVO(friendlyActor, (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeB_01);
        break;
      case 102:
        yield return (object) boHGuldan07.MissionPlayVO(friendlyActor, (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeC_01);
        break;
      case 103:
        yield return (object) boHGuldan07.MissionPlayVOOnce(friendlyActor, (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeD_01);
        break;
      case 104:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan07.MissionPlayVO(friendlyActor, (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeE_01);
        GameState.Get().SetBusy(false);
        break;
      case 105:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan07.MissionPlayVO(enemyActor, (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7ExchangeE_02);
        GameState.Get().SetBusy(false);
        break;
      case 106:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan07.MissionPlayVO(friendlyActor, (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7Intro_01);
        yield return (object) boHGuldan07.MissionPlayVO(boHGuldan07.GetFriendlyActorByCardId("Story_09_CouncilNecrolyte"), (string) BoH_Guldan_07.VO_Story_Minion_Necrolyte_Male_Orc_Story_Guldan_Mission7Intro_02);
        GameState.Get().SetBusy(false);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan07.MissionPlayVO(boHGuldan07.GetEnemyActorByCardId("Story_09_OrgrimMinion2"), (string) BoH_Guldan_07.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_01);
        yield return (object) boHGuldan07.MissionPlayVO(friendlyActor, (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7Victory_02);
        yield return (object) boHGuldan07.MissionPlayVO(enemyActor, (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Victory_03);
        yield return (object) boHGuldan07.MissionPlayVO(boHGuldan07.GetEnemyActorByCardId("Story_09_OrgrimMinion2"), (string) BoH_Guldan_07.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_04);
        yield return (object) boHGuldan07.MissionPlayVO(enemyActor, (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Victory_05);
        yield return (object) boHGuldan07.MissionPlayVO(boHGuldan07.GetEnemyActorByCardId("Story_09_OrgrimMinion2"), (string) BoH_Guldan_07.VO_Story_Minion_Orgrim_Male_Orc_Story_Guldan_Mission7Victory_06);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHGuldan07.MissionPlayVO(enemyActor, (string) BoH_Guldan_07.VO_Story_Hero_TeronGorefiend_Male_Undead_Story_Guldan_Mission7Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 520:
        yield return (object) boHGuldan07.MissionPlayVO(friendlyActor, (string) BoH_Guldan_07.VO_Story_Hero_Guldan_Male_Orc_Story_Guldan_Mission7ExchangeA_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHGuldan07.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Guldan_07 boHGuldan07 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHGuldan07.\u003C\u003En__1(entity);
    while (boHGuldan07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGuldan07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHGuldan07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGuldan07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Guldan_07 boHGuldan07 = this;
    while (boHGuldan07.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGuldan07.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHGuldan07.\u003C\u003En__2(entity);
      yield return (object) boHGuldan07.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGuldan07.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Guldan_07 boHGuldan07 = this;
    while (boHGuldan07.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
  }
}
