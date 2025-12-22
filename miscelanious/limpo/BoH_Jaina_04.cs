using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Jaina_04 : BoH_Jaina_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Jaina_04.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4EmoteResponse_01 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4EmoteResponse_01.prefab:15c60d17bdabd674d80309293e5713eb");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4ExchangeA_01 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4ExchangeA_01.prefab:1117f7cfe3651354782f6f6789a18ac1");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4ExchangeB_01 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4ExchangeB_01.prefab:c726cf6310252d24d959f73837757809");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_01 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_01.prefab:c547253ef12be0d499954ede128b2a4d");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_02 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_02.prefab:5529c0a384efb944d94a85ee313d5e37");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_03 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_03.prefab:3c404f92997e9ee46afc39972f4c352e");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_01 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_01.prefab:2c71bf5eef1fbbb44907f8fca0d69803");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_02 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_02.prefab:1ef4a3214969abd408fbb322ba6d00c5");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_03 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_03.prefab:71919c7c8ffc2574091924c45848add9");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Intro_01 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Intro_01.prefab:a1e18bbde14d60948883d830ddfb059d");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Intro_02 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Intro_02.prefab:e96c65e01c544764daaba8d5fa283a02");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Loss_01 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Loss_01.prefab:63a2314eaaf97df47b6cb50c8987e5d4");
  private static readonly AssetReference VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Victory_01.prefab:64f3a2b31d85c0b4b8e3a83fa4d7f809");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeA_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeA_01.prefab:0a5317a4515e4aa4ab34a65ec796264d");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeB_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeB_01.prefab:6e536a6e1d138a149ab257e379ff2030");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeC_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeC_01.prefab:61038aed51fb36c4cacdf8b1b7efe8bd");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4Intro_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4Intro_01.prefab:1ed5388a7190ad549aa3fab63b2ab0db");
  private static readonly AssetReference VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4Victory_01.prefab:6d44386f1f852024dac446257a39a6a0");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ExchangeC_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ExchangeC_01.prefab:0f5556d64e0743439c18c5c4d05d2b7e");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_01.prefab:48dffc5ef0bb4118a1425d585e3f5063");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_02 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_02.prefab:7c5eb9a511364f8da1a77958b764374b");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_03 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_03.prefab:83503a35178640b7a6e304760c80e413");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4Victory_01.prefab:6c989bfe86c64571a158f5ecb18073db");
  public static readonly AssetReference ThrallBrassRing = new AssetReference("Thrall_BrassRing_Quote.prefab:962e58c9390b0f842a8b64d0d44cf7b4");
  private List<string> m_VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPowerLines = new List<string>()
  {
    (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_01,
    (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_02,
    (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_03
  };
  private List<string> m_VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4IdleLines = new List<string>()
  {
    (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_01,
    (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_02,
    (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Jaina_04() => this.m_gameOptions.AddBooleanOptions(BoH_Jaina_04.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4EmoteResponse_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4ExchangeA_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4ExchangeB_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_02,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPower_03,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_02,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Idle_03,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Intro_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Intro_02,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Loss_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Victory_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeA_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeB_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeC_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4Intro_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4Victory_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ExchangeC_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_01,
      (string) BoH_Jaina_04.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_02,
      (string) BoH_Jaina_04.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_03,
      (string) BoH_Jaina_04.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Jaina_04 boHJaina04 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHJaina04.PlayLineAlways(enemyActor, (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Intro_01);
    yield return (object) boHJaina04.PlayLineAlways(friendlyActor, (string) BoH_Jaina_04.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4Intro_01);
    yield return (object) boHJaina04.PlayLineAlways(enemyActor, (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetIdleLines() => this.m_VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4IdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4HeroPowerLines;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Intro_02, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech((string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4EmoteResponse_01, Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Jaina_04 boHJaina04 = this;
    while (boHJaina04.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) boHJaina04.PlayLineAlways(actor, (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Victory_01);
        yield return (object) boHJaina04.PlayLineAlways((string) BoH_Jaina_04.ThrallBrassRing, (string) BoH_Jaina_04.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4Victory_01);
        yield return (object) boHJaina04.PlayLineAlways(friendlyActor, (string) BoH_Jaina_04.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 502:
        GameState.Get().SetBusy(true);
        yield return (object) boHJaina04.PlayLineAlways((string) BoH_Jaina_04.ThrallBrassRing, (string) BoH_Jaina_04.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ExchangeC_01);
        yield return (object) boHJaina04.PlayLineAlways(friendlyActor, (string) BoH_Jaina_04.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeC_01);
        GameState.Get().SetBusy(false);
        break;
      case 503:
        yield return (object) boHJaina04.PlayLineAlways((string) BoH_Jaina_04.ThrallBrassRing, (string) BoH_Jaina_04.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_01);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHJaina04.PlayLineAlways(actor, (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4Loss_01);
        GameState.Get().SetBusy(false);
        break;
      case 505:
        yield return (object) boHJaina04.PlayLineAlways((string) BoH_Jaina_04.ThrallBrassRing, (string) BoH_Jaina_04.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_02);
        break;
      case 506:
        yield return (object) boHJaina04.PlayLineAlways((string) BoH_Jaina_04.ThrallBrassRing, (string) BoH_Jaina_04.VO_Story_Hero_Thrall_Male_Orc_Story_Jaina_Mission4ThrallHelp_03);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHJaina04.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Jaina_04 boHJaina04 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHJaina04.\u003C\u003En__1(entity);
    while (boHJaina04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHJaina04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHJaina04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHJaina04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Jaina_04 boHJaina04 = this;
    while (boHJaina04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHJaina04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHJaina04.\u003C\u003En__2(entity);
      yield return (object) boHJaina04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHJaina04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Jaina_04 boHJaina04 = this;
    while (boHJaina04.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) boHJaina04.PlayLineAlways(actor, (string) BoH_Jaina_04.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeA_01);
        yield return (object) boHJaina04.PlayLineAlways(enemyActor, (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4ExchangeA_01);
        break;
      case 5:
        yield return (object) boHJaina04.PlayLineAlways(actor, (string) BoH_Jaina_04.VO_Story_Hero_Jaina_Human_Female_Story_Jaina_Mission4ExchangeB_01);
        yield return (object) boHJaina04.PlayLineAlways(enemyActor, (string) BoH_Jaina_04.VO_Story_Hero_Grommash_Male_Orc_Story_Jaina_Mission4ExchangeB_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BRMAdventure);
}
