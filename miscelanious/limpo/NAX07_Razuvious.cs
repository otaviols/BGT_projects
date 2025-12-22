using PegasusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX07_Razuvious : NAX_MissionEntity
{
  private bool m_heroPowerLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX7_01_HP_02.prefab:cb3aadc3fbe355e40bbd5463f09ffdf8");
    this.PreloadSound("VO_NAX7_01_START_01.prefab:3fc94f039bccb2d4ca0e0a242b2f955e");
    this.PreloadSound("VO_NAX7_01_EMOTE_05.prefab:a116dabbfbb825e4cb519d18a2c21779");
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
          m_soundName = "VO_NAX7_01_EMOTE_05.prefab:a116dabbfbb825e4cb519d18a2c21779",
          m_stringTag = "VO_NAX7_01_EMOTE_05"
        }
      }
    }
  };

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_RAZUVIOUS2_59", "VO_KT_RAZUVIOUS2_59.prefab:58901b0d8c4e834489caca72c1fb5ecc");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX07_Razuvious naX07Razuvious = this;
    while (naX07Razuvious.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (entity.GetCardId() == "NAX7_03" && !naX07Razuvious.m_heroPowerLinePlayed)
    {
      naX07Razuvious.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(naX07Razuvious.PlaySoundAndBlockSpeech("VO_NAX7_01_HP_02.prefab:cb3aadc3fbe355e40bbd5463f09ffdf8", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    NAX07_Razuvious naX07Razuvious = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) Gameplay.Get().StartCoroutine(naX07Razuvious.\u003C\u003En__0(missionEvent));
    if (missionEvent == 1)
    {
      bool flag = false;
      PowerTaskList currentTaskList = GameState.Get().GetPowerProcessor().GetCurrentTaskList();
      Entity entity1 = currentTaskList == null ? (Entity) null : currentTaskList.GetSourceEntity();
      if (entity1 != null && entity1.GetCardId() == "NAX7_05")
      {
        foreach (PowerTask task in currentTaskList.GetTaskList())
        {
          Network.PowerHistory power = task.GetPower();
          if (power.Type == Network.PowerType.META_DATA)
          {
            Network.HistMetaData histMetaData = power as Network.HistMetaData;
            if (histMetaData.MetaType == HistoryMeta.Type.TARGET && histMetaData.Info != null && histMetaData.Info.Count != 0)
            {
              for (int index = 0; index < histMetaData.Info.Count; ++index)
              {
                Entity entity2 = GameState.Get().GetEntity(histMetaData.Info[index]);
                if (entity2 != null && entity2.GetCardId() == "NAX7_02")
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
                break;
            }
          }
        }
      }
      if (flag)
      {
        Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
        Gameplay.Get().StartCoroutine(naX07Razuvious.PlaySoundAndBlockSpeech("VO_NAX7_01_START_01.prefab:3fc94f039bccb2d4ca0e0a242b2f955e", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
  }
}
