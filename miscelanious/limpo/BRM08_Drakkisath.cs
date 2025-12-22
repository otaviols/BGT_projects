using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRM08_Drakkisath : BRM_MissionEntity
{
  private bool m_cardLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA08_1_RESPONSE_04.prefab:6f5840c652a862f46baddba08396a839");
    this.PreloadSound("VO_BRMA08_1_CARD_05.prefab:fd0151285c6aec540b909e0a29f5acb8");
    this.PreloadSound("VO_BRMA08_1_TURN1_03.prefab:e4d206e77c3c8f548934be5fcce89ea5");
    this.PreloadSound("VO_NEFARIAN_DRAKKISATH_RESPOND_48.prefab:f422b6326aa079743967cb9988b445c7");
    this.PreloadSound("VO_BRMA08_1_TURN1_ALT_02.prefab:d71f74a42d0105446a5e2e3c4b60e067");
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
          m_soundName = "VO_BRMA08_1_RESPONSE_04.prefab:6f5840c652a862f46baddba08396a839",
          m_stringTag = "VO_BRMA08_1_RESPONSE_04"
        }
      }
    }
  };

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BRM08_Drakkisath brM08Drakkisath = this;
    while (brM08Drakkisath.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (entity.GetCardId() == "BRMA08_3" && !brM08Drakkisath.m_cardLinePlayed)
    {
      brM08Drakkisath.m_cardLinePlayed = true;
      Gameplay.Get().StartCoroutine(brM08Drakkisath.PlaySoundAndBlockSpeech("VO_BRMA08_1_CARD_05.prefab:fd0151285c6aec540b909e0a29f5acb8", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    BRM08_Drakkisath brM08Drakkisath = this;
    Vector3 quotePos = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (turn)
    {
      case 1:
        yield return (object) Gameplay.Get().StartCoroutine(brM08Drakkisath.PlaySoundAndBlockSpeech("VO_BRMA08_1_TURN1_ALT_02.prefab:d71f74a42d0105446a5e2e3c4b60e067", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case 4:
        if (GameMgr.Get().IsClassChallengeMission())
          break;
        yield return (object) Gameplay.Get().StartCoroutine(brM08Drakkisath.PlaySoundAndBlockSpeech("VO_BRMA08_1_TURN1_03.prefab:e4d206e77c3c8f548934be5fcce89ea5", Notification.SpeechBubbleDirection.TopRight, actor));
        NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", quotePos, GameStrings.Get("VO_NEFARIAN_DRAKKISATH_RESPOND_48"), "VO_NEFARIAN_DRAKKISATH_RESPOND_48.prefab:f422b6326aa079743967cb9988b445c7");
        break;
      case 6:
        if (GameMgr.Get().IsClassChallengeMission())
          break;
        NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", quotePos, GameStrings.Get("VO_NEFARIAN_DRAKKISATH1_49"), "VO_NEFARIAN_DRAKKISATH1_49.prefab:792c02424ea5f5d43989d65b4b3ca839");
        break;
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", GameStrings.Get("VO_NEFARIAN_DRAKKISATH_DEAD_50"), "VO_NEFARIAN_DRAKKISATH_DEAD_50.prefab:a0d0aa371c62ff24ca675731ff3e5396");
    }
  }
}
