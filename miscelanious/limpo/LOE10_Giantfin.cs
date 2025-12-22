using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOE10_Giantfin : LOE_MissionEntity
{
  private bool m_cardLinePlayed1;
  private bool m_cardLinePlayed2;
  private bool m_nyahLinePlayed;
  private int m_turnToPlayFoundLine = -1;

  public override void StartGameplaySoundtracks() => MusicManager.Get().StartPlaylist(MusicPlaylistType.InGame_LOE_Wing3);

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_LOEA10_1_MIDDLEFIN.prefab:db87360596b82634f9350b9fb516a52c");
    this.PreloadSound("VO_LOE10_NYAH_FINLEY.prefab:7d68f7ae697cde142ae6875d4758b0b0");
    this.PreloadSound("VO_LOE_10_NYAH.prefab:e9c4965c3cf4d274886180e4facf749f");
    this.PreloadSound("VO_LOE_10_RESPONSE.prefab:d59cf1de856198e4f9443ae4bdb2d04a");
    this.PreloadSound("VO_LOE_10_START_2.prefab:dec3b2452f06e4542b21afabb06cbdbf");
    this.PreloadSound("VO_LOE_10_TURN1.prefab:5f1e5b8506cd049419e78ee08f8143e4");
    this.PreloadSound("VO_LOE_10_WIN.prefab:31018ff004f803045bcb5e3af8447198");
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOE10_Giantfin loE10Giantfin = this;
    // ISSUE: reference to a compiler-generated method
    yield return (object) Gameplay.Get().StartCoroutine(loE10Giantfin.\u003C\u003En__0(missionEvent));
    while (loE10Giantfin.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 2:
        if (loE10Giantfin.m_cardLinePlayed2)
          break;
        loE10Giantfin.m_cardLinePlayed2 = true;
        Gameplay.Get().StartCoroutine(loE10Giantfin.PlaySoundAndBlockSpeechOnce("VO_LOEA10_1_MIDDLEFIN.prefab:db87360596b82634f9350b9fb516a52c", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case 3:
        if (loE10Giantfin.m_cardLinePlayed1)
          break;
        loE10Giantfin.m_cardLinePlayed1 = true;
        break;
    }
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    LOE10_Giantfin loE10Giantfin = this;
    while (loE10Giantfin.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    if (entity.GetCardId() == "LOEA10_5" && !loE10Giantfin.m_nyahLinePlayed)
    {
      loE10Giantfin.m_nyahLinePlayed = true;
      Gameplay.Get().StartCoroutine(loE10Giantfin.PlaySoundAndBlockSpeechOnce("VO_LOE_10_NYAH.prefab:e9c4965c3cf4d274886180e4facf749f", Notification.SpeechBubbleDirection.TopRight, actor));
      yield return (object) new WaitForSeconds(4f);
      yield return (object) Gameplay.Get().StartCoroutine(loE10Giantfin.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote.prefab:1c1c332cf5009194cb7dd7316c465aee", "VO_LOE10_NYAH_FINLEY.prefab:7d68f7ae697cde142ae6875d4758b0b0"));
    }
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
          m_soundName = "VO_LOE_10_RESPONSE.prefab:d59cf1de856198e4f9443ae4bdb2d04a",
          m_stringTag = "VO_LOE_10_RESPONSE"
        }
      }
    }
  };

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    LOE10_Giantfin loE10Giantfin = this;
    while (loE10Giantfin.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (loE10Giantfin.m_turnToPlayFoundLine == 5)
      loE10Giantfin.m_turnToPlayFoundLine = 7;
    if (turn == loE10Giantfin.m_turnToPlayFoundLine)
      loE10Giantfin.m_turnToPlayFoundLine = -1;
    else if (turn == 1)
    {
      yield return (object) Gameplay.Get().StartCoroutine(loE10Giantfin.PlaySoundAndBlockSpeechOnce("VO_LOE_10_TURN1.prefab:5f1e5b8506cd049419e78ee08f8143e4", Notification.SpeechBubbleDirection.TopRight, actor));
      yield return (object) Gameplay.Get().StartCoroutine(loE10Giantfin.PlayBigCharacterQuoteAndWaitOnce("Finley_BigQuote.prefab:1c1c332cf5009194cb7dd7316c465aee", "VO_LOE_10_START_2.prefab:dec3b2452f06e4542b21afabb06cbdbf"));
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    LOE10_Giantfin loE10Giantfin = this;
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      yield return (object) Gameplay.Get().StartCoroutine(loE10Giantfin.PlayCharacterQuoteAndWait("Blaggh_Quote.prefab:f5d1e7053e6368e4a930ca3906cff53a", "VO_LOE_10_WIN.prefab:31018ff004f803045bcb5e3af8447198", allowRepeatDuringSession: false));
    }
  }
}
