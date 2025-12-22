using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX13_Thaddius : NAX_MissionEntity
{
  private bool m_heroPowerLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX13_01_HP_02.prefab:cc9afdc24fabea54abc939924c34c7f8");
    this.PreloadSound("VO_NAX13_01_EMOTE_04.prefab:337ef024b2d71e84393f6da891bf83cc");
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
          m_soundName = "VO_NAX13_01_EMOTE_04.prefab:337ef024b2d71e84393f6da891bf83cc",
          m_stringTag = "VO_NAX13_01_EMOTE_04"
        }
      }
    }
  };

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_THADDIUS2_81", "VO_KT_THADDIUS2_81.prefab:47685f2ff524d944f90a9cb87b8e9861");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX13_Thaddius naX13Thaddius = this;
    while (naX13Thaddius.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (entity.GetCardId() == "NAX13_02" && !naX13Thaddius.m_heroPowerLinePlayed)
    {
      naX13Thaddius.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(naX13Thaddius.PlaySoundAndBlockSpeech("VO_NAX13_01_HP_02.prefab:cc9afdc24fabea54abc939924c34c7f8", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }
}
