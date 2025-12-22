using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX06_Loatheb : NAX_MissionEntity
{
  private bool m_cardLinePlayed;
  private bool m_heroPowerLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX6_01_HP_02.prefab:3e1a96b32f62ce041a2fc526a59aa3d1");
    this.PreloadSound("VO_NAX6_01_CARD_03.prefab:a0f9ea501662eb344b224d4b1e79aa4c");
    this.PreloadSound("VO_NAX6_01_EMOTE_05.prefab:aa2803acd071a934d98b78ec49d51698");
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
          m_soundName = "VO_NAX6_01_EMOTE_05.prefab:aa2803acd071a934d98b78ec49d51698",
          m_stringTag = "VO_NAX6_01_EMOTE_05"
        }
      }
    }
  };

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_LOATHEB2_57", "VO_KT_LOATHEB2_57.prefab:decea68ad8178c34aad7172b1f7739ac");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX06_Loatheb naX06Loatheb = this;
    while (naX06Loatheb.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "NAX6_02"))
    {
      if (cardId == "NAX6_03" && !naX06Loatheb.m_cardLinePlayed)
      {
        naX06Loatheb.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(naX06Loatheb.PlaySoundAndBlockSpeech("VO_NAX6_01_CARD_03.prefab:a0f9ea501662eb344b224d4b1e79aa4c", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!naX06Loatheb.m_heroPowerLinePlayed)
    {
      naX06Loatheb.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(naX06Loatheb.PlaySoundAndBlockSpeech("VO_NAX6_01_HP_02.prefab:3e1a96b32f62ce041a2fc526a59aa3d1", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }
}
