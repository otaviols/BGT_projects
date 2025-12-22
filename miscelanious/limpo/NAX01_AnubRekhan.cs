using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX01_AnubRekhan : NAX_MissionEntity
{
  private bool m_locustSwarmLinePlayed;
  private bool m_heroPowerLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX1_01_HP_03.prefab:acb19fa6cb04b424b950583d2e71293e");
    this.PreloadSound("VO_NAX1_01_CARD_02.prefab:ed8f928f8289afd4b964eea03bf348fa");
    this.PreloadSound("VO_NAX1_01_EMOTE_04.prefab:ab367aeaf987d9a43ba10d474fa1e792");
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
          m_soundName = "VO_NAX1_01_EMOTE_04.prefab:ab367aeaf987d9a43ba10d474fa1e792",
          m_stringTag = "VO_NAX1_01_EMOTE_04"
        }
      }
    }
  };

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_ANUB2_43", "VO_KT_ANUB2_43.prefab:243d9a81662680b4dae85648d8f38f54");
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX01_AnubRekhan naX01AnubRekhan = this;
    while (naX01AnubRekhan.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "NAX1_04"))
    {
      if (cardId == "NAX1_05" && !naX01AnubRekhan.m_locustSwarmLinePlayed)
      {
        naX01AnubRekhan.m_locustSwarmLinePlayed = true;
        Gameplay.Get().StartCoroutine(naX01AnubRekhan.PlaySoundAndBlockSpeech("VO_NAX1_01_CARD_02.prefab:ed8f928f8289afd4b964eea03bf348fa", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!naX01AnubRekhan.m_heroPowerLinePlayed)
    {
      naX01AnubRekhan.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(naX01AnubRekhan.PlaySoundAndBlockSpeech("VO_NAX1_01_HP_03.prefab:acb19fa6cb04b424b950583d2e71293e", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }
}
