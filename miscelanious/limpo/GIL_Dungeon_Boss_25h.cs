using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_25h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_PlayerSecret = new List<string>()
  {
    "VO_GILA_BOSS_25h_Male_Dwarf_EventPlaysSecret_01.prefab:f15f63a93fb0fb447b218b4e9366c8f2",
    "VO_GILA_BOSS_25h_Male_Dwarf_EventPlaysSecret_02.prefab:75f0b53801fdb04429e57667479d80ae",
    "VO_GILA_BOSS_25h_Male_Dwarf_EventPlaysSecret_03.prefab:6fb3457a36d01754b9b54b2db165d831"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_25h_Male_Dwarf_Intro_01.prefab:77d5c0517d85a3946b7fc790529b757d",
      "VO_GILA_BOSS_25h_Male_Dwarf_IntroDarius_01.prefab:c8db45a57afef7145baceddc90bbc63c",
      "VO_GILA_BOSS_25h_Male_Dwarf_IntroTess_02.prefab:2a09b2d6d36724349a8cfbfd91013e76",
      "VO_GILA_BOSS_25h_Male_Dwarf_IntroToki_01.prefab:9adeab1320171c243a65489f14926936",
      "VO_GILA_BOSS_25h_Male_Dwarf_IntroShaw_01.prefab:ae89e24b92f8f6c4abb9574f5cb88065",
      "VO_GILA_BOSS_25h_Male_Dwarf_EmoteResponse_01.prefab:ee61b254aa4f2d64eb3539ad2ec8171f",
      "VO_GILA_BOSS_25h_Male_Dwarf_Death_01.prefab:cef498459698f86469d29d256ea0ccd5",
      "VO_GILA_BOSS_25h_Male_Dwarf_HeroPower_01.prefab:e64ac861d4bf04045bdefe6403fd749d",
      "VO_GILA_BOSS_25h_Male_Dwarf_HeroPower_02.prefab:10098d50ce3eb2b48bcb3259ca175ef7",
      "VO_GILA_BOSS_25h_Male_Dwarf_HeroPower_03.prefab:5388c3e465b13dc4b9e8dd24dbcc64d4",
      "VO_GILA_BOSS_25h_Male_Dwarf_HeroPower_04.prefab:106312a44427ffd4bb6102dafaa2c113",
      "VO_GILA_BOSS_25h_Male_Dwarf_EventPlaysSecret_01.prefab:f15f63a93fb0fb447b218b4e9366c8f2",
      "VO_GILA_BOSS_25h_Male_Dwarf_EventPlaysSecret_02.prefab:75f0b53801fdb04429e57667479d80ae",
      "VO_GILA_BOSS_25h_Male_Dwarf_EventPlaysSecret_03.prefab:6fb3457a36d01754b9b54b2db165d831",
      "VO_GILA_BOSS_25h_Male_Dwarf_EventTokiAbility_01.prefab:d844029587e1094419abaa128505e224",
      "VO_GILA_BOSS_25h_Male_Dwarf_EventPlayTrapper_01.prefab:e95606488e23ac447b5c8e9452755f76",
      "VO_GILA_BOSS_25h_Male_Dwarf_EventPlayCompass_01.prefab:6f67f38e3d94c9641b379d77336fe177",
      "VO_GILA_BOSS_25h_Male_Dwarf_EventPlayToolsOfTheTrade_01.prefab:f6223a5550a2a90429b49a372c5fcffd"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_GILA_BOSS_25h_Male_Dwarf_HeroPower_01.prefab:e64ac861d4bf04045bdefe6403fd749d",
    "VO_GILA_BOSS_25h_Male_Dwarf_HeroPower_02.prefab:10098d50ce3eb2b48bcb3259ca175ef7",
    "VO_GILA_BOSS_25h_Male_Dwarf_HeroPower_03.prefab:5388c3e465b13dc4b9e8dd24dbcc64d4",
    "VO_GILA_BOSS_25h_Male_Dwarf_HeroPower_04.prefab:106312a44427ffd4bb6102dafaa2c113"
  };

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_25h_Male_Dwarf_Death_01.prefab:cef498459698f86469d29d256ea0ccd5";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
      if (!(cardId == "GILA_500h3"))
      {
        if (!(cardId == "GILA_600h"))
        {
          if (!(cardId == "GILA_900h"))
          {
            if (cardId == "GILA_400h")
              Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_25h_Male_Dwarf_IntroShaw_01.prefab:ae89e24b92f8f6c4abb9574f5cb88065", Notification.SpeechBubbleDirection.TopRight, actor));
            else
              Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_25h_Male_Dwarf_Intro_01.prefab:77d5c0517d85a3946b7fc790529b757d", Notification.SpeechBubbleDirection.TopRight, actor));
          }
          else
            Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_25h_Male_Dwarf_IntroToki_01.prefab:9adeab1320171c243a65489f14926936", Notification.SpeechBubbleDirection.TopRight, actor));
        }
        else
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_25h_Male_Dwarf_IntroDarius_01.prefab:c8db45a57afef7145baceddc90bbc63c", Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_25h_Male_Dwarf_IntroTess_02.prefab:2a09b2d6d36724349a8cfbfd91013e76", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_25h_Male_Dwarf_EmoteResponse_01.prefab:ee61b254aa4f2d64eb3539ad2ec8171f", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_25h gilDungeonBoss25h = this;
    while (gilDungeonBoss25h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (missionEvent == 101)
    {
      string line = gilDungeonBoss25h.PopRandomLineWithChance(gilDungeonBoss25h.m_PlayerSecret);
      if (line != null)
        yield return (object) gilDungeonBoss25h.PlayLineOnlyOnce(actor, line);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_25h gilDungeonBoss25h = this;
    while (gilDungeonBoss25h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss25h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss25h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss25h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "GILA_509"))
      {
        if (!(cardId == "GILA_853b"))
        {
          if (cardId == "GILA_510")
            yield return (object) gilDungeonBoss25h.PlayEasterEggLine(actor, "VO_GILA_BOSS_25h_Male_Dwarf_EventPlayToolsOfTheTrade_01.prefab:f6223a5550a2a90429b49a372c5fcffd");
        }
        else
          yield return (object) gilDungeonBoss25h.PlayEasterEggLine(actor, "VO_GILA_BOSS_25h_Male_Dwarf_EventPlayCompass_01.prefab:6f67f38e3d94c9641b379d77336fe177");
      }
      else
        yield return (object) gilDungeonBoss25h.PlayEasterEggLine(actor, "VO_GILA_BOSS_25h_Male_Dwarf_EventPlayTrapper_01.prefab:e95606488e23ac447b5c8e9452755f76");
    }
  }

  protected override IEnumerator RespondToResetGameFinishedWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_25h gilDungeonBoss25h = this;
    while (gilDungeonBoss25h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss25h.m_playedLines.Contains(entity.GetCardId()))
    {
      string cardId = entity.GetCardId();
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "GILA_900p")
        yield return (object) gilDungeonBoss25h.PlayEasterEggLine(actor, "VO_GILA_BOSS_25h_Male_Dwarf_EventTokiAbility_01.prefab:d844029587e1094419abaa128505e224");
    }
  }
}
