using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_40h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_PlayerAxe = new List<string>()
  {
    "VO_GILA_BOSS_40h_Female_Treant_EventPlaysAxe_01.prefab:fb8468b00a5f0cc428dfcfce728fa042",
    "VO_GILA_BOSS_40h_Female_Treant_EventPlaysAxe_02.prefab:0a5c52faccaf8c5489a2da55a4e3590f"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_40h_Female_Treant_Intro_01.prefab:5d14b06a592ef5640ae7acbdb74c031a",
      "VO_GILA_BOSS_40h_Female_Treant_EmoteResponse_01.prefab:1252f452e179aea4c97325973e1c3dc4",
      "VO_GILA_BOSS_40h_Female_Treant_Death_01.prefab:14e1eb2eba09ae74eafa6c80011b74bb",
      "VO_GILA_BOSS_40h_Female_Treant_HeroPower_01.prefab:3a425a60e1d4a9c4dbc092e0a82bbf20",
      "VO_GILA_BOSS_40h_Female_Treant_HeroPower_02.prefab:ba115807ed6f853428c7742cdb023f39",
      "VO_GILA_BOSS_40h_Female_Treant_HeroPower_03.prefab:3c769fa8be22e194ca01135dd8169999",
      "VO_GILA_BOSS_40h_Female_Treant_HeroPower_04.prefab:0add75f1f2992354b89863a7b0672469",
      "VO_GILA_BOSS_40h_Female_Treant_EventPlaysWoodsmansAxe_01.prefab:f765de0ef498d5f48998db8dac4ff08a",
      "VO_GILA_BOSS_40h_Female_Treant_EventPlaysAxe_01.prefab:fb8468b00a5f0cc428dfcfce728fa042",
      "VO_GILA_BOSS_40h_Female_Treant_EventPlaysAxe_02.prefab:0a5c52faccaf8c5489a2da55a4e3590f"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_GILA_BOSS_40h_Female_Treant_HeroPower_01.prefab:3a425a60e1d4a9c4dbc092e0a82bbf20",
    "VO_GILA_BOSS_40h_Female_Treant_HeroPower_02.prefab:ba115807ed6f853428c7742cdb023f39",
    "VO_GILA_BOSS_40h_Female_Treant_HeroPower_03.prefab:3c769fa8be22e194ca01135dd8169999",
    "VO_GILA_BOSS_40h_Female_Treant_HeroPower_04.prefab:0add75f1f2992354b89863a7b0672469"
  };

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_40h_Female_Treant_Death_01.prefab:14e1eb2eba09ae74eafa6c80011b74bb";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_40h_Female_Treant_Intro_01.prefab:5d14b06a592ef5640ae7acbdb74c031a", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_40h_Female_Treant_EmoteResponse_01.prefab:1252f452e179aea4c97325973e1c3dc4", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_40h gilDungeonBoss40h = this;
    while (gilDungeonBoss40h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss40h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss40h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss40h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      switch (cardId)
      {
        case "CS2_106":
        case "CS2_112":
        case "EX1_247":
        case "EX1_398":
        case "EX1_411":
        case "FP1_021":
        case "ICC_236":
        case "LOOT_380":
          string line = gilDungeonBoss40h.PopRandomLineWithChance(gilDungeonBoss40h.m_PlayerAxe);
          if (line == null)
            break;
          yield return (object) gilDungeonBoss40h.PlayLineOnlyOnce(actor, line);
          break;
        case "GIL_653":
          yield return (object) gilDungeonBoss40h.PlayEasterEggLine(actor, "VO_GILA_BOSS_40h_Female_Treant_EventPlaysWoodsmansAxe_01.prefab:f765de0ef498d5f48998db8dac4ff08a");
          break;
      }
    }
  }
}
