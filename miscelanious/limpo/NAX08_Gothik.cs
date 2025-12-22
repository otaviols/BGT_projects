using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX08_Gothik : NAX_MissionEntity
{
  private bool m_cardLinePlayed;
  private bool m_unrelentingMinionLinePlayed;
  private bool m_deadReturnLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX8_01_CARD_02.prefab:39a2051167989ad48a234c7bfcf6bb0b");
    this.PreloadSound("VO_NAX8_01_CUSTOM_03.prefab:4e1adb5a87d8efd45ba4fab32ba9dff1");
    this.PreloadSound("VO_NAX8_01_EMOTE1_06.prefab:d000156f1cd9f7d4d816d660fc74caa0");
    this.PreloadSound("VO_NAX8_01_EMOTE2_07.prefab:61420eb86b3febb4da7077c26b66fe82");
    this.PreloadSound("VO_NAX8_01_EMOTE3_08.prefab:e54c3c21bf16794418b8d684af0d13ea");
    this.PreloadSound("VO_NAX8_01_EMOTE4_09.prefab:d9acf9ba48ac4324e96a0c2ec232545b");
    this.PreloadSound("VO_NAX8_01_EMOTE5_10.prefab:d6d0522885dbd074b9d554dd47fddbb5");
    this.PreloadSound("VO_NAX8_01_CUSTOM2_04.prefab:10c0d0ad330cfb44eb7faa0413cd2e23");
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
          m_soundName = "VO_NAX8_01_EMOTE1_06.prefab:d000156f1cd9f7d4d816d660fc74caa0",
          m_stringTag = "VO_NAX8_01_EMOTE1_06"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX8_01_EMOTE2_07.prefab:61420eb86b3febb4da7077c26b66fe82",
          m_stringTag = "VO_NAX8_01_EMOTE2_07"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX8_01_EMOTE3_08.prefab:e54c3c21bf16794418b8d684af0d13ea",
          m_stringTag = "VO_NAX8_01_EMOTE3_08"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX8_01_EMOTE4_09.prefab:d9acf9ba48ac4324e96a0c2ec232545b",
          m_stringTag = "VO_NAX8_01_EMOTE4_09"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX8_01_EMOTE5_10.prefab:d6d0522885dbd074b9d554dd47fddbb5",
          m_stringTag = "VO_NAX8_01_EMOTE5_10"
        }
      }
    }
  };

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_GOTHIK2_62", "VO_KT_GOTHIK2_62.prefab:0ac7f3dd8ea055b4c81abd7f25e3f782");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX08_Gothik naX08Gothik = this;
    while (naX08Gothik.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "NAX8_03") && !(cardId == "NAX8_04") && !(cardId == "NAX8_05"))
    {
      if (cardId == "NAX8_02" && !naX08Gothik.m_cardLinePlayed)
      {
        naX08Gothik.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(naX08Gothik.PlaySoundAndBlockSpeech("VO_NAX8_01_CUSTOM_03.prefab:4e1adb5a87d8efd45ba4fab32ba9dff1", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!naX08Gothik.m_unrelentingMinionLinePlayed)
    {
      naX08Gothik.m_unrelentingMinionLinePlayed = true;
      Gameplay.Get().StartCoroutine(naX08Gothik.PlaySoundAndBlockSpeech("VO_NAX8_01_CARD_02.prefab:39a2051167989ad48a234c7bfcf6bb0b", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    NAX08_Gothik naX08Gothik = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) Gameplay.Get().StartCoroutine(naX08Gothik.\u003C\u003En__0(missionEvent));
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 1 && !naX08Gothik.m_deadReturnLinePlayed)
    {
      naX08Gothik.m_deadReturnLinePlayed = true;
      Gameplay.Get().StartCoroutine(naX08Gothik.PlaySoundAndBlockSpeech("VO_NAX8_01_CUSTOM2_04.prefab:10c0d0ad330cfb44eb7faa0413cd2e23", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }
}
