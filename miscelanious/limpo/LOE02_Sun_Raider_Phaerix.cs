using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOE02_Sun_Raider_Phaerix : LOE_MissionEntity
{
  private int m_staffLinesPlayed;
  private bool m_damageLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_LOE_01_RESPONSE.prefab:003ddb96a133c634b8f74c8a9ef1e55c");
    this.PreloadSound("VO_LOE_01_WOUNDED.prefab:0fb9c01bbbbacd0408fb478d13b9574b");
    this.PreloadSound("VO_LOE_01_STAFF.prefab:b412b19ab6e0def45a74aeb7ebb60ec1");
    this.PreloadSound("VO_LOE_01_STAFF_2.prefab:587ffb164487ac0429b1dac0ca33b9aa");
    this.PreloadSound("VO_LOE_02_PHAERIX_STAFF_RECOVER.prefab:bdbf17959a28fa247976168e5d545f5d");
    this.PreloadSound("VO_LOE_01_STAFF_2_RENO.prefab:81e1aae8f257ed448bcdbd89ea881fc5");
    this.PreloadSound("VO_LOE_01_WIN_2.prefab:e8acaf5e90b7f7a419739aba63b6f8bc");
    this.PreloadSound("VO_LOE_01_WIN_2_ALT_2.prefab:4d3c656d8071a8e438cd2fc2f4d5b862");
    this.PreloadSound("VO_LOE_01_START.prefab:e4840e7e825b2aa438f783b50bb19584");
    this.PreloadSound("VO_LOE_01_WIN.prefab:ece5a717571dc164db373500aed7a707");
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
          m_soundName = "VO_LOE_01_RESPONSE.prefab:003ddb96a133c634b8f74c8a9ef1e55c",
          m_stringTag = "VO_LOE_01_RESPONSE"
        }
      }
    }
  };

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOE02_Sun_Raider_Phaerix sunRaiderPhaerix = this;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (sunRaiderPhaerix.m_staffLinesPlayed < missionEvent)
    {
      if (missionEvent > 9)
      {
        if (!sunRaiderPhaerix.m_damageLinePlayed)
        {
          sunRaiderPhaerix.m_damageLinePlayed = true;
          yield return (object) Gameplay.Get().StartCoroutine(sunRaiderPhaerix.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_01_WOUNDED.prefab:0fb9c01bbbbacd0408fb478d13b9574b"));
        }
      }
      else
      {
        sunRaiderPhaerix.m_staffLinesPlayed = missionEvent;
        switch (missionEvent)
        {
          case 1:
            GameState.Get().SetBusy(true);
            yield return (object) Gameplay.Get().StartCoroutine(sunRaiderPhaerix.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_01_STAFF.prefab:b412b19ab6e0def45a74aeb7ebb60ec1"));
            Gameplay.Get().StartCoroutine(sunRaiderPhaerix.PlaySoundAndBlockSpeechOnce("VO_LOE_01_STAFF_2.prefab:587ffb164487ac0429b1dac0ca33b9aa", Notification.SpeechBubbleDirection.TopRight, enemyActor));
            GameState.Get().SetBusy(false);
            break;
          case 2:
            GameState.Get().SetBusy(true);
            Gameplay.Get().StartCoroutine(sunRaiderPhaerix.PlaySoundAndBlockSpeechOnce("VO_LOE_02_PHAERIX_STAFF_RECOVER.prefab:bdbf17959a28fa247976168e5d545f5d", Notification.SpeechBubbleDirection.TopRight, enemyActor));
            GameState.Get().SetBusy(false);
            break;
          case 3:
            GameState.Get().SetBusy(true);
            yield return (object) Gameplay.Get().StartCoroutine(sunRaiderPhaerix.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_01_STAFF_2_RENO.prefab:81e1aae8f257ed448bcdbd89ea881fc5"));
            GameState.Get().SetBusy(false);
            break;
        }
      }
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LOE02_Sun_Raider_Phaerix sunRaiderPhaerix = this;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (turn == 1)
    {
      Gameplay.Get().StartCoroutine(sunRaiderPhaerix.PlaySoundAndBlockSpeech("VO_LOE_01_START.prefab:e4840e7e825b2aa438f783b50bb19584", Notification.SpeechBubbleDirection.TopRight, actor));
      yield return (object) new WaitForSeconds(4f);
      yield return (object) Gameplay.Get().StartCoroutine(sunRaiderPhaerix.PlayBigCharacterQuoteAndWait("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_01_WIN_2.prefab:e8acaf5e90b7f7a419739aba63b6f8bc"));
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    LOE02_Sun_Raider_Phaerix sunRaiderPhaerix = this;
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) Gameplay.Get().StartCoroutine(sunRaiderPhaerix.PlayCharacterQuoteAndWait("Reno_Quote.prefab:0a2e34fa6782a0747b4f5d5574d1331a", "VO_LOE_01_WIN.prefab:ece5a717571dc164db373500aed7a707", allowRepeatDuringSession: false));
      yield return (object) Gameplay.Get().StartCoroutine(sunRaiderPhaerix.PlayCharacterQuoteAndWait("Reno_Quote.prefab:0a2e34fa6782a0747b4f5d5574d1331a", "VO_LOE_01_WIN_2_ALT_2.prefab:4d3c656d8071a8e438cd2fc2f4d5b862", allowRepeatDuringSession: false));
    }
  }
}
