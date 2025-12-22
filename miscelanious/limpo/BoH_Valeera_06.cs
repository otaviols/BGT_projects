using System.Collections;
using System.Collections.Generic;

public class BoH_Valeera_06 : BoH_Valeera_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Death_01 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Death_01.prefab:d4b75918e6afaf34b840b2a558b1a7b1");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6EmoteResponse_01 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6EmoteResponse_01.prefab:0110c821ef498d24da9e153e71af0729");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeA_02 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeA_02.prefab:096d086a4fcbfd04dbceaef4a96ffbec");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeB_01 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeB_01.prefab:f16c8c3351cf26a4f9ab0443b2a0d0af");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeC_01 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeC_01.prefab:59b79fe7aa6881d4c88679fda7e23395");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_01 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_01.prefab:5dead36f717db5c4d99dbb049c83a219");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_02 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_02.prefab:8489c253666cfb94a9aa3690463761bf");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_03 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_03.prefab:0098145bff4bfdf439cab04fd5d7c4b4");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_01 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_01.prefab:42f49c3ee98210e42baa03ca50461338");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_02 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_02.prefab:eacecb3711cb11545b0564366eb24809");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_03 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_03.prefab:7c3ccab02fe20ed41ba881e7cc602f36");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Intro_02 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Intro_02.prefab:8f3539847cc3dff40a3926096d23a718");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Loss_01 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Loss_01.prefab:55d721e9d7ec7f44c89516f9b9cd5c6a");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_01 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_01.prefab:e40326e699342794ab6942a04da88982");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_04 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_04.prefab:0a36d228fd5802f449b214dd3db44b00");
  private static readonly AssetReference VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_05 = new AssetReference("VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_05.prefab:00e3a98fd67793e428abf80d2c813316");
  private static readonly AssetReference VO_Story_Hero_Jaina_Female_Human_Story_Valeera_Mission6Victory_03 = new AssetReference("VO_Story_Hero_Jaina_Female_Human_Story_Valeera_Mission6Victory_03.prefab:0600016db0bd5fe4eabedabdb22c8c7e");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6ExchangeA_01 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6ExchangeA_01.prefab:bac080dc857fe574fb22a771e025d06c");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6ExchangeC_02 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6ExchangeC_02.prefab:7b994f652fd682d4d9c2c2bb33ad18fa");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6Intro_01 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6Intro_01.prefab:ffcedc853085c5b4990c562e1b82a05e");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6Victory_02 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6Victory_02.prefab:4d3b51d3cf1765d44a82ec0bfdc74f02");
  public static readonly AssetReference JainaBrassRing = new AssetReference("JainaMid_BrassRing_Quote.prefab:7eba171d881f6764e81abddbb125bb19");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_01,
    (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_02,
    (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_01,
    (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_02,
    (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Death_01,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6EmoteResponse_01,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeA_02,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeB_01,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeC_01,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_01,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_02,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6HeroPower_03,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_01,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_02,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Idle_03,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Intro_02,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Loss_01,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_01,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_04,
      (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_05,
      (string) BoH_Valeera_06.VO_Story_Hero_Jaina_Female_Human_Story_Valeera_Mission6Victory_03,
      (string) BoH_Valeera_06.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6ExchangeA_01,
      (string) BoH_Valeera_06.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6ExchangeC_02,
      (string) BoH_Valeera_06.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6Intro_01,
      (string) BoH_Valeera_06.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6Victory_02
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Valeera_06 boHValeera06 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHValeera06.MissionPlayVO(actor, (string) BoH_Valeera_06.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6Intro_01);
    yield return (object) boHValeera06.MissionPlayVO(enemyActor, (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Death_01;
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_BRMAdventure;
    this.m_standardEmoteResponseLine = (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6EmoteResponse_01;
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
    BoH_Valeera_06 boHValeera06 = this;
    while (boHValeera06.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 102:
        GameState.Get().SetBusy(true);
        yield return (object) boHValeera06.PlayLineAlways(enemyActor, (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_01);
        yield return (object) boHValeera06.PlayLineAlways(friendlyActor, (string) BoH_Valeera_06.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6Victory_02);
        yield return (object) boHValeera06.PlayLineAlways((string) BoH_Valeera_06.JainaBrassRing, (string) BoH_Valeera_06.VO_Story_Hero_Jaina_Female_Human_Story_Valeera_Mission6Victory_03);
        yield return (object) boHValeera06.PlayLineAlways(enemyActor, (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_04);
        GameState.Get().SetBusy(false);
        break;
      case 103:
        GameState.Get().SetBusy(true);
        yield return (object) boHValeera06.PlayLineAlways(enemyActor, (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Victory_05);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        GameState.Get().SetBusy(true);
        yield return (object) boHValeera06.PlayLineAlways(enemyActor, (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHValeera06.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Valeera_06 boHValeera06 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHValeera06.\u003C\u003En__1(entity);
    while (boHValeera06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHValeera06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHValeera06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHValeera06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Valeera_06 boHValeera06 = this;
    while (boHValeera06.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHValeera06.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHValeera06.\u003C\u003En__2(entity);
      yield return (object) boHValeera06.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHValeera06.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Valeera_06 boHValeera06 = this;
    while (boHValeera06.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHValeera06.PlayLineAlways(friendlyActor, (string) BoH_Valeera_06.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6ExchangeA_01);
        yield return (object) boHValeera06.PlayLineAlways(enemyActor, (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeA_02);
        break;
      case 5:
        yield return (object) boHValeera06.PlayLineAlways(enemyActor, (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeB_01);
        break;
      case 11:
        yield return (object) boHValeera06.PlayLineAlways(enemyActor, (string) BoH_Valeera_06.VO_Story_Hero_Garona_Female_Orc_Story_Valeera_Mission6ExchangeC_01);
        yield return (object) boHValeera06.PlayLineAlways(friendlyActor, (string) BoH_Valeera_06.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission6ExchangeC_02);
        break;
    }
  }
}
