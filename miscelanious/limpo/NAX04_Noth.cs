using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX04_Noth : NAX_MissionEntity
{
  private bool m_cardLinePlayed;
  private bool m_heroPowerLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX4_01_HP_02.prefab:ef429c4ce7a413d4fa8ce390025bd388");
    this.PreloadSound("VO_NAX4_01_CARD_03.prefab:676865be5229fbb4ea8b71bb7570b6f2");
    this.PreloadSound("VO_NAX4_01_EMOTE_06.prefab:837b9665fb7727145966a74e0610ee05");
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
          m_soundName = "VO_NAX4_01_EMOTE_06.prefab:837b9665fb7727145966a74e0610ee05",
          m_stringTag = "VO_NAX4_01_EMOTE_06"
        }
      }
    }
  };

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_NOTH2_53", "VO_KT_NOTH2_53.prefab:0ac170c747ea31f4182b1abce130228b");
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    NAX04_Noth naX04Noth = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) Gameplay.Get().StartCoroutine(naX04Noth.\u003C\u003En__0(missionEvent));
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (missionEvent == 1 && !naX04Noth.m_heroPowerLinePlayed)
    {
      naX04Noth.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(naX04Noth.PlaySoundAndBlockSpeech("VO_NAX4_01_HP_02.prefab:ef429c4ce7a413d4fa8ce390025bd388", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX04_Noth naX04Noth = this;
    while (naX04Noth.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (entity.GetCardId() == "NAX4_05" && !naX04Noth.m_cardLinePlayed)
    {
      naX04Noth.m_cardLinePlayed = true;
      Gameplay.Get().StartCoroutine(naX04Noth.PlaySoundAndBlockSpeech("VO_NAX4_01_CARD_03.prefab:676865be5229fbb4ea8b71bb7570b6f2", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }
}
