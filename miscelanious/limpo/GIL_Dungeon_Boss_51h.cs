using System.Collections;
using System.Collections.Generic;

public class GIL_Dungeon_Boss_51h : GIL_Dungeon
{
  private HashSet<string> m_playedLines = new HashSet<string>();
  private List<string> m_MinionWarning = new List<string>()
  {
    "VO_GILA_BOSS_51h_Male_Undead_EventPlayMinion_01.prefab:3ac948e98a418e445bcd6c1e1a12d190",
    "VO_GILA_BOSS_51h_Male_Undead_EventPlayMinion_03.prefab:78ec78c78bd8d5a4f9542ca810d780bb",
    "VO_GILA_BOSS_51h_Male_Undead_EventPlayMinion_04.prefab:37cccdba1fae2e24291aab3773c1e2fa"
  };

  public override void PreloadAssets()
  {
    base.PreloadAssets();
    foreach (string soundPath in new List<string>()
    {
      "VO_GILA_BOSS_51h_Male_Undead_Intro_01.prefab:f62d6a72af299064b8e79ffae9ece1dc",
      "VO_GILA_BOSS_51h_Male_Undead_EmoteResponse_01.prefab:5a733778d0078834e8c3610dcf4c9279",
      "VO_GILA_BOSS_51h_Male_Undead_Death_01.prefab:50b65f13e26e81544b99e25ac9af8264",
      "VO_GILA_BOSS_51h_Male_Undead_HeroPower_01.prefab:e54b7a566f812674cb5a060c12c44580",
      "VO_GILA_BOSS_51h_Male_Undead_HeroPower_02.prefab:69e0f8122944f2b458578c005e70de19",
      "VO_GILA_BOSS_51h_Male_Undead_HeroPower_03.prefab:4231df7278f04304b959c2bd30b2de92",
      "VO_GILA_BOSS_51h_Male_Undead_HeroPower_04.prefab:dc48451b3ab43f24b8956ca0d99c6cfc",
      "VO_GILA_BOSS_51h_Male_Undead_EventPlayMinion_01.prefab:3ac948e98a418e445bcd6c1e1a12d190",
      "VO_GILA_BOSS_51h_Male_Undead_EventPlayMinion_03.prefab:78ec78c78bd8d5a4f9542ca810d780bb",
      "VO_GILA_BOSS_51h_Male_Undead_EventPlayMinion_04.prefab:37cccdba1fae2e24291aab3773c1e2fa",
      "VO_GILA_BOSS_51h_Male_Undead_EventPlayCoinPouch_01.prefab:7cb96fee789ac8044994fbabb6061db7"
    })
      this.PreloadSound(soundPath);
  }

  protected override IEnumerator RespondToPlayedCardWithTiming(Entity entity)
  {
    yield return (object) base.RespondToPlayedCardWithTiming(entity);
  }

  protected override List<string> GetBossHeroPowerRandomLines() => new List<string>()
  {
    "VO_GILA_BOSS_51h_Male_Undead_HeroPower_01.prefab:e54b7a566f812674cb5a060c12c44580",
    "VO_GILA_BOSS_51h_Male_Undead_HeroPower_02.prefab:69e0f8122944f2b458578c005e70de19",
    "VO_GILA_BOSS_51h_Male_Undead_HeroPower_03.prefab:4231df7278f04304b959c2bd30b2de92",
    "VO_GILA_BOSS_51h_Male_Undead_HeroPower_04.prefab:dc48451b3ab43f24b8956ca0d99c6cfc"
  };

  protected override string GetBossDeathLine() => "VO_GILA_BOSS_51h_Male_Undead_Death_01.prefab:50b65f13e26e81544b99e25ac9af8264";

  protected override bool GetShouldSupressDeathTextBubble() => true;

  protected override void PlayEmoteResponse(EmoteType emoteType, CardSoundSpell emoteSpell)
  {
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (emoteType == EmoteType.START)
    {
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_51h_Male_Undead_Intro_01.prefab:f62d6a72af299064b8e79ffae9ece1dc", Notification.SpeechBubbleDirection.TopRight, actor));
    }
    else
    {
      if (!MissionEntity.STANDARD_EMOTE_RESPONSE_TRIGGERS.Contains(emoteType))
        return;
      Gameplay.Get().StartCoroutine(this.PlaySoundAndBlockSpeech("VO_GILA_BOSS_51h_Male_Undead_EmoteResponse_01.prefab:5a733778d0078834e8c3610dcf4c9279", Notification.SpeechBubbleDirection.TopRight, actor));
    }
  }

  protected override IEnumerator HandleMissionEventWithTiming(int missionEvent)
  {
    GIL_Dungeon_Boss_51h gilDungeonBoss51h = this;
    while (gilDungeonBoss51h.m_enemySpeaking)
      yield return (object) null;
    Actor actor = GameState.Get().GetOpposingSidePlayer().GetHeroCard().GetActor();
    if (missionEvent == 101)
    {
      string line = gilDungeonBoss51h.PopRandomLineWithChance(gilDungeonBoss51h.m_MinionWarning);
      if (line != null)
        yield return (object) gilDungeonBoss51h.PlayLineOnlyOnce(actor, line);
    }
  }

  protected override IEnumerator RespondToFriendlyPlayedCardWithTiming(Entity entity)
  {
    GIL_Dungeon_Boss_51h gilDungeonBoss51h = this;
    while (gilDungeonBoss51h.m_enemySpeaking)
      yield return (object) null;
    while (entity.GetCardType() == TAG_CARDTYPE.INVALID)
      yield return (object) null;
    if (!gilDungeonBoss51h.m_playedLines.Contains(entity.GetCardId()))
    {
      yield return (object) gilDungeonBoss51h.WaitForEntitySoundToFinish(entity);
      string cardId = entity.GetCardId();
      gilDungeonBoss51h.m_playedLines.Add(cardId);
      Actor actor = GameState.Get().GetOpposingSidePlayer().GetHero().GetCard().GetActor();
      if (cardId == "GILA_816a")
        yield return (object) gilDungeonBoss51h.PlayEasterEggLine(actor, "VO_GILA_BOSS_51h_Male_Undead_EventPlayCoinPouch_01.prefab:7cb96fee789ac8044994fbabb6061db7");
    }
  }
}
