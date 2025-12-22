using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRM04_Garr : BRM_MissionEntity
{
  private bool m_heroPowerLinePlayed;
  private bool m_cardLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA04_1_RESPONSE_03.prefab:75a029ecfd071914aaf0def7bc041b85");
    this.PreloadSound("VO_BRMA04_1_HERO_POWER_05.prefab:1c2e947768a86424abf65a8b5ad573ec");
    this.PreloadSound("VO_BRMA04_1_CARD_04.prefab:53f20ec5598fc8a459615f6a57c661be");
    this.PreloadSound("VO_BRMA04_1_TURN1_02.prefab:198010c5061020b499e36ee02b9a6e9f");
    this.PreloadSound("VO_NEFARIAN_GARR2_35.prefab:17167cbeb359c8c459a1ce3824206474");
    this.PreloadSound("VO_NEFARIAN_GARR3_36.prefab:a9d3c5553f63ed54bac596039f115511");
    this.PreloadSound("VO_NEFARIAN_GARR4_37.prefab:12898207b42d4ca42b7cdb7a711f5726");
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
          m_soundName = "VO_BRMA04_1_RESPONSE_03.prefab:75a029ecfd071914aaf0def7bc041b85",
          m_stringTag = "VO_BRMA04_1_RESPONSE_03"
        }
      }
    }
  };

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BRM04_Garr brM04Garr = this;
    while (brM04Garr.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "BRMA04_2"))
    {
      if ((cardId == "BRMA04_4" || cardId == "BRMA04_4H") && !brM04Garr.m_cardLinePlayed)
      {
        brM04Garr.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(brM04Garr.PlaySoundAndBlockSpeech("VO_BRMA04_1_CARD_04.prefab:53f20ec5598fc8a459615f6a57c661be", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!brM04Garr.m_heroPowerLinePlayed)
    {
      brM04Garr.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(brM04Garr.PlaySoundAndBlockSpeech("VO_BRMA04_1_HERO_POWER_05.prefab:1c2e947768a86424abf65a8b5ad573ec", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BRM04_Garr brM04Garr = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Vector3 position = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (turn)
    {
      case 1:
        Gameplay.Get().StartCoroutine(brM04Garr.PlaySoundAndBlockSpeech("VO_BRMA04_1_TURN1_02.prefab:198010c5061020b499e36ee02b9a6e9f", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case 4:
        if (!GameMgr.Get().IsClassChallengeMission())
        {
          NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", position, GameStrings.Get("VO_NEFARIAN_GARR2_35"), "VO_NEFARIAN_GARR2_35.prefab:17167cbeb359c8c459a1ce3824206474");
          break;
        }
        break;
      case 8:
        if (!GameMgr.Get().IsClassChallengeMission())
        {
          NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", position, GameStrings.Get("VO_NEFARIAN_GARR3_36"), "VO_NEFARIAN_GARR3_36.prefab:a9d3c5553f63ed54bac596039f115511");
          break;
        }
        break;
      case 12:
        if (!GameMgr.Get().IsClassChallengeMission())
        {
          NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", position, GameStrings.Get("VO_NEFARIAN_GARR4_37"), "VO_NEFARIAN_GARR4_37.prefab:12898207b42d4ca42b7cdb7a711f5726");
          break;
        }
        break;
    }
    return false;
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", GameStrings.Get("VO_NEFARIAN_GARR_DEAD1_38"), "VO_NEFARIAN_GARR_DEAD1_38.prefab:7cfd65566df0d294f9591e2ad70e1781");
    }
  }
}
