using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX05_Heigan : NAX_MissionEntity
{
  private bool m_cardLinePlayed;
  private bool m_heroPowerLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX5_01_HP_02.prefab:3f0aa9156dbf8c04198a38aea754c0bf");
    this.PreloadSound("VO_NAX5_01_CARD_03.prefab:877904b2ca125a24983e76dfb44929fb");
    this.PreloadSound("VO_NAX5_01_EMOTE_05.prefab:33680944c523f394e95245a6e4ea9f00");
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
          m_soundName = "VO_NAX5_01_EMOTE_05.prefab:33680944c523f394e95245a6e4ea9f00",
          m_stringTag = "VO_NAX5_01_EMOTE_05"
        }
      }
    }
  };

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_HEIGAN2_55", "VO_KT_HEIGAN2_55.prefab:f465a1b0b2312764f92f4d86160c9dac");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX05_Heigan naX05Heigan = this;
    while (naX05Heigan.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "NAX5_02"))
    {
      if (cardId == "NAX5_03" && !naX05Heigan.m_cardLinePlayed)
      {
        naX05Heigan.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(naX05Heigan.PlaySoundAndBlockSpeech("VO_NAX5_01_CARD_03.prefab:877904b2ca125a24983e76dfb44929fb", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!naX05Heigan.m_heroPowerLinePlayed)
    {
      naX05Heigan.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(naX05Heigan.PlaySoundAndBlockSpeech("VO_NAX5_01_HP_02.prefab:3f0aa9156dbf8c04198a38aea754c0bf", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }
}
