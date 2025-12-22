using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRM01_GrimGuzzler : BRM_MissionEntity
{
  private bool m_heroPowerLinePlayed;
  private bool m_cardLinePlayed;
  private bool m_eTCLinePlayed;
  private bool m_succubusLinePlayed;
  private bool m_warGolemLinePlayed;
  private bool m_disableSpecialCardVO = true;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA01_1_RESPONSE_03.prefab:6bce12880a0d1ef408aab318f0c0d699");
    this.PreloadSound("VO_BRMA01_1_HERO_POWER_04.prefab:ddd556f3fc3107642ba85ffa60e56efd");
    this.PreloadSound("VO_BRMA01_1_CARD_05.prefab:4d60a73b9cc4c3645a387eb198be2d8a");
    this.PreloadSound("VO_BRMA01_1_ETC_06.prefab:966f2e43e86303b4da0adc2529bd22a3");
    this.PreloadSound("VO_BRMA01_1_SUCCUBUS_08.prefab:6e04c1f0a2ce98d4187e5ee6499211a9");
    this.PreloadSound("VO_BRMA01_1_WARGOLEM_07.prefab:d45ef18ce5906d74e94ecb0e56323d37");
    this.PreloadSound("VO_BRMA01_1_TURN1_02.prefab:914cf2bf87da18c458f38f8fcbc98481");
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
          m_soundName = "VO_BRMA01_1_RESPONSE_03.prefab:6bce12880a0d1ef408aab318f0c0d699",
          m_stringTag = "VO_BRMA01_1_RESPONSE_03"
        }
      }
    }
  };

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BRM01_GrimGuzzler brM01GrimGuzzler = this;
    while (brM01GrimGuzzler.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "BRMA01_2") && !(cardId == "BRMA01_2H"))
    {
      if (cardId == "BRMA01_4" && !brM01GrimGuzzler.m_cardLinePlayed)
      {
        brM01GrimGuzzler.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(brM01GrimGuzzler.PlaySoundAndBlockSpeech("VO_BRMA01_1_CARD_05.prefab:4d60a73b9cc4c3645a387eb198be2d8a", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!brM01GrimGuzzler.m_heroPowerLinePlayed)
    {
      brM01GrimGuzzler.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(brM01GrimGuzzler.PlaySoundAndBlockSpeech("VO_BRMA01_1_HERO_POWER_04.prefab:ddd556f3fc3107642ba85ffa60e56efd", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BRM01_GrimGuzzler brM01GrimGuzzler = this;
    while (brM01GrimGuzzler.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 1:
        if (brM01GrimGuzzler.m_eTCLinePlayed || brM01GrimGuzzler.m_disableSpecialCardVO)
          break;
        brM01GrimGuzzler.m_eTCLinePlayed = true;
        GameState.Get().SetBusy(true);
        Gameplay.Get().StartCoroutine(brM01GrimGuzzler.PlaySoundAndBlockSpeech("VO_BRMA01_1_ETC_06.prefab:966f2e43e86303b4da0adc2529bd22a3", Notification.SpeechBubbleDirection.TopRight, actor));
        GameState.Get().SetBusy(false);
        break;
      case 2:
        if (brM01GrimGuzzler.m_succubusLinePlayed || brM01GrimGuzzler.m_disableSpecialCardVO)
          break;
        brM01GrimGuzzler.m_succubusLinePlayed = true;
        GameState.Get().SetBusy(true);
        Gameplay.Get().StartCoroutine(brM01GrimGuzzler.PlaySoundAndBlockSpeech("VO_BRMA01_1_SUCCUBUS_08.prefab:6e04c1f0a2ce98d4187e5ee6499211a9", Notification.SpeechBubbleDirection.TopRight, actor));
        GameState.Get().SetBusy(false);
        break;
      case 3:
        if (brM01GrimGuzzler.m_warGolemLinePlayed || brM01GrimGuzzler.m_disableSpecialCardVO)
          break;
        brM01GrimGuzzler.m_warGolemLinePlayed = true;
        GameState.Get().SetBusy(true);
        Gameplay.Get().StartCoroutine(brM01GrimGuzzler.PlaySoundAndBlockSpeech("VO_BRMA01_1_WARGOLEM_07.prefab:d45ef18ce5906d74e94ecb0e56323d37", Notification.SpeechBubbleDirection.TopRight, actor));
        GameState.Get().SetBusy(false);
        break;
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BRM01_GrimGuzzler brM01GrimGuzzler = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (turn)
    {
      case 1:
        Gameplay.Get().StartCoroutine(brM01GrimGuzzler.PlaySoundAndBlockSpeech("VO_BRMA01_1_TURN1_02.prefab:914cf2bf87da18c458f38f8fcbc98481", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case 3:
        brM01GrimGuzzler.m_disableSpecialCardVO = false;
        break;
    }
    return false;
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", GameStrings.Get("VO_NEFARIAN_COREN_DEAD_28"), "VO_NEFARIAN_COREN_DEAD_28.prefab:0539437e9ff9ee9409bd7cd236d59d53");
    }
  }
}
