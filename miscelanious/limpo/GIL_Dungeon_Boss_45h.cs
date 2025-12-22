using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GIL_Dungeon_Boss_45h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_PlayerDraw = new List<string>()
  {
    "VO_GILA_BOSS_45h_Female_Human_EventDrawsExtraCards_01.prefab:59b04c5fc78d998419e9031e5724ac21",
    "VO_GILA_BOSS_45h_Female_Human_EventDrawsExtraCards_02.prefab:f1ced766b713bcd4bab20b5df385b375",
    "VO_GILA_BOSS_45h_Female_Human_EventDrawsExtraCards_03.prefab:92c52e500b3a5a049b2d3d44db263ec8"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_45h_Female_Human_Intro_01.prefab:5cbaf2dc51852e24dbd17bbbdf87cf8f",
      "VO_GILA_BOSS_45h_Female_Human_EmoteResponse_01.prefab:ec9bd3abc0eb6d243b3fc7a34fa43406",
      "VO_GILA_BOSS_45h_Female_Human_Death_02.prefab:a281121fabb777e49a73fa3abde185d9",
      "VO_GILA_BOSS_45h_Female_Human_EventDrawsExtraCards_01.prefab:59b04c5fc78d998419e9031e5724ac21",
      "VO_GILA_BOSS_45h_Female_Human_EventDrawsExtraCards_02.prefab:f1ced766b713bcd4bab20b5df385b375",
      "VO_GILA_BOSS_45h_Female_Human_EventDrawsExtraCards_03.prefab:92c52e500b3a5a049b2d3d44db263ec8",
      "VO_GILA_BOSS_45h_Female_Human_EventPlaysAcolyte_01.prefab:6cd648f8d77f0c944b9d82978c4ed975",
      "VO_GILA_BOSS_45h_Female_Human_EventPlaysNovice_01.prefab:c54565566da737d4f8a8aa92cf1648da",
      "VO_GILA_BOSS_45h_Female_Human_EventPlaysHoarder_01.prefab:2a99af8e9559db74a88b2f53f41ceb2d",
      "VO_GILA_BOSS_45h_Female_Human_EventFirstDamage_01.prefab:2aae7467c86b7624085aa0e2bf6beaef",
      "VO_GILA_BOSS_45h_Female_Human_EventPlayHallowedWater_01.prefab:a0de02488030e9e4fb0caa5deced73d6"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_45h_Female_Human_Death_02.prefab:a281121fabb777e49a73fa3abde185d9";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_45h_Female_Human_Intro_01.prefab:5cbaf2dc51852e24dbd17bbbdf87cf8f", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_45h_Female_Human_EmoteResponse_01.prefab:ec9bd3abc0eb6d243b3fc7a34fa43406", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_45h gilDungeonBoss45h = this;
    while (gilDungeonBoss45h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    switch (missionEvent)
    {
      case 101:
        if (gilDungeonBoss45h.m_PlayerDraw.Count <= 0)
          break;
        string line = gilDungeonBoss45h.m_PlayerDraw[Random.Range(0, gilDungeonBoss45h.m_PlayerDraw.Count)];
        gilDungeonBoss45h.m_PlayerDraw.Remove(line);
        yield return (object) gilDungeonBoss45h.PlayLineOnlyOnce(actor, line);
        break;
      case 102:
        yield return (object) gilDungeonBoss45h.PlayBossLine(actor, "VO_GILA_BOSS_45h_Female_Human_EventFirstDamage_01.prefab:2aae7467c86b7624085aa0e2bf6beaef");
        break;
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_45h gilDungeonBoss45h = this;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss45h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss45h.WaitForEntitySoundToFinish(entity);
      string cardID = entity.GetCardId();
      gilDungeonBoss45h.m_playedLines.Add(cardID);
      while (gilDungeonBoss45h.m_enemySpeaking)
        yield return (object) null;
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      string str = cardID;
      if (!(str == "EX1_007"))
      {
        if (!(str == "EX1_015"))
        {
          if (!(str == "EX1_096"))
          {
            if (str == "GILA_850b")
              yield return (object) gilDungeonBoss45h.PlayEasterEggLine(actor, "VO_GILA_BOSS_45h_Female_Human_EventPlayHallowedWater_01.prefab:a0de02488030e9e4fb0caa5deced73d6");
          }
          else
            yield return (object) gilDungeonBoss45h.PlayEasterEggLine(actor, "VO_GILA_BOSS_45h_Female_Human_EventPlaysHoarder_01.prefab:2a99af8e9559db74a88b2f53f41ceb2d");
        }
        else
          yield return (object) gilDungeonBoss45h.PlayEasterEggLine(actor, "VO_GILA_BOSS_45h_Female_Human_EventPlaysNovice_01.prefab:c54565566da737d4f8a8aa92cf1648da");
      }
      else
        yield return (object) gilDungeonBoss45h.PlayEasterEggLine(actor, "VO_GILA_BOSS_45h_Female_Human_EventPlaysAcolyte_01.prefab:6cd648f8d77f0c944b9d82978c4ed975");
    }
  }
}
