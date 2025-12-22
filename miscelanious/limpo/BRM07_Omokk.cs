using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRM07_Omokk : BRM_MissionEntity
{
  private bool m_heroPowerLinePlayed;
  private bool m_cardLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA07_1_RESPONSE_03.prefab:b43d82ce7a9bb59438b594dd3c185050");
    this.PreloadSound("VO_BRMA07_1_HERO_POWER_05.prefab:10f8a1b1fc7c9374b8c2b741f27694be");
    this.PreloadSound("VO_BRMA07_1_CARD_04.prefab:f498bd13724f67d48a0f0bc55034c44b");
    this.PreloadSound("VO_BRMA07_1_TURN1_02.prefab:ac11bf2418c6e0f418f2216348b224c3");
    this.PreloadSound("VO_NEFARIAN_OMOKK1_44.prefab:82ad9b06a62bf044b9e5660054e5fae6");
    this.PreloadSound("VO_NEFARIAN_OMOKK2_45.prefab:bb20664f6b0c27149a6048f473ca0398");
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
          m_soundName = "VO_BRMA07_1_RESPONSE_03.prefab:b43d82ce7a9bb59438b594dd3c185050",
          m_stringTag = "VO_BRMA07_1_RESPONSE_03"
        }
      }
    }
  };

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BRM07_Omokk brM07Omokk = this;
    while (brM07Omokk.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "BRMA07_2") && !(cardId == "BRMA07_2H"))
    {
      if (cardId == "BRMA07_3" && !brM07Omokk.m_cardLinePlayed)
      {
        brM07Omokk.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(brM07Omokk.PlaySoundAndBlockSpeech("VO_BRMA07_1_CARD_04.prefab:f498bd13724f67d48a0f0bc55034c44b", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!brM07Omokk.m_heroPowerLinePlayed)
    {
      brM07Omokk.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(brM07Omokk.PlaySoundAndBlockSpeech("VO_BRMA07_1_HERO_POWER_05.prefab:10f8a1b1fc7c9374b8c2b741f27694be", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BRM07_Omokk brM07Omokk = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Vector3 position = new Vector3(95f, NotificationManager.DEPTH, 36.8f);
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (turn)
    {
      case 1:
        Gameplay.Get().StartCoroutine(brM07Omokk.PlaySoundAndBlockSpeech("VO_BRMA07_1_TURN1_02.prefab:ac11bf2418c6e0f418f2216348b224c3", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case 4:
        NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", position, GameStrings.Get("VO_NEFARIAN_OMOKK1_44"), "VO_NEFARIAN_OMOKK1_44.prefab:82ad9b06a62bf044b9e5660054e5fae6");
        break;
      case 8:
        NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", position, GameStrings.Get("VO_NEFARIAN_OMOKK2_45"), "VO_NEFARIAN_OMOKK2_45.prefab:bb20664f6b0c27149a6048f473ca0398");
        break;
    }
    return false;
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", GameStrings.Get("VO_NEFARIAN_OMOKK_DEAD_46"), "VO_NEFARIAN_OMOKK_DEAD_46.prefab:894d0e92341ab754281388682e449096");
    }
  }
}
