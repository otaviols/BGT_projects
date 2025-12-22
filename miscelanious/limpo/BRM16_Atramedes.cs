using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRM16_Atramedes : BRM_MissionEntity
{
  private bool m_heroPowerLinePlayed;
  private bool m_cardLinePlayed;
  private int m_gongLinePlayed;
  private int m_weaponLinePlayed;

  public override string GetAlternatePlayerName() => GameStrings.Get("MISSION_NEFARIAN_TITLE");

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA16_1_RESPONSE_03.prefab:ebc4ebb81b3a1b741b3895a371f72614");
    this.PreloadSound("VO_BRMA16_1_HERO_POWER_05.prefab:2facfeb30b95f49429ad143e643a3fe5");
    this.PreloadSound("VO_BRMA16_1_CARD_04.prefab:4c0923e1b9cbc854c9c78e549e2e62e4");
    this.PreloadSound("VO_BRMA16_1_GONG1_10.prefab:f36e1a59d28147749ad113da7831b5c6");
    this.PreloadSound("VO_BRMA16_1_GONG2_11.prefab:0064023a77f719646bea0ae472854c8b");
    this.PreloadSound("VO_BRMA16_1_GONG3_12.prefab:6d117262d495e6946aabe17ffff06c57");
    this.PreloadSound("VO_BRMA16_1_TRIGGER1_07.prefab:6651f227d949b2948b69f2317f29970c");
    this.PreloadSound("VO_BRMA16_1_TRIGGER2_08.prefab:97045358fdf509a42b86706dc0f3d477");
    this.PreloadSound("VO_BRMA16_1_TRIGGER3_09.prefab:6bd124e6a8a16fc4cacc8add95c429a6");
    this.PreloadSound("VO_BRMA16_1_TURN1_02.prefab:8edd557780fa3034c865f96650df136f");
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
          m_soundName = "VO_BRMA16_1_RESPONSE_03.prefab:ebc4ebb81b3a1b741b3895a371f72614",
          m_stringTag = "VO_BRMA16_1_RESPONSE_03"
        }
      }
    }
  };

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BRM16_Atramedes brM16Atramedes = this;
    while (brM16Atramedes.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "BRMA16_2") && !(cardId == "BRMA16_2H"))
    {
      if (cardId == "BRMA16_3" && !brM16Atramedes.m_cardLinePlayed)
      {
        brM16Atramedes.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(brM16Atramedes.PlaySoundAndBlockSpeech("VO_BRMA16_1_CARD_04.prefab:4c0923e1b9cbc854c9c78e549e2e62e4", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!brM16Atramedes.m_heroPowerLinePlayed)
    {
      brM16Atramedes.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(brM16Atramedes.PlaySoundAndBlockSpeech("VO_BRMA16_1_HERO_POWER_05.prefab:2facfeb30b95f49429ad143e643a3fe5", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BRM16_Atramedes brM16Atramedes = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (turn == 1)
      Gameplay.Get().StartCoroutine(brM16Atramedes.PlaySoundAndBlockSpeech("VO_BRMA16_1_TURN1_02.prefab:8edd557780fa3034c865f96650df136f", Notification.SpeechBubbleDirection.TopRight, actor));
    return false;
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    BRM16_Atramedes brM16Atramedes = this;
    while (brM16Atramedes.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 1:
        ++brM16Atramedes.m_gongLinePlayed;
        switch (brM16Atramedes.m_gongLinePlayed)
        {
          case 1:
            yield return (object) Gameplay.Get().StartCoroutine(brM16Atramedes.PlaySoundAndBlockSpeech("VO_BRMA16_1_GONG1_10.prefab:f36e1a59d28147749ad113da7831b5c6", Notification.SpeechBubbleDirection.TopRight, actor));
            yield break;
          case 2:
            yield return (object) Gameplay.Get().StartCoroutine(brM16Atramedes.PlaySoundAndBlockSpeech("VO_BRMA16_1_GONG3_12.prefab:6d117262d495e6946aabe17ffff06c57", Notification.SpeechBubbleDirection.TopRight, actor));
            yield break;
          case 3:
            yield return (object) Gameplay.Get().StartCoroutine(brM16Atramedes.PlaySoundAndBlockSpeech("VO_BRMA16_1_GONG2_11.prefab:0064023a77f719646bea0ae472854c8b", Notification.SpeechBubbleDirection.TopRight, actor));
            yield break;
          default:
            yield break;
        }
      case 2:
        ++brM16Atramedes.m_weaponLinePlayed;
        switch (brM16Atramedes.m_weaponLinePlayed)
        {
          case 1:
            yield return (object) Gameplay.Get().StartCoroutine(brM16Atramedes.PlaySoundAndBlockSpeech("VO_BRMA16_1_TRIGGER1_07.prefab:6651f227d949b2948b69f2317f29970c", Notification.SpeechBubbleDirection.TopRight, actor));
            yield break;
          case 2:
            yield return (object) Gameplay.Get().StartCoroutine(brM16Atramedes.PlaySoundAndBlockSpeech("VO_BRMA16_1_TRIGGER2_08.prefab:97045358fdf509a42b86706dc0f3d477", Notification.SpeechBubbleDirection.TopRight, actor));
            yield break;
          case 3:
            yield return (object) Gameplay.Get().StartCoroutine(brM16Atramedes.PlaySoundAndBlockSpeech("VO_BRMA16_1_TRIGGER3_09.prefab:6bd124e6a8a16fc4cacc8add95c429a6", Notification.SpeechBubbleDirection.TopRight, actor));
            yield break;
          default:
            yield break;
        }
    }
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateCharacterQuote("NefarianDragon_Quote.prefab:179fec888df7e4c02b8de3b7ad109a23", GameStrings.Get("VO_NEFARIAN_ATRAMEDES_DEATH_76"), "VO_NEFARIAN_ATRAMEDES_DEATH_76.prefab:7f23d65dd346a234fb410aeea9ec0d44");
    }
  }
}
