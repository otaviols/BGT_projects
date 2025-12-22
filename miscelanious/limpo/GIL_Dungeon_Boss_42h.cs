using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_42h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_RandomPlayerLines = new List<string>()
  {
    "VO_GILA_BOSS_42h_Male_Ogre_EventHeroPowerEnemy_01.prefab:ef150c3ba4a60d5458d9e247362591d2",
    "VO_GILA_BOSS_42h_Male_Ogre_EventHeroPowerEnemy_02.prefab:57607d76779377e4dbc655d6305af41c",
    "VO_GILA_BOSS_42h_Male_Ogre_EventHeroPowerEnemy_03.prefab:cd4e52c1b0db06046966c62b9648b2eb"
  };
  private List<string> m_RandomBossLines = new List<string>()
  {
    "VO_GILA_BOSS_42h_Male_Ogre_EventHeroPowerFriendly_01.prefab:f39845f182dbb084889d0ad1aed416ca",
    "VO_GILA_BOSS_42h_Male_Ogre_EventHeroPowerFriendly_02.prefab:9418c5fe0915ea64ea02701df85ffacb"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_42h_Male_Ogre_Intro_01.prefab:9149ad8f8b7c26945b6801fe5f0f86d5",
      "VO_GILA_BOSS_42h_Male_Ogre_EmoteResponse_01.prefab:b77493c05eb4de74dbd3b21e3359713e",
      "VO_GILA_BOSS_42h_Male_Ogre_Death_02.prefab:97ab39658338f224aa30bf1247c6b61e",
      "VO_GILA_BOSS_42h_Male_Ogre_EventHeroPowerFriendly_01.prefab:f39845f182dbb084889d0ad1aed416ca",
      "VO_GILA_BOSS_42h_Male_Ogre_EventHeroPowerFriendly_02.prefab:9418c5fe0915ea64ea02701df85ffacb",
      "VO_GILA_BOSS_42h_Male_Ogre_EventHeroPowerEnemy_01.prefab:ef150c3ba4a60d5458d9e247362591d2",
      "VO_GILA_BOSS_42h_Male_Ogre_EventHeroPowerEnemy_02.prefab:57607d76779377e4dbc655d6305af41c",
      "VO_GILA_BOSS_42h_Male_Ogre_EventHeroPowerEnemy_03.prefab:cd4e52c1b0db06046966c62b9648b2eb"
    })
      this.PreloadSound(soundPath);
  }

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_42h_Male_Ogre_Intro_01.prefab:9149ad8f8b7c26945b6801fe5f0f86d5", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_42h_Male_Ogre_EmoteResponse_01.prefab:b77493c05eb4de74dbd3b21e3359713e", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_42h_Male_Ogre_Death_02.prefab:97ab39658338f224aa30bf1247c6b61e";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_42h gilDungeonBoss42h = this;
    while (gilDungeonBoss42h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!gilDungeonBoss42h.m_playedLines.Contains(str))
    {
      switch (missionEvent)
      {
        case 101:
          string line1 = gilDungeonBoss42h.PopRandomLineWithChance(gilDungeonBoss42h.m_RandomPlayerLines);
          if (line1 == null)
            break;
          yield return (object) gilDungeonBoss42h.PlayLineOnlyOnce(actor, line1);
          break;
        case 102:
          string line2 = gilDungeonBoss42h.PopRandomLineWithChance(gilDungeonBoss42h.m_RandomBossLines);
          if (line2 == null)
            break;
          yield return (object) gilDungeonBoss42h.PlayLineOnlyOnce(actor, line2);
          break;
      }
    }
  }
}
