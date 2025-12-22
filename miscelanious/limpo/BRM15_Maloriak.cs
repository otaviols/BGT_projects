using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRM15_Maloriak : BRM_MissionEntity
{
  private bool m_heroPowerLinePlayed;
  private bool m_cardLinePlayed;

  public override void PreloadAssets()
  {
    this.PreloadSound("VO_BRMA15_1_RESPONSE_03.prefab:3fe1fdbbc2206a94dbc13d3e72a2eca4");
    this.PreloadSound("VO_BRMA15_1_HERO_POWER_06.prefab:27e3e01a28e94254a95c382f14bafe2c");
    this.PreloadSound("VO_BRMA15_1_CARD_05.prefab:459bd07b2915f1d448418b2ddcd89917");
    this.PreloadSound("VO_BRMA15_1_TURN1_02.prefab:b2e6fc2aa07c2c743844fad9a8e782bb");
    this.PreloadSound("VO_NEFARIAN_MALORIAK_TURN2_71.prefab:42b1e1e13b743b945a1f068acd056c21");
    this.PreloadSound("VO_NEFARIAN_MALORIAK_DEATH_PT1_72.prefab:a3baf4ca0cd4aff4db6a62ef44811c54");
    this.PreloadSound("VO_NEFARIAN_MALORIAK_DEATH_PT2_73.prefab:7b6518aa67d0a544598e6689904d9713");
    this.PreloadSound("VO_NEFARIAN_MALORIAK_DEATH_PT3_74.prefab:6f8c07be05efd4b458547b52559b61ae");
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
          m_soundName = "VO_BRMA15_1_RESPONSE_03.prefab:3fe1fdbbc2206a94dbc13d3e72a2eca4",
          m_stringTag = "VO_BRMA15_1_RESPONSE_03"
        }
      }
    }
  };

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    BRM15_Maloriak brM15Maloriak = this;
    while (brM15Maloriak.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
    string cardId = entity.GetCardId();
    if (!(cardId == "BRMA15_2") && !(cardId == "BRMA15_2H"))
    {
      if (cardId == "BRMA15_3" && !brM15Maloriak.m_cardLinePlayed)
      {
        brM15Maloriak.m_cardLinePlayed = true;
        Gameplay.Get().StartCoroutine(brM15Maloriak.PlaySoundAndBlockSpeech("VO_BRMA15_1_CARD_05.prefab:459bd07b2915f1d448418b2ddcd89917", Notification.SpeechBubbleDirection.TopRight, actor));
      }
    }
    else if (!brM15Maloriak.m_heroPowerLinePlayed)
    {
      brM15Maloriak.m_heroPowerLinePlayed = true;
      Gameplay.Get().StartCoroutine(brM15Maloriak.PlaySoundAndBlockSpeech("VO_BRMA15_1_HERO_POWER_06.prefab:27e3e01a28e94254a95c382f14bafe2c", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleStartOfTurnWithTiming(int turn)
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    BRM15_Maloriak brM15Maloriak = this;
    if (num != 0)
      return false;
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (turn)
    {
      case 1:
        Gameplay.Get().StartCoroutine(brM15Maloriak.PlaySoundAndBlockSpeech("VO_BRMA15_1_TURN1_02.prefab:b2e6fc2aa07c2c743844fad9a8e782bb", Notification.SpeechBubbleDirection.TopRight, actor));
        break;
      case 2:
        NotificationManager.Get().CreateCharacterQuote("NefarianDragon_Quote.prefab:179fec888df7e4c02b8de3b7ad109a23", new Vector3(95f, NotificationManager.DEPTH, 36.8f), GameStrings.Get("VO_NEFARIAN_MALORIAK_TURN2_71"), "VO_NEFARIAN_MALORIAK_TURN2_71.prefab:42b1e1e13b743b945a1f068acd056c21");
        break;
    }
    return false;
  }

  protected override IEnumerator HandleGameOverWithTiming(TAG_PLAYSTATE gameResult)
  {
    BRM15_Maloriak brM15Maloriak = this;
    if (gameResult == TAG_PLAYSTATE.WON && !GameMgr.Get().IsClassChallengeMission())
    {
      yield return (object) new WaitForSeconds(5f);
      NotificationManager.Get().CreateCharacterQuote("NormalNefarian_Quote.prefab:708840e536eb141479a23b632ebcc913", GameStrings.Get("VO_NEFARIAN_MALORIAK_DEATH_PT2_73"), "", durationSeconds: 5f);
      yield return (object) Gameplay.Get().StartCoroutine(brM15Maloriak.PlaySoundAndWait("VO_NEFARIAN_MALORIAK_DEATH_PT2_73.prefab:7b6518aa67d0a544598e6689904d9713", "", Notification.SpeechBubbleDirection.None, (Actor) null));
      NotificationManager.Get().DestroyActiveQuote(0.0f);
      NotificationManager.Get().CreateCharacterQuote("NefarianDragon_Quote.prefab:179fec888df7e4c02b8de3b7ad109a23", GameStrings.Get("VO_NEFARIAN_MALORIAK_DEATH_PT3_74"), "", durationSeconds: 7f);
      yield return (object) Gameplay.Get().StartCoroutine(brM15Maloriak.PlaySoundAndWait("VO_NEFARIAN_MALORIAK_DEATH_PT3_74.prefab:6f8c07be05efd4b458547b52559b61ae", "", Notification.SpeechBubbleDirection.None, (Actor) null));
      NotificationManager.Get().DestroyActiveQuote(0.0f);
    }
  }
}
