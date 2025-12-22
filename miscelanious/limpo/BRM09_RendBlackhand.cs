using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRM09_RendBlackhand : BRM_MissionEntity
{
  private bool m_heroPower1LinePlayed;
  private bool m_heroPower2LinePlayed;
  private bool m_heroPower3LinePlayed;
  private bool m_heroPower4LinePlayed;
  private bool m_cardLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA09_1_RESPONSE_04.prefab:425f0a827cd452642835f70c4ddaf74b");
    this.PreloadSound("VO_BRMA09_1_HERO_POWER1_06.prefab:fb6da306ffffaee43b86149480e916d1");
    this.PreloadSound("VO_BRMA09_1_HERO_POWER2_07.prefab:f59ec9255ae38a64c83c90b88b9b04ca");
    this.PreloadSound("VO_BRMA09_1_HERO_POWER3_08.prefab:213cfaf6d91a2ba42b087e59ed8099a4");
    this.PreloadSound("VO_BRMA09_1_HERO_POWER4_09.prefab:1068556d8d1e5694eb2d7925ec96957f");
    this.PreloadSound("VO_BRMA09_1_CARD_05.prefab:2976c75ead5ee7c409ff9ee6ce5bb77d");
    this.PreloadSound("VO_BRMA09_1_TURN1_03.prefab:72e081aa0659e0d49ad1b5dfa890f1c5");
    this.PreloadSound("VO_NEFARIAN_REND1_52.prefab:46edb7255109f2c44884b4871e0a4ede");
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
          m_soundName = "VO_BRMA09_1_RESPONSE_04.prefab:425f0a827cd452642835f70c4ddaf74b",
          m_stringTag = "VO_BRMA09_1_RESPONSE_04"
        }
      }
    }
  };

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BRM09_RendBlackhand m09RendBlackhand = this;
    while (m09RendBlackhand.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (entity.GetCardId())
    {
      case "BRMA09_2":
      case "BRMA09_2H":
        if (m09RendBlackhand.m_heroPower1LinePlayed)
          break;
        m09RendBlackhand.m_heroPower1LinePlayed = true;
        Gameplay.Get().StartCoroutine(m09RendBlackhand.PlaySoundAndBlockSpeech("VO_BRMA09_1_HERO_POWER1_06.prefab:fb6da306ffffaee43b86149480e916d1", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case "BRMA09_3":
      case "BRMA09_3H":
        if (m09RendBlackhand.m_heroPower2LinePlayed)
          break;
        m09RendBlackhand.m_heroPower2LinePlayed = true;
        Gameplay.Get().StartCoroutine(m09RendBlackhand.PlaySoundAndBlockSpeech("VO_BRMA09_1_HERO_POWER2_07.prefab:f59ec9255ae38a64c83c90b88b9b04ca", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case "BRMA09_4":
      case "BRMA09_4H":
        if (m09RendBlackhand.m_heroPower3LinePlayed)
          break;
        m09RendBlackhand.m_heroPower3LinePlayed = true;
        Gameplay.Get().StartCoroutine(m09RendBlackhand.PlaySoundAndBlockSpeech("VO_BRMA09_1_HERO_POWER3_08.prefab:213cfaf6d91a2ba42b087e59ed8099a4", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case "BRMA09_5":
      case "BRMA09_5H":
        if (m09RendBlackhand.m_heroPower4LinePlayed)
          break;
        m09RendBlackhand.m_heroPower4LinePlayed = true;
        Gameplay.Get().StartCoroutine(m09RendBlackhand.PlaySoundAndBlockSpeech("VO_BRMA09_1_HERO_POWER4_09.prefab:1068556d8d1e5694eb2d7925ec96957f", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case "BRMA09_6":
        if (m09RendBlackhand.m_cardLinePlayed)
          break;
        m09RendBlackhand.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(m09RendBlackhand.PlaySoundAndBlockSpeech("VO_BRMA09_1_CARD_05.prefab:2976c75ead5ee7c409ff9ee6ce5bb77d", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BRM09_RendBlackhand m09RendBlackhand = this;
    Vector3 quotePos = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (turn == 1)
    {
      yield return (object) Gameplay.Get().StartCoroutine(m09RendBlackhand.PlaySoundAndBlockSpeech("VO_BRMA09_1_TURN1_03.prefab:72e081aa0659e0d49ad1b5dfa890f1c5", Notification.SpeechBubbleDirection.TopRight, actor));
      if (!GameMgr.Get().IsClassChallengeMission())
        NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", quotePos, GameStrings.Get("VO_NEFARIAN_REND1_52"), "VO_NEFARIAN_REND1_52.prefab:46edb7255109f2c44884b4871e0a4ede");
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", GameStrings.Get("VO_NEFARIAN_REND_DEAD_53"), "VO_NEFARIAN_REND_DEAD_53.prefab:a81636f855f8587428d4998d2b6847b8");
    }
  }
}
