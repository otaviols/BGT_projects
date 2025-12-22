using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOE15_Boss1 : LOE_MissionEntity
{
  private bool m_magmaRagerLinePlayed;
  private bool m_lowHealth;
  private List<Zone> m_zonesToHide = new List<Zone>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_LOE_15_RESPONSE.prefab:a8accce27ae78cb4fb4b02f58e9c3036");
    this.PreloadSound("VO_LOEA15_1_LOW_HEALTH_10.prefab:a948e1a494daa01429f9c8af98ba2ba7");
    this.PreloadSound("VO_LOEA15_1_TURN1_08.prefab:6a6f93dab22e2aa44bafd03d5da8597a");
    this.PreloadSound("VO_LOEA15_1_MAGMA_RAGER_09.prefab:86f34787a5e7d5441a6331443d36880b");
    this.PreloadSound("VO_LOEA15_1_LOSS_11.prefab:7a3d507088374a240a490fc098f6791b");
    this.PreloadSound("VO_LOEA15_1_WIN_12.prefab:bdad6f025366f8645a65655e1d8fc751");
    this.PreloadSound("VO_LOEA15_GOLDEN.prefab:c7173172b70c5ca40b1f3157ee3e5279");
    this.PreloadSound("VO_LOEA15_1_START_07.prefab:663467aee38cfa3429a9f89eee6177fe");
    this.PreloadSound("VO_LOE_15_SPARE.prefab:a25a36df79b66394ebb054c12e4a9441");
    this.PreloadSound("VO_ELISE_WEIRD_DECK_05.prefab:89915bf24c91ba642bf3ae842e267fe3");
  }

  protected override void InitEmoteResponses() => this.m_emoteResponseGroups = new List<MissionEntity.EmoteResponseGroup>()
  {
    new MissionEntity.EmoteResponseGroup()
    {
      m_triggers = new List<EmoteType>((IEnumerable<EmoteType>) MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS),
      m_responses = new List<MissionEntity.EmoteResponse>()
      {
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_LOE_15_RESPONSE.prefab:a8accce27ae78cb4fb4b02f58e9c3036",
          m_stringTag = "VO_LOE_15_RESPONSE"
        }
      }
    }
  };

  public override bool DoAlternateMulliganIntro()
  {
    GameState.Get().GetOpposingSidePlayer().GetDeckZone().SetVisibility(false);
    this.m_zonesToHide.Clear();
    this.m_zonesToHide.AddRange((IEnumerable<Zone>) ZoneMgr.Get().FindZonesForTag(TAG_ZONE.HAND));
    this.m_zonesToHide.AddRange((IEnumerable<Zone>) ZoneMgr.Get().FindZonesForTag(TAG_ZONE.DECK));
    foreach (Zone zone in this.m_zonesToHide)
    {
      foreach (Card card in zone.GetCards())
      {
        card.HideCard();
        card.SetDoNotSort(true);
      }
    }
    return false;
  }

  public override IEnumerator DoActionsAfterIntroBeforeMulligan()
  {
    LOE15_Boss1 loE15Boss1 = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    LOE_DeckTakeEvent deckTakeEvent = AssetLoader.Get().InstantiatePrefab((AssetReference) "LOE_DeckTakeEvent.prefab:1d55e39305085094cbe8598e5fde37aa").GetComponent<LOE_DeckTakeEvent>();
    yield return (object) new WaitForEndOfFrame();
    ZoneDeck deckZone = GameState.Get().GetOpposingSidePlayer().GetDeckZone();
    deckZone.SetVisibility(true);
    Gameplay.Get().SwapCardBacks();
    deckZone.SetVisibility(false);
    GameState.Get().GetFriendlySidePlayer().GetDeckZone().SetVisibility(false);
    Gameplay.Get().StartCoroutine(deckTakeEvent.PlayTakeDeckAnim());
    yield return (object) Gameplay.Get().StartCoroutine(loE15Boss1.PlaySoundAndBlockSpeech("VO_LOEA15_1_START_07.prefab:663467aee38cfa3429a9f89eee6177fe", Notification.SpeechBubbleDirection.TopRight, enemyActor));
    while (deckTakeEvent.AnimIsPlaying())
      yield return (object) null;
    Gameplay.Get().StartCoroutine(deckTakeEvent.PlayReplacementDeckAnim());
    yield return (object) Gameplay.Get().StartCoroutine(loE15Boss1.PlayBigCharacterQuoteAndWait("Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26", "VO_LOE_15_SPARE.prefab:a25a36df79b66394ebb054c12e4a9441"));
    while (deckTakeEvent.AnimIsPlaying())
      yield return (object) null;
    foreach (Zone zone in loE15Boss1.m_zonesToHide)
    {
      foreach (Card card in zone.GetCards())
      {
        card.ShowCard();
        card.SetDoNotSort(false);
      }
    }
    loE15Boss1.m_zonesToHide.Clear();
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LOE15_Boss1 loE15Boss1 = this;
    while (loE15Boss1.m_enemySpeaking)
      yield return (object) null;
    Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
    Actor actor = opposingSidePlayer.GetHeroCard().GetActor();
    if (!loE15Boss1.m_lowHealth && opposingSidePlayer.GetHero().GetCurrentHealth() < 10 && GameState.Get().GetFriendlySidePlayer().GetHero().GetCurrentHealth() > 19)
    {
      loE15Boss1.m_lowHealth = true;
      Gameplay.Get().StartCoroutine(loE15Boss1.PlaySoundAndBlockSpeechOnce("VO_LOEA15_1_LOW_HEALTH_10.prefab:a948e1a494daa01429f9c8af98ba2ba7", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      switch (turn)
      {
        case 1:
          if (GameState.Get().GetGameEntity().GetCost() == 1)
          {
            Gameplay.Get().StartCoroutine(loE15Boss1.PlaySoundAndBlockSpeechOnce("VO_LOEA15_GOLDEN.prefab:c7173172b70c5ca40b1f3157ee3e5279", Notification.SpeechBubbleDirection.TopRight, actor));
            break;
          }
          Gameplay.Get().StartCoroutine(loE15Boss1.PlaySoundAndBlockSpeechOnce("VO_LOEA15_1_TURN1_08.prefab:6a6f93dab22e2aa44bafd03d5da8597a", Notification.SpeechBubbleDirection.TopRight, actor));
          break;
        case 5:
          yield return (object) Gameplay.Get().StartCoroutine(loE15Boss1.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26", "VO_ELISE_WEIRD_DECK_05.prefab:89915bf24c91ba642bf3ae842e267fe3"));
          break;
      }
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    LOE15_Boss1 loE15Boss1 = this;
    while (loE15Boss1.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (entity.GetCardId() == "CS2_118" && !loE15Boss1.m_magmaRagerLinePlayed)
    {
      loE15Boss1.m_magmaRagerLinePlayed = true;
      Gameplay.Get().StartCoroutine(loE15Boss1.PlaySoundAndBlockSpeechOnce("VO_LOEA15_1_MAGMA_RAGER_09.prefab:86f34787a5e7d5441a6331443d36880b", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    LOE15_Boss1 loE15Boss1 = this;
    if (!GameMgr.Get().IsClassChallengeMission())
    {
      if (gameResult == TAG_PLAYSTATE.WON)
      {
        yield return (object) new WaitForSeconds(5f);
        Gameplay.Get().StartCoroutine(loE15Boss1.PlayCharacterQuoteAndWait("Rafaam_wrap_Quote.prefab:d7100015bf618604ea93bad6b9f54f8b", "VO_LOEA15_1_LOSS_11.prefab:7a3d507088374a240a490fc098f6791b", allowRepeatDuringSession: false));
      }
      else if (gameResult == TAG_PLAYSTATE.LOST)
      {
        yield return (object) new WaitForSeconds(5f);
        Gameplay.Get().StartCoroutine(loE15Boss1.PlayCharacterQuoteAndWait("Rafaam_wrap_Quote.prefab:d7100015bf618604ea93bad6b9f54f8b", "VO_LOEA15_1_WIN_12.prefab:bdad6f025366f8645a65655e1d8fc751", allowRepeatDuringSession: false));
      }
    }
  }
}
