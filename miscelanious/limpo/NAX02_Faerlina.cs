using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX02_Faerlina : NAX_MissionEntity
{
  private bool m_cardLinePlayed;
  private bool m_heroPowerLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX2_01_HP_04.prefab:c2607f2df2849a3478f64624ed464f45");
    this.PreloadSound("VO_NAX2_01_CARD_02.prefab:462f4054a3d870d44bffda29568fb42f");
    this.PreloadSound("VO_NAX2_01_EMOTE_06.prefab:67f66bba9b0da65448b6a54b6206a4a8");
    this.PreloadSound("VO_NAX2_01_CUSTOM_03.prefab:87a19007902dc794e88dd8c21dd1babe");
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
          m_soundName = "VO_NAX2_01_EMOTE_06.prefab:67f66bba9b0da65448b6a54b6206a4a8",
          m_stringTag = "VO_NAX2_01_EMOTE_06"
        }
      }
    }
  };

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_FAERLINA2_45", "VO_KT_FAERLINA2_45.prefab:d6270f942c734014d998b58c40f9a22d");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX02_Faerlina naX02Faerlina = this;
    while (naX02Faerlina.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "NAX2_03"))
    {
      if ((cardId == "NAX2_05" || cardId == "NAX2_05H") && !naX02Faerlina.m_cardLinePlayed)
      {
        naX02Faerlina.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(naX02Faerlina.PlaySoundAndBlockSpeech("VO_NAX2_01_CARD_02.prefab:462f4054a3d870d44bffda29568fb42f", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!naX02Faerlina.m_heroPowerLinePlayed)
    {
      naX02Faerlina.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(naX02Faerlina.PlaySoundAndBlockSpeech("VO_NAX2_01_HP_04.prefab:c2607f2df2849a3478f64624ed464f45", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    NAX02_Faerlina naX02Faerlina = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) Gameplay.Get().StartCoroutine(naX02Faerlina.\u003C\u003En__0(missionEvent));
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (missionEvent == 1)
      Gameplay.Get().StartCoroutine(naX02Faerlina.PlaySoundAndBlockSpeech("VO_NAX2_01_CUSTOM_03.prefab:87a19007902dc794e88dd8c21dd1babe", Notification.SpeechBubbleDirection.TopRight, actor));
  }
}
