using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRM05_BaronGeddon : BRM_MissionEntity
{
  private bool m_heroPowerLinePlayed;
  private bool m_cardLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA05_1_RESPONSE_03.prefab:beac5b0620de49f42a2f2a66a906d4d6");
    this.PreloadSound("VO_BRMA05_1_HERO_POWER_06.prefab:2792e43708ba1df48baa3a41d636097a");
    this.PreloadSound("VO_BRMA05_1_CARD_05.prefab:c0bc2f9cc3d3ae047ba80ffa0f70dcb8");
    this.PreloadSound("VO_BRMA05_1_TURN1_02.prefab:b68353491d7f88a4a8479e7a031aec12");
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
          m_soundName = "VO_BRMA05_1_RESPONSE_03.prefab:beac5b0620de49f42a2f2a66a906d4d6",
          m_stringTag = "VO_BRMA05_1_RESPONSE_03"
        }
      }
    }
  };

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BRM05_BaronGeddon brM05BaronGeddon = this;
    while (brM05BaronGeddon.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "BRMA05_2") && !(cardId == "BRMA05_2H"))
    {
      if ((cardId == "BRMA05_3" || cardId == "BRMA05_3H") && !brM05BaronGeddon.m_cardLinePlayed)
      {
        brM05BaronGeddon.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(brM05BaronGeddon.PlaySoundAndBlockSpeech("VO_BRMA05_1_CARD_05.prefab:c0bc2f9cc3d3ae047ba80ffa0f70dcb8", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!brM05BaronGeddon.m_heroPowerLinePlayed)
    {
      brM05BaronGeddon.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(brM05BaronGeddon.PlaySoundAndBlockSpeech("VO_BRMA05_1_HERO_POWER_06.prefab:2792e43708ba1df48baa3a41d636097a", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BRM05_BaronGeddon brM05BaronGeddon = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (turn == 1)
      Gameplay.Get().StartCoroutine(brM05BaronGeddon.PlaySoundAndBlockSpeech("VO_BRMA05_1_TURN1_02.prefab:b68353491d7f88a4a8479e7a031aec12", Notification.SpeechBubbleDirection.TopRight, actor));
    return false;
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", GameStrings.Get("VO_NEFARIAN_BARON_GEDDON_DEAD_40"), "VO_NEFARIAN_BARON_GEDDON_DEAD_40.prefab:6872a4eb94e17a847aebec382654c835");
    }
  }
}
