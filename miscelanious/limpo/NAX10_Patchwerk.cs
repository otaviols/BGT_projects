using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX10_Patchwerk : NAX_MissionEntity
{
  private bool m_heroPowerLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX10_01_HP_02.prefab:b7fff1c198650934c8b2902ddd5bcdbf");
    this.PreloadSound("VO_NAX10_01_EMOTE2_05.prefab:905bdb461afbd5244954f25829e1b99b");
    this.PreloadSound("VO_NAX10_01_EMOTE1_04.prefab:c5b2fff109f63134597f357b4b67c0b9");
  }

  protected override void InitEmoteResponses() => this.m_emoteResponseGroups = new List<MissionEntity.EmoteResponseGroup>()
  {
    new MissionEntity.EmoteResponseGroup()
    {
      m_triggers = new List<EmoteType>()
      {
        EmoteType.GREETINGS,
        EmoteType.OOPS,
        EmoteType.SORRY,
        EmoteType.THANKS,
        EmoteType.THREATEN
      },
      m_responses = new List<MissionEntity.EmoteResponse>()
      {
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX10_01_EMOTE1_04.prefab:c5b2fff109f63134597f357b4b67c0b9",
          m_stringTag = "VO_NAX10_01_EMOTE1_04"
        }
      }
    },
    new MissionEntity.EmoteResponseGroup()
    {
      m_triggers = new List<EmoteType>()
      {
        EmoteType.WELL_PLAYED
      },
      m_responses = new List<MissionEntity.EmoteResponse>()
      {
        new MissionEntity.EmoteResponse()
        {
          m_soundName = "VO_NAX10_01_EMOTE2_05.prefab:905bdb461afbd5244954f25829e1b99b",
          m_stringTag = "VO_NAX10_01_EMOTE2_05"
        }
      }
    }
  };

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_PATCHWERK2_69", "VO_KT_PATCHWERK2_69.prefab:b11d9d854c9a8414693838d75f455f21");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX10_Patchwerk naX10Patchwerk = this;
    while (naX10Patchwerk.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (entity.GetCardId() == "NAX10_03" && !naX10Patchwerk.m_heroPowerLinePlayed)
    {
      naX10Patchwerk.m_heroPowerLinePlayed = true;
      yield return (object) new WaitForSeconds(4.5f);
      Gameplay.Get().StartCoroutine(naX10Patchwerk.PlaySoundAndBlockSpeech("VO_NAX10_01_HP_02.prefab:b7fff1c198650934c8b2902ddd5bcdbf", Notification.SpeechBubbleDirection.TopRight, enemyActor));
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    if (turn % 2 != 0)
    {
      GameState.Get().SetBusy(true);
      yield return (object) new WaitForSeconds(1f);
      GameState.Get().SetBusy(false);
    }
  }
}
