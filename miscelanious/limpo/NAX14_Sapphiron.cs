using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX14_Sapphiron : NAX_MissionEntity
{
  private bool m_cardKtLinePlayed;
  private int m_numTimesFrostBreathMisses;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX14_01_EMOTE_01.prefab:8e779dac945dafe4cae350690bfe5adb");
    this.PreloadSound("VO_NAX14_01_EMOTE_02.prefab:2062a07b94954c44aab47bc4edc1c307");
    this.PreloadSound("VO_NAX14_01_EMOTE_03.prefab:225909a5368dd56489a130eb22afbd19");
    this.PreloadSound("VO_NAX14_01_CARD_01.prefab:349cacac917de9f49ad5a75f1352e53c");
    this.PreloadSound("VO_NAX14_01_HP_01.prefab:641e44497675c5b4497a0be59dcec408");
    this.PreloadSound("VO_KT_SAPPHIRON2_84.prefab:c2f8a9371bca45441b0d069994e2fc96");
    this.PreloadSound("VO_KT_SAPPHIRON3_85.prefab:0d71aa21bf415bb448a90c1d87f73a82");
    this.PreloadSound("VO_KT_SAPPHIRON4_ALT_87.prefab:58b23de92708c8146b0f278db588c7d7");
    this.PreloadSound("VO_KT_SAPPHIRON5_88.prefab:92e68b9f36ba81e4d94e20135a842f0d");
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
          m_soundName = "VO_NAX14_01_EMOTE_01.prefab:8e779dac945dafe4cae350690bfe5adb",
          m_stringTag = "VO_NAX14_01_EMOTE_01"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX14_01_EMOTE_02.prefab:2062a07b94954c44aab47bc4edc1c307",
          m_stringTag = "VO_NAX14_01_EMOTE_02"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX14_01_EMOTE_03.prefab:225909a5368dd56489a130eb22afbd19",
          m_stringTag = "VO_NAX14_01_EMOTE_03"
        }
      }
    }
  };

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    if (turn == 1 && GameState.Get().GetOpposingSidePlayer().GetHero().GetCardId() != "NAX14_01H")
    {
      NotificationManager.Get().CreateKTQuote("VO_KT_SAPPHIRON2_84", "VO_KT_SAPPHIRON2_84.prefab:c2f8a9371bca45441b0d069994e2fc96", false);
      yield break;
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_SAPPHIRON5_88", "VO_KT_SAPPHIRON5_88.prefab:92e68b9f36ba81e4d94e20135a842f0d");
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    yield return (object) Gameplay.Get().StartCoroutine(base.HandleMissionEventWithTiming(missionEvent));
    if (missionEvent == 1)
    {
      ++this.m_numTimesFrostBreathMisses;
      if (this.m_numTimesFrostBreathMisses == 4)
        NotificationManager.Get().CreateKTQuote("VO_KT_SAPPHIRON3_85", "VO_KT_SAPPHIRON3_85.prefab:0d71aa21bf415bb448a90c1d87f73a82");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX14_Sapphiron naX14Sapphiron = this;
    while (naX14Sapphiron.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "NAX14_02"))
    {
      if (cardId == "NAX14_04")
      {
        yield return (object) new WaitForSeconds(1f);
        if (naX14Sapphiron.m_cardKtLinePlayed)
        {
          Gameplay.Get().StartCoroutine(naX14Sapphiron.PlaySoundAndBlockSpeech("VO_NAX14_01_CARD_01.prefab:349cacac917de9f49ad5a75f1352e53c", Notification.SpeechBubbleDirection.TopRight, enemyActor));
        }
        else
        {
          NotificationManager.Get().CreateKTQuote("VO_KT_SAPPHIRON4_ALT_87", "VO_KT_SAPPHIRON4_ALT_87.prefab:58b23de92708c8146b0f278db588c7d7", false);
          naX14Sapphiron.m_cardKtLinePlayed = true;
        }
      }
    }
    else
      Gameplay.Get().StartCoroutine(naX14Sapphiron.PlaySoundAndBlockSpeech("VO_NAX14_01_HP_01.prefab:641e44497675c5b4497a0be59dcec408", Notification.SpeechBubbleDirection.TopRight, enemyActor));
  }
}
