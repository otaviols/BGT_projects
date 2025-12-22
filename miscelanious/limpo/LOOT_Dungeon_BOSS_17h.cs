using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOT_Dungeon_BOSS_17h : LOOT_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_BattlecryLines = new List<string>()
  {
    "VO_LOOTA_BOSS_17h_Male_Troll_EventPlayerBattlecry1_01.prefab:e88d4148f01b87b49a0b3fab1270219e",
    "VO_LOOTA_BOSS_17h_Male_Troll_EventPlayerBattlecry2_01.prefab:5109f4d775235bb4c9d2736fe5d9860d",
    "VO_LOOTA_BOSS_17h_Male_Troll_EventPlayerBattlecry3_01.prefab:0a43c143114985640b7585405406eef2"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_17h_Male_Troll_Intro_01.prefab:dc1c1e6dcdd83a54a8ff082a741b6d46",
      "VO_LOOTA_BOSS_17h_Male_Troll_EmoteResponse_01.prefab:5f2ffdacd3c4d974787263b2708ac237",
      "VO_LOOTA_BOSS_17h_Male_Troll_Death_01.prefab:5cc9961752e42494fa049e753a79ec20",
      "VO_LOOTA_BOSS_17h_Male_Troll_DefeatPlayer_01.prefab:38b3cb0dcd08da041befcf72f892c3a1",
      "VO_LOOTA_BOSS_17h_Male_Troll_EventPlayerBrann_01.prefab:a2dcc57d60e941c4ca6d8045a8268671",
      "VO_LOOTA_BOSS_17h_Male_Troll_EventPlayerBattlecry1_01.prefab:e88d4148f01b87b49a0b3fab1270219e",
      "VO_LOOTA_BOSS_17h_Male_Troll_EventPlayerBattlecry2_01.prefab:5109f4d775235bb4c9d2736fe5d9860d",
      "VO_LOOTA_BOSS_17h_Male_Troll_EventPlayerBattlecry3_01.prefab:0a43c143114985640b7585405406eef2"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_LOOTA_BOSS_17h_Male_Troll_Death_01.prefab:5cc9961752e42494fa049e753a79ec20";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_17h_Male_Troll_Intro_01.prefab:dc1c1e6dcdd83a54a8ff082a741b6d46", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_17h_Male_Troll_EmoteResponse_01.prefab:5f2ffdacd3c4d974787263b2708ac237", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    LOOT_Dungeon_BOSS_17h lootDungeonBoss17h = this;
    while (lootDungeonBoss17h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!lootDungeonBoss17h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) lootDungeonBoss17h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      lootDungeonBoss17h.m_playedLines.Add(cardId);
      Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "LOE_077")
        yield return (object) lootDungeonBoss17h.PlayEasterEggLine(enemyActor, "VO_LOOTA_BOSS_17h_Male_Troll_EventPlayerBrann_01.prefab:a2dcc57d60e941c4ca6d8045a8268671");
      if (entity.HasTag(GAME_TAG.BATTLECRY) && lootDungeonBoss17h.m_BattlecryLines.Count != 0)
      {
        string randomLine = lootDungeonBoss17h.m_BattlecryLines[Random.Range(0, lootDungeonBoss17h.m_BattlecryLines.Count)];
        yield return (object) lootDungeonBoss17h.PlayLineOnlyOnce(enemyActor, randomLine);
        lootDungeonBoss17h.m_BattlecryLines.Remove(randomLine);
        yield return (object) null;
        randomLine = (string) null;
      }
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_17h lootDungeonBoss17h = this;
    while (lootDungeonBoss17h.m_enemySpeaking)
      yield return (object) null;
    yield return (object) lootDungeonBoss17h.PlayLoyalSideKickBetrayal(missionEvent);
  }
}
