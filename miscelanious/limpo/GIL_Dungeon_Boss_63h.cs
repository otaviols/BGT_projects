using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_63h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_RandomLines = new List<string>()
  {
    "VO_GILA_BOSS_63h_Female_Harpy_HeroPower_01.prefab:a2b0d7918b085ae47a2299b6bccd5acf",
    "VO_GILA_BOSS_63h_Female_Harpy_HeroPower_02.prefab:2fae3a49a855c0440af8dd3e19ff6e85",
    "VO_GILA_BOSS_63h_Female_Harpy_HeroPower_03.prefab:67795a869d43f364584bbbecdd81d421"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_63h_Female_Harpy_Intro_01.prefab:277992cd8bf83aa46a6f11beaa48074c",
      "VO_GILA_BOSS_63h_Female_Harpy_EmoteResponse_01.prefab:af25a266f8d1fe34388f2573a0b63bfd",
      "VO_GILA_BOSS_63h_Female_Harpy_Death_01.prefab:b7ad65011bf4b5d44a17c3409f1e3056",
      "VO_GILA_BOSS_63h_Female_Harpy_HeroPower_01.prefab:a2b0d7918b085ae47a2299b6bccd5acf",
      "VO_GILA_BOSS_63h_Female_Harpy_HeroPower_02.prefab:2fae3a49a855c0440af8dd3e19ff6e85",
      "VO_GILA_BOSS_63h_Female_Harpy_HeroPower_03.prefab:67795a869d43f364584bbbecdd81d421"
    })
      this.PreloadSound(soundPath);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_63h_Female_Harpy_Intro_01.prefab:277992cd8bf83aa46a6f11beaa48074c", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_63h_Female_Harpy_EmoteResponse_01.prefab:af25a266f8d1fe34388f2573a0b63bfd", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_63h_Female_Harpy_Death_01.prefab:b7ad65011bf4b5d44a17c3409f1e3056";

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_63h gilDungeonBoss63h = this;
    while (gilDungeonBoss63h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!gilDungeonBoss63h.m_playedLines.Contains(str) && missionEvent == 101)
    {
      string line = gilDungeonBoss63h.PopRandomLineWithChance(gilDungeonBoss63h.m_RandomLines);
      if (line != null)
        yield return (object) gilDungeonBoss63h.PlayLineOnlyOnce(actor, line);
    }
  }
}
