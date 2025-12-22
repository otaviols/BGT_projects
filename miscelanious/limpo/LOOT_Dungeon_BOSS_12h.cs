using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOOT_Dungeon_BOSS_12h : LOOT_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_IntroLines = new List<string>()
  {
    "VO_LOOTA_BOSS_12h_Male_Kobold_Intro1_01.prefab:46cb2187ee29a324580ee8107c7c3c0a",
    "VO_LOOTA_BOSS_12h_Male_Kobold_Intro2_01.prefab:0ead7cf01e448f24aae1b25b7d633d99",
    "VO_LOOTA_BOSS_12h_Male_Kobold_Intro3_01.prefab:f52f03fbf2a9b4542aafb456236240ac"
  };
  private List<string> m_DeathLines = new List<string>()
  {
    "VO_LOOTA_BOSS_12h_Male_Kobold_Death1_01.prefab:ed87ca80573e2ab46afa1b7ecd055682",
    "VO_LOOTA_BOSS_12h_Male_Kobold_Death2_01.prefab:ed9c247de5783ca4ba252280da1ec02b",
    "VO_LOOTA_BOSS_12h_Male_Kobold_Death3_01.prefab:cc139c64563141a4299d25958b9e8af4"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_LOOTA_BOSS_12h_Male_Kobold_Intro1_01.prefab:46cb2187ee29a324580ee8107c7c3c0a",
      "VO_LOOTA_BOSS_12h_Male_Kobold_Intro2_01.prefab:0ead7cf01e448f24aae1b25b7d633d99",
      "VO_LOOTA_BOSS_12h_Male_Kobold_Intro3_01.prefab:f52f03fbf2a9b4542aafb456236240ac",
      "VO_LOOTA_BOSS_12h_Male_Kobold_EmoteResponse_01.prefab:8114296dd0a4eb742837f7147c34e37a",
      "VO_LOOTA_BOSS_12h_Male_Kobold_Death1_01.prefab:ed87ca80573e2ab46afa1b7ecd055682",
      "VO_LOOTA_BOSS_12h_Male_Kobold_Death2_01.prefab:ed9c247de5783ca4ba252280da1ec02b",
      "VO_LOOTA_BOSS_12h_Male_Kobold_Death3_01.prefab:cc139c64563141a4299d25958b9e8af4",
      "VO_LOOTA_BOSS_12h_Male_Kobold_DefeatPlayer_01.prefab:51813afc620f6db4cafa3e8bc65aef20",
      "VO_LOOTA_BOSS_12h_Male_Kobold_EventChargeBigMinion_01.prefab:9029d30fc0c80ef419c1b763e46356db",
      "VO_LOOTA_BOSS_12h_Male_Kobold_EventPlayerPlaysPatches_01.prefab:25aab388198063f47b882b311169cece",
      "VO_LOOTA_BOSS_12h_Male_Kobold_EventPlayerPlaysCrab_01.prefab:1e4208a2ac0cc584292fe3375d0145c7"
    })
      this.PreloadSound(soundPath);
  }

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      string introLine = this.m_IntroLines[Random.Range(0, this.m_IntroLines.Count)];
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech(introLine, Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_LOOTA_BOSS_12h_Male_Kobold_EmoteResponse_01.prefab:8114296dd0a4eb742837f7147c34e37a", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    LOOT_Dungeon_BOSS_12h lootDungeonBoss12h = this;
    while (lootDungeonBoss12h.m_enemySpeaking)
      yield return (object) null;
    Actor enemyActor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    string str = "PLAYED_MISSION_EVENT_" + (object) missionEvent;
    if (!lootDungeonBoss12h.m_playedLines.Contains(str))
    {
      yield return (object) lootDungeonBoss12h.PlayLoyalSideKickBetrayal(missionEvent);
      switch (missionEvent)
      {
        case 101:
          string deathLine = lootDungeonBoss12h.m_DeathLines[Random.Range(0, lootDungeonBoss12h.m_DeathLines.Count)];
          yield return (object) lootDungeonBoss12h.PlayBossLine(enemyActor, deathLine);
          break;
        case 102:
          yield return (object) lootDungeonBoss12h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_12h_Male_Kobold_EventChargeBigMinion_01.prefab:9029d30fc0c80ef419c1b763e46356db");
          break;
        case 103:
          yield return (object) new WaitForSeconds(4.5f);
          yield return (object) lootDungeonBoss12h.PlayBossLine(enemyActor, "VO_LOOTA_BOSS_12h_Male_Kobold_EventPlayerPlaysPatches_01.prefab:25aab388198063f47b882b311169cece");
          break;
      }
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    LOOT_Dungeon_BOSS_12h lootDungeonBoss12h = this;
    while (lootDungeonBoss12h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!lootDungeonBoss12h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) lootDungeonBoss12h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      lootDungeonBoss12h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "UNG_807")
        yield return (object) lootDungeonBoss12h.PlayEasterEggLine(actor, "VO_LOOTA_BOSS_12h_Male_Kobold_EventPlayerPlaysCrab_01.prefab:1e4208a2ac0cc584292fe3375d0145c7");
    }
  }
}
