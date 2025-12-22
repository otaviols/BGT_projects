using System.Collections;
using System.Collections.Generic;

public class BoH_Garrosh_04 : BoH_Garrosh_Dungeon
{
  private static readonly AssetReference VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4Death_01 = new AssetReference("VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4Death_01.prefab:bfa5b4579b1945b1a0beba43c0bcb993");
  private static readonly AssetReference VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4EmoteResponse_01 = new AssetReference("VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4EmoteResponse_01.prefab:66222a97e5eb40f4a5d5a700da9d71d6");
  private static readonly AssetReference VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_01 = new AssetReference("VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_01.prefab:ed3c00e07654463286fa69563f566695");
  private static readonly AssetReference VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_02 = new AssetReference("VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_02.prefab:4f3a87b7dd6d43178a416c61eea55821");
  private static readonly AssetReference VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_03 = new AssetReference("VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_03.prefab:87afd0aa3ed94c9abf14a340502b2690");
  private static readonly AssetReference VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4Loss_01 = new AssetReference("VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4Loss_01.prefab:1de1f7f8705644c499d73059cf8d6e6c");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeA_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeA_01.prefab:2c7ead4c7d2299f4fa0d9777633a973c");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeB_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeB_01.prefab:012246e019320354288264519deffaf2");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeC_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeC_01.prefab:8f17be315bfaae7429fe82fcf776ea7a");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4Intro_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4Intro_01.prefab:e784b22048a1082498984a2ed37ddb42");
  private static readonly AssetReference VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4Victory_01.prefab:3119bee73eaea8b4b9efa03b5ce1b8c8");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4ExchangeC_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4ExchangeC_01.prefab:5eef752e55d6aea4080aeac7e0aeb67c");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_01 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_01.prefab:cf21762ad6240494c8097a3c816d661f");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_02 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_02.prefab:0ed5fe130a8c147408b9cad50624ee34");
  private static readonly AssetReference VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_03 = new AssetReference("VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_03.prefab:cb890dc97b3acd743b7c6cc4db678a6b");
  public static readonly AssetReference ThrallBrassRing = new AssetReference("Thrall_BrassRing_Quote.prefab:962e58c9390b0f842a8b64d0d44cf7b4");
  private List<string> m_VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPowerLines = new List<string>()
  {
    (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_01,
    (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_02,
    (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_03
  };
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4Death_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4EmoteResponse_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_02,
      (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPower_03,
      (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4Loss_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeA_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeB_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeC_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4Intro_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4Victory_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4ExchangeC_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_01,
      (string) BoH_Garrosh_04.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_02,
      (string) BoH_Garrosh_04.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_03
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BoH_Garrosh_04 boHGarrosh04 = this;
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
    this.\u003C\u003E2__current = (object) boHGarrosh04.PlayLineAlways(actor, (string) BoH_Garrosh_04.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4Intro_01);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public override List<string> GetBossHeroPowerRandomLines() => this.m_VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4HeroPowerLines;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4EmoteResponse_01;
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
    BoH_Garrosh_04 boHGarrosh04 = this;
    if (missionEvent == 911)
    {
      GameState.Get().SetBusy(true);
      while (boHGarrosh04.m_enemySpeaking)
        yield return (object) null;
      GameState.Get().SetBusy(false);
    }
    else
    {
      while (boHGarrosh04.m_enemySpeaking)
        yield return (object) null;
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      switch (missionEvent)
      {
        case 501:
          GameState.Get().SetBusy(true);
          yield return (object) boHGarrosh04.PlayLineAlways((string) BoH_Garrosh_04.ThrallBrassRing, (string) BoH_Garrosh_04.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_01);
          yield return (object) boHGarrosh04.PlayLineAlways((string) BoH_Garrosh_04.ThrallBrassRing, (string) BoH_Garrosh_04.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_02);
          yield return (object) boHGarrosh04.PlayLineAlways((string) BoH_Garrosh_04.ThrallBrassRing, (string) BoH_Garrosh_04.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4Victory_03);
          yield return (object) boHGarrosh04.PlayLineAlways(friendlyActor, (string) BoH_Garrosh_04.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4Victory_01);
          GameState.Get().SetBusy(false);
          break;
        case 504:
          GameState.Get().SetBusy(true);
          yield return (object) boHGarrosh04.PlayLineAlways(actor, (string) BoH_Garrosh_04.VO_Story_Hero_FireElemental_Male_Elemental_Story_Garrosh_Mission4Loss_01);
          GameState.Get().SetBusy(false);
          break;
        default:
          // ISSUE: reference to a compiler-generated method
          yield return (object) boHGarrosh04.\u003C\u003En__0(missionEvent);
          break;
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_04 boHGarrosh04 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHGarrosh04.\u003C\u003En__1(entity);
    while (boHGarrosh04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGarrosh04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHGarrosh04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGarrosh04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Garrosh_04 boHGarrosh04 = this;
    while (boHGarrosh04.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHGarrosh04.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHGarrosh04.\u003C\u003En__2(entity);
      yield return (object) boHGarrosh04.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHGarrosh04.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Garrosh_04 boHGarrosh04 = this;
    while (boHGarrosh04.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHGarrosh04.PlayLineAlways(actor, (string) BoH_Garrosh_04.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeA_01);
        break;
      case 3:
        yield return (object) boHGarrosh04.PlayLineAlways(actor, (string) BoH_Garrosh_04.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeB_01);
        break;
      case 7:
        yield return (object) boHGarrosh04.PlayLineAlways(actor, (string) BoH_Garrosh_04.VO_Story_Hero_Garrosh_Male_Orc_Story_Garrosh_Mission4ExchangeC_01);
        yield return (object) boHGarrosh04.PlayLineAlways((string) BoH_Garrosh_04.ThrallBrassRing, (string) BoH_Garrosh_04.VO_Story_Hero_Thrall_Male_Orc_Story_Garrosh_Mission4ExchangeC_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BRMAdventure);
}
