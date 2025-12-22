using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOE08_Archaedas : LOE_MissionEntity
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_LOE_08_RESPONSE.prefab:4efe606a6afa83b459855b0d4566f17e");
    this.PreloadSound("VO_LOEA08_TURN_1_BRANN.prefab:c48fbd5e80d73194c91ead50e9ee20ef");
    this.PreloadSound("VO_LOE_ARCHAEDAS_TURN_1_CARTOGRAPHER.prefab:1fbc6b6415f7c604cb00d0b88651e303");
    this.PreloadSound("VO_LOE_08_LANDSLIDE.prefab:c56764ff130183f4688c0dfb30eaf8b2");
    this.PreloadSound("VO_LOE_08_ANIMATE_STONE.prefab:75c31e408053e0748aae95242e662f27");
    this.PreloadSound("VO_LOE_08_WIN.prefab:d40a0f7dc56bcf74692815bb06710a00");
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
          m_soundName = "VO_LOE_08_RESPONSE.prefab:4efe606a6afa83b459855b0d4566f17e",
          m_stringTag = "VO_LOE_08_RESPONSE"
        }
      }
    }
  };

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LOE08_Archaedas loE08Archaedas = this;
    if (turn == 1)
    {
      yield return (object) Gameplay.Get().StartCoroutine(loE08Archaedas.PlayBigCharacterQuoteAndWaitOnce("Brann_BigQuote.prefab:a03dd286404083c439e371ba84d7a82b", "VO_LOEA08_TURN_1_BRANN.prefab:c48fbd5e80d73194c91ead50e9ee20ef"));
      yield return (object) Gameplay.Get().StartCoroutine(loE08Archaedas.PlayBigCharacterQuoteAndWaitOnce("Elise_BigQuote.prefab:932bc9e74bb49e047ae8dd480492db26", "VO_LOE_ARCHAEDAS_TURN_1_CARTOGRAPHER.prefab:1fbc6b6415f7c604cb00d0b88651e303", 5f));
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    LOE08_Archaedas loE08Archaedas = this;
    while (loE08Archaedas.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!loE08Archaedas.m_playedLines.Contains(entity.GetCardId()))
    {
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      string cardId = entity.GetCardId();
      if (!(cardId == "LOEA06_04"))
      {
        if (cardId == "LOEA06_03")
        {
          loE08Archaedas.m_playedLines.Add(cardId);
          Gameplay.Get().StartCoroutine(loE08Archaedas.PlaySoundAndBlockSpeechOnce("VO_LOE_08_ANIMATE_STONE.prefab:75c31e408053e0748aae95242e662f27", Notification.SpeechBubbleDirection.TopRight, actor));
        }
      }
      else
      {
        loE08Archaedas.m_playedLines.Add(cardId);
        Gameplay.Get().StartCoroutine(loE08Archaedas.PlaySoundAndBlockSpeechOnce("VO_LOE_08_LANDSLIDE.prefab:c56764ff130183f4688c0dfb30eaf8b2", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    LOE08_Archaedas loE08Archaedas = this;
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) Gameplay.Get().StartCoroutine(loE08Archaedas.PlayCharacterQuoteAndWait("Brann_Quote.prefab:2c11651ab7740924189734944b8d7089", "VO_LOE_08_WIN.prefab:d40a0f7dc56bcf74692815bb06710a00", allowRepeatDuringSession: false));
    }
  }
}
