using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Thrall_08 : BoH_Thrall_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Thrall_08.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeA_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeA_01.prefab:40541853068677f4d9745730730f7ea9");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_01.prefab:8b0b14fe3d5fbc243a062e69b3a9dd6b");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_02.prefab:8f7d986a3a04d44458d85e98e1741f6f");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeC_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeC_01.prefab:c24c2e172acd459498063de42600d818");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8Intro_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8Intro_01.prefab:f34b97fab2460a14aadda239cc9b8e23");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8EmoteResponse_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8EmoteResponse_01.prefab:342fc74826800814f89ce85462874cc0");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_01.prefab:2059183819207d647b3ab0d5f50627b1");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_02.prefab:90f5d2ea469112b4ab4ca8846806080e");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_03.prefab:4c2c3ca32dc6fc04184e4ff9837c9762");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_01.prefab:06f5900c5e838aa4b9450a278584b463");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_02 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_02.prefab:4920648bd5eaddb4e9c0d1db5c35b76e");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_03 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_03.prefab:cc43852e58716974bad83da4b9c0429a");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Loss_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Loss_01.prefab:86f2bf5150ae3764784ffa667d8f5142");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeA_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeA_01.prefab:e47985c6861b45f418cd59b0b2a6831f");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeC_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeC_01.prefab:22e9cef7c11e39c4396d9cf3da90489b");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Intro_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Intro_01.prefab:9ae50e3a6180b9b4facad8b833b66f4e");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission8Victory_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission8Victory_01.prefab:e239773a72bed4141a9a5e47dc977cc4");
  private Dictionary<int, string[]> m_popUpInfo = new Dictionary<int, string[]>()
  {
    {
      228,
      new string[1]{ "BOH_THRALL_08" }
    }
  };
  private float popUpScale = 1.25f;
  private Vector3 popUpPos;
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_01,
    (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_02,
    (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_01,
    (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_02,
    (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Thrall_08() => this.m_gameOptions.AddBooleanOptions(BoH_Thrall_08.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeA_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_02,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeC_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8Intro_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8EmoteResponse_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_02,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8HeroPower_03,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_02,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Idle_03,
      (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Loss_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeA_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeC_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Intro_01,
      (string) BoH_Thrall_08.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission8Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Thrall_08 boHThrall08 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHThrall08.MissionPlayVO(actor, (string) BoH_Thrall_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8Intro_01);
    yield return (object) boHThrall08.MissionPlayVO(enemyActor, (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_BT;
    this.m_standardEmoteResponseLine = (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8EmoteResponse_01;
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
    BoH_Thrall_08 boHThrall08 = this;
    while (boHThrall08.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 102:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall08.PlayLineAlways(actor, (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeC_01);
        yield return (object) boHThrall08.PlayLineAlways(friendlyActor, (string) BoH_Thrall_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeC_01);
        GameState.Get().SetBusy(false);
        break;
      case 228:
        yield return (object) new WaitForSeconds(2f);
        Notification popupText = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, boHThrall08.popUpPos, TutorialEntity.GetTextScale() * boHThrall08.popUpScale, GameStrings.Get(boHThrall08.m_popUpInfo[missionEvent][0]), false, NotificationManager.PopupTextType.FANCY);
        NotificationManager.Get().DestroyNotification(popupText, 5.5f);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall08.PlayLineAlways(friendlyActor, (string) BoH_Thrall_08.VO_Story_Hero_Thrall_Male_Orc_Story_Thrall_Mission8Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHThrall08.PlayLineAlways(actor, (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Thrall_Mission8Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHThrall08.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Thrall_08 boHThrall08 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHThrall08.\u003C\u003En__1(entity);
    while (boHThrall08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHThrall08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHThrall08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHThrall08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Thrall_08 boHThrall08 = this;
    while (boHThrall08.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHThrall08.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHThrall08.\u003C\u003En__2(entity);
      yield return (object) boHThrall08.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHThrall08.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Thrall_08 boHThrall08 = this;
    while (boHThrall08.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) boHThrall08.PlayLineAlways(actor, (string) BoH_Thrall_08.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission8ExchangeA_01);
        yield return (object) boHThrall08.PlayLineAlways(enemyActor, (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeA_01);
        break;
      case 7:
        yield return (object) boHThrall08.PlayLineAlways(enemyActor, (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_01);
        yield return (object) boHThrall08.PlayLineAlways(enemyActor, (string) BoH_Thrall_08.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission8ExchangeB_02);
        break;
    }
  }
}
