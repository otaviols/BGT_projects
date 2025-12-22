using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_64h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_SmallRandomLines = new List<string>()
  {
    "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandSmall_01.prefab:5d88bcc0a05075040b7cfb83fdf592e6",
    "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandSmall_02.prefab:28698528267e1ea4f986aecd06372f99"
  };
  private List<string> m_MediumRandomLines = new List<string>()
  {
    "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandMedium_01.prefab:e6e970329d0630d44812eff64b772d04",
    "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandMedium_02.prefab:d29b2384eabceff49809f4045f193b40",
    "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandMedium_03.prefab:64f6e89d477969b4fbec31647e2f6b86"
  };
  private List<string> m_BigRandomLines = new List<string>()
  {
    "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandBig_01.prefab:aa92613c7b125fc499746b9111ab3f24",
    "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandBig_03.prefab:5cef405dfb09b5c49beda04ef476b9ed"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_64h_Male_Goblin_Intro_01.prefab:65aab9bfdc181c34794b8d264d546266",
      "VO_GILA_BOSS_64h_Male_Goblin_EmoteResponse_01.prefab:a0a1b9e4c2e6f9f4bafe6ccf9a265e98",
      "VO_GILA_BOSS_64h_Male_Goblin_Death_01.prefab:c4ef05fd8de6e2843b7436f68c970354",
      "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandEmpty_01.prefab:6a88ebdeb564cf34c8a5ecc5f16f11de",
      "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandSmall_01.prefab:5d88bcc0a05075040b7cfb83fdf592e6",
      "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandSmall_02.prefab:28698528267e1ea4f986aecd06372f99",
      "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandMedium_01.prefab:e6e970329d0630d44812eff64b772d04",
      "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandMedium_02.prefab:d29b2384eabceff49809f4045f193b40",
      "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandMedium_03.prefab:64f6e89d477969b4fbec31647e2f6b86",
      "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandBig_01.prefab:aa92613c7b125fc499746b9111ab3f24",
      "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandBig_03.prefab:5cef405dfb09b5c49beda04ef476b9ed",
      "VO_GILA_BOSS_64h_Male_Goblin_EventMCTech_01.prefab:7918a8f38c26d5e4d878d1df3646a1aa",
      "VO_GILA_BOSS_64h_Male_Goblin_EventMindControl_01.prefab:a624eb8406e178d4da862a4de7765ab2",
      "VO_GILA_BOSS_64h_Male_Goblin_EvenPlayCursedCurio_01.prefab:335bddebe01902d479380f324eee0071"
    })
      this.PreloadSound(soundPath);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_64h_Male_Goblin_Intro_01.prefab:65aab9bfdc181c34794b8d264d546266", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_64h_Male_Goblin_EmoteResponse_01.prefab:a0a1b9e4c2e6f9f4bafe6ccf9a265e98", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_64h_Male_Goblin_Death_01.prefab:c4ef05fd8de6e2843b7436f68c970354";

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_64h gilDungeonBoss64h = this;
    while (gilDungeonBoss64h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss64h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss64h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss64h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "EX1_085"))
      {
        if (!(cardId == "CS1_113"))
        {
          if (cardId == "GILA_819")
            yield return (object) gilDungeonBoss64h.PlayBossLine(actor, "VO_GILA_BOSS_64h_Male_Goblin_EvenPlayCursedCurio_01.prefab:335bddebe01902d479380f324eee0071");
        }
        else
          yield return (object) gilDungeonBoss64h.PlayBossLine(actor, "VO_GILA_BOSS_64h_Male_Goblin_EventMindControl_01.prefab:a624eb8406e178d4da862a4de7765ab2");
      }
      else
        yield return (object) gilDungeonBoss64h.PlayBossLine(actor, "VO_GILA_BOSS_64h_Male_Goblin_EventMCTech_01.prefab:7918a8f38c26d5e4d878d1df3646a1aa");
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_64h gilDungeonBoss64h = this;
    while (gilDungeonBoss64h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!gilDungeonBoss64h.m_playedLines.Contains(str))
    {
      switch (missionEvent)
      {
        case 101:
          yield return (object) gilDungeonBoss64h.PlayLineOnlyOnce(actor, "VO_GILA_BOSS_64h_Male_Goblin_HeroPowerHandEmpty_01.prefab:6a88ebdeb564cf34c8a5ecc5f16f11de");
          break;
        case 102:
          string line1 = gilDungeonBoss64h.PopRandomLineWithChance(gilDungeonBoss64h.m_SmallRandomLines);
          if (line1 == null)
            break;
          yield return (object) gilDungeonBoss64h.PlayLineOnlyOnce(actor, line1);
          break;
        case 103:
          string line2 = gilDungeonBoss64h.PopRandomLineWithChance(gilDungeonBoss64h.m_MediumRandomLines);
          if (line2 == null)
            break;
          yield return (object) gilDungeonBoss64h.PlayLineOnlyOnce(actor, line2);
          break;
        case 104:
          string line3 = gilDungeonBoss64h.PopRandomLineWithChance(gilDungeonBoss64h.m_BigRandomLines);
          if (line3 == null)
            break;
          yield return (object) gilDungeonBoss64h.PlayLineOnlyOnce(actor, line3);
          break;
      }
    }
  }
}
