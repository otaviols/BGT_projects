using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOT_Dungeon_BOSS_20h : LOOT_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_StatueDestroyedLines = new List<string>()
  {
    "VO_LOOTA_BOSS_20h_Male_Earthen_EventStatueDestroyed1_01.prefab:ef7a58c5d160ca541958ec50bdfb356c",
    "VO_LOOTA_BOSS_20h_Male_Earthen_EventStatueDestroyed2_01.prefab:0432d18c4a89d3c449e38372253cecd7",
    "VO_LOOTA_BOSS_20h_Male_Earthen_EventStatueDestroyed3_01.prefab:6b618e4b53aaceb4598f071f0a1566ec",
    "VO_LOOTA_BOSS_20h_Male_Earthen_EventStatueDestroyed4_01.prefab:c8e2a6ed7c8b5584fb3b02cf29c82ed0",
    "VO_LOOTA_BOSS_20h_Male_Earthen_EventStatueDestroyed5_01.prefab:0294fb2a21551f844b543bbd163cb506"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_20h_Male_Earthen_Intro_01.prefab:b1712d4e38816874f82613e4fa8060e2",
      "VO_LOOTA_BOSS_20h_Male_Earthen_EmoteResponse_01.prefab:b77df03627d7c2f449aa626db3255011",
      "VO_LOOTA_BOSS_20h_Male_Earthen_HeroPower_01.prefab:e37c0e7e80d2a684ebff084ab31d4555",
      "VO_LOOTA_BOSS_20h_Male_Earthen_HeroPowerNoStatues_01.prefab:dfec071a415b9f6498924e7fcda87900",
      "VO_LOOTA_BOSS_20h_Male_Earthen_Death_01.prefab:9c60adabd0d578e42a982f43d9e38395",
      "VO_LOOTA_BOSS_20h_Male_Earthen_DefeatPlayer_01.prefab:5283495640bba424ebb09ad1b79bc5d0",
      "VO_LOOTA_BOSS_20h_Male_Earthen_EventStatueDestroyed1_01.prefab:ef7a58c5d160ca541958ec50bdfb356c",
      "VO_LOOTA_BOSS_20h_Male_Earthen_EventStatueDestroyed2_01.prefab:0432d18c4a89d3c449e38372253cecd7",
      "VO_LOOTA_BOSS_20h_Male_Earthen_EventStatueDestroyed3_01.prefab:6b618e4b53aaceb4598f071f0a1566ec",
      "VO_LOOTA_BOSS_20h_Male_Earthen_EventStatueDestroyed4_01.prefab:c8e2a6ed7c8b5584fb3b02cf29c82ed0",
      "VO_LOOTA_BOSS_20h_Male_Earthen_EventStatueDestroyed5_01.prefab:0294fb2a21551f844b543bbd163cb506"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_LOOTA_BOSS_20h_Male_Earthen_Death_01.prefab:9c60adabd0d578e42a982f43d9e38395";

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_20h_Male_Earthen_Intro_01.prefab:b1712d4e38816874f82613e4fa8060e2", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_20h_Male_Earthen_EmoteResponse_01.prefab:b77df03627d7c2f449aa626db3255011", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_20h lootDungeonBoss20h = this;
    while (lootDungeonBoss20h.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!lootDungeonBoss20h.m_playedLines.Contains(str))
    {
      yield return (object) lootDungeonBoss20h.PlayLoyalSideKickBetrayal(missionEvent);
      switch (missionEvent)
      {
        case 101:
          int num1 = 50;
          int num2 = Random.Range(0, 100);
          if (lootDungeonBoss20h.m_StatueDestroyedLines.Count == 0 || num1 < num2)
            break;
          string randomLine = lootDungeonBoss20h.m_StatueDestroyedLines[Random.Range(0, lootDungeonBoss20h.m_StatueDestroyedLines.Count)];
          yield return (object) lootDungeonBoss20h.PlayLineOnlyOnce(enemyActor, randomLine);
          lootDungeonBoss20h.m_StatueDestroyedLines.Remove(randomLine);
          yield return (object) null;
          randomLine = (string) null;
          break;
        case 102:
          yield return (object) lootDungeonBoss20h.PlayLineOnlyOnce(enemyActor, "VO_LOOTA_BOSS_20h_Male_Earthen_HeroPower_01.prefab:e37c0e7e80d2a684ebff084ab31d4555");
          yield return (object) null;
          break;
        case 103:
          yield return (object) lootDungeonBoss20h.PlayLineOnlyOnce(enemyActor, "VO_LOOTA_BOSS_20h_Male_Earthen_HeroPowerNoStatues_01.prefab:dfec071a415b9f6498924e7fcda87900");
          yield return (object) null;
          break;
      }
    }
  }
}
