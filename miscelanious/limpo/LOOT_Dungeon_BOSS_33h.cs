using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOT_Dungeon_BOSS_33h : LOOT_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_TriggerPowerLines = new List<string>()
  {
    "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPowerHard1_01.prefab:dfd28e8d857457e44a1bedce379ee0b1",
    "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPowerHard2_01.prefab:90a3fc7d66f2c1f41a7322f56d3aad21",
    "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPowerHard3_01.prefab:68a1fb41b7d14e446bebc1489278086b"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_19h_Male_Trogg_Intro_01.prefab:00df9f15d69d8ce4e8553e579e3ff728",
      "VO_LOOTA_BOSS_19h_Male_Trogg_EmoteResponse_01.prefab:4385254ca60d5d64eb86d9341372d69f",
      "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPowerHard1_01.prefab:dfd28e8d857457e44a1bedce379ee0b1",
      "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPowerHard2_01.prefab:90a3fc7d66f2c1f41a7322f56d3aad21",
      "VO_LOOTA_BOSS_19h_Male_Trogg_HeroPowerHard3_01.prefab:68a1fb41b7d14e446bebc1489278086b",
      "VO_LOOTA_BOSS_19h_Male_Trogg_Death_01.prefab:d0fa743934bc7a24db09df3af3ce0b77",
      "VO_LOOTA_BOSS_19h_Male_Trogg_DefeatPlayer_01.prefab:0a0997eeb9130dc4382df8e2f6c23b2d",
      "VO_LOOTA_BOSS_19h_Male_Trogg_EventHandFull_01.prefab:0cf7309fe898f1b4cb9def67e388d19e"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_LOOTA_BOSS_19h_Male_Trogg_Death_01.prefab:d0fa743934bc7a24db09df3af3ce0b77";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_19h_Male_Trogg_Intro_01.prefab:00df9f15d69d8ce4e8553e579e3ff728", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_19h_Male_Trogg_EmoteResponse_01.prefab:4385254ca60d5d64eb86d9341372d69f", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_33h lootDungeonBoss33h = this;
    while (lootDungeonBoss33h.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!lootDungeonBoss33h.m_playedLines.Contains(str))
    {
      yield return (object) lootDungeonBoss33h.PlayLoyalSideKickBetrayal(missionEvent);
      switch (missionEvent)
      {
        case 102:
          if (lootDungeonBoss33h.m_TriggerPowerLines.Count == 0)
            break;
          string randomLine = lootDungeonBoss33h.m_TriggerPowerLines[Random.Range(0, lootDungeonBoss33h.m_TriggerPowerLines.Count)];
          yield return (object) lootDungeonBoss33h.PlayLineOnlyOnce(enemyActor, randomLine);
          lootDungeonBoss33h.m_TriggerPowerLines.Remove(randomLine);
          randomLine = (string) null;
          break;
        case 103:
          yield return (object) lootDungeonBoss33h.PlayLineOnlyOnce(enemyActor, "VO_LOOTA_BOSS_19h_Male_Trogg_EventHandFull_01.prefab:0cf7309fe898f1b4cb9def67e388d19e");
          break;
      }
    }
  }
}
