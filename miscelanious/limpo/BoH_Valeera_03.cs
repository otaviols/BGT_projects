using System.Collections;
using System.Collections.Generic;

public class BoH_Valeera_03 : BoH_Valeera_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Death_01 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Death_01.prefab:3d5843774ea2fb74e8f623388afac3e8");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3EmoteResponse_01 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3EmoteResponse_01.prefab:e87df59d72560db43a0c805737acb8ab");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3ExchangeA_01 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3ExchangeA_01.prefab:0452236aef80d1e42ab8f513d0545ed8");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3ExchangeB_02 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3ExchangeB_02.prefab:7cb86c78a265acf498df7d586d467290");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_01 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_01.prefab:b21d8f5ca30482640bed3ff0eaf6752e");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_02 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_02.prefab:69960b8020fec1b468b0b9b2a2a791c0");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_03 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_03.prefab:8f869559b4857d14da4d90dd3b7f66cc");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_01 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_01.prefab:5d64b37a2c454c94090e725f40fd1302");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_02 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_02.prefab:e87e4b12750dcc8408eb644c5f263740");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_03 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_03.prefab:d1eccb2f550e9534383bc8fcdc98ff5e");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Intro_01 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Intro_01.prefab:a63c2206f873c124fb2cd326cdf9f859");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Loss_01 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Loss_01.prefab:fde076baa6590b041a0b1fb57d11900e");
  private static readonly AssetReference VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Victory_02 = new AssetReference("VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Victory_02.prefab:b73a336d8bf1bfe488144dc3d2343418");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeA_02 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeA_02.prefab:0a9dbf95b51a19642995cfede6acf09e");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeB_01 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeB_01.prefab:b66d5647be5ceda48a99af96e883d943");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeC_03 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeC_03.prefab:842a7e1730fbd15419c9aef2b17ca215");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Intro_02 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Intro_02.prefab:97777ec2d8c98f54a9649009784a9dad");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Victory_01 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Victory_01.prefab:6f7c3e0d5816b5947b10c227cf146929");
  private static readonly AssetReference VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Victory_03 = new AssetReference("VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Victory_03.prefab:d23e2affb68181a4b916afd99c394047");
  private List<string> m_BossUsesHeroPowerLines = new List<string>()
  {
    (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_01,
    (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_02,
    (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_03
  };
  private new List<string> m_BossIdleLines = new List<string>()
  {
    (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_01,
    (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_02,
    (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Death_01,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3EmoteResponse_01,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3ExchangeA_01,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3ExchangeB_02,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_01,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_02,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3HeroPower_03,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_01,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_02,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Idle_03,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Intro_01,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Loss_01,
      (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Victory_02,
      (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeA_02,
      (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeB_01,
      (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeC_03,
      (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Intro_02,
      (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Victory_01,
      (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Victory_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Valeera_03 boHValeera03 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHValeera03.MissionPlayVO(actor, (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Intro_01);
    yield return (object) boHValeera03.MissionPlayVO(friendlyActor, (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossIdleLines() => this.m_BossIdleLines;

  public override List<string> GetBossHeroPowerRandomLines() => this.m_BossUsesHeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_deathLine = (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Death_01;
    this.m_OverrideMusicTrack = MusicPlaylistType.InGame_GIL;
    this.m_standardEmoteResponseLine = (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3EmoteResponse_01;
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
    BoH_Valeera_03 boHValeera03 = this;
    while (boHValeera03.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 102:
        GameState.Get().SetBusy(true);
        yield return (object) boHValeera03.PlayLineAlways(friendlyActor, (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Victory_01);
        yield return (object) boHValeera03.PlayLineAlways(enemyActor, (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Victory_02);
        yield return (object) boHValeera03.PlayLineAlways(friendlyActor, (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3Victory_03);
        GameState.Get().SetBusy(false);
        break;
      case 507:
        yield return (object) boHValeera03.PlayLineAlways(enemyActor, (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3Loss_01);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHValeera03.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Valeera_03 boHValeera03 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHValeera03.\u003C\u003En__1(entity);
    while (boHValeera03.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHValeera03.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHValeera03.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHValeera03.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Valeera_03 boHValeera03 = this;
    while (boHValeera03.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHValeera03.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHValeera03.\u003C\u003En__2(entity);
      yield return (object) boHValeera03.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHValeera03.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Valeera_03 boHValeera03 = this;
    while (boHValeera03.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHValeera03.PlayLineAlways(enemyActor, (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3ExchangeA_01);
        yield return (object) boHValeera03.PlayLineAlways(friendlyActor, (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeA_02);
        break;
      case 5:
        yield return (object) boHValeera03.PlayLineAlways(friendlyActor, (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeB_01);
        yield return (object) boHValeera03.PlayLineAlways(enemyActor, (string) BoH_Valeera_03.VO_Story_Hero_Daerion_Male_Human_Story_Valeera_Mission3ExchangeB_02);
        break;
      case 11:
        yield return (object) boHValeera03.PlayLineAlways(friendlyActor, (string) BoH_Valeera_03.VO_Story_Hero_Valeera_Female_BloodElf_Story_Valeera_Mission3ExchangeC_03);
        break;
    }
  }
}
