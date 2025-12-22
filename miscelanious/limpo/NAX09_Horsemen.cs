using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NAX09_Horsemen : NAX_MissionEntity
{
  private bool m_heroPowerLinePlayed;
  private bool m_cardLinePlayed;
  private bool m_introSequenceComplete;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_NAX9_01_CUSTOM_02.prefab:aabdff7ec08cc1f44a7c8c391c744e2f");
    this.PreloadSound("VO_NAX9_01_EMOTE_04.prefab:1d1eb70ed25d60c429b27471ec10b191");
    this.PreloadSound("VO_FP1_031_EnterPlay_06.prefab:51754c9428cdf374882cb4020bbd5627");
    this.PreloadSound("VO_NAX9_02_CUSTOM_01.prefab:520d0daa9374bfa47ab3f380f0e1ef65");
    this.PreloadSound("VO_NAX9_03_CUSTOM_01.prefab:4fb7d8593f95c404f97ddd63c29e939c");
    this.PreloadSound("VO_NAX9_04_CUSTOM_01.prefab:9581debb360b7dd478f7ddfeeda6768e");
    this.PreloadSound("VO_FP1_031_Attack_07.prefab:b4c323c69c7f5cf418ec6b228b188c5d");
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
          m_soundName = "VO_NAX9_01_EMOTE_04.prefab:1d1eb70ed25d60c429b27471ec10b191",
          m_stringTag = "VO_NAX9_01_EMOTE_04"
        }
      }
    }
  };

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateKTQuote("VO_KT_BARON2_64", "VO_KT_BARON2_64.prefab:485607a6e18abc9458ba36d2b952d403");
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    NAX09_Horsemen naX09Horsemen = this;
    Actor baronActor;
    Actor blaumeuxActor;
    Actor thaneActor;
    if (turn == 1)
    {
      baronActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      blaumeuxActor = (Actor) null;
      thaneActor = (Actor) null;
      Actor actor = (Actor) null;
      foreach (Card card in GameState.Get().GetOpposingSidePlayer().GetBattlefieldZone().GetCards())
      {
        string cardId = card.GetEntity().GetCardId();
        if (cardId == "NAX9_02")
          blaumeuxActor = card.GetActor();
        else if (cardId == "NAX9_03")
          thaneActor = card.GetActor();
        else if (cardId == "NAX9_04")
          actor = card.GetActor();
      }
      if ((Object) actor == (Object) null)
      {
        naX09Horsemen.m_introSequenceComplete = true;
        yield break;
      }
      else
      {
        yield return (object) Gameplay.Get().StartCoroutine(naX09Horsemen.PlaySoundAndBlockSpeech("VO_NAX9_02_CUSTOM_01.prefab:520d0daa9374bfa47ab3f380f0e1ef65", Notification.SpeechBubbleDirection.TopRight, actor));
        if ((Object) blaumeuxActor != (Object) null)
          yield return (object) Gameplay.Get().StartCoroutine(naX09Horsemen.PlaySoundAndBlockSpeech("VO_NAX9_03_CUSTOM_01.prefab:4fb7d8593f95c404f97ddd63c29e939c", Notification.SpeechBubbleDirection.TopRight, blaumeuxActor));
        if ((Object) baronActor != (Object) null)
          yield return (object) Gameplay.Get().StartCoroutine(naX09Horsemen.PlaySoundAndBlockSpeech("VO_NAX9_01_CUSTOM_02.prefab:aabdff7ec08cc1f44a7c8c391c744e2f", Notification.SpeechBubbleDirection.TopRight, baronActor));
        if ((Object) thaneActor != (Object) null)
          yield return (object) Gameplay.Get().StartCoroutine(naX09Horsemen.PlaySoundAndBlockSpeech("VO_NAX9_04_CUSTOM_01.prefab:9581debb360b7dd478f7ddfeeda6768e", Notification.SpeechBubbleDirection.TopRight, thaneActor));
        naX09Horsemen.m_introSequenceComplete = true;
      }
    }
    baronActor = (Actor) null;
    blaumeuxActor = (Actor) null;
    thaneActor = (Actor) null;
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    NAX09_Horsemen naX09Horsemen = this;
    if (naX09Horsemen.m_introSequenceComplete)
    {
      while (naX09Horsemen.m_enemySpeaking)
        yield return (object) null;
      while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
        yield return (object) null;
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      string cardId = entity.GetCardId();
      if (!(cardId == "NAX9_06"))
      {
        if (cardId == "NAX9_07" && !naX09Horsemen.m_cardLinePlayed)
        {
          naX09Horsemen.m_cardLinePlayed = true;
          Gameplay.Get().StartCoroutine(naX09Horsemen.PlaySoundAndBlockSpeech("VO_FP1_031_Attack_07.prefab:b4c323c69c7f5cf418ec6b228b188c5d", Notification.SpeechBubbleDirection.TopRight, actor));
        }
      }
      else if (!naX09Horsemen.m_heroPowerLinePlayed)
      {
        naX09Horsemen.m_heroPowerLinePlayed = true;
        Gameplay.Get().StartCoroutine(naX09Horsemen.PlaySoundAndBlockSpeech("VO_FP1_031_EnterPlay_06.prefab:51754c9428cdf374882cb4020bbd5627", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
  }
}
