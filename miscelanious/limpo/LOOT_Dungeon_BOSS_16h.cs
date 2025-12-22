using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOT_Dungeon_BOSS_16h : LOOT_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_DeathrattleLines = new List<string>()
  {
    "VO_LOOTA_BOSS_16h_Male_Troll_EventPlayerDeathrattle1_01.prefab:395d36da235b4fb4f9b0477207d50e82",
    "VO_LOOTA_BOSS_16h_Male_Troll_EventPlayerDeathrattle2_01.prefab:386aae58797d12f4a8ada77b12b5145c",
    "VO_LOOTA_BOSS_16h_Male_Troll_EventPlayerDeathrattle3_01.prefab:65fd8caf09425d445af043833cc3c5ba",
    "VO_LOOTA_BOSS_16h_Male_Troll_EventPlayerDeathrattle4_01.prefab:63ee8d5e7b8fd9046b82eb7a138354ee",
    "VO_LOOTA_BOSS_16h_Male_Troll_EventPlayerDeathrattle5_01.prefab:3a64b92250de54a42b4458b3e81ca574"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_16h_Male_Troll_Intro_01.prefab:d18ce221a61804e47ad506f74a5ca73d",
      "VO_LOOTA_BOSS_16h_Male_Troll_EmoteResponse_01.prefab:aa8a544cd3c2f6943aeb6938349ed538",
      "VO_LOOTA_BOSS_16h_Male_Troll_Death_01.prefab:408bf2eabcb56f84abea5d2221843992",
      "VO_LOOTA_BOSS_16h_Male_Troll_DefeatPlayer_01.prefab:25af3e732ec6fb74494cb1ddd0fa94d7",
      "VO_LOOTA_BOSS_16h_Male_Troll_EventPlayerDeathrattle1_01.prefab:395d36da235b4fb4f9b0477207d50e82",
      "VO_LOOTA_BOSS_16h_Male_Troll_EventPlayerDeathrattle2_01.prefab:386aae58797d12f4a8ada77b12b5145c",
      "VO_LOOTA_BOSS_16h_Male_Troll_EventPlayerDeathrattle3_01.prefab:65fd8caf09425d445af043833cc3c5ba",
      "VO_LOOTA_BOSS_16h_Male_Troll_EventPlayerDeathrattle4_01.prefab:63ee8d5e7b8fd9046b82eb7a138354ee",
      "VO_LOOTA_BOSS_16h_Male_Troll_EventPlayerDeathrattle5_01.prefab:3a64b92250de54a42b4458b3e81ca574"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_LOOTA_BOSS_16h_Male_Troll_Death_01.prefab:408bf2eabcb56f84abea5d2221843992";

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_16h_Male_Troll_Intro_01.prefab:d18ce221a61804e47ad506f74a5ca73d", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_16h_Male_Troll_EmoteResponse_01.prefab:aa8a544cd3c2f6943aeb6938349ed538", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    LOOT_Dungeon_BOSS_16h lootDungeonBoss16h = this;
    while (lootDungeonBoss16h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!lootDungeonBoss16h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) lootDungeonBoss16h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      lootDungeonBoss16h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (entity.HasTag(GAME_TAG.DEATHRATTLE) && lootDungeonBoss16h.m_DeathrattleLines.Count != 0)
      {
        string randomLine = lootDungeonBoss16h.m_DeathrattleLines[Random.Range(0, lootDungeonBoss16h.m_DeathrattleLines.Count)];
        yield return (object) lootDungeonBoss16h.PlayLineOnlyOnce(actor, randomLine);
        lootDungeonBoss16h.m_DeathrattleLines.Remove(randomLine);
        yield return (object) null;
        randomLine = (string) null;
      }
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_16h lootDungeonBoss16h = this;
    while (lootDungeonBoss16h.m_enemySpeaking)
      yield return (object) null;
    yield return (object) lootDungeonBoss16h.PlayLoyalSideKickBetrayal(missionEvent);
  }
}
