using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_22h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_22h_Male_Undead_Intro_01.prefab:e2f409541d775204ea830c91c59b0fb8",
      "VO_GILA_BOSS_22h_Male_Undead_IntroTess_01.prefab:8c903084a02c75e47aa40f564b8f536a",
      "VO_GILA_BOSS_22h_Male_Undead_IntroCrowley_01.prefab:dacced81dd2ca414c89fe4e1700ede67",
      "VO_GILA_BOSS_22h_Male_Undead_EmoteResponse_01.prefab:35d13b301743d6b41b010da858bb9100",
      "VO_GILA_BOSS_22h_Male_Undead_EmoteResponseTess_01.prefab:54dd324d38e93764a8370741a5faf644",
      "VO_GILA_BOSS_22h_Male_Undead_Death_01.prefab:61c5f65e28a99f84d9d6219edf4ba769",
      "VO_GILA_BOSS_22h_Male_Undead_DefeatPlayer_01.prefab:54ef28245d1c78d4f988e652b352ccf1",
      "VO_GILA_BOSS_22h_Male_Undead_EventPlaysShiv_01.prefab:3b8e6220e4f4dee4dade6ae967cdd8f5",
      "VO_GILA_BOSS_22h_Male_Undead_EventPlaysCoin_01.prefab:2cf7e803e0a855149b7152e53e84ffc6"
    })
      this.PreloadSound(soundPath);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>();

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_22h_Male_Undead_Death_01.prefab:61c5f65e28a99f84d9d6219edf4ba769";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      string cardId = GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId();
      if (!(cardId == "GILA_500h3"))
      {
        if (cardId == "GILA_600h")
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_22h_Male_Undead_IntroCrowley_01.prefab:dacced81dd2ca414c89fe4e1700ede67", Notification.SpeechBubbleDirection.TopRight, actor));
        else
          Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_22h_Male_Undead_Intro_01.prefab:e2f409541d775204ea830c91c59b0fb8", Notification.SpeechBubbleDirection.TopRight, actor));
      }
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_22h_Male_Undead_IntroTess_01.prefab:8c903084a02c75e47aa40f564b8f536a", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      if (GameState.Get().GetFriendlySidePlayer().GetHero().GetCardId() == "GILA_500h3")
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_22h_Male_Undead_EmoteResponseTess_01.prefab:54dd324d38e93764a8370741a5faf644", Notification.SpeechBubbleDirection.TopRight, actor));
      else
        Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_22h_Male_Undead_EmoteResponse_01.prefab:35d13b301743d6b41b010da858bb9100", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_22h gilDungeonBoss22h = this;
    while (gilDungeonBoss22h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss22h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss22h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss22h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (!(cardId == "GAME_005") && !(cardId == "CFM_630"))
      {
        if (cardId == "EX1_278")
          yield return (object) gilDungeonBoss22h.PlayEasterEggLine(actor, "VO_GILA_BOSS_22h_Male_Undead_EventPlaysShiv_01.prefab:3b8e6220e4f4dee4dade6ae967cdd8f5");
      }
      else
        yield return (object) gilDungeonBoss22h.PlayEasterEggLine(actor, "VO_GILA_BOSS_22h_Male_Undead_EventPlaysCoin_01.prefab:2cf7e803e0a855149b7152e53e84ffc6");
    }
  }
}
