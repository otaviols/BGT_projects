using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOE14_Steel_Sentinel : LOE_MissionEntity
{
  public override void PreloadAssets()
  {
    this.PreloadSound("VO_LOE_14_START.prefab:0c76369f23915fd4897d9aecf53de768");
    this.PreloadSound("VO_LOE_14_TURN_5.prefab:878704acc2ab01c419de69caa7003a51");
    this.PreloadSound("VO_LOE_14_TURN_5_2.prefab:9a031cba8a034dd4681717165ff5bfb1");
    this.PreloadSound("VO_LOE_14_TURN_9.prefab:e729be5d0472cd54faaabb324d197263");
    this.PreloadSound("VO_LOE_14_TURN_13.prefab:8b8685be9e263a84aa5b477f69e0dd82");
    this.PreloadSound("VO_LOE_14_WIN.prefab:5feb008ac9f05354991afc1a0308cf03");
    this.PreloadSound("LOEA14_1_SteelSentinel_Response.prefab:f18eaf8aeb86dd047a849548e80cbd39");
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
          m_soundName = "LOEA14_1_SteelSentinel_Response.prefab:f18eaf8aeb86dd047a849548e80cbd39",
          m_stringTag = "VO_LOE_14_RESPONSE"
        }
      }
    }
  };

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LOE14_Steel_Sentinel e14SteelSentinel = this;
    while (e14SteelSentinel.m_enemySpeaking)
      yield return (object) null;
    switch (turn)
    {
      case 1:
        yield return (object) Gameplay.Get().StartCoroutine(e14SteelSentinel.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote.prefab:a03dd286404083c439e371ba84d7a82b", "VO_LOE_14_START.prefab:0c76369f23915fd4897d9aecf53de768"));
        break;
      case 5:
        yield return (object) Gameplay.Get().StartCoroutine(e14SteelSentinel.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote.prefab:1c1c332cf5009194cb7dd7316c465aee", "VO_LOE_14_TURN_5.prefab:878704acc2ab01c419de69caa7003a51"));
        yield return (object) Gameplay.Get().StartCoroutine(e14SteelSentinel.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26", "VO_LOE_14_TURN_5_2.prefab:9a031cba8a034dd4681717165ff5bfb1"));
        break;
      case 9:
        yield return (object) Gameplay.Get().StartCoroutine(e14SteelSentinel.PlayBigCharacterQuoteAndWaitOnce("Rafaam_wrap_BigQuote.prefab:ee7dbbb027adc1947b64b05f31d4c124", "VO_LOE_14_TURN_9.prefab:e729be5d0472cd54faaabb324d197263"));
        break;
      case 13:
        yield return (object) Gameplay.Get().StartCoroutine(e14SteelSentinel.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26", "VO_LOE_14_TURN_13.prefab:8b8685be9e263a84aa5b477f69e0dd82"));
        break;
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    LOE14_Steel_Sentinel e14SteelSentinel = this;
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) Gameplay.Get().StartCoroutine(e14SteelSentinel.PlayCharacterQuoteAndWait("Cartographer_Quote.prefab:c6056bfb8c0025a458553adabc8ed537", "VO_LOE_14_WIN.prefab:5feb008ac9f05354991afc1a0308cf03", allowRepeatDuringSession: false));
    }
  }
}
