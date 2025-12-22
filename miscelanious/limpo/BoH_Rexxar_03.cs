using Blizzard.T5.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoH_Rexxar_03 : BoH_Rexxar_Dungeon
{
  private static Map<GameEntityOption, bool> s_booleanOptions = BoH_Rexxar_03.InitBooleanOptions();
  private static readonly AssetReference VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Death_01 = new AssetReference("VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Death_01.prefab:566174dbe9ef429887501ebc140f4f2a");
  private static readonly AssetReference VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3EmoteResponse_01 = new AssetReference("VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3EmoteResponse_01.prefab:59fd122df5164b69abe23f085e586d65");
  private static readonly AssetReference VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3HeroPower_01 = new AssetReference("VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3HeroPower_01.prefab:6e3a504a826044bbb96b49bc91455f9c");
  private static readonly AssetReference VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3HeroPower_02 = new AssetReference("VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3HeroPower_02.prefab:9a7d4eff6de64775b381aa66d6a44664");
  private static readonly AssetReference VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3HeroPower_03 = new AssetReference("VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3HeroPower_03.prefab:a45450453ee5402d9ca301d00a32b301");
  private static readonly AssetReference VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Intro_01 = new AssetReference("VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Intro_01.prefab:f2474a3935334b11bc02bb0e54cdd893");
  private static readonly AssetReference VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Loss_01 = new AssetReference("VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Loss_01.prefab:7d0ad7ff8ef74817b9e50e9d5a37e390");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeA_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeA_01.prefab:7f907c72e23854845ae2acb64b9e3d4c");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeB_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeB_01.prefab:85fedca340d633244a550d25b6901436");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeC_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeC_01.prefab:31151640d943ecf4ab956f31b21a7edf");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeD_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeD_01.prefab:c30108161449b3849ae9abfec48b2e38");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeE_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeE_01.prefab:ee8683e8c73356f419b230905ea18c8c");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Intro_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Intro_01.prefab:d0ffecd2bd207084e9e923f34c817b3a");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Intro_02 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Intro_02.prefab:d604c2b5d871dcb4399f3a985d962864");
  private static readonly AssetReference VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Victory_01 = new AssetReference("VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Victory_01.prefab:337f228655730674aa797ec48f8a79e0");
  protected Notification m_minionMoveTutorialNotification;
  protected bool m_shouldPlayMinionMoveTutorial = true;
  private HashSet<string> m_playedLines = new HashSet<string>();

  private static Map<GameEntityOption, bool> InitBooleanOptions() => new Map<GameEntityOption, bool>()
  {
    {
      GameEntityOption.DO_OPENING_TAUNTS,
      false
    }
  };

  public BoH_Rexxar_03() => this.m_gameOptions.AddBooleanOptions(BoH_Rexxar_03.s_booleanOptions);

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    List<string> VOLines = new List<string>()
    {
      (string) BoH_Rexxar_03.VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Death_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3EmoteResponse_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3HeroPower_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3HeroPower_02,
      (string) BoH_Rexxar_03.VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3HeroPower_03,
      (string) BoH_Rexxar_03.VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Intro_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Loss_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeA_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeB_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeC_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeD_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeE_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Intro_01,
      (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Intro_02,
      (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Victory_01
    };
    this.SetBossVOLines(VOLines);
    foreach (string soundPath in VOLines)
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSuppressDeathTextBubble() => true;

  public override bool ShouldPlayHeroBlowUpSpells(TAG_PLAYSTATE playState) => playState != TAG_PLAYSTATE.WON;

  public override IEnumerator DoActionsBeforeDealingBaseMulliganCards()
  {
    BoH_Rexxar_03 boHRexxar03 = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor friendlyActor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    GameState.Get().SetBusy(true);
    yield return (object) boHRexxar03.PlayLineAlways(actor, (string) BoH_Rexxar_03.VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Intro_01);
    yield return (object) boHRexxar03.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Intro_01);
    yield return (object) boHRexxar03.PlayLineAlways(friendlyActor, (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Intro_02);
    GameState.Get().SetBusy(false);
  }

  public override void OnCreateGame()
  {
    base.OnCreateGame();
    this.m_standardEmoteResponseLine = (string) BoH_Rexxar_03.VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3EmoteResponse_01;
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
    BoH_Rexxar_03 boHRexxar03 = this;
    while (boHRexxar03.m_enemySpeaking)
      yield return (object) null;
    Actor actor1 = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor2 = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (missionEvent)
    {
      case 228:
        GameState.Get().SetBusy(true);
        boHRexxar03.ShowMinionMoveTutorial();
        yield return (object) new WaitForSeconds(3f);
        boHRexxar03.HideNotification(boHRexxar03.m_minionMoveTutorialNotification);
        GameState.Get().SetBusy(false);
        break;
      case 501:
        GameState.Get().SetBusy(true);
        yield return (object) boHRexxar03.PlayLineAlways(actor2, (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3Victory_01);
        GameState.Get().SetBusy(false);
        break;
      case 504:
        GameState.Get().SetBusy(true);
        yield return (object) boHRexxar03.PlayLineAlways(actor1, (string) BoH_Rexxar_03.VO_Story_Hero_Misha_Female_Bear_Story_Rexxar_Mission3Loss_01);
        GameState.Get().SetBusy(false);
        break;
      default:
        // ISSUE: reference to a compiler-generated method
        yield return (object) boHRexxar03.\u003C\u003En__0(missionEvent);
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    BoH_Rexxar_03 boHRexxar03 = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) boHRexxar03.\u003C\u003En__1(entity);
    while (boHRexxar03.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHRexxar03.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      yield return (object) boHRexxar03.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHRexxar03.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BoH_Rexxar_03 boHRexxar03 = this;
    while (boHRexxar03.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!boHRexxar03.m_playedLines.Contains(entity.GetCardId()) || entity.GetCardType() == TAG_CARDTYPE.HERO_POWER)
    {
      // ISSUE: reference to a compiler-generated method
      yield return (object) boHRexxar03.\u003C\u003En__2(entity);
      yield return (object) boHRexxar03.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      boHRexxar03.m_playedLines.Add(cardId);
      GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CS2_012"))
      {
        if (cardId == "LOOT_314")
          yield return (object) boHRexxar03.PlayLineOnlyOnce(actor, (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeE_01);
      }
      else
        yield return (object) boHRexxar03.PlayLineOnlyOnce(actor, (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeD_01);
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BoH_Rexxar_03 boHRexxar03 = this;
    while (boHRexxar03.m_enemySpeaking)
      yield return (object) null;
    GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    Actor actor = GameState.Get().GetFriendlySidePlayer().GetHero().GetCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) boHRexxar03.PlayLineAlways(actor, (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeA_01);
        break;
      case 5:
        yield return (object) boHRexxar03.PlayLineAlways(actor, (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeB_01);
        break;
      case 9:
        yield return (object) boHRexxar03.PlayLineAlways(actor, (string) BoH_Rexxar_03.VO_Story_Hero_Rexxar_Male_OrcOgre_Story_Rexxar_Mission3ExchangeC_01);
        break;
    }
  }

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_BT);

  protected void ShowMinionMoveTutorial()
  {
    Card minionInOpponentPlay = this.GetLeftMostMinionInOpponentPlay();
    if ((Object) minionInOpponentPlay == (Object) null)
      return;
    Vector3 position1 = minionInOpponentPlay.transform.position;
    Vector3 position2 = !(bool) UniversalInputManager.UsePhoneUI ? new Vector3(position1.x, position1.y, position1.z + 2.5f) : new Vector3(position1.x + 0.05f, position1.y, position1.z + 2.6f);
    string key = "BOH_REXXAR_02";
    this.m_minionMoveTutorialNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position2, TutorialEntity.GetTextScale(), GameStrings.Get(key));
    this.m_minionMoveTutorialNotification.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
    this.m_minionMoveTutorialNotification.PulseReminderEveryXSeconds(2f);
  }

  protected Card GetLeftMostMinionInOpponentPlay()
  {
    foreach (Card card in GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards())
    {
      if (card.GetEntity().GetTag(GAME_TAG.ZONE_POSITION) == 1)
        return card;
    }
    return (Card) null;
  }

  protected void HideNotification(Notification notification, bool hideImmediately = false)
  {
    if (!((Object) notification != (Object) null))
      return;
    if (hideImmediately)
      NotificationManager.Get().DestroyNotificationNowWithNoAnim(notification);
    else
      NotificationManager.Get().DestroyNotification(notification, 0.0f);
  }
}
