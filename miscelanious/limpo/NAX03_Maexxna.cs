using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX03_Maexxna : NAX_MissionEntity
{
  private bool m_heroPowerLinePlayed;
  private bool m_seaGiantLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX3_01_EMOTE_01.prefab:a31d7a46b65b449479a2a531268f9299");
    this.PreloadSound("VO_NAX3_01_EMOTE_02.prefab:c1e3c5438c9b2a1469badb6ac78ed010");
    this.PreloadSound("VO_NAX3_01_EMOTE_03.prefab:2d1d268f8503f0d4090ac41624711149");
    this.PreloadSound("VO_NAX3_01_EMOTE_04.prefab:8163100101e44994680768b0f4220eec");
    this.PreloadSound("VO_NAX3_01_EMOTE_05.prefab:0fc3e310650ab7149b5bf2b794895869");
    this.PreloadSound("VO_NAX3_01_CARD_01.prefab:8f9084036cb9a31429b886ee01cc9bad");
    this.PreloadSound("VO_NAX3_01_HP_01.prefab:428343bce2fa95c42837e3f7fd220634");
    this.PreloadSound("VO_KT_MAEXXNA2_47.prefab:ba0b856a1b49d4249b511c6d2a7e5a66");
    this.PreloadSound("VO_KT_MAEXXNA6_51.prefab:105796bfd5d566249beef8e4c8672ee3");
    this.PreloadSound("VO_KT_MAEXXNA3_48.prefab:3bd05eb0fda073245bf190fc623a148c");
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
          m_soundName = "VO_NAX3_01_EMOTE_01.prefab:a31d7a46b65b449479a2a531268f9299",
          m_stringTag = "VO_NAX3_01_EMOTE_01"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX3_01_EMOTE_02.prefab:c1e3c5438c9b2a1469badb6ac78ed010",
          m_stringTag = "VO_NAX3_01_EMOTE_02"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX3_01_EMOTE_03.prefab:2d1d268f8503f0d4090ac41624711149",
          m_stringTag = "VO_NAX3_01_EMOTE_03"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX3_01_EMOTE_04.prefab:8163100101e44994680768b0f4220eec",
          m_stringTag = "VO_NAX3_01_EMOTE_04"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX3_01_EMOTE_05.prefab:0fc3e310650ab7149b5bf2b794895869",
          m_stringTag = "VO_NAX3_01_EMOTE_05"
        }
      }
    }
  };

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    if (turn == 1)
    {
      NotificationManager.Get().CreateKTQuote("VO_KT_MAEXXNA2_47", "VO_KT_MAEXXNA2_47.prefab:ba0b856a1b49d4249b511c6d2a7e5a66", false);
      yield break;
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_MAEXXNA4_49", "VO_KT_MAEXXNA4_49.prefab:449ab7cb30688e344896e51a9fc4dfd1");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX03_Maexxna naX03Maexxna = this;
    while (naX03Maexxna.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "NAX3_02"))
    {
      if (!(cardId == "NAX3_03"))
      {
        if (cardId == "EX1_586" && !naX03Maexxna.m_seaGiantLinePlayed)
        {
          naX03Maexxna.m_seaGiantLinePlayed = true;
          yield return (object) new WaitForSeconds(1f);
          while (NotificationManager.Get().IsQuotePlaying)
            yield return (object) 0;
          NotificationManager.Get().CreateKTQuote("VO_KT_MAEXXNA3_48", "VO_KT_MAEXXNA3_48.prefab:3bd05eb0fda073245bf190fc623a148c", false);
        }
      }
      else
        Gameplay.Get().StartCoroutine(naX03Maexxna.PlaySoundAndBlockSpeech("VO_NAX3_01_CARD_01.prefab:8f9084036cb9a31429b886ee01cc9bad", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      Gameplay.Get().StartCoroutine(naX03Maexxna.PlaySoundAndBlockSpeech("VO_NAX3_01_HP_01.prefab:428343bce2fa95c42837e3f7fd220634", Notification.SpeechBubbleDirection.TopRight, actor));
      if (!naX03Maexxna.m_heroPowerLinePlayed)
      {
        naX03Maexxna.m_heroPowerLinePlayed = true;
        while (naX03Maexxna.m_enemySpeaking || NotificationManager.Get().IsQuotePlaying)
          yield return (object) 0;
        NotificationManager.Get().CreateKTQuote("VO_KT_MAEXXNA6_51", "VO_KT_MAEXXNA6_51.prefab:105796bfd5d566249beef8e4c8672ee3", false);
      }
    }
  }
}
