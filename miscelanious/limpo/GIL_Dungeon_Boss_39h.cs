using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_39h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_RandomShuffleLines = new List<string>()
  {
    "VO_GILA_BOSS_41h_Female_HumanGhost_EventShuffle_01.prefab:555e3e45f1d745a4f8c4a2cd6852c9e9",
    "VO_GILA_BOSS_41h_Female_HumanGhost_EventShuffle_02.prefab:d17efc332ca26e84698da64d005bc42c",
    "VO_GILA_BOSS_41h_Female_HumanGhost_EventShuffle_03.prefab:fbfd8102e3349e5409b55cb0bc3a9f32"
  };
  private List<string> m_RandomDrawLines = new List<string>()
  {
    "VO_GILA_BOSS_41h_Female_HumanGhost_HeroPower_01.prefab:1c885e702a6bd904fa0a2710922f33fd",
    "VO_GILA_BOSS_41h_Female_HumanGhost_HeroPower_02.prefab:49c1b6e5ba4085e46a44babc098c51b3",
    "VO_GILA_BOSS_41h_Female_HumanGhost_HeroPower_03.prefab:8fcebf4a48445414598e6d1d04e76526",
    "VO_GILA_BOSS_41h_Female_HumanGhost_HeroPower_04.prefab:0c39048a12578ef428fe10828178fe18"
  };
  private List<string> m_RandomFatigueLines = new List<string>()
  {
    "VO_GILA_BOSS_41h_Female_HumanGhost_EventFatigue_01.prefab:18453d7e2b37f714a805f082bcddf01d",
    "VO_GILA_BOSS_41h_Female_HumanGhost_EventFatigue_02.prefab:9299aad73816811429f4da6082e9fdda"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_41h_Female_HumanGhost_Intro_01.prefab:2d9a2c1744722b04487c9983f830b25f",
      "VO_GILA_BOSS_41h_Female_HumanGhost_EmoteResponse_01.prefab:2f02f1ad1940efc4498a56fdf9425bc1",
      "VO_GILA_BOSS_41h_Female_HumanGhost_Death_01.prefab:0826bc5f228a6724789cd46200d05a94",
      "VO_GILA_BOSS_41h_Female_HumanGhost_EventShuffle_01.prefab:555e3e45f1d745a4f8c4a2cd6852c9e9",
      "VO_GILA_BOSS_41h_Female_HumanGhost_EventShuffle_02.prefab:d17efc332ca26e84698da64d005bc42c",
      "VO_GILA_BOSS_41h_Female_HumanGhost_EventShuffle_03.prefab:fbfd8102e3349e5409b55cb0bc3a9f32",
      "VO_GILA_BOSS_41h_Female_HumanGhost_HeroPower_01.prefab:1c885e702a6bd904fa0a2710922f33fd",
      "VO_GILA_BOSS_41h_Female_HumanGhost_HeroPower_02.prefab:49c1b6e5ba4085e46a44babc098c51b3",
      "VO_GILA_BOSS_41h_Female_HumanGhost_HeroPower_03.prefab:8fcebf4a48445414598e6d1d04e76526",
      "VO_GILA_BOSS_41h_Female_HumanGhost_HeroPower_04.prefab:0c39048a12578ef428fe10828178fe18",
      "VO_GILA_BOSS_41h_Female_HumanGhost_EventFatigue_01.prefab:18453d7e2b37f714a805f082bcddf01d",
      "VO_GILA_BOSS_41h_Female_HumanGhost_EventFatigue_02.prefab:9299aad73816811429f4da6082e9fdda",
      "VO_GILA_BOSS_41h_Female_HumanGhost_EventFatigueDeath_01.prefab:0551a75f20704894ba0340fafe3eefc2",
      "VO_GILA_BOSS_41h_Female_HumanGhost_EventPlayRin_01.prefab:ad7bd3274cab68846ae61271015ebc01"
    })
      this.PreloadSound(soundPath);
  }

  protected override float ChanceToPlayRandomVOLine() => 1f;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_41h_Female_HumanGhost_Intro_01.prefab:2d9a2c1744722b04487c9983f830b25f", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_41h_Female_HumanGhost_EmoteResponse_01.prefab:2f02f1ad1940efc4498a56fdf9425bc1", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_41h_Female_HumanGhost_Death_01.prefab:0826bc5f228a6724789cd46200d05a94";

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_39h gilDungeonBoss39h = this;
    while (gilDungeonBoss39h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss39h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss39h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss39h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "BRM_007":
        case "CFM_602b":
        case "CFM_660":
        case "GILA_816a":
        case "GILA_817":
        case "GILA_821a":
        case "GILA_852a":
        case "GILA_BOSS_60t":
        case "GIL_815":
        case "GIL_828":
        case "ICC_091":
        case "LOE_002":
        case "LOE_079":
        case "LOE_104":
        case "LOOT_026":
        case "LOOT_106":
        case "UNG_851":
          string line = gilDungeonBoss39h.PopRandomLineWithChance(gilDungeonBoss39h.m_RandomShuffleLines);
          if (line == null)
            break;
          yield return (object) gilDungeonBoss39h.PlayBossLine(actor, line);
          break;
        case "LOOT_415":
          yield return (object) gilDungeonBoss39h.PlayBossLine(actor, "VO_GILA_BOSS_41h_Female_HumanGhost_EventPlayRin_01.prefab:ad7bd3274cab68846ae61271015ebc01");
          break;
      }
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_39h gilDungeonBoss39h = this;
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!gilDungeonBoss39h.m_playedLines.Contains(str))
    {
      while (gilDungeonBoss39h.m_enemySpeaking)
        yield return (object) null;
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
      switch (missionEvent)
      {
        case 101:
          string line1 = gilDungeonBoss39h.PopRandomLineWithChance(gilDungeonBoss39h.m_RandomDrawLines);
          if (line1 == null)
            break;
          yield return (object) gilDungeonBoss39h.PlayBossLine(actor, line1);
          break;
        case 102:
          string line2 = gilDungeonBoss39h.PopRandomLineWithChance(gilDungeonBoss39h.m_RandomFatigueLines);
          if (line2 == null)
            break;
          yield return (object) gilDungeonBoss39h.PlayBossLine(actor, line2);
          break;
        case 103:
          yield return (object) gilDungeonBoss39h.PlayBossLine(actor, "VO_GILA_BOSS_41h_Female_HumanGhost_EventFatigueDeath_01.prefab:0551a75f20704894ba0340fafe3eefc2");
          break;
      }
    }
  }
}
