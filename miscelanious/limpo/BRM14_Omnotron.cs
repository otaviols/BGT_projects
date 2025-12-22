using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRM14_Omnotron : BRM_MissionEntity
{
  private bool m_heroPower1LinePlayed;
  private bool m_heroPower2LinePlayed;
  private bool m_heroPower3LinePlayed;
  private bool m_heroPower4LinePlayed;
  private bool m_heroPower5LinePlayed;
  private bool m_cardLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA14_1_RESPONSE1_10.prefab:6f97c57440f1e5a4b8bac49eb39f97a8");
    this.PreloadSound("VO_BRMA14_1_RESPONSE2_11.prefab:c213dd29b35274b41b5c4c982171be04");
    this.PreloadSound("VO_BRMA14_1_RESPONSE3_12.prefab:a4c9a33c03b2cf143baae5829af69701");
    this.PreloadSound("VO_BRMA14_1_RESPONSE4_13.prefab:1807877b8c9538246a11d69fa003e369");
    this.PreloadSound("VO_BRMA14_1_RESPONSE5_14.prefab:23cb6b0e25ba78345a67a15416600774");
    this.PreloadSound("VO_BRMA14_1_HP1_03.prefab:49a86de40b9af024ea935f62b98017b3");
    this.PreloadSound("VO_BRMA14_1_HP2_04.prefab:a3d3b1b54ba372a49b3320f600da1b0f");
    this.PreloadSound("VO_BRMA14_1_HP3_05.prefab:0f20a4c1fdb6d9143b9d10b617c8c901");
    this.PreloadSound("VO_BRMA14_1_HP4_06.prefab:481cbb26507bdf24eba7d240ae131496");
    this.PreloadSound("VO_BRMA14_1_HP5_07.prefab:99da345a6ef8bbb46b9ebf4e729b34c8");
    this.PreloadSound("VO_BRMA14_1_CARD_09.prefab:40a09cf3c2c8a334da8195687dfa6dc1");
    this.PreloadSound("VO_BRMA14_1_TURN1_02.prefab:32791f2d54bc6c4478f8d9f28720da20");
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
          m_soundName = "VO_BRMA14_1_RESPONSE1_10.prefab:6f97c57440f1e5a4b8bac49eb39f97a8",
          m_stringTag = "VO_BRMA14_1_RESPONSE1_10"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_BRMA14_1_RESPONSE2_11.prefab:c213dd29b35274b41b5c4c982171be04",
          m_stringTag = "VO_BRMA14_1_RESPONSE2_11"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_BRMA14_1_RESPONSE3_12.prefab:a4c9a33c03b2cf143baae5829af69701",
          m_stringTag = "VO_BRMA14_1_RESPONSE3_12"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_BRMA14_1_RESPONSE4_13.prefab:1807877b8c9538246a11d69fa003e369",
          m_stringTag = "VO_BRMA14_1_RESPONSE4_13"
        },
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_BRMA14_1_RESPONSE5_14.prefab:23cb6b0e25ba78345a67a15416600774",
          m_stringTag = "VO_BRMA14_1_RESPONSE5_14"
        }
      }
    }
  };

  protected override void CycleNextResponseGroupIndex(MissionEntity.EmoteResponseGroup responseGroup)
  {
    if (responseGroup.m_responseIndex == responseGroup.m_responses.Count - 1)
      return;
    ++responseGroup.m_responseIndex;
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BRM14_Omnotron brM14Omnotron = this;
    while (brM14Omnotron.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    switch (entity.GetCardId())
    {
      case "BRMA14_10":
      case "BRMA14_10H":
        if (brM14Omnotron.m_heroPower5LinePlayed)
          break;
        brM14Omnotron.m_heroPower5LinePlayed = true;
        Gameplay.Get().StartCoroutine(brM14Omnotron.PlaySoundAndBlockSpeech("VO_BRMA14_1_HP5_07.prefab:99da345a6ef8bbb46b9ebf4e729b34c8", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case "BRMA14_11":
        if (brM14Omnotron.m_cardLinePlayed)
          break;
        brM14Omnotron.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(brM14Omnotron.PlaySoundAndBlockSpeech("VO_BRMA14_1_CARD_09.prefab:40a09cf3c2c8a334da8195687dfa6dc1", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case "BRMA14_2":
      case "BRMA14_2H":
        if (brM14Omnotron.m_heroPower3LinePlayed)
          break;
        brM14Omnotron.m_heroPower3LinePlayed = true;
        Gameplay.Get().StartCoroutine(brM14Omnotron.PlaySoundAndBlockSpeech("VO_BRMA14_1_HP3_05.prefab:0f20a4c1fdb6d9143b9d10b617c8c901", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case "BRMA14_4":
      case "BRMA14_4H":
        if (brM14Omnotron.m_heroPower2LinePlayed)
          break;
        brM14Omnotron.m_heroPower2LinePlayed = true;
        Gameplay.Get().StartCoroutine(brM14Omnotron.PlaySoundAndBlockSpeech("VO_BRMA14_1_HP2_04.prefab:a3d3b1b54ba372a49b3320f600da1b0f", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case "BRMA14_6":
      case "BRMA14_6H":
        if (brM14Omnotron.m_heroPower1LinePlayed)
          break;
        brM14Omnotron.m_heroPower1LinePlayed = true;
        Gameplay.Get().StartCoroutine(brM14Omnotron.PlaySoundAndBlockSpeech("VO_BRMA14_1_HP1_03.prefab:49a86de40b9af024ea935f62b98017b3", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case "BRMA14_8":
      case "BRMA14_8H":
        if (brM14Omnotron.m_heroPower4LinePlayed)
          break;
        brM14Omnotron.m_heroPower4LinePlayed = true;
        Gameplay.Get().StartCoroutine(brM14Omnotron.PlaySoundAndBlockSpeech("VO_BRMA14_1_HP4_06.prefab:481cbb26507bdf24eba7d240ae131496", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BRM14_Omnotron brM14Omnotron = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (turn == 1)
      Gameplay.Get().StartCoroutine(brM14Omnotron.PlaySoundAndBlockSpeech("VO_BRMA14_1_TURN1_02.prefab:32791f2d54bc6c4478f8d9f28720da20", Notification.SpeechBubbleDirection.TopRight, actor));
    return false;
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateCharacterQuote("NefarianDragon_Quote.prefab:179fec888df7e4c02b8de3b7ad109a23", GameStrings.Get("VO_NEFARIAN_OMNOTRON_DEAD_69"), "VO_NEFARIAN_OMNOTRON_DEAD_69.prefab:85ce5224ca43db3409797b209723d087");
    }
  }
}
