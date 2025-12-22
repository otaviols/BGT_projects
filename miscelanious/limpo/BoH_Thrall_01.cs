using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Thrall_01 : BoH_Thrall_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Thrall_01.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1EmoteResponse_01 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1EmoteResponse_01.prefab:6b15ee78bdb7d3e4eb817730292f67a0");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1ExchangeA_01 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1ExchangeA_01.prefab:94a18e1a15b525644ae701a8933e9870");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1ExchangeB_01 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1ExchangeB_01.prefab:ccf658130c95a974a819427341721210");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_01 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_01.prefab:48facd72d9af5094a8a0ad2fcbcb5e27");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_02 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_02.prefab:89e0881c8a41774478ac837d2f916209");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_03 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_03.prefab:ae94739cf04110447ba183a3247bb34c");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_01 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_01.prefab:56d7656451fd55048be2545881834b29");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_02 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_02.prefab:5989e805d24284d419afb8f9d2dd1268");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_03 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_03.prefab:409c200008c1eff44843cb59e1361a8b");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Intro_01 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Intro_01.prefab:2edf498dc0494924e96950f57b527726");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Intro_02 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Intro_02.prefab:5a74c71b012609441b6cc49eb2385767");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Loss_01 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Loss_01.prefab:6cb3cb427aab3474e908bcc32171c183");
  private static readonly AssetReference VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Victory_01 = new AssetReference("VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Victory_01.prefab:1d0196bd7fb4a6d46a2ea6bd46eb6c0b");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1ExchangeC_02 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1ExchangeC_02.prefab:56ed419838689ba4b99553bf6261ac6e");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Intro_02 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Intro_02.prefab:a6004d0d5e0d2d5488a743006bf3c9a3");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Victory_02 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Victory_02.prefab:c0ae5c6e8818e4c4a8e4c3bf7e3f0b05");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Victory_04 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Victory_04.prefab:4ffbeba38c60d11449fa8fe528dbc1cf");
  private static readonly AssetReference VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1ExchangeC_01 = new AssetReference("VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1ExchangeC_01.prefab:31402b425176411196f5aea1ff637fee");
  private static readonly AssetReference VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1ExchangeC_03 = new AssetReference("VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1ExchangeC_03.prefab:0e8b98d41d4045429f5a8abf0726cfa4");
  private static readonly AssetReference VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1Victory_03 = new AssetReference("VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1Victory_03.prefab:f8d7924a89ce2c94a92c3e3da1d90b11");
  private static readonly AssetReference VO_Story_Minion_Taretha_Female_Human_TriggerThrallLowHP_01 = new AssetReference("VO_Story_Minion_Taretha_Female_Human_TriggerThrallLowHP_01.prefab:90d6a3dd52cb4406986eac02dcdcba5c");
  private static readonly AssetReference troll_crowd_play_reaction_positive_1 = new AssetReference("troll_crowd_play_reaction_positive_1.prefab:ccb1b6d185b1e2e4480ef813153f3c9f");
  private static readonly AssetReference troll_crowd_play_reaction_very_positive_1 = new AssetReference("troll_crowd_play_reaction_very_positive_1.prefab:f69658ac1e4cacc4b94acdb1e0c38911");
  private static readonly AssetReference Low_Drumroll = new AssetReference("Low_Drumroll.prefab:d678997d507dd9041a499af987d4ff76");
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      228,
      new string[1]{ "BOH_THRALL_01" }
    },
    {
      328,
      new string[1]{ "BOH_THRALL_01a" }
    },
    {
      428,
      new string[1]{ "BOH_THRALL_01b" }
    }
  };
  private float popUpScale = 1.25f;
  private Vector3 popUpPos;
  public static readonly AssetReference TarethaBrassRing = new AssetReference("Taretha_BrassRing_Quote.prefab:683cb9ffa15e9af4cbbe387d4afe900d");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_01,
    (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_02,
    (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_01,
    (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_02,
    (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Thrall_01() => this.m_gameOptions.AddBooleanOptions(BoH_Thrall_01.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1EmoteResponse_01,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1ExchangeA_01,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1ExchangeB_01,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_01,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_02,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1HeroPower_03,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_01,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_02,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Idle_03,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Intro_01,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Intro_02,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Loss_01,
      (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Victory_01,
      (string) BoH_Thrall_01.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1ExchangeC_02,
      (string) BoH_Thrall_01.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Intro_02,
      (string) BoH_Thrall_01.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Victory_02,
      (string) BoH_Thrall_01.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Victory_04,
      (string) BoH_Thrall_01.VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1ExchangeC_01,
      (string) BoH_Thrall_01.VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1ExchangeC_03,
      (string) BoH_Thrall_01.VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1Victory_03,
      (string) BoH_Thrall_01.VO_Story_Minion_Taretha_Female_Human_TriggerThrallLowHP_01,
      (string) BoH_Thrall_01.troll_crowd_play_reaction_positive_1,
      (string) BoH_Thrall_01.troll_crowd_play_reaction_very_positive_1,
      (string) BoH_Thrall_01.Low_Drumroll
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Thrall_01 boHThrall01 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHThrall01.MissionPlayVO(enemyActor, (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Intro_01);
    yield return (object) boHThrall01.MissionPlayVO(enemyActor, (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Intro_02);
    yield return (object) boHThrall01.MissionPlayVO(friendlyActor, (string) BoH_Thrall_01.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_GILFinalBoss;
    this.m_standardEmoteResponseLine = (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1EmoteResponse_01;
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
    BoH_Thrall_01 boHThrall01 = this;
    while (boHThrall01.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    boHThrall01.popUpPos = new Vector3(0.0f, 0.0f, -40f);
    switch (missionEvent)
    {
      case 101:
        yield return (object) boHThrall01.PlayLineAlways((string) BoH_Thrall_01.TarethaBrassRing, (string) BoH_Thrall_01.VO_Story_Minion_Taretha_Female_Human_TriggerThrallLowHP_01);
        break;
      case 328:
        yield return (object) new WaitForSeconds(2f);
        yield return (object) boHThrall01.MissionPlaySound(enemyActor, (string) BoH_Thrall_01.Low_Drumroll);
        Notification popupText1 = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, boHThrall01.popUpPos, TutorialEntity.GetTextScale() * boHThrall01.popUpScale, GameStrings.Get(boHThrall01.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
        NotificationManager.Get().DestroyNotification(popupText1, 3.5f);
        break;
      case 428:
        yield return (object) new WaitForSeconds(2f);
        yield return (object) boHThrall01.MissionPlaySound(enemyActor, (string) BoH_Thrall_01.Low_Drumroll);
        Notification popupText2 = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, boHThrall01.popUpPos, TutorialEntity.GetTextScale() * boHThrall01.popUpScale, GameStrings.Get(boHThrall01.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
        NotificationManager.Get().DestroyNotification(popupText2, 3.5f);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall01.PlayLineAlways(enemyActor, (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Victory_01);
        yield return (object) boHThrall01.PlayLineAlways(friendlyActor, (string) BoH_Thrall_01.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Victory_02);
        yield return (object) boHThrall01.PlayLineAlways((string) BoH_Thrall_01.TarethaBrassRing, (string) BoH_Thrall_01.VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1Victory_03);
        yield return (object) boHThrall01.PlayLineAlways(friendlyActor, (string) BoH_Thrall_01.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1Victory_04);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall01.PlayLineAlways(enemyActor, (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHThrall01.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Thrall_01 boHThrall01 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHThrall01.\u003C\u003En__1(entity);
    while (boHThrall01.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHThrall01.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHThrall01.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHThrall01.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Thrall_01 boHThrall01 = this;
    while (boHThrall01.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHThrall01.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHThrall01.\u003C\u003En__2(entity);
      yield return (object) boHThrall01.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHThrall01.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Thrall_01 boHThrall01 = this;
    while (boHThrall01.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    int m_missionEventBannerID = 228;
    switch (turn)
    {
      case 1:
        yield return (object) boHThrall01.MissionPlaySound(enemyActor, (string) BoH_Thrall_01.Low_Drumroll);
        Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, boHThrall01.popUpPos, TutorialEntity.GetTextScale() * boHThrall01.popUpScale, GameStrings.Get(boHThrall01.m_popUpInfo[m_missionEventBannerID][0]), false, NotificationManager.PopupTextType.FANCY);
        NotificationManager.Get().DestroyNotification(popupText, 6.5f);
        yield return (object) boHThrall01.PlayLineAlways(enemyActor, (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1ExchangeA_01);
        yield return (object) boHThrall01.MissionPlaySound(enemyActor, (string) BoH_Thrall_01.troll_crowd_play_reaction_positive_1);
        break;
      case 5:
        yield return (object) boHThrall01.PlayLineAlways(enemyActor, (string) BoH_Thrall_01.VO_Story_Hero_Blackmoore_Male_Human_Story_Thrall_Mission1ExchangeB_01);
        yield return (object) boHThrall01.MissionPlaySound(enemyActor, (string) BoH_Thrall_01.troll_crowd_play_reaction_very_positive_1);
        break;
      case 9:
        yield return (object) boHThrall01.PlayLineAlways((string) BoH_Thrall_01.TarethaBrassRing, (string) BoH_Thrall_01.VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1ExchangeC_01);
        yield return (object) boHThrall01.PlayLineAlways(friendlyActor, (string) BoH_Thrall_01.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission1ExchangeC_02);
        yield return (object) boHThrall01.PlayLineAlways((string) BoH_Thrall_01.TarethaBrassRing, (string) BoH_Thrall_01.VO_Story_Minion_Taretha_Female_Human_Story_Thrall_Mission1ExchangeC_03);
        break;
    }
  }
}
