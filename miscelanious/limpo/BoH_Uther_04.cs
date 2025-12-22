using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;

public class BoH_Uther_04 : BoH_Uther_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Uther_04.InitBooleanOptions();
  private static readonly AssetReference Story_04_Darkportal_Death = new AssetReference("Story_04_Darkportal_Death.prefab:2457ed615b87f4644af4528837554e4e");
  private static readonly AssetReference Story_04_Darkportal_EmoteResponse = new AssetReference("Story_04_Darkportal_EmoteResponse.prefab:c1d824da692e02840a52d831053755a0");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeA_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeA_01.prefab:554a329cd72b44f41884fd1d13335aaf");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeB_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeB_01.prefab:d8ef4d696d3295544818b9e29dbc6849");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeD_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeD_01.prefab:ddb028020f81a45449aa25548c53df7f");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4Victory_01.prefab:ebe5c6adbe40a7441a8d115618aba3eb");
  private static readonly AssetReference VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeA_01 = new AssetReference("VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeA_01.prefab:7c183759d499460429614809e924efb2");
  private static readonly AssetReference VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeB_01 = new AssetReference("VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeB_01.prefab:6dcda9c2639256b47a3940d75fd0640d");
  private static readonly AssetReference VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeC_01 = new AssetReference("VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeC_01.prefab:9705efe18dcb35b4e8a05805b708f253");
  private static readonly AssetReference VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4Victory_01 = new AssetReference("VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4Victory_01.prefab:2845658c04680684bb569b5edc83941d");
  private static readonly AssetReference VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4Intro_01 = new AssetReference("VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4Intro_01.prefab:22bd16f3f61d4ff4b87ebbae46f20abe");
  private static readonly AssetReference VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4Intro_01 = new AssetReference("VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4Intro_01.prefab:582f47b3809509c48ab4257a6aff6cad");
  public static readonly AssetReference TuralyonBrassRing = new AssetReference("Turalyon_BrassRing_Quote.prefab:40afbe0d5b4da0643baf2ebf5756548d");
  private List<string> m_HeroPowerLines = new List<string>()
  {
    (string) BoH_Uther_04.Story_04_Darkportal_EmoteResponse
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Uther_04.Story_04_Darkportal_Death,
      (string) BoH_Uther_04.Story_04_Darkportal_EmoteResponse,
      (string) BoH_Uther_04.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4Intro_01,
      (string) BoH_Uther_04.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeA_01,
      (string) BoH_Uther_04.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeB_01,
      (string) BoH_Uther_04.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeD_01,
      (string) BoH_Uther_04.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4Victory_01,
      (string) BoH_Uther_04.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeA_01,
      (string) BoH_Uther_04.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeB_01,
      (string) BoH_Uther_04.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeC_01,
      (string) BoH_Uther_04.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4Victory_01,
      (string) BoH_Uther_04.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4Intro_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Uther_04 boHUther04 = this;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHUther04.PlayLineAlways((string) BoH_Uther_04.TuralyonBrassRing, (string) BoH_Uther_04.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4Intro_01);
    yield return (object) boHUther04.PlayLineAlways(friendlyActor, (string) BoH_Uther_04.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4Intro_01);
    GameState.Get().SetBusy(false);
  }

  public override List<string> GetBossHeroPowerRandomLines() => this.m_HeroPowerLines;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Uther_04.Story_04_Darkportal_EmoteResponse;
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetActor();
    GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
    if (emoteType == EmoteType.START || !MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
      return;
    Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(this.m_standardEmoteResponseLine, Notification.SpeechBubbleDirection.TopRight, actor));
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BoH_Uther_04 boHUther04 = this;
    while (boHUther04.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) boHUther04.PlayLineAlways(actor2, (string) BoH_Uther_04.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4Victory_01);
        yield return (object) boHUther04.PlayLineAlways((string) BoH_Uther_04.TuralyonBrassRing, (string) BoH_Uther_04.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4Victory_01);
        GameState.Get().SetBusy(true);
        break;
      case 502:
        GameState.Get().SetBusy(true);
        yield return (object) boHUther04.PlayLineAlways(actor2, (string) BoH_Uther_04.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeD_01);
        GameState.Get().SetBusy(true);
        break;
      case 504:
        yield return (object) boHUther04.PlayLineAlways(actor1, (string) BoH_Uther_04.Story_04_Darkportal_EmoteResponse);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHUther04.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Uther_04 boHUther04 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHUther04.\u003C\u003En__1(entity);
    while (boHUther04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHUther04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHUther04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHUther04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Uther_04 boHUther04 = this;
    while (boHUther04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHUther04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHUther04.\u003C\u003En__2(entity);
      yield return (object) boHUther04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHUther04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Uther_04 boHUther04 = this;
    while (boHUther04.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 3:
        yield return (object) boHUther04.PlayLineAlways((string) BoH_Uther_04.TuralyonBrassRing, (string) BoH_Uther_04.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeA_01);
        yield return (object) boHUther04.PlayLineAlways(friendlyActor, (string) BoH_Uther_04.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeA_01);
        break;
      case 9:
        yield return (object) boHUther04.PlayLineAlways((string) BoH_Uther_04.TuralyonBrassRing, (string) BoH_Uther_04.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeB_01);
        yield return (object) boHUther04.PlayLineAlways(friendlyActor, (string) BoH_Uther_04.VO_Story_Hero_Uther_Male_Human_Story_Uther_Mission4ExchangeB_01);
        break;
      case 15:
        yield return (object) boHUther04.PlayLineAlways((string) BoH_Uther_04.TuralyonBrassRing, (string) BoH_Uther_04.VO_Story_Minion_Turalyon_Male_Human_Story_Uther_Mission4ExchangeC_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.Store_PacksBT);
}
