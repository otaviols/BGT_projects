using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_67h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_67h_Male_Undead_Intro_01.prefab:77315cc1645828346beace1a75cad6eb",
      "VO_GILA_BOSS_67h_Male_Undead_EmoteResponse_01.prefab:2916a45adc35de04eb7c232e050bfaf9",
      "VO_GILA_BOSS_67h_Male_Undead_Death_01.prefab:ec969a349b58f10419abaccd73d282dd",
      "VO_GILA_BOSS_67h_Male_Undead_EventPlayRat_01.prefab:33a73764b8071f94eaf228ea7be402c9",
      "VO_GILA_BOSS_67h_Male_Undead_EventPlayRat_02.prefab:f28d93840a2669543aa292b9de1c594f",
      "VO_GILA_BOSS_67h_Male_Undead_EventPlayRat_03.prefab:226b571d460839248aaae37de16b2d95",
      "VO_GILA_BOSS_67h_Male_Undead_EventPlayRatTrap_01.prefab:3c24f336ee2317746be5b165d1409345"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_67h_Male_Undead_Death_01.prefab:ec969a349b58f10419abaccd73d282dd";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_67h_Male_Undead_Intro_01.prefab:77315cc1645828346beace1a75cad6eb", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_67h_Male_Undead_EmoteResponse_01.prefab:2916a45adc35de04eb7c232e050bfaf9", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_67h gilDungeonBoss67h = this;
    while (gilDungeonBoss67h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss67h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss67h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss67h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "CFM_316"))
      {
        if (!(cardId == "CFM_790"))
        {
          if (!(cardId == "LOOT_069"))
          {
            if (cardId == "GIL_577")
              yield return (object) gilDungeonBoss67h.PlayLineOnlyOnce(actor, "VO_GILA_BOSS_67h_Male_Undead_EventPlayRatTrap_01.prefab:3c24f336ee2317746be5b165d1409345");
          }
          else
            yield return (object) gilDungeonBoss67h.PlayLineOnlyOnce(actor, "VO_GILA_BOSS_67h_Male_Undead_EventPlayRat_01.prefab:33a73764b8071f94eaf228ea7be402c9");
        }
        else
          yield return (object) gilDungeonBoss67h.PlayLineOnlyOnce(actor, "VO_GILA_BOSS_67h_Male_Undead_EventPlayRat_03.prefab:226b571d460839248aaae37de16b2d95");
      }
      else
        yield return (object) gilDungeonBoss67h.PlayLineOnlyOnce(actor, "VO_GILA_BOSS_67h_Male_Undead_EventPlayRat_02.prefab:f28d93840a2669543aa292b9de1c594f");
    }
  }
}
