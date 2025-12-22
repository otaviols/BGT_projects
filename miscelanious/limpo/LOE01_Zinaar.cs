using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOE01_Zinaar : LOE_MissionEntity
{
  private bool m_wishMoreWishesLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_LOE_02_RESPONSE.prefab:81df49b8799ffe7408d1ca6d13a0b1a9");
    this.PreloadSound("VO_LOE_02_WISH.prefab:e58bbad31e7b0e944a7c5ae8c67a6837");
    this.PreloadSound("VO_LOE_02_START2.prefab:3f963f60c4ecb2341a001d4a8e80e4f0");
    this.PreloadSound("VO_LOE_02_START3.prefab:bf687ef7a2095b44bb25ff73d6795d90");
    this.PreloadSound("VO_LOE_02_TURN_6.prefab:d283d2a06fd1f4146a091e13e097492b");
    this.PreloadSound("VO_LOE_ZINAAR_TURN_6_CARTOGRAPHER_2.prefab:240f36f0d8777774387afe495f53b2d5");
    this.PreloadSound("VO_LOE_02_TURN_6_2.prefab:1bea540bd8f9f134c92c6841cd6e564d");
    this.PreloadSound("VO_LOE_ZINAAR_TURN_6_CARTOGRAPHER_2_ALT.prefab:13b73e3d2ea111f439b866c0da62773c");
    this.PreloadSound("VO_LOE_02_TURN_10.prefab:9c92e0752f28be44d902d98ea9617ab1");
    this.PreloadSound("VO_LOE_ZINAAR_TURN_10_CARTOGRAPHER_2.prefab:f4a2d6f5c7786cc4297a73fef98937fd");
    this.PreloadSound("VO_LOE_02_WIN.prefab:f311289fe2c9b5446943c3cb69f402da");
    this.PreloadSound("VO_LOE_02_MORE_WISHES.prefab:67291015ffb2c8f44b3e20553f25001b");
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
          m_soundName = "VO_LOE_02_RESPONSE.prefab:81df49b8799ffe7408d1ca6d13a0b1a9",
          m_stringTag = "VO_LOE_02_RESPONSE"
        }
      }
    }
  };

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOE01_Zinaar loE01Zinaar = this;
    if (missionEvent == 2)
    {
      GameState.Get().SetBusy(true);
      yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_02_WISH.prefab:e58bbad31e7b0e944a7c5ae8c67a6837"));
      GameState.Get().SetBusy(false);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    LOE01_Zinaar loE01Zinaar = this;
    while (loE01Zinaar.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (entity.GetCardId() == "LOEA02_06" && !loE01Zinaar.m_wishMoreWishesLinePlayed)
    {
      loE01Zinaar.m_wishMoreWishesLinePlayed = true;
      yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_02_MORE_WISHES.prefab:67291015ffb2c8f44b3e20553f25001b"));
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LOE01_Zinaar loE01Zinaar = this;
    while (loE01Zinaar.m_enemySpeaking)
      yield return (object) null;
    switch (turn)
    {
      case 1:
        yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_02_START2.prefab:3f963f60c4ecb2341a001d4a8e80e4f0"));
        yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26", "VO_LOE_02_START3.prefab:bf687ef7a2095b44bb25ff73d6795d90"));
        break;
      case 7:
        yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_02_TURN_6.prefab:d283d2a06fd1f4146a091e13e097492b"));
        yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26", "VO_LOE_ZINAAR_TURN_6_CARTOGRAPHER_2.prefab:240f36f0d8777774387afe495f53b2d5"));
        break;
      case 9:
        yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_02_TURN_6_2.prefab:1bea540bd8f9f134c92c6841cd6e564d"));
        yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26", "VO_LOE_ZINAAR_TURN_6_CARTOGRAPHER_2_ALT.prefab:13b73e3d2ea111f439b866c0da62773c"));
        break;
      case 13:
        yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayBigCharacterQuoteAndWaitOnce("Reno_BigQuote.prefab:63a25676d5e84264a9eb9c3d5c7e0921", "VO_LOE_02_TURN_10.prefab:9c92e0752f28be44d902d98ea9617ab1"));
        yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26", "VO_LOE_ZINAAR_TURN_10_CARTOGRAPHER_2.prefab:f4a2d6f5c7786cc4297a73fef98937fd"));
        break;
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    LOE01_Zinaar loE01Zinaar = this;
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) Gameplay.Get().StartCoroutine(loE01Zinaar.PlayCharacterQuoteAndWait("Reno_Quote.prefab:0a2e34fa6782a0747b4f5d5574d1331a", "VO_LOE_02_WIN.prefab:f311289fe2c9b5446943c3cb69f402da", allowRepeatDuringSession: false));
    }
  }
}
